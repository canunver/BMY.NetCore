using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Gerekçe ve Hedefler (eski ButceHazirlikv2/AltProjeGerekceGiris.aspx). ProjeDetay / AltProjeDetay'ın sekmesinde çerçevede açılır.
    /// Alanlar wwwroot/js/yatbilgi.js ile /api/yatprojegiris/bilgi ucundan okunur ve saklanır.
    /// </summary>
    public class AltProjeGerekceGirisModel : YatAltSayfaModel
    {
        public AltProjeGerekceGirisModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "AltProjeGerekceGiris";
    }
}
