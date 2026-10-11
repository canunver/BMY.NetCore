using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMPROGRAMONAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMPROGRAMONAY : TABOrtak
    {
        /// <summary>
        /// TABBHMPROGRAMONAY sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMPROGRAMONAY(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// CREATE TABLE BHMPROGRAMONAY
        ///     (BUTCEYILI INT
        ///     , SENARYOKOD VARCHAR(20)
        ///     , PROGRAMKODU VARCHAR(20)
        ///     , DURUM INT
        ///     PRIMARY KEY (BUTCEYILI, SENARYOKOD, PROGRAMKODU)
        ///     )
        /// </summary>
        public static string TABLOAD = "BHMPROGRAMONAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// BUTCEYILI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BUTCEYILI = 0,

            /// <summary>
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 1,

            /// <summary>
            /// PROGRAMKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PROGRAMKODU = 2,

            /// <summary>
            /// DURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "BUTCEYILI"
            , "SENARYOKOD"
            , "PROGRAMKODU"
            , "DURUM"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, ProgramOnayAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ProgramOnay form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BUTCEYILI], "=", form.butceYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", form.senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROGRAMKODU], "=", form.programKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, programOnay dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, ProgramOnayAna yapısında nesne</param>
        /// <param name="satirNo">programOnay dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(ProgramOnay form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.butceYili);
            sql += "," + VeriErisim.DegerYap(form.senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.programKodu);
            sql += "," + VeriErisim.DegerYap(form.durum);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, ProgramOnayAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(ProgramOnay form)
        {
            string sql = "SELECT  P.PEBKOD, P.PEBAD, BHMPROGRAMONAY.DURUM FROM BHMPROGRAMONAY RIGHT OUTER JOIN ButceHazirlik.dbo.ABSPEBKOD P ON ";
            sql += " BHMPROGRAMONAY.BUTCEYILI = P.YIL AND BHMPROGRAMONAY.PROGRAMKODU = P.PEBKOD AND BHMPROGRAMONAY.SENARYOKOD=" + VeriErisim.DegerYap(form.senaryoKod.Replace(".", ""));
            sql += " where LEN(P.PEBKOD)=2";
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", "YIL", "=", form.butceYili));

            if (!string.IsNullOrEmpty(form.programKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROGRAMKODU], "=", form.programKodu));
            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
