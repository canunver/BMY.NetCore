using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    /// <summary>Yardım sayfasının JavaScript'ten gönderdiği kayıt isteği.</summary>
    public class YardimKayitIstegi
    {
        public string sayfaKod { get; set; }
        public string metin { get; set; }
    }

    /// <summary>
    /// Sayfa yardımı (eski KYM/YardimSayfa.aspx) uçları:
    ///   GET  /api/yardim/oku?sayfaKod=KYM/ParolaDegistir : yardım metni (HTML) ve kullanıcının yazıp yazamayacağı
    ///   POST /api/yardim/kaydet { sayfaKod, metin }       : yalnızca yazabilen kullanıcı
    /// Metinler eskisi gibi KYM BuyukMetinOku / BuyukMetinSakla ile saklanır: modül "YARDIM", anahtar sayfa kodu
    /// (adresin son iki parçası, .aspx'siz: "Klasor/Sayfa"), dil kullanıcının aktifKultur'u. Böylece eski yardımlar aynen gelir.
    /// Yardıma eklenen dosyalar ortak ekli dosyalar bileşenindedir (modül "YRD", form = sayfa kodu; eski EkliDosyalarForm.aspx?mdl=YRD).
    /// Yazma yetkisi eski YoneticiMi: sistem yöneticisi; sayfa menüde Dünya Bankası Projesi (DBP) modülündeyse DBP proje yöneticisi de.
    /// </summary>
    public static class YardimUclari
    {
        public const string Modul = "YARDIM";
        public const string DosyaModul = "YRD";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yardim/oku", Oku);
            app.MapPost("/api/yardim/kaydet", Kaydet);
        }

        /// <summary>
        /// Sayfa kodunu eski biçime getirir: baştaki "../" ya da "/" ve sondaki ".aspx" atılır, sorgu kısmı alınmaz.
        /// Geçersizse boş döner.
        /// </summary>
        public static string SayfaKodDuzelt(string sayfaKod)
        {
            string kod = (sayfaKod ?? "").Trim();
            int soru = kod.IndexOfAny(new[] { '?', '#' });
            if (soru >= 0) kod = kod.Substring(0, soru);
            kod = kod.Replace('\\', '/');
            while (kod.StartsWith("../", StringComparison.Ordinal)) kod = kod.Substring(3);
            kod = kod.TrimStart('/');
            if (kod.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)) kod = kod.Substring(0, kod.Length - 5);
            if (kod.Length == 0 || kod.Length > 200 || kod.Contains("..")) return "";
            return kod;
        }

        /// <summary>Eski YardimSayfa.YoneticiMi.</summary>
        public static bool Yazabilir(Kullanici kullanan, string uygulamaKlasoru, string sayfaKod)
        {
            if (kullanan == null || string.IsNullOrEmpty(kullanan.kullaniciKodu)) return false;
            if (kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)) return true;
            // Eski kod sayfa menüde bulunamazsa (modül null) hata veriyordu; burada yetkisiz sayılır.
            string modulKod = SayfaModulBul(uygulamaKlasoru, sayfaKod);
            if (modulKod != null && modulKod.IndexOf("DBP", StringComparison.Ordinal) > -1)
                return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.DUNYABANKASIPROJEYONETICISI);
            return false;
        }

        private static string Dil(Kullanici kullanan)
        {
            return string.IsNullOrEmpty(kullanan.aktifKultur) ? CultureInfo.CurrentUICulture.Name : kullanan.aktifKultur;
        }

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, string sayfaKod)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            string kod = SayfaKodDuzelt(sayfaKod);
            if (kod == "") return Results.Json(new { basarili = false, hata = "Sayfa belirlenemedi." });
            try
            {
                IKYMServis servis = TNS.KYM.Arac.Tanimla();
                string dil = Dil(kullanan);
                string metin = servis.BuyukMetinOku(kullanan, Modul, kod, dil);
                // Dil kodu eski sistemde "tr-TR" ya da "tr" olabiliyordu; bu dilde metin yoksa öbür biçimiyle de bakılır.
                if (string.IsNullOrEmpty(metin))
                {
                    string diger = DigerDilKodu(dil);
                    if (diger != null) metin = servis.BuyukMetinOku(kullanan, Modul, kod, diger);
                }
                return Results.Json(new
                {
                    basarili = true,
                    sayfaKod = kod,
                    metin = metin ?? "",
                    yazabilir = Yazabilir(kullanan, ortam.ContentRootPath, kod),
                    dosyaModul = DosyaModul
                });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Yardım bilgisi okunamadı." });
            }
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, YardimKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (istek == null) return Results.BadRequest();
            string kod = SayfaKodDuzelt(istek.sayfaKod);
            if (kod == "") return Results.Json(new { basarili = false, hata = "Sayfa belirlenemedi." });
            // Eski btnKaydet_Click'teki metin.
            if (!Yazabilir(kullanan, ortam.ContentRootPath, kod)) return Results.Json(new { basarili = false, hata = "Yazma yetkiniz bulunmamaktadır!" });
            try
            {
                Sonuc s = TNS.KYM.Arac.Tanimla().BuyukMetinSakla(kullanan, Modul, kod, Dil(kullanan), istek.metin ?? "");
                if (s == null || !s.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(s != null ? s.hataStr : null) });
                return Results.Json(new { basarili = true, bilgi = "Kayıt başarılı" });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = HataMetni(ex.Message) });
            }
        }

        private static string DigerDilKodu(string dil)
        {
            if (string.IsNullOrEmpty(dil)) return null;
            try
            {
                if (dil.Contains('-')) return dil.Substring(0, dil.IndexOf('-'));
                string ozel = CultureInfo.CreateSpecificCulture(dil).Name;
                return string.Equals(ozel, dil, StringComparison.OrdinalIgnoreCase) ? null : ozel;
            }
            catch (CultureNotFoundException)
            {
                return null;
            }
        }

        private static string HataMetni(string hata)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return "Kayıt yapılamadı.";
            return hata;
        }

        // ------------------------------------------------------------------ sayfanın modülü (eski GenelIslemler.SayfaModulBul)

        private class ModulListesi
        {
            public DateTime zaman;
            public XmlDocument belge;
        }

        private static readonly ConcurrentDictionary<string, ModulListesi> onbellek = new ConcurrentDictionary<string, ModulListesi>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adresinde sayfa kodu geçen ilk menü seçeneğinin modülü (kendisinde ya da üstlerinde verilmiş Modul niteliği).
        /// Büyük-küçük harf ayrımı yoktur. Bulunamazsa null.
        /// </summary>
        public static string SayfaModulBul(string uygulamaKlasoru, string sayfaKod)
        {
            if (string.IsNullOrEmpty(sayfaKod)) return null;
            try
            {
                string dosya = MenuOkuyucu.DosyaYolu(uygulamaKlasoru);
                DateTime zaman = File.GetLastWriteTimeUtc(dosya);
                if (!onbellek.TryGetValue(dosya, out ModulListesi liste) || liste.zaman != zaman)
                {
                    XmlDocument belge = new XmlDocument();
                    belge.Load(dosya);
                    liste = new ModulListesi { zaman = zaman, belge = belge };
                    onbellek[dosya] = liste;
                }
                if (liste.belge.DocumentElement == null) return null;
                return ModulAra(liste.belge.DocumentElement.ChildNodes, sayfaKod.ToLowerInvariant(), "");
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string ModulAra(XmlNodeList dugumler, string sayfaKod, string modulKod)
        {
            foreach (XmlNode d in dugumler)
            {
                if (d.NodeType != XmlNodeType.Element) continue;
                string modul = modulKod;
                XmlAttribute mdl = d.Attributes["Modul"];
                if (mdl != null) modul = mdl.Value;
                XmlAttribute url = d.Attributes["URL"];
                if (url != null && url.Value.ToLowerInvariant().IndexOf(sayfaKod, StringComparison.Ordinal) >= 0) return modul;
                if (d.HasChildNodes)
                {
                    string bulunan = ModulAra(d.ChildNodes, sayfaKod, modul);
                    if (bulunan != null) return bulunan;
                }
            }
            return null;
        }
    }
}
