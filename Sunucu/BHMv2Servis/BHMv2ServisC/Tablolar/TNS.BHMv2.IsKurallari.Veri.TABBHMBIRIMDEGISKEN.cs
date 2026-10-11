using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// BHMV2BIRIMDEGISKEN veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMBIRIMDEGISKEN : TABOrtak
    {
        /// <summary>
        /// TABBHMV2BIRIMDEGISKEN sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBIRIMDEGISKEN(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMBIRIMDEGISKEN";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTANIM = "BHMBIRIMDEGISKENTANIM";

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
            /// SAYMANLIK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SAYMANLIK = 1,

            /// <summary>
            /// BIRIM alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIM = 2,

            /// <summary>
            /// KURUMKODU alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKODU = 3,

            /// <summary>
            /// DEGISKENADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISKENADI = 4,

            /// <summary>
            /// DEGERINT alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERINT = 5,

            /// <summary>
            /// DEGERDBL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERDBL = 6,

            /// <summary>
            /// DEGERSTR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERSTR = 7,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTANIM : int
        {
            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 0,

            /// <summary>
            /// DEGISKENADI alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGISKENADI = 1,

            /// <summary>
            /// TIP alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TIP = 2,

            /// <summary>
            /// ORTAK alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORTAK = 3,

            /// <summary>
            /// FORMUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FORMUL = 4,

            /// <summary>
            /// FORMUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KISAETIKET = 5,

            /// <summary>
            /// FORMUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UZUNETIKET = 6,

            /// <summary>
            /// FORMUL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 7,
        }



        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARTANIM = {
            "KURKOD",
            "DEGISKENADI",
            "TIP",
            "ORTAK",
            "FORMUL",
            "KISAETIKET",
            "UZUNETIKET",
            "SIRANO"
                                       };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SAYMANLIK"
            , "BIRIM"
            , "KURUMKODU"
            , "DEGISKENADI"
            , "DEGERINT"
            , "DEGERDBL"
            , "DEGERSTR"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, BirimDegisken yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int saymanlik, string birim, string kurumKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKODU], "=", kurumKodu.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="degisken">Eklenecek kayıt bilgisini tutan, BirimDegisken yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, int saymanlik, string birim, string kurumKodu, BirimDegisken degisken)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(saymanlik);
            sql += "," + VeriErisim.DegerYap(birim);
            sql += "," + VeriErisim.DegerYap(kurumKodu.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(degisken.degiskenAdi);
            sql += "," + VeriErisim.DegerYap(degisken.degerInt);
            sql += "," + VeriErisim.DegerYap(degisken.degerDbl);
            sql += "," + VeriErisim.DegerYap(degisken.degerStr);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, BirimDegisken yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, int saymanlik, string birim, string kurumKodu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIK], "=", saymanlik));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKODU], "=", kurumKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YapiListele(string kurKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTANIM, ALANLARTANIM) + " FROM " + TABLOADTANIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTANIM[(int)ENUMAlanlarTANIM.KURKOD], "=", kurKod.Replace(".", "")));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc YapiSil(string kurKod)
        {
            string sql = "DELETE FROM " + TABLOADTANIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTANIM[(int)ENUMAlanlarTANIM.KURKOD], "=", kurKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        public Sonuc YapiEkle(string kurKod, BirimDegiskenTanim tanim)
        {
            string sql = "INSERT INTO " + TABLOADTANIM + "(" + VeriErisim.TumAlanlar("", ALANLARTANIM) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(kurKod.Replace(".",""));
            sql += "," + VeriErisim.DegerYap(tanim.degiskenAdi);
            sql += "," + VeriErisim.DegerYap((int)tanim.tip);
            sql += "," + VeriErisim.DegerYap(tanim.ortak?1:0);
            sql += "," + VeriErisim.DegerYap(tanim.formula);
            sql += "," + VeriErisim.DegerYap(tanim.kisaEtiket);
            sql += "," + VeriErisim.DegerYap(tanim.aciklama);
            sql += "," + VeriErisim.DegerYap(tanim.siraNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }
    }
}
