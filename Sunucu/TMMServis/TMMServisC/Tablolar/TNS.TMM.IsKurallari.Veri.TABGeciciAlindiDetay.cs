using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Geçici olarak alınan (Konsinye) Taşınır malzemelerin detay bilgisini tutan TASGECICIDETAY veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABGeciciAlindiDetay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGeciciAlindiDetay(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASGECICIDETAY";

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
            /// Fiş no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FISNO = 3,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 4,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 5,
            /// <summary>
            /// Miktar alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 6,
            /// <summary>
            /// Kdv oran alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KDVORAN = 7,
            /// <summary>
            /// Birim fiyat alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYAT = 8,
            /// <summary>
            /// Birim fiyat kdv alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BIRIMFIYATKDVLI = 9,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD", "FISNO", "SIRANO", "HESAPPLANKOD", "MIKTAR", "KDVORAN", "BIRIMFIYAT", "BIRIMFIYATKDVLI" };

        /// <summary>
        /// Konsinye olarak alınan Taşınır Malzeme ilişkin detay bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="td">Taşınır İşlem detay bilgisini tutan, TasinirIslemDetay türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(TasinirIslemDetay td)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(td.yil) + ",";
            sql += VeriErisim.DegerYap(td.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(td.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(td.fisNo) + ",";
            sql += VeriErisim.DegerYap(td.siraNo) + ",";
            sql += VeriErisim.DegerYap(td.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(td.miktar) + ",";
            sql += VeriErisim.DegerYap(td.kdvOran) + ",";
            sql += VeriErisim.DegerYap(td.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(td.birimFiyatKDVLi) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kullanarak
        /// ilgili Konsinye olarak alınan Taşınır Malzeme ilişkin detay bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgilerini tutan, TasinirIslemForm türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(TasinirIslemForm tf)
        {
            string sql = "DELETE FROM " + TABLOAD;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Taşınır İşlem Form bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Konsinye olarak alınan Taşınır Malzeme ilişkin detay bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="tf">Taşınır İşlem Form bilgisini tutan, TasinirIslemForm türünde nesne</param>
        /// <param name="siraNoSirali">Sıra numarasına göre sıralama işleminin yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Konsinye olarak alınan Taşınır Malzeme ilişkin detay bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(TasinirIslemForm tf, bool siraNoSirali)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            //Fişe ait bütün satırlar
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tf.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tf.harcamaKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FISNO], "=", tf.fisNo));

            if (siraNoSirali)
                sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO];
            else
                sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
