using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Yolluk Planlama Formu (eski ButceHazirlikv2/YollukPlanlamaForm.aspx): seçilen faaliyetin yolluk tertiplerine eğitim /
    /// toplantı planlarının (daire, konu, süre, kişi sayısı, ay, yer, maliyet) girilmesi; yolluk değerleri ve birimlerin onay
    /// durumu (sistem yöneticisi). İşlemler wwwroot/js/yolluk.js ile /api/yolluk uçlarından yapılır.
    /// </summary>
    public class YollukPlanlamaFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public YollukPlanlamaFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = YollukPlanlamaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
