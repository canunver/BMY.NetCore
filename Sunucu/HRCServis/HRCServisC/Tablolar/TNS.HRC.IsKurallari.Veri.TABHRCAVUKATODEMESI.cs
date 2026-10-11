using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCAVUKATODEMESI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCAVUKATODEMESI : TABOrtak
    {
        /// <summary>
        /// TABHRCAVUKATODEMESI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCAVUKATODEMESI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCAVUKATODEMESI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SURECYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYILI = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// TCKIMLIKVEYAVERGINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKVEYAVERGINO = 2,

            /// <summary>
            /// KISIHESAPKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISIHESAPKODU = 3,

            /// <summary>
            /// ODEMEEMRINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRINO = 4,

            /// <summary>
            /// ODEMEEMRIYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEEMRIYIL = 5,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 6,

            /// <summary>
            /// BRUTUCRET alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BRUTUCRET = 7,

            /// <summary>
            /// ODENECEKTUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENECEKTUTAR = 8,

            /// <summary>
            /// TERTIPLER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TERTIPLER = 9,

            /// <summary>
            /// ODEMEHESAPKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODEMEHESAPKODU = 10,

            /// <summary>
            /// PARABIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PARABIRIMI = 11,

            /// <summary>
            /// FATURATARIHI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURATARIHI = 12,

            /// <summary>
            /// FATURANUMARASI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FATURANUMARASI = 13,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SURECYILI"
            , "SURECNO"
            , "TCKIMLIKVEYAVERGINO"
            , "KISIHESAPKODU"
            , "ODEMEEMRINO"
            , "ODEMEEMRIYIL"
            , "GEREKCE"
            , "BRUTUCRET"
            , "ODENECEKTUTAR"
            , "TERTIPLER"
            , "ODEMEHESAPKODU"
            , "PARABIRIMI"
            , "FATURATARIHI"
            , "FATURANUMARASI"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, AvukatOdemesi yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(AvukatOdemesi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYILI], "=", form.surecYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, AvukatOdemesi yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(AvukatOdemesi form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.surecYili);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(form.tcKimlikveyaVergiNo);
            sql += "," + VeriErisim.DegerYap(form.kisiHesapKodu);
            sql += "," + VeriErisim.DegerYap(form.odemeEmriNo);
            sql += "," + VeriErisim.DegerYap(form.odemeEmriYil);
            sql += "," + VeriErisim.DegerYap(form.gerekce);
            sql += "," + VeriErisim.DegerYap(form.brutUcret);
            sql += "," + VeriErisim.DegerYap(form.odenecekTutar);
            sql += "," + VeriErisim.DegerYap(form.tertipler);
            sql += "," + VeriErisim.DegerYap(form.odemeHesapKodu);
            sql += "," + VeriErisim.DegerYap(form.paraBirimi);
            sql += "," + VeriErisim.DegerYap(form.faturaTarihi);
            sql += "," + VeriErisim.DegerYap(form.faturaNumarasi);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, AvukatOdemesi yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(AvukatOdemesi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYILI], "=", form.surecYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc AvukatOdemesiOdemeEmriNoKaydet(AvukatOdemesi avukat)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", "ODEMEEMRINO", avukat.odemeEmriNo);
            sql += ", " + VeriErisim.AtamaOl("", "ODEMEEMRIYIL", avukat.odemeEmriYil);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYILI], "=", avukat.surecYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", avukat.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
