using System;
using System.Collections.Generic;
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
    public class YatirimProjeSatiri
    {
        public string Kod1 { get; set; }
        public string Kod2 { get; set; }
        public string Kod3 { get; set; }
        public string Kod4 { get; set; }
        public string Kod6 { get; set; }
        public double Yil0 { get; set; }
        public double Yil1 { get; set; }
        public double Yil2 { get; set; }
        public double EkOdenek { get; set; }
        public double EkOdenek1 { get; set; }
        public double EkOdenek2 { get; set; }
    }

    public class YatirimProjeKayitIstegi
    {
        public int Tur { get; set; }
        public int Yil { get; set; }
        public string Eko { get; set; }
        public string Sektor { get; set; }
        public string Birim { get; set; }
        public List<YatirimProjeSatiri> Satirlar { get; set; }
    }

    public class YatirimProjeOnayIstegi
    {
        public int Tur { get; set; }
        public int Yil { get; set; }
        public string Eko { get; set; }
        public string Sektor { get; set; }
        public string Birim { get; set; }
        /// <summary>onayla (5), kapat (6), kaldir (1), ac (5, yalnızca sistem yöneticisi).</summary>
        public string Islem { get; set; }
    }

    /// <summary>
    /// Birim Görüşme Ekranı (Yatırım / Cari) (eski ButceHazirlikv2/TavanYatirimProje.aspx?tur=2 / ?tur=1) uçları:
    ///   GET  /api/tavanyatirim/tanim?tur=, /birimler?tur=&amp;yil=, /ekosektor?tur=&amp;yil=&amp;birim=, /liste?tur=&amp;yil=&amp;eko=&amp;sektor=&amp;birim=, /rapor
    ///   POST /api/tavanyatirim/kaydet, /onay
    /// Birim ve onun onaylı ekonomik kod + sektör tavanı (tur 2: yatırım 06-08, tur 1: cari 01-03, 05) seçilir; faaliyet/projelere SGB
    /// görüşme tutarları girilir (tavan türü tavanTurEkoFaaliyet, senaryo "0100"). Menüde yalnızca tur=2 vardır; tur=1 sistem yöneticisine açıktır.
    /// </summary>
    public static class TavanYatirimProjeUclari
    {
        private const string Senaryo = "0100";
        private const string OnaySenaryo = "01.00";
        private const string TavanSenaryo = "0000";
        private static readonly string[] EkRaporlar = { "7500-2", "7500-3" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tavanyatirim/tanim", Tanim);
            app.MapGet("/api/tavanyatirim/birimler", Birimler);
            app.MapGet("/api/tavanyatirim/ekosektor", EkoSektor);
            app.MapGet("/api/tavanyatirim/liste", Liste);
            app.MapGet("/api/tavanyatirim/rapor", Rapor);
            app.MapPost("/api/tavanyatirim/kaydet", Kaydet);
            app.MapPost("/api/tavanyatirim/onay", Onay);
        }

        /// <summary>Sayfa türü: 2 yatırım (menüdeki), 1 cari; verilmezse eski sayfadaki gibi 1.</summary>
        public static int TurDuzelt(int tur)
        {
            return tur == 2 ? 2 : 1;
        }

        /// <summary>Kullanıcı bu türdeki sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, int tur)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../ButceHazirlikv2/TavanYatirimProje.aspx?tur=" + TurDuzelt(tur));
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, int tur, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, tur)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static bool Bos(params string[] s)
        {
            foreach (string x in s)
                if (string.IsNullOrWhiteSpace(x)) return true;
            return false;
        }

        private static bool KodlarGecerli(params string[] s)
        {
            foreach (string x in s)
                if (!GorusmeCariUclari.KodGecerli(x)) return false;
            return true;
        }

        /// <summary>Faaliyet görüşme onay durumu (eski Oku sonundaki BHMTavanGenelOnayOku; -1: okunamadı, 0: kayıt yok).</summary>
        private static int DurumOku(Kullanici kullanan, int yil, string eko, string sektor, string birim)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = eko;
            tg.kod2 = sektor;
            tg.kod3 = birim;
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, OnaySenaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tg);
            return tg == null ? -1 : tg.durum;
        }

        /// <summary>Bir kod türünün birinci düzey kodları, hiyerarşik ad (eski ABSKodDoldur(yil, tur, "", 1, ...)).</summary>
        internal static List<object> BirinciDuzey(int yil, string kodTur)
        {
            List<object> kodlar = new List<object>();
            ABSKodlarVAN van = TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, kodTur, "", 1, false, false, "", "", "", true);
            if (van != null && van.absKodlar != null)
                foreach (ABSKod k in van.absKodlar)
                    kodlar.Add(new { kod = k.kod, ad = k.kodAd });
            return kodlar;
        }

        // ------------------------------------------------------------------ okuma

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int tur)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = KodTanimUclari.YilBul(null);
            bool sy = GenelGorevUclari.YetkiVar(kullanan);
            List<object> raporlar = GorusmeRaporlari.Liste(EkRaporlar);
            try
            {
                return Results.Json(new { yil = yil, sistemYoneticisi = sy, raporlar = raporlar, birimler = BirinciDuzey(yil, "14") });
            }
            catch (Exception)
            {
                return Results.Json(new { yil = yil, sistemYoneticisi = sy, raporlar = raporlar, hata = "Birimler okunamadı." });
            }
        }

        private static IResult Birimler(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil)
        {
            IResult ret = Denetle(baglam, ortam, tur, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                return Results.Json(new { birimler = BirinciDuzey(yil, "14") });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Birimler okunamadı." });
            }
        }

        /// <summary>Birimin onaylı ekonomik kod + sektör tavanları; türe göre yalnızca yatırım ya da cari olanlar (eski cbFaal1_Changed).</summary>
        private static IResult EkoSektor(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string birim)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (Bos(birim)) return Results.Json(new { kodlar = new object[0] });
            if (!KodlarGecerli(birim)) return Results.Json(new { hata = "Kod hatalı!" });
            tur = TurDuzelt(tur);
            try
            {
                TavanGenel kistas = new TavanGenel();
                kistas.kod3 = birim.Trim();
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().ListeleBHMTavanGenel(kullanan, yil, TavanSenaryo, ENUMTavanTur.tavanTurEkoSektorBirim, kistas, true);
                if (oa == null) return Results.Json(new { hata = "Ekonomik kod / sektör tavanları okunamadı." });
                if (!oa.sonuc.islemSonuc) return Results.Json(new { hata = GorusmeCariUclari.HataMetni(oa.sonuc.hataStr, "Ekonomik kod / sektör tavanları okunamadı.") });

                List<object> kodlar = new List<object>();
                foreach (TavanGenel t in oa.objeler)
                {
                    if (Bos(t.kod1)) continue;
                    if (tur == 1 ? !GorusmeCariUclari.CariMi(t.kod1) : !GorusmeCariUclari.YatirimMi(t.kod1)) continue;
                    kodlar.Add(new { eko = t.kod1, sektor = t.kod2, birim = t.kod3, ad = t.ekoSektorAciklama });
                }
                return Results.Json(new { kodlar = kodlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Ekonomik kod / sektör tavanları okunamadı." });
            }
        }

        private static object Satir(TavanGenel t)
        {
            return new
            {
                id = t.tamKod,
                kod1 = t.kod1, kod2 = t.kod2, kod3 = t.kod3, kod4 = t.kod4, kod5 = t.kod5, kod6 = t.kod6, ad4 = t.ad4, ad5 = t.ad5,
                harcama_2 = t.harcama_2, odenek_1 = t.odenek_1, talep_0 = t.talep_0,
                teklif0 = t.teklif0, ekOdenekTeklif = t.ekOdenekTeklif, yil0 = t.yil0, ekOdenek = t.ekOdenek,
                teklif1 = t.teklif1, ekOdenek1Teklif = t.ekOdenek1Teklif, yil1 = t.yil1, ekOdenek1 = t.ekOdenek1,
                teklif2 = t.teklif2, ekOdenek2Teklif = t.ekOdenek2Teklif, yil2 = t.yil2, ekOdenek2 = t.ekOdenek2,
                projeBitisYili = t.projeBitisYili, projeDevYeni = t.projeDevYeni,
                sgb0 = t.sgb0, sgb1 = t.sgb1, sgb2 = t.sgb2, ekSgb = t.ekSgb, ekSgb1 = t.ekSgb1, ekSgb2 = t.ekSgb2
            };
        }

        /// <summary>
        /// Eski Oku: ekonomik kod + sektör + birim tavanı (TAVAN satırı) ve faaliyet/projelerin görüşme tutarları.
        /// Proje grubu (kod5) ara toplamlarını, FARK ve TOPLAM satırlarını sayfa hesaplar.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string eko, string sektor, string birim)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (Bos(eko, sektor, birim)) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            eko = eko.Trim(); sektor = sektor.Trim(); birim = birim.Trim();
            if (!KodlarGecerli(eko, sektor, birim)) return Results.Json(new { hata = "Kod hatalı!" });

            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                ObjectArray liste1 = servis.ListeleEkoSektorTavan(kullanan, yil, TavanSenaryo, eko, sektor, birim, "0600");
                if (liste1 == null) return Results.Json(new { hata = "Tavan okunamadı." });
                if (!liste1.sonuc.islemSonuc) return Results.Json(new { hata = GorusmeCariUclari.HataMetni(liste1.sonuc.hataStr, "Tavan okunamadı.") });
                if (liste1.ObjeSayisi != 1) return Results.Json(new { hata = "Seçilen kod için tavan bilgisi bulunamadı!" });

                ObjectArray liste = servis.ListeleYatirimProjeTavan(kullanan, yil, Senaryo, eko, sektor, birim, null);
                if (liste == null) return Results.Json(new { hata = "Faaliyet/proje tutarları okunamadı." });
                if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = GorusmeCariUclari.HataMetni(liste.sonuc.hataStr, "Faaliyet/proje tutarları okunamadı.") });

                List<object> satirlar = new List<object>();
                for (int i = 0; i < liste.ObjeSayisi; i++)
                    satirlar.Add(Satir((TavanGenel)liste.objeler[i]));

                return Results.Json(new
                {
                    tavan = Satir((TavanGenel)liste1.objeler[0]),
                    satirlar = satirlar,
                    durum = DurumOku(kullanan, yil, eko, sektor, birim)
                });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Faaliyet/proje tutarları okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Eski Kaydet: faaliyet/proje satırlarının görüşme tutarları ve ek ödenekleri (kod5 proje grubu yazılmaz). Onaylı tavan
        /// değiştirilemez (eski sayfa Kaydet'i gizliyordu, burada sunucu da denetler); satırlar listelenen tavana ait olmalıdır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, YatirimProjeKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil < 2000 || Bos(istek.Eko, istek.Sektor, istek.Birim))
                return Results.Json(new { basarili = false, hata = "Lütfen seçim yapınız!" });

            string eko = istek.Eko.Trim(), sektor = istek.Sektor.Trim(), birim = istek.Birim.Trim();
            if (!KodlarGecerli(eko, sektor, birim)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });
            try
            {
                int durum = DurumOku(kullanan, istek.Yil, eko, sektor, birim);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Onay durumu okunamadı." });
                if (durum > 1) return Results.Json(new { basarili = false, hata = "Onaylanmış, değiştirilemez." });

                TNSCollection tavanListe = new TNSCollection();
                foreach (YatirimProjeSatiri s in istek.Satirlar ?? new List<YatirimProjeSatiri>())
                {
                    if (s == null || (s.Kod1 ?? "").Length <= 1) continue; // ara toplam satırları
                    if (s.Kod1 != eko || s.Kod2 != sektor || s.Kod3 != birim) continue;
                    if (!GorusmeCariUclari.KodGecerli(s.Kod4) || !GorusmeCariUclari.KodGecerli(s.Kod6)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = s.Kod1;
                    tg.kod2 = s.Kod2;
                    tg.kod3 = s.Kod3;
                    tg.kod4 = s.Kod4;
                    tg.kod5 = null;
                    tg.kod6 = s.Kod6;
                    tg.yil0 = s.Yil0;
                    tg.yil1 = s.Yil1;
                    tg.yil2 = s.Yil2;
                    tg.ekOdenek = s.EkOdenek;
                    tg.ekOdenek1 = s.EkOdenek1;
                    tg.ekOdenek2 = s.EkOdenek2;
                    tavanListe.Add(tg);
                }
                if (tavanListe.Count == 0) return Results.Json(new { basarili = true, bilgi = "Kaydedilecek değişiklik yok." });

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tavanListe);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = GorusmeCariUclari.HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>
        /// Onayla (5), Faaliyet Girişine Kapat (6), Onay kaldır (1), Faaliyet Girişine Aç (5) (eski btnOnay_Click).
        /// Girişe kapalı (6) kaydı yalnızca sistem yöneticisi açabilir (eski sayfada yalnızca ona görünen tuş).
        /// </summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, YatirimProjeOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            if (istek == null) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil < 2000 || Bos(istek.Eko, istek.Sektor, istek.Birim))
                return Results.Json(new { basarili = false, hata = "Lütfen onaylanacak faaliyeti seçiniz!" });

            int yeniDurum;
            switch (istek.Islem)
            {
                case "onayla": yeniDurum = 5; break;
                case "kapat": yeniDurum = 6; break;
                case "kaldir": yeniDurum = 1; break;
                case "ac": yeniDurum = 5; break;
                default: return Results.Json(new { basarili = false, hata = "İşlem hatalı." });
            }

            try
            {
                TavanGenel tg = new TavanGenel();
                tg.kod1 = istek.Eko.Trim();
                tg.kod2 = istek.Sektor.Trim();
                tg.kod3 = istek.Birim.Trim();
                if (!KodlarGecerli(tg.kod1, tg.kod2, tg.kod3)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });

                int durum = DurumOku(kullanan, istek.Yil, tg.kod1, tg.kod2, tg.kod3);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Onay durumu okunamadı." });
                if ((durum == 6 || istek.Islem == "ac") && !GenelGorevUclari.YetkiVar(kullanan))
                    return Results.StatusCode(StatusCodes.Status403Forbidden);

                tg.durum = yeniDurum;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurEkoFaaliyet, tg);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = GorusmeCariUclari.HataMetni(sonuc.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "İşlem tamamlandı.", durum = yeniDurum });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>Rapor Al menüsü (eski btnRaporAl_Click → BHMv2Ortak.RaporAl(.., "0100", varsayılan kurum, "", "", "", birim)).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int tur, int yil, string eko, string sektor, string birim, string rapor)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (Bos(eko, sektor, birim)) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            birim = birim.Trim();
            if (!KodlarGecerli(birim)) return Results.Json(new { hata = "Kod hatalı!" });
            if (!GorusmeRaporlari.Tanimli(rapor, EkRaporlar)) return Results.Json(new { hata = "Rapor seçimi hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = GorusmeRaporlari.Al(kullanan, rapor, yil, Senaryo, TNS.DEG.Arac.VarsayilanKurumBul(), "", "", "", birim, Ayar.Oku("RaporSablonYol"));
                return DosyaIslem.DosyaGonder(dosya, "Rapor_" + rapor + "_" + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (RaporHatasi ex)
            {
                return Results.Json(new { hata = GorusmeCariUclari.HataMetni(ex.Message, "Rapor hazırlanamadı.") });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }
    }
}
