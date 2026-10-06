using System;
using TNS.MUH;

namespace TNS.PER
{
    public enum ENUMPersonelIstihdamSekli
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        Sozlesmeli = 2,
        Sozlesmeli_4C = 3,
        Gecici_Isci = 5,
        MEMUR = 1,
        ISCI = 4,
        SSKBagkur = 20,
        OzelSektor = 21
    }

    public enum ENUMPERSONELTEL
    {
        TANIMSIZ = 0,
        CEP = 1,
        IS = 2,
        EV = 3,
    }

    public enum EnumKadroKontenjanIslemTur
    {
        Ihdas = 1,
        Tahsis = 2,
        Tenkis = 3,
        Degisiklik = 4
    }

    enum ENUMMEDENIHAL
    {
        EVLI = 1,
        BEKAR = 2,
        TANIMSIZ = 0,
    }

    public enum ENUMCINSIYET
    {
        ERKEK = 1,
        KADIN = 2,
        TANIMSIZ = 0,
    }

    enum ENUMASKERLIK
    {
        TAMAMLADI = 1,
        TECILLI = 2,
        MUAF = 3,
        TANIMSIZ = 0,
    }

    public enum ENUMYAKINLIK
    {
        ES = 1,
        COCUK = 2,
        ANNE = 3,
        BABA = 4,
        KARDES = 5,
        TANIMSIZ = 0,
    }

    public enum ENUMCOCUKDURUM
    {
        OZ = 1,
        UVEY = 2,
        EVLATEDINILMIS = 3,
        TANIMSIZ = 0,
    }

    public enum ENUMGOREVLENDIRMETUR
    {
        TANIMSIZ = 0,
        EGITIMVEREN = 1,
        EGITIMALAN = 2,
        SINAV = 3,
        VEKALET = 4,
        GOREV = 5,
        KESIF = 6,
        DENETIM = 7,
        DIGER = 8,
    }

    public enum ENUMIZINTIP
    {
        BELIRSIZ = 0,
        YILLIK = 1,
        SIHHI = 6,
    }

    public enum ENUMPERIZINTUR
    {
        YILLIK = 1,
        C104 = 2,//104C
        EVLENMEIZNI = 3,
        BABALIKIZNI = 4,
        OLUMIZNI = 5,
        SIHHI = 6,
        ANALIKIZNI = 7,
        AYLIKSIZ = 9,
        HEYET = 10,
        TEKHEKIM = 11,
        REFAKATIZNI = 12,
        SUTIZNI = 13,
        E104 = 14,//104E
        MAZERETINIZNI = 15,
        UCRETLI = 8,

    }

    public enum ENUMTARIHCE
    {
        KISISEL = 1,
        ADRES = 2,
        AILE = 3,
        OGRENIM = 4,
        YABANCIDIL = 5,
        IZINHAREKET = 6,
        MEMURIYET = 7,
        KADRO = 8,
        EGITIM = 9,
        CEZAODUL = 10,
        MALBILDIRIMI = 11,
        IZINDEVIR = 12,
        EGITIMTANIM=13,
    }

    public enum ENUMTARIHCEISLEMTIP
    {
        KAYDET = 1,
        GUNCELLE = 2,
        SIL = 3
    }

    public enum ENUMGOREVDURUM
    {
        TANIMSIZ = 0,
        YENI = 1,
        EVRAKNOALDI = 2,
        ONAY = 5,
        IPTAL = 9,
    }

    public enum ENUMBELGEFORMTIP
    {
        PERSONELAILE = 1,
        PERSONELKURS = 2,
        PERSONELOGRENIM = 3,
        PERSONELDIL = 4,
        PERSONELKADRO = 5,
        PERSONELMEMURIYET = 6
    }

    public enum ENUMKatilimTipi
    {
        KURSIYER = 1,
        EGITICI = 2,
        GOREVLI = 3
    }

    public enum ENUMYAKININDURUMU
    {
        OKUYOR = 1,
        MEZUN = 2,
        EVLI = 3,
        ÇALISIYOR = 4,
        CALISMIYOR = 5,
        MALUL = 6,
        OLU = 7,
        OZURLU = 8,
        BURSALIYOR = 9,
        EMEKLI = 10,
        OKULONCESI = 11,
        DIGER = 12
        
    }
    public enum ENUMRAPORTURU
    {
        HEKIM = 1,
        HEYET = 2
    }
    public enum ENUMKURSDONEMI
    {
        BIRINCI3AYLIK = 1,
        IKINCI3AYLIK = 2,
        UCUNCU3AYLIK = 3,
        DORDUNCU3AYLIK = 4
    }
    public enum ENUMEGITIMSEVIYEBILGISI
    {
        EGITIMGRUBU = 1,
        EGITIMTURU = 2,
        EGITIMADI = 3
    }
    /// <summary>
    /// Görevlendirme bilgisi.
    /// </summary>
    [Serializable()]
    public class Gorev
    {
        public int kod = 0;
        public int muhasebeKod = 0;
        public string muhasebeAd = string.Empty;
        public string birimKod = string.Empty;
        public string birimAd = string.Empty;
        public string amac = string.Empty;
        public int tur = 0;
        public int islemKod = 0;
        public int durum = 0;
        public ObjectArray gorevYerleri;
        public ObjectArray gorevliPersoneller;
        public string evrEvrakNo = string.Empty;
        public string prEvrakNo = string.Empty;
        public int orgKod = 0;
        public string kurum = string.Empty;
        public string vasita = string.Empty;
        public string avans = string.Empty;
        public int yil = 0;
        public string alan1 = string.Empty;
        public string imza1 = string.Empty;
        public string imza2 = string.Empty;
        public string imza3 = string.Empty;
    }

    /// <summary>
    /// Görevlendirme Yer Tarih bilgisi.
    /// </summary>
    [Serializable()]
    public class GorevYerTarih
    {
        public int gorevKod = 0;
        public int siraNo = 0;
        public string bulunduguIlKod = string.Empty;
        public string bulunduguIlceKod = string.Empty;
        public string bulunduguYer = string.Empty;
        public string gidecegiIlKod = string.Empty;
        public string gidecegiIlceKod = string.Empty;
        public string gidecegiYer = string.Empty;
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public int gorevYil = 0;
    }

    /// <summary>
    /// Görevlendirme Personel bilgisi.
    /// </summary>
    [Serializable()]
    public class GorevPersonel
    {
        public int gorevKod = 0;
        public string tckimlikno = string.Empty;
        public string ad = string.Empty;
        public string soyad = string.Empty;
        public int unvanKod = 0;
        public string unvan = string.Empty;
        public int kadroDerece = 0;
        public int ekGosterge = 0;
        public int gorevYil = 0;
    }
    /// <summary>
    /// Personel Aile bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelAile
    {
        public string personelTcKimlikNo = "";
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public TNSDateTime dogumTarihi = new TNSDateTime();
        public int cinsiyet = 0;
        public int yakinlik = 0;
        public int durum = 0;
        public string saglikKarneNo = "";
        public TNSDateTime evlenmeTarihi = new TNSDateTime();
        public string aileCuzdaniSeriNo = "";
        public string esininCalistigiKurum = "";
        public string esininUnvani = "";
        public string esininGorevYeri = "";
        public string cocugununOkulununAdi = "";
    }

    /// <summary>
    /// Personel Epitim(Kurs) Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelKurs
    {
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Sıra Numarası
        /// </summary>
        public int siraNo = 0;
        /// <summary>
        /// Kurs Numarası
        /// </summary>
        public int kursKod = 0;
        /// <summary>
        /// Kursu aldığı yer
        /// </summary>
        public string aldigiYer = "";
        /// <summary>
        /// Başlama tarihi
        /// </summary>
        public TNSDateTime baslamaTarihi = new TNSDateTime();
        /// <summary>
        /// Bitirme tarihi
        /// </summary>
        public TNSDateTime bitirmeTarihi = new TNSDateTime();
        /// <summary>
        /// Derece
        /// </summary>
        public string derece = "";
        /// <summary>
        /// Kurs adı
        /// </summary>
        public string kursAdi = "";
        /// <summary>
        /// Kurs-Ders saati
        /// </summary>
        public int dersSaati = 0;
        /// <summary>
        /// Eğitim yeri
        /// </summary>
        public string egitimYeri = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Dönem
        /// </summary>
        public int donem = 0;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil = 0;
        /// <summary>
        /// Dosya Numarası
        /// </summary>
        public string dosyaNo = "";
        /// <summary>
        /// Gün
        /// </summary>
        public int gun = 0;
        /// <summary>
        /// katılım Tipi
        /// </summary>
        public int katilimTipi = 0;
        /// <summary>
        /// kurs niteliği
        /// </summary>
        public string kursNiteligi = "";
        /// <summary>
        /// Eğitimin yapıldığı il bilgisi
        /// </summary>
        public string yapildigiIl = "";
        /// <summary>
        /// Toplam eğitim saati
        /// </summary>
        public int egitimSaati = 0;
        /// <summary>
        /// Eğitim Tür Kodu
        /// </summary>
        public int egitimTurKodu = 0; // Eğitim seviye 3
    }
    /// <summary>
    /// Personel Eğitim Türleri Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelEgitimTurleri
    {
        /// <summary>
        /// Eğitim Kodu
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Eğitim Atakodu
        /// </summary>
        public int ataKod = 0;
        /// <summary>
        /// Eğitim Adı
        /// </summary>
        public string ad = "";
        /// <summary>
        /// Eğitim Seviyesi
        /// </summary>
        public int seviye = 0;
    }
    /// <summary>
    /// Personel bilgisi.
    /// </summary>
    [Serializable()]
    public class Personel
    {
        public string TCKIMLIKNO { get { return tcKimlikNo; } set { tcKimlikNo = value; } }
        public string AD { get { return ad; } set { ad = value; } }
        public string SOYAD { get { return soyad; } set { soyad = value; } }
        public string UNVANAD { get { return unvanAd; } set { unvanAd = value; } }
        public int UNVANKOD { get { return unvanKodu; } set { unvanKodu = value; } }
        public int KADRODERECE { get { return kadroDerece; } set { kadroDerece = value; } }
        public int EKGOSTERGE { get { return emekliEsasGosterge; } set { emekliEsasGosterge = value; } }
        public string EPOSTA { get { return eposta; } set { eposta = value; } }

        public string tcKimlikNo = "";
        public string sicilNo { get; set; }
        public string ad = "";
        public string soyad = "";
        public string babaAd = "";
        public string anaAd = "";
        public TNSDateTime dogumTarihi = new TNSDateTime();
        public int cinsiyet = 0;
        public int medeniHali = 0;
        public string kanGrubu = "";
        public string isTelefon = "";
        public string cepTelefon = "";
        public string evTelefon = "";
        public string faks = "";
        public string eposta = "";
        public string adres = "";
        public string vergiDairesi = "";
        public int askerlikKodu = 0;
        public int ogrenimKodu = 0;
        public int esDurumuKodu = 0;
        public string saglikKarneNo = "";
        public int durum = 0;
        public string emekliSicilNo = "";
        public TNSDateTime guncellenmeTarihi = new TNSDateTime();
        public string guncelleyenKisi = "";

        //Banka
        public string bankaKodu { get; set; }
        public string bankaSubeKodu { get; set; }
        public string bankaHesapNo { get; set; }

        //Evrak
        public string evrakBirimKod = "";

        //Memuriyet
        public int teskilatKodu = 0;
        public int unvanKodu = 0;
        public int istihdamSekli { get; set; }
        public int[] istihdamSekliListe;
        public int muhasebeKodu = 0;
        public string birimKodu = "";
        public string birimAd = "";
        public TNSDateTime memurBaslamaTarihi = new TNSDateTime();
        public TNSDateTime iseBaslamaTarihi = new TNSDateTime();
        public TNSDateTime istenCikisTarihi = new TNSDateTime();
        public string daireAdi = "";

        //Kadro
        public int ustBirimKodu = 0;
        public int sinifKodu { get; set; }
        public int kidemAy = 0;
        public int kidemYil = 0;

        public int kadroDerece { get; set; }
        public TNSDateTime kadroTarih = new TNSDateTime();

        public int odemeEsasDerece { get; set; }
        public int odemeEsasKademe { get; set; }
        public int odemeEsasGosterge { get; set; }
        public int odemeEsasEkGosterge { get; set; }
        public TNSDateTime odemeEsasTarih = new TNSDateTime();

        public int emekliEsasDerece { get; set; }
        public int emekliEsasKademe { get; set; }
        public int emekliEsasGosterge { get; set; }
        public int emekliEsasEkGosterge { get; set; }
        public TNSDateTime emekliEsasTarih = new TNSDateTime();

        public int kazanilmisHakDerece = 0;
        public int kazanilmisHakKademe = 0;
        public int kazanilmisHakGosterge = 0;
        public int kazanilmisHakEkGosterge = 0;
        public TNSDateTime kazanilmisHakTarih = new TNSDateTime();

        public string unvanAd = "";
        public string unvanAdKisa = "";

        //Yeni v2
        public string odaKodu = "";
        public string ilkSoyad = "";
        public string dogumYeri = "";
        public string nufusIlKod = "";
        public string nufusIlceKod = "";
        public string cuzdanSeriNo = "";
        public string cuzdanKayitNo = "";
        public string nufusMahalleKoy = "";
        public string nufusCiltNo = "";
        public string nufusSiraNo = "";
        public string nufusAileSiraNo = "";
        public string cuzdanVerilisNedeni = "";
        public string cuzdanVerildigiYer = "";
        public TNSDateTime cuzdanVerilisTarihi = new TNSDateTime();
        public string bakanlikKartNo = "";
        public string sskSicilNo { get; set; }
        public string bagkurSicilNo = "";
        public string surucuBelgeNo = "";
        public TNSDateTime surucuBelgeTarihi = new TNSDateTime();
        public TNSDateTime yasTashihTarihi = new TNSDateTime();
        public int organizasyonKodu = 0;
        public string kurum = "";
        public string url = "";
        public string mobilImzaTelNo = "";
        public PersonelResim resim = new PersonelResim();
        public string askerlikMuafNeden = "";
        public string askerlikCuzdanNo = "";
        public TNSDateTime tecilTarihi = new TNSDateTime();
        public TNSDateTime askerlikBaslamaTarihi = new TNSDateTime();
        public TNSDateTime terhisTarihi = new TNSDateTime();
        public int mulkDurumu = 0;
        public TNSDateTime tasinmaTarihi = new TNSDateTime();
        public int oturmaSuresi = 0;
        public TNSDateTime sonGoreveBaslamaTarihi = new TNSDateTime();
        public TNSDateTime sonBirimeBaslamaTarihi = new TNSDateTime();
        public int kadroOrganizasyonKodu = 0;
        public int kadroMuhasebeKodu = 0;
        public string kadroBirimKodu = "";
        public string kadroKurum = "";
        public string tebligatAdresi = "";
        public string acilKisi = "";
        public string acilKisiNumara = "";
        public bool unvanaGoreSirala = false;

        public ObjectArray dil;
        public ObjectArray aile;
        public ObjectArray kurs;
        public ObjectArray malBildirimi;
        //Sözleşmeli Maaş Tanim
        public SozlesmeliBordroTanim sozlesmeliBordroTanim = new SozlesmeliBordroTanim();
        public bool yetkiKontroluIleListele = false;
        public int[] yetkiliOlduguBirimler = new int[0];

        public TNSCollection tcList = null;
        public int aktifMi = 1;
        public int kod = 0;

        public Personel()
        {
            sskSicilNo = "";
            sicilNo = "";
            bankaKodu = "";
            bankaSubeKodu = "";
            bankaHesapNo = "";
            istihdamSekli = 0;
        }

        public string adSoyadTc { get; set; }
    }

    [Serializable()]
    public class PersonelPersonelRapor
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public string unvan = "";
        public int cinsiyet = 0;
        public TNSDateTime dogumTarihi = new TNSDateTime();
        public string okulAdi = "";
        public int seviyesi = 0;
        public string seviyesiText = "";
        public TNSDateTime bitirmeTarihi = new TNSDateTime();
        public string LUOkulAdi = "";
        public int LUSeviyesi = 0;
        public string LUSeviyesiText = "";
        public TNSDateTime LUBitirmeTarihi = new TNSDateTime();
        public string dil = "";
        public int notuDil = 0;
        public string duzeyDil = "";
        public int yilDil = 0;
        public int ayDil = 0;
        public int belgeTurDil = 0;
        public int gecerliOkul = 0;
        public string fakulteBolum = "";

        public string sinif { get; set; }
    }

    [Serializable()]
    public class PersonelEgitimRapor
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public string unvan = "";
        public string egitiminAdi = "";
        public string egitiminTipi = "";
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public int dersSaati = 0;
        public int gun = 0;
        public string egitimiVerenKurulus = "";
        public string egitimYeri = "";
        public string aciklama = "";
        public int donemi = 0;
        public int yili = 0;
        public int katilimciSayisi = 0;
        public string dosyaNo = "";
        public int egitimTurKodu = 0;
        public int katilimTipi = 0;
    }

    [Serializable()]
    public class PersonelIzinRapor
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public string unvan = "";
        public string sicilNumarasi = "";
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public int yillikIzin = 0;
        public string aciklama = "";
        public int tip = 0;
    }

    [Serializable()]
    public class PersonelIzinDurum
    {
        public string tcKimlikNo = "";
        public int tip = 0;
        public string ad = "";
        public int toplamGun = 0;
        public int kullanilanGun = 0;
        public int listelemeYili = 0;
    }


    /// <summary>
    /// Personelin Eklediği Belgeler
    /// </summary>
    [Serializable()]
    public class PersonelBelgeler
    {
        public PersonelBelgeler()
        {
            this.tcKimlikNo = "";
            this.tip = 0;
            this.kayitSiraNo = "";
            this.belgeSiraNo = 0;
            this.belgeAdi = "";
        }

        public PersonelBelgeler(string tcKimlikNo = "", int tip = 0, string kayitSiraNo = "", int belgeSiraNo = 0, string belgeAdi = "")
        {
            this.tcKimlikNo = tcKimlikNo;
            this.tip = tip;
            this.kayitSiraNo = kayitSiraNo;
            this.belgeSiraNo = belgeSiraNo;
            this.belgeAdi = belgeAdi;
        }

        public PersonelBelgeler(string tcKimlikNo, int tip, string kayitSiraNo, string belgeAdi, int belgeBoyutu)
        {
            this.tcKimlikNo = tcKimlikNo;
            this.tip = tip;
            this.kayitSiraNo = kayitSiraNo;
            this.belgeSiraNo = 0;
            string dosyaAdi = belgeAdi;
            if (dosyaAdi.LastIndexOf('/') > 0)
                dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('/')).TrimStart('/');
            if (dosyaAdi.LastIndexOf('\\') > 0)
                dosyaAdi = dosyaAdi.Substring(dosyaAdi.LastIndexOf('\\')).TrimStart('\\');
            this.belgeAdi = dosyaAdi;
            this.belge = new byte[belgeBoyutu];
        }
        /// <summary>
        /// T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Hangi forma eklendiği
        /// </summary>
        public int tip;
        /// <summary>
        /// Formdaki hangi kayda eklendiği
        /// </summary>
        public string kayitSiraNo;
        /// <summary>
        /// Kaydın kaçıncı belgesi olduğu
        /// </summary>
        public int belgeSiraNo;
        /// <summary>
        /// Belge adı
        /// </summary>
        public string belgeAdi;
        /// <summary>
        /// Belge içeriği
        /// </summary>
        public byte[] belge;
    }


    /// <summary>
    /// Personel bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelUnvanKadro
    {
        public string unvanAd = "";
        public int cinsiyet = 0;
        public int cinsiyetAdet = 0;
    }

    /// <summary>
    /// Personel Resim Bilgisi
    /// </summary>
    [Serializable()]
    public class PersonelResim
    {
        /// <summary>
        /// T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Resim bilgisi
        /// </summary>
        public byte[] resim;
    }

    /// <summary>
    /// Personel Adres Sınıfı
    /// </summary>
    /// 
    [Serializable()]
    public class PersonelAdres
    {
        public string tcKimlikNo = "";
        public string ilKod = "";
        public string ilceKod = "";
        public string mahalle = "";
        public string caddeSokak = "";
        public string binaDaireNo = "";
        public string postaKodu = "";
        public string adres = "";
        public string tebligatAdres = "";
        public int mulkDurumu = 0;
        public TNSDateTime tasinmaTarihi = new TNSDateTime();
        public string isTel = "";
        public string evTel = "";
        public string cepTel = "";
        public string faks = "";
        public string eposta = "";
        public string url = "";
        public string mobilImzaTel = "";
        public string acilKisi = "";
        public string acilKisiNumara = "";
    }

    /// <summary>
    /// Personel Ceza Ödül Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelCezaOdul
    {
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int sirano = 0;
        /// <summary>
        /// Tip
        /// </summary>
        public int tip = 0;
        /// <summary>
        /// Veriliş nedeni
        /// </summary>
        public string verilisNedeni = "";
        /// <summary>
        /// Tarih
        /// </summary>
        public TNSDateTime tarih = new TNSDateTime();
        /// <summary>
        /// Ceza Ödül tipi
        /// </summary>
        public int cezaodultip = 0;
        /// <summary>
        /// Veriliş nedeni
        /// </summary>
        public string belgeNo = "";
        /// <summary>
        /// Veriliş nedeni
        /// </summary>
        public string dayanak = "";
        /// <summary>
        /// Tarih
        /// </summary>
        public TNSDateTime belgeTarihi = new TNSDateTime();
    }

    /// <summary>
    /// Personel Mal Bildirimi Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelMalBildirimi
    {
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int sirano = 0;
        /// <summary>
        /// Mal bildirim tarihi
        /// </summary>
        public TNSDateTime tarih = new TNSDateTime();
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
    }

    /// <summary>
    /// Personel Öğrenim Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelOgrenim
    {
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Sıra Numarası
        /// </summary>
        public int siraNo = 0;
        /// <summary>
        /// Öğrenim Kodu
        /// </summary>
        public int ogrenimKod = 0;
        /// <summary>
        /// Okul Adı
        /// </summary>
        public string okulAdi = "";
        /// <summary>
        /// Fakülte Bölüm Bilgisi
        /// </summary>
        public string fakulteBolum = "";
        /// <summary>
        /// Belge türü
        /// </summary>
        public int belgeTur = 0;
        /// <summary>
        /// Belge tarihi
        /// </summary>
        public TNSDateTime belgeTarihi = new TNSDateTime();
        /// <summary>
        /// Belge no
        /// </summary>
        public string belgeNo = "";
        /// <summary>
        /// Başlangıç tarihi
        /// </summary>
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        /// <summary>
        /// Bitiş tarihi
        /// </summary>
        public TNSDateTime bitirmeTarihi = new TNSDateTime();
        /// <summary>
        /// Derece
        /// </summary>
        public int derece = 0;
        /// <summary>
        /// Geçerlilik durumu-Sistem tarafından raporlama için kullanılıyor
        /// </summary>
        public int gecerli = 0;
    }

    /// <summary>
    /// Personel Dil bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelDil
    {
        public string tcKimlikNo = "";
        public int siraNo = 0;
        public int dilKod = 0;
        public string dilAd = "";
        public string duzey = "";
        public double puan = 0;
        public int donemAy = 0;
        public int donemYil = 0;
        public int belgeTur = 0;
        public TNSDateTime belgeTarihi = new TNSDateTime();
        public string belgeNo = "";
        public int bordroyaEsas = 0;
    }

    /// <summary>
    /// Personel Memuriyet Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelMemuriyet
    {
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public int siraNo = 0;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebeKodu = 0;
        /// <summary>
        /// Muhasebe Birim kodu
        /// </summary>
        public string birimKodu = "";
        /// <summary>
        /// Organizasyon birim kodu
        /// </summary>
        public int organizasyonKodu = 0;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurum = "";
        /// <summary>
        /// Birim Adı
        /// </summary>
        public string birimAd = "";
        /// <summary>
        /// Görevi
        /// </summary>
        public string gorev = "";
        /// <summary>
        /// İstihdam şekli
        /// </summary>
        public int istihdamSekli = 0;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Başlama tarihi
        /// </summary>
        public TNSDateTime baslamaTarihi = new TNSDateTime();
        /// <summary>
        /// ayrilma tarihi
        /// </summary>
        public TNSDateTime ayrilmaTarihi = new TNSDateTime();
        /// <summary>
        /// Başlama nedeni
        /// </summary>
        public int baslamaNedeni = 0;
        /// <summary>
        /// ayrilma nedeni
        /// </summary>
        public int ayrilmaNedeni = 0;
        /// <summary>
        /// Başlama nedeni AD
        /// </summary>
        public string baslamaNedeniAd = "";
        /// <summary>
        /// ayrilma nedeni ad
        /// </summary>
        public string ayrilmaNedeniAd = "";
        /// <summary>
        /// Evrak numarası
        /// </summary>
        public string evrakNo = "";
        /// <summary>
        /// Yasal dayanak
        /// </summary>
        public string yasalDayanak = "";
        /// <summary>
        /// Onay tarihi
        /// </summary>
        public TNSDateTime onayTarih = new TNSDateTime();
        /// <summary>
        /// Kadro derece
        /// </summary>
        public int kadroDerece = 0;
        /// <summary>
        /// Kadro tarihi
        /// </summary>
        public TNSDateTime kadroTarih = new TNSDateTime();
        /// <summary>
        /// Ödemeye esas derece
        /// </summary>
        public int odemeyeEsasDerece = 0;
        /// <summary>
        /// Ödemeye esas kademe
        /// </summary>
        public int odemeyeEsasKademe = 0;
        /// <summary>
        /// Ödemeye esas tarih
        /// </summary>
        public TNSDateTime odemeyeEsasTarih = new TNSDateTime();
        /// <summary>
        /// Ödemeye esas gösterge
        /// </summary>
        public int odemeyeEsasGosterge = 0;
        /// <summary>
        /// Ödemeye esas ek gösterge
        /// </summary>
        public int odemeyeEsasEkGosterge = 0;
        /// <summary>
        /// Kazanılmış hak derece
        /// </summary>
        public int kazanilmisHakDerece = 0;
        /// <summary>
        /// Kazanılmış hak kademe
        /// </summary>
        public int kazanilmisHakKademe = 0;
        /// <summary>
        /// Kazanılmış hak tarih
        /// </summary>
        public TNSDateTime kazanilmisHakTarih = new TNSDateTime();
        /// <summary>
        /// Kazanılmış gösterge
        /// </summary>
        public int kazanilmisGosterge = 0;
        /// <summary>
        /// Kazanılmış ek gösterge
        /// </summary>
        public int kazanilmisEkGosterge = 0;
        /// <summary>
        /// Emekliliğe esas derece
        /// </summary>
        public int emekliyeEsasDerece = 0;
        /// <summary>
        /// Emekliliğe esas kademe
        /// </summary>
        public int emekliyeEsasKademe = 0;
        /// <summary>
        /// Emekliliğe esas tarih
        /// </summary>
        public TNSDateTime emekliyeEsasTarih = new TNSDateTime();
        /// <summary>
        /// Emekliliğe esas gösterge
        /// </summary>
        public int emekliyeEsasGosterge = 0;
        /// <summary>
        /// Emekliliğe esas ek gösterge
        /// </summary>
        public int emekliyeEsasEkGosterge = 0;
        /// <summary>
        /// Malul
        /// </summary>
        public int malul = 0;
        /// <summary>
        /// Daire Kodu
        /// </summary>
        public int daireKodu = 0;
        public string daireAdi = "";

    }

    /// <summary>
    /// Personel Kadro Sınıfı
    /// </summary>
    [Serializable()]
    public class PersonelKadro
    {
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public string tcKimlikNo = "";
        /// <summary>
        /// Personelin T.C. Kimlik numarası
        /// </summary>
        public int siraNo = 0;
        /// <summary>
        /// Muhasebe kodu
        /// </summary>
        public int muhasebeKodu = 0;
        /// <summary>
        /// Muhasebe Birim kodu
        /// </summary>
        public string birimKodu = "";
        /// <summary>
        /// Organizasyon birim kodu
        /// </summary>
        public int organizasyonKodu = 0;
        /// <summary>
        /// Kurumsal kod
        /// </summary>
        public string kurum = "";
        /// <summary>
        /// Birim Adı
        /// </summary>
        public string birimAd = "";
        /// <summary>
        /// Ünvan
        /// </summary>
        public int unvan = 0;
        /// <summary>
        /// Ünvan Ad
        /// </summary>
        public string unvanAd = "";
        /// <summary>
        /// Sınıf
        /// </summary>
        public int sinifi = 0;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string evrakNo = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public string yasalDayanak = "";
        /// <summary>
        /// Açıklama
        /// </summary>
        public TNSDateTime onayTarih = new TNSDateTime();
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama = "";
        /// <summary>
        /// Kadro derece
        /// </summary>
        public int kadroDerece = 0;
        /// <summary>
        /// Kadro tarihi
        /// </summary>
        public TNSDateTime kadroTarih = new TNSDateTime();
        /// <summary>
        /// Ödemeye esas derece
        /// </summary>
        public int odemeyeEsasDerece = 0;
        /// <summary>
        /// Ödemeye esas kademe
        /// </summary>
        public int odemeyeEsasKademe = 0;
        /// <summary>
        /// Ödemeye esas tarih
        /// </summary>
        public TNSDateTime odemeyeEsasTarih = new TNSDateTime();
        /// <summary>
        /// Ödemeye esas gösterge
        /// </summary>
        public int odemeyeEsasGosterge = 0;
        /// <summary>
        /// Ödemeye esas ek gösterge
        /// </summary>
        public int odemeyeEsasEkGosterge = 0;
        /// <summary>
        /// Kazanılmış hak derece
        /// </summary>
        public int kazanilmisHakDerece = 0;
        /// <summary>
        /// Kazanılmış hak kademe
        /// </summary>
        public int kazanilmisHakKademe = 0;
        /// <summary>
        /// Kazanılmış hak tarih
        /// </summary>
        public TNSDateTime kazanilmisHakTarih = new TNSDateTime();
        /// <summary>
        /// Kazanılmış gösterge
        /// </summary>
        public int kazanilmisGosterge = 0;
        /// <summary>
        /// Kazanılmış ek gösterge
        /// </summary>
        public int kazanilmisEkGosterge = 0;
        /// <summary>
        /// Emekliliğe esas derece
        /// </summary>
        public int emekliyeEsasDerece = 0;
        /// <summary>
        /// Emekliliğe esas kademe
        /// </summary>
        public int emekliyeEsasKademe = 0;
        /// <summary>
        /// Emekliliğe esas tarih
        /// </summary>
        public TNSDateTime emekliyeEsasTarih = new TNSDateTime();
        /// <summary>
        /// Emekliliğe esas gösterge
        /// </summary>
        public int emekliyeEsasGosterge = 0;
        /// <summary>
        /// Emekliliğe esas ek gösterge
        /// </summary>
        public int emekliyeEsasEkGosterge = 0;
    }

    /// <summary>
    /// Ünvan bilgisi.
    /// </summary>
    [Serializable()]
    public class Unvan
    {
        public int kod = 0;
        public string ad = "";
        public string adKisa = "";
        public int siraNo = 0;
        public int yonetici = 0;
    }

    /// <summary>
    /// Sınıf bilgisi.
    /// </summary>
    [Serializable()]
    public class Sinif
    {
        public int kod = 0;
        public string ad = "";
    }

    /// <summary>
    /// Başlama ve Ayrılma nedenleri.
    /// </summary>
    [Serializable()]
    public class Neden
    {
        public int kod = 0;
        public string ad = "";
        public int baslama = 0;
        public int ayrilma = 0;
    }

    /// <summary>
    /// UstBirim bilgisi.
    /// </summary>
    [Serializable()]
    public class UstBirim
    {
        public int kod = 0;
        public string ad = "";
    }

    /// <summary>
    /// Banka bilgisi.
    /// </summary>
    [Serializable()]
    public class Banka
    {
        public string kod = "";
        public string ad = "";
    }

    /// <summary>
    /// Banka Şube bilgisi.
    /// </summary>
    [Serializable()]
    public class BankaSube
    {
        public string kod = "";
        public string bankaKodu = "";
        public string ad = "";
        public string bankaAd = "";
    }

    /// <summary>
    /// Dil bilgisi.
    /// </summary>
    [Serializable()]
    public class Dil
    {
        public int kod = 0;
        public string ad = "";
    }

    /// <summary>
    /// Kurs bilgisi.
    /// </summary>
    [Serializable()]
    public class Kurs
    {
        public int kod = 0;
        public string ad = "";
    }


    /// <summary>
    /// Öğrenim bilgisi.
    /// </summary>
    [Serializable()]
    public class Ogrenim
    {
        public int kod = 0;
        public string ad = "";
        public int siraNo = 0;
        public int tip = 0;
    }

    /// <summary>
    /// İzin Tür Bilgisi.
    /// </summary>
    [Serializable()]
    public class IzinTur
    {
        public int kod = 0;
        public string ad = "";
        public int sure = 0;
        public int gorulebilirlik = 0;
        public int sira = 0;
        public int ataKod = 0;
        public Boolean sadeceAtaKodlar = false;

        public IzinTur(int kod, string ad, int sure, int gorulebilirlik, int sira,int ataKod)
        {
            this.kod = kod;
            this.ad = ad;
            this.sure = sure;
            this.gorulebilirlik = gorulebilirlik;
            this.sira = sira;
            this.ataKod = ataKod;
        }
        public IzinTur()
        {
        }
    }

    /// <summary>
    /// İzin Plan Dönem Bilgisi.
    /// </summary>
    [Serializable()]
    public class IzinDonem
    {
        public int kod = 0;
        public int yil = 0;
        public TNSDateTime donemBaslangicTarihi = new TNSDateTime();
        public TNSDateTime donemBitisTarihi = new TNSDateTime();
        public string donemAciklama = "";

        public IzinDonem(int kod, int yil, TNSDateTime donemBaslangicTarihi, TNSDateTime donemBitisTarihi, string donemAciklama)
        {
            this.kod = kod;
            this.yil = yil;
            this.donemBaslangicTarihi = donemBaslangicTarihi;
            this.donemBitisTarihi = donemBitisTarihi;
            this.donemAciklama = donemAciklama;
        }
        public IzinDonem()
        {
        }
    }

    /// <summary>
    /// Personel İzin Plan Bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelIzinTalep
    {
        public int donemId = 0;
        public int yil = 0;
        public TNSDateTime donemBaslangicTarihi = new TNSDateTime();
        public TNSDateTime donemBitisTarihi = new TNSDateTime();
        public string donemAciklama = "";
        public string tcKimlikNo = "";
        public TNSDateTime talepBaslangicTarihi = new TNSDateTime();
        public TNSDateTime talepBitisTarihi = new TNSDateTime();
        public int talepGun = 0;
        public int durum = 0;

        public PersonelIzinTalep(int donemId, int yil, TNSDateTime donemBaslangicTarihi, TNSDateTime donemBitisTarihi, string donemAciklama, string tcKimlikNo, TNSDateTime talepBaslangicTarihi, TNSDateTime talepBitisTarihi, int talepGun, int durum)
        {
            this.donemId = donemId;
            this.yil = yil;
            this.donemBaslangicTarihi = donemBaslangicTarihi;
            this.donemBitisTarihi = donemBitisTarihi;
            this.donemAciklama = donemAciklama;
            this.tcKimlikNo = tcKimlikNo;
            this.talepBaslangicTarihi = talepBaslangicTarihi;
            this.talepBitisTarihi = talepBitisTarihi;
            this.talepGun = talepGun;
            this.durum = durum;
        }
        public PersonelIzinTalep()
        {
        }
    }

    /// <summary>
    /// kadro bilgisi.
    /// </summary>
    [Serializable()]
    public class Kadro
    {
        public int teskilat = 0;
        public int ustBirim = 0;
        public int sinif = 0;
        public int unvan = 0;
        public int kadro = 0;
        public int sayIl = 0;
        public int sayTip = 0;
        public int sayIlce = 0;
        public int altIl = 0;
        public int altTip = 0;
        public int altIlce = 0;
        public int dolu = 0;
        public int bos = 0;
        public int adet = 0;
        public int oncesiDolu = 0;
        public int oncesiBos = 0;
        public int oncesiAdet = 0;
        public TNSDateTime islemTarih = new TNSDateTime();
        public TNSDateTime vizeTarih = new TNSDateTime();
        public int yil = 0;
        public int teklifNo = 0;
        public int icmalNo = 0;
        public int siraNo = 0;
        public int tahsisTenkis = 0;
        public string ustBirimAd = "";
        public string sinifAd = "";
        public string unvanAd = "";
        public string muhasebeAd = "";
        public string teskilatAd = "";
    }

    [Serializable()]
    public class SozlesmeliBordroTanim
    {
        public string tcKimlikNo = "";
        public int yil = 0;
        public int ay = 0;
        public decimal kumulatifVergiMatrahi = 0;
    }

    [Serializable()]
    public class OgrenimBazliPersonelSayi
    {
        public Ogrenim ogrenim;
        public int personelSayi;
    }

    [Serializable()]
    public class UnvanBazliPersonelSayi
    {
        public Unvan unvan;
        public int personelSayi;
    }

    [Serializable()]
    public class SinifBazliPersonelSayi
    {
        public Sinif sinif;
        public int personelSayi;
    }

    [Serializable()]
    public class UnvanVeKadroDereceBazliPersonelSayi
    {
        public Unvan unvan;
        public int kadroDerece;
        public int personelSayi;
        public int kontenjan;
    }

    [Serializable()]
    public class KadroKontenjan
    {
        public int orgBirimKod;
        public string orgBirimAd;
        public MuhasebeBirim mb;
        public int siraNo;
        public EnumKadroKontenjanIslemTur islemTur;
        public TNSDateTime islemTarih;
        public string aciklama;
        public KadroKontenjanDetay[] detaylar;
        public TNSDateTime dayanakBelgeTarih;
        public string dayanakBelgeNo;
    }

    [Serializable()]
    public class KadroKontenjanDetay
    {
        public int orgBirimKod;
        public string orgBirimAd;
        public MuhasebeBirim mb;
        public int siraNo;
        public int unvanKod;
        public string unvanAd;
        public int kadroDerece;
        public int kontenjan;
    }

    [Serializable()]
    public class PersonelIzin
    {
        public string tcKimlikNo = "";
        public int sirano = 0;
        public int tip = 0;
        public string aciklama = "";
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        public TNSDateTime bitisTarihi = new TNSDateTime();
        public string telefon = "";
        public int kullanilanGun = 0;
        public string izinNotu = "";
        public int onay = 0;
        public string evrEvrakNo = string.Empty;
        public string prEvrakNo = string.Empty;
        public string alan1 = string.Empty;
        public string not = string.Empty;
        public string donem = string.Empty;
        public TNSDateTime raporTarihi = new TNSDateTime();
        public string raporVeren = string.Empty;
        public string raporNumara = string.Empty;
        //public int raporTuru = 0;
    }

    [Serializable()]
    public class PersonelIzinDevir
    {
        public string tcKimlikNo = "";
        public int yil = 0;
        public int devreden = 0;
        public int yilici = 0;
        public string ad = "";
        public string soyad = "";
        public string sicilNo = "";
    }

    [Serializable()]
    public class PersonelTarihce
    {
        public string personel;
        public string kullanici;
        public TNSDateTime tarih;
        public string degisiklik;
        public string kullaniciAd;

        //public Object yeni;
        //public Object eski;
        //public ENUMTARIHCE alan;
    }

    /// <summary>
    /// Görevlendirme Konu bilgisi.
    /// </summary>
    [Serializable()]
    public class GorevKonu
    {
        public string kod = "";
        public string ad = "";
    }

    /// <summary>
    /// Birime ait daire bilgileri (durum değeri 1 ise aktif)
    /// </summary>
    [Serializable()]
    public class BirimDaireBilgileri
    {
        public int kod = 0;
        public int anakod = 0;
        public string birimkodu = "";
        public string adi = "";
        public int durum = 0;
    }
    [Serializable()]
    public class PersonelDaireBilgileriRapor
    {
        public string tcKimlikNo = "";
        public string ad = "";
        public string soyad = "";
        public string unvan = "";
        public string daireAdi = "";
        public int daireodu = 0;
        public TNSDateTime iseBaslamaTarihi = new TNSDateTime();
        public TNSDateTime istenCikisTarihi = new TNSDateTime();
    }
}