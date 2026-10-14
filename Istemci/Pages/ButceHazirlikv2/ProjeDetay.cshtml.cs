using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Proje bilgileri (eski ButceHazirlikv2/ProjeDetay.aspx): ProjeGiris'in sağındaki çerçevede açılır. Genel bilgiler, il / ilçeler,
    /// alt sayfa sekmeleri (Amaç, İlgililer, Gerekçe, Yolluk, Yetkili Kullanıcılar, Ek Dosyalar) ve tertipler.
    /// İşlemler wwwroot/js/yatprojedetay.js ile /api/yatprojegiris uçlarından yapılır.
    /// </summary>
    public class ProjeDetayModel : YatAltSayfaModel
    {
        public ProjeDetayModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "ProjeDetay";
    }
}
