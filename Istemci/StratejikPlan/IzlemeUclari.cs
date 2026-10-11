using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using PerformansButce;
using TNS;
using TNS.BKD;
using TNS.KYM;
using TNS.PEB;

namespace Istemci.StratejikPlan
{
    public class GerceklesmeSatiri
    {
        public int GostergeKod { get; set; }
        /// <summary>Dönem sonundaki (birikimli) gerçekleşme.</summary>
        public double BirikimliGerceklesme { get; set; }
        public string Aciklama { get; set; }
        // Yıl sonu değerlendirmesinde girilen ölçütler
        public string Ilgililik { get; set; }
        public string Etkililik { get; set; }
        public string Etkinlik { get; set; }
        public string Surdurulebilirlik { get; set; }
    }

    public class GerceklesmeIstegi
    {
        public string KurumKod { get; set; }
        /// <summary>Stratejik planın yılı (plan sürümü).</summary>
        public int DonemBYil { get; set; }
        public int Hedef { get; set; }
        /// <summary>Gerçekleşmenin yılı ve dönemi (3: altı aylık izleme, 4: yıl sonu değerlendirme).</summary>
        public int Yil { get; set; }
        public int Donem { get; set; }
        public GerceklesmeSatiri[] Satirlar { get; set; }
        public string Degerlendirmeler { get; set; }
        public string SapmaNedeni { get; set; }
        public string AlinacakOnlem { get; set; }
    }

    /// <summary>
    /// İzleme Tablosu sayfasının (eski PerformansButce/StratejikGostGerc.aspx) JavaScript'ten çağırdığı uçlar:
    ///   GET  /api/strizleme/tanim, /hedefler?kurum=&amp;donemBYil=, /gerceklesme?kurum=&amp;donemBYil=&amp;hedef=&amp;yil=&amp;donem=
    ///   POST /api/strizleme/kaydet
    ///   GET  /api/strizleme/rapor?tur=&amp;... (hedef izleme / değerlendirme raporları, Word),
    ///        /rapor-hedef, /rapor-gosterge?kurum=&amp;yil=&amp;donem= (kurum hedef ve gösterge başarımı, Excel)
    /// Raporlar eski kodla üretilir (PerformansButce.HedefIzlemeRaporlari, PerformansButce.SPIzlemeKurum).
    /// Sayfayı menüde İzleme Tablosu seçeneğini görebilenler kullanır.
    /// </summary>
    public static class IzlemeUclari
    {
        private const string DonemDegiskeni = "StrPlan";

        public static void Ekle(WebApplication app)
        {
            app.MapGet("/api/strizleme/tanim", Tanim);
            app.MapGet("/api/strizleme/hedefler", Hedefler);
            app.MapGet("/api/strizleme/gerceklesme", Gerceklesme);
            app.MapPost("/api/strizleme/kaydet", Kaydet);
            app.MapGet("/api/strizleme/rapor", Rapor);
            app.MapGet("/api/strizleme/rapor-hedef", RaporHedef);
            app.MapGet("/api/strizleme/rapor-gosterge", RaporGosterge);
        }

        /// <summary>Kullanıcı İzleme Tablosu sayfasını kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), "../PerformansButce/StratejikGostGerc.aspx");
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

        private static int HazirlikYili()
        {
            return OrtakFonksiyonlar.ConvertToInt(TNS.DEG.Arac.DegiskenDegerBulCache(0, "/BUTCEHAZIRLIK"), DateTime.Now.Year + 1);
        }

        /// <summary>Seçilen kurum ve plan, Stratejik Plan sayfasıyla ortak olan kullanıcı değişkenine yazılır (eski DonemSakla).</summary>
        private static void DonemSakla(Kullanici kullanan, string kurumKod, int donemBYil)
        {
            if (string.IsNullOrWhiteSpace(kurumKod) || donemBYil <= 0) return;
            TNSCollection degerler = new TNSCollection();
            degerler.Add(kurumKod + "-" + donemBYil);
            kullanan.DegiskenleriSakla(TNS.KYM.Arac.Tanimla(), DonemDegiskeni, degerler);
        }

        // ------------------------------------------------------------------ okuma

        /// <summary>Arama bölmesinin seçenekleri ve varsayılanları (eski Page_Load).</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;

            // Stratejik Plan sayfasında seçilmiş kurum ve plan ("kurumKod-yıl")
            string kurumKod = "", yilStr = "";
            try
            {
                TNSCollection degerler = TNS.KYM.Arac.Tanimla().DegiskenleriListele(kullanan.kullaniciKodu, DonemDegiskeni);
                if (degerler != null && degerler.Count > 0)
                    OrtakFonksiyonlar.StringiIkiyeBol(Convert.ToString(degerler[0]) ?? "", "-", ref kurumKod, ref yilStr);
            }
            catch (Exception)
            {
                kurumKod = "";
            }
            int hyil = HazirlikYili();
            int yil = OrtakFonksiyonlar.ConvertToInt(yilStr, -1);

            List<object> kurumlar = new List<object>();
            ABSKodlarVAN liste = TNS.BKD.Arac.Tanimla().KurumListesiKullanici(kullanan, yil > 0 ? yil : hyil);
            string ilkKurum = "";
            if (liste != null && liste.absKodlar != null)
                foreach (ABSKod k in liste.absKodlar)
                {
                    if (ilkKurum == "") ilkKurum = k.kod;
                    kurumlar.Add(new { kod = k.kod, ad = k.kodAd });
                }
            if (string.IsNullOrWhiteSpace(kurumKod)) kurumKod = ilkKurum;

            return Results.Json(new
            {
                kurumlar = kurumlar,
                kurumKod = kurumKod,
                donemBYil = yil > 0 ? yil : 0,
                sorguYil = hyil,
                donemler = new object[]
                {
                    new { kod = (int)ENUMDonemler.AltiAy, ad = TNS.PEB.Arac.DonemStr(ENUMDonemler.AltiAy, false) },
                    new { kod = (int)ENUMDonemler.Yil, ad = TNS.PEB.Arac.DonemStr(ENUMDonemler.Yil, false) }
                },
                // Yılın ikinci yarısında yıl sonu değerlendirmesi, ilk yarısında altı aylık izleme seçili gelir.
                donem = DateTime.Now.Month > 7 ? (int)ENUMDonemler.Yil : (int)ENUMDonemler.AltiAy,
                raporV1Gizli = TNS.DEG.Arac.DegiskenDegerBul(0, "TBMM") == "1"
            });
        }

        /// <summary>Kurumun seçilen planındaki hedefler.</summary>
        private static IResult Hedefler(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int donemBYil)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kurum) || donemBYil <= 0) return Results.Json(new { satirlar = new object[0] });

            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = donemBYil;
            hedef.kurumKod = kurum.Trim();
            ObjectArray bilgiler = TNS.PEB.Arac.Tanimla().StratejikHedefListele(kullanan, hedef, true);
            if (!bilgiler.sonuc.islemSonuc) return Results.Json(new { hata = "Hedefler okunamadı." });

            List<object> satirlar = new List<object>();
            foreach (TNS.PEB.StratejikHedef item in bilgiler.objeler)
                satirlar.Add(new { kod = item.strHedefKod, kisa = item.strHedefKisa, ad = item.strHedef });
            if (satirlar.Count > 0) DonemSakla(kullanan, hedef.kurumKod, donemBYil);
            return Results.Json(new { satirlar = satirlar });
        }

        private static TNS.PEB.StratejikHedef HedefKriteri(string kurum, int donemBYil, int hedefKod)
        {
            TNS.PEB.StratejikHedef hedef = new TNS.PEB.StratejikHedef();
            hedef.donemBYil = donemBYil;
            hedef.strHedefKod = hedefKod;
            hedef.dil.kod = 1;
            hedef.kurumKod = (kurum ?? "").Trim();
            return hedef;
        }

        private static string DiziElemanAl(ObjectArray oa, int indis)
        {
            return HedefIzlemeRaporlari.DiziElemanAl(oa, indis, "");
        }

        /// <summary>Hedefin göstergelerinin verilen yıl ve dönemdeki gerçekleşmeleri ile hedefin değerlendirme metinleri.</summary>
        private static IResult Gerceklesme(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int donemBYil, int hedef, int yil, int donem)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kurum) || donemBYil <= 0 || hedef <= 0) return Results.Json(new { hata = "Hedef seçiniz." });

            IPEBServis servisPEB = TNS.PEB.Arac.Tanimla();
            TNS.PEB.StratejikHedef kriter = HedefKriteri(kurum, donemBYil, hedef);
            ObjectArray oa1 = servisPEB.ListeleStrHedefGostergeGerc(kullanan, kriter, yil, donem);
            if (!oa1.sonuc.islemSonuc) return Results.Json(new { hata = "Gerçekleşmeler okunamadı." });

            List<object> satirlar = new List<object>();
            foreach (StrHedefGosterge g in oa1.objeler)
                satirlar.Add(new
                {
                    gostergeKod = g.gostergeKod,
                    gostergeKodAd = g.gostergeKodAd,
                    donemBaslangic = g.donemBaslangic,
                    oncYilGerceklesme = g.oncYilGerceklesme,
                    hedeflenenBirikimliDeger = g.hedeflenenBirikimliDeger,
                    birikimliGerceklesme = g.birikimliGerceklesme,
                    birikimliPerformans = g.birikimliPerformans,
                    // Performans yüzdesi tarayıcıda da aynı kuralla hesaplanır (StrHedefGosterge.Oranla).
                    oranABCB = g.gostergeTur == ENUMGostergeTur.OranABCB,
                    aciklama = g.aciklama ?? "",
                    ilgililik = g.ilgililik ?? "",
                    etkililik = g.etkililik ?? "",
                    etkinlik = g.etkinlik ?? "",
                    surdurulebilirlik = g.surdurulebilirlik ?? ""
                });

            string sapma = "", onlem = "", degerlendirme = "";
            ObjectArray oa2 = servisPEB.ListeleStrHedefGercBilgi(kullanan, kriter, yil, donem, 1);
            if (oa2.sonuc.islemSonuc)
            {
                sapma = DiziElemanAl(oa2, 0);
                onlem = DiziElemanAl(oa2, 1);
                degerlendirme = DiziElemanAl(oa2, 2);
            }
            return Results.Json(new { satirlar = satirlar, sapmaNedeni = sapma, alinacakOnlem = onlem, degerlendirmeler = degerlendirme });
        }

        // ------------------------------------------------------------------ yazma

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, GerceklesmeIstegi istek, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || string.IsNullOrWhiteSpace(istek.KurumKod) || istek.DonemBYil <= 0 || istek.Hedef <= 0)
                return Hata("Önce hedef seçiniz.");

            // Eski sayfa tablodaki satırları (göstergenin bütün alanlarıyla) geri gönderiyordu. Burada satırlar sunucuda yeniden
            // okunur, kullanıcının değiştirebildiği alanlar üzerine yazılır ve öyle kaydedilir.
            IPEBServis servisPEB = TNS.PEB.Arac.Tanimla();
            ObjectArray kayitli = servisPEB.ListeleStrHedefGostergeGerc(kullanan, HedefKriteri(istek.KurumKod, istek.DonemBYil, istek.Hedef), istek.Yil, istek.Donem);
            if (!kayitli.sonuc.islemSonuc) return Hata("Gerçekleşmeler okunamadı.");

            Dictionary<int, GerceklesmeSatiri> gelenler = new Dictionary<int, GerceklesmeSatiri>();
            foreach (GerceklesmeSatiri s in istek.Satirlar ?? new GerceklesmeSatiri[0])
                if (s != null) gelenler[s.GostergeKod] = s;

            TNSCollection gerceklesmeler = new TNSCollection();
            foreach (StrHedefGosterge g in kayitli.objeler)
            {
                if (gelenler.TryGetValue(g.gostergeKod, out GerceklesmeSatiri s))
                {
                    g.birikimliGerceklesme = s.BirikimliGerceklesme;
                    g.aciklama = (s.Aciklama ?? "").Trim();
                    g.ilgililik = (s.Ilgililik ?? "").Trim();
                    g.etkililik = (s.Etkililik ?? "").Trim();
                    g.etkinlik = (s.Etkinlik ?? "").Trim();
                    g.surdurulebilirlik = (s.Surdurulebilirlik ?? "").Trim();
                }
                gerceklesmeler.Add(g);
            }
            if (gerceklesmeler.Count == 0) return Hata("Kaydedilecek gösterge yok.");

            TNSCollection[] gercBilgiler = new TNSCollection[1];
            gercBilgiler[0] = new TNSCollection();
            gercBilgiler[0].Add(new IntKodStrAd(1, (istek.SapmaNedeni ?? "").Trim()));
            gercBilgiler[0].Add(new IntKodStrAd(2, (istek.AlinacakOnlem ?? "").Trim()));
            gercBilgiler[0].Add(new IntKodStrAd(3, (istek.Degerlendirmeler ?? "").Trim()));

            Sonuc sonuc = servisPEB.KaydetStrHedefGostergeGerc(kullanan, gerceklesmeler, gercBilgiler, false);
            if (!sonuc.islemSonuc) return Hata(HataMetni(sonuc, "Kayıt yapılamadı."));
            DonemSakla(kullanan, istek.KurumKod.Trim(), istek.DonemBYil);
            return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi tamamlandı." });
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        private static string RaporSablonYol()
        {
            return Ayar.Oku("RaporSablonYol");
        }

        /// <summary>
        /// Hedef izleme / değerlendirme raporu (Word; eski btnYazdir_Click). tur: 1 Yazdır, 2 Yazdır (v2), 12 Rapor, 11 Rapor (v1).
        /// Şablon türe ve döneme göre seçilir: StrHedef[Rapor|v2][Izleme|Degerlendirme].dotx.
        /// </summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, string tur, string kurum, int donemBYil, int? hedef, int yil, int donem)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (tur != "1" && tur != "2" && tur != "11" && tur != "12") return Results.BadRequest();
            if (string.IsNullOrWhiteSpace(kurum) || donemBYil <= 0) return Results.Json(new { hata = "Kurum ve plan dönemi seçiniz." });
            return RaporUret(kullanan, tur, kurum.Trim(), donemBYil, hedef ?? 0, yil, donem);
        }

        private static IResult RaporUret(Kullanici kullanan, string tur, string kurum, int donemBYil, int hedef, int yil, int donem)
        {
            Hashtable raporParam = new Hashtable();
            raporParam["donemBYil"] = donemBYil.ToString();
            raporParam["hedefKod"] = hedef.ToString();
            raporParam["kurumKod"] = kurum;
            raporParam["yil"] = yil.ToString();
            raporParam["donem"] = donem.ToString();

            HedefIzlemeRaporlari r = new HedefIzlemeRaporlari();
            string sonucDosyaAd = DosyaIslem.DosyaAdUret("StrHedefGerc_", "");
            string uzanti = "docx";
            try
            {
                string sablonAdEk = "";
                if (tur == "11" || tur == "12")
                    sablonAdEk = "Rapor";
                else if (tur == "2")
                    sablonAdEk = "v2";
                sablonAdEk += (donem == 4 ? "Degerlendirme" : "Izleme");

                AsposeLisans.Yukle();
                sonucDosyaAd = r.RaporAl(kullanan, TNS.PEB.Arac.Tanimla(), Path.Combine(RaporSablonYol(), "PEB", "StrHedef" + sablonAdEk + ".dotx"), sonucDosyaAd, uzanti, raporParam, tur);
                return DosyaIslem.DosyaGonder(sonucDosyaAd, Path.GetFileName(sonucDosyaAd), true, uzanti);
            }
            catch (Exception ex)
            {
                // Eski sayfa da rapor yordamının iletisini gösteriyordu (ör. "Lütfen hedef seçiniz!").
                return Results.Json(new { hata = KisaIleti(ex, "Rapor hazırlanamadı.") });
            }
        }

        /// <summary>Kurumun hedef başarımları (Excel; eski btnYazdirHedef_Click, şablon SPKurumIzleme.xltx).</summary>
        private static IResult RaporHedef(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int yil, int donem)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kurum)) return Results.Json(new { hata = "Kurum seçiniz." });
            return KurumRaporuUret(kullanan, kurum.Trim(), yil, donem, false);
        }

        /// <summary>Kurumun gösterge başarımları (Excel; eski btnYazdirGost_Click, şablon SPKurumIzlemeGost.xltx).</summary>
        private static IResult RaporGosterge(HttpContext baglam, IWebHostEnvironment ortam, string kurum, int yil, int donem)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (string.IsNullOrWhiteSpace(kurum)) return Results.Json(new { hata = "Kurum seçiniz." });
            return KurumRaporuUret(kullanan, kurum.Trim(), yil, donem, true);
        }

        private static IResult KurumRaporuUret(Kullanici kullanan, string kurum, int yil, int donem, bool gosterge)
        {
            try
            {
                AsposeLisans.Yukle();
                IPEBServis servisPEB = TNS.PEB.Arac.Tanimla();
                string sonucDosya = gosterge
                    ? SPIzlemeKurum.YazdirHedefGost(kullanan, servisPEB, yil, kurum, donem, -1, -1, RaporSablonYol())
                    : SPIzlemeKurum.YazdirHedef(kullanan, servisPEB, yil, kurum, donem, -1, -1, RaporSablonYol());
                return DosyaIslem.DosyaGonder(sonucDosya, Path.GetFileName(sonucDosya), true, "xlsx");
            }
            catch (Exception ex)
            {
                return Results.Json(new { hata = KisaIleti(ex, "Rapor hazırlanamadı.") });
            }
        }

        // ------------------------------------------------------------------ yardımcılar

        private static IResult Hata(string metin)
        {
            return Results.Json(new { basarili = false, hata = metin });
        }

        private static string HataMetni(Sonuc sonuc, string varsayilan)
        {
            string hata = sonuc.hataStr ?? "";
            return hata.Trim() == "" || hata.Contains("Exception") || hata.Contains("   at ") ? varsayilan : hata;
        }

        /// <summary>Rapor kodunun kendi attığı kısa Türkçe ileti gösterilir; iç hata ayrıntısı gösterilmez.</summary>
        private static string KisaIleti(Exception ex, string varsayilan)
        {
            string ileti = ex.Message ?? "";
            bool kisa = ex.GetType() == typeof(Exception) && ileti.Length > 0 && ileti.Length < 160 && !ileti.Contains("   at ");
            return kisa ? ileti : varsayilan;
        }
    }
}
