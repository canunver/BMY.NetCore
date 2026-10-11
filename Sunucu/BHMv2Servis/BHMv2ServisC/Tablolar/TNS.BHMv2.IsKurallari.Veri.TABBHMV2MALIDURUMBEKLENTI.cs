using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2MALIDURUMBEKLENTI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMV2MALIDURUMBEKLENTI : TABOrtak
    {
        /// <summary>
        /// TABBHMV2MALIDURUMBEKLENTI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMV2MALIDURUMBEKLENTI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMV2MALIDURUMBEKLENTI";

        /*
         * 
         CREATE TABLE BHMV2MALIDURUMBEKLENTI
    (YIL INT
    , FAALIYETKOD VARCHAR(20)
    , BUTCEUYGULAMASONUCU VARCHAR(MAX)
    , YURUTULENFAALIYETLER VARCHAR(MAX)
    , BEKLENTILERVEHEDEFLER VARCHAR(MAX)
    , YURUTULECEKFAALIYETLER VARCHAR(MAX)
    PRIMARY KEY (YIL, FAALIYETKOD)
    )
         * 
         * */

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// FAALIYETKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAALIYETKOD = 1,

            /// <summary>
            /// BUTCEUYGULAMASONUCU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEUYGULAMASONUCU = 2,

            /// <summary>
            /// YURUTULENFAALIYETLER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YURUTULENFAALIYETLER = 3,

            /// <summary>
            /// BEKLENTILERVEHEDEFLER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BEKLENTILERVEHEDEFLER = 4,

            /// <summary>
            /// YURUTULECEKFAALIYETLER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YURUTULECEKFAALIYETLER = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "FAALIYETKOD"
            , "BUTCEUYGULAMASONUCU"
            , "YURUTULENFAALIYETLER"
            , "BEKLENTILERVEHEDEFLER"
            , "YURUTULECEKFAALIYETLER"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, MaliDurumBeklenti yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MaliDurumBeklenti form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, MaliDurumBeklenti yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(MaliDurumBeklenti form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.faaliyetKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.butceUygulamaSonucu);
            sql += "," + VeriErisim.DegerYap(form.yurutulenFaaliyetler);
            sql += "," + VeriErisim.DegerYap(form.beklentilerveHedefler);
            sql += "," + VeriErisim.DegerYap(form.yurutulecekFaaliyetler);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, MaliDurumBeklenti yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(MaliDurumBeklenti form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "LIKE", form.faaliyetKod.Replace(".", "") + "%"));
            sql += " order by YIL, FAALIYETKOD";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
