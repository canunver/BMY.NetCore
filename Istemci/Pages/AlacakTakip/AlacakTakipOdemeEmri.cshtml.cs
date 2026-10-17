using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Toplu Ödeme Emri (eski AlacakTakip/AlacakTakipOdemeEmri.aspx): ödeme emri bağlanmamış hesap hareketlerinden seçilenler için
    /// tek ödeme emri oluşturur ya da oluşturulacak belgeyi Excel'e yazar. Takip İşlemleri ekranından
    /// Sgb.pencere('../AlacakTakip/AlacakTakipOdemeEmri', 'Toplu Ödeme Emri') ile açılır.
    /// Menüde yoktur; yetkisi OdemeUclari.UstSayfalar'dan gelir. İşlemler wwwroot/js/atmodemeemri.js ile /api/alacaktakip/odemeemri uçlarından yapılır.
    /// </summary>
    public class AlacakTakipOdemeEmriModel : AlacakTakipSayfaModel
    {
        public AlacakTakipOdemeEmriModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => OdemeEmriUclari.Yetkili(kullanan, klasor);
    }
}
