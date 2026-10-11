using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCBORDROPARAM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCBORDROPARAM : TABOrtak
    {
        /// <summary>
        /// TABHRCBORDROPARAM sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCBORDROPARAM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCBORDROPARAM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// DEGISKENAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISKENAD = 0,

            /// <summary>
            /// DEGISKENETIKET alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISKENETIKET = 1,

            /// <summary>
            /// SIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRA = 2,

            /// <summary>
            /// TIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIP = 3,

            /// <summary>
            /// TIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YONTEM = 4,

            /// <summary>
            /// LISTE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            LISTE = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "DEGISKENAD"
            , "DEGISKENETIKET"
            , "SIRA"
            , "TIP"
            , "YONTEM"
            , "LISTE"
                                  };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BordroParamAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(string degiskenAd, string degiskenEtiket, string sira, VariableType tip, int yontem, string liste)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DEGISKENETIKET], "=", degiskenEtiket);
            sql += ", " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", sira);
            sql += ", " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)tip);
            sql += ", " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YONTEM], "=", yontem);
            sql += ", " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.LISTE], "=", liste);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DEGISKENAD], "=", degiskenAd));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, bordroParam dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BordroParamAna yapısında nesne</param>
        /// <param name="satirNo">bordroParam dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(string degiskenAd, string degiskenEtiket, string sira, VariableType tip, int yontem, string liste)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(degiskenAd);
            sql += "," + VeriErisim.DegerYap(degiskenEtiket);
            sql += "," + VeriErisim.DegerYap(sira);
            sql += "," + VeriErisim.DegerYap((int)tip);
            sql += "," + VeriErisim.DegerYap(yontem);
            sql += "," + VeriErisim.DegerYap(liste);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BordroParamAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql += " ORDER BY 3,1";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
