using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Hizmet Sebepleri (eski InsanKaynaklari/HizmetSebebiForm.aspx): solda arama ve liste, sağda seçilen sebebin bilgileri.
    /// İşlemler wwwroot/js/hizmetsebebi.js ile /api/hizmetsebebi uçlarından yapılır.
    /// </summary>
    public class HizmetSebebiFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public HizmetSebebiFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = HizmetSebebiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
