using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe İşlemi çerçevesi (eski ButceHazirlikv2/MasrafPlaniButceIslemFrame.aspx): "Form" (MasrafPlaniRevize) ve "Belge Arama" (MasrafPlaniRevizeSorgu) sekmeleri; aramadan açılan belge Form sekmesinde okunur.
    /// </summary>
    public class MasrafPlaniButceIslemFrameModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MasrafPlaniButceIslemFrameModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPRevizeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
