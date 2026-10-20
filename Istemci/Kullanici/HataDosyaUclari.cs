using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
    public class HataDosyaSilIstegi
    {
        public string Ad { get; set; }
    }

    /// <summary>
    /// Sistem Durum Dosya Listesi (eski KYM/HataDosya.aspx): App.config HataDosyasi klasöründeki dosyaları listeler, açar ve siler.
    ///   GET  /api/hatadosya/liste       → { satirlar: [{ ad, tarih (yyyy-MM-ddTHH:mm:ss), tarihStr, buyukluk, buyuklukStr }] }
    ///   GET  /api/hatadosya/ac?ad=      → dosyanın kendisi (metin dosyaları tarayıcıda açılır, diğerleri indirilir)
    ///   POST /api/hatadosya/sil { ad }  → { basarili }
    /// Liste, okuma ve silme iş kurallarındaki HataDosyaListele / HataDosyaVer / HataDosyaSil ile yapılır (eski sayfa gibi).
    /// Dosya adı yol içeremez (eski DosyaGonder denetimi; burada silmeye de uygulanır).
    /// Yetki: menüdeki seçenek (Yetki="1", sistem yöneticisi).
    /// </summary>
    public static class HataDosyaUclari
    {
        public const string Adres = "../KYM/HataDosya.aspx";

        /// <summary>Tarayıcıda metin olarak açılan uzantılar; hata dosyaları windows-1254 ile yazılır (OrtakFonksiyonlar.DosyayaYaz).</summary>
        private static readonly string[] metinUzantilari = { ".txt", ".log", ".xml", ".htm", ".html", ".csv", ".json", ".sql", ".config" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/hatadosya/liste", Liste);
            app.MapGet("/api/hatadosya/ac", Ac);
            app.MapPost("/api/hatadosya/sil", Sil);
        }

        public static bool Yetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            ObjectArray<EkliDosyalar> fler = TNS.KYM.Arac.Tanimla().HataDosyaListele(kullanan);
            List<EkliDosyalar> dosyalar = new List<EkliDosyalar>();
            if (fler != null)
                foreach (EkliDosyalar ed in fler.objeler) dosyalar.Add(ed);
            // Eski KiyasYap: en yeni dosya üstte. Sütun başlığıyla başka sıralama tarayıcıda yapılır.
            dosyalar.Sort((a, b) => b.yuklemeTarihiDT.CompareTo(a.yuklemeTarihiDT));

            List<object> satirlar = new List<object>();
            foreach (EkliDosyalar ed in dosyalar)
                satirlar.Add(new
                {
                    ad = ed.dosyaAdi,
                    tarih = ed.yuklemeTarihiDT.ToString("yyyy-MM-ddTHH:mm:ss"),
                    tarihStr = ed.yuklemeTarihi != null ? ed.yuklemeTarihi.ToLongString() : "",
                    buyukluk = ed.buyukluk,
                    buyuklukStr = ed.buyuklukStr
                });
            return Results.Json(new { satirlar });
        }

        /// <summary>Eski DosyaGonder'deki denetim: yalnızca dosya adı (yol ve geçersiz karakter yok).</summary>
        private static bool AdGecerli(string ad)
        {
            if (string.IsNullOrWhiteSpace(ad)) return false;
            if (!string.Equals(ad, Path.GetFileName(ad), StringComparison.OrdinalIgnoreCase)) return false;
            if (ad.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return false;
            return ad != "." && ad != "..";
        }

        private static IResult Ac(HttpContext baglam, IWebHostEnvironment ortam, string ad)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (!AdGecerli(ad)) return Results.BadRequest();

            byte[] dosya = TNS.KYM.Arac.Tanimla().HataDosyaVer(kullanan, ad);
            if (dosya == null || dosya.Length == 0)
                // İş kuralı yaklaşık 4 MB'tan büyük dosyaları okumaz.
                return Results.Text("Dosya okunamadı: dosya boş, çok büyük (4 MB üstü) ya da silinmiş olabilir.", "text/plain; charset=utf-8");

            string uzanti = Path.GetExtension(ad).ToLowerInvariant();
            if (Array.IndexOf(metinUzantilari, uzanti) >= 0)
            {
                // Tarayıcıda düz metin olarak açılır (HTML olarak yorumlanmaz). UTF-8 BOM'lu dosyalar UTF-8, diğerleri windows-1254.
                bool utf8 = dosya.Length >= 3 && dosya[0] == 0xEF && dosya[1] == 0xBB && dosya[2] == 0xBF;
                baglam.Response.Headers["X-Content-Type-Options"] = "nosniff";
                baglam.Response.Headers["Content-Disposition"] = "inline; filename*=UTF-8''" + Uri.EscapeDataString(ad);
                return Results.Bytes(dosya, "text/plain; charset=" + (utf8 ? "utf-8" : "windows-1254"));
            }
            return Results.File(dosya, "application/octet-stream", ad);
        }

        private static async Task<IResult> Sil(HttpContext baglam, HataDosyaSilIstegi istek, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            string ad = istek != null ? (istek.Ad ?? "").Trim() : "";
            if (!AdGecerli(ad)) return Results.Json(new { basarili = false, hata = "Geçersiz dosya adı." });

            TNS.KYM.Arac.Tanimla().HataDosyaSil(kullanan, ad);
            return Results.Json(new { basarili = true });
        }
    }
}
