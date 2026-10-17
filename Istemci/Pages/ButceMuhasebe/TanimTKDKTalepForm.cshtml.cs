using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Tanım Talep Formu (eski ButceMuhasebe/TanimTKDKTalepForm.aspx); işlemler js/muhtanim-tkdk.js ile /api/muhtanim/tkdk uçlarından.</summary>
    public class TanimTKDKTalepFormModel : MuhTanimSayfa
    {
        public TanimTKDKTalepFormModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimListeUclari.TKDKYetkili(kullanan, Klasor);
        }
    }
}
