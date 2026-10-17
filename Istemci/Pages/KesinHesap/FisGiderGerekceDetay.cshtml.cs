using Istemci.KesinHesap;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KesinHesap
{
    /// <summary>
    /// Gider Cetveli Detay Açıklamalar (eski KesinHesap/FisGiderGerekceDetay.aspx, "Gider Cetveli Açıklama Girişi (Faaliyet Bazında)"):
    /// yıl ve kurumsal / ekonomik / faaliyet kodlarına göre tertipler listelenir; seçilen tertibin K.B.Ö. ve harcanan tutarı ile
    /// açıklaması okunur, kaydedilir, SGB'ye gönderilir; Word raporu alınır. İşlemler wwwroot/js/fisgidergerekcedetay.js ile
    /// /api/fisgidergerekcedetay uçlarından yapılır.
    /// </summary>
    public class FisGiderGerekceDetayModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FisGiderGerekceDetayModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FisGiderGerekceDetayUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
