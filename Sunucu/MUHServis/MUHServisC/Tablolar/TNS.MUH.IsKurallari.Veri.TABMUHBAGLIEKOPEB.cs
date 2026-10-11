using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.MUH;
namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHBAGLIEKOPEB veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// 
    /// CREATE TABLE MUHBAGLIEKOPEB
    /// (TERTIP VARCHAR(30)
    /// , PEB VARCHAR(20)
    /// PRIMARY KEY (TERTIP, PEB)
    /// )
    /// 
    /// 
    /// </summary>
    public class TABMUHBAGLIEKOPEB : TABOrtak
    {
        /// <summary>
        /// TABMUHBAGLIEKOPEB sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMUHBAGLIEKOPEB(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "MUHBAGLIEKOPEB";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// EKO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TERTIP = 0,

            /// <summary>
            /// PEB alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEB = 1,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
              "TERTIP"
            , "PEB"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BagliEkoPeb yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(BagliEkoPeb form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TERTIP], "=", form.tertip.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEB], "=", form.peb.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, bagliEkoPeb dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, BagliEkoPeb yapısında nesne</param>
        /// <param name="satirNo">bagliEkoPeb dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(BagliEkoPeb form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.tertip.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.peb.Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BagliEkoPeb yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(BagliEkoPeb form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";
            if (!string.IsNullOrEmpty(form.tertip))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TERTIP], "=", form.tertip.Replace(".", "")));
            if (!string.IsNullOrEmpty(form.peb))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEB], "=", form.peb.Replace(".","")));
            if (!string.IsNullOrEmpty(kosul))
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
