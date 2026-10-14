using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Tertip tanımları (eski ButceHazirlikv2/TertipTanimFormu.aspx; bütçe teklifi ekranındaki "Tertipler" tuşu, ?yil=..&amp;Tur=1 gider /
    /// 2 gelir): soldaki kodlara uyan tanımlı tertipler listelenir, kod ve adları düzenlenip kaydedilir; ad / tertip üretme, Excel'e
    /// indirme ve Excel'den yükleme. İşlemler wwwroot/js/tertiptanim.js ile /api/tertiptanim uçlarından yapılır.
    /// </summary>
    public class TertipTanimFormuModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TertipTanimFormuModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int Tur { get; private set; }
        public int Yil { get; private set; }

        public void OnGet(int Tur = 1, int yil = 0)
        {
            this.Tur = Tur == 2 ? 2 : 1;
            Yil = yil >= 2000 ? yil : 0;
            Yetkili = TertipTanimUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, this.Tur);
        }
    }
}
