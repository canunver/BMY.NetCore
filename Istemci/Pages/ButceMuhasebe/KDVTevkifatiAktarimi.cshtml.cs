using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>KDV Tevkifatı Aktarımı, eski ButceMuhasebe/KDVTevkifatiAktarimi.aspx. Ortak ekran: _VergiAktarim.cshtml, /api/vergiaktarim/*.</summary>
    public class KDVTevkifatiAktarimiModel : VergiAktarimSayfa
    {
        public KDVTevkifatiAktarimiModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        public override int Tur { get { return VergiAktarimUclari.TEVKIFAT; } }
    }
}
