using Istemci.Oturum;
using Istemci.StratejikPlan;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.PerformansButce
{
    /// <summary>
    /// İzleme Tablosu (eski PerformansButce/StratejikGostGerc.aspx): kurumun stratejik planındaki bir hedefin göstergelerinin
    /// altı aylık izleme ya da yıl sonu değerlendirme gerçekleşmeleri girilir; izleme / değerlendirme raporları alınır.
    /// İşlemler wwwroot/js/stratejikgostgerc.js ile /api/strizleme uçlarından yapılır.
    /// </summary>
    public class StratejikGostGercModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public StratejikGostGercModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = IzlemeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
