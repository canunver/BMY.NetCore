using Istemci.ButceHazirlik;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.ButceHazirlikv2
{
    /// <summary>
    /// Bütçe teklifi tertip detayı (eski ButceHazirlikv2/ButceTeklifHazirlaDetay.aspx; bütçe teklifi ekranında satırdaki detay simgesi,
    /// ?senaryo=..&amp;yil=..&amp;tahmin=..&amp;tertip=..): tertibin kesin hesap, taahhüt (harcama süreci) ve taşınır bilgileri ile
    /// 3 yıllık teklif girişi. İşlemler wwwroot/js/tertipdetay.js ile /api/tertipdetay uçlarından yapılır.
    /// </summary>
    public class ButceTeklifHazirlaDetayModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public ButceTeklifHazirlaDetayModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }
        public int Yil { get; private set; }
        public int Tahmin { get; private set; }
        public string Senaryo { get; private set; }
        public string Tertip { get; private set; }
        public string MuhasebeKod { get; private set; }
        public string BirimKod { get; private set; }

        public void OnGet(int yil = 0, int tahmin = 0, string senaryo = "", string tertip = "", string muhasebekod = "", string birimkod = "")
        {
            Yil = yil;
            Tahmin = tahmin;
            Senaryo = (senaryo ?? "").Trim();
            Tertip = (tertip ?? "").Trim();
            MuhasebeKod = (muhasebekod ?? "").Trim();
            BirimKod = (birimkod ?? "").Trim();
            Yetkili = TertipDetayUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, tahmin);
        }
    }
}
