using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TNS;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class YatSozlukIstegi
    {
        public int Kod { get; set; }
        public string Ad { get; set; }
    }

    /// <summary>
    /// Proje Türü (eski ButceHazirlikv2/ProjeTurTanimla.aspx + çerçevedeki ProjeTurTanimlaDetay.aspx) ve Döküman Türü (eski
    /// DokumanTurTanimla.aspx + DokumanTurTanimlaDetay.aspx) sayfalarının uçları. İkisi de kod - ad sözlüğüdür; {tur}: projetur | dokumantur.
    ///   GET  /api/yatsozluk/{tur}/liste
    ///   POST /api/yatsozluk/{tur}/kaydet, /api/yatsozluk/{tur}/sil
    /// Kod elle girilir; aynı kodla kaydetmek günceller (iş kuralı). İşlemler eskisi gibi TNS.YAT.Arac.Tanimla() üzerinden yapılır.
    /// </summary>
    public static class YatSozlukUclari
    {
        public const string ProjeTurAdresi = ButceTanimOrtak.Klasor + "ProjeTurTanimla.aspx";
        public const string DokumanTurAdresi = ButceTanimOrtak.Klasor + "DokumanTurTanimla.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yatsozluk/{tur}/liste", Liste);
            app.MapPost("/api/yatsozluk/{tur}/kaydet", Kaydet);
            app.MapPost("/api/yatsozluk/{tur}/sil", Sil);
        }

        private static string Adres(string tur)
        {
            if (string.Equals(tur, "projetur", StringComparison.OrdinalIgnoreCase)) return ProjeTurAdresi;
            if (string.Equals(tur, "dokumantur", StringComparison.OrdinalIgnoreCase)) return DokumanTurAdresi;
            return null;
        }

        private static bool ProjeTurMu(string tur)
        {
            return string.Equals(tur, "projetur", StringComparison.OrdinalIgnoreCase);
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string tur)
        {
            string adres = Adres(tur);
            if (adres == null) return Results.NotFound();
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, adres);
            if (ret != null) return ret;
            try
            {
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                BilgiVAN bv = ProjeTurMu(tur) ? servis.ProjeTurListele(kullanan, new Sozluk()) : servis.DokumanTurListele(kullanan, new Sozluk());
                if (bv == null || !bv.sonuc.islemSonuc)
                    return ButceTanimOrtak.Hata(YatProjeOrtak.HataMetni(bv == null ? null : bv.sonuc.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (Sozluk s in bv.liste)
                    satirlar.Add(new { kod = s.kod, ad = s.ad ?? "" });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Liste okunamadı.");
            }
        }

        /// <summary>Eski ...Detay.Kaydet: kod varsa güncellenir, yoksa eklenir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, string tur, YatSozlukIstegi istek, IAntiforgery sahtecilik)
        {
            string adres = Adres(tur);
            if (adres == null) return Results.NotFound();
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, adres);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                Sozluk s = new Sozluk(istek.Kod, ButceTanimOrtak.Temiz(istek.Ad));
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                Sonuc sonuc = ProjeTurMu(tur) ? servis.ProjeTurKaydet(kullanan, s) : servis.DokumanTurKaydet(kullanan, s);
                return ButceTanimOrtak.SonucYaz(sonuc, "Kayıt işlemi başarıyla tamamlandı.", "Kayıt işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kayıt işlemi başarısız.");
            }
        }

        /// <summary>Eski ...Detay.btnSil_Click: kullanılmış tür silinemez (iş kuralı).</summary>
        private static async Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, string tur, YatSozlukIstegi istek, IAntiforgery sahtecilik)
        {
            string adres = Adres(tur);
            if (adres == null) return Results.NotFound();
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, adres);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Results.BadRequest();
            try
            {
                Sozluk s = new Sozluk(istek.Kod, ButceTanimOrtak.Temiz(istek.Ad));
                IYATServis servis = TNS.YAT.Arac.Tanimla();
                Sonuc sonuc = ProjeTurMu(tur) ? servis.ProjeTurSil(kullanan, s) : servis.DokumanTurSil(kullanan, s);
                return ButceTanimOrtak.SonucYaz(sonuc, "Silme işlemi başarıyla tamamlandı.", "Silme işlemi başarısız.");
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Silme işlemi başarısız.");
            }
        }
    }
}
