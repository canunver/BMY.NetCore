using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Maaş Temel Bilgileri Tanımlama (eski HarcamaSurecleri/TanimPSLMaasTemelBilgi.aspx): yılın aylara göre maaş katsayıları,
    /// SGK oranları ve asgari ücret bilgileri. İşlemler wwwroot/js/maastemel.js ile /api/maastemel uçlarından yapılır.
    /// </summary>
    public class TanimPSLMaasTemelBilgiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimPSLMaasTemelBilgiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MaasTemelBilgiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
