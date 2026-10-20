using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Gelir Vergisi Bilgileri Tanımlama (eski HarcamaSurecleri/TanimPSLGelirVergisi.aspx): yılın gelir vergisi dilimleri (yüzde, miktar).
    /// İşlemler wwwroot/js/gelirvergisi.js ile /api/gelirvergisi uçlarından yapılır.
    /// </summary>
    public class TanimPSLGelirVergisiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimPSLGelirVergisiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GelirVergisiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
