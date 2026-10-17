using Istemci.KesinHesap;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.SayistayBulgu
{
    /// <summary>
    /// Sayıştay Bulguları (eski SayistayBulgu/SayBulgu.aspx): yılın bulguları listelenir, yeni bulgu girilir / düzenlenir, bulguyla
    /// ilgili birimler bağlanır, birimlerin cevap girişi izni açılıp kapatılır, ekli dosyalar ve Excel listeleri alınır. İşlemler
    /// wwwroot/js/saybulgu.js ile /api/saybulgu uçlarından yapılır.
    /// </summary>
    public class SayBulguModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public SayBulguModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = SayBulguUclari.BulguYetkisi(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
