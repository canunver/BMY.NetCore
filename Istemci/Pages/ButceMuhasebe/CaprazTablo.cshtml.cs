using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Çapraz Tablo (eski ButceMuhasebe/CaprazTablo.aspx): solda sorgu alanları, sağda satır ve sütun hesap kodları ile sütun türü;
    /// Tablo Göster Excel çapraz tablosunu (CaprazTablo.xltx) indirir, sorgular adıyla saklanır (Kaydet / Farklı Kaydet / Kayıtlı
    /// Sorgular). Sayfa yalnızca iskeleti basar; işlemler wwwroot/js/capraztablo.js ile /api/capraztablo uçlarından
    /// (Muhasebe/CaprazTabloUclari.cs) yapılır.
    /// </summary>
    public class CaprazTabloModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public CaprazTabloModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = CaprazTabloUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
