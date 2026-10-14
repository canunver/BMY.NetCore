using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe Çalışmalarında Görevli Personel Listesi (eski ButceHazirlikv2/ButceCalismasiPersoneliForm.aspx): seçilen yıl ve alt
    /// faaliyet için bütçe çalışmalarında görevli personelin adı, unvanı, birimi ve iletişim bilgileri; listeleme, kaydetme, Excel'e
    /// yazdırma ve (sistem yöneticisi) önceki yıldan kopyalama. İşlemler wwwroot/js/butcepersonel.js ile /api/butcepersonel uçlarından yapılır.
    /// </summary>
    public class ButceCalismasiPersoneliFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ButceCalismasiPersoneliFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ButceCalismasiPersoneliUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
