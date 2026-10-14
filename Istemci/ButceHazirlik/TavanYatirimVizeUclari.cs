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
using TNS.YAT;

namespace Istemci.ButceHazirlik
{
    public class YatirimVizeSatiri
    {
        public string Kod1 { get; set; }
        public string Kod2 { get; set; }
        public string Kod3 { get; set; }
        public string Kod4 { get; set; }
        public double Yil0 { get; set; }
        public double Yil1 { get; set; }
        public double Yil2 { get; set; }
    }

    public class YatirimVizeKayitIstegi
    {
        public int Yil { get; set; }
        public string Birim { get; set; }
        public string Eko { get; set; }
        public string Fin { get; set; }
        public List<YatirimVizeSatiri> Satirlar { get; set; }
    }

    public class YatirimVizeOnayIstegi
    {
        public int Yil { get; set; }
        public string Birim { get; set; }
        public string Eko { get; set; }
        public string Fin { get; set; }
        /// <summary>onayla (5), kapat (6), kaldir (1), ac (5, yalnızca sistem yöneticisi).</summary>
        public string Islem { get; set; }
    }

    public class YatirimVizeTopluIstegi
    {
        public int Yil { get; set; }
    }

    /// <summary>
    /// Yatırım Projeleri Vize Tavanı Girişi (eski ButceHazirlikv2/TavanYatirimVize.aspx) uçları:
    ///   GET  /api/tavanvize/tanim, /kodlar?yil=, /projeler?yil=&amp;birim=, /liste?yil=&amp;birim=&amp;eko=&amp;fin=&amp;proje=
    ///   POST /api/tavanvize/kaydet, /onay, /toplu-kapat
    ///   GET  /api/tavanvize/yazdir, /yazdir-detay, /rapor (Excel; eski kodla)
    /// Birim + ekonomik kod + finansman (birinci düzey) seçilir; faaliyet/projelerin üç yıllık vize tavanları girilir
    /// (tavan türü tavanTurYatirimVize, senaryo "10.00"). Onay: 0/1 açık, 5 onaylı, 6 girişe kapalı.
    /// </summary>
    public static class TavanYatirimVizeUclari
    {
        private const string Senaryo = "10.00";
        private const string RaporSenaryo = "1000";
        private const string SayfaAdres = "../ButceHazirlikv2/TavanYatirimVize.aspx";
        private static readonly string[] EkRaporlar = { "7500-2", "7500-3" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/tavanvize/tanim", Tanim);
            app.MapGet("/api/tavanvize/kodlar", Kodlar);
            app.MapGet("/api/tavanvize/projeler", Projeler);
            app.MapGet("/api/tavanvize/liste", Liste);
            app.MapPost("/api/tavanvize/kaydet", Kaydet);
            app.MapPost("/api/tavanvize/onay", Onay);
            app.MapPost("/api/tavanvize/toplu-kapat", TopluKapat);
            app.MapGet("/api/tavanvize/yazdir", Yazdir);
            app.MapGet("/api/tavanvize/yazdir-detay", YazdirDetay);
            app.MapGet("/api/tavanvize/rapor", Rapor);
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

        private static string Kirp(string s)
        {
            return (s ?? "").Trim();
        }

        /// <summary>Vize tavanı onay durumu (eski Oku sonundaki BHMTavanGenelOnayOku; -1: okunamadı, 0: kayıt yok).</summary>
        private static int DurumOku(Kullanici kullanan, int yil, string birim, string eko, string fin)
        {
            TavanGenel tg = new TavanGenel();
            tg.kod1 = birim;
            tg.kod2 = eko;
            tg.kod3 = fin;
            tg = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayOku(kullanan, yil, Senaryo, ENUMTavanTur.tavanTurYatirimVize, tg);
            return tg == null ? -1 : tg.durum;
        }

        private static object KodListeleri(int yil)
        {
            return new
            {
                birimler = TavanYatirimProjeUclari.BirinciDuzey(yil, "14"),
                ekolar = TavanYatirimProjeUclari.BirinciDuzey(yil, "4"),
                finler = TavanYatirimProjeUclari.BirinciDuzey(yil, "3")
            };
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış: bütçe hazırlık yılı, raporlar ve o yılın birim / ekonomik / finansman kodları (eski Page_Load / YilDegisti).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            int yil = KodTanimUclari.YilBul(null);
            bool sy = GenelGorevUclari.YetkiVar(kullanan);
            List<object> raporlar = GorusmeRaporlari.Liste(EkRaporlar);
            try
            {
                return Results.Json(new { yil = yil, sistemYoneticisi = sy, raporlar = raporlar, kodlar = KodListeleri(yil) });
            }
            catch (Exception)
            {
                return Results.Json(new { yil = yil, sistemYoneticisi = sy, raporlar = raporlar, hata = "Kodlar okunamadı." });
            }
        }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil)
        {
            IResult ret = Denetle(baglam, ortam, out _);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                return Results.Json(new { kodlar = KodListeleri(yil) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Kodlar okunamadı." });
            }
        }

        /// <summary>Birimin projeleri, başta "Hepsi" (-1) (eski ddProg_Changed).</summary>
        private static IResult Projeler(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            birim = Kirp(birim);
            if (!KodlarGecerli(birim)) return Results.Json(new { hata = "Kod hatalı!" });

            List<object> projeler = new List<object>();
            projeler.Add(new { kod = -1, ad = "Hepsi" });
            if (birim == "") return Results.Json(new { projeler = projeler });
            try
            {
                ObjectArray oa = TNS.YAT.Arac.Tanimla().ProjeListeleFaal(kullanan, yil, Senaryo, birim);
                if (oa == null || !oa.sonuc.islemSonuc)
                    return Results.Json(new { hata = GorusmeCariUclari.HataMetni(oa == null ? "" : oa.sonuc.hataStr, "Projeler okunamadı."), projeler = projeler });
                foreach (Proje p in oa.objeler)
                    projeler.Add(new { kod = p.kod, ad = p.kodAd });
                return Results.Json(new { projeler = projeler });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Projeler okunamadı.", projeler = projeler });
            }
        }

        private static object Satir(TavanGenel t)
        {
            return new
            {
                id = t.tamKod,
                kod1 = t.kod1, kod2 = t.kod2, kod3 = t.kod3, kod4 = t.kod4, kod5 = t.kod5, kod6 = t.kod6, ad4 = t.ad4, ad5 = t.ad5,
                harcama_2 = t.harcama_2, odenek_1 = t.odenek_1,
                teklif0 = t.teklif0, yil0 = t.yil0, teklif1 = t.teklif1, yil1 = t.yil1, teklif2 = t.teklif2, yil2 = t.yil2,
                sgb0 = t.sgb0, sgb1 = t.sgb1, sgb2 = t.sgb2,
                projeBitisYili = t.projeBitisYili, projeDevYeni = t.projeDevYeni
            };
        }

        /// <summary>Eski Oku: seçilen birim + ekonomik kod + finansman (ve proje) için faaliyet/projelerin vize tavanları.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, string eko, string fin, int? proje)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            if (Bos(birim, eko, fin)) return Results.Json(new { hata = "Lütfen seçim yapınız!" });
            birim = Kirp(birim); eko = Kirp(eko); fin = Kirp(fin);
            if (!KodlarGecerli(birim, eko, fin)) return Results.Json(new { hata = "Kod hatalı!" });

            try
            {
                ObjectArray liste = TNS.BHMv2.Arac.Tanimla().ListeleYatirimVizeTavan(kullanan, yil, Senaryo, birim, eko, fin, null, proje ?? -1);
                if (liste == null) return Results.Json(new { hata = "Vize tavanları okunamadı." });
                if (!liste.sonuc.islemSonuc) return Results.Json(new { hata = GorusmeCariUclari.HataMetni(liste.sonuc.hataStr, "Vize tavanları okunamadı.") });

                List<object> satirlar = new List<object>();
                for (int i = 0; i < liste.ObjeSayisi; i++)
                    satirlar.Add(Satir((TavanGenel)liste.objeler[i]));

                return Results.Json(new { satirlar = satirlar, durum = DurumOku(kullanan, yil, birim, eko, fin) });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Vize tavanları okunamadı." });
            }
        }

        // ------------------------------------------------------------------ yazma

        /// <summary>
        /// Eski Kaydet: faaliyet/proje satırlarının vize tavanları (kod5 / kod6 yazılmaz). Onaylı tavan değiştirilemez (eski sayfa
        /// Kaydet'i gizliyordu, burada sunucu da denetler); satırlar listelenen birim + ekonomik kod + finansmana ait olmalıdır.
        /// </summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, YatirimVizeKayitIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || Bos(istek.Birim, istek.Eko, istek.Fin))
                return Results.Json(new { basarili = false, hata = "Lütfen seçim yapınız!" });

            string birim = Kirp(istek.Birim), eko = Kirp(istek.Eko), fin = Kirp(istek.Fin);
            if (!KodlarGecerli(birim, eko, fin)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });
            try
            {
                int durum = DurumOku(kullanan, istek.Yil, birim, eko, fin);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Onay durumu okunamadı." });
                if (durum > 1) return Results.Json(new { basarili = false, hata = "Vize tavanları onaylanmış, değiştirilemez." });

                TNSCollection tavanListe = new TNSCollection();
                foreach (YatirimVizeSatiri s in istek.Satirlar ?? new List<YatirimVizeSatiri>())
                {
                    if (s == null || (s.Kod1 ?? "").Length <= 1) continue; // ara toplam satırları
                    if (s.Kod1 != birim || s.Kod2 != eko || s.Kod3 != fin) continue;
                    if (!GorusmeCariUclari.KodGecerli(s.Kod4)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });
                    TavanGenel tg = new TavanGenel();
                    tg.kod1 = s.Kod1;
                    tg.kod2 = s.Kod2;
                    tg.kod3 = s.Kod3;
                    tg.kod4 = s.Kod4;
                    tg.kod5 = null;
                    tg.kod6 = null;
                    tg.yil0 = s.Yil0;
                    tg.yil1 = s.Yil1;
                    tg.yil2 = s.Yil2;
                    tavanListe.Add(tg);
                }
                if (tavanListe.Count == 0) return Results.Json(new { basarili = true, bilgi = "Kaydedilecek değişiklik yok." });

                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().KaydetBHMTavanGenel(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurYatirimVize, tavanListe);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = GorusmeCariUclari.HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
        }

        /// <summary>Onayla (5), Faaliyet Girişine Kapat (6), Onay kaldır (1), Faaliyet Girişine Aç (5, sistem yöneticisi) (eski btnOnay_Click).</summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, YatirimVizeOnayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000 || Bos(istek.Birim, istek.Eko, istek.Fin))
                return Results.Json(new { basarili = false, hata = "Lütfen seçim yapınız!" });

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
                tg.kod1 = Kirp(istek.Birim);
                tg.kod2 = Kirp(istek.Eko);
                tg.kod3 = Kirp(istek.Fin);
                if (!KodlarGecerli(tg.kod1, tg.kod2, tg.kod3)) return Results.Json(new { basarili = false, hata = "Kod hatalı!" });

                int durum = DurumOku(kullanan, istek.Yil, tg.kod1, tg.kod2, tg.kod3);
                if (durum < 0) return Results.Json(new { basarili = false, hata = "Onay durumu okunamadı." });
                if ((durum == 6 || istek.Islem == "ac") && !GenelGorevUclari.YetkiVar(kullanan))
                    return Results.StatusCode(StatusCodes.Status403Forbidden);

                tg.durum = yeniDurum;
                Sonuc sonuc = TNS.BHMv2.Arac.Tanimla().BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurYatirimVize, tg);
                if (!sonuc.islemSonuc) return Results.Json(new { basarili = false, hata = GorusmeCariUclari.HataMetni(sonuc.hataStr, "İşlem başarısız.") });
                return Results.Json(new { basarili = true, bilgi = "İşlem tamamlandı.", durum = yeniDurum });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        /// <summary>Toplu Kapatma (eski btnTopluKapat_Click): yılın kapalı olmayan bütün vize tavanları girişe kapatılır (6).</summary>
        private static async Task<IResult> TopluKapat(HttpContext baglam, IWebHostEnvironment ortam, YatirimVizeTopluIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Yil < 2000) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });

            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                TavanGenel kistas = new TavanGenel();
                kistas.kod1 = "";
                kistas.kod2 = "";
                kistas.kod3 = "";
                ObjectArray oa = servis.BHMTavanGenelOnayListele(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurYatirimVize, kistas);
                if (oa == null || !oa.sonuc.islemSonuc)
                    return Results.Json(new { basarili = false, hata = GorusmeCariUclari.HataMetni(oa == null ? "" : oa.sonuc.hataStr, "Onay durumları okunamadı.") });

                int kapanan = 0;
                List<string> kapanmayan = new List<string>();
                foreach (TavanGenel item in oa.objeler)
                {
                    if (item.durum >= 6) continue;
                    item.durum = 6;
                    Sonuc sonuc = servis.BHMTavanGenelOnayKaydet(kullanan, istek.Yil, Senaryo, ENUMTavanTur.tavanTurYatirimVize, item);
                    if (sonuc.islemSonuc) kapanan++;
                    else kapanmayan.Add(item.kod1 + "-" + item.kod2 + "-" + item.kod3);
                }

                if (kapanmayan.Count > 0)
                    return Results.Json(new { basarili = false, hata = "Şu tertipler veri girişine kapatılamadı: " + string.Join(", ", kapanmayan) });
                return Results.Json(new { basarili = true, bilgi = kapanan == 0 ? "Kapatılacak tavan yok." : kapanan + " tavan veri girişine kapatıldı." });
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "İşlem başarısız." });
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, string eko, string fin)
        {
            return YazdirOrtak(baglam, ortam, yil, birim, eko, fin, false);
        }

        private static IResult YazdirDetay(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, string eko, string fin)
        {
            return YazdirOrtak(baglam, ortam, yil, birim, eko, fin, true);
        }

        /// <summary>
        /// Yazdır / Detay Yaz (eski btnYazdir_Click / btnYazdirDetay_Click): seçilen ölçütlerle (boş olanlar süzülmez) vize tavanları
        /// (şablonlar BHMv2\YatirimVizeTavan.xltx ve YatirimVizeTavanDetay.xltx).
        /// </summary>
        private static IResult YazdirOrtak(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, string eko, string fin, bool detay)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            birim = Kirp(birim); eko = Kirp(eko); fin = Kirp(fin);
            if (!KodlarGecerli(birim, eko, fin)) return Results.Json(new { hata = "Kod hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                string sablon = Ayar.Oku("RaporSablonYol");
                string dosya = detay
                    ? TavanRaporlari.YatirimVizeYazdirDetay(servis, kullanan, yil, birim, eko, fin, sablon)
                    : TavanRaporlari.YatirimVizeYazdirOzet(servis, kullanan, yil, birim, eko, fin, sablon);
                return DosyaIslem.DosyaGonder(dosya, (detay ? "VizeTavanDetay_" : "VizeTavan_") + DosyaIslem.DosyaAdUretSade(), true, "xlsx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        /// <summary>Rapor Al menüsü (eski btnRaporAl_Click → BHMv2Ortak.RaporAl(.., "1000", varsayılan kurum, "", fin, eko, birim)).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string birim, string eko, string fin, string rapor)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil < 2000) return Results.Json(new { hata = "Yıl hatalı!" });
            birim = Kirp(birim); eko = Kirp(eko); fin = Kirp(fin);
            if (!KodlarGecerli(birim, eko, fin)) return Results.Json(new { hata = "Kod hatalı!" });
            if (!GorusmeRaporlari.Tanimli(rapor, EkRaporlar)) return Results.Json(new { hata = "Rapor seçimi hatalı!" });
            try
            {
                AsposeLisans.Yukle();
                string dosya = GorusmeRaporlari.Al(kullanan, rapor, yil, RaporSenaryo, TNS.DEG.Arac.VarsayilanKurumBul(), "", fin, eko, birim, Ayar.Oku("RaporSablonYol"));
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
