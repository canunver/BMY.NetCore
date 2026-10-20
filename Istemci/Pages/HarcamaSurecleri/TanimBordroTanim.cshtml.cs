using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Bordro Tanım Ekranı (eski HarcamaSurecleri/TanimBordroTanim.aspx): bordro türü ve işlem türüne göre değişkenler, formüller,
    /// ortak fonksiyonlar ve ortak şablonlar. İşlemler wwwroot/js/bordrotanim.js ile /api/bordrotanim uçlarından yapılır.
    /// </summary>
    public class TanimBordroTanimModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimBordroTanimModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = BordroTanimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
