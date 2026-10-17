using Istemci.ButceUygulama;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceUygulama
{
    /// <summary>
    /// AHP / AGP Girişi (eski ButceUygulama/APGiris.aspx): ?tur=gider ayrıntılı harcama programı, ?tur=gelir ayrıntılı gelir programı.
    /// Seçilen yıl, aşama ve tertip kodlarına göre tertiplerin KBÖ'leri ve 12 aylık dağılımları listelenir, girilir, oranla dağıtılır,
    /// kaydedilir / onaylanır; Excel'e indirilir, Excel'den yüklenir; raporlar alınır. Eskiden pencerede açılan imza sayfası
    /// (ButceHazirlikv2/MasrafPlaniGenelImza.aspx) bu sayfada bir penceredir. İşlemler wwwroot/js/apgiris.js ile /api/apgiris uçlarından yapılır.
    /// </summary>
    public class APGirisModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public APGirisModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public bool Gelir { get; private set; }

        public void OnGet(string tur)
        {
            Gelir = APGirisUclari.GelirMi(tur);
            Yetkili = APGirisUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, Gelir ? "gelir" : "gider");
        }
    }
}
