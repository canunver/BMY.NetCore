using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// İlgililer (eski ButceHazirlikv2/AltProjeIlgiliGiris.aspx): hazırlayan, projeyi yürüten ve bütçe koordinasyonunu sağlayan birimin ilgilileri. ProjeDetay / AltProjeDetay'ın sekmesinde çerçevede açılır.
    /// Alanlar wwwroot/js/yatbilgi.js ile /api/yatprojegiris/bilgi ucundan okunur ve saklanır.
    /// </summary>
    public class AltProjeIlgiliGirisModel : YatAltSayfaModel
    {
        public AltProjeIlgiliGirisModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "AltProjeIlgiliGiris";
    }
}
