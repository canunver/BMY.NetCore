using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// ABSHarcama veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHarcama : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHarcama(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ISLHarcama";
        static string[] ALANLAR = { "YIL", "AY", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "HARCAMA" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            AY = 1,
            KURKOD = 2,
            FONKOD = 3,
            FINKOD = 4,
            EKOKOD = 5,
            HARCAMA = 6,
        }

        static string muhasebatTur = System.Configuration.ConfigurationManager.AppSettings.Get("MuhasebatTur") + "";

        /// <summary>
        /// Verilen parametrelere göre harcama rakamını bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="ekKosul">Ek kosul.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="mahsupMu">Bu parametre şu an kullanılmıyor.</param>
        /// <returns></returns>
        public double HarcamaAylikOdenek(int yil, int ay, string ekKosul, string[] kodlar, bool mahsupMu)
        {
            string kosul = GenelKosul.KistasKosulOl(yil, -1, ekKosul, kodlar, false);
            string esit;
            if (!string.IsNullOrEmpty(muhasebatTur))
                esit = "<=";
            else
                esit = "=";

            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], esit, ay));
            string sql = "SELECT SUM(HARCAMA) from " + TABLOAD + " where " + kosul;
            return veriErisim.TekKolonSQLDbl(sql);
        }
    }
}
