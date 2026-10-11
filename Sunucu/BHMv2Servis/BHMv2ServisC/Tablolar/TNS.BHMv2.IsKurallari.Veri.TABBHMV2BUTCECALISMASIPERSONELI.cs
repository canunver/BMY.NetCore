using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2BUTCECALISMASIPERSONELI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMV2BUTCECALISMASIPERSONELI : TABOrtak
    {
        /// <summary>
        /// TABBHMV2BUTCECALISMASIPERSONELI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMV2BUTCECALISMASIPERSONELI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMV2BUTCECALISMASIPERSONELI";

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
            /// PEBKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 1,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 2,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 3,

            /// <summary>
            /// SOYAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SOYAD = 4,

            /// <summary>
            /// UNVAN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UNVAN = 5,

            /// <summary>
            /// BIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIM = 6,

            /// <summary>
            /// ISTEL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISTEL = 7,

            /// <summary>
            /// CEPTEL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEPTEL = 8,

            /// <summary>
            /// FAKS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAKS = 9,

            /// <summary>
            /// EPOSTA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EPOSTA = 10,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "PEBKOD"
            , "SIRANO"
            , "AD"
            , "SOYAD"
            , "UNVAN"
            , "BIRIM"
            , "ISTEL"
            , "CEPTEL"
            , "FAKS"
            , "EPOSTA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, ButceCalismasiPersonelAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string pebKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (pebKod != null)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", pebKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, butceCalismasiPersoneli dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, ButceCalismasiPersonelAna yapısında nesne</param>
        /// <param name="satirNo">butceCalismasiPersoneli dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(ButceCalismasiPersoneli form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.pebkod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.soyad);
            sql += "," + VeriErisim.DegerYap(form.unvan);
            sql += "," + VeriErisim.DegerYap(form.birim);
            sql += "," + VeriErisim.DegerYap(form.isTel);
            sql += "," + VeriErisim.DegerYap(form.cepTel);
            sql += "," + VeriErisim.DegerYap(form.faks);
            sql += "," + VeriErisim.DegerYap(form.ePosta);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, ButceCalismasiPersonelAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(ButceCalismasiPersoneli form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLAR) + ", P.PEBAD FROM " + TABLOAD + " T INNER JOIN ButceHazirlik.dbo.ABSPEBKOD P ON T.YIL=P.YIL AND T.PEBKOD=P.PEBKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.PEBKOD], "like", form.pebkod.Replace(".", "") + "%"));
            sql += " order by 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int yil)
        {
            string sql = "INSERT INTO " + TABLOAD + " SELECT YIL+1 YIL,  PEBKOD, SIRANO, AD, SOYAD, UNVAN, BIRIM, ISTEL, CEPTEL, FAKS, EPOSTA FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil - 1));
            return KomutCalistir(sql, 0);
        }
    }
}
