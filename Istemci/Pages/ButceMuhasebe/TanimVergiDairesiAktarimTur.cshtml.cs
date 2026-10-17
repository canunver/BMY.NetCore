using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Damga Vergisi / KDV Tevkifatı / KDV / Muhtasar Aktarım Tür Tanımlama (eski ButceMuhasebe/TanimVergiDairesiAktarimTur.aspx?tur=1..4).
    /// Menüde her tür ayrı adrestir; yetki o adresle denetlenir. İşlemler js/muhtanim-vdaktarim.js ile /api/muhtanim/vdaktarim uçlarından.
    /// </summary>
    public class TanimVergiDairesiAktarimTurModel : MuhTanimSayfa
    {
        public TanimVergiDairesiAktarimTurModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        public int Tur { get; private set; }

        public string Baslik { get { return MuhTanimBasitUclari.VDAktarimBaslik(Tur); } }

        public override void OnGet()
        {
            int.TryParse(Request.Query["tur"], out int tur);
            Tur = tur;
            base.OnGet();
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimBasitUclari.VDAktarimYetkili(kullanan, Klasor, Tur);
        }
    }
}
