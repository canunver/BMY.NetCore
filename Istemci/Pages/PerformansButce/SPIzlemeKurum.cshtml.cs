using Istemci.Oturum;
using Istemci.StratejikPlan;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.PerformansButce
{
    /// <summary>
    /// Kurum Değerlendirme (eski PerformansButce/SPIzlemeKurum.aspx): kurumun stratejik planındaki hedeflerin plan yıllarına göre
    /// başarım yüzdeleri, hedefin altında göstergelerinin başarımları, hedef açıklaması ve iki Excel raporu.
    /// İşlemler wwwroot/js/spizlemekurum.js ile /api/strkurum uçlarından yapılır.
    /// </summary>
    public class SPIzlemeKurumModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SPIzlemeKurumModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = KurumIzlemeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
