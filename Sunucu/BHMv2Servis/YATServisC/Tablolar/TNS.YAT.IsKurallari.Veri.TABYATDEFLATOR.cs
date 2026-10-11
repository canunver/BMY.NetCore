using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.YAT;
namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// YATDEFLATOR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABYATDEFLATOR : TABOrtak
    {
        /// <summary>
        /// TABYATDEFLATOR sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABYATDEFLATOR(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// CREATE TABLE YATDEFLATOR
        ///     (YIL INT
        ///     , SENARYOKOD VARCHAR(20)
        ///     , SEKTORKOD VARCHAR(20)
        ///     , DEFLATOR MONEY
        ///     , PRIMARY KEY (YIL, SENARYOKOD, SEKTORKOD)
        ///     )
        /// 
        /// </summary>
        public static string TABLOAD = "YATDEFLATOR";

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
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 1,

            /// <summary>
            /// SEKTORKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SEKTORKOD = 2,

            /// <summary>
            /// DEFLATOR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEFLATOR = 3,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "SENARYOKOD"
            , "SEKTORKOD"
            , "DEFLATOR"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, DeflatorAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, string senaryoKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, deflator dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, DeflatorAna yapısında nesne</param>
        /// <param name="satirNo">deflator dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int yil, string senaryoKod, Deflator form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.sektorKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.deflator);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, DeflatorAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int yil, string senaryoKod, string sektorKod)
        {
            string sql = "SELECT YATSEKTOR.SEKTORNO, YATSEKTOR.AD, YATDEFLATOR.DEFLATOR FROM YATSEKTOR LEFT OUTER JOIN YATDEFLATOR ON YATDEFLATOR.SEKTORKOD=YATSEKTOR.SEKTORNO AND YATDEFLATOR.YIL=YATSEKTOR.YIL AND YATDEFLATOR.SENARYOKOD=" + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("YATSEKTOR", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", "LEN(YATSEKTOR.SEKTORNO)=2");
            if (!string.IsNullOrWhiteSpace(sektorKod))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("YATSEKTOR", "SEKTORNO", "=", sektorKod.Replace(".", "")));
            }
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
