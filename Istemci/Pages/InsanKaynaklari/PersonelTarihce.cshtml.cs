using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İşlemleri "Tarihçe" sekmesi (eski InsanKaynaklari/PersonelTarihce.aspx): personel bilgilerinde yapılan değişikliklerin
    /// salt okunur listesi. Liste wwwroot/js/personel-tarihce.js ile /api/personelbilgi/tarihce ucundan okunur.
    /// </summary>
    public class PersonelTarihceModel : PersonelAltSayfaModel
    {
        public PersonelTarihceModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "PersonelTarihce";
    }
}
