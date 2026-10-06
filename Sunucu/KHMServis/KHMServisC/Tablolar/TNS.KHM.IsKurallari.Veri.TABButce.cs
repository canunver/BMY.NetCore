using System.Data;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// ABSBUTCEGIDER veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABButce : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABButce(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ABSBUTCEGIDER";
        static string[] ALANLAR = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "KANUN", "KBO", "EKLENEN", "DUSULEN", "SERBEST", "EKLENEN_TASLAK", "DUSULEN_TASLAK", "SERBEST_TASLAK" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            KANUN = 5,
            KBO = 6,
            EKLENEN = 7,
            DUSULEN = 8,
            SERBEST = 9,
            EKLENEN_TASLAK = 10,
            DUSULEN_TASLAK = 11,
            SERBEST_TASLAK = 12
        }

        /// <summary>
        /// Veri tabanından Tertip listesininin alınması için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="kbolu">Eğer <c>true</c> ise sadece KBÖ rakamı olan kayıtları alır</param>
        /// <returns></returns>
        public IDataReader ButceTertipListele(int yil, string[] kodlar, bool kbolu)
        {

            string kosul = TNS.KHM.IsKurallari.Veri.GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false);
            if (kbolu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", "KBO", ">", 0));
            string alanlar = ALANLAR[(int)ENUMAlanlar.KURKOD] + "," + ALANLAR[(int)ENUMAlanlar.FONKOD] + "," + ALANLAR[(int)ENUMAlanlar.FINKOD] + "," + "SUBSTRING" + "(" + ALANLAR[(int)ENUMAlanlar.EKOKOD]+ ", 1, 3)";
            string sql = "SELECT  " + alanlar + ", SUM(" + ALANLAR[(int)ENUMAlanlar.KANUN] + "), SUM(" + ALANLAR[(int)ENUMAlanlar.KBO] + ")";
            sql = sql + " FROM " + TABLOAD;
            sql = sql + " WHERE " + kosul;
            sql = sql + " GROUP BY " + alanlar;
            sql = sql + " ORDER BY " + alanlar;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Kesintili başlangıç ödeneği rakamını bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="ekKosul">Ek koşullar</param>
        /// <returns></returns>
        public double kboBul(int yil, string[] kodlar, string ekKosul)
        {
            return YSOTBul(yil, kodlar, true, ekKosul);
        }

        /// <summary>
        /// Yıl sonu ödenek toplamı rakamını bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="kboBul">Eğer <c>true</c> ise sadece KBÖ rakamı olan kayıtları alır</param>
        /// <param name="ekKosul">Ek koşullar</param>
        /// <returns></returns>
        public double YSOTBul(int yil, string[] kodlar, bool kboBul, string ekKosul)
        {

            string kosul = GenelKosul.KistasKosulOl(yil, -1, ekKosul, kodlar, false);
            string sql = "";
            if (kboBul)
                sql = "SELECT SUM(KANUN) from " + TABLOAD + " where " + kosul;
            else
                sql = "SELECT (SUM(KBO)+SUM(EKLENEN)-SUM(DUSULEN)) AS YSOT from " + TABLOAD + " where " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }
    }
}
