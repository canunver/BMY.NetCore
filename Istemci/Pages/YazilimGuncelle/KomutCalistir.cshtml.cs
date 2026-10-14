using Istemci.Guncelleme;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.YazilimGuncelle
{
    /// <summary>
    /// Komut Çalıştır (eski YazilimGuncelle/KomutCalistir.aspx): seçilen veri tabanı bağlantısında SQL çalıştırılır; sonuç tablo olarak
    /// gösterilir ya da Excel dosyası olarak indirilir. Yalnızca sistem yöneticisi olan belirli kullanıcılar kullanır.
    /// İşlemler wwwroot/js/komutcalistir.js ile /api/guncelleme/komut uçlarından yapılır.
    /// </summary>
    public class KomutCalistirModel : PageModel
    {
        /// <summary>Eski bağlantı listesi (liste dışında bir ad da yazılabilir).</summary>
        public static readonly string[] Baglantilar =
        {
            "BaglantiSatiriATM", "BaglantiSatiriBHF", "BaglantiSatiriBHM", "BaglantiSatiriBHMv2", "BaglantiSatiriBKD", "BaglantiSatiriBUM",
            "BaglantiSatiriEGM", "BaglantiSatiriEKG", "BaglantiSatiriEVR", "BaglantiSatiriGEL", "BaglantiSatiriGRV", "BaglantiSatiriHRC",
            "BaglantiSatiriIKY", "BaglantiSatiriKHM", "BaglantiSatiriKYM", "BaglantiSatiriKYS", "BaglantiSatiriOMK", "BaglantiSatiriPB",
            "BaglantiSatiriPEB", "BaglantiSatiriPEBY", "BaglantiSatiriPER", "BaglantiSatiriPERv2", "BaglantiSatiriPYS", "BaglantiSatiriRCV",
            "BaglantiSatiriSTR", "BaglantiSatiriSTRY", "BaglantiSatiriTMM", "BaglantiSatiriYAT", "BaglantiSatiriTRM"
        };

        public bool Yetkili { get; private set; }

        public void OnGet()
        {
            Yetkili = GuncellemeOrtak.KomutYetkili(OturumBilgisi.KullaniciOku(HttpContext));
        }
    }
}
