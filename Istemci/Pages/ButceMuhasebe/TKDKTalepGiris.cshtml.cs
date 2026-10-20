using System;
using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Talep Giriş, eski ButceMuhasebe/TKDKTalepGiris.aspx (Talep Listesi'nden pencere olarak açılır, menüde yoktur): muhasebe / birim,
    /// talep türü, yıl / ay, mevcut bakiye, gerekçe ve satırlar; Kaydet / Sil / Yazdır. Adres: TKDKTalepGiris?kod= (boş: yeni talep).
    /// Yetki üst sayfa Talep Listesi'nden gelir (js/tkdktalep.js, /api/tkdktalep/form/*).
    /// </summary>
    public class TKDKTalepGirisModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TKDKTalepGirisModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public int BuYil { get; } = DateTime.Now.Year;

        public void OnGet()
        {
            Yetkili = TKDKTalepUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, TKDKTalepUclari.FormAdresi);
        }
    }
}
