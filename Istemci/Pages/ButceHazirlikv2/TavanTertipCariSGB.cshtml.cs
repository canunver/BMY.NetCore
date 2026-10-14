using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Faaliyet Detay Teklif Girişi - SGB (eski ButceHazirlikv2/TavanTertipCariSGB.aspx): seçilen faaliyet/projenin SGB görüşme (cari)
    /// tavanının tertiplere (kurumsal-fonksiyonel-finansman-ekonomik) üç yıllık görüşme sonrası dağıtımı ve ek ödenekleri; en üstte salt
    /// okunur TAVAN, FARK ve TOPLAM satırları, onay işlemleri, Tekliften Al, Kopyala / Yapıştır ve Rapor Al menüsü. İşlemler
    /// wwwroot/js/tavantertipsgb.js ile /api/tavantertipsgb uçlarından yapılır.
    /// </summary>
    public class TavanTertipCariSGBModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanTertipCariSGBModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TavanTertipCariSGBUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
