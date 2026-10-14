using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Koordinasyon Tavan Girişi ve Kontrolü (eski ButceHazirlikv2/TavanEkoFaaliyet.aspx): birimin ekonomik kod + sektör tavanının
    /// faaliyet/projelere üç yıllık dağıtımı; en üstte salt okunur TAVAN, FARK ve TOPLAM satırları, onay işlemleri ve toplu onay.
    /// İşlemler wwwroot/js/tavanfaal.js ile /api/tavanfaal uçlarından yapılır.
    /// </summary>
    public class TavanEkoFaaliyetModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanEkoFaaliyetModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TavanEkoFaaliyetUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
