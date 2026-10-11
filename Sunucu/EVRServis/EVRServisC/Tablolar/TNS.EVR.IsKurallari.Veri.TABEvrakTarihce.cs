using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak tarihçe bilgilerini içeren EVREVRAKTARIHCE veri tabanı tablosu ile ilgili, kaydetme ve listeleme yordamlarını tutan sınıf.
    /// </summary>
    public class TABEvrakTarihce : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABEvrakTarihce sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABEvrakTarihce(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Evrak tarihçe bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVREVRAKTARIHCE";

        /// <summary>
        /// Evrak tarihçe bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            EVREVRAKNO = 0,
            ISLEMTARIH = 1,
            ISLEMYAPAN = 2,
            ISLEM = 3,
            ISLEMYAPANAD = 4,
            VEKIL = 5,
            VEKILAD = 6
        }

        /// <summary>
        /// Evrak tarihçe bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "EVREVRAKNO", "ISLEMTARIH", "ISLEMYAPAN", "ISLEM", "ISLEMYAPANAD", "VEKIL", "VEKILAD" };

        /// <summary>
        /// Parametre olarak verilen evrak tarihçe bilgisine göre; evrak tarhiçe bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Evrak tarihçe bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(EvrakTarihce form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.islemTarih) + ",";
            sql += VeriErisim.DegerYap(form.islemYapan) + ",";
            sql += VeriErisim.DegerYap(form.islem) + ",";
            sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
            sql += VeriErisim.DegerYap(form.vekil) + ",";
            sql += VeriErisim.DegerYap(form.vekilAd) + ")";
            
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen evrak bilgisine göre; evrak tarhiçe bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="evrEvrakNo">Evrak evrEvrakNo bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(string evrEvrakNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", evrEvrakNo));
            sql += "ORDER BY ISLEMTARIH";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
