using System.Collections.Generic;
using Istemci.Giris;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.OrtakSayfa
{
    /// <summary>
    /// Şifremi Unuttum (eski OrtakSayfa/SifremiUnuttum.aspx; giriş sayfasındaki bağlantıyla açılır, giriş gerektirmez).
    /// Bütün işlem js/sifremiunuttum.js ve /api/giris/sifremi-unuttum ile yapılır (Istemci/Giris/SifremiUnuttumUclari.cs).
    /// </summary>
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class SifremiUnuttumModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;
        private readonly IAntiforgery sahtecilik;

        public SifremiUnuttumModel(IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            this.ortam = ortam;
            this.sahtecilik = sahtecilik;
        }

        public string YazilimAd { get; private set; }
        public string ArkaPlan { get; private set; }
        public string Logo { get; private set; }
        public string IstekJetonu { get; private set; }
        public bool Kullanilabilir { get; private set; }
        public Dictionary<string, string> Metinler { get; private set; } = new Dictionary<string, string>();

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return Redirect(Url.Content("~/" + IstemciBaslangic.AcilisSayfasi));

            YazilimAd = Ayar.Oku("yazilimAd", "SGB.net");
            Kullanilabilir = SifremiUnuttumUclari.Kullanilabilir();
            ArkaPlan = GirisModel.RastgeleArkaPlan(ortam, Url);
            Logo = ortam.WebRootFileProvider.GetFileInfo("img/kurumlogo.jpg").Exists ? Url.Content("~/img/kurumlogo.jpg") : null;
            IstekJetonu = sahtecilik.GetAndStoreTokens(HttpContext).RequestToken;

            Metinler["bilgiGir"] = Metin.Menu("Sifre_BilgiGir");
            Metinler["guvenlikKoduGir"] = Metin.Menu("Giris_GuvenlikKoduGir");
            Metinler["sunucuYok"] = Metin.Menu("Giris_SunucuYok");
            return Page();
        }
    }
}
