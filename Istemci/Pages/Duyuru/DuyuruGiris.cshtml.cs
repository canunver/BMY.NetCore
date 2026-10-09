using System.Collections.Generic;
using System.Text.Json;
using Istemci.Duyuru;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Duyuru
{
    /// <summary>
    /// Duyuru Girişi (eski Duyuru/DuyuruGiris.aspx): duyuru arama, ekleme, düzenleme ve silme.
    /// Sayfa yalnızca iskeleti ve metinleri basar; işlemler wwwroot/js/duyurugiris.js ile /api/duyuru uçlarından yapılır.
    /// Yalnızca sistem yöneticisi ve destek başvuru yöneticisi kullanabilir.
    /// </summary>
    public class DuyuruGirisModel : PageModel
    {
        public bool Yetkili { get; private set; }

        /// <summary>JavaScript'in kullandığı metinler (JSON).</summary>
        public string MetinlerJson { get; private set; }

        public void OnGet()
        {
            Yetkili = DuyuruUclari.GirisYetkisiVar(OturumBilgisi.KullaniciOku(HttpContext));

            Dictionary<string, string> metinler = new Dictionary<string, string>();
            foreach (string anahtar in new[] { "SilOnay", "FRMDYG021", "FRMDYG015", "IslemHata", "Bekleyin", "Gizli", "Acik",
                                               "Onem", "FRMDYG006", "Konu", "FRMDYL002", "Gizlilik", "Pencerede" })
                metinler[anahtar] = Metin.Duyuru(anahtar);
            MetinlerJson = JsonSerializer.Serialize(metinler);
        }
    }
}
