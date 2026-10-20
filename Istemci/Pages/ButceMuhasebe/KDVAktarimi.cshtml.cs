using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>KDV Aktarımı, eski ButceMuhasebe/KDVAktarimi.aspx. Ortak ekran: _VergiAktarim.cshtml, /api/vergiaktarim/*.</summary>
    public class KDVAktarimiModel : VergiAktarimSayfa
    {
        public KDVAktarimiModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        public override int Tur { get { return VergiAktarimUclari.KDV; } }
    }
}
