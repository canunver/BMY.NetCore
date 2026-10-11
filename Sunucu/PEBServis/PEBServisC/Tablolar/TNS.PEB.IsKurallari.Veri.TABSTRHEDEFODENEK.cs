using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// STRHEDEFODENEK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTRHEDEFODENEK : TABOrtak
    {
        /// <summary>
        /// TABSTRHEDEFODENEK sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTRHEDEFODENEK(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRHEDEFODENEK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// DONEMBYIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 0,

            /// <summary>
            /// HEDEFKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HEDEFKOD = 1,

            /// <summary>
            /// KURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 2,

            /// <summary>
            /// EKOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 3,

            /// <summary>
            /// ODENEK0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK0 = 4,

            /// <summary>
            /// ODENEK1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK1 = 5,

            /// <summary>
            /// ODENEK2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK2 = 6,

            /// <summary>
            /// ODENEK3 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK3 = 7,

            /// <summary>
            /// ODENEK4 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ODENEK4 = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "EKOKOD"
            , "ODENEK0"
            , "ODENEK1"
            , "ODENEK2"
            , "ODENEK3"
            , "ODENEK4"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, HEDEF yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int donemBasYil, int hedefKod, string kurumKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", donemBasYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", hedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurumKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, hEDEFOdenek dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, HEDEF yapısında nesne</param>
        /// <param name="satirNo">hEDEFOdenek dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int donemBasYil, int hedefKod, string kurumKod, StrHedefOdenek odenek)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(donemBasYil);
            sql += "," + VeriErisim.DegerYap(hedefKod);
            sql += "," + VeriErisim.DegerYap(kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(odenek.ekoKod);
            sql += "," + VeriErisim.DegerYap(odenek.d0);
            sql += "," + VeriErisim.DegerYap(odenek.d1);
            sql += "," + VeriErisim.DegerYap(odenek.d2);
            sql += "," + VeriErisim.DegerYap(odenek.d3);
            sql += "," + VeriErisim.DegerYap(odenek.d4);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, HEDEF yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(int donemBasYil, int hedefKod, string kurumKod)
        {
            string sql1 = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql1 = VeriErisim.KosulEkle(sql1, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", donemBasYil));
            sql1 = VeriErisim.KosulEkle(sql1, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", hedefKod));
            sql1 = VeriErisim.KosulEkle(sql1, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurumKod.Replace(".", "")));

            string sql2 = "SELECT EKOKOD, EKOAD FROM ButceHazirlik.dbo.ABSEKO E WHERE LEN(E.EKOKOD)=2 AND E.YIL=" + VeriErisim.DegerYap(donemBasYil);

            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLAR) + ", E.EKOKOD, E.EKOAD FROM (" + sql1 + ") T RIGHT OUTER JOIN (" + sql2 + ") E ON T.EKOKOD=E.EKOKOD";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
