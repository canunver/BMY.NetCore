using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.OrtakSayfa
{
    /// <summary>
    /// Sistem Dosyaları Sürüm Bilgisi (eski OrtakSayfa/SurumBilgi.aspx): uygulama klasöründeki dll dosyaları ve sürümleri
    /// (js/surumbilgi.js, /api/surumbilgi/liste).
    /// </summary>
    public class SurumBilgiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SurumBilgiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SurumBilgiUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
