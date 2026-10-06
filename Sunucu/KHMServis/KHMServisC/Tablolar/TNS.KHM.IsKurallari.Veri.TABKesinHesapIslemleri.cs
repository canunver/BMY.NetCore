using System.Data;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// KHYILICIIPTAL, KHGIDERGEREKCE, KHGELIRGEREKCE, KHITAAMIRLERI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABKHIslemler : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABKHIslemler(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "KHYILICIIPTAL";
        static string[] ALANLAR = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEK" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            ODENEK = 5,
        }

        static string TABLOADGIDERGEREKCE = "KHGIDERGEREKCE";
        static string[] ALANLARGIDERGEREKCE = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PARCA", "BILGI" };
        enum ENUMAlanlarGiderGerekce : int
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            PARCA = 5,
            BILGI = 6,
        }

        static string TABLOADGIDERGEREKCEDETAYONAY = "KHGIDERGEREKCEDETAYONAY";
        static string TABLOADGIDERGEREKCEDETAY = "KHGIDERGEREKCEDETAY";

        /*
         * 
         
CREATE TABLE KHGIDERGEREKCEDETAYONAY(
	YIL int NOT NULL,
	KURKOD varchar(20) NOT NULL,
	FONKOD varchar(20) NOT NULL,
	FINKOD varchar(20) NOT NULL,
	EKOKOD varchar(20) NOT NULL,
	PEBKOD varchar(20) NOT NULL,
	ONAY int NOT NULL,
PRIMARY KEY CLUSTERED ( YIL ASC, KURKOD ASC, FONKOD ASC, FINKOD ASC, EKOKOD ASC, PEBKOD ASC ) )  

         * */

        static string[] ALANLARGIDERGEREKCEDETAYONAY = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "ONAY" };
        static string[] ALANLARGIDERGEREKCEDETAY = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "PARCA", "BILGI" };
        enum ENUMAlanlarGiderGerekceDETAY : int
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            PEBKOD = 5,
            PARCA = 6,
            BILGI = 7,
        }

        static string TABLOADGELIRGEREKCE = "KHGELIRGEREKCE";
        static string[] ALANLARGELIRGEREKCE = { "YIL", "KURKOD", "EKOKOD", "PARCA", "BILGI" };
        enum ENUMAlanlarGelirGerekce : int
        {
            YIL = 0,
            KURKOD = 1,
            EKOKOD = 2,
            PARCA = 3,
            BILGI = 4,
        }

        static string TABLOADITAAMIRLERI = "KHITAAMIRLERI";
        static string[] ALANLARITAAMIRLERI = { "YIL", "KURKOD", "BIRIM", "DERECE", "ADSOYAD", "BASLAMATARIHI", "BITISTARIHI" };
        enum ENUMAlanlarItaAmirleri : int
        {
            YIL = 0,
            KURKOD = 1,
            BIRIM = 2,
            DERECE = 3,
            ADSOYAD = 4,
            BASLAMATARIHI = 5,
            BITISTARIHI = 6,
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="odenek">Ödenek.</param>
        /// <returns></returns>
        public Sonuc Kaydet(int yil, string[] kodlar, double odenek)
        {
            Sonuc sonuc = new Sonuc();
            if (odenek > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";

                sql += VeriErisim.DegerYap(yil) + ", ";
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
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public Sonuc Sil(int yil, string[] kodlar)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kodlar[0].ToString().Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", kodlar[1].ToString().Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", kodlar[2].ToString().Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, " AND ", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", kodlar[3].ToString().Replace(".", "")));

            sql = "delete from " + TABLOAD;
            sql = sql + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public double RakamGetir(int yil, string[] kodlar)
        {

            string sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")" + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));

            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader Listele(int yil, string[] kodlar)
        {

            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));


            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="parca">Parça numarası.</param>
        /// <param name="bilgi">Gerekçe bilgisi</param>
        /// <returns></returns>
        public Sonuc GiderGerekceKaydet(int yil, string[] kodlar, int parca, string bilgi)
        {
            string sql = "INSERT INTO " + TABLOADGIDERGEREKCE + " (" + VeriErisim.TumAlanlar("", ALANLARGIDERGEREKCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap((string)kodlar[0]) + ", ";
            sql = sql + VeriErisim.DegerYap((string)kodlar[1]) + ", ";
            sql = sql + VeriErisim.DegerYap((string)kodlar[2]) + ", ";
            sql = sql + VeriErisim.DegerYap((string)kodlar[3]) + ", ";
            sql = sql + VeriErisim.DegerYap(parca) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="parca">Parça numarası.</param>
        /// <param name="bilgi">Gerekçe bilgisi</param>
        /// <returns></returns>
        public Sonuc GiderGerekceDetayKaydet(int yil, string[] kodlar, int parca, string bilgi)
        {
            string sql = "INSERT INTO " + TABLOADGIDERGEREKCEDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARGIDERGEREKCEDETAY) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[0].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[1].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[2].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[3].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[4].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(parca) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak veilen bilgilere uygun verileri tablodan silen sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceSil(int yil, string[] kodlar)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGIDERGEREKCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOADGIDERGEREKCE));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak veilen bilgilere uygun verileri tablodan silen sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public Sonuc GiderGerekceDetaySil(int yil, string[] kodlar)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGIDERGEREKCEDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOADGIDERGEREKCEDETAY));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="parca">Parça numarası.</param>
        /// <param name="bilgi">Gerekçe bilgisi.</param>
        /// <returns></returns>
        public Sonuc GelirGerekceKaydet(int yil, string[] kodlar, int parca, string bilgi)
        {
            string sql = "INSERT INTO " + TABLOADGELIRGEREKCE + " (" + VeriErisim.TumAlanlar("", ALANLARGELIRGEREKCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap((string)kodlar[0]) + ", ";
            sql = sql + VeriErisim.DegerYap((string)kodlar[1]) + ", ";
            sql = sql + VeriErisim.DegerYap(parca) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak veilen bilgilere uygun verileri tablodan silen sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public Sonuc GelirGerekceSil(int yil, string[] kodlar)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGELIRGEREKCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.KURKOD], "=", (string)kodlar[0]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.EKOKOD], "=", (string)kodlar[1]));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader GiderGerekceGetir(int yil, string[] kodlar)
        {
            string sql = "";
            sql = "select " + ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.PARCA] + ", " + ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.BILGI];
            sql = sql + " FROM " + TABLOADGIDERGEREKCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.KURKOD], "=", (string)kodlar[0]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.FONKOD], "=", (string)kodlar[1]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.FINKOD], "=", (string)kodlar[2]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.EKOKOD], "=", (string)kodlar[3]));
            sql = sql + " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader GiderGerekceDetayGetir(int yil, string[] kodlar)
        {
            string sql = "";
            sql = "select " + ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.PARCA] + ", " + ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.BILGI];
            sql = sql + " FROM " + TABLOADGIDERGEREKCEDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.KURKOD], "=", kodlar[0].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FONKOD], "=", kodlar[1].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FINKOD], "=", kodlar[2].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.EKOKOD], "=", kodlar[3].Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.PEBKOD], "=", kodlar[4].Replace(".", "")));
            sql = sql + " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader GiderGerekceListele(int yil, string[] kodlar)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARGIDERGEREKCE);
            sql += " FROM " + TABLOADGIDERGEREKCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.YIL], "=", yil));
            if (!string.IsNullOrEmpty(kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.KURKOD], "LIKE", (string)kodlar[0] + "%"));
            if (!string.IsNullOrEmpty(kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.FONKOD], "LIKE", (string)kodlar[1] + "%"));
            if (!string.IsNullOrEmpty(kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.FINKOD], "LIKE", (string)kodlar[2] + "%"));
            if (!string.IsNullOrEmpty(kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCE[(int)ENUMAlanlarGiderGerekce.EKOKOD], "LIKE", (string)kodlar[3] + "%"));
            sql = sql + " ORDER BY 1,3,2,4,5,6";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader GiderGerekceListeleDetay(int yil, string[] kodlar, string pebKodlar = null)
        {
            string pebKod;
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARGIDERGEREKCEDETAY) + " FROM " + TABLOADGIDERGEREKCEDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.YIL], "=", yil));
            if (!string.IsNullOrEmpty(kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.KURKOD], "LIKE", kodlar[0].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FONKOD], "LIKE", kodlar[1].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FINKOD], "LIKE", kodlar[2].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.EKOKOD], "LIKE", kodlar[3].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(pebKod = TNS.OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, "")))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.PEBKOD], "LIKE", pebKod.Replace(".", "") + "%"));

            if(pebKodlar != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.OR_KosulOl(ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.PEBKOD], pebKodlar, true, true, 0));
            }

            sql = sql + " ORDER BY 1,2,3,4,5,6";
            return veriErisim.DataReaderDondur(sql);
        }

        public static string GiderGerekceDamgaSayBulDetaySql(int yil, string[] kodlar)
        {
            string pebKod;
            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, SUM(LEN(BILGI)) KARSAY FROM KHGIDERGEREKCEDETAY D";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.YIL], "=", yil));
            if (!string.IsNullOrEmpty(kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.KURKOD], "LIKE", kodlar[0].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FONKOD], "LIKE", kodlar[1].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FINKOD], "LIKE", kodlar[2].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.EKOKOD], "LIKE", kodlar[3].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(pebKod = TNS.OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, "")))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.PEBKOD], "LIKE", pebKod.Replace(".", "") + "%"));
            sql += " group by YIL, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD";
            sql += " order by 1, 2, 3, 4, 5";
            return sql;
        }

        public static string GiderGerekceOnayBulDetaySql(int yil, string[] kodlar)
        {
            string pebKod;
            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, ONAY KARSAY FROM KHGIDERGEREKCEDETAYONAY D";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.YIL], "=", yil));
            if (!string.IsNullOrEmpty(kodlar[0]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.KURKOD], "LIKE", kodlar[0].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[1]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FONKOD], "LIKE", kodlar[1].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[2]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.FINKOD], "LIKE", kodlar[2].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kodlar[3]))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.EKOKOD], "LIKE", kodlar[3].Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(pebKod = TNS.OrtakFonksiyonlar.DiziElemanAl(kodlar, 4, "")))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("D", ALANLARGIDERGEREKCEDETAY[(int)ENUMAlanlarGiderGerekceDETAY.PEBKOD], "LIKE", pebKod.Replace(".", "") + "%"));
            sql += " order by 1, 2, 3, 4, 5";
            return sql;
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <returns></returns>
        public IDataReader GelirGerekceGetir(int yil, string[] kodlar)
        {
            string sql = "";
            sql = "select " + ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.PARCA] + ", " + ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.BILGI];
            sql = sql + " FROM " + TABLOADGELIRGEREKCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.KURKOD], "=", (string)kodlar[0]));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGELIRGEREKCE[(int)ENUMAlanlarGelirGerekce.EKOKOD], "=", (string)kodlar[1]));
            sql = sql + " ORDER BY 1,2,3,4";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak veilen bilgilere uygun verileri tablodan silen sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurKod">Kurumsal kod.</param>
        /// <param name="derece">Derece.</param>
        /// <returns></returns>
        public Sonuc ItaAmirleriSil(int yil, string kurKod, int derece)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADITAAMIRLERI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARITAAMIRLERI[(int)ENUMAlanlarItaAmirleri.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARITAAMIRLERI[(int)ENUMAlanlarItaAmirleri.KURKOD], "=", kurKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARITAAMIRLERI[(int)ENUMAlanlarItaAmirleri.DERECE], "=", derece));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurKod">Kurumsal kod.</param>
        /// <param name="derece">Derece.</param>
        /// <returns></returns>
        public IDataReader ItaAmirleriListele(int yil, string kurKod, int derece)
        {
            string sql = "";
            sql = "select " + VeriErisim.TumAlanlar("", ALANLARITAAMIRLERI);
            sql = sql + " FROM " + TABLOADITAAMIRLERI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARITAAMIRLERI[(int)ENUMAlanlarItaAmirleri.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARITAAMIRLERI[(int)ENUMAlanlarItaAmirleri.KURKOD], "=", kurKod));
            if (derece > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARITAAMIRLERI[(int)ENUMAlanlarItaAmirleri.DERECE], "=", derece));
            sql = sql + " order by 1,2,3,4,6,5,7";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurKod">Kurumsal kod.</param>
        /// <param name="birimKod">Harcama birim kodu.</param>
        /// <param name="derece">Derece.</param>
        /// <param name="adSoyadi">Adı soyadı.</param>
        /// <param name="baslamaTarih">Başlama Tarihi.</param>
        /// <param name="bitisTarih">Bitiş Tarihi.</param>
        /// <returns></returns>
        public Sonuc ItaAmirleriKaydet(int yil, string kurKod, string birimKod, int derece, string adSoyadi, TNSDateTime baslamaTarih, TNSDateTime bitisTarih)
        {
            string sql = "INSERT INTO " + TABLOADITAAMIRLERI + " (" + VeriErisim.TumAlanlar("", ALANLARITAAMIRLERI) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurKod) + ", ";
            sql = sql + VeriErisim.DegerYap(birimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(derece) + ", ";
            sql = sql + VeriErisim.DegerYap(adSoyadi) + ", ";
            sql = sql + VeriErisim.DegerYap(baslamaTarih) + ", ";
            sql = sql + VeriErisim.DegerYap(bitisTarih) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc GiderGerekceDetayOnaySil(int yil, string[] kodlar)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGIDERGEREKCEDETAYONAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOADGIDERGEREKCEDETAYONAY));
            return KomutCalistir(sql, 0);
        }

        public Sonuc GiderGerekceDetayOnayla(int yil, string[] kodlar, int onayKod)
        {
            string sql = "INSERT INTO " + TABLOADGIDERGEREKCEDETAYONAY + " (" + VeriErisim.TumAlanlar("", ALANLARGIDERGEREKCEDETAYONAY) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[0].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[1].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[2].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[3].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kodlar[4].Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(onayKod) + ")";
            return KomutCalistir(sql, 1);
        }

        public int OnayKodBul(int yil, string[] kodlar)
        {
            string sql = "SELECT ONAY FROM " + TABLOADGIDERGEREKCEDETAYONAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOADGIDERGEREKCEDETAYONAY));
            return this.veriErisim.TekKolonSQLInt(sql);
        }
    }
}
