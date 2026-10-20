using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Terfiye Hak Kazananlar (eski InsanKaynaklari/TerfiyeHakKazananlar.aspx).
    /// İşlemler wwwroot/js/terfi.js ile /api/terfi uçlarından (TerfiUclari) yapılır.
    /// </summary>
    public class TerfiyeHakKazananlarModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TerfiyeHakKazananlarModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TerfiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, TerfiUclari.HakKazananSayfa);
        }
    }
}
