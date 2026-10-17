using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Alt Faaliyet İlişkileri (eski ButceHazirlikv2/AltFaaliyetIliski.aspx): ?tur=1 alt faaliyet - faaliyet bağı,
    /// ?tur=2 alt faaliyet - DPT proje bağı (aşamaya göre). Satırdaki düğmelerle faaliyet / proje seçilir, gider ve gelir tertipleri
    /// (TertipTanimFormu) ile birimler (kurumsal yapı şablonları) açılır. İşlemler wwwroot/js/altfaaliyetiliski.js ile
    /// /api/altfaaliyetiliski uçlarından yapılır.
    /// </summary>
    public class AltFaaliyetIliskiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public AltFaaliyetIliskiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int Tur { get; private set; }

        public void OnGet(int tur = 1)
        {
            Tur = AltFaaliyetIliskiUclari.TurBul(tur);
            Yetkili = AltFaaliyetIliskiUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, Tur);
        }
    }
}
