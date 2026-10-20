using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// İş Tanımları, eski ButceMuhasebe/TanimIs.aspx: solda sorgu ve iş listesi, sağda iş bilgileri ile Ödenek Bloke, Muhasebe Fişleri,
    /// Ödenek Durum ve Taahhütler sekmeleri (eski TanimIsButceFisleri, TanimIsMuhasebeFisleri, TanimIsOdenekDurum, TanimIsTaahhutler).
    /// İstemci: js/tanimis.js, uçlar /api/tanimis/* (Muhasebe/TanimIsUclari.cs).
    /// </summary>
    public class TanimIsModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimIsModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TanimIsUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
