using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    public class EbutceAktarIstegi
    {
        public int Yil { get; set; }
        /// <summary>1-12</summary>
        public int Ay { get; set; }
        /// <summary>harcama: Harcama Gerçekleşmelerini aktar; odenek: Ödenek Servisi Çalıştır</summary>
        public string Islem { get; set; }
    }

    public class EbutceParolaIstegi
    {
        public string Kullanici { get; set; }
        /// <summary>Boş gelirse kayıtlı parola değiştirilmez.</summary>
        public string Parola { get; set; }
    }

    /// <summary>
    /// e-bütçe Aktarımları (eski ButceMuhasebe/Aktar.aspx) uçları:
    ///   GET  /api/ebutceaktar/tanim    yıl, aylar, kayıtlı e-bütçe kullanıcısı (parola tarayıcıya gönderilmez, yalnızca kayıtlı olup olmadığı)
    ///   POST /api/ebutceaktar/aktar    {Yil, Ay, Islem}: harcama → servis HarcamaAktar; odenek → OdenekGondermeAktarimIslemleri
    ///   POST /api/ebutceaktar/parola   {Kullanici, Parola}: "stratek" kullanıcısının EBUTCEKULLANICI / EBUTCEPAROLA değişkenleri (eski btnEbutceParola_Click)
    /// </summary>
    public static class EbutceAktarUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/Aktar.aspx";
        private const string DegiskenKullanici = "stratek";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ebutceaktar/tanim", Tanim);
            app.MapPost("/api/ebutceaktar/aktar", Aktar);
            app.MapPost("/api/ebutceaktar/parola", Parola);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
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

        private static IResult Yanit(bool basarili, string metin)
        {
            return Results.Json(new { basarili = basarili, bilgi = basarili ? metin : null, hata = basarili ? null : metin });
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<int> yillar = new List<int>();
            for (int i = 2006; i <= DateTime.Now.Year + 1; i++) yillar.Add(i);
            CultureInfo tr = new CultureInfo("tr-TR");
            List<object> aylar = new List<object>();
            for (int i = 1; i <= 12; i++) aylar.Add(new { kod = i, ad = tr.TextInfo.ToTitleCase(tr.DateTimeFormat.GetMonthName(i)) });
            return Results.Json(new
            {
                yil = DateTime.Now.Year,
                ay = DateTime.Now.Month,
                yillar = yillar,
                aylar = aylar,
                ebutceKullanici = GenelIslemler.KullaniciDegiskenGetir(DegiskenKullanici, "EBUTCEKULLANICI"),
                parolaVar = GenelIslemler.KullaniciDegiskenGetir(DegiskenKullanici, "EBUTCEPAROLA") != ""
            });
        }

        private static async Task<IResult> Aktar(HttpContext baglam, IWebHostEnvironment ortam, EbutceAktarIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil <= 0) return Yanit(false, "Yıl giriniz.");
            IMUHServis servis = TNS.MUH.Arac.Tanimla();
            if (istek.Islem == "odenek")
            {
                // Eski BHMOdGondSer
                TNS.Sonuc s;
                try
                {
                    s = servis.OdenekGondermeAktarimIslemleri(kullanan, istek.Yil);
                }
                catch (Exception ex)
                {
                    s = new TNS.Sonuc(ex.Message);
                }
                return Results.Json(new { basarili = s.islemSonuc, sonuc = s.hataStr ?? "" });
            }

            // Eski HarcamaAktar
            if (istek.Ay < 1 || istek.Ay > 12) return Yanit(false, "Ay seçiniz.");
            try
            {
                TNS.Sonuc sonuc = servis.HarcamaAktar(kullanan, istek.Yil.ToString(), istek.Ay.ToString(), GenelIslemlerIstemci.VarsayilanKurumBul());
                if (sonuc.islemSonuc) return Yanit(true, "Aktarım tamamlandı.");
                return Yanit(false, string.IsNullOrWhiteSpace(sonuc.hataStr) ? "Aktarım yapılamadı." : sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Yanit(false, ex.Message);
            }
        }

        private static async Task<IResult> Parola(HttpContext baglam, IWebHostEnvironment ortam, EbutceParolaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Yanit(false, "Bilgi yok.");
            try
            {
                Kullanici kaydetKul = new Kullanici(DegiskenKullanici, "", "", GenelIslemlerIstemci.VarsayilanKurumBul(), DegiskenKullanici, "ltd", new TNSDateTime(), "", "",
                    new TNSDateTime(), ENUMAktiflik.AKTIF, "", "", "", new TNSDateTime(), "", "");
                GenelIslemler.KullaniciDegiskenSakla(kaydetKul, "EBUTCEKULLANICI", (istek.Kullanici ?? "").Trim());
                string parola = (istek.Parola ?? "").Trim();
                if (parola != "") GenelIslemler.KullaniciDegiskenSakla(kaydetKul, "EBUTCEPAROLA", parola);
                return Yanit(true, "Kullanıcı değişkenine saklandı.");
            }
            catch (Exception ex)
            {
                return Yanit(false, ex.Message);
            }
        }
    }
}
