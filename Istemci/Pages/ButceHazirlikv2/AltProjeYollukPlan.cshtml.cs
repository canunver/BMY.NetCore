using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Projenin / alt projenin yolluk planı (eski ButceHazirlikv2/AltProjeYollukPlan.aspx): salt okunur tablo ve Yazdır.
    /// ProjeDetay / AltProjeDetay sekmesinde iframe olarak açılır. İşlemler wwwroot/js/yatalt-yolluk.js ile /api/yatalt/yolluk uçlarından.
    /// </summary>
    public class AltProjeYollukPlanModel : YatAltSayfaModel
    {
        public AltProjeYollukPlanModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "AltProjeYollukPlan";
    }
}
