using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BKD;

namespace TNS.BHMv2.IsKurallari.Veri
{
    public class TABMPGMASRAFPLANI : TABOrtak
    {
        public TABMPGMASRAFPLANI(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADREVIZEFORM = "MPGMASRAFPLANIREVIZEFORM";
        public enum ENUMAlanlarREVIZEFORM
        {
            YIL = 0,
            REVIZENO = 1,
            FORMDURUM = 2,
            KAYDEDEN = 3,
            KAYITTARIH = 4,
            ONAYLAYAN = 5,
            ONAYTARIH = 6,
            BELGETUR = 7,
            EVRAKTARIH = 8,
            EVRAKNO = 9,
            EVRAKILGI = 10,
            ACIKLAMA = 11,
            BUMKOKURUMKOD = 12,
            BUMKOISLEMNO = 13,
            ALTISLEMKOD = 14,
            YEDEKTERTIPSTR = 15,
            ILGILIREVIZENO = 16,
            DAYANAKKODLAR = 17,
            ODEGONYIL = 18,
            ODEGONTUR = 19,
            ODEGONNO = 20,
            PROJELERARASI = 21,
            SEKTORLERARASI = 22,
            BOLGELERARASI = 23,
            ONAYYAZINO = 24,
            ONAYYAZITARIH = 25
        };

        public static string[] ALANLARREVIZEFORM =
        {
            "YIL",
            "REVIZENO",
            "FORMDURUM",
            "KAYDEDEN",
            "KAYITTARIH",
            "ONAYLAYAN",
            "ONAYTARIH",
            "BELGETUR",
            "EVRAKTARIH",
            "EVRAKNO",
            "EVRAKILGI",
            "ACIKLAMA",
            "BUMKOKURUMKOD",
            "BUMKOISLEMNO",
            "ALTISLEMKOD",
            "YEDEKTERTIPSTR",
            "ILGILIREVIZENO",
            "DAYANAKKODLAR", "ODEGONYIL", "ODEGONTUR", "ODEGONNO",
            "PROJELERARASI","SEKTORLERARASI","BOLGELERARASI","ONAYYAZINO","ONAYYAZITARIH"
        };

        public static string TABLOADTERTIP = "MPGMASRAFPLANITERTIP";
        public enum ENUMAlanlarTERTIP
        {
            YIL = 0,
            ASAMA = 1,
            KURKOD = 2,
            FONKOD = 3,
            FINKOD = 4,
            EKOKOD = 5,
            PEBKOD = 6,
            KURUMSALYAPIKOD = 7,
            AY = 8,
            ODENEK = 9,
            ODENEKGONDERME = 10,
            DURUM = 11,
            REVIZENO = 12,
            GRUP = 13,
            IOIISKOD = 14,
            TERTIPSIRA = 15
        }

        public static string[] ALANLARTERTIP =
        {
            "YIL",
            "ASAMA",
            "KURKOD",
            "FONKOD",
            "FINKOD",
            "EKOKOD",
            "PEBKOD",
            "KURUMSALYAPIKOD",
            "AY",
            "ODENEK",
            "ODENEKGONDERME",
            "DURUM",
            "REVIZENO",
            "GRUP",
            "IOIISKOD",
            "TERTIPSIRA"
        };

        public static string TABLOADDURUM = "MPGMASRAFPLANIDURUM";
        public enum ENUMAlanlarDURUM
        {
            YIL = 0,
            KURUMSALYAPIKOD = 1,
            ASAMA = 2,
            EKOKOD = 3,
            DURUM = 4,
            KULLANICI = 5,
            DURUMTARIH = 6,
        }

        public static string[] ALANLARDURUM =
        {
            "YIL",
            "KURUMSALYAPIKOD",
            "ASAMA",
            "EKOKOD",
            "DURUM",
            "KULLANICI",
            "DURUMTARIH"
        };

        public static string TABLOADTARIHCE = "MPGMASRAFPLANITARIHCE";
        public enum ENUMAlanlarTARIHCE
        {
            YIL = 0,
            KURUMSALYAPIKOD = 1,
            ASAMA = 2,
            EKOKOD = 3,
            DURUMTARIH = 4,
            DURUM = 5,
            KULLANICI = 6,
        }

        public static string[] ALANLARTARIHCE =
        {
            "YIL",
            "KURUMSALYAPIKOD",
            "ASAMA",
            "EKOKOD",
            "DURUMTARIH",
            "DURUM",
            "KULLANICI"
        };

        //public IDataReader ListeleGenel(int yil, string kurumsalYapikod, ENUMMasrafPlanAsama asama, string kurKod, string kurumsalYapiEk)
        //{
        //    string sql = "SELECT LEFT(EKOKOD, 3), AY, SUM(ODENEK) FROM " + TABLOADTERTIP;
        //    sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
        //    if (!string.IsNullOrEmpty(kurumsalYapikod))
        //        sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kurumsalYapikod.Replace(".", "") + "%");
        //    else if (!string.IsNullOrEmpty(kurumsalYapiEk))
        //    {
        //        if (kurumsalYapiEk == "9")
        //        {
        //            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", "9%");
        //        }
        //        else
        //            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "<", "9");
        //    }
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0);
        //    if (!string.IsNullOrEmpty(kurKod))
        //        sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
        //    sql += " GROUP BY LEFT(EKOKOD, 3), AY ORDER BY LEFT(EKOKOD, 3), AY";
        //    return this.veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader ListeleTertipAy(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama)
        //{
        //    string sql = "SELECT AY, SUM(ODENEK) FROM " + TABLOADTERTIP;
        //    sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", kurumsalYapiKod.Replace(".", ""));
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0);
        //    sql += " GROUP BY AY ORDER BY AY";
        //    return this.veriErisim.DataReaderDondur(sql);
        //}

        //public IDataReader ListeleTertip(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string ekoKod2)
        //{
        //    string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, AY, SUM(ODENEK) FROM " + TABLOADTERTIP;
        //    sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", kurumsalYapiKod.Replace(".", ""));
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod2.Replace(".", "") + "%");
        //    sql += " GROUP BY KURKOD, FONKOD, FINKOD, EKOKOD, AY ORDER BY 1,2,3,4,5";
        //    return this.veriErisim.DataReaderDondur(sql);
        //}

        public IDataReader ListeleTertip(Tertip kistasTertip, ENUMMasrafPlanAsama asama, int ay)
        {
            string groupByAlanlar = " ";
            groupByAlanlar += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + ", ";
            groupByAlanlar += TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.SAYMANLIKKOD] + ", ";
            groupByAlanlar += TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.BIRIMKOD] + ", ";
            groupByAlanlar += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD];

            string sql = "SELECT " + groupByAlanlar + ", " + "SUM(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ") ";
            sql += " FROM " + TABLOADTERTIP + ", " + TABBHMKURUMSALYAPI.TABLOAD;
            sql += " WHERE " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", kistasTertip.yil);
            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.AHP);
                sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "=", (int)5);
            }
            else
            {
                sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
                sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0);
            }
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "<=", ay);

            if (!string.IsNullOrEmpty(kistasTertip.kodlar[0]))
                sql += VeriErisim.AlanKosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], kistasTertip.kodlar[0], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[1]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], kistasTertip.kodlar[1], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[2]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], kistasTertip.kodlar[2], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[3]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], kistasTertip.kodlar[3], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[4]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], kistasTertip.kodlar[4], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[5]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], kistasTertip.kodlar[5], " AND ");
            sql += " AND " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + "=" + TABBHMKURUMSALYAPI.TABLOAD + "." + TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.KOD];
            sql += " GROUP BY " + groupByAlanlar;
            sql += " ORDER BY " + groupByAlanlar;
            return this.veriErisim.DataReaderDondur(sql);
        }


        public Sonuc EkleTertip(ENUMMasrafPlanAsama asama, Tertip tertip, int ay, double odenek, int revizeNo, int formDurum, int grup, int ioiIsKod, int tertipSira)
        {
            if (odenek == 0) return new Sonuc(true, "", "");
            string sql = "INSERT INTO " + TABLOADTERTIP + "(" + VeriErisim.TumAlanlar("", ALANLARTERTIP) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(tertip.yil);
            sql += "," + VeriErisim.DegerYap((int)asama);
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[0].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[1].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[2].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[3].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[4].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(tertip.kodlar[5].Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(ay);
            sql += "," + VeriErisim.DegerYap(odenek);
            sql += "," + VeriErisim.DegerYap(0);
            sql += "," + VeriErisim.DegerYap(formDurum);
            sql += "," + VeriErisim.DegerYap(revizeNo);
            sql += "," + VeriErisim.DegerYap(grup);
            sql += "," + VeriErisim.DegerYap(ioiIsKod);
            sql += "," + VeriErisim.DegerYap(tertipSira);
            sql += ")";

            if (double.IsNaN(odenek))
                throw new Exception(sql);
            return KomutCalistir(sql, 1);
        }

        public int DurumBul(int yil, string kurumsalYapikod, ENUMMasrafPlanAsama asama, string ekoKod2)
        {
            string sql = "SELECT DURUM FROM " + TABLOADDURUM;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.KURUMSALYAPIKOD], "=", kurumsalYapikod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.EKOKOD], "=", ekoKod2.Replace(".", ""));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc SilDurum(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string ekoKod2)
        {
            string sql = "DELETE FROM " + TABLOADDURUM;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.KURUMSALYAPIKOD], "=", kurumsalYapiKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARDURUM[(int)ENUMAlanlarDURUM.EKOKOD], "=", ekoKod2.Replace(".", ""));
            return KomutCalistir(sql, 0);
        }

        public Sonuc EkleDurum(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string ekoKod2, int durum, string kullaniciKod)
        {
            Sonuc s;
            string sql = "INSERT INTO " + TABLOADDURUM + "(" + VeriErisim.TumAlanlar("", ALANLARDURUM) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(kurumsalYapiKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap((int)asama);
            sql += "," + VeriErisim.DegerYap(ekoKod2.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(durum);
            sql += "," + VeriErisim.DegerYap(kullaniciKod);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += ")";
            s = KomutCalistir(sql, 1);
            if (s.islemSonuc)
                return EkleTarihce(yil, kurumsalYapiKod, asama, ekoKod2, durum, kullaniciKod);
            else
                return s;
        }

        public Sonuc EkleTarihce(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string ekoKod2, int durum, string kullaniciKod)
        {
            string sql = "INSERT INTO " + TABLOADTARIHCE + "(" + VeriErisim.TumAlanlar("", ALANLARTARIHCE) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(yil);
            sql += "," + VeriErisim.DegerYap(kurumsalYapiKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap((int)asama);
            sql += "," + VeriErisim.DegerYap(ekoKod2.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap(durum);
            sql += "," + VeriErisim.DegerYap(kullaniciKod);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SilTertip(ENUMMasrafPlanAsama asama, Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", tertip.yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", tertip.kodlar[5].Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "=", tertip.kodlar[0].Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "=", tertip.kodlar[1].Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "=", tertip.kodlar[2].Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "=", tertip.kodlar[3].Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], "=", tertip.kodlar[4].Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", 0);

            return KomutCalistir(sql, 0);
        }

        public double DagitimBul(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string fonKod, string finKod, string ekoKod)
        {
            string sql = "SELECT SUM(ODENEK)FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0);
            if (!string.IsNullOrEmpty(kurumsalYapiKod))
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kurumsalYapiKod.Replace(".", "") + "%");
            if (!string.IsNullOrEmpty(kurKod))
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            if (!string.IsNullOrEmpty(fonKod))
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", fonKod.Replace(".", "") + "%");
            if (!string.IsNullOrEmpty(finKod))
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", finKod.Replace(".", "") + "%");
            if (!string.IsNullOrEmpty(ekoKod))
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%");
            return veriErisim.TekKolonSQLDbl(sql);
        }

        public IDataReader EkonomikListele(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, int revizeNo)
        {
            string sql = "SELECT LEFT(EKOKOD,2), SUM(ODENEK) FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kurumsalYapiKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            if (revizeNo >= 0)
            {
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)revizeNo);
            }
            if (!string.IsNullOrEmpty(kurKod))
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            if (!string.IsNullOrEmpty(ekoKod))
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%");
            sql += " GROUP BY LEFT(EKOKOD,2) ORDER BY 1";
            return this.veriErisim.DataReaderDondur(sql);
        }

        public int KayitSayisiBul(int yil, ENUMMasrafPlanAsama asama)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += " GROUP BY AY ORDER BY 1";
            return this.veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc AsamaKopyala(int kaynakYil, ENUMMasrafPlanAsama kaynakAsama, int hedefYil, ENUMMasrafPlanAsama hedefAsama, string kurumsalYapiKod, string kurKod, string ekoKod)
        {
            string sql = "INSERT INTO " + TABLOADTERTIP + " ";

            sql += "SELECT " + VeriErisim.DegerYap(hedefYil) + " as " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL] + ", " +
            VeriErisim.DegerYap((int)hedefAsama) + " as " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEKGONDERME] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.GRUP] + ", " +
            ALANLARTERTIP[(int)ENUMAlanlarTERTIP.TERTIPSIRA] + " FROM " + TABLOADTERTIP;

            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", kaynakYil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)kaynakAsama);
            if (kurumsalYapiKod != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kurumsalYapiKod.Replace(".", "") + "%");
            if (kurKod != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            if (ekoKod != "")
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%");
            return KomutCalistir(sql, 0);
        }

        public IDataReader AHPFarkliMPler(int yil, int ay, string kurumKod, int ahpAsama, int mpAsama)
        {
            string sql = $@"SELECT COALESCE(T1.KURKOD, T2.KURKOD), COALESCE(T1.FONKOD, T2.FONKOD), COALESCE(T1.FINKOD, T2.FINKOD), COALESCE(T1.EKOKOD, T2.EKOKOD), COALESCE(T1.PEBKOD, T2.PEBKOD), COALESCE(T1.AY, T2.AY), T1.ODENEK, T2.ODENEK FROM  
            (SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY, SUM(ODENEK) ODENEK
            FROM ButceIslemleri.dbo.ISLAPGIDER
            where YIL={yil} AND AY<={ay} AND KURUMKOD={VeriErisim.DegerYap(kurumKod.Replace(".", ""))} AND ISLEM='0101' AND SIRANO={ahpAsama}
            GROUP BY KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY) T1 
            
            FULL OUTER JOIN 

            (SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY, SUM(ODENEK) ODENEK
            FROM ButceIslemleri.dbo.MPGMASRAFPLANITERTIP
            where YIL={yil} AND AY<={ay} AND ASAMA={mpAsama} AND KURKOD LIKE {VeriErisim.DegerYap(kurumKod.Replace(".", "") + "%")} AND REVIZENO=0
            GROUP BY KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY) T2
            
            ON T1.KURKOD = T2.KURKOD AND T1.FONKOD = T2.FONKOD AND T1.FINKOD = T2.FINKOD AND T1.EKOKOD = T2.EKOKOD AND T1.PEBKOD = T2.PEBKOD AND T1.AY = T2.AY
            where ISNULL(T1.ODENEK, 0) <> ISNULL(T2.ODENEK, 0) "; //AND ISNULL(T2.ODENEK, 0) <> 0 

            return this.veriErisim.DataReaderDondur(sql);
        }

        public IDataReader OdenektenBuyukAHPBul(int yil, string kurumKod, int ahpAsama, bool eko23, string senaryoKod)
        {
            string ekoAlan = BKD.Arac.EkoAlanOl(yil, "", "EKOKOD", 2, eko23, VeriErisim.IslecBul("SUBSTRING"));

            string sql = $@"SELECT T1.KURKOD, T1.FONKOD, T1.FINKOD, T1.EKOKOD, T1.PEBKOD, T1.ODENEK, T2.ODENEK FROM  (SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, SUM(ODENEK) ODENEK
            FROM ButceIslemleri.dbo.ISLAPGIDER AS AHP
            where AHP.YIL={yil} AND KURUMKOD={VeriErisim.DegerYap(kurumKod.Replace(".", ""))} AND AHP.SIRANO={ahpAsama} AND ISLEM='0101'
            GROUP BY AHP.KURKOD, AHP.FONKOD, AHP.FINKOD, AHP.EKOKOD, AHP.PEBKOD) T1 
            
            FULL OUTER JOIN 

            (SELECT KURKOD, FONKOD, FINKOD, {ekoAlan} EKOKOD, PEBKOD, SUM(ODENEK) ODENEK
            FROM ButceHazirlikv2.dbo.BHMODENEK AS O
            where O.YIL={yil} AND SENARYOKOD='{senaryoKod.Replace(".", "")}' AND TEKLIFYIL=0 AND O.KURKOD LIKE {VeriErisim.DegerYap(kurumKod.Replace(".", "") + "%")}
            GROUP BY KURKOD, FONKOD, FINKOD, {ekoAlan}, PEBKOD) T2
            
            ON T1.KURKOD = T2.KURKOD AND T1.FONKOD = T2.FONKOD AND T1.FINKOD = T2.FINKOD AND T1.EKOKOD = T2.EKOKOD AND T1.PEBKOD = T2.PEBKOD
            where ISNULL(T1.ODENEK, 0) > ISNULL(T2.ODENEK, 0)";

            return this.veriErisim.DataReaderDondur(sql);
        }

        public string AylikDagilimKosulOl(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, string[] ekoKodlar, int ay)
        {
            string sql = "";
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kurumsalYapiKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", 0);
            if (ay >= 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "=", ay);
            if (ekoKodlar != null && ekoKodlar.Length > 0)
            {
                string icSql = "";
                for (int i = 0; i < ekoKodlar.Length; i++)
                {
                    if (!string.IsNullOrEmpty(icSql)) icSql += " OR ";
                    icSql += VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "=", ekoKodlar[i].Replace(".", ""));
                }
                sql += " AND (" + icSql + ")";
            }

            return sql;
        }

        public Sonuc AylikDagilimYap(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, string[] ekoKodlar, int ay, double eskiDeger, double yeniDeger)
        {
            string sql = "UPDATE " + TABLOADTERTIP + " ";
            sql += "SET " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + "= ROUND(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + "*" + yeniDeger.ToString() + "/" + eskiDeger.ToString() + ",0) ";
            sql += AylikDagilimKosulOl(yil, kurumsalYapiKod, asama, kurKod, ekoKod, ekoKodlar, ay);
            return KomutCalistir(sql, 0);
        }

        public Sonuc AylikDagilimSil(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, string[] ekoKodlar, int ay)
        {
            string sql = "DELETE FROM " + TABLOADTERTIP + " " + AylikDagilimKosulOl(yil, kurumsalYapiKod, asama, kurKod, ekoKod, ekoKodlar, ay);
            return KomutCalistir(sql, 0);
        }

        public Sonuc BirimAsamaSil(int hedefYil, ENUMMasrafPlanAsama hedefAsama, string kurumsalYapiKod, string kurKod, string ekoKod)
        {
            string sql = "DELETE FROM " + TABLOADTERTIP;

            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", hedefYil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)hedefAsama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kurumsalYapiKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", 0);
            return KomutCalistir(sql, 0);
        }

        public IDataReader MasrafPlaniBirimleriBul(int yil, int ayBasla, int ayBitis, string kurKod, string ekoKod1)
        {
            string sql = "SELECT DISTINCT " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + ", " + TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.SAYMANLIKKOD] + ", " + TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.BIRIMKOD];
            sql += " FROM " + TABLOADTERTIP + ", " + TABBHMKURUMSALYAPI.TABLOAD;
            sql += " WHERE " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + "=" + TABBHMKURUMSALYAPI.TABLOAD + "." + TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.KOD];
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            //sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.MERKEZ);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABBHMKURUMSALYAPI.TABLOAD, TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod1.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], ">=", ayBasla);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "<=", ayBitis);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEKGONDERME], "=", 0);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "=", 5);
            return this.veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleTertipOG(int yil, string kurumsalYapiKod, string kurKod, string ekoKod1, int ayBasla, int ayBitis)
        {
            string sql = "SELECT " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD] + ", SUM(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ")";
            sql += " FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            //sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.MERKEZ);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", kurumsalYapiKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod1.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], ">=", ayBasla);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "<=", ayBitis);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEKGONDERME], "=", 0);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "=", 5);
            sql += " GROUP BY " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD];
            sql += " ORDER BY " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD];
            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc OdemeEmriOlusturulduYap(int yil, string kurumsalYapiKod, string kurKod, string ekoKod1, int ayBasla, int ayBitis)
        {
            string sql = " UPDATE " + TABLOADTERTIP;
            sql += " SET " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEKGONDERME] + " = 1 ";
            sql += " WHERE " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            //sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.MERKEZ);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", kurumsalYapiKod.Replace(".", ""));
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod1.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], ">=", ayBasla);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "<=", ayBitis);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEKGONDERME], "=", 0);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "=", 5);
            return KomutCalistir(sql, 0);
        }

        public IDataReader AylikDagilimBul(int yil, string kurumsalYapiKod, ENUMMasrafPlanAsama asama, string kurKod, string ekoKod, string[] ekoKodlar)
        {
            string sql = "SELECT AY,SUM(ODENEK) FROM " + TABLOADTERTIP + " " + AylikDagilimKosulOl(yil, kurumsalYapiKod, asama, kurKod, ekoKod, ekoKodlar, -1) + " GROUP BY AY";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleAyOdenek(ENUMMasrafPlanAsama asama, Tertip kistasTertip, int revizeNo, int ustIslemNo, bool onaysiz)
        {
            string sql = "SELECT AY, SUM(ODENEK) FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", kistasTertip.yil);
            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
            {
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.AHP);

                if (onaysiz)
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], ">=", (int)0));
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "<", (int)6));
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "=", (int)5));

                if (ustIslemNo > 0)
                    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "<", ustIslemNo);
                if (revizeNo >= 0)
                    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", revizeNo);
            }
            else
            {
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", 0);
            }
            if (kistasTertip.kodlar != null)
            {
                if (kistasTertip.kodlar.Length > 0 && !string.IsNullOrEmpty(kistasTertip.kodlar[0]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kistasTertip.kodlar[0].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 1 && !string.IsNullOrEmpty(kistasTertip.kodlar[1]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", kistasTertip.kodlar[1].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 2 && !string.IsNullOrEmpty(kistasTertip.kodlar[2]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", kistasTertip.kodlar[2].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 3 && !string.IsNullOrEmpty(kistasTertip.kodlar[3]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", kistasTertip.kodlar[3].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 4 && !string.IsNullOrEmpty(kistasTertip.kodlar[4]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], "LIKE", kistasTertip.kodlar[4].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 5 && !string.IsNullOrEmpty(kistasTertip.kodlar[5]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kistasTertip.kodlar[5].Replace(".", "") + "%"));
            }
            sql += " GROUP BY AY ORDER BY AY";
            return this.veriErisim.DataReaderDondur(sql);
        }

        private static string TABLOADAHP = "ButceIslemleri.dbo.ISLAPGIDER";
        private enum ENUMAlanlarAHP
        {
            YIL = 0, ISLEM = 1, SIRANO = 2, KURKOD = 3, FONKOD = 4, FINKOD = 5, EKOKOD = 6, PEBKOD = 7, AY = 8, ODENEK = 9, FORMDURUM = 10
        }
        private static string[] ALANLARAHP = { "YIL", "ISLEM", "SIRANO", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "AY", "ODENEK", "FORMDURUM" };
        public IDataReader ListeleAHPOdenek(int asama, Tertip kistasTertip, int revizeNo)
        {
            string sql = "SELECT AY, SUM(ODENEK) FROM " + TABLOADAHP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.YIL], "=", kistasTertip.yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.ISLEM], "=", "0101");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.SIRANO], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.FORMDURUM], "=", 5);

            if (kistasTertip.kodlar != null)
            {
                if (kistasTertip.kodlar.Length > 0 && !string.IsNullOrEmpty(kistasTertip.kodlar[0]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.KURKOD], "LIKE", kistasTertip.kodlar[0].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 1 && !string.IsNullOrEmpty(kistasTertip.kodlar[1]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.FONKOD], "LIKE", kistasTertip.kodlar[1].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 2 && !string.IsNullOrEmpty(kistasTertip.kodlar[2]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.FINKOD], "LIKE", kistasTertip.kodlar[2].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 3 && !string.IsNullOrEmpty(kistasTertip.kodlar[3]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.EKOKOD], "LIKE", kistasTertip.kodlar[3].Replace(".", "") + "%"));
                if (kistasTertip.kodlar.Length > 4 && !string.IsNullOrEmpty(kistasTertip.kodlar[4]))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAHP[(int)ENUMAlanlarAHP.PEBKOD], "LIKE", kistasTertip.kodlar[4].Replace(".", "") + "%"));
            }
            sql += " GROUP BY AY ORDER BY AY";
            return this.veriErisim.DataReaderDondur(sql);
        }


        //public IDataReader TertipAylikDagilimBul(int yil, string kurumsalYapikod, string kur, string fon, string fin, string eko, string peb)
        //{
        //    string sql = "SELECT AY, SUM(ODENEK) FROM " + TABLOADTERTIP;
        //    sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", kurumsalYapikod.Replace(".", ""));
        //    //            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "=", kur.Replace(".", ""));
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "=", fon.Replace(".", ""));
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "=", fin.Replace(".", ""));
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "=", eko.Replace(".", ""));
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], "=", peb.Replace(".", ""));
        //    sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "<=", 5);

        //    sql += " GROUP BY AY ORDER BY AY";
        //    return this.veriErisim.DataReaderDondur(sql);
        //}

        public Sonuc RevizeDetaySil(int yil, int revizeNo)
        {
            string sql = "DELETE FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.AHP);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", revizeNo);

            return KomutCalistir(sql, 0);
        }

        public Sonuc RevizeFormSil(int yil, int revizeNo)
        {
            string sql = "DELETE FROM " + TABLOADREVIZEFORM;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.REVIZENO], "=", revizeNo);

            return KomutCalistir(sql, 1);
        }

        string Sorgu(int yil, ENUMMasrafPlanAsama asama, string kurKod, int ay, bool farkli, Tertip tertip)
        {
            string sql = $@"SELECT AHP.YIL, {(int)asama} AS ASAMA, AHP.KURKOD, AHP.FONKOD, AHP.FINKOD, AHP.EKOKOD, AHP.PEBKOD, '90000000000' KURUMSALYAPIKOD, AHP.AY, AHP.ODENEK-ISNULL(MPT.ODENEK, 0)  AS ODENEK
            ,0 AS ODENEKGONDERME
            ,1 AS DURUM
            ,0 AS REVIZENO, 1 AS GRUP, 1 AS TERTIPSIRA, 0 AS IOIISKOD
            FROM ISLAPGIDER AHP

            LEFT OUTER JOIN 

            (SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY, SUM(ODENEK) ODENEK 
             FROM MPGMASRAFPLANITERTIP WHERE YIL={yil} AND ASAMA = {(int)asama} AND REVIZENO=0 
             GROUP BY KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY) MPT 
             ON MPT.KURKOD = AHP.KURKOD AND MPT.FONKOD = AHP.FONKOD AND MPT.FINKOD = AHP.FINKOD AND MPT.EKOKOD = AHP.EKOKOD AND MPT.PEBKOD = AHP.PEBKOD AND MPT.AY=AHP.AY 

             where AHP.YIL={yil} AND KURUMKOD={VeriErisim.DegerYap(kurKod.Replace(".", ""))} AND AHP.ISLEM='0101' AND AHP.SIRANO=3 AND AHP.AY<={ay} AND AHP.ODENEK  {(farkli ? "<>" : "<")}  ISNULL(MPT.ODENEK, 0)";

            sql = VeriErisim.KosulEkle(sql, "AND", TertipKosulOl("AHP", "", "", tertip));
            return sql;
        }

        public Sonuc MerkezMerkezleriOlustur(int yil, ENUMMasrafPlanAsama asama, string kurKod, int ay, Tertip tertip)
        {
            string sql = "INSERT INTO MPGMASRAFPLANITERTIP " + Sorgu(yil, asama, kurKod, ay, true, tertip);
            return KomutCalistir(sql, 0);
        }

        public IDataReader OlusanlariKontrolEt(int yil, ENUMMasrafPlanAsama asama, string kurKod, int ay, Tertip tertip)
        {
            string sql = Sorgu(yil, asama, kurKod, ay, false, tertip);
            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DagitilmayacaklariSil(int yil, ENUMMasrafPlanAsama asama)
        {
            string sql = $"DELETE FROM MPGMASRAFPLANITERTIP where YIL={yil} AND ASAMA={(int)asama} AND KURUMSALYAPIKOD = '90000000000' AND REVIZENO=0";
            return KomutCalistir(sql, 0);
        }

        public int RevizeFormSiraNoBul(int yil)
        {
            string sql = "SELECT MAX(REVIZENO) FROM " + TABLOADREVIZEFORM;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.YIL], "=", yil);

            int donusDeger = veriErisim.TekKolonSQLInt(sql);
            if (veriErisim.hata != "")
                return 0;
            return donusDeger + 1;
        }

        public Sonuc RevizeFormEkle(MPRevizeForm form, string kullaniciKodu)
        {
            string sql = "INSERT INTO " + TABLOADREVIZEFORM + "(" + VeriErisim.TumAlanlar("", ALANLARREVIZEFORM) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(form.revizeNo);
            sql += "," + VeriErisim.DegerYap((int)form.formDurum);
            sql += "," + VeriErisim.DegerYap(kullaniciKodu);
            sql += "," + VeriErisim.DegerYap(new TNSDateTime(DateTime.Now));
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap((int)form.revizeFormTip);

            sql += "," + VeriErisim.DegerYap(form.evrakTarih);
            sql += "," + VeriErisim.DegerYap(form.evrakNo);
            sql += "," + VeriErisim.DegerYap(form.evrakIlgi);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.bumkoKurumKod);
            sql += "," + VeriErisim.DegerYap(form.bumkoIslemNo);
            sql += "," + VeriErisim.DegerYap(form.altIslemKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(form.yedekTertipStr);
            sql += "," + VeriErisim.DegerYap(form.ilgiliRevizeNo);
            sql += "," + VeriErisim.DegerYap(form.dayanakKodlar);
            sql += "," + VeriErisim.DegerYap(form.odGonYil);
            sql += "," + VeriErisim.DegerYap(form.odGonTur);
            sql += "," + VeriErisim.DegerYap(form.odGonNo);

            sql += "," + VeriErisim.DegerYap(form.projelerArasi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.sektorlerArasi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.bolgelerArasi ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(form.onayYaziNo);
            sql += "," + VeriErisim.DegerYap(form.onayYaziTarih);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc RevizeDetayOnayla(int yil, int revizeNo, ENUMRevizeFormDurum durum)
        {
            string sql = "UPDATE " + TABLOADTERTIP;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], (int)durum);
            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], yil);
            sql += " AND " + VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], (int)ENUMMasrafPlanAsama.AHP);
            sql += " AND " + VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], revizeNo);
            return KomutCalistir(sql, 0);
        }

        public Sonuc RevizeFormOnayla(int yil, int revizeNo, string kullaniciKodu, ENUMRevizeFormDurum durum)
        {
            string sql = "UPDATE " + TABLOADREVIZEFORM;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.ONAYLAYAN], kullaniciKodu);
            sql += ", " + VeriErisim.AtamaOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.ONAYTARIH], new TNSDateTime(DateTime.Now));
            sql += ", " + VeriErisim.AtamaOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.FORMDURUM], (int)durum);

            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.YIL], yil);
            sql += " AND " + VeriErisim.AtamaOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.REVIZENO], revizeNo);
            return KomutCalistir(sql, 1);
        }

        public IDataReader RevizeFormOku(int yil, int revizeNo, int odeGonYil, int odeGonTur, string odeGonNo)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARREVIZEFORM) + " FROM " + TABLOADREVIZEFORM;
            if (odeGonYil > 0)
            {
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.ODEGONYIL], "=", odeGonYil);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.ODEGONTUR], "=", odeGonTur);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.ODEGONNO], "=", odeGonNo);
            }
            else
            {
                sql += " WHERE " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.YIL], "=", yil);
                sql += " AND " + VeriErisim.KosulOl("", ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.REVIZENO], "=", revizeNo);
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader PEBBilgiBul(int yil, string pebKod)
        {
            string bolgeSql = ", (SELECT DISTINCT BOLGEKOD FROM Yatirim.dbo.YATPROJEKALKINMABOLGE KB where KB.YIL=ALTP.YIL AND KB.SENARYOKOD=ALTP.SENARYOKOD AND KB.ALTPROJENO=ALTP.ALTPROJENO FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string sql = "SELECT PRJ.DPTPROJENO, ALTP.ALTPROJENO, PRJ.SEKTORNO, SEK.AD" + bolgeSql + " FROM Yatirim.dbo.YATBUTCEKAYNAKPEBKOD KAYPEB";
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAK KAY ON " + VeriErisim.Join("KAY", "KAYPEB", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATALTPROJE ALTP ON " + VeriErisim.Join("ALTP", "KAY", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATPROJE PRJ ON " + VeriErisim.Join("PRJ", "ALTP", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATSEKTOR SEK ON " + VeriErisim.Join("SEK", "PRJ", new string[] { "YIL", "SEKTORNO" });
            sql += " WHERE " + VeriErisim.KosulOl("KAYPEB", "YIL", "=", yil);
            sql += " AND " + VeriErisim.KosulOl("KAYPEB", "SENARYOKOD", "=", "0600");
            sql += " AND " + VeriErisim.KosulOl("KAYPEB", "PEBKOD", "=", pebKod.Replace(".", ""));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader RevizeDetayOku(int yil, int revizeNo)
        {
            string bolgeSql = ", (SELECT DISTINCT BOLGEKOD FROM Yatirim.dbo.YATPROJEKALKINMABOLGE KB where KB.YIL=ALTP.YIL AND KB.SENARYOKOD=ALTP.SENARYOKOD AND KB.ALTPROJENO=ALTP.ALTPROJENO FOR XML RAW, ELEMENTS xsinil, TYPE)";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTERTIP, ALANLARTERTIP) + ", PRJ.DPTPROJENO, ALTP.ALTPROJENO, PRJ.SEKTORNO, SEK.AD" + bolgeSql + ", IOIP.IOIPROJEADI, ALTP.AD FROM " + TABLOADTERTIP;
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAKPEBKOD KAYPEB ON " + VeriErisim.Join("KAYPEB", TABLOADTERTIP, new string[] { "YIL", "PEBKOD" }) + " AND KAYPEB.SENARYOKOD='0600'";
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATBUTCEKAYNAK KAY ON " + VeriErisim.Join("KAY", "KAYPEB", new string[] { "YIL", "SENARYOKOD", "KAYNAKNO" });
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATALTPROJE ALTP ON " + VeriErisim.Join("ALTP", "KAY", new string[] { "YIL", "SENARYOKOD", "ALTPROJENO" });
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATPROJE PRJ ON " + VeriErisim.Join("PRJ", "ALTP", new string[] { "YIL", "SENARYOKOD", "PROJENO" });
            sql += " LEFT OUTER JOIN Yatirim.dbo.YATSEKTOR SEK ON " + VeriErisim.Join("SEK", "PRJ", new string[] { "YIL", "SEKTORNO" });
            sql += " LEFT OUTER JOIN ButceHazirlikv2.dbo.BHMV2IOIPROJE IOIP ON IOIP.SIRANO = " + TABLOADTERTIP + ".IOIISKOD";
            sql += " WHERE " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.AHP);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", revizeNo);

            sql += " ORDER BY " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.GRUP] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.TERTIPSIRA] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + ", " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(Tertip kistasTertip, string[] kurumsalYapiKodlar)
        {
            string groupByAlanlar = " ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD];
            string sql = "SELECT " + groupByAlanlar + ", SUM(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ") FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", kistasTertip.yil);
            //sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.MERKEZ);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "=", 5);
            if (kurumsalYapiKodlar != null)
            {
                string icKosul = "";
                for (int i = 0; i < kurumsalYapiKodlar.Length; i++)
                {
                    if (!string.IsNullOrEmpty(icKosul)) icKosul += " OR ";
                    icKosul += VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", kurumsalYapiKodlar[i]);
                }
                if (!string.IsNullOrEmpty(icKosul)) sql += " AND (" + icKosul + ")";
            }
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], kistasTertip, 0);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], kistasTertip, 1);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], kistasTertip, 2);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], kistasTertip, 3);
            sql += TABBHMGENEL.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], kistasTertip, 4);

            sql += " GROUP BY " + groupByAlanlar;
            sql += " ORDER BY " + groupByAlanlar;
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AHPKatsayiUygula(int yil, ENUMMasrafPlanAsama asama, string[] tertipStr, bool dagitilanMi, int ay, double bolunen, double bolen)
        {
            string sql = "UPDATE " + TABLOADTERTIP;
            sql += " SET " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + " = ROUND(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + "*" + VeriErisim.DegerYap(bolunen) + "/" + VeriErisim.DegerYap(bolen) + ", 0) ";
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "=", ay);
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], tertipStr[0], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], tertipStr[1], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], tertipStr[2], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], tertipStr[3], " AND ");
            if (dagitilanMi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "<", "9"));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", "9%"));
            return KomutCalistir(sql, 0);
        }

        public IDataReader AHPEnYuksekSatirBul(int yil, ENUMMasrafPlanAsama asama, string[] tertipStr, bool dagitilanMi, int ay)
        {
            string sql = "SELECT " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + ", " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + " FROM " + TABLOADTERTIP + " DIS ";
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "=", ay);
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], tertipStr[0], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], tertipStr[1], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], tertipStr[2], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], tertipStr[3], " AND ");
            if (dagitilanMi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "<", "9"));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", "9%"));
            sql += " AND " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + " = (SELECT MAX(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ") FROM " + TABLOADTERTIP + " IC ";
            sql += " WHERE " + "IC." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL] + "=" + "DIS." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL];
            sql += " AND " + "IC." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA] + "=" + "DIS." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA];
            sql += " AND " + "IC." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO] + "=" + "DIS." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO];
            sql += " AND " + "IC." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY] + "=" + "DIS." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY];
            sql += " AND " + "IC." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + "=" + "DIS." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD];
            sql += " AND " + "IC." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + "=" + "DIS." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD];
            sql += " AND " + "IC." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + "=" + "DIS." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD];
            sql += VeriErisim.AlanKosulOl("IC", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], tertipStr[3], " AND ");
            if (dagitilanMi)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("IC", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "<", "9"));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("IC", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", "9%"));
            sql += ")";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc DegerGuncelle(int yil, ENUMMasrafPlanAsama asama, string kur, string fon, string fin, string eko, string peb, string kurumsalYapiKod, int ay, int revizeNo, double odenek)
        {
            string sql = "UPDATE " + TABLOADTERTIP;
            sql += " SET " + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + " = " + odenek.ToString();
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], kur, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], fon, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], fin, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], eko, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], peb, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], kurumsalYapiKod, " AND ");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "=", ay);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", revizeNo);
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleBirim(int yil, string kurKod, string fonKod, string finKod, string ekoKod, string pebKod)
        {
            string groupByAlanlar = " ";
            groupByAlanlar += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + ", ";
            groupByAlanlar += TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.SAYMANLIKKOD] + ", ";
            groupByAlanlar += TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.BIRIMKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY];

            string sql = "SELECT " + groupByAlanlar + ", " + "SUM(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ") ";
            sql += " FROM " + TABLOADTERTIP + ", " + TABBHMKURUMSALYAPI.TABLOAD;
            sql += " WHERE " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD] + "=" + TABBHMKURUMSALYAPI.TABLOAD + "." + TABBHMKURUMSALYAPI.ALANLAR[(int)TABBHMKURUMSALYAPI.ENUMAlanlar.KOD];
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            //sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.MERKEZ);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", fonKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", finKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], "LIKE", pebKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "<", 6);
            sql += " GROUP BY " + groupByAlanlar;
            sql += " ORDER BY " + groupByAlanlar;
            return this.veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleBirimTertip(int yil, string kurumsalYapi, string kurKod, string fonKod, string finKod, string ekoKod, string pebKod, int ekoUz)
        {
            string groupByAlanlar = " ";
            groupByAlanlar += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD] + ", ";
            groupByAlanlar += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD] + ", ";
            groupByAlanlar += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD] + ", ";

            groupByAlanlar += BKD.Arac.EkoAlanOl(yil, "", TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], 2, true, VeriErisim.IslecBul("SUBSTRING")) + ",";

            //groupByAlanlar += $"SUBSTRING" + "(" + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD] + $", 1, {ekoUz}), ";
            groupByAlanlar += TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD] + ", ";
            groupByAlanlar += ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY];

            string sql = "SELECT " + groupByAlanlar + ", " + "SUM(" + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ") ";
            sql += " FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", kurumsalYapi.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            //sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.MERKEZ);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", kurKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", fonKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", finKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], "LIKE", pebKod.Replace(".", "") + "%");
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "<", 6);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK], "<>", 0.0);
            sql += " GROUP BY " + groupByAlanlar;
            sql += " ORDER BY " + groupByAlanlar;
            return this.veriErisim.DataReaderDondur(sql);
        }

        public static string FormKosulOl(MPRevizeFormSorgu form, string tabloAd, bool detayaBak, string JoinToDisTabloAd)
        {
            string sql = VeriErisim.KosulOl(tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.YIL], "=", form.yil);
            if (form.revizeFormTip != ENUMRevizeFormTip.Hepsi)
            {
                if (form.revizeFormTip == ENUMRevizeFormTip.AktarmaEklemeler)
                    sql += " AND " + tabloAd + "." + ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.BELGETUR] + " IN  (" + ((int)ENUMRevizeFormTip.MasrafPlaniAktarma).ToString() + "," + ((int)ENUMRevizeFormTip.Aktarma).ToString() + "," + ((int)ENUMRevizeFormTip.Ekleme).ToString() + "," + ((int)ENUMRevizeFormTip.KurumAktarma).ToString() + ")";
                else if (form.revizeFormTip == ENUMRevizeFormTip.BumkoIslemleri)
                    sql += " AND " + tabloAd + "." + ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.BELGETUR] + " IN  (" + ((int)ENUMRevizeFormTip.Aktarma).ToString() + "," + ((int)ENUMRevizeFormTip.Ekleme).ToString() + "," + ((int)ENUMRevizeFormTip.AHPRevize).ToString() + "," + ((int)ENUMRevizeFormTip.KurumAktarma).ToString() + ")";
                else if (form.revizeFormTip == ENUMRevizeFormTip.MasrafPlaniIslemleri)
                    sql += " AND " + tabloAd + "." + ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.BELGETUR] + " IN  (" + ((int)ENUMRevizeFormTip.MasrafPlaniAktarma).ToString() + "," + ((int)ENUMRevizeFormTip.MasrafPlaniRevize).ToString() + ")";
                else
                    sql += " AND " + VeriErisim.KosulOl(tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.BELGETUR], "=", (int)form.revizeFormTip);
            }
            sql += AralikKosulOlTarih(" AND ", tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.KAYITTARIH], form.kayitTarihiAralik);
            sql += AralikKosulOlTarih(" AND ", tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.ONAYTARIH], form.onaylamaTarihiAralik);
            string tut = AralikKosulOlInt("", tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.REVIZENO], form.revizeNoAralik);
            if (string.IsNullOrEmpty(tut))
                sql += " AND " + VeriErisim.KosulOl(tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.REVIZENO], ">", 0);
            else
                sql += " AND " + tut;
            sql += ListeVeyaOlInt(" AND ", tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.FORMDURUM], form.formDurumAralik);
            if (!string.IsNullOrEmpty(form.kaydedenKullaniciKodu))
                sql += " AND " + VeriErisim.KosulOl(tabloAd, ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.KAYDEDEN], "=", form.kaydedenKullaniciKodu);
            if (detayaBak)
            {
                string icSql = "";
                if (form.tertip.kodlar.Length > 0 && !string.IsNullOrEmpty(form.tertip.kodlar[0]))
                    icSql = " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], "LIKE", form.tertip.kodlar[0].Replace(".", "") + "%");
                if (form.tertip.kodlar.Length > 1 && !string.IsNullOrEmpty(form.tertip.kodlar[1]))
                    icSql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], "LIKE", form.tertip.kodlar[1].Replace(".", "") + "%");
                if (form.tertip.kodlar.Length > 2 && !string.IsNullOrEmpty(form.tertip.kodlar[2]))
                    icSql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], "LIKE", form.tertip.kodlar[2].Replace(".", "") + "%");
                if (form.tertip.kodlar.Length > 3 && !string.IsNullOrEmpty(form.tertip.kodlar[3]))
                    icSql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], "LIKE", form.tertip.kodlar[3].Replace(".", "") + "%");
                if (form.tertip.kodlar.Length > 4 && !string.IsNullOrEmpty(form.tertip.kodlar[4]))
                    icSql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], "LIKE", form.tertip.kodlar[4].Replace(".", "") + "%");
                if (form.tertip.kodlar.Length > 5 && !string.IsNullOrEmpty(form.tertip.kodlar[5]))
                    icSql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "LIKE", form.tertip.kodlar[5].Replace(".", "") + "%");

                if (icSql != "")
                {
                    sql += " AND 1 IN (";
                    sql += "SELECT 1 FROM " + TABLOADTERTIP;
                    sql += " WHERE " + tabloAd + "." + ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.YIL] + " = " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL];
                    sql += " AND " + tabloAd + "." + ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.REVIZENO] + " = " + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO];
                    sql += icSql;
                    sql += ") ";
                }

                if (!string.IsNullOrWhiteSpace(JoinToDisTabloAd))
                {
                    sql += " AND " + VeriErisim.Join(tabloAd, JoinToDisTabloAd, new string[] { "YIL", "REVIZENO" });
                }
            }
            return sql;
        }

        public IDataReader RevizeFormListele(MPRevizeFormSorgu form)
        {
            string sql = "";

            sql = "SELECT DISTINCT " + VeriErisim.TumAlanlar(TABLOADREVIZEFORM, ALANLARREVIZEFORM) + " FROM " + TABLOADREVIZEFORM;
            sql += " WHERE " + FormKosulOl(form, TABLOADREVIZEFORM, true, null);


            sql += " ORDER BY " + TABLOADREVIZEFORM + "." + ALANLARREVIZEFORM[(int)ENUMAlanlarREVIZEFORM.REVIZENO];
            //OrtakFonksiyonlar.HataDosyaYaz("MPDebug.txt", sql, false);
            return veriErisim.DataReaderDondur(sql);
        }

        public double RevizeFormToplamBul(int yil, int revizeNo)
        {
            string sql = "";

            sql = "SELECT SUM(" + TABLOADTERTIP + "." + ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK] + ")" + " FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", revizeNo);
            sql += " AND " + VeriErisim.KosulOl(TABLOADTERTIP, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ODENEK], ">", 0);
            return veriErisim.TekKolonSQLDbl(sql);
        }

        private static string ListeVeyaOlInt(string onEk, string tabloAd, string alanAd, string kosulAralik)
        {
            if (string.IsNullOrEmpty(kosulAralik) || kosulAralik.Equals(";"))
                return "";
            string[] kosulDizi = kosulAralik.Split(';');
            string tut = "";
            for (int i = 0; i < kosulDizi.Length; i++)
            {
                if (!string.IsNullOrEmpty(tut)) tut += " OR ";
                tut += VeriErisim.KosulOl(tabloAd, alanAd, "=", OrtakFonksiyonlar.ConvertToInt(kosulDizi[i], 0));

            }
            if (!string.IsNullOrEmpty(tut))
                return onEk + "(" + tut + ")";
            else
                return "";
        }

        private static string AralikKosulOlInt(string onEk, string tabloAd, string alanAd, string kosulAralik)
        {
            if (string.IsNullOrEmpty(kosulAralik) || kosulAralik.Equals(";"))
                return "";
            string[] kosulDizi = kosulAralik.Split(';');
            string tut = "";
            if (kosulDizi.Length >= 1)
                tut = VeriErisim.KosulOl(tabloAd, alanAd, ">=", OrtakFonksiyonlar.ConvertToInt(kosulDizi[0], 0));
            if (kosulDizi.Length >= 2)
                tut += " AND " + VeriErisim.KosulOl(tabloAd, alanAd, "<=", OrtakFonksiyonlar.ConvertToInt(kosulDizi[1], 0));
            return onEk + tut;
        }

        private static string AralikKosulOlTarih(string onEk, string tabloAd, string alanAd, string kosulAralik)
        {
            if (string.IsNullOrEmpty(kosulAralik) || kosulAralik.Equals(";"))
                return "";
            string[] kosulDizi = kosulAralik.Split(';');
            string tut = "";
            if (kosulDizi.Length >= 1)
                tut = VeriErisim.KosulOl(tabloAd, alanAd, ">=", new TNSDateTime(kosulDizi[0]));
            if (kosulDizi.Length >= 1)
                tut += " AND " + VeriErisim.KosulOl(tabloAd, alanAd, "<=", new TNSDateTime(kosulDizi[1]));
            return onEk + tut;
        }

        public IDataReader ListeleTertipAy(int yil, string kur, string fon, string fin, string eko, string peb, string kurumsalYapiKod, string formDurumAralikDizi, int revizeTip)
        {
            string sql = "SELECT KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY, SUM(ODENEK) FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);

            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], kur, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], fon, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], fin, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], eko, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], peb, " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], kurumsalYapiKod, " AND ");

            sql += ListeVeyaOlInt(" AND ", "", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], formDurumAralikDizi);
            if (revizeTip == 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", 0);
            else if (revizeTip > 0)
                sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], ">", 0);

            sql += " GROUP BY KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, AY ORDER BY 1,2,3,4,5";
            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc MPSil(int yil, ENUMMasrafPlanAsama asama, Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);

            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], tertip.kodlar[0], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], tertip.kodlar[1], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], tertip.kodlar[2], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], tertip.kodlar[3], " AND ");
            sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], tertip.kodlar[4], " AND ");
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], "=", tertip.kodlar[5].Replace(".", ""));

            return KomutCalistir(sql, 0);
        }

        private string TertipKosulOl(string tabloAd, string kurumsalYapiAlanAd, string kurumsalYapiKod, Tertip tertip)
        {
            string sqlKosul = "";
            if (!string.IsNullOrWhiteSpace(kurumsalYapiKod))
                sqlKosul = VeriErisim.KosulEkle(sqlKosul, " AND ", VeriErisim.KosulOl(tabloAd, kurumsalYapiAlanAd, "LIKE", kurumsalYapiKod.Replace(".", "") + "%"));
            if (tertip != null)
            {
                sqlKosul = VeriErisim.KosulEkle(sqlKosul, " AND ", VeriErisim.AlanKosulOl(tabloAd, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], tertip.kodlar[0], ""));
                sqlKosul = VeriErisim.KosulEkle(sqlKosul, " AND ", VeriErisim.AlanKosulOl(tabloAd, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], tertip.kodlar[1], ""));
                sqlKosul = VeriErisim.KosulEkle(sqlKosul, " AND ", VeriErisim.AlanKosulOl(tabloAd, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], tertip.kodlar[2], ""));
                sqlKosul = VeriErisim.KosulEkle(sqlKosul, " AND ", VeriErisim.AlanKosulOl(tabloAd, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], tertip.kodlar[3], ""));
                sqlKosul = VeriErisim.KosulEkle(sqlKosul, " AND ", VeriErisim.AlanKosulOl(tabloAd, ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], tertip.kodlar[4], ""));
            }
            return sqlKosul;
        }

        public Sonuc MPSil(int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, Tertip tertip)
        {
            string sql = "DELETE FROM " + TABLOADTERTIP;
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", 0);

            sql = VeriErisim.KosulEkle(sql, "AND", TertipKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], kurumsalYapiKod, tertip));

            return KomutCalistir(sql, 0);
        }

        public Sonuc MPUretAHP(int yil, ENUMMasrafPlanAsama asama, string kurumsalYapiKod, int ay, Tertip tertip, string odenekAsama)
        {
            BKD.ButceKodYapi bky = BKD.Arac.Tanimla().ButceKodBilgiBul(yil, 4, string.Empty);

            string ekoAlan = TNS.BKD.Arac.Eko23AlanOl(yil, bky, "", "EKOKOD");

            string sql = "INSERT INTO " + TABLOADTERTIP + " SELECT AHP.YIL";
            sql += "," + ((int)asama).ToString() + " AS ASAMA";
            sql += ",AHP.KURKOD, AHP.FONKOD, AHP.FINKOD, AHP.EKOKOD, AHP.PEBKOD, TAVAN.KYKOD AS KURUMSALYAPIKOD, AY";
            sql += ",FLOOR(TAVAN.ODENEK * (AHP.ODENEK*1.0 / TOPLAMAHPODENEK)) AS ODENEK";
            sql += ",0 AS ODENEKGONDERME, 1 AS DURUM, 0 AS REVIZENO, 1 AS GRUP, 1 AS TERTIPSIRA, 0 AS IOIISKOD";
            sql += " FROM ButceIslemleri.dbo.ISLAPGIDER AS AHP, ";
            sql += $" (SELECT KURKOD, FONKOD, FINKOD,  {ekoAlan} EKOKOD, PEBKOD, CONVERT(MONEY, SUM(ODENEK)) AS TOPLAMAHPODENEK";
            sql += " FROM ButceHazirlikv2.dbo.BHMODENEK ";
            sql += " where YIL=" + yil.ToString() + " AND SENARYOKOD='" + odenekAsama.Replace(".", "") + "' AND TEKLIFYIL=0";
            sql += $" group by KURKOD, FONKOD, FINKOD, {ekoAlan}, PEBKOD) AS TOPLAMAHP,";
            sql += " ButceHazirlikV2.dbo.MPTAVAN AS TAVAN";
            sql += " where AHP.YIL=" + yil.ToString() + " AND AHP.ISLEM='0101' AND AHP.SIRANO=3 AND AHP.AY <= " + ay.ToString();
            sql += " AND TOPLAMAHP.KURKOD = AHP.KURKOD";
            sql += " AND TOPLAMAHP.FONKOD = AHP.FONKOD";
            sql += " AND TOPLAMAHP.FINKOD = AHP.FINKOD";
            sql += " AND TOPLAMAHP.EKOKOD = AHP.EKOKOD";
            sql += " AND TOPLAMAHP.PEBKOD = AHP.PEBKOD";
            sql += " AND TAVAN.YIL = AHP.YIL";
            sql += " AND TAVAN.DETAYMI = 1";
            sql += " AND TAVAN.KURKOD = AHP.KURKOD";
            sql += " AND TAVAN.FONKOD = AHP.FONKOD";
            sql += " AND TAVAN.FINKOD = AHP.FINKOD";
            sql += " AND TAVAN.EKOKOD = AHP.EKOKOD";
            sql += " AND TAVAN.PEBKOD = AHP.PEBKOD";

            sql = VeriErisim.KosulEkle(sql, "AND", TertipKosulOl("TAVAN", "KYKOD", kurumsalYapiKod, tertip));
            OrtakFonksiyonlar.HataDosyaYaz("sqlkomut.txt", sql, true);
            return KomutCalistir(sql, 0);
        }

        public Sonuc TertipDurumDegistir(int yil, ENUMMasrafPlanAsama asama, int durum, int revizeNo, bool buAsama)
        {
            string sql = "UPDATE " + TABLOADTERTIP;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], (int)durum);
            sql += " WHERE " + VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], yil);
            string islec = "=";
            if (!buAsama) islec = "<>";
            sql += " AND " + VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], islec, (int)asama);
            sql += " AND " + VeriErisim.AtamaOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], revizeNo);
            return KomutCalistir(sql, 0, 2000);
        }

        public IDataReader ListeleRapor(string alanIsimleri, Tertip kistasTertip, ENUMMasrafPlanAsama asama, int kistasAy, int haricRevizeNo)
        {
            string sql = "SELECT " + alanIsimleri;
            if (!string.IsNullOrEmpty(alanIsimleri))
                sql += ", ";
            sql += "SUM(ODENEK) FROM " + TABLOADTERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.YIL], "=", kistasTertip.yil));
            if (asama == ENUMMasrafPlanAsama.SONDURUMONAYLI)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)ENUMMasrafPlanAsama.AHP));
                sql = VeriErisim.KosulEkle(sql, "AND", "((ODENEK < 0 AND DURUM < 6) OR (ODENEK > 0 AND DURUM = 5))");
                //if (onaysiz)
                //{
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "<", (int)6));
                //}
                //else
                //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.DURUM], "=", (int)5));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)asama));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "=", (int)0));
            }

            if (kistasAy > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.AY], "<=", kistasAy));
            if (haricRevizeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.REVIZENO], "!=", haricRevizeNo));

            if (!string.IsNullOrEmpty(kistasTertip.kodlar[0]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], kistasTertip.kodlar[0], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[1]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], kistasTertip.kodlar[1], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[2]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], kistasTertip.kodlar[2], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[3]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], kistasTertip.kodlar[3], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[4]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], kistasTertip.kodlar[4], " AND ");
            if (!string.IsNullOrEmpty(kistasTertip.kodlar[5]))
                sql += VeriErisim.AlanKosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURUMSALYAPIKOD], kistasTertip.kodlar[5], " AND ");
            if (!string.IsNullOrEmpty(alanIsimleri))
            {
                sql += " GROUP BY " + alanIsimleri;
                sql += " ORDER BY " + alanIsimleri;
            }
            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AHPdenMPye(int yil, ENUMMasrafPlanAsama asama, Tertip tertip)
        {
            string kurumsalYapiKod = tertip.kodlar[5].Replace(".", "");
            string sql = "INSERT INTO " + TABLOADTERTIP + " SELECT AHP.YIL";
            sql += "," + ((int)asama).ToString() + " AS ASAMA";
            sql += ",AHP.KURKOD";
            sql += ",AHP.FONKOD";
            sql += ",AHP.FINKOD";
            sql += ",AHP.EKOKOD";
            sql += ",AHP.PEBKOD";
            sql += ", " + VeriErisim.DegerYap(kurumsalYapiKod) + "AS KURUMSALYAPIKOD";
            sql += ",AHP.AY";
            sql += ",AHP.ODENEK";
            sql += ",0 AS ODENEKGONDERME";
            sql += ",1 AS DURUM";
            sql += ",0 AS REVIZENO, 1 AS GRUP, 1 AS TERTIPSIRA";
            sql += " FROM ButceIslemleri.dbo.ISLAPGIDER AS AHP INNER JOIN ";
            sql += "  ButceHazirlikV2.dbo.MPTAVAN AS TAVAN ON AHP.YIL = TAVAN.YIL AND AHP.KURKOD = TAVAN.KURKOD AND AHP.FONKOD = TAVAN.FONKOD AND AHP.FINKOD = TAVAN.FINKOD AND ";
            sql += "  AHP.EKOKOD = TAVAN.EKOKOD AND AHP.PEBKOD = TAVAN.PEBKOD AND TAVAN.ODENEK > 0  AND TAVAN.DETAYMI=1 AND TAVAN.KYKOD='" + kurumsalYapiKod + "' ";

            sql += " where AHP.YIL=" + yil.ToString() + " AND AHP.ISLEM='0101' AND AHP.SIRANO=3 ";

            sql += VeriErisim.AlanKosulOl("AHP", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.KURKOD], tertip.kodlar[0], " AND ");
            sql += VeriErisim.AlanKosulOl("AHP", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FONKOD], tertip.kodlar[1], " AND ");
            sql += VeriErisim.AlanKosulOl("AHP", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.FINKOD], tertip.kodlar[2], " AND ");
            sql += VeriErisim.AlanKosulOl("AHP", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.EKOKOD], tertip.kodlar[3], " AND ");
            sql += VeriErisim.AlanKosulOl("AHP", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.PEBKOD], tertip.kodlar[4], " AND ");

            return KomutCalistir(sql, 0);
        }

        public Sonuc MPdenMPye(int yil, ENUMMasrafPlanAsama kaynakAsama, ENUMMasrafPlanAsama asama)
        {
            string sql = "INSERT INTO " + TABLOADTERTIP + " SELECT YIL";
            sql += "," + ((int)asama).ToString() + " AS ASAMA";
            sql += ",KURKOD";
            sql += ",FONKOD";
            sql += ",FINKOD";
            sql += ",EKOKOD";
            sql += ",PEBKOD";
            sql += ",KURUMSALYAPIKOD";
            sql += ",AY";
            sql += ",ODENEK";
            sql += ",ODENEKGONDERME";
            sql += ",DURUM";
            sql += ",REVIZENO, GRUP, TERTIPSIRA";
            sql += " FROM " + TABLOADTERTIP;
            sql += " where YIL=" + yil.ToString();
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARTERTIP[(int)ENUMAlanlarTERTIP.ASAMA], "=", (int)kaynakAsama));
            return KomutCalistir(sql, 0);
        }

    }
}
