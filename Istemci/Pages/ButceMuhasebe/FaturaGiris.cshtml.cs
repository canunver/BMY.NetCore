using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Fatura formu, eski ButceMuhasebe/FaturaGiris.aspx (Fatura Hazırlama listesinden pencere olarak açılır, menüde yoktur):
    /// fatura bilgileri ve satırları, Kaydet / İptal / Yazdır (Word), Tahakkuk / Ödeme MİF oluşturma, açıklama tanımları.
    /// Adres: FaturaGiris?kod= (0 ya da boş: yeni fatura). Yetki üst sayfa Fatura Hazırlama'dan gelir (js/fatura.js, /api/fatura/form/*).
    /// </summary>
    public class FaturaGirisModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FaturaGirisModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FaturaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, FaturaUclari.FormAdresi);
        }
    }
}
