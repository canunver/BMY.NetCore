using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Sistem Durum Dosya Listesi (eski KYM/HataDosya.aspx): hata dosyası klasöründeki dosyalar; açma ve silme
    /// (js/hatadosya.js, /api/hatadosya/*).
    /// </summary>
    public class HataDosyaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public HataDosyaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = HataDosyaUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
