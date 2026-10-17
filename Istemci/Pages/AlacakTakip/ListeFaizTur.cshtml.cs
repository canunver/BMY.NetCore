using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Faiz Türü Seçimi (eski AlacakTakip/ListeFaizTur.aspx?listeTur=faizTur): Alacak Takip ekranlarından pencere olarak açılır
    /// (Sgb.pencere ya da Sgb.faizTurSec). Yıl seçilip Ara'ya basılır; tıklanan faiz türü açan sayfaya bildirilir ve pencere kapanır.
    /// Menüde yoktur; yetkisi AlacakTakipTanimUclari.FaizTurListeUstSayfalar'dan gelir. İşlemler wwwroot/js/faiztursec.js ile yapılır.
    /// </summary>
    public class ListeFaizTurModel : AlacakTakipSayfaModel
    {
        public ListeFaizTurModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => AlacakTakipTanimUclari.FaizTurListeYetkili(kullanan, klasor);
    }
}
