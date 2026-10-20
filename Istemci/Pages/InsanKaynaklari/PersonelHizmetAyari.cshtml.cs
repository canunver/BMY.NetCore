using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Hizmet Ayarları penceresi (eski InsanKaynaklari/PersonelHizmetAyari.aspx); wwwroot/js/personel-hizmetayari.js, /api/personelbilgi/kadro/hizmetayari uçları.
    /// </summary>
    public class PersonelHizmetAyariModel : PersonelAltSayfaModel
    {
        public PersonelHizmetAyariModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelHizmetAyari";
    }
}
