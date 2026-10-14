using Istemci.Guncelleme;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.YazilimGuncelle
{
    /// <summary>
    /// Dosyalar (eski YazilimGuncelle/Dosyalar.aspx): uygulama klasöründeki klasörler ve dosyalar; indirme, silme, yükleme, klasör
    /// oluşturma ve dosya tarihlerini değiştirme. Teknik işler için yapılmıştır, yalnızca "stratek" kullanıcısı kullanır.
    /// İşlemler wwwroot/js/dosyalar.js ile /api/guncelleme uçlarından yapılır.
    /// </summary>
    public class DosyalarModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GuncellemeOrtak.DosyalarYetkili(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
