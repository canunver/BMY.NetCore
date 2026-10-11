using System.Collections.Generic;
using System.Text.Json;
using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muhasebe Bilgileri (eski ButceMuhasebe/TanimMuhasebe.aspx): muhasebe (saymanlık) arama, ekleme, düzenleme, silme
    /// ve saymanlık taşıma. Sayfa yalnızca iskeleti ve metinleri basar; işlemler wwwroot/js/tanimmuhasebe.js ile
    /// /api/muhasebe/saymanlik uçlarından yapılır. Yalnızca sistem yöneticisi ve muhasebe yöneticisi kullanabilir.
    /// </summary>
    public class TanimMuhasebeModel : PageModel
    {
        public bool Yetkili { get; private set; }

        /// <summary>JavaScript'in kullandığı metinler (JSON).</summary>
        public string MetinlerJson { get; private set; }

        /// <summary>Saymanlık taşıma penceresinin seçenekleri (JSON): islemler, tasimaHazir.</summary>
        public string TanimlarJson { get; private set; }

        public void OnGet()
        {
            Yetkili = MuhasebeUclari.TanimYetkisiVar(OturumBilgisi.KullaniciOku(HttpContext));
            if (!Yetkili) return;

            Dictionary<string, string> metinler = new Dictionary<string, string>();
            foreach (string anahtar in new[] { "MUHGNL075", "MUHGNL076", "MUHGNL162", "MUHGNL183", "IslemHata", "TasiEksik", "TasiIslemSec", "TasiOnay" })
                metinler[anahtar] = Metin.Muhasebe(anahtar);
            MetinlerJson = JsonSerializer.Serialize(metinler);

            TanimlarJson = JsonSerializer.Serialize(new
            {
                islemler = MuhasebeUclari.TasimaIslemleri(),
                tasimaHazir = MuhasebeUclari.TasimaKullanilabilir()
            });
        }
    }
}
