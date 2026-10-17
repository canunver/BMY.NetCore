using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Eski Kurumsal Yapı Tanımlama (eski ButceHazirlikv2/TanimKurumsalYapi.aspx): kurumsal yapı kodu, adı, muhasebe birimi ve
    /// kurumsal kodu. Listele formdaki değerlere uyanları listeler, listeden seçilen forma gelir. İşlemler wwwroot/js/kurumsalyapi.js
    /// ile /api/kurumsalyapi uçlarından yapılır.
    /// </summary>
    public class TanimKurumsalYapiModel : ButceTanimSayfaModel
    {
        public TanimKurumsalYapiModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { KurumsalYapiUclari.SayfaAdresi };
    }
}
