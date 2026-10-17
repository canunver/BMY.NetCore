using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.OrtakSayfa
{
    /// <summary>
    /// Bir menü başlığının bütün seçenekleri (eski OrtakSayfa/SayfaMenu.aspx?menuID=...): sol menüde panel yerine ayrı sayfada
    /// gösterilen kalabalık menüler (menu.xml: SayfadaGoster). Sayfa yalnızca iskeleti basar; seçenekler wwwroot/js/sayfamenu.js ile
    /// /api/ortaksayfa/sayfa-menu ucundan okunur (yetki denetimi uçtadır). Her seçeneğin yeni sisteme taşınıp taşınmadığı da gösterilir.
    /// </summary>
    public class SayfaMenuModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
