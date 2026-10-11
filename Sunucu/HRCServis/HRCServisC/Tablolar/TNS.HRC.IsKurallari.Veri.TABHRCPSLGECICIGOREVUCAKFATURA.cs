using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCPSLGECICIGOREVUCAKFATURA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCPSLGECICIGOREVUCAKFATURA : TABOrtak
    {
        /*
         * 
         
CREATE TABLE HRCPSLGECICIGOREVUCAKFATURA
    (SURECYIL INT
    , SURECNO VARCHAR(20)
    , SIRANO INT
    , TARIH DATETIME
    , FATURANO VARCHAR(20)
    , TUTAR MONEY
    , KOMISYONMU INT
    PRIMARY KEY (SURECYIL, SURECNO, SIRANO)
    )


         * 
         * */


        /// <summary>
        /// TABHRCPSLGECICIGOREVUCAKFATURA sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCPSLGECICIGOREVUCAKFATURA(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCPSLGECICIGOREVUCAKFATURA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SURECYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 3,

            /// <summary>
            /// FATURANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURANO = 4,

            /// <summary>
            /// TUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTAR = 5,

            /// <summary>
            /// KOMISYONMU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOMISYONMU = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SURECYIL"
            , "SURECNO"
            , "SIRANO"
            , "TARIH"
            , "FATURANO"
            , "TUTAR"
            , "KOMISYONMU"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name=\"form\">Anahtar değişkenleri tutan, " + icAd + "yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(PSLGeciciGorevUcak form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, pSLGeciciGorevUcakFatura dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, PSLGeciciGorevUcak yapısında nesne</param>
        /// <param name="satirNo">pSLGeciciGorevUcakFatura dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(PSLGeciciGorevUcak form, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.surecYil);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(form.faturalar[satirNo].tarih);
            sql += "," + VeriErisim.DegerYap(form.faturalar[satirNo].faturaNo);
            sql += "," + VeriErisim.DegerYap(form.faturalar[satirNo].tutar);
            sql += "," + VeriErisim.DegerYap(form.faturalar[satirNo].komisyonMu);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public bool FaturaVarMi(int haricSurecYil, string haricSurecNo, string vergiNo, string faturaNo, out int eskiSurecYil, out string eskiSurecNo)
        {
            eskiSurecYil = 0;
            eskiSurecNo = "";
            string sql = "SELECT HRCPSLGECICIGOREVUCAK.SURECYIL, HRCPSLGECICIGOREVUCAK.SURECNO";
            sql += " FROM HRCPSLGECICIGOREVUCAK";
            sql += " INNER JOIN HRCPSLGECICIGOREVUCAKFATURA ON HRCPSLGECICIGOREVUCAK.SURECYIL = HRCPSLGECICIGOREVUCAKFATURA.SURECYIL AND HRCPSLGECICIGOREVUCAK.SURECNO = HRCPSLGECICIGOREVUCAKFATURA.SURECNO";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAKFATURA", "FATURANO", "=", faturaNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAK", "TCKIMLIKVEYAVERGINO", "=", vergiNo));

            sql += " AND (";
            sql += VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAKFATURA", "SURECYIL", "<>", haricSurecYil);
            sql += " OR ";
            sql += VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAKFATURA", "SURECNO", "<>", haricSurecNo);
            sql += ")";

            IDataReader dr = this.veriErisim.DataReaderDondur(sql);
            if (dr != null)
            {
                if (dr.Read())
                {
                    eskiSurecYil = VeriErisim.DegerAl(dr, 0, 0);
                    eskiSurecNo = VeriErisim.DegerAl(dr, 1, "");
                }
                dr.Close();
                return eskiSurecYil != 0;
            }
            else
                return true;
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name=\"form\">Select edilecek kayıtlara ait kıstas bilgisini tutan, PSLGeciciGorevUcak yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(PSLGeciciGorevUcak form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            sql += " ORDER BY 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(FaturaTum kriter)
        {
            string sql = "SELECT HRCPSLGECICIGOREVUCAK.SURECYIL, HRCSUREC.MUHASEBEKOD, HRCSUREC.BIRIMKOD, HRCPSLGECICIGOREVUCAK.SURECNO, HRCPSLGECICIGOREVUCAKFATURA.FATURANO, ";
            sql += " HRCPSLGECICIGOREVUCAKFATURA.TARIH, HRCSUREC.ISINADI, HRCFIRMABILGISI.AD, HRCPSLGECICIGOREVUCAK.TCKIMLIKVEYAVERGINO, HRCPSLGECICIGOREVUCAK.ODEMEEMRIYIL, ";
            sql += " HRCPSLGECICIGOREVUCAK.ODEMEEMRINO, MIF.YEVMIYETARIH, MIF.YEVMIYENO, MIF.FORMDURUM, HRCPSLGECICIGOREVUCAKFATURA.TUTAR";
            sql += " FROM HRCPSLGECICIGOREVUCAK";
            sql += " INNER JOIN HRCPSLGECICIGOREVUCAKFATURA ON HRCPSLGECICIGOREVUCAK.SURECYIL = HRCPSLGECICIGOREVUCAKFATURA.SURECYIL AND HRCPSLGECICIGOREVUCAK.SURECNO = HRCPSLGECICIGOREVUCAKFATURA.SURECNO";
            sql += " INNER JOIN HRCSUREC ON HRCPSLGECICIGOREVUCAK.SURECYIL = HRCSUREC.YIL AND HRCPSLGECICIGOREVUCAK.SURECNO = HRCSUREC.SURECNO";
            sql += " INNER JOIN HRCFIRMABILGISI ON HRCPSLGECICIGOREVUCAK.TCKIMLIKVEYAVERGINO = HRCFIRMABILGISI.VERGINO AND HRCSUREC.MUHASEBEKOD = HRCFIRMABILGISI.MUHASEBEKOD";
            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMIFFORM AS MIF ON MIF.YIL= HRCPSLGECICIGOREVUCAK.SURECYIL AND MIF.BELGENO = HRCPSLGECICIGOREVUCAK.ODEMEEMRINO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAK", "SURECYIL", "=", kriter.yil));
            if (!string.IsNullOrWhiteSpace(kriter.firmaKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAK", "TCKIMLIKVEYAVERGINO", "=", kriter.firmaKodu));
            if (kriter.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "MUHASEBEKOD", "=", kriter.muhasebe));
            if (!string.IsNullOrWhiteSpace(kriter.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "BIRIMKOD", "=", kriter.birim));
            if (!string.IsNullOrWhiteSpace(kriter.faturaNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAKFATURA", "FATURANO", "LIKE", "%" + kriter.faturaNo + "%"));
            if (!string.IsNullOrWhiteSpace(kriter.firmaAdi))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCFIRMABILGISI", "AD", "LIKE", kriter.firmaAdi + "%"));
            if (kriter.tip >= 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCPSLGECICIGOREVUCAKFATURA", "KOMISYONMU", "=", kriter.tip));
            if (kriter.mifDurum == -100)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCSUREC", "SURECDURUM", "<", 9));

            return this.veriErisim.DataReaderDondur(sql);
        }
    }
}
