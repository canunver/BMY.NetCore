using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    /// <summary>Toplu veri girişindeki bir satır (eski stoYatTopluGirisDetay kaydı; kaydedilen alanlar).</summary>
    public class YatTopluSatir
    {
        public int KaynakNo { get; set; }
        public double ProjeToplam_Dis { get; set; }
        public double ProjeToplam_Toplam { get; set; }
        public double Odenek_Dis { get; set; }
        public double Odenek_Toplam { get; set; }
        public double RevizeOdenek_Dis { get; set; }
        public double RevizeOdenek_Toplam { get; set; }
        public double Harcama_Dis { get; set; }
        public double Harcama_Toplam { get; set; }
        public double KumulatifHarcama_Dis { get; set; }
        public double KumulatifHarcama_Toplam { get; set; }
    }

    public class YatTopluKayitIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public List<YatTopluSatir> Satirlar { get; set; }
    }

    /// <summary>
    /// Yatırım Projeleri Toplu Veri Girişi (eski ButceHazirlikv2/YatTopluVeriGirisForm.aspx; "Toplu Veri Giriş") uçları:
    ///   GET  /api/yattoplu/tanim, /liste?yil=&amp;senaryo=&amp;sektor=&amp;projeAd=
    ///   POST /api/yattoplu/kaydet
    /// Okuma eskisi gibi ButceKaynakBilgiOku, kaydetme YATKaynakKaydetTopluVeri ile yapılır (yazma yetkisini iş kuralı denetler).
    /// Sayfayı menüde bu seçeneği görebilenler kullanır.
    /// </summary>
    public static class YatTopluVeriUclari
    {
        public const string Adres = "../ButceHazirlikv2/YatTopluVeriGirisForm.aspx";

        // Eski Kaydet: kaydedilen mali yapı kalemleri.
        private static readonly ENUMProjeMaliYapiKod[] KaydedilenKalemler = new ENUMProjeMaliYapiKod[]
        {
            ENUMProjeMaliYapiKod.kaynakToplam, ENUMProjeMaliYapiKod.teklif0, ENUMProjeMaliYapiKod.odenekEklenen,
            ENUMProjeMaliYapiKod.odenekDusulen, ENUMProjeMaliYapiKod.harcamaYilSonu, ENUMProjeMaliYapiKod.kumulatifHarcama
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/yattoplu/tanim", Tanim);
            app.MapGet("/api/yattoplu/liste", Liste);
            app.MapPost("/api/yattoplu/kaydet", Kaydet);
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

        /// <summary>
        /// Açılış (eski Page_Load): yıl bu yıl; aşamalar (kullanıcının son aşaması seçili); sektörler (son kırılım, başta boş = hepsi).
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int yil = DateTime.Now.Year;

                List<object> asamalar = new List<object>();
                ButceSenaryo[] senaryolar = TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null);
                if (senaryolar != null)
                    foreach (ButceSenaryo bs in senaryolar)
                        asamalar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });

                // Eski: KullaniciSenaryoDegerBul, boşsa 06.00; KodNoktaKoy("99.99", ...)
                string senaryo = ButceRakamUclari.KullaniciSenaryoBul(kullanan);
                if (string.IsNullOrWhiteSpace(senaryo)) senaryo = "06.00";
                senaryo = OrtakFonksiyonlar.KodNoktaKoy("99.99", senaryo.Replace(".", ""));

                // Eski BHMv2Ortak.ABSKodDoldur(yil, "7", "", -2, ..., bosBasaEkle: true, ...); ilk (boş) seçenek seçili.
                List<object> sektorler = new List<object> { new { kod = "", ad = "" } };
                ABSKodlarVAN sek = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "7", "", -2, true, false, "", "", "", false);
                if (sek != null && sek.absKodlar != null)
                    foreach (ABSKod k in sek.absKodlar)
                        sektorler.Add(new { kod = k.kod, ad = k.kodAd });

                return Results.Json(new { yil = yil, senaryo = senaryo, asamalar = asamalar, sektorler = sektorler });
            }
            catch (Exception)
            {
                return YatProjeOrtak.Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Oku (eski btnOku_Click): seçime uyan bütçe kaynakları ve rakamları.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string sektor, string projeAd)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return YatProjeOrtak.Hata("Yıl hatalı!");
            try
            {
                BilgiVAN bv = TNS.YAT.Arac.Tanimla().ButceKaynakBilgiOku(kullanan, 0, yil, YatProjeOrtak.Temiz(senaryo),
                    YatProjeOrtak.Temiz(sektor), true, false, true, YatProjeOrtak.Temiz(projeAd));
                if (bv == null || !bv.sonuc.islemSonuc)
                    return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(bv == null ? null : bv.sonuc.hataStr, "Bilgiler okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (ButceKaynak bk in bv.liste)
                    satirlar.Add(new
                    {
                        bk.dptProjeNo, bk.dptProjeAd, bk.altProjeAdi, bk.kaynakAd, bk.kaynakNo,
                        bk.projeToplam_Dis, bk.projeToplam_Toplam, bk.odenek_Dis, bk.odenek_Toplam,
                        bk.revizeOdenek_Dis, bk.revizeOdenek_Toplam, bk.harcama_Dis, bk.harcama_Toplam,
                        bk.kumulatifHarcama_Dis, bk.kumulatifHarcama_Toplam
                    });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return YatProjeOrtak.Hata(YatProjeOrtak.HataMetni(ex.Message, "Bilgiler okunamadı."));
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click): tablodaki bütün satırlar, okunan yıl ve aşamaya kaydedilir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, YatTopluKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (istek.Satirlar == null || istek.Satirlar.Count == 0) return Results.Json(new { basarili = false, hata = "Kaydedilecek satır yok." });
            try
            {
                List<ButceKaynak> bklar = new List<ButceKaynak>();
                foreach (YatTopluSatir s in istek.Satirlar)
                {
                    if (s == null || s.KaynakNo <= 0) continue;
                    // Eski JSON.Deserialize(..., typeof(ButceKaynak[])) gibi: değerler özelliklerle ilk teklife yazılır.
                    ButceKaynak bk = new ButceKaynak();
                    bk.kaynakNo = s.KaynakNo;
                    bk.projeToplam_Dis = s.ProjeToplam_Dis;
                    bk.projeToplam_Toplam = s.ProjeToplam_Toplam;
                    bk.odenek_Dis = s.Odenek_Dis;
                    bk.odenek_Toplam = s.Odenek_Toplam;
                    bk.revizeOdenek_Dis = s.RevizeOdenek_Dis;
                    bk.revizeOdenek_Toplam = s.RevizeOdenek_Toplam;
                    bk.harcama_Dis = s.Harcama_Dis;
                    bk.harcama_Toplam = s.Harcama_Toplam;
                    bk.kumulatifHarcama_Dis = s.KumulatifHarcama_Dis;
                    bk.kumulatifHarcama_Toplam = s.KumulatifHarcama_Toplam;
                    bklar.Add(bk);
                }
                if (bklar.Count == 0) return Results.Json(new { basarili = false, hata = "Kaydedilecek satır yok." });
                Sonuc sonuc = TNS.YAT.Arac.Tanimla().YATKaynakKaydetTopluVeri(kullanan, istek.Yil, YatProjeOrtak.Temiz(istek.Senaryo),
                    bklar.ToArray(), KaydedilenKalemler);
                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(sonuc == null ? null : sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = YatProjeOrtak.HataMetni(ex.Message, "Kayıt işlemi başarısız.") });
            }
        }
    }
}
