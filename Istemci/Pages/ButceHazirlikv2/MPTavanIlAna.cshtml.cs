using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// İl / İlçe Ödenek Dağıtım (eski ButceHazirlikv2/MPTavanIlAna.aspx): il kullanıcısının saymanlık / birimine ait kurumsal yapının
    /// tertipleri, tavanı ve birimlere dağıtılan tutarı listelenir. İl izni açık tertipte tutara tıklanınca Tavan Girişi (MPTavanGiris,
    /// DG=I) pencerede açılır. Tavan / Detay raporları alınır. İşlemler wwwroot/js/mptavanil.js ile /api/mptavanil uçlarından yapılır.
    /// </summary>
    public class MPTavanIlAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MPTavanIlAnaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPTavanIlUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
