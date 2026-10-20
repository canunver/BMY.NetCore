using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// İmza Tanımları (eski ButceMuhasebe/ImzaTanim.aspx; İmza Bilgileri Tanımlama sayfası Imza.cshtml'den ayrıdır). Ödeme Emri / MİF
    /// formunun Yazdır menüsündeki "+" düğmesinden Sgb.pencere('ImzaTanim', 'İmza Tanımları', kapaninca) ile açılır. İşlemler
    /// js/imzatanim.js ile /api/imzatanim uçlarından. Eski sayfadaki gibi yalnız sistem yöneticisi ve muhasebe yöneticisi girer.
    /// </summary>
    public class ImzaTanimModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = kullanan != null && ImzaTanimUclari.TanimYetkisi(kullanan);
        }
    }
}
