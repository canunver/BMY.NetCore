using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// SGB - KBS Fark (Faal.) (eski ButceMuhasebe/SGBKBSDuzeltmePEB.aspx): SGB ile KBS harcamaları (ya da gönderilen ödenek ile KBS
    /// harcaması) arasındaki farkları faaliyet kırılımında listeler; seçili satırlar için ters kayıt yapar. İşlemler
    /// wwwroot/js/sgbkbsfark.js ile /api/sgbkbsfark uçlarından (Muhasebe/SgbKbsFarkUclari.cs) yapılır.
    /// </summary>
    public class SGBKBSDuzeltmePEBModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SGBKBSDuzeltmePEBModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SgbKbsFarkUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
