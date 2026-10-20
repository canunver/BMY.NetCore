using System.Linq;
using Istemci.HarcamaSurecleri;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Firma / Kişi Listesi (eski HarcamaSurecleri/ListeFirmaKisi.aspx). Menüde Harcama Yönetimi ve Bütçe Muhasebe altındadır; başka
    /// ekranlardan pencere olarak da açılır. Adres parametreleri eski sayfadaki gibidir: muhasebe, per (personelde de ara),
    /// yeni (açılışta Firma Tanımlama bu numarayla açılır), seckapat (seçince pencere kapanmasın), firmaSayisi / surecNo / yil
    /// (ihalede kazanan firmalardan seçim). İşlemler js/firmakisiliste.js ile /api/firmakisi uçlarından yapılır.
    /// </summary>
    public class ListeFirmaKisiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;
        private readonly IActionDescriptorCollectionProvider sayfalar;

        public ListeFirmaKisiModel(IWebHostEnvironment ortam, IActionDescriptorCollectionProvider sayfalar)
        {
            this.ortam = ortam;
            this.sayfalar = sayfalar;
        }

        public bool Yetkili { get; private set; }
        public int Muhasebe { get; private set; }
        public bool BirlestirGoster { get; private set; }

        /// <summary>Firma Tanımlama (TanimFirmaKisi) Core'a taşındı mı? Taşınmadıysa Yeni düğmesi uyarı verir.</summary>
        public bool TanimVar { get; private set; }

        public void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = FirmaKisiListeUclari.YetkiVar(kullanan, ortam.ContentRootPath);
            if (!Yetkili) return;

            string m = (Request.Query["muhasebe"].ToString() ?? "").Trim();
            Muhasebe = m != "" ? TNS.OrtakFonksiyonlar.ConvertToInt(m, 0) : FirmaKisiListeUclari.VarsayilanMuhasebe(kullanan);
            BirlestirGoster = FirmaKisiListeUclari.BirlestirGoster();
            TanimVar = sayfalar.ActionDescriptors.Items.OfType<PageActionDescriptor>()
                .Any(d => string.Equals(d.ViewEnginePath, "/HarcamaSurecleri/TanimFirmaKisi", System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
