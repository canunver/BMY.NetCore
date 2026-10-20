using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Sistem Kullanım Grafiği (eski KYM/CalisanKullaniciGrafik.aspx): tarih aralığında zaman dilimine göre sayfa görüntülenme
    /// sayılarını çizer; yalnızca ana tablo sayılır, arşiv okunmaz (js/calisankullanici.js, /api/calisankullanici/grafik).
    /// </summary>
    public class CalisanKullaniciGrafikModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public CalisanKullaniciGrafikModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = CalisanKullaniciUclari.GrafikYetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
