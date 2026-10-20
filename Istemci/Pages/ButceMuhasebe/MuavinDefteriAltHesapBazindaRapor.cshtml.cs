using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Alt hesap bazında muavin defteri (eski ButceMuhasebe/MuavinDefteriAltHesapBazindaRapor.aspx?hesapKod=&amp;altHesapKod=&amp;yil=&amp;projeNo=).
    /// Menüde yoktur: Alacak Takip "Takip İşlemleri" (alacaktakip.js, Muavin Defteri tuşu) bu adresi pencere olarak açar; Ödeme Emri / MİF
    /// formu aynı listeyi Sgb.oem.altHesapMuavin penceresiyle gösterir. Yetki üst sayfalardan (OdemeEmriSecimUclari.MuavinYetkiVar).
    /// Liste wwwroot/js/odemeemrisecim.js ile /api/oemsecim/muavin ucundan okunur.
    /// </summary>
    public class MuavinDefteriAltHesapBazindaRaporModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MuavinDefteriAltHesapBazindaRaporModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = OdemeEmriSecimUclari.MuavinYetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
