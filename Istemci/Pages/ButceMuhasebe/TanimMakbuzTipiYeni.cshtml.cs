using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Makbuz Tipi Bilgileri, yeni (eski ButceMuhasebe/TanimMakbuzTipiYeni.aspx; Aktif / Pasif durumlu); işlemler js/muhtanim-makbuztipi.js ile /api/muhtanim/makbuztipi uçlarından.</summary>
    public class TanimMakbuzTipiYeniModel : MuhTanimSayfa
    {
        public TanimMakbuzTipiYeniModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimBasitUclari.MakbuzTipiYeniAdresi);
        }
    }
}
