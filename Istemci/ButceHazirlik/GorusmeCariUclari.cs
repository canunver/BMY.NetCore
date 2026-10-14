using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public class GorusmeCariSatiri
    {
        public string Kod1 { get; set; }
        public string Kod2 { get; set; }
        public string Kod3 { get; set; }
        public string Kod4 { get; set; }
        public string Kod5 { get; set; }
        public string Kod6 { get; set; }
        public double Yil0 { get; set; }
        public double Yil1 { get; set; }
        public double Yil2 { get; set; }
        public double EkOdenek { get; set; }
    }

    public class GorusmeCariKayitIstegi
    {
        public int Yil { get; set; }
        /// <summary>Listelenen faaliyet/proje (onay durumu bununla okunur; adresle açılışta boş).</summary>
        public string Faal { get; set; }
        public List<GorusmeCariSatiri> Satirlar { get; set; }
    }

    public class GorusmeCariOnayIstegi
    {
        public int Yil { get; set; }
        public string Faal { get; set; }
        /// <summary>onayla (5), kapat (6), kaldir (1).</summary>
        public string Islem { get; set; }
    }

    /// <summary>
    /// Birim Görüşme Ekranı (eski ButceHazirlikv2/GorusmeCari.aspx) uçları:
    ///   GET  /api/gorusmecari/tanim, /faaliyetler?yil=&amp;ust=, /ekonomik?yil=&amp;ust=, /liste?yil=&amp;faal=&amp;eko= (ya da ?yil=&amp;tertip=), /rapor
    ///   POST /api/gorusmecari/kaydet, /onay
    /// Seçilen faaliyet/proje ve cari ekonomik kod için kurumun tertiplerinde SGB görüşme tutarları girilir (tavan türü
    /// tavanTurGorusmeCari, senaryo "0100"). Onay işlemleri seçili faaliyet/projenin kaydına yazılır.
    /// </summary>
    public static class GorusmeCariUclari
    {
        private const string Senaryo = "0100";
        private const string OnaySenaryo = "01.00";
        private const string SayfaAdres = "../ButceHazirlikv2/GorusmeCari.aspx";
        private static readonly string[] EkRaporlar = { "7500-4", "7500-5" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/gorusmecari/tanim", Tanim);
            app.MapGet("/api/gorusmecari/faaliyetler", Faaliyetler);
            app.MapGet("/api/gorusmecari/ekonomik", Ekonomik);
            app.MapGet("/api/gorusmecari/liste", Liste);
            app.MapGet("/api/gorusmecari/rapor", Rapor);
            app.MapPost("/api/gorusmecari/kaydet", Kaydet);
            app.MapPost("/api/gorusmecari/onay", Onay);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdres);
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

        internal static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        /// <summary>Kodlar iş kurallarında SQL'e yazılıyor: yalnızca rakam ve nokta (boş da olabilir).</summary>
        internal static bool KodGecerli(string kod)
        {
            return Regex.IsMatch(kod ?? "", @"^[0-9.]*$");
        }

        /// <summary>Eski BHMv2Ortak.CariMi.</summary>
        internal static bool CariMi(string kod)
        {
            kod = kod ?? "";
            return kod.StartsWith("01") || kod.StartsWith("02") || kod.StartsWith("03") || kod.StartsWith("05");
        }

        /// <summary>Eski BHMv2Ortak.YatirimMi.</summary>
        internal static bool YatirimMi(string kod)
        {
            kod = kod ?? "";
            return kod.StartsWith("06") || kod.StartsWith("07") || kod.StartsWith("08");
        }

        private static int DurumOku(Kullanici kullanan, int yil, string faal)
        {
            TavanGenel tg = new TavanGenel();
            tg.tamKod = faal;
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, OnaySenaryo, ENUMTavanTur.tavanTurGorusmeCari, tg);
            return tg == null ? -1 : tg.durum;
        }

        // ------------------------------------------------------------------ okuma

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(new { yil = KodTanimUclari.YilBul(null), sistemYoneticisi = GenelGorevUclari.YetkiVar(kullanan), raporlar = GorusmeRaporlari.Liste(EkRaporlar) });
        }

        /// <summary>Faaliyet/proje ağacı (eski ddProg: kod türü 14, bütçe hazırlık yetkisiyle süzülür).</summary>
        private static IResult Faaliyetler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, "14", ust);
        }

        /// <summary>Cari ekonomik kod ağacı (eski ddEko, kod türü "4c": kökte yalnızca 01, 02, 03 ve 05).</summary>
        private static IResult Ekonomik(HttpContext baglam, IWebHostEnvironment ortam, int yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return KodDuzeyi(kullanan, yil, "4", ust, true);
        }

        /// <summary>Kod ağacının bir düzeyi (yetki süzmesi olmadan); sadeceCari ise kökte yalnızca cari ekonomik kodlar.</summary>
        internal static IResult KodDuzeyi(Kullanici kullanan, int yil, string tur, string ust, bool sadeceCari)
        {
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            ust = (ust ?? "").Trim();
            try
            {
                IBKDServis servis = TNS.BKD.Arac.Tanimla();
                string kirilim = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD" + tur + "/KODYAPI") ?? "";
                int kirilimBoy = kirilim == "" ? 0 : kirilim.Split('.').Length;
                List<object> dugumler = new List<object>();

                int ustUz = 0;
                if (ust != "")
                {
                    ust = OrtakFonksiyonlar.KodNoktaKoy(kirilim, ust.Replace(".", ""));
                    ustUz = ust.Split('.').Length;
                }
                if (ustUz >= kirilimBoy) return Results.Json(new { dugumler = dugumler });
                bool altVar = ustUz < kirilimBoy - 1;

                ABSKodlarVAN kodlar = servis.KodListeleSeviyeIkiYil(yil, tur, ust, 0, false, false, "", "", "", true, true, "", false);
                if (kodlar == null || !kodlar.sonuc.islemSonuc) return Results.Json(new { hata = "Kodlar okunamadı." });
                string dil = string.IsNullOrEmpty(kullanan.aktifKultur) ? TNS.BKD.Arac.Diller(0) : kullanan.aktifKultur;
                foreach (ABSKod k in kodlar.absKodlar)
                {
                    if (sadeceCari && ust == "" && !CariMi(k.kod)) continue;
                    dugumler.Add(new { kod = k.kod, ad = k.kod + "- " + k.Ad(dil), altVar = altVar });
                }
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kodlar okunamadı." });
            }
        }

        private static object Satir(TavanGenel t)
        {
            return new
            {
                id = t.tamKod,
                kod1 = t.kod1, kod2 = t.kod2, kod3 = t.kod3, kod4 = t.kod4, kod5 = t.kod5, kod6 = t.kod6, ad4 = t.ad4,
                harcama_2 = t.harcama_2, odenek_1 = t.odenek_1, talep_0 = t.talep_0,
                teklif0 = t.teklif0, ekOdenekTeklif = t.ekOdenekTeklif, ekGerekceTeklif = (t.ekGerekceTeklif ?? "").Replace("<br>", "\n"),
                yil0 = t.yil0, ekOdenek = t.ekOdenek, teklif1 = t.teklif1, yil1 = t.yil1, teklif2 = t.teklif2, yil2 = t.yil2,
                sgb0 = t.sgb0, sgb1 = t.sgb1, sgb2 = t.sgb2, ekSgb = t.ekSgb
            };
        }

        /// <summary>
        /// Eski btnOku_Click / Oku: kurumun seçili cari ekonomik kod ve faaliyet/projeye uyan tertipleri (ekonomik kod 2. düzeyde).
        /// Adresle açılışta (eski ?yil=&amp;tertip=) verilen tertip ekonomik kodun tamamıyla okunur.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string faal, string eko, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            faal = (faal ?? "").Trim(); eko = (eko ?? "").Trim(); tertip = (tertip ?? "").Trim();

            bool eko2 = tertip == "";
            if (eko2)
            {
                if (!KodGecerli(faal) || !KodGecerli(eko)) return Results.Json(new { hata = "Kod hatalı!" });
                if (!CariMi(eko)) return Results.Json(new { hata = "Lütfen ekonomik kod olarak cari ekonomik kod seçiniz!" });
                tertip = TNS.DEG.Arac.VarsayilanKurumBul() + "---" + eko + "-" + faal;
            }
            else if (!Regex.IsMatch(tertip, @"^[0-9.\-]*$")) return Results.Json(new { hata = "Tertip hatalı!" });

            try
            {
                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                t.TertipAta(tertip);
                ObjectArray liste = TNS.BHMv2.Arac.Tanimla().ListeleGorusmeCari(kullanan, t, Senaryo, eko2);
                if (liste == null) return Results.Json(new { hata = "Tertipler okunamadı." });
                if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = HataMetni(liste.sonuc.hataStr, "Tertipler okunamadı.") });

                List<object> satirlar = new List<object>();
                for (int i = 0; i < liste.ObjeSayisi; i++)
                    satirlar.Add(Satir((TavanGenel)liste.objeler[i]));

                return Results.Json(new { satirlar = satirlar, eko2 = eko2, durum = faal != "" ? DurumOku(kullanan, yil, faal) : 1 });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Tertipler okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Eski Kaydet: değişen tertip satırlarının (ekonomik kodu 2 haneden uzun) SGB görüşme tutarları. Onaylı faaliyet/proje
        /// değiştirilemez (eski sayfa yalnızca girişi kapatıyordu, burada sunucu da denetler).
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, GorusmeCariKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                string faal = (istek.Faal ?? "").Trim();
                if (!KodGecerli(faal)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });
                if (faal != "")
                {
                    int durum = DurumOku(kullanan, istek.Yil, faal);
                    if (durum > 1) return Results.Json(new { basarili = false, hata = "Faaliyet/proje onaylanmış, değiştirilemez." });
                }

                TNSCollection tavanListe = new TNSCollection();
                foreach (GorusmeCariSatiri s in istek.Satirlar ?? new List<GorusmeCariSatiri>())
                {
                    if (s == null || (s.Kod4 ?? "").Length <= 2) continue;
                    if (!KodGecerli(s.Kod1) || !KodGecerli(s.Kod2) || !KodGecerli(s.Kod3) || !KodGecerli(s.Kod4) || !KodGecerli(s.Kod5) || !KodGecerli(s.Kod6)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = s.Kod1;
                    tg.kod2 = s.Kod2;
                    tg.kod3 = s.Kod3;
                    tg.kod4 = s.Kod4;
                    tg.kod5 = s.Kod5;
                    tg.kod6 = s.Kod6;
                    tg.yil0 = s.Yil0;
                    tg.yil1 = s.Yil1;
                    tg.yil2 = s.Yil2;
                    tg.ekOdenek = s.EkOdenek;
                    tavanListe.Add(tg);
                }
                if (tavanListe.Count == 0) return Results.Json(new { basarili = true, bilgi = "Kaydedilecek değişiklik yok." });

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurGorusmeCari, tavanListe);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Onayla (5), Kapat (6), Onay kaldır (1) (eski btnOnay_Click): seçili faaliyet/proje için.</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, GorusmeCariOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            if (string.IsNullOrWhiteSpace(istek.Faal)) return Results.Json(new { basarili = false, hata = "Lütfen onaylanacak faaliyeti seçiniz!" });
            if (!KodGecerli(istek.Faal.Trim())) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });

            int yeniDurum;
            switch (istek.Islem)
            {
                case "onayla": yeniDurum = 5; break;
                case "kapat": yeniDurum = 6; break;
                case "kaldir": yeniDurum = 1; break;
                default: return Results.Json(new { basarili = false, hata = "İşlem hatalı." });
            }

            try
            {
                TavanGenel tg = new TavanGenel();
                tg.tamKod = istek.Faal.Trim();
                tg.durum = yeniDurum;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurGorusmeCari, tg);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = HataMetni(sonuc.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "İşlem tamamlandı.", durum = yeniDurum });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>Rapor Al menüsü (eski btnRaporAl_Click → BHMv2Ortak.RaporAl(.., "0100", varsayılan kurum, "", "", eko, faaliyet)).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string faal, string eko, string rapor)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            faal = (faal ?? "").Trim(); eko = (eko ?? "").Trim();
            if (!KodGecerli(faal) || !KodGecerli(eko)) return Results.Json(new { hata = "Kod hatalı!" });
            if (!GorusmeRaporlari.Tanimli(rapor, EkRaporlar)) return Results.Json(new { hata = "Rapor seçimi hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = GorusmeRaporlari.Al(kullanan, rapor, yil, Senaryo, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", eko, faal, Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "Rapor_" + rapor + "_" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (RaporHatasi ex)
            {
                return Results.Json(new { hata = HataMetni(ex.Message, "Rapor hazırlanamadı.") });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
