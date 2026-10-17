using Istemci.AlacakTakip;
using Microsoft.AspNetCore.Hosting;
using TNS.KYM;

namespace Istemci.Pages.AlacakTakip
{
    /// <summary>
    /// Durum Tanımlama (eski AlacakTakip/TanimDurum.aspx): Alacak Takip dosya durumları. Açılışta listelenir; listeden tıklanan
    /// durum forma gelir. Kaydet eski sayfadaki gibi yalnızca alanları denetler (sunucuda durum kaydeden işlem yok).
    /// İşlemler wwwroot/js/atmdurum.js ile /api/alacaktakip/durum uçlarından yapılır.
    /// </summary>
    public class TanimDurumModel : AlacakTakipSayfaModel
    {
        public TanimDurumModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override bool YetkiVar(Kullanici kullanan, string klasor) => AlacakTakipTanimUclari.DurumYetkili(kullanan, klasor);
    }
}
