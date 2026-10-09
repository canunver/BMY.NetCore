using System;
using System.Collections.Generic;
using System.Globalization;
using Istemci.Oturum;
using Microsoft.AspNetCore.Http;
using TNS.KYM;

namespace Istemci
{
    /// <summary>
    /// İsteğin dili (eski GenelSayfa.InitializeCulture ve GenelGiris.DilDegistir).
    ///   - Giriş yapmış kullanıcıda dil, oturumdaki kullanıcının aktifKultur alanıdır.
    ///   - Giriş yapılmamışsa KulDil çerezine bakılır; o da yoksa uygulamanın varsayılanı (tr-TR) kalır.
    /// </summary>
    public static class Dil
    {
        public const string Cerez = "KulDil";

        /// <summary>Seçilebilecek diller (TNS.BKD.Arac.Diller).</summary>
        public static List<string> Desteklenen()
        {
            List<string> diller = new List<string>();
            for (int i = 0; i < TNS.BKD.Arac.DillerLength; i++)
            {
                string dil = TNS.BKD.Arac.Diller(i);
                if (!string.IsNullOrEmpty(dil) && !diller.Contains(dil)) diller.Add(dil);
            }
            if (diller.Count == 0) diller.Add("tr");
            return diller;
        }

        public static bool Gecerli(string dil)
        {
            return !string.IsNullOrEmpty(dil) && Desteklenen().Contains(dil);
        }

        /// <summary>İstekte geçerli olan dilin iki harfli kodu.</summary>
        public static string Aktif
        {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
        }

        /// <summary>Dilin kendi dilindeki adı: Türkçe, English ...</summary>
        public static string Ad(string dil)
        {
            try
            {
                CultureInfo k = new CultureInfo(dil);
                string ad = k.NativeName;
                return ad.Length > 0 ? char.ToUpper(ad[0], k) + ad.Substring(1) : dil;
            }
            catch (CultureNotFoundException)
            {
                return dil;
            }
        }

        /// <summary>Her istekte, kimlik doğrulamadan sonra çağrılır.</summary>
        public static void Uygula(HttpContext baglam)
        {
            string dil = null;
            bool girisYapmis = false;

            if (baglam.User != null && baglam.User.Identity != null && baglam.User.Identity.IsAuthenticated)
            {
                Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
                if (kullanan != null && !string.IsNullOrEmpty(kullanan.aktifKultur))
                {
                    dil = kullanan.aktifKultur;
                    girisYapmis = true;
                }
            }

            if (dil == null)
            {
                string cerez = baglam.Request.Cookies[Cerez];
                if (Gecerli(cerez)) dil = cerez;
            }

            if (dil == null) return;

            try
            {
                CultureInfo.CurrentUICulture = new CultureInfo(dil);
                // Eski sistemdeki gibi, giriş yapmış kullanıcıda sayı ve tarih biçimi de seçilen dile göre olur.
                if (girisYapmis) CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(dil);
            }
            catch (CultureNotFoundException)
            {
                // Tanınmayan dil kodu: varsayılan dil kalır.
            }
        }

        /// <summary>Dil seçimini çereze (bir yıl) yazar; giriş sayfası ve sonraki girişler bu dille açılır.</summary>
        public static void CerezYaz(HttpContext baglam, string dil)
        {
            CookieOptions secenek = new CookieOptions();
            secenek.Expires = DateTimeOffset.Now.AddYears(1);
            secenek.HttpOnly = true;
            secenek.Secure = baglam.Request.IsHttps;
            secenek.SameSite = SameSiteMode.Lax;
            baglam.Response.Cookies.Append(Cerez, dil, secenek);
        }
    }
}
