using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Taahhüt kartına belge (dosya) ekleme (eski ButceMuhasebe/TanimBelgeEkle.aspx, OrtakBelge). js/taahhutalt.js, /api/taahhutkarti/belge-ekle/*.
    /// Menüde yoktur; Taahhüt Kartı'ndan açılır, yetkisini oradan (Taahhüt Kartı / Personel Taahhütleri) alır.
    /// </summary>
    public class TanimBelgeEkleModel : MuhTanimSayfa
    {
        public TanimBelgeEkleModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return TaahhutKartiUclari.AltSayfaYetkili(kullanan, Klasor, "../ButceMuhasebe/TanimBelgeEkle.aspx");
        }
    }
}
