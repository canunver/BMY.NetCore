using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Faaliyet Raporu (eski ButceHazirlikv2/FaaliyetRaporuForm.aspx): yılın faaliyet raporu bölüm metinleri (4, 4.1, 4.2) ve
    /// tablo notları editörlerde okunur / kaydedilir (Kaydet yalnızca sistem yöneticisine); Raporlar menüsünden faaliyet raporu
    /// ve tabloları (eski rapor kodu, Word / PDF) alınır. İşlemler wwwroot/js/faaliyetraporu.js ile /api/faaliyetraporu uçlarından.
    /// </summary>
    public class FaaliyetRaporuFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FaaliyetRaporuFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FaaliyetRaporuUclari.FormYetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
