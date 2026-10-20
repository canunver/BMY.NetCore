using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet / Sil isteği: alan adları Unvan sınıfındaki gibi.</summary>
    public class UnvanIstegi
    {
        public string kod { get; set; }
        public string ad { get; set; }
        public string sinifAdKisa { get; set; }
        public int sinifKod { get; set; }
        public string siraNo { get; set; }
        public bool yonetici { get; set; }
        public bool aktif { get; set; }
        public string hitapKod { get; set; }
    }

    /// <summary>
    /// Unvan Bilgileri (eski InsanKaynaklari/TanimUnvan.aspx; yeni sayfa InsanKaynaklari/TanimUnvan) uçları:
    ///   GET  /api/unvan/siniflar                         Sınıf seçenekleri (eski SınıfDoldur: sözlük kayıt kodu 22, seviye 2)
    ///   GET  /api/unvan/liste?kod=&amp;ad=&amp;sinifAdKisa=&amp;siraNo=   (eski Listele / btnAra_Click)
    ///   POST /api/unvan/kaydet                           (eski btnKaydet_Click)
    ///   POST /api/unvan/sil                              (eski btnSil_Click)
    /// İş kuralları eski sayfadaki gibi IIKYServis.UnvanListele / UnvanKaydet / UnvanSil, SozlukListele.
    /// </summary>
    public static class UnvanUclari
    {
        public const string SayfaAdresi = "../InsanKaynaklari/TanimUnvan.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/unvan/siniflar", Siniflar);
            app.MapGet("/api/unvan/liste", Liste);
            app.MapPost("/api/unvan/kaydet", Kaydet);
            app.MapPost("/api/unvan/sil", Sil);
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

        /// <summary>Eski SınıfDoldur + Genel.SozlukYukle: ilk seçenek boş, alt seviyeler "- " ile girintili.</summary>
        private static IResult Siniflar(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            Sozluk kriter = new Sozluk();
            kriter.kayitKod = ((int)ENUMSozlukUnsurlari.KADROSINIF).ToString().PadLeft(2, '0');
            kriter.seviye = 2;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().SozlukListele(kriter);

            List<object> siniflar = new List<object>();
            siniflar.Add(new { KOD = 0, AD = "" });
            if (!oa.sonuc.islemSonuc) return Results.Json(new { siniflar = siniflar, hata = HataMetni(oa.sonuc.hataStr, "Sınıf bilgileri okunamadı.") });
            foreach (Sozluk s in oa.objeler)
            {
                string ad = "";
                for (int i = 2; i < s.seviye; i++) ad += "- ";
                siniflar.Add(new { KOD = s.kod, AD = ad + s.ad });
            }
            return Results.Json(new { siniflar = siniflar });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string kod, string ad, string sinifAdKisa, string siraNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski btnAra_Click ile aynı kriter aktarımı (Temizle / açılışta boş kriter).
            Unvan kriter = new Unvan();
            kriter.kod = OrtakFonksiyonlar.ConvertToInt((kod ?? "").Trim(), 0);
            kriter.ad = (ad ?? "").Trim();
            kriter.sinifAdKisa = (sinifAdKisa ?? "").Trim();
            kriter.siraNo = OrtakFonksiyonlar.ConvertToInt((siraNo ?? "").Trim(), 0);

            ObjectArray oa = TNS.IKY.Arac.Tanimla().UnvanListele(kriter);
            if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc.hataStr, "Unvan bilgileri okunamadı.") });

            List<object> satirlar = new List<object>();
            foreach (Unvan u in oa.objeler)
                satirlar.Add(new { u.kod, u.ad, u.sinifAdKisa, u.siraNo, u.sinifKod, u.sinifAd, u.yonetici, u.yoneticiAd, u.aktif, u.aktifAd, u.hitapKod });

            string bilgi = oa.objeler.Count == 0 ? Duz(oa.sonuc.bilgiStr) : "";
            return Results.Json(new { satirlar = satirlar, bilgi = bilgi });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, UnvanIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            // Eski btnKaydet_Click denetimleri.
            string hata = "";
            if (OrtakFonksiyonlar.ConvertToInt((istek.siraNo ?? "").Trim(), 0) == 0)
                hata = "Sıra No boş bırakılamaz\n";
            if ((istek.kod ?? "").Trim() == "")
                hata += "Unvan Kod bilgisi boş bırakılamaz\n";
            if ((istek.ad ?? "").Trim() == "")
                hata += "Unvan bilgisi boş bırakılamaz\n";
            if (hata != "") return Results.Json(new { basarili = false, hata = hata.Trim() });

            Unvan d = new Unvan();
            d.kod = OrtakFonksiyonlar.ConvertToInt(istek.kod.Trim(), 0);
            d.ad = istek.ad ?? "";
            d.sinifAdKisa = istek.sinifAdKisa ?? "";
            d.siraNo = OrtakFonksiyonlar.ConvertToInt(istek.siraNo.Trim(), 0);
            d.yonetici = istek.yonetici ? 1 : 0;
            d.sinifKod = istek.sinifKod;
            d.aktif = istek.aktif;
            d.hitapKod = OrtakFonksiyonlar.ConvertToInt((istek.hitapKod ?? "").Trim(), 0);

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().UnvanKaydet(kullanan, d);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Unvan bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, UnvanIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int kod = OrtakFonksiyonlar.ConvertToInt((istek.kod ?? "").Trim(), 0);
            if (kod <= 0) return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz unvanı seçin." });

            Unvan d = new Unvan();
            d.kod = kod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().UnvanSil(kullanan, d);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Unvan silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }

        private static string Duz(string s)
        {
            s = (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Replace("<b>", "").Replace("</b>", "");
            return s.Trim();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
        }
    }
}
