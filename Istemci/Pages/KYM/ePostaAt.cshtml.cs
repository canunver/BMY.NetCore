using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Kullanıcılara e-posta Gönderme (eski KYM/ePostaAt.aspx): seçilen kullanıcı grubundaki kullanıcılara konu ve mesajı gönderilmek
    /// üzere kaydeder. Sayfa yalnızca iskeleti basar; işlemler wwwroot/js/epostaat.js ile /api/eposta uçlarından yapılır.
    /// </summary>
    public class ePostaAtModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ePostaAtModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = EPostaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
