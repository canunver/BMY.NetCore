using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// HİTAP'tan Al penceresi (eski InsanKaynaklari/PersonelHITAP.aspx); wwwroot/js/personel-hitap.js, /api/personelbilgi/kadro/hitap uçları.
    /// Adres: PersonelHITAP?menuYok=1&amp;personelKod=&lt;kod&gt;&amp;tur=1 (tur: aktarım türü, eski RequestBul(this, "tur", 1)).
    /// </summary>
    public class PersonelHITAPModel : PersonelAltSayfaModel
    {
        public PersonelHITAPModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelHITAP";

        public int Tur { get; private set; }

        public override void OnGet()
        {
            base.OnGet();
            Tur = int.TryParse(Request.Query["tur"], out int t) ? t : 1;
        }
    }
}
