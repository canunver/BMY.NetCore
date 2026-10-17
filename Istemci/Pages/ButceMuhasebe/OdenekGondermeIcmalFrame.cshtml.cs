using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödenek Gönderme / Tenkis İcmali sayfalarının ortak modeli. Çerçevenin yetkisi menüden, alt sayfalarınki çerçeveden gelir
    /// (Muhasebe/OdenekGondermeIcmalUclari.YetkiVar).
    /// </summary>
    public abstract class OdenekGondermeIcmalSayfaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected OdenekGondermeIcmalSayfaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        /// <summary>Sayfanın menu.xml biçimindeki adresi.</summary>
        protected abstract string SayfaAdres { get; }

        public string Baslik { get { return "Ödenek Gönderme/Tenkis İcmali"; } }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = OdenekGondermeIcmalUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, SayfaAdres);
        }
    }

    /// <summary>
    /// Ödenek Gönderme / Tenkis İcmali ana çerçevesi (eski ButceMuhasebe/OdenekGondermeIcmalFrame.aspx): "Belge Düzenleme"
    /// (OdenekGondermeIcmalForm) ve "Belge Arama" (OdenekGondermeIcmalSorgu) sekmeleri.
    /// </summary>
    public class OdenekGondermeIcmalFrameModel : OdenekGondermeIcmalSayfaModel
    {
        public OdenekGondermeIcmalFrameModel(IWebHostEnvironment ortam) : base(ortam) { }

        protected override string SayfaAdres { get { return OdenekGondermeIcmalUclari.FrameAdres; } }
    }
}
