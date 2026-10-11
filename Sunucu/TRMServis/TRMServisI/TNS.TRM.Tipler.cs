using System;
using System.Collections.Generic;

namespace TNS.TRM
{
    [Serializable()]
    public class SorulanSorular
    {
        public int kod = 0;
        public int etapNo = 0;
        public int siraNo = 0;
        public TNSDateTime tarih = new TNSDateTime();
        public string soru = string.Empty;
        public string cevap = string.Empty;
        public int aktif = 0;
        public int tip = 0;
    }

    [Serializable()]
    public class MalzemeGrubu
    {
        public int kod = 0;
        public string ad = string.Empty;
        public decimal hibeOran = decimal.Zero;
        public int aktif = 0;
    }

    [Serializable()]
    public class Malzeme
    {
        public int kod = 0;
        public int anaKod = 0;
        public int siraNo = 0;
        public int cokluSecim = 0;
        public string kayitTur = string.Empty;
        public string il = string.Empty;
        public string ad = string.Empty;
        public string genelAd = string.Empty;
        public string marka = string.Empty;
        public string model = string.Empty;
        public int kullanimda = -1;
        public string cokluSecimIcerik = string.Empty;
        public decimal referansFiyat = decimal.Zero;
        public bool malzemeBaglama = false;
        public MakinaTanimOzellik[] tanimOzellik;
        public MalzemeGrubu malzemeGrubu = new MalzemeGrubu();

        public string yatirimMalzemeKod = "";
    }

    [Serializable()]
    public class MakinaTanimOzellik
    {
        public int malzemeKod = 0;
        public int siraNo = 0;
        public string ad = string.Empty;
        public string ozellikAd = string.Empty;
        public int ozellikKod = 0;
    }

    [Serializable()]
    public class Ceza
    {
        public int kod = 0;
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public TNSDateTime kayitTarihi = new TNSDateTime();
        public string kayitEden = string.Empty;
        public string aciklama = string.Empty;
        public string tip = string.Empty;
        public CezaliKisiler[] cezaliKisiler;
        public string adi = "";
        public string soyadi = "";
        public string kimlikNo = "";
    }

    [Serializable()]
    public class CezaliKisiler
    {
        public int cezaKod = 0;
        public string TCNO = string.Empty;
        public string adSoyad = string.Empty;
    }

    [Serializable()]
    public class BasvuruBelgeTanim
    {
        public int kod = 0;
        public int yil = 0;
        public int etapNo = 0;
        public MalzemeGrubu malzemeGrubu = new MalzemeGrubu();
        public string hibeTuru = string.Empty;
        public BasvuruBelgeAdlari[] belgeAdlari;
    }

    [Serializable()]
    public class BasvurudaYuklenenBelgeler
    {
        public string kod = string.Empty;
        public int yil = 0;
        public int etapNo = 0;
        public string basvuruNo = string.Empty;
        public int belgeKod = 0;
        public int siraNo = 0;
        public string evrEvrakNo = string.Empty;
        public string yukleyen = string.Empty;
        public string dosyaAdi = string.Empty;
        public int hibeKod = 0;
        public int hibeTur = 0;
        public string sablonAdi = string.Empty;
        public BasvuruBelgeAdlari belgeTanimi = new BasvuruBelgeAdlari();
    }

    [Serializable()]
    public class BasvuruBelgeAdlari
    {
        public int belgeKod = 0;
        public int siraNo = 0;
        public string belgeAdi = string.Empty;
        public string belgeKisaAdi = string.Empty;
        public string gercekTuzel = string.Empty;
        public string nitelik = string.Empty;
        public string kosul = string.Empty;
    }

    [Serializable()]
    public class BankaSube
    {
        public string kod = "";
        public string ilKod = "";
        public string ilAdi = "";
        public string adi = "";
        public int kapali = 0;
    }

    [Serializable()]
    public class Koy
    {
        public int kod = 0;
        public string ilKod = "";
        public string ilceKod = "";
        public string ilAdi = "";
        public string ilceAdi = "";
        public string adi = "";
        public string koordinat = "";
        public int ilMernisNo = 0;
        public int ilceMernisNo = 0;
        public int bucakMernisNo = 0;
        public int koyMernisNo = 0;
        public int kapali = 0;
    }

    [Serializable()]
    public class HibeMakine
    {
        public HibeMakineDetay[] detay;

        public bool online = false;
        public int kod = 0;
        public int yil = 0;
        public string basvuruNo = string.Empty;
        public string ilKod = string.Empty;
        public string ilceKod = string.Empty;
        public int koyKod = 0;
        public string kasaba = string.Empty;
        public int hukukiStatu = 0;
        public string kimlikNo = string.Empty;
        public string vergiDaire = string.Empty;
        public string ad = string.Empty;
        public string evrakNo = string.Empty;
        public TNSDateTime evrakTarih = new TNSDateTime();
        public string adres = string.Empty;
        public string telefon = string.Empty;
        public string faks = string.Empty;
        public string ePosta = string.Empty;
        public string bankaSubeKod = string.Empty;
        public int durum = 0;
        public TNSDateTime sozlesmeTarih = new TNSDateTime();
        public TNSDateTime yerindeTespitTarih = new TNSDateTime();
        public string projeNo = string.Empty;
        public string grupKod = string.Empty;
        public int etapNo = 0;
        public int[] etapNolar = null;
        public string redGerekce = string.Empty;
        public int ozellik = 0;
        public double puan = 0.0;
        public double puanOrjinal = 0.0;

        public TNSDateTime evrakTarihBaslangic = new TNSDateTime();
        public TNSDateTime evrakTarihBitis = new TNSDateTime();

        public TNSDateTime sozlesmeTarihBaslangic = new TNSDateTime();
        public TNSDateTime sozlesmeTarihBitis = new TNSDateTime();

        public string ilAd = string.Empty;
        public string ilceAd = string.Empty;
        public string koyAd = string.Empty;

        public string faturaNo = string.Empty;
        public TNSDateTime faturaTarih = new TNSDateTime();

        public string soyad = string.Empty;
        public string babaAd = string.Empty;
        public TNSDateTime dogumTarih = new TNSDateTime();


        public static string StatuAdBul(int hukukiStatu)
        {
            switch (hukukiStatu)
            {
                case (int)ENUMHibeMakineHukukiStatu.GENCCIFTCI:
                    return "GENÇ ÇİFTÇİ";
                case (int)ENUMHibeMakineHukukiStatu.GERCEKKISI:
                    return "GERÇEK KİŞİ";
                case (int)ENUMHibeMakineHukukiStatu.TUZELKISI:
                    return "TÜZEL KİŞİ";
                case (int)ENUMHibeMakineHukukiStatu.SUTBIRLIK:
                    return "SÜT BİRLİĞİ";
                default:
                    return "TANIMSIZ";
            }
        }
    }

    [Serializable()]
    public class HibeMakineDetay
    {
        public int hibeKod = 0;
        public int satirNo = 0;
        public int malzemeGrubuKod = 0;
        public string malzemeGrubuAd = string.Empty;
        public int malzemeKod = 0;
        public string malzemeAd = string.Empty;
        public decimal proformaTutar = decimal.Zero;
        public string malzemeSeriNo = string.Empty;
        public string firmaVergiNo = string.Empty;
        public string firmaAd = string.Empty;
        public decimal malzemeTutar = decimal.Zero;
        public string aciklama = string.Empty;

        public decimal hibeOran = decimal.Zero;
        public decimal hibeTutar = decimal.Zero;
        public decimal hibeEsasTutar = decimal.Zero;
        public decimal ayniKatki = decimal.Zero;
        public decimal toplamTutar = decimal.Zero;

        public decimal referansTutar = decimal.Zero;
        public int adet = 0;
        public decimal eksikHibeTutar = decimal.Zero;
        public decimal gercekHibeTutar = decimal.Zero;
        public int eksikOdemeDuzeltildi = 0;
        public decimal eksikOdemeOdenecek = decimal.Zero;
        public string eksikFazlaOdemeAciklama = "";
        public decimal gercekFatura = decimal.Zero;
    }

    [Serializable()]
    public class HibeMakineServis
    {
        public string isim = string.Empty;
        public string unvan = string.Empty;
        public string il = string.Empty;
        public string adres = string.Empty;
        public string malzemeGrubu = "";
        public string malzeme = "";
        public string basvuruDurumu = string.Empty;
        public string sozlesmeTarihi = string.Empty;
        public int yil = 0;
        public int etapNo = 0;
        public Sonuc sonuc = new Sonuc();
        public static string DurumAdBul(int durum)
        {
            switch (durum)
            {
                case (int)ENUMHibeMakineBasvuruDurum.ONLINE: return "Online Başvuru";
                case (int)ENUMHibeMakineBasvuruDurum.YENI: return "Başvurusu alındı";
                case (int)ENUMHibeMakineBasvuruDurum.ODEMEGERIGELDI: return "Ödeme geri gönderildi";
                case (int)ENUMHibeMakineBasvuruDurum.PROJENUMARASIVERILDI: return "Proje Numarası Verildi";
                case (int)ENUMHibeMakineBasvuruDurum.SOZLESMEYAPILDI: return "Sözleşme Yapıldı";
                case (int)ENUMHibeMakineBasvuruDurum.FATURAGELDI: return "Fatura Girişi Yapıldı";
                case (int)ENUMHibeMakineBasvuruDurum.YERINDETESPIT: return "Yerinde Tespit Yapıldı";
                case (int)ENUMHibeMakineBasvuruDurum.ODEMEYEGONDER: return "Ödemeye Gönderildi";
                case (int)ENUMHibeMakineBasvuruDurum.ODEMEBASLAT: return "Ödeme İşlemi Başladı";
                case (int)ENUMHibeMakineBasvuruDurum.ODENDI: return "Ödendi";
                case (int)ENUMHibeMakineBasvuruDurum.BASVURUREDDEDILDI: return "Başvuru Reddedildi";
                case (int)ENUMHibeMakineBasvuruDurum.DEGISTIRILDI: return "Değiştirildi";
                case (int)ENUMHibeMakineBasvuruDurum.BANKAYAGONDERILDI: return "Bankaya Gönderildi";
                default:
                    return "Tanımsız";
            }
        }
    }

    [Serializable()]
    public class HibeMakineTarihce
    {
        public int hibeKod = 0;
        public string kullaniciKod = string.Empty;
        public TNSDateTime tarih = new TNSDateTime();
        public int durum = 0;
        public string aciklama = "";
    }

    [Serializable()]
    public class IlBazindaIcmal
    {
        public string ilKod = "";
        public string ilAdi = "";
        public string koordinat = "";
        public int adet = 0;
        public decimal proforma = decimal.Zero;
        public decimal fatura = decimal.Zero;
        public decimal hibe = decimal.Zero;
        public double sulanacakAlan;
    }

    [Serializable()]
    public class Yonetim
    {
        public int etapNo = 0;
        public double gercek = 0.0;
        public double tuzel = 0.0;
        public double kooperatif = 0.0;
        public TNSDateTime tarih1 = new TNSDateTime();
        public TNSDateTime tarih2 = new TNSDateTime();
        public TNSDateTime onlineGirisiKapatmaTarihi = new TNSDateTime();
        public TNSDateTime degerlendirmeKapatmaTarihi = new TNSDateTime();
        public TNSDateTime sozlesmeKapatmaTarihi = new TNSDateTime();

        public int aktif = 0;
        public int giris = 0;
        public string tebligNo = "";
        public string resmiGazeteNo = "";
        public YonetimGiris[] girisler;
        public TNSDateTime eksikBelgeBasTarihi = new TNSDateTime();
        public TNSDateTime eksikBelgeBitTarihi = new TNSDateTime();
        public int izlemeSuresi = 0;

        public string tebligNoAltyapi = "";
        public string resmiGazeteNoAltyapi = "";
        public TNSDateTime tarih1Altyapi = new TNSDateTime();
        public TNSDateTime tarih2Altyapi = new TNSDateTime();
        public TNSDateTime onlineGirisiKapatmaTarihiAltyapi = new TNSDateTime();
        public TNSDateTime degerlendirmeKapatmaTarihiAltyapi = new TNSDateTime();
        public TNSDateTime eksikBelgeBasTarihiAltYapi = new TNSDateTime();
        public TNSDateTime eksikBelgeBitTarihiAltYapi = new TNSDateTime();
        public int girisAltyapi = 0;
        public int izlemeSuresiAltYapi = 0;
    }

    [Serializable()]
    public class YonetimGiris
    {
        public int etapNo = 0;
        public string ilKod = "";
        public TNSDateTime tarih = new TNSDateTime();
    }

    [Serializable()]
    public class HibeMakineSulamaGrup
    {
        public int hibeKod = 0;
        public int tipi = 0;
        public string kod = "";
        public string ilKod = "";
        public string ilAd = "";
        public int durum = 0;
        public int odemeKod = 0;
        public int kontrol = 0;
        public TNSDateTime kontrolTarih = new TNSDateTime();
        public int hakedisKod = 0;

        //Listeleme için ek alanlar
        public decimal hibeTutar = decimal.Zero;
        public decimal hibeEsasTutar = decimal.Zero;
        public decimal ayniKatki = decimal.Zero;
        public decimal refaransFarki = decimal.Zero;
        public decimal toplamTutar = decimal.Zero;
        public decimal sulamaHibeTutar = decimal.Zero;
        public decimal eksikHibeTutar = decimal.Zero;
        public decimal vergiKesintisi = decimal.Zero;
        public decimal netTutar = decimal.Zero;


        public string yatirimciKimlikNo = "";
        public string yatirimciAd = "";
        public string yatirimciSoyad = "";
        public string yatirimciBabaAd = "";
        public TNSDateTime yatirimciDogumTarih = new TNSDateTime();
        public string yatirimciBankaSube = "";
        public string yukleniciAd = "";
        public string sozlesmeTur = "";
        public string basvuruNo = "";
        public string telefon = "";
        public int sozlesmeKod = 0;
        public TNSDateTime odemeTarih = new TNSDateTime();

    }

    [Serializable()]
    public class HibeMakineSulamaOdeme
    {
        public int kod = 0;
        public int etapNo = 0;
        public int turKod = 0;
        public string tipi = "";
        public string gerekce = string.Empty;
        public string konu = string.Empty;
        public TNSDateTime gerceklestirmeSure = new TNSDateTime();
        public string gerceklestirmeUsulu = string.Empty;
        public string tertip = string.Empty;
        public string odemeHesap = string.Empty;

        public decimal hibe = decimal.Zero;
        public decimal komisyon = decimal.Zero;

        public decimal miktar = decimal.Zero;
        public decimal yaklasikBedel = decimal.Zero;
        public decimal odenek = decimal.Zero;
        public string gerceklestirmeGorevlileri = string.Empty;
        public string aciklama = string.Empty;
        public string koordinatorAd = string.Empty;
        public string koordinatorUnvan = string.Empty;
        public string hazirlayan1Ad = string.Empty;
        public string hazirlayan1Unvan = string.Empty;
        public string hazirlayan2Ad = string.Empty;
        public string hazirlayan2Unvan = string.Empty;
        public string teklifEdenAd = string.Empty;
        public string teklifEdenUnvan = string.Empty;
        public string harcamaYetkilisiAd = string.Empty;
        public string harcamaYetkilisiUnvan = string.Empty;
        public int odemeEmriYil = 0;
        public string odemeEmriNo = string.Empty;
        public string muhasebe = string.Empty;
        public string birim = string.Empty;
        public string vergiNo = string.Empty;
        public int odemeYapildi = 0;
        public TNSDateTime odemeYapilmaTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class HibeSulamaOdeme
    {
        public int kod = 0;
        public int etapNo = 0;
        public int turKod = 0;
        public string tipi = "";
        public string gerekce = string.Empty;
        public string konu = string.Empty;
        public TNSDateTime gerceklestirmeSure = new TNSDateTime();
        public string gerceklestirmeUsulu = string.Empty;
        public string tertip = string.Empty;
        public string odemeHesap = string.Empty;
        public decimal miktar = decimal.Zero;
        public decimal yaklasikBedel = decimal.Zero;
        public decimal odenek = decimal.Zero;
        public string gerceklestirmeGorevlileri = string.Empty;
        public string aciklama = string.Empty;
        public string teklifEdenAd = string.Empty;
        public string teklifEdenUnvan = string.Empty;
        public string harcamaYetkilisiAd = string.Empty;
        public string harcamaYetkilisiUnvan = string.Empty;
        public int odemeEmriYil = 0;
        public string odemeEmriNo = string.Empty;
        public string muhasebe = string.Empty;
        public string birim = string.Empty;
        public string vergiNo = string.Empty;
        public int odemeYapildi = 0;
        public TNSDateTime odemeYapilmaTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class HibeSulamaBankaKredi
    {
        public string kod = "";
        public int etapNo = 0;
        public string ilKod = "";
        public string ilAdi = "";
        public TNSDateTime tarih = new TNSDateTime();
        public string alimyeri = "";
        public string kimlikNo = "";
        public string adi = "";
        public string soyadi = "";
        public string parselNo = "";
        public string kayitEden = "";

        public string sorguParsel = "";
    }

    [Serializable()]
    public class SulamaGrubu
    {
        public int kod = 0;
        public string ad = string.Empty;
        public decimal hibeOran = decimal.Zero;
        public bool kooperatif = false;
        public int aktif = 0;
    }

    [Serializable()]
    public class HibeSulama
    {
        public HibeSulamaDetay[] detay;
        //public HibeSulamaDetayKonu detayKonu = new HibeSulamaDetayKonu();
        public HibeSulamaDiger[] diger;

        public int kod = 0;
        public int yil = 0;
        public string basvuruNo = string.Empty;
        public string ilKod = string.Empty;
        public string ilceKod = string.Empty;
        public int koyKod = 0;
        public string kasaba = string.Empty;
        public int hukukiStatu = 0;
        public string kimlikNo = string.Empty;
        public string vergiDaire = string.Empty;
        public string ad = string.Empty;
        public string evrakNo = string.Empty;
        public TNSDateTime evrakTarih = new TNSDateTime();
        public double puan = 0.0;
        public double puanOrjinal = 0.0;

        public string adres = string.Empty;
        public string telefon = string.Empty;
        public string faks = string.Empty;
        public string ePosta = string.Empty;
        public string bankaSubeKod = string.Empty;
        public int durum = 0;
        public TNSDateTime sozlesmeTarih = new TNSDateTime();
        public TNSDateTime yerindeTespitTarih = new TNSDateTime();
        public string projeNo = string.Empty;
        public string grupKod = string.Empty;
        public int etapNo = 0;
        public int[] etapNolar = null;
        public string redGerekce = string.Empty;
        public string asilYedek = string.Empty;
        public string parselMevkisi = string.Empty;
        public string kabulEdildi = string.Empty;

        public int varYokPuan = 0;
        public int redTip = 0;

        public TNSDateTime evrakTarihBaslangic = new TNSDateTime();
        public TNSDateTime evrakTarihBitis = new TNSDateTime();

        public TNSDateTime sozlesmeTarihBaslangic = new TNSDateTime();
        public TNSDateTime sozlesmeTarihBitis = new TNSDateTime();

        public double puanBas = 0.0;
        public double puanBit = 0.0;

        public string ilAd = string.Empty;
        public string ilceAd = string.Empty;
        public string koyAd = string.Empty;

        public string faturaNo = string.Empty;
        public TNSDateTime faturaTarih = new TNSDateTime();
        public string faturaAciklama = string.Empty;

        public string soyad = string.Empty;
        public string babaAd = string.Empty;
        public TNSDateTime dogumTarih = new TNSDateTime();

        public int sulamaModel = 0;
        public double sulanacakAlan = 0.0;
        public bool yazilimIciKayit = false;

        public string sorguKod = string.Empty;

        public string kontrolTip = "";
        public int sorguEtapNo = 0;
        public bool maskele = false;
        public string cinsiyet = "";

        public static string StatuAdBul(int hukukiStatu)
        {
            switch (hukukiStatu)
            {
                case (int)ENUMHibeSulamaHukukiStatu.GENCCIFTCI:
                    return "GENÇ ÇİFTÇİ";
                case (int)ENUMHibeSulamaHukukiStatu.GERCEKKISI:
                    return "GERÇEK KİŞİ";
                case (int)ENUMHibeSulamaHukukiStatu.TUZELKISI:
                    return "TÜZEL KİŞİ";
                case (int)ENUMHibeSulamaHukukiStatu.KOOPERATIF:
                    return "SULAMA KOOP.";
                case (int)ENUMHibeSulamaHukukiStatu.BIRLIK:
                    return "Tarımsal Kalkınma Kooperatifleri";
                default:
                    return "TANIMSIZ";
            }
        }
    }


    [Serializable()]
    public class HibeSulamaDetay
    {
        public int hibeKod = 0;
        public int satirNo = 0;
        public int sulamaGrubuKod = 0;
        public string sulamaGrubuAd = string.Empty;
        public string parselListe = string.Empty;
        public decimal proformaTutar = decimal.Zero;
        public string malzemeSeriNo = string.Empty;
        public string firmaVergiNo = string.Empty;
        public string firmaAd = string.Empty;
        public decimal malzemeTutar = decimal.Zero;
        public string aciklama = string.Empty;

        public decimal hibeOran = decimal.Zero;
        public decimal hibeTutar = decimal.Zero;
        public decimal hibeEsasTutar = decimal.Zero;
        public decimal ayniKatki = decimal.Zero;
        public decimal toplamTutar = decimal.Zero;
        public decimal hibeEsasTutarProje = decimal.Zero;
        public decimal hibeEsasTutarUygulama = decimal.Zero;
        public decimal proformaReferansTutar = decimal.Zero;

        public decimal eksikHibeTutar = decimal.Zero;
        public decimal gercekHibeTutar = decimal.Zero;
        public int eksikOdemeDuzeltildi = 0;
        public decimal eksikOdemeOdenecek = decimal.Zero;
        public string eksikFazlaOdemeAciklama = "";
        public decimal gercekFatura = decimal.Zero;

        public decimal ayniKatkiProje = decimal.Zero;
        public decimal basvuruToplamMalAlimTutar = decimal.Zero;
        public decimal basvuruHibeyeEsasMalAlimTutar = decimal.Zero;
        public decimal basvuruAyniKatkiTutar = decimal.Zero;
        public decimal basvuruTalepEdilenHibeTutar = decimal.Zero;
    }

    [Serializable()]
    public class HibeSulamaDetayKonu
    {
        public int kod = 0;
        public int hibeKod = 0;
        public int detayKod = 0;
        public int sulamaGrubuKod = 0;
        public string sulamaGrubuAd = string.Empty;
        public decimal proformaTutar = decimal.Zero;
        public string malzemeSeriNo = string.Empty;
        public string firmaVergiNo = string.Empty;
        public string firmaAd = string.Empty;
        public decimal malzemeTutar = decimal.Zero;
        public string aciklama = string.Empty;

        public decimal hibeOran = decimal.Zero;
        public decimal hibeTutar = decimal.Zero;
        public decimal hibeEsasTutar = decimal.Zero;
        public decimal ayniKatki = decimal.Zero;
        public decimal toplamTutar = decimal.Zero;
        public decimal hibeEsasTutarProje = decimal.Zero;
        public decimal hibeEsasTutarUygulama = decimal.Zero;
        public decimal proformaReferansTutar = decimal.Zero;

        public decimal eksikHibeTutar = decimal.Zero;
        public decimal gercekHibeTutar = decimal.Zero;
        public int eksikOdemeDuzeltildi = 0;
        public decimal eksikOdemeOdenecek = decimal.Zero;
        public string eksikFazlaOdemeAciklama = "";
        public decimal gercekFatura = decimal.Zero;

        public decimal ayniKatkiProje = decimal.Zero;
        public decimal basvuruToplamMalAlimTutar = decimal.Zero;
        public decimal basvuruHibeyeEsasMalAlimTutar = decimal.Zero;
        public decimal basvuruAyniKatkiTutar = decimal.Zero;
        public decimal basvuruTalepEdilenHibeTutar = decimal.Zero;
    }

    [Serializable()]
    public class HibeSulamaTarihce
    {
        public int hibeKod = 0;
        public string kullaniciKod = string.Empty;
        public TNSDateTime tarih = new TNSDateTime();
        public int durum = 0;
        public string aciklama = "";
    }

    [Serializable()]
    public class HibeSulamaDiger
    {
        public int hibeKod = 0;
        public int tip = 0;
        public int sulamaGrupKod = 0;
        public string parselNo = "";
        public string parselID = "";
        public double parselAlan = 0.0;
        public double parselSulanacakAlan = 0.0;
        public int parselSuKaynakTip = 0;
        public double parselSuKaynakDebi = 0.0;
        public string kuyuNo = "";
        public double kuyuDebi = 0.0;
        public double kuyuPompaGucu = 0.0;
        public double kuyuTrafoGucu = 0.0;
        public double kuyuSulanacakAlan = 0.0;
        public int parselSuKaynakCesidi = 0;
        public int parselSuTeminYonetimi = 0;
        public int parselSuKaliteSinifi = 0;
        public int parselSuKaynagiDurumu = 0;
        public int parselMulkiyeti = 0;
        public int toprakBunyeSinifi = 0;
        public string toprakTarlaKapasitesi = "";
        public double toprakInfiltrasyonHizi = 0.0;
        public string bitkiUrunCesidi = "";
        public int bitkiTesisYili = 0;
        public string bitkiDikimNormu = "";
        public string bitkiKokDerinligi = "";
        public string bitkiSuIhtiyaci = "";
        public string firmaVergiNo = "";
        public string firmaAd = "";
        public string faturaNo = "";
        public TNSDateTime faturaTarihi = new TNSDateTime();
        public string faturaAciklama = "";
    }

    [Serializable()]
    public class HibeSulamaMalzemeFiyat
    {
        public int hibeKod;
        public ENUMHibeSulamaMalzemeTip tip;
        public int malzemeKod;
        public int sulamaGrupKod = 0;
        public string sulamaGrupAd = "";
        public string parselID = "";
        public int satirNo;
        public decimal miktar;
        public decimal birimFiyat;
        public decimal referansFiyat;
        public decimal projeTutar;
        public decimal hibeEsasTutar;
        public HibeSulamaMalzemeOzellik[] ozellikler;
        public double parselAlan = 0.0;
        public double sulanacakAlan = 0.0;
        public string parselNo = "";

        public bool OzelliklerAyniMi(HibeSulamaMalzemeFiyat mf, int miktarKolonKod)
        {
            if ((this.ozellikler == null && mf.ozellikler != null) || (this.ozellikler != null && mf.ozellikler == null)
                || (this.ozellikler.Length != mf.ozellikler.Length))
                return false;

            foreach (HibeSulamaMalzemeOzellik o1 in this.ozellikler)
            {
                foreach (HibeSulamaMalzemeOzellik o2 in mf.ozellikler)
                {
                    if (o1.ozellikKod != o2.ozellikKod || o1.ozellikKod == miktarKolonKod)
                        continue;
                    if (o1.deger != o2.deger)
                        return false;
                    break;
                }
            }
            return true;
        }
    }

    [Serializable()]
    public class HibeSulamaMalzemeOzellik
    {
        public int hibeKod = 0;
        public ENUMHibeSulamaMalzemeTip tip = new ENUMHibeSulamaMalzemeTip();
        public int malzemeKod = 0;
        public int satirNo = 0;
        public int ozellikKod = 0;
        public int sulamaGrupKod = 0;
        public string parselID = "";
        public string deger = "";
    }

    [Serializable()]
    public class MakineEkipmanDestek
    {
        public string makinaAd = string.Empty;
        public int adet = 0;
        public int yil = 0;
        public decimal toplamTutar = decimal.Zero;
        public decimal toplamHibeTutar = decimal.Zero;
    }

    [Serializable()]
    public class HibeYatirimEksikBelge
    {
        public int kod = 0;
        public int hibeKod = 0;
        public string belgeAdi = string.Empty;
        public int belgeNo = 0;
    }

    [Serializable()]
    public class HibeYatirim
    {
        public bool online = false;
        public int kod = 0;
        public int yil = 0;
        public string basvuruNo = string.Empty;
        public string ilKod = string.Empty;
        public string ilceKod = string.Empty;
        public int koyKod = 0;
        public string kasaba = string.Empty;
        public int hukukiStatu = 0;
        public string kimlikNo = string.Empty;
        public string onlineTuzeleBasvuran = string.Empty;
        public string vergiDaire = string.Empty;
        public string ad = string.Empty;
        public string evrakNo = string.Empty;
        public TNSDateTime evrakTarih = new TNSDateTime();
        public string adres = string.Empty;
        public string telefon = string.Empty;
        public string faks = string.Empty;
        public string ePosta = string.Empty;
        public string bankaSubeKod = string.Empty;
        public string altYapiGrup = string.Empty;
        public int durum = 0;
        public int kabulCalismasi = 0;
        public TNSDateTime sozlesmeTarih = new TNSDateTime();
        public TNSDateTime yerindeTespitTarih = new TNSDateTime();
        public string grupKod = string.Empty;
        public int etapNo = 0;
        public int[] etapNolar = null;
        public string redGerekce = string.Empty;
        public bool konuNitelikUyusma = true;
        public bool degerlendirmeDisi = false;
        public string degerlendirmeDisiGerekce = string.Empty;
        //public string basvuruEskiSiraNo = string.Empty;
        //public string basvuruYeniSiraNo = string.Empty;
        public int basvuruSayisi = 0;
        public bool eskiBasvuru = false;
        public int istihdamSayisi = 0;
        public string dilekceNo = string.Empty;
        public int eskiEtapdaVarmi = 0;
        public int basvuruYapildi = 0;

        public int eskiEtapdaSozlesmesiVar = 0;
        public int eskiEtapdaSozlesmesiDevamEdiyor = 0;
        public int eskiEtapdaSozlesmesiBitti1 = 0;
        public int eskiEtapdaSozlesmesiBitti2 = 0;
        public int eskiEtapdaSozlesmesiImzalamayan = 0;
        public int eskiEtapdaSozlesmesiIptal = 0;
        public int eskiEtapdaSozlesmesiOzel = 0;
        public int eskiEtapdaSon5YilOrtak = 0;
        public int ayniAdres = 0;

        public ObjectArray eskiEtapdaSozlesmesiDevamEdiyorStr = new ObjectArray();
        public ObjectArray eskiEtapdaSozlesmesiBitti1Str = new ObjectArray();
        public ObjectArray eskiEtapdaSozlesmesiBitti2Str = new ObjectArray();
        public ObjectArray eskiEtapdaSozlesmesiImzalamayanStr = new ObjectArray();
        public ObjectArray eskiEtapdaSozlesmesiIptalStr = new ObjectArray();
        public ObjectArray eskiEtapdaSozlesmesiOzelStr = new ObjectArray();
        public ObjectArray eskiEtapdaSon5YilOrtakStr = new ObjectArray();
        public ObjectArray ayniAdresStr = new ObjectArray();

        public int pNitelik = 0;
        public int projeNitelik = 0;
        public string projeKonu = "";
        public string projeAltKonu = "";
        public string projeAdi = "";
        public decimal sozlesmeMaliyet = decimal.Zero;
        public decimal basvuruMaliyet = decimal.Zero;

        public string projeNitelikAdi = "";
        public string projeKonuAdi = "";
        public string projeAltKonuAdi = "";

        public decimal hibeTutar = decimal.Zero;
        public decimal hibeEsasTutar = decimal.Zero;
        public decimal ayniKatki = decimal.Zero;
        public decimal toplamTutar = decimal.Zero;
        public decimal sozlesmehibeEsasTutar = decimal.Zero;

        public decimal odenenInsaat = decimal.Zero;
        public decimal odenenMal = decimal.Zero;

        public int anket1 = 0;
        public int anket2 = 0;
        public int anket3 = 0;
        public int anket4 = 0;
        public int anket3Puan = 0;
        public int anket4Puan = 0;
        public double puan = 0;

        public double puanBas = 0.0;
        public double puanBit = 0.0;

        public TNSDateTime evrakTarihBaslangic = new TNSDateTime();
        public TNSDateTime evrakTarihBitis = new TNSDateTime();

        public TNSDateTime sozlesmeTarihBaslangic = new TNSDateTime();
        public TNSDateTime sozlesmeTarihBitis = new TNSDateTime();

        public string ilAd = string.Empty;
        public string ilceAd = string.Empty;
        public string koyAd = string.Empty;

        public string faturaNo = string.Empty;
        public TNSDateTime faturaTarih = new TNSDateTime();

        public string soyad = string.Empty;
        public string babaAd = string.Empty;
        public TNSDateTime dogumTarih = new TNSDateTime();
        public TNSDateTime basvuruTarihi = new TNSDateTime();

        public HibeYatirimSozlesme sozlesmeBilgileri;
        public HibeYatirimMaliyet[] maliyetBilgileri;
        public HibeYatirimUygulamaSozlesme[] uygSozlesmeBilgileri;
        public HibeYatirimSozlesmeFesih sozlesmeFesih = new HibeYatirimSozlesmeFesih();

        public decimal sorguHibeTutar1 = 0;
        public decimal sorguHibeTutar2 = 0;
        public string sorguGosterme = "";

        public string ipAdresi = "";

        public int yatirimOncelikTur = -1;
        public int sirketOrtagi = 0;
        public string eksikBelgeAdlari = "";
        public int eksikBelgeAdet = 0;
        public string cinsiyet = "";

        //public Malzeme malzeme = new Malzeme();
        public HibeYatirimSoru[] sorular;
        public YatirimMalzeme[] malzeme;

        public static string StatuAdBul(int hukukiStatu)
        {
            switch (hukukiStatu)
            {
                case (int)ENUMHibeYatirimHukukiStatu.GERCEKKISI:
                    return "GERÇEK KİŞİ";
                case (int)ENUMHibeYatirimHukukiStatu.TUZELKISI:
                    return "TÜZEL KİŞİ";
                default:
                    return "TANIMSIZ";
            }
        }

        public string mevki = string.Empty;
        public string paftaNo = string.Empty;
        public string adaNo = string.Empty;
        public string parselNo = string.Empty;
        public string koordinat = string.Empty;
        public string ozet = string.Empty;
        public string hedef = string.Empty;

        public bool komisyonRaporu = false;
        public string listeYazdir = string.Empty;
        public string engelli = string.Empty;
        public string organik = string.Empty;
        public int yas = 0;
        public string altKonuDetayKod = "";
        public string altKonuDetayKodGorunen = "";
        public string altKonuDetayAdi = "";

        public TNSDateTime sonOdemeYapilanTarih = new TNSDateTime();
        public bool maskele = false;
        public double uygulamaSozlesmeMal;
        public double uygulamaSozlesmeInsaat;
    }

    [Serializable()]
    public class HibeYatirimServis
    {
        public string isim = string.Empty;
        public string unvan = string.Empty;
        public string il = string.Empty;
        public string adres = string.Empty;
        public string projeAdi = "";
        public string yatirimKonusu = "";
        public string basvuruDurumu = string.Empty;
        public string sozlesmeTarihi = string.Empty;
        public int yil = 0;
        public int etapNo = 0;
        public string etapNoStr = "";
        public Sonuc sonuc = new Sonuc();
        public static string DurumAdBul(int durum)
        {
            switch (durum)
            {
                case (int)ENUMHibeYatirimBasvuruDurum.ONLINE: return "Online";
                case (int)ENUMHibeYatirimBasvuruDurum.YENI: return "Başvuru alındı";
                case (int)ENUMHibeYatirimBasvuruDurum.DEGISTIRILDI: return "Başvuru alındı";
                case (int)ENUMHibeYatirimBasvuruDurum.ILDEKABULEDILENLER: return "İlde Kabul Edilenler";
                case (int)ENUMHibeYatirimBasvuruDurum.KABULEDILDI: return "Merkezde kabul edilenler";
                case (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEYAPILDI: return "Sözleşme Yapıldı";
                case (int)ENUMHibeYatirimBasvuruDurum.UYGULAMASOZLESMESIYAPILDI: return "Uygulama Sözleşmesi Yapıldı";
                case (int)ENUMHibeYatirimBasvuruDurum.SOZLESMESILINDI: return "Sözleşmesi Silindi";
                case (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDIMERKEZ: return "Merkezde değerlendirme dışı bırakılanlar";
                case (int)ENUMHibeYatirimBasvuruDurum.BASVURUREDDEDILDI: return "İlde değerlendirme dışı bırakılanlar";
                case (int)ENUMHibeYatirimBasvuruDurum.DEGERLENDIRILDI: return "Değerlendirildi";
                case (int)ENUMHibeYatirimBasvuruDurum.GERICEKILDI: return "Geri Çekildi";
                case (int)ENUMHibeYatirimBasvuruDurum.SOZLESMEIMZALAMADI: return "Sözleşme İmzalamadı";
                default:
                    return "Tanımsız";
            }
        }
    }

    [Serializable()]
    public class HibeYatirimMaliyet
    {
        public int hibeKod;
        public int durum;
        public decimal hibeEsasTutarInsaat;
        public decimal hibeEsasTutarMal;
        public decimal hibeEsasTutarToplam;
        public decimal ayniKatkiInsaat;
        public decimal ayniKatkiMal;
        public decimal ayniKatkiToplam;
        public decimal projeToplamTutar;
        public decimal hibeTutar;
        public decimal ayniKatkiDanismanlik;
    }

    [Serializable()]
    public class YatirimIsletmePlani
    {
        public int hibeKod;
        public decimal toplamYatirimTutari;
        public decimal sabitYatirimTutari;
        public decimal sermayeYatirimTutari;
        public decimal isletmeGeliri;
        public decimal isletmeGideri;
        public decimal brutNakitAkim;
        public decimal IcKarlilikOrani;
        public decimal faydaMasrafOrani;
        public string geriDonusSuresi;
        public decimal brutKatmaDeger;
        public decimal netKatmaDeger;
        public decimal basabasNoktasi;
        public TNSDateTime baslamaTarihi;
        public TNSDateTime tamamlanmaTarihi;
        public decimal mevcutKapasite;
        public decimal projeIleSaglananKapasite;
        public decimal projeSonrasiOlacakKapasite;
        public decimal mevcutIstihdam;
        public decimal projeIleSaglananIstihdam;
        public decimal projeSonrasiIstihdam;
        public bool online = false;
        public Sonuc sonuc;
        public string ipAdresi = "";
    }

    [Serializable()]
    public class YatirimSayisi
    {
        public int adet;
        public decimal projeToplamTutar;
        public decimal sozlesmehibeTutar;
        public decimal hibeTutar;
    }

    [Serializable()]
    public class YatirimMakineSulamaOdenek
    {
        public int devamEdenSozlesmeSayisi;
        public int bitenSozlesmeSayisi;
        public int iptalEdilenSayi;
        public int oncekiEtapIptalEdilenSayi;
        public string projeKonuKod;
        public string projeKonuAd;
        public decimal devamEdenTutar;
        public decimal bitenTutar;
        public int istihdam;
        public int kistihdam;
        public string ilAd;
        public int oncekiEtap;
    }

    [Serializable()]
    public class YatirimIzlemeCetveli
    {
        public string ilKod;
        public string ilAd;
        public YatirimSayisi basvuru;
        public YatirimSayisi ilKabul;
        public YatirimSayisi merkezKabul;
        public YatirimSayisi sozlesmeYapilan;
        public YatirimSayisi sozlesmesiDevamEden;
        public YatirimSayisi sozlesmesiBiten;
        public YatirimSayisi sozlesmesiIptalOlan;
        public YatirimSayisi hakedisiOdenen;
        public Sonuc sonuc = new Sonuc();
    }

    [Serializable()]
    public class HibeYatirimSozlesme
    {
        public int hibeKod;
        public TNSDateTime sozlesmeTarih;
        public TNSDateTime projeBasTarih;
        public TNSDateTime projeBitTarih;
        public TNSDateTime nihaiRaporTarihi = new TNSDateTime();
        public int istihdam;
        public decimal uretimKapasitesi;
        public string kapasiteBirimKodu;
        public decimal teminatTutar;
        public int teminatTur;
        public TNSDateTime teminatTarih;
        public string bankaSubeKod;
        public int istihdamKadin;
        public bool eskiBasvuru = false;
        public ENUMHibeYatirimSozlesmeDurum durum;
        public string ipAdresi = "";
        public string yapiKimlikNo = "";

        public static string SozlesmeDurumAdBul(ENUMHibeYatirimSozlesmeDurum sozlesmeDurum)
        {
            switch (sozlesmeDurum)
            {
                case ENUMHibeYatirimSozlesmeDurum.DEVAMEDIYOR:
                    return "Devam Ediyor";
                case ENUMHibeYatirimSozlesmeDurum.BITTI:
                    return "Bitti";
                case ENUMHibeYatirimSozlesmeDurum.HATALI:
                    return "Hatalı Giriş";
                case ENUMHibeYatirimSozlesmeDurum.IPTAL:
                    return "İptal Edildi";
                default:
                    return string.Empty;
            }
        }
    }

    [Serializable()]
    public class HibeYatirimUygulamaSozlesme
    {
        public int hibeKod;
        public int sozlesmeKod;
        public int hukukiStatu;
        public string kimlikNo;
        public string yukleniciAd;
        public ENUMHibeYatirimUygulamaSozlesmeTur sozlesmeTur;
        public TNSDateTime sozlesmeTarih;
        public decimal sozlesmeTutar;
        public ENUMHibeYatirimUygulamaSozlesmeDurum sozlesmeDurum;
        public HibeYatirimHakedis[] hakedisBilgileri;

        public static string SozlesmeDurumAdBul(ENUMHibeYatirimUygulamaSozlesmeDurum sozlesmeDurum)
        {
            switch (sozlesmeDurum)
            {
                case ENUMHibeYatirimUygulamaSozlesmeDurum.DEVAMEDIYOR:
                    return "Devam Ediyor";
                case ENUMHibeYatirimUygulamaSozlesmeDurum.BITTI:
                    return "Bitti";
                case ENUMHibeYatirimUygulamaSozlesmeDurum.HATALI:
                    return "Hatalı Giriş";
                case ENUMHibeYatirimUygulamaSozlesmeDurum.IPTAL:
                    return "İptal Edildi";
                default:
                    return string.Empty;
            }
        }

        public static string SozlesmeTurAdBul(ENUMHibeYatirimUygulamaSozlesmeTur sozlesmeTur)
        {
            switch (sozlesmeTur)
            {
                case ENUMHibeYatirimUygulamaSozlesmeTur.INSAAT:
                    return "İnşaat";
                case ENUMHibeYatirimUygulamaSozlesmeTur.MALALIMI:
                    return "Mal Alımı";
                case ENUMHibeYatirimUygulamaSozlesmeTur.ANAHTARTESLIM:
                    return "Anahtar Teslim";
                default:
                    return string.Empty;
            }
        }
    }

    [Serializable()]
    public class HibeYatirimHakedis
    {
        public int hibeKod;
        public int sozlesmeKod;
        public int hakedisKod;
        public string faturaNo;
        public TNSDateTime faturaTarih;
        public decimal faturaTutar;
        public ENUMHibeYatirimHakedisDurum hakedisDurum;
        public TNSDateTime onayTarih;
        public string redAciklama;
        public TNSDateTime odemeTarih;
        public string odemeGrupNo;
        public int odemeKod;
        public int kontrol;
        public TNSDateTime kontrolTarih;
        public decimal esasFaturaTutar = 0;
        public int sonOdeme = -1;

        //Ait olduğu sözleşme bilgileri
        public HibeYatirimUygulamaSozlesme sozlesmeBilgi;

        public static string HakedisDurumAdBul(ENUMHibeYatirimHakedisDurum hakedisDurum)
        {
            switch (hakedisDurum)
            {
                case ENUMHibeYatirimHakedisDurum.YENI:
                    return "Yeni";
                case ENUMHibeYatirimHakedisDurum.ONAYLANDI:
                    return "Onaylandı";
                case ENUMHibeYatirimHakedisDurum.ODEMEDE:
                    return "Ödemede";
                case ENUMHibeYatirimHakedisDurum.ODEMEBASLAT:
                    return "Ödeme İşlemi Başladı";
                case ENUMHibeYatirimHakedisDurum.BANKAYAGONDERILDI:
                    return "Bankaya Gönderildi";
                case ENUMHibeYatirimHakedisDurum.ODENDI:
                    return "Ödendi";
                case ENUMHibeYatirimHakedisDurum.HATALI:
                    return "Hatalı Giriş";
                case ENUMHibeYatirimHakedisDurum.RED:
                    return "Reddedildi";
                default:
                    return string.Empty;
            }
        }
    }

    [Serializable()]
    public class HibeYatirimTarihce
    {
        public int hibeKod = 0;
        public string kullaniciKod = string.Empty;
        public string kullaniciAdi = string.Empty;
        public TNSDateTime tarih = new TNSDateTime();
        public int durum = 0;
        public string aciklama = "";
        public int basvuruDurum = 0;
        public string basvuruNo = "";
        public string ipAdresi = "";
    }

    [Serializable()]
    public class HibeYatirimRefFiyat
    {
        public int etapNo = 0;
        public string ilKod = "";
        public string malzemeKod = "";
        public double fiyat = 0.0;
        public string malzemeAd = "";
        public int malzemeGrupKod = 0;
        public string malzemeGrupAd = "";
        public int sadeceBasvuruMalzemeleri = 0;
        public string islemYapan = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
        public bool sorguMalzemeRefFiyat = false;
        public int miktar = 0;
    }

    [Serializable()]
    public class HibeMakineRefFiyat
    {
        public int etapNo = 0;
        public string ilKod = "";
        public int malzemeKod = 0;
        public double fiyat = 0.0;
        public string malzemeAd = "";
        public int malzemeGrupKod = 0;
        public string malzemeGrupAd = "";
        public int sadeceBasvuruMalzemeleri = 0;
        public string islemYapan = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class MalzemeFiyatAnalizi
    {
        public int etapNo = 0;
        public string ilKod = "";
        public int malzemeGrupKod = 0;
        public int malzemeKod = 0;
        public double referansTutar = 0.0;
        public double dusukTutar = 0.0;
        public double yuksekTutar = 0.0;
        public int adet = 0;
        public string malzemeGrupAd = "";
        public string malzemeAd = "";
        public string ilAd = "";
        public int durum = 0;
    }

    [Serializable()]
    public class SulamaMalzeme
    {
        public int kod;
        public string ad;
        public SulamaMalzemeGrup[] gruplar;
        public SulamaMalzemeOzellik[] ozellikler;
    }

    [Serializable()]
    public class SulamaMalzemeGrup
    {
        public int malzemeKod;
        public int grupKod;
        public bool cokluSatir;
    }

    [Serializable()]
    public class SulamaMalzemeOzellik
    {
        public int malzemeKod;
        public int kod;
        public string ad;
        public bool miktarKolon;
        public bool manuelGiris;
        public ENUMSulamaMalzemeOzellikKolonTip kolonTip;
        public SulamaMalzemeOzellikDeger[] degerler;
    }

    [Serializable()]
    public class SulamaMalzemeOzellikDeger
    {
        public int malzemeKod;
        public int ozellikKod;
        public int kod;
        public string ad;
    }

    [Serializable()]
    public class YatirimNitelik
    {
        public int kod = 0;
        public string ad = string.Empty;
        public int aktif = 0;
    }

    [Serializable()]
    public class YatirimAltKonuDetay
    {
        public string kod = "";
        public string altKonuKod = "";
        public string altKonuAd = "";
        public string numara = string.Empty;
        public string ad = string.Empty;
        public int aktif = 0;
    }

    [Serializable()]
    public class YatirimKonuNitelikKisit
    {
        public string kod = "";
        public int etapNo = 0;
        public string ilKod = "";
        public string ilceKod = "";
        public string konuKod = "";
        public string altKonuKod = "";
        public int nitelikKod = 0;
        public string ilAdi = "";
        public string ilceAdi = "";
        public string konuAdi = "";
        public string altKonuAdi = "";
        public string nitelikAdi = "";
    }

    [Serializable()]
    public class YatirimMalzemeTur
    {
        public int kod = 0;
        public int konuTur = 0;
        public string ad = string.Empty;
        public int aktif = 0;
        public int anaKod = 0;
        public string anaAd = string.Empty;
        public int coklu = 0;
    }

    [Serializable()]
    public class YatirimMalzemeTanim
    {
        public string kod = "";
        public YatirimMalzemeTur tur = new YatirimMalzemeTur();
        public string ad = string.Empty;
        public int aktif = 0;
        public int referansFiyat = 0;
        public string ozellik = string.Empty;
        public int anaGrupKod = 0;
        public string anaGrupAdi = "";
        public int miktar = 0;
    }

    [Serializable()]
    public class YatirimMalzeme
    {
        public int kod = 0;
        public int hibeKod = 0;
        public int malzemeAnaGrupKod = 0;
        public string malzemeAnaGrupAdi = "";
        public int malzemeTurKod = 0;
        public string malzemeTurAdi = "";
        public string marka = "";
        public string model = "";
        public string ozellik = "";
        public double tutar = 0;
        public double miktar = 0;
        public YatirimMalzemeTanim malzeme = new YatirimMalzemeTanim();
        public double referansTutar = 0.0;
        public string kosul = "";

        public int sorguAdet = 0;
        public int sorguDurum = 0;
        public HibeYatirim hibe = new HibeYatirim();
    }

    [Serializable()]
    public class YatirimKonu
    {
        public string kod = "";
        public string ad = "";
        public string kisaAd = "";
        public int aktif = 0;
        public string nitelik = "";
        public bool not = false;
    }

    [Serializable()]
    public class YatirimKonuUstSinir
    {
        public int etapNo = 0;
        public string konuKod = "";
        public string konuAd = "";
        public int nitelikKod = 0;
        public double gercek = 0.0;
        public double tuzel = 0.0;
    }

    [Serializable()]
    public class YatirimKonuIl
    {
        public int etapNo = 0;
        public string ilKod = "";
        public string konuKod = "";
        public string ilAd = "";
        public string konuAd = "";
    }

    [Serializable()]
    public class YatirimAltKonuIl
    {
        public int etapNo = 0;
        public string ilKod = "";
        public string altKonuKod = "";
        public string ilAd = "";
        public string altKonuAd = "";
    }

    [Serializable()]
    public class HibeYatirimOncelik
    {
        public int etapNo = 0;
        public string ilKod = string.Empty;
        public int siraNo = 0;
        public int tur = 0;
        public string altYapiGrup = "";
        public YatirimKonu konu = new YatirimKonu();
    }

    [Serializable()]
    public class HibeYatirimOncelikRapor
    {
        public string kod = "";
        public string[] basvuru;
    }

    [Serializable()]
    public class HibeYatirimKisaKonu
    {
        public string basvuru;
    }

    [Serializable()]
    public class Butce
    {
        public int etapNo = 0;
        public string ilKod = string.Empty;
        public string ilAd = string.Empty;
        public int ilTur = 0;
        public int tip = 0;
        public int yatirimOncelikTur = 0;
        public double butce = 0.0;
        public double katsayi = 0.0;
        public double talepEdilen = 0.0;
    }

    [Serializable()]
    public class EklenenDosya
    {
        public string kod = string.Empty;
        public string ad = string.Empty;
        public TNSDateTime dosyaTarihi = new TNSDateTime();
        public TNSDateTime kayitTarihi = new TNSDateTime();
        public int boyut = 0;
        public string tip = string.Empty;
        public string tur = string.Empty;
        public string kayitNo = string.Empty;
        public string evrakNo = string.Empty;
        public string kayitEden = string.Empty;
    }

    [Serializable()]
    public class PuanSoru
    {
        public int kod = 0;
        public int siraNo = 0;
        public int etapNo = 0;
        public int hibeTur = 0;
        public int grupKod = 0;
        public string soru = "";
        public string grupAd = "";
        public PuanYanit[] yanitlar = new PuanYanit[1];
    }

    [Serializable()]
    public class PuanYanit
    {
        public int kod = 0;
        public int soruKod = 0;
        public int siraNo = 0;
        public string yanit = "";
        public int puan = 0;
    }

    [Serializable()]
    public class Basvuru
    {
        public int hibeKod = 0;
        public int turKod = 0;
        public string bolumKod = "";
        public string metin = "";
        public string ipAdresi = "";
    }

    [Serializable()]
    public class BasvuruSablon
    {
        public int kod = 0;
        public int turKod = 0;
        public int etapNo = 0;
        public string bolumKod = "";
        public string bolumAd = "";
        public string aciklama = "";
        public string sablon = "";
        public int tabloGirisSekli = 0;
        public int altYapiA = 0;
        public int altYapiB = 0;
    }

    [Serializable()]
    public class OnlineBasvuruKaydi
    {
        public string kod = "";
        public int turKod = 0;
        public int etapNo = 0;
        public string kimlikNo = "";
        public string ePostaAdresi = "";
        public TNSDateTime tarih = new TNSDateTime();
        public string ip = "";
        public bool postaGonder = false;

        public string adi = "";
        public string soyadi = "";
        public string babaAdi = "";
        public string vergiDairesi = "";
        public string ePostaAdresiTekrar = "";
        public string parola = "";
        public string parolaTekrar = "";
        public int guvenlikSorusu = 0;
        public string guvenlikSorusuMetin = "";
        public string guvenlikSorusuCevap = "";
        public int aktif = 0;
        public string uyeKimlikNo = "";
        public string uyeAdi = "";
        public string uyeSoyadi = "";
        public string uyeBabaAdi = "";
        public TNSDateTime dogumTarih = new TNSDateTime();
        //public string girisKod = "";
        //public int sonGonderilenMi = 0;
    }

    [Serializable()]
    public class YonetimKurulu
    {
        public int hibeKod = 0;
        public int satirNo = 0;
        public string adSoyad = "";
        public string meslek = "";
        public int cinsiyet = 0;
        public string pozisyon = "";
        public int yil = 0;
    }

    [Serializable()]
    public class Ortaklar
    {
        public int hibeKod = 0;
        public int satirNo = 0;
        public string kimlikNo = "";
        public string adSoyad = "";
        public double yuzde = 0.0;
    }

    [Serializable()]
    public class IlDegerlendirmeKomisyonu
    {
        public int etapNo = 0;
        public int hibeTuru = 0;
        public int kod = 0;
        public string tcNo = "";
        public string il = "";
        public string ilAd = "";
        public string adSoyad = "";
        public string eMail = "";
        public string telefon = "";
        public int degerlendirmeci = 0;
        public int baskan = 0;
        public int kilit = 0;
        public int konuTur = 0;
        public string altYapiGrup = "";
        public TNSDateTime dogumTarih { get; set; } = new TNSDateTime();
        public DateTime dogumTarihDT { get { return dogumTarih.Oku(); } set { dogumTarih.Yaz(value); } }
        public bool maskele = false;
    }

    [Serializable()]
    public class YonetimFonKaynak
    {
        public int hibeKod = 0;
        public int satirNo = 0;
        public int fonTip = 0;
        public int yil = 0;
        public string projeAdiReferansNo = "";
        public string kurulusProgramAdi = "";
        public double tutar = 0.0;
        public double hibeKredi = 0.0;
        public string projeAsamasi = "";
    }

    [Serializable()]
    public class YatirimGiderlerTablosu
    {
        public int hibeKod = 0;
        public int siraNo = 0;
        public int tip = 0;
        public int ayni = 0;
        public string giderKalemi = "";
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public double toplamGider = 0.0;
        public YatirimGiderlerTablosuDetay[] detaylar;
    }

    [Serializable()]
    public class YatirimGiderlerTablosuDetay
    {
        public int hibeKod = 0;
        public int siraNo = 0;
        public int tip = 0;
        public int ayni = 0;
        public int ay = 0;
        public double gider = 0.0;
    }

    [Serializable()]
    public class YatirimSureEylemPlani
    {
        public int hibeKod = 0;
        public int satirNo = 0;
        public int yil = 0;
        public string faaliyetNo = string.Empty;
        public int asama = 0;
        public string aciklama = "";
        public string kurum = "";
        public YatirimSureEylemPlaniDetay[] detaylar;
    }

    [Serializable()]
    public class YatirimSureEylemPlaniDetay
    {
        public int hibeKod = 0;
        public int satirNo = 0;
        public int ay = 0;
        public int calisma = 0;
    }

    [Serializable()]
    public class YatirimIsTakvimi
    {
        public int hibeKod = 0;
        public int tur = 0;
        public int tip = 0;
        public string konu = "";
        public YatirimIsTakvimiDetay[] detaylar;
    }

    [Serializable()]
    public class YatirimIsTakvimiDetay
    {
        public int hibeKod = 0;
        public int tur = 0;
        public int tip = 0;
        public int ay = 0;
        public string aciklama = string.Empty;
    }

    [Serializable()]
    public class YatirimProjeButcesi
    {
        public int hibeKod = 0;
        public int giderKalemTuru = 0;
        public string siraNo = string.Empty;
        public string giderKalemi = string.Empty;
        public double toplamTutar = 0.0;
        public YatirimProjeButcesiDetay[] detaylar;
    }

    [Serializable()]
    public class YatirimProjeButcesiDetay
    {
        public int hibeKod = 0;
        public int giderKalemTuru = 0;
        public int ay = 0;
        public double tutar = 0.0;
    }

    [Serializable()]
    public class YatirimFinansmanKaynagi
    {
        public int hibeKod = 0;
        public int giderKalemTuru = 0;
        public string siraNo = string.Empty;
        public string giderKalemi = string.Empty;
        public double toplamTutar = 0.0;
        public YatirimFinansmanKaynagiDetay[] detaylar;
    }

    [Serializable()]
    public class YatirimFinansmanKaynagiDetay
    {
        public int hibeKod = 0;
        public int giderKalemTuru = 0;
        public int kaynakTipi = 0;
        public double tutar = 0.0;
        public double oran = 0.0;
    }


    [Serializable()]
    public class HazineRapor
    {
        public int etapNo = 0;
        public string projeKodu = "";
        public string kimlikKodu = "";
        public string ilKodu = "";
        public string ilceKodu = "";
        public string sektorKodu = "";
        public TNSDateTime muracaatTarihi = new TNSDateTime();
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public TNSDateTime baslamaTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public string destekUnsuru = "";
        public string durum = "";
        public string anahtar = "";
        public string destekVerilen = "";
        public double hibeEsasTutar = 0.0;
        public double hibeTutar = 0.0;
        public int sozlesmeTur = 0;
        public int yil = 0;
    }

    [Serializable()]
    public class TUIKveri
    {
        public double Yil;
        public string UrunAdi;
        public decimal SatisFiyati;
        public double EkilenAlanDA;
        public double HasatAlanDA;
        public double UretimTON;
        public double VerimKG_DA;
        public decimal OrtalamaMaliyet;
        public decimal AraziKatsayisi;
        public decimal AileIsciligi;
        public decimal YabanciIscilik;

        public decimal GSUD;
        public decimal Iscilik;
        public decimal AraziKirasi;
        public decimal ToplamMasraf;
        public decimal DegisenMasraf;
        public decimal IsletmeGeliri;
        public decimal ToplamIsletmeGeliriIciPayi;

        public decimal DegiskenMasrafYuzdeSulu;
        public decimal DegiskenMasrafYuzdeKuru;
        public decimal turtip;


    }

    [Serializable()]
    public class TUIKozetveri
    {
        public string IlAdi;
        public string IlceAdi;
        public int NormSulu;
        public int NormKuru;
        public int NormDikili;
        public int NormOrtuAlti;
        public double EkilenAlanDA;
        public double UretimTON;
        public double VerimKG_DA;
    }

    [Serializable()]
    public class Goruntu
    {
        public int hibeKod = 0;
        public string basvuruNo = "";
        public string kamera1Adres = "";
        public string kamera1Kullanici = "";
        public string kamera1Parola = "";
        public string kamera2Adres = "";
        public string kamera2Kullanici = "";
        public string kamera2Parola = "";
        public string kamera3Adres = "";
        public string kamera3Kullanici = "";
        public string kamera3Parola = "";
        public string kamera4Adres = "";
        public string kamera4Kullanici = "";
        public string kamera4Parola = "";
        public string ePosta = "";

    }

    [Serializable()]
    public class HibeYatirimSozlesmeFesih
    {
        public int kod = 0;
        public int hibeKod = 0;
        public TNSDateTime tarih = new TNSDateTime();
        public int durum = 0;
        public string aciklama = "";
        public double tutar = 0.0;
        public string islemYapanAdi = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class HibeYatirimYatirimciKimlikNo
    {
        public string kod = "";
        public int hibeKod = 0;
        public string eskiKimlikNo = "";
        public string yeniKimlikNo = "";
    }

    [Serializable()]
    public class HibeYatirimSoru
    {
        public int hibeKod = 0;
        public string soruKod = "";
        public int deger = 0;
    }

    [Serializable()]
    public class VeriToplama
    {
        public int kod = 0;
        public string aciklama = "";
        public int ilKod = 0;
        public int yeniYatirim = 0;
        public int modTekYen = 0;
        public int tamamlama = 0;
        public int faal = 0;
        public int faalDegil = 0;
        public int gercekKisiE = 0;
        public int gercekKisiK = 0;
        public int sirket = 0;
        public int kooperatif = 0;
        public double toplamYatirimTutari = 0.0;
        public double toplamHibeTutari = 0.0;
        public double islenenHammaddeMiktari = 0.0;
        public int buyukBas = 0;
        public int kucukBas = 0;
        public int saglananIstihdam = 0;
        public int ihracatYapanTesisSayisi = 0;
        public double sonYilCiro = 0.0;
    }

    [Serializable()]
    public class AsilYedekListe
    {
        public int hibeKod = 0;
        public double puan = 0.0;
        public double hibeTutari = 0.0;
        public string asilYedek = "";
    }

}