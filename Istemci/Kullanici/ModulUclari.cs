using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS.KYM;

namespace Istemci.Kullanicilar
{
    public class ModulDurum
    {
        public string KisaAd { get; set; }
        public string Durum { get; set; }
    }

    public class ModulKaydetIstegi
    {
        public List<ModulDurum> Moduller { get; set; }
    }

    /// <summary>
    /// Modüller (eski KYM/Moduller.aspx): sistemde kullanılan modüllerin durumu. Durum sistem değişkeni /MODUL/{kısa ad}'a yazılır
    /// (0: Kullanım Dışı, 1: Kullanımda, 2: Test Amaçlı). Menüde Modul niteliği olan seçenekler yalnızca modülü 1 ya da 2 olanlara
    /// görünür (MenuOkuyucu.ModulAcik); tarayıcıda saklanan menü sürümü değiştiği için (MenuSurum) bütün kullanıcıların menüsü
    /// bir sonraki sayfada yenilenir.
    ///   GET  /api/moduller/liste                          → { satirlar: [{ kisaAd, ad, durum }] }
    ///   POST /api/moduller/kaydet { moduller: [{ kisaAd, durum }] } → { basarili }   (yalnızca değişen satırlar gelir)
    /// Yetki: menüdeki seçenek (Yetki="1", sistem yöneticisi).
    /// </summary>
    public static class ModulUclari
    {
        public const string Adres = "../KYM/Moduller.aspx";
        private static readonly string[] durumlar = { "0", "1", "2" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/moduller/liste", Liste);
            app.MapPost("/api/moduller/kaydet", Kaydet);
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

            // Eski Listele: değişken servisinden doğrudan okunur.
            TNS.DEG.IDEGServis servisDEG = TNS.DEG.Arac.Tanimla();
            List<object> satirlar = new List<object>();
            foreach (string modulKisaAd in TNS.KYM.Arac.moduller)
                satirlar.Add(new
                {
                    kisaAd = modulKisaAd,
                    ad = TNS.KYM.Arac.ModulAdBul(modulKisaAd),
                    durum = (servisDEG.DegiskenDegerBul(0, "/MODUL/" + modulKisaAd) ?? "").Trim()
                });
            return Results.Json(new { satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, ModulKaydetIstegi istek, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Moduller == null) return Results.BadRequest();

            foreach (ModulDurum m in istek.Moduller)
            {
                string kisaAd = (m?.KisaAd ?? "").Trim();
                string durum = (m?.Durum ?? "").Trim();
                if (Array.IndexOf(TNS.KYM.Arac.moduller, kisaAd) < 0 || Array.IndexOf(durumlar, durum) < 0)
                    return Results.Json(new { basarili = false, hata = "Geçersiz modül ya da durum: " + kisaAd });
            }

            // Eski btnKaydet_Click: DegiskenDegerAta(0, "/MODUL/" + kısa ad, durum). Arac üzerinden yazılır ki önbellek de temizlensin.
            foreach (ModulDurum m in istek.Moduller)
                TNS.DEG.Arac.DegiskenDegerAta(0, "/MODUL/" + m.KisaAd.Trim(), m.Durum.Trim());

            return Results.Json(new { basarili = true });
        }
    }
}
