using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Istemci.Organizasyon;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.BHF;
using TNS.KYM;

namespace Istemci.ButceForm
{
    public class KiralikSozlesmeIstegi
    {
        public int yil { get; set; }
        public int sozlesmeKod { get; set; }
        public int saymanlikNo { get; set; }
        public string birimKod { get; set; }
        public string imzaTarihi { get; set; }
        public string sozlesmeNo { get; set; }
        public string isBaslangicTarihi { get; set; }
        public string isBitisTarihi { get; set; }
        public int firmaKod { get; set; }
        public bool ilcelerDahil { get; set; }
        public int surecYil { get; set; }
        public string surecNo { get; set; }
    }

    public class KiralikSozlesmeAnahtari
    {
        public int yil { get; set; }
        public int sozlesmeKod { get; set; }
    }

    public class KiralikAracIstegi
    {
        public int yil { get; set; }
        public int sozlesmeKod { get; set; }
        public int aracSiraNo { get; set; }
        public int tasitCinsi { get; set; }
        public int enBuyukYas { get; set; }
        public int koltukSayisi { get; set; }
        public int yakitTuru { get; set; }
        public bool soforDahil { get; set; }
        public bool yakitDahil { get; set; }
        public int adet { get; set; }
        public int sure { get; set; }
        public int sureBirimi { get; set; }
        public double birimFiyat { get; set; }
        public bool dsMi { get; set; }
    }

    /// <summary>
    /// Kiralık Araç Bilgi Formu (eski ButceForm/KiralikAraclarForm.aspx) uçları:
    ///   GET  /api/kiralikarac/tanim                         açılış: yıl, taşıt tipleri, yakıt türleri, süre birimleri, kullanıcının muhasebe / birimi
    ///   GET  /api/kiralikarac/cinsler?tip=                  taşıt tipinin cinsleri (eski TasitCinsForm.CinsleriDoldur)
    ///   GET  /api/kiralikarac/birim?muhasebe=&amp;birim=        birim adı ve "ilçeler dahil" sorulur mu (eski IlceAyarla: taşra il birimi)
    ///   GET  /api/kiralikarac/liste?yil=&amp;muhasebe=&amp;birim=   sözleşme listesi (eski Listele)
    ///   GET  /api/kiralikarac/oku?yil=&amp;sozlesmeKod=          sözleşme ve araçları (eski OkuSozlesme)
    ///   POST /api/kiralikarac/kaydet, /sil                  sözleşme (eski btnKaydet_Click, btnSozlesmeSil_Click)
    ///   GET  /api/kiralikarac/arac/oku?yil=&amp;sozlesmeKod=&amp;aracSiraNo=, POST arac/kaydet, arac/sil   araç penceresi
    ///   GET  /api/kiralikarac/firma/ara?muhasebe=&amp;ad=&amp;vergiNo=   kişi / firma arama (eski ListeFirmaKisi penceresi; kod da döner)
    ///   GET  /api/kiralikarac/rapor?yil=&amp;muhasebe=&amp;birim=&amp;tur=   1 Yazdır (araç listesi), 2 Tablo Yazdır (eski kodla, Excel)
    /// İş kuralı çağrıları eskisi gibi TNS.BHF.Arac.Tanimla() üzerinden yapılır.
    /// </summary>
    public static class KiralikAracUclari
    {
        private const string SayfaAdresi = "../ButceForm/KiralikAraclarForm.aspx";
        private const string U = "/api/kiralikarac/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(U + "tanim", Tanim);
            app.MapGet(U + "cinsler", Cinsler);
            app.MapGet(U + "birim", Birim);
            app.MapGet(U + "liste", Liste);
            app.MapGet(U + "oku", Oku);
            app.MapPost(U + "kaydet", Kaydet);
            app.MapPost(U + "sil", Sil);
            app.MapGet(U + "arac/oku", AracOku);
            app.MapPost(U + "arac/kaydet", AracKaydet);
            app.MapPost(U + "arac/sil", AracSil);
            app.MapGet(U + "firma/ara", FirmaAra);
            app.MapGet(U + "rapor", Rapor);
        }

        /// <summary>Kullanıcı bu sayfayı kullanabilir mi? (Sayfa modeli de bunu kullanır.)</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru)
        {
            if (kullanan == null) return false;
            if (GenelGorevUclari.YetkiVar(kullanan)) return true;
            try
            {
                return new MenuOkuyucu(kullanan).SayfaYetkili(MenuOkuyucu.DosyaYolu(uygulamaKlasoru), SayfaAdresi);
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

        private static async Task<IResult> DenetleYaz(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, Func<Kullanici, IResult> is_)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            return is_(kullanan);
        }

        private static IResult Hata(string hata)
        {
            return Results.Json(new { hata = hata });
        }

        private static IResult Basarisiz(string hata)
        {
            return Results.Json(new { basarili = false, hata = hata });
        }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        /// <summary>Tarih kutusu için yyyy-MM-dd.</summary>
        private static string TarihIso(TNSDateTime t)
        {
            if (TNSDateTime.IsNull(t)) return "";
            DateTime d = t.Oku();
            if (d == DateTime.MinValue) return "";
            return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static string TarihGoster(DateTime d)
        {
            if (d == DateTime.MinValue || d.Year < 1900) return "";
            return d.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        private static TNSDateTime TarihOku(string s)
        {
            s = Temiz(s);
            if (s == "") return new TNSDateTime();
            string[] bicimler = { "yyyy-MM-dd", "dd.MM.yyyy", "d.M.yyyy" };
            if (DateTime.TryParseExact(s, bicimler, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime t))
                return new TNSDateTime(t);
            return new TNSDateTime();
        }

        private static List<object> KodAdListe(IEnumerable liste)
        {
            List<object> l = new List<object>();
            if (liste == null) return l;
            foreach (IntKodStrAd k in liste)
                l.Add(new { kod = k.kod, ad = k.ad });
            return l;
        }

        /// <summary>Eski MuhBirimAdBul: saymanlık ve birim adı (bulunamazsa uyarı metni).</summary>
        private static string MuhBirimAdBul(int saymanlikNo, string birimKod)
        {
            if (saymanlikNo <= 0) return "";
            string birimAd = "";
            string sayAd = TNS.UZY.Arac.Tanimla().UzayDegeriStr(new Kullanici(), "MUHMUHASEBE", saymanlikNo.ToString(), true, "MUHASEBEAD");
            if (string.IsNullOrWhiteSpace(sayAd)) sayAd = "Saymanlık Bulunamadı!";
            else if (!string.IsNullOrWhiteSpace(birimKod))
            {
                TNS.MUH.MuhasebeBirim kriter = new TNS.MUH.MuhasebeBirim();
                kriter.muhasebe = saymanlikNo;
                kriter.birimKod = birimKod.Trim();
                kriter.birimAd = "";
                kriter.kurum = "";
                ObjectArray oa = TNS.MUH.Arac.Tanimla().MuhasebeBirimListele(kriter);
                if (oa != null && oa.sonuc.islemSonuc && oa.ObjeSayisi >= 1)
                    birimAd = ((TNS.MUH.MuhasebeBirim)oa.objeler[0]).birimAd;
                if (string.IsNullOrWhiteSpace(birimAd)) birimAd = "Birim Bulunamadı!";
            }
            return TNS.MUH.Arac.BirimAdOl(sayAd, birimAd);
        }

        /// <summary>Eski IlceAyarla: "İlçeler dahil" yalnızca taşra il birimlerinde sorulur.</summary>
        private static bool IlceSorulur(int saymanlik, string birim)
        {
            try
            {
                return TNS.BHMv2.Arac.BirimTurBul(saymanlik, Temiz(birim)) == TNS.BHMv2.ENUMBirimTur.Tasra_Il;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Eski hdnKurumsalYaz: kurumsal kod noktalı yazılır.</summary>
        private static string KurumsalYaz(int yil, string kurum)
        {
            kurum = (kurum ?? "").Replace(".", "").Trim();
            if (kurum == "") return "";
            return OrtakFonksiyonlar.KodNoktaKoy(TNS.BKD.TertipYapi.KodYapiBul(yil, 1, false), kurum);
        }

        // ------------------------------------------------------------------ açılış

        /// <summary>Eski Page_Load: taşıt tipleri, yakıt türleri, süre birimleri, bu yıl ve kullanıcı değişkenlerindeki muhasebe / birim.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int yil = DateTime.Now.Year;
            try
            {
                List<object> tipler = new List<object>();
                TasitTip kriter = new TasitTip();
                kriter.ad = "";
                ObjectArray oa = TNS.BHF.Arac.Tanimla().ListeleTasitTip(kullanan, kriter);
                if (oa != null && oa.sonuc.islemSonuc)
                    foreach (TasitTip t in oa.objeler)
                        tipler.Add(new { kod = t.kod, ad = t.ad ?? "" });

                string muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEBIRIM");
                string kurumsal = KurumsalYaz(yil, GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEKURUMSAL"));

                return Results.Json(new
                {
                    yil = yil,
                    tipler = tipler,
                    yakitTurleri = KodAdListe(TasitBilgi.YakitTurleri(false)),
                    sureBirimleri = KodAdListe((IEnumerable)KiralikArac.SureBirimleri(false)),
                    muhasebe = muhasebe,
                    birim = birim,
                    kurumsal = kurumsal,
                    birimAd = MuhBirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim)
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Eski cmbTipi_Selected / CinsleriDoldur.</summary>
        private static IResult Cinsler(HttpContext baglam, IWebHostEnvironment ortam, int tip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            List<object> l = new List<object>();
            if (tip <= 0) return Results.Json(new { cinsler = l });
            try
            {
                TasitCins kriter = new TasitCins();
                kriter.ad = "";
                kriter.tipKodu = tip;
                ObjectArray oa = TNS.BHF.Arac.Tanimla().ListeleTasitCins(kullanan, kriter);
                if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc?.hataStr, "Taşıt cinsleri okunamadı."));
                foreach (TasitCins c in oa.objeler)
                    l.Add(new { kod = c.kod, ad = c.ad ?? "" });
                return Results.Json(new { cinsler = l });
            }
            catch (Exception)
            {
                return Hata("Taşıt cinsleri okunamadı.");
            }
        }

        /// <summary>Yeni sözleşmede birim adı ve "İlçeler dahil" alanının görünürlüğü (eski btnYeni_Click, IlceAyarla).</summary>
        private static IResult Birim(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int m = muhasebe ?? 0;
            try
            {
                return Results.Json(new { birimAd = MuhBirimAdBul(m, birim), ilceSorulur = IlceSorulur(m, birim) });
            }
            catch (Exception)
            {
                return Hata("Birim bilgisi okunamadı.");
            }
        }

        // ------------------------------------------------------------------ sözleşmeler

        /// <summary>Eski Listele: seçilen yıl ve muhasebe / birimin sözleşmeleri.</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            KiralikAracSozlesmesi form = new KiralikAracSozlesmesi();
            form.yil = yil;
            form.saymanlikNo = muhasebe ?? 0;
            form.birimKod = Temiz(birim);
            ObjectArray oa;
            try
            {
                oa = TNS.BHF.Arac.Tanimla().ListeleKiralikAracSozlesme(kullanan, form);
            }
            catch (Exception)
            {
                return Hata("Sözleşmeler okunamadı.");
            }
            if (oa == null || !oa.sonuc.islemSonuc) return Hata(HataMetni(oa?.sonuc?.hataStr, "Sözleşmeler okunamadı."));
            List<object> satirlar = new List<object>();
            foreach (KiralikAracSozlesmesi s in oa.objeler)
            {
                double tutar = 0;
                try { tutar = s.sozlesmeBedeliTop; } catch (Exception) { }
                satirlar.Add(new
                {
                    yil = s.yil,
                    sozlesmeKod = s.sozlesmeKod,
                    birimAd = s.birimAd ?? "",
                    imzaTarihi = TarihGoster(s.imzaTarihiDT),
                    firmaAd = s.firmaAd ?? "",
                    sozlesmeBedeliTop = tutar
                });
            }
            return Results.Json(new { satirlar = satirlar });
        }

        private static object AracJson(KiralikArac a)
        {
            return new
            {
                aracSiraNo = a.aracSiraNo,
                tasitTipKodu = a.tasitTipKodu,
                tasitCinsi = a.tasitCinsi,
                tasitCinsiAdTKOD = a.tasitCinsiAdTKOD ?? "",
                enBuyukYas = a.enBuyukYas,
                koltukSayisi = a.koltukSayisi,
                yakitTuru = (int)a.yakitTuru,
                yakitTuruAd = a.yakitTuruAd ?? "",
                soforDahil = a.soforDahil == 1,
                soforDahilAd = a.soforDahilAd,
                yakitDahil = a.yakitDahil == 1,
                yakitDahilAd = a.yakitDahilAd,
                adet = a.adet,
                sure = a.sure,
                sureBirimi = (int)a.sureBirimi,
                sureAd = a.sureAd ?? "",
                birimFiyat = a.birimFiyat,
                Tutar = a.Tutar,
                dsMi = a.dsMi,
                FaaliyetAd = a.FaaliyetAd
            };
        }

        /// <summary>Eski OkuSozlesme: sözleşme bilgileri, araçları ve bedelleri.</summary>
        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int sozlesmeKod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            KiralikAracSozlesmesi form = new KiralikAracSozlesmesi();
            form.yil = yil;
            form.sozlesmeKod = sozlesmeKod;
            ObjectVAN ov;
            try
            {
                ov = TNS.BHF.Arac.Tanimla().OkuKiralikAracSozlesme(kullanan, form);
            }
            catch (Exception)
            {
                return Hata("Sözleşme okunamadı.");
            }
            if (ov == null || !ov.sonuc.islemSonuc) return Hata(HataMetni(ov?.sonuc?.hataStr, "Sözleşme okunamadı."));
            if (ov.o == null) return Hata("Kayıt bulunamadı.");
            form = (KiralikAracSozlesmesi)ov.o;

            List<object> araclar = new List<object>();
            if (form.kiralikArac != null)
                foreach (KiralikArac a in form.kiralikArac)
                    araclar.Add(AracJson(a));
            double butce = 0, doner = 0;
            if (form.kiralikArac != null)
            {
                butce = form.sozlesmeBedeliButce;
                doner = form.sozlesmeBedeliDoner;
            }
            return Results.Json(new
            {
                yil = form.yil,
                sozlesmeKod = form.sozlesmeKod,
                saymanlikNo = form.saymanlikNo,
                birimKod = form.birimKod ?? "",
                birimAd = form.birimAd ?? "",
                kodYazi = form.yil.ToString("0000") + "-" + form.sozlesmeKod.ToString("000000"),
                imzaTarihi = TarihIso(form.imzaTarihi),
                sozlesmeNo = form.sozlesmeNo ?? "",
                isBaslangicTarihi = TarihIso(form.isBaslangicTarihi),
                isBitisTarihi = TarihIso(form.isBitisTarihi),
                firmaKod = form.firmaKod,
                firmaAd = form.firmaAd ?? "",
                sozlesmeBedeliButce = butce,
                sozlesmeBedeliDoner = doner,
                sozlesmeBedeliTop = butce + doner,
                ilceSorulur = IlceSorulur(form.saymanlikNo, form.birimKod),
                ilcelerDahil = form.ilcelerDahil == 1,
                surecYil = form.surecYil,
                surecNo = form.surecNo ?? "",
                ToplamTutarBul = form.ToplamTutarBul,
                kiralikArac = araclar
            });
        }

        /// <summary>Eski Kaydet: sözleşme bilgileri yazılır (araçlar araç penceresinden ayrı kaydedilir); dönen anahtar "yıl-kod".</summary>
        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, KiralikSozlesmeIstegi istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null) return Basarisiz("Kaydedilecek bilgi yok.");
                if (istek.saymanlikNo <= 0 || Temiz(istek.birimKod) == "") return Basarisiz("Lütfen sol taraftan saymanlık birim seçiniz!");
                KiralikAracSozlesmesi form = new KiralikAracSozlesmesi();
                form.yil = istek.yil;
                form.sozlesmeKod = istek.sozlesmeKod;
                form.saymanlikNo = istek.saymanlikNo;
                form.birimKod = Temiz(istek.birimKod);
                form.imzaTarihi = TarihOku(istek.imzaTarihi);
                form.sozlesmeNo = Temiz(istek.sozlesmeNo);
                form.isBaslangicTarihi = TarihOku(istek.isBaslangicTarihi);
                form.isBitisTarihi = TarihOku(istek.isBitisTarihi);
                form.firmaKod = istek.firmaKod;
                form.ilcelerDahil = istek.ilcelerDahil && IlceSorulur(form.saymanlikNo, form.birimKod) ? 1 : 0;
                form.surecYil = istek.surecYil;
                form.surecNo = Temiz(istek.surecNo);
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHF.Arac.Tanimla().KaydetKiralikAracSozlesme(kullanan, form);
                }
                catch (Exception)
                {
                    return Basarisiz("Kayıt işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız."));
                string p1 = "", p2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(sonuc.anahtar ?? "", "-", ref p1, ref p2);
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Kayıt işlemi başarıyla gerçekleşti.",
                    yil = OrtakFonksiyonlar.ConvertToInt(p1, 0),
                    sozlesmeKod = OrtakFonksiyonlar.ConvertToInt(p2, 0)
                });
            });
        }

        /// <summary>Eski btnSozlesmeSil_Click.</summary>
        private static Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, KiralikSozlesmeAnahtari istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.yil <= 0 || istek.sozlesmeKod <= 0) return Basarisiz("Lütfen sözleşme seçiniz!");
                KiralikAracSozlesmesi form = new KiralikAracSozlesmesi();
                form.yil = istek.yil;
                form.sozlesmeKod = istek.sozlesmeKod;
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHF.Arac.Tanimla().SilKiralikAracSozlesme(kullanan, form);
                }
                catch (Exception)
                {
                    return Basarisiz("Silme işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Silme işlemi başarısız."));
                return Results.Json(new { basarili = true, bilgi = "Sözleşme silindi." });
            });
        }

        // ------------------------------------------------------------------ araçlar (eski wndAracGiris)

        /// <summary>Eski OkuArac.</summary>
        private static IResult AracOku(HttpContext baglam, IWebHostEnvironment ortam, int yil, int sozlesmeKod, int aracSiraNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (yil <= 0 || sozlesmeKod <= 0 || aracSiraNo <= 0) return Hata("Lütfen araç seçiniz!");
            ObjectVAN ov;
            try
            {
                ov = TNS.BHF.Arac.Tanimla().OkuKiralikArac(kullanan, yil, sozlesmeKod, aracSiraNo);
            }
            catch (Exception)
            {
                return Hata("Araç okunamadı.");
            }
            if (ov == null || !ov.sonuc.islemSonuc) return Hata(HataMetni(ov?.sonuc?.hataStr, "Araç okunamadı."));
            if (ov.o == null) return Hata("Kayıt bulunamadı.");
            return Results.Json(AracJson((KiralikArac)ov.o));
        }

        /// <summary>Eski btnAracKaydet_Click / AracFormOku: kaynak döner sermaye ise PEB kodu boş, bütçe ise 23000000.</summary>
        private static Task<IResult> AracKaydet(HttpContext baglam, IWebHostEnvironment ortam, KiralikAracIstegi istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.yil <= 0 || istek.sozlesmeKod <= 0) return Basarisiz("Önce sözleşmeyi kaydediniz.");
                KiralikArac ka = new KiralikArac();
                ka.aracSiraNo = istek.aracSiraNo;
                ka.tasitCinsi = istek.tasitCinsi;
                ka.enBuyukYas = istek.enBuyukYas;
                ka.koltukSayisi = istek.koltukSayisi;
                ka.yakitTuru = (ENUMYakitTuru)istek.yakitTuru;
                ka.soforDahil = istek.soforDahil ? 1 : 0;
                ka.yakitDahil = istek.yakitDahil ? 1 : 0;
                ka.adet = istek.adet;
                ka.sure = istek.sure;
                ka.sureBirimi = (ENUMSureBirim)istek.sureBirimi;
                ka.birimFiyat = istek.birimFiyat;
                ka.dsMi = istek.dsMi;
                ka.pebKod = istek.dsMi ? null : "23000000";
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHF.Arac.Tanimla().KaydetKiralikArac(kullanan, istek.yil, istek.sozlesmeKod, ka);
                }
                catch (Exception)
                {
                    return Basarisiz("Kayıt işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Kayıt işlemi başarısız."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla gerçekleşti.", aracSiraNo = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, 0) });
            });
        }

        /// <summary>Eski btnAracSil_Click.</summary>
        private static Task<IResult> AracSil(HttpContext baglam, IWebHostEnvironment ortam, KiralikAracIstegi istek, IAntiforgery sahtecilik)
        {
            return DenetleYaz(baglam, ortam, sahtecilik, kullanan =>
            {
                if (istek == null || istek.yil <= 0 || istek.sozlesmeKod <= 0 || istek.aracSiraNo <= 0) return Basarisiz("Lütfen araç seçiniz!");
                Sonuc sonuc;
                try
                {
                    sonuc = TNS.BHF.Arac.Tanimla().SilKiralikArac(kullanan, istek.yil, istek.sozlesmeKod, istek.aracSiraNo);
                }
                catch (Exception)
                {
                    return Basarisiz("Silme işlemi başarısız.");
                }
                if (!sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc.hataStr, "Silme işlemi başarısız."));
                return Results.Json(new { basarili = true, bilgi = "Araç silme işlemi tamamlanmıştır!" });
            });
        }

        // ------------------------------------------------------------------ kişi / firma (eski ListeFirmaKisi penceresi)

        private static IResult FirmaAra(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string ad, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            ad = Temiz(ad);
            vergiNo = Temiz(vergiNo);
            if (ad == "" && vergiNo == "") return Hata("Aramak için ad ya da kimlik / vergi no yazınız.");
            try
            {
                TNS.HRC.FirmaBilgisi h = new TNS.HRC.FirmaBilgisi();
                h.muhasebe = muhasebe ?? 0;
                h.ad = ad;
                h.vergiNo = vergiNo;
                h.aktifDurum = 1;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, h);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc?.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new { kod = f.kod, vergiNo = f.vergiNo ?? "", ad = f.ad ?? "", vergiDairesi = f.vergiDairesi ?? "", adres = f.adres ?? "" });
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        // ------------------------------------------------------------------ raporlar (eski kodla)

        /// <summary>tur 1: eski btnYazdir_Click (KiralikAraclarListele), 2: eski btnYazdirOzet_Click "Tablo Yazdır" (HizmetAlimiTasitlariDetayHazirlaKiralik).</summary>
        private static IResult Rapor(HttpContext baglam, IWebHostEnvironment ortam, int yil, int? muhasebe, string birim, int? tur)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            Hashtable raporParam = new Hashtable();
            raporParam.Add("ButceYil", yil.ToString());
            raporParam.Add("KurKod", TNS.DEG.Arac.VarsayilanKurumBul());
            raporParam.Add("saymanlikNo", (muhasebe ?? 0).ToString());
            raporParam.Add("birimKod", Temiz(birim));
            global::ButceForm.ButceFormRaporlari ir = new global::ButceForm.ButceFormRaporlari();
            if ((tur ?? 1) == 2)
                return EkFormUclari.RaporGonder(() => { AsposeLisans.Yukle(); return ir.HizmetAlimiTasitlariDetayHazirlaKiralik(kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(), raporParam, ""); },
                    "formKiralik");
            return EkFormUclari.RaporGonder(() => { AsposeLisans.Yukle(); return ir.KiralikAraclarListele(kullanan, TNS.BHF.Arac.Tanimla(), null, DosyaIslem.DosyaAdUret(), EkFormUclari.ExcelTur(), raporParam, ""); },
                "formKiralikListe");
        }
    }
}
