using Istemci.Oturum;
using Istemci.StratejikPlan;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.PerformansButce
{
    /// <summary>
    /// Stratejik Plan (eski PerformansButce/StratejikPlanAna.aspx): seçili kurum ve plan döneminin misyonu, vizyonu,
    /// amaçları ve hedefleri. Eski sayfanın pencerede açtığı alt sayfalar (DonemSec, Misyon, Vizyon, StratejikAmac,
    /// StratejikHedef ve onun Göstergeler / Ödenekler pencereleri) burada aynı sayfadaki pencerelerdir.
    /// İşlemler wwwroot/js/stratejikplan.js ile /api/strplan uçlarından yapılır.
    /// </summary>
    public class StratejikPlanAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public StratejikPlanAnaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = StratejikPlanUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
