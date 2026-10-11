using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// ABSPEBKOD veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABBHMODENEKONAY : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMODENEKONAY(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// create table BHMODENEKONAY
        /// (
        ///	    YIL int not null,
        ///	    KURKOD varchar(20)not null,
        ///	    SENARYOKOD varchar(20)not null,
        ///	    ONAYKOD int not null,
        ///	    primary key (YIL, KURKOD, SENARYOKOD))
        /// </summary>
        public static string TABLOAD = "BHMODENEKONAY";

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
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 2,

            /// <summary>
            /// ONAYKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYKOD = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "KURKOD"
            , "SENARYOKOD"
            , "ONAYKOD"
        };

        public int OnayKod(int yil, string kurKod, string senaryoKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.ONAYKOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc OnayKodSil(int yil, string birim, string senaryoKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", birim.Replace(".", "")));
            if (senaryoKod != null)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        public Sonuc OnayKodYaz(int yil, string birim, string senaryoKod, int onayDurum)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(birim.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(onayDurum);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public string SenaryoBul(int yil, string kurKod, int onayKod)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SENARYOKOD] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ONAYKOD], "=", onayKod));
            return veriErisim.TekKolonSQLStr(sql);
        }
    }
}
