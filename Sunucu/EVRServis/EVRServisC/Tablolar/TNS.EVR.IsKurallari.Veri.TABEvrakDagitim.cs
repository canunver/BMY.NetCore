using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
using System.Collections;
namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak dağıtım bilgilerini içeren EVREVRAKDAGITIM veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABEvrakDagitim : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakDagitim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakDagitim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak dağıtım bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKDAGITIM";

        /// <summary>
        /// Evrak dağıtım bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0, SIRANO = 1, DAGITIMKOD = 2, DAGITIMAD = 3, GEREGIBILGI = 4
        }

        /// <summary>
        /// Evrak dağıtım bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "PREVRAKNO", "SIRANO", "DAGITIMKOD", "DAGITIMAD", "GEREGIBILGI" };

        /// <summary>
        /// Parametre olarak verilen evrak bilgisi ve evrakDagitim nesne koleksiyonuna göre; evrak dağıtım bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <param name="liste">EvrakDagitim nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(string prEvrakNo, ArrayList liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            int siraNo = 0;
            foreach (EvrakDagitim form in liste)
            {
                siraNo++;
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(prEvrakNo) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(form.dagitimKod) + ",";
                sql += VeriErisim.DegerYap(form.dagitimAd) + ",";
                sql += VeriErisim.DegerYap(form.geregiBilgi) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen evrak bilgisine göre; evrak dağıtım bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(string prEvrakNo)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.GEREGIBILGI] + "," + ALANLAR[(int)ENUMAlanlar.DAGITIMKOD] + "," + ALANLAR[(int)ENUMAlanlar.DAGITIMAD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre; evrak dağıtım bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
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
