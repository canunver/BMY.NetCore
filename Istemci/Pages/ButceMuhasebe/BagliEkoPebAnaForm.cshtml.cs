using System.Collections.Generic;
using System.Text.Json;
using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Faaliyet Tertip Kısıtlama / Tertip Faaliyet İlişkisi (eski ButceMuhasebe/BagliEkoPebAnaForm.aspx): tertip - faaliyet
    /// ilişkilerinin listesi, ekleme ve silme. Sayfa yalnızca iskeleti ve metinleri basar; işlemler wwwroot/js/bagliekopeb.js ile
    /// /api/bagliekopeb uçlarından (Muhasebe/BagliEkoPebUclari.cs) yapılır.
    /// </summary>
    public class BagliEkoPebAnaFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public BagliEkoPebAnaFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        /// <summary>JavaScript'in kullandığı metinler (JSON).</summary>
        public string MetinlerJson { get; private set; }

        public void OnGet()
        {
            Yetkili = BagliEkoPebUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
            if (!Yetkili) return;

            Dictionary<string, string> metinler = new Dictionary<string, string>();
            foreach (string anahtar in new[] { "BagliEkoPebAna003", "BagliEkoPebAna013", "BagliEkoPebAna014", "BagliEkoPebKaydetOnay",
                                               "BagliEkoPebSilOnay", "BagliEkoPebBos", "Kopyala", "Kopyalandi", "MUHGNL002", "IslemHata" })
                metinler[anahtar] = Metin.Muhasebe(anahtar);
            MetinlerJson = JsonSerializer.Serialize(metinler);
        }
    }
}
