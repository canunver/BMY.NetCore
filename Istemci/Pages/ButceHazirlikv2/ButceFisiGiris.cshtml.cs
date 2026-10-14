using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe gider / gelir fişi (eski ButceHazirlikv2/ButceFisiGiris.aspx?odenekTur=1|2): sorgu alanlarına uyan tertipler listelenir,
    /// seçilen tertibin fişi sağda gösterilir; gerekçe düzenlenir, gösterilen ya da işaretli tertiplerin fişleri Word'e yazdırılır.
    /// İşlemler wwwroot/js/butcefisi.js ile /api/butcefisi uçlarından yapılır.
    /// </summary>
    public class ButceFisiGirisModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ButceFisiGirisModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int OdenekTur { get; private set; }

        public void OnGet(int odenekTur = 1)
        {
            OdenekTur = odenekTur == 2 ? 2 : 1;
            Yetkili = ButceFisiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, OdenekTur);
        }
    }
}
