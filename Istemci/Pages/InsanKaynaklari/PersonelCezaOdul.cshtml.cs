using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri > Ceza/Ödül (eski InsanKaynaklari/PersonelCezaOdul.aspx) sekmesi; PersonelBilgi sayfasında iframe içinde
    /// ?menuYok=1&amp;personelKod=&amp;personelTur=&amp;tcKimlikNo= ile açılır. Solda form, sağda kayıt listesi;
    /// içerik wwwroot/js/personel-cezaodul.js ile /api/personelbilgi/cezaodul uçlarından gelir.
    /// </summary>
    public class PersonelCezaOdulModel : PersonelAltSayfaModel
    {
        public PersonelCezaOdulModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelCezaOdul";
    }
}
