using System;
using TNS.KYM;
using TNS.UZY;
using TNS.Veri;

namespace TNS.TMM.IsKurallari
{
    /// <summary>
    /// ITMMServis arayüzünden türeyen, istemciden gelen çağrıları ilgili
    /// sınıflardaki ilgili yordamlara yönlendiren fonksiyonları tutan sınıf
    /// </summary>
    [Serializable()]
    public class TMMServis : MarshalByRefObject, ITMMServis
    {
        public static bool tabloDegisimYap = true;

        /// <summary>
        /// Parametresiz Taşınır mal servisi nesnesi oluşturucu yordam
        /// </summary>
        public TMMServis()
        {
            lock ("OncekiIslemiBiteneKadarBekle")
            {
                if (tabloDegisimYap)
                {
                    tabloDegisimYap = false;
                    TNS.TMM.IsKurallari.TMMTabloDegisiklikleri.Kaydet();
                }
            }
        }

        //private static bool yenialan = true;
        //private void YeniAlanOlustur()
        //{
        //    if (yenialan)
        //    {
        //        yenialan = false;

        //        string sql = "";
        //        VeriErisim ve = VeriErisim.VeriErisimBul(null, "BaglantiSatiriTMM", TNS.KYM.Arac.KimlikBul(null));
        //        try
        //        {
        //            //if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
        //            //{
        //            //    TNSDateTime tarih = new TNSDateTime(DateTime.Now);
        //            //    string aciklama = "";

        //            //    ve.hata = "";
        //            //    int versiyon = ve.TekKolonSQLInt("SELECT MAX(KOD) FROM VERSIYON");

        //            //    if (versiyon == 0)
        //            //    {
        //            //        sql = @"CREATE TABLE [dbo].[VERSIYON](
        //            //             [KOD] [int] NOT NULL,
        //            //             [TARIH] [datetime] NULL,
        //            //             [ACIKLAMA] [varchar](500) NULL,
        //            //             CONSTRAINT [PK_VERSIYON] PRIMARY KEY CLUSTERED 
        //            //            ([KOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);
        //            //    }
        //            //    versiyon++;

        //            //    if (versiyon == 1)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET SET FIYAT = 1341359.080000000000, KDVLIBIRIMFIYAT = 1341359.080000000000
        //            //                WHERE FIYAT = 1334205.330000000000 AND PRSICILNO IN(156209,156210,156211,156212,156213,156214,156215,156216)";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        sql = @"UPDATE TASISLEMDETAY SET BIRIMFIYAT = 1341359.080000000000, BIRIMFIYATKDVLI = 1341359.080000000000
        //            //                WHERE BIRIMFIYAT = 1334205.330000000000 AND YIL= 2018 AND HARCAMABIRIMKOD = '000' AND FISNO IN('000163','000164','000165','000166','000167','000168','000169','000170')";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        sql = @"UPDATE TASZIMMETDETAY SET BIRIMFIYAT = 1341359.080000000000
        //            //                WHERE BIRIMFIYAT = 1334205.330000000000 AND YIL = 2018 and HARCAMABIRIMKOD = '000' AND FISNO IN ('007308','007309','007310','007311','007312','007313','007314','007315')";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);


        //            //        sql = @"UPDATE TASSICILNOOZELLIK
        //            //                SET BULUNDUGUYER = H.NEREYEGITTI
        //            //                FROM TASSICILNOOZELLIK
        //            //                INNER JOIN TASSICILNOHAREKET H ON TASSICILNOOZELLIK.PRSICILNO = H.PRSICILNO
        //            //                WHERE SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = TASSICILNOOZELLIK.PRSICILNO)
        //            //                AND (AMBARKOD = '01' OR (AMBARKOD = '51' AND H.NEREYEGITTI != ''))
        //            //                AND H.NEREYEGITTI <> TASSICILNOOZELLIK.BULUNDUGUYER
        //            //                AND H.ISLEMTIPIKOD IN(5,1001,1002)";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "8 adet demirbaşaşın fiyatlarının değiştirilmesi talebi.\nZimmet düşme ve verme işleminde TASSICILNOOZELLIK tablosunda güncelleme yapılmasına yönelik düzelme.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }

        //            //    }

        //            //    if (versiyon == 2)
        //            //    {
        //            //        sql = @"ALTER VIEW [dbo].[TASODA]
        //            //                AS
        //            //                SELECT '00001' AS MUHASEBEKOD, BIRIMKOD AS HARCAMABIRIMKOD, CAST(KOD AS varchar) AS KOD, AD, '' AS AMBARKOD, USTKOD, SEVIYE, KOD AS UKOD, DURUM, MALIYETMERKEZI, 
        //            //                GORKODESKI AS ESKIKOD
        //            //                FROM dbo.YERLESKEODA";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASODA tablosuna GORKODESKI alanının eklenmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }



        //            //    if (versiyon == 3)
        //            //    {
        //            //        sql = @"UPDATE YERLESKEODA SET GORKODESKI = GORKOD

        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01011000102' WHERE KOD = 39
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01011134' WHERE KOD = 67
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D010111341' WHERE KOD = 60
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01011156' WHERE KOD = 49
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01011163001' WHERE KOD = 61
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012000201' WHERE KOD = 57
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012000202' WHERE KOD = 68
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012000207' WHERE KOD = 50
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012000208' WHERE KOD = 65
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012133' WHERE KOD = 73
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012143' WHERE KOD = 62
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012160' WHERE KOD = 37
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012163' WHERE KOD = 40
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012163001' WHERE KOD = 63
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01012204' WHERE KOD = 69
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01013140' WHERE KOD = 70
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D01013163001' WHERE KOD = 71
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101B00008' WHERE KOD = 51
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z00013' WHERE KOD = 52
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z0002' WHERE KOD = 53
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z0003' WHERE KOD = 72
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z0004' WHERE KOD = 54
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z001' WHERE KOD = 55
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z125' WHERE KOD = 38
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z128' WHERE KOD = 56
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z1281' WHERE KOD = 58
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z129' WHERE KOD = 42
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z1291' WHERE KOD = 45
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z131' WHERE KOD = 66
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z1311' WHERE KOD = 59
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z132' WHERE KOD = 43
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z139' WHERE KOD = 41
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z1391' WHERE KOD = 47
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z1392' WHERE KOD = 46
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z153' WHERE KOD = 44
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z155' WHERE KOD = 48
        //            //                UPDATE YERLESKEODA SET GORKODESKI = '1D0101Z163001' WHERE KOD = 64
        //            //                UPDATE YERLESKEODA SET GORKODESKI = 'İA2613K10' WHERE KOD = 1555
        //            //                ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "YERLESKEODA tablosunda değişen GORKOD değerlerine ait gerçek değerlerinin GORKODESKI alanına güncellenmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 4)
        //            //    {
        //            //        sql = @"UPDATE TASISLEMOZELLIK SET BULUNDUGUYER = YERLESKEODA.KOD
        //            //                FROM TASISLEMOZELLIK INNER JOIN YERLESKEODA ON YERLESKEODA.GORKODESKI = TASISLEMOZELLIK.BULUNDUGUYER

        //            //                UPDATE TASSICILNOOZELLIK SET BULUNDUGUYER = YERLESKEODA.KOD
        //            //                FROM TASSICILNOOZELLIK INNER JOIN YERLESKEODA ON YERLESKEODA.GORKODESKI = TASSICILNOOZELLIK.BULUNDUGUYER

        //            //                UPDATE TASSICILNOHAREKET SET NEREYEGITTI = YERLESKEODA.KOD
        //            //                FROM TASSICILNOHAREKET INNER JOIN YERLESKEODA ON YERLESKEODA.GORKODESKI = TASSICILNOHAREKET.NEREYEGITTI

        //            //                UPDATE TASSICILNOHAREKET SET NEREYEGITTI = 1555
        //            //                WHERE NEREYEGITTI = 'İA26K10'

        //            //                UPDATE TASSICILNOOZELLIK SET BULUNDUGUYER = 1555
        //            //                WHERE BULUNDUGUYER = 'İA26K10'
        //            //                ";
        //            //        ve.hata = "";
        //            //        ve.sureAsimi = 60 * 3;
        //            //        ve.ExecuteNonquery(sql);
        //            //        ve.sureAsimi = 30;

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Yerleşke bilgisi olan tablolarda GORKOD değerlerinin KOD değerleri ile değiştirilmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 5)
        //            //    {
        //            //        sql = @"UPDATE YERLESKEODA SET ALTDALVAR = 1
        //            //                WHERE ALTDALVAR = 0
        //            //                AND 0 < (SELECT COUNT(*) FROM YERLESKEODA IC WHERE IC.USTKOD = YERLESKEODA.KOD)";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Yerleşke tablosu için alt dal bilgi güncellemesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }


        //            //    if (versiyon == 6)
        //            //    {
        //            //        sql = @"UPDATE YERLESKEODA SET SAYMANLIKKOD = NULL, ILKOD = '', YERTURKOD = NULL, ORGKOD = NULL

        //            //                UPDATE YERLESKEODA SET BIRIMKOD = '001' WHERE BIRIMKOD = 965";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Yerleşke tablosu kayıtlarda düzeltme.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 7)
        //            //    {
        //            //        sql = @"UPDATE YERLESKEODA SET AD = 'ÜST YÖNETİM', ALTDALVAR = 1, USTKOD = 1 WHERE KOD = 24
        //            //                UPDATE YERLESKEODA SET AD = 'BAŞKANLIK MAKAMI', USTKOD = 24 WHERE GORKODESKI LIKE 'İA002A00'
        //            //                UPDATE YERLESKEODA SET AD = 'BŞK. YARD. ', USTKOD = 24 WHERE GORKODESKI LIKE 'İA012A01'
        //            //                UPDATE YERLESKEODA SET AD = 'BŞK. YRD. MURAT UYSAL', USTKOD = 24 WHERE GORKODESKI LIKE 'İA022A02'
        //            //                UPDATE YERLESKEODA SET AD = 'BŞK. YRD. ERKAN KİLİMCİ', USTKOD = 24 WHERE GORKODESKI LIKE 'İA032A03'
        //            //                UPDATE YERLESKEODA SET AD = 'BŞK .YRD. EMRAH ŞENER', USTKOD = 24 WHERE GORKODESKI LIKE 'İA042A04'
        //            //                UPDATE YERLESKEODA SET AD = 'OPE.VE DES.HİZ.KOR.EROL GÖNCÜ', USTKOD = 24 WHERE GORKODESKI LIKE 'İA302A05'
        //            //                UPDATE YERLESKEODA SET AD = 'ŞUBELER KOORD. AYHAN AYDIN', USTKOD = 24 WHERE GORKODESKI LIKE 'İA302A06'
        //            //                UPDATE YERLESKEODA SET AD = 'BAŞEKONOMİST A.HAKAN KARA', USTKOD = 24 WHERE GORKODESKI LIKE 'İA302A07'
        //            //                UPDATE YERLESKEODA SET AD = 'BAŞDANIŞMAN U.NAMIK KÜÇÜK', USTKOD = 24 WHERE GORKODESKI LIKE 'İA302A08'
        //            //                UPDATE YERLESKEODA SET AD = 'BANKA MECLİSİ', USTKOD = 24 WHERE GORKODESKI LIKE 'İA052A10'
        //            //                UPDATE YERLESKEODA SET AD = 'DENETLEME KURULU', USTKOD = 24 WHERE GORKODESKI LIKE 'İA062A20'
        //            //                UPDATE YERLESKEODA SET AD = 'PARA POLİTİKASI KRL.', USTKOD = 24 WHERE GORKODESKI LIKE 'İA082A35'

        //            //                UPDATE YERLESKEODA SET AD = 'STRATEJİ VE KURUM.YÖN.GN.M', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA182Ö10'
        //            //                UPDATE YERLESKEODA SET AD = 'STRATEJİ VE İŞ GELİŞ.MD.', USTKOD = 1449 WHERE GORKODESKI LIKE 'İA182Ö11'
        //            //                UPDATE YERLESKEODA SET AD = 'BAŞKANLIK OFİSİ MÜDÜRLÜĞÜ', USTKOD = 1449 WHERE GORKODESKI LIKE 'İA182Ö12'
        //            //                UPDATE YERLESKEODA SET AD = 'PROTOKOL MÜDÜRLÜĞÜ', USTKOD = 1449 WHERE GORKODESKI LIKE 'İA182Ö13'
        //            //                UPDATE YERLESKEODA SET AD = 'STRATEJİK PLANLAMA MD.', USTKOD = 1449 WHERE GORKODESKI LIKE 'İA182Ö14'
        //            //                UPDATE YERLESKEODA SET AD = 'B.MEC.DEN.KR.HİZ.MD.', USTKOD = 1449 WHERE GORKODESKI LIKE 'İA182Ö15'

        //            //                UPDATE YERLESKEODA SET AD = 'İLET.VE DIŞ İL.GN.MD', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA092A40'
        //            //                UPDATE YERLESKEODA SET AD = 'KURUMSAL İLETİŞİM MD.', USTKOD = 1455 WHERE GORKODESKI LIKE 'İA092A41'
        //            //                UPDATE YERLESKEODA SET AD = 'İKİLİ İLİŞK. VE TEKN.İŞBİRLİĞİ M', USTKOD = 1455 WHERE GORKODESKI LIKE 'İA0915A44'
        //            //                UPDATE YERLESKEODA SET AD = 'İLETİŞİM PLAN.VE YÖNETİMİ MD', USTKOD = 1455 WHERE GORKODESKI LIKE 'İA092A45'
        //            //                UPDATE YERLESKEODA SET AD = 'SANAT ODALARI', USTKOD = 1455 WHERE GORKODESKI LIKE 'İA092A46'
        //            //                UPDATE YERLESKEODA SET AD = 'ULUSL.KURUL.İLİŞ.MD.', USTKOD = 1455 WHERE GORKODESKI LIKE 'İA092A47'
        //            //                UPDATE YERLESKEODA SET AD = 'YATIRIMCI İLİŞKİLERİ MD.', USTKOD = 1455 WHERE GORKODESKI LIKE 'İA092A48'
        //            //                UPDATE YERLESKEODA SET AD = '17. KAT TOPLANTI ODASI', USTKOD = 1455 WHERE GORKODESKI LIKE 'İA0917A49'

        //            //                UPDATE YERLESKEODA SET AD = 'DANIŞMANLAR', USTKOD = 1 WHERE GORKODESKI LIKE 'İF0700A21'

        //            //                UPDATE YERLESKEODA SET AD = 'İDARE MERKEZİ GN.MD.', USTKOD = 1 WHERE GORKODESKI LIKE 'İF0700A22'

        //            //                UPDATE YERLESKEODA SET AD = 'KONTROLÖRLER', USTKOD = 1 WHERE GORKODESKI LIKE 'İF0700A23'

        //            //                UPDATE YERLESKEODA SET AD = 'DENETİM GN.MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İF192K20'
        //            //                UPDATE YERLESKEODA SET AD = 'DENETİM ELEMANLARI', USTKOD = 1466 WHERE GORKODESKI LIKE 'İF192K21'
        //            //                UPDATE YERLESKEODA SET AD = 'DENETİM ELEMAN. F BLOK', USTKOD = 1466 WHERE GORKODESKI LIKE 'İF192K24'

        //            //                UPDATE YERLESKEODA SET AD = 'ARAŞ.VE PAR.PL.GN.MD', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA1015A50'
        //            //                UPDATE YERLESKEODA SET AD = 'ARAŞTIRMA GRUBU', USTKOD = 1469 WHERE GORKODESKI LIKE 'İA1015A51'
        //            //                UPDATE YERLESKEODA SET AD = 'PARA POL.DEST.HZM.MD', USTKOD = 1469 WHERE GORKODESKI LIKE 'İA1015A52'
        //            //                UPDATE YERLESKEODA SET AD = 'ARAŞTIRMA GRUBU 2', USTKOD = 1469 WHERE GORKODESKI LIKE 'İF104A53'
        //            //                UPDATE YERLESKEODA SET AD = 'KÜTÜPHANE BİRİMİ', USTKOD = 1469 WHERE GORKODESKI LIKE 'İA1015A54'

        //            //                UPDATE YERLESKEODA SET AD = 'YAPISAL EKO.ARAŞ.GN.MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA1015A55'
        //            //                UPDATE YERLESKEODA SET AD = 'YAPISAL EKO.ARŞ.G.M. F BLOK', USTKOD = 1474 WHERE GORKODESKI LIKE 'İF1015A56'

        //            //                UPDATE YERLESKEODA SET AD = 'BANK.VE FİN KR.GN MD', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA1114B00'
        //            //                UPDATE YERLESKEODA SET AD = 'ULUS.KUR.VE DÜZN.MD.', USTKOD = 1476 WHERE GORKODESKI LIKE 'İA1114B01'
        //            //                UPDATE YERLESKEODA SET AD = 'FİNS.ARÇ.VE DÜZN.MD.', USTKOD = 1476 WHERE GORKODESKI LIKE 'İA1114B02'
        //            //                UPDATE YERLESKEODA SET AD = 'FİNANSAL İSTİKRAR MD', USTKOD = 1476 WHERE GORKODESKI LIKE 'İA1114B03'
        //            //                UPDATE YERLESKEODA SET AD = 'MAKRO FİNANS.ANLZ.MD', USTKOD = 1476 WHERE GORKODESKI LIKE 'İA1114B05'
        //            //                UPDATE YERLESKEODA SET AD = 'KAMBİYO MEVZUATI MD.', USTKOD = 1476 WHERE GORKODESKI LIKE 'İA1114B06'

        //            //                UPDATE YERLESKEODA SET AD = 'BİLİŞİM TEKN. GN.MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA125B10'
        //            //                UPDATE YERLESKEODA SET AD = 'İŞ BİRİMİ İLİŞKİLERİ YÖN.MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA125B11'
        //            //                UPDATE YERLESKEODA SET AD = 'BİLİŞİM TEKN.YÖNETİŞİM MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA125B12'
        //            //                UPDATE YERLESKEODA SET AD = 'BİLŞ.TEK.ALTYAPI MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA1213B13'
        //            //                UPDATE YERLESKEODA SET AD = 'BİLİŞİM TEKN.YARDIM MASASI MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA122BB14'
        //            //                UPDATE YERLESKEODA SET AD = 'İŞ ZEKASI VE VERİ YÖNETİMİ MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA1213B15'
        //            //                UPDATE YERLESKEODA SET AD = 'BİLİŞİM TEKN.GÜVENLİK MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA1213B16'
        //            //                UPDATE YERLESKEODA SET AD = '1. UYGUL.VE KABİL.MRK.MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA125B17'
        //            //                UPDATE YERLESKEODA SET AD = '2. UYGUL.VE KABİL.MRK.MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA125B18'
        //            //                UPDATE YERLESKEODA SET AD = '3. UYGUL.VE KABİL.MRK.MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA125B19'
        //            //                UPDATE YERLESKEODA SET AD = '4. UYGUL.VE KABİL.MRK.MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA125B22'
        //            //                UPDATE YERLESKEODA SET AD = 'UYGULAMA BAKIM VE DESTEK MD', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA125B23'
        //            //                UPDATE YERLESKEODA SET AD = 'KULLANICI DONANIMLARI MD.', USTKOD = 1482 WHERE GORKODESKI LIKE 'İA122BB24'
        //            //                UPDATE YERLESKEODA SET AD = 'OLAĞANÜSTÜ DY.MRKZ', USTKOD = 1482 WHERE GORKODESKI LIKE 'İBM1200B20'
        //            //                UPDATE YERLESKEODA SET AD = 'BİLİŞİM DEPOSU', USTKOD = 1482 WHERE GORKODESKI LIKE 'İE122BB21'

        //            //                UPDATE YERLESKEODA SET AD = 'EMİSYON GENEL MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA13ZE00'
        //            //                UPDATE YERLESKEODA SET AD = 'BANKNOT PLANLAMA MD.', USTKOD = 1497 WHERE GORKODESKI LIKE 'İA13ZE01'
        //            //                UPDATE YERLESKEODA SET AD = 'BANKNOT HAREKET.MD.', USTKOD = 1497 WHERE GORKODESKI LIKE 'İA13ZE02'
        //            //                UPDATE YERLESKEODA SET AD = 'MÜZE', USTKOD = 1497 WHERE GORKODESKI LIKE 'İA13ZE04'
        //            //                UPDATE YERLESKEODA SET AD = 'BANKNOT TEKNOLOJİLERİ MD.', USTKOD = 1497 WHERE GORKODESKI LIKE 'İA13ZE05'

        //            //                UPDATE YERLESKEODA SET AD = 'HUKUK GENEL MÜDÜRLÜĞÜ', USTKOD = 1 WHERE GORKODESKI LIKE 'İA14ZH00'

        //            //                UPDATE YERLESKEODA SET AD = 'DESTEK HİZMETLERİ GN. MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA167I10'
        //            //                UPDATE YERLESKEODA SET AD = 'SÖZ.YÖN.VE OPER.MD.', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA167I11'
        //            //                UPDATE YERLESKEODA SET AD = 'İNŞ. VE EMLAK İŞL.MD', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA167I12'
        //            //                UPDATE YERLESKEODA SET AD = 'SATIN ALMA PLAN.VE YÖN.MD.', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA167I13'
        //            //                UPDATE YERLESKEODA SET AD = 'İDARİ VE SOSYAL İŞL.MD.', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA16ZI15'
        //            //                UPDATE YERLESKEODA SET AD = 'TEKNİK İŞLETİM MD.', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA162BI18'
        //            //                UPDATE YERLESKEODA SET AD = 'GÜVENLİK MÜDÜRLÜĞÜ', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA16ZI14'
        //            //                UPDATE YERLESKEODA SET AD = 'SİV.SAV.VE SEFER.MD.', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA16ZI22'
        //            //                UPDATE YERLESKEODA SET AD = 'HABERLEŞME MÜDÜRLÜĞÜ', USTKOD = 1503 WHERE GORKODESKI LIKE 'İE16ZI23'
        //            //                UPDATE YERLESKEODA SET AD = 'KAHRAMANKAZAN ARŞİVİ', USTKOD = 1503 WHERE GORKODESKI LIKE 'İK167I24'
        //            //                UPDATE YERLESKEODA SET AD = 'GIDA İŞLERİ SERVİSİ', USTKOD = 1503 WHERE GORKODESKI LIKE 'İD241S11'
        //            //                UPDATE YERLESKEODA SET AD = 'SAĞLIK MERKEZİ', USTKOD = 1503 WHERE GORKODESKI LIKE 'İD241S12'
        //            //                UPDATE YERLESKEODA SET AD = 'MALZEME DEPOSU', USTKOD = 1503 WHERE GORKODESKI LIKE 'İE162BI16'
        //            //                UPDATE YERLESKEODA SET AD = 'DEMİRBAŞ MÜZESİ', USTKOD = 1503 WHERE GORKODESKI LIKE 'İE162BI17'
        //            //                UPDATE YERLESKEODA SET AD = 'SAĞLIK SERVİSİ', USTKOD = 1503 WHERE GORKODESKI LIKE 'İE24ZS15'
        //            //                UPDATE YERLESKEODA SET AD = 'ANKARA SOSYAL TESİSİ', USTKOD = 1503 WHERE GORKODESKI LIKE 'SAS2400S13'
        //            //                UPDATE YERLESKEODA SET AD = 'İZMİR ÖZDERE KAMPI', USTKOD = 1503 WHERE GORKODESKI LIKE 'KİK2400S21'
        //            //                UPDATE YERLESKEODA SET AD = '17. KAT RUF', USTKOD = 1503 WHERE GORKODESKI LIKE 'İ A24 17S19'
        //            //                UPDATE YERLESKEODA SET AD = 'K.KAZAN MALZEME DEPOSU', USTKOD = 1503 WHERE GORKODESKI LIKE 'İE0707I20'
        //            //                UPDATE YERLESKEODA SET AD = 'BULUNAMAYANLAR', USTKOD = 1503 WHERE GORKODESKI LIKE 'İA0707İ07'

        //            //                UPDATE YERLESKEODA SET AD = 'İSTATİSTİK GENEL MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA179I30'
        //            //                UPDATE YERLESKEODA SET AD = 'PARASAL FİN.VERİ.MD.', USTKOD = 1523 WHERE GORKODESKI LIKE 'İA179I31'
        //            //                UPDATE YERLESKEODA SET AD = 'REEL SEKTÖR VER.MD.', USTKOD = 1523 WHERE GORKODESKI LIKE 'İA179I32'
        //            //                UPDATE YERLESKEODA SET AD = 'ÖDEMELER DENGESİ MD.', USTKOD = 1523 WHERE GORKODESKI LIKE 'İA179I33'
        //            //                UPDATE YERLESKEODA SET AD = 'VERİ YÖNETİMİ MÜDÜRLÜĞÜ', USTKOD = 1523 WHERE GORKODESKI LIKE 'İA179I34'

        //            //                UPDATE YERLESKEODA SET AD = 'BÜTÇE VE FİNAN.RAPORLAMA G.M.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA206M00'
        //            //                UPDATE YERLESKEODA SET AD = 'MUHASEBE İŞLEMLERİ', USTKOD = 1528 WHERE GORKODESKI LIKE 'İA20ZM01'
        //            //                UPDATE YERLESKEODA SET AD = 'KAMBİYO MUHASEBESİ ', USTKOD = 1528 WHERE GORKODESKI LIKE 'İA206M02'
        //            //                UPDATE YERLESKEODA SET AD = 'FİNANSAL RAPORLAMA MD.', USTKOD = 1528 WHERE GORKODESKI LIKE 'İA206M03'
        //            //                UPDATE YERLESKEODA SET AD = 'BÜTÇE VE PLAN GN.MD.YRD.', USTKOD = 1528 WHERE GORKODESKI LIKE 'İA206M04'
        //            //                UPDATE YERLESKEODA SET AD = 'VERGİ UYGULAMALARI MD.', USTKOD = 1528 WHERE GORKODESKI LIKE 'İA206M05'

        //            //                UPDATE YERLESKEODA SET AD = 'PİYASALAR GENEL MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA2112P00'
        //            //                UPDATE YERLESKEODA SET AD = 'TL PİYASALARI MÜDÜRLÜĞÜ', USTKOD = 1534 WHERE GORKODESKI LIKE 'İA2112P01'
        //            //                UPDATE YERLESKEODA SET AD = 'PİYASA OPERASYONLARI MD', USTKOD = 1534 WHERE GORKODESKI LIKE 'İA2112P02'
        //            //                UPDATE YERLESKEODA SET AD = 'DÖVİZ PİYASALARI MÜDÜRL.', USTKOD = 1534 WHERE GORKODESKI LIKE 'İA2112P05'
        //            //                UPDATE YERLESKEODA SET AD = 'REZERV YÖNETİMİ MÜDÜRL.', USTKOD = 1534 WHERE GORKODESKI LIKE 'İA2112P06'

        //            //                UPDATE YERLESKEODA SET AD = 'İNSAN KAYN.GN.MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA228P10'
        //            //                UPDATE YERLESKEODA SET AD = 'ÖZLÜK İŞLERİ MÜDÜRLÜĞÜ', USTKOD = 1539 WHERE GORKODESKI LIKE 'İA228P11'
        //            //                UPDATE YERLESKEODA SET AD = 'İŞE ALIM VE PLANLAMA MD.', USTKOD = 1539 WHERE GORKODESKI LIKE 'İA228P12'
        //            //                UPDATE YERLESKEODA SET AD = 'EĞİTİM MÜDÜRLÜĞÜ', USTKOD = 1539 WHERE GORKODESKI LIKE 'İE228P14'
        //            //                UPDATE YERLESKEODA SET AD = 'EĞİTİM BİNASI', USTKOD = 1539 WHERE GORKODESKI LIKE 'İEM2200P15'
        //            //                UPDATE YERLESKEODA SET AD = 'PERFORMANS VE KARİYER MD.', USTKOD = 1539 WHERE GORKODESKI LIKE 'İA228P16'
        //            //                UPDATE YERLESKEODA SET AD = 'İŞ SAĞLIĞI VE GÜVENLİĞİ MD', USTKOD = 1539 WHERE GORKODESKI LIKE 'İA228P17'
        //            //                UPDATE YERLESKEODA SET AD = 'BİLGİSAYAR SINIFI', USTKOD = 1539 WHERE GORKODESKI LIKE 'İG224P18'

        //            //                UPDATE YERLESKEODA SET AD = 'ÖDEME SİSTEM.GEN.MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA2317Ö00'
        //            //                UPDATE YERLESKEODA SET AD = 'ÖDEME SİSTEMLERİ İŞ GELİŞ.MD.', USTKOD = 1547 WHERE GORKODESKI LIKE 'İA2317Ö01'
        //            //                UPDATE YERLESKEODA SET AD = 'DÜZENL.VE GÖZET.MD.', USTKOD = 1547 WHERE GORKODESKI LIKE 'İA2317Ö02'
        //            //                UPDATE YERLESKEODA SET AD = 'ÖDEME İŞLEMLERİ MD.', USTKOD = 1547 WHERE GORKODESKI LIKE 'İA2317Ö03'
        //            //                UPDATE YERLESKEODA SET AD = 'ÖDEME SİST.İŞLT.MD.', USTKOD = 1547 WHERE GORKODESKI LIKE 'İA2317Ö04'

        //            //                UPDATE YERLESKEODA SET AD = 'OPERASYON GENEL MÜDÜRLÜĞÜ', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İG153I00'
        //            //                UPDATE YERLESKEODA SET AD = 'İŞÇİ DÖVİZLERİ OPERASYON.MD.', USTKOD = 1552 WHERE GORKODESKI LIKE 'İG154I01'
        //            //                UPDATE YERLESKEODA SET AD = 'HAZİNE VE BANK.DEST.OPERA.MD.', USTKOD = 1552 WHERE GORKODESKI LIKE 'İG154I05'

        //            //                UPDATE YERLESKEODA SET AD = 'KURUMSAL DÖNÜŞÜM KOORD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA2613K10'
        //            //                UPDATE YERLESKEODA SET AD = 'KÜLTÜREL DÖNÜŞÜM MD.', USTKOD = 1555 WHERE GORKODESKI LIKE 'İA2613K11'
        //            //                UPDATE YERLESKEODA SET AD = 'ORGAN.VE SÜREÇ YÖN.MD.', USTKOD = 1555 WHERE GORKODESKI LIKE 'İA2613K12'
        //            //                UPDATE YERLESKEODA SET AD = 'PROGRAM İZLEME MÜDÜRLÜĞÜ', USTKOD = 1555 WHERE GORKODESKI LIKE 'İA2613K13'

        //            //                UPDATE YERLESKEODA SET AD = 'KURUMSAL RİSK YÖNETİMİ G.M.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İA2712K40'
        //            //                UPDATE YERLESKEODA SET AD = 'FİNANSAL RİSKLER YÖNETİMİ MD.', USTKOD = 1559 WHERE GORKODESKI LIKE 'İA2712K41'
        //            //                UPDATE YERLESKEODA SET AD = 'MEVZUATA UYUM MD.', USTKOD = 1559 WHERE GORKODESKI LIKE 'İA2712K42'
        //            //                UPDATE YERLESKEODA SET AD = 'OPERASYONEL RİSK YÖN.MD.', USTKOD = 1559 WHERE GORKODESKI LIKE 'İA2712K43'

        //            //                UPDATE YERLESKEODA SET AD = 'İST.MRK.BANK.AR.EĞ.MRK.GN.MD.', USTKOD = 1, ALTDALVAR = 1 WHERE GORKODESKI LIKE 'İE0016U10'
        //            //                UPDATE YERLESKEODA SET AD = 'İ.M.B.A.E.M(İD.MRK.)', USTKOD = 1563 WHERE GORKODESKI LIKE 'İE0016U11'
        //            //                UPDATE YERLESKEODA SET AD = 'F.BAHÇE KORU SİT.MİS', USTKOD = 1563 WHERE GORKODESKI LIKE 'LDLBK00W25'

        //            //                UPDATE YERLESKEODA SET AD = 'ULUGÖL PLAZA YERLEŞKESİ', USTKOD = 1 WHERE GORKODESKI LIKE 'İU2500U20'

        //            //                UPDATE YERLESKEODA SET AD = 'TEMSİLCİLİKLER', ALTDALVAR = 1, USTKOD = 1 WHERE KOD = 28
        //            //                UPDATE YERLESKEODA SET AD = 'LONDRA TEMSİLCİLİĞİ', USTKOD = 28 WHERE GORKODESKI LIKE 'TLT0000X10'
        //            //                UPDATE YERLESKEODA SET AD = 'NEW YORK TEMSİLCİLİĞ', USTKOD = 28 WHERE GORKODESKI LIKE 'TNT0000X20'
        //            //                UPDATE YERLESKEODA SET AD = 'FRANKFURT TEMSİLCİLİ', USTKOD = 28 WHERE GORKODESKI LIKE 'TFT0000X60'
        //            //                UPDATE YERLESKEODA SET AD = 'TOKYO TEMSİLCİLİĞİ', USTKOD = 28 WHERE GORKODESKI LIKE 'TTT0000X70'
        //            //                UPDATE YERLESKEODA SET AD = 'WASHINGTON BÜROSU', USTKOD = 28 WHERE GORKODESKI LIKE 'TBR0000X71'
        //            //                UPDATE YERLESKEODA SET AD = 'MOSKOVA BÜROSU', USTKOD = 28 WHERE GORKODESKI LIKE 'TBR0000X72'
        //            //                UPDATE YERLESKEODA SET AD = 'SİDNEY BÜROSU', USTKOD = 28 WHERE GORKODESKI LIKE 'TBR0000X73'
        //            //                UPDATE YERLESKEODA SET AD = 'PEKİN BÜROSU', USTKOD = 28 WHERE GORKODESKI LIKE 'TBR0000X74'
        //            //                UPDATE YERLESKEODA SET AD = 'OECD BÜROSU', USTKOD = 28 WHERE GORKODESKI LIKE 'TBR0000X75'
        //            //                UPDATE YERLESKEODA SET AD = 'DOHA BÜROSU', USTKOD = 28 WHERE GORKODESKI LIKE 'TBR0000X76'

        //            //                UPDATE YERLESKEODA SET AD = 'MİSAFİRHANELER', ALTDALVAR = 1, USTKOD = 1 WHERE KOD = 795
        //            //                UPDATE YERLESKEODA SET AD = 'ÖZLEM APT.', USTKOD = 795 WHERE GORKODESKI LIKE 'LDLÖA00W40'
        //            //                UPDATE YERLESKEODA SET AD = 'ÖZLEM APT.MİSAFİRHAN', USTKOD = 795 WHERE GORKODESKI LIKE 'LDLÖA00W401'

        //            //                UPDATE YERLESKEODA SET AD = 'LOJMANLAR', ALTDALVAR = 1, USTKOD = 1 WHERE KOD = 796
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPK00W00'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK B1/7', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKB1W017'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK B1/10', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKB1W0210'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK B1/13', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKB1W0313'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK B1/26', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKB1W0426'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK B1/28', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKB1W0528'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK B1/30', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKB1W0630'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK C1/28', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKC1W0728'
        //            //                UPDATE YERLESKEODA SET AD = 'PEMBE KÖŞK C1/14', USTKOD = 796 WHERE GORKODESKI LIKE 'LDLPKC1W0814'";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Yerleşke tablosunun gönderilen listeye göre düzenlenmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 8)
        //            //    {
        //            //        sql = @"ALTER TABLE dbo.TASISLEMFORM ADD MUTEMMIM int NULL";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASISLEMFORM tablosuna MUTEMMIM alanının eklenmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 9)
        //            //    {
        //            //        sql = @"DELETE FROM YERLESKEODA WHERE KOD = 4289

        //            //                UPDATE TASSICILNOHAREKET SET NEREYEGITTI = 4288 WHERE NEREYEGITTI = '4289'
        //            //                UPDATE TASSICILNOOZELLIK SET BULUNDUGUYER = 4288 WHERE BULUNDUGUYER = '4289'
        //            //                UPDATE TASISLEMOZELLIK SET BULUNDUGUYER = 4288 WHERE BULUNDUGUYER = '4289'
        //            //                ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Fazladan açılan ve kullanılmayan 4289 SPOR SALONU yerleşkesinin silinmesi talebi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 10)
        //            //    {
        //            //        sql = @"UPDATE KULLANICI SET AKTIFMI = 1
        //            //                UPDATE PERPERSONEL SET DURUM = 1, AKTIFMI = 1";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Bütün kullanıcıların aktif edilmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 11)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET SET AMBARKOD = '51', HESAPPLANKOD = '0940101'
        //            //                WHERE PRSICILNO IN(112742, 112743, 112744, 112745, 112746, 112747, 112748)";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Talep üzerine istanbul şube için, demirbaş olarak kaydedilen 7 kayıt yazılım olarak değiştiridi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 12)
        //            //    {
        //            //        sql = @"CREATE TABLE [dbo].[TASAYARLARMB](
        //            //             [MUHASEBEKOD] [varchar](5) NOT NULL,
        //            //             [HARCAMABIRIMKOD] [varchar](11) NOT NULL,
        //            //             [AMBARKOD] [varchar](2) NOT NULL,
        //            //             [ISLEMYAPAN] [varchar](255) NULL,
        //            //             [ISLEMTARIH] [datetime] NULL,
        //            //             [FISGIRISKAPANISTARIH] [datetime] NULL,
        //            //             [MUTEMMIMBITISTARIH] [datetime] NULL,
        //            //             CONSTRAINT [PK_TASAYARLARMB] PRIMARY KEY CLUSTERED 
        //            //            (
        //            //             [MUHASEBEKOD] ASC,
        //            //             [HARCAMABIRIMKOD] ASC,
        //            //             [AMBARKOD] ASC
        //            //            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        //            //            ) ON [PRIMARY]";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASAYARLARMB tablosunun oluşturulması.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 13)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET SET SICILNO = PRSICILNO
        //            //                WHERE YIL = 2018
        //            //                AND PRSICILNO > 158000
        //            //                AND PRSICILNO != SICILNO
        //            //                AND ISLEMTARIH >= '2018-12-12'";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "SICILNO düzeltme güncellemesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 14)
        //            //    {
        //            //        sql = @"declare @t table(YIL int, MUHASEBEKOD varchar(50), HARCAMABIRIMKOD varchar(50), FISNO varchar(50))
        //            //                insert into @t SELECT TASISLEMFORM.YIL, TASISLEMFORM.MUHASEBEKOD, TASISLEMFORM.HARCAMABIRIMKOD, TASISLEMFORM.FISNO FROM TASISLEMFORM INNER JOIN
        //            //                (SELECT YIL, MUHASEBEKOD, HARCAMABIRIMKOD, FISNO
        //            //                FROM TASSICILNOHAREKET
        //            //                WHERE (ISLEMTIPIKOD = 5) AND (DURUM = 2)
        //            //                GROUP BY YIL, MUHASEBEKOD, HARCAMABIRIMKOD, FISNO) AS H ON H.YIL = TASISLEMFORM.YIL AND H.MUHASEBEKOD = TASISLEMFORM.MUHASEBEKOD AND 
        //            //                H.HARCAMABIRIMKOD = TASISLEMFORM.HARCAMABIRIMKOD AND H.FISNO = TASISLEMFORM.FISNO
        //            //                WHERE TASISLEMFORM.ONAYDURUM = 0

        //            //                DELETE FROM TASISLEMOZELLIK
        //            //                FROM TASISLEMOZELLIK INNER JOIN
        //            //                @t AS H ON H.YIL = TASISLEMOZELLIK.YIL AND H.MUHASEBEKOD = TASISLEMOZELLIK.MUHASEBEKOD AND 
        //            //                H.HARCAMABIRIMKOD = TASISLEMOZELLIK.HARCAMABIRIMKOD AND H.FISNO = TASISLEMOZELLIK.FISNO

        //            //                DELETE FROM TASISLEMFORMTARIHCE
        //            //                FROM            TASISLEMFORMTARIHCE INNER JOIN
        //            //                @t AS H ON H.YIL = TASISLEMFORMTARIHCE.YIL AND H.MUHASEBEKOD = TASISLEMFORMTARIHCE.MUHASEBEKOD AND 
        //            //                H.HARCAMABIRIMKOD = TASISLEMFORMTARIHCE.HARCAMABIRIMKOD AND H.FISNO = TASISLEMFORMTARIHCE.FISNO

        //            //                DELETE FROM TASISLEMDETAY
        //            //                FROM            TASISLEMDETAY INNER JOIN
        //            //                @t AS H ON H.YIL = TASISLEMDETAY.YIL AND H.MUHASEBEKOD = TASISLEMDETAY.MUHASEBEKOD AND 
        //            //                H.HARCAMABIRIMKOD = TASISLEMDETAY.HARCAMABIRIMKOD AND H.FISNO = TASISLEMDETAY.FISNO

        //            //                DELETE FROM TASISLEMFORM
        //            //                FROM            TASISLEMFORM INNER JOIN
        //            //                @t AS H ON H.YIL = TASISLEMFORM.YIL AND H.MUHASEBEKOD = TASISLEMFORM.MUHASEBEKOD AND 
        //            //                H.HARCAMABIRIMKOD = TASISLEMFORM.HARCAMABIRIMKOD AND H.FISNO = TASISLEMFORM.FISNO

        //            //                DELETE FROM TASSICILNOHAREKET WHERE ISLEMTIPIKOD = 5 AND DURUM = 2 ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Devir Giriş Sorunu düzeltme güncellemesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //        else
        //            //        {
        //            //            aciklama = "Devir Giriş Sorunu düzeltme güncellemesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + -10000 * versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(ve.hata) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 15)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET SET DURUM = 2
        //            //                FROM TASSICILNOHAREKET INNER JOIN
        //            //                TASSICILNOHAREKET AS H2 ON H2.PRSICILNO = TASSICILNOHAREKET.PRSICILNO AND H2.DURUM = 2
        //            //                WHERE (TASSICILNOHAREKET.DURUM = 1)";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "İptal edilen kayıtların kontrol işlemi";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 16)
        //            //    {
        //            //        sql = @"ALTER TABLE DUYURU ALTER COLUMN DUYURU varchar(5000) NULL";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Duyuru girişi alanı için karakter uzunluğunun artırılması.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 17)
        //            //    {
        //            //        sql = @"CREATE TABLE [dbo].[TASHARMONI](
        //            //                 [SICILNO] [varchar](30) NOT NULL,
        //            //                 [TIP] [int] NOT NULL,
        //            //                 [ACIKLAMA] [varchar](500) NULL,
        //            //                 [ISLEMTARIH] [datetime] NULL,
        //            //                 CONSTRAINT [PK_TASHARMONI] PRIMARY KEY CLUSTERED 
        //            //                (
        //            //                 [SICILNO] ASC,
        //            //                 [TIP] ASC
        //            //                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        //            //                ) ON [PRIMARY]";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Harmoni tablosunun oluşturulması.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 18)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET
        //            //                SET FIYAT = H2.FIYAT, KDVLIBIRIMFIYAT = H2.KDVLIBIRIMFIYAT, DEGERARTISI = H2.DEGERARTISI
        //            //                FROM TASSICILNOHAREKET INNER JOIN TASSICILNOHAREKET AS H2 ON TASSICILNOHAREKET.PRSICILNO = H2.PRSICILNO AND H2.SIRANO = 1
        //            //                WHERE (TASSICILNOHAREKET.SIRANO > 1)";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Zimmet sırasında toplam değerin ilk bedel ile değer artış fiyatının toplanmasından kaynaklanan sorunun düzeltilmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 19)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET
        //            //                SET FIYAT = H2.FIYAT, KDVLIBIRIMFIYAT = H2.KDVLIBIRIMFIYAT, DEGERARTISI = H2.DEGERARTISI
        //            //                FROM TASSICILNOHAREKET INNER JOIN TASSICILNOHAREKET AS H2 ON TASSICILNOHAREKET.PRSICILNO = H2.PRSICILNO AND H2.SIRANO = 1
        //            //                WHERE (TASSICILNOHAREKET.SIRANO > 1)";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Yeniden Zimmet sırasında toplam değerin ilk bedel ile değer artış fiyatının toplanmasından kaynaklanan sorunun düzeltilmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 20)
        //            //    {
        //            //        sql = @"ALTER VIEW [dbo].[TASODA]
        //            //                AS
        //            //                SELECT '00001' AS MUHASEBEKOD, BIRIMKOD AS HARCAMABIRIMKOD, KOD, AD, '' AS AMBARKOD, USTKOD, SEVIYE, KOD AS UKOD, DURUM, MALIYETMERKEZI, 
        //            //                GORKODESKI AS ESKIKOD
        //            //                FROM dbo.YERLESKEODA";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASODA view de  CAST(KOD AS varchar) AS KOD ifadesi KOD olarak değiştirildi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }


        //            //    if (versiyon == 21)
        //            //    {
        //            //        sql = @"/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
        //            //                BEGIN TRANSACTION
        //            //                SET QUOTED_IDENTIFIER ON
        //            //                SET ARITHABORT ON
        //            //                SET NUMERIC_ROUNDABORT OFF
        //            //                SET CONCAT_NULL_YIELDS_NULL ON
        //            //                SET ANSI_NULLS ON
        //            //                SET ANSI_PADDING ON
        //            //                SET ANSI_WARNINGS ON
        //            //                COMMIT
        //            //                BEGIN TRANSACTION

        //            //                CREATE TABLE dbo.Tmp_YERLESKEODA
        //            //                 (
        //            //                 KOD varchar(6) NOT NULL,
        //            //                 AD varchar(200) NULL,
        //            //                 GORKOD varchar(50) NULL,
        //            //                 TARIF varchar(2000) NULL,
        //            //                 SAYMANLIKKOD int NULL,
        //            //                 BIRIMKOD varchar(3) NULL,
        //            //                 ALTDALVAR int NULL,
        //            //                 USTKOD int NULL,
        //            //                 YERTURKOD int NULL,
        //            //                 ODATURKOD int NULL,
        //            //                 ILKOD varchar(20) NULL,
        //            //                 ORGKOD int NULL,
        //            //                 SUBE varchar(20) NULL,
        //            //                 YERLESKE varchar(20) NULL,
        //            //                 BLOK varchar(20) NULL,
        //            //                 BIRIM varchar(20) NULL,
        //            //                 KAT varchar(20) NULL,
        //            //                 MUDURLUK varchar(20) NULL,
        //            //                 MEKAN varchar(20) NULL,
        //            //                 SEVIYE int NULL,
        //            //                 DURUM int NULL,
        //            //                 MALIYETMERKEZI int NULL,
        //            //                 GORKODESKI varchar(50) NULL
        //            //                 )  ON [PRIMARY]

        //            //                ALTER TABLE dbo.Tmp_YERLESKEODA SET (LOCK_ESCALATION = TABLE)

        //            //                IF EXISTS(SELECT * FROM dbo.YERLESKEODA)
        //            //                  EXEC('INSERT INTO dbo.Tmp_YERLESKEODA (KOD, AD, GORKOD, TARIF, SAYMANLIKKOD, BIRIMKOD, ALTDALVAR, USTKOD, YERTURKOD, ODATURKOD, ILKOD, ORGKOD, SUBE, YERLESKE, BLOK, BIRIM, KAT, MUDURLUK, MEKAN, SEVIYE, DURUM, MALIYETMERKEZI, GORKODESKI)
        //            //                  SELECT CONVERT(varchar(6), KOD), AD, GORKOD, TARIF, SAYMANLIKKOD, BIRIMKOD, ALTDALVAR, USTKOD, YERTURKOD, ODATURKOD, ILKOD, ORGKOD, SUBE, YERLESKE, BLOK, BIRIM, KAT, MUDURLUK, MEKAN, SEVIYE, DURUM, MALIYETMERKEZI, GORKODESKI FROM dbo.YERLESKEODA WITH (HOLDLOCK TABLOCKX)')

        //            //                DROP TABLE dbo.YERLESKEODA

        //            //                EXECUTE sp_rename N'dbo.Tmp_YERLESKEODA', N'YERLESKEODA', 'OBJECT' 

        //            //                ALTER TABLE dbo.YERLESKEODA ADD CONSTRAINT
        //            //                 PK_YERLESKEODA PRIMARY KEY CLUSTERED 
        //            //                 (
        //            //                 KOD
        //            //                 ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


        //            //                CREATE NONCLUSTERED INDEX IX_YERLESKEODA ON dbo.YERLESKEODA
        //            //                 (
        //            //                 GORKOD
        //            //                 ) INCLUDE (AD) 
        //            //                 WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        //            //                COMMIT";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "YERLESKEODA tablosunun KOD alanın tipi int den varchar(6) olacak şekilde değiştirildi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 22)
        //            //    {
        //            //        sql = @"/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
        //            //                BEGIN TRANSACTION
        //            //                SET QUOTED_IDENTIFIER ON
        //            //                SET ARITHABORT ON
        //            //                SET NUMERIC_ROUNDABORT OFF
        //            //                SET CONCAT_NULL_YIELDS_NULL ON
        //            //                SET ANSI_NULLS ON
        //            //                SET ANSI_PADDING ON
        //            //                SET ANSI_WARNINGS ON
        //            //                COMMIT
        //            //                BEGIN TRANSACTION

        //            //                CREATE TABLE dbo.Tmp_YERLESKEODA
        //            //                 (
        //            //                 KOD int NOT NULL,
        //            //                 AD varchar(200) NULL,
        //            //                 GORKOD varchar(50) NULL,
        //            //                 TARIF varchar(2000) NULL,
        //            //                 SAYMANLIKKOD int NULL,
        //            //                 BIRIMKOD varchar(3) NULL,
        //            //                 ALTDALVAR int NULL,
        //            //                 USTKOD int NULL,
        //            //                 YERTURKOD int NULL,
        //            //                 ODATURKOD int NULL,
        //            //                 ILKOD varchar(20) NULL,
        //            //                 ORGKOD int NULL,
        //            //                 SUBE varchar(20) NULL,
        //            //                 YERLESKE varchar(20) NULL,
        //            //                 BLOK varchar(20) NULL,
        //            //                 BIRIM varchar(20) NULL,
        //            //                 KAT varchar(20) NULL,
        //            //                 MUDURLUK varchar(20) NULL,
        //            //                 MEKAN varchar(20) NULL,
        //            //                 SEVIYE int NULL,
        //            //                 DURUM int NULL,
        //            //                 MALIYETMERKEZI int NULL,
        //            //                 GORKODESKI varchar(50) NULL
        //            //                 )  ON [PRIMARY]

        //            //                ALTER TABLE dbo.Tmp_YERLESKEODA SET (LOCK_ESCALATION = TABLE)

        //            //                IF EXISTS(SELECT * FROM dbo.YERLESKEODA)
        //            //                  EXEC('INSERT INTO dbo.Tmp_YERLESKEODA (KOD, AD, GORKOD, TARIF, SAYMANLIKKOD, BIRIMKOD, ALTDALVAR, USTKOD, YERTURKOD, ODATURKOD, ILKOD, ORGKOD, SUBE, YERLESKE, BLOK, BIRIM, KAT, MUDURLUK, MEKAN, SEVIYE, DURUM, MALIYETMERKEZI, GORKODESKI)
        //            //                  SELECT CONVERT(varchar(6), KOD), AD, GORKOD, TARIF, SAYMANLIKKOD, BIRIMKOD, ALTDALVAR, USTKOD, YERTURKOD, ODATURKOD, ILKOD, ORGKOD, SUBE, YERLESKE, BLOK, BIRIM, KAT, MUDURLUK, MEKAN, SEVIYE, DURUM, MALIYETMERKEZI, GORKODESKI FROM dbo.YERLESKEODA WITH (HOLDLOCK TABLOCKX)')

        //            //                DROP TABLE dbo.YERLESKEODA

        //            //                EXECUTE sp_rename N'dbo.Tmp_YERLESKEODA', N'YERLESKEODA', 'OBJECT' 

        //            //                ALTER TABLE dbo.YERLESKEODA ADD CONSTRAINT
        //            //                 PK_YERLESKEODA PRIMARY KEY CLUSTERED 
        //            //                 (
        //            //                 KOD
        //            //                 ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


        //            //                CREATE NONCLUSTERED INDEX IX_YERLESKEODA ON dbo.YERLESKEODA
        //            //                 (
        //            //                 GORKOD
        //            //                 ) INCLUDE (AD) 
        //            //                 WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

        //            //                COMMIT";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "YERLESKEODA tablosunun KOD alanın tipi varchar(6) dan int olacak şekilde değiştirildi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 23)
        //            //    {
        //            //        sql = @"CREATE VIEW[dbo].[TASDEVIRGIRISYAPILMAYANLARVIEW]
        //            //                AS SELECT A.PRSICILNO, A.SIRANO, A.YIL, A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.FISNO, A.FISSIRANO, A.SICILNO, A.SICILSIRANO, A.HESAPPLANKOD, A.AMBARKOD, 
        //            //                CONVERT(DECIMAL(21, 12), A.FIYAT * ((A.KDVORAN + 0.0) / 100 + 1) + A.DEGERARTISI) / CONVERT(DECIMAL(21, 12), (A.KDVORAN + 0.0) / 100 + 1) AS FIYAT, 
        //            //                A.KDVORAN, A.ISLEMTIPIKOD, A.ISLEMTURUKOD, 
        //            //                A.ISLEMTARIH, A.ISLEMYAPAN, A.DURUM, A.FISTARIH, A.NEREDENGELDI, A.NEREYEGITTI, A.KIMEGITTI, A.ZIMMETOZELLIK, A.KDVLIBIRIMFIYAT, A.DEGERARTISI, dbo.TASSICILNOOZELLIK.MARKAKOD, 
        //            //                dbo.TASSICILNOOZELLIK.MODELKOD, dbo.TASSICILNOOZELLIK.SASENO, dbo.TASSICILNOOZELLIK.MOTORNO, dbo.TASSICILNOOZELLIK.PLAKA, dbo.TASSICILNOOZELLIK.CILTNO, dbo.TASSICILNOOZELLIK.DIL, 
        //            //                dbo.TASSICILNOOZELLIK.YAZARADI, dbo.TASSICILNOOZELLIK.ADI, dbo.TASSICILNOOZELLIK.YAYINYERI, dbo.TASSICILNOOZELLIK.YAYINTARIHI, dbo.TASSICILNOOZELLIK.NEREDENGELDI AS OZELLIKNEREDENGELDI, 
        //            //                dbo.TASSICILNOOZELLIK.BOYUTLARI, dbo.TASSICILNOOZELLIK.SATIRSAYISI, dbo.TASSICILNOOZELLIK.YAPRAKSAYISI, dbo.TASSICILNOOZELLIK.SAYFASAYISI, dbo.TASSICILNOOZELLIK.CILTTURU, dbo.TASSICILNOOZELLIK.CESIDI, 
        //            //                dbo.TASSICILNOOZELLIK.YERIKONUSU, dbo.TASSICILNOOZELLIK.GELISTARIHI, dbo.TASSICILNOOZELLIK.NEREDEBULUNDU, dbo.TASSICILNOOZELLIK.CAGI, dbo.TASSICILNOOZELLIK.DURUMUMADDESI, 
        //            //                dbo.TASSICILNOOZELLIK.ONYUZ, dbo.TASSICILNOOZELLIK.ARKAYUZ, dbo.TASSICILNOOZELLIK.PUAN, dbo.TASSICILNOOZELLIK.FOTOGRAF, dbo.TASSICILNOOZELLIK.DISSICILNO, dbo.TASMUHASEBE.AD AS MUHASEBEAD, 
        //            //                dbo.TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, dbo.TASAMBAR.AD AS AMBARAD, dbo.TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD, dbo.TASMARKA.AD AS MARKAAD, dbo.TASMODEL.AD AS MODELAD, 
        //            //                dbo.TASPERSONEL.AD AS KISIAD, dbo.TASODA.AD AS ODAAD, dbo.TASPERSONEL.ODA, dbo.TASIL.KOD AS ILKOD, dbo.TASIL.AD AS ILAD, dbo.TASILCE.KOD AS ILCEKOD, dbo.TASILCE.AD AS ILCEAD, 
        //            //                dbo.TASOLCUBIRIM.KOD AS OLCUBIRIMKOD, dbo.TASOLCUBIRIM.AD AS OLCUBIRIMAD, dbo.TASSICILNOOZELLIK.DISSICILNO2, dbo.TASSICILNOOZELLIK.GIAI, dbo.TASSICILNOOZELLIK.BISNO, 
        //            //                dbo.TASSICILNOOZELLIK.ESKIBISNO1, dbo.TASSICILNOOZELLIK.EKNO, dbo.TASHESAPPLAN.AMORTIYIL, dbo.TASSICILNOOZELLIK.BULUNDUGUYER, '' AS BULUNDUGUYERAD
        //            //                FROM dbo.TASMARKA RIGHT OUTER JOIN
        //            //                dbo.TASSICILNOOZELLIK LEFT OUTER JOIN
        //            //                dbo.TASMODEL ON dbo.TASSICILNOOZELLIK.MARKAKOD = dbo.TASMODEL.MARKAKOD AND dbo.TASSICILNOOZELLIK.MODELKOD = dbo.TASMODEL.KOD RIGHT OUTER JOIN
        //            //                dbo.TASILCE INNER JOIN
        //            //                dbo.TASAMBAR INNER JOIN
        //            //                dbo.TASSICILNOHAREKET AS A INNER JOIN
        //            //                dbo.TASHESAPPLAN ON A.HESAPPLANKOD = dbo.TASHESAPPLAN.KOD INNER JOIN
        //            //                dbo.TASMUHASEBE ON A.MUHASEBEKOD = dbo.TASMUHASEBE.KOD INNER JOIN
        //            //                dbo.TASHARCAMABIRIM ON A.HARCAMABIRIMKOD = dbo.TASHARCAMABIRIM.KOD AND A.MUHASEBEKOD = dbo.TASHARCAMABIRIM.MUHASEBEKOD ON dbo.TASAMBAR.MUHASEBEKOD = A.MUHASEBEKOD AND
        //            //                dbo.TASAMBAR.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD AND dbo.TASAMBAR.KOD = A.AMBARKOD ON dbo.TASILCE.ILKOD = dbo.TASMUHASEBE.ILKOD AND dbo.TASILCE.KOD = dbo.TASMUHASEBE.ILCEKOD INNER JOIN
        //            //                dbo.TASIL ON dbo.TASILCE.ILKOD = dbo.TASIL.KOD INNER JOIN
        //            //                dbo.TASOLCUBIRIM ON dbo.TASHESAPPLAN.OLCUBIRIMKOD = dbo.TASOLCUBIRIM.KOD ON dbo.TASSICILNOOZELLIK.PRSICILNO = A.PRSICILNO ON dbo.TASMARKA.KOD = dbo.TASSICILNOOZELLIK.MARKAKOD LEFT OUTER JOIN
        //            //                dbo.TASPERSONEL ON A.KIMEGITTI = dbo.TASPERSONEL.KOD LEFT OUTER JOIN
        //            //                dbo.TASODA ON A.NEREYEGITTI = dbo.TASODA.KOD AND A.HARCAMABIRIMKOD = dbo.TASODA.HARCAMABIRIMKOD AND A.MUHASEBEKOD = dbo.TASODA.MUHASEBEKOD
        //            //                WHERE(A.DURUM <> 2) AND(A.PRSICILNO NOT IN  (SELECT PRSICILNO FROM      dbo.TASSICILNOHAREKET AS B
        //            //                WHERE(A.PRSICILNO = PRSICILNO) AND(A.SIRANO < SIRANO) AND(ISLEMTURUKOD <> 51))) AND(A.ISLEMTURUKOD = 51)
        //            //            ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASDEVIRGIRISYAPILMAYANLARVIEW adlı view oluşturuldu.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 24)
        //            //    {
        //            //        sql = @"ALTER VIEW[dbo].[TASDEMIRBASVIEW]  
        //            //                AS SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, '' KISIAD, '' ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASAMBARSICILVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, '' KIMEGITTI, '' NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, BULUNDUGUYERAD FROM TASAMBARSICILVIEW LEFT OUTER JOIN TASSICILNORFID ON TASAMBARSICILVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO 
        //            //                UNION SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, KISIAD, ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASAMBARZIMMETVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, KIMEGITTI, NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, ODAAD FROM TASAMBARZIMMETVIEW LEFT OUTER JOIN TASSICILNORFID ON TASAMBARZIMMETVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO
        //            //                UNION SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, KISIAD, ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASDEVIRGIRISYAPILMAYANLARVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, KIMEGITTI, NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, ODAAD FROM TASDEVIRGIRISYAPILMAYANLARVIEW LEFT OUTER JOIN TASSICILNORFID ON TASDEVIRGIRISYAPILMAYANLARVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO
        //            //            ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASDEMIRBASVIEW adlı view güncellemesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }



        //            //    if (versiyon == 25)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET
        //            //                SET FIYAT = H2.FIYAT, KDVLIBIRIMFIYAT = H2.KDVLIBIRIMFIYAT, DEGERARTISI = H2.DEGERARTISI
        //            //                FROM TASSICILNOHAREKET INNER JOIN TASSICILNOHAREKET AS H2 ON TASSICILNOHAREKET.PRSICILNO = H2.PRSICILNO AND H2.SIRANO = 1
        //            //                WHERE (TASSICILNOHAREKET.SIRANO > 1)
        //            //            ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Devir çıkış sırasında oluşan fiyat (fiyat+degerArtis) sorununun düzeltilmesi. ";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 26)
        //            //    {
        //            //        sql = @"ALTER VIEW [dbo].[TASODA]
        //            //                AS
        //            //                SELECT '00001' AS MUHASEBEKOD, BIRIMKOD AS HARCAMABIRIMKOD, CAST(KOD AS varchar(20)) AS KOD, AD, '' AS AMBARKOD, USTKOD, SEVIYE, KOD AS UKOD, DURUM, MALIYETMERKEZI, 
        //            //                GORKODESKI AS ESKIKOD
        //            //                FROM dbo.YERLESKEODA";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASODA view de KOD alanının CAST(KOD AS varchar) AS KOD olarak değiştirildi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 27)
        //            //    {
        //            //        sql = @"UPDATE TASSICILNOHAREKET SET FIYAT = 2062900.12, KDVLIBIRIMFIYAT = 2062900.12 WHERE PRSICILNO = 131130
        //            //                UPDATE TASISLEMDETAY SET BIRIMFIYAT = 2062900.12, BIRIMFIYATKDVLI = 2062900.12 WHERE YIL=2008 AND HARCAMABIRIMKOD = '010' AND FISNO = '000035' AND SIRANO = 1
        //            //                ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "131130 nolu gayrimenkul için fiyat değişikliği.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 28)
        //            //    {
        //            //        sql = @"ALTER VIEW[dbo].[TASDEMIRBASVIEW]  
        //            //                AS SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, '' KISIAD, '' ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASAMBARSICILVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, '' KIMEGITTI, '' NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, BULUNDUGUYERAD, ISLEMTARIH FROM TASAMBARSICILVIEW LEFT OUTER JOIN TASSICILNORFID ON TASAMBARSICILVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO 
        //            //                UNION SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, KISIAD, ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASAMBARZIMMETVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, KIMEGITTI, NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, ODAAD, ISLEMTARIH FROM TASAMBARZIMMETVIEW LEFT OUTER JOIN TASSICILNORFID ON TASAMBARZIMMETVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO
        //            //                UNION SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, KISIAD, ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASDEVIRGIRISYAPILMAYANLARVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, KIMEGITTI, NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, ODAAD, ISLEMTARIH FROM TASDEVIRGIRISYAPILMAYANLARVIEW LEFT OUTER JOIN TASSICILNORFID ON TASDEVIRGIRISYAPILMAYANLARVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO
        //            //               ";

        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASDEMIRBASVIEW adlı view güncellemesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 29)
        //            //    {
        //            //        sql = @"UPDATE KULLANICI SET SICILNO = '4381207' WHERE MERNIS = '4381207'
        //            //                UPDATE KULLANICI SET SICILNO = '4381011' WHERE MERNIS = '4381011'";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "Buket DEMİRKAPI ve Mustafa KAYAnolu isimli kullanıcıların sicil no düzeltmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 30)
        //            //    {
        //            //        sql = @"ALTER TABLE dbo.TASAYARLARMB ADD AONAYIMUHASEBEKONTROLU int NULL";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "TASAYARLARMB tablosuna AONAYIMUHASEBEKONTROLU alanının eklenmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //    if (versiyon == 31)
        //            //    {
        //            //        sql = @"UPDATE YERLESKEODA SET BIRIMKOD = '030' WHERE USTKOD = 4426 AND BIRIMKOD = ''
        //            //                UPDATE YERLESKEODA SET BIRIMKOD = '030' WHERE USTKOD = 4431 AND BIRIMKOD = ''
        //            //                UPDATE YERLESKEODA SET BIRIMKOD = '030' WHERE USTKOD = 4434 AND BIRIMKOD = ''
        //            //                UPDATE YERLESKEODA SET BIRIMKOD = '030' WHERE USTKOD = 4450 AND BIRIMKOD = ''
        //            //                ";
        //            //        ve.hata = "";
        //            //        ve.ExecuteNonquery(sql);

        //            //        if (string.IsNullOrEmpty(ve.hata))
        //            //        {
        //            //            aciklama = "İstanbul Yerleşkesi (Ümraniye) listeleme sorununun giderilmesi.";
        //            //            ve.ExecuteNonquery("INSERT INTO VERSIYON (KOD,TARIH,ACIKLAMA) VALUES(" + versiyon++ + "," + VeriErisim.DegerYap(tarih) + "," + VeriErisim.DegerYap(aciklama) + ")");
        //            //        }
        //            //    }

        //            //}
        //            //else
        //            {
        //                ALTER TABLE dbo.TASTALEPKARSILAMADETAY ADD PARABIRIMIKOD varchar(10) NULL

        //                ALTER TABLE dbo.TASTALEPKARSILAMA ADD	TUR int NULL

        //                CREATE NONCLUSTERED INDEX[TASISLEMFORM_TAV1] ON[dbo].[TASISLEMFORM]([MUHASEBEKOD],[HARCAMABIRIMKOD],[DURUM],[YIL]) INCLUDE([FISNO], [AMBARKOD], [ISLEMTIPIKOD], [GMUHASEBEKOD], [GHARCAMAKOD])

        //                CREATE NONCLUSTERED INDEX[TASISLEMFORM_TAV2] ON[dbo].[TASSICILNOHAREKET] ([ISLEMTURUKOD],[DURUM]) INCLUDE([PRSICILNO], [SIRANO], [YIL], [MUHASEBEKOD], [HARCAMABIRIMKOD], [FISNO], [SICILNO], [HESAPPLANKOD], [AMBARKOD], [FIYAT], [KDVORAN], [ISLEMTIPIKOD], [ISLEMTARIH], [FISTARIH], [NEREYEGITTI], [KIMEGITTI], [KDVLIBIRIMFIYAT], [DEGERARTISI])


        //                //sql = "DROP VIEW[dbo].[TASDEMIRBASVIEW]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);
        //                //sql = "CREATE VIEW[dbo].[TASDEMIRBASVIEW] AS SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, '' KISIAD, '' ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASAMBARSICILVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, '' KIMEGITTI, '' NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, BULUNDUGUYERAD, ISLEMTARIH FROM TASAMBARSICILVIEW LEFT OUTER JOIN TASSICILNORFID ON TASAMBARSICILVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO UNION SELECT SICILNO, HESAPPLANKOD, HESAPPLANAD, KISIAD, ODAAD, ADI, YAZARADI, DISSICILNO, YAYINTARIHI, CILTNO, YERIKONUSU, TASAMBARZIMMETVIEW.PRSICILNO, SASENO, MUHASEBEKOD, HARCAMABIRIMKOD,  AMBARKOD, KIMEGITTI, NEREYEGITTI, TASSICILNORFID.RFIDNO, KDVORAN, KDVLIBIRIMFIYAT, FIYAT, MUHASEBEAD, HARCAMABIRIMAD, AMBARAD, MARKAAD, MODELAD, ISLEMTIPIKOD,YIL,FISNO,DISSICILNO2,GIAI, FISTARIH, BISNO, ESKIBISNO1, EKNO, AMORTIYIL, BULUNDUGUYER, ODAAD, ISLEMTARIH FROM TASAMBARZIMMETVIEW LEFT OUTER JOIN TASSICILNORFID ON TASAMBARZIMMETVIEW.PRSICILNO = TASSICILNORFID.PRSICILNO";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);


        //BUNDAN SONRASI 01.12.2019 TARİHİ
        //                //sql = "CREATE TABLE [dbo].[TASAKTARIM](";
        //                //sql += " [KOD] [varchar](50) NOT NULL,";
        //                //sql += " [MUHASEBEKOD] [varchar](5) NOT NULL,";
        //                //sql += " [HARCAMABIRIMKOD] [varchar](11) NOT NULL,";
        //                //sql += " [AMBARKOD] [varchar](2) NULL,";
        //                //sql += " [BELGETARIH] [datetime] NULL,";
        //                //sql += " [ISLEMTIPIKOD] [int] NULL,";
        //                //sql += " [BELGETUR] [int] NULL,";
        //                //sql += " [NEREDENGELDI] [varchar](250) NULL,";
        //                //sql += " [ACIKLAMA] [varchar](750) NULL,";
        //                //sql += " [ISLEMTARIH] [datetime] NULL,";
        //                //sql += " [ISLEMYAPANKOD] [varchar](20) NULL,";
        //                //sql += " [ISLEMYAPANAD] [varchar](80) NULL,";
        //                //sql += " [DURUM] [int] NULL,";
        //                //sql += " [TOPLAMKAYIT] [int] NULL,";
        //                //sql += " [ISLENENKAYIT] [int] NULL,";
        //                //sql += " [BELGENO] [varchar](750) NULL,";
        //                //sql += " CONSTRAINT [PK_TASAKTARIM] PRIMARY KEY CLUSTERED ";
        //                //sql += " ([KOD] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE [dbo].[TASAKTARIMDETAY](";
        //                //sql += " [KOD] [varchar](50) NOT NULL,";
        //                //sql += " [KODANA] [varchar](50) NULL,";
        //                //sql += " [SIRANO] [int] NULL,";
        //                //sql += " [HESAPPLANKOD] [varchar](50) NULL,";
        //                //sql += " [MIKTAR] [money] NULL,";
        //                //sql += " [KDV] [int] NULL,";
        //                //sql += " [BIRIMFIYAT] [money] NULL,";
        //                //sql += " [ACIKLAMA] [varchar](500) NULL,";
        //                //sql += " [ESERADI] [varchar](500) NULL,";
        //                //sql += " [YAZARADI] [varchar](500) NULL,";
        //                //sql += " [SICILNO] [varchar](50) NULL,";
        //                //sql += " [DURUM] [int] NULL,";
        //                //sql += " [BELGENO] [varchar](20) NULL,";
        //                //sql += " CONSTRAINT [PK_TASAKTARIMDETAY] PRIMARY KEY CLUSTERED ";
        //                //sql += " (	[KOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE [dbo].[TASAKTARIMTARIHCE](";
        //                //sql += " [KOD] [varchar](50) NOT NULL,";
        //                //sql += " [ILGIKOD] [varchar](50) NOT NULL,";
        //                //sql += " [ISLEMTARIH] [datetime] NOT NULL,";
        //                //sql += " [ACIKLAMA] [varchar](500) NULL,";
        //                //sql += " CONSTRAINT [PK_TASAKTARIMTARIHCE] PRIMARY KEY CLUSTERED ";
        //                //sql += " (	[KOD] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE[dbo].[TASGECICIIHRAC] (";
        //                //sql += " [PRSICILNO][int] NOT NULL,";
        //                //sql += " [YIL] [int] NULL,";
        //                //sql += " [MUHASEBEKOD] [varchar] (5) NULL,";
        //                //sql += " [HARCAMABIRIMKOD] [varchar] (11) NULL,";
        //                //sql += " [FISNO] [varchar] (10) NULL,";
        //                //sql += " CONSTRAINT[PK_TASGECICIIHRAC] PRIMARY KEY CLUSTERED";
        //                //sql += " ([PRSICILNO] ASC )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMFORM ADD ONAYACIKLAMA varchar(500) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASAMORTISMANFORM ADD";
        //                //sql += " FISACIKLAMA varchar(500) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMDETAY ADD";
        //                //sql += " YERLESKEYERI varchar(50) NULL,";
        //                //sql += " SATISBEDELI decimal(24, 12) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMOZELLIK ADD";
        //                //sql += " AMORTISMANYIL int NULL,";
        //                //sql += " AMORTISMANBITTI int NULL,";
        //                //sql += " BULUNDUGUYER varchar(255) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNOOZELLIK ADD";
        //                //sql += " AMORTISMANYIL int NULL,";
        //                //sql += " AMORTISMANBITTI int NULL,";
        //                //sql += " BULUNDUGUYER varchar(255) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE NONCLUSTERED INDEX IX_TASSICILNOHAREKET ON dbo.TASSICILNOHAREKET    (";
        //                //sql += " SICILNO ) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE NONCLUSTERED INDEX IX_TASSICILNOHAREKET_1 ON dbo.TASSICILNOHAREKET (";
        //                //sql += " MUHASEBEKOD,    HARCAMABIRIMKOD,    SICILNO) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE[dbo].[TASMUHASEBEISLEMTIPI] (";
        //                //sql += " [KOD][varchar](40) NOT NULL,";
        //                //sql += " [TIP] [varchar] (10) NULL,";
        //                //sql += " [AD] [varchar] (200) NULL,";
        //                //sql += " CONSTRAINT[PK_TASMUHASEBEISLEMTIPI] PRIMARY KEY CLUSTERED";
        //                //sql += " ([KOD] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE[dbo].[TASSICILNONUMARA] (";
        //                //sql += " [PRSICILNO][int] NOT NULL,";
        //                //sql += " [YIL] [int] NULL,";
        //                //sql += " [MUHASEBEKOD] [varchar] (5) NULL,";
        //                //sql += " [HARCAMABIRIMKOD] [varchar] (11) NULL,";
        //                //sql += " [AMBARKOD] [varchar] (2) NULL,";
        //                //sql += " [FISNO] [varchar] (10) NULL,";
        //                //sql += " CONSTRAINT[PK_TASSICILNONUMARA] PRIMARY KEY CLUSTERED";
        //                //sql += " ([PRSICILNO] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TASMUHASEBEBILGI]') AND type in (N'U')) ";
        //                //sql += " BEGIN ";
        //                //sql += " CREATE TABLE[dbo].[TASMUHASEBEBILGI]( ";
        //                //sql += " [ISLEMREFNO][varchar](25) NOT NULL, ";
        //                //sql += " [ISLEMCINSI] [varchar] (50) NULL, ";
        //                //sql += " [SERVIS] [varchar] (50) NULL, ";
        //                //sql += " [DURUM] [int] NULL, ";
        //                //sql += " [ISLEMYAPAN] [varchar] (50) NULL, ";
        //                //sql += " [ISLEMTARIH] [datetime] NULL, ";
        //                //sql += " [JSON] [varchar] (max) NULL, ";
        //                //sql += " CONSTRAINT[PK_TASMUHASEBEBILGI_1] PRIMARY KEY CLUSTERED  ";
        //                //sql += " ( [ISLEMREFNO] ASC ) ";
        //                //sql += " WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY] ";
        //                //sql += " END";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TASMUHASEBEBILGIDETAY]') AND type in (N'U')) ";
        //                //sql += " BEGIN ";
        //                //sql += " CREATE TABLE[dbo].[TASMUHASEBEBILGIDETAY]( ";
        //                //sql += " [PRSICILNO][int] NOT NULL, ";
        //                //sql += " [ISLEMREFNO] [varchar] (25) NULL, ";
        //                //sql += " CONSTRAINT[PK_TASMUHASEBEBILGIDETAY] PRIMARY KEY CLUSTERED ";
        //                //sql += " ( [PRSICILNO] ASC ) ";
        //                //sql += " WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY] ";
        //                //sql += " END";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TASAMORTISMANFORM]') AND type in (N'U')) ";
        //                //sql += " BEGIN ";
        //                //sql += " CREATE TABLE[dbo].[TASAMORTISMANFORM]( ";
        //                //sql += " [KOD][varchar](50) NOT NULL, ";
        //                //sql += " [YIL] [int] NULL, ";
        //                //sql += " [DONEM] [int] NULL, ";
        //                //sql += " [MUHASEBEKOD] [varchar] (5) NULL, ";
        //                //sql += " [HARCAMABIRIMKOD] [varchar] (11) NULL, ";
        //                //sql += " [FISNO] [varchar] (10) NULL, ";
        //                //sql += " [FISTARIH] [datetime] NULL, ";
        //                //sql += " [AMBARKOD] [varchar] (2) NULL, ";
        //                //sql += " [DURUM] [int] NULL, ";
        //                //sql += " [ISLEMTARIH] [datetime] NULL, ";
        //                //sql += " [ISLEMYAPAN] [varchar] (50) NULL, ";
        //                //sql += " [ONAYDURUM] [int] NULL, ";
        //                //sql += " CONSTRAINT[PK_TASAMORTISMANFORM] PRIMARY KEY CLUSTERED ";
        //                //sql += " ( [KOD] ASC ) ";
        //                //sql += " WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY] ) ON[PRIMARY] ";
        //                //sql += " END";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "DROP VIEW[dbo].[TASBAONAYLISTEVIEW]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);
        //                //sql = "CREATE VIEW[dbo].[TASBAONAYLISTEVIEW] AS";
        //                //sql += " SELECT A.YIL, A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.FISNO, A.FISTARIH, A.AMBARKOD, NULL AS 'ISLEMTIPIKOD', NULL AS 'MUAYENENO', NULL AS 'MUAYENETARIH', NULL AS 'DAYANAKNO', NULL";
        //                //sql += " AS 'DAYANAKTARIH', NULL AS 'NEREDENGELDI', NULL AS 'NEREYEGITTI', NULL AS 'GMUHASEBEKOD', NULL AS 'GHARCAMAKOD', NULL AS 'GAMBARKOD', A.DURUM, A.ISLEMTARIH, A.ISLEMYAPAN, NULL";
        //                //sql += " AS 'FATURANO', NULL AS 'FATURATARIH', NULL AS 'KIMEGITTI', NULL AS 'GYIL', NULL AS 'GFISNO', A.KOD, A.ONAYDURUM, NULL AS 'ACIKLAMA', NULL AS 'ONAYACIKLAMA', TASMUHASEBE.AD AS 'MUHASEBEAD',";
        //                //sql += " TASHARCAMABIRIM.AD AS 'HARCAMAAD', TASAMBAR.AD AS 'AMBARAD', NULL AS 'TASISLEMTIP', TASISLEMONAYYETKI.MERNIS, A.DONEM ";
        //                //sql += " FROM            TASHARCAMABIRIM INNER JOIN";
        //                //sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN";
        //                //sql += " TASAMBAR ON TASHARCAMABIRIM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND TASHARCAMABIRIM.KOD = TASAMBAR.HARCAMABIRIMKOD INNER JOIN";
        //                //sql += " TASAMORTISMANFORM AS A ON TASAMBAR.MUHASEBEKOD = A.MUHASEBEKOD AND TASAMBAR.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD AND TASAMBAR.KOD = A.AMBARKOD INNER JOIN";
        //                //sql += " TASISLEMONAYYETKI ON A.MUHASEBEKOD = TASISLEMONAYYETKI.MUHASEBEKOD AND A.HARCAMABIRIMKOD = TASISLEMONAYYETKI.HARCAMABIRIMKOD";
        //                //sql += " AND ( (TASISLEMONAYYETKI.ONAYTUR = 2) OR (TASISLEMONAYYETKI.ONAYTUR = 1 ) )";
        //                //sql += " UNION ALL";
        //                //sql += " SELECT TASISLEMFORM.YIL, TASISLEMFORM.MUHASEBEKOD, TASISLEMFORM.HARCAMABIRIMKOD, TASISLEMFORM.FISNO, TASISLEMFORM.FISTARIH, TASISLEMFORM.AMBARKOD,";
        //                //sql += " TASISLEMFORM.ISLEMTIPIKOD, TASISLEMFORM.MUAYENENO, TASISLEMFORM.MUAYENETARIH, TASISLEMFORM.DAYANAKNO, TASISLEMFORM.DAYANAKTARIH, TASISLEMFORM.NEREDENGELDI,";
        //                //sql += " TASISLEMFORM.NEREYEGITTI, TASISLEMFORM.GMUHASEBEKOD, TASISLEMFORM.GHARCAMAKOD, TASISLEMFORM.GAMBARKOD, TASISLEMFORM.DURUM, TASISLEMFORM.ISLEMTARIH,";
        //                //sql += " TASISLEMFORM.ISLEMYAPAN, TASISLEMFORM.FATURANO, TASISLEMFORM.FATURATARIH, TASISLEMFORM.KIMEGITTI, TASISLEMFORM.GYIL, TASISLEMFORM.GFISNO, TASISLEMFORM.KOD,";
        //                //sql += " TASISLEMFORM.ONAYDURUM, TASISLEMFORM.ACIKLAMA, TASISLEMFORM.ONAYACIKLAMA, TASMUHASEBE.AD AS 'MUHASEBEAD', TASHARCAMABIRIM.AD AS 'HARCAMAAD', TASAMBAR.AD AS 'AMBARAD', TASISLEMTIP.AD AS 'ISLEMTIPAD',";
        //                //sql += " TASISLEMONAYYETKI.MERNIS, 0";
        //                //sql += " FROM TASHARCAMABIRIM INNER JOIN";
        //                //sql += " TASMUHASEBE ON TASHARCAMABIRIM.MUHASEBEKOD = TASMUHASEBE.KOD INNER JOIN";
        //                //sql += " TASAMBAR ON TASHARCAMABIRIM.MUHASEBEKOD = TASAMBAR.MUHASEBEKOD AND TASHARCAMABIRIM.KOD = TASAMBAR.HARCAMABIRIMKOD INNER JOIN";
        //                //sql += " TASISLEMFORM ON TASAMBAR.MUHASEBEKOD = TASISLEMFORM.MUHASEBEKOD AND TASAMBAR.HARCAMABIRIMKOD = TASISLEMFORM.HARCAMABIRIMKOD AND";
        //                //sql += " TASAMBAR.KOD = TASISLEMFORM.AMBARKOD INNER JOIN";
        //                //sql += " TASISLEMTIP ON TASISLEMFORM.ISLEMTIPIKOD = TASISLEMTIP.KOD INNER JOIN";
        //                //sql += " TASISLEMONAYYETKI ON TASISLEMFORM.MUHASEBEKOD = TASISLEMONAYYETKI.MUHASEBEKOD AND TASISLEMFORM.HARCAMABIRIMKOD = TASISLEMONAYYETKI.HARCAMABIRIMKOD";
        //                //sql += " AND ( (TASISLEMONAYYETKI.ONAYTUR = 2) OR (TASISLEMONAYYETKI.ONAYTUR = 1) )";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNODEGERARTIS ADD TUR int NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASHESAPPLAN ADD KISTAMORTISMAN [int] NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMOZELLIK ADD";
        //                //sql += " EKNO varchar(255) NULL,";
        //                //sql += " HESAPTARIHI varchar(255) NULL,";
        //                //sql += " BUTCEKODU varchar(255) NULL,";
        //                //sql += " BLOKAJKODU varchar(255) NULL,";
        //                //sql += " GELDIGISUBE varchar(255) NULL ";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNOOZELLIK ADD";
        //                //sql += " EKNO varchar(255) NULL,";
        //                //sql += " HESAPTARIHI varchar(255) NULL,";
        //                //sql += " BUTCEKODU varchar(255) NULL,";
        //                //sql += " BLOKAJKODU varchar(255) NULL,";
        //                //sql += " GELDIGISUBE varchar(255) NULL ";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " CREATE TABLE[dbo].[TASTALEPKARSILAMACEVAP](";
        //                //sql += " [REFID][varchar](40) NOT NULL,";
        //                //sql += " [REFIDANA] [varchar] (40) NULL,";
        //                //sql += " [TARIH] [datetime] NULL,";
        //                //sql += " [ACIKLAMA] [varchar] (1000) NULL,";
        //                //sql += " [DURUM] [int] NULL,";
        //                //sql += " [KARSILAYANADISOYADI] [varchar] (150) NULL,";
        //                //sql += " [KARSILAYANUNVAN] [varchar] (150) NULL,";
        //                //sql += " CONSTRAINT[PK_TASTALEPKARSILAMACEVAP] PRIMARY KEY CLUSTERED";
        //                //sql += " ([REFID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " CREATE TABLE[dbo].[TASTALEPKARSILAMACEVAPDETAY] (";
        //                //sql += " [REFID][varchar](40) NOT NULL,";
        //                //sql += " [REFIDANA] [varchar] (40) NOT NULL,";
        //                //sql += " [REFIDTALEP] [varchar] (40) NULL,";
        //                //sql += " [TUR] [varchar] (20) NULL,";
        //                //sql += " [HESAPPLANKOD] [varchar] (30) NULL,";
        //                //sql += " [HESAPPLANAD] [varchar] (150) NULL,";
        //                //sql += " [OLCUBIRIMI] [varchar] (30) NULL,";
        //                //sql += " [SATINALMADURUM] [int] NULL,";
        //                //sql += " [MIKTAR] [money] NULL,";
        //                //sql += " CONSTRAINT[PK_TASTALEPKARSILAMACEVAPDETAY] PRIMARY KEY CLUSTERED";
        //                //sql += " ([REFID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " CREATE TABLE[dbo].[TASTALEPKARSILAMADETAY] (";
        //                //sql += " [YIL][int] NOT NULL,";
        //                //sql += " [MUHASEBEKOD] [varchar] (5) NOT NULL,";
        //                //sql += " [HARCAMABIRIMKOD] [varchar] (11) NOT NULL,";
        //                //sql += " [BELGENO] [varchar] (10) NOT NULL,";
        //                //sql += " [HESAPPLANKOD] [varchar] (30) NOT NULL,";
        //                //sql += " [AMBARKOD] [varchar] (2) NOT NULL,";
        //                //sql += " [ISTENILENMIKTAR] [money] NULL,";
        //                //sql += " [KARSILANANMIKTAR] [money] NULL,";
        //                //sql += " [KARSILAMASEKLI] [int] NULL,";
        //                //sql += " [SIRANO] [int] NOT NULL,";
        //                //sql += " [SATINALINANMIKTAR] [money] NULL,";
        //                //sql += " [ACIKLAMA] [varchar] (500) NULL,";
        //                //sql += " [TIFYIL] [int] NULL,";
        //                //sql += " [TIFFISNO] [varchar] (10) NULL,";
        //                //sql += " [REFIDANA] [varchar] (40) NULL,";
        //                //sql += " [REFID] [varchar] (40) NULL,";
        //                //sql += " [BIRIMFIYAT] [money] NULL,";
        //                //sql += " [IPTAL] [int] NULL,";
        //                //sql += " CONSTRAINT[PK_TASTALEPKARSILAMADETAY_1] PRIMARY KEY CLUSTERED";
        //                //sql += " ([YIL] ASC, [MUHASEBEKOD] ASC, [HARCAMABIRIMKOD] ASC, [BELGENO] ASC, [SIRANO] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " ALTER TABLE[dbo].[TASTALEPKARSILAMADETAY] ADD CONSTRAINT[DF_TASTALEPKARSILAMADETAY_SIRANO]  DEFAULT((0)) FOR[SIRANO]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " ALTER TABLE[dbo].[TASTALEPKARSILAMADETAY] WITH CHECK ADD CONSTRAINT[FK_TASTALEPKARSILAMADETAY_TASHESAPPLAN] FOREIGN KEY([HESAPPLANKOD]) REFERENCES[dbo].[TASHESAPPLAN] ([KOD])";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " ALTER TABLE[dbo].[TASTALEPKARSILAMADETAY] CHECK CONSTRAINT[FK_TASTALEPKARSILAMADETAY_TASHESAPPLAN]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " ALTER TABLE[dbo].[TASTALEPKARSILAMADETAY] WITH CHECK ADD CONSTRAINT[FK_TASTALEPKARSILAMADETAY_TASTALEPKARSILAMA] FOREIGN KEY([YIL], [MUHASEBEKOD], [HARCAMABIRIMKOD], [BELGENO]) REFERENCES[dbo].[TASTALEPKARSILAMA] ([YIL], [MUHASEBEKOD], [HARCAMABIRIMKOD], [BELGENO])";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " ALTER TABLE[dbo].[TASTALEPKARSILAMADETAY] CHECK CONSTRAINT[FK_TASTALEPKARSILAMADETAY_TASTALEPKARSILAMA]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " CREATE TABLE[dbo].[TASSICILNODEGERARTIS](";
        //                //sql += " [KOD][varchar](40) NOT NULL,";
        //                //sql += " [MUHASEBEKOD] [varchar] (5) NULL,";
        //                //sql += " [HARCAMABIRIMKOD] [varchar] (11) NULL,";
        //                //sql += " [BELGENO] [varchar] (10) NULL,";
        //                //sql += " [BELGETARIHI] [datetime] NULL,";
        //                //sql += " [PRSICILNO] [int] NULL,";
        //                //sql += " [GORSICILNO] [varchar] (30) NULL,";
        //                //sql += " [TUTAR] [money] NULL,";
        //                //sql += " [GEREKCE] [varchar] (250) NULL,";
        //                //sql += " [DURUM] [int] NULL,";
        //                //sql += " CONSTRAINT[PK_TASSICILNODEGERARTIS] PRIMARY KEY CLUSTERED([KOD] ASC";
        //                //sql += " )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY =";
        //                //sql += " OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]";
        //                //sql += " ) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TASSICILNODURUM]') AND type in (N'U'))";
        //                //sql += " BEGIN";
        //                //sql += " CREATE TABLE [dbo].[TASSICILNODURUM](";
        //                //sql += " [PRSICILNO] [int] NOT NULL,";
        //                //sql += " [GORSICILNO] [varchar](50) NULL,";
        //                //sql += " [TIFKOD] [varchar](50) NULL,";
        //                //sql += " [DURUM] [int] NULL,";
        //                //sql += " [DURUMISLEM] [int] NULL,";
        //                //sql += " CONSTRAINT [PK_TASSICILNODURUM] PRIMARY KEY CLUSTERED ";
        //                //sql += " (";
        //                //sql += " [PRSICILNO] ASC";
        //                //sql += " )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        //                //sql += " ) ON [PRIMARY]";
        //                //sql += " END";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNODURUM ADD DURUMACIKLAMA varchar(250) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNODURUM ADD ISLEMTARIH datetime NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);


        //                //sql = "DROP VIEW[dbo].[TASAMBARSICILVIEW]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);
        //                //sql = "CREATE VIEW[dbo].[TASAMBARSICILVIEW] AS SELECT A.PRSICILNO, A.SIRANO, A.YIL, A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.FISNO, A.FISSIRANO, A.SICILNO, A.SICILSIRANO, A.HESAPPLANKOD,    A.AMBARKOD, (CONVERT(DECIMAL(21, 12),((A.FIYAT*(((A.KDVORAN+0.0)/100)+1)) + A.DEGERARTISI))/CONVERT(DECIMAL(21, 12), (A.KDVORAN + 0.0) / 100 + 1))  AS FIYAT, A.KDVORAN, A.ISLEMTIPIKOD, A.ISLEMTURUKOD, A.ISLEMTARIH, A.ISLEMYAPAN, A.DURUM,    A.FISTARIH, A.NEREDENGELDI, A.NEREYEGITTI, A.KIMEGITTI, A.ZIMMETOZELLIK, A.KDVLIBIRIMFIYAT, A.DEGERARTISI,    dbo.TASSICILNOOZELLIK.MARKAKOD, dbo.TASSICILNOOZELLIK.MODELKOD, dbo.TASSICILNOOZELLIK.SASENO, dbo.TASSICILNOOZELLIK.MOTORNO,    dbo.TASSICILNOOZELLIK.PLAKA, dbo.TASSICILNOOZELLIK.CILTNO, dbo.TASSICILNOOZELLIK.DIL, dbo.TASSICILNOOZELLIK.YAZARADI,    dbo.TASSICILNOOZELLIK.ADI, dbo.TASSICILNOOZELLIK.YAYINYERI, dbo.TASSICILNOOZELLIK.YAYINTARIHI,    dbo.TASSICILNOOZELLIK.NEREDENGELDI AS OZELLIKNEREDENGELDI, dbo.TASSICILNOOZELLIK.BOYUTLARI,    dbo.TASSICILNOOZELLIK.SATIRSAYISI, dbo.TASSICILNOOZELLIK.YAPRAKSAYISI, dbo.TASSICILNOOZELLIK.SAYFASAYISI,    dbo.TASSICILNOOZELLIK.CILTTURU, dbo.TASSICILNOOZELLIK.CESIDI, dbo.TASSICILNOOZELLIK.YERIKONUSU,    dbo.TASSICILNOOZELLIK.GELISTARIHI, dbo.TASSICILNOOZELLIK.NEREDEBULUNDU, dbo.TASSICILNOOZELLIK.CAGI,    dbo.TASSICILNOOZELLIK.DURUMUMADDESI, dbo.TASSICILNOOZELLIK.ONYUZ, dbo.TASSICILNOOZELLIK.ARKAYUZ, dbo.TASSICILNOOZELLIK.PUAN,    dbo.TASSICILNOOZELLIK.FOTOGRAF, dbo.TASSICILNOOZELLIK.DISSICILNO, dbo.TASMUHASEBE.AD AS MUHASEBEAD,    dbo.TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, dbo.TASAMBAR.AD AS AMBARAD, dbo.TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD,    dbo.TASMARKA.AD AS MARKAAD, dbo.TASMODEL.AD AS MODELAD, dbo.TASSICILNOOZELLIK.DISSICILNO2, dbo.TASSICILNOOZELLIK.GIAI, dbo.TASSICILNOOZELLIK.BISNO, dbo.TASSICILNOOZELLIK.ESKIBISNO1, dbo.TASSICILNOOZELLIK.EKNO, dbo.TASHESAPPLAN.AMORTIYIL, dbo.TASSICILNOOZELLIK.BULUNDUGUYER, dbo.TASODA.AD  AS BULUNDUGUYERAD FROM dbo.TASSICILNOHAREKET A INNER JOIN   dbo.TASHESAPPLAN ON A.HESAPPLANKOD = dbo.TASHESAPPLAN.KOD INNER JOIN dbo.TASAMBAR ON A.MUHASEBEKOD = dbo.TASAMBAR.MUHASEBEKOD AND A.HARCAMABIRIMKOD = dbo.TASAMBAR.HARCAMABIRIMKOD AND A.AMBARKOD = dbo.TASAMBAR.KOD INNER JOIN   dbo.TASHARCAMABIRIM ON A.MUHASEBEKOD = dbo.TASHARCAMABIRIM.MUHASEBEKOD AND A.HARCAMABIRIMKOD = dbo.TASHARCAMABIRIM.KOD INNER JOIN   dbo.TASMUHASEBE ON A.MUHASEBEKOD = dbo.TASMUHASEBE.KOD " +
        //                //    "LEFT OUTER JOIN dbo.TASMODEL RIGHT OUTER JOIN dbo.TASSICILNOOZELLIK ON dbo.TASMODEL.MARKAKOD = dbo.TASSICILNOOZELLIK.MARKAKOD AND dbo.TASMODEL.KOD = dbo.TASSICILNOOZELLIK.MODELKOD LEFT OUTER JOIN dbo.TASMARKA ON dbo.TASSICILNOOZELLIK.MARKAKOD = dbo.TASMARKA.KOD ON A.PRSICILNO = dbo.TASSICILNOOZELLIK.PRSICILNO LEFT OUTER JOIN dbo.TASODA ON dbo.TASSICILNOOZELLIK.BULUNDUGUYER = dbo.TASODA.KOD   " +
        //                //    "WHERE (A.ISLEMTURUKOD< 50) AND (A.DURUM<> 2) AND(A.PRSICILNO NOT IN   (SELECT PRSICILNO   FROM TASSICILNOHAREKET B   WHERE A.PRSICILNO = B.PRSICILNO AND A.SIRANO<B.SIRANO AND ISLEMTURUKOD >= 50)) AND(A.SIRANO IN   (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET B   WHERE A.PRSICILNO = B.PRSICILNO AND ISLEMTURUKOD < 50))";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "DROP VIEW[dbo].[TASAMBARZIMMETVIEW]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);
        //                //sql = @"CREATE VIEW[dbo].[TASAMBARZIMMETVIEW]
        //                //  AS SELECT A.PRSICILNO, A.SIRANO, A.YIL, A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.FISNO, A.FISSIRANO, A.SICILNO, A.SICILSIRANO, A.HESAPPLANKOD, A.AMBARKOD, 
        //                //  CONVERT(DECIMAL(21, 12), A.FIYAT * ((A.KDVORAN + 0.0) / 100 + 1) + A.DEGERARTISI) / CONVERT(DECIMAL(21, 12), (A.KDVORAN + 0.0) / 100 + 1) AS FIYAT, 
        //                //  A.KDVORAN, A.ISLEMTIPIKOD, A.ISLEMTURUKOD, 
        //                //  A.ISLEMTARIH, A.ISLEMYAPAN, A.DURUM, A.FISTARIH, A.NEREDENGELDI, A.NEREYEGITTI, A.KIMEGITTI, A.ZIMMETOZELLIK, A.KDVLIBIRIMFIYAT, A.DEGERARTISI, dbo.TASSICILNOOZELLIK.MARKAKOD, 
        //                //  dbo.TASSICILNOOZELLIK.MODELKOD, dbo.TASSICILNOOZELLIK.SASENO, dbo.TASSICILNOOZELLIK.MOTORNO, dbo.TASSICILNOOZELLIK.PLAKA, dbo.TASSICILNOOZELLIK.CILTNO, dbo.TASSICILNOOZELLIK.DIL, 
        //                //  dbo.TASSICILNOOZELLIK.YAZARADI, dbo.TASSICILNOOZELLIK.ADI, dbo.TASSICILNOOZELLIK.YAYINYERI, dbo.TASSICILNOOZELLIK.YAYINTARIHI, dbo.TASSICILNOOZELLIK.NEREDENGELDI AS OZELLIKNEREDENGELDI, 
        //                //  dbo.TASSICILNOOZELLIK.BOYUTLARI, dbo.TASSICILNOOZELLIK.SATIRSAYISI, dbo.TASSICILNOOZELLIK.YAPRAKSAYISI, dbo.TASSICILNOOZELLIK.SAYFASAYISI, dbo.TASSICILNOOZELLIK.CILTTURU, dbo.TASSICILNOOZELLIK.CESIDI, 
        //                //  dbo.TASSICILNOOZELLIK.YERIKONUSU, dbo.TASSICILNOOZELLIK.GELISTARIHI, dbo.TASSICILNOOZELLIK.NEREDEBULUNDU, dbo.TASSICILNOOZELLIK.CAGI, dbo.TASSICILNOOZELLIK.DURUMUMADDESI, 
        //                //  dbo.TASSICILNOOZELLIK.ONYUZ, dbo.TASSICILNOOZELLIK.ARKAYUZ, dbo.TASSICILNOOZELLIK.PUAN, dbo.TASSICILNOOZELLIK.FOTOGRAF, dbo.TASSICILNOOZELLIK.DISSICILNO, dbo.TASMUHASEBE.AD AS MUHASEBEAD, 
        //                //  dbo.TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, dbo.TASAMBAR.AD AS AMBARAD, dbo.TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD, dbo.TASMARKA.AD AS MARKAAD, dbo.TASMODEL.AD AS MODELAD, 
        //                //  dbo.TASPERSONEL.AD AS KISIAD, dbo.TASODA.AD AS ODAAD, dbo.TASPERSONEL.ODA, dbo.TASIL.KOD AS ILKOD, dbo.TASIL.AD AS ILAD, dbo.TASILCE.KOD AS ILCEKOD, dbo.TASILCE.AD AS ILCEAD, 
        //                //  dbo.TASOLCUBIRIM.KOD AS OLCUBIRIMKOD, dbo.TASOLCUBIRIM.AD AS OLCUBIRIMAD, dbo.TASSICILNOOZELLIK.DISSICILNO2, dbo.TASSICILNOOZELLIK.GIAI, dbo.TASSICILNOOZELLIK.BISNO, 
        //                //  dbo.TASSICILNOOZELLIK.ESKIBISNO1, dbo.TASSICILNOOZELLIK.EKNO, dbo.TASHESAPPLAN.AMORTIYIL, dbo.TASSICILNOOZELLIK.BULUNDUGUYER, '' AS BULUNDUGUYERAD
        //                //  FROM dbo.TASMARKA RIGHT OUTER JOIN
        //                //  dbo.TASSICILNOOZELLIK LEFT OUTER JOIN
        //                //  dbo.TASMODEL ON dbo.TASSICILNOOZELLIK.MARKAKOD = dbo.TASMODEL.MARKAKOD AND dbo.TASSICILNOOZELLIK.MODELKOD = dbo.TASMODEL.KOD RIGHT OUTER JOIN
        //                //  dbo.TASILCE INNER JOIN
        //                //  dbo.TASAMBAR INNER JOIN
        //                //  dbo.TASSICILNOHAREKET AS A INNER JOIN
        //                //  dbo.TASHESAPPLAN ON A.HESAPPLANKOD = dbo.TASHESAPPLAN.KOD INNER JOIN
        //                //  dbo.TASMUHASEBE ON A.MUHASEBEKOD = dbo.TASMUHASEBE.KOD INNER JOIN
        //                //  dbo.TASHARCAMABIRIM ON A.HARCAMABIRIMKOD = dbo.TASHARCAMABIRIM.KOD AND A.MUHASEBEKOD = dbo.TASHARCAMABIRIM.MUHASEBEKOD ON dbo.TASAMBAR.MUHASEBEKOD = A.MUHASEBEKOD AND
        //                //  dbo.TASAMBAR.HARCAMABIRIMKOD = A.HARCAMABIRIMKOD AND dbo.TASAMBAR.KOD = A.AMBARKOD ON dbo.TASILCE.ILKOD = dbo.TASMUHASEBE.ILKOD AND dbo.TASILCE.KOD = dbo.TASMUHASEBE.ILCEKOD INNER JOIN
        //                //  dbo.TASIL ON dbo.TASILCE.ILKOD = dbo.TASIL.KOD INNER JOIN
        //                //  dbo.TASOLCUBIRIM ON dbo.TASHESAPPLAN.OLCUBIRIMKOD = dbo.TASOLCUBIRIM.KOD ON dbo.TASSICILNOOZELLIK.PRSICILNO = A.PRSICILNO ON dbo.TASMARKA.KOD = dbo.TASSICILNOOZELLIK.MARKAKOD LEFT OUTER JOIN
        //                //  dbo.TASPERSONEL ON A.KIMEGITTI = dbo.TASPERSONEL.KOD LEFT OUTER JOIN
        //                //  dbo.TASODA ON A.NEREYEGITTI = dbo.TASODA.KOD AND A.HARCAMABIRIMKOD = dbo.TASODA.HARCAMABIRIMKOD AND A.MUHASEBEKOD = dbo.TASODA.MUHASEBEKOD
        //                //  WHERE(A.DURUM <> 2) AND(A.PRSICILNO NOT IN  (SELECT PRSICILNO FROM      dbo.TASSICILNOHAREKET AS B
        //                //  WHERE(A.PRSICILNO = PRSICILNO) AND(A.SIRANO < SIRANO) AND(ISLEMTURUKOD = 46 OR
        //                //  ISLEMTURUKOD = 47))) AND(A.ISLEMTURUKOD = '90' OR
        //                //  A.ISLEMTURUKOD = '91')";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);


        //                ////Cumhurbaşkanlığı veya başka kurumlarda TASODA viewı farklı olarak tasarlandığı için sorun oluyor
        //                ////sql = "DROP VIEW [dbo].[TASODA]";
        //                ////ve.hata = "";
        //                ////ve.ExecuteNonquery(sql);
        //                ////sql = "CREATE VIEW[dbo].[TASODA] AS SELECT '00001' AS MUHASEBEKOD, BIRIMKOD AS HARCAMABIRIMKOD, GORKOD AS KOD, AD, '' AS AMBARKOD, USTKOD, SEVIYE, KOD AS UKOD, DURUM FROM dbo.YERLESKEODA";
        //                ////ve.hata = "";
        //                ////ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASODA ADD UKOD int NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASGECICIFORM ADD TIFFISNO varchar(10) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMOZELLIK ADD GIAI varchar(255) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNOOZELLIK ADD  GIAI varchar(255) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNOOZELLIK ADD GARANTIBITISTARIHI datetime NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMOZELLIK ADD";
        //                //sql += " DISSICILNO2 varchar(255) NULL,        ";
        //                //sql += " BISNO varchar(255) NULL,              ";
        //                //sql += " ESKIBISNO1 varchar(255) NULL,         ";
        //                //sql += " ESKIBISNO2 varchar(255) NULL,         ";
        //                //sql += " RFIDETIKETTURKOD int NULL,            ";
        //                //sql += " GARANTIBITISTARIHI datetime NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASHESAPPLAN ADD RFIDETIKETKOD int NULL, MARKAKOD int NULL, MODELKOD int NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE[dbo].[TASISLEMDOSYA] (";
        //                //sql += " [DOSYAID][int] NOT NULL,   [YIL] [int] NOT NULL,   [MUHASEBEKOD] [varchar] (5) NOT NULL,    [HARCAMABIRIMKOD] [varchar] (11) NOT NULL,     [FISNO] [varchar] (10) NOT NULL,      [SIRANO] [int] NULL,";
        //                //sql += " [RESIM] [image] NULL,	[ADI] [varchar] (300) NULL,	[BOYUTU] [money] NULL,	[EKLEYENKISIAD] [varchar] (50) NULL,	[EKLEYENKISIKOD] [varchar] (50) NULL,	[EKLEMETARIHI] [datetime] NULL,";
        //                //sql += " CONSTRAINT[PK_TASISLEMDOSYA] PRIMARY KEY CLUSTERED([DOSYAID] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TASRFIDETIKETTURU]') AND type in (N'U')) ";
        //                //sql += " BEGIN";
        //                //sql += " CREATE TABLE [dbo].[TASRFIDETIKETTURU](";
        //                //sql += " [KOD] [int] NOT NULL,";
        //                //sql += " [AD] [varchar](50) NOT NULL,";
        //                //sql += " CONSTRAINT [PK_TASRFIDETIKETTURU] PRIMARY KEY CLUSTERED ";
        //                //sql += " ([KOD] ASC)";
        //                //sql += " WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
        //                //sql += " END";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNOOZELLIK ADD RFIDETIKETTURKOD int NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMFORM ADD ACIKLAMA varchar(MAX) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASPERSONEL ADD EPOSTA varchar(50) NULL, CEPTEL varchar(20) NULL,FAKS varchar(20) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSICILNOOZELLIK ADD DISSICILNO2 varchar(255) NULL,	BISNO varchar(255) NULL, ESKIBISNO1 varchar(255) NULL,	ESKIBISNO2 varchar(255) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMFORMTARIHCE ADD IPADRESI varchar(50) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASZIMMETFORMTARIHCE ADD IPADRESI varchar(50) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE[dbo].[TASISLEMONAYYETKI] (";
        //                //sql += " [MUHASEBEKOD][varchar](5) NOT NULL,";
        //                //sql += " [HARCAMABIRIMKOD] [varchar] (11) NOT NULL,";
        //                //sql += " [MERNIS] [varchar] (20) NOT NULL,";
        //                //sql += " [ADSOYAD] [varchar] (50) NULL,";
        //                //sql += " [ONAYTUR] [int] NOT NULL,";
        //                //sql += " CONSTRAINT[PK_TASISLEMONAYYETKI] PRIMARY KEY CLUSTERED (";
        //                //sql += " [MUHASEBEKOD] ASC, [HARCAMABIRIMKOD] ASC, [MERNIS] ASC, [ONAYTUR] ASC";
        //                //sql += " )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASRESIM ADD";
        //                //sql += " ADI varchar(300) NULL,";
        //                //sql += " BOYUTU money NULL,";
        //                //sql += " EKLEYENKISIAD varchar(50) COLLATE Turkish_CI_AS NULL,";
        //                //sql += " EKLEYENKISIKOD varchar(50) COLLATE Turkish_CI_AS NULL,";
        //                //sql += " EKLEMETARIHI datetime NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMFORMTARIHCE ADD TASISLEMFORMKOD varchar(50) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASISLEMFORM ADD KOD varchar(50) NULL, ONAYDURUM int NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSAYIMAMBARDETAY ADD SAYIMDETAYKOD varchar(50) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER TABLE dbo.TASSAYIMKISIDETAY ADD SAYIMDETAYKOD varchar(50) NULL";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "CREATE TABLE [dbo].[TASISLEMSUREC](";
        //                //sql += " [YIL] [int] NOT NULL,";
        //                //sql += " [MUHASEBEKOD] [varchar](5) NOT NULL,";
        //                //sql += " [HARCAMABIRIMKOD] [varchar](11) NOT NULL,";
        //                //sql += " [FISNO] [varchar](10) NOT NULL,";
        //                //sql += " [SURECNO] [varchar](50) NULL,";
        //                //sql += " CONSTRAINT [PK_TASISLEMSUREC] PRIMARY KEY CLUSTERED ";
        //                //sql += " (";
        //                //sql += " [YIL] ASC,";
        //                //sql += " [MUHASEBEKOD] ASC,";
        //                //sql += " [HARCAMABIRIMKOD] ASC,";
        //                //sql += " [FISNO] ASC";
        //                //sql += " )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        //                //sql += " ) ON [PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = " CREATE TABLE [dbo].[TASTOPLUISLEMFORM](";
        //                //sql += " [GRUPKOD] [int] NOT NULL,";
        //                //sql += " [ACIKLAMA] [varchar](150) NULL,";
        //                //sql += " CONSTRAINT [PK_TASTOPLUISLEMFORM] PRIMARY KEY CLUSTERED ";
        //                //sql += " (";
        //                //sql += " [GRUPKOD] ASC";
        //                //sql += " )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
        //                //sql += " ) ON [PRIMARY]";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "  ALTER VIEW [dbo].[TASAMBARSTOKVIEW] AS";
        //                //sql += " SELECT dbo.TASISLEMDETAY.YIL, dbo.TASISLEMDETAY.MUHASEBEKOD, dbo.TASISLEMDETAY.HARCAMABIRIMKOD, dbo.TASISLEMDETAY.FISNO, dbo.TASISLEMDETAY.SIRANO, dbo.TASISLEMDETAY.HESAPPLANKOD,";
        //                //sql += " dbo.TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD, dbo.TASISLEMDETAY.MIKTAR, dbo.TASISLEMDETAY.KDVORAN, dbo.TASISLEMDETAY.BIRIMFIYAT, dbo.TASOLCUBIRIM.AD AS OLCUBIRIMAD, ";
        //                //sql += " dbo.TASISLEMFORM.AMBARKOD, dbo.TASISLEMTIP.TUR AS ISLEMTUR, dbo.TASISLEMFORM.DURUM, dbo.TASISLEMFORM.ISLEMTIPIKOD";
        //                //sql += " FROM dbo.TASISLEMFORM INNER JOIN";
        //                //sql += " dbo.TASISLEMDETAY ON dbo.TASISLEMFORM.YIL = dbo.TASISLEMDETAY.YIL AND dbo.TASISLEMFORM.MUHASEBEKOD = dbo.TASISLEMDETAY.MUHASEBEKOD AND";
        //                //sql += " dbo.TASISLEMFORM.HARCAMABIRIMKOD = dbo.TASISLEMDETAY.HARCAMABIRIMKOD AND dbo.TASISLEMFORM.FISNO = dbo.TASISLEMDETAY.FISNO INNER JOIN";
        //                //sql += " dbo.TASISLEMTIP ON dbo.TASISLEMFORM.ISLEMTIPIKOD = dbo.TASISLEMTIP.KOD INNER JOIN";
        //                //sql += " dbo.TASHESAPPLAN ON dbo.TASISLEMDETAY.HESAPPLANKOD = dbo.TASHESAPPLAN.KOD INNER JOIN";
        //                //sql += " dbo.TASOLCUBIRIM ON dbo.TASHESAPPLAN.OLCUBIRIMKOD = dbo.TASOLCUBIRIM.KOD";
        //                //sql += " WHERE (dbo.TASISLEMDETAY.HESAPPLANKOD LIKE '150%') AND (dbo.TASISLEMFORM.DURUM = 5)";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //sql = "ALTER VIEW dbo.TASSICILHAREKETVIEW AS";
        //                //sql += " SELECT dbo.TASSICILNOHAREKET.YIL, dbo.TASSICILNOHAREKET.MUHASEBEKOD, dbo.TASMUHASEBE.AD AS MUHASEBEAD, dbo.TASSICILNOHAREKET.HARCAMABIRIMKOD, ";
        //                //sql += " dbo.TASHARCAMABIRIM.AD AS HARCAMABIRIMAD, dbo.TASSICILNOHAREKET.AMBARKOD, dbo.TASAMBAR.AD AS AMBARAD, dbo.TASSICILNOHAREKET.FISTARIH, dbo.TASSICILNOHAREKET.FISNO,";
        //                //sql += " dbo.TASSICILNOHAREKET.NEREDENGELDI, dbo.TASSICILNOHAREKET.NEREYEGITTI, dbo.TASSICILNOHAREKET.HESAPPLANKOD, dbo.TASHESAPPLAN.ACIKLAMA AS HESAPPLANAD, ";
        //                //sql += " dbo.TASSICILNOHAREKET.KDVLIBIRIMFIYAT, dbo.TASSICILNOHAREKET.PRSICILNO AS SICILNOHAREKETPRSICILNO, dbo.TASSICILNOHAREKET.SIRANO AS SICILNOHAREKETSIRANO, ";
        //                //sql += " dbo.TASSICILNOHAREKET.SICILNO AS SICILNOHAREKETSICILNO, dbo.TASOLCUBIRIM.KOD AS OLCUBIRIMKOD, dbo.TASOLCUBIRIM.AD AS OLCUBIRIMAD, dbo.TASIL.KOD AS ILKOD, dbo.TASIL.AD AS ILAD, ";
        //                //sql += " dbo.TASILCE.KOD AS ILCEKOD, dbo.TASILCE.AD AS ILCEAD, dbo.TASISLEMTIP.KOD AS ISLEMTIPKOD, dbo.TASISLEMTIP.AD AS ISLEMTIPAD, dbo.TASSICILNOHAREKET.ISLEMTURUKOD AS ISLEMTIPTUR, ";
        //                //sql += " dbo.TASSICILNOHAREKET.KIMEGITTI, dbo.TASSICILNOHAREKET.ZIMMETOZELLIK, dbo.TASSICILNOHAREKET.GONMUHASEBEKOD, dbo.TASSICILNOHAREKET.GONHARCAMABIRIMKOD, ";
        //                //sql += " dbo.TASSICILNOHAREKET.GONAMBARKOD, dbo.TASSICILNOOZELLIK.MARKAKOD, dbo.TASSICILNOOZELLIK.MODELKOD, dbo.TASSICILNOOZELLIK.SASENO, dbo.TASSICILNOOZELLIK.MOTORNO, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.PLAKA, dbo.TASSICILNOOZELLIK.CILTNO, dbo.TASSICILNOOZELLIK.DIL, dbo.TASSICILNOOZELLIK.YAZARADI, dbo.TASSICILNOOZELLIK.ADI, dbo.TASSICILNOOZELLIK.YAYINYERI, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.YAYINTARIHI, dbo.TASSICILNOOZELLIK.NEREDENGELDI AS OZELLIKNEREDENGELDI, dbo.TASSICILNOOZELLIK.BOYUTLARI, dbo.TASSICILNOOZELLIK.SATIRSAYISI, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.YAPRAKSAYISI, dbo.TASSICILNOOZELLIK.SAYFASAYISI, dbo.TASSICILNOOZELLIK.CILTTURU, dbo.TASSICILNOOZELLIK.CESIDI, dbo.TASSICILNOOZELLIK.YERIKONUSU, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.GELISTARIHI, dbo.TASSICILNOOZELLIK.NEREDEBULUNDU, dbo.TASSICILNOOZELLIK.CAGI, dbo.TASSICILNOOZELLIK.DURUMUMADDESI, dbo.TASSICILNOOZELLIK.ONYUZ, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.ARKAYUZ, dbo.TASSICILNOOZELLIK.FOTOGRAF, dbo.TASSICILNOOZELLIK.PUAN, dbo.TASSICILNOOZELLIK.DISSICILNO, dbo.TASSICILNOHAREKET.DEGERARTISI, ";
        //                //sql += " dbo.TASSICILNOHAREKET.ISLEMTIPIKOD, dbo.TASSICILNOOZELLIK.DISSICILNO2, dbo.TASSICILNOOZELLIK.BISNO, dbo.TASSICILNOOZELLIK.ESKIBISNO1, dbo.TASSICILNOOZELLIK.ESKIBISNO2, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.RFIDETIKETTURKOD, dbo.TASSICILNOOZELLIK.GARANTIBITISTARIHI, dbo.TASSICILNOOZELLIK.GIAI, dbo.TASSICILNOOZELLIK.EKNO, dbo.TASSICILNOOZELLIK.HESAPTARIHI, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.BUTCEKODU, dbo.TASSICILNOOZELLIK.BLOKAJKODU, dbo.TASSICILNOOZELLIK.GELDIGISUBE, ";
        //                //sql += " dbo.TASSICILNOOZELLIK.AMORTISMANYIL, dbo.TASSICILNOOZELLIK.AMORTISMANBITTI, dbo.TASSICILNOOZELLIK.BULUNDUGUYER, ";
        //                ////sql += " (SELECT AD FROM dbo.YERLESKEODA AS BULUNDUGUYER WHERE BULUNDUGUYER.GORKOD = dbo.TASSICILNOOZELLIK.BULUNDUGUYER) AS BULUNDUGUYERAD, ";
        //                //sql += " (SELECT AD FROM dbo.TASODA      AS BULUNDUGUYER WHERE BULUNDUGUYER.KOD    = dbo.TASSICILNOOZELLIK.BULUNDUGUYER) AS BULUNDUGUYERAD, ";
        //                //sql += " dbo.TASMARKA.AD AS MARKAAD,dbo.TASMODEL.AD AS MODELAD, dbo.TASSICILNOHAREKET.FISSIRANO AS FISSIRANO";
        //                //sql += " FROM dbo.TASOLCUBIRIM INNER JOIN";
        //                //sql += " dbo.TASHESAPPLAN ON dbo.TASOLCUBIRIM.KOD = dbo.TASHESAPPLAN.OLCUBIRIMKOD RIGHT OUTER JOIN";
        //                //sql += " dbo.TASSICILNOOZELLIK RIGHT OUTER JOIN";
        //                //sql += " dbo.TASHARCAMABIRIM INNER JOIN";
        //                //sql += " dbo.TASAMBAR ON dbo.TASHARCAMABIRIM.MUHASEBEKOD = dbo.TASAMBAR.MUHASEBEKOD AND dbo.TASHARCAMABIRIM.KOD = dbo.TASAMBAR.HARCAMABIRIMKOD INNER JOIN";
        //                //sql += " dbo.TASMUHASEBE ON dbo.TASHARCAMABIRIM.MUHASEBEKOD = dbo.TASMUHASEBE.KOD INNER JOIN";
        //                //sql += " dbo.TASILCE ON dbo.TASMUHASEBE.ILKOD = dbo.TASILCE.ILKOD AND dbo.TASMUHASEBE.ILCEKOD = dbo.TASILCE.KOD INNER JOIN";
        //                //sql += " dbo.TASIL ON dbo.TASILCE.ILKOD = dbo.TASIL.KOD INNER JOIN";
        //                //sql += " dbo.TASSICILNOHAREKET ON dbo.TASAMBAR.MUHASEBEKOD = dbo.TASSICILNOHAREKET.MUHASEBEKOD AND dbo.TASAMBAR.HARCAMABIRIMKOD = dbo.TASSICILNOHAREKET.HARCAMABIRIMKOD AND";
        //                //sql += " dbo.TASAMBAR.KOD = dbo.TASSICILNOHAREKET.AMBARKOD ON dbo.TASSICILNOOZELLIK.PRSICILNO = dbo.TASSICILNOHAREKET.PRSICILNO ON";
        //                //sql += " dbo.TASHESAPPLAN.KOD = dbo.TASSICILNOHAREKET.HESAPPLANKOD LEFT OUTER JOIN";
        //                //sql += " dbo.TASISLEMTIP ON dbo.TASSICILNOHAREKET.ISLEMTIPIKOD = dbo.TASISLEMTIP.KOD LEFT OUTER JOIN";
        //                //sql += " dbo.TASMARKA ON dbo.TASSICILNOOZELLIK.MARKAKOD = dbo.TASMARKA.KOD LEFT OUTER JOIN";
        //                //sql += " dbo.TASMODEL ON dbo.TASSICILNOOZELLIK.MODELKOD = dbo.TASMODEL.KOD";
        //                //sql += " WHERE(dbo.TASSICILNOHAREKET.DURUM <> 2)";
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("DROP VIEW[dbo].[TASSICILHAREKETVIEWHEPSI]");

        //                //sql = sql.Replace("ALTER VIEW dbo.TASSICILHAREKETVIEW", "CREATE VIEW dbo.TASSICILHAREKETVIEWHEPSI"); //Merkez Bankası için eklendi. H.Ö 25.09.2018
        //                //sql = sql.Replace("WHERE(dbo.TASSICILNOHAREKET.DURUM <> 2)", "");
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery(sql);


        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE dbo.TASTALEPKARSILAMADETAY ADD IPTAL int NULL");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASTALEPKARSILAMADETAY ADD REFIDANA varchar(40) NULL, REFID varchar(40) NULL ");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASTALEPKARSILAMA ADD REFID varchar(40) NULL");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASTALEPKARSILAMADETAY ADD BIRIMFIYAT money NULL");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASTALEPKARSILAMA ADD KARSILAYANADISOYADI varchar(150) NULL, KARSILAYANUNVAN varchar(150) NULL");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASTALEPKARSILAMA ADD TALEPEDENADISOYADI varchar(150) NULL,	TALEPEDENUNVAN varchar(150) NULL, GEREKCE varchar(MAX) NULL");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASHESAPPLAN ADD AMORTIYIL int NULL");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASAMORTISMANSINIR ADD MIFBELGENO varchar(1000) NULL");

        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE TASAMORTISMANSINIR ALTER COLUMN MIFBELGENO varchar(1000) NULL");
        //                //ve.hata = "";
        //                //ve.ExecuteNonquery("ALTER TABLE dbo.TASISLEMFORM ADD MUTEMMIM int NULL");
        //            }

        //        }
        //        catch { }
        //        ve.hata = "";
        //        ve.VeriErisimKapat();
        //    }
        //}

        /// <summary>
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
        public static string UzayYaziDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

            string deger = "";
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriStr(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// Gönderilen uzaydan verilen kriterlere uygun olan uzay alan bilgisini çeken yordam
        /// uzayAlan parametresi boş ise varsayılan kullanılır.
        /// </summary>
        /// <param name="uzayAd">Uzay adı</param>
        /// <param name="kosul">Uzaya verilecek koşul bilgisi</param>
        /// <param name="esitle">esitle true ise koşullar koşul alanlarına eşitlenir, false ise like kullanılır.</param>
        /// <param name="uzayAlan">Değeri istenen uzay alanı</param>
        /// <returns>İstenen uzay alanı değeri döndürülür.</returns>
        public static double UzaySayiDegerGetir(string uzayAd, string kosul, bool esitle, string uzayAlan)
        {
            IUZYServis servisUZY = TNS.UZY.Arac.Tanimla();

            double deger = 0;
            if (uzayAlan == "")
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle);
            else
                deger = servisUZY.UzayDegeriDbl(new TNS.KYM.Kullanici(), uzayAd, kosul, esitle, uzayAlan);

            return deger;
        }

        /// <summary>
        /// İşlem tipi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IslemTipKaydet(Kullanici kullanan, IslemTip islemTip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMIslemTip().Kaydet(null, kullanan, islemTip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İşlem tipi, taşınır işlem fişlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IslemTipSil(Kullanici kullanan, IslemTip islemTip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMIslemTip().Sil(null, kullanan, islemTip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan işlem tipi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="islemTip">İşlem tipi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında işlem tipi listesi döndürülür.</returns>
        public ObjectArray IslemTipListele(Kullanici kullanan, IslemTip islemTip)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMIslemTip().Listele(null, kullanan, islemTip);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Ölçü birim varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OlcuBirimKaydet(Kullanici kullanan, OlcuBirim olcuBirim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMOlcuBirim().Kaydet(null, kullanan, olcuBirim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Ölçü birim, taşınır hesap planında kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen OlcuBirim nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OlcuBirimSil(Kullanici kullanan, OlcuBirim olcuBirim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMOlcuBirim().Sil(null, kullanan, olcuBirim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="olcuBirim">Ölçü birim bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        public ObjectArray OlcuBirimListele(Kullanici kullanan, OlcuBirim olcuBirim)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMOlcuBirim().Listele(null, kullanan, olcuBirim);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// MuhasebeIslemTip varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen MuhasebeIslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeIslemTip">MuhasebeIslemTip bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc MuhasebeIslemTipiKaydet(Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMMuhasebeIslemTipi().Kaydet(null, kullanan, muhasebeIslemTip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// MuhasebeIslemTip, taşınır hesap planında kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen MuhasebeIslemTip nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeIslemTip">MuhasebeIslemTip bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc MuhasebeIslemTipiSil(Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMMuhasebeIslemTipi().Sil(null, kullanan, muhasebeIslemTip);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeIslemTip">MuhasebeIslemTip bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ölçü birim listesi döndürülür.</returns>
        public ObjectArray MuhasebeIslemTipiListele(Kullanici kullanan, MuhasebeIslemTipi muhasebeIslemTip)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMMuhasebeIslemTipi().Listele(null, kullanan, muhasebeIslemTip);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }


        /// <summary>
        /// İl varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Il nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlKaydet(Kullanici kullanan, Il il)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMIl().Kaydet(kullanan, il);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan il bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında il listesi döndürülür.</returns>
        public ObjectArray IlListele(Kullanici kullanan, Il il)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMIl().Listele(kullanan, il);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İl, ilçelerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Il nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlSil(Kullanici kullanan, Il il)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMIl().Sil(kullanan, il);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İlçe varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ilce nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlceKaydet(Kullanici kullanan, Ilce ilce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMIlce().Kaydet(kullanan, ilce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ilçe bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ilçe listesi döndürülür.</returns>
        public ObjectArray IlceListele(Kullanici kullanan, Ilce ilce)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMIlce().Listele(kullanan, ilce);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İlçe, muhasebe birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ilce nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ilce">İlçe bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IlceSil(Kullanici kullanan, Ilce ilce)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMIlce().Sil(kullanan, ilce);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Bölge varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Bolge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bolge">Bölge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc BolgeKaydet(Kullanici kullanan, Bolge bolge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMBolge().Kaydet(kullanan, bolge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan bölge bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bolge">Bölge bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bölge listesi döndürülür.</returns>
        public ObjectArray BolgeListele(Kullanici kullanan, Bolge bolge)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMBolge().Listele(kullanan, bolge);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Bölge, muhasebe birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Bolge nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="bolge">Bölge bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc BolgeSil(Kullanici kullanan, Bolge bolge)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMBolge().Sil(kullanan, bolge);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Marka varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Marka nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="marka">Marka bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc MarkaKaydet(Kullanici kullanan, Marka marka)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMMarka().Kaydet(kullanan, marka);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan marka bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="marka">Marka bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında marka listesi döndürülür.</returns>
        public ObjectArray MarkaListele(Kullanici kullanan, Marka marka)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMMarka().Listele(kullanan, marka);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Marka, modellerde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Marka nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="marka">Marka bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc MarkaSil(Kullanici kullanan, Marka marka)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMMarka().Sil(kullanan, marka);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Model varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Model nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="model">Model bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ModelKaydet(Kullanici kullanan, Model model)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMModel().Kaydet(kullanan, model);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan model bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="model">Model bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında model listesi döndürülür.</returns>
        public ObjectArray ModelListele(Kullanici kullanan, Model model)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMModel().Listele(kullanan, model);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Model, taşınır malzemelerin özelliklerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Model nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="model">Model bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ModelSil(Kullanici kullanan, Model model)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMModel().Sil(kullanan, model);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Muhasebe birimi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Muhasebe nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeBilgisi">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc MuhasebeKaydet(Kullanici kullanan, Muhasebe muhasebeBilgisi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMMuhasebe().Kaydet(kullanan, muhasebeBilgisi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Muhasebe birimi, harcama birimlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Muhasebe nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebeBilgisi">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc MuhasebeSil(Kullanici kullanan, Muhasebe muhasebeBilgisi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMMuhasebe().Sil(kullanan, muhasebeBilgisi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan muhasebe birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebe">Muhasebe birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında muhasebe birimi listesi döndürülür.</returns>
        public ObjectArray MuhasebeListele(Kullanici kullanan, Muhasebe muhasebe)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMMuhasebe().Listele(kullanan, muhasebe);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen muhasebe biriminin KDV mükellefi olup olmadığı bilgisini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kod">Muhasebe birimi kodu</param>
        /// <returns>KDV mükellefi olup olmadığı bilgisi döndürülür.</returns>
        public bool MuhasebeKdvMukellefiMi(Kullanici kullanan, string kod)
        {
            bool donusDeger = false;
            try
            {
                donusDeger = new TMMMuhasebe().KdvMukellefiMi(kod);
            }
            catch
            {
                donusDeger = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Harcama birimi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen HarcamaBirimi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HarcamaBirimiKaydet(Kullanici kullanan, HarcamaBirimi harcamaBirimi)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMHarcamaBirimi.Kaydet(null, kullanan, harcamaBirimi);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Harcama birimi, ambarlarda kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen HarcamaBirimi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HarcamaBirimiSil(Kullanici kullanan, HarcamaBirimi harcamaBirimi)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMHarcamaBirimi.Sil(null, kullanan, harcamaBirimi);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi listesi döndürülür.</returns>
        public ObjectArray HarcamaBirimiListele(Kullanici kullanan, HarcamaBirimi harcamaBirimi)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMHarcamaBirimi.Listele(kullanan, harcamaBirimi);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen taşınır hesap planlarını teker teker varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen taşınır hesap planlarından
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlani">Taşınır hesap planlarını tutan nesne</param>
        /// <param name="muhasebat">İşlemin muhasebat formatında excel dosyasından gelip gelmediği bilgisi</param>
        /// <param name="kaydetZorla">Hata olsa da kayıt işlemi yapılsın mı bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HesapPlaniKaydet(Kullanici kullanan, ObjectArray hesapPlani, bool muhasebat, bool kaydetZorla)
        {
            try
            {
                return new TMMHesapPlani().Kaydet(kullanan, hesapPlani, muhasebat, kaydetZorla);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Taşınır hesap planlarını teker teker silen yordam
        /// Eğer, taşınır işlem fişlerinde kullanılmışsa ve/veya detayında başka taşınır hesap
        /// planı varsa ya da kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlani">Taşınır hesap planlarını tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HesapPlaniSil(Kullanici kullanan, ObjectArray hesapPlani)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMHesapPlani().Sil(kullanan, hesapPlani);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc HesapPlaniKullanilmisMi(Kullanici kullanan, HesapPlaniSatir hesapPlani)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMHesapPlani().KullanilmisMi(kullanan, hesapPlani);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap planlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="hesapPlaniSatir">Taşınır hesap planı bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap planları listesi döndürülür.</returns>
        public ObjectArray HesapPlaniListele(Kullanici kullanan, HesapPlaniSatir hesapPlaniSatir, Sayfalama sayfalama)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMHesapPlani().Listele(kullanan, hesapPlaniSatir, sayfalama);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Taşınır hesap planlarının detay bilgilerini düzanleyen yordam
        /// </summary>
        public void HesapPlaniDetaySayilariDuzenle()
        {
            new TMMHesapPlani().HesapPlaniDetaySayilariDuzenle(null);
        }

        /// <summary>
        /// Ambar varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ambar nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AmbarKaydet(Kullanici kullanan, Ambar ambar)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMAmbar.Kaydet(null, kullanan, ambar);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen ambar bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Ambar nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AmbarSil(Kullanici kullanan, Ambar ambar)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMAmbar.Sil(null, kullanan, ambar);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ambar bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ambar listesi döndürülür.</returns>
        public ObjectArray AmbarListele(Kullanici kullanan, Ambar ambar)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMAmbar.Listele(null, kullanan, ambar);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Oda varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OdaKaydet(Kullanici kullanan, Oda oda)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMOda.Kaydet(null, kullanan, oda);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen oda bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OdaSil(Kullanici kullanan, Oda oda)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMOda.Sil(null, kullanan, oda);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan oda bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="oda">Oda bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında oda listesi döndürülür.</returns>
        public ObjectArray OdaListele(Kullanici kullanan, Oda oda)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMOda.Listele(kullanan, oda);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }


        /// <summary>
        /// Silinen ancak kullanımda olan oda bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında oda listesi döndürülür.</returns>
        public ObjectArray OdaListeleSilinenOdalar(Kullanici kullanan)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMOda.ListeleSilinenOdalar(null, kullanan);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Silinen ancak kullanımda olan odalar varsa yeniden oda kodu oluşturan yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Oda nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc OdaKaydetSilinenOdalar(Kullanici kullanan)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMOda.KaydetSilinenOdalar(null, kullanan);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }



        /// <summary>
        /// Personel varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PersonelKaydet(Kullanici kullanan, Personel personel)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMPersonel.Kaydet(null, kullanan, personel);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Personel, zimmet fişlerinde kullanılmamışsa silen, kullanılmışsa uyarı döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc PersonelSil(Kullanici kullanan, Personel personel)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMPersonel.Sil(null, kullanan, personel);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan personel bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Personel bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel listesi döndürülür.</returns>
        public ObjectArray PersonelListele(Kullanici kullanan, Personel personel)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMPersonel.Listele(kullanan, personel);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Taşınır işlem fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Taşınır işlem fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TasinirIslemFisiKaydet(Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMTasinirIslemForm.Kaydet(null, kullanan, tf);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        public Sonuc TasinirIslemFisiUstBilgiGuncelle(Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMTasinirIslemForm.GuncelleUstBilgi(null, kullanan, tf);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="siraNoSirali">Taşınır işlem fişinin detay bilgilerinin satır sırasına göre sıralı olup olmaması bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişi döndürülür.</returns>
        public ObjectArray TasinirIslemFisiAc(Kullanici kullanan, TasinirIslemForm tf, bool siraNoSirali)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMTasinirIslemForm.FisAc(null, kullanan, tf, siraNoSirali);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        public ObjectArray TasinirIslemDetayKaydiGetir(Kullanici kullanan, TasinirIslemForm tf, bool siraNoSirali, int sayfaNo, int kayitSayisi)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMTasinirIslemDetay.Listele(tf, siraNoSirali, sayfaNo, kayitSayisi);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişleri listesi döndürülür.</returns>
        public ObjectArray TasinirIslemFisiListele(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMTasinirIslemForm.Listele(kullanan, tf, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Taşınır işlem fişinin, muhasebe işlem fişi ile bağını varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="belgeNo">Muhasebe işlem fişi belge numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TasinirMIFNoKaydet(Kullanici kullanan, TasinirIslemForm tf, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMTasinirIslemForm.MIFNoKaydet(kullanan, tf, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerinin muhasebe işlem fişleri ile bağlarını listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasinirIslemMIF nesneleri listesi döndürülür.</returns>
        public ObjectArray TasinirMIFNoListele(Kullanici kullanan, TasinirIslemForm tf)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMTasinirIslemForm.MIFNoListele(kullanan, tf);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc TasinirSurecNoKaydet(Kullanici kullanan, TasinirIslemForm tf, string belgeNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMTasinirIslemForm.SurecNoKaydet(kullanan, tf, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem fişlerinin muhasebe işlem fişleri ile bağlarını listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında TasinirIslemMIF nesneleri listesi döndürülür.</returns>
        public ObjectArray TasinirSurecNoListele(TasinirIslemForm tf)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMTasinirIslemForm.SurecNoListele(tf);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc TasinirIslemFisiKHKFisNoGuncelle(Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMTasinirIslemForm.KHKFisNoGuncelle(kullanan, tf);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }


        /// <summary>
        /// Taşınır işlem fişinin durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak taşınır işlem fişine ve detay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - İptal, Onay veya OnayKaldir değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc TasinirIslemFisiDurumDegistir(Kullanici kullanan, TasinirIslemForm tf, string yeniDurum)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMTasinirIslemForm.BelgeDurumDegistir(null, kullanan, tf, yeniDurum);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen taşınır işlem fişinin durum bilgisini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi bilgilerini tutan nesne</param>
        /// <returns>Taşınır işlem fişinin durumu döndürülür.</returns>
        public int TasinirIslemFisiDurumGetir(Kullanici kullanan, TasinirIslemForm tf)
        {
            int durum = (int)ENUMBelgeDurumu.TANIMSIZ;

            try
            {
                durum = TMMTasinirIslemForm.BelgeDurumGetir(null, kullanan, tf);
            }
            catch (Exception)
            {
                durum = (int)ENUMBelgeDurumu.TANIMSIZ;
            }

            return durum;
        }

        /// <summary>
        /// Geçici alındı fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Geçici alındı fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc GeciciAlindiFisiKaydet(Kullanici kullanan, TasinirIslemForm tf)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMGeciciAlindiForm.Kaydet(null, kullanan, tf);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan geçici alındı fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçici alındı fişi döndürülür.</returns>
        public ObjectArray GeciciAlindiFisiAc(Kullanici kullanan, TasinirIslemForm tf)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMGeciciAlindiForm.FisAc(null, kullanan, tf);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan geçici alındı fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen TasinirIslemForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Geçici alındı fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçici alındı fişleri listesi döndürülür.</returns>
        public ObjectArray GeciciAlindiFisiListele(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMGeciciAlindiForm.Listele(kullanan, tf, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Zimmet fişini varsa güncelleyen, yoksa kaydeden yordam
        /// Zimmet fişi ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm ve ObjectArray nesneleri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="zimmetDetay">Zimmet fişinin detay bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ZimmetFisiKaydet(Kullanici kullanan, ZimmetForm zimmet, ObjectArray zimmetDetay)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMZimmetForm.Kaydet(null, kullanan, zimmet, zimmetDetay);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Zimmet fişinin durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak zimmet fişine ve detay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zf">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - İptal, Onay veya OnayKaldir değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ZimmetFisiDurumDegistir(Kullanici kullanan, ZimmetForm zf, string yeniDurum)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMZimmetForm.BelgeDurumDegistir(null, kullanan, zf, yeniDurum);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Zimmet fişi EOnay durumunu güncelleyen yordam
        /// Parametre olarak verilen yeniDurum bilgisine bakarak zimmet fişi eonay bilgilerine bir takım
        /// kontroller uygular, verilen fiş bilgileri bütün kontrolleri sorunsuz geçerse belgeye yeni durum atanır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetFormEOnay nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zf">Zimmet fişi eonay bilgilerini tutan nesne</param>
        /// <param name="yeniDurum">Belgeye atanması istenen yeni durum bilgisi - Gönderildi, Cevaplandı KABUL, Cevaplandı RED değerlerini alabilir.</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ZimmetFisiEOnayDurumDegistir(Kullanici kullanan, ZimmetFormEOnay zfeonay, string yeniDurum, string personelKod)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMZimmetFormEOnay.BelgeEOnayDurumDegistir(null, kullanan, zfeonay, yeniDurum, personelKod);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        public Sonuc ZimmetDusVer(Kullanici kullanan, ZimmetForm zimmet, TNSCollection zimmetDetay, string verilecekPersonelKod, string verilecekOdaKod, ENUMZimmetBelgeTur verilecekBelgeTur)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMZimmetForm.ZimmetDusVer(null, kullanan, zimmet, zimmetDetay, verilecekPersonelKod, verilecekOdaKod, verilecekBelgeTur);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Zimmet fişi EOnay varsa güncelleyen, yoksa kaydeden yordam
        /// Zimmet fişi EOnay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetFormEOnay ve ObjectArray nesneleri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmetEOnay">Zimmet fişi eonay bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ZimmetFisiEOnayKaydet(Kullanici kullanan, ZimmetFormEOnay zfeonay)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMZimmetFormEOnay.Kaydet(null, kullanan, zfeonay);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fiş eonay bilgisini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetFormEOnay nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zfeonay">Zimmet fişi eonay bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        public ObjectArray ZimmetFisiEOnayListele(Kullanici kullanan, ZimmetFormEOnay zfeonay, ZimmetFormEOnayKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMZimmetFormEOnay.Listele(null, kullanan, zfeonay, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Aktif olarak ambarda olan ve verilen kriterlere uygun olan demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public ObjectArray SicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilNoHareket.AmbardakiSicilNolar(null, kullanan, sicilNoHareket);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        public ZimmetForm ZimmetVarMi(Kullanici kullanan, string gorSicilNo)
        {
            ZimmetForm bilgi = new ZimmetForm();

            try
            {
                bilgi = TMMZimmetForm.ZimmetVarMi(null, kullanan, gorSicilNo);
            }
            catch
            {
            }

            return bilgi;
        }

        /// <summary>
        /// Sicils the no listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sicilNoHareket">The sicil no hareket.</param>
        /// <param name="ilkKayit">The ilk kayit.</param>
        /// <param name="kayitSay">The kayit say.</param>
        /// <returns></returns>
        public ObjectArray SicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket, int ilkKayit, int kayitSay)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilNoHareket.AmbardakiSicilNolar(null, kullanan, sicilNoHareket, ilkKayit, kayitSay);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Ambarda veya zimmette ayırdetmeksizin kriterlere uygun olan bütün demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public ObjectArray ButunSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilNoHareket.AmbardakiButunSicilNolar(null, kullanan, sicilNoHareket);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Butuns the sicil no listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sicilNoHareket">The sicil no hareket.</param>
        /// <param name="sayfa">The ilk kayit.</param>
        /// <returns></returns>
        public ObjectArray ButunSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket, Sayfalama sayfa)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilNoHareket.AmbardakiButunSicilNolar(null, kullanan, sicilNoHareket, sayfa);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Zimmette olan ve verilen kriterlere uygun olan demirbaşları listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public ObjectArray ZimmetliSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilNoHareket.ZimmetliSicilNolar(null, kullanan, sicilNoHareket);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Ambarda veya zimmette ayırdetmeksizin kriterlere uygun olan bütün demirbaşları listeleyen yordam
        /// Kriterlerde personel kriteri de varsa sadece zimmetleri listeler.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public ObjectArray BarkodSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNoHareket, Sayfalama sayfa)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilNoHareket.BarkodSicilNolar(null, kullanan, sicilNoHareket, sayfa);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ve aktif olarak ambarda olan henüz tüketilmemiş sarf malzemelerini listeleyen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="shBilgi">Tüketim malzemeleri kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemeleri listesi döndürülür.</returns>
        public ObjectArray TuketimListele(Kullanici kullanan, StokHareketBilgi shBilgi)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMStokHareket.AmbardakiStoklariGetir(null, kullanan, shBilgi);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişi döndürülür.</returns>
        public ObjectArray ZimmetFisiAc(Kullanici kullanan, ZimmetForm zimmet)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMZimmetForm.FisAc(null, kullanan, zimmet);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen zimmet fişinin detay bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişinin detay bilgileri döndürülür.</returns>
        public ObjectArray ZimmetFisiDetayListele(Kullanici kullanan, ZimmetForm zimmet)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMZimmetForm.ListeleDetay(null, kullanan, zimmet);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <param name="eOnayDurum"> Zimmet belgelerinin eposta ile gönderilip onaylanması için eklenildi. null ise kullanılmıyor</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesi döndürülür.</returns>
        public ObjectArray ZimmetFisiListele(Kullanici kullanan, ZimmetForm zimmet, ZimmetFormKriter kriter, string eOnayDurum)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMZimmetForm.Listele(null, kullanan, zimmet, kriter, eOnayDurum);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan zimmet fişlerini eonay bilgileri ile döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="zimmet">Zimmet fişi bilgilerini tutan nesne</param>
        /// <param name="kriter">Sorgulama için ek kriter bilgilerini tutan nesne</param>
        /// <param name="eonaykriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesini eonay bilgileri ile döndürülür.</returns>
        public ObjectArray ZimmetFisiListeleEOnayBilgisiyle(Kullanici kullanan, ZimmetForm zimmet, ZimmetFormKriter kriter, ZimmetFormEOnayKriter eonaykriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMZimmetForm.ListeleEOnayBilgisiyle(null, kullanan, zimmet, kriter, eonaykriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kisi/ortak alan zimmet fişlerini eonay bilgileri ile döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ZimmetForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kriter"> Sorgulama için eonay kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişleri listesini eonay bilgileri ile döndürülür.</returns>
        public ObjectArray ZimmetFisiListeleKisiOrtakEOnayBilgisiyle(ZimmetFormEOnayKriter eonaykriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMZimmetFormEOnay.ListeleKisiOrtakEOnayBilgisiyle(null, eonaykriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Sayım tutanağı formunu varsa güncelleyen, yoksa kaydeden yordam
        /// Sayım tutanağı formu ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formunun bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SayimKaydet(Kullanici kullanan, SayimForm sf)
        {
            try
            {
                return TMMSayimForm.Kaydet(null, kullanan, sf);
            }
            catch (Exception e)
            {
                Sonuc sonuc = new Sonuc();
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
                return sonuc;
            }
        }

        /// <summary>
        /// Parametre olarak verilen sayım tutanağı formunu ve detayını silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formunun bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SayimSil(Kullanici kullanan, SayimForm sf)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMSayimForm.Sil(null, kullanan, sf);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan sayım tutanağı formlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formu bilgilerini tutan nesne</param>
        /// <param name="detayVer">Sayım tutanağı formlarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sayım tutanağı formları listesi döndürülür.</returns>
        public ObjectArray SayimListele(Kullanici kullanan, SayimForm sf, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMSayimForm.Listele(null, kullanan, sf, detayVer, false);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan sayım tutanağı formlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı formu bilgilerini tutan nesne</param>
        /// <param name="detayVer">Sayım tutanağı formlarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında sayım tutanağı formları listesi döndürülür.</returns>
        public ObjectArray SayimRaporListele(Kullanici kullanan, SayimForm sf, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMSayimForm.Listele(null, kullanan, sf, detayVer, true);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ambardaki tüketim malzemelerini,
        /// demirbaşları ve zimmetli demirbaşları döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sf">Sayım tutanağı kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ambardaki malzemelerin detay bilgileri listesi döndürülür.</returns>
        public ObjectArray SayimAmbardakileriGetir(Kullanici kullanan, SayimForm sf)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMSayimForm.AmbardakileriGetir(null, kullanan, sf, true);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Taşınır istek formunu varsa güncelleyen, yoksa kaydeden yordam
        /// Taşınır istek formu ve detay bilgileri ile ilgili bir takım kontroller yapılmakta,
        /// sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi
        /// yapılmaktadır, aksi takdirde işlem yapılmaz ve hata döndürülür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="iForm">Taşınır istek formu bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc IstekKaydet(Kullanici kullanan, IstekForm iForm)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMIstekForm.Kaydet(null, kullanan, iForm);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır istek formlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen IstekForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="iForm">Taşınır istek formu bilgilerini tutan nesne</param>
        /// <param name="detayVer">Taşınır istek formlarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır istek formları listesi döndürülür.</returns>
        public ObjectArray IstekListele(Kullanici kullanan, IstekForm iForm, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMIstekForm.Listele(null, kullanan, iForm, detayVer);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Kayıttan düşme teklif ve onay tutanağını varsa güncelleyen, yoksa kaydeden yordam
        /// Kayıttan düşme teklif ve onay tutanağı ve detay bilgileri ile ilgili bir takım kontroller
        /// yapılmakta, sorunsuz bir bilgi kümesi olduğuna karar verilirse kayıt işlemi yapılmaktadır.
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağı bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KayittanDusmeKaydet(Kullanici kullanan, KayittanDusmeForm kf)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMKayittanDusmeFormClass.Kaydet(null, kullanan, kf);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kayıttan düşme teklif ve onay tutanaklarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağı bilgilerini tutan nesne</param>
        /// <param name="detayVer">Kayıttan düşme teklif ve onay tutanaklarının detay bilgilerinin istenip istenmediği bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kayıttan düşme teklif ve onay tutanakları listesi döndürülür.</returns>
        public ObjectArray KayittanDusmeListele(Kullanici kullanan, KayittanDusmeForm kf, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMKayittanDusmeFormClass.Listele(null, kullanan, kf, detayVer);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen kayıttan düşme teklif ve onay tutanağını ve detaylarını silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen KayittanDusmeForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kf">Kayıttan düşme teklif ve onay tutanağının bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KayittanDusmeSil(Kullanici kullanan, KayittanDusmeForm kf)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMKayittanDusmeFormClass.Sil(null, kullanan, kf);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan dayanıklı taşınır defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Dayanıklı taşınır defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında dayanıklı taşınır defteri listesi döndürülür.</returns>
        public ObjectArray DayanikliTasinirlarDefteriOlustur(Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMRaporlar.DTDOlustur(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan tüketim malzemeleri defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Tüketim malzemeleri defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemeleri defteri listesi döndürülür.</returns>
        public ObjectArray TuketimMalzemeDefteriOlustur(Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TMDOlustur(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan müze defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Müze defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında müze defteri listesi döndürülür.</returns>
        public ObjectArray MuzeDefteriOlustur(Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMRaporlar.MuzeDefteriOlustur(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kütüphane defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Kütüphane defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kütüphane defteri listesi döndürülür.</returns>
        public ObjectArray KutuphaneDefteriOlustur(Kullanici kullanan, DefterKriter kriter)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMRaporlar.KutuphaneDefteriOlustur(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır sayım ve döküm cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır sayım ve döküm cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır sayım ve döküm cetveli listesi döndürülür.</returns>
        public ObjectArray TasinirSayimCetveli(Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirSayimCetveli(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi taşınır yönetim hesabı cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Harcama birimi taşınır yönetim hesabı cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi taşınır yönetim hesabı cetveli listesi döndürülür.</returns>
        public ObjectArray TasinirYonetimHesabiCetveli(Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirYonetimHesabiCetveli(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli listesi döndürülür.</returns>
        public ObjectArray TasinirHesapCetveli(Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirHesapCetveli(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli (bölge) bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli (bölge) kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli (bölge) listesi döndürülür.</returns>
        public ObjectArray TasinirHesapCetveliBolge(Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirHesapCetveliBolge(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır hesap cetveli (il) bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır hesap cetveli (il) kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap cetveli (il) listesi döndürülür.</returns>
        public ObjectArray TasinirHesapCetveliIl(Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirHesapCetveliIl(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır kesin hesap cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır kesin hesap cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır kesin hesap cetveli listesi döndürülür.</returns>
        public ObjectArray TasinirKesinHesapCetveli(Kullanici kullanan, Cetvel kriter, bool muhasebeEkle)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirKesinHesapCetveli(null, kullanan, kriter, muhasebeEkle);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır kesin hesap icmal cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır kesin hesap icmal cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır kesin hesap icmal cetveli listesi döndürülür.</returns>
        public ObjectArray TasinirKesinHesapIcmalCetveli(Kullanici kullanan, Cetvel kriter, bool muhasebeEkle)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirKesinHesapIcmalCetveli(null, kullanan, kriter, muhasebeEkle);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan müze/kütüphane yönetim hesabı cetveli bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Cetvel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Müze/Kütüphane yönetim hesabı cetveli kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında müze/kütüphane yönetim hesabı cetveli listesi döndürülür.</returns>
        public ObjectArray MuzeKutuphaneCetveli(Kullanici kullanan, Cetvel kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.MuzeKutuphaneCetveli(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen firmaları varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="firmaBilgileri">Firma bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FirmaKaydet(ObjectArray firmaBilgileri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMFirma().Kaydet(firmaBilgileri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen firmaları silen yordam
        /// </summary>
        /// <param name="firmaBilgileri">Firma bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc FirmaSil(ObjectArray firmaBilgileri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMFirma().Sil(firmaBilgileri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan firma bilgilerini döndüren yordam
        /// </summary>
        /// <param name="firma">Firma bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında firma listesi döndürülür.</returns>
        public ObjectArray FirmaListele(FirmaBilgisi firma)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMFirma().Listele(firma);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Yetkili imza bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen ImzaBilgisi nesnelerinden
        /// herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="imzaBilgileri">Yetkili imza bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ImzaKaydet(Kullanici kullanan, ObjectArray imzaBilgileri)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMImza().Kaydet(kullanan, imzaBilgileri);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan yetkili imza bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="birim">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="imzaYer">ENUMImzaYer listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında yetkili imza bilgileri listesi döndürülür.</returns>
        public ObjectArray ImzaListele(Kullanici kullanan, string muhasebe, string birim, string ambar, int imzaYer)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMImza().Listele(kullanan, muhasebe, birim, ambar, imzaYer);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen taşınır işlem fişine ait işlem tarihçesi bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem fişine ait işlem tarihçesi listesi döndürülür.</returns>
        public ObjectArray IslemFormTarihceListele(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string belgeNo)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMTarihce().IslemFormListele(kullanan, yil, muhasebe, harcamaBirimi, belgeNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen zimmet fişine ait işlem tarihçesi bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="belgeNo">Belge numarası</param>
        /// <param name="belgeTur">ENUMZimmetBelgeTur listesindeki değerlerden biri olmalıdır.</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında zimmet fişine ait işlem tarihçesi listesi döndürülür.</returns>
        public ObjectArray ZimmetTarihceListele(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string belgeNo, int belgeTur)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMTarihce().ZimmetListele(kullanan, yil, muhasebe, harcamaBirimi, belgeNo, belgeTur);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        public ObjectArray IslemFormTarihceRapor(Kullanici kullanan, string muhasebe, string harcamaBirimi, TNSDateTime baslamaTarihi, TNSDateTime bitisTarihi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMTarihce().Rapor(kullanan, muhasebe, harcamaBirimi, baslamaTarihi, bitisTarihi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
            }

            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kullanarak ilgili demirbaşa ait hareketleri bulan yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNoHareket">Demirbaş kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşa ait hareket bilgileri listesi döndürülür.</returns>
        public ObjectArray SicilNoTarihceListele(Kullanici kullanan, SicilNoHareket sicilNoHareket)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilNoHareket.SicilNoTarihce(null, kullanan, sicilNoHareket);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşlara ait özellik bilgileri varsa güncelleyen, yoksa kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="sicilNoOzellik">Demirbaşlara ait özellik bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SicilNoOzellikKaydet(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, ObjectArray sicilNoOzellik)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = new TMMSicilNoOzellik().Kaydet(kullanan, yil, muhasebe, harcamaBirimi, ambar, sicilNoOzellik);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşa ait özellik bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="prSicilNo">Demirbaşın programatik sicil numarası</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşın özellik bilgileri döndürülür.</returns>
        public ObjectArray SicilNoOzellikListele(Kullanici kullanan, int prSicilNo)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMSicilNoOzellik.Listele(null, kullanan, prSicilNo);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşlara ait özellik bilgilerini silen yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi kodu</param>
        /// <param name="harcamaBirimi">Harcama birimi kodu</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <param name="prSicilNo">Demirbaş listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SicilNoOzellikSil(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, int[] prSicilNo)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = new TMMSicilNoOzellik().Sil(kullanan, yil, muhasebe, harcamaBirimi, ambar, prSicilNo);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan malzemelerin ortalama birim fiyat ve
        /// toplam miktar bilgilerini taşınır hesap planı bazında döndüren yordam
        /// hesapKod parametresinde "," ayracı kullanılarak birden fazla taşınır hesap kodu verilebilir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="harcamaBirimKod">Harcama birimi kodu</param>
        /// <param name="hesapKod">Taşınır hesap planı kodu/kodları bilgisi</param>
        /// <param name="sadeceRakamOlanlar">Taşınır hesap planı listesinin tamamı mı yoksa sadece rakam olanlar mı listelensin bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır hesap planları, ortalama birim fiyatları ve toplam miktarları listesi döndürülür.</returns>
        public ObjectArray HesapPlaniOrtalamaFiyatMiktarListele(Kullanici kullanan, int yil, string harcamaBirimKod, string hesapKod, bool sadeceRakamOlanlar)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.HesapPlaniOrtalamaFiyatMiktarListele(null, kullanan, yil, harcamaBirimKod, hesapKod, sadeceRakamOlanlar);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen yılda harcama birimlerinin kesmiş
        /// oldukları son taşınır işlem fişi numaralarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="islemTarihIlk"></param>
        /// <param name="islemTarihSon"></param>
        /// <returns>
        /// Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi bazında taşınır işlem fişi numaraları listesi döndürülür.
        /// </returns>
        public ObjectArray HarcamaBirimiSonTIFListele(Kullanici kullanan, int yil, TNSDateTime islemTarihIlk, TNSDateTime islemTarihSon)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.HarcamaBirimiSonTIFListele(null, kullanan, yil, islemTarihIlk, islemTarihSon);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public ObjectArray TasinirDepoDurumu(Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirDepoDurumu(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        //public ObjectArray TasinirDepoDurumuKismi(Kullanici kullanan, DepoDurum kriter)
        //{
        //    ObjectArray bilgi = new ObjectArray();
        //    try
        //    {
        //        bilgi = TMMRaporlar.TasinirDepoDurumuKismi(null, kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        bilgi.sonuc.islemSonuc = false;
        //        bilgi.sonuc.hataStr = e.ToString();
        //    }
        //    return bilgi;
        //}

        /// <summary>
        /// Verilen TuketimCikis kriterlerine uygun olan tüketim malzemelerinin
        /// dönemsel kullanım miktarları bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">TuketimCikis listeleme kriterleri</param>
        /// <param name="fark">Tüketim fark raporu mu, yoksa tüketim raporu mu hazırlansın bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında tüketim malzemelerinin dönemsel kullanım bilgileri listesi döndürülür.</returns>
        public ObjectArray TuketimCikis(Kullanici kullanan, TuketimCikis kriter, bool fark)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TuketimCikis(null, kullanan, kriter, fark);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ortak alan zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ortak alan zimmet bilgileri listesi döndürülür.</returns>
        public ObjectArray ZimmetOrtakAlan(Kullanici kullanan, ZimmetOrtakAlanVeKisi kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.ZimmetOrtakAlan(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kişi zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <param name="kontrolYap">Yetki kontrolü yapılsın mı yapılmasın mı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kişi zimmet bilgileri listesi döndürülür.</returns>
        public ObjectArray ZimmetKisi(Kullanici kullanan, ZimmetOrtakAlanVeKisi kriter, bool kontrolYap)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.ZimmetKisi(null, kullanan, kriter, kontrolYap);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan kişi zimmet bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Zimmet listeleme kriterleri</param>
        /// <param name="kontrolYap">Yetki kontrolü yapılsın mı yapılmasın mı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında kişi zimmet bilgileri listesi döndürülür.</returns>
        //public ObjectArray ZimmetKisideki(Kullanici kullanan, ZimmetForm kriter)
        //{
        //    ObjectArray bilgi = new ObjectArray();
        //    try
        //    {
        //        bilgi = TMMRaporlar.ZimmetKisideki(null, kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        bilgi.sonuc.islemSonuc = false;
        //        bilgi.sonuc.hataStr = e.ToString();
        //    }
        //    return bilgi;
        //}

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public ObjectArray TasinirIslemMalzemeTarihceRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirIslemMalzemeTarihceRaporu(null, kullanan, tf, tfKriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Demirbaş listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş bilgileri listesi döndürülür.</returns>
        public ObjectArray SicilRaporu(Kullanici kullanan, SicilNoHareket kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.SicilRaporu(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan girişi yapılmamış devir çıkış belgeleri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında girişi yapılmamış devir çıkış belgeleri listesi döndürülür.</returns>
        public ObjectArray GirisiYapilmamisDevirCikislari(Kullanici kullanan, TasinirIslemForm kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.GirisiYapilmamisDevirCikislari(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Parametre olarak verilen SQL cümlesini çalıştırıp sonucunu döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sql">Çalıştırılacak SQL cümlesi</param>
        /// <returns>Çalıştırılan SQL cümlesi sonucunda gelen bilgi formatlanarak döndürülür.</returns>
        public string SQLCalistir(Kullanici kullanan, string baglanti, string sql)
        {
            try
            {
                return Genel.SQLCalistir(kullanan, baglanti, sql);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// Verilen KurusFarki kriterlerine uygun olan taşınır malzemelerinin
        /// kuruş fark tutarları bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">KurusFarki listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin kuruş fark tutarları listesi döndürülür.</returns>
        public ObjectArray KurusFarklariRaporu(Kullanici kullanan, KurusFarki kriter)
        {
            try
            {
                return TMMRaporlar.KurusFarklariRaporu(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray bilgi = new ObjectArray();
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
                return bilgi;
            }
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren, çıkan, önceki
        /// yılın tüketim, dönemsel tüketim miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public ObjectArray EnCokKullanilanTasinirlar(Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.EnCokKullanilanTasinirlar(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan personel sayılarını istenen bazda döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="dp">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel sayıları listesi döndürülür.</returns>
        public ObjectArray PersonelSayilari(Kullanici kullanan, DepoDurum dp)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMPersonel.PersonelSayilari(kullanan, dp);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Harcama birimi değişikliğini yapan yordam
        /// Bu yordam, bütün veri tabanındaki kayıtları gezer ve harcama birimi geçen verileri günceller.
        /// Gönderilen bilgiler eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimiEski">Eski harcama birimi bilgisini tutan nesne</param>
        /// <param name="harcamaBirimiYeni">Yeni harcama birimi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HarcamaBirimDegistir(Kullanici kullanan, HarcamaBirimi harcamaBirimiEski, HarcamaBirimi harcamaBirimiYeni)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMHarcamaBirimDegistir.Degistir(null, kullanan, harcamaBirimiEski, harcamaBirimiYeni);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır form ve detay bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AcikPazarForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AcikPazarKaydet(Kullanici kullanan, AcikPazarForm apf)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMAcikPazarForm.KaydetAcikPazar(null, kullanan, apf);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır form, detay ve istek bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AcikPazarForm nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AcikPazarSil(Kullanici kullanan, AcikPazarForm apf)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMAcikPazarForm.SilAcikPazar(null, kullanan, apf, false);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ihtiyaç fazlası taşınır bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <param name="sorgudan">İsteğin sorgu ekranından mı yoksa form ekranından mı yapıldığı bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ihtiyaç fazlası taşınır bilgileri listesi döndürülür.</returns>
        public ObjectArray AcikPazarListele(Kullanici kullanan, AcikPazarForm apf, bool sorgudan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMAcikPazarForm.ListeleAcikPazar(null, kullanan, apf, sorgudan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// İhtiyaç fazlası taşınır malzemelerine yapılan istek bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen bilgiler eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="apf">İhtiyaç fazlası taşınır bilgilerini tutan nesne</param>
        /// <param name="istekler">İhtiyaç fazlası taşınır malzemelerine yapılan istek listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AcikPazarIstekKaydet(Kullanici kullanan, AcikPazarForm apf, TNSCollection istekler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMAcikPazarForm.KaydetAcikPazarIstek(null, kullanan, apf, istekler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ihtiyaç fazlası taşınır istek bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="istek">İhtiyaç fazlası taşınır istek bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında ihtiyaç fazlası taşınır istek bilgileri listesi döndürülür.</returns>
        public ObjectArray AcikPazarIstekListele(Kullanici kullanan, AcikPazarIstek istek)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMAcikPazarForm.ListeleAcikPazarIstek(null, kullanan, istek);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır istek ve karşılanan miktarlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Taşınır istek listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır istek bilgileri listesi döndürülür.</returns>
        public ObjectArray IstekKarsilananRaporu(Kullanici kullanan, IstekForm kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMRaporlar.IstekKarsilanmaListele(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır malzemelerinin giren, çıkan, kalan tutarlarını döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin tutar bilgileri listesi döndürülür.</returns>
        public ObjectArray MuhasebatKontrolRaporu(Kullanici kullanan, MuhasebatKontrol kriter)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMRaporlar.MuhasebatKontrolRaporu(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Amortisman sınırı varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanSinir nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sinir">Amortisman sınır bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AmortismanSinirKaydet(Kullanici kullanan, AmortismanSinir sinir)
        {
            try
            {
                return TMMAmortismanSinir.Kaydet(null, kullanan, sinir);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        /// <summary>
        /// Parametre olarak verilen yılın amortisman sınırını döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında amortisman sınır bilgileri döndürülür.</returns>
        public ObjectArray AmortismanSinirListele(Kullanici kullanan, int yil)
        {
            try
            {
                return TMMAmortismanSinir.Listele(null, kullanan, yil);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.DegerAta(e.ToString());
                return donusDeger;
            }
        }

        /// <summary>
        /// Verilen demirbaş sicil ve RFID eşleşme bilgilerini teker teker varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen eşleşmelerden herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="rfIdler">Demirbaş sicil ve RFID eşleşme bilgileri listesini tutan nesne</param>
        /// <param name="webServis">Eşleşme bilgileri web servisten mi geliyor yoksa başka bir arayüzden mi bilgisi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SicilRFIDKaydet(Kullanici kullanan, TNSCollection rfIdler, bool webServis)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMSicilRFID.Kaydet(null, kullanan, rfIdler, webServis);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SicilRFIDKaydet(Kullanici kullanan, ObjectArray rfIdler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMSicilRFIDStr.Kaydet(null, kullanan, rfIdler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public ObjectArray SicilRFIDListele(Kullanici kullanan, string rfIdNo, int prSicilNo)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilRFIDStr.Listele(null, kullanan, rfIdNo, prSicilNo);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Demirbaş sicil ve RFID eşleşme bilgilerini teker teker silen yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="rfIdler">Demirbaş sicil ve RFID eşleşme bilgileri listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc SicilRFIDSil(Kullanici kullanan, TNSCollection rfIdler)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMSicilRFID.Sil(null, kullanan, rfIdler);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş sicil ve RFID eşleşme bilgilerini döndüren yordam
        /// Eşleşme işlemi daha önceden yapılmış olsun veya olmasın bütün demirbaşlar listelenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Listeleme kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public ObjectArray SicilListeleRFIDHepsi(Kullanici kullanan, SicilNoHareket sicilNo, int ilkKayit = 0, int kayitSay = 0)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilRFID.SicilListele(null, kullanan, sicilNo, false, false, ilkKayit, kayitSay);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş sicil ve RFID eşleşme bilgilerini döndüren yordam
        /// Eşleşme işlemi daha önceden yapılmış demirbaşlar listelenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Listeleme kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public ObjectArray SicilListeleRFIDEslesen(Kullanici kullanan, SicilNoHareket sicilNo, int ilkKayit = 0, int kayitSay = 0)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilRFID.SicilListele(null, kullanan, sicilNo, true, false, ilkKayit, kayitSay);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaş sicil ve RFID eşleşme bilgilerini döndüren yordam
        /// Eşleşme işlemi daha önceden yapılmamış demirbaşlar listelenir.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicilNo">Listeleme kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaş listesi döndürülür.</returns>
        public ObjectArray SicilListeleRFIDEslesmeyen(Kullanici kullanan, SicilNoHareket sicilNo, int ilkKayit = 0, int kayitSay = 0)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilRFID.SicilListele(null, kullanan, sicilNo, false, true, ilkKayit, kayitSay);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Bilgi verilecek personele ait iletişim bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KapiGecisSmsKaydet(Kullanici kullanan, Personel personel)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMKapiGecisSms.Kaydet(null, kullanan, personel);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen Personel nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">İletişim bilgileri silinecek personele ait bilgileri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KapiGecisSmsSil(Kullanici kullanan, Personel personel)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMKapiGecisSms.Sil(null, kullanan, personel);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler kurumdan çıkarılacak
        /// olursa bilgilendirilecek personele ait iletişim bilgilerini döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="personel">Listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında personel iletişim bilgileri listesi döndürülür.</returns>
        public ObjectArray KapiGecisSmsListele(Kullanici kullanan, Personel personel)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMKapiGecisSms.Listele(null, kullanan, personel);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemeleri kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen demirbaşlardan herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siciller">Geçişine izin verilen demirbaşların listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KapiGecisSicilKaydet(Kullanici kullanan, TNSCollection siciller)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMKapiGecisSicil.Kaydet(null, kullanan, siciller);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemelerin izinlerini silen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen demirbaşlardan herhangi biri eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="siciller">Geçiş izni silinen demirbaşların listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KapiGecisSicilSil(Kullanici kullanan, TNSCollection siciller)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMKapiGecisSicil.Sil(null, kullanan, siciller);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilen malzemelerin verilen kriterlere uyanlarını listeleyen yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SicilNoHareket nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sicil">Geçişine izin verilen demirbaşları listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında geçişine izin verilen malzeme bilgileri listesi döndürülür.</returns>
        public ObjectArray KapiGecisSicilListele(Kullanici kullanan, SicilNoHareket sicil)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMKapiGecisSicil.SicilListele(null, kullanan, sicil);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak verilen demirbaşın RFID entegrasyonunda kurumdan
        /// çıkışına izin verilip verilmediği bilgisini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="prSicilNo">Demirbaşın programatik sicil numarası</param>
        /// <param name="rfIdNo">Demirbaşın RFID numarası</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KapiGecisSicilGecebilirMi(Kullanici kullanan, int prSicilNo, int rfIdNo)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMKapiGecisSicil.SicilGecebilirMi(null, kullanan, prSicilNo, rfIdNo);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// RFID entegrasyonunda kurumdan çıkışına izin verilmeyen malzemeler
        /// kurumdan çıkarılacak olursa bilgilendirme loglarını kaydeden yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kgller">Bilgilendirme logları listesini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc KapiGecisLogKaydet(Kullanici kullanan, TNSCollection kgller)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMKapiGecisLog.Kaydet(null, kullanan, kgller);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan bilgilendirme loglarını döndüren yordam
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kgl">Bilgilendirme logları listeleme kriterleri</param>
        /// <param name="tarih1">Tarih aralığının alt sınırı</param>
        /// <param name="tarih2">Tarih aralığının üst sınırı</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında bilgilendirme logları listesi döndürülür.</returns>
        public ObjectArray KapiGecisLogListele(Kullanici kullanan, KapiGecisLog kgl, TNSDateTime tarih1, TNSDateTime tarih2)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMKapiGecisLog.Listele(null, kullanan, kgl, tarih1, tarih2);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc RFIDSayimBaslat(Kullanici kullanan, SayimRFID srfid)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMSayimRFID.YeniSayimBaslat(null, kullanan, srfid);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// RFID sayım bilgilerini kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen SayimRFID nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="srfid">RFID sayım bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc RFIDSayimKaydet(Kullanici kullanan, SayimRFID srfid, bool detaySifirla)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMSayimRFID.Kaydet(null, kullanan, srfid, detaySifirla);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan RFID sayım bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="srfid">RFID sayım kriterlerini tutan nesne</param>
        /// <param name="detay">RFID sayımının detay bilgileri listelensin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında RFID sayım bilgileri listesi döndürülür.</returns>
        public ObjectArray RFIDSayimListele(Kullanici kullanan, SayimRFID srfid, bool detay)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = TMMSayimRFID.Listele(null, kullanan, srfid, detay);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen yıl, muhasebe birimi ve harcama birimi bazında son taşınır işlem
        /// fişi iptal edilmişse belge numarasını, değilse boşluk döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcamaKod">Harcama birimi</param>
        /// <returns>Belge numarası döndürülür.</returns>
        public string HarcamaYonetimiIcinTIFNoVer(int yil, string muhasebe, string harcamaKod)
        {
            return TMMTasinirIslemForm.HarcamaYonetimiIcinTIFNoVer(yil, muhasebe, harcamaKod);
        }

        /// <summary>
        /// Parametre olarak gönderilen kullanıcının muhasebe-harcama-ambar üçlüsü
        /// üzerinde yazma yetkisine sahip olup olmadığının kontrolünü yapan yordam
        /// </summary>
        /// <param name="kullanan">Yetki kontrolü yapılacak kullanıcı</param>
        /// <param name="muhasebe">Muhasebe birimi</param>
        /// <param name="harcama">Harcama birimi</param>
        /// <param name="ambar">Ambar kodu</param>
        /// <returns>Yetki kontrolünün sonucu döndürülür.</returns>
        public bool YazmaYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return TMMYetkiKontrol.YazmaYetkisiVarMi(kullanan, muhasebe, harcama, ambar);
        }

        /// <summary>
        /// Tekrars the eden sicilleri bul.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sicilNo">The sicil no.</param>
        /// <returns></returns>
        public ObjectArray TekrarEdenSicilleriBul(Kullanici kullanan, SicilNoHareket sicilNo)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMSicilTekrarEden.TekrarEdenSicilleriBul(null, kullanan, sicilNo);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Sicils the no guncelle.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="sicilNolar">The sicil nolar.</param>
        /// <returns></returns>
        public Sonuc SicilNoGuncelle(Kullanici kullanan, SicilNoHareket[] sicilNolar)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMSicilTekrarEden.SicilNoGuncelle(null, kullanan, sicilNolar);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        /// <summary>
        /// Taleps the karsilama kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tk">The tk.</param>
        /// <returns></returns>
        public Sonuc TalepKarsilamaKaydet(Kullanici kullanan, TalepKarsilama tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilama.Kaydet(null, kullanan, tk, false);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc TalepKarsilamaDurumGuncelle(Kullanici kullanan, string refId, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilama.DurumGuncelle(null, kullanan, refId, durumKod);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc TalepDetayIptal(Kullanici kullanan, string urunRefId)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilamaDetay.Iptal(null, kullanan, urunRefId);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc TalepKarsilamaCevapKaydet(Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilama.KaydetCevap(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc TalepKarsilamaCevapSil(Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilama.SilCevap(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public ObjectArray TalepKarsilamaCevapListele(Kullanici kullanan, TalepKarsilamaCevap tk, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMTalepKarsilama.ListeleCevap(null, kullanan, tk, detayVer);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public ObjectArray TalepKarsilamaCevapListeleSatinAlma(Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMTalepKarsilama.ListeleCevapSatinAlma(null, kullanan, tk);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public Sonuc TalepKarsilamaCevapTamamla(Kullanici kullanan, TalepKarsilamaCevap tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilama.TamamlaCevap(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Taleps the karsilama listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tk">The tk.</param>
        /// <param name="detayVer">if set to <c>true</c> [detay ver].</param>
        /// <returns></returns>
        public ObjectArray TalepKarsilamaListele(Kullanici kullanan, TalepKarsilama tk, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMTalepKarsilama.Listele(null, kullanan, tk, detayVer);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public ObjectArray TalepKarsilamaListele(Kullanici kullanan, TalepKarsilama tk, bool detayVer, bool harcamaModulu)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMTalepKarsilama.Listele(null, kullanan, tk, detayVer, harcamaModulu);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public Sonuc TalepKarsilamaTuketimTIFOlustur(Kullanici kullanan, TalepKarsilama tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilama.TuketimTIFOlustur(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc TalepKarsilamaDurumDegistir(Kullanici kullanan, TalepKarsilama tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTalepKarsilama.DurumGuncelle(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Tuketims the uretim kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <returns></returns>
        public Sonuc TuketimUretimKaydet(Kullanici kullanan, TuketimUretim tu)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTuketimUretim.Kaydet(null, kullanan, tu);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Tuketims the uretim listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <param name="detayVer">if set to <c>true</c> [detay ver].</param>
        /// <returns></returns>
        public ObjectArray TuketimUretimListele(Kullanici kullanan, TuketimUretim tu, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMTuketimUretim.Listele(null, kullanan, tu, detayVer);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Giyeceks the techizat kaydet.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <returns></returns>
        public Sonuc GiyecekTechizatKaydet(Kullanici kullanan, GiyecekTechizat tu)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMGiyecekTechizat.Kaydet(null, kullanan, tu);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Giyeceks the techizat listele.
        /// </summary>
        /// <param name="kullanan">The kullanan.</param>
        /// <param name="tu">The tu.</param>
        /// <param name="detayVer">if set to <c>true</c> [detay ver].</param>
        /// <returns></returns>
        public ObjectArray GiyecekTechizatListele(Kullanici kullanan, GiyecekTechizat tu, bool detayVer)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMGiyecekTechizat.Listele(null, kullanan, tu, detayVer);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public Sonuc DevirGirisTIFKaydetOnayla(Kullanici kullanan, TasinirIslemForm giris, TNSCollection cikislar)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMTasinirIslemForm.DevirGirisTIFKaydetOnayla(null, kullanan, giris, cikislar);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }

        public ObjectArray TasinirResimGetir(int prSicilNo, string resimID)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = TMMResim.Getir(null, prSicilNo, resimID);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TasinirResimSil(TasResim bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMResim.Sil(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TasinirResimKaydet(TasResim bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMResim.Kaydet(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TopluIslemDetayGrupKodListele(Kullanici kullanan)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMTopluIslem().DetaydanGrupKodlariniListele(kullanan);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TopluIslemListele(Kullanici kullanan, TopluIslem t)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMTopluIslem().Listele(kullanan, t);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TopluIslemGuncelle(Kullanici kullanan, TopluIslem t)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMTopluIslem().Guncelle(kullanan, t);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TopluIslemFormKaydet(Kullanici kullanan, TopluIslemForm t)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMTopluIslem().KaydetForm(kullanan, t);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc TopluIslemFormSil(Kullanici kullanan, int grupKod)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMTopluIslem().SilForm(kullanan, grupKod);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray TopluIslemFormListele(Kullanici kullanan, TopluIslemForm t)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMTopluIslem().ListeleForm(kullanan, t);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray HesapPlaniBarkodGetir(TasHesapPlaniBarkod barkod)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = TMMHesapPlaniBarkod.Listele(null, barkod);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc HesapPlaniBarkodSil(Kullanici kullanan, string barkodListe)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMHesapPlaniBarkod.Sil(null, barkodListe);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc HesapPlaniBarkodKaydet(Kullanici kullanan, TasHesapPlaniBarkod barkod)
        {
            Sonuc sonuc = new Sonuc(false, TasinirMalResI.TMM069, "");

            try
            {
                sonuc = TMMHesapPlaniBarkod.Kaydet(null, kullanan, barkod);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan demirbaşların amortisman bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="ak">Amortisman listeleme kriterleri</param>
        /// <param name="bitenlerDahil">Kalan amortismanı sıfırlanmış demirbaşlar dahil edilsin mi bilgisi</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında demirbaşların amortisman bilgileri listesi döndürülür.</returns>
        public ObjectArray AmortismanRaporla(Kullanici kullanan, AmortismanKriter ak, bool bitenlerDahil)
        {
            try
            {
                return TMMAmortisman.AmortismanRaporla(null, kullanan, ak, bitenlerDahil);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray AmortismanRaporla2(Kullanici kullanan, AmortismanKriter ak)
        {
            try
            {
                return TMMAmortisman.AmortismanRaporla2(null, kullanan, ak);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray AmortismanRaporla3(Kullanici kullanan, AmortismanKriter ak)
        {
            try
            {
                return TMMAmortisman.AmortismanRaporla3(null, kullanan, ak);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public AmortismanRaporBA AmortismanRaporlaBA(Kullanici kullanan, AmortismanKriter ak)
        {
            return TMMAmortisman.AmortismanRaporlaBA(kullanan, ak);
        }

        public Sonuc AmortismanVerisiOlustur(Kullanici kullanan, AmortismanKriter ak, int islemSiraNo)
        {
            try
            {
                return TMMAmortisman.AmortismanVerisiOlustur(null, kullanan, ak, islemSiraNo);
            }
            catch (Exception e)
            {
                return new Sonuc(true, e.ToString(), "");
            }
        }

        public Sonuc AmortismanVerisiOlustur2(Kullanici kullanan, AmortismanKriter ak)
        {
            try
            {
                return TMMAmortisman.AmortismanVerisiOlustur2(null, kullanan, ak);
            }
            catch (Exception e)
            {
                return new Sonuc(true, e.ToString(), "");
            }
        }

        public Sonuc AmortismanVerisiOlustur3(Kullanici kullanan, AmortismanKriter ak)
        {
            try
            {
                return TMMAmortisman.AmortismanVerisiOlustur3(null, kullanan, ak);
            }
            catch (Exception e)
            {
                return new Sonuc(true, e.ToString(), "");
            }
        }

        public Sonuc AmortismanRakamKaydet(Kullanici kullanan, string kod, int yil, int donem, int prSicilNo, ENUMAmortismanTur amortismanTur, decimal amortismanMiktar, string saymanlikKod, string birimKod, string ambarKod)
        {
            try
            {
                return TMMAmortisman.RakamKaydet(null, kullanan, kod, yil, donem, prSicilNo, amortismanTur, amortismanMiktar, saymanlikKod, birimKod, ambarKod, null);
            }
            catch (Exception e)
            {
                return new Sonuc(true, e.ToString(), "");
            }
        }
        /// <summary>
        /// Verilen kriterlere uygun olan amortisman bilgileri dahil edilmiş dayanıklı taşınır defteri bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen DefterKriter nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">Amortismanlı dayanıklı taşınır defteri kriterlerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında amortismanlı dayanıklı taşınır defteri listesi döndürülür.</returns>
        //public ObjectArray AmortismanDTDOlustur(Kullanici kullanan, DefterKriter kriter)
        //{
        //    try
        //    {
        //        return TMMAmortisman.DayanikliTasinirlarDefteriOlustur(null, kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        ObjectArray bilgi = new ObjectArray();
        //        bilgi.sonuc.DegerAta(e.ToString());
        //        return bilgi;
        //    }
        //}

        /// <summary>
        /// Taşınır malzemelerinin amortisman sürelerini varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen AmortismanSure nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="sure">Amortisman süre bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc AmortismanSureKaydet(Kullanici kullanan, HesapPlaniSatir sure)
        {
            try
            {
                return TMMHesapPlani.AmortismanSureKaydet((VeriErisim)null, kullanan, sure);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc SayimKisiFormBaslat(Kullanici kullanan, SayimKisiForm form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimKisiForm().SayimBaslat(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray SayimKisiFormListele(Kullanici kullanan, SayimKisiForm form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMSayimKisiForm().SayimListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SayimKisiKisiListele(Kullanici kullanan, Sayimkisi form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMSayimKisiForm().SayimKisiListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SayimKisiKisiDetayListele(Kullanici kullanan, SayimKisiDetay form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMSayimKisiForm().SayimKisiDetayListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayimKisiFormSonlandir(Kullanici kullanan, SayimKisiForm form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimKisiForm().SayimSonlandir(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayimKisiBirimeYetkilimi(Kullanici kullanan, SayimKisiForm sayim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, sayim.muhasebe, sayim.harcamaBirimi);
                if (!donusDeger.islemSonuc)
                {
                    donusDeger.hataStr = TasinirMalResI.TMM024;
                    return donusDeger;
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayimKisiYeniKisiSay(Kullanici kullanan, Sayimkisi sk)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimKisiForm().YeniKisiSay(null, kullanan, sk);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SayimKisiDetayKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimKisiForm().KisiDetayKaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayimKisiDetayOkundu(Kullanici kullanan, SayimKisiDetay sayim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimKisiForm().KisiDetayOkundu(null, kullanan, sayim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayimKisiFazlaSil(Kullanici kullanan, SayimKisiDetay sayim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimKisiForm().FazlaSil(null, kullanan, sayim);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SayimKisiRapor(Kullanici kullanan, Sayimkisi form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMSayimKisiForm().Rapor(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SayimKisiRaporRapor(Kullanici kullanan, Sayimkisi form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMSayimKisiForm().KisiRapor(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayimAmbarFormBaslat(Kullanici kullanan, SayimAmbarForm form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimAmbarForm().SayimBaslat(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public ObjectArray SayimAmbarFormListele(Kullanici kullanan, SayimAmbarForm form)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMSayimAmbarForm().SayimListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SayimAmbarAmbarListele(Kullanici kullanan, Sayimambar form)
        {
            try
            {
                return new TMMSayimAmbarForm().SayimAmbarListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray SayimAmbarAmbarDetayListele(Kullanici kullanan, SayimAmbarDetay form)
        {
            try
            {
                return new TMMSayimAmbarForm().SayimAmbarDetayListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SayimAmbarFormSonlandir(Kullanici kullanan, SayimAmbarForm form)
        {
            try
            {
                return new TMMSayimAmbarForm().SayimSonlandir(null, kullanan, form);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SayimAmbarBirimeYetkilimi(Kullanici kullanan, SayimAmbarForm sayim)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger.islemSonuc = TMMYetkiKontrol.YazmaYetkisiVarMiHarcamaBirimi(kullanan, sayim.muhasebe, sayim.harcamaBirimi);
                if (!donusDeger.islemSonuc)
                {
                    donusDeger.hataStr = TasinirMalResI.TMM024;
                    return donusDeger;
                }
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SayimAmbarYeniAmbarSay(Kullanici kullanan, Sayimambar sk)
        {
            try
            {
                return new TMMSayimAmbarForm().YeniAmbarSay(null, kullanan, sk);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SayimAmbarDetayKaydet(Kullanici kullanan, ObjectArray liste)
        {
            try
            {
                return new TMMSayimAmbarForm().AmbarDetayKaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SayimAmbarDetayOkundu(Kullanici kullanan, SayimAmbarDetay sayim)
        {
            try
            {
                return new TMMSayimAmbarForm().AmbarDetayOkundu(null, kullanan, sayim);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc SayimAmbarFazlaSil(Kullanici kullanan, SayimAmbarDetay sayim)
        {
            try
            {
                return new TMMSayimAmbarForm().FazlaSil(null, kullanan, sayim);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray SayimAmbarRapor(Kullanici kullanan, Sayimambar form)
        {
            try
            {
                return new TMMSayimAmbarForm().Rapor(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc AmortismanSinirMIFKaydet(Kullanici kullanan, int yil, string mifNo)
        {
            try
            {
                return TMMAmortismanSinir.MIFKaydet((VeriErisim)null, kullanan, yil, mifNo);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc TesisBilesenleriKaydet(Kullanici kullanan, TesisBilesenleri tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTesisBilesenleriCetveli.Kaydet(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc TesisBilesenleriDurumGuncelle(Kullanici kullanan, string refId, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMTesisBilesenleriCetveli.DurumGuncelle(null, kullanan, refId, durumKod);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }
        public ObjectArray TesisBilesenleriListele(Kullanici kullanan, TesisBilesenleri tk)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMTesisBilesenleriCetveli.Listele(null, kullanan, tk);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public Sonuc UretimFormuKaydet(Kullanici kullanan, UretimFormu tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMUretimFormu.Kaydet(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc UretimFormuDurumGuncelle(Kullanici kullanan, string kod, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMUretimFormu.DurumGuncelle(null, kullanan, kod, durumKod);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }
        public ObjectArray UretimFormuListele(Kullanici kullanan, UretimFormu tk)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMUretimFormu.Listele(null, kullanan, tk);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public Sonuc UretimRecetesiKaydet(Kullanici kullanan, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMUretimRecetesi.Kaydet(null, kullanan, liste);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public ObjectArray UretimRecetesiListele(Kullanici kullanan, UretimRecetesi tk)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMUretimRecetesi.Listele(null, kullanan, tk);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public Sonuc AktarimKaydet(Kullanici kullanan, Aktarim tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMAktarim.Kaydet(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc AktarimDevamEt(Kullanici kullanan, Aktarim tk)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMAktarim.AktarimDevamEt(null, kullanan, tk);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }

        public Sonuc AktarimDurumGuncelle(Kullanici kullanan, string refId, int durumKod)
        {
            Sonuc sonuc = new Sonuc();
            try
            {
                sonuc = TMMAktarim.DurumGuncelle(null, kullanan, refId, durumKod);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }
            return sonuc;
        }
        public ObjectArray AktarimListele(Kullanici kullanan, Aktarim tk)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMAktarim.Listele(null, kullanan, tk);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public ObjectArray AktarimListeleTarihce(Kullanici kullanan, string ilgiKod)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMAktarim.ListeleTarihce(null, kullanan, ilgiKod);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        public Sonuc TasinirIslemFisiOnayDurumDegistir(Kullanici kullanan, TasinirIslemForm tf, ENUMTasinirIslemFormOnayDurumu yeniDurum)
        {
            try
            {
                return TMMTasinirIslemForm.OnayDurumDegistir(null, kullanan, tf, yeniDurum);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray TasinirIslemFisiOnayYetkiListele(Kullanici kullanan, TasinirIslemOnayYetki form)
        {
            try
            {
                return TMMTasinirIslemOnayYetki.Listele(kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }

        public Sonuc TasinirIslemFisiOnayYetkiKaydet(Kullanici kullanan, string muhasebeKod, string harcamaBirimKod, TasinirIslemOnayYetki[] liste)
        {
            try
            {
                return TMMTasinirIslemOnayYetki.Kaydet(kullanan, muhasebeKod, harcamaBirimKod, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc BisKaydet(Kullanici kullanan, int yil, string muhasebe, string harcamaBirimi, string ambar, string sicilNo, int prSicilNo, SicilNoOzellik[] formListe)
        {
            Sonuc sonuc = new Sonuc();

            try
            {
                sonuc = TMMSicilNoOzellik.BisKaydet(null, kullanan, yil, muhasebe, harcamaBirimi, ambar, sicilNo, prSicilNo, formListe);
            }
            catch (Exception e)
            {
                sonuc.islemSonuc = false;
                sonuc.hataStr = e.ToString();
            }

            return sonuc;
        }
        public ObjectArray BisSicilListele(Kullanici kullanan, SicilNoHareket sicilNo)
        {
            try
            {
                return TMMSicilNoHareket.BisSicilNolar(null, kullanan, sicilNo);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }
        public ObjectArray IliskiliMalzemeListele(Kullanici kullanan, int[] prSicilNoListe, bool sadeceSatirSayisi = false)
        {
            try
            {
                return TMMSicilNoHareket.IliskiliMalzemeListele(null, kullanan, prSicilNoListe, sadeceSatirSayisi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.hataStr = e.ToString();
                donusDeger.sonuc.islemSonuc = false;
                return donusDeger;
            }
        }
        public int IliskiliMalzemeSayisi(Kullanici kullanan, int[] prSicilNoListe)
        {
            try
            {
                return TMMSicilNoHareket.IliskiliMalzemeSayisi(null, kullanan, prSicilNoListe);
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        public ObjectArray DosyaSicilNoListele(Kullanici kullanan, SicilNoHareket sicilNo)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = TMMSicilNoHareket.DosyaSicilNoListele(null, kullanan, sicilNo);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc RFIDEtiketTuruKaydet(Kullanici kullanan, RFIDEtiketTuru form)
        {
            try
            {
                return TMMRFIDEtiketTuru.Kaydet(kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ObjectArray RFIDEtiketTuruListele(Kullanici kullanan, RFIDEtiketTuru form)
        {
            try
            {
                return TMMRFIDEtiketTuru.Listele(kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc RFIDEtiketTuruSil(Kullanici kullanan, RFIDEtiketTuru form)
        {
            try
            {
                return TMMRFIDEtiketTuru.Sil(kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray IslemDosyaGetir(IslemDosya bilgi)
        {
            ObjectArray donusDeger = new ObjectArray();

            try
            {
                donusDeger = TMMIslemDosya.Getir(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IslemDosyaSil(IslemDosya bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMIslemDosya.Sil(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc IslemDosyaKaydet(IslemDosya bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMIslemDosya.Kaydet(null, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public bool GormeYetkisiVarMi(Kullanici kullanan, string muhasebe, string harcama, string ambar)
        {
            return TMMYetkiKontrol.GormeYetkisiVarMi(kullanan, muhasebe, harcama, ambar);
        }

        public Sonuc GeciciAlindiFisiSicilOlustur(Kullanici kullanan, TasinirIslemForm tf, string islem)
        {
            try
            {
                return TMMGeciciAlindiForm.SicilOlustur(null, kullanan, tf, islem);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public Sonuc GeciciAlindiFisiTifFisNoKaydet(Kullanici kullanan, TasinirIslemForm tf)
        {
            try
            {
                return TMMGeciciAlindiForm.TifFisNoKaydet(null, kullanan, tf);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray DevirCikis(Kullanici kullanan, DevirCikis kriter)
        {
            try
            {
                return TMMRaporlar.DevirCikis(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray bilgi = new ObjectArray();
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
                return bilgi;
            }
        }

        public Sonuc SicilNoVer(Kullanici kullanan, ObjectArray sicilNolar)
        {
            try
            {
                if (kullanan.kullaniciKodu == "aktarim")
                    return TMMSicilNoHareket.SicilNoVer(null, kullanan, sicilNolar);
                return new Sonuc("Bu metodu aktarım kullanıcısı çağırabilir.");
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SicilNoDegerArtisKaydet(Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMSicilNoDegerArtis.Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc SicilNoDegerArtisKaydet(Kullanici kullanan, ObjectArray bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMSicilNoDegerArtis.Kaydet(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray SicilNoDegerArtisListele(Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            try
            {
                return TMMSicilNoDegerArtis.Listele(null, kullanan, bilgi);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc SicilNoDegerArtisSil(Kullanici kullanan, SicilNoDegerArtis bilgi)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMSicilNoDegerArtis.Sil(kullanan, bilgi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }
        public Sonuc SicilNoDegerArtisDurumGuncelle(Kullanici kullanan, SicilNoDegerArtis bilgi, string durumTur)
        {
            Sonuc donusDeger = new Sonuc();

            try
            {
                donusDeger = TMMSicilNoDegerArtis.GuncelleDurum(kullanan, bilgi, durumTur);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray BAOnayiYapilacaklarListele(Kullanici kullanan, string mernis, BAOnay kriter)
        {
            try
            {
                return TMMTasinirIslemForm.BAOnayiYapilacaklarListele(kullanan, mernis, kriter);
            }
            catch (Exception e)
            {
                ObjectArray bilgi = new ObjectArray();
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
                return bilgi;
            }
        }

        public ObjectArray BAOnayiSorgulama(Kullanici kullanan, BAOnay kriter)
        {
            try
            {
                return TMMTasinirIslemForm.BAOnayiSorgulama(kullanan, kriter);
            }
            catch (Exception e)
            {
                ObjectArray bilgi = new ObjectArray();
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
                return bilgi;
            }
        }

        public Sonuc AmortismanIslemFisiOnayDurumDegistir(Kullanici kullanan, AmortismanIslemForm form, ENUMTasinirIslemFormOnayDurumu yeniDurum)
        {
            try
            {
                return TMMAmortismanIslemForm.OnayDurumDegistir(null, kullanan, form, yeniDurum);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc AmortismanIslemFisiDurumDegistir(Kullanici kullanan, AmortismanIslemForm form, ENUMAmortismanFormDurumu yeniDurum)
        {
            try
            {
                return TMMAmortismanIslemForm.DurumDegistir(null, kullanan, form, yeniDurum);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray AmortismanIslemFisiListele(Kullanici kullanan, AmortismanIslemForm form)
        {
            try
            {
                return TMMAmortismanIslemForm.Oku(null, kullanan, form, false);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc AmortismanIslemFisiKaydet(Kullanici kullanan, AmortismanIslemForm form)
        {
            try
            {
                return TMMAmortismanIslemForm.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public ObjectArray MuhasebeBilgiListele(Kullanici kullanan, MuhasebeIslemi form)
        {
            try
            {
                return TMMMuhasebeBilgi.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray MuhasebeBilgiOnIzlemeOlustur(Kullanici kullanan, MuhasebeIslemiKriter form)
        {
            try
            {
                return TMMMuhasebeBilgi.OnIzlemeOlustur(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc MuhasebeServisineGonder(Kullanici kullanan, MuhasebeIslemi[] liste)
        {
            try
            {
                return TMMMuhasebeBilgi.MuhasebeServisineGonder(null, kullanan, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public Sonuc MuhasebeBilgiDurumSorgulaVeGuncelle(Kullanici kullanan, MuhasebeIslemi[] liste)
        {
            try
            {
                return TMMMuhasebeBilgi.DurumSorgulaveGuncelle(null, kullanan, liste);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ObjectArray AmortismanRaporuMB(Kullanici kullanan, AmortismanKriter ak)
        {
            try
            {
                return TMMAmortisman.AmortismanRaporuMB(null, kullanan, ak);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public ObjectArray BAOnayiTarihceListele(MuhasebeIslemiKriter form)
        {
            try
            {
                return TMMMuhasebeBilgi.ListeleTarihce(null, null, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public decimal SonBedelGetir(Kullanici kullanan, int prSicilNo)
        {
            try
            {
                return TMMSicilNoHareket.SonBedelGetir(null, kullanan, prSicilNo);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public ObjectArray KHKRaporu(Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.KHKRaporu(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public ObjectArray TasinirKodGrupluTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.TasinirKodGrupluTasinirHurdaRaporu(null, kullanan, tf, tfKriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public ObjectArray BakanlikDuzeyiTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.BakanlikDuzeyiTasinirHurdaRaporu(null, kullanan, tf, tfKriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public ObjectArray BirimItibariylaTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.BirimItibariylaTasinirHurdaRaporu(null, kullanan, tf, tfKriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan taşınır işlem bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="tf">Taşınır işlem fişi kriter bilgilerini tutan nesne</param>
        /// <param name="tfKriter">Ek kriter bilgilerini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır işlem bilgileri listesi döndürülür.</returns>
        public ObjectArray IlBazliTasinirHurdaRaporu(Kullanici kullanan, TasinirIslemForm tf, TasinirFormKriter tfKriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.IlBazliTasinirHurdaRaporu(null, kullanan, tf, tfKriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan harcama birimi bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="harcamaBirimi">Harcama birimi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında harcama birimi listesi döndürülür.</returns>
        public ObjectArray ABSKuraGoreHarcamaBirimleriListele(Kullanici kullanan, int yil)
        {
            ObjectArray bilgi = new ObjectArray();

            try
            {
                bilgi = TMMHarcamaBirimi.ABSKuraGoreHarcamaBirimleriListele(kullanan, yil);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }

            return bilgi;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public ObjectArray HesapPlaniBazindaYillikTuketimRaporu(Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.HesapPlaniBazindaYillikTuketimRaporu(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        public ObjectArray ToplamYillikTuketimRaporu(Kullanici kullanan, DepoDurum kriter)
        {
            ObjectArray bilgi = new ObjectArray();
            try
            {
                bilgi = TMMRaporlar.ToplamYillikTuketimRaporu(null, kullanan, kriter);
            }
            catch (Exception e)
            {
                bilgi.sonuc.islemSonuc = false;
                bilgi.sonuc.hataStr = e.ToString();
            }
            return bilgi;
        }

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        //public ObjectArray ZimmetKisiBirimAmbarBazli(Kullanici kullanan, ZimmetForm kriter)
        //{
        //    ObjectArray bilgi = new ObjectArray();
        //    try
        //    {
        //        bilgi = TMMRaporlar.ZimmetKisiBirimAmbarBazli(null, kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        bilgi.sonuc.islemSonuc = false;
        //        bilgi.sonuc.hataStr = e.ToString();
        //    }
        //    return bilgi;
        //}

        /// <summary>
        /// Verilen DepoDurum kriterlerine uygun olan taşınır malzemelerin giren,
        /// çıkan, zimmet miktarları/tutarları gibi bilgileri döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="kriter">DepoDurum listeleme kriterleri</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında taşınır malzemelerinin miktar, tutar bilgileri listesi döndürülür.</returns>
        //public ObjectArray ZimmetOrtakAlanBirimAmbarBazli(Kullanici kullanan, ZimmetForm kriter)
        //{
        //    ObjectArray bilgi = new ObjectArray();
        //    try
        //    {
        //        bilgi = TMMRaporlar.ZimmetOrtakAlanBirimAmbarBazli(null, kullanan, kriter);
        //    }
        //    catch (Exception e)
        //    {
        //        bilgi.sonuc.islemSonuc = false;
        //        bilgi.sonuc.hataStr = e.ToString();
        //    }
        //    return bilgi;
        //}

        public ObjectArray AyarlarMBListele(Kullanici kullanan, AyarlarMB form)
        {
            try
            {
                return TMMAyarlarMB.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc AyarlarMBKaydet(Kullanici kullanan, AyarlarMB form)
        {
            try
            {
                return TMMAyarlarMB.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc AyarlarMBSil(Kullanici kullanan, AyarlarMB form)
        {
            try
            {
                return TMMAyarlarMB.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public bool MutemmimAktifMi(Kullanici kullanan, AyarlarMB form)
        {
            try
            {
                if (DateTime.Now.Month == 1)
                {
                    ObjectArray liste = TMMAyarlarMB.Listele(null, kullanan, form);
                    if (liste.sonuc.islemSonuc && liste.objeler.Count == 1)
                    {
                        AyarlarMB a = (AyarlarMB)liste.objeler[0];
                        if (DateTime.Now.Date <= a.mutemmimBitisTarihi.Oku())
                            return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Sonuc TestYap(Kullanici kullanan, int islem, object form)
        {
            try
            {
                return TMMAyarlarMB.TestYap(null, kullanan, islem, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }


        public int AmortismanDonemiBul(string harcamaKod, TNSDateTime tarih)
        {
            try
            {
                if (!tarih.isNull)
                    return TMMAmortisman.AmortismanDonemiBul(harcamaKod, tarih);
            }
            catch (Exception e) { }

            return 0;
        }

        public AmortismanBilgi AmortismanBilgileri(VeriErisim veriErisim, Kullanici kullanan, int prSicilNo, string muhasebeKod, string harcamaBirimKod, int amortismanDonem)
        {
            return TMMAmortisman.AmortismanBilgileri(null, kullanan, prSicilNo, muhasebeKod, harcamaBirimKod, amortismanDonem);
        }

        public Sonuc HarmoniKaydet(Kullanici kullanan, Harmoni form)
        {
            try
            {
                return TMMHarmoni.Kaydet(kullanan, form);
            }
            catch (Exception e)
            {
                Sonuc donusDeger = new Sonuc();
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray HarmoniListele(Kullanici kullanan, Harmoni form)
        {
            try
            {
                return TMMHarmoni.Listele(kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public ObjectArray BirimlereGoreDemirbasRaporu(Kullanici kullanan, TasinirIslemDetay form)
        {
            try
            {
                return TMMRaporlar.BirimlereGoreDemirbasRaporu(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc HesapPlanKodDegistir(Kullanici kullanan, string muhasebe, string harcamaBirimi, string ambar, int prSicilNo, HesapPlaniSatir form)
        {
            try
            {
                return new TMMSicilNoOzellik().HesapPlanKodDegistir(kullanan, muhasebe, harcamaBirimi, ambar, prSicilNo, form);
            }
            catch (Exception e)
            {
                return new Sonuc()
                {
                    islemSonuc = false,
                    hataStr = e.ToString()
                };
            }
        }

        /// <summary>
        /// EnflasyonDuzeltmesi varsa güncelleyen, yoksa kaydeden yordam
        /// Kullanıcı işleme yetkili değilse veya gönderilen EnflasyonDuzeltmesi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="enflasyonDuzeltmesi">EnflasyonDuzeltmesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc EnflasyonDuzeltmesiKaydet(Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMEnflasyonDuzeltmesi().Kaydet(null, kullanan, enflasyonDuzeltmesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Kullanıcı işleme yetkili değilse veya gönderilen EnflasyonDuzeltmesi nesnesi eksikse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="enflasyonDuzeltmesi">EnflasyonDuzeltmesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc EnflasyonDuzeltmesiSil(Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMEnflasyonDuzeltmesi().Sil(null, kullanan, enflasyonDuzeltmesi);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        /// <summary>
        /// Verilen kriterlere uygun olan ölçü birim bilgilerini döndüren yordam
        /// Kullanıcı işleme yetkili değilse işlemi yapmaz ve hata döndürür.
        /// </summary>
        /// <param name="kullanan">İşlemi yapan kullanıcıya ait bilgileri tutan nesne</param>
        /// <param name="enflasyonDuzeltmesi">EnflasyonDuzeltmesi bilgisini tutan nesne</param>
        /// <returns>Hata varsa ObjectArray içindeki Sonuc yapısında hata döndürülür, yoksa TNSCollection yapısında enflasyon duzeltmesi listesi döndürülür.</returns>
        public ObjectArray EnflasyonDuzeltmesiListele(Kullanici kullanan, EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            ObjectArray donusDeger = new ObjectArray();
            try
            {
                donusDeger = new TMMEnflasyonDuzeltmesi().Listele(null, kullanan, enflasyonDuzeltmesi);
            }
            catch (Exception e)
            {
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public Sonuc EnflasyonDuzeltmesiAmortismanKaydet(Kullanici kullanan, AmortismanKriter ar)
        {
            try
            {
                return TMMAmortisman.EnflasyonDuzeltmesiKaydet(null, kullanan, ar);
            }
            catch (Exception e)
            {
                return new Sonuc(e.ToString());
            }
        }

        public Sonuc EnflasyonDuzeltmesiTifKaydet(Kullanici kullanan, AmortismanKriter ar)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMAmortisman.EnflasyonDuzeltmesiTifKaydet(null, kullanan, ar);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray KontrolRaporu(Kullanici kullanan, KontrolRaporu form)
        {
            try
            {
                return TMMRaporlar.KontrolRaporu(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }


        public Sonuc EBYSCumhurbaskanligiKaydet(Kullanici kullanan, EBYSCumhurbaskanligi form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMEBYSCumhurbaskanligi.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray EBYSCumhurbaskanligiListele(Kullanici kullanan, EBYSCumhurbaskanligi form)
        {
            try
            {
                return TMMEBYSCumhurbaskanligi.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc EBYSCumhurbaskanligiBirimKaydet(Kullanici kullanan, EBYSCumhurbaskanligiBirim form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMEBYSCumhurbaskanligiBirim.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray EBYSCumhurbaskanligiBirimListele(Kullanici kullanan, EBYSCumhurbaskanligiBirim form)
        {
            try
            {
                return TMMEBYSCumhurbaskanligiBirim.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }


        public Sonuc RFIDHareketKaydet(Kullanici kullanan, RFIDHareket form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = TMMRFIDHareket.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }
            return donusDeger;
        }

        public ObjectArray RFIDHareketListele(Kullanici kullanan, RFIDHareket form)
        {
            try
            {
                return TMMRFIDHareket.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc RFIDEtiketTarihiGuncelle(Kullanici kullanan, int prSicilNo, TNSDateTime tarih)
        {
            try
            {
                return new TMMSicilNoOzellik().RFIDEtiketTarihiGuncelle(kullanan, prSicilNo, tarih);
            }
            catch (Exception e)
            {
                return new Sonuc() { islemSonuc = false, hataStr = e.ToString() };
            }
        }

        public int PrSicilNoGetir(Kullanici kullanan, string sicilNo)
        {
            try
            {
                return TMMSicilNoOzellik.PrSicilNoGetir(null, kullanan, sicilNo);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public ObjectArray PrSicilNoListele(Kullanici kullanan, SicilNoHareket form)
        {
            try
            {
                return TMMSicilNoOzellik.PrSicilNoListele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }

        public Sonuc RFIDYaziciKaydet(Kullanici kullanan, RFIDYazici form)
        {
            try
            {
                return TMMRFIDYazici.Kaydet(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }
        public ObjectArray RFIDYaziciListele(Kullanici kullanan, RFIDYazici form)
        {
            try
            {
                return TMMRFIDYazici.Listele(null, kullanan, form);
            }
            catch (Exception e)
            {
                ObjectArray donusDeger = new ObjectArray();
                donusDeger.sonuc.islemSonuc = false;
                donusDeger.sonuc.hataStr = e.ToString();
                return donusDeger;
            }
        }
        public Sonuc RFIDYaziciSil(Kullanici kullanan, RFIDYazici form)
        {
            try
            {
                return TMMRFIDYazici.Sil(null, kullanan, form);
            }
            catch (Exception e)
            {
                return new Sonuc(false, e.ToString(), "");
            }
        }

        public Sonuc SayimAmbarFormSil(Kullanici kullanan, SayimAmbarForm form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimAmbarForm().SayimSil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

        public Sonuc SayimKisiFormSil(Kullanici kullanan, SayimKisiForm form)
        {
            Sonuc donusDeger = new Sonuc();
            try
            {
                donusDeger = new TMMSayimKisiForm().SayimSil(null, kullanan, form);
            }
            catch (Exception e)
            {
                donusDeger.islemSonuc = false;
                donusDeger.hataStr = e.ToString();
            }

            return donusDeger;
        }

    }
}