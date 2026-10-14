using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Alt proje bilgileri (eski ButceHazirlikv2/AltProjeDetay.aspx): ProjeGiris'in sağındaki çerçevede açılır. Genel bilgiler, il / ilçeler,
    /// alt sayfa sekmeleri (Amaç, İlgililer, Gerekçe, Detay Program, Yolluk, Yetkili Kullanıcılar, Ek Dosyalar), tertipler ve her bütçe kaynağı için bir sekme (AltProjeButceKaynakGiris).
    /// İşlemler wwwroot/js/yatprojedetay.js ile /api/yatprojegiris uçlarından yapılır.
    /// </summary>
    public class AltProjeDetayModel : YatAltSayfaModel
    {
        public AltProjeDetayModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "AltProjeDetay";
    }
}
