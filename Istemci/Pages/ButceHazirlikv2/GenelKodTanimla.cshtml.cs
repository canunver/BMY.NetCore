using Istemci.ButceKod;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Genel Kod Tanımlama (eski ButceHazirlikv2/GenelKodTanimla.aspx ve çerçevedeki GenelKodTanimlaDetay.aspx):
    /// solda kod ağacı, sağda seçili kodun adı ve açıklaması (her dil için) ile ek alanları.
    /// Aynı sayfa adresteki Tur parametresine göre farklı kodları düzenler; menüde örneğin
    /// "Birim Tür Tanımlama" = ?Tur=21, "İl-İlçe Bilgileri" = ?Tur=20, bütçe sınıflandırmaları = ?Tur=1..7.
    /// İşlemler wwwroot/js/genelkodtanimla.js ile /api/kod uçlarından yapılır. Yetki menüden gelir (KodTanimUclari.YetkiVar).
    /// </summary>
    public class GenelKodTanimlaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public GenelKodTanimlaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet(int tur = 1)
        {
            Yetkili = KodTanimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), tur, ortam.ContentRootPath);
        }
    }
}
