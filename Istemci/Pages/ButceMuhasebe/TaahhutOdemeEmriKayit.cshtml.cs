using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Taahhüt hakediş ödeme emri kaydı (eski ButceMuhasebe/TaahhutOdemeEmriKayit.aspx). İşlemler js/taahhutodemeemri.js ile /api/taahhutodemeemri/* (TaahhutOdemeEmriUclari.cs).
    /// Menüde yoktur; Taahhüt Kartı'ndan açılır, yetkisini oradan (Taahhüt Kartı / Personel Taahhütleri) alır.
    /// </summary>
    public class TaahhutOdemeEmriKayitModel : MuhTanimSayfa
    {
        public TaahhutOdemeEmriKayitModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        protected override bool YetkiDenetle(Kullanici kullanan)
        {
            return TaahhutKartiUclari.AltSayfaYetkili(kullanan, Klasor, TaahhutOdemeEmriUclari.SayfaAdresi) && TaahhutKartiUclari.OdemeYetkili(kullanan);
        }
    }
}
