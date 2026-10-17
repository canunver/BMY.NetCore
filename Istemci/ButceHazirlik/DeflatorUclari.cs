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
using TNS.BHMv2;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class DeflatorSatir
    {
        public string SektorKod { get; set; }
        public double Deflator { get; set; }
    }

    public class DeflatorKayitIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public List<DeflatorSatir> Satirlar { get; set; }
    }

    /// <summary>
    /// Yatırım Projeleri Deflatörü (eski ButceHazirlikv2/DeflatorForm.aspx) uçları:
    ///   GET  /api/deflator/tanim, /liste?yil=&amp;senaryo=
    ///   POST /api/deflator/kaydet
    /// Eskisi gibi YAT servisinin ListeleDeflator / KaydetDeflator yordamlarıyla; kayıt yılın ve aşamanın deflatörlerini silip
    /// sıfır olmayanları yazar, yazma yetkisini (sistem yöneticisi) iş kuralı denetler.
    /// </summary>
    public static class DeflatorUclari
    {
        public const string Adres = "../ButceHazirlikv2/DeflatorForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/deflator/tanim", Tanim);
            app.MapGet("/api/deflator/liste", Liste);
            app.MapPost("/api/deflator/kaydet", Kaydet);
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

        private static bool SenaryoGecerli(string senaryo)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(senaryo ?? "", @"^[0-9]{2}\.[0-9]{2}$");
        }

        /// <summary>Açılış (eski Page_Load): yıl bütçe hazırlık yılı (HazirlikYiliBul); aşamalar "kod- ad", ilki seçili.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<object> asamalar = new List<object>();
                ButceSenaryo[] senaryolar = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null);
                if (senaryolar != null)
                    foreach (ButceSenaryo bs in senaryolar)
                        asamalar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                return Results.Json(new
                {
                    yil = ButceKod.KodTanimUclari.YilBul(null),
                    asamalar = asamalar,
                    yazabilir = kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)
                });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Listele (eski Oku): yılın ve aşamanın sektör deflatörleri.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 1000) return YatProjeOrtak.Hata("Yıl hatalı!");
            if (!SenaryoGecerli(senaryo)) return YatProjeOrtak.Hata("Senaryo seçiniz.");
            try
            {
                ObjectArray oa = TNS.YAT.Arac.Tanimla().ListeleDeflator(kullanan, yil, senaryo, "");
                if (!oa.sonuc.islemSonuc) return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(oa.sonuc.hataStr, "Deflatörler okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (object o in oa.objeler)
                    if (o is Deflator d)
                        satirlar.Add(new { sektorKod = d.sektorKod, sektorAd = d.sektorAd, deflator = d.deflator });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Deflatörler okunamadı.");
            }
        }

        /// <summary>Kaydet (eski Kaydet: grdDeflator.getRowsValues): tablodaki bütün satırlar.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, DeflatorKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil < 1000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (!SenaryoGecerli(istek.Senaryo)) return Results.Json(new { basarili = false, hata = "Senaryo seçiniz." });
            try
            {
                TNSCollection deflatorler = new TNSCollection();
                if (istek.Satirlar != null)
                    foreach (DeflatorSatir s in istek.Satirlar)
                    {
                        if (s == null || string.IsNullOrWhiteSpace(s.SektorKod)) continue;
                        deflatorler.Add(new Deflator { sektorKod = s.SektorKod.Trim(), deflator = Math.Round(s.Deflator, 4) });
                    }
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().KaydetDeflator(kullanan, istek.Yil, istek.Senaryo, deflatorler);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(sonuc.hataStr, "Kayıt yapılamadı.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Kayıt yapılamadı.") });
            }
        }
    }
}
