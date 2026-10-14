using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe teklifi / yıl sonu tahmini girişi (eski ButceHazirlikv2/ButceRakamDuzenleYeni.aspx ve içindeki ButceRakamDuzenleExt.aspx
    /// tablosu). Menüdeki seçeneğe göre çalışır: odenekTur 1 gider, 3 gider (eko 2), 4 gider (eko 4), 2 gelir; tahmin 0 teklif,
    /// 1 yıl sonu tahmini, 2 tavan üstü. Soldaki kod seçimine göre tertipler listelenir, tutarlar girilip kaydedilir; katsayı, ek
    /// sütunlar, raporlar ve Excel işlemleri wwwroot/js/butcerakam.js ile /api/butcerakam uçlarından yapılır.
    /// </summary>
    public class ButceRakamDuzenleYeniModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ButceRakamDuzenleYeniModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int OdenekTur { get; private set; }
        public int Tahmin { get; private set; }

        public void OnGet(int odenekTur = 1, int tahmin = 0)
        {
            OdenekTur = odenekTur == 2 || odenekTur == 3 || odenekTur == 4 ? odenekTur : 1;
            Tahmin = tahmin == 1 || tahmin == 2 ? tahmin : 0;
            Yetkili = ButceRakamUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, OdenekTur, Tahmin);
        }
    }
}
