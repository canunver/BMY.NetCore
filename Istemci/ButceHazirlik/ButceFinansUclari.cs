using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.ButceKod;
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
using TNS.UZY;

namespace Istemci.ButceHazirlik
{
    public class ButceFinansIstegi
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public double? Finansman { get; set; }
    }

    /// <summary>
    /// Finansman teklifi sayfasının (eski ButceHazirlikv2/ButceFinansGiris.aspx; menüde "Finansman Teklifi") uçları:
    ///   GET  /api/butcefinans/tanim   hazırlık yılı, aşamalar, kullanıcının aşaması
    ///   GET  /api/butcefinans/bilgi   varsayılan kurumun finansmanı, gider ve gelir toplamları (eski TablolariDoldur)
    ///   POST /api/butcefinans/kaydet  finansman tutarı finansman cetveline (eski btnFinansKaydet_Click)
    ///   POST /api/butcefinans/yazdir  Form 18 finansman cetveli, eski rapor koduyla (eski btnYazdir_Click → Rapor 0-13)
    /// Hazine yardımı hesabı (Haz. Yar. Hesapla) tarayıcıda yapılır. Yetki menüdeki adrese göre denetlenir.
    /// </summary>
    public static class ButceFinansUclari
    {
        private const string SayfaAdres = "../ButceHazirlikv2/ButceFinansGiris.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/butcefinans/tanim", Tanim);
            app.MapGet("/api/butcefinans/bilgi", Bilgi);
            app.MapPost("/api/butcefinans/kaydet", Kaydet);
            app.MapPost("/api/butcefinans/yazdir", Yazdir);
        }

        /// <summary>Sistem yöneticisi ya da menüdeki "Finansman Teklifi" seçeneğinin yetkilisi.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (Istemci.Organizasyon.GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                MenuOkuyucu menu = new MenuOkuyucu(kullanan);
                return menu.SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdres);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Senaryo(string senaryo) { senaryo = (senaryo ?? "").Trim(); return senaryo == "" ? "00.00" : senaryo; }

        /// <summary>Açılış bilgileri (eski Page_Load).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                List<object> senaryolar = new List<object>();
                foreach (ButceSenaryo bs in TNS.BHMv2.Arac.Tanimla().ButceSenaryoListele(kullanan, null) ?? new ButceSenaryo[0])
                    senaryolar.Add(new { kod = bs.kod, ad = bs.kod + "- " + bs.ad });
                string senaryo = ButceRakamUclari.KullaniciSenaryoBul(kullanan);
                return Results.Json(new
                {
                    yil = KodTanimUclari.YilBul(null),
                    senaryo = string.IsNullOrWhiteSpace(senaryo) ? "00.00" : senaryo,
                    senaryolar = senaryolar,
                    kurumAd = BHGiderRaporlari.KurumAd()
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>
        /// Eski TablolariDoldur: varsayılan kurumun girilmiş finansmanı; toplam, sermaye (06 ve 07) ve diğer ödenekleri; toplam gelir ve
        /// öz gelir (toplam - cari hazine yardımı 04.2.1.1 - sermaye hazine yardımı 04.2.2.1).
        /// </summary>
        private static IResult Bilgi(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Hata("Yıl hatalı!");
            string senaryokod = Senaryo(senaryo);
            try
            {
                string kurkod = TNS.DEG.Arac.VarsayilanKurumBul();
                IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

                double finansman = servisUZY.UzayDegeriDbl(kullanan, "ABSFINANSMAN", yil + "-" + senaryokod + "-0-" + kurkod + "-1.9.9.1", false);

                string toplamodenekkosul = yil + "-" + senaryokod + "-0-" + kurkod;
                double toplamodenek = servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", toplamodenekkosul, false);

                string sermayeodenekkosul = yil + "-" + senaryokod + "-0-" + kurkod + "-" + "" + "-" + "" + "-" + "06,07" + "-";
                double sermayeodenek = servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", sermayeodenekkosul, false);

                string toplamgelirkosul = yil + "-" + senaryokod + "-0-" + kurkod;
                double toplamgelir = servisUZY.UzayDegeriDbl(kullanan, "ABSGELIR", toplamgelirkosul, false);

                string cariyardimgelirkosul = yil + "-" + senaryokod + "-0-" + kurkod + "-" + "04.2.1.1" + "-";
                double cariyardimgelir = servisUZY.UzayDegeriDbl(kullanan, "ABSGELIR", cariyardimgelirkosul, false);

                string sermayeyardimgelirkosul = yil + "-" + senaryokod + "-0-" + kurkod + "-" + "04.2.2.1" + "-";
                double sermayeyardimgelir = servisUZY.UzayDegeriDbl(kullanan, "ABSGELIR", sermayeyardimgelirkosul, false);

                return Results.Json(new
                {
                    finansman = finansman,
                    giderToplam = toplamodenek,
                    giderSermaye = sermayeodenek,
                    giderDiger = toplamodenek - sermayeodenek,
                    gelirToplam = toplamgelir,
                    gelirOzGelir = toplamgelir - cariyardimgelir - sermayeyardimgelir
                });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Bilgiler okunamadı."));
            }
        }

        /// <summary>Kaydet (eski btnFinansKaydet_Click): yalnızca finansman tutarı varsayılan kurumun 1.9.9.1 finansman tertibine yazılır.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ButceFinansIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new ButceFinansIstegi();
            if (istek.Yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                string kurkod = TNS.DEG.Arac.VarsayilanKurumBul();
                TertipOdenek to = new TertipOdenek(1);
                to.tertip = new Tertip(istek.Yil, ENUMTabloKod.TABFinansman);
                to.tertip.TertipAta(kurkod + "-1.9.9.1");
                to.odenek[0] = istek.Finansman ?? 0;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().ButceOdenekKaydet(kullanan, Senaryo(istek.Senaryo), new[] { to });
                if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc.hataStr, "Kayıt yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı." });
            }
            catch (Exception ex)
            {
                return Hata(HataMetni(ex.Message, "Kayıt yapılamadı."));
            }
        }

        /// <summary>Yazdır (eski btnYazdir_Click → TekRaporAl(0, 13)): Form 18 finansman cetveli (Excel), eski rapor koduyla.</summary>
        private static async Task<IResult> Yazdir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, ButceFinansIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new ButceFinansIstegi();
            if (istek.Yil < 2000) return Hata("Yıl hatalı!");
            try
            {
                AsposeLisans.Yukle();
                string dosya = BHGiderRaporlari.RaporAl(kullanan, "0-13", istek.Yil, Senaryo(istek.Senaryo), TNS.DEG.Arac.VarsayilanKurumBul(),
                    "", "", "", "", Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "Form18_" + DosyaIslem.DosyaAdUretSade(), true, Path.GetExtension(dosya).TrimStart('.'));
            }
            catch (RaporHatasi ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }
    }
}
