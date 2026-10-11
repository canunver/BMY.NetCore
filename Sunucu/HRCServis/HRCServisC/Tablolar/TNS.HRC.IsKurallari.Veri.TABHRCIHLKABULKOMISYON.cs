using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCIHLKABULKOMISYON veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCIHLKABULKOMISYON : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCIHLKABULKOMISYON(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 

CREATE TABLE HRCIHLKABULKOMISYON
    (SURECYIL INT
    , SURECNO VARCHAR(25)
    , TUTANAKNO INT
    , KISISIRA INT
    , TCKIMLIK VARCHAR(15)
    , GOREV VARCHAR(50)
    PRIMARY KEY (SURECYIL, SURECNO, TUTANAKNO, KISISIRA)
    )

         
         * 
         * */

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCIHLKABULKOMISYON";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// SURECYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECYIL = 0,

            /// <summary>
            /// SURECNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SURECNO = 1,

            /// <summary>
            /// TUTANAKNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUTANAKNO = 2,

            /// <summary>
            /// KISISIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISISIRA = 3,

            /// <summary>
            /// TCKIMLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIK = 4,

            /// <summary>
            /// GOREV alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREV = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SURECYIL"
            , "SURECNO"
            , "TUTANAKNO"
            , "KISISIRA"
            , "TCKIMLIK"
            , "GOREV"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, IhaleKabulyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(IhaleKabul form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", form.tutanakNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, komUyeleri dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, IhaleKabulyapısında nesne</param>
        /// <param name="satirNo">komUyeleri dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(IhaleKabul form, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.surecYil);
            sql += "," + VeriErisim.DegerYap(form.surecNo);
            sql += "," + VeriErisim.DegerYap(form.tutanakNo);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(form.komUyeleri[satirNo].tcKimlik);
            sql += "," + VeriErisim.DegerYap(form.komUyeleri[satirNo].gorev);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IhaleKabulyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(IhaleKabul form)
        {
            string sql = "SELECT HRCIHLKABULKOMISYON.KISISIRA, HRCIHLKABULKOMISYON.TCKIMLIK, HRCIHLKABULKOMISYON.GOREV, HRCPERSONEL.ADSOYAD";
            sql +=" FROM HRCIHLKABULKOMISYON LEFT OUTER JOIN HRCPERSONEL ON HRCIHLKABULKOMISYON.TCKIMLIK = HRCPERSONEL.TCKIMLIKNO";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HRCIHLKABULKOMISYON", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", form.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCIHLKABULKOMISYON", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", form.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HRCIHLKABULKOMISYON", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", form.tutanakNo));

            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
