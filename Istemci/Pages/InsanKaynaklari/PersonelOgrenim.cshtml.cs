using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri > Öğrenim Durumu (eski InsanKaynaklari/PersonelOgrenim.aspx) sekmesi; PersonelBilgi sayfasında iframe içinde
    /// ?menuYok=1&amp;personelKod=&amp;personelTur=&amp;tcKimlikNo= ile açılır. Solda form, sağda kayıt listesi;
    /// içerik wwwroot/js/personel-ogrenim.js ile /api/personelbilgi/ogrenim uçlarından gelir.
    /// </summary>
    public class PersonelOgrenimModel : PersonelAltSayfaModel
    {
        public PersonelOgrenimModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelOgrenim";
    }
}
