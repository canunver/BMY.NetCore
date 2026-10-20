using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>
    /// Sistem Dosyaları Sürüm Bilgisi (eski OrtakSayfa/SurumBilgi.aspx): uygulamanın çalıştığı klasördeki dll (ve exe)
    /// dosyalarını ad, tarih, sürüm, üretici firma ve büyüklükle listeler.
    ///   GET /api/surumbilgi/liste?sgb=true|false → { klasor, satirlar: [{ ad, tarih, tarihStr, surum, urunSurum, firma, buyukluk }] }
    /// sgb=true: eski "Sadece SGB.net dll dosyalarını göster" seçeneği (üretici firma "SGB.net" ile başlayanlar).
    /// Yetki: menüdeki seçenek (Yetki="1", sistem yöneticisi).
    /// </summary>
    public static class SurumBilgiUclari
    {
        public const string Adres = "../OrtakSayfa/SurumBilgi.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/surumbilgi/liste", Liste);
        }

        public static bool Yetkili(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, bool? sgb)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);

            // Eski sayfa çalışan derlemenin klasörünü (bin) okuyordu; Core'da bu, uygulamanın çalıştığı klasör (Istemci.dll de burada).
            string anaYol = AppContext.BaseDirectory;
            List<object> satirlar = new List<object>();
            DirectoryInfo klasor = new DirectoryInfo(anaYol);
            if (klasor.Exists)
            {
                foreach (FileInfo f in klasor.GetFiles())
                {
                    string uzanti = f.Extension.ToLowerInvariant();
                    if (uzanti != ".dll" && uzanti != ".exe") continue;

                    string surum = "", urunSurum = "", firma = "";
                    try
                    {
                        FileVersionInfo bilgi = FileVersionInfo.GetVersionInfo(f.FullName);
                        surum = bilgi.FileVersion ?? "";
                        urunSurum = bilgi.ProductVersion ?? "";
                        firma = bilgi.CompanyName ?? "";
                    }
                    catch (Exception) { }

                    if (sgb == true && !firma.StartsWith("SGB.net", StringComparison.OrdinalIgnoreCase)) continue;

                    // Ürün sürümünün "+commit" eki gösterilmez.
                    int arti = urunSurum.IndexOf('+');
                    if (arti > 0) urunSurum = urunSurum.Substring(0, arti);

                    satirlar.Add(new
                    {
                        ad = f.Name,
                        tarih = f.LastWriteTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        tarihStr = f.LastWriteTime.ToString("dd.MM.yyyy HH:mm"),
                        surum,
                        urunSurum,
                        firma,
                        buyukluk = f.Length
                    });
                }
            }
            return Results.Json(new { klasor = anaYol, satirlar });
        }
    }
}
