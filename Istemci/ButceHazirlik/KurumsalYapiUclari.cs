using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;

namespace Istemci.ButceHazirlik
{
    public class KurumsalYapiIstegi
    {
        public string Kod { get; set; }
        public string Ad { get; set; }
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public string KurKod { get; set; }
    }

    /// <summary>
    /// Eski Kurumsal Yapı Tanımlama sayfasının (eski ButceHazirlikv2/TanimKurumsalYapi.aspx) uçları:
    ///   GET  /api/kurumsalyapi/tanim                 kurumsal kodlar (eski ddlKur: GenelIslemlerIstemci.DDLBirimDoldur)
    ///   GET  /api/kurumsalyapi/liste?kod=&amp;ad=&amp;muhasebe=&amp;birim=&amp;kurKod=   (eski btnAra_Click: formdaki değerler ölçüttür)
    ///   POST /api/kurumsalyapi/kaydet
    /// Eski sayfadaki Sil tuşunun kodu kapatılmıştı (hiçbir şey yapmıyordu); alınmadı.
    /// </summary>
    public static class KurumsalYapiUclari
    {
        public const string SayfaAdresi = ButceTanimOrtak.Klasor + "TanimKurumsalYapi.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/kurumsalyapi/tanim", Tanim);
            app.MapGet("/api/kurumsalyapi/liste", Liste);
            app.MapPost("/api/kurumsalyapi/kaydet", Kaydet);
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                List<object> kurumlar = new List<object>();
                ABSKodlarVAN akv = TNS.BKD.Arac.Tanimla().BirimListele(kullanan, DateTime.Today.Year, TNS.DEG.Arac.VarsayilanKurumBul(), true);
                if (akv != null && akv.sonuc.islemSonuc && akv.absKodlar != null)
                    foreach (ABSKod k in akv.absKodlar)
                        kurumlar.Add(new { kod = k.kod, ad = k.kod + " / " + k.Ad(kullanan.aktifKultur) });
                return Results.Json(new { kurumlar = kurumlar });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Kurumsal kodlar okunamadı.");
            }
        }

        private static KurumsalYapi Ol(string kod, string ad, int muhasebe, string birim, string kurKod)
        {
            KurumsalYapi ky = new KurumsalYapi();
            ky.kod = ButceTanimOrtak.Temiz(kod);
            ky.ad = ButceTanimOrtak.Temiz(ad);
            ky.saymanlikKod = muhasebe;
            ky.birimKod = ButceTanimOrtak.Temiz(birim);
            ky.kurKod = ButceTanimOrtak.Temiz(kurKod);
            return ky;
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string kod, string ad, int? muhasebe, string birim, string kurKod)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                KurumsalYapi[] kyler = TNS.BHMv2.Arac.Tanimla().KurumsalYapiListele(kullanan, Ol(kod, ad, muhasebe ?? 0, birim, kurKod), false, false)
                    ?? new KurumsalYapi[0];
                List<object> satirlar = new List<object>();
                foreach (KurumsalYapi k in kyler)
                    satirlar.Add(new { kod = k.kod ?? "", ad = k.ad ?? "", muhasebe = k.saymanlikKod, birim = k.birimKod ?? "", kurKod = k.kurKod ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Liste okunamadı.");
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click); başarılıysa kaydedilen yapı listelenir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, KurumsalYapiIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KurumsalYapiKaydet(kullanan, Ol(istek.Kod, istek.Ad, istek.Muhasebe, istek.Birim, istek.KurKod));
                return ButceTanimOrtak.SonucYaz(sonuc, "Kayıt işlemi başarıyla tamamlandı.", "Kayıt işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kayıt işlemi başarısız.");
            }
        }
    }
}
