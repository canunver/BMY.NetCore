using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri > Hizmet İçi Eğitim (eski InsanKaynaklari/PersonelKurs.aspx) sekmesi; PersonelBilgi sayfasında iframe içinde
    /// ?menuYok=1&amp;personelKod=&amp;personelTur=&amp;tcKimlikNo= ile açılır. Solda form, sağda kayıt listesi;
    /// içerik wwwroot/js/personel-kurs.js ile /api/personelbilgi/kurs uçlarından gelir.
    /// </summary>
    public class PersonelKursModel : PersonelAltSayfaModel
    {
        public PersonelKursModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelKurs";
    }
}
