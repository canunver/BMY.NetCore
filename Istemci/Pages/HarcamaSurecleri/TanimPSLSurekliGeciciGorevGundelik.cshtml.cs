using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Geçici / Sürekli Görev Gündelik Tanım Formu (eski HarcamaSurecleri/TanimPSLSurekliGeciciGorevGundelik.aspx):
    /// üstte tanım formu, altta tanımlar listesi. İşlemler wwwroot/js/gundeliktanim.js ile /api/gundeliktanim uçlarından yapılır.
    /// </summary>
    public class TanimPSLSurekliGeciciGorevGundelikModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimPSLSurekliGeciciGorevGundelikModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GundelikTanimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
