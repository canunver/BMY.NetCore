using Istemci.ButceForm;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceForm
{
    /// <summary>
    /// Kiralık Taşıt Planlama Formu (eski ButceForm/AracKiralamaPlaniForm.aspx): muhasebe birimlerinin önceki yıl, bu yıl ve sonraki
    /// yıl için kiraladığı / kiralayacağı taşıtlar (adet, süre, aylık kira bedeli); okuma, kaydetme, onaylama ve Excel raporları.
    /// Sistem yöneticisi bütün birimlerin durumunu görüp topluca onaylayabilir. İşlemler wwwroot/js/arackiralama.js ile
    /// /api/arackiralama uçlarından yapılır.
    /// </summary>
    public class AracKiralamaPlaniFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public AracKiralamaPlaniFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = AracKiralamaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
