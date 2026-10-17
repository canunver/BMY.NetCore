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
    public class KalkinmaBolgeIstegi
    {
        public string BolgeKod { get; set; }
        public string BolgeAd { get; set; }
        /// <summary>Bölgedeki illerin kodları.</summary>
        public List<string> Iller { get; set; }
    }

    /// <summary>
    /// Kalkınma Bölgeleri sayfasının (eski ButceHazirlikv2/KalkinmaBolgeleriTanimForm.aspx) uçları:
    ///   GET  /api/kalkinmabolge/tanim   iller (eski stoIlListe) ve tanımlı bölgeler (eski stobolgeKod)
    ///   GET  /api/kalkinmabolge/oku?kod= bölgenin adı ve illeri (eski Oku)
    ///   POST /api/kalkinmabolge/kaydet  bölge ve illeri silinip yeniden yazılır (iş kuralı; yalnızca sistem yöneticisi yazar)
    /// Eski sayfadaki Yazdır tuşunun kodu boştu; alınmadı.
    /// </summary>
    public static class KalkinmaBolgeUclari
    {
        public const string SayfaAdresi = ButceTanimOrtak.Klasor + "KalkinmaBolgeleriTanimForm.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/kalkinmabolge/tanim", Tanim);
            app.MapGet("/api/kalkinmabolge/oku", Oku);
            app.MapPost("/api/kalkinmabolge/kaydet", Kaydet);
        }

        private static List<object> Bolgeler(Kullanici kullanan)
        {
            List<object> bolgeler = new List<object>();
            ObjectArray oa = TNS.YAT.Arac.Tanimla().ListeleKalkinmaBolgeleriTanim(kullanan, new KalkinmaBolge());
            if (oa != null && oa.sonuc.islemSonuc)
                foreach (KalkinmaBolge b in oa.objeler)
                    bolgeler.Add(new { kod = b.bolgeKod ?? "", ad = b.bolgeAd ?? "" });
            return bolgeler;
        }

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            try
            {
                Cografya kistas = new Cografya();
                kistas.ilMi = true;
                Cografya[] cler = TNS.KYM.Arac.Tanimla().ListeleCografya(kullanan, kistas) ?? new Cografya[0];
                List<object> iller = new List<object>();
                foreach (Cografya c in cler)
                    iller.Add(new { kod = c.kod ?? "", ad = c.ad ?? "" });
                return Results.Json(new { iller = iller, bolgeler = Bolgeler(kullanan) });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Tanımlar okunamadı.");
            }
        }

        /// <summary>Oku (eski btnOku_Click): bölge koduna göre bölgenin adı ve illeri.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, string kod)
        {
            IResult ret = ButceTanimOrtak.Denetle(baglam, ortam, out Kullanici kullanan, SayfaAdresi);
            if (ret != null) return ret;
            kod = ButceTanimOrtak.Temiz(kod);
            if (kod == "") return ButceTanimOrtak.Hata("Bölge kodunu yazınız.");
            try
            {
                KalkinmaBolge form = new KalkinmaBolge();
                form.bolgeKod = kod;
                ObjectVAN ov = TNS.YAT.Arac.Tanimla().OkuKalkinmaBolgeleriTamim(kullanan, form);
                if (ov == null || !ov.sonuc.islemSonuc)
                    return ButceTanimOrtak.Hata(YatProjeOrtak.HataMetni(ov == null ? null : ov.sonuc.hataStr, "Bölge okunamadı."));
                if (ov.o == null) return Results.Json(new { bulunamadi = true });
                form = (KalkinmaBolge)ov.o;
                List<object> iller = new List<object>();
                if (form.kalkinmaBolgeIl != null)
                    foreach (Cografya c in form.kalkinmaBolgeIl)
                        iller.Add(new { kod = c.kod ?? "", ad = c.ad ?? "" });
                return Results.Json(new { bolgeKod = form.bolgeKod ?? "", bolgeAd = form.bolgeAd ?? "", iller = iller });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Hata("Bölge okunamadı.");
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click).</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, KalkinmaBolgeIstegi istek, IAntiforgery sahtecilik)
        {
            var (ret, kullanan) = await ButceTanimOrtak.DenetleYaz(baglam, ortam, sahtecilik, SayfaAdresi);
            if (ret != null) return ret;
            if (istek == null) return Results.BadRequest();
            try
            {
                KalkinmaBolge form = new KalkinmaBolge();
                form.bolgeKod = ButceTanimOrtak.Temiz(istek.BolgeKod);
                form.bolgeAd = ButceTanimOrtak.Temiz(istek.BolgeAd);
                form.kalkinmaBolgeIl = new TNSCollection();
                HashSet<string> eklenen = new HashSet<string>();
                foreach (string il in istek.Iller ?? new List<string>())
                {
                    string k = ButceTanimOrtak.Temiz(il);
                    if (k == "" || !eklenen.Add(k)) continue;
                    Cografya c = new Cografya();
                    c.kod = k;
                    form.kalkinmaBolgeIl.Add(c);
                }
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().KaydetKalkinmaBolgeleriTamim(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc)
                    return ButceTanimOrtak.Basarisiz(YatProjeOrtak.HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", bolgeler = Bolgeler(kullanan) });
            }
            catch (Exception)
            {
                return ButceTanimOrtak.Basarisiz("Kayıt işlemi başarısız.");
            }
        }
    }
}
