using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHISTATISTIKKOD veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMUHISTATISTIKKOD : TABOrtak
    {
        /// <summary>
        /// TABMUHISTATISTIKKOD sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMUHISTATISTIKKOD(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MUHISTATISTIKKOD";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            , AD = 1
            , ZORUNLU = 2
            , EKALAN1AD = 3
            , EKALAN2AD = 4
            , EKALAN3AD = 5
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "KOD"
            , "AD"
            , "ZORUNLU"
            , "EKALAN1AD"
            , "EKALAN2AD"
            , "EKALAN3AD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, IstatistikKod yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(IstatistikKod form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, IstatistikKod yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(IstatistikKod form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.kod);
            sql += "," + VeriErisim.DegerYap(form.ad);
            sql += "," + VeriErisim.DegerYap(form.zorunlu);
            sql += "," + VeriErisim.DegerYap(form.ekAlan1Ad);
            sql += "," + VeriErisim.DegerYap(form.ekAlan2Ad);
            sql += "," + VeriErisim.DegerYap(form.ekAlan3Ad);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, IstatistikKod yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(IstatistikKod form)
        {
            string sql = "UPDATE " + TABLOAD + " SET  ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZORUNLU], form.zorunlu);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKALAN1AD], form.ekAlan1Ad);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKALAN2AD], form.ekAlan2Ad);
            sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKALAN3AD], form.ekAlan3Ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IstatistikKod yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(IstatistikKod form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return veriErisim.DataReaderDondur(sql);
        }

        public int KodBul()
        {
            string sql = "SELECT MAX(KOD) FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}
