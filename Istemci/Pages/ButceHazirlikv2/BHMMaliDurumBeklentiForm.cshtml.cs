using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Kurumsal Mali Durum ve Beklentiler Raporu (eski ButceHazirlikv2/BHMMaliDurumBeklentiForm.aspx): seçilen yıl ve faaliyet için
    /// dört dönem metni okunur, kaydedilir, Word raporu alınır; sistem yöneticisi yılı veri girişine kapatıp açabilir. İşlemler
    /// wwwroot/js/malidurum.js ile /api/malidurum uçlarından yapılır.
    /// </summary>
    public class BHMMaliDurumBeklentiFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BHMMaliDurumBeklentiFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MaliDurumBeklentiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
