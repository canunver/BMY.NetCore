using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Unvan Bilgileri (eski InsanKaynaklari/TanimUnvan.aspx): solda unvan listesi, sağda seçilen unvanın bilgileri.
    /// İşlemler wwwroot/js/unvan.js ile /api/unvan uçlarından yapılır.
    /// </summary>
    public class TanimUnvanModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimUnvanModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = UnvanUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
