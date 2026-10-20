using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri "Kişisel Bilgiler" sekmesi (eski InsanKaynaklari/PersonelKisisel.aspx): özlük / nüfus bilgileri,
    /// resim, belgeler, T.C. kimlik no değiştirme ve yazdırma düğmeleri. personelKod boş ya da 0 ise yeni personel girişi.
    /// Yetki ve adres satırındaki personel bilgileri PersonelAltSayfaModel'den; içerik wwwroot/js/personel-kisisel.js ile
    /// /api/personelbilgi/kisisel/... ve /api/personelbilgi/rapor/... uçlarından gelir.
    /// </summary>
    public class PersonelKisiselModel : PersonelAltSayfaModel
    {
        public PersonelKisiselModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelKisisel";
    }
}
