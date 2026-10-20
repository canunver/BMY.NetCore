using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// İzin Türleri (eski InsanKaynaklari/TanimIzinTur.aspx): üstte form, altta izin türleri listesi.
    /// İşlemler wwwroot/js/izintur.js ile /api/izintur uçlarından yapılır.
    /// </summary>
    public class TanimIzinTurModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimIzinTurModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = IzinTurUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
