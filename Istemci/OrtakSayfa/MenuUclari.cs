using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    public class DilIstegi
    {
        public string Dil { get; set; }
    }

    public class TemaIstegi
    {
        public string Tema { get; set; }

        /// <summary>Üst şerit görünümü: renkli (varsayılan) ya da sade (beyaz).</summary>
        public string Ust { get; set; }
    }

    public class MenuDurumIstegi
    {
        public string Url { get; set; }
    }

    /// <summary>
    /// Sayfaların ortak çerçevesi (üst başlık, menü, alt bilgi) için uçlar.
    /// Menü verisi açılış sayfasında (GirisBilgi) bir kez okunur ve tarayıcıda saklanır; diğer sayfalar saklanandan çizer.
    /// </summary>
    public static class MenuUclari
    {
        /// <summary>
        /// Kullanıcının seçebileceği renk temaları. Klasik, temalar genişletilmeden önceki sade görünümdür ve varsayılandır.
        /// Theme1..4 eski App_Themes\Theme1..4'ün karşılığıdır; Theme5 ve Theme6 yalnızca Core'da vardır.
        /// Renkler wwwroot/css/tema.css dosyasındadır.
        /// </summary>
        public static readonly string[] Temalar = { "Klasik", "Theme1", "Theme2", "Theme3", "Theme4", "Theme5", "Theme6" };

        /// <summary>Eski uygulamanın da tanıdığı temalar: kullanıcı değişkeni TEMA'ya yalnızca bunlar yazılır.</summary>
        private static readonly string[] eskiTemalar = { "Theme1", "Theme2", "Theme3", "Theme4" };

        /// <summary>Üst şerit görünümleri.</summary>
        public static readonly string[] UstGorunumler = { "renkli", "sade" };

        /// <summary>Temaların kullanıcının dilindeki adları (Theme1..6 sırasıyla).</summary>
        public static Dictionary<string, string> TemaAdlari()
        {
            return new Dictionary<string, string>
            {
                { "Klasik", Metin.Menu("TemaKlasik") },
                { "Theme1", Metin.Kullanici("FRMTEM002") },
                { "Theme2", Metin.Kullanici("FRMTEM003") },
                { "Theme3", Metin.Kullanici("FRMTEM004") },
                { "Theme4", Metin.Kullanici("FRMTEM005") },
                { "Theme5", Metin.Menu("TemaMor") },
                { "Theme6", Metin.Menu("TemaAntrasit") }
            };
        }

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ortaksayfa/menu", Menu);
            app.MapPost("/api/ortaksayfa/tema", TemaKaydet);
            app.MapGet("/api/ortaksayfa/tema", TemaOku);
            app.MapPost("/api/ortaksayfa/dil", DilKaydet);
            app.MapPost("/api/ortaksayfa/menu-durum-eski", MenuDurumEski);
        }

        private static readonly object menuKilit = new object();
        private static readonly Regex dugumDeseni = new Regex(@"<Node\b[^>]*>", RegexOptions.Compiled);
        private static readonly Regex urlDeseni = new Regex(@"\bURL=""([^""]*)""", RegexOptions.Compiled);

        /// <summary>
        /// Durumu Yeni olan (kırmızı) bir menü seçeneğine tıklandı: menu.xml'de aynı adresli öğelerin Durum="Yeni" niteliği
        /// Durum="Eski" yapılır (ekran incelendi demektir). Dosya metin olarak değiştirilir, biçimi bozulmaz.
        /// </summary>
        private static async Task<IResult> MenuDurumEski(HttpContext baglam, MenuDurumIstegi istek, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (OturumBilgisi.KullaniciOku(baglam) == null) return Results.Unauthorized();
            string url = istek != null && istek.Url != null ? istek.Url.Trim() : "";
            if (url == "") return Results.BadRequest();

            try
            {
                int degisen = 0;
                lock (menuKilit)
                {
                    string yol = MenuOkuyucu.DosyaYolu(ortam.ContentRootPath);
                    byte[] bayt = File.ReadAllBytes(yol);
                    bool bom = bayt.Length >= 3 && bayt[0] == 0xEF && bayt[1] == 0xBB && bayt[2] == 0xBF;
                    string metin = new UTF8Encoding(false).GetString(bayt, bom ? 3 : 0, bom ? bayt.Length - 3 : bayt.Length);
                    string yeni = dugumDeseni.Replace(metin, m =>
                    {
                        string etiket = m.Value;
                        if (etiket.IndexOf("Durum=\"Yeni\"", StringComparison.Ordinal) < 0) return etiket;
                        Match u = urlDeseni.Match(etiket);
                        if (!u.Success || !string.Equals(WebUtility.HtmlDecode(u.Groups[1].Value).Trim(), url, StringComparison.OrdinalIgnoreCase)) return etiket;
                        degisen++;
                        return etiket.Replace("Durum=\"Yeni\"", "Durum=\"Eski\"");
                    });
                    if (degisen > 0) File.WriteAllText(yol, yeni, new UTF8Encoding(bom));
                }
                return Results.Json(new { basarili = true, degisen = degisen });
            }
            catch (Exception e)
            {
                return Results.Json(new { basarili = false, hata = e.Message });
            }
        }

        private static IResult Menu(HttpContext baglam, IWebHostEnvironment ortam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();

            List<MenuGrup> gruplar;
            string menuHatasi = null;
            try
            {
                gruplar = new MenuOkuyucu(kullanan).Oku(MenuOkuyucu.DosyaYolu(ortam.ContentRootPath));
            }
            catch (Exception e)
            {
                gruplar = new List<MenuGrup>();
                menuHatasi = "Menü okunamadı: " + e.Message;
            }

            List<object> diller = new List<object>();
            foreach (string kod in Dil.Desteklenen())
                diller.Add(new { kod = kod, ad = Dil.Ad(kod) });

            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            string[] tema = TemaBul(kullanan);

            return Results.Json(new
            {
                kullaniciKodu = kullanan.kullaniciKodu,
                // Sayfalardaki <meta name="menu-surum"> ile karşılaştırılır; farklıysa saklanan menü yenilenir (MenuSurum).
                menuSurum = MenuSurum.Deger(ortam.ContentRootPath),
                adSoyad = OrtakFonksiyonlar.IlkHarflerBuyuk((kullanan.ad + " " + kullanan.soyad).Trim()),
                yazilimAd = Ayar.Oku("yazilimAd", "SGB.net"),
                kurumAdi = Ayar.Degisken(DateTime.Now.Year, "/KURUMAD"),
                logo = ortam.WebRootFileProvider.GetFileInfo("img/kurumlogo.jpg").Exists ? "img/kurumlogo.jpg" : null,
                surum = v != null ? v.Major + "." + v.Minor + "." + v.Build : "",
                tema = tema[0],
                temaUst = tema[1],
                temalar = Temalar,
                temaAdlari = TemaAdlari(),
                dil = Dil.Aktif,
                // SadeceLDAP / Entegrasyon açıksa kullanıcı menüsünde Parola Değiştir gösterilmez.
                parolaDegistir = Istemci.Kullanicilar.ParolaDegistirUclari.Kullanilabilir(),
                diller = diller,
                // Sık kullanılan ekranların menü ID'leri (sol menünün başında gösterilir, sayfa başlığındaki +/− ile değişir).
                sikKullanilanlar = SikKullanilanOku(kullanan),
                metin = new
                {
                    anaSayfa = Metin.Ortak("IST009", "Ana Sayfa"),
                    cikis = Metin.Ortak("IST012", "Çıkış"),
                    yardim = Metin.Ortak("IST054", "Yardım"),
                    renkTemasi = Metin.Menu("RenkTemasi"),
                    tema = Metin.Menu("Tema"),
                    temaAyarlari = Metin.Menu("TemaAyarlari"),
                    dil = Metin.Menu("Dil"),
                    surum = Metin.Menu("Surum"),
                    menuYuklenemedi = Metin.Menu("MenuYuklenemedi"),
                    menuGruplari = Metin.Menu("MenuGruplari"),
                    menu = Metin.Menu("Menu"),
                    sifreDegistir = Metin.Kullanici("FRMPAD005"),
                    ekranAra = Metin.Menu("EkranAra"),
                    sonucYok = Metin.Menu("SonucYok"),
                    menuSabitle = Metin.Menu("MenuSabitle"),
                    menuGosterGizle = Metin.Menu("MenuGosterGizle"),
                    modul = Metin.Menu("Modul"),
                    modulSec = Metin.Menu("ModulSec"),
                    sikKullanilanlar = Metin.Menu("SikKullanilanlar"),
                    sikEkle = Metin.Menu("SikEkle"),
                    sikCikar = Metin.Menu("SikCikar"),
                    sikBos = Metin.Menu("SikBos"),
                    sikHata = Metin.Menu("SikHata")
                },
                menuHatasi = menuHatasi,
                gruplar = gruplar
            });
        }

        private static List<string> SikKullanilanOku(Kullanici kullanan)
        {
            try
            {
                return SikKullanilanUclari.Oku(kullanan);
            }
            catch (Exception)
            {
                // Okunamazsa menü sık kullanılanlarsız çizilir.
                return new List<string>();
            }
        }

        /// <summary>
        /// Kullanıcının seçtiği tema ve üst şerit görünümü: { tema, ust }.
        /// Core'un kullanıcı değişkeni TEMACORE ("Theme5|sade" biçiminde) okunur; seçim yapılmamışsa Klasik.
        /// (Eski uygulamanın TEMA değişkeni Core'da kullanılmaz.) Üst şerit verilmemişse renklidir.
        /// </summary>
        public static string[] TemaBul(Kullanici kullanan)
        {
            string tema = null, ust = null;
            try
            {
                TNSCollection<string> kayit = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullanan.kullaniciKodu, "TEMACORE");
                if (kayit != null && kayit.Count > 0 && !string.IsNullOrEmpty(kayit[0]))
                {
                    string[] parca = kayit[0].Split('|');
                    tema = parca[0];
                    if (parca.Length > 1) ust = parca[1];
                }
            }
            catch (Exception)
            {
                // Tema okunamazsa varsayılan kullanılır.
            }
            if (Array.IndexOf(Temalar, tema) < 0) tema = Temalar[0];
            if (Array.IndexOf(UstGorunumler, ust) < 0) ust = UstGorunumler[0];
            return new[] { tema, ust };
        }

        /// <summary>Tema Değiştirme sayfası için: seçili tema, üst şerit görünümü, temalar ve adları.</summary>
        private static IResult TemaOku(HttpContext baglam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            string[] tema = TemaBul(kullanan);
            return Results.Json(new { tema = tema[0], ust = tema[1], temalar = Temalar, temaAdlari = TemaAdlari() });
        }

        /// <summary>
        /// Kullanıcının dilini değiştirir: oturumdaki kullanıcının aktifKultur alanı güncellenir ve seçim çereze yazılır.
        /// </summary>
        private static async Task<IResult> DilKaydet(HttpContext baglam, DilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null || !Dil.Gecerli(istek.Dil)) return Results.BadRequest();

            OturumVerisi veri = OturumBilgisi.Oku(baglam);
            if (veri == null || veri.Kullanan == null) return Results.Unauthorized();

            veri.Kullanan.aktifKultur = istek.Dil;
            OturumBilgisi.Guncelle(baglam, veri);
            Dil.CerezYaz(baglam, istek.Dil);

            return Results.Json(new { basarili = true });
        }

        private static async Task<IResult> TemaKaydet(HttpContext baglam, TemaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();

            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (istek == null || Array.IndexOf(Temalar, istek.Tema) < 0) return Results.BadRequest();
            string ust = string.IsNullOrEmpty(istek.Ust) ? UstGorunumler[0] : istek.Ust;
            if (Array.IndexOf(UstGorunumler, ust) < 0) return Results.BadRequest();

            // Core'un seçimi TEMACORE'a yazılır. Eski uygulama TEMA'yı App_Themes klasör adı olarak kullandığı için
            // TEMA'ya yalnızca eskide de olan Theme1..4 yazılır (Theme5/6 seçilirse eski uygulamadaki tema değişmez).
            TNSCollection degerler = new TNSCollection();
            degerler.Add(istek.Tema + "|" + ust);
            Sonuc sonuc = kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), "TEMACORE", degerler);

            if (sonuc.islemSonuc && Array.IndexOf(eskiTemalar, istek.Tema) >= 0)
            {
                TNSCollection eski = new TNSCollection();
                eski.Add(istek.Tema);
                sonuc = kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), "TEMA", eski);
            }

            return Results.Json(new { basarili = sonuc.islemSonuc, hata = sonuc.hataStr });
        }
    }
}
