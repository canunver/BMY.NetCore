using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Tavan üstü ödenek talebi (eski ButceHazirlikv2/TavanUstuOdenek.aspx; bütçe teklifi ekranındaki "Tavan Üstü" tuşu,
    /// ?yil=..&amp;senaryo=..&amp;tertipler=a,b,..): seçilen tertiplerin ödenekleri yanında 3 yıllık ek ödenek talebi ve gerekçe girilir.
    /// İşlemler wwwroot/js/tavanustu.js ile /api/tavanustu uçlarından yapılır.
    /// </summary>
    public class TavanUstuOdenekModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanUstuOdenekModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int Yil { get; private set; }
        public string Senaryo { get; private set; }
        public string Tertipler { get; private set; }

        public void OnGet(int yil = 0, string senaryo = "", string tertipler = "")
        {
            Yil = yil;
            Senaryo = (senaryo ?? "").Trim();
            Tertipler = (tertipler ?? "").Trim();
            Yetkili = TavanUstuUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
