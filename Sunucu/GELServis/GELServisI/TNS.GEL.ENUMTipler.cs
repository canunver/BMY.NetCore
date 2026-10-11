using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace TNS.GEL
{
    public enum ENUMButceTuru
    {
        [Description(" ")]
        TANIMSIZ = 0,
        [Description("Özel Bütçe")]
        OZELBUTCE = 1,
        [Description("Dss")]
        DSS = 2
    }
    public enum ENUMListelemeSekli
    {
        TEKLI = 1,
        COKLU = 2
    }
    public enum ENUMKayitDurum
    {
        TANIMSIZ = 0,
        YENIKAYIT = 1,
        GUNCELLENDI = 2,
        SILINDI = 3
    }

    public enum ENUMSurecParcaDurum
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        TANIMSIZ = 0,
        /// <summary>
        /// Başladı
        /// </summary>
        BASLADI = 1,
        /// <summary>
        /// Devam Ediyor
        /// </summary>
        DEVAMEDIYOR = 2,
        /// <summary>
        /// Tamamlandı
        /// </summary>
        TAMAMLANDI = 3,
    }

    public enum ENUMIsTuru
    {
        TANIMSIZ = 0,
        TASINMAZKIRALAMA = 1
    }

    public enum ENUMDosyaTuru
    {
        TANIMSIZ = 0,
        TASNIMAZTALEPEKI = 1
    }

    public enum ENUMIslemTuru
    {
        KAYIT = 1,
        SILME = 2,
        LISTELEME = 3,
        KAYITKONTROL = 4,
        GUNCELLEME = 5
    }

    public enum ENUMKomisyonGorevi
    {
        Belirsiz = 0,
        Asil = 1,
        Yedek = 2,
        Baskan = 3,
        YedekBaskan = 4
    }

    public enum ENUMIhaleIslem
    {
        TANIMSIZ = 0,
        KIRALAMA = 1
    }

    public enum ENUMOGY
    {
        /// <summary>
        /// Onaylama
        /// </summary>
        Onaylama = 1,
        /// <summary>
        /// Görme
        /// </summary>
        Gorme = 2,
        /// <summary>
        /// Yazma
        /// </summary>
        Yazma = 3,
        /// <summary>
        /// Onay Kaldır
        /// </summary>
        OnayKaldir = 5,
        /// <summary>
        /// İptal
        /// </summary>
        Iptal = 6,
    }

    public enum ENUMTalepSonuc
    {
        /// <summary>
        /// Tanımsız
        /// </summary>
        Tanimsiz = 0,

        /// <summary>
        /// OnayAsamasinda
        /// </summary>
        OnayAsamasinda = 1,

        /// <summary>
        /// Onaylandi
        /// </summary>
        Onaylandi = 2,

        /// <summary>
        /// RedEdildi
        /// </summary>
        RedEdildi = 3
    }

    public enum ENUMModulTuru
    {
        Tanimsiz = 0,
        Harcama = 1,
        Gelir = 2

    }

    public enum ENUMIHALETeklifTuru
    {
        TANIMSIZ = 0,
        GOTURUBEDEL = 1,
        BIRIMFIYAT = 2,
        KISMITEKLIF = 3,
        PROTOKOL = 4
    }

    public enum ENUMSureTuru
    {
        TANIMSIZ = 0,
        AYLIK = 1,
        YILLIK = 2
    }

    public enum ENUMTasinmazKayittanCikisTuru
    {
        [Description(" ")]
        TANIMSIZ = 0,
        [Description("Satış")]
        SATIS = 1,
        [Description("Tahsis İptal")]
        TAHSISIPTAL = 2,
        [Description("Devir")]
        DEVIR = 3
    }

    public enum ENUMTasinmazEdinmeSekli
    {
        [Description(" ")]
        TANIMSIZ = 0,
        
        [Description("Kira")]
        KIRA = 1,
        
        [Description("İrtifak")]
        IRTIFAK = 2,
        
        [Description("Tahsis")]
        TAHSIS = 3,
        
        [Description("Tapulama")]
        TAPULAMA = 4,
        
        [Description("İfran")]
        IFRAN = 5,
        
        [Description("Tevhid")]
        TEVHID = 6,
        
        [Description("Zayiinden")]
        ZAYIINDEN = 7,
        
        [Description("Satış")]
        SATIS = 8,

        [Description("Kamulaştırma")]
        KAMULASTIRMA = 9,

        [Description("Bağış")]
        BAGIS = 10,

        [Description("İmar")]
        IMAR = 11,
    }


}
