using System;
using System.Collections;
using System.Data;
using System.Reflection.Emit;
using TNS;
using TNS.BKD;
using TNS.HRC;
using TNS.HRC.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;

namespace TNS.HRC.IsKurallari
{
    public static class HRCTabloDegisiklikleri
    {
        public static void Kaydet()
        {
            TabloDegisiklikleri.Kaydet("BaglantiSatiriHRC", komutlar);
        }

        private static SQLKomut[] komutlar ={

            new SQLKomut(1,"",@"ALTER TABLE dbo.HRCIDAREBILGI ADD IBAN varchar(50) NULL","")
            ,new SQLKomut(2,"",@"CREATE NONCLUSTERED INDEX [HRCFIRMABILGISI_ONERI1] ON [dbo].[HRCFIRMABILGISI] ([VERGINO])","")

            ,new SQLKomut(3,"",@"ALTER TABLE PSLMAASTEMELBILGITANIM ADD AILEYARDIMIKATSAYISI MONEY","")
            ,new SQLKomut(4,"",@"CREATE TABLE PSLGECICIGOREVYOLLUKMANAS(YIL INT, SURECNO VARCHAR(6), YI_YD INT, KIMLIKNO VARCHAR(11), UNVANKODU INT, GEREKCE VARCHAR(200), BASLANGICTARIH DATETIME, BITISTARIH DATETIME, BUTCETERTIBI VARCHAR(50), AVANSTUTARI MONEY, AVANSODEMEEMRI VARCHAR(50), ODEMEEMRI VARCHAR(50), DURUM INT, PRIMARY KEY(YIL, SURECNO))","")
            ,new SQLKomut(5,"",@"CREATE TABLE PSLGECICIGOREVYOLLUKMANASDETAY (YIL INT, SURECNO VARCHAR(6), SIRANO INT, TUR INT, GUN INT, GECELIKKONAKLAMAUCRETI MONEY, YERKOD VARCHAR(20), GUNDELIK MONEY, ODENECEK MONEY, ACIKLAMA VARCHAR(200), PRIMARY KEY(YIL, SURECNO, SIRANO) )","")
            ,new SQLKomut(6,"",@"CREATE TABLE dbo.HRCGUNDELIK(KOD int NOT NULL, YIL int NULL, UNVANKOD int NULL, COGRAFYAKOD varchar(50) NULL, GUNDELIK money NULL, PARABIRIMKOD int NULL, PRIMARY KEY CLUSTERED (KOD ASC))","")
            ,new SQLKomut(7,"",@"ALTER TABLE HRCGUNDELIK ADD TUR INT NOT NULL DEFAULT(1)","")
            ,new SQLKomut(8,"",@"ALTER TABLE dbo.PSLGECICIGOREVYOLLUKBELGE ADD YURTICIKONAKLAMATURU INT NOT NULL DEFAULT (1)","")
            ,new SQLKomut(9,"",@"ALTER TABLE dbo.HRCTEMINATMEKTUBU ADD SOZLESMEKOD int NULL","")
            ,new SQLKomut(10,"",@"ALTER TABLE dbo.HRCFIRMABILGISI ADD KARALISTE int NULL","")
            ,new SQLKomut(11,"",@"CREATE TABLE [dbo].[HRCBORDRORAPORSABLON](
                           [KOD] [int] NOT NULL,
                           [KULLANICIKODU] [varchar](30) NULL,
                           [AD] [varchar](250) NULL,
                           [ACIKLAMA] [varchar](max) NULL,
                               CONSTRAINT [PK_HRCBORDRORAPORSABLON] PRIMARY KEY CLUSTERED ([KOD] ASC
                              )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                              ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]","")

            ,new SQLKomut(12,"",@"ALTER TABLE dbo.HRCTEKLIF ADD PARABIRIMI int NULL","")
            ,new SQLKomut(13,"",@"ALTER TABLE dbo.HRCTEKLIFGENELTOPLAM ADD PARABIRIMI int NULL, TEKLIFTARIHI datetime NULL","")
            ,new SQLKomut(14,"",@"ALTER TABLE HRCFIRMABILGISI ADD ACIKLAMA varchar(500)","")
            ,new SQLKomut(15,"",@"ALTER TABLE PSLMAASTEMELBILGITANIM ADD TOPLUGORUSMESABITI MONEY","")
            ,new SQLKomut(16,"",@"ALTER TABLE HRCBORDROTABLODETAY ADD YIL INT NOT NULL DEFAULT 0","")
            ,new SQLKomut(17,"",@"CREATE TABLE HRCBORDROTANIMANA
                               (BORDROTUR INT
                               , BORDROISLEMTUR INT
                               , BORDROAD VARCHAR(50)
                               , KAYNAKBORDROTUR INT
                               , KAYNAKBORDROISLEMTUR INT
                               , PRIMARY KEY (BORDROTUR, BORDROISLEMTUR))","")

            ,new SQLKomut(18,"",@"ALTER TABLE PSLMAASTEMELBILGITANIM ADD SSKTAVAN MONEY","")
            ,new SQLKomut(19,"",@"CREATE TABLE [dbo].[HRCKOMISYONUYESABLON](
                                [KOD] [varchar](40) NOT NULL,
                                [SAHIBI] [varchar](50) NULL,
                                [ADI] [varchar](150) NULL,
                                [DURUM] [int] NULL,
                                    CONSTRAINT [PK_HRCKOMISYONUYESABLON] PRIMARY KEY CLUSTERED 
                                   ([KOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]","")

            ,new SQLKomut(20,"",@"CREATE TABLE [dbo].[HRCKOMISYONUYESABLONDETAY](
                                 [KOD] [varchar](40) NOT NULL,
                                 [KOMISYONUYESABLONKOD] [varchar](40) NULL,
                                 [SIRANO] [int] NOT NULL,
                                 [KOMISYONGOREV] [int] NULL,
                                 [AD] [varchar](255) NULL,
                                 [GOREV] [varchar](255) NULL,
                                 [UNVAN] [varchar](255) NULL,
                                 [SICILNO] [varchar](255) NULL,
                                 [TELEFON] [varchar](255) NULL,
                                 [CEPTELEFON] [varchar](255) NULL,
                                  CONSTRAINT [PK_HRCKOMISYONUYESABLONDETAY] PRIMARY KEY CLUSTERED 
                                  ([KOD] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]","")

            ,new SQLKomut(21,"",@"CREATE NONCLUSTERED INDEX [IX_HRCKOMISYONUYESABLONDETAY] ON [dbo].[HRCKOMISYONUYESABLONDETAY]
                                ([KOMISYONUYESABLONKOD] ASC ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]","")

            ,new SQLKomut(22,"",@"ALTER TABLE dbo.HRCTEKLIFGENELTOPLAM ADD TEKLIFTARIHI datetime NULL","")
            ,new SQLKomut(23,"",@"ALTER TABLE dbo.HRCTASINIRFORM ADD DOVIZKUR decimal(24, 18) NULL","")
            ,new SQLKomut(24,"",@"ALTER TABLE dbo.HRCODEMEEMRIFORM ADD PRFATURANOEK varchar(500) NULL","")
            ,new SQLKomut(25,"",@"ALTER TABLE dbo.HRCTEMINATMEKTUBU ADD EPOSTAUYARIGONDTARIH datetime NULL","")
            ,new SQLKomut(26,"",@"ALTER TABLE dbo.HRCABONEFATURA ADD	DAMGAVERGISI money NULL","")
            ,new SQLKomut(27,"",@"Create table HRCGGYTALEPBASLIK(
                                YIL INT, 
                                FORMKOD VARCHAR(20), 
                                MUHASEBE INT, 
                                BIRIM VARCHAR(20), 
                                KURKOD VARCHAR(20), 
                                DURUM INT, 
                                TARIH DATETIME, 
                                TCKIMLIKNO VARCHAR(20), 
                                AD VARCHAR(200), 
                                SOYAD VARCHAR(200), 
                                UNVAN VARCHAR(20), 
                                GUNDELIKYIL INT, 
                                GUNDELIKKOD VARCHAR(20), 
                                GUNDELIKTUTAR DECIMAL(18,2), 
                                GUNDELIKDOVIZKOD INT, 
                                NEREDEN VARCHAR(200), 
                                NEREYE VARCHAR(200), 
                                BASLANGICTARIH DATETIME, 
                                BITISTARIH DATETIME, 
                                SURECYIL INT, 
                                SURECNO VARCHAR(20), 
                                TUR INT, 
                                GOREVSURESI INT, 
                                GEREKCE VARCHAR(200), 
                                ACIKLAMALAR VARCHAR(200), 
                                TERTIP VARCHAR(200), 
                                AVANS MONEY, 
                                SUNANAD VARCHAR(200), 
                                SUNANUNVAN VARCHAR(200), 
                                HAZIRAD VARCHAR(200), 
                                HAZIRUNVAN VARCHAR(200), 
                                ONAYAD VARCHAR(200), 
                                ONAYUNVAN VARCHAR(200), 
                                ONAYMAKAM VARCHAR(200), 
                                AVANSODEMEEMRI VARCHAR(20), 
                                PRIMARY KEY (YIL, FORMKOD))","")

            ,new SQLKomut(28,"",@"Create table HRCGGYTALEPDETAY(
                                  YIL INT, 
                                  FORMKOD VARCHAR(20), 
                                  SIRANO INT, 
                                  TUR INT, 
                                  TARIH DATETIME, 
                                  GUN VARCHAR(20), 
                                  PRIMARY KEY (YIL, FORMKOD, SIRANO))","")

            ,new SQLKomut(29,"",@"Create table HRCGGYTALEPBERABER(
                                YIL INT, 
                                FORMKOD VARCHAR(20), 
                                SIRANO INT, TCKIMLIK VARCHAR(20), 
                                PRIMARY KEY (YIL, FORMKOD, SIRANO))","")

            ,new SQLKomut(30,"",@"create table HRCBORDROEPOSTAGONDER(SURECYIL INT, SURECNO VARCHAR(20), BORDROTUR INT, BORDROISLEMTUR INT, TCKIMLIKNO VARCHAR(20), EPOSTA VARCHAR(100), EKLEMEZAMANI DATETIME, EKLEYEN VARCHAR(100), SONUCZAMANI DATETIME, SONUCSTR VARCHAR(200), ISLEMEALINDI DATETIME, PRIMARY KEY(SURECYIL, SURECNO, BORDROTUR, BORDROISLEMTUR, TCKIMLIKNO, EKLEMEZAMANI))","")
            ,new SQLKomut(31,"",@"ALTER TABLE dbo.HRCBORDRODONEM ADD BORDROTUR varchar(250) NULL, BORDROISLEM INT NULL","")
            ,new SQLKomut(32,"",@"ALTER TABLE dbo.PSLCESITLIODEMEBORDRO ADD AVANSTUTAR money NULL, AVANSODEMEEMRINO varchar(20) NULL, ODEMEGUN DATETIME NULL, ODEMESIRANO INT NULL, AVANSODEMEGUN DATETIME NULL, AVANSODEMESIRANO INT NULL","")
            ,new SQLKomut(33,"",@"ALTER TABLE dbo.HRCKOMISYONUYE ADD TESLIMALAN varchar(50) NULL, TESLIMTARIH datetime NULL","")
            ,new SQLKomut(34,"",@"ALTER TABLE dbo.PSLCESITLIODEMEBORDRO ADD PARABIRIMI int NULL","")
            ,new SQLKomut(35,"",@"ALTER TABLE dbo.HRCTEKLIFGENELTOPLAM ADD PARABIRIMI int NULL","")
            ,new SQLKomut(36,"",@"ALTER TABLE dbo.HRCFATURAFORM ADD PARABIRIMI int NULL","")
            ,new SQLKomut(37,"",@"ALTER TABLE dbo.HRCTEKLIF ADD PARABIRIMI int NULL","")
            ,new SQLKomut(38,"",@"ALTER TABLE HRCBORDROPARAM ADD TIP INT NOT NULL DEFAULT 1, YONTEM INT NOT NULL DEFAULT 1, LISTE VARCHAR(2000)","")
            ,new SQLKomut(39,"",@"CREATE TABLE HRCUASTFIHTIYACBASLIK (
                              YIL INT NOT NULL,
                              FORMKOD VARCHAR(20)  NOT NULL,
                              SIRANO INT  NOT NULL,
                              ACIKLAMA VARCHAR(500)  NOT NULL,
                              PRIMARY KEY (YIL, FORMKOD, SIRANO))","")

            ,new SQLKomut(40,"",@"CREATE TABLE HRCUASTFIHTIYACDETAY (
                               YIL INT NOT NULL,
                               FORMKOD VARCHAR(20)  NOT NULL,
                               SIRANO INT NOT NULL,
                               DETAYSATIRNO INT NOT NULL,
                               IHTIYACKOD VARCHAR(20) NOT NULL,
                               IHTIYACAD VARCHAR(200) NOT NULL,
                               MIKTAR1 DECIMAL(18,6) NOT NULL,
                               OLCUBIRIMKOD1 INT NOT NULL,
                               MIKTAR2 DECIMAL(18,6),
                               OLCUBIRIMKOD2 INT,
                               ACIKLAMA VARCHAR(200) NOT NULL,
                               PRIMARY KEY (YIL, FORMKOD, SIRANO, DETAYSATIRNO))","")
            ,new SQLKomut(41, "", @"CREATE NONCLUSTERED INDEX IX_HRCFIRMABILGISI_VD1 ON dbo.HRCFIRMABILGISI 	(	VERGIDAIRESI	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]","")
            ,new SQLKomut(42, "", @"DROP INDEX _dta_index_HRCFATURAFORM_15_1581248688__K1_K8_K6_K18_K7_K4_K2_K3_5_9_10_11_12_13_14_15_16_17_19_20 ON dbo.HRCFATURAFORM","")
            ,new SQLKomut(43, "", @"ALTER TABLE HRCFATURAFORM ALTER COLUMN FATURANO varchar(255) NOT NULL","")
            ,new SQLKomut(44, "", @"CREATE NONCLUSTERED INDEX _dta_index_HRCFATURAFORM_15_1581248688__K1_K8_K6_K18_K7_K4_K2_K3_5_9_10_11_12_13_14_15_16_17_19_20 ON dbo.HRCFATURAFORM	(	YIL,	FATURATARIH,	FIRMANO,	DURUM,	FATURANO,	SURECNO,	MUHASEBEKOD,	BIRIMKOD,	ISKONTO	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]","")
            ,new SQLKomut(45, "", @"CREATE TABLE HRCOKAS(KOD varchar(50) NOT NULL, AD varchar(2000) NOT NULL, KODLEVEL int NOT NULL, KALEMTURU varchar(20) NOT NULL, OKASKOD varchar(50) NOT NULL, AD2 varchar(2000) NULL, PRIMARY KEY(KOD))", "")
            ,new SQLKomut(46, "", @"ALTER TABLE dbo.HRCIDAREBILGI ADD VADI varchar(255) NULL", "")
            ,new SQLKomut(47, "", @"ALTER TABLE dbo.HRCFIRMABILGISI ADD ADRESNO varchar(255) NULL", "")
            ,new SQLKomut(48, "", @"ALTER TABLE dbo.HRCSEYYARGOREVREVIZEFORM ADD ICMALNO int NULL", "")
            ,new SQLKomut(49, "", @"CREATE TABLE [dbo].[HRCSURECTARIHCE]([KOD] [varchar](40) NOT NULL, 	[YIL] [int] NOT NULL, [SURECNO] [varchar](50) NOT NULL,	[SIRANO] [int] Identity(1, 1),	[ISLEMKOD] [int] NOT NULL,	[PARCAKOD] [int] NOT NULL,	[TARIH] [datetime] NULL,	[KULLANICIAD] [varchar](255) NULL,	[DURUM] [int] NULL, CONSTRAINT [PK_HRCSURECTARIHCE] PRIMARY KEY CLUSTERED ([KOD] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]", "")
            ,new SQLKomut(50, "", @"ALTER TABLE HRCDOGRUDANTEMINPLAN ADD DTHARIC MONEY", "")
            ,new SQLKomut(51, "", @"ALTER TABLE HRCDOGRUDANTEMINPLAN ADD ONCEALIM MONEY", "")
            ,new SQLKomut(52, "", @"CREATE TABLE HRCBORDROKISIBORCUTANIM(ID INT IDENTITY(1,1) PRIMARY KEY,
	                                PERSONELTIP INT NOT NULL,
	                                ALANAD varchar(80) NOT NULL,
	                                DEGISKENAD varchar(80) NOT NULL,
	                                TUR int NOT NULL,
	                                HEP30 int NOT NULL) ", "")
            ,new SQLKomut(53, "", @"ALTER TABLE HRCBORDROSABLON ADD SIRANO VARCHAR(10) NOT NULL DEFAULT(''), TEKKISILIK INT, GECERLIKODLAR VARCHAR(100), SAYFA INT NOT NULL DEFAULT 1, ORTAKRAPOR INT", "")
            ,new SQLKomut(54, "", "UPDATE HRCBORDROSABLON SET TEKKISILIK= 1 WHERE DOSYAKOD=99", "")
            ,new SQLKomut(55, "", "UPDATE HRCBORDROSABLON SET GECERLIKODLAR=CONVERT(VARCHAR, BORDROTUR)+'-'+CONVERT(VARCHAR, ISLEMTUR) WHERE BORDROTUR>0","")
            ,new SQLKomut(56, "", @"UPDATE HRCBORDROSABLON SET ORTAKRAPOR=1 WHERE DOSYAKOD>=80 AND DOSYAKOD<99","")
            ,new SQLKomut(57, "", @"CREATE TABLE HRCMAHKEMEHARC (SURECYIL INT NOT NULL, MUHASEBEKOD INT NOT NULL, BIRIMKOD VARCHAR(4) NOT NULL, SURECNO  VARCHAR(50) NOT NULL, SIRANO INT NOT NULL, BIRIMACIKLAMA VARCHAR(250) NOT NULL,
              DAVACI VARCHAR(250) NOT NULL, DAVAKONUSU VARCHAR(250) NOT NULL, MAHKEMEKARARNO VARCHAR(250) NOT NULL, ICRATAKIPDURUMU VARCHAR(250),
              YARGITAYDANISTAYKARARI VARCHAR(250), KESINLESMISMAHKEMEKARARI VARCHAR(250), KESINLESTIMI VARCHAR(250), MIKTAR MONEY NOT NULL, ODEMEEMRINO VARCHAR(250),
              PRIMARY KEY(SURECYIL, MUHASEBEKOD, BIRIMKOD, SURECNO, SIRANO))", "")
            ,new SQLKomut(58, "", @"CREATE TABLE DBSOZLESME (SOZLESMEID INT IDENTITY(1,1) PRIMARY KEY,
                BUTCEREFID INT NOT NULL,  
                SOZLESMENO INT NULL,  
                KONU VARCHAR(50) NULL,
                ACIKLAMA VARCHAR(50) NULL,
                FIRMAVD VARCHAR(50) NOT NULL,  
                FIRMAVNO VARCHAR(25) NOT NULL,  
                FIRMAADI VARCHAR(255) NOT NULL,  
                FIRMAIBAN VARCHAR(50) NOT NULL,  
                SOZLESMETARIHI DATE NOT NULL,  
                PARABIRIMI VARCHAR(3), 
                KUREURUSD DECIMAL(18,10) NULL,  
                KURUSDTRY DECIMAL(18,10) NULL,  
                KUREURTRY DECIMAL(18,10) NULL,  
                SOZLESMETUTARI MONEY NOT NULL,  
                AVANSHUKMU INT NOT NULL DEFAULT 0,  
                AVANSTEMINAT INT NOT NULL DEFAULT 0,  
                KESINTEMINAT INT NOT NULL DEFAULT 0,  
                AVANSORANI MONEY NULL,  
                AVANSMAHSUBU INT NOT NULL DEFAULT 0,  
                AVANSMAHSUBUORANI MONEY NULL,  
                IHTIYATKESINTISI INT NOT NULL DEFAULT 0,  
                SARFMALZEMESI INT NOT NULL DEFAULT 0,  
                DEMIRBAS INT NOT NULL DEFAULT 0,  
                TEMINATMEKTUBUNO VARCHAR(100) NULL,  
                DURUM INT NOT NULL)", "")
            ,new SQLKomut(59, "", @"CREATE TABLE DBODEMEPLANIVERSIYON (ODEMEPLANID INT IDENTITY(1,1) PRIMARY KEY,
                SOZLESMEID INT NOT NULL,  
                VERSIYONTARIHI DATETIME NOT NULL DEFAULT GETDATE(),  
                DURUM INT  NOT NULL,
                FOREIGN KEY (SOZLESMEID) REFERENCES DBSOZLESME(SOZLESMEID))", "")
            ,new SQLKomut(60, "", @"CREATE TABLE DBODEMEPLANIDETAY (ODEMEPLANID INT NOT NULL,  
                YIL INT NOT NULL,  
                AY INT NOT NULL CHECK (AY BETWEEN 1 AND 12),  
                ODENECEKTUTAR MONEY NOT NULL,
                PRIMARY KEY (ODEMEPLANID, YIL, AY),
                FOREIGN KEY (ODEMEPLANID) REFERENCES DBODEMEPLANIVERSIYON(ODEMEPLANID))", "")
            ,new SQLKomut(61, "", @"CREATE TABLE DBIKRAZ
                ( IKRAZID INT IDENTITY(1,1) PRIMARY KEY
                , ANLASMAADITR VARCHAR(200)
                , ANLASMAADIEN VARCHAR(200)
                , ANLASMAKODU VARCHAR(20)
                , BASLANGICYILI INT
                , BITISYILI INT
                , PARABIRIMI VARCHAR(3)
                , DONUSUMKURU DECIMAL(18,14))", "")
            ,new SQLKomut(62, "", @"CREATE TABLE DBBILESEN
                ( BILESENID INT IDENTITY(1,1) PRIMARY KEY
                , IKRAZID INT
                , BILESENNO VARCHAR(20)
                , BILESENAD VARCHAR(200)
                , FOREIGN KEY (IKRAZID) REFERENCES DBIKRAZ(IKRAZID))", "")
            ,new SQLKomut(63, "", @"CREATE TABLE DBALTBILESEN
                ( ALTBILESENID INT IDENTITY(1,1) PRIMARY KEY
                , BILESENID INT
                , ALTBILESENNO VARCHAR(20)
                , ALTBILESENAD VARCHAR(200)
                , ILGILIBIRIMNO VARCHAR(20)
                , ILGILIBIRIMADI VARCHAR(200)
                , ILGILIBIRIMBUTCEKODU VARCHAR(40)
                , TOPLAMBUTCE MONEY
                , FOREIGN KEY (BILESENID) REFERENCES DBBILESEN(BILESENID))", "")
            ,new SQLKomut(65, "", @"ALTER TABLE DBALTBILESEN ADD BAGYIL INT, ILGILIPEBKOD VARCHAR(40)", "")
            ,new SQLKomut(66, "", @"CREATE TABLE DBSATINALMAPLANI
                (SATINALMAID INT IDENTITY(1,1) PRIMARY KEY
                , ALTBILESENID INT NOT NULL
                , SATINALMANO INT NOT NULL
                , SATINALMAGRUPNO INT NOT NULL
                , ACIKLAMATR VARCHAR(800)
                , ACIKLAMAEN VARCHAR(800)
                , REFERANSNO VARCHAR(40)
                , REFERANSGRUPEK VARCHAR(40)
                , REFERANSEK VARCHAR(40)
                , BUTCEKATEGORI VARCHAR(40)
                , SATINALMAYONTEMI VARCHAR(40)
                , SECIMYONTEMI VARCHAR(40)
                , ONCESONRA INT
                , ODENEK MONEY
                , TAHMINIDAVETTARIHI VARCHAR(40)
                , PLANLANANIMZATARIHI VARCHAR(40)
                , PLANLANANBITISTARIHI VARCHAR(40)
                , FOREIGN KEY (ALTBILESENID) REFERENCES DBALTBILESEN(ALTBILESENID))", "")
            ,new SQLKomut(67, "", @"CREATE VIEW HRCSEYYARGOREVGERCEKLESENSUREC AS
                SELECT S.YIL YIL, O.KOD HARCAMABIRIMI, B.ODEMETUR ISTIHDAMSEKLI, B.CARPAN GUNSAYISI, B.CARPAN * B.SABITTUTAR MALIYET, S.SURECNO, S.SURECDURUM
                FROM PSLCESITLIODEMEBORDRO AS B 
                INNER JOIN HRCSUREC AS S ON B.YIL = S.YIL AND B.SURECNO = S.SURECNO 
                INNER JOIN BHMKURUMSALYAPI O ON O.SAYMANLIKKOD = S.MUHASEBEKOD AND O.BIRIMKOD = S.BIRIMKOD
                where S.ISLEMKOD=95", "")
            ,new SQLKomut(68, "", @"ALTER TABLE DBSATINALMAPLANI ADD ALTBIRIMAD VARCHAR(40)", "")
            ,new SQLKomut(69, "", @"CREATE TABLE DBBUTCEREF
                (BUTCEREFID INT IDENTITY(1,1) PRIMARY KEY
                , SATINALMAID INT
                , REFERANSNO VARCHAR(40)
                , ACIKLAMATR VARCHAR(200)
                , ACIKLAMAEN VARCHAR(200)
                , ODENEK MONEY
                , DURUM INT
                , FOREIGN KEY (SATINALMAID) REFERENCES DBSATINALMAPLANI(SATINALMAID))", "")
            ,new SQLKomut(70, "", @"CREATE TABLE DBHAKEDIS
                (HAKEDISID INT IDENTITY(1,1) PRIMARY KEY
                , BUTCEREFID INT
                , ILGILIKOD INT
                , HAKEDISTURKOD INT
                , SOZLESMEID INT
                , BELGETURKOD INT
                , BELGETARIHI DATETIME
                , BELGENO VARCHAR(100)
                , PARABIRIMI VARCHAR(3)
                , KURTARIHI DATETIME
                , KDVHARICBELGETOPLAMI MONEY
                , KDVGIRISTURKOD INT
                , KDVORANI MONEY
                , KDVTUTARI MONEY
                , AVANSKESINTISI MONEY
                , DURUMKOD INT
                , FOREIGN KEY (BUTCEREFID) REFERENCES DBBUTCEREF(BUTCEREFID))", "")
            ,new SQLKomut(71, "", @"CREATE TABLE DBHAKEDISSAHIBI
                    (HAKEDISID INT
                    , SIRANO INT
                    , ILGILIKOD INT
                    , KESINTITURKOD INT
                    , ACIKLAMA VARCHAR(200)
                    , KESINTITUTARI MONEY
                    , NUMARATOR INT
                    , MERKEZNUMARATORID INT
                    , ODENDI INT
                    , PRIMARY KEY (HAKEDISID, SIRANO)
                    , FOREIGN KEY (HAKEDISID) REFERENCES DBHAKEDIS(HAKEDISID))", "")
            ,new SQLKomut(72, "", @"CREATE TABLE DBYOLLUKBILGISI
                    (HAKEDISID INT
                    , SIRANO INT
                    , YER VARCHAR(200)
                    , ACIKLAMA VARCHAR(200)
                    , TUTAR MONEY
                    , PRIMARY KEY (HAKEDISID, SIRANO)
                    , FOREIGN KEY (HAKEDISID) REFERENCES DBHAKEDIS(HAKEDISID))", "")
            ,new SQLKomut(73, "", @"CREATE TABLE DBHAKEDISMALZEME
                    (HAKEDISID INT
                    , SIRANO INT
                    , MALZEMEADI VARCHAR(200)
                    , MIKTAR INT
                    , BIRIMFIYAT MONEY
                    , PRIMARY KEY (HAKEDISID, SIRANO)
                    , FOREIGN KEY (HAKEDISID) REFERENCES DBHAKEDIS(HAKEDISID))", "")
            ,new SQLKomut(74, "", @"CREATE TABLE DBTARIHCE
                    (TARIHCEID INT IDENTITY(1,1) PRIMARY KEY
                    , MODULKOD VARCHAR(20)
                    , MODULANAHTAR VARCHAR(50)
                    , ISLEMTARIHI DATETIME
                    , ISLEMIYAPAN VARCHAR(40)
                    , ISLEM VARCHAR(200)
                    , EKBILGI VARCHAR(200))", "")
            ,new SQLKomut(75, "", @"CREATE TABLE DBEKLENEBILECEKDOSYALAR
                    (KOD INT IDENTITY(1,1) PRIMARY KEY
                    , MODULKOD VARCHAR(20)
                    , SIRANO INT
                    , AD VARCHAR(200))", "")
            ,new SQLKomut(76, "", @"CREATE TABLE DBMERKEZNUMARATOR (
                    MERKEZNUMARATORID INT IDENTITY(1,1) PRIMARY KEY
                    , ALTBILESENID INT NOT NULL
                    , NUMARATOR INT NOT NULL
                    , ISLEMTARIHI DATETIME
                    , ISLEMIYAPAN VARCHAR(40)
                    , DURUM INT NOT NULL
                    , FOREIGN KEY (ALTBILESENID) REFERENCES DBALTBILESEN(ALTBILESENID))", "")
            ,new SQLKomut(77, "", @"ALTER TABLE DBHAKEDISSAHIBI WITH CHECK ADD FOREIGN KEY(MERKEZNUMARATORID) REFERENCES DBMERKEZNUMARATOR (MERKEZNUMARATORID)", "")
            ,new SQLKomut(78, "", @"ALTER TABLE DBIKRAZ ADD IKRAZNO VARCHAR(50), MBHESAPNO VARCHAR(50)", "")
            ,new SQLKomut(79, "", @"ALTER TABLE DBHAKEDIS ADD GEREKCE VARCHAR(200)", "")
            ,new SQLKomut(80, "", @"CREATE TABLE DBIMZA(IMZAID INT IDENTITY(1,1) PRIMARY KEY
                , ALTBILESENID INT NOT NULL
                , ADSOYAD VARCHAR(100) NOT NULL
                , SATIR1 VARCHAR(100)
                , SATIR2 VARCHAR(100)
                , FOREIGN KEY (ALTBILESENID) REFERENCES DBALTBILESEN(ALTBILESENID))", "")
            ,new SQLKomut(81, "", @"ALTER TABLE DBMERKEZNUMARATOR ADD IMZASOLID INT NULL, IMZASAGID INT NULL;", "")
            ,new SQLKomut(82, "", @"ALTER TABLE DBMERKEZNUMARATOR 
                ADD CONSTRAINT FK_DBMERKEZNUMARATOR_IMZASOL FOREIGN KEY (IMZASOLID) REFERENCES DBIMZA(IMZAID),
                CONSTRAINT FK_DBMERKEZNUMARATOR_IMZASAG FOREIGN KEY (IMZASAGID) REFERENCES DBIMZA(IMZAID);", "")
            ,new SQLKomut(83, "", @"CREATE TABLE DBBUTCEREFLOT(
	              BUTCEREFLOTID int IDENTITY(1,1) PRIMARY KEY NOT NULL
	            , BUTCEREFID int NOT NULL
	            , LOTNO int NOT NULL
	            , ACIKLAMA varchar(50) NULL
	            , FOREIGN KEY (BUTCEREFID) REFERENCES DBBUTCEREF(BUTCEREFID))", "")
            , new SQLKomut(84, "", @"ALTER TABLE DBSOZLESME ADD BUTCEREFLOTID INT NULL", "")
            , new SQLKomut(85, "", @"ALTER TABLE DBSOZLESME 
                ADD CONSTRAINT FK_DBSOZLESME_LOT FOREIGN KEY (BUTCEREFLOTID) REFERENCES DBBUTCEREFLOT(BUTCEREFLOTID)", "")
            , new SQLKomut(86, "", @"ALTER TABLE DBSOZLESME 
                ADD CONSTRAINT FK_DBSOZLESME_BUTCEREF FOREIGN KEY (BUTCEREFID) REFERENCES DBBUTCEREF(BUTCEREFID)", "")
            , new SQLKomut(87, "", @"CREATE TABLE DBHAKEDISODENEKLER(HAKEDISID int NOT NULL
	            , EKOKOD VARCHAR(30) NOT NULL
	            , MIKTAR MONEY NOT NULL
                , PRIMARY KEY (HAKEDISID, EKOKOD)
	            , FOREIGN KEY (HAKEDISID) REFERENCES DBHAKEDIS(HAKEDISID))", "")
            , new SQLKomut(88, "", @"sp_rename 'DBODEMEPLANIVERSIYON', 'DBODEMEPLANI'", "")
            , new SQLKomut(89, "", @"sp_rename 'DBODEMEPLANI.VERSIYONTARIHI', 'ODEMEPLANITARIHI', 'COLUMN'", "")
            , new SQLKomut(90, "", @"ALTER TABLE DBODEMEPLANI ADD ACIKLAMA VARCHAR(MAX), ODEMEPLANITIPI INT", "")
            , new SQLKomut(91, "", @"ALTER TABLE DBHAKEDISSAHIBI ADD ODEMETARIHI DATETIME", "")
            , new SQLKomut(92, "", @"alter TABLE DBSOZLESME alter column SOZLESMETUTARI money NULL", "")
            , new SQLKomut(93, "", @"alter TABLE DBSOZLESME alter column SOZLESMENO int NULL; alter TABLE DBSOZLESME alter column AVANSTEMINAT varchar(200) NULL; alter TABLE DBSOZLESME alter column KESINTEMINAT varchar(200) NULL;", "")
            , new SQLKomut(94, "", @"ALTER TABLE DBSOZLESME ADD TAHMINIBASLAMATARIHI DATE, AVANSPARABIRIMI VARCHAR(3), SOZLESMENOALMATARIHI DATETIME", "")
            , new SQLKomut(95, "", @"ALTER TABLE DBODEMEPLANI ADD TAHMINISOZLESMEBITISTARIHI DATE", "")
            , new SQLKomut(96, "", @"ALTER TABLE DBSOZLESME ADD GEREKCE VARCHAR(500)", "")
            , new SQLKomut(97, "", @"CREATE TABLE DBSATINALMAPLANIAYDAGILIM(
                   DBSATINALMAPLANIAYDAGILIMID int IDENTITY(1,1) NOT NULL,
                   SATINALMAID int NOT NULL,
                   PLANTARIHI datetime NOT NULL,
                   DURUM int NOT NULL,
                   ACIKLAMA varchar(max) NULL,
                   PRIMARY KEY(DBSATINALMAPLANIAYDAGILIMID),
                   FOREIGN KEY(SATINALMAID) REFERENCES DBSATINALMAPLANI (SATINALMAID))", "")
            , new SQLKomut(98, "", @"CREATE TABLE DBSATINALMAPLANIAYDAGILIMDETAY(
                   DBSATINALMAPLANIAYDAGILIMID int NOT NULL,
                   YIL int  NOT NULL,
                   AY int  NOT NULL,
                   ODENEK MONEY NOT NULL,
                   PRIMARY KEY (DBSATINALMAPLANIAYDAGILIMID, YIL, AY),
                   FOREIGN KEY (DBSATINALMAPLANIAYDAGILIMID) REFERENCES DBSATINALMAPLANIAYDAGILIM (DBSATINALMAPLANIAYDAGILIMID))", "")
            , new SQLKomut(99, "", @"ALTER TABLE DBHAKEDIS ADD HAKEDISNO INT NULL, NUMARATORALMATARIHI DATE", "")
            , new SQLKomut(100, "", @"ALTER TABLE DBHAKEDIS ADD CONSTRAINT FK_DBHAKEDIS_DBSOZLESME FOREIGN KEY (SOZLESMEID) REFERENCES DBSOZLESME (SOZLESMEID);", "")
            , new SQLKomut(101, "", @"CREATE TABLE DBEKSTRELER(
	            NUMARATOR nvarchar(50) NOT NULL,
	            IKRAZID int NOT NULL,
	            EKSDATE date NOT NULL,
	            EKSDATESIRANO int NULL,
	            CATNO nchar(10) NULL,
	            CUR nchar(10) NULL,
	            AMOUNTPAID money NULL,
	            EXCHRATE money NULL,
	            TRYEQUIV money NULL,
	            EUREQUIV money NULL,
	            DEPOSITEUR money NULL,
	            DEPOSITTRY money NULL, 
                CONSTRAINT PK_DBEKSTRELER PRIMARY KEY CLUSTERED (NUMARATOR ASC, IKRAZID ASC, EKSDATE ASC),
                FOREIGN KEY(IKRAZID) REFERENCES DBIKRAZ(IKRAZID))", "")
            , new SQLKomut(102, "", @"ALTER TABLE DBEKSTRELER ADD SGBNUMARATOR INT NULL", "")
            , new SQLKomut(103, "", @"ALTER TABLE DBIMZA ADD PASIF INT NOT NULL DEFAULT 0", "")
            , new SQLKomut(104, "", @"ALTER TABLE DBEKSTRELER ADD EXPLANATION VARCHAR(500) NULL", "")
        };
    }
}
