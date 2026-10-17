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
    /// Bütçe Hazırlık > Tanımlar sütunundaki sayfaların (Proje Türü, Döküman Türü, Karakteristikler, Kalkınma Bölgeleri,
    /// Eski Kurumsal Yapı, Bütçe Hazırlık Parametreleri, Bütçe Sözlük İşlemleri, Program / Alt Program / Faaliyet / Alt Faaliyet,
    /// Alt Faaliyet Bağları, BÜMKO Aktarım) ortak işleri. Her sayfanın yetkisi menu.xml'deki kendi adresinden gelir.
    /// </summary>
    public static class ButceTanimOrtak
    {
        public const string Klasor = "../ButceHazirlikv2/";

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, params string[] adresler)
        {
            foreach (string adres in adresler)
                if (SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, adres)) return true;
            return false;
        }

        /// <summary>İstek için kullanıcıyı ve sayfa yetkisini denetler; uygun değilse döndürülecek yanıtı verir.</summary>
        public static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan, params string[] adresler)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, adresler)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>POST istekleri için: istek jetonu, kullanıcı ve sayfa yetkisi.</summary>
        public static async Task<(IResult hata, Kullanici kullanan)> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, params string[] adresler)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return (Results.BadRequest(), null);
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan, adresler);
            return (ret, kullanan);
        }

        public static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        /// <summary>İşlem sonucu: { basarili, bilgi } ya da { basarili: false, hata }.</summary>
        public static IResult SonucYaz(TNS.Sonuc s, string basariMetni, string varsayilanHata)
        {
            if (s == null || !s.islemSonuc)
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(s == null ? null : s.hataStr, varsayilanHata) });
            return Results.Json(new { basarili = true, bilgi = basariMetni });
        }

        public static IResult Basarisiz(string hata)
        {
            return Results.Json(new { basarili = false, hata = hata });
        }

        public static string Temiz(string deger)
        {
            return (deger ?? "").Trim();
        }

        public static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }
    }

    /// <summary>Tanım sayfalarının sayfa modeli: yalnızca yetkiyi denetler (içerik JavaScript ile uçlardan gelir).</summary>
    public abstract class ButceTanimSayfaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected ButceTanimSayfaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        /// <summary>Sayfanın menu.xml'deki adresleri (biri yeterli).</summary>
        protected abstract string[] Adresler { get; }

        public bool Yetkili { get; private set; }
        public bool Yonetici { get; private set; }

        public virtual void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = ButceTanimOrtak.YetkiVar(kullanan, ortam.ContentRootPath, Adresler);
            Yonetici = ButceTanimOrtak.SistemYoneticisi(kullanan);
        }
    }
}
