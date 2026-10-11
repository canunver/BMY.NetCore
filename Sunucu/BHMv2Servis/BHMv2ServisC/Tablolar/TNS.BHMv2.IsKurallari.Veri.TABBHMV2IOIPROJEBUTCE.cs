//using System;
//using System.Data;
//using TNS;
//using TNS.Veri;
//using TNS.BHMv2;
//namespace TNS.BHMv2.IsKurallari.Veri
//{
//    /// <summary>
//    /// BHMV2IOIPROJEBUTCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
//    /// </summary>
//    public class TABBHMV2IOIPROJEBUTCE : TABOrtak
//    {
//        /// <summary>
//        /// TABBHMV2IOIPROJEBUTCE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
//        /// </summary>
//        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
//        public TABBHMV2IOIPROJEBUTCE(VeriErisim veriErisim)
//            : base(veriErisim)
//        {
//        }

//        /*
           
//          CREATE TABLE BHMV2IOIPROJEBUTCE
//    (MUHASEBEKOD INT
//    , BIRIMKOD VARCHAR(3)
//    , SIRANO INT
//    , YIL INT
//    , IOIBUTCE MONEY
//    , YIKOBBUTCE MONEY
//    , BAKBUTCE MONEY
//    , IOIHARC MONEY
//    , YIKOBHARC MONEY
//    , BAKHARC MONEY
//    , PRIMARY KEY (MUHASEBEKOD, BIRIMKOD, SIRANO, YIL)
//    )


           
//         * */


//        /// <summary>
//        /// Tablo adını tutan sabit değişken
//        /// </summary>
//        public static string TABLOAD = "BHMV2IOIPROJEBUTCE";

//        /// <summary>
//        /// Tablo alanlarının sırasını tutan enumeration
//        /// </summary>
//        public enum ENUMAlanlar : int
//        {

//            /// <summary>
//            /// MUHASEBEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            MUHASEBEKOD = 0,

//            /// <summary>
//            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            BIRIMKOD = 1,

//            /// <summary>
//            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            SIRANO = 2,

//            /// <summary>
//            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            YIL = 3,

//            /// <summary>
//            /// IOIBUTCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            IOIBUTCE = 4,

//            /// <summary>
//            /// YIKOBBUTCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            YIKOBBUTCE = 5,

//            /// <summary>
//            /// BAKBUTCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            BAKBUTCE = 6,

//            /// <summary>
//            /// IOIHARC alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            IOIHARC = 7,

//            /// <summary>
//            /// YIKOBHARC alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            YIKOBHARC = 8,

//            /// <summary>
//            /// BAKHARC alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
//            /// </summary>
//            BAKHARC = 9,
//        }

//        /// <summary>
//        /// Tablo alanlarının isimlerini tutan sabit dizi
//        /// </summary>
//        static string[] ALANLAR = {
//            "MUHASEBEKOD"
//            , "BIRIMKOD"
//            , "SIRANO"
//            , "YIL"
//            , "IOIBUTCE"
//            , "YIKOBBUTCE"
//            , "BAKBUTCE"
//            , "IOIHARC"
//            , "YIKOBHARC"
//            , "BAKHARC"
//        };

//        /// <summary>
//        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
//        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
//        /// </summary>
//        /// <param name="form">Anahtar değişkenleri tutan, IOIProje yapısında nesne</param>
//        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
//        public Sonuc Sil(IOIProje form)
//        {
//            string sql = "DELETE FROM " + TABLOAD;
//            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
//            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
//            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
//            return KomutCalistir(sql, 0);
//        }

//        /// <summary>
//        /// Form yapısında duran, iOIProjeButce dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
//        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
//        /// </summary>
//        /// <param name="form">Eklenecek kayıt bilgisini tutan, IOIProje yapısında nesne</param>
//        /// <param name="satirNo">iOIProjeButce dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
//        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
//        public Sonuc Ekle(IOIProje form, int satirNo)
//        {
//            IOIProjeButce pb = (IOIProjeButce)form.iOIProjeButce[satirNo];
//            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
//            sql += "(" + VeriErisim.DegerYap(form.muhasebeKod);
//            sql += "," + VeriErisim.DegerYap(form.birimKod);
//            sql += "," + VeriErisim.DegerYap(form.siraNo);
//            sql += "," + VeriErisim.DegerYap(pb.yil);
//            sql += "," + VeriErisim.DegerYap(pb.iOIButce);
//            sql += "," + VeriErisim.DegerYap(pb.yIKOBButce);
//            sql += "," + VeriErisim.DegerYap(pb.bakButce);
//            sql += "," + VeriErisim.DegerYap(pb.iOIHarc);
//            sql += "," + VeriErisim.DegerYap(pb.yIKOBHarc);
//            sql += "," + VeriErisim.DegerYap(pb.bakHarc);
//            sql += ")";
//            return KomutCalistir(sql, 1);
//        }

//        /// <summary>
//        /// Form yapısında duran alanları kıstas olarak kullanarak,
//        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
//        /// </summary>
//        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IOIProje yapısında nesne</param>
//        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
//        public IDataReader Oku(IOIProje form)
//        {
//            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
//            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
//            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
//            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
//            return veriErisim.DataReaderDondur(sql);
//        }

//    }
//}
