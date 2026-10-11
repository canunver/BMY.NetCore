using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /*CREATE TABLE UAKISMIGECICIIMZA
    (SURECYIL INT
    , SURECNO VARCHAR(25)
    , MALSIRANO INT
    , KISISIRANO INT
    , TCKIMLIKNO VARCHAR(0)
    , AD VARCHAR(30)
    , SOYAD VARCHAR(30)
    , UNVAN VARCHAR(50)
    PRIMARY KEY (SURECYIL, SURECNO, MALSIRANO, KISISIRANO)
    )*/
    /// <summary>
    /// UAKISMIGECICIIMZA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABUAKISMIGECICIIMZA : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABUAKISMIGECICIIMZA(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCUAKISMIGECICIIMZA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SURECYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// MALSIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALSIRANO = 2,

            /// <summary>
            /// KISISIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISISIRANO = 3,
            /// <summary>
            /// TCKIMLIKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 4,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 5,

            /// <summary>
            /// SOYAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOYAD = 6,

            /// <summary>
            /// UNVAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UNVAN = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SURECYIL"
            , "SURECNO"
            , "MALSIRANO"
            , "KISISIRANO"
            , "TCKIMLIKNO"
            , "AD"
            , "SOYAD"
            , "UNVAN"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, KismiGeciciKabulyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(KismiGeciciKabul form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALSIRANO], "=", form.malzemeSiraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, kismiGeciciKabulImza dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, KismiGeciciKabulyapısında nesne</param>
        /// <param name="satirNo">kismiGeciciKabulImza dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(KismiGeciciKabul form, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.surecYil);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(form.malzemeSiraNo);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(form.kismiGeciciKabulImza[satirNo].TcKimlikNo);
            sql += "," + VeriErisim.DegerYap(form.kismiGeciciKabulImza[satirNo].Ad);
            sql += "," + VeriErisim.DegerYap(form.kismiGeciciKabulImza[satirNo].Soyad);
            sql += "," + VeriErisim.DegerYap(form.kismiGeciciKabulImza[satirNo].Unvan);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, KismiGeciciKabulyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(KismiGeciciKabul form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MALSIRANO], "=", form.malzemeSiraNo));
            sql += " ORDER BY KISISIRANO";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
