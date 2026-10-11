using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak dosya planı bilgilerini içeren EVREVRAKDOSYAPLANI veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABEvrakDosyaPlani : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakDosyaPlanisınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakDosyaPlani(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak dosya planı bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKDOSYAPLANI";

        /// <summary>
        /// Evrak dosya planı bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            PREVRAKNO = 0,
            DOSYAPLANKODU = 1,
        }

        /// <summary>
        /// Evrak dosya planı bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "PREVRAKNO", "DOSYAPLANKODU" };

        /// <summary>
        /// Parametre olarak verilen evrak bilgisi ve dosya planı bilgisine göre; evrak dosya plan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <param name="dosya">Dosya planı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(string prEvrakNo, DosyaPlani dosya)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(dosya.kod) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(string prEvrakNo, DosyaPlani dosya)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], dosya.kod);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paramete olarak verilen evrak bilgisine göre; evrak dosya plan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(string prEvrakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABDosyaPlani.TABLOAD + ".AD" + "  FROM " + TABLOAD;
            sql += "  LEFT OUTER  JOIN " + TABDosyaPlani.TABLOAD + " ON " + TABDosyaPlani.TABLOAD + ".KOD =" + TABLOAD + ".DOSYAPLANKODU ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele2(string prEvrakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABDosyaPlani.TABLOAD, TABDosyaPlani.ALANLAR) + " FROM " + TABLOAD;
            sql += " INNER JOIN EVRDOSYAPLANI ON EVRDOSYAPLANI.KOD = EVREVRAKDOSYAPLANI.DOSYAPLANKODU ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public bool EvrakNoVeKodBul(string prEvrakNo,string kod)
        {
            string sql = "SELECT COUNT(1) " + "FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], "=", kod));
            return veriErisim.TekKolonSQLDbl(sql) > 0;
        }

        /// <summary>
        /// Paramete olarak verilen evrak bilgisine göre; evrak dosya plan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(string prEvrakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Paramete olarak verilen evrak bilgisi ve dosya plan koduna göre; dosya sıra numarasını, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="prEvrakNo">Evrak prEvrakNo bilgisi içeren nesne</param>
        /// <param name="dosyaKodu">Dosya plan kodu bilgisi içeren nesne</param>
        /// <returns></returns>
        public int DosyaNumarasiGetir(string prEvrakNo, string dosyaKodu)
        {
            string sql = "SELECT SIRANO FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "PREVRAKNO", "=", prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "DOSYAPLANKODU", "=", dosyaKodu));

            return veriErisim.TekKolonSQLInt(sql);
        }

        /// <summary>
        /// Toplus the degistir.
        /// </summary>
        /// <param name="eskiKod">The eski kod.</param>
        /// <param name="yeniKod">The yeni kod.</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <returns></returns>
        public IDataReader TopluDegistirListele(string eskiKod, TNSDateTime tarih1, TNSDateTime tarih2)
        {
            string sql = " SELECT EVREVRAKSICIL.PREVRAKNO,EVREVRAKSICIL.EVREVRAKNO ";
            sql += " FROM EVREVRAK INNER JOIN EVREVRAKSICIL ON EVREVRAK.EVREVRAKNO = EVREVRAKSICIL.EVREVRAKNO INNER JOIN ";
            sql += " EVREVRAKDOSYAPLANI ON EVREVRAKSICIL.PREVRAKNO = EVREVRAKDOSYAPLANI.PREVRAKNO ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], "=", eskiKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EVREVRAK.EVRAKTARIH", ">=", tarih1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "EVREVRAK.EVRAKTARIH", "<=", tarih2));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Toplus the degistir hazirla.
        /// </summary>
        /// <param name="prEvrakNo">The pr evrak no.</param>
        /// <param name="eskiKod">The eski kod.</param>
        /// <param name="yeniKod">The yeni kod.</param>
        /// <returns></returns>
        public Sonuc TopluDegistirHazirla(string prEvrakNo, string eskiKod, string yeniKod)
        {
            string sql = "DELETE FROM EVREVRAKDOSYAPLANI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], "=", eskiKod));
            sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], "=", yeniKod));
            sql += ")";

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Toplus the degistir tarihce.
        /// </summary>
        /// <param name="evrEvrakNo">The evr evrak no.</param>
        /// <param name="islemYapanTC">The islem yapan TC.</param>
        /// <param name="islemYapanAd">The islem yapan ad.</param>
        /// <param name="eskiKod">The eski kod.</param>
        /// <param name="yeniKod">The yeni kod.</param>
        /// <param name="tarih1">The tarih1.</param>
        /// <param name="tarih2">The tarih2.</param>
        /// <param name="aciklama">The aciklama.</param>
        /// <returns></returns>
        public Sonuc TopluDegistirTarihce(string evrEvrakNo, string islemYapanTC, string islemYapanAd, string eskiKod, string yeniKod, TNSDateTime tarih1, TNSDateTime tarih2, string aciklama)
        {
            string sql = "INSERT INTO " + TABEvrakTarihce.TABLOAD + " (" + VeriErisim.TumAlanlar("", TABEvrakTarihce.ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.DegerYap(islemYapanTC) + ",";

            if (eskiKod != "" && yeniKod != "")
                sql += VeriErisim.DegerYap("Dosya Tasnif Planı Değişti. Eski:" + eskiKod + "; Yeni:" + yeniKod + " :" + aciklama) + ",";
            else
                sql += VeriErisim.DegerYap("Dosya Tasnif Planı Değişti. " + aciklama) + ",";
            sql += VeriErisim.DegerYap(islemYapanAd) + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap("") + ")";

            return KomutCalistir(sql, 1);
        }
    }
}
