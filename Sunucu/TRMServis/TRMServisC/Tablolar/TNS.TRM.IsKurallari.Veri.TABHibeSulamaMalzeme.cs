using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{
    public class TABHibeSulamaMalzeme : TABOrtak
    {
        public TABHibeSulamaMalzeme(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBESULAMAMALZEMEFIYAT";
        public enum ENUMAlanlar
        {
            HIBEKOD = 0,
            TIP = 1,
            MALZEMEKOD = 2,
            SATIRNO = 3,
            SULAMAGRUPKOD = 4,
            PARSELID = 5,
            MIKTAR = 6,
            BIRIMFIYAT = 7,
            REFERANSFIYAT = 8,
            PROJETUTAR = 9,
            HIBEESASTUTAR = 10
        }
        public static string[] ALANLAR = { "HIBEKOD", "TIP", "MALZEMEKOD", "SATIRNO", "SULAMAGRUPKOD", "PARSELID", "MIKTAR", "BIRIMFIYAT", "REFERANSFIYAT", "PROJETUTAR", "HIBEESASTUTAR" };

        public static string TABLOADMALZEMEOZELLIK = "TRMHIBESULAMAMALZEMEOZELLIK";
        public enum ENUMAlanlarMalzemeOzellik
        {
            HIBEKOD = 0,
            TIP = 1,
            MALZEMEKOD = 2,
            SATIRNO = 3,
            OZELLIKKOD = 4,
            SULAMAGRUPKOD = 5,
            PARSELID = 6,
            DEGER = 7
        }
        public static string[] ALANLARMALZEMEOZELLIK = { "HIBEKOD", "TIP", "MALZEMEKOD", "SATIRNO", "OZELLIKKOD", "SULAMAGRUPKOD", "PARSELID", "DEGER" };

        public Sonuc Kaydet(HibeSulamaMalzemeFiyat m)
        {
            if (m.malzemeKod==0)
            {
                m.satirNo = 0;
                m.sulamaGrupKod = 0;
            }

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(m.hibeKod) + ",";
            sql += VeriErisim.DegerYap((int)m.tip) + ",";
            sql += VeriErisim.DegerYap(m.malzemeKod) + ",";
            sql += VeriErisim.DegerYap(m.satirNo) + ",";
            sql += VeriErisim.DegerYap(m.sulamaGrupKod) + ",";
            sql += VeriErisim.DegerYap(m.parselID) + ",";
            sql += VeriErisim.DegerYap(m.miktar) + ",";
            sql += VeriErisim.DegerYap(m.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(m.referansFiyat) + ",";
            sql += VeriErisim.DegerYap(m.projeTutar) + ",";
            sql += VeriErisim.DegerYap(m.hibeEsasTutar) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int hibeKod, ENUMHibeSulamaMalzemeTip tip, int sulamaGrupKod, string parselID)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)tip));

            if (parselID != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.PARSELID], "=", parselID));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.SULAMAGRUPKOD], "=", sulamaGrupKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(int hibeKod, ENUMHibeSulamaMalzemeTip tip, int sulamaGrupKod, string parselID)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TRMSULAMAGRUBU.AD, TRMHIBESULAMADIGER.PARSELNO FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN TRMHIBESULAMADIGER ON TRMHIBESULAMADIGER.PARSELID = TRMHIBESULAMAMALZEMEFIYAT.PARSELID and TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD = TRMHIBESULAMADIGER.HIBEKOD";
            sql += " LEFT OUTER JOIN TRMSULAMAGRUBU ON TRMSULAMAGRUBU.KOD = TRMHIBESULAMADIGER.SULAMAGRUPKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));

            if (tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", (int)tip));

            if (parselID != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PARSELID], "=", parselID));

            if (sulamaGrupKod != 0)
                kosul = (VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SULAMAGRUPKOD], "=", sulamaGrupKod)));// + " OR SULAMAGRUPKOD=0");

            if (kosul != "")
            {
                kosul = " AND (" + kosul + ")";
                sql += kosul;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleSulamaGrupTutar(int hibeKod, int tip)
        {
            string sql = "SELECT TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD, TRMHIBESULAMAMALZEMEFIYAT.SULAMAGRUPKOD, SUM(PROJETUTAR) AS TOPLAMPROJETUTAR, SUM(HIBEESASTUTAR) AS TOPLAMHIBEESASTUTAR, TRMSULAMAGRUBU.AD, ";
            //sql += " MAX(TRMHIBESULAMADIGER.PARSELALAN), MAX(TRMHIBESULAMADIGER.PARSELSULANACAKALAN) ";
            sql += " (SELECT SUM(PARSELALAN) FROM TRMHIBESULAMADIGER SD WHERE SD.HIBEKOD=TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD AND SD.SULAMAGRUPKOD=TRMHIBESULAMAMALZEMEFIYAT.SULAMAGRUPKOD AND PARSELSUKAYNAKTIP<>2 ),";
            sql += " (SELECT SUM(PARSELSULANACAKALAN) FROM TRMHIBESULAMADIGER SD WHERE SD.HIBEKOD=TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD AND SD.SULAMAGRUPKOD=TRMHIBESULAMAMALZEMEFIYAT.SULAMAGRUPKOD AND PARSELSUKAYNAKTIP<>2) ";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN TRMSULAMAGRUBU ON TRMSULAMAGRUBU.KOD=TRMHIBESULAMAMALZEMEFIYAT.SULAMAGRUPKOD";
            sql += " LEFT OUTER JOIN TRMHIBESULAMADIGER ON TRMHIBESULAMADIGER.PARSELID = TRMHIBESULAMAMALZEMEFIYAT.PARSELID AND TRMHIBESULAMADIGER.HIBEKOD = TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", tip));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TRMHIBESULAMADIGER", "PARSELSUKAYNAKTIP", "<>", 2));//Reddedilen parsel işleme dahil edilmesin

            sql += " GROUP BY TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD,TRMHIBESULAMAMALZEMEFIYAT.SULAMAGRUPKOD,TRMSULAMAGRUBU.AD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BirimRefFarklimi(int hibeKod)
        {
            string sql = " select TRMHIBESULAMA.BASVURUNO, ";
            sql += "    (select SUM(TRMHIBESULAMAMALZEMEFIYAT.BIRIMFIYAT) from TRMHIBESULAMAMALZEMEFIYAT ";
            sql += "      where TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD=TRMHIBESULAMA.KOD and TRMHIBESULAMAMALZEMEFIYAT.TIP=1)- ";
            sql += "    (select SUM(TRMHIBESULAMAMALZEMEFIYAT.BIRIMFIYAT) from TRMHIBESULAMAMALZEMEFIYAT ";
            sql += "      where TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD=TRMHIBESULAMA.KOD and TRMHIBESULAMAMALZEMEFIYAT.TIP=2) BirimFark, ";
            sql += "    (select SUM(TRMHIBESULAMAMALZEMEFIYAT.REFERANSFIYAT) from TRMHIBESULAMAMALZEMEFIYAT ";
            sql += "      where TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD=TRMHIBESULAMA.KOD and TRMHIBESULAMAMALZEMEFIYAT.TIP=1) - ";
            sql += "    (select SUM(TRMHIBESULAMAMALZEMEFIYAT.REFERANSFIYAT) from TRMHIBESULAMAMALZEMEFIYAT ";
            sql += "      where TRMHIBESULAMAMALZEMEFIYAT.HIBEKOD=TRMHIBESULAMA.KOD and TRMHIBESULAMAMALZEMEFIYAT.TIP=2) ReferansFark ";
            sql += "  from TRMHIBESULAMA  ";
            sql += " where  KOD=" + hibeKod.ToString() + " ";

            return veriErisim.DataReaderDondur(sql);
        }

        public int SatirNoVer(int hibeKod, int sulamaGrupKod)
        {
            string sql = "SELECT MAX(SATIRNO) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIBEKOD], "=", hibeKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SULAMAGRUPKOD], "=", sulamaGrupKod));

            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc KaydetOzellik(HibeSulamaMalzemeOzellik ozellik)
        {
            string sql = "INSERT INTO " + TABLOADMALZEMEOZELLIK + " (" + VeriErisim.TumAlanlar(string.Empty, ALANLARMALZEMEOZELLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(ozellik.hibeKod) + ",";
            sql += VeriErisim.DegerYap((int)ozellik.tip) + ",";
            sql += VeriErisim.DegerYap(ozellik.malzemeKod) + ",";
            sql += VeriErisim.DegerYap(ozellik.satirNo) + ",";
            sql += VeriErisim.DegerYap(ozellik.ozellikKod) + ",";
            sql += VeriErisim.DegerYap(ozellik.sulamaGrupKod) + ",";
            sql += VeriErisim.DegerYap(ozellik.parselID) + ",";
            sql += VeriErisim.DegerYap(ozellik.deger) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilOzellik(int hibeKod, ENUMHibeSulamaMalzemeTip tip, int sulamaGrupKod, string parselID)
        {
            string sql = "DELETE FROM " + TABLOADMALZEMEOZELLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.TIP], "=", (int)tip));

            if (parselID != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.PARSELID], "=", parselID));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.SULAMAGRUPKOD], "=", sulamaGrupKod));


            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleOzellik(int hibeKod, ENUMHibeSulamaMalzemeTip tip, int malzemeKod, int satir)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(string.Empty, ALANLARMALZEMEOZELLIK) + " FROM " + TABLOADMALZEMEOZELLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.HIBEKOD], "=", hibeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.TIP], "=", (int)tip));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.MALZEMEKOD], "=", malzemeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLARMALZEMEOZELLIK[(int)ENUMAlanlarMalzemeOzellik.SATIRNO], "=", satir));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}