using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri "Askerlik" sekmesi (eski InsanKaynaklari/PersonelAskerlik.aspx): personelin askerlik bilgileri ve
    /// askerlik belgeleri. İşlemler wwwroot/js/personel-askerlik.js ile /api/personelbilgi/askerlik uçlarından yapılır.
    /// </summary>
    public class PersonelAskerlikModel : PersonelAltSayfaModel
    {
        public PersonelAskerlikModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "PersonelAskerlik";
    }
}
