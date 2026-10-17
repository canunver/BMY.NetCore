using Istemci.KesinHesap;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.SayistayBulgu
{
    /// <summary>
    /// Sayıştay Ön Bulgu Cevapları (eski SayistayBulgu/SayBulguCevap.aspx): seçilen birimin ilişkili olduğu bulgulara birim cevabı,
    /// yapılacak işler, öngörülen tamamlanma tarihi girilir; ekli dosyalar ve Excel listesi alınır. İşlemler
    /// wwwroot/js/saybulgucevap.js ile /api/saybulgu uçlarından yapılır.
    /// </summary>
    public class SayBulguCevapModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SayBulguCevapModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SayBulguUclari.CevapYetkisi(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
