using Istemci.Organizasyon;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Organizasyon
{
    /// <summary>
    /// Göreve Kişi Atama (eski "Gruba Kullanıcı Ata": Organizasyon/ORGAna.aspx?gorevata=1 içindeki birim ağacı, OrgGorev.aspx
    /// ve OrgGorevKisit.aspx): solda birim ağacı, ortada seçili birimin görevleri, sağda seçili göreve atanmış kişiler.
    /// Kişi ile görev T.C. kimlik numarasıyla ilişkilendirilir; kullanıcının yetkileri bu numara üzerinden oluşur.
    /// ?kullanici=kod ile açılırsa (Kullanıcı Giriş'teki Görevler tuşu) o kullanıcının şimdiki görevleri gösterilir ve
    /// kullanıcı seçilen göreve tek tuşla eklenir. Çoğunlukla pencere içinde (?menuYok=1) açılır.
    /// İşlemler wwwroot/js/gorevata.js ile /api/gorevata uçlarından yapılır. Yalnızca sistem yöneticisi kullanabilir.
    /// </summary>
    public class GorevAtaModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GenelGorevUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
