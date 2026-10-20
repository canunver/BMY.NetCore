using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Sayıştay Verileri (eski ButceMuhasebe/SayistayVerileri.aspx): solda Sayıştay'a gönderilecek tablolar, sağda seçilen tablonun
    /// kuralları; banka hesapları ve sayım tutanakları tabloya girilip saklanır, Dosya Hazırla dosyayı indirir. Sayfa yalnızca
    /// iskeleti basar; işlemler wwwroot/js/sayistayverileri.js ile /api/sayistay uçlarından (Muhasebe/SayistayVerileriUclari.cs) yapılır.
    /// </summary>
    public class SayistayVerileriModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SayistayVerileriModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SayistayVerileriUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
