using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Istemci.ButceHazirlik;
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
using TNS.KHM;
using TNS.KYM;

namespace Istemci.KesinHesap
{
    public class FisGerekceDetayIstegi
    {
        public int Yil { get; set; }
        public string Tertip { get; set; }
        public string Gerekce { get; set; }
    }

    public class FisGerekceDetayKistasIstegi
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public string Eko { get; set; }
        public string Prog { get; set; }
    }

    /// <summary>
    /// Gider Cetveli Detay Açıklamalar sayfasının (eski KesinHesap/FisGiderGerekceDetay.aspx, faaliyet bazında açıklama) JavaScript'ten
    /// çağırdığı uçlar:
    ///   GET  /api/fisgidergerekcedetay/tanim, /kodlar?yil=&amp;tur=&amp;ust= (kurumsal 1, ekonomik 4, faaliyet 14 kod ağacı)
    ///   GET  /api/fisgidergerekcedetay/liste?yil=&amp;kur=&amp;eko=&amp;prog=, /oku?yil=&amp;tertip=
    ///   POST /api/fisgidergerekcedetay/kaydet, /gonder (SGB'ye gönder: onay), /onaykaldir (yalnızca sistem yöneticisi)
    ///   GET  /api/fisgidergerekcedetay/rapor?yil=&amp;kur=&amp;eko=&amp;prog= (KHM\KH-GiderGerekceDetay.dotx şablonundan Aspose.Words ile; Word .docx)
    /// İşlemler eskisi gibi TNS.KHM / TNS.BHMv2 / TNS.UZY Arac.Tanimla() üzerinden yapılır.
    /// </summary>
    public static class FisGiderGerekceDetayUclari
    {
        private const string SayfaAdres = "../KesinHesap/FisGiderGerekceDetay.aspx";
        private const string KayitHatasi = "İşlem sırasında hata oluştu: "; // eski Resources.KesinHesap.FRM018
        private const string KayitBasarili = "Bilgiler başarıyla saklandı."; // eski Resources.KesinHesap.FRM019
        private const string SablonAd = "KH-GiderGerekceDetay.dotx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/fisgidergerekcedetay/tanim", Tanim);
            app.MapGet("/api/fisgidergerekcedetay/kodlar", Kodlar);
            app.MapGet("/api/fisgidergerekcedetay/liste", Liste);
            app.MapGet("/api/fisgidergerekcedetay/oku", Oku);
            app.MapPost("/api/fisgidergerekcedetay/kaydet", Kaydet);
            app.MapPost("/api/fisgidergerekcedetay/gonder", Gonder);
            app.MapPost("/api/fisgidergerekcedetay/onaykaldir", OnayKaldir);
            app.MapGet("/api/fisgidergerekcedetay/rapor", Rapor);
        }

        /// <summary>Kullanıcı sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
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

        private static bool SistemYoneticisi(Kullanici kullanan)
        {
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI);
        }

        private static IKHMServis Servis()
        {
            return TNS.KHM.Arac.Tanimla();
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static bool YilGecerli(int yil)
        {
            return yil >= 2007 && yil <= DateTime.Now.Year + 1;
        }

        /// <summary>Eski KodAl: "kod- ad" metninden kod (tireden önceki kısım); yalnızca kod da yazılabilir.</summary>
        private static string KodAl(string metin)
        {
            string s = (metin ?? "").Trim();
            int i = s.IndexOf('-');
            return (i > 0 ? s.Substring(0, i) : s).Trim();
        }

        /// <summary>Eski kıstas tertibi: kurumsal, ekonomik ve faaliyet (program) kodları.</summary>
        private static Tertip KistasKur(int yil, string kur, string eko, string prog)
        {
            Tertip kistas = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            kistas.kodlar[0] = KodAl(kur);
            kistas.kodlar[3] = KodAl(eko);
            kistas.kodlar[4] = KodAl(prog);
            return kistas;
        }

        private static Tertip TertipKur(int yil, string tertip)
        {
            Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
            t.TertipAta((tertip ?? "").Trim());
            return t;
        }

        /// <summary>Eski sayfada liste kıstasına uygulanan görme yetkisi; tek tertibe yapılan işlemlerde de aranır.</summary>
        private static bool GormeYetkisi(Kullanici kullanan, Tertip tertip)
        {
            return TNS.BHMv2.Arac.Tanimla().OGYYetkisiVarMi(kullanan, null, tertip, ENUMOGY.Gorme);
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Açılış bilgileri (eski Page_Load): varsayılan yıl geçen yıl; Onay Kaldır yalnızca sistem yöneticisine.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int buYil = DateTime.Now.Year;
            return Results.Json(new { yil = buYil - 1, enAz = 2007, enCok = buYil + 1, yonetici = SistemYoneticisi(kullanan) });
        }

        /// <summary>Kod ağacının bir düzeyi (eski nodeLoadPar / nodeLoadPar3): 1 kurumsal, 4 ekonomik, 14 faaliyet.</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tur, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (tur != "1" && tur != "4" && tur != "14") return Results.Json(new { hata = "Kod türü hatalı!" });
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, tur, ust);
        }

        /// <summary>Eski btnListele_Click: kıstasa uyan tertipler, ekonomik kod adı, kayıtlı açıklamanın damga sayısı ve onay kodu.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string eko, string prog)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YilGecerli(yil)) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                Tertip kistas = KistasKur(yil, kur, eko, prog);
                if (!GormeYetkisi(kullanan, kistas))
                    return Results.Json(new { hata = string.Format("Verdiğiniz kodu {0} görme yetkiniz yok!", kistas.TertipStrOl()) });

                UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
                kodBilgileri.Add(new UcgenKodBilgi(1, 4));
                kodBilgileri.Add(new UcgenKodBilgi(2, 4));
                kodBilgileri.Add(new UcgenKodBilgi(3, 1));
                kodBilgileri.Add(new UcgenKodBilgi(4, 2));
                kodBilgileri.Add(new UcgenKodBilgi(14, 4));
                EkDegerParam[] ekBilgiler = new EkDegerParam[1];

                OdenekBilgi[] ob = TNS.BHMv2.Arac.Tanimla().ListeTertipOl(kullanan, kistas, kodBilgileri, ekBilgiler, true, true, true);
                ObjectArray<GiderGerekceDetay> oa = Servis().FaaliyetAciklamaDamgaSayBul(kullanan, kistas);

                List<object> satirlar = new List<object>();
                foreach (OdenekBilgi o in ob ?? new OdenekBilgi[0])
                {
                    GiderGerekceDetay ggd = TertipGerekceBul(oa, o.tertip);
                    satirlar.Add(new
                    {
                        tertip = o.tertip.TertipStrOl(),
                        ekoAd = o.tertip.adlar[3],
                        damgaSay = ggd == null ? 0 : ggd.damgaSay,
                        onayKod = ggd == null ? 0 : ggd.onayKod
                    });
                }
                return Results.Json(new { tertipler = satirlar });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Tertipler listelenemedi." });
            }
        }

        private static GiderGerekceDetay TertipGerekceBul(ObjectArray<GiderGerekceDetay> oa, Tertip tertip)
        {
            if (oa == null || oa.objeler == null || (oa.sonuc != null && !oa.sonuc.islemSonuc)) return null;
            foreach (GiderGerekceDetay g in oa.objeler)
                if (g.tertip.Kiyasla(tertip))
                    return g;
            return null;
        }

        /// <summary>
        /// Eski TertipOku: tertibin K.B.Ö. ve harcanan tutarı ile kayıtlı açıklaması. (Eski sayfada eklenen, düşülen, ödenek ve
        /// kalan alanları gizliydi; burada da gösterilmez.)
        /// </summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, string tertip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YilGecerli(yil) || string.IsNullOrWhiteSpace(tertip)) return Results.Json(new { hata = "Yıl ve tertip seçiniz." });
            try
            {
                Tertip t = TertipKur(yil, tertip);
                if (!GormeYetkisi(kullanan, t)) return Results.Json(new { hata = "Bu tertibi görme yetkiniz yok!" });

                string yils = yil.ToString();
                string ttt = tertip.Trim();
                TNS.UZY.IUZYServis uzy = TNS.UZY.Arac.Tanimla();
                double kbo = uzy.UzayDegeriDbl(kullanan, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl(yils, "06.00", "0", ttt), false);
                double harcanan = uzy.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", TNS.UZY.Arac.UzayKosulOl(yils, "", "", "830", TertipHRCUzayYap(ttt)), false);

                GiderGerekceDetayVAN ggdv = Servis().GiderGerekceDetayGetir(kullanan, yil, t);
                if (ggdv.sonuc != null && !ggdv.sonuc.islemSonuc)
                    return Results.Json(new { hata = KayitHatasi + HataMetni(ggdv.sonuc.hataStr, "Açıklama okunamadı.") });
                string gerekce = ggdv.gerekce == null ? "" : (ggdv.gerekce.gerekce ?? "");
                return Results.Json(new { kbo = kbo, harcanan = harcanan, gerekce = gerekce });
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Açıklama okunamadı." });
            }
        }

        private static string TertipHRCUzayYap(string tertip)
        {
            string[] s = tertip.Split('-');
            return TNS.UZY.Arac.UzayKosulOl(OrtakFonksiyonlar.DiziElemanAl(s, 0, ""), OrtakFonksiyonlar.DiziElemanAl(s, 1, ""), OrtakFonksiyonlar.DiziElemanAl(s, 2, ""),
                OrtakFonksiyonlar.DiziElemanAl(s, 3, ""), "", OrtakFonksiyonlar.DiziElemanAl(s, 4, ""));
        }

        // ------------------------------------------------------------------ yazma

        private static async Task<(IResult hata, Kullanici kullanan, GiderGerekceDetay gerekce)> KayitHazirla(HttpContext baglam, IWebHostEnvironment ortam,
            FisGerekceDetayIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return (Results.BadRequest(), null, null);
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return (ret, null, null);
            if (istek == null || !YilGecerli(istek.Yil) || string.IsNullOrWhiteSpace(istek.Tertip))
                return (Results.Json(new { basarili = false, hata = "Yıl ve tertip seçiniz." }), null, null);

            GiderGerekceDetay gerekce = new GiderGerekceDetay(istek.Yil);
            gerekce.gerekce = (istek.Gerekce ?? "").Trim();
            gerekce.tertip.TertipAta(istek.Tertip.Trim());
            if (!GormeYetkisi(kullanan, gerekce.tertip))
                return (Results.Json(new { basarili = false, hata = "Bu tertibi görme yetkiniz yok!" }), null, null);
            return (null, kullanan, gerekce);
        }

        /// <summary>Eski btnKaydet_Click.</summary>
        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, FisGerekceDetayIstegi istek, IAntiforgery sahtecilik)
        {
            var (hata, kullanan, gerekce) = await KayitHazirla(baglam, ortam, istek, sahtecilik);
            if (hata != null) return hata;
            Sonuc sonuc;
            try
            {
                sonuc = Servis().GiderGerekceDetayKaydet(kullanan, istek.Yil, gerekce);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = KayitHatasi + HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız.") });
            return Results.Json(new { basarili = true, bilgi = KayitBasarili });
        }

        /// <summary>Eski btnGonder_Click: açıklama SGB'ye gönderilir (onaylanır); bundan sonra metin değiştirilemez.</summary>
        private static async Task<IResult> Gonder(HttpContext baglam, IWebHostEnvironment ortam, FisGerekceDetayIstegi istek, IAntiforgery sahtecilik)
        {
            var (hata, kullanan, gerekce) = await KayitHazirla(baglam, ortam, istek, sahtecilik);
            if (hata != null) return hata;
            Sonuc sonuc;
            try
            {
                sonuc = Servis().GiderGerekceDetayOnayla(kullanan, istek.Yil, gerekce);
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Onaylama işlemi hata ile sonuçlandı." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = "Onaylama işlemi hata ile sonuçlandı: " + HataMetni(sonuc.hataStr, "") });
            return Results.Json(new { basarili = true, bilgi = "Onaylama işlemi tamamlandı." });
        }

        /// <summary>Eski btnOnayKaldir_Click (yalnızca sistem yöneticisi): kıstasa uyan açıklamaların onayı kaldırılır.</summary>
        private static async Task<IResult> OnayKaldir(HttpContext baglam, IWebHostEnvironment ortam, FisGerekceDetayKistasIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!SistemYoneticisi(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (istek == null || !YilGecerli(istek.Yil)) return Results.Json(new { basarili = false, hata = "Yıl hatalı!" });
            Sonuc sonuc;
            try
            {
                sonuc = Servis().FaaliyetAciklamaOnayKaldir(kullanan, KistasKur(istek.Yil, istek.Kur, istek.Eko, istek.Prog));
            }
            catch (Exception)
            {
                return Results.Json(new { basarili = false, hata = "Onay kaldırma işlemi hata ile sonuçlandı." });
            }
            if (!sonuc.islemSonuc)
                return Results.Json(new { basarili = false, hata = "Onay kaldırma işlemi hata ile sonuçlandı: " + HataMetni(sonuc.hataStr, "") });
            return Results.Json(new { basarili = true, bilgi = "Onay kaldırma işlemi tamamlandı." });
        }

        // ------------------------------------------------------------------ rapor (Aspose.Words, .dotx şablonu)

        /// <summary>
        /// Eski btnRaporYazdir_Click: kıstasa uyan tertiplerin açıklamaları KHM\KH-GiderGerekceDetay.dotx şablonundan Aspose.Words
        /// adres-mektup birleştirmesiyle Word (.docx) olarak üretilir (eski RTF şablonu ve EkRtf yerine).
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string eko, string prog)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!YilGecerli(yil)) return Results.Json(new { hata = "Yıl hatalı!" });
            try
            {
                Tertip kistas = KistasKur(yil, kur, eko, prog);
                if (!GormeYetkisi(kullanan, kistas))
                    return Results.Json(new { hata = string.Format("Verdiğiniz kodu {0} görme yetkiniz yok!", kistas.TertipStrOl()) });
                string sablon = SablonBul(ortam.ContentRootPath);
                if (sablon == null) return Results.Json(new { hata = "Rapor şablonu bulunamadı (KHM\\" + SablonAd + ")." });
                string dosya = DetayRaporu(kullanan, yil, kistas, sablon, out string hata);
                if (dosya == null) return Results.Json(new { hata = hata });
                return DosyaIslem.DosyaGonder(dosya, "GiderGerekceDetay_" + yil, true, "docx");
            }
            catch (Exception)
            {
                return Results.Json(new { hata = "Rapor hazırlanamadı." });
            }
        }

        /// <summary>Şablon önce kurumun rapor şablonu klasöründe (RaporSablonYol\KHM), yoksa SGB.Net\RaporSablon\KHM klasöründe aranır.</summary>
        private static string SablonBul(string uygulamaKlasoru)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "KHM", SablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "KHM", SablonAd);
            return File.Exists(u) ? u : null;
        }

        /// <summary>Faaliyet (PEB) kodunun adı (eski GenelIslemler.KodAd(58, ...): ABSPEBKOD uzayı).</summary>
        private static string FaaliyetAd(int yil, string peb)
        {
            string ad = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "ABSPEBKOD", yil + "-" + peb, true, null);
            return (ad ?? "").Replace(",", " ");
        }

        /// <summary>
        /// Eski rapor kodunun hesapları aynen; rapor EkRtf yerine Aspose.Words ile yazılır. Şablonda üst bilgide «kurum» ve «yil»,
        /// gövdede Gerekce bölgesi: kodların parçaları (k1-k4, f1-f4, ff, e1-e2, p1-p4), kod (tertip), aciklama ve gerekce.
        /// </summary>
        private static string DetayRaporu(Kullanici kullanan, int yil, Tertip kistas, string sablon, out string hata)
        {
            hata = null;
            ObjectArray<GiderGerekceDetay> oa = Servis().GiderGerekceListeleDetay(kullanan, yil, kistas);
            if (!oa.sonuc.islemSonuc)
            {
                hata = HataMetni(oa.sonuc.hataStr, "Açıklamalar okunamadı.");
                return null;
            }
            if (oa.objeler == null || oa.objeler.Count == 0)
            {
                hata = "Verilen tertip kıstasına uygun kayıt bulunamadı!";
                return null;
            }

            string kurum = TNS.DEG.Arac.VarsayilanKurumBul() ?? "";
            string kurumEk = "";
            if (kurum.Length == 2) kurumEk = "00";
            string kurumYaz = kurum + kurumEk + " " + (TNS.DEG.Arac.DegiskenDegerBul(0, "/KURUMAD") ?? "");

            DataTable dtGerekce = new DataTable("Gerekce");
            foreach (string sutun in new[] { "k1", "k2", "k3", "k4", "f1", "f2", "f3", "f4", "ff", "e1", "e2", "p1", "p2", "p3", "p4", "kod", "aciklama", "gerekce" })
                dtGerekce.Columns.Add(sutun);

            foreach (GiderGerekceDetay g in oa.objeler)
            {
                g.tertip.Noktala();
                string kur = g.tertip.kodlar[0] ?? "";
                string fon = g.tertip.kodlar[1] ?? "";
                string fin = g.tertip.kodlar[2] ?? "";
                string eko = g.tertip.kodlar[3] ?? "";
                string peb = g.tertip.kodlar[4] ?? "";

                string[] kurDizi = kur.Split('.');
                string[] fonDizi = fon.Split('.');
                string[] finDizi = fin.Split('.');
                string[] ekoDizi = eko.Split('.');
                string[] pebDizi = peb.Split('.');

                string aciklama = FaaliyetAd(yil, peb) + " (" + GenelIslemler.KodAd(4, yil + "-" + eko, true) + ")";

                dtGerekce.Rows.Add(OrtakFonksiyonlar.DiziElemanAl(kurDizi, 0, ""), OrtakFonksiyonlar.DiziElemanAl(kurDizi, 1, ""), OrtakFonksiyonlar.DiziElemanAl(kurDizi, 2, ""), OrtakFonksiyonlar.DiziElemanAl(kurDizi, 3, ""),
                    OrtakFonksiyonlar.DiziElemanAl(fonDizi, 0, ""), OrtakFonksiyonlar.DiziElemanAl(fonDizi, 1, ""), OrtakFonksiyonlar.DiziElemanAl(fonDizi, 2, ""), OrtakFonksiyonlar.DiziElemanAl(fonDizi, 3, ""),
                    OrtakFonksiyonlar.DiziElemanAl(finDizi, 0, ""),
                    OrtakFonksiyonlar.DiziElemanAl(ekoDizi, 0, ""), OrtakFonksiyonlar.DiziElemanAl(ekoDizi, 1, ""),
                    OrtakFonksiyonlar.DiziElemanAl(pebDizi, 0, ""), OrtakFonksiyonlar.DiziElemanAl(pebDizi, 1, ""), OrtakFonksiyonlar.DiziElemanAl(pebDizi, 2, ""), OrtakFonksiyonlar.DiziElemanAl(pebDizi, 3, ""),
                    g.tertip.TertipStrOl(), aciklama, g.gerekce);
            }

            AsposeLisans.Yukle();
            Aspose.Words.Document doc = new Aspose.Words.Document(sablon);
            doc.MailMerge.CleanupOptions = Aspose.Words.Reporting.MailMergeCleanupOptions.RemoveUnusedRegions;
            doc.MailMerge.Execute(new[] { "yil", "kurum" }, new object[] { yil.ToString(), kurumYaz });
            doc.MailMerge.ExecuteWithRegions(dtGerekce);
            doc.UpdateFields(); // alt bilgideki tarih / saat ve sayfa

            string sonucDosyaad = DosyaIslem.DosyaAdUret("", ".docx");
            doc.Save(sonucDosyaad, Aspose.Words.SaveFormat.Docx);
            return sonucDosyaad;
        }
    }
}
