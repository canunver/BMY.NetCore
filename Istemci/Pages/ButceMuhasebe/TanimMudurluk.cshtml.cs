using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Müdürlük Bilgileri (eski ButceMuhasebe/TanimMudurluk.aspx); işlemler js/muhtanim-mudurluk.js ile /api/muhtanim/mudurluk uçlarından.</summary>
    public class TanimMudurlukModel : MuhTanimSayfa
    {
        public TanimMudurlukModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimMudurlukUclari.MudurlukYetkili(kullanan, Klasor);
        }
    }
}
