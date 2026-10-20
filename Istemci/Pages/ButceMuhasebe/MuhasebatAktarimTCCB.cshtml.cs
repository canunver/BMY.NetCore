using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muhasebat aktarım belgesi (eski ButceMuhasebe/MuhasebatAktarimTCCB.aspx): çerçevenin "Belge Düzenleme" sekmesi. Mizandan satır oluşturma, kaydet, onay / iptal, Muhasebat'a gönder, yazdır (js/muhasebataktarim.js, /api/muhasebataktarim).
    /// Yetki: menüdeki çerçeve sayfası ve ödeme emri görme yetkisi (Muhasebe/MuhasebatAktarimUclari.cs).
    /// </summary>
    public class MuhasebatAktarimTCCBModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MuhasebatAktarimTCCBModel(IWebHostEnvironment ortam)
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
