using Istemci.InsanKaynaklari;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Bekleyen İşlemler penceresi (eski InsanKaynaklari/PersonelKadroOnaysiz.aspx): Onaysızlar ve Tayin Bekleyenler listeleri,
    /// Birime Al. Adres: PersonelKadroOnaysiz?menuYok=1&amp;orgKod=&lt;birim&gt;. wwwroot/js/personel-kadroonaysiz.js,
    /// /api/personelbilgi/kadroonaysiz uçları.
    /// </summary>
    public class PersonelKadroOnaysizModel : PersonelAltSayfaModel
    {
        public PersonelKadroOnaysizModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override string SayfaAd => "PersonelKadroOnaysiz";

        public int OrgKod { get; private set; }

        public override void OnGet()
        {
            base.OnGet();
            OrgKod = int.TryParse(Request.Query["orgKod"], out int o) ? o : 0;
        }
    }
}
