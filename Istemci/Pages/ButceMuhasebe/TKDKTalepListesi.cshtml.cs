using System;
using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// İl Koordinatörlüğü Ödenek Talep İşlemleri, eski ButceMuhasebe/TKDKTalepListesi.aspx. Talepler yıl / ay / muhasebe / birim / tür / duruma
    /// göre listelenir; talep formu (TKDKTalepGiris) pencerede açılır; liste, rapor ve olur eki, banka listesi yazdırılır; ödeme emri
    /// (nakit, aktarma) oluşturulur; talepler onaylanır / reddedilir (js/tkdktalep.js, /api/tkdktalep/*).
    /// </summary>
    public class TKDKTalepListesiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TKDKTalepListesiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        /// <summary>Eski sayfada Onayla, Reddet, Rapor ve Olur Eki yalnız sistem yöneticisine görünür.</summary>
        public bool Yonetici { get; private set; }

        public int BuYil { get; } = DateTime.Now.Year;

        public void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = TKDKTalepUclari.YetkiVar(kullanan, ortam.ContentRootPath);
            Yonetici = Yetkili && TKDKTalepUclari.Yonetici(kullanan);
        }
    }
}
