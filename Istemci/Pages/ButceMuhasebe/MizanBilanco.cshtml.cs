using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muhasebe Raporları (eski ButceMuhasebe/MizanBilanco.aspx): solda rapor listesi, sağda seçilen raporun kuralları; Yazdır
    /// raporu eski şablonlarla (RaporSablon/MUH) Excel / metin dosyası olarak indirir. Sayfa yalnızca iskeleti basar; işlemler
    /// wwwroot/js/mizanbilanco.js ile /api/muhrapor uçlarından (Muhasebe/MizanBilancoUclari.cs) yapılır.
    /// </summary>
    public class MizanBilancoModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MizanBilancoModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MizanBilancoUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
