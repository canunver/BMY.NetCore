using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Hesap Planı Bilgileri (eski ButceMuhasebe/TanimHesapPlani.aspx ve HesapPlaniNitelik.aspx); işlemler js/muhtanim-hesapplani.js ile /api/muhtanim/hesapplani uçlarından.</summary>
    public class TanimHesapPlaniModel : MuhTanimSayfa
    {
        public TanimHesapPlaniModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimOrtak.HesapPlaniAdresi);
        }
    }
}
