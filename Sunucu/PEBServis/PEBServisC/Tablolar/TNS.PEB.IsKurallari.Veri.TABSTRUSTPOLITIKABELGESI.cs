using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// PEBSTRUSTPOLITIKABELGESI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTRHEDEFUSTPOLITIKABELGESI : TABOrtak
    {
        /// <summary>
        /// TABPEBSTRUSTPOLITIKABELGESI sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTRHEDEFUSTPOLITIKABELGESI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// CREATE TABLE STRHEDEFUSTPOLITIKABELGESI
        ///    (DONEMBYIL INT
        ///    , STRHEDEFKOD INT
        ///    , KURUMKOD VARCHAR(20)
        ///    , SATIRNO INT
        ///    , USTBELGEKOD INT
        ///    , REFERANS VARCHAR(200)
        ///    , ACIKLAMA VARCHAR(200)
        ///    , PRIMARY KEY (DONEMBYIL, STRHEDEFKOD, KURUMKOD, SATIRNO, USTBELGEKOD)
        ///    )
        /// </summary>
        public static string TABLOAD = "STRHEDEFUSTPOLITIKABELGESI";

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// CREATE TABLE STRUSTPOLITIKABELGESI
        ///    (YIL INT
        ///    , KOD INT
        ///    , AD VARCHAR(100)
        ///    , PRIMARY KEY (YIL, KOD))
        /// </summary>
        public static string TABLOADUPB = "STRUSTPOLITIKABELGESI";

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
            /// STRHEDEFKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            STRHEDEFKOD = 1,

            /// <summary>
            /// KURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 2,

            /// <summary>
            /// KURUMKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SATIRNO = 3,

            /// <summary>
            /// USTBELGEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            USTBELGEKOD = 4,

            /// <summary>
            /// REFERANS alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            REFERANS = 5,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 6,
        }

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarUPB : int
        {

            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 1,

            /// <summary>
            /// AD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 2,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "DONEMBYIL"
            , "STRHEDEFKOD"
            , "KURUMKOD"
            , "SATIRNO"
            , "USTBELGEKOD"
            , "REFERANS"
            , "ACIKLAMA"
        };

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLARUPB = {
            "YIL"
            , "KOD"
            , "AD"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, StrUstPolitikaBelgesiAna yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int donemBYil, int strHedefKod, string kurumKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.STRHEDEFKOD], "=", strHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurumKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, strUstPolitikaBelgesi dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, StrUstPolitikaBelgesiAna yapısında nesne</param>
        /// <param name="satirNo">strUstPolitikaBelgesi dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int donemBYil, int strHedefKod, string kurumKod, int satirNo, StrUstPolitikaBelgesi form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(donemBYil);
            sql += "," + VeriErisim.DegerYap(strHedefKod);
            sql += "," + VeriErisim.DegerYap(kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += "," + VeriErisim.DegerYap(form.ustBelgeKod);
            sql += "," + VeriErisim.DegerYap(form.referans);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, StrUstPolitikaBelgesiAna yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int donemBYil, int strHedefKod, string kurumKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("A", ALANLAR) + ", B.AD FROM " + TABLOAD + " A INNER JOIN " + TABLOADUPB + " B ON A.DONEMBYIL=B.YIL AND A.USTBELGEKOD=B.KOD ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.STRHEDEFKOD], "=", strHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurumKod.Replace(".", "")));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UstPolitikaBelgeleriListele(int yil)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADUPB, ALANLARUPB) + " FROM " + TABLOADUPB;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARUPB[(int)ENUMAlanlarUPB.YIL], "=", yil));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil} AND IC.STRHEDEFKOD = {TABLOAD}.STRHEDEFKOD AND IC.KURUMKOD = {TABLOAD}.KURUMKOD AND IC.SATIRNO = {TABLOAD}.SATIRNO AND IC.USTBELGEKOD = {TABLOAD}.USTBELGEKOD) ";

            return KomutCalistir(sql, 0);
        }
    }
}
