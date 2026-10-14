using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Yatırım Programı İzleme ve Değerlendirme (eski ButceHazirlikv2/YATIzlemeDegerlendirme.aspx): ProjeGiris islemKod=1
    /// ekranında proje ya da alt proje seçilince sağdaki çerçevede açılır. Aşamaya göre Pr. ve Gerç. Durumu ile Tamamlama
    /// Raporu metinleri, ödenek tablosu, onay ve raporlar; sistem yöneticisine Genel Değerlendirme penceresi.
    /// İşlemler wwwroot/js/yatizleme.js ile /api/yatizleme uçlarından yapılır.
    /// </summary>
    public class YATIzlemeDegerlendirmeModel : YatAltSayfaModel
    {
        public YATIzlemeDegerlendirmeModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "YATIzlemeDegerlendirme";
    }
}
