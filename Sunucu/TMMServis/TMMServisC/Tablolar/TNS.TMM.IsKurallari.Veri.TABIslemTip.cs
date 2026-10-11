using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// İşlem Tipi bilgilerini tutan TASISLEMTIP veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABIslemTip : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIslemTip(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "TASISLEMTIP";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// İşlem tipi kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// İşlem tipi adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1,
            /// <summary>
            /// İşlem tipi türü alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUR = 2
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR ={ "KOD", "AD", "TUR" };

        /// <summary>
        /// İşlem Tipi bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="islemTipi">İşlem Tipi bilgilerini tutan, İşlemTip türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(IslemTip islemTipi)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(islemTipi.kod) + ",";
            sql += VeriErisim.DegerYap(islemTipi.ad) + ",";
            sql += VeriErisim.DegerYap(islemTipi.tur) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// İşlem Tipi bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="islemTipi">İşlem Tipi bilgilerini tutan, İşlemTip türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(IslemTip islemTipi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], islemTipi.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], islemTipi.tur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", islemTipi.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen İşlem Tipi bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun İşlem Tipi bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="islemTipi">İşlem Tipi bilgilerini tutan, İşlemTip türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İşlem Tipi bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(IslemTip islemTipi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (islemTipi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", islemTipi.kod));

            if (islemTipi.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + islemTipi.ad + "%"));

            if (islemTipi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", islemTipi.tur));

            if (kosul != "")
                sql += " WHERE " + kosul;

            //sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen İşlem Tipi bilgisini kullanarak
        /// ilgili İşlem Tipi bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="islemTipi">İşlem Tipi bilgilerini tutan, İşlemTip türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(IslemTip islemTipi)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", islemTipi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen İşlem Tipi bilgilerini kullanarak,
        /// Kayıtlı olan Taşınır İşlem Form bilgilerini dönen,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tip">İşlem Tipi bilgilerini tutan, İşlemTip türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Taşınır İşlem Form bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader KullanilmisMi(IslemTip tip)
        {
            string sql = "SELECT YIL FROM TASISLEMFORM";
            sql = sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TASISLEMFORM", "ISLEMTIPIKOD", "=", tip.kod));
            sql = sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMFORM", "DURUM", "<>", (int)ENUMBelgeDurumu.IPTAL));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}