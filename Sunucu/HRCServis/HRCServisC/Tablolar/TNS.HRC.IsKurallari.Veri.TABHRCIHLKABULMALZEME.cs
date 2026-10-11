using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCIHLKABULMALZEME veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCIHLKABULMALZEME : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCIHLKABULMALZEME(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /*
         * 
         * 
         CREATE TABLE HRCIHLKABULMALZEME
    (SURECYIL INT
    , SURECNO VARCHAR(25)
    , TUTANAKNO INT
    , MALSIRA INT
    , MALKOD VARCHAR(25)
    , MALAD VARCHAR(250)
    , MIKTAR MONEY
    , BIRIMFIYAT MONEY
    PRIMARY KEY (SURECYIL, SURECNO, TUTANAKNO, MALSIRA)
    )

         * 
         * 
         * */

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "HRCIHLKABULMALZEME";

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
            /// MALSIRA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALSIRA = 3,

            /// <summary>
            /// MALKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALKOD = 4,

            /// <summary>
            /// MALAD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MALAD = 5,

            /// <summary>
            /// MIKTAR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 6,

            /// <summary>
            /// BIRIMFIYAT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "SURECYIL"
            , "SURECNO"
            , "TUTANAKNO"
            , "MALSIRA"
            , "MALKOD"
            , "MALAD"
            , "MIKTAR"
            , "BIRIMFIYAT"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, IhaleKabulyapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int surecYil, string surecNo, int tutanakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", tutanakNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, malListesi dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, IhaleKabulyapısında nesne</param>
        /// <param name="satirNo">malListesi dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int surecYil, string surecNo, int tutanakNo, int satirNo, IhaleKabulMalzeme malListesi)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(surecYil);
            sql += "," + VeriErisim.DegerYap(surecNo);
            sql += "," + VeriErisim.DegerYap(tutanakNo);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(malListesi.malKodu);
            sql += "," + VeriErisim.DegerYap(malListesi.malAdi);
            sql += "," + VeriErisim.DegerYap(malListesi.miktar);
            sql += "," + VeriErisim.DegerYap(malListesi.birimFiyat);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, IhaleKabulyapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int surecYil, string surecNo, int tutanakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUTANAKNO], "=", tutanakNo));
            sql += " ORDER BY 1,2,3,4";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
