using System.Data;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// KHDEVIR veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKHDevir : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKHDevir(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "KHDEVIR";
        static string[] ALANLAR = { "YIL", "KURUM", "ISLEM", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEK" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            KURUM = 1,
            ISLEM = 2,
            KURKOD = 3,
            FONKOD = 4,
            FINKOD = 5,
            EKOKOD = 6,
            ODENEK = 7,
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="islem">İşlem kodu.</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="odenek">Ödenek rakamı</param>
        /// <returns></returns>
        public Sonuc Kaydet(int yil, string kurum, string islem, string[] kodlar, double odenek)
        {
            Sonuc sonuc = new Sonuc();
            if (odenek > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";

                sql += VeriErisim.DegerYap(yil) + ", ";
                sql += VeriErisim.DegerYap(kurum) + ", ";
                sql += VeriErisim.DegerYap(islem) + ", ";
                sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(((string)kodlar[2]).Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(((string)kodlar[3]).Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(odenek) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;

        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="islem">İşlem kodu.</param>
        /// <returns></returns>
        public Sonuc Sil(int yil, string kurum, string islem)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurum));
            if (islem != "")
                kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", islem));
            //kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kodlar[0].ToString().Replace(".", "")));
            //kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", kodlar[1].ToString().Replace(".", "")));
            //kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", kodlar[2].ToString().Replace(".", "")));
            //kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", kodlar[3].ToString().Replace(".", "")));

            sql = "delete from " + TABLOAD;
            sql = sql + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen işlem koduna uygun listeyi bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="islemKod">İşlem Kodu.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public double RakamGetir(int yil, string islemKod, string[] kodlar)
        {
            string sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")" + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], "=", islemKod.ToString()));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen işlem koduna uygun listeyi bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="islemler">İşlem kodları</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public double RakamGetir(int yil, string[] islemler, string[] kodlar)
        {
            string kosul = "";
            string sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")" + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));

            for (int i = 0; i < islemler.Length; i++)
            {
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", islemler[i].Replace(".", "") + "%"));
            }
            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="islemler">İşlem kodları.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader Listele(int yil, string kurum, string[] islemler, string[] kodlar)
        {
            string kosul = "";

            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurum));

            for (int i = 0; i < islemler.Length; i++)
            {
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", islemler[i].Replace(".", "") + "%"));
            }
            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">Kurum.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader Listele(int yil, string kurum, string[] kodlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurum));

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, tertip listesini bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="ekKosul">Eko koşullar.</param>
        /// <returns></returns>
        public IDataReader TertipListele(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOl(yil, -1, ekKosul, kodlar, false, "");

            string sql = "select DISTINCT " +
                ALANLAR[(int)ENUMAlanlar.KURKOD] +
                "," + ALANLAR[(int)ENUMAlanlar.FONKOD] +
                "," + ALANLAR[(int)ENUMAlanlar.FINKOD] +
                "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] +
                " from " + TABLOAD + " where " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Devir işlemleri listesini bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="islemKod">İşlem kodu.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekkosul">Ek koşullar.</param>
        /// <returns></returns>
        public IDataReader DevirIslemleriBul(int yil, string islemKod, string[] kodlar, string ekkosul)
        {

            string kosul = "";
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + "," + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + "), SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ") FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));
            sql = VeriErisim.KosulEkle(sql, "and", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", islemKod.Replace(".", "") + "%"));
            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            sql += " group by " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + "," + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")";

            IDataReader dr = veriErisim.DataReaderDondur(sql);
            return dr;
        }
    }
}
