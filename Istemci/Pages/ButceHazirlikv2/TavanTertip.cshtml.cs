using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Faaliyet/Proje Detay Teklif Girişi (eski ButceHazirlikv2/TavanTertip.aspx): seçilen faaliyet/projenin ekonomik kod + sektör + birim
    /// tavanının tertiplere (kurumsal-fonksiyonel-finansman-ekonomik) üç yıllık dağıtımı ve ek ödenek teklifleri; en üstte salt okunur
    /// TAVAN, FARK ve TOPLAM satırları, onay işlemleri ve Rapor Al menüsü. İşlemler wwwroot/js/tavantertip.js ile /api/tavantertip
    /// uçlarından yapılır.
    /// </summary>
    public class TavanTertipModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanTertipModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TavanTertipUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
