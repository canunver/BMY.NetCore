using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Hesap Planı Kısıtlama Tanımlama (eski ButceMuhasebe/TanimHesapPlaniKisit.aspx); işlemler js/muhtanim-mutemet.js ile /api/muhtanim/hesapkisit uçlarından.</summary>
    public class TanimHesapPlaniKisitModel : MuhTanimSayfa
    {
        public TanimHesapPlaniKisitModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimOrtak.HesapKisitAdresi);
        }
    }
}
