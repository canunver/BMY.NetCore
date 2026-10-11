using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Bütçe gelir fişi ile ilgili, gerekçeyi tutan BHMBUTCEGELIRFISIGEREKCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMBUTCEGELIRFISI : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBUTCEGELIRFISI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Gerekçe tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADBILGI = "BHMBUTCEGELIRFISIGEREKCE";

        /// <summary>
        /// Gerekçe tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarBilgi : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 2,

            /// <summary>
            /// PARCA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PARCA = 3,

            /// <summary>
            /// BILGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BILGI = 4,
        }

        /// <summary>
        /// Gerekçe tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARBILGI = {
            "YIL",
            "KURKOD",
            "EKOKOD",
            "PARCA",
            "BILGI"
        };

        /// <summary>
        /// Veri tabanına tertip için kaydedilecek açıklamalar parça parça kaydedilmektedir. Kaydedilecek parça için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Açıklama parçası eklenecek tertip</param>
        /// <param name="parcaNo">Eklenecek açıklamanın parça numarası</param>
        /// <param name="bilgi">Eklenecek açıklamanın parçası</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc KaydetGerekce(Tertip tertip, int parcaNo, string bilgi)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADBILGI + " (" + VeriErisim.TumAlanlar("", ALANLARBILGI) + ") VALUES (";
            sql += VeriErisim.DegerYap(tertip.yil) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(parcaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen tertiple ilgili veri tabanına kaydedilmiş gerekçenin tüm parçalarını döndürmek için 
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Açıklama parçaları döndürülecek tertip</param>
        /// <returns>Tertip parçalarını bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleGerekce(Tertip tertip)
        {
            string sql = string.Empty;

            sql = "SELECT " + ALANLARBILGI[(int)ENUMAlanlarBilgi.PARCA] + ", " + ALANLARBILGI[(int)ENUMAlanlarBilgi.BILGI];
            sql = sql + " FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD], "=", tertip.kodlar[1].Replace(".", "")));
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen tertiple ilgili veri tabanına kaydedilmiş gerekçenin tüm parçalarını silmek için 
        /// delete sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tertip">Açıklama parçaları silinecek tertip</param>
        /// <returns>Tertip parçalarını silmek için oluşturulan delete komutu çalıştırılır ve sonucu döndürülür</returns>
        public Sonuc SilGerekce(Tertip tertip)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "=", tertip.kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD], "=", tertip.kodlar[1].Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bütçe fişlerini bir önceki yıldan alıp, bu yıla kopyalayan sql komutunu oluşturup çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="tertip">Kopyalanacak bütçe fişlerinin tertip şablonunu tutan nesne.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc ButceFisiKopyalaOncYil(Tertip tertip)
        {
            string sql = "INSERT INTO " + TABLOADBILGI + " (";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.PARCA] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.BILGI] + ") SELECT ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL] + "+1, ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.PARCA] + ", ";
            sql += ALANLARBILGI[(int)ENUMAlanlarBilgi.BILGI] + " FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil - 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Bütçe fişi girilmiş olan tertip sayısını bulup döndüren sql komutunu hazırlayıp, çalıştırıp sonucunu döndüren yordam.
        /// </summary>
        /// <param name="tertip">Sayısı bulunacak bütçe fişlerinin tertip şablonunu tutan nesne.</param>
        /// <returns>Bütçe fişi sayısı döndürülür.</returns>
        public int ButceFisiSayisiBul(Tertip tertip)
        {
            string sql = "select COUNT(DISTINCT " + ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD] + "+" + ALANLARBILGI[(int)ENUMAlanlarBilgi.EKOKOD] + ") FROM " + TABLOADBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.YIL], "=", tertip.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARBILGI[(int)ENUMAlanlarBilgi.KURKOD], "LIKE", tertip.kodlar[0].Replace(".", "") + "%"));

            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
