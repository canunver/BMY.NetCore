using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır malzemelerin Kapı Geçiş Sicil bilgilerini tutan TASKAPIGECISSICIL veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABKapiGecisSicil : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKapiGecisSicil(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASKAPIGECISSICIL";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Programatik sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSICILNO = 0
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "PRSICILNO" };

        /// <summary>
        /// Kapı Geçiş Sicil bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="prSicilNo">Programatik Sicil No bilgilerini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(int prSicilNo)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(prSicilNo) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil No Hareket bilgisini kullanarak
        /// ilgili Kapı Geçiş Sicil bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="snh">Sicil No Hareket bilgisini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(SicilNoHareket snh)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql += " WHERE EXISTS (SELECT 1 FROM TASSICILNORFIDVIEW B";
            sql += " WHERE " + TABLOAD + ".PRSICILNO = B.PRSICILNO";

            if (!string.IsNullOrEmpty(snh.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "MUHASEBEKOD", "=", snh.muhasebeKod));

            if (!string.IsNullOrEmpty(snh.harcamaBirimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "HARCAMABIRIMKOD", "=", snh.harcamaBirimKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(snh.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "AMBARKOD", "=", snh.ambarKod));

            if (!string.IsNullOrEmpty(snh.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "HESAPPLANKOD", "LIKE", snh.hesapPlanKod.Replace(".", "") + "%"));

            if (snh.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "PRSICILNO", "=", snh.prSicilNo));

            return KomutCalistir(sql + ")", 0);
        }

        /// <summary>
        /// Parametre olarak verilen Sicil No Hareket bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Sicil bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="snh">Sicil No Hareket bilgilerini tutan, SicilNoHareket türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Taşınır Sicil bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SicilListele(SicilNoHareket snh)
        {
            string sql = "SELECT * FROM TASSICILNORFIDVIEW B";
            sql += " LEFT OUTER JOIN " + TABLOAD + " A";
            sql += " ON A." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " = B.PRSICILNO";

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(snh.muhasebeKod))
                kosul = VeriErisim.KosulOl("B", "MUHASEBEKOD", "=", snh.muhasebeKod);

            if (!string.IsNullOrEmpty(snh.harcamaBirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "HARCAMABIRIMKOD", "=", snh.harcamaBirimKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(snh.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "AMBARKOD", "=", snh.ambarKod));

            if (!string.IsNullOrEmpty(snh.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "HESAPPLANKOD", "LIKE", snh.hesapPlanKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(snh.sicilNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "SICILNO", "=", snh.sicilNo));

            if (snh.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "PRSICILNO", "=", snh.prSicilNo));

            if (snh.kapiGecis)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", "PRSICILNO", " > ", " 0" ));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY B.SICILNO";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}