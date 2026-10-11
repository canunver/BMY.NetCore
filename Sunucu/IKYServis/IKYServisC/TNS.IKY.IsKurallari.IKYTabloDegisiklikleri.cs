using System;
using System.Data;
using TNS.IKY;
using TNS.IKY.IsKurallari.Veri;
using TNS.KYM;
using TNS.Veri;
using TNS.UZY;
using System.Resources;

namespace TNS.IKY.IsKurallari
{
    public static class IKYTabloDegisiklikleri
    {
        public static void Kaydet()
        {
            TabloDegisiklikleri.Kaydet("BaglantiSatiriIKY", komutlar);
        }

        private static SQLKomut[] komutlar ={

         new SQLKomut(1,"",@"ALTER TABLE IKYPERSONELAILE ADD SAKATLIKDERECESI INT NOT NULL DEFAULT 0","")
        ,new SQLKomut(2,"",@"ALTER TABLE IKYPERSONELTIPI ADD MAASDONEMGUN INT NOT NULL DEFAULT 0, MAASODEMEGUN INT NOT NULL DEFAULT 0","")
        ,new SQLKomut(3,"",@"CREATE TABLE IKYPERSONELGIDER(
                       PKOD INT NOT NULL,
                       YERLESKEODAKOD INT NOT NULL,
                       HARCAMATUR INT NOT NULL,
                       TARIH DATETIME NOT NULL,
                       TUTAR MONEY NOT NULL,
                       MAILGONDERILDI INT NOT NULL,
                       DURUM INT NOT NULL,
                       DEGISIKLIGIYAPAN VARCHAR(50) NOT NULL,
                       DEGISIKLIKZAMANI DATETIME NOT NULL,
                       PRIMARY KEY(PKOD, YERLESKEODAKOD, HARCAMATUR, TARIH))","")

        ,new SQLKomut(4,"","ALTER TABLE IKYPERSONELILETISIM ADD EPOSTA2 VARCHAR(50)","")
        ,new SQLKomut(5,"","ALTER TABLE IKYPERSONELKADRO ADD TAYINOLACAGIBIRIMKOD INT","")
        ,new SQLKomut(6,"",@"CREATE TABLE IKYGOREVLER (
                                        PKOD INT
                                        , GOREVNO INT
                                        , UNVANKOD INT
                                        , GOREVDURUMU INT
                                        , BIRIMI INT
                                        , BASLAMATARIHI DATETIME
                                        , BITISTARIHI DATETIME
                                        , PRIMARY KEY (PKOD, GOREVNO))","")

        ,new SQLKomut(7,"",@"ALTER TABLE IKYPERSONELIZIN ADD RAPORTARIHI DATETIME, RAPORNO VARCHAR(50), YATILIGECENSURE INT","")
        ,new SQLKomut(8,"",@"ALTER TABLE IKYPERSONELIZIN ADD YERKOD VARCHAR(20)","")
        ,new SQLKomut(9,"",@"ALTER TABLE IKYGOREVLER ADD ACIKLAMA VARCHAR(60), ONAYNO VARCHAR(50), ONAYTARIHI DATETIME","")
        ,new SQLKomut(10,"",@"ALTER TABLE dbo.IKYPERSONELNUFUS ADD PASAPORTNO varchar(50) NULL,
                                         PASAPORTVERILISTARIHI datetime NULL,
                                         PASAPORTVERILDIGIYER varchar(250) NULL,
                                         PASAPORTVERENMAKAM varchar(250) NULL,
                                         PASAPORTSURESI int NULL","")
        ,new SQLKomut(11,"",@"ALTER TABLE dbo.IKYPERSONELIZIN ADD IZINNOTU2 varchar(6000) NULL","")
        ,new SQLKomut(12,"",@"CREATE TABLE [dbo].[IKYPERSONELSOZLESME](
	                                [KOD] [int] NOT NULL,
	                                [YIL] [int] NULL,
	                                [TCKIMLIKNO] [varchar](50) NULL,
	                                [ADSOYAD] [varchar](100) NULL,
	                                [BASLANGICTARIHI] [datetime] NULL,
	                                [BITISTARIHI] [datetime] NULL,
	                                [TUTAR] [money] NULL,
	                                [SOZLESMEKOD] [int] NULL,
                                     CONSTRAINT [PK_IKYPERSONELSOZLESME] PRIMARY KEY CLUSTERED 
                                    (
	                                    [KOD] ASC
                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                    ) ON [PRIMARY]
                                    ","")

        ,new SQLKomut(13,"",@"CREATE FUNCTION [dbo].[DonemlerKesisiyorMu](@donem1Bas Date, @donem1Son date, @donem2Bas Date, @donem2Son date) RETURNS int AS
                                        BEGIN
	                                        DECLARE @sonuc int
	                                        declare @donem1SonGun Date
	                                        declare @donem2SonGun Date
	                                        select @sonuc = 0

	                                        select @sonuc = dbo.GunTarihAraligindaMi(@donem1Bas, @donem2Bas, @donem2Son)
	                                        if(@sonuc=1) return 1

	                                        if(@donem1Son is not null)
		                                        select @donem1SonGun = DATEADD(day, -1, @donem1Son)
	                                        else
		                                        select @donem1SonGun = null
	                                        select @sonuc = dbo.GunTarihAraligindaMi(@donem1SonGun, @donem2Bas, @donem2Son)
	                                        if(@sonuc=1) return 1

	                                        select @sonuc = dbo.GunTarihAraligindaMi(@donem2Bas, @donem1Bas, @donem1Son)
	                                        if(@sonuc=1) return 1

	                                        if(@donem2Son is not null)
		                                        select @donem2SonGun = DATEADD(day, -1, @donem2Son)
	                                        else
		                                        select @donem2SonGun = null
	                                        select @sonuc = dbo.GunTarihAraligindaMi(@donem2SonGun, @donem1Bas, @donem1Son)
	                                        if(@sonuc=1) return 1

	                                         return 0
                                        END
                                    ","DonemlerKesisiyorMu fonksiyonunun eklenmesi")

                ,new SQLKomut(14,"",@"CREATE FUNCTION [dbo].[gunDondur](@pKucuk date, @pBuyuk date, @hesapCarpani int, @sonTarih date, @gun int) RETURNS int
                                    AS
                                    BEGIN
                                        declare @y1 int
                                        declare @y2 int
                                        declare @a1 int
                                        declare @a2 int
                                        declare @g1 int
                                        declare @g2 int

                                        if(@gun is null) set @gun = 0;
                                        if(@pKucuk is null) return 0;
                                        if(@pBuyuk is null) set @pBuyuk = @sonTarih;
                                        if(@pBuyuk is null) set @pBuyuk = getdate();
                                        if(@pBuyuk <= @pKucuk) return 0;   

                                        if(@sonTarih is not null)
                                        begin
		                                    if(@pBuyuk > @sonTarih) 
		                                    begin
			                                    set @gun = 0
			                                    if(@pKucuk <= @sonTarih)
				                                    set @pBuyuk = @sonTarih;
			                                    else
				                                    return 0;
		                                    end
                                        end

	                                    if(@gun<=0)
	                                    begin
		                                    set @y1=DATEPART(YEAR, @pKucuk);
		                                    set @y2=DATEPART(YEAR, @pBuyuk);

		                                    set @a1=DATEPART(MONTH, @pKucuk);
		                                    set @a2=DATEPART(MONTH, @pBuyuk);

		                                    set @g1=DATEPART(DAY, @pKucuk);
		                                    set @g2=DATEPART(DAY, @pBuyuk);
   
		                                    set @g1=@g2-@g1;
		                                    if(@g1<0) 
		                                    Begin
		                                    set @g1=@g1+30
		                                    set @a2=@a2-1
		                                    End
  
		                                    set @a1=@a2-@a1;
		                                    if(@a1<0) 
		                                    Begin
		                                    set @a1=@a1+12
		                                    set @y2=@y2-1
		                                    End

		                                    set @y1=@y2-@y1

		                                    set @g1= @y1*360 + @a1*30 + @g1
	                                    end
	                                    else
		                                    set @g1 = @gun 

                                        return @g1 * @hesapCarpani;
                                    END
                                    ","gunDondur fonksiyonunun eklenmesi")

                 ,new SQLKomut(15,"",@"CREATE FUNCTION [dbo].[GunTarihAraligindaMi](@gun Date, @donemBas Date, @donemSon date) RETURNS INT AS
                                        BEGIN
	                                        if(@gun is null) return 0;
	                                        if(@donemBas is not null and @donemBas>@gun) return 0;
	                                        if(@donemSon is not null and @donemSon<=@gun) return 0;
	                                        return 1;
                                        END
                                    ","GunTarihAraligindaMi fonksiyonunun eklenmesi")

                 ,new SQLKomut(16,"",@"CREATE FUNCTION [dbo].[izinDondur](@sonTarih datetime) RETURNS int
                                     AS
                                     BEGIN
                                         return 0;
                                     END
                                    ","izinDondur fonksiyonunun eklenmesi")

                 ,new SQLKomut(17,"",@"CREATE FUNCTION [dbo].[terfiMiDondur] 
                                    (
	                                    @terfiAyi varchar(2), @istenenTarih varchar(8), @acilisTarihi varchar(8)
                                    )
                                    RETURNS int
                                    AS
                                    BEGIN
                                        if(@terfiAyi is null or @terfiAyi = 0)
                                        begin
	                                        if(@acilisTarihi is null)
		                                        return 0;
	                                        if(@acilisTarihi > @istenenTarih)
		                                        return 0;
	                                        else
		                                        return 1;
                                        end
                                        else
                                        begin
	                                        if(SUBSTRING (@istenenTarih, 5, 2) = Right('00' + CONVERT(VARCHAR, @terfiAyi), 2))
		                                        return 1;
	                                        else
		                                        return 0;
                                        end		 
	                                    return 0;
                                    END
                                    ","terfiMiDondur fonksiyonunun eklenmesi")

                 ,new SQLKomut(18,"",@"CREATE FUNCTION [dbo].[terfiTipiDondur] 
                                    (
	                                    @kTerfiAy varchar(2), @eTerfiAy varchar(2), @istenenAy varchar(2)
                                    )
                                    RETURNS int
                                    AS
                                    BEGIN
                                     if(Right('00' + CONVERT(VARCHAR, @kTerfiAy), 2) = @istenenAy and Right('00' + CONVERT(VARCHAR, @eTerfiAy), 2) = @istenenAy)
	                                    return 3;
                                     if(Right('00' + CONVERT(VARCHAR, @kTerfiAy), 2) = @istenenAy)
	                                    return 1;
                                     if(Right('00' + CONVERT(VARCHAR, @eTerfiAy), 2) = @istenenAy)
	                                    return 2;
		 
	                                    return 0;
                                    END", "terfiTipiDondur fonksiyonunun eklenmesi")
                ,new SQLKomut(19, "", @"ALTER TABLE IKYPERSONELKADRO ADD MESLEKKODU VARCHAR(40)", "Meslek kodu")
                ,new SQLKomut(20, "", @"ALTER TABLE IKYPERSONELDIL ADD KURUMFAYDALANIYOR INT", "Kurum faydalanıyor alanı eklendi")
                ,new SQLKomut(21, "", @"CREATE TABLE IKYSENDIKABILGI(SENDIKANO INT NOT NULL, SENDIKAAD VARCHAR(200) NOT NULL, ADRES VARCHAR(400) NOT NULL, IBAN VARCHAR(200) NOT NULL, HESAPNO VARCHAR(200) NOT NULL, TOPLUGORUSMEPUANI INT NOT NULL, PRIMARY KEY (SENDIKANO))", "")
                ,new SQLKomut(22, "", @"CREATE TABLE IKYPERSONELSENDIKA(PKOD INT NOT NULL, SENDIKANO INT NOT NULL, PRIMARY KEY (PKOD))", "")
                ,new SQLKomut(23, "", @"ALTER TABLE IKYPERSONELSENDIKA ADD KESINTIORANI DECIMAL(18,6)", "")
                ,new SQLKomut(24,"",@"CREATE TABLE IKYMALBILKIMLIK
                    (KOD INT
                    , SIRANO INT
                    , ADISOYADI VARCHAR(220)
                    , DOGUMTARIHI DATETIME
                    , DOGUMYERI VARCHAR(150)
                    , YAKINLIGI VARCHAR(140)
                    , TCKIMLIKNO VARCHAR(160)
                    , PRIMARY KEY (KOD, SIRANO)
                    )","")

                ,new SQLKomut(25,"",@"CREATE TABLE IKYMALBILTASINMAZ
                    (KOD INT
                    , SIRANO INT
                    , DEGERI MONEY
                    , TASINMAZCINSI VARCHAR(180)
                    , ADRES VARCHAR(320)
                    , HISSEMIKTARI MONEY
                    , EDINMETARIHI DATETIME
                    , MALIKTCKN VARCHAR(170)
                    , PRIMARY KEY (KOD, SIRANO) )","")

                ,new SQLKomut(26,"",@"CREATE TABLE IKYMALBILKOOPERATIF
                    (KOD INT
                    , SIRANO INT
                    , KOOPERATIFADIYERI VARCHAR(320)
                    , HISSEDEGERI MONEY
                    , UYELIKTARIHI DATETIME
                    , TCKIMLIKNO VARCHAR(200)
                    , PRIMARY KEY (KOD, SIRANO) )","")

                ,new SQLKomut(27,"",@"CREATE TABLE IKYMALBILTASIT
                    (KOD INT
                    , SIRANO INT
                    , PLAKANO VARCHAR(120)
                    , TASITINCINSI VARCHAR(160)
                    , TASITINMARKASI VARCHAR(160)
                    , EDINMEDEGERI MONEY
                    , MODELYILI INT
                    , EDINMETARIHI DATETIME
                    , TCKIMLIKNO VARCHAR(200)
                    , PRIMARY KEY (KOD, SIRANO) )","")

                ,new SQLKomut(28,"",@"CREATE TABLE IKYMALBILDIGERTASINIR
                    (KOD INT
                    , SIRANO INT
                    , TASINIRMALCINSI VARCHAR(300)
                    , EDINMEDEGERI MONEY
                    , MODELYILI INT
                    , EDINMETARIHI DATETIME
                    , TCKIMLIKNO VARCHAR(220)
                    , PRIMARY KEY (KOD, SIRANO) )","")

                ,new SQLKomut(29,"",@"CREATE TABLE IKYMALBILBANKAMENKUL
                    (KOD INT
                    , SIRANO INT
                    , NITELIK VARCHAR(320)
                    , MIKTARI MONEY
                    , CINSI VARCHAR(120)
                    , TCKIMLIKNO VARCHAR(220)
                    , PRIMARY KEY (KOD, SIRANO) )","")

                ,new SQLKomut(30,"",@"CREATE TABLE IKYMALBILALTINMUCEVHER
                    (KOD INT
                    , SIRANO INT
                    , CINSI VARCHAR(260)
                    , DEGERI MONEY
                    , MIKTARI MONEY
                    , TCKIMLIKNO VARCHAR(220)
                    , PRIMARY KEY (KOD, SIRANO) )","")

                ,new SQLKomut(31,"",@"CREATE TABLE IKYMALBILBORCALACAK
                    (KOD INT
                    , SIRANO INT
                    , BORCLUADI VARCHAR(260)
                    , ALACAKLIADI VARCHAR(260)
                    , TUTAR MONEY
                    , PRIMARY KEY (KOD, SIRANO) )","")

                ,new SQLKomut(32,"",@"CREATE TABLE IKYMALBILDIGERSERVET
                    (KOD INT
                    , SIRANO INT
                    , SERVETUNSURU VARCHAR(420)
                    , EDINMESEKLI VARCHAR(200)
                    , TCKIMLIKNO VARCHAR(220)
                    , PRIMARY KEY (KOD, SIRANO) )","")
        };
    }
}
