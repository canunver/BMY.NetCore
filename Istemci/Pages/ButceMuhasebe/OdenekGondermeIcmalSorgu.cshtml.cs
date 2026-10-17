using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// İcmal belgesi arama (eski ButceMuhasebe/OdenekGondermeIcmalSorgu.aspx): sorgu alanları, icmal listesi, Onayla / Yeni Yap.
    /// İşlemler wwwroot/js/odenekgondermeicmalliste.js ile /api/odenekgondermeicmal uçlarından yapılır.
    /// </summary>
    public class OdenekGondermeIcmalSorguModel : OdenekGondermeIcmalSayfaModel
    {
        public OdenekGondermeIcmalSorguModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAdres { get { return OdenekGondermeIcmalUclari.SorguAdres; } }
    }
}
