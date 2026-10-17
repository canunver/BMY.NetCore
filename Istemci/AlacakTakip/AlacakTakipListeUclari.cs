using System;
using System.Collections.Generic;
using System.Globalization;
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
using TNS.ATM;
using TNS.KYM;

namespace Istemci.AlacakTakip
{
    /// <summary>Dosya kaydı isteği (eski btnKaydet_Click alanları). Tarihler yyyy-MM-dd.</summary>
    public class AlacakDosyaIstegi
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
        public string saymanlikDosyaNo { get; set; }
        public string muhasebe { get; set; }
        public string birim { get; set; }
        public string kimlikNo { get; set; }
        public string borcluAd { get; set; }
        public string borcluAdres { get; set; }
        public double borc { get; set; }
        public int paraBirimi { get; set; }
        /// <summary>"yıl-kod" (eski ddlFaizTuru değeri); boşsa faiz türü yok.</summary>
        public string faizTur { get; set; }
        public string borcDogusTarihi { get; set; }
        public string faizBaslangicTarihi { get; set; }
        public string borcTespitTarihi { get; set; }
        public string tebligTarihi { get; set; }
        public string borcDegerlendirmeTarihi { get; set; }
        public string taksitBaslangicTarihi { get; set; }
        public int zamanAsimiBaslangicYili { get; set; }
        public string borcSebep { get; set; }
        public string gelenYaziYer { get; set; }
        public string gelenYaziTarih { get; set; }
        public string gelenYaziSayi { get; set; }
        public string saymanligaGidenYaziTarih { get; set; }
        public string saymanligaGidenYaziSayi { get; set; }
        public string kisiyiGidenYaziTarih { get; set; }
        public string kisiyeGidenYaziSayi { get; set; }
    }

    public class AlacakDosyaAnahtar
    {
        public int yil { get; set; }
        public int dosyaNo { get; set; }
    }

    /// <summary>Durum değiştirme isteği (eski btnDurum_Click / btnDosyaKapat_Click): işaretli dosyalar yeni duruma geçer.</summary>
    public class AlacakDurumIstegi
    {
        public int durum { get; set; }
        public string aciklama { get; set; }
        public List<AlacakDosyaAnahtar> dosyalar { get; set; }
    }

    /// <summary>
    /// Takip İşlemleri (eski AlacakTakip/AlacakTakipListe.aspx) ve Dosya Tarihçesi (eski DosyaTarihce.aspx) uçları:
    ///   GET  /api/alacaktakip/takip/tanim                 durumlar, kapatma durumları, faiz türleri, para birimleri, varsayılan muhasebe / birim
    ///   GET  /api/alacaktakip/takip/liste?yil=&amp;durum=&amp;muhasebe=&amp;dosyaNo=&amp;kimlikNo=&amp;borcluAd=&amp;islemYapan=   (eski Listele)
    ///   GET  /api/alacaktakip/takip/oku?yil=&amp;dosyaNo=  dosyanın bilgileri (eski AlanlariDoldur)
    ///   POST /api/alacaktakip/takip/kaydet                dosya kaydı (eski btnKaydet_Click)
    ///   POST /api/alacaktakip/takip/durum                 işaretli dosyaların durumunu değiştirir / dosya kapatır (eski btnDurum_Click, btnDosyaKapat_Click)
    ///   GET  /api/alacaktakip/takip/kimlik?tc=            T.C. kimlik nodan ad soyad ve adres (KPS; eski imgKimlik_Click)
    ///   GET  /api/alacaktakip/takip/firma/ara?muhasebe=&amp;ad=&amp;vergiNo=   kişi / firma arama (eski ListeFirmaKisi penceresi)
    ///   GET  /api/alacaktakip/takip/yazdir?sablon=1|2&amp;(liste ölçütleri)   Excel: 1 Kısa Liste (ATM-DosyaListe.xlt), 2 Uzun Liste (ATM-AlacakTakip.xltx)
    ///   GET  /api/alacaktakip/takip/tarihce?yil=&amp;dosyaNo=   dosyanın durum tarihçesi (eski DosyaTarihce.aspx)
    /// </summary>
    public static class AlacakTakipListeUclari
    {
        public const string SayfaAdresi = "../AlacakTakip/AlacakTakipListe.aspx";
        public const string SayfaAdresiAB = "../AlacakTakip/AlacakTakipListeAB.aspx";
        public const string TarihceAdresi = "../AlacakTakip/DosyaTarihce.aspx";

        /// <summary>Takip ekranından açılan alt sayfaların (Tarihçe, Hesap Hareket, Ödeme, Toplu Ödeme Emri) üst sayfaları.</summary>
        public static readonly string[] UstSayfalar = { SayfaAdresi, SayfaAdresiAB };

        /// <summary>EGO (kurum 460604): borç muhasebeden gelir, borç alanı salt okunur, listede borç ve para birimi sütunları gizli.</summary>
        public const string EgoKurum = "460604";

        public static void Ekle(WebApplication app)
        {
            const string u = "/api/alacaktakip/takip/";
            app.MapGet(u + "tanim", Tanim);
            app.MapGet(u + "liste", Liste);
            app.MapGet(u + "oku", Oku);
            app.MapPost(u + "kaydet", Kaydet);
            app.MapPost(u + "durum", DurumDegistir);
            app.MapGet(u + "kimlik", Kimlik);
            app.MapGet(u + "firma/ara", FirmaAra);
            app.MapGet(u + "yazdir", Yazdir);
            app.MapGet(u + "tarihce", Tarihce);
        }

        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi);
        }

        public static bool AltSayfaYetkili(Kullanici kullanan, string klasor, string altSayfa)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, altSayfa, UstSayfalar);
        }

        // ---------------------------------------------------------------- yardımcılar

        private static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        private static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }
        private static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata }); }

        private static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Replace("<br/>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        private static string Temiz(string s) { return (s ?? "").Trim(); }

        private static IATMServis Servis() { return TNS.ATM.Arac.Tanimla(); }

        public static string KurumKod() { return (TNS.DEG.Arac.VarsayilanKurumBul() ?? "").Replace(".", ""); }

        private static bool Ego() { return KurumKod() == EgoKurum; }

        public static string ParaBirimiStr(int paraBirimi)
        {
            if (paraBirimi == (int)ENUMParaBirimi.TL) return "TL";
            if (paraBirimi == (int)ENUMParaBirimi.DOLAR) return "$";
            if (paraBirimi == (int)ENUMParaBirimi.EURO) return "€";
            if (paraBirimi == (int)ENUMParaBirimi.STERLIN) return "£";
            return "";
        }

        private static string TarihYaz(TNSDateTime t)
        {
            if (TNSDateTime.IsNull(t)) return "";
            DateTime d = t.Oku();
            return d == DateTime.MinValue ? "" : d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static DateTime TarihOku(string s)
        {
            s = Temiz(s);
            if (s == "") return DateTime.MinValue;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) return d;
            if (DateTime.TryParse(s, new CultureInfo("tr-TR"), DateTimeStyles.None, out d)) return d;
            return DateTime.MinValue;
        }

        private static TNSDateTime TnsTarih(string s) { return new TNSDateTime(TarihOku(s)); }

        private static double TlKarsilik(int paraBirimi, double borc)
        {
            if (paraBirimi == (int)ENUMParaBirimi.TL || paraBirimi == 0) return 0;
            try { return Servis().DovizKurGetir((ENUMParaBirimi)paraBirimi, true) * borc; }
            catch (Exception) { return 0; }
        }

        private static Dosya Kriter(int? yil, int? durum, string muhasebe, string dosyaNo, string kimlikNo, string borcluAd, string islemYapan)
        {
            Dosya d = new Dosya();
            d.yil = yil ?? 0;
            d.sonDurum.kod = durum ?? 0;
            d.saymanlikKodu = OrtakFonksiyonlar.ConvertToInt(Temiz(muhasebe), 0);
            d.dosyaNo = OrtakFonksiyonlar.ConvertToInt(Temiz(dosyaNo), 0);
            d.kimlikNo = Temiz(kimlikNo);
            d.borcluAd = Temiz(borcluAd);
            d.islemYapan = Temiz(islemYapan);
            d.muhasebeBorcDurumuAl = TNS.ATM.Arac.MuhasebedenGelsin(KurumKod());
            return d;
        }

        // ---------------------------------------------------------------- tanım

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                IATMServis s = Servis();
                // Sorgudaki durum listesi: önce iki "Hepsi" seçeneği (eski durumStore).
                List<object> durumlar = new List<object>
                {
                    new { kod = 999, ad = "Hepsi (Tamamlanmamışlar)" },
                    new { kod = 0, ad = "Hepsi (İptal Hariç)" }
                };
                ObjectArray bilgi = s.DurumListele(kullanan, true);
                if (bilgi != null && bilgi.sonuc.islemSonuc)
                    foreach (TNS.ATM.Durum d in bilgi.objeler) durumlar.Add(new { kod = d.kod, ad = d.ad ?? "" });

                // Dosya Kapat penceresindeki durumlar (eski ddlDurum2: DurumListele(hepsiMi: false)).
                List<object> kapatDurumlar = new List<object>();
                bilgi = s.DurumListele(kullanan, false);
                if (bilgi != null && bilgi.sonuc.islemSonuc)
                    foreach (TNS.ATM.Durum d in bilgi.objeler) kapatDurumlar.Add(new { kod = d.kod, ad = d.ad ?? "" });

                List<object> faizTurleri = new List<object>();
                bilgi = s.FaizTurListele(kullanan, 0, "");
                if (bilgi != null && bilgi.sonuc.islemSonuc)
                    foreach (TNS.ATM.FaizTur f in bilgi.objeler)
                        faizTurleri.Add(new { kod = f.yil + "-" + f.kod, ad = f.yil + "-" + (f.ad ?? "") });

                var paraBirimleri = new[]
                {
                    new { kod = (int)ENUMParaBirimi.TL, ad = "TL" },
                    new { kod = (int)ENUMParaBirimi.DOLAR, ad = "DOLAR" },
                    new { kod = (int)ENUMParaBirimi.EURO, ad = "EURO" },
                    new { kod = (int)ENUMParaBirimi.STERLIN, ad = "STERLİN" }
                };

                int buYil = DateTime.Today.Year;
                return Results.Json(new
                {
                    aktifYil = buYil,
                    ilkYil = 2006,
                    durumlar = durumlar,
                    kapatDurumlar = kapatDurumlar,
                    faizTurleri = faizTurleri,
                    paraBirimleri = paraBirimleri,
                    muhasebe = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ALACAKTAKIPMUHASEBE") ?? "",
                    birim = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ALACAKTAKIPMUHASEBEBIRIM") ?? "",
                    firmaZorunlu = TNS.DEG.Arac.DegiskenDegerBul(0, "/ALACAKTAKIPFIRMAZORUNLU") == "1",
                    ego = Ego()
                });
            }
            catch (Exception)
            {
                return Hata("Tanımlar okunamadı.");
            }
        }

        // ---------------------------------------------------------------- liste

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? durum, string muhasebe, string dosyaNo,
            string kimlikNo, string borcluAd, string islemYapan)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                ObjectArray bilgi = Servis().DosyaListele(kullanan, Kriter(yil, durum, muhasebe, dosyaNo, kimlikNo, borcluAd, islemYapan));
                if (bilgi == null) return Hata("Liste okunamadı.");
                if (bilgi.sonuc != null && !bilgi.sonuc.islemSonuc && bilgi.objeler.Count == 0)
                    return Hata(HataMetni(bilgi.sonuc.hataStr, "Liste okunamadı."));
                string kurKod = KurumKod();
                List<object> satirlar = new List<object>();
                foreach (Dosya d in bilgi.objeler)
                    satirlar.Add(new
                    {
                        yil = d.yil,
                        dosyaID = d.dosyaNo,
                        dosyaNo = d.yil + "/" + d.dosyaNo.ToString("0000"),
                        borcluAd = d.borcluAd ?? "",
                        borc = TNS.ATM.Arac.BorcBul(d, kurKod),
                        durum = d.sonDurum?.ad ?? "",
                        durumKod = d.sonDurum?.kod ?? 0,
                        paraBirimi = ParaBirimiStr(d.paraBirimi)
                    });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Liste okunamadı.");
            }
        }

        // ---------------------------------------------------------------- oku (eski AlanlariDoldur)

        private static IResult Oku(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? dosyaNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((dosyaNo ?? 0) == 0) return Hata("Dosya seçiniz.");
            try
            {
                Dosya k = new Dosya();
                k.yil = yil ?? 0;
                k.dosyaNo = dosyaNo ?? 0;
                string kurKod = KurumKod();
                k.muhasebeBorcDurumuAl = TNS.ATM.Arac.MuhasebedenGelsin(kurKod);
                ObjectArray bilgi = Servis().DosyaListele(kullanan, k);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc?.hataStr, "Dosya okunamadı."));
                if (bilgi.objeler.Count == 0) return Hata(HataMetni(bilgi.sonuc.bilgiStr, "Dosya bulunamadı."));
                Dosya d = (Dosya)bilgi.objeler[0];
                double borc = TNS.ATM.Arac.BorcBul(d, kurKod);
                return Results.Json(new
                {
                    yil = d.yil,
                    dosyaNo = d.dosyaNo,
                    durumKod = d.sonDurum?.kod ?? 0,
                    durumAd = d.sonDurum?.ad ?? "",
                    saymanlikDosyaNo = d.saymanlikDosyaNo ?? "",
                    muhasebe = d.saymanlikKodu > 0 ? d.saymanlikKodu.ToString() : "",
                    muhasebeAd = d.saymanlikAdi ?? "",
                    birim = d.birimKodu ?? "",
                    birimAd = d.birimAdi ?? "",
                    kimlikNo = d.kimlikNo ?? "",
                    borcluAd = d.borcluAd ?? "",
                    borcluAdres = d.borcluAdres ?? "",
                    borc = borc,
                    paraBirimi = d.paraBirimi,
                    tlKarsilik = TlKarsilik(d.paraBirimi, d.borc),
                    faizTur = d.faizTur != null && d.faizTur.kod > 0 ? d.faizTur.yil + "-" + d.faizTur.kod : "",
                    borcDogusTarihi = TarihYaz(d.borcDogusTarihi),
                    faizBaslangicTarihi = TarihYaz(d.faizBaslangicTarihi),
                    borcTespitTarihi = TarihYaz(d.borcTespitTarihi),
                    tebligTarihi = TarihYaz(d.tebligTarihi),
                    borcDegerlendirmeTarihi = TarihYaz(d.borcDegerlendirmeTarihi),
                    taksitBaslangicTarihi = TarihYaz(d.taksitBaslangicTarihi),
                    zamanAsimiBaslangicYili = d.zamanAsimiBaslangicYili > 0 ? d.zamanAsimiBaslangicYili.ToString() : "",
                    borcSebep = d.borcSebep ?? "",
                    gelenYaziYer = d.gelenYaziYer ?? "",
                    gelenYaziTarih = TarihYaz(d.gelenYaziTarih),
                    gelenYaziSayi = d.gelenYaziSayi ?? "",
                    saymanligaGidenYaziTarih = TarihYaz(d.saymanligaGidenYaziTarih),
                    saymanligaGidenYaziSayi = d.saymanligaGidenYaziSayi ?? "",
                    kisiyiGidenYaziTarih = TarihYaz(d.kisiyiGidenYaziTarih),
                    kisiyeGidenYaziSayi = d.kisiyeGidenYaziSayi ?? "",
                    sonDurumTarihi = TarihYaz(d.sonDurumTarihi),
                    islemYapan = d.islemYapan ?? ""
                });
            }
            catch (Exception)
            {
                return Hata("Dosya okunamadı.");
            }
        }

        // ---------------------------------------------------------------- kaydet

        private static async Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, AlacakDosyaIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            try
            {
                // Eski sayfa her kayıtta seçili muhasebe / birimi kullanıcı değişkeni olarak saklıyordu (Yeni Dosya'da gelir).
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ALACAKTAKIPMUHASEBE", Temiz(i.muhasebe));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ALACAKTAKIPMUHASEBEBIRIM", Temiz(i.birim));

                Dosya a = new Dosya();
                a.yil = i.yil;
                a.dosyaNo = i.dosyaNo;
                a.saymanlikDosyaNo = Temiz(i.saymanlikDosyaNo);
                a.saymanlikKodu = OrtakFonksiyonlar.ConvertToInt(Temiz(i.muhasebe), 0);
                a.birimKodu = Temiz(i.birim);
                a.kimlikNo = Temiz(i.kimlikNo);
                a.borcluTipi = 1; // bu alan kullanılmıyor (eski kod)
                a.borcluAd = Temiz(i.borcluAd);
                a.borcluAdres = Temiz(i.borcluAdres);
                a.borc = i.borc;
                if (Ego()) a.borc = 0.001;
                a.paraBirimi = i.paraBirimi > 0 ? i.paraBirimi : (int)ENUMParaBirimi.TL;
                a.borcSebep = Temiz(i.borcSebep);

                DateTime t;
                if ((t = TarihOku(i.borcTespitTarihi)) != DateTime.MinValue) a.borcTespitTarihi = new TNSDateTime(t);
                if ((t = TarihOku(i.borcDogusTarihi)) != DateTime.MinValue) a.borcDogusTarihi = new TNSDateTime(t);
                if ((t = TarihOku(i.faizBaslangicTarihi)) != DateTime.MinValue) a.faizBaslangicTarihi = new TNSDateTime(t);
                if ((t = TarihOku(i.taksitBaslangicTarihi)) != DateTime.MinValue) a.taksitBaslangicTarihi = new TNSDateTime(t);
                if ((t = TarihOku(i.tebligTarihi)) != DateTime.MinValue) a.tebligTarihi = new TNSDateTime(t);
                if ((t = TarihOku(i.borcDegerlendirmeTarihi)) != DateTime.MinValue) a.borcDegerlendirmeTarihi = new TNSDateTime(t);

                string yilS = "", kodS = "";
                TNS.OrtakFonksiyonlar.StringiIkiyeBol(Temiz(i.faizTur), "-", ref yilS, ref kodS);
                a.faizTur.yil = OrtakFonksiyonlar.ConvertToInt(yilS, 0);
                a.faizTur.kod = OrtakFonksiyonlar.ConvertToInt(kodS, 0);

                a.zamanAsimiBaslangicYili = i.zamanAsimiBaslangicYili;
                a.sonDurumTarihi = new TNSDateTime(DateTime.Today);

                a.gelenYaziYer = i.gelenYaziYer ?? "";
                a.gelenYaziTarih = TnsTarih(i.gelenYaziTarih);
                a.gelenYaziSayi = i.gelenYaziSayi ?? "";
                a.saymanligaGidenYaziTarih = TnsTarih(i.saymanligaGidenYaziTarih);
                a.saymanligaGidenYaziSayi = i.saymanligaGidenYaziSayi ?? "";
                a.kisiyiGidenYaziTarih = TnsTarih(i.kisiyiGidenYaziTarih);
                a.kisiyeGidenYaziSayi = i.kisiyeGidenYaziSayi ?? "";

                Sonuc sonuc = Servis().DosyaKaydet(kullanan, a);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Kayıt işlemi başarısız."));
                int dosyaNo = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, a.dosyaNo);
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Kayıt işlemi başarıyla tamamlandı.",
                    yil = a.yil,
                    dosyaNo = dosyaNo,
                    tlKarsilik = TlKarsilik(a.paraBirimi, a.borc)
                });
            }
            catch (Exception)
            {
                return Basarisiz("Kayıt işlemi başarısız.");
            }
        }

        // ---------------------------------------------------------------- durum değiştir / dosya kapat

        private static async Task<IResult> DurumDegistir(HttpContext baglam, IWebHostEnvironment ortam, AlacakDurumIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            TNSCollection tc = new TNSCollection();
            foreach (AlacakDosyaAnahtar k in i.dosyalar ?? new List<AlacakDosyaAnahtar>())
            {
                if (k == null || k.dosyaNo <= 0) continue;
                DosyaDurumTarihce d = new DosyaDurumTarihce();
                d.yil = k.yil;
                d.dosyaNo = k.dosyaNo;
                d.durum.kod = i.durum;
                d.tarih = new TNSDateTime(DateTime.Today);
                d.aciklama = Temiz(i.aciklama);
                d.kullaniciKodu = kullanan.kullaniciKodu;
                tc.Add(d);
            }
            if (tc.Count == 0) return Basarisiz("İşlem yapılacak herhangi bir dosya seçilmemiş!");
            try
            {
                Sonuc sonuc = Servis().DosyaDurumTarihceKaydet(kullanan, tc);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "İşlem yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "İşlem başarıyla tamamlandı." });
            }
            catch (Exception)
            {
                return Basarisiz("İşlem yapılamadı.");
            }
        }

        // ---------------------------------------------------------------- kimlik (KPS)

        private static IResult Kimlik(HttpContext baglam, IWebHostEnvironment ortam, string tc)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            tc = Temiz(tc);
            if (tc == "") return Hata("T.C. kimlik numarasını yazınız.");
            const string hata = "Kimlik Doğrulama Servisi ile bağlantı kurulamıyor veya verilen kimlik numarası hatalı.";
            try
            {
                TNS.KPM.Istek istek = new TNS.KPM.Istek();
                istek.istekTur = (int)TNS.KPM.ENUMIstekTur.SADECEADRES;
                istek.tcKimlikNo = tc;
                istek.islemYapan = kullanan.kullaniciKodu;
                istek.islemTarih = new TNSDateTime(DateTime.Now);
                ObjectArray liste = TNS.KPM.Arac.Tanimla().IkametgahRaporla(kullanan, istek);
                TNS.KPM.Kisi k = liste != null && liste.ObjeSayisi > 0 ? (TNS.KPM.Kisi)liste.objeler[0] : new TNS.KPM.Kisi();

                string ad = ((k.ad ?? "") + " " + (k.soyad ?? "")).Trim();
                string adres = "";
                if (k.adres != null)
                {
                    if (!string.IsNullOrEmpty(k.adres.mahalle)) adres += k.adres.mahalle + " ";
                    if (!string.IsNullOrEmpty(k.adres.sokak)) adres += k.adres.sokak + " ";
                    if (!string.IsNullOrEmpty(k.adres.numara)) adres += "No : " + k.adres.numara + " ";
                    if (!string.IsNullOrEmpty(k.adres.koy)) adres += k.adres.koy + " / ";
                    if (!string.IsNullOrEmpty(k.adres.bucak)) adres += k.adres.bucak + " / ";
                    if (!string.IsNullOrEmpty(k.adres.ilce)) adres += k.adres.ilce + " / ";
                    if (!string.IsNullOrEmpty(k.adres.il)) adres += k.adres.il;
                }
                if (ad == "") return Hata(hata);
                return Results.Json(new { ad = ad, adres = adres });
            }
            catch (Exception)
            {
                return Hata(hata);
            }
        }

        // ---------------------------------------------------------------- kişi / firma arama

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

        // ---------------------------------------------------------------- yazdır (Excel)

        public static string SablonBul(string uygulamaKlasoru, string sablonAd)
        {
            string kurumYol = Ayar.Oku("RaporSablonYol");
            if (kurumYol != "")
            {
                string s = Path.Combine(kurumYol, "ATM", sablonAd);
                if (File.Exists(s)) return s;
            }
            string u = Path.Combine(uygulamaKlasoru, "RaporSablon", "ATM", sablonAd);
            return File.Exists(u) ? u : null;
        }

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? sablon, int? yil, int? durum, string muhasebe,
            string dosyaNo, string kimlikNo, string borcluAd, string islemYapan)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            bool uzun = sablon == 2;
            string sablonAd = uzun ? "ATM-AlacakTakip.xltx" : "ATM-DosyaListe.xlt";
            string sablonYol = SablonBul(ortam.ContentRootPath, sablonAd);
            if (sablonYol == null) return Hata("Rapor şablonu bulunamadı (ATM\\" + sablonAd + ").");
            try
            {
                ObjectArray bilgi = Servis().DosyaListele(kullanan, Kriter(yil, durum, muhasebe, dosyaNo, kimlikNo, borcluAd, islemYapan));
                if (bilgi == null || bilgi.objeler.Count == 0) return Hata("Verilen kriterlere uygun bilgi bulunamadı.");
                AsposeLisans.Yukle();
                string dosya = uzun ? AlacakTakipListeRaporu.UzunListe(bilgi, sablonYol, KurumKod())
                                    : AlacakTakipListeRaporu.KisaListe(bilgi, sablonYol, KurumKod());
                return DosyaIslem.DosyaGonder(dosya, "AlacakTakip" + DosyaIslem.DosyaAdUretSade(), true, GenelIslemler.ExcelTur());
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        // ---------------------------------------------------------------- tarihçe (eski DosyaTarihce.aspx)

        private static IResult Tarihce(HttpContext baglam, IWebHostEnvironment ortam, int? yil, int? dosyaNo)
        {
            Kullanici kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!AltSayfaYetkili(kullanan, ortam.ContentRootPath, TarihceAdresi)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            try
            {
                ObjectArray bilgi = Servis().DosyaDurumTarihceListele(kullanan, yil ?? 0, dosyaNo ?? 0);
                if (bilgi == null || !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi?.sonuc?.hataStr, "Tarihçe okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (DosyaDurumTarihce t in bilgi.objeler)
                    satirlar.Add(new
                    {
                        siraNo = t.siraNo,
                        durum = t.durum?.ad ?? "",
                        tarih = TarihYaz(t.tarih),
                        kullaniciKodu = t.kullaniciKodu ?? "",
                        aciklama = t.aciklama ?? ""
                    });
                return Results.Json(new { satirlar = satirlar, bilgi = satirlar.Count == 0 ? HataMetni(bilgi.sonuc.bilgiStr, "Kayıt bulunamadı.") : "" });
            }
            catch (Exception)
            {
                return Hata("Tarihçe okunamadı.");
            }
        }
    }
}
