using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// İş/görevlerin sorumlu personellerini tutan PEBISGOREVSORUMLU veri tabanı
    /// tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABIsGorevSorumlu : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIsGorevSorumlu(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBISGOREVSORUMLU";

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
            /// TCKIMLIKNO alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,

            /// <summary>
            /// GOREV alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GOREV = 5,

            /// <summary>
            /// SORUMLU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SORUMLU = 6
        }

        /// <summary>
        /// İş/görevlerin sorumlu personellerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "ISGOREVKOD", "SIRANO", "TCKIMLIKNO", "GOREV", "SORUMLU" };

        /// <summary>
        /// İş/görevin sorumlu personellerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <param name="sorumlu">İş/görevin sorumlu personeli bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(IsGorev ig, IsGorevSorumlu sorumlu)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ig.yil) + ",";
            sql += VeriErisim.DegerYap(ig.orgKod) + ",";
            sql += VeriErisim.DegerYap(ig.isGorevKod) + ",";
            sql += VeriErisim.DegerYap(sorumlu.siraNo) + ",";
            sql += VeriErisim.DegerYap(sorumlu.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(sorumlu.gorev) + ",";
            if (sorumlu.sorumlu)
                sql += VeriErisim.DegerYap(1) + ")";
            else
                sql += VeriErisim.DegerYap(0) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen iş/görevi kullanarak,
        /// ilgili iş/görevin sorumlu personellerini veri tabanından silen
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
        /// ve iş/göreve ait sorumlu personelleri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="ig">İş/görev bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan sorumlu personelleri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(IsGorev ig)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", PERPERSONEL.AD";
            sql += ", PERPERSONEL.SOYAD";
            sql += " FROM " + TABLOAD;
            sql += ", PERPERSONEL";

            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO] + " = PERPERSONEL.TCKIMLIKNO";

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ig.yil));
            if (ig.orgKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", ig.orgKod));
            if (ig.isGorevKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "=", ig.isGorevKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}