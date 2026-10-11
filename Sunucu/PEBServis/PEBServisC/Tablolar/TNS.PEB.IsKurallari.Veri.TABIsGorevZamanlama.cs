using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// İş/görevlerin zaman planı bilgilerini tutan PEBISGOREVZAMANLAMA veri tabanı
    /// tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABIsGorevZamanlama : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsGorevZamanlama(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBISGOREVZAMANLAMA";

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
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// ISGOREVKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ISGOREVKOD = 2,

            /// <summary>
            /// SIRANO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,

            /// <summary>
            /// BASTARIH alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BASTARIH = 4,

            /// <summary>
            /// BITTARIH alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BITTARIH = 5,

            /// <summary>
            /// BASACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BASACIKLAMA = 6,

            /// <summary>
            /// BITACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            BITACIKLAMA = 7,

            /// <summary>
            /// SURE alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SURE = 8,

            /// <summary>
            /// YER alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YER = 9
        }

        /// <summary>
        /// İş/görevlerin zaman planı bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "ISGOREVKOD", "SIRANO", "BASTARIH",
                                   "BITTARIH", "BASACIKLAMA", "BITACIKLAMA", "SURE", "YER" };

        /// <summary>
        /// İş/görevin zaman planı bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="zamanlama">İş/görevin zaman planı bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(IsGorev ig, IsGorevZamanlama zamanlama)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ig.yil) + ",";
            sql += VeriErisim.DegerYap(ig.orgKod) + ",";
            sql += VeriErisim.DegerYap(ig.isGorevKod) + ",";
            sql += VeriErisim.DegerYap(zamanlama.siraNo) + ",";
            sql += VeriErisim.DegerYap(zamanlama.basTarih) + ",";
            sql += VeriErisim.DegerYap(zamanlama.bitTarih) + ",";
            sql += VeriErisim.DegerYap(zamanlama.basAciklama) + ",";
            sql += VeriErisim.DegerYap(zamanlama.bitAciklama) + ",";
            sql += VeriErisim.DegerYap(zamanlama.sure) + ",";
            sql += VeriErisim.DegerYap(zamanlama.yer) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen iş/görevi kullanarak,
        /// ilgili iş/görevin zaman planı bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(IsGorev ig)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", ig.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", ig.orgKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen iş/görevi kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve iş/göreve ait
        /// zaman planı bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan zaman planı bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(IsGorev ig)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ig.yil));
            if (ig.orgKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", ig.orgKod));
            if (ig.isGorevKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}