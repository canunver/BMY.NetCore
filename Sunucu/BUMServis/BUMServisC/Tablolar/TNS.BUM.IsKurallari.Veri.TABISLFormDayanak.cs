using System.Data;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{

    /// <summary>
    /// Form Dayanak bilgilerini tutan ISLFORMDAYANAK veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABISLFormDayanak : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABISLFormDayanak(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "ISLFORMDAYANAK";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURUMKOD", "ISLEM", "SIRANO", "DAYANAKKOD", "DAYANAKSIRA" };

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
            /// Kurum kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURUMKOD = 1,
            /// <summary>
            /// İşlem alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEM = 2,
            /// <summary>
            /// Sıra no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SIRANO = 3,
            /// <summary>
            /// Dayanak kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKKOD = 4,
            /// <summary>
            /// Dayanak kodu sıra alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DAYANAKKODSIRA = 5
        }


        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu, Sıra no bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Form Dayanak bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Form Dayanak bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DAYANAKKODSIRA] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DAYANAKKOD] + ", " + TABISLDayanaklar.TABLOAD + "." + TABISLDayanaklar.ALANLAR[(int)TABISLDayanaklar.ENUMAlanlar.DAYANAKAD] + " FROM " + TABLOAD + ", " + TABISLDayanaklar.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " + TABISLDayanaklar.TABLOAD + "." + TABISLDayanaklar.ALANLAR[(int)TABISLDayanaklar.ENUMAlanlar.YIL];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DAYANAKKOD] + " = " + TABISLDayanaklar.TABLOAD + "." + TABISLDayanaklar.ALANLAR[(int)TABISLDayanaklar.ENUMAlanlar.DAYANAKKOD];
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", siraNo));
            sql += " order by 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, İşlem Kodu ve Sıra no bilgilerini kullanarak,
        /// İlgili Form Dayanak bilgilerini silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(int yil, string kurum, string islemKod, int siraNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurum.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.IslemKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], islemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], " = ", siraNo));

            Sonuc s = KomutCalistir(sql, 0);

            return s;
        }

        /// <summary>
        /// Verilen Form Dayanak bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <param name="islemKod">İşlem kodu bilgisini tutan, String türünde nesne</param>
        /// <param name="siraNo">Sıra no bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakKod">Dayanak kodu bilgisini tutan, Int türünde nesne</param>
        /// <param name="dayanakSira">Dayanak sıra bilgisini tutan, Int türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(int yil, string kurum, string islemKod, int siraNo, int dayanakKod, int dayanakSira)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";
            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(kurum.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(islemKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(siraNo) + ", ";
            sql += VeriErisim.DegerYap(dayanakKod) + ", ";
            sql += VeriErisim.DegerYap(dayanakSira) + ") ";

            Sonuc s = KomutCalistir(sql, 1);
            return s;
        }

    }

}
