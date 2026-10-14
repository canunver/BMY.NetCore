using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Yatırım Projeleri Vize Tavanı Girişi (eski ButceHazirlikv2/TavanYatirimVize.aspx): seçilen birim + ekonomik kod + finansman (ve proje)
    /// için faaliyet/projelerin üç yıllık vize tavanları, proje grubu ara toplamları; onay işlemleri, toplu kapatma, Yazdır / Detay Yaz ve
    /// Rapor Al menüsü. İşlemler wwwroot/js/tavanvize.js ile /api/tavanvize uçlarından yapılır.
    /// </summary>
    public class TavanYatirimVizeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TavanYatirimVizeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = TavanYatirimVizeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
