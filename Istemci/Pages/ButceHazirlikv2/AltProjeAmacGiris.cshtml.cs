using Istemci.ButceHazirlik;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Amaç ve Kapsam (eski ButceHazirlikv2/AltProjeAmacGiris.aspx). ProjeDetay / AltProjeDetay'ın sekmesinde çerçevede açılır.
    /// Alanlar wwwroot/js/yatbilgi.js ile /api/yatprojegiris/bilgi ucundan okunur ve saklanır.
    /// </summary>
    public class AltProjeAmacGirisModel : YatAltSayfaModel
    {
        public AltProjeAmacGirisModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAd => "AltProjeAmacGiris";
    }
}
