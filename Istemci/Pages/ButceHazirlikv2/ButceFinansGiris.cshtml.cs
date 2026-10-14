using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Finansman teklifi (eski ButceHazirlikv2/ButceFinansGiris.aspx): varsayılan kurumun gider ve gelir toplamları gösterilir, finansman
    /// tutarı girilip saklanır; Haz. Yar. Hesapla finansman farkından hazine yardımlarını hesaplar, Yazdır Form 18'i verir.
    /// İşlemler wwwroot/js/butcefinans.js ile /api/butcefinans uçlarından yapılır.
    /// </summary>
    public class ButceFinansGirisModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ButceFinansGirisModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ButceFinansUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
