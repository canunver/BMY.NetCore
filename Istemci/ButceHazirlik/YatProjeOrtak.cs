using System;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    /// <summary>
    /// Yatırım Projeleri ekranının (eski ButceHazirlikv2/ProjeGiris.aspx) ve içinde açılan alt sayfaların ortak işleri.
    /// Alt sayfalar (ProjeDetay, AltProjeDetay, AltProjeAmacGiris, ...) menu.xml'de yoktur; yetkileri üst sayfa ProjeGiris'in
    /// menüdeki yetkisinden gelir (SayfaYetki.Yetkili'ye üst sayfa olarak verilir). Menüde ProjeGiris iki seçenekle vardır:
    /// Yatırım Projeleri (ProjeGiris.aspx) ve İzleme Değerlendirme (ProjeGiris.aspx?islemKod=1); ikisinden birini görebilen
    /// alt sayfalara girer.
    /// </summary>
    public static class YatProjeOrtak
    {
        public const string Klasor = "../ButceHazirlikv2/";
        public const string UstSayfa = Klasor + "ProjeGiris.aspx";
        public const string UstSayfaIzleme = Klasor + "ProjeGiris.aspx?islemKod=1";

        /// <summary>Kullanıcı alt sayfayı (ör. "AltProjeAmacGiris") kullanabilir mi?</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string sayfaAd)
        {
            // ProjeGiris'in kendisi menüde iki adresle var; ikisinden birini görebilen ekranı ve uçlarını kullanır.
            if (string.Equals(sayfaAd, "ProjeGiris", StringComparison.OrdinalIgnoreCase))
                return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, UstSayfa) || SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, UstSayfaIzleme);
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Klasor + sayfaAd + ".aspx", UstSayfa, UstSayfaIzleme);
        }

        /// <summary>İstek için kullanıcıyı ve sayfa yetkisini denetler; uygun değilse döndürülecek yanıtı verir.</summary>
        public static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, string sayfaAd, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, sayfaAd)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>POST istekleri için: istek jetonu, kullanıcı ve sayfa yetkisi.</summary>
        public static async Task<(IResult hata, Kullanici kullanan)> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, string sayfaAd)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return (Results.BadRequest(), null);
            IResult ret = Denetle(baglam, ortam, sayfaAd, out Kullanici kullanan);
            return (ret, kullanan);
        }

        public static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        public static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        /// <summary>Sunucudan gelen hata metni; boşsa ya da istisna ayrıntısı içeriyorsa varsayılan metin.</summary>
        public static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        public static string Temiz(string deger)
        {
            return (deger ?? "").Trim();
        }
    }

    /// <summary>
    /// ProjeGiris'in alt sayfalarının sayfa modeli: yalnızca yetkiyi denetler (sayfa içeriği JavaScript ile uçlardan gelir).
    /// </summary>
    public abstract class YatAltSayfaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected YatAltSayfaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        /// <summary>Sayfanın eski adı (uzantısız), ör. "AltProjeAmacGiris".</summary>
        protected abstract string SayfaAd { get; }

        public bool Yetkili { get; private set; }
        public bool Yonetici { get; private set; }

        public void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = YatProjeOrtak.YetkiVar(kullanan, ortam.ContentRootPath, SayfaAd);
            Yonetici = YatProjeOrtak.SistemYoneticisi(kullanan);
        }
    }
}
