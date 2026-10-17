using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// İl Özel İdaresi Projeleri (eski ButceHazirlikv2/IOIProjeListeForm.aspx): İÖİ / YİKOB projeleri kriterlere göre listelenir;
    /// proje bilgisi, tahsis ve harcama tutarları pencerelerde girilir; birimin proje girişi onaylanır; listeler Excel'e alınır.
    /// Eski IOIProjeGirisForm.aspx'in işi bu sayfanın proje tanım penceresindedir.
    /// İşlemler wwwroot/js/ioiproje.js ile /api/ioiproje uçlarından yapılır.
    /// </summary>
    public class IOIProjeListeFormModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public IOIProjeListeFormModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = IOIProjeUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath);
        }
    }
}
