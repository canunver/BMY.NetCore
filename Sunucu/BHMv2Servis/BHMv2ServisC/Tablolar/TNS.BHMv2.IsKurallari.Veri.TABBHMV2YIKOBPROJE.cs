using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2YIKOBPROJE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMV2YIKOBPROJE : TABOrtak
    {
        /// <summary>
        /// TABBHMV2YIKOBPROJE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMV2YIKOBPROJE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }
        /*
          
        CREATE TABLE BHMV2YIKOBPROJE
    (YIL INT
    , MUHASEBEKOD INT
    , BIRIMKOD VARCHAR(3)
    , SIRANO INT
    , PROJEADI VARCHAR(250)
    , BASLAYIL INT
    , BITISYIL INT
    , TOPLAMMALIYET MONEY
    , UYGULAMAYERI VARCHAR(250)
    , PROJENINAMACI VARCHAR(2500)
    , ACIKLAMA VARCHAR(2500)
    , YATIRIMYIL INT
    , YATIRIMPROJEAD VARCHAR(1000)
    , YATIRIMPROJENO VARCHAR(20)
    , PRIMARY KEY (YIL, MUHASEBEKOD, BIRIMKOD, SIRANO)
    )
         * */


        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMV2YIKOBPROJE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 2,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// PROJEADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJEADI = 4,

            /// <summary>
            /// BASLAYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLAYIL = 5,

            /// <summary>
            /// BITISYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISYIL = 6,

            /// <summary>
            /// TOPLAMMALIYET alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TOPLAMMALIYET = 7,
             
            /// <summary>
            /// UYGULAMAYERI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UYGULAMAYERI = 8,

            /// <summary>
            /// PROJENINAMACI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJENINAMACI = 9,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 10,

            /// <summary>
            /// YATIRIMYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YATIRIMYIL = 11,

            /// <summary>
            /// YATIRIMPROJEAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YATIRIMPROJEAD = 12,

            /// <summary>
            /// YATIRIMPROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YATIRIMPROJENO = 13
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "MUHASEBEKOD"
            , "BIRIMKOD"
            , "SIRANO"
            , "PROJEADI"
            , "BASLAYIL"
            , "BITISYIL"
            , "TOPLAMMALIYET"
            , "UYGULAMAYERI"
            , "PROJENINAMACI"
            , "ACIKLAMA"
            , "YATIRIMYIL"
            , "YATIRIMPROJEAD"
            , "YATIRIMPROJENO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, YIKOBProje yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(YIKOBProje form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, YIKOBProje yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(YIKOBProje form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.muhasebeKod);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.projeAdi);
            sql += "," + VeriErisim.DegerYap(form.baslaYil);
            sql += "," + VeriErisim.DegerYap(form.bitisYil);
            sql += "," + VeriErisim.DegerYap(form.toplamMaliyet);
            sql += "," + VeriErisim.DegerYap(form.uygulamaYeri);
            sql += "," + VeriErisim.DegerYap(form.projeninAmaci);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.yatirimYil); 
            sql += "," + VeriErisim.DegerYap(form.yatirimProjeAd); 
            sql += "," + VeriErisim.DegerYap(form.yatirimProjeNo); 
            sql += ")";
            return KomutCalistir(sql, 1);
        }
        
        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, YIKOBProje yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(YIKOBProje form)
        {
            string alanlar = VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", M.MUHASEBEAD, B.BIRIMAD";
            string sql = "SELECT " + alanlar + ", SUM(PB.YIKOBBUTCE), SUM(PB.BAKBUTCE), SUM(PB.YIKOBHARC), SUM(PB.BAKHARC), SUM(PB.YIKOBDEVIR), SUM(PB.BAKDEVIR) FROM BHMV2YIKOBPROJE LEFT OUTER JOIN ";
            sql += "ButceIslemleri.dbo.MUHMUHASEBE AS M ON BHMV2YIKOBPROJE.MUHASEBEKOD = M.MUHASEBEKOD LEFT OUTER JOIN ";
            sql += "ButceIslemleri.dbo.MUHMUHASEBEBIRIM AS B ON BHMV2YIKOBPROJE.MUHASEBEKOD = B.MUHASEBEKOD AND BHMV2YIKOBPROJE.BIRIMKOD = B.BIRIMKOD LEFT OUTER JOIN ";
            sql += "BHMV2YIKOBPROJEBUTCE AS PB ON BHMV2YIKOBPROJE.YIL = PB.YIL AND BHMV2YIKOBPROJE.MUHASEBEKOD = PB.MUHASEBEKOD AND BHMV2YIKOBPROJE.BIRIMKOD = PB.BIRIMKOD AND BHMV2YIKOBPROJE.SIRANO = PB.SIRANO ";
            
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            
            if (form.muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            if (!string.IsNullOrWhiteSpace(form.birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public int SiraNoBul(YIKOBProje form)
        {
            string sql = "SELECT MAX(SIRANO) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            int siraNo = (int)veriErisim.TekKolonSQLDbl(sql);
            return siraNo + 1;
        }
    }
}
