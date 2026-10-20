using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TNS.KYM;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Parola Değiştir (eski KYM/ParolaDegistir.aspx): oturumdaki kullanıcı kendi parolasını değiştirir.
    /// Sağ üstteki kullanıcı menüsünden açılır. Sayfa yalnızca iskeleti basar; değiştirme wwwroot/js/paroladegistir.js ile
    /// /api/paroladegistir/degistir ucundan yapılır.
    /// </summary>
    public class ParolaDegistirModel : PageModel
    {
        /// <summary>SadeceLDAP ya da Entegrasyon açıksa parola buradan değiştirilemez (eski SayfayaGirmesin).</summary>
        public bool Kullanilabilir { get; private set; }

        /// <summary>Parola süresi dolmuşsa uyarı gösterilir (eski FRMPAD008).</summary>
        public bool SuresiDoldu { get; private set; }

        public string KullaniciKodu { get; private set; }

        public void OnGet()
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(HttpContext);
            Kullanilabilir = ParolaDegistirUclari.Kullanilabilir();
            SuresiDoldu = ParolaDegistirUclari.SuresiDoldu(kullanan);
            KullaniciKodu = kullanan != null ? kullanan.kullaniciKodu : "";
        }
    }
}
