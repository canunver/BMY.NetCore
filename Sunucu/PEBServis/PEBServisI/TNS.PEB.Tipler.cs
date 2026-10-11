using System;
using System.Collections.Generic;
using System.Text;
using TNS.BKD;
using System.Collections;

namespace TNS.PEB
{
    public enum ENUMDonemler : int
    {
        Ay = 1,
        UcAy = 2,
        AltiAy = 3,
        Yil = 4
    }


    public enum ENUMSTRHEDEFAciklama : int
    {
        //: Hedefin gerçekleşmesini etkileyebilecek en fazla beş temel riske yer verilir. 
        Riskler = 1,

        //: Hedeflerin nasıl gerçekleştirileceğine yönelik en fazla üç stratejiye yer verilir. 
        Stratejiler = 2,

        //: Durum analizi çalışmaları sonucunda elde edilen ve hedeflere gerekçe oluşturacak bulgulardan en fazla 5 tanesine maddeler halinde yer verilir. 
        Tespitler = 3,

        //: Üst politika belgeleri analizinden gelen ihtiyaçlar öncelikli olmak üzere tespit edilen sorun alanlarına ilişkin yapılması gerekenlere en fazla 5 madde olmak üzere yer verilir. Burada yer verilen ihtiyaçlar hedefin gerekçesini oluşturur.
        Ihtiyaclar = 4,

        //: Hedefin program bağını tutar
        ProgramBagi = 5
    }

    public enum ENUMAsamaKontrolStatu
    {
        Baslanmadi = 0,
        DevamEdiyor = 1,
        Revizyonda = 2,
        Onayda = 3,
        UstOnayda = 4,
        Tamamlandi = 5
    }

    public enum ENUMAsamaKontrolIslemTur
    {
        Kaydet = 1,
        BirimOnay = 2,
        BirimRed = 3,
        UstBirimOnay = 4,
        UstBirimRed = 5
    }

    /// <summary>
    /// Stratejik plan tanımlarının sırasını tutan enumeration
    /// </summary>
    public enum ENUMStratejikPlan : int
    {
        /// <summary>
        /// Misyon tanımı
        /// </summary>
        Misyon = 1,

        /// <summary>
        /// Vizyon tanımı
        /// </summary>
        Vizyon = 2,

        /// <summary>
        /// Tema tanımı
        /// </summary>
        Tema = 3,

        /// <summary>
        /// Stratejik amaç tanımı
        /// </summary>
        StratejikAmac = 4,

        /// <summary>
        /// Stratejik hedef tanımı
        /// </summary>
        StratejikHedef = 5
    }

    public enum ENUMSurec
    {
        SpHazirlik = 1,
        MevcutDurumAnalizi = 2,
        StratejikModelleme = 3,
        PerformansProgrami = 4
    }

    public enum EnumSPGenelHavuzSurec
    {
        Tema = 3,
        Amac = 3,
        Hedef = 3,
        Faaliyet = 3,
        Hizmet = 2,
        SorunAlan = 2,
        UstBelge = 2,
        IsGorev = 3
    }

    public enum EnumSPGenelHavuzTur
    {
        Tema = 1,
        Amac = 2,
        Hedef = 3,
        Faaliyet = 4,
        Hizmet = 6,
        SorunAlan = 7,
        UstBelge = 8,
        IsGorev = 9
    }

    public enum EnumSPGenelHavuzIliski
    {
        Tema = 0,
        Amac = 1,
        Hedef = 2,
        Faaliyet = 7,
        Hizmet = 0,
        SorunAlan = 1,
        UstBelge = 0,
        IsGorev = 4
    }

    public enum EnumSPGenelHavuzAltIliski
    {
        Tema = 7,
        Amac = 3,
        Hedef = 0,
        Faaliyet = 9,
        Hizmet = 0,
        SorunAlan = 4,
        UstBelge = 0,
        IsGorev = 0
    }

    public enum EnumSPGenelHavuzIliskiCoklu
    {
        Tema = 0,
        Amac = 0,
        Hedef = 0,
        Faaliyet = 1,
        Hizmet = 0,
        SorunAlan = 0,
        UstBelge = 0,
        IsGorev = 0
    }

    public enum EnumSPGenelHavuzDurum
    {
        Pasif = 0,
        Aktif = 1,
        Hepsi = 2
    }

    public enum ENUMGzftTur
    {
        GucluYonler = 1,
        ZayifYonler = 2,
        Firsatlar = 3,
        Tehditler = 4
    }

    public enum ENUMPestTur
    {
        PolitikYasal = 1,
        Ekonomik = 2,
        SosyalKulturel = 3,
        Teknolojik = 4
    }

    public enum ENUMKurumsalAnalizTur
    {
        OrgutselYapi = 1,
        InsanKaynaklari = 2,
        MaliKaynaklar = 3,
        TeknolojikDuzey = 4,
        IstatistikiVeriler = 5
    }

    public enum ENUMPaydasAnalizTur
    {
        IcPaydas = 1,
        DisPaydas = 2,
        Yararlanici = 3
    }

    public enum ENUMMetrikTuru
    {
        Belirsiz = 0,
        Elle = 1,
        Formul = 2,
        Verisistemi = 3
    }

    public enum ENUMGostergeTuru
    {
        Belirsiz = 0,
        Elle = 1,
        Formul = 2,
        VeriSistemi = 3
    }

    /// <summary>
    /// ENUMTekrarSikligi İş/Görev sayfasında kullanılmak üzere yapıldı.: 
    /// </summary>
    public enum ENUMTekrarSikligi
    {
        BirKez = 1,
        Gunluk = 2,
        Haftalik = 3,
        Aylik = 4,
        UcAylik = 5,
        AltiAylik = 6,
        Yillik = 7
    }

    /// <summary>
    /// LPUretim: 
    /// </summary>
    [System.Serializable()]
    public class LPUretim
    {
        /// <summary>
        /// Yıl: Yılı giriniz
        /// </summary>
        public int yil;

        /// <summary>
        /// Sira No: Üretim numarası giriniz
        /// </summary>
        public string siraNo;

        /// <summary>
        /// Birim: Birim
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Birim: Birim
        /// </summary>
        public int orgKod;

        /// <summary>
        /// Org Tür: Org tür
        /// </summary>
        public string orgTur;


        /// <summary>
        /// cozumYontem
        /// </summary>
        public int cozumYontem;

        /// <summary>
        /// Durum: Durum
        /// </summary>
        public int durum = -1;

        /// <summary>
        /// Bilgi Turu: Bilgi turu
        /// </summary>
        public STRGosterge[] degiskenler = null;
        public string kopruKod;

        private string DosyaAd(string ek)
        {
            string yol = TNS.DEG.Arac.DegiskenDegerBul(0, "/PerformansLP");
            return yol + "/" + yil.ToString() + "_" + siraNo.ToString() + ek + ".xml";
        }

        public string ModelYol()
        {
            return TNS.DEG.Arac.DegiskenDegerBul(0, "/PerformansLP");
        }

        public string ModelDosyaAd()
        {
            return DosyaAd("M");
        }

        public string SonucDosyaAd()
        {
            return DosyaAd("S");
        }

        public string YontemAd()
        {
            if (cozumYontem == 1)
                return "VRS-IO";
            else if (cozumYontem == 2)
                return "CRS-IO";
            else if (cozumYontem == 3)
                return "VRS-OO";
            else
                return "CRS-OO";
        }
    }

    /// <summary>
    /// Dil sözlük tanımını tutan sınıf
    /// </summary>
    [Serializable()]
    public class Dil
    {
        /// <summary>
        /// Dilin programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Dilin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Parametresiz Dil nesnesi oluşturucu yordam
        /// </summary>
        public Dil()
        {
            kod = 0;
            ad = string.Empty;
        }

        /// <summary>
        /// Kod ve ad parametreleri alan Dil nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="kod">Dilin programatik kodu</param>
        /// <param name="ad">Dilin adı</param>
        public Dil(int kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
        }
    }

    /// <summary>
    /// Stratejik planın ilk basamağı olan misyon bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Misyon
    {
        /// <summary>
        /// Vizyona sahip kurum
        /// </summary>
        public string kurumKod { get; set; }

        /// <summary>
        /// Stratejik plan döneminin başlangıç yılı
        /// </summary>
        public int donemBYil;

        /// <summary>
        /// Misyonun dil bilgisini tutan nesne
        /// </summary>
        public Dil dil;

        /// <summary>
        /// Misyonun tanımı
        /// </summary>
        public string misyon;

        /// <summary>
        /// Misyonun açıklaması
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Misyona eklenecek dosya varsa adı
        /// </summary>
        public string dosya;

        /// <summary>
        /// Misyonun e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin stratejik plan entegrasyonunda misyon için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        /// <summary>
        /// Strajik dönemin ilk yılı
        /// </summary>
        public int aralikBasiYil;

        /// <summary>
        /// Strajik dönemin son yılı
        /// </summary>
        public int aralikSonuYil;

        /// <summary>
        /// Parametresiz Misyon nesnesi oluşturucu yordam
        /// </summary>
        public Misyon()
        {
            donemBYil = 0;
            dil = new Dil();
            dil.kod = 1;
            misyon = string.Empty;
            aciklama = string.Empty;
            dosya = string.Empty;
            ebutceKod = string.Empty;
        }

        public string Ad
        {
            get
            {
                return string.Format("{0}-{1} Dönemi Stratejik Planı{2}", aralikBasiYil, aralikSonuYil, Versiyon);
            }
        }

        public string Versiyon
        {
            get
            {
                if (aralikBasiYil == donemBYil) return "";
                return " (" + donemBYil + " Yılı Sürümü)";
            }
        }

        public int izlemeYil = -1;

        public string kurumAd { get; set; }

        public int YilBul(int ekle)
        {
            return aralikBasiYil + ekle;
        }
    }

    /// <summary>
    /// Stratejik planın ikinci basamağı olan vizyon bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Vizyon
    {
        /// <summary>
        /// Stratejik plan döneminin başlangıç yılı
        /// </summary>
        public int donemBYil;

        /// <summary>
        /// Vizyonun dil bilgisini tutan nesne
        /// </summary>
        public Dil dil;

        /// <summary>
        /// Vizyonun tanımı
        /// </summary>
        public string vizyon;

        /// <summary>
        /// Vizyonun açıklaması
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Vizyona eklenecek dosya varsa adı
        /// </summary>
        public string dosya;

        /// <summary>
        /// Vizyonun e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin stratejik plan entegrasyonunda vizyon için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        /// <summary>
        /// Parametresiz Vizyon nesnesi oluşturucu yordam
        /// </summary>
        public Vizyon()
        {
            donemBYil = 0;
            dil = new Dil();
            vizyon = string.Empty;
            aciklama = string.Empty;
            dosya = string.Empty;
            ebutceKod = string.Empty;
            kurumKod = string.Empty;
        }

        /// <summary>
        /// İlgili parametreleri alarak Vizyon nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="donemBYil">Stratejik plan dönem başlangıç yılı</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <param name="vizyon">Vizyon tanımı</param>
        /// <param name="aciklama">Vizyon açıklaması</param>
        /// <param name="dosya">Dosya adı</param>
        public Vizyon(int donemBYil, Dil dil, string vizyon, string aciklama, string dosya, string kurumKod)
        {
            this.donemBYil = donemBYil;
            this.dil = dil;
            this.vizyon = vizyon;
            this.aciklama = aciklama;
            this.dosya = dosya;
            this.kurumKod = kurumKod;
        }

        public string kurumKod { get; set; }
    }

    /// <summary>
    /// Stratejik planın üçüncü basamağı olan tema bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class Tema
    {
        /// <summary>
        /// Stratejik plan döneminin başlangıç yılı
        /// </summary>
        public int donemBYil;

        /// <summary>
        /// Temanın programatik kodu
        /// </summary>
        public int temaKod;

        /// <summary>
        /// Raporlamalarda ve ekranlarda gösterilmek üzere temanın özet/kısa bilgisi
        /// </summary>
        public string temaKisa;

        /// <summary>
        /// Temanın tanımı
        /// </summary>
        public string tema;

        /// <summary>
        /// Temanın açıklaması
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Temaya eklenecek dosya varsa adı
        /// </summary>
        public string dosya;

        /// <summary>
        /// Temanın dil bilgisini tutan nesne
        /// </summary>
        public Dil dil;

        /// <summary>
        /// Temanın e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin stratejik plan entegrasyonunda tema için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        /// <summary>
        /// Parametresiz Tema nesnesi oluşturucu yordam
        /// </summary>
        public Tema()
        {
            donemBYil = 0;
            temaKod = 0;
            temaKisa = string.Empty;
            tema = string.Empty;
            aciklama = string.Empty;
            dosya = string.Empty;
            dil = new Dil();
            ebutceKod = string.Empty;
        }

        /// <summary>
        /// İlgili parametreleri alarak Tema nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="donemBYil">Stratejik plan dönem başlangıç yılı</param>
        /// <param name="temaKod">Tema programatik kodu</param>
        /// <param name="temaKisa">Tema özet/kısa bilgisi</param>
        /// <param name="tema">Tema tanımı</param>
        /// <param name="aciklama">Tema açıklaması</param>
        /// <param name="dosya">Dosya adı</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        public Tema(int donemBYil, int temaKod, string temaKisa, string tema, string aciklama, string dosya, Dil dil, string kurumKod)
        {
            this.donemBYil = donemBYil;
            this.temaKod = temaKod;
            this.temaKisa = temaKisa;
            this.tema = tema;
            this.aciklama = aciklama;
            this.dosya = dosya;
            this.dil = dil;
            this.kurumKod = kurumKod;
        }

        public string kurumKod { get; set; }
    }

    /// <summary>
    /// Stratejik planın dördüncü basamağı olan stratejik amaç bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class StratejikAmac
    {
        /// <summary>
        /// Stratejik plan döneminin başlangıç yılı
        /// </summary>
        public int donemBYil;

        /// <summary>
        /// Stratejik amacın programatik kodu
        /// </summary>
        public int strAmacKod;

        /// <summary>
        /// Raporlamalarda ve ekranlarda gösterilmek üzere stratejik amacın özet/kısa bilgisi
        /// </summary>
        public string strAmacKisa;

        /// <summary>
        /// Stratejik amacın tanımı
        /// </summary>
        public string strAmac;

        /// <summary>
        /// Stratejik amacın açıklaması
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Stratejik amaca eklenecek dosya varsa adı
        /// </summary>
        public string dosya;

        /// <summary>
        /// Stratejik amacın bağlı olduğu tema bilgilerini tutan nesne
        /// </summary>
        public Tema tema;

        /// <summary>
        /// Stratejik amacın dil bilgisini tutan nesne
        /// </summary>
        public Dil dil;

        /// <summary>
        /// Stratejik amacın e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin stratejik plan entegrasyonunda stratejik amaç için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        /// <summary>
        /// Parametresiz Stratejik Amaç nesnesi oluşturucu yordam
        /// </summary>
        public StratejikAmac()
        {
            donemBYil = 0;
            strAmacKod = 0;
            strAmacKisa = string.Empty;
            strAmac = string.Empty;
            aciklama = string.Empty;
            dosya = string.Empty;
            tema = new Tema();
            dil = new Dil();
            ebutceKod = string.Empty;
        }

        /// <summary>
        /// İlgili parametreleri alarak Stratejik Amaç nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="donemBYil">Stratejik plan dönem başlangıç yılı</param>
        /// <param name="strAmacKod">Stratejik amaç programatik kodu</param>
        /// <param name="strAmacKisa">Stratejik amaç özet/kısa bilgisi</param>
        /// <param name="strAmac">Stratejik amaç tanımı</param>
        /// <param name="aciklama">Stratejik amaç açıklaması</param>
        /// <param name="dosya">Dosya adı</param>
        /// <param name="tema">Tema bilgilerini tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        public StratejikAmac(int donemBYil, int strAmacKod, string strAmacKisa, string strAmac, string aciklama, string dosya, Tema tema, Dil dil, string kurumKod)
        {
            this.donemBYil = donemBYil;
            this.strAmacKod = strAmacKod;
            this.strAmacKisa = strAmacKisa;
            this.strAmac = strAmac;
            this.aciklama = aciklama;
            this.dosya = dosya;
            this.tema = tema;
            this.dil = dil;
            this.kurumKod = kurumKod;
        }

        public string kurumKod { get; set; }
    }

    /// <summary>
    /// Stratejik planın beşinci basamağı olan stratejik hedef bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class StrUstPolitikaBelgesi
    {
        public int ustBelgeKod { get; set; }
        public string ustBelgeAd { get; set; }
        public string kodAd
        {
            get
            {
                return ustBelgeKod.ToString() + "-" + ustBelgeAd;
            }
            set
            {
                string a1 = "";
                string a2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(value, "-", ref a1, ref a2);
                ustBelgeKod = OrtakFonksiyonlar.ConvertToInt(a1, 0);
                ustBelgeAd = a2;
            }
        }

        public string referans { get; set; }
        public string aciklama { get; set; }
    }

    /// <summary>
    /// Stratejik planın beşinci basamağı olan stratejik hedef bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class StratejikHedef
    {
        /// <summary>
        /// Stratejik plan döneminin başlangıç yılı
        /// </summary>
        public int donemBYil;

        /// <summary>
        /// Stratejik hedefin programatik kodu
        /// </summary>
        public int strHedefKod;

        /// <summary>
        /// Raporlamalarda ve ekranlarda gösterilmek üzere stratejik hedefin özet/kısa bilgisi
        /// </summary>
        public string strHedefKisa;

        /// <summary>
        /// Stratejik hedefin tanımı
        /// </summary>
        public string strHedef;

        /// <summary>
        /// Stratejik hedefin açıklaması
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Stratejik hedefe eklenecek dosya varsa adı
        /// </summary>
        public string dosya;

        /// <summary>
        /// Stratejik hedefin bağlı olduğu stratejik amaç bilgilerini tutan nesne
        /// </summary>
        public StratejikAmac strAmac;

        /// <summary>
        /// Stratejik hedefin dil bilgisini tutan nesne
        /// </summary>
        public Dil dil;

        /// <summary>
        /// Stratejik hedefin e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin stratejik plan entegrasyonunda stratejik hedef için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        public TNSCollection birimler;
        public TNSCollection riskler;
        public TNSCollection stratejiler;
        public TNSCollection tespitler;
        public TNSCollection ihtiyaclar;
        public TNSCollection programBagi;
        public TNSCollection ustPolitikaBaglari;

        /// <summary>
        /// Parametresiz Stratejik Hedef nesnesi oluşturucu yordam
        /// </summary>
        public StratejikHedef()
        {
            birimler = new TNSCollection();
            riskler = new TNSCollection();
            stratejiler = new TNSCollection();
            tespitler = new TNSCollection();
            ihtiyaclar = new TNSCollection();
            programBagi = new TNSCollection();
            ustPolitikaBaglari = new TNSCollection();

            donemBYil = 0;
            strHedefKod = 0;
            strHedefKisa = string.Empty;
            strHedef = string.Empty;
            aciklama = string.Empty;
            dosya = string.Empty;
            strAmac = new StratejikAmac();
            dil = new Dil();
            ebutceKod = string.Empty;
        }

        /// <summary>
        /// İlgili parametreleri alarak Stratejik Hedef nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="donemBYil">Stratejik plan dönem başlangıç yılı</param>
        /// <param name="strHedefKod">Stratejik hedef programatik kodu</param>
        /// <param name="strHedefKisa">Stratejik hedef özet/kısa bilgisi</param>
        /// <param name="strHedef">Stratejik hedef tanımı</param>
        /// <param name="aciklama">Stratejik hedef açıklaması</param>
        /// <param name="dosya">Dosya adı</param>
        /// <param name="strAmac">Stratejik amaç bilgilerini tutan nesne</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        public StratejikHedef(int donemBYil, int strHedefKod, string strHedefKisa, string strHedef, string aciklama, string dosya, StratejikAmac strAmac, Dil dil)
        {
            this.donemBYil = donemBYil;
            this.strHedefKod = strHedefKod;
            this.strHedefKisa = strHedefKisa;
            this.strHedef = strHedef;
            this.aciklama = aciklama;
            this.dosya = dosya;
            this.strAmac = strAmac;
            this.dil = dil;
            this.ebutceKodKaydet = false;
        }
        public string kurumKod { get; set; }
        public bool ebutceKodKaydet = false;

        public int aralikBasYil { get; set; }
        public int aralikSonYil { get; set; }
    }

    /// <summary>
    /// Misyon, Vizyon, Tema, Stratejik Amaç ve Stratejik Hedef ortak bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class StratejikYazi
    {
        /// <summary>
        /// Stratejik plan döneminin başlangıç yılı
        /// </summary>
        public int donemBYil;

        /// <summary>
        /// ENUMStratejikPlan listesinden bir tür
        /// </summary>
        public int kodTur;

        /// <summary>
        /// Seçilen türe ait programatik kod
        /// </summary>
        public int kod;

        /// <summary>
        /// Seçilen türe ait dil bilgisini tutan nesne
        /// </summary>
        public Dil dil;

        /// <summary>
        /// Seçilen türün tanımı
        /// </summary>
        public string kisaYazi;

        /// <summary>
        /// Seçilen türün açıklaması
        /// </summary>
        public string uzunYazi;

        /// <summary>
        /// Seçilen türe ait eklenecek dosya varsa adı
        /// </summary>
        public string dosya;

        /// <summary>
        /// Seçilen türün e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin stratejik plan entegrasyonunda kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        /// <summary>
        /// Verilen Misyon nesnesinden Stratejik Yazı nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="misyon">Misyon bilgilerini tutan sınıf</param>
        public StratejikYazi(Misyon misyon)
        {
            kurumKod = misyon.kurumKod;
            donemBYil = misyon.donemBYil;
            kodTur = (int)ENUMStratejikPlan.Misyon;
            kod = 1;
            dil = misyon.dil;
            kisaYazi = misyon.misyon;
            uzunYazi = misyon.aciklama;
            dosya = misyon.dosya;
            ebutceKod = misyon.ebutceKod;
        }

        /// <summary>
        /// Verilen Vizyon nesnesinden Stratejik Yazı nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="vizyon">Vizyon bilgilerini tutan sınıf</param>
        public StratejikYazi(Vizyon vizyon)
        {
            kurumKod = vizyon.kurumKod;
            donemBYil = vizyon.donemBYil;
            kodTur = (int)ENUMStratejikPlan.Vizyon;
            kod = 1;
            dil = vizyon.dil;
            kisaYazi = vizyon.vizyon;
            uzunYazi = vizyon.aciklama;
            dosya = vizyon.dosya;
            ebutceKod = vizyon.ebutceKod;
        }

        /// <summary>
        /// Verilen Tema nesnesinden Stratejik Yazı nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="tema">Tema bilgilerini tutan sınıf</param>
        public StratejikYazi(Tema tema)
        {
            donemBYil = tema.donemBYil;
            kodTur = (int)ENUMStratejikPlan.Tema;
            kod = tema.temaKod;
            dil = tema.dil;
            kisaYazi = tema.tema;
            uzunYazi = tema.aciklama;
            dosya = tema.dosya;
            ebutceKod = tema.ebutceKod;
            kurumKod = tema.kurumKod;
        }

        /// <summary>
        /// Verilen Stratejik Amaç nesnesinden Stratejik Yazı nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="amac">Stratejik amaç bilgilerini tutan sınıf</param>
        public StratejikYazi(StratejikAmac amac)
        {
            donemBYil = amac.donemBYil;
            kodTur = (int)ENUMStratejikPlan.StratejikAmac;
            kod = amac.strAmacKod;
            dil = amac.dil;
            kisaYazi = amac.strAmac;
            uzunYazi = amac.aciklama;
            dosya = amac.dosya;
            ebutceKod = amac.ebutceKod;
            kurumKod = amac.kurumKod;
        }

        /// <summary>
        /// Verilen Stratejik Hedef nesnesinden Stratejik Yazı nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="hedef">Stratejik hedef bilgilerini tutan sınıf</param>
        public StratejikYazi(StratejikHedef hedef)
        {
            donemBYil = hedef.donemBYil;
            kodTur = (int)ENUMStratejikPlan.StratejikHedef;
            kod = hedef.strHedefKod;
            dil = hedef.dil;
            kisaYazi = hedef.strHedef;
            uzunYazi = hedef.aciklama;
            dosya = hedef.dosya;
            ebutceKod = hedef.ebutceKod;
            kurumKod = hedef.kurumKod;
        }

        public string kurumKod { get; set; }
    }

    /// <summary>
    /// Analitik bütçe kodlarının sırasını tutan enumeration
    /// Ödenek ile ilgili işlemlerde ve raporlarda kullanılıyor.
    /// </summary>
    public enum ENUMAbsKodSinif : int
    {
        /// <summary>
        /// TERTIP kodunun sıra numarası
        /// </summary>
        TERTIP = 0,

        /// <summary>
        /// KURKOD kodunun sıra numarası
        /// </summary>
        KURKOD = 1,

        /// <summary>
        /// FONKOD kodunun sıra numarası
        /// </summary>
        FONKOD = 2,

        /// <summary>
        /// FINKOD kodunun sıra numarası
        /// </summary>
        FINKOD = 3,

        /// <summary>
        /// EKOKOD kodunun sıra numarası
        /// </summary>
        EKOKOD = 4
    }

    /// <summary>
    /// Faaliyet/proje sırasını tutan enumeration
    /// </summary>
    public enum ENUMFaaliyetProje : int
    {
        /// <summary>
        /// Faaliyete ait sıra numarası
        /// </summary>
        Faaliyet = 1,

        /// <summary>
        /// Projeye ait sıra numarası
        /// </summary>
        Proje = 2
    }

    /// <summary>
    /// Performans göstergesinin türünün sırasını tutan enumeration
    /// </summary>
    public enum ENUMGostergeTur : int
    {
        Tanimsiz = 0,
        /// <summary>
        /// Kalite türünün sıra numarası
        /// </summary>
        OranAC = 1,

        /// <summary>
        /// Miktar türünün sıra numarası
        /// </summary>
        OranABCB = 2
    }

    /// <summary>
    /// Performans göstergesinin kullanım şeklinin sırasını tutan enumeration
    /// </summary>
    public enum ENUMGostergeKullanimSekli : int
    {
        /// <summary>
        /// Hizmet kullanım şeklinin sıra numarası
        /// </summary>
        Hizmet = 1,

        /// <summary>
        /// OperasyonelPlan kullanım şeklinin sıra numarası
        /// </summary>
        OperasyonelPlan = 2
    }

    /// <summary>
    /// Performans göstergesinin seviyesinin sırasını tutan enumeration
    /// </summary>
    public enum ENUMGostergeSeviye : int
    {
        /// <summary>
        /// SeviyeI sıra numarası
        /// </summary>
        SeviyeI = 1,

        /// <summary>
        /// SeviyeII sıra numarası
        /// </summary>
        SeviyeII = 2,

        /// <summary>
        /// SeviyeIII sıra numarası
        /// </summary>
        SeviyeIII = 3,

        /// <summary>
        /// SeviyeIV sıra numarası
        /// </summary>
        SeviyeIV = 4
    }

    /// <summary>
    /// Performans göstergesinin ilerleme yönünün sırasını tutan enumeration
    /// </summary>
    public enum ENUMGostergeIlerlemeYonu : int
    {
        /// <summary>
        /// Artan ilerleme yönünün sıra numarası
        /// </summary>
        Tanimsiz = 0,

        /// <summary>
        /// Artan ilerleme yönünün sıra numarası
        /// </summary>
        Artan = 1,

        /// <summary>
        /// Azalan ilerleme yönünün sıra numarası
        /// </summary>
        Azalan = 2
    }

    /// <summary>
    /// Faaliyet/Proje bilgisinin türlerinin sırasını tutan enumeration
    /// </summary>
    public enum ENUMFaaliyetProjeTur : int
    {
        /// <summary>
        /// Merkez türünün sıra numarası
        /// </summary>
        Merkez = 1,

        /// <summary>
        /// Tasra türünün sıra numarası
        /// </summary>
        Tasra = 2,

        /// <summary>
        /// MerkezTasra türünün sıra numarası
        /// </summary>
        MerkezTasra = 3,

        /// <summary>
        /// DigerKurumIci türünün sıra numarası
        /// </summary>
        DigerKurumIci = 4,

        /// <summary>
        /// DigerKurumDisi türünün sıra numarası
        /// </summary>
        DigerKurumDisi = 5
    }

    /// <summary>
    /// Sabit maliyet bilgisinin türlerinin sırasını tutan enumeration
    /// </summary>
    public enum ENUMSabitMaliyetTur : int
    {
        /// <summary>
        /// Birim türünün sıra numarası
        /// </summary>
        Birim = 1,

        /// <summary>
        /// Merkez türünün sıra numarası
        /// </summary>
        Merkez = 2,

        /// <summary>
        /// Tasra türünün sıra numarası
        /// </summary>
        Tasra = 3,

        /// <summary>
        /// MerkezTasra türünün sıra numarası
        /// </summary>
        MerkezTasra = 4
    }

    /// <summary>
    /// Ödeneğin faaliyet/projeye mi yoksa sabit maliyete mi ait olduğunun sırasını tutan enumeration
    /// </summary>
    public enum ENUMOdenekTur : int
    {
        /// <summary>
        /// FaaliyetProje ödeneğinin sıra numarası
        /// </summary>
        FaaliyetProje = 0,

        /// <summary>
        /// SabitMaliyet ödeneğinin sıra numarası
        /// </summary>
        SabitMaliyet = 1
    }

    //public enum ENUMAsama : int
    //{
    //    BIRIMTEKLIFI = 1,
    //    SGBTEKLIFI = 2,
    //    KURUMTEKLIFI = 3,
    //    TASARI = 4,
    //    KANUN = 5,
    //    PROGRAMTEKLIFI = 6,
    //    PROGRAM = 7
    //}

    /// <summary>
    /// Ölçü Birim sözlük tanımını tutan sınıf
    /// </summary>
    [Serializable()]
    public class OlcuBirim
    {
        /// <summary>
        /// Ölçü biriminin programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Ölçü biriminin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Ölçü biriminin e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin performans bütçe entegrasyonunda ölçü birimi için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        /// <summary>
        /// Parametresiz OlcuBirim nesnesi oluşturucu yordam
        /// </summary>
        public OlcuBirim()
        {
            kod = 0;
            ad = string.Empty;
            ebutceKod = string.Empty;
        }

        /// <summary>
        /// Kod ve ad parametreleri alan OlcuBirim nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="kod">Ölçü biriminin programatik kodu</param>
        /// <param name="ad">Ölçü biriminin adı</param>
        public OlcuBirim(int kod, string ad)
        {
            this.kod = kod;
            this.ad = ad;
        }
    }

    /// <summary>
    /// Performans göstergesi sözlük tanımını tutan sınıf
    /// Raporlarda kullanılmak üzere kullanılan ödenek, stratejik hedef gibi alanları da içerir.
    /// </summary>
    [Serializable()]
    public class PerformansGosterge
    {
        /// <summary>
        /// Performans göstergesinin programatik kodu
        /// </summary>
        public int gostergeKod { get; set; }

        /// <summary>
        /// Performans göstergesinin adı
        /// </summary>
        public string gostergeAd { get; set; }

        /// <summary>
        /// Performans göstergesinin ölçü birim bilgisini tutan nesne
        /// </summary>
        public OlcuBirim olcuBirim { get; set; }
        public string olcuBirimAd { get { return olcuBirim.ad; } }

        /// <summary>
        /// Performans göstergesinin türü
        /// ENUMGostergeTur listesindeki alanlardan biri olmalıdır.
        /// </summary>
        public ENUMGostergeTur tur { get; set; }

        /// <summary>
        /// Performans göstergesinin kullanım şekli
        /// ENUMGostergeKullanimSekli listesindeki alanlardan biri olmalıdır.
        /// </summary>
        public int ondalikSayisi { get; set; }

        /// <summary>
        /// Performans göstergesinin seviyesi
        /// ENUMGostergeSeviye listesindeki alanlardan biri olmalıdır.
        /// </summary>
        public int seviye { get; set; }

        /// <summary>
        /// Performans göstergesinin ilerleme yönü
        /// ENUMGostergeIlerlemeYonu listesindeki alanlardan biri olmalıdır.
        /// </summary>
        public ENUMGostergeIlerlemeYonu ilerlemeYonu { get; set; }

        /// <summary>
        /// Performans göstergesinin e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin performans bütçe entegrasyonunda performans göstergesi için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod { get; set; }

        public string kurumKod { get; set; }

        public int kod { get { return gostergeKod; } set { gostergeKod = value; } }
        public string ad { get { return gostergeAd + "(" + olcuBirim.ad + ")"; } }
        /// <summary>
        /// Performans göstergesinin ödeneği
        /// Raporlarda kullanılan bir alandır.
        /// </summary>
        public double odenek { get; set; }

        /// <summary>
        /// Performans göstergesinin bağlı olduğu stratejik hedef
        /// Raporlarda kullanılan bir alandır.
        /// </summary>
        public StratejikHedef strHedef { get; set; }

        /// <summary>
        /// Parametresiz PerformansGosterge nesnesi oluşturucu yordam
        /// </summary>
        public PerformansGosterge()
        {
            gostergeKod = 0;
            gostergeAd = string.Empty;
            olcuBirim = new OlcuBirim();
            tur = 0;
            ondalikSayisi = 2;
            seviye = 0;
            ilerlemeYonu = 0;
            odenek = (double)0;
            strHedef = new StratejikHedef();
            ebutceKod = string.Empty;
            this.kurumKod = null;
        }

        /// <summary>
        /// İlgili parametreleri alarak PerformansGosterge nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="gostergeKod">Performans göstergesinin programatik kodu</param>
        /// <param name="gostergeAd">Performans göstergesinin adı</param>
        /// <param name="olcuBirim">Performans göstergesinin ölçü birim bilgisini tutan nesne</param>
        /// <param name="tur">Performans göstergesinin türü</param>
        /// <param name="strHedef">Performans göstergesinin bağlı olduğu stratejik hedef</param>
        public PerformansGosterge(int gostergeKod, string gostergeAd, OlcuBirim olcuBirim, ENUMGostergeTur tur, StratejikHedef strHedef)
        {
            this.gostergeKod = gostergeKod;
            this.gostergeAd = gostergeAd;
            this.olcuBirim = olcuBirim;
            this.tur = tur;
            this.strHedef = strHedef;
            this.kurumKod = null;
        }

        public bool adaGoreSirala = false;
    }

    /// <summary>
    /// Performans göstergesi için girilen hedefenen değerlerin üst bilgilerini tutan sınıf
    /// Hedeflenen değerlerin, hangi performans hedefine ve birime ait olduğu bilgilerini içerir.
    /// </summary>
    [Serializable()]
    public class PerformansGostergeHedeflenen
    {
        /// <summary>
        /// Hedeflenen değerlerin girildiği yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Performans hedefin biriminin kodu
        /// </summary>
        public string phKurKod;

        /// <summary>
        /// Performans hedefin biriminin adı
        /// </summary>
        public string phKurAd;

        /// <summary>
        /// Hedeflenen değerlerin ait olduğu birimin kodu
        /// </summary>
        public string ghKurKod;

        /// <summary>
        /// Hedeflenen değerlerin ait olduğu birimin adı
        /// </summary>
        public string ghKurAd;

        /// <summary>
        /// Performans hedefin programatik kodu
        /// </summary>
        public int perHedefKod;

        /// <summary>
        /// Performans hedefin adı
        /// </summary>
        public string perHedefAd;

        /// <summary>
        /// Hedeflenen değerlerin girildiği aşama
        /// </summary>
        public int asama;

        /// <summary>
        /// Hedeflenen değerlerin listesini tutan nesne
        /// </summary>
        public TNSCollection gostergeDegerler;

        /// <summary>
        /// Parametresiz PerformansGostergeHedeflenen nesnesi oluşturucu yordam
        /// </summary>
        public PerformansGostergeHedeflenen()
        {
            yil = 0;
            phKurKod = string.Empty;
            phKurAd = string.Empty;
            ghKurKod = string.Empty;
            ghKurAd = string.Empty;
            perHedefKod = 0;
            perHedefAd = string.Empty;
            asama = -1;
            gostergeDegerler = new TNSCollection();
        }

        public string perHedefGorKod { get; set; }
    }

    /// <summary>
    /// Performans göstergesi için girilen hedefenen değerleri tutan sınıf
    /// </summary>
    [Serializable()]
    public class PerformansGostergeHedeflenenDegerler
    {
        /// <summary>
        /// Hedeflenen değerlerin ait olduğu performans göstergesinin bilgilerini tutan nesne
        /// </summary>
        public PerformansGosterge gosterge;

        /// <summary>
        /// Hedeflenen değerler dizisi
        /// </summary>
        public double[] miktar;

        /// <summary>
        /// Hedeflenen değerlerin formdaki giriş sırası
        /// Bumko'nun aldığı bir raporda ilk sıradakiler toplanıp gönderiliyor.
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Hedeflenen değerler için girilen açıklama
        /// </summary>
        public string[] aciklama;

        /// <summary>
        /// Parametresiz PerformansGostergeHedeflenenDegerler nesnesi oluşturucu yordam
        /// </summary>
        public PerformansGostergeHedeflenenDegerler()
        {
            gosterge = new PerformansGosterge();
            miktar = new double[5];
            siraNo = 0;
            aciklama = new string[5];
        }

        public string phKurKod { get; set; }

        public int perHedefKod { get; set; }
    }

    /// <summary>
    /// Performans göstergesi için girilen gerçekleşen değerlerin üst bilgilerini tutan sınıf
    /// Gerçekleşen değerlerin, hangi performans hedefine ve birime ait olduğu bilgilerini içerir.
    /// </summary>
    [Serializable()]
    public class PerformansGostergeGerceklesen
    {
        /// <summary>
        /// Gerçekleşen değerlerin girildiği yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Performans hedefin biriminin kodu
        /// </summary>
        public string phKurKod;

        /// <summary>
        /// Performans hedefin biriminin adı
        /// </summary>
        public string phKurAd;

        /// <summary>
        /// Gerçekleşen değerlerin ait olduğu birimin kodu
        /// </summary>
        public string ggKurKod;

        /// <summary>
        /// Gerçekleşen değerlerin ait olduğu birimin adı
        /// </summary>
        public string ggKurAd;

        /// <summary>
        /// Performans hedefin programatik kodu
        /// </summary>
        public int perHedefKod;

        /// <summary>
        /// Performans hedefin adı
        /// </summary>
        public string perHedefAd;

        /// <summary>
        /// Gerçekleşen değerlerin girildiği ay/dönem
        /// </summary>
        public int ay;

        /// <summary>
        /// Gerçekleşen değerlerin listesini tutan nesne
        /// </summary>
        public TNSCollection degerler;

        /// <summary>
        /// Parametresiz PerformansGostergeGerceklesen nesnesi oluşturucu yordam
        /// </summary>
        public PerformansGostergeGerceklesen()
        {
            yil = 0;
            phKurKod = string.Empty;
            phKurAd = string.Empty;
            ggKurKod = string.Empty;
            ggKurAd = string.Empty;
            perHedefKod = 0;
            perHedefAd = string.Empty;
            ay = 0;
            degerler = new TNSCollection();
        }
    }

    /// <summary>
    /// Gösterge gerçekleşmelerini dizi içinde tutmak için kullanılan yapı.
    /// </summary>
    [Serializable()]
    public class GGSade
    {
        /// <summary>
        /// Gerçekleşen değerlerin girildiği ay/dönem
        /// </summary>
        public int ay;

        /// <summary>
        /// Gerçekleşen değer
        /// </summary>
        public double miktar;

        /// <summary>
        /// Gerçekleşen değerlere ait kaynak açıklaması
        /// </summary>
        public string kaynak;

        /// <summary>
        /// Gerçekleşen değerlere ait sonuç açıklaması
        /// </summary>
        public string sonuc;

        /// <summary>
        /// Gerçekleşen değerlere ait sapma açıklaması
        /// </summary>
        public string sapma;

        /// <summary>
        /// Gerçekleşen değerlere ait önlem açıklaması
        /// </summary>
        public string onlem;


        public bool veriYok { get; set; }

        public GGSade()
        {
            ay = 0;
            veriYok = false;
        }
    }

    /// <summary>
    /// Performans göstergesi için girilen gerçekleşen değerleri tutan sınıf
    /// </summary>
    [Serializable()]
    public class PerformansGostergeGerceklesenDeger
    {
        /// <summary>
        /// Gerçekleşen değerlerin ait olduğu performans göstergesinin bilgilerini tutan nesne
        /// </summary>
        public PerformansGosterge gosterge;

        /// <summary>
        /// Gerçekleşen değer
        /// </summary>
        public double miktar;

        /// <summary>
        /// Hedeflenen değerler dizisi
        /// </summary>
        public double[] hedefMiktar;

        /// <summary>
        /// Gerçekleşen değerlere ait kaynak açıklaması
        /// </summary>
        public string kaynak;

        /// <summary>
        /// Gerçekleşen değerlere ait sonuç açıklaması
        /// </summary>
        public string sonuc;

        /// <summary>
        /// Gerçekleşen değerlere ait sapma açıklaması
        /// </summary>
        public string sapma;

        /// <summary>
        /// Gerçekleşen değerlere ait önlem açıklaması
        /// </summary>
        public string onlem;

        public TNSCollection donemDegerler = null;
        /// <summary>
        /// Parametresiz PerformansGostergeGerceklesenDeger nesnesi oluşturucu yordam
        /// </summary>
        public PerformansGostergeGerceklesenDeger()
        {
            gosterge = new PerformansGosterge();
            miktar = 0;
            hedefMiktar = new double[3];
            kaynak = string.Empty;
            sonuc = string.Empty;
            sapma = string.Empty;
            onlem = string.Empty;
        }

        public GGSade AyBilgiBul(int ay)
        {
            if (donemDegerler == null) return null;
            foreach (GGSade s in donemDegerler)
            {
                if (s.ay == ay) return s;
            }
            return null;
        }

        public bool veriYok { get; set; }
    }

    /// <summary>
    /// Performans Hedef bilgilerini tutan sınıf
    /// Performans göstergesi hedeflenen değerleri ve gerçekleşen değerleri, stratejik hedeflerle bağı ve ağırlıkları
    /// ve faaliyet/projeler ile bağı bilgilerini de içerir. Bu alanlar, raporlamada kullanılıyor.
    /// </summary>
    [Serializable()]
    public class PerformansHedef
    {
        /// <summary>
        /// Performans hedefin girildiği yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Performans hedefin biriminin kodu
        /// </summary>
        public string kurKod { get; set; }

        /// <summary>
        /// Performans hedefin biriminin adı
        /// </summary>
        public string kurAd { get; set; }

        /// <summary>
        /// Performans hedefin programatik kodu
        /// </summary>
        public int perHedefKod { get; set; }

        /// <summary>
        /// Performans hedefin adı
        /// </summary>
        public string perHedef { get; set; }

        /// <summary>
        /// Performans hedefin açıklaması
        /// </summary>
        public string aciklama { get; set; }

        /// <summary>
        /// Performans hedefe ait performans göstergesi hedeflenen değerlerinin listesini tutan nesne
        /// </summary>
        public TNSCollection perGostergeHedeflenen;

        /// <summary>
        /// Performans hedefe ait performans göstergesi gerçekleşen değerlerinin listesini tutan nesne
        /// </summary>
        public TNSCollection perGostergeGerceklesen;

        /// <summary>
        /// Performans hedefin stratejik hedeflerle bağı ve ağırlıkları listesini tutan nesne
        /// </summary>
        public TNSCollection perAgirlik;

        /// <summary>
        /// Performans hedefe ait ödenek bilgisi
        /// </summary>
        public double odenek;

        /// <summary>
        /// Performans hedefin e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin performans bütçe entegrasyonunda performans hedefi için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod { get; set; }

        /// <summary>
        /// Performans hedefin faaliyet/projeler ile bağlarını tutan nesne
        /// </summary>
        public TNSCollection fpler;

        /// <summary>
        /// Parametresiz PerformansHedef nesnesi oluşturucu yordam
        /// </summary>
        public PerformansHedef()
        {
            yil = 0;
            kurKod = string.Empty;
            kurAd = string.Empty;
            perHedefKod = 0;
            perHedef = string.Empty;
            aciklama = string.Empty;
            perGostergeHedeflenen = new TNSCollection();
            perGostergeGerceklesen = new TNSCollection();
            perAgirlik = new TNSCollection();
            odenek = 0;
            ebutceKod = string.Empty;
            fpler = new TNSCollection();
            perHedefGorKod = "";
        }

        public string perHedefGorKod { get; set; }

        public string TamKod
        {
            get
            {
                return yil.ToString() + "-" + kurKod + "-" + perHedefKod.ToString();
            }
        }

        /// <summary>
        /// Performans hedefin Tam adı
        /// </summary>
        public string TamAd { get { if (string.IsNullOrWhiteSpace(perHedef)) return ""; return "(" + kurKod + ") " + perHedefGorKod + " " + perHedef; } }


        public string kalkinmaAmac { get; set; }

        public string kalkinmaHedef { get; set; }

        public string kalkinmaPolitika { get; set; }
    }

    /// <summary>
    /// Performans hedefin stratejik hedeflerle bağını ve ağırlıklarını tutan sınıf
    /// </summary>
    [Serializable()]
    public class PerformansHedefAgirlik
    {
        /// <summary>
        /// Performans hedefin girildiği yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Performans hedefin biriminin kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Performans hedefin biriminin adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Performans hedefin programatik kodu
        /// </summary>
        public int perHedefKod;

        /// <summary>
        /// Stratejik hedef bilgilerini tutan nesne
        /// </summary>
        public StratejikHedef strHedef;

        public int strHedefKod { get { return strHedef.strHedefKod; } set { strHedef.strHedefKod = value; } }
        public string strHedefAd { get { return strHedef.strHedef; } set { strHedef.strHedef = value; } }
        public string strHedefKodAd { get { return strHedef.strHedefKisa + " " + strHedef.strHedef; } }
        /// <summary>
        /// Performans hedef ile stratejik hedef arasındaki ağırlık yüzdesi
        /// </summary>
        public double agirlik { get; set; }

        /// <summary>
        /// Parametresiz PerformansHedefAgirlik nesnesi oluşturucu yordam
        /// </summary>
        public PerformansHedefAgirlik()
        {
            yil = 0;
            kurKod = string.Empty;
            kurAd = string.Empty;
            perHedefKod = 0;
            strHedef = new StratejikHedef();
            agirlik = 0;
        }
    }

    /// <summary>
    /// Faaliyet/proje bilgilerini tutan sınıf
    /// Tertibin 5. boyutu olan alt faaliyetlerle bağını, performans hedeflerle bağını ve ağırlıklarını
    /// ve ödenek bilgilerini içerir. Bu alanlar, raporlamada kullanılıyor.
    /// </summary>
    [Serializable()]
    public class FaaliyetProje
    {
        /// <summary>
        /// Faaliyet/projenin girildiği yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Faaliyet/projenin biriminin kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Faaliyet/projenin biriminin adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Faaliyet/projenin programatik kodu
        /// </summary>
        public int fpKod;

        /// <summary>
        /// Faaliyet/projenin adı
        /// </summary>
        public string fpAd;

        /// <summary>
        /// Faaliyet/projenin seviye açıklaması
        /// </summary>
        public string seviye;

        /// <summary>
        /// Faaliyet/projenin sorumlusu
        /// </summary>
        public string sorumlu;

        /// <summary>
        /// Nesnenin tuttuğu bilgilerin faaliyete mi, projeye mi ait olduğu
        /// ENUMFaaliyetProje listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int projeMi;

        /// <summary>
        /// Faaliyet/projenin türü
        /// ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int tur;

        /// <summary>
        /// Faaliyet/projenin e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin performans bütçe entegrasyonunda faaliyet/proje için kullanılan bir koddur.
        /// </summary>
        public string ebutceKod;

        /// <summary>
        /// Faaliyet/projenin performans hedeflerle bağı ve ağırlıkları listesini tutan nesne
        /// </summary>
        public TNSCollection fpAgirlik;

        /// <summary>
        /// Tertibin 5. boyutu olan alt faaliyetlerle bağı listesini tutan nesne
        /// </summary>
        public TNSCollection fpPEBFonKodlar;

        /// <summary>
        /// Faaliyet/projeye ait ödenek bilgilerinin listesinin tutulduğu nesne
        /// </summary>
        public TNSCollection odenekler;

        public string fpAciklama;

        /// <summary>
        /// Parametresiz FaaliyetProje nesnesi oluşturucu yordam
        /// </summary>
        public FaaliyetProje()
        {
            yil = 0;
            fpAciklama = string.Empty;
            kurKod = string.Empty;
            kurAd = string.Empty;
            fpKod = 0;
            fpAd = string.Empty;
            seviye = string.Empty;
            sorumlu = string.Empty;
            projeMi = 0;
            tur = 0;
            ebutceKod = string.Empty;
            fpAgirlik = new TNSCollection();
            fpPEBFonKodlar = new TNSCollection();
            odenekler = new TNSCollection();
        }
    }

    /// <summary>
    /// Faaliyet/projenin performans hedeflerle bağını ve ağırlıklarını tutan sınıf
    /// </summary>
    [Serializable()]
    public class FaaliyetProjeAgirlik
    {
        /// <summary>
        /// Faaliyet/projenin girildiği yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Faaliyet/projenin biriminin kodu
        /// </summary>
        public string fpKurKod;

        /// <summary>
        /// Faaliyet/projenin biriminin adı
        /// </summary>
        public string fpKurAd;

        /// <summary>
        /// Faaliyet/projenin programatik kodu
        /// </summary>
        public int fpKod;

        /// <summary>
        /// Performans hedefin biriminin kodu
        /// </summary>
        public string phKurKod { get; set; }

        /// <summary>
        /// Performans hedefin biriminin adı
        /// </summary>
        public string phKurAd { get; set; }

        /// <summary>
        /// Performans hedefin programatik kodu
        /// </summary>
        public int perHedefKod { get; set; }

        /// <summary>
        /// Performans hedefin adı
        /// </summary>
        public string perHedefAd { get; set; }

        /// <summary>
        /// Performans hedefin Tam adı
        /// </summary>
        public string perHedefTamAd { get { if (string.IsNullOrWhiteSpace(perHedefAd)) return ""; return "(" + phKurKod + ") " + perHedefGorKod + " " + perHedefAd; } }

        /// <summary>
        /// Performans hedefin e-butce sistemindeki programatik kodu
        /// e-butce ile sgb.net sistemlerinin performans bütçe entegrasyonunda performans hedefi için kullanılan bir koddur.
        /// </summary>
        public string perHedefEbutceKod;

        /// <summary>
        /// Faaliyet/proje ile performans hedef arasındaki ağırlık yüzdesi
        /// </summary>
        public double agirlik { get; set; }

        /// <summary>
        /// Parametresiz PerformansHedefAgirlik nesnesi oluşturucu yordam
        /// </summary>
        public FaaliyetProjeAgirlik()
        {
            yil = 0;
            fpKurKod = string.Empty;
            fpKurAd = string.Empty;
            fpKod = 0;
            phKurKod = string.Empty;
            phKurAd = string.Empty;
            perHedefKod = 0;
            perHedefAd = string.Empty;
            perHedefEbutceKod = string.Empty;
            agirlik = 0;
        }

        public string perHedefGorKod { get; set; }
    }

    /// <summary>
    /// Faaliyet/projenin ödenek bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class FaaliyetProjeOdenek
    {
        /// <summary>
        /// Faaliyet/projenin girildiği yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Faaliyet/projenin biriminin kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Faaliyet/projenin biriminin adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Faaliyet/projenin ödeneklerinin girildiği senaryo
        /// </summary>
        public string senaryoKod;

        /// <summary>
        /// Faaliyet/projenin programatik kodu
        /// </summary>
        public int fpKod;

        /// <summary>
        /// Faaliyet/projenin adı
        /// </summary>
        public string fpAd;

        /// <summary>
        /// Ödeneğin girildiği tertip bilgilerini tutan nesne
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Faaliyet/projenin ilgili tertibine ait ödenek miktarı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Faaliyet/projenin alt faaliyet kodu
        /// </summary>
        public string fpFonKod;

        /// <summary>
        /// Faaliyet/projenin alt faaliyet adı
        /// </summary>
        public string fpFonAd;

        /// <summary>
        /// Parametresiz FaaliyetProjeOdenek nesnesi oluşturucu yordam
        /// </summary>
        public FaaliyetProjeOdenek()
        {
            yil = 0;
            kurKod = string.Empty;
            kurAd = string.Empty;
            senaryoKod = string.Empty;
            fpKod = 0;
            fpAd = string.Empty;
            tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            odenek = 0;
            fpFonKod = string.Empty;
            fpFonAd = string.Empty;
        }
    }

    /// <summary>
    /// Sabit maliyet bilgilerini tutan sınıf
    /// Tertibin 5. boyutu olan alt faaliyetlerle bağını, faaliyet/projelerle bağını ve ağırlıklarını,
    /// ödenek bilgilerini ve operasyonel planda kullanılmak üzere performans hedefle bağını içerir.
    /// Bu alanlar, genel olarak raporlamada kullanılıyor.
    /// </summary>
    [Serializable()]
    public class SabitMaliyet
    {
        /// <summary>
        /// Sabit maliyetin girildiği yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Sabit maliyetin biriminin kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Sabit maliyetin biriminin adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Sabit maliyetin programatik kodu
        /// </summary>
        public int smKod;

        /// <summary>
        /// Sabit maliyetin adı
        /// </summary>
        public string smAd;

        /// <summary>
        /// Sabit maliyetin türü
        /// ENUMSabitMaliyetTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int tur;

        /// <summary>
        /// Sabit maliyetin faaliyet/projelerle bağı ve ağırlıkları listesini tutan nesne
        /// </summary>
        public TNSCollection smAgirlik;

        /// <summary>
        /// Sabit maliyete ait ödenek bilgilerinin listesinin tutulduğu nesne
        /// </summary>
        public TNSCollection smOdenek;

        /// <summary>
        /// Sabit maliyetin performans hedef ile bağını tutan nesne
        /// Bu bilgi operasyonel planda kullanılıyor, yani aşağı doğru bir bağı ifade ediyor,
        /// performans bütçe ödenekleri ile ilgili bir bağ teşkil etmiyor.
        /// </summary>
        public PerformansHedef perHedef;

        /// <summary>
        /// Tertibin 5. boyutu olan alt faaliyetlerle bağı listesini tutan nesne
        /// </summary>
        public TNSCollection smPEBFonKodlar;


        public string smAciklama;

        /// <summary>
        /// Parametresiz SabitMaliyet nesnesi oluşturucu yordam
        /// </summary>
        public SabitMaliyet()
        {
            yil = 0;
            smAciklama = string.Empty;
            kurKod = string.Empty;
            kurAd = string.Empty;
            smKod = 0;
            smAd = string.Empty;
            tur = 0;
            smAgirlik = new TNSCollection();
            smOdenek = new TNSCollection();
            perHedef = new PerformansHedef();
            smPEBFonKodlar = new TNSCollection();
        }
    }

    /// <summary>
    /// Sabit maliyetin faaliyet/projelerle bağını ve ağırlıklarını tutan sınıf
    /// </summary>
    [Serializable()]
    public class SabitMaliyetAgirlik
    {
        /// <summary>
        /// Sabit maliyetin girildiği yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Sabit maliyetin biriminin kodu
        /// </summary>
        public string smKurKod { get; set; }

        /// <summary>
        /// Sabit maliyetin biriminin adı
        /// </summary>
        public string smKurAd { get; set; }

        /// <summary>
        /// Sabit maliyetin programatik kodu
        /// </summary>
        public int smKod { get; set; }

        /// <summary>
        /// Sabit maliyetin adı
        /// </summary>
        public string smAd { get; set; }

        /// <summary>
        /// Faaliyet/projenin biriminin kodu
        /// </summary>
        public string fpKurKod { get; set; }

        /// <summary>
        /// Faaliyet/projenin biriminin adı
        /// </summary>
        public string fpKurAd { get; set; }

        /// <summary>
        /// Faaliyet/projenin programatik kodu
        /// </summary>
        public int fpKod { get; set; }

        /// <summary>
        /// Faaliyet/projenin adı
        /// </summary>
        public string fpAd { get; set; }

        /// <summary>
        /// Sabit maliyet ile faaliyet/proje arasındaki ağırlık yüzdesi
        /// </summary>
        public double agirlik { get; set; }

        /// <summary>
        /// Sabit maliyet ile faaliyet/proje arasındaki ağırlığa göre hesaplanmış yansıyan ödenek
        /// </summary>
        public double odenek { get; set; }

        /// <summary>
        /// Sabit maliyet ile faaliyet/proje arasındaki ağırlığa göre hesaplanmış yansıyan harcama
        /// </summary>
        public double harcama { get; set; }

        /// <summary>
        /// Sabit maliyetin ödeneklerinin girildiği senaryo
        /// </summary>
        public string senaryoKod { get; set; }

        /// <summary>
        /// Parametresiz SabitMaliyetAgirlik nesnesi oluşturucu yordam
        /// </summary>
        public SabitMaliyetAgirlik()
        {
            yil = 0;
            smKurKod = string.Empty;
            smKurAd = string.Empty;
            smKod = 0;
            smAd = string.Empty;
            fpKurKod = string.Empty;
            fpKurAd = string.Empty;
            fpKod = 0;
            fpAd = string.Empty;
            agirlik = 0;
            odenek = 0;
            harcama = 0;
            senaryoKod = string.Empty;
        }
    }

    /// <summary>
    /// Sabit maliyetin ödenek bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class SabitMaliyetOdenek
    {
        /// <summary>
        /// Sabit maliyetin girildiği yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Sabit maliyetin biriminin kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Sabit maliyetin biriminin adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Sabit maliyetin ödeneklerinin girildiği senaryo
        /// </summary>
        public string senaryoKod;

        /// <summary>
        /// Sabit maliyetin programatik kodu
        /// </summary>
        public int smKod;

        /// <summary>
        /// Ödeneğin girildiği tertip bilgilerini tutan nesne
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Sabit maliyetin ilgili tertibine ait ödenek miktarı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Sabit maliyetin alt faaliyet kodu
        /// </summary>
        public string smFonKod;

        /// <summary>
        /// Sabit maliyetin alt faaliyet adı
        /// </summary>
        public string smFonAd;

        /// <summary>
        /// Parametresiz SabitMaliyetOdenek nesnesi oluşturucu yordam
        /// </summary>
        public SabitMaliyetOdenek()
        {
            yil = 0;
            kurKod = string.Empty;
            kurAd = string.Empty;
            senaryoKod = string.Empty;
            smKod = 0;
            tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            odenek = 0;
            smFonKod = string.Empty;
            smFonAd = string.Empty;
            //smTur = 0;
        }
    }

    /// <summary>
    /// Stratejik hedef ve birim bazında ödenek toplamlarını tutan sınıf
    /// Stratejik amaç raporu için kullanılan bir sınıftır ve 1 nesne raporun tamamını ifade eder.
    /// </summary>
    [Serializable()]
    public class StrAmacRapor
    {
        /// <summary>
        /// Stratejik hedef listesini tutan nesne
        /// </summary>
        public TNSCollection strHedefler;

        /// <summary>
        /// Stratejik hedeflere ait ödenek toplamları listesini tutan nesne
        /// </summary>
        public TNSCollection odenekler;

        /// <summary>
        /// Parametresiz StrAmacRapor nesnesi oluşturucu yordam
        /// </summary>
        public StrAmacRapor()
        {
            strHedefler = new TNSCollection();
            odenekler = new TNSCollection();
        }
    }

    /// <summary>
    /// Stratejik hedef ve birim bazında ödenek toplamlarını tutan sınıf
    /// Stratejik amaç raporu için kullanılan bir sınıftır ve 1 nesne raporun 1 satırını ifade eder.
    /// </summary>
    [Serializable()]
    public class StrAmacRaporOdenek
    {
        /// <summary>
        /// Stratejik plan döneminin başlangıç yılı
        /// </summary>
        public int donemBYil;

        /// <summary>
        /// Stratejik hedef programatik kodu
        /// </summary>
        public int strHedefKod;

        /// <summary>
        /// Stratejik hedef adı
        /// </summary>
        public string strHedefAd;

        /// <summary>
        /// Stratejik hedefin özet/kısa bilgisi
        /// </summary>
        public string strHedefKisa;

        /// <summary>
        /// Birim kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Birim adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Ödenek toplamı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Parametresiz StrAmacRaporOdenek nesnesi oluşturucu yordam
        /// </summary>
        public StrAmacRaporOdenek()
        {
            donemBYil = 0;
            strHedefKod = 0;
            strHedefAd = string.Empty;
            strHedefKisa = string.Empty;
            kurKod = string.Empty;
            kurAd = string.Empty;
            odenek = 0;
        }
    }

    /// <summary>
    /// Faaliyetlerin, projelerin ve birim türündeki sabit maliyetlerin ödeneklerini tutan sınıf
    /// Sabit maliyetlerden faaliyetlere yansıyan ödenekler de eklenir ödenek bilgilerine.
    /// </summary>
    [Serializable()]
    public class OdenekDurumListesi
    {
        /// <summary>
        /// Rapor yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Raporun birim kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Raporun birim adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Raporun senaryosu
        /// </summary>
        public string odenekSenaryo;

        /// <summary>
        /// Faaliyetlerin ve ödeneklerinin liste şeklinde tutulduğu nesne
        /// </summary>
        public TNSCollection faaliyetler;

        /// <summary>
        /// Projelerin ve ödeneklerinin liste şeklinde tutulduğu nesne
        /// </summary>
        public TNSCollection projeler;

        /// <summary>
        /// Birim türündeki sabit maliyetlerin ve ödeneklerinin liste şeklinde tutulduğu nesne
        /// </summary>
        public TNSCollection birimSMOdenekler;

        /// <summary>
        /// Parametresiz OdenekDurumListesi nesnesi oluşturucu yordam
        /// </summary>
        public OdenekDurumListesi()
        {
            yil = 0;
            kurKod = string.Empty;
            kurAd = string.Empty;
            odenekSenaryo = "";
            faaliyetler = new TNSCollection();
            projeler = new TNSCollection();
            birimSMOdenekler = new TNSCollection();
        }
    }

    /// <summary>
    /// Proje bilgilerini ve ödeneklerini tutan sınıf
    /// OdenekDurumListesi sınıfındaki projeler listesindeki nesneler bu yapıdadır.
    /// </summary>
    [Serializable()]
    public class Proje
    {
        /// <summary>
        /// Projenin programatik kodu
        /// </summary>
        public int kod;

        /// <summary>
        /// Projenin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Projenin türü
        /// ENUMFaaliyetProjeTur listesindeki değerlerden biri olmalıdır.
        /// </summary>
        public int tur;

        /// <summary>
        /// Projenin ödenek bilgileri listesini tutan nesne
        /// TertipOdenek yapısında nesneler içerir. 
        /// </summary>
        public TNSCollection odenekler;

        /// <summary>
        /// Parametresiz Proje nesnesi oluşturucu yordam
        /// </summary>
        public Proje()
        {
            kod = 0;
            ad = string.Empty;
            tur = 0;
            odenekler = new TNSCollection();
        }
    }

    /// <summary>
    /// Faaliyet bilgilerini ve ödeneklerini tutan sınıf
    /// OdenekDurumListesi sınıfındaki faaliyetler listesindeki nesneler bu yapıdadır.
    /// </summary>
    [Serializable()]
    public class Faaliyet : Proje
    {
        /// <summary>
        /// Sabit maliyetlerden faaliyetlere yansıyan ödenek bilgileri
        /// Bu değişkende ENUMSabitMaliyetTur listesindeki türler için ödenek toplamları tutulur.
        /// </summary>
        public double[] yansitilanlar;

        /// <summary>
        /// Parametresiz Faaliyet nesnesi oluşturucu yordam
        /// </summary>
        public Faaliyet()
        {
            yansitilanlar = new double[4];
        }
    }

    /// <summary>
    /// Tertip bazında ödenek bilgisi tutan sınıf
    /// </summary>
    [Serializable()]
    public class TertipOdenek
    {
        /// <summary>
        /// Tertip bilgilerini tutan nesne
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// İlgili tertibin ödenek miktarı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Yıl parametresi alan TertipOdenek nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="yil">Tertibin yılı</param>
        public TertipOdenek(int yil)
        {
            tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            odenek = 0;
        }
    }

    /// <summary>
    /// Faaliyet/proje bilgilerini, ödeneklerini ve sabit maliyetlerden yansıyan ödenekleri tutan sınıf
    /// </summary>
    [Serializable()]
    public class ButceFaaliyetDagilim
    {
        /// <summary>
        /// Faaliyet/proje bilgilerini tutan nesne
        /// </summary>
        public FaaliyetProje fp;

        /// <summary>
        /// Faaliyet/projenin ödenek toplamı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Faaliyet/projenin personel gideri kaleminden ödenek toplamı
        /// </summary>
        public double personelGider;

        /// <summary>
        /// Birim türündeki sabit maliyetlerden faaliyet/projeye yansıyan ödenek toplamı
        /// </summary>
        public double birimSM;

        /// <summary>
        /// Merkez türündeki sabit maliyetlerden faaliyet/projeye yansıyan ödenek toplamı
        /// </summary>
        public double merkezSM;

        /// <summary>
        /// Taşra türündeki sabit maliyetlerden faaliyet/projeye yansıyan ödenek toplamı
        /// </summary>
        public double tasraSM;

        /// <summary>
        /// Merkez + Taşra türündeki sabit maliyetlerden faaliyet/projeye yansıyan ödenek toplamı
        /// </summary>
        public double merkezTasraSM;

        /// <summary>
        /// Parametresiz ButceFaaliyetDagilim nesnesi oluşturucu yordam
        /// </summary>
        public ButceFaaliyetDagilim()
        {
            fp = new FaaliyetProje();
            odenek = 0;
            personelGider = 0;
            birimSM = 0;
            merkezSM = 0;
            tasraSM = 0;
            merkezTasraSM = 0;
        }
    }

    /// <summary>
    /// Tema bilgilerini, ödeneğini ve temaya bağlı olan stratejik amaçları tutan sınıf
    /// Performans hedef dağılım raporundaki hiyerarşik yapıda kullanılıyor.
    /// </summary>
    [Serializable()]
    public class TemaOdenek
    {
        /// <summary>
        /// Tema bilgilerini tutan nesne
        /// </summary>
        public Tema tema;

        /// <summary>
        /// Temanın ödenek toplamı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Temaya bağlı olan stratejik amaçlar listesini tutan nesne
        /// </summary>
        public TNSCollection sAmaclar;

        /// <summary>
        /// Parametresiz TemaOdenek nesnesi oluşturucu yordam
        /// </summary>
        public TemaOdenek()
        {
            tema = new Tema();
            odenek = 0;
            sAmaclar = new TNSCollection();
        }
    }

    /// <summary>
    /// Stratejik amaç bilgilerini, ödeneğini ve stratejik amaca bağlı olan stratejik hedefleri tutan sınıf
    /// Performans hedef dağılım raporundaki hiyerarşik yapıda kullanılıyor.
    /// </summary>
    [Serializable()]
    public class StratejikAmacOdenek
    {
        /// <summary>
        /// Stratejik amaç bilgilerini tutan nesne
        /// </summary>
        public StratejikAmac sAmac;

        /// <summary>
        /// Stratejik amacın ödenek toplamı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Stratejik amaca bağlı olan stratejik hedefler listesini tutan nesne
        /// </summary>
        public TNSCollection sHedefler;

        /// <summary>
        /// Parametresiz StratejikAmacOdenek nesnesi oluşturucu yordam
        /// </summary>
        public StratejikAmacOdenek()
        {
            sAmac = new StratejikAmac();
            odenek = 0;
            sHedefler = new TNSCollection();
        }
    }

    /// <summary>
    /// Stratejik hedef bilgilerini, ödeneğini ve stratejik hedefe bağlı olan performans hedefleri tutan sınıf
    /// Performans hedef dağılım raporundaki hiyerarşik yapıda kullanılıyor.
    /// </summary>
    [Serializable()]
    public class StratejikHedefOdenek
    {
        /// <summary>
        /// Stratejik hedef bilgilerini tutan nesne
        /// </summary>
        public StratejikHedef sHedef;

        /// <summary>
        /// Stratejik hedefin ödenek toplamı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Stratejik hedefe bağlı olan performans hedefler listesini tutan nesne
        /// </summary>
        public TNSCollection pHedefler;

        /// <summary>
        /// Parametresiz StratejikHedefOdenek nesnesi oluşturucu yordam
        /// </summary>
        public StratejikHedefOdenek()
        {
            sHedef = new StratejikHedef();
            odenek = 0;
            pHedefler = new TNSCollection();
        }
    }

    /// <summary>
    /// Stratejik amaçları ve ödenek toplamlarını tutan sınıf
    /// Diğer kurum içi ve diğer kurum dışı türündeki faaliyetlerin ödeneklerini de içeriyor.
    /// </summary>
    [Serializable()]
    public class PEBOzetRapor
    {
        /// <summary>
        /// Stratejik amaçları ve ödeneklerini tutan StratejikAmacOdenek
        /// nesnelerinden oluşan listeyi tutan nesne
        /// </summary>
        public TNSCollection sAmaclar;

        /// <summary>
        /// Diğer kurum içi türündeki faaliyetlerin ödenekleri toplamı
        /// </summary>
        public double digerKurumIci;

        /// <summary>
        /// Diğer kurum dışı türündeki faaliyetlerin ödenekleri toplamı
        /// </summary>
        public double digerKurumDisi;

        /// <summary>
        /// Parametresiz PEBOzetRapor nesnesi oluşturucu yordam
        /// </summary>
        public PEBOzetRapor()
        {
            sAmaclar = new TNSCollection();
            digerKurumIci = 0;
            digerKurumDisi = 0;
        }
    }

    /// <summary>
    /// Stratejik amaçların tertip bazında veya tertibin bir kırılımı bazında ödeneklerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class StratejikAmacOdenekDagilim
    {
        /// <summary>
        /// Stratejik amaç bilgilerini tutan nesne
        /// </summary>
        public StratejikAmac sAmac;

        /// <summary>
        /// Tertip ve ödenek listesini tutan nesne
        /// TertipOdenek yapısındaki nesnelerden oluşan bir listedir.
        /// </summary>
        public TNSCollection odenekler;

        /// <summary>
        /// Parametresiz StratejikAmacOdenekDagilim nesnesi oluşturucu yordam
        /// </summary>
        public StratejikAmacOdenekDagilim()
        {
            sAmac = new StratejikAmac();
            odenekler = new TNSCollection();
        }
    }

    /// <summary>
    /// Stratejik hedeflerin tertip bazında veya tertibin bir kırılımı bazında ödeneklerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class StratejikHedefOdenekDagilim
    {
        /// <summary>
        /// Stratejik hedef bilgilerini tutan nesne
        /// </summary>
        public StratejikHedef sHedef;

        /// <summary>
        /// Tertip ve ödenek listesini tutan nesne
        /// TertipOdenek yapısındaki nesnelerden oluşan bir listedir.
        /// </summary>
        public TNSCollection odenekler;

        /// <summary>
        /// Parametresiz StratejikHedefOdenekDagilim nesnesi oluşturucu yordam
        /// </summary>
        public StratejikHedefOdenekDagilim()
        {
            sHedef = new StratejikHedef();
            odenekler = new TNSCollection();
        }
    }

    /// <summary>
    /// Birimlerin yaptıkları işlerle ilgili aldıkları tedbirlere ilişkin üst bilgileri tutan sınıf
    /// </summary>
    [Serializable()]
    public class Tedbir
    {
        /// <summary>
        /// Tedbir yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Birim kodu
        /// </summary>
        public int kurKod;

        /// <summary>
        /// Birim adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Tedbirin başlangıç tarihi
        /// </summary>
        public TNSDateTime baslangicTarih;

        /// <summary>
        /// Tedbire ilişkin mevcut durum bilgilerinin tarihi
        /// </summary>
        public TNSDateTime mevcutTarih;

        /// <summary>
        /// Tedbir detay bilgilerinin tutulduğu TedbirDetay nesnelerinden oluşan liste
        /// </summary>
        public TNSCollection detaylar;

        /// <summary>
        /// Parametresiz Tedbir nesnesi oluşturucu yordam
        /// </summary>
        public Tedbir()
        {
            yil = 0;
            kurKod = 0;
            kurAd = string.Empty;
            baslangicTarih = new TNSDateTime();
            mevcutTarih = new TNSDateTime();
            detaylar = new TNSCollection();
        }
    }

    /// <summary>
    /// Birimlerin yaptıkları işlerle ilgili aldıkları tedbirlere ilişkin detay bilgileri tutan sınıf
    /// </summary>
    [Serializable()]
    public class TedbirDetay
    {
        /// <summary>
        /// Tedbir yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Birim kodu
        /// </summary>
        public int kurKod;

        /// <summary>
        /// Birim adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Tedbirin satır numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Tedbirin ana eksen açıklaması
        /// </summary>
        public string anaEksen;

        /// <summary>
        /// Tedbirin alt eksen açıklaması
        /// </summary>
        public string altEksen;

        /// <summary>
        /// Öncelikli alınacak tedbir açıklaması
        /// </summary>
        public string oncelikTedbir;

        /// <summary>
        /// Tedbirle ilgili işbirliği yapılacak kurum, kuruluş, idare bilgileri
        /// </summary>
        public string isbirlikciIdare;

        /// <summary>
        /// Tedbirin biteceği zaman
        /// </summary>
        public string sure;

        /// <summary>
        /// Yapılanlarla ilgili açıklama
        /// </summary>
        public string islemAciklama;

        /// <summary>
        /// Başlangıçtaki durum analizi
        /// </summary>
        public string baslangicDurum;

        /// <summary>
        /// Mevcut durumun giriliş tarihi
        /// </summary>
        public TNSDateTime mevcutTarih;

        /// <summary>
        /// Tedbire ilişkin mevcut durum açıklaması
        /// </summary>
        public string mevcutDurum;

        /// <summary>
        /// Yetkili kişiynin tedbirle/mevcut durumla ilgili görüşü
        /// </summary>
        public string yetkiliGorusu;

        /// <summary>
        /// Parametresiz TedbirDetay nesnesi oluşturucu yordam
        /// </summary>
        public TedbirDetay()
        {
            yil = 0;
            kurKod = 0;
            kurAd = string.Empty;
            siraNo = 0;
            anaEksen = string.Empty;
            altEksen = string.Empty;
            oncelikTedbir = string.Empty;
            isbirlikciIdare = string.Empty;
            sure = string.Empty;
            islemAciklama = string.Empty;
            baslangicDurum = string.Empty;
            mevcutTarih = new TNSDateTime();
            mevcutDurum = string.Empty;
            yetkiliGorusu = string.Empty;
        }
    }

    /// <summary>
    /// Tedbir birimlerini tutan enumeration
    /// </summary>
    public enum ENUMTedbirBirim : int
    {
        /// <summary>
        /// Strateji Geliştirme Başkanlığı tedbir birimi kodu
        /// </summary>
        YETKILIBIRIM = 5
    }

    /// <summary>
    /// Tedbir birimlerinin bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class TedbirBirim
    {
        /// <summary>
        /// Birim kodu
        /// </summary>
        public int kurKod;

        /// <summary>
        /// Birim adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Parametresiz TedbirBirim nesnesi oluşturucu yordam
        /// </summary>
        public TedbirBirim()
        {
            kurKod = 0;
            kurAd = string.Empty;
        }
    }

    /// <summary>
    /// Birimlerin performans hedefleri, performans göstergeleri ve hedeflenen/gerçekleşen bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class BirimGostergeleriRaporu
    {
        /// <summary>
        /// Stratejik hedef bilgilerini tutan nesne
        /// </summary>
        public StratejikHedef strHedef;

        /// <summary>
        /// Peformans hedefleri ve performans göstergelerini içeren PerformansHedef yapısı listesini tutan nesne
        /// </summary>
        public TNSCollection perHedefler;

        /// <summary>
        /// Parametresiz BirimGostergeleriRaporu nesnesi oluşturucu yordam
        /// </summary>
        public BirimGostergeleriRaporu()
        {
            strHedef = new StratejikHedef();
            perHedefler = new TNSCollection();
        }
    }

    /// <summary>
    /// Birimlerin altındaki daire tanım bilgilerini tutan sınıf
    /// Operasyonel planda kullanılmaktadır.
    /// </summary>
    [Serializable()]
    public class Daire
    {
        /// <summary>
        /// Dairenin kayıtlı olduğu yıl
        /// </summary>
        public int yil;

        /// <summary>
        /// Dairenin bağlı olduğu birimin kodu
        /// </summary>
        public string kurKod;

        /// <summary>
        /// Dairenin bağlı olduğu birimin adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Dairenin programatik kodu
        /// </summary>
        public int daireKod;

        /// <summary>
        /// Dairenin adı
        /// </summary>
        public string daireAd;

        /// <summary>
        /// Parametresiz Daire nesnesi oluşturucu yordam
        /// </summary>
        public Daire()
        {
            yil = 0;
            kurKod = string.Empty;
            kurAd = string.Empty;
            daireKod = 0;
            daireAd = string.Empty;
        }
    }

    /// <summary>
    /// Dairelerin, faaliyet/proje ve sabit maliyetlere olan desteğini ifade eden alt faaliyet bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class AltFaaliyet
    {
        /// <summary>
        /// Alt faaliyet yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Alt faaliyetin bağlı olduğu birimin kodu
        /// </summary>
        public int orgKod;

        /// <summary>
        /// Alt faaliyetin bağlı olduğu birimin adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// Alt faaliyetin programatik kodu
        /// </summary>
        public int afKod;

        /// <summary>
        /// Alt faaliyetin adı
        /// </summary>
        public string afAd;

        /// <summary>
        /// Alt faaliyetin bağlı olduğu faaliyet/proje bilgilerini tutan nesne
        /// </summary>
        public FaaliyetProje fp;

        /// <summary>
        /// Alt faaliyetin bağlı olduğu sabit maliyet bilgilerini tutan nesne
        /// </summary>
        public SabitMaliyet sm;

        ///// <summary>
        ///// Alt faaliyetin bağlı olduğu daire bilgilerini tutan nesne
        ///// </summary>
        //public Daire daire;

        /// <summary>
        /// Parametresiz AltFaaliyet nesnesi oluşturucu yordam
        /// </summary>
        public AltFaaliyet()
        {
            yil = 0;
            orgKod = 0;
            kurAd = string.Empty;
            afKod = 0;
            afAd = string.Empty;
            fp = new FaaliyetProje();
            sm = new SabitMaliyet();
            //daire = new Daire();
        }
    }


    /// <summary>
    /// İş/görev bilgilerini, zaman planını, sorumlu personelleri ve performans göstergelerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class IsGorev
    {
        /// <summary>
        /// İş/görevin yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Birim kodu
        /// </summary>
        public int orgKod;

        /// <summary>
        /// Birim adı
        /// </summary>
        public string kurAd;

        /// <summary>
        /// İş/görevin programatik kodu
        /// </summary>
        public int isGorevKod;

        /// <summary>
        /// İş/görevin adı
        /// </summary>
        public string isGorevAd;

        /// <summary>
        /// Alt faaliyet bilgilerini tutan nesne
        /// </summary>
        public AltFaaliyet af;

        /// <summary>
        /// İş/göreve ait süreç açıklaması
        /// </summary>
        public string aciklama;

        /// <summary>
        /// İş/görevle ilgili zamanlama bilgilerini tutan liste
        /// IsGorevZamanlama yapısında nesnelerden oluşan listeyi tutar.
        /// </summary>
        public TNSCollection zamanlamalar;

        /// <summary>
        /// İş/görevle ilgili sorumlu personel bilgilerini tutan liste
        /// IsGorevSorumlu yapısında nesnelerden oluşan listeyi tutar.
        /// </summary>
        public TNSCollection sorumlular;

        /// <summary>
        /// İş/görevle ilgili performans göstergelerini tutan liste
        /// IsGorevGosterge yapısında nesnelerden oluşan listeyi tutar.
        /// </summary>
        public TNSCollection gostergeler;

        /// <summary>
        /// Yapılacak işin genel hedefinin açıklamasının tutulduğu alan.
        /// </summary>
        public string hedef;

        /// <summary>
        /// Yapılacak işin tekrar sıklığının tutulduğu alan.
        /// </summary>
        public int tekrarSikligi;

        /// <summary>
        /// Parametresiz IsGorev nesnesi oluşturucu yordam
        /// </summary>
        public IsGorev()
        {
            yil = 0;
            orgKod = 0;
            kurAd = string.Empty;
            isGorevKod = 0;
            isGorevAd = string.Empty;
            af = new AltFaaliyet();
            aciklama = string.Empty;
            zamanlamalar = new TNSCollection();
            sorumlular = new TNSCollection();
            gostergeler = new TNSCollection();
            tekrarSikligi = 0;
        }
    }

    /// <summary>
    /// İş/görevin zaman planı bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class IsGorevZamanlama
    {
        public string basTarihStr
        {
            set
            {
                basTarih.Yaz(value.Replace('T', ' '));
            }
            get
            {
                return basTarih.ToString();
            }
        }

        public string bitTarihStr
        {
            set
            {
                bitTarih.Yaz(value.Replace('T', ' '));
            }
            get
            {
                return bitTarih.ToString();
            }
        }
        /// <summary>
        /// Satır sıra numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// İş/görevin başlangıç tarihi
        /// </summary>
        public TNSDateTime basTarih;

        /// <summary>
        /// İş/görevin bitiş tarihi
        /// </summary>
        public TNSDateTime bitTarih;

        /// <summary>
        /// İş/görevin başlangıç açıklaması
        /// </summary>
        public string basAciklama;

        /// <summary>
        /// İş/görevin bitiş açıklaması
        /// </summary>
        public string bitAciklama;

        /// <summary>
        /// İş/göreve ayrılması beklenen süre
        /// </summary>
        public string sure;

        /// <summary>
        /// İş/görevin yapılacağı yer
        /// </summary>
        public string yer;

        /// <summary>
        /// Sorgulama ekranında gerek duyulan bu alan başlangıç tarihi için aralık verebilmek amacıyla eklendi.
        /// </summary>
        public TNSDateTime basTarih2;

        /// <summary>
        /// Sorgulama ekranında gerek duyulan bu alan bitiş tarihi için aralık verebilmek amacıyla eklendi.
        /// </summary>
        public TNSDateTime bitTarih2;

        /// <summary>
        /// Parametresiz IsGorevZamanlama nesnesi oluşturucu yordam
        /// </summary>
        public IsGorevZamanlama()
        {
            siraNo = 0;
            basTarih = new TNSDateTime();
            bitTarih = new TNSDateTime();
            basAciklama = string.Empty;
            bitAciklama = string.Empty;
            sure = string.Empty;
            yer = string.Empty;

            basTarih2 = new TNSDateTime();
            bitTarih2 = new TNSDateTime();
        }
    }

    /// <summary>
    /// İş/görevi yapacak sorumlu personel bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class IsGorevSorumlu
    {
        /// <summary>
        /// Satır sıra numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Sorumlu personelin T.C. kimlik numarası
        /// </summary>
        public string tcKimlikNo;

        /// <summary>
        /// Sorumlu personelin adı
        /// </summary>
        public string ad;

        /// <summary>
        /// Sorumlu personelin soyadı
        /// </summary>
        public string soyad;

        /// <summary>
        /// Sorumlu personelin iş/göreve ait açıklaması
        /// </summary>
        public string gorev;

        /// <summary>
        /// Personellerden hangisinin ana sorumlu olduğu bilgisi
        /// </summary>
        public bool sorumlu;

        /// <summary>
        /// Parametresiz IsGorevSorumlu nesnesi oluşturucu yordam
        /// </summary>
        public IsGorevSorumlu()
        {
            siraNo = 0;
            tcKimlikNo = string.Empty;
            ad = string.Empty;
            soyad = string.Empty;
            gorev = string.Empty;
            sorumlu = false;
        }
    }

    /// <summary>
    /// İş/göreve ait performans göstergeleri bilgilerini tutan sınıf
    /// </summary>
    [Serializable()]
    public class IsGorevGosterge
    {
        /// <summary>
        /// Satır sıra numarası
        /// </summary>
        public int siraNo;

        /// <summary>
        /// Performans göstergesine ait bilgileri tutan nesne
        /// </summary>
        public PerformansGosterge gosterge;

        /// <summary>
        /// Performans göstergesi hedeflenen değeri
        /// </summary>
        public double hedeflenen;

        /// <summary>
        /// Performans göstergesi gerçekleşen değerleri
        /// Gerçekleşme değerleri dönemlik tutuluyor.
        /// </summary>
        public double[] gerceklesen;

        /// <summary>
        /// Performans göstergesine ait açıklama
        /// </summary>
        public string aciklama;

        /// <summary>
        /// Parametresiz IsGorevGosterge nesnesi oluşturucu yordam
        /// </summary>
        public IsGorevGosterge()
        {
            siraNo = 0;
            gosterge = new PerformansGosterge();
            hedeflenen = 0;
            gerceklesen = new double[4];
            aciklama = string.Empty;
        }

        public double donem1
        {
            get { return DonemDeger(0); }
            set { DonemDeger(0, value); }
        }

        public double donem2
        {
            get { return DonemDeger(1); }
            set { DonemDeger(1, value); }
        }

        public double donem3
        {
            get { return DonemDeger(2); }
            set { DonemDeger(2, value); }
        }

        public double donem4
        {
            get { return DonemDeger(3); }
            set { DonemDeger(3, value); }
        }

        public int gostergeKod
        {
            get
            {
                if (gosterge == null) return 0;
                return gosterge.gostergeKod;
            }
            set
            {
                if (gosterge == null) gosterge = new PerformansGosterge();
                gosterge.gostergeKod = value;
            }
        }

        private void DonemDeger(int donemNo, double value)
        {
            if (donemNo < gerceklesen.Length)
                gerceklesen[donemNo] = value;
        }

        private double DonemDeger(int donemNo)
        {
            if (donemNo < gerceklesen.Length) return gerceklesen[donemNo];
            return 0;
        }

        public bool HataliMi()
        {
            if (gosterge.gostergeKod > 0 && hedeflenen < 0 && !GercekVarmi())
            {
                return true;
            }
            return false;

        }

        private bool GercekVarmi()
        {
            if (donem1 != 0) return true;
            if (donem2 != 0) return true;
            if (donem3 != 0) return true;
            if (donem4 != 0) return true;
            return false;
        }
    }

    /// <summary>
    /// Faaliyet/proje veya sabit maliyet bazında tertip ödenek toplamlarını tutan sınıf
    /// </summary>
    [Serializable()]
    public class Odenek
    {
        /// <summary>
        /// Ödenek yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Ödenek senaryosu
        /// </summary>
        public string senaryoKod;

        /// <summary>
        /// Birim kodu
        /// </summary>
        public string maliyetKurKod;

        /// <summary>
        /// Birim adı
        /// </summary>
        public string maliyetKurAd;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin programatik kodu
        /// </summary>
        public int maliyetKod;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin adı
        /// </summary>
        public string maliyetAd;

        /// <summary>
        /// Ödeneğin faaliyet/projeye mi yoksa sabit maliyete mi ait olduğu bilgisi
        /// </summary>
        public int sabitMi;

        /// <summary>
        /// Tertip bilgilerini tutan nesne
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin bütçe ödenekleri ile bağını kuran alt faaliyet kodu
        /// </summary>
        public string maliyetFonKod;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin bütçe ödenekleri ile bağını kuran alt faaliyet adı
        /// </summary>
        public string maliyetFonAd;

        /// <summary>
        /// Tertibin ödenek toplamı
        /// </summary>
        public double odenek;

        /// <summary>
        /// Parametresiz Odenek nesnesi oluşturucu yordam
        /// </summary>
        public Odenek()
        {
            yil = 0;
            senaryoKod = string.Empty;
            maliyetKurKod = string.Empty;
            maliyetKurAd = string.Empty;
            maliyetKod = 0;
            maliyetAd = string.Empty;
            sabitMi = -1;
            tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            maliyetFonKod = string.Empty;
            maliyetFonAd = string.Empty;
            odenek = 0;
        }
    }

    /// <summary>
    /// Faaliyet/proje veya sabit maliyet bazında tertip harcama toplamlarını tutan sınıf
    /// </summary>
    [Serializable()]
    public class Harcama
    {
        /// <summary>
        /// Harcama yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Harcama ayı
        /// </summary>
        public int ay;

        /// <summary>
        /// Birim kodu
        /// </summary>
        public string maliyetKurKod;

        /// <summary>
        /// Birim adı
        /// </summary>
        public string maliyetKurAd;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin programatik kodu
        /// </summary>
        public int maliyetKod;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin adı
        /// </summary>
        public string maliyetAd;

        /// <summary>
        /// Harcamanın faaliyet/projeye mi yoksa sabit maliyete mi ait olduğu bilgisi
        /// </summary>
        public int sabitMi;

        /// <summary>
        /// Tertip bilgilerini tutan nesne
        /// </summary>
        public Tertip tertip;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin bütçe harcamaları ile bağını kuran alt faaliyet kodu
        /// </summary>
        public string maliyetFonKod;

        /// <summary>
        /// Faaliyet/projenin veya sabit maliyetin bütçe harcamaları ile bağını kuran alt faaliyet adı
        /// </summary>
        public string maliyetFonAd;

        /// <summary>
        /// Tertibin harcamasının ağırlık yüzdesi
        /// </summary>
        public double agirlik;

        /// <summary>
        /// Tertibin harcama toplamı
        /// </summary>
        public double harcama;

        /// <summary>
        /// Parametresiz Harcama nesnesi oluşturucu yordam
        /// </summary>
        public Harcama()
        {
            yil = 0;
            ay = 0;
            maliyetKurKod = string.Empty;
            maliyetKurAd = string.Empty;
            maliyetKod = 0;
            maliyetAd = string.Empty;
            sabitMi = -1;
            tertip = new Tertip(yil, ENUMTabloKod.TABOdenek);
            maliyetFonKod = string.Empty;
            maliyetFonAd = string.Empty;
            agirlik = 0;
            harcama = 0;
        }
    }

    /// <summary>
    /// Sabit maliyetlerden faaliyet/projelere yansıyan harcamaları tutan sınıf
    /// </summary>
    [Serializable()]
    public class FaaliyetProjeHarcamaYansiyan
    {
        /// <summary>
        /// Harcama yılı
        /// </summary>
        public int yil;

        /// <summary>
        /// Harcama ayı
        /// </summary>
        public int ay;

        /// <summary>
        /// Faaliyet/proje bilgilerini tutan nesne
        /// </summary>
        public FaaliyetProje fp;

        /// <summary>
        /// Sabit maliyet bilgilerini tutan nesne
        /// </summary>
        public SabitMaliyet sm;

        /// <summary>
        /// Yansıyan harcamanın ağırlık yüzdesi
        /// </summary>
        public double agirlik;

        /// <summary>
        /// Yansıyan harcama miktarı
        /// </summary>
        public double harcama;

        /// <summary>
        /// Parametresiz FaaliyetProjeHarcamaYansiyan nesnesi oluşturucu yordam
        /// </summary>
        public FaaliyetProjeHarcamaYansiyan()
        {
            yil = 0;
            ay = 0;
            fp = new FaaliyetProje();
            sm = new SabitMaliyet();
            agirlik = 0;
            harcama = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SpGenelHavuzBag
    {
        public string kod;
        public ABSKod bagliKod;

        public SpGenelHavuzBag()
        {
            kod = string.Empty;
            bagliKod = new ABSKod();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SpGenelHavuz
    {
        // sabit alanlar
        public string kod;
        public string tur;
        public ABSKod absKod;
        public int durum;
        public ObjectArray organizasyonListe;
        public TNSDateTime baslangicTarih;
        public TNSDateTime bitisTarih;
        public SpGenelHavuzBag[] bagListe;

        //extra alanlar
        public string hizmetMevzuat;
        public string hizmetFaaliyet;
        public string projeTip;
        public string sorunKategori;
        public string sorunTip;
        public string ustBelgeKategori;
        public string onerilenAltMetin;

        public string kurKod;
        public string fonKod;
        public string finKod;
        public string ekoKod;
        public string pebKod;

        /// <summary>
        /// 
        /// </summary>
        public SpGenelHavuz()
        {
            //sabit alanlar
            kod = string.Empty;
            tur = string.Empty;
            durum = 0;
            organizasyonListe = new ObjectArray();
            baslangicTarih = new TNSDateTime();
            bitisTarih = new TNSDateTime();

            //extra alanlar
            hizmetMevzuat = string.Empty;
            hizmetFaaliyet = string.Empty;
            projeTip = string.Empty;
            sorunKategori = string.Empty;
            sorunTip = string.Empty;
            ustBelgeKategori = string.Empty;
            onerilenAltMetin = string.Empty;

            kurKod = string.Empty;
            fonKod = string.Empty;
            finKod = string.Empty;
            ekoKod = string.Empty;
            pebKod = string.Empty;
        }
    }

    [Serializable()]
    public class SpGenelHavuzRapor
    {
        public int donemNo;
        public string birimTur;
        public string havuzKod;
        public string havuzAd;
        public int kullanimSayisi;
    }

    [Serializable()]
    public class SpGenelHavuzOrganizasyon
    {
        public string kod;
        public string tur;
        public string orgKod;

        public SpGenelHavuzOrganizasyon()
        {
            kod = string.Empty;
            tur = string.Empty;
            orgKod = string.Empty;
        }
    }

    [Serializable()]
    public class Donem
    {
        public int donemNo;
        public int baslamaYili;
        public int bitisYili;
        public bool pasif;
        public string pasifAciklama;
        public int versiyon;
        public string donemAd;
    }

    [Serializable()]
    public class GorevlendirmeEkip
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public int kod;
        public string ad;
        public Gorevlendirme[] gorevler;
    }

    [Serializable()]
    public class Gorevlendirme
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public int ekipKod;
        public int siraNo;
        public string personelAd;
        public string gorevAciklama;
        public string gorevYer;
        public string tel;
        public string eMail;
        //public string tcKimlikNo;
        //public TNSDateTime baslamaTarih;
        //public TNSDateTime bitisTarih;

        public bool BosMu()
        {
            if (!string.IsNullOrEmpty(personelAd))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class BirimMisyonVizyon
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public int tur;
        public int siraNo;
        public string ad;
        public string aciklama;
    }

    [Serializable()]
    public class KurumsalAnaliz
    {
        public Donem donem;
        public int orgBirimKod;
        public int tur;
        public int siraNo;
        public string metin;
    }

    [Serializable()]
    public class KurumsalAnalizSablon
    {
        public Donem donem;
        public int kod;
        public string ad;
        public KurumsalAnaliz[] detay;
    }

    [Serializable()]
    public class StratejiBelirleme
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int siraNo;
        public string stratejiAd;
        public string stratejiAciklama;

        public bool BosMu()
        {
            if (!string.IsNullOrEmpty(stratejiAd))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class BirimStratejikHedefMaliyet
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int yil;
        public double maliyet;

        public string temaKod;
        public string temaAd;
        public string amacKod;
        public string amacAd;
    }

    [Serializable()]
    public class BirimStratejikHedefMaliyetKatman
    {
        public string orgBirimAd;
        public string katmanKod;
        public string katmanAd;
        public BirimStratejikHedefMaliyetKatman[] altKatman;
        public int yil;
        public double maliyet;

        public double ToplamMaliyetBul(BirimStratejikHedefMaliyetKatman katman, int yil)
        {
            double sonuc = 0;
            if (katman.altKatman != null)
                foreach (var altKatman in katman.altKatman)
                    sonuc += ToplamMaliyetBul(altKatman, yil);
            else
                if (katman.yil == yil) sonuc += katman.maliyet;
            return sonuc;
        }
    }

    [Serializable()]
    public class Gzft
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string analizYontemi;
        public GzftDetay[] gzftKonuListe;
    }

    [Serializable()]
    public class GzftDetay
    {
        public Donem donem;
        public int orgBirimKod;
        public int tur;
        public int siraNo;
        public string aciklama;
        public int oncelikKod;
        public string sorunAlanKod;

        public bool BosMu()
        {
            if (!string.IsNullOrEmpty(aciklama))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class AsamaKontrol
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public int konuNo;
        public ENUMAsamaKontrolStatu statu;
        public TNSDateTime baslamaTarihi;
        public TNSDateTime tamamlanmaTarihi;
        public TNSDateTime islemTarihi;
        public int islemYapanBirim;
        public string islemYapan;
        public AsamaKontrolDetay[] askonKonuListe;

        public static string StatuAdBul(ENUMAsamaKontrolStatu statu)
        {
            return PerformansButceResI.ResourceManager.GetString("STR075_" + (int)statu);
        }

        public static ENUMAsamaKontrolStatu StatuBul(int statuId)
        {
            return (ENUMAsamaKontrolStatu)System.Enum.Parse(typeof(ENUMAsamaKontrolStatu), System.Enum.GetName(typeof(ENUMAsamaKontrolStatu), statuId));
        }

        public static ENUMAsamaKontrolStatu StatuBul(ENUMAsamaKontrolIslemTur islem)
        {
            if (islem == ENUMAsamaKontrolIslemTur.Kaydet)
                return ENUMAsamaKontrolStatu.DevamEdiyor;
            else if (islem == ENUMAsamaKontrolIslemTur.BirimOnay)
                return ENUMAsamaKontrolStatu.UstOnayda;
            else if (islem == ENUMAsamaKontrolIslemTur.BirimRed || islem == ENUMAsamaKontrolIslemTur.UstBirimRed)
                return ENUMAsamaKontrolStatu.Revizyonda;
            else if (islem == ENUMAsamaKontrolIslemTur.UstBirimOnay)
                return ENUMAsamaKontrolStatu.Tamamlandi;
            return ENUMAsamaKontrolStatu.Baslanmadi;
        }
    }

    [Serializable()]
    public class AsamaKontrolDetay
    {
        public Donem donem;
        public int orgBirimKod;
        public int konuNo;
        public int siraNo;
        public string kontrolListesi;
        public bool birimOnayi;
        public string birimAciklama;
        public bool ustBirimOnayi;
        public string ustBirimAciklama;

        public int birimOnayiSorguKriter;
        public int ustBirimOnayiSorguKriter;

        public AsamaKontrolDetay()
        {
            birimOnayiSorguKriter = -1;
            ustBirimOnayiSorguKriter = -1;
        }
    }

    [Serializable()]
    public class AsamaKontrolGecmis
    {
        public Donem donem;
        public int orgBirimKod;
        public int konuNo;
        public int siraNo;
        public string islem;
        public string islemiYapan;
        public TNSDateTime tarih;

        public AsamaKontrolGecmis()
        {
        }

        public AsamaKontrolGecmis(AsamaKontrol askon)
        {
            this.donem = askon.donem;
            this.orgBirimKod = askon.orgBirimKod;
            this.konuNo = askon.konuNo;
        }
    }

    [Serializable()]
    public class PerformansHamVeri
    {
        public int yil;
        public TNS.ORG.OrganizasyonBirim birim;
        public int bilgiTuru;
        public PerformansHamVeriDetay[] detay;
        public int birimToplam = 1;
    }

    [Serializable()]
    public class PerformansHamVeriDetay
    {
        public string anahtar1 = "-";
        public string anahtar2 = "-";
        public string anahtar3 = "-";
        public string anahtar4 = "-";
        public double bilgi = 0;
        public double bolen = 1;
    }

    [Serializable()]
    public class Pest
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string analizYontemi;
        public PestDetay[] pestKonuListe;
    }

    [Serializable()]
    public class PestDetay
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public int konuNo;
        public int siraNo;
        public string temelEgilimler;
        public string aciklama;
        public string iliski;

        public bool BosMu()
        {
            if (!string.IsNullOrEmpty(temelEgilimler))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class Birim_SpGenelHavuz
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public SpGenelHavuz spGenelHavuz;

        public string kod;
        public string ad;
        public Birim_SpGenelHavuz[] altHavuzlar;
    }

    [Serializable()]
    public class PaydasAnaliz
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string analizYontemi;
        public PaydasAnalizDetay[] paydasAnalizDetayListe;
    }

    [Serializable()]
    public class PaydasAnalizDetay
    {
        public int sinif;
        public Donem donem;
        public int orgBirimKod;
        public int siraNo;
        public string ad;
        public string aciklama;
        public string kategoriKod;
        public int oncelikKod;
        public int etkiKod;
        public int stratejiKod;

        public bool BosMu()
        {
            if (!string.IsNullOrEmpty(ad))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class Planlanan
    {
        public Donem donem;
        public int orgBirimKod;
        public PlanlananDetay[] planlananDetayListe;
    }

    [Serializable()]
    public class PlanlananDetay
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public int siraNo;
        public string calismaAdimi;
        public TNSDateTime baslangicTarihi;
        public TNSDateTime bitisTarihi;
        public TNSDateTime gerBaslangicTarihi;
        public TNSDateTime gerBitisTarihi;

        public bool BosMu()
        {
            if (!string.IsNullOrEmpty(calismaAdimi))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class Birim_SpGenelHavuz_Paydas_Iliski
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public SpGenelHavuz spGenelHavuz;
        public PaydasAnalizDetay paydas;
        public int yararlanma;
        public string aciklama;

        public bool BosMu()
        {
            if (spGenelHavuz != null && !string.IsNullOrEmpty(spGenelHavuz.kod) && paydas != null && paydas.siraNo > 0 && paydas.sinif > 0)
                return false;
            return true;
        }
    }

    [Serializable()]
    public class TarihselGelisim
    {
        public Donem donem;
        public int orgBirimKod;
        public int siraNo;
        public string aciklama;
    }

    [Serializable()]
    public class BirimHedef
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int sorumluBirimKod;
        public string sorumluBirimAd;
        public string aciklama;
        public TNSCollection koordineBirimler;
        public TNSCollection ustHedefler;
        public TNSCollection sorunAlanlar;
        public TNSCollection ustPolitikalar;
        public TNSCollection gzftler;
        public TNSCollection faaliyetler;
        public TNSCollection gostergeler;
    }

    [Serializable()]
    public class BirimHedefKoordineBirim
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int birimKod;
        public string birimAd;
        public int siraNo;
        public string aciklama;

        public override int GetHashCode()
        {
            return this.donem.donemNo + this.orgBirimKod + this.hedefKod.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            BirimHedef form = (BirimHedef)obj;
            if (form.donem.donemNo == this.donem.donemNo && form.orgBirimKod == this.orgBirimKod && form.hedefKod == this.hedefKod)
            {
                if (form.koordineBirimler == null)
                    form.koordineBirimler = new TNSCollection();

                form.koordineBirimler.Add(this);
                return true;
            }
            return false;
        }

        public bool BosMu()
        {
            if (birimKod > 0 || !string.IsNullOrEmpty(aciklama))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class BirimHedefUstHedef
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int birimKod;
        public string birimAd;
        public string ustHedefKod;
        public string ustHedefAd;

        public override int GetHashCode()
        {
            return this.donem.donemNo + this.orgBirimKod + this.hedefKod.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            BirimHedef form = (BirimHedef)obj;
            if (form.donem.donemNo == this.donem.donemNo && form.orgBirimKod == this.orgBirimKod && form.hedefKod == this.hedefKod)
            {
                if (form.ustHedefler == null)
                    form.ustHedefler = new TNSCollection();

                form.ustHedefler.Add(this);
                return true;
            }
            return false;
        }
    }

    [Serializable()]
    public class BirimHedefSorunAlan
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public string sorunAlanKod;
        public string sorunAlanAd;

        public override int GetHashCode()
        {
            return this.donem.donemNo + this.orgBirimKod + this.hedefKod.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            BirimHedef form = (BirimHedef)obj;
            if (form.donem.donemNo == this.donem.donemNo && form.orgBirimKod == this.orgBirimKod && form.hedefKod == this.hedefKod)
            {
                if (form.sorunAlanlar == null)
                    form.sorunAlanlar = new TNSCollection();

                form.sorunAlanlar.Add(this);
                return true;
            }
            return false;
        }
    }

    [Serializable()]
    public class BirimHedefUstPolitika
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public string ustPolitikaKod;
        public string ustPolitikaAd;

        public override int GetHashCode()
        {
            return this.donem.donemNo + this.orgBirimKod + this.hedefKod.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            BirimHedef form = (BirimHedef)obj;
            if (form.donem.donemNo == this.donem.donemNo && form.orgBirimKod == this.orgBirimKod && form.hedefKod == this.hedefKod)
            {
                if (form.ustPolitikalar == null)
                    form.ustPolitikalar = new TNSCollection();

                form.ustPolitikalar.Add(this);
                return true;
            }
            return false;
        }
    }

    [Serializable()]
    public class BirimHedefGzft
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public int gzftSiraNo;
        public int gzftTur;
        public string gzftAciklama;

        public override int GetHashCode()
        {
            return this.donem.donemNo + this.orgBirimKod + this.hedefKod.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            BirimHedef form = (BirimHedef)obj;
            if (form.donem.donemNo == this.donem.donemNo && form.orgBirimKod == this.orgBirimKod && form.hedefKod == this.hedefKod)
            {
                if (form.gzftler == null)
                    form.gzftler = new TNSCollection();

                form.gzftler.Add(this);
                return true;
            }
            return false;
        }
    }

    [Serializable()]
    public class BirimHedefFaaliyet
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public string faaliyetKod;
        public string faaliyetAd;

        public override bool Equals(object obj)
        {
            BirimHedef form = (BirimHedef)obj;
            if (form.donem.donemNo == this.donem.donemNo && form.orgBirimKod == this.orgBirimKod && form.hedefKod == this.hedefKod)
            {
                if (form.faaliyetler == null)
                    form.faaliyetler = new TNSCollection();

                form.faaliyetler.Add(this);
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.donem.donemNo + this.orgBirimKod + this.hedefKod.GetHashCode();
        }
    }

    [Serializable()]
    public class MevzuatAnaliz
    {
        public Donem donem;
        public int orgBirimKod;
        public int siraNo;
        public string aciklama;
    }

    [Serializable()]
    public class BirimFaaliyetAgirlik
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string faaliyetKod;
        public string hedefKod;
        public string hedefAd;
        public double agirlik;
    }

    [Serializable()]
    public class Birim_AmacHedef
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string amacHedefKod;
        public string temaAmacKod;
        public string temaAmacAd;
        public string aciklama;
        public string sira;
        public string sorunAlanKod;
        public string sorunAlanAd;
        public Birim_AmacHedef[] hedefler;

        public bool BosMu()
        {
            if (!string.IsNullOrEmpty(aciklama))
                return false;
            return true;
        }
    }

    [Serializable()]
    public class BirimFaaliyetMaliyet
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string faaliyetKod;
        public string faaliyetAd;
        public int yil;
        public string ekoKod;
        public string ekoAd;
        public int maliyetTur;
        public double maliyet;
    }

    [Serializable()]
    public class RTFOutline
    {
        public int kod;
        public int sira;
        public string metin;
        public int icerlek;
        public string tur;
        public string varsayilan;
        public int merkezTasra;

        public RTFOutline()
        {
            kod = -1;
            sira = -1;
            metin = "";
            icerlek = 1;
        }

        public RTFOutline(int kod, int sira, string metin, int icerlek)
        {
            this.kod = kod;
            this.sira = sira;
            this.metin = metin;
            this.icerlek = icerlek;
        }
    }

    [Serializable()]
    public class STRGosterge
    {
        public int kod;
        public string ad;
        public int birim;
        public int basarimTuru;
        public int gostergeTuru;
        public string hesaplamaYontemi;
        public string veriKaynagi;
        public string veriKaynagiSistemi;
        public string ekParam;
        public int veriToplamaPeriyodu;
        public int raporlamaPeriyodu;
        public string aciklama;
        public int durum = (int)EnumSPGenelHavuzDurum.Hepsi;
        public TNSDateTime baslangicTarihi;
        public TNSDateTime bitisTarihi;

        public STRGostergeBag[] gostergeBag;
        public int girdiCikti;

        public override int GetHashCode()
        {
            return kod;
        }

        public override bool Equals(object obj)
        {
            int tut = CompareTo(obj);
            return tut == 0;
        }

        public int CompareTo(object obj)
        {
            int kod = 0;
            if (obj.GetType() == typeof(STRGosterge))
            {
                kod = ((STRGosterge)obj).kod;
            }
            else if (obj.GetType() == typeof(int))
            {
                kod = (int)obj;
            }
            else
                return -1;
            return this.kod.CompareTo(kod);
        }

        public string DegiskenAciklamaBul(UZY.IUZYServis uzy)
        {
            return uzy.UzayDegeriStr(null, "STRGOSTERGE", this.kod.ToString(), true);
        }
    }

    [Serializable()]
    public class STRGostergeBag
    {
        public int gostergeKod;
        public string bagliKod;
        public string bagliAd;
    }

    [Serializable()]
    public class BirimHedefGosterge
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int gostergeKod;
        public string gostergeAd;
        public int yil;
        public decimal hedeflenenDeger;

        public override bool Equals(object obj)
        {
            BirimHedef form = (BirimHedef)obj;
            if (form.donem.donemNo == this.donem.donemNo && form.orgBirimKod == this.orgBirimKod && form.hedefKod == this.hedefKod)
            {
                if (form.gostergeler == null)
                    form.gostergeler = new TNSCollection();

                form.gostergeler.Add(this);
                return true;
            }
            return false;
        }


        public override int GetHashCode()
        {
            return this.donem.GetHashCode() + this.gostergeKod;
        }
    }

    public enum ENUMKarsilastirmaGrubu
    {
        KURUM = 1, ILCE = 2, IL = 3, TANIMSIZ = 0
    }

    public enum ENUMCografiBirim
    {
        ILCE = 1, IL = 2, ULKE = 3, TANIMSIZ = 0
    }

    //[Serializable()]
    //public class KarsilastirmaGrubu
    //{
    //    public int kod = -1;
    //    public string metin = null;
    //    public ObjectArray orgTurler = null;
    //    public ENUMCografiBirim cografiBirim = ENUMCografiBirim.TANIMSIZ;
    //    public ENUMKarsilastirmaGrubu karsilastirmaGrubu = ENUMKarsilastirmaGrubu.TANIMSIZ;

    //    public KarsilastirmaGrubu(int kod, string metin)
    //    {
    //        this.kod = kod;
    //        this.metin = metin;
    //    }
    //}

    [Serializable()]
    public class DegiskenSablon
    {
        public int kod;
        public string metin;
        public STRGosterge[] girdiler = null;
        public STRGosterge[] ciktilar = null;

        public DegiskenSablon(int kod, string metin)
        {
            this.kod = kod;
            this.metin = metin;
        }

        public ObjectArray orgTurler = null;
    }

    //[Serializable()]
    //public class GostergeTanim : Object
    //{
    //    public int kod;
    //    public string metin;

    //    /// <summary>
    //    /// Bilgi Turu: Bilgi turu
    //    /// </summary>
    //    public int bilgiTuru
    //    {
    //        get
    //        {
    //            if (kod == 1)
    //                return 2;
    //            else if (kod == 2)
    //                return 3;
    //            else if (kod == 3)
    //                return 4;
    //            else if (kod == 4 || kod == 6 || kod == 8 || kod == 9 || kod == 10 || kod == 11 || kod == 12 || kod == 13 || kod == 14 || kod == 15 || kod == 16 || kod == 17 || kod == 18 || kod == 19 || kod == 20 || kod == 21)
    //                return 1;
    //            else if (kod == 5)
    //                return 5;
    //            return 5;
    //        }
    //        set
    //        {
    //            _bilgiTuru = value;
    //        }
    //    }
    //    private int _bilgiTuru;

    //    /// <summary>
    //    /// Anantar 1: Arama anahtarı giriniz
    //    /// </summary>
    //    public string anahtar1
    //    {
    //        get
    //        {
    //            if (kod == 4) return "03";
    //            else if (kod == 6) return "14";
    //            else if (kod == 7) return "15";
    //            else if (kod == 8) return "16";
    //            else if (kod == 9) return "17";
    //            else if (kod == 10) return "8";
    //            else if (kod == 11) return "9";
    //            else if (kod == 12) return "12";
    //            else if (kod == 13) return "13";
    //            else if (kod == 14) return "18";
    //            else if (kod == 15) return "19";
    //            else if (kod == 16) return "20";
    //            else if (kod == 17) return "21";
    //            else if (kod == 18) return "22";
    //            else if (kod == 19) return "23";
    //            else if (kod == 20) return "24";
    //            else if (kod == 21) return "25";
    //            else if (kod == 22) return "26";
    //            return "";
    //        }
    //        set
    //        {
    //            _anahtar1 = value;
    //        }
    //    }
    //    public string _anahtar1;

    //    /// <summary>
    //    /// Anantar 2: Arama anahtarı giriniz
    //    /// </summary>
    //    public string anahtar2 { get { return ""; } set { _anahtar2 = value; } }
    //    public string _anahtar2;

    //    /// <summary>
    //    /// Anantar 3: Arama anahtarı giriniz
    //    /// </summary>
    //    public string anahtar3 { get { return ""; } set { _anahtar3 = value; } }
    //    public string _anahtar3;

    //    /// <summary>
    //    /// Anantar 4: Arama anahtarı giriniz
    //    /// </summary>
    //    public string anahtar4 { get { return ""; } set { _anahtar4 = value; } }
    //    public string _anahtar4;

    //    public GostergeTanim(int kod, string metin)
    //    {
    //        // TODO: Complete member initialization
    //        this.kod = kod;
    //        this.metin = metin;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        int tut = CompareTo(obj);
    //        return tut == 0;
    //    }

    //    public int CompareTo(object obj)
    //    {
    //        int kod = 0;
    //        if (obj.GetType() == typeof(GostergeTanim))
    //        {
    //            kod = ((GostergeTanim)obj).kod;
    //        }
    //        else if (obj.GetType() == typeof(int))
    //        {
    //            kod = (int)obj;
    //        }
    //        else
    //            return -1;
    //        return this.kod.CompareTo(kod);
    //    }
    //}

    [Serializable()]
    public class BirimIsGorevMaliyet
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string isGorevKod;
        public string isGorevAd;
        public int yil;
        public double maliyet;
        public string faaliyetKod;
        public string faaliyetAd;
    }

    [Serializable()]
    public class BirimHedefGostergeYillik
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int gostergeKod;
        public string gostergeAd;
        public int yil;
        public decimal hedeflenenDeger;
        public decimal tahminDeger;
    }

    [Serializable()]
    public class VZAIslemSonuc
    {
        public string kod;
        public string metin;
        public int durum;
    }

    [Serializable()]
    public class VZAAnalizDetay
    {
        public int kod = -1;
        public string metin = null;
        public int degiskenKod = -1;
        public ENUMCografiBirim kgKod = ENUMCografiBirim.TANIMSIZ;
        public string[] cografiKodlar = null;
        public int durumKod = -1;
        public VZAIslemSonuc[] IslenenYerler;
        public int cozumYontem;
        public VZAAnalizDetay(int kod, string metin)
        {
            this.kod = kod;
            this.metin = metin;
        }
    }

    [Serializable()]
    public class VZAAnalizGrup
    {
        public int kod;
        public string metin;
        public int varsayilan;

        public VZAAnalizDetay[] analizDetaylar = null;

        public VZAAnalizGrup(int kod, string metin)
        {
            // TODO: Complete member initialization
            this.kod = kod;
            this.metin = metin;
        }
    }

    [Serializable]
    public class VZAIslemBilgi
    {
        public object kod;
        public object metin;
    }

    [Serializable]
    public class VZASonucDegisken
    {
        public int gostergeKod;
        public double girisDeger;
        public double cikisDeger;
        public double oran;
        public string gostergeAd = null;
    }

    [Serializable]
    public class VZASonucBirim
    {
        public int orgKod;
        public string orgAd;
        public double amac;
        public int ornekBirimSay;
        public ArrayList degiskenler;
    }

    [Serializable]
    public class VZASonucBilgi
    {
        public ArrayList gostergeler;
        public ArrayList sonucBirimler;
    }

    [Serializable()]
    public class BirimGostergeGerceklesen
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public int gostergeKod;
        public string gostergeAd;
        public int yil;
        public int ay;
        public decimal gerceklesenDeger;
    }

    [Serializable()]
    public class BirimGostergeGerceklesmeDurum
    {
        public int basariliGerceklesmeSayi;
        public int basarisizGerceklesmeSayi;
        public BirimGostergeGerceklesmeDurumDetay[] detay;
    }

    [Serializable()]
    public class BirimGostergeGerceklesmeDurumDetay
    {
        public int gostergeKod;
        public string gostergeAd;
        public string hedefKod;
        public string hedefAd;
        public decimal hedeflenen;
        public decimal gerceklesen;
        public bool basarili;
    }

    [Serializable()]
    public class BirimPerHedef
    {
        public Donem donem;
        public int orgBirimKod;
        public string orgBirimAd;
        public string hedefKod;
        public string hedefAd;
        public int yil;
        public string perHedef;
        public BirimHedefGostergeYillik[] gostergeler;
    }

    /// <summary>
    /// SPALTFAALIYET: Alt Faaliyetler
    /// </summary>
    [System.Serializable()]
    public class STRAltFaaliyet
    {
        public STRAltFaaliyet()
        {
            sPALTFAALIYETBIRIM = new TNSCollection();
        }

        /// <summary>
        /// ALTFAALNO: Alt Faaliyet Numarası
        /// </summary>
        public int aLTFAALNO { get; set; }

        /// <summary>
        /// ALTFAALKODU: Alt Faaliyet Kodu
        /// </summary>
        public string aLTFAALKODU { get; set; }

        /// <summary>
        /// ALTFAALADI: Alt Faaliyet Adı
        /// </summary>
        public string aLTFAALADI { get; set; }

        /// <summary>
        /// SPALTFAALIYETBIRIM: Birimler
        /// </summary>
        public TNSCollection sPALTFAALIYETBIRIM;

        public String sorumluBirim
        {
            get
            {
                return DizidenAl(sPALTFAALIYETBIRIM, 0, 1);
            }
            set
            {
                DiziyeAta(sPALTFAALIYETBIRIM, 0, value);
            }
        }

        public object sorumluBirim2
        {
            get
            {
                return DizidenAl(sPALTFAALIYETBIRIM, 0, 1, true, ", ");
            }
        }

        public String ilgiliBirimler
        {
            get
            {
                return DizidenAl(sPALTFAALIYETBIRIM, 1, -1);
            }
            set
            {
                string[] naneler = value.Split(',');
                for (int i = 0; i < naneler.Length; i++)
                {
                    DiziyeAta(sPALTFAALIYETBIRIM, i + 1, naneler[i]);
                }
            }
        }

        public object ilgiliBirimler2
        {
            get
            {
                return DizidenAl(sPALTFAALIYETBIRIM, 1, -1, true, ", ");
            }
        }

        private void DiziyeAta(TNSCollection dizi, int yer, string deger)
        {
            if (!string.IsNullOrWhiteSpace(deger))
            {
                string p1 = "", p2 = "";
                OrtakFonksiyonlar.StringiIkiyeBol(deger, "-", ref p1, ref p2);
                DiziyeAta(dizi, yer, p1, p2);
            }
        }

        private void DiziyeAta(TNSCollection dizi, int yer, string kod, string ad)
        {
            if (yer >= dizi.Count)
            {
                for (int i = 0; i < yer - dizi.Count + 1; i++)
                {
                    dizi.Add((TNS.BKD.ABSKod)null);
                }
            }
            TNS.BKD.ABSKod k = new ABSKod(kod);
            k.AdAta(ad);
            dizi[yer] = k;
        }

        private string DizidenAl(TNSCollection dizi, int basla, int bit, bool sadeceAd = false, string ayrac = ",")
        {
            string donusDeger = "";
            if (dizi != null)
            {
                int son = dizi.Count;
                if (bit == -1 || bit > son) bit = son;
                for (int i = basla; i < bit; i++)
                {
                    TNS.BKD.ABSKod k = (TNS.BKD.ABSKod)dizi[i];
                    if (donusDeger != "") donusDeger += ayrac;
                    if (sadeceAd)
                        donusDeger += k.ad;
                    else
                        donusDeger += k.kodAd;
                }
            }
            return donusDeger;
        }

        public TNSDateTime baslaTarih { get; set; }
        public DateTime baslaTarihDT
        {
            get
            {
                if (baslaTarih == null) baslaTarih = new TNSDateTime();
                return baslaTarih.Oku();
            }
            set
            {
                if (baslaTarih == null) baslaTarih = new TNSDateTime();
                baslaTarih.Yaz(value);
            }
        }

        public TNSDateTime bitisTarih { get; set; }
        public DateTime bitisTarihDT
        {
            get
            {
                if (bitisTarih == null) bitisTarih = new TNSDateTime();
                return bitisTarih.Oku();
            }
            set
            {
                if (bitisTarih == null) bitisTarih = new TNSDateTime();
                bitisTarih.Yaz(value);
            }
        }
    }

    /// <summary>
    /// SPFaaliyet: Stratejik Faaliyet
    /// </summary>
    [System.Serializable()]
    public class STRFaaliyet
    {
        public STRFaaliyet()
        {
            sPALTFAALIYET = new TNSCollection();
        }

        /// <summary>
        /// DONEMBYIL: Stratejik Plan Başlama Yılı
        /// </summary>
        public int dONEMBYIL { get; set; }

        /// <summary>
        /// HEDEFKOD: Hedef Kodu
        /// </summary>
        public int hEDEFKOD { get; set; }

        /// <summary>
        /// KURUMKOD: Hedef Sahibi Kurum
        /// </summary>
        public string kURUMKOD { get; set; }

        /// <summary>
        /// FAALNO: Faaliyet Numarası
        /// </summary>
        public int fAALNO { get; set; }

        /// <summary>
        /// FAALKODU: Faaliyet Kodu
        /// </summary>
        public string fAALKODU { get; set; }

        /// <summary>
        /// FAALADI: Faaliyet Adı
        /// </summary>
        public string fAALADI { get; set; }

        /// <summary>
        /// SPALTFAALIYET: Alt Faaliyetler
        /// </summary>
        public TNSCollection sPALTFAALIYET;

        public int yil { get; set; }
    }
    public enum ENUMTARIHCEISLEMTIP
    {
        KAYDET = 1,
        GUNCELLE = 2,
        SIL = 3

    }
    public enum ENUMTARIHCE
    {
        ISGOREVDETAY = 1,
        ISGOREVZAMANLAMA = 2,
        ISGOREVSORUMLULAR = 3,
        ISGOREVPERFORMANSGOSTERGE = 4,
    }

    [Serializable()]
    public class IsGorevTarihce
    {
        public int kod;
        public string kullanici;
        public TNSDateTime tarih;
        public string degisiklik;
        public int yil;
        public string kurKod;
    }

    /// <summary>
    /// EylemGrubu: 
    /// </summary>
    [System.Serializable()]
    public class EylemGrubu
    {
        /// <summary>
        /// Sıra No: Sıra No
        /// </summary>
        public int siraNo { get; set; }

        /// <summary>
        /// Adı: Grup Adı
        /// </summary>
        public string adi { get; set; }
    }


    /// <summary>
    /// EylemGerceklesme: 
    /// </summary>
    [System.Serializable()]
    public class EylemGerceklesme
    {
        public string olcuBirimAd { get; set; }

        /// <summary>
        /// Yıl: Yıl
        /// </summary>
        public int yil { get; set; }

        /// <summary>
        /// Ay: Ay
        /// </summary>
        public int ay { get; set; }

        public string ayAd
        {
            get
            {
                return OrtakFonksiyonlar.AyAdiGetir(ay);
            }
        }

        /// <summary>
        /// Gerçekleşme: Gerçekleşme
        /// </summary>
        public double gerceklesme { get; set; }

        public double miktar { get; set; }
        public string planlanan { get; set; }
        public string gerceklesen { get; set; }
        public string sorunlar { get; set; }
        public string aciklamalar { get; set; }
    }

    /// <summary>
    /// EylemPlani: 
    /// </summary>
    [System.Serializable()]
    public class EylemTarihce
    {
        public string kullaniciKodu { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string telefon { get; set; }
        public string aciklama { get; set; }

        public string islemiYapan
        {
            get
            {
                return ad + " " + soyad + " (" + telefon + ")";
            }
        }

        /// <summary>
        /// Bitiş Tarihi: Eylemin İlgili Birimleri
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
        /// Eylem ID: Eylem Planı IDsi
        /// </summary>
        public int eylemID { get; set; }
    }

    /// <summary>
    /// EylemPlani: 
    /// </summary>
    [System.Serializable()]
    public class EylemPlani
    {
        /// <summary>
        /// Eylem ID: Eylem Planı IDsi
        /// </summary>
        public int eylemID { get; set; }

        /// <summary>
        /// Eylem Adı: Eylem adını giriniz.
        /// </summary>
        public string eylemAdi { get; set; }

        /// <summary>
        /// Eylem Türü: 1- SPye bağlı, 2-Üst Yönetim.
        /// </summary>
        public int eylemTuru { get; set; }

        /// <summary>
        /// Yürüten Birim: Eylemi Yürüten Birim
        /// </summary>
        public string yurutenBirim { get; set; }

        /// <summary>
        /// Başlama Tarihi: Eylemin İlgili Birimleri
        /// </summary>
        public TNSDateTime baslamaTarihi { get; set; }
        public DateTime baslamaTarihiDT
        {
            get
            {
                if (baslamaTarihi == null)
                    return DateTime.MinValue;
                else
                    return baslamaTarihi.Oku();
            }
            set
            {
                if (baslamaTarihi == null) baslamaTarihi = new TNSDateTime(value);
                else baslamaTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Bitiş Tarihi: Eylemin İlgili Birimleri
        /// </summary>
        public TNSDateTime bitisTarihi { get; set; }
        public DateTime bitisTarihiDT
        {
            get
            {
                if (bitisTarihi == null)
                    return DateTime.MinValue;
                else
                    return bitisTarihi.Oku();
            }
            set
            {
                if (bitisTarihi == null) bitisTarihi = new TNSDateTime(value);
                else bitisTarihi.Yaz(value);
            }
        }

        /// <summary>
        /// Donem B Yıl: SP Gösterge Dönemi
        /// </summary>
        public int donemBYil { get; set; }

        /// <summary>
        /// Amaç Kod: SP Gösterge Hedefi
        /// </summary>
        public int amacKod { get; set; } = 0;

        /// <summary>
        /// Hedef Kod: SP Gösterge Hedefi
        /// </summary>
        public int hedefKod { get; set; } = 0;

        /// <summary>
        /// Kurum Kod: SP Kurum Kodu.
        /// </summary>
        public string kurumKod { get; set; }

        /// <summary>
        /// Gösterge Kod: SP Gösterge Kodu
        /// </summary>
        public int gostergeKod { get; set; } = 0;

        /// <summary>
        /// Gösterge Kod: SP Gösterge Kodu
        /// </summary>
        public int durum { get; set; } = 0;
        public int donemDurum { get; set; } = 0;

        /// <summary>
        /// Eylemle ilgili birimler 
        /// </summary>
        public TNSCollection ilgiliBirimler = new TNSCollection();

        /// <summary>
        /// EylemGerceklesme: 
        /// </summary>
        public TNSCollection eylemGerceklesme = new TNSCollection();
        public string olcuBirimAd { get; set; } = "";

        public string hedefBirimAd { get; set; } = "";
        public string hedefBirimKisaAd { get; set; } = "";

        public string yurutenBirimAd { get; set; } = "";
        public string yurutenBirimKisaAd { get; set; } = "";
        public string hedefKisa { get; set; } = "";
        public string hedefAd { get; set; } = "";
        public string amacKisa { get; set; } = "";
        public string amacAd { get; set; } = "";
        public string gostergeKisa { get; set; } = "";
        public string gostergeAd { get; set; } = "";

        public int agirlik { get; set; } = 0;
        public int gerceklesme { get; set; } = 0;
        public double miktar { get; set; } = 0.0;
        /// <summary>
        /// Eylem Adı: Eylem adını giriniz.
        /// </summary>
        public string eylemAmac { get; set; }
        public double gostergeBaslangic { get; set; } = 0.0;
        public double yilTahmin { get; set; } = 0.0;
        public int gostergeTur { get; set; } = 0;

        public EylemGerceklesme GerceklesmeBul(int yil, int ayNo, bool tamAy)
        {
            EylemGerceklesme yakin = null;
            foreach (EylemGerceklesme eg in eylemGerceklesme)
            {
                if (eg.yil == yil)
                {
                    if (eg.ay == ayNo) return eg;
                    if (eg.ay > ayNo) continue;
                    if (yakin == null) yakin = eg;
                    else if (yakin.ay < eg.ay) yakin = eg;
                }
            }
            if (tamAy) return null;
            if (yakin == null && eylemGerceklesme.Count > 0) yakin = (EylemGerceklesme)eylemGerceklesme[0];
            return yakin;
        }
    }
}