using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

namespace Istemci.ButceHazirlik
{
    /// <summary>Sayfanın kod alanları (yıl, kurumsal, fonksiyonel, finans, ekonomik, alt faaliyet).</summary>
    public class MPTavanSecim
    {
        public int Yil { get; set; }
        public string Kur { get; set; }
        public string Fon { get; set; }
        public string Fin { get; set; }
        public string Eko { get; set; }
        public string Peb { get; set; }
    }

    /// <summary>Onayla Tavan / Detay ve yönetici işlemleri (eski OnayDurumDegistir: 1-6).</summary>
    public class MPTavanOnayIstegi
    {
        public int Yil { get; set; }
        public string Peb { get; set; }
        public int Kod { get; set; }
    }

    /// <summary>İl izni (eski MPTavanIlGirisDegistir.aspx).</summary>
    public class MPTavanIlIzniIstegi
    {
        public int Yil { get; set; }
        public string Tertip { get; set; }
    }

    /// <summary>Merkez Yükle (eski btnMerkezYukle_Click).</summary>
    public class MPTavanYilIstegi
    {
        public int Yil { get; set; }
    }

    /// <summary>Tavan girişi tablosunun satırı (eski MPTavanGiris.MPOdenekBilgi; alan adları eski Ext store'undaki gibi).</summary>
    public class MPOdenekBilgi
    {
        public string birimKod { get; set; }
        public string birimKodAd { get; set; }
        public double harcama { get; set; }
        public double birimTalep { get; set; }
        public double gonderilen { get; set; }
        public double tavan { get; set; }
        public double detayTavan { get; set; }
    }

    /// <summary>Tavan girişi penceresinin parametreleri (eski MPTavanGiris.aspx?yil=&amp;senaryo=&amp;tertip=&amp;DG=&amp;kykod=&amp;saybir=).</summary>
    public class MPTavanGirisParam
    {
        public int Yil { get; set; }
        public string Senaryo { get; set; }
        public string Tertip { get; set; }
        /// <summary>G: il tavanı (birimlere), D: merkez detay (harcama birimine), I: il girişi (MPTavanIlAna'dan).</summary>
        public string DG { get; set; }
        public string KyKod { get; set; }
        public string SayBir { get; set; }

        public bool MerkezDetayMi() { return DG == "D" || DG == "d"; }
        public bool IlGirisiMi() { return DG == "I" || DG == "i"; }
        public bool DetayMi() { return MerkezDetayMi() || IlGirisiMi(); }
    }

    public class MPTavanGirisKayit : MPTavanGirisParam
    {
        public List<MPOdenekBilgi> Satirlar { get; set; }
    }

    /// <summary>
    /// Masraf Planı Tavan ekranının (eski ButceHazirlikv2/MPTavanAna.aspx) ve içinden pencerede açılan Tavan Girişi sayfasının
    /// (eski MPTavanGiris.aspx; İl İzni değişikliği eski MPTavanIlGirisDegistir.aspx) JavaScript'ten çağırdığı uçlar.
    ///   GET  /api/mptavan/tanim                   yıllar, hazırlık yılı, son aranan tertip, rapor kırılım seçenekleri, yönetici mi
    ///   GET  /api/mptavan/kodlar?yil=&amp;kod=&amp;ust=   kod ağacı (eski KodAgac.aspx: 1, 2, 3, 4, 14, 17)
    ///   GET  /api/mptavan/kodad?yil=&amp;tur=&amp;kod=    yazılan kodun adı (eski KodAdGetir: 1, 2, 3, 4, 58, 53)
    ///   GET  /api/mptavan/liste?...               (eski btnListele_Click: MPTavanGenelListele, il izni, toplamlar)
    ///   POST /api/mptavan/ilizni                  (eski MPTavanIlGirisDegistir.aspx: tertibin il bazında giriş izni aç / kapat)
    ///   GET  /api/mptavan/fazlagonderilen?...     (eski GonderileniFazlalariYukle: onay öncesi gönderileni fazla olanlar)
    ///   POST /api/mptavan/onay                    (eski OnayDurumDegistir 1-6; 3-6 sistem yöneticisi)
    ///   GET  /api/mptavan/rapor?rapor=...          genel (Hata Raporları: Genel Liste 1/2/3), dagitilmayan, gondfazla (detay 0/1),
    ///                                             tavan (Yazdır), ho (H.Öd.Üret, cezali 0/1), uret (Üretim Ekranı; yönetici)
    ///   POST /api/mptavan/dosyayukle              (eski btnDosyaYukle_Click: tertip / saymanlık / birim Excel'i, KBÖ birime tavan)
    ///   POST /api/mptavan/merkezyukle             (eski btnMerkezYukle_Click: merkez birimlerine ait ödenekler merkez birimlerine)
    ///   GET  /api/mptavan/giris/liste?...         (eski MPTavanGiris OdenekListele)
    ///   POST /api/mptavan/giris/kaydet            (eski MPTavanGiris OdenekKaydet)
    ///   POST /api/mptavan/giris/yukle             (eski MPTavanGiris btnDosyaYukle_Click; yalnızca merkez detay)
    /// İş kuralları eskisi gibi TNS.BHMv2 (IBHMv2Servis) servisinden çağrılır. Raporlar MasrafPlaniRaporlari.cs (MPRaporDetay).
    /// </summary>
    public static class MPTavanUclari
    {
        public const string AnaAdres = "../ButceHazirlikv2/MPTavanAna.aspx";
        public const string IlAnaAdres = "../ButceHazirlikv2/MPTavanIlAna.aspx";
        public const string GirisAdres = "../ButceHazirlikv2/MPTavanGiris.aspx";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/mptavan/tanim", Tanim);
            app.MapGet("/api/mptavan/kodlar", Kodlar);
            app.MapGet("/api/mptavan/kodad", KodAdi);
            app.MapGet("/api/mptavan/liste", Liste);
            app.MapPost("/api/mptavan/ilizni", IlIzni);
            app.MapGet("/api/mptavan/fazlagonderilen", FazlaGonderilen);
            app.MapPost("/api/mptavan/onay", Onay);
            app.MapGet("/api/mptavan/rapor", Rapor);
            app.MapPost("/api/mptavan/dosyayukle", DosyaYukle).DisableAntiforgery();
            app.MapPost("/api/mptavan/merkezyukle", MerkezYukle);
            app.MapGet("/api/mptavan/giris/liste", GirisListe);
            app.MapPost("/api/mptavan/giris/kaydet", GirisKaydet);
            app.MapPost("/api/mptavan/giris/yukle", GirisYukle).DisableAntiforgery();
        }

        // ------------------------------------------------------------------ yetki

        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, AnaAdres);
        }

        /// <summary>Tavan girişi menüde yoktur; yetkisi açan sayfadan gelir (il girişi MPTavanIlAna, diğerleri MPTavanAna).</summary>
        public static bool GirisYetkiVar(Kullanici kullanan, string uygulamaKlasoru, string dg)
        {
            bool il = dg == "I" || dg == "i";
            return SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, GirisAdres, il ? IlAnaAdres : AnaAdres);
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

        private static IResult DenetleGiris(HttpContext baglam, IWebHostEnvironment ortam, string dg, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!GirisYetkiVar(kullanan, ortam.ContentRootPath, dg)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        /// <summary>Kod ağacı ve kod adı: ana ekrana ya da tavan girişine (her iki açılışıyla) yetkisi olan.</summary>
        private static IResult DenetleOrtak(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            string kok = ortam.ContentRootPath;
            if (!YetkiVar(kullanan, kok) && !GirisYetkiVar(kullanan, kok, "G") && !GirisYetkiVar(kullanan, kok, "I"))
                return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Bilgi(string bilgi) { return Results.Json(new { bilgi = bilgi }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Eski GenelIslemlerIstemci.HazirlikYiliBul: /BUTCEHAZIRLIK değişkeni, yoksa gelecek yıl.</summary>
        private static int HazirlikYili()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
        }

        /// <summary>Eski AdBul: kurumsal yapı (53) dışındaki kodlar "yıl-kod" koşuluyla aranır.</summary>
        private static string AdBul(int yil, int kodTur, string kod)
        {
            kod = Temiz(kod);
            if (kod == "") return "";
            try
            {
                if (kodTur != 53) kod = yil + "-" + kod;
                return GenelIslemler.KodAd(kodTur, kod, true) ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>Eski DdlYapiSecimDoldur: kırılım sayısından "Hepsi", "n. Bölüme Kadar", "Yok" seçenekleri (değer: bölüm sayısı).</summary>
        private static List<object> KirilimSecenek(string kodYapi)
        {
            List<object> l = new List<object>();
            if (string.IsNullOrEmpty(kodYapi)) return l;
            int boy = kodYapi.Split('.').Length;
            for (int i = boy; i >= 0; i--)
            {
                string ad = i == 0 ? "Yok" : i == boy ? "Hepsi" : string.Format("{0}. Bölüme Kadar", i);
                l.Add(new { kod = i, ad = ad });
            }
            return l;
        }

        // ------------------------------------------------------------------ açılış, kod ağacı, kod adı

        /// <summary>Açılış (eski Page_Load): yıllar (bu yıl - 6 .. bu yıl + 1), hazırlık yılı, son aranan tertip, kırılım seçenekleri.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int buYil = DateTime.Now.Year;
                int secYil = HazirlikYili();
                List<int> yillar = new List<int>();
                for (int y = buYil + 1; y >= buYil - 6; y--) yillar.Add(y);
                if (!yillar.Contains(secYil)) yillar.Insert(0, secYil);

                // Eski KullaniciDegiskenDoldur: son listelenen tertip ("aramaTertip"); kurumsal boşsa varsayılan kurum.
                string kur = "", fon = "", fin = "", eko = "", peb = "";
                try
                {
                    string kayitli = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "aramaTertip");
                    Tertip t = new Tertip(secYil, ENUMTabloKod.TABPEBOdenek);
                    if (!string.IsNullOrWhiteSpace(kayitli)) t.TertipAta(kayitli);
                    kur = t.kodlar[0] ?? ""; fon = t.kodlar[1] ?? ""; fin = t.kodlar[2] ?? ""; eko = t.kodlar[3] ?? ""; peb = t.kodlar[4] ?? "";
                }
                catch (Exception) { }
                if (string.IsNullOrWhiteSpace(kur)) kur = GenelIslemlerIstemci.VarsayilanKurumBul() ?? "";

                Tertip tt = new Tertip(secYil, ENUMTabloKod.TABTasraOdenek);
                return Results.Json(new
                {
                    yil = secYil,
                    yillar = yillar,
                    kur = kur, fon = fon, fin = fin, eko = eko, peb = peb,
                    kurAd = AdBul(secYil, 1, kur), fonAd = AdBul(secYil, 2, fon), finAd = AdBul(secYil, 3, fin),
                    ekoAd = AdBul(secYil, 4, eko), pebAd = AdBul(secYil, 58, peb),
                    kirilimlar = new
                    {
                        kur = KirilimSecenek(tt.Kirilimlar(0)), fon = KirilimSecenek(tt.Kirilimlar(1)), fin = KirilimSecenek(tt.Kirilimlar(2)),
                        eko = KirilimSecenek(tt.Kirilimlar(3)), peb = KirilimSecenek(tt.Kirilimlar(4)), hrc = KirilimSecenek(tt.Kirilimlar(5))
                    },
                    yonetici = Yonetici(kullanan)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Kod ağacı (eski KodAgac.aspx?kodTur=): 1, 2, 3, 4, 14 bütçe kodları; 17 kurumsal yapı (harcama birimi).</summary>
        private static IResult Kodlar(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kod, string ust)
        {
            IResult ret = DenetleOrtak(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return KodAgaci(kullanan, yil, kod, ust);
        }

        /// <summary>
        /// Kod ağacının bir düzeyi (yetki denetimi çağıranda): 1, 2, 3, 4, 14 bütçe kodları; 17 kurumsal yapı. Masraf Planı ve
        /// Masraf Planı Dağıtım ekranları da (MasrafPlaniUclari, MPDagitimUclari) kendi yetkileriyle bunu kullanır.
        /// </summary>
        public static IResult KodAgaci(Kullanici kullanan, int yil, string kod, string ust)
        {
            kod = Temiz(kod);
            if (kod == "17") return KurumsalYapiDuzeyi(kullanan, yil, ust);
            if (kod != "1" && kod != "2" && kod != "3" && kod != "4" && kod != "14") return Hata("Kod türü hatalı.");
            return ButceRakamUclari.KodDuzeyi(kullanan, yil, kod, ust);
        }

        /// <summary>Yazılan kodun adı (yetki denetimi çağıranda): 1, 2, 3, 4, 58 bütçe kodları, 53 kurumsal yapı.</summary>
        public static IResult KodAdiBul(int yil, int tur, string kod)
        {
            if (Array.IndexOf(new[] { 1, 2, 3, 4, 53, 58 }, tur) < 0) return Hata("Kod türü hatalı.");
            return Results.Json(new { ad = AdBul(yil, tur, kod) });
        }

        /// <summary>Kodun adı; kurumsal yapı (53) dışındaki kodlar "yıl-kod" koşuluyla aranır. Bulunamazsa boş.</summary>
        public static string KodAd(int yil, int kodTur, string kod)
        {
            return AdBul(yil, kodTur, kod);
        }

        /// <summary>Kurumsal yapı ağacının bir düzeyi (eski KodAgac.aspx?kodTur=17).</summary>
        private static IResult KurumsalYapiDuzeyi(Kullanici kullanan, int yil, string ust)
        {
            if (yil < 2000) return Hata("Yıl hatalı!");
            ust = Temiz(ust);
            try
            {
                const string kodTur = "17";
                string kirilim = TNS.DEG.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD" + kodTur + "/KODYAPI") ?? "";
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
                ABSKodlarVAN kodlar = TNS.BKD.Arac.Tanimla().KodListeleSeviyeIkiYil(yil, kodTur, ust, 0, false, false, "", "", "", true, true, "", false);
                if (kodlar == null || !kodlar.sonuc.islemSonuc) return Hata("Kodlar okunamadı.");
                string dil = string.IsNullOrEmpty(kullanan.aktifKultur) ? TNS.BKD.Arac.Diller(0) : kullanan.aktifKultur;
                foreach (ABSKod k in kodlar.absKodlar)
                    dugumler.Add(new { kod = k.kod, ad = k.kod + "- " + k.Ad(dil), altVar = altVar });
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Kodlar okunamadı.");
            }
        }

        /// <summary>Yazılan kodun adı (eski KodAdGetir): 1 kurumsal, 2 fonksiyonel, 3 finans, 4 ekonomik, 58 alt faaliyet, 53 kurumsal yapı.</summary>
        private static IResult KodAdi(HttpContext baglam, IWebHostEnvironment ortam, int yil, int tur, string kod)
        {
            IResult ret = DenetleOrtak(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return KodAdiBul(yil, tur, kod);
        }

        // ------------------------------------------------------------------ listele

        /// <summary>Eski OdenekleriAl: yıl, onaylı senaryo ve kurumsal kod denetlenir; tamKontrol'de kurumsal tam ya da başka bir kod da girilmeli.</summary>
        private static OdenekBilgi[] OdenekleriAl(Kullanici kullanan, MPTavanSecim s, bool tamKontrol, out string hataStr, out string SENARYOKOD, out Tertip tertip)
        {
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            hataStr = "";
            tertip = null;
            SENARYOKOD = "";
            int secYil = s.Yil;
            if (secYil == 0)
                hataStr += "Yıl alanı boş boş bırakılamaz!\n";
            else
            {
                SENARYOKOD = servis.OnayliAsamaBul(secYil);
                if (string.IsNullOrEmpty(SENARYOKOD))
                    hataStr += "Bu yıla ait onaylı senaryo bulunmamaktadır!\n";
            }

            string kur = Temiz(s.Kur), fon = Temiz(s.Fon), fin = Temiz(s.Fin), eko = Temiz(s.Eko), peb = Temiz(s.Peb);
            if (string.IsNullOrEmpty(kur))
                hataStr += "Kurumsal kod alanı boş bırakılamaz!\n";
            if (tamKontrol)
            {
                if (kur.Replace(".", "").Length != 8)
                    if (fon == "" && fin == "" && eko == "" && peb == "")
                    {
                        hataStr += "Listeleme yapmak için, Kurumsal kodu tam doldurmak veya Fonksiyonel, Finansal, Ekonomik, PEB kod alanlarından birisini daha girmek zorundasınız.";
                    }
            }
            if (hataStr == "")
            {
                tertip = new Tertip(secYil, ENUMTabloKod.TABPEBOdenek, kur, fon, fin, eko, peb);
                return servis.MPTavanGenelListele(kullanan, SENARYOKOD, tertip);
            }
            return null;
        }

        private static MPTavanSecim Secim(int yil, string kur, string fon, string fin, string eko, string peb)
        {
            return new MPTavanSecim { Yil = yil, Kur = kur, Fon = fon, Fin = fin, Eko = eko, Peb = peb };
        }

        /// <summary>
        /// Listele (eski OdenekListele): KBÖ, MP tavan ve detay toplamı, birim talebi, gönderilenlerden en az biri sıfırdan büyük olan
        /// tertipler; il izni her tertip için ayrıca okunur. Toplamlar eskiden sütun başlıklarındaydı; burada toplam satırıdır.
        /// </summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, string kur, string fon, string fin, string eko, string peb)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                OdenekBilgi[] odnler = OdenekleriAl(kullanan, Secim(yil, kur, fon, fin, eko, peb), true, out string hataStr, out string SENARYOKOD, out Tertip tertip);
                if (hataStr != "") return Hata(hataStr.Trim());
                if (odnler == null || odnler.Length == 0) return Hata("Aranan kriterlere uygun tertip bulunamadı!");

                // Eski KullaniciDegiskenSakla: son listelenen tertip.
                try
                {
                    TNSCollection tc = new TNSCollection();
                    tc.Add(tertip.TertipStrOl());
                    kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), "aramaTertip", tc);
                }
                catch (Exception) { }

                List<object> satirlar = new List<object>();
                foreach (OdenekBilgi od in odnler)
                {
                    if (od.EkRakam(0) > 0 || od.EkRakam(1) > 0 || od.EkRakam(2) > 0 || od.EkRakam(3) > 0)
                    {
                        bool ilIzni = servis.MPTavanIlIzniBul(kullanan, od.tertip);
                        satirlar.Add(new
                        {
                            tertip = od.tertip.TertipStrOl(),
                            tertipAdi = od.tertip.adlar[3] + " (" + od.tertip.adlar[4] + ")",
                            odenek = od.EkRakam(0),
                            tavan = od.EkRakam(1),
                            detay = od.EkRakam(2),
                            talep = od.EkRakam(3),
                            dagitilmayan = od.EkRakam(0) - od.EkRakam(1),
                            gonderilen = od.EkRakam(4),
                            ilIzni = ilIzni,
                            oncGonderilen = od.EkRakam(5),
                            oncHarcanan = od.EkRakam(6),
                        });
                    }
                }
                if (satirlar.Count == 0) return Hata("Aranan kriterlere uygun tertip bulunamadı!");
                return Results.Json(new { senaryo = SENARYOKOD, satirlar = satirlar });
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Listeleme yapılamadı." : ex.Message);
            }
        }

        // ------------------------------------------------------------------ il izni, onay

        /// <summary>İl İzni (eski MPTavanIlGirisDegistir.aspx): tertibin il bazında tavan girişi açılır / kapatılır.</summary>
        private static async Task<IResult> IlIzni(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPTavanIlIzniIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPTavanIlIzniIstegi();
            string tertipStr = Temiz(istek.Tertip);
            if (istek.Yil <= 0 || tertipStr == "") return Hata("Yıl ve tertip boş olamaz.");
            try
            {
                Tertip t = new Tertip(istek.Yil, ENUMTabloKod.TABPEBOdenek);
                t.TertipAta(tertipStr);
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MPIlIzniDegistir(kullanan, t);
                if (s == null || !s.islemSonuc) return Hata("İşlem gerçekleştirilememiştir:" + (s?.hataStr ?? ""));
                string sonucStr = s.anahtar == "1" ? "açılmıştır" : "kapatılmıştır";
                return Results.Json(new
                {
                    ilIzni = s.anahtar == "1",
                    bilgi = string.Format("{0} yılında {1} tertibi için, il bazında tavan girişi işlemi {2}.", istek.Yil, tertipStr, sonucStr)
                });
            }
            catch (Exception ex)
            {
                return Hata("İşlem gerçekleştirilememiştir:" + ex.Message);
            }
        }

        /// <summary>
        /// Onay penceresi (eski GonderileniFazlalariYukle): masraf planı gönderilenden az olan tertipler. Varsayılan kurum ile ekonomik ve
        /// alt faaliyet koduna göre aranır; onayKod 1 ve 3'te tavan, diğerlerinde detay.
        /// </summary>
        private static IResult FazlaGonderilen(HttpContext baglam, IWebHostEnvironment ortam, int yil, string eko, string peb, int onayKod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0) return Hata("Yıl alanı boş boş bırakılamaz!");
            try
            {
                Tertip tertip = new Tertip(yil, ENUMTabloKod.TABPEBOdenek, GenelIslemlerIstemci.VarsayilanKurumBul(), "", "", Temiz(eko), Temiz(peb));
                bool detay = !(onayKod == 1 || onayKod == 3);
                ObjectArray oa = TNS.BHMv2.Arac.Tanimla().MPFazlaGonderilen(kullanan, detay ? 1 : 0, tertip);
                if (!oa.sonuc.islemSonuc) return Hata(oa.sonuc.hataStr);
                List<object> satirlar = new List<object>();
                foreach (object o in oa.objeler)
                {
                    TertipOdenekBilgi to = (TertipOdenekBilgi)o;
                    satirlar.Add(new { TertipKod = to.TertipKod, Odenek0 = to.Odenek0, Odenek1 = to.Odenek1 });
                }
                string mesaj = oa.ObjeSayisi == 0
                    ? "Tüm ödeneklerin, masraf planları gönderileninden fazla. Onaylama işlemi yapabilirsiniz!"
                    : "listelenen tertip ve birimlerde, masraf planları gönderilenden azdır. Bu tertipleri düzeltmek yıl içi işlemler açısından daha iyi olacaktır. Bu duruma rağmen onaylama işlemi için Onayla tuşuna basabilirsiniz!";
                return Results.Json(new { satirlar = satirlar, bilgi = mesaj });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        /// <summary>
        /// Onay durumu (eski OnayDurumDegistir): 1 Onayla Tavan, 2 Onayla Detay, 3 / 4 Tavanı / Detayı veri girişine kapat,
        /// 5 / 6 Tavanı / Detayı veri girişine aç (3-6 yönetici işlemleri). Alt faaliyet kodu zorunludur.
        /// </summary>
        private static async Task<IResult> Onay(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPTavanOnayIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPTavanOnayIstegi();
            if (istek.Kod < 1 || istek.Kod > 6) return Hata("İşlem hatalı.");
            if (istek.Kod >= 3 && !Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            int onayKod, detayMi;
            switch (istek.Kod)
            {
                case 1: onayKod = 1; detayMi = 0; break;
                case 2: onayKod = 1; detayMi = 1; break;
                case 3: onayKod = 2; detayMi = 0; break;
                case 4: onayKod = 2; detayMi = 1; break;
                case 5: onayKod = 0; detayMi = 0; break;
                default: onayKod = 0; detayMi = 1; break;
            }
            string hataStr = "";
            if (istek.Yil == 0)
                hataStr += "Yıl alanı boş boş bırakılamaz!\n";
            string peb = Temiz(istek.Peb);
            if (string.IsNullOrEmpty(peb))
                hataStr += "Alt faaliyet kod alanı boş bırakılamaz!\n";
            if (hataStr != "") return Hata("Onaylama yapılamadı." + hataStr.Trim());
            try
            {
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MPTavanOnay(kullanan, istek.Yil, detayMi, peb, onayKod);
                if (s != null && s.islemSonuc) return Bilgi("Onaylama İşlemi Başarılı.");
                return Hata("Onaylama yapılamadı." + (s?.hataStr ?? ""));
            }
            catch (Exception ex)
            {
                return Hata("Onaylama yapılamadı." + ex.Message);
            }
        }

        // ------------------------------------------------------------------ raporlar

        private static Hashtable RaporParam(int yil, string kur, string fon, string fin, string eko, string peb)
        {
            Hashtable h = new Hashtable();
            h["ButceYil"] = yil.ToString();
            h["KurKod"] = Temiz(kur);
            h["FonKod"] = Temiz(fon);
            h["FinKod"] = Temiz(fin);
            h["EkoKod"] = Temiz(eko);
            h["PebKod"] = Temiz(peb);
            return h;
        }

        private static string Ad(string onEk)
        {
            return onEk + DateTime.Now.Ticks.ToString() + "." + GenelIslemler.ExcelTur();
        }

        /// <summary>
        /// Raporlar (hepsi Excel):
        ///   genel        Hata Raporları > Genel Liste (liste 1 hepsi, 2 tavanı fazla, 3 dağıtılanı fazla): MPRaporDetay TavanIcmDetay
        ///   dagitilmayan Hata Raporları > Dağıtılmayanlar (eski btnProblemliler_Click)
        ///   gondfazla    Hata Raporları > Gönderileni Fazla Olanlar (detay 0 tavan, 1 detay; eski GonderileniFazlaOlanlar)
        ///   tavan        Rapor > Yazdır (harcama birimi ve kırılımlarla; MPRaporDetay Tavan)
        ///   ho           Rapor > H.Öd.Üret (cezali 0) / H.Öd.(ÖE>=Say) (cezali 1), oran (MPRaporDetay HO)
        ///   uret         Yönetici İşlemleri > Üretim Ekranı (kaynak 0 ödenek gönderme, 1 harcama; artış oranı)
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string rapor, int yil, string kur, string fon, string fin, string eko, string peb,
            string hrc, int? liste, int? detay, int? cezali, string oran, int? kaynak,
            string kurUz, string fonUz, string finUz, string ekoUz, string pebUz, string hrcUz)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            rapor = Temiz(rapor);
            if (yil <= 0) return Hata("Yıl alanı boş boş bırakılamaz!");
            try
            {
                AsposeLisans.Yukle();
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                MPRaporDetay d = new MPRaporDetay(ortam.ContentRootPath);
                Hashtable h = RaporParam(yil, kur, fon, fin, eko, peb);
                string uz = GenelIslemler.ExcelTur();
                switch (rapor)
                {
                    case "genel":
                        {
                            h["Tur"] = (liste >= 1 && liste <= 3 ? liste.Value : 1).ToString();
                            string sonucDosyaAd = d.RaporAl(kullanan, servis, "", "", uz, h, "TavanIcmDetay");
                            return DosyaIslem.DosyaGonder(sonucDosyaAd, Ad("MPRapor_"), true, uz);
                        }
                    case "tavan":
                        {
                            h["HrcKod"] = Temiz(hrc);
                            h["KurKodUz"] = Temiz(kurUz);
                            h["FonKodUz"] = Temiz(fonUz);
                            h["FinKodUz"] = Temiz(finUz);
                            h["EkoKodUz"] = Temiz(ekoUz);
                            h["PebKodUz"] = Temiz(pebUz);
                            h["HrcKodUz"] = Temiz(hrcUz);
                            string sonucDosyaAd = d.RaporAl(kullanan, servis, "", "", uz, h, "Tavan");
                            return DosyaIslem.DosyaGonder(sonucDosyaAd, Ad("MPRapor_"), true, uz);
                        }
                    case "ho":
                        {
                            h["HrcKod"] = Temiz(hrc);
                            h["cezali"] = cezali == 1 ? "1" : "0";
                            h["oran"] = OrtakFonksiyonlar.ConvertToDouble(Temiz(oran).Replace(',', '.'), 0).ToString();
                            string dosyaAd = DosyaIslem.DosyaAdUret("Hizli", "");
                            string sonucDosyaAd = d.RaporAl(kullanan, servis, "", dosyaAd, uz, h, "HO");
                            return DosyaIslem.DosyaGonder(sonucDosyaAd, Ad("HO_"), true, uz);
                        }
                    case "dagitilmayan":
                        return Dagitilmayanlar(kullanan, Secim(yil, kur, fon, fin, eko, peb));
                    case "gondfazla":
                        return GonderileniFazlaOlanlar(kullanan, Secim(yil, kur, fon, fin, eko, peb), detay == 1);
                    case "uret":
                        if (!Yonetici(kullanan)) return Results.StatusCode(StatusCodes.Status403Forbidden);
                        return Uret(kullanan, Secim(yil, kur, fon, fin, eko, peb), kaynak == 0, oran);
                    default:
                        return Hata("Rapor türü hatalı.");
                }
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Rapor hazırlanamadı." : ex.Message);
            }
        }

        /// <summary>Gönderileni Fazla Olanlar (Tavan / Detay) (eski GonderileniFazlaOlanlar).</summary>
        private static IResult GonderileniFazlaOlanlar(Kullanici kullanan, MPTavanSecim s, bool detay)
        {
            Tertip tertip = new Tertip(s.Yil, ENUMTabloKod.TABPEBOdenek, Temiz(s.Kur), Temiz(s.Fon), Temiz(s.Fin), Temiz(s.Eko), Temiz(s.Peb));
            ObjectArray oa = TNS.BHMv2.Arac.Tanimla().MPFazlaGonderilen(kullanan, detay ? 1 : 0, tertip);
            if (!oa.sonuc.islemSonuc) return Hata(oa.sonuc.hataStr);

            Tablo XLS = GenelIslemler.NewTablo();
            string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.BosDosyaAc(dosyaAd);

            int sat = 0;

            XLS.HucreDegerYaz(0, 0, tertip.yil + " Yılı  Gönderilen Ödeneği Fazla Olanlar");
            XLS.HucreBirlestir(0, 0, 0, 2);
            XLS.YatayHizala(0, 0, 2);
            XLS.DuseyHizala(0, 0, 2);

            sat = 2;
            XLS.HucreDegerYaz(sat, 0, "Tertip");
            XLS.HucreDegerYaz(sat, 1, "Masraf Planı");
            XLS.HucreDegerYaz(sat, 2, "Gönderilen");

            for (int islemSatir = 0; islemSatir < oa.ObjeSayisi; islemSatir++)
            {
                sat++;
                TertipOdenekBilgi od = (TertipOdenekBilgi)oa.objeler[islemSatir];

                XLS.HucreDegerYaz(sat, 0, od.tertip);
                XLS.HucreDegerYaz(sat, 1, od.odenekler[1]);
                XLS.HucreDegerYaz(sat, 2, od.odenekler[0]);
            }
            XLS.SutunGenislikAyarla(0, 0, 55);
            XLS.SutunGenislikAyarla(1, 2, 15);
            if (sat > 2)
                XLS.HucreFormatla(3, 1, sat, 2, "#,##0");
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(dosyaAd, "MasrafPlaniGondFazla_" + DateTime.Now.Ticks.ToString() + ".xlsx", true, "xlsx");
        }

        /// <summary>Dağıtılmayanlar (eski btnProblemliler_Click): KBÖ'sü olup tavanı olmayan ya da tavanı detayına eşit olmayan tertipler.</summary>
        private static IResult Dagitilmayanlar(Kullanici kullanan, MPTavanSecim s)
        {
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            Tablo XLS = GenelIslemler.NewTablo();
            string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.BosDosyaAc(dosyaAd);

            int sat = 0;
            int sut = 0;

            OdenekBilgi[] odnler = OdenekleriAl(kullanan, s, false, out string hataStr, out string SENARYOKOD, out Tertip tertip);
            if (hataStr != "")
                XLS.HucreDegerYaz(sat, sut, hataStr);
            else
            {
                XLS.HucreDegerYaz(0, 0, tertip.yil + " YILI " + SENARYOKOD + " Aşaması Masraf Planı ");
                XLS.HucreBirlestir(0, 0, 0, 7);
                XLS.YatayHizala(0, 0, 2);
                XLS.DuseyHizala(0, 0, 2);
                XLS.HucreDegerYaz(1, 0, "Birimlere Dağıtılmayan Ödenekler");
                XLS.HucreBirlestir(1, 0, 1, 7);
                XLS.YatayHizala(1, 0, 2);
                XLS.DuseyHizala(1, 0, 2);

                sat = 2;
                XLS.HucreDegerYaz(sat, sut++, "Tertip");
                XLS.HucreDegerYaz(sat, sut++, "Tertip Ad");

                XLS.HucreDegerYaz(sat, sut++, "KBÖ");
                XLS.HucreDegerYaz(sat, sut++, "Birim Talep");
                XLS.HucreDegerYaz(sat, sut++, "MP Tavan Top.");
                XLS.HucreDegerYaz(sat, sut++, "MP Detay Top.");
                XLS.HucreDegerYaz(sat, sut++, "Dağıtılmayan");
                XLS.HucreDegerYaz(sat++, sut++, "İl İzni");

                for (int islemSatir = 0; islemSatir < odnler.Length; islemSatir++)
                {
                    OdenekBilgi od = odnler[islemSatir];

                    string tertipKod = od.tertip.TertipStrOl();
                    string tertipAdi = od.tertip.adlar[3] + " (" + od.tertip.adlar[4] + ")";
                    double odenek = od.EkRakam(0);
                    double tavan = od.EkRakam(1);
                    double detay = od.EkRakam(2);
                    double talep = od.EkRakam(3);
                    double dagitilmayan = od.EkRakam(0) - od.EkRakam(1);

                    if (odenek > 0 && tavan == 0 || tavan != detay)
                    {
                        bool ilIzni = servis.MPTavanIlIzniBul(kullanan, od.tertip);
                        sut = 0;
                        XLS.HucreDegerYaz(sat, sut++, tertipKod);
                        XLS.HucreDegerYaz(sat, sut++, tertipAdi);

                        XLS.HucreDegerYaz(sat, sut++, odenek);
                        XLS.HucreDegerYaz(sat, sut++, talep);
                        XLS.HucreDegerYaz(sat, sut++, tavan);
                        XLS.HucreDegerYaz(sat, sut++, detay);
                        XLS.HucreDegerYaz(sat, sut++, dagitilmayan);
                        XLS.HucreDegerYaz(sat++, sut++, ilIzni ? 1 : 0);
                    }
                }
                sut = 0;
                XLS.SutunGenislikAyarla(sut, sut++, 36);
                XLS.SutunGenislikAyarla(sut, sut++, 70);

                XLS.SutunGenislikAyarla(sut, sut + 5, 12);
            }

            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(dosyaAd, "MasrafPlaniDagitilmayan_" + DateTime.Now.Ticks.ToString() + ".xlsx", true, "xlsx");
        }

        /// <summary>
        /// Üretim Ekranı (eski urtUret_Click): kriterlere uyan tertiplerin bütün birimlerdeki ödenek gönderme ya da harcama değerleri,
        /// verilen oranda arttırılarak dosyaya yazılır; dosya tavan girişi ekranında (merkez detay) yüklenebilir.
        /// </summary>
        private static IResult Uret(Kullanici kullanan, MPTavanSecim s, bool gonderme, string oranStr)
        {
            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            string hataStr = "";
            int secYil = s.Yil;
            if (secYil == 0)
                hataStr += "Yıl alanı boş boş bırakılamaz!\n";

            string SENARYOKOD = servis.OnayliAsamaBul(secYil);
            if (string.IsNullOrEmpty(SENARYOKOD))
                hataStr += "Bu yıla ait onaylı senaryo bulunmamaktadır!\n";

            if (string.IsNullOrEmpty(Temiz(s.Kur)))
                hataStr += "Kurumsal kod alanı boş bırakılamaz!\n";

            if (!string.IsNullOrWhiteSpace(hataStr))
                return Hata(hataStr.Trim());
            ENUMTabloKod odenekTur = gonderme ? ENUMTabloKod.TABOdenekGondermeKY : ENUMTabloKod.TABMIFKY;
            EkDegerParam[] ekBilgiler = new EkDegerParam[1];
            ekBilgiler[0] = new EkDegerParam(odenekTur, -1, 0, "");

            Tertip tertip = new Tertip(secYil, odenekTur, Temiz(s.Kur), Temiz(s.Fon), Temiz(s.Fin), Temiz(s.Eko), Temiz(s.Peb));

            if (!servis.OGYYetkisiVarMi(kullanan, null, tertip, ENUMOGY.Gorme))
                return Hata("Görme yetkiniz yok!");

            bool eko23;
            bool eko2 = TNS.MUH.Arac.OdenekGondermeEko2liMi(secYil);
            if (eko2)
                eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
            else
                eko23 = false;

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            for (int i = 0; i < tertip.KodNoSay(); i++)
            {
                int tut = tertip.KodNoKirilimSay(i);
                bool eko23ic;
                if (i == 3 && eko2)
                {
                    tut = 2;
                    eko23ic = eko23;
                }
                else
                    eko23ic = false;
                kodBilgileri.Add(new UcgenKodBilgi(tertip.KodNolar(i), tut, eko23ic));
            }

            OdenekBilgi[] odnler = servis.ListeTertipOl(kullanan, tertip, kodBilgileri, ekBilgiler, true, false, false, -1);

            Tablo XLS = GenelIslemler.NewTablo();
            string dosyaAd = DosyaIslem.DosyaAdUret() + ".xlsx";
            XLS.BosDosyaAc(dosyaAd);

            int sat;
            int sut = 0;

            double yuzde = OrtakFonksiyonlar.ConvertToDouble(Temiz(oranStr).Replace(',', '.'), 0);
            sat = 0;
            XLS.HucreDegerYaz(sat, sut++, "tertip");
            XLS.HucreDegerYaz(sat, sut++, "kurumsalyapi");
            XLS.HucreDegerYaz(sat, sut++, "tavan");
            XLS.HucreDegerYaz(sat, sut++, gonderme ? "Gönderilen" : "Harcanan");

            for (int islemSatir = 0; islemSatir < odnler.Length; islemSatir++)
            {
                sat++;
                OdenekBilgi od = odnler[islemSatir];
                string kyKod = od.tertip.kodlar[5];
                od.tertip.kodlar[5] = "";
                string tertipKod = od.tertip.TertipStrOl();
                double odenek = od.EkRakam(0);
                sut = 0;
                XLS.HucreDegerYaz(sat, sut++, tertipKod);
                XLS.HucreDegerYaz(sat, sut++, kyKod);
                XLS.HucreDegerYaz(sat, sut++, OrtakFonksiyonlar.YuvarlaAsagi(odenek + (odenek * yuzde) / 100));
                XLS.HucreDegerYaz(sat, sut++, odenek);
            }
            sut = 0;
            XLS.SutunGenislikAyarla(sut, sut++, 70);
            XLS.SutunGenislikAyarla(sut, sut + 2, 36);
            XLS.DosyaSaklaTamYol();
            return DosyaIslem.DosyaGonder(dosyaAd, "MasrafPlaniYukle_" + DateTime.Now.Ticks.ToString() + ".xlsx", true, "xlsx");
        }

        // ------------------------------------------------------------------ dosya yükle, merkez yükle

        private static IResult DosyaDenetle(IFormFile dosya)
        {
            if (dosya == null) return Hata("Yükleme için bir dosya seçiniz");
            if (dosya.Length == 0) return Hata("Yüklemek istediğiniz dosyanın içi boş.");
            if (dosya.Length > 20 * 1024 * 1024) return Hata("Dosya çok büyük (en çok 20 MB).");
            return null;
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

        /// <summary>
        /// Eski BirimAta: tertibin onaylı senaryodaki ödenekleri (ekonomik kod 2. düzeyde, alt kırılımlarıyla) verilen kurumsal yapıya
        /// tavan olarak yazılır. Hata ya da uyarı metni döner; başarılıysa boş.
        /// </summary>
        private static string BirimAta(IBHMv2Servis servis, Kullanici kullanan, bool eko23, string SENARYOKOD, Tertip tertip, string kurumsalYapiKod)
        {
            string uyari;
            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();
            for (int i = 0; i < tertip.kodlar.Length; i++)
            {
                bool eko23ic;
                int say = 0;

                if (i == 3)
                {
                    say = 2;
                    eko23ic = eko23;
                }
                else
                {
                    say = tertip.KodNoKirilimSay(i);
                    eko23ic = false;
                }
                kodBilgileri.Add(new UcgenKodBilgi(tertip.KodNolar(i), say, eko23ic));
            }

            TertipOdenek[] odnler = servis.ButceOdenekListele(kullanan, SENARYOKOD, tertip, null, 0, kodBilgileri);
            ArrayList al = new ArrayList();
            for (int i = 0; i < odnler.Length; i++)
            {
                TertipOdenek to = new TertipOdenek(1);
                to.tertip = new Tertip(tertip.yil, ENUMTabloKod.TABTasraOdenek);
                to.tertip.TertipAta(odnler[i].tertip.TertipStrOl() + "-" + kurumsalYapiKod);
                to.odenek[0] = odnler[i].odenek[0];
                al.Add(to);
            }
            if (al.Count == 0)
                uyari = string.Format("Kaydedilecek satır bulunamadı. Lütfen gereksiz boş sütun ve satırları temizleyiniz ve sayfa parametresi olarak verdiğiniz tertibin ({0}), gönderdiğiniz dosya içinde olup olmadığını kontrol ediniz!", tertip.TertipStrOl());
            else
            {
                Sonuc s = servis.MPTavanKaydet(kullanan, SENARYOKOD, (TertipOdenek[])al.ToArray(typeof(TertipOdenek)), true, kurumsalYapiKod, true, "");
                if (!s.islemSonuc)
                    uyari = s.hataStr;
                else
                    uyari = "";
            }

            return uyari;
        }

        /// <summary>
        /// Dosya Yükle (eski btnDosyaYukle_Click): 1. satırdan başlayarak sütunlar tertip, saymanlık, birim. Her satırda tertip şablonuna
        /// uyan tertiplerin KBÖ'leri o saymanlık / birimin kurumsal yapısına tavan olarak atanır.
        /// </summary>
        private static async Task<IResult> DosyaYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            IFormCollection form = await baglam.Request.ReadFormAsync();
            IFormFile dosya = form.Files["dosya"];
            ret = DosyaDenetle(dosya);
            if (ret != null) return ret;
            int secYil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], DateTime.Now.Year);

            string genelMesaj = "";
            string dosyaAdxls;
            try
            {
                dosyaAdxls = await DosyaAl(dosya);
            }
            catch (Exception)
            {
                return Hata("Dosya sunucuda saklanamadı");
            }
            try
            {
                AsposeLisans.Yukle();
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                bool eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);
                string SENARYOKOD = servis.OnayliAsamaBul(secYil);

                Tablo XLS = GenelIslemler.NewTablo();
                XLS.DosyaOkuAc(dosyaAdxls);
                int sat = 0;
                while (true)
                {
                    string tertipStr = XLS.HucreDegerAl(sat, 0);
                    string uyari = "";
                    if (string.IsNullOrEmpty(tertipStr)) break;
                    Tertip tertip = new Tertip(secYil, ENUMTabloKod.TABPEBOdenek);
                    tertip.TertipAta(tertipStr);
                    string saymanlik = XLS.HucreDegerAl(sat, 1);
                    string birim = XLS.HucreDegerAl(sat, 2);
                    string kurumsalYapiKod = "";
                    string[] kurumsalYapiKodDizi = servis.KurumsalYapiListele(saymanlik, birim, tertip.kodlar[0]);
                    if (kurumsalYapiKodDizi != null && kurumsalYapiKodDizi.Length > 0)
                        kurumsalYapiKod = kurumsalYapiKodDizi[0];
                    else
                        uyari = string.Format("Verilen saymanlık({0}) - birim({1}) bulunamadı.", saymanlik, birim);
                    sat++;

                    if (string.IsNullOrEmpty(uyari))
                    {
                        uyari = BirimAta(servis, kullanan, eko23, SENARYOKOD, tertip, kurumsalYapiKod);
                    }
                    if (!string.IsNullOrEmpty(uyari))
                        genelMesaj += "\n" + uyari;
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

        /// <summary>
        /// Merkez Yükle (eski btnMerkezYukle_Click): merkez birimlerinin (1 ile başlayan 11 haneli kurumsal yapı) kurumsal kodu tek bir
        /// merkez birimine bağlıysa, o kurumsal koda ait ödenekler o birime tavan olarak atanır.
        /// </summary>
        private static async Task<IResult> MerkezYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPTavanYilIstegi istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            istek ??= new MPTavanYilIstegi();
            int secYil = istek.Yil > 0 ? istek.Yil : DateTime.Now.Year;
            try
            {
                IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                string[] sql = new string[1];
                string[] baglanti = new string[1];
                sql[0] = "SELECT KOD, SAYMANLIKKOD, BIRIMKOD, KURKOD FROM BHMKURUMSALYAPI where KOD LIKE '1%' AND LEN(KOD)=11";
                baglanti[0] = "BaglantiSatiriBUM";
                string SENARYOKOD = servis.OnayliAsamaBul(secYil);
                bool eko23 = TNS.BKD.Arac.Eko2_3Bul(kullanan);

                SortedList sl = uzyServis.SorguBirlestir(sql, baglanti, new int[] { 0 });
                string genelMesaj = "";
                Dictionary<string, int> kurSay = new Dictionary<string, int>();
                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    string kurKod = alVeri.StrAl(2);
                    if (!string.IsNullOrWhiteSpace(kurKod))
                    {
                        if (kurSay.TryGetValue(kurKod, out int say))
                            kurSay[kurKod] = say + 1;
                        else
                            kurSay[kurKod] = 1;
                    }
                }

                foreach (DictionaryEntry de in sl)
                {
                    TNS.UZY.HTArray alAnahtar = (TNS.UZY.HTArray)de.Key;
                    TNS.UZY.HTArray alVeri = (TNS.UZY.HTArray)de.Value;
                    string kurumsalYapiKod = alAnahtar.StrAl(0);
                    string kurKod = alVeri.StrAl(2);
                    if (string.IsNullOrEmpty(kurKod)) continue;
                    if (kurSay.TryGetValue(kurKod, out int say))
                    {
                        if (say > 1) continue;
                        Tertip tertip = new Tertip(secYil, ENUMTabloKod.TABPEBOdenek);
                        tertip.TertipAta(kurKod);

                        string uyari = BirimAta(servis, kullanan, eko23, SENARYOKOD, tertip, kurumsalYapiKod);
                        if (!string.IsNullOrEmpty(uyari))
                            genelMesaj += "\n" + uyari;
                        else
                            genelMesaj += "\n" + tertip.TertipStrOl() + ":" + kurumsalYapiKod + " dağıtıldı!";
                    }
                }
                return Results.Json(new { bilgi = "İşlem tamamlandı. Uyarılar:" + genelMesaj, uyariVar = true });
            }
            catch (Exception ex)
            {
                return Hata(ex.Message);
            }
        }

        // ------------------------------------------------------------------ tavan girişi (eski MPTavanGiris.aspx)

        private static string KyTemiz(string kyKod)
        {
            return Temiz(kyKod).Split('-')[0].Trim();
        }

        /// <summary>
        /// Tavan girişi listesi (eski OdenekListele): tertibin birim tavanları (harcama, birim talep, gönderilen, tavan, detay tavan
        /// toplamı). Üstteki toplam il tavanında tertibin ödeneği (ABSODENEK), detayda kurumsal yapının tavanıdır (MPTAVAN).
        /// </summary>
        private static IResult GirisListe(HttpContext baglam, IWebHostEnvironment ortam, int yil, string senaryo, string tertip, string dg, string kykod, string saybir)
        {
            MPTavanGirisParam sp = new MPTavanGirisParam { Yil = yil, Senaryo = Temiz(senaryo), Tertip = Temiz(tertip), DG = Temiz(dg), KyKod = KyTemiz(kykod), SayBir = Temiz(saybir) };
            IResult ret = DenetleGiris(baglam, ortam, sp.DG, out Kullanici kullanan);
            if (ret != null) return ret;
            if (sp.Yil <= 0 || sp.Tertip == "") return Hata("Yıl ve tertip boş olamaz.");
            try
            {
                Tertip t = new Tertip(sp.Yil, ENUMTabloKod.TABPEBOdenek);
                t.TertipAta(sp.Tertip);
                MPTavanTertip[] odnler = TNS.BHMv2.Arac.Tanimla().MPTavanListele(kullanan, sp.Senaryo, t, sp.KyKod, sp.DetayMi(), sp.SayBir);
                if (odnler == null || odnler.Length == 0) return Hata("Aranan kriterlere uygun tertip bulunamadı!");

                List<MPOdenekBilgi> al = new List<MPOdenekBilgi>();
                double tavanToplam = 0;
                foreach (MPTavanTertip od in odnler)
                {
                    if (od.birimTavanlari == null) continue;
                    foreach (MPTavanDetay bt in od.birimTavanlari)
                    {
                        MPOdenekBilgi ob = new MPOdenekBilgi();
                        ob.birimKod = bt.kurumsalYapi.kod;
                        ob.birimKodAd = bt.kurumsalYapi.kod + " " + bt.kurumsalYapi.ad;
                        ob.harcama = bt.odenek[0];
                        ob.birimTalep = bt.odenek[1];
                        ob.gonderilen = bt.odenek[2];
                        ob.tavan = bt.odenek[3];
                        ob.detayTavan = sp.DetayMi() ? 0 : bt.odenek[4];
                        tavanToplam += bt.odenek[3];
                        al.Add(ob);
                    }
                }

                TNS.UZY.IUZYServis uzyServis = TNS.UZY.Arac.Tanimla();
                double toplam;
                if (sp.DetayMi())
                    toplam = uzyServis.UzayDegeriDbl(kullanan, "MPTAVAN", TNS.UZY.Arac.UzayKosulOl(sp.Yil.ToString(), "0", sp.Tertip.Replace(".", ""), sp.KyKod), false);
                else
                    toplam = uzyServis.UzayDegeriDbl(kullanan, "ABSODENEK", TNS.UZY.Arac.UzayKosulOl(sp.Yil.ToString(), sp.Senaryo, "0", sp.Tertip), false);

                return Results.Json(new
                {
                    tertipAdi = odnler[0].tertipOdenek.tertip.tertipAdi ?? "",
                    toplam = toplam,
                    dagitilmayan = toplam - tavanToplam,
                    satirlar = al
                });
            }
            catch (Exception ex)
            {
                return Hata(string.IsNullOrWhiteSpace(ex.Message) ? "Listeleme yapılamadı." : ex.Message);
            }
        }

        /// <summary>Kaydet (eski OdenekKaydet): tablodaki her birimin tavanı tertip + kurumsal yapı olarak yazılır.</summary>
        private static async Task<IResult> GirisKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, MPTavanGirisKayit istek)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            istek ??= new MPTavanGirisKayit();
            istek.DG = Temiz(istek.DG);
            IResult ret = DenetleGiris(baglam, ortam, istek.DG, out Kullanici kullanan);
            if (ret != null) return ret;
            string tertipStr = Temiz(istek.Tertip);
            if (istek.Yil <= 0 || tertipStr == "") return Hata("Yıl ve tertip boş olamaz.");
            List<MPOdenekBilgi> tavanlar = istek.Satirlar ?? new List<MPOdenekBilgi>();
            if (tavanlar.Count == 0) return Hata("Kaydedilecek satır yok.");
            try
            {
                ArrayList al = new ArrayList();
                foreach (MPOdenekBilgi ob in tavanlar)
                {
                    if (ob == null || string.IsNullOrWhiteSpace(ob.birimKod)) continue;
                    TertipOdenek to = new TertipOdenek(1);
                    to.tertip = new Tertip(istek.Yil, ENUMTabloKod.TABTasraOdenek);
                    to.tertip.TertipAta(tertipStr + "-" + ob.birimKod.Trim());
                    to.odenek[0] = ob.tavan;
                    al.Add(to);
                }
                Sonuc s = TNS.BHMv2.Arac.Tanimla().MPTavanKaydet(kullanan, Temiz(istek.Senaryo), (TertipOdenek[])al.ToArray(typeof(TertipOdenek)), istek.DetayMi(), KyTemiz(istek.KyKod), false, Temiz(istek.SayBir));
                if (s != null && s.islemSonuc) return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı" });
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız:" + (s?.hataStr ?? "") });
            }
            catch (Exception ex)
            {
                return Results.Json(new { basarili = false, hata = "Kayıt işlemi başarısız:" + ex.Message });
            }
        }

        /// <summary>
        /// Dosya Yükle (eski MPTavanGiris btnDosyaYukle_Click; merkez detay): 1. satır başlıktır: 'tertip', 'tavan' ve 'kurumsalyapi' ya da
        /// 'saymanlik' + 'birim' sütunları. Satırlar doğrudan kaydedilir (toplam da yazılır).
        /// </summary>
        private static async Task<IResult> GirisYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IFormCollection form = await baglam.Request.ReadFormAsync();
            MPTavanGirisParam sp = new MPTavanGirisParam
            {
                Yil = OrtakFonksiyonlar.ConvertToInt((string)form["yil"], 0),
                Senaryo = Temiz(form["senaryo"]),
                Tertip = Temiz(form["tertip"]),
                DG = Temiz(form["dg"]),
                KyKod = KyTemiz(form["kykod"])
            };
            IResult ret = DenetleGiris(baglam, ortam, sp.DG, out Kullanici kullanan);
            if (ret != null) return ret;
            if (!sp.MerkezDetayMi()) return Results.StatusCode(StatusCodes.Status403Forbidden);
            if (sp.Yil <= 0) return Hata("Yıl boş olamaz.");
            IFormFile dosya = form.Files["dosya"];
            ret = DosyaDenetle(dosya);
            if (ret != null) return ret;

            IBHMv2Servis servis = TNS.BHMv2.Arac.Tanimla();
            ArrayList al = new ArrayList();
            Sonuc s = new Sonuc(true, "", "");
            try
            {
                AsposeLisans.Yukle();
                Tablo XLS = GenelIslemler.NewTablo();
                using (Stream akis = dosya.OpenReadStream())
                    XLS.DosyaOkuAc(akis);

                int sat = 0;
                int tertipSutun = -1;
                int kurumsalYapiSutun = -1;
                int saymanlikSutun = -1;
                int birimSutun = -1;
                int tavanSutun = -1;
                int sutun = 0;
                while (true)
                {
                    string baslik = XLS.HucreDegerAl(sat, sutun).ToLower();
                    if (string.IsNullOrEmpty(baslik))
                        break;
                    if (baslik == "tertip" || baslik.ToLower() == "tertıp")
                        tertipSutun = sutun;
                    else if (baslik == "tavan")
                        tavanSutun = sutun;
                    else if (baslik == "kurumsalyapi" || baslik == "kurumsalyapı")
                        kurumsalYapiSutun = sutun;
                    else if (baslik == "saymanlık" || baslik == "saymanlik")
                        saymanlikSutun = sutun;
                    else if (baslik == "birim" || baslik == "bırım")
                        birimSutun = sutun;
                    sutun++;
                }

                sat = 1;

                if (tertipSutun < 0)
                    s.DegerAta("Başlığı 'tertip' olan sütun bulunamadı!");
                if (tavanSutun < 0)
                    s.DegerAta(s.hataStr + "\n" + "Başlığı 'tavan' olan sütun bulunamadı!");

                if (kurumsalYapiSutun < 0)
                {
                    if (saymanlikSutun < 0 || birimSutun < 0)
                        s.DegerAta(s.hataStr + "\n" + "Başlığı 'saymanlik' ve 'birim' olan sütunlar veya 'kurumsalyapi' olan sütun bulunamadı!");
                }

                while (s.islemSonuc)
                {
                    string tertipStr = XLS.HucreDegerAl(sat, tertipSutun);
                    if (string.IsNullOrEmpty(tertipStr))
                        break;

                    double tavan = Math.Floor(OrtakFonksiyonlar.ConvertToDbl(XLS.HucreDegerAl(sat, tavanSutun)));

                    string kurumsalYapiKod = "";
                    if (kurumsalYapiSutun > -1)
                    {
                        kurumsalYapiKod = XLS.HucreDegerAl(sat, kurumsalYapiSutun);
                    }
                    else
                    {
                        string saymanlikStr = XLS.HucreDegerAl(sat, saymanlikSutun);
                        string birimStr = XLS.HucreDegerAl(sat, birimSutun);
                        string[] kurumsalYapiKodDizi = servis.KurumsalYapiListele(saymanlikStr, birimStr, tertipStr.Replace(".", "").Substring(0, 8));
                        if (kurumsalYapiKodDizi != null && kurumsalYapiKodDizi.Length > 0)
                            kurumsalYapiKod = kurumsalYapiKodDizi[0];
                        else
                            s.DegerAta(string.Format("Verilen saymanlık({0}) - birim({1}) bulunamadı.", saymanlikStr, birimStr));
                    }

                    TertipOdenek to = new TertipOdenek(1);
                    to.tertip = new Tertip(sp.Yil, ENUMTabloKod.TABTasraOdenek);
                    to.tertip.TertipAta(tertipStr + "-" + kurumsalYapiKod);
                    to.odenek[0] = tavan;
                    al.Add(to);

                    sat++;
                }
                XLS.DosyaKapat();
            }
            catch (Exception ex)
            {
                s.DegerAta("Gönderilen dosya okunamadı!" + ex.Message);
            }
            if (s.islemSonuc)
            {
                if (al.Count == 0)
                    s.DegerAta(string.Format("Kaydedilecek satır bulunamadı. Lütfen gereksiz boş sütun ve satırları temizleyiniz ve sayfa parametresi olarak verdiğiniz tertibin ({0}), gönderdiğiniz dosya içinde olup olmadığını kontrol ediniz!", sp.Tertip));
                else
                {
                    try
                    {
                        s = servis.MPTavanKaydet(kullanan, sp.Senaryo, (TertipOdenek[])al.ToArray(typeof(TertipOdenek)), sp.DetayMi(), sp.KyKod, true, "");
                    }
                    catch (Exception ex)
                    {
                        s = new Sonuc(false, ex.Message, "");
                    }
                }
            }
            if (s.islemSonuc) return Bilgi("Yükleme işlemi başarılı.");
            return Hata("Yükleme işlemi başarısız:" + s.hataStr);
        }
    }
}
