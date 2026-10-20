using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using TNS;
using TNS.KYM;

namespace Istemci.Giris
{
    /// <summary>Şifremi Unuttum sayfasının JavaScript'ten gönderdiği istek.</summary>
    public class SifremiUnuttumIstegi
    {
        /// <summary>T.C. kimlik no ya da e-posta adresi.</summary>
        public string Bilgi { get; set; }
        public string GuvenlikKodu { get; set; }
    }

    /// <summary>
    /// Şifremi Unuttum (eski OrtakSayfa/SifremiUnuttum.aspx): T.C. kimlik no ya da e-posta adresiyle bulunan kullanıcıya
    /// iş kurallarındaki KullaniciyaYeniParolaAta ile yeni parola üretilir; yeni parola eskisi gibi iş kurallarında
    /// (ParolaAta / YeniParolaMailAt) kullanıcının kayıtlı e-posta adresine gönderilir.
    ///   POST /api/giris/sifremi-unuttum
    /// Sayfa giriş yapılmadan açıldığı için eskisine şu korumalar eklendi:
    ///   - her istekte güvenlik resmindeki kod istenir,
    ///   - kullanıcı bulunsa da bulunmasa da aynı yanıt verilir (kayıtlı kişi olup olmadığı anlaşılmaz; eskisi adı soyadı gösteriyordu),
    ///   - bir IP adresinden 15 dakikada en çok 5 istek, aynı kullanıcıya 15 dakikada bir yeni parola.
    /// LDAP kullanılıyorsa (sistem değişkeni /LDAP) eskisi gibi kullanılamaz.
    /// </summary>
    public static class SifremiUnuttumUclari
    {
        private static readonly TimeSpan Sure = TimeSpan.FromMinutes(15);
        private const int IpBasinaIstek = 5;

        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/giris/sifremi-unuttum", Gonder).AllowAnonymous();
        }

        /// <summary>LDAP kullanılıyorsa parola buradan yenilenmez (eski Giris.aspx lnkUnuttum gizleniyordu).</summary>
        public static bool Kullanilabilir()
        {
            return string.IsNullOrWhiteSpace(Ayar.Degisken(0, "/LDAP"));
        }

        private static async Task<IResult> Gonder(HttpContext baglam, SifremiUnuttumIstegi istek, IAntiforgery sahtecilik, IMemoryCache bellek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam))
                return Hatali(Metin.Menu("Giris_SayfaSuresi"));

            if (!Kullanilabilir())
                return Hatali(Metin.Menu("Sifre_Kullanilamaz"));

            string bilgi = (istek != null ? istek.Bilgi : null) ?? "";
            bilgi = bilgi.Trim();
            if (bilgi == "")
                return Hatali(Metin.Menu("Sifre_BilgiGir"));
            if (!BicimUygun(bilgi))
                return Hatali(Metin.Menu("Sifre_BilgiGecersiz"));

            if (string.IsNullOrWhiteSpace(istek.GuvenlikKodu))
                return Hatali(Metin.Menu("Giris_GuvenlikKoduGir"));
            if (!GuvenlikKodu.Dogrula(baglam, bellek, istek.GuvenlikKodu))
                return Hatali(Metin.Menu("Giris_GuvenlikKoduYanlis"));

            string ip = Ayar.IPAdresi(baglam);
            string ipAnahtari = "sifremiunuttum|ip|" + ip;
            int sayi;
            bellek.TryGetValue(ipAnahtari, out sayi);
            if (sayi >= IpBasinaIstek)
                return Hatali(string.Format(Metin.Menu("Giris_CokDeneme"), (int)Sure.TotalMinutes));
            bellek.Set(ipAnahtari, sayi + 1, Sure);

            IKYMServis servisKym = TNS.KYM.Arac.Tanimla();
            // Eski sayfa: KullaniciVarMi "Ad Soyad-TCKimlikNo" ya da boş döner.
            string uye = servisKym.KullaniciVarMi(bilgi);
            if (!string.IsNullOrEmpty(uye))
            {
                // Ad soyad içinde '-' olabilir; kimlik no son parçadır.
                string tcNo = uye.Split('-').Last().Trim();
                string kisiAnahtari = "sifremiunuttum|kisi|" + tcNo;
                object dolu;
                if (tcNo != "" && !bellek.TryGetValue(kisiAnahtari, out dolu))
                {
                    bellek.Set(kisiAnahtari, true, Sure);
                    // Yanıt süresinden kullanıcının var olup olmadığı anlaşılmasın diye e-posta gönderimi beklenmez.
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            Sonuc sonuc = servisKym.KullaniciyaYeniParolaAta(tcNo);
                            if (sonuc == null || !sonuc.islemSonuc)
                                TNS.OrtakFonksiyonlar.HataStrYaz("Şifremi Unuttum: yeni parola oluşturulamadı (" + ip + "): " + (sonuc != null ? sonuc.hataStr : ""));
                        }
                        catch (Exception ex)
                        {
                            TNS.OrtakFonksiyonlar.HataStrYaz("Şifremi Unuttum: " + ex.Message);
                        }
                    });
                }
            }

            return Results.Json(new GirisYaniti { Basarili = true, Bilgi = Metin.Menu("Sifre_Gonderildi") });
        }

        /// <summary>11 haneli T.C. kimlik no ya da e-posta adresi.</summary>
        private static bool BicimUygun(string bilgi)
        {
            if (bilgi.Length > 100) return false;
            if (bilgi.IndexOf('@') > 0)
                return bilgi.IndexOf('@') < bilgi.Length - 1 && bilgi.IndexOfAny(new[] { ' ', '\'', '"', ';', '<', '>' }) < 0;
            return bilgi.Length == 11 && bilgi.All(char.IsAsciiDigit);
        }

        private static IResult Hatali(string hata)
        {
            return Results.Json(new GirisYaniti { Basarili = false, Hata = hata });
        }
    }
}
