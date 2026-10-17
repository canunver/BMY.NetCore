using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe İşlemi belge arama (eski ButceHazirlikv2/MasrafPlaniRevizeSorgu.aspx): kıstaslara uyan formlar, durum değiştirme (yönetici) ve raporlar. Menüde yoktur, yetki çerçeveden gelir. wwwroot/js/mprevizesorgu.js.
    /// </summary>
    public class MasrafPlaniRevizeSorguModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MasrafPlaniRevizeSorguModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPRevizeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, MPRevizeUclari.SorguAdres);
        }
    }
}
