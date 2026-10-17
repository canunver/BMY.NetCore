using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe İşlemi formu (eski ButceHazirlikv2/MasrafPlaniRevize.aspx): masraf planı revize / aktarma / ekleme belgesi; masraf planı onaylı değilse BUM bütçe işlemi olarak kaydedilir. Menüde yoktur, yetki çerçeveden gelir. İşlemler wwwroot/js/mprevize.js ile /api/mprevize uçlarından yapılır.
    /// </summary>
    public class MasrafPlaniRevizeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MasrafPlaniRevizeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPRevizeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, MPRevizeUclari.FormAdres);
        }
    }
}
