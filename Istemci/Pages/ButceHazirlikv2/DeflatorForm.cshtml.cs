using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Yatırım Projeleri Deflatörü (eski ButceHazirlikv2/DeflatorForm.aspx): yıl ve senaryoya göre sektörlerin deflatörleri
    /// tabloda girilir, kaydedilir. İşlemler wwwroot/js/deflator.js ile /api/deflator uçlarından yapılır.
    /// </summary>
    public class DeflatorFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public DeflatorFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = DeflatorUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
