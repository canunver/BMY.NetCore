using System;
using System.IO;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using TNS.KYM;

namespace Istemci.Guncelleme
{
    /// <summary>
    /// Güncelleme menüsündeki teknik sayfaların (Yazılım Güncelle, Dosyalar, Komut Çalıştır, Şifre Üret, Veritabanı Index)
    /// ortak yetki denetimleri ve yardımcıları. Yetkiler eski sayfalardaki gibidir:
    ///   Yazılım Güncelle : menü yetkisi (eski SayfayaGirisYapabilirMi; menu.xml'de Yetki="1")
    ///   Veritabanı Index : sistem yöneticisi
    ///   Dosyalar, Komut Çalıştır, Şifre Üret : yalnızca sistem yöneticisi olan "stratek" kullanıcısı (kullanıcı isteği;
    ///   menu.xml'de Kullanici="stratek")
    /// Sayfa modelleri ve sayfaların bütün uçları aynı denetimi kullanır.
    /// </summary>
    public static class GuncellemeOrtak
    {
        public const string YazilimGuncelleSayfa = "../YazilimGuncelle/YazilimGuncelle.aspx";

        /// <summary>Dosyalar, Komut Çalıştır ve Şifre Üret sayfalarını kullanabilen tek kullanıcı.</summary>
        public const string StratekKullanici = "stratek";

        /// <summary>Yüklenen dosyalar için üst sınır (Zip'ten güncelleme, Dosyalar sayfası).</summary>
        public const long AzamiYukleme = 1024L * 1024L * 1024L;

        public static bool Yonetici(Kullanici kullanan)
        {
            return kullanan != null && kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        public static bool YazilimGuncelleYetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, YazilimGuncelleSayfa);
        }

        /// <summary>Sistem yöneticisi olan "stratek" kullanıcısı mı?</summary>
        public static bool Stratek(Kullanici kullanan)
        {
            return Yonetici(kullanan) && kullanan.kullaniciKodu == StratekKullanici;
        }

        public static bool DosyalarYetkili(Kullanici kullanan)
        {
            return Stratek(kullanan);
        }

        public static bool KomutYetkili(Kullanici kullanan)
        {
            return Stratek(kullanan);
        }

        public static bool SifreUretYetkili(Kullanici kullanan)
        {
            return Stratek(kullanan);
        }

        /// <summary>Oturumu ve yetkiyi denetler; geçerse null döner.</summary>
        public static IResult Denetle(HttpContext baglam, Func<Kullanici, bool> yetki, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!yetki(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        public static IResult Hata(string hata)
        {
            return Results.Json(new { basarili = false, hata = hata });
        }

        /// <summary>Büyük dosya yüklemesi için isteğin gövde sınırını kaldırır (gövde okunmadan önce çağrılır).</summary>
        public static void BuyukGovdeyeIzinVer(HttpContext baglam)
        {
            IHttpMaxRequestBodySizeFeature boyut = baglam.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (boyut != null && !boyut.IsReadOnly) boyut.MaxRequestBodySize = AzamiYukleme;
        }

        public static FormOptions BuyukForm()
        {
            return new FormOptions { MultipartBodyLengthLimit = AzamiYukleme };
        }

        /// <summary>Uygulama klasörünün tam yolu (sonunda ayraç yok).</summary>
        public static string KokBul(string uygulamaKlasoru)
        {
            return Path.GetFullPath(uygulamaKlasoru).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>Düz bir dosya / klasör adı mı (yol ayracı, geçersiz karakter, . ya da .. değil).</summary>
        public static bool AdGecerli(string ad)
        {
            return !string.IsNullOrWhiteSpace(ad) && ad == Path.GetFileName(ad) && ad.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
                   && ad != "." && ad != "..";
        }
    }
}
