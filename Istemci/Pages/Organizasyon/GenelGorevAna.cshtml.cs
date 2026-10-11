using Istemci.Organizasyon;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Organizasyon
{
    /// <summary>
    /// Genel Görev Tanımlama (eski Organizasyon/GenelGorevAna.aspx): solda görevler, sağda seçili görevin bilgileri ve
    /// yazılım yetkileri. Eski sayfada sağ taraf ayrı bir sayfaydı (GenelGorevDetay.aspx, çerçeve içinde) ve her yazılım
    /// yetkisi türü kendi sayfasını (YETxxx.aspx) pencerede açıyordu; burada hepsi tek sayfadadır, yetki pencereleri
    /// Organizasyon/YetkiTanimlari.cs içindeki alan tanımlarından çizilir.
    /// İşlemler wwwroot/js/genelgorev.js ile /api/genelgorev uçlarından yapılır. Yalnızca sistem yöneticisi kullanabilir.
    /// </summary>
    public class GenelGorevAnaModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GenelGorevUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
