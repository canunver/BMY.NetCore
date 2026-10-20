using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Fatura Hazırlama, eski ButceMuhasebe/FaturaListesi.aspx: kriterlere göre faturalar listelenir; Yeni Fatura ve satırdaki Aç
    /// fatura formunu (FaturaGiris) pencerede açar; Listeyi Yazdır (Excel), Toplu Tahakkuk Oluştur (FaturaTahakkukIslemleri penceresi).
    /// İşlemler wwwroot/js/fatura.js ile /api/fatura uçlarından (Muhasebe/FaturaUclari.cs) yapılır.
    /// </summary>
    public class FaturaListesiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public FaturaListesiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = FaturaUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
