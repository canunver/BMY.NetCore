using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Masraf Planı Düzenleme (eski ButceHazirlikv2/MasrafPlaniYatirimAyarla.aspx; Masraf Planı ekranındaki "MP Düzelt"): aynı
    /// kodlu iki tertipten birinin aylık tutarları düşülüp diğerine eklenir; masraf planı, AHP ve tavanlar doğrudan veri tabanında
    /// değiştirilir. Yalnızca sistem yöneticisi. İşlemler wwwroot/js/mpayarla.js ile /api/mpayarla uçlarından yapılır.
    /// </summary>
    public class MasrafPlaniYatirimAyarlaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MasrafPlaniYatirimAyarlaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPAyarlaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
