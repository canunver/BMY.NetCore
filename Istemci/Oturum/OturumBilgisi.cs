using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using TNS;
using TNS.Api;
using TNS.KYM;

namespace Istemci.Oturum
{
    /// <summary>Oturum boyunca tutulan bilgiler (eski Session["kullanan"], Session["YoneticiMi"] ...).</summary>
    public class OturumVerisi
    {
        public Kullanici Kullanan;
        public bool YoneticiMi;
        public bool KullaniciYoneticisiMi;
    }

    /// <summary>
    /// Giriş yapan kullanıcının oturum bilgisi (eski OrtakClass.OturumBilgisiIslem).
    ///
    /// Tarayıcıda yalnızca şifreli bir çerez (oturum numarası) durur. Oturum verisi kullanıcı veri tabanındaki
    /// KYMOTURUM tablosunda saklanır (eski ASP.NET State Server'ın yerine); bu sayede uygulama yeniden başlasa da
    /// kullanıcılar oturumda kalır. Her istekte veri tabanına gitmemek için veri bellekte kısa süre tutulur.
    /// </summary>
    public static class OturumBilgisi
    {
        /// <summary>İşlem yapılmazsa oturumun düşeceği süre. appSettings: OturumSuresiDakika (varsayılan 40).</summary>
        public static readonly TimeSpan Sure = TimeSpan.FromMinutes(
            Math.Max(1, OrtakFonksiyonlar.ConvertToInt(Ayar.Oku("OturumSuresiDakika", "40"), 40)));

        /// <summary>Oturum verisinin bellekte tutulma süresi; veri tabanındaki süre en geç bu aralıkla uzatılır.</summary>
        private static readonly TimeSpan BellekSuresi = TimeSpan.FromMinutes(1);

        private const string OturumNoTipi = "sgb:oturum";

        private static string Anahtar(string oturumNo)
        {
            return "oturum|" + oturumNo;
        }

        private static IMemoryCache Bellek(HttpContext baglam)
        {
            return baglam.RequestServices.GetRequiredService<IMemoryCache>();
        }

        private static string OturumNo(HttpContext baglam, ClaimsPrincipal kimlik)
        {
            kimlik = kimlik ?? baglam.User;
            Claim no = kimlik != null ? kimlik.FindFirst(OturumNoTipi) : null;
            return no != null ? no.Value : null;
        }

        private static void Sakla(HttpContext baglam, string oturumNo, OturumVerisi veri)
        {
            string json = ApiJson.Yaz(veri, typeof(OturumVerisi));
            Sonuc sonuc = TNS.KYM.Arac.Tanimla().OturumYaz(oturumNo, veri.Kullanan.kullaniciKodu, json, (int)Sure.TotalMinutes);
            if (!sonuc.islemSonuc)
                throw new TNSIslemHatasi("Oturum kaydedilemedi: " + sonuc.hataStr);

            Bellek(baglam).Set(Anahtar(oturumNo), veri, BellekSuresi);
        }

        /// <summary>Kullanıcıyı oturuma yazar ve giriş çerezini verir (eski KullaniciBilgiYaz).</summary>
        public static async Task Baslat(HttpContext baglam, Kullanici kullanan)
        {
            OturumVerisi veri = new OturumVerisi();
            veri.Kullanan = kullanan;
            veri.YoneticiMi = Yetki.YoneticiMi(kullanan).islemSonuc;
            veri.KullaniciYoneticisiMi = Yetki.SistemveyaKullaniciYoneticisiMi(kullanan);

            string oturumNo = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            Sakla(baglam, oturumNo, veri);

            List<Claim> bilgiler = new List<Claim>();
            bilgiler.Add(new Claim(ClaimTypes.NameIdentifier, kullanan.kullaniciKodu ?? ""));
            bilgiler.Add(new Claim(ClaimTypes.Name, kullanan.ADSOYAD ?? ""));
            bilgiler.Add(new Claim(OturumNoTipi, oturumNo));
            ClaimsPrincipal kimlik = new ClaimsPrincipal(new ClaimsIdentity(bilgiler, CookieAuthenticationDefaults.AuthenticationScheme));

            await baglam.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, kimlik);

            // Eski KullaniciVeriTabaninaYaz: giriş yapan kullanıcıyı çevrimiçi kullanıcılar tablosuna işler.
            TNS.KYM.Arac.Tanimla().KullaniciSessionAta(kullanan.kullaniciKodu, new TNSDateTime(DateTime.Now), Ayar.IPAdresi(baglam),
                baglam.Request.Path.ToString(), kullanan.ad + " " + kullanan.soyad, "", "", "", "");
        }

        /// <summary>Oturum verisini döndürür; oturum yoksa ya da süresi dolmuşsa null.</summary>
        public static OturumVerisi Oku(HttpContext baglam, ClaimsPrincipal kimlik = null)
        {
            string oturumNo = OturumNo(baglam, kimlik);
            if (oturumNo == null) return null;

            IMemoryCache bellek = Bellek(baglam);
            OturumVerisi veri;
            if (bellek.TryGetValue(Anahtar(oturumNo), out veri)) return veri;

            // Bellekte yok (süresi doldu ya da uygulama yeniden başladı): veri tabanından okunur, süresi uzatılır.
            string json = TNS.KYM.Arac.Tanimla().OturumOku(oturumNo, (int)Sure.TotalMinutes);
            if (string.IsNullOrEmpty(json)) return null;

            try
            {
                veri = (OturumVerisi)ApiJson.Oku(json, typeof(OturumVerisi));
            }
            catch (Exception)
            {
                // Kayıt okunamıyorsa (ör. güncellemeyle sınıf yapısı değişti) kullanıcı yeniden giriş yapar.
                return null;
            }
            if (veri == null || veri.Kullanan == null) return null;

            bellek.Set(Anahtar(oturumNo), veri, BellekSuresi);
            return veri;
        }

        /// <summary>Giriş yapmış kullanıcı; oturum yoksa null (eski KullaniciBilgiOku).</summary>
        public static Kullanici KullaniciOku(HttpContext baglam, ClaimsPrincipal kimlik = null)
        {
            OturumVerisi veri = Oku(baglam, kimlik);
            return veri != null ? veri.Kullanan : null;
        }

        /// <summary>Oturumdaki bilgi değiştirildiyse (ör. kullanıcının aktif kurumu) değişikliği kalıcı hale getirir.</summary>
        public static void Guncelle(HttpContext baglam, OturumVerisi veri)
        {
            string oturumNo = OturumNo(baglam, null);
            if (oturumNo != null && veri != null && veri.Kullanan != null)
                Sakla(baglam, oturumNo, veri);
        }

        public static async Task Bitir(HttpContext baglam)
        {
            string oturumNo = OturumNo(baglam, null);
            if (oturumNo != null)
            {
                Bellek(baglam).Remove(Anahtar(oturumNo));
                TNS.KYM.Arac.Tanimla().OturumSil(oturumNo);
            }
            await baglam.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
