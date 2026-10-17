using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Kapalı Birim Bilgileri (eski ButceMuhasebe/KapaliMuhasebeBirimForm.aspx); işlemler js/muhtanim-kapalibirim.js ile /api/muhtanim/kapalibirim uçlarından.</summary>
    public class KapaliMuhasebeBirimFormModel : MuhTanimSayfa
    {
        public KapaliMuhasebeBirimFormModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return MuhTanimOrtak.Yetkili(kullanan, Klasor, MuhTanimBasitUclari.KapaliBirimAdresi, false);
        }
    }
}
