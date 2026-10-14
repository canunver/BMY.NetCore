using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe gider fişi, e-Bütçe biçimi (eski ButceHazirlikv2/ButceFisiGirisEski.aspx?odenekTur=1): arama kriterlerine uyan tertipler
    /// İlk Tertip / Önceki / Sonraki ile tek tek getirilir; tertibin gerekçesi, farklılaşma sebepleri ve 3 yıllık teklifi girilip saklanır,
    /// kriterlere uyan bütün tertiplerin fişleri Word'e yazdırılır. İşlemler wwwroot/js/butcefisieski.js ile /api/butcefisieski uçlarından yapılır.
    /// </summary>
    public class ButceFisiGirisEskiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ButceFisiGirisEskiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ButceFisiEskiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
