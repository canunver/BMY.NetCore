using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Hesap Planı Resim bilgisini tutan TASHESAPPLANRESIM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABHesapPlanResim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHesapPlanResim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASHESAPPLANRESIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Hesap planı kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 0,
            /// <summary>
            /// Resim alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            RESIM = 1
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "HESAPPLANKOD", "RESIM" };

        /// <summary>
        /// Hesap Plani Resim bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hpResim">Hesap Planı Resim bilgilerini tutan, Hesap Plani Satir türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(HesapPlaniSatir hpResim)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hpResim.hesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(hpResim.resim) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Planı Resim bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Hesap Planı Resim bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hpResim">Hesap Planı Resim bilgilerini tutan, Hesap Plani Satir türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Hesap Planı Resim bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(HesapPlaniSatir hpResim)
        {
            string sql = "SELECT TASHESAPPLAN.KOD, TASHESAPPLAN.ACIKLAMA, " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.RESIM];
            sql += " FROM " + TABLOAD + " RIGHT OUTER JOIN TASHESAPPLAN";
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD] + " = TASHESAPPLAN.KOD";

            if (!string.IsNullOrEmpty(hpResim.hesapKod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "LIKE", hpResim.hesapKod.Replace(".", "") + "%"));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Hesap Planı Resim bilgisini kullanarak
        /// ilgili Hesap Planı Resim bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hpResim">Hesap Planı Resim bilgilerini tutan, Hesap Plani Satir türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(HesapPlaniSatir hpResim)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "=", hpResim.hesapKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }
    }
}