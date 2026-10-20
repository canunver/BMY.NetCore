using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Taşınır Ekonomik Kod (eski ButceMuhasebe/TasinirEkonomikKod.aspx); işlemler js/muhtanim-tasinir.js ile /api/muhtanim/tasinir uçlarından.</summary>
    public class TasinirEkonomikKodModel : MuhTanimSayfa
    {
        public TasinirEkonomikKodModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimTasinirUclari.TasinirYetkili(kullanan, Klasor);
        }
    }
}
