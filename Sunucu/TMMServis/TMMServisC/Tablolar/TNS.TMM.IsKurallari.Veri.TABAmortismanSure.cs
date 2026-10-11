using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır malzemelerinin amortisman süreleri bilgisini tutan TASAMORTISMANSURE veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAmortismanSure : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAmortismanSure(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASAMORTISMANSURE";

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
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 1,
            /// <summary>
            /// Amortisman süre alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMORTISMANSURE = 2
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "HESAPPLANKOD", "AMORTISMANSURE" };

        /// <summary>
        /// Amortisman Süre bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sure">Amortisman Süre bilgilerini tutan, Amortisman Süre türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(AmortismanSure sure)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sure.yil) + ",";
            sql += VeriErisim.DegerYap(sure.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sure.sure) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Amortisman Süre bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sure">Amortisman Süre bilgilerini tutan, Amortisman Süre türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(AmortismanSure sure)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMORTISMANSURE], sure.sure);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sure.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "=", sure.hesapPlanKod.Replace(".", "")));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman Süre bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Amortisman Süre bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sure">Amortisman Süre bilgilerini tutan, Amortisman Süre türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortisman Süre bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(AmortismanSure sure)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (sure.yil > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sure.yil);
            if (!string.IsNullOrEmpty(sure.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "LIKE", sure.hesapPlanKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman Süre bilgisini kullanarak
        /// ilgili Amortisman Süre bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="sure">Amortisman Süre bilgilerini tutan, Amortisman Süre türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(AmortismanSure sure)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", sure.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "=", sure.hesapPlanKod.Replace(".", "")));
            return KomutCalistir(sql, 1);
        }
    }
}