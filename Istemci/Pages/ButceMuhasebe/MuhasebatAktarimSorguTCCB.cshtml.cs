using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muhasebat aktarım belgeleri araması (eski ButceMuhasebe/MuhasebatAktarimSorguTCCB.aspx): çerçevenin "Belge Arama" sekmesi (js/muhasebataktarimliste.js, /api/muhasebataktarim/liste).
    /// Yetki: menüdeki çerçeve sayfası ve ödeme emri görme yetkisi (Muhasebe/MuhasebatAktarimUclari.cs).
    /// </summary>
    public class MuhasebatAktarimSorguTCCBModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MuhasebatAktarimSorguTCCBModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MuhasebatAktarimUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
