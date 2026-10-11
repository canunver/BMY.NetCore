using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.PEB;
namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// STRHEDEFBIRIM veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABSTRHEDEFBIRIM : TABOrtak
    {
        /// <summary>
        /// TABSTRHEDEFBIRIM sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABSTRHEDEFBIRIM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "STRHEDEFBIRIM";

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
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 3,

            /// <summary>
            /// SIRANO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "DONEMBYIL"
            , "HEDEFKOD"
            , "KURUMKOD"
            , "KURKOD"
            , "SIRANO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, StratejikHedef yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(StratejikHedef form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.strHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran, sTRHEDEFBIRIM dizisinin, satirno sırasındaki satırını veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, StratejikHedef yapısında nesne</param>
        /// <param name="satirNo">sTRHEDEFBIRIM dizisinin kacıncı satırının ekleneceğini gösteren parametre</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(StratejikHedef form, string kod, int satirNo)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.donemBYil);
            sql += "," + VeriErisim.DegerYap(form.strHedefKod);
            sql += "," + VeriErisim.DegerYap(form.kurumKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(kod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(satirNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, StratejikHedef yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(StratejikHedef form)
        {
            string sql = "SELECT T.KURKOD,K.KURAD FROM " + TABLOAD + " T INNER JOIN ButceHazirlik.dbo.ABSKUR K ON K.YIL=T." + ALANLAR[(int)ENUMAlanlar.DONEMBYIL] + " AND K.KURKOD=T.KURKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", form.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", form.strHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("T", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", form.kurumKod.Replace(".", "")));
            sql += " ORDER BY SIRANO";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader RaporHedefBirim(StratejikHedef hedef)
        {
            string sql = "SELECT H.HEDEFKISA, H.HEDEFKISA + ' - ' + Y.KISAYAZI, K.KURKOD, K.KURAD, CASE WHEN HB.SIRANO=0 THEN 'S' ELSE 'İ' END ";
            sql += " FROM " + TABLOAD + " HB INNER JOIN " + TABHedef.TABLOAD + " H ON H.DONEMBYIL = HB.DONEMBYIL AND H.HEDEFKOD=HB.HEDEFKOD and H.KURUMKOD=HB.KURUMKOD";
            sql += " INNER JOIN " + TABYazi.TABLOAD + " Y ON Y.DONEMBYIL = H.DONEMBYIL AND Y.KOD=H.HEDEFKOD and H.KURUMKOD=HB.KURUMKOD and Y.KODTUR=" + VeriErisim.DegerYap((int)ENUMStratejikPlan.StratejikHedef); 
            sql += " INNER JOIN ButceHazirlik.dbo.ABSKUR K ON K.YIL=HB." + ALANLAR[(int)ENUMAlanlar.DONEMBYIL] + " AND K.KURKOD=HB.KURKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("HB", ALANLAR[(int)ENUMAlanlar.DONEMBYIL], "=", hedef.donemBYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("HB", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", hedef.kurumKod.Replace(".", "")));
            sql += " ORDER BY 1,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = $"INSERT INTO {TABLOAD}(" + VeriErisim.TumAlanlar("", ALANLAR) + ")";
            sql += $" SELECT {hedefYil} DONEMBYIL, " + VeriErisim.TumAlanlarEksik("", ALANLAR, new int[] { 0 }) + " FROM " + TABLOAD;
            sql += $" WHERE DONEMBYIL={kaynakYil} AND 1 NOT IN (SELECT 1 FROM {TABLOAD} IC WHERE IC.DONEMBYIL={hedefYil} AND IC.HEDEFKOD = {TABLOAD}.HEDEFKOD AND IC.KURUMKOD = {TABLOAD}.KURUMKOD AND IC.KURKOD = {TABLOAD}.KURKOD) ";

            return KomutCalistir(sql, 0);
        }
    }
}
