using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.OrtakSayfa
{
    /// <summary>
    /// Girişten sonra açılan sayfa (eski OrtakSayfa/GirisBilgi.aspx): bilgi ekranlarından (portal pencereleri) oluşur.
    /// Sayfa yalnızca iskeleti basar; içerik wwwroot/js/girisbilgi.js tarafından /api/ortaksayfa/giris-bilgi ucundan doldurulur.
    /// </summary>
    public class GirisBilgiModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
