using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// MUHODENEKIHTIYAC veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOdenekIhtiyac : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOdenekIhtiyac(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "MUHODENEKIHTIYAC";

        enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            KURKOD = 3,
            FONKOD = 4,
            FINKOD = 5,
            EKOKOD = 6,
            HARCAMA1 = 7,
            HARCAMA2 = 8,
            HARCAMA3 = 9,
            ODENEK1 = 10,
            ODENEK2 = 11,
            ODENEK3 = 12,
            ODENEK4 = 13,
            ACIKLAMA = 14
        }
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "HARCAMA1", "HARCAMA2", "HARCAMA3",
                                    "ODENEK1", "ODENEK2", "ODENEK3", "ODENEK4", "ACIKLAMA"};

        /// <summary>
        /// Parametre olarak gelen bilgileri veri tabanına ekleyen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Eklenecek kayıt bilgisini tutan nesne</param>
        /// <returns>Kaydet komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(OdenekIhtiyac o)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(o.yil) + ", ";
            sql += VeriErisim.DegerYap(o.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(o.birim) + ", ";
            sql += VeriErisim.DegerYap(o.kurKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.fonKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.finKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.ekoKod.Replace(".", "")) + ", ";
            sql += VeriErisim.DegerYap(o.harcama1) + ", ";
            sql += VeriErisim.DegerYap(o.harcama2) + ", ";
            sql += VeriErisim.DegerYap(o.harcama3) + ", ";
            sql += VeriErisim.DegerYap(o.odenek1) + ", ";
            sql += VeriErisim.DegerYap(o.odenek2) + ", ";
            sql += VeriErisim.DegerYap(o.odenek3) + ", ";
            sql += VeriErisim.DegerYap(o.odenek4) + ", ";
            sql += VeriErisim.DegerYap(o.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen anahtar alanları kullanarak, ilgili kaydı veri tabanından silen sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="o">Anahtar değişkenleri tutan nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(OdenekIhtiyac o)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", o.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", o.birim));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak gelen alanları kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(OdenekIhtiyac o)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " ,EKOAD";
            sql += " FROM MUHODENEKIHTIYAC LEFT OUTER JOIN ABSEKO ON ";
            sql += " MUHODENEKIHTIYAC.YIL = ABSEKO.YIL AND MUHODENEKIHTIYAC.EKOKOD = ABSEKO.EKOKOD";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", o.muhasebe));

            if (o.birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", o.birim));

            if (o.kurKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", o.kurKod.Replace(".", "")));
            if (o.ekoKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", o.ekoKod.Replace(".", "")));

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleIcmal(OdenekIhtiyac o)
        {
            string sql = "SELECT MUHODENEKIHTIYAC.YIL, MUHODENEKIHTIYAC.MUHASEBEKOD, MUHODENEKIHTIYAC.KURKOD, MUHODENEKIHTIYAC.FONKOD, MUHODENEKIHTIYAC.FINKOD, MUHODENEKIHTIYAC.EKOKOD, sum(MUHODENEKIHTIYAC.HARCAMA1),sum(MUHODENEKIHTIYAC.HARCAMA2),sum(MUHODENEKIHTIYAC.HARCAMA3),sum(MUHODENEKIHTIYAC.ODENEK1),sum(MUHODENEKIHTIYAC.ODENEK2), sum(MUHODENEKIHTIYAC.ODENEK3), sum(MUHODENEKIHTIYAC.ODENEK4), ABSEKO.EKOAD";
            sql += " FROM MUHODENEKIHTIYAC LEFT OUTER JOIN ABSEKO ON ";
            sql += " MUHODENEKIHTIYAC.YIL = ABSEKO.YIL AND MUHODENEKIHTIYAC.EKOKOD = ABSEKO.EKOKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", o.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", o.muhasebe));

            if (o.kurKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", o.kurKod.Replace(".", "")));

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", " + "ABSEKO" + "." + "EKOAD";
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FONKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FINKOD] + ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", " + "ABSEKO" + "." + "EKOAD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <param name="tur">Tur.</param>
        /// <param name="kurumsallar">Kurumsallar kodlar.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleIlk(OdenekIhtiyac o, string tur, string kurumsallar)
        {
            string sql = "SELECT YIL,KURKOD,FONKOD,FINKOD,EKOKOD,SUM(HARCAMA) FROM ISLHARCAMA";
            sql += " WHERE (YIL=" + (o.yil - 3) + " OR YIL=" + (o.yil - 2) + " OR YIL=" + (o.yil - 1) + ")";
            sql += " AND AY = 12 ";

            if (tur == "EKOKODVER")
                sql += " AND MUHASEBEKOD>1000";
            else
                sql += " AND MUHASEBEKOD=" + o.muhasebe;

            if (o.muhasebe > 0)
                sql += " AND MUHASEBEKOD=" + o.muhasebe;
            if (o.birim != "")
                sql += " AND BIRIMKOD='" + o.birim + "'";
            if (o.kurKod != "")
                sql += " AND KURKOD='" + o.kurKod.Replace(".", "") + "'";
            if (o.ekoKod != "")
                sql += " AND EKOKOD LIKE '" + o.ekoKod.Replace(".", "") + "%'";

            string[] k = kurumsallar.Split(';');
            string kurSql = "";
            for (int i = 0; i < k.Length; i++)
            {
                if (kurSql != "") kurSql += " OR ";
                kurSql += " KURKOD='" + k[i].Replace(".", "") + "'";
            }

            if (kurSql != "")
                sql += " AND (" + kurSql + ")";

            sql += " GROUP BY KURKOD,FONKOD,FINKOD,EKOKOD,YIL";
            sql += " ORDER BY KURKOD,FONKOD,FINKOD,EKOKOD,YIL";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <param name="kurumsallar">Kurumsallar kodlar.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleHarcama(OdenekIhtiyac o, string kurumsallar)
        {
            string sql = "SELECT ISLHARCAMA.YIL, ISLHARCAMA.AY, ISLHARCAMA.MUHASEBEKOD, ISLHARCAMA.BIRIMKOD, ISLHARCAMA.KURKOD, ISLHARCAMA.FONKOD, ";
            sql += " ISLHARCAMA.FINKOD, ISLHARCAMA.EKOKOD, ISLHARCAMA.HARCAMA, ISLHARCAMAYILLIK.HARCAMAYILLIK ";
            //sql += " ISLHARCAMA.HARCAMA * 100 / ISLHARCAMAYILLIK.HARCAMAYILLIK AS ORAN";
            sql += " FROM ISLHARCAMA INNER JOIN";
            sql += " ISLHARCAMAYILLIK ON ISLHARCAMA.KURKOD = ISLHARCAMAYILLIK.KURKOD AND ISLHARCAMA.FONKOD = ISLHARCAMAYILLIK.FONKOD AND ";
            sql += " ISLHARCAMA.FINKOD = ISLHARCAMAYILLIK.FINKOD AND ISLHARCAMA.EKOKOD = ISLHARCAMAYILLIK.EKOKOD AND ";
            sql += " ISLHARCAMA.YIL = ISLHARCAMAYILLIK.YIL";
            sql += " WHERE MUHASEBEKOD = " + o.muhasebe + " AND ";
            sql += " BIRIMKOD = '" + o.birim + "' AND ";
            if (o.ekoKod != "")
                sql += " ISLHARCAMA.EKOKOD LIKE '" + o.ekoKod.Replace(".", "") + "%' AND ";
            sql += " (ISLHARCAMA.YIL = " + (o.yil - 3) + " OR ISLHARCAMA.YIL = " + (o.yil - 2) + " OR ISLHARCAMA.YIL = " + (o.yil - 1) + ")";

            string[] k = kurumsallar.Split(';');
            string kurSql = "";
            for (int i = 0; i < k.Length; i++)
            {
                if (kurSql != "") kurSql += " OR ";
                kurSql += " ISLHARCAMA.KURKOD='" + k[i].Replace(".", "") + "'";
            }

            if (kurSql != "")
                sql += " AND (" + kurSql + ")";

            //sql += " ORDER BY ISLHARCAMA.YIL, ISLHARCAMA.AY, ISLHARCAMA.KURKOD, ISLHARCAMA.MUHASEBEKOD, ISLHARCAMA.BIRIMKOD, ISLHARCAMA.EKOKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <param name="kurumsallar">Kurumsallar kodlar.</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleKBO(OdenekIhtiyac o, string kurumsallar)
        {
            string sql = "SELECT YIL, KURKOD, FONKOD, FINKOD, EKOKOD, ODENEK FROM ABSBUTCEHAZIRLIKGIDER";
            sql += " WHERE YIL=" + o.yil + " AND ASAMA=6";
            sql += " AND EKOKOD LIKE '" + o.ekoKod + "%'";
            string[] k = kurumsallar.Split(';');
            string kurSql = "";
            for (int i = 0; i < k.Length; i++)
            {
                if (kurSql != "") kurSql += " OR ";
                kurSql += " KURKOD='" + k[i].Replace(".", "") + "'";
            }

            if (kurSql != "")
                sql += " AND (" + kurSql + ")";

            sql += " ORDER BY YIL, KURKOD, FONKOD, FINKOD, EKOKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Liste sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="o">Select edilecek kayıtlara ait kıstas bilgisini tutan nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader ListeleHarcamaEski(OdenekIhtiyac o)
        {
            string sql = "EKOKOD, YIL, AY, SUM(HARCAMA) FROM ISLHARCAMA ";
            sql += "WHERE (MUHASEBEKOD = " + o.muhasebe + ") AND (BIRIMKOD = '" + o.birim + "') AND " +
                        "(YIL = " + (o.yil - 3) + " OR " +
                        "YIL = " + (o.yil - 2) + " OR" +
                        "YIL = " + (o.yil - 1) + ") " +
                    "AND (KURKOD = '" + o.kurKod + "') " +
                    "AND EKOKOD LIKE '" + o.ekoKod.Replace(".", "") + "%'";


            sql += " GROUP BY EKOKOD,AY,YIL";
            sql += " ORDER BY EKOKOD,AY,YIL";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}