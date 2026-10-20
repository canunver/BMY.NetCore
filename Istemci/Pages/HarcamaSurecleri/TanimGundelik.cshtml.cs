using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Yıl, Unvan ve Ülke Kriterine Göre Gündelik Tanımlama (eski HarcamaSurecleri/TanimGundelik.aspx):
    /// üstte form, altta gündelik listesi. İşlemler wwwroot/js/unvangundelik.js ile /api/unvangundelik uçlarından yapılır.
    /// </summary>
    public class TanimGundelikModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimGundelikModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = UnvanGundelikUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
