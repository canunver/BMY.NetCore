using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.ButceUygulama;
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
using TNS.Raporlama;

namespace Istemci.ButceHazirlik
{
    /// <summary>Masraf planı ekranının seçimleri (yıl, aşama, kodlar, harcama birimi).</summary>
    public class MasrafPlaniSecim
    {
        public int Yil { get; set; }
        public int Asama { get; set; }
        public string Kur { get; set; }
        public string Fon { get; set; }
        public string Fin { get; set; }
        public string Eko { get; set; }
        public string Peb { get; set; }
        public string Ky { get; set; }
    }

    /// <summary>Tavandan MP'ye, MP Onayla (ay: Ocak / Şubat / Aralık).</summary>
    public class MasrafPlaniIslemIstegi : MasrafPlaniSecim
    {
        public int Ay { get; set; }
        /// <summary>Tavandan MP'ye (Tertip): yalnızca seçilen kodlara uyan tertipler.</summary>
        public bool Tertip { get; set; }
    }

    /// <summary>Kontrol Dönemi (eski MasrafPlaniKontrolDonem.aspx).</summary>
    public class MasrafPlaniKontrolDonem
    {
        public string Yil { get; set; }
        public string Ay { get; set; }
        public string HizYil { get; set; }
        public bool KontrolYap { get; set; }
        public string Tertipler { get; set; }
        /// <summary>1: dönem ve hizmet maliyeti yılı (eski btnKaydet), 2: ödenek kontrolü ve tertipler (eski btnKaydet2).</summary>
        public int Bolum { get; set; }
    }

    /// <summary>
    /// Masraf Planı sayfasının (eski ButceHazirlikv2/MasrafPlaniGenel.aspx, "Masraf Planı Veri Girişi") JavaScript'ten çağırdığı uçlar.
    ///   GET  /api/masrafplani/tanim                 aşamalar, son harcama birimi, yönetici mi
    ///   GET  /api/masrafplani/kodlar, /kodad         kod ağacı (1, 2, 3, 4, 14, 17) ve kod adı (1, 2, 3, 4, 58, 53)
    ///   GET  /api/masrafplani/liste?...              (eski btnListele_Click: MasrafPlaniBulTavan, aylık dağılım)
    ///   POST /api/masrafplani/tavandanmp             (eski btnTavandanMPye_Click / btnTavandanMPyeTertip_Click: MasrafPlaniGenelUretAHP)
    ///   POST /api/masrafplani/ahpdenmp               (eski btnAHPdenMPye_Click: Excel'deki tertiplerde AHP'den masraf planına)
    ///   POST /api/masrafplani/onayla, /onaykaldir    (eski btnOnayla_Click, btnOnayKaldir_Click)
    ///   POST /api/masrafplani/oncal                  (eski btnAsamaKopyala_Click "Önc. Al": önceki aşamadan bu aşamaya)
    ///   GET  /api/masrafplani/rapor?rapor=grup-no    (eski btnYazdir_Click: Rapor.xml 998 ve 5000-13 raporları)
    ///   GET  /api/masrafplani/hizli?cezali=0|1       (eski btnHizliOdGondUret_Click / btnHizliOdGondUretC_Click)
    ///   GET/POST /api/masrafplani/imza               (eski MasrafPlaniGenelImza.aspx; APGirisUclari.ImzaBilgisi / ImzaSakla)
    ///   GET/POST /api/masrafplani/kontroldonem       (eski MasrafPlaniKontrolDonem.aspx; sistem yöneticisi)
    /// İş kuralları eskisi gibi TNS.BHMv2 (IBHMv2Servis) servisinden çağrılır.
    /// </summary>
    public static class MasrafPlaniUclari
    {
        public const string Adres = "../ButceHazirlikv2/MasrafPlaniGenel.aspx";
        private const string KyDegisken = "BHMV2KURUMSALYAPIKOD";

        private static readonly ENUMMasrafPlanAsama[] Asamalar =
        {
            ENUMMasrafPlanAsama.URETIM, ENUMMasrafPlanAsama.TALEP, ENUMMasrafPlanAsama.SGB, ENUMMasrafPlanAsama.MERKEZ,
            ENUMMasrafPlanAsama.AHP, ENUMMasrafPlanAsama.SONDURUMONAYLI, ENUMMasrafPlanAsama.SONDURUMHEPSI
        };

        /// <summary>Rapor listesi (eski ddlRaporListe; değer Rapor.xml grup-no).</summary>
        private static readonly string[][] Raporlar =
        {
            new[] { "998-5", "Form 1: Birim Düzey Masra Planı İcmali" },
            new[] { "998-3", "Form 2/3: Merkez/İl/Birim Ekonomik-2 Düzey Masra Planı İcmali" },
            new[] { "998-2", "Merkez/İl/Birim Ekonomik-1 Düzey Masra Planı İcmali" },
            new[] { "998-4", "Merkez/İl/Birim ve Ekonomik-1 Düzey Masra Planı İcmali" },
            new[] { "998-1", "Masra Planı Ödenek Cetveli (4 Düzey)" },
            new[] { "998-30", "Gönderileni Fazla Olanlar" },
            new[] { "998-32", "Gönderileni Fazla Olanlar (eko2)" },
            new[] { "998-31", "Gönderileni Az Olanlar" },
            new[] { "998-33", "Gönderileni Az Olanlar (eko2)" },
            new[] { "998-34", "Masraf Planı ve Gönderilen" },
            new[] { "998-35", "Masraf Planı ve Gönderilen (eko2)" },
            new[] { "998-37", "Masraf Planı Hatalı Kayıtlar (eko2)" },
            new[] { "5000-13", "Saymanlık-SGB.net Harcama kontrol" },
            new[] { "998-47", "Detay Program- Ödenek İcmali (Prg)" },
            new[] { "998-48", "Detay Program- Ödenek İcmali (Eko)" },
            new[] { "998-49", "Detay Program- Ödenek İcmali (Eko2)" },
            new[] { "998-50", "Detay Program- Ödenek İcmali (01,02,05,08)" },
            new[] { "998-51", "Detay Program- Ödenek İcmali (03)" },
            new[] { "998-52", "Detay Program- Ödenek İcmali (06)" },
            new[] { "998-53", "Detay Program- Ödenek İcmali (07)" },
            new[] { "998-41", "Detay Program- Taşra Detay" },
            new[] { "998-42", "Detay Program- Taşra Özet" },
            new[] { "998-43", "Detay Program- Merkez Detay" },
            new[] { "998-44", "Detay Program- Merkez Özet" },
            new[] { "998-45", "Detay Program- Merkez Merkez Detay" },
            new[] { "998-46", "Detay Program- Merkez Merkez Özet" },
            new[] { "998-36", "AHP - MP Fark" },
            new[] { "998-99", "Açık avans listesi" }
        };

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/masrafplani/tanim", Tanim);
            app.MapGet("/api/masrafplani/kodlar", Kodlar);
            app.MapGet("/api/masrafplani/kodad", KodAdi);
            app.MapGet("/api/masrafplani/liste", Liste);
            app.MapPost("/api/masrafplani/tavandanmp", TavandanMP);
            app.MapPost("/api/masrafplani/ahpdenmp", AHPdenMP).DisableAntiforgery();
            app.MapPost("/api/masrafplani/onayla", Onayla);
            app.MapPost("/api/masrafplani/onaykaldir", OnayKaldir);
            app.MapPost("/api/masrafplani/oncal", OncekiAsamadanAl);
            app.MapGet("/api/masrafplani/rapor", Rapor);
            app.MapGet("/api/masrafplani/hizli", Hizli);
            app.MapGet("/api/masrafplani/imza", ImzaOku);
            app.MapPost("/api/masrafplani/imza", ImzaKaydet);
            app.MapGet("/api/masrafplani/kontroldonem", KontrolDonemOku);
            app.MapPost("/api/masrafplani/kontroldonem", KontrolDonemKaydet);
        }

        // ------------------------------------------------------------------ yetki

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, Adres);
        }

        private static bool Yonetici(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Bilgi(string bilgi) { return Results.Json(new { bilgi = bilgi }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static ENUMMasrafPlanAsama AsamaAl(int asama)
        {
            return Array.IndexOf(Asamalar, (ENUMMasrafPlanAsama)asama) >= 0 ? (ENUMMasrafPlanAsama)asama : ENUMMasrafPlanAsama.TALEP;
        }

        // ------------------------------------------------------------------ açılış, kod ağacı, kod adı

        /// <summary>
        /// Açılış (eski Page_Load): aşamalar (varsayılan Son durum (Onaylı)), yıl, son kullanılan harcama birimi (BHMV2KURUMSALYAPIKOD),
        /// rapor listesi; Kontrol Dönemi ve MP Düzelt eskiden yalnızca sistem yöneticisine görünüyordu.
        /// </summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = DateTime.Now.Year;
            List<object> asamalar = new List<object>();
            foreach (ENUMMasrafPlanAsama a in Asamalar)
                asamalar.Add(new { kod = (int)a, ad = TNS.BHMv2.Arac.MasrafPlanAsamaAd(a) });
            List<object> raporlar = new List<object>();
            foreach (string[] r in Raporlar)
                raporlar.Add(new { kod = r[0], ad = r[1] });
            string ky = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, KyDegisken);
            return Results.Json(new
            {
                yil = yil,
                asamalar = asamalar,
                asama = (int)ENUMMasrafPlanAsama.SONDURUMONAYLI,
                ky = ky,
                kyAd = MPTavanUclari.KodAd(yil, 53, ky),
                raporlar = raporlar,
                yonetici = Yonetici(kullanan)
            });
        }

        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return MPTavanUclari.KodAgaci(kullanan, yil, kod, ust);
        }

        private static IResult KodAdi(HttpContext baglam, IWebHostEnvironment ortam, int yil, int tur, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return MPTavanUclari.KodAdiBul(yil, tur, kod);
        }

        // ------------------------------------------------------------------ listele

        /// <summary>
        /// Listele (eski Listele / MPTertipGoster): seçilen aşamada kodlara ve harcama birimine uyan tertiplerin tavanı, toplamı ve aylık
        /// dağılımı. Toplamlar eskiden sütun başlıklarındaydı; burada tablonun altındaki toplam satırıdır. Harcama birimi saklanır.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, int asama, string kur, string fon, string fin, string eko, string peb, string ky)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                ky = Temiz(ky);
                Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABTasraOdenek);
                kistasTertip.TertibeKodEkle(0, Temiz(kur), "");
                kistasTertip.TertibeKodEkle(1, Temiz(fon), "");
                kistasTertip.TertibeKodEkle(2, Temiz(fin), "");
                kistasTertip.TertibeKodEkle(3, Temiz(eko), "");
                kistasTertip.TertibeKodEkle(4, Temiz(peb), "");
                kistasTertip.TertibeKodEkle(5, ky, "");
                UcgenKodBilgileri ukb = new UcgenKodBilgileri();
                for (int i = 0; i < 6; i++)
                    ukb.Add(new UcgenKodBilgi(kistasTertip.KodNolar(i), kistasTertip.KodNoKirilimSay(i)));
                BirimMasrafPlaniGenel bmpg = TNS.BHMv2.Arac.Tanimla().MasrafPlaniBulTavan(kullanan, AsamaAl(asama), kistasTertip, ukb, "", true);

                List<object> satirlar = new List<object>();
                MPTertipBilgi[] liste = bmpg?.bmpTertip?.tertipBilgiListe ?? new MPTertipBilgi[0];
                foreach (MPTertipBilgi mpeb in liste)
                {
                    GenelOdenekBilgi gob = mpeb.genelOdenekBilgi;
                    double[] aylar = new double[12];
                    for (int ay = 0; ay < 12; ay++) aylar[ay] = gob.ayDizi[ay];
                    satirlar.Add(new
                    {
                        tertip = mpeb.tertipOdenek.tertip.TertipStrOl(),
                        ad = mpeb.tertipOdenek.tertip.TertipAdiKodAddan,
                        birim = mpeb.tertipOdenek.tertip.adlar[5],
                        tavan = mpeb.tertipOdenek.odenek[0],
                        toplam = gob.Toplam(),
                        a1 = aylar[0], a2 = aylar[1], a3 = aylar[2], a4 = aylar[3], a5 = aylar[4], a6 = aylar[5],
                        a7 = aylar[6], a8 = aylar[7], a9 = aylar[8], a10 = aylar[9], a11 = aylar[10], a12 = aylar[11]
                    });
                }
                GenelIslemler.KullaniciDegiskenSakla(kullanan, KyDegisken, ky);
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Listeleme yapılamadı." : ex.Message);
            }
        }

        // ------------------------------------------------------------------ işlemler

        /// <summary>
        /// Tavandan MP'ye (eski TavandanMPYe): seçilen aşamada tavanlar seçilen aya (Ocak / Şubat / Aralık) dağıtılır. Tertip=true ise
        /// yalnızca seçilen kodlara uyan tertipler (eski "Tavandan MP'ye (Tertip)").
        /// </summary>
        private static async Task<IResult> TavandanMP(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MasrafPlaniIslemIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MasrafPlaniIslemIstegi();
            if (istek.Yil <= 0) return Hata("Yıl giriniz.");
            try
            {
                string kur = "", fon = "", fin = "", eko = "", peb = "";
                if (istek.Tertip)
                {
                    kur = Temiz(istek.Kur); fon = Temiz(istek.Fon); fin = Temiz(istek.Fin); eko = Temiz(istek.Eko); peb = Temiz(istek.Peb);
                }
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MasrafPlaniGenelUretAHP(kullanan, istek.Yil, AsamaAl(istek.Asama), "", true, istek.Ay,
                    new Tertip(istek.Yil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb));
                string mesaj = s?.hataStr;
                if (string.IsNullOrEmpty(mesaj)) return Bilgi("Dağıtım işlemi tamamlandı!");
                return s.islemSonuc ? Bilgi(mesaj) : Hata(mesaj);
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>
        /// AHP'den MP'ye (eski btnAHPdenMPye_Click; sistem yöneticisi iş kuralında denetlenir): Excel'in 2. satırından başlayarak sütunlar
        /// tertip, saymanlık, birim. Her satırın tertibi o saymanlık / birimin kurumsal yapısıyla tamamlanır; bütün satırlar bulunursa
        /// bu tertiplerde seçilen aşamanın masraf planı AHP'den yeniden oluşturulur. (Eski sayfa tertipleri listeye eklemeyi unuttuğu
        /// için işlem boş listeyle çağrılıyordu; burada okunan tertipler gönderilir.)
        /// </summary>
        private static async Task<IResult> AHPdenMP(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            if (dosya == null || dosya.Length == 0) return Hata("Yükleme için bir dosya seçiniz");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");
            int secYil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], 0);
            if (secYil <= 0) return Hata("Yıl giriniz.");
            ENUMMasrafPlanAsama asama = AsamaAl(OrtakFonksiyonlar.ConvertToInt((string)form["asama"], 0));

            string uzanti = Path.GetExtension(dosya.FileName ?? "").ToLowerInvariant();
            if (uzanti != ".xls" && uzanti != ".xlsx") uzanti = "." + GenelIslemler.ExcelTur();
            string dosyaAdxls = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul() + uzanti;
            try
            {
                using (FileStream hedef = File.Create(dosyaAdxls))
                    await dosya.CopyToAsync(hedef);
            }
            catch (Exception)
            {
                return Hata(string.Format("Dosya sunucuda saklanamadı ({0})", Path.GetFileName(dosyaAdxls)));
            }

            string genelMesaj = "";
            try
            {
                AsposeLisans.Yukle();
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                int sat = 1;
                string uyari = "";
                TNSCollection tertipler = new TNSCollection();
                while (true)
                {
                    string tertipStr = XLS.HucreDegerAl(sat, 0);
                    if (string.IsNullOrEmpty(tertipStr)) break;
                    // Önce dosyadaki tertip atanır ki kurumsal kod (kodlar[0]) ile harcama birimi aranabilsin; bulunan kurumsal yapı
                    // koduyla tertip tamamlanıp listeye eklenir. (Eski sayfa kurumsal kodu boş arıyor ve tertipleri listeye eklemiyordu.)
                    Tertip tertip = new Tertip(secYil, ENUMTabloKod.TABTasraOdenek);
                    tertip.TertipAta(tertipStr);
                    string saymanlik = XLS.HucreDegerAl(sat, 1);
                    string birim = XLS.HucreDegerAl(sat, 2);
                    string[] kurumsalYapiKodDizi = servis.KurumsalYapiListele(saymanlik, birim, tertip.kodlar[0]);
                    if (kurumsalYapiKodDizi != null && kurumsalYapiKodDizi.Length > 0)
                    {
                        tertip.TertipAta(tertipStr + "-" + kurumsalYapiKodDizi[0]);
                        tertipler.Add(tertip);
                    }
                    else
                        uyari += string.Format("{0}. satır: verilen saymanlık({1}) - birim({2}) bulunamadı.\n", sat + 1, saymanlik, birim);
                    sat++;
                }
                XLS.DosyaKapat();

                if (string.IsNullOrEmpty(uyari) && tertipler.Count == 0)
                    uyari = "Dosyada tertip bulunamadı (1. satır başlıktır, tertipler 2. satırdan başlamalıdır).";
                if (string.IsNullOrEmpty(uyari))
                {
                    Sonuc s = servis.MPAHPdenMPye(kullanan, secYil, asama, tertipler);
                    if (!s.islemSonuc)
                        uyari = s.hataStr;
                }
                if (!string.IsNullOrEmpty(uyari))
                    genelMesaj += "\n" + uyari;
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

        /// <summary>MP Onayla (eski btnOnayla_Click): harcama biriminin masraf planı seçilen aşama ve ay için onaylanır.</summary>
        private static async Task<IResult> Onayla(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MasrafPlaniIslemIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MasrafPlaniIslemIstegi();
            try
            {
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MasrafPlaniGenelOnayla(kullanan, istek.Yil, Temiz(istek.Ky), AsamaAl(istek.Asama), istek.Ay);
                if (s != null && s.islemSonuc) return Bilgi("Onaylandı");
                return Hata(s?.hataStr ?? "Onaylanamadı.");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>MP Onay Kaldır (eski btnOnayKaldir_Click).</summary>
        private static async Task<IResult> OnayKaldir(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MasrafPlaniIslemIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MasrafPlaniIslemIstegi();
            try
            {
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MasrafPlaniGenelOnayKaldir(kullanan, istek.Yil, Temiz(istek.Ky), null, AsamaAl(istek.Asama));
                if (s != null && s.islemSonuc) return Bilgi("Onay kaldırıldı");
                return Hata(s?.hataStr ?? "Onay kaldırılamadı.");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>Önc. Al (eski btnAsamaKopyala_Click): bir önceki aşamanın masraf planı seçilen aşamaya kopyalanır.</summary>
        private static async Task<IResult> OncekiAsamadanAl(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MasrafPlaniIslemIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MasrafPlaniIslemIstegi();
            ENUMMasrafPlanAsama asama = AsamaAl(istek.Asama);
            try
            {
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MPdenMPye(kullanan, istek.Yil, (ENUMMasrafPlanAsama)(((int)asama) - 1), asama);
                if (s != null && s.islemSonuc) return Bilgi("İşlem tamamlandı.");
                return Hata("Hata:" + (s?.hataStr ?? ""));
            }
            catch (Exception ex)
            {
                return Hata("Hata:" + ex.Message);
            }
        }

        // ------------------------------------------------------------------ raporlar

        private static Hashtable RaporParam(int yil, ENUMMasrafPlanAsama asama, string kur, string fon, string fin, string eko, string peb, string ky, string kyAd)
        {
            Hashtable h = new Hashtable();
            h["ButceYil"] = yil.ToString();
            h["kurumsalyapi"] = Temiz(ky);
            h["BirimAd"] = kyAd;
            h["Asama"] = ((int)asama).ToString();
            h["KurKod"] = Temiz(kur);
            h["FonKod"] = Temiz(fon);
            h["FinKod"] = Temiz(fin);
            h["EkoKod"] = Temiz(eko);
            h["PebKod"] = Temiz(peb);
            return h;
        }

        /// <summary>Eski ImzalariEkle: kullanıcının rapor imzaları (MasrafPlaniImza).</summary>
        private static void ImzalariEkle(Kullanici kullanan, Hashtable raporParam)
        {
            APImza imza = APGirisUclari.ImzaBilgisi(kullanan);
            raporParam["HazGorev"] = imza.HazGorev ?? "";
            raporParam["HazKisi"] = imza.HazKisi ?? "";
            raporParam["ImzGorev"] = imza.ImzGorev ?? "";
            raporParam["ImzKisi"] = imza.ImzKisi ?? "";
        }

        /// <summary>
        /// Yazdır (eski btnYazdir_Click): rapor "grup-no" biçiminde (listedeki değer). 5000-13 saymanlık-SGB.net harcama kontrolüdür (bu
        /// ay, harcama biriminin saymanlık ve birimi, Excel); diğerleri Rapor.xml 998 grubundan seçimler ve imzalarla alınır.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string rapor, int yil, int asama, string kur, string fon, string fin, string eko, string peb, string ky)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            string kod = Temiz(rapor);
            if (Array.Find(Raporlar, r => r[0] == kod) == null) return Hata("Rapor türü hatalı.");
            string[] rapKod = kod.Split('-');
            int rapGrupNo = OrtakFonksiyonlar.ConvertToInt(rapKod[0], 0);
            int rapNo = OrtakFonksiyonlar.ConvertToInt(rapKod[1], 0);
            try
            {
                AsposeLisans.Yukle();
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                Hashtable raporParam;
                ky = Temiz(ky);
                if (rapGrupNo == 5000)
                {
                    if (ky == "") return Hata("Harcama birimi seçiniz.");
                    KurumsalYapi k = servis.KurumsalYapiDondur(kullanan.aktifKultur, ky);
                    if (k == null) return Hata("Harcama birimi bulunamadı.");
                    raporParam = new Hashtable();
                    raporParam["ButceYil"] = yil.ToString();
                    raporParam["AyNo"] = DateTime.Now.Month.ToString();
                    raporParam["Muhasebe"] = k.saymanlikKod;
                    raporParam["Birim"] = k.birimKod;
                    raporParam["Kur"] = Temiz(kur);
                    raporParam["EkoKirilim"] = "4";
                    raporParam["CiktiYeri"] = "EXCEL";
                }
                else
                {
                    // Eski sayfa BirimAd'a harcama biriminin kodunu yazıyordu (kodKY.Text); rapor başlığında adı görünsün diye adı verilir.
                    string kyAd = ky == "" ? "" : MPTavanUclari.KodAd(yil, 53, ky);
                    raporParam = RaporParam(yil, AsamaAl(asama), kur, fon, fin, eko, peb, ky, kyAd == "" ? ky : kyAd);
                    ImzalariEkle(kullanan, raporParam);
                }
                string dosya = RaporlamaServis.TekRaporAl(rapGrupNo, rapNo, kullanan, servis, raporParam, null);
                if (RaporlamaServis.MesajMi(dosya)) return Hata(dosya);
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, "MasrafPlani_" + rapGrupNo + "_" + rapNo + "_" + DateTime.Now.Ticks + "." + uzanti, true, uzanti);
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Rapor hazırlanamadı." : ex.Message);
            }
        }

        /// <summary>Hızlı Öd. Gönd. Üret (cezali 0) / Hızlı Öd. (SGB&gt;KBS) (cezali 1) (eski FormUret: MPRaporDetay "Hizli", Excel).</summary>
        private static IResult Hizli(HttpContext baglam, IWebHostEnvironment ortam, int cezali, int yil, int asama, string kur, string fon, string fin, string eko, string peb, string ky)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl giriniz.");
            string dosyaAd = null;
            try
            {
                AsposeLisans.Yukle();
                ky = Temiz(ky);
                Hashtable raporParam = RaporParam(yil, AsamaAl(asama), kur, fon, fin, eko, peb, ky, ky);
                raporParam["cezali"] = cezali == 1 ? "1" : "0";
                MPRaporDetay r = new MPRaporDetay(ortam.ContentRootPath);
                dosyaAd = TNS.OrtakFonksiyonlar.GeciciDosyaAdBul();
                string uz = GenelIslemler.ExcelTur();
                string uretilenDosya = r.RaporAl(kullanan, TNS.BHMv2.Arac.Tanimla(), "", dosyaAd, uz, raporParam, "Hizli");
                if (string.IsNullOrEmpty(uretilenDosya)) return Hata("Dosya üretilemedi.");
                return DosyaIslem.DosyaGonder(uretilenDosya, "HizliOdenek" + DateTime.Now.Ticks + "." + uz, true, uz);
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
            finally
            {
                if (!string.IsNullOrEmpty(dosyaAd))
                {
                    try { File.Delete(dosyaAd); } catch { }
                }
            }
        }

        // ------------------------------------------------------------------ imzalar (eski MasrafPlaniGenelImza.aspx)

        private static IResult ImzaOku(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return Results.Json(APGirisUclari.ImzaBilgisi(kullanan));
        }

        private static async Task<IResult> ImzaKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, APImza istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Sonuc s = APGirisUclari.ImzaSakla(kullanan, istek);
            if (s != null && s.islemSonuc) return Bilgi("İşlem Başarılı");
            return Hata(s?.hataStr ?? "Kaydedilemedi.");
        }

        // ------------------------------------------------------------------ kontrol dönemi (eski MasrafPlaniKontrolDonem.aspx)

        private static string StratekDegisken(string ad)
        {
            try
            {
                return GenelIslemler.KullaniciDegiskenGetir("stratek", ad);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static Sonuc StratekSakla(string ad, string deger)
        {
            TNSCollection c = new TNSCollection();
            c.Add(deger ?? "");
            return TNS.KYM.Arac.Tanimla().DegiskenKaydet("stratek", ad, c);
        }

        /// <summary>Kontrol dönemi bilgileri (eski Listele): ödenek kontrol yılı / ayı, hizmet maliyeti kontrol yılı, kontrol açık mı, tertipler.</summary>
        private static IResult KontrolDonemOku(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            try
            {
                TNS.BHMv2.Arac.OdenekKontrolDegiskenleri.Oku(DateTime.Now.Year);
                TNS.BHMv2.Arac.OdenekKontrolDegiskenleri d = TNS.BHMv2.Arac.degisken;
                return Results.Json(new
                {
                    yil = d.yilKontrol[0].ToString(),
                    ay = d.ayKontrol[0].ToString(),
                    hizYil = d.hizMalKontrolYil.ToString(),
                    kontrolYap = StratekDegisken("OdenekKontrolYap") == "1",
                    tertipler = StratekDegisken("OdenekKontrolTertipler")
                });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>
        /// Kontrol dönemi kaydı: Bolum 1 dönem (yıl-ay) ve hizmet maliyeti kontrol yılı (eski btnKaydet_Click; ödenek kontrol değişkenleri
        /// sıfırlanır), Bolum 2 ödenek kontrolü açık / kapalı ve kontrol edilecek tertipler (eski btnKaydet2_Click). Değerler "stratek"
        /// değişkenlerine yazılır.
        /// </summary>
        private static async Task<IResult> KontrolDonemKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MasrafPlaniKontrolDonem istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            istek ??= new MasrafPlaniKontrolDonem();
            try
            {
                Sonuc s;
                if (istek.Bolum == 2)
                {
                    s = StratekSakla("OdenekKontrolYap", istek.KontrolYap ? "1" : "0");
                    if (s != null && s.islemSonuc) s = StratekSakla("OdenekKontrolTertipler", Temiz(istek.Tertipler));
                }
                else
                {
                    s = StratekSakla("MasrafPlaniKontrolDonem", Temiz(istek.Yil) + "-" + Temiz(istek.Ay));
                    if (s != null && s.islemSonuc) s = StratekSakla("HizMalTesKontrolYili", Temiz(istek.HizYil));
                    TNS.BHMv2.Arac.OdenekKontrolDegiskenleri.Sifirla();
                }
                if (s != null && s.islemSonuc) return Bilgi("İşlem Tamamlandı");
                return Hata(s?.hataStr ?? "Kaydedilemedi.");
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }
    }
}
