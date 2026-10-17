using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Yatırım Projeleri Geçmiş Veri Girişi (eski ButceHazirlikv2/YatTopluVeriGirisForm.aspx; menüde "Toplu Veri Giriş"):
    /// yıl, aşama, sektör ve proje adına göre bütçe kaynaklarının proje tutarı, ödenek, revize ödenek, harcama ve kümülatif harcama
    /// rakamları tabloda girilir, toplu kaydedilir. İşlemler wwwroot/js/yattoplu.js ile /api/yattoplu uçlarından yapılır.
    /// </summary>
    public class YatTopluVeriGirisFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public YatTopluVeriGirisFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = YatTopluVeriUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
