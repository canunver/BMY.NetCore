using Istemci.ButceForm;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceForm
{
    /// <summary>
    /// Faaliyet Gerekçesi (eski ButceForm/HizmetGerekcesiv2.aspx): birimin faaliyetleri listelenir, seçilen faaliyetin
    /// gerekçe, dayanak ve maliyet metinleri okunur / önceki yıldan alınır / kaydedilir; Word raporu tek ya da tüm faaliyetler için.
    /// İşlemler wwwroot/js/hizmetgerekce.js ile /api/hizmetgerekce uçlarından yapılır.
    /// </summary>
    public class HizmetGerekcesiv2Model : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public HizmetGerekcesiv2Model(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = HizmetGerekcesiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
