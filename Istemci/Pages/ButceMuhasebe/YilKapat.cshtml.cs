using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muhasebe Kayıt İşlemi Tarih Sınırlaması (eski ButceMuhasebe/YilKapat.aspx): verilen tarihten öncesine ait Muhasebe Kayıt
    /// Belgesi girişini engeller (js/yilkapat.js, /api/yilkapat/*).
    /// </summary>
    public class YilKapatModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public YilKapatModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = YilKapatUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
