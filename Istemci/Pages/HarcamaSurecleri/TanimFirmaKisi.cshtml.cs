using Istemci.HarcamaSurecleri;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.Pages.HarcamaSurecleri
{
    /// <summary>
    /// Firma / Kişi Tanımlama (eski HarcamaSurecleri/TanimFirmaKisi.aspx). Menüde Harcama Yönetimi, Bütçe Muhasebe ve Tanımlar &gt; Genel
    /// Tanımlar altındadır; Firma Kişi Listesi'nin Yeni tuşuyla pencerede de açılır (?muhasebe=&amp;yeniFirma=).
    /// İşlemler js/firmatanim.js ile /api/firmatanim uçlarından (HarcamaSurecleri/FirmaTanimUclari.cs) yapılır.
    /// </summary>
    public class TanimFirmaKisiModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimFirmaKisiModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public string Muhasebe { get; private set; } = "";
        public string MuhasebeAd { get; private set; } = "";
        public bool Gelirler { get; private set; }

        public void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Yetkili = FirmaTanimUclari.YetkiVar(kullanan, ortam.ContentRootPath);
            if (!Yetkili) return;

            Gelirler = FirmaTanimUclari.GelirlerBaglantisiVar();
            Muhasebe = FirmaTanimUclari.VarsayilanMuhasebe(kullanan, Request.Query["muhasebe"].ToString());
            int m = TNS.OrtakFonksiyonlar.ConvertToInt(Muhasebe, 0);
            if (m > 0)
            {
                try { MuhasebeAd = MuhasebeBirimUclari.BirimAdBul(m, "", out string _); } catch (System.Exception) { MuhasebeAd = ""; }
            }
        }
    }
}
