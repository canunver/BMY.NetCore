using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Yerleşke Oda Tanımı (eski KYM/TanimOda.aspx): solda yerleşke / bina / oda ağacı, sağda seçili yerin bilgileri.
    /// Tanımlanan yerler Personel Bilgi &gt; Giderler'deki Harcama Yeri'nde (ve Merkez Bankası kurulumunda Taşınır'da) kullanılır.
    /// İşlemler wwwroot/js/yerleskeoda.js ile /api/yerleskeoda uçlarından yapılır.
    /// </summary>
    public class TanimOdaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimOdaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = YerleskeOdaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
