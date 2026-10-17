using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödenek Durum Bilgisi (Birim) (eski ButceMuhasebe/OdenekDurumHarcamaBirim.aspx): seçilen muhasebe / harcama birimi ve tertip için
    /// ödenek gönderme / tenkis ve harcama bilgileri, birimlere göre liste; Excel raporu (MUH-OdenekDurum.xlt).
    /// İşlemler wwwroot/js/odenekdurum.js ile /api/odenekdurum uçlarından (Muhasebe/OdenekDurumUclari.cs) yapılır.
    /// </summary>
    public class OdenekDurumHarcamaBirimModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public OdenekDurumHarcamaBirimModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = OdenekDurumUclari.BirimYetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
