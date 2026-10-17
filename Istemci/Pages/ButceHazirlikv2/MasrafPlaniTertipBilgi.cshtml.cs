using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Tertip bilgisi (eski ButceHazirlikv2/MasrafPlaniTertipBilgi.aspx): tertibin aylara göre birimlere dağıtılan ve dağıtılmayan masraf planı; Bütçe İşlemi formundaki satırdan pencerede açılır. wwwroot/js/mptertipbilgi.js.
    /// </summary>
    public class MasrafPlaniTertipBilgiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MasrafPlaniTertipBilgiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPRevizeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, MPRevizeUclari.TertipBilgiAdres);
        }
    }
}
