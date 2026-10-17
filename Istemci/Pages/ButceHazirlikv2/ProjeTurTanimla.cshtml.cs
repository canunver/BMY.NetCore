using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Proje Türü (eski ButceHazirlikv2/ProjeTurTanimla.aspx + ProjeTurTanimlaDetay.aspx): Yatırım projelerinin türleri. Solda liste, sağda seçilenin
    /// kodu ve adı. İşlemler wwwroot/js/yatsozluk.js ile /api/yatsozluk/projetur uçlarından yapılır.
    /// </summary>
    public class ProjeTurTanimlaModel : ButceTanimSayfaModel
    {
        public ProjeTurTanimlaModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { YatSozlukUclari.ProjeTurAdresi };
    }
}
