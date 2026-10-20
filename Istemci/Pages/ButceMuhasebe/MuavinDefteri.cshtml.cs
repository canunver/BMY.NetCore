using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Muavin Defteri (eski ButceMuhasebe/MuavinDefteri.aspx): Ödeme Emri / MİF çerçevesinin "Muavin Defteri" sekmesi; çerçevede
    /// MuavinDefteri?menuYok=1 olarak açılır. Sorgu alanları, hesap koduna göre gruplu satırlar (DEVİR, bakiye, grup toplamları),
    /// Yazdır ve Yazdır2 Excel raporları. Sayfa yalnızca iskeleti basar; işlemler wwwroot/js/muavindefteri.js ile
    /// /api/muavindefteri uçlarından (Muhasebe/MuavinDefteriUclari.cs) yapılır. Yetki üst sayfadan (Ödeme Emri / MİF çerçevesi).
    /// </summary>
    public class MuavinDefteriModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MuavinDefteriModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MuavinDefteriUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
