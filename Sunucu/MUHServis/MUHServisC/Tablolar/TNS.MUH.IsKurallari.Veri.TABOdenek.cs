using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// ABSODENEK veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOdenek : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOdenek(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ABSODENEK";
        enum ENUMAlanlar
        {
            YIL = 0,
            ASAMA = 1,
            KURKOD = 2,
            FONKOD = 3,
            FINKOD = 4,
            EKOKOD = 5,
            ODENEK = 6,
        }
        static string[] ALANLAR = { "YIL", "ASAMA", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEK" };

        static string TABLOADBUTCE = "ABSBUTCEGIDER";
        enum ENUMAlanlarButce
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
            SERBEST_TASLAK = 12,
        }
        static string[] ALANLARBUTCE = { "YIL", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "KANUN", "KBO", "EKLENEN", "DUSULEN", "SERBEST", "EKLENEN_TASLAK", "DUSULEN_TASLAK", "SERBEST_TASLAK" };

        static string TABLOADGIDEREBUTCE = "ISLGIDEREBUTCE";
        enum ENUMAlanlarGiderEButce : int
        {
            YIL = 0,
            KURUMKOD = 1,
            ISLEM = 2,
            AY = 3,
            SIRANO = 4,
            KURKOD = 5,
            FONKOD = 6,
            FINKOD = 7,
            EKOKOD = 8,
            ODENEK = 9,
            FORMDURUM = 10,
            YILDIZYER = 11
        }
        static string[] ALANLARGIDEREBUTCE = { "YIL", "KURUMKOD", "ISLEM", "AY", "SIRANO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEK", "FORMDURUM", "YILDIZYER" };

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="asama">Aşama.</param>
        /// <returns></returns>
        public Sonuc ButceHazirlikOdenekSil(int yil, int asama)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ASAMA], "=", asama));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="asama">Aşama.</param>
        /// <param name="kurKod">Kurumsal kod.</param>
        /// <param name="fonKod">Fonksiyonel kod.</param>
        /// <param name="finKod">Finans kod.</param>
        /// <param name="ekoKod">Ekonomik kod.</param>
        /// <param name="odenek">Ödenek.</param>
        /// <returns></returns>
        public Sonuc ButceHazirlikOdenekKaydet(int yil, int asama, string kurKod, string fonKod, string finKod, string ekoKod, double odenek)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(asama) + ", ";
            sql = sql + VeriErisim.DegerYap(kurKod) + ", ";
            sql = sql + VeriErisim.DegerYap(fonKod) + ", ";
            sql = sql + VeriErisim.DegerYap(finKod) + ", ";
            sql = sql + VeriErisim.DegerYap(ekoKod) + ", ";
            sql = sql + VeriErisim.DegerYap(odenek) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodTuru">Bütçe kod türü.</param>
        /// <param name="ustKod">Üst kod.</param>
        /// <returns></returns>
        public Sonuc ButceHazirlikOdenekSilKod(int yil, string kodTuru, string ustKod)
        {
            string sql = "";
            string tablo = "";
            string kodAlan = "";

            switch (kodTuru)
            {
                case "1": tablo = "ABSKUR"; kodAlan = "KURKOD"; break;
                case "2": tablo = "ABSFON"; kodAlan = "FONKOD"; break;
                case "3": tablo = "ABSFIN"; kodAlan = "FINKOD"; break;
                case "4": tablo = "ABSEKO"; kodAlan = "EKOKOD"; break;
                case "5": tablo = "ABSGELIRKOD"; kodAlan = "EKOKOD"; break;
                case "6": tablo = "ABSFINANSMANKOD"; kodAlan = "FINKOD"; break;
            }
            sql = "DELETE FROM " + tablo;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            if (ustKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", kodAlan, "LIKE", ustKod + "%"));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kodTuru">Bütçe kod türü.</param>
        /// <param name="kod">Kod.</param>
        /// <param name="ad">Adı.</param>
        /// <returns></returns>
        public Sonuc ButceHazirlikOdenekKaydetKod(string yil, string kodTuru, string kod, string ad)
        {
            string sql = "";
            string tablo = "";

            switch (kodTuru)
            {
                case "1": tablo = "ABSKUR"; break;
                case "2": tablo = "ABSFON"; break;
                case "3": tablo = "ABSFIN"; break;
                case "4": tablo = "ABSEKO"; break;
                case "5": tablo = "ABSGELIRKOD"; break;
                case "6": tablo = "ABSFINANSMANKOD"; break;
            }
            sql = "INSERT INTO " + tablo + " VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kod) + ", ";
            sql = sql + VeriErisim.DegerYap(ad) + ",";
            sql = sql + VeriErisim.DegerYap("") + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <returns></returns>
        public Sonuc ButceSil(int yil)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADBUTCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBUTCE[(int)ENUMAlanlarButce.YIL], "=", yil));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurKod">Kurumsal kod.</param>
        /// <param name="fonKod">Fonksiyonel kod.</param>
        /// <param name="finKod">Finans kod.</param>
        /// <param name="ekoKod">Ekonomik kod.</param>
        /// <param name="kanun">Kanun rakamı.</param>
        /// <param name="kbo">Kesintili başlangıç ödeneği.</param>
        /// <param name="eklenen">Yıl içinde Eklenen.</param>
        /// <param name="dusulen">Yıl içinde Düşülen.</param>
        /// <param name="serbest">Serbest bırakılan.</param>
        /// <returns></returns>
        public Sonuc ButceKaydet(int yil, string kurKod, string fonKod, string finKod, string ekoKod, double kanun, double kbo, double eklenen, double dusulen, double serbest)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADBUTCE + " (" + VeriErisim.TumAlanlar("", ALANLARBUTCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurKod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fonKod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(finKod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(ekoKod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(kanun) + ", ";
            sql = sql + VeriErisim.DegerYap(kbo) + ", ";
            sql = sql + VeriErisim.DegerYap(eklenen) + ", ";
            sql = sql + VeriErisim.DegerYap(dusulen) + ", ";
            sql = sql + VeriErisim.DegerYap(serbest) + ", ";
            sql = sql + VeriErisim.DegerYap(0) + ", ";
            sql = sql + VeriErisim.DegerYap(0) + ", ";
            sql = sql + VeriErisim.DegerYap(0) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="kurum">The kurum.</param>
        /// <param name="islem">The islem.</param>
        /// <param name="siraNo">The sira no.</param>
        /// <param name="kur">The kur.</param>
        /// <param name="fon">The fon.</param>
        /// <param name="fin">The fin.</param>
        /// <param name="eko">The eko.</param>
        /// <param name="peb">The peb.</param>
        /// <param name="odenek">The odenek.</param>
        /// <param name="durum">The durum.</param>
        /// <param name="ay">The ay.</param>
        /// <returns></returns>
        public Sonuc AHPKaydet(int yil, string kurum, string islem, int siraNo, string kur, string fon, string fin, string eko, string peb, double odenek, int durum, int ay)
        {
            string sql = "";

            sql = "INSERT INTO ISLAPGIDER (YIL,KURUMKOD,ISLEM,SIRANO,KURKOD,FONKOD,FINKOD,EKOKOD,PEBKOD,FORMDURUM,AY,ODENEK) VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurum) + ", ";
            sql = sql + VeriErisim.DegerYap(islem) + ", ";
            sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
            sql = sql + VeriErisim.DegerYap(kur.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fon.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fin.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(eko.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(peb.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(durum) + ", ";
            sql = sql + VeriErisim.DegerYap(ay) + ", ";
            sql = sql + VeriErisim.DegerYap(odenek) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="kurum">The kurum.</param>
        /// <param name="siraNo">The sira no.</param>
        /// <returns></returns>
        public Sonuc AHPSil(int yil, string kurum, int siraNo)
        {
            string sql = "";

            sql = "DELETE FROM ISLAPGIDER";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURUMKOD", "=", kurum));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SIRANO", "=", siraNo));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanında değişiklik yapan sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="kurum">The kurum.</param>
        /// <returns></returns>
        public Sonuc AHPOnaylariKaldir(int yil, string kurum)
        {
            string sql = "";

            sql = "UPDATE ISLAPGIDER SET FORMDURUM=7";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURUMKOD", "=", kurum));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "FORMDURUM", "=", 5));

            return KomutCalistir(sql, 0);
        }
        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <param name="kurum">The kurum.</param>
        /// <param name="islem">The islem.</param>
        /// <param name="ay">The ay.</param>
        /// <param name="siraNo">The sira no.</param>
        /// <param name="kur">The kur.</param>
        /// <param name="fon">The fon.</param>
        /// <param name="fin">The fin.</param>
        /// <param name="eko">The eko.</param>
        /// <param name="peb">The peb.</param>
        /// <param name="odenek">The odenek.</param>
        /// <param name="durum">The durum.</param>
        /// <param name="yildizYer">The yildiz yer.</param>
        /// <returns></returns>
        public Sonuc ButceIslemKaydet(int yil, string kurum, string islem, int ay, int siraNo, string kur, string fon, string fin, string eko, string peb, double odenek, int durum, int yildizYer)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADGIDEREBUTCE + " (" + VeriErisim.TumAlanlar("", ALANLARGIDEREBUTCE) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurum.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(islem.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(ay) + ", ";
            sql = sql + VeriErisim.DegerYap(siraNo) + ", ";
            sql = sql + VeriErisim.DegerYap(kur.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fon.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(fin.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(eko.Replace(".", "")) + ", ";
            //sql = sql + VeriErisim.DegerYap(peb.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(odenek) + ", ";
            sql = sql + VeriErisim.DegerYap(durum) + ", ";
            sql = sql + VeriErisim.DegerYap(yildizYer) + ")";

            return KomutCalistir(sql, 1);
        }
        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">The yil.</param>
        /// <returns></returns>
        public Sonuc ButceIslemSil(int yil)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADGIDEREBUTCE;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", yil));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "KURUMKOD", "=", kurum));

            return KomutCalistir(sql, 0);
        }
    }
}
