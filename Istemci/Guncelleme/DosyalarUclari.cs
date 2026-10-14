using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS.KYM;

namespace Istemci.Guncelleme
{
    public class DosyaIstegi
    {
        /// <summary>Klasörün tam yolu; boşsa uygulama klasörü, göreli yol uygulama klasörüne göre.</summary>
        public string Dir { get; set; }
        /// <summary>Dosya ya da yeni klasör adı.</summary>
        public string Ad { get; set; }
        /// <summary>Tarih değiştirmede: yyyy-MM-dd</summary>
        public string Tarih { get; set; }
    }

    /// <summary>
    /// Dosyalar sayfasının (eski YazilimGuncelle/Dosyalar.aspx) uçları. Uygulama klasöründeki klasörler ve dosyalar listelenir,
    /// dosya indirilir / silinir / yüklenir, klasör oluşturulur, dosya tarihleri değiştirilir. Yalnızca "stratek" kullanıcısı.
    ///   GET  /api/guncelleme/dosyalar?dir=, /dosya-indir?dir=&amp;ad=
    ///   POST /api/guncelleme/dosya-sil, /klasor-olustur, /tarih-degistir, /dosya-yukle (multipart: dir, dosya)
    /// Eski sayfadaki gibi sunucudaki her klasöre gidilebilir (Path alanına yazarak ya da Üst Klasör ile); klasör tam yol olarak
    /// her istekte gelir.
    /// </summary>
    public static class DosyalarUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/guncelleme/dosyalar", Liste);
            app.MapGet("/api/guncelleme/dosya-indir", Indir);
            app.MapPost("/api/guncelleme/dosya-sil", Sil);
            app.MapPost("/api/guncelleme/klasor-olustur", KlasorOlustur);
            app.MapPost("/api/guncelleme/tarih-degistir", TarihDegistir);
            app.MapPost("/api/guncelleme/dosya-yukle", Yukle).DisableAntiforgery();
        }

        private static IResult Denetle(HttpContext baglam, out Kullanici kullanan)
        {
            return GuncellemeOrtak.Denetle(baglam, GuncellemeOrtak.DosyalarYetkili, out kullanan);
        }

        /// <summary>
        /// İstekteki klasörün tam yolu: boşsa uygulama klasörü, göreli ise uygulama klasörüne göre. "C:" gibi yalnız sürücü adı
        /// verilirse sürücünün kökü alınır. Yol geçersizse null.
        /// </summary>
        private static string KlasorBul(IWebHostEnvironment ortam, string dir)
        {
            string kok = GuncellemeOrtak.KokBul(ortam.ContentRootPath);
            string d = (dir ?? "").Trim().Trim('"');
            if (d == "") return kok;
            if (d.Length == 2 && d[1] == ':') d += Path.DirectorySeparatorChar;
            try
            {
                string tam = Path.GetFullPath(Path.IsPathRooted(d) ? d : Path.Combine(kok, d));
                string surucu = Path.GetPathRoot(tam);
                return tam.Length > (surucu ?? "").Length ? tam.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) : tam;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string Zaman(DateTime t)
        {
            return t.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string dir)
        {
            IResult ret = Denetle(baglam, out _);
            if (ret != null) return ret;

            string klasor = KlasorBul(ortam, dir);
            if (klasor == null || !Directory.Exists(klasor)) return GuncellemeOrtak.Hata("Klasör bulunamadı: " + dir);

            List<object> satirlar = new List<object>();
            string hata = null;
            try
            {
                foreach (DirectoryInfo d in new DirectoryInfo(klasor).GetDirectories().OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
                    satirlar.Add(new { tur = "k", ad = d.Name, yol = d.FullName, boyut = (long?)null, tarih = Zaman(d.LastWriteTime) });
                // Dosyalar eskisi gibi en son değişen başta.
                foreach (FileInfo f in new DirectoryInfo(klasor).GetFiles().OrderByDescending(x => x.LastWriteTime))
                    satirlar.Add(new { tur = "d", ad = f.Name, yol = (string)null, boyut = (long?)f.Length, tarih = Zaman(f.LastWriteTime) });
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            // Sürücü kökünde üst klasör yok.
            string ust = new DirectoryInfo(klasor).Parent?.FullName;
            return Results.Json(new { basarili = hata == null, hata = hata, yol = klasor, dir = klasor, ust = ust, satirlar = satirlar });
        }

        /// <summary>Klasördeki bir dosyanın tam yolu; ad ya da klasör geçersizse null.</summary>
        private static string DosyaYolu(IWebHostEnvironment ortam, string dir, string ad)
        {
            if (!GuncellemeOrtak.AdGecerli(ad)) return null;
            string klasor = KlasorBul(ortam, dir);
            return klasor == null ? null : Path.Combine(klasor, ad);
        }

        private static IResult Indir(HttpContext baglam, IWebHostEnvironment ortam, string dir, string ad)
        {
            IResult ret = Denetle(baglam, out _);
            if (ret != null) return ret;
            string dosya = DosyaYolu(ortam, dir, ad);
            if (dosya == null || !File.Exists(dosya)) return Results.NotFound();
            return Results.File(dosya, "application/octet-stream", ad);
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, DosyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out _);
            if (ret != null) return ret;
            string dosya = DosyaYolu(ortam, istek?.Dir, istek?.Ad);
            if (dosya == null || !File.Exists(dosya)) return GuncellemeOrtak.Hata("Dosya bulunamadı.");
            try
            {
                File.Delete(dosya);
                return Results.Json(new { basarili = true, bilgi = "Dosya silindi." });
            }
            catch (Exception ex)
            {
                TNS.OrtakFonksiyonlar.HataDosyaYaz("FileBrowser.txt", "Hata:" + ex.Message, false);
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        private static async Task<IResult> KlasorOlustur(HttpContext baglam, IWebHostEnvironment ortam, DosyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out _);
            if (ret != null) return ret;
            string ad = (istek?.Ad ?? "").Trim();
            if (ad == "") return GuncellemeOrtak.Hata("Klasör adını yazınız.");
            string yeni = DosyaYolu(ortam, istek?.Dir, ad);
            if (yeni == null) return GuncellemeOrtak.Hata("Klasör adı geçersiz.");
            try
            {
                Directory.CreateDirectory(yeni);
                return Results.Json(new { basarili = true, bilgi = "Klasör oluşturuldu." });
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            GuncellemeOrtak.BuyukGovdeyeIzinVer(baglam);
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out _);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();

            IFormCollection form = await baglam.Request.ReadFormAsync(GuncellemeOrtak.BuyukForm());
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return GuncellemeOrtak.Hata("Dosya seçiniz.");
            string hedef = DosyaYolu(ortam, form["dir"], Path.GetFileName(dosya.FileName));
            if (hedef == null) return GuncellemeOrtak.Hata("Dosya adı geçersiz.");
            try
            {
                using (FileStream yaz = File.Create(hedef))
                    await dosya.CopyToAsync(yaz);
                return Results.Json(new { basarili = true, bilgi = "Dosya yüklendi." });
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        /// <summary>
        /// Eski btnTarihYaz_Click: bulunulan klasör ve alt klasörlerindeki, oluşturulma tarihi verilen tarihten sonra olan dosyaların
        /// oluşturulma, değiştirilme ve erişim tarihleri verilen tarihe çekilir.
        /// </summary>
        private static async Task<IResult> TarihDegistir(HttpContext baglam, IWebHostEnvironment ortam, DosyaIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, out _);
            if (ret != null) return ret;
            if (!DateTime.TryParseExact(istek?.Tarih ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tarih))
                return GuncellemeOrtak.Hata("Tarihi giriniz.");
            string klasor = KlasorBul(ortam, istek.Dir);
            if (klasor == null || !Directory.Exists(klasor)) return GuncellemeOrtak.Hata("Klasör bulunamadı.");
            try
            {
                DosyaTarihleriDegistir(klasor, tarih);
                return Results.Json(new { basarili = true, bilgi = "Dosya tarihleri değiştirildi." });
            }
            catch (Exception ex)
            {
                return GuncellemeOrtak.Hata(ex.Message);
            }
        }

        private static void DosyaTarihleriDegistir(string araDir, DateTime dt)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(araDir);
            try
            {
                foreach (DirectoryInfo d in dirInfo.GetDirectories())
                    DosyaTarihleriDegistir(d.FullName, dt);
            }
            catch { }

            foreach (FileInfo f in dirInfo.GetFiles())
                DosyaTarihDegistir(f, dt);
        }

        private static void DosyaTarihDegistir(FileInfo f, DateTime dt)
        {
            try
            {
                if (File.GetCreationTime(f.FullName).CompareTo(dt) <= 0) return;

                FileAttributes attributes = File.GetAttributes(f.FullName);
                File.SetAttributes(f.FullName, attributes & ~FileAttributes.ReadOnly);

                File.SetCreationTime(f.FullName, dt);
                File.SetLastWriteTime(f.FullName, dt);
                File.SetLastAccessTime(f.FullName, dt);
            }
            catch (Exception ex)
            {
                TNS.OrtakFonksiyonlar.HataStrYaz(ex.Message);
            }
        }
    }
}
