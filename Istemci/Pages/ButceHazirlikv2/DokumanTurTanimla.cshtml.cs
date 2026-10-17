using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Döküman Türü (eski ButceHazirlikv2/DokumanTurTanimla.aspx + DokumanTurTanimlaDetay.aspx): Yatırım projelerine eklenen dokümanların türleri. Solda liste, sağda seçilenin
    /// kodu ve adı. İşlemler wwwroot/js/yatsozluk.js ile /api/yatsozluk/dokumantur uçlarından yapılır.
    /// </summary>
    public class DokumanTurTanimlaModel : ButceTanimSayfaModel
    {
        public DokumanTurTanimlaModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string[] Adresler => new[] { YatSozlukUclari.DokumanTurAdresi };
    }
}
