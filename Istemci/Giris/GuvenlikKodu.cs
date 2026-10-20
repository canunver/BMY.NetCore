using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Istemci.Giris
{
    /// <summary>
    /// Güvenlik resmi (eski OrtakSayfa/GuvenlikResmi.aspx): 4 karakterlik kod resim olarak çizilir.
    /// Kod sunucuda bellekte, tarayıcıya verilen rastgele bir çerez kimliğiyle saklanır (eski oturum değişkeni "guvenlikKodu").
    /// Eskisinden farkı: kod bir kez kullanılır; doğru ya da yanlış her denemeden sonra yeni resim gerekir,
    /// böylece bir kez okunan kodla art arda deneme yapılamaz.
    ///   GET /api/giris/guvenlik-resmi : yeni kod üretir, PNG döner.
    /// </summary>
    public static class GuvenlikKodu
    {
        private const string CerezAdi = "SgbGuvenlik";
        private const string Karakterler = "123456789QWERTYUIPASDFGHJKLZXCVBNM";
        private const int Uzunluk = 4;
        private static readonly TimeSpan Sure = TimeSpan.FromMinutes(10);

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/giris/guvenlik-resmi", Resim).AllowAnonymous();
        }

        private static IResult Resim(HttpContext baglam, IMemoryCache bellek)
        {
            string kimlik = baglam.Request.Cookies[CerezAdi];
            if (!KimlikGecerli(kimlik))
            {
                kimlik = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
                CookieOptions secenek = new CookieOptions();
                secenek.HttpOnly = true;
                secenek.Secure = baglam.Request.IsHttps;
                secenek.SameSite = SameSiteMode.Strict;
                secenek.Path = baglam.Request.PathBase.HasValue ? baglam.Request.PathBase.Value : "/";
                baglam.Response.Cookies.Append(CerezAdi, kimlik, secenek);
            }

            StringBuilder kod = new StringBuilder(Uzunluk);
            for (int i = 0; i < Uzunluk; i++)
                kod.Append(Karakterler[RandomNumberGenerator.GetInt32(Karakterler.Length)]);
            bellek.Set(Anahtar(kimlik), kod.ToString(), Sure);

            baglam.Response.Headers.CacheControl = "no-store, no-cache";
            return Results.File(Ciz(kod.ToString()), "image/png");
        }

        /// <summary>Girilen kod son üretilen resimdeki kodla aynı mı? Kod her durumda silinir (bir kez kullanılır).</summary>
        public static bool Dogrula(HttpContext baglam, IMemoryCache bellek, string girilen)
        {
            string kimlik = baglam.Request.Cookies[CerezAdi];
            if (!KimlikGecerli(kimlik)) return false;

            string anahtar = Anahtar(kimlik);
            string kod;
            if (!bellek.TryGetValue(anahtar, out kod) || string.IsNullOrEmpty(kod)) return false;
            bellek.Remove(anahtar);

            girilen = (girilen ?? "").Trim().ToUpperInvariant();
            return girilen.Length == kod.Length
                && CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(girilen), Encoding.ASCII.GetBytes(kod));
        }

        private static string Anahtar(string kimlik)
        {
            return "guvenlikkodu|" + kimlik;
        }

        private static bool KimlikGecerli(string kimlik)
        {
            if (string.IsNullOrEmpty(kimlik) || kimlik.Length != 32) return false;
            foreach (char c in kimlik)
                if (!Uri.IsHexDigit(c)) return false;
            return true;
        }

        /// <summary>Eski resimle aynı görünüm (bej zemin, üstü çizili kahverengi yazı); okunaklı olsun diye biraz büyütüldü, araya çizgiler eklendi.</summary>
        private static byte[] Ciz(string kod)
        {
            using (Bitmap resim = new Bitmap(120, 38))
            using (Graphics g = Graphics.FromImage(resim))
            using (SolidBrush firca = new SolidBrush(Color.SandyBrown))
            using (Font yazi = new Font(FontFamily.GenericSerif.Name, 22, FontStyle.Strikeout, GraphicsUnit.Pixel))
            using (Pen cizgi = new Pen(Color.FromArgb(120, Color.Peru), 1))
            using (MemoryStream akis = new MemoryStream())
            {
                g.Clear(Color.Beige);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                float x = 10;
                foreach (char c in kod)
                {
                    float y = 4 + RandomNumberGenerator.GetInt32(6);
                    g.DrawString(c.ToString(), yazi, firca, x, y);
                    x += 24 + RandomNumberGenerator.GetInt32(4);
                }

                for (int i = 0; i < 4; i++)
                    g.DrawLine(cizgi, RandomNumberGenerator.GetInt32(120), RandomNumberGenerator.GetInt32(38),
                        RandomNumberGenerator.GetInt32(120), RandomNumberGenerator.GetInt32(38));

                resim.Save(akis, ImageFormat.Png);
                return akis.ToArray();
            }
        }
    }
}
