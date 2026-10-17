using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>İstatistik Kod Girişi (eski ButceMuhasebe/IstatistikKodGirisForm.aspx); işlemler js/muhtanim-istatistik.js ile /api/muhtanim/istatistik uçlarından.</summary>
    public class IstatistikKodGirisFormModel : MuhTanimSayfa
    {
        public IstatistikKodGirisFormModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimDigerUclari.IstatistikAdresi, false);
        }
    }
}
