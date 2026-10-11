using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Enflasyon Düzeltmesi bilgilerini tutan EnflasyonDuzeltmesi veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABEnflasyonDuzeltmesi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABEnflasyonDuzeltmesi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASENFLASYONDUZELTMESI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            AY = 2,
            YIUFE = 3,
            SINIR = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "YIL", "AY", "YIUFE", "SINIR" };

        /// <summary>
        /// Enflasyon Düzeltmesi bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="enflasyonDuzeltmesi">Enflasyon Düzeltmesi bilgilerini tutan, Enflasyon Düzeltmesi türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(enflasyonDuzeltmesi.kod) + ",";
            sql += VeriErisim.DegerYap(enflasyonDuzeltmesi.yil) + ",";
            sql += VeriErisim.DegerYap(enflasyonDuzeltmesi.ay) + ",";
            sql += VeriErisim.DegerYap(enflasyonDuzeltmesi.yiUfe) + ",";
            sql += VeriErisim.DegerYap(enflasyonDuzeltmesi.sinir) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Enflasyon Düzeltmesi bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="enflasyonDuzeltmesi">Enflasyon Düzeltmesi bilgilerini tutan, Enflasyon Düzeltmesi türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], enflasyonDuzeltmesi.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AY], enflasyonDuzeltmesi.ay) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIUFE], enflasyonDuzeltmesi.yiUfe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIR], enflasyonDuzeltmesi.sinir);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", enflasyonDuzeltmesi.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Enflasyon Düzeltmesi bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Enflasyon Düzeltmesi bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="enflasyonDuzeltmesi">Enflasyon Düzeltmesi bilgilerini tutan, Enflasyon Düzeltmesi türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Enflasyon Düzeltmesi bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (enflasyonDuzeltmesi.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", enflasyonDuzeltmesi.kod));

            if (enflasyonDuzeltmesi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", enflasyonDuzeltmesi.yil));

            if (enflasyonDuzeltmesi.ay > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", enflasyonDuzeltmesi.ay));

            if (enflasyonDuzeltmesi.yiUfe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIUFE], "=", enflasyonDuzeltmesi.yiUfe));

            if (enflasyonDuzeltmesi.sinir > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SINIR], "=", enflasyonDuzeltmesi.sinir));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC," + ALANLAR[(int)ENUMAlanlar.AY] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Enflasyon Düzeltmesi bilgisini kullanarak
        /// Enflasyon Düzeltmesi bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="enflasyonDuzeltmesi">Enflasyon Düzeltmesi bilgilerini tutan, Enflasyon Düzeltmesi türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(EnflasyonDuzeltmesi enflasyonDuzeltmesi)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", enflasyonDuzeltmesi.kod));

            return KomutCalistir(sql, 1);
        }
    }
}