using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
    /// <summary>Kaydet / Sil / Bul isteği: alan adları IzinTur sınıfındaki gibi.</summary>
    public class IzinTurIstegi
    {
        public int kod { get; set; }
        public string ad { get; set; }
        public int sure { get; set; }
        public int gorulebilirlik { get; set; }
        public int sira { get; set; }
    }

    /// <summary>
    /// İzin Türleri (eski InsanKaynaklari/TanimIzinTur.aspx; yeni sayfa InsanKaynaklari/TanimIzinTur) uçları:
    ///   GET  /api/izintur/liste            tüm izin türleri + görülebilirlik seçenekleri (eski Page_Load / Temizle)
    ///   POST /api/izintur/bul              formdaki değerlere göre süzülmüş liste (eski btnAra_Click)
    ///   POST /api/izintur/kaydet           (eski btnKaydet_Click)
    ///   POST /api/izintur/sil              (eski btnSil_Click)
    ///   GET  /api/izintur/donem?yil=       izin dönemi adı (eski IzinDonemBul; diğer izin ekranları kullanır)
    /// İş kuralları eski sayfadaki gibi IIKYServis.IzinTurListele / IzinTurKaydet / IzinTurSil.
    /// Bul: eski sayfa IzinTurListele'ye ölçüt veriyordu; iş kuralı sonuç boşsa ilk kayıtları oluşturmaya kalktığı için
    /// burada tüm liste okunup aynı ölçütlerle (ad başı, süre, görülebilirlik, sıra) süzülür.
    /// </summary>
    public static class IzinTurUclari
    {
        public const string SayfaAdresi = "../InsanKaynaklari/TanimIzinTur.aspx";

        /// <summary>Eski resx FRMTITG001.. (görülebilirlik seçenekleri).</summary>
        private static readonly string[] Gorulebilirlikler = { "Herkes", "Yetkili Personel" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/izintur/liste", Liste);
            app.MapPost("/api/izintur/bul", Bul);
            app.MapPost("/api/izintur/kaydet", Kaydet);
            app.MapPost("/api/izintur/sil", Sil);
            app.MapGet("/api/izintur/donem", Donem);
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

        private static string GorulebilirlikAd(int g)
        {
            return g >= 1 && g <= Gorulebilirlikler.Length ? Gorulebilirlikler[g - 1] : "";
        }

        private static List<object> Secenekler()
        {
            List<object> l = new List<object>();
            for (int i = 0; i < Gorulebilirlikler.Length; i++)
                l.Add(new { kod = i + 1, ad = Gorulebilirlikler[i] });
            return l;
        }

        private static object Satir(IzinTur d)
        {
            return new
            {
                d.kod,
                d.ad,
                sure = d.sure,
                gorulebilirlik = d.gorulebilirlik,
                gorulebilirlikAd = GorulebilirlikAd(d.gorulebilirlik),
                sira = d.sira
            };
        }

        /// <summary>Eski TABIzinTur.Listele ölçütleri: ad LIKE 'ad%', süre =, görülebilirlik 2 ise 1 olanlar, sıra =.</summary>
        private static bool Uyar(IzinTur d, IzinTurIstegi k)
        {
            string ad = (k.ad ?? "").Trim();
            if (ad != "" && !(d.ad ?? "").StartsWith(ad, true, new CultureInfo("tr-TR"))) return false;
            if (k.sure > 0 && d.sure != k.sure) return false;
            if (k.gorulebilirlik == 2 && d.gorulebilirlik != 1) return false;
            if (k.sira > 0 && d.sira != k.sira) return false;
            return true;
        }

        private static IResult Listele(IzinTurIstegi kriter)
        {
            ObjectArray oa = TNS.IKY.Arac.Tanimla().IzinTurListele(new IzinTur());
            if (oa == null || !oa.sonuc.islemSonuc)
                return Results.Json(new { hata = oa == null ? "İzin türleri okunamadı." : HataMetni(oa.sonuc, "İzin türleri okunamadı."), gorulebilirlikler = Secenekler() });

            List<object> satirlar = new List<object>();
            foreach (IzinTur d in oa.objeler)
                if (kriter == null || Uyar(d, kriter))
                    satirlar.Add(Satir(d));

            // Eski Page_Load: liste boşsa ilk kayıtlar oluşturulamamıştır, Kaydet kapatılır (FRMTIT005).
            if (kriter == null && oa.objeler.Count == 0)
                return Results.Json(new
                {
                    satirlar = satirlar,
                    gorulebilirlikler = Secenekler(),
                    kaydetKapali = true,
                    hata = "İlk kayıtlar oluşturulamadı. Lütfen veri tabanı güncellemelerini aldığınıza emin olun."
                });
            return Results.Json(new { satirlar = satirlar, gorulebilirlikler = Secenekler() });
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Listele(null);
        }

        private static async Task<IResult> Bul(HttpContext baglam, IWebHostEnvironment ortam, IzinTurIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Listele(istek ?? new IzinTurIstegi());
        }

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IzinTurIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();

            TNS.IKY.Arac.izinTipListesi = null;

            IzinTur d = new IzinTur();
            d.kod = istek.kod;
            d.ad = istek.ad ?? "";
            d.sure = istek.sure;
            d.gorulebilirlik = istek.gorulebilirlik > 0 ? istek.gorulebilirlik : 1;
            d.sira = istek.sira;

            Sonuc sonuc = TNS.IKY.Arac.Tanimla().IzinTurKaydet(kullanan, d);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc, "İzin türü kaydedilemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
        }

        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, IzinTurIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.kod <= 0)
                return Results.Json(new { basarili = false, hata = "Lütfen silmek istediğiniz izin türünü seçin." });

            TNS.IKY.Arac.izinTipListesi = null;

            IzinTur d = new IzinTur();
            d.kod = istek.kod;
            Sonuc sonuc = TNS.IKY.Arac.Tanimla().IzinTurSil(kullanan, d);
            if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc, "İzin türü silinemedi.") });
            return Results.Json(new { basarili = true, bilgi = "Silme işlemi başarıyla gerçekleşti." });
        }

        /// <summary>Eski [DirectMethod] IzinDonemBul.</summary>
        private static IResult Donem(HttpContext baglam, int? yil)
        {
            if (OturumBilgisi.KullaniciOku(baglam) == null) return Results.Unauthorized();
            return Results.Json(new { ad = TNS.IKY.IzinArac.DonemAdBul(yil ?? DateTime.Now.Year) });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = Regex.Replace(sonuc.hataStr ?? "", "<[^>]*>", " ");
            hata = Regex.Replace(hata, @"\s+", " ").Trim();
            return hata == "" || hata.Contains("Exception") || (hata.Contains(" at ") && hata.Contains("line ")) ? varsayilan : hata;
        }
    }
}
