using Istemci.Muhasebe;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceMuhasebe
{
    /// <summary>
    /// Ödeme Emri / MİF belge arama (eski ButceMuhasebe/OdemeEmriMIFSorgu.aspx): Ödeme Emri / MİF çerçevesinin "Belge Arama" sekmesi;
    /// çerçevede OdemeEmriMIFSorgu?menuYok=1 olarak açılır. Sorgu alanları, belge listesi (satır ayrıntısı, tarihçe), Liste Yazdır,
    /// Liste İşlemleri (onayla, onay kaldır, belgeleri yazdır, iptal), Yevmiye / Çek / İstatistik işlemleri. Sayfa yalnızca iskeleti
    /// basar; işlemler wwwroot/js/odemeemrimifliste.js ile /api/odemeemrimif/liste... uçlarından (Muhasebe/OdemeEmriMIFSorguUclari.cs)
    /// yapılır. Yetki üst sayfadan (Ödeme Emri / MİF çerçevesi).
    /// </summary>
    public class OdemeEmriMIFSorguModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public OdemeEmriMIFSorguModel(IWebHostEnvironment ortam)
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
