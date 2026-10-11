using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BKDFAALIYETINYAPILDIGIYER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBKDFAALIYETINYAPILDIGIYER : TABOrtak
    {
        /// <summary>
        /// TABBKDFAALIYETINYAPILDIGIYER sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBKDFAALIYETINYAPILDIGIYER(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        ///// <summary>
        ///// Tablo adını tutan sabit değişken
        ///// </summary>
        //public static string TABLOAD = "BKDFAALIYETINYAPILDIGIYER";

        ///// <summary>
        ///// Tablo alanlarının sırasını tutan enumeration
        ///// </summary>
        //public enum ENUMAlanlar : int
        //{

        //    /// <summary>
        //    /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        //    /// </summary>
        //    YIL = 0,

        //    /// <summary>
        //    /// FAALIYETKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        //    /// </summary>
        //    FAALIYETKOD = 1,

        //    /// <summary>
        //    /// MERKEZDAHIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        //    /// </summary>
        //    MERKEZDAHIL = 2,

        //    /// <summary>
        //    /// TUMILVEILCELER alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        //    /// </summary>
        //    TUMILVEILCELER = 3,
        //}

        ///// <summary>
        ///// Tablo alanlarının isimlerini tutan sabit dizi
        ///// </summary>
        //static string[] ALANLAR = {
        //    "YIL"
        //    , "FAALIYETKOD"
        //    , "MERKEZDAHIL"
        //    , "TUMILVEILCELER"
        //};

        ///// <summary>
        ///// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Anahtar değişkenleri tutan, FaaliyetinYapildigiYer yapısında nesne</param>
        ///// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        //public Sonuc Delete(FaaliyetinYapildigiYer form)
        //{
        //    string sql = "DELETE FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod));

        //    return KomutCalistir(sql, 0);
        //}

        ///// <summary>
        ///// Form yapısında duran alanları veri tabanına ekleyen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Eklenecek kayıt bilgisini tutan, FaaliyetinYapildigiYer yapısında nesne</param>
        ///// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        //public Sonuc Insert(FaaliyetinYapildigiYer form)
        //{
        //    string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
        //    sql += "(" + VeriErisim.DegerYap(form.yil);
        //    sql += "," + VeriErisim.DegerYap(form.faaliyetKod);
        //    sql += "," + VeriErisim.DegerYap(form.merkezDahil);
        //    sql += "," + VeriErisim.DegerYap(form.tumilveilceler);
        //    sql += ")";
        //    return KomutCalistir(sql, 1);
        //}

        ///// <summary>
        ///// Form yapısında duran alanları kıstas olarak kullanarak,
        ///// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, FaaliyetinYapildigiYer yapısında nesne</param>
        ///// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        //public IDataReader Get(FaaliyetinYapildigiYer form)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod));

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader TumunuGetir(FaaliyetinYapildigiYer form)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader FaaliyetBul(FaaliyetinYapildigiYer form, int ilkod)
        //{
        //    string sql = "SELECT YIL, FAALIYETKOD, ILKOD";
        //    sql += " FROM BKDFAALIYETINYAPILDIGIYERDETAY";
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod));
        //    sql += " AND (ILKOD = " + ilkod + ")";

        //    return veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader BirimBul(FaaliyetinYapildigiYer form, int birimKod)
        //{
        //    string sql = "SELECT  YIL, FAALIYETKOD, BIRIMKOD";
        //    sql += " FROM BKDFAALIYETINYAPILDIGIYERBIRIM";
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod));
        //    sql += " AND (BIRIMKOD = " + birimKod + ")";

        //    return veriErisim.DataReaderDondur(sql);
        //}
    }
}
