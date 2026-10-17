using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Faiz Türü Tanımlama (eski AlacakTakip/TanimFaizTur.aspx): yıl, faiz türü adı ve oranı; Kaydet, Bul, Temizle.
    /// Listeden bir faiz türüne tıklanınca forma gelir. İşlemler wwwroot/js/faiztur.js ile /api/alacaktakip/faiztur uçlarından yapılır.
    /// </summary>
    public class TanimFaizTurModel : AlacakTakipSayfaModel
    {
        public TanimFaizTurModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => AlacakTakipTanimUclari.FaizTurYetkili(kullanan, klasor);
    }
}
