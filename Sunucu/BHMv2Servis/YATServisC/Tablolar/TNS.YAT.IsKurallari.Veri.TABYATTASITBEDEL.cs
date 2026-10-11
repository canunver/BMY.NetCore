using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Hizmet alımı suretiyle kullanılan/kullanılacak taşıtlara ilişkin
    /// ödenek teklif bilgilerini tutan YATTASITBEDEL veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATTasitBedel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATTasitBedel(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATTASITBEDEL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// TASITSIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TASITSIRANO = 1,

            /// <summary>
            /// TEKLIFYIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TEKLIFYIL = 2,

            /// <summary>
            /// BEDEL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BEDEL = 3
        }

        /// <summary>
        /// Taşıt bedellerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "TASITSIRANO", "TEKLIFYIL", "BEDEL" };

        /// <summary>
        /// Taşıt bedel bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="tasitSiraNo">Taşıtın cinsi</param>
        /// <param name="teklifYil">Ödeneğin teklif yılı (Çok yıllı bütçe)</param>
        /// <param name="bedel">Taşıt bedeli</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string tasitSiraNo, int teklifYil, double bedel)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(tasitSiraNo) + ",";
            sql += VeriErisim.DegerYap(teklifYil) + ",";
            sql += VeriErisim.DegerYap(bedel) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        /// <summary>
        /// Verilen yılı ve taşıt cinsini kullanarak, ilgili taşıt bedelini veri tabanından
        /// silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="tasitSiraNo">Taşıtın cinsi</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string tasitSiraNo)
        {
            string sql;
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASITSIRANO], "=", tasitSiraNo));

            Sonuc sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen yılı kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve taşıt bedellerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <returns>Verilen kriterlere uygun olan taşıt bedellerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil)
        {
            string sql;
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql += " ORDER BY 2,3";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}