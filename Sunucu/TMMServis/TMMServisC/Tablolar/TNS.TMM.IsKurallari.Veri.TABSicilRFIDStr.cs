using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Sicil RFID bilgilerini tutan TASSICILNORFID veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABSicilRFIDStr : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSicilRFIDStr(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASSICILNORFID";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// RFID no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            RFIDNO = 0,
            /// <summary>
            /// Programatik sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSICILNO = 1,
            /// <summary>
            /// Kill no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KILLNO = 2,
            /// <summary>
            /// Access no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACCESSNO = 3,
            /// <summary>
            /// Tag type alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAGTYPE = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "RFIDNO", "PRSICILNO", "KILLNO", "ACCESSNO", "TAGTYPE" };

        /// <summary>
        /// Sicil RFID bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="rfId">Sicil RFID bilgisini tutan, SicilRFID türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(SicilNoHareket rfId)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(rfId.epc) + ",";
            sql += VeriErisim.DegerYap(rfId.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(rfId.killNo) + ",";
            sql += VeriErisim.DegerYap(rfId.accessNo) + ",";
            sql += VeriErisim.DegerYap(rfId.tagType) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil RFID bilgisini kullanarak
        /// Sicil RFID  bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="rfId">Sicil RFID bilgisini tutan, SicilRFID türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(SicilNoHareket rfId)
        {
            string sql = "DELETE FROM " + TABLOAD;
            if (rfId.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", rfId.prSicilNo));
            else if (rfId.epc != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RFIDNO], "=", rfId.epc));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen RFID numarasının,
        /// Daha önce kullanılıp kullanılmadığı bilgisini dönen,
        /// sql komutunu oluşturan, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="rfIdNo">RFID no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, RFID bilgisi kullanılmışsa true, kullanılmamışsa false bilgisini tutan Bool türünde nesne döndürür</returns>
        public int RfIdBostaMi(string rfIdNo)
        {
            string sql = "SELECT PRSICILNO FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RFIDNO], "=", rfIdNo));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader Listele(string rfIdNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RFIDNO], "=", rfIdNo));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}