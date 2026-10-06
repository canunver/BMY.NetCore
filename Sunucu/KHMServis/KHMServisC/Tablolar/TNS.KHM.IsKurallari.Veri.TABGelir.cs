using System.Data;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// KHGELIRCETVELI, KHGELIRGERCEKLESEN, ABSGELIRKOD, KHGELIRKOD veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGelir : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGelir(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "KHGELIRCETVELI";
        static string[] ALANLAR = { "YIL", "AY", "KURKOD", "EKOKOD", "TAHMIN", "ARTIK", "YILITAHAKKUKU", "REDIADE", "NETTAHAKKUK", "NETTAHSILAT", "DEVIRARTIK" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            AY = 1,
            KURKOD = 2,
            EKOKOD = 3,
            TAHMIN = 4,
            ARTIK = 5,
            YILITAHAKKUKU = 6,
            REDIADE = 7,
            NETTAHAKKUK = 8,
            NETTAHSILAT = 9,
            DEVIRARTIK = 10,
        }

        static string TABLOADGERCEKLESEN = "KHGELIRGERCEKLESEN";
        static string[] ALANLARGERCEKLESEN = { "YIL", "AY", "HESKOD", "KURKOD", "EKOKOD", "BORC", "ALACAK" };
        enum ENUMAlanlarGerceklesen : int
        {
            YIL = 0,
            AY = 1,
            HESKOD = 2,
            KURKOD = 3,
            EKOKOD = 4,
            BORC = 5,
            ALACAK = 6,
        }

        static string TABLOADKOD = "ABSGELIRKOD";
        static string[] ALANLARKOD = { "YIL", "EKOKOD", "EKOAD", "EKOAD2" };
        enum ENUMAlanlarKod : int
        {
            YIL = 0,
            EKOKOD = 1,
            EKOAD = 2,
            EKOAD2 = 3
        }

        static string TABLOADEKOKOD = "KHGELIRKOD";
        static string[] ALANLAREKOKOD = { "YIL", "KURKOD", "EKOKOD" };
        enum ENUMAlanlarEkoKod : int
        {
            YIL = 0,
            KURKOD = 1,
            EKOKOD = 2,
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, ekonomik ad listesini bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl</param>
        /// <param name="kod">Ekonomik kod</param>
        /// <returns></returns>
        public string AciklamaBul(int yil, string kod)
        {
            string donusDeger = "";
            string sql = "SELECT " + ALANLARKOD[(int)ENUMAlanlarKod.EKOAD];
            sql = sql + " FROM " + TABLOADKOD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKOD[(int)ENUMAlanlarKod.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKOD[(int)ENUMAlanlarKod.EKOKOD], "=", kod));
            donusDeger = veriErisim.TekKolonSQLStr(sql);
            return donusDeger;
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="tahmin">Tahmin rakamı.</param>
        /// <param name="artik">Artık rakamı.</param>
        /// <param name="yilitahakkuk">Yılı tahakkuk rakamı.</param>
        /// <param name="rediade">Red iade rakamı.</param>
        /// <param name="nettahakkuk">Net tahakkuk rakamı.</param>
        /// <param name="nettahsilat">Net tahsilat rakamı.</param>
        /// <param name="devir">Devir rakamı.</param>
        /// <returns></returns>
        public Sonuc GelirBilgileriKaydet(int yil, int ay, string[] kodlar, double tahmin, double artik, double yilitahakkuk, double rediade, double nettahakkuk, double nettahsilat, double devir)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc(true, "", "");
            if (tahmin > 0 || artik > 0 || yilitahakkuk > 0 || rediade > 0 || nettahakkuk > 0 || devir > 0)
            {
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";

                sql += VeriErisim.DegerYap(yil) + ", ";
                sql += VeriErisim.DegerYap(ay) + ", ";
                sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(tahmin) + ", ";
                sql += VeriErisim.DegerYap(artik) + ", ";
                sql += VeriErisim.DegerYap(yilitahakkuk) + ", ";
                sql += VeriErisim.DegerYap(rediade) + ", ";
                sql += VeriErisim.DegerYap(nettahakkuk) + ", ";
                sql += VeriErisim.DegerYap(nettahsilat) + ", ";
                sql += VeriErisim.DegerYap(devir) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;

        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kurum">Kurum.</param>
        /// <returns></returns>
        public Sonuc GelirBilgileriSil(int yil, int ay, string kurum)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", ay));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurum + "%"));

            sql = "delete from " + TABLOAD;
            sql = sql + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, gelir rakamlarını listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public IDataReader GelirDegerBul(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOl(yil, ay, ekKosul, kodlar, false, "");

            string sql = "select SUM(" + ALANLAR[(int)ENUMAlanlar.TAHMIN] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.ARTIK] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.YILITAHAKKUKU] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.REDIADE] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.NETTAHAKKUK] + ")" +
                  ",SUM(" + ALANLAR[(int)ENUMAlanlar.NETTAHSILAT] + ")" +
                 ",SUM(" + ALANLAR[(int)ENUMAlanlar.DEVIRARTIK] + ")" +

                " from " + TABLOAD +
                " where " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, tertip listesini listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public IDataReader TertipListele(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, ay, ekKosul, kodlar);

            string sql = "select DISTINCT " +
                ALANLAR[(int)ENUMAlanlar.KURKOD] +
                "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] +
                " from " + TABLOAD + " where " + kosul;
            sql = sql + " order by 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, kurumsal ve ekonomik kodları listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public IDataReader TertipListeleGerceklesme(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, ay, ekKosul, kodlar);

            string sql = "select DISTINCT " +
                ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.KURKOD] +
                "," + ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.EKOKOD] +
                " from " + TABLOADGERCEKLESEN + " where " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, gerçekleştirme tablosundaki ekonomik kodları listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public IDataReader KodListeleGerceklesme(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, ay, ekKosul, kodlar);

            string sql = "select DISTINCT " +
                 ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.EKOKOD] +
                " from " + TABLOADGERCEKLESEN + " where " + kosul;
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, hesap kodu bazında gerçekleşme rakamlarını listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public IDataReader KHGelirGerceklesmesiListeDegerBul(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, ay, ekKosul, kodlar);


            string sql = "select " + ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.HESKOD] + ",SUM(" + ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.BORC] + "-" + ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.ALACAK] + ")" +
                " from " + TABLOADGERCEKLESEN +
                " where " + kosul;
            sql = sql + " GROUP BY " + ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.HESKOD];
            sql = sql + " order by 1,2";
            return veriErisim.DataReaderDondur(sql);

        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, gerçekleşme rakamlarını listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <returns></returns>
        public double KHGelirGerceklesmesiDegerBul(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, ay, ekKosul, kodlar);


            string sql = "select SUM(" + ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.BORC] + "-" + ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.ALACAK] + ")" +
                " from " + TABLOADGERCEKLESEN +
                " where " + kosul;

            double deger = veriErisim.TekKolonSQLDbl(sql);
            if (yil < 2005)
                deger = deger / 1000;

            return deger;

        }

        /// <summary>
        /// Parametre olarak gelen kırılım sayısına göre ekonomik kodları listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşullar.</param>
        /// <param name="seviye">Seviye.</param>
        /// <returns></returns>
        public IDataReader EkoKodListeleSeviye(int yil, string[] kodlar, string ekKosul, int seviye)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, -1, ekKosul, kodlar);

            string sql = "select DISTINCT left(" +
                 ALANLAREKOKOD[(int)ENUMAlanlarEkoKod.EKOKOD] + "," + seviye + ")" +
                " from " + TABLOADEKOKOD + " where " + kosul;
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen kriterlere göre ekonomik kodları listeleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <returns></returns>
        public IDataReader EkoKodListele(int yil, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOlGelir(yil, -1, ekKosul, kodlar);

            string sql = "select DISTINCT " +
                 ALANLAREKOKOD[(int)ENUMAlanlarEkoKod.EKOKOD] +
                " from " + TABLOADEKOKOD + " where " + kosul;
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
