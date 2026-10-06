using TNS.KYM;
using TNS.BKD;
using System.Collections;
using System;
using System.Data;
using System.Text;

namespace TNS.BHMv2
{
    /// <summary>
    /// Faaliyetler bilgileri, sabit maliyet ve faaliyet olarak ikiye ayrılmıştır. Bu iki bilgiyi tutan liste yapı
    /// </summary>
    public enum EnumFaaliyetSabitMaliyet
    {
        /// <summary>
        /// Faaliyet olan faaliyetler
        /// </summary>
        Faaliyet = 0,

        /// <summary>
        /// Sabit maliyet olan faaliyetler
        /// </summary>
        SabitMaliyet = 1,
    }

    /// <summary>
    /// Alt faaliyet ile ilişki faaliyetin bilgilerini tutan sınıf.
    /// </summary>
    [System.Serializable()]
    public class FaaliyetBag
    {
        /// <summary>
        /// Faaliyetin kurumsal  kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Faaliyet sabit mi? faaliyet mi? bilgisi
        /// </summary>
        public EnumFaaliyetSabitMaliyet sabitFaaliyet;

        /// <summary>
        /// Faaliyetin kodu
        /// </summary>
        public int sabitFaaliyetKod;

        /// <summary>
        /// Faaliyet kodunu S-Numara veya F-Numara döndüren yordam
        /// </summary>
        /// <returns></returns>
        public string Tireli()
        {
            string onEk;
            if (sabitFaaliyet == EnumFaaliyetSabitMaliyet.Faaliyet)
                onEk = "F";
            else
                onEk = "S";
            return onEk + "-" + sabitFaaliyetKod.ToString();
        }

        /// <summary>
        /// S-Numara veya F-Numara şeklindeki dizgiden faaliyet kodu bilgileri bularak, faaliyet nesnesi üreten oluşturucu yordam.
        /// </summary>
        /// <param name="kurKod">Sabit maliyetin kurumsal kodu</param>
        /// <param name="tireli">Sabit maliyetin '-' karakteri ile ayrımış, faaliyet türü(S/F) ve numarası (Örneğin S-1). </param>
        public FaaliyetBag(string kurKod, string tireli)
        {
            this.kurKod = kurKod;
            string[] fsklar = tireli.Split('-');
            bool hata = false;
            if (fsklar.Length == 2)
            {
                if (fsklar[0].Equals("S", StringComparison.CurrentCultureIgnoreCase))
                    this.sabitFaaliyet = EnumFaaliyetSabitMaliyet.SabitMaliyet;
                else if (fsklar[0].Equals("F", StringComparison.CurrentCultureIgnoreCase))
                    this.sabitFaaliyet = EnumFaaliyetSabitMaliyet.Faaliyet;
                else
                    hata = true;

                this.sabitFaaliyetKod = OrtakFonksiyonlar.ConvertToInt(fsklar[1], -1);

                if (this.sabitFaaliyetKod <= 0)
                    hata = true;
            }
            else
                hata = true;
            if (hata)
                throw new Exception("Hatalı faaliyet/sabit maliyet dizgisi (" + tireli + ")");
        }
    }


    [System.Serializable()]
    public class BirimYapi
    {
        public string birimKod = "";
        public int muhasebeKod = 0;
        public string kurKod = "";
        public string il = "";
        public string muhAd = "";
        public string birimAd = "";

        public string muhBirimAd
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(muhAd)) return birimAd + " (" + muhAd + ")";
                return birimAd;
            }
        }

        public string deger
        {
            get
            {
                if (this.muhasebeKod > 0 && !string.IsNullOrEmpty(this.birimKod) && !string.IsNullOrEmpty(this.kurKod))
                    return this.muhasebeKod + "-" + this.birimKod + "-" + this.kurKod;
                else
                    return "";

            }
            set
            {
                bool hata = true;
                if (value != null)
                {
                    string[] p = value.Split('-');
                    if (p.Length == 3)
                    {
                        hata = false;
                        this.muhasebeKod = OrtakFonksiyonlar.ConvertToInt(p[0], 0);
                        this.birimKod = p[1];
                        this.kurKod = p[2];
                    }
                }
                if (hata)
                {
                    this.muhasebeKod = 0;
                    this.birimKod = "";
                    this.kurKod = "";
                }
            }

        }
    }

    /// <summary>
    /// Alt faaliyet kimlik bilgileri ile alt faaliyetle faaliyet ve alt proje kaynak arasındaki ilişkileri tutan sınıf.
    /// </summary>
    [System.Serializable()]
    public class AltFaaliyet
    {

        /// <summary>
        /// Alt faaliyetin yılı
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Kod: Alt faaliyet kodu
        /// </summary>
        public string pebKod { get; set; }

        /// <summary>
        /// Alt faaliyet adı (Konfigürasyondaki birinci dil için)
        /// </summary>
        public string pebAd { get; set; }

        /// <summary>
        /// Alt faaliyet adı (Konfigürasyondaki ikinci dil için)
        /// </summary>
        public string pebAd2 { get; set; }

        /// <summary>
        /// Alt faaliyetle ilişkili kurumsal kod
        /// </summary>
        public string kurKod { get; set; }

        /// <summary>
        /// Alt faaliyetle ilişkili fonksiyonel kod
        /// </summary>
        public string fonKod { get; set; }

        /// <summary>
        /// Alt faaliyetle ilişkili finans kodu
        /// </summary>
        public string finKod { get; set; }

        /// <summary>
        /// Alt faaliyetle ilişkili alt proje numarası
        /// </summary>
        public int altProje { get; set; }

        /// <summary>
        /// Alt faaliyetle ilişkili alt projenin kaynak numarası
        /// </summary>
        public int butceKaynakNo { get; set; }

        /// <summary>
        /// Alt faaliyetle ilişkili faaliyetin veya sabit maliyetin kodu
        /// </summary>
        public FaaliyetBag faaliyetBag = null;

        /// <summary>
        /// Alt faaliyetle ilişkili harcama birimleri şablonlarının dizisi
        /// </summary>
        public string[] kurYapiSablonListe;

        public bool KurumsalYapiUyumluMu(KurumsalYapi kY, int sablonKes) //, out string bilgi
        {
            bool uyumlu = false;
            //bilgi = "1";
            if (TNS.BKD.Arac.KodKiyasla(kY.kurKod, kurKod) || TNS.BKD.Arac.KodKiyasla(kurKod, kY.kurKod))
            {
                //bilgi += "2";
                if (kurYapiSablonListe.Length > 0)
                {
                    for (int j = 0; j < kurYapiSablonListe.Length && !uyumlu; j++)
                    {
                        string sablon = kurYapiSablonListe[j];
                        if (sablonKes > 0)
                        {
                            string[] sablonParcalar = sablon.Split('.');
                            sablon = "";
                            for (int i = 0; i < Math.Min(sablonKes, sablonParcalar.Length); i++)
                            {
                                sablon += sablonParcalar[i] + ".";
                            }
                        }
                        //bilgi += " ::" +kY.kod + ":" +sablon +"; ";

                        if (TNS.BKD.Arac.KodKiyasla(kY.kod, sablon))
                        {
                            //bilgi += " buldu ";
                            uyumlu = true;
                        }
                    }
                }
                else
                    uyumlu = true;
            }
            return uyumlu;
        }

        public string altProjeKaynakAd = null;

        public string AltProjeKaynakKod
        {
            get
            {
                if (altProje > 0 && butceKaynakNo > 0)
                    return altProje.ToString() + "-" + butceKaynakNo.ToString();
                else
                    return "";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string altProjeS = "";
                    string butceKaynakS = "";
                    OrtakFonksiyonlar.StringiIkiyeBol(value, "-", ref altProjeS, ref butceKaynakS);
                    altProje = OrtakFonksiyonlar.ConvertToInt(altProjeS, 0);
                    butceKaynakNo = OrtakFonksiyonlar.ConvertToInt(butceKaynakS, 0);
                }
                else
                {
                    altProje = 0;
                    butceKaynakNo = 0;
                }
            }
        }

        public string FaaliyetBagKod
        {
            get
            {
                if (faaliyetBag != null)
                    return faaliyetBag.kurKod + "-" + faaliyetBag.Tireli();
                else
                    return "";
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string faalTireli = "";
                    string faalKurKod = "";
                    OrtakFonksiyonlar.StringiIkiyeBol(value, "-", ref faalKurKod, ref faalTireli);
                    faaliyetBag = new FaaliyetBag(faalKurKod, faalTireli);
                }
                else
                {
                    faaliyetBag = null;
                }
            }
        }

        public string faaliyetAd = null;

        public int kodUzunluk = -1;


        public int iliskileriOku = 0;
    }

    /// <summary>
    /// Bütçe hazırlık esnasında harcamacı birimlerin taşınır girişleri sırasında kullanacakları ortalama fiyatları tutan sınıf.
    /// </summary>
    [Serializable()]
    public class ParTasinirOrtalamaFiyat
    {
        /// <summary>
        /// Taşınır ortalama fiyatının ait olduğu yıl
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Taşınır ortalama fiyatının ait olduğu hesap kodu
        /// </summary>
        public string hesapkod = string.Empty;

        /// <summary>
        /// Sınıfta tutulan hesap kodunun, ilgili yıldaki ortalam fiyatı
        /// </summary>
        public double ortalamafiyat = 0;
    }

    /// <summary>
    /// Bütçe hazırlık esnasında harcamacı birimlerin tüketim girişleri sırasında kullanacakları tüketim tipi bilgilerini tutan sınıf.
    /// </summary>
    public class TuketimTip
    {
        /// <summary>
        /// Tüketim tiplerinin kodunu ve adını tutan statik değişken dizi
        /// </summary>
        public static TuketimTip[] tuketimTipler;

        /// <summary>
        /// Tüketim tipinin kodu
        /// </summary>
        public string kod;

        /// <summary>
        /// Tüketim tipinin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Tüketim tipinin ilgili olduğu telefon aboneliği türleri tipleri
        /// </summary>
        public TNS.HRC.ENUMAboneTip aboneTip;

        /// <summary>
        /// Sadece kod alanı bilinen tüketim tipleri için oluşturucu yordam
        /// </summary>
        /// <param name="kod">Tüketim tipi kodu</param>
        public TuketimTip(string kod)
        {
            this.kod = kod;
        }

        /// <summary>
        /// Kod, ad ve abone tipi bilinen tüketim tipleri için oluşturucu yordam
        /// </summary>
        /// <param name="kod">Tüketim tipi kodu</param>
        /// <param name="ad">Tüketim tipi adı</param>
        /// <param name="aboneTip">Tüketim tipinin ilgili olduğu telefon aboneliği türleri tipleri</param>
        public TuketimTip(string kod, string ad, TNS.HRC.ENUMAboneTip aboneTip)
        {
            this.aboneTip = aboneTip;
            this.kod = kod;
            this.ad = ad;
        }

        /// <summary>
        /// Statik kurucu yordam, tüketim tipler dizisi doldurulur.
        /// </summary>
        static TuketimTip()
        {
            tuketimTipler = new TuketimTip[6];
            tuketimTipler[0] = new TuketimTip("ADSL", TNS.BHMv2.ButceHazirlikResI.BHM022, TNS.HRC.ENUMAboneTip.ADSL);
            tuketimTipler[1] = new TuketimTip("AVEA", TNS.BHMv2.ButceHazirlikResI.BHM023, TNS.HRC.ENUMAboneTip.AVEA);
            tuketimTipler[2] = new TuketimTip("Elektrik", TNS.BHMv2.ButceHazirlikResI.BHM024, TNS.HRC.ENUMAboneTip.TANIMSIZ);
            tuketimTipler[3] = new TuketimTip("Telefon", TNS.BHMv2.ButceHazirlikResI.BHM025, TNS.HRC.ENUMAboneTip.TELEFON);
            tuketimTipler[4] = new TuketimTip("Turkcell", TNS.BHMv2.ButceHazirlikResI.BHM026, TNS.HRC.ENUMAboneTip.TURKCELL);
            tuketimTipler[5] = new TuketimTip("Vodafone", TNS.BHMv2.ButceHazirlikResI.BHM027, TNS.HRC.ENUMAboneTip.VODAFONE);
        }
    }

    /// <summary>
    /// Tüketim tipler dizisinde yer alan her tüketim tipi için ortalama fiyat tutan sınıf
    /// </summary>
    [Serializable()]
    public class ParTuketimOrtalamaFiyat
    {
        /// <summary>
        /// Tütetim tipinin ortalama fiyatının yılı
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Ortalama fiyatı tutulan tüketim tipinin kodu
        /// </summary>
        public TuketimTip tuketimTip = null;

        /// <summary>
        /// Tüketim tipinin, ilgili yıldaki ortalama fiyatı
        /// </summary>
        public double ortalamaFiyat = 0;
    }


    /// <summary>
    /// Bütçe hazırlık esnasında harcamacı birimlerin taşınır taleplerini ve ilgili tertibi tutan dizi
    /// </summary>
    [Serializable()]
    public class ParTalepTasinir
    {
        /// <summary>
        /// Taşınır talebi yapılan yıl
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Taşınır talebinin yapıldığı aşama
        /// </summary>
        public ENUMBHBirimAsama asama = ENUMBHBirimAsama.URETIM;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin kurumsal kodu
        /// </summary>
        public string kur = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin fonksiyonel kodu
        /// </summary>
        public string fon = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin finans kodu
        /// </summary>
        public string fin = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin ekonomik kodu
        /// </summary>
        public string eko = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin alt faaliyet kodu
        /// </summary>
        public string peb = string.Empty;

        /// <summary>
        /// Talebi yapan birimin kodu
        /// </summary>
        public string kurumsalYapiKod = string.Empty;

        /// <summary>
        /// Talep yapılan malzemenin taşınır hesap kodu
        /// </summary>
        public string tasHesapKod = string.Empty;

        /// <summary>
        /// İstenilen malzemeden ne kadar talep edildiği
        /// </summary>
        public int miktar = 0;
    }

    /// <summary>
    /// Bütçe hazırlık esnasında harcamacı birimlerin taşınır tüketim taleplerini ve ilgili tertibi tutan dizi
    /// </summary>
    [Serializable()]
    public class ParTuketim
    {
        /// <summary>
        /// Tüketim talebi yapılan yıl
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Tüketim talebinin yapıldığı aşama
        /// </summary>
        public ENUMBHBirimAsama asama = ENUMBHBirimAsama.URETIM;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin kurumsal kodu
        /// </summary>
        public string kur = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin fonksiyonel kodu
        /// </summary>
        public string fon = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin finans kodu
        /// </summary>
        public string fin = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin ekonomik kodu
        /// </summary>
        public string eko = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin alt faaliyet kodu
        /// </summary>
        public string peb = string.Empty;

        /// <summary>
        /// Talebi yapan birimin kodu
        /// </summary>
        public string kurumsalYapiKod = string.Empty;

        /// <summary>
        /// Talep yapılan tüketimin tipi
        /// </summary>
        public TuketimTip tuketimTip = null;

        /// <summary>
        /// Tüketim tipinden ne kadar tüketimin planlandığı
        /// </summary>
        public int miktar = 0;

        /// <summary>
        /// Tüketim tipinin kullanıcı tarafından girilen birim fiyatı
        /// </summary>
        public double ortalamaFiyat = 0;
    }

    /// <summary>
    /// Yolluk türlerini tutan liste yapı
    /// </summary>
    public enum ENUMParYollukTip
    {
        /// <summary>
        /// Yurt İçi Geçici Görev Yolluğu
        /// </summary>
        YIGGY = 1,

        /// <summary>
        /// Yurt Dışı Geçici Görev Yolluğu
        /// </summary>
        YDGGY = 2,

        /// <summary>
        /// Yurt İçi Sürekli Görev Yolluğu
        /// </summary>
        YISGY = 3,

        /// <summary>
        /// Yurt Dışı Sürekli Görev Yolluğu
        /// </summary>
        YDSGY = 4,

        /// <summary>
        /// Yol tazminatı
        /// </summary>
        YOLTAZMINATI = 5,

        /// <summary>
        /// İlk değer atamalarında kullanılacak, yolluk tipinin boş bırakıldığı bilgisini tutan kod
        /// </summary>
        TANIMSIZ = 99,
    }

    /// <summary>
    /// Bütçe hazırlık esnasında yolluk taleplerini (türleri itibariyle) ve ilgili tertibi tutan dizi
    /// </summary>
    [Serializable()]
    public class ParYolluk
    {
        /// <summary>
        /// Tüketim talebi yapılan yıl
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Tüketim talebinin yapıldığı aşama
        /// </summary>
        public ENUMBHBirimAsama asama = ENUMBHBirimAsama.URETIM;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin kurumsal kodu
        /// </summary>
        public string kur = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin fonksiyonel kodu
        /// </summary>
        public string fon = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin finans kodu
        /// </summary>
        public string fin = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin ekonomik kodu
        /// </summary>
        public string eko = string.Empty;

        /// <summary>
        /// Talebin ilişkili olduğu tertibin alt faaliyet kodu
        /// </summary>
        public string peb = string.Empty;

        /// <summary>
        /// Talebi yapan birimin kodu
        /// </summary>
        public string kurumsalYapiKod = string.Empty;

        /// <summary>
        /// Birim ödenek talebinin yolluk tipi
        /// </summary>
        public ENUMParYollukTip tip = ENUMParYollukTip.TANIMSIZ;

        /// <summary>
        /// Birim ödenek talebinin yolluk alt tipi
        /// </summary>
        public int altTip;

        /// <summary>
        /// Planlanan geçici görev sayısı.
        /// </summary>
        public int gorevSayisi;

        /// <summary>
        /// Geçici görevler için planlanan toplam gün sayısı
        /// </summary>
        public int toplamGunSayisi;

        /// <summary>
        /// Planlanan atama sayısı (Sürekli görevlerde kullanılır.)
        /// </summary>
        public int atamaSayisi;

        /// <summary>
        /// Yol tazminatında kullanılır.
        /// </summary>
        public int dosyaSayisi;

        /// <summary>
        /// Verilen parametrelere ve ortalama fiyatlara göre hesaplanan ödenek tutarı
        /// </summary>
        public double tutar;
    }

    /// <summary>
    /// Yolluk giderlerinde kullanılacak ortalama fiyat bilgilerini tutan sınıf.
    /// </summary>
    [Serializable()]
    public class ParYollukOrtalamaFiyat
    {
        /// <summary>
        /// Ortalama fiyat bilgisi tutulan yolluk tipinin yılı
        /// </summary>
        public int yil = 0;

        /// <summary>
        /// Ortalama fiyat bilgisi tutulan yolluk tipi
        /// </summary>
        public ENUMParYollukTip tip = ENUMParYollukTip.TANIMSIZ;

        /// <summary>
        /// Ortalama fiyat bilgisi tutulan yolluğun alt tipi
        /// </summary>
        public int altTip = 0;

        /// <summary>
        /// Yolluk ortalama fiyatları girilirken, her tip için alt tip girilebilir. Girilen alt tipin adı.
        /// </summary>
        public string altTipAd = string.Empty;

        /// <summary>
        /// Görevlendirme için kullanılacak olan ortalama fiyat.
        /// </summary>
        public double ortFiyatGorev = 0.0;

        /// <summary>
        /// Atamalar için kullanılacak ortalama fiyat.
        /// </summary>
        public double ortFiyatAtama = 0.0;

        /// <summary>
        /// Yol tazminatlarında, dosya başına kullanılacak ortalama fiyat
        /// </summary>
        public double ortFiyatDosya = 0.0;

        /// <summary>
        /// Geçici görev yolluklarında kullanılacak gün sayısı için ortalama fiyat.
        /// </summary>
        public double ortFiyatToplamGun = 0.0;
    }

    /// <summary>
    /// MaliDurumBeklenti: 
    /// </summary>
    [System.Serializable()]
    public class MaliDurumBeklenti
    {
        /// <summary>
        /// Yıl: Yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Faaliyet Kod: Faaliyeti giriniz
        /// </summary>
        public string faaliyetKod { get; set; }

        /// <summary>
        /// Bütçe Uygulama Sonucu: Ocak - Haziran Dönemi Bütçe Uygulama Sonuçları
        /// </summary>
        public string butceUygulamaSonucu { get; set; }

        /// <summary>
        /// Yürütülen Faaliyetler: Ocak - Haziran Döneminde Yürütülen Faaliyetler
        /// </summary>
        public string yurutulenFaaliyetler { get; set; }

        /// <summary>
        /// Beklentiler ve Hedefler: Temmuz - Aralık Dönemine İlişkin Beklentiler ve Hedefler
        /// </summary>
        public string beklentilerveHedefler { get; set; }

        /// <summary>
        /// Yürütülecek Faaliyetler: Temmuz - Aralık Döneminde Yürütülecek Faaliyetler
        /// </summary>
        public string yurutulecekFaaliyetler { get; set; }
    }


    public enum ENUMTarihceKod : int
    {
        KAYDEDILDI = 1,
        ONAYLANDI = 20,
        ONAYKALDIRILDI = 100,
        KAPATILDI = 30,
    }


    [Serializable()]
    public class ProjeTeklif
    {
        public bool sadeceYATProje = false;
        public string kod = "";

        public BirimYapi sorumluBirim = new BirimYapi();

        public string ad = "";
        public string sektorKod = "";
        public int baslamaYil = 0;
        public int bitisYil = 0;

        public string baslamaBitisYil { get { return this.baslamaYil + " - " + this.bitisYil; } }

        public string sektorAd = "";
        public string spHedefKod = "";

        public ENUMProjeDurum durum { get; set; }
        public int yatProjeYil { get; set; }
        public int yatAltProjeKod { get; set; }
        public int yatProjeKod { get; set; }

        public string projeNo
        {
            get
            {
                if (yatProjeKod > 0 && yatAltProjeKod > 0 && yatProjeYil > 0)
                    return yatProjeYil.ToString() + "." + yatProjeKod.ToString() + "." + yatAltProjeKod.ToString();
                else
                    return "";
            }
            set
            {
                yatProjeKod = 0;
                yatAltProjeKod = 0;
                yatProjeYil = 0;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string[] kodlar = value.Split('.');
                    if (kodlar.Length == 3)
                    {
                        yatProjeYil = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(kodlar, 0, ""), 0);
                        yatProjeKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(kodlar, 1, ""), 0);
                        yatAltProjeKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(kodlar, 2, ""), 0);
                    }
                }
            }
        }


        public string projeAd
        {
            get
            {
                if (string.IsNullOrWhiteSpace(dptProjeNo)) return "";
                return dptProjeNo + "- " + dptProjeAd + " (" + dptAltProjeAd + ")";
            }
        }

        public bool yatProjeli { get; set; }

        public string dptProjeNo;
        public string dptProjeAd;
        public string dptAltProjeAd;

        public ProjeAciklama[] aciklamalar = null;


        public string genelArama = "";
        public int yilAra = 0;
    }

    [Serializable()]
    public class ProjeBirim
    {
        public string projeKod = "";
        public string birimKod = "";
        public string birimAd = "";
    }

    [Serializable()]
    public class ProjeYurutucu
    {
        public string projeKod = "";
        public string tcKimlikNo = "";
        public string adSoyad = "";
        public string kurum = "";
        public string unvan = "";
        public string tel = "";
        public string eposta = "";
        public string projedekiRolu = "";
    }

    [Serializable()]
    public class ProjeFaaliyet
    {
        public string kod = "";
        public string projeKod = "";
        public string ad = "";
        public int yil = 0;
        public TNSDateTime basTarih = new TNSDateTime();
        public TNSDateTime bitTarih = new TNSDateTime();
        public string anaFaaliyet = "";
        public string anaFaaliyetAd = "";
        public BirimYapi sorumluBirim = new BirimYapi();
        public string izleyenKod = "";
        public string izleyenAd = "";
        public double tahminiMaliyet = 0;
        public string projeAd = "";
        public bool kullaniciFaaliyetleri = false;
        public string aramaAlani = "";
    }

    [Serializable()]
    public class ProjeFaaliyetGosterge
    {
        public string kod = "";
        public string faaliyetKod = "";
        public string faaliyetAd = "";
        public int yil = 0;
        public string gosterge = "";
        public string birim = "";
        public double planlananDeger = 0;
        public double gerceklesenDeger = 0;

        public int ay { get; set; }
    }

    [Serializable()]
    public class ProjeFaaliyetGerceklesme
    {
        public string kod = "";
        public double gerceklesenDeger = 0;
        public string aciklama { get; set; }

        public TNSDateTime kayitTarihi = new TNSDateTime();
        public TNSCollection gostergeler = new TNSCollection();
        public ProjeFaaliyet faaliyet = null;

        public int yil { get; set; }
        public int ay { get; set; }
        public string kullaniciKod { get; set; }
        public ObjectArray dosyalar { get; set; } = null;

        public ProjeFaaliyetGosterge GostergeBul(ProjeFaaliyetGosterge gelenGosterge)
        {
            foreach (ProjeFaaliyetGosterge item in gostergeler)
            {
                if (item.kod == gelenGosterge.kod)
                    return item;
            }
            return null;
        }
    }


    [Serializable()]
    public class ProjeUrun
    {
        public string projeKod = "";
        public string urunKod = "";
        public string urunAd = "";
        public string urunGrupAd = "";
    }

    [Serializable()]
    public class ProjeFinansman
    {
        public string kod = "";
        public string projeKod = "";
        public int yil = 0;
        public string ad = "";
        public int kaynak = 0;
        public string kaynakAd = "";
        public decimal tutar = 0;
    }

    //[Serializable()]
    //public class ProjeBirimIlgili
    //{
    //    public string projeKod = "";
    //    public string birimKod = "";
    //    public int muhasebeKod = 0;
    //    public string kurKod = "";
    //    public string ad = "";
    //}


    [Serializable()]
    public class ProjeGirdi
    {
        public string projeKod = "";
        public string kod = "";
        public ENUMProjeGirdiTur tur = 0;
        public string girdi = "";
        public string birim = "";
        public double miktar = 0;
        public decimal birimMaliyet = 0;
        public decimal toplamMaliyet = 0;
    }

    [Serializable()]
    public class ProjeSorun
    {
        public string projeKod = "";
        public string kod = "";
        public TNSDateTime tarih = new TNSDateTime();
        public string sorun = "";
        public string cozumOnerisi = "";
    }

    public enum ENUMProjeDurum
    {
        TANIMSIZ = 0,
        DEVAMEDIYOR = 1,
        TAMAMLANDI = 2,
        IPTAL = 99,
    }

    public enum ENUMProjeGirdiTur
    {
        TANIMSIZ = 0,
        URETIM = 1,
        EKIPMAN = 2,
        INSAAT = 3,
        TASIT = 4,
        YOLLUK = 5,
        DIGER = 6,
    }


    [Serializable()]
    public class Faaliyet
    {
        public string kod = "";
        public string ad = "";
    }

    [Serializable()]
    public class FinansmanKaynak
    {
        public int kod = 0;
        public string ad = "";
    }

    [Serializable()]
    public class Urun
    {
        public string kod = "";
        public string ad = "";
        public UrunGrup grup = new UrunGrup();
    }

    [Serializable()]
    public class UrunGrup
    {
        public string kod = "";
        public string ad = "";
    }


    [Serializable()]
    public class ProjeAciklama
    {
        public string projeKod = "";
        public ProjeAciklamaTur tur = new ProjeAciklamaTur();
        public string aciklama = "";
    }

    [Serializable()]
    public class SPAmac
    {
        public string amacKodAd { get { return amacKod + "- " + amac; } }
        public string amac = "";
        public string amacKod = "";
        public TNSCollection hedefler = new TNSCollection();

        public SPAmac(string amacKod, string amac)
        {
            this.amacKod = amacKod;
            this.amac = amac;
        }

        public void HedefEkle(SPHedef item)
        {
            this.hedefler.Add(item);
        }
    }

    [Serializable()]
    public class SPHedef
    {
        public string hedefKodAd { get { return hedefKod + "- " + hedefAd; } }
        public string hedefAd = "";
        public string hedefKod = "";

        public SPHedef(string hedefKod, string hedefAd)
        {
            this.hedefKod = hedefKod;
            this.hedefAd = hedefAd;
        }
    }

    [Serializable()]
    public class ProjeAciklamaTur
    {
        public string kod = "";
        public string ad = "";
        public ENUMProjeAciklamaTur tur = 0;
        public int siraNo = 0;
    }

    public enum ENUMProjeAciklamaTur
    {
        METIN = 0,
        METINCOKSATIR = 1,
    }

}

