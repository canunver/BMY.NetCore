using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Mahsup İşlemleri, eski ButceMuhasebe/Kapatma.aspx. Menüde üç seçenek: hesap kodsuz, ?hesap=323 ve ?hesap=103 (hesap verilince
    /// hesap kodu alanı gizlenir ve o hesapla listelenir). Seçili belgelerden kapatma (mahsup) belgesi oluşturulur, kapatma belgesi
    /// iptal edilir, belgeler listeden çıkartılır, liste Excel'e ve banka listesi Word'e yazdırılır (js/kapatma.js, /api/kapatma/*).
    /// </summary>
    public class KapatmaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public KapatmaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        /// <summary>Menüden gelen hesap kodu (323 / 103) ya da boş.</summary>
        public string Hesap { get; private set; }

        public void OnGet(string hesap)
        {
            Hesap = (hesap ?? "").Trim();
            Yetkili = KapatmaUclari.SayfaYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, Hesap);
        }
    }
}
