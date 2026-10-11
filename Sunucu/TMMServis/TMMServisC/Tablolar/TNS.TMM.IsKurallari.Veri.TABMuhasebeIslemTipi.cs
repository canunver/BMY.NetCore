using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Ölçü birimi bilgilerini tutan MuhasebeIslemTipi veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABMuhasebeIslemTipi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMuhasebeIslemTipi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASMUHASEBEISLEMTIPI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            TIP = 1,
            AD = 2
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "TIP", "AD" };

        /// <summary>
        /// Ölçü birimi bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeIslemTip">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(MuhasebeIslemTipi muhasebeIslemTip)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(muhasebeIslemTip.kod) + ",";
            sql += VeriErisim.DegerYap(muhasebeIslemTip.tip) + ",";
            sql += VeriErisim.DegerYap(muhasebeIslemTip.ad) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Ölçü birimi bilgilerini veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeIslemTip">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(MuhasebeIslemTipi muhasebeIslemTip)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], muhasebeIslemTip.tip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], muhasebeIslemTip.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", muhasebeIslemTip.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Ölçü Birim bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Ölçü birim bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeIslemTip">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Ölçü birim bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(MuhasebeIslemTipi muhasebeIslemTip)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (muhasebeIslemTip.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", muhasebeIslemTip.kod));

            if (muhasebeIslemTip.tip != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], " =", muhasebeIslemTip.tip));

            if (muhasebeIslemTip.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + muhasebeIslemTip.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Ölçü Birim bilgisini kullanarak
        /// Ölçü birim bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebeIslemTip">Ölçü birimi bilgilerini tutan, Ölçü birim türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(MuhasebeIslemTipi muhasebeIslemTip)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", muhasebeIslemTip.kod));

            return KomutCalistir(sql, 1);
        }
    }
}