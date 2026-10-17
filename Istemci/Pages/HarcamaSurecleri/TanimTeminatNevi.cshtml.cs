using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>Teminat Mektubu Nevi Tanımlama (eski HarcamaSurecleri/TanimTeminatNevi.aspx); işlemler js/muhtanim-teminat.js ile /api/muhtanim/teminatnevi uçlarından.</summary>
    public class TanimTeminatNeviModel : MuhTanimSayfa
    {
        public TanimTeminatNeviModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimBasitUclari.TeminatNeviAdresi, false);
        }
    }
}
