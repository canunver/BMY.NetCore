using Istemci.ButceForm;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceForm
{
    /// <summary>
    /// Deprem Harcamaları Bilgi Formu (eski ButceForm/DepremHarcamaForm.aspx): seçilen yıl / ay, muhasebe birimi ve il için
    /// ekonomik kodlara göre bütçe içi ve bütçe dışı (özel hesap, döner sermaye, fon, sosyal tesis, diğer) deprem harcamaları
    /// okunur, girilir ve kaydedilir; Excel raporları; sistem yöneticisi için onay işlemleri ve e-bütçeye aktarım.
    /// İşlemler wwwroot/js/depremharcama.js ile /api/depremharcama uçlarından yapılır.
    /// </summary>
    public class DepremHarcamaFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public DepremHarcamaFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = DepremHarcamaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
