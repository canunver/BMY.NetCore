using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Resmi Tatil Günleri Düzenleme (eski KYM/ResmiTatilXml.aspx): her yıl aynı günde olan tatiller ile tarihi yıldan yıla değişen
    /// tatiller (dini bayramlar) tanımlanır ve resmi tatil XML dosyasına yazılır. Yalnızca sistem yöneticisi girer.
    /// Sayfa yalnızca iskeleti basar; işlemler wwwroot/js/resmitatil.js ile /api/resmitatil uçlarından yapılır.
    /// </summary>
    public class ResmiTatilXmlModel : PageModel
    {
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = ResmiTatilUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
