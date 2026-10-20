using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Modüller (eski KYM/Moduller.aspx): modüllerin kullanım durumu; menü buna göre oluşur (js/moduller.js, /api/moduller/*).
    /// </summary>
    public class ModullerModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ModullerModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ModulUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
