using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// e-bütçe Aktarımları (eski ButceMuhasebe/Aktar.aspx): harcama gerçekleşmelerinin e-bütçeye aktarımı ya da ödenek servisinin
    /// çalıştırılması; e-bütçe kullanıcı adı / parolası saklanır (js/ebutceaktar.js, /api/ebutceaktar).
    /// </summary>
    public class AktarModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public AktarModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = EbutceAktarUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
