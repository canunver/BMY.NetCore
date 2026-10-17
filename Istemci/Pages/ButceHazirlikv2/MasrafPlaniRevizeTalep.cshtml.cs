using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Masraf Planı Revize Talep (eski ButceHazirlikv2/MasrafPlaniRevizeTalep.aspx): seçilen saymanlık - birimin ve kodların
    /// tertiplerinde gönderilen, harcanan ödenek ve masraf planının aylık dağılımı; Liste Yazdır, KBS-SGB Fark, Kopyala, Merkeze Al
    /// ve tertibin aylık harcamaları. İşlemler wwwroot/js/mprevizetalep.js ile /api/mprevizetalep uçlarından yapılır.
    /// </summary>
    public class MasrafPlaniRevizeTalepModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public MasrafPlaniRevizeTalepModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = MPRevizeTalepUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
