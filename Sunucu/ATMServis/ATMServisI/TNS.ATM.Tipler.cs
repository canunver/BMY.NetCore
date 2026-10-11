using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.ATM
{
    /// <summary>
    /// Para Birimi Enumaratörü
    /// </summary>

    public enum ENUMDosyaIslemTur : int
    {
        Listeleme = 0,
        Kaydet = 1,
        OzelGuncelle = 2,
        Onayla = 5,
        IptalEt = 9,
        YeniYap = 6
    }

    public enum ENUMParaBirimi : int
    {
        /// <summary>
        /// Türk Lirası
        /// </summary>
        TL = 1,
        /// <summary>
        /// Dolar
        /// </summary>
        DOLAR = 2,
        /// <summary>
        /// Euro
        /// </summary>
        EURO = 3,
        /// <summary>
        /// Sterlin
        /// </summary>
        STERLIN = 4
    }

    /// <summary>
    /// Alacak Takip Durum Enumaratörü
    /// </summary>
    public enum ENUMDurum : int
    {
        /// <summary>
        /// Yeni
        /// </summary>
        Yeni = 1,
        /// <summary>
        /// Değişti
        /// </summary>
        Degisti = 2,
        /// <summary>
        /// İade edildi
        /// </summary>
        IadeEdildi = 3,
        /// <summary>
        /// Takip biriminde
        /// </summary>
        TakipBiriminde = 5,
        /// <summary>
        /// Tebliğ edildi
        /// </summary>
        TebligEdildi = 6,
        /// <summary>
        /// İlan edildi
        /// </summary>
        IlanEdildi = 7,
        /// <summary>
        /// İtiraz edildi
        /// </summary>
        ItirazEdildi = 8,
        /// <summary>
        /// Mahkemelik
        /// </summary>
        Mahkemelik = 9,
        /// <summary>
        /// Taksitlendirildi
        /// </summary>
        Taksitlendirildi = 10,
        /// <summary>
        /// İcra yoluyla tahsilat
        /// </summary>
        IcraYoluylaTahsilat = 14,
        /// <summary>
        /// Hükmen tahsilat
        /// </summary>
        HukmenTahsilat = 15,
        /// <summary>
        /// Ödeme yapıldı
        /// </summary>
        OdemeYapildi = 16,
        /// <summary>
        /// Terkin
        /// </summary>
        Terkin = 17,
        /// <summary>
        /// Af
        /// </summary>
        Af = 18,
        /// <summary>
        /// İptal
        /// </summary>
        Iptal = 20
    }

    /// <summary>
    /// Durum Bilgileri Sınıfı 
    /// </summary>
    [Serializable()]
    public class Durum
    {
        /// <summary>
        /// Durum kodu
        /// </summary>
        public int kod;
        /// <summary>
        /// Durum adı
        /// </summary>
        public string ad;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public Durum()
        {
            kod = 0;
            ad = "";
        }

        /// Sınıfın kod ve ad parametresi alan oluşturucu(Constructer) fonksiyonu
        public Durum(int kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
        }
    }

    /// <summary>
    /// Faiz Türü bilgileri sınıfı.
    /// </summary>
    [Serializable()]
    public class FaizTur
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Ad
        /// </summary>
        public string ad;
        /// <summary>
        /// Oran
        /// </summary>
        public double oran;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public FaizTur()
        {
            yil = 0;
            kod = 0;
            ad = "";
            oran = 0;
        }

        /// Sınıfın yıl, kod, ad ve oran parametreleri alan oluşturucu(Constructer) fonksiyonu
        public FaizTur(int yil, int kod, string ad, double oran)
        {
            this.yil = yil;
            this.kod = kod;
            this.ad = ad;
            this.oran = oran;
        }
    }

    /// <summary>
    /// Alacak Takip dosyası bilgileri sınıfı.
    /// </summary>
    [Serializable()]
    public class Dosya
    {
        public string kurKod;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Dosya numarası
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// Saymanlık kodu
        /// </summary>
        public int saymanlikKodu;
        /// <summary>
        /// Saymanlık adı
        /// </summary>
        public string saymanlikAdi;
        /// <summary>
        /// Birim kodu
        /// </summary>
        public string birimKodu;
        /// <summary>
        /// Birim adı
        /// </summary>
        public string birimAdi;
        /// <summary>
        /// Kimlik numarası
        /// </summary>
        public string kimlikNo;
        /// <summary>
        /// Borçlu tipi
        /// </summary>
        public int borcluTipi;
        /// <summary>
        /// Borç tespit tarihi
        /// </summary>
        public TNSDateTime borcTespitTarihi;
        /// <summary>
        /// Borç doğuş tarihi
        /// </summary>
        public TNSDateTime borcDogusTarihi;
        /// <summary>
        /// Borçlu adı
        /// </summary>
        public string borcluAd;
        /// <summary>
        /// Borçlu adresi
        /// </summary>
        public string borcluAdres;
        /// <summary>
        /// Borç
        /// </summary>
        public double borc;

        /// <summary>
        /// Para birimi
        /// </summary>
        public int paraBirimi;
        /// <summary>
        /// Borç sebebi
        /// </summary>
        public string borcSebep;
        /// <summary>
        /// Faiz başlangıç tarihi
        /// </summary>
        public TNSDateTime faizBaslangicTarihi;
        public TNSDateTime taksitBaslangicTarihi { get; set; }

        /// <summary>
        /// Faiz türü
        /// </summary>
        public FaizTur faizTur;
        /// <summary>
        /// Zaman aşımı başlangıç tarihi
        /// </summary>
        public int zamanAsimiBaslangicYili;
        /// <summary>
        /// Tebliğ tarihi
        /// </summary>
        public TNSDateTime tebligTarihi;
        /// <summary>
        /// Son durum
        /// </summary>
        public Durum sonDurum;
        /// <summary>
        /// Son durum tarihi
        /// </summary>
        public TNSDateTime sonDurumTarihi;
        /// <summary>
        /// Borç değerlendirme tarihi
        /// </summary>
        public TNSDateTime borcDegerlendirmeTarihi;
        /// <summary>
        /// Saymanlık dosya numarası
        /// </summary>
        public string saymanlikDosyaNo;
        /// <summary>
        /// The odeme emri no
        /// </summary>
        public string odemeEmriNo;
        public double muhasebeBorcDurumu;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public Dosya()
        {
            yil = 0;
            dosyaNo = 0;
            saymanlikKodu = 0;
            saymanlikAdi = "";
            birimKodu = "";
            birimAdi = "";
            kimlikNo = "";
            borcluTipi = 0;
            borcTespitTarihi = new TNSDateTime();
            borcDogusTarihi = new TNSDateTime();
            borcluAd = "";
            borcluAdres = "";
            borc = 0;
            paraBirimi = 0;
            borcSebep = "";
            faizBaslangicTarihi = new TNSDateTime();
            faizTur = new FaizTur();
            zamanAsimiBaslangicYili = 0;
            tebligTarihi = new TNSDateTime();
            sonDurum = new Durum();
            sonDurumTarihi = new TNSDateTime();
            borcDegerlendirmeTarihi = new TNSDateTime();
            saymanlikDosyaNo = "";
            odemeEmriNo = "";
            muhasebeBorcDurumu = 0.0;

            gelenYaziTarih = new TNSDateTime();
            saymanligaGidenYaziTarih = new TNSDateTime();
            kisiyiGidenYaziTarih = new TNSDateTime();
        }

        public ObjectArray taksitBilgi = null;
        public double odenenBorc
        {
            get
            {
                double donusDeger = 0;
                if (taksitBilgi != null)
                {
                    foreach (DosyaTaksit taksit in taksitBilgi.objeler)
                    {
                        if (taksit.odendi) donusDeger += taksit.miktar;
                    }
                }
                return donusDeger;
            }
        }

        public double odenenFaiz
        {
            get
            {
                double donusDeger = 0;
                if (taksitBilgi != null)
                {
                    foreach (DosyaTaksit taksit in taksitBilgi.objeler)
                    {
                        if (taksit.odendi) donusDeger += taksit.FaizToplam;
                    }
                }
                return donusDeger;
            }
        }

        public int toplamTaksit
        {
            get
            {
                if (taksitBilgi != null) return taksitBilgi.ObjeSayisi;
                else return 0;
            }
        }

        public int odenenTaksit
        {
            get
            {
                int donusDeger = 0;
                if (taksitBilgi != null)
                {
                    foreach (DosyaTaksit taksit in taksitBilgi.objeler)
                    {
                        if (taksit.odendi) donusDeger++;
                    }
                }
                return donusDeger;
            }
        }

        public string makbuzBilgi
        {
            get
            {
                string donusDeger = "";
                if (taksitBilgi != null)
                {
                    foreach (DosyaTaksit taksit in taksitBilgi.objeler)
                    {
                        if (taksit.odendi)
                        {
                            if (donusDeger != "") donusDeger += ", ";
                            donusDeger += taksit.odemeTarihi.ToString() + " - " + taksit.makbuzNo;
                        }
                    }
                }
                return donusDeger;
            }
        }

        public string gelenYaziYer { get; set; }
        public TNSDateTime gelenYaziTarih { get; set; }
        public string gelenYaziSayi { get; set; }
        public TNSDateTime saymanligaGidenYaziTarih { get; set; }
        public string saymanligaGidenYaziSayi { get; set; }
        public TNSDateTime kisiyiGidenYaziTarih { get; set; }
        public string kisiyeGidenYaziSayi { get; set; }

        public void taksitEkle(DosyaTaksit gelen)
        {
            if (taksitBilgi == null) taksitBilgi = new ObjectArray();
            if (gelen.siraNo >= 0)
            {
                taksitBilgi.Ekle(gelen);
            }
        }

        public bool muhasebeBorcDurumuAl = false;

        public string islemYapan { get; set; }
        public double Odenen
        {
            get
            {
                double odenen = 0;
                foreach (DosyaTaksit taksit in taksitBilgi.objeler)
                {
                    odenen = odenen + taksit.miktar;
                }
                return odenen;
            }
        }

        public bool TaksitOdendiMi()
        {
            foreach (DosyaTaksit taksit in taksitBilgi.objeler)
            {
                if (!TNSDateTime.IsNull(taksit.odemeTarihi))
                    return true;
            }
            return false;
        }

        public int OdemeSiraBul()
        {
            int maks = 0;
            foreach (DosyaTaksit taksit in taksitBilgi.objeler)
            {
                maks = Math.Max(maks, taksit.siraNo);
            }
            return maks + 1;
        }

        public DosyaTaksit TaksitBul(int siraNo)
        {
            foreach (DosyaTaksit taksit in taksitBilgi.objeler)
            {
                if (taksit.siraNo == siraNo)
                    return taksit;
            }
            return null;
        }
    }

    [System.Serializable()]
    public class MiktarIslem
    {
        public MiktarIslem(double kur, double miktarAB, double miktarUlusal, double faizAB, double faizUlusal, double ertelemeAB, double ertelemeUlusal)
        {
            this.kur = kur;
            this.miktarAB = miktarAB;
            this.miktarUlusal = miktarUlusal;
            this.faizAB = faizAB;
            this.faizUlusal = faizUlusal;
            this.ertelemeAB = ertelemeAB;
            this.ertelemeUlusal = ertelemeUlusal;
            if (this.kur <= 0) this.kur = 1;
        }

        public double kur { get; set; }

        public double miktarAB { get; set; }

        public double silinmeyecekMiktar { get; set; }

        public double miktarABKur(bool tl)
        {
            if (tl) return miktarAB;
            else return miktarABEUR;
        }

        public double miktarToplamKur(bool tl)
        {
            if (tl) return miktarAB + miktarUlusal;
            else return miktarABEUR + miktarUlusalEUR;
        }

        public double faizToplamKur(bool tl)
        {
            if (tl) return faizAB + faizUlusal;
            else return faizABEUR + faizUlusalEUR;
        }

        public double miktarUlusal { get; set; }
        public double miktarUlusalKur(bool tl)
        {
            if (tl) return miktarUlusal;
            else return miktarUlusalEUR;
        }

        public double faizAB { get; set; }
        public double faizABKur(bool tl)
        {
            if (tl) return faizAB;
            else return faizABEUR;
        }

        public double faizUlusal { get; set; }
        public double ertelemeAB { get; set; }
        public double ertelemeUlusal { get; set; }

        public double faizUlusalKur(bool tl)
        {
            if (tl) return faizUlusal;
            else return faizUlusalEUR;
        }

        public double miktarToplam { get { return OrtakFonksiyonlar.KurusYap(miktarAB + miktarUlusal); } }
        public double faizToplam { get { return OrtakFonksiyonlar.KurusYap(faizAB + faizUlusal); } }

        public double toplamAB { get { return OrtakFonksiyonlar.KurusYap(miktarAB + faizAB); } }
        public double toplamUlusal { get { return OrtakFonksiyonlar.KurusYap(miktarUlusal + faizUlusal); } }
        public double toplamToplam { get { return OrtakFonksiyonlar.KurusYap(toplamAB + toplamUlusal); } }
        public double kalanErtelemeToplam { get { return OrtakFonksiyonlar.KurusYap(ertelemeAB + ertelemeUlusal); } }

        //public double ErtelemeAB { get { return OrtakFonksiyonlar.KurusYap(ertelemeAB); } }
        //public double ErtelemeUlusal { get { return OrtakFonksiyonlar.KurusYap(ertelemeUlusal); } }


        public double toplamToplamKur(bool tl)
        {
            if (tl) return toplamToplam;
            else return toplamToplamEUR;
        }

        public double miktarABEUR { get { return OrtakFonksiyonlar.Yuvarla(miktarAB / kur, 2); } }
        public double silinmeyecekMiktarEUR { get { return OrtakFonksiyonlar.Yuvarla(silinmeyecekMiktar / kur, 2); } }

        public double miktarUlusalEUR { get { return OrtakFonksiyonlar.Yuvarla(miktarUlusal / kur, 2); } }
        public double miktarToplamEUR { get { return OrtakFonksiyonlar.KurusYap(miktarABEUR + miktarUlusalEUR); } }

        public double faizABEUR { get { return OrtakFonksiyonlar.Yuvarla(faizAB / kur, 2); } }

        public double faizUlusalEUR { get { return OrtakFonksiyonlar.Yuvarla(faizUlusal / kur, 2); } }

        public double faizToplamEUR { get { return OrtakFonksiyonlar.KurusYap(faizABEUR + faizUlusalEUR); } }

        public double toplamABEUR { get { return OrtakFonksiyonlar.KurusYap(miktarABEUR + faizABEUR); } }
        public double toplamUlusalEUR { get { return OrtakFonksiyonlar.KurusYap(miktarUlusalEUR + faizUlusalEUR); } }
        public double toplamToplamEUR { get { return OrtakFonksiyonlar.KurusYap(toplamABEUR + toplamUlusalEUR); } }

        internal void Ekle(double miktarAB, double faizAB, double miktarUlusal, double faizUlusal)
        {
            this.faizUlusal += faizUlusal;
            this.miktarUlusal += miktarUlusal;
            this.faizAB += faizAB;
            this.miktarAB += miktarAB;
        }
    }

    /// <summary>
    /// BorcHareket: Borç Ödemeleri veya Yapılan Değişilikler
    /// </summary>
    [System.Serializable()]
    public class BorcHareket : DosyaABParca
    {
        public DosyaAB anaDosya;
        public BorcHareket(DosyaAB anaDosya)
        {
            this.anaDosya = anaDosya;
            duzeltimTarihi = new TNSDateTime();
            aktarimTarihi = new TNSDateTime();
        }

        override public string ToString()
        {
            return
            "tarih:" + VeriErisim_DegerYap(this.tarih) +
            " turAd:" + VeriErisim_DegerYap(this.turAd) +
            " karar:" + VeriErisim_DegerYap(this.karar) +
            " miktarAB:" + VeriErisim_DegerYap(this.miktarAB) +
            " miktarUlusal:" + VeriErisim_DegerYap(this.miktarUlusal) +
            " faizAB:" + VeriErisim_DegerYap(this.faizAB) +
            " faizUlusal:" + VeriErisim_DegerYap(this.faizUlusal) +
            " yatirilanAB:" + VeriErisim_DegerYap(this.yatirilanAB) +
            " yatirilanUlusal:" + VeriErisim_DegerYap(this.yatirilanUlusal) +
            " duzeltimTarihi:" + VeriErisim_DegerYap(this.duzeltimTarihi) +
            " aktarimTarihi:" + VeriErisim_DegerYap(this.aktarimTarihi) +
            " aktarimTutari:" + VeriErisim_DegerYap(this.aktarimTutari) +
            " borcunIlgiliOlduguTaksit:" + VeriErisim_DegerYap(this.borcunIlgiliOlduguTaksit);

        }

        public static string VeriErisim_DegerYap(string turAd)
        {
            return "'" + turAd + "'";
        }

        public static string VeriErisim_DegerYap(double miktarAB)
        {
            return VeriErisim_DegerYap(miktarAB.ToString());
        }

        public static string VeriErisim_DegerYap(TNSDateTime tarih)
        {
            if (TNSDateTime.IsNull(tarih))
                return VeriErisim_DegerYap("");
            return VeriErisim_DegerYap(tarih.ToString());
        }

        public enum ENUMHareketTur : int
        {
            FaizHesabi = 1,
            FaizHesabiIslem = 2,

            Odeme = 51,
            TaksitOdeme = 52,
            Iade = 53,
            Mahsup = 54,
            Terkin = 59,

            Taksit = 61,
            OdeneniAktarma = 62,

            Duzeltme = 102,
            Silme = 151,
            FazlaOdemeAktarim = 152,
            Tanimsiz = 0
        }

        /// <summary>
        /// Sıra No: Hareket sıra numarası
        /// </summary>
        public int siraNo { get; set; }
        public int ilgiliIslemSiraNo { get; set; }
        /// <summary>
        /// Sıra No: Hareket sıra numarası
        /// </summary>
        public int yerNo = 0;
        /// <summary>
        /// Döviz Kuru: Geri Alım Döviz Kuru
        /// </summary>
        public double dovizKuru { get; set; }
        public double dovizKuruIslem
        {
            get
            {
                if (this.OdemeTuruIslem || this.tur == ENUMHareketTur.OdeneniAktarma || this.tur == ENUMHareketTur.Iade)
                    return dovizKuru;
                return anaDosya.geriAlimDovizKuru;
            }
        }

        /// <summary>
        /// Tarih: Hareketin Tarihi
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
        /// Tur: Ödeme/Düzeltme/Silme
        /// </summary>
        public ENUMHareketTur tur { get; set; }

        /// <summary>
        /// TurAd: Tur değişkenin açıklamasını (Ödeme/Düzeltme/Silme) döndürür
        /// </summary>
        public string turAd
        {
            get
            {
                if (tur == ENUMHareketTur.Odeme && DosyaAB.TaksitVarMi(anaDosya.taksitTalepTarihDT, this.tarihDT))
                    return "Tak. Öd.";
                else
                    return TurAdBul(tur);
            }
        }

        public static string TurAdBul(ENUMHareketTur tur)
        {
            switch (tur)
            {
                case ENUMHareketTur.Odeme: return "Ödeme";
                case ENUMHareketTur.Mahsup: return "Mahsup";
                case ENUMHareketTur.TaksitOdeme: return "Ödeme (Taksit)";
                case ENUMHareketTur.Iade: return "İade";
                case ENUMHareketTur.Terkin: return "Terkin";
                case ENUMHareketTur.Taksit: return "Taksit";
                case ENUMHareketTur.Duzeltme: return "Düzeltme";
                case ENUMHareketTur.Silme: return "Silme";
                case ENUMHareketTur.FazlaOdemeAktarim: return "Fazla Ödeme Aktarım";
                case ENUMHareketTur.OdeneniAktarma: return "Ödeme Aktarımı";
                case ENUMHareketTur.FaizHesabi: return "Faiz Hesabı";
                case ENUMHareketTur.FaizHesabiIslem: return "Faiz Hesabı (İşlem)";
            }
            return "";
        }

        /// <summary>
        /// Karar: Hareketin Tarihi
        /// </summary>
        public string karar { get; set; }

        public TNSDateTime duzeltimTarihi { get; set; }
        public DateTime duzeltimTarihiDT
        {
            get
            {
                if (duzeltimTarihi == null)
                    return DateTime.MinValue;
                else
                    return duzeltimTarihi.Oku();
            }
            set
            {
                if (duzeltimTarihi == null) duzeltimTarihi = new TNSDateTime(value);
                else duzeltimTarihi.Yaz(value);
            }
        }
        public double aktarimTutari { get; set; }
        public double aktarimTutariEUR
        {
            get
            {
                if (dovizKuruIslem != 0)
                    return OrtakFonksiyonlar.Yuvarla(aktarimTutari / dovizKuruIslem, 2);
                return 0;
            }
        }

        public double aktarimTutariKur(bool tl)
        {
            if (tl) return aktarimTutari;
            return aktarimTutariEUR;
        }

        public TNSDateTime aktarimTarihi { get; set; }
        public DateTime aktarimTarihiDT
        {
            get
            {
                if (aktarimTarihi == null)
                    return DateTime.MinValue;
                else
                    return aktarimTarihi.Oku();
            }
            set
            {
                if (aktarimTarihi == null) aktarimTarihi = new TNSDateTime(value);
                else aktarimTarihi.Yaz(value);
            }
        }

        public string borcunIlgiliOlduguTaksit { get; set; }
        //public DateTime odemeSonTarihiDT { get; set; }
        //public DateTime faizBaslangicTarihi { get; set; }

        private MiktarIslem _miktarIslem = null;

        public MiktarIslem miktarIslem
        {
            get
            {
                if (_miktarIslem == null)
                {
                    _miktarIslem = new MiktarIslem(dovizKuruIslem, miktarAB, miktarUlusal, faizAB, faizUlusal, ertelemeAB, ertelemeUl);
                }
                return _miktarIslem;
            }
        }

        /// <summary>
        /// Miktar AB: Miktarı AB
        /// </summary>
        public double miktarAB { get; set; }

        /// <summary>
        /// Miktar AB: Miktarı AB
        /// </summary>
        public double miktarABSil { get { if (anaDosya.silmeVar) return 0; else return miktarAB; } }

        /// <summary>
        /// Miktar Ulusal: Miktar Ulusal
        /// </summary>
        public double miktarUlusal { get; set; }
        public double ertelemeAB { get; set; }
        public double ertelemeUl { get; set; }
        /// <summary>
        /// Miktar Ulusal: Miktar Ulusal
        /// </summary>
        public double miktarUlusalSil { get { if (anaDosya.silmeVar) return OrtakFonksiyonlar.KurusYap(miktarAB + miktarUlusal); else return miktarUlusal; } }

        /// <summary>
        /// Faiz AB: Faiz AB
        /// </summary>
        public double faizAB { get; set; }
        /// <summary>
        /// Miktar AB: Miktarı AB
        /// </summary>
        public double faizABSil { get { if (anaDosya.silmeVar) return 0; else return faizAB; } }

        /// <summary>
        /// Faiz Ulusal: Faiz Ulusal
        /// </summary>
        public double faizUlusal { get; set; }
        /// <summary>
        /// Miktar Ulusal: Miktar Ulusal
        /// </summary>
        public double faizUlusalSil { get { if (anaDosya.silmeVar) return OrtakFonksiyonlar.KurusYap(faizAB + faizUlusal); else return faizUlusal; } }

        public double miktarToplam { get { return miktarIslem.miktarToplam; } }
        public double miktarToplamKur(bool tl)
        {
            if (tl) return miktarToplam;
            return miktarToplamEUR;
        }

        public double miktarABKur(bool tl)
        {
            if (tl) return miktarAB;
            return miktarABEUR;
        }

        public double miktarABKurSil(bool tl)
        {
            if (tl) return miktarABSil;
            return miktarABEURSil;
        }

        public double faizABKur(bool tl)
        {
            if (tl) return faizAB;
            return faizABEUR;
        }
        public double faizABKurSil(bool tl)
        {
            if (tl) return faizABSil;
            return faizABEURSil;
        }

        public double faizToplam { get { return miktarIslem.faizToplam; } }
        public double faizToplamKur(bool tl)
        {
            if (tl) return faizToplam;
            return faizToplamEUR;
        }
        public double toplamAB { get { return miktarIslem.toplamAB; } }
        public double toplamUlusal { get { return miktarIslem.toplamUlusal; } }
        public double toplamToplam { get { return miktarIslem.toplamToplam; } }
        public double miktarABEUR { get { return miktarIslem.miktarABEUR; } }
        public double miktarABEURSil { get { if (anaDosya.silmeVar) return 0; return miktarIslem.miktarABEUR; } }

        public double toplamToplamKur(bool tl)
        {
            if (tl) return toplamToplam;
            else return toplamToplamEUR;
        }

        public double miktarUlusalEUR { get { return miktarIslem.miktarUlusalEUR; } }
        public double miktarUlusalEURSil { get { if (anaDosya.silmeVar) return OrtakFonksiyonlar.KurusYap(miktarIslem.miktarABEUR + miktarIslem.miktarUlusalEUR); return miktarIslem.miktarUlusalEUR; } }
        public double miktarToplamEUR { get { return miktarIslem.miktarToplamEUR; } }

        public double miktarUlusalKur(bool tl)
        {
            if (tl) return miktarUlusal;
            else return miktarUlusalEUR;
        }

        public double miktarUlusalKurSil(bool tl)
        {
            if (tl) return miktarUlusalSil;
            else return miktarUlusalEURSil;
        }

        public double faizUlusalKur(bool tl)
        {
            if (tl) return faizUlusal;
            else return faizUlusalEUR;
        }
        public double faizUlusalKurSil(bool tl)
        {
            if (tl) return faizUlusalSil;
            else return faizUlusalEURSil;
        }

        public double faizABEUR { get { return miktarIslem.faizABEUR; } }
        public double faizABEURSil { get { if (anaDosya.silmeVar) return 0; return miktarIslem.faizABEUR; } }

        public double faizUlusalEUR { get { return miktarIslem.faizUlusalEUR; } }
        public double faizUlusalEURSil { get { if (anaDosya.silmeVar) return OrtakFonksiyonlar.KurusYap(miktarIslem.faizABEUR + miktarIslem.faizUlusalEUR); return miktarIslem.faizUlusalEUR; } }
        public double faizToplamEUR { get { return miktarIslem.faizToplamEUR; } }
        public double toplamABEUR { get { return miktarIslem.toplamABEUR; } }
        public double toplamUlusalEUR { get { return miktarIslem.toplamUlusalEUR; } }
        public double toplamToplamEUR { get { return miktarIslem.toplamToplamEUR; } }

        public double yatirilanAB { get; set; }
        public double yatirilanUlusal { get; set; }
        public double yatirilanToplam { get { return OrtakFonksiyonlar.KurusYap(yatirilanAB + yatirilanUlusal); } }

        public double yatirilanABEUR { get { return EuroYap(yatirilanAB, dovizKuruIslem); } }
        public double yatirilanUlusalEUR { get { return EuroYap(yatirilanUlusal, dovizKuruIslem); } }

        private double EuroYap(double miktar, double dovizKuru)
        {
            if (dovizKuru > 0)
                return OrtakFonksiyonlar.Yuvarla(miktar / dovizKuru, 2);
            else
                return miktar;
        }

        public double yatirilanToplamEUR { get { return OrtakFonksiyonlar.KurusYap(yatirilanABEUR + yatirilanUlusalEUR); } }

        double oran1AB, oran2AB, oran1Ul, oran2Ul;
        int ay1, ay2, gun1, gun2;

        public double kalanToplamAB { get { return anaDosya.kalanToplamAB; } }
        public double kalanToplamUlusal { get { return anaDosya.kalanToplamUlusal; } }
        public double kalanToplamToplam { get { return anaDosya.kalanToplamToplam; } }

        public double MiktarBul(int UlAbT, int ana_Faiz_Top, bool tl)
        {
            if (UlAbT == 1)
            {
                if (ana_Faiz_Top == 1)
                    return miktarUlusalKur(tl);
                else if (ana_Faiz_Top == 2)
                    return faizUlusalKur(tl);
                else
                    return OrtakFonksiyonlar.KurusYap(miktarUlusalKur(tl) + faizUlusalKur(tl));
            }
            else if (UlAbT == 2)
            {
                if (ana_Faiz_Top == 1)
                    return miktarABKur(tl);
                else if (ana_Faiz_Top == 2)
                    return faizABKur(tl);
                else
                    return OrtakFonksiyonlar.KurusYap(miktarABKur(tl) + faizABKur(tl));
            }
            else
            {
                if (ana_Faiz_Top == 1)
                    return miktarToplamKur(tl);
                else if (ana_Faiz_Top == 1)
                    return faizToplamKur(tl);
                else
                    return OrtakFonksiyonlar.KurusYap(miktarToplamKur(tl) + faizToplamKur(tl));
            }
        }

        public void OdenendenMiktarHesapla(bool yazildigiGibi, bool azaltma = false)
        {
            double carp;
            if (kalanToplamToplam <= 0)
            {
                yazildigiGibi = true;
            }
            if (yatirilanToplam > kalanToplamToplam && !azaltma) carp = kalanToplamToplam;
            else carp = yatirilanToplam;
            //OrtakFonksiyonlar.HataDosyaYaz("TaksitHesap.txt", $"A :: KTT:{kalanToplamToplam} :: YT:{yatirilanToplam} :: CARP:{carp} MA:{miktarAB} ", false);

            double yatMiktarAB = 0;
            double yatMiktarUlusal = 0;
            if (yazildigiGibi)
            {
                yatMiktarAB = yatirilanAB;
                yatMiktarUlusal = yatirilanUlusal;
            }
            else
            {
                yatMiktarAB = OrtakFonksiyonlar.Yuvarla(carp * kalanToplamAB / kalanToplamToplam, 2);
                yatMiktarUlusal = OrtakFonksiyonlar.Yuvarla(carp * kalanToplamUlusal / kalanToplamToplam, 2);
            }

            //OrtakFonksiyonlar.HataDosyaYaz("TaksitHesap.txt", $"B :: YA:{yatMiktarAB} :: YU:{yatMiktarUlusal} MA:{miktarAB} ", false);

            BorcHareket bhTaksitBul = null;
            if (DosyaAB.TaksitVarMi(anaDosya.taksitTalepTarihDT, this.IslemTarihBul()))
            {
                this.anaDosya.TaksitAyarla(this.IslemTarihBul(), false);
                BorcHareket bhTaksit = this.anaDosya.HareketBul(BorcHareket.ENUMHareketTur.Taksit, this.IslemTarihBul(), this.IslemTarihBul().AddDays(1));
                if (bhTaksit != null)
                {
                    bhTaksit.tarihDT = this.tarihDT;
                    bhTaksit.OdenendenFaizHesapla(anaDosya.tecilFaizNo, null);
                    if (this.yatirilanAB == bhTaksit.toplamAB && this.yatirilanUlusal == bhTaksit.toplamUlusal)
                    {
                        yatMiktarAB = this.yatirilanAB;
                        yatMiktarUlusal = this.yatirilanUlusal;
                        bhTaksitBul = bhTaksit;
                    }
                }
            }
            //OrtakFonksiyonlar.HataDosyaYaz("TaksitHesap.txt", $"C :: YA:{yatMiktarAB} :: YU:{yatMiktarUlusal} MA:{miktarAB} ", false);

            double artan = OrtakFonksiyonlar.KurusYap(yatMiktarAB + yatMiktarUlusal - carp);
            if (artan > 0)
                yatMiktarUlusal -= artan;
            else if (artan < 0)
                yatMiktarAB -= artan;
            double _miktar, _faiz;
            if (DosyaAB.TaksitVarMi(anaDosya.taksitTalepTarihDT, this.IslemTarihBul()))
            {
                if (bhTaksitBul != null)
                {
                    miktarAB = bhTaksitBul.miktarAB;
                    faizAB = bhTaksitBul.faizAB;
                    miktarUlusal = bhTaksitBul.miktarUlusal;
                    faizUlusal = bhTaksitBul.faizUlusal;
                    ertelemeAB = bhTaksitBul.ertelemeAB;
                    ertelemeUl = bhTaksitBul.ertelemeUl;
                }
                else
                {
                    double _tecilFaizi, _ertelemeFaizi;
                    FaizIslemler.FaizBol2(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, yatMiktarAB, anaDosya.tecilFaizNo, out _miktar, out _tecilFaizi, out _ertelemeFaizi, 1, anaDosya.ertelemeOranAB);
                    miktarAB = _miktar;
                    ertelemeAB = _ertelemeFaizi;
                    faizAB = _tecilFaizi + _ertelemeFaizi;
                    FaizIslemler.FaizBol2(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, yatMiktarUlusal, anaDosya.tecilFaizNo, out _miktar, out _tecilFaizi, out _ertelemeFaizi, 1, anaDosya.ertelemeOranUlusal);
                    miktarUlusal = _miktar;
                    ertelemeUl = _ertelemeFaizi;
                    faizUlusal = _tecilFaizi + _ertelemeFaizi;
                }
            }
            else
            {
                FaizIslemler.FaizBol(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, DateTime.MinValue, DateTime.MinValue, IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, yatMiktarAB, true, anaDosya.tecilFaizNo, out _miktar, out _faiz, 1);
                miktarAB = _miktar;
                faizAB = _faiz;
                FaizIslemler.FaizBol(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, DateTime.MinValue, DateTime.MinValue, IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, yatMiktarUlusal, true, anaDosya.tecilFaizNo, out _miktar, out _faiz, 1);
                miktarUlusal = _miktar;
                faizUlusal = _faiz;
            }
            //OrtakFonksiyonlar.HataDosyaYaz("TaksitHesap.txt", $"D :: YA:{yatMiktarAB} :: YU:{yatMiktarUlusal} :: MA:{miktarAB} ", false);
        }

        public void OdenendenFaizHesapla(int tacilFaizNo, StringBuilder sonucBilgi)
        {
            if (anaDosya.taksitTalepTarihDT == DateTime.MinValue || IslemTarihBul() <= anaDosya.taksitTalepTarihDT)
            {
                faizAB = FaizIslemler.FaizHesapla(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, miktarAB, anaDosya.oncekiGunHesapla, anaDosya.faizinDurdurulmaTarihiDT, tacilFaizNo, sonucBilgi, out oran1AB, out oran2AB, out ay1, out ay2, out gun1, out gun2);
                faizUlusal = FaizIslemler.FaizHesapla(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, miktarUlusal, anaDosya.oncekiGunHesapla, anaDosya.faizinDurdurulmaTarihiDT, tacilFaizNo, sonucBilgi, out oran1Ul, out oran2Ul, out ay1, out ay2, out gun1, out gun2);
            }
            else
            {
                double oranTut;
                DateTime tutDT = anaDosya.taksitTalepTarihDT.AddDays(-1);
                if (ertelemeAB == 0 && ertelemeUl == 0)
                {
                    ertelemeAB = FaizIslemler.FaizHesapla(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), tutDT, anaDosya.faizinDurdurulmaTarihiDT, miktarAB, anaDosya.oncekiGunHesapla, anaDosya.faizinDurdurulmaTarihiDT, tacilFaizNo, sonucBilgi, out oran1AB, out oranTut, out ay1, out ay2, out gun1, out gun2);
                    ertelemeUl = FaizIslemler.FaizHesapla(anaDosya.odemeSonTarihiDT, anaDosya.faizBaslangicTarihi, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), tutDT, anaDosya.faizinDurdurulmaTarihiDT, miktarUlusal, anaDosya.oncekiGunHesapla, anaDosya.faizinDurdurulmaTarihiDT, tacilFaizNo, sonucBilgi, out oran1Ul, out oranTut, out ay1, out ay2, out gun1, out gun2);
                }
                faizAB = OrtakFonksiyonlar.KurusYap(ertelemeAB + FaizIslemler.FaizHesapla(tutDT, tutDT, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, OrtakFonksiyonlar.KurusYap(miktarAB + ertelemeAB), anaDosya.oncekiGunHesapla, anaDosya.faizinDurdurulmaTarihiDT, tacilFaizNo, sonucBilgi, out oranTut, out oran2AB, out ay1, out ay2, out gun1, out gun2));
                faizUlusal = OrtakFonksiyonlar.KurusYap(ertelemeUl + FaizIslemler.FaizHesapla(tutDT, tutDT, anaDosya.taksitTalepTarihDT, anaDosya.taksitGirisTarih.Oku(), IslemTarihBul(), anaDosya.faizinDurdurulmaTarihiDT, OrtakFonksiyonlar.KurusYap(miktarUlusal + ertelemeUl), anaDosya.oncekiGunHesapla, anaDosya.faizinDurdurulmaTarihiDT, tacilFaizNo, sonucBilgi, out oranTut, out oran2Ul, out ay1, out ay2, out gun1, out gun2));
            }
            _miktarIslem = null;
        }

        private DateTime IslemTarihBul()
        {
            DateTime ilgiliTarih = IlgiliIslemTarihBul();
            if (ilgiliTarih != DateTime.MinValue)
                return ilgiliTarih;
            return tarihDT;
        }

        private int odemeDurum = -1;
        public DateTime IlgiliIslemTarihBul()
        {
            if (ilgiliIslemSiraNo > 0)
            {
                BorcHareket ilgili = anaDosya.HareketBul(ilgiliIslemSiraNo);
                if (ilgili != null)
                {
                    return ilgili.tarihDT;
                }
            }
            return DateTime.MinValue;
        }

        //private bool silmeVar;

        public double tecilFaizi { get { return OrtakFonksiyonlar.KurusYap(faizToplam - gecikmeZammi); } }

        public bool sonTaksitmi
        {
            get
            {
                if (tur == ENUMHareketTur.Taksit)
                    return !anaDosya.BendenSonraTaksitVarMi(this.siraNo);
                else
                    return false;
            }
        }
        public double gecikmeZammi { get { return OrtakFonksiyonlar.KurusYap(gecikmeZammiAB + gecikmeZammiUL); } }
        public double gecikmeZammiAB
        {
            get
            {
                return ertelemeAB;
            }
        }

        public static DateTime OncekiGun(DateTime tarih)
        {
            if (tarih == DateTime.MinValue) return tarih;
            else return tarih.AddDays(-1);
        }

        public double gecikmeZammiUL
        {
            get
            {
                return ertelemeUl;
            }
        }

        public int taksitGun { get { return (duzeltimTarihiDT - anaDosya.taksitTalepTarihDT).Days + 1; } }

        public int OdemeDurum
        {
            get
            {
                if (odemeDurum == -1)
                {
                    if (OrtakFonksiyonlar.EsitMi(yatirilanToplam, kalanToplamToplam))
                    {
                        if (ipardDonemi == 3) odemeDurum = 1;
                        else
                        {
                            if (OrtakFonksiyonlar.EsitMi(yatirilanAB, kalanToplamABSil)) odemeDurum = 1;
                            else if (kalanToplamABSil < yatirilanAB) odemeDurum = 2;
                            else odemeDurum = 3;
                        }
                    }
                    else if (yatirilanToplam < kalanToplamToplam)
                    {
                        if (ipardDonemi == 3) odemeDurum = 2;
                        else
                        {
                            if (OrtakFonksiyonlar.EsitMi(yatirilanAB, toplamABSil) || OrtakFonksiyonlar.EsitMi(yatirilanUlusal, toplamUlusalSil)) odemeDurum = 4;
                            else if (toplamABSil < yatirilanAB) odemeDurum = 5;
                            else if (toplamUlusalSil < yatirilanUlusal) odemeDurum = 6;
                            else odemeDurum = 4;
                        }
                    }
                    else
                    {
                        if (ipardDonemi == 3) odemeDurum = 3;
                        else
                        {
                            if (yatirilanAB >= kalanToplamABSil && yatirilanUlusal >= kalanToplamUlusalSil) odemeDurum = 7;
                            else if (kalanToplamABSil < yatirilanAB) odemeDurum = 8;
                            else odemeDurum = 9;
                        }
                    }
                }
                return odemeDurum;
            }
        }

        public string OdemeDurumAd
        {
            get
            {
                switch (OdemeDurum)
                {
                    case 1: return "Yeterli ve doğru ödeme";
                    case 2: return "Yeterli fakat doğru olmayan geri ödeme - AB Yatırılandan Az";
                    case 3: return "Yeterli fakat doğru olmayan geri ödeme - Ulusal Yatırılandan Az";
                    case 4: return "Yetersiz fakat Doğru Geri ödeme";
                    case 5: return "Yetersiz ve doğru olmayan geri ödeme - AB Yatırılandan Az";
                    case 6: return "Yetersiz ve doğru olmayan geri ödeme - Ulusal Yatırılandan Az";
                    //case 7: return "Yetersiz ve Emanate alınması gereken tutar";
                    case 7: return "Fazla ve doğru ödeme";
                    case 8: return "Fazla fakat doğru olmayan geri ödeme - AB Yatırılandan Az";
                    case 9: return "Fazla fakat doğru olmayan geri ödeme - Ulusal Yatırılandan Az";
                    default: return "";
                }
            }
        }
        public string DurumKodAd { get { return OdemeDurum + "-" + OdemeDurumAd; } }

        public double aktarAB { get { return OrtakFonksiyonlar.KurusYap(toplamABSil - yatirilanAB); } }
        public double aktarABEUR { get { return EuroYap(aktarAB, dovizKuruIslem); } }
        public double aktarUlusal { get { return OrtakFonksiyonlar.KurusYap(toplamUlusalSil - yatirilanUlusal); } }
        public double aktarUlusalEUR { get { return EuroYap(aktarUlusal, dovizKuruIslem); } }
        public double aktarKisiye { get { return OrtakFonksiyonlar.KurusYap(yatirilanToplam - toplamToplam); } }
        public double aktarKisiyeEUR { get { return EuroYap(aktarKisiye, dovizKuruIslem); } }

        public double toplamABSil { get { if (anaDosya.silmeVar) return 0; else return miktarIslem.toplamAB; } }
        public double toplamABEURSil { get { if (anaDosya.silmeVar) return 0; else return miktarIslem.toplamABEUR; } }

        public double toplamUlusalSil { get { if (anaDosya.silmeVar) return OrtakFonksiyonlar.KurusYap(miktarIslem.toplamUlusal + miktarIslem.toplamAB); else return miktarIslem.toplamUlusal; } }

        public double toplamUlusalEURSil { get { if (anaDosya.silmeVar) return OrtakFonksiyonlar.KurusYap(miktarIslem.toplamUlusalEUR + miktarIslem.toplamABEUR); else return miktarIslem.toplamUlusalEUR; } }

        public double kalanToplamABSil { get { return anaDosya.kalanToplamABSil; } }
        public double kalanToplamUlusalSil { get { return anaDosya.kalanToplamUlusalSil; } }

        public bool HataliOdeme
        {
            get
            {
                if (aktarKisiye > 0 || aktarUlusal > 0 || aktarAB > 0) return true;
                return false;
            }
        }

        public bool HataliOdemeLimitli
        {
            get
            {
                if (aktarKisiye > 20 || aktarUlusal > 20 || aktarAB > 20) return true;
                return false;
            }
        }

        public bool OdemeTuruIslem
        {
            get
            {
                return OdemeTuruIslemMi(tur);
            }
        }

        public bool IadeDuzeltmeTuruIslem
        {
            get
            {
                return IadeDuzeltmeTuruIslemMi(tur);
            }
        }

        public bool FaizIslemi
        {
            get
            {
                return FaizIslemiMi(tur);
            }
        }

        public static bool OdemeTuruIslemMi(BorcHareket.ENUMHareketTur tur)
        {
            return tur == BorcHareket.ENUMHareketTur.Odeme || tur == BorcHareket.ENUMHareketTur.Mahsup || tur == BorcHareket.ENUMHareketTur.TaksitOdeme || tur == BorcHareket.ENUMHareketTur.Terkin || tur == BorcHareket.ENUMHareketTur.Iade;
        }

        public static bool IadeDuzeltmeTuruIslemMi(BorcHareket.ENUMHareketTur tur)
        {
            return tur == BorcHareket.ENUMHareketTur.Iade || tur == BorcHareket.ENUMHareketTur.Duzeltme;
        }

        public static bool FaizIslemiMi(BorcHareket.ENUMHareketTur tur)
        {
            return tur == BorcHareket.ENUMHareketTur.FaizHesabi || tur == BorcHareket.ENUMHareketTur.FaizHesabiIslem;
        }

        public void silmeDuzelt()
        {
            if (this.miktarAB == 0 && miktarUlusal > 0)
            {
                double toplam = OrtakFonksiyonlar.KurusYap(miktarAB + miktarUlusal);
                miktarAB = OrtakFonksiyonlar.KurusYap(toplam * TNS.ATM.Arac.ABOranDondur(ipardDonemi));
                miktarUlusal = OrtakFonksiyonlar.KurusYap(toplam - miktarAB);
            }
            if (this.faizAB == 0 && faizUlusal > 0)
            {
                double toplam = OrtakFonksiyonlar.KurusYap(faizAB + faizUlusal);
                faizAB = OrtakFonksiyonlar.KurusYap(toplam * TNS.ATM.Arac.ABOranDondur(ipardDonemi));
                faizUlusal = OrtakFonksiyonlar.KurusYap(toplam - faizAB);
            }
        }
    }


    /// <summary>
    /// Alacak Takip dosyası tarihçe bilgileri sınıfı.
    /// </summary>
    [Serializable()]
    public class TarihceAB
    {
        public enum TarihceIslemKodlar : int
        {
            Kaydet = 1,
            KaydetHareket = 5,
            DetaySil = 9,
            Taksitlendirme = 50,
            TaksitKaldirma = 59
        }

        public TarihceIslemKodlar islemKod { get; set; }
        public string islemAd
        {
            get
            {
                return TarihceIslemAd(islemKod);
            }
        }

        public static string TarihceIslemAd(TarihceIslemKodlar islemKod)
        {
            switch (islemKod)
            {
                case TarihceIslemKodlar.Kaydet: return "Dosya Bilgisi Kaydetme";
                case TarihceIslemKodlar.KaydetHareket: return "Dosya İşlem Bilgisi Ekleme";
                case TarihceIslemKodlar.DetaySil: return "Dosya İşlem Bilgisi Silme";
                case TarihceIslemKodlar.Taksitlendirme: return "Dosya Taksitlendirildi";
                case TarihceIslemKodlar.TaksitKaldirma: return "Dosya Taksitleri Silindi";
                default: return "";
            }
        }

        public TNSDateTime zaman { get; set; }
        public DateTime zamanDT
        {
            get
            {
                if (!TNSDateTime.IsNull(zaman)) return zaman.Oku(); return DateTime.MinValue;
            }
        }

        public string islemiYapan { get; set; }
        public string metin { get; set; }
        public string mernis { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }

        public string Kim
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ad)) return islemiYapan;
                return mernis + "-" + ad + " " + soyad;
            }
        }
    }


    public static class BorcTurleriSinif
    {
        public static TNSCollection[] borcTuruListesi = new TNSCollection[8];

        public static string BorcTuruAdBul(DosyaAB.BorcTurleri borcTuru, string kod)
        {
            int indis = ((int)borcTuru) - 1;
            if (borcTuruListesi[indis] == null || borcTuruListesi[indis].Count == 0)
            {
                if (borcTuruListesi[indis] == null)
                    borcTuruListesi[indis] = new TNSCollection();
                BorcKoduDoldur((int)borcTuru, borcTuruListesi[indis]);
            }
            int i = borcTuruListesi[indis].IndexOf(kod);
            if (i >= 0)
            {
                TNS.BKD.ABSKod k = (TNS.BKD.ABSKod)(borcTuruListesi[indis])[i];
                return k.kodAd;
            }
            return "";
        }

        public static string BorcTuruAdListeBul(DosyaAB.BorcTurleri borcTuru, string borcunKodu)
        {
            List<StrKodStrAd> l = ListeOl(borcTuru, borcunKodu);
            string tut = "";
            foreach (StrKodStrAd item in l)
            {
                if (tut != "") tut += ", ";
                tut += item.ad;
            }
            return tut;
        }

        public static void BorcKoduDoldur(int borcTuru, TNSCollection borcTuruListesi)
        {
            if (borcTuru < 1 || borcTuru > 8) borcTuru = 1;
            TNSCollection tc = TNS.BKD.Arac.Tanimla().KodListele(0, "32", "").absKodlar;
            foreach (TNS.BKD.ABSKod item in tc)
            {
                if ((int)item.ekAlanlar[0].Deger == borcTuru)
                    borcTuruListesi.Add(item);
            }
        }


        public static List<StrKodStrAd> ListeOl(DosyaAB.BorcTurleri borcTuru, string borcunKodu)
        {
            List<StrKodStrAd> t = new List<StrKodStrAd>();
            if (!string.IsNullOrWhiteSpace(borcunKodu))
            {
                string[] eleler = borcunKodu.Split(',', ';');
                foreach (string item in eleler)
                {
                    string ad = BorcTurleriSinif.BorcTuruAdBul(borcTuru, item);
                    if (!string.IsNullOrWhiteSpace(ad))
                        t.Add(new StrKodStrAd(item, ad));
                }
            }
            return t;
        }
    }

    /// <summary>
    /// Alacak Takip dosyası bilgileri sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaAB
    {
        public TNSCollection borcuBilgileri = new TNSCollection();
        public TNSCollection durumBilgileri = new TNSCollection();
        public TNSCollection TaksitBilgileri = new TNSCollection();
        public TNSCollection OnBorcBilgileri = new TNSCollection();
        public TNSCollection GAKBilgileri = new TNSCollection();

        override public string ToString()
        {
            return
               "defterSirasi:" + BorcHareket.VeriErisim_DegerYap(defterSirasi) +
               " tedbir:" + BorcHareket.VeriErisim_DegerYap(tedbir) +
               " altTedbir:" + BorcHareket.VeriErisim_DegerYap(altTedbir) +
               " butceKalemi:" + BorcHareket.VeriErisim_DegerYap(butceKalemi) +
               " saymanlikKod:" + BorcHareket.VeriErisim_DegerYap(saymanlikKod) +
               " birimKod:" + BorcHareket.VeriErisim_DegerYap(birimKod) +
               " borcluKod:" + BorcHareket.VeriErisim_DegerYap(borcluKod) +
               " basvuruKayitNumarasi:" + BorcHareket.VeriErisim_DegerYap(basvuruKayitNumarasi) +
               " sozlesmeNumarasi:" + BorcHareket.VeriErisim_DegerYap(sozlesmeNumarasi) +
               " borcunKodu:" + BorcHareket.VeriErisim_DegerYap(borcunKodu) +
               " vakaNumarasi:" + BorcHareket.VeriErisim_DegerYap(vakaNumarasi) +
               " avans:" + BorcHareket.VeriErisim_DegerYap(avans.ToString()) +
               " onBorcTarihi:" + BorcHareket.VeriErisim_DegerYap(onBorcTarihi) +
               " onBorcSayi:" + BorcHareket.VeriErisim_DegerYap(onBorcSayi) +
               " onBorcMiktarAB:" + BorcHareket.VeriErisim_DegerYap(onBorcMiktarAB) +
               " onBorcMiktarUlusal:" + BorcHareket.VeriErisim_DegerYap(onBorcMiktarUlusal) +
               " geriAlimTarihi:" + BorcHareket.VeriErisim_DegerYap(geriAlimKarariTarihi) +
               " geriAlimSayi:" + BorcHareket.VeriErisim_DegerYap(geriAlimSayi) +
               " bildirimTarihi:" + BorcHareket.VeriErisim_DegerYap(bildirimTarihi) +
               " tesellumTarihi:" + BorcHareket.VeriErisim_DegerYap(tesellumTarihi) +
               " odemeSonTarihi:" + BorcHareket.VeriErisim_DegerYap(odemeSonTarihi) +
               " geriAlimMiktarAB:" + BorcHareket.VeriErisim_DegerYap(geriAlimMiktarAB) +
               " geriAlimMiktarUlusal:" + BorcHareket.VeriErisim_DegerYap(geriAlimMiktarUlusal) +
               " temerrutTarihi:" + BorcHareket.VeriErisim_DegerYap(temerrutTarihi) +
               " faizMiktarAB:" + BorcHareket.VeriErisim_DegerYap(faizMiktarAB) +
               " faizMiktarUlusal:" + BorcHareket.VeriErisim_DegerYap(faizMiktarUlusal) +
               " yasalTakipTarihi:" + BorcHareket.VeriErisim_DegerYap(yasalTakipTarihi) +
               " geriAlimSurecAsamasi:" + BorcHareket.VeriErisim_DegerYap(geriAlimSurecAsamasiAd) +
               " donemYil:" + BorcHareket.VeriErisim_DegerYap(donemYil) +
               " donemCeyrek:" + BorcHareket.VeriErisim_DegerYap(donemCeyrek) +
               " borcTuru:" + BorcHareket.VeriErisim_DegerYap(borcTuruAd) +
               " ipardAktarimTar:" + BorcHareket.VeriErisim_DegerYap(ipardAktarimTar) +
               " yorumlarTR:" + BorcHareket.VeriErisim_DegerYap(yorumlarTR) +
               " yorumlarEN:" + BorcHareket.VeriErisim_DegerYap(yorumlarEN);
        }

        public void TaksitleriSil()
        {
            for (int i = this.borcHareket.Count - 1; i >= 0; i--)
            {
                if (((BorcHareket)this.borcHareket[i]).tur == BorcHareket.ENUMHareketTur.Taksit)
                {
                    this.borcHareket.RemoveAt(i);
                }
            }
        }

        public void YeniTaksitYap(int taksitSay, DosyaABTaksit islemTaksit)
        {
            if (taksitSay < 1 || taksitSay > 1000)
                taksitSay = 0;
            bool oncekiGunHesaplaSakla = this.oncekiGunHesapla;
            this.oncekiGunHesapla = true;

            TNSDateTime dtTaksitTalepTarihi = new TNSDateTime(this.taksitTalepTarih.Oku());
            TNSDateTime dtGirisTarihi = new TNSDateTime(this.taksitGirisTarih.Oku());
            this.IslemToplamBul(dtTaksitTalepTarihi, -1, true, islemTaksit, dtGirisTarihi, -1, true);

            double _kalanMiktarAB = this.kalanMiktarAB;
            double _kalanMiktarUlusal = this.kalanMiktarUlusal;
            double _kalanMiktarABFaiz = this.kalanFaizMiktarAB;
            double _kalanMiktarUlusalFaiz = this.kalanFaizMiktarUlusal;
            this.oncekiGunHesapla = oncekiGunHesaplaSakla;

            double taksitTutarAB = OrtakFonksiyonlar.KurusYap(_kalanMiktarAB / taksitSay);
            double taksitTutarUl = OrtakFonksiyonlar.KurusYap(_kalanMiktarUlusal / taksitSay);
            double taksitTutarABFaiz = OrtakFonksiyonlar.KurusYap(_kalanMiktarABFaiz / taksitSay);
            double taksitTutarUlFaiz = OrtakFonksiyonlar.KurusYap(_kalanMiktarUlusalFaiz / taksitSay);

            TaksitleriSil();
            StringBuilder sonucBilgi = new StringBuilder();
            for (int i = 0; i < taksitSay; i++)
            {
                DateTime taksitTar = taksitIlkTarihDT.AddMonths(i);
                while (TNS.ResmiTatil.ResmiTatilMi(taksitTar, 0))
                {
                    taksitTar = taksitTar.AddDays(1);
                }
                if (i == taksitSay - 1)
                {
                    taksitTutarAB = _kalanMiktarAB;
                    taksitTutarUl = _kalanMiktarUlusal;
                    taksitTutarABFaiz = _kalanMiktarABFaiz;
                    taksitTutarUlFaiz = _kalanMiktarUlusalFaiz;
                }
                _kalanMiktarAB = OrtakFonksiyonlar.KurusYap(_kalanMiktarAB - taksitTutarAB);
                _kalanMiktarUlusal = OrtakFonksiyonlar.KurusYap(_kalanMiktarUlusal - taksitTutarUl);
                _kalanMiktarABFaiz = OrtakFonksiyonlar.KurusYap(_kalanMiktarABFaiz - taksitTutarABFaiz);
                _kalanMiktarUlusalFaiz = OrtakFonksiyonlar.KurusYap(_kalanMiktarUlusalFaiz - taksitTutarUlFaiz);

                BorcHareket detay = new BorcHareket(this);
                detay.tur = BorcHareket.ENUMHareketTur.Taksit;
                detay.duzeltimTarihi.Yaz(taksitTar);

                detay.miktarAB = taksitTutarAB;
                detay.miktarUlusal = taksitTutarUl;
                detay.ertelemeAB = taksitTutarABFaiz;
                detay.ertelemeUl = taksitTutarUlFaiz;

                detay.siraNo = i + 1;
                detay.tarihDT = taksitTar;
                detay.OdenendenFaizHesapla(this.tecilFaizNo, sonucBilgi);
                detay.tarih = null;
                detay.aktarimTarihi = null;
                this.borcHareket.Add(detay);
            }
            //OrtakFonksiyonlar.DebugYaz(null, sonucBilgi.ToString());
        }

        public enum ENUMDosyaDurum : int
        {
            ONBORC = 9,
            GERIALIM = 1,
            YASALTAKIP = 2,
            GERIODEME = 3,
            GERIODEMEKISMEN = 12,
            GERIODEMETAMAMEN = 13,
            MAHSUP = 4,
            YURUTMEYIDURDURMA = 5,
            KESINLESMISMAHKEMEKARARI = 6,
            GERIALINAMAYANBORCUNSILINMESI = 7,
            SILINMISBORCLARDEVAMEDENADIMLAR = 8,
            SILINMISBORCLARTAMODEME = 14,
            SILINMISBORCLARKISMIODEME = 15,
            TAKSITLENDIRILMIS = 10,
            SILINMISTAKSITLENDIRILMIS = 16,
            ONBORCIPTALI = 11,
            KESINBORCIPTALI = 17,
            TERKIN = 18,
            IFLAS_ACZ = 19,
            TANIMSIZ = 0
        }

        public enum BorcTurleri : int
        {
            UsulsuzluklerToptan = 1,
            FazlaOdeme = 2,
            HataliOdeme = 3,
            SozlesmeninFeshi = 4,
            DigerToptan = 5,
            DigerKismi = 6,
            UsulsuzluklerKismi = 7,
            Yaptirimlar = 8,
            Tanimsiz = 0
        }

        public static TNSCollection ListeleBorcTurleri()
        {
            TNSCollection tc = new TNSCollection();
            tc.Add(new IntKodStrAd((int)BorcTurleri.UsulsuzluklerToptan, BorcTuruAd(BorcTurleri.UsulsuzluklerToptan)));
            tc.Add(new IntKodStrAd((int)BorcTurleri.UsulsuzluklerKismi, BorcTuruAd(BorcTurleri.UsulsuzluklerKismi)));
            tc.Add(new IntKodStrAd((int)BorcTurleri.FazlaOdeme, BorcTuruAd(BorcTurleri.FazlaOdeme)));
            tc.Add(new IntKodStrAd((int)BorcTurleri.HataliOdeme, BorcTuruAd(BorcTurleri.HataliOdeme)));
            tc.Add(new IntKodStrAd((int)BorcTurleri.SozlesmeninFeshi, BorcTuruAd(BorcTurleri.SozlesmeninFeshi)));
            tc.Add(new IntKodStrAd((int)BorcTurleri.DigerToptan, BorcTuruAd(BorcTurleri.DigerToptan)));
            tc.Add(new IntKodStrAd((int)BorcTurleri.DigerKismi, BorcTuruAd(BorcTurleri.DigerKismi)));
            tc.Add(new IntKodStrAd((int)BorcTurleri.Yaptirimlar, BorcTuruAd(BorcTurleri.Yaptirimlar)));
            return tc;
        }

        public static string BorcTuruAd(BorcTurleri borcTuru)
        {
            switch (borcTuru)
            {
                case BorcTurleri.UsulsuzluklerToptan:
                    return "Usulsüzlükler/Toptan";
                case BorcTurleri.UsulsuzluklerKismi:
                    return "Usulsüzlükler/Kısmi";
                case BorcTurleri.FazlaOdeme:
                    return "Fazla Ödeme";
                case BorcTurleri.HataliOdeme:
                    return "Hatalı Ödeme";
                case BorcTurleri.SozlesmeninFeshi:
                    return "Sözleşmenin Feshi";
                case BorcTurleri.DigerToptan:
                    return "Diğer/Toptan";
                case BorcTurleri.DigerKismi:
                    return "Diğer/Kısmi";
                case BorcTurleri.Yaptirimlar:
                    return "Yaptırımlar";
            }
            return "";
        }

        public static string DosyaDurumAdBul(ENUMDosyaDurum durum)
        {
            switch (durum)
            {
                case ENUMDosyaDurum.GERIALIM: return "Geri alım kararı";
                case ENUMDosyaDurum.YASALTAKIP: return "Durumun yasal takibe düşmesi";
                case ENUMDosyaDurum.GERIODEME: return "Doğrudan geri ödeme";
                case ENUMDosyaDurum.MAHSUP: return "Mahsup";
                case ENUMDosyaDurum.YURUTMEYIDURDURMA: return "Yürütmeyi durdurma kararı";
                case ENUMDosyaDurum.KESINLESMISMAHKEMEKARARI: return "Kesinleşmiş mahkeme kararı";
                case ENUMDosyaDurum.GERIALINAMAYANBORCUNSILINMESI: return "Geri alınamamazlık nedeniyle borcun silinmesi";
                case ENUMDosyaDurum.SILINMISBORCLARDEVAMEDENADIMLAR: return "Silinmiş borçlar için devam eden geri alımlar";
                case ENUMDosyaDurum.ONBORC: return "Ön Borç";
                case ENUMDosyaDurum.TAKSITLENDIRILMIS: return "Taksitlendirilmiş borçlar";
                case ENUMDosyaDurum.ONBORCIPTALI: return "Ön Borcun iptali";
                case ENUMDosyaDurum.GERIODEMEKISMEN: return "Kısmen geri alım";
                case ENUMDosyaDurum.GERIODEMETAMAMEN: return "Tamamen geri alım";

                case ENUMDosyaDurum.SILINMISBORCLARTAMODEME: return "Silinmiş borçların tamamen geri alımı";
                case ENUMDosyaDurum.SILINMISBORCLARKISMIODEME: return "Silinmiş borçların kısmi geri alımı";
                case ENUMDosyaDurum.SILINMISTAKSITLENDIRILMIS: return "Silinmiş borçlardan taksitlendirme";
                case ENUMDosyaDurum.KESINBORCIPTALI: return "Kesin Borç iptali";
                case ENUMDosyaDurum.TERKIN: return "Terkin";
                case ENUMDosyaDurum.IFLAS_ACZ: return "İflas/Acz";
            }
            return "";
        }
        public static string DosyaDurumAdBulEn(ENUMDosyaDurum durum)
        {
            switch (durum)
            {
                case ENUMDosyaDurum.GERIALIM: return "Recovery Decision";
                case ENUMDosyaDurum.YASALTAKIP: return "Case subject to judicial procedure";
                case ENUMDosyaDurum.GERIODEME: return "Direct Recovery";
                case ENUMDosyaDurum.MAHSUP: return "Offset";
                case ENUMDosyaDurum.YURUTMEYIDURDURMA: return "The decision to suspend execution";
                case ENUMDosyaDurum.KESINLESMISMAHKEMEKARARI: return "Final court decision";
                case ENUMDosyaDurum.GERIALINAMAYANBORCUNSILINMESI: return "Writing-off due to irrecoverability";
                case ENUMDosyaDurum.SILINMISBORCLARDEVAMEDENADIMLAR: return "Written-off debts";
                case ENUMDosyaDurum.ONBORC: return "Pre-debt";
                case ENUMDosyaDurum.TAKSITLENDIRILMIS: return "Recovery via installment";
                case ENUMDosyaDurum.ONBORCIPTALI: return "Pre-debt Cancellation";
                case ENUMDosyaDurum.GERIODEMEKISMEN: return "Partial recovery";
                case ENUMDosyaDurum.GERIODEMETAMAMEN: return "Complete recovery";

                case ENUMDosyaDurum.SILINMISBORCLARTAMODEME: return "Complete recovery of Written-off debts";
                case ENUMDosyaDurum.SILINMISBORCLARKISMIODEME: return "Partial recovery of Written-off debts";
                case ENUMDosyaDurum.SILINMISTAKSITLENDIRILMIS: return "Installment of Written-off debts";
                case ENUMDosyaDurum.KESINBORCIPTALI: return "Recovery decision Cancellation";
                case ENUMDosyaDurum.TERKIN: return "Deleting the debt";
                case ENUMDosyaDurum.IFLAS_ACZ: return "Bankruptcy/Insolvency";
            }
            return "";
        }

        public bool SilmeVarMi(TNSDateTime tarih, out BorcHareket silmeHareket)
        {
            if (this.borcTuru == DosyaAB.BorcTurleri.FazlaOdeme)
            {
                silmeHareket = null;
                return true;
            }
            foreach (BorcHareket bh in borcHareket)
            {
                if (bh.tur == BorcHareket.ENUMHareketTur.Silme)
                {
                    if (TNSDateTime.IsNull(tarih) || bh.tarihDT <= tarih.Oku())
                    {
                        silmeHareket = bh;
                        return true;
                    }
                }
            }
            silmeHareket = null;
            return false;
        }

        public static TNSCollection DosyaDurumListesiBul(bool tanimsizEkle)
        {
            TNSCollection tc = new TNSCollection();

            if (tanimsizEkle)
                tc.Add(BilgiOl(ENUMDosyaDurum.TANIMSIZ));
            tc.Add(BilgiOl(ENUMDosyaDurum.ONBORC));
            tc.Add(BilgiOl(ENUMDosyaDurum.GERIALIM));
            tc.Add(BilgiOl(ENUMDosyaDurum.YASALTAKIP));
            tc.Add(BilgiOl(ENUMDosyaDurum.GERIODEME));
            tc.Add(BilgiOl(ENUMDosyaDurum.GERIODEMEKISMEN));
            tc.Add(BilgiOl(ENUMDosyaDurum.GERIODEMETAMAMEN));
            tc.Add(BilgiOl(ENUMDosyaDurum.MAHSUP));
            tc.Add(BilgiOl(ENUMDosyaDurum.YURUTMEYIDURDURMA));
            tc.Add(BilgiOl(ENUMDosyaDurum.KESINLESMISMAHKEMEKARARI));
            tc.Add(BilgiOl(ENUMDosyaDurum.GERIALINAMAYANBORCUNSILINMESI));
            tc.Add(BilgiOl(ENUMDosyaDurum.SILINMISBORCLARDEVAMEDENADIMLAR));
            tc.Add(BilgiOl(ENUMDosyaDurum.TAKSITLENDIRILMIS));
            tc.Add(BilgiOl(ENUMDosyaDurum.ONBORCIPTALI));
            tc.Add(BilgiOl(ENUMDosyaDurum.SILINMISBORCLARTAMODEME));
            tc.Add(BilgiOl(ENUMDosyaDurum.SILINMISBORCLARKISMIODEME));
            tc.Add(BilgiOl(ENUMDosyaDurum.SILINMISTAKSITLENDIRILMIS));
            tc.Add(BilgiOl(ENUMDosyaDurum.KESINBORCIPTALI));
            tc.Add(BilgiOl(ENUMDosyaDurum.TERKIN));
            tc.Add(BilgiOl(ENUMDosyaDurum.IFLAS_ACZ));

            return tc;
        }

        private static IntKodStrAd BilgiOl(ENUMDosyaDurum durum)
        {
            return new IntKodStrAd((int)durum, DosyaDurumAdBul(durum));
        }

        public DosyaAB(int ipardDonem, int dosyaAnahtar) : base()
        {
            this.iPARDDonemi = ipardDonem;
            this.dosyaAnahtar = dosyaAnahtar;
            this.defterSirasi = "";
        }
        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaAB()
        {
            borcHareket = new TNSCollection();
            odemeSonTarihi = new TNSDateTime();
            temerrutTarihi = new TNSDateTime();
            taksitGirisTarih = new TNSDateTime();
            taksitTalepTarih = new TNSDateTime();
            taksitIlkTarih = new TNSDateTime();
            taksitKararTarih = new TNSDateTime();
            geriAlimKarariTarihi = new TNSDateTime();
            bildirimTarihi = new TNSDateTime();
            tesellumTarihi = new TNSDateTime();
            faizinDurdurulmaTarihi = new TNSDateTime();
            defterSirasi = "";
            _silmeVar = null;
            onBorcTarihi = new TNSDateTime();
            ipardAktarimTar = new TNSDateTime();
            islemTarihi = new TNSDateTime();
            onayTarihi = new TNSDateTime();
        }

        public DosyaAB(ABBorclu borcluBilgi) : base()
        {
            this.altTedbir = borcluBilgi.altTedbir;
            this.basvuruKayitNumarasi = borcluBilgi.basvuruKayitNumarasi;
            this.birimKod = borcluBilgi.birimKod;
            this.borcluKod = borcluBilgi.borcluKod;
            this.butceKalemi = borcluBilgi.butceKalemi;
            this.defterSirasi = borcluBilgi.defterSirasi;
            this.dosyaAnahtar = borcluBilgi.dosyaAnahtar;
            this.iPARDDonemi = borcluBilgi.ipardDonemi;
            this.saymanlikKod = borcluBilgi.saymanlikKod;
            this.sozlesmeNumarasi = borcluBilgi.sozlesmeNumarasi;
            this.tedbir = borcluBilgi.tedbir;
        }

        public MiktarIslem ToplamAl(BorcHareket.ENUMHareketTur[] islemTur, DateTime basTar, DateTime BitTar, bool dovizToplam, out string tarihler, TNSDateTime sonTarih, bool islemZamaniSilme, bool dosyaKuru)
        {
            sonTarih.Yaz(DateTime.MinValue);
            tarihler = "";
            double miktarUlusal = 0;
            double miktarAB = 0;
            double faizUlusal = 0;
            double faizAB = 0;
            double ertelemeAB = 0;
            double ertelemeUl = 0;
            double bol;
            if (borcHareket != null)
            {
                foreach (BorcHareket taksit in borcHareket)
                {
                    if (TurListedeVarMi(taksit.tur, islemTur) && TarihIcindeMi(taksit.tarih, basTar, BitTar))
                    {
                        sonTarih.Yaz(taksit.tarihDT);
                        if (tarihler != "") tarihler += ", ";
                        tarihler += taksit.tarih.ToString();
                        if (dovizToplam)
                        {
                            if (dosyaKuru)
                                bol = this.geriAlimDovizKuru;
                            else
                                bol = taksit.dovizKuru;
                            if (bol <= 0) bol = 1;
                        }
                        else
                            bol = 1;

                        if (islemZamaniSilme)
                        {
                            BorcHareket silmeHareket;
                            bool silmeVar = SilmeVarMi(taksit.tarih, out silmeHareket);

                            if (silmeVar)
                            {
                                miktarUlusal = OrtakFonksiyonlar.KurusYap(miktarUlusal + taksit.miktarUlusal / bol);
                                miktarUlusal = OrtakFonksiyonlar.KurusYap(miktarUlusal + taksit.miktarAB / bol);
                                faizUlusal = OrtakFonksiyonlar.KurusYap(faizUlusal + taksit.faizUlusal / bol);
                                faizUlusal = OrtakFonksiyonlar.KurusYap(faizUlusal + taksit.faizAB / bol);
                                ertelemeAB = OrtakFonksiyonlar.KurusYap(ertelemeAB + taksit.ertelemeAB / bol);
                                ertelemeAB = OrtakFonksiyonlar.KurusYap(ertelemeAB + taksit.ertelemeUl / bol);
                            }
                            else
                            {
                                miktarUlusal = OrtakFonksiyonlar.KurusYap(miktarUlusal + taksit.miktarUlusal / bol);
                                miktarAB = OrtakFonksiyonlar.KurusYap(miktarAB + taksit.miktarAB / bol);
                                faizUlusal = OrtakFonksiyonlar.KurusYap(faizUlusal + taksit.faizUlusal / bol);
                                faizAB = OrtakFonksiyonlar.KurusYap(faizAB + taksit.faizAB / bol);
                                ertelemeAB = OrtakFonksiyonlar.KurusYap(ertelemeAB + taksit.ertelemeAB / bol);
                                ertelemeUl = OrtakFonksiyonlar.KurusYap(ertelemeUl + taksit.ertelemeUl / bol);
                            }
                        }
                        else
                        {
                            miktarUlusal = OrtakFonksiyonlar.KurusYap(miktarUlusal + taksit.miktarUlusal / bol);
                            miktarAB = OrtakFonksiyonlar.KurusYap(miktarAB + taksit.miktarAB / bol);
                            faizUlusal = OrtakFonksiyonlar.KurusYap(faizUlusal + taksit.faizUlusal / bol);
                            faizAB = OrtakFonksiyonlar.KurusYap(faizAB + taksit.faizAB / bol);
                            ertelemeAB = OrtakFonksiyonlar.KurusYap(ertelemeAB + taksit.ertelemeAB / bol);
                            ertelemeUl = OrtakFonksiyonlar.KurusYap(ertelemeUl + taksit.ertelemeUl / bol);
                        }
                    }
                }
            }
            double kur = this.geriAlimDovizKuru;
            if (kur <= 0) kur = 1;
            return new MiktarIslem(dovizToplam ? 1 : kur, miktarAB, miktarUlusal, faizAB, faizUlusal, ertelemeAB, ertelemeUl);
        }

        public static bool TurListedeVarMi(BorcHareket.ENUMHareketTur tur, BorcHareket.ENUMHareketTur[] islemTur)
        {
            foreach (BorcHareket.ENUMHareketTur item in islemTur)
            {
                if (item == tur) return true;
            }
            return false;
        }

        public DateTime faizBaslangicTarihi
        {
            get
            {
                if (TNSDateTime.IsNull(tesellumTarihi))
                    return DateTime.MinValue;
                return tesellumTarihiDT.AddMonths(1);
            }
        }

        public BorcHareket HareketBul(int islemNo)
        {
            foreach (BorcHareket item in borcHareket)
            {
                if (item.siraNo == islemNo) return item;
            }
            return null;
        }

        public BorcHareket HareketBul(BorcHareket.ENUMHareketTur tur, DateTime islemTarihDT)
        {
            foreach (BorcHareket item in borcHareket)
            {
                if (item.tur == tur && item.tarihDT == islemTarihDT) return item;
            }
            return null;
        }

        public BorcHareket HareketBulIslemTarih()
        {
            foreach (BorcHareket item in borcHareket)
            {
                if (item.FaizIslemi && item.kayitDurum == DosyaABParca.ENUMKayitDurum.ONAYLI && item.tarih.Oku().Date == islemTarihiDT.Date) return item;
            }
            return null;
        }

        public BorcHareket HareketBul(BorcHareket.ENUMHareketTur tur, DateTime dtBas, DateTime dtSon)
        {
            return HareketBul(new BorcHareket.ENUMHareketTur[] { tur }, dtBas, dtSon);
        }

        public BorcHareket HareketBul(BorcHareket.ENUMHareketTur[] tur, DateTime dtBas, DateTime dtSon)
        {
            foreach (BorcHareket item in borcHareket)
            {
                if (TurListedeVarMi(item.tur, tur) && TarihIcindeMi(BorcHareket.ENUMHareketTur.Taksit == item.tur ? item.duzeltimTarihi : item.tarih, dtBas, dtSon)) return item;
            }
            return null;
        }


        public MiktarIslem HareketToplamBul(BorcHareket.ENUMHareketTur[] tur, DateTime dtBas, DateTime dtSon, bool tlMi)
        {

            MiktarIslem miTop = new MiktarIslem(1, 0, 0, 0, 0, 0, 0);

            foreach (BorcHareket item in borcHareket)
            {
                if (TurListedeVarMi(item.tur, tur) && TarihIcindeMi(BorcHareket.ENUMHareketTur.Taksit == item.tur ? item.duzeltimTarihi : item.tarih, dtBas, dtSon))
                {
                    if (tlMi)
                        miTop.Ekle(item.miktarAB, item.faizAB, item.miktarUlusal, item.faizUlusal);
                    else
                        miTop.Ekle(item.miktarABEUR, item.faizABEUR, item.miktarUlusalEUR, item.faizUlusalEUR);
                }
            }
            return miTop;
        }

        public int OnayliSay(TNSCollection onBorcBilgileri)
        {
            int say = 0;
            foreach (DosyaABParca item in onBorcBilgileri)
            {
                if (item.kayitDurum == DosyaABParca.ENUMKayitDurum.ONAYLI)
                    say++;
            }
            return say;
        }

        public static bool TarihIcindeMi(TNSDateTime tarih, DateTime dtBas, DateTime dtSon)
        {
            if (dtBas == DateTime.MinValue || dtSon == DateTime.MinValue) return true;
            if (!TNSDateTime.IsNull(tarih) && tarih.Oku() >= dtBas && tarih.Oku() < dtSon) return true;
            return false;
        }

        public static bool TaksitVarMi(DateTime taksitTalepTarihi, DateTime tarihDT)
        {
            return taksitTalepTarihi != DateTime.MinValue && tarihDT > taksitTalepTarihi;
        }

        public void FaizHesapla(TNSDateTime hesapTarihi)
        {
            faizMiktarAB = this.kalanFaizMiktarAB;
            faizMiktarUlusal = this.kalanFaizMiktarUlusal;
        }

        public bool FarkliMi(GeriAlimKarari sonKayit)
        {
            return (this.geriAlimMiktarAB != sonKayit.gakMiktarAB || this.geriAlimMiktarUlusal != sonKayit.gakMiktarUlusal);
        }

        //public static double FaizHesapla(DateTime odemeSonGunu, DateTime faizBaslangicTarihi, DateTime hesapTarihi, double anaPara, double faizOran, bool GIBYontem)
        //{
        //    return OrtakFonksiyonlar.KurusYap(anaPara * FaizOranHesapla(odemeSonGunu, faizBaslangicTarihi, hesapTarihi, faizOran, GIBYontem));
        //}

        //public static double FaizOranHesapla(DateTime odemeSonGunu, DateTime faizBaslangicTarihi, DateTime hesapTarihi, FaizDonem[] faizOran, bool gibYontem)
        //{
        //    return FaizOranHesaplaIc(odemeSonGunu, faizBaslangicTarihi, hesapTarihi, faizOran, gibYontem, null);
        //    //if (taksitTalepTarih == DateTime.MinValue || hesapTarihi <= taksitTalepTarih)
        //    //{
        //    //}
        //    //else
        //    //{
        //    //    DateTime oncGun = taksitTalepTarih.AddDays(-1);
        //    //    double fo = FaizOranHesaplaIc(odemeSonGunu, faizBaslangicTarihi, oncGun, faizOran, true);
        //    //    double fo2 = FaizOranHesaplaIc(oncGun, oncGun, hesapTarihi, faizOranAy, false);
        //    //    return fo2 + fo + OrtakFonksiyonlar.Yuvarla(fo * fo2, 6);
        //    //}
        //}

        public static double FaizOranHesapla(DateTime odemeSonTarihiDT, DateTime faizBaslangicTarihi, DateTime islemTarihiDT, DateTime faizDurdurulmaTarihi, bool gecikmeZammi, DateTime taksitTalepTarihDT, DateTime taksitFaizBulmaTarihi, int tacilFaizNo, StringBuilder sonucBilgi, out int ay, out int gun)
        {
            FaizIslemler.FaizDonem[] kullanilacakFaizOranlari;
            if (gecikmeZammi)
                kullanilacakFaizOranlari = FaizIslemler.faizOran;
            else
                kullanilacakFaizOranlari = FaizIslemler.FaizOranAyBul(taksitTalepTarihDT, tacilFaizNo);

            return FaizIslemler.FaizOranHesapla(odemeSonTarihiDT, faizBaslangicTarihi, islemTarihiDT, faizDurdurulmaTarihi, kullanilacakFaizOranlari, gecikmeZammi, sonucBilgi, out ay, out gun);
        }

        public static TNSCollection IPARDAsamaListesiBul(bool bosEkle)
        {
            TNSCollection tc = new TNSCollection();
            if (bosEkle)
                tc.Add(new IntKodStrAd(0, "-"));
            tc.Add(new IntKodStrAd(1, iPARDDonemiAdBul(1))); //
            tc.Add(new IntKodStrAd(2, iPARDDonemiAdBul(2))); //"IPARD 2"
            tc.Add(new IntKodStrAd(3, iPARDDonemiAdBul(3))); //"IPARD 2"
            return tc;
        }

        public static string iPARDDonemiAdBul(int donemNo)
        {
            return "IPARD " + donemNo.ToString();
        }

        /// <summary>
        /// IPARD Dönemi: Borç Dönemi
        /// </summary>
        public int iPARDDonemi { get; set; }
        public string iPARDDonemiAd
        {
            get
            {
                return iPARDDonemiAdBul(iPARDDonemi);

            }
        }

        /// <summary>
        /// Dosya Anahtar: Sistem tarafından üretilen anahtar
        /// </summary>
        public int dosyaAnahtar { get; set; }

        /// <summary>
        /// Defter Sırası: Borçlu Defteri'ne giriş sırası
        /// </summary>
        public string defterSirasi { get; set; }

        public string tertip { get { return tedbir + "-" + altTedbir + "-" + butceKalemi; } }

        /// <summary>
        /// Tedbir: Borçlu Defteri'ne giriş sırası
        /// </summary>
        public string tedbir { get; set; }

        /// <summary>
        /// Alt Tedbir: Borçlu Defteri'ne giriş sırası
        /// </summary>
        public string altTedbir { get; set; }

        /// <summary>
        /// Bütçe Kalemi: Borçlu Defteri'ne giriş sırası
        /// </summary>
        public string butceKalemi { get; set; }

        /// <summary>
        /// Saymanlık Kod: Borçlu takip eden saymanlık
        /// </summary>
        public int saymanlikKod { get; set; }

        /// <summary>
        /// Birim Kod: Borçlu takip eden birim
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// Borçlu Kod: Firma Kodu
        /// </summary>
        public int borcluKod { get; set; }

        /// <summary>
        /// Başvuru Kayıt Numarası: Başvuru Kayıt Numarası
        /// </summary>
        public string basvuruKayitNumarasi { get; set; }

        /// <summary>
        /// Sözleşme Numarası: Sözleşme Numarası
        /// </summary>
        public string sozlesmeNumarasi { get; set; }

        /// <summary>
        /// Borcun Kodu: Borcun Kodu
        /// </summary>
        public string borcunKodu { get; set; }

        public string borcunKoduAd
        {
            get
            {
                return BorcTurleriSinif.BorcTuruAdListeBul(borcTuru, borcunKodu);
            }
        }

        /// <summary>
        /// Vaka Numarası: Vaka Numarası
        /// </summary>
        public string vakaNumarasi { get; set; }
        public bool avans { get; set; } = false;

        /// <summary>
        /// Ön Borç Sayı: Ön Borç Sayı
        /// </summary>
        public string onBorcSayi { get; set; }

        /// <summary>
        /// Geri Alım Sayı: Geri Alım Sayı
        /// </summary>
        public string geriAlimSayi { get; set; }

        /// <summary>
        /// Bildirim Tarihi: Borç Bildirim Mektubunun borçluya gönderildiği tarih
        /// </summary>
        public TNSDateTime bildirimTarihi { get; set; }
        public DateTime bildirimTarihiDT
        {
            get
            {
                if (bildirimTarihi == null)
                    return DateTime.MinValue;
                else
                    return bildirimTarihi.Oku();
            }
            set
            {
                if (bildirimTarihi == null) bildirimTarihi = new TNSDateTime(value);
                else bildirimTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Tesellüm Tarihi: Borç Bildirim Mektubunun Borçlu tarafından alındığı tarih
        /// </summary>
        public TNSDateTime tesellumTarihi { get; set; }
        public DateTime tesellumTarihiDT
        {
            get
            {
                if (tesellumTarihi == null)
                    return DateTime.MinValue;
                else
                    return tesellumTarihi.Oku();
            }
            set
            {
                if (tesellumTarihi == null) tesellumTarihi = new TNSDateTime(value);
                else tesellumTarihi.Yaz(value);
            }
        }

        public DateTime taksitTalepTarihDT
        {
            get
            {
                if (taksitTalepTarih == null)
                    return DateTime.MinValue;
                else
                    return taksitTalepTarih.Oku();
            }
            set
            {
                if (taksitTalepTarih == null) taksitTalepTarih = new TNSDateTime(value);
                else taksitTalepTarih.Yaz(value);
            }
        }
        public TNSDateTime taksitTalepTarih { get; set; }
        public TNSDateTime taksitGirisTarih { get; set; }
        public string taksitTalepSayi { get; set; }
        public DateTime taksitKararTarihDT
        {
            get
            {
                if (taksitKararTarih == null)
                    return DateTime.MinValue;
                else
                    return taksitKararTarih.Oku();
            }
            set
            {
                if (taksitKararTarih == null) taksitKararTarih = new TNSDateTime(value);
                else taksitKararTarih.Yaz(value);
            }
        }
        public TNSDateTime taksitKararTarih { get; set; }
        public string taksitKararSayi { get; set; }

        public TNSDateTime taksitIlkTarih { get; set; }
        public DateTime taksitIlkTarihDT
        {
            get
            {
                if (taksitIlkTarih == null)
                    return DateTime.MinValue;
                else
                    return taksitIlkTarih.Oku();
            }
            set
            {
                if (taksitIlkTarih == null) taksitIlkTarih = new TNSDateTime(value);
                else taksitIlkTarih.Yaz(value);
            }
        }
        public int taksitSayisi { get; set; }

        /// <summary>
        /// Ödeme Son Tarihi: Borcun Ödemesinin Son Tarihi
        /// </summary>
        public TNSDateTime odemeSonTarihi { get; set; }
        public DateTime odemeSonTarihiDT
        {
            get
            {
                if (odemeSonTarihi == null)
                    return DateTime.MinValue;
                else
                    return odemeSonTarihi.Oku();
            }
            set
            {
                if (odemeSonTarihi == null) odemeSonTarihi = new TNSDateTime(value);
                else odemeSonTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Temerrüt Tarihi: Geri alım için temerrüt tarihi
        /// </summary>
        public TNSDateTime temerrutTarihi { get; set; }
        public DateTime temerrutTarihiDT
        {
            get
            {
                if (temerrutTarihi == null)
                    return DateTime.MinValue;
                else
                    return temerrutTarihi.Oku();
            }
            set
            {
                if (temerrutTarihi == null) temerrutTarihi = new TNSDateTime(value);
                else temerrutTarihi.Yaz(value);
            }
        }
        public TNSDateTime faizHesabiSonTarihi
        {
            get
            {
                return new TNSDateTime(faizHesabiSonTarihiDT);
            }
        }
        public DateTime faizHesabiSonTarihiDT
        {
            get
            {
                DateTime arananTarih = DateTime.MinValue;
                if (borcHareket != null)
                {
                    for (int i = borcHareket.Count - 1; i >= 0; i--)
                    {
                        BorcHareket bh = (BorcHareket)borcHareket[i];
                        if ((bh.tur == BorcHareket.ENUMHareketTur.FaizHesabi || bh.tur == BorcHareket.ENUMHareketTur.FaizHesabiIslem) && bh.tarihDT > arananTarih)
                        {
                            arananTarih = bh.tarihDT;
                        }
                    }
                }
                return arananTarih;
            }
        }

        /// <summary>
        /// Yasal Takip Tarihi: Yasal takibe tabi olma (tarih)
        /// </summary>

        public TNSDateTime faizinDurdurulmaTarihi { get; set; }
        public DateTime faizinDurdurulmaTarihiDT
        {
            get
            {
                if (faizinDurdurulmaTarihi == null)
                    return DateTime.MinValue;
                else
                    return faizinDurdurulmaTarihi.Oku();
            }
            set
            {
                if (faizinDurdurulmaTarihi == null) faizinDurdurulmaTarihi = new TNSDateTime(value);
                else faizinDurdurulmaTarihi.Yaz(value);
            }
        }

        public TNSDateTime yasalTakipTarihi { get { return bildirimTarihi; } }
        public DateTime yasalTakipTarihiDT { get { return bildirimTarihiDT; } }

        /// <summary>
        /// Yorumlar TR: Türkçe Yorumlar
        /// </summary>
        public string yorumlarTR { get; set; }

        /// <summary>
        /// Yorumlar EN: İngilizce Yorumlar
        /// </summary>
        public string yorumlarEN { get; set; }
        public string borcluAd { get; set; }
        public string kimlikNo { get; set; }
        public string HrcBirimAd { get { return birimAd; } } //TNS.MUH.Arac.BirimAdOl(saymanlikAd, birimAd); 
        public string borcluKodAd { get { return kimlikNo + "- " + borcluAd; } }
        /// <summary>
        /// BorcHareket: Borç Ödemeleri veya Yapılan Değişilikler
        /// </summary>
        public TNSCollection borcHareket;
        public ENUMDosyaDurum geriAlimSurecAsamasi { get; set; }
        public string geriAlimSurecAsamasiAd { get { return DosyaDurumAdBul(geriAlimSurecAsamasi); } }
        public string geriAlimSurecAsamasiAdEn { get { return DosyaDurumAdBulEn(geriAlimSurecAsamasi); } }

        /// <summary>
        /// Ön Borç Tarihi: Ön Borç Tarihi
        /// </summary>
        public TNSDateTime onBorcTarihi { get; set; }
        public DateTime onBorcTarihiDT
        {
            get
            {
                if (onBorcTarihi == null)
                    return DateTime.MinValue;
                else
                    return onBorcTarihi.Oku();
            }
            set
            {
                if (onBorcTarihi == null) onBorcTarihi = new TNSDateTime(value);
                else onBorcTarihi.Yaz(value);
            }
        }

        #region ONBORC
        private MiktarIslem _onBorcIslem = null;
        public MiktarIslem onBorcIslem
        {
            get
            {
                if (_onBorcIslem == null) _onBorcIslem = new MiktarIslem(onBorcDovizKuru, onBorcMiktarAB, onBorcMiktarUlusal, 0, 0, 0, 0);
                return _onBorcIslem;
            }
        }
        /// <summary>
        /// Ön Borç Döviz Kuru: Ön Borç Döviz Kuru
        /// </summary>
        public double onBorcDovizKuru { get; set; }

        /// <summary>
        /// Ön Borç Miktar AB: Ön Borç Miktarı AB
        /// </summary>
        public double onBorcMiktarAB { get; set; }

        /// <summary>
        /// Ön Borç Miktar Ulusal: Ön Borç Miktar Ulusal
        /// </summary>
        public double onBorcMiktarUlusal { get; set; }

        public double onBorcToplam { get { return onBorcIslem.miktarToplam; } }
        public double onBorcToplamEUR { get { return onBorcIslem.miktarToplamEUR; } }
        public double onBorcMiktarUlusalEUR { get { return onBorcIslem.miktarUlusalEUR; } }
        public double onBorcMiktarABEUR { get { return onBorcIslem.miktarABEUR; } }

        public double onBorcMiktarABKur(bool tl)
        {
            if (tl) return onBorcMiktarAB;
            else return onBorcMiktarABEUR;
        }

        public double onBorcMiktarUlusalKur(bool tl)
        {
            if (tl) return onBorcMiktarUlusal;
            else return onBorcMiktarUlusalEUR;
        }
        #endregion

        /// <summary>
        /// Geri Alım Tarihi: Geri Alım Tarihi
        /// </summary>
        public TNSDateTime geriAlimKarariTarihi { get; set; }
        public TNSDateTime geriAlimKarariOnayTarihi = new TNSDateTime();
        public DateTime geriAlimKarariTarihiDT
        {
            get
            {
                if (geriAlimKarariTarihi == null)
                    return DateTime.MinValue;
                else
                    return geriAlimKarariTarihi.Oku();
            }
            set
            {
                if (geriAlimKarariTarihi == null) geriAlimKarariTarihi = new TNSDateTime(value);
                else geriAlimKarariTarihi.Yaz(value);
            }
        }


        #region GERIALIM
        private MiktarIslem _geriAlimIslem = null;
        public MiktarIslem geriAlimIslem
        {
            get
            {
                if (_geriAlimIslem == null) _geriAlimIslem = new MiktarIslem(geriAlimDovizKuru, geriAlimMiktarAB, geriAlimMiktarUlusal, faizMiktarAB, faizMiktarUlusal, 0, 0);
                return _geriAlimIslem;
            }
        }

        /// <summary>
        /// Geri Alım Döviz Kuru: Geri Alım Döviz Kuru
        /// </summary>
        public double geriAlimDovizKuru { get; set; }

        /// <summary>
        /// Geri Alım Miktar AB: Geri Alım Miktarı AB
        /// </summary>
        public double geriAlimMiktarAB { get; set; }

        public double silinmeyecekMiktar { get; set; }

        public double silinmeyecekMiktarEUR { get; set; }

        /// <summary>
        /// Geri Alım Miktar Ulusal: Geri Alım Miktar Ulusal
        /// </summary>
        public double geriAlimMiktarUlusal { get; set; }

        /// <summary>
        /// Faiz Miktar AB: Faiz AB
        /// </summary>
        public double faizMiktarAB { get; set; }

        /// <summary>
        /// Faiz Miktar Ulusal: Faiz Ulusal
        /// </summary>
        public double faizMiktarUlusal { get; set; }

        public double geriAlimMiktarToplam { get { return geriAlimIslem.miktarToplam; } }
        public double faizMiktarToplam { get { return geriAlimIslem.faizToplam; } }

        public double geriAlimToplamAB { get { return geriAlimIslem.toplamAB; } }
        public double geriAlimToplamUlusal { get { return geriAlimIslem.toplamUlusal; } }
        public double geriAlimToplamToplam { get { return geriAlimIslem.toplamToplam; } }

        public double geriAlimMiktarABEUR { get { return geriAlimIslem.miktarABEUR; } }
        public double silinmeyecekMiktarABEUR { get { return geriAlimIslem.silinmeyecekMiktarEUR; } }
        public double geriAlimMiktarUlusalEUR { get { return geriAlimIslem.miktarUlusalEUR; } }
        public double geriAlimMiktarToplamEUR { get { return geriAlimIslem.miktarToplamEUR; } }

        public double faizMiktarABEUR { get { return geriAlimIslem.faizABEUR; } }
        public double faizMiktarUlusalEUR { get { return geriAlimIslem.faizUlusalEUR; } }
        public double faizMiktarToplamEUR { get { return geriAlimIslem.faizToplamEUR; } }

        public double geriAlimToplamABEUR { get { return geriAlimIslem.toplamABEUR; } }
        public double geriAlimToplamUlusalEUR { get { return geriAlimIslem.toplamUlusalEUR; } }
        public double geriAlimToplamToplamEUR { get { return geriAlimIslem.toplamToplamEUR; } }

        public double geriAlimMiktarToplamKur(bool tl)
        {
            if (tl) return geriAlimMiktarToplam;
            else return geriAlimMiktarToplamEUR;
        }

        public double geriAlimMiktarABKur(bool tl)
        {
            if (tl) return geriAlimMiktarAB;
            else return geriAlimMiktarABEUR;
        }

        public double geriAlimMiktarUlusalKur(bool tl)
        {
            if (tl) return geriAlimMiktarUlusal;
            else return geriAlimMiktarUlusalEUR;
        }

        public double faizMiktarToplamKur(bool tl)
        {
            if (tl) return faizMiktarToplam;
            else return faizMiktarToplamEUR;
        }

        public double faizMiktarABKur(bool tl)
        {
            if (tl) return faizMiktarAB;
            else return faizMiktarABEUR;
        }

        public double faizMiktarUlusalKur(bool tl)
        {
            if (tl) return faizMiktarUlusal;
            else return faizMiktarUlusalEUR;
        }
        #endregion

        #region ISLEMYAPILAN
        private MiktarIslem _islemYapilanIslem = null;
        public MiktarIslem islemYapilanIslem
        {
            get
            {
                if (_islemYapilanIslem == null)
                {
                    IslemToplamBul(null, -1, true, null, null, -1);
                }
                return _islemYapilanIslem;
            }
        }

        public double islemYapilanMiktarAB { get { return islemYapilanIslem.miktarAB; } }
        public double islemYapilanMiktarUlusal { get { return islemYapilanIslem.miktarUlusal; } }
        public double islemYapilanMiktarToplam { get { return islemYapilanIslem.miktarToplam; } }

        public double islemYapilanFaizMiktarAB { get { return islemYapilanIslem.faizAB; } }
        public double islemYapilanFaizMiktarUlusal { get { return islemYapilanIslem.faizUlusal; } }
        public double islemYapilanFaizMiktarToplam { get { return islemYapilanIslem.faizToplam; } }

        public double islemYapilanToplamAB { get { return islemYapilanIslem.toplamAB; } }
        public double islemYapilanToplamUlusal { get { return islemYapilanIslem.toplamUlusal; } }
        public double islemYapilanToplamToplam { get { return islemYapilanIslem.toplamToplam; } }

        public double islemYapilanMiktarABEUR { get { return islemYapilanIslem.miktarABEUR; } }
        public double islemYapilanMiktarUlusalEUR { get { return islemYapilanIslem.miktarUlusalEUR; } }
        public double islemYapilanMiktarToplamEUR { get { return islemYapilanIslem.miktarToplamEUR; } }

        public double islemYapilanFaizMiktarABEUR { get { return islemYapilanIslem.faizABEUR; } }
        public double islemYapilanFaizMiktarUlusalEUR { get { return islemYapilanIslem.faizUlusalEUR; } }
        public double islemYapilanFaizMiktarToplamEUR { get { return islemYapilanIslem.faizToplamEUR; } }

        public double islemYapilanToplamABEUR { get { return islemYapilanIslem.toplamABEUR; } }
        public double islemYapilanToplamUlusalEUR { get { return islemYapilanIslem.toplamUlusalEUR; } }
        public double islemYapilanToplamToplamEUR { get { return islemYapilanIslem.toplamToplamEUR; } }
        #endregion

        #region KALANISLEM

        public double oran1, oran2;
        public int ay1, gun1, ay2, gun2;
        private MiktarIslem _kalanIslem = null;
        public StringBuilder debugHesap;
        public MiktarIslem kalanIslem
        {
            get
            {
                if (_kalanIslem == null)
                {
                    double ab = 0;
                    double abf = 0;
                    double ul = 0;
                    double ulf = 0;
                    double ertab = 0;
                    double ertul = 0;
                    double gamAB = geriAlimMiktarAB;
                    double gamUL = geriAlimMiktarUlusal;
                    debugHesap = new StringBuilder();
                    debugHesap.AppendLine($"geriAlimMiktarAB: {geriAlimMiktarAB} geriAlimMiktarUlusal:{geriAlimMiktarUlusal}");
                    //OrtakFonksiyonlar.HataDosyaYaz("Hesap.txt", "Başla", true);
                    if (!TNSDateTime.IsNull(islemTarihi) && !TNSDateTime.IsNull(geriAlimKarariTarihi) && islemTarihi.Oku() < geriAlimKarariTarihi.Oku())
                    {
                        gamAB = 0;
                        gamUL = 0;
                    }
                    ab = OrtakFonksiyonlar.KurusYap(gamAB - islemYapilanMiktarAB);
                    ul = OrtakFonksiyonlar.KurusYap(gamUL - islemYapilanMiktarUlusal);
                    debugHesap.AppendLine($"Ana Para AB: {ab} UL:{ul}");
                    if (!taksitVar)
                    {
                        DateTime tutDT = islemTarihiDT;
                        DateTime tutSon;
                        if (taksitTalepTarihDT != DateTime.MinValue && islemTarihiDT == taksitTalepTarihDT && oncekiGunHesapla)
                        {
                            tutDT = tutDT.AddDays(-1);
                            tutSon = faizBaslangicTarihi;
                        }
                        else
                            tutSon = odemeSonTarihiDT;
                        oran1 = FaizOranHesapla(tutSon, faizBaslangicTarihi, tutDT, this.faizinDurdurulmaTarihiDT, true, this.taksitTalepTarihDT, this.taksitGirisTarih.Oku(), this.tecilFaizNo, debugHesap, out ay1, out gun1);

                        debugHesap.AppendLine($"Ödeme Son Tarihi: {tutSon.ToString()} faizBaslangicTarihi:{faizBaslangicTarihi.ToString()} işlem Tarihi: {tutDT.ToString()}: oran:{oran1}");
                        ay2 = 0;
                        gun2 = 0;
                        abf = OrtakFonksiyonlar.KurusYap(ab * oran1);
                        ulf = OrtakFonksiyonlar.KurusYap(ul * oran1);
                        debugHesap.AppendLine($"Hesaplanan Faiz AB: {abf} UL:{ulf}");
                    }
                    else
                    {
                        DateTime tutDT = taksitTalepTarihDT.AddDays(-1);

                        oran1 = FaizOranHesapla(faizBaslangicTarihi, faizBaslangicTarihi, tutDT, this.faizinDurdurulmaTarihiDT, true, this.taksitTalepTarihDT, this.taksitGirisTarih.Oku(), this.tecilFaizNo, debugHesap, out ay1, out gun1);
                        debugHesap.AppendLine($"Ödeme Son Tarihi: {faizBaslangicTarihi.ToString()} faizBaslangicTarihi:{faizBaslangicTarihi.ToString()} işlem Tarihi: {tutDT.ToString()}: oran:{oran1}");
                        ertab = OrtakFonksiyonlar.KurusYap(ab * oran1);
                        ertul = OrtakFonksiyonlar.KurusYap(ul * oran1);
                        debugHesap.AppendLine($"Hesaplanan Ert Faiz AB: {ertab} UL:{ertul}");

                        oran2 = FaizOranHesapla(faizBaslangicTarihi, tutDT, islemTarihiDT, this.faizinDurdurulmaTarihiDT, false, this.taksitTalepTarihDT, this.taksitGirisTarih.Oku(), this.tecilFaizNo, debugHesap, out ay2, out gun2);
                        debugHesap.AppendLine($"Ödeme Son Tarihi: {faizBaslangicTarihi.ToString()} faizBaslangicTarihi:{tutDT.ToString()} işlem Tarihi: {islemTarihiDT.ToString()}: oran:{oran2}");
                        abf = ertab + OrtakFonksiyonlar.KurusYap((ab + ertab) * oran2);
                        ulf = ertul + OrtakFonksiyonlar.KurusYap((ul + ertul) * oran2);
                        debugHesap.AppendLine($"Toplam Faiz AB: {abf} UL:{ulf}");
                    }
                    _kalanIslem = new MiktarIslem(geriAlimDovizKuru, ab, ul, abf, ulf, ertab, ertul);
                }
                return _kalanIslem;
            }
        }

        public double kalanMiktarAB { get { return kalanIslem.miktarAB; } }

        public double kalanMiktarABKur(bool tl)
        {
            if (tl) return kalanMiktarAB;
            else return kalanMiktarABEUR;
        }

        public double kalanMiktarUlusal { get { return kalanIslem.miktarUlusal; } }

        public double kalanMiktarUlusalKur(bool tl)
        {
            if (tl) return kalanMiktarUlusal;
            else return kalanMiktarUlusalEUR;
        }

        public double kalanMiktarToplam { get { return kalanIslem.miktarToplam; } }

        public double kalanMiktarToplamKur(bool tl)
        {
            if (tl) return kalanMiktarToplam;
            else return kalanMiktarToplamEUR;
        }

        public double kalanFaizMiktarToplamKur(bool tl)
        {
            if (tl) return kalanFaizMiktarToplam;
            else return kalanFaizMiktarToplamEUR;
        }
        public double kalanFaizMiktarAB { get { return kalanIslem.faizAB; } }

        public double kalanFaizMiktarABKur(bool tl)
        {
            if (tl) return kalanFaizMiktarAB;
            else return kalanFaizMiktarABEUR;
        }

        public double kalanFaizMiktarUlusal { get { return kalanIslem.faizUlusal; } }

        public double kalanFaizMiktarUlusalKur(bool tl)
        {
            if (tl) return kalanFaizMiktarUlusal;
            else return kalanFaizMiktarUlusalEUR;
        }

        public double kalanFaizMiktarToplam { get { return kalanIslem.faizToplam; } }

        public double kalanToplamAB { get { return kalanIslem.toplamAB; } }
        public double kalanToplamABSil
        {
            get { if (silmeVar) return 0; else return kalanToplamAB; }
        }

        public double kalanToplamABKur(bool tl)
        {
            if (tl) return kalanToplamAB;
            else return kalanToplamABEUR;
        }

        public double kalanToplamUlusal { get { return kalanIslem.toplamUlusal; } }
        public double kalanToplamUlusalSil { get { if (silmeVar) return kalanToplamUlusal + kalanToplamAB; else return kalanToplamUlusal; } }

        public double kalanToplamToplam { get { return kalanIslem.toplamToplam; } }
        public double kalanErtelemeToplam { get { return kalanIslem.kalanErtelemeToplam; } }
        public double kalanToplamToplamKur(bool tl)
        {
            if (tl) return kalanToplamToplam;
            else return kalanToplamToplamEUR;
        }

        public double kalanMiktarABEUR { get { return kalanIslem.miktarABEUR; } }
        public double kalanMiktarUlusalEUR { get { return kalanIslem.miktarUlusalEUR; } }
        public double kalanMiktarToplamEUR { get { return kalanIslem.miktarToplamEUR; } }

        public double kalanFaizMiktarABEUR { get { return kalanIslem.faizABEUR; } }
        public double kalanFaizMiktarUlusalEUR { get { return kalanIslem.faizUlusalEUR; } }
        public double kalanFaizMiktarToplamEUR { get { return kalanIslem.faizToplamEUR; } }

        public double kalanToplamABEUR { get { return kalanIslem.toplamABEUR; } }

        public double kalanToplamABEURSil
        {
            get { if (silmeVar) return 0; else return kalanToplamABEUR; }
        }

        public double kalanToplamUlusalEUR { get { return kalanIslem.toplamUlusalEUR; } }
        public double kalanToplamUlusalEURSil
        {
            get { if (silmeVar) return kalanToplamUlusalEUR + kalanToplamABEUR; else return kalanToplamUlusalEUR; }
        }

        public double kalanToplamToplamEUR { get { return kalanIslem.toplamToplamEUR; } }

        public double kalanMiktarABSil
        {
            get { if (silmeVar) return 0; else return kalanMiktarAB; }
        }
        public double kalanMiktarABEURSil
        {
            get { if (silmeVar) return 0; else return kalanMiktarABEUR; }
        }
        public double kalanMiktarABKurSil(bool tl)
        {
            if (tl) return kalanMiktarABSil;
            else return kalanMiktarABEURSil;
        }

        public double kalanFaizMiktarABSil
        {
            get { if (silmeVar) return 0; else return kalanFaizMiktarAB; }
        }
        public double kalanFaizMiktarABEURSil
        {
            get { if (silmeVar) return 0; else return kalanFaizMiktarABEUR; }
        }

        public double kalanFaizMiktarABKurSil(bool tl)
        {
            if (tl) return kalanFaizMiktarABSil;
            else return kalanFaizMiktarABEURSil;
        }

        public double kalanMiktarUlusalSil
        {
            get { if (silmeVar) return kalanMiktarUlusal + kalanMiktarAB; else return kalanMiktarUlusal; }
        }
        public double kalanMiktarUlusalEURSil
        {
            get { if (silmeVar) return kalanMiktarUlusalEUR + kalanMiktarABEUR; else return kalanMiktarUlusalEUR; }
        }

        public double kalanMiktarUlusalKurSil(bool tl)
        {
            if (tl) return kalanMiktarUlusalSil;
            else return kalanMiktarUlusalEURSil;
        }

        public double kalanFaizMiktarUlusalSil
        {
            get { if (silmeVar) return kalanFaizMiktarAB + kalanFaizMiktarUlusal; else return kalanFaizMiktarUlusal; }
        }
        public double kalanFaizMiktarUlusalEURSil
        {
            get { if (silmeVar) return kalanFaizMiktarABEUR + kalanFaizMiktarUlusalEUR; else return kalanFaizMiktarUlusalEUR; }
        }
        public double kalanFaizMiktarUlusalKurSil(bool tl)
        {
            if (tl) return kalanFaizMiktarUlusalSil;
            else return kalanFaizMiktarUlusalEURSil;
        }

        public double kalanErtelemeAB { get { return kalanIslem.ertelemeAB; } }
        public double kalanErtelemeUlusal { get { return kalanIslem.ertelemeUlusal; } }

        #endregion

        public string geriAlimSurecAsamalari;
        public string borcTurleri;

        public DateTime ongorulenSilinmeTarihiDT
        {
            get { return OnGorulenSilinmeTarihiBul(geriAlimKarariTarihiDT); }
        }

        public static DateTime OnGorulenSilinmeTarihiBul(DateTime gatDt)
        {
            if (gatDt == DateTime.MinValue) return DateTime.MinValue;
            return gatDt.AddYears(2);
        }

        public string borcTuruAd
        {
            get
            {
                return BorcTuruAd(borcTuru);
            }
        }
        public string saymanlikAd { get; set; }
        public string birimAd { get; set; }

        private bool islemGunuDahil = true;
        public int borcDurumu = 0;
        public BorcTurleri borcTuru;
        public int donemYil { get { if (TNSDateTime.IsNull(geriAlimKarariTarihi)) return 0; return geriAlimKarariTarihi.Yil; } }
        public int donemCeyrek { get { if (TNSDateTime.IsNull(geriAlimKarariTarihi)) return 0; return CeyrekBul(geriAlimKarariTarihi.Ay); } }

        private int CeyrekBul(int ay)
        {
            if (ay <= 3) return 1;
            if (ay <= 6) return 2;
            if (ay <= 9) return 3;
            return 4;
        }

        [NonSerialized()]
        private bool? _silmeVar = null;
        public string islemTarihAralik;
        public string islemliste;
        public string geriAlimKarariTarihAralik;
        public string onBorcTarihAralik;
        public bool oncekiGunHesapla = false;
        public string aramaMetni;

        public TNSDateTime islemTarihi { get; set; }
        public TNSDateTime onayTarihi { get; set; }
        public DateTime islemTarihiDT
        {
            get
            {
                if (TNSDateTime.IsNull(islemTarihi)) islemTarihi.Yaz(DateTime.Now);
                return islemTarihi.Oku();
            }
        }

        public string tedbirAdEn { get { return TedbirAdEn(tedbir); } }

        public static string TedbirAdEn(string tedbir)
        {
            if (tedbir == "101") return "Investments in agricultural holdings";
            else if (tedbir == "102") return "Support for the setting-up of producer groups";
            else if (tedbir == "103") return "Investments in the processing and marketing of agriculture and fishery products";
            else if (tedbir == "201") return "Actions to improve environment and countryside";
            else if (tedbir == "202") return "Preparation and implementation of local rural development strategies";
            else if (tedbir == "302") return "Diversification and development of rural economic activities";
            else if (tedbir == "501") return "Technical Assistance";

            return "";
        }
        public bool silmeVar
        {
            get
            {
                if (_silmeVar == null)
                {
                    BorcHareket bh;
                    _silmeVar = SilmeVarMi(islemTarihi, out bh);
                }
                return _silmeVar.Value;
            }
        }
        double ertelemeBaslangicAB = 0;
        double ertelemeBaslangicUl = 0;
        bool taksitVar = false;

        public double ertelemeOranAB { get { if (kalanIslem.miktarAB > 0) return kalanIslem.ertelemeAB / kalanIslem.miktarAB; else return 0; } }
        public double ertelemeOranUlusal { get { if (kalanIslem.miktarUlusal > 0) return kalanIslem.ertelemeUlusal / kalanIslem.miktarUlusal; else return 0; } }

        public bool VerilenAraliktaIslemVarMi(TNSDateTime islemTarih, TNSDateTime sonTarih)
        {
            foreach (BorcHareket item in this.borcHareket)
            {
                //if (item.tur == BorcHareket.ENUMHareketTur.Silme || item.tur == BorcHareket.ENUMHareketTur.Duzeltme || item.OdemeTuruIslem)
                {
                    if (item.tarihDT >= islemTarih.Oku() && item.tarihDT <= sonTarih.Oku())
                        return true;
                }
            }
            return false;
        }

        public DosyaABParca KayitBul(TNSCollection liste, bool yeniDahil, TNSDateTime onayTarihi)
        {
            DosyaABParca yeniKayit = null;
            foreach (DosyaABParca item in liste)
            {
                if (item.kayitDurum == DosyaABParca.ENUMKayitDurum.ONAYLI)
                {
                    if (TNSDateTime.IsNull(onayTarihi) || item.onayTarihiDT <= onayTarihi.Oku())
                    {
                        if (TNSDateTime.IsNull(onayTarihi) ||
                            (TNS.ResmiTatil.TarihIcindeMi(onayTarihi.Oku(), item.basTarih.Oku(), item.bitTarih.Oku())))
                            return item;
                    }
                }
                else if (item.kayitDurum == DosyaABParca.ENUMKayitDurum.YENI && yeniDahil)
                    yeniKayit = item;
            }
            return yeniKayit;
        }

        private void DegerAtaABBorclu(ABBorclu sonKayit)
        {
            if (sonKayit == null) sonKayit = new ABBorclu();
            this.defterSirasi = sonKayit.defterSirasi;
            this.tedbir = sonKayit.tedbir;
            this.altTedbir = sonKayit.altTedbir;
            this.butceKalemi = sonKayit.butceKalemi;
            this.saymanlikKod = sonKayit.saymanlikKod;
            this.birimKod = sonKayit.birimKod;
            this.borcluKod = sonKayit.borcluKod;
            this.basvuruKayitNumarasi = sonKayit.basvuruKayitNumarasi;
            this.sozlesmeNumarasi = sonKayit.sozlesmeNumarasi;
            this.kimlikNo = sonKayit.kimlikNo;
            this.borcluAd = sonKayit.borcluAd;
            this.saymanlikAd = sonKayit.saymanlikAd;
            this.birimAd = sonKayit.birimAd;
        }

        private void DegerAtaDurum(DosyaABDurum sonKayit)
        {
            if (sonKayit == null) sonKayit = new DosyaABDurum();
            this.geriAlimSurecAsamasi = sonKayit.surecAsamasi;
            this.yorumlarTR = sonKayit.yorumlarTR;
            this.yorumlarEN = sonKayit.yorumlarEN;
        }

        private void DegerAtaOnBorc(OnBorc sonKayit)
        {
            if (sonKayit == null) sonKayit = new OnBorc();
            this.borcTuru = sonKayit.borcTuru;
            this.borcunKodu = sonKayit.borcunKodu;
            this.onBorcSayi = sonKayit.onBorcSayi;
            this.vakaNumarasi = sonKayit.vakaNumarasi;
            this.avans = sonKayit.avans;
            this.onBorcMiktarAB = sonKayit.onBorcMiktarAB;
            this.onBorcMiktarUlusal = sonKayit.onBorcMiktarUlusal;
            this.onBorcDovizKuru = sonKayit.onBorcDovizKuru;
            this.onBorcTarihi.Yaz(sonKayit.onBorcTarihi.Oku());
            this.ipardAktarimTar.Yaz(sonKayit.aktarimTarihi.Oku());
        }

        private void DegerAtaGAK(GeriAlimKarari sonKayit)
        {
            if (sonKayit == null) sonKayit = new GeriAlimKarari();
            this.geriAlimSayi = sonKayit.geriAlimSayi;
            this.geriAlimMiktarAB = sonKayit.gakMiktarAB;
            this.silinmeyecekMiktar = sonKayit.silinmeyecekMiktar;
            this.geriAlimMiktarUlusal = sonKayit.gakMiktarUlusal;
            this.geriAlimDovizKuru = sonKayit.gakDovizKuru;
            this.faizinDurdurulmaTarihi.Yaz(sonKayit.faizinDurdurulmaTarihi.Oku());
            this.geriAlimKarariTarihi.Yaz(sonKayit.geriAlimKarariTarihi.Oku());
            this.geriAlimKarariOnayTarihi.Yaz(sonKayit.onayTarihiDT);
            this.bildirimTarihi.Yaz(sonKayit.bildirimTarihi.Oku());
            this.tesellumTarihi.Yaz(sonKayit.tesellumTarihi.Oku());
            this.odemeSonTarihi.Yaz(sonKayit.odemeSonTarihi.Oku());
            this.temerrutTarihi.Yaz(sonKayit.temerrutTarihi.Oku());
            //dosya.faizMiktarAB = sonKayit.faizMiktarAB;
            //dosya.faizMiktarUlusal = sonKayit.faizMiktarUlusal;
        }

        private void DegerAtaTaksit(DosyaABTaksit sonKayit)
        {
            if (sonKayit == null) sonKayit = new DosyaABTaksit();
            this.taksitTalepSayi = sonKayit.taksitTalepSayi;
            this.taksitKararSayi = sonKayit.taksitKararSayi;
            this.taksitSayisi = sonKayit.taksitSayisi;
            this.taksitGirisTarih.Yaz(sonKayit.girisTarihi.Oku());
            this.taksitTalepTarih.Yaz(sonKayit.taksitTalepTarihi.Oku());
            this.taksitKararTarih.Yaz(sonKayit.taksitKararTarihi.Oku());
            this.taksitIlkTarih.Yaz(sonKayit.taksitIlkTarih.Oku());
            this.tecilFaizNo = sonKayit.tecilFaizNo;
        }

        /// <summary>
        /// Faiz Hesabı Son Tarihi: Faiz hesaplama için son tarih
        /// </summary>
        public void IslemToplamBul(TNSDateTime tarih, int islemYerNo, bool pIslemGunuDahil, DosyaABTaksit dosyaTaksit, TNSDateTime onayTarihi, int atlaSiraNo, bool taksitKararTarihKullan = false)
        {
            if (dosyaTaksit == null)
            {
                DegerAtaTaksit((DosyaABTaksit)KayitBul(this.TaksitBilgileri, false, onayTarihi));
                if (tarih != null)
                    islemTarihi.Yaz(tarih.Oku());
            }
            else
            {
                onayTarihi.Yaz(dosyaTaksit.onayTarihiDT);
                DegerAtaTaksit(dosyaTaksit);
                islemTarihi.Yaz(dosyaTaksit.taksitTalepTarihi);
            }

            DegerAtaABBorclu((ABBorclu)KayitBul(this.borcuBilgileri, true, onayTarihi));
            DegerAtaDurum((DosyaABDurum)KayitBul(this.durumBilgileri, true, onayTarihi));
            DegerAtaOnBorc((OnBorc)KayitBul(this.OnBorcBilgileri, false, onayTarihi));
            DegerAtaGAK((GeriAlimKarari)KayitBul(this.GAKBilgileri, false, onayTarihi));

            double _islemYapilanMiktarAB = 0;
            double _islemYapilanMiktarUlusal = 0;
            double _islemYapilanFaizMiktarAB = 0;
            double _islemYapilanFaizMiktarUlusal = 0;
            double _islemYapilanertelemeAB = 0;
            double _islemYapilanertelemeUl = 0;
            bool _silmeVarIc = false;

            double _islemYapilanMiktarABTak = 0;
            double _islemYapilanMiktarUlusalTak = 0;
            double _islemYapilanFaizMiktarABTak = 0;
            double _islemYapilanFaizMiktarUlusalTak = 0;
            double _islemYapilanertelemeABTak = 0;
            double _islemYapilanertelemeUlTak = 0;
            bool _silmeVarIcTak = false;

            taksitVar = TaksitVarMi(taksitTalepTarihDT, islemTarihiDT);

            islemGunuDahil = pIslemGunuDahil;
            TNSDateTime araTarih = new TNSDateTime();
            if (taksitKararTarihKullan)
            {
                araTarih.Yaz(this.taksitKararTarihDT);
            }
            else
            {
                if (tarih != null)
                    araTarih.Yaz(tarih.Oku());
                else
                    araTarih.Yaz(islemTarihiDT);
            }
            foreach (BorcHareket item in this.borcHareket)
            {
                if (item.siraNo == atlaSiraNo) continue;
                if (item.tur == BorcHareket.ENUMHareketTur.Silme || item.tur == BorcHareket.ENUMHareketTur.Duzeltme || item.OdemeTuruIslem)
                {
                    ItemTutarEkle(item, islemYerNo, islemGunuDahil, araTarih, ref _islemYapilanMiktarAB, ref _islemYapilanMiktarUlusal, ref _islemYapilanFaizMiktarAB, ref _islemYapilanFaizMiktarUlusal, ref _islemYapilanertelemeAB, ref _islemYapilanertelemeUl, ref _silmeVarIc, onayTarihi);
                    if (taksitVar && item.tarihDT >= this.taksitKararTarihDT)
                        ItemTutarEkle(item, -1, true, taksitKararTarih, ref _islemYapilanMiktarABTak, ref _islemYapilanMiktarUlusalTak, ref _islemYapilanFaizMiktarABTak, ref _islemYapilanFaizMiktarUlusalTak, ref _islemYapilanertelemeABTak, ref _islemYapilanertelemeUlTak, ref _silmeVarIcTak, onayTarihi);
                }
            }
            _silmeVar = _silmeVarIc;
            
            if (taksitVar)
            {
                int a1, g1, a2, g2;
                double o1, o2;
                ertelemeBaslangicAB = FaizIslemler.FaizHesapla(faizBaslangicTarihi, faizBaslangicTarihi, taksitTalepTarihDT, this.taksitGirisTarih.Oku(), taksitTalepTarihDT.AddDays(-1), this.faizinDurdurulmaTarihiDT, geriAlimMiktarAB - _islemYapilanMiktarABTak, true, faizinDurdurulmaTarihiDT, this.tecilFaizNo, null, out o1, out o2, out a1, out g1, out a2, out g2);
                ertelemeBaslangicUl = FaizIslemler.FaizHesapla(faizBaslangicTarihi, faizBaslangicTarihi, taksitTalepTarihDT, this.taksitGirisTarih.Oku(), taksitTalepTarihDT.AddDays(-1), this.faizinDurdurulmaTarihiDT, geriAlimMiktarUlusal - _islemYapilanMiktarUlusalTak, true, faizinDurdurulmaTarihiDT, this.tecilFaizNo, null, out o1, out o2, out a1, out g1, out a2, out g2);
            }
            else
            {
                ertelemeBaslangicAB = 0;
                ertelemeBaslangicUl = 0;
            }
            _islemYapilanIslem = new MiktarIslem(geriAlimDovizKuru, _islemYapilanMiktarAB, _islemYapilanMiktarUlusal, _islemYapilanFaizMiktarAB, _islemYapilanFaizMiktarUlusal, _islemYapilanertelemeAB, _islemYapilanertelemeUl);
            _kalanIslem = null;
        }

        private static void ItemTutarEkle(BorcHareket item, int islemYerNo, bool islemGunuDahil, TNSDateTime tarih, ref double islemYapilanMiktarAB, ref double islemYapilanMiktarUlusal, ref double islemYapilanFaizMiktarAB, ref double islemYapilanFaizMiktarUlusal, ref double islemYapilanertelemeAB, ref double islemYapilanertelemeUl, ref bool silmeVar, TNSDateTime onayTarihi)
        {
            if (item.kayitDurum == DosyaABParca.ENUMKayitDurum.ONAYLI &&
                (TNSDateTime.IsNull(item.onayTarihi) || TNSDateTime.IsNull(onayTarihi) || item.onayTarihi.Oku() <= onayTarihi.Oku())
               )
            {
                if (TarihDahilMi(item.tarih, tarih, item.yerNo, islemYerNo, islemGunuDahil))
                {
                    if (item.tur == BorcHareket.ENUMHareketTur.Silme)
                    {
                        silmeVar = true;
                    }
                    else
                    {
                        int carp;
                        if (item.tur == BorcHareket.ENUMHareketTur.Duzeltme) carp = -1;
                        else carp = 1;
                        islemYapilanMiktarAB = OrtakFonksiyonlar.KurusYap(islemYapilanMiktarAB + (carp * item.miktarAB));
                        islemYapilanMiktarUlusal = OrtakFonksiyonlar.KurusYap(islemYapilanMiktarUlusal + (carp * item.miktarUlusal));
                        islemYapilanFaizMiktarAB = OrtakFonksiyonlar.KurusYap(islemYapilanFaizMiktarAB + (carp * item.faizAB));
                        islemYapilanFaizMiktarUlusal = OrtakFonksiyonlar.KurusYap(islemYapilanFaizMiktarUlusal + (carp * item.faizUlusal));
                        islemYapilanertelemeAB = OrtakFonksiyonlar.KurusYap(islemYapilanertelemeAB + (carp * item.ertelemeAB));
                        islemYapilanertelemeUl = OrtakFonksiyonlar.KurusYap(islemYapilanertelemeUl + (carp * item.ertelemeUl));
                    }
                }
            }
        }

        private static bool TarihDahilMi(TNSDateTime tarihKucuk, TNSDateTime tarihBuyuk, int item_siraNo, int islemSinir, bool islemGunuDahil)
        {
            if (TNSDateTime.IsNull(tarihKucuk) || TNSDateTime.IsNull(tarihBuyuk)) return true;
            if (tarihKucuk.Oku() < tarihBuyuk.Oku()) return true;
            if (islemGunuDahil && tarihKucuk.Oku() == tarihBuyuk.Oku())
            {
                if (islemSinir < 0 || item_siraNo < islemSinir) return true;
            }
            return false;
        }

        public static void MebugYaz(params string[] v1)
        {
            string yaz = string.Format("{0}:{1}:{2}", DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            foreach (var item in v1)
            {
                yaz += ", " + item;
            }
            OrtakFonksiyonlar.HataDosyaYaz("DosyaAB.txt", yaz, false);
        }

        public void ToplamlariSifirla()
        {
            _geriAlimIslem = null;
            _kalanIslem = null;
            _islemYapilanIslem = null;
            _onBorcIslem = null;
        }

        public double MiktarBul(int ulAbT, int ana_Faiz_Top, bool tl)
        {
            if (ulAbT == 1)
            {
                if (ana_Faiz_Top == 1)
                    return geriAlimMiktarUlusalKur(tl);
                else if (ana_Faiz_Top == 2)
                    return faizMiktarUlusalKur(tl);
                else
                    return geriAlimMiktarUlusalKur(tl) + faizMiktarUlusalKur(tl);
            }
            else if (ulAbT == 2)
            {
                if (ana_Faiz_Top == 1)
                    return geriAlimMiktarABKur(tl);
                else if (ana_Faiz_Top == 2)
                    return faizMiktarABKur(tl);
                else
                    return geriAlimMiktarABKur(tl) + faizMiktarABKur(tl);
            }
            else
            {
                if (ana_Faiz_Top == 1)
                    return geriAlimMiktarToplamKur(tl);
                else if (ana_Faiz_Top == 2)
                    return faizMiktarToplamKur(tl);
                else
                    return geriAlimMiktarToplamKur(tl) + faizMiktarToplamKur(tl);
            }
        }

        public double KalanBul(int ulAbT, int ana_Faiz_Top, bool tl)
        {
            if (ulAbT == 1)
            {
                if (ana_Faiz_Top == 1)
                    return kalanMiktarUlusalKurSil(tl);
                else if (ana_Faiz_Top == 2)
                    return kalanFaizMiktarUlusalKurSil(tl);
                else
                    return kalanMiktarUlusalKurSil(tl) + kalanFaizMiktarUlusalKurSil(tl);
            }
            else if (ulAbT == 2)
            {
                if (ana_Faiz_Top == 1)
                    return kalanMiktarABKurSil(tl);
                else if (ana_Faiz_Top == 2)
                    return kalanFaizMiktarABKurSil(tl);
                else
                    return kalanMiktarABKurSil(tl) + kalanFaizMiktarABKurSil(tl);
            }
            else
            {
                if (ana_Faiz_Top == 1)
                    return kalanMiktarToplamKur(tl);
                else if (ana_Faiz_Top == 2)
                    return kalanFaizMiktarToplamKur(tl);
                else
                    return kalanMiktarToplamKur(tl) + kalanFaizMiktarToplamKur(tl);
            }
        }

        public double OnBorcBul(int ulAbT, int ana_Faiz_Top, bool tl)
        {
            if (ulAbT == 1)
            {
                if (ana_Faiz_Top == 1)
                    return onBorcMiktarUlusalKur(tl);
                else if (ana_Faiz_Top == 2)
                    return 0;
                else
                    return onBorcMiktarUlusalKur(tl) + 0;
            }
            else if (ulAbT == 2)
            {
                if (ana_Faiz_Top == 1)
                    return onBorcMiktarABKur(tl);
                else if (ana_Faiz_Top == 2)
                    return 0;
                else
                    return onBorcMiktarABKur(tl) + 0;
            }
            else
            {
                if (ana_Faiz_Top == 1)
                    return onBorcMiktarUlusalKur(tl) + onBorcMiktarABKur(tl);
                else if (ana_Faiz_Top == 2)
                    return 0;
                else
                    return onBorcMiktarUlusalKur(tl) + onBorcMiktarABKur(tl);
            }
        }

        internal bool BendenSonraTaksitVarMi(int siraNo)
        {
            foreach (BorcHareket item in this.borcHareket)
            {
                if (item.siraNo > siraNo && item.tur == BorcHareket.ENUMHareketTur.Taksit) return true;
            }
            return false;
        }

        public void TaksitAyarla(DateTime islemTarih, bool tarihDahil)
        {
            double toplamOdenenAB = 0;
            double toplamOdenenUl = 0;
            double toplamOdenenABGZ = 0;
            double toplamOdenenUlGZ = 0;
            for (int i = 0; i < this.borcHareket.Count; i++)
            {
                BorcHareket bh = ((BorcHareket)this.borcHareket[i]);
                if (!bh.OdemeTuruIslem) continue;
                if (bh.tarihDT <= this.taksitKararTarihDT) continue;
                if (islemTarih > DateTime.MinValue)
                {
                    if (bh.tarihDT > islemTarih) continue;
                    if (!tarihDahil && bh.tarihDT == islemTarih) continue;
                }
                bh.anaDosya = this;
                toplamOdenenAB = OrtakFonksiyonlar.KurusYap(toplamOdenenAB + bh.miktarAB);
                toplamOdenenUl = OrtakFonksiyonlar.KurusYap(toplamOdenenUl + bh.miktarUlusal);
                toplamOdenenABGZ = OrtakFonksiyonlar.KurusYap(toplamOdenenABGZ + bh.ertelemeAB);
                toplamOdenenUlGZ = OrtakFonksiyonlar.KurusYap(toplamOdenenUlGZ + bh.ertelemeUl);
            }

            //Ödenenleri taksitlerden düşür
            for (int i = 0; i < this.borcHareket.Count && (toplamOdenenAB > 0 || toplamOdenenUl > 0 || toplamOdenenABGZ > 0 || toplamOdenenUlGZ > 0); i++)
            {
                BorcHareket bh = ((BorcHareket)this.borcHareket[i]);
                if (bh.tur != BorcHareket.ENUMHareketTur.Taksit) continue;
                bh.anaDosya = this;
                double bh_miktar = bh.miktarAB;
                MiktarAyarla(ref bh_miktar, ref toplamOdenenAB);
                bh.miktarAB = bh_miktar;

                bh_miktar = bh.miktarUlusal;
                MiktarAyarla(ref bh_miktar, ref toplamOdenenUl);
                bh.miktarUlusal = bh_miktar;

                bh_miktar = bh.ertelemeAB;
                MiktarAyarla(ref bh_miktar, ref toplamOdenenABGZ);
                bh.ertelemeAB = bh_miktar;

                bh_miktar = bh.ertelemeUl;
                MiktarAyarla(ref bh_miktar, ref toplamOdenenUlGZ);
                bh.ertelemeUl = bh_miktar;

                bh.tarihDT = bh.duzeltimTarihiDT;
                bh.OdenendenFaizHesapla(this.tecilFaizNo, null);
                bh.tarihDT = DateTime.MinValue;
            }
            //Bugün ve bugünden önce olan taksitleri topla tarihini bugün yap;
            double toplamGecAB = 0;
            double toplamGecUl = 0;
            double toplamGecABGZ = 0;
            double toplamGecUlGZ = 0;

            BorcHareket bhOdenmeyen = null;
            for (int i = 0; i < this.borcHareket.Count; i++)
            {
                BorcHareket bh = ((BorcHareket)this.borcHareket[i]);
                if (bh.tur != BorcHareket.ENUMHareketTur.Taksit) continue;
                if (bh.duzeltimTarihiDT > islemTarih) continue;
                if (bh.miktarAB == 0 && bh.miktarUlusal == 0 && bh.ertelemeAB == 0 && bh.ertelemeUl == 0) continue;

                toplamGecAB = OrtakFonksiyonlar.KurusYap(toplamGecAB + bh.miktarAB);
                toplamGecUl = OrtakFonksiyonlar.KurusYap(toplamGecUl + bh.miktarUlusal);
                toplamGecABGZ = OrtakFonksiyonlar.KurusYap(toplamGecABGZ + bh.ertelemeAB);
                toplamGecUlGZ = OrtakFonksiyonlar.KurusYap(toplamGecUlGZ + bh.ertelemeUl);
                bh.miktarAB = 0;
                bh.miktarUlusal = 0;
                bh.ertelemeAB = 0;
                bh.ertelemeUl = 0;
                bhOdenmeyen = bh;
            }
            if (bhOdenmeyen != null)
            {
                bhOdenmeyen.tarihDT = islemTarih;
                bhOdenmeyen.duzeltimTarihiDT = islemTarih;
                bhOdenmeyen.miktarAB = toplamGecAB;
                bhOdenmeyen.miktarUlusal = toplamGecUl;
                bhOdenmeyen.ertelemeAB = toplamGecABGZ;
                bhOdenmeyen.ertelemeUl = toplamGecUlGZ;
                bhOdenmeyen.OdenendenFaizHesapla(this.tecilFaizNo, null);
            }
        }
        private void MiktarAyarla(ref double bh_miktar, ref double toplamOdenen)
        {
            if (bh_miktar > toplamOdenen)
            {
                bh_miktar = OrtakFonksiyonlar.KurusYap(bh_miktar - toplamOdenen);
                toplamOdenen = 0;
            }
            else
            {
                toplamOdenen = OrtakFonksiyonlar.KurusYap(toplamOdenen - bh_miktar);
                bh_miktar = 0;
            }
        }

        public TNSCollection islemler = null;
        public bool bekleyenIslemler = false;

        public void IslemEkle(DosyaABParca islemBilgi)
        {
            if (islemler == null) islemler = new TNSCollection();
            islemler.Add(islemBilgi);
        }

        public string TecilFaizTur()
        {
            if (tecilFaizNo == 0)
                return "Tecil Faizi";
            else if (tecilFaizNo == 1)
                return "Tüzel Tecili";
            else
                return "Tanımsız";
        }

        public int aySayisi
        {
            get
            {
                if (FaizHesabiYapilacakMi)
                {
                    TNS.TarihFark tf = new TarihFark(faizBaslangicTarihi, islemTarihiDT, true);
                    return tf.YilAy;
                }
                else
                    return 0;
            }
        }

        public int gunSayisi
        {
            get
            {
                if (FaizHesabiYapilacakMi)
                {
                    TNS.TarihFark tf = new TarihFark(faizBaslangicTarihi, islemTarihiDT, true);
                    return tf.Gun;
                }
                else
                    return 0;
            }
        }

        public double faizOranAyToplam
        {
            get
            {
                int ay, gun;
                DateTime sadeceAy = faizBaslangicTarihi.AddMonths(aySayisi);
                return FaizIslemler.FaizOranHesapla(odemeSonTarihiDT, faizBaslangicTarihi, sadeceAy, this.faizinDurdurulmaTarihiDT, FaizIslemler.faizOran, true, null, out ay, out gun);
            }
        }

        public double faizOranGunToplam
        {
            get
            {
                int ay, gun;
                return FaizIslemler.FaizOranHesapla(odemeSonTarihiDT, faizBaslangicTarihi, islemTarihiDT, this.faizinDurdurulmaTarihiDT, FaizIslemler.faizOran, true, null, out ay, out gun) - faizOranAyToplam;
            }
        }

        public bool FaizHesabiYapilacakMi
        {
            get
            {
                return !TNSDateTime.IsNull(odemeSonTarihi) && faizBaslangicTarihi.Year > 2000 && odemeSonTarihiDT < islemTarihiDT;
            }
        }

        public string D1Donem
        {
            get
            {
                if (donemYil > 0) return donemYil.ToString() + "/" + (donemCeyrek).ToString() + ". Çeyrek";
                return "";
            }
        }

        public TNSDateTime ipardAktarimTar { get; set; }
        public int tecilFaizNo { get; set; }
    }

    /// <summary>
    /// Alacak Takip Dosya Durum Tarihçesi sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaDurumTarihce
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Dosya numarası
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// Durum
        /// </summary>
        public Durum durum;
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKodu;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaDurumTarihce()
        {
            yil = 0;
            dosyaNo = 0;
            durum = new Durum();
            siraNo = 0;
            tarih = new TNSDateTime();
            aciklama = "";
            kullaniciKodu = "";
        }
    }

    /// <summary>
    /// Alacak Takip Dosya Durum Tarihçesi sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaDurumTarihceAB
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Dosya numarası
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// Durum
        /// </summary>
        public Durum durum;
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKodu;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaDurumTarihceAB()
        {
            yil = 0;
            dosyaNo = 0;
            durum = new Durum();
            siraNo = 0;
            tarih = new TNSDateTime();
            aciklama = "";
            kullaniciKodu = "";
        }
    }

    /// <summary>
    /// Alacak Takip Dosya Taksit sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaTaksit
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Dosya numarası
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// Sıra numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Son ödeme tarihi
        /// </summary>
        public TNSDateTime sonOdemeTarihi;
        public DateTime ODEMETARIH { get { return sonOdemeTarihi.Oku(); } }

        public DateTime GERCEKODEMETARIH { get { return odemeTarihi.Oku(); } }
        /// <summary>
        /// Miktar
        /// </summary>
        public double miktar;
        /// <summary>
        /// Ödeme tarihi
        /// </summary>
        public TNSDateTime odemeTarihi;
        /// <summary>
        /// Faiz miktarı
        /// </summary>
        public double faizMiktar;
        public double taksitTarihiFaizi;
        public double SATIRTUTAR { get { return miktar + faizMiktar + taksitTarihiFaizi; } }
        /// <summary>
        /// Makbuz numarası
        /// </summary>
        public string makbuzNo;
        /// <summary>
        /// The odeme emri no
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// The faiz odeme emri no
        /// </summary>
        public string faizOdemeEmriNo;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaTaksit()
        {
            yil = 0;
            dosyaNo = 0;
            siraNo = 0;
            sonOdemeTarihi = new TNSDateTime();
            miktar = 0;
            odemeTarihi = new TNSDateTime();
            faizMiktar = 0;
            taksitTarihiFaizi = 0;
            makbuzNo = "";
            odemeEmriNo = "";
            faizOdemeEmriNo = "";
        }

        public bool odendi
        {
            get
            {
                return odemeTarihi.isNull ? false : true;
            }
        }

        public double FaizToplam { get { return faizMiktar + taksitTarihiFaizi; } }

        public void FaizleriHesapla(Dosya dosya)
        {
            if (TNSDateTime.IsNull(dosya.faizBaslangicTarihi) || TNSDateTime.IsNull(this.odemeTarihi))
            {
                this.taksitTarihiFaizi = 0;
                this.faizMiktar = 0;
            }
            else
            {
                DateTime taksitBasDT;
                if (TNSDateTime.IsNull(dosya.taksitBaslangicTarihi))
                    taksitBasDT = DateTime.MinValue;
                else
                    taksitBasDT = dosya.taksitBaslangicTarihi.Oku();

                DateTime tutD = dosya.faizBaslangicTarihi.Oku();
                if (taksitBasDT > DateTime.MinValue && taksitBasDT >= tutD && this.odemeTarihi.Oku() >= taksitBasDT)
                {
                    tutD = dosya.taksitBaslangicTarihi.Oku().AddDays(-1);
                    if (this.taksitTarihiFaizi <= 0)
                        this.taksitTarihiFaizi = this.miktar * TNS.ATM.Arac.FaizOrani(dosya.faizTur.oran, tutD, dosya.faizBaslangicTarihi.Oku(), DateTime.MinValue, DateTime.MinValue, 0);
                }
                else
                    this.taksitTarihiFaizi = 0;

                this.faizMiktar = Math.Round((this.miktar + this.taksitTarihiFaizi) * TNS.ATM.Arac.FaizOrani(dosya.faizTur.oran, this.odemeTarihi.Oku(), tutD, taksitBasDT, taksitBasDT, 0), 2);
            }
        }

        public bool FarkliMi(DosyaTaksit gelenTaksit)
        {
            return FarkliTarih(this.sonOdemeTarihi, gelenTaksit.sonOdemeTarihi) ||
            this.miktar != gelenTaksit.miktar ||
            FarkliTarih(this.odemeTarihi, gelenTaksit.odemeTarihi) ||
            this.makbuzNo != gelenTaksit.makbuzNo ||
            this.taksitTarihiFaizi != gelenTaksit.taksitTarihiFaizi;

        }

        private bool FarkliTarih(TNSDateTime t1, TNSDateTime t2)
        {
            if (TNSDateTime.IsNull(t1) && TNSDateTime.IsNull(t2)) return false;
            if (TNSDateTime.IsNull(t1) && !TNSDateTime.IsNull(t2)) return true;
            if (!TNSDateTime.IsNull(t1) && TNSDateTime.IsNull(t2)) return true;
            return t1.Oku() != t2.Oku();
        }
    }

    /// <summary>
    /// Alacak Takip Dosya Ödeme Planı sınıfı.
    /// </summary>
    [Serializable()]
    public class OdemePlani
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Dosya numarası
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// Taksit sayısı
        /// </summary>
        public int taksitSayisi;
        /// <summary>
        /// Periyod
        /// </summary>
        public int periyod;
        /// <summary>
        /// Son ödeme tarihi
        /// </summary>
        public TNSDateTime ilkOdemeTarihi;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public OdemePlani()
        {
            yil = 0;
            dosyaNo = 0;
            taksitSayisi = 0;
            periyod = 0;
            ilkOdemeTarihi = new TNSDateTime();
        }
    }

    /// <summary>
    /// Alacak Takip Dosya Hareket Ana sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaHesapHareketAna
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// TasarimKod
        /// </summary>
        public int tasarimKod;
        /// <summary>
        /// TasarimAdi
        /// </summary>
        public string tasarimAdi;
        /// <summary>
        /// Dosya numarası
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// tarih
        /// </summary>
        public TNSDateTime tarih;
        /// <summary>
        /// ödeme emri numarası
        /// </summary>
        public string odemeEmriNo;
        /// <summary>
        /// saymanlık kodu
        /// </summary>
        public int saymanlikKodu;
        /// <summary>
        /// toplamtutar
        /// </summary>
        public double toplamTutar;
        /// <summary>
        /// islem yapan adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// işlme yapan kod
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// işlem tarihi
        /// </summary>
        public TNSDateTime islemTarihi;
        /// <summary>
        /// birimKodu
        /// </summary>
        public string birimKodu;
        /// <summary>
        /// borçlu adı
        /// </summary>
        public string borcluAdi;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaHesapHareketAna()
        {
            kod = 0;
            yil = 0;
            dosyaNo = 0;
            tasarimKod = 0;
            tarih = new TNSDateTime();
            odemeEmriNo = "";
            toplamTutar = 0;
            islemYapanAd = "";
            islemYapanKod = "";
            islemTarihi = new TNSDateTime();
            birimKodu = "";
            borcluAdi = "";
            tasarimAdi = "";
        }
    }

    /// <summary>
    /// Alacak Takip Hesap Hareket Tasarım sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaHesapHareketTasarim
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Sıra No
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Hesap Kodu
        /// </summary>
        public string hesapKodu;
        /// <summary>
        /// adı
        /// </summary>
        public string adi;
        /// <summary>
        /// int
        /// </summary>
        public int borcAlacak;
        /// <summary>
        /// islem yapan adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// işlme yapan kod
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// işlem tarihi
        /// </summary>
        public TNSDateTime islemTarihi;
        /// <summary>
        /// yansitmaHesapKodu
        /// </summary>
        public string yansitmaHesapKodu;
        /// <summary>
        /// kdvHesapKodu 
        /// </summary>
        public string kdvHesapKodu = "";
        /// <summary>
        /// altHesap
        /// </summary>
        public int altHesap = 0;
        /// <summary>
        /// belgeSatirNo
        /// </summary>
        public int belgeSatirNo = 0;
        /// <summary>
        /// oran
        /// </summary>
        public decimal oran = 0;
        /// <summary>
        /// anakod
        /// </summary>
        public int anaKod = 0;
        public string alanAdi = "";
        public string formul = "";

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaHesapHareketTasarim()
        {
            kod = -1;
            siraNo = 0;
            hesapKodu = "";
            adi = "";
            borcAlacak = -1;
            islemYapanAd = "";
            islemYapanKod = "";
            islemTarihi = new TNSDateTime();
            yansitmaHesapKodu = "";
            kdvHesapKodu = "";
            altHesap = 0;
            belgeSatirNo = 0;
            anaKod = 0;
            alanAdi = "";
            formul = "";
        }
    }

    /// <summary>
    /// Alacak Takip Hesap Hareket Çocuk sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaHesapHareketCocuk
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// Ana No
        /// </summary>
        public int anaKod;
        /// <summary>
        /// Ana kod listesi
        /// </summary>
        public string anaKodList;
        /// <summary>
        /// sıra no
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Hesap Kodu
        /// </summary>
        public string hesapKodu;
        /// <summary>
        /// Adı
        /// </summary>
        public string adi;
        /// <summary>
        /// borç
        /// </summary>
        public double borc;
        /// <summary>
        /// alacak
        /// </summary>
        public double alacak;
        /// <summary>
        /// islem yapan adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// işlme yapan kod
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// işlem tarihi
        /// </summary>
        public TNSDateTime islemTarihi;
        /// <summary>
        /// alt hesap kod
        /// </summary>
        public string altHesapKod;
        /// <summary>
        /// The dosya yil
        /// </summary>
        public int dosyaYil;
        /// <summary>
        /// The dosya muhasebe
        /// </summary>
        public int dosyaMuhasebe;
        /// <summary>
        /// The dosya birim
        /// </summary>
        public string dosyaBirim;
        /// <summary>
        /// The dosya no
        /// </summary>
        public int dosyaNo;
        /// <summary>
        /// The dosya borclu ad
        /// </summary>
        public string dosyaBorcluAd;
        /// <summary>
        /// The tasarim kod
        /// </summary>
        public int tasarimKod;
        public int uretilenSatir;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaHesapHareketCocuk()
        {
            kod = 0;
            anaKod = 0;
            siraNo = 0;
            hesapKodu = "";
            adi = "";
            borc = 0;
            alacak = 0;
            islemYapanAd = "";
            islemYapanKod = "";
            islemTarihi = new TNSDateTime();
            altHesapKod = "";
            dosyaYil = 0;
            dosyaMuhasebe = 0;
            dosyaBirim = "";
            dosyaNo = 0;
            dosyaBorcluAd = "";
            tasarimKod = 0;
            uretilenSatir = 0;
        }
    }

    /// <summary>
    /// Alacak Takip Hesap Hareket Tasarım sınıfı.
    /// </summary>
    [Serializable()]
    public class DosyaHesapHareketTasarimAna
    {
        /// <summary>
        /// Kod
        /// </summary>
        public int kod;
        /// <summary>
        /// ad
        /// </summary>
        public string ad;
        /// <summary>
        /// aktif
        /// </summary>
        public int aktif;
        /// <summary>
        /// islem yapan adı
        /// </summary>
        public string islemYapanAd;
        /// <summary>
        /// işlme yapan kod
        /// </summary>
        public string islemYapanKod;
        /// <summary>
        /// işlem tarihi
        /// </summary>
        public TNSDateTime islemTarihi;

        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        public DosyaHesapHareketTasarimAna()
        {
            kod = -1;
            ad = "";
            aktif = 0;
            islemYapanAd = "";
            islemYapanKod = "";
            islemTarihi = new TNSDateTime();
        }
    }

    /// <summary>
    /// OnBorc: 
    /// </summary>
    [System.Serializable()]
    public class ABBorclu : DosyaABParca
    {
        public string kimlikNo { get; set; }
        public string borcluAd { get; set; }
        public string borcluKodAd { get { return kimlikNo + "- " + borcluAd; } }

        public string saymanlikAd { get; set; }
        public string birimAd { get; set; }

        public string defterSirasi { get; set; }

        public string tertip { get { return tedbir + "-" + altTedbir + "-" + butceKalemi; } }

        /// <summary>
        /// Tedbir: Borçlu Defteri'ne giriş sırası
        /// </summary>
        public string tedbir { get; set; }

        /// <summary>
        /// Alt Tedbir: Borçlu Defteri'ne giriş sırası
        /// </summary>
        public string altTedbir { get; set; }

        /// <summary>
        /// Bütçe Kalemi: Borçlu Defteri'ne giriş sırası
        /// </summary>
        public string butceKalemi { get; set; }

        /// <summary>
        /// Saymanlık Kod: Borçlu takip eden saymanlık
        /// </summary>
        public int saymanlikKod { get; set; }

        /// <summary>
        /// Birim Kod: Borçlu takip eden birim
        /// </summary>
        public string birimKod { get; set; }

        /// <summary>
        /// Borçlu Kod: Firma Kodu
        /// </summary>
        public int borcluKod { get; set; }

        /// <summary>
        /// Başvuru Kayıt Numarası: Başvuru Kayıt Numarası
        /// </summary>
        public string basvuruKayitNumarasi { get; set; }

        /// <summary>
        /// Sözleşme Numarası: Sözleşme Numarası
        /// </summary>
        public string sozlesmeNumarasi { get; set; }
    }

    [Serializable()]
    public class DosyaABParca
    {
        /// <summary>
        /// IPARDDONEMI: IPARD Dönemi.
        /// </summary>
        public int ipardDonemi { get; set; }

        /// <summary>
        /// DOSYAANAHTAR: Dosya Anahtar
        /// </summary>
        public int dosyaAnahtar { get; set; }

        /// <summary>
        /// Dosya ile ilgili parçaların sıra numarası
        /// </summary>
        public int sira { get; set; }

        public TNSDateTime basTarih { get; set; } = new TNSDateTime();
        public DateTime basTarihDT
        {
            get
            {
                if (basTarih == null)
                    return DateTime.MinValue;
                else
                    return basTarih.Oku();
            }
            set
            {
                if (basTarih == null) basTarih = new TNSDateTime(value);
                else basTarih.Yaz(value);
            }
        }

        public TNSDateTime bitTarih { get; set; } = new TNSDateTime();
        public ENUMKayitDurum kayitDurum { get; set; }
        public string kayitDurumAd
        {
            get
            {
                return KayitDurumAd(kayitDurum);
            }
        }

        public string KayitDurumAd(ENUMKayitDurum kayitDurum)
        {
            switch (kayitDurum)
            {
                case ENUMKayitDurum.YENI:
                    return "Yeni";
                case ENUMKayitDurum.ONAYLI:
                    return "Onaylı";
                case ENUMKayitDurum.IPTAL:
                    return "İptal";
                default:
                    return "";
            }
        }

        public string gerekce { get; set; }
        public ENUMTabloKod tabloKod { get; set; }
        public string tabloKodAd
        {
            get
            {
                return TabloAdBul(this.tabloKod);
            }
        }

        public static string TabloAdBul(ENUMTabloKod tabloKod)
        {
            switch (tabloKod)
            {
                case ENUMTabloKod.BORCLU:
                    return "Kimlik";
                case ENUMTabloKod.ONBORC:
                    return "Ön Borç";
                case ENUMTabloKod.GERIALIMKARARI:
                    return "Geri Alım";
                case ENUMTabloKod.DURUM:
                    return "Durum";
                case ENUMTabloKod.TAKSIT:
                    return "Taksit";
                default:
                    return "";
            }
        }

        public string kaydeden { get; set; }
        public TNSDateTime kayitTarihi { get; set; } = new TNSDateTime();
        public DateTime kayitTarihiDT { get { return kayitTarihi.Oku(); } }

        public string kaydedenAd { get; set; }
        public string kaydedenSoyad { get; set; }
        public string kaydedenUnvan { get; set; }
        public string kaydedenAdSoyad { get { return kaydedenAd + " " + kaydedenSoyad; } }

        public string onaylayan { get; set; }
        public TNSDateTime onayTarihi { get; set; } = new TNSDateTime();
        public DateTime onayTarihiDT { get { return onayTarihi.Oku(); } }

        public string onaylayanAd { get; set; }
        public string onaylayanSoyad { get; set; }
        public string onaylayanUnvan { get; set; }
        public string onaylayanAdSoyad { get { return onaylayanAd + " " + onaylayanSoyad; } }

        public bool YeniYapilirMi()
        {
            if (this.kayitDurum == ENUMKayitDurum.ONAYASUNULDU)
                return true;
            else if (this.kayitDurum == ENUMKayitDurum.ONAYLI)
            {
                int ay;
                int yil;
                yil = onayTarihiDT.Year;
                ay = onayTarihiDT.Month + 1;
                if (ay == 13)
                {
                    ay = 1;
                    yil++;
                }
                DateTime sonYeniTarih = new DateTime(yil, ay, 20);
                if (DateTime.Now.Date <= sonYeniTarih)
                    return true;
            }
            return false;
        }

        public void IslemKontrol(Sonuc s, ENUMDosyaIslemTur islemTur, string islemYapanKod)
        {
            if (islemTur == ENUMDosyaIslemTur.Onayla)
            {
                if (this.kaydeden == islemYapanKod && islemYapanKod != "stratek")
                    s.DegerAta("Kayıt işlemini yapanla, onaylama işlemini yapan aynı kişi olamaz!");
            }
            else if (islemTur == ENUMDosyaIslemTur.YeniYap)
            {
                if (!YeniYapilirMi())
                {
                    s.DegerAta("Onaylanmış kayıtlar ancak, bir sonraki ayın ortasına kadar yeni yapılabilir!");
                }
            }
            if (islemTur != ENUMDosyaIslemTur.OzelGuncelle)
            {
                if (islemTur != ENUMDosyaIslemTur.YeniYap && this.kayitDurum != DosyaABParca.ENUMKayitDurum.YENI)
                {
                    s.DegerAta("Durumu yeni olan kayıtlar güncellenebilir ya da durumu değiştirilebilir!");
                }
            }
        }


        public enum ENUMTabloKod : int
        {
            BORCLU = 1,
            ONBORC = 2,
            GERIALIMKARARI = 3,
            DURUM = 4,
            TAKSIT = 5,
        }

        public enum ENUMKayitDurum : int
        {
            YENI = 1,
            ONAYASUNULDU = 2,
            ONAYLI = 5,
            IPTAL = 9,
        }
    }


    /// <summary>
    /// OnBorc: 
    /// </summary>
    [System.Serializable()]
    public class OnBorc : DosyaABParca
    {
        /// <summary>
        /// BORCTURU: Borç Türü
        /// </summary>
        public DosyaAB.BorcTurleri borcTuru { get; set; }

        public string borcTuruAd { get { return DosyaAB.BorcTuruAd(borcTuru); } }

        /// <summary>
        /// BORCUNKODU: Borcun Kodu
        /// </summary>
        public string borcunKodu { get; set; }

        /// <summary>
        /// ONBORCTARIHI: Ön Borç Tarihi
        /// </summary>
        public TNSDateTime onBorcTarihi { get; set; } = new TNSDateTime();
        public DateTime onBorcTarihiDT { get { return onBorcTarihi.Oku(); } }

        /// <summary>
        /// ONBORCSAYI: Ön Borç Sayı
        /// </summary>
        public string onBorcSayi { get; set; }

        /// <summary>
        /// VAKANUMARASI: Vaka Numarası
        /// </summary>
        public string vakaNumarasi { get; set; }
        public bool avans { get; set; } = false;

        private MiktarIslem _onBorcIslem = new MiktarIslem(0, 0, 0, 0, 0, 0, 0);

        public double onBorcDovizKuru { get { return _onBorcIslem.kur; } set { _onBorcIslem.kur = value; } }

        /// <summary>
        /// ONBORCMIKTARAB: Ön Borç Miktarı (AB)
        /// </summary>
        public double onBorcMiktarAB { get { return _onBorcIslem.miktarAB; } set { _onBorcIslem.miktarAB = value; } }

        /// <summary>
        /// ONBORCMIKTARULUSAL: Ön Borç Miktarı (Ulusal)
        /// </summary>
        public double onBorcMiktarUlusal { get { return _onBorcIslem.miktarUlusal; } set { _onBorcIslem.miktarUlusal = value; } }

        public double onBorcToplam { get { return _onBorcIslem.miktarToplam; } }

        public double onBorcMiktarABEUR { get { return _onBorcIslem.miktarABEUR; } }

        /// <summary>
        /// ONBORCMIKTARULUSAL: Ön Borç Miktarı (Ulusal)
        /// </summary>
        public double onBorcMiktarUlusalEUR { get { return _onBorcIslem.miktarUlusalEUR; } }

        public double onBorcToplamEUR { get { return _onBorcIslem.miktarToplamEUR; } }

        public bool OBKaldirma { get; set; }

        public TNSDateTime aktarimTarihi = new TNSDateTime();
    }

    [Serializable()]
    public class GeriAlimKarari : DosyaABParca
    {
        /// <summary>
        /// ONBORCTARIHI: Ön Borç Tarihi
        /// </summary>
        public TNSDateTime geriAlimKarariTarihi { get; set; } = new TNSDateTime();
        public DateTime geriAlimKarariTarihiDT { get { return geriAlimKarariTarihi.Oku(); } }
        private MiktarIslem _gakIslem = new MiktarIslem(0, 0, 0, 0, 0, 0, 0);
        public bool gakKaldirma;

        public double gakDovizKuru { get { return _gakIslem.kur; } set { _gakIslem.kur = value; } }

        /// <summary>
        /// ONBORCMIKTARAB: Ön Borç Miktarı (AB)
        /// </summary>
        public double gakMiktarAB { get { return _gakIslem.miktarAB; } set { _gakIslem.miktarAB = value; } }

        /// <summary>
        /// ONBORCMIKTARAB: Ön Borç Miktarı (AB)
        /// </summary>
        public double silinmeyecekMiktar { get { return _gakIslem.silinmeyecekMiktar; } set { _gakIslem.silinmeyecekMiktar = value; } }

        /// <summary>
        /// ONBORCMIKTARULUSAL: Ön Borç Miktarı (Ulusal)
        /// </summary>
        public double gakMiktarUlusal { get { return _gakIslem.miktarUlusal; } set { _gakIslem.miktarUlusal = value; } }

        public double gakToplam { get { return _gakIslem.miktarToplam; } }

        public double gakMiktarABEUR { get { return _gakIslem.miktarABEUR; } }

        public double silinmeyecekMiktarEUR { get { return _gakIslem.silinmeyecekMiktarEUR; } }

        /// <summary>
        /// ONBORCMIKTARULUSAL: Ön Borç Miktarı (Ulusal)
        /// </summary>
        public double gakMiktarUlusalEUR { get { return _gakIslem.miktarUlusalEUR; } }

        public double gakToplamEUR { get { return _gakIslem.miktarToplamEUR; } }

        public string geriAlimSayi { get; set; }

        public TNSDateTime bildirimTarihi { get; set; } = new TNSDateTime();
        public TNSDateTime tesellumTarihi { get; set; } = new TNSDateTime();
        public TNSDateTime odemeSonTarihi { get; set; } = new TNSDateTime();
        public TNSDateTime temerrutTarihi { get; set; } = new TNSDateTime();
        public DateTime temerrutTarihiDT { get { return temerrutTarihi.Oku(); } }
        public TNSDateTime faizinDurdurulmaTarihi { get; set; } = new TNSDateTime();

        public void TarihleriBul()
        {
            odemeSonTarihi.Yaz(this.tesellumTarihi.Oku().AddMonths(1));
            while (true)
            {
                if (!TNS.ResmiTatil.ResmiTatilMi(odemeSonTarihi, 0)) break;
                odemeSonTarihi.AddDays(1);
            }
            temerrutTarihi.Yaz(this.odemeSonTarihi.Oku().AddDays(1));
            //bildirimTarihi.Yaz(geriAlimKarariTarihi.Oku());
        }
    }

    [Serializable()]
    public class DosyaABDurum : DosyaABParca
    {
        public DosyaAB.ENUMDosyaDurum surecAsamasi;
        public string surecAsamasiAd { get { return DosyaAB.DosyaDurumAdBul(surecAsamasi); } }
        public string yorumlarTR;
        public string yorumlarEN;
    }

    [Serializable()]
    public class DosyaABTaksit : DosyaABParca
    {
        public TNSDateTime girisTarihi { get; set; } = new TNSDateTime();
        public DateTime girisTarihiDT { get { return girisTarihi.Oku(); } }
        public TNSDateTime taksitTalepTarihi { get; set; } = new TNSDateTime();
        public DateTime taksitTalepTarihiDT { get { return taksitTalepTarihi.Oku(); } }
        public string taksitTalepSayi { get; set; }
        public TNSDateTime taksitKararTarihi { get; set; } = new TNSDateTime();
        public string taksitKararSayi { get; set; }
        public TNSDateTime taksitIlkTarih { get; set; } = new TNSDateTime();
        public int taksitSayisi { get; set; }
        public int tecilFaizNo { get; set; }

        public bool taksitKaldirma = false;
    }
}
