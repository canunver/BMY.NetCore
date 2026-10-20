using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Sistemi Kullanan Kişilerin Listesi (eski KYM/CalisanKullaniciListe.aspx): verilen dakika içinde sayfa açan kullanıcıları
    /// listeler (js/calisankullanici.js, /api/calisankullanici/liste).
    /// </summary>
    public class CalisanKullaniciListeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public CalisanKullaniciListeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = CalisanKullaniciUclari.ListeYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
