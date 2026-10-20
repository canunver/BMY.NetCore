using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Avans Sorgulama, eski ButceMuhasebe/AvansSorgulaForm.aspx: avans hesaplarının kişi / hesap (/ tertip) bazında borç - alacak
    /// toplamları; satır açılınca belge hareketleri görünür (js/avanssorgu.js, /api/avans/sorgu).
    /// </summary>
    public class AvansSorgulaFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public AvansSorgulaFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = AvansUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, AvansUclari.SorguSayfa);
        }
    }
}
