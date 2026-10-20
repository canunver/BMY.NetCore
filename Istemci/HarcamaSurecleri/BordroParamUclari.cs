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
    /// <summary>Bordro parametresi satırı: alan adları BordroGenelDegisken sınıfındaki gibi (eski stoBordroParam alanları).</summary>
    public class BordroParamSatiri
    {
        public string degiskenAd { get; set; }
        public string aciklama { get; set; }
        public string sira { get; set; }
        public int yeni { get; set; }
        public string veriTipiStr { get; set; }
        public int yontem { get; set; }
        public string liste { get; set; }
    }

    /// <summary>
    /// Personel Bordro Parametreleri (eski HarcamaSurecleri/PersonelBordroParamForm.aspx; yeni sayfa HarcamaSurecleri/PersonelBordroParamForm) uçları:
    ///   GET  /api/bordroparam/oku       parametre listesi (eski Oku)
    ///   POST /api/bordroparam/kaydet    tablodaki bütün satırlar (eski Kaydet; yeni=1 eklenir, yeni=0 güncellenir)
    /// İş kuralları eski sayfadaki gibi IHRCServis.ListelePersonelBordroParam / KaydetPersonelBordroParam.
    /// </summary>
    public static class BordroParamUclari
    {
        public const string SayfaAdresi = "../HarcamaSurecleri/PersonelBordroParamForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/bordroparam/oku", Oku);
            app.MapPost("/api/bordroparam/kaydet", Kaydet);
        }

        /// <summary>Eski Page_Load: GenelIslemler.SayfayaGirisYapabilirMi (menü yetkisi).</summary>
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

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            ObjectArray oa = TNS.HRC.Arac.Tanimla().ListelePersonelBordroParam(kullanan);
            if (!oa.sonuc.islemSonuc) return Hata(HataMetni(oa.sonuc, "Bordro parametreleri okunamadı."));

            List<BordroParamSatiri> satirlar = new List<BordroParamSatiri>();
            foreach (BordroGenelDegisken d in oa.objeler)
                satirlar.Add(new BordroParamSatiri
                {
                    degiskenAd = d.degiskenAd,
                    aciklama = d.aciklama,
                    sira = d.sira,
                    yeni = d.yeni,
                    veriTipiStr = d.veriTipiStr,
                    yontem = d.yontem,
                    liste = d.liste
                });
            return Results.Json(new { basarili = true, satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, List<BordroParamSatiri> satirlar, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (satirlar == null) return Results.BadRequest();

            // Eski Kaydet: önbellekteki değişken listesi sıfırlanır, tablodaki satırlar BordroGenelDegisken'e çevrilir.
            BordroGenelDegisken.DegiskenleriSifirla();
            TNSCollection degiskenler = new TNSCollection();
            foreach (BordroParamSatiri s in satirlar)
            {
                if (s == null) continue;
                BordroGenelDegisken d = new BordroGenelDegisken();
                d.degiskenAd = s.degiskenAd;
                d.aciklama = s.aciklama;
                d.sira = s.sira;
                d.yeni = s.yeni;
                d.veriTipiStr = s.veriTipiStr;
                d.yontem = s.yontem;
                d.liste = s.liste;
                degiskenler.Add(d);
            }

            Sonuc sonuc = TNS.HRC.Arac.Tanimla().KaydetPersonelBordroParam(kullanan, degiskenler);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt yapılamadı."));
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }
    }
}
