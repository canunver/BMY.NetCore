using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Açıklama Tanımları (eski ButceMuhasebe/TanimAciklama.aspx). Ödeme Emri / MİF formundan
    /// Sgb.pencere('TanimAciklama?muhasebe=..&amp;birim=..&amp;tip=1', 'Açıklama Listesi') ile açılır; listeden aktarılan açıklama
    /// üst sayfanın window.AciklamaBilgiYaz(aciklama) işlevine verilir ve pencere kapanır. İşlemler js/tanimaciklama.js ile
    /// /api/tanimaciklama uçlarından. Yetki Ödeme Emri / MİF menü seçeneklerinden gelir.
    /// </summary>
    public class TanimAciklamaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimAciklamaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int Muhasebe { get; private set; }
        public string Birim { get; private set; } = "";
        public int Tip { get; private set; }

        public void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = kullanan != null && TanimAciklamaUclari.YetkiVar(kullanan, ortam.ContentRootPath);
            Muhasebe = OrtakFonksiyonlar.ConvertToInt((string)Request.Query["muhasebe"], 0);
            Birim = ((string)Request.Query["birim"] ?? "").Trim();
            Tip = OrtakFonksiyonlar.ConvertToInt((string)Request.Query["tip"], 0);
        }
    }
}
