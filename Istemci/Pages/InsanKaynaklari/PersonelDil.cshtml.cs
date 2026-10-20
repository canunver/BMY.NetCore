using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri > Yabancı Dil (eski InsanKaynaklari/PersonelDil.aspx) sekmesi; PersonelBilgi sayfasında iframe içinde
    /// ?menuYok=1&amp;personelKod=&amp;personelTur=&amp;tcKimlikNo= ile açılır. Solda form, sağda kayıt listesi;
    /// içerik wwwroot/js/personel-dil.js ile /api/personelbilgi/dil uçlarından gelir.
    /// </summary>
    public class PersonelDilModel : PersonelAltSayfaModel
    {
        public PersonelDilModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelDil";
    }
}
