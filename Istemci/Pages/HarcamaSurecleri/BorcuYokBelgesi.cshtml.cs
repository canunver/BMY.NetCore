using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Borcu Yoktur Belgesi / Vergi Borcu Sorgulama Girişi (eski HarcamaSurecleri/BorcuYokBelgesi.aspx). Menüde yoktur: Vergi Borcu
    /// Sorgulama (BorcuYokAna) ekranında ve Ödeme Emri/MİF Belgesi formundan pencere olarak açılır
    /// (?muhasebe=&amp;birim=&amp;belgeYil=&amp;belgeNo=&amp;vNo=; eski Belge Arama'nın kullandığı ?kod=&amp;tip=1|2&amp;muhasebe= de geçerlidir).
    /// İşlemler wwwroot/js/borcuyokbelgesi.js ile /api/borcuyok uçlarından (HarcamaSurecleri/BorcuYokBelgesiUclari.cs) yapılır.
    /// </summary>
    public class BorcuYokBelgesiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BorcuYokBelgesiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = BorcuYokBelgesiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
