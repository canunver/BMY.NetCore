using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Personel İzin Devir Bilgileri (eski InsanKaynaklari/PersonelIzinDevir.aspx): üstte ölçütler, altta personelin yıllara göre
    /// devreden / yıl içi izinleri. İşlemler wwwroot/js/izindevir.js ile /api/izindevir uçlarından yapılır.
    /// </summary>
    public class PersonelIzinDevirModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public PersonelIzinDevirModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = PersonelIzinDevirListeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
