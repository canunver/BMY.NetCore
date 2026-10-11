using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BKDFAALIYETINYAPILDIGIYERBIRIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBKDFAALIYETINYAPILDIGIYERBIRIM : TABOrtak
    {
        /// <summary>
        /// BKDFAALIYETINYAPILDIGIYERBIRIM sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBKDFAALIYETINYAPILDIGIYERBIRIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        ///// <summary>
        ///// Tablo adını tutan sabit değişken
        ///// </summary>
        //public static string TABLENAME = "BKDFAALIYETINYAPILDIGIYERBIRIM";

        ///// <summary>
        ///// Tablo alanlarının sırasını tutan enumeration
        ///// </summary>
        //public enum ENUMFields : int
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
        //    /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
        //    /// </summary>
        //    BIRIMKOD = 2,
        //}

        ///// <summary>
        ///// Tablo alanlarının isimlerini tutan sabit dizi
        ///// </summary>
        //static string[] FIELDS = {
        //    "YIL"
        //    , "FAALIYETKOD"
        //    , "BIRIMKOD"
        //};

        ///// <summary>
        ///// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Anahtar değişkenleri tutan, FaaliyetinYapildigiYer yapısında nesne</param>
        ///// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        //public Sonuc Delete(FaaliyetinYapildigiYer form)
        //{
        //    string sql = "DELETE FROM " + TABLENAME;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.YIL], "=", form.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FAALIYETKOD], "=", form.faaliyetKod));

        //    return KomutCalistir(sql, 0);
        //}

        ///// <summary>
        ///// Form yapısında duran alanları veri tabanına ekleyen
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="form">Eklenecek kayıt bilgisini tutan, FaaliyetinYapildigiYer yapısında nesne</param>
        ///// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        //public Sonuc Insert(FaaliyetinYapildigiYer form, int satirNo)
        //{
        //    string sql = "INSERT INTO " + TABLENAME + "(" + VeriErisim.TumAlanlar("", FIELDS) + ") VALUES";
        //    sql += "(" + VeriErisim.DegerYap(form.yil);
        //    sql += "," + VeriErisim.DegerYap(form.faaliyetKod);
        //    sql += "," + VeriErisim.DegerYap(form.faaliyetinYapildigiYerBirim[satirNo].birimKod);
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
        //    string sql = "SELECT " + VeriErisim.TumAlanlar(TABLENAME, FIELDS) + " FROM " + TABLENAME;
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.YIL], "=", form.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", FIELDS[(int)ENUMFields.FAALIYETKOD], "=", form.faaliyetKod));

        //    return veriErisim.DataReaderDondur(sql);
        //}

    }
}
