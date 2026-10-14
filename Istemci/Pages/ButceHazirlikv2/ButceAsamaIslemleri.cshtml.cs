using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe Aşama İşlemleri (eski ButceHazirlikv2/ButceAsamaIslemleri.aspx): birim için her bütçe aşamasının (senaryonun)
    /// onay durumu (Kapalı / Açık / Yöneticilere Açık / Onaylı) ve bir aşamanın gider / gelir ödeneklerini başka aşamaya kopyalama.
    /// İşlemler wwwroot/js/butceasama.js ile /api/butceasama uçlarından yapılır.
    /// </summary>
    public class ButceAsamaIslemleriModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ButceAsamaIslemleriModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ButceAsamaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
