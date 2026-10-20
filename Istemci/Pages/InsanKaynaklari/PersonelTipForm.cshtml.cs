using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel Tipleri (eski InsanKaynaklari/PersonelTipForm.aspx): solda personel tipleri listesi, sağda seçilenin bilgileri.
    /// İşlemler wwwroot/js/personeltip.js ile /api/personeltip uçlarından yapılır.
    /// </summary>
    public class PersonelTipFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public PersonelTipFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = PersonelTipUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
