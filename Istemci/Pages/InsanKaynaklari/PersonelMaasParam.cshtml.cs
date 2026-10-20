using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri sekmesi: Maaş parametreleri (eski InsanKaynaklari/PersonelMaasParam.aspx): sendika, değişken değerleri, matrahlar, bordro yazdırma.
    /// İçerik wwwroot/js ile /api/personelbilgi uçlarından gelir (PersonelMaasParamUclari).
    /// </summary>
    public class PersonelMaasParamModel : PersonelAltSayfaModel
    {
        public PersonelMaasParamModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelMaasParam";
    }
}
