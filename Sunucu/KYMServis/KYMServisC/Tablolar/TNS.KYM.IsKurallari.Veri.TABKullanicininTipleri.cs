using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM.IsKurallari;

namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// Kullanıcıların tiplerini tutan "KULLANICININTIPLERI" veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABKullanicininTipleri : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABKullanicininTipleri(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Kullanıcıların tiplerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOAD = "KULLANICININTIPLERI";

        /// <summary>
        /// Kullanıcıların tiplerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// KULLANICIKODU alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// KTKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KTKOD = 1
        }

        /// <summary>
        /// Kullanıcıların tiplerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLAR = { "KULLANICIKODU", "KTKOD" };

        /// <summary>
        /// Veri tabanına kaydedilecek duyuru için sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Kullanıcı tipinin ekleneceği kullanıcının kodu.</param>
        /// <param name="tip">Kullanıcıya eklenecek kullanıcı tipi.</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc Kaydet(string kullaniciKodu, int tip)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ")  VALUES (";
            sql = sql + VeriErisim.DegerYap(kullaniciKodu) + ", ";
            sql = sql + VeriErisim.DegerYap(tip) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir kullanıcıya ait tüm kullanıcı tiplerini silen
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Kullanıcı tipleri silinecek kullanıcının kodu.</param>
        /// <returns>Kullanıcı tiplerini silmek için oluşturulan delete komutu çalıştırılır ve sonucu döndürülür.</returns>
        public Sonuc Sil(string kullaniciKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bir kullanıcıya ait tüm kullanıcı tiplerini döndüren
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="kullaniciKodu">Kullanıcı tipleri listelenecet kullanıcının kodu.</param>
        /// <returns>Kullanıcı tiplerini listeleyen select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public System.Data.IDataReader Listele(string kullaniciKodu)
        {
            string sql = "SELECT " + TABLOAD+"."+ALANLAR[(int)ENUMAlanlar.KTKOD] + "  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, " WHERE ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
