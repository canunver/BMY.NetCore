using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages
{
    /// <summary>Uygulamanın kökü: giriş yapılmışsa açılış sayfasına yönlendirir (giriş yapılmamışsa zaten giriş sayfası açılır).</summary>
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Redirect(Url.Content("~/" + IstemciBaslangic.AcilisSayfasi));
        }
    }
}
