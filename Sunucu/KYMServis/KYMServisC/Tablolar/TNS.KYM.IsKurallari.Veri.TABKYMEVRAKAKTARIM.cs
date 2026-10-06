using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.KYM;
namespace TNS.KYM.IsKurallari.Veri
{
    /// <summary>
    /// KYMEVRAKAKTARIMDETAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKYMEVRAKAKTARIM : TABOrtak
    {
        /// <summary>
        /// TABKYMEVRAKAKTARIMDETAY sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKYMEVRAKAKTARIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "KYMEVRAKAKTARIM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// EVRAKKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EVRAKKODU = 0,

            /// <summary>
            /// ALANKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALANKODU = 1,

            /// <summary>
            /// ALANDEGER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALANDEGER = 2,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "EVRAKKODU"
            , "ALANKODU"
            , "ALANDEGER"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, EvrakAktarim yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(string evrakKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKKODU], "=", evrakKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, evrakAktarimDetay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, EvrakAktarim yapısında nesne</param>
        /// <param name="satirNo">evrakAktarimDetay dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(string evrakKodu, EvrakAktarimMeta form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(evrakKodu);
            sql += "," + VeriErisim.DegerYap(form.alanKodu);
            sql += "," + VeriErisim.DegerYap(form.alanDeger);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, EvrakAktarim yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(string evrakKodu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKKODU], "=", evrakKodu));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
