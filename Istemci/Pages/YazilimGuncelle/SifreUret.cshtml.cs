using Istemci.Guncelleme;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.YazilimGuncelle
{
    /// <summary>
    /// Şifre Üret (eski YazilimGuncelle/SifreUret.aspx): verilen metni ortak anahtarla şifreler ya da çözer; "xml" seçiliyse appSettings
    /// biçimindeki XML'in bütün value değerleri işlenir. Yalnızca sistem yöneticisi. İşlem wwwroot/js/sifreuret.js ile /api/guncelleme/sifre ucundan.
    /// </summary>
    public class SifreUretModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GuncellemeOrtak.SifreUretYetkili(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
