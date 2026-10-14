using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Birim Görüşme Ekranı (eski ButceHazirlikv2/GorusmeCari.aspx): seçilen faaliyet/proje ve cari ekonomik kod için kurumun tertiplerinde
    /// SGB görüşme tutarları; en üstte salt okunur TOPLAM satırı, onay işlemleri ve Rapor Al menüsü. Adresle (?yil=&amp;tertip=) açılınca
    /// yalnızca verilen tertip gösterilir. İşlemler wwwroot/js/gorusmecari.js ile /api/gorusmecari uçlarından yapılır.
    /// </summary>
    public class GorusmeCariModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public GorusmeCariModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GorusmeCariUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
