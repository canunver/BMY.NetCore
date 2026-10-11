using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// İl bilgilerini tutan YATIL veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATIl : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATIl(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATIL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// ILKODU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILKODU = 0,

            /// <summary>
            /// ILADI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ILADI = 1,

            /// <summary>
            /// ACIKLAMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 2
        }

        /// <summary>
        /// İllerin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "ILKODU", "ILADI", "ACIKLAMA" };

        /// <summary>
        /// İl bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Sozluk il)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(il.kod) + ",";
            sql += VeriErisim.DegerYap(il.ad) + ",";
            sql += VeriErisim.DegerYap(il.aciklama) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan il bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(Sozluk il)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILADI], il.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], il.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], "=", il.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen il nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve illeri DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan illeri tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Sozluk il)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (il.kod == 100)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], "<=", il.kod));
            else if (il.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], "=", il.kod));

            if (il.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILADI], " LIKE ", "%" + il.ad + "%"));

            if (il.aciklama != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", "%" + il.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ILKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen il nesnesini kullanarak, ilgili ili veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="il">İl bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Sozluk il)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKODU], "=", il.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen il kodunu kullanarak ilin veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="ilKodu">İl kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool IlVarMi(int ilKodu)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.ILKODU] };
            int[] intDeger ={ ilKodu };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }
    }
}