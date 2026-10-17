using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>KBS Aktarım Tanımlama (eski ButceMuhasebe/TanimKBSAktarim.aspx); işlemler js/muhtanim-kbs.js ile /api/muhtanim/kbs uçlarından.</summary>
    public class TanimKBSAktarimModel : MuhTanimSayfa
    {
        public TanimKBSAktarimModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimDigerUclari.KBSAdresi);
        }
    }
}
