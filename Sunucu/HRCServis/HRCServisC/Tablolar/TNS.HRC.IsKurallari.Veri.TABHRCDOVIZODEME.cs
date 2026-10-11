using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCDOVIZODEME veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCDOVIZODEME : TABOrtak
    {

        /*
         * 
create table HRCDOVIZODEME
( SURECYILI INT NOT NULL
, SURECNO VARCHAR(20) NOT NULL
, TCKIMLIKVEYAVERGINO VARCHAR(20) NOT NULL
, ODEMEEMRINO VARCHAR(20) NULL
, ODEMEEMRIYIL INT NULL
, GEREKCE VARCHAR(2000) NOT NULL
, TUTAR MONEY
, TERTIPLER VARCHAR(200) 
, ODEMEHESAPKODU VARCHAR(20) 
, PRIMARY KEY (SURECYILI, SURECNO)
)         
         * 
         * */

        /// <summary>
        /// TABHRCDOVIZODEME sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCDOVIZODEME(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLENAME = "HRCDOVIZODEME";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMFields : int
        {

            /// <summary>
            /// SURECYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            SURECYILI = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// TCKIMLIKVEYAVERGINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            TCKIMLIKVEYAVERGINO = 2,

            /// <summary>
            /// ODEMEEMRINO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            ODEMEEMRINO = 3,

            /// <summary>
            /// ODEMEEMRIYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            ODEMEEMRIYIL = 4,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            GEREKCE = 5,

            /// <summary>
            /// TUTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            TUTAR = 6,

            /// <summary>
            /// TERTIPLER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            TERTIPLER = 7,

            /// <summary>
            /// ODEMEHESAPKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            ODEMEHESAPKODU = 8,

            /// <summary>
            /// PARABIRIMI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)"
            /// </summary>
            PARABIRIMI = 9,

        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] FIELDS = {
            "SURECYILI"
            , "SURECNO"
            , "TCKIMLIKVEYAVERGINO"
            , "ODEMEEMRINO"
            , "ODEMEEMRIYIL"
            , "GEREKCE"
            , "TUTAR"
            , "TERTIPLER"
            , "ODEMEHESAPKODU"
            , "PARABIRIMI"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name=\"form\">Anahtar değişkenleri tutan, " + icAd + "yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Delete(DovizOdeme form)
        {
            string sql = "DELETE FROM " + TABLENAME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SURECYILI], "=", form.surecYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SURECNO], "=", form.surecNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name=\"form\">Eklenecek kayıt bilgisini tutan, DovizOdeme yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Insert(DovizOdeme form)
        {
            string sql = "INSERT INTO " + TABLENAME + "(" + VeriErisim.TumAlanlar("", FIELDS) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.surecYili);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(form.tcKimlikveyaVergiNo);
            sql += "," + VeriErisim.DegerYap(form.odemeemriNo);
            sql += "," + VeriErisim.DegerYap(form.odemeemriyil);
            sql += "," + VeriErisim.DegerYap(form.gerekce);
            sql += "," + VeriErisim.DegerYap(form.tutar);
            sql += "," + VeriErisim.DegerYap(form.tertipler);
            sql += "," + VeriErisim.DegerYap(form.odemeHesapKodu);
            sql += "," + VeriErisim.DegerYap(form.paraBirimi);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name=\"form\">Select edilecek kayıtlara ait kıstas bilgisini tutan, DovizOdeme yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Get(DovizOdeme form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLENAME, FIELDS) + " FROM " + TABLENAME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SURECYILI], "=", form.surecYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SURECNO], "=", form.surecNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DovizOdemeOdemeEmriNoKaydet(DovizOdeme doviz)
        {
            string sql = "";

            sql = "UPDATE " + TABLENAME + " SET ";
            sql += VeriErisim.AtamaOl("", "ODEMEEMRINO", doviz.odemeemriNo);
            sql += ", " + VeriErisim.AtamaOl("", "ODEMEEMRIYIL", doviz.odemeemriyil);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SURECYILI], "=", doviz.surecYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.SURECNO], "=", doviz.surecNo));

            return KomutCalistir(sql, 0);
        }
    }
}
