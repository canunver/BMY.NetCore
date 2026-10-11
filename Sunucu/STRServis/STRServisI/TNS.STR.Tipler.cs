using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.STR
{
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
        /// Parametresiz Misyon nesnesi oluşturucu yordam
        /// </summary>
        public Misyon()
        {
            donemBYil = 0;
            dil = new Dil();
            misyon = string.Empty;
            aciklama = string.Empty;
            dosya = string.Empty;
            ebutceKod = string.Empty;
        }

        /// <summary>
        /// İlgili parametreleri alarak Misyon nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="donemBYil">Stratejik plan dönem başlangıç yılı</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <param name="misyon">Misyon tanımı</param>
        /// <param name="aciklama">Misyon açıklaması</param>
        /// <param name="dosya">Dosya adı</param>
        public Misyon(int donemBYil, Dil dil, string misyon, string aciklama, string dosya)
        {
            this.donemBYil = donemBYil;
            this.dil = dil;
            this.misyon = misyon;
            this.aciklama = aciklama;
            this.dosya = dosya;
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
        }

        /// <summary>
        /// İlgili parametreleri alarak Vizyon nesnesi oluşturucu yordam
        /// </summary>
        /// <param name="donemBYil">Stratejik plan dönem başlangıç yılı</param>
        /// <param name="dil">Dil bilgisini tutan nesne</param>
        /// <param name="vizyon">Vizyon tanımı</param>
        /// <param name="aciklama">Vizyon açıklaması</param>
        /// <param name="dosya">Dosya adı</param>
        public Vizyon(int donemBYil, Dil dil, string vizyon, string aciklama, string dosya)
        {
            this.donemBYil = donemBYil;
            this.dil = dil;
            this.vizyon = vizyon;
            this.aciklama = aciklama;
            this.dosya = dosya;
        }
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
        public Tema(int donemBYil, int temaKod, string temaKisa, string tema, string aciklama, string dosya, Dil dil)
        {
            this.donemBYil = donemBYil;
            this.temaKod = temaKod;
            this.temaKisa = temaKisa;
            this.tema = tema;
            this.aciklama = aciklama;
            this.dosya = dosya;
            this.dil = dil;
        }
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
        public StratejikAmac(int donemBYil, int strAmacKod, string strAmacKisa, string strAmac, string aciklama, string dosya, Tema tema, Dil dil)
        {
            this.donemBYil = donemBYil;
            this.strAmacKod = strAmacKod;
            this.strAmacKisa = strAmacKisa;
            this.strAmac = strAmac;
            this.aciklama = aciklama;
            this.dosya = dosya;
            this.tema = tema;
            this.dil = dil;
        }
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

        /// <summary>
        /// Parametresiz Stratejik Hedef nesnesi oluşturucu yordam
        /// </summary>
        public StratejikHedef()
        {
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
        }
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
        }
    }
}