// Static Model
using System.Data;
using TNS.Veri;

namespace TNS.BUM.IsKurallari.Veri
{
    /// <summary>
    /// AHP Bloke bilgilerini tutan ISLAPBloke veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAHPBloke : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAHPBloke(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ISLAPBloke";

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "AHPUST" };

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
            /// Kurumsal Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,
            /// <summary>
            /// Fonksiyonel Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FONKOD = 2,
            /// <summary>
            /// Finansal kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            FINKOD = 3,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 4,
            /// <summary>
            /// Ekonomik kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 5,
            /// <summary>
            /// Ayrıntılı Harcama Programı üst alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AHPUst = 6
        }
        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Bloke bilgilerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kriterlere uygun Bloke bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BlokeListele(int yil, string kurum)
        {
            string[] kistasKodlar ={ kurum, "", "", "" };

            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kistasKodlar, false);
            string sql = "SELECT * from " + TABLOAD + " where " + kosul + " Order By 2,3,4,5";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Yıl, Kurum, Fonksiyonel, Finansal ve Ekonomik kodları kıstas olarak kullanarak,
        /// kıstasa uygun Üst Limit değerini döndüren,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurkod">Kurumsal Kod bilgisini tutan, String türünde nesne</param>
        /// <param name="fonkod">Fonksiyonel kod bilgisini tutan, String türünde nesne</param>
        /// <param name="finkod">Finansal kod bilgisini tutan, String türünde nesne</param>
        /// <param name="ekokod">Ekonomik kod bilgisini tutan, String türünde nesne</param>
        /// <param name="pebkod">Performans kod bilgisini tutan, String türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, verilen kritere uygun üst limit bilgisini tutan Double türünde nesne döndürür</returns>
        public double UstLimitBul(int yil, string kurkod, string fonkod, string finkod, string ekokod, string pebkod)
        {
            string kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurkod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "=", fonkod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "=", finkod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", ekokod.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", pebkod.Replace(".", "")));

            string sql = "SELECT AHPUST from " + TABLOAD + " where " + kosul;

            IDataReader dr = veriErisim.DataReaderDondur(sql);
            double donusDeger = -1;
            if (dr != null)
            {
                if (dr.Read())
                    donusDeger = VeriErisim.DegerAl(dr, 0, 0.0);
                dr.Close();
            }

            return donusDeger;
        }

        /// <summary>
        /// Verilen bloke bilgilerini veri tabanına kaydeden,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kodlar">Kurum, Fonksiyonel, Finansal ve Ekonomik kodları tutan, String dizi türünde nesne</param>
        /// <param name="AHPUst">Üst limit değer bilgisini tutan, Double türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BlokeKaydet(int yil, string[] kodlar, double AHPUst)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";

            sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
            VeriErisim.DegerYap(yil) + ", " +
            VeriErisim.DegerYap(kodlar[0].Replace(".", "")) + ", " +
            VeriErisim.DegerYap(kodlar[1].Replace(".", "")) + ", " +
            VeriErisim.DegerYap(kodlar[2].Replace(".", "")) + ", " +
            VeriErisim.DegerYap(kodlar[3].Replace(".", "")) + ", " +
            VeriErisim.DegerYap(AHPUst) + ")";

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Yıl ve Kurum bilgisini kullanarak,
        /// ilgili Bloke bilgilerini veri tabanından silen,
        /// sql komutunu oluşturup, çalıştıran ve sonucunu dönen fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan, Int türünde nesne</param>
        /// <param name="kurum">Kurum bilgisini tutan, String türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc BlokeSil(int yil, string kurum)
        {
            string[] kistasKodlar = { kurum, "", "", "" };

            string kosul = GenelKosul.KistasKosulOl(yil, -1, "", kistasKodlar, false);

            string sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
            return KomutCalistir(sql, 0);
        }
    }
} 
