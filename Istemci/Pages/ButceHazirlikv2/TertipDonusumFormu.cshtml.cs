using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Tertip dönüşümü (eski ButceHazirlikv2/TertipDonusumFormu.aspx; bütçe teklifi ekranındaki "Tertip Dönüşüm" tuşu, ?yil=..):
    /// analitik tertip ile program tertibi arasında çeviri. Satırlar elle, Excel'den ya da panodan girilir; Dönüştür boş tarafı doldurur.
    /// İşlemler wwwroot/js/tertipdonusum.js ile /api/tertipdonusum uçlarından yapılır.
    /// </summary>
    public class TertipDonusumFormuModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TertipDonusumFormuModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int Yil { get; private set; }

        public void OnGet(int yil = 0)
        {
            Yil = yil >= 2000 ? yil : System.DateTime.Now.Year;
            Yetkili = TertipDonusumUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
