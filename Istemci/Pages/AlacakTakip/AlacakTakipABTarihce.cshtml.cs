using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// AB Dosya Tarihçesi (eski AlacakTakip/AlacakTakipABTarihce.aspx?dosyaAnahtar=&amp;iPARDDonemi=): dosyada yapılan işlemler.
    /// AB Takip İşlemleri ekranının Tarihçe sekmesinde çerçeve olarak (?menuYok=1) açılır; menüde yoktur, yetkisi üst sayfadan gelir.
    /// Liste wwwroot/js/atmabtarihce.js ile /api/alacaktakip/ab/tarihce ucundan okunur.
    /// </summary>
    public class AlacakTakipABTarihceModel : AlacakTakipSayfaModel
    {
        public AlacakTakipABTarihceModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) =>
            AlacakTakipABTarihceUclari.Yetkili(kullanan, klasor);
    }
}
