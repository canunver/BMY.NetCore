using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Taahhüt planı (eski ButceMuhasebe/Taahhutler.aspx): yıl / ay / tertip bazında taahhüt tutarları. js/taahhutalt.js, /api/taahhutkarti/plan/*.
    /// Menüde yoktur; Taahhüt Kartı'ndan açılır, yetkisini oradan (Taahhüt Kartı / Personel Taahhütleri) alır.
    /// </summary>
    public class TaahhutlerModel : MuhTanimSayfa
    {
        public TaahhutlerModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return TaahhutKartiUclari.AltSayfaYetkili(kullanan, Klasor, "../ButceMuhasebe/Taahhutler.aspx");
        }
    }
}
