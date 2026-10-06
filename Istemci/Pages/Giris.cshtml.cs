using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Istemci.Giris;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;

namespace Istemci.Pages
{
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class GirisModel : PageModel
    {
        private static readonly string[] ResimUzantilari = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly IWebHostEnvironment ortam;
        private readonly IAntiforgery sahtecilik;

        public GirisModel(IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            this.ortam = ortam;
            this.sahtecilik = sahtecilik;
        }

        public string YazilimAd { get; private set; }
        public string KurumAdi { get; private set; }
        public string Surum { get; private set; }
        public string SistemUyarisi { get; private set; }
        public string ArkaPlan { get; private set; }
        public string Logo { get; private set; }
        public string HatirlananKullanici { get; private set; }
        public string IstekJetonu { get; private set; }

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return Redirect(Url.Content("~/" + IstemciBaslangic.AcilisSayfasi));

            YazilimAd = Ayar.Oku("yazilimAd", "SGB.net");
            KurumAdi = Ayar.Degisken(DateTime.Now.Year, "/KURUMAD");
            SistemUyarisi = Ayar.Oku("sistemUyarisi");
            Surum = SurumBul();
            ArkaPlan = RastgeleArkaPlan();
            Logo = ortam.WebRootFileProvider.GetFileInfo("img/kurumlogo.jpg").Exists ? Url.Content("~/img/kurumlogo.jpg") : null;
            HatirlananKullanici = Request.Cookies["KulAd"];
            IstekJetonu = sahtecilik.GetAndStoreTokens(HttpContext).RequestToken;
            return Page();
        }

        /// <summary>wwwroot/img/giris klasöründeki resimlerden birini rastgele seçer. Klasör boşsa null döner (düz renkli arka plan).</summary>
        private string RastgeleArkaPlan()
        {
            // Dosyalar diskten değil web kökü sağlayıcısından okunur: kitaplıkla gelen wwwroot dosyaları
            // geliştirme sırasında exe'nin klasöründe değil, Istemci projesinin içindedir.
            string[] dosyalar = ortam.WebRootFileProvider.GetDirectoryContents("img/giris")
                .Where(d => !d.IsDirectory && ResimUzantilari.Contains(Path.GetExtension(d.Name).ToLowerInvariant()))
                .Select(d => d.Name)
                .ToArray();
            if (dosyalar.Length == 0) return null;

            string secilen = dosyalar[Random.Shared.Next(dosyalar.Length)];
            return Url.Content("~/img/giris/" + Uri.EscapeDataString(secilen));
        }

        private static string SurumBul()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return v != null ? v.Major + "." + v.Minor + "." + v.Build : "";
        }
    }
}
