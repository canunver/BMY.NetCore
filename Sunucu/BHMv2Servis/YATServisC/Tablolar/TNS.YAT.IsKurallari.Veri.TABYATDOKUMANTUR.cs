using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Doküman türü bilgilerini tutan YATDOKUMANTUR veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATDokumanTur : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATDokumanTur(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATDOKUMANTUR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DOKUMANTURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANTURKOD = 0,

            /// <summary>
            /// DOKUMANTURAD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DOKUMANTURAD = 1
        }

        /// <summary>
        /// Doküman türün tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "DOKUMANTURKOD", "DOKUMANTURAD" };

        /// <summary>
        /// Doküman tür bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="dokumanTur">Doküman tür bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Sozluk dokumanTur)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(dokumanTur.kod) + ",";
            sql += VeriErisim.DegerYap(dokumanTur.ad) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan doküman türü bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="dokumanTur">Doküman tür bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(Sozluk dokumanTur)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANTURAD], dokumanTur.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANTURKOD], "=", dokumanTur.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen doküman türü nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve doküman türlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="dokumanTur">Doküman tür bilgisini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan doküman türlerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Sozluk dokumanTur)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (dokumanTur.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANTURKOD], "=", dokumanTur.kod));

            if (dokumanTur.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANTURAD], " LIKE ", "%" + dokumanTur.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DOKUMANTURKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen doküman türü nesnesini kullanarak,
        /// ilgili doküman türünü veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="dokumanTur">Doküman tür bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Sozluk dokumanTur)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOKUMANTURKOD], "=", dokumanTur.kod));

            return KomutCalistir(sql, 1);
        }
    }
}