using System.Collections.Generic;
using System.Text.Json;
using Istemci.Organizasyon;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Kullanıcı Arama (eski KYM/KullaniciArama.aspx): solda sorgu alanları, sağda kullanıcı listesi.
    /// Sayfa yalnızca iskeleti ve metinleri basar; işlemler wwwroot/js/kullaniciarama.js ile /api/kullanici uçlarından yapılır.
    /// Giriş yapmış herkes arayabilir; kimin neyi görüp açabileceğini uçlar belirler.
    /// </summary>
    public class KullaniciAramaModel : PageModel
    {
        /// <summary>Görev tanımlama ve göreve kişi atama pencereleri açılabilir mi? Yalnızca sistem yöneticisi (eski Grup Oluştur / Gruba Kullanıcı Ata).</summary>
        public bool GorevAtayabilir { get; private set; }

        /// <summary>JavaScript'in kullandığı metinler (JSON).</summary>
        public string MetinlerJson { get; private set; }

        public void OnGet()
        {
            GorevAtayabilir = GenelGorevUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext));
            Dictionary<string, string> metinler = new Dictionary<string, string>();
            foreach (string anahtar in new[] { "FRMKUA015", "FRMKUG005", "FRMKUA016", "FRMKUA017", "FRMKUA018", "FRMKUA019", "FRMKUA020", "FRMKUA021",
                                               "FRMKUA026", "Aktif", "Pasif", "IslemHata", "DisaAktarBos", "DosyaAd" })
                metinler[anahtar] = Metin.Kullanici(anahtar);
            MetinlerJson = JsonSerializer.Serialize(metinler);
        }
    }
}
