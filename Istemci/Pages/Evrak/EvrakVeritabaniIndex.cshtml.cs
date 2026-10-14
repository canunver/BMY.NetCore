using Istemci.Guncelleme;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Evrak
{
    /// <summary>
    /// Veritabanı Index İşlemleri (eski Evrak/EvrakVeritabaniIndex.aspx): seçilen veri tabanındaki indexler listelenir; seçilenler
    /// rebuild / reorganize / drop edilir. Evrak veri tabanında, tanımlı indexlerden eksik olanlar oluşturulur. Yalnızca sistem yöneticisi.
    /// İşlemler wwwroot/js/vtindex.js ile /api/vtindex uçlarından yapılır. Adresteki ?veritabani= ile o veri tabanı seçili açılır.
    /// </summary>
    public class EvrakVeritabaniIndexModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GuncellemeOrtak.Yonetici(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
