using Istemci.KesinHesap;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KesinHesap
{
    /// <summary>
    /// Gider Cetveli Açıklaması Girişi (eski KesinHesap/FisGiderGerekce.aspx): yıl ve gider tertibi seçilir, tertibin bütçe
    /// değerleri ve gerekçesi okunur / şablondan üretilir / kaydedilir; Word raporu tablolu ya da tablosuz alınır.
    /// İşlemler wwwroot/js/fisgidergerekce.js ile /api/fisgidergerekce uçlarından yapılır.
    /// </summary>
    public class FisGiderGerekceModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FisGiderGerekceModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FisGiderGerekceUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
