using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Faaliyet Raporu Raporları (eski ButceHazirlikv2/FaaliyetRaporuAl.aspx; menüde "Faaliyet Raporları"): soldaki listeden rapor
    /// seçilir; yıl, gruplama ve YİKOB seçimine göre Göster (PDF yeni sekmede) ya da İndir. Rapor eski kodla üretilir.
    /// İşlemler wwwroot/js/faaliyetraporual.js ile /api/faaliyetraporu uçlarından yapılır.
    /// </summary>
    public class FaaliyetRaporuAlModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FaaliyetRaporuAlModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FaaliyetRaporuUclari.AlYetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
