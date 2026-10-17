using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>İmza Bilgileri Tanımlama (eski ButceMuhasebe/Imza.aspx); işlemler js/muhtanim-imza.js ile /api/muhtanim/imza uçlarından.</summary>
    public class ImzaModel : MuhTanimSayfa
    {
        public ImzaModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimDigerUclari.ImzaAdresi, false);
        }
    }
}
