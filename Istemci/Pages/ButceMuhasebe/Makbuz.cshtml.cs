using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Makbuz Bilgileri (eski ButceMuhasebe/Makbuz.aspx): kriterlere göre makbuz listesi, yeni makbuz / makbuz açma (MakbuzGiris
    /// penceresi), seçilenleri iptal etme, Kesilen Makbuzlar ve Hesap Bazında Rapor (Excel), makbuzlardan ödeme emri oluşturma.
    /// İşlemler wwwroot/js/makbuz.js ile /api/makbuz uçlarından (Muhasebe/MakbuzUclari.cs) yapılır.
    /// </summary>
    public class MakbuzModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MakbuzModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MakbuzUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
