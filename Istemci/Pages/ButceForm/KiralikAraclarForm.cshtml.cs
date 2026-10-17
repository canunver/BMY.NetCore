using Istemci.ButceForm;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceForm
{
    /// <summary>
    /// Kiralık Araç Bilgi Formu (eski ButceForm/KiralikAraclarForm.aspx): muhasebe birimlerinin araç kiralama sözleşmeleri
    /// (imza / iş tarihleri, firma, süreç) ve her sözleşmede kiralanan araçlar (taşıt tipi / cinsi, adet, süre, birim fiyat, kaynak).
    /// İşlemler wwwroot/js/kiralikarac.js ile /api/kiralikarac uçlarından yapılır.
    /// </summary>
    public class KiralikAraclarFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public KiralikAraclarFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = KiralikAracUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
