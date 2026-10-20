using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri sekmesi: Aile bilgileri (eski InsanKaynaklari/PersonelAile.aspx): aile bireyleri, bireyin belgeleri, nüfus cüzdanı örneği ve ikametgah ilmühaberi.
    /// İçerik wwwroot/js ile /api/personelbilgi uçlarından gelir (PersonelAileUclari).
    /// </summary>
    public class PersonelAileModel : PersonelAltSayfaModel
    {
        public PersonelAileModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelAile";
    }
}
