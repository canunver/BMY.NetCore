using TNS.KYM;
using System.Collections;
using System;
using System.Data;
using TNS.BKD;
using TNS.DEG;
using System.Text;
using TNS.UZY;
using TNS.HRC;

namespace TNS.BHMv2
{

    public enum ENUMBirimOnayTur
    {
        BIRIMBUTCEHAZIRLIK = 1,
        KIKYUZDE10 = 2,
        PROJEGEREKCE = 3,
        HIZMETMALIYETITASRA = 4,
        ODENEKGONDERME = 5,
        BORDRO = 6,
        MALIBEKLENTI = 7,
        YATIZLEMEDEGERLENDIRME = 7,
    }

    public enum ENUMTorbaOdenekTalepDurum
    {
        TANIMSIZ = 0,
        YENI = 1,
        TALEPEDILDI = 2,
        ONAYLANDI = 3,
        REDEDILDI = 4
    }

    public enum ENUMOGY : int
    {
        Onaylama = 1,
        Gorme = 2,
        Yazma = 3,
        OnayKaldir = 4
    }

    /// <summary>
    /// Ödenek rakamı üretilirken kaynak olarak kullanılacak bilgilerin listesi
    /// </summary>
    public enum ENUMUretimKaynakTur
    {
        /// <summary>
        /// Ödenek rakamı üretilirken, kaynak olarak PEB Odenek kullanılacak
        /// </summary>
        Odenek = 1,

        /// <summary>
        /// Ödenek rakamı üretilirken, kaynak olarak Ödeme Emirleri kullanılacak
        /// </summary>
        OdemeEmri = 3,

        /// <summary>
        /// Ödenek rakamı üretilirken, kaynak olarak muhasebe kayıtları kullanılacak
        /// </summary>
        Muhasebe = 2,
    }

    /// <summary>
    /// Bütçe verilerinde yapılan değişikliklerin takip edileceği veri türlerinin listesi
    /// </summary>
    public enum ENUMZamanDamgaTur
    {
        /// <summary>
        /// Gider bilgisi değişiklik zamanı
        /// </summary>
        GIDER = 1,
        /// <summary>
        /// Gelir bilgisi değişiklik zamanı
        /// </summary>
        GELIR = 2,
        /// <summary>
        /// Finansman bilgisi değişiklik zamanı
        /// </summary>
        FINANSMAN = 3,
        /// <summary>
        /// Harcama bilgisi değişiklik zamanı
        /// </summary>
        HARCAMA = 4,
        /// <summary>
        /// Harcama bilgisi değişiklik zamanı
        /// </summary>
        TAVANUSTU = 5,
    }

    /// <summary>
    /// Taşra birimlerinden bütçe hazırlığı için veri toplanırken kullanılacak aşamalar
    /// </summary>
    public enum ENUMBHBirimAsama
    {
        /// <summary>
        /// Sistem tarafından üretilen veriler.
        /// </summary>
        URETIM = 1,
        /// <summary>
        /// Birimlerin girdiği değerler
        /// </summary>
        TALEP = 2,
    }

    /// <summary>
    /// Veri tabanından tertip listesi alınacakken, eklenecek ve düşülecek tertiplerin kodlarını tutan yapı. 
    /// </summary>
    [System.Serializable()]
    public class KistasYapi
    {
        /// <summary>
        /// Sonuc kümesinde bulunacak tertiplerin listesi. En az bir tane eleman olmalıdır.
        /// </summary>
        public Tertip[] ekleTertipler = null;

        /// <summary>
        /// Sonuc kümesinde bulunmayacak tertiplerin listesi. Boş olabilir.
        /// </summary>
        public Tertip[] dusTertipler = null;
    }


    /// <summary>
    /// TavanUstuOdenek: 
    /// </summary>
    [System.Serializable()]
    public class TavanUstuTertipOdenek
    {
        string geciciKod;
        public string tertipStr { get { return tertip.TertipStrOl(); } set { geciciKod = value; } }

        string geciciAd;
        public string tertipAd { get { return tertip.tertipAdi; } set { geciciAd = value; } }

        /// <summary>
        /// Tavan üstü istenen tertibin bilgisi.
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Tavan üstü istenilen senaryonun o andaki ödenek miktarı
        /// </summary>
        public double[] senaryoOdenegi;
        public double odenek0 { get { return TNS.BKD.Arac.DiziElemenAl(senaryoOdenegi, 0); } set { TNS.BKD.Arac.DiziElemenAta(senaryoOdenegi, 0, value); } }
        public double odenek1 { get { return TNS.BKD.Arac.DiziElemenAl(senaryoOdenegi, 1); } set { TNS.BKD.Arac.DiziElemenAta(senaryoOdenegi, 1, value); } }
        public double odenek2 { get { return TNS.BKD.Arac.DiziElemenAl(senaryoOdenegi, 2); } set { TNS.BKD.Arac.DiziElemenAta(senaryoOdenegi, 2, value); } }

        /// <summary>
        /// Tavan üstü istenilen miktar
        /// </summary>
        public double ekOdenek0 { get; set; }
        public double ekOdenek1 { get; set; }
        public double ekOdenek2 { get; set; }

        /// <summary>
        /// Tavan üstü gerekçesi
        /// </summary>
        public string gerekce { get; set; }

        public TavanUstuTertipOdenek(int senSay)
        {
            if (senSay > 0)
                senaryoOdenegi = new double[senSay * 3];
            else
                senaryoOdenegi = null;
        }

        public TavanUstuTertipOdenek()
        {
            senaryoOdenegi = new double[3];
        }

        public void TertipOl(int yil, ENUMTabloKod tabloKod)
        {
            tertip = new Tertip(yil, tabloKod);
            tertip.TertipAta(geciciKod);
        }
    }

    /// <summary>
    /// Verilerde yapılan değişiklik bilgilerini (hangi veri, veri bilgisi, kullanıcı) tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class DegisimZaman
    {
        /// <summary>
        /// Değişiklik yapılan verinin yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Değişiklik yapılan veri ödenek ise senaryo, gerçekleşme ise ay bilgisi
        /// </summary>
        public string senaryoAy;

        /// <summary>
        /// Değişiklik yapılan verinin sahibi birim
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Değişiklik yapılan verinin türü
        /// </summary>
        public ENUMZamanDamgaTur tur;

        /// <summary>
        /// Değişikliği yapan kullanıcının kodu
        /// </summary>
        public string kullaniciKod;

        /// <summary>
        /// Değişim zamanı nesnesi oluturan oluşturucu yordam.
        /// </summary>
        /// <param name="yil">Değişiklik yapılan verinin yılı</param>
        /// <param name="senaryoAy">Değişiklik yapılan veri ödenek ise senaryo, gerçekleşme ise ay bilgisi</param>
        /// <param name="kurKod">Değişiklik yapılan verinin sahibi birim</param>
        /// <param name="tur">Değişiklik yapılan verinin türü</param>
        public DegisimZaman(int yil, string senaryoAy, string kurKod, ENUMZamanDamgaTur tur)
        {
            this.yil = yil;
            this.senaryoAy = senaryoAy;
            this.kurKod = kurKod;
            this.tur = tur;
        }

        /// <summary>
        /// Değişim zamanı nesnesi oluturan oluşturucu yordam.
        /// </summary>
        /// <param name="yil">Değişiklik yapılan verinin yılı</param>
        /// <param name="senaryoAy">Değişiklik yapılan veri ödenek ise senaryo, gerçekleşme ise ay bilgisi</param>
        /// <param name="kurKod">Değişiklik yapılan verinin sahibi birim</param>
        /// <param name="tur">Değişiklik yapılan verinin türü</param>
        /// <param name="kullaniciKod">Değişikliği yapan kullanıcının kodu</param>
        public DegisimZaman(int yil, string senaryoAy, string kurKod, ENUMZamanDamgaTur tur, string kullaniciKod)
        {
            this.yil = yil;
            this.senaryoAy = senaryoAy;
            this.kurKod = kurKod;
            this.tur = tur;
            this.kullaniciKod = kullaniciKod;
        }
    }

    /// <summary>
    /// Bütçe senaryo kodu, adı ve ilgili tasnif bilgisinin kodunu ve adını tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class ButceSenaryo : IComparable
    {
        /// <summary>
        /// Bütçe senaryo kodu
        /// </summary>
        public string kod { get; set; }
        /// <summary>
        /// Bütçe senaryo adı
        /// </summary>
        public string ad { get; set; }

        public ButceSenaryo(string kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
        }

        public ButceSenaryo()
        {
            this.kod = "";
            this.ad = "";
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return -1;
            if (obj is ButceSenaryo)
            {
                ButceSenaryo bsgelen = (ButceSenaryo)obj;
                return this.kod.CompareTo(bsgelen.kod);
            }
            return 1;
        }
    }

    /// <summary>
    /// Gider bütçesini, kullanıcının seçtiği hiyerarşik yapıda ve ilişkilendirdiği tertiplerle birlikte tutan tasnifin kimlik bilgisi sınıfı
    /// </summary>
    [System.Serializable()]
    public class ButceTasnif
    {

        /// <summary>
        /// Tasnifin yazılım içinde tutulan sıra numarası.
        /// </summary>
        public int siraNo = -1;

        /// <summary>
        /// Tasnifin adı
        /// </summary>
        public string ad = null;

        /// <summary>
        /// Tasnifin sahibi birim
        /// </summary>
        public string kur = null;

        /// <summary>
        /// Tasnifin türü (1 ise kurum tavan, 2 ise birim tavan)
        /// </summary>
        public int tur = 0;

        /// <summary>
        /// Tasnifle ilişkilendirilmiş senaryoları tutan dizi.
        /// </summary>
        public ButceSenaryo[] senaryolar = null;
    }

    /// <summary>
    /// Tasnifde her bir satır için birden fazaa girilen tertip şablonunun,
    /// oluşturulacak tertip kümesine ekleneceği veya çıkartılacağı bilgisi
    /// </summary>
    public enum TertipEkleCikar
    {
        /// <summary>
        /// İlişkili şablon tertip kümesine eklenecek
        /// </summary>
        Ekle = 1,
        /// <summary>
        /// İlişkili şablon tertip kümesinden çıkartılacak
        /// </summary>
        Cikar = 2
    }


    /// <summary>
    /// Bir bütçe tasnifinde yer alan bir satırla ilgili bilgileri tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class ButceTasnifHiyerarsikSatir
    {
        /// <summary>
        /// Bütçe tasnifinde yer alan satırın kodu, 99.99.99.99 şeklinde tutulur.
        /// </summary>
        public string kod;

        /// <summary>
        /// Bütçe tasnifi düzenlendiğinde satır bilgileri değişdiğinde, her satıra yeni kod atanır.
        /// </summary>
        public string yeniKod;

        /// <summary>
        /// Bütçe tasnifi düzenlendiğinde, yeni kod üretmek için kullanılan, her satırın içerleklik seviyesi
        /// </summary>
        public int seviye;

        /// <summary>
        /// Satırın kullanıcı tarafından girilen ve gösterilen açıklaması.
        /// </summary>
        public string ad;

        /// <summary>
        /// Alt kırılımı var mı?
        /// </summary>
        public bool altKirilimVar;

        /// <summary>
        /// Satırla ilişkilendirilmiş olan tertip şablonlarını tutan dizi
        /// </summary>
        public ButceTasnifTertipSablon[] tertipSablonlar = null;

        /// <summary>
        /// Tertip şablonlar dizisinde yer alan elemanları teker teker dizgiye çevirip "enter" karakteri ile birleştiren ve döndüren yordam.
        /// </summary>
        /// <returns>Tertip şablonlar dizisinin dizgi hali döndürülür.</returns>
        public string TertiplerStrOl()
        {
            string donusDeger = string.Empty;
            for (int i = 0; i < tertipSablonlar_Length(); i++)
            {
                if (!String.IsNullOrEmpty(donusDeger)) donusDeger += "\r\n";
                donusDeger += tertipSablonlar[i].ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen dizgi, enter karekterinden ayrılır ve ayrım sonucu oluşan her bir dizgi
        /// tertip şablonu yapısına çevrilerek tertipSablonlar dizisine atanır.
        /// </summary>
        /// <param name="tertiplerStr">Tertip şablonları yapısına çevrilecek dizgi</param>
        public void TertipAta(string tertiplerStr)
        {
            char[] sep = { '\r', '\n' };
            string[] tertipler = tertiplerStr.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            ArrayList al = new ArrayList();
            for (int i = 0; i < tertipler.Length; i++)
            {
                if (!string.IsNullOrEmpty(tertipler[i]))
                {
                    ButceTasnifTertipSablon sablon = null;
                    try
                    {
                        sablon = new ButceTasnifTertipSablon(tertipler[i]);
                    }
                    catch
                    {
                        sablon = null;
                    }
                    if (sablon != null)
                        al.Add(sablon);
                }
            }
            tertipSablonlar = (ButceTasnifTertipSablon[])al.ToArray(typeof(ButceTasnifTertipSablon));
        }

        /// <summary>
        /// Tertip şablonları dizisinin eleman sayısını döndüren yordam
        /// </summary>
        /// <returns>Tertip şablonları dizisinin eleman sayısını döndürülür</returns>
        public int tertipSablonlar_Length()
        {
            if (tertipSablonlar == null)
                return 0;
            else
                return tertipSablonlar.Length;
        }

        /// <summary>
        /// Tertip şablonlar dizisine yeni bir tertiğ şablonu bilgisi ekleyen yordam.
        /// </summary>
        /// <param name="tertipSablon">dizgi şeklinde, boyutları tire (-) ile ayrılmış tertip şablonu</param>
        /// <param name="ekleCikar">Verilen tertip şablonu ile bulunan değerler, toplama dahil mi edilecek, toplamdan çıkartılacak mı?</param>
        public void TertipEkle(string tertipSablon, TertipEkleCikar ekleCikar)
        {
            int yer;
            if (tertipSablonlar == null)
            {
                tertipSablonlar = new ButceTasnifTertipSablon[1];
            }
            else
            {
                ButceTasnifTertipSablon[] yeniDizi = new ButceTasnifTertipSablon[tertipSablonlar.Length + 1];
                tertipSablonlar.CopyTo(yeniDizi, 0);
                tertipSablonlar = yeniDizi;
            }
            yer = tertipSablonlar.Length - 1;
            tertipSablonlar[yer] = new ButceTasnifTertipSablon(tertipSablon, ekleCikar);
        }
    }

    /// <summary>
    /// Bütçe tasnifinde yer alan, her bir satırla ilişkili tertip şablonu bilgisini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class ButceTasnifTertipSablon
    {
        /// <summary>
        /// Dizgi şeklinde, boyutları tire (-) ile ayrılmış tertip şablonu
        /// </summary>
        public string tertipSablon;

        /// <summary>
        /// Verilen tertip şablonu ile bulunan değerler, toplama dahil mi edilecek, toplamdan çıkartılacak mı?
        /// </summary>
        public TertipEkleCikar ekleCikar;

        /// <summary>
        /// Tertip şablon nesnesi oluşturan oluşturucu yordam
        /// </summary>
        /// <param name="tertipSablon">Dizgi şeklinde, boyutları tire (-) ile ayrılmış tertip şablonu</param>
        /// <param name="ekleCikar">Verilen tertip şablonu ile bulunan değerler, toplama dahil mi edilecek, toplamdan çıkartılacak mı?</param>
        public ButceTasnifTertipSablon(string tertipSablon, TertipEkleCikar ekleCikar)
        {
            this.tertipSablon = tertipSablon;
            this.ekleCikar = ekleCikar;
        }

        /// <summary>
        /// Tertip şablon nesnesi oluşturan oluşturucu yordam
        /// </summary>
        /// <param name="sablon">Tertip şablonu ve ekleme/çıkarma bilgisi dizgisi</param>
        public ButceTasnifTertipSablon(string sablon)
        {
            if (string.IsNullOrEmpty(sablon))
                throw new Exception(ButceHazirlikResI.BHM015);
            string[] sablonParca = sablon.Split(' ');
            tertipSablon = sablonParca[0];
            if ((sablonParca.Length == 2) && sablonParca[1].Equals("(-)"))
                ekleCikar = TertipEkleCikar.Cikar;
            else
                ekleCikar = TertipEkleCikar.Ekle;
        }

        /// <summary>
        /// Tertip şablonu nesnesini, tertip şablonu ve ekleme/çıkarma bilgisini birbirine
        /// ekleyerek dizgiye çeviren yordam.
        /// </summary>
        /// <returns>Tertip şablonu ve ekleme/çıkarma bilgisini dizgisi döndürülür</returns>
        public override string ToString()
        {
            string strEkleCikar = string.Empty;
            if (ekleCikar == TertipEkleCikar.Ekle) strEkleCikar = "(+)";
            else strEkleCikar = "(-)";
            return tertipSablon + " " + strEkleCikar;
        }
    }

    /// <summary>
    /// ButceTasnifDegerler: 
    /// </summary>
    [System.Serializable()]
    public class ButceTasnifSatirOdenegi
    {
        /// <summary>
        /// Tasnif değerlerinin ilişkili olduğu satıra ait bilgileri tutan değişken
        /// </summary>
        public ButceTasnifHiyerarsikSatir hiyerarsi = new ButceTasnifHiyerarsikSatir();

        /// <summary>
        /// Bu tasnif satırı için sunucudan döndürülecek ek değer bilgileri
        /// </summary>
        public double[] ekDegerler = null;

        /// <summary>
        /// Tasnif değer nesnesi döndüren oluşturucu yordam. Ek değer dizisinin boyutu verilen sayı kadar oluşturulur.
        /// </summary>
        /// <param name="ekDegerUzunluk">Ek değer dizisinin boyutu</param>
        public ButceTasnifSatirOdenegi(int ekDegerUzunluk)
        {
            if (ekDegerUzunluk <= 0)
                ekDegerler = null;
            else
                ekDegerler = new double[ekDegerUzunluk];
        }
    }


    /// <summary>
    /// Kullanıcının girdiği bir tasnifin, yıl ve senaryoya ilişkili değerlerini tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class ButceTasnifOdenekleriFormu
    {

        /// <summary>
        /// Ödeneklerinin tutulduğu tasnif bilgisi.
        /// </summary>
        public ButceTasnif tasnif = new ButceTasnif();

        /// <summary>
        /// Tasnif ödeneklerinin ait olduğu yıl bilgisi.
        /// </summary>
        public int yil;

        /// <summary>
        /// Bu teasnife ait girilen her bir satırın ödeneğini tutan dizi
        /// </summary>
        public ButceTasnifSatirOdenegi[] butceTasnifSatirOdenekleri;
    }

    /// <summary>
    /// Bütçe fişine gerekçe girilirken, alınacak malzeme bilgisi tutan sınıf
    /// </summary>
    [System.Serializable()]
    public class ButceFisiMalzeme
    {
        /// <summary>
        /// Alınacak malzemenin hesap kodu
        /// </summary>
        public string hesapKod;
        /// <summary>
        /// Alınacak malzemenin hesap adı
        /// </summary>
        public string hesapAd;
        /// <summary>
        /// Alınacak malzemenin ölçü birimi
        /// </summary>
        public int olcuBirimKod;
        /// <summary>
        /// Alınacak malzemenin ölçü birimi
        /// </summary>
        public string olcuBirimAd;
        /// <summary>
        /// Alınacak malzemenin miktarı
        /// </summary>
        public decimal miktar;
        /// <summary>
        /// Alınacak malzemenin tahmini birim fiyatı
        /// </summary>
        public decimal birimFiyat;
        /// <summary>
        /// Alınacak malzemenin tutarı (=miktar * birim fiyat)
        /// </summary>
        public decimal toplamTutar;
    }


    public class OdenekBilgiSiralaEko2Peb : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            if (x.GetType() == typeof(OdenekBilgi) && y.GetType() == typeof(OdenekBilgi))
            {
                return BKD.Arac.TertipKodKiyaslaEko2Peb(((OdenekBilgi)x).tertip.kodlar, ((OdenekBilgi)y).tertip.kodlar);
            }
            return 0;
        }
    }

    /// <summary>
    /// Sunucudan istemciye, yada istemciden sunucuya bir tertip ve tertiple ilgili rakamları taşırken kullanılan sınıf.
    /// Bu sınıfta tertiple birlikte, asli ödenek rakamlarını tutan dizi ile birlikte gereken ek rakamları da tutan ikinci bir dizi vardır.
    /// Bu sınıf collection tuttuğu için kullanımdan kaldırılmıştır. (Obsolete("Lüften TertipOdenek sınıfını kullanınız!"))
    /// </summary>
    [Serializable()]
    public class OdenekBilgi
    {
        public string aciklama;

        public string kod4
        {
            get
            {
                return tertip.kodlar[4];
            }
        }

        public string kodad4
        {
            get
            {
                return tertip.kodlar[4] + "-" + tertip.adlar[4];
            }
        }

        /// <summary>
        /// Ödenek bilgisi tutulan tertip
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Tertibin ödeneklerini tutan Collection. Bu Collection double elemanlar tutacaktır. 
        /// Dizinin ilk elemanı (0. eleman) bütçesi yapılan yıla karşılık gelecek, sonraki elemanlar ise sonraki yılların rakamları olacaktır.
        /// </summary>
        public TNSCollection odenekler = new TNSCollection();

        /// <summary>
        /// Tertible ilgili ödenek harici ek rakamları tutan Collection. Bu Collection double elemanlar tutacaktır. 
        /// </summary>
        public TNSCollection ekRakamlar = new TNSCollection();

        public double EkRakam(int indis)
        {
            if (ekRakamlar[indis] is double)
                return (double)ekRakamlar[indis];
            else return 0;
        }
        /// <summary>
        /// Ödenek Bilgi nesnesi oluşturan oluşturucu yordam. Tertiple ilgili bilgiler parametre olarak geçirilir.
        /// </summary>
        /// <param name="yil">Tertibin yılı</param>
        /// <param name="tabloNo">Tertibin yapısı. Bu listede bulunan her bir tablo numarası değişken dosyasında her yıl için yapılandırılabilir.
        /// Eğer ilgili yılda bu tablo bilgisi bulunamazsa, 0 yılına ait bilgi alınır. 0 yılı da bulunamazsa hata oluşur.
        /// </param>
        public OdenekBilgi(int yil, ENUMTabloKod tabloNo)
        {
            tertip = new Tertip(yil, tabloNo);
        }
    }

    /// <summary>
    /// Sunucudan istemciye, yada istemciden sunucuya bir tertip ve tertiple ilgili rakamları taşırken kullanılan sınıf.
    /// Bu sınıfta tertiple birlikte ödenek dizisi ve tertip açıklaması tutulmaktadır.
    /// </summary>
    [System.Serializable()]
    public class TertipOdenek
    {
        /// <summary>
        /// Ödenek bilgisi tutulan tertip
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Tertibin ödeneklerini tutan dizi.
        /// Dizinin elemanları tertiple ilgili ödenek, gerçekleşme yada tahmin gibi bilgiler tutmaktadır.
        /// </summary>
        public double[] odenek;
        /// <summary>
        /// Tertipin açıklaması, tertiple ilgili kaydedilmiş bir açıklama varsa, bu alanda tutulacaktır.
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Tertip ödenek nesnesi oluşturan oluşturucu yordam. 
        /// Dizi elemanı sayısı verilmediğinden, çok yıllı bilgi taşıyabilmek için 3 elemanlı rakam dizisi oluşturulur.
        /// </summary>
        public TertipOdenek()
        {
            odenek = new double[3];
        }

        /// <summary>
        /// Tertip ödenek nesnesi oluşturan oluşturucu yordam. 
        /// Dizi elemanı sayısı olduğu için, verilen sayı kadar rakam dizisi oluşturulur.
        /// </summary>
        public TertipOdenek(int odenekSay)
        {
            odenek = new double[odenekSay];
        }

        public TertipOdenek(OdenekBilgi item)
        {
            this.tertip = item.tertip;
            this.odenek = (double[])item.ekRakamlar.ToArray(typeof(double));
        }
    }

    /// <summary>
    /// Üçgen yada normal tertip listesi üretilecekken, sonuç kümede bulunacak boyutun bilgisi. 
    /// Hangi boyutun, hangi kırılım sayısında döndürüleceği bilgisini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class UcgenKodBilgi
    {
        /// <summary>
        /// Tertip içi boyut numarası. Değişken.xml'de boyut bilgisi için tanımlı numara ile aynı değerlerde olmalıdır.
        /// Varsayılan değer olarak kurumsal kod alınmıştır.
        /// </summary>
        public int kodNo = 1;

        /// <summary>
        /// Bu boyutun tanımlı kırılımlarının kaç tanesi cevap kümesinde yer alacak.
        /// Örneğin 99.9.9.99 olarak tanımlı bür boyutun, kirilim sayısı 1 ise, 99; 2 ise 99.9; 3 ise 99.9.9 ve 4 ise 99.9.9.99 kısmı alınır.
        /// </summary>
        public int kirilimSay = 0;

        /// <summary>
        /// Bazı ekonomik kodların 2 bazı ekonomik kodların ise 3 kırılım olarak alınacağını gösterir.
        /// </summary>
        public bool eko23;
        /// <summary>
        /// Ucgen kod bilgisi nesnesi oluşturan oluşturucu yordam. 
        /// Döndürülecek listede bulunacak olan boyutun numarası ve kırılım sayısı verilmelidir.
        /// </summary>
        /// <param name="p_kodNo">Talep edilen boyutun değişken.xml'deki kod numarası</param>
        /// <param name="p_kirilimSay">Talep edilen boyutun kırılımlarının kaç tanesinin döndürüleceği</param>
        public UcgenKodBilgi(int p_kodNo, int p_kirilimSay)
        {
            kodNo = p_kodNo;
            kirilimSay = p_kirilimSay;
            this.eko23 = false;
        }

        public UcgenKodBilgi(int p_kodNo, int p_kirilimSay, bool eko23)
        {
            kodNo = p_kodNo;
            kirilimSay = p_kirilimSay;
            if (p_kodNo == 4 && p_kirilimSay == 2)
                this.eko23 = eko23;
            else
                this.eko23 = false;
        }
    }

    /// <summary>
    /// Üçgen yada normal tertip listesi üretilecekken, sonuç listesinde oluşacak boyutların sırasını tutan sınıf.
    /// </summary>
    [Serializable()]
    public class UcgenKodBilgileri
    {
        /// <summary>
        /// Döndürülecek boyutların bilgilerini tutan dizi
        /// </summary>
        private TNSCollection boyutlar = new TNSCollection();

        /// <summary>
        /// Döndürülecek ve listeye eklenmil olan boyut sayısı
        /// </summary>
        public int BoyutSayisi
        {
            get
            {
                return boyutlar.Count;
            }
        }

        /// <summary>
        /// Yeni boyut bilgisi listeye eklenir
        /// </summary>
        /// <param name="ukb"></param>
        public void Add(UcgenKodBilgi ukb)
        {
            boyutlar.Add(ukb);
        }

        /// <summary>
        /// Listedeki verilen sıradaki boyut bilgisi döndürülür.
        /// </summary>
        /// <param name="i">Hangi sırdaki boyut bilgisinin döndürüleceği</param>
        /// <returns>Verilen sıradaki boyut bilgisi UcgenKodBilgi yapısında döndürülür.</returns>
        public UcgenKodBilgi this[int i]
        {
            get
            {
                return (UcgenKodBilgi)boyutlar[i];
            }
            set
            {
                boyutlar[i] = value;
            }
        }

        public int KirilimSayisi
        {
            get
            {
                int say = 0;
                for (int i = 0; i < BoyutSayisi; i++)
                {
                    say += this[i].kirilimSay;
                }
                return say;
            }
        }
    }

    /// <summary>
    /// Tertip listesi istenirken, üçgen formatta istenmiş ise, üçgen listenin her bir satırına ait bilgiyi tutan sınıf.
    /// Üçgen satırında yer alan kod parçasının; yeri, parçanın içeriği, açıklaması ve ek bilgileri tutulur.
    /// </summary>
    [Serializable()]
    public class UcgenSatir
    {
        /// <summary>
        /// Kod parçasının, sonuç kümede yer aldığı kolon numarası. 0'dan başlar
        /// </summary>
        public int yer = 0;

        /// <summary>
        /// Sonuç kümesindeki kod parçasının değeri
        /// </summary>
        public string kodParca = string.Empty;

        /// <summary>
        /// Sonuç satırına karşılık gelen, tüm koda ait açıklama.
        /// </summary>
        public string aciklama = string.Empty;

        /// <summary>
        /// Sonuç satırına karşılık tutulan ek bilgiler.
        /// </summary>
        public TNSCollection ekDegerler = new TNSCollection();
    }

    /// <summary>
    /// Harcama birimlerini hiyerarşik olarak tutabilmek için, hiyerarşinin her bir noktasını tutan sınıf. 
    /// </summary>
    [System.Serializable()]
    public class KurumsalYapi
    {
        public static int BirimTurBul(string kurumsalYapiKod)
        {
            if (string.IsNullOrWhiteSpace(kurumsalYapiKod) || kurumsalYapiKod.Length < 1) return 9;
            int birimTur = OrtakFonksiyonlar.ConvertToInt(kurumsalYapiKod.Substring(0, 1), 9);
            if (birimTur == 1) return 1;
            if (birimTur == 3) return 4;
            if (birimTur == 2)
            {
                string birimKod = "";
                if (kurumsalYapiKod.Length >= 6)
                {
                    birimKod = kurumsalYapiKod.Substring(3, 3);
                }
                if (birimKod == "100" || birimKod == "150")
                    return 2;
                else
                    return 3;
            }
            else
                return birimTur;
        }

        public static int IlkodBul(string kurumsalYapiKod)
        {
            if (string.IsNullOrWhiteSpace(kurumsalYapiKod) || kurumsalYapiKod.Length < 3) return 1000;
            int ilKod = OrtakFonksiyonlar.ConvertToInt(kurumsalYapiKod.Substring(1, 2), 1000);
            if (ilKod == 0) return 6;
            return ilKod;
        }

        /// <summary>
        /// Harcama birimi kodu. Bu kod değişken xml'de tanımlı 17 numaralı kod ile uyumlu olmalıdır.
        /// </summary>
        public string kod { get; set; } = string.Empty;

        /// <summary>
        /// Harcama biriminin adı
        /// </summary>
        public string ad { get; set; } = string.Empty;

        /// <summary>
        /// Harcama biriminin saymanlık kodu.
        /// </summary>
        public int saymanlikKod = 0;

        /// <summary>
        /// Harcama biriminin harcama birimi kodu.
        /// </summary>
        public string birimKod = string.Empty;

        /// <summary>
        /// Harcama biriminin kurumsal kodu.
        /// </summary>
        public string kurKod = string.Empty;


        //public string birimAd = string.Empty;
        //public string saymanlikAd = string.Empty;
        //public string kurAd = string.Empty;

        //public string kisaAd = string.Empty;
        //public string evrakBirimKod = string.Empty;
        //public string ulkeAd = string.Empty;
        //public string ulkeKod = string.Empty;
        //public string bolgeAd = string.Empty;
        //public string bolgeKod = string.Empty;
        //public string ilAd = string.Empty;
        //public string ilKod = string.Empty;
        //public string ilceAd = string.Empty;
        //public string ilceKod = string.Empty;
        //public string fonKod = string.Empty;
        //public string fonAd = string.Empty;
        //public string finKod = string.Empty;
        //public string finAd = string.Empty;
        //public string ekoKod = string.Empty;
        //public string ekoAd = string.Empty;
        //public string pebKod = string.Empty;
        //public string pebAd = string.Empty;
        //public string vergiNo = string.Empty;
        //public string vergiDairesi = string.Empty;
        //public string adres = string.Empty;
        //public string telefon = string.Empty;
        //public string faks = string.Empty;
        //public string eposta = string.Empty;
        //public string url = string.Empty;
        //public string antet = string.Empty;
        //public int ustKod = -1;
        //public string teskilatKod = string.Empty;
    }

    [System.Serializable()]
    public class TorbaTertipBirimOdenek
    {
        public int yil;
        public ArrayList torbatertipler;
        public ArrayList birimler;
        public TorbaTertipBirimOdenek()
        {
            yil = 0;
            torbatertipler = new ArrayList();
            birimler = new ArrayList();
        }
        public TorbaTertipBirimOdenek(int yil)
        {
            this.yil = yil;
        }
    }

    [System.Serializable()]
    public class TorbaTertip : Tertip
    {
        public double miktar;
        public TorbaTertip(int yil, ENUMTabloKod tKod)
            : base(yil, tKod)
        {
            miktar = 0.0;
        }
    }

    [System.Serializable()]
    public class MuhasebeBirim
    {
        public int muhasebeKod;
        public string muhasebeAd;
        public string birimKod;
        public string birimAd;
        public string kurkod;
        public double miktar;
        public MuhasebeBirim()
        {
            muhasebeKod = 0;
            muhasebeAd = string.Empty;
            birimKod = string.Empty;
            birimAd = string.Empty;
            kurkod = string.Empty;
            miktar = 0.0;
        }
        public MuhasebeBirim(int muhasebeKod, string muhasebeAd, string birimKod, string birimAd)
        {
            this.muhasebeKod = muhasebeKod;
            this.muhasebeAd = muhasebeAd;
            this.birimKod = birimKod;
            this.birimAd = birimAd;
        }

    }

    [Serializable]
    public class BirimTorbaOdenekTalep
    {
        public int yil;
        public MuhasebeBirim birim;
        public ArrayList detay;
        public BirimTorbaOdenekTalep()
        {
            yil = 0;
            birim = new MuhasebeBirim();
            detay = new ArrayList();
        }
    }

    [Serializable]
    public class BirimTorbaOdenekTalepDetay
    {
        public int sirano;
        public string isadi;
        public string talepgerekce;
        public double miktar;
        public int durum;
        public string redgerekce;
        public BirimTorbaOdenekTalepDetay()
        {
            sirano = 0;
            isadi = string.Empty;
            talepgerekce = string.Empty;
            miktar = 0.0;
            durum = 0;
            redgerekce = string.Empty;
        }
    }

    /// <summary>
    /// Bütçe hazırlık sınıfı için; istemci ve sunucuda ortak kullanılacak statik fonksiyonlar içeren sınıf
    /// </summary>
    public class Arac
    {
        public static string KurumsalKodYapiBul(int muhasebe, string birim, string kurKod)
        {
            return KurumsalKodYapiBul(TNS.BHMv2.Arac.Tanimla(), muhasebe, birim, kurKod);
        }

        public static string KurumsalKodYapiBul(IBHMv2Servis servis , int muhasebe, string birim, string kurKod)
        {
            string donusDeger = string.Empty;
            string[] liste = servis.KurumsalYapiListele(muhasebe.ToString(), birim, kurKod);
            if (liste.Length > 0)
                donusDeger = liste[0];

            return donusDeger;
        }

        private static Hashtable programButceKodlar = new Hashtable();
        public static void ProgramButceKodCevirSifirla(int yil)
        {
            if (programButceKodlar.Contains(yil))
                programButceKodlar[yil] = null;
        }

        public static string ProgramButceKodaCevir(int yil, int kodNo, string kod)
        {
            if (programButceKodlar[yil] == null)
            {
                programButceKodlar[yil] = TNS.BHMv2.Arac.Tanimla().ProgramButceCevrimOku(yil);
            }
            if (programButceKodlar[yil] != null)
            {
                Hashtable ht = (Hashtable)programButceKodlar[yil];
                return (string)ht[kodNo + "_" + kod];
            }
            return null;
        }

        public static string ProgramButceKodaCevir(int yil, int kodNo, string kod, out long p, out long ap, out long f, out long af)
        {
            if (programButceKodlar[yil] == null)
            {
                programButceKodlar[yil] = TNS.BHMv2.Arac.Tanimla().ProgramButceCevrimOku(yil);
            }
            string cevap;
            string[] kodlar;
            cevap = "";
            p = 0;
            ap = 0;
            f = 0;
            af = 0;
            Hashtable ht = (Hashtable)programButceKodlar[yil];
            if (ht != null)
            {
                cevap = (string)ht[kodNo + "_" + kod];
                if (!string.IsNullOrWhiteSpace(cevap))
                {
                    kodlar = cevap.Split('.');
                    p = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(kodlar, 0, ""), 0);
                    ap = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(kodlar, 1, ""), 0);
                    f = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(kodlar, 2, ""), 0);
                    af = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(kodlar, 3, ""), 0);
                }
            }
            return cevap;
        }

        public static UcgenSatir[] UcgenTertipOl(Kullanici kullanan, UcgenKodBilgileri kodBilgileri, bool sifirlarHaric, OdenekBilgi[] obler, int yil)
        {
            ArrayList donusDeger = new ArrayList();
            IUZYServis uzS = TNS.UZY.Arac.Tanimla();
            int kirilimSay = 0;
            int alanSay = kodBilgileri.BoyutSayisi;
            if (alanSay == 0) return null;

            string[] degerTablo = new string[alanSay];
            string[] degerTabloAlanAd = new string[alanSay];
            int[] degerSay = new int[alanSay];
            string[] degerMaske = new string[alanSay];
            //string ekKosul = string.Empty;
            Hashtable hashTable = new Hashtable();
            ButceKodYapi bky;
            IBKDServis bkdServis = TNS.BKD.Arac.Tanimla();

            for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
            {
                UcgenKodBilgi ukb = kodBilgileri[i];
                bky = bkdServis.ButceKodBilgiBul(yil, ukb.kodNo, string.Empty);
                degerTablo[i] = bky.tabloAd;
                degerTabloAlanAd[i] = bky.vtAlanAd + AlanEkBul();
                degerSay[i] = kodBilgileri[i].kirilimSay;
                degerMaske[i] = bky.kodYapi;
                kirilimSay += kodBilgileri[i].kirilimSay;
            }

            string[] eskiDegerler = new string[kirilimSay];
            string[] yeniDegerler = new string[kirilimSay];
            UcgenSatir[] SonUcgenSatir = new UcgenSatir[kirilimSay];
            for (int i = 0; i < kirilimSay; i++) eskiDegerler[i] = "x";

            foreach (OdenekBilgi ob in obler)
            {
                string tertip = "";
                for (int i = 0; i < kodBilgileri.BoyutSayisi; i++)
                {
                    if (i > 0) tertip += "-";
                    for (int j = 0; j < ob.tertip.kodlar.Length; j++)
                    {
                        if (ob.tertip.KodNolar(j) == kodBilgileri[i].kodNo)
                        {
                            tertip += ob.tertip.kodlar[j];
                            break;
                        }
                    }
                }
                yeniDegerler = tertip.Split('.', '-');

                string aciklamaKod = string.Empty;
                string[] boyutDeger = new string[kodBilgileri.BoyutSayisi];
                int boyutYer = -1;
                for (int kirilimYer = 0; kirilimYer < kirilimSay; kirilimYer++)
                {
                    string yeniDegerler_kirilimYer = OrtakFonksiyonlar.DiziElemanAl(yeniDegerler, kirilimYer, "");

                    if (eskiDegerler[kirilimYer] == yeniDegerler_kirilimYer)
                    {
                        KirilimParcaEkle(yeniDegerler, kirilimYer, degerSay, boyutDeger, ref boyutYer);
                        RakamlariEkle(SonUcgenSatir[kirilimYer], ob);
                    }
                    else
                    {
                        for (; kirilimYer < kirilimSay; kirilimYer++)
                        {
                            yeniDegerler_kirilimYer = OrtakFonksiyonlar.DiziElemanAl(yeniDegerler, kirilimYer, "");
                            KirilimParcaEkle(yeniDegerler, kirilimYer, degerSay, boyutDeger, ref boyutYer);
                            UcgenSatir us = new UcgenSatir();
                            us.yer = kirilimYer;
                            us.kodParca = yeniDegerler_kirilimYer;
                            us.aciklama = UcgenAciklamaBul(uzS, hashTable, kullanan, yil, boyutYer, degerTablo[boyutYer], degerTabloAlanAd[boyutYer], boyutDeger[boyutYer]);
                            if (kirilimYer == kirilimSay - 1)
                            {
                                if (!string.IsNullOrWhiteSpace(ob.aciklama))
                                    us.aciklama += " (" + ob.aciklama + ")";
                            }
                            donusDeger.Add(us);
                            eskiDegerler[kirilimYer] = yeniDegerler_kirilimYer;
                            SonUcgenSatir[kirilimYer] = us;
                            RakamlariEkle(SonUcgenSatir[kirilimYer], ob);
                        }
                    }
                }
            }
            return (UcgenSatir[])donusDeger.ToArray(typeof(UcgenSatir));
        }

        /// <summary>
        /// Kodun açıklaması bulunurken, seçilecek alanın adı, aktif kültüre göre belirlenir.
        /// </summary>
        /// <returns>Dile duyarlı alan adı, alan adına eklenecek ek bir karakter ile bulunur.</returns>
        public static string AlanEkBul()
        {
            string kulturAd = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            if (kulturAd.StartsWith("tr")) return "";
            else return "2";
        }

        /// <summary>
        /// Her bir üçgen satırda veya liste için bir satırda yer alan boyut kodlarının açıklamasını bulan yordam.
        /// Bu yordam seçilen ilk alan hariç diğer alanlar için bulduğu kodları hashtable'da tutar. 
        /// Bu sayede listede tekrarlanan kodlar için tekrar veri tabanına gidilmez.
        /// </summary>
        /// <param name="uzS">Kod açıklamasının bulunması için kullanılacak uzay bağlantı nesnesi.</param>
        /// <param name="hashTable">Kod açıklamasının aranacağı hashtable. Kod bulunamazsa uzaydan açıklama bulunur ve hashtable'a eklenir.</param>
        /// <param name="kullanan">Uzaya parametre olarak geçirilecek uzay nesnesi</param>
        /// <param name="yil">Kodlar yıl bazlı tutulduğundan, kod açıklaması bulunacakken kullanılacak yıl nesnesi.</param>
        /// <param name="kodNo">Kodun seçim listesindeki ve dolayısıyla bilgi alanlarındaki yeri.</param>
        /// <param name="uzayAd">Kodun açıklamasının bulunacağı uzayın ismi.</param>
        /// <param name="uzayAlanAd">Kodun açıklamasının bulunacağı alanın ismi.</param>
        /// <param name="kodUzayKosul">Açıklaması bulunacak kodun, uzay koşulu.</param>
        /// <returns></returns>
        public static string UcgenAciklamaBul(IUZYServis uzS, System.Collections.Hashtable hashTable, Kullanici kullanan, int yil, int kodNo, string uzayAd, string uzayAlanAd, string kodUzayKosul)
        {
            string acik = string.Empty;
            string anahtar = kodNo.ToString() + "-" + kodUzayKosul + "-" + yil.ToString();
            acik = (string)hashTable[anahtar];
            if (acik == null)
            {
                acik = AciklamaSorgulaCokYil(uzS, kullanan, uzayAd, yil, kodUzayKosul, uzayAlanAd);
                hashTable.Add(anahtar, acik);
            }
            return acik;
        }

        private static string AciklamaSorgulaCokYil(IUZYServis uzS, Kullanici kullanan, string uzayAd, int yil, string kodUzayKosul, string alanAd)
        {
            if (uzayAd == "BHMKURUMSALYAPIVIEW" || uzayAd == "BHMKURUMSALYAPI")
            {
                return uzS.UzayDegeriStr(kullanan, uzayAd, kodUzayKosul, true, alanAd);
            }
            else
            {
                string acik = uzS.UzayDegeriStr(kullanan, uzayAd, yil.ToString() + "-" + kodUzayKosul, true, alanAd);
                if (acik == string.Empty)
                    acik = uzS.UzayDegeriStr(kullanan, uzayAd, (yil - 1).ToString() + "-" + kodUzayKosul, true, alanAd);
                if (acik == string.Empty)
                    acik = uzS.UzayDegeriStr(kullanan, uzayAd, (yil - 2).ToString() + "-" + kodUzayKosul, true, alanAd);
                return acik;
            }
        }

        private static void RakamlariEkle(UcgenSatir us, OdenekBilgi ob)
        {
            if (us.ekDegerler == null) us.ekDegerler = new TNSCollection();
            int yer = 0;
            foreach (double ekRakam in ob.ekRakamlar)
            {
                if (yer < us.ekDegerler.Count)
                {
                    double eski = (double)us.ekDegerler[yer];
                    us.ekDegerler[yer] = ekRakam + eski;
                }
                else
                    us.ekDegerler.Add(ekRakam);
                yer++;
            }
        }

        private static void KirilimParcaEkle(string[] yeniDegerler, int kirilimYer, int[] degerSay, string[] boyutDeger, ref int boyutYer)
        {
            int yerToplam = 0;
            for (int i = 0; i < degerSay.Length; i++)
            {
                yerToplam += degerSay[i];
                if (kirilimYer < yerToplam)
                {
                    if (boyutDeger[i] == null) boyutDeger[i] = OrtakFonksiyonlar.DiziElemanAl(yeniDegerler, kirilimYer, "");
                    else boyutDeger[i] += "." + OrtakFonksiyonlar.DiziElemanAl(yeniDegerler, kirilimYer, "");
                    boyutYer = i;
                    return;
                }
            }
            throw new Exception("Yazılım algoritması hatası: Gelen kırılım parçasını, alana ekleyemedik!");
        }

        public static OdenekBilgi[] TertipListele(TNS.KYM.Kullanici kullanan, TNS.BHMv2.IBHMv2Servis servisBHMv2, int belgeTur, int yil, int muhasebe, string birim, string kur, string fon, string fin, string ekoKistas, string pebKistas, TNS.MUH.ENUMOdenekKontrolTur ogKontrolTur, TNS.MUH.ENUMOdenekUstuKontrol ouKontrol, bool tumTertipleriGoster, out bool mpOnayli)
        {
            //TertipListele(
            string ortak000 = "";
            try
            {
                ortak000 = TNS.DEG.Arac.DegiskenDegerBulCache(yil, "/Ortak000");
            }
            catch { }

            if (ortak000 == "1")
                birim = "";

            EkDegerParam[] ekBilgiler = null;

            Tertip kistasTertip = new Tertip(yil, ENUMTabloKod.TABOdenekGonderme, kur, "", "", "", "", "@" + muhasebe + "." + birim);

            UcgenKodBilgileri kodBilgileri = new UcgenKodBilgileri();

            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(0), kistasTertip.KodNoKirilimSay(0)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(1), kistasTertip.KodNoKirilimSay(1)));
            kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(2), kistasTertip.KodNoKirilimSay(2)));

            if ((belgeTur != 4 && ogKontrolTur != TNS.MUH.ENUMOdenekKontrolTur.E4POG_E4POE_E4PK) ||
                  (belgeTur == 4 && (ogKontrolTur == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K || ogKontrolTur == TNS.MUH.ENUMOdenekKontrolTur.E2POG_E4POE_E2PK_E4PTALEP)))
            {
                ekoKistas = IkileEko(yil, ekoKistas, TNS.BKD.Arac.Eko2_3Bul(kullanan));
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), 2, TNS.BKD.Arac.Eko2_3Bul(kullanan)));
            }
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(3), kistasTertip.KodNoKirilimSay(3)));

            if ((belgeTur != 4 && (ogKontrolTur == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K || ogKontrolTur == TNS.MUH.ENUMOdenekKontrolTur.E4POG_E4POE_E2K)) ||
                (belgeTur == 4 && (ogKontrolTur == TNS.MUH.ENUMOdenekKontrolTur.E2OG_E4OE_E2K)))
                pebKistas = "";
            else
                kodBilgileri.Add(new UcgenKodBilgi(kistasTertip.KodNolar(4), kistasTertip.KodNoKirilimSay(4)));

            kistasTertip.kodlar[1] = fon;
            kistasTertip.kodlar[2] = fin;
            kistasTertip.kodlar[3] = ekoKistas;
            kistasTertip.kodlar[4] = pebKistas;

            mpOnayli = servisBHMv2.MasrafPlaniGenelOnayDurumBul(kullanan, yil, "", ENUMMasrafPlanAsama.AHP) == 5;
            if (mpOnayli)
            {
                ekBilgiler = new EkDegerParam[4];
                ekBilgiler[3] = new EkDegerParam(ENUMTabloKod.TABMasrafPlaniBirim, 0, 0, "9");
            }
            else
                ekBilgiler = new EkDegerParam[3];

            if (ouKontrol == TNS.MUH.ENUMOdenekUstuKontrol.ODENEK)
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABButce, 0, 0, "");
            else
                ekBilgiler[0] = new EkDegerParam(ENUMTabloKod.TABOdenekGonderme, 0, 0, ";5");

            ekBilgiler[1] = new EkDegerParam(ENUMTabloKod.TABOdemeEmri, 0, 0, "");
            ekBilgiler[2] = new EkDegerParam(ENUMTabloKod.TABHYBloke, 0, 0, "");

            return servisBHMv2.ListeTertipOl(kullanan, kistasTertip, kodBilgileri, ekBilgiler, !tumTertipleriGoster, tumTertipleriGoster);
        }

        public static string IkileEko(int yil, string ekoKistas, bool eko23)
        {
            string donusDeger = "";
            if (string.IsNullOrWhiteSpace(ekoKistas)) return donusDeger;
            string[] ekoParca = ekoKistas.Split(',', ';');
            for (int i = 0; i < ekoParca.Length; i++)
            {
                if (donusDeger != "") donusDeger += ",";
                donusDeger += IkileEkoParca(yil, ekoParca[i], eko23);
            }
            return donusDeger;
        }

        public static string IkileEkoParca(int yil, string ekoParca, bool eko23)
        {
            ekoParca = ekoParca.Replace(".", "");

            string ekoKodYapi = TNS.MUH.Arac.DegiskenDegerBul(yil, "/KODYAPI/KOD4/KODYAPI");
            int parcaUz;
            int ekle;
            if (ekoKodYapi.Length == 11)
            {
                parcaUz = 4;
                ekle = 2;
            }
            else
            {
                parcaUz = 3;
                ekle = 1;
            }

            if (ekoParca.Length > parcaUz)
            {
                int uz = parcaUz;
                if (eko23 && TNS.BKD.Arac.Eko23Mu(yil, ekoParca))
                    uz += ekle;
                return ekoParca.Substring(0, uz);
            }
            return ekoParca;
        }

        public static string TiredenKodBul(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return "";

            string aranan = "-";
            int tireYer = str.IndexOf(aranan);
            if (tireYer > 0)
            {
                return str.Substring(0, tireYer);
            }
            else
                return str;
        }

        public static string BirimAdOl(string saymanlikAd, string birimAd)
        {
            return TNS.MUH.Arac.BirimAdOl(saymanlikAd, birimAd);
            //if (string.IsNullOrWhiteSpace(saymanlikAd)) return birimAd;
            //if (string.IsNullOrWhiteSpace(birimAd)) return saymanlikAd;
            //string[] BirimAdParcalar = saymanlikAd.Split(' ');
            //string IlkAd = BirimAdParcalar[0];
            //if (birimAd.IndexOf(IlkAd) >= 0) return birimAd;
            //else return IlkAd + " " + birimAd;
        }

        public static bool SGBKBSFarkVarMi(TNS.MUH.IMUHServis servisMUH, int yil, Kullanici kullanan, int saymanlik, string birim, out string hata)
        {
            bool yaz = !TNS.BHMv2.Arac.SGBKBSFarkBul(servisMUH, yil, kullanan, saymanlik, birim, out hata);

            return yaz;
        }

        public static bool SGBKBSFarkBul(TNS.MUH.IMUHServis servisMUH, int yil, Kullanici kullanan, int saymanlik, string birim, out string hata)
        {
            OdenekKontrolDegiskenleri.Oku(yil);
            //int[] yilKontrol, int[] ayKontrol, int hizMalKontrolYil, 
            hata = "";
            for (int i = 0; i < degisken.yilKontrol.Length; i++)
            {
                if (degisken.yilKontrol[i] > 0 && degisken.ayKontrol[i] > 0)
                {
                    Sonuc s = servisMUH.BirimHarcamaSayOE(kullanan, degisken.yilKontrol[i], degisken.ayKontrol[i], saymanlik, birim);
                    if (s.islemSonuc)
                    {
                        if (s.kayitSay > 0)
                        {
                            hata = string.Format("{0} yılı, {1}. ayı için KBS ile SGB harcamalarınız farklı olduğundan kayıt işlemi yapamazsınız. Bu farkı düzelttikten sonra işleminize devam edebilirsiniz!", degisken.yilKontrol[i], degisken.ayKontrol[i]);
                            return true;
                        }
                    }
                }
            }
            if (degisken.hizMalKontrolYil > 0)
            {
                TNS.BHMv2.IBHMv2Servis servisBHMv2 = TNS.BHMv2.Arac.Tanimla();
                string kurKod = TNS.DEG.Arac.VarsayilanKurumBul();
                ObjectArray hmv = servisBHMv2.HizmetMaliyetiTasraListele(kullanan, degisken.hizMalKontrolYil, saymanlik, birim, kurKod, false);
                if (!hmv.sonuc.islemSonuc || string.IsNullOrWhiteSpace(hmv.sonuc.anahtar))
                {
                    hata = (degisken.hizMalKontrolYil) + " yılı için hizmet maliyeti tespiti formu girilmemiş! (Hata:" + hmv.sonuc.hataStr + ")";
                    return true;
                }
            }

            return false;
        }

        public static OdenekKontrolDegiskenleri degisken = null;

        public class OdenekKontrolDegiskenleri
        {
            public int[] yilKontrol = new int[2];
            public int[] ayKontrol = new int[2];
            public int hizMalKontrolYil = 0;

            public static void Sifirla()
            {
                degisken = null;
            }

            public static void Oku(int yil)
            {
                if (degisken == null)
                {
                    degisken = new OdenekKontrolDegiskenleri();
                    TNS.KYM.IKYMServis servisKYM = TNS.KYM.Arac.Tanimla();

                    AyKontrolBul(servisKYM, out degisken.yilKontrol[0], out degisken.ayKontrol[0]);

                    DateTime tut = DateTime.Now.AddDays(-1);
                    degisken.ayKontrol[1] = tut.Month;
                    degisken.yilKontrol[1] = tut.Year;

                    string shizMalTesKontrolYili = "";
                    TNSCollection hizMalTesKontrolYili = servisKYM.DegiskenleriListele("stratek", "HizMalTesKontrolYili");
                    if (hizMalTesKontrolYili != null && hizMalTesKontrolYili.Count > 0)
                        shizMalTesKontrolYili = hizMalTesKontrolYili[0].ToString();
                    if (!string.IsNullOrWhiteSpace(shizMalTesKontrolYili))
                        degisken.hizMalKontrolYil = OrtakFonksiyonlar.ConvertToInt(shizMalTesKontrolYili, 0);
                    else
                        degisken.hizMalKontrolYil = 0;
                }
            }

            public static void AyKontrolBul(IKYMServis servisKYM, out int yilKontrol, out int ayKontrol)
            {
                yilKontrol = -1;
                ayKontrol = -1;
                string yilAy = "";
                TNSCollection kDeg = servisKYM.DegiskenleriListele("stratek", "MasrafPlaniKontrolDonem");

                if (kDeg != null && kDeg.Count > 0)
                    yilAy = kDeg[0].ToString();
                if (!string.IsNullOrWhiteSpace(yilAy))
                {
                    string[] degerler = yilAy.Split('-');
                    yilKontrol = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DiziElemanAl(degerler, 0, ""), -1);
                    ayKontrol = OrtakFonksiyonlar.ConvertToInt(TNS.MUH.Arac.DiziElemanAl(degerler, 1, ""), -1);
                }
            }
        }

        public static string[] Durumlar = { "Kapalı", "Açık", "Yöneticilere Açık", "Onaylı" };

        /// <summary>
        /// Bu sunucu sınıfının nesnesini, konfigürasyon dosyasından okuduğu
        /// remoting dizgisini kullanarak, .net remoting ile oluşturup döndüren statik yordam
        /// </summary>
        /// <returns>.net remoting ile oluşturulan nesne</returns>
        public static IBHMv2Servis Tanimla()
        {
            string sUrl = System.Configuration.ConfigurationManager.AppSettings.Get("BHMv2ServisURL");
            IBHMv2Servis servis = TNS.Api.ServisIstemci.Olustur<IBHMv2Servis>(sUrl);
            return servis;
        }

        public static int YuvarlamaTutariBul(double miktar)
        {
            double us = Math.Ceiling(Math.Log10(Math.Abs(miktar)));
            if (us >= 6) us = 4;
            else if (us >= 2) us = us - 2;
            else us = 0;
            return (int)Math.Pow(10, us);
        }

        public static double Yuvarla(double miktar, int yuvarlamaTutari)
        {
            return Math.Round(miktar / yuvarlamaTutari, 0) * yuvarlamaTutari;
        }

        public static double YuvarlaAsagi(double miktar, int yuvarlamaTutari)
        {
            return Math.Floor(miktar / yuvarlamaTutari) * yuvarlamaTutari;
        }

        public static double YuvarlaYukari(double miktar, int yuvarlamaTutari)
        {
            return Math.Ceiling(miktar / yuvarlamaTutari) * yuvarlamaTutari;
        }

        public static int SerbestAyBul(int yil)
        {
            int ay;
            if (yil < DateTime.Now.Year)
                ay = 12;
            else if (yil > DateTime.Now.Year)
                ay = 0;
            else
            {
                //ay = ((DateTime.Now.Month + 2) / 3) * 3;
                ay = DateTime.Now.Month;
            }
            return ay;
        }

        private static IUZYServis _servisUZY = null;
        /// <summary>
        /// Uzay Servisi
        /// </summary>
        private static IUZYServis servisUZY
        {
            get
            {
                if (_servisUZY == null)
                    _servisUZY = TNS.UZY.Arac.Tanimla();
                return _servisUZY;
            }
        }

        public static double OdenekBul(Kullanici kullanan, int yil, string asama, string kurKod, string fonKod, string finKod, string ekoKod)
        {
            return servisUZY.UzayDegeriDbl(kullanan, "ABSODENEK", yil + "-" + asama + "-0-" + kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod, false);
        }

        public static double HarcamaBul(Kullanici kullanan, int yil, string kurumsalYapiKod, int saymanlikKod, string birimKod, string kurKod, string fonKod, string finKod, string ekoKod)
        {
            string sorgu = yil.ToString() + "-" + SaymanlikKodUzayKosulOl(saymanlikKod, true) + "-" + birimKod + "-830-" + kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod + "-0/6-";
            return servisUZY.UzayDegeriDbl(kullanan, "MUHODEMEEMRIMIF", sorgu, false);
        }

        public static string SaymanlikKodUzayKosulOl(int saymanlikKod, bool intMi)
        {
            //if (saymanlikKod <= 0)
            //    return "";
            //if (intMi)
            //{
            //    if (saymanlikKod % 1000 == 0)
            //        return saymanlikKod.ToString() + "/" + (saymanlikKod + 1000).ToString();
            //    return saymanlikKod.ToString();
            //}
            //else
            //{
            //    if (saymanlikKod % 1000 == 0)
            //        return (saymanlikKod / 1000).ToString("00");
            //    return saymanlikKod.ToString("00000");
            //}
            if (saymanlikKod == 0) return "";
            return saymanlikKod.ToString();
        }

        public static double OdenekGondermeBul(Kullanici kullanan, int yil, string kurumsalYapiKod, int saymanlikKod, string birimKod, string kurKod, string fonKod, string finKod, string ekoKod)
        {
            return servisUZY.UzayDegeriDbl(kullanan, "MUHGONDERMEBELGESITENKISBIRIM", (yil - 1).ToString() + "--" + SaymanlikKodUzayKosulOl(saymanlikKod, true) + "-" + birimKod + "-5-" + kurKod + "-" + fonKod + "-" + finKod + "-" + ekoKod, false);
        }

        public static string MasrafPlanAsamaAd(ENUMMasrafPlanAsama mpAsama)
        {
            switch (mpAsama)
            {
                case ENUMMasrafPlanAsama.URETIM: return "Üretim";
                case ENUMMasrafPlanAsama.TALEP: return "Talep";
                case ENUMMasrafPlanAsama.SGB: return "SGB";
                case ENUMMasrafPlanAsama.MERKEZ: return "Merkez";
                case ENUMMasrafPlanAsama.AHP: return "AHP";
                case ENUMMasrafPlanAsama.SONDURUMONAYLI: return "Son durum (Onaylı)";
                case ENUMMasrafPlanAsama.SONDURUMHEPSI: return "Son durum (Hepsi)";
            }
            return mpAsama.ToString();
        }



        public static string EklemeDusmeIslemler()
        {
            return ((int)ENUMRevizeFormTip.MasrafPlaniAktarma).ToString() + "," + ((int)ENUMRevizeFormTip.Ekleme).ToString() + "," + ((int)ENUMRevizeFormTip.KurumAktarma).ToString() + "," + ((int)ENUMRevizeFormTip.Aktarma).ToString();
        }

        public static ENUMBirimTur BirimTurBul(int saymanlik, string birim)
        {
            if (saymanlik == 0 && birim == "000")
                return ENUMBirimTur.Kurum;
            else if (saymanlik <= 0 || string.IsNullOrWhiteSpace(birim) || birim == "000")
                return ENUMBirimTur.Bilinmiyor;
            else if (saymanlik <= 1000)
            {
                if (birim.StartsWith("9")) return ENUMBirimTur.Merkez;
                else return ENUMBirimTur.Tasra_MerkezeBagli;
            }
            else
            {
                //int ilcekod = saymanlik % 1000;
                if (birim.StartsWith("260")) return ENUMBirimTur.Tasra_Il;
                if (birim.StartsWith("25")) return ENUMBirimTur.Tasra_Ilce;
                return ENUMBirimTur.Tasra_MerkezeBagli;
            }
        }

        public static string EvetHayir(int ehKod)
        {
            if (ehKod == 1) return "Evet";
            return "Hayır";
        }

        public static string RevizeFormDurumAdBul(BKD.ENUMRevizeFormDurum formDurum)
        {
            switch (formDurum)
            {
                case BKD.ENUMRevizeFormDurum.Musvette: return "Müsvette";
                case BKD.ENUMRevizeFormDurum.Taslak: return "Taslak";
                case BKD.ENUMRevizeFormDurum.Iletildi: return "İletildi";
                case BKD.ENUMRevizeFormDurum.Onayli: return "Onaylı";
                case BKD.ENUMRevizeFormDurum.Iptal: return "İptal";
                case BKD.ENUMRevizeFormDurum.EskiAsama: return "Eski Aşama";
                case BKD.ENUMRevizeFormDurum.IslemleIptal: return "İşlemle İptal";
                case BKD.ENUMRevizeFormDurum.IptalOlmayan: return "İptal Olmayan";
                case BKD.ENUMRevizeFormDurum.TANIMSIZ: return "Tanımsız";
                default: return "";
            }
        }

        public static Hashtable Eko2Uzlar = new Hashtable();
        public static int Eko2UzBul(int yil)
        {
            if (Eko2Uzlar.ContainsKey(yil))
            {
                return (int)Eko2Uzlar[yil];
            }
            else
            {
                Tertip t = new Tertip(yil, ENUMTabloKod.TABPEBOdenek);
                int eko2Uz = t.KodUzunlukBul(3, 2, true);
                Eko2Uzlar.Add(yil, eko2Uz);
                return eko2Uz;
            }
        }

        public static string ProgramTertipOl(Tertip tertip, bool kurKodSifirlariAt)
        {
            return ProgramTertipOl(tertip.yil, tertip.kodlar[0], tertip.kodlar[1], tertip.kodlar[2], tertip.kodlar[3], tertip.kodlar[4], kurKodSifirlariAt);
        }

        public static string ProgramTertipOl(int yil, string tertip, bool kurKodSifirlariAt)
        {
            if (string.IsNullOrWhiteSpace(tertip)) return "";
            string[] kodlar = tertip.Split('-');
            return ProgramTertipOl(yil, OrtakFonksiyonlar.DiziElemanAl(kodlar, 0, ""), OrtakFonksiyonlar.DiziElemanAl(kodlar, 1, ""), OrtakFonksiyonlar.DiziElemanAl(kodlar, 2, ""), OrtakFonksiyonlar.DiziElemanAl(kodlar, 3, ""), OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, ""), kurKodSifirlariAt);
        }


        public static string ProgramTertipOl(int yil, string kurKod, string fonKod, string finKod, string ekoKod, string pebKod, bool kurKodSifirlariAt)
        {
            //if (ekoKod == "03.03" || ekoKod == "03.04" || ekoKod == "01.03")
            //{
            //    ekoKod += "." + fonKod.Substring(fonKod.Length - 1) + "0";
            //}
            if (kurKodSifirlariAt)
                kurKod = TNS.BKD.Arac.SifirlariAt(kurKod);

            return TNS.BHMv2.Arac.ProgramButceKodaCevir(yil, 140, pebKod.Replace(".", "")) + "-" + kurKod + "-" + finKod + "-" + ekoKod;
        }

        public static void ProgramButceKodaCevirSifirla(int yil)
        {
            programButceKodlar[yil] = null;
        }
    }

    public enum ENUMTasraHazirlikOnayDurum : int
    {
        IlceOnayla = 1,
        IlceOnayKaldir = 2,
        IlOnayla = 3,
        IlOnayKaldir = 4,
        KurumOnayla = 5,
        KurumOnayKaldir = 6,
    }

    public enum ENUMBirimTur : int
    {
        Bilinmiyor = 0,
        Merkez = 1,
        Tasra_Il = 2,
        Tasra_Ilce = 3,
        Tasra_MerkezeBagli = 4,
        Kurum = 5
    }

    [System.Serializable()]
    public class OdenekSenaryoDurum
    {
        public string senaryoKod { get { return senaryo.kod; } set { senaryo.kod = value; } }

        public string senaryoAd { get { return senaryo.ad; } set { senaryo.ad = value; } }

        public string durumAd
        {
            get
            {
                return TNS.MUH.Arac.DiziElemanAl(TNS.BHMv2.Arac.Durumlar, onayDurum, "");
            }

            set
            {
                onayDurum = Array.IndexOf<string>(TNS.BHMv2.Arac.Durumlar, value);
            }
        }

        ButceSenaryo _senaryo = null;
        public ButceSenaryo senaryo { get { if (_senaryo == null) _senaryo = new ButceSenaryo(); return _senaryo; } set { _senaryo = value; } }

        public int onayDurum { get; set; }
    }

    /// <summary>
    /// ProgramOnay: 
    /// </summary>
    [System.Serializable()]
    public class ProgramOnay
    {
        private int _butceYili;
        /// <summary>
        /// Bütçe Yılı: Bütçe yılını giriniz.
        /// </summary>
        public int butceYili { get { return _butceYili; } set { _butceYili = value; } }

        private string _senaryoKod;
        /// <summary>
        /// Senaryo Kod: Senaryo kodunu giriniz.
        /// </summary>
        public string senaryoKod { get { return _senaryoKod; } set { _senaryoKod = value; } }

        private string _programKodu;
        /// <summary>
        /// Program Kodu: Program kodunu giriniz.
        /// </summary>
        public string programKodu { get { return _programKodu; } set { _programKodu = value; } }

        public string programAdi { get; set; }

        private int _durum;
        /// <summary>
        /// Durum: Onay kodunu giriniz.
        /// </summary>
        public int durum { get { return _durum; } set { _durum = value; } }
    }

    /// <summary>
    /// BirimDegisken: 
    /// </summary>
    [System.Serializable()]
    public class BirimDegiskenYapi
    {
        public BirimDegiskenTanim[] degiskenler { get; set; }

        public BirimDegiskenTanim[] formuller { get; set; }

        public BirimDegiskenTanim DegiskenBul(string degiskenAd)
        {
            return DizideBul(degiskenler, degiskenAd);
        }

        private BirimDegiskenTanim DizideBul(BirimDegiskenTanim[] dizi, string degiskenAd)
        {
            if (dizi != null)
            {
                for (int i = 0; i < dizi.Length; i++)
                {
                    if (dizi[i].degiskenAdi == degiskenAd) return dizi[i];
                }
            }
            return null;
        }
    }

    /// <summary>
    /// BirimDegisken: 
    /// </summary>
    [System.Serializable()]
    public class BirimDegisken
    {
        private string _degiskenAdi;
        /// <summary>
        /// Degisken Adı: Değişken Adını giriniz
        /// </summary>
        public string degiskenAdi { get { return _degiskenAdi; } set { _degiskenAdi = value; } }

        private int _degerInt;
        /// <summary>
        /// DegerInt: Tamsayı değer giriniz.
        /// </summary>
        public int degerInt { get { return _degerInt; } set { _degerInt = value; } }

        private double _degerDbl;
        /// <summary>
        /// DegerDbl: Kesirli sayı giriniz.
        /// </summary>
        public double degerDbl { get { return _degerDbl; } set { _degerDbl = value; } }

        private string _degerStr;
        /// <summary>
        /// DegerStr: Yazı giriniz.
        /// </summary>
        public string degerStr { get { return _degerStr; } set { _degerStr = value; } }

        public string DegiskenDegerStr(VariableType variableType)
        {
            if (variableType == VariableType.Money) return degerDbl.ToString("#,###,00");
            else if (variableType == VariableType.String) return degerStr;
            else if (variableType == VariableType.Integer) return degerInt.ToString();
            return "";
        }

        public string deger { get; set; }


        public void DegerAyarla(VariableType variableType)
        {
            if (variableType == VariableType.Integer) degerInt = OrtakFonksiyonlar.ConvertToInt(deger, 0);
            else if (variableType == VariableType.Money) degerDbl = OrtakFonksiyonlar.ConvertToDbl(deger);
            else degerStr = deger;
        }
    }

    [Serializable]
    public class BirimDegiskenTanim
    {
        public string degiskenAdi { get; set; }

        public VariableType tip { get; set; }

        public bool ortak { get; set; }

        public string tipStr
        {
            get
            {
                return VariableFunctions.GetTypeUserName(tip);
            }

            set
            {
                if (VariableFunctions.GetTypeUserName(VariableType.Integer) == value) tip = VariableType.Integer;
                else if (VariableFunctions.GetTypeUserName(VariableType.Money) == value) tip = VariableType.Money;
                else if (VariableFunctions.GetTypeUserName(VariableType.String) == value) tip = VariableType.String;
                else tip = VariableType.Bilinmiyor;
            }
        }

        public string _formula;
        public string formula
        {
            get { return _formula; }
            set
            {
                _formula = value; if (!string.IsNullOrWhiteSpace(_formula))
                {
                    tip = VariableType.Money; kisaEtiket = ""; aciklama = ""; ortak = false; siraNo = "";
                }
            }
        }

        public string kisaEtiket { get; set; }

        public string aciklama { get; set; }

        public string siraNo { get; set; }

        public string deger { get; set; }
    }

    /// <summary>
    /// FaaliyetinYapildigiYerDetay: FaaliyetinYapildigiYerDetay
    /// </summary>
    [System.Serializable()]
    public class FaaliyetinYapildigiYerDetay
    {
        private string _ilKod;
        /// <summary>
        /// IlKod: Faaliyetin yapıldığı ili seçiniz.
        /// </summary>
        public string ilKod { get { return _ilKod; } set { _ilKod = value; } }

        private string _ilAdi;
        /// <summary>
        /// IlAdi: Faaliyetin yapıldığı ili seçiniz.
        /// </summary>
        public string ilAdi { get { return _ilAdi; } set { _ilAdi = value; } }
    }

    /// <summary>
    /// BirimButceHazirlikKarsilanan: Taşra bütçe hazırlığı için kullanılan yapı
    /// </summary>
    [System.Serializable()]
    public class BirimButceHazirlikKarsilanan : IComparer
    {
        private bool faaliyetGrup;

        public BirimButceHazirlikKarsilanan()
        {
            this.faaliyetGrup = false;
        }

        public BirimButceHazirlikKarsilanan(bool faaliyetGrup)
        {
            this.faaliyetGrup = true;
        }
        /// <summary>
        /// İl: İl plaka kodu
        /// </summary>
        public int il { get; set; }

        /// <summary>
        /// Saymanlık: Saymanlık kodu
        /// </summary>
        public int saymanlik { get; set; }

        private int _IlOndeSaymanlik = -1;
        /// <summary>
        /// Saymanlık: Saymanlık kodu
        /// </summary>
        public int IlOndeSaymanlik
        {
            get
            {
                if (_IlOndeSaymanlik < 0)
                {
                    _IlOndeSaymanlik = saymanlik;
                    if (_IlOndeSaymanlik % 1000 == 150) _IlOndeSaymanlik = (_IlOndeSaymanlik / 1000) * 1000;
                }
                return _IlOndeSaymanlik;
            }
        }


        /// <summary>
        /// Birim: Birim Kodu
        /// </summary>
        public string birim { get; set; }

        /// <summary>
        /// Ad: İl Birim Adı
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Planlanan: Masraf planı ile planlanan
        /// </summary>
        public double planlanan { get; set; }

        /// <summary>
        /// Talep Edilen: Birimin talep ettiği
        /// </summary>
        public double talepEdilen { get; set; }

        /// <summary>
        /// Karşılanan: Karşılanan tutar
        /// </summary>
        public double karsilanan { get; set; }

        public double harcama { get; set; }

        /// <summary>
        /// Oran: Karşılama Oranı
        /// </summary>
        public double oran { get; set; }

        public string tertip { get { if (!string.IsNullOrWhiteSpace(tertipTam)) return tertipTam; else return tertipEko2; } }
        public string tertipTam { get; set; }

        public string GenelAciklamaEko2
        {
            get
            {
                return string.Format("{0}:{1},{2}; MP:{3}, TALEP:{4}, KARŞILANAN:{5}", tertipEko2, ekoad, pebad, planlanan, talepEdilen, karsilanan);
            }
        }

        public string tertipEko2 { get; set; }

        public string projeAd { get; set; }

        public string aciklama { get; set; }

        public int Compare(object x, object y)
        {
            if (x is BirimButceHazirlikKarsilanan && y is BirimButceHazirlikKarsilanan)
            {
                BirimButceHazirlikKarsilanan xx = (BirimButceHazirlikKarsilanan)x;
                BirimButceHazirlikKarsilanan yy = (BirimButceHazirlikKarsilanan)y;
                int tut = 0;
                if (faaliyetGrup)
                {
                    tut = xx.tertipEko2.Substring(29, 2).CompareTo(yy.tertipEko2.Substring(29, 2));
                    if (tut != 0) return tut;
                }


                tut = xx.IlOndeSaymanlik.CompareTo(yy.IlOndeSaymanlik);
                if (tut != 0) return tut;

                tut = xx.birim.CompareTo(yy.birim);
                if (tut != 0) return tut;

                tut = xx.tertipEko2.CompareTo(yy.tertipEko2);
                if (tut != 0) return tut;

                string xxeko4 = xx.tertipTam + "";
                string yyeko4 = yy.tertipTam + "";
                return xxeko4.CompareTo(yyeko4);
            }
            else
                return 0;
        }

        public bool detayMi
        {
            get
            {
                if (string.IsNullOrWhiteSpace(tertipTam)) return false;
                else return true;
            }
        }

        public string ekoad { get; set; }

        public string pebad { get; set; }

        public string FaaliyetKodBul()
        {
            return TNS.OrtakFonksiyonlar.DiziElemanAl(tertip.Split('-'), 4, "");
        }
    }

    /// <summary>
    /// IslemTarihce: Yapılan işlemler için tutulan tarihçe
    /// </summary>
    [System.Serializable()]
    public class IslemTarihce
    {
        public int YIL { get; set; }
        public string SENARYOKOD { get; set; }
        public int MUHASEBEKOD { get; set; }
        public string BIRIMKOD { get; set; }
        public int ONAYTUR { get; set; }
        public string ISLEMTARIHSTR
        {
            get
            {
                return ISLEMTARIH.ToLongString();
            }
        }
        public TNSDateTime ISLEMTARIH { get; set; }
        public int ONAYKOD { get; set; }
        public string ISLEMIYAPAN { get; set; }

        public string aciklama
        {
            get
            {
                return IslemAciklamaBul(ONAYKOD);
            }
        }

        private static string IslemAciklamaBul(int ONAYKOD)
        {
            if (ONAYKOD == -1) return "Kaydedildi";
            if (ONAYKOD == 5) return "Onaylandı";
            return "-";
        }

        public string MUHASEBEAD { get; set; }

        public string BIRIMAD { get; set; }

        public string MUHASEBEBIRIMAD
        {
            get
            {
                return TNS.BHMv2.Arac.BirimAdOl(MUHASEBEAD, BIRIMAD);
            }
        }

        public double GIRILEN { get; set; }

        public string DURUMAD
        {
            get
            {
                if (ONAYKOD == 5)
                    return "Onaylı";
                else
                    return "Onaysız";
            }
        }
    }

    /// <summary>
    /// IOIProjeHarcama: Harcama
    /// </summary>
    [System.Serializable()]
    public class IOIProjeHarcama
    {
        public int harcamaNo { get; set; }

        public TNSDateTime harcamaTarihi { get; set; }
        public DateTime harcamaTarihiDT
        {
            get
            {
                if (harcamaTarihi == null)
                    return DateTime.MinValue;
                else
                    return harcamaTarihi.Oku();
            }
            set
            {
                if (harcamaTarihi == null) harcamaTarihi = new TNSDateTime(value);
                else harcamaTarihi.Yaz(value);
            }
        }


        /// <summary>
        /// IOIButce: IOI Bütçesi
        /// </summary>
        public double harcamaTutari { get; set; }
        public string aciklama { get; set; }

        public bool Dolu()
        {
            if (!string.IsNullOrWhiteSpace(aciklama) || harcamaTutari > 0) return true;
            return false;
        }

        public void Kontrol(int satir, Sonuc sonuc)
        {
            if (harcamaTutari < 0) sonuc.DegerAta($"{satir}. satırda negatif rakam girilmiş");
            else if (Dolu() && TNSDateTime.IsNull(harcamaTarihi)) sonuc.DegerAta($"{satir}. satırda tarih girilmemiş");
        }
    }

    /// <summary>
    /// IOIProjeAktarilan: Aktarılan
    /// </summary>
    [System.Serializable()]
    public class IOIProjeAktarilan
    {
        public int tur { get; set; }

        public string turStr
        {
            get
            {
                switch (tur)
                {
                    case 1: return "Tahsis";
                    case 2: return "Eklenen";
                    case 3: return "Düşülen";
                    default:
                        return "";
                }
            }

            set
            {
                switch (value)
                {
                    case "Tahsis": tur = 1; return;
                    case "Eklenen": tur = 2; return;
                    case "Düşülen": tur = 3; return;
                }
                tur = 0;
            }
        }


        public int aktarmaNo { get; set; }

        public TNSDateTime aktarmaTarihi { get; set; }
        public DateTime aktarmaTarihiDT
        {
            get
            {
                if (aktarmaTarihi == null)
                    return DateTime.MinValue;
                else
                    return aktarmaTarihi.Oku();
            }
            set
            {
                if (aktarmaTarihi == null) aktarmaTarihi = new TNSDateTime(value);
                else aktarmaTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// IOIButce: IOI Bütçesi
        /// </summary>
        public double aktarilanTutar { get; set; }
        public string aciklama { get; set; }

        public bool Dolu()
        {
            if (!string.IsNullOrWhiteSpace(aciklama) || aktarilanTutar != 0) return true;
            return false;
        }

        public void Kontrol(int satir, Sonuc sonuc)
        {
            if (Dolu())
            {
                if (tur < 1 || tur > 3) sonuc.DegerAta($"{satir}. satırda lütfen işlem türü seçiniz!");
                else
                {
                    if (aktarilanTutar < 0 && (tur == 1 || tur == 2)) sonuc.DegerAta($"{satir}. satırda negatif rakam girilmiş");
                    if (aktarilanTutar > 0 && tur == 3) sonuc.DegerAta($"{satir}. düşülen işlemlerde rakamı pozitif girmeyiniz!");
                }
                if (TNSDateTime.IsNull(aktarmaTarihi)) sonuc.DegerAta($"{satir}. satırda tarih girilmemiş");
            }
        }
    }

    [System.Serializable()]
    public class IOIDetayBilgi : IComparable
    {
        public TNSDateTime islemTarihi { get; set; }
        public string aciklama { get; set; }
        public double harcamaTutari { get; set; }
        public double aktarilanTutar { get; set; }
        public double fark { get { return aktarilanTutar - harcamaTutari; } }

        public IOIDetayBilgi(IOIProjeAktarilan item)
        {
            this.islemTarihi = new TNSDateTime(item.aktarmaTarihi);
            this.aciklama = item.aciklama;
            this.aktarilanTutar = item.aktarilanTutar;
            this.harcamaTutari = 0;
        }

        public IOIDetayBilgi(IOIProjeHarcama item)
        {
            this.islemTarihi = new TNSDateTime(item.harcamaTarihi);
            this.aciklama = item.aciklama;
            this.harcamaTutari = item.harcamaTutari;
            this.aktarilanTutar = 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is IOIDetayBilgi)
            {
                IOIDetayBilgi gelen = (IOIDetayBilgi)obj;
                int cmp = this.islemTarihi.Oku().CompareTo(gelen.islemTarihi.Oku());
                if (cmp == 0)
                {
                    cmp = gelen.fark.CompareTo(this.fark);
                }
                return cmp;
            }
            else
                return -1;
        }
    }

    /// <summary>
    /// IOIProje: 
    /// </summary>
    [System.Serializable()]
    public class IOIProjeBag
    {
        public int yil { get; set; }
        public string pebKod { get; set; }
        public string pebAd { get; set; }
        public string pebAd2 { get; set; }
        public int projeNo { get; set; }
        public string DPTProjeNo { get; set; }
        public string projeAd { get; set; }
        public int altProjeNo { get; set; }
        public string altProjeAd { get; set; }
    }

    /// <summary>
    /// IOIProje: 
    /// </summary>
    [System.Serializable()]
    public class IOIProje
    {
        public TNSCollection pebKodlar { get; set; } = null;

        public TNSCollection detaylar { get; set; }

        /// <summary>
        /// siraNo: Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// muhasebeKod: Saymanlık Kodu
        /// </summary>
        public int muhasebeKod { get; set; }

        /// <summary>
        /// birimKod: Birim Kodu
        /// </summary>
        public string birimKod { get; set; }

        public string muhasebeBirimAd { get; set; }

        ///// <summary>
        ///// Proje Adı: Proje No
        ///// </summary>
        //public int altProjeNo { get { return 0; } }

        ///// <summary>
        ///// Proje Adı: Proje No
        ///// </summary>
        //public int projeNo { get { return 0; } }

        public string DPTProjeNo { get { return ProjeBilgiBul(1); } }

        public string projeAdi { get { return ProjeBilgiBul(2); } }
        public string altProjeAdi { get { return ProjeBilgiBul(3); } }

        private string ProjeBilgiBul(int alanKod)
        {
            if (pebKodlar == null || pebKodlar.Count == 0) return "";
            string cevap = "";
            foreach (IOIProjeBag item in pebKodlar)
            {
                string deger;
                if (alanKod == 1) deger = item.DPTProjeNo;
                else if (alanKod == 2) deger = item.projeAd;
                else deger = item.altProjeAd;
                if (!cevap.Contains(deger))
                {
                    if (cevap != "") cevap += ", ";
                    cevap += deger;
                }
            }
            return cevap;
        }

        /// <summary>
        /// Proje Adı: İOİ Proje Adı
        /// </summary>
        public string ioiProjeAdi { get; set; }

        /// <summary>
        /// baslaYil: Başlangıç Yılı
        /// </summary>
        public int baslaYil { get; set; }

        /// <summary>
        /// bitisYil: Bitiş Yılı
        /// </summary>
        public int bitisYil { get; set; }

        /// <summary>
        /// toplam Maliyet: Bitiş Yılı
        /// </summary>
        public double projeTutari { get; set; } = 0;

        /// <summary>
        /// IOIButce: IOI ekranında girilen, aktarılan tutar toplamı veya yıl verilmişse onaylı MPRevize ile işlem yapılan toplam
        /// </summary>
        public double aktarilanTutar { get; set; }

        /// <summary>
        /// MPRevize ile işlem yapılan tüm yıllardaki toplam  MPRevize ile yapılan işlem veya yıl verilmişse onaylı olmayan MPRevize ile yapılan işlem
        /// </summary>
        public double islemleEklenen { get; set; }

        /// <summary>
        /// IOI ekranında girilen, harcanan tutar toplamı veya yıl verilmişse onaylı gönderilen - tenkis
        /// </summary>
        public double iOIHarc { get; set; }
        /// <summary>
        /// bütün yıllar için onaylı gönderilen - tenkis veya yıl verilmişse onaysız gönderilen - tenkis
        /// </summary>
        public double gonderilen { get; set; } = 0;

        public double toplamButce { get { return aktarilanTutar + islemleEklenen; } }
        public double toplamHarc { get { return iOIHarc + gonderilen; } }

        public int ilKod { get; set; }
        public string ilAdi { get; set; }

        /// <summary>
        /// Açıklama: Açıklama
        /// </summary>
        public string aciklama { get; set; }
        public int isTur { get; set; }

        ///// <summary>
        ///// IOIProjeButce: Bütçe ve Harcama
        ///// </summary>
        //public TNSCollection iOIProjeButce = new TNSCollection();

        //public void YilDoldur(int sonYil)
        //{
        //    for (int yil = sonYil - 5; yil <= sonYil; yil++)
        //    {
        //        if (!ButceYilVarMi(yil))
        //            iOIProjeButce.Add(new IOIProjeButce(yil));
        //    }
        //}

        //private bool ButceYilVarMi(int yil)
        //{
        //    foreach (IOIProjeButce item in iOIProjeButce)
        //    {
        //        if (item.yil == yil) return true;
        //    }
        //    return false;
        //}

        public string RakamKontrol()
        {
            int yil = DateTime.Now.Year;
            string hata = "";
            if (baslaYil < 2000) hata += "Başlangıç yılını giriniz!<br />";
            if (baslaYil > yil) hata += "Başlangıç yılı bu yıldan büyük olamaz!<br />";
            if (baslaYil > bitisYil) hata += "Başlangıç yılı bitişden büyük olamaz!<br />";

            //double toplamButce = 0;
            //double toplamHarcama = 0;
            //foreach (IOIProjeButce item in iOIProjeButce)
            //{
            //    toplamButce += (item.yIKOBButce + item.iOIButce + item.bakButce);
            //    toplamHarcama += (item.yIKOBHarc + item.iOIHarc + item.bakHarc);
            //}

            //if (toplamButce >= projeTutari)
            //{
            //    if (toplamButce > projeTutari)
            //        hata += String.Format("Yıllara girilen bütçe toplamı ({0}), proje toplam maliyetinden ({1}) büyük olamaz!<br />", toplamButce, projeTutari);
            //    foreach (IOIProjeButce item in iOIProjeButce)
            //    {
            //        hata += item.Kontrol(baslaYil, bitisYil, true);
            //    }
            //}
            return hata;
        }

        public string Kontrol()
        {
            int yil = DateTime.Now.Year;
            string hata = "";
            if (muhasebeKod <= 0) hata += "Saymanlık kodunu giriniz!<br />";
            if (string.IsNullOrWhiteSpace(birimKod)) hata += "Birim kodunu giriniz!<br />";
            if (string.IsNullOrWhiteSpace(ioiProjeAdi)) hata += "Proje adını giriniz!<br />";
            if (ilKod <= 0) hata += "Lütfen il seçiniz!<br />";
            //if (pebKodlar == null || pebKodlar.Count == 0) hata += "Lütfen alt faaliyetlerle ilişkilendiriniz!<br />";
            if (baslaYil < 2000) hata += "Başlangıç yılını giriniz!<br />";
            if (baslaYil > yil) hata += "Başlangıç yılı bu yıldan büyük olamaz!<br />";
            if (baslaYil > bitisYil) hata += "Başlangıç yılı bitişden büyük olamaz!<br />";
            if (projeTutari < 1000) hata += "Toplam maliyet 1000 TL'den küçük olamaz!<br />";
            if (ilKod < 1) hata += "Lütfen il seçiniz!<br />";
            if (isTur < 1 || isTur > 2) hata += "Lütfen iş türü seçiniz!<br />";
            if (string.IsNullOrWhiteSpace(aciklama)) hata += "Açıklama boş olamaz!<br />";

            //double toplamButce = 0;
            //double toplamHarcama = 0;
            //foreach (IOIProjeButce item in iOIProjeButce)
            //{
            //    hata += item.Kontrol(baslaYil, bitisYil, false);
            //    toplamButce += (item.yIKOBButce + item.iOIButce + item.bakButce);
            //    toplamHarcama += (item.yIKOBHarc + item.iOIHarc + item.bakHarc);
            //}

            //if (toplamButce > projeTutari) hata += "Yıllara girilen bütçe toplamı, proje toplam maliyetinden büyük olamaz!<br />";
            //if (toplamButce < 1000) hata += "Yıllara girilen toplam bütçe 1000 TL'den küçük olamaz!<br />";
            //if (toplamHarcama < 1000 && baslaYil < DateTime.Now.Year) hata += "Yıllara girilen toplam harcama 1000 TL'den küçük olamaz!<br />";

            return hata;
        }

        public void DetayEkle(ObjectArray islemler, bool aktarilan)
        {
            foreach (object item in islemler.objeler)
            {
                IOIDetayBilgi db;
                if (aktarilan)
                    detaylar.Add(new IOIDetayBilgi((IOIProjeAktarilan)item));
                else
                    detaylar.Add(new IOIDetayBilgi((IOIProjeHarcama)item));
            }
        }
    }

    /// <summary>
    /// IOIProjeButce: Bütçe ve Harcama
    /// </summary>
    [System.Serializable()]
    public class YIKOBProjeButce
    {
        public YIKOBProjeButce(ABSKod ekoKodYapi)
        {
            this.ekoKodYapi = ekoKodYapi;
        }

        public YIKOBProjeButce()
        {
            this.ekoKodYapi = null;
        }

        /// <summary>
        /// Yıl: Bütçe Yılı
        /// </summary>
        public ABSKod ekoKodYapi { get; set; }

        public string ekoAd
        {
            get
            {
                if (ekoKodYapi != null)
                {
                    return ekoKodYapi.ad;
                }
                return "";
            }
        }

        public string ekoKod
        {
            get
            {
                if (ekoKodYapi != null)
                {
                    return ekoKodYapi.kod;
                }
                return "";
            }
            set
            {
                if (ekoKodYapi == null)
                {
                    ekoKodYapi = new ABSKod();
                }
                ekoKodYapi.kod = value;
            }
        }

        /// <summary>
        /// YIKOBButce: YIKOB Bütçesi
        /// </summary>
        public double yIKOBButce { get; set; }

        /// <summary>
        /// YIKOBHarc: YIKOB Harcaması
        /// </summary>
        public double yIKOBHarc { get; set; }

        /// <summary>
        /// YIKOBHarc: YIKOB Harcaması
        /// </summary>
        public double yIKOBDevir { get { return yIKOBButce - yIKOBHarc; } }

        /// <summary>
        /// BakButce: Bakanlık Bütçe Katkısı
        /// </summary>
        public double bakButce { get; set; }

        /// <summary>
        /// BakHarc: Bakanlık Bütçe Katkısından Harcanan
        /// </summary>
        public double bakHarc { get; set; }

        /// <summary>
        /// BakHarc: Bakanlık Bütçe Katkısından Harcanan
        /// </summary>
        public double bakDevir { get { return bakButce - bakHarc; } }

        public string Kontrol(bool sadeceRakamKontrol)
        {
            string hata = "";
            if (yIKOBButce < 0) hata += String.Format("{0} kodu için YIKOB bütçesi negatif olamaz!<br />", ekoKodYapi.kod);
            if (yIKOBHarc < 0) hata += String.Format("{0} kodu için YIKOB harcaması negatif olamaz!<br />", ekoKodYapi.kod);
            if (yIKOBDevir < 0) hata += String.Format("{0} kodu için YIKOB devreden rakamı negatif olamaz!<br />", ekoKodYapi.kod);
            if (bakButce < 0) hata += String.Format("{0} kodu için Bakanlık katkısı bütçesi negatif olamaz!<br />", ekoKodYapi.kod);
            if (bakHarc < 0) hata += String.Format("{0} kodu için Bakanlık katkısı harcaması negatif olamaz!<br />", ekoKodYapi.kod);
            if (bakDevir < 0) hata += String.Format("{0} kodu için Bakanlık devreden rakamı negatif olamaz!<br />", ekoKodYapi.kod);

            if (bakButce < bakHarc) hata += String.Format("{0} kodu için Bakanlık katkısı harcaması, ödeneğinden büyük olamaz!<br />", ekoKodYapi.kod);
            //double bakKalan = bakButce - bakHarc;
            //if (bakKalan < bakDevir) hata += String.Format("{0} kodu için Bakanlık katkısı devredeni, kalandan ({1}) büyük olamaz!<br />", ekoKodYapi.kod, bakKalan.ToString());

            if (yIKOBButce < yIKOBHarc) hata += String.Format("{0} kodu için YIKOB harcaması, ödeneğinden büyük olamaz!<br />", ekoKodYapi.kod);
            //double kalan = yIKOBButce - yIKOBHarc;
            //if (kalan < yIKOBDevir) hata += String.Format("{0} kodu için YIKOB devredeni, kalandan ({1}) büyük olamaz!<br />", ekoKodYapi.kod, kalan.ToString());

            //if (string.IsNullOrWhiteSpace(hata))
            //{
            //    if (sadeceRakamKontrol)
            //    {
            //        //hata += String.Format("{0} kodu için veri girişi yapılmadığı halde, proje maliyetine ulaştınız/geçtiniz. Rakamlarınızı ve/veya proje yıllarını değiştiriniz!<br />", yil);
            //    }
            //}
            return hata;
        }

        private bool BAKDolu()
        {
            return (bakButce > 0 || bakHarc > 0 || bakDevir > 0);
        }

        private bool YIKOBDolu()
        {
            return (yIKOBButce > 0 || yIKOBHarc > 0 || yIKOBDevir > 0);
        }

        public bool Dolu()
        {
            return BAKDolu() || YIKOBDolu();
        }
    }

    /// <summary>
    /// IOIProje: 
    /// </summary>
    [System.Serializable()]
    public class YIKOBProje
    {
        public int yil { get; set; }

        /// <summary>
        /// muhasebeKod: Saymanlık Kodu
        /// </summary>
        public int muhasebeKod { get; set; }

        /// <summary>
        /// birimKod: Birim Kodu
        /// </summary>
        public string birimKod { get; set; }

        public string muhasebeBirimAd { get; set; }
        /// <summary>
        /// siraNo: Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// Proje Adı: Proje Adı
        /// </summary>
        public string projeAdi { get; set; }

        /// <summary>
        /// baslaYil: Başlangıç Yılı
        /// </summary>
        public int baslaYil { get; set; }

        /// <summary>
        /// bitisYil: Bitiş Yılı
        /// </summary>
        public int bitisYil { get; set; }

        /// <summary>
        /// toplam Maliyet: Bitiş Yılı
        /// </summary>
        public double toplamMaliyet { get; set; }

        /// <summary>
        /// toplam Harcama: Detaya girilmiş harcamaların toplamı
        /// </summary>
        public double toplamHarcama
        {
            get
            {
                return yIKOBHarc + bakHarc;
            }
        }

        public double toplamDevreden
        {
            get
            {
                return yIKOBDevir + bakDevir;
            }
        }

        public double toplamButce
        {
            get
            {
                return yIKOBButce + bakButce;
            }
        }
        /// <summary>
        /// Uygulama Yeri: Uygulama Yeri
        /// </summary>
        public string uygulamaYeri { get; set; }

        /// <summary>
        /// Projenin Amacı: Projenin Amacı
        /// </summary>
        public string projeninAmaci { get; set; }

        /// <summary>
        /// Açıklama: Açıklama
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// IOIProjeButce: Bütçe ve Harcama
        /// </summary>
        public TNSCollection projeButce = new TNSCollection();

        public string RakamKontrol()
        {
            int yil = DateTime.Now.Year;
            string hata = "";
            if (baslaYil < 2000) hata += "Başlangıç yılını giriniz!<br />";
            if (baslaYil > yil) hata += "Başlangıç yılı bu yıldan büyük olamaz!<br />";
            if (baslaYil > bitisYil) hata += "Başlangıç yılı bitişden büyük olamaz!<br />";

            double toplamButce = 0;
            double toplamHarcama = 0;
            foreach (YIKOBProjeButce item in projeButce)
            {
                toplamButce += (item.yIKOBButce + item.bakButce);
                toplamHarcama += (item.yIKOBHarc + item.bakHarc);
                hata += item.Kontrol(true);
            }

            if (toplamButce > toplamMaliyet)
                hata += String.Format("Yıllara girilen bütçe toplamı ({0}), proje toplam maliyetinden ({1}) büyük olamaz!<br />", toplamButce, toplamMaliyet);

            return hata;
        }

        public string Kontrol()
        {
            int yil = DateTime.Now.Year;
            string hata = "";
            if (yil <= 0) hata += "Yılı giriniz!<br />";
            if (muhasebeKod <= 0) hata += "Saymanlık kodunu giriniz!<br />";
            if (string.IsNullOrWhiteSpace(birimKod)) hata += "Birim kodunu giriniz!<br />";
            if (string.IsNullOrWhiteSpace(projeAdi)) hata += "Proje adını giriniz!<br />";
            if (baslaYil < 2000) hata += "Başlangıç yılını giriniz!<br />";
            if (baslaYil > yil) hata += "Başlangıç yılı bu yıldan büyük olamaz!<br />";
            if (baslaYil > bitisYil) hata += "Başlangıç yılı bitişden büyük olamaz!<br />";
            if (toplamMaliyet < 0.01) hata += "Toplam maliyet 1 Kr'dan küçük olamaz!<br />";
            if (string.IsNullOrWhiteSpace(uygulamaYeri)) hata += "Uygulama yeri boş olamaz!<br />";
            if (string.IsNullOrWhiteSpace(projeninAmaci)) hata += "Projenin amacı boş olamaz!<br />";
            if (string.IsNullOrWhiteSpace(aciklama)) hata += "Açıklama boş olamaz!<br />";

            if (!((this.yatirimYil == 0 && string.IsNullOrWhiteSpace(this.yatirimProjeNo) && string.IsNullOrWhiteSpace(this.yatirimProjeAd))
               || (this.yatirimYil > 0 && !string.IsNullOrWhiteSpace(this.yatirimProjeNo) && !string.IsNullOrWhiteSpace(this.yatirimProjeAd))))
                hata += "Yatırım projesi ile ilgili alanlardan birine değer girilirse, hepsine değer girilmelidir. Lütfen ya tüm yatırım proje alanlarına değer giriniz yada tüm yatırım proje alanlarını boşaltınız!<br />";

            double toplamButce = 0;
            double toplamHarcama = 0;
            foreach (YIKOBProjeButce item in projeButce)
            {
                hata += item.Kontrol(false);
                toplamButce += (item.yIKOBButce + item.bakButce);
                toplamHarcama += (item.yIKOBHarc + item.bakHarc);
            }

            if (toplamButce > toplamMaliyet) hata += "Girilen bütçe toplamı, proje toplam maliyetinden büyük olamaz!<br />";
            if (toplamButce < 0.01) hata += "Girilen toplam bütçe 1 Kr'dan küçük olamaz!<br />";
            //  if (toplamHarcama < 1000) hata += "Girilen toplam harcama 1000 TL'den küçük olamaz!<br />";

            return hata;
        }

        /// <summary>
        /// YIKOBButce: YIKOB Bütçesi
        /// </summary>
        public double yIKOBButce { get; set; }

        /// <summary>
        /// BakButce: Bakanlık Bütçe Katkısı
        /// </summary>
        public double bakButce { get; set; }

        /// <summary>
        /// YIKOBHarc: YIKOB Harcaması
        /// </summary>
        public double yIKOBHarc { get; set; }

        /// <summary>
        /// BakHarc: Bakanlık Bütçe Katkısından Harcanan
        /// </summary>
        public double bakHarc { get; set; }

        public int yatirimYil { get; set; }

        public string yatirimProjeAd { get; set; }

        public string yatirimProjeNo { get; set; }

        public double yIKOBDevir { get { return yIKOBButce - yIKOBHarc; } }

        public double bakDevir { get { return bakButce - bakHarc; } }

        public YIKOBProjeButce ProjeButceBul(string ekoKod)
        {
            ekoKod = ekoKod.Replace(".", "");
            foreach (YIKOBProjeButce k in projeButce)
            {
                if (k.ekoKodYapi.kod.Replace(".", "") == ekoKod) return k;
            }
            return null;
        }

        public void EkoDoldur(ABSKodlarVAN oa)
        {
            foreach (ABSKod k in oa.absKodlar)
            {
                projeButce.Add(new YIKOBProjeButce(k));
            }
        }

        public void EkoDoldur(int yil)
        {
            EkoDoldur(EkonomikListele(yil));
        }

        public static ABSKodlarVAN EkonomikListele(int yil)
        {
            return TNS.BKD.Arac.Tanimla().KodListeleSeviye(yil, "4", "06", 2, true);
        }
    }

    /// <summary>
    /// ButceCalismasiPersoneli: Personel Listesi
    /// </summary>
    [System.Serializable()]
    public class ButceCalismasiPersoneli
    {
        /// <summary>
        /// Yıl: Kod
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Pebkod: Faaliyet Kodu
        /// </summary>
        public string pebkod { get; set; }
        /// <summary>
        /// Sıra No: Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// Ad: Adı
        /// </summary>
        public string ad { get; set; }

        /// <summary>
        /// Soyad: Soyadı
        /// </summary>
        public string soyad { get; set; }

        /// <summary>
        /// Unvan: Unvanı
        /// </summary>
        public string unvan { get; set; }

        /// <summary>
        /// Birim: Birimi
        /// </summary>
        public string birim { get; set; }

        /// <summary>
        /// İş Tel: İş Telefonu
        /// </summary>
        public string isTel { get; set; }

        /// <summary>
        /// Cep Tel: Cep Telefonu
        /// </summary>
        public string cepTel { get; set; }

        /// <summary>
        /// Faks: Faks Numarası
        /// </summary>
        public string faks { get; set; }

        /// <summary>
        /// EPosta: Elektronik Posta Adresi
        /// </summary>
        public string ePosta { get; set; }

        public string pebad { get; set; }
    }
    ///// <summary>
    ///// FaaliyetinYapildigiYerDetay: FaaliyetinYapildigiYerBirim
    ///// </summary>
    //[System.Serializable()]
    //public class FaaliyetinYapildigiYerBirim
    //{
    //    private int _birimKod;
    //    /// <summary>
    //    /// birimKod: 
    //    /// </summary>
    //    public int birimKod { get { return _birimKod; } set { _birimKod = value; } }
    //}

    ///// <summary>
    ///// FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer
    ///// </summary>
    //[System.Serializable()]
    //public class FaaliyetinYapildigiYer
    //{
    //    private int _yil;
    //    /// <summary>
    //    /// Yıl: Lütfen Yıl'ı seçin.
    //    /// </summary>
    //    public int yil { get { return _yil; } set { _yil = value; } }

    //    private string _faaliyetKod;
    //    /// <summary>
    //    /// Faaliyet Kod: Lütfen Faaliyet Kodu'nu seçin.
    //    /// </summary>
    //    public string faaliyetKod { get { return _faaliyetKod; } set { _faaliyetKod = value; } }

    //    private int _merkezDahil;
    //    /// <summary>
    //    /// Merkez Dahil: Lütfen Talep No yazınız.
    //    /// </summary>
    //    public int merkezDahil { get { return _merkezDahil; } set { _merkezDahil = value; } }

    //    private int _tumilveilceler;
    //    /// <summary>
    //    /// Tüm il ve ilçeler: Merkez Dahil mi seçiniz.
    //    /// </summary>
    //    public int tumilveilceler { get { return _tumilveilceler; } set { _tumilveilceler = value; } }

    //    /// <summary>
    //    /// FaaliyetinYapildigiYerDetay: FaaliyetinYapildigiYerDetay
    //    /// </summary>
    //    public FaaliyetinYapildigiYerDetay[] faaliyetinYapildigiYerDetay;
    //    public FaaliyetinYapildigiYerBirim[] faaliyetinYapildigiYerBirim;
    //}

    /// <summary>
    /// Bu sunucu sınıfı tarafından sunulan fonksiyonları belirten arayüz sınıf
    /// </summary>
    public interface IBHMv2Servis
    {
        /// <summary>
        /// Verilen kriterlere uygun olan ve hiyerarşideki bir alt kırılımı ve bütün alt kırılımları döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">Yordamı çağıran kullanıcı</param>
        /// <param name="kurumsalYapi">Kurumsal yapı kriteri. Bu yapı içindeki değerlere göre kriter oluşturulur.</param>
        /// <param name="birAltSeviye">True ise Kurumsal yapı kriterinde verilen kodun bir alt seviyesindeki kodlar listelenir. False ise o kod ile başlayan tüm kodlar listelenir.</param>
        /// <returns>Verilen kriterlere uyan kurumsal kodlar dizisi döndürülür.</returns>
        KurumsalYapi[] KurumsalYapiListele(Kullanici kullanan, KurumsalYapi kurumsalYapi, bool birAltSeviye, bool enAltSeviye);

        /// <summary>
        /// Verilen saymanlık, birim ve kurumsal kod kriterlerine uygun olan en alt kırılımdaki harcama birimlerini döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="saymanlik">Harcama birimi verilen saymanlık olan harcama birimleri listelenir.</param>
        /// <param name="birim">Harcama birimi verilen birim kodu olan harcama birimleri listelenir.</param>
        /// <param name="kurKod">Harcama birimi verilen kurumsal kod harcama birimleri listelenir.</param>
        /// <returns>Verilen kriterlere uyan kurumsal kodlar dizisi döndürülür.</returns>
        string[] KurumsalYapiListele(string saymanlik, string birim, string kurKod);

        /// <summary>
        /// Verilen kurumsal yapı koduna ait bilgiler döndürülür.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="aktifKultur">İstemci tarafının aktif kültürü. Oluşan mesajlar bu dile göre döndürülecektir.</param>
        /// <param name="kod">Kurumsal yapı bilgisi döndürülecek kod</param>
        /// <returns>Verilen koda ait kurumsal yapı bilgisi</returns>
        KurumsalYapi KurumsalYapiDondur(string aktifKultur, string kod);

        Sonuc KurumsalYapiKaydet(Kullanici kullanan, KurumsalYapi ky);

        Sonuc KurumsalYapiSil(Kullanici kullanan, KurumsalYapi ky);

        /// <summary>
        /// Kullanıcının yetkisi kontrol edildikten sonra, 
        /// var olan senaryo tanım bilgisi değiştirilir veya senaryo kodu boş ise yeni senaryo tanımı oluşturulur.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="bs">Bütçe senaryo tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceSenaryoKaydet(Kullanici kullanan, ButceSenaryo bs);

        /// <summary>
        /// Kullanıcının yetkisi kontrol edildikten sonra var olan senaryo tanım bilgisi silinir.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceSenaryo">Bütçe senaryo tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceSenaryoSil(Kullanici kullanan, ButceSenaryo butceSenaryo);

        /// <summary>
        /// Verilen senaryo tanım kıstasına uyan senaryo tanımlarını döndüren yordam.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="bs">Kıstas olarak kullanılacak bütçe senaryo tanım bilgisi</param>
        /// <returns>Verilen bütçe senaryo tanım kıstasına uyan senaryolar döndürülür.</returns>
        ButceSenaryo[] ButceSenaryoListele(Kullanici kullanan, ButceSenaryo bs);

        /// <summary>
        /// Senaryo adını verilen senaryo koduna göre döndüren yordam
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="aktifKultur">İstemci tarafının aktif kültürü. Oluşan mesajlar bu dile göre döndürülecektir.</param>
        /// <param name="senaryoKod">Adı döndürülecek senaryo kodu</param>
        /// <returns>Verilen senaryo koduna göre senaryo adı döndürülür.</returns>
        string ButceSenaryoAdBul(string aktifKultur, string senaryoKod);

        /// <summary>
        /// Bütçe tasnif tanım bilgisini verilen sıra numarasına göre döndüren yordam
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Tasnif tanım bilgisi döndürülecek tasnif sıra numarası</param>
        /// <returns>Verilen sıra numarasına ait tasnif tanımı döndürülür.</returns>
        ButceTasnif ButceTasnifOku(Kullanici kullanan, int tasnifSiraNo);


        Sonuc ButceTasnifOnayla(Kullanici kullanan, int tasnifSiraNo, int yil, string kurKod, string asamaKod, int kaldir);

        int ButceTasnifOnayBul(int tasnifSiraNo, int yil, string kurKod, string asamaKod);

        /// <summary>
        /// Kullanıcının yetkisi kontrol edildikten sonra, 
        /// var olan tasnif tanım bilgisi değiştirilir veya tasnif kodu boş ise yeni tasnif tanımı oluşturulur.
        /// Hata olursa Exception oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceTasnif">Bütçe tasnif tanım bilgisi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceTasnifKaydet(Kullanici kullanan, ButceTasnif butceTasnif);

        /// <summary>
        /// Bütçe tasnifi kimlik bilgilerini döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kurKod">Hangi birime ait tasniflerin listeleneceği bilgisi.</param>
        /// <param name="tur">Hangi türdeki tasnifler listelenecek.</param>
        /// <returns>Sistemde kayıtlı tüm tasnif kimlik bilgileri dizi içerisinde döndürülür.</returns>
        ButceTasnif[] ButceTasnifListele(Kullanici kullanan, string kurKod, int tur);

        /// <summary>
        /// Bir bütçe tasnifine ait, tanımlı tüm satırları kaydeden yordam
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Kaydedilecek hiyerarşinin ait olduğu tasnifin sıra numarası</param>
        /// <param name="hiyerarsi">Verilen tasnif numarasına ait satırları içeren dizi</param>
        /// <param name="yil">Her bir satırdaki tertip şablonunun yıldan yıla değişebileceği tasarlanmıştır. Verilen satırlardaki tertip şablonlarının hangi yıla ait olduğu bilgisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceTasnifHiyerarsiKaydet(Kullanici kullanan, int tasnifSiraNo, ButceTasnifHiyerarsikSatir[] hiyerarsi, int yil);

        /// <summary>
        /// Sıra numarası verilen tasnife ait, hiyerarşik satır bilgilerini ve her bir satırın verilen yıla ait tertip şablonlarını döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasnifSiraNo">Hiyerarşi satırları döndürülecek tasnifin sıra numarası</param>
        /// <param name="yil">Her bir satırdaki tertip şablonunun yıldan yıla değişebileceği tasarlanmıştır. Döndürülecek satırlardaki tertip şablonlarının hangi yıla ait olduğu bilgisi.</param>
        /// <param name="hiyerarsiKod">Sadece verilen hiyerarsik kodun bilgileri okunur.</param>
        /// <returns>Sıra numarası verilen tasnife ait hiyerarşik satır bilgileri dizi olarak döndürülür.</returns>
        ButceTasnifHiyerarsikSatir[] ButceTasnifHiyerarsiListele(Kullanici kullanan, int tasnifSiraNo, int yil, string hiyerarsiKod);

        /// <summary>
        /// Verilen tasnif sıra numarası, yıl ve senaryo bilgisine göre, 
        /// kullanıcının girmiş olduğu tasnif değerlerini döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="form">Form parametresinin; tasnif sıra numarası, yıl ve senaryo alanları kullanılarak değerler bulunacaktır.</param>
        /// <param name="ekDegerParamlar">Okunan her bir satırla birlikte döndürülecek olan ek bilgilerin tanımlarını tutan dizi.</param>
        /// <param name="senaryoKod">Eğer dolu ise, tasnifle ilişkilendirilmiş senaryolar yerine verilen senaryo değerleri okunur.</param>
        /// <param name="hiyerarsiKod">Eğer tek bir satıra ait bilgi okunmak isteniyorsa doldurulmalıdır.</param>
        /// <param name="kistasTertip">Okunan her bir satırdaki tertip şablonlarının uyması gereken kistas tertip.</param>
        /// <returns>Verilen tasnif sıra numarası, yıl ve senaryo bilgisine göre girilen değerler ve istenilen ek bilgiler form yapısı içindeki dizide döndürülür.</returns>
        ButceTasnifOdenekleriFormu ButceTasnifOdenekOku(Kullanici kullanan, ButceTasnifOdenekleriFormu form, EkDegerParam[] ekDegerParamlar, string senaryoKod, string hiyerarsiKod, string kistasTertip);

        /// <summary>
        /// Form içerisinde verilen tasnif ödenek değerlerini, tasnif sıra numarası, yıl ve senaryo bilgisini kullanarak kaydeden yordam.
        /// Hata olursa Exception oluşturur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="form">Form parametresinin; tasnif sıra numarası, yıl ve senaryo alanları ve verilen ödenekler kullanılarak kayıt yapılır.</param>
        /// <param name="onceSilme">Bu parametre doğru ise silme işlemi yapılmaz, sadece ekleme işlemi yapılır.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceTasnifOdenekKaydet(Kullanici kullanan, ButceTasnifOdenekleriFormu form, bool onceSilme);

        /// <summary>
        /// Her senaryo bir tasnif ile ilişkilendirilmiş ve her tasnif satırına bir kaynak bilgisi tanımlanmıştır.
        /// Bu senaryoya girilen tasnif değerleri ile, bu senaryonun tertip ödenek değerlerini; her satır için girilen kaynak verisi ile oluşturan yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="hedefYil">Oluşturulacak detay tertip ödeneklerinin yılı</param>
        /// <param name="hedefSenaryoKod">Oluşturulacak detay tertip ödeneklerinin senaryo kodu. Senaryo kodu kullanılarak tasnif verilerine de ulaşılır.</param>
        /// <param name="tasnifNo">Oluşturma işleminde kullanılacak tertiplerin listesini bulmakta kullanılacak tasnifin numarası.</param>
        /// <param name="hiyerarsiKod">Oluşturma işleminde kullanılacak tertiplerin listesini bulmakta kullanılacak tasnifin satırını gösterem hiyerarşik satır kodu.</param>
        /// <param name="kaynakTur">Tertip listesi elde edildikten sonra, tertip için kullanılacak kaynak rakamın elde edileceği kaynak.</param>
        /// <param name="kaynakYil">Tertip listesi elde edildikten sonra, tertip için kullanılacak kaynak rakamın elde edileceği kaynağın yıl bilgisi.</param>
        /// <param name="kaynakSenaryoKodVeyaAy">Tertip listesi elde edildikten sonra, tertip için kullanılacak kaynak rakamın elde edileceği kaynağın senaryosu veya ayı.</param>
        /// <param name="sadeceKontrol">İşlemin yapılıp yapılmayacağı. True ise sadece kontrol yapılır işlem yapılmaz.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TasnifdenGiderOdenekOlustur(Kullanici kullanan, int hedefYil, string hedefSenaryoKod, int tasnifNo, string hiyerarsiKod, ENUMUretimKaynakTur kaynakTur, int kaynakYil, string kaynakSenaryoKodVeyaAy, bool sadeceKontrol);

        /// <summary>
        /// Kıstas olarak verilen AltFaaliyet nesnesindeki yıl ve kod alanı kullanılarak, alt faaliyet bilgileri okunur ve döndürülür.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Kıstas olarak verilen AltFaaliyet nesnesi (yıl ve kod alanları kullanılır)</param>
        /// <returns>Kıstas olarak verilen AltFaaliyete ait bilgiler okunur ve döndürülür.</returns>
        AltFaaliyet AltFaaliyetTanimOku(Kullanici kullanan, AltFaaliyet altFaaliyet);

        /// <summary>
        /// Alt faaliyet tanım bilgisini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Kaydedilecek bilgileri içeren AltFaaliyet tanım nesnesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AltFaaliyetTanimKaydet(Kullanici kullanan, AltFaaliyet altFaaliyet);

        /// <summary>
        /// Parametre olarak verilen dizideki tüm faaliyetleri silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyetler">Silinecek faaliyetleri içeren dizi (yıl ve kod alanları dolu olmalıdır.)</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc AltFaaliyetTanimSil(Kullanici kullanan, AltFaaliyet[] altFaaliyetler);

        /// <summary>
        /// Verilen alt faaliyet kıstasına (yıla ait kodla başlayan faaliyetler) uyan faaliyetlerin listesi döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Listelenecek faaliyetlerin uyacağı kıstas bilgisi</param>
        /// <returns>Verilen kıstasa uyan faaliyetlerin dizisi döndürülür.</returns>
        AltFaaliyet[] AltFaaliyetListele(Kullanici kullanan, AltFaaliyet altFaaliyet, string senaryoKod);

        /// <summary>
        /// Alt faaliyet kıstası içinde tanımlı olan yıl ve DPT alt proje bilgileri kullanılarak,
        /// bu DPT alt projesi ile ilişkilendirilmiş olan alt faaliyetlerin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="altFaaliyet">Yıl ve DPT alt proje kodu bilgileri kullanılan alt faaliyet yapısı</param>
        /// <returns>Verilen yıl ve DPT alt proje kodu kıstasına uyan faaliyetlerin listesi</returns>
        AltFaaliyet[] YatBagPebKodlariListele(Kullanici kullanan, AltFaaliyet altFaaliyet, string senaryoKod);

        /// <summary>
        /// Kıstas olarak verilen yıl ve DPT proje kodu bilgileri kullanılarak,
        /// bu DPT projesi ile ilişkilendirilmiş olan alt faaliyetlerin listesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="yil">Alt faaliyetlerin listeleneceği yıl bilgisi</param>
        /// <param name="projeKod">Alt faaliyetleri bulunacak DPT yatırım projesi kodu</param>
        /// <returns>Verilen yıl ve DPT proje kodu kıstasına uyan faaliyetlerin listesi</returns>
        AltFaaliyet[] YatBagPebKodlariListele(Kullanici kullanan, int yil, int projeKod, string senaryoKod);

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        Tertip[] TertipTanimlariListele(Kullanici kullanan, Tertip kistasTertip);

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        Tertip[] TertipTanimlariListeleFonkod(Kullanici kullanan, Tertip kistasTertip, bool fonkodUyumsuz);

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Tanımlı tertiplerden verilen tertip kıstasına uygun olanlarının listesini dizi olarak döndüren yordam.
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="tertipDurumlar">Listelecek tertiplerin uyacağı durumların listesi. Tertip bu durumlardan birinde ise döndürülür.</param>
        /// <returns>Verilen tertip kıstasına uyan tertip bilgileri dizi olarak döndürülür.</returns>
        Tertip[] TertipTanimlariListele(Kullanici kullanan, Tertip kistasTertip, ENUMTertipDurum[] tertipDurumlar);

        /// <summary>
        /// Bütçe hazırlık işlemleri sırasında kullanılan gelir ve gider tertipleri en alt düzeyde sistemde tanımlanmış olmalıdır.
        /// Verilen tertip dizisindeki, tertip bilgileri sisteme kaydedilir.
        /// Parametre olarak verilen dizide, tertip adı boş olan tertipler, tertip havuzundan silinir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertipler">GiderTertipListe yapısında GiderTertipListe: bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc TertipTanimlariKaydet(Kullanici kullanan, Tertip[] tertipler);

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde 3 + istenilen ek bilgi kadar değer oluşacaktır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verile kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        TertipOdenek[] ButceOdenekListele(Kullanici kullanan, string senaryoKod, Tertip kistasTertip, EkDegerParam[] ekDegerParamlar, int tahmin);
        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde 3 + istenilen ek bilgi kadar değer oluşacaktır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasYapi">Listelecek tertiplerin uyacağı kıstasları birden fazla olarak ve dahil hariç tutan yapı</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verile kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        TertipOdenek[] ButceOdenekListele(Kullanici kullanan, string senaryoKod, KistasYapi kistasYapi, EkDegerParam[] ekDegerParamlar, int tahmin, int aciklamaKolon, bool eko2);

        /// <summary>
        /// Kıstas olarak verilen tertipe uyan, tertipleri (gelir, gider ve finansman) 
        /// ve bu tertiple birlikte, tertip yılı ve senaryo kodu kullanılarak, yıl ve senaryoya ait çok yıllı ödeneklerle birlikte
        /// istenilen ek bilgileri TertipOdenek dizisi şeklinde döndüren yordam.
        /// TertipOdenek nesnesinde ödenek ise 3 + istenilen, tertip ise 1 + istenilen ek bilgi kadar değer oluşacaktır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Çok yıllı ödenek değerleri döndürülecek senaryo kodu</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="ekDegerParamlar">Kullanıcının verdiği senaryoya ait ödeneklerle birlikte döndürülecek ek değerler dizisi</param>
        /// <param name="tahmin">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <param name="kodBilgileriGelen">1 ise tahmin bilgileri, değil ise ödenek bilgileri listelenir.</param>
        /// <returns>Verile kıstas tertibe uygun tertipler, bu tertiplerin verilen senaryoya ait bilgileri ile ek değerleri TertipOdenek dizisinde döndürülür.</returns>
        TertipOdenek[] ButceOdenekListele(Kullanici kullanan, string senaryoKod, Tertip kistasTertip, EkDegerParam[] ekDegerParamlar, int tahmin, UcgenKodBilgileri kodBilgileriGelen);

        /// <summary>
        /// Verilen tertip listesindeki çok yıllı ödenekler veri tabanına kaydedilir. 
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Ödeneklerin kaydedileceği senaryo kodu</param>
        /// <param name="tertipListesi">Tertip ve çok yıllı ödenek değerlerinin bulunduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceOdenekKaydet(Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipListesi);
        Sonuc ButceOdenekKaydet(Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipOdenekListesi, bool aciklamali);

        /// <summary>
        /// Verilen tertip listesindeki yıl sonu ödenekleri veri tabanına kaydedilir. 
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Yıl sonu gerçekleşme tahmininin kaydedileceği senaryo kodu</param>
        /// <param name="tertipListesi">Tertip ve yıl sonu gerçekleşme tahmini değerlerinin bulunduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceYSTahminKaydet(Kullanici kullanan, string senaryoKod, TertipOdenek[] tertipListesi);

        /// <summary>
        /// Verilen tertip listesindeki yıl sonu ödenekleri veri tabanına kaydedilir. 
        /// Sıfır değeri olan ödenekler kaydedilmez. Ödenekler tertibin yılına ve verilen senaryoya kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="senaryoKod">Yıl sonu gerçekleşme tahmininin kaydedileceği senaryo kodu</param>
        /// <param name="kistas">Seçilecek tertiplerin kıstas değerlerini tutat değişken</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        ObjectArray ButceYSTahminListele(Kullanici kullanan, string senaryoKod, Tertip kistas);

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric"></param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric);

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Oluşan tertip bölümüne ait ek bilgiler hesaplanır ve döndürülür. kırılımlardan bölünürek sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Parametre true ise döndürülecek listede sıfır değeri olan satırlar bulunmaz.</param>
        /// <param name="tanimliTertipler">Kayıtlı tüm tertipler getirilir.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler);

        OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul);
        OdenekBilgi[] ListeTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul, int aciklamaKolon);
        OdenekBilgi[] ListeTertipOl(Kullanici kullanan, KistasYapi kistasYapi, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, bool tanimliTertipler, bool adlariBul, int aciklamaKolon);

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Daha sonra bu kısmın 1., 2. 3. ve n. kırılımları alınarak üçgen format elde edilir.
        /// Her bir üçgen satır için ek bilgiler hesapllanır ve sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Değişken değeri true ise ek bilgilerinin hepsi sıfır olan tertipler sonuç kümesine alınmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde üçgen olarak ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        UcgenSatir[] UcgenTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric);

        /// <summary>
        /// Verilen kıstas tertibe uyan tertipler bulunur. Bu tertipler verilen kod bilgilerine göre sıralanır 
        /// ve verilen kırılım sayısı kadar olan kısmı alınır. Daha sonra bu kısmın 1., 2. 3. ve n. kırılımları alınarak üçgen format elde edilir.
        /// Her bir üçgen satır için ek bilgiler hesapllanır ve sonuç dizisi oluşturulur.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="kistasTertip">Listelecek tertiplerin uyacağı kıstasları tutan tertip nesnesi</param>
        /// <param name="kodBilgileri">Tertip sonuç listesi üretilecekken, sonuç listesinde oluşacak boyutların kodları, sırası ve kırılımlarını tutan dizi.</param>
        /// <param name="ekBilgiler">Sonuc listesinde yer alan her bir tertiple birlikte, döndürülecek ödeneklerin hangileri olduğunu tutan değişken</param>
        /// <param name="sifirlarHaric">Değişken değeri true ise ek bilgilerinin hepsi sıfır olan tertipler sonuç kümesine alınmaz.</param>
        /// <returns>Verilen kıstasa uyan tertipler, verilen kod şeklinde üçgen olarak ve ek bilgilerle OdenekBilgi dizisi şeklinde döndürülür.</returns>
        UcgenSatir[] UcgenTertipOl(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri kodBilgileri, EkDegerParam[] ekBilgiler, bool sifirlarHaric, int aciklamaKolon);

        /// <summary>
        /// Bütçe fişleri girilirken her bir tertip için girilen gerekçe yazısını veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <param name="yazi">Bütçe tertibi için istenen ödeneğin gerekçesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceFisiGerekceKaydet(Kullanici kullanan, Tertip tertip, string[] yazi);

        /// <summary>
        /// Verilen yıl ve tertip için istenilen ödenek için girilmiş olan gerekçe döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <returns>Bütçe tertibi için istenen ödeneğin gerekçesi döndürülür</returns>
        string[] ButceFisiGerekceYaziGetir(Kullanici kullanan, Tertip tertip);

        /// <summary>
        /// Verilen yıl ve tertip şablonu için, girilmiş bütçe fişi sayısını döndüren yordam.
        /// </summary>
        /// <param name="tertip">Bütçe fişleri sayısı bulunacak tertip bilgisi.</param>
        /// <returns>Bütçe tertibi için kayıtlı bütçe fişi sayısı döndürülür.</returns>
        int ButceFisiSayisiBul(Tertip tertip);

        /// <summary>
        /// Tertip yapısında verilen şablona uyan tertiplerin, bütçe fişleri bir önceki yıldan alınır ve kaydedilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişleri kopyalanacak tertip bilgisi.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceFisiKopyalaOncYil(Kullanici kullanan, Tertip tertip);

        /// <summary>
        /// Bütçe tertibi için istenen ödeneğin gerekçesi girilirken, satın alınması planlanan malzemeler
        /// kullanıcı tarafından girilir. Bu malzemeleri veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Bütçe fişinin tertibi</param>
        /// <param name="tertipMalzemeleri">Bu tertip ile alınaca malzemelerin bilgilerinin tutulduğu dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceFisiMalzemeKaydet(Kullanici kullanan, Tertip tertip, ButceFisiMalzeme[] tertipMalzemeleri);

        /// <summary>
        /// Parametre olarak verilen tertip için istenen ödenek gerekçelendirilirken, girilen malzeme bilgilerini veri tabanından okuyup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Kendisi ile ilişkilendirilen malzemelerin döndürüleceği tertip</param>
        /// <returns>Parametre olarak verilen tertip için istenen ödenek gerekçelendirilirken girilen malzeme bilgileri döndürülür.</returns>
        ButceFisiMalzeme[] ButceFisiMalzemeListele(Kullanici kullanan, Tertip tertip);

        /// <summary>
        /// Bütçe fişleri girilirken her bir gelir tertibi için, gelir tahmini için girilen gerekçe yazısını veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Gelir fişinin tertibi</param>
        /// <param name="yazi">Gelir tertibi için istenen ödeneğin gerekçesi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceGelirFisiGerekceKaydet(Kullanici kullanan, Tertip tertip, string yazi);

        /// <summary>
        /// Verilen yıl ve gelir tertibi için tahmin edilen gelir için girilmiş olan gerekçe döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Gelir fişinin tertibi</param>
        /// <returns>Gelir tertibi için tahmin edilen gelirin gerekçesi döndürülür</returns>
        string ButceGelirFisiGerekceYaziGetir(Kullanici kullanan, Tertip tertip);

        /// <summary>
        /// Kurumsal, fonksiyonel, finans kodu ve ekonomik (4 düzey) kodu verilen dizgi olarak verilen tertibin
        /// alt faaliyet kodu bazında verilen tertibin harcamalarının verilen yıl ve ay için dağıtım yüzdesini döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Dağılım yüzdesi girilecek yıl bilgisi</param>
        /// <param name="ay">Dağılım yüzdesi girilecek ay bilgisi</param>
        /// <param name="tertip">Kurumsal, fonksiyonel, finans kodu ve ekonomik (4 düzey) koddan oluşan tertip</param>
        /// <returns>Verilen tertibe ait alt faaliyet kodları ve alt faaliyetin oranı dizide döndürülür.</returns>
        OdenekBilgi[] HarcamaDagitimYuzdeListele(Kullanici kullanan, int butceYili, int yil, int ay, Tertip tertip);

        /// <summary>
        /// Muhasebattan gelen harcamaları, alt faaliyet kodlarının eklenebilmesi için,
        /// muhasebat harcamaları tertiplerine alt faaliyet eklenebilmesi için, verilen tertibin alt faaliyet oranlarını
        /// veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Dağılım yüzdesi kaydedilecek yıl bilgisi</param>
        /// <param name="ay">Dağılım yüzdesi kaydedilecek ay bilgisi</param>
        /// <param name="oranlar">kur, fon, fin ve eko kısmı aynı, pebleri farklı olan ve oranları tutan dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HarcamaDagitimKaydetTekTertip(Kullanici kullanan, int butceYili, int yil, int ay, OdenekBilgi[] oranlar);

        /// <summary>
        /// Muhasebattan gelen harcamaları, alt faaliyet kodlarının eklenebilmesi için,
        /// muhasebat harcamaları tertiplerine alt faaliyet eklenebilmesi için, tüm tertiplerin alt faaliyet oranlarını
        /// veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Dağılım yüzdesi kaydedilecek yıl bilgisi</param>
        /// <param name="ay">Dağılım yüzdesi kaydedilecek ay bilgisi</param>
        /// <param name="oranlar">tüm tertipler için, pebleri farklı olan ve oranları tutan dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HarcamaDagitimKaydetTumTertipler(Kullanici kullanan, int butceYili, int yil, int ay, OdenekBilgi[] oranlar);

        /// <summary>
        /// Muhasebat harcamalarından gelen tertiplerden, alt faaliyetler ilişkisi hiç kurulmamış 
        /// olanların listesi dizi olarak döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Tertip alt faaliyet eşleşmesi olmayan tertiplerin bulunacağı yıl</param>
        /// <param name="ay">Tertip alt faaliyet eşleşmesi olmayan tertiplerin bulunacağı ay</param>
        /// <returns>Tertip alt faaliyet eşleşmesi olmayan tertiplerin listesi</returns>
        OdenekBilgi[] HarcamaDagitimPebListeleYuzdesiOlmayanlar(Kullanici kullanan, int butceYili, int yil, int ay);

        /// <summary>
        /// Tertip, alt faaliyet yüzdeleri kullanılarak, verilen yıl ve aydaki harcama gerçekleşmeleri 
        /// alt faaliyet bazında hesaplanır ve veri tabanına yazılır.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="butceYili">Her bütçe yılı için ayrı veri tabanları tutulduğundan, her bütçe yılı için ayrı parametreler kullanılabilmektedir.</param>
        /// <param name="yil">Harcama gerçekleşmeleri girilen oranlarla alt faaliyetlere dağıtılacağı yıl</param>
        /// <param name="ay">Harcama gerçekleşmeleri girilen oranlarla alt faaliyetlere dağıtılacağı ay</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc HarcamalaraAltFaalEkle(Kullanici kullanan, int butceYili, int yil, int ay);

        #region "Taşra ödenekleri ve parametreleri girişi"

        /// <summary>
        /// Verilen kıstas tertibe uyan ve harcama birimi bazında girilen ödenekleri listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tertip">Listelenecek ödenekler için verilen kıstas.
        /// Tertip tasra ödenek türünde olmalı ve son kırılımı birimi göstermelidir.</param>
        /// <param name="asama">Listelenecek verilerin aşaması. Harcama birimi bazında ödenekler çeşitli aşamalarda girilebilemekte ve listelenmektedir.</param>
        /// <param name="ekDegerParamlar">Tertipler listelenirken, her bir tertiple birlikte listelenecek ek bilgiler dizisi</param>
        /// <returns>Verilen kıstas tertiple uyumlu tasra ödenekleri, istenilen ek bilgilerle birlikte döndürülür.</returns>
        TertipOdenek[] BirimOdenekGiderListele(Kullanici kullanan, Tertip tertip, ENUMBHBirimAsama asama, EkDegerParam[] ekDegerParamlar);

        /// <summary>
        /// Verilen ödenekler dizisini, verilen asama için veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="asama">Listelenecek verilerin aşaması. Harcama birimi bazında ödenekler çeşitli aşamalarda girilebilemekte ve listelenmektedir.</param>
        /// <param name="tertipOdenekler">Veri tabanına kaydedilecek ödenek bilgilerini tutan dizi.
        /// Dizideki tertipler tasra ödenek türünde olmalı ve son kırılımı birimi göstermelidir.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc BirimOdenekKaydet(Kullanici kullanan, ENUMBHBirimAsama asama, TertipOdenek[] tertipOdenekler);

        /// <summary>
        /// Harcama birimi ödenek talebi yaparken, ekonomik kod bazında gerekçe girilirken,
        /// ekonomik kod taşınır ile ilişkili ise, taşınır bilgileri girilir.
        /// Girilen taşınır bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasinirTalep">Kaydedilecek taşınır bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTalepTasinirKaydet(Kullanici kullanan, ParTalepTasinir tasinirTalep);

        /// <summary>
        /// Bir harcama biriminin talep ettiği taşınırları verilen kriterlere göre bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasinirTalep">Listelenecek taşınır bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen taşınır kıstasına uyan, taşınır bilgilerinin listesi ParTalepTasinir nesneleri içeren dizide döndürülür.</returns>
        ObjectArray ParTalepTasinirListele(Kullanici kullanan, ParTalepTasinir tasinirTalep);

        /// <summary>
        /// Bir harcama biriminin talep ettiği taşınırlardan, verilen kriterlere uyanları, silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tasinirTalep">Silinecek taşınır taleplerinden silinecekleri gösteren kıstas nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTalepTasinirSil(Kullanici kullanan, ParTalepTasinir tasinirTalep);

        /// <summary>
        /// Taşınır talepleri girilirken, kullanılacak ortalama fiyat bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Kaydedilecek ortalama fiyat bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTasinirOrtalamaFiyatKaydet(Kullanici kullanan, ParTasinirOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Taşınır talepleri girilirken, kullanılacak ortalama fiyat bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Listelenecek ortalama fiyat bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen ortalama fiyat kıstasına uyan ortalama fiyat bilgilerinin listesi ParTasinirOrtalamaFiyat nesneleri içeren dizide döndürülür.</returns>
        ObjectArray ParTasinirOrtalamaFiyatListele(Kullanici kullanan, ParTasinirOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Kaydedilen taşınır ortalama fiyatlarından verilen kritere uyanları silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Silinecek ortalama fiyat bilgileri için kriter bilgilerini tutan nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTasinirOrtalamaFiyatSil(Kullanici kullanan, ParTasinirOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Harcama birimi ödenek talebi yaparken, ekonomik kod bazında gerekçe girilirken,
        /// ekonomik kod tüketim ile ilişkili ise, tüketim bilgileri girilir.
        /// Girilen tüketim bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tuketim">Kaydedilecek tüketim bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTuketimKaydet(Kullanici kullanan, ParTuketim tuketim);

        /// <summary>
        /// Bir harcama biriminin talep ettiği tüketim bilgilerini verilen kriterlere göre bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tuketim">Listelenecek tüketim bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen tüketim kıstasına uyan, tüketim bilgilerinin listesi ParTuketim nesneleri içeren dizide döndürülür.</returns>
        ObjectArray ParTuketimListele(Kullanici kullanan, ParTuketim tuketim);

        /// <summary>
        /// Bir harcama biriminin girdiği tüketim bilgilerinden, verilen kriterlere uyanları, silen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="tuketim">Silinecek tüketim bilgilerinden, silinecekleri gösteren kıstas nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTuketimSil(Kullanici kullanan, ParTuketim tuketim);

        /// <summary>
        /// Tüketim bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Kaydedilecek ortalama fiyat bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTuketimOrtalamaFiyatKaydet(Kullanici kullanan, ParTuketimOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Tüketim bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Listelenecek ortalama fiyat bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen ortalama fiyat kıstasına uyan ortalama fiyat bilgilerinin listesi ParTuketimOrtalamaFiyat nesneleri içeren dizide döndürülür.</returns>
        ObjectArray ParTuketimOrtalamaFiyatListele(Kullanici kullanan, ParTuketimOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Kaydedilen tüketim bilgileri ortalama fiyatlarından verilen kritere uyanları silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Silinecek ortalama fiyat bilgileri için kriter bilgilerini tutan nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParTuketimOrtalamaFiyatSil(Kullanici kullanan, ParTuketimOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Harcama birimi ödenek talebi yaparken, ekonomik kod bazında gerekçe girilirken,
        /// ekonomik kod yolluk ile ilişkili ise, yolluk bilgileri girilir.
        /// Girilen yolluk bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="yolluklar">Kaydedilecek yolluk bilgilerini içeren dizi</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParYolluklarKaydet(Kullanici kullanan, ParYolluk[] yolluklar);

        /// <summary>
        /// Bir harcama biriminin talep ettiği yolluk bilgilerini verilen kriterlere göre bulup döndüren yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="yolluk">Listelenecek yolluk bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen yolluk kıstasına uyan, tüketim bilgilerinin listesi ParYolluk nesneleri içeren dizide döndürülür.</returns>
        ObjectArray ParYolluklarListele(Kullanici kullanan, ParYolluk yolluk);

        /// <summary>
        /// Yolluk bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini kaydeden yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Kaydedilecek ortalama fiyat bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParYolluklarOrtalamaFiyatKaydet(Kullanici kullanan, ParYollukOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Yolluk bilgileri girilirken, kullanılacak ortalama fiyat bilgilerini listeleyen yordam.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Listelenecek ortalama fiyat bilgilerinin kıstasını içeren nesne</param>
        /// <returns>Verilen ortalama fiyat kıstasına uyan ortalama fiyat bilgilerinin listesi ParYollukOrtalamaFiyat nesneleri içeren dizide döndürülür.</returns>
        ObjectArray ParYolluklarOrtalamaFiyatListele(Kullanici kullanan, ParYollukOrtalamaFiyat ortalamaFiyat);

        /// <summary>
        /// Kaydedilen yolluk bilgileri ortalama fiyatlarından verilen kritere uyanları silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler</param>
        /// <param name="ortalamaFiyat">Silinecek ortalama fiyat bilgileri için kriter bilgilerini tutan nesne</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ParYolluklarOrtalamaFiyatSil(Kullanici kullanan, ParYollukOrtalamaFiyat ortalamaFiyat);
        #endregion

        /// <summary>
        /// Verilen veri türleri listesi için yapılan en son değişiklik zamanını bulup döndüren yordam.
        /// </summary>
        /// <param name="degZaman">En son değişim zamanı bulunacak veri türlerinin listesini tutan dizi. Yıl, veri türü, kurkod, senaryo yada ay bilgileri kullanılır.</param>
        /// <returns>Verilen kısta bilgiler kullanılarak, değişim zamanı en yeni olan verinin, değişim zamanı döndürülür.</returns>
        TNSDateTime IslemKayitZaman(DegisimZaman[] degZaman);

        BirimMasrafPlaniGenel MasrafPlaniBulTavan(Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, UcgenKodBilgileri ukb, string saymanlikBirim, bool yeni = false);

        TertipOdenek[] BirimMasrafPlaniOzetListele(Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, int ekBilgi);

        Sonuc MasrafPlaniGenelOnayla(Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama mpAsama, int ay);

        Sonuc MasrafPlaniGenelOnayKaldir(Kullanici kullanan, int yil, string kurumsalYapiKod, string[] onayKalkacakBirimler, ENUMMasrafPlanAsama mpAsama);

        int MasrafPlaniGenelOnayDurumBul(Kullanici kullanan, int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama mpAsama);

        Sonuc MasrafPlaniGenelTertipKaydet(Kullanici kullanan, ENUMMasrafPlanAsama asama, TertipOdenek[] tertipOdenekler);

        TertipOdenek[] MPBirimTertipListele(Tertip kistasTertip, int saymanlik, string birim);
        TertipOdenek[] MPBirimTertipListele(Tertip kistasTertip, string[] kurumsalYapiKodlar);
        double[] MPMasrafPlaniOku(Kullanici kullanan, ENUMMasrafPlanAsama asama, Tertip kistasTertip, int islemNo, int ustIslemNo, bool onaysiz);

        bool OGYYetkisiVarMi(Kullanici kullanan, string senaryoKod, Tertip tertip, ENUMOGY ogy);

        MPTavanTertip[] MPTavanListele(Kullanici kullanan, string senaryoKod, Tertip tertip, string kyKod, bool detayliMi, string saymanlikBirim);
        TertipOdenek[] MPTavanIlListele(Kullanici kullanan, Tertip tertip, string saymanlikBirim);
        Sonuc MPTavanKaydet(Kullanici kullanan, string odenekKontrolSenaryo, TertipOdenek[] tertipOdenek, bool detayliMi, string silKYKod, bool toplamYaz, string saymanlikBirim);
        bool MPTavanIlIzniBul(Kullanici kullanan, Tertip tertip);
        Sonuc MPIlIzniDegistir(Kullanici kullanan, Tertip tertip);
        TertipOdenek[] MPListeleRapor(Kullanici kullanan, Tertip kistasTertip, UcgenKodBilgileri ukb, bool ekBilgili, string saymanlikBirim);
        ObjectArray MPFazlaGonderilen(Kullanici kullanan, int detayMi, Tertip kistasTertip);
        Sonuc MPTavanOnay(Kullanici kullanan, int yil, int detayMi, string kurKod, int onayKod);
        OdenekBilgi[] MPTavanGenelListele(Kullanici kullanan, string senaryoKod, Tertip kistasTertip);
        TertipOdenek[] MPTertipListeleAy(Kullanici kullanan, Tertip kistas, string birim);

        Tertip[] TertipOdenekAlacaklarListele(Kullanici kullanan, Tertip tertip, bool kayitli);
        Sonuc TertipOdenekAlacaklarKaydet(Kullanici kullanan, Tertip tertip, Tertip[] odenekAlacagiTertipler);

        MPBilgi[] BirimMasrafPlaniBul(Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, string ekoKod);
        Sonuc MasrafPlaniGenelOtomatikHesapla(Kullanici kullanan, int yil, string kurumsalYapiKod, string ekoKod2, double artisYuzde, bool ustuneYaz, bool bosIseYaz);
        Sonuc MasrafPlaniGenelUretAHP(Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, bool ustuneYaz, int ay, Tertip tertip);

        TertipOdenek[] MasrafPlaniTertipToplamGonderilen(Kullanici kullanan, ENUMMasrafPlanAsama eNUMMasrafPlanAsama, int ay, Tertip kistasTertip);
        MPRevizeTalep[] MasrafPlaniRevizeTalepListele(Kullanici kullanan, MPRevizeTalep kistasForm);

        BirimMasrafPlaniGenel MasrafPlaniBul(Kullanici kullanan, ENUMMasrafPlanAsama eNUMMasrafPlanAsama, Tertip kistasTertip, UcgenKodBilgileri ukb, EkDegerParam[] ekBilgiler);

        Sonuc MPRevizeFormKaydet(Kullanici kullanan, MPRevizeForm form);

        Sonuc MPRevizeFormKaydetEklDus(Kullanici kullanan, MPRevizeForm form, bool dortDuzeyDegisimUret, bool eklDus);

        Sonuc MPRevizeFormKaydet(Kullanici kullanan, MPRevizeForm form, bool dortDuzeyDegisimUret);

        MPRevizeForm MPRevizeFormOku(Kullanici kullanan, int yil, int revizeNo);

        Sonuc MPRevizeFormDurumAta(Kullanici kullanan, int yil, int revizeNo, BKD.ENUMRevizeFormDurum formDurum);

        Sonuc MPRevizeFormDurumAta(Kullanici kullanan, TNSCollection formlar);

        string FormKosulOl(Kullanici kullanan, MPRevizeFormSorgu form, string tabloAd, bool detayaBak, string JoinToDisTabloAd);

        MPRevizeForm[] MPRevizeFormListele(Kullanici kullanan, MPRevizeFormSorgu form);

        TertipOdenek[] MPBirimListele(Kullanici kullanan, Tertip kistas);

        TertipOdenek[] MPTertipListele(Kullanici kullanan, string kurumsalYapiKod, Tertip kistas);

        Sonuc MPRevizeTelepKaydet(Kullanici kullanan, MPRevizeTalep form);

        Sonuc MPRevizeTalepDurumAta(Kullanici kullanan, int yil, string kurumsalYapiKod, int revizeTalepNo, BKD.ENUMRevizeFormDurum eNUMRevizeFormDurum);

        MPRevizeTalep MPRevizeTalepOku(Kullanici kullanan, int yil, string kurumsalKod, int revizeTalepNo);

        Sonuc[] MPRevizeVeyaTalepYap(Kullanici kullanan, MPRevizeTalep form, int eskiTalepNo, int eskiRevizeNo);

        bool TertipDegisimIzniVarMi(Kullanici kullanan, Tertip alanTertip, Tertip verenTertip);

        Sonuc MasrafPlaniKarsilananlariYaz(Kullanici kullanan, MPRevizeTalep[] revizeTalepler, int revizeNo, bool bosSatirKontroluYap);

        double[] MasrafPlaniOdenekBilgiBul(Tertip tertip);
        double[] MasrafPlaniOdenekBilgiBul(Tertip tertip, bool onaysiz, bool harcamaVar, bool blokeVar);
        double[] MasrafPlaniOdenekBilgiBul(Tertip tertip, bool onaysiz, int haricRevizeNo);

        Sonuc MasrafPlaniTalepleriIptalEt(Kullanici kullanan, MPRevizeTalep[] mPRevizeTalep);

        Sonuc MPRevize4DuzeyDegisimYap(Kullanici kullanan, int yil, int revizeNo);

        int TertipSayisiBul(Tertip tertip);

        Sonuc TertipTanimKopyalaOncYil(Kullanici kullanan, Tertip tertip);

        /// <summary>
        /// Numarası verilen tasnif, ve tasnife ait tüm bilgiler silinir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgiler.</param>
        /// <param name="tasnifSiraNo">Tasnif bilgisi silinecek tasnifin sıra numarası.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        Sonuc ButceTasnifHiyerarsiSil(Kullanici kullanan, int tasnifSiraNo);

        Sonuc ButceTasnifSenaryolarAta(Kullanici kullanan, ButceTasnif form, ButceSenaryo[] bsler);

        ButceTasnif ButceTasnifSenaryoEkle(Kullanici kullanan, ButceTasnif form, ButceSenaryo bs);

        ButceTasnif ButceTasnifSenaryoSil(Kullanici kullanan, ButceTasnif form, ButceSenaryo bs);

        Sonuc ButceOdenekKopyala(Kullanici kullanan, Tertip hedefTertip, string hedefAsama, int kaynakYil, string kaynakAsama);

        Sonuc ButceOdenekOnayla(Kullanici kullanan, string birim, int hedefYil, string hedefAsama, int onayDurum);

        Sonuc KaydetHarcamaSurecOnay(int yil, ObjectArray liste);
        Sonuc KaydetHarcamaSurecAlim(int yil, ObjectArray liste);
        Sonuc KaydetHarcamaSurecFatura(int yil, ObjectArray liste);
        Sonuc KaydetHarcamaSurecTeslim(int yil, ObjectArray liste);
        Sonuc KaydetHarcamaSurecOdeme(int yil, ObjectArray liste);
        Sonuc SilSurecBilgi(int hedefYil, int yil, string kur, string fon, string fin, string eko);
        ObjectArray SurecBilgiListele(Kullanici kullanan, BHMHarcamaSorgu kriter);

        /// <summary>
        /// Tavan üstü bilgilerini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="senaryoKod">Ödenek üstünün ait olduğu senaryo.</param>
        /// <param name="tertip"> Tavan üstü bilgisi okunacak tertiplerin kıstası</param>
        TavanUstuTertipOdenek[] TavanUstuOdenekListele(Kullanici kullanan, string[] senaryoKod, KistasYapi tertip, bool ButceFormdan);

        /// <summary>
        /// Tavan üstü ödenek bilgilerini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="senaryoKod">Ödenek üstünün ait olduğu senaryo.</param>
        /// <param name="odenekler">Kaydedilecek tavan üstü ödenek bilgileri</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc TavanUstuOdenekKaydet(Kullanici kullanan, string senaryoKod, TavanUstuTertipOdenek[] odenekler);

        ABSKod[] BirimKodListele(int yil, string kodTur, string birimKurKod);

        Sonuc AltFaaliyetIliskiKaydet(Kullanici kullanan, AltFaaliyet[] altFaaliyetler, int tur, string senaryoKod);

        Sonuc TorbaTertipBirimOdenekKaydet(Kullanici kullanan, TorbaTertipBirimOdenek torbatertipbirim);

        ObjectArray TorbaTertipBirimOdenekListele(Kullanici kullanan, TorbaTertipBirimOdenek torbatertipbirim);

        ObjectArray BirimTorbaOdenekTalepListele(Kullanici kullanan, BirimTorbaOdenekTalep talep);

        Sonuc BirimTorbaOdenekTalepKaydet(Kullanici kullanan, BirimTorbaOdenekTalep talep);

        Sonuc BirimTorbaOdenekTalepDurumGuncelle(Kullanici kullanan, BirimTorbaOdenekTalep talep);

        /// <summary>
        /// ProgramOnayAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        ProgramOnay[] ListeleProgramOnayForm(Kullanici kullanan, ProgramOnay form);

        /// <summary>
        /// ProgramOnayAna nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        ProgramOnay OkuProgramOnayForm(Kullanici kullanan, ProgramOnay form);

        /// <summary>
        /// ProgramOnayAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ProgramOnayAna yapısında ProgramOnayAna:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetProgramOnayForm(Kullanici kullanan, ProgramOnay form);
        Sonuc OnaylaProgramOdenek(Kullanici kullanan, int yil, string senaryoKod, string programKod);

        OdenekSenaryoDurum[] ButceOdenekDurumListele(Kullanici kullanan, int yil, string birim);

        Sonuc ButceOdenekDurumKaydet(Kullanici kullanan, int yil, string birim, OdenekSenaryoDurum[] onaylar);

        /// <summary>
        /// BirimDegisken listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimDegisken yapısında BirimDegisken:  bilgilerini tutan değişken</param>
        BirimDegisken[] ListeleBirimDegisken(Kullanici kullanan, int yil, int saymanlik, string birim, string kurumKodu);

        /// <summary>
        /// BirimDegisken nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">BirimDegisken yapısında BirimDegisken:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetBirimDegisken(Kullanici kullanan, int yil, int saymanlik, string birim, string kurumKodu, BirimDegisken[] degiskenler);

        BirimDegiskenYapi BirimDegiskenYapiOku(Kullanici kullanan, string kurKod);

        Sonuc BirimDegiskenYapiSakla(Kullanici kullanan, string kurKod, BirimDegiskenYapi by);

        Tertip OncekiSonrakiTertip(Kullanici kullanan, int yil, Tertip simdikiTertip, Tertip kistasTertip, int yon);

        string OnayliAsamaBul(int secYil);

        Sonuc MPAHPdenMPye(Kullanici kullanan, int yil, ENUMMasrafPlanAsama asama, TNSCollection tertipler);

        Sonuc MPdenMPye(Kullanici kullanan, int yil, ENUMMasrafPlanAsama kaynakAsama, ENUMMasrafPlanAsama hedefAsama);


        ///// <summary>
        ///// FaaliyetinYapildigiYer listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
        //FaaliyetinYapildigiYer[] ListFaaliyetinYapildigiYer(Kullanici kullanan, FaaliyetinYapildigiYer form);

        ///// <summary>
        ///// FaaliyetinYapildigiYer nesnesini veri tabanından okuyan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
        //FaaliyetinYapildigiYer GetFaaliyetinYapildigiYer(Kullanici kullanan, FaaliyetinYapildigiYer form);

        ///// <summary>
        ///// FaaliyetinYapildigiYer nesnesini veri tabanına yazan fonksiyon
        ///// Hata olursa Exception oluşturur
        ///// </summary>
        ///// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        ///// <param name="form">FaaliyetinYapildigiYer yapısında FaaliyetinYapildigiYer: Faaliyetin Yapıldığı Yer bilgilerini tutan değişken</param>
        ///// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        //Sonuc SaveFaaliyetinYapildigiYer(Kullanici kullanan, FaaliyetinYapildigiYer form);

        //FaaliyetinYapildigiYer[] ListeleTumunu(Kullanici kullanan, FaaliyetinYapildigiYer form);
        //bool BirimeGoreFaaliyetGetir(Kullanici kullanan, FaaliyetinYapildigiYer form, int birimkod);
        //bool IllereGoreFaaliyetGetir(Kullanici kullanan, FaaliyetinYapildigiYer form, int ilkod);

        /// <summary>
        /// BirimButceHazirlik listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertip">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        ObjectArray ListeleBirimButceHazirlik(Kullanici kullanan, Tertip tertip, int saymanlik, string birim, string senaryo, bool birimGrupla, bool eko2);

        /// <summary>
        /// BirimButceHazirlik nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="tertipler">BirimButceHazirlik yapısında BirimButceHazirlik: Birim Bütçe Hazırlık bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc SaklaBirimButceHazirlik(Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, TertipOdenek[] tertipler);
        ObjectArray ListeleBirimButceKarsilama(Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, bool sifirlarGizli, int tur);
        Sonuc BirimButceDetayOranla(Kullanici kullanan, Tertip tertip, int il, int saymanlik, string birim, double karsilanan, int dagitimYontem);

        Sonuc OnayKodAtaTasra(Kullanici kullanan, int yil, int saymanlik, string birim, string asama, ENUMBirimOnayTur onayTur, ENUMTasraHazirlikOnayDurum onayDurum);
        Sonuc OnayKodAtaToplu(Kullanici kullanan, int yil, int saymanlik, string birim, string asama, ENUMBirimOnayTur onayTur, int onayKod);
        ObjectArray OnayKodListeleTasra(Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, ENUMBirimOnayTur onayTur);
        int OnayKodOkuTasra(Kullanici kullanan, int yil, int saymanlik, string birim, string senaryoKod, ENUMBirimOnayTur onayTur);

        bool IlKullanicisiMi(Kullanici kullanan, int ilKod);

        /// <summary>
        /// Tavan nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">Tavan yapısında Tavan:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetBHMTavanGenel(Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TNSCollection tavanListe);

        ObjectArray ListeleBHMTavanGenel(Kullanici kullanan, int yil, string senaryo, ENUMTavanTur tavanTur, TavanGenel kistas, bool sadeceOnayli, bool bosEkle = false);

        /// <summary>
        /// Tavan listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl: Listelenecek tavan yılını gösterir</param>
        /// <param name="ekoKod">ekoKod: Listelenecek ekonomik kodu gösterir</param>
        /// <param name="sektorKod">sektorKod: Listelenecek sektör kodunu gösterir</param>
        ObjectArray ListeleEkoSektorTavan(Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string oncekiYilSenaryo);

        ObjectArray ListeleEkoSektorFaaliyetTavan(Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string pebFaalKod);

        ObjectArray ListeleYatirimProjeTavan(Kullanici kullanan, int yil, string senaryoKod, string ekoKod, string sektorKod, string pebBirimKod, string pebFaalKod);

        ObjectArray ListeleYatirimVizeTavan(Kullanici kullanan, int yil, string senaryoKod, string pebBirimKod, string ekoKod, string finKod, string pebFaalKod, int projeKod);

        ObjectArray ListeleYatirimVizeTavanRapor(Kullanici kullanan, int yil, string senaryoKod, bool detay, string pebKod, string ekoKod, string finKod);

        ObjectArray ListeleTertipTavan(Kullanici kullanan, int yil, string senaryoKod, string[] kodlar);

        ObjectArray ListeleTertipVize(Kullanici kullanan, int yil, string senaryoKod, string finkod, string ekoKod, string pebFaalKod);

        ObjectArray BirimButceHazirlikOnayDurum(Kullanici kullanan, int yil, string senaryoKod, int saymanlik, string birim);

        TavanGenel BHMTavanGenelOnayOku(Kullanici kullanan, int yil, string senaryKod, ENUMTavanTur tavanTur, TavanGenel form);

        ObjectArray BHMTavanGenelOnayListele(Kullanici kullanan, int yil, string senaryoKod, ENUMTavanTur tavanTur, TavanGenel form);

        Sonuc BHMTavanGenelOnayKaydet(Kullanici kullanan, int yil, string senaryoKod, ENUMTavanTur tavanTur, TavanGenel form);

        ObjectArray ListeleGorusmeCari(Kullanici kullanan, Tertip tertip, string senaryoKod, bool eko2);

        /// <summary>
        /// MaliDurumBeklenti listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        MaliDurumBeklenti[] ListeleBHMMaliDurumBeklenti(Kullanici kullanan, MaliDurumBeklenti form);

        /// <summary>
        /// MaliDurumBeklenti nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        MaliDurumBeklenti OkuBHMMaliDurumBeklenti(Kullanici kullanan, MaliDurumBeklenti form);

        /// <summary>
        /// MaliDurumBeklenti nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">MaliDurumBeklenti yapısında MaliDurumBeklenti:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetBHMMaliDurumBeklenti(Kullanici kullanan, MaliDurumBeklenti form);

        /// <summary>
        /// IOIProje listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        ObjectArray ListeleIOIProjeGiris(Kullanici kullanan, IOIProje form, bool detaylarOlsun, int butceYil);

        /// <summary>
        /// IOIProje listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        ObjectVAN ListeleIOIProjePEBBilgi(Kullanici kullanan, int yil, string pebKod);

        /// <summary>
        /// IOIProje nesnesini veri tabanından okuyan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        ObjectVAN OkuIOIProjeGiris(Kullanici kullanan, IOIProje form);

        /// <summary>
        /// IOIProje nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">IOIProje yapısında IOIProje:  bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetIOIProjeGiris(Kullanici kullanan, IOIProje form);

        Sonuc SilIOIProjeGiris(Kullanici kullanan, IOIProje form);

        ObjectArray ListeleYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form);

        Sonuc KaydetYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form);

        ObjectVAN OkuYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form);

        Sonuc SilYIKOBProjeGiris(Kullanici kullanan, YIKOBProje form);

        ObjectArray SektorKullanilmisMi(Kullanici kullanan, int yil, string senaryoKod, string sektorKod);

        Sonuc TavanUretEkoSektorBirim(Kullanici kullanan, int yil, string senaryoKod, string kaynakSenaryoKod);

        /// <summary>
        /// ButceCalismasiPersonelAna listesini veri tabanından okuyan ve dizi olarak döndüren fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="form">ButceCalismasiPersonelAna yapısında ButceCalismasiPersonelAna: Bütçe Çalışmalarında Görevli Personel Listesi bilgilerini tutan değişken</param>
        ObjectArray ListeleButceCalismasiPersoneli(Kullanici kullanan, ButceCalismasiPersoneli form);

        /// <summary>
        /// ButceCalismasiPersonelAna nesnesini veri tabanına yazan fonksiyon
        /// Hata olursa Exception oluşturur
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="formlar">ButceCalismasiPersonelAna yapısında ButceCalismasiPersonelAna: Bütçe Çalışmalarında Görevli Personel Listesi bilgilerini tutan değişken</param>
        /// <returns>İşlem sonucunu döndüren, Sonuc sınıfında nesne</returns>
        Sonuc KaydetButceCalismasiPersoneli(Kullanici kullanan, TNSCollection formlar);
        Sonuc KopyalaButceCalismasiPersoneli(Kullanici kullanan, int yil);

        PEBProjeBilgi PEBBilgiBul(Kullanici kullanan, int yil, string peb);
        ObjectArray HizmetMaliyetiTasraListele(Kullanici kullanan, int hizMalKontrolYil, int saymanlik, string birim, string kurKod, bool kurumsalToplam);

        TertipOdenek[] DagitilmayanlariHesapla(Kullanici kullanan, int yil);

        ObjectVAN OdenekGondermeKontrolMiktarlariniBul(Kullanici kullanan, int yil, TertipKodlari kodlar);
        Hashtable ProgramButceCevrimOku(int yil);

        Sonuc ProjeKaydet(Kullanici kullanan, ProjeTeklif form);
        ObjectArray ProjeListele(Kullanici kullanan, ProjeTeklif form);
        Sonuc ProjeSil(Kullanici kullanan, ProjeTeklif form);

        Sonuc ProjeBirimKaydet(Kullanici kullanan, ProjeBirim form);
        ObjectArray ProjeBirimListele(Kullanici kullanan, ProjeBirim form);
        Sonuc ProjeBirimSil(Kullanici kullanan, ProjeBirim form);

        Sonuc ProjeYurutucuKaydet(Kullanici kullanan, ProjeYurutucu form);
        ObjectArray ProjeYurutucuListele(Kullanici kullanan, ProjeYurutucu form);
        Sonuc ProjeYurutucuSil(Kullanici kullanan, ProjeYurutucu form);

        Sonuc ProjeFaaliyetKaydet(Kullanici kullanan, ProjeFaaliyet form);
        ObjectArray ProjeFaaliyetListele(Kullanici kullanan, ProjeFaaliyet form);
        Sonuc ProjeFaaliyetSil(Kullanici kullanan, ProjeFaaliyet form);

        Sonuc ProjeUrunKaydet(Kullanici kullanan, ProjeUrun form);
        ObjectArray ProjeUrunListele(Kullanici kullanan, ProjeUrun form);
        Sonuc ProjeUrunSil(Kullanici kullanan, ProjeUrun form);

        Sonuc ProjeFinansmanKaydet(Kullanici kullanan, ProjeFinansman form);
        ObjectArray ProjeFinansmanListele(Kullanici kullanan, ProjeFinansman form);
        Sonuc ProjeFinansmanSil(Kullanici kullanan, ProjeFinansman form);

        Sonuc ProjeGirdiKaydet(Kullanici kullanan, ProjeGirdi form);
        ObjectArray ProjeGirdiListele(Kullanici kullanan, ProjeGirdi form);
        Sonuc ProjeGirdiSil(Kullanici kullanan, ProjeGirdi form);

        //Sonuc ProjeBirimIlgiliKaydet(Kullanici kullanan, ProjeBirimIlgili form);
        //ObjectArray ProjeBirimIlgiliListele(Kullanici kullanan, ProjeBirimIlgili form);
        //Sonuc ProjeBirimIlgiliSil(Kullanici kullanan, ProjeBirimIlgili form);

        Sonuc ProjeSorunKaydet(Kullanici kullanan, ProjeSorun form);
        ObjectArray ProjeSorunListele(Kullanici kullanan, ProjeSorun form);
        Sonuc ProjeSorunSil(Kullanici kullanan, ProjeSorun form);

        Sonuc FaaliyetKaydet(Kullanici kullanan, Faaliyet form);
        ObjectArray FaaliyetListele(Kullanici kullanan, Faaliyet form);
        Sonuc FaaliyetSil(Kullanici kullanan, Faaliyet form);

        Sonuc FinansmanKaynakKaydet(Kullanici kullanan, FinansmanKaynak form);
        ObjectArray FinansmanKaynakListele(Kullanici kullanan, FinansmanKaynak form);
        Sonuc FinansmanKaynakSil(Kullanici kullanan, FinansmanKaynak form);

        Sonuc UrunKaydet(Kullanici kullanan, Urun form);
        ObjectArray UrunListele(Kullanici kullanan, Urun form);
        Sonuc UrunSil(Kullanici kullanan, Urun form);

        Sonuc ProjeFaaliyetGostergeKaydet(Kullanici kullanan, ProjeFaaliyetGosterge form);
        ObjectArray ProjeFaaliyetGostergeListele(Kullanici kullanan, ProjeFaaliyetGosterge form);
        Sonuc ProjeFaaliyetGostergeSil(Kullanici kullanan, ProjeFaaliyetGosterge form);

        Sonuc ProjeAciklamaTurKaydet(Kullanici kullanan, ProjeAciklamaTur form);
        ObjectArray ProjeAciklamaTurListele(Kullanici kullanan, ProjeAciklamaTur form);
        Sonuc ProjeAciklamaTurSil(Kullanici kullanan, ProjeAciklamaTur form);

        Sonuc ProjeAciklamaKaydet(Kullanici kullanan, ProjeAciklama form);
        ObjectArray ProjeAciklamaListele(Kullanici kullanan, ProjeAciklama form);
        Sonuc ProjeAciklamaSil(Kullanici kullanan, ProjeAciklama form);
        ObjectArray SPAmacHedefListele(Kullanici kullanan);
        ObjectVAN ProjeFaaliyetGerceklesmeListele(Kullanici kullanan, int yil, int ay, string faaliyetId);
        Sonuc ProjeFaaliyetGerceklesmeKaydet(Kullanici kullanan, ProjeFaaliyetGerceklesme gerc);
        ObjectArray IOIProjeHarcamaListele(Kullanici kullanan, int ioiProjeNo);
        ObjectArray IOIProjeAktarimListele(Kullanici kullanan, int ioiProjeNo);
        Sonuc IOIProjeHarcamaKaydet(Kullanici kullanan, int ioiProjeNo, IOIProjeHarcama[] liste);
        Sonuc IOIProjeAktarimKaydet(Kullanici kullanan, int ioiProjeNo, IOIProjeAktarilan[] liste);
        Sonuc ButceIslemUret(Kullanici kullanan, int yil, MPRevizeSatir islemSatir, int yontem);
        Sonuc KaydetMPDagitim(Kullanici kullanan, MPDagitimAna form);
        ObjectArray ListeleMPDagitim(Kullanici kullanan, MPDagitimAna mPDagitimAna);
        ObjectVAN OkuMPDagitim(Kullanici kullanan, int id);
        ObjectVAN MPDagitimBirimEkle(Kullanici kullanan, int id, string birimKod, double tutar);
        Sonuc MPDagitimRevizeUret(Kullanici kullanan, int id, int ay);
    }
}
