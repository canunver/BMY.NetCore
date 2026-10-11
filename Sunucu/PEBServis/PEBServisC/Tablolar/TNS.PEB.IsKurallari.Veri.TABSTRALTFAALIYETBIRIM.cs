using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// PEBSPALTFAALIYETBIRIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTRALTFAALIYETBIRIM : TABOrtak
    {
        /// <summary>
        /// TABPEBSPALTFAALIYETBIRIM sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTRALTFAALIYETBIRIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRALTFAALIYETBIRIM";

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
            /// FAALNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 3,

            /// <summary>
            /// FAALNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FAALNO = 4,

            /// <summary>
            /// ALTFAALNO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ALTFAALNO = 5,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 6,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 7,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "YIL"
            , "FAALNO"
            , "ALTFAALNO"
            , "KURKOD"
            , "SIRANO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, SPFaaliyet yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(STRFaaliyet form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.dONEMBYIL));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.hEDEFKOD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kURUMKOD.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALNO], "=", form.fAALNO));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, sPALTFAALIYETBIRIM dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, SPFaaliyet yapısında nesne</param>
        /// <param name="satirNo">sPALTFAALIYETBIRIM dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(STRFaaliyet form, int altFaalNo, string kurKod, int yer)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.dONEMBYIL);
            sql += "," + VeriErisim.DegerYap(form.hEDEFKOD);
            sql += "," + VeriErisim.DegerYap(form.kURUMKOD.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.fAALNO);
            sql += "," + VeriErisim.DegerYap(altFaalNo);
            sql += "," + VeriErisim.DegerYap(kurKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(yer);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, SPFaaliyet yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(STRFaaliyet form, int altFaalNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("T", ALANLAR) + ",KUR.KURAD FROM " + TABLOAD + " T LEFT OUTER JOIN ButceHazirlik.dbo.ABSKUR KUR ON KUR.YIL=T.YIL AND KUR.KURKOD=T.KURKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.dONEMBYIL));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.hEDEFKOD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kURUMKOD.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.FAALNO], "=", form.fAALNO));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.ALTFAALNO], "=", altFaalNo));
            sql += " ORDER BY T.SIRANO";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
