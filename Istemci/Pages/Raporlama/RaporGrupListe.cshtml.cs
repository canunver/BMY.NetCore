using Istemci.Oturum;
using Istemci.Raporlama;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.Raporlama
{
    /// <summary>
    /// Genel rapor ekranı (eski Raporlama/RaporGrupListe.aspx, içinde RaporKistasSayfa.aspx ve RaporDosya.aspx): adresteki
    /// ?raporGrup= grubunun raporları Rapor.xml'den solda listelenir; seçilen raporun kıstas alanları dinamik çizilir, Rapor Al ile
    /// rapor üretilip indirilir ve saklanır. Saklanan raporlar (son 3) kütüphaneye eklenerek kalıcı yapılabilir.
    /// Menüde çok sayıda seçenek bu sayfayı farklı gruplarla açar. İşlemler wwwroot/js/raporgrup.js ile /api/raporgrup uçlarından yapılır.
    /// </summary>
    public class RaporGrupListeModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public RaporGrupListeModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public string RaporGrup { get; private set; }
        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            RaporGrup = (Request.Query["raporGrup"].ToString() ?? "").Trim();
            Yetkili = RaporGrupUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, RaporGrup);
        }
    }
}
