using Istemci.ButceForm;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Ek Formlar (eski ButceHazirlikv2/EkFormAna.aspx). Eski sayfa alt formları (ButceForm/*.aspx) bir çerçevede açıp gizli
    /// düğmelerini tetikliyordu; burada çerçeve yoktur: formlar aynı sayfada çizilir ve aynı iş kurallarını /api/ekform uçlarından
    /// çağırır. İşlemler wwwroot/js/ekform.js (sayfa) ve wwwroot/js/ekform/*.js (formlar) içindedir.
    /// </summary>
    public class EkFormAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public EkFormAnaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = EkFormUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
