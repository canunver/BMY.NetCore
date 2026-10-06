using System.Data;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// KHGECICIMIZAN, KHKESINMIZAN veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABMizan : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABMizan(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOADGECICI = "KHGECICIMIZAN";
        static string TABLOADKESIN = "KHKESINMIZAN";
        static string[] ALANLAR = { "YIL", "KURUM", "HESKOD", "BORC", "ALACAK" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            KURUMKOD = 1,
            HESKOD = 2,
            BORC = 3,
            ALACAK = 4,
            ACIKLAMA = 5,
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, mizan borç,alacak rakamını bulan sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurKod">Kurumsal kod.</param>
        /// <param name="hesKod">Hesap kod.</param>
        /// <param name="gecici">Eğer <c>true</c> ise Geçici Mizan tablosundan sorgulama yapar.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <returns></returns>
        public IDataReader RakamGetir(int yil, string kurKod, string hesKod, bool gecici, string ekKosul)
        {
            string tabloAd = "";
            if (gecici)
                tabloAd = TABLOADGECICI;
            else
                tabloAd = TABLOADKESIN;

            string sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.BORC] + "),SUM(" + ALANLAR[(int)ENUMAlanlar.ALACAK] + ")" +
                " FROM " + tabloAd;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, " AND ", GenelKosul.KurumKosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurKod));
            if (hesKod != "")
                sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HESKOD], "LIKE", hesKod + "%"));
            if (ekKosul != "")
                sql = sql = VeriErisim.KosulEkle(sql, " AND ", ekKosul);
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, hesap kodu bazında mizan borç,alacak rakamlarını bulan sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurKod">Kurumsal kod.</param>
        /// <param name="gecici">Eğer <c>true</c> ise Geçici Mizan tablosundan sorgulama yapar.</param>
        /// <returns></returns>
        public IDataReader MizanListele(int yil, string kurKod, bool gecici)
        {
            string tabloAd = "";
            if (gecici)
                tabloAd = TABLOADGECICI;
            else
                tabloAd = TABLOADKESIN;


            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.HESKOD] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.BORC] + "),SUM(" + ALANLAR[(int)ENUMAlanlar.ALACAK] +
              ") FROM " + tabloAd;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, " AND ", GenelKosul.KurumKosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMKOD], "=", kurKod));
            sql = sql + " group by " + ALANLAR[(int)ENUMAlanlar.HESKOD];
            sql = sql + " order by " + ALANLAR[(int)ENUMAlanlar.HESKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
