using System.Data;
using TNS;
using TNS.Veri;
using TNS.BUM.IsKurallari.Veri;



namespace TNS.BUM.IsKurallari.Veri
{

    /// <summary>
    /// Dayanak bilgilerini tutan ISLDAYANAKLAR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABISLDayanaklar : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLDayanaklar(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ISLDAYANAKLAR";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "DAYANAKKOD", "DAYANAKAD", "ACIKLAMA" };

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
            /// Dayanak kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKKOD = 1,
            /// <summary>
            /// Dayanak ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKAD = 2,
            /// <summary>
            /// Açıklama alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3
        }

        
        /// <summary>
        /// Parametre olarak verilen Yıl, Bütçe İşlem Kod ve Sıra bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Dayanak bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="butceIslemKod">Bütçe İşlem Kodu bilgisini tutan, Int türünde nesne</param>
        /// <param name="sira">Sıra bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Dayanak bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int yil, int butceIslemKod, int sira)
        {
            string sql = "select " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql += " order by " + sira;
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Dayanak Kodu bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Dayanak bilgilerini döndüren
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakKod">Dayanak kodu bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Dayanak bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Oku(int yil, int dayanakKod)
        {
            string sql = "select " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKKOD], "=", dayanakKod));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisini kullanarak,
        /// Listele rutinini çalıştırarak Dayanak bilgilerini döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, verilen kriterlere uygun Dayanak bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int yil)
        {
            return Listele(yil, -1, 3);
        }

        /// <summary>
        /// Verilen dayanak bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakKod">Dayanak kodu bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakAd">Dayanak adı bilgisini tutan, String türünde nesne</param>
        /// <param name="aciklama">Açıklama bilgisini tutan, String türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(int yil, int dayanakKod, string dayanakAd, string aciklama)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(dayanakKod) + ", ";
            sql += VeriErisim.DegerYap(dayanakAd) + ", ";
            sql += VeriErisim.DegerYap(aciklama) + ") ";

            Sonuc s = KomutCalistir(sql, 1);
            return s;

        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Dayanak kodu, Dayanak adı ve Açıklama bilgilerine göre
        /// Dayanak bilgilerini veri tabanından güncelleyen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakKod">Dayanak kodu bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakAd">Dayanak Adı bilgisini tutan, String türünde nesne</param>
        /// <param name="aciklama">Açıklama bilgisini tutan, String türünde nesne</param>
        /// <returns>Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Degistir(int yil, int dayanakKod, string dayanakAd, string aciklama)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DAYANAKAD], dayanakAd) + ", ";
            sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], aciklama) + " ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKKOD], "=", dayanakKod));

            return KomutCalistir(sql, 1);

        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Dayanak Kodu bilgisini kullanarak,
        /// İlgili Dayanak bilgilerini silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakKod">Dayanak kodu bilgisini tutan, Int türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(int yil, int dayanakKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DAYANAKKOD], "=", dayanakKod));

            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun En büyük dayanak kodu bilgisini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun En büyük dayanak kodu bilgisini tutan Int türünde nesne döndürür</returns>
        public int EnBuyukKodBul(int yil)
        {
            string sql = "select MAX(" + ALANLAR[(int)ENUMAlanlar.DAYANAKKOD] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            int tut = veriErisim.TekKolonSQLInt(sql);
            tut++;
            return tut;
        }

        /// <summary>
        /// Parametre olarak verilen Kaynak Yıla ilişkin Dayanak bilgilerini Hedef Yıla kopyalan,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kaynakYil">Kaynak yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="hedefYil">Hedef Yıl bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kopyala(int kaynakYil, int hedefYil)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") SELECT ";
            sql += hedefYil.ToString() + " as  " + ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.DAYANAKKOD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.DAYANAKAD] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.ACIKLAMA] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", kaynakYil));

            Sonuc s = KomutCalistir(sql, 0);
            return s;

        }
    }

} 
