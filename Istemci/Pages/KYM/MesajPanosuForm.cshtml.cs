using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Sistem Mesajları bilgi ekranı (eski KYM/MesajPanosuForm.aspx): kullanıcıya yazılmış sistem mesajlarını listeler.
    /// Sayfa yalnızca iskeleti basar; mesajlar wwwroot/js/mesajpanosu.js ile /api/mesajpanosu/liste'den gelir.
    /// </summary>
    public class MesajPanosuFormModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
