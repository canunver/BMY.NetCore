using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Yatırım Projeleri Yönetim (eski ButceHazirlikv2/YatProjeYonetim.aspx): sekmeler hâlinde aşama kopyalama, veri girişine
    /// açma / kapatma (eski YATProjeOnay.aspx) ve Projeler / Alt Projeler / Kaynaklar sözlükleri (eski YATTanimSozluk.aspx?tur=1/2/3).
    /// İşlemler wwwroot/js/yatproje.js ile /api/yatproje uçlarından yapılır.
    /// </summary>
    public class YatProjeYonetimModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public YatProjeYonetimModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = YatProjeYonetimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
