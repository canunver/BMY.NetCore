using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel Raporları (eski InsanKaynaklari/RaporPersonel.aspx): solda rapor ağacı, sağda seçilen raporun kriterleri ve
    /// Göster (PDF, tarayıcıda) / İndir (Excel-Word) / İndir (PDF) düğmeleri. İşlemler wwwroot/js/raporpersonel.js ile
    /// /api/raporpersonel uçlarından yapılır.
    /// </summary>
    public class RaporPersonelModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public RaporPersonelModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = RaporPersonelUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
