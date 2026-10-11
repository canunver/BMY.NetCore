using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.OrtakSayfa
{
    /// <summary>
    /// Grafik gösterme sayfası (eski OrtakSayfa/GrafikGoster.aspx?param=...): başka sayfaların çerçeve ya da pencere içinde açtığı grafik.
    /// Sayfa yalnızca kutuyu basar; grafik wwwroot/js/grafikgoster.js ile /api/grafik ucundan okunur (yetki denetimi uçtadır)
    /// ve Ext JS yerine ortak.js'teki Sgb.grafik ile çizilir.
    /// </summary>
    public class GrafikGosterModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
