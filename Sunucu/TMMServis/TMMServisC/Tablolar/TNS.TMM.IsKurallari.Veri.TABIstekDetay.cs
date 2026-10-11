using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// İstek detay bilgilerini tutan TASISTEKDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABIstekDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABIstekDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASISTEKDETAY";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 1,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 2,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 3,
            /// <summary>
            /// Belge no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGENO = 4,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 5,
            /// <summary>
            /// İstenilen miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISTENILENMIKTAR = 6,
            /// <summary>
            /// Karşılanan miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KARSILANANMIKTAR = 7,
            KDVORAN = 8,
            BIRIMFIYAT = 9,
            ACIKLAMA = 10
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "BELGENO",
                                   "HESAPPLANKOD", "ISTENILENMIKTAR", "KARSILANANMIKTAR",
                                   "KDVORAN", "BIRIMFIYAT", "ACIKLAMA" };

        /// <summary>
        /// İstek Detay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="id">İstek Detay bilgilerini tutan, İstek Detay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(IstekDetay id)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(id.yil) + ",";
            sql += VeriErisim.DegerYap(id.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(id.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(id.ambarKod) + ",";
            sql += VeriErisim.DegerYap(id.belgeNo) + ",";
            sql += VeriErisim.DegerYap(id.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(id.istenilenMiktar) + ",";
            sql += VeriErisim.DegerYap(id.karsilananMiktar) + ",";
            sql += VeriErisim.DegerYap(id.kdvOran) + ",";
            sql += VeriErisim.DegerYap(id.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(id.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen İstek Detay bilgisini kullanarak
        /// ilgili İstek Detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="id">İstek Detay bilgilerini tutan, İstek Detay türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(IstekDetay id)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", id.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", id.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", id.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", id.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", id.belgeNo));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen İstek Detay bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun İstek Detay bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="id">İstek Detay bilgilerini tutan, İstek Detay türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İstek Detay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(IstekDetay id)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASOLCUBIRIM.KOD, TASOLCUBIRIM.AD";
            sql += " FROM " + TABLOAD + ", TASHESAPPLAN, TASOLCUBIRIM";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD] + "=TASHESAPPLAN.KOD";
            sql += " AND TASHESAPPLAN.OLCUBIRIMKOD=TASOLCUBIRIM.KOD";

            if (id.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", id.yil));

            if (!string.IsNullOrEmpty(id.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", id.muhasebeKod));

            if (!string.IsNullOrEmpty(id.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", id.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(id.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", id.ambarKod));

            if (!string.IsNullOrEmpty(id.belgeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", id.belgeNo));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMBARKOD];
            sql += ", " + "CAST(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BELGENO] + " AS INT)";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}