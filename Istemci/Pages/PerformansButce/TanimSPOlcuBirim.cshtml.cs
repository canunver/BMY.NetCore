using Istemci.Oturum;
using Istemci.StratejikPlan;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.PerformansButce
{
    /// <summary>
    /// Stratejik plan ölçü birimleri (eski PerformansButce/TanimOlcuBirimYeni.aspx): solda ölçü birimleri, sağda seçilenin adı.
    /// Gösterge Tanımları sayfasından da açılır. İşlemler wwwroot/js/tanimspolcubirim.js ile /api/spgosterge/olcu-* uçlarından yapılır.
    /// </summary>
    public class TanimSPOlcuBirimModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimSPOlcuBirimModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SPGostergeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
