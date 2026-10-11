using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
using static TNS.HRC.IsKurallari.Veri.TABDBHAKEDIS;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCDBHAKEDISTARIHCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABDBTARIHCE : TABOrtak
    {
        /// <summary>
        /// TABHRCDBHAKEDISTARIHCE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABDBTARIHCE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "DBTARIHCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// HAKEDISID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIHCEID = 0,

            /// <summary>
            /// HAKEDISID alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODULKOD = 1,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MODULANAHTAR = 2,

            /// <summary>
            /// ISLEMTARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIHI = 3,

            /// <summary>
            /// ISLEMIYAPAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMIYAPAN = 4,

            /// <summary>
            /// ISLEM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEM = 5,

            /// <summary>
            /// ISLEM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKBILGI = 6,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "TARIHCEID"
            , "MODULKOD"
            , "MODULANAHTAR"
            , "ISLEMTARIHI"
            , "ISLEMIYAPAN"
            , "ISLEM"
            , "EKBILGI"
        };

        /// <summary>
        /// Form yapısında duran, dBHakedisTarihce dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <param name="satirNo">dBHakedisTarihce dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(string modulKod, string modulAnahtar, string islemiYapan, string islem, string ekBilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(modulKod);
            sql += "," + VeriErisim.DegerYap(modulAnahtar);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap(islemiYapan);
            sql += "," + VeriErisim.DegerYap(islem);
            sql += "," + VeriErisim.DegerYap(ekBilgi);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DBHakedis yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(string modulKod, string modulAnahtar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODULKOD], "=", modulKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MODULANAHTAR], "=", modulAnahtar));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI] + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
