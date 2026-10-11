using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BKD.IsKurallari.Veri
{
    /// <summary>
    /// Kod açıklamalarının tutulduğu ABSKODACIKLAMA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABABSKODACIKLAMA : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABABSKODACIKLAMA(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ABSKODACIKLAMA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// DILTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            DILTUR = 1,

            /// <summary>
            /// KODTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KODTUR = 2,

            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 3,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,

            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "DILTUR"
            , "KODTUR"
            , "KOD"
            , "SIRANO"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Açıklama tablosunu, satır numarası hariç anahtar alanlarını kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Açıklaması okunacak kodun yıl bilgisi.</param>
        /// <param name="kodTur">Açıklaması okunacak kodun değişken.xml'deki kod numarası.</param>
        /// <param name="kod">Açıklaması okunacak kodun kod değeri.</param>
        /// <param name="dilKod">Okunacak açıklamanın dili.</param>
        /// <returns>Verilen kriterlere göre oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public IDataReader Listele(int yil, int kodTur, string kod, string dilKod)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.SIRANO] + ", " + ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + " FROM " + TABLOAD;

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILTUR], "=", dilKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", kodTur));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod.Replace(".", "")));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kod açıklamasının bir parçasını veri tabanına kaydeden 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Açıklaması kaydedilecek kodun yıl bilgisi.</param>
        /// <param name="dilKod">Kaydedilecek açıklamanın dili.</param>
        /// <param name="kodTur">Açıklaması kaydedilecek kodun değişken.xml'deki kod numarası.</param>
        /// <param name="kod">Açıklaması kaydedilecek kodun kod değeri.</param>
        /// <param name="sirano">Açıklamanın bölünmesi sonucunda oluşan parçanın sıra numarası.</param>
        /// <param name="parca">Açıklamanın bölünmesi sonucunda oluşan parça.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc Kaydet(int yil, string dilKod, int kodTur, string kod, int sirano, string parca)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(dilKod) + ",";
            sql += VeriErisim.DegerYap(kodTur) + ",";
            sql += VeriErisim.DegerYap(kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sirano) + ",";
            sql += VeriErisim.DegerYap(parca) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir kodun bir dildeki açıklamasını ve tüm dillerdeki açıklamalarını veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Açıklaması silinecek kodun yıl bilgisi.</param>
        /// <param name="dilKod">Silinecek açıklamanın dili. Boş verilirse bir kodun tüm açıklamaları silinir.</param>
        /// <param name="kodTur">Açıklaması silinecek kodun değişken.xml'deki kod numarası.</param>
        /// <param name="kod">Açıklaması silinecek kodun kod değeri.</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc Sil(int yil, string dilKod, int kodTur, string kod)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (!string.IsNullOrEmpty(dilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DILTUR], "=", dilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", kodTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }
    }
}