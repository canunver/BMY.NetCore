using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ButceHazirlikv2;
using Istemci.ButceKod;
using Istemci.Organizasyon;
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

namespace Istemci.ButceHazirlik
{
    public class TavanSatiri
    {
        public string Kod1 { get; set; }
        public string Kod2 { get; set; }
        public string Kod3 { get; set; }
        public double Yil0 { get; set; }
        public double Yil1 { get; set; }
        public double Yil2 { get; set; }
    }

    public class TavanKayitIstegi
    {
        public int Yil { get; set; }
        public string Eko { get; set; }
        public string Sektor { get; set; }
        public List<TavanSatiri> Satirlar { get; set; }
    }

    public class TavanOnayIstegi
    {
        public int Yil { get; set; }
        public string Eko { get; set; }
        public string Sektor { get; set; }
        /// <summary>true: onayla (durum 5), false: onayı kaldır (durum 1).</summary>
        public bool Onayla { get; set; }
    }

    public class TavanUretIstegi
    {
        public int Yil { get; set; }
        public bool YatirimAsamasi { get; set; }
    }

    /// <summary>
    /// Ana Harcama Grupları Tavan Girişi sayfasının (eski ButceHazirlikv2/TavanEkoSektorBirim.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/tavaneko/tanim, /kodlar?yil=, /liste?yil=&amp;eko=&amp;sektor=&amp;yatirim=
    ///   POST /api/tavaneko/kaydet, /onay, /uret
    ///   GET  /api/tavaneko/rapor?yil=&amp;yatirim=, /rapor-vize?yil= (Excel; eski kodla, TavanRaporlari)
    /// Bir ekonomik kod + sektör için birimlerin (PEB birimleri) üç yıllık tavanları girilir; tavan türü tavanTurEkoSektorBirim,
    /// senaryo "0000". "Yat. Aşaması Kullan" işaretliyse önceki yıl ödenekleri 0700, değilse 0600 aşamasından okunur.
    /// Sayfayı menüde bu seçeneği görebilenler kullanır; onaylama / onay kaldırma yalnızca sistem yöneticisinindir.
    /// </summary>
    public static class TavanEkoSektorBirimUclari
    {
        private const string Senaryo = "0000";
        // Onay durumu: 5 onaylı, 1 onaysız, -1 kayıt yok. Eski sayfa durum "1"den büyükse tavanları değiştirtmiyordu.
        private const int Onayli = 5;

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tavaneko/tanim", Tanim);
            app.MapGet("/api/tavaneko/kodlar", Kodlar);
            app.MapGet("/api/tavaneko/liste", Liste);
            app.MapPost("/api/tavaneko/kaydet", Kaydet);
            app.MapPost("/api/tavaneko/onay", Onay);
            app.MapPost("/api/tavaneko/uret", TavanUret);
            app.MapGet("/api/tavaneko/rapor", Rapor);
            app.MapGet("/api/tavaneko/rapor-vize", RaporVize);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/TavanEkoSektorBirim.aspx");
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

        private static string OncekiYilSenaryo(bool yatirimAsamasi)
        {
            return yatirimAsamasi ? "0700" : "0600";
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        /// <summary>Ekonomik kod + sektör için tavanların onay durumu (eski BHMTavanGenelOnayOku; -1: kayıt yok).</summary>
        private static int DurumOku(Kullanici kullanan, int yil, string eko, string sektor)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = eko;
            tg.kod2 = sektor;
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, "00.00", ENUMTavanTur.tavanTurEkoSektorBirim, tg);
            return tg == null ? -1 : tg.durum;
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış: bütçe hazırlık yılı ve o yılın kodları (eski Page_Load).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = KodTanimUclari.YilBul(null);
            try
            {
                return Results.Json(new
                {
                    yil = yil,
                    sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan),
                    kodlar = KodlariOku(yil)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kodlar okunamadı." });
            }
        }

        /// <summary>
        /// Yıla bağlı seçenekler (eski YilDegisti): birinci düzey ekonomik kodlar (hiyerarşik) ve sektörlerin son kırılımı,
        /// başta "00 - Cari".
        /// </summary>
        private static object KodlariOku(int yil)
        {
            IBKDServis servis = TNS.BKD.Arac.Tanimla();

            List<object> ekoKodlar = new List<object>();
            ABSKodlarVAN eko = servis.KodListeleSeviye(yil, "4", "", 1, false, false, "", "", "", true);
            if (eko != null && eko.absKodlar != null)
                foreach (ABSKod k in eko.absKodlar)
                    ekoKodlar.Add(new { kod = k.kod, ad = k.kodAd });

            List<object> sektorler = new List<object> { new { kod = "00", ad = "00-Cari" } };
            ABSKodlarVAN sek = servis.KodListeleSeviye(yil, "7", "", -2, true, false, "", "", "", false);
            if (sek != null && sek.absKodlar != null)
                foreach (ABSKod k in sek.absKodlar)
                    sektorler.Add(new { kod = k.kod, ad = k.kodAd });

            return new { ekoKodlar = ekoKodlar, sektorler = sektorler };
        }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                return Results.Json(KodlariOku(yil));
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kodlar okunamadı." });
            }
        }

        /// <summary>
        /// Ekonomik kod + sektör için birimlerin tavanları ve karşılaştırma tutarları (eski Oku). Toplam satırını sayfa hesaplar.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string eko, string sektor, bool? yatirim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(eko) || string.IsNullOrWhiteSpace(sektor))
                return Results.Json(new { hata = "Lütfen ekonomik kod ve sektör seçiniz!" });

            try
            {
                ObjectArray liste = TNS.BHMv2.Arac.Tanimla().ListeleEkoSektorTavan(kullanan, yil, Senaryo, eko.Trim(), sektor.Trim(), null, OncekiYilSenaryo(yatirim == true));
                if (liste == null) return Results.Json(new { hata = "Tavanlar okunamadı." });
                if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(liste.sonuc.hataStr, "Tavanlar okunamadı.") });

                List<object> satirlar = new List<object>();
                for (int i = 0; i < liste.ObjeSayisi; i++)
                {
                    TavanGenel t = (TavanGenel)liste.objeler[i];
                    satirlar.Add(new
                    {
                        kod1 = t.kod1, kod2 = t.kod2, kod3 = t.kod3, ad3 = t.ad3,
                        odenek_2 = t.odenek_2, harcama_2 = t.harcama_2, odenek_1 = t.odenek_1,
                        odenek_1_1 = t.odenek_1_1, odenek_1_2 = t.odenek_1_2,
                        yil0 = t.yil0, yil1 = t.yil1, yil2 = t.yil2
                    });
                }
                return Results.Json(new { satirlar = satirlar, durum = DurumOku(kullanan, yil, eko.Trim(), sektor.Trim()) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Tavanlar okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Birim tavanlarını kaydeder (eski Kaydet). Onaylanmış tavanlar değiştirilemez (eski sayfa yalnızca ekranda engelliyordu,
        /// burada sunucu da denetler). Tutar denetimleri (negatif olamaz, binin katı) iş kurallarındadır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, TavanKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || string.IsNullOrWhiteSpace(istek.Eko) || string.IsNullOrWhiteSpace(istek.Sektor))
                return Results.Json(new { basarili = false, hata = "Lütfen ekonomik kod ve sektör seçiniz!" });

            try
            {
                if (DurumOku(kullanan, istek.Yil, istek.Eko.Trim(), istek.Sektor.Trim()) > 1)
                    return Results.Json(new { basarili = false, hata = "Tavanlar onaylanmış, değiştirilemez." });

                TNSCollection tavanListe = new TNSCollection();
                foreach (TavanSatiri s in istek.Satirlar ?? new List<TavanSatiri>())
                {
                    if (s == null || s.Kod3 == "00") continue; // toplam satırı
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = s.Kod1;
                    tg.kod2 = s.Kod2;
                    tg.kod3 = s.Kod3;
                    tg.yil0 = s.Yil0;
                    tg.yil1 = s.Yil1;
                    tg.yil2 = s.Yil2;
                    tavanListe.Add(tg);
                }

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurEkoSektorBirim, tavanListe);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Onayla / Onay kaldır (eski btnOnay_Click): yalnızca sistem yöneticisi.</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, TavanOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!GenelGorevUclari.YetkiVar(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || istek.Yil < 2000 || string.IsNullOrWhiteSpace(istek.Eko) || string.IsNullOrWhiteSpace(istek.Sektor))
                return Results.Json(new { basarili = false, hata = "Lütfen ekonomik kod ve sektör seçiniz!" });

            try
            {
                TavanGenel tg = new TavanGenel();
                tg.kod1 = istek.Eko.Trim();
                tg.kod2 = istek.Sektor.Trim();
                tg.durum = istek.Onayla ? Onayli : 1;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurEkoSektorBirim, tg);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", durum = tg.durum });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        /// <summary>
        /// Tavan Üret (eski btnTavanUret_Click): yılın var olan tavanları silinir, önceki yıl aşamasından (OVMP) üretilen tutarlar
        /// tavan olarak kaydedilir.
        /// </summary>
        private static async Task<IResult> TavanUret(HttpContext baglam, IWebHostEnvironment ortam, TavanUretIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().TavanUretEkoSektorBirim(kullanan, istek.Yil, Senaryo, OncekiYilSenaryo(istek.YatirimAsamasi));
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Tavanlar oluşturulamadı.") });
                return Results.Json(new { basarili = true, bilgi = "Tavanlar oluşturuldu!" });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Tavanlar oluşturulamadı." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>Yazdır: yılın bütün ekonomik kod / sektör / birim tavanları (şablon BHMv2\TavanEkoSektorBirim.xltx).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, bool? yatirim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = TavanRaporlari.TavanEkoSektorBirimYazdir(TNS.BHMv2.Arac.Tanimla(), kullanan, yil, OncekiYilSenaryo(yatirim == true), Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "OdenekTavanEkoSektorBirim_" + yil, true, "xlsx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        /// <summary>Yazdır (Yat. Viz. Detay): yatırım vize tavanları özeti, kriter yok (şablon BHMv2\YatirimVizeTavan.xltx).</summary>
        private static IResult RaporVize(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = TavanRaporlari.YatirimVizeYazdirOzet(TNS.BHMv2.Arac.Tanimla(), kullanan, yil, "", "", "", Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "VizeTavan_" + yil, true, "xlsx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
