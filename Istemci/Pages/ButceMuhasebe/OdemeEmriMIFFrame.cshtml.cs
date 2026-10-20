using System.Net;
using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödeme Emri / MİF ana çerçevesi (eski ButceMuhasebe/OdemeEmriMIFFrame.aspx): "Belge Düzenleme" (OdemeEmriMIF2),
    /// "Belge Arama" (OdemeEmriMIFSorgu) ve "Muavin Defteri" sekmeleri. Menüdeki üç seçenek (düz, ?yeni=1, ?yeni=1&amp;baslikSayfasi=1)
    /// bu sayfayı açar. yil / belgeNo verilirse belge formda açılır; aramaYok=1 ise yalnız form sekmesi görünür.
    /// </summary>
    public class OdemeEmriMIFFrameModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public OdemeEmriMIFFrameModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public bool AramaYok { get; private set; }
        public string FormParametreleri { get; private set; } = "menuYok=1";

        public void OnGet()
        {
            Yetkili = OdemeEmriMIFUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
            AramaYok = (string)Request.Query["aramaYok"] == "1";
            string yil = Request.Query["yil"];
            string belgeNo = Request.Query["belgeNo"];
            string baslik = Request.Query["baslikSayfasi"];
            FormParametreleri = "menuYok=1"
                + "&yil=" + WebUtility.UrlEncode(yil ?? "")
                + "&belgeNo=" + WebUtility.UrlEncode(belgeNo ?? "")
                + "&aramaYok=" + (AramaYok ? "1" : "")
                + "&baslikSayfasi=" + WebUtility.UrlEncode(baslik ?? "");
        }
    }
}
