using System;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>Parola Değiştir sayfasının JavaScript'ten gönderdiği istek.</summary>
    public class ParolaDegistirIstegi
    {
        public string parola { get; set; }
        public string yeniParola { get; set; }
        public string yeniParolaTekrar { get; set; }
    }

    /// <summary>
    /// Parola Değiştir sayfasının (eski KYM/ParolaDegistir.aspx) ucu:
    ///   POST /api/paroladegistir/degistir : oturumdaki kullanıcının parolasını değiştirir (IKYMServis.ParolaDegistir).
    ///   Başarılıysa oturum kapatılır ve kullanıcı yeni parolasıyla yeniden girer (eskisi Cikis.aspx'e yönlendiriyordu).
    /// Sayfa sağ üstteki kullanıcı menüsünden açılır; menu.xml'de yoktur.
    /// </summary>
    public static class ParolaDegistirUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/paroladegistir/degistir", Degistir);
        }

        /// <summary>
        /// Parola bu ekrandan değiştirilebilir mi? Eski sayfa appSettings SadeceLDAP ya da Entegrasyon açıksa açılmıyordu
        /// (GenelIslemler.SayfayaGirmesin).
        /// </summary>
        public static bool Kullanilabilir()
        {
            bool sadeceLDAP = OrtakFonksiyonlar.ConvertToInt(Ayar.Oku("SadeceLDAP", ""), 0) > 0;
            bool entegrasyon = OrtakFonksiyonlar.ConvertToInt(Ayar.Oku("Entegrasyon", "0"), 0) > 0;
            return !sadeceLDAP && !entegrasyon;
        }

        /// <summary>
        /// Parola süresi dolmuş mu (eski GenelIslemler.ParolaSuresiKontrol; eski sayfa bu durumda FRMPAD008 uyarısını gösteriyordu).
        /// LDAP kullanılıyorsa kontrol yapılmaz.
        /// </summary>
        public static bool SuresiDoldu(Kullanici kullanan)
        {
            if (kullanan == null || string.IsNullOrEmpty(kullanan.kullaniciKodu)) return false;
            if (!string.IsNullOrWhiteSpace(Ayar.Degisken(0, "LDAP"))) return false;

            DateTime bitis;
            if (kullanan.parolaBitisTarihi == null || kullanan.parolaBitisTarihi.isNull)
                bitis = DateTime.Today.AddDays(-1);
            else
                bitis = kullanan.parolaBitisTarihi.Oku().AddDays(-1);
            return DateTime.Today > bitis;
        }

        private static async Task<IResult> Degistir(HttpContext baglam, ParolaDegistirIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Kullanilabilir()) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null) return Results.BadRequest();

            string parola = istek.parola ?? "";
            string yeni = istek.yeniParola ?? "";
            string tekrar = istek.yeniParolaTekrar ?? "";

            // Eski btnParolaDegistir_Click'teki denetimler (aynı sırayla: tekrar farklıysa o hata yazılır).
            if (yeni != tekrar || yeni == "")
            {
                string hata = "";
                if (yeni == "") hata = Metin.Kullanici("FRMPAD011");
                if (yeni != tekrar) hata = Metin.Kullanici("FRMPAD012");
                return Hata(hata);
            }

            Sonuc sonuc;
            try
            {
                sonuc = TNS.KYM.Arac.Tanimla().ParolaDegistir(kullanan, parola, yeni, tekrar);
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
            if (sonuc == null || !sonuc.islemSonuc)
                return Hata(HataMetni(sonuc != null ? sonuc.hataStr : null));

            await OturumBilgisi.Bitir(baglam);
            return Results.Json(new
            {
                basarili = true,
                bilgi = "Parolanız değiştirildi. Lütfen yeni parolanızla yeniden giriş yapınız.",
                yonlen = baglam.Request.PathBase + "/Giris"
            });
        }

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        /// <summary>İş kuralının hata metni; boşsa ya da istisna dökümüyse genel metin. HTML etiketleri düz metne çevrilir.</summary>
        private static string HataMetni(string hata)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return "Parola değiştirilemedi.";
            hata = System.Text.RegularExpressions.Regex.Replace(hata, @"<br\s*/?>", "\n", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            hata = System.Text.RegularExpressions.Regex.Replace(hata, @"<[^>]+>", "");
            return System.Net.WebUtility.HtmlDecode(hata).Trim();
        }
    }
}
