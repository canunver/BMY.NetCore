using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Çek Listesi, eski ButceMuhasebe/CekKocaniForm.aspx. Hesap kodu, çek no, durum ve tarih aralığına göre çekler listelenir, Excel'e
    /// yazdırılır; iptal çek eklenir ve iptal bilgisi silinir (js/ceklistesi.js, /api/ceklistesi/*).
    /// </summary>
    public class CekKocaniFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public CekKocaniFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = CekListesiUclari.SayfaYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
