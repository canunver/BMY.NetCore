using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    /// <summary>Kaydet isteği: alan adları Sozluk sınıfındaki gibi.</summary>
    public class SozlukIstegi
    {
        /// <summary>Sayfanın açıldığı anaKayitKod (yetki denetimi için; boş olabilir).</summary>
        public string anaKayitKod { get; set; }
        public int kod { get; set; }
        /// <summary>Ana kayıt seçimi: "kod:kayitKod" ya da boş (kök).</summary>
        public string anaKayit { get; set; }
        public string kayitKod { get; set; }
        public string ad { get; set; }
        public string kisaAd { get; set; }
        public int seviye { get; set; }
        public string hitapID { get; set; }
    }

    /// <summary>
    /// Sözlük Bilgileri (eski InsanKaynaklari/TanimSozluk.aspx; yeni sayfa InsanKaynaklari/TanimSozluk) uçları:
    ///   GET  /api/sozluk/liste?anaKayitKod=   sözlük kayıtları + ana kayıt seçenekleri (eski Listele + AnaKayitDoldur)
    ///   POST /api/sozluk/kaydet               (eski btnKaydet_Click)
    /// anaKayitKod verilince yalnızca o kayıt kodu ile başlayan kayıtlar gelir (menüde Askerlik, Öğrenim, Dil ... seçenekleri).
    /// Yetki: açılan adresin (anaKayitKod'lu) menü yetkisi; adres menüde yoksa parametresiz Sözlük Bilgileri seçeneğinin yetkisi.
    /// İş kuralları eski sayfadaki gibi IIKYServis.SozlukListele / SozlukKaydet. Eski sayfadaki Sil tuşu gizliydi (Hidden="true"),
    /// taşınmadı.
    /// </summary>
    public static class SozlukUclari
    {
        public const string SayfaAdresi = "../InsanKaynaklari/TanimSozluk.aspx";
        private static readonly Regex KayitKodBicimi = new Regex(@"^[0-9.]{0,40}$");

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/sozluk/liste", Liste);
            app.MapPost("/api/sozluk/kaydet", Kaydet);
        }

        /// <summary>anaKayitKod yalnızca rakam ve noktadan oluşabilir.</summary>
        public static bool KodGecerli(string anaKayitKod)
        {
            return KayitKodBicimi.IsMatch(anaKayitKod ?? "");
        }

        /// <summary>Eski Page_Load: menü yetkisi (adres anaKayitKod parametresiyle).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string anaKayitKod)
        {
            if (!KodGecerli(anaKayitKod)) return false;
            string adres = string.IsNullOrEmpty(anaKayitKod) ? SayfaAdresi : SayfaAdresi + "?anaKayitKod=" + anaKayitKod;
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, adres, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, string anaKayitKod, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!KodGecerli(anaKayitKod)) return Results.BadRequest();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, anaKayitKod)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string anaKayitKod)
        {
            anaKayitKod = (anaKayitKod ?? "").Trim();
            IResult ret = Denetle(baglam, ortam, anaKayitKod, out Kullanici kullanan);
            if (ret != null) return ret;

            Sozluk kriter = new Sozluk();
            kriter.kayitKod = anaKayitKod;
            ObjectArray oa = TNS.IKY.Arac.Tanimla().SozlukListele(kriter);
            if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc.hataStr, "Sözlük bilgileri okunamadı.") });

            List<object> satirlar = new List<object>();
            // Eski AnaKayitDoldur: ilk seçenek kök (boş), sonra kayıtlar seviyesi kadar "- " ile girintili.
            List<object> anaKayitlar = new List<object>();
            anaKayitlar.Add(new { deger = "", ad = "(Kök - ana kayıt yok)" });
            string varsayilanAna = "";
            string anaAd = "";
            foreach (Sozluk s in oa.objeler)
            {
                satirlar.Add(new { s.kod, s.anaKod, s.kayitKod, s.ad, s.kisaAd, s.seviye, s.hitapID, s.anaKayitAd });
                string girinti = "";
                for (int i = 1; i < s.seviye; i++) girinti += "- ";
                string deger = s.kod + ":" + s.kayitKod;
                anaKayitlar.Add(new { deger = deger, ad = girinti + s.ad });
                // Eski Listele: anaKayitKod verilince ana kayıt o kayıt olarak seçilir.
                if (anaKayitKod != "" && s.kayitKod == anaKayitKod)
                {
                    varsayilanAna = deger;
                    anaAd = s.ad;
                }
            }

            string bilgi = oa.objeler.Count == 0 ? Duz(oa.sonuc.bilgiStr) : "";
            return Results.Json(new { satirlar = satirlar, anaKayitlar = anaKayitlar, varsayilanAna = varsayilanAna, anaAd = anaAd, bilgi = bilgi });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, SozlukIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, (istek.anaKayitKod ?? "").Trim(), out Kullanici kullanan);
            if (ret != null) return ret;

            if ((istek.ad ?? "").Trim() == "")
                return Results.Json(new { basarili = false, hata = "Kayıt Adı boş bırakılamaz" });

            string anaKayit = (istek.anaKayit ?? "").Trim();
            if (anaKayit != "" && anaKayit.IndexOf(':') < 0) return Results.BadRequest();

            // Eski btnKaydet_Click ile aynı alan aktarımı.
            Sozluk d = new Sozluk();
            d.kod = istek.kod;
            if (d.kod > 0)
                d.anaKod = OrtakFonksiyonlar.ConvertToInt(anaKayit.Split(':')[0], 0);
            d.anaKayit = anaKayit;
            d.ad = istek.ad ?? "";
            d.kisaAd = istek.kisaAd ?? "";
            d.seviye = istek.seviye;
            d.hitapID = istek.hitapID ?? "";
            d.kayitKod = istek.kayitKod ?? "";

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().SozlukKaydet(kullanan, d);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Sözlük bilgisi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", kod = d.kod });
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
