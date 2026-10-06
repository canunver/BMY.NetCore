using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// ABSHarcama, KHHarcamaMahsup veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHarcamaMahsup : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHarcamaMahsup(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ABSHarcama";
        static string TABLOADMAHSUP = "KHHarcamaMahsup";
        static string[] ALANLAR = { "YIL", "AY", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "MAHSUP" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            AY = 1,
            KURKOD = 2,
            FONKOD = 3,
            FINKOD = 4,
            EKOKOD = 5,
            MAHSUP = 6,
        }

        /// <summary>
        /// Verilen parametrelere göre harcama rakamını bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="ekKosul">Ek kosul.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="mahsupMu">Eğer <c>true</c> ise mahsup tablosundan sorgulama yapılır .</param>
        /// <returns></returns>
        public double HarcamaAylikOdenek(int yil, int ay, string ekKosul, string[] kodlar, bool mahsupMu)
        {
            string kulTablo = "";

            if (mahsupMu)
                kulTablo = TABLOADMAHSUP;
            else
                kulTablo = TABLOAD;

            string kosul = GenelKosul.KistasKosulOl(yil, -1, ekKosul, kodlar, false);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", ay));
            string sql = "SELECT SUM(MAHSUP) from " + kulTablo + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);
        }
    }
}
