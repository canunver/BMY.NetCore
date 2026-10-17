using System;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.Muhasebe
{
    /// <summary>
    /// Muhasebe Tanımları sayfalarının ortak sayfa modeli: yetki (sistem / muhasebe yöneticisi ya da menü yetkisi) ve yıl sınırları.
    /// Sayfalar yalnızca iskeleti basar; işlemler wwwroot/js/muhtanim.js ve sayfanın kendi betiğiyle /api/muhtanim uçlarından yapılır.
    /// </summary>
    public abstract class MuhTanimSayfa : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        protected MuhTanimSayfa(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        protected string Klasor { get { return ortam.ContentRootPath; } }

        public bool Yetkili { get; private set; }

        /// <summary>Sistem yöneticisi ya da muhasebe yöneticisi (eski sayfalarda bazı tuşlar yalnızca bunlara görünür).</summary>
        public bool Yonetici { get; private set; }

        public int BuYil { get { return DateTime.Now.Year; } }

        /// <summary>Yıl kutusunun alt sınırı (eski sayfalarda 2005'ten başlayan yıl listesi).</summary>
        public int IlkYil { get { return 2005; } }

        protected abstract bool YetkiDenetle(Kullanici kullanan);

        public virtual void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = kullanan != null && YetkiDenetle(kullanan);
            Yonetici = kullanan != null && MuhTanimOrtak.Yonetici(kullanan);
        }
    }
}
