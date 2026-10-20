using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Firma Bilgisi Düzenleme (eski ButceMuhasebe/FirmaBilgisiDuzenleme.aspx): aynı firma/kişinin kayıtları tek ana kayıtta
    /// birleştirilir (js/firmaduzenleme.js, /api/firmaduzenleme/*). ?muhasebe= verilirse arama o muhasebe birimiyle yapılır.
    /// </summary>
    public class FirmaBilgisiDuzenlemeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FirmaBilgisiDuzenlemeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public string Hata { get; private set; }

        public void OnGet()
        {
            Yetkili = FirmaBilgisiDuzenlemeUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, out string hata);
            Hata = hata;
        }
    }
}
