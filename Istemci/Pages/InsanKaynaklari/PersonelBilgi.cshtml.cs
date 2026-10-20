using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri (eski InsanKaynaklari/PersonelBilgi.aspx): solda personel arama ve listesi, sağda seçilen personelin
    /// özeti ve bilgileri sekmeler hâlinde (her sekme ayrı bir alt sayfa, çerçeve içinde). İşlemler wwwroot/js/personelbilgi.js
    /// ile /api/personelbilgi uçlarından yapılır.
    /// </summary>
    public class PersonelBilgiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public PersonelBilgiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = PersonelBilgiOrtak.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
