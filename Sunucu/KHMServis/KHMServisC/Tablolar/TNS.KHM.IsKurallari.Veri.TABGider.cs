using System.Data;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// KHGIDERCETVELI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABGider : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGider(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "KHGIDERCETVELI";
        static string[] ALANLAR = { "YIL", "AY", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "BO", "KBO", "KICIEK", "KICIDUS", "KARAEK", "KARADUS", "YEDEKEK", "YEDEKDUS", "ODENEKEKLE", "OZELODENEK", "YILICIIPTAL", "HARCAMA", "ODENEKUSTU", "YILSONUIPTAL", "DEVIR", "AKREDITIF", "AKREDITIFDEVIR", "TAAHHUTDEVIR", "DISPROJEDEVIR", "BAGISDEVIR", "OZELDEVIR", "DIGERDEVIR", "GONDERILEN" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            AY = 1,
            KURKOD = 2,
            FONKOD = 3,
            FINKOD = 4,
            EKOKOD = 5,
            BO = 6,
            KBO = 7,
            KICIEK = 8,
            KICIDUS = 9,
            KARAEK = 10,
            KARADUS = 11,
            YEDEKEK = 12,
            YEDEKDUS = 13,
            ODENEKEKLE = 14,
            OZELODENEK = 15,
            YILICIIPTAL = 16,
            HARCAMA = 17,
            ODENEKUSTU = 18,
            YILSONUIPTAL = 19,
            DEVIR = 20,
            AKREDITIF = 21,
            AKREDITIFDEVIR = 22,
            TAAHHUTDEVIR = 23,
            DISPROJEDEVIR = 24,
            BAGISDEVIR = 25,
            OZELDEVIR = 26,
            DIGERDEVIR = 27,
            GONDERILEN = 28,
        }

        /// <summary>
        /// Parametre olarak gelen alanları veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="bo">Başlangıç ödeneği</param>
        /// <param name="kbo">Kesintili Başlangıç ödeneği.</param>
        /// <param name="kiciEk">Kurum için eklenene.</param>
        /// <param name="kiciDus">Kurum içi düşülen.</param>
        /// <param name="karaEk">Kurumlar arası eklenen.</param>
        /// <param name="karaDus">Kurumlar arası düşülen.</param>
        /// <param name="yedekEk">Yedekten eklenen.</param>
        /// <param name="yedekDus">Yedek düşülen.</param>
        /// <param name="odenekEkle">Ödenek ekle.</param>
        /// <param name="ozelOdenek">Özel ödenek.</param>
        /// <param name="yiliciIptal">Yıl içinde iptal edilen.</param>
        /// <param name="harcama">Harcama.</param>
        /// <param name="odenekUstu">Ödenek üstü.</param>
        /// <param name="yilSonuIptal">Yıl sonu iptal edilen.</param>
        /// <param name="devir">Devir.</param>
        /// <param name="akreditif">Akreditif.</param>
        /// <param name="akreditifdevir">Akreditif devir.</param>
        /// <param name="taahhutdevir">Taahhut devir.</param>
        /// <param name="disprojedevir">Dış proje devir.</param>
        /// <param name="bagisdevir">Bağış devir.</param>
        /// <param name="ozeldevir">Özel devir.</param>
        /// <param name="digerdevir">Diğer devir.</param>
        /// <param name="gonderilen">Gönderilen ödenek rakamı.</param>
        /// <returns></returns>
        public Sonuc KesinHesapBilgileriKaydet(int yil, int ay, string[] kodlar, double bo, double kbo, double kiciEk, double kiciDus, double karaEk, double karaDus, double yedekEk, double yedekDus, double odenekEkle, double ozelOdenek, double yiliciIptal, double harcama, double odenekUstu, double yilSonuIptal, double devir, double akreditif, double akreditifdevir, double taahhutdevir, double disprojedevir, double bagisdevir, double ozeldevir, double digerdevir, double gonderilen)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") Values (";

            sql += VeriErisim.DegerYap(yil) + ", ";
            sql += VeriErisim.DegerYap(ay) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[0]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[1]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[2]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(((string)kodlar[3]).Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(bo) + ", ";
            sql += VeriErisim.DegerYap(kbo) + ", ";
            sql += VeriErisim.DegerYap(kiciEk) + ", ";
            sql += VeriErisim.DegerYap(kiciDus) + ", ";
            sql += VeriErisim.DegerYap(karaEk) + ", ";
            sql += VeriErisim.DegerYap(karaDus) + ", ";
            sql += VeriErisim.DegerYap(yedekEk) + ", ";
            sql += VeriErisim.DegerYap(yedekDus) + ", ";
            sql += VeriErisim.DegerYap(odenekEkle) + ", ";
            sql += VeriErisim.DegerYap(ozelOdenek) + ", ";
            sql += VeriErisim.DegerYap(yiliciIptal) + ", ";
            sql += VeriErisim.DegerYap(harcama) + ", ";
            sql += VeriErisim.DegerYap(odenekUstu) + ", ";
            sql += VeriErisim.DegerYap(yilSonuIptal) + ", ";
            sql += VeriErisim.DegerYap(devir) + ", ";
            sql += VeriErisim.DegerYap(akreditif) + ", ";
            sql += VeriErisim.DegerYap(akreditifdevir) + ", ";
            sql += VeriErisim.DegerYap(taahhutdevir) + ", ";
            sql += VeriErisim.DegerYap(disprojedevir) + ", ";
            sql += VeriErisim.DegerYap(bagisdevir) + ", ";
            sql += VeriErisim.DegerYap(ozeldevir) + ", ";
            sql += VeriErisim.DegerYap(digerdevir) + ", ";
            sql += VeriErisim.DegerYap(gonderilen) + ") ";

            Sonuc sonuc = KomutCalistir(sql, 1);

            return sonuc;

        }

        /// <summary>
        /// Parametre olarak veilen bilgilere uygun verileri tablodan silen sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kurum">Kurum kodu.</param>
        /// <returns></returns>
        public Sonuc KesinHesapBilgileriSil(int yil, int ay, string kurum)
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
        /// Parametre olarak veilen bilgilere uygun verileri tablodan silen sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <returns></returns>
        public Sonuc KesinHesapBilgileriSil(int yil, int ay, string[] kodlar, string ekKosul)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            string kosul = "";
            kosul = GenelKosul.KistasKosulOl(yil, ay, ekKosul, kodlar, false, "");

            sql = "delete from " + TABLOAD;
            sql = sql + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak veilen bilgilere uygun kesin hesap raporlarını oluşturacak rakamları listeleyen sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <param name="tertipVer">Eğer <c>true</c> ise kur, fon, fin, eko kodlarını da döndürür.</param>
        /// <returns></returns>
        public IDataReader DegerBul(int yil, int ay, string[] kodlar, string ekKosul, bool tertipVer)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOl(yil, ay, ekKosul, kodlar, false, "");

            string sql = "select SUM(" + ALANLAR[(int)ENUMAlanlar.BO] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.KBO] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.KICIEK] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.KICIDUS] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.KARAEK] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.KARADUS] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.YEDEKEK] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.YEDEKDUS] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEKEKLE] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.OZELODENEK] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.YILICIIPTAL] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.HARCAMA] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEKUSTU] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.YILSONUIPTAL] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.DEVIR] + ")" +
                 ",SUM(" + ALANLAR[(int)ENUMAlanlar.AKREDITIF] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.AKREDITIFDEVIR] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.TAAHHUTDEVIR] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.DISPROJEDEVIR] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.BAGISDEVIR] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.OZELDEVIR] + ")" +
                ",SUM(" + ALANLAR[(int)ENUMAlanlar.DIGERDEVIR] + ")" +
                 ",SUM(" + ALANLAR[(int)ENUMAlanlar.GONDERILEN] + ")";
            if (tertipVer)
                sql += ",KURKOD,FONKOD,FINKOD,EKOKOD";

            sql += " from " + TABLOAD;
            sql += " where " + kosul;
            if (tertipVer)
            {
                sql += " GROUP BY KURKOD,FONKOD,FINKOD,EKOKOD";
                sql += " ORDER BY KURKOD,FONKOD,FINKOD,EKOKOD";
            }
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olrak verilen alanlara uygun tertip listesi oluşturan sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="ekKosul">Ek koşul.</param>
        /// <returns></returns>
        public IDataReader TertipListele(int yil, int ay, string[] kodlar, string ekKosul)
        {
            string kosul = "";
            kosul = GenelKosul.KistasKosulOl(yil, ay, ekKosul, kodlar, false, "");

            string sql = "select DISTINCT " +
                ALANLAR[(int)ENUMAlanlar.KURKOD] +
                "," + ALANLAR[(int)ENUMAlanlar.FONKOD] +
                "," + ALANLAR[(int)ENUMAlanlar.FINKOD] +
                "," + ALANLAR[(int)ENUMAlanlar.EKOKOD] +
                " from " + TABLOAD + " where " + kosul +
                " order by 2,1,3,4";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen tür bilgisine göre ek koşul cümlesi oluşturan yordam
        /// </summary>
        /// <param name="tur">Tür.</param>
        /// <returns></returns>
        public string ekKosulOl(int tur)
        {
            string[] kodlar = null;
            string kosul = "";
            if (tur == 1)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YILICIIPTAL], ">", 0);
            else if (tur == 2)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DEVIR], ">", 0);
            else if (tur == 4)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ODENEKUSTU], ">", 0);
            else if (tur == 5)
            {
                kosul = "(" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEDEKDUS], ">", 0);
                kosul = VeriErisim.KosulEkle(kosul, "OR", kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YEDEKEK], ">", 0));
                kosul = kosul + ")";
            }
            else if (tur == 6)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YILSONUIPTAL], ">", 0);
            else if (tur == 7)
            {
                kodlar[0] = "12013100";
                kodlar[1] = "011266";
                kodlar[2] = "1";
                kodlar[3] = "099";
                kosul = GenelKosul.EkKosulOlustur(kodlar);
            }
            else if (tur == 8)
            {
                kosul = "(" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AKREDITIFDEVIR], ">", 0);
                kosul = VeriErisim.KosulEkle(kosul, "OR", kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAAHHUTDEVIR], ">", 0));
                kosul = VeriErisim.KosulEkle(kosul, "OR", kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DISPROJEDEVIR], ">", 0));
                kosul = VeriErisim.KosulEkle(kosul, "OR", kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BAGISDEVIR], ">", 0));
                kosul = VeriErisim.KosulEkle(kosul, "OR", kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.OZELDEVIR], ">", 0));
                kosul = VeriErisim.KosulEkle(kosul, "OR", kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DIGERDEVIR], ">", 0));
                kosul = kosul + ")"; ;
            }
            return kosul;

        }
    }
}
