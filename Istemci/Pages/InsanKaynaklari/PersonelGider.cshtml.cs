using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri sekmesi: Personel giderleri (eski InsanKaynaklari/PersonelGider.aspx): gider ekleme / değiştirme, kümülatif yükleme, aktarım.
    /// İçerik wwwroot/js ile /api/personelbilgi uçlarından gelir (PersonelGiderUclari).
    /// </summary>
    public class PersonelGiderModel : PersonelAltSayfaModel
    {
        public PersonelGiderModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelGider";
    }
}
