using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır malzemelerin kapıdan geçişleri sırasında haber verilecek kişilere ait iletişim bilgilerinin tutulduğu TASKAPIGECISSMS veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABKapiGecisSms : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKapiGecisSms(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASKAPIGECISSMS";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// T.C. Kimlik numarası alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 0,
            /// <summary>
            /// Cep telefonu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            CEPTEL = 1,
            /// <summary>
            /// Sms At alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SMSAT = 2,
            /// <summary>
            /// E-Posta adresi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EPOSTA = 3,
            /// <summary>
            /// E-Posta At alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EPOSTAAT = 4
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "TCKIMLIKNO", "CEPTEL", "SMSAT", "EPOSTA", "EPOSTAAT" };

        /// <summary>
        /// Taşınır malzemelerin kapı geçişleri sırasında haber verilecek personele ait iletişim bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="personel">Haber verilecek Personel bilgisinin tutulduğu, Personel türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(Personel personel)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(personel.kod) + ",";
            sql += VeriErisim.DegerYap(personel.cepTel) + ",";

            if (personel.smsAt)
                sql += VeriErisim.DegerYap(1) + ",";
            else
                sql += VeriErisim.DegerYap(0) + ",";

            sql += VeriErisim.DegerYap(personel.eposta) + ",";

            if (personel.epostaAt)
                sql += VeriErisim.DegerYap(1) + ")";
            else
                sql += VeriErisim.DegerYap(0) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Personel bilgisini kullanarak
        /// Haber verilecek Personelin iletişim bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="personel">Haber verilecek Personelin bilgilerinin tutulduğu, Personel türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(Personel personel)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", personel.kod));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Personel bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır malzeme geçişi sırasında haber verilmek için kaydedilen iletişim bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="personel">Haber verilecek Personelin bilgilerinin tutulduğu, Personel türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Taşınır Malzeme geçisi sırasında haber verilmek için kaydedilen iletişim bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(Personel personel)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrEmpty(personel.kod))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], "=", personel.kod));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}