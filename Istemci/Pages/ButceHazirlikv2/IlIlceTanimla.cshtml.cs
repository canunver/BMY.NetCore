using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// İl ve İlçe Tanımı (eski ButceHazirlikv2/IlIlceTanimla.aspx; sağdaki çerçevede açılan IlTanimlaDetay.aspx ve
    /// IlceTanimlaDetay.aspx bu sayfanın formuna alındı): solda il / ilçe ağacı, sağda seçilenin bilgileri.
    /// İşlemler wwwroot/js/ililce.js ile /api/ililce uçlarından yapılır.
    /// </summary>
    public class IlIlceTanimlaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public IlIlceTanimlaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = IlIlceUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
