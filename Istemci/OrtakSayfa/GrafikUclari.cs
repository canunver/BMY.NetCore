using System;
using System.Collections.Generic;
using Istemci.Oturum;
using Istemci.StratejikPlan;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>Bir grafik serisi: etiketlerle aynı sırada değerler.</summary>
    public class GrafikSeri
    {
        public string ad { get; set; }
        public string renk { get; set; }
        public List<double> degerler { get; set; } = new List<double>();
    }

    /// <summary>
    /// Tarayıcıda çizilen grafiğin verisi (wwwroot/js/ortak.js: Sgb.grafik). Eski sayfalar Ext JS grafik tanımı (TNSChart) üretiyordu;
    /// yeni sayfalarda grafik kütüphanesinden bağımsız bu sade veri kullanılır.
    ///   tur: "pasta" (tek seri; dilim renkleri renkler), "cizgi", "sutun"
    /// </summary>
    public class GrafikVeri
    {
        public string baslik { get; set; }
        public string tur { get; set; }
        public List<string> etiketler { get; set; } = new List<string>();
        public List<GrafikSeri> seriler { get; set; } = new List<GrafikSeri>();
        public List<string> renkler { get; set; } = new List<string>();
    }

    /// <summary>
    /// Grafik verisi ucu (eski OrtakSayfa/GrafikGoster.aspx):
    ///   GET /api/grafik?param=grafikNo-...   (param eski sayfadakiyle aynıdır; ilk parça grafiği üreten sınıfı seçer)
    /// Eski sayfa grafikNo'ya göre ilgili modülün IChartGenerator sınıfını çalıştırıyordu. Burada da aynı numaralar kullanılır;
    /// her modülün üreticisi o modül taşındıkça Ureticiler listesine eklenir. Şu an taşınan: 1 (PerformansButce.GrafikDataUret, sp1 ve sp2).
    /// </summary>
    public static class GrafikUclari
    {
        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/grafik", Grafik);
        }

        /// <summary>grafikNo -> üretici. Üretici, tanımadığı grafik türü için null döndürür.</summary>
        private static readonly Dictionary<string, Func<Kullanici, string[], string, GrafikVeri>> Ureticiler = new Dictionary<string, Func<Kullanici, string[], string, GrafikVeri>>
        {
            { "1", PerformansButceGrafigi }
        };

        private static GrafikVeri PerformansButceGrafigi(Kullanici kullanan, string[] paramParca, string uygulamaKlasoru)
        {
            // Stratejik plan grafikleri, onları gösteren iki sayfadan (Kurum Değerlendirme, İzleme Tablosu) birini kullanabilenlere açıktır.
            if (!KurumIzlemeUclari.YetkiVar(kullanan, uygulamaKlasoru) && !IzlemeUclari.YetkiVar(kullanan, uygulamaKlasoru))
                throw new UnauthorizedAccessException();
            return new PerformansButce.GrafikDataUret().Uret(kullanan, paramParca);
        }

        private static IResult Grafik(HttpContext baglam, IWebHostEnvironment ortam, string param)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (string.IsNullOrWhiteSpace(param)) return Results.BadRequest();

            string[] paramParca = param.Split('-');
            if (!Ureticiler.TryGetValue(paramParca[0], out Func<Kullanici, string[], string, GrafikVeri> uretici))
                return Results.Json(new { hata = "Bu grafik henüz taşınmadı." });
            try
            {
                GrafikVeri veri = uretici(kullanan, paramParca, ortam.ContentRootPath);
                if (veri == null) return Results.Json(new { hata = "Bu grafik henüz taşınmadı." });
                return Results.Json(veri);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Grafik verileri okunamadı." });
            }
        }
    }
}
