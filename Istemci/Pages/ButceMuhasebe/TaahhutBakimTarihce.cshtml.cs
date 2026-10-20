using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Taahhüt kartı bakım tarihçesi (eski ButceMuhasebe/TaahhutBakimTarihce.aspx). js/taahhutalt.js, /api/taahhutkarti/bakim/*.
    /// Menüde yoktur; Taahhüt Kartı'ndan açılır, yetkisini oradan (Taahhüt Kartı / Personel Taahhütleri) alır.
    /// </summary>
    public class TaahhutBakimTarihceModel : MuhTanimSayfa
    {
        public TaahhutBakimTarihceModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return TaahhutKartiUclari.AltSayfaYetkili(kullanan, Klasor, "../ButceMuhasebe/TaahhutBakimTarihce.aspx");
        }
    }
}
