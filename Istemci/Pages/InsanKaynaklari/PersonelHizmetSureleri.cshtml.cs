using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Hizmet Süreleri penceresi (eski InsanKaynaklari/PersonelHizmetSureleri.aspx); wwwroot/js/personel-hizmetsureleri.js, /api/personelbilgi/kadro/hizmetsureleri ucu.
    /// </summary>
    public class PersonelHizmetSureleriModel : PersonelAltSayfaModel
    {
        public PersonelHizmetSureleriModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelHizmetSureleri";
    }
}
