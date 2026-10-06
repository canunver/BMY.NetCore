using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TNS;
using TNS.BHMv2;
using TNS.BKD;
using TNS.KYM;
using TNS.UZY;
//using TNS.BHM;

namespace TNS.BUM
{
    /// <summary>
    /// Bütçe Uygulama ortak işlemleri için kullanılan sınıf
    /// </summary>
    [Serializable()]
    public static class Arac
    {
        /// <summary>
        /// Bütçe Uygulama Servisini, Konfigürasyondan okuyarak tanımlayan fonksiyon
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe Uygulama Servisini tutan, IBUMServis türünde nesne döndürür</returns>
        public static IBUMServis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("BUMServisURL");
            IBUMServis servis = TNS.Api.ServisIstemci.Olustur<IBUMServis>(sUrl);
            return servis;
        }

    }


    /// <summary>
    /// Form durumu Grup türlerinin listesi
    /// </summary>
    public enum ENUMFormDurumGrup : int
    {
        /// <summary>
        /// Hepsi Form durumu Grubu
        /// </summary>
        HEPSI = 0,
        /// <summary>
        /// Yeni Form durumu Grubu
        /// </summary>
        YENI = 1,
        /// <summary>
        /// Onaylı Form durumu Grubu
        /// </summary>
        ONAYLI = 2,
        /// <summary>
        /// Yeni Onaylı Form durumu Grubu
        /// </summary>
        YENI_ONAYLI = 3,
        /// <summary>
        /// İptalli Form durumu Grubu
        /// </summary>
        IPTALLI = 4,
        /// <summary>
        /// Hatalı Form durumu Grubu
        /// </summary>
        HATALI = 99,
    }

    /// <summary>
    /// Form durumu türlerinin listesi
    /// </summary>
    public enum ENUMFormDurum : int
    {
        /// <summary>
        /// Boş kayıt form durumu
        /// </summary>
        BOSKAYIT = 0,
        /// <summary>
        /// Yeni kayıt form durumu
        /// </summary>
        YENIKAYIT = 1,
        /// <summary>
        /// Dayanak doğru form durumu
        /// </summary>
        DYDOGRU = 2,
        /// <summary>
        /// Sayıştaya gönderildi form durumu
        /// </summary>
        SAYISTAYAGONDERILDI = 3,
        /// <summary>
        /// Sayıştayda form durumu
        /// </summary>
        SAYISTAYDA = 4,
        /// <summary>
        /// Onaylı form durumu
        /// </summary>
        ONAYLI = 5,
        /// <summary>
        /// Sayıştay red form durumu
        /// </summary>
        SAYISTAYRED = 6,
        /// <summary>
        /// Bütçe işlem iptal form durumu
        /// </summary>
        BUTCEISLEMIPTAL = 7,
        /// <summary>
        /// Dayanak yanlış form durumu
        /// </summary>
        DYYANLIS = 8,
        /// <summary>
        /// Silindi form durumu
        /// </summary>
        SILINDI = 9
    }


    [Serializable()]
    public class GunDegerHarcamaTur
    {
        public GunDegerHarcamaTur(int harcamaTur, double tutar)
        {
            this.harcamaTur = harcamaTur;
            this.tutar = tutar;
        }

        public GunDegerHarcamaTur()
        {
            this.harcamaTur = 0;
            this.tutar = 0;
        }

        public int harcamaTur { get; set; }
        public double tutar { get; set; }
    }


    [Serializable()]
    public class GunDeger
    {
        public GunDeger(DateTime d)
        {
            Tarih.Yaz(d);
        }

        public GunDeger()
        {
        }

        public double Toplam { get; set; } = 0;
        public TNSDateTime Tarih { get; set; } = new TNSDateTime();

        public TNSCollection harcamaTutarlari { get; set; }=new TNSCollection();

        public DateTime TarihDT
        {
            get { return Tarih.Oku(); }
            set { Tarih.Yaz(value); }
        }

        public void DegerAta(int harcamaTur, double tutar)
        {
            Toplam += tutar;

            foreach (GunDegerHarcamaTur item in harcamaTutarlari)
            {
                if (item.harcamaTur == harcamaTur)
                {
                    item.tutar += tutar;
                    return;
                }
            }
            harcamaTutarlari.Add(new GunDegerHarcamaTur(harcamaTur, tutar));
        }
    }

    [Serializable()]
    public sealed class NakitPlanGrup
    {
        public int tur { get; set; } = 0;
        public int Muhasebe { get; set; }
        public string Birim { get; set; }
        public string KurKod { get; set; }
        public TNSCollection GunMiktarlari { get; set; } = new TNSCollection(); // GunDeger objeleri
        public string muhasebeAd { get; set; }
        public string birimAd { get; set; }
        public string Anahtar
        {
            get { return Muhasebe + "_" + Birim + "_" + KurKod; }
        }

        public void DegerAta(GunDeger gunDeger)
        {
            foreach (GunDeger item in GunMiktarlari)
            {
                if (item.TarihDT == gunDeger.TarihDT)
                {
                    //if (gunDeger.harcamaTutarlari != null)
                    //{
                    //    for (int i = 1; i < gunDeger.harcamaTutarlari.Length; i++)
                    //    {
                    //        item.DegerAta(i, gunDeger.harcamaTutarlari[i]);
                    //    }
                    //}
                    //else
                    //{
                    //    System.Diagnostics.Debug.WriteLine("gunDeger.AlanMiktarlari null!");
                    //}

                    break;
                }
            }
        }

        public GunDeger GunGetir(DateTime tarih)
        {
            foreach (GunDeger gd in GunMiktarlari)
            {
                if (gd.TarihDT == tarih)
                    return gd;
            }
            return null;
        }

        public void GunleriEkle(TNSDateTime tarih1, TNSDateTime tarih2)
        {
            DateTime d = tarih1.Oku();
            while (d <= tarih2.Oku())
            {
                GunMiktarlari.Add(new GunDeger(d.Date));
                d = d.AddDays(1);
            }
        }
    }



    /// <summary>
    /// Ayrıntılı Harcama Bloke Sınıfı
    /// </summary>
    [Serializable()]
    public class AHPBloke
    {
        /// <summary>
        /// Tertip bilgisi
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Kesintili Başlangıç Ödeneği
        /// </summary>
        public double KBO;
        /// <summary>
        /// Ayrıntılı Harcama Programı Üst limiti
        /// </summary>
        public double AHPUst;

        /// <summary>
        /// Sınıfın oluşturucu(Constructer) Fonksiyonu. Yıl parametresi alır. Yıl ve TABButce tablo koduna göre yeni tertip oluşturur.
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        public AHPBloke(int yil)
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABButce);
        }
    }

    /// <summary>
    /// Tertipler Veri Aktarma Nesnesi Sınıfı
    /// </summary>
    [Serializable()]
    public class TertiplerVAN
    {
        /// <summary>
        /// Ödenek Üstü Tertipler
        /// </summary>
        public TNSCollection odenekUstuTertipler;//Tertiplerden oluşan
        /// <summary>
        /// Dağıtılmayan Tertipler
        /// </summary>
        public TNSCollection dagitilamiyanTertipler;//Tertiplerden oluşan
        /// <summary>
        /// Veri aktarımı sonucu
        /// </summary>
        public Sonuc sonuc;

        /// <summary>
        /// Sınıfın oluşturucu(constructer) fonksiyonu. Sonuc, Dağıtılmayan Tertipler, Ödenek üstü tertipleri oluşturur.
        /// </summary>
        public TertiplerVAN()
        {
            sonuc = new Sonuc();
            dagitilamiyanTertipler = new TNSCollection();
            odenekUstuTertipler = new TNSCollection();
        }
    }

    /// <summary>
    /// İşlem kodu türlerinin listesi
    /// </summary>
    public enum ENUMIslemKod : int
    {
        /// <summary>
        /// Ayrıntılı Harcama Programı işlem türü
        /// </summary>
        AHP = 0,
        /// <summary>
        /// Ayrıntılı Harcama Programı Revize işlem türü
        /// </summary>
        AHPREVIZE = 1,
        /// <summary>
        /// Serbest işlem türü
        /// </summary>
        SERBEST = 2,
        /// <summary>
        /// İcmal Harcama Programı işlem türü
        /// </summary>
        IHP = 3,
        /// <summary>
        /// İcmal Harcama Programı işlem türü
        /// </summary>
        IHPREVIZE = 4,
        /// <summary>
        /// Ekleme işlem türü
        /// </summary>
        EKLEME = 5,
        /// <summary>
        /// Aktarma işlem türü
        /// </summary>
        AKTARMA = 6,
        /// <summary>
        /// İptal işlem türü
        /// </summary>
        IPTAL = 7,
        /// <summary>
        /// Tel emri işlem türü
        /// </summary>
        TELEMRI = 8,
        /// <summary>
        /// Gelirli işlem türü
        /// </summary>
        GELIRLI = 9,
        /// <summary>
        /// Devren gelirli işlem türü
        /// </summary>
        DEVRENGELIRLI = 10,
        /// <summary>
        /// Ödenek ekleme işlem türü
        /// </summary>
        ODENEKEKLEME = 11,
        /// <summary>
        /// Akreditif işlem türü
        /// </summary>
        AKREDITIF = 12,
        /// <summary>
        /// Taahhüt artığı işlem türü
        /// </summary>
        TAAHHUTARTIGI = 13,
        /// <summary>
        /// Devreden iş proje işlem türü
        /// </summary>
        DEVREDENDISPROJE = 14,
        /// <summary>
        /// Kurum içi aktarma işlem türü
        /// </summary>
        KURUMICIAKTARMA = 15,
        /// <summary>
        /// Kurumlar arası aktarma işlem türü
        /// </summary>
        KURUMLARARASIAKTARMA = 16,
        /// <summary>
        /// Yedekten aktarma işlem türü
        /// </summary>
        YEDEKTENAKTARMA = 17,
        /// <summary>
        /// Ayrıntılı Harcama Programı Revize Serbest işlem türü
        /// </summary>
        AHPREVIZESERBEST = 18,
        /// <summary>
        /// Ayrıntılı Harcama Programı Revize İcmal işlem türü
        /// </summary>
        AHPREVIZEICMAL = 19,
        /// <summary>
        /// Gelir fazlası işlem türü
        /// </summary>
        GELIRFAZLASI = 20,
        /// <summary>
        /// Likit karşılığı işlem türü
        /// </summary>
        LIKITKARSILIGI = 21,
        /// <summary>
        /// Yedeğe aktarma işlem türü
        /// </summary>
        YEDEGEAKTARMA = 22,
        /// <summary>
        /// Ana serbest işlem türü
        /// </summary>
        ANASERBEST = 23,
        /// <summary>
        /// Ana ekleme aktarma işlem türü
        /// </summary>
        ANAEKLEMEAKTARMA = 24,
        /// <summary>
        /// Ana iptal işlem türü
        /// </summary>
        ANAIPTAL = 25,
        /// <summary>
        /// Bağış devri işlem türü
        /// </summary>
        BAGISDEVRI = 26,
        /// <summary>
        /// Gelir ödenek protokol işlem türü
        /// </summary>
        GELIRLIODENEKPROTOKOL = 27,

        /// <summary>
        /// Bağış yardım işlem türü
        /// </summary>
        BAGISYARDIM = 28,

        /// <summary>
        /// Gelir ödenek diğer işlem türü
        /// </summary>
        GELIRLIODENEKDIGER = 29,

        /// <summary>
        /// Gelir ödenek diğer işlem türü
        /// </summary>
        YIKOBAKTARMA = 30,

        /// <summary>
        /// Gelir ödenek diğer işlem türü
        /// </summary>
        YIKOBODENEKDEVRI = 31,

    }

    /// <summary>
    /// İşlem Bilgi Sınıfı
    /// </summary>
    public class IslemBilgi
    {
        /// <summary>
        /// İşlem kodu
        /// </summary>
        public string islemKod;
        /// <summary>
        /// İşlem adı
        /// </summary>
        public string islemAd;
        /// <summary>
        /// Son kırılım olup olmadığı
        /// </summary>
        public bool sonKirilim;
        /// <summary>
        /// Bütçe işlemi olup olmadığı
        /// </summary>
        public bool butceIslemiMi;
        /// <summary>
        /// İşlem kodu türü
        /// </summary>
        public ENUMIslemKod enumIK;
        /// <summary>
        /// Sınıfın oluşturucu(Constructer fonksiyonu) Almış olduğu İşlem kodu, Adı, Son Kırılım ve Bütçe İşlemi bilgilerinin local değerlere atamasını yapar.
        /// </summary>
        /// <param name="enumIK">İşlem kodu türünü tutan, ENUMIslemKod türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="islemAd">İşlem adı bilgisini tutan, String türünde nesne</param>
        /// <param name="sonKirilim">Son Kırılım olup olmadığı bilgisini tutan, Bool türünde nesne</param>
        /// <param name="butceIslemiMi">Bütçe işlemi olup olmadığı bilgisini tutan, Bool türünde nesne</param>
        public IslemBilgi(ENUMIslemKod enumIK, string islemKod, string islemAd, bool sonKirilim, bool butceIslemiMi)
        {
            this.islemKod = islemKod;
            this.islemAd = islemAd;
            this.sonKirilim = sonKirilim;
            this.butceIslemiMi = butceIslemiMi;
            this.enumIK = enumIK;
        }
    }

    /// <summary>
    /// Bütçe İşlemi Sabitler Sınıfı
    /// </summary>
    public class BISabitler
    {
        //public static string[] IslemKod ={"02.01", "02.02", "03.01", "01.04", "02.01.02", "02.01.04", "02.01.01", "02.01.03", "02.01.05", "02.01.06", "02.02.01", "02.02.02", "02.02.03", "01.02.01", "01.02.02", "02.01.07", "02.01.08", "02.02.04" };
        //public static string[] IslemAd ={"Ekleme", "Aktarma", "İptal", "Tel Emri", "Gelirli Ödenek Kaydı", "Ödenek Devri", "Ödenek Ekleme", "Akreditif Artığı Ekleme", "Taahhüt Avans Artığı Ekleme", "Devreden Dış Proje Kredileri", "Kurum İçi Aktarma", "Kurumlar Arası Aktarma", "Yedekten Aktarma", "AHP Revize ile Serbest", "AHP Revize Değişim", "Gelir Fazlası Karşılığı Ödenek Kaydı", "Likit Karşılığı Ödenek Kaydı", "Yedeğe Aktarma" };

        /// <summary>
        /// İşlemleri Kıyaslayan sınıf
        /// </summary>
        public class IslemKiyas : IComparer
        {
            /// <summary>
            /// Verilen iki objeyi birbiriyle kıyaslar
            /// </summary>
            /// <param name="x">Kıyaslanacak obje, Object türünde nesne</param>
            /// <param name="y">Kıyaslanacak obje, Object türünde nesne</param>
            /// <returns>Fonksiyonun çalışması sonucunda Int türünde nesne döndürür</returns>
            public int Compare(object x, object y)
            {
                throw new Exception(ButceUygulamaResI.BUM252);
            }
        }

        /// <summary>
        /// İşlem listesini tutan dizi
        /// </summary>
        public static ArrayList islemListesi;
        static BISabitler()
        {
            islemListesi = new ArrayList();
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.ANASERBEST, "01", ButceUygulamaResI.BUM253, false, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.AHP, "01.01", ButceUygulamaResI.BUM254, true, false));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.AHPREVIZE, "01.02", ButceUygulamaResI.BUM255, false, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.AHPREVIZESERBEST, "01.02.01", ButceUygulamaResI.BUM256, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.AHPREVIZEICMAL, "01.02.02", ButceUygulamaResI.BUM257, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.SERBEST, "01.03", ButceUygulamaResI.BUM258, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.TELEMRI, "01.04", ButceUygulamaResI.BUM259, true, false));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.IHP, "01.06", ButceUygulamaResI.BUM260, true, false));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.IHPREVIZE, "01.07", ButceUygulamaResI.BUM261, true, true));

            islemListesi.Add(new IslemBilgi(ENUMIslemKod.ANAEKLEMEAKTARMA, "02", ButceUygulamaResI.BUM262, false, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.EKLEME, "02.01", ButceUygulamaResI.BUM263, false, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.ODENEKEKLEME, "02.01.01", ButceUygulamaResI.BUM264, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.GELIRLI, "02.01.02", ButceUygulamaResI.BUM265, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.AKREDITIF, "02.01.03", ButceUygulamaResI.BUM266, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.DEVRENGELIRLI, "02.01.04", ButceUygulamaResI.BUM267, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.TAAHHUTARTIGI, "02.01.05", ButceUygulamaResI.BUM268, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.DEVREDENDISPROJE, "02.01.06", ButceUygulamaResI.BUM269, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.GELIRFAZLASI, "02.01.07", ButceUygulamaResI.BUM270, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.LIKITKARSILIGI, "02.01.08", ButceUygulamaResI.BUM271, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.BAGISDEVRI, "02.01.09", ButceUygulamaResI.BUM272, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.GELIRLIODENEKPROTOKOL, "02.01.10", ButceUygulamaResI.BUM273, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.BAGISYARDIM, "02.01.11", ButceUygulamaResI.BUM274, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.GELIRLIODENEKDIGER, "02.01.12", ButceUygulamaResI.BUM275, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.YIKOBODENEKDEVRI, "02.01.13", "YIKOB Ödenek Devri", true, true));

            islemListesi.Add(new IslemBilgi(ENUMIslemKod.AKTARMA, "02.02", ButceUygulamaResI.BUM276, false, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.KURUMICIAKTARMA, "02.02.01", ButceUygulamaResI.BUM277, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.KURUMLARARASIAKTARMA, "02.02.02", ButceUygulamaResI.BUM278, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.YEDEKTENAKTARMA, "02.02.03", ButceUygulamaResI.BUM279, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.YEDEGEAKTARMA, "02.02.04", ButceUygulamaResI.BUM280, true, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.YIKOBAKTARMA, "02.02.05", "YIKOB Yıl İçi Aktarma", true, true));

            islemListesi.Add(new IslemBilgi(ENUMIslemKod.ANAIPTAL, "03", ButceUygulamaResI.BUM281, false, true));
            islemListesi.Add(new IslemBilgi(ENUMIslemKod.IPTAL, "03.01", ButceUygulamaResI.BUM282, true, true));
        }

        /// <summary>
        /// Parametre olarak verilen İşlem kodu bilgisini, İşlem listesi dizisinde arayarak,
        /// aynı işlem koduna sahip işlem listesi dizi elemanının işlem kodunu döndüren fonksiyon
        /// </summary>
        /// <param name="enumIslemKod">İşlem kodu bilgisini tutan, ENUMIslemKod türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İşlem kodu bilgisini tutan String türünde nesne döndürür</returns>
        public static string IslemKod(ENUMIslemKod enumIslemKod)
        {
            for (int i = 0; i < islemListesi.Count; i++)
            {
                IslemBilgi ib = (IslemBilgi)islemListesi[i];
                if (enumIslemKod == ib.enumIK)
                    return ib.islemKod;

            }
            return "";
        }

        /// <summary>
        /// Parametre olarak verilen fonksiyon kodunu split ederek Kurum kodunu döndüren fonksiyon
        /// </summary>
        /// <param name="fonksiyonelKod">Fonksiyonel kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kurum kodunu tutan String türünde nesne döndürür</returns>
        public static string HazineYardimiTertibindenKurumBul(string fonksiyonelKod)
        {
            string[] kirilimlar = fonksiyonelKod.Split('.');
            if (kirilimlar.Length == 4)
            {
                if (kirilimlar[2] == "1")
                    return "38." + kirilimlar[3];
                else if (kirilimlar[2] == "2")
                    return "40." + kirilimlar[3];
            }
            return "";
        }


        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum koduna göre,
        /// Cari ve Sermaye kodlarının tertiplerini oluşturup döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="cariKodlar">Cari kodlar tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="sermayeKodlar">Sermaye kodlar tertip bilgisini tutan, Tertip türünde nesne</param>
        public static void HazineYardimiTertipleriBul(int yil, string kurumKod, ref Tertip cariKodlar, ref Tertip sermayeKodlar)
        {
            string kurKod = kurumKod.Replace(".", "");
            if (kurKod.Length != 4)
                return;

            string yardimKurKod = "";
            string yardimSermaye = "";
            string fonkKod = "";
            string finKod = "1";
            //string ekoKod="";
            string sermayeEkoKod = "", cariEkoKod = "";


            if (kurKod.Substring(0, 2) == "38")
            {
                yardimKurKod = "12.01.31.00";
                fonkKod = "01.6.1." + kurKod.Substring(2, 2);
                cariEkoKod = "05.2.7." + kurKod.Substring(2, 2);
                sermayeEkoKod = "07.1.7." + kurKod.Substring(2, 2);
            }
            else
            {
                cariEkoKod = "05.2.2." + kurKod.Substring(2, 2);
                sermayeEkoKod = "07.1.2." + kurKod.Substring(2, 2);
                fonkKod = "01.6.2." + kurKod.Substring(2, 2);
                //ekoKod="01.6.1." + kurKod.Substring(2, 2);
                if (kurKod == "4001") yardimKurKod = "01.01.00.04";
                else if (kurKod == "4002") yardimKurKod = "07.01.00.04";
                else if (kurKod == "4007") yardimKurKod = "10.01.00.23";
                else if (kurKod == "4008") yardimKurKod = "07.01.00.04";
                else if (kurKod == "4009") yardimKurKod = "07.01.00.04";
                else if (kurKod == "4010")
                {
                    yardimKurKod = "08.01.00.04";
                    yardimSermaye = "08.01.00.13";
                }
                else if (kurKod == "4013") yardimKurKod = "13.01.39.00";
                else if (kurKod == "4014") yardimKurKod = "07.01.00.04";
                else if (kurKod == "4015") yardimKurKod = "21.01.30.00";
                else if (kurKod == "4016") yardimKurKod = "21.01.30.00";
                else if (kurKod == "4017") yardimKurKod = "22.01.00.04";
                else if (kurKod == "4018") yardimKurKod = "12.01.31.00";
                else if (kurKod == "4019") yardimKurKod = "12.01.31.00";
                else if (kurKod == "4021") yardimKurKod = "19.01.36.00";
                else if (kurKod == "4023") yardimKurKod = "12.01.31.00";
                else if (kurKod == "4026") yardimKurKod = "20.01.30.00";
                else if (kurKod == "4027") yardimKurKod = "20.01.31.00";
                else if (kurKod == "4028") yardimKurKod = "09.01.30.00";
                else if (kurKod == "4030") yardimKurKod = "19.01.32.00";
                else if (kurKod == "4031") yardimKurKod = "07.83.30.00";
                else if (kurKod == "4032") yardimKurKod = "07.01.00.04";
                else if (kurKod == "4033") yardimKurKod = "22.01.00.04";
                else if (kurKod == "4034") yardimKurKod = "07.01.00.04";
                else if (kurKod == "4039") yardimKurKod = "20.01.31.00";
                else if (kurKod == "4040") yardimKurKod = "20.01.30.00";
                else if (kurKod == "4041") yardimKurKod = "08.01.32.62";
            }
            if (yardimKurKod == "") return;
            if (yardimSermaye == "") yardimSermaye = yardimKurKod;

            cariKodlar = new Tertip(yil, ENUMTabloKod.TABOdenek, yardimKurKod, fonkKod, finKod, cariEkoKod);
            sermayeKodlar = new Tertip(yil, ENUMTabloKod.TABOdenek, yardimKurKod, fonkKod, finKod, sermayeEkoKod);
        }
    }

    /// <summary>
    /// Tertip Bilgi Sınıfı
    /// </summary>
    [Serializable()]
    public class TertipBilgi
    {
        /// <summary>
        /// Tertip
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Tertip adı
        /// </summary>
        public string tertipAd;
        /// <summary>
        /// Kesintili başlangıç ödeneği miktarı
        /// </summary>
        public double kbo;
        /// <summary>
        /// Serbest miktarı
        /// </summary>
        public double serbest;
        /// <summary>
        /// Ay değerlerini tutan dizi
        /// </summary>
        public double[] ayDegerler = new double[12];
        /// <summary>
        /// Ay oranlarını tutan dizi
        /// </summary>
        public double[] ayOranlar = new double[12];
    }

    /// <summary>
    /// Bütçe İşlem Tertip Bilgi Sınıfı
    /// </summary>
    [Serializable()]
    public class BITertipBilgi
    {
        /// <summary>
        /// Eklenen miktar
        /// </summary>
        public double eklenen;
        /// <summary>
        /// Düşülen miktar
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Serbest miktar
        /// </summary>
        public double serbest;
        /// <summary>
        /// Gönderilen miktar
        /// </summary>
        public double gonderilen;
        /// <summary>
        /// Tenkis edilen miktar
        /// </summary>
        public double tenkisEdilen;
        /// <summary>
        /// Harcanan miktar
        /// </summary>
        public double harcanan;
    }


    /// <summary>
    /// Bütçe İşlem Form Durum Sınıfı
    /// </summary>
    [Serializable()]
    public class BIFormDurum
    {
        /// <summary>
        /// Durum kodu
        /// </summary>
        public ENUMFormDurum durumKod;
        /// <summary>
        /// Durum adı
        /// </summary>
        public string durumAd;
        /// <summary>
        /// Parametre olarak verilen durum koduna göre Form durum adını döndüren fonksiyon
        /// </summary>
        /// <param name="durumKod">Durum kodu bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Form Durum adını tutan String türünde nesne döndürür</returns>
        public static string FormDurumAdBul(int durumKod)
        {
            if (durumKod == (int)ENUMFormDurum.BOSKAYIT)
                return ButceUygulamaResI.BUM283;
            else if (durumKod == (int)ENUMFormDurum.BUTCEISLEMIPTAL)
                return ButceUygulamaResI.BUM284;
            else if (durumKod == (int)ENUMFormDurum.DYDOGRU)
                return ButceUygulamaResI.BUM285;
            else if (durumKod == (int)ENUMFormDurum.DYYANLIS)
                return ButceUygulamaResI.BUM286;
            else if (durumKod == (int)ENUMFormDurum.ONAYLI)
                return ButceUygulamaResI.BUM287;
            else if (durumKod == (int)ENUMFormDurum.SAYISTAYAGONDERILDI)
                return ButceUygulamaResI.BUM287;
            else if (durumKod == (int)ENUMFormDurum.SAYISTAYDA)
                return ButceUygulamaResI.BUM288;
            else if (durumKod == (int)ENUMFormDurum.SAYISTAYRED)
                return ButceUygulamaResI.BUM289;
            else if (durumKod == (int)ENUMFormDurum.SILINDI)
                return ButceUygulamaResI.BUM290;
            else if (durumKod == (int)ENUMFormDurum.YENIKAYIT)
                return ButceUygulamaResI.BUM291;
            else
                return "";
        }

        /// <summary>
        /// Parametre olarak verilen durum kodunu local durum kodu bilgisine kontrol ederek atayan fonksiyon
        /// </summary>
        /// <param name="i_durumKod">Durum kodu bilgisini tutan, Int türünde nesne</param>
        public BIFormDurum(int i_durumKod)
        {
            if (i_durumKod == (int)ENUMFormDurum.BOSKAYIT)
            {
                durumKod = ENUMFormDurum.BOSKAYIT;
            }
            else if (i_durumKod == (int)ENUMFormDurum.BUTCEISLEMIPTAL)
            {
                durumKod = ENUMFormDurum.BUTCEISLEMIPTAL;
            }
            else if (i_durumKod == (int)ENUMFormDurum.DYDOGRU)
            {
                durumKod = ENUMFormDurum.DYDOGRU;
            }
            else if (i_durumKod == (int)ENUMFormDurum.DYYANLIS)
            {
                durumKod = ENUMFormDurum.DYYANLIS;
            }
            else if (i_durumKod == (int)ENUMFormDurum.ONAYLI)
            {
                durumKod = ENUMFormDurum.ONAYLI;
            }
            else if (i_durumKod == (int)ENUMFormDurum.SAYISTAYAGONDERILDI)
            {
                durumKod = ENUMFormDurum.SAYISTAYAGONDERILDI;
            }
            else if (i_durumKod == (int)ENUMFormDurum.SAYISTAYDA)
            {
                durumKod = ENUMFormDurum.SAYISTAYDA;
            }
            else if (i_durumKod == (int)ENUMFormDurum.SAYISTAYRED)
            {
                durumKod = ENUMFormDurum.SAYISTAYRED;
            }
            else if (i_durumKod == (int)ENUMFormDurum.SILINDI)
            {
                durumKod = ENUMFormDurum.SILINDI;
            }
            else if (i_durumKod == (int)ENUMFormDurum.YENIKAYIT)
            {
                durumKod = ENUMFormDurum.YENIKAYIT;
            }
            durumAd = FormDurumAdBul(i_durumKod);
        }
    }

    /// <summary>
    /// Bütçe İşlem Satır Detay Sınıfı
    /// </summary>
    [Serializable()]
    public class BISatirDetayli : BISatir
    {
        /// <summary>
        /// Satir Detay Sınıfının oluşturucu fonksiyonu. Yıldız yer değişkenini sıfırlar.
        /// </summary>
        public BISatirDetayli()
        {
            yildizYer = 0;
        }

        /// <summary>
        /// Kesintili Başlangıç Ödeneği miktarı
        /// </summary>
        public double kbo;
        /// <summary>
        /// Onaylı Tertip bilgisi
        /// </summary>
        public BITertipBilgi tertipBilgiOnayli;
        /// <summary>
        /// Onaysız Tertip bilgisi
        /// </summary>
        public BITertipBilgi tertipBilgiOnaysiz;
    }



    /// <summary>
    /// Bütçe İşlem Satır Sınıfı
    /// </summary>
    [Serializable()]
    public class BISatir
    {
        /// <summary>
        /// Bütçe İşlem Satır sınıfının oluşturucu fonksiyonu. Yıldız yer değişkenini sıfırlar
        /// </summary>
        public BISatir()
        {
            yildizYer = 0;
        }

        /// <summary>
        /// Tertip
        /// </summary>
        public Tertip tertip;
        /// <summary>
        /// Eklenen serbest miktarı
        /// </summary>
        public double eklenen_serbest; // Ödenek aktarmaları veya eklemelerinde eklenen anlamında, serbest işleminde serbest anlammında kullanılır.
        /// <summary>
        /// Düşülen miktar
        /// </summary>
        public double dusulen;
        /// <summary>
        /// Yıldız yeri
        /// </summary>
        public int yildizYer;
        /// <summary>
        /// Aylık program miktarı
        /// </summary>
        public double[] aylikProgram;
        /// <summary>
        /// Devreden tertip
        /// </summary>
        public Tertip devredenTertip;

        /// <summary>
        /// Miktar. Eklenen Serbest'ten düşülen düşüldükten sonra eğer değer 0'dan büyükse değer eklenen serbeste atanır, düşülen sıfırlanır, değilse eklenen serbest 0 sıfırlanır düşülene - değer atanır.
        /// </summary>
        public double miktar
        {
            get
            {
                return eklenen_serbest - dusulen;
            }
            set
            {
                if (value > 0)
                {
                    eklenen_serbest = value;
                    dusulen = 0;
                }
                else
                {
                    eklenen_serbest = 0;
                    dusulen = 0 - value;
                }
            }
        }
    }

    /// <summary>
    /// Bütçe İşlem Dayanak Sınıfı
    /// </summary>
    [Serializable()]
    public class BIDayanak
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Dayanak kodu
        /// </summary>
        public int dayanakKod;
        /// <summary>
        /// Dayanak adı
        /// </summary>
        public string dayanakAd;
        /// <summary>
        /// Açıklama
        /// </summary>
        public string aciklama;
        /// <summary>
        /// İşlem kodları dizisi
        /// </summary>
        public int[] islemKodlari;
    }


    class IslemKurumToplam
    {
        public string kurumKod;

        public double gider;
        public double giderCariHY;
        public double giderSermayeHY;
        public double giderCariHYTahmin;
        public double giderSermayeHYTahmin;

        public double gelir;
        public double gelirCariHY;
        public double gelirSermayeHY;

        public double finansman;

        public IslemKurumToplam(string p_kurum)
        {
            kurumKod = p_kurum;

            gider = 0;
            giderCariHY = 0;
            giderSermayeHY = 0;
            giderCariHYTahmin = 0;
            giderSermayeHYTahmin = 0;

            gelir = 0;
            gelirCariHY = 0;
            gelirSermayeHY = 0;

            finansman = 0;
        }

        public void GiderVeTahminEkle(string ekonomikKod, double miktar)
        {
            gider += miktar;
            if (ekonomikKod.StartsWith("07"))
                giderSermayeHYTahmin += miktar;
            else
                giderCariHYTahmin += miktar;
        }

        public void GiderHYEkle(string ekonomikKod, double miktar)
        {
            if (ekonomikKod.StartsWith("07"))
                giderSermayeHY += miktar;
            else
                giderCariHY += miktar;
        }

        public void GelirVeHYEkle(string ekonomikKod, double miktar)
        {
            gelir += miktar;
            if (ekonomikKod == "04.2.2.02")
                gelirSermayeHY += miktar;
            else if (ekonomikKod == "04.2.1.02")
                gelirCariHY += miktar;
        }

        public void FinansEkle(double miktar)
        {
            finansman += miktar;
        }
    }


    class IslemFormToplam
    {
        public TNSCollection kurumlar = new TNSCollection();

        private IslemKurumToplam KurumToplamBul(string kurum)
        {
            IslemKurumToplam ikt;
            for (int i = 0; i < kurumlar.Count; i++)
            {
                ikt = (IslemKurumToplam)kurumlar[i];
                if (ikt.kurumKod == kurum)
                    return ikt;
            }
            ikt = new IslemKurumToplam(kurum);
            kurumlar.Add(ikt);
            return ikt;
        }

        public void GiderHYEkle(string kurum, string ekoKod, double miktar)
        {
            IslemKurumToplam ikt = KurumToplamBul(kurum);
            ikt.GiderHYEkle(ekoKod, miktar);
        }

        public void GiderEkle(Tertip t, double miktar, string kurYapi)
        {
            string kurum = TNS.BKD.Arac.KurumsaldanKuruma((string)t.kodlar[0], kurYapi);
            IslemKurumToplam ikt = KurumToplamBul(kurum);
            ikt.GiderVeTahminEkle((string)t.kodlar[3], miktar);
        }

        public void GelirEkle(Tertip t, double miktar)
        {
            IslemKurumToplam ikt = KurumToplamBul((string)t.kodlar[0]);
            ikt.GelirVeHYEkle((string)t.kodlar[1], miktar);
        }

        public void FinansmanEkle(Tertip t, double miktar)
        {
            IslemKurumToplam ikt = KurumToplamBul((string)t.kodlar[0]);
            ikt.FinansEkle(miktar);
        }
        public static string Virgulle(double d)
        {
            return d.ToString("#,##0.##0");
        }

        public void ToplamKontroluYap(Sonuc sonuc)
        {
            IslemKurumToplam ikt;
            for (int i = 0; i < kurumlar.Count; i++)
            {
                ikt = (IslemKurumToplam)kurumlar[i];
                if (!BIForm.GenelButceliMi(ikt.kurumKod))
                {
                    if (Math.Round(ikt.gider, 2) != Math.Round(ikt.gelir + ikt.finansman, 2))
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ikt.kurumKod + " " + ButceUygulamaResI.BUM292 + "(" + Virgulle(ikt.gider) + "), " + ButceUygulamaResI.BUM293 + "(" + Virgulle(ikt.gelir) + ") " + ButceUygulamaResI.BUM294 + "(" + Virgulle(ikt.finansman) + ") " + ButceUygulamaResI.BUM295;
                        return;
                    }

                    if (ikt.gelirCariHY != ikt.giderCariHY)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ikt.kurumKod + " " + ButceUygulamaResI.BUM296 + "(" + Virgulle(ikt.giderCariHY) + "), " + ButceUygulamaResI.BUM297 + " (" + Virgulle(ikt.gelirCariHY) + ") " + ButceUygulamaResI.BUM298;
                        return;
                    }

                    if (ikt.gelirSermayeHY != ikt.giderSermayeHY)
                    {
                        sonuc.islemSonuc = false;
                        sonuc.hataStr = ikt.kurumKod + " " + ButceUygulamaResI.BUM299 + "(" + Virgulle(ikt.giderSermayeHY) + "), " + ButceUygulamaResI.BUM297 + " (" + Virgulle(ikt.gelirSermayeHY) + ") " + ButceUygulamaResI.BUM298;
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Bütçe işlemleri ile ilgili özet bilgilerin tutulduğu sınıf. Eğer özet içerisinde detaylar isteniyorsa, 
    /// BIOzetGider, BIOzetGelir ve BIOzetFinans yapıları da doldurulabilir.
    /// </summary>
    [Serializable()]
    public class BIForm
    {
        /// <summary>
        /// Yıl
        /// </summary>
        public int yil;
        /// <summary>
        /// Kurum
        /// </summary>
        public string kurum;
        /// <summary>
        /// İşlem kodu
        /// </summary>
        public string islemKod;
        /// <summary>
        /// Sıra no
        /// </summary>
        public int siraNo;
        /// <summary>
        /// Evrak tarihi
        /// </summary>
        public TNSDateTime evrakTarih;
        /// <summary>
        /// Evrak numarası
        /// </summary>
        public string evrakNo;
        /// <summary>
        /// İlgi
        /// </summary>
        public string ilgi;
        /// <summary>
        /// Notlar
        /// </summary>
        public string notlar;
        /// <summary>
        /// Kullanıcı kodu
        /// </summary>
        public string kullaniciKod;
        /// <summary>
        /// Kullanıcı tip kodu
        /// </summary>
        public int kullaniciTipKod;
        /// <summary>
        /// Miktar
        /// </summary>
        public double miktar;
        /// <summary>
        /// Değişiklik tarihi
        /// </summary>
        public TNSDateTime degisiklikTarih;
        /// <summary>
        /// Onay tarihi
        /// </summary>
        public TNSDateTime onayTarih;
        /// <summary>
        /// İlgili işlem kodu
        /// </summary>
        public string ilgiliIslemKod;
        /// <summary>
        /// İlgili sıra numarası
        /// </summary>
        public int ilgiliSiraNo;
        /// <summary>
        /// BÜMKO Kurum kodu
        /// </summary>
        public string bumkoKurum;
        /// <summary>
        /// BÜMKO Sıra No
        /// </summary>
        public int bumkoSiraNo;

        /// <summary>
        /// Form durumu
        /// </summary>
        public BIFormDurum formDurum;

        /// <summary>
        /// BIDayanak yapısında elemanlar bulunur.
        /// </summary>
        public TNSCollection dayanaklar;

        ///// <summary>
        ///// Üçgen istenirse UcgenSatir, Özet istenirse BIOzetDetay, Form istenirse BIFormDetay yapısında elemanlar bulunacak.
        ///// </summary>
        //public TNSCollection finanslar;

        /// <summary>
        /// Üçgen istenirse UcgenSatir, Özet istenirse BIOzetDetay, Form istenirse BIFormDetay yapısında elemanlar bulunacak.
        /// </summary>
        public TNSCollection giderler;

        ///// <summary>
        ///// Üçgen istenirse UcgenSatir, Özet istenirse BIOzetDetay, Form istenirse BIFormDetay yapısında elemanlar bulunacak.
        ///// </summary>
        //public TNSCollection gelirler;


        /// <summary>
        /// Form Durum Grubu
        /// </summary>
        public ENUMFormDurumGrup formDurumGrup;
        /// <summary>
        /// Sınıfın oluşturucu fonksiyonu. Parametre olarak aldığı değerleri lokal değişkenlere atar.
        /// </summary>
        /// <param name="p_yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="p_kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="p_islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="p_siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        public BIForm(int p_yil, string p_kurum, string p_islemKod, int p_siraNo)
        {
            yil = p_yil;
            kurum = p_kurum;
            islemKod = p_islemKod;
            siraNo = p_siraNo;
            ilgiliIslemKod = "";
            ilgiliSiraNo = 0;
            bumkoKurum = "";
            bumkoSiraNo = -1;
        }

        /// <summary>
        /// Form durumuna göre Yeni olup olmadığını dönen fonksiyon. Form durumu boş ve yeni kayıt ise true, değilse false döner.
        /// </summary>
        /// <returns></returns>
        public bool YeniMi()
        {
            if (formDurum == null) return true;
            if (formDurum.durumKod <= ENUMFormDurum.YENIKAYIT) return true;
            return false;
        }

        /// <summary>
        /// Form durumuna göre Onaylı olup olmadığını dönen fonksiyon. Form durumu Dayanak Doğru'dan büyük ve Onaylıdan küçük ise true değilse false döner.
        /// </summary>
        /// <returns></returns>
        public bool OnayliMi()
        {
            if (formDurum == null) return false;
            if (formDurum.durumKod >= ENUMFormDurum.DYDOGRU && formDurum.durumKod <= ENUMFormDurum.ONAYLI) return true;
            return false;
        }

        /// <summary>
        /// Form durumuna göre İptal olup olmadığını dönen fonksiyon. Form durumu Sayıştay Red'den büyükse true değilse false döner.
        /// </summary>
        /// <returns></returns>
        public bool IptalliMi()
        {
            if (formDurum == null) return false;
            if (formDurum.durumKod >= ENUMFormDurum.SAYISTAYRED) return true;
            return false;
        }

        /// <summary>
        /// Parametre olarak aldığı kurum koduna Genel Bütçeli olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Genel bütçeli olup olmadığı bilgisini tutan Bool türünde nesne döndürür</returns>
        public static bool GenelButceliMi(string kurum)
        {
            return kurum.CompareTo(TNS.BKD.Arac.GenelButceSonKod()) < 0;
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre İşlemin, Devir işlemi olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Devir İşlemi ise True, değilse False değerini tutan Bool türünde nesne döndürür</returns>
        public static bool DevirIslemiMi(string islemKod)
        {
            if (islemKod == BISabitler.IslemKod(ENUMIslemKod.AKREDITIF) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.DEVREDENDISPROJE) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.DEVRENGELIRLI) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.TAAHHUTARTIGI))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre İşlemin, Revize işlemi olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Revize İşlemi ise True, değilse False değerini tutan Bool türünde nesne döndürür</returns>
        public static bool RevizeIslemiMi(string islemKod)
        {
            if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)) ||
                islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre İşlemin, Yedekli işlem olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yedekli İşlem ise True, değilse False değerini tutan Bool türünde nesne döndürür</returns>
        public static bool YedekliIslemMi(string islemKod)
        {
            if (islemKod == BISabitler.IslemKod(ENUMIslemKod.YEDEGEAKTARMA) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.YEDEKTENAKTARMA))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre İşlemin, Hazine Yardımı işlemi olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Hazine Yardımı İşlemi ise True, değilse False değerini tutan Bool türünde nesne döndürür</returns>
        public static bool HazineYardimliIslemMi(string islemKod)
        {
            if (islemKod == BISabitler.IslemKod(ENUMIslemKod.KURUMLARARASIAKTARMA) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.YEDEGEAKTARMA) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.YEDEKTENAKTARMA))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre Gider olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="kurumKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bool türünde nesne döndürür</returns>
        public static bool GiderVarMi(string islemKod, string kurumKod)
        {
            if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IPTAL)))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre Gelir olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="kurumKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bool türünde nesne döndürür</returns>
        public static bool GelirVarMi(string islemKod, string kurumKod)
        {
            if (islemKod == BISabitler.IslemKod(ENUMIslemKod.ODENEKEKLEME) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.GELIRLI) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.GELIRFAZLASI))
            {
                if (BIForm.GenelButceliMi(kurumKod))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen İşlem koduna göre Finansman olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="kurumKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bool türünde nesne döndürür</returns>
        public static bool FinansmanVarMi(string islemKod, string kurumKod)
        {
            if (BIForm.DevirIslemiMi(islemKod) ||
                islemKod == BISabitler.IslemKod(ENUMIslemKod.LIKITKARSILIGI))
            {
                if (BIForm.GenelButceliMi(kurumKod))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Parametre olarak verilen Yedek Tertipleri listesini bakarak, ilgili tertip kodunun olup olmadığı dönen fonksiyon.
        /// </summary>
        /// <param name="tertipStr">Tertip bilgisini tutan, String türünde nesne</param>
        /// <param name="yedekTertipleri">Yedek Tertip bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, verilen tertip bilgisi yedek tertiplerin içinde var ise true değilse false değerini tutan Bool türünde nesne döndürür</returns>
        private bool YedekTertibiMi(string tertipStr, TNSCollection yedekTertipleri)
        {
            for (int i = 0; i < yedekTertipleri.Count; i++)
            {
                if (((ListeSatir)yedekTertipleri[i]).kod == tertipStr)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Parametre olarak verilen Kurum kodunu Kurumlar listesinde bakarak eğer yok ise ilgili kurum listesine ekleyen fonksiyon.
        /// </summary>
        /// <param name="kurumlar">Kurumlar listesini tutan, TNSCollection türünde nesne</param>
        /// <param name="kurumKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        private void KurumYoksaEkle(TNSCollection kurumlar, string kurumKod)
        {
            for (int i = 0; i < kurumlar.Count; i++)
            {
                if (((string)kurumlar[i]) == kurumKod)
                    return;
            }
            kurumlar.Add(kurumKod);
        }

        /// <summary>
        /// Parametre olarak verilen İşlem Form toplamına Gelir ve Giderlere göre ekleme yapan fonksiyon.
        /// </summary>
        /// <param name="ft">İşlem Form toplamını tutan, IslemFormToplam türünde nesne</param>
        private void KurumToplamlariAl(IslemFormToplam ft)
        {
            if (giderler != null)
            {
                Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

                for (int i = 0; i < giderler.Count; i++)
                {
                    BISatir s = (BISatir)giderler[i];
                    if (!BIForm.GiderHazineYardimiMi(s.tertip))
                    {
                        ft.GiderEkle(s.tertip, s.miktar, tUzBul.Kirilimlar(0));
                    }
                    else
                    {
                        ft.GiderHYEkle(BISabitler.HazineYardimiTertibindenKurumBul((string)s.tertip.kodlar[1]), (string)s.tertip.kodlar[3], s.miktar);
                    }
                }
            }
            //if (gelirler != null)
            //    for (int i = 0; i < gelirler.Count; i++)
            //    {
            //        BISatir s = (BISatir)gelirler[i];
            //        ft.GelirEkle(s.tertip, s.miktar);
            //    }

            //if (finanslar != null)
            //    for (int i = 0; i < finanslar.Count; i++)
            //    {
            //        BISatir s = (BISatir)finanslar[i];
            //        ft.FinansmanEkle(s.tertip, s.miktar);
            //    }
        }

        /// <summary>
        /// Kurum toplamlarını aldıktan sonra kontrolünü yapan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="sonuc">İşlem sonucunu tutan, Sonuç türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İşlem sonucunu tutan Bool türünde nesne döndürür</returns>
        public bool KurumToplamlariDogruMu(Sonuc sonuc)
        {
            IslemFormToplam ft = new IslemFormToplam();
            KurumToplamlariAl(ft);
            ft.ToplamKontroluYap(sonuc);
            return sonuc.islemSonuc;
        }

        /// <summary>
        /// Ayrıntılı Finansman Programı toplamlarını kontrol eden fonksiyon.
        /// </summary>
        /// <param name="sonuc">İşlem sonucunu tutan, Sonuc türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İşlem sonucunu tutan Bool türünde nesne döndürür</returns>
        public bool AFPToplamlariDogruMu(Sonuc sonuc)
        {

            if (GenelButceliMi(kurum))
                return true;
            for (int i = 0; i < 12; i++)
            {
                double toplam = 0;
                if (giderler != null)
                    for (int j = 0; j < giderler.Count; j++)
                    {
                        BISatir satir = (BISatir)giderler[j];
                        //if (satir.tertip.tabloKod == ENUMTabloKod.TABGiderIcmal)
                        //{
                        //    toplam = toplam - ((double)satir.aylikProgram[i]);
                        //}
                    }
                //if (gelirler != null)
                //    for (int j = 0; j < gelirler.Count; j++)
                //    {
                //        BISatir satir = (BISatir)gelirler[j];
                //        toplam = toplam + ((double)satir.aylikProgram[i]);
                //    }
                //if (finanslar != null)
                //    for (int j = 0; j < finanslar.Count; j++)
                //    {
                //        BISatir satir = (BISatir)finanslar[j];
                //        toplam = toplam + ((double)satir.aylikProgram[i]);
                //    }
                if (toplam < 0)
                {
                    sonuc.islemSonuc = false;
                    sonuc.hataStr = ButceUygulamaResI.BUM300 + ", " + (i + 1).ToString() + ". " + ButceUygulamaResI.BUM301;
                    return sonuc.islemSonuc;
                }
            }
            return sonuc.islemSonuc;
        }

        /// <summary>
        /// Hazine yardımlarını ekleyen fonksiyon.
        /// </summary>
        /// <param name="sonuc">İşlem sonucunu tutan, Sonuc türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İşlem sonucunu tutan Bool türünde nesne döndürür</returns>
        //public bool HazineYardimlariEkle(Sonuc sonuc)
        //{
        //    if (islemKod == BISabitler.IslemKod(ENUMIslemKod.GELIRLI) || islemKod == BISabitler.IslemKod(ENUMIslemKod.DEVRENGELIRLI))
        //    {
        //        sonuc.hataStr = ButceUygulamaResI.BUM302;
        //        sonuc.islemSonuc = false;
        //        return false;
        //    }
        //    IslemKurumToplam ikt;
        //    IslemFormToplam ft = new IslemFormToplam();
        //    KurumToplamlariAl(ft);
        //    if (ft.kurumlar.Count == 0)
        //    {
        //        sonuc.hataStr = ButceUygulamaResI.BUM303;
        //        sonuc.islemSonuc = false;
        //        return false;
        //    }

        //    for (int i = 0; i < ft.kurumlar.Count; i++)
        //    {
        //        ikt = (IslemKurumToplam)ft.kurumlar[i];

        //        if (ikt.finansman + ikt.gelir + ikt.gelirCariHY + ikt.gelirSermayeHY + ikt.giderCariHY + ikt.giderSermayeHY != 0)
        //        {
        //            sonuc.hataStr = ButceUygulamaResI.BUM304;
        //            sonuc.islemSonuc = false;
        //            return false;
        //        }
        //    }

        //    BISatir formSatir;
        //    for (int i = 0; i < ft.kurumlar.Count; i++)
        //    {
        //        ikt = (IslemKurumToplam)ft.kurumlar[i];
        //        Tertip[] cariSermaye = new Tertip[2];
        //        BISabitler.HazineYardimiTertipleriBul(yil, ikt.kurumKod, ref cariSermaye[0], ref cariSermaye[1]);
        //        if (cariSermaye[0] != null)
        //        {
        //            if (ikt.giderCariHYTahmin != 0)
        //            {
        //                formSatir = new BISatir();
        //                formSatir.tertip = new Tertip(yil, ENUMTabloKod.TABButce, cariSermaye[0].kodlar[0], cariSermaye[0].kodlar[1], cariSermaye[0].kodlar[2], cariSermaye[0].kodlar[3].Substring(0, 4));
        //                formSatir.miktar = ikt.giderCariHYTahmin;
        //                formSatir.yildizYer = 0;
        //                giderler.Add(formSatir);

        //                formSatir = new BISatir();
        //                formSatir.tertip = new Tertip(yil, ENUMTabloKod.TABGelir, ikt.kurumKod, "04.2.1.02");
        //                formSatir.miktar = ikt.giderCariHYTahmin;
        //                formSatir.yildizYer = 0;
        //                gelirler.Add(formSatir);
        //            }
        //            if (ikt.giderSermayeHYTahmin != 0)
        //            {
        //                formSatir = new BISatir();
        //                formSatir.tertip = new Tertip(yil, ENUMTabloKod.TABButce, cariSermaye[1].kodlar[0], cariSermaye[1].kodlar[1], cariSermaye[1].kodlar[2], cariSermaye[1].kodlar[3].Substring(0, 4));
        //                formSatir.miktar = ikt.giderSermayeHYTahmin;
        //                formSatir.yildizYer = 0;
        //                giderler.Add(formSatir);

        //                formSatir = new BISatir();
        //                formSatir.tertip = new Tertip(yil, ENUMTabloKod.TABGelir, ikt.kurumKod, "04.2.2.02");
        //                formSatir.miktar = ikt.giderSermayeHYTahmin;
        //                formSatir.yildizYer = 0;
        //                gelirler.Add(formSatir);
        //            }
        //        }
        //    }
        //    return sonuc.islemSonuc;
        //}

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum bilgisine göre Uzay servisine bağlanarak, Kurumun yeni kurum olup olmadığı bilgisini döndüren fonksiyon.
        /// </summary>
        /// <param name="servis">Servis bilgisini tutan, IUZYServis türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yeni kurum olup olmadığı bilgisini tutan Bool türünde nesne döndürür</returns>
        public static bool YeniKurumMu(IUZYServis servis, int yil, string kurum)
        {
            return servis.UzayDegeriStr(null, "ABSKUR", (yil - 1).ToString() + "-" + kurum, true) == "";
        }

        /// <summary>
        /// Parametre olarak verilen Yedek Tertipleri bilgisine göre, Uzay Servisine bağlanarak Kurum Sayısının doğru olup olmadığı bilgisini döndüren fonksiyon.
        /// </summary>
        /// <param name="servis">Servis bilgisini tutan, IUZYServis türünde nesne</param>
        /// <param name="sonuc">Sonuc bilgisini tutan, Sonuc türünde nesne</param>
        /// <param name="yedekTertipleri">Yedek tertipleri tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kurum sayısının doğru olup olmadığı bilgisini tutan Bool türünde nesne döndürür</returns>
        public bool KurumSayisiDogruMu(IUZYServis servis, Sonuc sonuc, TNSCollection yedekTertipleri)
        {
            Tertip tUzBul = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);

            TNSCollection kurumlar = new TNSCollection();
            for (int i = 0; i < giderler.Count; i++)
            {
                BISatir s = (BISatir)giderler[i];
                //bool ekle = true;
                //if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.YEDEKTENAKTARMA)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.YEDEGEAKTARMA)))
                //    if (YedekTertibiMi(s.tertip.TertipStrOl(), yedekTertipleri))
                //        ekle = false;
                //if (ekle)
                //    if (GiderHazineYardimiMi(s.tertip) && ((islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.ANAEKLEMEAKTARMA)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))))
                //        ekle = false;
                //if (ekle)
                //{
                KurumYoksaEkle(kurumlar, TNS.BKD.Arac.KurumsaldanKuruma((string)s.tertip.kodlar[0], tUzBul.Kirilimlar(0)));
                //}
            }
            //if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.KURUMLARARASIAKTARMA)))
            //{
            //    if (kurumlar.Count != 2)
            //    {
            //        sonuc.islemSonuc = false;
            //        sonuc.hataStr = ButceUygulamaResI.BUM305;
            //        return false;
            //    }
            //    if (!GenelButceliMi((string)kurumlar[0]) || !GenelButceliMi((string)kurumlar[1]))
            //    {
            //        if (!YeniKurumMu(servis, yil, (string)kurumlar[0]) && !YeniKurumMu(servis, yil, (string)kurumlar[1]))
            //        {
            //            sonuc.islemSonuc = false;
            //            sonuc.hataStr = ButceUygulamaResI.BUM306;
            //            return false;
            //        }
            //    }

            //    for (int i = 0; i < giderler.Count; i++)
            //    {
            //        BISatir s = (BISatir)giderler[i];
            //        if (s.dusulen > 0)
            //        {
            //            if (YedekTertibiMi(s.tertip.TertipStrOl(), yedekTertipleri))
            //            {
            //                sonuc.islemSonuc = false;
            //                sonuc.hataStr = ButceUygulamaResI.BUM307;
            //                return false;
            //            }
            //            if (!GiderHazineYardimiMi(s.tertip))
            //            {
            //                if (kurum != TNS.BKD.Arac.KurumsaldanKuruma((string)s.tertip.kodlar[0]))
            //                {
            //                    sonuc.islemSonuc = false;
            //                    sonuc.hataStr = ButceUygulamaResI.BUM308+" (" + kurum + ") "+ButceUygulamaResI.BUM309+" ("+ButceUygulamaResI.BUM310 + s.tertip.TertipStrOl() + ")!";
            //                    return false;
            //                }
            //            }
            //        }
            //    }

            //}
            //else if (kurumlar.Count != 1 && !(kurumlar.Count == 0 && islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE))))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = ButceUygulamaResI.BUM311;
            //    return false;
            //}
            //if (!kurumlar.Contains(kurum) && !islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
            //{
            //    sonuc.islemSonuc = false;
            //    sonuc.hataStr = ButceUygulamaResI.BUM312+" (" + kurum + ") "+ButceUygulamaResI.BUM313;
            //    return false;
            //}

            return kurumlar.Count == 1;
            //return true;
        }

        /// <summary>
        /// Parametre olarak verilen Dizide, dizi elemanlarını karşılaştırarak aynı satırın olup olmadığını ve eğer varsa hangi satırlar ve tertip olduğu bilgisini dönen fonksiyon.
        /// </summary>
        /// <param name="dizi">Dizi bilgisini tutan, TNSCollection türünde nesne</param>
        /// <param name="satir1">Satır bilgisini tutan, Int türünde nesne</param>
        /// <param name="satir2">Satır bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Aynı Satır var ise true, yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public bool AyniSatirVarMi(TNSCollection dizi, out int satir1, out int satir2, out string tertip)
        {
            satir1 = -1;
            satir2 = -1;
            tertip = "";
            if (dizi != null && dizi.Count > 1)
            {
                for (int i = 0; i < dizi.Count - 1; i++)
                {
                    string kaynakTertip = ((BISatir)dizi[i]).tertip.TertipStrOl();
                    for (int j = i + 1; j < dizi.Count; j++)
                    {
                        if (kaynakTertip == ((BISatir)dizi[j]).tertip.TertipStrOl())
                        {
                            satir1 = i + 1;
                            satir2 = j + 1;
                            tertip = kaynakTertip;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Aynı Satır Var mı rutinini kullanarak, Giderler, Gelirler, Finanslar dizilerini kontrol eden ve ve aynı satır var ise hangi dizide olduğu ve satır değerleri ve tertip bilgilerini döndüren fonksiyon.
        /// </summary>
        /// <param name="yer">Yer bilgisini tutan, Int türünde nesne</param>
        /// <param name="satir1">Satır bilgisini tutan, Int türünde nesne</param>
        /// <param name="satir2">Satır bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Aynı satır var ise true yoksa false değerini tutan Bool türünde nesne döndürür</returns>
        public bool AyniSatirVarMi(out int yer, out int satir1, out int satir2, out string tertip)
        {
            yer = 1;
            AyniSatirVarMi(giderler, out satir1, out satir2, out tertip);
            //if (satir1 == -1)
            //{
            //    yer = 2;
            //    AyniSatirVarMi(gelirler, out satir1, out satir2, out tertip);
            //}
            //if (satir1 == -1)
            //{
            //    yer = 3;
            //    AyniSatirVarMi(finanslar, out satir1, out satir2, out tertip);
            //}

            if (satir1 != -1)
                return true;
            else
            {
                yer = 0;
                return false;
            }
        }

        /// <summary>
        /// Parametre olarak verilen dizide Boş satır olup olmadığını var ise hangi satır olduğunu dönen fonksiyon.
        /// </summary>
        /// <param name="dizi">Dizi bilgisini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, boş satır var ise hangi satır olduğunu tutan String türünde nesne döndürür</returns>
        public string BosSatirVarMi(TNSCollection dizi)
        {
            if (dizi != null && dizi.Count > 0)
            {
                for (int i = 0; i < dizi.Count; i++)
                {
                    BISatir kaynak = (BISatir)dizi[i];
                    int satir = i + 1;
                    if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AHPREVIZE)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
                    {
                        if (kaynak.aylikProgram == null)
                            return satir + ButceUygulamaResI.BUM314;
                        int degerSay = 0;
                        for (int ay = 0; ay < 12; ay++)
                            if ((double)kaynak.aylikProgram[ay] != 0) degerSay++;
                        if (degerSay == 0)
                            return satir + ButceUygulamaResI.BUM315;
                    }
                    else
                    {
                        if (kaynak.eklenen_serbest < 0 || kaynak.dusulen < 0)
                            return satir + ButceUygulamaResI.BUM316;
                        if (kaynak.eklenen_serbest == 0 && kaynak.dusulen == 0)
                            return satir + ButceUygulamaResI.BUM317;
                        if ((islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.SERBEST)) || islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.EKLEME))) && kaynak.dusulen > 0)
                            return satir + ButceUygulamaResI.BUM318;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// BosSatirVarmi rutinini Giderler, Gelirler, Finanslar dizileri için çalıştırarak sonucunu döndüren fonksiyon.
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Boş Satır var ise hangi satır olduğu bilgisini tutan String türünde nesne döndürür</returns>
        public string BosSatirVarMi()
        {
            string hataStr;
            if ((hataStr = BosSatirVarMi(giderler)) != "")
                return hataStr;

            //if ((hataStr = BosSatirVarMi(gelirler)) != "")
            //    return hataStr;

            //if ((hataStr = BosSatirVarMi(finanslar)) != "")
            //    return hataStr;
            return "";
        }

        /// <summary>
        /// Parametre olarak verilen dizide, Hatalı Tertip olup olmadığını kontrol eden fonksiyon. Eğer hatalı tertip var ise, hangi satır ve hangi tertip olduğu bilgilerini döner.
        /// </summary>
        /// <param name="dizi">Dizi bilgisini tutan, TNSCollection türünde nesne</param>
        /// <param name="satir">Satır bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, String türünde nesne</param>
        /// <param name="devirKontrol">Devir kontrolünün yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Hatalı tertip olup olmadığı bilgisini tutan Bool türünde nesne döndürür</returns>
        public bool HataliTertipVarMi(TNSCollection dizi, out int satir, out string tertip, bool devirKontrol)
        {
            satir = -1;
            tertip = "";
            if (dizi != null && dizi.Count > 0)
            {
                for (int i = 0; i < dizi.Count; i++)
                {
                    BISatir kaynak = (BISatir)dizi[i];
                    if (!kaynak.tertip.TamMi())
                    {
                        tertip = kaynak.tertip.TertipStrOl();
                        satir = i + 1;
                        return true;
                    }
                    if (devirKontrol)
                    {
                        if (!kaynak.devredenTertip.TamMi())
                        {
                            tertip = kaynak.devredenTertip.TertipStrOl() + ButceUygulamaResI.BUM319;
                            satir = i + 1;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// HataliTertipVarmi rutinini Giderler, Gelirler, Finanslar dizileri için çalıştırak Hatalı Tertip olup olmadığı var ise hangi dizide olduğu ile satır ve tertip bilgilerini döndüren fonksiyon.
        /// </summary>
        /// <param name="yer">Yer bilgisini tutan, Int türünde nesne</param>
        /// <param name="satir">Satır bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Hatalı Tertip var ise true, yok ise false değerini tutan Bool türünde nesne döndürür</returns>
        public bool HataliTertipVarMi(out int yer, out int satir, out string tertip)
        {
            yer = 1;
            HataliTertipVarMi(giderler, out satir, out tertip, DevirIslemiMi(islemKod));
            //if (satir == -1)
            //{
            //    yer = 2;
            //    HataliTertipVarMi(gelirler, out satir, out tertip, false);
            //}
            //if (satir == -1)
            //{
            //    yer = 3;
            //    HataliTertipVarMi(finanslar, out satir, out tertip, false);
            //}

            if (satir != -1)
                return true;
            else
            {
                yer = 0;
                return false;
            }
        }

        /// <summary>
        /// İşlem kodu ve Giderler sayısına göre Detay olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda,  IPTAL olmayan kayıtların giderler dizisi eleman sayısı 0 veya null ise false değilse true değerini tutan Bool türünde nesne döndürür</returns>
        public bool DetayVarMi()
        {
            if (islemKod != BISabitler.IslemKod(ENUMIslemKod.IPTAL))
            {
                if (giderler == null || giderler.Count == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Dayanaklar dizisinin eleman sayısına göre Dayanak olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <returns>Fonksiyonunun çalışması sonucunda, Dayanak dizisini eleman sayısı 0'dan fazla ise true değilse false değerini tutan Bool türünde nesne döndürür</returns>
        public bool DayanakVarMi()
        {
            if (dayanaklar == null) return false;
            if (dayanaklar.Count < 1) return false;
            return true;
        }

        /// <summary>
        /// Evrak tarihine göre Tarih olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Evrak tarihi null ise false değilse true değerini tutan Bool türünde nesne döndürür</returns>
        public bool TarihVarMi()
        {
            return !evrakTarih.isNull;
        }

        /// <summary>
        /// Evrak numarasına göre Evrak Numarasının olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, Evrak numarası boşluksa false değilse true değerini tutan Bool türünde nesne döndürür</returns>
        public bool EvrakNoVarMi()
        {
            return evrakNo != "";
        }

        /// <summary>
        /// Parametre olarak verilen tertip bilgisine göre, Hazine yardımı olup olmadığını dönen fonksiyon.
        /// </summary>
        /// <param name="t">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe ve Ödenek tertipleri için, Fonksiyonel kodu 01.6.1 veya 01.6.2 ile başlıyorsa true değilse false değerini tutan Bool türünde nesne döndürür</returns>
        public static bool GiderHazineYardimiMi(Tertip t)
        {
            if (t.tabloKod == ENUMTabloKod.TABButce || t.tabloKod == ENUMTabloKod.TABOdenek)
            {
                if (t.kodlar.Length > 1)
                {
                    string fon = t.kodlar[1].Replace(".", "");

                    if (fon.StartsWith("01.6.1.") || fon.StartsWith("01.6.2."))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Giderler dizisine bakarak, ilgili tertiplerin hazine yardımı olup olmamasına göre Düşülen ve Eklenenleri hesaplayan fonksiyon.
        /// </summary>
        /// <param name="dusulen">Düşülen miktarı tutan, Double türünde nesne</param>
        /// <param name="eklenen">Eklenen miktarı tutan, Double türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Hazine yardımı olan tertiplerde hesaplanan düşülen ve eklenen miktarların farkı 0'a eşit ise true değilse false değerini tutan Bool türünde nesne döndürür</returns>
        public bool GiderToplamiSifirMi(out double dusulen, out double eklenen)
        {
            dusulen = 0;
            eklenen = 0;
            if (giderler != null && giderler.Count > 0)
            {
                for (int i = 0; i < giderler.Count; i++)
                {
                    BISatir s = (BISatir)giderler[i];
                    if (!GiderHazineYardimiMi(s.tertip))
                    {
                        dusulen += s.dusulen;
                        eklenen += s.eklenen_serbest;
                    }
                }
            }
            return Math.Round((dusulen - eklenen) * 100) == 0;
        }

        /// <summary>
        /// DayanakVarmi, TarihVarmi, EvrakNoVarmi, DetayVarmi, AyniSatirVarmi rutinlerini çalıştırarak varsa hataları döndüren fonksiyon.
        /// </summary>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer hata var ise tutan String türünde nesne döndürür</returns>
        public string HataliMi()
        {
            int yer, satir1, satir2;
            string tertip;
            if (!DayanakVarMi())
                return ButceUygulamaResI.BUM320;
            if (!TarihVarMi())
                return ButceUygulamaResI.BUM321;
            if (!EvrakNoVarMi())
                return ButceUygulamaResI.BUM322;

            if (!DetayVarMi())
                return ButceUygulamaResI.BUM323;
            if (AyniSatirVarMi(out yer, out satir1, out satir2, out tertip))
                return satir1 + ButceUygulamaResI.BUM324 + "," + " " + satir2 + ButceUygulamaResI.BUM325 + " '" + tertip + "' " + ButceUygulamaResI.BUM326;
            string hataStr;
            if ((hataStr = BosSatirVarMi()) != "")
                return hataStr;
            if (HataliTertipVarMi(out yer, out satir1, out tertip))
                return satir1 + ButceUygulamaResI.BUM327 + " '" + tertip + "' " + ButceUygulamaResI.BUM328;
            if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.AKTARMA)))
            {
                double eklenen, dusulen;
                if (!GiderToplamiSifirMi(out dusulen, out eklenen))
                    return ButceUygulamaResI.BUM329 + " (" + dusulen.ToString("#,##0.##") + ") " + ButceUygulamaResI.BUM330 + "(" + eklenen.ToString("#,##0.##") + ") " + ButceUygulamaResI.BUM331;
            }
            return "";
        }

        /// <summary>
        /// Parametre olarak veirlen Satırlar dizinde verilen tertipi arayan ve eğer ilgili tertipi bulursa hangi satırda olduğu bilgisini döndüren fonksiyon.
        /// </summary>
        /// <param name="satirlar">Satırları tutan, TNSCollection türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, eğer tertipi bulduysa ilgili satır numarasını bulamadıysa -1 değerini tutan Int türünde nesne döndürür</returns>
        public static int SatirBulTertip(TNSCollection satirlar, Tertip tertip)
        {
            for (int i = 0; i < satirlar.Count; i++)
            {
                BISatir satir = (BISatir)satirlar[i];
                if (satir.tertip.tabloKod == tertip.tabloKod)
                {
                    if (satir.tertip.TertipStrOl() == tertip.TertipStrOl())
                        return i;
                }
            }
            return -1;
        }

        ///// <summary>
        ///// Genel bütçeli olmayan kurumlar için, İcmal Harcama Programı Revize işlemleri için, Hazine Yardımlarına bakarak,
        ///// 04.2.1.02 ve 04.2.2.02 tertiplerin dışındaki kayıtlar için Yansıtma işlemini gerçekleştiren fonksiyon
        ///// </summary>
        //public void IcmalRevizeGelirYansit()
        //{
        //    if (GenelButceliMi(kurum))
        //        return;
        //    if (islemKod.StartsWith(BISabitler.IslemKod(ENUMIslemKod.IHPREVIZE)))
        //    {
        //        Tertip[] hazineYardimlari = new Tertip[2];
        //        BISabitler.HazineYardimiTertipleriBul(yil, kurum, ref hazineYardimlari[0], ref hazineYardimlari[1]);
        //        if (gelirler == null)
        //            return;
        //        for (int i = 0; i < gelirler.Count; i++)
        //        {
        //            int jj;
        //            BISatir satir = (BISatir)gelirler[i];
        //            int hazineYer;
        //            if ((string)satir.tertip.kodlar[1] == "04.2.1.02")
        //                hazineYer = 0;
        //            else if ((string)satir.tertip.kodlar[1] == "04.2.2.02")
        //                hazineYer = 1;
        //            else
        //                hazineYer = -1;
        //            if (hazineYer != -1)
        //            {
        //                Tertip hyTertip = hazineYardimlari[hazineYer];
        //                hyTertip.kodlar[3] = hyTertip.kodlar[3].Substring(0, 4);
        //                int satirNo = SatirBulTertip(giderler, hyTertip);
        //                if (satirNo == -1)
        //                {
        //                    BISatir yeniSatir = new BISatir();
        //                    yeniSatir.tertip = new Tertip(yil, ENUMTabloKod.TABButce);
        //                    for (jj = 0; jj < hyTertip.kodlar.Length; jj++)
        //                        yeniSatir.tertip.kodlar[jj] = hyTertip.kodlar[jj];
        //                    yeniSatir.yildizYer = 0;
        //                    yeniSatir.eklenen_serbest = 0;
        //                    yeniSatir.dusulen = 0;
        //                    yeniSatir.aylikProgram = new double[12];
        //                    /*
        //                    for (jj = 0; jj < 12; jj++)
        //                        yeniSatir.aylikProgram.Add((double)0);
        //                    */
        //                    giderler.Add(yeniSatir);
        //                    satirNo = giderler.Count - 1;
        //                }
        //                BISatir bulSatir = (BISatir)giderler[satirNo];
        //                for (jj = 0; jj < 12; jj++)
        //                    bulSatir.aylikProgram[jj] = satir.aylikProgram[jj];
        //            }
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Serbest Bilgi Satır Sınıfı
    /// </summary>
    [Serializable()]
    public class SerbestBilgiSatir
    {
        /// <summary>
        /// Kurum kodu
        /// </summary>
        public string kurumKod;
        /// <summary>
        /// Ekonomik kod
        /// </summary>
        public string ekoKod;
        /// <summary>
        /// Ay
        /// </summary>
        public int ay;
    }

    /*
    [Serializable()]
    public class TertipAyBilgi
    {
        public string ekoKod;
        public string ekoAd;
        public double kbo;
        public double serbest;
        public TNSCollection ayDegerler;
        public TNSCollection ayOranlar;
    }
    */

    /*

    
        [Serializable()]
        public class TelEmriSatiri
        {
            public string sablon;
            public double oran;
            public TNSCollection istisnaTertipler; // içinde string tipinde istisna tertip şablonları tutulur.
        }// END CLASS DEFINITION TelEmriSatiri
        
        [Serializable()]
        public class TelEmri
        {
            public int yil;
            public string telEmriNo;
            public TNSDateTime tarih;
            public TNSCollection telEmriSatirlari; // İçerisinde TelEmriSatiri tipinde nesneler bulunur.
            public Sonuc sonuc;
        }// END CLASS DEFINITION TelEmri

        [Serializable()]
        public class TelEmriOzet
        {
            public string telEmriNo;
            public TNSDateTime tarih;
        }// END CLASS DEFINITION TelEmriOzet

        [Serializable()]
        public class AHPVAN
        {
            public TNSCollection AHPListe;//TertipBilgiden oluşan
            public Sonuc sonuc;
            public AHPVAN()
            {
                sonuc=new Sonuc();
                AHPListe = new TNSCollection ();
            }
        }// END CLASS DEFINITION AHPVAN

        [Serializable()]
        public class TNSCollectionVAN
        {
            public TNSCollection dizi;
            public Sonuc sonuc;
        
            public TNSCollectionVAN()
            {
                TNSCollection dizi=new TNSCollection();
                Sonuc sonuc=new Sonuc(true, "", "");
            }
        }// END CLASS DEFINITION AHPVAN


        [Serializable()]
        public class HarcamaVAN
        {
            public TNSCollection HarcamaListe;//TertipBilgiden oluşan
            public Sonuc sonuc;
            public HarcamaVAN()
            {
                sonuc=new Sonuc();
                HarcamaListe = new TNSCollection ();
            }
        }// END CLASS DEFINITION HarcamaVAN

        */

    /// <summary>
    /// Bütçe Uygulama Servis Ara Yüzü
    /// </summary>
    public interface IBUMServis
    {
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// Icmal Harcama Programını listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İcmal Harcama Bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        ObjectArray IHPListele(Kullanici kullanan, int yil, int asama, Tertip tertip);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, İcmal Satırlarına göre,
        /// İcmal Harcama Programı bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="icmalSatirlari">İcmal Satırları bilgisini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc IHPKaydet(Kullanici kullanan, int yil, int asama, TNSCollection icmalSatirlari);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Kurum Birim bilgilerine göre,
        /// İcmal Harcama Programı bilgilerini onaylayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum/Birim bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc IHPOnayla(Kullanici kullanan, int yil, int asama, string kurumBirim);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama ve Kurum Birim bilgilerine göre,
        /// İcmal Harcama Programı bilgilerinin onayını kaldıran fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum/Birim bilgisini tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onay kaldırma işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc IHPOnayKaldir(Kullanici kullanan, int yil, int asama, string kurumBirim);
        /// <summary>
        /// Parametre olarak verilen, Kullanıcı, Yıl, Kurum Birim, Aşama ve Tertip bilgilerine göre,
        /// Ayrıntılı Program bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumBirim">Kurum/Birim bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertipBilgileri">Tertip bilgilerini tutan, TNSCollection türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc APKaydet(Kullanici kullanan, int yil, string kurumBirim, int asama, TertipOdenek[] tertipBilgileri);
        /// <summary>
        /// Parametre olarak verilen, Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// Ayrıntılı Program bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program Bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        TNS.BHMv2.TertipOdenek[] APListele(Kullanici kullanan, int asama, Tertip kistasTertip);
        /// <summary>
        /// Parametre olarak verilen, Kullanıcı, Yıl, Aşama, Tertip ve Ekonomik Seviye bilgilerine göre,
        /// Ayrıntılı Program bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="kistasTertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <param name="ukb">Sonuç kümesinde bulunacak tertibin hangi kodlardan oluşacağı bilgisi.</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program Bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        TertipOdenek[] APListeleKodBilgi(Kullanici kullanan, int asama, Tertip kistasTertip, UcgenKodBilgileri ukb);
        /// <summary>
        /// Parametre olarak verilen, Kullanıcı, Yıl, Aşama ve Tertip bilgilerine göre,
        /// Ayrıntılı Program bilgisini okuyan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program Bilgisini tutan TertipOdenek türünde nesne döndürür</returns>
        TertipOdenek APOku(Kullanici kullanan, int yil, int asama, Tertip tertip);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Ayrıntılı Program bilgilerini onaylayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc OnaylaAP(Kullanici kullanan, int yil, string kurum, int asama);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Aşama bilgilerine göre,
        /// Ayrıntılı Program bilgilerinin Onayını kaldıran fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onay Kaldırma işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc OnayKaldirAP(Kullanici kullanan, int yil, string kurum, int asama);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Aşama, Tertip, Yöntem bilgilerine göre,
        /// Taslak oluşturan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="asama">Aşama bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgilerini tutan, Tertip türünde nesne</param>
        /// <param name="yontem">Yöntem bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Program bilgilerini tutan TertiplerVAN türünde nesne döndürür</returns>
        TertiplerVAN TaslakOlusturAP(Kullanici kullanan, int yil, int asama, Tertip tertip, int yontem);
        /// <summary>
        /// Parametre olarak verilen Üst Kod, Hiyerarşi ve Ana Kodlar bilgilerine göre,
        /// Bütçe İşlem bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="ustKod">Üst kod bilgisini tutan, String türünde nesne</param>
        /// <param name="hiyerarsik">Hiyerarşiye göre işlem yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <param name="anaKodlar">Ana kodlara göre işlem yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        ObjectArray ButceIslemListesi(string ustKod, bool hiyerarsik, bool anaKodlar);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Form bilgilerine göre,
        /// Bütçe İşlem Form bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="form">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="sadeceOzet">Özet listelemenin yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem Form bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        ObjectArray BIListele(Kullanici kullanan, BIForm form, bool sadeceOzet);
        /// <summary>
        /// Parametre olarak verilen Yıl bilgisine göre,
        /// Yedek tertipleri listeleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Yedek tertip bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        ObjectArray YedekTertipListesi(int yil);
        /// <summary>
        /// Parametre olarak verilen Yıl ve Tertip bilgilerine göre,
        /// Ayrıntılı Harcama Programının Son durumunu listeleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="tertip">Tertip bilgisini tutan, Tertip türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Ayrıntılı Harcama Programının Son durumuna ait bilgileri tutan ObjectArray türünde nesne döndürür</returns>
        ObjectArray AHPSonDurumGoster(int yil, Tertip tertip);
        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu ve Sıra numarası bilgilerine göre,
        /// Kurumları listeleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kurumlara ait bilgileri tutan ObjectArray türünde nesne döndürür</returns>
        ObjectArray BIKurumlarListesi(int yil, string kurum, string islemKod, int siraNo);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı Yıl, Kurum, İşlem Kodu ve Sıra numarası bilgilerine göre,
        /// Bütçe İşlem bilgisi okuyan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgisini tutan Object türünde nesne döndürür</returns>
        object BIOku(Kullanici kullanan, int yil, string kurum, string islemKod, int siraNo);

        ///// <summary>
        ///// Parametre olarak verilen, Kullanıcı, Yıl, Kurum, İşlem Kod ve Sıra no bilgilerini,
        ///// Bütçe İşlem bilgilerinin Tertip kodlarını hiyerarşik bir biçimde kırarak listeleyen fonksiyon
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        ///// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        ///// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        ///// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        ///// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, Bütçe İşlem bilgisini tutan Object türünde nesne döndürür</returns>
        //object BIOkuUcgen(Kullanici kullanan, int yil, string kurum, string islemKod, int siraNo);

        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Formu ve Aktarım bilgilerine göre,
        /// Bütçe İşlem Form bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="aktarim">Aktarıma göre yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc BIKaydet(Kullanici kullanan, BIForm islem, bool aktarim);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Formu ve BUMKO Onay bilgilerine göre,
        /// Bütçe İşlem Form bilgilerini onaylayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <param name="bumkoOnayiMi">BUMKO Onayı olup olmadığını tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Onaylama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc BIOnayla(Kullanici kullanan, BIForm islem, bool bumkoOnayiMi);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Bütçe İşlem Formu bilgilerine göre,
        /// Bütçe İşlem Form bilgilerini iptal eden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, İptal işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc BIIptal(Kullanici kullanan, BIForm islem);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı, Yıl, Kurum ve Serbest bilgilerine göre
        /// Serbestleri hesaplayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="ahpSerbestBilgi">Ayrıntılı Harcama Programına ilişkin Serbest bilgilerini tutan, TNSCollection türünde nesne </param>
        /// <returns>Fonksiyonun çalışması sonucunda, Serbestleri Hesaplama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc SerbestleriHesapla(Kullanici kullanan, int yil, string kurum, TNSCollection ahpSerbestBilgi);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve İşlem bilgilerini kullanarak,
        /// Bütçe İşlemine BUMKO Kodu atayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="islem">Bütçe İşlem Form bilgisini tutan, BIForm türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kod Atama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc BIBUMKOKodAta(Kullanici kullanan, BIForm islem);

        /// <summary>
        /// Parametre olarak verilen Yıl ve İşlem Kodu bilgilerini kullanarak,
        /// Dayanak bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Dayanak Bilgilerini tutan ObjectArray türünde nesne döndürür</returns>
        ObjectArray BIDayanakListesi(int yil, int islemKod);

        /// <summary>
        /// Parametre olarak verilen Kullanıcı ve Dayanak bilgilerine göre,
        /// Dayanak bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="dayanak">Dayanak bilgisini tutan, BIDayanak türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kaydetme işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        Sonuc DayanakKaydet(Kullanici kullanan, BIDayanak dayanak);
        /// <summary>
        /// Parametre olarak verilen Dayanak bilgilerine göre,
        /// Dayanak bilgisini okuyan fonksiyon
        /// </summary>
        /// <param name="dayanak">Dayanak bilgisini tutan, BIDayanak türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Dayanak bilgisini tutan BIDayanak türünde nesne döndürür</returns>
        BIDayanak DayanakOku(BIDayanak dayanak);
        /// <summary>
        /// Parametre olarak verilen Kullanıcı bilgilerini kullanarak,
        /// Kaynak Yıl Dayanak bilgilerini, Hedef Yıl Dayanak bilgilerine kopyalayan fonksiyon
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgisini tutan, Kullanici türünde nesne</param>
        /// <param name="kaynakYil">Kaynak Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="hedefYil">Hedef Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Kopyalama işleminin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür </returns>
        Sonuc DayanakKopyala(Kullanici kullanan, int kaynakYil, int hedefYil);

        ObjectArray ListeleGunlukNakitIhtiyac(Kullanici kullanan, TNSDateTime tarih, string birimKod, bool tarihAraligiOlsun);
        Sonuc KaydetGunlukNakitIhtiyac(Kullanici kullanan, TNSDateTime tarih, string birimKod, GunDegerHarcamaTur[] harcamaTutarlari);
    }
}
