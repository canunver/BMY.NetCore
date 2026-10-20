using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri / Hizmet Belgesi sekmesi (eski InsanKaynaklari/PersonelKadro.aspx). İşlemler wwwroot/js/personel-kadro.js ile /api/personelbilgi/kadro uçlarından yapılır.
    /// </summary>
    public class PersonelKadroModel : PersonelAltSayfaModel
    {
        public PersonelKadroModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelKadro";
    }
}
