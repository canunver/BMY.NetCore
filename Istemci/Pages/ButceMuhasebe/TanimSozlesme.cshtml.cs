using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Sözleşme koşulları / garanti / teminat şartları metni (eski ButceMuhasebe/TanimSozlesme.aspx). js/taahhutalt.js, /api/taahhutkarti/sozlesme-metni*.
    /// Menüde yoktur; Taahhüt Kartı'ndan açılır, yetkisini oradan (Taahhüt Kartı / Personel Taahhütleri) alır.
    /// </summary>
    public class TanimSozlesmeModel : MuhTanimSayfa
    {
        public TanimSozlesmeModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return TaahhutKartiUclari.AltSayfaYetkili(kullanan, Klasor, "../ButceMuhasebe/TanimSozlesme.aspx");
        }
    }
}
