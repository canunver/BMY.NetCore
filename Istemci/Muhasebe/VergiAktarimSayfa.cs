using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Vergi dairesi aktarım sayfalarının ortak modeli (Damga Vergisi, KDV Tevkifatı, KDV, Muhtasar). Her sayfa yalnızca türünü
    /// verir; ekran Pages/ButceMuhasebe/_VergiAktarim.cshtml, işlemler js/vergiaktarim.js ile /api/vergiaktarim/* uçlarından.
    /// </summary>
    public abstract class VergiAktarimSayfa : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected VergiAktarimSayfa(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        /// <summary>ENUMVergiDairesiAktarim: 1 Damga Vergisi, 2 KDV Tevkifatı, 3 KDV, 4 Muhtasar.</summary>
        public abstract int Tur { get; }

        public string Baslik { get { return VergiAktarimUclari.Baslik(Tur); } }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = VergiAktarimUclari.SayfaYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, Tur);
        }
    }
}
