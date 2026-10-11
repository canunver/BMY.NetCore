using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır İşlem Formu Tarihçe bilgisini tutan TASISLEMFORMTARIHCE veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABTarihce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABTarihce(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASISLEMFORMTARIHCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// İşlem yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 4,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 5,
            /// <summary>
            /// İşlem alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEM = 6,
            TASISLEMFORMKOD = 7,
            IPADRESI = 8
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "ISLEMYAPAN", "ISLEMTARIH", "ISLEM", "TASISLEMFORMKOD", "IPADRESI" };

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOADZIMMET = "TASZIMMETFORMTARIHCE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarZimmet
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Belge türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 4,
            /// <summary>
            /// İşlem yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 5,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 6,
            /// <summary>
            /// İşlem alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEM = 7,
            IPADRESI = 8
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARZIMMET = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "BELGETUR", "ISLEMYAPAN", "ISLEMTARIH", "ISLEM", "IPADRESI" };

        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe, Harcama Birimi ve Belge No bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun İşlem Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaBirimi">Harcama Birimi bilgisini tutan, String türünde nesne</param>
        /// <param name="belgeNo">Belge Numarası bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İşlem Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader IslemFormListele(int yil, string muhasebe, string harcamaBirimi, string belgeNo)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + ", (SELECT TOP 1 ONAYDURUM FROM TASISLEMFORM WHERE  TASISLEMFORM.YIL=" + yil + " AND TASISLEMFORM.MUHASEBEKOD='" + muhasebe + "' AND TASISLEMFORM.HARCAMABIRIMKOD='" + harcamaBirimi + "' AND TASISLEMFORM.FISNO = '" + belgeNo + "') AS ONAYDURUM";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", harcamaBirimi));

            if (belgeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", belgeNo));

            sql += " ORDER BY 1,2,3,4,6";

            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Parametre olarak verilen Yıl, Muhasebe, Harcama Birimi, Belge No ve Belge Tür bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Zimmet Form bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="harcamaBirimi">Harcama Birimi bilgisini tutan, String türünde nesne</param>
        /// <param name="belgeNo">Belge Numarası bilgisini tutan, String türünde nesne</param>
        /// <param name="belgeTur">Belge Tür bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Zimmet bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ZimmetListele(int yil, string muhasebe, string harcamaBirimi, string belgeNo, int belgeTur)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARZIMMET) + " FROM " + TABLOADZIMMET;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARZIMMET[(int)ENUMAlanlarZimmet.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARZIMMET[(int)ENUMAlanlarZimmet.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARZIMMET[(int)ENUMAlanlarZimmet.HARCAMABIRIMKOD], "=", harcamaBirimi.Replace(".", "")));

            if (belgeNo != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARZIMMET[(int)ENUMAlanlarZimmet.FISNO], "=", belgeNo));

            if (belgeTur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARZIMMET[(int)ENUMAlanlarZimmet.BELGETUR], "=", belgeTur));

            sql += " ORDER BY 1,2,3,4,6,7 ";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Rapor(string muhasebe, string harcamaBirimi, TNSDateTime baslamaTarihi, TNSDateTime bitisTarihi)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", harcamaBirimi.Replace(".", "")));

            if (!baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], ">=", baslamaTarihi));

            if (!bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], "<=", bitisTarihi));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
