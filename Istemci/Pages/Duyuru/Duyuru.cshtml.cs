using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Duyuru
{
    /// <summary>
    /// Duyuru ekranı (eski Duyuru/Duyuru.aspx): açılış sayfasında bilgi ekranı olarak ve "pencerede göster" duyuruları için
    /// açılış penceresinde gösterilir. Sayfa yalnızca iskeleti basar; duyurular wwwroot/js/duyuru.js ile /api/duyuru/goster'den gelir.
    /// </summary>
    public class DuyuruModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
