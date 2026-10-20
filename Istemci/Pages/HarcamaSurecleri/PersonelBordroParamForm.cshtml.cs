using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Personel Bordro Parametreleri (eski HarcamaSurecleri/PersonelBordroParamForm.aspx): bordroda kullanılan kişi bilgisi
    /// değişkenleri. İşlemler wwwroot/js/bordroparam.js ile /api/bordroparam uçlarından yapılır.
    /// </summary>
    public class PersonelBordroParamFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public PersonelBordroParamFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = BordroParamUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
