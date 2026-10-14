using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Yatırım Projeleri (eski ButceHazirlikv2/ProjeGiris.aspx; ?islemKod=1 ile İzleme Değerlendirme): solda arama ölçütleri ve
    /// proje ağacı, sağda seçilen projenin ya da alt projenin sayfası (ProjeDetay / AltProjeDetay, izlemede YATIzlemeDegerlendirme)
    /// çerçevede. İşlemler wwwroot/js/yatprojegiris.js ile /api/yatprojegiris uçlarından yapılır.
    /// </summary>
    public class ProjeGirisModel : YatAltSayfaModel
    {
        public ProjeGirisModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "ProjeGiris";
    }
}
