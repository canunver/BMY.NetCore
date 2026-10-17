using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe Hazırlık Parametreleri (eski ButceHazirlikv2/ButceParametre.aspx): seçilen yılın bütçe hazırlık parametrelerinin
    /// durumu ve her birinin "Durum Değiştir" tuşu. İşlemler wwwroot/js/butceparametre.js ile /api/butceparametre uçlarından yapılır.
    /// </summary>
    public class ButceParametreModel : ButceTanimSayfaModel
    {
        public ButceParametreModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { ButceParametreUclari.SayfaAdresi };
    }
}
