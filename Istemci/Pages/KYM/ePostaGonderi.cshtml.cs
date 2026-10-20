using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Önceki e-posta Gönderimleri Durum İzleme (eski KYM/ePostaGonderi.aspx): daha önce gönderilen e-postaları, gönderilecek ve
    /// gönderilen kişi sayılarını listeler; kişi listeleri Excel olarak alınır, gönderilemeyenlere tekrar gönderim başlatılır.
    /// Menüde yoktur, e-posta Gönder sayfasındaki "Önceki Gönderiler" düğmesiyle açılır. Yalnızca sistem yöneticisi ya da destek
    /// yöneticisi girer (eski Page_Load). İşlemler wwwroot/js/epostagonderi.js ile /api/eposta uçlarından yapılır.
    /// </summary>
    public class ePostaGonderiModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = EPostaUclari.GonderiYetkiVar(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
