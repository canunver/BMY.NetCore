using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{

    /*
     CREATE TABLE PSLCALISANBORDROGELKES
    (GELKESKOD INT
    , GELKESTUR INT
    , HESAPKODU VARCHAR(50)
    , ADI VARCHAR(50)
    , SIRANO VARCHAR(10)
    , KISISIRA VARCHAR(10)
    , FORMUL VARCHAR(500)
    , FORMULSIRA VARCHAR(10)
    PRIMARY KEY (GELKESKOD)
    )
     
     */
    /// <summary>
    /// PSLCALISANBORDROGELKES veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABPSLCALISANBORDROGELKES : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPSLCALISANBORDROGELKES(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PSLCALISANBORDROGELKES";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// GELKESKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELKESKOD = 0,

            /// <summary>
            /// GELKESTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GELKESTUR = 1,

            /// <summary>
            /// HESAPKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPKODU = 2,

            /// <summary>
            /// ADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ADI = 3,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,

            /// <summary>
            /// KISISIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISISIRA = 5,

            /// <summary>
            /// FORMUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMUL = 6,

            /// <summary>
            /// FORMULSIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMULSIRA = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "GELKESKOD"
            , "GELKESTUR"
            , "HESAPKODU"
            , "ADI"
            , "SIRANO"
            , "KISISIRA"
            , "FORMUL"
            , "FORMULSIRA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, CalisanBordroGelirKesintiyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(CalisanBordroGelirKesinti form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GELKESKOD], "=", form.gelirKesintiKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, CalisanBordroGelirKesintiyapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(CalisanBordroGelirKesinti form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.gelirKesintiKod);
            sql += "," + VeriErisim.DegerYap((int)form.gelirKesintiTur);
            sql += "," + VeriErisim.DegerYap(form.hesapKodu);
            sql += "," + VeriErisim.DegerYap(form.adi);
            sql += "," + VeriErisim.DegerYap(form.siraNo);
            sql += "," + VeriErisim.DegerYap(form.kisiSira);
            sql += "," + VeriErisim.DegerYap(form.formul);
            sql += "," + VeriErisim.DegerYap(form.formulSira);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, CalisanBordroGelirKesintiyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(CalisanBordroGelirKesinti form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (form != null)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GELKESKOD], "=", form.gelirKesintiKod));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];
            return veriErisim.DataReaderDondur(sql);
        }


    }
}
