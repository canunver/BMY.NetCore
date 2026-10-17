using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Birim Bütçe Teklifi (eski ButceHazirlikv2/BirimButceHazirlikForm.aspx): seçilen saymanlık / birimin taşra tertiplerine yılın
    /// bütçe teklifi ve gerekçesi girilir; il / merkeze gönderme ve iade, Excel raporları. yeni=0 "Mevcut Proje/Faaliyetler"
    /// (önceki yıllarda yürütülenler), yeni=1 (varsayılan) "Diğer Proje/Faaliyetler". Birim Bütçe Hazırlığı çerçevesinin
    /// (BirimButceHazirlikFormAna) iki sekmesinde açılır; menüde "Ödenek Planlaması" olarak tek başına da açılır.
    /// İşlemler wwwroot/js/birimbutcehazirlik.js ile /api/birimbutcehazirlik uçlarından yapılır.
    /// </summary>
    public class BirimButceHazirlikFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BirimButceHazirlikFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public bool Yeni { get; private set; }

        public void OnGet()
        {
            Yeni = OrtakFonksiyonlar.ConvertToInt((string)Request.Query["yeni"], 1) == 1;
            Yetkili = BirimButceHazirlikUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
