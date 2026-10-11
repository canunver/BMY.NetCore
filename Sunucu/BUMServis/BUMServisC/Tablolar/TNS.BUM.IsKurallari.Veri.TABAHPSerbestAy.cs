using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// Serbest Ay bilgilerini tutan AHPSERBESTAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAHPSerbestAy : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAHPSerbestAy(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "AHPSERBESTAY";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "KURKOD", "EKOKOD", "AY" };

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Kurumsal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 2,
            /// <summary>
            /// Ay alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AY = 3
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Serbest Ay bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, Serbest Ay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Oku(int yil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " from " + TABLOAD + " where " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil) + " order by 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum bilgilerini kullanarak,
        /// İlgili Serbest Ay bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(int yil, string kurum)
        {
            string sql = "DELETE " + " from " + TABLOAD + " where ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurum);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen Serbest Ay bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurumKod">Kurum kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="ekoKod">Ekonomik kod bilgisini tutan, String türünde nesne</param>
        /// <param name="ay">Ay bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(int yil, string kurumKod, string ekoKod, int ay)
        {
            string sql = "INSERT INTO  " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") " + " VALUES (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(kurumKod) + ", ";
            sql += VeriErisim.DegerYap(ekoKod) + ", ";
            sql += VeriErisim.DegerYap(ay) + ") ";

            return KomutCalistir(sql, 1);
        }

    }
}
