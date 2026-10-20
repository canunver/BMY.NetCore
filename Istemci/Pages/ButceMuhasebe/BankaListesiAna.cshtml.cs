using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Banka Listesi İşlemleri, eski ButceMuhasebe/BankaListesiAna.aspx. Solda muhasebe birimi ve tarihe göre banka listeleri, sağda seçilen
    /// listenin başlık bilgileri ve listeye eklenmiş ödeme emri satırları. Liste eklenir, onaylanır, ödeme emirleri eklenir / çıkartılır,
    /// ödendi bilgisi ve çek numarası verilir, Word / Excel / pdf yazdırılır, banka dosyası gönderilir (js/bankalistesi.js, /api/bankalistesi/*).
    /// </summary>
    public class BankaListesiAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BankaListesiAnaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = BankaListesiUclari.SayfaYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
