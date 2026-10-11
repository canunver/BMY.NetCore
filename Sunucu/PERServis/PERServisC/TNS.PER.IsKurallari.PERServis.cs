using System;
using System.Runtime;
using TNS.KYM;
using TNS.PER;
using TNS.UZY;
using TNS.ORG;

namespace TNS.PER.IsKurallari
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class PERServis : MarshalByRefObject, TNS.PER.IPERServis
    {
        public static IUZYServis servisUZY = null;

        public PERServis()
        {
            //TabloDegisimYap();
        }

        //static bool tabloGuncelle = false;
        //private static void TabloDegisimYap()
        //{
        //    if (tabloGuncelle == false)
        //    {
        //        tabloGuncelle = true;
        //        /*
        //                        TNS.Veri.VeriErisim ve = TNS.Veri.VeriErisim.VeriErisimBul(null, "BaglantiSatiriPER", TNS.KYM.Arac.KimlikBul(null));

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery("ALTER TABLE PERPERSONEL ADD AKTIFMI INT NOT NULL DEFAULT 1, KOD INT NOT NULL DEFAULT 0");//25.11.2017 Tuğçe pasif personel login olmasın diyor.

        //                        //ve.hata = "";
        //                        //ve.ExecuteNonquery("CREATE TABLE PERPERSONELMAASPARAM(TCKIMLIKNO VARCHAR(11), DEGISKEN VARCHAR(50), DEGER MONEY, PRIMARY KEY (TCKIMLIKNO, DEGISKEN))");//17.02.2015 Can

        //                        //ve.hata = "";
        //                        //ve.ExecuteNonquery("CREATE TABLE PERPERSONELMAASMATRAH(TCKIMLIKNO VARCHAR(11), YIL INT, SURECNO VARCHAR(50), MATRAHTUR INT, TARIH DATETIME, DEGER MONEY, PRIMARY KEY (TCKIMLIKNO, YIL, SURECNO, MATRAHTUR, TARIH))");//17.02.2015 Can

        //                        //ve.hata = "";
        //                        //ve.ExecuteNonquery("CREATE TABLE PERBORDROTUR(BORDROTURU int, BORDROTURADI varchar(50) NOT NULL, PRIMARY KEY (BORDROTURU))");//17.02.2015 Can

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"update [PERPERSONELKURS]
        //                                            set donem = 
        //                                            case when (DATEPART(mm,BASLAMATARIHI)>0 and DATEPART(mm,BASLAMATARIHI)<4) then 1
        //                                            when (DATEPART(mm,BASLAMATARIHI)>3 and DATEPART(mm,BASLAMATARIHI)<7) then 2
        //                                            when (DATEPART(mm,BASLAMATARIHI)>6 and DATEPART(mm,BASLAMATARIHI)<10) then 3
        //                                            when (DATEPART(mm,BASLAMATARIHI)>9 and DATEPART(mm,BASLAMATARIHI)<13) then 4
        //                                            else 0 end
        //                                            where ([BASLAMATARIHI] is not null)");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"update [PERPERSONELKURS]
        //                                            set donem = 
        //                                            case when (DATEPART(mm,BITIRMETARIHI)>0 and DATEPART(mm,BITIRMETARIHI)<4) then 1
        //                                            when (DATEPART(mm,BITIRMETARIHI)>3 and DATEPART(mm,BITIRMETARIHI)<7) then 2
        //                                            when (DATEPART(mm,BITIRMETARIHI)>6 and DATEPART(mm,BITIRMETARIHI)<10) then 3
        //                                            when (DATEPART(mm,BITIRMETARIHI)>9 and DATEPART(mm,BITIRMETARIHI)<13) then 4
        //                                            else 0 end
        //                                            where [BASLAMATARIHI] is null and BITIRMETARIHI is not null
        //                                            and (donem is not null and donem <>'')");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"update [PERPERSONELKURS]
        //                                            set donem = 
        //                                            case DONEM when 'I. 6 Aylık' then 1
        //                                                 when 'II. 6 Aylık' then 3
        //                                            else 0 end
        //                                            where [BASLAMATARIHI] is null and BITIRMETARIHI is null
        //                                            and (donem is not null and donem <>'')");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"alter table PERPERSONELKURS alter column DONEM int,  KATILIMTIPI INT , KURSNITELIGI varchar(120), 
        //                                             YAPILDIGIIL varchar(20),  EGITIMSAATI INT");//05.03.2018 Duygu

        //        //                ve.hata = "";
        //        //                ve.ExecuteNonquery(@"INSERT INTO PERNEDEN (KOD,AD,BASLAMA,AYRILMA) VALUES ((SELECT MAX(KOD)+1 FROM PERNEDEN),'GEÇİCİ GÖREVLİ',0,1);
        //        //INSERT INTO PERNEDEN (KOD,AD,BASLAMA,AYRILMA) VALUES ((SELECT MAX(KOD)+1 FROM PERNEDEN),'GÖREVLENDİRME',0,1);");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"update PERSINIF set KISAAD='GİH' where KOD =1
        //                                            update PERSINIF set KISAAD='MİA' where KOD	=2
        //                                            update PERSINIF set KISAAD='SH' where KOD	=3
        //                                            update PERSINIF set KISAAD='TH' where KOD	=4
        //                                            update PERSINIF set KISAAD='EÖH' where KOD	=5
        //                                            update PERSINIF set KISAAD='AH' where KOD	=6
        //                                            update PERSINIF set KISAAD='EH' where KOD	=7
        //                                            update PERSINIF set KISAAD='DH' where KOD	=8
        //                                            update PERSINIF set KISAAD='YH' where KOD	=9
        //                                            update PERSINIF set KISAAD='MİH' where KOD	=10");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"update PERSINIF set KISAAD='GİH' where KOD =1;
        //                                            update PERSINIF set KISAAD='MİA' where KOD	=2;
        //                                            update PERSINIF set KISAAD='SH' where KOD	=3;
        //                                            update PERSINIF set KISAAD='TH' where KOD	=4;
        //                                            update PERSINIF set KISAAD='EÖH' where KOD	=5;
        //                                            update PERSINIF set KISAAD='AH' where KOD	=6;
        //                                            update PERSINIF set KISAAD='EH' where KOD	=7;
        //                                            update PERSINIF set KISAAD='DH' where KOD	=8;
        //                                            update PERSINIF set KISAAD='YH' where KOD	=9;
        //                                            update PERSINIF set KISAAD='MİH' where KOD	=10");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery("ALTER TABLE PERPERSONELAILE ADD ESININCALISTIGIKURUM varchar(100),  ESININUNVANI varchar(100),  ESININGOREVYERI varchar(100),  COCUGUNUNOKULUNUNADI varchar(100)");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery("ALTER TABLE PERPERSONELIZIN ADD RAPORTURU INT");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery("ALTER TABLE PERPERSONELMEMURIYET ADD MALUL INT,  DAIREKODU INT");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"CREATE TABLE [dbo].[PERPERSONELDAIRE](
        //                                            [KOD] [int] IDENTITY(1,1) NOT NULL,
        //                                            [ANAKOD] [int] NULL,
        //                                            [BIRIMKODU] [int] NULL,
        //                                            [ADI] [nvarchar](100) NULL,
        //                                            CONSTRAINT [PK_PERPERSONELDAIRE] PRIMARY KEY CLUSTERED 
        //                                            (
        //                                            [KOD] ASC
        //                                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        //                                            ) ON [PRIMARY]");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery("ALTER TABLE dbo.PERPERSONELKURS ADD KATILIMTIPI int NULL");

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"update PERPERSONELKURS set KATILIMTIPI=1 where KURSKODU IN (1,2,3,4,5,6,7,8);
        //                                             update PERPERSONELKURS set KATILIMTIPI=2 where KURSKODU=9");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"update PERPERSONELKURS set KATILIMTIPI=1 where KURSKODU IN (1,2,3,4,5,6,7,8);
        //                                             update PERPERSONELKURS set KATILIMTIPI=2 where KURSKODU=9");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery("ALTER TABLE PERIZINTUR ADD ATAKOD INT");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery("ALTER TABLE PERIZINTUR ADD ATAKOD INT");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"UPDATE PERIZINTUR SET AD='104/C',SIRA=11,ATAKOD=15 WHERE KOD=2;
        //                                            UPDATE PERIZINTUR SET AD='Evlenme İzni',SIRA=9,ATAKOD=15 WHERE KOD=3;
        //                                            UPDATE PERIZINTUR SET AD='Babalık İzni',SIRA=8,ATAKOD=15 WHERE KOD=4;
        //                                            UPDATE PERIZINTUR SET AD='Ölüm İzni',SIRA=10,ATAKOD=15 WHERE KOD=5;
        //                                            UPDATE PERIZINTUR SET AD='Sıhhi',SIRA=2 WHERE KOD=6;
        //                                            UPDATE PERIZINTUR SET AD='Analık İzni',SIRA=7,ATAKOD=15 WHERE KOD=7;
        //                                            UPDATE PERIZINTUR SET AD='Aylıksız',SIRA=14 WHERE KOD=9;
        //                                            UPDATE PERIZINTUR SET AD='Heyet',SIRA=4,ATAKOD=6 WHERE KOD=10");//05.03.2018 Duygu

        //        //                ve.hata = "";
        //        //                ve.ExecuteNonquery(@"INSERT INTO PERIZINTUR (KOD,AD,SURE,GORULEBILIRLIK,SIRA,ATAKOD) VALUES (11,'Tek Hekim',0,2,3,6);
        //        //INSERT INTO PERIZINTUR (KOD,AD,SURE,GORULEBILIRLIK,SIRA,ATAKOD) VALUES (12,'Refakat İzni',0,2,5,6);
        //        //INSERT INTO PERIZINTUR (KOD,AD,SURE,GORULEBILIRLIK,SIRA,ATAKOD) VALUES (13,'Süt İzni',0,2,12,15);
        //        //INSERT INTO PERIZINTUR (KOD,AD,SURE,GORULEBILIRLIK,SIRA,ATAKOD) VALUES (14,'104/E',0,2,13,15);
        //        //INSERT INTO PERIZINTUR (KOD,AD,SURE,GORULEBILIRLIK,SIRA) VALUES (15,'Mazeret İzni',0,2,6);

        //        //INSERT INTO PERPERSONELDAIRE (ADI,KOD,BIRIMKODU,DURUM) VALUES ('Strateji Geliştirme Başkanlığı',1,923,0);
        //        //INSERT INTO PERPERSONELDAIRE (ADI,KOD,ANAKOD,BIRIMKODU,DURUM) VALUES ('Stratejik Yönetim ve Planlama Dairesi',2,1,923,1);
        //        //INSERT INTO PERPERSONELDAIRE (ADI,KOD,ANAKOD,BIRIMKODU,DURUM) VALUES ('Bütçe ve Kesin Hesap Dairesi',3,1,923,1);
        //        //INSERT INTO PERPERSONELDAIRE (ADI,KOD,ANAKOD,BIRIMKODU,DURUM) VALUES ('Mali Yönetim Dairesi',4,1,923,1);
        //        //INSERT INTO PERPERSONELDAIRE (ADI,KOD,ANAKOD,BIRIMKODU,DURUM) VALUES ('İç Kontrol Dairesi',5,1,923,1);
        //        //INSERT INTO PERPERSONELDAIRE (ADI,KOD,ANAKOD,BIRIMKODU,DURUM) VALUES ('Kurumsal Yönetim Yazılımları Dairesi',6,1,923,1);
        //        //INSERT INTO PERPERSONELDAIRE (ADI,KOD,ANAKOD,BIRIMKODU,DURUM) VALUES ('Destek Hizmetleri Dairesi',7,1,923,1)");//05.03.2018 Duygu

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"alter table PERPERSONELDAIRE add DURUM int null");

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"CREATE TABLE [dbo].[PEREGITIMTURLERI](
        //            [KOD] [int] NOT NULL,
        //            [ATAKOD] [int] NULL,
        //            [AD] [varchar](150) NULL,
        //            [SEVIYE] [int] NULL,
        //         CONSTRAINT [PK_PEREGITIMTURLERI] PRIMARY KEY CLUSTERED 
        //        (
        //            [KOD] ASC
        //        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        //        ) ON [PRIMARY]

        //        GO

        //        SET ANSI_PADDING OFF
        //        GO

        //        ALTER TABLE [dbo].[PEREGITIMTURLERI]  WITH CHECK ADD  CONSTRAINT [FK_PEREGITIMTURLERI_PEREGITIMTURLERI] FOREIGN KEY([ATAKOD])
        //        REFERENCES [dbo].[PEREGITIMTURLERI] ([KOD])
        //        GO

        //        ALTER TABLE [dbo].[PEREGITIMTURLERI] CHECK CONSTRAINT [FK_PEREGITIMTURLERI_PEREGITIMTURLERI]
        //        GO");

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"alter table PERPERSONELDAIRE alter column ADI varchar(100) not null");

        //                        ve.hata = "";
        //                        ve.ExecuteNonquery(@"alter table PERPERSONELDAIRE alter column BIRIMKODU varchar(3) not null");



        //                        ve.VeriErisimKapat();
        //                        */
        //    }
        //}

        public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            string deger = "";
            if (uzayAd == "")
                deger = servisUZY.UzayDegeriStr(new Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriStr(new Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        public static double UzaySayiDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            if (servisUZY == null)
                servisUZY = TNS.UZY.Arac.Tanimla();

            double deger = 0;
            if (uzayAd == "")
                deger = servisUZY.UzayDegeriDbl(new Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriDbl(new Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        public Sonuc GorevKaydet(Kullanici islemiYapan, Gorev p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorev().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray GorevListele(Kullanici islemiYapan, Gorev p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorev().Listele(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray GorevSorgula(Kullanici islemiYapan, Gorev p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorev().Sorgula(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevSil(Kullanici islemiYapan, Gorev p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorev().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevDurumDegistir(Kullanici islemiYapan, int gorevYil, int gorevKod, int durum)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorev().DurumDegistir(null, islemiYapan, new Gorev { yil = gorevYil, kod = gorevKod, durum = durum });
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevDurumDegistir(Kullanici islemiYapan, string prEvrakNo, string evrEvrakNo, int durum)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorev().DurumDegistir(null, islemiYapan, prEvrakNo, evrEvrakNo, durum);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevEvrakBilgiKaydet(Kullanici islemiYapan, int gorevYil, int gorevKod, string evrEvrakNo, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorev().KaydetEvrakBilgi(islemiYapan, gorevYil, gorevKod, evrEvrakNo, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevAlanBilgiKaydet(Kullanici islemiYapan, int gorevYil, int gorevKod, string alan1, string imza1, string imza2, string imza3)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorev().KaydetAlanBilgi(islemiYapan, gorevYil, gorevKod, alan1, imza1, imza2, imza3);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevYerTarihKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevYil, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorevYerTarih().Kaydet(islemiYapan, liste, gorevYil, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray GorevYerTarihListele(GorevYerTarih p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorevYerTarih().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevYerTarihSil(Kullanici islemiYapan, int gorevYil, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorevYerTarih().Sil(islemiYapan, gorevYil, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevPersonelKaydet(Kullanici islemiYapan, ObjectArray liste, int gorevYil, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorevPersonel().Kaydet(islemiYapan, liste, gorevYil, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray GorevPersonelListele(GorevPersonel p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorevPersonel().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevPersonelSil(Kullanici islemiYapan, int gorevYil, int gorevKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorevPersonel().Sil(islemiYapan, gorevYil, gorevKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKaydet(Kullanici islemiYapan, Personel p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelBirimKaydet(Kullanici islemiYapan, Personel p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetBirim(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelListele(Kullanici kullanan, Personel p)
        {
            try
            {
                return new PERPersonel().Listele(kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray BirimBazliPersonelSonMemuriyetListele(Kullanici kullanan, Personel p)
        {
            try
            {
                return new PERPersonel().BirimBazliPersonelSonMemuriyetListele(kullanan, p);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        public ObjectArray PersonelListelePersonelRapor(Kullanici kullanan, Personel p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListelePersonelRapor(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelListeleEgitimRapor(Kullanici kullanan, Personel p, PersonelKurs pk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleEgitimRapor(kullanan, p, pk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray DonemlikEgitimFaaliyetRaporu(Kullanici kullanan, Personel p, PersonelKurs pk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().DonemlikEgitimFaaliyetRaporu(kullanan, p, pk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelListeleDaireRapor(Kullanici kullanan, Personel p, PersonelMemuriyet pk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleDaireRapor(kullanan, p, pk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray PersonelListeleIzinRapor(Kullanici kullanan, Personel p, PersonelIzin pk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleIzinRapor(kullanan, p, pk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelListeleIzinDurum(Kullanici kullanan, PersonelIzinDurum pk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleIzinDurum(kullanan, pk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelListeleUnvanKadro(Kullanici kullanan, Personel p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleUnvanKadro(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelListeleOzet(Kullanici kullanan, Personel p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleOzet(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Personel PersonelMemuriyetTarihleriListele(string tcKimlikNo)
        {
            Personel donusDeger = new Personel();
            try
            {
                donusDeger = new PERPersonel().ListeleMemuriyetTarihleri(tcKimlikNo);
            }
            catch { }

            return donusDeger;
        }

        public Sonuc PersonelSil(Kullanici islemiYapan, Personel p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Personel Adres Bilgilerini kaydeden fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Personel Adres bilgisi, Personel Adres tipinde sınıf</param>
        /// <returns>Sonuç tipinde sınıf döndürür</returns>
        public Sonuc PersonelAdresKaydet(Kullanici islemiYapan, PersonelAdres p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetAdres(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Personel Adres bilgilerini listeleyen fonksiyon
        /// </summary>
        /// <param name="islemiYapan">Kullanıcı bilgisi, Kullanıcı tipinde sınıf</param>
        /// <param name="p">Personel Adres bilgisi, Personel Adres tipinde sınıf</param>
        /// <returns>Object Array tipinde sınıf döndürür</returns>
        public ObjectArray PersonelAdresListele(Kullanici islemiYapan, PersonelAdres p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleAdres(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Personel resminin kayıt edilmesi işlemini gerçekleştirir
        /// </summary>
        /// <param name="bilgi">Resme ait bilgileri saklar</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc PersonelResimKaydet(PersonelResim bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = PERPersonelResim.Kaydet(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Personele ait resmin silinmesi işlemini gerçekleştirir
        /// </summary>
        /// <param name="bilgi">Resme ait bilgileri saklar</param>
        /// <returns>Sonuc yapısı içerisindeki; islemSonuc:komut çalışmışsa true, çalışmamışsa false; hataStr:Hata olmuşsa, hata bilgisini, döndürür.</returns>
        public Sonuc PersonelResimSil(PersonelResim bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = PERPersonelResim.Sil(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Personele ait resim bilgisinin alınması işlemini gerçekleştirir
        /// </summary>
        /// <param name="bilgi">Resme ait bilgileri içerir</param>
        /// <returns>ObjectArray yapısı içinde, uygun olan verileri döndürür.</returns>
        public ObjectArray PersonelResimGetir(string bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = PERPersonelResim.Getir(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc PersonelBelgeKaydet(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetBelge(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelBelgeListele(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = new PERPersonel().ListeleBelge(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc PersonelBelgeListesiGetir(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().ListeleBelgeGetir(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelBelgeSil(Kullanici islemiYapan, PersonelBelgeler belge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilBelge(islemiYapan, belge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelDilKaydet(Kullanici islemiYapan, PersonelDil p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetDil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelDilListele(Kullanici kullanan, PersonelDil p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleDil(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelDilSil(Kullanici islemiYapan, PersonelDil p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilDil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKursKaydet(Kullanici islemiYapan, PersonelKurs p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetKurs(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public Sonuc PersonelEgitimTanimKaydet(Kullanici islemiYapan, PersonelEgitimTurleri p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetPersonelEgitimTanim(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray PersonelKursListele(Kullanici kullanan, PersonelKurs p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleKurs(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelEgitimTurleriListele(Kullanici kullanan, PersonelEgitimTurleri p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleEgitimTurleri(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray ListeleEgitimTurBilgileriRecursive(Kullanici kullanan, int p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleEgitimTurBilgileriRecursive(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKursSil(Kullanici islemiYapan, PersonelKurs p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilKurs(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }


        public Sonuc PersonelEgitimTanimSil(Kullanici islemiYapan, PersonelEgitimTurleri p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilEgitimTanim(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelAileKaydet(Kullanici islemiYapan, PersonelAile p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetAile(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelAileListele(Kullanici kullanan, PersonelAile p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleAile(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelAileSil(Kullanici islemiYapan, PersonelAile p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilAile(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelMalBildirimiKaydet(Kullanici islemiYapan, PersonelMalBildirimi p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetMalBildirimi(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelMalBildirimiListele(Kullanici kullanan, PersonelMalBildirimi p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleMalBildirimi(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelMalBildirimiSil(Kullanici islemiYapan, PersonelMalBildirimi p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilMalBildirimi(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelCezaOdulKaydet(Kullanici islemiYapan, PersonelCezaOdul p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetCezaOdul(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelCezaOdulListele(Kullanici kullanan, PersonelCezaOdul p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleCezaOdul(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelCezaOdulSil(Kullanici islemiYapan, PersonelCezaOdul p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilCezaOdul(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelOgrenimKaydet(Kullanici islemiYapan, PersonelOgrenim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetOgrenim(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelOgrenimListele(Kullanici kullanan, PersonelOgrenim p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleOgrenim(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelOgrenimSil(Kullanici islemiYapan, PersonelOgrenim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilOgrenim(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        public ObjectArray BirimDaireBilgileriListele(BirimDaireBilgileri daire)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = new PERPersonel().ListeleBirimDaire(daire);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc DilKaydet(Kullanici islemiYapan, Dil p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERDil().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray DilListele(Dil p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERDil().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc DilSil(Kullanici islemiYapan, Dil p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERDil().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelMemuriyetKaydet(Kullanici islemiYapan, PersonelMemuriyet p)
        {
            try
            {
                return new PERPersonel().KaydetMemuriyet(islemiYapan, p);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray PersonelMemuriyetListele(Kullanici kullanan, PersonelMemuriyet p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleMemuriyet(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelMemuriyetSil(Kullanici islemiYapan, PersonelMemuriyet p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilMemuriyet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKadroKaydet(Kullanici islemiYapan, PersonelKadro p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetKadro(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelKadroListele(Kullanici kullanan, PersonelKadro p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleKadro(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelKadroSil(Kullanici islemiYapan, PersonelKadro p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilKadro(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KursKaydet(Kullanici islemiYapan, Kurs p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERKurs().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray KursListele(Kurs p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERKurs().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KursSil(Kullanici islemiYapan, Kurs p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERKurs().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc BankaKaydet(Kullanici islemiYapan, Banka p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERBanka().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray BankaListele(Banka p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERBanka().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc BankaSil(Kullanici islemiYapan, Banka p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERBanka().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc BankaSubeKaydet(Kullanici islemiYapan, BankaSube p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERBankaSube().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray BankaSubeListele(BankaSube p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERBankaSube().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc BankaSubeSil(Kullanici islemiYapan, BankaSube p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERBankaSube().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc OgrenimKaydet(Kullanici islemiYapan, Ogrenim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PEROgrenim().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray OgrenimListele(Ogrenim p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PEROgrenim().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc OgrenimSil(Kullanici islemiYapan, Ogrenim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PEROgrenim().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc IzinTurKaydet(Kullanici islemiYapan, IzinTur p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERIzinTur().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray IzinTurListele(IzinTur p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERIzinTur().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc IzinTurSil(Kullanici islemiYapan, IzinTur p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERIzinTur().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray IzinTurIlkKaydaDon()
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                //donusDeger = new PERIzinTur().IlkKayitlariOlustur();
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Personel Yıllık İzin Talepleri için eklenen servisler
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ObjectArray IzinDonemListele(IzinDonem p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERIzinDonem().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelIzinTalepListele(Kullanici kullanan, PersonelIzinTalep p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleIzinTalep(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinTalepKaydet(Kullanici islemiYapan, PersonelIzinTalep[] p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetIzinTalep(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }
        //
        // Personel Izin Talep SONU

        public Sonuc UnvanKaydet(Kullanici islemiYapan, Unvan p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERUnvan().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray UnvanListele(Unvan p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERUnvan().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc UnvanSil(Kullanici islemiYapan, Unvan p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERUnvan().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SinifKaydet(Kullanici islemiYapan, Sinif p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERSinif().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray SinifListele(Sinif p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERSinif().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SinifSil(Kullanici islemiYapan, Sinif p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERSinif().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc NedenKaydet(Kullanici islemiYapan, Neden p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERNeden().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray NedenListele(Neden p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERNeden().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc NedenSil(Kullanici islemiYapan, Neden p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERNeden().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc UstBirimKaydet(Kullanici islemiYapan, UstBirim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERUstBirim().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray UstBirimListele(UstBirim p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERUstBirim().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc UstBirimSil(Kullanici islemiYapan, UstBirim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERUstBirim().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KadroKaydet(Kullanici islemiYapan, ObjectArray p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERKadro().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray KadroRapor(Kullanici islemiYapan, Kadro p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERKadro().Rapor(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KadroSil(Kullanici islemiYapan, TNSDateTime tarih)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERKadro().Sil(islemiYapan, tarih);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray KadroVizeListele()
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERKadro().VizeListele();
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SozlesmeliBordroTanimKaydet(Kullanici islemiYapan, SozlesmeliBordroTanim p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetSozlesmeliBordroTanim(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SozlesmeliBordroTanimSil(Kullanici islemiYapan, SozlesmeliBordroTanim kriter)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilSozlesmeliBordroTanim(islemiYapan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = PEROgrenim.OgrenimBazliPersonelSayilariListele(muhasebeKod, birimKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray UnvanBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = PERUnvan.UnvanBazliPersonelSayilariListele(muhasebeKod, birimKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray SinifBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = PERSinif.SinifBazliPersonelSayilariListele(muhasebeKod, birimKod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray UnvanVeKadroDereceBazliPersonelSayilariListele(Kullanici kullanan, KadroKontenjan kk)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = PERUnvan.UnvanVeKadroDereceBazliPersonelSayilariListele(kullanan, kk);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KadroKontenjanKaydet(Kullanici kullanan, KadroKontenjan kk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = PERKadroKontenjan.Kaydet(null, kullanan, kk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc KadroKontenjanSil(Kullanici kullanan, KadroKontenjan kk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = PERKadroKontenjan.Sil(null, kullanan, kk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public KadroKontenjan[] KadroKontenjanListele(Kullanici kullanan, KadroKontenjan kk, bool detay)
        {
            try
            {
                return PERKadroKontenjan.Listele(null, kullanan, kk, detay);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Sonuc PersonelIzinKaydet(Kullanici islemiYapan, PersonelIzin p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetIzin(islemiYapan, p, false);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinEvrakBilgiKaydet(Kullanici islemiYapan, PersonelIzin p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetIzinEvrakBilgi(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinAlanKaydet(Kullanici islemiYapan, PersonelIzin p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetIzinAlan(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelIzinListele(Kullanici kullanan, PersonelIzin p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleIzin(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinSil(Kullanici islemiYapan, PersonelIzin p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilIzin(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinOnayla(Kullanici islemiYapan, PersonelIzin p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetIzin(islemiYapan, p, true);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinDevirKaydet(Kullanici islemiYapan, PersonelIzinDevir[] p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().KaydetIzinDevir(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray PersonelIzinDevirListele(Kullanici kullanan, PersonelIzinDevir p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleIzinDevir(null, kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc PersonelIzinDevirSil(Kullanici islemiYapan, PersonelIzinDevir p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().SilIzinDevir(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public int PersonelKullanilanIzinHesapla(string tcKimlikNo, int yil, TNSDateTime baslangicTarih, int izinTip)
        {
            int donusDeger = 0;
            try
            {
                donusDeger = new PERPersonel().KullanilanIzinHesapla(tcKimlikNo, yil, baslangicTarih, izinTip);
            }
            catch (Exception)
            {

            }
            return donusDeger;
        }

        public ObjectArray PersonelTarihceListele(Kullanici kullanan, PersonelTarihce p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERPersonel().ListeleTarihce(kullanan, p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="personelTCKimlikNo"></param>
        /// <returns>TARIH,KATEGORI,DEGISENALAN,YENIDEGER,ESKIDEGER,DEGISIKLIKYAPAN</returns>
        public String[] PersonelTarihceListele(Kullanici kullanan, string personelTCKimlikNo)
        {
            String[] donusDeger = null;
            try
            {
                donusDeger = PERPersonelTarihce.Listele(kullanan, personelTCKimlikNo);
            }
            catch { }

            return donusDeger;
        }

        public Sonuc GorevKonuKaydet(Kullanici islemiYapan, GorevKonu p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorevKonu().Kaydet(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray GorevKonuListele(GorevKonu p)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new PERGorevKonu().Listele(p);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc GorevKonuSil(Kullanici islemiYapan, GorevKonu p)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERGorevKonu().Sil(islemiYapan, p);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Personel nitelik bilgilerini varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// Personelin sistemde kayıtlı olması gerekmektedir, eğer kayıtlı değilse hata döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PersonelBilgiBirikimiKaydet(Kullanici kullanan, PersonelBilgiBirikimi personel)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TNS.PER.IsKurallari.PERNitelik.Kaydet(null, kullanan, personel);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan personel nitelik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel nitelik bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel nitelik listesi döndürülür.</returns>
        public ObjectArray PersonelBilgiBirikimiListele(Kullanici kullanan, PersonelBilgiBirikimi personel)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TNS.PER.IsKurallari.PERNitelik.Listele(null, kullanan, personel);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IzinEvrakNoKaydet(Kullanici kullanan, string tcKimlikNo, int siraNo, string prEvrakNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new PERPersonel().IzinEvrakNoKaydet(kullanan, tcKimlikNo, siraNo, prEvrakNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public string IzinEvrakNoOku(Kullanici kullanan, string tcKimlikNo, int siraNo)
        {
            string evrakNo = "";
            try
            {
                evrakNo = new PERPersonel().IzinEvrakNoOku(tcKimlikNo, siraNo);
            }
            catch (Exception e)
            {
                evrakNo = e.ToString();

            }

            return evrakNo;
        }

        public string ListeleEgitimTurleriAdlari(Kullanici kullanan, int kod)
        {
            string egitimAdlari = "";
            try
            {
                egitimAdlari = new PERPersonel().ListeleEgitimTurleriAdlari(kullanan, kod);
            }
            catch (Exception e)
            {
                egitimAdlari = e.ToString();

            }

            return egitimAdlari;
        }

        public Sonuc BirimKoduDegistir(Kullanici kullanan, int eskiSaymanlik, string eskiBirim, int yeniSaymanlik, string yeniBirim)
        {
            return PERPersonel.BirimKoduDegistir(null, kullanan, eskiSaymanlik, eskiBirim, yeniSaymanlik, yeniBirim);
        }
    }
}

namespace TNS
{
    /// <summary>
    /// Yetki tipi bilgileriyle ilgili yordamları tutan sınıf.
    /// </summary>
    public class KYMListe : YetkiTipiListesi, YetkiTipiAgaci
    {
        public static IORGServis servisORG = TNS.ORG.Arac.Tanimla();

        /// <summary>
        /// Parametre olark verilen bilgilere göre yetki tipi elemanlarını ListeSatir nesne dizisi olarak getiren yordam.
        /// </summary>
        /// <param name="kullanan">Kullanıcı bilgilerini içeren nesne</param>
        /// <param name="yil">Yıl bilgisini içeren nesne</param>
        /// <param name="ekParamCagri">Çağrı bilgisini içeren nesne</param>
        /// <param name="ekParamTanim">Tanim bilgisini içeren nesne</param>
        /// <returns>ListeSatir nesne dizisi olarak döndürülür.</returns>
        public ListeSatir[] YetkiTipiListeElemanlari(Kullanici kullanan, int yil, string ekParamCagri, string ekParamTanim)
        {
            throw new NotImplementedException("Bilinmeyen parametre!");
        }

        public ListeSatir[] YetkiTipiAgacElemanlari(Kullanici kullanan, int yil, string ustKod, string ekParamCagri, string ekParamTanim)
        {
            TNS.ORG.OrganizasyonBirim kriter = new TNS.ORG.OrganizasyonBirim();
            kriter.anaBirimKod = OrtakFonksiyonlar.ConvertToInt(ustKod, 0);
            if (kriter.anaBirimKod == 0)
                kriter.anaBirimKod = -1;
            ObjectArray dler = servisORG.OrganizasyonBirimListele(kriter, false, false);

            ListeSatir[] donusDeger = new ListeSatir[dler.ObjeSayisi];

            int i = 0;

            foreach (OrganizasyonBirim y in dler.objeler)
            {
                donusDeger[i] = new ListeSatir(y.kod.ToString(), y.adi);
                donusDeger[i].cocuguVar = y.altDalVar;
                i++;
            }
            return donusDeger;
        }

        public string YetkiTipiKodAd(Kullanici kullanan, int yil, string kod, string ekParamCagri, string ekParamTanim)
        {
            OrganizasyonBirim bilgi = new OrganizasyonBirim();
            bilgi.kod = OrtakFonksiyonlar.ConvertToInt(kod, 0);

            ObjectArray liste = servisORG.OrganizasyonBirimListele(bilgi, false, false);

            if (liste.objeler.Count > 0)
            {
                bilgi = (OrganizasyonBirim)liste[0];
                return bilgi.kod + " - " + bilgi.adi;
            }
            return "";
        }
    }
}
