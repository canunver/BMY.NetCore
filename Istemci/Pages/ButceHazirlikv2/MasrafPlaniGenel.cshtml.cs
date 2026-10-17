using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Masraf Planı (eski ButceHazirlikv2/MasrafPlaniGenel.aspx, "Masraf Planı Veri Girişi"): seçilen aşamada kodlara ve harcama
    /// birimine uyan tertiplerin tavanı, toplamı ve aylık dağılımı listelenir; tavanlar masraf planına dağıtılır, AHP'den masraf
    /// planına aktarılır, masraf planı onaylanır / onayı kaldırılır, önceki aşamadan alınır; raporlar, hızlı ödenek dosyaları, rapor
    /// imzaları ve (sistem yöneticisine) ödenek kontrol dönemi. İşlemler wwwroot/js/masrafplani.js ile /api/masrafplani uçlarından yapılır.
    /// </summary>
    public class MasrafPlaniGenelModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MasrafPlaniGenelModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MasrafPlaniUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
