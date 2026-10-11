using System;
using System.Collections;
using System.Text;
using WebServisler.MobilImzaEvrak;

namespace TNS.EVR
{
    /// <summary>
    /// Evrak Özel Alan bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class EvrakOzelAlan
    {
        /// <summary>
        /// Evrak prEvrakNo
        /// </summary>
        public string prEvrakNo = "";
        /// <summary>
        /// Özel Alan bilgisinin tutulduğu nesne
        /// </summary>
        public OzelAlan ozelAlan = new OzelAlan();
        /// <summary>
        /// Evrak Özel Alan değeri
        /// </summary>
        public string deger = "";
        /// <summary>
        /// Sadece yetki kontrolü yapılırken kullanılmak üzere
        /// </summary>
        public string kullaniciAktifBirimi = "";
    }

    /// <summary>
    /// Özel Alan bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class OzelAlan
    {
        /// <summary>
        /// Evrak Birim bilgisinin tutulduğu nesne
        /// </summary>
        public Birim evrakBirim;
        /// <summary>
        /// Dosya Plan bilgisinin tutulduğu nesne
        /// </summary>
        public DosyaPlani dosya;
        /// <summary>
        /// Alan kodu
        /// </summary>
        public int alanKod;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int sira;
        /// <summary>
        /// Adı
        /// </summary>
        public string ad;

        /// <summary>
        /// OzelAlan sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OzelAlan()
        {
            evrakBirim = new Birim();
            dosya = new DosyaPlani();
            alanKod = 0;
            sira = 0;
            ad = string.Empty;
        }
    }


    /// <summary>
    /// Birim bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Birim
    {
        /// <summary>
        /// Kodu
        /// </summary>
        public string kod;
        /// <summary>
        /// İdare Kimlik Kodu
        /// </summary>
        public string idareKimlikKodu;
        /// <summary>
        /// Adı
        /// </summary>
        public string ad;
        /// <summary>
        /// Kısa Adı
        /// </summary>
        public string kisaAd;
        /// <summary>
        /// Gelen Evrak olup olmadığı
        /// </summary>
        public int genelEvrakMi;
        /// <summary>
        /// Seviyesi
        /// </summary>
        public int seviye;
        /// <summary>
        /// Detayı
        /// </summary>
        public bool detay;
        /// <summary>
        /// Birimin kullanılıp kullanılmadığı
        /// </summary>
        public int kullanilmiyor;
        /// <summary>
        /// Üst Birimi
        /// </summary>
        public string ustBirim;
        /// <summary>
        /// Üst Birim Adı
        /// </summary>
        public string ustBirimAd;
        /// <summary>
        /// E-posta Adresi
        /// </summary>
        public string ePosta;
        /// <summary>
        /// Sadece kişiye havale edilme durumu
        /// </summary>
        public int kisiyeHavale;
        public int detayDagitim;
        public string dagitimBirimKod;
        public string dagitimBirimAd;

        /// <summary>
        /// Birim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Birim()
        {
            kod = "";
            idareKimlikKodu = "";
            ad = "";
            kisaAd = "";
            genelEvrakMi = 0;
            seviye = 0;
            detay = false;
            kullanilmiyor = 0;
            ustBirim = "";
            ustBirimAd = "";
            ePosta = "";
            kisiyeHavale = 0;
            detayDagitim = 0;
            dagitimBirimKod = "";
            dagitimBirimAd = "";
        }

    }

    /// <summary>
    /// Dosya Planı bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DosyaPlani
    {
        public string kod = "";
        public string ad = "";
        public int seviye = 0;
        public bool detay = false;
        public int kullanilmiyor = 0;
        public int saklamaSuresi = 0;
        public int saklamaKriteri = 0;
        public TNSDateTime acilisTarihi = new TNSDateTime();
        public TNSDateTime kapanisTarihi = new TNSDateTime();
        public int tur = 0;
        public int islemKod = 0;
        public int sureBaslangicTipi = 0;
        public int tasfiyePlanTipi = 0;
        public string gerekce = "";
        public string aciklama = "";
        public bool aramadaKendisiniGetir = false;
        public DosyaPlaniBolumTanim bolumTanim = new DosyaPlaniBolumTanim();
        public string bakimdakiKodlar = ""; //Belge Yöneticisi bakım amaçlı olarak açtığı kapalı konumdaki dosya plan kodlarını taşır
        public int gizlilik = 0;
        public string anahtarKelime = "";
        public int saklamaPlanTipi = 0;
        public int adTipi = 0;
    }

    /// <summary>
    /// Dosya Planı Tarihçe bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DosyaPlaniTarihce
    {
        public string islemKod = "";
        public string kod = "";
        public int kodTur = 0;
        public TNSDateTime tarih = new TNSDateTime();
        public string kolon = "";
        public string kolonAciklama = "";
        public string eskiDeger = "";
        public string yeniDeger = "";
        public ENUMDosyaPlaniTarihceIslem islem = ENUMDosyaPlaniTarihceIslem.TANIMSIZ;
        public string islemAciklama = "";
        public string kullaniciKodu = "";
        public TNSDateTime baslangicTarih = new TNSDateTime();
        public TNSDateTime bitisTarih = new TNSDateTime();
        public int acilanDosya = 0;
        public int kapananDosya = 0;
    }

    /// <summary>
    /// Dosya Planı Birim bilgilerini tatan sınıf
    /// </summary>
    [Serializable()]
    public class DosyaPlaniBirim
    {
        /// <summary>
        /// Evrak Birim Kodu
        /// </summary>
        public string evrakBirimKodu = "";
        /// <summary>
        /// Dosya Plan bilgisinin tutulduğu nesne
        /// </summary>
        public DosyaPlani dosyaPlani = new DosyaPlani();
    }

    [Serializable()]
    public class DosyaPlaniBolumTanim
    {
        /// <summary>
        /// Dosya planı Kodu
        /// </summary>
        public string hesapKodu = "";
        /// <summary>
        /// Bölüm adı
        /// </summary>
        public string bolumAdi = "";
        /// <summary>
        /// durumu
        /// </summary>
        public int durumu = 0;
        /// <summary>
        /// periyod
        /// </summary>
        public int periyod = 0;
        /// <summary>
        /// baslangicGun
        /// </summary>
        public int baslangicGun = 0;
        /// <summary>
        /// bitisGun
        /// </summary>
        public int bitisGun = 0;
        /// <summary>
        /// baslangicAy
        /// </summary>
        public int baslangicAy = 0;
        /// <summary>
        /// bitisAy
        /// </summary>
        public int bitisAy = 0;
        /// <summary>
        /// sonislem
        /// </summary>
        public int sonIslem = 0;
        /// <summary>
        /// tur
        /// </summary>
        public int tur = 0;
        /// <summary>
        /// SaklamaOrtamı
        /// </summary>
        public int saklamaOrtami = 0;
        /// <summary>
        /// fizikiSaklamaYeri
        /// </summary>
        public string fizikiSaklamaYeri = "";
    }

    [Serializable()]
    public class DosyaPlaniBolum
    {
        /// <summary>
        /// Bölüm Kodu
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Klasör Kodu
        /// </summary>
        public int klasorKod = 0;
        /// <summary>
        /// Dosya planı Kodu
        /// </summary>
        public string hesapKodu = "";
        /// <summary>
        /// Bölüm adı
        /// </summary>
        public string bolumAdi = "";
        /// <summary>
        /// durumu
        /// </summary>
        public int durumu = 0;
        /// <summary>
        /// baslangicTarihi
        /// </summary>
        public TNSDateTime baslangic = new TNSDateTime();
        /// <summary>
        /// bitisTarihi
        /// </summary>
        public TNSDateTime bitis = new TNSDateTime();
        /// <summary>
        /// dosyaPlani
        /// </summary>
        public DosyaPlani dosyaPlani = new DosyaPlani();
        /// <summary>
        /// tur
        /// </summary>
        public int tur = 0;
        /// <summary>
        /// SaklamaOrtamı
        /// </summary>
        public int saklamaOrtami = 0;
        /// <summary>
        /// fizikiSaklamaYeri
        /// </summary>
        public string fizikiSaklamaYeri = "";
    }

    /// <summary>
    /// Tanımlama bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Tanimlama
    {
        /// <summary>
        /// Kodu
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Adı
        /// </summary>
        public string ad = "";
        /// <summary>
        /// Derece
        /// </summary>
        public int derece = 0;

    }

    /// <summary>
    /// Evrak Sicil bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakSicil
    {
        /// <summary>
        /// Evrak evrEvrakNosu
        /// </summary>
        public string evrEvrakNo = "";
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo = "";
        /// <summary>
        /// Asil bilgisi
        /// </summary>
        public int asil = 0;
    }

    /// <summary>
    /// Evrak İlişki bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakIliski
    {
        /// <summary>
        /// Evrak evrEvrakNo
        /// </summary>
        public string evrEvrakNo = "";
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo = "";
        /// <summary>
        /// İlişkisi tutulan evrak'ın prEvrakNosu
        /// </summary>
        public string prEvrakNoIliski = "";
        /// <summary>
        /// Evrak Tipi
        /// </summary>
        public int tip = 0;
    }

    /// <summary>
    /// Evrak bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakSinif
    {
        /// <summary>
        /// Evrak Yılı
        /// </summary>
        public int yil;
        /// <summary>
        /// Evrak evrEvrakNosu
        /// </summary>
        public string evrEvrakNo;
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo;
        /// <summary>
        /// Evrak Tipi. Giden-Gelen 
        /// </summary>
        public int evrakTipi;
        /// <summary>
        /// Evrak Birim Kodu
        /// </summary>
        public string evrakBirimKodu;
        /// <summary>
        /// Evrak Numarası
        /// </summary>
        public int evrakNo;
        /// <summary>
        /// Evrak Sorgu Numarası. Sorgu alanındaki 2. bilgi için
        /// </summary>
        public int sorguEvrakNo;
        /// <summary>
        /// Evrak Tarih bilgisinin tutulduğu nesne
        /// </summary>
        public TNSDateTime evrakTarih;
        /// <summary>
        /// Evrak Sorgu Tarihi. Sorgu alanındaki 2. bilgi için
        /// </summary>
        public TNSDateTime sorguEvrakTarih;
        /// <summary>
        /// Geldiği Yer Türü
        /// </summary>
        public int geldigiYerTur;
        /// <summary>
        /// Geldiği Yer Adı
        /// </summary>
        public string geldigiYer;
        public string geldigiYerKod;
        /// <summary>
        /// Gelen Evrak Tarihi
        /// </summary>
        public TNSDateTime gelenEvrakTarih;
        /// <summary>
        /// Gelen Evrak'ın Evrak Numarası
        /// </summary>
        public string gelenEvrakNo;
        /// <summary>
        /// Gizlilik
        /// </summary>
        public int gizlilik;
        /// <summary>
        /// Geliş Şekli
        /// </summary>
        public int gelisSekli;
        /// <summary>
        /// Yazı Durumu
        /// </summary>
        public int yaziDurum;
        /// <summary>
        /// Konusu
        /// </summary>
        public string konu;
        /// <summary>
        /// Açıklaması
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Cevap Süresi
        /// </summary>
        public int cevapSuresi;
        /// <summary>
        /// Ek Adedi
        /// </summary>
        public string ekAdedi;
        /// <summary>
        /// Gizlilik Adı
        /// </summary>
        public string gizlilikAd;
        /// <summary>
        /// Yazı Durum Adı
        /// </summary>
        public string yaziDurumAd;
        /// <summary>
        /// Geliş Şekli Adı
        /// </summary>
        public string gelisSekliAd;
        /// <summary>
        /// Evrak'ın Bulunduğu Yerin Adı.Kullanıcının aktif evrak birimini taşıyor. Listelemede evrakın son bulunduğu yeri
        /// </summary>
        public string bulunduguYer;
        /// <summary>
        /// Havale Birim Kodu
        /// </summary>
        public string havaleBirimKodu;
        /// <summary>
        /// Havale Birim Adı
        /// </summary>
        public string havaleBirimAdi;
        /// <summary>
        /// Dağıtım bilgisinin tutulduğu nesne
        /// </summary>
        public ArrayList dagitim;
        /// <summary>
        /// Paraf içeriği
        /// </summary>
        public string paraf;
        /// <summary>
        /// Evrrak Türü
        /// </summary>
        public int evrakTuru;
        /// <summary>
        /// Evrak Türünün Adı
        /// </summary>
        public string evrakTuruAd;
        /// <summary>
        /// Antet İçeriği
        /// </summary>
        public string antet;
        /// <summary>
        /// Evrak Metin İçeriği
        /// </summary>
        public string icerik;
        /// <summary>
        /// İmza Bilgisi 1
        /// </summary>
        public string imza1;
        /// <summary>
        /// İmza Bilgisi 2
        /// </summary>
        public string imza2;
        /// <summary>
        /// İmza Bilgisi 3
        /// </summary>
        public string imza3;
        /// <summary>
        /// Ek bilgisi
        /// </summary>
        public string ek;
        /// <summary>
        /// Alt Bilgi İçeriği
        /// </summary>
        public string altBilgi;
        /// <summary>
        /// Evrak Oluşturma Tarihi
        /// </summary>
        public TNSDateTime evrakOlusturmaTarihi;
        /// <summary>
        /// İmza Tarihi
        /// </summary>
        public TNSDateTime imzaTarihi;
        /// <summary>
        /// prEvrakNo Liste. Liste halinde verilen prEvrakNo (; ayıraçlı) bilgileri üzerinde işlem yapmak için
        /// </summary>
        public string prEvrakNoListe;
        /// <summary>
        /// evrEvrakNo Liste. Liste halinde verilen evrEvrakNo (; ayıraçlı) bilgileri üzerinde işlem yapmak içinListe halinde verilen evrEvrakNo (; ayıraçlı) bilgileri üzerinde işlem yapmak için
        /// </summary>
        public string evrEvrakNoListe;
        /// <summary>
        /// Aktarim
        /// </summary>
        public bool aktarim;
        /// <summary>
        /// Sorgu İçeriği
        /// </summary>
        public string sorguIcerik;
        /// <summary>
        /// Ek Yazı Alanı
        /// </summary>
        public string ekYaziAlan;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Dosya Numarası
        /// </summary>
        public string dosyaNo;
        /// <summary>
        /// Evrak İçeriği
        /// </summary>
        public string evrakIcerigi;
        /// <summary>
        /// Evrak Cinsi
        /// </summary>
        public int evrakCinsi;
        /// <summary>
        /// Evrakın dağıtım birimlerine havale edilip edilmemesini belirler
        /// </summary>
        public bool dagitimBirimlerineHavale;
        public string esasNo;
        public int bilgiEdinme;
        public int kisiselBilgi;
        public int dosyasiVar;
        public string bakanlikEvrakNo;
        public string baskanlikEvrakNo;
        /// <summary>
        /// Giden Evrakın Dağıtım Tarihi
        /// </summary>
        public TNSDateTime dagitimTarih;
        public string dinamikSayfaKod;
        public string dinamikSurecNo;
        public string surecAd;
        public int kayitIslem;
        public int resmiBelge;
        public int dil;
        public int eImzalandi;
        public TNSDateTime eImzaTarihi;
        public string eImzalayan;
        public string uretici;
        public TNSDateTime uretimTarihi;
        public int imzaBitti;
        public string uretimVersiyon;
        public TNSDateTime gizlilikGecerlilikSuresi;
        public TNSDateTime sorguGizlilikGecerlilikSuresi;
        public TNSDateTime iletimTarihi;
        public TNSDateTime arsivlemeTarihi;
        public TNSDateTime transferTarihi;
        public TNSDateTime imhaTarihi;
        public int ilgileriEkteGoster;
        public TNSDateTime kepGonderimTarih;
        public int editorGuncellenmeli;
        public int asilNushaGidecek;

        public int havaleyeGoreOlusanEvrakSayisi = 0;

        /// <summary>
        public bool sablonXmlKayit;
        public int evrakBelgeTur;
        public int imzaYolu;
        public bool imzaYoluDegisti;
        public WebServisXmlEkParametre webServisXmlEkParametre;
        public Arsiv arsiv;
        public bool gizliEvrakIslemi;

        /// <summary>
        /// Evrak Hareket bilgisinin tutulduğu nesne
        /// </summary>
        public EvrakHareket hareket;
        /// <summary>
        /// Evrak Tarihçesin tutulduğu nesne
        /// </summary>
        public EvrakTarihce tarihce;
        /// <summary>
        /// Evrak tasfiye planının tutulduğu nesne
        /// </summary>
        public EvrakTasfiye tasfiye;
        /// <summary>
        /// Evrak saklama planının tutulduğu nesne
        /// </summary>
        public EvrakSaklama saklama;
        /// <summary>
        /// Evrak faks  tutulduğu nesne
        /// </summary>
        public EvrakFaks evrakFaks;
        /// <summary>
        /// Evrak faks tarihce planının tutulduğu nesne
        /// </summary>
        public EvrakFaksTarihce evrakFaksTarihce;
        /// <summary>
        /// Evrak faks tarihce planının tutulduğu nesne
        /// </summary>
        public EvrakFaksKimlik evrakFaksKimlik;
        /// <summary>
        /// Doysa Kodlarının tutulduğu nesne
        /// </summary>
        public ArrayList saklamaPlanlari;
        /// <summary>
        /// Doysa Kodlarının tutulduğu nesne
        /// </summary>
        public ArrayList dosyaKodlari;
        /// <summary>
        /// Evrak İlgi Alanının tutulduğu nesne
        /// </summary>
        public ArrayList evrakIlgi;
        /// <summary>
        /// Evrak Özel Alan bilgisinin tutulduğu nesne
        /// </summary>
        public ArrayList evrakOzelAlan;
        /// <summary>
        /// Evrak Sicil bilgisi
        /// </summary>
        public EvrakSicil sicil;
        /// <summary>
        /// Evrak Havale Hareket bilgisinin tutulduğu nesne
        /// </summary>
        public ArrayList hareketHavale;
        /// <summary>
        /// Evrak İçerik Arama bilgisinin tutulduğu nesne
        /// </summary>
        public EvrakIcerikArama icerikArama;
        /// <summary>
        /// Evrak İlişki bilgisinin tutulduğu nesne
        /// </summary>
        public ArrayList iliski;
        public string siralamaKolonu = "";

        public EvrakDosya evrDosya;

        public EvrakEtiket etiket;

        public Hashtable parametre;

        public Hashtable sorguKriterleri;

        public Hashtable kayitKriterleri;

        public ENUMEvrakSinifIslem islem;

        /// <summary>
        /// EvrakSinif sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EvrakSinif()
        {
            yil = 0;
            evrEvrakNo = "";
            prEvrakNo = "";
            evrakBirimKodu = "";
            evrakNo = 0;
            sorguEvrakNo = 0;
            evrakTarih = new TNSDateTime();
            sorguEvrakTarih = new TNSDateTime();
            geldigiYerTur = 0;
            geldigiYer = "";
            gelenEvrakTarih = new TNSDateTime();
            gelenEvrakNo = "";
            gizlilik = -1;
            gelisSekli = -1;
            yaziDurum = -1;
            konu = "";
            aciklama = "";
            cevapSuresi = -1;
            ekAdedi = "";
            gizlilikAd = "";
            yaziDurumAd = "";
            gelisSekliAd = "";
            bulunduguYer = "";
            havaleBirimKodu = "";
            evrakTipi = 0;
            dagitim = new ArrayList();
            paraf = "";
            evrakTuru = -1;
            evrakTuruAd = "";
            antet = "";
            icerik = "";
            imza1 = "";
            imza2 = "";
            imza3 = "";
            ek = "";
            altBilgi = "";
            evrakOlusturmaTarihi = new TNSDateTime();
            imzaTarihi = new TNSDateTime();
            prEvrakNoListe = "";
            evrEvrakNoListe = "";
            aktarim = false;
            sorguIcerik = "";
            ekYaziAlan = "";
            siraNo = 0;
            dosyaNo = "";
            evrakIcerigi = "";
            evrakCinsi = -1;
            dagitimBirimlerineHavale = false;
            esasNo = "";
            bilgiEdinme = -1;
            kisiselBilgi = -1;
            dosyasiVar = -1;
            bakanlikEvrakNo = "";
            baskanlikEvrakNo = "";
            dagitimTarih = new TNSDateTime();
            dinamikSayfaKod = "";
            dinamikSurecNo = "";
            surecAd = "";
            kayitIslem = (int)ENUMEvrakKayitIslem.GENEL;
            resmiBelge = -1;
            dil = 0;
            eImzalandi = -1;
            eImzaTarihi = new TNSDateTime();
            eImzalayan = "";
            uretici = "";
            uretimTarihi = new TNSDateTime();
            imzaBitti = -1;
            uretimVersiyon = "";
            gizlilikGecerlilikSuresi = new TNSDateTime();
            sorguGizlilikGecerlilikSuresi = new TNSDateTime();
            iletimTarihi = new TNSDateTime();
            arsivlemeTarihi = new TNSDateTime();
            transferTarihi = new TNSDateTime();
            imhaTarihi = new TNSDateTime();
            ilgileriEkteGoster = -1;
            kepGonderimTarih = new TNSDateTime();
            editorGuncellenmeli = -1;

            sablonXmlKayit = false;
            evrakBelgeTur = 0;
            imzaYolu = 0;
            webServisXmlEkParametre = new WebServisXmlEkParametre();
            arsiv = new Arsiv();
            gizliEvrakIslemi = false;

            hareket = new EvrakHareket();
            tarihce = new EvrakTarihce();
            tasfiye = new EvrakTasfiye();
            saklama = new EvrakSaklama();
            saklamaPlanlari = new ArrayList();
            dosyaKodlari = new ArrayList();
            evrakIlgi = new ArrayList();
            evrakOzelAlan = new ArrayList();
            sicil = new EvrakSicil();
            hareketHavale = new ArrayList();
            icerikArama = new EvrakIcerikArama();
            iliski = new ArrayList();
            evrakFaks = new EvrakFaks();
            evrakFaksTarihce = new EvrakFaksTarihce();
            evrakFaksKimlik = new EvrakFaksKimlik();
            evrDosya = new EvrakDosya();
            etiket = new EvrakEtiket();
            parametre = new Hashtable();
            sorguKriterleri = new Hashtable();
            kayitKriterleri = new Hashtable();
            islem = (int)ENUMEvrakSinifIslem.GENEL;
        }

        /// <summary>
        /// Verilen parametrelere göre sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="evrEvrakNo">evrEvrakNo bilgisini içeren nesne</param>
        /// <param name="prEvrakNo">prEvrakNo bilgisini içeren nesne</param>
        public EvrakSinif(string evrEvrakNo, string prEvrakNo)
        {
            this.evrEvrakNo = evrEvrakNo;
            this.prEvrakNo = prEvrakNo;
        }
    }

    /// <summary>
    /// Evrak bilgilerinin bir kısmını tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakSinifKucuk
    {
        /// <summary>
        /// Evrak evrEvrakNosu
        /// </summary>
        public string evrEvrakNo;
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo;
        /// <summary>
        /// Evrak Numarası
        /// </summary>
        public int evrakNo;
        /// <summary>
        /// Evrak Tarih bilgisinin tutulduğu nesne
        /// </summary>
        public TNSDateTime evrakTarih;
        /// <summary>
        /// Evrak Konusu
        /// </summary>
        public string konuYer;
        /// <summary>
        /// Evrak Hareket işlme kodu
        /// </summary>
        public int hareketIslem;
        /// <summary>
        /// Evrak Tipi
        /// </summary>
        public int evrakTipi;
        /// <summary>
        /// Evrak Hareket okundu bilgisi
        /// </summary>
        public int hareketOkundu;
        /// <summary>
        /// Evrak Konusu
        /// </summary>
        public string konu;
        /// <summary>
        /// Evrak'ın Geldiği Yer
        /// </summary>
        public string geldigiYer;
        /// <summary>
        /// Evrak Türü
        /// </summary>
        public int evrakTuru;
        /// <summary>
        /// Evrak Asil mi bilgisi
        /// </summary>
        public int sicilAsil;
        /// <summary>
        /// Cevap Süresi
        /// </summary>
        public int cevapSuresi;
        /// <summary>
        /// Evrak İçerik Arama bilgisi
        /// </summary>
        public EvrakIcerikArama icerikArama;
    }

    /// <summary>
    /// Evrak İçerik Arama bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakIcerikArama
    {
        /// <summary>
        /// Klasör bilgisi
        /// </summary>
        public string klasor = "";
        /// <summary>
        /// Dosya Adı
        /// </summary>
        public string dosyaAd = "";
        /// <summary>
        /// Derece bilgisi
        /// </summary>
        public int derece = 0;
        /// <summary>
        /// Evrak evrEvrakNo bilgisi
        /// </summary>
        public string evrEvrakNo = "";

        public int dosyaSiraNo = 0;
    }

    /// <summary>
    /// Evrak İlgi bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakIlgi
    {
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo = "";
        /// <summary>
        /// İlgi Sıra Numarası
        /// </summary>
        public int siraNo = 0;
        /// <summary>
        /// İlgi Tarihi
        /// </summary>
        public TNSDateTime ilgiTarihi = new TNSDateTime();
        /// <summary>
        /// İlgi Numarası
        /// </summary>
        public string ilgiNo = "";

        public string aciklama = "";
    }

    /// <summary>
    /// Evrak Tarihçe bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakTarihce
    {
        /// <summary>
        /// Evrak evrEvrakNosu
        /// </summary>
        public string evrEvrakNo = "";
        /// <summary>
        /// İşlem Tarihi
        /// </summary>
        public TNSDateTime islemTarih = new TNSDateTime();
        /// <summary>
        /// İşlem Yapan Kullanıcının TCKimlikNosu
        /// </summary>
        public string islemYapan = "";
        /// <summary>
        /// İşlem Yapan Kullanıcının Adı
        /// </summary>
        public string islemYapanAd = "";
        /// <summary>
        /// İşlem Adı
        /// </summary>
        public string islem = "";
        public string vekil = "";
        public string vekilAd = "";
    }

    /// <summary>
    /// Evrak Hareket bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakHareket
    {
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo;
        /// <summary>
        /// Evrak hareket sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Evrak'ı kaydededen birimin Adı
        /// </summary>
        public string birimKod1Ad;
        /// <summary>
        /// Evrak'ı kaydededen birimin Kodu
        /// </summary>
        public string birimKod1;
        /// <summary>
        /// Evrak'ın gönderildiği birimin Adı
        /// </summary>
        public string birimKod2Ad;
        /// <summary>
        /// Evrak'ın gönderildiği birimin Kodu
        /// </summary>
        public string birimKod2;
        /// <summary>
        /// Evrak'ı kaydededen kişinin TcKimlik numarası
        /// </summary>
        public string kisi1;
        /// <summary>
        /// Evrak'ı kaydededen kişinin adı
        /// </summary>
        public string kisi1Ad;
        /// <summary>
        /// Evrak'ın gönderildiği kişinin TcKimlik numarası
        /// </summary>
        public string kisi2;
        /// <summary>
        /// Evrak'ın gönderildiği kişinin adı
        /// </summary>
        public string kisi2Ad;
        /// <summary>
        /// Yapılan işlem hareketinin tarihi
        /// </summary>
        public TNSDateTime islemTarih;
        /// <summary>
        /// Sorgu işlem tarihi
        /// </summary>
        public TNSDateTime sorguIslemTarih;
        /// <summary>
        /// İşlem yapan kişinin TcKimlik numarası
        /// </summary>
        public string islemYapan;
        /// <summary>
        /// İşlem yapan kişinin adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// Hareket işlem kodu
        /// </summary>
        public int islem;
        /// <summary>
        /// Havale notu
        /// </summary>
        public string havaleNotu;
        /// <summary>
        /// Evrak okundu durumu
        /// </summary>
        public int okundu;
        /// <summary>
        /// Evrak havale tür kodu
        /// </summary>
        public int havaleTur;

        public string vekil;
        public string vekilAd;
        public int erisemez;

        /// <summary>
        /// EvrakHareket sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EvrakHareket()
        {
            prEvrakNo = "";
            siraNo = 0;
            birimKod1 = "";
            birimKod1Ad = "";
            birimKod2 = "";
            birimKod2Ad = "";
            kisi1 = "";
            kisi2 = "";
            kisi2Ad = "";
            kisi1Ad = "";
            islemTarih = new TNSDateTime();
            sorguIslemTarih = new TNSDateTime();
            islemYapan = "";
            islemYapanAd = "";
            islem = 0;
            havaleNotu = "";
            okundu = -1;
            havaleTur = 0;
            vekil = "";
            vekilAd = "";
            erisemez = 0;
        }
    }


    //[System.Serializable()]
    //public class EvrakBirimYetkiSinif
    //{
    //    public string evrakBirimKodu;
    //    public string[] altBirimKodlar;
    //}

    /// <summary>
    /// Antet bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Antet
    {
        /// <summary>
        /// Antet adı
        /// </summary>
        public string antet = "";
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBirimKodu = "";
        /// <summary>
        /// Evrak birim adı
        /// </summary>
        public string evrakBirimAd = "";
    }

    /// <summary>
    /// Altbilgi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Altbilgi
    {
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKod = "";
        /// <summary>
        /// Altbilgi içeriği
        /// </summary>
        public string altbilgi = "";
        public string kurumAdres = "";
        public string ayrintiliBilgi = "";
        public string telefon = "";
        public string faks = "";
        public string ePosta = "";
        public string elektronikAg = "";
    }

    /// <summary>
    /// Paraf bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Paraf
    {
        /// <summary>
        /// Paraf kodu
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKod = "";
        /// <summary>
        /// Paraf adı
        /// </summary>
        public string ad = "";
        /// <summary>
        /// Paraf detay bilgilerini içeren nesne
        /// </summary>
        public ArrayList detaylar = new ArrayList();
    }

    /// <summary>
    /// Paraf Detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ParafDetay
    {
        /// <summary>
        /// Paraf Kodu
        /// </summary>
        public int parafKodu;
        /// <summary>
        /// Paraf detay sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Kişi adı
        /// </summary>
        public string ad;
        /// <summary>
        /// Kişi soyadı
        /// </summary>
        public string soyad;
        /// <summary>
        /// Kişi unvanı
        /// </summary>
        public string unvan;

        /// <summary>
        /// ParafDetay sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ParafDetay()
        {
            parafKodu = 0;
            siraNo = 0;
            ad = "";
            soyad = "";
            unvan = "";
        }
    }

    /// <summary>
    /// Evrak Konu bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class EvrakKonu
    {
        /// <summary>
        /// Birim
        /// </summary>
        public string birim = "";
        /// <summary>
        /// Konu
        /// </summary>
        public string konu = "";
        /// <summary>
        /// Kullanın Sayısı
        /// </summary>
        public int kullanimSayisi = 0;
    }

    /// <summary>
    /// Evrak Yer bililerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class EvrakYer
    {
        /// <summary>
        /// Birim
        /// </summary>
        public string birim;
        /// <summary>
        /// Yer adı
        /// </summary>
        public string yerAd;
        /// <summary>
        /// Yer türü
        /// </summary>
        public int yerTur;
        /// <summary>
        /// Yer birim kodu
        /// </summary>
        public string yerBirimKod;
        /// <summary>
        /// Kullanım sayısı
        /// </summary>
        public int kullanimSayisi;

        /// <summary>
        /// EvrakYer sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EvrakYer()
        {
            birim = "";
            yerAd = "";
            yerTur = 0;
            yerBirimKod = "";
            kullanimSayisi = 0;
        }
    }

    /// <summary>
    /// Dağıtım bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Dagitim
    {
        /// <summary>
        /// Dağıtım kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBrimKodu;
        /// <summary>
        /// Dağıtım adı
        /// </summary>
        public string ad;
        /// <summary>
        /// Dağıtım alanlarını tutan nesne
        /// </summary>
        public ArrayList alanlar;

        /// <summary>
        /// Dagitim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Dagitim()
        {
            kod = 0;
            evrakBrimKodu = "";
            ad = "";
            alanlar = new ArrayList();
        }
    }

    /// <summary>
    /// Dağıtım Alan bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DagitimAlan
    {
        /// <summary>
        /// Dağıtım kodu
        /// </summary>
        public int dagitimKodu;
        /// <summary>
        /// Dağıtım alan sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Dağıtım alan yeri
        /// </summary>
        public string yer;
        /// <summary>
        /// Dağıtım alan birim kodu
        /// </summary>
        public string evrakBrimKodu;
        /// <summary>
        /// Dağıtım alan arzRica bilgisi
        /// </summary>
        public int arzRica;
        /// <summary>
        /// Dağıtım alan kurum içi, kurum dışı bilgisi
        /// </summary>
        public int kurumBilgi;
        public string kisi;

        /// <summary>
        /// DagitimAlan sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public DagitimAlan()
        {
            dagitimKodu = 0;
            siraNo = 0;
            yer = "";
            evrakBrimKodu = "";
            arzRica = 0;
            kurumBilgi = 0;
            kisi = "";
        }
    }

    /// <summary>
    /// Şablon bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Sablon
    {
        /// <summary>
        /// Şablon kodu
        /// </summary>
        public int kod;
        /// <summary>
        /// Şablon adı
        /// </summary>
        public string ad;
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKodu;
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBrimKodu;
        /// <summary>
        /// Şablon kayıt tarihi
        /// </summary>
        public TNSDateTime kayitTarihi;
        /// <summary>
        /// Evrak türü
        /// </summary>
        public int evrakTuru;
        /// <summary>
        /// Gizlilik bilgisi
        /// </summary>
        public int gizlilik;
        /// <summary>
        /// Antet içeriği
        /// </summary>
        public string antet;
        /// <summary>
        /// Yazı durumu
        /// </summary>
        public int yaziDurum;
        /// <summary>
        /// Dosya plan kodu
        /// </summary>
        public string dosyaPlanKodu;
        /// <summary>
        /// Konusu
        /// </summary>
        public string konu;
        /// <summary>
        /// Gideceği yeri
        /// </summary>
        public string gidecegiYer;
        /// <summary>
        /// Metin içerik
        /// </summary>
        public string icerik;
        /// <summary>
        /// Ek bilgisi
        /// </summary>
        public string ek;
        /// <summary>
        /// Dağıtım bilgisi
        /// </summary>
        public ArrayList dagitim;
        /// <summary>
        /// Paraf içeriği
        /// </summary>
        public string paraf;
        /// <summary>
        /// Altbilgi içeriği
        /// </summary>
        public string altBilgi;
        /// <summary>
        /// Şablon detayı
        /// </summary>
        public int detay;

        /// <summary>
        /// Sablon sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Sablon()
        {
            kod = 0;
            ad = "";
            kullaniciKodu = "";
            evrakBrimKodu = "";
            kayitTarihi = new TNSDateTime();
            evrakTuru = 0;
            gizlilik = 0;
            antet = "";
            yaziDurum = 0;
            dosyaPlanKodu = "";
            konu = "";
            gidecegiYer = "";
            icerik = "";
            ek = "";
            dagitim = new ArrayList();
            paraf = "";
            altBilgi = "";
            detay = 0;
        }
    }

    /// <summary>
    /// Evrak Dağıtım bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class EvrakDagitim
    {
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo;
        /// <summary>
        /// Evrak dağıtım sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Evrak dağıtım kodu
        /// </summary>
        public string dagitimKod;
        /// <summary>
        /// Evrak dağıtım adı
        /// </summary>
        public string dagitimAd;
        /// <summary>
        /// Evrak dağıtım gereği-bilgi kodu
        /// </summary>
        public int geregiBilgi;

        /// <summary>
        /// EvrakDagitim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EvrakDagitim()
        {
            prEvrakNo = "";
            siraNo = 0;
            dagitimKod = "";
            dagitimAd = "";
            geregiBilgi = 0;
        }
    }

    /// <summary>
    /// Sablon Dağıtım bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SablonDagitim
    {
        /// <summary>
        /// Şablon kodu
        /// </summary>
        public int SablonKod;
        /// <summary>
        /// Şablon dağıtım sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Şablon dağıtım kodu
        /// </summary>
        public string dagitimKod;
        /// <summary>
        /// Şablon dağıtım adı
        /// </summary>
        public string dagitimAd;
        /// <summary>
        /// Şablon dağıtım gereği-bilgi kodu
        /// </summary>
        public int geregiBilgi;

        /// <summary>
        /// SablonDagitim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SablonDagitim()
        {
            SablonKod = 0;
            siraNo = 0;
            dagitimKod = "";
            dagitimAd = "";
            geregiBilgi = 0;
        }
    }

    /// <summary>
    /// Gönderilen Evrak bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class EvrakGonderilen
    {
        /// <summary>
        /// Evrak prEvrakNosu
        /// </summary>
        public string prEvrakNo;
        /// <summary>
        /// Gönderilen evrak birim kodu
        /// </summary>
        public string gonderilenBirimKodu;
        /// <summary>
        /// Gönderen evrak birim kodu
        /// </summary>
        public string gonderenBirimKodu;
        /// <summary>
        /// Evrak konusu
        /// </summary>
        public string konu;
        /// <summary>
        /// Evrak tarihi
        /// </summary>
        public TNSDateTime evrakTarih;
        /// <summary>
        /// Evrak numarası
        /// </summary>
        public int evrakNo;
        /// <summary>
        /// Gizlilik bilgisi
        /// </summary>
        public int gizlilik;
        /// <summary>
        /// Yazı durumu
        /// </summary>
        public int yaziDurum;
        /// <summary>
        /// Evrak dosya kodu
        /// </summary>
        public string evrakDosyaKodu;
        /// <summary>
        /// Dosya
        /// </summary>
        public string dosya;
        /// <summary>
        /// Alındımı bilgisi
        /// </summary>
        public string alindimi;

        /// <summary>
        /// EvrakGonderilen sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EvrakGonderilen()
        {
            prEvrakNo = "";
            gonderilenBirimKodu = "";
            gonderenBirimKodu = "";
            konu = "";
            evrakTarih = new TNSDateTime();
            evrakNo = 0;
            gizlilik = 0;
            yaziDurum = 0;
            evrakDosyaKodu = "";
            dosya = "";
            alindimi = "";
        }
    }

    /// <summary>
    /// İmza bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Imza
    {
        /// <summary>
        /// Beyan edilen imzalama tarihi
        /// </summary>
        public TNSDateTime beyanEdilenImzalamaTarihi;
        /// <summary>
        /// İmzalama tarihi
        /// </summary>
        public TNSDateTime imzalamaZamani;
        /// <summary>
        /// Sertifikası
        /// </summary>
        public ImzaSertifika sertifika;

        /// <summary>
        /// Imza sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Imza()
        {
            beyanEdilenImzalamaTarihi = new TNSDateTime();
            imzalamaZamani = new TNSDateTime();
            sertifika = new ImzaSertifika();
        }
    }

    /// <summary>
    /// İmza sertifikasını tutan sınıf
    /// </summary>
    [Serializable()]
    public class ImzaSertifika
    {
        /// <summary>
        /// Başlangıç tarihi
        /// </summary>
        public TNSDateTime baslangicTarihi;
        /// <summary>
        /// Bitiş tarihi
        /// </summary>
        public TNSDateTime bitisTarihi;
        /// <summary>
        /// E-posta adresi
        /// </summary>
        public string ePosta;
        /// <summary>
        /// TcKimlik Numarası
        /// </summary>
        public string kimlikNo;
        /// <summary>
        /// İmza sertifika sahibi
        /// </summary>
        public string sahibi;

        /// <summary>
        /// ImzaSertifika sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ImzaSertifika()
        {
            baslangicTarihi = new TNSDateTime();
            bitisTarihi = new TNSDateTime();
            ePosta = string.Empty;
            kimlikNo = string.Empty;
            sahibi = string.Empty;
        }
    }

    /// <summary>
    /// İmza sonuç bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class ImzaSonuc
    {
        /// <summary>
        /// Mesaj
        /// </summary>
        public string message;
        /// <summary>
        /// Sonuç kodu
        /// </summary>
        public int resultCode;
        /// <summary>
        /// İşlem başarılı mı bilgisi
        /// </summary>
        public bool isSuccessful;
        /// <summary>
        /// apTransId
        /// </summary>
        public string apTransId;
        /// <summary>
        /// Parmak İzi
        /// </summary>
        public string parmakIzi;
        /// <summary>
        /// Şonuç nesnesi
        /// </summary>
        public Sonuc sonuc;
        /// <summary>
        /// İmzalı dosyanın içeriği
        /// </summary>
        public byte[] imzaliDosya;
        /// <summary>
        /// İmzalanacak dosyanın içeriği
        /// </summary>
        public byte[] imzalanacakDosya;

        /// <summary>
        /// ImzaSonuc sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ImzaSonuc()
        {
            message = string.Empty;
            resultCode = 0;
            isSuccessful = false;
            apTransId = string.Empty;
            parmakIzi = string.Empty;
            sonuc = new Sonuc();
            imzaliDosya = null;
            imzalanacakDosya = null;
        }
    }

    /// <summary>
    /// Evrak Haberci bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Haberci
    {
        /// <summary>
        /// Haberci kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Kullanıcı 
        /// </summary>
        public string kullanici;
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBrimKodu;
        /// <summary>
        /// Haberci adı
        /// </summary>
        public string ad;
        /// <summary>
        /// Haberci baslangıç tarihi
        /// </summary>
        public TNSDateTime baslangicTarih;
        /// <summary>
        /// Haberci bitiş tarihi
        /// </summary>
        public TNSDateTime bitisTarih;
        /// <summary>
        /// Haberci detaylarını tutan nesne
        /// </summary>
        public ArrayList detaylar;

        /// <summary>
        /// Haberci  sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Haberci()
        {
            kod = 0;
            kullanici = "";
            evrakBrimKodu = "";
            ad = "";
            baslangicTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();
            detaylar = new ArrayList();
        }
    }

    /// <summary>
    /// Haberci detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HaberciDetay
    {
        /// <summary>
        /// Haberci kodu
        /// </summary>
        public int haberciKodu;
        /// <summary>
        /// Haberci detay kodu
        /// </summary>
        public int kod;
        /// <summary>
        /// Haberci detay alanı
        /// </summary>
        public int alan;
        /// <summary>
        /// Haberci detay işleç bilgisi
        /// </summary>
        public int islec;
        /// <summary>
        /// Haberci detay değeri
        /// </summary>
        public string deger;

        /// <summary>
        /// HaberciDetay sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HaberciDetay()
        {
            haberciKodu = 0;
            kod = 0;
            alan = 0;
            islec = 0;
            deger = "";
        }
    }

    /// <summary>
    /// Havale kural bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HavaleKural
    {
        /// <summary>
        /// Havale kural kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Kullanıcı TcKimlik Nosu
        /// </summary>
        public string kullanici;
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBirimKodu;
        /// <summary>
        /// Havale kural adı
        /// </summary>
        public string ad;
        /// <summary>
        /// Havale kural başlangıç tarihi
        /// </summary>
        public TNSDateTime baslangicTarih;
        /// <summary>
        /// Havale kural bitiş tarihi
        /// </summary>
        public TNSDateTime bitisTarih;
        /// <summary>
        /// Havale kural gereği-bilgi 
        /// </summary>
        public ArrayList gbler;
        /// <summary>
        /// Havale kural detayları
        /// </summary>
        public ArrayList detaylar;

        /// <summary>
        /// HavaleKural sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HavaleKural()
        {
            kod = 0;
            kullanici = "";
            evrakBirimKodu = "";
            ad = "";
            baslangicTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();
            gbler = new ArrayList();
            detaylar = new ArrayList();
        }
    }

    /// <summary>
    /// Havale kural gereği-bilgi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HavaleKuralGB
    {
        /// <summary>
        /// Havale kural kodu
        /// </summary>
        public int havaleKuralKodu;
        /// <summary>
        /// Havale kural gereği-bilgi kodu
        /// </summary>
        public int kod;
        /// <summary>
        /// Havale kural gereği-bilgi değeri
        /// </summary>
        public string deger;
        /// <summary>
        /// Havale kural gereği-bilgi durumu
        /// </summary>
        public int geregiBilgi;

        /// <summary>
        /// HavaleKuralGB sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HavaleKuralGB()
        {
            havaleKuralKodu = 0;
            kod = 0;
            deger = "";
            geregiBilgi = 0;
        }
    }

    /// <summary>
    /// Havale kural detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class HavaleKuralDetay
    {
        /// <summary>
        /// Havale kural kodu
        /// </summary>
        public int havaleKuralKodu;
        /// <summary>
        /// Havale kural detay kodu
        /// </summary>
        public int kod;
        /// <summary>
        /// Havale kural detay alanı
        /// </summary>
        public int alan;
        /// <summary>
        /// İşleç durumu
        /// </summary>
        public int islec;
        /// <summary>
        /// Değeri
        /// </summary>
        public string deger;

        /// <summary>
        /// HavaleKuralDetay sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HavaleKuralDetay()
        {
            havaleKuralKodu = 0;
            kod = 0;
            alan = 0;
            islec = 0;
            deger = "";
        }
    }

    /// <summary>
    /// Posta işlem bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class PostaIslem
    {
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBirimKodu;
        /// <summary>
        /// Posta işlem yılı
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge numarası
        /// </summary>
        public int belgeNo;
        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime belgeTarih;
        /// <summary>
        /// Kullanıcı TcKimlik numarası
        /// </summary>
        public string kullaniciKodu;
        /// <summary>
        /// Kullanıcı adı
        /// </summary>
        public string kullaniciAdi = "";
        /// <summary>
        /// Kayıt tarihi
        /// </summary>
        public TNSDateTime kayitTarih;
        /// <summary>
        /// Posta işlem detay bilgilerini tutan nesne
        /// </summary>
        public ArrayList detay;
        /// <summary>
        /// Sorgu belge numarası. Sorgu için
        /// </summary>
        public int sorguBelgeNo;
        /// <summary>
        /// Sorgu belge tarihi. Sorgu için
        /// </summary>
        public TNSDateTime sorguBelgeTarih;

        public TNSDateTime evrakTarih1;
        public TNSDateTime evrakTarih2;

        /// <summary>
        /// Sorgu için
        /// </summary>
        public int sorguEvrakTuru;

        /// <summary>
        /// PostaIslem sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PostaIslem()
        {
            evrakBirimKodu = "";
            yil = 0;
            belgeNo = 0;
            belgeTarih = new TNSDateTime();
            kullaniciKodu = "";
            kullaniciAdi = "";
            kayitTarih = new TNSDateTime();
            detay = new ArrayList();
            sorguBelgeNo = 0;
            sorguBelgeTarih = new TNSDateTime();
            evrakTarih1 = new TNSDateTime();
            evrakTarih2 = new TNSDateTime();
            sorguEvrakTuru = 0;
        }
    }

    /// <summary>
    /// Posta işlem detay bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class PostaIslemDetay
    {
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBirimKodu;
        /// <summary>
        /// Yılı
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge numarası
        /// </summary>
        public int belgeNo;
        /// <summary>
        /// Posta işlem detay sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Evrak evrEvrakNosu
        /// </summary>
        public string evrEvrakNo;
        /// <summary>
        /// Evrak numarası
        /// </summary>
        public int evrakNo;
        /// <summary>
        /// Gideceği yeri
        /// </summary>
        public string gidecegiYer;
        /// <summary>
        /// Posta türü
        /// </summary>
        public int postaTur;
        /// <summary>
        /// Normal fiyatı
        /// </summary>
        public decimal fiyatNormal;
        /// <summary>
        /// Postane fiyatı
        /// </summary>
        public decimal fiyatPostane;
        /// <summary>
        /// Barkod kodu
        /// </summary>
        public string barkod;
        public int sayfaNo;
        public int zarfNo;
        public string ekBilgisi;
        public ObjectArray dagitimListesi;
        public int gizlilik = 0;
        public string gizlilikAd = "";
        public string konu = "";
        public string hazirlayanBirimKodu = "";
        public string hazirlayanBirimAdi = "";
        public TNSDateTime gidenEvrakTarih = new TNSDateTime();
        public string dagitimBirimKodu = "";
        public string dagitimBirimAdi = "";
        public string baskanlikEvrakNo = "";
        public string bakanlikEvrakNo = "";
        public string aciklama = "";

        /// <summary>
        /// PostaIslemDetay sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PostaIslemDetay()
        {
            evrakBirimKodu = "";
            yil = 0;
            belgeNo = 0;
            siraNo = 0;
            evrEvrakNo = "";
            evrakNo = 0;
            gidecegiYer = "";
            postaTur = 0;
            fiyatNormal = (decimal)0;
            fiyatPostane = (decimal)0;
            barkod = "";
            sayfaNo = 0;
            zarfNo = 0;
            ekBilgisi = "";
            dagitimListesi = new ObjectArray();
            dagitimBirimKodu = "";
            dagitimBirimAdi = "";
            baskanlikEvrakNo = "";
            bakanlikEvrakNo = "";
            aciklama = "";
        }
    }

    /// <summary>
    /// Personel bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Personel
    {
        /// <summary>
        /// TcKimlik numarası
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Sicil numarası
        /// </summary>
        public string sicilNo;
        /// <summary>
        /// Personel adı
        /// </summary>
        public string ad;
        /// <summary>
        /// Personel soyadı
        /// </summary>
        public string soyad;
        /// <summary>
        /// E-postası
        /// </summary>
        public string eposta;
        /// <summary>
        /// Personel durumu
        /// </summary>
        public int durum;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public string muhasebeKodu;
        /// <summary>
        /// Unvanı
        /// </summary>
        public string unvan;
        /// <summary>
        /// Evrak birim kodu
        /// </summary>
        public string evrakBirimKodu;

        public string akademikUnvan;

        /// <summary>
        /// Personel sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Personel()
        {
            tcKimlikNo = "";
            sicilNo = "";
            ad = "";
            soyad = "";
            eposta = "";
            durum = 0;
            muhasebeKodu = "";
            unvan = "";
            evrakBirimKodu = "";
            akademikUnvan = "";
        }
    }

    /// <summary>
    /// Evrak değişiklik bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class EvrakDegisiklik
    {

        /// <summary>
        /// Evrak evrEvrakNo
        /// </summary>
        public string evrEvrakNo;
        /// <summary>
        /// Kullanıcı tcKimlik numarası
        /// </summary>
        public string kullanici;
        /// <summary>
        /// Kayıt tarihi
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Evrak değişiklik bilgileri
        /// </summary>
        public string deger;
        public string islemYapanAd;


        /// <summary>
        /// EvrakOzelAlan sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EvrakDegisiklik()
        {
            evrEvrakNo = "";
            kullanici = string.Empty;
            tarih = new TNSDateTime();
            deger = string.Empty;
            islemYapanAd = string.Empty;
        }
    }


    /// <summary>
    /// Evrak versiyon bilgilerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class EvrakVersiyon : EvrakSinif
    {
        public int kod;
        public string versiyonAd;
        public int versiyonMu;
        public string islemYapan;
        public string islemYapanAd;
        public TNSDateTime islemTarih;

        public EvrakVersiyon()
        {
            Varsayilan();
        }

        public EvrakVersiyon(EvrakSinif es)
        {
            base.yil = es.yil;
            base.evrEvrakNo = es.evrEvrakNo;
            base.prEvrakNo = es.prEvrakNo;
            base.evrakBirimKodu = es.evrakBirimKodu;
            base.evrakNo = es.evrakNo;
            base.sorguEvrakNo = es.sorguEvrakNo;
            base.evrakTarih = es.evrakTarih;
            base.sorguEvrakTarih = es.sorguEvrakTarih;
            base.geldigiYerTur = es.geldigiYerTur;
            base.geldigiYer = es.geldigiYer;
            base.gelenEvrakTarih = es.gelenEvrakTarih;
            base.gelenEvrakNo = es.gelenEvrakNo;
            base.gizlilik = es.gizlilik;
            base.gelisSekli = es.gelisSekli;
            base.yaziDurum = es.yaziDurum;
            base.konu = es.konu;
            base.aciklama = es.aciklama;
            base.cevapSuresi = es.cevapSuresi;
            base.ekAdedi = es.ekAdedi;
            base.gizlilikAd = es.gizlilikAd;
            base.yaziDurumAd = es.yaziDurumAd;
            base.gelisSekliAd = es.gelisSekliAd;
            base.bulunduguYer = es.bulunduguYer;
            base.havaleBirimKodu = es.havaleBirimKodu;
            base.evrakTipi = es.evrakTipi;
            base.dagitim = es.dagitim;
            base.paraf = es.paraf;
            base.evrakTuru = es.evrakTuru;
            base.evrakTuruAd = es.evrakTuruAd;
            base.antet = es.antet;
            base.icerik = es.icerik;
            base.imza1 = es.imza1;
            base.imza2 = es.imza2;
            base.imza3 = es.imza3;
            base.ek = es.ek;
            base.altBilgi = es.altBilgi;
            base.evrakOlusturmaTarihi = es.evrakOlusturmaTarihi;
            base.imzaTarihi = es.imzaTarihi;
            base.prEvrakNoListe = es.prEvrakNoListe;
            base.evrEvrakNoListe = es.evrEvrakNoListe;
            base.aktarim = es.aktarim;
            base.sorguIcerik = es.sorguIcerik;
            base.ekYaziAlan = es.ekYaziAlan;
            base.siraNo = es.siraNo;
            base.dosyaNo = es.dosyaNo;
            base.evrakIcerigi = es.evrakIcerigi;
            base.evrakCinsi = es.evrakCinsi;
            base.esasNo = es.esasNo;
            base.evrakBelgeTur = es.evrakBelgeTur;
            base.imzaYolu = es.imzaYolu;

            base.hareket = es.hareket;
            base.tarihce = es.tarihce;
            base.dosyaKodlari = es.dosyaKodlari;
            base.evrakIlgi = es.evrakIlgi;
            base.evrakOzelAlan = es.evrakOzelAlan;
            base.sicil = es.sicil;
            base.hareketHavale = es.hareketHavale;
            base.icerikArama = es.icerikArama;
            base.iliski = es.iliski;
            base.saklamaPlanlari = es.saklamaPlanlari;
            Varsayilan();
        }

        private void Varsayilan()
        {
            this.kod = 0;
            this.versiyonAd = "";
            this.versiyonMu = 0;
            this.islemYapan = "";
            this.islemYapanAd = "";
            this.islemTarih = new TNSDateTime();
        }
    }


    [Serializable()]
    public class ImzaYolu
    {
        public int kod;
        public string ad;
        public string tcKimlikNo;
        public ImzaYoluDetay[] detaylar;

        public ImzaYolu()
        {
            kod = 0;
            ad = "";
            tcKimlikNo = "";
        }
    }

    [Serializable()]
    public class ImzaYoluDetay
    {
        public int imzaYoluKod;
        public int siraNo;
        public int islemTip; //ENUMImzaYoluTip
        public string tcKimlikNo;
        public string ad;
        public string soyad;
        public string evrakBirimKod;
        public string unvan;
        public string akademikUnvan;
        public string evrakBirimAd;

        public ImzaYoluDetay()
        {
            imzaYoluKod = 0;
            siraNo = 0;
            islemTip = 0;
            tcKimlikNo = "";
            ad = "";
            soyad = "";
            evrakBirimKod = "";
            unvan = "";
            akademikUnvan = "";
            evrakBirimAd = "";
        }
    }

    [Serializable()]
    public class EvrakImzaYolu
    {
        public string prEvrakNo;
        public ImzaYolu imzaYolu;
        public EvrakImzaYoluHareket[] hareketler;

        public EvrakImzaYolu()
        {
            prEvrakNo = "";
            imzaYolu = new ImzaYolu();
        }
    }

    [Serializable()]
    public class EvrakImzaYoluHareket
    {
        public string prEvrakNo;
        public int siraNo;
        public string kisi1;
        public string kisi2;
        public int islemTip; /*ENUMEvrakHareketDurum GERIGONDERILDI = 11 ; IMZALANDI = 8 ; EIMZALANDI = 14; PARAFLANDI = 15 */
        public TNSDateTime islemTarih;
    }

    [Serializable()]
    public class WebServisXmlEkParametre
    {
        public bool WebServisIleKayit;
        public bool OtoEvrakNoAlma; //EvrakNo, Yıl, Evrak Tarihi otomatik değil parametre ile gelenler kaydedilsin
        public bool OtoYilAlma;
        public bool OtoEvrakTarihAlma;
        public bool SadeceEkDosyalariKaydet; //Web Servis ile evrakları ve ekleri değil, sadece kayıtlı olan evrakların ek dosyalarını kaydetmek için

        public WebServisXmlEkParametre()
        {
            WebServisIleKayit = false;
            OtoEvrakNoAlma = false;
            OtoYilAlma = false;
            OtoEvrakTarihAlma = false;
            SadeceEkDosyalariKaydet = false;
        }

    }

    [Serializable()]
    public class Arsiv
    {
        public int kod;
        public int anaKod;
        public int sira;
        public string ad;
        public int turKod;
        public string kodYol;
        public string islemYapan;
        public string evrakBirimKod;
        public ArsivYetki[] yetki;
        public ArsivMeta[] meta;
        public ArsivKullaniciBilgi kullaniciBilgi;

        public Arsiv()
        {
            kod = 0;
            anaKod = 0;
            sira = 0;
            ad = "";
            turKod = 0;
            kodYol = "";
            islemYapan = "";
            evrakBirimKod = "";
            kullaniciBilgi = new ArsivKullaniciBilgi();
        }
    }

    [Serializable()]
    public class EvrakArsiv
    {
        public string prEvrakNo;
        public Arsiv arsiv;
        public string aciklama;
        public string islemYapan;
        public string islemYapanAd;
        public int iptal;
        public TNSDateTime islemTarih;
        public EvrakArsivDosya dosya;
        public EvrakArsivMeta[] meta;

        public EvrakArsiv()
        {
            prEvrakNo = "";
            arsiv = new Arsiv();
            aciklama = "";
            islemYapan = "";
            islemTarih = new TNSDateTime();
            dosya = new EvrakArsivDosya();
        }
    }

    [Serializable()]
    public class ArsivYetki
    {
        public int arsivKod = 0;
        public string yetkiDeger = "";
        public int yetkiTip = 0;
        public string aciklama = "";
    }

    [Serializable()]
    public class ArsivKullaniciBilgi
    {
        public string ad = "";
        public string soyad = "";
        public string mernis = "";
        public string unvan = "";
    }

    [Serializable()]
    public class Meta
    {
        public int kod = 0;
        public string ad = "";
        public int tip = 0;
        public int zorunlu = 0;
        public string islemYapan = "";
        public string evrakBirimKod = "";
        public string detay = "";
        public int klasor = 0;
        public int grupKod = 0;
        public string grupAd = "";
    }

    [Serializable()]
    public class MetaGrup
    {
        public string birim = "";
        public string adi = "";
        public int kod = 0;
    }

    [Serializable()]
    public class ArsivMeta
    {
        public int arsivKod = 0;
        public Meta meta = new Meta();
    }


    [Serializable()]
    public class EvrakArsivMeta
    {
        public string prEvrakNo = "";
        public string metaDeger = "";
        public string metaDeger2 = "";
        public Meta meta = new Meta();
    }

    [Serializable()]
    public class EvrakArsivDosya
    {
        public string prEvrakNo = "";
        public int arsivKod = 0;
        public string dosyaAd = "";
        public string dosyaYol = "";
        public int dosyaAdet = 0;
    }

    [Serializable()]
    public class EvrakBilgi
    {
        public string prEvrakNo = "";
        public string kullaniciId = "";
        public string aciklama = "";
    }

    [Serializable()]
    public class EvrakBilgiIliski
    {
        public EvrakBilgi evrakBilgi = new EvrakBilgi();
        public string prEvrakNoIliski = "";
        public string evrakNoIliski = "";
        public int onay = 0;
    }

    [Serializable()]
    public class EvrakImha
    {
        public string prEvrakNo;
        public string evrEvrakNo;
        public int yil;
        public int evrakTip;
        public int evrakNo;
        public TNSDateTime evrakTarih;
        public string geldigiYer;
        public string konu;
        public TNSDateTime gelenEvrakTarih;
        public int gelenEvrakNo;
        public int saklamaSuresiYil;
        public TNSDateTime saklamaSuresiBitisTarih;
        public string dosyaPlanKod;
        public string dosyaPlanAd;


        public EvrakImha()
        {
            prEvrakNo = "";
            evrEvrakNo = "";
            yil = 0;
            evrakTip = 0;
            evrakNo = 0;
            evrakTarih = new TNSDateTime();
            geldigiYer = "";
            konu = "";
            gelenEvrakTarih = new TNSDateTime();
            gelenEvrakNo = 0;
            saklamaSuresiYil = 0;
            saklamaSuresiBitisTarih = new TNSDateTime();
            dosyaPlanKod = "";
            dosyaPlanAd = "";
        }
    }

    [Serializable()]
    public class HavaleFavori
    {
        public int kod;
        public string islemYapan;
        public int siraNo;
        public string havaleBirimKod;
        public string havalePersonelMernis;
        public string havalePersonelAd;
        public string havalePersonelSoyad;

        public HavaleFavori()
        {
            kod = 0;
            islemYapan = "";
            siraNo = 0;
            havaleBirimKod = "";
            havalePersonelMernis = "";
            havalePersonelAd = "";
            havalePersonelSoyad = "";
        }
    }

    [Serializable()]
    public class Vekalet
    {
        public int kod;
        public string kisi1;
        public string kisi2;
        public TNSDateTime tarih1;
        public TNSDateTime tarih2;
        public string islemYapan;
        public TNSDateTime islemTarih;
        public string kisi1Ad;
        public string kisi2Ad;
        public string islemYapanAd;
        public int aktif;
        public string birim1;

        public Vekalet()
        {
            kod = 0;
            kisi1 = "";
            kisi2 = "";
            tarih1 = new TNSDateTime();
            tarih2 = new TNSDateTime();
            islemYapan = "";
            islemTarih = new TNSDateTime();
            kisi1Ad = "";
            kisi2Ad = "";
            islemYapanAd = "";
            aktif = 0;
            birim1 = "";
        }
    }


    [Serializable()]
    public class DTVTBirim
    {
        public string idareKimlikKodu;
        public string parentIdareKimlikKodu;
        public string ad;
        public string kurumKodu;
        public string tipkod;
        public int altKurumVarmi;
        public string anakod;
        public string birimkod;
        public string dagilimkod1;
        public string dagilimkod2;
        public string dagilimkod3;
        public string dagilimkod4;
        public string altbirimkod1;
        public string altbirimkod2;
        public string altbirimkod3;
        public string altbirimkod4;
        public string adres;
        public string telefon;
        public string faks;
        public string webAdresi;
        public string ePosta;
        public string kepAdres;
        public DateTime sonGuncellemeZamani;
        public string disYazismaDurumu;
        public string durum;
        public int eYazismaServisTur;

        public DTVTBirim()
        {
            idareKimlikKodu = "";
            parentIdareKimlikKodu = "";
            ad = "";
            kurumKodu = "";
            tipkod = "";
            altKurumVarmi = 0;
            anakod = "";
            birimkod = "";
            dagilimkod1 = "";
            dagilimkod2 = "";
            dagilimkod3 = "";
            dagilimkod4 = "";
            altbirimkod1 = "";
            altbirimkod2 = "";
            altbirimkod3 = "";
            altbirimkod4 = "";
            adres = "";
            telefon = "";
            faks = "";
            webAdresi = "";
            ePosta = "";
            kepAdres = "";
            sonGuncellemeZamani = new DateTime();
            disYazismaDurumu = "";
            durum = "";
            eYazismaServisTur = 0;
        }
    }

    [Serializable()]
    public class DTVTBirimBilgi
    {
        public string idareKimlikKodu = "";
        public string kurumAd = "";
        public DateTime sonGuncellemeZamani = new DateTime();
        public int sonGuncellemeSayisi = 0;
    }

    [Serializable()]
    public class EvrakAraKriter
    {
        public int kod;
        public string ad;
        public string islemyapan;
        public TNSDateTime islemtarih;
        public string evrakbirimkodu;
        public string evrakbirimkodualt;
        public int yil;
        public int evraktipi;
        public TNSDateTime kayittarihi1;
        public TNSDateTime kayittarihi2;
        public int evrakno1;
        public int evrakno2;
        public string konu;
        public string geldigiyer;
        public int geldigiyertur;
        public string gelenevrakno;
        public TNSDateTime gelenevraktarih;
        public string icerik;
        public int yazisekli;
        public int gelissekli;
        public int gizlilik;
        public int yazidurum;
        public int tasfiyeKod;
        public int saklamaSuresi;
        public int saklamaKriteriKod;
        public int saklamaBaslangicKod;
        public int sorguTuru;
        public string dosyaNo;
        public string evrakIcerigi;

        public EvrakAraKriter()
        {
            kod = 0;
            ad = "";
            islemyapan = "";
            islemtarih = new TNSDateTime();
            evrakbirimkodu = "";
            evrakbirimkodualt = "";
            yil = 0;
            evraktipi = 0;
            kayittarihi1 = new TNSDateTime();
            kayittarihi2 = new TNSDateTime();
            evrakno1 = 0;
            evrakno2 = 0;
            konu = "";
            geldigiyer = "";
            geldigiyertur = 0;
            gelenevrakno = "";
            gelenevraktarih = new TNSDateTime();
            icerik = "";
            yazisekli = 0;
            gelissekli = 0;
            gizlilik = 0;
            yazidurum = 0;
            tasfiyeKod = 0;
            saklamaSuresi = 0;
            saklamaKriteriKod = 0;
            saklamaBaslangicKod = 0;
            sorguTuru = 0;
            dosyaNo = "";
            evrakIcerigi = "";
        }

    }

    [Serializable]
    public class POPAdres
    {
        public string hostAdres;
        public int port;
        public string kullanici;
        public string parola;
        public bool enableSsl;

        public POPAdres(string hostAdres, int port, string kullanici, string parola, bool enableSsl)
        {
            this.hostAdres = hostAdres;
            this.port = port;
            this.kullanici = kullanici;
            this.parola = parola;
            this.enableSsl = enableSsl;
        }
    }

    [Serializable]
    public class TNSContentType
    {
        public string Boundary;
        public string CharSet;
        public string MediaType;
        public string Name;
    }

    [Serializable]
    public class TNSMailEk
    {
        //public System.Net.Mime.ContentDisposition ContentDisposition;
        public String ContentId;
        public TNSContentType ContentType;
        public String Name;
        public Encoding NameEncoding;
        public System.Net.Mime.TransferEncoding TransferEncoding;
        public byte[] Content;
    }

    [Serializable]
    public class TNSMailMessage
    {
        public string Body = null;
        public ArrayList Ekler = null;
    }

    [Serializable()]
    public class EvrakArsivTakip
    {
        public string klasorad = "";
        public string islemyapan = "";
        public TNSDateTime kopyalatarih1 = new TNSDateTime();
        public TNSDateTime kopyalatarih2 = new TNSDateTime();
        public TNSDateTime kayittarih1 = new TNSDateTime();
        public TNSDateTime kayittarih2 = new TNSDateTime();
    }

    [Serializable()]
    public class EvrakImzaBilgi
    {
        public string imza1 = "";
        public string imza2 = "";
        public string imza3 = "";
        public string imza4 = "";
        public string paraf = "";

        public string imza1Isim = "";
        public string imza2Isim = "";
        public string imza3Isim = "";
        public string imza1Unvan = "";
        public string imza2Unvan = "";
        public string imza3Unvan = "";
    }

    [Serializable()]
    public class EvrakArsivTarihce
    {
        public TNSDateTime islemTarih = new TNSDateTime();
        public int islem = 0;
        public string islemYapan = "";
        public string islemYapanAd = "";
        public string prevrakNo = "";
    }

    [Serializable()]
    public class EvrakDosya
    {
        public string evrEvrakNo = "";
        public int siraNo = -1;
        public string dosyaYolu = "";
        public string dosyaAdi = "";
        public TNSDateTime dosyaTarihi = new TNSDateTime();
        public double dosyaBoyutu = 0.0;
        public double dosyaBoyutuSorgu = 0.0;
        public int arsivKod = -1;
        public int ustYaziDosyasi = -1;

        public int eImzaDosyasi = -1;
        public int eImzalananSiraNo = -1;
        public string eImzaGuid = "";

        public int kriptoYapildi = -1;
        public TNSDateTime kriptoTarihi = new TNSDateTime();

        public int ocrYapildi = -1;
        public string ocrGuid = "";
        public TNSDateTime ocrTarihi = new TNSDateTime();

        public int icerikYapildi = -1;
        public string icerikGuid = "";
        public TNSDateTime icerikTarihi = new TNSDateTime();
        public string icerik = "";

        public string islemYapan = "";
        public string islemYapanAd = "";
        public TNSDateTime islemTarih = new TNSDateTime();
        public string aciklama = "";
        public int iptal = -1;
        public TNSDateTime sorguDosyaTarihi1 = new TNSDateTime();
        public TNSDateTime sorguDosyaTarihi2 = new TNSDateTime();
    }

    [Serializable()]
    public class EvrakAyarlar
    {
        public string anahtarSahibi;
        public int anahtar;
        public string deger;
        public string deger2;

        public EvrakAyarlar()
        {
            anahtarSahibi = "";
            anahtar = 0;
            deger = "";
            deger2 = "";
        }
    }

    [Serializable()]
    public class EvrakArsivRaporEvrakSayisi
    {
        public string islemYapanAd;
        public int evrakSayisi;
        public int sayfaSayisi;
        public int islemDurum;

        public EvrakArsivRaporEvrakSayisi()
        {
            islemYapanAd = "";
            evrakSayisi = 0;
            sayfaSayisi = 0;
            islemDurum = 0;
        }
    }

    [Serializable()]
    public class Form
    {
        public string sayfaKod = "";
        public string sayfaAd = "";
        public int durum = 0;
        public int gidenEvrakOlarakKayitEdilebilir = 0;
        public string sablonKod = "";
        public string surecNo = "";
        public string evrBirimKod = "";
        public FormKontrol[] kontroller;
    }

    [Serializable()]
    public class FormKontrol
    {
        public string kontrolKod = "";
        public string sayfaKod = "";
        public string kontrolTur = "";
        public FormOzellik[] ozellikler;
    }

    [Serializable()]
    public class FormOzellik
    {
        public string kontrolKod = "";
        public string ozellikTur = "";
        public string sayfaKod = "";
        public string deger = "";
    }

    [Serializable()]
    public class FormOzellikler
    {
        public string panelId = "";
        public string componentTur = "";
        public string top = "";
        public string left = "";
        public string height = "";
        public string width = "";
        public string fieldName = "";
        public string fieldLabel = "";
        public string comboListItem = "";
        public string wcfParametre = "";
        public string tabIndex = "";
        public string evrakIliskisi = "";
    }

    [Serializable()]
    public class EvrakForm
    {
        public string evrEvrakNo = "";
        public string alanAdi = "";
        public string deger = "";
    }


    /// <summary>
    /// Evrak EVREVRAKTASFIYE bilgilerini tutan sınıf
    /// </summary>

    [Serializable()]
    public class EvrakTasfiye
    {
        public string prEvrakNo = "";
        public int tasfiyeKod = -1;
        public TNSDateTime tasfiyeTarihi = new TNSDateTime();
    }


    /// <summary>
    /// Evrak EVREVRAKSAKLAMA bilgilerini tutan sınıf
    /// </summary>

    [Serializable()]
    public class EvrakSaklama
    {
        public string kod = "";
        public string prEvrakNo = "";
        public int kriterKod = 0;
        public int baslangicKod = 0;
        public int sure = 0;
        public string sureBirim = "";
        public TNSDateTime bitisTarih = new TNSDateTime();
        public int islem = 0;
        public int tasfiyePlani = 0;
        public TNSDateTime raporTarih = new TNSDateTime();
        public TNSDateTime islemTarih = new TNSDateTime();
    }

    /// <summary>
    /// Evrak EVRFAKS bilgilerini tutan sınıf
    /// </summary>

    [Serializable()]
    public class EvrakFaks
    {
        public string kodu = "";
        public string birimKod = "";
        public string kimden = "";
        public string kime = "";
        public string aciklama = "";
        public string faksNo = "";
        public int oncelik = 0;
        public string kayitEden = "";
        public TNSDateTime kayitZamani = new TNSDateTime();
        public int teyit = 0;
        public TNSDateTime gonderilmeZamani = new TNSDateTime();
        public int durumu = 0;
        public int tekrarSayisi = 0;
        public int kapakSayfasi = 0;
        public int sayfaSayisi = 0;
        public string konu = "";
    }


    /// <summary>
    /// Evrak EVRFAKSTARIHCE bilgilerini tutan sınıf
    /// </summary>

    [Serializable()]
    public class EvrakFaksTarihce
    {
        public string tarihceKodu = "";
        public string kodu = "";
        public string kayitEden = "";
        public TNSDateTime kayitZamani = new TNSDateTime();
        public string kayitAciklama = "";
    }

    [Serializable()]
    public class EvrakFaksKimlik
    {
        public string kimlikKodu = "";
        public string birimKod = "";
        public TNSDateTime kayitZamani = new TNSDateTime();
    }

    [System.Serializable()]
    public class EvrakFizikselYeri
    {
        public string prEvrakNo = "";
        public string bina = "";
        public string kat = "";
        public string oda = "";
        public string bolum = "";
        public string raf = "";
        public string klasor = "";
        public string RFID = "";
        /// <summary>
        /// Sadece yetki kontrolü yapılırken kullanılmak üzere
        /// </summary>
        public string kullaniciAktifBirimi = "";
    }

    [Serializable()]
    public class EvrakDagitimHareket
    {
        public string prEvrakNo;
        public int siraNo;
        public int evrakNo;
        public TNSDateTime evrakTarihi;
        public string konu;
        public string dagitimYeri;
        public int dagitimTip;
        public string idariBirimKimlikKod;
        public string evrakBirimKod;
        public int islem;
        public TNSDateTime sonIslemTarihi;
        public string kisi;

        public EvrakDagitimHareket()
        {
            prEvrakNo = "";
            siraNo = 0;
            evrakNo = 0;
            evrakTarihi = new TNSDateTime();
            konu = "";
            dagitimYeri = "";
            dagitimTip = 0;
            idariBirimKimlikKod = "";
            evrakBirimKod = "";
            islem = 0;
            sonIslemTarihi = new TNSDateTime();
            kisi = "";
        }
    }

    [Serializable()]
    public class Etiket
    {
        public int kod = 0;
        public string ad = "";
        public int ustKod = 0;
        public string sahibi = "";
        public int detay = 0;
        public string sahibi2 = "";
    }

    [Serializable()]
    public class EvrakEtiket
    {
        public int etiketKod = 0;
        public int kod = 0;
        public string prEvrakNo = "";
    }

    [Serializable()]
    public class EYazismaBilgi
    {
        public int servisTur;
        public string servisIdariKimlikKod;
        public string servisKurumAd;
        public string kullaniciAd;
        public string parola;

        public EYazismaBilgi()
        {
            servisTur = 0;
            servisIdariKimlikKod = "";
            servisKurumAd = "";
            kullaniciAd = "";
            parola = "";
        }
    }

    [Serializable()]
    public class SayacBilgi
    {
        public string kod = "";
        public string deger = "";
    }

    [Serializable()]
    public class EvrakVersiyon2
    {
        public string evrEvrakNo;
        public int siraNo;
        public string guid;
        public TNSDateTime islemTarih;
        public string islemYapan;
        public string islemYapanAd;

        public EvrakVersiyon2()
        {
            evrEvrakNo = "";
            siraNo = 0;
            guid = "";
            islemTarih = new TNSDateTime();
            islemYapan = "";
            islemYapanAd = "";
        }
    }

    [Serializable()]
    public class EvrakEImzalandiBilgi
    {
        public string evrEvrakNo;
        public bool eImzalandi;
        public string eImzalayan;
        public TNSDateTime eImzaTarihi;

        public EvrakEImzalandiBilgi()
        {
            evrEvrakNo = "";
            eImzalandi = false;
            eImzalayan = "";
            eImzaTarihi = new TNSDateTime();
        }
    }

    [Serializable()]
    public class EvrakTasfiyeImha
    {
        public string evrakBirimKod;
        public string evrEvrakNo;
        public string prEvrakNo;
        public int yil;
        public int evrakTip;
        public int evrakNo;
        public TNSDateTime evrakTarih;
        public string geldigiYer;
        public string konu;
        public EvrakSaklama saklamaPlani;
        public string fizikselYeri;

        public EvrakTasfiyeImha()
        {
            evrakBirimKod = "";
            evrEvrakNo = "";
            prEvrakNo = "";
            yil = 0;
            evrakTip = 0;
            evrakNo = 0;
            evrakTarih = new TNSDateTime();
            geldigiYer = "";
            konu = "";
            saklamaPlani = new EvrakSaklama();
            fizikselYeri = "";
        }
    }

    [Serializable()]
    public class OrtakDosyalar
    {
        public int kod = 0;
        public int anaKod = 0;
        public string adi = "";
        public int turu = 0;
        public double boyut = 0.0;
        public string fizikselDosyaID = "";
        public string olusturan = "";
        public string olusturanAd = "";
        public TNSDateTime olusturmaTarihi = new TNSDateTime();
        public string degistiren = "";
        public string degistirenAd = "";
        public TNSDateTime degistirmeTarihi = new TNSDateTime();
        public bool birimKlasoruMu = false;
        public ArrayList yetkiler = new ArrayList();
        public int gosterilecekDosyaTuru = 0;
        public string dizin = "";
        public int eskiKod = 0;
        public int eskiAnaKod = 0;
        public string eskiFizikselDosyaID = "";
        public string[] yetkiliOlduguBirimler = new string[1];
        public bool paylasimVarMi = false;
        public bool notVarMi = false;
        public int seviye = 0;
        public string baglantiAdresi = "";
        public string yolKod = "";//Dosya veya klasörün üst dizin yapısı (kodları taşır)
        public string yolAdi = "";//Dosya veya klasörün üst dizin yapısı (adları taşır)
        public string aramaAdi = "";
        public int[] kodlar = new int[1];

        public static int TurKodAta(string tur)
        {
            if (tur == "KLASÖR")
                return (int)ENUMOrtakDosyalarDosyaTipleri.KLASOR;
            else if (tur == "DOSYA")
                return (int)ENUMOrtakDosyalarDosyaTipleri.DOSYA;
            else
                return (int)ENUMOrtakDosyalarDosyaTipleri.TANIMSIZ;
        }

        public static string TurKodVer(int tur)
        {
            if (tur == (int)ENUMOrtakDosyalarDosyaTipleri.KLASOR)
                return "KLASÖR";
            else if (tur == (int)ENUMOrtakDosyalarDosyaTipleri.DOSYA)
                return "DOSYA";
            else
                return "";
        }
    }

    [Serializable()]
    public class OrtakDosyalarTarihce
    {
        public int kod = 0;
        public int dosyaKod = 0;
        public int islemKod = 0;
        public string islemAd = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
        public string islemiYapan = "";
        public string islemiYapanAd = "";
        public string aciklama = "";
    }

    [Serializable()]
    public class OrtakDosyalarYetki
    {
        public int kod = 0;
        public int dosyaKod = 0;
        public string yetkiVerilen = "";
        public string yetkiVeren = "";
        public string yetkiVerilenAd = "";
        public string yetkiVerenAd = "";
        public int yetkiTur = 0;
        public bool gorme = false;
        public bool ekle = false;
        public bool epostaGonder = false;
        public bool adDegistir = false;
        public bool indir = false;
        public bool kopyala = false;
        public bool tasima = false;
        public bool silme = false;
        public bool surum = false;
        public bool yetki = false;
    }

    [Serializable()]
    public class OrtakDosyalarBirim
    {
        public int kod = 0;
        public string birimKod = "";
    }

    [Serializable()]
    public class OrtakDosyalarNot
    {
        public int kod = 0;
        public int dosyaKod = 0;
        public string ekleyenKisi = "";
        public string ekleyenKisiAd = "";
        public TNSDateTime eklemeTarihi = new TNSDateTime();
        public string notBilgisi = "";
    }

    [Serializable()]
    public class OrtakDosyalarSurumler
    {
        public int kod = 0;
        public int dosyaKod = 0;
        public string ekleyenKisi = "";
        public string ekleyenKisiAd = "";
        public TNSDateTime eklemeTarihi = new TNSDateTime();
        public double boyut = 0.0;
        public string fizikselDosyaID = "";
        public string adi = "";
        public string aciklama = "";
    }

    [Serializable()]
    public class OrtakDosyalarKullanimAlani
    {
        public float ayrilanAlan = 0f;
        public float kullanilanAlan = 0f;
        public float kullanimYuzdesi = 0f;
    }

    /// <summary>
    /// Duyuru Grubu bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DuyuruGrubu
    {
        public int grupID = 0;
        public string grupAdi = "";
        public string islemYapan = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
        public string islemYapanBirimKod = "";
        public ArrayList kisiler = new ArrayList();
    }

    /// <summary>
    /// Duyuru Grubu Kişi bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class DuyuruGrubuKisiler
    {
        public int grupKisilerID = 0;
        public int grupID = 0;
        public string KisiKimlikNo = "";
        public string KisiBirimKod = "";
        public int tip = 0;
    }

    [Serializable()]
    public class EvrakAdresDefteri
    {
        public int kod = 0;
        public string adres = "";
        public string adi = "";
        public string islemYapan = "";
        public string islemYapanBirimKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class EvrakAdresGrubuAdi
    {
        public int kod = 0;
        public string adi = "";
        public string islemYapan = "";
        public string islemYapanBirimKod = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class EvrakAdresGrubTablosu
    {
        public int kod = 0;
        public int grupKod = 0;
        public int adresKod = 0;
    }

    [Serializable()]
    public class EvrakSonraHavaleEt
    {
        public string prEvrakNo = "";
        public int siraNo = 0;
        public string evrakBirimKod = "";
        public string tcKimlikNo = "";
        public string havaleNot = "";
        public int gonderildi = 0;
    }

    [Serializable()]
    public class KEPGonderi
    {
        public string kod = "";
        public string evrakBirimKod = "";
        public string evrEvrakNo = "";
        public string kimden = "";
        public string kime = "";
        public string ilgiKisi = "";
        public string konu = "";
        public TNSDateTime tarih = new TNSDateTime();
        public string islemYapan = "";
        public int durum = 0;
        public string mime = "";
        public string smime = "";
    }

    [Serializable()]
    public class HavaleGrup
    {
        public int kod;
        public string ad;
        public string islemYapan;
        public string islemYapanAd;
        public string evrakBirimKod;
        public int paylasimDurumu;
        public int varsayilan;
        public HavaleGrupDetay[] detaylar;
        public HavaleGrupPaylas[] paylasimlar;

        public HavaleGrup()
        {
            kod = 0;
            ad = "";
            islemYapan = "";
            islemYapanAd = "";
            evrakBirimKod = "";
            paylasimDurumu = 0;
            varsayilan = 0;
            detaylar = null;
            paylasimlar = null;
        }
    }

    [Serializable()]
    public class HavaleGrupDetay
    {
        public int havaleGrupKod;
        public int siraNo;
        public string tcKimlikNo;
        public string ad;
        public string soyad;
        public string evrakBirimKod;
        public string evrakBirimAd;
        public int sayac;

        public HavaleGrupDetay()
        {
            havaleGrupKod = 0;
            siraNo = 0;
            tcKimlikNo = "";
            ad = "";
            soyad = "";
            evrakBirimKod = "";
            evrakBirimAd = "";
            sayac = 0;
        }
    }

    [Serializable()]
    public class HavaleGrupPaylas
    {
        public int havaleGrupKod;
        public string tcKimlikNo;
        public string ad;
        public string soyad;
        public string evrakBirimKod;
        public string evrakBirimAd;

        public HavaleGrupPaylas()
        {
            havaleGrupKod = 0;
            tcKimlikNo = "";
            ad = "";
            soyad = "";
            evrakBirimKod = "";
            evrakBirimAd = "";
        }
    }

    [Serializable()]
    public class EvrakMeta
    {
        public string evrEvrakNo = "";
        public string metaDeger = "";
        public string metaDeger2 = "";
        public Meta meta = new Meta();
    }

    [Serializable()]
    public class Rol
    {
        public int kod = 0;
        public string ad = "";

        public RolYetki[] yetkiler;
        public KullaniciRol[] kullanicilar;
    }

    [Serializable()]
    public class YetkiEnum
    {
        public int kod = 0;
        public string ad = "";
    }

    [Serializable()]
    public class RolYetki
    {
        public Rol rol = new Rol();
        public YetkiEnum yetkiEnum = new YetkiEnum();
        public int yetkiDeger = 0;
    }

    [Serializable()]
    public class KullaniciRol
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public Rol rol = new Rol();
    }

    [Serializable()]
    public class SaklamaPlani
    {
        public int kod = 0;
        public string ad = "";
        public int sure = 0;
        public int kriterKod = 0;
        public int baslangicKod = 0;
    }

    [Serializable()]
    public class GuncellemeBilgi
    {
        public string kod = "";
        public string tabloAd = "";
        public string aciklama = "";
        public TNSDateTime yayinTarihi = new TNSDateTime();
        public TNSDateTime islemTarihi = new TNSDateTime();
        public int islemDurumu = 0;
        public string sql = "";
    }

    [Serializable()]
    public class KepHesap
    {
        public int kod = 0;
        public int hesapTur = 0;
        public string hesap = "";
        public string hesapSahibi = "";
        public int varsayilan = 0;
        public string islemYapan = "";
        public string islemYapanAd = "";
        public string anahtar = "";
    }

    [Serializable()]
    public class KepEvrak
    {
        public int kod = 0;
        public string evrEvrakNo = "";
        public string gonderen = "";
        public string alici = "";
        public ENUMKEPEvrakIslem islem = ENUMKEPEvrakIslem.TANIMSIZ;
        public TNSDateTime islemTarihi = new TNSDateTime();
        public string islemYapan = "";
        public string islemYapanAd = "";
        public string kepId = "";
        public string kepMesajId = "";

        [System.Xml.Serialization.XmlIgnore]
        public EvrakSinif evrak = new EvrakSinif();
    }

    [Serializable()]
    public class VeritabaniIndex
    {
        public int objectId = 0;
        public string tabloAd = "";
        public string indexAd = "";
        public string indexTip = "";
        public double indexSize = 0.0;
        public double fragmentation = 0.0;
        public int fillFactor = 0;

        public string kolon = "";
        public string include = "";
    }


    [Serializable()]
    public class EvrakIsTalep
    {
        public int kod = 0;
        public string prEvrakNo = "";
        public string servisKayitKod = "";
        public string atananKisi = "";
        public string atananKisiAd = "";
        public string islemYapan = "";
        public string islemYapanAd = "";
        public TNSDateTime islemTarihi = new TNSDateTime();
        public ENUMEvrakIsTalepIslem islem = ENUMEvrakIsTalepIslem.TANIMSIZ;
        public string islemAciklama = "";
        public string detay = "";
    }

    [Serializable()]
    public class KepAdres
    {
        public int kod = 0;
        public string evrakBirimKod = "";
        public string adres = "";
        public ENUMKepAdresTip tip = ENUMKepAdresTip.TANIMSIZ;
        public int kullanimSayisi = 0;
        public int kalici = 0;
        public TNSDateTime islemTarihi = new TNSDateTime();
        public string islemYapan = "";
        public string islemYapanAd = "";
        public int iptal = 0;
    }

    [Serializable]
    public class BirimDagitimYetki
    {
        public string evrakBirimKod = "";
        public Birim dagitimBirimi = new Birim();
        public int siraNo = 0;
    }

    [Serializable]
    public class BirimHavaleYetki
    {
        public string evrakBirimKod = "";
        public Birim havaleBirimi = new Birim();
        public int siraNo = 0;
        public int sorguIslem = 0;
    }

    [Serializable]
    public class InfocomDosyaImza
    {
        public string evrEvrakNo = "";
        public int siraNo = 0;
        public string personIdnp = "";
        public string hash = "";
        public string sign = "";
        public string islemYapan = "";
        public string islemYapanAd = "";
        public TNSDateTime islemTarih = new TNSDateTime();
        public string vekil = "";
        public string vekilAd = "";
    }

    [Serializable]
    public class InfocomApiTarihce
    {
        public string kod = "";
        public string islemYapan = "";
        public string islemYapanAd = "";
        public TNSDateTime islemTarih = new TNSDateTime();
        public string personIdnp = "";
        public string organizationInn = "";
        public string servis = "";
        public string talep = "";
        public string cevap = "";
    }
}