using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödeme Emri / MİF belgesi (eski ButceMuhasebe/OdemeEmriMIF2.aspx): Ödeme Emri / MİF çerçevesinin "Belge Düzenleme" sekmesi.
    /// Adres: OdemeEmriMIF2?yil=&amp;belgeNo=&amp;aramaYok=&amp;baslikSayfasi= (baslikSayfasi=1: yalnız başlık bilgileri, tablo gizli).
    /// Sayfa yalnızca iskeleti basar; işlemler wwwroot/js/odemeemrimif.js ile /api/odemeemrimif uçlarından (Muhasebe/OdemeEmriMIFUclari.cs)
    /// yapılır. Seçim pencereleri js/odemeemrisecim.js (Sgb.oem). Yetki: çerçevenin menü adreslerinden.
    /// </summary>
    public class OdemeEmriMIF2Model : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public OdemeEmriMIF2Model(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = OdemeEmriMIFUclari.Yetkili(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
