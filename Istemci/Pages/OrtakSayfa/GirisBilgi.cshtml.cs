using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.OrtakSayfa
{
    /// <summary>
    /// Girişten sonra açılan sayfa (eski OrtakSayfa/GirisBilgi.aspx): bilgi ekranlarından (portal pencereleri) oluşur.
    /// Sayfa yalnızca iskeleti basar; içerik wwwroot/js/girisbilgi.js tarafından /api/ortaksayfa/giris-bilgi ucundan doldurulur.
    /// </summary>
    public class GirisBilgiModel : PageModel
    {
        private readonly IAntiforgery sahtecilik;

        public GirisBilgiModel(IAntiforgery sahtecilik)
        {
            this.sahtecilik = sahtecilik;
        }

        public string YazilimAd { get; private set; }
        public string IstekJetonu { get; private set; }

        public void OnGet()
        {
            YazilimAd = Ayar.Oku("yazilimAd", "SGB.net");
            IstekJetonu = sahtecilik.GetAndStoreTokens(HttpContext).RequestToken;
        }
    }
}
