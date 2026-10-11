using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHBANKAODEMEDETAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMUHBANKAODEMEDETAY : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMUHBANKAODEMEDETAY(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }
        /*
         * 
         * 
CREATE TABLE [dbo].[MUHBANKAODEMEDETAY](
	[SAYMANLIK] [int] NOT NULL,
	[BIRIM] [varchar](20) NOT NULL,
	[GUN] [datetime] NOT NULL,
	[SIRANO] [int] NOT NULL,
	[SIRA] [int] NOT NULL,
	[MIFYIL] [int] NULL,
	[MIFBELGENO] [varchar](15) NULL,
	[MIFSIRA] [int] NULL,
	[MUHHESAPKOD] [varchar](30) NULL,
	[SOZLESMENO] [varchar](100) NULL,
	[ILGILI] [varchar](200) NULL,
	[TC_VNO] [varchar](50) NULL,
	[ILGILIBANKA] [varchar](100) NULL,
	[ILGILIIBAN] [varchar](100) NULL,
	[TUTAR] [money] NULL,
	[DOVIZ] [varchar](20) NULL,
	[ACIKLAMA] [varchar](500) NULL,
	[DURUM] [int] NULL,
	[DEKONTNO] [varchar](100) NULL,
	[ODEMEEMRINO] [varchar](15) NULL,
 PRIMARY KEY  
(
	[SAYMANLIK] ASC ,
	[BIRIM] ASC ,
	[GUN] ASC,
	[SIRANO] ASC,
	[SIRA] ASC
) 
)  
         ALTER TABLE MUHBANKAODEMEDETAY
              ADD CONSTRAINT MUHBANKAODEMEDETAY_FK1
              FOREIGN KEY (SAYMANLIK, BIRIM, GUN, SIRANO)
              REFERENCES MUHBANKAODEMEANA(SAYMANLIK, BIRIM, GUN, SIRANO)

         ALTER TABLE MUHBANKAODEMEDETAY
              ADD CONSTRAINT MUHBANKAODEMEDETAY_FK2
              FOREIGN KEY (MIFYIL, MIFBELGENO, MIFSIRA)
              REFERENCES MUHMIF(YIL, BELGENO, SIRA)
         
         ALTER TABLE MUHBANKAODEMEDETAY
              ADD CONSTRAINT MUHBANKAODEMEDETAY_FK3
              FOREIGN KEY (MIFYIL, MUHHESAPKOD)
              REFERENCES MUHHESAPPLANI(YIL, HESAPKOD)

        CEKNO
         * 
         * 
         * */


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MUHBANKAODEMEDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// SAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIK = 0,

            /// <summary>
            /// SAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIM = 1,

            /// <summary>
            /// GUN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GUN = 2,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// SIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRA = 4,

            /// <summary>
            /// MIFYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIFYIL = 5,

            /// <summary>
            /// MIFBELGENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIFBELGENO = 6,

            /// <summary>
            /// MIFSIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIFSIRA = 7,

            /// <summary>
            /// MUHHESAPKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHHESAPKOD = 8,

            /// <summary>
            /// SOZLESMENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMENO = 9,

            /// <summary>
            /// ILGILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILI = 10,

            /// <summary>
            /// TC_VNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TC_VNO = 11,

            /// <summary>
            /// ILGILIBANKA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIBANKA = 12,

            /// <summary>
            /// ILGILIIBAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILGILIIBAN = 13,

            /// <summary>
            /// TUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTAR = 14,

            /// <summary>
            /// DOVIZ alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DOVIZ = 15,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 16,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 17,

            /// <summary>
            /// DEKONTNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEKONTNO = 18,

            /// <summary>
            /// ODEMEEMRINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 19,

            /// <summary>
            /// ODEMEEMRINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEKNO = 20

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "SAYMANLIK"
            , "BIRIM"
            , "GUN"
            , "SIRANO"
            , "SIRA"
            , "MIFYIL"
            , "MIFBELGENO"
            , "MIFSIRA"
            , "MUHHESAPKOD"
            , "SOZLESMENO"
            , "ILGILI"
            , "TC_VNO"
            , "ILGILIBANKA"
            , "ILGILIIBAN"
            , "TUTAR"
            , "DOVIZ"
            , "ACIKLAMA"
            , "DURUM"
            , "DEKONTNO"
            , "ODEMEEMRINO"
            , "CEKNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, MUHBankaOdemeyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MUHBankaOdeme form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, muhBankaOdemeDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, MUHBankaOdemeyapısında nesne</param>
        /// <param name="satirNo">muhBankaOdemeDetay dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(MUHBankaOdeme form, int satirNo, MUHBankaOdemeDetay detay)
        {
            if (detay.aciklama.Length > 1000) detay.aciklama = detay.aciklama.Substring(0, 1000);

            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.saymanlik);
            sql += "," + VeriErisim.DegerYap(form.birim);
            sql += "," + VeriErisim.DegerYap(form.gun);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(detay.mifYil);
            sql += "," + VeriErisim.DegerYap(detay.mifBelgeNo);
            sql += "," + VeriErisim.DegerYap(detay.mifSira);
            sql += "," + VeriErisim.DegerYap(detay.muhHesapKodu.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(detay.sozlesmeNo);
            sql += "," + VeriErisim.DegerYap(detay.ilgili);
            sql += "," + VeriErisim.DegerYap(detay.tc_Vno);
            sql += "," + VeriErisim.DegerYap(detay.ilgBanka);
            sql += "," + VeriErisim.DegerYap(detay.ilgIBAN);
            sql += "," + VeriErisim.DegerYap(detay.tutar);
            sql += "," + VeriErisim.DegerYap(detay.doviz);
            sql += "," + VeriErisim.DegerYap(detay.aciklama);
            sql += "," + VeriErisim.DegerYap((int)ENUMBankaListesiDetayDurum.Yeni);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOE(MUHBankaOdeme form, MUHBankaOdemeDetay detay)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MIFYIL], "=", detay.mifYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MIFBELGENO], "=", detay.mifBelgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MIFSIRA], "=", detay.mifSira));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, MUHBankaOdemeyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(int saymanlik, string birim, TNSDateTime gun, int siraNo, int sira)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", F.YEVMIYENO, F2.ISLEMTARIH FROM " + TABLOAD + " LEFT OUTER JOIN MUHMIFFORM AS F ";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOAD + ".MIFYIL=F.YIL");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + ".MIFBELGENO=F.BELGENO");

            sql = sql + " LEFT OUTER JOIN MUHMIFFORM AS F2 ";
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOAD + ".MIFYIL = F2.YIL");
            sql = VeriErisim.KosulEkle(sql, "AND", TABLOAD + ".ODEMEEMRINO = F2.BELGENO");

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GUN], "=", gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            if (sira > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", sira));
            sql += " ORDER BY F.YEVMIYENO, MUHBANKAODEMEDETAY.MIFSIRA";
            return veriErisim.DataReaderDondur(sql);
        }

        public int SiraNoBul(MUHBankaOdeme form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRA] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", form.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", form.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader OdenecekGondermeListele(int saymanlikKod, string birimKod, int yil, bool alacakliHesaplar, TNSDateTime yevBasTar, TNSDateTime yevBitTar, string cekNo)
        {
            string ba;
            if (alacakliHesaplar)
                ba = "ALACAK";
            else
                ba = "BORC";
            string sql = "SELECT D.YIL, D.BELGENO, D.SIRA, D.HESAPKOD, D.DOVIZ, D." + ba + ", D.DOVIZ" + ba + ", F.IADSOYAD, F.IKIMLIKNO, F.ISUBEAD, F.IHESAPNO, F.ACIKLAMA, F.IVERGIDAIRE, D.ALTHESAPKOD, F.YEVMIYEACIKLAMA,F.BIRIMKOD,F.OTARIH,F.ONO";
            sql += " FROM MUHMIFFORM AS F INNER JOIN MUHMIF AS D ON F.YIL = D.YIL AND F.BELGENO = D.BELGENO";
            sql += " WHERE (F.YIL = " + VeriErisim.DegerYap(yil) + ") AND (F.FORMDURUM = 6) AND (D." + ba + " > 0)";
            sql += " AND (F.MUHASEBEKOD = " + VeriErisim.DegerYap(saymanlikKod) + ")";
            if (birimKod != "000")
                sql += " AND (F.BIRIMKOD = " + VeriErisim.DegerYap(birimKod) + ")";
            sql += " AND (D.HESAPKOD LIKE '103%' OR D.HESAPKOD LIKE '106%')";

            if (yevBasTar != null && !yevBasTar.isNull)
                sql += " AND (F.YEVMIYETARIH >= " + VeriErisim.DegerYap(yevBasTar) + ")";
            if (yevBitTar != null && !yevBitTar.isNull)
                sql += " AND (F.YEVMIYETARIH <= " + VeriErisim.DegerYap(yevBitTar) + ")";
            if (!string.IsNullOrWhiteSpace(cekNo))
                sql += " AND (F.CEKEMRINO = " + VeriErisim.DegerYap(cekNo) + ")";

            sql += " AND 1 NOT IN (SELECT 1 FROM MUHBANKAODEMEDETAY O";
            sql += " WHERE O.MIFYIL = D.YIL";
            sql += " AND O.MIFBELGENO = D.BELGENO";
            sql += " AND O.MIFSIRA = D.SIRA";
            sql += " AND O.DURUM < 9) ORDER BY F.YEVMIYENO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DetayOdendi(KYM.Kullanici kullanan, int saymanlik, string birim, TNSDateTime gun, int siraNo, int sira, string dekontNo, string odemeEmriNo)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEKONTNO], dekontNo);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMBankaListesiDetayDurum.Odendi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], odemeEmriNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", sira));
            return KomutCalistir(sql, 1);
        }


        public Sonuc CekNoGuncelle(MUHBankaOdeme ana, MUHBankaOdemeDetay detay)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.CEKNO], detay.cekNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", ana.saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", ana.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GUN], "=", ana.gun));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", ana.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", detay.sira));
            return KomutCalistir(sql, 1);
        }

        ///// <summary>
        ///// SELECT     MUHASEBEKODU AS SAYMANLIKKOD, TCKIMLIKNO AS TC_VNO, 1 as SIRA, AD + ' ' + SOYAD AS ILGILI, BANKAKODU AS BANKAAD, '' AS BICSWIFT, 
        /////                       BANKAHESAPNO AS IBAN
        ///// FROM         Personel.dbo.PERPERSONEL
        ///// 
        ///// UNION 
        ///// 
        ///// SELECT     MUHASEBEKOD AS SAYMANLIKKOD, VERGINO AS TC_VNO, 2 as SIRA, AD AS ILGILI, BANKA AS BANKAAD, BICSWIFT AS BICSWIFT, 
        /////                       HESAPNO AS IBAN
        ///// FROM         HarcamaYonetimiUA.dbo.HRCFIRMABILGISI
        ///// 
        ///// UNION 
        ///// 
        ///// SELECT     MUHASEBEKOD AS SAYMANLIKKOD, TCKIMLIKNO AS TC_VNO, 3 as SIRA, ADISOYADI AS ILGILI, BANKAKODU AS BANKAAD, '' AS BICSWIFT, 
        /////                       BANKAHESAPNO AS IBAN
        ///// FROM         HarcamaYonetimiUA.dbo.HRCOGRENCIEGITMEN
        ///// </summary>
        ///// <param name="tc_Vno"></param>
        ///// <param name="saymanlikKod"></param>
        ///// <returns></returns>
        //public IDataReader IlgiliBul(string tc_Vno, int saymanlikKod)
        //{
        //    string sql = "SELECT ILGILI, BANKAAD, BICSWIFT, IBAN FROM ILGILI WHERE TC_VNO = " + VeriErisim.DegerYap(tc_Vno) + " AND SAYMANLIKKOD = " + VeriErisim.DegerYap(saymanlikKod) + " ORDER BY SIRA";
        //    return this.veriErisim.DataReaderDondur(sql);
        //}
    }
}
