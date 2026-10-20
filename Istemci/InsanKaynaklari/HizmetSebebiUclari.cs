using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.IKY;
using TNS.KYM;

namespace Istemci.InsanKaynaklari
{
    /// <summary>Kaydet isteği: alan adları HizmetSebebi sınıfındaki gibi.</summary>
    public class HizmetSebebiIstegi
    {
        public int kod { get; set; }
        public string sebepAdi { get; set; }
        public int tip { get; set; }
    }

    /// <summary>Sil isteği: silinecek hizmet sebeplerinin kodları.</summary>
    public class HizmetSebebiSilIstegi
    {
        public List<int> kodlar { get; set; }
    }

    /// <summary>
    /// Hizmet Sebepleri (eski InsanKaynaklari/HizmetSebebiForm.aspx; yeni sayfa InsanKaynaklari/HizmetSebebiForm) uçları:
    ///   GET  /api/hizmetsebebi/tipler                tip seçenekleri (eski stoTip)
    ///   GET  /api/hizmetsebebi/liste?sebepAdi=&amp;tip=   (eski Listele)
    ///   POST /api/hizmetsebebi/kaydet                (eski Kaydet)
    ///   POST /api/hizmetsebebi/sil                   seçili satırlar (eski Sil)
    /// İş kuralları eski sayfadaki gibi IIKYServis.ListeleHizmetSebebi / KaydetHizmetSebebi / SilHizmetSebebleri.
    /// </summary>
    public static class HizmetSebebiUclari
    {
        public const string SayfaAdresi = "../InsanKaynaklari/HizmetSebebiForm.aspx";

        // Eski Page_Load'daki stoTip sırası
        private static readonly ENUMHizmetSebebiNevi[] TIPLER =
        {
            ENUMHizmetSebebiNevi.TANIMSIZ,
            ENUMHizmetSebebiNevi.BASLAMA,
            ENUMHizmetSebebiNevi.AYRILMA,
            ENUMHizmetSebebiNevi.TERFI,
            ENUMHizmetSebebiNevi.NAKILBASLAMA,
            ENUMHizmetSebebiNevi.NAKILAYRILMA,
            ENUMHizmetSebebiNevi.UCRETSIZAYRILMA,
            ENUMHizmetSebebiNevi.UCRETSIZIZIN,
            ENUMHizmetSebebiNevi.GOREVDENUZAK,
            ENUMHizmetSebebiNevi.GOREVUZAKAYRILMA,
            ENUMHizmetSebebiNevi.YURTDISIEGITIM,
            ENUMHizmetSebebiNevi.YURTDISIEGITIMEAYRILMA
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/hizmetsebebi/tipler", Tipler);
            app.MapGet("/api/hizmetsebebi/liste", Liste);
            app.MapPost("/api/hizmetsebebi/kaydet", Kaydet);
            app.MapPost("/api/hizmetsebebi/sil", Sil);
        }

        /// <summary>Eski Page_Load: GenelIslemler.SayfayaGirisYapabilirMi (menü yetkisi).</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Tipler(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            List<object> tipler = new List<object>();
            foreach (ENUMHizmetSebebiNevi t in TIPLER)
                tipler.Add(new { kod = (int)t, ad = HizmetSebebi.TipAd(t) });
            return Results.Json(new { tipler = tipler });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string sebepAdi, int? tip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            HizmetSebebi form = new HizmetSebebi();
            form.sebepAdi = sebepAdi ?? "";
            form.tip = (ENUMHizmetSebebiNevi)(tip ?? 0);
            ObjectArray oa = TNS.IKY.Arac.Tanimla().ListeleHizmetSebebi(kullanan, form);
            if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(oa.sonuc, "Hizmet sebepleri okunamadı.") });

            List<object> satirlar = new List<object>();
            foreach (HizmetSebebi h in oa.objeler)
                satirlar.Add(new { h.kod, h.sebepAdi, tip = (int)h.tip, h.tipAd });
            return Results.Json(new { satirlar = satirlar });
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, HizmetSebebiIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            HizmetSebebi form = new HizmetSebebi();
            form.kod = istek.kod;
            form.sebepAdi = istek.sebepAdi ?? "";
            form.tip = (ENUMHizmetSebebiNevi)istek.tip;

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().KaydetHizmetSebebi(kullanan, form);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc, "Hizmet sebebi kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", kod = form.kod });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, HizmetSebebiSilIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.kodlar == null || istek.kodlar.Count == 0) return Results.BadRequest();

            TNSCollection formlar = new TNSCollection();
            foreach (int kod in istek.kodlar)
            {
                HizmetSebebi form = new HizmetSebebi();
                form.kod = kod;
                formlar.Add(form);
            }

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().SilHizmetSebebleri(kullanan, formlar);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc, "Hizmet sebepleri silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = (sonuc.hataStr ?? "").Replace("<br>", " ").Replace("<br/>", " ");
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata.Trim();
        }
    }
}
