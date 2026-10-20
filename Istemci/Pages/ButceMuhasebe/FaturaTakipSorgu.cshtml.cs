using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Fatura Takip, eski ButceMuhasebe/FaturaTakipSorgu.aspx: ödeme emri / MİF belgeleri hesap ve alt hesaba göre listelenir, takip
    /// durumu değiştirilir, liste Excel'e yazdırılır (js/faturatakip.js, /api/faturatakip/*, Muhasebe/FaturaTakipUclari.cs).
    /// </summary>
    public class FaturaTakipSorguModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FaturaTakipSorguModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FaturaTakipUclari.SayfaYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
