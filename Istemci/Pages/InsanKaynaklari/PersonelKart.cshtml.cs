using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri "Kart" sekmesi (eski InsanKaynaklari/PersonelKart.aspx): solda seçilen kartın bilgileri, sağda personelin
    /// kartları. Zimmet eden / iade alan personel ortak personel seçimiyle (Sgb.personelSec) seçilir. İşlemler wwwroot/js/personel-kart.js ile
    /// /api/personelbilgi/kart uçlarından yapılır.
    /// </summary>
    public class PersonelKartModel : PersonelAltSayfaModel
    {
        public PersonelKartModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "PersonelKart";
    }
}
