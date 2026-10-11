using System.Collections.Generic;
using System.Text.Json;
using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muhasebe Birim Bilgileri (eski ButceMuhasebe/TanimMuhasebeBirim.aspx): muhasebe birimi arama, ekleme, düzenleme,
    /// silme ve listenin dışa aktarımı. Sayfa yalnızca iskeleti ve metinleri basar; işlemler
    /// wwwroot/js/tanimmuhasebebirim.js ile /api/muhasebe/birim uçlarından yapılır.
    /// Yalnızca sistem yöneticisi ve muhasebe yöneticisi kullanabilir.
    /// </summary>
    public class TanimMuhasebeBirimModel : PageModel
    {
        public bool Yetkili { get; private set; }

        /// <summary>JavaScript'in kullandığı metinler (JSON).</summary>
        public string MetinlerJson { get; private set; }

        public void OnGet()
        {
            Yetkili = MuhasebeUclari.TanimYetkisiVar(OturumBilgisi.KullaniciOku(HttpContext));
            if (!Yetkili) return;

            Dictionary<string, string> metinler = new Dictionary<string, string>();
            foreach (string anahtar in new[] { "FRMMUB003", "MUHGNL191", "MUHGNL077", "MUHGNL078", "MUHGNL079", "VergiNo", "FRMOES010", "Aktif", "Kapali",
                                               "MUHGNL162", "MUHGNL183", "IslemHata", "DisaAktarBos", "DosyaAd" })
                metinler[anahtar] = Metin.Muhasebe(anahtar);
            MetinlerJson = JsonSerializer.Serialize(metinler);
        }
    }
}
