using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.PEB;

namespace Istemci.StratejikPlan
{
    using Dil = TNS.PEB.Dil;

    public class SPDilIstegi
    {
        /// <summary>Dil kodu; yeni dilde 0.</summary>
        public int Kod { get; set; }
        public string Ad { get; set; }
    }

    /// <summary>
    /// Stratejik plan dil tanımları (eski PerformansButce/TanimDil.aspx; yeni sayfa PerformansButce/TanimDil) uçları:
    ///   GET  /api/spdil/liste
    ///   POST /api/spdil/kaydet, /api/spdil/sil
    /// İş kuralları eski sayfadaki gibi IPEBServis.DilListele / DilKaydet / DilSil.
    /// </summary>
    public static class SPDilUclari
    {
        public const string SayfaAdresi = "../PerformansButce/TanimDil.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/spdil/liste", Liste);
            app.MapPost("/api/spdil/kaydet", Kaydet);
            app.MapPost("/api/spdil/sil", Sil);
        }

        /// <summary>Menüde Dil Tanımlama sayfasını görebilen ya da Stratejik Plan sayfasını kullanabilen kullanıcı.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (StratejikPlanUclari.YetkiVar(kullanan, uygulamaKlasoru)) return true;
            try
            {
                MenuOkuyucu menu = new MenuOkuyucu(kullanan);
                return menu.SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            ObjectArray liste = TNS.PEB.Arac.Tanimla().DilListele(kullanan, new Dil());
            if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(liste.sonuc, "Diller okunamadı.") });

            List<object> satirlar = new List<object>();
            foreach (Dil d in liste.objeler)
                satirlar.Add(new { kod = d.kod, ad = d.ad });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, SPDilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (string.IsNullOrWhiteSpace(istek.Ad)) return Hata("Dilin adını yazınız.");

            Dil dil = new Dil(Math.Max(0, istek.Kod), istek.Ad.Trim());
            Sonuc sonuc = TNS.PEB.Arac.Tanimla().DilKaydet(kullanan, dil);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Dil kaydedilemedi."));
            return Results.Json(new { basarili = true, bilgi = "Dil kaydedildi.", kod = OrtakFonksiyonlar.ConvertToInt((sonuc.anahtar ?? "").Trim(), dil.kod) });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, SPDilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();

            Sonuc sonuc = TNS.PEB.Arac.Tanimla().DilSil(kullanan, new Dil(istek.Kod, ""));
            // Stratejik planda kullanılmış dil silinmez; iş kuralının uyarısı olduğu gibi gösterilir.
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Dil silinemedi."));
            return Results.Json(new { basarili = true, bilgi = "Dil silindi." });
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }
    }
}
