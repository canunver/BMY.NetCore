using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Gider Bütçesi Açılış Fişi (eski ButceMuhasebe/ButceAcilisFisi.aspx): bütçe ödeneklerinden (faaliyet bazlı) açılış MİF belgesi
    /// oluşturur (js/butceacilisfisi.js, /api/butceacilis). Yalnızca sistem yöneticisi.
    /// </summary>
    public class ButceAcilisFisiModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ButceAcilisFisiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
