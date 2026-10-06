using System.Data;
using TNS.BUM;
using TNS.Veri;

namespace TNS.KHM.IsKurallari.Veri
{
    /// <summary>
    /// Veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABButceIslemleri : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABButceIslemleri(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ISLGIDEREBUTCE";
        static string[] ALANLAR = { "YIL", "KURUMKOD", "ISLEM", "SIRANO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "ODENEK", "FORMDURUM", "YILDIZYER" };
        enum ENUMAlanlar : int
        {
            YIL = 0,
            KURUMKOD = 1,
            ISLEM = 2,
            SIRANO = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            ODENEK = 8,
            FORMDURUM = 9,
            YILDIZYER = 10
        }

        //static string TABLOADFORM = "ISLFORM";
        //static string[] ALANLARFORM = { "YIL", "KURUMKOD", "ISLEM", "SIRANO", "TARIH", "YAZINO", "ILGI", "NOTLAR", "FORMDURUM", "ALTISLEM", "ILGILIISLEM", "ILGILISIRANO", "ISLEMYAPANKOD", "ISLEMYAPAN", "DEGISIKLIKTARIH", "ONAYTARIH" };
        //enum ENUMAlanlarFORM : int
        //{
        //    YIL = 0,
        //    KURUMKOD = 1,
        //    ISLEM = 2,
        //    SIRANO = 3,
        //    TARIH = 4,
        //    YAZINO = 5,
        //    ILGI = 6,
        //    NOTLAR = 7,
        //    FORMDURUM = 8,
        //    ALTISLEM = 9,
        //    ILGILIISLEM = 10,
        //    ILGILISIRANO = 11,
        //    ISLEMYAPANKTKOD = 12,
        //    ISLEMYAPANKULKOD = 13,
        //    DEGISIKLIKTARIH = 14,
        //    ONAYTARIH = 15
        //}

        static string TABLOADDEVIR = "ISLDEVIR";
        static string[] ALANLARDEVIR = { "YIL", "KURUMKOD", "ISLEM", "SIRANO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "KURKODE", "FONKODE", "FINKODE", "EKOKODE", "ODENEK", "FORMDURUM" };
        enum ENUMAlanlarDEVIR : int
        {
            YIL = 0,
            KURUMKOD = 1,
            ISLEM = 2,
            SIRANO = 3,
            KURKOD = 4,
            FONKOD = 5,
            FINKOD = 6,
            EKOKOD = 7,
            KURKODE = 8,
            FONKODE = 9,
            FINKODE = 10,
            EKOKODE = 11,
            ODENEK = 12,
            FORMDURUM = 13
        }

        /// <summary>
        /// Butce islemleri sonuçlarını aylık olarak bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="ay">Ay.</param>
        /// <param name="islemKod">İşlem kodu.</param>
        /// <param name="kodlar">Bütçe kodları.</param>
        /// <param name="formDurumGrupKod">Form durumu.</param>
        /// <param name="ekkosul">Ek koşullar.</param>
        /// <returns></returns>
        public IDataReader ButceIslemleriSonuclariBulAylik(int yil, int ay, string islemKod, string[] kodlar, ENUMFormDurumGrup formDurumGrupKod, string ekkosul)
        {

            string kosul = "";
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + "," + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + "), SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + "),KURUMKOD FROM " + TABLOAD;// +"," + TABLOADFORM;

            sql = VeriErisim.KosulEkle(sql, "WHERE", GenelKosul.KistasKosulOl(yil, -1, "", kodlar, false, TABLOAD));

            //sql = VeriErisim.KosulEkle(sql, " AND ", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + "=" + TABLOADFORM + "." + ALANLARFORM[(int)ENUMAlanlarFORM.YIL]);
            //sql = VeriErisim.KosulEkle(sql, " AND ", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KURUMKOD] + "=" + TABLOADFORM + "." + ALANLARFORM[(int)ENUMAlanlarFORM.KURUMKOD]);
            //sql = VeriErisim.KosulEkle(sql, " AND ", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + "=" + TABLOADFORM + "." + ALANLARFORM[(int)ENUMAlanlarFORM.ISLEM]);
            //sql = VeriErisim.KosulEkle(sql, " AND ", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRANO] + "=" + TABLOADFORM + "." + ALANLARFORM[(int)ENUMAlanlarFORM.SIRANO]);
            //if (ay < 12)
            //{
            //    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", "MONTH(ONAYTARIH)", "<=", ay));
            //    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", "YEAR(ONAYTARIH)", "<=", yil));
            //}


            sql = VeriErisim.KosulEkle(sql, "and", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEM], "LIKE", islemKod.Replace(".", "") + "%"));

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            sql = GenelKosul.FormDurumKosulOl(sql, "AND", formDurumGrupKod, TABLOAD, ALANLAR[(int)ENUMAlanlar.FORMDURUM]);
            sql += " group by " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + "," + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + "),KURUMKOD";
            sql += " order by " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ISLEM] + "," + VeriErisim.IslecBul("SIGN", veriErisim.baglantiAd) + "(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + "),KURUMKOD";

            IDataReader dr = veriErisim.DataReaderDondur(sql);
            return dr;
        }

        /// <summary>
        /// Devir rakamını bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="islemKod">İşlem kodu</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="formDurumGrupKod">Form durumu</param>
        /// <returns></returns>
        public double DevirRakamiGetir(int yil, string islemKod, string[] kodlar, ENUMFormDurumGrup formDurumGrupKod)
        {
            string kosul = "";
            string sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")" + " FROM " + TABLOADDEVIR;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.YIL], "=", yil));
            if (kodlar.Length >= 1 && (string)kodlar[0] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.KURKODE], "LIKE", kodlar[0] + "%"));
            if (kodlar.Length >= 2 && (string)kodlar[1] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FONKODE], "LIKE", kodlar[1] + "%"));
            if (kodlar.Length >= 3 && (string)kodlar[2] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FINKODE], "LIKE", kodlar[2] + "%"));
            if (kodlar.Length >= 4 && (string)kodlar[3] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.EKOKODE], "LIKE", kodlar[3] + "%"));

            ObjectArray d = new ObjectArray();
            int parca = OrtakFonksiyonlar.StringiParcala(islemKod, ";", d);
            foreach (string yetkiTipiDegeriParca in d.objeler)
            {
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.ISLEM], "LIKE", yetkiTipiDegeriParca.Replace(".", "") + "%"));
            }
            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            sql = GenelKosul.FormDurumKosulOl(sql, "AND", formDurumGrupKod, "", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FORMDURUM]);
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Devir rakamını bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="islemKod">İşlem kodu</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="formDurumGrupKod">Form durumu</param>
        /// <returns></returns>
        public double DevirRakamiGetir(int yil, string[] islemKod, string[] kodlar, ENUMFormDurumGrup formDurumGrupKod)
        {
            string kosul = "";
            string sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")" + " FROM " + TABLOADDEVIR;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.YIL], "=", yil));
            if (kodlar.Length >= 1 && (string)kodlar[0] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.KURKODE], "LIKE", kodlar[0] + "%"));
            if (kodlar.Length >= 2 && (string)kodlar[1] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FONKODE], "LIKE", kodlar[1] + "%"));
            if (kodlar.Length >= 3 && (string)kodlar[2] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FINKODE], "LIKE", kodlar[2] + "%"));
            if (kodlar.Length >= 4 && (string)kodlar[3] != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.EKOKODE], "LIKE", kodlar[3] + "%"));

            for (int i = 0; i < islemKod.Length; i++)
            {
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.ISLEM], "LIKE", islemKod[i].Replace(".", "") + "%"));
            }
            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");
            sql = GenelKosul.FormDurumKosulOl(sql, "AND", formDurumGrupKod, "", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FORMDURUM]);
            return veriErisim.TekKolonSQLDbl(sql);
        }

        /// <summary>
        /// Devir akreditif (Bütçe İşlemleri) listelesini bulmak için sql cümlesini oluşturan yordam
        /// </summary>
        /// <param name="yil">Yıl.</param>
        /// <param name="islemKod">İşlem kodu</param>
        /// <param name="kodlar">Bütçe kodları</param>
        /// <param name="formDurumGrupKod">Form durumu</param>
        /// <returns></returns>
        public IDataReader DevirAkreditifListele(int yil, string[] islemKod, string[] kodlar, ENUMFormDurumGrup formDurumGrupKod)
        {
            string kosul = "";
            string kosul2 = "";
            string sql = "SELECT " +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.KURKODE] + "," +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FONKODE] + "," +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FINKODE] + "," +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.EKOKODE] + "," +
                "SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ")" + " FROM " + TABLOADDEVIR;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.YIL], "=", yil));

            sql = VeriErisim.KosulEkle(sql, "AND", GenelKosul.FormDurumKosulOl(kosul, "AND", formDurumGrupKod, "", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FORMDURUM]));

            for (int i = 0; i < islemKod.Length; i++)
            {
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.ISLEM], "LIKE", islemKod[i].Replace(".", "") + "%"));
            }

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "AND", "(" + kosul + ")");

            sql = sql + "GROUP BY " +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.KURKODE] + "," +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FONKODE] + "," +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FINKODE] + "," +
                ALANLARDEVIR[(int)ENUMAlanlarDEVIR.EKOKODE];

            if (kodlar.Length >= 1 && (string)kodlar[0] != "")
                kosul2 = VeriErisim.KosulEkle(kosul2, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.KURKODE], "LIKE", kodlar[0] + "%"));
            if (kodlar.Length >= 2 && (string)kodlar[1] != "")
                kosul2 = VeriErisim.KosulEkle(kosul2, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FONKODE], "LIKE", kodlar[1] + "%"));
            if (kodlar.Length >= 3 && (string)kodlar[2] != "")
                kosul2 = VeriErisim.KosulEkle(kosul2, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.FINKODE], "LIKE", kodlar[2] + "%"));
            if (kodlar.Length >= 4 && (string)kodlar[3] != "")
                kosul2 = VeriErisim.KosulEkle(kosul2, "AND", VeriErisim.KosulOl("", ALANLARDEVIR[(int)ENUMAlanlarDEVIR.EKOKODE], "LIKE", kodlar[3] + "%"));

            if (kosul2 != "")
                sql = VeriErisim.KosulEkle(sql, " HAVING ", kosul2);

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
