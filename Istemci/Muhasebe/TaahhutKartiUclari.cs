using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Istemci.OrtakSayfa;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OrtakClass;
using TNS;
using TNS.KYM;
using TNS.MUH;
using static Istemci.Muhasebe.MuhTanimOrtak;

namespace Istemci.Muhasebe
{
    /// <summary>Değer artışı satırı (eski StoreDegerArtisi alanları). Tarihler yyyy-MM-dd.</summary>
    public class TaahhutDegerArtisiSatiri
    {
        public string KOD { get; set; }
        public string ONAYTARIHI { get; set; }
        public string SOZLESMETARIHI { get; set; }
        public double SOZLESMETUTAR { get; set; }
        public string DAMGAVERGISITARIHI { get; set; }
        public string DAMGAVERGISINO { get; set; }
        public double DAMGAVERGISITUTAR { get; set; }
        public int ODEMEEMRIYIL { get; set; }
        public string ODEMEEMRINO { get; set; }
    }

    /// <summary>Taahhüt kartı kaydı (eski btnKaydet_Click). Tarihler yyyy-MM-dd; kod -1 ise yeni kayıt.</summary>
    public class TaahhutKartiIstegi
    {
        public int tur { get; set; }
        public int kod { get; set; } = -1;
        public int yil { get; set; }
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
        public string projeSiraNo { get; set; }
        public string DptProjeSiraNo { get; set; }
        public string muteahhitKimlikNo { get; set; }
        public string muteahhitUnvan { get; set; }
        public string muteahhitAdres { get; set; }
        public string konu { get; set; }
        public double kesifBeldeli { get; set; }
        public double indirimOrani { get; set; }
        public double ihaleBedeli { get; set; }
        public double gerceklesenTaahhutTutar { get; set; }
        public string damgaVergisiTarihi { get; set; }
        public string damgaVergisiNo { get; set; }
        public double damgaVergisiTutar { get; set; }
        public string sozlesmeDVTarihi { get; set; }
        public string sozlesmeDVNo { get; set; }
        public double sozlesmeDVTutar { get; set; }
        public string sozlesmeTarihi { get; set; }
        public string iseBaslamaTarihi { get; set; }
        public string geciciKabulTarihi { get; set; }
        public string isBitirmeTarihi { get; set; }
        public string sureUzatimi1 { get; set; }
        public string sureUzatimi2 { get; set; }
        public string sureUzatimi3 { get; set; }
        public int isKod { get; set; }
        public string garantiBaslamaTarihi { get; set; }
        public string garantiBitisTarihi { get; set; }
        public string alimUsuluKod { get; set; }
        public int sgkBildirimi { get; set; }
        public string odenekTertip { get; set; }
        public string duranTertip { get; set; }
        public string aciklama { get; set; }
        public string parabirimiKod { get; set; }
        public List<TaahhutDegerArtisiSatiri> degerArtisi { get; set; }
    }

    /// <summary>Kod (ve gerekirse kod listesi / değer artışı kodu) taşıyan istek.</summary>
    public class TaahhutKodIstegi
    {
        public int kod { get; set; }
        public string daKod { get; set; }
        public List<int> kodlar { get; set; }
    }

    /// <summary>
    /// Açılış tahakkuku (tur 1, eski btnOdemeEmriOlusturTahakkuk) ya da değer artışı muhasebe fişi (tur 2, eski btnDegerArtisiOdemeEmriOlustur).
    /// Hesaplar, belge yılı / no, ödenek tertibi ve para birimi eski sayfadaki gibi formdan gelir.
    /// </summary>
    public class TaahhutAcilisIstegi
    {
        public int kod { get; set; }
        public int tur { get; set; }
        public string daKod { get; set; }
        public string borcHesap { get; set; }
        public string alacakHesap { get; set; }
        public int oeYil { get; set; }
        public string oeBelgeNo { get; set; }
        public string odenekTertip { get; set; }
        public string parabirimiKod { get; set; }
    }

    /// <summary>Değer artışı penceresi (eski wndDegerArtisi / DegerArtisiKaydet).</summary>
    public class TaahhutDegerArtisiIstegi
    {
        public int taahhutKod { get; set; }
        public string kod { get; set; }
        public string onayTarihi { get; set; }
        public string sozlesmeTarihi { get; set; }
        public string damgaVergisiTarihi { get; set; }
        public double damgaVergisiTutari { get; set; }
        public string damgaVergisiNo { get; set; }
        public double sozlesmeTutari { get; set; }
        public int odemeEmriYil { get; set; }
        public string odemeEmriNo { get; set; }
    }

    /// <summary>Personel sözleşmesi / maaş farkı üretimi (eski SozUretParams; satır satır çağrılır).</summary>
    public class TaahhutSozlesmeUretIstegi
    {
        public int sozKod { get; set; }
        public string tcKimlikNo { get; set; }
        public string basTar { get; set; }
        public string bitTar { get; set; }
        public int sure { get; set; }
        public int islemKod { get; set; }
    }

    /// <summary>Personel ödeme emri üretimi (eski PersonelMIFUret): kriterlerdeki yıl, muhasebe, birim.</summary>
    public class TaahhutMifUretIstegi
    {
        public int yil { get; set; }
        public int muhasebeKod { get; set; }
        public string birimKod { get; set; }
    }

    /// <summary>Teminat / avans / borç pencerelerinin satırı (eski Store alanları). Tarihler yyyy-MM-dd.</summary>
    public class TaahhutPencereSatiri
    {
        public int KOD { get; set; }
        public int YIL { get; set; }
        public string DOSYANO { get; set; }
        public string BELGENO { get; set; }
        public string TARIH { get; set; }
        public string NUMARA { get; set; }
        public string ACIKLAMA { get; set; }
        public double TUTAR { get; set; }
        public double VERILENAVANS { get; set; }
        public double MAHSUPEDILENAVANS { get; set; }
        public string ALACAKLIADSOYAD { get; set; }
        public string TEMLIKACANVERGINO { get; set; }
        public string TEMLIKVERENVERGINO { get; set; }
        public double TEMLIKTUTARI { get; set; }
    }

    public class TaahhutPencereIstegi
    {
        public int kod { get; set; }
        public List<TaahhutPencereSatiri> satirlar { get; set; }
    }

    /// <summary>Taahhüt planı satırı (eski Taahhutler.aspx storeData).</summary>
    public class TaahhutPlanSatiri
    {
        public int kod { get; set; } = -1;
        public int yil { get; set; }
        public int ay { get; set; }
        public string tertipKod { get; set; }
        public double ilkTaahhutTutari { get; set; }
        public double fiyatFarki { get; set; }
        public double ilaveTaahhutler { get; set; }
        public double digerTaahhutler { get; set; }
    }

    public class TaahhutPlanIstegi
    {
        public int kod { get; set; }
        public string daKod { get; set; }
        public List<TaahhutPlanSatiri> satirlar { get; set; }
    }

    /// <summary>Bakım tarihçesi (eski TaahhutBakimTarihce.aspx).</summary>
    public class TaahhutBakimIstegi
    {
        public int kod { get; set; }
        public int taahhutKod { get; set; }
        public string girisTarihi { get; set; }
        public string girisAciklama { get; set; }
    }

    /// <summary>Sözleşme koşulları / garanti / teminat şartları metni (eski TanimSozlesme.aspx).</summary>
    public class TaahhutSozlesmeMetniIstegi
    {
        public int kod { get; set; }
        public int taahhutKod { get; set; }
        public int tur { get; set; }
        public string aciklama { get; set; }
    }

    public class TaahhutBelgeSilIstegi
    {
        public string kod { get; set; }
    }

    /// <summary>
    /// Sözleşme ve Taahhüt Kartı (eski ButceMuhasebe/TaahhutKarti.aspx; ?tur=2 Personel Taahhütleri) ve açtığı alt ekranlar.
    /// İş kuralları (IMUHServis.TaahhutKarti*, Taahhut*, OrtakBelge*, OdemeEmriMIFKaydet) değişmeden çağrılır; eski code-behind
    /// mantığı aynen buradadır. Uçlar /api/taahhutkarti/*:
    ///   GET  tanim?tur, liste?..., belge?kod, degerartisi/acilabilir?kod
    ///   POST kaydet, sil, kapat, iptal, acilis-tahakkuk, degerartisi/kaydet, personel/sozlesme-uret, personel/mif-uret, personel/da-mif-uret
    ///   teminat|avans|borc: GET liste?kod, GET secim?kod, POST kaydet, POST sil      odemeemri: GET liste?kod, POST sil
    ///   plan (Taahhutler.aspx): GET liste?kod&amp;dakod, POST kaydet, POST sil
    ///   bakim (TaahhutBakimTarihce.aspx), sozlesme-metni (TanimSozlesme.aspx), belge-ekle (TanimBelgeEkle.aspx)
    ///   hesap/ara|ad|agac (Sgb.hesapKodu), rapor (TaahhutListe.xlt), yazdir (TaahhutKarti.xlt), karaliste (MUH-KaraListeFirma.xlt)
    /// Hakediş ödeme emri formu (TaahhutOdemeEmriKayit.aspx): TaahhutOdemeEmriUclari.cs.
    /// Yetki: menüdeki iki seçenekten (Taahhüt Kartı, Personel Taahhütleri) biri; alt sayfalar yetkiyi bunlardan alır. Eski sayfada
    /// sistem yöneticisi ya da Ödeme Emri/MİF kullanıcısı olmayanlara gizlenen işlemler (sil, dosya kapat, açılış tahakkuku, teminat /
    /// avans / borç / ödeme emri pencereleri) sunucuda da denetlenir.
    /// </summary>
    public static class TaahhutKartiUclari
    {
        public const string Kok = "/api/taahhutkarti/";
        public const string SayfaAdresi = "../ButceMuhasebe/TaahhutKarti.aspx";
        public const string PersonelAdresi = SayfaAdresi + "?tur=2";
        public static readonly string[] UstSayfalar = { SayfaAdresi, PersonelAdresi };

        public static void Ekle(WebApplication app)
        {
            app.MapGet(Kok + "tanim", Tanim);
            app.MapGet(Kok + "liste", Liste);
            app.MapGet(Kok + "belge", Belge);
            app.MapPost(Kok + "kaydet", Kaydet);
            app.MapPost(Kok + "sil", Sil);
            app.MapPost(Kok + "kapat", Kapat);
            app.MapPost(Kok + "iptal", Iptal);
            app.MapPost(Kok + "acilis-tahakkuk", AcilisTahakkuk);
            app.MapGet(Kok + "degerartisi/acilabilir", DegerArtisiAcilabilir);
            app.MapPost(Kok + "degerartisi/kaydet", DegerArtisiKaydet);
            app.MapPost(Kok + "personel/sozlesme-uret", SozlesmeUret);
            app.MapPost(Kok + "personel/mif-uret", PersonelMifUret);
            app.MapPost(Kok + "personel/da-mif-uret", PersonelDaMifUret);

            app.MapGet(Kok + "teminat/liste", TeminatListe);
            app.MapGet(Kok + "teminat/secim", TeminatSecim);
            app.MapPost(Kok + "teminat/kaydet", TeminatKaydet);
            app.MapPost(Kok + "teminat/sil", TeminatSil);
            app.MapGet(Kok + "avans/liste", AvansListe);
            app.MapGet(Kok + "avans/secim", AvansSecim);
            app.MapPost(Kok + "avans/kaydet", AvansKaydet);
            app.MapPost(Kok + "avans/sil", AvansSil);
            app.MapGet(Kok + "borc/liste", BorcListe);
            app.MapGet(Kok + "borc/secim", BorcSecim);
            app.MapPost(Kok + "borc/kaydet", BorcKaydet);
            app.MapPost(Kok + "borc/sil", BorcSil);
            app.MapGet(Kok + "odemeemri/liste", OdemeEmriListe);
            app.MapPost(Kok + "odemeemri/sil", OdemeEmriSil);

            app.MapGet(Kok + "plan/liste", PlanListe);
            app.MapPost(Kok + "plan/kaydet", PlanKaydet);
            app.MapPost(Kok + "plan/sil", PlanSil);
            app.MapGet(Kok + "bakim/liste", BakimListe);
            app.MapPost(Kok + "bakim/kaydet", BakimKaydet);
            app.MapPost(Kok + "bakim/sil", BakimSil);
            app.MapGet(Kok + "sozlesme-metni", SozlesmeMetni);
            app.MapPost(Kok + "sozlesme-metni/kaydet", SozlesmeMetniKaydet);
            app.MapPost(Kok + "sozlesme-metni/sil", SozlesmeMetniSil);
            app.MapGet(Kok + "belge-ekle/liste", EkBelgeListe);
            app.MapPost(Kok + "belge-ekle/yukle", EkBelgeYukle).DisableAntiforgery();
            app.MapGet(Kok + "belge-ekle/indir", EkBelgeIndir);
            app.MapPost(Kok + "belge-ekle/sil", EkBelgeSil);

            app.MapGet(Kok + "hesap/ara", HesapAra);
            app.MapGet(Kok + "hesap/ad", HesapAd);
            app.MapGet(Kok + "hesap/agac", HesapAgac);

            app.MapGet(Kok + "rapor", TaahhutKartiRaporlari.Rapor);
            app.MapGet(Kok + "yazdir", TaahhutKartiRaporlari.Yazdir);
            app.MapGet(Kok + "karaliste", TaahhutKartiRaporlari.KaraListe);

            TaahhutOdemeEmriUclari.Ekle(app);
        }

        // ------------------------------------------------------------------ yetki

        /// <summary>Menüdeki seçenek: tur 2 Personel Taahhütleri, diğerleri Taahhüt Kartı.</summary>
        public static bool Yetkili(Kullanici kullanan, string klasor, int tur)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, tur == 2 ? PersonelAdresi : SayfaAdresi);
        }

        /// <summary>İki seçenekten birine yetkisi olan uçları kullanabilir.</summary>
        public static bool YetkiVar(Kullanici kullanan, string klasor)
        {
            return Yetkili(kullanan, klasor, 1) || Yetkili(kullanan, klasor, 2);
        }

        /// <summary>Alt sayfa (menüde yok): yetki üst sayfalardan.</summary>
        public static bool AltSayfaYetkili(Kullanici kullanan, string klasor, string altAdres)
        {
            return SayfaYetki.Yetkili(kullanan, klasor, altAdres, UstSayfalar);
        }

        /// <summary>
        /// Eski Page_Load: sistem yöneticisi ya da Ödeme Emri/MİF kullanıcısı değilse Taahhüt Planı, Alınan Teminatlar, Avans, Borçlar,
        /// Ödeme Emri menüleri ile Açılış Tahakkuku, Sil, Dosya Kapat ve Tahakkuku Oluştur tuşları gizlenir.
        /// </summary>
        public static bool OdemeYetkili(Kullanici kullanan)
        {
            if (kullanan == null) return false;
            return kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.SISTEMYONETICISI)
                || kullanan.KullaniciTipiIceriyorMu((int)ENUMKullaniciTipi.ODEMEEMRI_MIF);
        }

        private static IMUHServis Servis() { return TNS.MUH.Arac.Tanimla(); }
        private static TNS.HRC.IHRCServis ServisHRC() { return TNS.HRC.Arac.Tanimla(); }

        private static Func<Kullanici, bool> Genel(IWebHostEnvironment ortam) { return k => YetkiVar(k, ortam.ContentRootPath); }
        private static Func<Kullanici, bool> Odeme(IWebHostEnvironment ortam) { return k => YetkiVar(k, ortam.ContentRootPath) && OdemeYetkili(k); }
        private static Func<Kullanici, bool> Personel(IWebHostEnvironment ortam) { return k => Yetkili(k, ortam.ContentRootPath, 2); }

        private static string Q(HttpContext baglam, string ad) { return Temiz(baglam.Request.Query[ad]); }
        private static int QSayi(HttpContext baglam, string ad, int varsayilan = 0) { return OrtakFonksiyonlar.ConvertToInt(Q(baglam, ad), varsayilan); }
        private static TNSDateTime Tarih(string s) { return OdemeEmriMIFUclari.Tarih(s); }
        private static string TarihYaz(TNSDateTime t) { return OdemeEmriMIFUclari.TarihYaz(t); }

        private static string DurumAdi(int kapandi)
        {
            if (kapandi == 1) return "KAPANDI";
            if (kapandi == 9) return "İPTAL";
            if (kapandi == 99) return "YOK";
            return "DEVAM";
        }

        /// <summary>Kaydı kodundan okur (eski TaahhutKartiListele(kod)). Bulunamazsa null; hata metni hata'ya yazılır.</summary>
        internal static TaahhutKartiBilgi KartOku(Kullanici kullanan, int kod, out string hata)
        {
            hata = "";
            TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
            bilgi.kod = kod;
            ObjectArray liste = Servis().TaahhutKartiListele(kullanan, bilgi, 0);
            if (liste == null) { hata = "Taahhüt kartı okunamadı."; return null; }
            if (!liste.sonuc.islemSonuc) { hata = HataMetni(liste.sonuc.hataStr, "Taahhüt kartı okunamadı."); return null; }
            foreach (TaahhutKartiBilgi t in liste.objeler) return t;
            hata = "Taahhüt kartı bulunamadı.";
            return null;
        }

        // ------------------------------------------------------------------ tanım ve liste

        /// <summary>Sayfa açılışı (eski Page_Load): dönem yılı, para birimleri, işler, alım usulleri, personel sözleşme tarihleri.</summary>
        private static IResult Tanim(HttpContext baglam, IWebHostEnvironment ortam, int? tur)
        {
            int t = tur == 2 ? 2 : 1;
            return Oku(baglam, k => Yetkili(k, ortam.ContentRootPath, t), kullanan =>
            {
                string kurumKod = GenelIslemlerIstemci.VarsayilanKurumBul() ?? "";
                int yil = DateTime.Now.Year;
                int donemYil = yil;
                // Manas ve personel ise 9. aydan öncesi önceki yıldır
                if (kurumKod.Replace(".", "") == "1399" && t == 2)
                    donemYil = DateTime.Now.Month < 9 ? yil - 1 : yil;

                bool doviz = TNS.MUH.Arac.MuhasebeDovizVarMi();
                List<object> paraBirimleri = new List<object>();
                if (doviz)
                {
                    ObjectArray paralar = ServisHRC().ParaBirimiListele(kullanan, new TNS.BKD.ParaBirimi());
                    if (paralar != null && paralar.objeler != null)
                        foreach (TNS.BKD.ParaBirimi p in paralar.objeler)
                            paraBirimleri.Add(new { kod = p.kisaltma ?? "", ad = p.kisaltma ?? "" });
                }

                List<object> alimUsulleri = new List<object>();
                ObjectArray alimListe = ServisHRC().IhaleTurMaddeListele(kullanan, 0);
                if (alimListe != null && alimListe.objeler != null)
                    foreach (TNS.HRC.IhaleTurMadde a in alimListe.objeler)
                        alimUsulleri.Add(new { kod = a.adi ?? "", ad = (a.adi ?? "") + "-" + (a.aciklama ?? "") });

                // Personel sözleşme üretimi varsayılan tarihleri: 1 Eylül'den 12 ay
                DateTime basTar = new DateTime(DateTime.Now.Year + (DateTime.Now.Month > 6 ? 0 : -1), 9, 1);
                DateTime bitTar = basTar.AddMonths(12).AddDays(-1);

                return Results.Json(new
                {
                    tur = t,
                    yil = yil,
                    donemYil = donemYil,
                    doviz = doviz,
                    paraBirimleri = paraBirimleri,
                    isler = IsListesi(kullanan, 0),
                    alimUsulleri = alimUsulleri,
                    odemeYetkili = OdemeYetkili(kullanan),
                    sozBasTar = basTar.ToString("yyyy-MM-dd"),
                    sozBitTar = bitTar.ToString("yyyy-MM-dd"),
                    bugun = DateTime.Now.ToString("yyyy-MM-dd")
                });
            }, "Sayfa bilgileri okunamadı.");
        }

        /// <summary>Eski IsDoldur: bu yılın işleri; kayıttaki iş listede yoksa o da eklenir.</summary>
        private static List<object> IsListesi(Kullanici kullanan, int isKod)
        {
            List<object> isler = new List<object>();
            bool kayitVar = false;
            ObjectArray liste = Servis().IsListele(kullanan, new IsTanim() { sorguYil = DateTime.Now.Year });
            if (liste != null && liste.objeler != null)
                foreach (IsTanim i in liste.objeler)
                {
                    isler.Add(new { kod = i.kod, ad = i.adi ?? "" });
                    if (i.kod == isKod) kayitVar = true;
                }
            if (isKod > 0 && !kayitVar)
            {
                ObjectArray aktifIs = Servis().IsListele(kullanan, new IsTanim() { kod = isKod });
                if (aktifIs != null && aktifIs.objeler != null)
                    foreach (IsTanim i in aktifIs.objeler)
                        isler.Add(new { kod = i.kod, ad = i.adi ?? "" });
            }
            return isler;
        }

        /// <summary>Belge Arama (eski Listele / btnListe_Click / btnListelePer_Click).</summary>
        private static IResult Liste(HttpContext baglam, IWebHostEnvironment ortam)
        {
            int tur = QSayi(baglam, "tur", 1) == 2 ? 2 : 1;
            bool personel = Q(baglam, "personel") == "1";
            return Oku(baglam, k => Yetkili(k, ortam.ContentRootPath, tur), kullanan =>
            {
                TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                bilgi.yil = QSayi(baglam, "yil");
                bilgi.muhasebeKod = QSayi(baglam, "muhasebeKod");
                bilgi.birimKod = Q(baglam, "birimKod");
                bilgi.muteahhitKimlikNo = Q(baglam, "vergiNo");
                bilgi.muteahhitUnvan = Q(baglam, "adSoyad");
                bilgi.projeSiraNo = Q(baglam, "projeSiraNo");
                bilgi.konu = Q(baglam, "konu");
                bilgi.odenekTertip = Q(baglam, "eko");
                bilgi.isKod = QSayi(baglam, "isKod");
                bilgi.alimUsuluKod = Q(baglam, "alimUsuluKod");
                bilgi.kapandi = QSayi(baglam, "durum");
                bilgi.arananTarih = Tarih(Q(baglam, "tarih"));
                bilgi.sorguSozlesmeTarihBas = Tarih(Q(baglam, "sozBas"));
                bilgi.sorguSozlesmeTarihBit = Tarih(Q(baglam, "sozBit"));
                int kod = QSayi(baglam, "kod");
                if (kod > 0) bilgi.kod = kod;

                if (bilgi.projeSiraNo.Length > 0 && bilgi.projeSiraNo.Length < 9)
                {
                    int siraNo = OrtakFonksiyonlar.ConvertToInt(bilgi.projeSiraNo, 0);
                    bilgi.projeSiraNo = bilgi.yil + siraNo.ToString("0000");
                }
                bilgi.sozTur = tur == 2 ? TaahhutKartiBilgi.ENUMSozTur.Personel : TaahhutKartiBilgi.ENUMSozTur.Normal;
                if (personel && tur != 2) return Hata("Bu işlem yalnızca Personel Taahhütleri ekranında yapılır.");

                ObjectArray liste = Servis().TaahhutKartiListele(kullanan, bilgi, personel ? 1 : 0);
                if (liste == null) return Results.Json(new { satirlar = new List<object>() });
                if (!liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc.hataStr, "Liste okunamadı."));

                Hashtable birimAd = new Hashtable();
                Hashtable muhasebeAd = new Hashtable();
                List<object> satirlar = new List<object>();
                foreach (TaahhutKartiBilgi t in liste.objeler)
                {
                    string mk = t.muhasebeKod.ToString();
                    if (muhasebeAd[mk] == null) muhasebeAd[mk] = GenelIslemler.KodAd(11, mk, true);
                    string bk = t.birimKod + "-" + mk.Trim();
                    if (birimAd[bk] == null) birimAd[bk] = GenelIslemler.KodAd(12, bk, false);

                    string odemeEmriYilNo = "";
                    if (t.odemeEmriYil > 0) odemeEmriYilNo = t.odemeEmriYil + " / " + t.odemeEmriNo;

                    satirlar.Add(new
                    {
                        KOD = t.kod,
                        MUHASEBEKOD = t.muhasebeKod,
                        MUHASEBEAD = (string)muhasebeAd[mk],
                        PROJESIRANO = t.projeSiraNo ?? "",
                        DPTPROJESIRANO = t.DptProjeSiraNo ?? "",
                        MUTEAHHITKIMLIKNO = t.muteahhitKimlikNo ?? "",
                        MUTEAHHITUNVAN = t.muteahhitUnvan ?? "",
                        MUTEAHHITADRES = t.muteahhitAdres ?? "",
                        KONU = t.konu ?? "",
                        KESIFBEDELI = t.kesifBeldeli,
                        INDIRIMORANI = t.indirimOrani,
                        IHALEBEDELI = t.ihaleBedeli,
                        SOZLESMETARIHI = TarihYaz(t.sozlesmeTarihi),
                        ISEBASLAMATARIHI = TarihYaz(t.iseBaslamaTarihi),
                        ISBITIRMETARIHI = TarihYaz(t.isBitirmeTarihi),
                        ODENEKTERTIP = t.odenekTertip ?? "",
                        DURANTERTIP = t.duranTertip ?? "",
                        ISLEMYAPANAD = t.islemYapanAd ?? "",
                        BIRIMKOD = t.birimKod ?? "",
                        BIRIMAD = (string)birimAd[bk],
                        YIL = t.yil,
                        ACIKLAMA = t.aciklama ?? "",
                        ODEMEEMRIYIL = t.odemeEmriYil,
                        ODEMEEMRINO = t.odemeEmriNo ?? "",
                        ODEMEEMRIYILNO = odemeEmriYilNo,
                        ODEMEEMRIDURUM = t.odemeEmriDurum,
                        ISKOD = t.isKod,
                        KAPANDI = t.kapandi,
                        DURUMADI = DurumAdi(t.kapandi),
                        TAAHHUTTUTARI = t.sorguTaahhutTutar,
                        ODENENTUTAR = t.sorguOdenenTutar,
                        KALANTUTAR = t.sorguTaahhutTutar - t.sorguOdenenTutar,
                        ALIMUSULUKOD = t.alimUsuluKod ?? ""
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            }, "Liste okunamadı.");
        }

        private static object DegerArtisiSatiri(TaahhutKartiDegerArtisi d)
        {
            return new
            {
                KOD = d.kod ?? "",
                ONAYTARIHI = TarihYaz(d.onayTarihi),
                SOZLESMETARIHI = TarihYaz(d.sozlesmeTarihi),
                SOZLESMETUTAR = d.sozlesmeTutari,
                DAMGAVERGISITARIHI = TarihYaz(d.damgaVergisiTarihi),
                DAMGAVERGISINO = d.damgaVergisiNo ?? "",
                DAMGAVERGISITUTAR = d.damgaVergisiTutari,
                ODEMEEMRIYIL = d.odemeEmriYil,
                ODEMEEMRINO = d.odemeEmriNo ?? ""
            };
        }

        private static List<object> DegerArtisleri(TaahhutKartiBilgi t)
        {
            List<object> l = new List<object>();
            if (t.degerArtisi != null && t.degerArtisi.objeler != null)
                foreach (TaahhutKartiDegerArtisi d in t.degerArtisi.objeler) l.Add(DegerArtisiSatiri(d));
            return l;
        }

        /// <summary>Kaydı forma getirir (eski DegerleriGetir).</summary>
        private static IResult Belge(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                if ((kod ?? 0) <= 0) return Hata("Kayıt seçilmedi.");
                TaahhutKartiBilgi t = KartOku(kullanan, kod.Value, out string hata);
                if (t == null) return Hata(hata);

                int firmaKod = 0;
                try
                {
                    TNS.HRC.FirmaBilgisi fir = ServisHRC().FirmaBilgisiListele(kullanan, 0, t.muteahhitKimlikNo);
                    if (fir != null) firmaKod = fir.kod;
                }
                catch (Exception) { }

                return Results.Json(new
                {
                    belge = new
                    {
                        kod = t.kod,
                        yil = t.yil,
                        muhasebeKod = t.muhasebeKod,
                        birimKod = t.birimKod ?? "",
                        projeSiraNo = t.projeSiraNo ?? "",
                        DptProjeSiraNo = t.DptProjeSiraNo ?? "",
                        muteahhitKimlikNo = t.muteahhitKimlikNo ?? "",
                        muteahhitUnvan = t.muteahhitUnvan ?? "",
                        muteahhitAdres = t.muteahhitAdres ?? "",
                        konu = t.konu ?? "",
                        kesifBeldeli = t.kesifBeldeli,
                        indirimOrani = t.indirimOrani,
                        ihaleBedeli = t.ihaleBedeli,
                        gerceklesenTaahhutTutar = t.gerceklesenTaahhutTutar,
                        damgaVergisiTarihi = TarihYaz(t.damgaVergisiTarihi),
                        damgaVergisiNo = t.damgaVergisiNo ?? "",
                        damgaVergisiTutar = t.damgaVergisiTutar,
                        sozlesmeDVTarihi = TarihYaz(t.sozlesmeDVTarihi),
                        sozlesmeDVNo = t.sozlesmeDVNo ?? "",
                        sozlesmeDVTutar = t.sozlesmeDVTutar,
                        sozlesmeTarihi = TarihYaz(t.sozlesmeTarihi),
                        iseBaslamaTarihi = TarihYaz(t.iseBaslamaTarihi),
                        geciciKabulTarihi = TarihYaz(t.geciciKabulTarihi),
                        isBitirmeTarihi = TarihYaz(t.isBitirmeTarihi),
                        sureUzatimi1 = t.sureUzatimi1 ?? "",
                        sureUzatimi2 = t.sureUzatimi2 ?? "",
                        sureUzatimi3 = t.sureUzatimi3 ?? "",
                        odenekTertip = t.odenekTertip ?? "",
                        duranTertip = t.duranTertip ?? "",
                        aciklama = t.aciklama ?? "",
                        parabirimiKod = t.parabirimiKod ?? "",
                        isKod = t.isKod,
                        garantiBaslamaTarihi = TarihYaz(t.garantiBaslamaTarihi),
                        garantiBitisTarihi = TarihYaz(t.garantiBitisTarihi),
                        alimUsuluKod = t.alimUsuluKod ?? "",
                        sgkBildirimi = t.sgkBildirimi,
                        odemeEmriYil = t.odemeEmriYil,
                        odemeEmriNo = t.odemeEmriNo ?? "",
                        islemYapanAd = t.islemYapanAd ?? "",
                        kapandi = t.kapandi,
                        sozlesmeYili = t.sozlesmeTarihi == null || t.sozlesmeTarihi.isNull ? 0 : t.sozlesmeTarihi.Yil,
                        firmaKod = firmaKod
                    },
                    degerArtisi = DegerArtisleri(t),
                    isler = IsListesi(kullanan, t.isKod)
                });
            }, "Kayıt okunamadı.");
        }

        // ------------------------------------------------------------------ kaydet, sil, kapat, iptal

        /// <summary>Eski btnKaydet_Click.</summary>
        private static Task<IResult> Kaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKartiIstegi istek)
        {
            int tur = istek != null && istek.tur == 2 ? 2 : 1;
            return Yaz(baglam, sahtecilik, k => Yetkili(k, ortam.ContentRootPath, tur), kullanan =>
            {
                if (istek == null) return Hata("Bilgiler alınamadı.");
                TaahhutKartiBilgi form = new TaahhutKartiBilgi();
                form.yil = istek.yil;
                form.muhasebeKod = istek.muhasebeKod;
                form.birimKod = Temiz(istek.birimKod);
                form.kod = istek.kod > 0 ? istek.kod : -1;
                form.projeSiraNo = istek.projeSiraNo ?? "";
                form.DptProjeSiraNo = istek.DptProjeSiraNo ?? "";
                form.muteahhitKimlikNo = istek.muteahhitKimlikNo ?? "";
                form.muteahhitUnvan = istek.muteahhitUnvan ?? "";
                form.muteahhitAdres = istek.muteahhitAdres ?? "";
                form.konu = istek.konu ?? "";
                form.kesifBeldeli = istek.kesifBeldeli;
                form.indirimOrani = istek.indirimOrani;
                form.ihaleBedeli = istek.ihaleBedeli;
                form.gerceklesenTaahhutTutar = istek.gerceklesenTaahhutTutar;
                form.damgaVergisiTarihi = Tarih(istek.damgaVergisiTarihi);
                form.damgaVergisiNo = istek.damgaVergisiNo ?? "";
                form.damgaVergisiTutar = istek.damgaVergisiTutar;
                form.sozlesmeDVTarihi = Tarih(istek.sozlesmeDVTarihi);
                form.sozlesmeDVNo = istek.sozlesmeDVNo ?? "";
                form.sozlesmeDVTutar = istek.sozlesmeDVTutar;
                form.sozlesmeTarihi = Tarih(istek.sozlesmeTarihi);
                form.iseBaslamaTarihi = Tarih(istek.iseBaslamaTarihi);
                form.geciciKabulTarihi = Tarih(istek.geciciKabulTarihi);
                form.isBitirmeTarihi = Tarih(istek.isBitirmeTarihi);
                form.sureUzatimi1 = istek.sureUzatimi1 ?? "";
                form.sureUzatimi2 = istek.sureUzatimi2 ?? "";
                form.sureUzatimi3 = istek.sureUzatimi3 ?? "";
                form.isKod = istek.isKod;
                form.garantiBaslamaTarihi = Tarih(istek.garantiBaslamaTarihi);
                form.garantiBitisTarihi = Tarih(istek.garantiBitisTarihi);
                form.alimUsuluKod = istek.alimUsuluKod ?? "";
                form.sgkBildirimi = istek.sgkBildirimi;
                form.odenekTertip = istek.odenekTertip ?? "";
                form.tertipHesapKod = "";
                form.duranTertip = istek.duranTertip ?? "";
                form.aciklama = istek.aciklama ?? "";
                form.islemYapanAd = kullanan.ADSOYAD;
                form.islemYapanKod = kullanan.TCKIMLIKNO;
                form.islemTarihi.Yaz(DateTime.Now);
                form.parabirimiKod = TNS.MUH.Arac.MuhasebeDovizVarMi() ? (istek.parabirimiKod ?? "") : "";

                ObjectArray degerArtisListe = new ObjectArray();
                if (istek.degerArtisi != null)
                    foreach (TaahhutDegerArtisiSatiri s in istek.degerArtisi)
                    {
                        if (s == null) continue;
                        TaahhutKartiDegerArtisi da = new TaahhutKartiDegerArtisi();
                        da.kod = s.KOD ?? "";
                        da.onayTarihi = Tarih(s.ONAYTARIHI);
                        da.sozlesmeTarihi = Tarih(s.SOZLESMETARIHI);
                        da.damgaVergisiTarihi = Tarih(s.DAMGAVERGISITARIHI);
                        da.sozlesmeTutari = s.SOZLESMETUTAR;
                        da.damgaVergisiNo = s.DAMGAVERGISINO ?? "";
                        da.damgaVergisiTutari = s.DAMGAVERGISITUTAR;
                        da.odemeEmriYil = s.ODEMEEMRIYIL;
                        da.odemeEmriNo = s.ODEMEEMRINO ?? "";
                        if (da.sozlesmeTutari != 0) degerArtisListe.objeler.Add(da);
                    }
                form.degerArtisi = degerArtisListe;
                form.sozTur = tur == 2 ? TaahhutKartiBilgi.ENUMSozTur.Personel : TaahhutKartiBilgi.ENUMSozTur.Normal;

                Sonuc sonuc = Servis().TaahhutKartiKaydet(kullanan, form, null);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Results.Json(new
                {
                    basarili = true,
                    bilgi = "Bilgiler başarıyla kayıt edildi.",
                    uyari = "Açılış tahakkuku oluşturma işlemi yapmayı unutmayın",
                    kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, form.kod)
                });
            });
        }

        /// <summary>Eski btnTaahhutSil_Click.</summary>
        private static Task<IResult> Sil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Silinecek kayıt seçilmedi.");
                Sonuc sonuc = Servis().TaahhutKartiSil(kullanan, istek.kod);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Silinemedi."));
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        /// <summary>Eski btnDosyaKapat_Click: açılış ve değer artışı tahakkukları yapılmış, muhasebe fişlerinde fark kalmamış olmalı.</summary>
        private static Task<IResult> Kapat(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Kayıt seçilmedi.");
                int kod = istek.kod;
                TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                bilgi.kod = kod;
                ObjectArray liste = Servis().TaahhutKartiListele(kullanan, bilgi, 0);
                string pb = "TL";
                if (liste != null && liste.objeler != null)
                    foreach (TaahhutKartiBilgi d in liste.objeler)
                    {
                        if (string.IsNullOrEmpty(d.odemeEmriNo))
                            return Hata("Açılış tahakkuk belgesi hazırlanmamış. Bu nedenle bu Taahhüt Kartı kapatılamaz");
                        foreach (TaahhutKartiDegerArtisi da in d.degerArtisi.objeler)
                            if (string.IsNullOrEmpty(da.odemeEmriNo))
                                return Hata("Değer artışı tahakkuk belgesi hazırlanmamış. Bu nedenle bu Taahhüt Kartı kapatılamaz");
                        if (TNS.MUH.Arac.MuhasebeDovizVarMi()) pb = d.parabirimiKod ?? "";
                    }

                double kapatmaFark = Servis().TaahhutOdemeEmriHazirlananKapatmaFarki(kullanan, kod);
                if (Math.Round(kapatmaFark, 0) != 0)
                    return Hata("Muhasebe fişlerinde " + kapatmaFark.ToString("#,###.00") + pb + " kadar bir fark bulunmaktadır. Bu nedenle bu Taahhüt Kartı kapatılamaz");

                TNSCollection tc = new TNSCollection();
                TaahhutKartiBilgi tkb = new TaahhutKartiBilgi();
                tkb.kod = kod;
                tc.Add(tkb);
                Sonuc sonuc = Servis().TaahhutKartiKapat(kullanan, tc, 1);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Dosya kapatılamadı."));
                return Tamam("Dosya kapatıldı");
            });
        }

        /// <summary>Eski btnIptalListe_Click: listede seçilen dosyalar iptal edilir (durum 9).</summary>
        private static Task<IResult> Iptal(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.kodlar == null || istek.kodlar.Count == 0) return Hata("Lütfen seçim yapınız!");
                TNSCollection tc = new TNSCollection();
                foreach (int kod in istek.kodlar)
                {
                    TaahhutKartiBilgi tk = new TaahhutKartiBilgi();
                    tk.kod = kod;
                    tc.Add(tk);
                }
                Sonuc sonuc = Servis().TaahhutKartiKapat(kullanan, tc, 9);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Dosyalar iptal edilemedi."));
                return Tamam("Dosyalar iptal edildi.");
            });
        }

        // ------------------------------------------------------------------ açılış tahakkuku / değer artışı muhasebe fişi

        /// <summary>
        /// Açılış tahakkuku (tur 1, eski btnOdemeEmriOlusturTahakkuk) ya da değer artışı muhasebe fişi (tur 2, eski
        /// btnDegerArtisiOdemeEmriOlustur): ödeme emri belgesi kaydedilir, numarası taahhüt kartına / değer artışına yazılır.
        /// </summary>
        private static Task<IResult> AcilisTahakkuk(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutAcilisIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                bool degerArtisi = istek.tur == 2;
                if (degerArtisi && string.IsNullOrEmpty(istek.daKod)) return Hata("İlk önce değer artışı satırını kayıt edin");

                TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                bilgi.kod = istek.kod;
                ObjectArray liste = Servis().TaahhutKartiListele(kullanan, bilgi, 0);
                if (liste != null && !liste.sonuc.islemSonuc) return Hata(HataMetni(liste.sonuc.hataStr, "Taahhüt kartı okunamadı."));

                TNS.MUH.OdemeEmriMIFForm oe = new TNS.MUH.OdemeEmriMIFForm();
                double tutar = 0.0;
                if (liste != null)
                    foreach (TaahhutKartiBilgi d in liste.objeler)
                    {
                        string aciklama = d.yil + "/" + d.projeSiraNo + " numaralı taahhüt dosyası kapsamında oluşturulmuştur. Sözleşme Tarihi:" + d.sozlesmeTarihi.ToString() + "; Sözleşme Süresi:" + d.iseBaslamaTarihi.ToString() + "-" + d.isBitirmeTarihi.ToString() + " tarihleri arasıdır.";
                        if (!degerArtisi)
                        {
                            oe.yil = istek.oeYil;
                            TNSDateTime belgeTarih;
                            if (d.sozlesmeTarihi.Yil < oe.yil) belgeTarih = new TNSDateTime("02.01." + oe.yil);
                            else belgeTarih = d.sozlesmeTarihi;
                            oe.yil = belgeTarih.Yil;
                            oe.islemTarih = belgeTarih;
                            oe.aciklama = aciklama;
                            if (d.aciklama != "") oe.aciklama += d.aciklama;
                            tutar = d.ihaleBedeli;
                        }
                        else
                        {
                            oe.yil = d.sozlesmeTarihi.Yil;
                            oe.islemTarih = d.sozlesmeTarihi;
                            oe.aciklama = aciklama;
                        }
                        oe.belgeNo = Temiz(istek.oeBelgeNo);
                        oe.kurum = TNS.MUH.Arac.VarsayilanKurumKod();
                        oe.muhasebe = d.muhasebeKod;
                        oe.birim = d.birimKod;
                        oe.ilgiliAd = d.muteahhitUnvan;
                        oe.ilgiliNo = d.muteahhitKimlikNo;
                        oe.projeNo = d.projeSiraNo;

                        if (degerArtisi)
                            foreach (TaahhutKartiDegerArtisi da in d.degerArtisi.objeler)
                            {
                                if (da.kod != istek.daKod) continue;
                                tutar = da.sozlesmeTutari;
                                if (da.sozlesmeTarihi.Yil > 0) oe.yil = da.sozlesmeTarihi.Yil;
                                if (da.sozlesmeTarihi.Yil > 0) oe.islemTarih = da.sozlesmeTarihi;
                                if (da.odemeEmriNo != "") oe.belgeNo = da.odemeEmriNo;
                                if (da.odemeEmriYil > 0) oe.yil = da.odemeEmriYil;
                            }
                    }

                string borcHesap = Temiz(istek.borcHesap).Split('-')[0].Trim();
                string alacakHesap = Temiz(istek.alacakHesap).Split('-')[0].Trim();
                if (borcHesap == "" || alacakHesap == "") return Hata("Borç ve alacak hesap kodlarını giriniz.");

                ObjectArray detay = new ObjectArray();
                TNS.MUH.OdemeEmriMIFDetay td = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = borcHesap;
                if (!degerArtisi || tutar > 0)
                {
                    td.borc = Math.Round(Math.Abs(tutar), 2);
                    td.alacak = 0.0;
                }
                else
                {
                    td.borc = 0.0;
                    td.alacak = Math.Round(Math.Abs(tutar), 2);
                }
                if (!degerArtisi) td.borc = Math.Round(tutar, 2);
                td.altHesapKod = oe.ilgiliNo;
                TNS.MUH.Arac.TertipAta(ref td, istek.odenekTertip ?? "");
                detay.objeler.Add(td);

                td = new TNS.MUH.OdemeEmriMIFDetay(oe.yil);
                td.hesapKod = alacakHesap;
                if (!degerArtisi || tutar > 0)
                {
                    td.borc = 0.0;
                    td.alacak = Math.Round(Math.Abs(tutar), 2);
                }
                else
                {
                    td.borc = Math.Round(Math.Abs(tutar), 2);
                    td.alacak = 0;
                }
                if (!degerArtisi) td.alacak = Math.Round(tutar, 2);
                td.altHesapKod = oe.ilgiliNo;
                detay.objeler.Add(td);
                oe.detay = detay;

                string paraBirimiKod = Temiz(istek.parabirimiKod);
                if (paraBirimiKod != "")
                {
                    decimal kurDegeri = KurDegeri(kullanan, paraBirimiKod, oe.islemTarih);
                    oe = OdemeEmriDetayDovizDonustur(Servis(), kullanan, oe, kurDegeri);
                }

                Sonuc sonuc = Servis().OdemeEmriMIFKaydet(kullanan, oe);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Ödeme emri belgesi oluşturulamadı."));
                string belgeNo = sonuc.anahtar;

                if (!degerArtisi)
                {
                    int kartYil = bilgi.yil;
                    if (kartYil == 0) kartYil = istek.oeYil;
                    sonuc = Servis().TaahhutKartiOdemeEmriKaydet(kullanan, kartYil, bilgi.kod, oe.yil, belgeNo);
                }
                else
                    sonuc = Servis().TaahhutKartiDegerArtisiOdemeEmriKaydet(kullanan, istek.daKod, bilgi.kod, oe.yil, belgeNo);

                if (sonuc == null || !sonuc.islemSonuc)
                    return Results.Json(new { hata = HataMetni(sonuc?.hataStr, "Belge numarası taahhüt kartına yazılamadı."), oeYil = oe.yil, oeBelgeNo = belgeNo });
                return Results.Json(new { basarili = true, bilgi = "Ödeme emri belgesi başarıyla oluşturuldu.", oeYil = oe.yil, oeBelgeNo = belgeNo });
            });
        }

        // ------------------------------------------------------------------ değer artışı penceresi ve personel işlemleri

        /// <summary>Eski DegerArtisiYapiabilirMi.</summary>
        private static IResult DegerArtisiAcilabilir(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                if ((kod ?? 0) <= 0) return Hata("Değer artışı kaydetmeden önce, lütfen daha önce kaydedilmiş bir kaydı açınız!");
                ObjectVAN ov = Servis().TaahhutKartiDegerArtisiAcilabilirMi(kullanan, new TaahhutKartiDegerArtisi() { taahhutKod = kod.Value });
                if (ov == null || ov.sonuc == null || !ov.sonuc.islemSonuc) return Hata(HataMetni(ov?.sonuc?.hataStr, "Tahakkuk kaydı kontrol edilemedi!"));
                return Results.Json(new { basarili = true });
            }, "Tahakkuk kaydı kontrol edilemedi!");
        }

        /// <summary>Eski DegerArtisiKaydet (değer artışı penceresi).</summary>
        private static Task<IResult> DegerArtisiKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutDegerArtisiIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.taahhutKod <= 0) return Hata("Değer artışı kaydetmeden önce, lütfen daha önce kaydedilmiş bir kaydı açınız!");
                TaahhutKartiDegerArtisi da = new TaahhutKartiDegerArtisi();
                da.taahhutKod = istek.taahhutKod;
                da.kod = string.IsNullOrWhiteSpace(istek.kod) ? null : istek.kod;
                da.onayTarihi = Tarih(istek.onayTarihi);
                da.sozlesmeTarihi = Tarih(istek.sozlesmeTarihi);
                da.damgaVergisiTarihi = Tarih(istek.damgaVergisiTarihi);
                da.damgaVergisiTutari = istek.damgaVergisiTutari;
                da.damgaVergisiNo = istek.damgaVergisiNo ?? "";
                da.sozlesmeTutari = istek.sozlesmeTutari;
                da.odemeEmriYil = istek.odemeEmriYil;
                da.odemeEmriNo = istek.odemeEmriNo ?? "";
                Sonuc s = Servis().TaahhutKartiDegerArtisiKaydet(kullanan, da, null);
                if (s == null || !s.islemSonuc) return Hata(HataMetni(s?.hataStr, "Kayıt işlemi hata ile sonuçlandı!"));
                TaahhutKartiBilgi t = KartOku(kullanan, istek.taahhutKod, out string _);
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarılı.", kod = s.anahtar ?? "", degerArtisi = t != null ? DegerArtisleri(t) : null });
            });
        }

        /// <summary>Eski SozlesmeVeyaDegOlustur: personel sözleşmesi (islemKod 1) ya da maaş farkı (islemKod 2). Hata yoksa boş metin.</summary>
        private static Task<IResult> SozlesmeUret(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutSozlesmeUretIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Personel(ortam), kullanan =>
            {
                if (istek == null) return Hata("Bilgiler alınamadı.");
                string sonuc;
                try
                {
                    sonuc = istek.islemKod == 1 ? SozlesmeOlusturIc(kullanan, istek, true) : DegisimOlusturIc(kullanan, istek, true);
                }
                catch (Exception ex)
                {
                    sonuc = ex.Message;
                }
                return Results.Json(new { basarili = true, sonuc = sonuc ?? "" });
            });
        }

        /// <summary>Eski PersonelMIFUret.</summary>
        private static Task<IResult> PersonelMifUret(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutMifUretIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Personel(ortam), kullanan =>
            {
                if (istek == null) return Hata("Bilgiler alınamadı.");
                TaahhutKartiBilgi bilgi = new TaahhutKartiBilgi();
                bilgi.yil = istek.yil;
                bilgi.muhasebeKod = istek.muhasebeKod;
                bilgi.birimKod = Temiz(istek.birimKod);
                Sonuc s = Servis().TaahhutKartiOdemeEmriUret(kullanan, bilgi);
                if (s == null || !s.islemSonuc) return Hata(HataMetni(s?.hataStr, "Ödeme emri üretilemedi."));
                return Tamam("Kayıt başarılı, üretilen MİF No:" + s.anahtar);
            });
        }

        /// <summary>Eski PersonelDegerArtisiMIFUret.</summary>
        private static Task<IResult> PersonelDaMifUret(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Personel(ortam), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                Sonuc s = Servis().TaahhutKartiOdemeEmriUret(kullanan, istek.kod, istek.daKod);
                if (s == null || !s.islemSonuc) return Hata(HataMetni(s?.hataStr, "Değer artışı MİF üretilemedi!"));
                TaahhutKartiBilgi t = KartOku(kullanan, istek.kod, out string _);
                return Results.Json(new { basarili = true, bilgi = "MİF üretildi, lütfen kontrol ediniz!", degerArtisi = t != null ? DegerArtisleri(t) : null });
            });
        }

        // Eski DegisimOlusturIc (maaş farkı): mevcut taahhüt planı ile yeni bordro maliyeti arasındaki fark değer artışı olarak yazılır.
        private static string DegisimOlusturIc(Kullanici kullanan, TaahhutSozlesmeUretIstegi sup, bool hataYoksaBos)
        {
            IMUHServis servisMUH = Servis();
            TNSDateTime basTar = Tarih(sup.basTar);
            TaahhutKartiBilgi taahhutMevcut = new TaahhutKartiBilgi();
            taahhutMevcut.arananTarih = basTar;
            taahhutMevcut.muteahhitKimlikNo = sup.tcKimlikNo;
            ObjectArray oaTah = servisMUH.TaahhutKartiListele(kullanan, taahhutMevcut, 0);
            if (!oaTah.sonuc.islemSonuc)
                return oaTah.sonuc.hataStr;
            else if (oaTah.ObjeSayisi == 0)
                return $"{sup.tcKimlikNo} kişisine ait taahhüt kartı bulunamadı!";
            else if (oaTah.ObjeSayisi > 1)
                return $"{sup.tcKimlikNo} kişisine ait birden fazla taahhüt kartı bulunamadı!";
            else
            {
                taahhutMevcut = (TaahhutKartiBilgi)oaTah.objeler[0];
                if (taahhutMevcut.kapandi != 0)
                    return $"{sup.tcKimlikNo} kişisine ait taahhüt kartı üzerinde işlem yapılamaz!";
            }

            TNSDateTime bitTar = taahhutMevcut.isBitirmeTarihi;
            int yil = basTar.Yil;
            ObjectArray oa = TNS.HRC.Arac.Tanimla().PersonelMaliyetHesapla(kullanan, sup.tcKimlikNo, basTar, bitTar, sup.sure);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 2)
            {
                TNS.IKY.Personel perBilgi = (TNS.IKY.Personel)oa.Al(0);
                if (perBilgi == null)
                    return $"{sup.tcKimlikNo} kişi bulunamadı!";
                TNSCollection taahhutDetaylar = (TNSCollection)oa.Al(1);

                if (taahhutDetaylar == null || taahhutDetaylar.Count == 0)
                    return $"{sup.tcKimlikNo} kişiye ait bordro üretilemedi, fiyat farkını elle ekleyiniz!";

                ObjectArray oaMevcutTutarlar = servisMUH.TaahhutToplamListele(kullanan, taahhutMevcut.kod);
                if (!oaMevcutTutarlar.sonuc.islemSonuc)
                    return $"{sup.tcKimlikNo} kişiye ait taahhütün, ödeme planı okunurken hata oldu: {oaMevcutTutarlar.sonuc.hataStr}!";

                if (oaMevcutTutarlar.ObjeSayisi == 0)
                    return $"{sup.tcKimlikNo} kişiye ait taahhütün, ödeme planı bulunamadı!";

                double toplam = 0;
                TNSCollection taahhutBilgiler = new TNSCollection();
                TaahhutBilgi tb = null;
                foreach (TNS.HRC.BordroMuhasebe item in taahhutDetaylar)
                {
                    DateTime taahhutTar = basTar.Oku();
                    for (int tNo = 0; tNo <= 12; tNo++)
                    {
                        tb = new TaahhutBilgi();
                        tb.yil = taahhutTar.Year;
                        tb.ay = taahhutTar.Month;
                        tb.tertipKod = TertipBul(yil, item.hesapKod);
                        tb.ilkTaahhutTutari = item.ToplamBul(tNo, tNo + 1).borc - SifirlaDondur(oaMevcutTutarlar.objeler, tb.yil, tb.ay, tb.tertipKod);

                        tb.digerTaahhutler = 0;
                        tb.fiyatFarki = 0;
                        tb.ilaveTaahhutler = 0;
                        if (tb.ilkTaahhutTutari != 0)
                        {
                            taahhutBilgiler.Add(tb);
                            toplam += tb.ilkTaahhutTutari;
                        }
                        taahhutTar = taahhutTar.AddMonths(1);
                    }
                }
                SifirOlmayanlariEkle(oaMevcutTutarlar, taahhutBilgiler, ref toplam, basTar);

                TaahhutKartiDegerArtisi artisSatir = null;
                foreach (TaahhutKartiDegerArtisi item in taahhutMevcut.degerArtisi.objeler)
                {
                    if (item.odemeEmriFormDurum < 5)
                        artisSatir = item;
                }
                if (artisSatir == null)
                {
                    artisSatir = new TaahhutKartiDegerArtisi();
                    artisSatir.taahhutKod = taahhutMevcut.kod;

                    artisSatir.kod = null;
                    artisSatir.onayTarihi.Yaz(DateTime.Now.Date);
                    artisSatir.sozlesmeTarihi.Yaz(DateTime.Now.Date);
                    artisSatir.damgaVergisiTarihi.Yaz(DateTime.Now.Date);
                    artisSatir.damgaVergisiTutari = 0;
                    artisSatir.damgaVergisiNo = "";

                    artisSatir.odemeEmriYil = 0;
                    artisSatir.odemeEmriNo = "";
                }
                artisSatir.sozlesmeTutari = 0;
                foreach (TaahhutBilgi item in taahhutBilgiler)
                    artisSatir.sozlesmeTutari += item.ilkTaahhutTutari;

                Sonuc s = servisMUH.TaahhutKartiDegerArtisiKaydet(kullanan, artisSatir, taahhutBilgiler);
                if (!s.islemSonuc)
                    return s.hataStr;
                return hataYoksaBos ? "" : "Sözleşme kaydedildi!";
            }
            else
            {
                if (oa.sonuc.islemSonuc)
                    return "Personel maaş hesaplamada bir sorun oluştu, tahakkuk kaydı oluşturulamayacak!";
                else
                    return oa.sonuc.hataStr;
            }
        }

        private static void SifirOlmayanlariEkle(ObjectArray oaMevcutTutarlar, TNSCollection taahhutBilgiler, ref double toplam, TNSDateTime basTar)
        {
            foreach (TaahhutBilgi item in oaMevcutTutarlar.objeler)
            {
                if (item.ilkTaahhutTutari != 0 && YilAyGecerli(item.yil, item.ay, basTar.Yil, basTar.Ay))
                {
                    item.ilkTaahhutTutari = 0 - item.ilkTaahhutTutari;
                    taahhutBilgiler.Add(item);
                }
            }
        }

        private static bool YilAyGecerli(int arananYil, int arananAy, int baslaYil, int baslaAy)
        {
            if (arananYil < baslaYil) return false;
            else if (arananYil > baslaYil) return true;
            else if (arananAy < baslaAy) return false;
            else return true;
        }

        private static double SifirlaDondur(TNSCollection taahhutBilgiler, int yil, int ay, string tertipKod)
        {
            foreach (TaahhutBilgi item in taahhutBilgiler)
            {
                if (item.tertipKod == tertipKod && item.yil == yil && item.ay == ay)
                {
                    double tut = item.ilkTaahhutTutari;
                    item.ilkTaahhutTutari = 0;
                    return tut;
                }
            }
            return 0;
        }

        // Eski SozlesmeOlusturIc: personelin bordro maliyetinden taahhüt kartı ve planı.
        private static string SozlesmeOlusturIc(Kullanici kullanan, TaahhutSozlesmeUretIstegi sup, bool hataYoksaBos)
        {
            TNSDateTime basTar = Tarih(sup.basTar);
            TNSDateTime bitTar = Tarih(sup.bitTar);
            int yil = basTar.Yil;
            ObjectArray oa = TNS.HRC.Arac.Tanimla().PersonelMaliyetHesapla(kullanan, sup.tcKimlikNo, basTar, bitTar, sup.sure);
            if (oa.sonuc.islemSonuc && oa.ObjeSayisi == 2)
            {
                TNS.IKY.Personel perBilgi = (TNS.IKY.Personel)oa.Al(0);
                if (perBilgi == null)
                    return $"{sup.tcKimlikNo} kişi bulunamadı!";
                TNSCollection taahhutDetaylar = (TNSCollection)oa.Al(1);

                if (taahhutDetaylar == null || taahhutDetaylar.Count == 0)
                    return $"{sup.tcKimlikNo} kişiye ait bordro üretilemedi, taahhütü elle ekleyiniz!";

                double toplam = 0;
                TaahhutKartiBilgi taahhut = new TaahhutKartiBilgi();
                TNSCollection taahhutBilgiler = new TNSCollection();
                TaahhutBilgi tb = null;
                foreach (TNS.HRC.BordroMuhasebe item in taahhutDetaylar)
                {
                    DateTime taahhutTar = basTar.Oku();
                    for (int tNo = 0; tNo <= 12; tNo++)
                    {
                        tb = new TaahhutBilgi();
                        tb.yil = taahhutTar.Year;
                        tb.ay = taahhutTar.Month;
                        tb.ilkTaahhutTutari = item.ToplamBul(tNo, tNo + 1).borc;
                        tb.tertipKod = TertipBul(yil, item.hesapKod);

                        tb.digerTaahhutler = 0;
                        tb.fiyatFarki = 0;
                        tb.ilaveTaahhutler = 0;
                        if (tb.ilkTaahhutTutari != 0)
                        {
                            taahhutBilgiler.Add(tb);
                            toplam += tb.ilkTaahhutTutari;
                        }
                        taahhutTar = taahhutTar.AddMonths(1);
                    }
                }

                taahhut.muhasebeKod = perBilgi.kadro.muhasebeKodu;
                taahhut.birimKod = perBilgi.kadro.muhasebeBirimi;
                taahhut.muteahhitKimlikNo = perBilgi.tcKimlikNo;
                taahhut.muteahhitUnvan = perBilgi.adSoyad;
                taahhut.muteahhitAdres = perBilgi.iletisim.adres;
                taahhut.konu = perBilgi.personelTipAd + " (" + perBilgi.unvanAd + ")";
                taahhut.sozTur = TaahhutKartiBilgi.ENUMSozTur.Personel;
                taahhut.kod = sup.sozKod;
                taahhut.projeSiraNo = "";
                taahhut.DptProjeSiraNo = "";
                taahhut.yil = yil;
                taahhut.aciklama = "Personel Sözleşmesi";
                taahhut.sozlesmeTarihi = basTar;

                taahhut.kesifBeldeli = toplam;
                taahhut.indirimOrani = 0;
                taahhut.ihaleBedeli = toplam;
                taahhut.gerceklesenTaahhutTutar = toplam;
                taahhut.damgaVergisiTarihi = null;
                taahhut.damgaVergisiNo = null;
                taahhut.damgaVergisiTutar = 0;
                taahhut.sozlesmeDVTarihi = null;
                taahhut.sozlesmeDVNo = null;
                taahhut.sozlesmeDVTutar = 0;
                taahhut.iseBaslamaTarihi = basTar;
                taahhut.isBitirmeTarihi = bitTar;
                taahhut.geciciKabulTarihi = null;
                taahhut.sureUzatimi1 = "";
                taahhut.sureUzatimi2 = "";
                taahhut.sureUzatimi3 = "";
                taahhut.odenekTertip = "";
                if (tb != null)
                    taahhut.odenekTertip = tb.tertipKod;
                taahhut.duranTertip = "";

                taahhut.kapandi = 0;
                taahhut.parabirimiKod = "USD";
                taahhut.garantiBaslamaTarihi = null;
                taahhut.garantiBitisTarihi = null;
                taahhut.isKod = 0;
                Sonuc s = Servis().TaahhutKartiKaydet(kullanan, taahhut, taahhutBilgiler);

                if (!s.islemSonuc)
                    return s.hataStr;
                return hataYoksaBos ? "" : "Sözleşme kaydedildi!";
            }
            else
            {
                if (oa.sonuc.islemSonuc)
                    return "Personel maaş hesaplamada bir sorun oluştu, tahakkuk kaydı oluşturulamayacak!";
                else
                    return oa.sonuc.hataStr;
            }
        }

        private static string TertipBul(int yil, string hesapKod)
        {
            HesapPlaniSatir h = new HesapPlaniSatir();
            h.ekoBasla = 1;
            h.hesapKod = hesapKod;
            return "13.99.01.01-09.4.1.00-2-" + TNS.MUH.Arac.HesapkoddanEkonomikKodBul(yil, h, "99.9.9.99") + "-01.01";
        }

        // ------------------------------------------------------------------ döviz (eski ButceMuhasebe/Ortak.cs; istemci katmanı)

        /// <summary>Eski Ortak.KurDegeri: Merkez Bankası alış kuru (bulunamazsa 1).</summary>
        internal static decimal KurDegeri(Kullanici kullanan, string paraBirimiKod, TNSDateTime tarih)
        {
            decimal kurDegeri = 1;
            if (tarih == null || tarih.isNull) tarih = new TNSDateTime(DateTime.Now.ToString());

            TNS.HRC.MerkezBankasiKurSinif kriter = new TNS.HRC.MerkezBankasiKurSinif();
            kriter.kurTarih = tarih;
            TNS.HRC.MerkezBankasiKurSinif kurListe = ServisHRC().MerkezBankKurOku(kullanan, kriter);
            foreach (TNS.HRC.MerkezBankasiKurDetaySinif kur in kurListe.satirlar)
            {
                if (kur.paraBirim.kisaltma == paraBirimiKod)
                    kurDegeri = kur.DegerBul("ALIS", -1);
            }
            return kurDegeri;
        }

        /// <summary>Eski Ortak.OdemeEmriDetayDovizDonustur: dövizli hesap satırlarına döviz tutarı yazılır, tutarlar kurla çarpılır.</summary>
        internal static TNS.MUH.OdemeEmriMIFForm OdemeEmriDetayDovizDonustur(IMUHServis servisMUH, Kullanici kullanan, TNS.MUH.OdemeEmriMIFForm tf, decimal kur)
        {
            string hesapKod = "";
            bool islemYapma = false;
            foreach (TNS.MUH.OdemeEmriMIFDetay detay in tf.detay.objeler)
            {
                if (hesapKod != "") hesapKod += ";";
                hesapKod += detay.hesapKod;

                if (detay.dovizBorc > 0 || detay.dovizAlacak > 0)
                {
                    islemYapma = true;
                    break;
                }
            }

            // Döviz borç / alacak gelmişse belge zaten döviz işlemi yapılarak gönderilmiştir.
            if (islemYapma) return tf;

            TNS.MUH.HesapPlaniSatir hs = new TNS.MUH.HesapPlaniSatir();
            hs.yil = tf.yil;
            hs.hesapKod = hesapKod;
            ObjectArray hListe = servisMUH.HesapPlaniListele(kullanan, hs);

            Hashtable htHesap = new Hashtable();
            foreach (TNS.MUH.HesapPlaniSatir item in hListe.objeler)
                htHesap[item.hesapKod.Replace(".", "")] = item;

            foreach (TNS.MUH.OdemeEmriMIFDetay detay in tf.detay.objeler)
            {
                if (htHesap[detay.hesapKod.Replace(".", "")] == null) continue;

                TNS.MUH.HesapPlaniSatir hsp = (TNS.MUH.HesapPlaniSatir)htHesap[detay.hesapKod.Replace(".", "")];
                if (hsp.dovizKurKod != "")
                {
                    detay.dovizBorc = detay.borc;
                    detay.dovizAlacak = detay.alacak;
                    detay.dovizKur = kur;
                    detay.dovizKurTur = hsp.dovizKurTurAlis;
                    detay.doviz = hsp.dovizKurKod;
                }

                detay.borc = Math.Round(detay.borc * OrtakFonksiyonlar.ConvertToDbl(kur), 2);
                detay.alacak = Math.Round(detay.alacak * OrtakFonksiyonlar.ConvertToDbl(kur), 2);
            }
            return tf;
        }

        // ------------------------------------------------------------------ teminat / avans / borç / ödeme emri pencereleri

        /// <summary>Eski TeminatlarListele.</summary>
        private static IResult TeminatListe(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Odeme(ortam), kullanan =>
            {
                TaahhutTeminatlar b = new TaahhutTeminatlar();
                b.taahhutKod = kod ?? 0;
                ObjectArray liste = Servis().TaahhutTeminatlarListele(kullanan, b);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata("İşlem sırasında hata oluştu." + (liste?.sonuc?.hataStr ?? ""));
                List<object> satirlar = new List<object>();
                foreach (TaahhutTeminatlar t in liste.objeler)
                    satirlar.Add(new { TAAHHUTKOD = t.taahhutKod, YIL = t.yil, DOSYANO = t.dosyaNo ?? "", TARIH = TarihYaz(t.tarih), NUMARA = t.numara ?? "", ACIKLAMA = t.aciklama ?? "", TUTAR = t.tutar });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski TaahhutSecimAlinanTeminatlar.aspx Listele: firmanın teminat mektupları (sözleşme yılı).</summary>
        private static IResult TeminatSecim(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Odeme(ortam), kullanan =>
            {
                TaahhutKartiBilgi t = KartOku(kullanan, kod ?? 0, out string hata);
                if (t == null) return Hata("Taahhüt kartı bilgisi alınamadı.");
                TNS.HRC.FirmaBilgisi fir = ServisHRC().FirmaBilgisiListele(kullanan, 0, t.muteahhitKimlikNo);
                if (fir == null || fir.kod <= 0) return Hata("Müteahhidin firma kaydı bulunamadı.");

                TNS.HRC.TeminatMektubu bilgi = new TNS.HRC.TeminatMektubu();
                bilgi.yil = t.sozlesmeTarihi == null || t.sozlesmeTarihi.isNull ? 0 : t.sozlesmeTarihi.Yil;
                bilgi.giris.firma.kod = fir.kod;
                bilgi.giris.firma.vergiNo = t.muteahhitKimlikNo;
                ObjectArray liste = ServisHRC().TeminatMektubuListele(kullanan, bilgi);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata("İşlem sırasında hata oluştu." + (liste?.sonuc?.hataStr ?? ""));
                List<object> satirlar = new List<object>();
                foreach (TNS.HRC.TeminatMektubu m in liste.objeler)
                    satirlar.Add(new
                    {
                        YIL = m.yil,
                        DOSYANO = m.dosyaNo ?? "",
                        TARIH = TarihYaz(m.giris.dosyaTarih),
                        NUMARA = m.giris.numara ?? "",
                        ACIKLAMA = m.giris.alinisSebebi ?? "",
                        TUTAR = m.giris.tutar
                    });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski TaahhutSecimAlinanTeminatlar.aspx btnKaydet_Click.</summary>
        private static Task<IResult> TeminatKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutPencereIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Listeden ilgili kaydı seçip sonra kaydet düğmesine basınız.");
                ObjectArray liste = new ObjectArray();
                foreach (TaahhutPencereSatiri s in istek.satirlar)
                {
                    TaahhutTeminatlar t = new TaahhutTeminatlar();
                    t.yil = s.YIL;
                    t.taahhutKod = istek.kod;
                    t.dosyaNo = s.DOSYANO ?? "";
                    t.tarih = Tarih(s.TARIH);
                    t.numara = s.NUMARA ?? "";
                    t.aciklama = s.ACIKLAMA ?? "";
                    t.tutar = s.TUTAR;
                    t.islemTarihi = new TNSDateTime(DateTime.Now);
                    t.islemYapanKod = kullanan.TCKIMLIKNO;
                    t.islemYapanAd = kullanan.ADSOYAD;
                    liste.Ekle(t);
                }
                Sonuc sonuc = Servis().TaahhutTeminatlarKaydet(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Tamam("Bilgiler kayıt edildi.");
            });
        }

        /// <summary>Eski btnTeminatCikar_Click.</summary>
        private static Task<IResult> TeminatSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutPencereIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Silinecek bilgi bulunamadı.");
                ObjectArray liste = new ObjectArray();
                foreach (TaahhutPencereSatiri s in istek.satirlar)
                {
                    TaahhutTeminatlar form = new TaahhutTeminatlar();
                    form.taahhutKod = istek.kod;
                    form.yil = s.YIL;
                    form.dosyaNo = s.DOSYANO ?? "";
                    liste.objeler.Add(form);
                }
                Sonuc sonuc = Servis().TaahhutTeminatlarSil(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata("Kayit silinemedi.");
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // Eski avans listesi: kayıtlı avanslar + muhasebede 16x / 259 hesaplı (yevmiyesi verilmiş) belgeler "Otomatik eklendi" olarak.
        private static ObjectArray AvansBelgeleri(Kullanici kullanan, TaahhutKartiBilgi t, bool projeNoIle)
        {
            TNS.MUH.OdemeEmriMIFForm f = new TNS.MUH.OdemeEmriMIFForm();
            TNS.MUH.OdemeEmriMIFDetay detay = new TNS.MUH.OdemeEmriMIFDetay(DateTime.Now.Year);
            f.yil = t.sozlesmeTarihi == null || t.sozlesmeTarihi.isNull ? 0 : t.sozlesmeTarihi.Yil;
            f.ilgiliNo = t.muteahhitKimlikNo ?? "";
            if (projeNoIle) f.projeNo = t.projeSiraNo ?? "";
            f.formDurum = (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER;
            detay.hesapKod = "16;259";
            f.detay.objeler.Add(detay);
            return Servis().OdemeEmriMIFListele(kullanan, f, true);
        }

        private static void Toplamlar(TNS.MUH.OdemeEmriMIFForm t, out double borcToplam, out double alacakToplam)
        {
            borcToplam = 0;
            alacakToplam = 0;
            foreach (TNS.MUH.OdemeEmriMIFDetay d in t.detay.objeler)
            {
                borcToplam += d.borc;
                alacakToplam += d.alacak;
            }
        }

        /// <summary>Eski AvanslarListele.</summary>
        private static IResult AvansListe(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Odeme(ortam), kullanan =>
            {
                int taahhutKod = kod ?? 0;
                List<object> satirlar = new List<object>();
                TaahhutAvanslar b = new TaahhutAvanslar();
                b.taahhutKod = taahhutKod;
                ObjectArray liste = Servis().TaahhutAvanslarListele(kullanan, b);
                if (liste != null && liste.sonuc.islemSonuc)
                    foreach (TaahhutAvanslar a in liste.objeler)
                        satirlar.Add(new { TAAHHUTKOD = a.taahhutKod, YIL = a.yil, BELGENO = a.belgeNo ?? "", TARIH = TarihYaz(a.tarih), NUMARA = a.numara ?? "", VERILENAVANS = a.verilenAvans, MAHSUPEDILENAVANS = a.mahsupEdilenAvans, OTOMATIK = false });

                TaahhutKartiBilgi t = KartOku(kullanan, taahhutKod, out string _);
                if (t != null)
                {
                    ObjectArray kayitsizListe = AvansBelgeleri(kullanan, t, true);
                    if (kayitsizListe != null && kayitsizListe.objeler != null)
                        foreach (TNS.MUH.OdemeEmriMIFForm f in kayitsizListe.objeler)
                        {
                            Toplamlar(f, out double borc, out double alacak);
                            satirlar.Add(new { TAAHHUTKOD = taahhutKod, YIL = f.yil, BELGENO = f.belgeNo ?? "", TARIH = TarihYaz(f.YEVYevTarih), NUMARA = f.YEVYevNo + " (Otomatik eklendi)", VERILENAVANS = borc, MAHSUPEDILENAVANS = alacak, OTOMATIK = true });
                        }
                }
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski TaahhutSecimVerilenAvanslar.aspx Listele.</summary>
        private static IResult AvansSecim(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Odeme(ortam), kullanan =>
            {
                TaahhutKartiBilgi t = KartOku(kullanan, kod ?? 0, out string _);
                if (t == null) return Hata("Taahhüt kartı bilgisi alınamadı.");
                ObjectArray liste = AvansBelgeleri(kullanan, t, false);
                bool tamam = liste != null && (liste.sonuc.islemSonuc || liste.sonuc.hataStr == "Verilen kriterlere uygun kayıt bulunamadı.");
                if (!tamam) return Hata("Kayıt bulunamadı." + (liste?.sonuc?.hataStr ?? ""));
                List<object> satirlar = new List<object>();
                foreach (TNS.MUH.OdemeEmriMIFForm f in liste.objeler)
                {
                    Toplamlar(f, out double borc, out double alacak);
                    satirlar.Add(new { YIL = f.yil, BELGENO = f.belgeNo ?? "", TARIH = TarihYaz(f.YEVYevTarih), NUMARA = f.YEVYevNo.ToString(), VERILENAVANS = borc, MAHSUPEDILENAVANS = alacak });
                }
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski TaahhutSecimVerilenAvanslar.aspx btnKaydet_Click.</summary>
        private static Task<IResult> AvansKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutPencereIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Listeden ilgili kaydı seçip sonra kaydet düğmesine basınız.");
                ObjectArray liste = new ObjectArray();
                foreach (TaahhutPencereSatiri s in istek.satirlar)
                {
                    TaahhutAvanslar t = new TaahhutAvanslar();
                    t.yil = s.YIL;
                    t.taahhutKod = istek.kod;
                    t.belgeNo = s.BELGENO ?? "";
                    if (!string.IsNullOrEmpty(s.TARIH)) t.tarih = Tarih(s.TARIH);
                    t.numara = s.NUMARA ?? "";
                    t.verilenAvans = s.VERILENAVANS;
                    t.mahsupEdilenAvans = s.MAHSUPEDILENAVANS;
                    t.islemTarihi = new TNSDateTime(DateTime.Now);
                    t.islemYapanKod = kullanan.TCKIMLIKNO;
                    t.islemYapanAd = kullanan.ADSOYAD;
                    liste.Ekle(t);
                }
                Sonuc sonuc = Servis().TaahhutAvanslarKaydet(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Tamam("Bilgiler kayıt edildi.");
            });
        }

        /// <summary>Eski btnAvansCikar_Click.</summary>
        private static Task<IResult> AvansSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutPencereIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Silinecek bilgi bulunamadı.");
                ObjectArray liste = new ObjectArray();
                foreach (TaahhutPencereSatiri s in istek.satirlar)
                {
                    TaahhutAvanslar form = new TaahhutAvanslar();
                    form.taahhutKod = istek.kod;
                    form.yil = s.YIL;
                    form.belgeNo = s.BELGENO ?? "";
                    liste.objeler.Add(form);
                }
                Sonuc sonuc = Servis().TaahhutAvanslarSil(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata("Kayit silinemedi.");
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        /// <summary>Eski BorcListeYenile.</summary>
        private static IResult BorcListe(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Odeme(ortam), kullanan =>
            {
                TaahhutBorclar b = new TaahhutBorclar();
                b.taahhutKod = kod ?? 0;
                ObjectArray liste = Servis().TaahhutBorclarListele(kullanan, b);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata("İşlem sırasında hata oluştu." + (liste?.sonuc?.hataStr ?? ""));
                List<object> satirlar = new List<object>();
                foreach (TaahhutBorclar t in liste.objeler)
                    satirlar.Add(new { KOD = t.kod, TAAHHUTKOD = t.taahhutKod, ALACAKLIADSOYAD = t.alacakliAdSoyad ?? "", DOSYANO = t.dosyaNo, TEMLIKACANVERGINO = t.temlikAcanVergiNo ?? "", TEMLIKVERENVERGINO = t.temlikVerenVergiNo ?? "", TEMLIKTUTARI = t.temlikTutari });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski TaahhutSecimBorclarVeTemlikler.aspx Listele: müteahhidin temlik tanımları.</summary>
        private static IResult BorcSecim(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Odeme(ortam), kullanan =>
            {
                TaahhutKartiBilgi t = KartOku(kullanan, kod ?? 0, out string _);
                if (t == null) return Hata("Taahhüt kartı bilgisi alınamadı.");
                TaahhutBorclarTanim bilgi = new TaahhutBorclarTanim();
                bilgi.temlikVerenVergiNo = t.muteahhitKimlikNo ?? "";
                ObjectArray liste = Servis().TaahhutBorclarTanimListele(kullanan, bilgi);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata("İşlem sırasında hata oluştu." + (liste?.sonuc?.hataStr ?? ""));
                List<object> satirlar = new List<object>();
                foreach (TaahhutBorclarTanim b in liste.objeler)
                    satirlar.Add(new
                    {
                        KOD = b.kod,
                        KONU = b.konu ?? "",
                        ALACAKLIADSOYAD = b.alacakliAdSoyad ?? "",
                        TEMLIKACANVERGINO = b.temlikAcanVergiNo ?? "",
                        TEMLIKVERENVERGINO = b.temlikVerenVergiNo ?? "",
                        TEMLIKTUTARI = b.temlikTutari,
                        ACIKLAMA = b.aciklama ?? "",
                        NOTERONAYTARIHI = TarihYaz(b.noterOnayTarihi),
                        NOTERONAYNO = b.noterOnayNo ?? ""
                    });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski TaahhutSecimBorclarVeTemlikler.aspx btnKaydet_Click (kod ve dosya no temlik tanımının kodudur).</summary>
        private static Task<IResult> BorcKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutPencereIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Listeden ilgili kaydı seçip sonra kaydet düğmesine basınız.");
                ObjectArray liste = new ObjectArray();
                foreach (TaahhutPencereSatiri s in istek.satirlar)
                {
                    TaahhutBorclar t = new TaahhutBorclar();
                    t.kod = s.KOD;
                    t.taahhutKod = istek.kod;
                    t.dosyaNo = s.KOD;
                    t.alacakliAdSoyad = s.ALACAKLIADSOYAD ?? "";
                    t.temlikAcanVergiNo = s.TEMLIKACANVERGINO ?? "";
                    t.temlikVerenVergiNo = s.TEMLIKVERENVERGINO ?? "";
                    t.temlikTutari = s.TEMLIKTUTARI;
                    t.islemYapanAd = kullanan.ADSOYAD;
                    t.islemYapanKod = kullanan.TCKIMLIKNO;
                    t.islemTarihi = new TNSDateTime(DateTime.Now);
                    liste.Ekle(t);
                }
                Sonuc sonuc = Servis().TaahhutBorclarKaydet(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Tamam("Bilgiler kayıt edildi.");
            });
        }

        /// <summary>Eski btnBorcCikar_Click.</summary>
        private static Task<IResult> BorcSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutPencereIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Silinecek bilgi bulunamadı.");
                ObjectArray liste = new ObjectArray();
                foreach (TaahhutPencereSatiri s in istek.satirlar)
                {
                    TaahhutBorclar form = new TaahhutBorclar();
                    form.kod = s.KOD;
                    liste.objeler.Add(form);
                }
                Sonuc sonuc = Servis().TaahhutBorclarSil(kullanan, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata("Kayit silinemedi.");
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        /// <summary>Eski OdemeEmriListeYenile (hakediş ödeme emirleri).</summary>
        private static IResult OdemeEmriListe(HttpContext baglam, IWebHostEnvironment ortam, int? kod)
        {
            return Oku(baglam, Odeme(ortam), kullanan =>
            {
                TaahhutOdeme odeme = new TaahhutOdeme();
                odeme.taahhutKod = kod ?? 0;
                ObjectArray liste = Servis().TaahhutOdemeEmriListele(kullanan, odeme);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata("İşlem sırasında hata oluştu." + (liste?.sonuc?.hataStr ?? ""));
                List<object> satirlar = new List<object>();
                foreach (TaahhutOdeme t in liste.objeler)
                {
                    int odemeEmriYil = t.odemeEmriTarihi == null || t.odemeEmriTarihi.isNull ? 0 : t.odemeEmriTarihi.Oku().Year;
                    satirlar.Add(new
                    {
                        KOD = t.kod,
                        TAAHHUTKOD = t.taahhutKod,
                        ODEMEEMRIKOD = t.odemeEmriKod ?? "",
                        ODEMEEMRIYIL = odemeEmriYil,
                        ODEMEEMRITARIHI = TarihYaz(t.odemeEmriTarihi),
                        ACIKLAMA = t.aciklama ?? "",
                        HAKEDISTUTARI = t.hakedisTutari,
                        HAKEDISHESAP = t.hakedisHesap ?? "",
                        ODEMEHESAP = t.odemeHesap ?? "",
                        YANSITMAHESAP = t.yansitmaHesap ?? "",
                        DAMGAVERGISI = t.damgaVergisi,
                        GELIRVERGISISTOPAJI = t.gelirVergisiStopaji,
                        FONPAYI = t.fonPayi,
                        PARACEZALARI = t.paraCezalari,
                        MAHSUPEDILENAVANS = t.mahsupEdilenAvans,
                        ICRAKESINTISI = t.icraKesintisi,
                        TEMINATKESINTISI = t.teminatKesintisi,
                        SSKPRIMIVEGECIKMEZAMMI = t.sskPrimiVeGecikmeZammi,
                        VERGIBORCU = t.vergiBorcu,
                        KDVTEVKIFATI = t.kdvTevkifati,
                        TEMLIK = t.temlik,
                        BUTCEEMANETI = t.butceEmaneti,
                        KDV = t.kdv,
                        KESINTILERTOPLAMI = t.kesintilerToplami,
                        YAPILANODEME = t.yapilanOdeme,
                        ISLEMYAPANAD = t.islemYapanAd ?? ""
                    });
                }
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski btnSil_Click (ödeme emri penceresi): seçili satırlar tek tek silinir.</summary>
        private static Task<IResult> OdemeEmriSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Odeme(ortam), kullanan =>
            {
                if (istek == null || istek.kodlar == null || istek.kodlar.Count == 0) return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");
                List<string> hatalar = new List<string>();
                foreach (int kod in istek.kodlar)
                {
                    Sonuc sonuc = Servis().TaahhutOdemeEmriSil(kullanan, kod);
                    if (sonuc == null || !sonuc.islemSonuc) hatalar.Add(HataMetni(sonuc?.hataStr, kod + " kodlu kayıt silinemedi."));
                }
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar));
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ taahhüt planı (eski Taahhutler.aspx)

        private static IResult PlanListe(HttpContext baglam, IWebHostEnvironment ortam, int? kod, string dakod)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                if ((kod ?? 0) <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                TaahhutBilgi taahhut = new TaahhutBilgi();
                taahhut.taahhutKod = kod.Value;
                taahhut.degerArtisKod = string.IsNullOrWhiteSpace(dakod) ? null : dakod.Trim();
                ObjectArray liste = Servis().TaahhutListele(kullanan, taahhut);
                if (liste == null || !liste.sonuc.islemSonuc) return Hata("İşlem sırasında hata oluştu." + (liste?.sonuc?.hataStr ?? ""));
                List<object> satirlar = new List<object>();
                foreach (TaahhutBilgi t in liste.objeler)
                    satirlar.Add(new
                    {
                        kod = t.kod,
                        yil = t.yil,
                        ay = t.ay,
                        tertipKod = t.tertipKod ?? "",
                        ilkTaahhutTutari = t.ilkTaahhutTutari,
                        fiyatFarki = t.fiyatFarki,
                        ilaveTaahhutler = t.ilaveTaahhutler,
                        digerTaahhutler = t.digerTaahhutler,
                        toplam = t.toplam
                    });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski Taahhutler.aspx btnKaydet_Click: değişen / yeni satırlar.</summary>
        private static Task<IResult> PlanKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutPlanIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                if (istek.satirlar == null || istek.satirlar.Count == 0) return Hata("Kayıt edilecek bilgi bulunamadı.");
                List<TaahhutBilgi> l = new List<TaahhutBilgi>();
                foreach (TaahhutPlanSatiri s in istek.satirlar)
                {
                    TaahhutBilgi t = new TaahhutBilgi();
                    t.kod = s.kod;
                    t.taahhutKod = istek.kod;
                    t.yil = s.yil;
                    t.ay = s.ay;
                    t.tertipKod = s.tertipKod ?? "";
                    t.ilkTaahhutTutari = s.ilkTaahhutTutari;
                    t.fiyatFarki = s.fiyatFarki;
                    t.ilaveTaahhutler = s.ilaveTaahhutler;
                    t.digerTaahhutler = s.digerTaahhutler;
                    l.Add(t);
                }
                TNSCollection liste = new TNSCollection(l.ToArray());
                string daKod = string.IsNullOrWhiteSpace(istek.daKod) ? null : istek.daKod.Trim();
                Sonuc sonuc = Servis().TaahhutKaydet(kullanan, istek.kod, daKod, liste);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Tamam("Bilgiler kayıt edildi.");
            });
        }

        /// <summary>Eski Taahhutler.aspx btnSil_Click.</summary>
        private static Task<IResult> PlanSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.kodlar == null || istek.kodlar.Count == 0) return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");
                List<string> hatalar = new List<string>();
                foreach (int kod in istek.kodlar)
                {
                    Sonuc sonuc = Servis().TaahhutSil(kullanan, kod);
                    if (sonuc == null || !sonuc.islemSonuc) hatalar.Add(HataMetni(sonuc?.hataStr, "Kayıt silinemedi."));
                }
                if (hatalar.Count > 0) return Hata(string.Join("\n", hatalar));
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ bakım tarihçesi (eski TaahhutBakimTarihce.aspx)

        private static IResult BakimListe(HttpContext baglam, IWebHostEnvironment ortam, int? taahhutKod)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                TaahhutKartiBakimTarihce kriter = new TaahhutKartiBakimTarihce();
                kriter.taahhutKartiKod = taahhutKod ?? 0;
                ObjectArray yler = Servis().TaahhutKartiBakimTarihceListele(kullanan, kriter);
                if (yler == null || !yler.sonuc.islemSonuc) return Hata(HataMetni(yler?.sonuc?.hataStr, "Liste okunamadı."));
                List<object> satirlar = new List<object>();
                foreach (TaahhutKartiBakimTarihce f in yler.objeler)
                    satirlar.Add(new
                    {
                        kod = f.kod,
                        taahhutKartiKod = f.taahhutKartiKod,
                        girisTarihi = TarihYaz(f.girisTarihi),
                        girisAciklama = f.girisAciklama ?? ""
                    });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        private static Task<IResult> BakimKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutBakimIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.taahhutKod <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                TaahhutKartiBakimTarihce form = new TaahhutKartiBakimTarihce();
                form.kod = istek.kod;
                form.taahhutKartiKod = istek.taahhutKod;
                form.girisTarihi = Tarih(istek.girisTarihi);
                form.girisAciklama = istek.girisAciklama ?? "";
                Sonuc sonuc = Servis().TaahhutKartiBakimTarihceKaydet(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi başarıyla tamamlandı.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, istek.kod) });
            });
        }

        private static Task<IResult> BakimSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Lütfen silmek istediğiniz bilgiyi seçin.");
                Sonuc sonuc = Servis().TaahhutKartiBakimTarihceSil(kullanan, new TaahhutKartiBakimTarihce { kod = istek.kod });
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Silinemedi."));
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ sözleşme koşulları / garanti / teminat şartları (eski TanimSozlesme.aspx)

        private static IResult SozlesmeMetni(HttpContext baglam, IWebHostEnvironment ortam, int? taahhutKod, int? tur)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                TaahhutKartiSozlesme form = new TaahhutKartiSozlesme();
                form.taahhutKartiKod = taahhutKod ?? 0;
                form.tur = (ENUMSozlesmeTur)(tur ?? 0);
                ObjectArray liste = Servis().TaahhutKartiSozlesmeListele(kullanan, form);
                if (liste != null && liste.sonuc.islemSonuc && liste.objeler.Count > 0)
                {
                    form = (TaahhutKartiSozlesme)liste.objeler[0];
                    return Results.Json(new { kod = form.kod, aciklama = form.aciklama ?? "" });
                }
                return Results.Json(new { kod = 0, aciklama = "" });
            });
        }

        private static Task<IResult> SozlesmeMetniKaydet(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutSozlesmeMetniIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.taahhutKod <= 0) return Hata("Taahhüt kartı bilgisi alınamadı.");
                TaahhutKartiSozlesme form = new TaahhutKartiSozlesme();
                form.kod = istek.kod;
                form.taahhutKartiKod = istek.taahhutKod;
                form.tur = (ENUMSozlesmeTur)istek.tur;
                form.maddeNo = 0;
                form.ad = "";
                form.aciklama = istek.aciklama ?? "";
                Sonuc sonuc = Servis().TaahhutKartiSozlesmeKaydet(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Kaydedilemedi."));
                return Results.Json(new { basarili = true, bilgi = "Kayıt işlemi tamamlandı.", kod = OrtakFonksiyonlar.ConvertToInt(sonuc.anahtar, istek.kod) });
            });
        }

        private static Task<IResult> SozlesmeMetniSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutKodIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || istek.kod <= 0) return Hata("Silinecek kayıt bulunamadı");
                TaahhutKartiSozlesme form = new TaahhutKartiSozlesme();
                form.kod = istek.kod;
                Sonuc sonuc = Servis().TaahhutKartiSozlesmeSil(kullanan, form);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Silinemedi."));
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ belge ekle (eski TanimBelgeEkle.aspx, OrtakBelge)

        /// <summary>Belgelerin saklandığı ortak klasör (eski web.config OrtakDosyaYol; App.config appSettings).</summary>
        private static string OrtakDosyaYol() { return Ayar.Oku("OrtakDosyaYol", ""); }

        private static IResult EkBelgeListe(HttpContext baglam, IWebHostEnvironment ortam, string ilgiKod, int? tip)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                List<object> satirlar = new List<object>();
                if (string.IsNullOrWhiteSpace(ilgiKod)) return Results.Json(new { satirlar = satirlar });
                OrtakBelge belge = new OrtakBelge();
                belge.kod = "";
                belge.ilgiKod = ilgiKod.Trim();
                belge.tip = (ENUMOrtakBelgeTip)(tip ?? 0);
                ObjectArray belgeler = Servis().OrtakBelgeListele(kullanan, belge);
                if (belgeler != null && belgeler.objeler != null)
                    foreach (OrtakBelge d in belgeler.objeler)
                        satirlar.Add(new { KOD = d.kod ?? "", ILGIKOD = d.ilgiKod ?? "", TIP = (int)d.tip, BELGEADI = d.belgeAdi ?? "" });
                return Results.Json(new { satirlar = satirlar });
            });
        }

        /// <summary>Eski BelgeKaydet: seçilen dosyalar sırayla kaydedilir.</summary>
        private static async Task<IResult> EkBelgeYukle(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik)
        {
            if (!await sahtecilik.IsRequestValidAsync(baglam)) return Results.BadRequest();
            IResult ret = Denetle(baglam, Genel(ortam), out Kullanici kullanan);
            if (ret != null) return ret;
            if (!baglam.Request.HasFormContentType) return Results.BadRequest();
            IFormCollection istek = await baglam.Request.ReadFormAsync();
            string ilgiKod = Temiz(istek["ilgiKod"]);
            int tip = OrtakFonksiyonlar.ConvertToInt(Temiz(istek["tip"]), 0);
            if (ilgiKod == "") return Hata("Önce taahhüt kartını kaydedin.");
            if (istek.Files.Count == 0) return Hata("Lütfen yüklenecek dosyayı seçin.");
            try
            {
                string hata = "";
                string ortakYol = OrtakDosyaYol();
                foreach (IFormFile dosya in istek.Files)
                {
                    int boyut = (int)dosya.Length;
                    if (boyut <= 0) continue;
                    OrtakBelge yeni = new OrtakBelge(System.IO.Path.GetFileName(dosya.FileName), boyut);
                    yeni.ilgiKod = ilgiKod;
                    yeni.tip = (ENUMOrtakBelgeTip)tip;
                    using (System.IO.MemoryStream bellek = new System.IO.MemoryStream())
                    {
                        await dosya.CopyToAsync(bellek);
                        yeni.icerik = bellek.ToArray();
                    }
                    yeni.kayitEdilecekDosyaYol = ortakYol;
                    Sonuc sonuc = Servis().OrtakBelgeKaydet(kullanan, yeni);
                    if (sonuc == null || !sonuc.islemSonuc) hata += HataMetni(sonuc?.hataStr, yeni.belgeAdi + " kaydedilemedi.") + "\n";
                }
                if (hata != "") return Hata(hata.Trim());
                return Tamam("Belgeler başarıyla kaydedildi");
            }
            catch (Exception)
            {
                return Hata("Belgeler kaydedilemedi.");
            }
        }

        /// <summary>Eski BelgeGoster: dosya ortak klasörden (OrtakDosyaYol\tip\kod) okunur.</summary>
        private static IResult EkBelgeIndir(HttpContext baglam, IWebHostEnvironment ortam, string kod)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                if (string.IsNullOrWhiteSpace(kod)) return Hata("Belge seçiniz.");
                OrtakBelge dokuman = new OrtakBelge();
                dokuman.kod = kod.Trim();
                dokuman = Servis().OrtakBelgeOku(kullanan, dokuman);
                if (dokuman == null) return Hata("Belge bulunamadı.");
                dokuman.kayitEdilecekDosyaYol = OrtakDosyaYol();
                string dosya = System.IO.Path.Combine(dokuman.kayitEdilecekDosyaYol, ((int)dokuman.tip).ToString(), dokuman.kod ?? "");
                if (System.IO.File.Exists(dosya)) dokuman.icerik = System.IO.File.ReadAllBytes(dosya);
                if (dokuman.icerik == null) return Hata("Belge dosyası bulunamadı.");
                string ad = System.IO.Path.GetFileName(string.IsNullOrEmpty(dokuman.belgeAdi) ? "belge" : dokuman.belgeAdi);
                baglam.Response.Headers["X-Content-Type-Options"] = "nosniff";
                return Results.File(dokuman.icerik, "application/octet-stream", ad);
            }, "Belge okunamadı.");
        }

        private static Task<IResult> EkBelgeSil(HttpContext baglam, IWebHostEnvironment ortam, IAntiforgery sahtecilik, TaahhutBelgeSilIstegi istek)
        {
            return Yaz(baglam, sahtecilik, Genel(ortam), kullanan =>
            {
                if (istek == null || string.IsNullOrWhiteSpace(istek.kod)) return Hata("Belge seçiniz.");
                OrtakBelge belge = new OrtakBelge();
                belge.kod = istek.kod.Trim();
                belge.kayitEdilecekDosyaYol = OrtakDosyaYol();
                Sonuc sonuc = Servis().OrtakBelgeSil(kullanan, belge);
                if (sonuc == null || !sonuc.islemSonuc) return Hata(HataMetni(sonuc?.hataStr, "Silinemedi."));
                return Tamam("Silme işlemi başarıyla gerçekleşti.");
            });
        }

        // ------------------------------------------------------------------ hesap kodu (Sgb.hesapKodu / Sgb.hesapPlaniSec)

        private static int HesapYil(int? yil)
        {
            int y = yil ?? 0;
            return y <= 2000 ? DateTime.Now.Year : y;
        }

        private static object HesapDugum(HesapPlaniSatir h)
        {
            return new { kod = h.hesapKod, ad = h.hesapKod + " - " + h.aciklama, hesapAd = h.aciklama ?? "", altVar = !h.detay, detay = h.detay };
        }

        private static IResult HesapAra(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string metin, string sonKirilim)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                metin = Temiz(metin);
                if (metin.Length < 2) return Results.Json(new { satirlar = new List<object>() });
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
            }, "Arama yapılamadı.");
        }

        private static IResult HesapAd(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string kod)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
            {
                kod = Temiz(kod).Split('-')[0].Trim();
                if (kod == "") return Results.Json(new { kod = "", ad = "", detay = false });
                int y = HesapYil(yil);
                string hesYapi = TNS.MUH.Arac.DegiskenDegerBul(y, "/KODYAPI/KOD8/KODYAPI");
                if (!string.IsNullOrEmpty(hesYapi)) kod = OrtakFonksiyonlar.KodNoktaKoy(hesYapi, kod.Replace(".", ""));
                ObjectArray hv = Servis().HesapPlaniListele(kullanan, new HesapPlaniSatir { yil = y, hesapKod = kod });
                if (hv != null && hv.objeler != null)
                    foreach (HesapPlaniSatir d in hv.objeler)
                        if (d.hesapKod == kod) return Results.Json(new { kod = d.hesapKod, ad = d.aciklama ?? "", detay = d.detay });
                return Results.Json(new { kod = kod, ad = "", detay = false });
            }, "Hesap kodu okunamadı.");
        }

        private static IResult HesapAgac(HttpContext baglam, IWebHostEnvironment ortam, int? yil, string ust)
        {
            return Oku(baglam, Genel(ortam), kullanan =>
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
                string[] kirilimlar = ust.Split('.');
                HesapPlaniSatir hs = new HesapPlaniSatir();
                hs.yil = y;
                hs.hesapKod = ust;
                hs.seviye = ust.Length < 3 ? 1 : kirilimlar.Length + 1;
                ObjectArray hv = new ObjectArray();
                for (int i = 0; i < 10; i++)
                {
                    hv = Servis().HesapPlaniListele(new Kullanici(), hs);
                    if (hv.objeler.Count == 0) hs.seviye++;
                    else break;
                }
                foreach (HesapPlaniSatir h in hv.objeler) dugumler.Add(HesapDugum(h));
                return Results.Json(new { dugumler = dugumler });
            }, "Hesap planı okunamadı.");
        }
    }
}
