using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
using System.Collections;
namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak ilgi bilgilerini içeren EVREVRAKILGI veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABEvrakIlgi : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakIlgi sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakIlgi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak ilgi bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKILGI";

        /// <summary>
        /// Evrak ilgi bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0,
            SIRANO = 1,
            ILGITARIH = 2,
            ILGINO = 3,
            ACIKLAMA = 4,
        }

        /// <summary>
        /// Evrak ilgi bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "PREVRAKNO", "SIRANO", "ILGITARIH", "ILGINO", "ACIKLAMA" };

        /// <summary>
        /// Parametre olarak verilen evrak bilgisi ve EvrakIlgi nesne koleksiyonuna göre; evrak ilgi bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <param name="liste">EvrakIlgi nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(EvrakIlgi form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.ilgiTarihi) + ",";
            sql += VeriErisim.DegerYap(form.ilgiNo) + ",";
            sql += VeriErisim.DegerYap(form.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre; evrak ilgi bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(string prEvrakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            sql += " ORDER BY 1,2,3";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre; evrak ilgi bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(string prEvrakNo)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }
    }
}
