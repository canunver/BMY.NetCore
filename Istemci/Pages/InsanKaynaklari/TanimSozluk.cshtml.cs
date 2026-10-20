using Istemci.InsanKaynaklari;
using Istemci.Oturum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.InsanKaynaklari
{
    /// <summary>
    /// Sözlük Bilgileri (eski InsanKaynaklari/TanimSozluk.aspx): solda sözlük kayıtları, sağda seçilenin bilgileri.
    /// anaKayitKod verilmezse tüm sözlük, verilirse yalnızca o bölüm (menüde Askerlik, Öğrenim, Üniversite, Dil ... seçenekleri).
    /// İşlemler wwwroot/js/sozluk.js ile /api/sozluk uçlarından yapılır.
    /// </summary>
    public class TanimSozlukModel : PageModel
    {
        private readonly IWebHostEnvironment ortam;

        public TanimSozlukModel(IWebHostEnvironment ortam)
        {
            this.ortam = ortam;
        }

        public bool Yetkili { get; private set; }

        /// <summary>Menüden gelen ana kayıt kodu (ör. 07) ya da boş.</summary>
        public string AnaKayitKod { get; private set; }

        public void OnGet(string anaKayitKod)
        {
            AnaKayitKod = (anaKayitKod ?? "").Trim();
            Yetkili = SozlukUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext), ortam.ContentRootPath, AnaKayitKod);
        }
    }
}
