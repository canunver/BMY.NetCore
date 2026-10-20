using System;
using System.Globalization;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri ekranının (eski InsanKaynaklari/PersonelBilgi.aspx) ve sekmelerinde / pencerelerinde açılan alt sayfaların
    /// ortak işleri. Alt sayfalar (PersonelKisisel, PersonelKadro, PersonelAtama, ...) menu.xml'de yoktur; yetkileri üst sayfa
    /// PersonelBilgi'nin menüdeki yetkisinden gelir (SayfaYetki.Yetkili'ye üst sayfa olarak verilir).
    /// </summary>
    public static class PersonelBilgiOrtak
    {
        public const string Klasor = "../InsanKaynaklari/";
        public const string SayfaAdresi = Klasor + "PersonelBilgi.aspx";
        public static readonly string[] UstSayfalar = { SayfaAdresi };

        /// <summary>Ana sayfa (PersonelBilgi) yetkisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        /// <summary>Alt sayfa yetkisi; sayfaAd eski sayfanın uzantısız adı, ör. "PersonelOgrenim".</summary>
        public static bool AltSayfaYetkili(Kullanici kullanan, string uygulamaKlasoru, string sayfaAd)
        {
            if (string.IsNullOrEmpty(sayfaAd) || string.Equals(sayfaAd, "PersonelBilgi", StringComparison.OrdinalIgnoreCase))
                return YetkiVar(kullanan, uygulamaKlasoru);
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Klasor + sayfaAd + ".aspx", UstSayfalar);
        }

        /// <summary>
        /// JSON uçlarının ortak denetimi: oturum yoksa 401, yetki yoksa 403, uygunsa null döner.
        /// sayfaAd: ucun ait olduğu alt sayfanın eski adı (ör. "PersonelOgrenim"); ana sayfa uçları için "PersonelBilgi".
        /// </summary>
        public static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, string sayfaAd, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!AltSayfaYetkili(kullanan, ortam.ContentRootPath, sayfaAd)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>İş kuralı mesajlarındaki HTML'i düz metne çevirir.</summary>
        public static string Duz(string s)
        {
            s = (s ?? "").Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n").Replace("<BR>", "\n")
                .Replace("<b>", "").Replace("</b>", "");
            return s.Trim();
        }

        /// <summary>İş kuralı hata metni; boşsa ya da istisna dökümü içeriyorsa varsayılan metin.</summary>
        public static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return Duz(hata);
        }

        /// <summary>TNSDateTime -> "yyyy-MM-dd" (boşsa "").</summary>
        public static string TarihYaz(TNSDateTime t)
        {
            try
            {
                if (t == null || TNSDateTime.IsNull(t)) return "";
                return TarihYaz(t.Oku());
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>DateTime -> "yyyy-MM-dd" (1900 öncesi boş).</summary>
        public static string TarihYaz(DateTime d)
        {
            return d.Year < 1900 ? "" : d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>"yyyy-MM-dd" ya da "gg.aa.yyyy" -> DateTime (boş/yanlışsa DateTime.MinValue).</summary>
        public static DateTime TarihOku(string s)
        {
            s = (s ?? "").Trim();
            if (s == "") return DateTime.MinValue;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) return d;
            if (DateTime.TryParse(s, new CultureInfo("tr-TR"), DateTimeStyles.None, out d)) return d;
            return DateTime.MinValue;
        }

        /// <summary>Metin -> TNSDateTime (boşsa boş tarih).</summary>
        public static TNSDateTime TnsTarih(string s) { return new TNSDateTime(TarihOku(s)); }

        /// <summary>Sistem yöneticisi mi?</summary>
        public static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }
    }

    /// <summary>
    /// PersonelBilgi'nin sekme / pencere alt sayfalarının ortak sayfa modeli: yetkiyi denetler, adres satırındaki
    /// personelKod / personelTur / tcKimlikNo değerlerini sayfaya verir (içerik JavaScript ile uçlardan gelir).
    /// </summary>
    public abstract class PersonelAltSayfaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected PersonelAltSayfaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        /// <summary>Sayfanın eski adı (uzantısız), ör. "PersonelOgrenim".</summary>
        protected abstract string SayfaAd { get; }

        public bool Yetkili { get; private set; }
        public bool Yonetici { get; private set; }
        public int PersonelKod { get; private set; }
        public string PersonelTur { get; private set; }
        public string TcKimlikNo { get; private set; }

        public virtual void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = PersonelBilgiOrtak.AltSayfaYetkili(kullanan, ortam.ContentRootPath, SayfaAd);
            Yonetici = PersonelBilgiOrtak.SistemYoneticisi(kullanan);
            int.TryParse(Request.Query["personelKod"], out int pk);
            PersonelKod = pk;
            PersonelTur = Request.Query["personelTur"].ToString();
            TcKimlikNo = Request.Query["tcKimlikNo"].ToString();
        }
    }
}
