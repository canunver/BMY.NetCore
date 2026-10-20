using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri sekmesi (eski InsanKaynaklari/PersonelIzin.aspx). Yetki ve adres satırındaki personel bilgileri
    /// PersonelAltSayfaModel'den; içerik JavaScript ile /api/personelbilgi uçlarından gelir.
    /// </summary>
    public class PersonelIzinModel : PersonelAltSayfaModel
    {
        public PersonelIzinModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelIzin";
    }
}
