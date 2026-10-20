using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Borçlar ve Temlikler / Temlik Tanımlama (eski ButceMuhasebe/TaahhutBorclarVeTemliklerKayit.aspx): solda kıstaslar ve temlik
    /// listesi, sağda seçili temliğin formu (Kaydet / Sil / Temizle). İşlemler js/temlik.js ile /api/temlik uçlarından.
    /// </summary>
    public class TaahhutBorclarVeTemliklerKayitModel : MuhTanimSayfa
    {
        public TaahhutBorclarVeTemliklerKayitModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return TemlikUclari.YetkiVar(kullanan, Klasor);
        }
    }
}
