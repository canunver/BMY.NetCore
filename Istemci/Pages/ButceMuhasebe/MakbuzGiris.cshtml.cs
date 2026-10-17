using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Makbuz formu (eski ButceMuhasebe/MakbuzGiris.aspx; Makbuz Bilgileri listesinden pencere olarak açılır, menüde yoktur):
    /// makbuz bilgileri, hesap satırları, açıklama tanımları, Kaydet / Yazdır (eski MakbuzYazdir.aspx, PDF) / Ödendi.
    /// Adres: MakbuzGiris?yil=&amp;muhasebe=&amp;birim=&amp;makbuzNo= . Yetki üst sayfa Makbuz Bilgileri'nden gelir.
    /// İşlemler wwwroot/js/makbuzgiris.js ile /api/makbuz uçlarından (Muhasebe/MakbuzUclari.cs) yapılır.
    /// </summary>
    public class MakbuzGirisModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MakbuzGirisModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MakbuzUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, true);
        }
    }
}
