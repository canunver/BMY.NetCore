using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Faaliyet/Proje Vize Detayı Girişi (eski ButceHazirlikv2/TavanTertipYatirimVize.aspx): seçilen faaliyet/projenin yatırım vize
    /// tavanının (birim + ekonomik kod + finansman) tertiplere üç yıllık dağıtımı; her yılın onaylı teklifi yanında vize tutarı girilir.
    /// En üstte salt okunur TAVAN, FARK ve TOPLAM satırları, onay işlemleri, Yazdır / Detay Yaz ve Rapor Al menüsü. İşlemler
    /// wwwroot/js/tavantertipvize.js ile /api/tavantertipvize uçlarından yapılır.
    /// </summary>
    public class TavanTertipYatirimVizeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanTertipYatirimVizeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TavanTertipYatirimVizeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
