using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Ana Harcama Grupları Tavan Girişi (eski ButceHazirlikv2/TavanEkoSektorBirim.aspx): seçilen ekonomik kod ve sektör için
    /// birimlerin üç yıllık tavanları; en üstte salt okunur toplam satırı, karşılaştırma tutarları ve artış yüzdeleri.
    /// İşlemler wwwroot/js/taveneko.js ile /api/tavaneko uçlarından yapılır.
    /// </summary>
    public class TavanEkoSektorBirimModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanEkoSektorBirimModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TavanEkoSektorBirimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
