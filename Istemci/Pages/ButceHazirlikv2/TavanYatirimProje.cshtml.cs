using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Birim Görüşme Ekranı (Yatırım / Cari) (eski ButceHazirlikv2/TavanYatirimProje.aspx?tur=2 / ?tur=1): birimin yatırım (ya da cari)
    /// ekonomik kod + sektör tavanının faaliyet/projelere görüşme tutarları; proje grubu ara toplamları, en üstte salt okunur TAVAN, FARK
    /// ve TOPLAM satırları, onay işlemleri ve Rapor Al menüsü. İşlemler wwwroot/js/tavanyatirim.js ile /api/tavanyatirim uçlarından yapılır.
    /// </summary>
    public class TavanYatirimProjeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanYatirimProjeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        /// <summary>2: yatırım (menüdeki), 1: cari (eski sayfada tur verilmezse).</summary>
        public int Tur { get; private set; }

        public void OnGet(int? tur)
        {
            Tur = TavanYatirimProjeUclari.TurDuzelt(tur ?? 1);
            Yetkili = TavanYatirimProjeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, Tur);
        }
    }
}
