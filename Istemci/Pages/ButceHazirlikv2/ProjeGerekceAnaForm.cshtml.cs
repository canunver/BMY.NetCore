using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Proje Gerekçe Girişi (eski ButceHazirlikv2/ProjeGerekceAnaForm.aspx): yıl, kurum ve sektör seçilir; tavan içi projelerin
    /// gerekçeleri ile yeni tavan dışı projeler iki sekmede girilir (eski ProjeGerekceMevcut.aspx / ProjeGerekceYeni.aspx çerçeveleri).
    /// İşlemler wwwroot/js/projegerekce.js ile /api/projegerekce uçlarından yapılır.
    /// </summary>
    public class ProjeGerekceAnaFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ProjeGerekceAnaFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ProjeGerekceUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
