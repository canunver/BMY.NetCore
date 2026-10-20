using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Bordro Tabloları (eski HarcamaSurecleri/BordroTabloForm.aspx): solda tablolar, sağda seçilen tablonun ünvan ve yıla göre
    /// derece değerleri. İşlemler wwwroot/js/bordrotablo.js ile /api/bordrotablo uçlarından yapılır.
    /// </summary>
    public class BordroTabloFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BordroTabloFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = BordroTabloUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
