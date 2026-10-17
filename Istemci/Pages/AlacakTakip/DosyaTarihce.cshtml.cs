using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Dosya Tarihçesi (eski AlacakTakip/DosyaTarihce.aspx?yil=&amp;dosyaNo=): dosyanın durum değişiklikleri. Takip İşlemleri ekranından
    /// pencere olarak açılır; menüde yoktur, yetkisi üst sayfadan gelir. Liste wwwroot/js/dosyatarihce.js ile okunur.
    /// </summary>
    public class DosyaTarihceModel : AlacakTakipSayfaModel
    {
        public DosyaTarihceModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) =>
            AlacakTakipListeUclari.AltSayfaYetkili(kullanan, klasor, AlacakTakipListeUclari.TarihceAdresi);
    }
}
