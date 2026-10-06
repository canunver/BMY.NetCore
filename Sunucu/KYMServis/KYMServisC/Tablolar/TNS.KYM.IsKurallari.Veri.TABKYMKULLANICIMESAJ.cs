using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /*
     * 
     * 
CREATE TABLE KYMKULLANICIMESAJ
    (KULLANICIKODU VARCHAR(20)
    , MESAJNO INT
    , TARIH DATETIME
    , MESAJ VARCHAR(255)
    PRIMARY KEY (KULLANICIKODU, MESAJNO)
    )

     
     * 
     * */

    /// <summary>
    /// KYMKULLANICIMESAJ veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKYMKULLANICIMESAJ : TABOrtak
    {
        /// <summary>
        /// TABKYMKULLANICIMESAJ sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKYMKULLANICIMESAJ(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "KYMKULLANICIMESAJ";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// KULLANICIKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KULLANICIKODU = 0,

            /// <summary>
            /// MESAJNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MESAJNO = 1,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 2,

            /// <summary>
            /// MESAJ alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MESAJ = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KULLANICIKODU"
            , "MESAJNO"
            , "TARIH"
            , "MESAJ"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, KullaniciMesaj yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(KullaniciMesaj form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MESAJNO], "=", form.mesajNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, KullaniciMesaj yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(KullaniciMesaj form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(form.mesajNo);
            sql += "," + VeriErisim.DegerYap(form.tarih);
            sql += "," + VeriErisim.DegerYap(form.mesaj);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, KullaniciMesaj yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(KullaniciMesaj form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", form.kullaniciKodu));
            if (form.mesajNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MESAJNO], "=", form.mesajNo));
            sql += " ORDER BY 2 DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        public int MaxMesajNo(string kullaniciKodu)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.MESAJNO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKodu));
            return (int)veriErisim.TekKolonSQLDbl(sql);
        }
    }
}
