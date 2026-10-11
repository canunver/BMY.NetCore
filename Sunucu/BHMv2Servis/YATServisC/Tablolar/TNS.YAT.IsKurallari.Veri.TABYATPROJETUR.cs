using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Proje türü bilgilerini tutan YATPROJETUR veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABYATProjeTur : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATProjeTur(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATPROJETUR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// TURKODU alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TURKODU = 0,

            /// <summary>
            /// TURADI alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            TURADI = 1
        }

        /// <summary>
        /// Proje türün tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "TURKODU", "TURADI" };

        /// <summary>
        /// Proje tür bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="projeTur">Proje tür bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(Sozluk projeTur)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(projeTur.kod) + ",";
            sql += VeriErisim.DegerYap(projeTur.ad) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanında kayıtlı olan proje türü bilgilerini güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="projeTur">Proje tür bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Guncelle(Sozluk projeTur)
        {
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TURADI], projeTur.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKODU], "=", projeTur.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen proje türü nesnesini kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve proje türlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="projeTur">Proje tür bilgisini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan proje türlerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(Sozluk projeTur)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (projeTur.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKODU], "=", projeTur.kod));

            if (projeTur.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURADI], " LIKE ", "%" + projeTur.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TURKODU];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen proje türü nesnesini kullanarak,
        /// ilgili proje türünü veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="projeTur">Proje tür bilgisini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(Sozluk projeTur)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKODU], "=", projeTur.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen proje tür kodunu kullanarak proje türünün
        /// veri tabanında kayıtlı olup olmadığını kontrol eden yordam
        /// </summary>
        /// <param name="turKodu">Proje tür kodu</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public bool ProjeTurVarMi(int turKodu)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.TURKODU] };
            int[] intDeger ={ turKodu };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        } 
    }
}