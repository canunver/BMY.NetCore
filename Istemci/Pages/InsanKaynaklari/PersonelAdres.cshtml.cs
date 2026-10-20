using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri sekmesi: İletişim / adres bilgileri (eski InsanKaynaklari/PersonelAdres.aspx).
    /// İçerik wwwroot/js ile /api/personelbilgi uçlarından gelir (PersonelAdresUclari).
    /// </summary>
    public class PersonelAdresModel : PersonelAltSayfaModel
    {
        public PersonelAdresModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelAdres";
    }
}
