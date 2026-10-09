using System;
using System.Collections.Generic;
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

        /// <summary>Sayfanın altındaki dil seçenekleri.</summary>
        public List<DilSecenegi> Diller { get; private set; } = new List<DilSecenegi>();

        /// <summary>giris.js'in kullandığı, isteğin dilindeki metinler.</summary>
        public Dictionary<string, string> Metinler { get; private set; } = new Dictionary<string, string>();

        public class DilSecenegi
        {
            public string Kod { get; set; }
            public string Ad { get; set; }
            public string Adres { get; set; }
            public bool Secili { get; set; }
        }

        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return Redirect(Url.Content("~/" + IstemciBaslangic.AcilisSayfasi));

            // Dil seçimi (?dil=en): çereze yazılır ve sayfa o dille yeniden açılır (eski GenelGiris.DilDegistir).
            string donus = Request.Query["ReturnUrl"];
            string donusEki = string.IsNullOrEmpty(donus) ? "" : "&ReturnUrl=" + Uri.EscapeDataString(donus);
            string secilen = Request.Query["dil"];
            if (Dil.Gecerli(secilen))
            {
                Dil.CerezYaz(HttpContext, secilen);
                return Redirect(Url.Content("~/Giris") + (donusEki == "" ? "" : "?" + donusEki.Substring(1)));
            }

            foreach (string kod in Dil.Desteklenen())
                Diller.Add(new DilSecenegi { Kod = kod, Ad = Dil.Ad(kod), Adres = "?dil=" + kod + donusEki, Secili = kod == Dil.Aktif });

            Metinler["kodParolaGir"] = Metin.Menu("Giris_KodParolaGir");
            Metinler["kodGir"] = Metin.Menu("Giris_KodGir");
            Metinler["sunucuYok"] = Metin.Menu("Giris_SunucuYok");

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
