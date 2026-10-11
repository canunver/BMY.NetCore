using Istemci.Organizasyon;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Organizasyon
{
    /// <summary>
    /// Organizasyon Şeması (eski Organizasyon/ORGAna.aspx ve çerçevelerdeki ORGAgac.aspx, ORGBirim.aspx, OrgGorev.aspx):
    /// solda birim ağacı, sağda seçili birimin iki sekmesi: Birim Bilgileri ve Görevler (göreve kişi atama).
    /// Eski sayfadaki diğer sekmeler (Stratejik Plan, Ödenek Durumu, Taşınır, Personel, Evrak, Mevzuat, E-Fatura) taşınmadı.
    /// ?sablon=1 ile şablon şeması düzenlenir (menüde Şablon Tanımlama).
    /// İşlemler wwwroot/js/organa.js ile /api/orgbirim uçlarından yapılır; Görevler sekmesi Organizasyon/GorevAta sayfasını
    /// seçili birim için çerçevede açar.
    /// </summary>
    public class ORGAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ORGAnaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = OrgBirimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
