using Istemci.ButceUygulama;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceUygulama
{
    /// <summary>
    /// Günlük Nakit İhtiyacı (eski ButceUygulama/GunlukNakitIhtiyaci.aspx): seçilen muhasebe birimi için tarihin 5 iş günü öncesi ve
    /// sonrası günlük nakit ihtiyacı toplamları listelenir; ileri tarihli iş günlerinin tutarları harcama türlerine göre girilir;
    /// bütün birimlerin Excel raporu alınır. İşlemler wwwroot/js/gunluknakit.js ile /api/gunluknakit uçlarından yapılır.
    /// </summary>
    public class GunlukNakitIhtiyaciModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public GunlukNakitIhtiyaciModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GunlukNakitIhtiyaciUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
