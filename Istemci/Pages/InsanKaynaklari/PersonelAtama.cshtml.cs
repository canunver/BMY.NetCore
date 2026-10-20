using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel Atama İşlemleri penceresi (eski InsanKaynaklari/PersonelAtama.aspx). Hizmet Belgesi (Kadro) sekmesinden
    /// Sgb.pencere ile açılır: ?menuYok=1&amp;personelKod=&lt;p&gt;&amp;kod=&lt;hizmet kaydı kodu; yeni kayıt için 0&gt;.
    /// İçerik wwwroot/js/personel-atama.js ile /api/personelbilgi/atama uçlarından gelir.
    /// </summary>
    public class PersonelAtamaModel : PersonelAltSayfaModel
    {
        public PersonelAtamaModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelAtama";

        /// <summary>Açılacak hizmet (kadro) kaydının kodu; 0: yeni kayıt.</summary>
        public int Kod { get; private set; }

        public override void OnGet()
        {
            base.OnGet();
            int.TryParse(Request.Query["kod"], out int kod);
            Kod = kod;
        }
    }
}
