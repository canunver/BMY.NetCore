using System;
using System.Collections;
using System.Text;
using TNS.BKD;
using HRCServisI;
using TNS.KYM;
using System.ComponentModel;
using System.Collections.Generic;

namespace TNS.HRC
{
    /// <summary>
    /// Is bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class Is
    {
        /// <summary>
        /// İş kodu
        /// </summary>
        public int kod;
        /// <summary>
        /// İşin adı
        /// </summary>
        public string ad;
        /// <summary>
        /// İş için kullanılabilecek ekonomik kod(lar)
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// İş için kullanılabilecek ödeme hesap kod(lar)
        /// </summary>
        public string odemeHesapKod;
        /// <summary>
        /// İş için kullanılabilecek yansıtma kod(lar)
        /// </summary>
        public string yansitmaKod;
        /// <summary>
        /// Damga vergisi
        /// </summary>
        public int damgaVergisi;
        /// <summary>
        /// İşin türü
        /// </summary>
        public int tur;
        /// <summary>
        /// İşin durumu
        /// </summary>
        public int durum;

        /// <summary>
        /// <see cref="Is"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Is()
        {
            kod = 0;
            ad = string.Empty;
            ekoKod = string.Empty;
            odemeHesapKod = string.Empty;
            yansitmaKod = string.Empty;
            damgaVergisi = 0;
            tur = 0;
            durum = 0;
        }
    }

    [Serializable()]
    public class HizmetAlimi
    {
        private int surecYil;

        public int SurecYil
        {
            get { return surecYil; }
            set { surecYil = value; }
        }
        private string surecNo;

        public string SurecNo
        {
            get { return surecNo; }
            set { surecNo = value; }
        }
        private string konu;

        public string Konu
        {
            get { return konu; }
            set { konu = value; }
        }
        private int tur;

        public int Tur
        {
            get { return tur; }
            set { tur = value; }
        }
        private string formKod;

        public string FormKod
        {
            get { return formKod; }
            set { formKod = value; }
        }
        private int siraNo;

        public int SiraNo
        {
            get { return siraNo; }
            set { siraNo = value; }
        }


        public HizmetAlimi()
        {
            surecYil = 0;
            surecNo = string.Empty;
            konu = string.Empty;
            tur = 0;
            formKod = string.Empty;
            siraNo = 0;
        }
    }
    /// <summary>
    /// Parca bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class Parca
    {
        /// <summary>
        /// Parça kod
        /// </summary>
        public int kod;
        public int Kod { get { return kod; } }
        /// <summary>
        /// Parça adı
        /// </summary>
        public string ad;
        public string Ad { get { return ad; } }
        /// <summary>
        /// Parçanın bulunduğu sayfa
        /// </summary>
        public string sayfa;
        public string Sayfa { get { return sayfa; } }
        /// <summary>
        /// Parçanın düzeyi
        /// </summary>
        public int duzey;
        public int Duzey { get { return duzey; } }
        /// <summary>
        /// Parçanın zorunluluk değeri
        /// </summary>
        public int zorunlu;
        public int Zorunlu { get { return zorunlu; } }
        /// <summary>
        /// Parçanın sayfa yüksekliği
        /// </summary>
        public int yukseklik;
        public int Yukseklik { get { return yukseklik; } }
        /// <summary>
        /// Parçanın sayfa genişliği
        /// </summary>
        public int genislik;
        public int Genislik { get { return genislik; } }

        /// <summary>
        /// <see cref="Parca"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Parca()
        {
            kod = 0;
            ad = string.Empty;
            sayfa = string.Empty;
            duzey = 0;
            zorunlu = 0;
            yukseklik = 0;
            genislik = 0;
        }
    }

    /// <summary>
    /// <see cref="Is"/> ve <see cref="Parca"/> bilgilerinin ilişkisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IsParca
    {
        /// <summary>
        /// İlişkili <see cref="Is"/> nesnesi
        /// </summary>
        public Is isAd;
        /// <summary>
        /// İlişkili <see cref="Parca"/> nesnelerini içeren liste
        /// </summary>
        public ObjectArray parcalar;

        /// <summary>
        /// <see cref="IsParca"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IsParca()
        {
            isAd = new Is();
            parcalar = new ObjectArray();
        }
    }

    /// <summary>
    /// İhale ve <see cref="Parca"/> bilgilerini ilişkilendiren nesne
    /// </summary>
    [Serializable()]
    public class IhaleParca
    {
        /// <summary>
        /// İhale Usulü
        /// </summary>
        public int ihaleUsulu;
        /// <summary>
        /// İhale Maddesi
        /// </summary>
        public int ihaleMaddesi;
        /// <summary>
        /// İhale komisyonu var mı
        /// </summary>
        public int komisyon;
        /// <summary>
        /// İşin türü
        /// </summary>
        public int tur;
        /// <summary>
        /// İhaleye ait <see cref="Parca"/> listesini tutan nesne
        /// </summary>
        public ObjectArray parcalar;

        /// <summary>
        /// <see cref="IhaleParca"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleParca()
        {
            ihaleUsulu = 0;
            ihaleMaddesi = 0;
            komisyon = 0;
            tur = 0;
            parcalar = new ObjectArray();
        }
    }

    /// <summary>
    /// İhaleye ait <see cref="Parca"/>lara ait bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleParcaBag
    {
        /// <summary>
        /// İhale Usulü
        /// </summary>
        public int ihaleUsulu;
        /// <summary>
        /// İhale Maddesi
        /// </summary>
        public int ihaleMaddesi;
        /// <summary>
        /// İhale Komisyonu Var Mı
        /// </summary>
        public int komisyon;
        /// <summary>
        /// İhale Türü
        /// </summary>
        public int tur;
        /// <summary>
        /// İhale Parça Kodu
        /// </summary>
        public int kod;
        /// <summary>
        /// İhale Parça Sıra No
        /// </summary>
        public int sira;
        /// <summary>
        /// İhale Parça Düzey
        /// </summary>
        public int duzey;
        /// <summary>
        /// İhale Parça Zorunlu Mu
        /// </summary>
        public int zorunlu;

        /// <summary>
        /// <see cref="IhaleParcaBag"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleParcaBag()
        {
            ihaleUsulu = 0;
            ihaleMaddesi = 0;
            komisyon = 0;
            tur = 0;
            kod = 0;
            sira = 0;
            duzey = 0;
            zorunlu = 0;
        }
    }

    /// <summary>
    /// İhale Sözleşme Tanım bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleSozlesmeTanim
    {
        /// <summary>
        /// İhale Usulü
        /// </summary>
        public int ihaleUsulu;
        /// <summary>
        /// İhale Maddesi
        /// </summary>
        public int ihaleMaddesi;
        /// <summary>
        /// İhale Türü
        /// </summary>
        public int tur;
        /// <summary>
        /// İş Türü
        /// </summary>
        public int isTur;
        /// <summary>
        /// İhale Sözleşme Metni
        /// </summary>
        public StringBuilder metin;

        /// <summary>
        /// <see cref="IhaleSozlesmeTanim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleSozlesmeTanim()
        {
            ihaleUsulu = 0;
            ihaleMaddesi = 0;
            tur = 0;
            isTur = 0;
            metin = new StringBuilder();
        }
    }

    /// <summary>
    /// Sürece ait İhale Sözleşme bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleSozlesme
    {
        /// <summary>
        /// Belge Yılı
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Belge Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Belge Tür
        /// </summary>
        public int tur;
        /// <summary>
        /// Belge Metni Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Belge Metni
        /// </summary>
        public StringBuilder metin;

        /// <summary>
        /// <see cref="IhaleSozlesme"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleSozlesme()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tur = 0;
            siraNo = 0;
            metin = new StringBuilder();
        }
    }


    /// <summary>
    /// İhaleye ait Şartname bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleBelge
    {
        /// <summary>
        /// Belge Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Belge Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Belge Türü
        /// </summary>
        public int belgeTur;
        /// <summary>
        /// Belge Tiği
        /// </summary>
        public int belgeTip;
        /// <summary>
        /// Belge detaylarını <see cref="IhaleBelgeDetay"/> nesne koleksiyonu şeklinde tutan nesne
        /// </summary>
        public TNSCollection detay;

        /// <summary>
        /// <see cref="IhaleBelge"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleBelge()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            belgeTur = 0;
            belgeTip = 0;
            detay = new TNSCollection();
        }
    }


    /// <summary>
    /// İhale Şartname Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleBelgeDetay
    {
        /// <summary>
        /// Detay Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Detay Alan
        /// </summary>
        public string alan;
        /// <summary>
        /// Detay Değeri
        /// </summary>
        public string deger;
        /// <summary>
        /// Detay Kontrol ID
        /// </summary>
        public string kontrolID;
        /// <summary>
        /// Detay Kontrol Tür
        /// </summary>
        public int kontrolTur;

        /// <summary>
        /// <see cref="IhaleBelgeDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleBelgeDetay()
        {
            siraNo = 0;
            alan = string.Empty;
            deger = string.Empty;
            kontrolID = string.Empty;
            kontrolTur = 0;
        }
    }

    /// <summary>
    /// Hakediş bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Hakedis
    {
        /// <summary>
        /// Hakediş Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hakediş Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Hakediş Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Hakediş Süreç No
        /// </summary>
        public string surecNo;

        /// <summary>
        /// Hakediş Sözleşme Bedeli
        /// </summary>
        public double sozlesmeBedeli;

        /// <summary>
        /// Sözleşme Bedelinin KDV Bedeli
        /// </summary>
        public double sozlesmeKDVBedeli;

        /// <summary>
        /// İhale Tarihi
        /// </summary>
        public TNSDateTime ihaleTarihi;
        /// <summary>
        /// İhale Karar Tarihi
        /// </summary>
        public TNSDateTime ihaleKararTarihi;
        /// <summary>
        /// İhale Karar No
        /// </summary>
        public string ihaleKararNo;
        /// <summary>
        /// Sözleşme Tarihi
        /// </summary>
        public TNSDateTime sozlesmeTarihi;
        /// <summary>
        /// Sözleşme No
        /// </summary>
        public string sozlesmeNo;
        /// <summary>
        /// Teslim Tarihi
        /// </summary>
        public TNSDateTime teslimTarihi;
        /// <summary>
        /// İşin Süresi
        /// </summary>
        public string isinSuresi;
        /// <summary>
        /// Bitim Tarihi
        /// </summary>
        public TNSDateTime bitimTarihi;
        /// <summary>
        /// <see cref="HakedisOdemePlan"/> bilgilerini tutan nesne
        /// </summary>
        public ObjectArray odemePlan;
        /// <summary>
        /// <see cref="HakedisEkSozlesme"/> bilgilerini tutan nesne
        /// </summary>
        public ObjectArray ekSozlesme;
        /// <summary>
        /// <see cref="HakedisSureUzatim"/> bilgilerini tutan nesne
        /// </summary>
        public ObjectArray sureUzatim;
        /// <summary>
        /// Hesap Planı Mı
        /// </summary>
        public bool hesapPlaniMi;

        /// <summary>
        /// <see cref="Hakedis"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Hakedis()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            sozlesmeBedeli = 0;
            sozlesmeKDVBedeli = 0;
            ihaleTarihi = new TNSDateTime();
            ihaleKararTarihi = new TNSDateTime();
            ihaleKararNo = string.Empty;
            sozlesmeTarihi = new TNSDateTime();
            sozlesmeNo = string.Empty;
            teslimTarihi = new TNSDateTime();
            isinSuresi = string.Empty;
            bitimTarihi = new TNSDateTime();
            odemePlan = new ObjectArray();
            ekSozlesme = new ObjectArray();
            sureUzatim = new ObjectArray();
            hesapPlaniMi = false;
        }

        public Hakedis(int yil, int muhasebe, string birim, string surecNo)
        {
            this.yil = yil;
            this.muhasebe = muhasebe;
            this.birim = birim;
            this.surecNo = surecNo;
            sozlesmeBedeli = 0;
            sozlesmeKDVBedeli = 0;
            ihaleTarihi = new TNSDateTime();
            ihaleKararTarihi = new TNSDateTime();
            ihaleKararNo = string.Empty;
            sozlesmeTarihi = new TNSDateTime();
            sozlesmeNo = string.Empty;
            teslimTarihi = new TNSDateTime();
            isinSuresi = string.Empty;
            bitimTarihi = new TNSDateTime();
            odemePlan = new ObjectArray();
            ekSozlesme = new ObjectArray();
            sureUzatim = new ObjectArray();
            hesapPlaniMi = false;
        }

        public double sozlesmeArtisBedeli
        {
            get
            {
                double toplam = 0;
                if (ekSozlesme != null)
                {
                    foreach (HakedisEkSozlesme item in ekSozlesme.objeler)
                    {
                        toplam += item.sozlesmeBedeli;
                    }
                }
                return toplam;
            }
        }
    }

    /// <summary>
    /// Hakediş Ödeme Planı bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class HakedisOdemePlan
    {
        /// <summary>
        /// Hakediş Ödeme Planı Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hakediş Ödeme Planı Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Hakediş Ödeme Planı Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Hakediş Ödeme Planı Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Hakediş Ödeme Planı Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Hakediş Ödeme Planı Ödeme Tutarı
        /// </summary>
        public double odemeTutari;
        /// <summary>
        /// Hakediş Ödeme Planı Ödeme Tarihi
        /// </summary>
        public TNSDateTime odemeTarihi;
        /// <summary>
        /// Hakediş Ödeme Planı Fiyat Farkı Oranı
        /// </summary>
        public double fiyatFarkiOrani;
        /// <summary>
        /// Hakediş Ödeme Planı İşçilik Fiyat Farkı
        /// </summary>
        public double iscilikFiyatFarki;
        /// <summary>
        /// Hakediş Ödeme Planı Avans Miktarı
        /// </summary>
        public double avansMiktari;
        /// <summary>
        /// Hakediş Ödeme Planı Programatik Fatura No
        /// </summary>
        public int prFaturaNo;
        /// <summary>
        /// Hakediş Ödeme Planı Fatura No
        /// </summary>
        public string faturaNo;
        /// <summary>
        /// Hakediş Ödeme Planı Ödeme Emri No
        /// </summary>
        public string odemeEmriNo;//Kayıt edilmiyor bilgi taşıyor


        /// <summary>
        /// <see cref="HakedisOdemePlan"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HakedisOdemePlan()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            siraNo = 0;
            odemeTutari = 0;
            odemeTarihi = new TNSDateTime();
            fiyatFarkiOrani = 0;
            iscilikFiyatFarki = 0;
            avansMiktari = 0;
            prFaturaNo = 0;
            faturaNo = string.Empty;
            odemeEmriNo = string.Empty;
        }
    }

    /// <summary>
    /// Hakediş Ek Sözleşme bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class HakedisEkSozlesme
    {
        /// <summary>
        /// Hakediş Ek Sözleşme Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hakediş Ek Sözleşme Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Hakediş Ek Sözleşme Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Hakediş Ek Sözleşme Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Hakediş Ek Sözleşme Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Hakediş Ek Sözleşme Onay Tarih
        /// </summary>
        public TNSDateTime onayTarihi;
        /// <summary>
        /// Hakediş Ek Sözleşme Onay No
        /// </summary>
        public string onayNo;
        /// <summary>
        /// Hakediş Ek Sözleşme Sözleşme Bedel
        /// </summary>
        public double sozlesmeBedeli;

        /// <summary>
        /// <see cref="HakedisEkSozlesme"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HakedisEkSozlesme()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            siraNo = 0;
            onayTarihi = new TNSDateTime();
            onayNo = string.Empty;
            sozlesmeBedeli = 0;
        }
    }

    /// <summary>
    /// Hakediş Süre Uzatım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class HakedisSureUzatim
    {
        /// <summary>
        /// Hakediş Süre Uzatım Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hakediş Süre Uzatım Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Hakediş Süre Uzatım Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Hakediş Süre Uzatım Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Hakediş Süre Uzatım Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Hakediş Süre Uzatım Onay Tarih
        /// </summary>
        public TNSDateTime onayTarihi;
        /// <summary>
        /// Hakediş Süre Uzatım Onay No
        /// </summary>
        public string onayNo;
        /// <summary>
        /// Hakediş Süre Uzatım Süre
        /// </summary>
        public int sure;
        /// <summary>
        /// Hakediş Süre Uzatım Bitim Tarih
        /// </summary>
        public TNSDateTime bitimTarihi;

        /// <summary>
        /// <see cref="HakedisSureUzatim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HakedisSureUzatim()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            siraNo = 0;
            onayTarihi = new TNSDateTime();
            sure = 0;
            bitimTarihi = new TNSDateTime();
        }
    }

    /// <summary>
    /// Muhasebe Birim bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class MuhasebeBirim
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim Ad
        /// </summary>
        public string birimAd;

        /// <summary>
        /// <see cref="MuhasebeBirim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MuhasebeBirim()
        {
            muhasebeKod = 0;
            birimKod = string.Empty;
            muhasebeAd = string.Empty;
            birimAd = string.Empty;
        }

        public string kurKod { get; set; }
        public int plakaKod { get; set; }
        public string kyKod { get; set; }
        public string kyAd { get; set; }

        public string MuhasebeBirimKurum { get { return muhasebeKod + "." + birimKod + "." + kurKod; } }
        public string BirimKurum { get { return birimKod + "." + kurKod; } }

        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string MuhasebeBirimAd { get { return TNS.MUH.Arac.BirimAdOl(muhasebeAd, birimAd); } }

        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string BirimKodAd { get { return BirimKurum + "-" + MuhasebeBirimAd; } }

        /// <summary>
        /// Harcama Birim adı
        /// </summary>
        public string MuhasebeBirimKodAd { get { return MuhasebeBirimKurum + "-" + MuhasebeBirimAd; } }
    }

    /// <summary>
    /// Muhasebe Birim Tertip bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class MuhasebeBirimTertip
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurKod;
        /// <summary>
        /// Fonksiyonel Kod
        /// </summary>
        public string fonKod;
        /// <summary>
        /// Finansman Kod
        /// </summary>
        public string finKod;
        /// <summary>
        /// Ekonomik Kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// pebEkoKod
        /// </summary>
        public string pebEkoKod;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuhasebeBirimTertip"/> class.
        /// </summary>
        public MuhasebeBirimTertip()
        {
            muhasebeKod = 0;
            birimKod = string.Empty;
            kurKod = string.Empty;
            fonKod = string.Empty;
            finKod = string.Empty;
            ekoKod = string.Empty;
            pebEkoKod = string.Empty;
        }
    }

    /// <summary>
    /// Öğrenci Eğitmen Bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class OgrenciEgitmen
    {
        /// <summary>
        /// Öğrenci Eğitmen Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Öğrenci Eğitmen Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Öğrenci Eğitmen Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Öğrenci Eğitmen Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Öğrenci Eğitmen TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Öğrenci Eğitmen Ad Soyad
        /// </summary>
        public string adiSoyadi;
        /// <summary>
        /// Öğrenci Eğitmen Görev
        /// </summary>
        public string gorev;
        /// <summary>
        /// Öğrenci Eğitmen Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Öğrenci Eğitmen Durum
        /// </summary>
        public int durum;
        /// <summary>
        /// Öğrenci Eğitmen Banka Kod
        /// </summary>
        public string bankaKodu;
        /// <summary>
        /// Öğrenci Eğitmen Banka Şube Kod
        /// </summary>
        public string bankaSubeKodu;
        /// <summary>
        /// Öğrenci Eğitmen Banka Hesap No
        /// </summary>
        public string bankaHesapNo;
        /// <summary>
        /// Öğrenci Eğitmen Banka Ad
        /// </summary>
        public string bankaAdi;
        /// <summary>
        /// Öğrenci Eğitmen Banka Şube Ad
        /// </summary>
        public string bankaSubeAdi;

        /// <summary>
        /// <see cref="OgrenciEgitmen"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OgrenciEgitmen()
        {
            muhasebe = 0;
            birim = string.Empty;
            muhasebeAd = string.Empty;
            birimAd = string.Empty;
            tcKimlikNo = string.Empty;
            adiSoyadi = string.Empty;
            gorev = string.Empty;
            tip = 0;
            durum = 0;
            bankaKodu = string.Empty;
            bankaSubeKodu = string.Empty;
            bankaHesapNo = string.Empty;
            bankaAdi = string.Empty;
            bankaSubeAdi = string.Empty;
        }
    }

    /// <summary>
    /// Abone Fatura bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneFatura
    {
        /// <summary>
        /// Abone Fatura Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Abone Fatura Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Abone Fatura Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Abone Fatura Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Abone Fatura Fatura Yıl
        /// </summary>
        public int faturaYil;
        /// <summary>
        /// Abone Fatura Fatura Ay
        /// </summary>
        public int faturaAy;
        /// <summary>
        /// Abone Fatura Abone No
        /// </summary>
        public string aboneNo;
        /// <summary>
        /// Abone Fatura İşletme Kod
        /// </summary>
        public string isletmeKodu;
        /// <summary>
        /// Abone Fatura Abone Tip
        /// </summary>
        public int aboneTip;
        /// <summary>
        /// Abone Fatura Fatura Tutar
        /// </summary>
        public double faturaTutar;
        /// <summary>
        /// Abone Fatura Faiz Miktar
        /// </summary>
        public double faizMiktari;
        public double damgaVergisi;
        /// <summary>
        /// Abone Fatura Özel Kullanım Miktar
        /// </summary>
        public double ozelKullanim;
        /// <summary>
        /// Abone Fatura Ödeme Tarih
        /// </summary>
        public TNSDateTime odemeTarihi;
        /// <summary>
        /// Abone Fatura Ödeme Emri Belge Yıl
        /// </summary>
        public int odemeEmriYil;
        /// <summary>
        /// Abone Fatura Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Abone Fatura Fatura No
        /// </summary>
        public string faturaNo;

        /// <summary>
        /// <see cref="AboneFatura"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneFatura()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            faturaAy = 0;
            faturaYil = 0;
            aboneNo = string.Empty;
            isletmeKodu = string.Empty;
            aboneTip = 0;
            faturaTutar = 0.0;
            faizMiktari = 0.0;
            damgaVergisi = 0.0;
            ozelKullanim = 0.0;
            odemeTarihi = new TNSDateTime();
            odemeEmriYil = 0;
            odemeEmriNo = string.Empty;
            faturaNo = string.Empty;
        }

        public TNSDateTime yevmiyeTarih { get; set; }
        public int yevmiyeNo { get; set; }
        public int mifDurum { get; set; }
    }


    /// <summary>
    /// Ödenek Gönderme Belgesinin Aboneye Mail olarak gönderilmesi için gerekli bilgiyi tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneOdenekGondermeMail
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Elektronik Posta
        /// </summary>
        public string ePosta;
        /// <summary>
        /// Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurKod;

        /// <summary>
        /// <see cref="AboneOdenekGondermeMail"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneOdenekGondermeMail()
        {
            kod = 0;
            ePosta = string.Empty;
            adSoyad = string.Empty;
            kurKod = string.Empty;
        }

        /// <summary>
        /// <see cref="AboneOdenekGondermeMail"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="kurumsalKod">Kurumsal kod.</param>
        public AboneOdenekGondermeMail(string kurumsalKod)
        {
            kod = 0;
            ePosta = string.Empty;
            adSoyad = string.Empty;
            kurKod = kurumsalKod;
        }
    }

    /// <summary>
    /// Süreç bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Surec
    {
        public string islemAdAciklama
        {
            get
            {
                string donDeger = "(" + yil + ") " + islem.ad;
                if (isinAdiAciklama != "")
                    donDeger += " (" + isinAdiAciklama + ")";
                return donDeger;
            }
        }

        public string isinAdiAciklama
        {
            get
            {
                if (string.IsNullOrEmpty(isinAdi))
                    return aciklama;
                if (string.IsNullOrEmpty(aciklama))
                    return isinAdi;
                if (string.IsNullOrEmpty(isinAdi) && string.IsNullOrEmpty(aciklama))
                    return "";

                if (isinAdi.IndexOf(aciklama) >= 0) return isinAdi;
                if (aciklama.IndexOf(isinAdi) >= 0) return aciklama;
                return isinAdi + " (" + aciklama + ")";
            }
        }
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil { get; set; }
        /// <summary>
        /// Süreç Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Süreç Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Süreç Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Süreç Tarih
        /// </summary>
        public TNSDateTime surecTarih;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo { get; set; }
        /// <summary>
        /// Sürece ait <see cref="Is"/> bilgisinin tutulduğu özellik
        /// </summary>
        public Is islem;
        /// <summary>
        /// Süreç Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Süreç Durum
        /// </summary>
        public int surecDurum;
        /// <summary>
        /// Süreç İş Ad
        /// </summary>
        public string isinAdi;
        /// <summary>
        /// Sürece ait <see cref="Parca"/> bilgisinin tutulduğu özellik
        /// </summary>
        public Parca parca;
        /// <summary>
        /// Sürece ait <see cref="SurecParcaDurum"/> bilgisinin tutulduğu özellik
        /// </summary>
        public SurecParcaDurum parcaDurum;
        /// <summary>
        /// Sürece ait Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Süreç için takip eden yıllarda işlem yapılacak mı
        /// </summary>
        public int yillaraSariIslemMi;
        public string kullaniciAdi;
        public double onayTutari = 0.0;
        public double yaklasikMaliyetTutari = 0.0;
        public double piyasaFiyatTutari = 0.0;
        public double sozlesmeTutari = 0.0;

        public string SurecDurumAd
        {
            get
            {
                if (surecDurum < 5) return "Devam";
                else if (surecDurum == 5) return "Arşiv";
                else return "İptal";
            }
        }

        /// <summary>
        /// <see cref="Surec"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Surec()
        {
            yil = 0;
            muhasebe = 0;
            muhasebeAd = string.Empty;
            birim = string.Empty;
            birimAd = string.Empty;
            surecTarih = new TNSDateTime();
            surecNo = string.Empty;
            islem = new Is();
            aciklama = string.Empty;
            surecDurum = 0;
            isinAdi = string.Empty;
            parca = new Parca();
            parcaDurum = new SurecParcaDurum();
            odemeEmriNo = string.Empty;
            yillaraSariIslemMi = 0;
            kullaniciAdi = string.Empty;
        }

        public Surec(int yil, string surecNo)
        {
            this.yil = yil;
            this.surecNo = surecNo;
        }

        public string MalHizmetYapimAd()
        {
            if (this.islem.tur == (int)ENUMIhaleTur.MAL)
                return "Mal";
            else if (this.islem.tur == (int)ENUMIhaleTur.HIZMET)
                return "Hizmet";
            else if (this.islem.tur == (int)ENUMIhaleTur.YAPIM)
                return "Yapım";
            return "";
        }
    }


    /// <summary>
    /// Süreç Sorgu bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class SurecSorguKriter
    {
        /// <summary>
        /// Süreç Sorgu Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç Sorgu Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Süreç Sorgu Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç Sorgu Başlangıç Tarih
        /// </summary>
        public TNSDateTime surecTarihBasla;
        /// <summary>
        /// Süreç Sorgu Bitiş Tarih
        /// </summary>
        public TNSDateTime surecTarihBitis;
        /// <summary>
        /// Süreç Sorgu Başlangıç Süreç No
        /// </summary>
        public string surecNoBasla;
        /// <summary>
        /// Süreç Sorgu Bitiş Süreç No
        /// </summary>
        public string surecNoBitis;
        /// <summary>
        /// Süreç Sorgu <see cref="Is"/> bilgisini tutan özellik
        /// </summary>
        public Is islem;
        /// <summary>
        /// Süreç Sorgu <see cref="SurecParcaDurum"/> bilgisini tutan özellik
        /// </summary>
        public SurecParcaDurum parcaDurum;
        /// <summary>
        /// Süreç Sorgu Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Süreç Sorgu İş Ad
        /// </summary>
        public string isinAdi;
        /// <summary>
        /// Süreç Sorgu Süreç Durum
        /// </summary>
        public int surecDurum;
        /// <summary>
        /// Süreç Sorgu TC Kimlik No
        /// </summary>
        public string tckimlikno;
        /// <summary>
        /// Süreç Sorgu İhale Usul
        /// </summary>
        public int ihaleUsulu;
        /// <summary>
        /// Süreç Sorgu İhale Madde
        /// </summary>
        public int ihaleMaddesi;
        /// <summary>
        /// Süreç Sorgu Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        public string firmaVNO;
        public string firmaAdi;

        public bool alimIsleri;
        /// <summary>
        /// <see cref="SurecSorguKriter"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SurecSorguKriter()
        {
            alimIsleri = false;
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecTarihBasla = new TNSDateTime();
            surecTarihBitis = new TNSDateTime();
            surecNoBasla = string.Empty;
            surecNoBitis = string.Empty;
            islem = new Is();
            parcaDurum = new SurecParcaDurum();
            aciklama = string.Empty;
            isinAdi = string.Empty;
            surecDurum = 0;
            tckimlikno = string.Empty;
            ihaleUsulu = 0;
            ihaleMaddesi = 0;
            odemeEmriNo = string.Empty;
            muhasebeZorunlu = true;
            firmaVNO = "";
            firmaAdi = "";
        }

        public int malHizmetYapim { get; set; }

        public bool muhasebeZorunlu;
    }

    /// <summary>
    /// Süreç Parça Durum bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class SurecParcaDurum
    {
        /// <summary>
        /// Süreç Parça Durum Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç Parça Durum Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Süreç Parça Durum İş Kod
        /// </summary>
        public int islemKod;
        /// <summary>
        /// Süreç Parça Durum Parça Kod
        /// </summary>
        public int parcaKod;
        /// <summary>
        /// Süreç Parça Durum Parça Ad
        /// </summary>
        public string parcaAd;
        /// <summary>
        /// Süreç Parça Durum Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Süreç Parça Durum İşlem Yapan Kullanıcı Ad
        /// </summary>
        public string kullaniciAd;
        /// <summary>
        /// Süreç Parça Durum
        /// </summary>
        public int durum;

        /// <summary>
        /// <see cref="SurecParcaDurum"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SurecParcaDurum()
        {
            yil = 0;
            surecNo = string.Empty;
            islemKod = 0;
            parcaKod = 0;
            parcaAd = string.Empty;
            tarih = new TNSDateTime();
            kullaniciAd = string.Empty;
            durum = (int)ENUMSurecParcaDurum.TANIMSIZ;
        }
    }

    /// <summary>
    /// Teklif verilecek Malzeme bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifMalzeme
    {
        /// <summary>
        /// Teklif Malzeme Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Malzeme Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Malzeme Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Malzeme Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Malzeme No
        /// </summary>
        public int malzemeNo;
        /// <summary>
        /// Teklif Malzeme Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Teklif Malzeme Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// Teklif Malzeme Kod
        /// </summary>
        public string malzemeKod;
        /// <summary>
        /// Teklif Malzeme Miktar
        /// </summary>
        public double miktar;
        /// <summary>
        /// Teklif Malzeme Birim Fiyat
        /// </summary>
        public double birimFiyat;
        /// <summary>
        /// Teklif Malzeme <see cref="OlcuBirim"/> bilgisini tutan özellik
        /// </summary>
        public OlcuBirim olcuBirim;
        /// <summary>
        /// Teklif Malzeme Silinecek Mi
        /// </summary>
        public bool sil;

        /// <summary>
        /// <see cref="TeklifMalzeme"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifMalzeme()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            malzemeNo = 0;
            tip = 0;
            adi = string.Empty;
            malzemeKod = string.Empty;
            miktar = 0.0;
            birimFiyat = 0.0;
            olcuBirim = new OlcuBirim();
            sil = false;
        }
    }

    /// <summary>
    /// Ölçü Birim bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class OlcuBirim
    {
        /// <summary>
        /// Ölçü Birim Kod
        /// </summary>
        public int _kod;
        public int kod { get { return _kod; } set { _kod = value; } }



        /// <summary>
        /// Ölçü Birim Ad
        /// </summary>
        public string _ad;
        public string ad { get { return _ad; } set { _ad = value; } }

        /// <summary>
        /// <see cref="OlcuBirim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OlcuBirim()
        {
            kod = 0;
            ad = string.Empty;
        }
    }

    /// <summary>
    /// Teklif verecek Firma Bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifFirma
    {
        /// <summary>
        /// Teklif Firma Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Firma Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Firma Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Firma Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Firma Firma No
        /// </summary>
        public int firmaNo;
        /// <summary>
        /// Teklif Firma Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Teklif Firma Silinecek Mi
        /// </summary>
        public bool sil;
        /// <summary>
        /// Teklif Firma <see cref="FirmaBilgisi"/> bilgisini tutan özellik
        /// </summary>
        public FirmaBilgisi firmaBilgisi;

        /// <summary>
        /// <see cref="TeklifFirma"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifFirma()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            firmaNo = 0;
            tip = 0;
            sil = false;
            firmaBilgisi = new FirmaBilgisi();
        }
    }

    /// <summary>
    /// Teklif yapılan Dönem bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifDonem
    {
        /// <summary>
        /// Teklif Dönem Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Dönem Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Dönem Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Dönem Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Dönem No
        /// </summary>
        public int donemNo;
        /// <summary>
        /// Teklif Dönem Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Teklif Dönem Silinecek Mi
        /// </summary>
        public bool sil;

        /// <summary>
        /// <see cref="TeklifDonem"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifDonem()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            donemNo = 0;
            tarih = new TNSDateTime();
            sil = false;
        }
    }

    /// <summary>
    /// Teklif bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Teklif
    {
        /// <summary>
        /// Teklif Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Teklif yapılan malzeme <see cref="TeklifMalzeme"/> bilgisini tutan özellik
        /// </summary>
        public TeklifMalzeme malzeme;
        /// <summary>
        /// Teklif yapan firma <see cref="TeklifFirma"/> bilgisini tutan özellik
        /// </summary>
        public TeklifFirma firma;
        /// <summary>
        /// Teklif yapılan dönem <see cref="TeklifDonem"/> bilgisini tutan özellik
        /// </summary>
        public TeklifDonem donem;
        /// <summary>
        /// Teklif Birim Fiyat
        /// </summary>
        public double birimFiyat;
        /// <summary>
        /// Teklif Toplam Tutar
        /// </summary>
        public decimal toplamTutar;
        /// <summary>
        /// Teklif Kazanan Bilgisi
        /// </summary>
        public int kazanan;
        /// <summary>
        /// Teklif Açıklama
        /// </summary>
        public string aciklama;
        public int yMParaBirimi = 0;

        /// <summary>
        /// <see cref="Teklif"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Teklif()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tip = 0;
            malzeme = new TeklifMalzeme();
            firma = new TeklifFirma();
            donem = new TeklifDonem();
            birimFiyat = 0.0;
            toplamTutar = 0;
            kazanan = 0;
            aciklama = string.Empty;
        }

        /// <summary>
        /// <see cref="Teklif"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Teklif(int yil, int muhasebe, string birim, string surecNo)
        {
            this.yil = yil;
            this.muhasebe = muhasebe;
            this.birim = birim;
            this.surecNo = surecNo;
            tip = 0;
            malzeme = new TeklifMalzeme();
            firma = new TeklifFirma();
            donem = new TeklifDonem();
            birimFiyat = 0.0;
            toplamTutar = 0;
            kazanan = 0;
            aciklama = string.Empty;
        }
    }
    [Serializable()]
    public class TeklifKazananFirma
    {
        public int firmaNo;
        public int tip;
        public int muhasebeKod;
        public string vergiNo;
        public string ad;

        public TeklifKazananFirma()
        {
            firmaNo = 0;
            tip = 0;
            muhasebeKod = 0;
            vergiNo = string.Empty;
            ad = string.Empty;
        }

    }
    /// <summary>
    /// Teklif Genel Toplam bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifGenelToplam
    {
        /// <summary>
        /// Teklif Genel Toplam Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Genel Toplam Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Genel Toplam Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Genel Toplam Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Genel Toplam Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Teklif Genel Toplam 
        /// </summary>
        public double genelToplam;
        public int paraBirimi = 0;
        public TNSDateTime teklifTarihi = new TNSDateTime();

        /// <summary>
        /// <see cref="TeklifGenelToplam"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifGenelToplam()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tip = 0;
            genelToplam = 0.0;
        }
    }


    /// <summary>
    /// Üretici Fiyatları Endeksi (ÜFE) bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Ufe
    {
        /// <summary>
        /// Ufe Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ufe Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Ufe Oran
        /// </summary>
        public double oran;


        /// <summary>
        /// <see cref="Ufe"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Ufe()
        {
            yil = 0;
            ay = 0;
            oran = 0.0;
        }
    }

    /// <summary>
    /// Komisyon Üye bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class KomisyonUye
    {
        /// <summary>
        /// Komisyon Üye Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Komisyon Üye Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Komisyon Üye Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Komisyon Üye Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Komisyon Üye Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Komisyon Üye Görevlendirme No
        /// </summary>
        public int gorevlendirmeNo;
        /// <summary>
        /// Komisyon Üye Görevlendirme Tarih
        /// </summary>
        public TNSDateTime gorevlendirmeTarih;

        public string teslimAlan;
        public TNSDateTime teslimTarih;

        /// <summary>
        /// Komisyon Üyelerini <see cref="Uye"/> nesnesi koleksiyonu şeklinde tutan özellik
        /// </summary>
        public ObjectArray uyeler;

        /// <summary>
        /// <see cref="KomisyonUye"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KomisyonUye()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tip = 0;
            gorevlendirmeTarih = new TNSDateTime();
            gorevlendirmeNo = 0;
            teslimAlan = string.Empty;
            teslimTarih = new TNSDateTime();
            uyeler = new ObjectArray();
        }
    }


    /// <summary>
    /// Üye bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Uye
    {
        /// <summary>
        /// Üye Komisyon Görev
        /// </summary>
        public int komisyonGorev;
        /// <summary>
        /// Üye Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Üye Unvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Üye Görev
        /// </summary>
        public string gorev;
        /// <summary>
        /// Üye Telefon
        /// </summary>
        public string telefon;
        /// <summary>
        /// Üye Cep Telefon
        /// </summary>
        public string cepTelefon;
        /// <summary>
        /// Üye Sicil No
        /// </summary>
        public string sicilNo;

        /// <summary>
        /// <see cref="Uye"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Uye()
        {
            komisyonGorev = 0;
            ad = string.Empty;
            unvan = string.Empty;
            gorev = string.Empty;
            telefon = string.Empty;
            cepTelefon = string.Empty;
            sicilNo = string.Empty;
        }
    }

    /// <summary>
    /// Fatura belge bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FaturaForm
    {
        /// <summary>
        /// Fatura Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Fatura Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Fatura Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Fatura Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Fatura No
        /// </summary>
        public int prFaturaNo;
        public string prFaturaNoSorgu;
        /// <summary>
        /// Fatura veren Firma No
        /// </summary>
        public int firmaNo;
        /// <summary>
        /// Fatura Personel Amir
        /// </summary>
        public string personelAmiri;
        /// <summary>
        /// Fatura Personel Amir Unvan
        /// </summary>
        public string personelAmiriUnvan;
        /// <summary>
        /// Fatura veren firma bilgisini <see cref="FirmaBilgisi"/> nesnesi ile tutan özellik
        /// </summary>
        public FirmaBilgisi firma;
        /// <summary>
        /// Fatura No
        /// </summary>
        public string faturaNo;
        /// <summary>
        /// Fatura Tarih
        /// </summary>
        public TNSDateTime faturaTarih;
        /// <summary>
        /// Fatura Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Fatura ile hazırlanan Taşınır Belge No
        /// </summary>
        public string tasinirNo;
        /// <summary>
        /// Fatura ile hazırlanan Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Fatura detaylarını <see cref="FaturaDetay"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Fatura Detay Tip
        /// </summary>
        public int detayTip;
        /// <summary>
        /// Fatura Detay Sıra No
        /// </summary>
        public int detaySiraNo;
        /// <summary>
        /// Fatura Genel Toplam
        /// </summary>
        public decimal genelToplam;
        /// <summary>
        /// Fatura Özel İletişim Vergisi (ÖİV)
        /// </summary>
        public decimal oiv;
        /// <summary>
        /// Fatura KDV Tevkifatı
        /// </summary>
        public decimal kdvTevkifati;
        /// <summary>
        /// Fatura Durum
        /// </summary>
        public int durum;
        /// <summary>
        /// Fatura İskonto Tutar
        /// </summary>
        public decimal iskonto;
        /// <summary>
        /// Fatura Firma Kaydı Mı
        /// </summary>
        public bool firmaKayitYapiyor;
        /// <summary>
        /// Fatura İskonto Oran
        /// </summary>
        public decimal iskontoOran;
        /// <summary>
        /// Fatura KDV Tutar %1
        /// </summary>
        public decimal kdv1;
        /// <summary>
        /// Fatura KDV Tutar %8
        /// </summary>
        public decimal kdv8;

        /// <summary>
        /// Fatura KDV Tutar %10
        /// </summary>
        public decimal kdv10;

        /// <summary>
        /// Fatura KDV Tutar %12
        /// </summary>
        public decimal kdv12;

        /// <summary>
        /// Fatura KDV Tutar %18
        /// </summary>
        public decimal kdv18;
        /// <summary>
        /// Fatura KDV Tutar %20
        /// </summary>
        public decimal kdv20;
        /// <summary>
        /// Fatura Ödeme Emri Belge Yıl
        /// </summary>
        public int odemeEmriYil;
        /// <summary>
        /// Fatura Taşınır Belge Yıl
        /// </summary>
        public int tasinirYil;
        public int paraBirimi = 0;

        public TeklifMalzeme tMalzeme = new TeklifMalzeme();

        /// <summary>
        /// <see cref="FaturaForm"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FaturaForm()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            personelAmiri = string.Empty;
            personelAmiriUnvan = string.Empty;
            prFaturaNo = 0;
            prFaturaNoSorgu = "";
            firmaNo = 0;
            firma = new FirmaBilgisi();
            faturaNo = string.Empty;
            tasinirNo = string.Empty;
            odemeEmriNo = string.Empty;
            faturaTarih = new TNSDateTime();
            aciklama = string.Empty;
            detay = new ObjectArray();
            detayTip = 0;
            detaySiraNo = 0;
            genelToplam = 0;
            oiv = 0;
            kdvTevkifati = 0;
            durum = 0;
            iskonto = 0;
            firmaKayitYapiyor = false;
            iskontoOran = 0;
            kdv1 = 0;
            kdv8 = 0;
            kdv10 = 0;
            kdv12 = 0;
            kdv18 = 0;
            kdv20 = 0;
            odemeEmriYil = 0;
            tasinirYil = 0;
            tMalzeme = new TeklifMalzeme();
        }

        public decimal kdvsizToplam { get; set; }
    }

    /// <summary>
    /// Fatura Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FaturaDetay
    {
        /// <summary>
        /// Fatura Detay Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Fatura Detay Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Fatura Detay Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Fatura Detay Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Fatura No
        /// </summary>
        public int prFaturaNo;
        /// <summary>
        /// Fatura Detay Malzeme Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// Fatura Detay Malzeme Miktar
        /// </summary>
        public decimal miktar;
        /// <summary>
        /// Fatura Detay Malzeme KDV Oran
        /// </summary>
        public int kdvOran;
        /// <summary>
        /// Fatura Detay Malzeme Birim Fiyat
        /// </summary>
        public decimal birimFiyat;
        /// <summary>
        /// Fatura Detay Satır Toplam
        /// </summary>
        public decimal satirToplam;
        /// <summary>
        /// Fatura Detay Belge Sahibi
        /// </summary>
        public string belgeSahibi;
        /// <summary>
        /// Fatura Detay Belge Tarihi
        /// </summary>
        public TNSDateTime belgeTarihi;
        /// <summary>
        /// Fatura Detay Belge No
        /// </summary>
        public string belgeNo;
        /// <summary>
        /// Fatura Detay Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Fatura Detay Tip
        /// </summary>
        public int detayTip;
        /// <summary>
        /// Fatura Detay Sıra No
        /// </summary>
        public int detaySiraNo;
        /// <summary>
        /// Fatura Detay Belge Sahibi TC Kimlik No
        /// </summary>
        public string belgeSahibiTCNo;
        /// <summary>
        /// Fatura Detay KDV Tutarı
        /// </summary>
        public decimal kdvTutari;
        /// <summary>
        /// Fatura Detay Malzeme No
        /// </summary>
        public int malzemeNo;
        /// <summary>
        /// Fatura Detay Malzeme Kod
        /// </summary>
        public string malzemeKod;
        /// <summary>
        /// Fatura Detay Malzeme Ölçü Birim Kod
        /// </summary>
        public int olcuBirimKod;
        /// <summary>
        /// Fatura Detay Malzeme Ölçü Birim Ad
        /// </summary>
        public string olcuBirimAd;
        /// <summary>
        /// Fatura Detay Belgeyi Alan Ad Soyad
        /// </summary>
        public string belgeyiAlan;
        /// <summary>
        /// Fatura Detay Belgeyei Alan TC Kimlik No
        /// </summary>
        public string belgeyiAlanTCNo;
        /// <summary>
        /// Fatura Detay Malzeme Indirimli Birim Fiyat
        /// </summary>
        public decimal birimFiyatIndirimli;

        /// <summary>
        /// <see cref="FaturaDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FaturaDetay()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            prFaturaNo = 0;
            adi = string.Empty;
            miktar = 0;
            kdvOran = 0;
            birimFiyat = 0;
            satirToplam = 0;
            belgeSahibi = string.Empty;
            belgeTarihi = new TNSDateTime();
            belgeNo = string.Empty;
            aciklama = string.Empty;
            detayTip = 0;
            detaySiraNo = 0;
            belgeSahibiTCNo = string.Empty;
            kdvTutari = 0;
            malzemeNo = 0;
            malzemeKod = string.Empty;
            olcuBirimKod = 0;
            olcuBirimAd = string.Empty;
            belgeyiAlan = string.Empty;
            belgeyiAlanTCNo = string.Empty;
            birimFiyatIndirimli = 0;
        }
    }

    /// <summary>
    /// Taşınır Belge bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TasinirForm
    {
        /// <summary>
        /// Taşınır Belge Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Taşınır Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Taşınır Belge Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Taşınır Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Taşınır Belge No
        /// </summary>
        public int prTasinirNo;
        /// <summary>
        /// Programatik Fatura No
        /// </summary>
        public int prFaturaNo;
        public string prFaturaNoSorgu;
        /// <summary>
        /// Taşınır Belge No
        /// </summary>
        public string tasinirNo;
        /// <summary>
        /// Taşınır Belge Tarih
        /// </summary>
        public TNSDateTime tasinirTarih;
        /// <summary>
        /// Taşınır Belge Harcama Birim Kod
        /// </summary>
        public string harcamaBirimKod;
        /// <summary>
        /// Taşınır Belge Ambar Kod
        /// </summary>
        public string ambarKod;
        /// <summary>
        /// Taşınır Belge Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Taşınır Belge detay bilgilerini <see cref="TasinirDetay"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Taşınır Belge Yıl
        /// </summary>
        public int tasinirYil;
        public double dovizKur = 0.0;

        /// <summary>
        /// <see cref="TasinirForm"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TasinirForm()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            prTasinirNo = 0;
            prFaturaNo = 0;
            prFaturaNoSorgu = "";
            tasinirNo = string.Empty;
            tasinirTarih = new TNSDateTime();
            harcamaBirimKod = string.Empty;
            ambarKod = string.Empty;
            aciklama = string.Empty;
            detay = new ObjectArray();
            tasinirYil = 0;
        }
    }

    /// <summary>
    /// Taşınır Belge Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TasinirDetay
    {
        /// <summary>
        /// Taşınır Detay Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Taşınır Detay Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Taşınır Detay Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Taşınır Detay Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Taşınır No
        /// </summary>
        public int prTasinirNo;
        /// <summary>
        /// Taşınır Detay Malzeme Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// Taşınır Detay Malzeme Hesap Plan Kod
        /// </summary>
        public string hesapPlanKod;
        /// <summary>
        /// Taşınır Detay Malzeme Hesap Plan Ad
        /// </summary>
        public string hesapPlanAd;
        /// <summary>
        /// Taşınır Detay Malzeme Miktar
        /// </summary>
        public double miktar;
        /// <summary>
        /// Taşınır Detay Malzeme Ölçü Birim Kod
        /// </summary>
        public int olcuBirimKod;
        /// <summary>
        /// Taşınır Detay Malzeme KDV Oran
        /// </summary>
        public int kdvOran;
        /// <summary>
        /// Taşınır Detay Malzeme Birim Fiyat
        /// </summary>
        public double birimFiyat;
        /// <summary>
        /// Taşınır Detay Malzeme için Taşınır Belge Oluşturuldu Mu
        /// </summary>
        public int olusturuldu;
        /// <summary>
        /// Taşınır Detay Fatura Detay Sıra No
        /// </summary>
        public int faturaDetaySiraNo;

        /// <summary>
        /// <see cref="TasinirDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TasinirDetay()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            prTasinirNo = 0;
            adi = string.Empty;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            miktar = 0.0;
            olcuBirimKod = 0;
            kdvOran = 0;
            birimFiyat = 0.0;
            olusturuldu = 0;
            faturaDetaySiraNo = 0;
        }
    }

    /// <summary>
    /// Ödeme Emri Belge bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class OdemeEmriMIFForm
    {
        /// <summary>
        /// Ödeme Emri Belge Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ödeme Emri Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Ödeme Emri Belge Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Ödeme Emri Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Ödeme Emri Belge No
        /// </summary>
        public int prOdemeEmriNo;
        /// <summary>
        /// Programatik Fatura No
        /// </summary>
        public int prFaturaNo;
        public string prFaturaNoEk = "";
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Ödeme Emri Belge Tarih
        /// </summary>
        public TNSDateTime odemeEmriTarih;
        /// <summary>
        /// Ödeme Emri Belge Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Ödeme Emri Belge detay bilgilerini <see cref="OdemeEmriMIFDetay"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Ödeme Emri Belge Firma No
        /// </summary>
        public int firmaNo;
        /// <summary>
        /// Ödeme Emri Belge Tur
        /// </summary>
        public int belgeTur;
        /// <summary>
        /// Ödeme Emri Belge Toplam Ödeme
        /// </summary>
        public double toplamOdeme;//Kayıt edilmiyor
        /// <summary>
        /// Ödeme Emri Belge Yıl
        /// </summary>
        public int odemeEmriYil;

        /// <summary>
        /// <see cref="OdemeEmriMIFForm"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdemeEmriMIFForm()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            prOdemeEmriNo = 0;
            prFaturaNo = 0;
            prFaturaNoEk = "";
            odemeEmriNo = string.Empty;
            odemeEmriTarih = new TNSDateTime();
            aciklama = string.Empty;
            detay = new ObjectArray();
            firmaNo = 0;
            belgeTur = 0;
            toplamOdeme = 0;
            odemeEmriYil = 0;
        }
    }

    /// <summary>
    /// Ödeme Emri Belge Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class OdemeEmriMIFDetay
    {
        /// <summary>
        /// Ödeme Emri Belge Detay Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ödeme Emri Belge Detay Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Ödeme Emri Belge Detay Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Ödeme Emri Belge Detay Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Ödeme Emri Belge No
        /// </summary>
        public int prOdemeEmriNo;
        /// <summary>
        /// Ödeme Emri Belge Detay Hesap Plan Kod
        /// </summary>
        public string hesapPlanKod;
        /// <summary>
        /// Ödeme Emri Belge Detay Hesap Plan Ad
        /// </summary>
        public string hesapPlanAd;
        /// <summary>
        /// Ödeme Emri Belge Detay Borç Tutar
        /// </summary>
        public double borc;
        /// <summary>
        /// Ödeme Emri Belge Detay Alacak Tutar
        /// </summary>
        public double alacak;
        /// <summary>
        /// Ödeme Emri Belge Detay Tertip Bilgisi
        /// </summary>
        public string tertip;
        /// <summary>
        /// Ödeme Emri Belge Detay Yansıtma Bilgisi
        /// </summary>
        public string yansitma;
        public string altHesapKod = "";

        /// <summary>
        /// <see cref="OdemeEmriMIFDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OdemeEmriMIFDetay()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            prOdemeEmriNo = 0;
            hesapPlanKod = string.Empty;
            hesapPlanAd = string.Empty;
            borc = 0.0;
            alacak = 0.0;
            tertip = string.Empty;
            yansitma = string.Empty;
            mahsupBelgeNo = null;
        }

        public string mahsupBelgeNo { get; set; }

        public string tertipVeMahsupBelgeNo
        {
            get { return tertip + "@" + mahsupBelgeNo; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] alanlar = value.Split('@');
                    tertip = OrtakFonksiyonlar.DiziElemanAl(alanlar, 0, "");
                    mahsupBelgeNo = OrtakFonksiyonlar.DiziElemanAl(alanlar, 1, "");
                }
                else
                {
                    tertip = "";
                    mahsupBelgeNo = "";
                }
            }
        }
    }

    /// <summary>
    /// Firma Bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FirmaBilgisi
    {
        /// <summary>
        /// Firma Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Firma kayıtlı olduğu Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Firma kayıtlı olduğu Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Firma Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Firma Vergi No
        /// </summary>
        public string vergiNo;
        /// <summary>
        /// Firma Vergi Dairesi
        /// </summary>
        public string vergiDairesi;
        /// <summary>
        /// Firma Banka
        /// </summary>
        public string banka;

        /// <summary>
        /// Firma Hesap No
        /// </summary>
        public string hesapNo;
        /// <summary>
        /// Firma E-Mail
        /// </summary>
        public string eMail;
        /// <summary>
        /// Firma Yetkilisi Ad Soyad
        /// </summary>
        public string firmaYetkilisi;
        /// <summary>
        /// Firma Telefon
        /// </summary>
        public string telefon;
        /// <summary>
        /// Firma Faks
        /// </summary>
        public string faks;
        /// <summary>
        /// Firma Adres
        /// </summary>
        public string adres;
        /// <summary>
        /// Firma Uyruk
        /// </summary>
        public int uyruk;
        /// <summary>
        /// Firma Listede Görünsün Mü
        /// </summary>
        public int gorulmesin;
        /// <summary>
        /// Firma Yasaklı Mı
        /// </summary>
        public int yasakli;
        /// <summary>
        /// Firma Cep Telefon
        /// </summary>
        public string cepTel;
        /// <summary>
        /// Firma Web Adresi
        /// </summary>
        public string web;
        /// <summary>
        /// Firma Fatura
        /// </summary>
        public int fatura;

        /// <summary>
        /// Firma Parola
        /// </summary>
        public string parola;
        /// <summary>
        /// Firma Sisteme Son Giriş Tarih
        /// </summary>
        public TNSDateTime sonGirisTarih;

        public string bicSwift;

        public string kisiUnvan;

        public int ekGosterge;

        public int derece;
        public string aciklama = "";

        public int karaListe;

        public string adresNo = "";

        /// <summary>
        /// <see cref="FirmaBilgisi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FirmaBilgisi()
        {
            kod = 0;
            muhasebe = 0;
            birim = string.Empty;
            ad = string.Empty;
            vergiNo = string.Empty;
            vergiDairesi = string.Empty;
            banka = string.Empty;
            hesapNo = string.Empty;
            eMail = string.Empty;
            firmaYetkilisi = string.Empty;
            telefon = string.Empty;
            faks = string.Empty;
            adres = string.Empty;
            uyruk = 0;
            gorulmesin = 0;
            yasakli = 0;
            cepTel = string.Empty;
            web = string.Empty;
            fatura = 0;
            parola = string.Empty;
            sonGirisTarih = new TNSDateTime();
            bicSwift = string.Empty;
            kisiUnvan = string.Empty;
            ekGosterge = 0;
            derece = 0;
            aktifDurum = 0;
            personeldeDeAra = false;
        }

        /// <summary>
        /// 1 ise aktifler, 2 ise aktif olmayanlar , 0 ise hepsi
        /// </summary>
        public int aktifDurum { get; set; }

        public bool personeldeDeAra { get; set; }
    }

    /// <summary>
    /// Onay Form Belgesi bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class OnayForm
    {
        /// <summary>
        /// Onay Form Belge Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Onay Form Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Onay Form Belge Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Onay Form Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Onay Form Belge İdare Ad
        /// </summary>
        public string idareAdi;
        /// <summary>
        /// Onay Form Belge Tarih
        /// </summary>
        public TNSDateTime belgeTarihi;
        /// <summary>
        /// Onay Form Belge Evrak Sayı
        /// </summary>
        public string evrakSayi;
        /// <summary>
        /// Onay Form Belge İş Tanım
        /// </summary>
        public string isTanimi;
        /// <summary>
        /// Onay Form Belge İş Nitelik
        /// </summary>
        public string isNiteligi;
        /// <summary>
        /// Onay Form Belge İş Miktar
        /// </summary>
        public string isMiktari;
        /// <summary>
        /// Onay Form Belge Odenek Yıl
        /// </summary>
        public int odenekYili;
        /// <summary>
        /// Yaklaşık Maliyet Tutarı
        /// </summary>
        public double yaklasikMaliyet;
        /// <summary>
        /// Yaklaşık Maliyet Para Birim Kod
        /// </summary>
        public int yMParaBirimi;
        /// <summary>
        /// Yaklaşık Maliyet Tip
        /// </summary>
        public int yMTip;
        /// <summary>
        /// Onay Form Belge tertip bilgisini <see cref="OnayFormTertip"/> nesne koleksiyonu olarak tutan özellik 
        /// </summary>
        public ObjectArray tertip;
        /// <summary>
        /// İhale Döküman Bedeli
        /// </summary>
        public string dokumanBedeli;
        /// <summary>
        /// Proje No
        /// </summary>
        public string projeNo;
        /// <summary>
        /// Avans Var Mı
        /// </summary>
        public int avansVarmi;
        /// <summary>
        /// Avans Şartları
        /// </summary>
        public string avansSartlari;
        /// <summary>
        /// İhale Usul
        /// </summary>
        public int ihaleUsulu;
        /// <summary>
        /// İhale Madde
        /// </summary>
        public int ihaleMaddesi;
        /// <summary>
        /// İhale Madde Ad
        /// </summary>
        public string ihaleMaddeAdi;
        /// <summary>
        /// İlan Verilecek Mi
        /// </summary>
        public int ilanVerilecekmi;
        /// <summary>
        /// İlan Şekli
        /// </summary>
        public string ilanSekli;
        /// <summary>
        /// Ön Yeterlilik Tutar
        /// </summary>
        public double onYeterlilik;
        /// <summary>
        /// Fiyat Farkı Var Mı
        /// </summary>
        public int fiyatFarkiVarmi;
        /// <summary>
        /// Fiyat Farkı
        /// </summary>
        public string fiyatFarki;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Gerçekleştirme Görevlisi Ad Soyad
        /// </summary>
        public string gercekGorevli;
        /// <summary>
        /// Gerçekleştirme Görevlisi Unvan
        /// </summary>
        public string gercekGorevliUnvan;
        /// <summary>
        /// İhale Yetkilisi Ad Soyad
        /// </summary>
        public string ihaleYetkili;
        /// <summary>
        /// İhale Yetkilisi Unvan
        /// </summary>
        public string ihaleYetkiliUnvan;
        /// <summary>
        /// Onay Form Belge Onaylama Tarih
        /// </summary>
        public TNSDateTime onaylamaTarihi;
        /// <summary>
        /// Onay Form Belge Onaya Sunum Tarih
        /// </summary>
        public TNSDateTime onayaSunumTarihi;
        /// <summary>
        /// Onay Form Belge İhale Makam Bilgisi
        /// </summary>
        public string ihaleMakami;
        /// <summary>
        /// Komisyon Var Mı
        /// </summary>
        public int komisyonVarmi;
        /// <summary>
        /// İhale İlan Tanım bilgisini <see cref="IhaleIlanTanim"/> nesnesi ile tutan özellik
        /// </summary>
        public IhaleIlanTanim ilanTanimi;
        /// <summary>
        /// Onay Form Belge Süreç bilgisini <see cref="Surec"/> nesnesi ile tutan özellik
        /// </summary>
        public Surec surec;
        /// <summary>
        /// Harcama Talimatı Gerçekleştirme Süresi
        /// </summary>
        public string hTgerceklestirmeSuresi;
        /// <summary>
        /// Harcama Talimatı Gerçekleştirme Usul
        /// </summary>
        public string hTgerceklestirmeUsulu;
        /// <summary>
        /// Harcama Talimatı Gerçekleştirme Görevlisi Ad Soyad
        /// </summary>
        public string hTgerceklestirmeGorevli;
        /// <summary>
        /// Harcama Talimatı Mihmandar Var Mı
        /// </summary>
        public int hTmihmandarlikVarmi;
        /// <summary>
        /// Harcama Talimatı Gerçekleştirme Personel TC Kimlik No
        /// </summary>
        public string hTperTcNo;
        /// <summary>
        /// Harcama Talimatı Mı
        /// </summary>
        public int hrcTalimati;
        /// <summary>
        /// Ön İlan Var Mı
        /// </summary>
        public int onIlanVarMi;
        /// <summary>
        /// Ön İlanın Yılın Hangi Çeyreğinde olduğunun bilgisi
        /// </summary>
        public int onIlanYilCeyrek;
        /// <summary>
        /// Ön İlan Gerçekleştirme Görevlisi Ad Soyad
        /// </summary>
        public string onIlanGercekGorevli;
        /// <summary>
        /// Ön İlan Gerçekleştirme Görevlisi Unvan
        /// </summary>
        public string onIlanGercekGorevliUnvan;
        /// <summary>
        /// Ön İlan Ihale Yetkilisi Ad Soyad 
        /// </summary>
        public string onIlanIhaleYetkili;
        /// <summary>
        /// Ön İlan Ihale Yetkilisi Unvan
        /// </summary>
        public string onIlanIhaleYetkiliUnvan;
        /// <summary>
        /// Ön İlan Onaylama Tarih
        /// </summary>
        public TNSDateTime onIlanOnaylamaTarihi;
        /// <summary>
        /// Ön İlan Onaya Sunum Tarih
        /// </summary>
        public TNSDateTime onIlanOnayaSunumTarihi;
        /// <summary>
        /// İhale Dokümanının Posta Yoluyla Satışı Yapılacak Mı
        /// </summary>
        public int dokumanPostaSatisVarMi;
        /// <summary>
        /// İhale Dökümanının Yurtiçi Posta Bedeli
        /// </summary>
        public string dokumanYurticiBedel;
        /// <summary>
        /// İhale Dökümanının Yurtdışı Posta Bedeli
        /// </summary>
        public string dokumanYurtdisiBedel;

        public string harcGorevAd;
        public string harcGorevUnvan;
        public bool harcGorevliVar;

        /// <summary>
        /// <see cref="OnayForm"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OnayForm()
        {
            harcGorevliVar = false;
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            idareAdi = string.Empty;
            belgeTarihi = new TNSDateTime();
            evrakSayi = string.Empty;
            isTanimi = string.Empty;
            isNiteligi = string.Empty;
            isMiktari = string.Empty;
            odenekYili = 0;
            yaklasikMaliyet = 0.0;
            yMParaBirimi = 1;//YTL
            yMTip = 0;
            dokumanBedeli = string.Empty;
            tertip = new ObjectArray();
            projeNo = string.Empty;
            avansVarmi = 0;
            avansSartlari = string.Empty;
            ihaleUsulu = 0;
            ihaleMaddesi = 0;
            ihaleMaddeAdi = string.Empty;
            ilanVerilecekmi = 0;
            ilanSekli = string.Empty;
            onYeterlilik = 0.0;
            fiyatFarkiVarmi = 0;
            fiyatFarki = string.Empty;
            aciklama = string.Empty;
            gercekGorevli = string.Empty;
            ihaleYetkili = string.Empty;
            gercekGorevliUnvan = string.Empty;
            ihaleYetkiliUnvan = string.Empty;
            onaylamaTarihi = new TNSDateTime();
            onayaSunumTarihi = new TNSDateTime();
            ihaleMakami = string.Empty;
            komisyonVarmi = 0;
            ilanTanimi = new IhaleIlanTanim();
            surec = new Surec();
            hTgerceklestirmeSuresi = string.Empty;
            hTgerceklestirmeUsulu = string.Empty;
            hTgerceklestirmeGorevli = string.Empty;
            hTmihmandarlikVarmi = 0;
            hTperTcNo = string.Empty;
            hrcTalimati = 0;//1 olursa bu sınıf harcama talimatıdır
            onIlanVarMi = 0;
            onIlanYilCeyrek = 0;
            onIlanGercekGorevli = string.Empty;
            onIlanGercekGorevliUnvan = string.Empty;
            onIlanIhaleYetkili = string.Empty;
            onIlanIhaleYetkiliUnvan = string.Empty;
            onIlanOnayaSunumTarihi = new TNSDateTime();
            onIlanOnaylamaTarihi = new TNSDateTime();
            dokumanPostaSatisVarMi = 0;
            dokumanYurticiBedel = string.Empty;
            dokumanYurtdisiBedel = string.Empty;
        }

        public OnayForm(int yil, int muhasebe, string birim, string surecNo)
        {
            this.yil = yil;
            this.muhasebe = muhasebe;
            this.birim = birim;
            this.surecNo = surecNo;

            harcGorevliVar = false;
            idareAdi = string.Empty;
            belgeTarihi = new TNSDateTime();
            evrakSayi = string.Empty;
            isTanimi = string.Empty;
            isNiteligi = string.Empty;
            isMiktari = string.Empty;
            odenekYili = 0;
            yaklasikMaliyet = 0.0;
            yMParaBirimi = 1;//YTL
            yMTip = 0;
            dokumanBedeli = string.Empty;
            tertip = new ObjectArray();
            projeNo = string.Empty;
            avansVarmi = 0;
            avansSartlari = string.Empty;
            ihaleUsulu = 0;
            ihaleMaddesi = 0;
            ihaleMaddeAdi = string.Empty;
            ilanVerilecekmi = 0;
            ilanSekli = string.Empty;
            onYeterlilik = 0.0;
            fiyatFarkiVarmi = 0;
            fiyatFarki = string.Empty;
            aciklama = string.Empty;
            gercekGorevli = string.Empty;
            ihaleYetkili = string.Empty;
            gercekGorevliUnvan = string.Empty;
            ihaleYetkiliUnvan = string.Empty;
            onaylamaTarihi = new TNSDateTime();
            onayaSunumTarihi = new TNSDateTime();
            ihaleMakami = string.Empty;
            komisyonVarmi = 0;
            ilanTanimi = new IhaleIlanTanim();
            surec = new Surec();
            hTgerceklestirmeSuresi = string.Empty;
            hTgerceklestirmeUsulu = string.Empty;
            hTgerceklestirmeGorevli = string.Empty;
            hTmihmandarlikVarmi = 0;
            hTperTcNo = string.Empty;
            hrcTalimati = 0;//1 olursa bu sınıf harcama talimatıdır
            onIlanVarMi = 0;
            onIlanYilCeyrek = 0;
            onIlanGercekGorevli = string.Empty;
            onIlanGercekGorevliUnvan = string.Empty;
            onIlanIhaleYetkili = string.Empty;
            onIlanIhaleYetkiliUnvan = string.Empty;
            onIlanOnayaSunumTarihi = new TNSDateTime();
            onIlanOnaylamaTarihi = new TNSDateTime();
            dokumanPostaSatisVarMi = 0;
            dokumanYurticiBedel = string.Empty;
            dokumanYurtdisiBedel = string.Empty;
        }

        public string TertipKodlariAl(bool tekTertip, bool miktarEkle = false)
        {
            string tertip = "";
            foreach (OnayFormTertip t in this.tertip.objeler)
            {
                if (t.kurKod != "")
                {
                    if (tertip != "") tertip += ";";
                    tertip += t.kurKod + "-" + t.fonKod + "-" + t.finKod + "-" + t.ekoKod;
                    if (!string.IsNullOrWhiteSpace(t.pebKod))
                        tertip += "-" + t.pebKod;
                    if (tekTertip) return tertip;
                    if (miktarEkle)
                    {
                        tertip += "@" + t.kullanilacakTutar.ToString();
                    }
                }
            }
            return tertip;
        }

        public string EkoKod2DuzeyAl(bool tekTertip, int eko2Uz)
        {
            string ekoKod = "";
            foreach (OnayFormTertip t in this.tertip.objeler)
            {
                if (t.ekoKod != "")
                {
                    string eko = t.ekoKod.Replace(".", "");
                    if (eko2Uz == 3)
                        eko = eko.Substring(0, 2) + ".0" + eko.Substring(2, 1);
                    else
                        eko = eko.Substring(0, 4);
                    if (tekTertip) return eko;
                    if (ekoKod != "") ekoKod += ";";
                    ekoKod += eko;
                }
            }
            return ekoKod;
        }

        public bool DogrudanTeminMi()
        {
            if (this.hrcTalimati == 1) return true;
            return DogrudanTeminMi(ihaleMaddeAdi);
        }

        public static bool DogrudanTeminMi(string ihaleMaddeAdi)
        {
            return IcindeMi(ihaleMaddeAdi, TNS.MUH.Arac.YuzdeOnAlimTurBul());
        }

        private static bool IcindeMi(string ihaleMaddeAdi, string[] dogrudanTeminMaddeKodlari)
        {
            if (!string.IsNullOrWhiteSpace(ihaleMaddeAdi))
            {
                ihaleMaddeAdi = ihaleMaddeAdi.Replace(".", "");
                for (int i = 0; i < dogrudanTeminMaddeKodlari.Length; i++)
                {
                    if (dogrudanTeminMaddeKodlari[i] == ihaleMaddeAdi)
                        return true;
                }
            }
            return false;
        }

    }

    /// <summary>
    /// Onay Form Belge Tertip bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class OnayFormTertip
    {
        /// <summary>
        /// Onay Form Belge Tertip Süreç Yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Onay Form Belge Tertip Muhasebe Kod
        /// </summary>
        public int muhasebe { get; set; }

        /// <summary>
        /// Onay Form Belge Tertip Birim Kod
        /// </summary>
        public string birim { get; set; }

        /// <summary>
        /// Onay Form Belge Tertip Birim Kod
        /// </summary>
        public string tertip
        {
            get { return kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod + "-" + pebKod; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] kodlar = value.Split('-');
                    kurKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 0, "");
                    fonKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 1, "");
                    finKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 2, "");
                    ekoKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 3, "");
                    pebKod = TNS.MUH.Arac.DiziElemanAl(kodlar, 4, "");
                }
            }
        }

        /// <summary>
        /// Onay Form Belge Tertip Süreç No
        /// </summary>
        public string surecNo { get; set; }

        /// <summary>
        /// Onay Form Belge Tertip Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// Tertip Yıl
        /// </summary>
        public int tertipYil { get; set; }

        /// <summary>
        /// Onay Form Belge Tertip Kurumsal Kod
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Onay Form Belge Tertip Fonksiyonel Kod
        /// </summary>
        public string fonKod;

        /// <summary>
        /// Onay Form Belge Tertip Finansman Kod
        /// </summary>
        public string finKod;

        /// <summary>
        /// Onay Form Belge Tertip Ekonomik Kod
        /// </summary>
        public string ekoKod;

        /// <summary>
        /// Onay Form Belge Tertip Alt Faaliyet Kod
        /// </summary>
        public string pebKod;

        /// <summary>
        /// Onay Form Belge Tertip Ödenek Tutar
        /// </summary>
        public double odenekTutari { get; set; }

        /// <summary>
        /// Onay Form Belge Tertip için Bloke yapılacak Tutar
        /// </summary>
        public double blokeTutari { get; set; }

        public double toplamKullanilacak { get { return kullanilacakTutar + kdvTutar; } }
        /// <summary>
        /// Onay Form Belge Tertip için Kullanılacak Tutar
        /// </summary>
        public double kullanilacakTutar;

        /// <summary>
        /// Tahmini KDV Tutar
        /// </summary>
        public double kdvTutar;

        public string isinAdi { get; set; }

        public double sozlesmeKdvliTutar = 0;
        /// <summary>
        /// <see cref="OnayFormTertip"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public OnayFormTertip()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            siraNo = 0;
            tertipYil = 0;
            kurKod = string.Empty;
            fonKod = string.Empty;
            finKod = string.Empty;
            ekoKod = string.Empty;
            pebKod = string.Empty;
            odenekTutari = 0.0;
            blokeTutari = 0.0;
            kullanilacakTutar = 0.0;
            kdvTutar = 0.0;
        }

        public int surecDurum { get; set; }
        public string surecDurumAd
        {
            get
            {
                if (surecDurum < 5) return "İşlemde";
                else if (surecDurum < 9) return "Arşiv";
                else return "İptal";
            }
        }
    }

    /// <summary>
    /// Tek Kaynaktan Temin Formu bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TekKaynaktanTemin
    {
        /// <summary>
        /// Tek Kaynaktan Temin Formu Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Tek Kaynaktan Temin Formu Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Tek Kaynaktan Temin Formu Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Tek Kaynaktan Temin Formu Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Tek Kaynaktan Temin Formu Firma Kod
        /// </summary>
        public int firmaKod;
        /// <summary>
        /// Tek Kaynaktan Temin Formu Yaklaşık Maliyet Tutar
        /// </summary>
        public double yaklasikMaliyet;
        /// <summary>
        /// Tek Kaynaktan Temin Formu Teslimat Tarih
        /// </summary>
        public TNSDateTime teslimatZamani;
        /// <summary>
        /// Tek Kaynaktan Temin Formu İhtiyaç Konu
        /// </summary>
        public string ihtiyacKonusu;
        /// <summary>
        /// Tek Kaynaktan Temin Formu Gerçek ya da Tüzel Kişi bilgisi
        /// </summary>
        public string gercekTuzel;

        /// <summary>
        /// <see cref="TekKaynaktanTemin"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TekKaynaktanTemin()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            firmaKod = 0;
            yaklasikMaliyet = 0.0;
            teslimatZamani = new TNSDateTime();
            ihtiyacKonusu = string.Empty;
            gercekTuzel = string.Empty;
        }
    }

    /// <summary>
    /// İhale Tür bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleTur
    {
        /// <summary>
        /// İhale Tür Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// İhale Tür Ad
        /// </summary>
        public string adi;

        /// <summary>
        /// <see cref="IhaleTur"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleTur()
        {
            kod = 0;
            adi = string.Empty;
        }
    }

    /// <summary>
    /// İhale Tür bilgisine ait İhale Madde bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleTurMadde
    {
        /// <summary>
        /// İhale Madde Kod
        /// </summary>
        public int kod { get; set; }
        public string goster { get { return adi + ": " + aciklama; } }
        /// <summary>
        /// İhale Tür Kod
        /// </summary>
        public int ihaleKod;
        /// <summary>
        /// İhale Madde Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// İhale Madde Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// İhale Madde için ilan zorunlu mu
        /// </summary>
        public int ilanZorunlu;

        /// <summary>
        /// <see cref="IhaleTurMadde"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleTurMadde()
        {
            kod = 0;
            ihaleKod = 0;
            adi = string.Empty;
            aciklama = string.Empty;
            ilanZorunlu = 0;
        }
    }

    /// <summary>
    /// İhale İlan Tanım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleIlanTanim
    {
        /// <summary>
        /// İhale Tür Kod
        /// </summary>
        public int ihaleKod;
        /// <summary>
        /// İhale Madde Sıra No
        /// </summary>
        public int ihaleMaddeSira;
        /// <summary>
        /// İhale İlan Tanım Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Mal ve Hizmet Alımı Yaklaşık Maliyet Tutar Alt Sınır
        /// </summary>
        public double yaklasikMaliyetMalHizmet;
        /// <summary>
        /// Mal ve Hizmet Alımı Yaklaşık Maliyet Tutar Üst Sınır
        /// </summary>
        public double yaklasikMaliyetMalHizmetUst;
        /// <summary>
        /// Yapım İşi Yaklaşık Maliyet Tutar Alt Sınır
        /// </summary>
        public double yaklasikMaliyetYapim;
        /// <summary>
        /// Yapım İşi Yaklaşık Maliyet Tutar Üst Sınır
        /// </summary>
        public double yaklasikMaliyetYapimUst;
        /// <summary>
        /// İhale İlanı için gün bazında zaman kısıtı
        /// </summary>
        public int zaman;
        /// <summary>
        /// İhale İlanı Başlama Tarihi Tür
        /// </summary>
        public int zamanBaslamaTur;

        /// <summary>
        /// <see cref="IhaleIlanTanim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleIlanTanim()
        {
            ihaleKod = 0;
            ihaleMaddeSira = 0;
            aciklama = string.Empty;
            yaklasikMaliyetMalHizmet = 0.0;
            yaklasikMaliyetMalHizmetUst = 0.0;
            yaklasikMaliyetYapim = 0.0;
            yaklasikMaliyetYapimUst = 0.0;
            zaman = 0;
            zamanBaslamaTur = 0;
        }
    }

    /// <summary>
    /// İhale İlan Bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleIlanBilgisi
    {
        /// <summary>
        /// İhale İlan Bilgi Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale İlan Bilgi Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale İlan Bilgi Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale İlan Bilgi Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İhale İlan Bilgi Süreç İş Kod
        /// </summary>
        public int islemKod;
        /// <summary>
        /// İhale İlan Bilgi Süreç Parça Kod
        /// </summary>
        public int parcaKod;
        /// <summary>
        /// İhale İlan Bilgi Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// İhale İlan Bilgi Teslim Yeri
        /// </summary>
        public string teslimYeri;
        /// <summary>
        /// İhale İlan Bilgi Teslim Tarih
        /// </summary>
        public string teslimTarihi;
        /// <summary>
        /// İhale İlan Bilgi İhale Yeri
        /// </summary>
        public string ihaleYeri;
        /// <summary>
        /// İhale İlan Bilgi İhale Tarih
        /// </summary>
        public TNSDateTime ihaleTarihi;
        /// <summary>
        /// İhale İlan Bilgi İhale Saat
        /// </summary>
        public string ihaleSaati;
        /// <summary>
        /// İhale İlan Bilgi Döküman Temin Yeri
        /// </summary>
        public string ihaleTeminYeri;
        /// <summary>
        /// İhale İlan Bilgi Teklif Süresi
        /// </summary>
        public int teklifSuresi;
        /// <summary>
        /// İhale İlan Bilgi İş Süresi
        /// </summary>
        public int isSuresi;
        /// <summary>
        /// İhale İlan Bilgi Teklif Tur
        /// </summary>
        public int teklifTuru;
        /// <summary>
        /// İhale İlan Bilgi Teklif Belirleme Tur
        /// </summary>
        public int teklifBelirlemeTuru;
        /// <summary>
        /// İhale İlan Bilgi Katılım Tur
        /// </summary>
        public int katilimTuru;
        /// <summary>
        /// İhale İlan Bilgi İndirim Oran
        /// </summary>
        public int indirimOrani;
        /// <summary>
        /// İhale İlan Bilgi Konsorsiyum Katılabilir Mi
        /// </summary>
        public int konsorsiyum;
        /// <summary>
        /// İhale İlan Bilgi İlan Verilecek Mi
        /// </summary>
        public bool ilanVerilecekmi;
        /// <summary>
        /// İhale İlan Bilgi İlan Adet
        /// </summary>
        public int ilanAdedi;
        /// <summary>
        /// İhale İlan Bilgi İlan Şekli
        /// </summary>
        public int ilanSekli;
        /// <summary>
        /// İhale İlan Bilgi Kaynak Tur
        /// </summary>
        public int kaynakTuru;
        /// <summary>
        /// İhale Kısmi Teklife Açık Mı
        /// </summary>
        public int kismiTeklifeAcikMi;
        /// <summary>
        /// İhale İlan Bilgi Sektör Kod (KİK'e ait bilgi)
        /// </summary>
        public string sektorKodu;
        /// <summary>
        /// İhale İlan Bilgi Sektör Ad (KİK'e ait bilgi)
        /// </summary>
        public string sektorAdi;
        /// <summary>
        /// İhale No (KİK'e ait bilgi)
        /// </summary>
        public string ihaleNo;
        /// <summary>
        /// Yaklaşık Maliyet Aralığı Kod (KİK'e ait bilgi)
        /// </summary>
        public int ymaKod;
        /// <summary>
        /// Yaklaşık Maliyet Aralığı Başlangıç Tarih
        /// </summary>
        public TNSDateTime ymaBaslangicTarihi;
        /// <summary>
        /// Yaklaşık Maliyet Aralığı Bitiş Tarih
        /// </summary>
        public TNSDateTime ymaBitisTarihi;
        /// <summary>
        /// Yaklaşık Maliyet Aralığı Açıklama
        /// </summary>
        public string ymaAciklama;
        /// <summary>
        /// Ön Yeterlik Davet Tür
        /// </summary>
        public int onYeterlikDavetTuru;
        /// <summary>
        /// Ön Yeterlik Davet Sayı
        /// </summary>
        public int onYeterlikDavetSayi;

        /// <summary>
        /// <see cref="IhaleIlanBilgisi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleIlanBilgisi()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            islemKod = 0;
            parcaKod = 0;
            tarih = new TNSDateTime();
            teslimYeri = string.Empty;
            teslimTarihi = string.Empty;
            ihaleYeri = string.Empty;
            ihaleTarihi = new TNSDateTime();
            ihaleSaati = string.Empty;
            ihaleTeminYeri = string.Empty;
            teklifSuresi = 0;
            isSuresi = 0;
            teklifTuru = 0;
            teklifBelirlemeTuru = 1;
            katilimTuru = 2;
            indirimOrani = 0;
            konsorsiyum = 2;
            ilanVerilecekmi = false;
            ilanAdedi = 0;
            ilanSekli = 0;
            kaynakTuru = 0;
            kismiTeklifeAcikMi = 0;
            sektorKodu = string.Empty;
            sektorAdi = string.Empty;
            ihaleNo = string.Empty;
            ymaKod = 0;
            ymaBaslangicTarihi = new TNSDateTime();
            ymaBitisTarihi = new TNSDateTime();
            ymaAciklama = string.Empty;
            onYeterlikDavetTuru = 0;
            onYeterlikDavetSayi = 0;
        }
    }

    /// <summary>
    /// İhale İlana ait Belge bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleIlanBilgisiBelge
    {
        /// <summary>
        /// İhale İlan Bilgisi Belge Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale İlan Bilgisi Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale İlan Bilgisi Belge Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale İlan Bilgisi Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İhale İlan Bilgisi Belge Tur
        /// </summary>
        public int belgeTur;
        /// <summary>
        /// İhale İlan Bilgisi Belge Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// İhale İlan Bilgisi Belge Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// İhale İlan Bilgisi Belge Açıklama
        /// </summary>
        public string aciklama;

        /// <summary>
        /// <see cref="IhaleIlanBilgisiBelge"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleIlanBilgisiBelge()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            belgeTur = 0;
            siraNo = 0;
            adi = string.Empty;
            aciklama = string.Empty;
        }
    }

    /// <summary>
    /// Yardım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Yardim
    {
        /// <summary>
        /// Yardım yazılan sayfa bilgisi
        /// </summary>
        public string sayfa;
        /// <summary>
        /// Yardım Tur
        /// </summary>
        public int tur;
        /// <summary>
        /// Yardım Metni
        /// </summary>
        public StringBuilder metin;
        /// <summary>
        /// Yardım Metni Parça Sıra No
        /// </summary>
        public int sira;
        /// <summary>
        /// Yardım Metni Parça
        /// </summary>
        public string metinParca;

        /// <summary>
        /// <see cref="Yardim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Yardim()
        {
            sayfa = string.Empty;
            tur = 0;
            sira = 0;
            metin = new StringBuilder();
            metinParca = string.Empty;
        }
    }

    /// <summary>
    /// Bütçe Gider bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class ButceGider
    {
        /// <summary>
        /// Bütçe Gider Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Bütçe Gider Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Bütçe Gider Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Bütçe Gider Kurumsal Kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Bütçe Gider Fonksiyonel Kod
        /// </summary>
        public string fon;
        /// <summary>
        /// Bütçe Gider Finansman Kod
        /// </summary>
        public string fin;
        /// <summary>
        /// Bütçe Gider Ekonomik Kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Bütçe Gider Alt Faaliyet Kod
        /// </summary>
        public string peb;
        /// <summary>
        /// Bütçe Gider Kanun
        /// </summary>
        public double kanun;
        /// <summary>
        /// Bütçe Gider Kanuni Başlangıç Ödeneği
        /// </summary>
        public double kbo;
        /// <summary>
        /// Bütçe Gider Eklenen Tutar
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Bütçe Gider Düşülen Tutar
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Bütçe Gider Serbest Tutar
        /// </summary>
        public double serbest;
        /// <summary>
        /// Bütçe Gider Yüzde 10 Tutar
        /// </summary>
        public double yuzde10;
        /// <summary>
        /// Bütçe Gider Ödenek Gönderme Tutar
        /// </summary>
        public double odenekGonderme;
        /// <summary>
        /// Bütçe Gider Avans Tutar
        /// </summary>
        public double avans;
        /// <summary>
        /// Bütçe Gider Bloke Tutar
        /// </summary>
        public double bloke;

        /// <summary>
        /// <see cref="ButceGider"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceGider()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            kur = string.Empty;
            fon = string.Empty;
            fin = string.Empty;
            eko = string.Empty;
            peb = string.Empty;
            kanun = 0.0;
            kbo = 0.0;
            eklenen = 0.0;
            dusulen = 0.0;
            serbest = 0.0;
            yuzde10 = 0.0;
            odenekGonderme = 0.0;
            avans = 0.0;
            bloke = 0.0;
        }
    }

    /// <summary>
    /// Bütçe Harcama bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class ButceHarcama
    {
        /// <summary>
        /// Bütçe Harcama Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Bütçe Harcama Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Bütçe Harcama Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Bütçe Harcama Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Bütçe Harcama Kurumsal Kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Bütçe Harcama Fonksiyonel Kod
        /// </summary>
        public string fon;
        /// <summary>
        /// Bütçe Harcama Finanasman Kod
        /// </summary>
        public string fin;
        /// <summary>
        /// Bütçe Harcama Ekonomik Kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Bütçe Harcama Tutarı
        /// </summary>
        public double harcama;

        /// <summary>
        /// <see cref="ButceHarcama"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ButceHarcama()
        {
            yil = 0;
            ay = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            kur = string.Empty;
            fon = string.Empty;
            fin = string.Empty;
            eko = string.Empty;
            harcama = 0.0;
        }
    }

    /// <summary>
    /// İdare Bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IdareBilgi
    {
        /// <summary>
        /// İdare Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İdare Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İdare Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// İdare Adres
        /// </summary>
        public string adres;
        /// <summary>
        /// İdare Telefon
        /// </summary>
        public string telefon;
        /// <summary>
        /// İdare Faks
        /// </summary>
        public string faks;
        /// <summary>
        /// İdare E-Posta
        /// </summary>
        public string ePosta;
        /// <summary>
        /// İdare Antet
        /// </summary>
        public string antet;
        /// <summary>
        /// İdare Evrak Seri
        /// </summary>
        public string evrakSeri;
        /// <summary>
        /// İdare Büyükşehirde Mi
        /// </summary>
        public int buyuksehirMi;
        /// <summary>
        /// İdare Arge Mi
        /// </summary>
        public int argeMi;
        /// <summary>
        /// İdare Sicil No
        /// </summary>
        public string sicilNo;
        /// <summary>
        /// İdare Vergi Kimlik No
        /// </summary>
        public string vNo;
        /// <summary>
        /// İdarenin Bağlı Olduğu Vergi Dairesi
        /// </summary>
        public string vd;

        /// <summary>
        /// İşyeri güvenliği nace kodu
        /// </summary>
        public string IGNaceKod;

        /// <summary>
        /// IBAN no
        /// </summary>
        public string iban;

        public string vergiAdi;

        /// <summary>
        /// <see cref="IdareBilgi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IdareBilgi()
        {
            muhasebe = 0;
            birim = string.Empty;
            adi = string.Empty;
            adres = string.Empty;
            telefon = string.Empty;
            faks = string.Empty;
            ePosta = string.Empty;
            antet = string.Empty;
            evrakSeri = string.Empty;
            buyuksehirMi = 0;
            argeMi = 0;
            sicilNo = string.Empty;
            vNo = string.Empty;
            vd = string.Empty;
            IGNaceKod = string.Empty;
            iban = string.Empty;
            vergiAdi = string.Empty;
        }

        public string IGNaceAd;

        public string NaceBul()
        {
            return IdareBilgi.NaceBul(sicilNo);
        }

        public static string NaceBul(string sicilNo)
        {
            if (!string.IsNullOrWhiteSpace(sicilNo))
            {
                string tut = sicilNo.Replace(" ", "").Replace(".", "").Replace("-", "");
                if (tut.Length > 4)
                {
                    return tut.Substring(1, 4);
                }
            }
            return "";
        }
    }

    /// <summary>
    /// Harcama Pusulası bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class HarcamaPusulasi
    {
        /// <summary>
        /// Harcama Pusulası Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Harcama Pusulası Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Harcama Pusulası Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Harcama Pusulası Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Harcama Pusulası No
        /// </summary>
        public int prHarcamaPusulasiNo;
        /// <summary>
        /// Harcama Pusulası Çeşit
        /// </summary>
        public string cesit;
        /// <summary>
        /// Harcama Pusulası Miktar
        /// </summary>
        public double miktar;
        /// <summary>
        /// Harcama Pusulası Birim Fiyat
        /// </summary>
        public double birimFiyat;
        /// <summary>
        /// Harcama Pusulası Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Harcama Pusulası Firma Vergi No
        /// </summary>
        public string firmaVergiNo;
        /// <summary>
        /// Firma Unvan / Kisi Ad Soyad
        /// </summary>
        public string firmaKisiAdiSoyadi;
        /// <summary>
        /// Firma Unvan / Kisi Adres
        /// </summary>
        public string firmaKisiAdresi;
        /// <summary>
        /// Satın Alan Kişi Ad Soyad
        /// </summary>
        public string satinAlanAdiSoyadi;
        /// <summary>
        /// Satın Alan Kişi Unvan
        /// </summary>
        public string satinAlanUnvani;
        //public int siraNo;

        /// <summary>
        /// <see cref="HarcamaPusulasi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HarcamaPusulasi()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            prHarcamaPusulasiNo = 0;
            cesit = string.Empty;
            miktar = 0.0;
            birimFiyat = 0.0;
            aciklama = string.Empty;
            firmaVergiNo = string.Empty;
            firmaKisiAdiSoyadi = string.Empty;
            firmaKisiAdresi = string.Empty;
            satinAlanAdiSoyadi = string.Empty;
            satinAlanUnvani = string.Empty;
            //siraNo = 0;
        }
    }

    /// <summary>
    /// İhale Numarası bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleNumarasi
    {
        /// <summary>
        /// İhale Numarası Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale Numarası Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale Numarası Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale Numarası Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İhale Numarası
        /// </summary>
        public string ihaleNo;


        /// <summary>
        /// <see cref="IhaleNumarasi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleNumarasi()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            ihaleNo = string.Empty;
        }
    }

    /// <summary>
    /// Harcama Talimatı bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class HarcamaTalimati
    {
        /// <summary>
        /// Harcama Talimatı Evrak Tarih
        /// </summary>
        public TNSDateTime evrakTarihi;
        /// <summary>
        /// Harcama Talimatı Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Harcama Talimatı Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Harcama Talimatı Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Harcama Talimatı Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Harcama Makamı Bilgisi
        /// </summary>
        public string harcamaMakami;
        /// <summary>
        /// Harcama Birimi Bilgisi
        /// </summary>
        public string harcamaBirimi;
        /// <summary>
        /// Harcama Gerekçe
        /// </summary>
        public string gerekcesi;
        /// <summary>
        /// Harcama Konu
        /// </summary>
        public string konusu;
        /// <summary>
        /// Harcama Miktar Açıklama
        /// </summary>
        public string miktarAciklama;
        /// <summary>
        /// Gerçekleştirme Süresi
        /// </summary>
        public string gerceklestirmeSuresi;
        /// <summary>
        /// Gerçekleştirme Usulü
        /// </summary>
        public string gerceklestirmeUsulu;
        /// <summary>
        /// Harcama Yaklaşık Bedel
        /// </summary>
        public double yaklasikBedel;
        /// <summary>
        /// Harcama Bütçe Tertip
        /// </summary>
        public string butceTertip;
        /// <summary>
        /// Ödenek Tutar
        /// </summary>
        public double odenekTutar;
        /// <summary>
        /// Evrak Seri
        /// </summary>
        public string evrakSeri;
        /// <summary>
        /// Evrak Sayı
        /// </summary>
        public string evrakSayi;
        /// <summary>
        /// Gerçekleştirme Görevli Ad Soyad
        /// </summary>
        public string gerceklestirmeGorevli;
        /// <summary>
        /// Harcama Talimatı Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Teklif Eden Yetkili Ad Soyad
        /// </summary>
        public string teklifEdenYetkili;
        /// <summary>
        /// Teklif Eden Yetkili Unvan
        /// </summary>
        public string teklifEdenYetkiliUnvan;
        /// <summary>
        /// Harcama Yetkilisi Ad Soyad
        /// </summary>
        public string harcamaYetkilisi;
        /// <summary>
        /// Harcama Yetkilisi Unvan
        /// </summary>
        public string harcamaYetkilisiUnvan;
        /// <summary>
        /// Harcama Talimatı Onay Tarih
        /// </summary>
        public TNSDateTime onayTarihi;
        /// <summary>
        /// Avans Var Mı
        /// </summary>
        public int avansVarmi;
        /// <summary>
        /// Mihmandar Var Mı
        /// </summary>
        public int mihmandarlikVarmi;
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string perTcNo;
        /// <summary>
        /// Personel Ad Soyad
        /// </summary>
        public string perAdSoyad;
        /// <summary>
        /// Personel Banka
        /// </summary>
        public string perBankaAd;
        /// <summary>
        /// Personel Hesap No
        /// </summary>
        public string perHesapNo;
        /// <summary>
        /// Personel Vergi Daire
        /// </summary>
        public string perVergiDairesi;
        /// <summary>
        /// Personel Unvan
        /// </summary>
        public string perUnvan;

        /// <summary>
        /// <see cref="HarcamaTalimati"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HarcamaTalimati()
        {
            evrakTarihi = new TNSDateTime();
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            avansVarmi = 1;
            mihmandarlikVarmi = 1;
            harcamaMakami = string.Empty;
            harcamaBirimi = string.Empty;
            gerekcesi = string.Empty;
            konusu = string.Empty;
            miktarAciklama = string.Empty;
            gerceklestirmeSuresi = string.Empty;
            gerceklestirmeUsulu = string.Empty;
            yaklasikBedel = 0.0;
            butceTertip = string.Empty;
            odenekTutar = 0.0;
            evrakSeri = string.Empty;
            evrakSayi = string.Empty;
            gerceklestirmeGorevli = string.Empty;
            aciklama = string.Empty;
            teklifEdenYetkili = string.Empty;
            teklifEdenYetkiliUnvan = string.Empty;
            harcamaYetkilisi = string.Empty;
            harcamaYetkilisiUnvan = string.Empty;
            onayTarihi = new TNSDateTime();
            perTcNo = string.Empty;
            perAdSoyad = string.Empty;
            perBankaAd = string.Empty;
            perHesapNo = string.Empty;
            perVergiDairesi = string.Empty;
            perUnvan = string.Empty;
        }
    }

    /// <summary>
    /// Hesap Cetveli bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class HesapCetveli
    {
        /// <summary>
        /// Hesap Cetveli Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Hesap Cetveli Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Hesap Cetveli Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Hesap Cetveli Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Programatik Fatura No
        /// </summary>
        public int pRFaturaNo;
        /// <summary>
        /// Fatura Detay Tip
        /// </summary>
        public int faturaDetayTip;
        /// <summary>
        /// Hesap Cetveli fatura bilgisini <see cref="FaturaForm"/> nesnesi olarak tutan özellik
        /// </summary>
        public FaturaForm faturaBaslik;
        /// <summary>
        /// Harcama Pusulası Detay Sıra No
        /// </summary>
        public int hPDetaySiraNo;
        /// <summary>
        /// Hesap Cetveli Satır Sayı
        /// </summary>
        public int hCetvelSatirSayi;

        /// <summary>
        /// <see cref="HesapCetveli"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public HesapCetveli()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            pRFaturaNo = 0;
            faturaDetayTip = 0;
            faturaBaslik = new FaturaForm();
            hPDetaySiraNo = 0;
            hCetvelSatirSayi = 0;
        }
    }

    /// <summary>
    /// İhale Dosyasında bulunan belgeye ait bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleDosyasiIcerik
    {
        /// <summary>
        /// İş Tür
        /// </summary>
        public int isTuru;
        /// <summary>
        /// İhale Usul
        /// </summary>
        public int ihaleUsul;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int sira;
        /// <summary>
        /// Belge Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// Belge Dosya Ad
        /// </summary>
        public string dosyaAdi;

        /// <summary>
        /// <see cref="IhaleDosyasiIcerik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleDosyasiIcerik()
        {
            isTuru = 0;
            ihaleUsul = 0;
            sira = 0;
            adi = string.Empty;
            dosyaAdi = string.Empty;
        }
    }

    /// <summary>
    /// İhale Dosyasında bulunan belgeleri tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleDosyasiIcerikListe
    {
        /// <summary>
        /// İçerik Liste Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İçerik Liste Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İçerik Liste Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İçerik Liste Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Belgeleri <see cref="IhaleDosyasiIcerik"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray liste;

        /// <summary>
        /// Initializes a new instance of the <see cref="IhaleDosyasiIcerikListe"/> class.
        /// </summary>
        public IhaleDosyasiIcerikListe()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            liste = new ObjectArray();

        }
    }

    /// <summary>
    /// İhale Teklif Dosyası işlemlerine ait bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleTeklifDosyasi
    {
        /// <summary>
        /// İhale Teklif Dosyası Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale Teklif Dosyası Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale Teklif Dosyası Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale Teklif Dosyası Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İhale Teklif Dosyası Satın Alma Sıra No
        /// </summary>
        public int alindiSiraNo;
        /// <summary>
        /// İhale Teklif Dosyası Satın Alan Firma Kod
        /// </summary>
        public int firmaKodu;
        /// <summary>
        /// İhale Teklif Dosyası Satın Alan Firma Ad
        /// </summary>
        public string firmaAdi;
        /// <summary>
        /// İhale Teklif Dosyası Satın Alma Tarih
        /// </summary>
        public TNSDateTime alindiBelgeTarihi;
        /// <summary>
        /// İhale Teklif Dosyası Satın Alma Saat
        /// </summary>
        public string alindiBelgeSaati;
        /// <summary>
        /// İhale Teklif Dosyası Satış Belge Tarih
        /// </summary>
        public TNSDateTime satisBelgeTarihi;
        /// <summary>
        /// İhale Teklif Dosyası Satış Belge No
        /// </summary>
        public int satisBelgeNo;
        /// <summary>
        /// İhale Teklif Dosyası Firma Teslim Sıra No
        /// </summary>
        public int teslimSiraNo;
        /// <summary>
        /// İhale Teklif Dosyası Firma Teslim Tarih
        /// </summary>
        public TNSDateTime teslimTarihi;
        /// <summary>
        /// İhale Teklif Dosyası Firma Teslim Saat
        /// </summary>
        public string teslimSaati;
        /// <summary>
        /// İhale Teklif Dosyası Geliş Şekli
        /// </summary>
        public int gelisSekli;
        /// <summary>
        /// İhale Teklif Dosyası Postada Gecikti Mi
        /// </summary>
        public int postadaGeciktimi;
        /// <summary>
        /// İhale Teklif Dosyası Verildi Mi
        /// </summary>
        public int teklifDosyasiVerdimi;
        /// <summary>
        /// İhale Teklif Dosyası Firma Adına Satın Alan Kisi Ad Soyad
        /// </summary>
        public string satinAlanKisi;
        /// <summary>
        /// İhale Teklif Dosyası bilgileri silinsin mi
        /// </summary>
        public bool sil;
        /// <summary>
        /// Firma İhale Devam Etsin Mi
        /// </summary>
        public int firmaDevam;
        /// <summary>
        /// Firma İhale için Yeterli Mi
        /// </summary>
        public int firmaYeterli;
        /// <summary>
        /// Firma Devam Gerekçe
        /// </summary>
        public string gerekce;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;
        /// <summary>
        /// Belge Gidiş Şekli
        /// </summary>
        public int belgeGidisSekli;
        /// <summary>
        /// Belge Gidiş Tarih
        /// </summary>
        public TNSDateTime belgeGidisTarihi;

        /// <summary>
        /// <see cref="IhaleTeklifDosyasi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleTeklifDosyasi()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            alindiSiraNo = 0;
            firmaKodu = 0;
            firmaAdi = string.Empty;
            alindiBelgeTarihi = new TNSDateTime();
            alindiBelgeSaati = string.Empty;
            satisBelgeTarihi = new TNSDateTime();
            satisBelgeNo = 0;
            teslimSiraNo = 0;
            teslimTarihi = new TNSDateTime();
            teslimSaati = string.Empty;
            gelisSekli = 0;
            postadaGeciktimi = 0;
            teklifDosyasiVerdimi = 0;
            satinAlanKisi = string.Empty;
            sil = false;
            firmaDevam = 0;
            firmaYeterli = 0;
            gerekce = string.Empty;
            ihaleAsamasi = 0;
            belgeGidisSekli = 0;
            belgeGidisTarihi = new TNSDateTime();

        }
    }

    /// <summary>
    /// Teklif Dosyası Komisyon Tutanağına ait bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifDosyasiKomisyonTutanagi
    {
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanak Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanak Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanak Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanak Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanak Tarih
        /// </summary>
        public TNSDateTime tutanakTarihi;
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanak Saat
        /// </summary>
        public string tutanakSaati;
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanağı Teslim Eden Ad Soyad
        /// </summary>
        public string teslimEden;
        /// <summary>
        /// Teklif Dosyası Komisyon Tutanağı Teslim Eden Unvan
        /// </summary>
        public string teslimEdenUnvan;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeklifDosyasiKomisyonTutanagi"/> class.
        /// </summary>
        public TeklifDosyasiKomisyonTutanagi()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tutanakTarihi = new TNSDateTime();
            tutanakSaati = string.Empty;
            teslimEden = string.Empty;
            teslimEdenUnvan = string.Empty;
            ihaleAsamasi = 0;
        }
    }

    /// <summary>
    /// Teklif Dosyası İçerik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifDosyasiIcerik
    {
        /// <summary>
        /// İş Tür
        /// </summary>
        public int isTuru;
        /// <summary>
        /// İhale Usul
        /// </summary>
        public int ihaleUsul;
        /// <summary>
        /// Teklif Dosyası İçerik Sıra No
        /// </summary>
        public int sira;
        /// <summary>
        /// Belge Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// Dosya Ad
        /// </summary>
        public string dosyaAdi;
        /// <summary>
        /// İhale Aşama
        /// </summary>
        public int ihaleAsamasi;
        /// <summary>
        /// Belge Değerlendirme Kriter
        /// </summary>
        public int degKriteri;

        /// <summary>
        /// <see cref="TeklifDosyasiIcerik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifDosyasiIcerik()
        {
            isTuru = 0;
            ihaleUsul = 0;
            sira = 0;
            adi = string.Empty;
            dosyaAdi = string.Empty;
            ihaleAsamasi = 0;
            degKriteri = 0;
        }
    }

    /// <summary>
    /// Teklif Dosyasında bulunan belgeleri tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifDosyasiIcerikListe
    {
        /// <summary>
        /// Teklif Dosya Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Dosya Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Dosya Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Dosya Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif dosyasındaki belgeleri <see cref="TeklifDosyasiIcerik"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray liste;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;


        /// <summary>
        /// Initializes a new instance of the <see cref="TeklifDosyasiIcerikListe"/> class.
        /// </summary>
        public TeklifDosyasiIcerikListe()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            liste = new ObjectArray();
            ihaleAsamasi = 0;
        }
    }

    /// <summary>
    /// Teklif Zarf Biçimi bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifZarfBicimi
    {
        /// <summary>
        /// Teklif Zarf Biçim Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Teklif Zarf Biçim Ad
        /// </summary>
        public string adi;

        /// <summary>
        /// <see cref="TeklifZarfBicimi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifZarfBicimi()
        {
            kod = 0;
            adi = string.Empty;
        }
    }

    /// <summary>
    /// Teklif Zarf Kontrol bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifZarfKontroluBaslik
    {
        /// <summary>
        /// Teklif Zarf Kontrol Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Zarf Kontrol Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Zarf Kontrol Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Zarf Kontrol Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Zarf Kontrol Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Teklif Zarf Kontrol Saat
        /// </summary>
        public string saat;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;
        /// <summary>
        /// Teklif Zarf Kontrol bilgilerini <see cref="TeklifZarfKontroluSatir"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray satirlar;

        /// <summary>
        /// <see cref="TeklifZarfKontroluBaslik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifZarfKontroluBaslik()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tarih = new TNSDateTime();
            saat = string.Empty;
            ihaleAsamasi = 0;
            satirlar = new ObjectArray();
        }
    }

    /// <summary>
    /// Teklif Zarf Kontrol Satır bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifZarfKontroluSatir
    {
        /// <summary>
        /// Teklif Zarf Sahibi Firma Kod
        /// </summary>
        public int firmaKodu;
        /// <summary>
        /// Teklif Zarf Sahibi Firma Ad
        /// </summary>
        public string firmaAdi;
        /// <summary>
        /// Teklif Zarfı Uygun Mu
        /// </summary>
        public int uygunmu;
        /// <summary>
        /// Teklif Zarfı Değerlendirme
        /// </summary>
        public string degerlendirme;

        /// <summary>
        /// <see cref="TeklifZarfKontroluSatir"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifZarfKontroluSatir()
        {
            firmaKodu = 0;
            firmaAdi = string.Empty;
            uygunmu = 0;
            degerlendirme = string.Empty;
        }
    }

    /// <summary>
    /// Belge Zamanı bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class BelgeZamani
    {
        /// <summary>
        /// Belge Zamanı Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Belge Zamanı Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Belge Zamanı Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Belge Zamanı Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Belge Zamanı Belge Kod
        /// </summary>
        public int belgeKod;
        /// <summary>
        /// Belge Zamanı Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Belge Zamanı Saat
        /// </summary>
        public string saat;

        /// <summary>
        /// <see cref="BelgeZamani"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public BelgeZamani()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            belgeKod = 0;
            tarih = new TNSDateTime();
            saat = string.Empty;
        }
    }

    /// <summary>
    /// Teklif Dosyası Eksik Belge bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifDosyasiEksikBelge
    {
        /// <summary>
        /// Teklif Dosyası Eksik Belge Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Saat
        /// </summary>
        public string saat;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Tutanak Ad
        /// </summary>
        public string tutanakAd;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Sahibi Firma Kod
        /// </summary>
        public int firmaKod;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Sahibi Firma Ad
        /// </summary>
        public string firmaAd;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Kod
        /// </summary>
        public int belgeKod;
        /// <summary>
        /// Teklif Dosyası Eksik Belge Ad
        /// </summary>
        public string belgeAd;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;


        /// <summary>
        /// <see cref="TeklifDosyasiEksikBelge"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifDosyasiEksikBelge()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tarih = new TNSDateTime();
            saat = string.Empty;
            tutanakAd = string.Empty;
            firmaKod = 0;
            firmaAd = string.Empty;
            belgeKod = 0;
            belgeAd = string.Empty;
            ihaleAsamasi = 0;
        }
    }

    /// <summary>
    /// Teklife Uygun Olmayan Belgeye ait bilgiyi tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifUygunOlmayanBelge
    {
        /// <summary>
        /// Teklif Uygun Olmayan Belge Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Uygun Olmayan Belge Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Uygun Olmayan Belge Birim
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Uygun Olmayan Belge Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Uygun Olmayan Belge İşlem Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Teklif Uygun Olmayan Belge İşlem Saat
        /// </summary>
        public string saat;
        /// <summary>
        /// Teklif Uygun Olmayan Belge sahibi Firma Kod
        /// </summary>
        public int firmaKod;
        /// <summary>
        /// Teklif Uygun Olmayan Belge sahibi Firma Ad
        /// </summary>
        public string firmaAd;
        /// <summary>
        /// Teklif Uygun Olmayan Belge Kod
        /// </summary>
        public int belgeKod;
        /// <summary>
        /// Teklif Uygun Olmayan Belge Ad
        /// </summary>
        public string belgeAd;
        /// <summary>
        /// İşlem Gerekçe
        /// </summary>
        public string gerekce;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;

        /// <summary>
        /// <see cref="TeklifUygunOlmayanBelge"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifUygunOlmayanBelge()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tarih = new TNSDateTime();
            saat = string.Empty;
            firmaKod = 0;
            firmaAd = string.Empty;
            belgeKod = 0;
            belgeAd = string.Empty;
            gerekce = string.Empty;
            ihaleAsamasi = 0;
        }
    }

    /// <summary>
    /// İhale için firmalara ait teklif edilen fiyat bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TeklifEdilenFiyat
    {
        /// <summary>
        /// Teklif Edilen Fiyat Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Teklif Edilen Fiyat Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Teklif Edilen Fiyat Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Teklif Edilen Fiyat Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Teklif Edilen Fiyat İşlem Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Teklif Edilen Fiyat İşlem Saat
        /// </summary>
        public string saat;
        /// <summary>
        /// Teklif Edilen Fiyat sahibi Firma Kod
        /// </summary>
        public int firmaKod;
        /// <summary>
        /// Teklif Edilen Fiyat sahibi Firma Ad
        /// </summary>
        public string firmaAd;
        /// <summary>
        /// Firma Teklif Tutarı
        /// </summary>
        public double teklifTutari;
        /// <summary>
        /// Firma Fiyat Dışı Unsur Teklif Tutarı
        /// </summary>
        public double fiyatDisiTeklifTutari;
        /// <summary>
        /// Yerli Firma için Yerli Malı İndirimli Teklif Tutarı
        /// </summary>
        public double yerliMaliTeklifTutari;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;
        /// <summary>
        /// Teklif Tür
        /// </summary>
        public int teklifTur;
        /// <summary>
        /// Teklif sonucu kazanan firma bilgisi
        /// </summary>
        public FirmaBilgisi kazananFirma;

        /// <summary>
        /// <see cref="TeklifEdilenFiyat"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TeklifEdilenFiyat()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tarih = new TNSDateTime();
            saat = string.Empty;
            firmaKod = 0;
            firmaAd = string.Empty;
            teklifTutari = 0.0;
            fiyatDisiTeklifTutari = 0.0;
            yerliMaliTeklifTutari = 0.0;
            ihaleAsamasi = 0;
            teklifTur = 0;
            kazananFirma = new FirmaBilgisi();
        }

        public TeklifEdilenFiyat(int yil, int muhasebe, string birim, string surecNo)
        {
            this.yil = yil;
            this.muhasebe = muhasebe;
            this.birim = birim;
            this.surecNo = surecNo;
            tarih = new TNSDateTime();
            saat = string.Empty;
            firmaKod = 0;
            firmaAd = string.Empty;
            teklifTutari = 0.0;
            fiyatDisiTeklifTutari = 0.0;
            yerliMaliTeklifTutari = 0.0;
            ihaleAsamasi = 0;
            teklifTur = 0;
            kazananFirma = new FirmaBilgisi();
        }
    }

    /// <summary>
    /// İhale Kazanan Firma bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class KazananFirma
    {
        /// <summary>
        /// Kazanan Firma Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kazanan Firma Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Kazanan Firma Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Kazanan Firma Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// 1. olarak Kazanan Firma Kod
        /// </summary>
        public int firmaKod1;
        /// <summary>
        /// 1. olarak Kazanan Firma Ad
        /// </summary>
        public string firmaAd1;
        /// <summary>
        /// 1. olarak Kazanan Firma Vergi No
        /// </summary>
        public string firmaVergiNo1;
        /// <summary>
        /// 2. olarak Kazanan Firma Kod
        /// </summary>
        public int firmaKod2;
        /// <summary>
        /// 2. olarak Kazanan Firma Ad
        /// </summary>
        public string firmaAd2;
        /// <summary>
        /// 2. olarak Kazanan Firma Vergi No
        /// </summary>
        public string firmaVergiNo2;

        /// <summary>
        /// <see cref="KazananFirma"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KazananFirma()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            firmaKod1 = 0;
            firmaAd1 = string.Empty;
            firmaVergiNo1 = string.Empty;
            firmaKod2 = 0;
            firmaAd2 = string.Empty;
            firmaVergiNo2 = string.Empty;
        }

        public KazananFirma(int yil, int muhasebe, string birim, string surecNo)
        {
            this.yil = yil;
            this.muhasebe = muhasebe;
            this.birim = birim;
            this.surecNo = surecNo;
            firmaKod1 = 0;
            firmaAd1 = string.Empty;
            firmaVergiNo1 = string.Empty;
            firmaKod2 = 0;
            firmaAd2 = string.Empty;
            firmaVergiNo2 = string.Empty;
        }
    }

    /// <summary>
    /// İhale Komisyon Kararına ait bilgiyi tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleKarari//Komisyon Karari
    {
        /// <summary>
        /// İhale Komisyon Kararı Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale Komisyon Kararı Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale Komisyon Kararı Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale Komisyon Kararı Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İhale Komisyon Kararı Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// İhale Komisyon Kararı Saat
        /// </summary>
        public string saat;
        /// <summary>
        /// İhale Komisyon Kararı Gerekçe
        /// </summary>
        public string gerekce;

        /// <summary>
        /// <see cref="IhaleKarari"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleKarari()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tarih = new TNSDateTime();
            saat = string.Empty;
            gerekce = string.Empty;
        }
    }

    /// <summary>
    /// Firma için İhale Sonucunun onaylanması ve Firmaya gönderilmesine ait bilgiyi tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleFirmaSonucu
    {
        /// <summary>
        /// İhale Firma Sonucu Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale Firma Sonucu Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale Firma Sonucu Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale Firma Sonucu Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İlgili Firma Kod
        /// </summary>
        public int firmaKod;
        /// <summary>
        /// İlgili Firma Ad
        /// </summary>
        public string firmaAd;
        /// <summary>
        /// Belge Onay Tarih
        /// </summary>
        public TNSDateTime onayTarihi;
        /// <summary>
        /// Belgenin Firmaya Gidiş Tarihi
        /// </summary>
        public TNSDateTime gidisTarihi;
        /// <summary>
        /// Belgenin Firmaya Gidiş Şekli
        /// </summary>
        public int gidisSekli;
        /// <summary>
        /// Gerekçe
        /// </summary>
        public string gerekce;
        /// <summary>
        /// Teklif Tür
        /// </summary>
        public int teklifTur;

        /// <summary>
        /// <see cref="IhaleFirmaSonucu"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleFirmaSonucu()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            firmaKod = 0;
            firmaAd = string.Empty;
            onayTarihi = new TNSDateTime();
            gidisTarihi = new TNSDateTime();
            gidisSekli = 0;
            gerekce = string.Empty;
            teklifTur = 0;
        }
    }

    /// <summary>
    /// İhaleyi Kazanan Firmayı Sözleşmeye Davet etmek için kullanılanan bilgiyi tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleSozlesmeyeDavet
    {
        /// <summary>
        /// İhale Sözleşmeye Davet Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale Sözleşmeye Davet Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale Sözleşmeye Davet Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale Sözleşmeye Davet Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Sözleşmeye Davet Edilen Firma Kod
        /// </summary>
        public int firmaKod;
        /// <summary>
        /// Sözleşmeye Davet Edilen Firma Ad
        /// </summary>
        public string firmaAd;
        /// <summary>
        /// Davet Yazısı Gidiş Tarihi
        /// </summary>
        public TNSDateTime gidisTarihi;
        /// <summary>
        /// Davet Yazısı Gidiş Şekli
        /// </summary>
        public int gidisSekli;

        /// <summary>
        /// <see cref="IhaleSozlesmeyeDavet"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleSozlesmeyeDavet()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            firmaKod = 0;
            firmaAd = string.Empty;
            gidisTarihi = new TNSDateTime();
            gidisSekli = 0;
        }
    }

    /// <summary>
    /// Sözleşme Sonucunda Oluşan Ücretlere ait bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleSozlesmeSonuUcret
    {
        /// <summary>
        /// İhale Sözleşme Sonu Ücret Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale Sözleşme Sonu Ücret Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale Sözleşme Sonu Ücret Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale Sözleşme Sonu Ücret Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// İhale Sözleşme Sonu Ücret Tutar
        /// </summary>
        public double tutar;
        /// <summary>
        /// Belge Tarih
        /// </summary>
        public TNSDateTime belgeTarihi;
        /// <summary>
        /// Belge No
        /// </summary>
        public string belgeNo;

        /// <summary>
        /// <see cref="IhaleSozlesmeSonuUcret"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleSozlesmeSonuUcret()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            siraNo = 0;
            tutar = 0;
            belgeTarihi = new TNSDateTime();
            belgeNo = string.Empty;
        }

        public IhaleSozlesmeSonuUcret(int yil, int muhasebe, string birim, string surecNo)
        {
            this.yil = yil;
            this.muhasebe = muhasebe;
            this.birim = birim;
            this.surecNo = surecNo;
            siraNo = 0;
            tutar = 0;
            belgeTarihi = new TNSDateTime();
            belgeNo = string.Empty;
        }
    }

    /// <summary>
    /// İhale Sonucuna ait bilgiyi tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleSonucu
    {
        /// <summary>
        /// İhale Sonucu Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İhale Sonucu Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İhale Sonucu Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// İhale Sonucu Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İhale Sonucu Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// İhale Sonucu Saat
        /// </summary>
        public string saat;
        /// <summary>
        /// İhale Sonucu İptal Gerekçesi
        /// </summary>
        public string gerekce;
        /// <summary>
        /// İhale Sonucu 
        /// </summary>
        public int sonuc;

        /// <summary>
        /// <see cref="IhaleSonucu"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleSonucu()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tarih = new TNSDateTime();
            saat = string.Empty;
            gerekce = string.Empty;
            sonuc = 0;
        }
    }


    /// <summary>
    /// Süreç Firma Bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FirmaAnahtari
    {
        /// <summary>
        /// Firma Anahtarı Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Firma Anahtarı Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Firma Anahtarı Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Firma Anahtarı Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Firma Anahtarı Firma Kod
        /// </summary>
        public int firmaKodu;
        /// <summary>
        /// İhale Aşaması
        /// </summary>
        public int ihaleAsamasi;

        /// <summary>
        /// <see cref="FirmaAnahtari"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FirmaAnahtari()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            firmaKodu = 0;
            ihaleAsamasi = 0;
        }
    }

    /// <summary>
    /// Sözleşmeli Maaş Bordro bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLSozlesmeliMaasBordro
    {
        /// <summary>
        /// Sözleşmeli Maaş Bordro Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Sözleşmeli Maaş Bordro Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Sözleşmeli Maaş Bordro Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Sözleşmeli Maaş Bordro Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Sözleşmeli Maaş Bordro Maaş Yıl
        /// </summary>
        public int maasYil;
        /// <summary>
        /// Sözleşmeli Maaş Bordro Maaş Ay
        /// </summary>
        public int maasAy;
        /// <summary>
        /// Maaş sahibi Personel TC Kimlik No
        /// </summary>
        public string tCKimlikNo;
        /// <summary>
        /// Personelin Bordro listesindeki sırası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Maaş sahibi Personel Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Maaş sahibi Personel Soyad
        /// </summary>
        public string soyad;
        /// <summary>
        /// Maaş sahibi Personel Ay içinde kaç gün çalışmış
        /// </summary>
        public int gun;
        /// <summary>
        /// Bordro Tur
        /// </summary>
        public int bordroTur;
        /// <summary>
        /// Bordro Tip
        /// </summary>
        public int bordroTip;
        /// <summary>
        /// Maaş sahibi Personel Banka Kod
        /// </summary>
        public string bankaKodu;
        /// <summary>
        /// Maaş sahibi Personel Banka Şube Kod
        /// </summary>
        public string bankaSubeKodu;
        /// <summary>
        /// Maaş sahibi Personel Hesap No
        /// </summary>
        public string bankaHesapNo;
        /// <summary>
        /// Maaş sahibi Personel Banka Ad
        /// </summary>
        public string bankaAdi;
        /// <summary>
        /// Maaş sahibi Personel Banka Şube Ad
        /// </summary>
        public string bankaSubeAdi;

        /// <summary>
        /// Sendika Devlet Yardımı Tutar
        /// </summary>
        public decimal sendikaDevletYardimi;
        /// <summary>
        /// Sendika Aidatı Tutar
        /// </summary>
        public decimal sendikaAidati;
        /// <summary>
        /// Nafaka Tutar
        /// </summary>
        public decimal nafaka;
        /// <summary>
        /// Ücret Tutar
        /// </summary>
        public decimal ucret;
        /// <summary>
        /// Özel Sigorta Tutar
        /// </summary>
        public decimal ozelSigorta;
        /// <summary>
        /// Lojman Kira Tutar
        /// </summary>
        public decimal lojman;
        /// <summary>
        /// İcra Tutar
        /// </summary>
        public decimal icra;
        /// <summary>
        /// Diğer Kesinti Tutar
        /// </summary>
        public decimal digerKesinti;
        /// <summary>
        /// Sakatlık İndirim Tutar
        /// </summary>
        public decimal sakatlikIndirim;
        /// <summary>
        /// Ek Tazminat Tutar
        /// </summary>
        public decimal ekTazminat;
        /// <summary>
        /// Tahakkuk Toplam Tutar
        /// </summary>
        public decimal tahakkukToplam;
        /// <summary>
        /// Asıl Gelir Vergisi Tutar
        /// </summary>
        public decimal asilGelirVergisi;
        /// <summary>
        /// Asgari Geçim İndirimi Tutar
        /// </summary>
        public decimal asgGecimIndirimi;
        /// <summary>
        /// Gelir Vergisi Tutar
        /// </summary>
        public decimal gelirVergisi;
        /// <summary>
        /// Damga Vergisi Tutar
        /// </summary>
        public decimal damgaVergisi;
        /// <summary>
        /// SSK İşveren Payı Tutar
        /// </summary>
        public decimal sskIsverenPayi;
        /// <summary>
        /// SSK İşçi Payı Tutar
        /// </summary>
        public decimal sskIsciPayi;
        /// <summary>
        /// Kesinti Toplam Tutar
        /// </summary>
        public decimal kesintiToplam;
        /// <summary>
        /// Ödenmesi Gereken Tutar
        /// </summary>
        public decimal odenmesiGereken;
        /// <summary>
        /// Kümülatif Vergi Matrahi Tutar
        /// </summary>
        public decimal kumVergiMatrahi;
        /// <summary>
        /// Vergi Matrahı Tutar
        /// </summary>
        public decimal vergiMatrahi;
        /// <summary>
        /// Gelir Vergisi Oranı
        /// </summary>
        public decimal gelirVergisiOran;
        /// <summary>
        /// Damga Vergisi Oranı
        /// </summary>
        public decimal damgaVergisiOran;
        /// <summary>
        /// SSK İşveren Payı Oranı
        /// </summary>
        public decimal sskIsverenPayiOran;
        /// <summary>
        /// SSK İşçi Payı Oranı
        /// </summary>
        public decimal sskIsciPayiOran;
        /// <summary>
        /// En Yüksek Maaş Göstergesi
        /// </summary>
        public decimal maxMaasGostergesi;
        /// <summary>
        /// Aylık Katsayı
        /// </summary>
        public decimal aylikKatsayi;
        /// <summary>
        /// Brüt Asgari Ücret
        /// </summary>
        public decimal brutAsgariUcret;
        /// <summary>
        /// İşsizlik Sigorta Prim Oran
        /// </summary>
        public decimal issizSigPrimOran;
        /// <summary>
        /// Ek Tazminat Oran
        /// </summary>
        public decimal ekTazminatOran;
        /// <summary>
        /// Asgari Geçim İndirim Katsayısı
        /// </summary>
        public decimal asgGecimIndKatsayi;
        /// <summary>
        /// Aile Yardımı Toplamı
        /// </summary>
        public decimal aileYardimi;
        /// <summary>
        /// Personel Sicil No
        /// </summary>
        public string sicilNo;
        /// <summary>
        /// Personel Unvan Kod
        /// </summary>
        public int unvanKodu;
        /// <summary>
        /// Personel Unvan Ad
        /// </summary>
        public string unvanAdi;
        /// <summary>
        /// Personel Medeni Durum
        /// </summary>
        public int medeniDurumu;

        /// <summary>
        /// <see cref="PSLSozlesmeliMaasBordro"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLSozlesmeliMaasBordro()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            maasYil = 0;
            maasAy = 0;
            siraNo = 0;
            tCKimlikNo = string.Empty;
            ad = string.Empty;
            soyad = string.Empty;
            bankaKodu = string.Empty;
            bankaSubeKodu = string.Empty;
            bankaHesapNo = string.Empty;
            bankaAdi = string.Empty;
            bankaSubeAdi = string.Empty;
            gun = 0;
            bordroTur = 0;
            sendikaAidati = 0;
            sendikaDevletYardimi = 0;
            nafaka = 0;
            ucret = 0;
            ozelSigorta = 0;
            lojman = 0;
            icra = 0;
            digerKesinti = 0;
            sakatlikIndirim = 0;
            ekTazminat = 0;
            tahakkukToplam = 0;
            asilGelirVergisi = 0;
            asgGecimIndirimi = 0;
            gelirVergisi = 0;
            damgaVergisi = 0;
            sskIsverenPayi = 0;
            sskIsciPayi = 0;
            kesintiToplam = 0;
            odenmesiGereken = 0;
            kumVergiMatrahi = 0;
            vergiMatrahi = 0;
            gelirVergisiOran = 0;
            damgaVergisiOran = 0;
            sskIsverenPayiOran = 0;
            sskIsciPayiOran = 0;
            maxMaasGostergesi = 0;
            aylikKatsayi = 0;
            brutAsgariUcret = 0;
            issizSigPrimOran = 0;
            ekTazminatOran = 0;
            asgGecimIndKatsayi = 0;
            sicilNo = string.Empty;
            unvanKodu = 0;
            unvanAdi = string.Empty;
            medeniDurumu = 0;
            aileYardimi = 0;
        }
    }

    /// <summary>
    /// Öğrenci Maaş Bordro bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLOgrenciMaasBordro
    {
        /// <summary>
        /// Öğrenci Maaş Bordro Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Öğrenci Maaş Bordro Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Öğrenci Maaş Bordro Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Öğrenci Maaş Bordro Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Öğrenci Maaş Bordro Maaş Yıl
        /// </summary>
        public int maasYil;
        /// <summary>
        /// Öğrenci Maaş Bordro Maaş Ay
        /// </summary>
        public int maasAy;
        /// <summary>
        /// Öğrenci Maaş Bordro Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Öğrenci TC Kimlik No
        /// </summary>
        public string tCKimlikNo;
        /// <summary>
        /// Öğrenci Ad Soyad
        /// </summary>
        public string adiSoyadi;
        /// <summary>
        /// Öğrencinin ay içinde kaç gün çalıştığı
        /// </summary>
        public int gun;
        /// <summary>
        /// Öğrenci Banka Kod
        /// </summary>
        public string bankaKodu;
        /// <summary>
        /// Öğrenci Banka Şube Kod
        /// </summary>
        public string bankaSubeKodu;
        /// <summary>
        /// Öğrenci Banka Hesap No
        /// </summary>
        public string bankaHesapNo;
        /// <summary>
        /// Öğrenci Banka Adı
        /// </summary>
        public string bankaAdi;
        /// <summary>
        /// Öğrenci Banka Şube Adı
        /// </summary>
        public string bankaSubeAdi;
        /// <summary>
        /// Tahakkuk Eden Tutar
        /// </summary>
        public decimal tahakkukEden;
        /// <summary>
        /// Ücret
        /// </summary>
        public decimal ucret;
        /// <summary>
        /// Diğer Kesinti Tutar
        /// </summary>
        public decimal digerKesinti;
        /// <summary>
        /// Damga Vergisi Tutar
        /// </summary>
        public decimal damgaVergisi;
        /// <summary>
        /// Net Asgari Ücret Tutar
        /// </summary>
        public decimal netAsgariUcret;

        /// <summary>
        /// <see cref="PSLOgrenciMaasBordro"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLOgrenciMaasBordro()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            maasYil = 0;
            maasAy = 0;
            siraNo = 0;
            tCKimlikNo = string.Empty;
            adiSoyadi = string.Empty;
            gun = 0;
            bankaKodu = string.Empty;
            bankaSubeKodu = string.Empty;
            bankaHesapNo = string.Empty;
            bankaAdi = string.Empty;
            bankaSubeAdi = string.Empty;
            tahakkukEden = 0;
            ucret = 0;
            digerKesinti = 0;
            damgaVergisi = 0;
            netAsgariUcret = 0;
        }
    }

    /// <summary>
    /// Personel Hastalık İznine ait bilgiyi tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLHastalikIzni
    {
        /// <summary>
        /// Personel Hastalık İzni Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Personel Hastalık İzni Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Personel Hastalık İzni Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Personel Hastalık İzni Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İzin Yıl
        /// </summary>
        public int izinYil;
        /// <summary>
        /// İzin Ay
        /// </summary>
        public int izinAy;
        /// <summary>
        /// Maaş No
        /// </summary>
        public int maasNo;
        /// <summary>
        /// Say 2000 No
        /// </summary>
        public int say2000No;
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string tCKimlikNo;
        /// <summary>
        /// Personelin bu aya kadar kullandığı Hastalık İzni gün sayısı
        /// </summary>
        public int buAyaKadarKulHastIzni;
        /// <summary>
        /// Personelin bu ay kullandığı Hastalık İzni gün sayısı
        /// </summary>
        public int buAyKulHastIzni;
        /// <summary>
        /// Personelin kullandığı toplam Hastalık İzni gün sayısı
        /// </summary>
        public int toplamHastIzni;
        /// <summary>
        /// Personelden kesilmesi gereken Hastalık İzni gün sayısı
        /// </summary>
        public int kesilmesiGerekenHastIzni;
        /// <summary>
        /// Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Personelin sene boyunca kullandığı toplam izin gün sayısı
        /// </summary>
        public int senedeKulToplamIzin;

        /// <summary>
        /// <see cref="PSLHastalikIzni"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLHastalikIzni()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            izinYil = 0;
            izinAy = 0;
            maasNo = 0;
            say2000No = 0;
            tCKimlikNo = string.Empty;
            buAyaKadarKulHastIzni = 0;
            buAyKulHastIzni = 0;
            toplamHastIzni = 0;
            kesilmesiGerekenHastIzni = 0;
            adSoyad = string.Empty;
            senedeKulToplamIzin = 0;

        }
    }
    /// <summary>
    /// Birime gelen ve birimden giden personel bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLGelenGidenPersonel
    {
        /// <summary>
        /// Gelen Giden Personel Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Gelen Giden Personel Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Gelen Giden Personel Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Gelen Giden Personel Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Gelen Giden Personel Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Maaş No
        /// </summary>
        public int maasNo;
        /// <summary>
        /// Say 2000 No
        /// </summary>
        public int say2000No;
        /// <summary>
        /// Gelen Giden Personel TC Kimlik No
        /// </summary>
        public string tCKimlikNo;
        /// <summary>
        /// Gelen Giden Personel Başlama Tarihi
        /// </summary>
        public TNSDateTime baslamaTarihi;
        /// <summary>
        /// Gelen Giden Personel Görevi
        /// </summary>
        public string gorevi;
        /// <summary>
        /// Gelen Giden Personel Kadro Bilgisi
        /// </summary>
        public string kadro;
        /// <summary>
        /// Gelen Giden Personel Ad Soyad
        /// </summary>
        public string adSoyad;

        /// <summary>
        /// <see cref="PSLGelenGidenPersonel"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLGelenGidenPersonel()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            tip = 0;
            maasNo = 0;
            say2000No = 0;
            tCKimlikNo = string.Empty;
            baslamaTarihi = new TNSDateTime();
            gorevi = string.Empty;
            kadro = string.Empty;
            adSoyad = string.Empty;

        }
    }

    /// <summary>
    /// Maaş hesaplamalarında kullanılan temel bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLMaasTemelBilgisi
    {
        /// <summary>
        /// Maaş Temel Bilgisi Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Maaş Temel Bilgisi Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Taban Aylık Katsayı
        /// </summary>
        public decimal tabanAylikKatsayi;
        /// <summary>
        /// Yan Odeme Katsayı
        /// </summary>
        public decimal yanOdemeKatsayi;
        /// <summary>
        /// En Yüksek Memur Maaş Göstergesi
        /// </summary>
        public decimal enYuksMemurMaasGost;
        /// <summary>
        /// Aylık Katsayı
        /// </summary>
        public decimal aylikKatsayi;
        /// <summary>
        /// Gelir Vergisi Oranı
        /// </summary>
        public decimal gelirVergisiOran;
        /// <summary>
        /// SSK İşveren Payı Oranı
        /// </summary>
        public decimal sskIsverenPayiOran;
        /// <summary>
        /// SSK İşçi Payı Oranı
        /// </summary>
        public decimal sskIsciPayiOran;
        /// <summary>
        /// Damga Vergisi Oranı
        /// </summary>
        public decimal damgaVergisiOran;
        /// <summary>
        /// Brüt Asgari Ücret
        /// </summary>
        public decimal brutAsgariUcret;
        /// <summary>
        /// İş Kazaları ve Meslek Hastalıkları Sigortası
        /// </summary>
        public decimal isKazasiMeslekHast;
        /// <summary>
        /// Analık Sigortası
        /// </summary>
        public decimal analikSigortasi;
        /// <summary>
        /// Hastalık Sigortası
        /// </summary>
        public decimal hastalikSigortasi;
        /// <summary>
        /// Malüllük, Yaşlılık ve Ölüm Sigortası
        /// </summary>
        public decimal malulYasliOlumSigortasi;
        /// <summary>
        /// SSK İşsizlik Sigortası İşveren Payı Oranı
        /// </summary>
        public decimal sskIssizSigIsVerenPayiOran;
        /// <summary>
        /// SSK İşsizlik Sigortası İşçi Payı Oranı
        /// </summary>
        public decimal sskIssizSigIsciPayiOran;
        /// <summary>
        /// Net Asgari Ücret
        /// </summary>
        public decimal netAsgariUcret;

        public decimal sakatlikIndirimi1;
        public decimal sakatlikIndirimi2;
        public decimal sakatlikIndirimi3;
        public decimal kira1;
        public decimal yakit1;
        public decimal sskTavan;
        public decimal topluGorusmeSabiti;
        public decimal aileYardimiKatsayisi;

        /// <summary>
        /// <see cref="PSLMaasTemelBilgisi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLMaasTemelBilgisi()
        {
            yil = 0;
            ay = 0;
            tabanAylikKatsayi = 0;
            yanOdemeKatsayi = 0;
            enYuksMemurMaasGost = 0;
            aylikKatsayi = 0;
            gelirVergisiOran = 0;
            sskIsverenPayiOran = 0;
            sskIsciPayiOran = 0;
            damgaVergisiOran = 0;
            brutAsgariUcret = 0;
            isKazasiMeslekHast = 0;
            analikSigortasi = 0;
            hastalikSigortasi = 0;
            malulYasliOlumSigortasi = 0;
            sskIssizSigIsVerenPayiOran = 0;
            sskIssizSigIsciPayiOran = 0;
            netAsgariUcret = 0;
            sakatlikIndirimi1 = 0;
            sakatlikIndirimi2 = 0;
            sakatlikIndirimi3 = 0;
            kira1 = 0;
            yakit1 = 0;
            sskTavan = 0;
            topluGorusmeSabiti = 0;
            aileYardimiKatsayisi = 0;
        }
    }

    /// <summary>
    /// Gelir Vergisi tanım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLGelirVergisi
    {
        /// <summary>
        /// Gelir Vergisi Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Gelir Vergisi Yüzde
        /// </summary>
        public decimal yuzde;
        /// <summary>
        /// Gelir Vergisi Uygulanacak Tavan Miktar
        /// </summary>
        public decimal miktar;
        /// <summary>
        /// Artan
        /// </summary>
        public decimal artan;
        /// <summary>
        /// Vergi
        /// </summary>
        public decimal vergisi;

        /// <summary>
        /// <see cref="PSLGelirVergisi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLGelirVergisi()
        {
            yil = 0;
            yuzde = 0;
            miktar = 0;
            artan = 0;
            vergisi = 0;
        }
    }

    /// <summary>
    /// SSK Hizmet Belgesine ait bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLSSKHizmetBelgesi
    {
        /// <summary>
        /// SSK Hizmet Belgesi Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// SSK Hizmet Belgesi Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// SSK Hizmet Belgesi Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// SSK Hizmet Belgesi Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Toplam Ücretsiz İzinli Personel Sayısı
        /// </summary>
        public int ucretsizIzinliSayi;
        /// <summary>
        /// Toplam Ücretsiz İzin Gün Sayısı
        /// </summary>
        public int ucretsizIzinGunSayisi;
        /// <summary>
        /// Toplam Ücretli İzinli Personel Sayısı
        /// </summary>
        public int ucretliIzinliSayi;
        /// <summary>
        /// Toplam Ücretli İzin Gün Sayısı
        /// </summary>
        public int ucretliIzinGunSayisi;
        /// <summary>
        /// İzin Ücreti
        /// </summary>
        public decimal izinUcreti;
        /// <summary>
        /// Tür
        /// </summary>
        public int tur;

        /// <summary>
        /// <see cref="PSLSSKHizmetBelgesi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLSSKHizmetBelgesi()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            ucretsizIzinliSayi = 0;
            ucretsizIzinGunSayisi = 0;
            ucretliIzinliSayi = 0;
            ucretliIzinGunSayisi = 0;
            izinUcreti = 0;
            tur = 0;
        }
    }

    /// <summary>
    /// Firma Teklif Daveti bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FirmaTeklifDaveti
    {
        /// <summary>
        /// Firma Teklif Daveti Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Firma Teklif Daveti Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Firma Teklif Daveti Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Firma Teklif Daveti Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Firma Teklif Daveti Sıra No
        /// </summary>
        public int davetSiraNo;
        /// <summary>
        /// Teklif Daveti Yapılan Firma Kod
        /// </summary>
        public int firmaKodu;
        /// <summary>
        /// Teklif Daveti Yapılan Firma Ad
        /// </summary>
        public string firmaAdi;
        /// <summary>
        /// Teklif Daveti Yapılan Firma Adres
        /// </summary>
        public string firmaAdres;
        /// <summary>
        /// Teklif Daveti Yapılan Firma Telefon
        /// </summary>
        public string firmaTelefon;
        /// <summary>
        /// Teklif Daveti Yapılan Firma Faks
        /// </summary>
        public string firmaFaks;
        /// <summary>
        /// Teklif Daveti Belge Gönderme Tarihi
        /// </summary>
        public TNSDateTime belgeGondermeTarihi;
        /// <summary>
        /// Teklif Daveti Belge Gönderme Şekli
        /// </summary>
        public int gidisSekli;
        /// <summary>
        /// Belge Silinecek Mi
        /// </summary>
        public bool sil;

        /// <summary>
        /// <see cref="FirmaTeklifDaveti"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FirmaTeklifDaveti()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            davetSiraNo = 0;
            firmaKodu = 0;
            firmaAdi = string.Empty;
            firmaAdres = string.Empty;
            firmaTelefon = string.Empty;
            firmaFaks = string.Empty;
            belgeGondermeTarihi = new TNSDateTime();
            gidisSekli = 0;
            sil = false;
        }
    }

    /// <summary>
    /// Personel Sosyal Yardım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLSosyalYardimi
    {
        /// <summary>
        /// Personel Sosyal Yardım Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Personel Sosyal Yardım Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Personel Sosyal Yardım Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Personel Sosyal Yardım Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İlgili Personel TC Kimlik No
        /// </summary>
        public string TCKimlikNo;
        /// <summary>
        /// Olay Yıl
        /// </summary>
        public int olayYili;
        /// <summary>
        /// Olay Ay
        /// </summary>
        public int olayAyi;
        /// <summary>
        /// Aylık Katsayı
        /// </summary>
        public decimal aylikKatsayi;
        /// <summary>
        /// Katsayı
        /// </summary>
        public decimal katsayi;
        /// <summary>
        /// En Yüksek Memur Maaş Göstergesi
        /// </summary>
        public decimal enYuksMemurMaasi;
        /// <summary>
        /// Oran
        /// </summary>
        public decimal oran;
        /// <summary>
        /// Tutar
        /// </summary>
        public decimal tutar;
        /// <summary>
        /// Olay Niteliği
        /// </summary>
        public int olayNiteligi;
        /// <summary>
        /// Kişinin Personele yakınlık Derecesi
        /// </summary>
        public int yakinlikDerecesi;
        /// <summary>
        /// Personel yakınına ait TC Kimlik No
        /// </summary>
        public string yakinTCKimlikNo;
        /// <summary>
        /// Olay Tarihi
        /// </summary>
        public TNSDateTime olayTarihi;
        /// <summary>
        /// Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Personel yakınına ait Ad Soyad
        /// </summary>
        public string yakinAdSoyad;
        /// <summary>
        /// Personel Unvan Kod
        /// </summary>
        public int unvanKodu;
        /// <summary>
        /// Personel Unvan Ad
        /// </summary>
        public string unvanAdi;
        /// <summary>
        /// Personel Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// İlgili Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;

        /// <summary>
        /// <see cref="PSLSosyalYardimi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLSosyalYardimi()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            olayYili = 0;
            olayAyi = 0;
            aylikKatsayi = 0;
            katsayi = 0;
            enYuksMemurMaasi = 0;
            oran = 0;
            tutar = 0;
            olayNiteligi = 0;
            yakinlikDerecesi = 0;
            TCKimlikNo = string.Empty;
            yakinTCKimlikNo = string.Empty;
            olayTarihi = new TNSDateTime();
            adSoyad = string.Empty;
            yakinAdSoyad = string.Empty;
            unvanKodu = 0;
            unvanAdi = string.Empty;
            birimAd = string.Empty;
            odemeEmriNo = string.Empty;
        }
    }

    /// <summary>
    /// Personel Sosyal Yardım Bilgileri tanımlama bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLSosyalYardimTemelBilgisi
    {
        /// <summary>
        /// Personel Sosyal Yardım Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Personel Sosyal Yardım Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Aile Yardımı Göstergesi
        /// </summary>
        public decimal aileYardimGost;
        /// <summary>
        /// 0-6 yaş grubundaki (72.Ay dahil) çocuklar için Çocuk Yardımı Göstergesi
        /// </summary>
        public decimal cocukYardimGost0_6;
        /// <summary>
        /// 6 ve üzeri yaş grubundaki çocuklar için Çocuk Yardımı Göstergesi
        /// </summary>
        public decimal cocukYardimGost6_X;
        /// <summary>
        /// Doğum Yardımı Göstergesi
        /// </summary>
        public decimal dogumYardimGost;
        /// <summary>
        /// Memur Ölümü Katsayısı
        /// </summary>
        public decimal memurOlumKatsayi;
        /// <summary>
        /// Eş ve Çocuk Ölümü Katsayısı
        /// </summary>
        public decimal esCocukOlumKatsayi;

        /// <summary>
        /// <see cref="PSLSosyalYardimTemelBilgisi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLSosyalYardimTemelBilgisi()
        {
            yil = 0;
            ay = 0;
            aileYardimGost = 0;
            cocukYardimGost0_6 = 0;
            cocukYardimGost6_X = 0;
            dogumYardimGost = 0;
            memurOlumKatsayi = 0;
            esCocukOlumKatsayi = 0;
        }
    }

    /// <summary>
    /// Personel Aile Yardım Bildirimi Başlık bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLAileYardimBildirimiBaslik
    {
        /// <summary>
        /// Personel Aile Yardım Bildirimi Başlık Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Personel Aile Yardım Bildirimi Başlık Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Personel Aile Yardım Bildirimi Başlık Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Personel Aile Yardım Bildirimi Başlık Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Bildirim Sahibi Personel TC Kimlik No
        /// </summary>
        public string TCKimlikNo;
        /// <summary>
        /// Bildirim Sahibi Personel Ad Soyad
        /// </summary>
        public string perAdSoyad;
        /// <summary>
        /// Bildirim Sahibi Personel Gorev
        /// </summary>
        public string gorevi;
        /// <summary>
        /// Bildirim Sahibi Personel Unvan Kod
        /// </summary>
        public int unvanKodu;
        /// <summary>
        /// Bildirim Sahibi Personel Medeni Hal
        /// </summary>
        public int medeniHali;
        /// <summary>
        /// Bildirim İlgili Ay
        /// </summary>
        public int ilgiliAy;
        /// <summary>
        /// Bildirim İlgili Yıl
        /// </summary>
        public int ilgiliYil;
        /// <summary>
        /// Bildirim Sahibi Personel Eş Ad Soyad
        /// </summary>
        public string esAdSoyad;
        /// <summary>
        /// Bildirim Sahibi Personel Evlenme Tarih
        /// </summary>
        public TNSDateTime evlenmeTarihi;
        /// <summary>
        /// Bildirim Sahibi Personel Aile Cüzdanı Seri No
        /// </summary>
        public string aileCuzdaniSeriNo;
        /// <summary>
        /// Bildirim Sahibi Personel Eş İş Durumu
        /// </summary>
        public int esIsDurumu;
        /// <summary>
        /// Bildirim Sahibi Personel Eş Hakkında Açıklama
        /// </summary>
        public string esAciklama;
        /// <summary>
        /// Personel Aile Yardım Bildirimi Başlık Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Personel Aile Yardım Bildirimi Başlık Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Bildirim Sahibi Personele ait cocuk bilgilerini <see cref="PSLAileYardimBildirimiSatir"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray cocuklar;

        /// <summary>
        /// <see cref="PSLAileYardimBildirimiBaslik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLAileYardimBildirimiBaslik()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            TCKimlikNo = string.Empty;
            perAdSoyad = string.Empty;
            gorevi = string.Empty;
            unvanKodu = 0;
            medeniHali = 0;
            ilgiliAy = 0;
            ilgiliYil = 0;
            esAdSoyad = string.Empty;
            evlenmeTarihi = new TNSDateTime();
            aileCuzdaniSeriNo = string.Empty;
            esIsDurumu = 0;
            esAciklama = string.Empty;
            muhasebeAd = string.Empty;
            birimAd = string.Empty;
            cocuklar = new ObjectArray();
        }
    }


    /// <summary>
    /// Personel Aile Yardım Bildirimi Personel çocuk bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLAileYardimBildirimiSatir
    {
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string perTCKimlikNo;
        /// <summary>
        /// Personel Çocuğuna ait TC Kimlik No
        /// </summary>
        public string cocukTCKimlikNo;
        /// <summary>
        /// Personel Çocuğuna ait Ad Soyad
        /// </summary>
        public string cocukAdSoyad;
        /// <summary>
        /// Personel Çocuğuna ait Doğum Tarihi
        /// </summary>
        public TNSDateTime dogumTarihi;
        /// <summary>
        /// Personel Çocuğunun Cinsiyeti
        /// </summary>
        public int cinsiyet;
        /// <summary>
        /// Personel Çocuğunun Baba Adı
        /// </summary>
        public string babaAd;
        /// <summary>
        /// Personel Çocuğunun Anne Adı
        /// </summary>
        public string anaAd;
        /// <summary>
        /// Personel Çocuğunun Personele yakınlığı
        /// </summary>
        public int cocukYakinligi;
        /// <summary>
        /// Personel Çocuğunun Yüksek Öğretim Kurumuna Kayıt Tarihi
        /// </summary>
        public TNSDateTime yuksOgrKayitTarihi;
        /// <summary>
        /// Personel Çocuğunun Yüksek Öğretim gördüğü Okul Adı
        /// </summary>
        public string yuksOgrOkulAdi;
        /// <summary>
        /// Personel Çocuğunun Yüksek Öğretim gördüğü Sınıf bilgisi
        /// </summary>
        public int sinif;
        /// <summary>
        /// Personel Çocuğunun Burs Durumu
        /// </summary>
        public int bursDurumu;
        /// <summary>
        /// Personel Çocuğu hakkında açıklama
        /// </summary>
        public string aciklama;

        /// <summary>
        /// <see cref="PSLAileYardimBildirimiSatir"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLAileYardimBildirimiSatir()
        {

            perTCKimlikNo = string.Empty;
            cocukTCKimlikNo = string.Empty;
            cocukAdSoyad = string.Empty;
            dogumTarihi = new TNSDateTime();
            cinsiyet = 0;
            babaAd = string.Empty;
            anaAd = string.Empty;
            cocukYakinligi = 0;
            yuksOgrKayitTarihi = new TNSDateTime();
            yuksOgrOkulAdi = string.Empty;
            sinif = 0;
            bursDurumu = 0;
            aciklama = string.Empty;
        }
    }
    /// <summary>
    /// Sözleşmeli Personel Bordro hesaplamalarında kullanılan bilgileri tutan nesne
    /// </summary>
    [Serializable()]
    public class SozlesmeliBordroTanim
    {
        /// <summary>
        /// Sözleşmeli Personel Bordo Tanım Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Sözleşmeli Personel Bordo Tanım Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Sözleşmeli Personel Bordo Tanım İlgili Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Sözleşmeli Personel Bordo Tanım İlgili Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// İlgili Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Asgari Geçim İndirimi Oranı
        /// </summary>
        public decimal asgariGecimIndirim;
        /// <summary>
        /// Sözleşme Ücreti
        /// </summary>
        public decimal sozlesmeUcret;
        /// <summary>
        /// Ek Tazminat Oranı
        /// </summary>
        public decimal ekTazminatOrani;
        /// <summary>
        /// Özel Sigorta
        /// </summary>
        public decimal ozelSigorta;
        /// <summary>
        /// Sakatlık İndirimi
        /// </summary>
        public decimal sakatlikIndirimi;
        /// <summary>
        /// Sendika Devlet Yardımı
        /// </summary>
        public decimal sendikaDevletYardimi;
        /// <summary>
        /// Sendika Aidatı
        /// </summary>
        public decimal sendikaAidati;
        /// <summary>
        /// Lojman Kira Tutar
        /// </summary>
        public decimal lojman;
        /// <summary>
        /// İcra Tutar
        /// </summary>
        public decimal icra;
        /// <summary>
        /// Nafaka Tutar
        /// </summary>
        public decimal nafaka;
        /// <summary>
        /// Diğer Kesinti Tutar
        /// </summary>
        public decimal digerKesinti;
        /// <summary>
        /// Kümülatif Vergi Matrahi Başlangıç Değeri
        /// </summary>
        public decimal kumulatifVergiMatrahiBaslangic;
        /// <summary>
        /// İlgili Personel Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// İlgili Personel Soyad
        /// </summary>
        public string soyad;
        /// <summary>
        /// İlgili Personel Unvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// İlgili Personel Maaş Başlangıç Yıl
        /// </summary>
        public int maasYilBaslangic;
        /// <summary>
        /// İlgili Personel Maaş Başlangıç Ay
        /// </summary>
        public int maasAyBaslangic;
        /// <summary>
        /// Bordro Tur
        /// </summary>
        public int bordroTur;
        /// <summary>
        /// SSK İşveren Payı Oranı
        /// </summary>
        public decimal sskIsverenPayiOrani;

        /// <summary>
        /// <see cref="SozlesmeliBordroTanim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SozlesmeliBordroTanim()
        {
            muhasebe = 0;
            birim = string.Empty;
            yil = 0;
            ay = 0;
            tcKimlikNo = string.Empty;
            asgariGecimIndirim = 0;
            sozlesmeUcret = 0;
            ekTazminatOrani = 0;
            sakatlikIndirimi = 0;
            ozelSigorta = 0;
            sendikaAidati = 0;
            sendikaDevletYardimi = 0;
            lojman = 0;
            icra = 0;
            nafaka = 0;
            kumulatifVergiMatrahiBaslangic = 0;
            ad = string.Empty;
            soyad = string.Empty;
            unvan = string.Empty;
            maasYilBaslangic = 0;
            maasAyBaslangic = 0;
            bordroTur = 0;
            sskIsverenPayiOrani = 0;
        }
    }

    /// <summary>
    /// İhale Takvim bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IhaleTakvim
    {
        /// <summary>
        /// İhaleye ait süreç
        /// </summary>
        public string ihaleSurec;
        /// <summary>
        /// Süre bilgisi
        /// </summary>
        public TNSDateTime sure;

        /// <summary>
        /// <see cref="IhaleTakvim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IhaleTakvim()
        {
            ihaleSurec = string.Empty;
            sure = new TNSDateTime();
        }
    }

    /// <summary>
    /// Fazla Mesai bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FazlaMesai
    {
        /// <summary>
        /// Fazla Mesai Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Fazla Mesai Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Fazla Mesai Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Fazla Mesai Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fazla Mesai İlgili Ay
        /// </summary>
        public int ilgiliAy;
        /// <summary>
        /// Fazla Mesai İlgili Yıl
        /// </summary>
        public int ilgiliYil;
        /// <summary>
        /// Fazla Mesaiye ait ilgili Personelleri <see cref="FazlaMesaiPersonel"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public TNSCollection personel;
        /// <summary>
        /// Fazla Mesai sonuç durumunu <see cref="Sonuc"/> nesnesi ile tutan özellik
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// <see cref="FazlaMesai"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FazlaMesai()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            ilgiliAy = 0;
            ilgiliYil = 0;
            personel = new TNSCollection();
            sonuc = new Sonuc();
        }
    }

    /// <summary>
    /// Fazla Mesai Bordro bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FazlaMesaiBordro
    {
        /// <summary>
        /// Fazla Mesai Bordro Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Fazla Mesai Bordro Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Fazla Mesai Bordro Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Fazla Mesai Bordro Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Ay
        /// </summary>
        public int ilgiliAy;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Yıl
        /// </summary>
        public int ilgiliYil;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel Unvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel Banka
        /// </summary>
        public string banka;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel Banka Şube
        /// </summary>
        public string sube;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel Hesap No
        /// </summary>
        public string hesapNo;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel İstihdam Şekli
        /// </summary>
        public int istihdamSekli;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel hafta içi çalıştığı saat toplamı
        /// </summary>
        public double haftaIciSaat;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel hafta sonu çalıştığı saat toplamı
        /// </summary>
        public double haftaSonuSaat;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel hafta içi ücret toplamı
        /// </summary>
        public double haftaIciUcret;
        /// <summary>
        /// Fazla Mesai Bordro İlgili Personel hafta sonu ücret toplamı
        /// </summary>
        public double haftaSonuUcret;
        /// <summary>
        /// Damga Vergisi Tutar
        /// </summary>
        public double damgaVergisi;
        /// <summary>
        /// Kesinti Tutar
        /// </summary>
        public double kesinti;
        /// <summary>
        /// Gelir Vergisi Tutar
        /// </summary>
        public double gelirVergisi;
        /// <summary>
        /// Gelir Vergisi Oran
        /// </summary>
        public double gelirVergisiOran;
        public double sgkIsveren;
        public double sgkIsci;

        /// <summary>
        /// <see cref="FazlaMesaiBordro"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FazlaMesaiBordro()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            ilgiliAy = 0;
            ilgiliYil = 0;
            tcKimlikNo = string.Empty;
            adSoyad = string.Empty;
            unvan = string.Empty;
            istihdamSekli = 0;
            haftaIciSaat = 0;
            haftaIciUcret = 0;
            haftaSonuSaat = 0;
            haftaSonuUcret = 0;
            damgaVergisi = 0;
            kesinti = 0;
            gelirVergisi = 0;
            gelirVergisiOran = 0;
            sgkIsveren = 0;
            sgkIsci = 0;
        }
    }


    /// <summary>
    /// Fazla Mesai Bordro listesini tutan nesne
    /// </summary>
    [Serializable()]
    public class FazlaMesaiBordrolar
    {
        /// <summary>
        /// <see cref="FazlaMesaiBordro"/> nesne koleksiyonu tutan özellik
        /// </summary>
        public TNSCollection liste;
        /// <summary>
        /// <see cref="Sonuc"/> liste sonuç bilgisini tutan özellik
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// <see cref="FazlaMesaiBordrolar"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FazlaMesaiBordrolar()
        {
            liste = new TNSCollection();
            sonuc = new Sonuc();
        }
    }

    /// <summary>
    /// Fazla Mesai Personel bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FazlaMesaiPersonel
    {
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Personel Unvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Personel İstihdam Şekli
        /// </summary>
        public int istihdamSekli;
        /// <summary>
        /// Gelir Vergisi Oranı
        /// </summary>
        public decimal gelirVergisiOrani;
        /// <summary>
        /// Personel Fazla Mesai yaptığı zaman bilgisini <see cref="FazlaMesaiPersonelSaat"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public TNSCollection gunSaat;
        /// <summary>
        /// Fazla Mesai hesabında kullanılacak puantaj bilgisini <see cref="FazlaMesaiPuantaj"/> nesnesi olarak tutan özellik
        /// </summary>
        public FazlaMesaiPuantaj puantaj;

        /// <summary>
        /// <see cref="FazlaMesaiPersonel"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FazlaMesaiPersonel()
        {
            tcKimlikNo = string.Empty;
            adSoyad = string.Empty;
            unvan = string.Empty;
            gelirVergisiOrani = 0;
            gunSaat = new TNSCollection();
            puantaj = new FazlaMesaiPuantaj();
        }
    }

    /// <summary>
    /// Fazla Mesai Puantaj tanım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FazlaMesaiPuantaj
    {
        /// <summary>
        /// Fazla Mesai Puantaj Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Fazla Mesai Puantaj Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Fazla Mesai Puantaj Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Bir ay boyunca en fazla, fazla mesai yapılabilecek saat sayısı
        /// </summary>
        public double aylikSaat;
        /// <summary>
        /// Bir hafta boyunca en fazla, fazla mesai yapılabilecek saat sayısı
        /// </summary>
        public double haftaIciSaat;
        /// <summary>
        /// Hafta içi saat ücreti
        /// </summary>
        public double haftaIciUcret;
        /// <summary>
        /// Cumartesi günü en fazla, fazla mesai yapılabilecek saat sayısı
        /// </summary>
        public double cumartesiSaat;
        /// <summary>
        /// Cumartesi günü saat ücreti
        /// </summary>
        public double cumartesiUcret;
        /// <summary>
        /// Pazar günü en fazla, fazla mesai yapılabilecek saat sayısı
        /// </summary>
        public double pazarSaat;
        /// <summary>
        /// Pazar günü saat ücreti
        /// </summary>
        public double pazarUcret;
        /// <summary>
        /// Puantaj Varsayılan olarak işaretlensin mi
        /// </summary>
        public int varsayilan;

        /// <summary>
        /// <see cref="FazlaMesaiPuantaj"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FazlaMesaiPuantaj()
        {
            yil = 0;
            kod = 0;
            aciklama = string.Empty;
            aylikSaat = 0;
            haftaIciSaat = 0;
            haftaIciUcret = 0;
            cumartesiSaat = 0;
            cumartesiUcret = 0;
            pazarSaat = 0;
            pazarUcret = 0;
            varsayilan = 0;
        }
    }

    /// <summary>
    /// Fazla Mesai Puantaj bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class FazlaMesaiPuantajlar
    {
        /// <summary>
        /// Fazla Mesai Puantaj bilgilerini <see cref="FazlaMesaiPuantaj"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public TNSCollection liste;
        /// <summary>
        /// <see cref="Sonuc"/> liste sonuç bilgisini tutan özellik
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// <see cref="FazlaMesaiPuantajlar"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FazlaMesaiPuantajlar()
        {
            liste = new TNSCollection();
            sonuc = new Sonuc();
        }
    }

    /// <summary>
    /// Fazla Mesai Personel Saat bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class FazlaMesaiPersonelSaat
    {
        /// <summary>
        /// Fazla Mesai yapılan ayın kaçıncı günü olduğu
        /// </summary>
        public int gun;
        /// <summary>
        /// Fazla Mesai yapılan saat sayısı
        /// </summary>
        public double saat;

        /// <summary>
        /// <see cref="FazlaMesaiPersonelSaat"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public FazlaMesaiPersonelSaat()
        {
            gun = 0;
            saat = 0;
        }
    }

    [Serializable()]
    public class FazlaMesaiPuantajUnvan
    {
        public int unvanKod;
        public double toplamSaat;
        public double kesilenSaat;
        public string unvanAd;

        public FazlaMesaiPuantajUnvan()
        {
            unvanKod = 0;
            toplamSaat = 0.0;
            kesilenSaat = 0.0;
            unvanAd = string.Empty;
        }
    }

    [Serializable()]
    public class FazlaMesaiBordroUnvan
    {
        public int yil;
        public string surecNo;
        public int ilgiliYil;
        public int ilgiliAy;
        public string tcKimlikNo;
        public int unvanKod;
        public double toplamSaat;
        public double kanuniIzin;
        public double yurtDisiGorev;
        public double resmiDiniGun;
        public double kesilenSaat;
        public double toplamKesilenSaat;
        public double hesaplanacakSaat;
        public double saatUcreti;
        public double toplamTutar;
        public double fazlaMesaiSaat;
        public double genelToplamSaat;
        public string ad;
        public string soyad;
        public string unvanAd;
        public double damgaVergisi;
        public double sgk195;
        public double sgk14;
        public double kesinti;
        public double kesintiToplam;
        public double netOdenecek;
        public bool sozlesmeliMi;


        public FazlaMesaiBordroUnvan()
        {
            yil = 0;
            surecNo = string.Empty;
            ilgiliYil = 0;
            ilgiliAy = 0;
            tcKimlikNo = string.Empty;
            unvanKod = 0;
            toplamSaat = 0.0;
            kanuniIzin = 0.0;
            yurtDisiGorev = 0.0;
            resmiDiniGun = 0.0;
            kesilenSaat = 0.0;
            toplamKesilenSaat = 0.0;
            hesaplanacakSaat = 0.0;
            saatUcreti = 0.0;
            toplamTutar = 0.0;
            fazlaMesaiSaat = 0.0;
            genelToplamSaat = 0.0;
            ad = string.Empty;
            soyad = string.Empty;
            unvanAd = string.Empty;
            damgaVergisi = 0.0;
            sgk195 = 0.0;
            sgk14 = 0.0;
            kesinti = 0.0;
            kesintiToplam = 0.0;
            netOdenecek = 0.0;
            sozlesmeliMi = false;
        }
    }


    /// <summary>
    /// Ek Ders bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class EkDers
    {
        /// <summary>
        /// Ek Ders Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ek Ders Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Ek Ders Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Ek Ders Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Ek Ders İlgili Ay
        /// </summary>
        public int ilgiliAy;
        /// <summary>
        /// Ek Ders İlgili Yıl
        /// </summary>
        public int ilgiliYil;
        /// <summary>
        /// Ek Ders Gece Mi
        /// </summary>
        public int gecemi;
        /// <summary>
        /// Ek Derse ait personel bilgilerini <see cref="EkDersPersonel"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public TNSCollection personel;
        /// <summary>
        /// Ek Derse ait sonuç bilgisini <see cref="Sonuc"/> nesnesi olarak tutan özellik
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// <see cref="EkDers"/> 
        /// </summary>
        public EkDers()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            ilgiliAy = 0;
            ilgiliYil = 0;
            gecemi = 0;
            personel = new TNSCollection();
            sonuc = new Sonuc();
        }
    }


    /// <summary>
    /// Ek Ders Bordro bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class EkDersBordro
    {
        /// <summary>
        /// Ek Ders Bordro Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ek Ders Bordro Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Ek Ders Bordro Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Ek Ders Bordro Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Ek Ders Bordro İlgili Ay
        /// </summary>
        public int ilgiliAy;
        /// <summary>
        /// Ek Ders Bordro İlgili Yıl
        /// </summary>
        public int ilgiliYil;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel Unvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel Banka
        /// </summary>
        public string banka;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel Banka Şube
        /// </summary>
        public string sube;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel Hesap No
        /// </summary>
        public string hesapNo;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel İstihdam Şekli
        /// </summary>
        public int istihdamSekli;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel hafta içi çalıştığı toplam saat sayısı
        /// </summary>
        public double haftaiciSaat;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel hafta içi ücret toplamı
        /// </summary>
        public double haftaiciUcret;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel hafta sonu çalıştığı toplam saat sayısı
        /// </summary>
        public double haftasonuSaat;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel hafta sonu ücret toplamı
        /// </summary>
        public double haftasonuUcret;
        /// <summary>
        /// Damga Vergisi Tutar
        /// </summary>
        public double damgaVergisi;
        /// <summary>
        /// Gelir Vergisi Tutar
        /// </summary>
        public double gelirVergisi;
        /// <summary>
        /// Gelir Vergisi Oran
        /// </summary>
        public double gelirVergisiOran;
        /// <summary>
        /// Kesinti Tutar
        /// </summary>
        public double kesinti;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel Banka Ad
        /// </summary>
        public string bankaAdi;
        /// <summary>
        /// Ek Ders Bordro İlgili Personel Banka Şube Ad
        /// </summary>
        public string subeAdi;

        /// <summary>
        /// <see cref="EkDersBordro"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EkDersBordro()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            ilgiliAy = 0;
            ilgiliYil = 0;
            tcKimlikNo = string.Empty;
            adSoyad = string.Empty;
            unvan = string.Empty;
            istihdamSekli = 0;
            haftaiciSaat = 0;
            haftaiciUcret = 0;
            haftasonuSaat = 0;
            haftasonuUcret = 0;
            damgaVergisi = 0;
            gelirVergisi = 0;
            gelirVergisiOran = 0;
            kesinti = 0;
            bankaAdi = string.Empty;
            subeAdi = string.Empty;
        }
    }

    /// <summary>
    /// Ek Ders Bordro bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class EkDersBordrolar
    {
        /// <summary>
        /// Ek ders bordro bilgilerini <see cref="EkDersBordro"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public TNSCollection liste;
        /// <summary>
        /// Ek Ders bordrolarına ait sonuç bilgisini <see cref="Sonuc"/> nesnesi olarak tutan özellik
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// <see cref="EkDersBordrolar"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EkDersBordrolar()
        {
            liste = new TNSCollection();
            sonuc = new Sonuc();
        }
    }

    /// <summary>
    /// Ek Ders Personel bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class EkDersPersonel
    {
        /// <summary>
        /// Ek Ders Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Ek Ders Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Ek Ders Personel Unvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Ek Ders Personel İstihdam Şekli
        /// </summary>
        public int istihdamSekli;
        /// <summary>
        /// Ek Ders Personel Gelir Vergisi Oranı
        /// </summary>
        public decimal gelirVergisiOrani;
        /// <summary>
        /// Ek Ders Personeline ait çalışma bilgilerini <see cref="EkDersPersonelSaat"/> nesne koleksiyonu olarak tutan koleksiyon
        /// </summary>
        public TNSCollection gunSaat;
        /// <summary>
        /// Ek Ders Personele ait puantaj bilgisi
        /// </summary>
        public EkDersPuantaj puantaj;

        /// <summary>
        /// <see cref="EkDersPersonel"/> class.
        /// </summary>
        public EkDersPersonel()
        {
            tcKimlikNo = string.Empty;
            adSoyad = string.Empty;
            unvan = string.Empty;
            gelirVergisiOrani = 0;
            gunSaat = new TNSCollection();
            puantaj = new EkDersPuantaj();
        }
    }

    /// <summary>
    /// Ek Ders Puantaj Tanım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class EkDersPuantaj
    {
        /// <summary>
        /// Ek Ders Puantaj Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ek Ders Puantaj Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Ek Ders Puantaj Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Ek Ders Puantaj Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Personelin yıllık maksimum ek ders ücreti alabileceği toplam saat sayısı
        /// </summary>
        public double yillikSaat;
        /// <summary>
        /// Personelin haftalık maksimum ek ders ücreti alabileceği toplam saat sayısı
        /// </summary>
        public double haftalikSaat;
        /// <summary>
        /// Hafta içi saat ücreti
        /// </summary>
        public double haftaiciUcret;
        /// <summary>
        /// Hafta sonu saat ücreti
        /// </summary>
        public double haftasonuUcret;
        /// <summary>
        /// Ek Ders Puantaj bilgisi varsayılan olarak seçilecek mi
        /// </summary>
        public int varsayilan;
        /// <summary>
        /// Ek Ders Puantaj bilgisi bütün yıl için kaydedilsin mi
        /// </summary>
        public int butunYilKaydedilsinmi;
        /// <summary>
        /// Ek Ders Puantaj bilgisi durum
        /// </summary>
        public int durum;

        /// <summary>
        /// <see cref="EkDersPuantaj"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EkDersPuantaj()
        {
            yil = 0;
            ay = 0;
            kod = 0;
            aciklama = string.Empty;
            yillikSaat = 0;
            haftalikSaat = 0;
            haftaiciUcret = 0;
            haftasonuUcret = 0;
            varsayilan = 0;
            butunYilKaydedilsinmi = 0;
            durum = 0;
        }
    }

    /// <summary>
    /// Ek Ders Puantaj bilgisi listesini tutan nesne
    /// </summary>
    [Serializable()]
    public class EkDersPuantajlar
    {
        /// <summary>
        /// Ek Ders Puantaj bilgilerini <see cref="EkDersPuantaj"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public TNSCollection liste;
        /// <summary>
        /// Ek Ders Puantajlarına ait sonuç bilgisini <see cref="Sonuc"/> nesnesi olarak tutan özellik
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// <see cref="EkDersPuantajlar"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EkDersPuantajlar()
        {
            liste = new TNSCollection();
            sonuc = new Sonuc();
        }
    }

    /// <summary>
    /// Ek Ders Personel Saat bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class EkDersPersonelSaat
    {
        /// <summary>
        /// Yapılan Ek Dersin ayın kaçıncı günü olduğu
        /// </summary>
        public int gun;
        /// <summary>
        /// Yapılan Ek Dersin kaç saat olduğu
        /// </summary>
        public double saat;

        /// <summary>
        /// <see cref="EkDersPersonelSaat"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public EkDersPersonelSaat()
        {
            gun = 0;
            saat = 0;
        }
    }

    /// <summary>
    /// Geçici Görev Yolluk bilgisini tutan belge
    /// </summary>
    [Serializable()]
    public class GeciciGorevYollukBelge
    {
        /// <summary>
        /// Geçici Görev Yolluk Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Geçici Görev Yolluk Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Geçici Görev Yolluk Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Geçici Görev Yolluk Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Geçici Görev Yolluk İlgili Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Geçici Görev Yolluk Belge Tür
        /// </summary>
        public ENUMYollukGundelikTuru belgeTur;
        /// <summary>
        /// Geçici Görev Yolluk İlgili Personel Kadro Ek Gösterge
        /// </summary>
        public string kadroEk;
        /// <summary>
        /// Geçici Görev Yolluk Gündelik Yıl
        /// </summary>
        public int gundelikYil;
        /// <summary>
        /// Geçici Görev Yolluk Gündelik Kod
        /// </summary>
        public string gundelikKod;
        /// <summary>
        /// Geçici Görev Yolluk Gündelik Değer
        /// </summary>
        public decimal gundelikDeger;
        /// <summary>
        /// Geçici Görev Yolluk İlgili Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Geçici Görev Yolluk İlgili Personel Unvan Ad
        /// </summary>
        public string unvanAdi;
        /// <summary>
        /// Geçici Görev Yolluk İlgili Personel Unvan Kod
        /// </summary>
        public int unvanKodu;
        /// <summary>
        /// Döviz Kod
        /// </summary>
        public int dovizKodu;
        /// <summary>
        /// Döviz Kur
        /// </summary>
        public decimal dovizKuru;
        /// <summary>
        /// Geçici Görev Yolluk detay bilgilerini <see cref="GeciciGorevYollukDetay"/> nesne koleksiyonu olarak tutan nesne
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Konaklama için Günlük Ödenen Tutar
        /// </summary>
        public decimal konaklamaGunlukOdenen;
        /// <summary>
        /// Geçici Görev Yolluk Bildirim Tarihi
        /// </summary>
        public TNSDateTime bildirimTarihi;
        /// <summary>
        /// Geçici Görev Yolluk Belge ile ilgili Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Geçici Görev Yolluk Belge Toplam Tutarına eklenecek tutar hakkındaki açıklama
        /// </summary>
        public string eklenecekAciklama;
        /// <summary>
        /// Geçici Görev Yolluk Belge Toplam Tutarına eklenecek tutar 
        /// </summary>
        public decimal eklenecekTutar;
        /// <summary>
        /// Geçici Görev Yolluk Belge Toplam Tutarına eklenecek tutar para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan özellik
        /// </summary>
        public ParaBirimi eklenecekTutarParaBirimi;
        /// <summary>
        /// Geçici Görev Yolluk Belge Toplam Tutarından çıkarılacak tutar hakkındaki açıklama
        /// </summary>
        public string cikarilacakAciklama;
        /// <summary>
        /// Geçici Görev Yolluk Belge Toplam Tutarına çıkarılacak tutar 
        /// </summary>
        public decimal cikarilacakTutar;
        /// <summary>
        /// Geçici Görev Yolluk Belge Toplam Tutarından çıkarılacak tutar para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan özellik
        /// </summary>
        public ParaBirimi cikarilacakTutarParaBirimi;
        /// <summary>
        /// Geçici Görev Yolluk Belge tutarı arttırımlı hesaplansın mı
        /// </summary>
        public bool arttirimliHesapla;
        /// <summary>
        /// Kayıt sıra no
        /// </summary>
        public int kayitSiraNo;
        /// <summary>
        /// Ödeme Oranı
        /// </summary>
        public string odemeOrani;
        /// <summary>
        /// Emanet Oranı
        /// </summary>
        public double emanetOrani;
        /// <summary>
        /// İlgili Personel Avans Aldı Mı
        /// </summary>
        public bool avansAlindiMi;
        /// <summary>
        /// Kur Tarihi
        /// </summary>
        public TNSDateTime kurTarihi;
        /// <summary>
        /// Konaklama Tutarının Tamamı Ödenecek Mi
        /// </summary>
        public bool konaklamaTamamiOdenecekMi;
        public int yurtIciKonaklamaTuru;

        /// <summary>
        /// <see cref="GeciciGorevYollukBelge"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GeciciGorevYollukBelge()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebeKod = 0;
            birimKod = string.Empty;
            tcKimlikNo = string.Empty;
            kadroEk = string.Empty;
            gundelikYil = 0;
            gundelikKod = string.Empty;
            belgeTur = 0;
            gundelikDeger = 0;
            adSoyad = string.Empty;
            unvanAdi = string.Empty;
            dovizKodu = 0;
            dovizKuru = 0;
            detay = new ObjectArray();
            unvanKodu = 0;
            konaklamaGunlukOdenen = 0;
            bildirimTarihi = new TNSDateTime();
            odemeEmriNo = string.Empty;
            eklenecekAciklama = string.Empty;
            eklenecekTutar = 0;
            eklenecekTutarParaBirimi = new ParaBirimi();
            cikarilacakAciklama = string.Empty;
            cikarilacakTutar = 0;
            cikarilacakTutarParaBirimi = new ParaBirimi();
            arttirimliHesapla = false;
            kayitSiraNo = 0;
            odemeOrani = string.Empty;
            emanetOrani = 0;
            avansAlindiMi = false;
            kurTarihi = new TNSDateTime();
            konaklamaTamamiOdenecekMi = false;
            yurtIciKonaklamaTuru = 1;
        }

        public decimal GorevliGunSayisi(bool yolculukDahil)
        {
            decimal gunSay = 0;
            foreach (GeciciGorevYollukDetay item in detay.objeler)
            {
                gunSay += item.gorevGunSayisi + (yolculukDahil ? item.yolculukGunSayi : 0);
            }
            return gunSay;
        }

        public decimal GelirVergisiTutariHesapla(decimal muafTutar, bool yolculukDahil)
        {
            decimal gvTutar = 0;
            foreach (GeciciGorevYollukDetay item in detay.objeler)
            {
                decimal gun = item.gorevGunSayisi + (yolculukDahil ? item.yolculukGunSayi : 0);
                if (gun > 0 && item.yevmiyeTutar > muafTutar)
                    gvTutar += (gun * (item.yevmiyeTutar - muafTutar));
            }
            return gvTutar;
        }
    }

    /// <summary>
    /// Geçici Görev Yolluk Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class GeciciGorevYollukDetay
    {
        /// <summary>
        /// Geçici Görev Yolluk Detay Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Geçici Görev Yolluk Detay Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Geçici Görev Yolluk Detay Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Geçici Görev Yolluk Detay Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Geçici Görev Yolluk Detay Belge Tur
        /// </summary>
        public ENUMYollukGundelikTuru belgeTur;
        /// <summary>
        /// Geçici Görev Yolluk Detay İlgili Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNoBelge;
        /// <summary>
        /// Geçici Görev Yolluk Detay Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Yolculuk Tipi
        /// </summary>
        public int yolculukTipi;
        /// <summary>
        /// Yolculuk gün sayısı
        /// </summary>
        public decimal yolculukGunSayi;
        /// <summary>
        /// Yolculuk gün sayısı (yazı)
        /// </summary>
        public string yolculukGunSayiStr;
        /// <summary>
        /// Göreve nereden gidildiği bilgisi
        /// </summary>
        public string nereden;
        /// <summary>
        /// Göreve nereye gidildiği bilgisi
        /// </summary>
        public string nereye;
        /// <summary>
        /// Görev için hareket tarihi
        /// </summary>
        public TNSDateTime hareketTarihi;
        /// <summary>
        /// Görev için hareket saati
        /// </summary>
        public string hareketSaati;
        /// <summary>
        /// Yevmiye tutar
        /// </summary>
        public decimal yevmiyeTutar;
        /// <summary>
        /// Yevmiye para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan özellik
        /// </summary>
        public ParaBirimi yevmiyeParaBirimi;
        /// <summary>
        /// İkametten Terminale gidilirken kullanılan araç tipi
        /// </summary>
        public int ikamettenTerminaleAracTipi;
        /// <summary>
        /// İkametten Terminale gidilirken harcanan tutar
        /// </summary>
        public decimal ikamettenTerminaleHarcanan;
        /// <summary>
        /// İkametten Terminale gidilirken harcanan tutarın para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan nesne
        /// </summary>
        public ParaBirimi ikamettenTerminaleParaBirimi;
        /// <summary>
        /// Yolculuk yaparken kullanılan araç tipi
        /// </summary>
        public int yolculukAracTipi;
        /// <summary>
        /// Yolculuk yaparken harcanan tutar
        /// </summary>
        public decimal yolculuktaHarcanan;
        /// <summary>
        /// Yolculuk yaparken harcanan tutarın para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan nesne
        /// </summary>
        public ParaBirimi yolculuktaParaBirimi;
        /// <summary>
        /// Terminalden İkamete gidilirken kullanılan araç tipi
        /// </summary>
        public int terminaldenIkameteAracTipi;
        /// <summary>
        /// Terminalden İkamete gidilirken harcanan tutar
        /// </summary>
        public decimal terminaldenIkameteHarcanan;
        /// <summary>
        /// Terminalden İkamete gidilirken harcanan tutarın para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan özellik
        /// </summary>
        public ParaBirimi terminaldenIkameteParaBirimi;
        /// <summary>
        /// Görev Gün Sayısı
        /// </summary>
        public int gorevGunSayisi;
        /// <summary>
        /// Görev yevmiye toplam tutarı
        /// </summary>
        public decimal gorevYevmiyeToplami;
        /// <summary>
        /// Konaklama yapılan gün sayısı
        /// </summary>
        public decimal konaklananGunSayi;
        /// <summary>
        /// Konaklama günlük fatura tutarı
        /// </summary>
        public decimal konaklamaGunlukFatura;
        /// <summary>
        /// Konaklama günlük fatura tutarı para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan özellik
        /// </summary>
        public ParaBirimi konaklamaGunlukFaturaParaBirimi;
        /// <summary>
        /// Konaklama için İdare tarafından günlük ödenen tutar
        /// </summary>
        public decimal konaklamaGunlukOdenen;
        /// <summary>
        /// Hamal bagaj tutarı
        /// </summary>
        public decimal hamalBagaj;
        /// <summary>
        /// Hamal bagaj tutarı para birimini <see cref="ParaBirimi"/> nesnesi olarak tutan özeklik
        /// </summary>
        public ParaBirimi hamalBagajParaBirimi;
        /// <summary>
        /// Geçici Görev Yolluk Toplam Tutar
        /// </summary>
        public decimal toplamTutar;
        /// <summary>
        /// Geçici Görev Yolluk Toplam Konaklama için Ödenen Tutar
        /// </summary>
        public decimal toplamKonaklamaOdenen;
        /// <summary>
        /// Geçici Görev Yolluk Döviz cinsinden Toplam Tutar
        /// </summary>
        public decimal toplamTutarDoviz;
        /// <summary>
        /// Kayıt Sıra No
        /// </summary>
        public int kayitSiraNo;

        /// <summary>
        /// <see cref="GeciciGorevYollukDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GeciciGorevYollukDetay()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebeKod = 0;
            birimKod = string.Empty;
            belgeTur = 0;
            tcKimlikNoBelge = string.Empty;
            siraNo = 0;
            yolculukTipi = 0;
            yolculukGunSayi = 0;
            yolculukGunSayiStr = string.Empty;
            nereden = string.Empty;
            nereye = string.Empty;
            hareketSaati = string.Empty;
            hareketTarihi = new TNSDateTime();
            yevmiyeTutar = 0;
            yevmiyeParaBirimi = new ParaBirimi();
            ikamettenTerminaleAracTipi = 0;
            ikamettenTerminaleHarcanan = 0;
            ikamettenTerminaleParaBirimi = new ParaBirimi();
            yolculukAracTipi = 0;
            yolculuktaHarcanan = 0;
            yolculuktaParaBirimi = new ParaBirimi();
            terminaldenIkameteAracTipi = 0;
            terminaldenIkameteHarcanan = 0;
            terminaldenIkameteParaBirimi = new ParaBirimi();
            gorevGunSayisi = 0;
            gorevYevmiyeToplami = 0;
            konaklananGunSayi = 0;
            konaklamaGunlukFatura = 0;
            konaklamaGunlukFaturaParaBirimi = new ParaBirimi();
            konaklamaGunlukOdenen = 0;
            hamalBagaj = 0;
            hamalBagajParaBirimi = new ParaBirimi();
            toplamTutar = 0;
            toplamKonaklamaOdenen = 0;
            toplamTutarDoviz = 0;
            kayitSiraNo = 0;
        }
    }

    /// <summary>
    /// Geçici Görev Yolluk Toplu Sınıfı
    /// </summary>
    [Serializable()]
    public class GeciciGorevYollukToplu
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kodu
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kodu
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Belge Türü
        /// </summary>
        public int belgeTur;
        /// <summary>
        /// TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Çektiği Avans
        /// </summary>
        public decimal cektigiAvans;
        /// <summary>
        /// Yolluk Tutarı
        /// </summary>
        public decimal yollukTutari;
        /// <summary>
        /// PTT'ye öden Kargo Tutarı
        /// </summary>
        public decimal ptt;
        /// <summary>
        /// Damga Vergisi
        /// </summary>
        public decimal damgaVergisi;
        /// <summary>
        /// Ele Geçen Tutar
        /// </summary>
        public decimal eleGecen;
        /// <summary>
        /// Mahsup Edilen Tutar
        /// </summary>
        public decimal mahsupEdilen;
        /// <summary>
        /// Ödenecek Tutar
        /// </summary>
        public decimal odenecekTutar;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Sil Kontrolü
        /// </summary>
        public bool sil;
        /// <summary>
        /// Adı Soyadı
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Ünvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Ödeme Emri Numarası
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Banka
        /// </summary>
        public string banka;
        /// <summary>
        /// Şube
        /// </summary>
        public string sube;
        /// <summary>
        /// Hesap Numarası
        /// </summary>
        public string hesapNo;
        /// <summary>
        /// Gelir Vergisi
        /// </summary>
        public decimal gelirVergisi;

        /// <summary>
        /// <see cref="GeciciGorevYollukToplu"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public GeciciGorevYollukToplu()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebeKod = 0;
            birimKod = string.Empty;
            belgeTur = 0;
            tcKimlikNo = string.Empty;
            siraNo = 0;
            cektigiAvans = 0;
            yollukTutari = 0;
            ptt = 0;
            damgaVergisi = 0;
            eleGecen = 0;
            mahsupEdilen = 0;
            odenecekTutar = 0;
            aciklama = string.Empty;
            sil = false;
            adSoyad = string.Empty;
            unvan = string.Empty;
            odemeEmriNo = string.Empty;
            banka = string.Empty;
            sube = string.Empty;
            hesapNo = string.Empty;
            gelirVergisi = 0;
        }
    }

    /// <summary>
    /// Yolluk Gündelik Tanım Kriteri
    /// </summary>
    [Serializable()]
    public class YollukGundelikTanim
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Gündelik Kod
        /// </summary>
        public string kod;
        /// <summary>
        /// Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Değer
        /// </summary>
        public double deger;
        /// <summary>
        /// Tür
        /// </summary>
        public ENUMYollukGundelikTuru tur;
        /// <summary>
        /// Ay Kadro Derece Başlangıç
        /// </summary>
        public int ayKadroDereceBaslangic;
        /// <summary>
        /// Ay Kadro Derece Bitiş
        /// </summary>
        public int ayKadroDereceBitis;
        /// <summary>
        /// Ek Gösterge Başlangıcı
        /// </summary>
        public int ekGostergeBaslangic;
        /// <summary>
        /// Ek Gösterge Bitişi
        /// </summary>
        public int ekGostergeBitis;
        /// <summary>
        /// Ülke
        /// </summary>
        public Ulke ulke;
        /// <summary>
        /// Para Birimi
        /// </summary>
        public ParaBirimi paraBirimi;

        /// <summary>
        /// Sınıfın oluşturucu(constructer) fonksiyonu
        /// </summary>
        public YollukGundelikTanim()
        {
            yil = 0;
            kod = string.Empty;
            ad = string.Empty;
            deger = (double)0;
            tur = 0;
            ayKadroDereceBaslangic = 0;
            ayKadroDereceBitis = 0;
            ekGostergeBaslangic = 0;
            ekGostergeBitis = 0;
            ulke = new Ulke();
            paraBirimi = new ParaBirimi();
        }
    }

    /// <summary>
    /// Sürekli Görev Yolluk Belge bilgileri
    /// </summary>
    [Serializable()]
    public class SurekliGorevYollukBelge
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Belge Tür
        /// </summary>
        public int belgeTur;
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Personel Kadro Ek Gösterge
        /// </summary>
        public string kadroEk;
        /// <summary>
        /// Gündelik Yılı
        /// </summary>
        public int gundelikYil;
        /// <summary>
        /// Gündelik Kod
        /// </summary>
        public string gundelikKod;
        /// <summary>
        /// Atama Tarih
        /// </summary>
        public TNSDateTime atamaTarih;
        /// <summary>
        /// Nereden gidildiği bilgisi
        /// </summary>
        public string nereden;
        /// <summary>
        /// Nereye gidildiği bilgisi
        /// </summary>
        public string nereye;
        /// <summary>
        /// Kullanılan Taşıt Çeşidi
        /// </summary>
        public string tasitCesit;
        /// <summary>
        /// Bilet Ücreti
        /// </summary>
        public decimal biletUcret;
        /// <summary>
        /// Yolculuk Gün Sayısı
        /// </summary>
        public int gunSayi;
        /// <summary>
        /// Gündelik Tutar
        /// </summary>
        public decimal gundelikTutar;
        /// <summary>
        /// Yer Değiştirme Mesafe
        /// </summary>
        public decimal yerDegisMesafe;
        /// <summary>
        /// Yer Değiştirme Tutar
        /// </summary>
        public decimal yerDegisTutari;
        /// <summary>
        /// Döviz Kod
        /// </summary>
        public int dovizKodu;
        /// <summary>
        /// Döviz Kur
        /// </summary>
        public decimal dovizKuru;
        /// <summary>
        /// Değişken Unsur Katsayısı
        /// </summary>
        public decimal degiskenUnsur;
        /// <summary>
        /// Yer Değiştirme Gider
        /// </summary>
        public decimal yerDegistirmeGideri;
        /// <summary>
        /// Belge detaylarını <see cref="SurekliGorevYollukBelge"/> nesne koleksiyonu olarak tutan özellik
        /// </summary>
        public ObjectArray detay;
        /// <summary>
        /// Gündelik Değer
        /// </summary>
        public decimal gundelikDeger;
        /// <summary>
        /// Personel Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Personel Unvan Kod
        /// </summary>
        public int unvanKodu;
        /// <summary>
        /// Personel Unvan Ad
        /// </summary>
        public string unvanAdi;
        /// <summary>
        /// Bildirim Tarih
        /// </summary>
        public TNSDateTime bildirimTarihi;
        /// <summary>
        /// Belgeye ait Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Emeklilik Maaşı Mı
        /// </summary>
        public int emeklilikMaasiMi;
        /// <summary>
        /// Emeklilik Maaşı Tutar
        /// </summary>
        public decimal emeklilikMaasi;

        /// <summary>
        /// <see cref="SurekliGorevYollukBelge"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SurekliGorevYollukBelge()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebeKod = 0;
            birimKod = string.Empty;
            tcKimlikNo = string.Empty;
            belgeTur = 0;
            kadroEk = string.Empty;
            gundelikYil = 0;
            gundelikKod = string.Empty;
            atamaTarih = new TNSDateTime();
            nereden = string.Empty;
            nereye = string.Empty;
            tasitCesit = string.Empty;
            biletUcret = 0;
            gunSayi = 0;
            gundelikTutar = 0;
            yerDegisMesafe = 0;
            yerDegisTutari = 0;
            dovizKodu = 0;
            dovizKuru = 0;
            degiskenUnsur = 0;
            yerDegistirmeGideri = 0;
            detay = new ObjectArray();
            gundelikDeger = 0;
            adSoyad = string.Empty;
            unvanAdi = string.Empty;
            unvanKodu = 0;
            bildirimTarihi = new TNSDateTime();
            odemeEmriNo = string.Empty;
            emeklilikMaasiMi = 0;
            emeklilikMaasi = 0;
        }
    }

    /// <summary>
    /// Sürekli Görev Yolluk Detay bilgileri
    /// </summary>
    [Serializable()]
    public class SurekliGorevYollukDetay
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Belge Tur
        /// </summary>
        public int belgeTur;
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNoBelge;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Personel Yakını TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Personel Yakını Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Personel Yakını Akrabalık Kod
        /// </summary>
        public int akrabalikKodu;
        /// <summary>
        /// Sabit Unsur
        /// </summary>
        public decimal sabitUnsur;
        /// <summary>
        /// Toplam Tutar
        /// </summary>
        public decimal toplamTutar;
        /// <summary>
        /// Toplam Tutar Döviz
        /// </summary>
        public decimal toplamTutarDoviz;

        /// <summary>
        /// <see cref="SurekliGorevYollukDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SurekliGorevYollukDetay()
        {
            yil = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            surecNo = string.Empty;
            belgeTur = 0;
            tcKimlikNoBelge = string.Empty;
            siraNo = 0;
            tcKimlikNo = string.Empty;
            adSoyad = string.Empty;
            akrabalikKodu = 0;
            sabitUnsur = 0;
            toplamTutar = 0;
            toplamTutarDoviz = 0;
        }
    }

    /// <summary>
    /// Çeşit Ödeme bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class CesitliOdeme
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int surecYil { get; set; }

        /// <summary>
        /// muhasebeKod: 
        /// </summary>
        public int muhasebeKod { get; set; }

        /// <summary>
        /// birimKod: 
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// surecNo: 
        /// </summary>
        public string surecNo { get; set; }

        /// <summary>
        /// tcKimlikNo: 
        /// </summary>
        public string tcKimlikNo { get; set; }

        /// <summary>
        /// surecYil: 
        /// </summary>
        public int hakedisYil { get; set; }

        /// <summary>
        /// surecAy: 
        /// </summary>
        public int hakedisAy { get; set; }

        /// <summary>
        /// odemeTur: 
        /// </summary>
        public int odemeTur { get; set; }

        /// <summary>
        /// siraNo: 
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// sabitTutar: 
        /// </summary>
        public decimal sabitTutar { get; set; }

        /// <summary>
        /// carpan: 
        /// </summary>
        public decimal carpan { get; set; }

        /// <summary>
        /// ekTutar: 
        /// </summary>
        public decimal ekTutar { get; set; }


        public decimal toplamTutar
        {
            get
            {
                return carpan * sabitTutar + ekTutar;
            }
        }

        /// <summary>
        /// gelirVergi: 
        /// </summary>
        public decimal gelirVergi { get; set; }

        /// <summary>
        /// damgaVergi: 
        /// </summary>
        public decimal damgaVergi { get; set; }

        /// <summary>
        /// digerVergi: 
        /// </summary>
        public decimal digerVergi { get; set; }

        /// <summary>
        /// toplamVergi: 
        /// </summary>
        public decimal toplamVergi { get { return digerVergi + damgaVergi + gelirVergi; } }

        /// <summary>
        /// odenecekTutar: 
        /// </summary>
        public decimal odenecekTutar
        {
            get
            {
                return toplamTutar - toplamVergi;
            }
        }

        /// <summary>
        /// aciklama: 
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// adSoyad: 
        /// </summary>
        public string adSoyad { get; set; }

        /// <summary>
        /// unvan: 
        /// </summary>
        public string unvan { get; set; }

        /// <summary>
        /// odemeEmriNo: 
        /// </summary>
        public string odemeEmriNo { get; set; }

        /// <summary>
        /// avansTutar: 
        /// </summary>
        public decimal avansTutar { get; set; }

        /// <summary>
        /// avansOdemeEmriNo: 
        /// </summary>
        public string avansOdemeEmriNo { get; set; }

        /// <summary>
        /// tcKimlikNo: 
        /// </summary>
        public string ibanNo { get; set; }
        public int paraBirimi { get; set; }

        /// <summary>
        /// <see cref="CesitliOdeme"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public CesitliOdeme()
        {
            surecYil = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            surecNo = string.Empty;
            tcKimlikNo = string.Empty;
            hakedisYil = 0;
            hakedisAy = 0;
            odemeTur = 0;
            siraNo = 0;
            sabitTutar = (decimal)0;
            carpan = (decimal)0;
            ekTutar = (decimal)0;
            gelirVergi = (decimal)0;
            damgaVergi = (decimal)0;
            digerVergi = (decimal)0;
            aciklama = string.Empty;
            adSoyad = string.Empty;
            unvan = string.Empty;
            odemeEmriNo = string.Empty;
            avansTutar = (decimal)0;
            avansOdemeEmriNo = string.Empty;
            paraBirimi = 0;
        }

        public TNSDateTime avansOdemeGun;
        public int avansOdemeSiraNo;

        public TNSDateTime blGun;
        public int blSiraNo;

    }


    /// <summary>
    /// Çeşit Ödeme bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class MahkemeHarc
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int surecYil { get; set; }

        /// <summary>
        /// muhasebeKod: 
        /// </summary>
        public int muhasebeKod { get; set; }

        /// <summary>
        /// birimKod: 
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// surecNo: 
        /// </summary>
        public string surecNo { get; set; }

        public string birimAciklama { get; set; }
        public string davaci { get; set; }
        public string davaKonusu { get; set; }
        public string mahkemeKararNo { get; set; }
        public string icraTakipDurumu { get; set; }
        public string yargitayDanistayKarari { get; set; }
        public string kesinlesmisMahkemeKarari { get; set; }
        public string kesinlestiMi { get; set; }
        public double miktar { get; set; }

        /// <summary>
        /// odemeEmriNo: 
        /// </summary>
        public string odemeEmriNo { get; set; }

        /// <summary>
        /// <see cref="CesitliOdeme"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MahkemeHarc()
        {
            surecYil = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            surecNo = string.Empty;
            birimAciklama = string.Empty;
            davaci = string.Empty;
            davaKonusu = string.Empty;
            mahkemeKararNo = string.Empty;
            icraTakipDurumu = string.Empty;
            yargitayDanistayKarari = string.Empty;
            kesinlesmisMahkemeKarari = string.Empty;
            kesinlestiMi = string.Empty;
            miktar = 0;
        }
    }


    /// <summary>
    /// İşçi Bordro Tanım bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IsciBordroTanim
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Tanım Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Tanım Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Yevmiye Tutar
        /// </summary>
        public decimal yevmiye;
        /// <summary>
        /// Asgari Geçim İndirim Tutar
        /// </summary>
        public decimal asgariGecimIndirim;
        /// <summary>
        /// Özel Sigorta Tutar
        /// </summary>
        public decimal ozelSigorta;
        /// <summary>
        /// Sakatlık İndirimi Tutar
        /// </summary>
        public decimal sakatlikIndirimi;
        /// <summary>
        /// Prim Tutar
        /// </summary>
        public decimal prim;
        /// <summary>
        /// Sosyal Yardım Tutar
        /// </summary>
        public decimal sosyalYardim;
        /// <summary>
        /// Taşıt Yardım Tutar
        /// </summary>
        public decimal tasitYardimi;
        /// <summary>
        /// Yemek Yardım Tutar
        /// </summary>
        public decimal yemekYardimi;
        /// <summary>
        /// Sendika Aidatı Tutar
        /// </summary>
        public decimal sendikaAidati;
        /// <summary>
        /// Kümülatif Vergi Matrah Başlangıç
        /// </summary>
        public decimal kumulatifVergiMatrahiBaslangic;

        /// <summary>
        /// <see cref="IsciBordroTanim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IsciBordroTanim()
        {
            muhasebe = 0;
            birim = string.Empty;
            yil = 0;
            ay = 0;
            tcKimlikNo = string.Empty;
            asgariGecimIndirim = 0;
            yevmiye = 0;
            sakatlikIndirimi = 0;
            ozelSigorta = 0;
            prim = 0;
            sosyalYardim = 0;
            tasitYardimi = 0;
            yemekYardimi = 0;
            sendikaAidati = 0;
            kumulatifVergiMatrahiBaslangic = 0;
        }
    }

    /// <summary>
    /// İşçi Maaş Bordro bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class IsciMaasBordro
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Maaş Yıl
        /// </summary>
        public int maasYil;
        /// <summary>
        /// Maaş Ay
        /// </summary>
        public int maasAy;
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string tCKimlikNo;
        /// <summary>
        /// Bordro Tip
        /// </summary>
        public int bordroTip;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Personel Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Personel Soyad
        /// </summary>
        public string soyad;
        /// <summary>
        /// Personel Sicil No
        /// </summary>
        public string sicilNo;
        //Banka
        /// <summary>
        /// Personel Banka Kod
        /// </summary>
        public string bankaKodu;
        /// <summary>
        /// Personel Banka Şube Kod
        /// </summary>
        public string bankaSubeKodu;
        /// <summary>
        /// Personel Banka Hesap No
        /// </summary>
        public string bankaHesapNo;
        /// <summary>
        /// Personel Banka Ad
        /// </summary>
        public string bankaAdi;
        /// <summary>
        /// Personel Banka Şube Ad
        /// </summary>
        public string bankaSubeAdi;
        /// <summary>
        /// Ay içinde çalıştığı gün sayısı
        /// </summary>
        public int gun;
        /// <summary>
        /// Yevmiye Tutar
        /// </summary>
        public decimal yevmiye;
        /// <summary>
        /// Ücret
        /// </summary>
        public decimal ucret;
        /// <summary>
        /// Sakatlık İndirimi
        /// </summary>
        public decimal sakatlikIndirimi;
        /// <summary>
        /// Özel Sigorta
        /// </summary>
        public decimal ozelSigorta;
        /// <summary>
        /// Prim
        /// </summary>
        public decimal prim;
        /// <summary>
        /// Sosyal Yardım
        /// </summary>
        public decimal sosyalYardim;
        /// <summary>
        /// Taşıt yardımı yapılacak gün sayısı
        /// </summary>
        public int tasitGunu;
        /// <summary>
        /// Taşıt Yardımı Tutar
        /// </summary>
        public decimal tasitYardimi;
        /// <summary>
        /// Yemek yardımı yapılacak gün sayısı
        /// </summary>
        public int yemekGunu;
        /// <summary>
        /// Yemek Yardımı Tutar
        /// </summary>
        public decimal yemekYardimi;
        //hakediş
        /// <summary>
        /// Hakediş SSK İşveren Payı
        /// </summary>
        public decimal hakSskIsverenPayi;
        /// <summary>
        /// Hakediş SSK Primine Esas Kazanç
        /// </summary>
        public decimal hakSskPrimineEsasKazanc;
        /// <summary>
        /// Hakediş İşsizlik Sigorta İşveren Payı
        /// </summary>
        public decimal hakIssizlikSigIsverenPayi;
        /// <summary>
        /// Hakediş Toplam Kazanç
        /// </summary>
        public decimal hakToplamKazanc;
        /// <summary>
        /// Kesilen Damga Vergisi Matrah
        /// </summary>
        public decimal kesDamgaVergisiMatrah;
        /// <summary>
        /// Kesilen Gelir Vergisi Matrah
        /// </summary>
        public decimal kesGelirVergisiMatrah;
        /// <summary>
        /// Kesilen Gelir Vergisi Tutar
        /// </summary>
        public decimal kesGelirVergisi;
        /// <summary>
        /// Gelir Vergisi Oran
        /// </summary>
        public decimal gelirVergisiOrani;
        /// <summary>
        /// Kesilen Damga Vergisi tutar
        /// </summary>
        public decimal kesDamgaVergisi;
        /// <summary>
        /// Damga Vergisi Oran
        /// </summary>
        public decimal damgaVergisiOrani;
        /// <summary>
        /// Kesilen SSK Prim İşveren Payı
        /// </summary>
        public decimal kesSskPrimIsverenPayi;
        /// <summary>
        /// SSK İşveren Oran
        /// </summary>
        public decimal sskIsverenOran;
        /// <summary>
        /// Kesilen SSK Prim İşçi Payı
        /// </summary>
        public decimal kesSskPrimIsciPayi;
        /// <summary>
        /// SSK İşçi Oran
        /// </summary>
        public decimal sskIsciOran;
        /// <summary>
        /// Kesilen İşsizlik Sigorta İşveren Payı
        /// </summary>
        public decimal kesIssizSigIsverenPayi;
        /// <summary>
        /// İşsizlik Sigorta İşveren Oran
        /// </summary>
        public decimal issizlikSigIsverenOran;
        /// <summary>
        /// Kesilen İşsizlik Sigorta İşçi Payı
        /// </summary>
        public decimal kesIssizSigIsciPayi;
        /// <summary>
        /// İşsizlik Sigorta İşçi Oran
        /// </summary>
        public decimal issizSigIsciOran;
        /// <summary>
        /// Sendika Aidat Tutar
        /// </summary>
        public decimal sendikaAidat;
        /// <summary>
        /// Brüt Asgari Ücret
        /// </summary>
        public decimal brutAsgariUcret;
        /// <summary>
        /// Asgari Geçim İndirim
        /// </summary>
        public decimal asgariGecimIndirim;
        /// <summary>
        /// Asgari Geçim İndirimi Katsayı
        /// </summary>
        public decimal asgGecimIndKatsayi;
        /// <summary>
        /// Kümülatif Vergi Matrahi
        /// </summary>
        public decimal kumulatifVergiMatrahi;
        /// <summary>
        /// Taşıt Yardım Katsayı
        /// </summary>
        public decimal tasitYardimKatsayi;
        /// <summary>
        /// Yemek Yardım Katsayı
        /// </summary>
        public decimal yemekYardimKatsayi;
        /// <summary>
        /// Toplam Kesinti
        /// </summary>
        public decimal toplamKesinti;
        /// <summary>
        /// Net Ödenen
        /// </summary>
        public decimal netOdenen;
        /// <summary>
        /// Personel Unvan Kod
        /// </summary>
        public int unvanKodu;
        /// <summary>
        /// Personel Unvan Ad
        /// </summary>
        public string unvanAdi;
        /// <summary>
        /// Personel Medeni Durum
        /// </summary>
        public int medeniDurumu;
        /// <summary>
        /// Fark Bordrosu Mu
        /// </summary>
        public int farkmi;
        /// <summary>
        /// Diğer Kesinti Tutar
        /// </summary>
        public decimal digerKesinti;

        /// <summary>
        /// <see cref="IsciMaasBordro"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public IsciMaasBordro()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            maasYil = 0;
            maasAy = 0;
            siraNo = 0;
            tCKimlikNo = string.Empty;
            bordroTip = 0;
            ad = string.Empty;
            soyad = string.Empty;
            sicilNo = string.Empty;
            gun = 0;
            yevmiye = 0;
            ucret = 0;
            sakatlikIndirimi = 0;
            ozelSigorta = 0;
            prim = 0;
            sosyalYardim = 0;
            tasitGunu = 0;
            tasitYardimi = 0;
            yemekGunu = 0;
            yemekYardimi = 0;

            hakSskIsverenPayi = 0;
            hakSskPrimineEsasKazanc = 0;
            hakIssizlikSigIsverenPayi = 0;
            hakToplamKazanc = 0;
            kesDamgaVergisiMatrah = 0;
            kesGelirVergisiMatrah = 0;
            kesGelirVergisi = 0;
            gelirVergisiOrani = 0;
            kesDamgaVergisi = 0;
            damgaVergisiOrani = 0;
            kesSskPrimIsverenPayi = 0;
            sskIsverenOran = 0;
            kesSskPrimIsciPayi = 0;
            sskIsciOran = 0;
            kesIssizSigIsverenPayi = 0;
            issizlikSigIsverenOran = 0;
            kesIssizSigIsciPayi = 0;
            issizSigIsciOran = 0;
            sendikaAidat = 0;
            brutAsgariUcret = 0;
            asgariGecimIndirim = 0;
            asgGecimIndKatsayi = 0;
            toplamKesinti = 0;
            netOdenen = 0;

            //eklenen
            kumulatifVergiMatrahi = 0;
            tasitYardimKatsayi = 0;
            yemekYardimKatsayi = 0;

            //Banka
            bankaKodu = string.Empty;
            bankaSubeKodu = string.Empty;
            bankaHesapNo = string.Empty;
            bankaAdi = string.Empty;
            bankaSubeAdi = string.Empty;

            unvanKodu = 0;
            unvanAdi = string.Empty;
            medeniDurumu = 0;
            farkmi = 0;
            digerKesinti = 0;

        }
    }

    /// <summary>
    /// Abone Fatura Telefon bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneFaturaTelefon
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fatura Yıl
        /// </summary>
        public int faturaYil;
        /// <summary>
        /// Fatura Ay
        /// </summary>
        public int faturaAy;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Vergi Kimlik No
        /// </summary>
        public string vkn;
        /// <summary>
        /// Ad Soyad Unvan
        /// </summary>
        public string adSoyadUnvan;
        /// <summary>
        /// Adres
        /// </summary>
        public string adres;
        /// <summary>
        /// Hizmet No
        /// </summary>
        public string hizmetNo;
        /// <summary>
        /// Döküman No
        /// </summary>
        public string docNo;
        /// <summary>
        /// Tarih   
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Son Ödeme Tarih
        /// </summary>
        public TNSDateTime sonOdemeTarih;
        /// <summary>
        /// Fatura No
        /// </summary>
        public string faturaNo;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Para Birim Kod
        /// </summary>
        public string paraBirimKod;
        /// <summary>
        /// Mal Hizmet Toplam Tutar
        /// </summary>
        public decimal malHizTopTutar;
        /// <summary>
        /// Toplam İskonto
        /// </summary>
        public decimal topIskonto;
        /// <summary>
        /// KDV Oran
        /// </summary>
        public decimal kdvOran;
        /// <summary>
        /// KDV Tutar
        /// </summary>
        public decimal kdvTutar;
        /// <summary>
        /// KDV Toplam Tutar
        /// </summary>
        public decimal kdvTopTutar;
        /// <summary>
        /// Diğer Vergi Fon Kod
        /// </summary>
        public string digVerFonKod;
        /// <summary>
        /// Diğer Vergi Fon Oran
        /// </summary>
        public decimal digVerFonOran;
        /// <summary>
        /// Diğer Vergi Fon Tutar
        /// </summary>
        public decimal digVerFonTutar;
        /// <summary>
        /// Diğer Vergi Fon Toplam Tutar
        /// </summary>
        public decimal dvfTopTutar;
        /// <summary>
        /// Fatura Toplam Tutar
        /// </summary>
        public decimal fatTopTutar;
        /// <summary>
        /// Ödenecek Tutar
        /// </summary>
        public decimal odnTutar;
        /// <summary>
        /// Aylık Ücret
        /// </summary>
        public decimal aylikUcret;
        /// <summary>
        /// Görüşme
        /// </summary>
        public decimal gorusme;
        /// <summary>
        /// Kayıtlı 
        /// </summary>
        public decimal kayitli;
        /// <summary>
        /// Ayrıntı
        /// </summary>
        public decimal ayrinti;
        /// <summary>
        /// Fonotel
        /// </summary>
        public decimal fonotel;
        /// <summary>
        /// Gecikme
        /// </summary>
        public decimal gecikme;
        /// <summary>
        /// Kapatılma
        /// </summary>
        public decimal kapatilma;
        /// <summary>
        /// Bağlantı
        /// </summary>
        public decimal baglanti;
        /// <summary>
        /// Fatura Edilmemiş
        /// </summary>
        public decimal faturaEdilmemis;
        /// <summary>
        /// Çeşitli
        /// </summary>
        public decimal cesitli;
        /// <summary>
        /// Damga
        /// </summary>
        public decimal damga;
        /// <summary>
        /// Şehiriçi
        /// </summary>
        public decimal sehirIci;
        /// <summary>
        /// Şehirdışı
        /// </summary>
        public decimal sehirDisi;
        /// <summary>
        /// Milletlerarası
        /// </summary>
        public decimal milletlerArasi;
        /// <summary>
        /// Ücretli Servisler
        /// </summary>
        public decimal ucretliServisler;
        /// <summary>
        /// İnternet
        /// </summary>
        public decimal internet;
        /// <summary>
        /// GSM
        /// </summary>
        public decimal gsm;
        /// <summary>
        /// Sesli Mesaj Aylık Ücret
        /// </summary>
        public decimal sesliMesajAylikUcret;
        /// <summary>
        /// SMS Ücret
        /// </summary>
        public decimal smsUcret;
        /// <summary>
        /// Sesli Mesaj Ücreti
        /// </summary>
        public decimal sesliMesajUcreti;
        /// <summary>
        /// Aile Kart Ücreti
        /// </summary>
        public decimal aileKartUcreti;
        /// <summary>
        /// Cihaz Ücreti
        /// </summary>
        public decimal cihazUcreti;
        /// <summary>
        /// Mail    
        /// </summary>
        public string mail;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;

        /// <summary>
        /// <see cref="AboneFaturaTelefon"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneFaturaTelefon()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            faturaYil = 0;
            faturaAy = 0;
            tcKimlikNo = string.Empty;
            siraNo = 0;
            tcKimlikNo = string.Empty;
            vkn = string.Empty;
            adSoyadUnvan = string.Empty;
            adres = string.Empty;
            hizmetNo = string.Empty;
            docNo = string.Empty;
            tarih = new TNSDateTime();
            sonOdemeTarih = new TNSDateTime();
            faturaNo = string.Empty;
            aciklama = string.Empty;
            paraBirimKod = string.Empty;
            malHizTopTutar = 0;
            topIskonto = 0;
            kdvOran = 0;
            kdvTutar = 0;
            kdvTopTutar = 0;
            digVerFonKod = string.Empty;
            digVerFonOran = 0;
            digVerFonTutar = 0;
            dvfTopTutar = 0;
            fatTopTutar = 0;
            odnTutar = 0;
            aylikUcret = 0;
            gorusme = 0;
            kayitli = 0;
            ayrinti = 0;
            fonotel = 0;
            gecikme = 0;
            kapatilma = 0;
            baglanti = 0;
            faturaEdilmemis = 0;
            cesitli = 0;
            damga = 0;
            sehirIci = 0;
            sehirDisi = 0;
            milletlerArasi = 0;
            ucretliServisler = 0;
            internet = 0;
            gsm = 0;
            sesliMesajAylikUcret = 0;
            smsUcret = 0;
            sesliMesajUcreti = 0;
            aileKartUcreti = 0;
            cihazUcreti = 0;
            muhasebe = 0;
            birim = string.Empty;
            mail = string.Empty;
            odemeEmriNo = string.Empty;
        }
    }

    /// <summary>
    /// Telekom Fatura Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TelekomFaturaDetay
    {
        /// <summary>
        /// Fatura Yıl
        /// </summary>
        public int faturaYil;
        /// <summary>
        /// Fatura Ay
        /// </summary>
        public int faturaAy;
        /// <summary>
        /// Hizmet No
        /// </summary>
        public string hizmetNo;
        /// <summary>
        /// Sıra
        /// </summary>
        public int sira;
        /// <summary>
        /// Arama Zamanı
        /// </summary>
        public TNSDateTime aramaZamani;
        /// <summary>
        /// Aranan No
        /// </summary>
        public string arananNo;
        /// <summary>
        /// Detay Ücret
        /// </summary>
        public decimal detayUcret;

        /// <summary>
        /// <see cref="TelekomFaturaDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TelekomFaturaDetay()
        {
            faturaYil = 0;
            faturaAy = 0;
            hizmetNo = string.Empty;
            sira = 0;
            aramaZamani = new TNSDateTime();
            arananNo = string.Empty;
            detayUcret = 0;
        }
    }

    /// <summary>
    /// Telekom E-Mail Liste
    /// </summary>
    [Serializable()]
    public class TelekomEmailListe
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Mail
        /// </summary>
        public string mail;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fatura nesne koleksiyonunu tutan özellik
        /// </summary>
        public ObjectArray fatura;

        /// <summary>
        /// <see cref="TelekomEmailListe"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TelekomEmailListe()
        {
            muhasebe = 0;
            birim = string.Empty;
            mail = string.Empty;
            odemeEmriNo = string.Empty;
            surecNo = string.Empty;
            fatura = new ObjectArray();
        }
    }

    /// <summary>
    /// Elektrik Email Liste bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class ElektrikEmailListe
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Mail
        /// </summary>
        public string mail;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fatura nesne koleksiyonunu tutan özellik
        /// </summary>
        public ObjectArray fatura;

        /// <summary>
        /// <see cref="ElektrikEmailListe"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ElektrikEmailListe()
        {
            muhasebe = 0;
            birim = string.Empty;
            mail = string.Empty;
            odemeEmriNo = string.Empty;
            surecNo = string.Empty;
            fatura = new ObjectArray();
        }
    }

    /// <summary>
    /// Su Fatura Email Liste bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class SuEmailListe
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Mail
        /// </summary>
        public string mail;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fatura nesne koleksiyonunu tutan özellik
        /// </summary>
        public ObjectArray fatura;

        /// <summary>
        /// <see cref="SuEmailListe"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SuEmailListe()
        {
            muhasebe = 0;
            birim = string.Empty;
            mail = string.Empty;
            odemeEmriNo = string.Empty;
            surecNo = string.Empty;
            fatura = new ObjectArray();
        }
    }

    /// <summary>
    /// Rapor Kriter bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RaporKriter
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// İşlem Kod
        /// </summary>
        public int islemKodu;
        /// <summary>
        /// Başlama Tarih
        /// </summary>
        public TNSDateTime baslamaTarih;
        /// <summary>
        /// Bitiş Tarih
        /// </summary>
        public TNSDateTime bitisTarih;
        /// <summary>
        /// Rapor Tip
        /// </summary>
        public int raporTipi;
        /// <summary>
        /// Belge Tur
        /// </summary>
        public int belgeTuru;
        /// <summary>
        /// Rapor Şekli
        /// </summary>
        public int raporSekli;
        /// <summary>
        /// Rapor Grup By
        /// </summary>
        public int raporGrupBy;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurumsalKod;
        /// <summary>
        /// Ekonomik Kod
        /// </summary>
        public string ekonomikKod;
        /// <summary>
        /// Taşınır Hesap Kod
        /// </summary>
        public string tasinirHesapKod;
        /// <summary>
        /// Vergi No
        /// </summary>
        public string vergiNo;
        /// <summary>
        /// Parça Kod 1
        /// </summary>
        public int parcaKod1;
        /// <summary>
        /// Parça Kod 2
        /// </summary>
        public int parcaKod2;
        /// <summary>
        /// Parça Durum 1
        /// </summary>
        public int parcaDurum1;
        /// <summary>
        /// Parça Durum 2
        /// </summary>
        public int parcaDurum2;
        /// <summary>
        /// Talimat Mı
        /// </summary>
        public int talimatmi;//onaymi
        /// <summary>
        /// Abone Tip
        /// </summary>
        public int aboneTip;
        /// <summary>
        /// İl kod
        /// </summary>
        public int il;
        /// <summary>
        /// Ödeme Emri Belge Göster
        /// </summary>
        public bool odemeEmriGoster;

        /// <summary>
        /// <see cref="RaporKriter"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RaporKriter()
        {
            yil = 0;
            ay = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            islemKodu = 0;
            baslamaTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();
            belgeTuru = 0;
            raporTipi = 0;
            raporSekli = 0;
            raporGrupBy = 0;
            kurumsalKod = string.Empty;
            ekonomikKod = string.Empty;
            tasinirHesapKod = string.Empty;
            vergiNo = string.Empty;
            parcaKod1 = 0;
            parcaKod2 = 0;
            parcaDurum1 = 0;
            parcaDurum2 = 0;
            talimatmi = 0;
            aboneTip = 0;
            il = -1;
            odemeEmriGoster = false;
        }
    }

    /// <summary>
    /// Rapor Kriter bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RaporKriterv2
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç no
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// İşlem Kod
        /// </summary>
        public int islemKodu;
        /// <summary>
        /// Vergi No
        /// </summary>
        public string vergiNo;


        /// <summary>
        /// <see cref="RaporKriter"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RaporKriterv2()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebeKod = 0;
            birimKod = string.Empty;
            islemKodu = 0;
            vergiNo = string.Empty;
        }
    }


    /// <summary>
    /// Rapor Kriter bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RaporKriterv2Sonuc
    {
        public int yil;
        public string surecNo;
        public int muhasebeKod;
        public string muhasebeAd;
        public decimal yaklasikMaliyet;
        public string firmaVergiNo;
        public string firmaAd;
        public decimal firmaTeklifTutar;
        public decimal sozlesmeBedel;
        public decimal yapilanOdeme;
        public int teklifTur;


        /// <summary>
        /// <see cref="RaporKriter"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RaporKriterv2Sonuc()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebeKod = 0;
            muhasebeAd = string.Empty;
            yaklasikMaliyet = decimal.Zero;
            firmaVergiNo = string.Empty;
            firmaAd = string.Empty;
            firmaTeklifTutar = decimal.Zero;
            sozlesmeBedel = decimal.Zero;
            yapilanOdeme = decimal.Zero;
            teklifTur = 1;
        }
    }

    /// <summary>
    /// Rapor Abonelik İstatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRAbonelikIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// İl Kod
        /// </summary>
        public int il;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// İl Ad
        /// </summary>
        public string ilAd;
        /// <summary>
        /// Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Tutar
        /// </summary>
        public double tutar;
        /// <summary>
        /// Abone Sayısı
        /// </summary>
        public int aboneSayisi;
        /// <summary>
        /// Ortalama
        /// </summary>
        public double ortalama;
        /// <summary>
        /// Personel Sayısı
        /// </summary>
        public int personelSayisi;
        /// <summary>
        /// Değer 1
        /// </summary>
        public int deger1;

        /// <summary>
        /// <see cref="RPRAbonelikIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRAbonelikIstatistik()
        {
            yil = 0;
            ay = 0;
            muhasebe = 0;
            il = 0;
            birim = string.Empty;
            tutar = 0.0;
            aboneSayisi = 0;
            ortalama = 0.0;
            personelSayisi = 0;
            muhasebeAd = string.Empty;
            ilAd = string.Empty;
            birimAd = string.Empty;
            deger1 = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class RPRMalHizmetYapim
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Süreç Tarih
        /// </summary>
        public TNSDateTime surecTarih;
        /// <summary>
        /// Aşama
        /// </summary>
        public int asama;
        /// <summary>
        /// Aşama Ad
        /// </summary>
        public string asamaAd;
        /// <summary>
        /// Onay Tarih
        /// </summary>
        public TNSDateTime onayTarih;
        /// <summary>
        /// Onay Tutar
        /// </summary>
        public double onayTutar;
        /// <summary>
        /// Firma bilgisini <see cref="FirmaBilgisi"/> nesnesi ile tutan özellik
        /// </summary>
        public FirmaBilgisi firma;
        /// <summary>
        /// Alım Tutar
        /// </summary>
        public double alimTutar;
        /// <summary>
        /// Fatura Tarih
        /// </summary>
        public TNSDateTime faturaTarih;
        /// <summary>
        /// Fatura Tutar
        /// </summary>
        public double faturaTutar;
        /// <summary>
        /// Tahakkuk Tarih
        /// </summary>
        public TNSDateTime tahhakkukTarih;
        /// <summary>
        /// Tahakkuk Tutar
        /// </summary>
        public double tahakkukTutar;
        /// <summary>
        /// Faturalar
        /// </summary>
        public ObjectArray faturalar;
        /// <summary>
        /// Ödeme Emri Bilgileri
        /// </summary>
        public ObjectArray odemeEmirleri;
        /// <summary>
        /// Firmalar
        /// </summary>
        public ObjectArray firmalar;
        /// <summary>
        /// Onay form bilgilerini <see cref="OnayForm"/> nesnesi ile tutan özellik
        /// </summary>
        public OnayForm onay;

        /// <summary>
        /// <see cref="RPRMalHizmetYapim"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRMalHizmetYapim()
        {
            muhasebe = 0;
            birim = string.Empty;
            muhasebeAd = string.Empty;
            birimAd = string.Empty;
            surecNo = string.Empty;
            surecTarih = new TNSDateTime();
            asama = 0;
            asamaAd = string.Empty;
            onayTarih = new TNSDateTime();
            onayTutar = 0.0;
            firma = new FirmaBilgisi();
            alimTutar = 0.0;
            faturaTarih = new TNSDateTime();
            faturaTutar = 0.0;
            tahhakkukTarih = new TNSDateTime();
            tahakkukTutar = 0.0;
            faturalar = new ObjectArray();
            odemeEmirleri = new ObjectArray();
            firmalar = new ObjectArray();
            onay = new OnayForm();
        }
    }

    /// <summary>
    /// Rapor Mal Hizmet Yapım İşi Harcama Takip Özet bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRMalHizmetYapimIsiHRCTakipOzet
    {
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurumsalKod;
        /// <summary>
        /// Muhasebe Sayısı
        /// </summary>
        public int muhasebeSayi;
        /// <summary>
        /// Birim Sayısı
        /// </summary>
        public int birimSayi;
        /// <summary>
        /// Toplam İşlem Sayısı
        /// </summary>
        public int toplamIslemSayi;
        /// <summary>
        /// Alım Çeşidi
        /// </summary>
        public int alimCesidi;
        /// <summary>
        /// Onay İşlem Sayısı
        /// </summary>
        public int onayIslemSayi;
        /// <summary>
        /// Onay Tutar
        /// </summary>
        public decimal onayTutar;
        /// <summary>
        /// ALım İşlem Sayı
        /// </summary>
        public int alimIslemSayi;
        /// <summary>
        /// Alım Toplam Tutar
        /// </summary>
        public decimal alimToplamTutar;
        /// <summary>
        /// Fatura İşlem Sayı
        /// </summary>
        public int faturaIslemSayi;
        /// <summary>
        /// Fatura Toplam Tutar
        /// </summary>
        public decimal faturaToplamTutar;
        /// <summary>
        /// Ödeme Sayısı
        /// </summary>
        public int odemeSayi;
        /// <summary>
        /// Ödeme Tahakkuk Toplam Tutar
        /// </summary>
        public decimal odemeTahakkukToplam;
        /// <summary>
        /// Ödeme Toplam
        /// </summary>
        public decimal odemeToplam;


        /// <summary>
        /// <see cref="RPRMalHizmetYapimIsiHRCTakipOzet"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRMalHizmetYapimIsiHRCTakipOzet()
        {

            kurumsalKod = string.Empty;
            muhasebeSayi = 0;
            birimSayi = 0;
            toplamIslemSayi = 0;
            alimCesidi = 0;
            onayIslemSayi = 0;
            onayTutar = 0;
            alimIslemSayi = 0;
            alimToplamTutar = 0;
            faturaIslemSayi = 0;
            faturaToplamTutar = 0;
            odemeSayi = 0;
            odemeTahakkukToplam = 0;
            odemeToplam = 0;

        }
    }

    /// <summary>
    /// Rapor Mal Hizmet Yapım İşi Harcama Takip Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRMalHizmetYapimIsiHRCTakipDetay
    {
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAdi;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Ad
        /// </summary>
        public string birimAdi;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Süreç Aşama
        /// </summary>
        public string surecAsama;
        /// <summary>
        /// Onay Tarihi
        /// </summary>
        public TNSDateTime onayTarihi;
        /// <summary>
        /// Yaklaşık Maliyet Tutar
        /// </summary>
        public decimal yaklasikMaliyet;
        /// <summary>
        /// Firma Kod
        /// </summary>
        public int firmaKodu;
        /// <summary>
        /// Firma Ad
        /// </summary>
        public string firmaAdi;
        /// <summary>
        /// Alım Tarihi
        /// </summary>
        public TNSDateTime alimTarihi;
        /// <summary>
        /// Alım Tutar
        /// </summary>
        public decimal alimTutari;
        /// <summary>
        /// Fatura Tarih
        /// </summary>
        public TNSDateTime faturaTarihi;
        /// <summary>
        /// Fatura Tutar
        /// </summary>
        public decimal faturaTutari;
        /// <summary>
        /// Ödeme Tarihi
        /// </summary>
        public TNSDateTime odemeTarihi;
        /// <summary>
        /// Ödeme Tahakkuk Tutar
        /// </summary>
        public decimal odemeTahakkukTutar;
        /// <summary>
        /// Ödeme Tutar
        /// </summary>
        public decimal odemeTutar;
        /// <summary>
        /// Eko Kod Onay
        /// </summary>
        public string ekoKodOnay;
        /// <summary>
        /// Ödeme Emri Belge Ekonomik Kod
        /// </summary>
        public string ekoKodOdemeEmri;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;

        /// <summary>
        /// <see cref="RPRMalHizmetYapimIsiHRCTakipDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRMalHizmetYapimIsiHRCTakipDetay()
        {

            muhasebeAdi = string.Empty;
            muhasebeKod = 0;
            birimAdi = string.Empty;
            birimKod = string.Empty;
            surecNo = string.Empty;
            surecAsama = string.Empty;
            onayTarihi = new TNSDateTime();
            yaklasikMaliyet = 0;
            firmaKodu = 0;
            firmaAdi = string.Empty;
            alimTarihi = new TNSDateTime();
            alimTutari = 0;
            faturaTarihi = new TNSDateTime();
            faturaTutari = 0;
            odemeTarihi = new TNSDateTime();
            odemeTahakkukTutar = 0;
            odemeTutar = 0;
            ekoKodOnay = string.Empty;
            ekoKodOdemeEmri = string.Empty;
            odemeEmriNo = string.Empty;

        }
    }

    /// <summary>
    /// Rapor İhale Tur Toplam bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRIhaleTurToplam
    {
        /// <summary>
        /// İşlem Kod
        /// </summary>
        public int islemKod;
        /// <summary>
        /// İşlem Sayı
        /// </summary>
        public int islemSayi;
        /// <summary>
        /// İhale Usul
        /// </summary>
        public int ihaleUsul;
        /// <summary>
        /// İhale Usul Madde
        /// </summary>
        public int ihaleUsulMadde;
        /// <summary>
        /// İhale Ad
        /// </summary>
        public string ihaleAd;
        /// <summary>
        /// Tutar
        /// </summary>
        public double tutar;

        /// <summary>
        /// <see cref="RPRIhaleTurToplam"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRIhaleTurToplam()
        {
            islemKod = 0;
            ihaleUsul = 0;
            ihaleUsulMadde = 0;
            ihaleAd = string.Empty;
            islemSayi = 0;
            tutar = 0;
        }
    }

    /// <summary>
    /// Rapor İhale İl Toplam
    /// </summary>
    [Serializable()]
    public class RPRIhaleIlToplam
    {
        /// <summary>
        /// İşlem Kod
        /// </summary>
        public int islemKod;
        /// <summary>
        /// İşlem Sayı
        /// </summary>
        public int islemSayi;
        /// <summary>
        /// İl Kod
        /// </summary>
        public int ilKod;
        /// <summary>
        /// İl Ad
        /// </summary>
        public string ilAd;
        /// <summary>
        /// Tutar
        /// </summary>
        public double tutar;

        /// <summary>
        /// <see cref="RPRIhaleIlToplam"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRIhaleIlToplam()
        {
            islemKod = 0;
            ilKod = 0;
            ilAd = string.Empty;
            islemSayi = 0;
            tutar = 0;
        }
    }

    /// <summary>
    /// Rapor Sürekli Görev Yolluk Özet bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRSGYollukOzet
    {
        /// <summary>
        /// Emekli sayı
        /// </summary>
        public int emekliSayi;
        /// <summary>
        /// Atanmış Sayı
        /// </summary>
        public int atanmisSayi;
        /// <summary>
        /// Emekli Tutar
        /// </summary>
        public double emekliTutar;
        /// <summary>
        /// Atanmış Tutar
        /// </summary>
        public double atanmisTutar;
        /// <summary>
        /// İl Özet Listesi
        /// </summary>
        public RPRYollukIlOzet[] ilOzetListe;

        /// <summary>
        /// <see cref="RPRSGYollukOzet"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRSGYollukOzet()
        {
            emekliSayi = 0;
            atanmisSayi = 0;
            emekliTutar = 0;
            atanmisTutar = 0;

        }
    }

    /// <summary>
    /// Rapor Yolluk İl Özet bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRYollukIlOzet
    {
        /// <summary>
        /// İl Kod
        /// </summary>
        public int ilKod;
        /// <summary>
        /// İl Ad
        /// </summary>
        public string ilAd;
        /// <summary>
        /// İl Adet
        /// </summary>
        public int ilAdet;
        /// <summary>
        /// İl Tutar
        /// </summary>
        public double ilTutar;


        /// <summary>
        /// <see cref="RPRYollukIlOzet"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRYollukIlOzet()
        {
            ilKod = 0;
            ilAd = string.Empty;
            ilAdet = 0;
            ilTutar = 0;
        }
    }

    /// <summary>
    /// Rapor Ürün Mukayese Harcama Takip Özet bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRUrunMukayeseHRCTakipOzet
    {
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurumsalKod;
        /// <summary>
        /// Muhasebe Sayı
        /// </summary>
        public int muhasebeSayi;
        /// <summary>
        /// Birim Sayı
        /// </summary>
        public int birimSayi;
        /// <summary>
        /// Toplam İşlem Sayı
        /// </summary>
        public int toplamIslemSayi;
        /// <summary>
        /// Ürün Kod Ad
        /// </summary>
        public string urunKodAd;
        /// <summary>
        /// Ürün Miktarı
        /// </summary>
        public decimal urunMiktari;
        /// <summary>
        /// Ürün Ortalama Birim Fiyat
        /// </summary>
        public decimal urunOrtBirimFiyat;
        /// <summary>
        /// Ürün Toplam Tutar
        /// </summary>
        public decimal urunToplamTutari;
        /// <summary>
        /// En düşük birim fiyat Muhasebe Kod
        /// </summary>
        public int enDusukBirimFiyatMuhasebeKodu;
        /// <summary>
        /// En düşük birim fiyat Birim Kod
        /// </summary>
        public string enDusukBirimFiyatBirimKodu;
        /// <summary>
        /// En düşük birim fiyat
        /// </summary>
        public decimal enDusukBirimFiyat;
        /// <summary>
        /// En düşük birim fiyattan alınan miktar
        /// </summary>
        public decimal enDusukFiyattanAlinanMiktar;
        /// <summary>
        /// En yüksek birim fiyat Muhasebe Kod
        /// </summary>
        public int enYuksekBirimFiyatMuhasebeKodu;
        /// <summary>
        /// En yüksek birim fiyat birim kod
        /// </summary>
        public string enYuksekBirimFiyatBirimKodu;
        /// <summary>
        /// En yüksek birim fiyat
        /// </summary>
        public decimal enYuksekBirimFiyat;
        /// <summary>
        /// En yükse birim fiyattan alınan miktar
        /// </summary>
        public decimal enYuksekFiyattanAlinanMiktar;

        /// <summary>
        /// <see cref="RPRUrunMukayeseHRCTakipOzet"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRUrunMukayeseHRCTakipOzet()
        {
            kurumsalKod = string.Empty;
            muhasebeSayi = 0;
            birimSayi = 0;
            toplamIslemSayi = 0;
            urunKodAd = string.Empty;
            urunMiktari = 0;
            urunOrtBirimFiyat = 0;
            urunToplamTutari = 0;
            enDusukBirimFiyatMuhasebeKodu = 0;
            enDusukBirimFiyatBirimKodu = string.Empty;
            enDusukBirimFiyat = 0;
            enDusukFiyattanAlinanMiktar = 0;
            enYuksekBirimFiyatMuhasebeKodu = 0;
            enYuksekBirimFiyatBirimKodu = string.Empty;
            enYuksekBirimFiyat = 0;
            enYuksekFiyattanAlinanMiktar = 0;

        }
    }

    /// <summary>
    /// Rapor Ürün Mukayese Harcama Takip Detay
    /// </summary>
    [Serializable()]
    public class RPRUrunMukayeseHRCTakipDetay
    {
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAdi;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Ad
        /// </summary>
        public string birimAdi;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Ürün Adı
        /// </summary>
        public string urunAdi;
        /// <summary>
        /// Alış Tarih
        /// </summary>
        public TNSDateTime alisTarihi;
        /// <summary>
        /// Miktar
        /// </summary>
        public decimal miktar;
        /// <summary>
        /// Birim Fiyat
        /// </summary>
        public decimal birimFiyat;
        /// <summary>
        /// Toplam Tutar
        /// </summary>
        public decimal toplamTutar;
        /// <summary>
        /// Sapma Tutar
        /// </summary>
        public decimal sapmaTutari;
        /// <summary>
        /// Sapma Oran
        /// </summary>
        public decimal sapmaOrani;

        /// <summary>
        /// <see cref="RPRUrunMukayeseHRCTakipDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRUrunMukayeseHRCTakipDetay()
        {
            muhasebeAdi = string.Empty;
            muhasebeKod = 0;
            birimAdi = string.Empty;
            birimKod = string.Empty;
            surecNo = string.Empty;
            urunAdi = string.Empty;
            alisTarihi = new TNSDateTime();
            miktar = 0;
            birimFiyat = 0;
            toplamTutar = 0;
            sapmaTutari = 0;
            sapmaOrani = 0;
        }
    }
    /// <summary>
    /// Rapor İl Ekonomik Kod Bazlı Mukayese Harcama Takip Özet bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRIlEkonomikKodBazliMukayeseHRCTakipOzet
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurumsalKod;
        /// <summary>
        /// İl Kod
        /// </summary>
        public int ilKodu;
        /// <summary>
        /// İl Ad
        /// </summary>
        public string ilAdi;
        /// <summary>
        /// Personel Sayı
        /// </summary>
        public int personelSayi;
        /// <summary>
        /// Toplam Harc Tutar
        /// </summary>
        public decimal toplamHarcTutari;
        /// <summary>
        /// Kişi Başı Ortalama
        /// </summary>
        public decimal kisiBasiOrt;
        /// <summary>
        /// Toplam İçindeki Pay
        /// </summary>
        public decimal toplamIcindekiPayi;
        /// <summary>
        /// Ortalamadan Sapma
        /// </summary>
        public decimal ortalamadanSapma;

        /// <summary>
        /// <see cref="RPRIlEkonomikKodBazliMukayeseHRCTakipOzet"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRIlEkonomikKodBazliMukayeseHRCTakipOzet()
        {
            muhasebeKod = 0;
            muhasebeAd = string.Empty;
            birimKod = string.Empty;
            kurumsalKod = string.Empty;
            ilAdi = string.Empty;
            ilKodu = 0;
            personelSayi = 0;
            toplamHarcTutari = 0;
            kisiBasiOrt = 0;
            toplamIcindekiPayi = 0;
            ortalamadanSapma = 0;
        }
    }

    /// <summary>
    /// Rapor İl Ekonomik Kod Bazlı Mukayese Harcama Takip Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class RPRIlEkonomikKodBazliMukayeseHRCTakipDetay
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// İl Kod
        /// </summary>
        public int ilKodu;
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAdi;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kurumsalKod;
        /// <summary>
        /// İl Ad
        /// </summary>
        public string ilAdi;
        /// <summary>
        /// Personel Sayı
        /// </summary>
        public int personelSayi;
        /// <summary>
        /// Toplam Harç Tutarı
        /// </summary>
        public decimal toplamHarcTutari;
        /// <summary>
        /// Kişi Başı Ortalama
        /// </summary>
        public decimal kisiBasiOrt;
        /// <summary>
        /// Toplam İçindeki Payı
        /// </summary>
        public decimal toplamIcindekiPayi;
        /// <summary>
        /// Ortalamadan Sapma
        /// </summary>
        public decimal ortalamadanSapma;

        /// <summary>
        /// <see cref="RPRIlEkonomikKodBazliMukayeseHRCTakipDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public RPRIlEkonomikKodBazliMukayeseHRCTakipDetay()
        {
            muhasebeKod = 0;
            muhasebeAdi = string.Empty;
            kurumsalKod = string.Empty;
            ilKodu = 0;
            ilAdi = string.Empty;
            personelSayi = 0;
            toplamHarcTutari = 0;
            kisiBasiOrt = 0;
            toplamIcindekiPayi = 0;
            ortalamadanSapma = 0;
        }
    }

    /// <summary>
    /// Mal Hizmet Yapım Alım Tutar bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class MHYAlimTutari
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Alım Tutar
        /// </summary>
        public decimal alimTutari;
        /// <summary>
        /// Firma Kod
        /// </summary>
        public int firmaKod;
        /// <summary>
        /// Firma Ad
        /// </summary>
        public string firmaAd;
        /// <summary>
        /// Tarih
        /// </summary>
        public TNSDateTime tarih;

        /// <summary>
        /// <see cref="MHYAlimTutari"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MHYAlimTutari()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            alimTutari = 0;
            firmaKod = 0;
            firmaAd = string.Empty;
            tarih = new TNSDateTime();
        }
    }

    /// <summary>
    /// Kamu İhale Kurumu bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class KIKBilgileri
    {
        /// <summary>
        /// Silinecek Mi
        /// </summary>
        public bool sil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Kullanıcı Adı
        /// </summary>
        public string kullaniciAdi;
        /// <summary>
        /// Parola
        /// </summary>
        public string parola;

        /// <summary>
        /// <see cref="KIKBilgileri"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KIKBilgileri()
        {
            sil = false;
            muhasebe = 0;
            birim = string.Empty;
            kullaniciAdi = string.Empty;
            parola = string.Empty;
        }
    }

    /// <summary>
    /// İl bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Il
    {
        /// <summary>
        /// Kod
        /// </summary>
        public string kod { get; set; }
        /// <summary>
        /// Ad
        /// </summary>
        public string ad { get; set; }

        public string kodAd { get { return kod + " " + ad; } }

        /// <summary>
        /// <see cref="Il"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Il()
        {
            kod = string.Empty;
            ad = string.Empty;
        }
    }

    /// <summary>
    /// Ülke bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Ulke
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Para Birimi Kod
        /// </summary>
        public int paraBirimiKod;

        /// <summary>
        /// <see cref="Ulke"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Ulke()
        {
            kod = 0;
            ad = string.Empty;
            paraBirimiKod = 0;
        }
    }

    /// <summary>
    /// Abone No bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneNo
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Muhasebe  Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Abone No
        /// </summary>
        public string aboneNo;
        /// <summary>
        /// Abone Ad
        /// </summary>
        public string aboneAdi;
        /// <summary>
        /// Durum
        /// </summary>
        public int durum;
        /// <summary>
        /// İl Kod
        /// </summary>
        public string ilKodu;
        /// <summary>
        /// Abone Tip
        /// </summary>
        public int aboneTip;
        /// <summary>
        /// Firma bilgisini <see cref="FirmaBilgisi"/> nesnesi olarak tutan özellik
        /// </summary>
        public FirmaBilgisi firma;
        /// <summary>
        /// Kayıt Tarihi
        /// </summary>
        public TNSDateTime kayitTarihi;
        /// <summary>
        /// Kayıt Eden
        /// </summary>
        public string kayitEden;
        /// <summary>
        /// Başlama Tarih
        /// </summary>
        public TNSDateTime baslamaTarih;
        /// <summary>
        /// Bitiş Tarih
        /// </summary>
        public TNSDateTime bitisTarih;
        /// <summary>
        /// İşletme Kod
        /// </summary>
        public string isletmeKodu;

        public bool sadeceIlgili = false;
        /// <summary>
        /// <see cref="AboneNo"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneNo()
        {
            kod = 0;
            muhasebe = 0;
            muhasebeAd = string.Empty;
            birim = string.Empty;
            birimAd = string.Empty;
            aboneNo = string.Empty;
            aboneAdi = string.Empty;
            durum = -1;
            ilKodu = string.Empty;
            aboneTip = 0;
            firma = new FirmaBilgisi();
            kayitTarihi = new TNSDateTime();
            kayitEden = string.Empty;
            baslamaTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();
            isletmeKodu = string.Empty;
        }
    }

    /// <summary>
    /// Tedavi bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Tedavi
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Muhasebe Ad
        /// </summary>
        public string muhasebeAd;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Birim Ad
        /// </summary>
        public string birimAd;
        /// <summary>
        /// Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Firma bilgisini <see cref="FirmaBilgisi"/> nesnesi olarak tutan özellik
        /// </summary>
        public FirmaBilgisi firma;
        /// <summary>
        /// Tertip bilgisi
        /// </summary>
        public string tertip;
        /// <summary>
        /// Ödeme Hesabı
        /// </summary>
        public string odemeHesap;
        /// <summary>
        /// Tedavi detay bilgilerini <see cref="TedaviDetay"/> nesne koleksiyonu olarak tutan nesne
        /// </summary>
        public TNSCollection detay;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;

        /// <summary>
        /// <see cref="Tedavi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Tedavi()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            muhasebeAd = string.Empty;
            birim = string.Empty;
            birimAd = string.Empty;
            tip = 0;
            firma = new FirmaBilgisi();
            tertip = string.Empty;
            odemeHesap = string.Empty;
            detay = new TNSCollection();
            odemeEmriNo = string.Empty;
        }
    }

    /// <summary>
    /// Tedavi Detay bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TedaviDetay
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Fatura No
        /// </summary>
        public string faturaNo;
        /// <summary>
        /// Fatura Tarih
        /// </summary>
        public TNSDateTime faturaTarih;
        /// <summary>
        /// Fatura Tutar
        /// </summary>
        public double faturaTutar;
        /// <summary>
        /// Katılım Payı
        /// </summary>
        public double katilimPayi;

        /// <summary>
        /// <see cref="TedaviDetay"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TedaviDetay()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            tip = 0;
            siraNo = 0;
            faturaNo = string.Empty;
            faturaTarih = new TNSDateTime();
            faturaTutar = 0;
            katilimPayi = 0;
        }
    }
    /// <summary>
    /// Tedavi İstatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TedaviIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Tip
        /// </summary>
        public int tip;
        /// <summary>
        /// Personel sayısı
        /// </summary>
        public int perSayi;
        /// <summary>
        /// Ortalama Harcama
        /// </summary>
        public double ortHarcama;
        /// <summary>
        /// Toplam Harcama
        /// </summary>
        public double toplamHarcama;

        /// <summary>
        /// <see cref="TedaviIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TedaviIstatistik()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            tip = 0;
            perSayi = 0;
            ortHarcama = 0.0;
            toplamHarcama = 0.0;
        }
    }


    /// <summary>
    /// Personel Sayısı bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PersonelSayisi
    {
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Personel Sayısı
        /// </summary>
        public int personelSayisi;

        /// <summary>
        /// <see cref="PersonelSayisi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PersonelSayisi()
        {
            muhasebe = 0;
            birim = string.Empty;
            personelSayisi = 0;
        }
    }

    /// <summary>
    /// Personel Bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelBilgi
    {
        /// <summary>
        /// TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Ad Soyad
        /// </summary>
        public string adSoyad;
        /// <summary>
        /// Unvan
        /// </summary>
        public string unvan;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Banka
        /// </summary>
        public string banka;
        /// <summary>
        /// Şube
        /// </summary>
        public string sube;
        /// <summary>
        /// Unvan Kod
        /// </summary>
        public int unvanKodu;
        /// <summary>
        /// Hesap No
        /// </summary>
        public string hesapNo;
        /// <summary>
        /// Vergi Dairesi
        /// </summary>
        public string vergiDairesi;
        /// <summary>
        /// Kadro Derece
        /// </summary>
        public int kadroDerece;
        /// <summary>
        /// Ödeme Esas Derece
        /// </summary>
        public int odemeEsasDerece;
        /// <summary>
        /// Ödeme Esas Gösterge
        /// </summary>
        public int odemeEsasGosterge;

        /// <summary>
        /// Ödemeye esas ek gösterge
        /// </summary>
        public int odemeyeEsasEkGosterge;

        /// <summary>
        /// <see cref="PersonelBilgi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PersonelBilgi()
        {
            tcKimlikNo = string.Empty;
            adSoyad = string.Empty;
            unvan = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            banka = string.Empty;
            sube = string.Empty;
            unvanKodu = 0;
            hesapNo = string.Empty;
            vergiDairesi = string.Empty;
            kadroDerece = 0;
            odemeEsasDerece = 0;
            odemeEsasGosterge = 0;
            odemeyeEsasEkGosterge = 0;
        }

        public void AdSoyadBol(out string ad, out string soyad)
        {
            if (string.IsNullOrWhiteSpace(adSoyad))
            {
                ad = "";
                soyad = "";
            }
            int sonBosluk = adSoyad.LastIndexOf(' ');
            if (sonBosluk < 0)
            {
                ad = "";
                soyad = adSoyad;
            }
            else
            {
                ad = adSoyad.Substring(0, sonBosluk);
                soyad = adSoyad.Substring(sonBosluk + 1);
            }
        }
    }

    /// <summary>
    /// Personel Aile Bilgisi.
    /// </summary>
    [Serializable()]
    public class PersonelAileBilgi
    {
        /// <summary>
        /// Personel TC Kimlik No
        /// </summary>
        public string personelTcKimlikNo;
        /// <summary>
        /// TC Kimlik No
        /// </summary>
        public string tcKimlikNo;
        /// <summary>
        /// Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Soyad
        /// </summary>
        public string soyad;
        /// <summary>
        /// Doğum Tarihi
        /// </summary>
        public TNSDateTime dogumTarihi;
        /// <summary>
        /// Cinsiyet
        /// </summary>
        public int cinsiyet;
        /// <summary>
        /// Yakınlık Kod
        /// </summary>
        public int yakinlikKodu;
        /// <summary>
        /// Durum
        /// </summary>
        public int durum;
        /// <summary>
        /// Sağlık Karne No
        /// </summary>
        public string saglikKarneNo;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Evlenme Tarihi
        /// </summary>
        public TNSDateTime evlenmeTarihi;
        /// <summary>
        /// Aile Cüzdan Sıra No
        /// </summary>
        public string aileCuzdaniSeriNo;
        /// <summary>
        /// Çocuk Yakınlığı
        /// </summary>
        public int cocukYakinligi;
        /// <summary>
        /// Anne Ad
        /// </summary>
        public string anneAdi;
        /// <summary>
        /// Baba Ad
        /// </summary>
        public string babaAdi;

        /// <summary>
        /// <see cref="PersonelAileBilgi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PersonelAileBilgi()
        {
            personelTcKimlikNo = string.Empty;
            tcKimlikNo = string.Empty;
            ad = string.Empty;
            soyad = string.Empty;
            dogumTarihi = new TNSDateTime();
            cinsiyet = 0;
            yakinlikKodu = 0;
            durum = 0;
            saglikKarneNo = string.Empty;
            siraNo = 0;
            evlenmeTarihi = new TNSDateTime();
            aileCuzdaniSeriNo = string.Empty;
            cocukYakinligi = 0;
            anneAdi = string.Empty;
            babaAdi = string.Empty;

        }
    }

    /// <summary>
    /// Para Birimlerine ait kur bilgileri
    /// </summary>
    [Serializable()]
    public class KurBilgisi
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Parabirimi 
        /// </summary>
        public ParaBirimi paraBirimi;
        /// <summary>
        /// Kur
        /// </summary>
        public decimal kur;
        /// <summary>
        /// Tip
        /// </summary>
        public ENUMKurBilgileriTur tip;
        /// <summary>
        /// TC Kimlik No
        /// </summary>
        public string tckimlikno;

        /// <summary>
        /// <see cref="KurBilgisi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KurBilgisi()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            paraBirimi = new ParaBirimi();
            kur = 0;
            tip = 0;
            tckimlikno = string.Empty;
        }
    }

    /// <summary>
    /// Mevzuat Parça bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class MevzuatParca
    {
        /// <summary>
        /// Sayfa
        /// </summary>
        public string sayfa;
        /// <summary>
        /// Tür
        /// </summary>
        public int tur;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int sira;
        /// <summary>
        /// Metin Parça
        /// </summary>
        public string metinParca;

        /// <summary>
        /// <see cref="MevzuatParca"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MevzuatParca()
        {
            sayfa = string.Empty;
            tur = 0;
            sira = 0;
            metinParca = string.Empty;
        }
    }

    /// <summary>
    /// Borcu Yok Belgesi bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class BorcuYokBelgesi
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod = 0;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe = 0;
        /// <summary>
        /// Cevap Id
        /// </summary>
        public string cevapId = "";
        /// <summary>
        /// Vergi Kimlik No
        /// </summary>
        public string vergiKimlikNo = "";
        /// <summary>
        /// Talep Nedeni
        /// </summary>
        public int talepNedeni = 0;
        /// <summary>
        /// Durum
        /// </summary>
        public int durum = 0;
        /// <summary>
        /// Başlangıç Tarihi
        /// </summary>
        public TNSDateTime baslangicTarihi = new TNSDateTime();
        /// <summary>
        /// Bitiş Tarihi
        /// </summary>
        public TNSDateTime bitisTarihi = new TNSDateTime();
        /// <summary>
        /// Kayıt Tarihi
        /// </summary>
        public TNSDateTime kayitTarihi = new TNSDateTime();
        /// <summary>
        /// Kaydeden Kişi TC Kimlik No
        /// </summary>
        public string kaydedenTcKimlik = "";
        /// <summary>
        /// Son İşlem Tarihi
        /// </summary>
        public TNSDateTime sonIslemTarihi = new TNSDateTime();
        /// <summary>
        /// İşlem Yapan
        /// </summary>
        public string islemYapan = "";
        /// <summary>
        /// Hata Mesajı
        /// </summary>
        public string hataMesaji = "";
        /// <summary>
        /// Vergi Daire
        /// </summary>
        public string vergiDaire = "";
        /// <summary>
        /// Unvan
        /// </summary>
        public string unvan = "";
        /// <summary>
        /// Adres
        /// </summary>
        public string adres = "";
        /// <summary>
        /// İlgili Makam
        /// </summary>
        public string ilgiliMakam = "";
        /// <summary>
        /// Form Durum
        /// </summary>
        public int formDurum = 0;
        /// <summary>
        /// Cevap Kodu Geldi
        /// </summary>
        public bool cevapKoduGeldi = false;

        /// <summary>
        /// <see cref="BorcuYokBelgesi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="id">Kod bilgisi</param>
        public BorcuYokBelgesi(int id)
        {
            kod = id;
        }
        public BorcuYokBelgesi(string dilekceKod)
        {
            cevapId = dilekceKod;
        }

        public string birim { get; set; }

        public int ilgiliBelgeYil { get; set; }

        public string ilgiliBelgeNo { get; set; }
    }


    [Serializable()]
    public class BorcuYokBelgesiSGK
    {
        public int kod;
        public int muhasebe;
        public string refNo;
        public string vergiNo;
        public int sorguTip;
        public int durum;
        public TNSDateTime tarih;
        public TNSDateTime kayitTarih;
        public string islemYapan;
        public int hata;
        public string hataMesaj;

        public BorcuYokBelgesiSGK(int id)
        {
            kod = id;
            muhasebe = 0;
            refNo = string.Empty;
            vergiNo = string.Empty;
            sorguTip = 0;
            durum = 0;
            kayitTarih = new TNSDateTime();
            islemYapan = string.Empty;
            hata = 0;
            hataMesaj = string.Empty;
        }
    }

    /// <summary>
    /// SGK Borcu Yok Belgesi bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class BorcuYokBelgesiSGKSonuc
    {
        public int kod;
        public string refNo;
        public double damgaBorc;
        public double ekpBorc;
        public double ipcBorc;
        public double issizlikBorc;
        public double oivBorc;
        public double primBorc;


        public BorcuYokBelgesiSGKSonuc()
        {
            kod = 0;
            refNo = string.Empty;
            damgaBorc = 0.0;
            ekpBorc = 0.0;
            ipcBorc = 0.0;
            issizlikBorc = 0.0;
            oivBorc = 0.0;
            primBorc = 0.0;
        }
    }

    /// <summary>
    /// Avans Hesap bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AvansHesap
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Gündelik Tutar
        /// </summary>
        public double gundelikTutar;
        /// <summary>
        /// Gündelik para birimi
        /// </summary>
        public ParaBirimi gundelikParaBirimi;
        /// <summary>
        /// Gün Sayısı
        /// </summary>
        public int gunSayisi;
        /// <summary>
        /// Arttırımlı gün sayısı
        /// </summary>
        public int arttirimliGunSayisi;
        /// <summary>
        /// Tür
        /// </summary>
        public int tur;
        /// <summary>
        /// Tutar
        /// </summary>
        public decimal tutar;
        /// <summary>
        /// ParaBirimi
        /// </summary>
        public ParaBirimi paraBirimi;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Avans Miktarı
        /// </summary>
        public double avansMiktari;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Damga Vergisi
        /// </summary>
        public double damgaVergisi;
        /// <summary>
        /// Tarih Başlangıç
        /// </summary>
        public TNSDateTime tarihBaslangic;
        /// <summary>
        /// Tarih Başlangıç Arttırımlı
        /// </summary>
        public TNSDateTime tarihBaslangicArttirimli;

        /// <summary>
        /// <see cref="AvansHesap"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AvansHesap()
        {
            yil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            gundelikTutar = 0;
            gundelikParaBirimi = new ParaBirimi();
            gunSayisi = 0;
            arttirimliGunSayisi = 0;
            tur = 0;
            tutar = 0;
            paraBirimi = new ParaBirimi();
            aciklama = string.Empty;
            avansMiktari = 0;
            siraNo = 0;
            damgaVergisi = 0;
            tarihBaslangic = new TNSDateTime();
            tarihBaslangicArttirimli = new TNSDateTime();
        }
    }

    /// <summary>
    /// Abone Fatura Su bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneFaturaSu
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fatura Yıl
        /// </summary>
        public int faturaYil;
        /// <summary>
        /// Fatura Ay
        /// </summary>
        public int faturaAy;
        /// <summary>
        /// İşletme Kodu
        /// </summary>
        public string isletmeKodu;
        /// <summary>
        /// Hizmet No
        /// </summary>
        public string hizmetNo;
        /// <summary>
        /// Fatura No
        /// </summary>
        public string faturaNo;
        /// <summary>
        /// Tebliğ Tarih
        /// </summary>
        public TNSDateTime tebligTarih;
        /// <summary>
        /// Son Ödeme Tarih
        /// </summary>
        public TNSDateTime sonOdemeTarih;
        /// <summary>
        /// Abone Ad
        /// </summary>
        public string aboneAd;
        /// <summary>
        /// Adres
        /// </summary>
        public string adresi;
        /// <summary>
        /// Sayaç No
        /// </summary>
        public string sayacNo;
        /// <summary>
        /// İlk Okuma
        /// </summary>
        public TNSDateTime ilkOkuma;
        /// <summary>
        /// Son Okuma
        /// </summary>
        public TNSDateTime sonOkuma;
        /// <summary>
        /// İlk Endeks
        /// </summary>
        public double ilkEndex;
        /// <summary>
        /// Son Endeks
        /// </summary>
        public double sonEndex;
        /// <summary>
        /// Tüketim
        /// </summary>
        public double tuketim;
        /// <summary>
        /// Kıyas
        /// </summary>
        public double kiyas;
        /// <summary>
        /// Çevre Temizlik Vergi Tutarı
        /// </summary>
        public double ctvTutari;
        /// <summary>
        /// KDV
        /// </summary>
        public double kdv;
        /// <summary>
        /// Su Bedeli
        /// </summary>
        public double suBedeli;
        /// <summary>
        /// Atık Su
        /// </summary>
        public double atikSu;
        /// <summary>
        /// Sube Yolu
        /// </summary>
        public double subeYolu;
        /// <summary>
        /// Fatura Toplam Tutar
        /// </summary>
        public double fatTopTutar;
        /// <summary>
        /// Tahsil
        /// </summary>
        public double tahsil;
        /// <summary>
        /// Tenzil
        /// </summary>
        public double tenzil;
        /// <summary>
        /// Ceza
        /// </summary>
        public double ceza;
        /// <summary>
        /// Kalan
        /// </summary>
        public double kalan;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Mail
        /// </summary>
        public string mail;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Firma Ad
        /// </summary>
        public string firmaAd;

        /// <summary>
        /// <see cref="AboneFaturaSu"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneFaturaSu()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            faturaYil = 0;
            faturaAy = 0;
            faturaNo = string.Empty;
            isletmeKodu = string.Empty;
            tebligTarih = new TNSDateTime();
            sonOdemeTarih = new TNSDateTime();
            hizmetNo = string.Empty;
            aboneAd = string.Empty;
            adresi = string.Empty;
            sayacNo = string.Empty;
            ilkOkuma = new TNSDateTime();
            sonOkuma = new TNSDateTime();
            ilkEndex = 0;
            sonEndex = 0;
            tuketim = 0;
            kiyas = 0;
            ctvTutari = 0;
            kdv = 0;
            suBedeli = 0;
            atikSu = 0;
            subeYolu = 0;
            fatTopTutar = 0;
            tahsil = 0;
            tenzil = 0;
            ceza = 0;
            kalan = 0;
            siraNo = 0;
            mail = string.Empty;
            odemeEmriNo = string.Empty;
            firmaAd = string.Empty;
        }
    }

    /// <summary>
    /// Abone Fatura Elektrik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneFaturaElektrik
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Fatura Yıl
        /// </summary>
        public int faturaYil;
        /// <summary>
        /// Fatura Ay
        /// </summary>
        public int faturaAy;
        /// <summary>
        /// İşletme Kod
        /// </summary>
        public string isletmeKodu;
        /// <summary>
        /// Hizmet No
        /// </summary>
        public string hizmetNo;
        /// <summary>
        /// Ad Soyad Unvan
        /// </summary>
        public string adSoyadUnvan;
        /// <summary>
        /// Dosya No
        /// </summary>
        public string dosyaNo;
        /// <summary>
        /// Sıra No
        /// </summary>
        public double siraNo;
        /// <summary>
        /// Adres
        /// </summary>
        public string adres;
        /// <summary>
        /// Doc No
        /// </summary>
        public string docNo;
        /// <summary>
        /// Tarife
        /// </summary>
        public string tarife;
        /// <summary>
        /// Fatura Toplam Tutar
        /// </summary>
        public decimal fatTopTutar;
        /// <summary>
        /// Ödeme Durum
        /// </summary>
        public string odemeDurum;
        /// <summary>
        /// Tüketim Aktif KWH
        /// </summary>
        public decimal tuketimAktifKwh;
        /// <summary>
        /// Tüketim ReAktif KWH
        /// </summary>
        public decimal tuketimReaktifKwh;
        /// <summary>
        /// Son Ödeme Tarih
        /// </summary>
        public TNSDateTime sonOdemeTarih;
        /// <summary>
        /// Mail
        /// </summary>
        public string mail;
        /// <summary>
        /// Ödeme Emri Belge No
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// Sayaçlar
        /// </summary>
        public ObjectArray sayaclar;

        /// <summary>
        /// <see cref="AboneFaturaElektrik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneFaturaElektrik()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            faturaYil = 0;
            faturaAy = 0;
            isletmeKodu = string.Empty;
            hizmetNo = string.Empty;
            adSoyadUnvan = string.Empty;
            dosyaNo = string.Empty;
            siraNo = 0.0;
            adres = string.Empty;
            docNo = string.Empty;
            tarife = string.Empty;
            fatTopTutar = 0;
            odemeDurum = string.Empty;
            tuketimAktifKwh = 0;
            tuketimReaktifKwh = 0;
            sonOdemeTarih = new TNSDateTime();
            mail = string.Empty;
            odemeEmriNo = string.Empty;
            sayaclar = new ObjectArray();
        }
    }

    /// <summary>
    /// Abone Elektrik Fatura Elektrik Sayaç bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneFaturaElektrikSayac
    {
        /// <summary>
        /// Fatura Yıl
        /// </summary>
        public int faturaYil;
        /// <summary>
        /// Fatura Ay
        /// </summary>
        public int faturaAy;
        /// <summary>
        /// İşletme Kod
        /// </summary>
        public string isletmeKodu;
        /// <summary>
        /// Hizmet No
        /// </summary>
        public string hizmetNo;
        /// <summary>
        /// Seri No
        /// </summary>
        public string seriNo;
        /// <summary>
        /// Marka
        /// </summary>
        public string marka;
        /// <summary>
        /// Enerji Ölçüm Cins
        /// </summary>
        public string enerjiOlcumCins;
        /// <summary>
        /// Hane Tam
        /// </summary>
        public string haneTam;
        /// <summary>
        /// Hane Kesir
        /// </summary>
        public string haneKesir;
        /// <summary>
        /// Son Okuma Tarih
        /// </summary>
        public TNSDateTime sonOkumaTarih;

        /// <summary>
        /// <see cref="AboneFaturaElektrikSayac"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneFaturaElektrikSayac()
        {
            faturaYil = 0;
            faturaAy = 0;
            isletmeKodu = string.Empty;
            hizmetNo = string.Empty;
            seriNo = string.Empty;
            marka = string.Empty;
            enerjiOlcumCins = string.Empty;
            haneTam = string.Empty;
            haneKesir = string.Empty;
            sonOkumaTarih = new TNSDateTime();
        }
    }


    /// <summary>
    /// Avans Artığı bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AvansArtigi
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Avans Artığı Muhasebe İşlem Fişi No
        /// </summary>
        public string avansArtigiMif;

        /// <summary>
        /// <see cref="AvansArtigi"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AvansArtigi()
        {
            yil = 0;
            surecNo = string.Empty;
            avansArtigiMif = string.Empty;
        }

        public int avansArtigiMifYil { get; set; }

        public string avansArtigiMifveYil
        {
            get { return avansArtigiMifYil.ToString() + "/" + avansArtigiMif; }

            set
            {
                int bolumYer = value.IndexOf('/');
                if (bolumYer > -1)
                {
                    this.avansArtigiMif = value.Substring(bolumYer + 1);
                    if (bolumYer > 2)
                        this.avansArtigiMifYil = OrtakFonksiyonlar.ConvertToInt(value.Substring(0, bolumYer), 0);
                }
                else
                {
                    this.avansArtigiMif = value;
                }
                if (this.avansArtigiMifYil == 0)
                    this.avansArtigiMifYil = this.yil;
            }
        }
    }

    /// <summary>
    /// Süreç Parça bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class SurecParca
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// İşlem Kod
        /// </summary>
        public int isKod;
        /// <summary>
        /// Parça Kod
        /// </summary>
        public int parcaKod;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// İhale Usul Kod
        /// </summary>
        public int ihaleUsulu;
        /// <summary>
        /// İhale Madde Kod
        /// </summary>
        public int ihaleMaddesi;
        /// <summary>
        /// Düzey
        /// </summary>
        public int duzey;
        /// <summary>
        /// Zorunlu
        /// </summary>
        public int zorunlu;
        /// <summary>
        /// Parçalar
        /// </summary>
        public ObjectArray parcalar;
        /// <summary>
        /// Sonraki Parça İste
        /// </summary>
        public bool sonrakiParcaIste;

        /// <summary>
        /// <see cref="SurecParca"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SurecParca()
        {
            yil = 0;
            surecNo = string.Empty;
            isKod = 0;
            parcaKod = 0;
            siraNo = 0;
            ihaleUsulu = 0;
            ihaleMaddesi = 0;
            duzey = 0;
            zorunlu = 0;
            parcalar = new ObjectArray();
            sonrakiParcaIste = true;
        }
    }

    /// <summary>
    /// Personel Yolluk İstatistik Sınıf bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class PSLYollukIstatistikSinif
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Yolluk Tür
        /// </summary>
        public int yollukTur;
        /// <summary>
        /// Yolluk Tip
        /// </summary>
        public int yollukTip;
        /// <summary>
        /// Personel Sayı
        /// </summary>
        public int perSayi;
        /// <summary>
        /// İşlem Yapan Sayı
        /// </summary>
        public int islemYapanSayi;
        /// <summary>
        /// İşlem Sayısı
        /// </summary>
        public int islemSayisi;
        /// <summary>
        /// İl
        /// </summary>
        public int il;
        /// <summary>
        /// Taksi tutar
        /// </summary>
        public double taksi;
        /// <summary>
        /// Yol tutar
        /// </summary>
        public double yol;
        /// <summary>
        /// Konaklama tutar
        /// </summary>
        public double konaklama;
        /// <summary>
        /// Taksi Döviz tutar
        /// </summary>
        public double taksiDoviz;
        /// <summary>
        /// Yol Döviz tutar
        /// </summary>
        public double yolDoviz;
        /// <summary>
        /// Konaklama Döviz tutar
        /// </summary>
        public double konaklamaDoviz;

        /// <summary>
        /// <see cref="PSLYollukIstatistikSinif"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public PSLYollukIstatistikSinif()
        {
            yil = 0;
            perSayi = 0;
            il = 0;
            taksi = 0;
            yol = 0;
            konaklama = 0;
            yollukTur = 0;
            yollukTip = 0;
            taksiDoviz = 0;
            yolDoviz = 0;
            konaklamaDoviz = 0;
            islemYapanSayi = 0;
            islemSayisi = 0;
        }
    }

    /// <summary>
    /// Abone Fatura İstatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class AboneFaturaIstatistikSinif
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Abone Tip
        /// </summary>
        public int aboneTip;
        /// <summary>
        /// Toplam Fatura Tutar
        /// </summary>
        public decimal toplamFaturaTutar;
        /// <summary>
        /// Fatura Ay Sayısı
        /// </summary>
        public int faturaAySayisi;
        /// <summary>
        /// Ortalama
        /// </summary>
        public decimal ortalama;

        /// <summary>
        /// <see cref="AboneFaturaIstatistikSinif"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public AboneFaturaIstatistikSinif()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            aboneTip = 0;
            toplamFaturaTutar = 0;
            faturaAySayisi = 0;
            ortalama = 0;
        }
    }


    /// <summary>
    /// Kiralama bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class Kiralama
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Tur
        /// </summary>
        public int tur;
        /// <summary>
        /// Araç bilgisini <see cref="KiralamaArac"/> nesnesi ile tutan özellik
        /// </summary>
        public KiralamaArac Arac;
        /// <summary>
        /// Taşınır bilgisini <see cref="KiralamaTasinir"/> nesnesi ile tutan özellik
        /// </summary>
        public KiralamaTasinir Tasinir;
        /// <summary>
        /// Taşınmaz bilgisini <see cref="KiralamaTasinmaz"/> nesnesi ile tutan özellik
        /// </summary>
        public KiralamaTasinmaz Tasinmaz;
        /// <summary>
        /// Tag
        /// </summary>
        public object tag;

        /// <summary>
        /// <see cref="Kiralama"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public Kiralama()
        {
            yil = 0;
            muhasebe = 0;
            birim = string.Empty;
            surecNo = string.Empty;
            siraNo = 0;
            tur = 0;
            Arac = new KiralamaArac();
            Tasinir = new KiralamaTasinir();
            Tasinmaz = new KiralamaTasinmaz();
            tag = null;
        }
    }

    /// <summary>
    /// Kiralama Araç bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class KiralamaArac
    {
        /// <summary>
        /// Marka
        /// </summary>
        public string marka;
        /// <summary>
        /// Tip
        /// </summary>
        public string tip;
        /// <summary>
        /// Model
        /// </summary>
        public string model;
        /// <summary>
        /// Model Yılı
        /// </summary>
        public int modelYili;
        /// <summary>
        /// Cins
        /// </summary>
        public string cinsi;
        /// <summary>
        /// Plaka
        /// </summary>
        public string plaka;
        /// <summary>
        /// Menşei
        /// </summary>
        public string mensei;
        /// <summary>
        /// Silindir Hacmi
        /// </summary>
        public string silindirHacmi;
        /// <summary>
        /// Motor Gücü
        /// </summary>
        public string motorGucu;
        /// <summary>
        /// Ağırlığı
        /// </summary>
        public string agirligi;
        /// <summary>
        /// Koltuk Sayısı
        /// </summary>
        public string koltukSayisi;
        /// <summary>
        /// Kullanış Amacı
        /// </summary>
        public string kullanisAmaci;

        /// <summary>
        /// <see cref="KiralamaArac"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KiralamaArac()
        {
            marka = string.Empty;
            tip = string.Empty;
            model = string.Empty;
            modelYili = 0;
            cinsi = string.Empty;
            plaka = string.Empty;
            mensei = string.Empty;
            silindirHacmi = string.Empty;
            motorGucu = string.Empty;
            agirligi = string.Empty;
            koltukSayisi = string.Empty;
            kullanisAmaci = string.Empty;
        }
    }

    /// <summary>
    /// Kiralama Taşınır bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class KiralamaTasinir
    {
        /// <summary>
        /// Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// Markası
        /// </summary>
        public string markasi;
        /// <summary>
        /// Model
        /// </summary>
        public string model;
        /// <summary>
        /// Model Yıl
        /// </summary>
        public int modelYili;
        /// <summary>
        /// Seri
        /// </summary>
        public string serisi;

        /// <summary>
        /// <see cref="KiralamaTasinir"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KiralamaTasinir()
        {
            adi = string.Empty;
            markasi = string.Empty;
            model = string.Empty;
            modelYili = 0;
            serisi = string.Empty;
        }
    }

    /// <summary>
    /// Kiralama Taşınmaz bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class KiralamaTasinmaz
    {
        /// <summary>
        /// Cins
        /// </summary>
        public int cinsi;
        /// <summary>
        /// Ad
        /// </summary>
        public string adi;
        /// <summary>
        /// Pafta Ada Parsel
        /// </summary>
        public string paftaAdaParsel;
        /// <summary>
        /// Yüz Ölçümü
        /// </summary>
        public string yuzolcumu;
        /// <summary>
        /// Hisse Oranı
        /// </summary>
        public string hisseOrani;
        /// <summary>
        /// Kat Sayısı
        /// </summary>
        public string katSayisi;
        /// <summary>
        /// Yapım yılı
        /// </summary>
        public int yapimYili;

        /// <summary>
        /// <see cref="KiralamaTasinmaz"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KiralamaTasinmaz()
        {
            cinsi = 0;
            adi = string.Empty;
            paftaAdaParsel = string.Empty;
            yuzolcumu = string.Empty;
            hisseOrani = string.Empty;
            katSayisi = string.Empty;
            yapimYili = 0;
        }
    }

    /// <summary>
    /// KurBilgiSinif: 
    /// </summary>
    [System.Serializable()]
    public class MerkezBankasiKurDetaySinif
    {

        /// <summary>
        /// Kur Kod: Kur Kodu Giriniz
        /// </summary>
        public ParaBirimi paraBirim;

        /// <summary>
        /// Döviz Alış: Döviz Alış Değeri Giriniz
        /// </summary>
        public decimal dovizAlis;

        /// <summary>
        /// Döviz Satış: Döviz Satış Değeri Giriniz
        /// </summary>
        public decimal dovizSatis;

        /// <summary>
        /// Efektif Alış: Efektif Alış Değeri Giriniz
        /// </summary>
        public decimal efektifAlis;

        /// <summary>
        /// Efektif Satış: Efektif Satış Değeri Giriniz
        /// </summary>
        public decimal efektifSatis;

        /// <summary>
        /// Serbest Alış: Serbest Alış Değeri Giriniz
        /// </summary>
        public decimal serbestAlis;

        /// <summary>
        /// Serbest Satış: Serbest Satış Değeri Giriniz
        /// </summary>
        public decimal serbestSatis;

        public decimal crossRateOther;
        public decimal crossRateUSD;

        /// <summary>
        /// KurTarihi : Kur Tarihini Giriniz
        /// </summary>
        public TNSDateTime kurTarihi;

        /// <summary>
        /// <see cref="MerkezBankasiKurDetaySinif"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MerkezBankasiKurDetaySinif()
        {
            paraBirim = new ParaBirimi();
        }

        public static MerkezBankasiKurDetaySinif Olustur(string kisaltma)
        {
            MerkezBankasiKurDetaySinif det = new HRC.MerkezBankasiKurDetaySinif();
            det.paraBirim.kisaltma = kisaltma;
            return det;
        }

        public decimal DegerBul(string kurTur, decimal defaultVal)
        {
            if (kurTur == "ALIS")
                return dovizAlis;
            else if (kurTur == "SATIS")
                return dovizSatis;
            else if (kurTur == "EALIS")
                return efektifAlis;
            else if (kurTur == "ESATIS")
                return efektifSatis;
            else if (kurTur == "SALIS")
                return serbestAlis;
            else if (kurTur == "SSATIS")
                return serbestSatis;
            else if (defaultVal > 0)
                return defaultVal;
            else
                return dovizAlis;
        }
    }


    /// <summary>
    /// MerkezBankasiKurSinif: 
    /// </summary>
    [System.Serializable()]
    public class MerkezBankasiKurSinif
    {
        /// <summary>
        /// İşlem sonucunu tutan Sonuc nesnesi
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// Kur Tarihi: Kur Tarihi Seçiniz
        /// </summary>
        public TNSDateTime kurTarih;

        /// <summary>
        /// KurBilgiSinif: 
        /// </summary>
        public MerkezBankasiKurDetaySinif[] satirlar;
        public bool yakinGunBul = false;
        public bool tumParalariDondur = false;

        /// <summary>
        /// <see cref="MerkezBankasiKurSinif"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public MerkezBankasiKurSinif()
        {
            sonuc = new Sonuc(true, "", "");
        }

        public decimal KurBul(string paraBirimiKod, string kurTur, decimal defaultVal)
        {
            return KurBul(paraBirimiKod, kurTur, defaultVal, 0);
        }

        public decimal KurBul(string paraBirimiKod, string kurTur, decimal defaultVal, int yuvarlama)
        {
            if (satirlar == null) return defaultVal;
            foreach (MerkezBankasiKurDetaySinif kur in satirlar)
            {
                if (kur.paraBirim.kisaltma == paraBirimiKod)
                {
                    return kur.DegerBul(kurTur, defaultVal);
                }
            }
            if (yuvarlama == 0)
                return defaultVal;
            else
                return Math.Round(defaultVal, yuvarlama);
        }
    }

    /// <summary>
    /// SurecSifreSinif: 
    /// </summary>
    [System.Serializable()]
    public class SurecSifreSinif
    {
        /// <summary>
        /// Yıl: Yıl Giriniz
        /// </summary>
        public int yil;

        /// <summary>
        /// Surec No: Süreç Numarası Giriniz
        /// </summary>
        public string surecNo;

        /// <summary>
        /// Şifre: Şifre Giriniz
        /// </summary>
        public string sifre;

        /// <summary>
        /// TCKimlik: TCKimlik Giriniz
        /// </summary>
        public string TCKimlik;

        /// <summary>
        /// TCKimlik: TCKimlik Giriniz
        /// </summary>
        public TNSDateTime tarih;


        /// <summary>
        /// Parca: 
        /// </summary>
        public Parca[] satirlar;
    }

    /// <summary>
    /// Yurt içi geçici görev yolluk istatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class YIGGYIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Tür
        /// </summary>
        public int tur;
        /// <summary>
        /// Görev Sayısı
        /// </summary>
        public int gorevSayisi;
        /// <summary>
        /// Toplam gün sayısı
        /// </summary>
        public decimal toplamGunSayisi;
        /// <summary>
        /// Toplam Tutar
        /// </summary>
        public decimal toplamTutar;


        /// <summary>
        /// <see cref="YIGGYIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public YIGGYIstatistik()
        {
            yil = 0;
            tur = 0;
            gorevSayisi = 0;
            toplamGunSayisi = 0;
            toplamTutar = 0;
        }
    }

    /// <summary>
    /// Yurt dışı geçici görev yolluk istatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class YDGGYIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Tür
        /// </summary>
        public int tur;
        /// <summary>
        /// Görev Sayısı
        /// </summary>
        public int gorevSayisi;
        /// <summary>
        /// Toplam Gün Sayısı
        /// </summary>
        public decimal toplamGunSayisi;
        /// <summary>
        /// Toplam tutar
        /// </summary>
        public decimal toplamTutar;

        /// <summary>
        /// <see cref="YDGGYIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public YDGGYIstatistik()
        {
            yil = 0;
            tur = 0;
            gorevSayisi = 0;
            toplamGunSayisi = 0;
            toplamTutar = 0;
        }
    }

    /// <summary>
    /// Yurt içi sürekli görev yolluk istatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class YISGYIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Tür
        /// </summary>
        public int tur;
        /// <summary>
        /// Atama Sayısı
        /// </summary>
        public int atamaSayisi;
        /// <summary>
        /// Toplam Tutar
        /// </summary>
        public decimal toplamTutar;

        /// <summary>
        /// <see cref="YISGYIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public YISGYIstatistik()
        {
            yil = 0;
            tur = 0;
            atamaSayisi = 0;
            toplamTutar = 0;
        }
    }

    /// <summary>
    /// Yurt dışı sürekli görev yolluk istatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class YDSGYIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Tur
        /// </summary>
        public int tur;
        /// <summary>
        /// Atama Sayısı
        /// </summary>
        public int atamaSayisi;
        /// <summary>
        /// Toplam Tutar
        /// </summary>
        public decimal toplamTutar;

        /// <summary>
        /// <see cref="YDSGYIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public YDSGYIstatistik()
        {
            yil = 0;
            tur = 0;
            atamaSayisi = 0;
            toplamTutar = 0;
        }
    }

    /// <summary>
    /// Elektrik İstatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class ElektrikIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Toplam Tuketim
        /// </summary>
        public decimal toplamTuketim;
        /// <summary>
        /// Toplam tutar
        /// </summary>
        public decimal toplamTutar;

        /// <summary>
        /// <see cref="ElektrikIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public ElektrikIstatistik()
        {
            yil = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            toplamTuketim = 0;
            toplamTutar = 0;
        }
    }

    /// <summary>
    /// Su İstatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class SuIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Toplam tüketim
        /// </summary>
        public decimal toplamTuketim;
        /// <summary>
        /// Toplam tutar
        /// </summary>
        public decimal toplamTutar;

        /// <summary>
        /// <see cref="SuIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public SuIstatistik()
        {
            yil = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            toplamTuketim = 0;
            toplamTutar = 0;
        }
    }

    /// <summary>
    /// Telekom İstatistik bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class TelekomIstatistik
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Ay
        /// </summary>
        public int ay;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebeKod;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birimKod;
        /// <summary>
        /// Toplam Tutar
        /// </summary>
        public decimal toplamTutar;
        /// <summary>
        /// Toplam Fatura sayı
        /// </summary>
        public decimal toplamFaturaSayi;
        /// <summary>
        /// Ortalama abone sayı
        /// </summary>
        public int ortAboneSayi;

        /// <summary>
        /// <see cref="TelekomIstatistik"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public TelekomIstatistik()
        {
            yil = 0;
            ay = 0;
            muhasebeKod = 0;
            birimKod = string.Empty;
            toplamTutar = 0;
            toplamFaturaSayi = 0;
            ortAboneSayi = 0;
        }
    }

    /// <summary>
    /// Bütçe Harcama bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class BHMHarcama
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İşlem Yıl
        /// </summary>
        public int islemYil;
        /// <summary>
        /// İşlem Ay
        /// </summary>
        public int islemAy;
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int surecYil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Muhsebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Fonksiyonel Kod
        /// </summary>
        public string fon;
        /// <summary>
        /// Finansman Kod
        /// </summary>
        public string fin;
        /// <summary>
        /// Ekonomik Kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Alt Faaliyet Kod
        /// </summary>
        public string peb;
        /// <summary>
        /// Toplam Tutar
        /// </summary>
        public decimal toplamTutar;
        /// <summary>
        /// Yıllara Sari işlem mi
        /// </summary>
        public bool yillaraSariIslemMi;

        /// <summary>
        /// <see cref="BHMHarcama"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public BHMHarcama()
        {
            yil = 0;
            islemYil = 0;
            islemAy = 0;
            surecYil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            kur = string.Empty;
            fon = string.Empty;
            fin = string.Empty;
            eko = string.Empty;
            peb = string.Empty;
            toplamTutar = 0;
            yillaraSariIslemMi = false;
        }
    }

    /// <summary>
    /// Bütçe Harcama bilgisini tutan nesne
    /// </summary>
    [Serializable()]
    public class BHMHarcamaSorgu
    {
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// İşlem Yıl
        /// </summary>
        public int islemYil;
        /// <summary>
        /// İşlem Ay
        /// </summary>
        public int islemAy;
        /// <summary>
        /// Süreç Yıl
        /// </summary>
        public int surecYil;
        /// <summary>
        /// Süreç No
        /// </summary>
        public string surecNo;
        /// <summary>
        /// Muhasebe Kod
        /// </summary>
        public int muhasebe;
        /// <summary>
        /// Birim Kod
        /// </summary>
        public string birim;
        /// <summary>
        /// Kurumsal Kod
        /// </summary>
        public string kur;
        /// <summary>
        /// Fonksiyonel Kod
        /// </summary>
        public string fon;
        /// <summary>
        /// Finansman Kod
        /// </summary>
        public string fin;
        /// <summary>
        /// Ekonomik Kod
        /// </summary>
        public string eko;
        /// <summary>
        /// Alt Faaliyet Kod
        /// </summary>
        public string peb;
        /// <summary>
        /// Onay Tutar
        /// </summary>
        public decimal onay;
        /// <summary>
        /// Alım Tutar
        /// </summary>
        public decimal alim;
        /// <summary>
        /// Fatura Tutar
        /// </summary>
        public decimal fatura;
        /// <summary>
        /// Teslim tutar
        /// </summary>
        public decimal teslim;
        /// <summary>
        /// Ödeme Tutar
        /// </summary>
        public decimal odeme;

        /// <summary>
        /// <see cref="BHMHarcamaSorgu"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public BHMHarcamaSorgu()
        {
            yil = 0;
            islemYil = 0;
            islemAy = 0;
            surecYil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            kur = string.Empty;
            fon = string.Empty;
            fin = string.Empty;
            eko = string.Empty;
            peb = string.Empty;
            onay = 0;
            alim = 0;
            fatura = 0;
            teslim = 0;
            odeme = 0;
        }
    }
    /// <summary>
    /// Kur bilgilerini tutan nesne
    /// </summary>
    [Serializable()]
    public class KurDegerleri
    {
        /// <summary>
        /// Kur bilgisi para birimi
        /// </summary>
        public ParaBirimi paraBirimi;
        /// <summary>
        /// Kur bilgisi alış tutarı
        /// </summary>
        public decimal alis;
        /// <summary>
        /// Kur bilgisi satış tutarı
        /// </summary>
        public decimal satis;
        /// <summary>
        /// Kur bilgisi önceki alış tutarı
        /// </summary>
        public decimal oncekiAlis;
        /// <summary>
        /// Kur bilgisi önceki satış tutarı
        /// </summary>
        public decimal oncekiSatis;
        /// <summary>
        /// Kur bilgisi e.alış tutarı
        /// </summary>
        public decimal efektifAlis;
        /// <summary>
        /// Kur bilgisi e.satış tutarı
        /// </summary>
        public decimal efektifSatis;
        /// <summary>
        /// Kur bilgisi serbest alış tutarı
        /// </summary>
        public decimal serbestAlis;
        /// <summary>
        /// Kur bilgisi serbest satış tutarı
        /// </summary>
        public decimal serbestSatis;

        /// <summary>
        /// <see cref="KurDegerleri"/> sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        public KurDegerleri()
        {
            paraBirimi = new ParaBirimi();
            alis = 0;
            satis = 0;
            oncekiAlis = 0;
            oncekiSatis = 0;
            efektifAlis = 0;
            efektifSatis = 0;
            serbestAlis = 0;
            serbestSatis = 0;
        }
    }

    [Serializable()]
    public class MPRevizeNoTalepNo
    {
        public int yil;
        public int surecYil;
        public string surecNo;
        public int revizeNo;
        public int talepNo;
    }

    public enum ENUMPersonelIstihdamSekli : int
    {
        MEMUR = 1,
        ISCI = 4,
        SOZLESMELI = 2,
        TOPLAM = 0
    }

    [Serializable()]
    public class IstihtamSekli
    {
        public ENUMPersonelIstihdamSekli kod;

        public static string AdBul(ENUMPersonelIstihdamSekli kod)
        {
            switch (kod)
            {
                case ENUMPersonelIstihdamSekli.MEMUR:
                    return "Memur";
                case ENUMPersonelIstihdamSekli.ISCI:
                    return "İşçi";
                case ENUMPersonelIstihdamSekli.SOZLESMELI:
                    return "Sözleşmeli";
                case ENUMPersonelIstihdamSekli.TOPLAM:
                    return "Toplam";
                default:
                    return "";
            }
        }

        public string ad
        {
            get
            {
                return AdBul(kod);
            }
        }
        public int adet;
    }

    [Serializable()]
    public class SeyyarGorevDurumAna
    {
        public Sonuc sonuc = new Sonuc(true, "", "");
        public TNSCollection istihdamSekilleri = new TNSCollection();
        public double gonderilen;
        public double harcanan;
        public double surecHarcama;
        public double oeHarcama;

        public SeyyarGorevDurum MemurBul()
        {
            return IstihdamSekliBul(1);
        }

        public SeyyarGorevDurum SozlesmeliBul()
        {
            return IstihdamSekliBul(2);
        }

        public SeyyarGorevDurum IsciBul()
        {
            return IstihdamSekliBul(4);
        }

        private SeyyarGorevDurum IstihdamSekliBul(int istihdamSekli)
        {
            if (istihdamSekilleri != null)
            {
                for (int i = 0; i < istihdamSekilleri.Count; i++)
                {
                    SeyyarGorevDurum sgd = (SeyyarGorevDurum)istihdamSekilleri[i];
                    if (sgd.istihdamSekli == istihdamSekli)
                        return sgd;
                }
            }
            return null;
        }
    }

    [Serializable()]
    public class SeyyarGorevDurum
    {
        public int istihdamSekli { get; set; }

        public int vizeEdilen { get; set; }

        public int kullanilan { get; set; }
        public double tutar { get; set; }

        public int surecKullanilan { get; set; }
        public double surecTutar { get; set; }

        public int surecDisiKullanilan { get { return kullanilan - surecKullanilan; } }
        public double surecDisiTutar { get { return tutar - surecTutar; } }

        public bool AsiyorMu(int surecToplam)
        {
            return (surecDisiKullanilan + surecToplam) > vizeEdilen;
        }
    }

    [Serializable()]
    public class SeyyarGorevI
    {
        public int yil;
        public string harcamaBirimiKodu { get; set; }
        public string harcamaBirimiAdi { get; set; }
        public int asama;
        public int saymanlikKod { get; set; }
        public string birimKod { get; set; }
        public string kurKod { get; set; }

        //public SeyyarGorevDetay[] satirlar;
        public SeyyarGorevUnvanBilgi[] unvanlar;
        public int memur { get; set; }
        public int isci { get; set; }
        public int sozlesmeli { get; set; }
        public int oncAsamaMemur { get; set; }
        public int oncAsamaIsci { get; set; }
        public int oncAsamaSozlesmeli { get; set; }

        public int oncVizeliMemur { get; set; }
        public int oncVizeliIsci { get; set; }
        public int oncVizeliSozlesmeli { get; set; }

        public int oncKullanilanMemur { get; set; }
        public int oncKullanilanIsci { get; set; }
        public int oncKullanilanSozlesmeli { get; set; }

        public double oncekiYilKullanilanTutar { get; set; }
        public int personelSayisiMemur { get; set; }
        public int personelSayisiIsci { get; set; }
        public int personelSayisiSozlesmeli { get; set; }

        public void Ekle(SeyyarGorevI gelen)
        {
            this.memur += gelen.memur;
            this.isci += gelen.isci;
            this.sozlesmeli += gelen.sozlesmeli;

            this.oncAsamaMemur += gelen.oncAsamaMemur;
            this.oncAsamaIsci += gelen.oncAsamaIsci;
            this.oncAsamaSozlesmeli += gelen.oncAsamaSozlesmeli;

            this.oncVizeliMemur += gelen.oncVizeliMemur;
            this.oncVizeliIsci += gelen.oncVizeliIsci;
            this.oncVizeliSozlesmeli += gelen.oncVizeliSozlesmeli;

            this.oncKullanilanMemur += gelen.oncKullanilanMemur;
            this.oncKullanilanIsci += gelen.oncKullanilanIsci;
            this.oncKullanilanSozlesmeli += gelen.oncKullanilanSozlesmeli;

            this.oncekiYilKullanilanTutar += gelen.oncekiYilKullanilanTutar;

            this.personelSayisiMemur += gelen.personelSayisiMemur;
            this.personelSayisiIsci += gelen.personelSayisiIsci;
            this.personelSayisiSozlesmeli += gelen.personelSayisiSozlesmeli;
        }

        public int GunBul(ENUMPersonelIstihdamSekli isSek)
        {
            if (isSek == ENUMPersonelIstihdamSekli.SOZLESMELI)
                return sozlesmeli;
            else if (isSek == ENUMPersonelIstihdamSekli.ISCI)
                return isci;
            else
                return memur;
        }

        public void GunYaz(ENUMPersonelIstihdamSekli isSek, int rakam)
        {
            if (isSek == ENUMPersonelIstihdamSekli.SOZLESMELI)
                sozlesmeli = rakam;
            else if (isSek == ENUMPersonelIstihdamSekli.ISCI)
                isci = rakam;
            else
                memur = rakam;
        }
    }

    [Serializable()]
    public class SeyyarGorevDetay
    {
        public IstihtamSekli istihtamSekli = new IstihtamSekli();
        public int Talep { get; set; }
        public int oncTahsis { get; set; }
        public int oncKullanilan { get; set; }
        public double oncOdenen { get; set; } = 0;

        public int Kod { get { return (int)istihtamSekli.kod; } set { istihtamSekli.kod = (ENUMPersonelIstihdamSekli)value; } }
        public string Ad { get { return istihtamSekli.ad; } }

        public void Ekle(SeyyarGorevDetay talepBilgi)
        {
            this.Talep += talepBilgi.Talep;
            this.oncOdenen += talepBilgi.oncOdenen;
            this.oncTahsis += talepBilgi.oncTahsis;
            this.oncKullanilan += talepBilgi.oncKullanilan;
        }

        public SeyyarGorevDetay(int Kod, string Ad)
        {
            this.Kod = Kod;
        }

        public SeyyarGorevDetay()
        {
            this.Kod = 0;
        }
    }

    [Serializable()]
    public class SeyyarGorevUnvanBilgi
    {
        public IstihtamSekli istihdamSekli = new IstihtamSekli();
        public ENUMPersonelIstihdamSekli istihdamSekliKod { get { return istihdamSekli.kod; } set { istihdamSekli.kod = value; } }
        public string istihdamSekliAd
        {
            get
            {
                return istihdamSekli.ad;
            }
        }
        public int unvanKod { get; set; }
        public string unvanAd { get; set; }
        public int personelSayisi { get; set; }
        public bool oncekiVizeCetvelindeVarMi { get; set; }
        public bool vizeCetvelindeOlmaliMi { get; set; }

        public SeyyarGorevUnvanBilgi()
        {

        }
        public SeyyarGorevUnvanBilgi(ENUMPersonelIstihdamSekli istihdamSekliKod, int unvanKod, string unvanAd, int personelSayisi, bool oncekiVizeCetvelindeVarMi, bool vizeCetvelindeOlmaliMi)
        {
            this.istihdamSekliKod = istihdamSekliKod;
            //this.istihdamSekliAd = istihdamSekliAd;
            this.unvanKod = unvanKod;
            this.unvanAd = unvanAd;
            this.personelSayisi = personelSayisi;
            this.oncekiVizeCetvelindeVarMi = oncekiVizeCetvelindeVarMi;
            this.vizeCetvelindeOlmaliMi = vizeCetvelindeOlmaliMi;
        }
    }

    [Serializable()]
    public class HarcamaBirimiPersoneli
    {
        public string adi;
        public string soyadi;
        public int istihdamSekli;
        public int unvanKodu;
        public string unvanAdi;
        public int odemeyeEsasDerece;
        public int odemeyeEsasKademe;

    }

    [Serializable()]
    public class RaporIhaleFirma
    {
        public int yil;
        public int ay;
        public string kurKod;
        public int muhasebe;
        public string birim;
        public int firmaSayisi;
        public string firmaVergiNo;
        public string firmaAd;
        public decimal toplam;
        public int islemKod;
        public string ihaleTur;
        public int ihaleUsulu;
        public TNSDateTime faturaTarih;
    }

    [Serializable()]
    public class SurecMuhMifBaglanti
    {
        public int yil;
        public string surecNo;
        public string odemeEmriNo;
        public int muhasebe;
        public string birim;
        public string kurKod;
        public string fonKod;
        public string finKod;
        public string ekoKod;
        public string pebKod;
        public decimal borc;
    }

    [Serializable()]
    public class TurkcellLog
    {
        public int yil;
        public string surecNo;
        public TNSDateTime islemTarih;
        public int islemTur; //1-kaydet,2-iptal
        public string transactionId;
        public string dosyaAdi;
        public string faturaBilgi; //aboneno-faturaid-faturaTutar
        public string islemSonuc; //kod-aciklama
        public string aciklama;
    }

    [Serializable()]
    public class BankaBilgisi
    {
        public string kod;
        public string ad;
        public BankaBilgisi()
        {
            kod = "";
            ad = "";
        }
    }

    [Serializable()]
    public class BankaSubeBilgisi : BankaBilgisi
    {
        public BankaSubeBilgisi()
        {
            kod = "";
            ad = "";
        }
    }

    [Serializable()]
    public class TeminatMektubuGiris
    {
        public FirmaBilgisi firma;
        public TNSDateTime dosyaTarih;
        public string numara;
        public BankaBilgisi banka;
        public BankaSubeBilgisi bankaSube;
        public string alinisSebebi;
        public TNSDateTime vadeTarih;
        public TNSDateTime vadeTarih2;//rapor amaçlı
        public int nevi;
        public double tutar;
        public MuhasebeIslem muhasebeIslem;
        public string neviAdi;
        public TeminatMektubuGiris()
        {
            firma = new FirmaBilgisi();
            dosyaTarih = new TNSDateTime();
            numara = "";
            alinisSebebi = "";
            vadeTarih = new TNSDateTime();
            vadeTarih2 = new TNSDateTime();
            tutar = 0;
            muhasebeIslem = new MuhasebeIslem();
            banka = new BankaBilgisi();
            bankaSube = new BankaSubeBilgisi();
            nevi = 0;
            neviAdi = "";
        }
    }

    [Serializable()]
    public class TeminatMektubuCikis
    {
        public string iadeNeden;
        public TNSDateTime tarih;
        public MuhasebeIslem muhasebeIslem;
        public int durum;
        public double tutar;
        public TeminatMektubuCikis()
        {
            durum = 0;
            iadeNeden = "";
            muhasebeIslem = new MuhasebeIslem();
            tarih = new TNSDateTime();
            tutar = 0.0;
        }
    }

    [Serializable()]
    public class TeminatMektubuCikisBilgileri
    {
        public int kod;
        public int yil;
        public string dosyaNo;
        public double tutar;
        public TNSDateTime cikisTarih;
        public string iadeNeden;
        public MuhasebeIslem muhasebeIslem;
        public int durum;

        public TeminatMektubuCikisBilgileri()
        {
            kod = 0;
            yil = 0;
            dosyaNo = "";
            tutar = 0;
            cikisTarih = new TNSDateTime();
            iadeNeden = "";
            muhasebeIslem = new MuhasebeIslem();
            durum = 0;
        }
    }

    [Serializable()]
    public class MuhasebeIslem
    {
        public int yil;
        public string belgeNo;
        public TNSDateTime belgeTarihi;
        public MuhasebeIslem()
        {
            yil = 0;
            belgeNo = "";
            belgeTarihi = new TNSDateTime();
        }
    }

    [Serializable()]
    public class TeminatMektubu
    {
        public int yil;
        public string dosyaNo;
        public int muhasebeKod;
        public string birimKod;
        public string iadeNeden;
        public TNSDateTime kayitTarihi;
        public TeminatMektubuGiris giris;
        public TeminatMektubuCikis cikis;
        public SinifAlani siralananAlan;
        public ArrayList cikisBilgileri;
        public int suresiz;
        public int sozlesmeKod;

        public TeminatMektubu()
        {
            yil = 0;
            dosyaNo = "";
            muhasebeKod = 0;
            birimKod = "";
            iadeNeden = "";
            kayitTarihi = new TNSDateTime();
            giris = new TeminatMektubuGiris();
            cikis = new TeminatMektubuCikis();
            siralananAlan = new SinifAlani();
            cikisBilgileri = new ArrayList();
            suresiz = 0;
            sozlesmeKod = 0;
        }
        public int vadeDurum = 0;
        public string vadeAciklama
        {
            get
            {
                if ((cikis.tutar + 0.01) >= giris.tutar)//Yuvarlama sorunu nedeniyle 1 Kuruş eklenerek kontrol ediliyor Melih 08.12.2023
                {
                    cikis.durum = (int)ENUMCikisDurum.CIKISYAPILAN;
                    vadeDurum = 0;
                    return "Çıkış Yapılmış";
                }

                if (giris.vadeTarih.Oku().Year > 1)
                {
                    DateTime birAyOncesi = giris.vadeTarih.Oku().AddMonths(-1);
                    if (DateTime.Now.Date >= birAyOncesi && DateTime.Now.Date <= giris.vadeTarih.Oku()) // vade tarihine 1 ay kalanlar
                    {
                        vadeDurum = 1;
                        if (giris.vadeTarih.Oku() != new TNSDateTime(DateTime.Now.ToShortDateString()).Oku())
                            return (giris.vadeTarih.Oku() - DateTime.Now).Days + " Gün Kaldı";
                        else
                            return "Bugün";
                    }
                    else if (giris.vadeTarih.Oku() < DateTime.Now.Date) //vade tarihi geçenler
                    {
                        vadeDurum = 2;
                        return "Vadesi Geçti";
                    }
                }
                vadeDurum = 0;
                return "";
            }
        }

        public string muhasebeBelgeNo
        {
            get
            {
                if (giris.muhasebeIslem.yil > 0)
                    return giris.muhasebeIslem.yil + "/" + giris.muhasebeIslem.belgeNo;
                return "";

            }
        }

        public string yilDosyaNo { get { return yil + "-" + dosyaNo; } }
    }

    [Serializable()]
    public class SinifAlani
    {
        public string ad;
        public string siralamaYonu;
        public SinifAlani()
        {
            ad = "";
            siralamaYonu = "";
        }
    }

    [Serializable()]
    public class UAAvansTalep
    {
        public string sayi;
        public string gerekce;
        public string personel;
        public double miktar;
        public string banka;
        public string ibanNo;
        public string tcKimlikNo;
        public string koordinator;
        public string onaylayan;
        public string birimKod;
        public int saymanlikKod;
        public int surecYil;
        public string surecNo;
        public TNSDateTime olurTarihi;
        public string onaylayanUnvan;
        public string koordinatorUnvan;
        public string odemeEmriNo;
        public int durum;


        public UAAvansTalep()
        {
            sayi = string.Empty;
            gerekce = string.Empty;
            personel = string.Empty;
            miktar = 0;
            banka = string.Empty;
            ibanNo = string.Empty;
            tcKimlikNo = string.Empty;
            koordinator = string.Empty;
            onaylayan = string.Empty;
            birimKod = string.Empty;
            saymanlikKod = 0;
            surecYil = 0;
            surecNo = string.Empty;
            olurTarihi = null;
            onaylayanUnvan = string.Empty;
            koordinatorUnvan = string.Empty;
            odemeEmriNo = string.Empty;
            durum = 0;
        }
    }

    [Serializable()]
    public class UAAvansKapatma
    {
        public int surecYil;
        public string surecNo;

        public string odemeEmriNo;
        public string kapatmaOdemeEmriNo;
        public TNSCollection tertipler;

        public UAAvansKapatma()
        {
            surecNo = string.Empty;
            surecYil = 0;
            tertipler = null;
            odemeEmriNo = string.Empty;
            kapatmaOdemeEmriNo = string.Empty;
        }
    }

    [Serializable()]
    public class UAAvansKapatmaTertip
    {
        public string tertip { get { return _tertip; } set { _tertip = value; } }
        private string _tertip;

        public double miktar { get { return _miktar; } set { _miktar = value; } }
        private double _miktar;

        public UAAvansKapatmaTertip()
        {
            tertip = string.Empty;
            miktar = 0;
        }
    }

    [Serializable()]
    public class STFTertipBilgi
    {
        public int YIL { get; set; }
        public string TERTIP { get; set; }
        public double ODENEK { get; set; }
        public string AD { get; set; }
    }

    public enum ENUMAlimTuru : int
    {
        ihaleli = 0,
        avansli = 1,
        kamu = 2
    }
    [Serializable()]
    public class SatinAlmaTalepForm
    {
        public int yil;
        public string formKod;
        public string evrakSayi;
        public string konu;
        public string gerekce;
        public TNSDateTime iseBaslamaTarihi;
        public string aciklama;
        public decimal yaklasikMaliyet;
        public int ihaleTurKod;
        public int ihaleMaddeKod;
        public int islemKod;
        public string islemAd;
        public int durum;
        public int surecYil;
        public string surecNo;
        public ObjectArray ihtiyacListesi;
        public int muhasebe;
        public string birim;
        public string kurKod;
        public ENUMAlimTuru alimTuru;
        public string onayaSunanAdSoyad;
        public string onayaSunanUnvan;
        public string onaylayanAdSoyad;
        public string onaylayanUnvan;

        public STFTertipBilgi[] tertipler = null;
        public SatinAlmaTalepForm()
        {
            yil = 0;
            formKod = string.Empty;
            evrakSayi = string.Empty;
            konu = string.Empty;
            gerekce = string.Empty;
            iseBaslamaTarihi = new TNSDateTime();
            aciklama = string.Empty;
            yaklasikMaliyet = decimal.Zero;
            ihaleTurKod = 0;
            ihaleMaddeKod = 0;
            islemKod = 0;
            islemAd = string.Empty;
            durum = 0;
            surecYil = 0;
            surecNo = string.Empty;
            ihtiyacListesi = new ObjectArray();
            muhasebe = 0;
            birim = string.Empty;
            kurKod = string.Empty;
            alimTuru = 0;
            onayaSunanAdSoyad = string.Empty;
            onayaSunanUnvan = string.Empty;
            onaylayanAdSoyad = string.Empty;
            onaylayanUnvan = string.Empty;
        }

        public string tertipStr
        {
            get
            {
                string retVal = "";
                if (tertipler != null)
                {
                    for (int i = 0; i < tertipler.Length; i++)
                    {
                        if (i > 0) retVal += ";";
                        retVal += tertipler[i].YIL + "@" + tertipler[i].TERTIP + "@" + tertipler[i].ODENEK.ToString();
                    }
                }
                return retVal;
            }
            set
            {
                tertipler = null;
                if (!string.IsNullOrEmpty(value))
                {
                    System.Collections.Generic.List<STFTertipBilgi> l = new System.Collections.Generic.List<STFTertipBilgi>();
                    string[] satirlar = value.Split(';');
                    for (int i = 0; i < satirlar.Length; i++)
                    {
                        STFTertipBilgi b = new STFTertipBilgi();
                        string[] sutunlar = satirlar[i].Split('@');
                        if (sutunlar.Length == 2 || sutunlar.Length == 3)
                        {
                            int basla = 0;
                            if (sutunlar.Length == 3)
                            {
                                b.YIL = OrtakFonksiyonlar.ConvertToInt(sutunlar[0], 0);
                                basla = 1;
                            }
                            b.TERTIP = sutunlar[basla + 0];
                            b.ODENEK = OrtakFonksiyonlar.ConvertToDbl(sutunlar[basla + 1]);
                            if (b.YIL == 0) b.YIL = this.yil;
                            l.Add(b);
                        }
                    }
                    if (l.Count > 0) tertipler = l.ToArray();
                }

            }
        }

        public TNSDateTime olusturulmaTarih { get; set; }

        public string IhaleTurKisaAciklama
        {
            get
            {
                if (this.alimTuru == ENUMAlimTuru.avansli)
                    return "Avanslı Alım";
                else if (this.alimTuru == ENUMAlimTuru.kamu)
                    return "Kamudan Alım";
                else
                {
                    if (this.ihaleTurKod == (int)ENUMIhaleUsul.DOGRUDANTEMIN) return "Doğ. Temin";
                    else if (this.ihaleTurKod == (int)ENUMIhaleUsul.BELIRLIISTEKLI) return "Bel. İstekli";
                    else if (this.ihaleTurKod == (int)ENUMIhaleUsul.PAZARLIKUSULU) return "Pazarlık";
                    else if (this.ihaleTurKod == (int)ENUMIhaleUsul.ACIK) return "Açık İhale";
                    return "";
                }
            }
        }
    }

    [Serializable()]
    public class SatinAlmaTalepIhtiyacBaslik
    {
        private int surecYil;

        public int SurecYil
        {
            get { return surecYil; }
            set { surecYil = value; }
        }
        private string formKod;

        public string FormKod
        {
            get { return formKod; }
            set { formKod = value; }
        }
        private int malzemeSiraNo;

        public int MalzemeSiraNo
        {
            get { return malzemeSiraNo; }
            set { malzemeSiraNo = value; }
        }

        private string aciklama;

        public string Aciklama
        {
            get { return aciklama; }
            set { aciklama = value; }
        }
        public TNSCollection detay;

        public SatinAlmaTalepIhtiyacBaslik()
        {
            surecYil = 0;
            formKod = string.Empty;
            malzemeSiraNo = 0;
            aciklama = string.Empty;
            detay = new TNSCollection();
        }
    }

    [Serializable()]
    public class SatinAlmaTalepIhtiyacDetay
    {
        private int _detaySatirNo;
        public int detaySatirNo
        {
            get { return _detaySatirNo; }
            set { _detaySatirNo = value; }
        }

        private string _ihtiyacKod;

        public string ihtiyacKod
        {
            get { return _ihtiyacKod; }
            set { _ihtiyacKod = value; }
        }

        private string _ihtiyacAd;

        public string ihtiyacAd
        {
            get { return _ihtiyacAd; }
            set { _ihtiyacAd = value; }
        }

        private decimal _miktar1;

        public decimal miktar1
        {
            get { return _miktar1; }
            set { _miktar1 = value; }
        }

        private int _olcuBirimiKod1;

        public int olcuBirimiKod1
        {
            get { return _olcuBirimiKod1; }
            set { _olcuBirimiKod1 = value; }
        }

        private decimal _miktar2;

        public decimal miktar2
        {
            get { return _miktar2; }
            set { _miktar2 = value; }
        }

        private int _olcuBirimiKod2;

        public int olcuBirimiKod2
        {
            get { return _olcuBirimiKod2; }
            set { _olcuBirimiKod2 = value; }
        }

        private string _aciklama;

        public string aciklama
        {
            get { return _aciklama; }
            set { _aciklama = value; }
        }

        public string MIKTARSTR
        {
            get
            {
                string mStr = string.Format("{0} {1}", miktar1.ToString("0"), OlcuBirimAdBul(olcuBirimiKod1));
                if (miktar2 != 0)
                    mStr += " X " + string.Format("{0} {1}", miktar2.ToString("0.000,00"), OlcuBirimAdBul(olcuBirimiKod2));
                return mStr;
            }
        }

        IHRCServis servis = null;
        private string OlcuBirimAdBul(int olcuBirimKod)
        {
            if (servis == null) servis = TNS.HRC.Arac.Tanimla();
            return servis.OlcuBirimAdBul(null, olcuBirimKod);
        }

        private string _birimAd1;
        public string birimAd1
        {
            get
            {
                if (string.IsNullOrEmpty(_birimAd1))
                    _birimAd1 = OlcuBirimAdBul(olcuBirimiKod1);
                return _birimAd1;
            }
            set
            {
                _birimAd1 = value;
            }
        }

        private string _birimAd2;
        public string birimAd2
        {
            get
            {
                if (string.IsNullOrEmpty(_birimAd2)) _birimAd2 = OlcuBirimAdBul(olcuBirimiKod2);
                return _birimAd2;
            }
            set
            {
                _birimAd2 = value;
            }
        }

        public string Birimi
        {
            get
            {
                string obAd1 = birimAd1;
                string obAd2 = birimAd2;

                bool obAd1belirsiz = obAd1.Equals("Belirsiz");
                bool obAd2belirsiz = obAd2.Equals("Belirsiz");

                if (obAd1belirsiz && obAd2belirsiz) return "";
                if (!obAd1belirsiz && obAd2belirsiz) return obAd1;
                if (obAd1belirsiz && !obAd2belirsiz) return obAd2;
                return obAd1 + " x " + obAd2;
            }
        }

        public int yil;
        public string formKod;
        public int siraNo;

        public SatinAlmaTalepIhtiyacDetay()
        {
            yil = 0;
            formKod = string.Empty;
            siraNo = 0;
            detaySatirNo = 0;
            ihtiyacKod = string.Empty;
            ihtiyacAd = string.Empty;
            miktar1 = decimal.Zero;
            olcuBirimiKod1 = 0;
            miktar2 = decimal.Zero;
            olcuBirimiKod2 = 0;
            aciklama = string.Empty;
        }
    }

    [Serializable()]
    public class SatinAlmaTalepFormSorgu
    {
        public int yil;
        public string formKodBasla;
        public string formKodBitis;
        public string evrakSayi;
        public TNSDateTime iseBaslamaTarihiBasla;
        public TNSDateTime iseBaslamaTarihiBitis;
        public decimal yaklasikMaliyetBasla;
        public decimal yaklasikMaliyetBitis;
        public string tertip;
        public int ihaleTurKod;
        public int islemKod;
        public int durum;
        public int surecYil;
        public string surecNo;
        public int muhasebe;
        public string birim;
        public int alimTuru;
        public string kurKod;

        public SatinAlmaTalepFormSorgu()
        {
            yil = 0;
            formKodBasla = string.Empty;
            formKodBitis = string.Empty;
            evrakSayi = string.Empty;
            iseBaslamaTarihiBasla = new TNSDateTime();
            iseBaslamaTarihiBitis = new TNSDateTime();
            yaklasikMaliyetBasla = decimal.Zero;
            yaklasikMaliyetBitis = decimal.Zero;
            tertip = string.Empty;
            ihaleTurKod = 0;
            islemKod = 0;
            durum = 0;
            surecYil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            alimTuru = 0;
            kurKod = string.Empty;
        }

        public string konu { get; set; }
    }

    [Serializable()]
    public class IhtiyacPlaniSatir
    {
        public string kod;
        public string aciklama;
        public int durum;
        public string kodAciklama;

        public IhtiyacPlaniSatir()
        {
            kod = string.Empty;
            aciklama = string.Empty;
            durum = -1;
            kodAciklama = string.Empty;
        }
    }

    [Serializable()]
    public class SatinAlmaTalepIhtiyacTeklif
    {
        private int _detaySatirNo;
        public int detaySatirNo
        {
            get { return _detaySatirNo; }
            set { _detaySatirNo = value; }
        }

        private string _ihtiyacKod;
        public string ihtiyacKod
        {
            get { return _ihtiyacKod; }
            set { _ihtiyacKod = value; }
        }

        private string _ihtiyacAd;
        public string ihtiyacAd
        {
            get { return _ihtiyacAd; }
            set { _ihtiyacAd = value; }
        }

        private decimal _miktar1;
        public decimal miktar1
        {
            get { return _miktar1; }
            set { _miktar1 = value; }
        }

        private int _olcuBirimiKod1;
        public int olcuBirimiKod1
        {
            get { return _olcuBirimiKod1; }
            set { _olcuBirimiKod1 = value; }
        }

        private decimal _miktar2;
        public decimal miktar2
        {
            get { return _miktar2; }
            set { _miktar2 = value; }
        }

        private int _olcuBirimiKod2;
        public int olcuBirimiKod2
        {
            get { return _olcuBirimiKod2; }
            set { _olcuBirimiKod2 = value; }
        }

        public decimal _birimFiyat;
        public decimal birimFiyat
        {
            get { return _birimFiyat; }
            set { _birimFiyat = value; }
        }

        private decimal gerceklesenMiktar1;
        public decimal GerceklesenMiktar1
        {
            get { return gerceklesenMiktar1; }
            set { gerceklesenMiktar1 = value; }
        }

        private decimal gerceklesenMiktar2;
        public decimal GerceklesenMiktar2
        {
            get { return gerceklesenMiktar2; }
            set { gerceklesenMiktar2 = value; }
        }

        public int firmaKod;
        public int tip;
        public int muhasebeKod;
        public string harcamaBirimKod;
        public string birimi;



        public int yil;
        public string surecNo;
        public int siraNo;
        IHRCServis servis = null;

        public string MIKTARSTR
        {
            get
            {
                string mStr = string.Format("{0} {1}", miktar1.ToString("0"), OlcuBirimAdBul(olcuBirimiKod1));
                if (miktar2 != 0)
                    mStr += " X " + string.Format("{0} {1}", miktar2.ToString("0.000,00"), OlcuBirimAdBul(olcuBirimiKod2));
                return mStr;
            }
        }

        public string Birimi
        {
            get
            {
                string obAd1 = birimAd1;
                string obAd2 = birimAd2;

                bool obAd1belirsiz = obAd1.Equals("Belirsiz");
                bool obAd2belirsiz = obAd2.Equals("Belirsiz");

                if (obAd1belirsiz && obAd2belirsiz) return "";
                if (!obAd1belirsiz && obAd2belirsiz) return obAd1;
                if (obAd1belirsiz && !obAd2belirsiz) return obAd2;
                return obAd1 + " x " + obAd2;
            }


        }

        private string _birimAd1;
        public string birimAd1
        {
            get
            {
                if (string.IsNullOrEmpty(_birimAd1))
                    _birimAd1 = OlcuBirimAdBul(olcuBirimiKod1);
                return _birimAd1;
            }
            set
            {
                _birimAd1 = value;
            }
        }

        private string _birimAd2;
        public string birimAd2
        {
            get
            {
                if (string.IsNullOrEmpty(_birimAd2)) _birimAd2 = OlcuBirimAdBul(olcuBirimiKod2);
                return _birimAd2;
            }
            set
            {
                _birimAd2 = value;
            }
        }

        public SatinAlmaTalepIhtiyacTeklif()
        {
            yil = 0;
            surecNo = string.Empty;
            siraNo = 0;
            detaySatirNo = 0;
            ihtiyacKod = string.Empty;
            ihtiyacAd = string.Empty;
            miktar1 = decimal.Zero;
            olcuBirimiKod1 = 0;
            miktar2 = decimal.Zero;
            olcuBirimiKod2 = 0;
            birimFiyat = decimal.Zero;
            firmaKod = 0;
            tip = 0;
            muhasebeKod = 0;
            harcamaBirimKod = string.Empty;
            gerceklesenMiktar1 = 0;
            gerceklesenMiktar2 = 0;
            birimi = string.Empty;
        }

        private string OlcuBirimAdBul(int olcuBirimKod)
        {
            if (servis == null) servis = TNS.HRC.Arac.Tanimla();
            return servis.OlcuBirimAdBul(null, olcuBirimKod);
        }
    }

    public enum ENUMYollukIslemTur : int
    {
        YOLLUK = 1,
        ZORUNLUMASRAF = 2,
        KONAKLAMA = 3,
        UCAK = 4,
        DIGER = 9,
        TANIMSIZ = 0
    }
    [Serializable]
    public class GeciciGorevYollukManasDetay
    {
        public string turAciklama { get { return TurAciklama(this.tur); } }

        public static string TurAciklama(ENUMYollukIslemTur tur)
        {
            switch (tur)
            {
                case ENUMYollukIslemTur.YOLLUK:
                    return "Görevlendirme Yevmiye";
                case ENUMYollukIslemTur.ZORUNLUMASRAF:
                    return "Görevlendirme Zorunlu Masraf";
                case ENUMYollukIslemTur.KONAKLAMA:
                    return "Konaklama Karşılanan";
                case ENUMYollukIslemTur.UCAK:
                    return "Uçak Bileti Ödemesi";
                case ENUMYollukIslemTur.DIGER:
                    return "Diğer Harcalar";
            }
            return "";
        }

        public ENUMYollukIslemTur tur { get; set; }
        public string yerKod { get; set; }
        public int gun { get; set; }
        public double gecelikKonaklamaUcreti { get; set; }
        public double gundelik { get; set; }
        public double odenecek { get; set; }
        public string aciklama { get; set; }
        public string detayliAciklama
        {
            get
            {
                switch (tur)
                {
                    case ENUMYollukIslemTur.YOLLUK:
                        return yerAdSade + " " + this.gun + " Günlük Yol Yevmiyesi";
                    case ENUMYollukIslemTur.ZORUNLUMASRAF:
                        return yerAdSade + " " + this.gun + " Günlük Zorunlu Masraf";
                    case ENUMYollukIslemTur.KONAKLAMA:
                        return yerAdSade + " " + this.gun + " Günlük Konaklama Masrafı";
                }
                return aciklama;
            }
        }

        public string yerAd { get { if (string.IsNullOrWhiteSpace(yerKod)) return ""; return yerKod + "- " + yerAdSade; } }
        public string yerAdSade { get; set; }

        public GeciciGorevYollukManasDetay()
        {
            tur = 0;
            yerKod = "";
            gun = 0;
            gecelikKonaklamaUcreti = 0;
            gundelik = 0;
            odenecek = 0;
            aciklama = "";
            yerAdSade = "";
        }

        public double GundelikHesapla(int toplamGundelik, int unvanKod)
        {
            double katsayi = KatsayiBul(unvanKod);
            int gun1, gun2;
            Bol(toplamGundelik, out gun1, out gun2);
            return gun1 * gundelik * katsayi + gun2 * gundelik;
        }

        public double KatsayiBul(int unvanKod)
        {
            if (unvanKod == 7369 || unvanKod == 7370 || unvanKod == 7371 || unvanKod == 7372)
                return 1.5;
            return 1;
        }

        public double KonaklamaHesapla(int toplamKonaklama)
        {
            double katsayi = 1.5;
            int gun1, gun2;
            Bol(toplamKonaklama, out gun1, out gun2);
            return gun1 * KarsilanKonaklama(gundelik * katsayi) + gun2 * KarsilanKonaklama(gundelik);
        }

        private double KarsilanKonaklama(double gundelikUcret)
        {
            double karsilanan = OrtakFonksiyonlar.KurusYap(gundelikUcret * 0.4);
            if (this.gecelikKonaklamaUcreti > karsilanan)
            {
                karsilanan = OrtakFonksiyonlar.KurusYap((gecelikKonaklamaUcreti - karsilanan) * 0.8);
                if (karsilanan > gundelikUcret * 2) karsilanan = gundelikUcret * 2;
                return karsilanan;
            }
            return 0;
        }

        private void Bol(int toplam, out int gun1, out int gun2)
        {
            // toplam = 7, gun = 5; gun2=2; gun1=3
            // toplam = 7, gun = 2; gun2=-1'den 0 ; gun1=5
            // toplam = 17, gun = 2; gun2=9'den 0 ; gun1=5

            if (toplam >= 10)
            {
                gun2 = gun;
                gun1 = 0;
            }
            else
            {
                gun2 = toplam + gun - 10;
                if (gun2 < 0) gun2 = 0;
                gun1 = gun - gun2;
            }
        }
    }

    [Serializable]
    public class GeciciGorevYollukManas
    {

        public double ToplamBul(params ENUMYollukIslemTur[] turler)
        {
            double toplam = 0;
            foreach (GeciciGorevYollukManasDetay item in detay)
            {
                if (IcindeMi(item.tur, turler))
                    toplam += item.odenecek;
            }
            return toplam;
        }

        private bool IcindeMi(ENUMYollukIslemTur tur, ENUMYollukIslemTur[] turler)
        {
            if (turler.Length == 0) return true;
            foreach (ENUMYollukIslemTur item in turler)
            {
                if (item == tur) return true;
            }
            return false; ;
        }

        public double belgeDeger
        {
            get
            {
                return ToplamBul();
            }
        }

        public void AvansBul()
        {
            this.avansTutari = ToplamBul(ENUMYollukIslemTur.YOLLUK, ENUMYollukIslemTur.ZORUNLUMASRAF);
        }

        public int yil { get; set; }
        public string surecNo { get; set; }
        public ENUMYollukGundelikTuru yi_yd { get; set; }
        public string kimlikNo { get; set; }
        public int unvanKodu { get; set; }
        public string gerekce { get; set; }
        //public string nereden { get; set; }
        //public string nereye { get; set; }
        public TNSDateTime baslangicTarih { get; set; } = new TNSDateTime();
        public TNSDateTime bitisTarih { get; set; } = new TNSDateTime();
        public string butceTertibi { get; set; }
        public double avansTutari { get; set; }
        public string avansOdemeEmri { get; set; }
        public string odemeEmri { get; set; }
        public int durum { get; set; }
        public TNSCollection detay { get; set; } = new TNSCollection();
        public string soyad { get; set; }
        public string ad { get; set; }
        public string unvan { get; set; }
        public int istihdamSekli { get; set; } = 0;
        public int muhasebe { get; set; } = 0;
        public string birim { get; set; } = "";

        public bool TCMi
        {
            get
            {
                if (yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISIMISAFIR || yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISINDANGELISMISAFIR)
                    return firmaUyruk == 0;
                else
                    return istihdamSekli == 11 || istihdamSekli == 12 || istihdamSekli == 13 || istihdamSekli == 17 || istihdamSekli == 37;
            }
        }

        public ENUMGundelikTur YollukTuru
        {
            get
            {
                if (this.yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISI || this.yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISIMISAFIR) return ENUMGundelikTur.GIDIS;
                else if (this.yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISINDANGELIS || this.yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISINDANGELISMISAFIR) return ENUMGundelikTur.GELIS;
                return ENUMGundelikTur.YURTICI;
            }
        }

        public string firmaAd { get; set; }
        public int firmaUyruk { get; set; }
        public string KisiFirmaAd
        {
            get
            {
                if (yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISIMISAFIR || yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISINDANGELISMISAFIR)
                    return this.firmaAd;
                else
                    return this.ad + " " + this.soyad;
            }


        }

        public int KisiFirmaUnvanKodu
        {
            get
            {
                if (yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISIMISAFIR || yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISINDANGELISMISAFIR)
                    return 0;
                else
                    return this.unvanKodu;
            }

        }
        public string KisiFirmaUnvan
        {
            get
            {
                if (yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISIMISAFIR || yi_yd == ENUMYollukGundelikTuru.GECICIYURTDISINDANGELISMISAFIR)
                    return "Diğer";
                else
                    return this.unvan;
            }
        }

        public string gorevSuresiBul()
        {
            return this.baslangicTarih.ToString() + "-" + this.bitisTarih.ToString() + " " + gurevGunu() + " gün";
        }

        private int gurevGunu()
        {
            int toplam = 0;
            foreach (GeciciGorevYollukManasDetay item in detay)
            {
                if (item.tur == ENUMYollukIslemTur.YOLLUK)
                    toplam += item.gun;
            }
            return toplam;
        }

        public string gundelikBul()
        {
            string donusDeger = "";
            List<double> gLer = new List<double>();
            foreach (GeciciGorevYollukManasDetay item in detay)
            {
                if (item.tur == ENUMYollukIslemTur.YOLLUK)
                {
                    if (!gLer.Contains(item.gundelik))
                    {
                        if (donusDeger != "") donusDeger += ", ";
                        donusDeger += item.gundelik.ToString("0,00");
                        gLer.Add(item.gundelik);
                    }
                }
            }
            return donusDeger;
        }

        public bool Kontrol(Sonuc s)
        {
            if (string.IsNullOrWhiteSpace(kimlikNo)) s.DegerAta("TC Kimlik numarası dolu olmalı!");
            if (string.IsNullOrWhiteSpace(this.butceTertibi)) s.DegerAta("Bütçe tertibi dolu olmalı!");
            if (unvanKodu == 0) s.DegerAta("Unvan olmalı");
            if (TNSDateTime.IsNull(baslangicTarih)) s.DegerAta("Başlama tarihi dolu olmalı!");
            int toplamGun = 0;
            int toplamKonaklama = 0;
            int satir = 0;

            foreach (GeciciGorevYollukManasDetay item in detay)
            {
                satir++;
                if (item.tur == 0) s.DegerAta($"{satir}.satırda yolluk türünü seçiniz!");
                else
                {
                    if (item.tur == ENUMYollukIslemTur.YOLLUK || item.tur == ENUMYollukIslemTur.KONAKLAMA || item.tur == ENUMYollukIslemTur.ZORUNLUMASRAF)
                    {
                        if (string.IsNullOrWhiteSpace(item.yerKod)) s.DegerAta("Yer seçilmiş olmalı!");
                        else
                        {
                            string[] parcalar = item.yerKod.Split('.');
                            if (parcalar.Length < 3)
                                s.DegerAta("Yer olarak bir il seçilmelidir!");
                        }
                        if (item.gundelik <= 0) s.DegerAta($"{satir}. satırda gündelik değeri bulunamadı, ülke ve unvana göre değer girilip girilmediğini kontrol ediniz!");
                        if (item.gun < 1) s.DegerAta($"{satir}. satırda gün değeri hatalı!");
                        if (item.tur == ENUMYollukIslemTur.YOLLUK || item.tur == ENUMYollukIslemTur.ZORUNLUMASRAF)
                            toplamGun += item.gun;
                        else
                            toplamKonaklama += item.gun;
                    }
                    else
                    {
                        if (item.gun != 0)
                            s.DegerAta($"{satir}. satırda diğer harcamalar için gün girilmemeli!");
                        if (string.IsNullOrWhiteSpace(item.aciklama))
                            s.DegerAta($"{satir}. satırda uçak veya diğer harcamalar için açıklama girilmeli!");
                    }
                    if (item.tur == ENUMYollukIslemTur.YOLLUK || item.tur == ENUMYollukIslemTur.ZORUNLUMASRAF)
                    {
                        if (item.gecelikKonaklamaUcreti != 0) s.DegerAta($"{satir}. satırda otel geceliği/masraf ücreti girilmemeli!");

                    }
                    else if (item.gecelikKonaklamaUcreti <= 0) s.DegerAta($"{satir}. satırda otel geceliği/masraf ücreti girilmeli!");
                }
            }
            if (TNSDateTime.IsNull(bitisTarih)) s.DegerAta("Bitiş tarihi dolu olmalı!");
            else if (!TNSDateTime.IsNull(baslangicTarih) && bitisTarih.Oku() < baslangicTarih.Oku()) s.DegerAta("Başlangıç tarihi Bitiş tarihinden büyük olmamalı!");
            else
            {
                int gun = (bitisTarih.Oku() - baslangicTarih.Oku()).Days + 1;
                if (gun < toplamGun) s.DegerAta("Yolluk günleri toplamı, başlangıç bitiş tarihleri arasındaki farktan daha büyük!");
            }
            if (toplamKonaklama >= toplamGun)
                s.DegerAta("Kalış günleri toplamı, yolluk günleri toplamından küçük olmalı!");

            return s.islemSonuc;
        }

        public void OdenecekHesapla()
        {
            int toplamGundelik = 0;
            int toplamKonaklama = 0;
            for (int i = 0; i < detay.Count; i++)
            {
                GeciciGorevYollukManasDetay det = (GeciciGorevYollukManasDetay)detay[i];
                if (det.tur == ENUMYollukIslemTur.YOLLUK || det.tur == ENUMYollukIslemTur.ZORUNLUMASRAF)
                {
                    det.odenecek = det.GundelikHesapla(toplamGundelik, unvanKodu);
                    toplamGundelik += det.gun;
                }
                else if (det.tur == ENUMYollukIslemTur.KONAKLAMA)
                {
                    det.odenecek = det.KonaklamaHesapla(toplamKonaklama);
                    toplamKonaklama += det.gun;
                }
                else //ENUMYollukIslemTur.UCAK, ENUMYollukIslemTur.DIGER
                {
                    det.odenecek = det.gecelikKonaklamaUcreti;
                }
            }
        }

        public string YerleriBul()
        {
            string donusDeger = "";
            for (int i = 0; i < detay.Count; i++)
            {
                GeciciGorevYollukManasDetay det = (GeciciGorevYollukManasDetay)detay[i];
                if (det.tur == ENUMYollukIslemTur.YOLLUK)
                {
                    if (donusDeger != "") donusDeger += ", ";
                    donusDeger += det.yerAdSade;
                }
            }
            return donusDeger;
        }
    }

    [Serializable()]
    public class GeciciGorevYollukTalepBaslik
    {
        public int yil;
        public string formKod;
        public int muhasebe;
        public string birim;
        public string kurKod;
        public int durum;
        public TNSDateTime tarih;
        public string kimlikNo;
        public string ad;
        public string soyad;
        public string unvan;
        public int gundelikYil;
        public string gundelikKod;
        public decimal gundelikTutar;
        public int gundelikDovizKod;
        public string gerekce;
        public string nereden;
        public string nereye;
        public TNSDateTime baslangicTarih;
        public TNSDateTime bitisTarih;
        public int surecYil;
        public string surecNo;
        public int tur;
        //public int gorevSuresi;
        public TNSCollection detay;
        public GeciciGorevYollukBeraberler[] beraberindekiler = null;
        public string aciklamalar;
        public string butceTertibi;
        public double avansTutari;
        public string sunanUnvan;
        public string onayAd;
        public string onayUnvan;
        public string hazirUnvan;
        public string hazirAd;
        public string sunanAd;
        public string onayMakam;
        public string avansOdemeEmri;

        public GeciciGorevYollukTalepBaslik()
        {
            yil = 0;
            formKod = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
            kurKod = string.Empty;
            durum = 0;
            tarih = new TNSDateTime(DateTime.Now);
            kimlikNo = string.Empty;
            ad = string.Empty;
            soyad = string.Empty;
            unvan = string.Empty;
            gundelikYil = 0;
            gundelikKod = string.Empty;
            gundelikTutar = decimal.Zero;
            gundelikDovizKod = 0;
            nereden = string.Empty;
            nereye = string.Empty;
            baslangicTarih = new TNSDateTime(DateTime.Now);
            bitisTarih = new TNSDateTime(DateTime.Now);
            surecYil = 0;
            surecNo = string.Empty;
            tur = 0;
            //gorevSuresi = 0;
            detay = new TNSCollection();
            aciklamalar = string.Empty;
        }

        public int GorevSuresiBul()
        {
            if (detay == null) return 0;
            return detay.Count;//???
        }

        public double GorevSuresiBulAvans()
        {
            double toplam = 0;
            if (detay == null) return 0;
            foreach (GeciciGorevYollukTalepDetay item in detay)
            {
                if (item.gun == "1") toplam += 1;
                else if (item.gun == "1/2") toplam += 0.5;
            }
            return toplam;
        }

    }

    [Serializable()]
    public class GeciciGorevYollukBeraberler
    {
        private string _tcKimlik;
        public string tcKimlik
        {
            get
            {
                return _tcKimlik;
            }
            set
            {
                _tcKimlik = value;
            }
        }

        private string _adSoyad;
        public string adSoyad
        {
            get
            {
                return _adSoyad;
            }
            set
            {
                _adSoyad = value;
            }
        }
    }

    [Serializable()]
    public class GeciciGorevYollukTalepDetay
    {
        public int _tur;
        public int tur
        {
            get
            {
                return _tur;
            }
            set
            {
                _tur = value;
            }
        }

        public string _gun;
        public string gun
        {
            get
            {
                return _gun;
            }
            set
            {
                _gun = value;
            }
        }

        public TNSDateTime tarih;
        public DateTime tarihD
        {
            get
            {
                return tarih.Oku();
            }
            set
            {
                tarih.Yaz(value);
            }
        }

        public GeciciGorevYollukTalepDetay()
        {
            _tur = 0;
            tarih = new TNSDateTime();
            _gun = string.Empty;
        }
    }

    [Serializable()]
    public class GeciciGorevYollukTalepSorgu
    {
        public int yil;
        public string formKodBasla;
        public string formKodBitis;
        public TNSDateTime baslangicTarih;
        public TNSDateTime bitisTarih;
        public string kimlikNo;
        public int durum;
        public int surecYil;
        public string surecNo;
        public int muhasebe;
        public string birim;

        public GeciciGorevYollukTalepSorgu()
        {
            yil = 0;
            formKodBasla = string.Empty;
            formKodBitis = string.Empty;
            baslangicTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();
            kimlikNo = string.Empty;
            durum = 0;
            surecYil = 0;
            surecNo = string.Empty;
            muhasebe = 0;
            birim = string.Empty;
        }
    }

    [Serializable()]
    public class OnMaliKontrolAna
    {
        public int surecYil;
        public string surecNo;

        public ENUMOnMaliKontrolTur onMaliKontrolTur;
        public OnMaliKontrolDetay[] dizi;
        public string aciklama;
        public TNSDateTime tarih;
        public string islemiYapan;
        public int islemKod;
        public int parcaKod;

        public OnMaliKontrolDetay DetayBul(int kod)
        {
            if (dizi == null) return null;
            for (int i = 0; i < dizi.Length; i++)
            {
                if (dizi[i].id == kod) return dizi[i];
            }
            return null;
        }

        public string odemeEmriNo { get; set; }
    }

    [Serializable()]
    public class OnMaliKontrolDetay
    {
        public int id;
        public string ad;
        public int deger;

        public OnMaliKontrolDetay(int id, string ad)
        {
            this.id = id;
            this.ad = ad;
        }

        public OnMaliKontrolDetay(int id, int deger)
        {
            this.id = id;
            this.deger = deger;
        }

    }
    [Serializable()]
    public class TeminatMektubuNevi
    {
        public int kod = 0;
        public string adi = "";
        public string hesapKodu = "";
    }

    /// <summary>
    /// KismiGeciciKabulIMza: 
    /// </summary>
    [System.Serializable()]
    public class KismiGeciciKabulImza
    {
        /// <summary>
        /// Tc Kimlik No: Girilecek bilgi açıklaması.
        /// </summary>
        private string tcKimlikNo;

        public string TcKimlikNo
        {
            get { return tcKimlikNo; }
            set { tcKimlikNo = value; }
        }

        /// <summary>
        /// : Bir yıl önceki kesin harcama veya bilgiler.
        /// </summary>
        private string ad;

        public string Ad
        {
            get { return ad; }
            set { ad = value; }
        }

        /// <summary>
        /// : Bu yıla ait haziran harcaması veya bilgiler.
        /// </summary>
        private string soyad;

        public string Soyad
        {
            get { return soyad; }
            set { soyad = value; }
        }

        /// <summary>
        /// : Gelecek yıl için planlanan ödenek veya bilgiler.
        /// </summary>
        private string unvan;

        public string Unvan
        {
            get { return unvan; }
            set { unvan = value; }
        }
    }

    /// <summary>
    /// IhaleKabulMalzeme: Kabul Edilen Malzeme/Hizmetler
    /// </summary>
    [System.Serializable()]
    public class IhaleKabulMalzeme
    {
        /// <summary>
        /// Malzeme Kodu: Malzeme Kodu
        /// </summary>
        public string malKodu { get { return _malKodu; } set { _malKodu = value; } }
        private string _malKodu;


        /// <summary>
        /// Malzeme Adı: Malzeme Adı
        /// </summary>
        public string malAdi { get { return _malAdi; } set { _malAdi = value; } }
        private string _malAdi;

        /// <summary>
        /// Miktarı: Miktarı
        /// </summary>
        public double miktar { get { return _miktar; } set { _miktar = value; } }
        private double _miktar;

        /// <summary>
        /// Birim fiyatı (KDV Hariç): Birim fiyatı (KDV Hariç)
        /// </summary>
        public double birimFiyat { get { return _birimFiyat; } set { _birimFiyat = value; } }
        private double _birimFiyat;
    }


    /// <summary>
    /// KismiGeciciKabul: 
    /// </summary>
    [System.Serializable()]
    public class KismiGeciciKabul
    {

        /// <summary>
        /// Süreç Yılı: Bütçe yılını giriniz.
        /// </summary>
        public int surecYil;

        /// <summary>
        /// Birim Adı: Birim adını giriniz.
        /// </summary>
        public string surecNo;

        /// <summary>
        /// Sözleşme No: Sözleşme numarasını giriniz.
        /// </summary>
        public int malzemeSiraNo;

        /// <summary>
        /// Tertipler: Tertipleri giriniz, birden fazla ise virgülle ayırarak giriniz.
        /// </summary>
        public TNSDateTime yaziTarihi;

        /// <summary>
        /// : 
        /// </summary>
        public string aciklama;

        /// <summary>
        /// KismiGeciciKabulIMza: 
        /// </summary>
        public KismiGeciciKabulImza[] kismiGeciciKabulImza;

        public ObjectArray teklifListe = null;

        public SatinAlmaTalepIhtiyacTeklif TeklifBul(SatinAlmaTalepIhtiyacTeklif gelen)
        {
            if (teklifListe == null) return null;
            if (gelen == null) return null;
            foreach (SatinAlmaTalepIhtiyacTeklif teklif in teklifListe.objeler)
            {
                if (teklif.detaySatirNo == gelen.detaySatirNo) return teklif;
            }
            return null;
        }

        public decimal ToplamBul()
        {
            decimal toplam = 0;
            foreach (SatinAlmaTalepIhtiyacTeklif item in teklifListe.objeler)
            {
                toplam += item.birimFiyat * item.GerceklesenMiktar1;
            }
            return toplam;
        }
    }

    /// <summary>
    /// IhaleKomisyonUye: Komisyon Üyeleri
    /// </summary>
    [System.Serializable()]
    public class IhaleKomisyonUye
    {
        /// <summary>
        /// TC Kimlik No: TC Kimlik No
        /// </summary>
        public string tcKimlik;

        /// <summary>
        /// Görevi: Görevi
        /// </summary>
        public string gorev;

        public string adSoyad;
    }

    public enum ENUMIhaleKabulTutanakTur
    {
        HI_KismiKabul = 1,
        HI_KesinKabul = 2,
        Mal_Kabul = 3,
        Tanimsiz = 0
    }

    /// <summary>
    /// IhaleKabul: 
    /// </summary>
    [System.Serializable()]
    public class IhaleKabul
    {
        /// <summary>
        /// Süreç Yıl: Süreç yılı
        /// </summary>
        public int surecYil;

        /// <summary>
        /// Süreç No: Süreç numarası
        /// </summary>
        public string surecNo;

        /// <summary>
        /// Tutanak No: Tutanak Numarası
        /// </summary>
        public int tutanakNo;

        /// <summary>
        /// Tutanak Türü: Tutanak türünü giriniz
        /// </summary>
        public ENUMIhaleKabulTutanakTur tutanakTur;

        /// <summary>
        /// Firma No: Firma No
        /// </summary>
        public string vergiNo;

        /// <summary>
        /// Muhasebe Kodu: Muhasebe Kodu
        /// </summary>
        public int firmaMuhasebeKod;

        /// <summary>
        /// Sözleşme Tarihi: Sözleşme Tarihi
        /// </summary>
        public TNSDateTime sozlesmeTarihi;

        /// <summary>
        /// Sözleşme Bedeli: Sözleşme Bedeli
        /// </summary>
        public double sozlesmeBedeli;

        /// <summary>
        /// Fatura Tarihi: Fatura Tarihi
        /// </summary>
        public TNSDateTime faturaTarihi;

        /// <summary>
        /// Fatura No: Fatura No
        /// </summary>
        public string faturaNo;

        /// <summary>
        /// Fatura Tutarı (KDVsiz): Fatura Tutarı (KDVsiz)
        /// </summary>
        public double faturaTutari;

        /// <summary>
        /// İşin Süresi: İşin Süresi
        /// </summary>
        public string isinSuresi;

        /// <summary>
        /// Sözleşmeye Göre İşin Biteceği Tarih: Sözleşmeye Göre İşin Biteceği Tarih
        /// </summary>
        public TNSDateTime sozBitisTarihi;

        /// <summary>
        /// Varsa Süre Uzatımları: Varsa Süre Uzatımları
        /// </summary>
        public string sureUzatim;

        /// <summary>
        /// Süre Uzatımına Göre İşin Biteceği Tarih: Süre Uzatımına Göre İşin Biteceği Tarih
        /// </summary>
        public TNSDateTime sonBitisTarihi;

        /// <summary>
        /// Kabul Komisyonu Onay Tarihi: Kabul Komisyonu Onay Tarihi
        /// </summary>
        public TNSDateTime onayTarihi;

        /// <summary>
        /// Tutanak Tarihi: Tutanak Tarihi
        /// </summary>
        public TNSDateTime tutanakTarihi;

        /// <summary>
        /// IhaleKomisyonUye: Komisyon Üyeleri
        /// </summary>
        public IhaleKomisyonUye[] komUyeleri;

        public string firmaAd;

        public int malSiraNo;
    }

    /// <summary>
    /// CalisanBordroDetay: 
    /// </summary>
    [System.Serializable()]
    public class CalisanBordroDetay
    {

        /// <summary>
        /// gelirKesintiKod: Gelir Kesinti Kod
        /// </summary>
        public int gelirKesintiKod;

        /// <summary>
        /// miktar: 
        /// </summary>
        public double miktar;

        /// <summary>
        /// Tutar: 
        /// </summary>
        public string aciklama;

    }

    /// <summary>
    /// CalisanBordroDetay: 
    /// </summary>
    [System.Serializable()]
    public class CalisanBordroKisi : IComparable
    {
        /// <summary>
        /// Tc Kimlik No: 
        /// </summary>
        public string tcKimlikNo;

        /// <summary>
        /// Bu kişiye ait tüm bordro parametreleri
        /// </summary>
        public TNSCollection parametreler = null;

        public CalisanBordroDetay ParametreBul(int GelKesKod)
        {
            for (int i = 0; i < parametreler.Count; i++)
            {
                CalisanBordroDetay d = (CalisanBordroDetay)parametreler[i];
                if (d.gelirKesintiKod == GelKesKod)
                {
                    return d;
                }
            }
            return null;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is CalisanBordroKisi))
                return 0;
            CalisanBordroKisi p = (CalisanBordroKisi)obj;
            return 1;
        }

        public void ParametreEkle(CalisanBordroDetay detay)
        {
            if (parametreler == null)
            {
                parametreler = new TNSCollection();
            }

            parametreler.Add(detay);
        }
    }

    /// <summary>
    /// CalisanBordroAna: 
    /// </summary>
    [System.Serializable()]
    public class CalisanBordroAna
    {

        /// <summary>
        /// Süreç Yılı: 
        /// </summary>
        public int surecYil;

        /// <summary>
        /// Surec No: 
        /// </summary>
        public string surecNo;

        /// <summary>
        /// Odeme Emri No: 
        /// </summary>
        public string odemeEmriNo;

        public int odemeSiraNo;

        public TNSDateTime odemeGun;
        /// <summary>
        /// CalisanBordroDetay: 
        /// </summary>
        public CalisanBordroKisi[] kisiler;
    }


    /// <summary>
    /// CalisanBordroDetay: 
    /// </summary>
    [System.Serializable()]
    public class CalisanBordroDegisken
    {

        /// <summary>
        /// gelirKesintiKod: Gelir Kesinti Kod
        /// </summary>
        public int gelirKesintiKod;

        /// <summary>
        /// miktar: 
        /// </summary>
        public double miktar;

        /// <summary>
        /// Tutar: 
        /// </summary>
        public string aciklama;

    }

    ///// <summary>
    ///// CalisanBordroDetay: 
    ///// </summary>
    //[System.Serializable()]
    //public class CalisanDinamikBordroDegisken
    //{
    //    /// <summary>
    //    /// gelirKesintiKod: Gelir Kesinti Kod
    //    /// </summary>
    //    public string degiskenAd;

    //    /// <summary>
    //    /// miktar: 
    //    /// </summary>
    //    public object deger;
    //}

    ///// <summary>
    ///// CalisanDinamikBordroDetay: 
    ///// </summary>
    //[System.Serializable()]
    //public class CalisanDinamikBordroKisi //: IComparable
    //{
    //    /// <summary>
    //    /// Tc Kimlik No: 
    //    /// </summary>
    //    public string tcKimlikNo;

    //    /// <summary>
    //    /// Bu kişiye ait tüm bordro parametreleri
    //    /// </summary>
    //    public TNSCollection parametreler = null;

    //    //public CalisanDinamikBordroDegisken ParametreBul(string degiskenFormulAd)
    //    //{
    //    //    for (int i = 0; i < parametreler.Count; i++)
    //    //    {
    //    //        CalisanDinamikBordroDegisken d = (CalisanDinamikBordroDegisken)parametreler[i];
    //    //        if (d.degiskenAd == degiskenFormulAd)
    //    //        {
    //    //            return d;
    //    //        }
    //    //    }
    //    //    return null;
    //    //}

    //    //public int CompareTo(object obj)
    //    //{
    //    //    if (!(obj is CalisanDinamikBordroKisi))
    //    //        return 0;
    //    //    CalisanDinamikBordroKisi p = (CalisanDinamikBordroKisi)obj;
    //    //    return 1;
    //    //}

    //    //public void ParametreEkle(CalisanDinamikBordroDegisken detay)
    //    //{
    //    //    if (parametreler == null)
    //    //    {
    //    //        parametreler = new TNSCollection();
    //    //    }
    //    //    parametreler.Add(detay);
    //    //}
    //}

    /// <summary>
    /// CalisanDinamikBordroDonem: 
    /// </summary>
    [System.Serializable()]
    public class CalisanDinamikBordroDonem
    {
        public CalisanDinamikBordroDonem(int yil, string surecNo)
        {
            this.yil = yil;
            this.surecNo = surecNo;
        }

        public int yil { get; set; }
        public string surecNo { get; set; }

        public TNSDateTime hazirlamaTarih { get; set; }
        public DateTime hazirlamaTarihDT
        {
            get
            {
                if (hazirlamaTarih == null) hazirlamaTarih = new TNSDateTime();
                return hazirlamaTarih.Oku();
            }
            set
            {
                if (hazirlamaTarih == null) hazirlamaTarih = new TNSDateTime();
                hazirlamaTarih.Yaz(value);
            }
        }

        public TNSDateTime odemeTarih { get; set; }
        public DateTime odemeTarihDT
        {
            get
            {
                if (odemeTarih == null) odemeTarih = new TNSDateTime();
                return odemeTarih.Oku();
            }
            set
            {
                if (odemeTarih == null) odemeTarih = new TNSDateTime();
                odemeTarih.Yaz(value);
            }
        }

        public TNSDateTime bordroDonemBasTarih { get; set; }
        public DateTime bordroDonemBasTarihDT
        {
            get
            {
                if (bordroDonemBasTarih == null) bordroDonemBasTarih = new TNSDateTime();
                return bordroDonemBasTarih.Oku();
            }
            set
            {
                if (bordroDonemBasTarih == null) bordroDonemBasTarih = new TNSDateTime();
                bordroDonemBasTarih.Yaz(value);
            }
        }

        public DateTime oncBordroDonemBasTarihDT
        {
            get
            {
                return bordroDonemBasTarihDT.AddMonths(-1);
            }
        }

        public string hazirlayan { get; set; }
        public string onayaSunan { get; set; }
        public string onaylayan { get; set; }

        public TNSCollection MifFormListe = null;
        //public TNSCollection taahhutMIFListe = null;
        public BordroYapi bordroYapi = null;

        public Sonuc HataKontrol(int pesinMi, int maasDonemGun, int maasOdemeGun, BordroIslemTur.ENUMBordroIslemTurYapi bordroIslem)
        {
            Sonuc s = new Sonuc();
            if (bordroDonemBasTarih == null || bordroDonemBasTarih.isNull) s.DegerAta("Bordro başlangıç (Hakediş başlangıç) tarihi girilmeli");
            else if (maasDonemGun > 0 && (bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Normal || bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Fark))
            {
                if (maasDonemGun != bordroDonemBasTarih.Gun) s.DegerAta("Bordro döneminin ilk günü " + maasDonemGun + " olarak girilmeli!");
            }
            if (hazirlamaTarih == null || hazirlamaTarih.isNull) s.DegerAta("Hazırlama tarihi girilmeli");
            if (odemeTarih == null || odemeTarih.isNull) s.DegerAta("Ödeme tarihi girilmeli");
            else if (maasOdemeGun > 0 && (bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Normal || bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Fark))
            {
                if (maasOdemeGun != odemeTarih.Gun) s.DegerAta("Bordro ödeme tarihinin  günü " + maasOdemeGun + " olarak girilmeli!");
            }
            int maksGunFark = 20;
            if (s.islemSonuc)
            {
                if (hazirlamaTarihDT > odemeTarihDT)
                    s.DegerAta("Hazırlama tarihi, ödeme tarihinden önce olmalı!");
                else
                {
                    TimeSpan gunFark = odemeTarihDT - hazirlamaTarihDT;
                    if (gunFark.Days > maksGunFark)
                        s.DegerAta("Hazırlama tarihi ile ödeme tarihin arasında en fazla " + maksGunFark + " gün olabilir!");
                }
                if (bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Normal || bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Normal)
                {
                    TimeSpan gunFark2 = hazirlamaTarihDT - hazirlamaTarihDT;
                    if (pesinMi == 1)
                    {
                        if (hazirlamaTarihDT >= bordroDonemBasTarihDT && bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Normal)
                        {
                            s.DegerAta("Peşin maaşlarda bordro hazırlama tarihi maaş döneminden önce olmalıdır!");
                        }
                        else
                        {
                            gunFark2 = bordroDonemBasTarihDT - hazirlamaTarihDT;
                        }
                    }
                    else
                    {
                        if (hazirlamaTarihDT < bordroDonemBasTarihDT && bordroIslem == BordroIslemTur.ENUMBordroIslemTurYapi.Normal)
                        {
                            s.DegerAta("Çalışılıp alınan maaşlarda hazırlama tarihi maaş döneminden sonra olmalıdır!");
                        }
                        else
                        {
                            gunFark2 = hazirlamaTarihDT - sonGunDT;
                        }
                    }
                    if (gunFark2.Days > 30)
                        s.DegerAta("Hazırlama tarihi ile bordro dönemi arasında en fazla 30 gün olabilir!");
                }
            }

            return s;
        }

        public bool GecerliOEVarMi()
        {
            if (MifFormListe == null) return false;
            foreach (TNS.MUH.OdemeEmriMIFForm item in MifFormListe)
            {
                if (item.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.YENI || item.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.DEGISTIRILDI || item.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.ONAYLI || item.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                    return true;
            }
            return false;
        }

        public bool OnayliMIFVarMi()
        {
            if (MifFormListe == null) return false;
            foreach (TNS.MUH.OdemeEmriMIFForm item in MifFormListe)
            {
                if (item.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.ONAYLI || item.formDurum == (int)TNS.MUH.ENUMBelgeDurumu.YEVMIYE_OESGBYEGONDER)
                    return true;
            }
            return false;
        }

        public TNSDateTime AktifTarih(bool isciMi)
        {
            if (isciMi)
                return sonGun;
            else
                return bordroDonemBasTarih;
        }

        public static string AktifTarihYAG(TNSDateTime dt)
        {
            return dt.Yil.ToString() + "-" + dt.Ay.ToString() + "-" + dt.Gun.ToString();
        }

        public TNSDateTime sonGun
        {
            get
            {
                return new TNSDateTime(sonGunDT);
            }
        }

        public DateTime sonGunDT
        {
            get
            {
                DateTime dt = bordroDonemBasTarihDT;
                if (dt == DateTime.MinValue)
                    return DateTime.MinValue;
                dt = dt.AddMonths(1);
                dt = dt.AddDays(-1);
                return dt;
            }
        }

        public int muhasebeKod { get; set; }

        public string birimKod { get; set; }

        public string muhasebeAd { get; set; }

        public string birimAd { get; set; }

        public DateTime sonGunDTEkle1
        {
            get
            {
                DateTime dt = bordroDonemBasTarihDT;
                if (dt == DateTime.MinValue)
                    return DateTime.MinValue;
                dt = dt.AddMonths(1);
                return dt;
            }
        }

        public int islemAy
        {
            get
            {
                return AktifTarih(true).Ay;
            }
        }

        public string bordroTur { get; set; }

        public int bordroIslem { get; set; }

        public CalisanDinamikBordro calisanDinamikBordro { get; set; }
    }

    public class CalisanDinamikBordroSirala : IComparer
    {
        //public IntKodStrAd[] alanlar = null;
        public IntKodStrAd[] alanlar = null;
        public BordroYapi bordroYapi;

        public CalisanDinamikBordroSirala(BordroYapi bordroYapi)
        {
            this.bordroYapi = bordroYapi;
            this.alanlar = bordroYapi.SiralamaBul();
        }

        public int Compare(object x, object y)
        {
            KisiSatir kscx = (KisiSatir)x;
            KisiSatir kscy = (KisiSatir)y;
            int sonuc = 0;
            if (kscx.kisiSatirBilgi != null && kscy.kisiSatirBilgi != null)
            {
                sonuc = kscx.kisiSatirBilgi.oncBordroTarih.Oku().CompareTo(kscy.kisiSatirBilgi.oncBordroTarih.Oku());
                if (sonuc != 0) return sonuc;
            }
            if (alanlar == null) return 0;
            object[] cx = kscx.degerler;
            object[] cy = kscy.degerler;

            for (int i = 0; i < alanlar.Length; i++)
            {
                int carp = 1;
                string yon = alanlar[i].ad.Substring(0, 1);
                if (yon == "-") carp = -1;
                string alan = alanlar[i].ad.Substring(1);
                sonuc = Kiyasla(CalisanDinamikBordro.DegerBul(bordroYapi, kscx, alan), CalisanDinamikBordro.DegerBul(bordroYapi, kscy, alan)) * carp;
                if (sonuc != 0) return sonuc;
            }
            return 0;
        }

        private int Kiyasla(object deger1, object deger2)
        {
            if (deger1 == null && deger2 == null) return 0;
            if (deger1 == null) return -1;
            if (deger2 == null) return 1;
            if (typeof(int) == deger1.GetType())
                return ((int)deger1).CompareTo((int)deger2);
            else if (typeof(double) == deger1.GetType())
                return ((double)deger1).CompareTo((double)deger2);
            else if (typeof(DateTime) == deger1.GetType())
                return ((DateTime)deger1).CompareTo((DateTime)deger2);
            else
                return (deger1.ToString()).CompareTo(deger2.ToString());
        }
    }


    [Serializable()]
    public class BordroBilgi
    {
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BordroBilgi gelObj = (BordroBilgi)obj;
            if (gelObj.personelTur == this.personelTur && gelObj.kesintiAd == this.kesintiAd)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public enum KesintiTur : int
        {
            Eklenti = 1,
            EklentiDVli = 9,
            KesintiOran = 2,
            KesintiTam = 3,
            KesintiIadeTam = 5,
        }

        public int id;
        public int personelTur;
        public string kesintiAd;
        public string alanAd;
        public KesintiTur kesintiEklentiTur;
        public double toplam = 0;
        public bool hep30 = false;

        public bool eklentiMi { get { return (kesintiEklentiTur == KesintiTur.Eklenti || kesintiEklentiTur == KesintiTur.EklentiDVli); } }

        public BordroBilgi(int id, int personelTur, string kesintiAd, string alanAd, KesintiTur kesinti, bool hep30 = false)
        {
            this.id = id;
            this.personelTur = personelTur;
            this.kesintiAd = kesintiAd;
            this.alanAd = alanAd;
            this.kesintiEklentiTur = kesinti;
            this.hep30 = hep30;
        }

        public BordroBilgi(BordroBilgi item)
        {
            this.personelTur = item.personelTur;
            this.kesintiAd = item.kesintiAd;
            this.alanAd = item.alanAd;
            this.kesintiEklentiTur = item.kesintiEklentiTur;
            this.hep30 = item.hep30;
        }
    }


    [Serializable()]
    public class KisiSatirBilgi
    {
        public TNSDateTime odemeTarih = new TNSDateTime();
        public int muhasebeKod = 0;
        public string muhasebeAd = "";
        public string birimKod = "";
        public string birimAd = "";
        public int surecYil = 0;
        public string surecNo = "";

        public string hazirlayan = "";
        public string onayaSunan = "";
        public string onaylayan = "";

        public TNSDateTime oncBordroTarih;
    }

    [Serializable()]
    public class KisiSatir
    {
        public object[] degerler;
        public object[] degerlerOnc;
        public KisiSatirBilgi kisiSatirBilgi = null;

        public KisiSatir(object[] degerler)
        {
            this.degerler = degerler;
        }
    }

    /// <summary>
    /// CalisanDinamikBordro: 
    /// </summary>
    [System.Serializable()]
    public class CalisanDinamikBordro
    {
        /// <summary>
        /// Süreç Yılı: 
        /// </summary>
        public int surecYil;

        /// <summary>
        /// Surec No: 
        /// </summary>
        public string surecNo;

        /// <summary>
        /// Odeme Emri No: 
        /// </summary>
        public string odemeEmriNo;

        public int odemeSiraNo;

        public TNSDateTime odemeGun;

        public System.Collections.ArrayList kisiDegerler = null;

        public static object DegerBul(BordroYapi bordroYapi, KisiSatir kisiDegerler, string alan)
        {
            if (alan == "0") return kisiDegerler.degerler[0];
            int yer = bordroYapi.YerBul(alan, 0);

            if (yer >= 0)
            {
                return kisiDegerler.degerler[yer + 1];
            }
            return null;
        }

        public static int DegerBulInt(BordroYapi by, KisiSatir al, string alan)
        {
            object o = DegerBul(by, al, alan);
            if (o == null) return 0;
            return (int)o;
        }

        public KisiSatir KisiBul(KisiSatir oncKisi)
        {
            if (oncKisi != null && oncKisi.degerler != null && oncKisi.degerler.Length > 0)
            {
                foreach (KisiSatir kisi in this.kisiDegerler)
                {
                    if (kisi != null && kisi.degerler != null && kisi.degerler.Length > 0 && (string)kisi.degerler[0] == (string)oncKisi.degerler[0])
                        return kisi;
                }
            }
            return null;
        }

        public int toplamKayitSay = 0;
    }

    public enum ENUMGelirKesintiTur
    {
        GELIR = 1,
        KESINTI = 2,
        BILGI = 3,
        NUMERIKBILGI = 4
    }

    /// <summary>
    /// CalisanBordroGelirKesinti: 
    /// </summary>
    [System.Serializable()]
    public class CalisanBordroGelirKesinti
    {

        /// <summary>
        /// gelirKesintiKod: Gelir Kesinti Kodu
        /// </summary>
        public int gelirKesintiKod;

        /// <summary>
        /// gelirKesintiTur: Gelir Kesinti Türü 1: Gelir(sayı), 2: Kesinti(sayı), 3: Bilgi(yazı)
        /// </summary>
        public ENUMGelirKesintiTur gelirKesintiTur;

        /// <summary>
        /// Hesap Kodu: 
        /// </summary>
        public string hesapKodu;

        /// <summary>
        /// Adı: 
        /// </summary>
        public string adi;

        /// <summary>
        /// Sira No: Parametrelerin ekranda gösteriliş sırası için
        /// </summary>
        public string siraNo;

        /// <summary>
        /// Kisi Sira No: Kişilerin Unvanına göre sıralanması için
        /// </summary>
        public string kisiSira;

        /// <summary>
        /// formul: Hesaplamalar için kullanılacak formuller
        /// </summary>
        public string formul;

        /// <summary>
        /// Formul Sira: Formullerin kullanılış sırası
        /// </summary>
        public string formulSira;
    }

    /// <summary>
    /// PSLGeciciGorevUcakFatura: Ödenecek Faturalar
    /// </summary>
    [System.Serializable()]
    public class PSLGeciciGorevUcakFatura
    {
        private TNSDateTime _tarih;
        /// <summary>
        /// Tarih: 
        /// </summary>
        public TNSDateTime tarih { get { return _tarih; } set { _tarih = value; } }
        /// <summary>
        /// Tarih: 
        /// </summary>
        public DateTime tarihDT
        {
            get
            {
                if (_tarih == null)
                    return DateTime.MinValue;
                else
                    return _tarih.Oku();
            }
            set
            {
                if (_tarih == null) _tarih = new TNSDateTime(value);
                else _tarih.Yaz(value);
            }
        }

        private string _faturaNo;
        /// <summary>
        /// Fatura No: 
        /// </summary>
        public string faturaNo { get { return _faturaNo; } set { _faturaNo = value; } }

        private double _tutar;
        /// <summary>
        /// Tutar: KDV Dahil fatura tutarı
        /// </summary>
        public double tutar { get { return _tutar; } set { _tutar = value; } }

        private int _komisyonMu;
        /// <summary>
        /// Komisyon Mu: 
        /// </summary>
        public int komisyonMu { get { return _komisyonMu; } set { _komisyonMu = value; } }
    }


    /// <summary>
    /// PSLGeciciGorevUcak: 
    /// </summary>
    [System.Serializable()]
    public class PSLGeciciGorevUcak
    {

        private int _surecYil;
        /// <summary>
        /// Süreç Yıl: 
        /// </summary>
        public int surecYil { get { return _surecYil; } set { _surecYil = value; } }

        private string _surecNo;
        /// <summary>
        /// Süreç No: 
        /// </summary>
        public string surecNo { get { return _surecNo; } set { _surecNo = value; } }

        public FirmaBilgisi firma;

        //private string _tCKimlikveyaVergiNo;
        ///// <summary>
        ///// TCKimlik veya VergiNo: 
        ///// </summary>
        //public string tCKimlikveyaVergiNo { get { return _tCKimlikveyaVergiNo; } set { _tCKimlikveyaVergiNo = value; } }

        private string _odemeEmriNo;
        /// <summary>
        /// Ödeme Emri No: 
        /// </summary>
        public string odemeEmriNo { get { return _odemeEmriNo; } set { _odemeEmriNo = value; } }

        public int _odemeEmriYil;
        public int odemeEmriYil { get { return _odemeEmriYil; } set { _odemeEmriYil = value; } }

        /// <summary>
        /// PSLGeciciGorevUcakFatura: Ödenecek Faturalar
        /// </summary>
        public PSLGeciciGorevUcakFatura[] faturalar;

        public PSLGeciciGorevUcak()
        {
            firma = new FirmaBilgisi();
        }

        public double FaturaToplam(int tur)
        {
            if (faturalar == null || faturalar.Length == 0) return 0;
            double toplam = 0;
            for (int i = 0; i < faturalar.Length; i++)
            {
                if (faturalar[i].komisyonMu == tur || tur == -1)
                    toplam += faturalar[i].tutar;
            }
            return toplam;
        }

        public string FaturaNolar
        {
            get
            {
                if (faturalar == null || faturalar.Length == 0) return "";
                return faturalar[0].faturaNo;
            }
        }

        public TNSDateTime FaturaTarih
        {
            get
            {
                if (faturalar == null || faturalar.Length == 0) return null;
                return faturalar[0].tarih;
            }
        }

        public string FaturaOzetBul()
        {
            string donusDeger = "";
            if (faturalar == null || faturalar.Length == 0) return donusDeger;
            for (int i = 0; i < faturalar.Length; i++)
            {
                if (i > 0) donusDeger += ";";
                donusDeger += faturalar[i].faturaNo + " (" + faturalar[i].tarih.ToString() + ")";
            }
            return donusDeger;
        }
    }

    /// <summary>
    /// DovizOdeme: 
    /// </summary>
    [System.Serializable()]
    public class DovizOdeme
    {

        private int _surecYili;
        /// <summary>
        /// Süreç Yılı: Süreç yılını giriniz.
        /// </summary>
        public int surecYili { get { return _surecYili; } set { _surecYili = value; } }

        private string _surecNo;
        /// <summary>
        /// Sureç No: Süreç no giriniz.
        /// </summary>
        public string surecNo { get { return _surecNo; } set { _surecNo = value; } }

        public FirmaBilgisi firma;

        public string tcKimlikveyaVergiNo { get { return firma.vergiNo; } set { firma.vergiNo = value; } }

        public string firmaveyaKisiAd { get { return firma.ad; } set { firma.ad = value; } }

        private string _odemeemriNo;
        /// <summary>
        /// Odeme emri No: Odeme emri no giriniz.
        /// </summary>
        public string odemeemriNo { get { return _odemeemriNo; } set { _odemeemriNo = value; } }

        private int _odemeemriyil;
        /// <summary>
        /// Ödeme emri yıl: Ödeme emri yılını giriniz.
        /// </summary>
        public int odemeemriyil { get { return _odemeemriyil; } set { _odemeemriyil = value; } }

        private string _gerekce;
        /// <summary>
        /// Gerekçe: Gerekçe giriniz.
        /// </summary>
        public string gerekce { get { return _gerekce; } set { _gerekce = value; } }

        private double _tutar;
        /// <summary>
        /// Tutar: Tutar giriniz.
        /// </summary>
        public double tutar { get { return _tutar; } set { _tutar = value; } }

        private string _tertipler;
        /// <summary>
        /// Tertipler: Tertipleri giriniz, birden fazla ise virgülle ayırarak giriniz.
        /// </summary>
        public string tertipler { get { return _tertipler; } set { _tertipler = value; } }

        private string _odemeHesapKodu;
        /// <summary>
        /// Tertipler: Tertipleri giriniz, birden fazla ise virgülle ayırarak giriniz.
        /// </summary>
        public string odemeHesapKodu { get { return _odemeHesapKodu; } set { _odemeHesapKodu = value; } }

        private string _paraBirimi;
        public string paraBirimi { get { return _paraBirimi; } set { _paraBirimi = value; } }


        public DovizOdeme()
        {
            firma = new FirmaBilgisi();
        }


    }

    /// <summary>
    /// AvukatOdemesi: 
    /// </summary>
    [System.Serializable()]
    public class AvukatOdemesi
    {
        private int _surecYili;
        /// <summary>
        /// Süreç Yılı: Süreç Yılı giriniz.
        /// </summary>
        public int surecYili { get { return _surecYili; } set { _surecYili = value; } }

        private string _surecNo;
        /// <summary>
        /// Süreç No : Süreç No giriniz.
        /// </summary>
        public string surecNo { get { return _surecNo; } set { _surecNo = value; } }

        public string tcKimlikveyaVergiNo { get { return firma.vergiNo; } set { firma.vergiNo = value; } }

        public string firmaveyaKisiAd { get { return firma.ad; } set { firma.ad = value; } }

        private string _kisiHesapKodu;
        /// <summary>
        /// Kişi Hesap Kodu: Kişi Hesap Kodu giriniz
        /// </summary>
        public string kisiHesapKodu { get { return _kisiHesapKodu; } set { _kisiHesapKodu = value; } }

        private string _odemeEmriNo;
        /// <summary>
        /// Ödeme Emri No: Ödeme Emri No giriniz.
        /// </summary>
        public string odemeEmriNo { get { return _odemeEmriNo; } set { _odemeEmriNo = value; } }

        private int _odemeEmriYil;
        /// <summary>
        /// Ödeme Emri Yıl: Ödeme Emri Yıl giriniz
        /// </summary>
        public int odemeEmriYil { get { return _odemeEmriYil; } set { _odemeEmriYil = value; } }

        private string _gerekce;
        /// <summary>
        /// Gerekce: Gerekce giriniz
        /// </summary>
        public string gerekce { get { return _gerekce; } set { _gerekce = value; } }

        private double _brutUcret;
        /// <summary>
        /// Brüt Ücret: Brüt Ücret
        /// </summary>
        public double brutUcret { get { return _brutUcret; } set { _brutUcret = value; } }

        private double _odenecekTutar;
        /// <summary>
        /// Ödenecek Tutar: Ödenecek Tutar giriniz
        /// </summary>
        public double odenecekTutar { get { return _odenecekTutar; } set { _odenecekTutar = value; } }

        private string _tertipler;
        /// <summary>
        /// Tertipler: Tertipleri giriniz
        /// </summary>
        public string tertipler { get { return _tertipler; } set { _tertipler = value; } }

        private string _odemeHesapKodu;

        /// <summary>
        /// Ödeme Hesap Kodu: Ödeme Hesap Kodu giriniz
        /// </summary>
        public string odemeHesapKodu { get { return _odemeHesapKodu; } set { _odemeHesapKodu = value; } }

        private string _paraBirimi;

        /// <summary>
        /// Para Birimi: Para Birimi giriniz
        /// </summary>
        public string paraBirimi { get { return _paraBirimi; } set { _paraBirimi = value; } }

        private TNSDateTime _faturaTarihi;

        /// <summary>
        /// Fatura Tarihi: Fatura Tarihi giriniz
        /// </summary>
        public TNSDateTime faturaTarihi { get { return _faturaTarihi; } set { _faturaTarihi = value; } }

        public DateTime faturaTarihiDT
        {
            get
            {
                if (_faturaTarihi == null)
                    return DateTime.MinValue;
                else
                    return _faturaTarihi.Oku();
            }
            set
            {
                if (_faturaTarihi == null) _faturaTarihi = new TNSDateTime(value);
                else _faturaTarihi.Yaz(value);
            }
        }

        private string _faturaNumarasi;
        /// <summary>
        /// Fatura Numarası: Fatura Numarası giriniz
        /// </summary>
        public string faturaNumarasi { get { return _faturaNumarasi; } set { _faturaNumarasi = value; } }

        public FirmaBilgisi firma;

        public AvukatOdemesi()
        {
            firma = new FirmaBilgisi();
        }
    }

    [System.Serializable()]
    public class SurecOzetBilgi
    {
        public int SURECYIL { get; set; }

        public string SURECNO { get; set; }

        public int MUHASEBEKOD { get; set; }

        public string BIRIMKOD { get; set; }

        public int ISLEMKOD { get; set; }

        public TNSDateTime SURECTARIH { get; set; }

        public int SURECDURUM { get; set; }

        public string ACIKLAMA { get; set; }

        public string ISINADI { get; set; }

        public int YILLARASARIISLEMMI { get; set; }

        public string HRCIS_AD { get; set; }

        public int HRCIS_TUR { get; set; }

        public string ONAY_ISTANIM { get; set; }

        public string ONAY_ISNITELIK { get; set; }

        public string ONAY_ISMIKTAR { get; set; }

        public int ONAY_IHALEUSULU { get; set; }

        public int ONAY_IHALEMADDESI { get; set; }

        public string MADDE_ADI { get; set; }

        public string MADDE_ACIKLAMA { get; set; }
    }

    //public enum ENUMBordroIslemTur : int
    //{
    //    Normal = 1,
    //    Fark = 2,
    //    Kist = 3
    //}

    public enum ENUMBordroDegiskenTur : int
    {
        Elle = 1,
        Kisisel = 2,
        KisiBordroBilgi = 3,
        //KisiBordroBilgi = 4,

        Formul = 99,
    }

    [Serializable()]
    public class BordroGenelDegisken
    {
        private static BordroGenelDegisken[] _bordroDegiskenler = null;
        public static void DegiskenleriSifirla()
        {
            _bordroDegiskenler = null;
        }

        public static BordroGenelDegisken[] bordroDegiskenler
        {
            get
            {
                if (_bordroDegiskenler == null)
                {
                    ArrayList al = new ArrayList(_bordroSabitlerDegiskenler);

                    ObjectArray oa = TNS.HRC.Arac.Tanimla().ListelePersonelBordroParam(null);
                    al.AddRange(oa.objeler);

                    ///*
                    //    new BordroGenelDegisken("banka", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Banka kodu"),
                    //    new BordroGenelDegisken("sube", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Banka şubesi"),
                    //    new BordroGenelDegisken("lojmanKirasi", ENUMBordroDegiskenTur.KisiBordroGider, VariableType.Money, "Lojman Kirası"),
                    //    new BordroGenelDegisken("yakit", ENUMBordroDegiskenTur.KisiBordroGider, VariableType.Money, "Yakıt"),
                    //    new BordroGenelDegisken("sakatlikIndirimi", ENUMBordroDegiskenTur.KisiBordroGelir, VariableType.Money, "Sakatlık İndirimi"),
                    //    new BordroGenelDegisken("yabDilTazminatOrani", ENUMBordroDegiskenTur.KisiBordroGelir, VariableType.Money, "Yabancı Dil Tazminat Oranı"),

                    // * 
                    // * */
                    //al.AddRange(new BordroGenelDegisken[]{
                    //    //new BordroGenelDegisken("ucretKatsayisi", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Ücret Katsayısı"),
                    //    //new BordroGenelDegisken("emsalOzelHizmet", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Emsal Özel Hizmet"),
                    //    //new BordroGenelDegisken("tazminatKatsayisi", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Tazminat Katsayisi"),
                    //    //new BordroGenelDegisken("ekTazminatKatsayisi", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Ek Tazminat Katsayisi"),
                    //    //new BordroGenelDegisken("isGuclugu", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "İş Güçlüğü"),
                    //    //new BordroGenelDegisken("isRiski", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "İş Riski"),
                    //    //new BordroGenelDegisken("temindeGuclukZammi", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Teminde Güçlük Zammı"),
                    //    //new BordroGenelDegisken("maliSorumlulukTazminati", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Mali Sorumluluk Tazminatı"),
                    //    //new BordroGenelDegisken("ozelSigortaTutari", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Özel Sigorta Tutarı"),        
                    //    //new BordroGenelDegisken("gecmisHizmetBorcu", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Geçmiş Hizmet Borcu"),
                    //    //new BordroGenelDegisken("icraKesintisi", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "İcra Kesintisi"),
                    //    //new BordroGenelDegisken("nafakaKesintisi", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Nafaka Kesintisi"),
                    //    //new BordroGenelDegisken("kefaletAidati", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Kefalet Aidatı"),
                    //    //new BordroGenelDegisken("ayliktanKesmeCezasi", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Aylıktan Kesme Cezası"),
                    //    //new BordroGenelDegisken("ikrazBorcu", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "İkraz Borcu"),
                    //    //new BordroGenelDegisken("sahisBorcu", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Şahıs Borcu"),
                    //    //new BordroGenelDegisken("yardimSanAidatı", ENUMBordroDegiskenTur.KisiBordroBilgi, VariableType.Money, "Yardım San. Aidatı"),
                    //});
                    _bordroDegiskenler = (BordroGenelDegisken[])al.ToArray(typeof(BordroGenelDegisken));
                }
                return _bordroDegiskenler;
            }
        }

        private static BordroGenelDegisken[] _bordroSabitlerDegiskenler =
        {
            new BordroGenelDegisken("VERİ GİRİŞ", ENUMBordroDegiskenTur.Elle, VariableType.Bilinmiyor, "Kullanıcının veri gireceği alan"),
            new BordroGenelDegisken("ad", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Adı"),
            new BordroGenelDegisken("soyad", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Soyadı"),
            new BordroGenelDegisken("sicilNo", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Kurum Sicil No"),
            new BordroGenelDegisken("sigortaSicilNo", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "SGK Sicil Numarası"),
            new BordroGenelDegisken("unvan", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Unvanı"),
            new BordroGenelDegisken("unvanKod", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Unvan Kodu"),
            new BordroGenelDegisken("unvanSira", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Unvan Sıra Kodu"),
            new BordroGenelDegisken("vekilUnvanAd", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Vekalet Unvanı"),
            new BordroGenelDegisken("vekilUnvanKod", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Vekalet Unvan Kodu"),
            new BordroGenelDegisken("meslekKodu", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Meslek Kodu"),
            new BordroGenelDegisken("hizmetSinifi", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Hizmet Sınıfı"),
            new BordroGenelDegisken("cinsiyet", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Cinsiyet"),
            new BordroGenelDegisken("es", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "0:Eşi Yok,1:Eşi Çalışmıyor,2:Eşi Çalışıyor)"),
            new BordroGenelDegisken("cocukSay6YasdanKucuk", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Bakmakla Yükümlü Olduğu 6 yaşından küçük çocuk sayısı"),
            new BordroGenelDegisken("cocukSay6YasdanBuyuk", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Bakmakla Yükümlü Olduğu 6 yaşından büyük çocuk sayısı"),
            new BordroGenelDegisken("cocukSay6YasdanKucukE", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Bakmakla Yükümlü Olduğu 6 yaşından küçük Eng. çocuk sayısı"),
            new BordroGenelDegisken("cocukSay6YasdanBuyukE", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Bakmakla Yükümlü Olduğu 6 yaşından büyük Eng. çocuk sayısı"),
            new BordroGenelDegisken("cocukSay", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "AGİ için çocuk sayısı"),
            new BordroGenelDegisken("sakatlikDerecesi", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Sakatlık Derecesi"),
            new BordroGenelDegisken("IBAN", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Banka hesap numarası"),
            new BordroGenelDegisken("iseBaslaTar", ENUMBordroDegiskenTur.Kisisel, VariableType.Date, "İşe Başlama Tarihi"),
            new BordroGenelDegisken("istenCikisTar", ENUMBordroDegiskenTur.Kisisel, VariableType.Date, "İşten Ayrılış Tarihi"),
            new BordroGenelDegisken("istenCikisNeden", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "İşten Ayrılış Neden Kod"),
            new BordroGenelDegisken("kidemBilgi", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Kıdem hesabı için kullanılacak bilgi"),
            new BordroGenelDegisken("kidemKG", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Personelin kıdem günü (Kur. B.T.)"),

            new BordroGenelDegisken("kadroDerece", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Kadro derecesi"),
            new BordroGenelDegisken("odemeDerece", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Maaş hesaplamasında kullanılan derece"),
            new BordroGenelDegisken("odemeKademe", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Maaş hesaplamasında kullanılan kademe"),
            new BordroGenelDegisken("odemeGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Maaş hesaplamasında kullanılan gösterge"),
            new BordroGenelDegisken("odemeEkGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Maaş hesaplamasında kullanılan ek gösterge"),
            new BordroGenelDegisken("KHKDerece", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Kazanılmış Hak derece"),
            new BordroGenelDegisken("KHKKademe", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Kazanılmış Hak kademe"),
            new BordroGenelDegisken("KHKGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Kazanılmış Hak gösterge"),
            new BordroGenelDegisken("KHKEkGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Kazanılmış Hak ek gösterge"),

            new BordroGenelDegisken("emekliDerece", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Emekliliğe esas derece"),
            new BordroGenelDegisken("emekliKademe", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Emekliliğe esas kademe"),
            new BordroGenelDegisken("emekliGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Emekliliğe esas gösterge"),
            new BordroGenelDegisken("emekliEkGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Emekliliğe esas ek gösterge"),
            new BordroGenelDegisken("oncEmekliDerece", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Terfi öncesi emekliliğe esas derece"),
            new BordroGenelDegisken("oncEmekliKademe", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Terfi öncesi emekliliğe esas kademe"),
            new BordroGenelDegisken("oncEmekliEkGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Terfi öncesi emekliliğe esas ek gösterge"),
            new BordroGenelDegisken("oncEmekliGosterge", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Terfi öncesi emekliliğe esas gösterge"),
            new BordroGenelDegisken("kidem", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Personelin kıdemi"),
            new BordroGenelDegisken("oncKidem", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Personelin terfi öncesi kıdemi"),

            new BordroGenelDegisken("kidemK", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Personelin kıdemi (Kur. B.T.)"),
            new BordroGenelDegisken("oncKidemK", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Personelin terfi öncesi kıdemi (Kur. B.T.)"),

            new BordroGenelDegisken("dilGostergesi", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Dil Tazminatı"),
            new BordroGenelDegisken("sendikaBilgi", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Sendika Bilgi"),
            new BordroGenelDegisken("lojmanDaireTur", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Lojman Daire Tur"),
            new BordroGenelDegisken("lojmanM2", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Lojman M2"),
            new BordroGenelDegisken("yakitM2", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Yakıt M2"),

            new BordroGenelDegisken("oncGVMatrah", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "Önceki Gelir Vergisi Matrahı"),
            new BordroGenelDegisken("oncSGKMatrah", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "Önceki SGK Matrahı"),

            new BordroGenelDegisken("sgkMatrahDevir1", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "SGK Mtrh. Devir (1 ay önce)"),
            new BordroGenelDegisken("sgkMatrahDevir2", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "SGK Mtrh. Devir (2 ay önce)"),

            new BordroGenelDegisken("AGIDevreden", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "AGİ Devreden"),

            new BordroGenelDegisken("GVIstisnaDevir", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "GV İstisna Devreden"),
            new BordroGenelDegisken("DVIstisnaDevir", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "DV İstisna Devreden"),
            new BordroGenelDegisken("oncGVHesapMatrah", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "Önceki Hes. Gelir Vergisi Matrahı"),
            new BordroGenelDegisken("AsgariUcretDevir", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "Asgari Ücret Devreden"),

            new BordroGenelDegisken("GVHesIstisnaDevir", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "GV Hes. İstisna Devr."),
            new BordroGenelDegisken("DVHesIstisnaDevir", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "DV Hes. İstisna Devr."),

            new BordroGenelDegisken("personelTip", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Personel Tip Kodu"),
            new BordroGenelDegisken("birimTuru", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Kadro Birimi Türü"),
            new BordroGenelDegisken("calistigiBirimTuru", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Çalıştığı Birim Türü"),
            new BordroGenelDegisken("kurumaBaslamaTarihi", ENUMBordroDegiskenTur.Kisisel, VariableType.Date, "Kuruma Başlama Tarihi"),

            new BordroGenelDegisken("sayKod", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Saymanlık Kodu"),
            new BordroGenelDegisken("sayAd", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Sayanlık Adı"),
            new BordroGenelDegisken("birimKod", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Birim Kodu"),
            new BordroGenelDegisken("birimAd", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Birim Adı"),

            new BordroGenelDegisken("toplamGun", ENUMBordroDegiskenTur.Kisisel, VariableType.Integer, "Toplam Çalışma Günü"),
            new BordroGenelDegisken("unvanGunler", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Unvan Bazlı Çalışma Günü"),
            new BordroGenelDegisken("unvanGunlerIs", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Unvan Bazlı Çalışma İş Günü"),
            new BordroGenelDegisken("gorevGunler", ENUMBordroDegiskenTur.Kisisel, VariableType.String, "Görev Çalışma Günü"),

            new BordroGenelDegisken("Yemek", ENUMBordroDegiskenTur.Kisisel, VariableType.Money, "Yemekhane Kesintisi"),
        };

        public string degiskenAd { get; set; }
        public ENUMBordroDegiskenTur degiskenTur { get; set; }
        public VariableType veriTipi { get; set; }
        public string aciklama { get; set; }

        public string veriTipiStr
        {
            get
            {
                return VariableFunctions.GetTypeUserName(veriTipi);
            }
            set
            {
                veriTipi = VariableFunctions.GetType(value);
            }
        }

        public BordroGenelDegisken(string degiskenAd, ENUMBordroDegiskenTur degiskenTur, VariableType veriTipi, string aciklama)
        {
            this.degiskenAd = degiskenAd;
            this.degiskenTur = degiskenTur;
            this.veriTipi = veriTipi;
            this.aciklama = aciklama;
            yeni = 0;
        }

        public BordroGenelDegisken()
        {
            this.degiskenAd = "";
            this.degiskenTur = ENUMBordroDegiskenTur.KisiBordroBilgi;
            this.veriTipi = VariableType.Money;
            this.aciklama = "";
            yeni = 0;
        }

        public static BordroGenelDegisken GenelDegiskenBul(string degiskenAd)
        {
            for (int i = 0; i < bordroDegiskenler.Length; i++)
            {
                if (bordroDegiskenler[i].degiskenAd == degiskenAd)
                    return bordroDegiskenler[i];
            }
            return null;
        }

        public int yeni { get; set; }

        public string sira { get; set; }

        public int yontem { get; set; }

        public string liste { get; set; }
    }

    [Serializable()]
    public class BordroIslemTur
    {
        public enum ENUMBordroIslemTurYapi : int
        {
            Normal = 1,
            Fark = 2,
            Kist = 3,
            Ikramiye = 4,
            EksikOdemeFark = 5,
            KistFark = 6,
            FazlaMesai = 7,
            Gorevlendirme = 8
        }

        public int kod { get; set; }
        public string _ad { get; set; }
        public string ad { get { return kod + "-" + _ad; } }
        public string entegrasyonKod { get; set; }
        public ENUMBordroIslemTurYapi bordroIslemTurYapi;

        public BordroIslemTur()
        {
            this.kod = 0;
            this._ad = "";
            entegrasyonKod = "";
            bordroIslemTurYapi = ENUMBordroIslemTurYapi.Normal;
        }

        public BordroIslemTur(int kod, string ad)
        {
            this.kod = kod;
            this._ad = ad;
            entegrasyonKod = "";
            bordroIslemTurYapi = ENUMBordroIslemTurYapi.Normal;
        }

        public static string TurYapiAdBul(ENUMBordroIslemTurYapi turYapi)
        {
            switch (turYapi)
            {
                case ENUMBordroIslemTurYapi.Normal: return "Normal";
                case ENUMBordroIslemTurYapi.Fark: return "Fark";
                case ENUMBordroIslemTurYapi.Kist: return "Kıst";
                case ENUMBordroIslemTurYapi.Ikramiye:
                    if (TNS.DEG.Arac.VarsayilanKurumBul() == "31")
                        return "Ek Ödeme Oran Farkı";
                    else
                        return "İkramiye";
                case ENUMBordroIslemTurYapi.EksikOdemeFark: return "Eksik Ödeme";
                case ENUMBordroIslemTurYapi.KistFark: return "Kıst Fark";
                case ENUMBordroIslemTurYapi.FazlaMesai: return "Fazla Mesai";
                case ENUMBordroIslemTurYapi.Gorevlendirme: return "Görevlendirmeler";
                default: return "";
            }
        }
    }

    [Serializable()]
    public class BordroKistas
    {
        public TNSCollection tcListe = null;
        public TNSDateTime verilenTarih = null;
        public TNSDateTime verilenBitisTarih = null;
        public int yil = 0;
        public string surecNo = null;
        public TNSCollection tcler = null;
        public int odemeAy = 0;
        public int odemeYil = 0;
        public int donemAy = 0;
        public int donemYil = 0;
        public int bordroTur = 0;
        public int bordroIslem = 0;
        public string siralama;


        public string bordroYapiTurler = null;
        public string bordroIslemTurler = null;

        public BordroKistas()
        {
            this.yil = 0;
            this.surecNo = null;
        }

        public BordroKistas(int surecYil, string surecNo, TNSCollection tcler)
        {
            this.yil = surecYil;
            this.surecNo = surecNo;
            this.tcler = tcler;
        }
    }

    [Serializable()]
    public class BordroMuhasebeKayitlari
    {
        public Sonuc sonuc = new Sonuc();
        public TNSCollection OdemeEmri = null;
        public TNSCollection TahakkukKayitlari = null;

        public TNSCollection Hepsi
        {
            get
            {
                if (TahakkukKayitlari == null || TahakkukKayitlari.Count == 0)
                    return OdemeEmri;
                TNSCollection tut = new TNSCollection();
                tut.AddRange(OdemeEmri);
                tut.AddRange(TahakkukKayitlari);
                return tut;
            }

        }
    }


    [Serializable()]
    public class BordroYapi
    {
        public int bordroTur;
        public int bordroIslemTur;
        public DegiskenFormul[] degiskenler;
        public DegiskenFormul[] formuller;
        public DegiskenFormul[] hepsi;

        public string VTTabloAd { get { return "HRCBORDRO_DIN_" + bordroTur.ToString() + "_" + BordoIslemStr; } }
        public static string BordroTurTabloEkBul(int bordroTur)
        {
            return bordroTur.ToString();
        }

        public string BordoIslemStr
        {
            get
            {
                return ((int)bordroIslemTur).ToString();
            }
        }

        public string bordroAd { get; set; }
        public int kaynakBordroIslemTur { get; set; }
        public int kaynakBordroTur { get; set; }
        public string kaynakBordroKod
        {
            get
            {
                if (kaynakBordroTur <= 0 && kaynakBordroIslemTur <= 0) return "";
                return kaynakBordroTur + "-" + kaynakBordroIslemTur;
            }
            set
            {
                string par1 = "", par2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(value, "-", ref par1, ref par2);
                kaynakBordroTur = OrtakFonksiyonlar.ConvertToInt(par1, 0);
                kaynakBordroIslemTur = OrtakFonksiyonlar.ConvertToInt(par2, 0);
            }
        }

        public int YerBul(string kod, int tur)
        {
            for (int i = 0; i < this.hepsi.Length; i++)
            {
                if (this.hepsi[i].ad == kod)
                    return i;
            }
            return -1;
        }

        public class Sirala : IComparer<IntKodStrAd>
        {
            public int Compare(IntKodStrAd x, IntKodStrAd y)
            {
                return x.kod.CompareTo(y.kod);
            }
        }

        public int siralamaYontem = 0;
        public IntKodStrAd[] SiralamaBul()
        {
            System.Collections.Generic.List<IntKodStrAd> l = new System.Collections.Generic.List<IntKodStrAd>();
            if (!string.IsNullOrWhiteSpace(siralamaStr))
            {
                string[] siralamaStrDizi = siralamaStr.Split(',');
                foreach (var item in siralamaStrDizi)
                {
                    SiraEkle(l, item, null);
                }
                l.Sort(new Sirala());
            }
            else if (siralamaYontem == 1)
            {
                l.Add(new IntKodStrAd(1, "+0"));
            }
            else
            {
                for (int i = 0; i < this.hepsi.Length; i++)
                {
                    SiraEkle(l, this.hepsi[i].siraNo, this.hepsi[i].ad);
                }
                l.Sort(new Sirala());
            }
            return l.ToArray();
        }

        private void SiraEkle(List<IntKodStrAd> l, string siraNo, string alanAd)
        {
            string on = null, arka = null, sil = null;
            OrtakFonksiyonlar.StringiIkiyeBol(siraNo, " (", ref on, ref arka);
            if (alanAd == null) alanAd = on;
            if (!string.IsNullOrWhiteSpace(arka))
            {
                OrtakFonksiyonlar.StringiIkiyeBol(arka, ")", ref on, ref sil);
                if (on.EndsWith("+") || on.EndsWith("-"))
                {
                    string isaret = on.Substring(on.Length - 1, 1);
                    string siraStr = on.Substring(0, on.Length - 1);
                    l.Add(new IntKodStrAd(OrtakFonksiyonlar.ConvertToInt(siraStr, 0), isaret + alanAd));
                }
            }
        }

        public string siralamaStr = null;
        public int basla = 0;
        public int limit = 0;
        public int kolonSay = 0;
        public TNSCollection filter = null;

        public string SiralamaBulVT()
        {
            string donDeger = "";
            IntKodStrAd[] tut = SiralamaBul();
            foreach (IntKodStrAd item in tut)
            {
                char isaret = item.ad[0];
                string alanAd = item.ad.Substring(1);
                if (alanAd == "0") alanAd = this.VTTabloAd + ".TCKIMLIKNO" + ", " + this.VTTabloAd + ".SATIRNO";
                else
                {
                    alanAd = this.VTTabloAd + "." + alanAd;
                    if (isaret == '-') alanAd += " DESC";
                }
                if (donDeger != "") donDeger += ", ";
                donDeger += alanAd;
            }
            return donDeger;
        }

        public bool tekTekSil = false;

        public bool toplamAl = false;

        public void FormulAta(ArrayList alTum, ArrayList alDeg, ArrayList alFor)
        {
            hepsi = (DegiskenFormul[])alTum.ToArray(typeof(DegiskenFormul));
            degiskenler = (DegiskenFormul[])alDeg.ToArray(typeof(DegiskenFormul));
            formuller = (DegiskenFormul[])alFor.ToArray(typeof(DegiskenFormul));
        }

        public void HepsiOl()
        {
            ArrayList al = new ArrayList(degiskenler);
            al.AddRange(formuller);
            al.Sort();
            hepsi = (DegiskenFormul[])al.ToArray(typeof(DegiskenFormul));
        }
    }

    [Serializable()]
    public class DegiskenFormul : IComparable
    {
        public int CompareTo(object obj)
        {
            if (obj is DegiskenFormul)
            {
                return siraNo.CompareTo(((DegiskenFormul)obj).siraNo);
            }
            return -1;
        }

        public override bool Equals(object obj)
        {
            if (obj is DegiskenFormul)
            {
                return ad.Equals(((DegiskenFormul)obj).ad);
            }
            return false;
        }
        public ENUMBordroDegiskenTur giris { get; set; }

        public string girisStr
        {
            get
            {
                if (giris == ENUMBordroDegiskenTur.Elle)
                    return "VERİ GİRİŞ";
                if (giris == ENUMBordroDegiskenTur.Formul)
                    return "FORMÜL";
                return ad;
            }
        }

        public VariableType tip { get; set; }

        private string _ad;
        public string ad
        {
            get
            {
                return _ad;
            }

            set
            {
                _ad = value;
                BordroGenelDegisken bgd = BordroGenelDegisken.GenelDegiskenBul(_ad);
                if (bgd != null)
                {
                    giris = bgd.degiskenTur;
                    if (!(giris == ENUMBordroDegiskenTur.Elle || giris == ENUMBordroDegiskenTur.Formul))
                    {
                        tip = bgd.veriTipi;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(formula))
                        giris = ENUMBordroDegiskenTur.Elle;
                    else
                        giris = ENUMBordroDegiskenTur.Formul;
                }
            }
        }

        public string tipStr
        {
            get
            {
                return VariableFunctions.GetTypeUserName(tip);
            }
            set
            {
                tip = VariableFunctions.GetType(value);
            }
        }

        public string formula { get; set; }

        public string alacakHesap { get; set; }

        public string borcHesap { get; set; }

        public string kisaEtiket { get; set; }

        public string uzunEtiket { get; set; }

        public string siraNo { get; set; }

        public int genislik { get; set; }

        public bool VeriGirebilir
        {
            get
            {
                if (this.giris == ENUMBordroDegiskenTur.Elle) return true;
                return false;
            }
        }

        public int bordroTanim { get; set; }

        public int bordroIslem { get; set; }

        public int noktaSay { get; set; }

        public string ilkDeger { get; set; }

        public object DegerBul(string sdeger, double ddeger, object eskiDeger)
        {
            object deger = null;
            if (this.tip == VariableType.Integer)
                deger = OrtakFonksiyonlar.ConvertToInt(ddeger, 0) + (int)eskiDeger;
            else if (this.tip == VariableType.Money)
                deger = ddeger + (double)eskiDeger;
            else if (this.tip == VariableType.Date)
            {
                TNSDateTime dt = new TNSDateTime(sdeger);
                if (!TNSDateTime.IsNull(dt)) deger = dt.Oku();
            }
            else
                deger = sdeger;
            return deger;
        }

        public object Bos()
        {
            object deger = null;
            if (this.tip == VariableType.Integer)
                deger = 0;
            else if (this.tip == VariableType.Money)
                deger = (double)0.0;
            else if (this.tip == VariableType.Date)
                deger = null;
            else
                deger = "";
            return deger;
        }

    }

    /// <summary>
    /// DogrudanTeminPlan: Alıncak malzeme listesi
    /// </summary>
    [System.Serializable()]
    public class DogrudanTeminPlan
    {
        public DogrudanTeminPlan()
        {
            talepNo = -2;
        }
        /// <summary>
        /// Yıl: Yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Saymanlik: Saymanlık seçiniz
        /// </summary>
        public int saymanlik { get; set; }

        /// <summary>
        /// Birim: Birim seçiniz
        /// </summary>
        public string birim { get; set; }

        /// <summary>
        /// TAS Hesap Kod: Taşınır Hesap Kodu
        /// </summary>
        public string tasHesapKod { get; set; }

        /// <summary>
        /// okasKod: Okas Kodu
        /// </summary>
        public string okasKod { get; set; }

        public string okasAd { get; set; }

        public string tasHesapAd { get; set; }

        /// <summary>
        /// Miktar: Alınacak miktarı giriniz
        /// </summary>
        public double miktar { get; set; }

        /// <summary>
        /// Birim Fiyat: Birim Fiyatı
        /// </summary>
        public double birimFiyat { get; set; }

        /// <summary>
        /// Kanun Maddesi: Kanun Maddesini Seçiniz
        /// </summary>
        public string kanunMaddesi { get; set; }

        /// <summary>
        /// Harcama Türü: Harcama Türünü Seçiniz
        /// </summary>
        public string harcamaTuru { get; set; }

        /// <summary>
        /// Harcama Türü: Harcama Türünü Seçiniz
        /// </summary>
        public string birimAd { get; set; }

        public string muhasebeBirimAd { get; set; }
        public string muhasebeAd { get; set; }

        /// <summary>
        /// Arama kriteri olarak kullanılıyor. Verilen ildeki teklifleri getirir.
        /// </summary>
        public int il { get; set; }

        public string senaryoKod { get; set; }

        public int talepNo { get; set; }

        /// <summary>
        /// Birim Fiyat: Birim Fiyatı
        /// </summary>
        public double dtHaric { get; set; }

        /// <summary>
        /// Birim Fiyat: Birim Fiyatı
        /// </summary>
        public double onceAlim { get; set; }
    }

    /// <summary>
    /// YuzdeOnKapat: 
    /// </summary>
    [System.Serializable()]
    public class YuzdeOnKapat
    {
        public YuzdeOnKapat()
        {
            this.yili = 0;
            this.malHizmetYapim = string.Empty;
        }

        public YuzdeOnKapat(int yil, string malHizmetYapim)
        {
            this.yili = yil;
            this.malHizmetYapim = malHizmetYapim;
        }

        /// <summary>
        /// Yılı: Yılı giriniz.
        /// </summary>
        public int yili { get; set; }

        /// <summary>
        /// MalHizmetYapım: Mal/Hizmet/Yapım seçiniz.
        /// </summary>
        public string malHizmetYapim { get; set; }

        /// <summary>
        /// Tarih: İşlemin yapılma gerekçesi
        /// </summary>
        public TNSDateTime tarih { get; set; }
        public DateTime tarihDT
        {
            get
            {
                if (tarih == null)
                    return DateTime.MinValue;
                else
                    return tarih.Oku();
            }
            set
            {
                if (tarih == null) tarih = new TNSDateTime(value);
                else tarih.Yaz(value);
            }
        }

        /// <summary>
        /// Açıklama: Kapatılma/Açılma gerekçesi
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// Kapalı: 1 ise kapalı değilse açık
        /// </summary>
        public int kapali { get; set; }

        public int talepNo { get; set; }
    }


    public enum ENUMRevizeFormBelgeTur : int
    {
        //IcmalRevize = 1, 
        DetayRevize = 2,
        //DetayTalep = 3
    }

    public enum ENUMRevizeFormDurum : int
    {
        Bos = 0, Yeni = 1, OMK = 4, Onayli = 5, Iptal = 9,

        YeniTalep = 91, KarsilananTalep = 95, IptalTalep = 99
    }

    public enum ENUMSeyyarGorevRevizeAltIslemTur : int
    {
        Bos = 0, IlRevize = 1, IlEkleme = 2, DetayRevize = 3, DetayIade = 4, DetayEkleme = 5, DetayDegisim = 6
    }

    /// <summary>
    /// SeyyarGorevRevize: 
    /// </summary>
    [System.Serializable()]
    public class SeyyarGorevRevize
    {
        public Sonuc sonuc = new Sonuc(true, "", "");

        public TNSCollection birimDegisimler = new TNSCollection();
        public int icmalNo { get; set; }

        public int memurToplam { get; set; }
        public int isciToplam { get; set; }
        public int sozlesmeliToplam { get; set; }

        /// <summary>
        /// Yıl: Amortisman İşleminin Yapılacağı Yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Belge Tür: Tavan veya Detay Revize
        /// </summary>
        public ENUMRevizeFormBelgeTur belgeTur { get; set; }

        /// <summary>
        /// Belge No: Belge No
        /// </summary>
        public int belgeNo { get; set; }

        /// <summary>
        /// Açıklama: Yapılan işlemin açıklaması
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// Tarih: Yapılan işlemin tarihi
        /// </summary>
        public TNSDateTime tarih { get; set; }
        public DateTime tarihDT
        {
            get
            {
                if (tarih == null)
                    return DateTime.MinValue;
                else
                    return tarih.Oku();
            }
            set
            {
                if (tarih == null) tarih = new TNSDateTime(value);
                else tarih.Yaz(value);
            }
        }

        /// <summary>
        /// İşlem Tür: Yapılan işlemin alt işlemi
        /// </summary>
        public ENUMSeyyarGorevRevizeAltIslemTur altIslemTur { get; set; }

        /// <summary>
        /// Durum: Formun onaylama/iptal durumu
        /// </summary>
        public ENUMRevizeFormDurum durum { get; set; }

        public string BelgeNoStr
        {
            get { return belgeNo.ToString("000000"); }
            set { belgeNo = OrtakFonksiyonlar.ConvertToInt(value, 0); }
        }

        public string onayaSunanAdSoyad { get; set; }

        public string onayaSunanUnvan { get; set; }

        public string onaylayanAdSoyad { get; set; }

        public string onaylayanUnvan { get; set; }

        public int belgeNoBas { get; set; }

        public int belgeNoBit { get; set; }

        public TNSDateTime tarihBas { get; set; }

        public TNSDateTime tarihBit { get; set; }

        public string plakaKod { get; set; }

        public string DurumAd { get { return DurumAdDondur(durum); } }

        public static string DurumAdDondur(ENUMRevizeFormDurum durum)
        {
            if (durum == ENUMRevizeFormDurum.Bos) return "Hepsi";
            if (durum == ENUMRevizeFormDurum.YeniTalep) return "Yeni Talep";
            if (durum == ENUMRevizeFormDurum.KarsilananTalep) return "Karşılanan Talep";
            if (durum == ENUMRevizeFormDurum.IptalTalep) return "İptal Edilen Talep";
            if (durum == ENUMRevizeFormDurum.Yeni) return "Yeni İşlem";
            if (durum == ENUMRevizeFormDurum.Onayli) return "Onaylı  İşlem";
            if (durum == ENUMRevizeFormDurum.OMK) return "Ön Mali Kontrolde";
            if (durum == ENUMRevizeFormDurum.Iptal) return "İptal İşlem";
            return "";
        }

        public void SatirlariEkle(SeyyarGorevRevize gelenForm)
        {
            foreach (BKD.TavanGenel item in gelenForm.birimDegisimler)
            {
                BKD.TavanGenel ilgiliSatir = BulYadaEkle(item.kod1);
                ilgiliSatir.yil0 += item.yil0;
                ilgiliSatir.yil1 += item.yil1;
                ilgiliSatir.yil2 += item.yil2;
            }
        }

        private BKD.TavanGenel BulYadaEkle(string kod1)
        {
            foreach (BKD.TavanGenel item in this.birimDegisimler)
            {
                if (item.kod1 == kod1)
                    return item;
            }
            BKD.TavanGenel tg = new BKD.TavanGenel();
            tg.kod1 = kod1;
            this.birimDegisimler.Add(tg);
            return tg;
        }

        public void YedekAyarla()
        {
            double yil0 = 0;
            double yil1 = 0;
            double yil2 = 0;
            foreach (BKD.TavanGenel item in this.birimDegisimler)
            {
                if (item.kod1 != "9.00.000.00000")
                {
                    yil0 += item.yil0;
                    yil1 += item.yil1;
                    yil2 += item.yil2;
                }
            }

            BKD.TavanGenel yedekSatir = BulYadaEkle("9.00.000.00000");

            yedekSatir.yil0 = (0 - yil0);
            yedekSatir.yil1 = (0 - yil1);
            yedekSatir.yil2 = (0 - yil2);
        }

        //public int talepDurum { get; set; }

        public string kaydedenKullaniciKod { get; set; }

        public string onaylayanKullaniciKod { get; set; }

        public TNSCollection birimler { get; set; }

        //public int talepDurumSrg { get; set; }

    }

    [Serializable()]
    public class BordroSablonDosya
    {
        public int bordroTur { get; set; }
        public int islemTur { get; set; }
        public int sablonKod { get; set; }
        public string sablonAd { get; set; }
        public string siraNo { get; set; }

        public bool tekKisilik { get; set; }
        public string gecerliKodlar { get; set; }
        public bool sayfa { get; set; }
        public bool ortakRapor { get; set; }
        public BordroSablonDosya()
        {
            this.bordroTur = 0;
            this.islemTur = 1;
            this.sablonKod = 0;
            this.sablonAd = "";

            this.siraNo = "";
            this.tekKisilik = false;
            this.gecerliKodlar = null;
            this.sayfa = true;
            this.ortakRapor = false;

        }

        public BordroSablonDosya(int bordroTur, int islemTur, int sablonKod, string sablonAd,
                                            string siraNo, bool tekKisilik, string gecerliKodlar, bool sayfa, bool ortakRapor)
        {
            this.bordroTur = bordroTur;
            this.islemTur = islemTur;
            this.sablonKod = sablonKod;
            this.sablonAd = sablonAd;

            this.siraNo = siraNo;
            this.tekKisilik = tekKisilik;
            this.gecerliKodlar = gecerliKodlar;
            this.sayfa = sayfa;
            this.ortakRapor = ortakRapor;
        }

        public string SablonDosyaAd
        {
            get
            {
                return string.Format("Sablon_{0}_{1}_{2}.xlsx", bordroTur, (int)islemTur, sablonKod);
            }
        }


        public bool UyumluMu(int bordroTur, int islemTur, ENUMUCDURUM sayfa, ENUMUCDURUM ortakRapor)
        {
            bool uyumlu = KodVarMi(bordroTur, islemTur);
            if (!uyumlu) return false;
            if (sayfa != ENUMUCDURUM.HEPSI && (this.sayfa != (sayfa == ENUMUCDURUM.DOGRU))) return false;
            if (ortakRapor != ENUMUCDURUM.HEPSI && (this.ortakRapor != (ortakRapor == ENUMUCDURUM.DOGRU))) return false;
            return true;
        }

        private bool KodVarMi(int bordroTur, int islemTur)
        {
            if (bordroTur > 0 && !string.IsNullOrWhiteSpace(gecerliKodlar))
            {
                string[] tipVeIslemler = gecerliKodlar.Split(';');
                foreach (string item in tipVeIslemler)
                {
                    string[] tipVeIslem = item.Split('-');
                    string tipStr = OrtakFonksiyonlar.DiziElemanAl(tipVeIslem, 0, "");

                    if (string.IsNullOrWhiteSpace(tipStr) || tipStr == "*" || OrtakFonksiyonlar.ConvertToInt(tipStr, -1) == bordroTur)
                    {
                        string islemStrler = OrtakFonksiyonlar.DiziElemanAl(tipVeIslem, 1, "");
                        if (string.IsNullOrWhiteSpace(islemStrler) || islemStrler == "*")
                        {
                            return true;
                        }
                        string[] islemler = islemStrler.Split(',');
                        foreach (string itemIslem in islemler)
                        {
                            if (OrtakFonksiyonlar.ConvertToInt(itemIslem, -1) == islemTur) return true;
                        }
                    }
                }
                return false;
            }
            else
                return true;
        }
    }

    [Serializable()]
    public class BordroMuhasebe : IComparable
    {
        public static BordroMuhasebe HesapBul(TNSCollection coll, string hesapKod)
        {
            if (string.IsNullOrWhiteSpace(hesapKod)) return null;
            hesapKod = hesapKod.Replace(".", "");
            foreach (BordroMuhasebe item in coll)
            {
                if (item.hesapKod.Replace(".", "").Equals(hesapKod)) return item;
            }
            return null;
        }

        public override bool Equals(object o)
        {
            string icHesapKod = null;
            BordroMuhasebe bmGelen = null;
            if (o is BordroMuhasebe)
            {
                bmGelen = (BordroMuhasebe)o;
                icHesapKod = bmGelen.hesapKod;
            }
            else return false;

            if (!this.hesapKod.Equals(bmGelen.hesapKod)) return false;
            if (!this.altHesapKod.Equals(bmGelen.altHesapKod)) return false;
            if (!this.birimKod.Equals(bmGelen.birimKod)) return false;
            if (TNS.MUH.Arac.NitelikVarmi(hesapSatir.nitelik, (int)MUH.ENUMHesapKodNitelik.OZELGIDER))//if (this.hesapSatir.nitelik Kod.Replace(".", "") == "360010102")
            {
                BorcAlacak tplm = this.toplam;
                if (tplm.borc == 0) return true;
                return Math.Sign(tplm.borc) == Math.Sign(bmGelen.toplam.borc);
            }
            return true;
        }

        public int CompareTo(object obj)
        {
            BordroMuhasebe objGelen = null;
            if (obj is BordroMuhasebe) objGelen = (BordroMuhasebe)obj;
            if (objGelen == null) return -1;

            int comp = this.HesapGrup().CompareTo(objGelen.HesapGrup());
            if (comp != 0) return comp;

            comp = this.BorcAlacakKod().CompareTo(objGelen.BorcAlacakKod());
            if (comp != 0) return comp;

            comp = Math.Abs(objGelen.toplam.borc).CompareTo(Math.Abs(this.toplam.borc));
            if (comp != 0) return comp;

            return this.hesapKod.Replace(".", "").CompareTo(objGelen.hesapKod.Replace(".", ""));
        }

        private int HesapGrup()
        {
            string ilk = this.hesapKod.Substring(0, 1);
            string iki = this.hesapKod.Substring(0, 2);
            if (ilk == "6") return 1;
            if (iki == "36") return 2;
            if (ilk == "3") return 3;
            if (iki == "10") return 7;
            if (ilk == "8") return 8;
            if (ilk == "9") return 9;
            return 5;
        }

        private int BorcAlacakKod() //0 Borç, 1 Alacak;
        {
            if (this.toplam.borc >= 0) return 0;
            else return 1;
        }

        public string hesapKodAlthesapkod
        {
            get
            {
                string on = "";
                if (!string.IsNullOrWhiteSpace(birimKod)) on = "[" + birimKod + "]-";
                if (string.IsNullOrWhiteSpace(altHesapKod)) return on + hesapKod;
                return on + hesapKod + "-" + altHesapKod;
            }
        }

        public string hesapKod { get { return hesapSatir.hesapKod; } set { hesapSatir.hesapKod = value; } }
        public string hesapAd { get { return hesapSatir.aciklama; } }

        [Serializable()]
        public class BorcAlacak
        {
            public double borc = 0;
            public double borcDoviz = 0;
        }
        public BorcAlacak[] dizi;

        public double dizi0 { get { return dizi[0].borc; } }
        public double dizi1 { get { return dizi[1].borc; } }
        public double dizi2 { get { return dizi[2].borc; } }
        public double dizi3 { get { return dizi[3].borc; } }
        public double dizi4 { get { return dizi[4].borc; } }
        public double dizi5 { get { return dizi[5].borc; } }
        public double dizi6 { get { return dizi[6].borc; } }
        public double dizi7 { get { return dizi[7].borc; } }
        public double dizi8 { get { return dizi[8].borc; } }
        public double dizi9 { get { return dizi[9].borc; } }

        public double dizi10 { get { return dizi[10].borc; } }
        public double dizi11 { get { return dizi[11].borc; } }
        public double dizi12 { get { return dizi[12].borc; } }
        public double dizi13 { get { return dizi[13].borc; } }
        public double dizi14 { get { return dizi[14].borc; } }
        public double dizi15 { get { return dizi[15].borc; } }
        public double dizi16 { get { return dizi[16].borc; } }
        public double dizi17 { get { return dizi[17].borc; } }
        public double dizi18 { get { return dizi[18].borc; } }
        public double dizi19 { get { return dizi[19].borc; } }

        public double dizi20 { get { return dizi[20].borc; } }
        public double dizi21 { get { return dizi[21].borc; } }
        public double dizi22 { get { return dizi[22].borc; } }
        public double dizi23 { get { return dizi[23].borc; } }
        public double dizi24 { get { return dizi[24].borc; } }
        public double dizi25 { get { return dizi[25].borc; } }
        public double dizi26 { get { return dizi[26].borc; } }
        public double dizi27 { get { return dizi[27].borc; } }
        public double dizi28 { get { return dizi[28].borc; } }
        public double dizi29 { get { return dizi[29].borc; } }

        public BorcAlacak toplam
        {
            get
            {
                return ToplamBul(0, dizi.Length);
            }
        }

        public BorcAlacak ToplamBul(int bas, int bit)
        {
            BorcAlacak toplamIc = new BorcAlacak();
            for (int i = bas; i < bit; i++)
            {
                toplamIc.borc += dizi[i].borc;
                toplamIc.borcDoviz += dizi[i].borcDoviz;
            }
            return toplamIc;
        }

        public double toplamAP
        {
            get
            {
                return toplam.borc;
            }
        }

        public BordroMuhasebe()
        {
            dizi = new BorcAlacak[30];
            for (int i = 0; i < dizi.Length; i++)
            {
                dizi[i] = new BorcAlacak();
            }
            altHesapKod = "";
        }

        public MUH.HesapPlaniSatir hesapSatir = null;

        public string altHesapKod { get; set; }
        public bool ortakAltHesap = false;
        public bool AltHesapVarMiOrtak()
        {
            if (hesapSatir == null) return false;
            if ((hesapSatir.altHesapKod == 1))
                return true;
            return false;
        }

        public bool AltHesapVarMi()
        {
            if (hesapSatir == null) return false;
            if ((hesapSatir.altHesapKod == 1) && !ortakAltHesap)
                return true;
            return false;
        }

        public bool MahsupluMu(bool borclu)
        {
            if (hesapSatir == null) return false;
            if (TNS.MUH.Arac.NitelikVarmi(hesapSatir.nitelik, (int)TNS.MUH.ENUMHesapKodNitelik.MAHSUP))
            {
                if (hesapSatir.bakiye == 1 && !borclu) return true;
                if (hesapSatir.bakiye == 2 && borclu) return true;
            }
            return false;
        }

        public string birimKod { get; set; }

        //private void AtaEkle(int bordroTurYer, double deger, MerkezBankasiKurSinif gunlukKur, bool atamaMi)
        //{
        //}

        private void AtaEkle(int bordroTurYer, double deger, double degerDoviz, bool atamaMi)
        {
            if (atamaMi)
            {
                dizi[bordroTurYer].borc = deger;
                dizi[bordroTurYer].borcDoviz = degerDoviz;
            }
            else
            {
                dizi[bordroTurYer].borc += deger;
                dizi[bordroTurYer].borcDoviz += degerDoviz;
            }
        }

        public void DegerAta(int bordroTurYer, double deger, double degerDoviz)
        {
            AtaEkle(bordroTurYer, deger, degerDoviz, true);
        }

        public void DegerEkle(int bordroTurYer, double deger, double degerDoviz)
        {
            AtaEkle(bordroTurYer, deger, degerDoviz, false);
        }
    }

    [Serializable()]
    public class ButceBildirim
    {
        public int orgKod { get; set; }
        public int yil { get; set; }
        public int ay { get; set; }
        public int islemTur { get; set; }
        public double tutar { get; set; }
        public TNSDateTime tarih { get; set; }
    }

    [Serializable()]
    public class ManasSatinAlmaIsTanim
    {
        public int kod = 0;
        public TNSDateTime tarih = new TNSDateTime();
        public string ad = "";
        public string nitelik = "";
        public int ihaleUsulu = 0;
        public string ihaleMaddesi = "";
        public int alimTuru = 0;
        public ParaBirimi paraBirimi = new ParaBirimi();
        public int alimYontemi = 0;
        public TNSDateTime onayTarihi = new TNSDateTime();
    }

    [Serializable]
    public class ManasSatinAlmaGorevli
    {
        public int satinAlmaKod { get; set; }
        public int siraNo { get; set; }
        public string yetkili { get; set; }
        public string unvan { get; set; }

        public ManasSatinAlmaGorevli()
        {
            satinAlmaKod = 0;
            siraNo = 0;
            yetkili = "";
            unvan = "";
        }
    }

    [Serializable]
    public class ManasSatinAlmaUrun
    {
        public int satinAlmaKod = 0;
        public int siraNo = 0;
        public string talepRefId = "";
        public MuhasebeBirim muhasebeBirim = new MuhasebeBirim();
        public string talepBelgeNo = "";
        public TNSDateTime talepTarihi = new TNSDateTime();
        public string hesapPlanKodu = "";
        public string hesapPlanAdi = "";
        public string yansitmaHesapKod = "";
        public string aciklama = "";
        public decimal miktar = 0;
        public string olcuBirimi = "";
    }

    [Serializable]
    public class ManasSatinAlmaFirma
    {
        public int satinAlmaKod { get; set; }
        public int siraNo { get; set; }
        public string vergiNo { get; set; }
        public string unvan { get; set; }
        public string refId { get; set; }
        public int tur { get; set; }

        public ManasSatinAlmaFirma()
        {
            satinAlmaKod = 0;
            siraNo = 0;
            vergiNo = "";
            unvan = "";
            refId = "";
            tur = 0;
        }
    }

    [Serializable]
    public class ManasSatinAlmaTeklif
    {
        public int satinAlmaKod = 0;
        public string refId = "";
        public string urunRefId = "";
        public string firmaRefId = "";
        public double tutar = 0.0;
        public int tur = 0;
        public ManasSatinAlmaFirma firma = new ManasSatinAlmaFirma();
        public ManasSatinAlmaUrun urun = new ManasSatinAlmaUrun();
        public double[] raporTutar = new double[20];
    }

    [Serializable]
    public class ManasSatinAlmaTeklifForm
    {
        public int satinAlmaKod = 0;
        public TNSDateTime tarih = new TNSDateTime();
        public int yontem = 0;
        public double tutar = 0.0;
        public ManasSatinAlmaTeklif[] teklifler;
        public int tur = 0;
    }

    [Serializable]
    public class KomisyonUyeSablon
    {
        public string kod = "";
        public string sahibi = "";
        public string adi = "";
        public int durum = 0;
        public ObjectArray uyeler = new ObjectArray();
    }
    [Serializable]
    public class KomisyonUyeSablonDetay
    {
        public string kod = "";
        public string sablonKod = "";
        public int siraNo = 0;
        public int komisyonGorev = 0;
        public string adi = "";
        public string gorev = "";
        public string unvan = "";
        public string sicilNo = "";
        public string telefon = "";
        public string cepTelefon = "";
    }

    [Serializable]
    public class Gundelik
    {
        public int kod = 0;
        public int yil = 0;
        public int unvanKod = 0;
        public string unvanAd = "";
        public string ulkeKod = "";
        public string ulkeAd = "";
        public double gundelikTutar = 0.0;
        public ParaBirimi paraBirimi = new ParaBirimi();
        public ENUMGundelikTur tur = 0;
    }

    [Serializable]
    public class BordroRaporSablon
    {
        public int kod = 0;
        public string kullaniciKod = "";
        public string ad = "";
        public string aciklama = "";

        public int sablonKod { get { return kod; } }
        public string sablonAd { get { return ad; } }
    }

}
