using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödenek Gönderme/Tenkis Bilgisi (eski ButceMuhasebe/OdenekGondermeTenkisEkonomik.aspx): seçilen yıl, muhasebe / harcama birimi ve
    /// ekonomik kod için tertip başına onaysız / onaylı ödenek gönderme-tenkis, harcama ve kullanılabilir tutar.
    /// Menüde yoktur; Ödenek Gönderme / Tenkis formundan ?muhasebe=&amp;birim= ile açılır. İşlemler wwwroot/js/ogtekonomik.js ile
    /// /api/ogtekonomik uçlarından (Muhasebe/OdenekGondermeEkonomikUclari.cs) yapılır.
    /// </summary>
    public class OdenekGondermeTenkisEkonomikModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public OdenekGondermeTenkisEkonomikModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = OdenekGondermeEkonomikUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
