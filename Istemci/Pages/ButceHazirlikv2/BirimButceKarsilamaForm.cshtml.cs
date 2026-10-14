using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Proje Teklifi Talep Karşılama (eski ButceHazirlikv2/BirimButceKarsilamaForm.aspx): seçilen taşra tertibi için illerin ve
    /// birimlerin harcama, masraf planı, talep ve karşılanan tutarları; karşılananın alt birimlere / tertiplere dağıtılması ve
    /// birim tertiplerinin karşılananlarının girilmesi. İşlemler wwwroot/js/birimkarsilama.js ile /api/birimkarsilama uçlarından yapılır.
    /// </summary>
    public class BirimButceKarsilamaFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BirimButceKarsilamaFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = BirimButceKarsilamaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
