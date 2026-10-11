using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// İş/görevlerin performans göstergelerini tutan PEBISGOREVGOSTERGE veri tabanı
    /// tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABIsGorevGosterge : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsGorevGosterge(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBISGOREVGOSTERGE";

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
            /// GOSTERGEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GOSTERGEKOD = 4,

            /// <summary>
            /// HEDEFLENEN alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            HEDEFLENEN = 5,

            /// <summary>
            /// GERCEKLESEN1 alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GERCEKLESEN1 = 6,

            /// <summary>
            /// GERCEKLESEN2 alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GERCEKLESEN2 = 7,

            /// <summary>
            /// GERCEKLESEN3 alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GERCEKLESEN3 = 8,

            /// <summary>
            /// GERCEKLESEN4 alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GERCEKLESEN4 = 9,

            /// <summary>
            /// ACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 10
        }

        /// <summary>
        /// İş/görevlerin performans göstergelerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "ISGOREVKOD", "SIRANO", "GOSTERGEKOD",
                                   "HEDEFLENEN", "GERCEKLESEN1", "GERCEKLESEN2", "GERCEKLESEN3",
                                   "GERCEKLESEN4", "ACIKLAMA" };

        /// <summary>
        /// İş/görevin performans göstergelerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="gosterge">İş/görevin performans göstergesi bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(IsGorev ig, IsGorevGosterge gosterge)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ig.yil) + ",";
            sql += VeriErisim.DegerYap(ig.orgKod) + ",";
            sql += VeriErisim.DegerYap(ig.isGorevKod) + ",";
            sql += VeriErisim.DegerYap(gosterge.siraNo) + ",";
            sql += VeriErisim.DegerYap(gosterge.gosterge.gostergeKod) + ",";
            sql += VeriErisim.DegerYap(gosterge.hedeflenen) + ",";
            for (int i = 0; i < gosterge.gerceklesen.Length; i++)
                sql += VeriErisim.DegerYap(gosterge.gerceklesen[i]) + ",";
            sql += VeriErisim.DegerYap(gosterge.aciklama) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen iş/görevi kullanarak,
        /// ilgili iş/görevin performans göstergelerini veri tabanından silen
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
        /// select sql cümlesini oluşturup, çalıştıran
        /// ve iş/göreve ait performans göstergelerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergelerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(IsGorev ig)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", PEBGOSTERGE.GOSTERGEAD";
            sql += ", PEBOLCUBIRIM.OLCUBIRIMAD";
            sql += " FROM " + TABLOAD;
            sql += ", PEBGOSTERGE";
            sql += ", PEBOLCUBIRIM";

            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD] + " = PEBGOSTERGE.GOSTERGEKOD";
            sql += " AND PEBGOSTERGE.OLCUBIRIMKOD = PEBOLCUBIRIM.OLCUBIRIMKOD";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ig.yil));
            if ( ig.orgKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", ig.orgKod));
            if (ig.isGorevKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}