using Istemci.ButceForm;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceForm
{
    /// <summary>
    /// Birimlerin Hizmet Maliyetinin Tespitine İlişkin Bilgi Formu, taşra (eski ButceForm/HizmetMaliyetiTespitiTasra.aspx): seçilen yıl ve
    /// muhasebe / birim için hizmet maliyeti tutarları (yıl-2 gerçekleşme .. yıl+2 tahmin) listelenir, girilir ve kaydedilir; Excel raporu;
    /// sistem yöneticisi için toplu rapor, girmeyenler listesi ve onay (veri girişine kapatma) işlemleri.
    /// İşlemler wwwroot/js/hizmetmaliyetitasra.js ile /api/hizmetmaliyetitasra uçlarından yapılır.
    /// </summary>
    public class HizmetMaliyetiTespitiTasraModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public HizmetMaliyetiTespitiTasraModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = HizmetMaliyetiTasraUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
