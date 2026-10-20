using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Damga Vergisi Aktarımı, eski ButceMuhasebe/DamgaVergisiAktarimi.aspx. Ortak ekran: _VergiAktarim.cshtml, /api/vergiaktarim/*.</summary>
    public class DamgaVergisiAktarimiModel : VergiAktarimSayfa
    {
        public DamgaVergisiAktarimiModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        public override int Tur { get { return VergiAktarimUclari.DAMGA; } }
    }
}
