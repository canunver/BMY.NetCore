using System;
using System.Globalization;
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

namespace Istemci.Muhasebe
{
    public class YilKapatIstegi
    {
        /// <summary>Kapatma tarihi (yyyy-MM-dd); boşsa sınırlama kalkar (eski sayfadaki gibi boş değer saklanır).</summary>
        public string Tarih { get; set; }
    }

    /// <summary>
    /// Muhasebe Kayıt İşlemi Tarih Sınırlaması (eski ButceMuhasebe/YilKapat.aspx): verilen tarihten öncesine ait Muhasebe Kayıt
    /// Belgesi girişini engeller. Tarih, "stratek" kullanıcısının MIFTARIHKAPAT değişkeninde saklanır; MUH iş kuralı
    /// (MUHOdemeEmriMIFServis.TarihKapaliMi) kaydederken bu değişkene bakar.
    ///   GET  /api/yilkapat/tanim    { tarih (yyyy-MM-dd), tarihMetin (saklanan değer) }
    ///   POST /api/yilkapat/kaydet   { Tarih } : eski btnKaydet_Click ("İşleme Kapat") → { basarili, bilgi, tarih, tarihMetin } / { hata }
    /// Yetki: menüdeki seçenek (eski SayfayaGirisYapabilirMi).
    /// </summary>
    public static class YilKapatUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/YilKapat.aspx";
        private const string DegiskenAd = "MIFTARIHKAPAT";
        private const string KapatKullanici = "stratek";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yilkapat/tanim", Tanim);
            app.MapPost("/api/yilkapat/kaydet", Kaydet);
        }

        public static bool Yetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>Eski Page_Load'daki kapatma kullanıcısı.</summary>
        private static Kullanici KapatKullanici_()
        {
            return new Kullanici(KapatKullanici, "", "", GenelIslemlerIstemci.VarsayilanKurumBul(), KapatKullanici, "ltd", new TNSDateTime(), "", "",
                new TNSDateTime(), ENUMAktiflik.AKTIF, "", "", "", new TNSDateTime(), "", "");
        }

        private static object Durum()
        {
            string saklanan = GenelIslemler.KullaniciDegiskenGetir(KapatKullanici, DegiskenAd) ?? "";
            TNSDateTime t = new TNSDateTime(saklanan);
            string iso = t.isNull || t.Oku() == DateTime.MinValue ? "" : t.Oku().ToString("yyyy-MM-dd");
            return new { tarih = iso, tarihMetin = saklanan };
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            return Results.Json(Durum());
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, YilKapatIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;

            string s = (istek?.Tarih ?? "").Trim();
            TNSDateTime dt;
            if (s == "") dt = new TNSDateTime();
            else if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t)) dt = new TNSDateTime(t);
            else return Results.Json(new { basarili = false, hata = "Tarih bilgisi geçersiz." });

            try
            {
                Sonuc sonuc = GenelIslemler.KullaniciDegiskenSakla(KapatKullanici_(), DegiskenAd, dt.ToString());
                if (sonuc != null && !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = string.IsNullOrWhiteSpace(sonuc.hataStr) ? "Kayıt yapılamadı." : sonuc.hataStr });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = ex.Message });
            }
            return Results.Json(new { basarili = true, bilgi = "Muhasebe Belgesi Kaydetme işi, belirtilen tarihten öncesi için kapatılmıştır.", durum = Durum() });
        }
    }
}
