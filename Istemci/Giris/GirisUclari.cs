using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using TNS;
using TNS.KYM;

namespace Istemci.Giris
{
    /// <summary>Giriş sayfasının JavaScript'ten gönderdiği istek.</summary>
    public class GirisIstegi
    {
        public string KullaniciKodu { get; set; }
        public string Parola { get; set; }
        public string IkinciKod { get; set; }
        public bool Hatirla { get; set; }
        public string DonusAdresi { get; set; }
        /// <summary>Hatalı deneme sınırı aşıldıysa resimdeki güvenlik kodu.</summary>
        public string GuvenlikKodu { get; set; }
    }

    /// <summary>Giriş uçlarının JSON yanıtı.</summary>
    public class GirisYaniti
    {
        public bool Basarili { get; set; }
        public string Hata { get; set; }
        public string Bilgi { get; set; }
        /// <summary>Parola doğru; ikinci adım olarak doğrulama kodu istenecek.</summary>
        public bool IkinciFaktorGerekli { get; set; }
        /// <summary>Giriş tamamlandı; tarayıcının gideceği adres.</summary>
        public string Yonlen { get; set; }
        /// <summary>Bu IP adresinden yapılan hatalı denemeler sınıra ulaştı; sonraki istekte güvenlik kodu istenir.</summary>
        public bool GuvenlikKoduGerekli { get; set; }
    }

    /// <summary>
    /// Giriş akışı (eski Giris.aspx.cs):
    ///   1. /api/giris/parola   : kullanıcı kodu + parola doğrulanır. İki aşamalı giriş kapalıysa giriş tamamlanır;
    ///                            açıksa doğrulama kodu e-posta ile gönderilir ve ikinci adım istenir.
    ///   2. /api/giris/tamamla  : kullanıcı kodu + parola + doğrulama kodu ile giriş tamamlanır.
    ///      /api/giris/kod-yenile: yeni doğrulama kodu gönderir.
    ///      /api/giris/cikis    : oturumu kapatır.
    /// Parola sunucuda adımlar arasında saklanmaz; ikinci adımda tarayıcı yeniden gönderir.
    /// Aynı IP adresinden /GirisDenemeSayi kadar hatalı giriş yapılınca (tarayıcı değişse de) her istekte güvenlik resmindeki kod
    /// istenir (eski hdnGuvenlikResmi). Sayı eskisi gibi veritabanında IP adresine bağlı "OlumsuzGirisSayisi" değişkeninde tutulur,
    /// başarılı girişte sıfırlanır.
    /// </summary>
    public static class GirisUclari
    {
        private static readonly TimeSpan KodGecerlilikSuresi = TimeSpan.FromHours(8);
        private static readonly TimeSpan KodYenilemeAraligi = TimeSpan.FromSeconds(60);

        public static void Ekle(WebApplication app)
        {
            app.MapPost("/api/giris/parola", Parola).AllowAnonymous();
            app.MapPost("/api/giris/tamamla", Tamamla).AllowAnonymous();
            app.MapPost("/api/giris/kod-yenile", KodYenile).AllowAnonymous();
            app.MapPost("/api/giris/cikis", Cikis);
        }

        /// <summary>appSettings ikiAsamaliGiris = 1 veya 2 ise giriş iki aşamalıdır.</summary>
        public static bool IkiAsamali
        {
            get
            {
                string deger = Ayar.Oku("ikiAsamaliGiris", "0");
                return deger == "1" || deger == "2";
            }
        }

        // ------------------------------------------------------------------ uçlar

        private static async Task<IResult> Parola(HttpContext baglam, GirisIstegi istek, IAntiforgery sahtecilik, IMemoryCache bellek)
        {
            GirisYaniti onKontrol = await OnKontrol(baglam, istek, sahtecilik, bellek);
            if (onKontrol != null) return Yanit(baglam, onKontrol);

            if (!IkiAsamali)
                return Yanit(baglam, await GirisiTamamla(baglam, istek, bellek));

            IKYMServis servisKym = TNS.KYM.Arac.Tanimla();
            KullaniciVAN kv = servisKym.GirisYapabilirMi(istek.KullaniciKodu, istek.Parola, false);
            if (kv == null || kv.sonuc == null || !kv.sonuc.islemSonuc || kv.kullanici == null)
            {
                HataliDenemeYaz(baglam);
                Izle(baglam, istek.KullaniciKodu, ENUMIzleme.HATALIGIRIS);
                return Yanit(baglam, Hatali(kv != null && kv.sonuc != null ? kv.sonuc.hataStr : null));
            }

            IkinciKodGonder(servisKym, kv.kullanici, false);

            GirisYaniti yanit = new GirisYaniti();
            yanit.Basarili = true;
            yanit.IkinciFaktorGerekli = true;
            yanit.Bilgi = string.Format(Metin.Menu("Giris_KodGonderildi"), EPostaMaskele(kv.kullanici.EPOSTA));
            return Yanit(baglam, yanit);
        }

        private static async Task<IResult> Tamamla(HttpContext baglam, GirisIstegi istek, IAntiforgery sahtecilik, IMemoryCache bellek)
        {
            GirisYaniti onKontrol = await OnKontrol(baglam, istek, sahtecilik, bellek);
            if (onKontrol != null) return Yanit(baglam, onKontrol);

            return Yanit(baglam, await GirisiTamamla(baglam, istek, bellek));
        }

        private static async Task<IResult> KodYenile(HttpContext baglam, GirisIstegi istek, IAntiforgery sahtecilik, IMemoryCache bellek)
        {
            GirisYaniti onKontrol = await OnKontrol(baglam, istek, sahtecilik, bellek);
            if (onKontrol != null) return Yanit(baglam, onKontrol);

            // Kod, parolayı bilen kişiye gönderilir; bu yüzden parola burada yeniden doğrulanır.
            IKYMServis servisKym = TNS.KYM.Arac.Tanimla();
            KullaniciVAN kv = servisKym.GirisYapabilirMi(istek.KullaniciKodu, istek.Parola, false);
            if (kv == null || kv.sonuc == null || !kv.sonuc.islemSonuc || kv.kullanici == null)
            {
                HataliDenemeYaz(baglam);
                return Yanit(baglam, Hatali(kv != null && kv.sonuc != null ? kv.sonuc.hataStr : null));
            }

            string yenilemeAnahtari = "kodyenile|" + kv.kullanici.kullaniciKodu;
            object dolu;
            if (bellek.TryGetValue(yenilemeAnahtari, out dolu))
                return Yanit(baglam, Hatali(Metin.Menu("Giris_KodAzOnce")));
            bellek.Set(yenilemeAnahtari, true, KodYenilemeAraligi);

            IkinciKodGonder(servisKym, kv.kullanici, true);

            GirisYaniti yanit = new GirisYaniti();
            yanit.Basarili = true;
            yanit.IkinciFaktorGerekli = true;
            yanit.Bilgi = string.Format(Metin.Menu("Giris_YeniKodGonderildi"), EPostaMaskele(kv.kullanici.EPOSTA));
            return Yanit(baglam, yanit);
        }

        private static async Task<IResult> Cikis(HttpContext baglam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam))
                return Results.BadRequest();

            await OturumBilgisi.Bitir(baglam);

            GirisYaniti yanit = new GirisYaniti();
            yanit.Basarili = true;
            yanit.Yonlen = baglam.Request.PathBase + "/Giris";
            return Results.Json(yanit);
        }

        // ------------------------------------------------------------------ giriş

        /// <summary>Bütün giriş uçlarında ortak kontroller. Sorun yoksa null döner.</summary>
        private static async Task<GirisYaniti> OnKontrol(HttpContext baglam, GirisIstegi istek, IAntiforgery sahtecilik, IMemoryCache bellek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam))
                return Hatali(Metin.Menu("Giris_SayfaSuresi"));

            if (istek == null || string.IsNullOrWhiteSpace(istek.KullaniciKodu) || string.IsNullOrEmpty(istek.Parola))
                return Hatali(Metin.Menu("Giris_KodParolaGir"));

            istek.KullaniciKodu = istek.KullaniciKodu.Trim();

            // Eski Giris.aspx: hatalı deneme sınırı aşıldıysa güvenlik kodu doğru girilmeden parola denenmez.
            if (GuvenlikKoduGerekli(baglam))
            {
                if (string.IsNullOrWhiteSpace(istek.GuvenlikKodu))
                    return Hatali(Metin.Menu("Giris_GuvenlikKoduGir"));
                if (!GuvenlikKodu.Dogrula(baglam, bellek, istek.GuvenlikKodu))
                    return Hatali(Metin.Menu("Giris_GuvenlikKoduYanlis"));
            }

            // Bakım modu: sistem değişkeni /TEKKULLANICI doluysa yalnızca o kullanıcı girebilir.
            string tekKullanici = Ayar.Degisken(0, "/TEKKULLANICI");
            if (!string.IsNullOrEmpty(tekKullanici) && tekKullanici != istek.KullaniciKodu)
                return Hatali(Metin.Menu("Giris_Bakim"));

            return null;
        }

        private static async Task<GirisYaniti> GirisiTamamla(HttpContext baglam, GirisIstegi istek, IMemoryCache bellek)
        {
            IKYMServis servisKym = TNS.KYM.Arac.Tanimla();
            KullaniciVAN kv = servisKym.GirisYap(istek.KullaniciKodu, istek.Parola);

            if (kv != null && kv.sonuc != null && kv.sonuc.islemSonuc && kv.kullanici != null && IkiAsamali)
                IkinciKodKontrol(servisKym, kv, istek.IkinciKod);

            if (kv == null || kv.sonuc == null || !kv.sonuc.islemSonuc || kv.kullanici == null)
            {
                HataliDenemeYaz(baglam);
                Izle(baglam, istek.KullaniciKodu, ENUMIzleme.HATALIGIRIS);
                return Hatali(kv != null && kv.sonuc != null ? kv.sonuc.hataStr : null);
            }

            Kullanici kullanan = kv.kullanici;
            string ip = Ayar.IPAdresi(baglam);
            kullanan.aktifKultur = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            kullanan.ipAddress = ip;
            // Eski kod oturumdaki kullanıcının telefon alanına da IP adresini yazıyordu; bu alanı okuyan kod olabileceği için korundu.
            kullanan.telefon = ip;

            await OturumBilgisi.Baslat(baglam, kullanan);

            if (istek.Hatirla)
            {
                CookieOptions secenek = new CookieOptions();
                secenek.Expires = DateTimeOffset.Now.AddYears(1);
                secenek.HttpOnly = true;
                secenek.Secure = baglam.Request.IsHttps;
                secenek.SameSite = SameSiteMode.Lax;
                baglam.Response.Cookies.Append("KulAd", istek.KullaniciKodu, secenek);
            }
            else
                baglam.Response.Cookies.Delete("KulAd");

            HataliDenemeSifirla(baglam);
            Izle(baglam, istek.KullaniciKodu, ENUMIzleme.BASARILIGIRIS);

            GirisYaniti yanit = new GirisYaniti();
            yanit.Basarili = true;
            // Giriş sayfası uygulamanın kökündedir; açılış sayfasının adresi ona göre görelidir.
            yanit.Yonlen = YerelAdresMi(istek.DonusAdresi) ? istek.DonusAdresi : IstemciBaslangic.AcilisSayfasi;
            return yanit;
        }

        /// <summary>Yanıta, sonraki istekte güvenlik kodunun gerekip gerekmediği eklenir.</summary>
        private static IResult Yanit(HttpContext baglam, GirisYaniti yanit)
        {
            if (!yanit.Basarili || yanit.IkinciFaktorGerekli)
                yanit.GuvenlikKoduGerekli = GuvenlikKoduGerekli(baglam);
            return Results.Json(yanit);
        }

        private static GirisYaniti Hatali(string hata)
        {
            GirisYaniti yanit = new GirisYaniti();
            yanit.Basarili = false;
            yanit.Hata = string.IsNullOrWhiteSpace(hata) ? Metin.Menu("Giris_Yapilamadi") : hata;
            return yanit;
        }

        /// <summary>Girişten sonra yalnızca bu uygulamanın içindeki bir adrese dönülür.</summary>
        private static bool YerelAdresMi(string adres)
        {
            if (string.IsNullOrEmpty(adres)) return false;
            if (adres[0] != '/') return false;
            if (adres.Length > 1 && (adres[1] == '/' || adres[1] == '\\')) return false;
            return adres.IndexOfAny(new[] { '\r', '\n' }) < 0;
        }

        private static void Izle(HttpContext baglam, string kullaniciKodu, ENUMIzleme izlemeKod)
        {
            try
            {
                TNS.KYM.Arac.Tanimla().IzlemeKaydet(Ayar.IPAdresi(baglam), kullaniciKodu, izlemeKod, "KYM", baglam.Request.Path.ToString(), null);
            }
            catch (Exception)
            {
                // İzleme kaydı yazılamaması girişi engellemez.
            }
        }

        // ------------------------------------------------------------------ hatalı deneme sınırı

        private const string DenemeDegiskeni = "OlumsuzGirisSayisi";

        /// <summary>Sistem değişkeni /GirisDenemeSayi: güvenlik kodu istenmeden yapılabilecek hatalı deneme sayısı (0 = güvenlik kodu istenmez).</summary>
        private static int DenemeSiniri()
        {
            return OrtakFonksiyonlar.ConvertToInt(Ayar.Degisken(0, "/GirisDenemeSayi", "0"), 0);
        }

        /// <summary>Bu IP adresinden yapılmış ardışık hatalı deneme sayısı (eski GirisDenemeSayisiKontrolu).</summary>
        private static int HataliDenemeSayisi(HttpContext baglam)
        {
            int sayi = 0;
            TNSCollection<string> kayit = TNS.KYM.Arac.Tanimla().DegiskenleriListele(Ayar.IPAdresi(baglam), DenemeDegiskeni);
            if (kayit != null)
                foreach (string deger in kayit)
                    sayi = OrtakFonksiyonlar.ConvertToInt(deger, 0);
            return sayi;
        }

        /// <summary>Giriş sayfası ve giriş uçları: bu IP adresinden sonraki denemede güvenlik kodu istenecek mi?</summary>
        public static bool GuvenlikKoduGerekli(HttpContext baglam)
        {
            int sinir = DenemeSiniri();
            return sinir > 0 && HataliDenemeSayisi(baglam) >= sinir;
        }

        private static void HataliDenemeYaz(HttpContext baglam)
        {
            if (DenemeSiniri() <= 0) return;
            TNSCollection degerler = new TNSCollection();
            degerler.Add((HataliDenemeSayisi(baglam) + 1).ToString());
            TNS.KYM.Arac.Tanimla().DegiskenKaydet(Ayar.IPAdresi(baglam), DenemeDegiskeni, degerler);
        }

        private static void HataliDenemeSifirla(HttpContext baglam)
        {
            if (DenemeSiniri() <= 0) return;
            TNS.KYM.Arac.Tanimla().DegiskenKaydet(Ayar.IPAdresi(baglam), DenemeDegiskeni, new TNSCollection());
        }

        // ------------------------------------------------------------------ doğrulama kodu

        /// <summary>
        /// Kullanıcının geçerli doğrulama kodu yoksa (ya da zorla isteniyorsa) yeni kod üretir, saklar ve e-posta ile gönderir.
        /// Kod, eski sistemdeki gibi 8 saat geçerlidir ve bu sürede yapılan girişlerde aynı kod kullanılır.
        /// </summary>
        private static void IkinciKodGonder(IKYMServis servisKym, Kullanici kullanici, bool zorla)
        {
            string mevcutKod;
            if (!zorla && !KodSuresiDolmus(servisKym, kullanici, out mevcutKod)) return;

            string kod = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            TNSCollection degerler = new TNSCollection();
            degerler.Add(kod + ";" + DateTime.Now.Ticks);
            kullanici.DegiskenleriSakla(servisKym, "IkinciParola", degerler);

            string yazilimAd = Ayar.Oku("yazilimAd", "SGB.net");
            string mesaj = string.Format(Metin.Menu("Giris_EpostaMesaj"), yazilimAd, kod, (int)KodGecerlilikSuresi.TotalHours);
            servisKym.EPostaGonder("", kullanici.EPOSTA, string.Format(Metin.Menu("Giris_EpostaKonu"), yazilimAd), mesaj, true, false, null);
        }

        private static bool KodSuresiDolmus(IKYMServis servisKym, Kullanici kullanici, out string kod)
        {
            kod = "";
            DateTime zaman = DateTime.MinValue;

            TNSCollection<string> kayit = servisKym.DegiskenleriListele(kullanici.kullaniciKodu, "IkinciParola");
            if (kayit != null && kayit.Count > 0 && !string.IsNullOrWhiteSpace(kayit[0]))
            {
                string[] parca = kayit[0].Split(';');
                kod = parca[0];
                long tik;
                if (parca.Length > 1 && long.TryParse(parca[1], out tik) && tik > DateTime.MinValue.Ticks && tik < DateTime.MaxValue.Ticks)
                    zaman = new DateTime(tik);
            }

            return string.IsNullOrEmpty(kod) || DateTime.Now - zaman > KodGecerlilikSuresi;
        }

        private static void IkinciKodKontrol(IKYMServis servisKym, KullaniciVAN kv, string girilenKod)
        {
            string kod;
            if (KodSuresiDolmus(servisKym, kv.kullanici, out kod))
            {
                kv.sonuc.DegerAta(Metin.Menu("Giris_KodSuresi"));
                return;
            }

            girilenKod = (girilenKod ?? "").Trim();
            if (girilenKod.Length == 0 || !CryptographicOperations.FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(girilenKod), System.Text.Encoding.UTF8.GetBytes(kod)))
                kv.sonuc.DegerAta(Metin.Menu("Giris_KodHatali"));
        }

        private static string EPostaMaskele(string eposta)
        {
            if (string.IsNullOrWhiteSpace(eposta)) return Metin.Menu("Giris_KayitliEposta");
            int at = eposta.IndexOf('@');
            if (at < 1) return Metin.Menu("Giris_KayitliEposta");
            return eposta.Substring(0, 1) + new string('*', Math.Min(at - 1, 5)) + eposta.Substring(at);
        }
    }
}
