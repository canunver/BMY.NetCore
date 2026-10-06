using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// ABSGELIR, ABSGELIRTAHMINI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGelirTahmini : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGelirTahmini(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOADGELIR = "ABSGELIR";
        static string TABLOAD = "ABSGELIRTAHMINI";
        static string[] ALANLAR = { "YIL", "ASAMA", "KURKOD", "EKOKOD", "GELIR" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            ASAMA = 1,
            KURKOD = 2,
            EKOKOD = 3,
            GELIR = 4,
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, gelir tahmini rakamını bulan sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public double GelirTahminiDegerBul(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, ay, ekKosul,kodlar, true);

            string sql = "select SUM(" + ALANLAR[(int)ENUMAlanlar.GELIR] + ")" +
                " from " + TABLOAD +
                " where " + kosul;

            double deger = veriErisim.TekKolonSQLDbl(sql);
            if (yil < 2005)
                deger = deger / 1000;

            return deger;

        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, gelir rakamını bulan sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public double GelirDegerBul(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, ay, ekKosul, kodlar, true);

            string sql = "select SUM(" + ALANLAR[(int)ENUMAlanlar.GELIR] + ")" +
                " from " + TABLOADGELIR +
                " where " + kosul;

            double deger = veriErisim.TekKolonSQLDbl(sql);
            if (yil < 2005)
                deger = deger / 1000;

            return deger;

        }
    }
}
