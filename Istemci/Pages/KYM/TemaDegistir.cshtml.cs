using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Tema Değiştirme (eski KYM/TemaDegistir.aspx): kullanıcı renk temasını ve üst şerit görünümünü seçer.
    /// Sayfa yalnızca iskeleti basar; temalar wwwroot/js/temadegistir.js ile GET /api/ortaksayfa/tema ucundan okunur,
    /// seçim POST /api/ortaksayfa/tema ile kullanıcı değişkenine (TEMACORE, eski uygulama için TEMA) yazılır.
    /// </summary>
    public class TemaDegistirModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
