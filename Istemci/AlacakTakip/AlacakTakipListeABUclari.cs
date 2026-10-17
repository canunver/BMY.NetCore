using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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
using TNS.Raporlama;

namespace Istemci.AlacakTakip
{
    /// <summary>
    /// Liste ölçütleri (eski PropertyGrid1). Tarihler yyyy-MM-dd.
    ///   durum, borcTuru: virgülle ayrılmış kodlar (eski çoklu seçim). borcDurumu: 0 Hepsi, 1 Borçlular, 2 Borçlu (G.A. Var),
    ///   3 Borçlu (G.A. Yok), 4 Kapatmışlar. tarihTur: "" Tüm İşlemler, "geriAlim" Geri Alım, "silme" Silme, "onBorc" Ön Borç,
    ///   "geriAlimK" Geri Alım K. islemTarihi / onayTarihi: araç çubuğundaki "İşl. Tarih" / "Onay Tarih".
    /// </summary>
    public class ABListeKriteri
    {
        public int no { get; set; }
        public int ipard { get; set; }
        public string durum { get; set; }
        public string muhasebe { get; set; }
        public string birim { get; set; }
        public string aramaMetni { get; set; }
        public string borcTuru { get; set; }
        public int borcDurumu { get; set; }
        public bool avans { get; set; }
        public string tarihTur { get; set; }
        public string islTarih { get; set; }
        public string islTarihSon { get; set; }
        public string islemTarihi { get; set; }
        public string onayTarihi { get; set; }
        public bool bekleyen { get; set; }
    }

    /// <summary>Dönem faizi hesabı (eski wndHesaplat / btnHesaplat_Click). hTur: 1 Üret, 2 Onayla, 3 İptal Et.</summary>
    public class ABDonemFaiziIstegi
    {
        public int iPARDDonemi { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int hTur { get; set; }
    }

    /// <summary>İmza bilgileri (eski wndImzalar). Kullanıcı değişkenlerinde saklanır.</summary>
    public class ABImzaIstegi
    {
        public string hazirlayan { get; set; }
        public string hazirlayanUnv { get; set; }
        public string kontrolEden { get; set; }
        public string kontrolEdenUnv { get; set; }
        public string gozdenGeciren { get; set; }
        public string gozdenGecirenUnv { get; set; }
        public string onaylayan { get; set; }
        public string onaylayanUnv { get; set; }
        public string uygunBulan { get; set; }
        public string uygunBulanUnv { get; set; }
    }

    /// <summary>
    /// AB (IPARD) Takip İşlemleri (eski AlacakTakip/AlacakTakipListeAB.aspx) liste, dosya ve rapor uçları:
    ///   GET  /api/alacaktakip/ab/tanim                       IPARD dönemleri, süreç durumları, borç türleri, varsayılan muhasebe / birim, dönem faizi yıl / ay
    ///   GET  /api/alacaktakip/ab/liste?(ölçütler)            dosya listesi (eski btnListele_Click / btnListeleBek_Click: bekleyen=true)
    ///   GET  /api/alacaktakip/ab/dosya?iPARDDonemi=&amp;dosyaAnahtar=&amp;islemTarihi=&amp;onayTarihi=   dosyanın bütün bilgileri (eski SatirSecildi / EkranaKayitYaz)
    ///   GET  /api/alacaktakip/ab/yazdir?no=&amp;(ölçütler)   Liste Yazdır / Mutabakat raporları (eski btnYazdir_Click; rapor grubu 11000)
    ///   POST /api/alacaktakip/ab/sakla                        Borçlu defterini üretip Saklı Raporlar'a koyar (eski btnSakla_Click)
    ///   GET  /api/alacaktakip/ab/rapor?no=&amp;tur=&amp;pb=&amp;iPARDDonemi=&amp;dosyaAnahtar=&amp;islemTarihi=&amp;onayTarihi=   dosya raporları (eski btnRaporlar_Click)
    ///   GET  /api/alacaktakip/ab/rapor/odeme?no=&amp;tur=&amp;iPARDDonemi=&amp;dosyaAnahtar=&amp;tarih=   işlem penceresindeki 6307 raporu (eski btnRaporlar2_Click)
    ///   GET  /api/alacaktakip/ab/rapor/geribildirim?...       Geri Bildirim Yazdır (eski btnGeriBildirimYazdir_Click; rapor 2)
    ///   GET  /api/alacaktakip/ab/rapor/taksit?kayitli=&amp;... Taksit raporu (eski btnTaksitYazdir_Click; rapor 17)
    ///   POST /api/alacaktakip/ab/donemfaizi                   Dönem faizi Üret / Onayla / İptal Et (eski btnHesaplat_Click)
    ///   GET  /api/alacaktakip/ab/imza, POST /api/alacaktakip/ab/imza   İmzalar penceresi (eski wndImzalar_Show / btnImzaKaydet_Click)
    ///   GET  /api/alacaktakip/ab/firma/ara?muhasebe=&amp;ad=&amp;vergiNo=   kişi / firma arama (eski ListeFirmaKisi penceresi; kod da döner)
    ///   GET  /api/alacaktakip/ab/kur?tarih=                   tarihten bir önceki ayın son iş günü EUR kuru (eski DovizKuruBul)
    /// İşlem pencerelerinin uçları AlacakTakipABIslemUclari'ndadır. Sayfa durumsuzdur: dosya her istekte ListeleDosyaAB ile yeniden okunur
    /// (eski Session["DosyaAB"] yok).
    /// </summary>
    public static class AlacakTakipListeABUclari
    {
        public const string SayfaAdresi = "../AlacakTakip/AlacakTakipListeAB.aspx";
        public const int RaporGrup = 11000;
        internal const string U = "/api/alacaktakip/ab/";

        public static void Ekle(WebApplication app)
        {
            app.MapGet(U + "tanim", Tanim);
            app.MapGet(U + "liste", Liste);
            app.MapGet(U + "dosya", DosyaGetir);
            app.MapGet(U + "yazdir", Yazdir);
            app.MapPost(U + "sakla", Sakla);
            app.MapGet(U + "rapor", DosyaRaporu);
            app.MapGet(U + "rapor/odeme", RaporOdeme);
            app.MapGet(U + "rapor/geribildirim", RaporGeriBildirim);
            app.MapGet(U + "rapor/taksit", RaporTaksit);
            app.MapPost(U + "donemfaizi", DonemFaizi);
            app.MapGet(U + "imza", ImzaOku);
            app.MapPost(U + "imza", ImzaKaydet);
            app.MapGet(U + "firma/ara", FirmaAra);
            app.MapGet(U + "kur", Kur);
        }

        /// <summary>Menü yetkisi ve ATM sayfaya giriş yetkisi (eski Page_Load: TNS.ATM.Yetki.SayfayaGirisYapabilirMi).</summary>
        public static bool Yetkili(Kullanici kullanan, string klasor)
        {
            if (kullanan == null) return false;
            if (!SayfaYetki.Yetkili(kullanan, klasor, SayfaAdresi)) return false;
            try
            {
                return TNS.ATM.Yetki.SayfayaGirisYapabilirMi(kullanan);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ---------------------------------------------------------------- ortak yardımcılar (AlacakTakipABIslemUclari de kullanır)

        internal static IResult Denetle(HttpContext baglam, IWebHostEnvironment ortam, out Kullanici kullanan)
        {
            kullanan = OturumBilgisi.KullaniciOku(baglam);
            if (kullanan == null) return Results.Unauthorized();
            if (!Yetkili(kullanan, ortam.ContentRootPath)) return Results.StatusCode(StatusCodes.Status403Forbidden);
            return null;
        }

        internal static IResult Hata(string hata) { return Results.Json(new { hata = hata }); }
        internal static IResult Basarisiz(string hata) { return Results.Json(new { basarili = false, hata = hata }); }

        internal static string HataMetni(string hata, string varsayilan)
        {
            hata = (hata ?? "").Replace("<br>", " ").Replace("<br/>", " ").Trim();
            if (hata == "" || hata.Contains("Exception") || hata.Contains("   at ")) return varsayilan;
            return hata;
        }

        internal static string Temiz(string s) { return (s ?? "").Trim(); }

        internal static IATMServis Servis() { return TNS.ATM.Arac.Tanimla(); }

        internal static bool SistemYoneticisi(Kullanici kullanan)
        {
            try { return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI); }
            catch (Exception) { return false; }
        }

        /// <summary>JSON NaN / sonsuz yazamaz (ör. kur 0 iken EUR karşılığı): 0 yazılır.</summary>
        internal static double N(double d) { return double.IsNaN(d) || double.IsInfinity(d) ? 0 : d; }

        /// <summary>Hesaplanan özelliği okur; hata verirse varsayılan döner (ör. anaDosya olmayan satırda turAd).</summary>
        internal static T G<T>(Func<T> f, T varsayilan)
        {
            try { return f(); }
            catch (Exception) { return varsayilan; }
        }

        internal static string TarihYaz(TNSDateTime t)
        {
            try
            {
                if (t == null || TNSDateTime.IsNull(t)) return "";
                return TarihYaz(t.Oku());
            }
            catch (Exception)
            {
                return "";
            }
        }

        internal static string TarihYaz(DateTime d)
        {
            return d.Year < 1900 ? "" : d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        internal static DateTime TarihOku(string s)
        {
            s = Temiz(s);
            if (s == "") return DateTime.MinValue;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) return d;
            if (DateTime.TryParse(s, new CultureInfo("tr-TR"), DateTimeStyles.None, out d)) return d;
            return DateTime.MinValue;
        }

        internal static TNSDateTime TnsTarih(string s) { return new TNSDateTime(TarihOku(s)); }

        internal static DateTime Dt(TNSDateTime t)
        {
            try { return t == null || TNSDateTime.IsNull(t) ? DateTime.MinValue : t.Oku(); }
            catch (Exception) { return DateTime.MinValue; }
        }

        /// <summary>Eski IslemTarihiBul / OnayTarihiBul: boşsa ya da bugünse şimdiki an.</summary>
        internal static TNSDateTime IslemTarihiBul(string s)
        {
            DateTime d = TarihOku(s);
            if (d == DateTime.MinValue || d.Date == DateTime.Now.Date) return new TNSDateTime(DateTime.Now);
            return new TNSDateTime(d);
        }

        /// <summary>
        /// Eski SessiondanAl(true): dosya yalnızca anahtarlarıyla yeniden okunur. Bulunamazsa (eskisi gibi) anahtarlı boş dosya döner;
        /// servis hata verirse null ve hata.
        /// </summary>
        internal static DosyaAB DosyaOku(Kullanici kullanan, int iPARDDonemi, int dosyaAnahtar, out string hata)
        {
            hata = null;
            DosyaAB form = new DosyaAB();
            form.iPARDDonemi = iPARDDonemi;
            form.dosyaAnahtar = dosyaAnahtar;
            form.borcHareket = new TNSCollection();
            if (form.iPARDDonemi > 0 && form.dosyaAnahtar > 0)
            {
                ObjectArray bilgi = Servis().ListeleDosyaAB(kullanan, form);
                if (bilgi == null) { hata = "Dosya okunamadı."; return null; }
                if (bilgi.objeler.Count >= 1) return (DosyaAB)bilgi.objeler[0];
                if (bilgi.sonuc != null && !bilgi.sonuc.islemSonuc) { hata = HataMetni(bilgi.sonuc.hataStr, "Dosya okunamadı."); return null; }
            }
            return form;
        }

        /// <summary>Dosya parçası (borçlu, ön borç, geri alım kararı, durum, taksit) satırı: işlem tabloları ve İşlem Listesi.</summary>
        internal static Dictionary<string, object> Parca(DosyaABParca p)
        {
            Dictionary<string, object> s = new Dictionary<string, object>
            {
                ["ipardDonemi"] = p.ipardDonemi,
                ["dosyaAnahtar"] = p.dosyaAnahtar,
                ["sira"] = p.sira,
                ["tabloKod"] = (int)p.tabloKod,
                ["tabloKodAd"] = G(() => p.tabloKodAd, ""),
                ["basTarih"] = TarihYaz(p.basTarih),
                ["kayitDurum"] = (int)p.kayitDurum,
                ["kayitDurumAd"] = G(() => p.kayitDurumAd, ""),
                ["gerekce"] = p.gerekce ?? "",
                ["kaydedenAdSoyad"] = G(() => p.kaydedenAdSoyad, "").Trim(),
                ["kayitTarihi"] = TarihYaz(p.kayitTarihi),
                ["onaylayanAdSoyad"] = G(() => p.onaylayanAdSoyad, "").Trim(),
                ["onayTarihi"] = TarihYaz(p.onayTarihi)
            };
            if (p is ABBorclu b)
            {
                s["defterSirasi"] = b.defterSirasi ?? "";
                s["birimAd"] = b.birimAd ?? "";
                s["borcluKodAd"] = G(() => b.borcluKodAd, "");
            }
            else if (p is OnBorc o)
            {
                s["borcTuruAd"] = G(() => o.borcTuruAd, "");
                s["onBorcTarihi"] = TarihYaz(o.onBorcTarihi);
                s["onBorcToplam"] = N(G(() => o.onBorcToplam, 0d));
            }
            else if (p is GeriAlimKarari g)
            {
                s["geriAlimKarariTarihi"] = TarihYaz(g.geriAlimKarariTarihi);
                s["temerrutTarihi"] = TarihYaz(g.temerrutTarihi);
                s["gakToplam"] = N(G(() => g.gakToplam, 0d));
            }
            else if (p is DosyaABDurum d)
            {
                s["surecAsamasiAd"] = G(() => d.surecAsamasiAd, "");
            }
            else if (p is DosyaABTaksit t)
            {
                s["taksitTalepTarihi"] = TarihYaz(t.taksitTalepTarihi);
                s["taksitSayisi"] = t.taksitSayisi;
            }
            return s;
        }

        /// <summary>Borç hareketi satırı (eski stoBorcHareket ve stoTaksitler alanları).</summary>
        internal static Dictionary<string, object> Hareket(BorcHareket h)
        {
            return new Dictionary<string, object>
            {
                ["siraNo"] = h.siraNo,
                ["ilgiliIslemSiraNo"] = h.ilgiliIslemSiraNo,
                ["tur"] = (int)h.tur,
                ["turAd"] = G(() => h.turAd, G(() => BorcHareket.TurAdBul(h.tur), "")),
                ["tarih"] = TarihYaz(h.tarih),
                ["karar"] = h.karar ?? "",
                ["miktarAB"] = N(h.miktarAB),
                ["miktarUlusal"] = N(h.miktarUlusal),
                ["miktarToplam"] = N(G(() => h.miktarToplam, 0d)),
                ["faizAB"] = N(h.faizAB),
                ["faizUlusal"] = N(h.faizUlusal),
                ["toplamAB"] = N(G(() => h.toplamAB, 0d)),
                ["toplamUlusal"] = N(G(() => h.toplamUlusal, 0d)),
                ["toplamToplam"] = N(G(() => h.toplamToplam, 0d)),
                ["dovizKuru"] = N(h.dovizKuru),
                ["miktarABEUR"] = N(G(() => h.miktarABEUR, 0d)),
                ["miktarUlusalEUR"] = N(G(() => h.miktarUlusalEUR, 0d)),
                ["faizABEUR"] = N(G(() => h.faizABEUR, 0d)),
                ["faizUlusalEUR"] = N(G(() => h.faizUlusalEUR, 0d)),
                ["toplamToplamEUR"] = N(G(() => h.toplamToplamEUR, 0d)),
                ["kayitDurum"] = (int)h.kayitDurum,
                ["kayitDurumAd"] = G(() => h.kayitDurumAd, ""),
                ["kayitTarihi"] = TarihYaz(h.kayitTarihi),
                ["kaydedenAdSoyad"] = G(() => h.kaydedenAdSoyad, "").Trim(),
                ["onayTarihi"] = TarihYaz(h.onayTarihi),
                ["onaylayanAdSoyad"] = G(() => h.onaylayanAdSoyad, "").Trim(),
                // Taksit Bilgileri tablosu (eski stoTaksitler)
                ["duzeltimTarihi"] = TarihYaz(h.duzeltimTarihi),
                ["taksitGun"] = G(() => h.taksitGun, 0),
                ["gecikmeZammi"] = N(G(() => h.gecikmeZammi, 0d)),
                ["tecilFaizi"] = N(G(() => h.tecilFaizi, 0d))
            };
        }

        internal static List<object> Satirlar(TNSCollection tc, Func<object, object> donustur)
        {
            List<object> l = new List<object>();
            if (tc == null) return l;
            foreach (object o in tc) l.Add(donustur(o));
            return l;
        }

        internal static List<object> Hareketler(TNSCollection tc, bool yalnizTaksit)
        {
            List<object> l = new List<object>();
            if (tc == null) return l;
            foreach (BorcHareket h in tc)
            {
                bool taksit = h.tur == BorcHareket.ENUMHareketTur.Taksit || h.tur == BorcHareket.ENUMHareketTur.TaksitOdeme;
                if (yalnizTaksit && !taksit) continue;
                l.Add(Hareket(h));
            }
            return l;
        }

        /// <summary>
        /// Rapor grubu 11000'den raporu üretip indirilecek dosya olarak döndürür. Rapor "HATA..." / "BİLGİ..." iletisi verirse {hata}.
        /// </summary>
        internal static IResult RaporGonder(Kullanici kullanan, int rapNo, Hashtable ht)
        {
            try
            {
                AsposeLisans.Yukle();
                string dosya = RaporlamaServis.TekRaporAl(RaporGrup, rapNo, kullanan, Servis(), ht, null);
                if (string.IsNullOrEmpty(dosya)) return Hata("Rapor hazırlanamadı.");
                if (RaporlamaServis.MesajMi(dosya)) return Hata(HataMetni(dosya, "Rapor hazırlanamadı."));
                if (!File.Exists(dosya)) return Hata("Rapor hazırlanamadı.");
                string uzanti = Path.GetExtension(dosya).TrimStart('.').ToLowerInvariant();
                return DosyaIslem.DosyaGonder(dosya, RaporDosyaAd(rapNo) + "_" + DosyaIslem.DosyaAdUretSade(), true, uzanti);
            }
            catch (RaporException ex)
            {
                return Hata(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Hata("Rapor hazırlanamadı.");
            }
        }

        private static string RaporDosyaAd(int rapNo)
        {
            string ad = G(() => RaporlamaServis.RaporAd(RaporGrup, rapNo), "") ?? "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in ad.Trim())
                sb.Append(Array.IndexOf(Path.GetInvalidFileNameChars(), c) >= 0 || c == '/' || c == '\\' ? '_' : c);
            string s = sb.ToString().Trim();
            return s == "" ? "ATMAB_" + rapNo : s;
        }

        // ---------------------------------------------------------------- tanım (eski Page_Load)

        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string aktifSay = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ALACAKTAKIPMUHASEBE") ?? "";
                string aktifBir = GenelIslemler.KullaniciDegiskenGetir(kullanan.kullaniciKodu, "ALACAKTAKIPMUHASEBEBIRIM") ?? "";
                if (!SistemYoneticisi(kullanan))
                {
                    if (string.IsNullOrWhiteSpace(aktifSay) || string.IsNullOrWhiteSpace(aktifBir))
                    {
                        string tut = G(() => (string)kullanan.yetkiTipiDegerleri["Alacak Takibi Yapacağı Saymanlık-Birim"], null);
                        if (!string.IsNullOrWhiteSpace(tut))
                        {
                            string[] saymanlikBirim = tut.Split(';')[0].Split('-');
                            aktifSay = saymanlikBirim.Length > 0 ? saymanlikBirim[0].Trim() : "";
                            if (aktifSay == "*") aktifSay = "";
                            aktifBir = saymanlikBirim.Length > 1 ? saymanlikBirim[1].Trim() : "";
                            if (aktifBir == "*") aktifBir = "";
                        }
                    }
                }
                else
                {
                    aktifSay = "";
                    aktifBir = "";
                }

                // Dönem faizi penceresi: bir önceki çeyreğin son ayı (eski sfHesaplaYil / cbHesaplaAy)
                int aYil = DateTime.Now.Year;
                int aDonem = (DateTime.Now.Month - 1) / 3;
                if (aDonem > 3) aDonem = 3;
                aDonem--;
                if (aDonem < 0)
                {
                    aDonem = 3;
                    aYil--;
                }

                return Results.Json(new
                {
                    ipardlar = Satirlar(DosyaAB.IPARDAsamaListesiBul(true), o => KodAd(o)),
                    durumlar = Satirlar(DosyaAB.DosyaDurumListesiBul(false), o => KodAd(o)),
                    borcTurleri = Satirlar(DosyaAB.ListeleBorcTurleri(), o => KodAd(o)),
                    fazlaOdeme = (int)DosyaAB.BorcTurleri.FazlaOdeme,
                    muhasebe = aktifSay,
                    birim = aktifBir,
                    hesaplaYil = aYil,
                    hesaplaAy = (aDonem + 1) * 3,
                    hesaplaYilEnAz = DateTime.Now.Year - 1,
                    hesaplaYilEnCok = DateTime.Now.Year
                });
            }
            catch (Exception)
            {
                return Hata("Tanımlar okunamadı.");
            }
        }

        private static object KodAd(object o)
        {
            IntKodStrAd k = (IntKodStrAd)o;
            return new { kod = k.kod, ad = k.ad ?? "" };
        }

        // ---------------------------------------------------------------- liste (eski KriterTopla / Listele)

        private static string TarihAralikOl(ABListeKriteri k)
        {
            DateTime b = TarihOku(k.islTarih), s = TarihOku(k.islTarihSon);
            string tut = (b == DateTime.MinValue ? "" : new TNSDateTime(b).ToString()) + "-" + (s == DateTime.MinValue ? "" : new TNSDateTime(s).ToString());
            return tut == "-" ? null : tut;
        }

        internal static DosyaAB KriterTopla(ABListeKriteri k)
        {
            DosyaAB a = new DosyaAB();
            a.iPARDDonemi = k.ipard;
            a.geriAlimSurecAsamalari = Temiz(k.durum);
            a.saymanlikKod = OrtakFonksiyonlar.ConvertToInt(Temiz(k.muhasebe), 0);
            a.birimKod = Temiz(k.birim);
            a.aramaMetni = Temiz(k.aramaMetni);
            a.borcTurleri = Temiz(k.borcTuru);
            a.borcDurumu = k.borcDurumu >= 1 && k.borcDurumu <= 4 ? k.borcDurumu : 0;
            a.avans = k.avans;

            string tarihTur = Temiz(k.tarihTur);
            if (tarihTur == "onBorc")
                a.onBorcTarihAralik = TarihAralikOl(k);
            else if (tarihTur == "geriAlimK")
                a.geriAlimKarariTarihAralik = TarihAralikOl(k);
            else
            {
                a.islemTarihAralik = TarihAralikOl(k);
                if (tarihTur == "geriAlim")
                    a.islemliste = ((int)BorcHareket.ENUMHareketTur.Odeme) + "," + ((int)BorcHareket.ENUMHareketTur.Mahsup) + "," +
                                   ((int)BorcHareket.ENUMHareketTur.TaksitOdeme) + "," + ((int)BorcHareket.ENUMHareketTur.Terkin) + "," +
                                   ((int)BorcHareket.ENUMHareketTur.Iade) + "," + ((int)BorcHareket.ENUMHareketTur.TaksitOdeme);
                else if (tarihTur == "silme")
                    a.islemliste = ((int)BorcHareket.ENUMHareketTur.Silme).ToString();
            }
            a.islemTarihi = IslemTarihiBul(k.islemTarihi);
            a.onayTarihi = IslemTarihiBul(k.onayTarihi);
            return a;
        }

        private static ABListeKriteri Kriter(int? no, int? ipard, string durum, string muhasebe, string birim, string aramaMetni, string borcTuru,
            int? borcDurumu, bool? avans, string tarihTur, string islTarih, string islTarihSon, string islemTarihi, string onayTarihi, bool? bekleyen)
        {
            return new ABListeKriteri
            {
                no = no ?? 0,
                ipard = ipard ?? 0,
                durum = durum,
                muhasebe = muhasebe,
                birim = birim,
                aramaMetni = aramaMetni,
                borcTuru = borcTuru,
                borcDurumu = borcDurumu ?? 0,
                avans = avans ?? false,
                tarihTur = tarihTur,
                islTarih = islTarih,
                islTarihSon = islTarihSon,
                islemTarihi = islemTarihi,
                onayTarihi = onayTarihi,
                bekleyen = bekleyen ?? false
            };
        }

        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam, int? ipard, string durum, string muhasebe, string birim,
            string aramaMetni, string borcTuru, int? borcDurumu, bool? avans, string tarihTur, string islTarih, string islTarihSon,
            string islemTarihi, string onayTarihi, bool? bekleyen)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                DosyaAB a = KriterTopla(Kriter(0, ipard, durum, muhasebe, birim, aramaMetni, borcTuru, borcDurumu, avans, tarihTur, islTarih,
                    islTarihSon, islemTarihi, onayTarihi, bekleyen));
                a.bekleyenIslemler = bekleyen ?? false;
                ObjectArray bilgi = Servis().ListeleDosyaAB(kullanan, a);
                if (bilgi == null) return Hata("Liste okunamadı.");
                List<object> satirlar = new List<object>();
                if (bilgi.objeler.Count == 0)
                {
                    if (bilgi.sonuc != null && !bilgi.sonuc.islemSonuc)
                    {
                        string h = HataMetni(bilgi.sonuc.hataStr, "");
                        return Hata(h == "" ? "Listeleme sırasında bir hata oluştu." : "Listeleme sırasında bir hata oluştu: " + h);
                    }
                    return Results.Json(new { satirlar = satirlar, bilgi = "Verilen kriterlere uygun bilgi bulunamadı." });
                }

                // eski DegiskenSakla: sistem yöneticisi dışındakilerin muhasebe / birimi saklanır
                if (!SistemYoneticisi(kullanan))
                {
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "ALACAKTAKIPMUHASEBE", a.saymanlikKod.ToString());
                    GenelIslemler.KullaniciDegiskenSakla(kullanan, "ALACAKTAKIPMUHASEBEBIRIM", a.birimKod);
                }

                foreach (DosyaAB d in bilgi.objeler)
                    satirlar.Add(new
                    {
                        iPARDDonemi = d.iPARDDonemi,
                        dosyaAnahtar = d.dosyaAnahtar,
                        defterSirasi = d.defterSirasi ?? "",
                        HrcBirimAd = G(() => d.HrcBirimAd, "") ?? "",
                        borcluKodAd = G(() => d.borcluKodAd, ""),
                        geriAlimSurecAsamasiAd = G(() => d.geriAlimSurecAsamasiAd, ""),
                        borcTuruAd = G(() => d.borcTuruAd, ""),
                        kalanToplamToplam = N(G(() => d.kalanToplamToplam, 0d)),
                        kalanMiktarToplam = N(G(() => d.kalanMiktarToplam, 0d)),
                        geriAlimMiktarToplam = N(G(() => d.geriAlimMiktarToplam, 0d)),
                        islemYapilanMiktarToplam = N(G(() => d.islemYapilanMiktarToplam, 0d))
                    });
                return Results.Json(new { satirlar = satirlar });
            }
            catch (Exception)
            {
                return Hata("Liste okunamadı.");
            }
        }

        // ---------------------------------------------------------------- dosya (eski SatirSecildi / IslemTarihDegisti / EkranaKayitYaz)

        private static IResult DosyaGetir(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, string islemTarihi, string onayTarihi)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            DosyaAB k = new DosyaAB();
            k.iPARDDonemi = iPARDDonemi ?? 0;
            k.dosyaAnahtar = dosyaAnahtar ?? 0;
            k.islemTarihi = IslemTarihiBul(islemTarihi);
            k.onayTarihi = IslemTarihiBul(onayTarihi);
            if (k.iPARDDonemi <= 0 || k.dosyaAnahtar <= 0) return Hata("Dosya seçiniz.");
            try
            {
                ObjectArray bilgi = Servis().ListeleDosyaAB(kullanan, k);
                if (bilgi == null) return Hata("Dosya okunamadı.");
                if (bilgi.sonuc != null && !bilgi.sonuc.islemSonuc) return Hata(HataMetni(bilgi.sonuc.hataStr, "Dosya okunamadı."));
                if (bilgi.objeler.Count == 0) return Hata(HataMetni(bilgi.sonuc?.bilgiStr, "Dosya bulunamadı."));
                DosyaAB f = (DosyaAB)bilgi.objeler[0];
                f.IslemToplamBul(f.islemTarihi, -1, true, null, f.onayTarihi, -1);
                return Results.Json(DosyaJson(f));
            }
            catch (Exception)
            {
                return Hata("Dosya okunamadı.");
            }
        }

        private static object DosyaJson(DosyaAB f)
        {
            return new
            {
                iPARDDonemi = f.iPARDDonemi,
                dosyaAnahtar = f.dosyaAnahtar,
                iPARDDonemiAd = DosyaAB.iPARDDonemiAdBul(f.iPARDDonemi) + "/" + f.dosyaAnahtar,
                // Borçlu Kimlik Bilgileri
                defterSirasi = f.defterSirasi ?? "",
                tedbir = f.tedbir ?? "",
                altTedbir = f.altTedbir ?? "",
                butceKalemi = f.butceKalemi ?? "",
                saymanlikKod = f.saymanlikKod,
                birimKod = f.birimKod ?? "",
                HrcBirimAd = G(() => f.HrcBirimAd, "") ?? "",
                borcluKod = f.borcluKod,
                borcluKodAd = G(() => f.borcluKodAd, ""),
                basvuruKayitNumarasi = f.basvuruKayitNumarasi ?? "",
                sozlesmeNumarasi = f.sozlesmeNumarasi ?? "",
                // Ön Borç Bilgileri
                borcTuru = (int)f.borcTuru,
                borcTuruAd = G(() => f.borcTuruAd, ""),
                borcunKoduAd = G(() => f.borcunKoduAd, ""),
                vakaNumarasi = f.vakaNumarasi ?? "",
                avans = f.avans,
                onBorcTarihi = TarihYaz(f.onBorcTarihi),
                onBorcSayi = f.onBorcSayi ?? "",
                ipardAktarimTar = TarihYaz(f.ipardAktarimTar),
                fazlaOdeme = f.borcTuru == DosyaAB.BorcTurleri.FazlaOdeme,
                donemYil = G(() => f.donemYil, 0),
                donemCeyrek = G(() => f.donemCeyrek, 0),
                ongorulenSilinmeTarihi = TarihYaz(G(() => f.ongorulenSilinmeTarihiDT, DateTime.MinValue)),
                onBorcDovizKuru = N(f.onBorcDovizKuru),
                onBorcMiktarAB = N(f.onBorcMiktarAB),
                onBorcMiktarUlusal = N(f.onBorcMiktarUlusal),
                onBorcToplam = N(G(() => f.onBorcToplam, 0d)),
                onBorcMiktarABEUR = N(G(() => f.onBorcMiktarABEUR, 0d)),
                onBorcMiktarUlusalEUR = N(G(() => f.onBorcMiktarUlusalEUR, 0d)),
                onBorcToplamEUR = N(G(() => f.onBorcToplamEUR, 0d)),
                // Süreç durumu
                geriAlimSurecAsamasi = (int)f.geriAlimSurecAsamasi,
                geriAlimSurecAsamasiAd = G(() => f.geriAlimSurecAsamasiAd, ""),
                yorumlarTR = f.yorumlarTR ?? "",
                // Geri Alım Kararı
                geriAlimKarariTarihi = TarihYaz(f.geriAlimKarariTarihi),
                geriAlimSayi = f.geriAlimSayi ?? "",
                bildirimTarihi = TarihYaz(f.bildirimTarihi),
                tesellumTarihi = TarihYaz(f.tesellumTarihi),
                odemeSonTarihi = TarihYaz(f.odemeSonTarihi),
                temerrutTarihi = TarihYaz(f.temerrutTarihi),
                faizHesabiSonTarihi = TarihYaz(G(() => f.faizHesabiSonTarihi, null)),
                faizinDurdurulmaTarihi = TarihYaz(f.faizinDurdurulmaTarihi),
                geriAlimDovizKuru = N(f.geriAlimDovizKuru),
                geriAlimMiktarAB = N(f.geriAlimMiktarAB),
                geriAlimMiktarUlusal = N(f.geriAlimMiktarUlusal),
                geriAlimMiktarToplam = N(G(() => f.geriAlimMiktarToplam, 0d)),
                geriAlimMiktarABEUR = N(G(() => f.geriAlimMiktarABEUR, 0d)),
                geriAlimMiktarUlusalEUR = N(G(() => f.geriAlimMiktarUlusalEUR, 0d)),
                geriAlimMiktarToplamEUR = N(G(() => f.geriAlimMiktarToplamEUR, 0d)),
                silinmeyecekMiktar = N(f.silinmeyecekMiktar),
                silinmeyecekMiktarEUR = N(f.silinmeyecekMiktarEUR),
                // Geri Ödeme İşlemleri (TL)
                kalanMiktarABSil = N(G(() => f.kalanMiktarABSil, 0d)),
                kalanMiktarUlusalSil = N(G(() => f.kalanMiktarUlusalSil, 0d)),
                kalanMiktarToplam = N(G(() => f.kalanMiktarToplam, 0d)),
                kalanFaizMiktarABSil = N(G(() => f.kalanFaizMiktarABSil, 0d)),
                kalanFaizMiktarUlusalSil = N(G(() => f.kalanFaizMiktarUlusalSil, 0d)),
                kalanFaizMiktarToplam = N(G(() => f.kalanFaizMiktarToplam, 0d)),
                kalanToplamABSil = N(G(() => f.kalanToplamABSil, 0d)),
                kalanToplamUlusalSil = N(G(() => f.kalanToplamUlusalSil, 0d)),
                kalanToplamToplam = N(G(() => f.kalanToplamToplam, 0d)),
                // Geri Ödeme İşlemleri (EUR)
                kalanMiktarABEURSil = N(G(() => f.kalanMiktarABEURSil, 0d)),
                kalanMiktarUlusalEURSil = N(G(() => f.kalanMiktarUlusalEURSil, 0d)),
                kalanMiktarToplamEUR = N(G(() => f.kalanMiktarToplamEUR, 0d)),
                kalanFaizMiktarABEURSil = N(G(() => f.kalanFaizMiktarABEURSil, 0d)),
                kalanFaizMiktarUlusalEURSil = N(G(() => f.kalanFaizMiktarUlusalEURSil, 0d)),
                kalanFaizMiktarToplamEUR = N(G(() => f.kalanFaizMiktarToplamEUR, 0d)),
                kalanToplamABEURSil = N(G(() => f.kalanToplamABEURSil, 0d)),
                kalanToplamUlusalEURSil = N(G(() => f.kalanToplamUlusalEURSil, 0d)),
                kalanToplamToplamEUR = N(G(() => f.kalanToplamToplamEUR, 0d)),
                // Borç hareket tablosu başlığındaki toplamlar (eski GridBaslikToplamYaz)
                islemYapilanMiktarAB = N(G(() => f.islemYapilanMiktarAB, 0d)),
                islemYapilanMiktarUlusal = N(G(() => f.islemYapilanMiktarUlusal, 0d)),
                islemYapilanFaizMiktarAB = N(G(() => f.islemYapilanFaizMiktarAB, 0d)),
                islemYapilanFaizMiktarUlusal = N(G(() => f.islemYapilanFaizMiktarUlusal, 0d)),
                islemYapilanToplamToplam = N(G(() => f.islemYapilanToplamToplam, 0d)),
                islemYapilanMiktarABEUR = N(G(() => f.islemYapilanMiktarABEUR, 0d)),
                islemYapilanMiktarUlusalEUR = N(G(() => f.islemYapilanMiktarUlusalEUR, 0d)),
                islemYapilanFaizMiktarABEUR = N(G(() => f.islemYapilanFaizMiktarABEUR, 0d)),
                islemYapilanFaizMiktarUlusalEUR = N(G(() => f.islemYapilanFaizMiktarUlusalEUR, 0d)),
                islemYapilanToplamToplamEUR = N(G(() => f.islemYapilanToplamToplamEUR, 0d)),
                // Taksitlendirme
                taksitTalepTarih = TarihYaz(f.taksitTalepTarih),
                taksitTalepSayi = f.taksitTalepSayi ?? "",
                taksitKararTarih = TarihYaz(f.taksitKararTarih),
                taksitKararSayi = f.taksitKararSayi ?? "",
                taksitIlkTarih = TarihYaz(f.taksitIlkTarih),
                taksitSayisi = f.taksitSayisi.ToString("#"),
                taksitTur = G(() => f.TecilFaizTur(), ""),
                // Tablolar
                borcluIslemler = Satirlar(f.borcuBilgileri, o => Parca((DosyaABParca)o)),
                onBorcIslemler = Satirlar(f.OnBorcBilgileri, o => Parca((DosyaABParca)o)),
                gakIslemler = Satirlar(f.GAKBilgileri, o => Parca((DosyaABParca)o)),
                durumIslemler = Satirlar(f.durumBilgileri, o => Parca((DosyaABParca)o)),
                taksitIslemler = Satirlar(f.TaksitBilgileri, o => Parca((DosyaABParca)o)),
                islemler = Satirlar(f.islemler, o => Parca((DosyaABParca)o)),
                // Bütün hareketler (Taksit dahil); süzme (eski TurVarMi) sayfadadır
                hareketler = Hareketler(f.borcHareket, false)
            };
        }

        // ---------------------------------------------------------------- liste raporları (eski YazdirIslem)

        private static Hashtable ListeRaporParam(ABListeKriteri k, DosyaAB a)
        {
            Hashtable ht = new Hashtable();
            ht["avans"] = a.avans.ToString();
            ht["iPARDDonemi"] = a.iPARDDonemi.ToString();
            ht["islemTarihAralik"] = a.islemTarihAralik;
            ht["onBorcTarihAralik"] = a.onBorcTarihAralik;
            ht["geriAlimTarihAralik"] = a.geriAlimKarariTarihAralik;
            ht["geriAlimSurecAsamalari"] = a.geriAlimSurecAsamalari;
            ht["saymanlikKod"] = a.saymanlikKod.ToString();
            ht["birimKod"] = a.birimKod;
            ht["aramaMetni"] = a.aramaMetni;
            ht["borcTurleri"] = a.borcTurleri;
            ht["islemliste"] = a.islemliste;
            ht["borcDurumu"] = a.borcDurumu.ToString();
            DateTime b = TarihOku(k.islTarih);
            ht["basTarih"] = b == DateTime.MinValue ? "" : new TNSDateTime(b).ToString();
            ht["islemTarihi"] = new TNSDateTime(TarihOku(k.islemTarihi)).ToString();
            ht["onayTarihi"] = new TNSDateTime(TarihOku(k.onayTarihi)).ToString();
            return ht;
        }

        /// <summary>Liste Yazdır (3, 4, 5, 6, 8) ve Mutabakat Raporları (18, 11, 12, 13, 14, 15, 19, 20).</summary>
        private static readonly int[] ListeRaporlari = { 3, 4, 5, 6, 8, 11, 12, 13, 14, 15, 18, 19, 20 };

        private static IResult Yazdir(HttpContext baglam, IWebHostEnvironment ortam, int? no, int? ipard, string durum, string muhasebe, string birim,
            string aramaMetni, string borcTuru, int? borcDurumu, bool? avans, string tarihTur, string islTarih, string islTarihSon,
            string islemTarihi, string onayTarihi)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int rapNo = no ?? 0;
            if (Array.IndexOf(ListeRaporlari, rapNo) < 0) return Hata("Rapor seçiniz.");
            ABListeKriteri k = Kriter(rapNo, ipard, durum, muhasebe, birim, aramaMetni, borcTuru, borcDurumu, avans, tarihTur, islTarih, islTarihSon,
                islemTarihi, onayTarihi, false);
            return RaporGonder(kullanan, rapNo, ListeRaporParam(k, KriterTopla(k)));
        }

        /// <summary>Eski RaporBilgiBul: saklanabilen borçlu defterleri ve saklı rapor adları.</summary>
        private static string SakliRaporAd(int rapNo)
        {
            switch (rapNo)
            {
                case 3: return "6305 Borçlu Defteri (TL)";
                case 4: return "6305 Borçlu Defteri (EUR)";
                case 5: return "6305 Borçlu Defteri Yeni (TL)";
                case 6: return "6305 Borçlu Defteri Yeni (EUR)";
                default: return null;
            }
        }

        private static async Task<IResult> Sakla(HttpContext baglam, IWebHostEnvironment ortam, ABListeKriteri k, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (k == null) return Results.BadRequest();
            string raporAd = SakliRaporAd(k.no);
            if (raporAd == null) return Basarisiz("Bu rapor saklanamaz.");
            string dosya = null;
            try
            {
                AsposeLisans.Yukle();
                dosya = RaporlamaServis.TekRaporAl(RaporGrup, k.no, kullanan, Servis(), ListeRaporParam(k, KriterTopla(k)), null);
                if (string.IsNullOrEmpty(dosya)) return Basarisiz("Rapor hazırlanamadı.");
                if (RaporlamaServis.MesajMi(dosya)) { string m = dosya; dosya = null; return Basarisiz(HataMetni(m, "Rapor hazırlanamadı.")); }
                byte[] icerik = File.ReadAllBytes(dosya);

                DateTime simdi = DateTime.Now;
                raporAd += " (" + simdi.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + " " + simdi.ToString("HHmm", CultureInfo.InvariantCulture) + ").xlsx";

                // ASSUMPTION: saklı rapor grubu SakliRaporGruplari.Gruplar[0].kod (eski SakliRaporlar.dgb[0].dosyaGrupKod = "G_BDR").
                EkliDosyalar kayit = new EkliDosyalar("ATMAB", SakliRaporGruplari.Gruplar[0].kod, 0)
                {
                    dosyaAdi = raporAd,
                    buyukluk = icerik.Length
                };
                kayit.ekliDosyaIcerik = icerik;
                Sonuc sonuc = TNS.KYM.Arac.Tanimla().KaydetEkliDosya(kullanan, kayit);
                if (sonuc == null || !sonuc.islemSonuc) return Basarisiz(HataMetni(sonuc?.hataStr, "Rapor saklanamadı."));
                return Results.Json(new { basarili = true, bilgi = "Rapor saklandı" });
            }
            catch (RaporException ex)
            {
                return Basarisiz(HataMetni(ex.Message, "Rapor hazırlanamadı."));
            }
            catch (Exception)
            {
                return Basarisiz("Rapor saklanamadı.");
            }
            finally
            {
                if (dosya != null)
                {
                    try { File.Delete(dosya); } catch { }
                }
            }
        }

        // ---------------------------------------------------------------- dosya raporları (eski btnRaporlar_Click)

        /// <summary>1 6307 Faiz Hes. Tab., 7 6310, 10 6807, 9 Silinecek Tutarların Bildirimi (pb 1 TL / 2 EUR), 16 6805, 21 Yeniden Hesapla.</summary>
        private static readonly int[] DosyaRaporlari = { 1, 7, 9, 10, 16, 21 };

        private static IResult DosyaRaporu(HttpContext baglam, IWebHostEnvironment ortam, int? no, int? tur, int? pb, int? iPARDDonemi, int? dosyaAnahtar,
            string islemTarihi, string onayTarihi)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            int rapNo = no ?? 1;
            if (Array.IndexOf(DosyaRaporlari, rapNo) < 0) return Hata("Rapor seçiniz.");
            if ((iPARDDonemi ?? 0) <= 0 || (dosyaAnahtar ?? 0) <= 0) return Hata("Lütfen önce bir dosya seçiniz.");
            Hashtable ht = new Hashtable();
            ht["iPARDDonemi"] = (iPARDDonemi ?? 0).ToString();
            ht["dosyaAnahtar"] = (dosyaAnahtar ?? 0).ToString();
            ht["CiktiYeri"] = tur == 2 ? "pdf" : (rapNo == 9 ? "docx" : "xlsx");
            DateTime it = TarihOku(islemTarihi), ot = TarihOku(onayTarihi);
            ht["islemTarihi"] = (it == DateTime.MinValue ? DateTime.Now.Date : it).ToString();
            ht["onayTarihi"] = (ot == DateTime.MinValue ? DateTime.Now.Date : ot).ToString();
            if (rapNo == 9) ht["pb"] = (pb ?? 1).ToString();
            return RaporGonder(kullanan, rapNo, ht);
        }

        /// <summary>İşlem penceresindeki "6307 Faiz Hes. Tab." (no 1; tur 2 PDF): faiz hesabı işlem tarihine kadar (eski btnRaporlar2_Click).</summary>
        private static IResult RaporOdeme(HttpContext baglam, IWebHostEnvironment ortam, int? no, int? tur, int? iPARDDonemi, int? dosyaAnahtar, string tarih)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((iPARDDonemi ?? 0) <= 0 || (dosyaAnahtar ?? 0) <= 0) return Hata("Lütfen önce bir dosya seçiniz.");
            int rapNo = no ?? 1;
            if (rapNo != 1) return Hata("Rapor seçiniz.");
            Hashtable ht = new Hashtable();
            ht["iPARDDonemi"] = (iPARDDonemi ?? 0).ToString();
            ht["dosyaAnahtar"] = (dosyaAnahtar ?? 0).ToString();
            ht["CiktiYeri"] = tur == 2 ? "pdf" : "xlsx";
            ht["faizHesabiSonTarihi"] = TarihOku(tarih).ToString();
            return RaporGonder(kullanan, rapNo, ht);
        }

        /// <summary>Geri Bildirim Yazdır (rapor 2, docx): işlem penceresindeki değerlerle (eski btnGeriBildirimYazdir_Click).</summary>
        private static IResult RaporGeriBildirim(HttpContext baglam, IWebHostEnvironment ortam, int? iPARDDonemi, int? dosyaAnahtar, int? siraNo,
            string tarih, string borcunIlgiliOlduguTaksit, string duzeltimTarihi, double? yatirilanAB, double? yatirilanUlusal, bool? olduguGibi)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((iPARDDonemi ?? 0) <= 0 || (dosyaAnahtar ?? 0) <= 0) return Hata("Lütfen önce bir dosya seçiniz.");
            Hashtable ht = new Hashtable();
            ht["iPARDDonemi"] = (iPARDDonemi ?? 0).ToString();
            ht["dosyaAnahtar"] = (dosyaAnahtar ?? 0).ToString();
            ht["CiktiYeri"] = "docx";
            ht["borcunIlgiliOlduguTaksit"] = borcunIlgiliOlduguTaksit ?? "";
            ht["tarih"] = TnsTarih(tarih).ToString();
            ht["siraNo"] = (siraNo ?? 0).ToString();
            ht["olduguGibi"] = olduguGibi == true ? "1" : "0";
            ht["duzeltimTarihi"] = TnsTarih(duzeltimTarihi).ToString();
            ht["yatirilanAB"] = (yatirilanAB ?? 0).ToString();
            ht["yatirilanUlusal"] = (yatirilanUlusal ?? 0).ToString();
            return RaporGonder(kullanan, 2, ht);
        }

        /// <summary>
        /// Taksit raporu (rapor 17, xlsx; eski btnTaksitYazdir_Click). kayitli=1: kayıtlı taksitler (Taksitlendirme sekmesi);
        /// kayitli=0: taksit penceresindeki kaydedilmemiş değerlerle.
        /// </summary>
        private static IResult RaporTaksit(HttpContext baglam, IWebHostEnvironment ortam, int? kayitli, int? iPARDDonemi, int? dosyaAnahtar,
            string islemTarihi, string onayTarihi, string girisTarihi, string taksitTalepTarihi, string taksitTalepSayi, string taksitKararTarihi,
            string taksitKararSayi, string taksitIlkTarih, int? taksitSayisi, int? tecilFaizNo)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if ((iPARDDonemi ?? 0) <= 0 || (dosyaAnahtar ?? 0) <= 0) return Hata("Lütfen önce bir dosya seçiniz.");
            Hashtable ht = new Hashtable();
            ht["iPARDDonemi"] = (iPARDDonemi ?? 0).ToString();
            ht["dosyaAnahtar"] = (dosyaAnahtar ?? 0).ToString();
            ht["CiktiYeri"] = "xlsx";
            ht["islemTarihi"] = TnsTarih(islemTarihi).ToString();
            ht["onayTarihi"] = TnsTarih(onayTarihi).ToString();
            if (kayitli == 0)
            {
                ht["taksitGirisTarih"] = TnsTarih(girisTarihi).ToString();
                ht["taksitTalepTarih"] = TnsTarih(taksitTalepTarihi).ToString();
                ht["taksitTalepSayi"] = taksitTalepSayi ?? "";
                ht["taksitKararTarih"] = TnsTarih(taksitKararTarihi).ToString();
                ht["taksitKararSayi"] = taksitKararSayi ?? "";
                ht["taksitIlkTarih"] = TnsTarih(taksitIlkTarih).ToString();
                ht["taksitSayisi"] = (taksitSayisi ?? 0).ToString();
                ht["tecilFaizNo"] = (tecilFaizNo ?? 0).ToString();
            }
            return RaporGonder(kullanan, 17, ht);
        }

        // ---------------------------------------------------------------- dönem faizi (eski btnHesaplat_Click)

        private static async Task<IResult> DonemFaizi(HttpContext baglam, IWebHostEnvironment ortam, ABDonemFaiziIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            if (i.yil < 2000 || i.ay < 1 || i.ay > 12)
                return Basarisiz("Lütfen yıl ve ay değerlerini seçiniz, girilen tarihin ayına ait son günü için işlem yapılacaktır.");
            int hTur = i.hTur >= 1 && i.hTur <= 3 ? i.hTur : 1;
            try
            {
                DosyaAB form = new DosyaAB();
                form.geriAlimSurecAsamalari = "1,2,3,4,5,6,7,8,9,10,12,13,14,15,16,18,19";
                form.iPARDDonemi = i.iPARDDonemi;
                TNSDateTime dt2 = new TNSDateTime(new DateTime(i.yil, i.ay, DateTime.DaysInMonth(i.yil, i.ay)));
                Sonuc s = Servis().DosyalarinFaiziniHesapla(kullanan, dt2, form, hTur);
                if (s == null || !s.islemSonuc) return Basarisiz(HataMetni(s?.hataStr, "İşlem yapılamadı."));
                return Results.Json(new { basarili = true, bilgi = "İşlem tamamlandı." });
            }
            catch (Exception)
            {
                return Basarisiz("İşlem yapılamadı.");
            }
        }

        // ---------------------------------------------------------------- imzalar (eski wndImzalar)

        private static IResult ImzaOku(HttpContext baglam, IWebHostEnvironment ortam)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            try
            {
                string k = kullanan.kullaniciKodu;
                return Results.Json(new
                {
                    hazirlayan = GenelIslemler.KullaniciDegiskenGetir(k, "ImzHazirlayan") ?? "",
                    kontrolEden = GenelIslemler.KullaniciDegiskenGetir(k, "ImzKontrolEden") ?? "",
                    gozdenGeciren = GenelIslemler.KullaniciDegiskenGetir(k, "ImzGozdenGeciren") ?? "",
                    onaylayan = GenelIslemler.KullaniciDegiskenGetir(k, "ImzOnaylayan") ?? "",
                    uygunBulan = GenelIslemler.KullaniciDegiskenGetir(k, "ImzUygunBulan") ?? "",
                    hazirlayanUnv = GenelIslemler.KullaniciDegiskenGetir(k, "ImzHazirlayanUnv") ?? "",
                    kontrolEdenUnv = GenelIslemler.KullaniciDegiskenGetir(k, "ImzKontrolEdenUnv") ?? "",
                    gozdenGecirenUnv = GenelIslemler.KullaniciDegiskenGetir(k, "ImzGozdenGecirenUnv") ?? "",
                    onaylayanUnv = GenelIslemler.KullaniciDegiskenGetir(k, "ImzOnaylayanUnv") ?? "",
                    uygunBulanUnv = GenelIslemler.KullaniciDegiskenGetir(k, "ImzUygunBulanUnv") ?? ""
                });
            }
            catch (Exception)
            {
                return Hata("İmza bilgileri okunamadı.");
            }
        }

        private static async Task<IResult> ImzaKaydet(HttpContext baglam, IWebHostEnvironment ortam, ABImzaIstegi i, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            if (i == null) return Results.BadRequest();
            try
            {
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzHazirlayan", Temiz(i.hazirlayan));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzKontrolEden", Temiz(i.kontrolEden));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzGozdenGeciren", Temiz(i.gozdenGeciren));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzOnaylayan", Temiz(i.onaylayan));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzUygunBulan", Temiz(i.uygunBulan));

                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzHazirlayanUnv", Temiz(i.hazirlayanUnv));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzKontrolEdenUnv", Temiz(i.kontrolEdenUnv));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzGozdenGecirenUnv", Temiz(i.gozdenGecirenUnv));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzOnaylayanUnv", Temiz(i.onaylayanUnv));
                GenelIslemler.KullaniciDegiskenSakla(kullanan, "ImzUygunBulanUnv", Temiz(i.uygunBulanUnv));
                return Results.Json(new { basarili = true, bilgi = "İmza bilgileri saklandı!" });
            }
            catch (Exception)
            {
                return Basarisiz("İmza bilgileri saklanamadı.");
            }
        }

        // ---------------------------------------------------------------- kişi / firma arama (eski ListeFirmaKisi; borçlu penceresi)

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
                    // kod: borçlu kaydındaki borcluKod (eski FirmaBilgisiGoster: txtborcluKod = firma.kod)
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

        // ---------------------------------------------------------------- kur (eski DovizKuruBul)

        private static IResult Kur(HttpContext baglam, IWebHostEnvironment ortam, string tarih)
        {
            IResult ret = Denetle(baglam, ortam, out Kullanici kullanan);
            if (ret != null) return ret;
            DateTime d = TarihOku(tarih);
            if (d == DateTime.MinValue) return Results.Json(new { kur = 0d });
            try
            {
                return Results.Json(new { kur = N(Servis().DovizKuruBulOncAy(kullanan, new TNSDateTime(d))) });
            }
            catch (Exception)
            {
                return Hata("Döviz kuru okunamadı.");
            }
        }
    }
}
