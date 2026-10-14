using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Faaliyet/Proje Detay Teklif Girişi, SGB yatırım (eski ButceHazirlikv2/TavanTertipYatirimSGB.aspx): seçilen faaliyet/projenin
    /// ekonomik kod + sektör + birim tavanının (senaryo 0100) tertiplere görüşme sonrası üç yıllık dağıtımı ve ek ödenekleri; birimin
    /// teklifi yanında salt okunur durur. En üstte salt okunur TAVAN, FARK ve TOPLAM satırları, onay işlemleri, Tekliften Al ve
    /// Rapor Al menüsü. İşlemler wwwroot/js/tavantertipyatirimsgb.js ile /api/tavantertipyatirimsgb uçlarından yapılır.
    /// </summary>
    public class TavanTertipYatirimSGBModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanTertipYatirimSGBModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TavanTertipYatirimSGBUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
