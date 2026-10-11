using System.Collections.Generic;
using System.Text.Json;
using Istemci.Kullanicilar;
using Istemci.Oturum;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Istemci.Pages.KYM
{
    /// <summary>
    /// Kullanıcı Giriş (eski KYM/KullaniciGiris.aspx ve onun açtığı KullaniciYetkiGiris, YetkiTipiDegerSec, YetkiTipiAgactanSec):
    /// yeni kullanıcı oluşturma ya da ?kullaniciKodu=... ile var olanı düzenleme; kullanıcı tipleri ve yetki değerleri,
    /// yeni parola atama, aktif/pasif yapma. Sayfa yalnızca iskeleti ve metinleri basar; işlemler
    /// wwwroot/js/kullanicigiris.js ile /api/kullanici uçlarından yapılır.
    /// Yalnızca sistem yöneticisi, kullanıcı yöneticisi ve destek yöneticisi kullanabilir.
    /// </summary>
    public class KullaniciGirisModel : PageModel
    {
        public bool Yetkili { get; private set; }

        /// <summary>Kullanıcıyı görevlere atama penceresi (Organizasyon/GorevAta) açılabilir mi? Yalnızca sistem yöneticisi.</summary>
        public bool GorevAtayabilir { get; private set; }

        /// <summary>JavaScript'in kullandığı metinler (JSON).</summary>
        public string MetinlerJson { get; private set; }

        public void OnGet(string kullaniciKodu)
        {
            Yetkili = KullaniciUclari.GirisYetkisiVar(OturumBilgisi.KullaniciOku(HttpContext));
            // Kullanıcı entegrasyonu açıkken yeni kullanıcı buradan oluşturulmaz (eski sayfadaki gibi).
            if (Yetkili && KullaniciUclari.EntegrasyonVar() && string.IsNullOrWhiteSpace(kullaniciKodu)) Yetkili = false;
            if (!Yetkili) return;
            GorevAtayabilir = Istemci.Organizasyon.GenelGorevUclari.YetkiVar(OturumBilgisi.KullaniciOku(HttpContext));

            Dictionary<string, string> metinler = new Dictionary<string, string>();
            foreach (string anahtar in new[] { "IslemHata", "FRMKUG024", "FRMKUG025", "FRMKUG053", "FRMKUG055", "FRMKUG056", "FRMKUG057", "FRMKUG027",
                                               "FRMKYG010", "TipSec", "TipZatenVar", "TipYok", "Duzenle", "FRMKUG012", "Sec", "Kaldir", "SecilenYok",
                                               "TekDeger", "FRMYTD008", "OneriBaslik", "DegerSecBaslik" })
                metinler[anahtar] = Metin.Kullanici(anahtar);
            MetinlerJson = JsonSerializer.Serialize(metinler);
        }
    }
}
