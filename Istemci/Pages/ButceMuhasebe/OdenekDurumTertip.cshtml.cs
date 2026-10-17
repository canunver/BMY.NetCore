using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödenek Durum Bilgisi (eski ButceMuhasebe/OdenekDurumTertip.aspx): seçilen yıl ve tertip için ödenek, ödenek gönderme / tenkis,
    /// harcama ve AHP bilgileri; Excel raporu (MUH-OdenekDurum.xlt).
    /// İşlemler wwwroot/js/odenekdurum.js ile /api/odenekdurum uçlarından (Muhasebe/OdenekDurumUclari.cs) yapılır.
    /// </summary>
    public class OdenekDurumTertipModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public OdenekDurumTertipModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = OdenekDurumUclari.TertipYetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
