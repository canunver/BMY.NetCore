using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Birim Bütçe Hazırlığı çerçevesi (eski ButceHazirlikv2/BirimButceHazirlikFormAna.aspx): "Mevcut Proje/Faaliyetler"
    /// (BirimButceHazirlikForm?yeni=0) ve "Diğer Proje/Faaliyetler" (yeni=1) sekmeleri. Sekmeler birbirinin kaydedilmemiş
    /// değişikliğini ve "teklif yoktur" seçimini wwwroot/js/birimbutcehazirlikcerceve.js üzerinden sorar.
    /// </summary>
    public class BirimButceHazirlikFormAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BirimButceHazirlikFormAnaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = BirimButceHazirlikUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
