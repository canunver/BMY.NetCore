using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHF;
namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// BHFKIRALIKARACSOZLESMESI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHFKIRALIKARACSOZLESMESI : TABOrtak
    {
        /// <summary>
        /// TABBHFKIRALIKARACSOZLESMESI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// CREATE TABLE BHFKIRALIKARACSOZLESMESI
        ///    (YIL VARCHAR(50)
        ///    , SOZLESMEKOD INT
        ///    , SAYMANLIKNO VARCHAR(50)
        ///    , BIRIMKOD INT
        ///    , IMZATARIHI DATETIME
        ///    , SOZLESMENO VARCHAR(20)
        ///    , ISBASLANGICTARIHI DATETIME
        ///    , ISBITISTARIHI DATETIME
        ///    , FIRMAKOD INT
        ///    , SOZLESMEBEDELI MONEY
        ///    , BUTCETURU INT
        ///    , ILCELERDAHIL INT
        ///    , SURECYIL INT
        ///    , SURECNO VARCHAR(20)
        ///    , PRIMARY KEY(YIL, SOZLESMEKOD)
        ///    )
        ///
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHFKIRALIKARACSOZLESMESI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHFKIRALIKARACSOZLESMESI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// SOZLESMEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMEKOD = 1,

            /// <summary>
            /// SAYMANLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIKNO = 2,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 3,

            /// <summary>
            /// IMZATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IMZATARIHI = 4,

            /// <summary>
            /// SOZLESMENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMENO = 5,

            /// <summary>
            /// ISBASLANGICTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISBASLANGICTARIHI = 6,

            /// <summary>
            /// ISBITISTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISBITISTARIHI = 7,

            /// <summary>
            /// FIRMAKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FIRMAKOD = 8,

            /// <summary>
            /// SOZLESMEBEDELI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMEBEDELIBUTCE = 9,

            /// <summary>
            /// BUTCETURU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOZLESMEBEDELIDONER = 10,

            /// <summary>
            /// ILCELERDAHIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ILCELERDAHIL = 11,

            /// <summary>
            /// SURECYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 12,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 13,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SOZLESMEKOD"
            , "SAYMANLIKNO"
            , "BIRIMKOD"
            , "IMZATARIHI"
            , "SOZLESMENO"
            , "ISBASLANGICTARIHI"
            , "ISBITISTARIHI"
            , "FIRMAKOD"
            , "SOZLESMEBEDELIBUTCE"
            , "SOZLESMEBEDELIDONER"
            , "ILCELERDAHIL"
            , "SURECYIL"
            , "SURECNO"
        };

        public int NoBul(KiralikAracSozlesmesi form)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            double dbl = this.veriErisim.TekKolonSQLDbl(sql);
            return Convert.ToInt32(dbl + 1);
        }

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, KiralikAracSozlesmesi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(KiralikAracSozlesmesi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", form.sozlesmeKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, KiralikAracSozlesmesi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(KiralikAracSozlesmesi form)
        {
            string sql = "UPDATE " + TABLOAD;

            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKNO], form.saymanlikNo);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], form.birimKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMENO], form.sozlesmeNo);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZATARIHI], form.imzaTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISBASLANGICTARIHI], form.isBaslangicTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISBITISTARIHI], form.isBitisTarihi);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], form.firmaKod);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILCELERDAHIL], form.ilcelerDahil);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], form.surecYil);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], form.surecNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", form.sozlesmeKod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, KiralikAracSozlesmesi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(KiralikAracSozlesmesi form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.sozlesmeKod);
            sql += "," + VeriErisim.DegerYap(form.saymanlikNo);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.imzaTarihi);
            sql += "," + VeriErisim.DegerYap(form.sozlesmeNo);
            sql += "," + VeriErisim.DegerYap(form.isBaslangicTarihi);
            sql += "," + VeriErisim.DegerYap(form.isBitisTarihi);
            sql += "," + VeriErisim.DegerYap(form.firmaKod);
            sql += "," + VeriErisim.DegerYap(0);
            sql += "," + VeriErisim.DegerYap(0);
            sql += "," + VeriErisim.DegerYap(form.ilcelerDahil);
            sql += "," + VeriErisim.DegerYap(form.surecYil);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, KiralikAracSozlesmesi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(KiralikAracSozlesmesi form)
        {
            string aracSorgu = "\r\n, (" + TABBHFKIRALIKARAC.ListeleUstTablo("SOZ." + ALANLAR[(int)ENUMAlanlar.YIL], "SOZ." + ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD]) + " FOR XML RAW, ELEMENTS xsinil, TYPE)";

            string sql = "SELECT " + VeriErisim.TumAlanlar("SOZ", ALANLAR) + ", FRM.AD, SAY.MUHASEBEAD, BRM.BIRIMAD" + aracSorgu + " FROM " + TABLOAD + " SOZ ";
            sql += " LEFT OUTER JOIN HarcamaYonetimi.dbo.HRCFIRMABILGISI FRM ON FRM.KOD = SOZ.FIRMAKOD ";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE SAY ON SAY.MUHASEBEKOD = SOZ.SAYMANLIKNO";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM BRM ON BRM.MUHASEBEKOD = SOZ.SAYMANLIKNO AND BRM.BIRIMKOD=SOZ.BIRIMKOD";
            string kosul = "";
            int anahSay = 0;
            if (form.yil > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("SOZ", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
                anahSay++;
            }
            if (form.sozlesmeKod > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("SOZ", ALANLAR[(int)ENUMAlanlar.SOZLESMEKOD], "=", form.sozlesmeKod));
                anahSay++;
            }
            if (anahSay != 2)
            {
                if (form.saymanlikNo > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("SOZ", ALANLAR[(int)ENUMAlanlar.SAYMANLIKNO], "=", form.saymanlikNo));
                }
                if (!string.IsNullOrWhiteSpace(form.birimKod))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("SOZ", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
                }
            }
            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            sql += " order by 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Raporla(int yil, int saymanlikNo, string birimKod)
        {
            string sql = "SELECT BHFTASITCINS.TKODU, SOFORDAHIL, YAKITDAHIL, ADET, SURE, SUREBIRIMI, BIRIMFIYAT, PEBKOD";
            sql += " FROM BHFKIRALIKARAC INNER JOIN BHFKIRALIKARACSOZLESMESI";
            sql += " ON BHFKIRALIKARAC.YIL = BHFKIRALIKARACSOZLESMESI.YIL";
            sql += " AND BHFKIRALIKARAC.SOZLESMEKOD = BHFKIRALIKARACSOZLESMESI.SOZLESMEKOD INNER JOIN BHFTASITCINS ON BHFTASITCINS.KOD=TASITCINSI";
            sql += " where DATEPART(yy, ISBASLANGICTARIHI) >= " + VeriErisim.DegerYap(yil) + " AND DATEPART(yy, ISBITISTARIHI) <= " + VeriErisim.DegerYap(yil);
            if (saymanlikNo > 0)
                sql += " and BHFKIRALIKARACSOZLESMESI.SAYMANLIKNO = " + VeriErisim.DegerYap(saymanlikNo);
            if (!string.IsNullOrWhiteSpace(birimKod))
                sql += " and BHFKIRALIKARACSOZLESMESI.BIRIMKOD = " + VeriErisim.DegerYap(birimKod);
            sql += " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
