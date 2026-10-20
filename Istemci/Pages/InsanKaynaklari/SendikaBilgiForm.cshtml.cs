using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Sendika Bilgisi (eski InsanKaynaklari/SendikaBilgiForm.aspx): solda sendika listesi, sağda seçilen sendika ve üye personeli.
    /// İşlemler wwwroot/js/sendika.js ile /api/sendika uçlarından; personel ortak Sgb.personelSec penceresiyle eklenir.
    /// </summary>
    public class SendikaBilgiFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SendikaBilgiFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SendikaBilgiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
