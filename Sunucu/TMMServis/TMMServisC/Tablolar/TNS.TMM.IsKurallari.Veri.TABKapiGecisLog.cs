using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır malzemelerin Kapı Geçiş Log bilgilerini tutan TASKAPIGECISLOG veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABKapiGecisLog : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKapiGecisLog(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASKAPIGECISLOG";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Programatik sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSICILNO = 0,
            /// <summary>
            /// Tarih alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 1,
            /// <summary>
            /// T.C. Kimlik numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 2
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "PRSICILNO", "TARIH", "TCKIMLIKNO" };

        /// <summary>
        /// Kapı Geçiş log bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kgl">Kapı Geçiş Log bilgilerini tutan, KapiGecisLog türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(KapiGecisLog kgl)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kgl.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(kgl.tarih) + ",";
            sql += VeriErisim.DegerYap(kgl.tcKimlikNo) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Kapı Geçiş Log ve Tarih bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Kapı Geçiş Log bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kgl">Kapı Geçiş Log bilgilerini tutan, KapiGecisLog türünde nesne</param>
        /// <param name="tarih1">Başlangıç Tarih bilgisini tutan, TNSDateTime türünde nesne</param>
        /// <param name="tarih2">Bitiş Tarih bilgisini tutan, TNSDateTime türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Kapı Geçiş Log bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(KapiGecisLog kgl, TNSDateTime tarih1, TNSDateTime tarih2)
        {
            string sql = "SELECT * FROM TASSICILNORFIDVIEW B";
            sql += " INNER JOIN " + TABLOAD + " A";
            sql += " ON A." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " = B.PRSICILNO";

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kgl.snh.muhasebeKod))
                kosul = VeriErisim.KosulOl("B", "MUHASEBEKOD", "=", kgl.snh.muhasebeKod);

            if (!string.IsNullOrEmpty(kgl.snh.harcamaBirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "HARCAMABIRIMKOD", "=", kgl.snh.harcamaBirimKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kgl.snh.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "AMBARKOD", "=", kgl.snh.ambarKod));

            if (!string.IsNullOrEmpty(kgl.snh.hesapPlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "HESAPPLANKOD", "LIKE", kgl.snh.hesapPlanKod.Replace(".", "") + "%"));

            if (!string.IsNullOrEmpty(kgl.snh.sicilNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "SICILNO", "=", kgl.snh.sicilNo));

            if (kgl.snh.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("B", "PRSICILNO", "=", kgl.snh.prSicilNo));

            if (!tarih1.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.TARIH], ">=", tarih1));

            if (!tarih2.isNull)
            {
                tarih2.AddDays(1);
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.TARIH], "<=", tarih2));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY B.SICILNO";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}