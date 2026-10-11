using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.ATM;
namespace TNS.ATM.IsKurallari.Veri
{
    /*
     * 
     * 
     CREATE TABLE ATMDOSYAABBORCLU (
	IPARDDONEMI int NOT NULL,
	DOSYAANAHTAR int NOT NULL,
	SIRA int NOT NULL,
	BASTARIH datetime NOT NULL,
	BITTARIH datetime NULL,
	DURUM int NOT NULL,

	DEFTERSIRASI varchar(50) NOT NULL,
	TEDBIR varchar(20) NOT NULL,
	ALTTEDBIR varchar(20) NOT NULL,
	BUTCEKALEMI varchar(20) NOT NULL,
	SAYMANLIKKOD int NOT NULL,
	BIRIMKOD varchar(200) NOT NULL,
	BORCLUKOD int NOT NULL,
	BASVURUKAYITNUMARASI varchar(30) NULL,
	SOZLESMENUMARASI varchar(30) NULL,

	KAYDEDEN varchar(30) NOT NULL,
	KAYITTARIHI datetime NOT NULL,
	ONAYLAYAN varchar(30) NULL,
	ONAYTARIHI datetime NULL,
	GEREKCE varchar(200) NOT NULL,

	PRIMARY KEY
	(
		IPARDDONEMI ASC,
		DOSYAANAHTAR ASC
	)
)
     * 
     * 
INSERT INTO ATMDOSYAABBORCLU
SELECT 	IPARDDONEMI,
        DOSYAANAHTAR,
        1 as SIRA,
        xONBORCTARIHI BASTARIH,
        NULL BITTARIH,
        5 DURUM,

	DEFTERSIRASI,
	TEDBIR,
	ALTTEDBIR,
	BUTCEKALEMI,
	SAYMANLIKKOD,
	BIRIMKOD,
	BORCLUKOD,
	BASVURUKAYITNUMARASI,
	SOZLESMENUMARASI

        , 'stratek' KAYDEDEN, xONBORCTARIHI KAYITTARIHI, 'stratek' ONAYLAYAN, xONBORCTARIHI ONAYTARIHI, 'AÇILIŞ' GEREKCE
    from ATMDOSYAAB
	where xONBORCTARIHI is not null
order by xONBORCTARIHI

     * 
     * 
     * 
     * 
     * 
     UPDATE ATMDOSYAABTAKSIT --ATMDOSYAABDURUM, ATMDOSYAABGERIALIM, ATMDOSYAABONBORC
set KAYITTARIHI=BASTARIH,  ONAYTARIHI=BASTARIH,  KAYDEDEN='stratek',  ONAYLAYAN='stratek'


        CREATE TABLE ATMDOSYAABDURUM (
      IPARDDONEMI int NOT NULL,
      DOSYAANAHTAR int NOT NULL,
      SIRA int NOT NULL,
      BASTARIH DATETIME NOT NULL,
      BITTARIH DATETIME NULL,
      DURUM INT NOT NULL

    , SURECASAMASI INT NOT NULL
    , YORUMLARTR VARCHAR(2000)
    , YORUMLAREN VARCHAR(2000)

    ,KAYDEDEN varchar(30), KAYITTARIHI DATETIME, ONAYLAYAN varchar(30), ONAYTARIHI DATETIME, GEREKCE VARCHAR(200)
    PRIMARY KEY (IPARDDONEMI ASC, DOSYAANAHTAR ASC, SIRA ASC))

 INSERT INTO ATMDOSYAABDURUM
SELECT 	IPARDDONEMI,
        DOSYAANAHTAR,
        1 as SIRA,
        ONBORCTARIHI BASTARIH,
        NULL BITTARIH,
        5 DURUM,

        SURECASAMASI SURECASAMASI,
        YORUMLARTR YORUMLARTR,
        YORUMLAREN YORUMLARTR
        , 'stratek' KAYDEDEN, NULL KAYITTARIHI, 'stratek' ONAYLAYAN, NULL ONAYTARIHI, 'AÇILIŞ' GEREKCE
    from ATMDOSYAAB
order by ONBORCTARIHI

     * 
     CREATE TABLE ATMDOSYAABTAKSIT (
      IPARDDONEMI int NOT NULL,
      DOSYAANAHTAR int NOT NULL,
      SIRA int NOT NULL,
      BASTARIH DATETIME NOT NULL,
      BITTARIH DATETIME NULL,
      DURUM INT NOT NULL

    , TAKSITTALEPTARIHI DATETIME
    , TAKSITTALEPSAYI VARCHAR(50)
    , TAKSITKARARTARIHI DATETIME
    , TAKSITKARARSAYI VARCHAR(50)
    , TAKSITBASTARIHI DATETIME
    , TAKSITSAYISI INT
    ,KAYDEDEN varchar(30), KAYITTARIHI DATETIME, ONAYLAYAN varchar(30), ONAYTARIHI DATETIME, GEREKCE VARCHAR(200)
    PRIMARY KEY (IPARDDONEMI ASC, DOSYAANAHTAR ASC, SIRA ASC))
 
 INSERT INTO ATMDOSYAABTAKSIT
    SELECT 	IPARDDONEMI,
        DOSYAANAHTAR,
        1 as SIRA,
        TAKSITTALEPTARIHI BASTARIH,
        NULL BITTARIH,
        5 DURUM,

        TAKSITTALEPTARIHI TAKSITTALEPTARIHI,
        TAKSITTALEPSAYI TAKSITTALEPSAYI,
        TAKSITKARARTARIHI TAKSITKARARTARIHI,
        TAKSITKARARSAYI TAKSITKARARSAYI,
		TAKSITBASTARIHI TAKSITBASTARIHI,
        2 TAKSITSAYISI
        , 'stratek' KAYDEDEN, NULL KAYITTARIHI, 'stratek' ONAYLAYAN, NULL ONAYTARIHI, 'AÇILIŞ' GEREKCE
    from ATMDOSYAAB
where TAKSITTALEPTARIHI is not null
order by TAKSITTALEPTARIHI

        
update ATMDOSYAABTAKSIT
set TAKSITSAYISI = (select COUNT(*) from  ATMBORCHAREKET 
  where ATMDOSYAABTAKSIT.IPARDDONEMI = ATMBORCHAREKET.IPARDDONEMI AND ATMDOSYAABTAKSIT.DOSYAANAHTAR = ATMBORCHAREKET.DOSYAANAHTAR 
  and TUR=61 )
  


     * 
     * 
     * 
     
INSERT INTO ATMDOSYAABGERIALIM
SELECT IPARDDONEMI,
    DOSYAANAHTAR,
    1 as SIRA,
    GERIALIMTARIHI BASTARIH,
    NULL BITTARIH,
    5 DURUM,
    
    GERIALIMTARIHI GERIALIMKARARITARIHI
    , GERIALIMSAYI GERIALIMSAYI
    , BILDIRIMTARIHI BILDIRIMTARIHI
    , TESELLUMTARIHI TESELLUMTARIHI
    , ODEMESONTARIHI ODEMESONTARIHI
    , TEMERRUTTARIHI TEMERRUTTARIHI
    , FAIZINDURDURULMATARIHI FAIZINDURDURULMATARIHI
    , GERIALIMMIKTARAB GERIALIMMIKTARAB
    , GERIALIMMIKTARULUSAL  GERIALIMMIKTARULUSAL
    , FAIZMIKTARAB  FAIZMIKTARAB 
    , FAIZMIKTARULUSAL  FAIZMIKTARULUSAL
	, 'stratek' KAYDEDEN, NULL KAYITTARIHI, 'stratek' ONAYLAYAN, NULL ONAYTARIHI, 'AÇILIŞ' GEREKCE
from ATMDOSYAAB
where GERIALIMTARIHI is not null
order by GERIALIMTARIHI




CREATE TABLE ATMDOSYAABGERIALIM (
    IPARDDONEMI int NOT NULL,
    DOSYAANAHTAR int NOT NULL,
    SIRA int NOT NULL,
    BASTARIH DATETIME NOT NULL,
    BITTARIH DATETIME NULL,
    DURUM INT NOT NULL
    
    , GERIALIMKARARITARIHI DATETIME NOT NULL
    , GERIALIMSAYI VARCHAR(30)
    , BILDIRIMTARIHI DATETIME 
    , TESELLUMTARIHI DATETIME  
    , ODEMESONTARIHI DATETIME  
    , TEMERRUTTARIHI DATETIME  
    , FAIZINDURDURULMATARIHI DATETIME
    , GERIALIMMIKTARAB money 
    , GERIALIMMIKTARULUSAL money
    , FAIZMIKTARAB money 
    , FAIZMIKTARULUSAL money

    ,KAYDEDEN varchar(30), KAYITTARIHI DATETIME, ONAYLAYAN varchar(30), ONAYTARIHI DATETIME, GEREKCE VARCHAR(200)
    PRIMARY KEY (IPARDDONEMI ASC, DOSYAANAHTAR ASC, SIRA ASC) 
) 

CREATE TABLE ATMDOSYAABONBORC (
    IPARDDONEMI int NOT NULL,
    DOSYAANAHTAR int NOT NULL,
    SIRA int NOT NULL,
    BASTARIH DATETIME NOT NULL,
    BITTARIH DATETIME NULL,
    DURUM INT NOT NULL,
    
    BORCTURU int NOT NULL ,
    BORCUNKODU  varchar(300) NOT NULL,
    ONBORCTARIHI datetime NOT NULL ,
    ONBORCSAYI varchar(30) NOT NULL ,
    VAKANUMARASI varchar(30) NOT NULL ,

    ONBORCMIKTARAB money NOT NULL ,
    ONBORCMIKTARULUSAL money NOT NULL ,
    IPARDAKTARIMTAR date,
    KAYDEDEN varchar(30), KAYITTARIHI DATETIME, ONAYLAYAN varchar(30), ONAYTARIHI DATETIME, GEREKCE VARCHAR(200)
    PRIMARY KEY (IPARDDONEMI ASC, DOSYAANAHTAR ASC, SIRA ASC) 
) 
 
INSERT INTO ATMDOSYAABONBORC
    SELECT IPARDDONEMI,
        DOSYAANAHTAR,
        1 as SIRA,
        ONBORCTARIHI BASTARIH,
        NULL BITTARIH,
        5 DURUM,

        BORCTURU BORCTURU,
        BORCUNKODU  BORCUNKODU,
        ONBORCTARIHI ONBORCTARIHI,
        ONBORCSAYI  ONBORCSAYI,
        VAKANUMARASI  VAKANUMARASI,

        ONBORCMIKTARAB  ONBORCMIKTARAB,
        ONBORCMIKTARULUSAL ONBORCMIKTARULUSAL,
        IPARDAKTARIMTAR IPARDAKTARIMTAR,
        'stratek' KAYDEDEN , 
        null KAYITTARIHI, 
        'stratek' ONAYLAYAN, 
        null ONAYTARIHI,
        'AÇILIŞ' GEREKCE
    from ATMDOSYAAB    
    
         ALTER TABLE ATMBORCHAREKET
ADD 	KAYDEDEN varchar(30) ,
	KAYITTARIHI datetime ,
	ONAYLAYAN varchar(30) ,
	ONAYTARIHI datetime ,
	GEREKCE varchar(200) 

         
         */

    /// <summary>
    /// ATMDOSYAAB veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMDOSYAAB : TABOrtak
    {
        /*  sql += ", " + VeriErisim.DegerYap((int)form.borcTuru);
            sql += ", " + VeriErisim.DegerYap(form.borcunKodu);
            sql += ", " + VeriErisim.DegerYap(form.onBorcTarihi);
            sql += ", " + VeriErisim.DegerYap(form.onBorcSayi);
            sql += ", " + VeriErisim.DegerYap(form.vakaNumarasi);
            sql += ", " + VeriErisim.DegerYap(form.onBorcMiktarAB);
            sql += ", " + VeriErisim.DegerYap(form.onBorcMiktarUlusal);
        */

        /// <summary>
        /// TABATMDOSYAAB sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMDOSYAAB(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ATMDOSYAAB";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// IPARDDONEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPARDDONEMI = 0,

            /// <summary>
            /// DOSYAANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYAANAHTAR = 1,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "IPARDDONEMI"
            , "DOSYAANAHTAR"
        };

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        /// <summary>
        /// Tablo tarihçe adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTARIHCE = "ATMDOSYAABTARIHCE";

        public enum ENUMAlanlarTARIHCE : int
        {
            /// <summary>
            /// IPARDDONEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPARDDONEMI = 0,

            /// <summary>
            /// DOSYAANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYAANAHTAR = 1,

            HAREKETNO = 2,
            ZAMAN = 3,
            METIN = 4,
            ISLEMKOD = 5,
            ISLEMIYAPAN = 6,
        }

        static string[] ALANLARTARIHCE = {
            "IPARDDONEMI",
            "DOSYAANAHTAR",
            "HAREKETNO",
            "ZAMAN",
            "METIN",
            "ISLEMKOD",
            "ISLEMIYAPAN "
        };

        //public IDataReader ListeleIslemler(DosyaAB okunan, bool onayBekleyenler)
        //{
        //    string sql = IslemSqlOl(DosyaABParca.ENUMTabloKod.ONBORC, TABLOADOB, okunan, onayBekleyenler);
        //    sql += " UNION ALL " + IslemSqlOl(DosyaABParca.ENUMTabloKod.GERIALIMKARARI, "ATMDOSYAABGERIALIM", okunan, onayBekleyenler);
        //    sql += " UNION ALL " + IslemSqlOl(DosyaABParca.ENUMTabloKod.DURUM, "ATMDOSYAABDURUM", okunan, onayBekleyenler);
        //    sql += " UNION ALL " + IslemSqlOl(DosyaABParca.ENUMTabloKod.TAKSIT, "ATMDOSYAABTAKSIT", okunan, onayBekleyenler);
        //    sql += " ORDER BY DURUM, KAYITTARIHI DESC, TABLOKOD";
        //    return veriErisim.DataReaderDondur(sql);
        //}

        //private string IslemSqlOl(DosyaABParca.ENUMTabloKod tabloKod, string tabloAd, DosyaAB okunan, bool onayBekleyenler)
        //{
        //    string sql = $"SELECT IPARDDONEMI, DOSYAANAHTAR, SIRA, BASTARIH, BITTARIH, DURUM, KAYDEDEN, KAYITTARIHI, ONAYLAYAN, ONAYTARIHI, GEREKCE, {(int)tabloKod} TABLOKOD, KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN FROM {tabloAd} T";
        //    sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
        //    sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";

        //    string kosul = "";
        //    if (okunan != null)
        //    {
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("T", "IPARDDONEMI", "=", okunan.iPARDDonemi));
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("T", "DOSYAANAHTAR", "=", okunan.dosyaAnahtar));
        //    }
        //    if (onayBekleyenler)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("T", "DURUM", "=", 1));

        //    return VeriErisim.KosulEkle(sql, "WHERE", kosul);
        //}

        ///// <summary>
        ///// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Anahtar değişkenleri tutan, DosyaAB yapısında nesne</param>
        ///// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        //public Sonuc Sil(DosyaAB form)
        //{
        //    string sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
        //    return KomutCalistir(sql, 0);
        //}

        public int AnahtarBul(DosyaAB form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
            double maks = this.veriErisim.TekKolonSQLDbl(sql);
            return (int)maks + 1;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DosyaAB yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(DosyaAB form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.iPARDDonemi);
            sql += ", " + VeriErisim.DegerYap(form.dosyaAnahtar);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        //public Sonuc Guncelle(DosyaAB form)
        //{
        //    string sql = "UPDATE " + TABLOAD;
        //    sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEFTERSIRASI], form.defterSirasi);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEDBIR], form.tedbir);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALTTEDBIR], form.altTedbir);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BUTCEKALEMI], form.butceKalemi);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], form.saymanlikKod);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], form.birimKod);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BORCLUKOD], form.borcluKod);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASVURUKAYITNUMARASI], form.basvuruKayitNumarasi);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMENUMARASI], form.sozlesmeNumarasi);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
        //    return KomutCalistir(sql, 1);
        //}

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DosyaAB yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(DosyaAB form)
        {
            //if (TNSDateTime.IsNull(tarih))
            //    tarih = new TNSDateTime(DateTime.Now);

            string alanlarBorclu = "\r\n (" + ListeleBorclu("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], null) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string alanlarDurum = "\r\n (" + ListeleDurum("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], null) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string alanlarOB = "\r\n (" + ListeleOB("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], null) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string alanlarGAK = "\r\n (" + ListeleGAK("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], null) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string alanlarTAKSIT = "\r\n (" + ListeleTaksit("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], null) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";

            string hareketSorgu = "\r\n (" + TABATMBORCHAREKET.ListeleUstTablo("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";

            string alanlar = "D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI] + ", D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR] + ", " + alanlarBorclu + ", " + alanlarDurum + ", " + alanlarOB + ", " + alanlarGAK + ", " + alanlarTAKSIT + ", " + hareketSorgu;
            string sql = "SELECT " + alanlar + "\r\nFROM " + TABLOAD + " D";

            string kosul = "";
            int anahSay = 0;
            if (form.iPARDDonemi > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
                anahSay++;
            }
            if (form.dosyaAnahtar > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("D", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
                anahSay++;
            }
            if (anahSay < 2)
            {
                string BorcluKosul = ListeleBorclu("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], form);
                if (!string.IsNullOrWhiteSpace(BorcluKosul))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"(1 IN ({BorcluKosul}))");

                string DurumKosul = ListeleDurum("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], form);
                if (!string.IsNullOrWhiteSpace(DurumKosul))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"(1 IN ({DurumKosul}))");

                string OBKosul = ListeleOB("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], form);
                if (!string.IsNullOrWhiteSpace(OBKosul))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"(1 IN ({OBKosul}))");

                string GAKKosul = ListeleGAK("D." + ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "D." + ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], form);
                if (!string.IsNullOrWhiteSpace(GAKKosul))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", $"(1 IN ({GAKKosul}))");

                if (!String.IsNullOrWhiteSpace(form.islemTarihAralik) || !String.IsNullOrWhiteSpace(form.islemliste))
                {
                    string icKosul = VeriErisim.KosulEkle(kosul, "AND", TarihKosulOl("IC", "TARIH", form.islemTarihAralik));
                    if (!String.IsNullOrWhiteSpace(form.islemliste))
                    {
                        icKosul += VeriErisim.IN_KosulOl(" AND", "IC." + TABATMBORCHAREKET.ALANLAR[(int)TABATMBORCHAREKET.ENUMAlanlar.TUR], form.islemliste, false, false);
                    }
                    if (!String.IsNullOrWhiteSpace(icKosul))
                    {
                        icKosul = " AND " + icKosul;
                    }
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(1 IN (SELECT 1 FROM " + TABATMBORCHAREKET.TABLOAD + " IC WHERE  " + VeriErisim.Join("IC", "D", new string[] { ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR] }) + icKosul + "))");
                }
                if (form.bekleyenIslemler)
                {
                    string icKosul = "";
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", DurumKosulOl("ATMDOSYAABBORCLU"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", DurumKosulOl("ATMBORCHAREKET"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", DurumKosulOl("ATMDOSYAABDURUM"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", DurumKosulOl("ATMDOSYAABGERIALIM"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", DurumKosulOl("ATMDOSYAABONBORC"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", DurumKosulOl("ATMDOSYAABTAKSIT"));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
                }
            }
            sql = VeriErisim.KosulEkle(sql, "\r\nWHERE", kosul);
            //            sql += "\r\nORDER BY BRC." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DEFTERSIRASI];
            return veriErisim.DataReaderDondur(sql);
        }

        private string DurumKosulOl(string tabloAd)
        {
            return $"1 In (select 1 from {tabloAd} IC where IC.IPARDDONEMI=D.IPARDDONEMI AND IC.DOSYAANAHTAR=D.DOSYAANAHTAR AND IC.DURUM=1)";
        }

        private string TarihKosulOl(string tabloAd, TNSDateTime tarih)
        {
            //return $"({tabloAd}.BASTARIH <= @tarih)";
            //return $"({tabloAd}.BITTARIH IS NULL)";
            return $"({tabloAd}.BASTARIH <= {VeriErisim.DegerYap(tarih)} AND ({tabloAd}.BITTARIH > {VeriErisim.DegerYap(tarih)} OR {tabloAd}.BITTARIH IS NULL))";
        }

        public static string OncekiAyIlkGun(string alanAd)
        {
            return "DATEADD(mm, -1, DATEADD(dd, 1-1*DAY(" + alanAd + "), " + alanAd + "))";
        }

        private string TarihKosulOl(string tabloAd, string alanAd, string strTarihAralik)
        {
            string kosul = "";
            if (!String.IsNullOrWhiteSpace(strTarihAralik))
            {
                string[] tarihler = strTarihAralik.Split('-', ',', ';');
                string islec;
                string tarihParca;
                if (tarihler.Length == 2)
                {
                    tarihParca = tarihler[1].Trim();
                    if (!string.IsNullOrWhiteSpace(tarihParca))
                        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, "<=", new TNSDateTime(tarihParca)));
                    islec = ">=";
                }
                else
                    islec = "=";
                tarihParca = tarihler[0].Trim();
                if (!string.IsNullOrWhiteSpace(tarihParca))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tabloAd, alanAd, islec, new TNSDateTime(tarihParca)));
            }
            return kosul;
        }

        //public Sonuc FaizGuncelle(DosyaAB form)
        //{
        //    string sql = "UPDATE " + TABLOADGAK;
        //    sql += " SET " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.FAIZMIKTARAB], form.faizMiktarAB);
        //    sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.FAIZMIKTARULUSAL], form.faizMiktarUlusal);
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
        //    return KomutCalistir(sql, 0);
        //}

        public Sonuc TarihceEkle(string kullaniciKodu, int iPARDDonemi, int dosyaAnahtar, int hareketNo, string metin, int islemKod)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + "(" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(iPARDDonemi);
            sql += ", " + VeriErisim.DegerYap(dosyaAnahtar);
            sql += ", " + VeriErisim.DegerYap(hareketNo);
            sql += ", " + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += ", " + VeriErisim.DegerYap(metin);
            sql += ", " + VeriErisim.DegerYap(islemKod);
            sql += ", " + VeriErisim.DegerYap(kullaniciKodu);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader TarihceListele(int iPARDDonemi, int dosyaAnahtar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLARTARIHCE) + ", K.MERNIS, K.AD, K.SOYAD FROM " + TABLOADTARIHCE + " T";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI K ON K.KULLANICIKODU=T.ISLEMIYAPAN";
            string kosul = "";
            int anahSay = 0;
            if (iPARDDonemi > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("T", ALANLARTARIHCE[(int)ENUMAlanlarTARIHCE.IPARDDONEMI], "=", iPARDDonemi));
                anahSay++;
            }
            if (dosyaAnahtar > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("T", ALANLARTARIHCE[(int)ENUMAlanlarTARIHCE.DOSYAANAHTAR], "=", dosyaAnahtar));
                anahSay++;
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " ORDER BY T." + ALANLARTARIHCE[(int)ENUMAlanlarTARIHCE.ZAMAN] + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        //public Sonuc TaksitlendiYaz(DosyaAB form)
        //{
        //    string sql = "UPDATE " + TABLOAD;
        //    //sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITTALEPTARIHI], form.taksitTalepTarih);
        //    //sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITTALEPSAYI], form.taksitTalepSayi);
        //    //sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITKARARTARIHI], form.taksitKararTarih);
        //    //sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITKARARSAYI], form.taksitKararSayi);
        //    //sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITFAIZKURUSFARKIAB], 0);
        //    //sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITFAIZKURUSFARKIUL], 0);
        //    //sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAKSITILKTARIH], form.taksitIlkTarih);
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.SURECASAMASI] + " = " + ((int)TNS.ATM.DosyaAB.ENUMDosyaDurum.TAKSITLENDIRILMIS);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
        //    return KomutCalistir(sql, 1);
        //}

        //public Sonuc TaksitlendiSil(DosyaAB form)
        //{
        //    string sql = "UPDATE " + TABLOAD;
        //    //sql += " SET " + ALANLAR[(int)ENUMAlanlar.TAKSITTALEPTARIHI] + " = NULL";
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.TAKSITTALEPSAYI] + " = NULL";
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.TAKSITKARARTARIHI] + " = NULL";
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.TAKSITKARARSAYI] + " = NULL";
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.TAKSITILKTARIH] + " = NULL";
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.TAKSITFAIZKURUSFARKIAB] + " = NULL";
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.TAKSITFAIZKURUSFARKIUL] + " = NULL";
        //    //sql += ", " + ALANLAR[(int)ENUMAlanlar.SURECASAMASI] + " = " + ((int)TNS.ATM.DosyaAB.ENUMDosyaDurum.YASALTAKIP);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IPARDDONEMI], "=", form.iPARDDonemi));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAANAHTAR], "=", form.dosyaAnahtar));
        //    return KomutCalistir(sql, 1);
        //}

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, OnBorc yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(OnBorc form)
        {
            string sql = "DELETE FROM " + TABLOADOB;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }

        #region "Borçlu"
        public static string TABLOADBORCLU = "ATMDOSYAABBORCLU";

        public enum ENUMAlanlarBORCLU : int
        {
            /// <summary>
            /// IPARDDONEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPARDDONEMI = 0,

            /// <summary>
            /// DOSYAANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYAANAHTAR = 1,

            /// <summary>
            /// SIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRA = 2,

            /// <summary>
            /// BASTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASTARIH = 3,

            /// <summary>
            /// BITTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITTARIH = 4,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 5,

            /// <summary>
            /// DEFTERSIRASI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEFTERSIRASI = 6,

            /// <summary>
            /// TEDBIR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TEDBIR = 7,

            /// <summary>
            /// ALTTEDBIR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTTEDBIR = 8,

            /// <summary>
            /// BUTCEKALEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEKALEMI = 9,

            /// <summary>
            /// SAYMANLIKKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIKKOD = 10,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 11,

            /// <summary>
            /// BORCLUKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCLUKOD = 12,

            /// <summary>
            /// BASVURUKAYITNUMARASI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASVURUKAYITNUMARASI = 13,

            /// <summary>
            /// SOZLESMENUMARASI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMENUMARASI = 14,

            /// <summary>
            /// KAYDEDEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYDEDEN = 15,

            /// <summary>
            /// KAYITTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYITTARIHI = 16,

            /// <summary>
            /// ONAYLAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYLAYAN = 17,

            /// <summary>
            /// ONAYTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 18,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 19
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARBORCLU = {
            "IPARDDONEMI", "DOSYAANAHTAR", "SIRA", "BASTARIH", "BITTARIH", "DURUM"
            , "DEFTERSIRASI"
            , "TEDBIR"
            , "ALTTEDBIR"
            , "BUTCEKALEMI"
            , "SAYMANLIKKOD"
            , "BIRIMKOD"
            , "BORCLUKOD"
            , "BASVURUKAYITNUMARASI"
            , "SOZLESMENUMARASI"
            ,"KAYDEDEN", "KAYITTARIHI", "ONAYLAYAN", "ONAYTARIHI", "GEREKCE"};

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, ABBorçlu  yapısında nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodunu tutan değişken</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleBorclu(ABBorclu form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADBORCLU;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BASTARIH], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DEFTERSIRASI], form.defterSirasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.TEDBIR], form.tedbir);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.ALTTEDBIR], form.altTedbir);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BUTCEKALEMI], form.butceKalemi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SAYMANLIKKOD], form.saymanlikKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BIRIMKOD], form.birimKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BORCLUKOD], form.borcluKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BASVURUKAYITNUMARASI], form.basvuruKayitNumarasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SOZLESMENUMARASI], form.sozlesmeNumarasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.GEREKCE], form.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, ABBorçlu  yapısında nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodunu tutan değişken</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleBorcluOzel(ABBorclu form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADBORCLU;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DEFTERSIRASI], form.defterSirasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.TEDBIR], form.tedbir);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.ALTTEDBIR], form.altTedbir);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BUTCEKALEMI], form.butceKalemi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SAYMANLIKKOD], form.saymanlikKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BIRIMKOD], form.birimKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BORCLUKOD], form.borcluKod);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BASVURUKAYITNUMARASI], form.basvuruKayitNumarasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SOZLESMENUMARASI], form.sozlesmeNumarasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.GEREKCE], form.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }

        public Sonuc EkleBorclu(ABBorclu form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "INSERT INTO " + TABLOADBORCLU + "(" + VeriErisim.TumAlanlar("", ALANLARBORCLU) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.ipardDonemi);
            sql += ", " + VeriErisim.DegerYap(form.dosyaAnahtar);
            sql += ", " + VeriErisim.DegerYap(form.sira);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.bitTarih);
            sql += ", " + VeriErisim.DegerYap((int)DosyaABParca.ENUMKayitDurum.YENI);
            sql += ", " + VeriErisim.DegerYap(form.defterSirasi);
            sql += ", " + VeriErisim.DegerYap(form.tedbir);
            sql += ", " + VeriErisim.DegerYap(form.altTedbir);
            sql += ", " + VeriErisim.DegerYap(form.butceKalemi);
            sql += ", " + VeriErisim.DegerYap(form.saymanlikKod);
            sql += ", " + VeriErisim.DegerYap(form.birimKod);
            sql += ", " + VeriErisim.DegerYap(form.borcluKod);
            sql += ", " + VeriErisim.DegerYap(form.basvuruKayitNumarasi);
            sql += ", " + VeriErisim.DegerYap(form.sozlesmeNumarasi);
            sql += ", " + VeriErisim.DegerYap(kullaniciKodu);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL";// + VeriErisim.DegerYap(form.islemBilgi.onaylayan);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.onayTarihi);
            sql += ", " + VeriErisim.DegerYap(form.gerekce);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, OnBorc yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader BorcluListele(ABBorclu form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLARBORCLU) + ", F.VERGINO, F.AD, MUH.MUHASEBEAD, BIR.BIRIMAD, KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN FROM " + TABLOADBORCLU + " T";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI F ON F.KOD=T.BORCLUKOD";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMUHASEBE MUH ON MUH.MUHASEBEKOD=T.SAYMANLIKKOD";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMUHASEBEBIRIM BIR ON BIR.MUHASEBEKOD=T.SAYMANLIKKOD AND BIR.BIRIMKOD=T.BIRIMKOD";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SIRA], "=", form.sira));
            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeleBorclu(string ipardDonemiAlanAd, string dosyaAnahtarAlanAd, DosyaAB form)
        {
            string alanlar;
            if (form == null)
                alanlar = VeriErisim.TumAlanlar("BRC", ALANLARBORCLU) + ", F.VERGINO, F.AD, MUH.MUHASEBEAD, BIR.BIRIMAD, KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN";
            else
                alanlar = "1";

            string sql = "SELECT " + alanlar + " FROM " + TABLOADBORCLU + " BRC ";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCFIRMABILGISI F ON F.KOD=BRC.BORCLUKOD";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMUHASEBE MUH ON MUH.MUHASEBEKOD=BRC.SAYMANLIKKOD";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("ButceIslemleri.dbo.") + "MUHMUHASEBEBIRIM BIR ON BIR.MUHASEBEKOD=BRC.SAYMANLIKKOD AND BIR.BIRIMKOD=BRC.BIRIMKOD";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=BRC.KAYDEDEN";
            sql += "\r\nLEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=BRC.ONAYLAYAN";

            sql += " WHERE BRC." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.IPARDDONEMI] + " = " + ipardDonemiAlanAd;
            sql += " AND BRC." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DOSYAANAHTAR] + " = " + dosyaAnahtarAlanAd;
            if (form != null)
            {
                string kosul = "";
                if (form.saymanlikKod > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BRC", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.SAYMANLIKKOD], "=", form.saymanlikKod));
                }
                if (!string.IsNullOrWhiteSpace(form.birimKod))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("BRC", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BIRIMKOD], "=", form.birimKod));
                }
                if (!string.IsNullOrWhiteSpace(form.aramaMetni))
                {
                    string icKosul = "";
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("F", "AD", "LIKE", "%" + form.aramaMetni + "%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("F", "VERGINO", "=", form.aramaMetni));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("BRC", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BASVURUKAYITNUMARASI], "LIKE", form.aramaMetni + "%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("BRC", ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DEFTERSIRASI], "=", form.aramaMetni));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
                }
                if (!string.IsNullOrWhiteSpace(form.borcluAd))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("F", "AD", "LIKE", form.borcluAd + "%"));
                }
                if (!string.IsNullOrWhiteSpace(form.kimlikNo))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("F", "VERGINO", "=", form.kimlikNo));
                }
                if (string.IsNullOrWhiteSpace(kosul))
                    return null;
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", kosul);
            }
            else
                sql += " ORDER BY " + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BASTARIH] + " DESC";
            return sql;
        }

        #endregion

        #region "Ön Borç"
        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADOB = "ATMDOSYAABONBORC";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarOB : int
        {
            /// <summary>
            /// IPARDDONEMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPARDDONEMI = 0,

            /// <summary>
            /// DOSYAANAHTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOSYAANAHTAR = 1,

            /// <summary>
            /// SIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRA = 2,

            /// <summary>
            /// BASTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASTARIH = 3,

            /// <summary>
            /// BITTARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITTARIH = 4,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 5,

            /// <summary>
            /// BORCTURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCTURU = 6,

            /// <summary>
            /// BORCUNKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BORCUNKODU = 7,

            /// <summary>
            /// ONBORCTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONBORCTARIHI = 8,

            /// <summary>
            /// ONBORCSAYI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONBORCSAYI = 9,

            /// <summary>
            /// VAKANUMARASI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            VAKANUMARASI = 10,

            /// <summary>
            /// ONBORCMIKTARAB alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONBORCMIKTARAB = 11,

            /// <summary>
            /// ONBORCMIKTARULUSAL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONBORCMIKTARULUSAL = 12,

            /// <summary>
            /// IPARDAKTARIMTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IPARDAKTARIMTAR = 13,

            /// <summary>
            /// ONBORCSAYI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AVANS = 14,

            /// <summary>
            /// KAYDEDEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYDEDEN = 15,

            /// <summary>
            /// KAYITTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAYITTARIHI = 16,

            /// <summary>
            /// ONAYLAYAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYLAYAN = 17,

            /// <summary>
            /// ONAYTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYTARIHI = 18,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 19
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAROB = {
            "IPARDDONEMI", "DOSYAANAHTAR", "SIRA", "BASTARIH", "BITTARIH", "DURUM",
            "BORCTURU", "BORCUNKODU", "ONBORCTARIHI",
            "ONBORCSAYI", "VAKANUMARASI", "ONBORCMIKTARAB", "ONBORCMIKTARULUSAL", "IPARDAKTARIMTAR", "AVANS",
            "KAYDEDEN", "KAYITTARIHI", "ONAYLAYAN", "ONAYTARIHI", "GEREKCE"};

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Ön borç bilgilerini tutan, OnBorc yapısında nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodunu tutan değişken</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleOB(OnBorc form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADOB;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BASTARIH], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BORCTURU], (int)form.borcTuru);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BORCUNKODU], form.borcunKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONBORCTARIHI], form.onBorcTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONBORCSAYI], form.onBorcSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.VAKANUMARASI], form.vakaNumarasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.AVANS], form.avans ? 1 : 0);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONBORCMIKTARAB], form.onBorcMiktarAB);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONBORCMIKTARULUSAL], form.onBorcMiktarUlusal);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDAKTARIMTAR], form.aktarimTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.GEREKCE], form.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Ön borç bilgilerini tutan, OnBorc yapısında nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodunu tutan değişken</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleOBOzel(OnBorc form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADOB;

            sql += " SET " +  VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BORCTURU], (int)form.borcTuru);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BORCUNKODU], form.borcunKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONBORCTARIHI], form.onBorcTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONBORCSAYI], form.onBorcSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.VAKANUMARASI], form.vakaNumarasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.AVANS], form.avans ? 1 : 0);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.GEREKCE], form.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, OnBorc yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleOB(OnBorc form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "INSERT INTO " + TABLOADOB + "(" + VeriErisim.TumAlanlar("", ALANLAROB) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.ipardDonemi);
            sql += ", " + VeriErisim.DegerYap(form.dosyaAnahtar);
            sql += ", " + VeriErisim.DegerYap(form.sira);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.bitTarih);
            sql += ", " + VeriErisim.DegerYap((int)DosyaABParca.ENUMKayitDurum.YENI);
            sql += ", " + VeriErisim.DegerYap((int)form.borcTuru);
            sql += ", " + VeriErisim.DegerYap(form.borcunKodu);
            sql += ", " + VeriErisim.DegerYap(form.onBorcTarihi);
            sql += ", " + VeriErisim.DegerYap(form.onBorcSayi);
            sql += ", " + VeriErisim.DegerYap(form.vakaNumarasi);
            sql += ", " + VeriErisim.DegerYap(form.onBorcMiktarAB);
            sql += ", " + VeriErisim.DegerYap(form.onBorcMiktarUlusal);
            sql += ", " + VeriErisim.DegerYap(form.aktarimTarihi);
            sql += ", " + VeriErisim.DegerYap(form.avans ? 1 : 0);
            sql += ", " + VeriErisim.DegerYap(kullaniciKodu);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL";// + VeriErisim.DegerYap(form.islemBilgi.onaylayan);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.onayTarihi);
            sql += ", " + VeriErisim.DegerYap(form.gerekce);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, OnBorc yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader OBListele(OnBorc form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLAROB) + ", KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN, DOB.DOVIZALIS FROM " + TABLOADOB + " T";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCMERKEZBANKKUR DOB ON DOB.PARABIRIMI='EUR' AND DOB.KURTARIH=" + OncekiAyIlkGun("T.ONBORCTARIHI");

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.SIRA], "=", form.sira));
            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeleOB(string ipardDonemiAlanAd, string dosyaAnahtarAlanAd, DosyaAB form)
        {
            string alanlar;
            if (form == null)
                alanlar = VeriErisim.TumAlanlar("T", ALANLAROB) + ", KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN, DOB.DOVIZALIS";
            else
                alanlar = "1";

            string sql = "SELECT " + alanlar + " FROM " + TABLOADOB + " T";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCMERKEZBANKKUR DOB ON DOB.PARABIRIMI='EUR' AND DOB.KURTARIH=" + OncekiAyIlkGun("T.ONBORCTARIHI");

            sql += " WHERE T." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.IPARDDONEMI] + " = " + ipardDonemiAlanAd;
            sql += " AND T." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DOSYAANAHTAR] + " = " + dosyaAnahtarAlanAd;

            if (form != null)
            {
                string kosul = "";
                if (!string.IsNullOrWhiteSpace(form.borcTurleri))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.IN_KosulOl("", "T." + ALANLAROB[(int)ENUMAlanlarOB.BORCTURU], form.borcTurleri, false, false));
                }
                kosul = VeriErisim.KosulEkle(kosul, "AND", TarihKosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.ONBORCTARIHI], form.onBorcTarihAralik));
                if (!TNSDateTime.IsNull(form.onBorcTarihi) && form.onBorcTarihi.Yil > 2000)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.ONBORCTARIHI], "<=", form.onBorcTarihi));
                }
                if (form.avans)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.AVANS], "=", 1));
                }
                if (string.IsNullOrWhiteSpace(kosul))
                    return null;
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", kosul);
            }
            else
                sql += " ORDER BY " + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.BASTARIH] + " DESC";
            return sql;
        }
        #endregion

        #region "Geri Alım Kararı"
        public static string TABLOADGAK = "ATMDOSYAABGERIALIM";

        public enum ENUMAlanlarGAK : int
        {
            IPARDDONEMI = 0, DOSYAANAHTAR = 1, SIRA = 2, BASTARIH = 3, BITTARIH = 4, DURUM = 5
            , GERIALIMKARARITARIHI = 6
            , GERIALIMSAYI = 7
            , BILDIRIMTARIHI = 8
            , TESELLUMTARIHI = 9
            , ODEMESONTARIHI = 10
            , TEMERRUTTARIHI = 11
            , FAIZINDURDURULMATARIHI = 12
            , GERIALIMMIKTARAB = 13
            , GERIALIMMIKTARULUSAL = 14
            , SILINMEYECEKMIKTAR = 15
            , FAIZMIKTARULUSAL = 16
            , KAYDEDEN = 17, KAYITTARIHI = 18, ONAYLAYAN = 19, ONAYTARIHI = 20, GEREKCE = 21
        }

        static string[] ALANLARGAK =
        {
            "IPARDDONEMI", "DOSYAANAHTAR", "SIRA", "BASTARIH", "BITTARIH", "DURUM"
            , "GERIALIMKARARITARIHI"
            , "GERIALIMSAYI"
            , "BILDIRIMTARIHI"
            , "TESELLUMTARIHI"
            , "ODEMESONTARIHI"
            , "TEMERRUTTARIHI"
            , "FAIZINDURDURULMATARIHI"
            , "GERIALIMMIKTARAB"
            , "GERIALIMMIKTARULUSAL"
            , "FAIZMIKTARAB"
            , "FAIZMIKTARULUSAL"
            , "KAYDEDEN", "KAYITTARIHI", "ONAYLAYAN", "ONAYTARIHI", "GEREKCE"
         };

        public IDataReader GAKListele(GeriAlimKarari form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLARGAK) + ",KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN, DGAK.DOVIZALIS FROM " + TABLOADGAK + " T";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCMERKEZBANKKUR DGAK ON DGAK.PARABIRIMI='EUR' AND DGAK.KURTARIH=" + OncekiAyIlkGun("T.GERIALIMKARARITARIHI");

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLARGAK[(int)ENUMAlanlarGAK.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARGAK[(int)ENUMAlanlarGAK.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARGAK[(int)ENUMAlanlarGAK.SIRA], "=", form.sira));
            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeleGAK(string ipardDonemiAlanAd, string dosyaAnahtarAlanAd, DosyaAB form)
        {
            string alanlar;
            if (form == null)
                alanlar = VeriErisim.TumAlanlar("T", ALANLARGAK) + ", KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN, DGAK.DOVIZALIS";
            else
                alanlar = "1";

            string sql = "SELECT " + alanlar + " FROM " + TABLOADGAK + " T";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("HarcamaYonetimi.dbo.") + "HRCMERKEZBANKKUR DGAK ON DGAK.PARABIRIMI='EUR' AND DGAK.KURTARIH=" + OncekiAyIlkGun("T.GERIALIMKARARITARIHI");
            sql += " WHERE T." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.IPARDDONEMI] + " = " + ipardDonemiAlanAd;
            sql += " AND T." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DOSYAANAHTAR] + " = " + dosyaAnahtarAlanAd;

            if (form != null)
            {
                string kosul = "";
                kosul = VeriErisim.KosulEkle(kosul, "AND", TarihKosulOl("T", ALANLARGAK[(int)ENUMAlanlarGAK.GERIALIMKARARITARIHI], form.geriAlimKarariTarihAralik));
                if (string.IsNullOrWhiteSpace(kosul))
                    return null;
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", kosul);
            }
            else
                sql += " ORDER BY " + ALANLARGAK[(int)ENUMAlanlarGAK.BASTARIH] + " DESC";
            return sql;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, OnBorc yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleGAK(GeriAlimKarari form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "INSERT INTO " + TABLOADGAK + "(" + VeriErisim.TumAlanlar("", ALANLARGAK) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.ipardDonemi);
            sql += ", " + VeriErisim.DegerYap(form.dosyaAnahtar);
            sql += ", " + VeriErisim.DegerYap(form.sira);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.bitTarih);
            sql += ", " + VeriErisim.DegerYap((int)DosyaABParca.ENUMKayitDurum.YENI);

            sql += ", " + VeriErisim.DegerYap(form.geriAlimKarariTarihi);
            sql += ", " + VeriErisim.DegerYap(form.geriAlimSayi);
            sql += ", " + VeriErisim.DegerYap(form.bildirimTarihi);
            sql += ", " + VeriErisim.DegerYap(form.tesellumTarihi);
            sql += ", " + VeriErisim.DegerYap(form.odemeSonTarihi);
            sql += ", " + VeriErisim.DegerYap(form.temerrutTarihi);
            sql += ", " + VeriErisim.DegerYap(form.faizinDurdurulmaTarihi);
            sql += ", " + VeriErisim.DegerYap(form.gakMiktarAB);
            sql += ", " + VeriErisim.DegerYap(form.gakMiktarUlusal);
            sql += ", " + VeriErisim.DegerYap(form.silinmeyecekMiktar);
            sql += ", " + VeriErisim.DegerYap(0);

            sql += ", " + VeriErisim.DegerYap(kullaniciKodu);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL";// + VeriErisim.DegerYap(form.islemBilgi.onaylayan);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.onayTarihi);
            sql += ", " + VeriErisim.DegerYap(form.gerekce);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Geri alım kararı bilgilerini tutan, GeriAlimKarari yapısında nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodunu tutan değişken</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleGAK(GeriAlimKarari form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADGAK;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.BASTARIH], islemZamani);

            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GERIALIMKARARITARIHI], form.geriAlimKarariTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GERIALIMSAYI], form.geriAlimSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.BILDIRIMTARIHI], form.bildirimTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.TESELLUMTARIHI], form.tesellumTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.ODEMESONTARIHI], form.odemeSonTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.TEMERRUTTARIHI], form.temerrutTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.FAIZINDURDURULMATARIHI], form.faizinDurdurulmaTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GERIALIMMIKTARAB], form.gakMiktarAB);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GERIALIMMIKTARULUSAL], form.gakMiktarUlusal);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.SILINMEYECEKMIKTAR], form.silinmeyecekMiktar);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.FAIZMIKTARULUSAL], 0);

            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GEREKCE], form.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGAK[(int)ENUMAlanlarGAK.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGAK[(int)ENUMAlanlarGAK.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGAK[(int)ENUMAlanlarGAK.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Geri alım kararı bilgilerini tutan, GeriAlimKarari yapısında nesne</param>
        /// <param name="kullaniciKodu">Kullanıcı kodunu tutan değişken</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleGAKOzel(GeriAlimKarari form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADGAK;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GERIALIMKARARITARIHI], form.geriAlimKarariTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GERIALIMSAYI], form.geriAlimSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.BILDIRIMTARIHI], form.bildirimTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.TESELLUMTARIHI], form.tesellumTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.ODEMESONTARIHI], form.odemeSonTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.TEMERRUTTARIHI], form.temerrutTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.FAIZINDURDURULMATARIHI], form.faizinDurdurulmaTarihi);

            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARGAK[(int)ENUMAlanlarGAK.GEREKCE], form.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGAK[(int)ENUMAlanlarGAK.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGAK[(int)ENUMAlanlarGAK.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGAK[(int)ENUMAlanlarGAK.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }

        #endregion

        #region "Durum"
        public static string TABLOADDURUM = "ATMDOSYAABDURUM";
        public enum ENUMAlanlarDURUM : int
        {
            IPARDDONEMI = 0, DOSYAANAHTAR = 1, SIRA = 2, BASTARIH = 3, BITTARIH = 4, DURUM = 5
            , SURECASAMASI = 6
            , YORUMLARTR = 7
            , YORUMLAREN = 8
            , KAYDEDEN = 9, KAYITTARIHI = 10, ONAYLAYAN = 11, ONAYTARIHI = 12, GEREKCE = 13
        }

        static string[] ALANLARDURUM = {
            "IPARDDONEMI", "DOSYAANAHTAR", "SIRA", "BASTARIH", "BITTARIH", "DURUM"
            , "SURECASAMASI"
            , "YORUMLARTR"
            , "YORUMLAREN"
            , "KAYDEDEN", "KAYITTARIHI", "ONAYLAYAN", "ONAYTARIHI", "GEREKCE"};


        public IDataReader DURUMListele(DosyaABDurum form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLARDURUM) + ",KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN FROM " + TABLOADDURUM + " T";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLARDURUM[(int)ENUMAlanlarDURUM.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARDURUM[(int)ENUMAlanlarDURUM.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARDURUM[(int)ENUMAlanlarDURUM.SIRA], "=", form.sira));
            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeleDurum(string ipardDonemiAlanAd, string dosyaAnahtarAlanAd, DosyaAB form)
        {
            string alanlar;
            if (form == null)
                alanlar = VeriErisim.TumAlanlar("T", ALANLARDURUM) + ",KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN";
            else
                alanlar = "1";

            string sql = "SELECT " + alanlar + " FROM " + TABLOADDURUM + " T ";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";
            sql += " WHERE T." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.IPARDDONEMI] + " = " + ipardDonemiAlanAd;
            sql += " AND T." + ALANLARBORCLU[(int)ENUMAlanlarBORCLU.DOSYAANAHTAR] + " = " + dosyaAnahtarAlanAd;

            if (form != null)
            {
                string kosul = "";
                if (!string.IsNullOrWhiteSpace(form.geriAlimSurecAsamalari))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.IN_KosulOl("", "T." + ALANLARDURUM[(int)ENUMAlanlarDURUM.SURECASAMASI], form.geriAlimSurecAsamalari, false, false));
                }
                if (string.IsNullOrWhiteSpace(kosul))
                    return null;
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", kosul);
            }
            else
                sql += " ORDER BY " + ALANLARDURUM[(int)ENUMAlanlarDURUM.BASTARIH] + " DESC";
            return sql;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, OnBorc yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleDurum(DosyaABDurum form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "INSERT INTO " + TABLOADDURUM + "(" + VeriErisim.TumAlanlar("", ALANLARDURUM) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.ipardDonemi);
            sql += ", " + VeriErisim.DegerYap(form.dosyaAnahtar);
            sql += ", " + VeriErisim.DegerYap(form.sira);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.bitTarih);
            sql += ", " + VeriErisim.DegerYap((int)DosyaABParca.ENUMKayitDurum.YENI);

            sql += ", " + VeriErisim.DegerYap((int)form.surecAsamasi);
            sql += ", " + VeriErisim.DegerYap(form.yorumlarTR);
            sql += ", " + VeriErisim.DegerYap(form.yorumlarEN);

            sql += ", " + VeriErisim.DegerYap(kullaniciKodu);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL";// + VeriErisim.DegerYap(form.islemBilgi.onaylayan);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.onayTarihi);
            sql += ", " + VeriErisim.DegerYap(form.gerekce);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, OnBorc yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleDurum(DosyaABDurum form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADDURUM;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.BASTARIH], islemZamani);

            sql += ", " + VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.SURECASAMASI], (int)form.surecAsamasi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.YORUMLARTR], form.yorumlarTR);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.YORUMLAREN], form.yorumlarEN);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.GEREKCE], form.gerekce);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }
        #endregion

        #region "Taksit"
        public static string TABLOADTAKSIT = "ATMDOSYAABTAKSIT";
        public enum ENUMAlanlarTAKSIT : int
        {
            IPARDDONEMI = 0, DOSYAANAHTAR = 1, SIRA = 2, BASTARIH = 3, BITTARIH = 4, DURUM = 5,
            TAKSITTALEPTARIHI = 6,
            TAKSITTALEPSAYI = 7,
            TAKSITKARARTARIHI = 8,
            TAKSITKARARSAYI = 9,
            TAKSITILKTARIH = 10,
            TAKSITSAYISI = 11,
            GIRISTARIHI = 12,
            KAYDEDEN = 13, KAYITTARIHI = 14, ONAYLAYAN = 15, ONAYTARIHI = 16, GEREKCE = 17, TECILTUR = 18
        }

        static string[] ALANLARTAKSIT =
        {
            "IPARDDONEMI", "DOSYAANAHTAR", "SIRA", "BASTARIH", "BITTARIH", "DURUM"
            , "TAKSITTALEPTARIHI"
            , "TAKSITTALEPSAYI"
            , "TAKSITKARARTARIHI"
            , "TAKSITKARARSAYI"
            , "TAKSITBASTARIHI"
            , "TAKSITSAYISI"
            , "GIRISTARIHI"
            , "KAYDEDEN", "KAYITTARIHI", "ONAYLAYAN", "ONAYTARIHI", "GEREKCE", "TECILTUR"
        };

        public IDataReader TaksitListele(DosyaABTaksit form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLARTAKSIT) + ",KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN FROM " + TABLOADTAKSIT + " T";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.SIRA], "=", form.sira));
            return veriErisim.DataReaderDondur(sql);
        }

        private string ListeleTaksit(string ipardDonemiAlanAd, string dosyaAnahtarAlanAd, DosyaAB form)
        {
            string alanlar;
            if (form == null)
                alanlar = VeriErisim.TumAlanlar("T", ALANLARTAKSIT) + ", KULKAYIT.AD, KULKAYIT.SOYAD, KULKAYIT.UNVAN, KULONAY.AD, KULONAY.SOYAD, KULONAY.UNVAN";
            else
                alanlar = "1";

            string sql = "SELECT " + alanlar + " FROM " + TABLOADTAKSIT + " T";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULKAYIT ON KULKAYIT.KULLANICIKODU=T.KAYDEDEN";
            sql += " LEFT OUTER JOIN " + DatabaseAdBul("Kullanici.dbo.") + "KULLANICI KULONAY ON KULONAY.KULLANICIKODU=T.ONAYLAYAN";
            sql += " WHERE T." + ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.IPARDDONEMI] + " = " + ipardDonemiAlanAd;
            sql += " AND T." + ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.DOSYAANAHTAR] + " = " + dosyaAnahtarAlanAd;

            if (form != null)
            {
                return null;
            }
            else
                sql += " ORDER BY " + ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.BASTARIH] + " DESC";

            return sql;
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, OnBorc yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc EkleTaksit(DosyaABTaksit form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "INSERT INTO " + TABLOADTAKSIT + "(" + VeriErisim.TumAlanlar("", ALANLARTAKSIT) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.ipardDonemi);
            sql += ", " + VeriErisim.DegerYap(form.dosyaAnahtar);
            sql += ", " + VeriErisim.DegerYap(form.sira);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.bitTarih);
            sql += ", " + VeriErisim.DegerYap((int)DosyaABParca.ENUMKayitDurum.YENI);

            sql += ", " + VeriErisim.DegerYap(form.taksitTalepTarihi);
            sql += ", " + VeriErisim.DegerYap(form.taksitTalepSayi);
            sql += ", " + VeriErisim.DegerYap(form.taksitKararTarihi);
            sql += ", " + VeriErisim.DegerYap(form.taksitKararSayi);
            sql += ", " + VeriErisim.DegerYap(form.taksitIlkTarih);
            sql += ", " + VeriErisim.DegerYap(form.taksitSayisi);
            sql += ", " + VeriErisim.DegerYap(form.girisTarihi);

            sql += ", " + VeriErisim.DegerYap(kullaniciKodu);
            sql += ", " + VeriErisim.DegerYap(islemZamani);
            sql += ", NULL";// + VeriErisim.DegerYap(form.islemBilgi.onaylayan);
            sql += ", NULL"; //+ VeriErisim.DegerYap(form.islemBilgi.onayTarihi);
            sql += ", " + VeriErisim.DegerYap(form.gerekce);
            sql += ", " + VeriErisim.DegerYap(form.tecilFaizNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, OnBorc yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc GuncelleTaksit(DosyaABTaksit form, string kullaniciKodu)
        {
            TNSDateTime islemZamani = new TNSDateTime(DateTime.Now);
            string sql = "UPDATE " + TABLOADTAKSIT;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.BASTARIH], islemZamani);

            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.TAKSITTALEPTARIHI], form.taksitTalepTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.TAKSITTALEPSAYI], form.taksitTalepSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.TAKSITKARARTARIHI], form.taksitKararTarihi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.TAKSITKARARSAYI], form.taksitKararSayi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.TAKSITILKTARIH], form.taksitIlkTarih);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.TAKSITSAYISI], form.taksitSayisi);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.GIRISTARIHI], form.girisTarihi);

            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.KAYDEDEN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.KAYITTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.GEREKCE], form.gerekce);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARTAKSIT[(int)ENUMAlanlarTAKSIT.TECILTUR], form.tecilFaizNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.IPARDDONEMI], "=", form.ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.DOSYAANAHTAR], "=", form.dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.SIRA], "=", form.sira));
            return KomutCalistir(sql, 0);
        }
        #endregion

        #region "İşlemler Ortak"
        public int SiraNoBul(int ipardDonemi, int dosyaAnahtar, string tabloAd)
        {
            string sql = "SELECT MAX(SIRA) FROM " + tabloAd + " T";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", dosyaAnahtar));

            double d = veriErisim.TekKolonSQLDbl(sql);
            return ((int)d) + 1;
        }

        public Sonuc DurumDegistir(TNSDateTime islemZamani, int ipardDonemi, int dosyaAnahtar, int sira, int islemTur, string tabloAd, string kullaniciKodu)
        {
            string sql = "UPDATE " + tabloAd;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BASTARIH], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONAYLAYAN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.ONAYTARIHI], islemZamani);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.DURUM], islemTur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.SIRA], "=", sira));
            return KomutCalistir(sql, 0);
        }

        public Sonuc PasifYap(TNSDateTime islemZamani, int ipardDonemi, int dosyaAnahtar, int sira, string tabloAd)
        {
            string sql = "UPDATE " + tabloAd;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BITTARIH], islemZamani);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLAROB[(int)ENUMAlanlarOB.BITTARIH] + " IS NULL");
            sql = VeriErisim.KosulEkle(sql, "AND", ALANLAROB[(int)ENUMAlanlarOB.DURUM] + " = 5");
            return KomutCalistir(sql, 0);
        }

        public int YeniKayitSayisiBul(int ipardDonemi, int dosyaAnahtar, string tabloAd)
        {
            string sql = "SELECT COUNT(*) FROM " + tabloAd + " T";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAROB[(int)ENUMAlanlarOB.DURUM], "=", 1));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc YeniYap(TNSDateTime islemZamani, int ipardDonemi, int dosyaAnahtar, int sira, string tabloAd)
        {
            string sql = "UPDATE " + tabloAd;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.BASTARIH], islemZamani);
            sql += ", " + ALANLAROB[(int)ENUMAlanlarOB.ONAYLAYAN] + " = NULL";
            sql += ", " + ALANLAROB[(int)ENUMAlanlarOB.ONAYTARIHI] + " = NULL";
            sql += ", " + VeriErisim.AtamaOl("", ALANLAROB[(int)ENUMAlanlarOB.DURUM], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", ipardDonemi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", dosyaAnahtar));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.SIRA], "=", sira));

            Sonuc s = KomutCalistir(sql, 0);
            if (s.islemSonuc)
            {
                sql = "UPDATE " + tabloAd + " SET " + ALANLAROB[(int)ENUMAlanlarOB.BITTARIH] + " = NULL";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI], "=", ipardDonemi));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR], "=", dosyaAnahtar));
                sql = VeriErisim.KosulEkle(sql, "AND", ALANLAROB[(int)ENUMAlanlarOB.DURUM] + " = 5");
                sql += $" AND {ALANLAROB[(int)ENUMAlanlarOB.BITTARIH]} = (SELECT MAX({ALANLAROB[(int)ENUMAlanlarOB.BITTARIH]}) FROM {tabloAd} WHERE {ALANLAROB[(int)ENUMAlanlarOB.IPARDDONEMI]} = {ipardDonemi} and {ALANLAROB[(int)ENUMAlanlarOB.DOSYAANAHTAR]} = {dosyaAnahtar})";
                s = KomutCalistir(sql, 0);
            }
            return s;
        }
        #endregion "İşlemler Ortak"
    }
}
