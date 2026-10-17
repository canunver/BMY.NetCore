using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Ödeme Bilgileri (eski AlacakTakip/Odeme.aspx): bir alacak dosyasının taksitleri, ödeme planı, ana paradan faiz hesabı,
    /// açılış / faiz / ödeme tahakkukları (ödeme emri) ve taksit listesi (Excel). Takip İşlemleri ekranından
    /// Sgb.pencere('../AlacakTakip/Odeme?yil=&amp;dosyaNo=&amp;paraBirimi=', 'Ödeme Bilgileri') ile açılır.
    /// Menüde yoktur; yetkisi OdemeUclari.UstSayfalar'dan gelir. İşlemler wwwroot/js/atmodeme.js ile /api/alacaktakip/odeme uçlarından yapılır.
    /// </summary>
    public class OdemeModel : AlacakTakipSayfaModel
    {
        public OdemeModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => OdemeUclari.Yetkili(kullanan, klasor);
    }
}
