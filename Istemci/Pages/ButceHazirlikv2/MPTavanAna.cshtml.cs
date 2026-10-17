using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Masraf Planı Tavan (eski ButceHazirlikv2/MPTavanAna.aspx, "Gider Ödenekleri Düzenleme"): seçilen kodlara uyan tertiplerin KBÖ,
    /// önceki yıl gönderilen / harcanan, birim talebi, gönderilen, MP tavan ve detay toplamları ve dağıtılmayan tutarları listelenir.
    /// Tavan / detay tutarına tıklanınca tavan girişi (MPTavanGiris) pencerede açılır; il izni açılıp kapatılır; tavan ve detay
    /// onaylanır; hata raporları, tavan raporu ve hızlı ödenek dosyaları alınır; Excel'den ve merkez birimlerinden tavan yüklenir.
    /// İşlemler wwwroot/js/mptavan.js ile /api/mptavan uçlarından yapılır.
    /// </summary>
    public class MPTavanAnaModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MPTavanAnaModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPTavanUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
