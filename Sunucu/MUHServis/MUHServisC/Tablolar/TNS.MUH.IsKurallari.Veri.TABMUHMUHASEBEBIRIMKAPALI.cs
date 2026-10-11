using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHMUHASEBEBIRIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMUHMUHASEBEBIRIMKAPALI : TABOrtak
    {
        /// <summary>
        /// TABMUHMUHASEBEBIRIM sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMUHMUHASEBEBIRIMKAPALI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MUHMUHASEBEBIRIMKAPALI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// MUHASEBE alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,

            /// <summary>
            /// BIRIMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 1,

            /// <summary>
            /// KURUM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "MUHASEBEKOD"
            , "BIRIMKOD"
            , "KURKOD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, MuhasebeBirim yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(MuhasebeBirim form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", form.kurum.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, MuhasebeBirim yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(MuhasebeBirim form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.muhasebe);
            sql += "," + VeriErisim.DegerYap(form.birimKod);
            sql += "," + VeriErisim.DegerYap(form.kurum.Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, MuhasebeBirim yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Lislele(MuhasebeBirim form)
        {
            string sql = "SELECT MUHMUHASEBEBIRIMKAPALI.MUHASEBEKOD, MUHMUHASEBEBIRIMKAPALI.BIRIMKOD, MUHMUHASEBEBIRIMKAPALI.KURKOD, MUHMUHASEBEBIRIM.BIRIMAD, MUHMUHASEBE.MUHASEBEAD ";
            sql += "FROM MUHMUHASEBEBIRIMKAPALI LEFT OUTER JOIN MUHMUHASEBE ON MUHMUHASEBEBIRIMKAPALI.MUHASEBEKOD = MUHMUHASEBE.MUHASEBEKOD ";
            sql += "LEFT OUTER JOIN MUHMUHASEBEBIRIM ON MUHMUHASEBEBIRIMKAPALI.BIRIMKOD = MUHMUHASEBEBIRIM.BIRIMKOD AND MUHMUHASEBEBIRIMKAPALI.MUHASEBEKOD = MUHMUHASEBEBIRIM.MUHASEBEKOD AND  MUHMUHASEBEBIRIMKAPALI.KURKOD = MUHMUHASEBEBIRIM.KURKOD";

            string kosul = "";
            if (form.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebe));
            if (!string.IsNullOrWhiteSpace(form.birimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
            if (!string.IsNullOrWhiteSpace(form.kurum))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", form.kurum.Replace(".", "") + "%"));

            if (!string.IsNullOrWhiteSpace(kosul))
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
