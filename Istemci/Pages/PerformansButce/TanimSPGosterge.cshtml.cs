using Istemci.Oturum;
using Istemci.StratejikPlan;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.PerformansButce
{
    /// <summary>
    /// Stratejik plan gösterge tanımları (eski PerformansButce/TanimPerformansGostergeYeni.aspx): solda göstergeler,
    /// sağda seçili göstergenin adı, ölçü birimi, türü, ilerleme yönü ve ondalık sayısı. Stratejik Plan'ın Hedef Gösterge
    /// penceresinden de açılır. İşlemler wwwroot/js/tanimspgosterge.js ile /api/spgosterge uçlarından yapılır.
    /// </summary>
    public class TanimSPGostergeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimSPGostergeModel(IWebHostEnvironment ortam)
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
