using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Amortisman yapılacak taşınır malzemelerinin alt sınırını belirleyen yıl bazında bilgileri tutan TASAMORTISMANSINIR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAmortismanSinir : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAmortismanSinir(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASAMORTISMANSINIR";

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
            /// Amortisman sınır alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMORTISMANSINIR = 1,
            MIFBELGENO = 2,
            AMORTISMANSINIRTASIT = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "AMORTISMANSINIR", "MIFBELGENO", "AMORTISMANSINIRTASIT" };

        /// <summary>
        /// Amortisman alt sınır bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sinir">Amortisman sınır bilgilerini tutan, AmortismanSinir türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(AmortismanSinir sinir)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sinir.yil) + ",";
            sql += VeriErisim.DegerYap(sinir.sinir) + ",";
            sql += VeriErisim.DegerYap(sinir.mifBelgeNo) + ",";
            sql += VeriErisim.DegerYap(sinir.sinirTasit) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen yıl bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Amortisman sınır bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortisman Sınır bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int yil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen yıl bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Amortisman sınır değerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortisman Sınır değerini tutan Double türünde nesne döndürür</returns>
        public decimal AmortismanSinirVer(int yil)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.AMORTISMANSINIR] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return System.Convert.ToDecimal(veriErisim.TekKolonSQLDbl(sql));
        }

        public decimal AmortismanSinirTasitVer(int yil)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.AMORTISMANSINIRTASIT] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return System.Convert.ToDecimal(veriErisim.TekKolonSQLDbl(sql));
        }

        /// <summary>
        /// Parametre olarak verilen yıl bilgisini kullanarak
        /// ilgili Amortisman Sınır bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(int yil)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return KomutCalistir(sql, 0);
        }

        public Sonuc MIFKaydet(int yil, string mifNo)
        {
            string sql = "UPDATE  " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MIFBELGENO], mifNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            return KomutCalistir(sql, 1);
        }
    }
}