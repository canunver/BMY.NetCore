using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Masraf Planı Dağıtım (eski ButceHazirlikv2/MPDagitim.aspx; menüde "Masraf Planı Tavan"): bir tertip için dağıtılacak miktar
    /// kaydedilir, birimler eklenir (seçerek ya da Excel'den tutarlarıyla), birimlerin geçen yıl gerçekleşme / bu yıl planlanan /
    /// atanacak miktarları görülür ve seçilen ay için masraf planı revizesi üretilir. İşlemler wwwroot/js/mpdagitim.js ile
    /// /api/mpdagitim uçlarından yapılır.
    /// </summary>
    public class MPDagitimModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MPDagitimModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPDagitimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
