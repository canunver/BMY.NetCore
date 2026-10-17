using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Mutemet Tanımlama (eski ButceMuhasebe/TanimMutemetLimit.aspx); işlemler js/muhtanim-mutemet.js ile /api/muhtanim/mutemet uçlarından.</summary>
    public class TanimMutemetLimitModel : MuhTanimSayfa
    {
        public TanimMutemetLimitModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimOrtak.MutemetAdresi);
        }
    }
}
