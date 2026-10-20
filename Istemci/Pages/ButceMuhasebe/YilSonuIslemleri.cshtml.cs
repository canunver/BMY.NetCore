using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Yıl Sonu Kapatma İşlemleri (eski ButceMuhasebe/YilSonuIslemleri.aspx): yıl sonu kapanış belgelerini listeler, seçilen
    /// türün belgesini oluşturur ya da oluşacak satırları Excel'e raporlar (js/yilsonuislemleri.js, /api/yilsonu).
    /// </summary>
    public class YilSonuIslemleriModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = YilSonuIslemleriUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
