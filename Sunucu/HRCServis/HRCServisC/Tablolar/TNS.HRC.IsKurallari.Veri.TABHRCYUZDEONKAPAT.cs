using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCYUZDEONKAPAT veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCYUZDEONKAPAT : TABOrtak
    {
        /// <summary>
        /// TABHRCYUZDEONKAPAT sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCYUZDEONKAPAT(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
         * 
         CREATE TABLE HRCYUZDEONKAPAT
    (YILI INT
    , MALHIZMETYAPIM VARCHAR(8)
    , TARIH DATETIME
    , ACIKLAMA VARCHAR(400)
    , KAPALI INT
    , TALEPNO INT
    PRIMARY KEY (YILI, MALHIZMETYAPIM, TARIH)
    ) 
         * 
         * */

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCYUZDEONKAPAT";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YILI = 0,

            /// <summary>
            /// MALHIZMETYAPIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALHIZMETYAPIM = 1,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 2,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,

            /// <summary>
            /// KAPALI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KAPALI = 4,

            TALEPNO=5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YILI"
            , "MALHIZMETYAPIM"
            , "TARIH"
            , "ACIKLAMA"
            , "KAPALI"
            , "TALEPNO"
        };


        /// <summary>
        /// Form yapısında duran, yuzdeOnKapat dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, YuzdeOnKapatAna yapısında nesne</param>
        /// <param name="satirNo">yuzdeOnKapat dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(YuzdeOnKapat form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yili);
            sql += "," + VeriErisim.DegerYap(form.malHizmetYapim);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.kapali);
            sql += "," + VeriErisim.DegerYap(form.talepNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, YuzdeOnKapatAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(YuzdeOnKapat form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YILI], "=", form.yili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALHIZMETYAPIM], "=", form.malHizmetYapim));
            sql += " ORDER BY 3 DESC";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
