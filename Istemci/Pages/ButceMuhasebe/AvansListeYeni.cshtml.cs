using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Avans Kapatma (Avans Listesi), eski ButceMuhasebe/AvansListeYeni.aspx: yıl, saymanlık, birim, TC kimlik / vergi no ve ad ile
    /// kişi / firma bazında verilen, alınan ve kalan avanslar listelenir, Excel'e yazdırılır (js/avansliste.js, /api/avans/liste).
    /// </summary>
    public class AvansListeYeniModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public AvansListeYeniModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = AvansUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, AvansUclari.ListeSayfa);
        }
    }
}
