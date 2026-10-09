using System;
using System.Collections.Generic;
using System.Reflection;
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
    }

    /// <summary>
    /// Sayfaların ortak çerçevesi (üst başlık, menü, alt bilgi) için uçlar.
    /// Menü verisi açılış sayfasında (GirisBilgi) bir kez okunur ve tarayıcıda saklanır; diğer sayfalar saklanandan çizer.
    /// </summary>
    public static class MenuUclari
    {
        /// <summary>Kullanıcının seçebileceği renk temaları (eski App_Themes\Theme1..4).</summary>
        public static readonly string[] Temalar = { "Theme1", "Theme2", "Theme3", "Theme4" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ortaksayfa/menu", Menu);
            app.MapPost("/api/ortaksayfa/tema", TemaKaydet);
            app.MapPost("/api/ortaksayfa/dil", DilKaydet);
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

            return Results.Json(new
            {
                kullaniciKodu = kullanan.kullaniciKodu,
                adSoyad = OrtakFonksiyonlar.IlkHarflerBuyuk((kullanan.ad + " " + kullanan.soyad).Trim()),
                yazilimAd = Ayar.Oku("yazilimAd", "SGB.net"),
                kurumAdi = Ayar.Degisken(DateTime.Now.Year, "/KURUMAD"),
                logo = ortam.WebRootFileProvider.GetFileInfo("img/kurumlogo.jpg").Exists ? "img/kurumlogo.jpg" : null,
                surum = v != null ? v.Major + "." + v.Minor + "." + v.Build : "",
                tema = TemaBul(kullanan),
                temalar = Temalar,
                dil = Dil.Aktif,
                diller = diller,
                metin = new
                {
                    anaSayfa = Metin.Ortak("IST009", "Ana Sayfa"),
                    cikis = Metin.Ortak("IST012", "Çıkış"),
                    renkTemasi = Metin.Menu("RenkTemasi"),
                    tema = Metin.Menu("Tema"),
                    dil = Metin.Menu("Dil"),
                    surum = Metin.Menu("Surum"),
                    menuYuklenemedi = Metin.Menu("MenuYuklenemedi"),
                    menuGruplari = Metin.Menu("MenuGruplari"),
                    menu = Metin.Menu("Menu"),
                    sifreDegistir = Metin.Ortak("Menu6_1_3", "Parola Değiştirme"),
                    ekranAra = Metin.Menu("EkranAra"),
                    sonucYok = Metin.Menu("SonucYok"),
                    menuSabitle = Metin.Menu("MenuSabitle"),
                    menuGosterGizle = Metin.Menu("MenuGosterGizle"),
                    modul = Metin.Menu("Modul"),
                    modulSec = Metin.Menu("ModulSec")
                },
                menuHatasi = menuHatasi,
                gruplar = gruplar
            });
        }

        /// <summary>Kullanıcının seçtiği tema (kullanıcı değişkeni TEMA); yoksa appSettings Theme; o da yoksa Theme1.</summary>
        private static string TemaBul(Kullanici kullanan)
        {
            string tema = null;
            try
            {
                TNSCollection<string> kayit = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullanan.kullaniciKodu, "TEMA");
                if (kayit != null && kayit.Count > 0) tema = kayit[0];
            }
            catch (Exception)
            {
                // Tema okunamazsa varsayılan kullanılır.
            }
            if (string.IsNullOrEmpty(tema)) tema = Ayar.Oku("Theme", Temalar[0]);
            return Array.IndexOf(Temalar, tema) >= 0 ? tema : Temalar[0];
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

            TNSCollection degerler = new TNSCollection();
            degerler.Add(istek.Tema);
            Sonuc sonuc = kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), "TEMA", degerler);

            return Results.Json(new { basarili = sonuc.islemSonuc, hata = sonuc.hataStr });
        }
    }
}
