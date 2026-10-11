using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// STRHEDEFACIKLAMA veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTRHEDEFACIKLAMA : TABOrtak
    {
        /// <summary>
        /// TABPEBSTRHEDEFACIKLAMA sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTRHEDEFACIKLAMA(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// CREATE TABLE STRHEDEFACIKLAMA
        ///      (DONEMBYIL INT
        ///      , HEDEFKOD INT
        ///      , KURUMKOD VARCHAR(20)
        ///      , TUR INT
        ///      , SIRANO INT
        ///      , ACIKLAMA VARCHAR(2000)
        ///      , PRIMARY KEY (DONEMBYIL, HEDEFKOD, KURUMKOD, TUR, SIRANO)
        ///      )
        /// </summary>
        public static string TABLOAD = "STRHEDEFACIKLAMA";

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
            /// TUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TUR = 3,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "TUR"
            , "SIRANO"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, StratejikHedef yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(StratejikHedef form, ENUMSTRHEDEFAciklama tur)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.strHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)tur));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, sTRHEDEFACIKLAMA dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, StratejikHedef yapısında nesne</param>
        /// <param name="satirNo">sTRHEDEFACIKLAMA dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(StratejikHedef form, ENUMSTRHEDEFAciklama tur, int satirNo, string aciklama)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.donemBYil);
            sql += "," + VeriErisim.DegerYap(form.strHedefKod);
            sql += "," + VeriErisim.DegerYap(form.kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap((int)tur);
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, StratejikHedef yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(StratejikHedef form, ENUMSTRHEDEFAciklama tur)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.strHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)tur));
            sql += " ORDER BY 5";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil} AND IC.HEDEFKOD = {TABLOAD}.HEDEFKOD AND IC.KURUMKOD = {TABLOAD}.KURUMKOD AND IC.TUR = {TABLOAD}.TUR AND IC.SIRANO = {TABLOAD}.SIRANO) ";

            return KomutCalistir(sql, 0);
        }
    }
}
