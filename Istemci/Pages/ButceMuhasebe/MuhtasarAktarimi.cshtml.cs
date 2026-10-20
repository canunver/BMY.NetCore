using Istemci.Muhasebe;
using Microsoft.AspNetCore.Hosting;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>Muhtasar Aktarımı, eski ButceMuhasebe/MuhtasarAktarimi.aspx. Ortak ekran: _VergiAktarim.cshtml, /api/vergiaktarim/*.</summary>
    public class MuhtasarAktarimiModel : VergiAktarimSayfa
    {
        public MuhtasarAktarimiModel(IWebHostEnvironment ortam) : base(ortam)
        {
        }

        public override int Tur { get { return VergiAktarimUclari.MUHTASAR; } }
    }
}
