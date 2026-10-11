using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2PROJEGEREKCE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYATALTPROJETEKLIF : TABOrtak
    {
        /// <summary>
        /// TABBHMV2ALTPROJETEKLIF sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATALTPROJETEKLIF(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
CREATE TABLE YATALTPROJETEKLIF (
      YIL INT
    , BIRIM VARCHAR(20)
    , SEKTORNO VARCHAR(20)
    , ALTPROJENO INT
    , AD VARCHAR(250)
    , BASLANGICYILI INT
    , BITISYILI INT
    , PROJETUTARI MONEY
    , YILITEKLIFI0 MONEY
    , YILITEKLIFI1 MONEY
    , YILITEKLIFI2 MONEY
    , GEREKCE VARCHAR(500)
    PRIMARY KEY (YIL, BIRIM, SEKTORNO, ALTPROJENO)
)
         * 
         * */
        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "YATALTPROJETEKLIF";

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
            /// BIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIM = 1,

            /// <summary>
            /// SEKTORNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SEKTORNO = 2,

            /// <summary>
            /// ALTPROJENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTPROJENO = 3,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 4,

            /// <summary>
            /// BASLANGICYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BASLANGICYILI = 5,

            /// <summary>
            /// BITISYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BITISYILI = 6,

            /// <summary>
            /// PROJETUTARI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROJETUTARI = 7,

            /// <summary>
            /// YILITEKLIFI0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YILITEKLIFI0 = 8,

            /// <summary>
            /// YILITEKLIFI1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YILITEKLIFI1 = 9,

            /// <summary>
            /// YILITEKLIFI2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YILITEKLIFI2 = 10,

            /// <summary>
            /// GEREKCE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GEREKCE = 11,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "BIRIM"
            , "SEKTORNO"
            , "ALTPROJENO"
            , "AD"
            , "BASLANGICYILI"
            , "BITISYILI"
            , "PROJETUTARI"
            , "YILITEKLIFI0"
            , "YILITEKLIFI1"
            , "YILITEKLIFI2"
            , "GEREKCE"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BirimButceKarsilama yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(AltProjeTeklif form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", form.birim).Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], "=", form.sektorNo.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, projeGerekce dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BirimButceKarsilama yapısında nesne</param>
        /// <param name="satirNo">projeGerekce dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, string birim, string sektorNo, int siraNo, AltProjeTeklif form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(birim.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(sektorNo.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(siraNo);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.baslangicYil);
            sql += "," + VeriErisim.DegerYap(form.bitisYil);
            sql += "," + VeriErisim.DegerYap(form.projeTutari);
            sql += "," + VeriErisim.DegerYap(form.yiliTeklifi0);
            sql += "," + VeriErisim.DegerYap(form.yiliTeklifi1);
            sql += "," + VeriErisim.DegerYap(form.yiliTeklifi2);
            sql += "," + VeriErisim.DegerYap(form.gerekce);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BirimButceKarsilama yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(AltProjeTeklif form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            if (!string.IsNullOrWhiteSpace(form.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "LIKE", form.birim.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(form.sektorNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SEKTORNO], "LIKE", form.sektorNo.Replace(".", "") + "%"));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
