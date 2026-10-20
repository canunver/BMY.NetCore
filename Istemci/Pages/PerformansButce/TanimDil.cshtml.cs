using Istemci.Oturum;
using Istemci.StratejikPlan;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.PerformansButce
{
    /// <summary>
    /// Stratejik plan dil tanımları (eski PerformansButce/TanimDil.aspx): solda diller, sağda seçilenin adı.
    /// İşlemler wwwroot/js/tanimspdil.js ile /api/spdil uçlarından yapılır.
    /// </summary>
    public class TanimDilModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimDilModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SPDilUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
