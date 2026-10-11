using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BHMv2;
namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Tasnif numarası ve ismini tutan BHMBUTCETASNIF veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMBUTCETASNIF : TABOrtak
    {
        /// <summary>
        ///  sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMBUTCETASNIF(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tasnif tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMBUTCETASNIF";

        /// <summary>
        /// Tasnif tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// TASNIFNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASNIFNO = 0,

            /// <summary>
            /// TASNIFAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASNIFAD = 1,

            /// <summary>
            /// TASNIFKURKOD (tasnifin sahibi birim) alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// TUR (tasnifin türü, 1 ise kurum tavanı, 2 ise birim tavanı) alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 3,
        }

        /// <summary>
        /// Tasnif tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "TASNIFNO"
            , "TASNIFAD"
            , "KURKOD"
            , "TUR"
        };

        /// <summary>
        /// Tasniflerle senaryolar arasındaki ilişkiyi tutan tablonun adını tutan sabit değişken
        /// </summary>
        public static string TABLOADTS = "BHMBUTCETASNIFSENARYO";

        /// <summary>
        /// Tasnif tablosunun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarTS : int
        {
            /// <summary>
            /// TASNIFNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TASNIFNO = 0,

            /// <summary>
            /// SIRANO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRANO = 1,

            /// <summary>
            /// SENARYOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 2,
        }

        /// <summary>
        /// Tasnif tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLARTS = {
            "TASNIFNO", "SIRANO", "SENARYOKOD"
        };

        /// <summary>
        /// ButceTasnif yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="butceTasnif">Anahtar alanları tutan, ButceTasnif yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(ButceTasnif butceTasnif)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", butceTasnif.siraNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// ButceTasnif yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="butceTasnif">Eklenecek kayıt bilgisini tutan, ButceTasnif yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(ButceTasnif butceTasnif)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(butceTasnif.siraNo);
            sql += "," + VeriErisim.DegerYap(butceTasnif.ad);
            sql += "," + VeriErisim.DegerYap(butceTasnif.kur.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(butceTasnif.tur);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen tasnif numarasına göre, tasnif kimlik bilgisini bulan,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="tasnifSiraNo">Kimlik bilgisi döndürülecek tasnif numarası parametresi</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(int tasnifSiraNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifSiraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kayıtlı tüm tasnifleri döndüren,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(string kurKod, int tur)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod.Replace(".", "")));
            if (tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Tasnif kayıtlarında bulunan en son tasnif numarasını döndüren yordam.
        /// </summary>
        /// <returns>Tasnif kayıtlarında bulunan en son tasnif numarası döndürülür</returns>
        public int MaksSiraNoBul()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.TASNIFNO] + ")" + " FROM " + TABLOAD;
            return this.veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Bütçe tasnifinin adını veri tabanında güncellemek için 
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="butceTasnif">Güncellenecek tasnif tanım bilgisini tutan nesne</param>
        /// <returns>Oluşturulan update komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Guncelle(ButceTasnif butceTasnif)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASNIFAD], butceTasnif.ad);
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", butceTasnif.siraNo);
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// ButceTasnif yapısında duran anahtar alanları kullanarak, tasnifin verilen senaryo ile ilişkisini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="tasnifSiraNo">İlişkisi silinecek tasnifin sıra numarası.</param>
        /// <param name="senaryoKod">İlişkisi silinecek tasnifin, ilişkili olduğu senaryonun kodu.</param>
        /// <returns>Sil komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc TasnifIliskisiSil(int tasnifSiraNo, string senaryoKod)
        {
            string sql = "DELETE FROM " + TABLOADTS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTS[(int)ENUMAlanlarTS.TASNIFNO], "=", tasnifSiraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTS[(int)ENUMAlanlarTS.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Tasnif kayıtlarında bulunan en son tasnif numarasını döndüren yordam.
        /// </summary>
        /// <returns>Tasnif kayıtlarında bulunan en son tasnif numarası döndürülür</returns>
        public int MaksSiraNoBul(int tasnifSiraNo)
        {
            string sql = "SELECT MAX(" + ALANLARTS[(int)ENUMAlanlarTS.SIRANO] + ")" + " FROM " + TABLOADTS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTS[(int)ENUMAlanlarTS.TASNIFNO], "=", tasnifSiraNo));
            return this.veriErisim.TekKolonSQLInt(sql);
        }


        /// <summary>
        /// ButceTasnif ile verilen senaryo arasındaki ilişkiyi kuran veri tabanı kaydını oluşturan
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="tasnifSiraNo">Senaryo eklenecek kaydın tasnifin sıra numarası.</param>
        /// <param name="sirano">Tasnife eklenecek senaryonun tasnif içi sıra numarası.</param>
        /// <param name="senaryoKod">Tasnife eklenecek senaryonun kodu.</param>
        /// <returns>Ekleme komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür.</returns>
        public Sonuc TasnifIliskisiEkle(int tasnifSiraNo, int sirano, string senaryoKod)
        {
            string sql = "INSERT INTO " + TABLOADTS + "(" + VeriErisim.TumAlanlar("", ALANLARTS) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tasnifSiraNo);
            sql += "," + VeriErisim.DegerYap(sirano);
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen tasnif numarasına göre, tasnifin ilişkili olduğu senaryoları bulan,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon.
        /// </summary>
        /// <param name="tasnifSiraNo">İlişkili senaryoları döndürülecek tasnif numarası parametresi.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür.</returns>
        public IDataReader TasnifIliskileriListele(int tasnifSiraNo)
        {
            string sql = "SELECT ";
            sql += TABLOADTS + "." + ALANLARTS[(int)ENUMAlanlarTS.SIRANO] + ", ";
            sql += TABLOADTS + "." + ALANLARTS[(int)ENUMAlanlarTS.SENARYOKOD] + ", ";
            sql += TABBHMBUTCESENARYO.TABLOAD + "." + TABBHMBUTCESENARYO.ALANLAR[(int)TABBHMBUTCESENARYO.ENUMAlanlar.AD];

            sql += " FROM " + TABLOADTS + " LEFT OUTER JOIN " + TABBHMBUTCESENARYO.TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "ON", TABLOADTS + "." + ALANLARTS[(int)ENUMAlanlarTS.SENARYOKOD] + " = " + TABBHMBUTCESENARYO.TABLOAD + "." + TABBHMBUTCESENARYO.ALANLAR[(int)TABBHMBUTCESENARYO.ENUMAlanlar.KOD]);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTS, ALANLARTS[(int)ENUMAlanlarTS.TASNIFNO], "=", tasnifSiraNo));
            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilSenaryo(int tasnifNo)
        {
            string sql = "DELETE FROM " + TABLOADTS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc Sil(int tasnifNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASNIFNO], "=", tasnifNo));
            return KomutCalistir(sql, 0);
        }

        /*
         * 
CREATE TABLE [dbo].[BHMBUTCETASNIFONAY](
    [YIL] [int] NOT NULL,
    [TASNIFNO] [int] NOT NULL,
    [KURKOD] [varchar](20) NOT NULL,
    [SENARYOKOD] [varchar](20) NOT NULL,
    [ONAYKOD] [int] NOT NULL,
PRIMARY KEY
(
    [YIL] ASC,
    [TASNIFNO] ASC,
    [KURKOD] ASC,
    [SENARYOKOD] ASC
))        
         * */
        public static string TABLOADONAY = "BHMBUTCETASNIFONAY";
        public static string[] ALANLARONAY = { "YIL", "TASNIFNO", "KURKOD", "SENARYOKOD", "ONAYKOD" };
        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarONAY : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TASNIFNO = 1,

            /// <summary>
            /// KURKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// SENARYOKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SENARYOKOD = 3,

            /// <summary>
            /// ONAYKOD alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ONAYKOD = 4,
        }

        public int OnayKod(int tasnifSiraNo, int yil, string kurKod, string senaryoKod)
        {
            string sql = "SELECT " + ALANLARONAY[(int)ENUMAlanlarONAY.ONAYKOD] + " FROM " + TABLOADONAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.TASNIFNO], "=", tasnifSiraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KURKOD], "=", kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc OnayKodSil(int tasnifSiraNo, int yil, string birim, string senaryoKod)
        {
            string sql = "DELETE  FROM " + TABLOADONAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.TASNIFNO], "=", tasnifSiraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.KURKOD], "=", birim.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarONAY.SENARYOKOD], "=", senaryoKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        public Sonuc OnayKodYaz(int tasnifSiraNo, int yil, string birim, string senaryoKod, int onayDurum)
        {
            string sql = "INSERT INTO " + TABLOADONAY + "(" + VeriErisim.TumAlanlar("", ALANLARONAY) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(tasnifSiraNo);
            sql += "," + VeriErisim.DegerYap(birim.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(senaryoKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(onayDurum);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

    }
}
