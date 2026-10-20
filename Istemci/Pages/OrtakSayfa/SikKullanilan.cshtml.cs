using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.OrtakSayfa
{
    /// <summary>
    /// Sık Kullanılanlar bilgi ekranı (eski Dashboard/Genel/SikKullanilan.ascx): kullanıcının sık kullanılan ekranlarını modüle göre listeler.
    /// Sayfa yalnızca iskeleti basar; liste wwwroot/js/sikkullanilan.js ile /api/ortaksayfa/sik-kullanilan-liste'den gelir.
    /// </summary>
    public class SikKullanilanModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
