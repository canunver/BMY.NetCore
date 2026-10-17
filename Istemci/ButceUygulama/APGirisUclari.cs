using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
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
using TNS.BUM;
using TNS.KYM;

namespace Istemci.ButceUygulama
{
    /// <summary>Tablodaki bir satır (eski AHPDagilim; alan adları eski Ext store'undaki gibi).</summary>
    public class APDagilim
    {
        public string tertip { get; set; }
        public double odenek { get; set; }
        public double a1 { get; set; }
        public double a2 { get; set; }
        public double a3 { get; set; }
        public double a4 { get; set; }
        public double a5 { get; set; }
        public double a6 { get; set; }
        public double a7 { get; set; }
        public double a8 { get; set; }
        public double a9 { get; set; }
        public double a10 { get; set; }
        public double a11 { get; set; }
        public double a12 { get; set; }

        public double Ay(int ay)
        {
            switch (ay)
            {
                case 1: return a1; case 2: return a2; case 3: return a3; case 4: return a4;
                case 5: return a5; case 6: return a6; case 7: return a7; case 8: return a8;
                case 9: return a9; case 10: return a10; case 11: return a11; default: return a12;
            }
        }
    }

    /// <summary>Sayfanın seçim alanları (yıl, aşama, kurumsal / fonksiyonel / finans / ekonomik / faaliyet kodları).</summary>
    public class APSecim
    {
        public string Tur { get; set; }
        public int Yil { get; set; }
        public int Asama { get; set; }
        public string Kur { get; set; }
        public string Fon { get; set; }
        public string Fin { get; set; }
        public string Eko { get; set; }
        public string Peb { get; set; }
    }

    public class APKayitIstegi : APSecim
    {
        public List<APDagilim> Satirlar { get; set; }
    }

    public class APIndirIstegi
    {
        public string Tur { get; set; }
        public int Sade { get; set; }
        public List<APDagilim> Satirlar { get; set; }
    }

    public class APSbbIstegi : APSecim
    {
        /// <summary>4: faaliyet, 5: alt faaliyet.</summary>
        public int Seviye { get; set; }
    }

    public class APImza
    {
        public string HazGorev { get; set; }
        public string HazKisi { get; set; }
        public string ImzGorev { get; set; }
        public string ImzKisi { get; set; }
    }

    /// <summary>
    /// AHP / AGP Girişi sayfasının (eski ButceUygulama/APGiris.aspx?tur=gider | gelir) JavaScript'ten çağırdığı uçlar. tur=gider ayrıntılı
    /// harcama programı (TABPEBOdenek), tur=gelir ayrıntılı gelir programı (TABGelir). Yetki menüdeki ilgili seçenekten gelir.
    ///   GET  /api/apgiris/tanim?tur=                yıllar, aşamalar, kayıtlı arama tertibi, kilitli ay, yönetici mi
    ///   GET  /api/apgiris/kodlar?tur=&amp;yil=&amp;kod=&amp;ust=   kod ağacı (eski nodeLoadPar: 1, 2, 3, 4 / gelirde 5, 14)
    ///   GET  /api/apgiris/liste?...                 (eski btnListele_Click: APListele + toplam satırı)
    ///   POST /api/apgiris/kaydet                    (eski btnKaydet_Click: APKaydet)
    ///   POST /api/apgiris/onayla, /onaykaldir       (eski btnOnay_Click, btnOnayKaldir_Click)
    ///   GET  /api/apgiris/oncekioran?...            (eski btnOranGetir_Click: bir önceki yılın onaylı AHP oranları)
    ///   POST /api/apgiris/aykilitle                 (eski btnAyKilitle_Click; sistem yöneticisi)
    ///   POST /api/apgiris/indir                     (eski btnDosyaSakla_Click: tablo Excel'e, şablonlu ya da sade)
    ///   POST /api/apgiris/yukle                     (eski btnDosyaYukle_Click: Excel'den tabloya)
    ///   POST /api/apgiris/eko2yukle                 (eski btnDosyaAHP2Duzey_Click: Eko 2 düzeyli AHP dosyasını önceki aşamaya göre dağıtıp kaydeder)
    ///   GET  /api/apgiris/rapor?...                 (eski btnListe_Click: Ek 2, Ek 2 Detay, Özet 1A / 1B, İcmal-Detay, Ek 2 Tertip)
    ///   GET  /api/apgiris/prgindir?...              (eski btnPrgIndir: program yapısında AHP, faaliyet / alt faaliyet; yalnızca gider)
    ///   POST /api/apgiris/prggonder, /prgal         (eski btnPrgGonder, btnPrgAl: SBB'ye gönder / SBB'den al; sistem yöneticisi, gider)
    ///   GET/POST /api/apgiris/imza                  (eski ButceHazirlikv2/MasrafPlaniGenelImza.aspx: rapor imzaları, "MasrafPlaniImza" değişkeni)
    /// İş kuralları eskisi gibi TNS.BUM / TNS.BHMv2 / TNS.KYM servislerinden çağrılır.
    /// </summary>
    public static class APGirisUclari
    {
        public const string GiderAdres = "../ButceUygulama/APGiris.aspx?tur=gider";
        public const string GelirAdres = "../ButceUygulama/APGiris.aspx?tur=gelir";

        private static readonly string[] AyAdlari = { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/apgiris/tanim", Tanim);
            app.MapGet("/api/apgiris/kodlar", Kodlar);
            app.MapGet("/api/apgiris/liste", Liste);
            app.MapPost("/api/apgiris/kaydet", Kaydet);
            app.MapPost("/api/apgiris/onayla", Onayla);
            app.MapPost("/api/apgiris/onaykaldir", OnayKaldir);
            app.MapGet("/api/apgiris/oncekioran", OncekiOran);
            app.MapPost("/api/apgiris/aykilitle", AyKilitle);
            app.MapPost("/api/apgiris/indir", Indir);
            app.MapPost("/api/apgiris/yukle", Yukle).DisableAntiforgery();
            app.MapPost("/api/apgiris/eko2yukle", Eko2Yukle).DisableAntiforgery();
            app.MapGet("/api/apgiris/rapor", Rapor);
            app.MapGet("/api/apgiris/prgindir", PrgIndir);
            app.MapPost("/api/apgiris/prggonder", PrgGonder);
            app.MapPost("/api/apgiris/prgal", PrgAl);
            app.MapGet("/api/apgiris/imza", ImzaOku);
            app.MapPost("/api/apgiris/imza", ImzaKaydet);
        }

        public static bool GelirMi(string tur)
        {
            return string.Equals((tur ?? "").Trim(), "gelir", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Eski ENUMTabloKod_TABPEBOdenek: tur=gelir ise TABGelir, değilse TABPEBOdenek.</summary>
        private static ENUMTabloKod TabloKod(string tur)
        {
            return GelirMi(tur) ? ENUMTabloKod.TABGelir : ENUMTabloKod.TABPEBOdenek;
        }

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, string tur)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, GelirMi(tur) ? GelirAdres : GiderAdres);
        }

        private static bool Yonetici(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, string tur, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath, tur)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>İki sayfadan birine yetkisi olan (imza penceresi gibi ortak uçlar).</summary>
        private static IResult DenetleOrtak(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            string kok = ortam.ContentRootPath;
            if (!YetkiVar(kullanan, kok, "gider") && !YetkiVar(kullanan, kok, "gelir")) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Bilgi(string bilgi) { return Results.Json(new { bilgi = bilgi }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Eski TiredenAl: "13.01.31.00- Ad" -> "13.01.31.00".</summary>
        private static string KodAl(string p)
        {
            if (string.IsNullOrWhiteSpace(p)) return "";
            return p.Split('-')[0].Trim();
        }

        private static string Asama(int asama)
        {
            return asama == 1 ? "Birim Teklifi" : asama == 2 ? "Kurum Teklifi" : asama == 3 ? "Onaylı AHP" : asama.ToString();
        }

        // ------------------------------------------------------------------ açılış ve kod ağacı

        /// <summary>Açılış (eski Page_Load): yıllar (bu yıl - 5 .. bu yıl + 1), aşamalar, kayıtlı arama tertibi, kilitli ay.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, string tur)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int yil = DateTime.Now.Year;
                List<int> yillar = new List<int>();
                for (int y = yil + 1; y >= yil - 5; y--) yillar.Add(y);

                // Eski KullaniciDegiskenDoldur: son listelenen tertip ("aramaTertip").
                string kayitli = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "aramaTertip");
                Tertip t = new Tertip(yil, TabloKod(tur));
                string kur = "", fon = "", fin = "", eko = "", peb = "";
                try
                {
                    if (!string.IsNullOrWhiteSpace(kayitli)) t.TertipAta(kayitli);
                    kur = t.kodlar[0] ?? "";
                    if (GelirMi(tur))
                        eko = t.kodlar[1] ?? "";
                    else
                    {
                        fon = t.kodlar[1] ?? ""; fin = t.kodlar[2] ?? ""; eko = t.kodlar[3] ?? ""; peb = t.kodlar[4] ?? "";
                    }
                }
                catch (Exception) { }

                return Results.Json(new
                {
                    gelir = GelirMi(tur),
                    yil = yil,
                    yillar = yillar,
                    asama = 1,
                    asamalar = new[] { new { kod = 1, ad = Asama(1) }, new { kod = 2, ad = Asama(2) }, new { kod = 3, ad = Asama(3) } },
                    kur = kur, fon = fon, fin = fin, eko = eko, peb = peb,
                    kilitliAy = OrtakFonksiyonlar.ConvertToInt(GenelIslemler.KullaniciDegiskenGetir("stratek", "AHPKilitliAy"), 0),
                    yonetici = Yonetici(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Kod ağacı (eski nodeLoadPar / nodeLoadPar3; yetki süzmesi olmadan). Gelir sayfasında ekonomik kod yerine gelir kodu (5).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, string tur, int yil, string kod, string ust)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod);
            if (kod != "1" && kod != "2" && kod != "3" && kod != "4" && kod != "5" && kod != "14") return Hata("Kod türü hatalı.");
            return GorusmeCariUclari.KodDuzeyi(kullanan, yil, kod, ust, false);
        }

        // ------------------------------------------------------------------ listele / kaydet

        private static Tertip KistasTertip(APSecim s, bool varsayilanKurum)
        {
            string birimKurum = KodAl(s.Kur);
            if (varsayilanKurum && string.IsNullOrWhiteSpace(birimKurum))
                birimKurum = TNS.DEG.Arac.VarsayilanKurumBul();

            Tertip tertip = new Tertip(s.Yil, TabloKod(s.Tur), birimKurum);
            if (tertip.tabloKod == ENUMTabloKod.TABPEBOdenek)
            {
                tertip.kodlar[1] = KodAl(s.Fon);
                tertip.kodlar[2] = KodAl(s.Fin);
                tertip.kodlar[3] = KodAl(s.Eko);
                tertip.kodlar[4] = KodAl(s.Peb);
            }
            else
            {
                tertip.kodlar[1] = KodAl(s.Eko);
            }
            return tertip;
        }

        private static object Satir(string tertip, string tertipAdi, double[] o, bool toplam)
        {
            return new
            {
                tertip = tertip,
                tertipAdi = tertipAdi,
                toplam = toplam,
                odenek = o[0], serbest = o[1],
                a1 = o[2], a2 = o[3], a3 = o[4], a4 = o[5], a5 = o[6], a6 = o[7],
                a7 = o[8], a8 = o[9], a9 = o[10], a10 = o[11], a11 = o[12], a12 = o[13]
            };
        }

        private static double[] OnDort(double[] kaynak)
        {
            double[] d = new double[14];
            if (kaynak != null) Array.Copy(kaynak, d, Math.Min(14, kaynak.Length));
            return d;
        }

        /// <summary>Listele (eski APListele): tertiplerin KBÖ, serbest ve 12 ay dağılımı, en altta toplam satırı.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string tur, int yil, int asama, string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            APSecim s = new APSecim { Tur = tur, Yil = yil > 0 ? yil : DateTime.Now.Year, Asama = asama > 0 ? asama : 1, Kur = kur, Fon = fon, Fin = fin, Eko = eko, Peb = peb };
            try
            {
                Tertip tertip = KistasTertip(s, true);
                TertipOdenek[] degerler = TNS.BUM.Arac.Tanimla().APListele(kullanan, s.Asama, tertip);

                // Eski KullaniciDegiskenSakla: son listelenen tertip.
                try
                {
                    TNSCollection tc = new TNSCollection();
                    tc.Add(tertip.TertipStrOl());
                    kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), "aramaTertip", tc);
                }
                catch (Exception) { }

                List<object> satirlar = new List<object>();
                double[] toplam = new double[14];
                if (degerler != null)
                    foreach (TertipOdenek tb in degerler)
                    {
                        double[] o = OnDort(tb.odenek);
                        for (int i = 0; i < 14; i++) toplam[i] += o[i];
                        satirlar.Add(Satir(tb.tertip.TertipStrOl(), tb.tertip.tertipAdi, o, false));
                    }
                return Results.Json(new { satirlar = satirlar, toplam = Satir("", "T O P L A M", toplam, true) });
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Listeleme yapılamadı." : ex.Message);
            }
        }

        /// <summary>Kaydet (eski APKaydet): tertibi boş olmayan satırların 12 aylık dağılımı kaydedilir.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APKayitIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new APKayitIstegi();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil <= 0 || istek.Asama <= 0) return Hata("Yıl ve Aşama Bilgileri Boş Geçilemez.");
            try
            {
                List<TertipOdenek> ahpSatirlar = new List<TertipOdenek>();
                foreach (APDagilim d in istek.Satirlar ?? new List<APDagilim>())
                {
                    if (d == null || string.IsNullOrEmpty(d.tertip)) continue;
                    TertipOdenek tb = new TertipOdenek(12);
                    tb.tertip = new TNS.BKD.Tertip(istek.Yil, TabloKod(istek.Tur));
                    tb.tertip.TertipAta(d.tertip);
                    for (int ay = 1; ay <= 12; ay++) tb.odenek[ay - 1] = d.Ay(ay);
                    ahpSatirlar.Add(tb);
                }

                Sonuc sonuc = TNS.BUM.Arac.Tanimla().APKaydet(kullanan, istek.Yil, KodAl(istek.Kur), istek.Asama, ahpSatirlar.ToArray());
                if (sonuc != null && sonuc.islemSonuc)
                    return Results.Json(new { basarili = true, bilgi = "Kaydetme İşlemi Başarı ile tamamlanmıştır" });
                return Results.Json(new { basarili = false, hata = "Kaydetme İşlemi tamamlanamamıştır." + (sonuc?.hataStr ?? "") });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = "Kaydetme İşlemi tamamlanamamıştır." + ex.Message });
            }
        }

        private static async Task<IResult> Onayla(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APSecim istek)
        {
            return await OnayIslem(baglam, ortam, sahtecilik, istek, true);
        }

        private static async Task<IResult> OnayKaldir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APSecim istek)
        {
            return await OnayIslem(baglam, ortam, sahtecilik, istek, false);
        }

        /// <summary>Onayla / Onay Kaldır (eski btnOnay_Click, btnOnayKaldir_Click): kurum boşsa varsayılan kurum.</summary>
        private static async Task<IResult> OnayIslem(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APSecim istek, bool onayla)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new APSecim();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek.Yil <= 0 || istek.Asama <= 0) return Hata("Yıl ve Aşama Bilgileri Boş Geçilemez.");
            try
            {
                string kurumBirim = KodAl(istek.Kur);
                if (string.IsNullOrEmpty(kurumBirim))
                    kurumBirim = TNS.DEG.Arac.VarsayilanKurumBul();
                IBUMServis bumServis = TNS.BUM.Arac.Tanimla();
                Sonuc sonuc = onayla ? bumServis.OnaylaAP(kullanan, istek.Yil, kurumBirim, istek.Asama)
                                     : bumServis.OnayKaldirAP(kullanan, istek.Yil, kurumBirim, istek.Asama);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(sonuc?.hataStr ?? "İşlem yapılamadı.");
                return Bilgi(onayla ? "Bilgiler başarıyla onaylandı." : "Onay işlemi başarıyla kaldırılmıştır.");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Önc. Yıl Oran (eski btnOranGetir_Click): tertibin bir önceki yıl onaylı AHP'sinin aylık oranları (aşağı yuvarlanmış).</summary>
        private static IResult OncekiOran(HttpContext baglam, IWebHostEnvironment ortam, string tur, int yil, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            tertip = Temiz(tertip);
            if (tertip == "") return Hata("Bu işlemi yapmadan önce listeleme yaparak ekrana tertip getiriniz!");
            try
            {
                Tertip t = new Tertip(yil - 1, TabloKod(tur));
                t.TertipAta(tertip);
                TertipOdenek to = TNS.BUM.Arac.Tanimla().APOku(kullanan, yil - 1, 3, t);
                if (to == null) return Hata(string.Format("{0} tertibinin bir öceki yıl AHP bilgisi bulunamadı!", tertip));
                if (!(to.odenek[0] > 0)) return Hata(string.Format("{0} tertibinin bir öceki yıl KBÖ bilgisi bulunamadı!", tertip));
                double[] oranlar = new double[12];
                for (int ay = 0; ay < 12; ay++)
                    oranlar[ay] = Math.Floor((to.odenek[ay + 2] * 100) / to.odenek[0]);
                return Results.Json(new { oranlar = oranlar });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        public class AyKilitIstegi
        {
            public string Tur { get; set; }
            public int Ay { get; set; }
        }

        /// <summary>Ay kilitle (eski btnAyKilitle_Click; yalnızca sistem yöneticisi): verilen aya kadar olan aylar girişe kapanır.</summary>
        private static async Task<IResult> AyKilitle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, AyKilitIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new AyKilitIstegi();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek.Ay < 0 || istek.Ay > 12) return Hata("Ay 0 ile 12 arasında olmalı.");
            Sonuc s = GenelIslemler.KullaniciDegiskenSakla(new TNS.KYM.Kullanici { kullaniciKodu = "stratek" }, "AHPKilitliAy", istek.Ay.ToString());
            if (s == null || !s.islemSonuc) return Hata(s?.hataStr ?? "Kaydedilemedi.");
            return Bilgi(istek.Ay + ". aya kadar olan aylar, veri girişine kapatıldı!");
        }

        // ------------------------------------------------------------------ dosya işlemleri

        /// <summary>İndir / İndir (Sade) (eski btnDosyaSakla_Click): tablodaki satırlar APGiris.XLT şablonuna ya da boş Excel'e yazılır.</summary>
        private static async Task<IResult> Indir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APIndirIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new APIndirIstegi();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                AsposeLisans.Yukle();
                int sade = istek.Sade == 1 ? 1 : 0;
                Tablo XLS = GenelIslemler.NewTablo();
                string dosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                int satir = 0;
                int sutun = 0;
                if (sade == 1)
                {
                    XLS.BosDosyaAc(dosyaAd);
                }
                else
                {
                    string sablon = ButceUygulamaRaporlari.SablonBul(ortam.ContentRootPath, "APGiris.XLT");
                    if (sablon == null) return Hata("Rapor şablonu bulunamadı (BUM\\APGiris.XLT).");
                    XLS.DosyaAc(sablon, dosyaAd);
                    XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                }
                int[] kolonlarSadeDegil = { 2, 4, 6, 10, 12, 14, 18, 20, 22, 26, 28, 30 };
                int[] kolonlarSade = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                int[] kolonlar = sade == 1 ? kolonlarSade : kolonlarSadeDegil;
                List<APDagilim> dagilim = istek.Satirlar ?? new List<APDagilim>();
                int yuks = XLS.SatirYukseklikAl(satir);
                int kaynakSatir = satir - 1;
                foreach (APDagilim d in dagilim)
                {
                    if (d == null) continue;
                    if (sade == 0)
                        XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 38, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, d.tertip ?? "");

                    if (sade == 0)
                        XLS.HucreDegerYaz(satir, 1, d.odenek);
                    for (int ay = 1; ay <= 12; ay++)
                        XLS.HucreDegerYaz(satir, kolonlar[ay - 1], d.Ay(ay));
                    if (sade == 1)
                        XLS.HucreDegerYaz(satir, 13, d.odenek);
                    satir++;
                }
                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir - 1, yuks);
                if (sade == 1)
                {
                    XLS.SutunGenislikAyarla(0, 0, 34);
                    XLS.SutunGenislikAyarla(1, 13, 14);
                }
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, "APGiris_" + DateTime.Now.Ticks.ToString() + "." + GenelIslemler.ExcelTur(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Liste excel dosyası olarak oluşturulamadı.");
            }
        }

        /// <summary>Gelen Excel dosyasını geçici dosyaya yazar (eski fuListe.PostedFile.SaveAs).</summary>
        private static async Task<string> DosyaAl(IFormFile dosya)
        {
            string uzanti = Path.GetExtension(dosya.FileName ?? "").ToLowerInvariant();
            if (uzanti != ".xls" && uzanti != ".xlsx") uzanti = "." + GenelIslemler.ExcelTur();
            string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + uzanti;
            using (FileStream hedef = File.Create(dosyaAdxls))
                await dosya.CopyToAsync(hedef);
            return dosyaAdxls;
        }

        private static IResult DosyaDenetle(IFormFile dosya)
        {
            if (dosya == null) return Hata("Yüklenilecek dosya gelmedi!");
            if (dosya.Length == 0) return Hata("Yüklemek istediğiniz dosyanın içi boş.");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");
            return null;
        }

        /// <summary>
        /// Yükle (eski btnDosyaYukle_Click): 1. satırdan başlayarak ilk sütun tertip, sonraki 12 sütun ay değerleri; tertip boş olan ilk
        /// satırda durulur. Satırlar tabloya gelir (kaydedilmez).
        /// </summary>
        private static async Task<IResult> Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            IResult ret = Denetle(baglam, ortam, form["tur"], out Kullanici kullanan);
            if (ret != null) return ret;
            IFormFile dosya = form.Files["dosya"];
            ret = DosyaDenetle(dosya);
            if (ret != null) return ret;
            string dosyaAdxls = null;
            try
            {
                dosyaAdxls = await DosyaAl(dosya);
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                List<object> al = new List<object>();
                try
                {
                    for (int sat = 0; ; sat++)
                    {
                        string tertip = XLS.HucreDegerAl(sat, 0);
                        if (string.IsNullOrWhiteSpace(tertip)) break;
                        double[] o = new double[14];
                        for (int ay = 1; ay <= 12; ay++) o[ay + 1] = XLS.HucreDegerAlDbl(0, sat, ay);
                        al.Add(Satir(tertip.Trim(), "", o, false));
                    }
                }
                finally
                {
                    XLS.DosyaKapat();
                }
                return Results.Json(new { satirlar = al, bilgi = "Dosya yükleme işlemi tamamlandı!" });
            }
            catch (Exception)
            {
                return Hata("Yüklenmesi için belirtilen dosya yüklenemedi.");
            }
            finally
            {
                if (dosyaAdxls != null) try { File.Delete(dosyaAdxls); } catch { }
            }
        }

        /// <summary>
        /// Eko 2 AHP (eski btnDosyaAHP2Duzey_Click): dosyada 2. düzey ekonomik kodlu tertipler ve 12 ay değeri vardır. Her tertibin alt
        /// tertipleri bir önceki aşamadaki dağılımlarının oranıyla bölüştürülür (10'a yuvarlanır, kalan son satıra), Aralık KBÖ'den kalandır;
        /// eksiye düşen ay başka satırlarla takasla düzeltilir. Dağıtılan tertipler seçili aşamaya kaydedilir.
        /// </summary>
        private static async Task<IResult> Eko2Yukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            string tur = form["tur"];
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            int asama = OrtakFonksiyonlar.ConvertToInt((string)form["asama"], 3);
            if (asama < 2)
                return Hata("Bu işlem için kaynak aşama, bir önceki aşama olduğundan, birinci aşama seçili iken bu işlem yapılamaz");
            int secYil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], DateTime.Now.Year);
            string kurumBirim = KodAl(form["kur"]);
            IFormFile dosya = form.Files["dosya"];
            ret = DosyaDenetle(dosya);
            if (ret != null) return ret;

            string genelMesaj = "";
            string dosyaAdxls = null;
            try
            {
                dosyaAdxls = await DosyaAl(dosya);
            }
            catch (Exception)
            {
                return Hata("Yüklenmesi için belirtilen dosya yüklenemedi.");
            }
            try
            {
                AsposeLisans.Yukle();
                IBUMServis bumServis = TNS.BUM.Arac.Tanimla();
                Tertip tertip = new Tertip(secYil, TabloKod(tur));
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                int sat = 0;
                while (true)
                {
                    bool tertipHata = false;
                    string tertipStr = XLS.HucreDegerAl(sat, 0);
                    if (string.IsNullOrEmpty(tertipStr)) break;
                    double[] ayDegerler = new double[12];
                    for (int ay = 0; ay < 12; ay++)
                    {
                        ayDegerler[ay] = XLS.HucreDegerAlDbl(0, sat, ay + 1);
                    }
                    sat++;
                    try
                    {
                        tertip.TertipAta(tertipStr);
                        TertipOdenek[] degerler = bumServis.APListele(kullanan, asama - 1, tertip);
                        if (degerler != null && degerler.Length > 0)
                        {
                            TertipOdenek[] yeniDegerler = new TertipOdenek[degerler.Length];
                            for (int i = 0; i < yeniDegerler.Length; i++)
                            {
                                yeniDegerler[i] = new TertipOdenek(12);
                                yeniDegerler[i].tertip = new Tertip(tertip.yil, tertip.tabloKod);
                                yeniDegerler[i].tertip.TertipAta(degerler[i].tertip);
                            }
                            for (int ay = 0; ay < 11; ay++)
                            {
                                double top = 0;
                                for (int i = 0; i < yeniDegerler.Length; i++)
                                {
                                    top += degerler[i].odenek[ay + 2];
                                }
                                double oran = 1;
                                if (ayDegerler[ay] != top)
                                    if (top <= 0)
                                    {
                                        tertipHata = true;
                                        genelMesaj += "\n" + string.Format("{0} tertibinde, {1} ay için kayıtlı ödenek yokken, verilen dosyada ödenek ({2}) var. İşlem yapılamıyor. Rakamları güncelleyip tekrar deneyiniz.!", tertip.TertipStrOl(), ay + 1, ayDegerler[ay].ToString("#,##0"));
                                        continue;
                                    }
                                    else
                                        oran = ayDegerler[ay] / top;
                                top = 0;
                                for (int i = 0; i < yeniDegerler.Length - 1; i++)
                                {
                                    yeniDegerler[i].odenek[ay] = Math.Floor(degerler[i].odenek[ay + 2] * oran / 10) * 10;
                                    top += yeniDegerler[i].odenek[ay];
                                }
                                yeniDegerler[yeniDegerler.Length - 1].odenek[ay] = ayDegerler[ay] - top;
                            }
                            if (!tertipHata)
                            {
                                for (int i = 0; i < yeniDegerler.Length; i++)
                                {
                                    yeniDegerler[i].odenek[11] = degerler[i].odenek[0] - Topla(yeniDegerler[i].odenek, 0, 11);
                                }
                            }
                            string uyari = "";
                            for (int i = 0; i < yeniDegerler.Length; i++)
                            {
                                if (yeniDegerler[i].odenek[11] < 0)
                                {
                                    if (!HucreKurtar(yeniDegerler, i, 11))
                                        uyari = tertip.TertipStrOl() + " dağıtılamadı, elle dağıtınız!";
                                }
                            }

                            if (string.IsNullOrEmpty(uyari))
                            {
                                Sonuc sonuc = bumServis.APKaydet(kullanan, secYil, kurumBirim, asama, yeniDegerler);
                                if (!sonuc.islemSonuc)
                                {
                                    genelMesaj += "\n" + tertip.TertipStrOl() + " dağıtılamadı:" + sonuc.hataStr;
                                }
                            }
                            else
                                genelMesaj += "\n" + uyari;
                        }
                        else
                            genelMesaj += "\n" + tertip.TertipStrOl() + " ile ilgili tertip bulunamadı!";
                    }
                    catch (Exception ex)
                    {
                        genelMesaj += "\n" + ex.Message;
                        break;
                    }
                }
                XLS.DosyaKapat();
            }
            catch (Exception ex)
            {
                genelMesaj += "\n" + ex.Message;
            }
            finally
            {
                try { File.Delete(dosyaAdxls); } catch { }
            }
            return Results.Json(new { bilgi = "İşlem tamamlandı." + (genelMesaj == "" ? "" : " Uyarılar:" + genelMesaj), uyariVar = genelMesaj != "" });
        }

        private static bool HucreKurtar(TertipOdenek[] yeniDegerler, int sat, int ay)
        {
            double fazla = yeniDegerler[sat].odenek[ay];
            if (fazla >= 0) return true;
            fazla = 0 - fazla;
            for (int i = 1; i < 12; i++)
            {
                int yeniAy = ay - i;
                if (yeniAy < 0) yeniAy += 12;
                for (int yeniSat = 0; yeniSat < yeniDegerler.Length; yeniSat++)
                {
                    if (yeniSat != sat)
                    {
                        double islemMiktar = Math.Min(yeniDegerler[sat].odenek[yeniAy], yeniDegerler[yeniSat].odenek[ay]);
                        if (islemMiktar > 0)
                        {
                            if (islemMiktar >= fazla)
                            {
                                islemMiktar = fazla;
                            }
                            yeniDegerler[sat].odenek[yeniAy] -= islemMiktar;
                            yeniDegerler[yeniSat].odenek[ay] -= islemMiktar;
                            yeniDegerler[sat].odenek[ay] += islemMiktar;
                            yeniDegerler[yeniSat].odenek[yeniAy] += islemMiktar;
                            fazla -= islemMiktar;
                            if (fazla == 0) return true;
                        }
                    }
                }
            }

            return false;
        }

        private static double Topla(double[] dizi, int basla, int bit)
        {
            double top = 0;
            for (int i = basla; i < bit; i++)
            {
                top = top + dizi[i];
            }
            return top;
        }

        // ------------------------------------------------------------------ raporlar

        /// <summary>
        /// Rapor Hazırla (eski btnListe_Click): Liste (Ek 2), Liste2 (Ek 2 Detay), OzetA (Özet Dök-1A), OzetB (Özet Dök-1B), Kiyas (İcmal-Detay),
        /// Liste3 (Ek 2 Tertip). imzali=1 ise imzalı şablon; imzalar "MasrafPlaniImza" kullanıcı değişkeninden.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string tur, string rapor, int? imzali, int yil, int asama,
            string kur, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl veya Kurum alanlarından biri veya bir kaçı boş bırakılmış. Lütfen ilgili alanları kontrol edip tekrar deneyin.");
            if (asama <= 0) asama = 1;
            try
            {
                Hashtable raporParam = new Hashtable();
                raporParam.Add("ButceYil", yil.ToString());
                raporParam.Add("Asama", asama.ToString());
                raporParam.Add("AsamaAd", Asama(asama));
                raporParam.Add("KurKod", TNS.DEG.Arac.VarsayilanKurumBul());
                raporParam.Add("KurAd", GenelIslemlerIstemci.VarsayilanKurumBulAd());
                raporParam.Add("Birim", KodAl(kur));
                raporParam.Add("BirimAd", Temiz(kur));
                raporParam.Add("eko", KodAl(eko));
                raporParam.Add("peb", KodAl(peb));
                raporParam.Add("CiktiYeri", "EXCEL");
                raporParam.Add("AyNo", "1");
                raporParam.Add("TabloKod", ((int)TabloKod(tur)).ToString());
                raporParam.Add("Format", imzali == 1 ? "1" : "2");

                TNSCollection coll = null;
                try
                {
                    coll = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullanan.kullaniciKodu, "MasrafPlaniImza");
                }
                catch
                {
                }
                raporParam.Add("AdSoyadSol", CollectionDegerAl(coll, 1, ""));
                raporParam.Add("UnvanSol", CollectionDegerAl(coll, 0, ""));
                raporParam.Add("AdSoyadSag", CollectionDegerAl(coll, 3, ""));
                raporParam.Add("UnvanSag", CollectionDegerAl(coll, 2, ""));

                string ekBilgi;
                string raporAd;
                string secRapor = Temiz(rapor);
                if (secRapor == "Liste" || secRapor == "Liste3")
                {
                    ekBilgi = "2";
                    raporAd = secRapor == "Liste" ? "Ek 2" : "Ek 2 Tertip";
                    if (secRapor == "Liste3")
                        raporParam.Add("TertipTam", "1");
                }
                else if (secRapor == "Liste2")
                {
                    ekBilgi = "2_DETAY";
                    raporAd = "Ek 2 Detay";
                }
                else if (secRapor == "OzetA")
                {
                    ekBilgi = "1A_DETAY";
                    raporAd = "Özet Dök-1A";
                }
                else if (secRapor == "OzetB")
                {
                    ekBilgi = "1B_DETAY";
                    raporAd = "Özet Dök-1B";
                }
                else
                {
                    ekBilgi = "KIYAS";
                    raporAd = "Icmal-Detay";
                }

                AsposeLisans.Yukle();
                ButceUygulamaRaporlari bur = new ButceUygulamaRaporlari();
                string sonucDosyaAd = bur.RaporAl(kullanan, ortam.ContentRootPath, DosyaIslem.DosyaAdUret(), raporParam, ekBilgi);
                return DosyaIslem.DosyaGonder(sonucDosyaAd, raporAd + "_" + DateTime.Now.Ticks.ToString(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Rapor hazırlanamadı." : ex.Message);
            }
        }

        /// <summary>Eski GenelIslemler.CollectionDegerAl.</summary>
        private static string CollectionDegerAl(TNSCollection col, int indis, string varsayilan)
        {
            if (col != null && indis < col.Count && col[indis] != null)
                return col[indis].ToString();
            return varsayilan;
        }

        /// <summary>Program yapısında kıstas ve kod bilgileri (eski btnPrgIndir / btnPrgGonder'in ortak kısmı).</summary>
        private static OdenekBilgi[] ProgramDagilimi(Kullanici kullanan, APSecim s, int seviye)
        {
            int secYil = s.Yil > 0 ? s.Yil : DateTime.Now.Year;
            int secAsama = s.Asama > 0 ? s.Asama : 1;
            string birimKurum = KodAl(s.Kur);
            if (string.IsNullOrWhiteSpace(birimKurum))
                birimKurum = TNS.DEG.Arac.VarsayilanKurumBul();

            Tertip kistasTertip = new Tertip(secYil, ENUMTabloKod.TABPRGOdenek);
            kistasTertip.kodlar[5] = birimKurum;
            kistasTertip.kodlar[6] = KodAl(s.Fin);
            kistasTertip.kodlar[7] = KodAl(s.Eko);
            kistasTertip.kodlar[8] = KodAl(s.Peb);
            kistasTertip.kodlar[9] = KodAl(s.Fon);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            kodBilgileri.Add(new UcgenKodBilgi(37, 1));
            kodBilgileri.Add(new UcgenKodBilgi(33, 1));
            kodBilgileri.Add(new UcgenKodBilgi(34, 1));
            kodBilgileri.Add(new UcgenKodBilgi(35, 1));
            if (seviye == 5)
                kodBilgileri.Add(new UcgenKodBilgi(36, 1));
            kodBilgileri.Add(new UcgenKodBilgi(1, 2));
            kodBilgileri.Add(new UcgenKodBilgi(3, 1));
            kodBilgileri.Add(new UcgenKodBilgi(4, 2, seviye == 5));

            EkDegerParam[] ekBilgiler = new EkDegerParam[13];
            ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABPRGOdenek, 0, 0, "0600");
            for (int i = 1; i <= 12; i++)
            {
                ekBilgiler[i] = new EkDegerParam(ENUMTabloKod.TABPRGGIDERAP, 0, 0, secAsama.ToString() + ";" + i.ToString());
            }

            return TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, false, false);
        }

        /// <summary>Raporla AHP (Faal / Alt Faal) (eski btnPrgIndir): program yapısındaki AHP, APGiris.XLT şablonuna.</summary>
        private static IResult PrgIndir(HttpContext baglam, IWebHostEnvironment ortam, string tur, int seviye, int yil, int asama,
            string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (GelirMi(tur)) return Hata("Bu işlem yalnızca gider (AHP) için yapılır.");
            try
            {
                OdenekBilgi[] dagilim = ProgramDagilimi(kullanan, new APSecim { Tur = tur, Yil = yil, Asama = asama, Kur = kur, Fon = fon, Fin = fin, Eko = eko, Peb = peb }, seviye == 5 ? 5 : 4);
                string sablon = ButceUygulamaRaporlari.SablonBul(ortam.ContentRootPath, "APGiris.XLT");
                if (sablon == null) return Hata("Rapor şablonu bulunamadı (BUM\\APGiris.XLT).");
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                string dosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                XLS.DosyaAc(sablon, dosyaAd);

                int satir = 0;
                int sutun = 0;
                XLS.HucreAdAdresCoz("BaslaSatir", ref satir, ref sutun);
                int yuks = XLS.SatirYukseklikAl(satir);
                int kaynakSatir = satir - 1;
                int[] kolonNolar = { 1, 2, 4, 6, 10, 12, 14, 18, 20, 22, 26, 28, 30 };
                for (int i = 0; i < dagilim.Length; i++)
                {
                    XLS.HucreKopyala(kaynakSatir, sutun, kaynakSatir, sutun + 38, satir, sutun);
                    XLS.HucreDegerYaz(satir, 0, dagilim[i].tertip.TertipStrOl());
                    for (int j = 0; j < dagilim[i].ekRakamlar.Count && j < kolonNolar.Length; j++)
                    {
                        XLS.HucreDegerYaz(satir, kolonNolar[j], (double)(dagilim[i].ekRakamlar[j]));
                    }
                    satir++;
                }
                XLS.SatirYukseklikAyarla(kaynakSatir + 1, satir - 1, yuks);
                XLS.DosyaSaklaTamYol();
                return DosyaIslem.DosyaGonder(dosyaAd, "APGiris_" + DateTime.Now.Ticks.ToString() + "." + GenelIslemler.ExcelTur(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Liste excel dosyası olarak oluşturulamadı.");
            }
        }

        /// <summary>SBB e-bütçe kullanıcı adı ve parolası (eski KullaniciParolaBul).</summary>
        private static void KullaniciParolaBul(out string kullanici, out string parola)
        {
            Sonuc s = TNS.MUH.Arac.Tanimla().EbutceAktarimBilgiOku();
            kullanici = s.anahtar;
            parola = s.bilgiStr;
        }

        private static void Logla(string mesaj, bool yeni = false)
        {
            OrtakFonksiyonlar.HataDosyaYaz("AHPAktarim.txt", mesaj, yeni);
        }

        /// <summary>SBB'ye Gönder (Faal / Alt Faal) (eski btnPrgGonder; sistem yöneticisi): program yapısındaki AHP SBB servisine gönderilir.</summary>
        private static async Task<IResult> PrgGonder(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APSbbIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new APSbbIstegi();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (GelirMi(istek.Tur)) return Hata("Bu işlem yalnızca gider (AHP) için yapılır.");
            try
            {
                bool altFaal = istek.Seviye == 5;
                int secYil = istek.Yil > 0 ? istek.Yil : DateTime.Now.Year;
                OdenekBilgi[] dagilim = ProgramDagilimi(kullanan, istek, altFaal ? 5 : 4);
                OrtakFonksiyonlar.HataDosyaYaz("AHPGonder.txt", "Başla", true);
                List<SbbButceServisi.AhpSatiri> ahpler = new List<SbbButceServisi.AhpSatiri>();
                for (int i = 0; i < dagilim.Length; i++)
                {
                    SbbButceServisi.AhpSatiri oht = new SbbButceServisi.AhpSatiri();
                    oht.ProgramId = OrtakFonksiyonlar.ConvertToInt(dagilim[i].tertip.kodlar[1], 0);
                    oht.AltProgramId = OrtakFonksiyonlar.ConvertToInt(dagilim[i].tertip.kodlar[2], 0);
                    oht.FaaliyetId = OrtakFonksiyonlar.ConvertToInt(dagilim[i].tertip.kodlar[3], 0);
                    oht.AltFaaliyetId = altFaal ? OrtakFonksiyonlar.ConvertToInt(dagilim[i].tertip.kodlar[4], 0) : 0;
                    oht.KurKod = dagilim[i].tertip.kodlar[5];
                    oht.FinKod = dagilim[i].tertip.kodlar[6];
                    oht.EkoKod = dagilim[i].tertip.kodlar[7];
                    oht.ayDagilim = new double[12];
                    double toplam = 0;
                    for (int j = 1; j < dagilim[i].ekRakamlar.Count && j <= 12; j++)
                    {
                        double tut = (double)(dagilim[i].ekRakamlar[j]);
                        oht.ayDagilim[j - 1] = tut;
                        toplam += tut;
                    }
                    oht.bloke = OrtakFonksiyonlar.KurusYap((double)(dagilim[i].ekRakamlar[0]) - toplam);
                    ahpler.Add(oht);
                }

                KullaniciParolaBul(out string kullanici, out string parola);
                string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
                SbbButceServisi.WsSonuc sonuc = altFaal
                    ? await SbbButceServisi.AHPDetayKaydet(kullanici, parola, secYil, kurumKod, ahpler)
                    : await SbbButceServisi.AHPKaydet(kullanici, parola, secYil, kurumKod, ahpler);

                if (sonuc.islemSonuc) return Bilgi("Aktarıldı");
                return Hata(sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Hata("Hata: " + ex.Message + (ex.InnerException == null ? "" : " - İç: " + ex.InnerException.Message));
            }
        }

        /// <summary>
        /// SBB'den Al (Alt Faal) (eski btnPrgAl; sistem yöneticisi): ekonomik kodun 01..09 birinci düzeyleri için SBB'deki alt faaliyet
        /// AHP'si okunur, alt faaliyet program kodundan analitik koda çevrilir ve seçili aşamaya kaydedilir.
        /// </summary>
        private static async Task<IResult> PrgAl(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APSecim istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new APSecim();
            IResult ret = Denetle(baglam, ortam, istek.Tur, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (GelirMi(istek.Tur)) return Hata("Bu işlem yalnızca gider (AHP) için yapılır.");
            try
            {
                string kurumKod = TNS.DEG.Arac.VarsayilanKurumBul();
                int secYil = istek.Yil > 0 ? istek.Yil : DateTime.Now.Year;
                int secAsama = istek.Asama > 0 ? istek.Asama : 1;
                TNS.BHMv2.Arac.ProgramButceKodCevirSifirla(secYil);

                KullaniciParolaBul(out string kullanici, out string parola);
                List<TertipOdenek> ahpSatirlar = new List<TertipOdenek>();

                for (int ekoSira = 1; ekoSira < 10; ekoSira++)
                {
                    List<SbbButceServisi.AhpSatiri> sonucListe = await SbbButceServisi.AHPDetayListele(kullanici, parola, secYil, kurumKod, ekoSira.ToString("00"));
                    if (sonucListe == null || sonucListe.Count == 0) continue;

                    foreach (SbbButceServisi.AhpSatiri item in sonucListe)
                    {
                        string altFaalStr = item.AltFaaliyetId.ToString();
                        string afaal_fon = TNS.BHMv2.Arac.ProgramButceKodaCevir(secYil, 1200, altFaalStr);
                        if (string.IsNullOrWhiteSpace(afaal_fon))
                        {
                            Logla("Alt faaliyet çözümlenemedi:" + altFaalStr);
                            continue;
                        }
                        string[] a_f = afaal_fon.Split('.');
                        if (a_f.Length != 2)
                        {
                            Logla($"Alt faaliyet çözümü yanlış: {altFaalStr}: {afaal_fon}");
                            continue;
                        }

                        TertipOdenek tb = new TertipOdenek(12);
                        tb.tertip = new TNS.BKD.Tertip(secYil, TabloKod(istek.Tur));
                        tb.tertip.TertipAta($"{item.KurKod}-{a_f[1]}-{item.FinKod}-{item.EkoKod}-{a_f[0]}");
                        for (int i = 0; i < 12 && i < item.ayDagilim.Length; i++)
                        {
                            tb.odenek[i] = item.ayDagilim[i];
                        }
                        Logla($"Alt faaliyet çözümü yanlış: {item.ProgramId}.{item.AltProgramId}.{item.FaaliyetId}.{item.AltFaaliyetId}-{item.KurKod}-{item.FinKod}-{item.EkoKod}: {tb.tertip.TertipStrOl()}");
                        ahpSatirlar.Add(tb);
                    }
                }

                if (ahpSatirlar.Count == 0) return Hata("Liste boş geldi");
                Sonuc sonuc = TNS.BUM.Arac.Tanimla().APKaydet(kullanan, secYil, kurumKod, secAsama, ahpSatirlar.ToArray());
                if (sonuc.islemSonuc) return Bilgi("Veri aktarımı tamamlandı, rapor alıp kontrol ediniz!");
                return Hata("Aktarılan veriler kaydedilemedi:" + sonuc.hataStr);
            }
            catch (Exception ex)
            {
                return Hata("Hata: " + ex.Message + (ex.InnerException == null ? "" : " - İç: " + ex.InnerException.Message));
            }
        }

        // ------------------------------------------------------------------ imzalar (eski MasrafPlaniGenelImza.aspx)

        private static IResult ImzaOku(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = DenetleOrtak(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(ImzaBilgisi(kullanan));
        }

        private static async Task<IResult> ImzaKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APImza istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = DenetleOrtak(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Sonuc s = ImzaSakla(kullanan, istek);
            if (s != null && s.islemSonuc) return Bilgi("İşlem Başarılı");
            return Hata(s?.hataStr ?? "Kaydedilemedi.");
        }

        /// <summary>
        /// Kullanıcının rapor imzaları ("MasrafPlaniImza" değişkeni: hazırlayan görev / kişi, imzalayan görev / kişi). Masraf Planı
        /// ekranı (MasrafPlaniUclari) da imza penceresinde ve raporlarda bunu kullanır.
        /// </summary>
        public static APImza ImzaBilgisi(Kullanici kullanan)
        {
            TNSCollection coll = null;
            try
            {
                coll = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullanan.kullaniciKodu, "MasrafPlaniImza");
            }
            catch
            {
            }
            return new APImza
            {
                HazGorev = CollectionDegerAl(coll, 0, ""),
                HazKisi = CollectionDegerAl(coll, 1, ""),
                ImzGorev = CollectionDegerAl(coll, 2, ""),
                ImzKisi = CollectionDegerAl(coll, 3, "")
            };
        }

        /// <summary>Rapor imzalarını kullanıcının "MasrafPlaniImza" değişkenine yazar.</summary>
        public static Sonuc ImzaSakla(Kullanici kullanan, APImza istek)
        {
            istek ??= new APImza();
            TNSCollection coll = new TNSCollection();
            coll.Add(Temiz(istek.HazGorev));
            coll.Add(Temiz(istek.HazKisi));
            coll.Add(Temiz(istek.ImzGorev));
            coll.Add(Temiz(istek.ImzKisi));
            return kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), "MasrafPlaniImza", coll);
        }
    }
}
