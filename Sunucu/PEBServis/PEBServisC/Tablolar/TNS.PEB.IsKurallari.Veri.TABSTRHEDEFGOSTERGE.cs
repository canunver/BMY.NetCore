using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// STRHEDEFGOSTERGE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTRHEDEFGOSTERGE : TABOrtak
    {
        /// <summary>
        /// TABSTRHEDEFGOSTERGE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTRHEDEFGOSTERGE(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRHEDEFGOSTERGE";

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
            /// GOSTERGEKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOSTERGEKOD = 3,

            /// <summary>
            /// GERCEKLESEN alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GERCEKLESEN = 4,

            /// <summary>
            /// TAHMIN0 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAHMIN0 = 5,

            /// <summary>
            /// TAHMIN1 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAHMIN1 = 6,

            /// <summary>
            /// TAHMIN2 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAHMIN2 = 7,

            /// <summary>
            /// TAHMIN3 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAHMIN3 = 8,

            /// <summary>
            /// TAHMIN4 alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TAHMIN4 = 9,

            GORKOD = 10,
            HEDEFEETKISI = 11,
            IZDONEMI = 12,
            RAPDONEMI = 13,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "GOSTERGEKOD"
            , "GERCEKLESEN"
            , "TAHMIN0"
            , "TAHMIN1"
            , "TAHMIN2"
            , "TAHMIN3"
            , "TAHMIN4"
            , "GORKOD"
            , "HEDEFEETKISI"
            , "IZDONEMI"
            , "RAPDONEMI"
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
        /// Form yapısında duran, hEDEFGosterge dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, HEDEF yapısında nesne</param>
        /// <param name="satirNo">hEDEFGosterge dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(int donemBasYil, int hedefKod, string kurumKod, StrHedefGosterge gosterge)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(donemBasYil);
            sql += "," + VeriErisim.DegerYap(hedefKod);
            sql += "," + VeriErisim.DegerYap(kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(gosterge.gostergeKod);
            sql += "," + VeriErisim.DegerYap(gosterge.gerceklesen);
            sql += "," + VeriErisim.DegerYap(gosterge.tahmin0);
            sql += "," + VeriErisim.DegerYap(gosterge.tahmin1);
            sql += "," + VeriErisim.DegerYap(gosterge.tahmin2);
            sql += "," + VeriErisim.DegerYap(gosterge.tahmin3);
            sql += "," + VeriErisim.DegerYap(gosterge.tahmin4);

            sql += "," + VeriErisim.DegerYap(gosterge.gorKod);
            sql += "," + VeriErisim.DegerYap(gosterge.hedefeEtkisi);
            sql += "," + VeriErisim.DegerYap((int)gosterge.izlemeDonemi);
            sql += "," + VeriErisim.DegerYap((int)gosterge.raporlamaDonemi);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, HEDEF yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(int donemBasYil, int hedefKod, string kurumKod)
        {
            string sql = "SELECT  " + VeriErisim.TumAlanlar("HG", ALANLAR) + ", PG.GOSTERGEAD, PO.OLCUBIRIMAD, PG.KULLANIMSEKLI, STRMISYON.ARALIKBASYIL, STRMISYON.ARALIKSONYIL, PG.TUR FROM " + TABLOAD + " HG INNER JOIN [PerformansButce].[dbo].[PEBGOSTERGE] PG ON HG.GOSTERGEKOD=PG.GOSTERGEKOD";
            sql += " INNER JOIN PerformansButce.dbo.PEBOLCUBIRIM PO ON PG.OLCUBIRIMKOD=PO.OLCUBIRIMKOD ";
            sql += " INNER JOIN STRMISYON ON STRMISYON.DONEMBYIL=HG.DONEMBYIL AND STRMISYON.KURUMKOD=HG.KURUMKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HG", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", donemBasYil));
            if (hedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HG", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", hedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HG", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurumKod.Replace(".", "")));
            sql += " ORDER BY HG.GORKOD";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil} AND IC.HEDEFKOD = {TABLOAD}.HEDEFKOD AND IC.KURUMKOD = {TABLOAD}.KURUMKOD AND IC.GOSTERGEKOD = {TABLOAD}.GOSTERGEKOD) ";

            return KomutCalistir(sql, 0);
        }
    }
}
