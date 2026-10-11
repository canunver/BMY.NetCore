using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Birim bilgilerini tutan ABSKUR veri tabanı tablosu
    /// ile ilgili listeleme sql işlemlerini tutan sınıf
    /// </summary>
    public class TABKur : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKur(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ABSKUR";

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
            /// KURAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURAD = 2,

            /// <summary>
            /// KURAD2 alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURAD2 = 3
        }

        /// <summary>
        /// Birim bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "KURAD", "KURAD2" };

        /// <summary>
        /// Parametre olarak verilen yılı ve birim kodunu kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve kriterlere uyan
        /// birim bilgilerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <returns>Verilen kriterlere uygun olan birim bilgilerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string kurKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", VeriErisim.IslecBul("LEN") + "(" + ALANLAR[(int)ENUMAlanlar.KURKOD] + ")", "=", 8));
            //if (!hepsiniListele) 
            //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", VeriErisim.IslecBul("SUBSTRING") + "(" + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", 7, 2)", "<", "60"));

            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurKod.Replace(".", "") + "%"));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}