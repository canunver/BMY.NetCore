using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.HRC;
using TNS.KYM;

namespace Istemci.HarcamaSurecleri
{
    /// <summary>Gelir vergisi tablosunun bir satırı: dilim yüzdesi ve dilimin üst miktarı.</summary>
    public class GelirVergisiSatiri
    {
        public decimal yuzde { get; set; }
        public decimal miktar { get; set; }
    }

    /// <summary>Kaydet isteği: yıl ve tablodaki satırlar (ekrandaki sırayla).</summary>
    public class GelirVergisiKayit
    {
        public int yil { get; set; }
        public List<GelirVergisiSatiri> satirlar { get; set; }
    }

    /// <summary>
    /// Gelir Vergisi Bilgileri Tanımlama (eski HarcamaSurecleri/TanimPSLGelirVergisi.aspx; yeni sayfa HarcamaSurecleri/TanimPSLGelirVergisi) uçları:
    ///   GET  /api/gelirvergisi/listele?yil=   yılın vergi dilimleri (eski Listele)
    ///   POST /api/gelirvergisi/kaydet         { yil, satirlar } (eski btnKaydet_Click)
    /// İş kuralları eski sayfadaki gibi IHRCServis.PSLGelirVergisiListele / PSLGelirVergisiKaydet.
    /// </summary>
    public static class GelirVergisiUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/TanimPSLGelirVergisi.aspx";

        /// <summary>Yıl kutusunun alt sınırı (eski GenelIslemler.YilDoldur(ddlYil, 2008, ...)).</summary>
        private const int IlkYil = 2008;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/gelirvergisi/listele", Listele);
            app.MapPost("/api/gelirvergisi/kaydet", Kaydet);
        }

        /// <summary>Menü yetkisi (eski sayfada GenelIslemler.SayfayaGirisYapabilirMi yorumdaydı; menüdeki Yetki yine de geçerli).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ").Replace("<br/>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata.Trim();
        }

        private static List<int> Yillar(int yil)
        {
            int son = Math.Max(DateTime.Now.Year, yil);
            List<int> yillar = new List<int>();
            for (int i = Math.Min(IlkYil, yil > 0 ? yil : IlkYil); i <= son; i++) yillar.Add(i);
            return yillar;
        }

        private static IResult Listele(HttpContext baglam, IWebHostEnvironment ortam, int? yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int y = yil ?? DateTime.Now.Year;
            List<GelirVergisiSatiri> satirlar = new List<GelirVergisiSatiri>();
            if (y <= 0) return Results.Json(new { basarili = true, yil = y, yillar = Yillar(DateTime.Now.Year), satirlar });

            PSLGelirVergisi kriter = new PSLGelirVergisi();
            kriter.yil = y;
            ObjectArray bilgi = TNS.HRC.Arac.Tanimla().PSLGelirVergisiListele(kullanan, kriter);
            if (!bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc, "Gelir vergisi bilgileri okunamadı."));

            foreach (PSLGelirVergisi g in bilgi.objeler)
                satirlar.Add(new GelirVergisiSatiri { yuzde = g.yuzde, miktar = g.miktar });

            return Results.Json(new { basarili = true, yil = y, yillar = Yillar(y), satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, GelirVergisiKayit istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.satirlar == null) return Results.BadRequest();

            // Eski btnKaydet_Click: yüzdesi 0'dan büyük satırlar ekrandaki sırayla gönderilir; yüzde tam sayıya çevrilirdi (ConvertToInt).
            ObjectArray bilgi = new ObjectArray();
            foreach (GelirVergisiSatiri s in istek.satirlar)
            {
                if (s == null) continue;
                int yuzde = (int)Math.Truncate(s.yuzde);
                if (yuzde <= 0) continue;
                PSLGelirVergisi vergi = new PSLGelirVergisi();
                vergi.yil = istek.yil;
                vergi.yuzde = yuzde;
                vergi.miktar = s.miktar;
                bilgi.objeler.Add(vergi);
            }

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().PSLGelirVergisiKaydet(kullanan, bilgi);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = string.IsNullOrWhiteSpace(sonuc.bilgiStr) ? "Kayıt işlemi başarıyla gerçekleşti." : sonuc.bilgiStr });
        }
    }
}
