using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Toplu Tahakkuk, eski ButceMuhasebe/FaturaTahakkukIslemleri.aspx (Fatura Hazırlama'daki Toplu Tahakkuk Oluştur ile pencere olarak
    /// açılır, menüde yoktur): listedeki kriterlerle faturalar gelir, seçilenlerden tek Tahakkuk MİF'i ya da Ödeme Emri oluşturulur.
    /// Yetki üst sayfa Fatura Hazırlama'dan gelir (js/fatura.js, /api/fatura/toplu/*).
    /// </summary>
    public class FaturaTahakkukIslemleriModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FaturaTahakkukIslemleriModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FaturaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, FaturaUclari.TopluAdresi);
        }
    }
}
