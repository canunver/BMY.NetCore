using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
using TNS.BHMv2.IsKurallari.Veri;
using TNS.KYM;
using System.Collections;

namespace TNS.BHMv2.IsKurallari
{
    public class BHMv2TabloDegisiklikleri
    {
        public static void Kaydet()
        {
            //Sözlük için
            TabloDegisiklikleri.Kaydet(BHMv2Genel.VeriErisimAdBulSozluk(), komutlarSozluk);

            //BHMv2 için
            TabloDegisiklikleri.Kaydet(BHMv2Genel.VeriErisimAdBul(0), komutlar);
        }

        private static SQLKomut[] komutlarSozluk ={
            new SQLKomut(1,"",@"CREATE TABLE [dbo].[PRGKODYAPI](
								[YIL] [int] NOT NULL,
								[PRGTUR] [int] NOT NULL,
								[KOD] [int] NOT NULL,
								[AD] [varchar](500) NULL,
								[TUR] [int] NULL,
								 CONSTRAINT [PK_PRGKODYAPI] PRIMARY KEY CLUSTERED ( [YIL] ASC, [PRGTUR] ASC, [KOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]","")
            , new SQLKomut(2,"",@"CREATE TABLE [dbo].[PRGPEBILISKI](
								[YIL] [int] NOT NULL,
								[PEBKOD] [varchar](20) NOT NULL,
								[PRGKOD] [int] NULL,
								[ALTPRGKOD] [int] NULL,
								[FAALKOD] [int] NULL,
								[ALTFAALKOD] [int] NULL,
								 CONSTRAINT [PK_PRGPEBILISKI] PRIMARY KEY CLUSTERED ( [YIL] ASC, [PEBKOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]","")
            , new SQLKomut(3,"",@"ALTER TABLE dbo.PRGPEBILISKI ADD FONKOD varchar(20) NULL","")
            , new SQLKomut(4,"",@"EXECUTE sp_rename N'dbo.PRGKODYAPI.PRGTUR', N'KODTUR', 'COLUMN'","")

            , new SQLKomut(5, "", @"ALTER TABLE dbo.BHFHIZMETGEREKCESI ADD FAALIYETKOD int NOT NULL DEFAULT 0", "")
            , new SQLKomut(6, "", @"declare @const nvarchar(200)
                                    set @const = QUOTENAME((
                                    select kc.name from sys.key_constraints as kc
                                    where kc.parent_object_id = object_id('BHFHIZMETGEREKCESI') and kc.type = 'PK'))
                                    exec('ALTER TABLE BHFHIZMETGEREKCESI DROP CONSTRAINT ' + @const)", "")
            , new SQLKomut(7, "", @"ALTER TABLE dbo.BHFHIZMETGEREKCESI ADD CONSTRAINT PK_BHFHIZMETGEREKCESI PRIMARY KEY CLUSTERED (
                                  YIL, KURUMSAL, PARCA, FAALIYETKOD) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]", "")

            , new SQLKomut(8, "", @"create table BHFKULLANILANTASITLAR
									(
									 YIL INT not null, 
									 KURUMSAL varchar(20) not null, 
									 MERKEZTASRA int not null,
									 TKOD varchar(20) not null, 
									 KAYNAK  int not null,
									 YAS int not null,
									 ADET int not null,
									 primary key (YIL, KURUMSAL, MERKEZTASRA, TKOD, KAYNAK, YAS)
									)", "")
            , new SQLKomut(9, "", @"create view PRGPROGRAM as SELECT YIL, KOD PRGKOD, AD PRGAD, AD PRGAD2 FROM PRGKODYAPI WHERE KODTUR=1", "")
            , new SQLKomut(10, "", @"create view PRGALTPROGRAM as SELECT YIL, KOD APRGKOD, AD APRGAD, AD APRGAD2 FROM PRGKODYAPI WHERE KODTUR=2", "")
            , new SQLKomut(11, "", @"create view PRGFAALIYET as SELECT YIL, KOD FAALKOD, AD FAALAD, AD FAALAD2 FROM PRGKODYAPI WHERE KODTUR=3", "")
            , new SQLKomut(12, "", @"create view PRGALTFAALIYET as SELECT YIL, KOD AFAALKOD, AD AFAALAD, AD AFAALAD2 FROM PRGKODYAPI WHERE KODTUR=4", "")
            , new SQLKomut(13, "", @"create TABLE PRGPROGRAMTUR (PTKOD INT NOT NULL, PTAD varchar(100) NOT NULL, PTAD2 varchar(100), PRIMARY KEY(PTKOD))", "")
            , new SQLKomut(14, "", @"INSERT INTO PRGPROGRAMTUR (PTKOD, PTAD, PTAD2)VALUES (1, 'PROGRAM', 'PROGRAM'),(2, 'GENEL Y.G.', 'GENEL Y.G.'),(3, 'TRANSFER', 'TRANSFER') ", "")

            , new SQLKomut(15, "", @"CREATE TABLE BHFDEPREMSOZLUK
                                    (KOD VARCHAR(20)
                                    , AD VARCHAR(200)
                                    , PRIMARY KEY (KOD)
                                    )", "")
            , new SQLKomut(16, "", @"CREATE TABLE BHFDEPREMHARCAMAANA
                                    (YIL INT
                                    , AY INT
                                    , MUHASEBEKOD INT
                                    , BIRIMKOD VARCHAR(3)
                                    , DURUM INT
                                    , KAYDEDEN VARCHAR(200)
                                    , KAYITTARIHI DATETIME
                                    , ONAYLAYAN VARCHAR(200)
                                    , ONAYTARIHI DATETIME
                                    , PRIMARY KEY (YIL, AY, MUHASEBEKOD, BIRIMKOD)
                                    )", "")
            , new SQLKomut(17, "", @"CREATE TABLE BHFDEPREMHARCAMADETAY
                                        (YIL INT
                                        , AY INT
                                        , MUHASEBEKOD INT
                                        , BIRIMKOD VARCHAR(3)
                                        , ILKOD INT
                                        , EKOKOD VARCHAR(20)
                                        , BUTCEICI MONEY
                                        , BUTCEICIACIKLAMA VARCHAR(50)
                                        , HT1 MONEY
                                        , HT2 MONEY
                                        , HT3 MONEY
                                        , HT4 MONEY
                                        , HT5 MONEY
                                        , BUTCEDISIACIKLAMA VARCHAR(50)
                                        , PRIMARY KEY (YIL, AY, MUHASEBEKOD, BIRIMKOD, ILKOD, EKOKOD)
                                    )", "")
            , new SQLKomut(18,"",@"CREATE TABLE BHFARACKIRALAMAPLANANA(YIL INT
                                    , MUHASEBEKOD INT
                                    , BIRIMKOD VARCHAR(3)
                                    , DURUM INT
                                    , KAYDEDEN VARCHAR(200)
                                    , KAYITTARIHI DATETIME
                                    , ONAYLAYAN VARCHAR(200)
                                    , ONAYTARIHI DATETIME
                                    , PRIMARY KEY (YIL, MUHASEBEKOD, BIRIMKOD))","")
            , new SQLKomut(19,"",@"CREATE TABLE BHFARACKIRALAMAPLANDETAY(YIL INT
                                    , MUHASEBEKOD INT
                                    , BIRIMKOD VARCHAR(3)
                                    , SIRA INT
                                    , TASITTIPI INT
                                    , TASITKODU VARCHAR(10)
                                    , BUTCEKAYNAGI INT
                                    , BUYILKIRALANAN INT
                                    , BUYILKIRALANANSURE INT
                                    , GELECEKYILKIRALANACAK INT
                                    , GELECEKYILKIRALANACAKSURE INT
                                    , PRIMARY KEY (YIL, MUHASEBEKOD, BIRIMKOD, SIRA))","")
            , new SQLKomut(20,"",@"alter table BHFARACKIRALAMAPLANDETAY
                                    add BUYILFIYAT MONEY,
                                    GELECEKYILFIYAT MONEY,
                                    ONCYILKIRALANAN INT,
                                    ONCYILKIRALANANSURE INT, 
                                    ONCYILFIYAT MONEY
                                    ","")
            , new SQLKomut(21,"",@"alter table PRGKODYAPI add FONKOD varchar(20)","")
        };

        private static SQLKomut[] komutlar = {
            new SQLKomut(1,"",@"--DROP TABLE [dbo].[BHMBIRIMONAY] --TCCB için eklendi. Diğer kurumlar için drop edilmesin diye kapatıldı.
                                    CREATE TABLE [dbo].[BHMBIRIMONAY](
	                                    [YIL] [int] NOT NULL,
	                                    [SENARYOKOD] [varchar](20) NOT NULL,
	                                    [MUHASEBEKOD] [int] NOT NULL,
	                                    [BIRIMKOD] [varchar](20) NOT NULL,
	                                    [ONAYTUR] [int] NOT NULL,
	                                    [ONAYKOD] [int] NULL,
                                    PRIMARY KEY CLUSTERED 
                                    (
	                                    [YIL] ASC,
	                                    [SENARYOKOD] ASC,
	                                    [MUHASEBEKOD] ASC,
	                                    [BIRIMKOD] ASC,
	                                    [ONAYTUR] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]","BHMBIRIMONAY DROP AND CREATE")

            , new SQLKomut(2,"",@"--DROP TABLE [dbo].[BHMBIRIMONAYTARIHCE] --TCCB için eklendi. Diğer kurumlar için drop edilmesin diye kapatıldı.
                                    CREATE TABLE [dbo].[BHMBIRIMONAYTARIHCE](
	                                    [YIL] [int] NOT NULL,
	                                    [SENARYOKOD] [varchar](20) NOT NULL,
	                                    [MUHASEBEKOD] [int] NOT NULL,
	                                    [BIRIMKOD] [varchar](20) NOT NULL,
	                                    [ONAYTUR] [int] NOT NULL,
	                                    [ISLEMTARIH] [datetime] NOT NULL,
	                                    [ONAYKOD] [int] NULL,
	                                    [ISLEMIYAPAN] [varchar](100) NULL,
                                    PRIMARY KEY CLUSTERED 
                                    (
	                                    [YIL] ASC,
	                                    [SENARYOKOD] ASC,
	                                    [MUHASEBEKOD] ASC,
	                                    [BIRIMKOD] ASC,
	                                    [ONAYTUR] ASC,
	                                    [ISLEMTARIH] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]","BHMBIRIMONAYTARIHCE DROP AND CREATE")

            , new SQLKomut(3, "", @"ALTER TABLE BHMBUTCEFISIGEREKCE ADD PEBKOD varchar(20) NOT NULL DEFAULT ''", "")

            , new SQLKomut(4, "", @"declare @const nvarchar(200)
                                    set @const = QUOTENAME((
                                    select kc.name from sys.key_constraints as kc
                                    where kc.parent_object_id = object_id('BHMBUTCEFISIGEREKCE') and kc.type = 'PK'))
                                    exec('ALTER TABLE BHMBUTCEFISIGEREKCE DROP CONSTRAINT ' + @const)", "")

            , new SQLKomut(5, "", @"ALTER TABLE BHMBUTCEFISIGEREKCE ADD CONSTRAINT PK_BHMBUTCEFISIGEREKCE PRIMARY KEY CLUSTERED (
                                  YIL, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, PARCA) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]", "")

            , new SQLKomut(6, "", @"ALTER TABLE BHMBUTCEFISIMALZEME ADD PEBKOD varchar(20) NOT NULL DEFAULT ''", "")

            , new SQLKomut(7, "", @"declare @const nvarchar(200)
                                    set @const = QUOTENAME((
                                    select kc.name from sys.key_constraints as kc
                                    where kc.parent_object_id = object_id('BHMBUTCEFISIMALZEME') and kc.type = 'PK'))
                                    exec('ALTER TABLE BHMBUTCEFISIMALZEME DROP CONSTRAINT ' + @const)", "")

            , new SQLKomut(8, "", @"ALTER TABLE BHMBUTCEFISIMALZEME ADD CONSTRAINT PK_BHMBUTCEFISIMALZEME PRIMARY KEY CLUSTERED (
                                  YIL, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, HESAPKOD) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]", "")

            , new SQLKomut(9, "", @"CREATE VIEW PRGODENEK AS SELECT O.YIL YIL
                                      ,O.SENARYOKOD SENARYOKOD
                                      ,O.TEKLIFYIL TEKLIFYIL
	                                  ,Y.TUR PTKOD
	                                  ,I.PRGKOD PRGKOD
	                                  ,I.ALTPRGKOD APRGKOD
	                                  ,I.FAALKOD FAALKOD
	                                  ,I.ALTFAALKOD AFAALKOD
                                      ,O.KURKOD KURKOD
                                      ,O.FINKOD FINKOD
                                      ,O.EKOKOD EKOKOD
                                      ,O.PEBKOD PEBKOD
                                      ,O.FONKOD FONKOD
                                      ,O.ODENEK ODENEK
                                  FROM BHMODENEK O
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGPEBILISKI I ON
                                  O.YIL=I.YIL AND
                                  O.PEBKOD =I.PEBKOD
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGKODYAPI Y ON
                                  Y.YIL =I.YIL AND
                                  Y.KODTUR = 1 AND
                                  Y.KOD = I.PRGKOD", "")

            , new SQLKomut(10, "", @"CREATE VIEW PRGHARCAMATAHMIN AS SELECT O.YIL YIL
                                      ,O.SENARYOKOD SENARYOKOD
                                      ,O.TEKLIFYIL TEKLIFYIL
	                                  ,Y.TUR PTKOD
	                                  ,I.PRGKOD PRGKOD
	                                  ,I.ALTPRGKOD APRGKOD
	                                  ,I.FAALKOD FAALKOD
	                                  ,I.ALTFAALKOD AFAALKOD
                                      ,O.KURKOD KURKOD
                                      ,O.FINKOD FINKOD
                                      ,O.EKOKOD EKOKOD
                                      ,O.PEBKOD PEBKOD
                                      ,O.FONKOD FONKOD
                                      ,O.ODENEK ODENEK
                                  FROM BHMTAHMINGIDER O
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGPEBILISKI I ON
                                  O.YIL=I.YIL AND
                                  O.PEBKOD =I.PEBKOD
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGKODYAPI Y ON
                                  Y.YIL =I.YIL AND
                                  Y.KODTUR = 1 AND
                                  Y.KOD = I.PRGKOD", "")

            , new SQLKomut(11, "", @"CREATE VIEW PRGTAVANUSTU AS SELECT O.YIL YIL
                                      ,O.SENARYOKOD SENARYOKOD, Y.TUR PTKOD, I.PRGKOD PRGKOD, I.ALTPRGKOD APRGKOD
	                                  ,I.FAALKOD FAALKOD, I.ALTFAALKOD AFAALKOD, O.KURKOD KURKOD, O.FINKOD FINKOD
                                      ,O.EKOKOD EKOKOD, O.PEBKOD PEBKOD, O.FONKOD FONKOD
                                      ,O.ODENEK ODENEK, O.ODENEK1 ODENEK1, O.ODENEK2 ODENEK2
                                  FROM BHMTAVANUSTU O
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGPEBILISKI I ON
                                  O.YIL=I.YIL AND
                                  O.PEBKOD =I.PEBKOD
                                  LEFT OUTER JOIN ButceHazirlik.dbo.PRGKODYAPI Y ON
                                  Y.YIL =I.YIL AND
                                  Y.KODTUR = 1 AND
                                  Y.KOD = I.PRGKOD", "")

            , new SQLKomut(12,"",@"CREATE TABLE [dbo].[BHMPROJE](
	                [KOD] [varchar](40) NOT NULL,
	                [ORTAKLAR] [varchar](500) NULL,
	                [AD] [varchar](250) NULL,
	                [UYGULANACAKYER] [varchar](500) NULL,
	                [BASTARIH] [datetime] NULL,
	                [BITTARIH] [datetime] NULL,
	                [SORUNLAR] [varchar](500) NULL,
	                [NEDENLER] [varchar](500) NULL,
	                [AMAC] [varchar](500) NULL,
	                [TEDBIR] [varchar](500) NULL,
	                [HEDEFKITLE] [varchar](500) NULL,
	                [HEDEFKITLEETKI] [varchar](500) NULL,
	                [CEVRESELETKI] [varchar](500) NULL,
	                [CARPANETKI] [varchar](500) NULL,
	                [KATILIMSAHIPLIK] [varchar](500) NULL,
	                [PAYLASIM] [varchar](500) NULL,
	                [YAYGINLASMA] [varchar](500) NULL,
	                [DURUM] [int] NULL,
                    CONSTRAINT [PK_BHMPROJE] PRIMARY KEY CLUSTERED([KOD] ASC)) ON [PRIMARY]","")

            , new SQLKomut(13,"",@"CREATE TABLE [dbo].[BHMPROJEBIRIM](
	                            [PROJEKOD] [varchar](40) NOT NULL,
	                            [BIRIMKOD] [varchar](50) NOT NULL,
	                            [BIRIMAD] [varchar](250) NULL,
                                CONSTRAINT [PK_BHMPROJEBIRIM] PRIMARY KEY CLUSTERED ([PROJEKOD] ASC, [BIRIMKOD] ASC))","")

            , new SQLKomut(14,"",@"CREATE TABLE [dbo].[BHMPROJEYURUTUCU](
	            [PROJEKOD] [varchar](40) NOT NULL,
	            [TCKIMLIKNO] [varchar](11) NOT NULL,
	            [ADSOYAD] [varchar](50) NULL,
	            [KURUM] [varchar](100) NULL,
	            [UNVAN] [varchar](50) NULL,
	            [TEL] [varchar](50) NULL,
	            [EPOSTA] [varchar](50) NULL,
	            [PROJEDEKIROLU] [varchar](100) NULL,
                CONSTRAINT [PK_BHMPROJEYURUTUCU] PRIMARY KEY CLUSTERED 
                (
	                [PROJEKOD] ASC,
	                [TCKIMLIKNO] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY]","")

            , new SQLKomut(15,"",@"CREATE TABLE [dbo].[BHMPROJEFAALIYET](
	            [PROJEKOD] [varchar](40) NOT NULL,
	            [KOD] [varchar](40) NOT NULL,
	            [AD] [varchar](250) NULL,
	            [ANAKOD] [varchar](40) NULL,
                CONSTRAINT [PK_BHMPROJEFAALIYET] PRIMARY KEY CLUSTERED  ([PROJEKOD] ASC, [KOD] ASC))","")

            , new SQLKomut(16,"",@"
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEFAALIYET]') AND type in (N'U'))
                    DROP TABLE [dbo].[BHMPROJEFAALIYET]
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEYURUTUCU]') AND type in (N'U'))
                    DROP TABLE [dbo].[BHMPROJEYURUTUCU]
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEBIRIM]') AND type in (N'U'))
                    DROP TABLE [dbo].[BHMPROJEBIRIM]
                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJE]') AND type in (N'U'))
                    DROP TABLE [dbo].[BHMPROJE]","BHMPROJE tablo yapıları değiştirildi.")

            , new SQLKomut(17,"",@"CREATE TABLE [dbo].[BHMPROJE](
	                    [KOD] [varchar](40) NOT NULL,
	                    [SORUMLUBIRIMKOD] [varchar](3) NULL,
	                    [SORUMLUMUHASEBEKOD] [int] NULL,
	                    [SORUMLUKURKOD] [varchar](20) NULL,
	                    [AD] [varchar](250) NULL,
	                    [SEKTORKOD] [varchar](40) NULL,
	                    [BASLAMAYIL] [int] NULL,
	                    [BITISYIL] [int] NULL,
	                    [UYGULAMAYERI] [varchar](500) NULL,
	                    [ISBIRLIGIYAPANKURUM] [varchar](500) NULL,
	                    [ILISKILIYATIRIMPROJENO] [varchar](40) NULL,
	                    [ILISKILIYATIRIMPROJEAD] [varchar](250) NULL,
	                    [GENELHEDEF] [varchar](500) NULL,
	                    [AMAC] [varchar](500) NULL,
	                    [TEDBIR] [varchar](500) NULL,
	                    [ONCELIKLERI] [varchar](500) NULL,
	                    [BEKLENENSONUC] [varchar](500) NULL,
	                    [PROJESONUC] [varchar](500) NULL,
	                    [DURUM] [int] NULL,
	                    [KPAMACHEDEF] [varchar](500) NULL,
	                    [KPAMACHEDEFPOLITIKA] [varchar](500) NULL,
	                    [KPTEDBIR] [varchar](500) NULL,
	                    [SPAMAC] [varchar](500) NULL,
	                    [SPHEDEF] [varchar](500) NULL,
	                    [PERHEDEF] [varchar](500) NULL,
	                    [PERGOSTERGE] [varchar](500) NULL,
	                    [PROJENO] [varchar](50) NULL,
                     CONSTRAINT [PK_BHMPROJE] PRIMARY KEY CLUSTERED([KOD] ASC))","")

            , new SQLKomut(24,"",@"
                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEYURUTUCU]') AND type in (N'U'))
                        DROP TABLE [dbo].[BHMPROJEYURUTUCU]
                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEBIRIM]') AND type in (N'U'))
                        DROP TABLE [dbo].[BHMPROJEBIRIM]
                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEBIRIMILGILI]') AND type in (N'U'))
                        DROP TABLE [dbo].[BHMPROJEBIRIMILGILI]
                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEFINANSMAN]') AND type in (N'U'))
                        DROP TABLE [dbo].[BHMPROJEFINANSMAN]
                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJEGIRDI]') AND type in (N'U'))
                        DROP TABLE [dbo].[BHMPROJEGIRDI]
                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BHMPROJESORUN]') AND type in (N'U'))
                        DROP TABLE [dbo].[BHMPROJESORUN]","BHMPROJE tablo yapıları değiştirildi.")

            , new SQLKomut(25,"",@"CREATE TABLE [dbo].[BHMPROJEFINANSMAN](
	                    [KOD] [varchar](40) NOT NULL,
	                    [PROJEKOD] [varchar](40) NOT NULL,
	                    [YIL] [int] NULL,
	                    [AD] [varchar](50) NULL,
	                    [KAYNAK] [int] NULL,
	                    [TUTAR] [decimal](18, 2) NULL,
                        CONSTRAINT [PK_BHMPROJEFINANSMAN] PRIMARY KEY CLUSTERED ([KOD] ASC))","")

            , new SQLKomut(26,"",@"CREATE TABLE [dbo].[BHMPROJEGIRDI](
	                    [KOD] [varchar](40) NOT NULL,
	                    [PROJEKOD] [varchar](40) NOT NULL,
	                    [TUR] [int] NULL,
	                    [GIRDI] [varchar](250) NULL,
	                    [BIRIM] [varchar](250) NULL,
	                    [MIKTAR] [money] NULL,
	                    [BIRIMMALIYET] [decimal](18, 2) NULL,
	                    [TOPLAMMALIYET] [decimal](18, 2) NULL,
                        CONSTRAINT [PK_BHMPROJEGIRDI] PRIMARY KEY CLUSTERED([KOD] ASC))","")

            , new SQLKomut(27,"",@"CREATE TABLE [dbo].[BHMPROJESORUN](
	                    [KOD] [varchar](40) NOT NULL,
	                    [PROJEKOD] [varchar](40) NOT NULL,
	                    [TARIH] [datetime] NULL,
	                    [SORUN] [varchar](500) NULL,
	                    [COZUMONERISI] [varchar](5000) NULL,
                        CONSTRAINT [PK_BHMPROJESORUN] PRIMARY KEY CLUSTERED ([KOD] ASC))","")

            , new SQLKomut(28,"",@"CREATE TABLE [dbo].[BHMPROJEURUN](
	                    [PROJEKOD] [varchar](40) NOT NULL,
	                    [URUNKOD] [varchar](40) NOT NULL,
                        CONSTRAINT [PK_BHMPROJEURUN] PRIMARY KEY CLUSTERED ([PROJEKOD] ASC,  [URUNKOD] ASC))","")

            , new SQLKomut(29,"",@"CREATE TABLE [dbo].[BHMFINANSMANKAYNAK](
	                              [KOD] INT NOT NULL,
	                              [AD] [varchar](250) NULL,
                                  CONSTRAINT [PK_BHMFINANSMANKAYNAK] PRIMARY KEY CLUSTERED([KOD] ASC)) 

                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (1, 'GENEL BÜTÇE')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (2, 'KALKINMA İDARESİ')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (3, 'TÜBİTAK')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (4, 'KALKINMA AJANSI')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (5, 'BELEDİYE')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (6, 'ÜNİVERSİTE')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (7, 'DİĞER KAMU')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (8, 'ÖZEL SEKTÖR')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (9, 'İL ÖZEL İDARESİ')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (10, 'YİKOP')
                            INSERT [dbo].[BHMFINANSMANKAYNAK] ([KOD], [AD]) VALUES (11, 'SYDV')","")

            , new SQLKomut(30,"",@"CREATE TABLE [dbo].[BHMURUNGRUP](
	                            [KOD] [varchar](40) NOT NULL,
	                            [AD] [varchar](250) NULL,
                                CONSTRAINT [PK_BHMURUNGRUP] PRIMARY KEY CLUSTERED([KOD] ASC)) 
                                INSERT [dbo].[BHMURUNGRUP] ([KOD], [AD]) VALUES (N'1', N'Meyveler Turunçgiller')
                                INSERT [dbo].[BHMURUNGRUP] ([KOD], [AD]) VALUES (N'2', N'Meyveler Sert Kabuklular')","")

            , new SQLKomut(31,"",@"CREATE TABLE [dbo].[BHMURUN](
	                            [KOD] [varchar](40) NOT NULL,
	                            [AD] [varchar](250) NULL,
	                            [GRUPKOD] [varchar](40) NULL,
                                CONSTRAINT [PK_BHMURUN] PRIMARY KEY CLUSTERED ([KOD] ASC)) 
                                INSERT [dbo].[BHMURUN] ([KOD], [AD], [GRUPKOD]) VALUES (N'1', N'Portakal', N'1')
                                INSERT [dbo].[BHMURUN] ([KOD], [AD], [GRUPKOD]) VALUES (N'2', N'Limon', N'1')
                                INSERT [dbo].[BHMURUN] ([KOD], [AD], [GRUPKOD]) VALUES (N'3', N'Mandalina', N'1')
                                INSERT [dbo].[BHMURUN] ([KOD], [AD], [GRUPKOD]) VALUES (N'4', N'Antep Fıstığı', N'2')
                                INSERT [dbo].[BHMURUN] ([KOD], [AD], [GRUPKOD]) VALUES (N'5', N'Badem', N'2')
                                INSERT [dbo].[BHMURUN] ([KOD], [AD], [GRUPKOD]) VALUES (N'6', N'Ceviz', N'2')","")

            , new SQLKomut(32,"",@"CREATE TABLE [dbo].[BHMFAALIYET](
	                            [KOD] [varchar](40) NOT NULL,
	                            [AD] [varchar](250) NULL,
                                CONSTRAINT [PK_BHMFAALIYET] PRIMARY KEY CLUSTERED([KOD] ASC))
                                INSERT [dbo].[BHMFAALIYET] ([KOD], [AD]) VALUES (N'1', N'Çilek Tarlası Kurulumu')","")

            , new SQLKomut(33,"",@"CREATE TABLE [dbo].[BHMPROJEFAALIYET](
	                            [KOD] [varchar](40) NOT NULL,
	                            [PROJEKOD] [varchar](40) NULL,
	                            [AD] [varchar](250) NULL,
	                            [YIL] [int] NULL,
	                            [BASTARIH] [datetime] NULL,
	                            [BITTARIH] [datetime] NULL,
	                            [ANAFAALIYET] [varchar](40) NULL,
	                            [SORUMLUBIRIMKOD] [varchar](3) NULL,
	                            [SORUMLUMUHASEBEKOD] [int] NULL,
	                            [SORUMLUKURKOD] [varchar](20) NULL,
	                            [IZLEYENKOD] [varchar](50) NULL,
	                            [IZLEYENAD] [varchar](50) NULL,
                                CONSTRAINT [PK_BHMPROJEFAALIYET_1] PRIMARY KEY CLUSTERED([KOD]))","")

            , new SQLKomut(34,"",@"CREATE TABLE [dbo].[BHMPROJEFAALIYETGOSTERGE](
	                              [KOD] [varchar](40) NOT NULL,
	                              [FAALIYETKOD] [varchar](40) NULL,
	                              [YIL] [int] NULL,
	                              [GOSTERGE] [varchar](50) NULL,
	                              [BIRIM] [varchar](50) NULL,
	                              [PLANLANANDEGER] [varchar](50) NULL,
	                              [GERCEKLESENDEGER] [varchar](50) NULL,
	                              [DOGRULAMAKAYNAK] [varchar](50) NULL,
	                              [GERCEKLESMETARIH] [datetime] NULL,
                                  CONSTRAINT [PK_BHMPROJEFAALIYETGOSTERGE_1] PRIMARY KEY CLUSTERED([KOD] ASC))","")

            , new SQLKomut(35,"",@"CREATE TABLE [dbo].[BHMPROJEACIKLAMATUR](
	                            [KOD] [varchar](40) NOT NULL,
	                            [AD] [varchar](100) NULL,
	                            [TUR] [int] NULL,
	                            [SIRANO] [int] NULL,
                                CONSTRAINT [PK_BHMPROJEACIKLAMATUR] PRIMARY KEY CLUSTERED([KOD]))

                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'01', N'Genel Hedef', 2, 1)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'02', N'Amaç', 2, 2)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'03', N'Bekelen Sonuç', 2, 3)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'04', N'Tamamlanan Proje Sonucu', 2, 4)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'05', N'Kalkınma Planı Amaç ve Hedefi', 2, 5)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'06', N'Kalkınma Planı Amaç ve Hedefi Politikaları', 2, 6)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'07', N'Kalkınma Planı Tedbir', 2, 7)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'08', N'Stratejik Plan Amaç', 2, 8)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'09', N'Stratejik Plan Hedef', 2, 9)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'10', N'Performans Hedefi ', 2, 10)
                            INSERT [dbo].[BHMPROJEACIKLAMATUR] ([KOD], [AD], [TUR], [SIRANO]) VALUES (N'11', N'Performans Göstergesi ', 2, 11)","")

            , new SQLKomut(36,"",@"CREATE TABLE [dbo].[BHMPROJEACIKLAMA](
	                            [PROJEKOD] [varchar](40) NOT NULL,
	                            [TUR] [varchar](40) NOT NULL,
	                            [ACIKLAMA] [varchar](max) NULL,
                                CONSTRAINT [PK_BHMPROJEACIKLAMA] PRIMARY KEY CLUSTERED ([PROJEKOD] ASC, [TUR] ASC))","")

            , new SQLKomut(37,"",@"INSERT BHMFINANSMANKAYNAK (KOD, AD) VALUES ('12', 'DIŞ KAYNAK')","")

            , new SQLKomut(38,"",@"ALTER TABLE BHMPROJE ADD YATPROJEKOD INT NULL, YATALTPROJEKOD INT NULL","")

            , new SQLKomut(39,"",@"ALTER TABLE BHMPROJE ADD YATPROJEYIL INT NULL","")

            , new SQLKomut(40,"",@"ALTER TABLE BHMPROJE ADD SPHEDEFKOD VARCHAR(20)","")

            , new SQLKomut(41,"",@"ALTER TABLE BHMPROJEFAALIYET ADD TAHMINIMALIYET MONEY","")

            , new SQLKomut(42,"",@"ALTER TABLE BHMPROJEFAALIYETGOSTERGE ALTER COLUMN PLANLANANDEGER money","")

            , new SQLKomut(43,"",@"CREATE TABLE BHMPROJEFAALIYETGERC(
	                            FAALKOD varchar(40) NOT NULL, YIL int NOT NULL, AY int NOT NULL, 
                                NAKDIGERCEKLESME MONEY, KAYDEDENKULKOD VARCHAR(40), KAYITTARIHI DATETIME,
                                CONSTRAINT [PK_BHMPROJEFAALIYETGERC] PRIMARY KEY CLUSTERED(FAALKOD, YIL, AY))","")

            , new SQLKomut(44,"",@"CREATE TABLE BHMPROJEFAALIYETGOSTERGEGERC(
	                            GOSKOD varchar(40) NOT NULL, YIL int NOT NULL, AY int NOT NULL, 
                                GERCEKLESME MONEY,
                                CONSTRAINT [PK_BHMPROJEFAALIYETGOSTERGEGERC] PRIMARY KEY CLUSTERED(GOSKOD, YIL, AY))","")

            , new SQLKomut(45,"",@"ALTER TABLE BHMPROJEFAALIYETGERC ADD ACIKLAMA VARCHAR(2000)","")
            , new SQLKomut(46,"",@"CREATE TABLE BHMV2IOIPROJE (SIRANO INT PRIMARY KEY IDENTITY(1,1), MUHASEBEKOD INT, BIRIMKOD VARCHAR(3), ALTPROJENO INT, IOIPROJEADI VARCHAR(250), BASLAYIL INT, BITISYIL INT, PROJETUTARI MONEY, ACIKLAMA VARCHAR(2500))","")
            , new SQLKomut(47,"",@"CREATE TABLE BHMV2IOIPROJEAKTARMA(AKTARMANO INT PRIMARY KEY IDENTITY(1,1), SIRANO INT, AKTARMATARIHI DATE, AKTARILANTUTAR MONEY, ACIKLAMA VARCHAR(2500))","")
            , new SQLKomut(48,"",@"CREATE TABLE BHMV2IOIPROJEHARCAMA(HARCAMANO INT PRIMARY KEY IDENTITY(1,1), SIRANO INT, HARCAMATARIHI DATE, HARCAMATUTARI MONEY, ACIKLAMA VARCHAR(2500))","")
            , new SQLKomut(49,"",@"ALTER TABLE BHMV2IOIPROJE ADD ILKOD INT","")
            , new SQLKomut(50,"",@"ALTER TABLE BHMV2IOIPROJEAKTARMA ADD TUR INT NOT NULL DEFAULT 1","")
            , new SQLKomut(51,"",@"UPDATE BHMV2IOIPROJE SET ILKOD =  CASE when  [MUHASEBEKOD]<1000 then 6 else MUHASEBEKOD/1000 END WHERE ILKOD IS NULL","")
            , new SQLKomut(52,"",@"ALTER TABLE BHMV2IOIPROJE ADD ISTUR INT NOT NULL DEFAULT 1","")
            , new SQLKomut(53,"",@"CREATE TABLE BHMV2IOIPROJEPEB (SIRANO INT NOT NULL, YIL INT NOT NULL, PEBKOD VARCHAR(20) NOT NULL, PRIMARY KEY (SIRANO, YIL, PEBKOD))","")
            , new SQLKomut(54, "", @"CREATE TABLE BHMV2MPDAGITIM
                (ID INT PRIMARY KEY IDENTITY(1,1), YIL INT NOT NULL, KUR VARCHAR(100)
                , FON VARCHAR(100), FIN VARCHAR(100), EKO VARCHAR(100), PEB VARCHAR(100)
                , ACIKLAMA VARCHAR(1000) NOT NULL, MIKTAR MONEY NOT NULL)", "")
            , new SQLKomut(55, "", @"CREATE TABLE BHMV2MPDAGITIMDETAY(ID INT NOT NULL
                , BIRIMKOD VARCHAR(100) NOT NULL, MIKTAR MONEY NOT NULL
                , BUYILDEGER MONEY NOT NULL, ONCYILDEGER MONEY NOT NULL
                , PRIMARY KEY (ID, BIRIMKOD))", "")
            ,new SQLKomut(56, "", @"ALTER TABLE BHMV2MPDAGITIMDETAY
                ADD CONSTRAINT FK_BHMV2MPDAGITIMDETAY_BHMV2MPDAGITIM FOREIGN KEY (ID) REFERENCES BHMV2MPDAGITIM(ID)", "")
        };
    }
}
