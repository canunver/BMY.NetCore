using System;
using System.Collections.Generic;
using System.Globalization;
using Istemci.OrtakSayfa;
using Istemci.Oturum;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;

namespace Istemci.Muhasebe
{
    public class MakbuzAnahtar
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string YevmiyeNo { get; set; }
    }

    public class MakbuzIptalIstegi
    {
        public List<MakbuzAnahtar> Makbuzlar { get; set; }
    }

    public class MakbuzOdemeEmriIstegi
    {
        /// <summary>yyyy-MM-dd</summary>
        public string Bas { get; set; }
        public string Bit { get; set; }
        public int Muhasebe { get; set; }
    }

    public class MakbuzHesapIstegi
    {
        public string hesapKod { get; set; }
        public string hesapAd { get; set; }
        public string altHesap { get; set; }
        public string altHesapAd { get; set; }
        public string tertip { get; set; }
        public double tutar { get; set; }
    }

    public class MakbuzKaydetIstegi
    {
        /// <summary>Kayıtlı makbuzda yevmiye no (yeni makbuzda boş).</summary>
        public string YevmiyeNo { get; set; }
        public int Muhasebe { get; set; }
        /// <summary>Sayfanın açıldığı birim (eski adres satırındaki birim).</summary>
        public string Birim { get; set; }
        /// <summary>Kayıtlı makbuzun birimi (eski txtMakbuzBirim; değiştirilebilir).</summary>
        public string MakbuzBirim { get; set; }
        /// <summary>yyyy-MM-dd</summary>
        public string Tarih { get; set; }
        public int MakbuzTipi { get; set; }
        public string TcKimlikNoVergiNo { get; set; }
        public string KisiAd { get; set; }
        public double Tutar { get; set; }
        public string ParaBirimi { get; set; }
        public string Aciklama { get; set; }
        public string Duzenleyen { get; set; }
        public string Unvan { get; set; }
        /// <summary>Kart ile ödenen; boşsa null (eski -1).</summary>
        public double? KrediKarti { get; set; }
        public List<MakbuzHesapIstegi> Hesaplar { get; set; }
    }

    public class MakbuzOdendiIstegi
    {
        public int Yil { get; set; }
        public int Muhasebe { get; set; }
        public string YevmiyeNo { get; set; }
        public double? KrediKarti { get; set; }
    }

    public class MakbuzAciklamaIstegi
    {
        public int Kod { get; set; }
        public string Aciklama { get; set; }
        public string Birim { get; set; }
        public int MakbuzTipi { get; set; }
    }

    /// <summary>
    /// Makbuz Bilgileri (eski ButceMuhasebe/Makbuz.aspx) ve içinden açılan sayfaların (MakbuzGiris.aspx makbuz formu, MakbuzYazdir.aspx
    /// makbuz çıktısı; formdaki hesap planı, kişi / firma, tertip ve açıklama pencereleri) JavaScript'ten çağırdığı uçlar:
    ///   Liste: GET  /api/makbuz/tanim, liste, rapor/liste (Kesilen Makbuzlar, Excel), rapor/hesap (Hesap Bazında Rapor, Excel)
    ///          POST /api/makbuz/iptal (seçilenleri iptal et), odeme-emri (Ödeme Emri Oluştur)
    ///   Form : GET  /api/makbuz/form/tanim, form/yazdir (PDF), tip-aciklama, firma, firma/ara, hesap/agac, hesap/ara, hesap/ad, tertip,
    ///               aciklama/liste, aciklama/oku
    ///          POST /api/makbuz/form/kaydet, form/odendi, aciklama/kaydet, aciklama/sil
    /// Yetki menüdeki Makbuz Bilgileri seçeneğinden gelir (MakbuzGiris menüde yok, alt sayfadır); kayıt, iptal, ödendi işaretleme yetkileri
    /// eskisi gibi ayrıca iş kurallarında (TNS.MUH) denetlenir.
    /// </summary>
    public static class MakbuzUclari
    {
        public const string SayfaAdresi = "../ButceMuhasebe/Makbuz.aspx";
        public const string FormAdresi = "../ButceMuhasebe/MakbuzGiris.aspx";
        private const string Uc = "/api/makbuz/";

        private static readonly CultureInfo Tr = new CultureInfo("tr-TR");

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Uc + "tanim", Tanim);
            app.MapGet(Uc + "liste", Liste);
            app.MapPost(Uc + "iptal", Iptal);
            app.MapGet(Uc + "rapor/liste", RaporListe);
            app.MapGet(Uc + "rapor/hesap", RaporHesap);
            app.MapPost(Uc + "odeme-emri", OdemeEmri);

            app.MapGet(Uc + "form/tanim", FormTanim);
            app.MapPost(Uc + "form/kaydet", FormKaydet);
            app.MapPost(Uc + "form/odendi", FormOdendi);
            app.MapGet(Uc + "form/yazdir", FormYazdir);
            app.MapGet(Uc + "tip-aciklama", TipAciklama);
            app.MapGet(Uc + "firma", Firma);
            app.MapGet(Uc + "firma/ara", FirmaAra);
            app.MapGet(Uc + "hesap/agac", HesapAgac);
            app.MapGet(Uc + "hesap/ara", HesapAra);
            app.MapGet(Uc + "hesap/ad", HesapAd);
            app.MapGet(Uc + "tertip", Tertip);
            app.MapGet(Uc + "aciklama/liste", AciklamaListe);
            app.MapGet(Uc + "aciklama/oku", AciklamaOku);
            app.MapPost(Uc + "aciklama/kaydet", AciklamaKaydet);
            app.MapPost(Uc + "aciklama/sil", AciklamaSil);
        }

        // ------------------------------------------------------------------ yetki ve ortak

        /// <summary>Liste sayfası menüdeki adresle, form sayfası (menüde yok) üst sayfası Makbuz Bilgileri ile denetlenir.</summary>
        public static bool YetkiVar(Kullanici kullanan, string uygulamaKlasoru, bool form = false)
        {
            if (kullanan == null) return false;
            return form ? SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, FormAdresi, SayfaAdresi)
                        : SayfaYetki.Yetkili(kullanan, uygulamaKlasoru, SayfaAdresi);
        }

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!YetkiVar(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }

        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata ?? "" }); }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = Temiz(hata);
            if (hata == "" || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static bool TarihCoz(string metin, out DateTime tarih)
        {
            return DateTime.TryParseExact(Temiz(metin), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih);
        }

        private static string TarihYaz(TNSDateTime t)
        {
            try
            {
                if (t == null || t.isNull) return "";
                DateTime d = t.Oku();
                return d <= DateTime.MinValue.AddDays(1) ? "" : d.ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static bool Veznedar(Kullanici k) { return TNS.MUH.Yetki.VeznedarMi(k) || TNS.MUH.Yetki.VeznedarMakbuzHazirlayan(k); }

        /// <summary>Eski MakbuzTipiDoldur: aktif makbuz tipleri.</summary>
        private static List<object> MakbuzTipleri(Kullanici kullanan, bool basaBosEkle)
        {
            List<object> liste = new List<object>();
            if (basaBosEkle) liste.Add(new { kod = 0, aciklama = "" });
            MakbuzTipi bos = new MakbuzTipi();
            bos.durum = 1; // Aktif olanlar
            ObjectArray mtler = Servis().MakbuzTipiListele(kullanan, bos);
            if (mtler != null && mtler.objeler != null)
                foreach (MakbuzTipi t in mtler.objeler)
                    liste.Add(new { kod = t.kod, aciklama = t.aciklama });
            return liste;
        }

        private static object Satir(Makbuz m)
        {
            return new
            {
                yil = m.yil,
                yevmiyeNo = m.yevmiyeNo,
                islemTarih = TarihYaz(m.islemTarih),
                durum = m.durum,
                durumAd = m.durumAd,
                muhasebe = m.muhasebe,
                muhasebeAd = m.muhasebeAd,
                birim = m.birim,
                birimAd = m.birimAd,
                makbuzTipiAciklama = m.makbuzTipiAciklama,
                kisiAd = m.kisiAd,
                tcKimlikNoVergiNo = m.tcKimlikNoVergiNo,
                genelToplam = m.genelToplam,
                paraBirimiKod = m.paraBirimiKod,
                duzenleyen = m.duzenleyen,
                unvan = m.unvan,
                aciklama = m.aciklama,
                odemeEmriNo = m.odemeEmriNo
            };
        }

        /// <summary>Eski KriterTopla: listenin sorgu alanları.</summary>
        private static Makbuz Kriter(string bas, string bit, int? muhasebe, string birim, int? tip, string yevmiyeNo, string hesapKod,
            string tcKimlik, string kisiAd, int? durum)
        {
            TarihCoz(bas, out DateTime t1);
            TarihCoz(bit, out DateTime t2);
            if (t1 == DateTime.MinValue) t1 = DateTime.Today;
            if (t2 == DateTime.MinValue) t2 = t1;

            Makbuz m = new Makbuz();
            m.yil = t1.Year;
            m.muhasebe = muhasebe ?? 0;
            m.birim = Temiz(birim);
            m.makbuzTipi.kod = tip ?? 0;
            m.tcKimlikNoVergiNo = tcKimlik ?? "";
            m.kisiAd = Temiz(kisiAd);
            m.araBasTarih.Yaz(t1);
            m.araBitTarih.Yaz(t2);
            m.yevmiyeNo = Temiz(yevmiyeNo);
            m.sorguHesapKod = Temiz(hesapKod);
            if (!string.IsNullOrEmpty(m.sorguHesapKod))
            {
                string[] sh = m.sorguHesapKod.Split('-');
                m.sorguHesapKod = sh[0].Trim().Replace(".", "");
            }
            m.durum = durum ?? 0;
            return m;
        }

        // ------------------------------------------------------------------ liste sayfası

        /// <summary>Açılış (eski Page_Load): tarih bugün, kullanıcının muhasebe / makbuz birimi, makbuz tipleri, durumlar.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBE");
                string birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHASEBEMAKBUZBIRIM");
                string kurum;
                string birimAd = MuhasebeBirimUclari.BirimAdBul(OrtakFonksiyonlar.ConvertToInt(muhasebe, 0), birim, out kurum);
                return Results.Json(new
                {
                    bugun = DateTime.Today.ToString("yyyy-MM-dd"),
                    muhasebe = muhasebe, birim = birim, birimAd = birimAd,
                    kurumsal = MuhasebeBirimUclari.KurumsalYaz(DateTime.Now.Year, kurum),
                    makbuzTipleri = MakbuzTipleri(kullanan, true),
                    durumlar = new object[]
                    {
                        new { kod = 0, ad = "Hepsi" }, new { kod = 1, ad = "Yeni" }, new { kod = 3, ad = "Ödendi" },
                        new { kod = 5, ad = "Onaylandı" }, new { kod = 9, ad = "İptal" }
                    },
                    durum = 1
                });
            }
            catch (Exception)
            {
                return Hata("Sayfa bilgileri okunamadı.");
            }
        }

        /// <summary>Listele (eski btnListele_Click).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, string bas, string bit, int? muhasebe, string birim, int? tip,
            string yevmiyeNo, string hesapKod, string tcKimlik, string kisiAd, int? durum)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray mler = Servis().MakbuzListele(kullanan, Kriter(bas, bit, muhasebe, birim, tip, yevmiyeNo, hesapKod, tcKimlik, kisiAd, durum));
                if (mler == null || !mler.sonuc.islemSonuc) return Hata(HataMetni(mler?.sonuc.hataStr, "Listeleme yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (Makbuz m in mler.objeler) satirlar.Add(Satir(m));
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Listeleme yapılamadı.");
            }
        }

        /// <summary>Seçilenleri İptal Et (eski btnIptal_Click): seçili makbuzların durumu 9 (iptal).</summary>
        private static IResult Iptal(HttpContext baglam, IWebHostEnvironment ortam, MakbuzIptalIstegi istek)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek?.Makbuzlar == null || istek.Makbuzlar.Count == 0) return Basarisiz("Herhangi bir Makbuz seçilmemiş!");
            try
            {
                TNSCollection mler = new TNSCollection();
                foreach (MakbuzAnahtar a in istek.Makbuzlar)
                {
                    if (a == null) continue;
                    Makbuz m = new Makbuz(a.Yil, a.Muhasebe, Temiz(a.YevmiyeNo));
                    m.durum = 9;
                    mler.Add(m);
                }
                if (mler.Count == 0) return Basarisiz("Herhangi bir Makbuz seçilmemiş!");
                Sonuc sonuc = Servis().MakbuzDurumGuncelle(kullanan, mler);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "İptal işlemi yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = HataMetni(sonuc.bilgiStr, "Seçilen makbuzlar iptal edildi.") });
            }
            catch (Exception)
            {
                return Basarisiz("İptal işlemi yapılamadı.");
            }
        }

        /// <summary>Kesilen Makbuzlar (eski btnKestigimMakbuzlar_Click): MUH\MakbuzListesi.xlt.</summary>
        private static IResult RaporListe(HttpContext baglam, IWebHostEnvironment ortam, string bas, string bit, int? muhasebe, string birim, int? tip,
            string yevmiyeNo, string hesapKod, string tcKimlik, string kisiAd, int? durum)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string sablon = MakbuzRaporlari.SablonBul(ortam.ContentRootPath, MakbuzRaporlari.ListeSablon);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\" + MakbuzRaporlari.ListeSablon + ").");
            try
            {
                Makbuz kriter = Kriter(bas, bit, muhasebe, birim, tip, yevmiyeNo, hesapKod, tcKimlik, kisiAd, durum);
                ObjectArray mler = Servis().MakbuzListele(kullanan, kriter);
                if (mler == null || !mler.sonuc.islemSonuc) return Hata(HataMetni(mler?.sonuc.hataStr, "Rapor hazırlanamadı."));
                if (mler.objeler.Count <= 0) return Hata("Verilen kriterlere uygun kayıt bulunamadı.");
                AsposeLisans.Yukle();
                string dosya = MakbuzRaporlari.KesilenMakbuzlar(mler, sablon, kriter.araBasTarih.Oku(), kriter.araBitTarih.Oku());
                return DosyaIslem.DosyaGonder(dosya, "Makbuz", true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>Hesap Bazında Rapor (eski btnRaporHesap_Click): MUH\MakbuzListesiMizan.xlt.</summary>
        private static IResult RaporHesap(HttpContext baglam, IWebHostEnvironment ortam, string bas, string bit, int? muhasebe, string birim, int? tip,
            string yevmiyeNo, string hesapKod, string tcKimlik, string kisiAd, int? durum)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string sablon = MakbuzRaporlari.SablonBul(ortam.ContentRootPath, MakbuzRaporlari.MizanSablon);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\" + MakbuzRaporlari.MizanSablon + ").");
            try
            {
                Makbuz kriter = Kriter(bas, bit, muhasebe, birim, tip, yevmiyeNo, hesapKod, tcKimlik, kisiAd, durum);
                ObjectArray mler = Servis().MakbuzListeleMizan(kullanan, kriter);
                if (mler == null || !mler.sonuc.islemSonuc) return Hata(HataMetni(mler?.sonuc.hataStr, "Rapor hazırlanamadı."));
                if (mler.objeler.Count <= 0) return Hata("Verilen kriterlere uygun kayıt bulunamadı.");
                AsposeLisans.Yukle();
                string dosya = MakbuzRaporlari.HesapBazinda(mler, sablon, kriter.araBasTarih.Oku(), kriter.araBitTarih.Oku());
                return DosyaIslem.DosyaGonder(dosya, "MakbuzMizan", true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        /// <summary>Ödeme Emri Oluştur (eski btnOdemeEmri_Click): tarih aralığındaki ödenmiş makbuzlardan birim (ya da kişi) bazında ödeme emri.</summary>
        private static IResult OdemeEmri(HttpContext baglam, IWebHostEnvironment ortam, MakbuzOdemeEmriIstegi istek)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || !TarihCoz(istek.Bas, out DateTime bas)) return Basarisiz("Lütfen tarih giriniz.");
            if (!TarihCoz(istek.Bit, out DateTime bit)) bit = bas;
            if (bit < bas) return Basarisiz("Bitiş tarihi başlangıç tarihinden önce olamaz.");
            if (istek.Muhasebe <= 0) return Basarisiz("Lütfen muhasebe seçiniz.");
            try
            {
                List<string> mesajlar = MakbuzOdemeEmri.Olustur(kullanan, Servis(), bas, bit, istek.Muhasebe, out string hata);
                if (hata != null) return Basarisiz(HataMetni(hata, "Ödeme emri oluşturulamadı."));
                if (mesajlar.Count == 0) mesajlar.Add("Ödeme emri oluşturulacak makbuz yok.");
                return Results.Json(new { basarili = true, mesajlar = mesajlar });
            }
            catch (Exception)
            {
                return Basarisiz("Ödeme emri oluşturulamadı.");
            }
        }

        // ------------------------------------------------------------------ makbuz formu (eski MakbuzGiris.aspx)

        /// <summary>Eski TuslariAcKapat (ve Page_Load'daki Kaydet yetkisi): Kaydet, Yazdır, Ödendi tuşlarının görünürlüğü.</summary>
        private static object Tuslar(Kullanici kullanan, string makbuzKullanici, int durum)
        {
            bool kaydet = TNS.MUH.Yetki.SistemYoneticisimi(kullanan) || TNS.MUH.Yetki.MuhasebeYoneticisimi(kullanan)
                || TNS.MUH.Yetki.MakbuzKullanicisimi(kullanan) || kullanan.KullaniciTipiIceriyorMu(18); // Ödeme Emri Yetkisi
            if (kaydet && TNS.MUH.Yetki.VeznedarMi(kullanan) && !TNS.MUH.Yetki.MuhasebeYoneticisimi(kullanan)) kaydet = false;

            bool yazdir = (durum == (int)ENUMBelgeDurumu.ONAYLI || durum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI) && Veznedar(kullanan);
            bool odendi = (durum == (int)ENUMBelgeDurumu.YENI || durum == (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI) && Veznedar(kullanan);
            if ((makbuzKullanici ?? "") != kullanan.kullaniciKodu && !TNS.MUH.Yetki.MuhasebeYoneticisimi(kullanan))
                kaydet = kaydet && !odendi;
            return new { kaydet = kaydet, yazdir = yazdir, odendi = odendi };
        }

        /// <summary>Eski ListedenEkrana: formun alanları (hesaplanan alanlar dışındaki hesap satırları, en az 7 satır).</summary>
        private static object FormMakbuz(Kullanici kullanan, Makbuz m)
        {
            m.HesapSatirEkle(7);
            List<object> hesaplar = new List<object>();
            foreach (MakbuzHesap mh in m.hesaplar)
            {
                if (mh.hesaplananAlan > 0) continue;
                hesaplar.Add(new { hesapKod = mh.hesapKod ?? "", hesapAd = mh.hesapAd ?? "", altHesap = mh.altHesap ?? "", altHesapAd = mh.altHesapAd ?? "", tertip = mh.tertip ?? "", tutar = mh.tutar });
            }
            string paraBirimiKod = string.IsNullOrEmpty(m.paraBirimiKod) ? "TRY" : m.paraBirimiKod;
            return new
            {
                yil = m.yil,
                yevmiyeNo = m.yevmiyeNo,
                muhasebe = m.muhasebe,
                birim = m.birim,
                islemTarih = TarihYaz(m.islemTarih),
                tcKimlikNoVergiNo = m.tcKimlikNoVergiNo ?? "",
                kisiAd = m.kisiAd ?? "",
                makbuzTipi = m.makbuzTipi.kod,
                makbuzTipiTutar = m.makbuzTipiTutar,
                paraBirimiKod = paraBirimiKod,
                krediKartiIleOdenen = m.krediKartiIleOdenen,
                aciklama = m.aciklama ?? "",
                duzenleyen = m.duzenleyen ?? "",
                unvan = m.unvan ?? "",
                durum = m.durum,
                durumAd = m.durumAd,
                odemeEmriNo = m.odemeEmriNo ?? "",
                genelToplam = m.genelToplam,
                hesaplar = hesaplar,
                tuslar = Tuslar(kullanan, m.kullaniciKodu, m.durum)
            };
        }

        private static Makbuz MakbuzOku(Kullanici kullanan, int yil, int muhasebe, string yevmiyeNo, out string hata)
        {
            hata = null;
            ObjectArray mler = Servis().MakbuzListele(kullanan, new Makbuz(yil, muhasebe, yevmiyeNo));
            if (mler == null || !mler.sonuc.islemSonuc) { hata = HataMetni(mler?.sonuc.hataStr, "Makbuz okunamadı."); return null; }
            if (mler.objeler.Count != 1) { hata = "Makbuz bulunamadı."; return null; }
            return (Makbuz)mler.objeler[0];
        }

        /// <summary>
        /// Form açılışı (eski MakbuzGiris Page_Load): kullanıcının yetkileri, makbuz tipleri, para birimleri, kurumsal kod; makbuz no
        /// verildiyse kayıtlı makbuz, verilmediyse bugünün tarihi ve son kullanılan düzenleyen / unvan.
        /// </summary>
        private static IResult FormTanim(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muhasebe, string birim, string makbuzNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                int muh = muhasebe ?? 0;
                birim = Temiz(birim);
                makbuzNo = Temiz(makbuzNo);

                // Eski KurumsalKodBul
                string kurumsal = "";
                MuhasebeBirim mb = new MuhasebeBirim();
                mb.muhasebe = muh;
                mb.birimKod = birim;
                ObjectArray mbl = Servis().MuhasebeBirimListele(mb);
                if (mbl != null && mbl.objeler != null)
                    foreach (MuhasebeBirim mbb in mbl.objeler) kurumsal = mbb.kurum;
                string birimAd = MuhasebeBirimUclari.BirimAdBul(muh, birim, out string _);

                List<string> paraBirimleri = new List<string>();
                bool doviz = TNS.MUH.Arac.MuhasebeDovizVarMi();
                if (doviz)
                {
                    ObjectArray paralar = TNS.HRC.Arac.Tanimla().ParaBirimiListele(kullanan, new TNS.BKD.ParaBirimi());
                    if (paralar != null && paralar.objeler != null)
                        foreach (TNS.BKD.ParaBirimi p in paralar.objeler) paraBirimleri.Add(p.kisaltma);
                }

                object makbuz;
                if (makbuzNo == "")
                {
                    Makbuz m = new Makbuz();
                    m.muhasebe = muh;
                    m.birim = birim;
                    m.duzenleyen = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHMAKBUZDUZENLEYEN");
                    m.unvan = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "MUHMAKBUZDUZENLEYENUNVAN");
                    m.islemTarih.Yaz(DateTime.Now.Date);
                    m.yil = m.islemTarih.Yil;
                    makbuz = FormMakbuz(kullanan, m);
                }
                else
                {
                    Makbuz m = MakbuzOku(kullanan, yil ?? 0, muh, makbuzNo, out string hata);
                    if (m == null) return Hata(hata);
                    makbuz = FormMakbuz(kullanan, m);
                }

                return Results.Json(new
                {
                    muhasebe = muh,
                    birim = birim,
                    birimAd = birimAd,
                    kurumsal = kurumsal ?? "",
                    kisiElle = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.WebConfigOku("MakbuzKisiElleYazilabilir", ""), 0) > 0,
                    plaka = TNS.MUH.Arac.VarsayilanKurumKod().Replace(".", "") == "460604",
                    doviz = doviz,
                    paraBirimleri = paraBirimleri,
                    yeniTuslar = Tuslar(kullanan, "", 0),
                    makbuzTipleri = MakbuzTipleri(kullanan, false),
                    makbuz = makbuz
                });
            }
            catch (Exception)
            {
                return Hata("Makbuz bilgileri okunamadı.");
            }
        }

        /// <summary>Kaydet (eski btnKaydet_Click).</summary>
        private static IResult FormKaydet(HttpContext baglam, IWebHostEnvironment ortam, MakbuzKaydetIstegi istek)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Basarisiz("Makbuz bilgileri alınamadı.");
            if (!TarihCoz(istek.Tarih, out DateTime tarih)) return Basarisiz("Makbuz tarihini giriniz!");
            try
            {
                IMUHServis servis = Servis();
                string odemeEmriNoGuncelle = "";
                string birim = Temiz(istek.Birim);

                Makbuz m = new Makbuz();
                m.islemTarih.Yaz(tarih);
                m.yil = m.islemTarih.Yil;
                m.yevmiyeNo = Temiz(istek.YevmiyeNo);
                m.muhasebe = istek.Muhasebe;

                string makbuzBirim = Temiz(istek.MakbuzBirim);
                if (makbuzBirim != "" && birim != makbuzBirim)
                {
                    birim = makbuzBirim;

                    // Eğer kayıt edilmiş olan makbuzun birimi değişmiş ise ve bu makbuz ile ödeme emri oluşturulmuş ise yeni kayıt edilen birime
                    // ait olan ödeme emri numarasını güncellemek gerekli. Yoksa sistem bu ödeme emri numarasını görüyor ve 2 birimede aynı ödeme emri
                    // belgesini kayıt edip diğerini ezmiş oluyor. EGO Melih 20.08.2015
                    Makbuz makbuz = new Makbuz();
                    makbuz.yil = m.yil;
                    makbuz.muhasebe = m.muhasebe;
                    makbuz.birim = birim;
                    makbuz.araBasTarih.Yaz(tarih);
                    makbuz.araBitTarih.Yaz(tarih.AddDays(1));

                    ObjectArray mler = servis.MakbuzListele(kullanan, makbuz);
                    if (mler != null && mler.objeler != null)
                    {
                        foreach (Makbuz mm in mler.objeler)
                        {
                            if (mm.odemeEmriNo != "")
                            {
                                odemeEmriNoGuncelle = mm.odemeEmriNo; // Makbuzun yapıldığı yeni birime ait önceden hazırlanmış ödeme emri numarası
                                break;
                            }
                        }
                        // Eğer değiştirilen birimden ödeme emri oluşturulmamış ise ödeme emri boş olsun
                        if (mler.objeler.Count == 0)
                            odemeEmriNoGuncelle = "BOS";
                    }
                }

                m.birim = birim;
                m.makbuzTipi.kod = istek.MakbuzTipi;
                m.tcKimlikNoVergiNo = Temiz(istek.TcKimlikNoVergiNo);
                m.kisiAd = Temiz(istek.KisiAd);
                m.makbuzTipiTutar = Math.Round(istek.Tutar, 2);
                m.aciklama = Temiz(istek.Aciklama);
                m.duzenleyen = Temiz(istek.Duzenleyen);
                m.unvan = Temiz(istek.Unvan);
                m.paraBirimiKod = Temiz(istek.ParaBirimi);

                m.hesaplar = new TNSCollection();
                if (istek.Hesaplar != null)
                    foreach (MakbuzHesapIstegi h in istek.Hesaplar)
                    {
                        if (h == null) continue;
                        MakbuzHesap mh = new MakbuzHesap();
                        mh.hesapKod = Temiz(h.hesapKod);
                        mh.hesapAd = Temiz(h.hesapAd);
                        mh.altHesap = Temiz(h.altHesap);
                        mh.altHesapAd = Temiz(h.altHesapAd);
                        mh.tertip = Temiz(h.tertip);
                        mh.tutar = Math.Round(h.tutar, 2);
                        if (mh.hesapKod == "" && mh.altHesap == "" && mh.tertip == "" && mh.tutar == 0) continue;
                        m.hesaplar.Add(mh);
                    }

                double krediKarti = istek.KrediKarti.HasValue ? Math.Round(istek.KrediKarti.Value, 2) : -1;
                m.krediKartiIleOdenen = krediKarti;
                int durumKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.WebConfigOku("MakbuzOdemeEmriOlusturDurum", ""), 0);
                if (durumKod == 1 && m.krediKartiIleOdenen == -1)
                    m.krediKartiIleOdenen = -2; // Yeni durumundan ödeme emri oluşuyorsa kredi kart ödemesi ilk kayıt edilirken 0 olmalı

                Sonuc sonuc = servis.MakbuzKaydet(kullanan, m);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Kayıt işlemi yapılamadı."));

                if (odemeEmriNoGuncelle != "")
                    OdemeEmriNoGuncelle(kullanan, servis, m.yil, m.muhasebe, m.yevmiyeNo, odemeEmriNoGuncelle);

                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBE", m.muhasebe.ToString());
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHASEBEMAKBUZBIRIM", birim);
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHMAKBUZDUZENLEYEN", m.duzenleyen);
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "MUHMAKBUZDUZENLEYENUNVAN", m.unvan);

                Makbuz okunan = MakbuzOku(kullanan, m.yil, m.muhasebe, sonuc.anahtar, out string _);
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Makbuz başarıyla kayıt edildi.",
                    yevmiyeNo = sonuc.anahtar,
                    makbuz = okunan == null ? null : FormMakbuz(kullanan, okunan)
                });
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi yapılamadı.");
            }
        }

        /// <summary>Eski OdemeEmriNoGuncelle: birimi değişen makbuzun eski ödeme emrini iptal edip yeni birimin ödeme emrine bağlar.</summary>
        private static void OdemeEmriNoGuncelle(Kullanici kullanan, IMUHServis servis, int yil, int muhasebe, string yevmiyeNo, string yeniOdemeEmriNo)
        {
            Makbuz makbuz = new Makbuz();
            makbuz.yil = yil;
            makbuz.muhasebe = muhasebe;
            makbuz.yevmiyeNo = yevmiyeNo;
            ObjectArray mler = servis.MakbuzListele(kullanan, makbuz);

            if (yeniOdemeEmriNo == "BOS") yeniOdemeEmriNo = "";

            string iptalEdilecekOdemeEmriNo = "";
            ObjectArray makbuzlar = new ObjectArray();
            foreach (Makbuz mm in mler.objeler)
            {
                iptalEdilecekOdemeEmriNo = mm.odemeEmriNo;
                mm.odemeEmriNo = yeniOdemeEmriNo;
                makbuzlar.objeler.Add(mm);
            }

            Sonuc sonuc = new Sonuc(true, "", "");
            if (iptalEdilecekOdemeEmriNo != "")
            {
                TNSCollection fler = new TNSCollection();

                OdemeEmriMIFForm f = new OdemeEmriMIFForm();
                f.yil = yil;
                f.belgeNo = iptalEdilecekOdemeEmriNo;
                f.kurum = GenelIslemlerIstemci.VarsayilanKurumBul();
                f.islemYapan = kullanan.kullaniciKodu;
                f.islemYapanAd = kullanan.ad + " " + kullanan.soyad;
                f.formDurum = (int)ENUMBelgeDurumu.IPTAL;

                if (f.belgeNo != "") fler.Add(f);

                sonuc = servis.BelgeDurumDegistirODEMIF(kullanan, fler, true);
            }

            if (sonuc.islemSonuc)
                servis.MakbuzOdemeEmriGuncelle(kullanan, makbuzlar);
        }

        /// <summary>Ödendi (eski btnOdendi_Click): makbuz ödendi (muhasebata gönderildi) olarak işaretlenir.</summary>
        private static IResult FormOdendi(HttpContext baglam, IWebHostEnvironment ortam, MakbuzOdendiIstegi istek)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || Temiz(istek.YevmiyeNo) == "") return Basarisiz("Önce makbuzu kaydediniz.");
            try
            {
                Makbuz makbuz = new Makbuz();
                makbuz.yil = istek.Yil;
                makbuz.muhasebe = istek.Muhasebe;
                makbuz.yevmiyeNo = Temiz(istek.YevmiyeNo);
                makbuz.durum = (int)ENUMBelgeDurumu.MUHASEBATAGONDERILDI;
                makbuz.krediKartiIleOdenen = istek.KrediKarti.HasValue ? Math.Round(istek.KrediKarti.Value, 2) : -1;

                Sonuc s = Servis().MakbuzOdendi(kullanan, makbuz);
                if (s == null || !s.islemSonuc) return Basarisiz(HataMetni(s?.hataStr, "İşlem yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Ödendi olarak işaretlendi", tuslar = Tuslar(kullanan, "", makbuz.durum), durum = makbuz.durum, durumAd = makbuz.durumAd });
            }
            catch (Exception)
            {
                return Basarisiz("İşlem yapılamadı.");
            }
        }

        /// <summary>Yazdır (eski MakbuzYazdir.aspx): MUH\Makbuz.dotx, PDF.</summary>
        private static IResult FormYazdir(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muhasebe, string makbuzNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            string sablon = MakbuzRaporlari.SablonBul(ortam.ContentRootPath, MakbuzRaporlari.MakbuzSablon);
            if (sablon == null) return Hata("Rapor şablonu bulunamadı (MUH\\" + MakbuzRaporlari.MakbuzSablon + ").");
            try
            {
                ObjectArray mler = Servis().MakbuzListele(kullanan, new Makbuz(yil ?? 0, muhasebe ?? 0, Temiz(makbuzNo)));
                if (mler == null || !mler.sonuc.islemSonuc) return Hata(HataMetni(mler?.sonuc.hataStr, "Makbuz okunamadı."));
                if (mler.objeler.Count <= 0) return Hata("Verilen kriterlere uygun kayıt bulunamadı.");
                AsposeLisans.Yukle();
                string dosya = MakbuzRaporlari.MakbuzYazdir(kullanan, (Makbuz)mler.objeler[0], sablon);
                return DosyaIslem.DosyaGonder(dosya, "Makbuz.pdf", true, "pdf");
            }
            catch (Exception)
            {
                return Hata("Makbuz yazdırılamadı.");
            }
        }

        /// <summary>Makbuz tipi seçilince (eski cmbMakbuzTipler_SelectedIndexChanged): tipe bağlı açıklamalar.</summary>
        private static IResult TipAciklama(HttpContext baglam, IWebHostEnvironment ortam, int? tip)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                List<string> aciklamalar = new List<string>();
                if ((tip ?? 0) > 0)
                {
                    MakbuzAciklama t = new MakbuzAciklama();
                    t.makbuzTipiKod = tip.Value;
                    ObjectArray bilgiler = Servis().MakbuzAciklamaListele(t);
                    if (bilgiler != null && bilgiler.sonuc.islemSonuc)
                        foreach (MakbuzAciklama tt in bilgiler.objeler) aciklamalar.Add(tt.aciklama);
                }
                return Results.Json(new { aciklamalar = aciklamalar });
            }
            catch (Exception)
            {
                return Hata("Açıklamalar okunamadı.");
            }
        }

        // ------------------------------------------------------------------ kişi / firma (eski HarcamaSurecleri/ListeFirmaKisi)

        /// <summary>Kimlik no yazılınca (eski FirmaBilgisiGetir): kişi / firma adı.</summary>
        private static IResult Firma(HttpContext baglam, IWebHostEnvironment ortam, int? muhasebe, string vergiNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            vergiNo = Temiz(vergiNo);
            if (vergiNo == "") return Results.Json(new { ad = "" });
            try
            {
                TNS.HRC.FirmaBilgisi firma = new TNS.HRC.FirmaBilgisi();
                firma.muhasebe = muhasebe ?? 0;
                firma.vergiNo = vergiNo;
                ObjectArray bilgi = TNS.HRC.Arac.Tanimla().FirmaBilgisiListele(kullanan, firma);
                string ad = "";
                if (bilgi != null && bilgi.objeler != null && bilgi.objeler.Count == 1)
                    ad = ((TNS.HRC.FirmaBilgisi)bilgi.objeler[0]).ad;
                return Results.Json(new { ad = ad ?? "" });
            }
            catch (Exception)
            {
                return Hata("Kişi / firma bilgisi okunamadı.");
            }
        }

        /// <summary>Kişi / firma arama (eski ListeFirmaKisi ve alt hesap otomatik tamamlama AltHesapListesiDoldur).</summary>
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
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.FirmaBilgisi f in bilgi.objeler)
                {
                    satirlar.Add(new { vergiNo = f.vergiNo ?? "", ad = f.ad ?? "", vergiDairesi = f.vergiDairesi ?? "", adres = f.adres ?? "" });
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        // ------------------------------------------------------------------ hesap planı (eski ListeHesapPlani ve HesapKod.ascx)

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        /// <summary>Hesap planı ağacı (eski ListeHesapPlani.DalYukle): kökte 1-9 ana gruplar, sonra düzey düzey hesaplar.</summary>
        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                int y = HesapYil(yil);
                ust = Temiz(ust);
                List<object> dugumler = new List<object>();
                if (ust == "")
                {
                    TNS.UZY.IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();
                    for (int i = 1; i < 10; i++)
                    {
                        string ad = servisUZY.UzayDegeriStr(new Kullanici(), "MUHHESAPPLANI", y.ToString() + "-" + i.ToString(), true);
                        dugumler.Add(new { kod = i.ToString(), ad = i.ToString() + "-" + ad, hesapAd = ad ?? "", altVar = true, detay = false });
                    }
                    return Results.Json(new { dugumler = dugumler });
                }

                string[] hesapKirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                if (ust.Length < 3) hs.seviye = 1;
                else hs.seviye = hesapKirilimlar.Length + 1;

                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = Servis().HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }
            catch (Exception)
            {
                return Hata("Hesap planı okunamadı.");
            }
        }

        /// <summary>Hesap kodu / adı ile arama (eski HesapListesiDoldur). sonKirilim: yalnızca detay (son kırılım) hesaplar.</summary>
        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            metin = Temiz(metin);
            if (metin.Length < 2) return Hata("En az iki karakter yazınız.");
            try
            {
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = HesapYil(yil);
                h.hesapKodHesapAd = metin;
                h.detay = sonKirilim != "0";
                ObjectArray hesap = Servis().HesapPlaniListele(kullanan, h);
                if (hesap == null || !hesap.sonuc.islemSonuc) return Hata(HataMetni(hesap?.sonuc.hataStr, "Arama yapılamadı."));
                List<object> satirlar = new List<object>();
                foreach (HesapPlaniSatir d in hesap.objeler)
                {
                    satirlar.Add(HesapDugum(d));
                    if (satirlar.Count >= 500) break;
                }
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Arama yapılamadı.");
            }
        }

        /// <summary>Hesap kodu elle yazılınca adı.</summary>
        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            kod = Temiz(kod);
            if (kod == "") return Results.Json(new { ad = "", detay = false });
            try
            {
                int y = HesapYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                HesapPlaniSatir h = new HesapPlaniSatir();
                h.yil = y;
                h.hesapKod = kod;
                ObjectArray hv = Servis().HesapPlaniListele(kullanan, h);
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }
            catch (Exception)
            {
                return Hata("Hesap kodu okunamadı.");
            }
        }

        // ------------------------------------------------------------------ tertip (eski ButceMuhasebe/ListeTertip, seç ve kapat)

        /// <summary>Eski HesapkoddanEkoBul: hesap kodunun ekonomik kodu.</summary>
        private static string HesapkoddanEkoBul(int yil, string hesapKod, IMUHServis servis)
        {
            string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD8/KODYAPI");
            hesapKod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, hesapKod.Replace(".", ""));

            HesapPlaniSatir h = new HesapPlaniSatir();
            h.yil = yil;
            h.hesapKod = hesapKod;
            try
            {
                h = servis.HesapKodOzellikAl(h);
            }
            catch
            {
                return "";
            }
            string ekoYapi = TNS.MUH.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD4/KODYAPI");
            return TNS.MUH.Arac.HesapkoddanEkonomikKodBul(yil, h, ekoYapi);
        }

        /// <summary>
        /// Tertip listesi (eski ListeTertip.aspx, belgeTur=1, secKapat=1). ilk=1: kurumsal kod muhasebe biriminden, ekonomik kod hesap
        /// kodundan bulunur. En az iki kriter dolu olmalıdır. hepsi=1: aradığım tertip listede yok (bütün tertipler).
        /// </summary>
        private static IResult Tertip(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? muhasebe, string birim, string hesapKod,
            string kur, string eko, string peb, string hepsi, string ilk)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IMUHServis servis = Servis();
                int y = yil ?? DateTime.Now.Year;
                if (y > DateTime.Now.Year || y <= 2000) y = DateTime.Now.Year;
                int muh = muhasebe ?? 0;
                birim = Temiz(birim);
                hesapKod = Temiz(hesapKod);
                kur = Temiz(kur);
                eko = Temiz(eko);
                peb = Temiz(peb);

                if (ilk == "1")
                {
                    if (muh > 0 && birim != "")
                    {
                        MuhasebeBirim mb = new MuhasebeBirim();
                        mb.muhasebe = muh;
                        mb.birimKod = birim;
                        ObjectArray mbler = servis.MuhasebeBirimListele(mb);
                        string kurKod = "";
                        if (mbler != null && mbler.objeler != null)
                            foreach (MuhasebeBirim mm in mbler.objeler)
                            {
                                if (kurKod != "") kurKod += ",";
                                kurKod += mm.kurum;
                            }
                        if (kurKod != "") kur = kurKod;
                    }
                    if (hesapKod != "" && eko == "") eko = HesapkoddanEkoBul(y, hesapKod, servis) ?? "";
                }

                kur = kur.Replace(";", ",");
                eko = eko.Replace(";", ",");
                peb = peb.Replace(";", ",");

                int doluAlan = (kur != "" ? 1 : 0) + (eko != "" ? 1 : 0) + (peb != "" ? 1 : 0);
                if (doluAlan < 2)
                    return Results.Json(new
                    {
                        kur = kur, eko = eko, peb = peb, mp = false, satirlar = new List<object>(),
                        uyari = ilk == "1" ? "" : "Lütfen kriter bölümünde, birden fazla alana değer giriniz."
                    });

                bool mpOnayli;
                TNS.BHMv2.OdenekBilgi[] obler = TNS.BHMv2.Arac.TertipListele(kullanan, TNS.BHMv2.Arac.Tanimla(), 1, y, muh, birim, kur, "", "", eko, peb,
                    TNS.MUH.Arac.OdenekKontrolTurBul(y), TNS.MUH.Arac.OdenekUstuKontrolBul(), hepsi == "1", out mpOnayli);

                const int enCok = 300;
                List<object> satirlar = new List<object>();
                foreach (TNS.BHMv2.OdenekBilgi ob in obler)
                {
                    if (satirlar.Count >= enCok) break;
                    string[] kodlar = ob.tertip.TertipStrOl().Split('-');
                    string kurKod = OrtakFonksiyonlar.DiziElemanAl(kodlar, 0, "");
                    string fonKod = OrtakFonksiyonlar.DiziElemanAl(kodlar, 1, "");
                    string finKod = OrtakFonksiyonlar.DiziElemanAl(kodlar, 2, "");
                    string ekoKod = OrtakFonksiyonlar.DiziElemanAl(kodlar, 3, "");
                    string pebKod = OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, "");
                    double odenek = ob.EkRakam(0), harcanan = ob.EkRakam(1), bloke = ob.EkRakam(2);
                    satirlar.Add(new
                    {
                        TERTIP = kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod + (pebKod != "" ? "-" + pebKod : ""),
                        KALAN = odenek - harcanan - bloke,
                        ODENEK = odenek,
                        HARCANAN = harcanan,
                        BLOKE = bloke,
                        MP = mpOnayli ? ob.EkRakam(3) : 0,
                        ACIKLAMA = ob.tertip.adlar.Length > 3 ? ob.tertip.adlar[3] : "",
                        FAALIYETACIKLAMA = ob.tertip.adlar.Length >= 5 ? ob.tertip.adlar[4] : ""
                    });
                }
                string uyari = obler.Length > enCok
                    ? string.Format("Verdiğiniz arama sonucunda {0} adet tertip listelenmiştir. Ekranda bu tertiplerden {1} adedi görüntülenmektedir. Lütfen kıstas alanlarına bilgi girerek gelecek listenin boyunu azaltınız!", obler.Length, enCok)
                    : "";
                return Results.Json(new { kur = kur, eko = eko, peb = peb, mp = mpOnayli, satirlar = satirlar, uyari = uyari });
            }
            catch (Exception)
            {
                return Hata("Tertipler okunamadı.");
            }
        }

        // ------------------------------------------------------------------ açıklama tanımları (eski wndMakbuzAciklama)

        private static IResult AciklamaListe(HttpContext baglam, IWebHostEnvironment ortam, string birim)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            try
            {
                MakbuzAciklama t = new MakbuzAciklama();
                t.birimKod = Temiz(birim);
                ObjectArray bilgiler = Servis().MakbuzAciklamaListele(t);
                if (bilgiler == null || !bilgiler.sonuc.islemSonuc) return Hata(HataMetni(bilgiler?.sonuc.hataStr, "Açıklamalar okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (MakbuzAciklama tt in bilgiler.objeler)
                    satirlar.Add(new { KOD = tt.kod, ACIKLAMA = tt.aciklama, MAKBUZTIPI = tt.makbuzTipiKod });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Açıklamalar okunamadı.");
            }
        }

        private static IResult AciklamaOku(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici _);
            if (ret != null) return ret;
            if ((kod ?? 0) <= 0) return Hata("Açıklama seçilmemiş.");
            try
            {
                MakbuzAciklama t = new MakbuzAciklama();
                t.kod = kod.Value;
                ObjectArray bilgiler = Servis().MakbuzAciklamaListele(t);
                if (bilgiler != null && bilgiler.sonuc.islemSonuc)
                    foreach (MakbuzAciklama tt in bilgiler.objeler)
                        return Results.Json(new { KOD = tt.kod, ACIKLAMA = tt.aciklama, MAKBUZTIPI = tt.makbuzTipiKod });
                return Hata("Açıklama bulunamadı.");
            }
            catch (Exception)
            {
                return Hata("Açıklama okunamadı.");
            }
        }

        private static IResult AciklamaKaydet(HttpContext baglam, IWebHostEnvironment ortam, MakbuzAciklamaIstegi istek)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null) return Basarisiz("Açıklama bilgileri alınamadı.");
            try
            {
                MakbuzAciklama t = new MakbuzAciklama();
                t.kod = istek.Kod;
                t.aciklama = istek.Aciklama ?? "";
                t.birimKod = Temiz(istek.Birim);
                t.makbuzTipiKod = istek.MakbuzTipi;
                Sonuc sonuc = Servis().MakbuzAciklamaKaydet(kullanan, t);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Kayıt işlemi yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Başarıyla kayıt edildi" });
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi yapılamadı.");
            }
        }

        private static IResult AciklamaSil(HttpContext baglam, IWebHostEnvironment ortam, MakbuzAciklamaIstegi istek)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (istek == null || istek.Kod <= 0) return Basarisiz("Silinecek açıklama seçilmemiş.");
            try
            {
                MakbuzAciklama t = new MakbuzAciklama();
                t.kod = istek.Kod;
                Sonuc sonuc = Servis().MakbuzAciklamaSil(kullanan, t);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Silme işlemi yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "Başarıyla silindi" });
            }
            catch (Exception)
            {
                return Basarisiz("Silme işlemi yapılamadı.");
            }
        }
    }
}
