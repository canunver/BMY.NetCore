using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Sayfa yardımı (eski KYM/YardimSayfa.aspx?sayfaKod=Klasor/Sayfa). Her sayfanın üst şeridindeki Yardım düğmesi bunu pencerede açar.
    /// Yazabilen kullanıcı (bkz. YardimUclari.Yazabilir) HTML editörü, Kaydet, Düzenle ve Dosyalar'ı görür; diğerleri metni salt okunur görür.
    /// Sayfa yalnızca iskeleti basar; metin wwwroot/js/yardimsayfa.js ile /api/yardim/* uçlarından okunur ve kaydedilir.
    /// </summary>
    public class YardimSayfaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public YardimSayfaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public string SayfaKod { get; private set; }

        /// <summary>Editör ve dosya bileşeninin betikleri yalnızca yazabilene yüklenir; kaydetme ucu ayrıca denetler.</summary>
        public bool Yazabilir { get; private set; }

        public void OnGet(string sayfaKod)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            SayfaKod = YardimUclari.SayfaKodDuzelt(sayfaKod);
            Yazabilir = SayfaKod != "" && YardimUclari.Yazabilir(kullanan, ortam.ContentRootPath, SayfaKod);
        }
    }
}
