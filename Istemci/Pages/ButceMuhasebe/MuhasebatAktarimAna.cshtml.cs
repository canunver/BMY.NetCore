using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muhasebat Aktarım İşlemleri ana çerçevesi (eski ButceMuhasebe/MuhasebatAktarimAna.aspx): "Belge Düzenleme" (MuhasebatAktarimTCCB) ve "Belge Arama" (MuhasebatAktarimSorguTCCB) sekmeleri (js/muhasebataktarimcerceve.js).
    /// Yetki: menüdeki çerçeve sayfası ve ödeme emri görme yetkisi (Muhasebe/MuhasebatAktarimUclari.cs).
    /// </summary>
    public class MuhasebatAktarimAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MuhasebatAktarimAnaModel(IWebHostEnvironment ortam)
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
