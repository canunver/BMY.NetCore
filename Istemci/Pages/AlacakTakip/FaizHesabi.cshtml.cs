using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Faiz Hesaplama ve Detay (eski AlacakTakip/FaizHesabi.aspx): ana para, faiz başlangıç tarihi, tecil durumu ve işlem tarihine göre
    /// faizi hesaplar, hesabın ayrıntısını gösterir. Hesap wwwroot/js/faizhesabi.js ile /api/alacaktakip/faizhesabi/hesapla ucundan yapılır.
    /// </summary>
    public class FaizHesabiModel : AlacakTakipSayfaModel
    {
        public FaizHesabiModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => FaizHesabiUclari.Yetkili(kullanan, klasor);
    }
}
