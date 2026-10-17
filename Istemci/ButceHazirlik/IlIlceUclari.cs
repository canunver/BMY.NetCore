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
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class IlKayitIstegi
    {
        public int Kod { get; set; }
        public string Ad { get; set; }
        /// <summary>Düzey 2 bölge kodu (Sozluk.aciklama).</summary>
        public string DuzeyKod { get; set; }
    }

    public class IlceKayitIstegi
    {
        public int IlKod { get; set; }
        public int Kod { get; set; }
        public string Ad { get; set; }
    }

    /// <summary>
    /// İl ve İlçe Tanımı sayfasının (eski ButceHazirlikv2/IlIlceTanimla.aspx ve içinde açılan IlTanimlaDetay.aspx,
    /// IlceTanimlaDetay.aspx) uçları:
    ///   GET  /api/ililce/iller, /ilceler?il=
    ///   POST /api/ililce/il-kaydet, /il-sil, /ilce-kaydet, /ilce-sil
    /// İşlemler eskisi gibi TNS.YAT.Arac.Tanimla() üzerinden yapılır; kaydetme ve silmeye iş kuralları yalnızca sözlük yazma hakkı
    /// olana izin verir. Sayfayı menüde bu seçeneği görebilenler kullanır.
    /// </summary>
    public static class IlIlceUclari
    {
        public const string Adres = "../ButceHazirlikv2/IlIlceTanimla.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/ililce/iller", Iller);
            app.MapGet("/api/ililce/ilceler", Ilceler);
            app.MapPost("/api/ililce/il-kaydet", IlKaydet);
            app.MapPost("/api/ililce/il-sil", IlSil);
            app.MapPost("/api/ililce/ilce-kaydet", IlceKaydet);
            app.MapPost("/api/ililce/ilce-sil", IlceSil);
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static async Task<(IResult hata, Kullanici kullanan)> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return (Results.BadRequest(), null);
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            return (ret, kullanan);
        }

        private static IResult SonucYaz(Sonuc s, string basariMetni, string varsayilanHata)
        {
            if (s == null || !s.islemSonuc)
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(s == null ? null : s.hataStr, varsayilanHata) });
            return Results.Json(new { basarili = true, bilgi = basariMetni });
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>İller (eski GridDoldur'un ilk düzeyi): kod, ad, düzey 2 bölge kodu. Ağaçta her ilin altı açılabilir.</summary>
        private static IResult Iller(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                BilgiVAN bv = TNS.YAT.Arac.Tanimla().IlListele(kullanan, new Sozluk());
                if (bv == null || !bv.sonuc.islemSonuc)
                    return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(bv == null ? null : bv.sonuc.hataStr, "İller okunamadı."));
                List<object> iller = new List<object>();
                foreach (Sozluk il in bv.liste)
                    iller.Add(new { kod = il.kod, ad = il.ad, duzeyKod = il.aciklama ?? "" });
                return Results.Json(new { iller = iller });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("İller okunamadı.");
            }
        }

        /// <summary>Bir ilin ilçeleri (eski GridDoldur'un ikinci düzeyi).</summary>
        private static IResult Ilceler(HttpContext baglam, IWebHostEnvironment ortam, int il)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (il <= 0) return YatProjeOrtak.Hata("İl seçilmedi.");
            try
            {
                IlceSozluk kistas = new IlceSozluk();
                kistas.il.kod = il;
                BilgiVAN bv = TNS.YAT.Arac.Tanimla().IlceListele(kullanan, kistas);
                if (bv == null || !bv.sonuc.islemSonuc)
                    return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(bv == null ? null : bv.sonuc.hataStr, "İlçeler okunamadı."));
                List<object> ilceler = new List<object>();
                foreach (IlceSozluk ilce in bv.liste)
                    ilceler.Add(new { kod = ilce.ilceKodu, ad = ilce.ilceAdi, ilKod = il });
                return Results.Json(new { ilceler = ilceler });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("İlçeler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>Eski IlTanimlaDetay.Kaydet: kod varsa güncellenir, yoksa eklenir (iş kuralı).</summary>
        private static async Task<IResult> IlKaydet(HttpContext baglam, IWebHostEnvironment ortam, IlKayitIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                Sozluk il = new Sozluk(istek.Kod, YatProjeOrtak.Temiz(istek.Ad), YatProjeOrtak.Temiz(istek.DuzeyKod));
                return SonucYaz(TNS.YAT.Arac.Tanimla().IlKaydet(kullanan, il), "Kayıt işlemi başarıyla tamamlandı.", "Kayıt işlemi başarısız.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Kayıt işlemi başarısız.") });
            }
        }

        /// <summary>Eski IlTanimlaDetay.btnSil_Click: projelerde kullanılmış il silinemez (iş kuralı).</summary>
        private static async Task<IResult> IlSil(HttpContext baglam, IWebHostEnvironment ortam, IlKayitIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                Sozluk il = new Sozluk(istek.Kod, YatProjeOrtak.Temiz(istek.Ad), YatProjeOrtak.Temiz(istek.DuzeyKod));
                return SonucYaz(TNS.YAT.Arac.Tanimla().IlSil(kullanan, il), "Silme işlemi başarıyla tamamlandı.", "Silme işlemi başarısız.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Silme işlemi başarısız.") });
            }
        }

        /// <summary>Eski IlceTanimlaDetay.Kaydet: ilçe seçili ilin altına kaydedilir.</summary>
        private static async Task<IResult> IlceKaydet(HttpContext baglam, IWebHostEnvironment ortam, IlceKayitIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            if (istek.IlKod <= 0) return Results.Json(new { basarili = false, hata = "Yeni ilçe kaydedebilmek için lütfen il oluşturunuz" });
            try
            {
                IlceSozluk ilce = new IlceSozluk(new Sozluk(istek.IlKod, string.Empty), istek.Kod, YatProjeOrtak.Temiz(istek.Ad));
                return SonucYaz(TNS.YAT.Arac.Tanimla().IlceKaydet(kullanan, ilce), "Kayıt işlemi başarıyla tamamlandı.", "Kayıt işlemi başarısız.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Kayıt işlemi başarısız.") });
            }
        }

        private static async Task<IResult> IlceSil(HttpContext baglam, IWebHostEnvironment ortam, IlceKayitIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await DenetleYaz(baglam, ortam, sahtecilik);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                IlceSozluk ilce = new IlceSozluk(new Sozluk(istek.IlKod, string.Empty), istek.Kod, YatProjeOrtak.Temiz(istek.Ad));
                return SonucYaz(TNS.YAT.Arac.Tanimla().IlceSil(kullanan, ilce), "Silme işlemi başarıyla tamamlandı.", "Silme işlemi başarısız.");
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Silme işlemi başarısız.") });
            }
        }
    }
}
