using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABSeyyarGorev : TABOrtak
    {
        public TABSeyyarGorev(VeriErisim veriErisim) : base(veriErisim) { }
        public static string TABLOADPLANLANAN = "HRCSEYYARGOREVPLANLANAN";
        public enum ENUMAlanlarPlanlanan
        {

            YIL = 0,
            HARCAMABIRIMI = 1,
            ASAMA = 2,
            ISTIHDAMSEKLI = 3,
            GUNSAYISI = 4,
            BELGENO = 5
        }
        public static string[] ALANLARPLANLANAN = { "YIL", "HARCAMABIRIMI", "ASAMA", "ISTIHDAMSEKLI", "GUNSAYISI", "BELGENO" };

        public static string TABLOADGERCEKLESEN = "HRCSEYYARGOREVGERCEKLESEN";
        public enum ENUMAlanlarGerceklesen
        {

            YIL = 0,
            HARCAMABIRIMI = 1,
            ISTIHDAMSEKLI = 2,
            GUNSAYISI = 3
        }
        public static string[] ALANLARGERCEKLESEN = { "YIL", "HARCAMABIRIMI", "ISTIHDAMSEKLI", "GUNSAYISI" };

        public static string TABLOADONAY = "HRCSEYYARGOREVONAY";
        public enum ENUMAlanlarOnay
        {
            YIL = 0,
            HARCAMABIRIMI = 1,
            ASAMA = 2
        }
        public static string[] ALANLARONAY = { "YIL", "HARCAMABIRIMI", "ASAMA" };

        public static string TABLOADUNVANBILGI = "HRCSEYYARGOREVUNVANBILGISI";
        public enum ENUMAlanlarUnvanBilgi
        {
            YIL = 0,
            HARCAMABIRIMI = 1,
            ASAMA = 2,
            ISTIHDAMSEKLI = 3,
            UNVANKODU = 4,
            ONCEKIYILVIZECETVELIDURUMU = 5,
            SIMDIKIYILSEYYARGOREVDURUMU = 6
        }
        public static string[] ALANLARUNVANBILGI = { "YIL", "HARCAMABIRIMI", "ASAMA", "ISTIHDAMSEKLI", "UNVANKODU", "ONCEKIYILVIZECETVELIDURUMU", "SIMDIKIYILSEYYARGOREVDURUMU" };

        public Sonuc SeyyarGorevPlanlananGuncelle(string kullanici, SeyyarGorevI sg, SeyyarGorevDetay sgd)
        {
            string sql = "UPDATE " + TABLOADPLANLANAN + " SET ";
            sql = VeriErisim.AtamaOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.GUNSAYISI], sgd.Talep);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.YIL], "=", sg.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.HARCAMABIRIMI], "=", sg.harcamaBirimiKodu.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.ASAMA], "=", sg.asama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.ISTIHDAMSEKLI], "=", (int)sgd.istihtamSekli.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.BELGENO], "=", 0));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SeyyarGorevPlanlananKaydet(string kullanici, SeyyarGorevI sg, ENUMPersonelIstihdamSekli istihtamSekli_kod, int deger)
        {
            string sql = "INSERT INTO " + TABLOADPLANLANAN + " (" + VeriErisim.TumAlanlar("", ALANLARPLANLANAN) + ") VALUES (";
            sql += VeriErisim.DegerYap(sg.yil) + ",";
            sql += VeriErisim.DegerYap(sg.harcamaBirimiKodu.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sg.asama) + ",";
            sql += VeriErisim.DegerYap((int)istihtamSekli_kod) + ",";
            sql += VeriErisim.DegerYap(deger) + ",";
            sql += VeriErisim.DegerYap(0) + ")";
            return KomutCalistir(sql, 1);
        }

        private string ToplamAlanOl(int yil, ENUMPersonelIstihdamSekli ishSekli)
        {
            string str = $"(SELECT SUM(GUNSAYISI) FROM HRCSEYYARGOREVPLANLANAN";
            str += $" LEFT OUTER JOIN HRCSEYYARGOREVREVIZEFORM ON HRCSEYYARGOREVREVIZEFORM.YIL = HRCSEYYARGOREVPLANLANAN.YIL AND HRCSEYYARGOREVREVIZEFORM.BELGETUR = 2 AND HRCSEYYARGOREVREVIZEFORM.BELGENO = HRCSEYYARGOREVPLANLANAN.BELGENO";
            str += $" WHERE HRCSEYYARGOREVPLANLANAN.YIL = {yil} AND HARCAMABIRIMI = KY.KOD AND ISTIHDAMSEKLI = {(int)ishSekli} AND ASAMA = 3 AND (HRCSEYYARGOREVREVIZEFORM.DURUM = 5 OR HRCSEYYARGOREVREVIZEFORM.DURUM IS NULL))";
            return str;
        }

        private string JoinStr(string tabloAd, string tabloAlias, int yil, int asama, ENUMPersonelIstihdamSekli ishSekli)
        {
            string str = $" LEFT JOIN {tabloAd} {tabloAlias} ON {tabloAlias}.YIL = {yil} AND {tabloAlias}.HARCAMABIRIMI = KY.KOD AND {tabloAlias}.ISTIHDAMSEKLI = {(int)ishSekli} ";
            if (asama != -1)
            {
                str += $" AND {tabloAlias}.BELGENO = 0 AND {tabloAlias}.ASAMA = {asama} ";
            }
            return str;
        }

        public IDataReader SeyyarGorevPlanlananListeleDetayli(int yil, string harcamabirimi, int asama)
        {
            string alanP1;
            string alanP2;
            string alanP2s;

            if (asama != 4)
            {
                alanP1 = "P1.GUNSAYISI";
                alanP2 = "P2.GUNSAYISI";
                alanP2s = "P2s.GUNSAYISI";
            }
            else
            {
                alanP1 = ToplamAlanOl(yil, ENUMPersonelIstihdamSekli.MEMUR);
                alanP2 = ToplamAlanOl(yil, ENUMPersonelIstihdamSekli.ISCI);
                alanP2s = ToplamAlanOl(yil, ENUMPersonelIstihdamSekli.SOZLESMELI);
            }

            string sql = $"SELECT KY.KOD, KY.AD, KY.SAYMANLIKKOD, KY.BIRIMKOD, KY.KURKOD, {alanP1}, {alanP2}, P3.GUNSAYISI, P4.GUNSAYISI, P5.GUNSAYISI, P6.GUNSAYISI, P7.GUNSAYISI, P8.GUNSAYISI, {alanP2s}, P4s.GUNSAYISI, P6s.GUNSAYISI, P8s.GUNSAYISI FROM ButceIslemleri.dbo.BHMKURUMSALYAPI KY";

            if (asama != 4)
            {
                sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P1", yil, asama, ENUMPersonelIstihdamSekli.MEMUR);
                sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P2", yil, asama, ENUMPersonelIstihdamSekli.ISCI);
                sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P2s", yil, asama, ENUMPersonelIstihdamSekli.SOZLESMELI);
            }

            sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P3", yil, asama - 1, ENUMPersonelIstihdamSekli.MEMUR);
            sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P4", yil, asama - 1, ENUMPersonelIstihdamSekli.ISCI);
            sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P4s", yil, asama - 1, ENUMPersonelIstihdamSekli.SOZLESMELI);
            sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P5", yil - 1, 3, ENUMPersonelIstihdamSekli.MEMUR);
            sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P6", yil - 1, 3, ENUMPersonelIstihdamSekli.ISCI);
            sql += JoinStr("HRCSEYYARGOREVPLANLANAN", "P6s", yil - 1, 3, ENUMPersonelIstihdamSekli.SOZLESMELI);
            sql += JoinStr("HRCSEYYARGOREVGERCEKLESENSUREC", "P7", yil - 1, -1, ENUMPersonelIstihdamSekli.MEMUR);
            sql += JoinStr("HRCSEYYARGOREVGERCEKLESENSUREC", "P8", yil - 1, -1, ENUMPersonelIstihdamSekli.ISCI);
            sql += JoinStr("HRCSEYYARGOREVGERCEKLESENSUREC", "P8s", yil - 1, -1, ENUMPersonelIstihdamSekli.SOZLESMELI);

            string hrcKod = harcamabirimi.Replace(".", "");

            string ilKosul = VeriErisim.KosulOl("KY", "KOD", " LIKE ", hrcKod + "%");
            if (hrcKod.StartsWith("_06"))
            {
                string merkezKod = hrcKod.Replace("_06", "100");
                ilKosul = VeriErisim.KosulEkle(ilKosul, " OR ", VeriErisim.KosulOl("KY", "KOD", " LIKE ", merkezKod + "%"));
                merkezKod = hrcKod.Replace("_06", "300");
                ilKosul = VeriErisim.KosulEkle(ilKosul, " OR ", VeriErisim.KosulOl("KY", "KOD", " LIKE ", merkezKod + "%"));
            }

            sql = VeriErisim.KosulEkle(sql, " WHERE ", "(" + ilKosul + ")");
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", "LEN(KY.KOD)", " = ", 11));

            sql += " ORDER BY KY.KOD, KY.AD, KY.SAYMANLIKKOD, KY.BIRIMKOD, KY.KURKOD";
            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader SeyyarGorevPlanlananListele(int yil, string harcamabirimi, int asama)
        //{
        //    string sql = "SELECT " + ENUMAlanlarPlanlanan.ISTIHDAMSEKLI + ", ISNULL(SUM(" + ENUMAlanlarPlanlanan.GUNSAYISI + "), 0) AS " + ENUMAlanlarPlanlanan.GUNSAYISI;
        //    sql += " FROM " + TABLOADPLANLANAN;
        //    sql = VeriErisim.KosulEkle(sql, " WHERE ", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.HARCAMABIRIMI], " LIKE ", harcamabirimi.Replace(".", "") + "%"));
        //    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.ASAMA], "=", asama));
        //    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.BELGENO], "=", 0));
        //    sql += " AND " + ENUMAlanlarPlanlanan.ISTIHDAMSEKLI + " IN (1,4)";

        //    sql += " GROUP BY " + ENUMAlanlarPlanlanan.ISTIHDAMSEKLI;
        //    return veriErisim.DataReaderDondur(sql);
        //}

        public Sonuc SeyyarGorevPlanSil(SeyyarGorevI sg)
        {
            string sql = "DELETE FROM " + TABLOADPLANLANAN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.YIL], "=", sg.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.HARCAMABIRIMI], "=", sg.harcamaBirimiKodu.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.ASAMA], "=", sg.asama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.BELGENO], "=", 0));
            return KomutCalistir(sql, 0);
        }

        //public int SeyyarGorevPlanlananMiktarBul(int yil, string harcamaBirimi, int asama, int istihdamsekli)
        //{
        //    string sql = "";
        //    sql = "SELECT ISNULL(SUM(" + ENUMAlanlarPlanlanan.GUNSAYISI + "), 0) AS " + ENUMAlanlarPlanlanan.GUNSAYISI + " FROM " + TABLOADPLANLANAN + " P LEFT OUTER JOIN HRCSEYYARGOREVREVIZEFORM F ON " + VeriErisim.Join("P", "F", new string[] { "YIL", "BELGENO" });
        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("P", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.HARCAMABIRIMI], "LIKE", harcamaBirimi.Replace(".", "") + "%"));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.ASAMA], "=", asama));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", ALANLARPLANLANAN[(int)ENUMAlanlarPlanlanan.ISTIHDAMSEKLI], "=", istihdamsekli));
        //    sql = VeriErisim.KosulEkle(sql, "AND", "(P.BELGENO = 0 OR F.DURUM = 5)");
        //    return veriErisim.TekKolonSQLInt(sql);
        //}

        //public Sonuc SeyyarGorevGerceklesenGuncelle(string kullanici, SeyyarGorevI sg, SeyyarGorevDetay sgd)
        //{
        //    Sonuc sonuc;
        //    string sql = "UPDATE " + TABLOADGERCEKLESEN + " SET ";
        //    sql = VeriErisim.AtamaOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarPlanlanan.GUNSAYISI], sgd.Talep);

        //    sql = VeriErisim.KosulEkle(sql, " WHERE ", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.YIL], "=", sg.yil - 1));
        //    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.HARCAMABIRIMI], "=", sg.harcamaBirimiKodu.Replace(".", "")));
        //    sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.ISTIHDAMSEKLI], "=", (int)sgd.istihtamSekli.kod));
        //    sonuc = KomutCalistir(sql, 1);
        //    return sonuc;
        //}

        public Sonuc SeyyarGorevGerceklesenKaydet(string kullanici, SeyyarGorevI sg, ENUMPersonelIstihdamSekli ishSek, int kullanilan)
        {
            Sonuc sonuc;
            string sql = "INSERT INTO " + TABLOADGERCEKLESEN + " (" + VeriErisim.TumAlanlar("", ALANLARGERCEKLESEN) + ") VALUES (";
            sql += VeriErisim.DegerYap(sg.yil - 1) + ",";
            sql += VeriErisim.DegerYap(sg.harcamaBirimiKodu.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap((int)ishSek) + ",";
            sql += VeriErisim.DegerYap(kullanilan) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int SeyyarGorevGerceklesenMiktarBul(int yil, string harcamabirimi, int istihdamsekli)
        {
            string sql = "SELECT ISNULL( SUM(" + ENUMAlanlarGerceklesen.GUNSAYISI + "),0) AS " + ENUMAlanlarGerceklesen.GUNSAYISI + " FROM " + TABLOADGERCEKLESEN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.HARCAMABIRIMI], "LIKE", harcamabirimi.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.ISTIHDAMSEKLI], "=", istihdamsekli));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc SeyyarGorevGerceklesenSil(SeyyarGorevI sg)
        {
            string sql = "DELETE  FROM " + TABLOADGERCEKLESEN;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.YIL], "=", sg.yil - 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESEN[(int)ENUMAlanlarGerceklesen.HARCAMABIRIMI], "=", sg.harcamaBirimiKodu.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SeyyarGorevOnayKaydet(string kullanici, int yil, string harcamabirimi, int asama)
        {
            Sonuc sonuc;
            string sql = "INSERT INTO " + TABLOADONAY + " (" + VeriErisim.TumAlanlar("", ALANLARONAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(harcamabirimi.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(asama) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc SeyyarGorevOnaySil(VeriErisim ve, int yil, string harcamabirimi, int asama)
        {
            Sonuc sonuc;
            string sql = "";
            sql = "DELETE  FROM " + TABLOADONAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarOnay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarOnay.HARCAMABIRIMI], "=", harcamabirimi.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarOnay.ASAMA], "=", asama));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int SeyyarGorevOnayliVarMi(VeriErisim ve, int yil, string harcamabirimi, int asama, bool like = false)
        {
            string sql = "SELECT ISNULL(COUNT(" + ENUMAlanlarOnay.YIL + ") ,0) AS  ADET FROM " + TABLOADONAY;
            sql = VeriErisim.KosulEkle(sql, " WHERE ", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarOnay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarOnay.HARCAMABIRIMI], like ? "LIKE" : "=", harcamabirimi.Replace(".", "") + (like ? "%" : "")));
            sql = VeriErisim.KosulEkle(sql, " AND ", VeriErisim.KosulOl("", ALANLARONAY[(int)ENUMAlanlarOnay.ASAMA], "=", asama));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader UnvanListele(string birimkodu, int muhasebekodu)
        {
            string sql;
            string alanAd = "CASE when PERPERSONEL.ISTIHDAMSEKLI = 2 OR PERPERSONEL.ISTIHDAMSEKLI = 3 THEN 2 else PERPERSONEL.ISTIHDAMSEKLI END";
            sql = $" SELECT {alanAd}, PERPERSONEL.UNVANKODU, PERUNVAN.AD, COUNT(*) AS ADET ";
            sql += " FROM PERPERSONEL INNER JOIN ";
            sql += " PERUNVAN ON PERPERSONEL.UNVANKODU = PERUNVAN.KOD ";
            sql += $" WHERE (PERPERSONEL.BIRIMKODU = {VeriErisim.DegerYap(birimkodu)}) AND (PERPERSONEL.MUHASEBEKODU = {VeriErisim.DegerYap(muhasebekodu)}) AND (PERPERSONEL.ISTIHDAMSEKLI IN(1,2,4))";

            sql += $" GROUP BY {alanAd}, PERPERSONEL.UNVANKODU, PERUNVAN.AD ";
            sql += " ORDER BY 1, 3";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc UnvanBilgisiKaydet(SeyyarGorevI sg, SeyyarGorevUnvanBilgi sgub)
        {
            string sql = "INSERT INTO " + TABLOADUNVANBILGI + " (";
            sql += ENUMAlanlarUnvanBilgi.YIL + ",";
            sql += ENUMAlanlarUnvanBilgi.HARCAMABIRIMI + ",";
            sql += ENUMAlanlarUnvanBilgi.ASAMA + ",";
            sql += ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI + ",";
            sql += ENUMAlanlarUnvanBilgi.UNVANKODU + ",";
            sql += ENUMAlanlarUnvanBilgi.ONCEKIYILVIZECETVELIDURUMU + ",";
            sql += ENUMAlanlarUnvanBilgi.SIMDIKIYILSEYYARGOREVDURUMU;
            sql += " ) VALUES (";
            sql += VeriErisim.DegerYap(sg.yil) + ",";
            sql += VeriErisim.DegerYap(sg.harcamaBirimiKodu.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sg.asama) + ",";
            sql += VeriErisim.DegerYap((int)sgub.istihdamSekliKod) + ",";
            sql += VeriErisim.DegerYap(sgub.unvanKod) + ",";
            sql += VeriErisim.DegerYap(sgub.oncekiVizeCetvelindeVarMi ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(sgub.vizeCetvelindeOlmaliMi ? 1 : 0) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc UnvanBilgisiSil(int yil, string harcamaBirimi, int asama, ENUMPersonelIstihdamSekli istihdamSekli, int unvanKod = 0)
        {
            string sql = "";
            sql = "DELETE  FROM " + TABLOADUNVANBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.HARCAMABIRIMI], "=", harcamaBirimi.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ASAMA], "=", asama));
            if (istihdamSekli > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI], "=", (int)istihdamSekli));
            if (unvanKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.UNVANKODU], "=", unvanKod));
            return KomutCalistir(sql, 0);
        }

        public IDataReader UnvanBilgiGetir(VeriErisim ve, int yil, string harcamaBirimi, int asama, ENUMPersonelIstihdamSekli istihdamSekli, int unvanKod)
        {
            string sql;
            sql = "SELECT ISNULL(" + ENUMAlanlarUnvanBilgi.ONCEKIYILVIZECETVELIDURUMU + ",0) AS " + ENUMAlanlarUnvanBilgi.ONCEKIYILVIZECETVELIDURUMU + " , ISNULL(" + ENUMAlanlarUnvanBilgi.SIMDIKIYILSEYYARGOREVDURUMU + ",0) AS " + ENUMAlanlarUnvanBilgi.SIMDIKIYILSEYYARGOREVDURUMU + " FROM " + TABLOADUNVANBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.HARCAMABIRIMI], "=", harcamaBirimi.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ASAMA], "=", asama));

            string istSekKosul = "(" + VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI], "=", (int)istihdamSekli);
            //if (istihdamSekli == ENUMPersonelIstihdamSekli.SOZLESMELI)
            //{
            //    istSekKosul += " OR " + VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI], "=", 3);
            //}
            istSekKosul += ")";
            sql = VeriErisim.KosulEkle(sql, "AND", istSekKosul);
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.UNVANKODU], "=", unvanKod));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UnvanTespitListele(int yil, int asama, string harcamaBirimKosul, ENUMPersonelIstihdamSekli istihdamSekli, int unvanKodu)
        {
            string sql = "";
            //sql = "SELECT " + ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI + " , " + ENUMAlanlarUnvanBilgi.UNVANKODU;
            sql = "SELECT " + ENUMAlanlarUnvanBilgi.SIMDIKIYILSEYYARGOREVDURUMU;
            sql += " FROM " + TABLOADUNVANBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ASAMA], "=", asama));
            if (unvanKodu > -1)
                sql += " AND " + ENUMAlanlarUnvanBilgi.UNVANKODU + "=" + unvanKodu;
            if ((int)istihdamSekli > 0)
                sql += " AND " + ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI + "=" + (int)istihdamSekli;
            else if (istihdamSekli == 0)
                sql += " AND " + ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI + " IN (" + (int)TNS.PER.ENUMPersonelIstihdamSekli.MEMUR + "," + (int)TNS.PER.ENUMPersonelIstihdamSekli.ISCI + "," + (int)TNS.PER.ENUMPersonelIstihdamSekli.Sozlesmeli + "," + (int)TNS.PER.ENUMPersonelIstihdamSekli.Gecici_Isci + ")";
            sql += " AND " + ENUMAlanlarUnvanBilgi.HARCAMABIRIMI + " " + harcamaBirimKosul;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UnvanTespitTalepSayisiListele(int yil, int asama, string harcamabirimiKosul, int unvanKod, ENUMPersonelIstihdamSekli istihdamSekli)
        {
            string sql = "";
            sql = " SELECT COUNT(*) AS TALEPSAYISI ";
            sql += " FROM " + TABLOADUNVANBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ASAMA], "=", asama));
            sql += " AND " + ENUMAlanlarUnvanBilgi.HARCAMABIRIMI + harcamabirimiKosul;
            if (unvanKod > -1)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.UNVANKODU], "=", unvanKod));
            if (istihdamSekli > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARUNVANBILGI[(int)ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI], "=", (int)istihdamSekli));
            sql += " GROUP BY " + ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI + "," + ENUMAlanlarUnvanBilgi.UNVANKODU;
            return veriErisim.DataReaderDondur(sql);
        }

        public int HarcamaBirimiVeritabanindaKayitlimi(string harcamaBirimi)
        {
            string sql = "";
            sql = "SELECT COUNT(KURUMSALYAPIKOD) AS ADET FROM BHMKURUMSALYAPI WHERE KURUMSALYAPIKOD =" + VeriErisim.DegerYap(harcamaBirimi.Replace(".", ""));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader PersonelBilgisiListele(string birimkodu, int muhasebekodu)
        {
            string sql = "";
            sql = "SELECT PERPERSONEL.AD, PERPERSONEL.SOYAD, PERPERSONEL.ISTIHDAMSEKLI, ";
            sql += " PERPERSONEL.UNVANKODU, PERUNVAN.AD AS UNVANADI,  PERPERSONEL.ODEMEESASDERECE,  PERPERSONEL.ODEMEESASKADEME  ";// PERPERSONEL.KADRODERECE ";
            sql += " FROM PERPERSONEL INNER JOIN ";
            sql += " PERUNVAN ON PERPERSONEL.UNVANKODU = PERUNVAN.KOD ";
            sql += " WHERE (PERPERSONEL.BIRIMKODU = '" + birimkodu + "') AND (PERPERSONEL.MUHASEBEKODU = " + muhasebekodu + ") ";
            return veriErisim.DataReaderDondur(sql);
        }

        public int PersonelSayisiGetir(int istihdamSekli, string birimkodu, int muhasebekodu)
        {
            string sql = "SELECT COUNT(*) AS ADET FROM PERPERSONEL WHERE (PERPERSONEL.ISTIHDAMSEKLI =" + istihdamSekli;
            //if (istihdamSekli == 2)
            //    sql += " OR PERPERSONEL.ISTIHDAMSEKLI = 3";
            sql += ")";
            if (!string.IsNullOrWhiteSpace(birimkodu) && muhasebekodu > 0)
                sql += $" AND PERPERSONEL.BIRIMKODU = {VeriErisim.DegerYap(birimkodu)} AND PERPERSONEL.MUHASEBEKODU = {VeriErisim.DegerYap(muhasebekodu)}";
            //sql += " GROUP BY ISTIHDAMSEKLI ";
            return veriErisim.TekKolonSQLInt(sql);
        }

        public IDataReader HarcamaBirimiGetir(string harcamaBirimi)
        {
            string sql = "SELECT KOD, AD ";
            sql += " FROM BHMKURUMSALYAPI ";
            sql += " WHERE (LEN(KOD) = 11) AND (KOD LIKE '" + harcamaBirimi.Replace(".", "") + "%')  ";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UnvanListele(int istihdamSekli, int unvanKodu)
        {
            string sql = "SELECT DISTINCT PERPERSONEL.ISTIHDAMSEKLI, PERPERSONEL.UNVANKODU, PERUNVAN.AD ";
            sql += " FROM PERPERSONEL INNER JOIN PERUNVAN ON PERPERSONEL.UNVANKODU = PERUNVAN.KOD ";
            sql += " WHERE ";
            if (istihdamSekli > 0)
                sql += "(PERPERSONEL.ISTIHDAMSEKLI =" + istihdamSekli + ")";
            else
                sql += "(PERPERSONEL.ISTIHDAMSEKLI IN (1, 2, 4))";

            if (unvanKodu > 0)
                sql += " AND (PERPERSONEL.UNVANKODU = " + unvanKodu + ") ";
            sql += " ORDER BY 1,3";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader HarcamaBirimiVePersonelAdetListele(int istihdamSekli, int unvanKodu)
        {
            string sql = "SELECT " + ENUMAlanlarUnvanBilgi.HARCAMABIRIMI + " , COUNT(*) AS ADET ";
            sql += " FROM " + TABLOADUNVANBILGI;
            sql += " WHERE " + ENUMAlanlarUnvanBilgi.UNVANKODU + "=" + unvanKodu + " AND " + ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI + "=" + istihdamSekli;
            sql += " AND HARCAMABIRIMI <>'' ";
            sql += " GROUP BY  " + ENUMAlanlarUnvanBilgi.HARCAMABIRIMI + " , " + ENUMAlanlarUnvanBilgi.UNVANKODU + " ,  " + ENUMAlanlarUnvanBilgi.ISTIHDAMSEKLI;
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader VizeliDurumBul(int yil, int saymanlik, string birim, string kurKod)
        {
            string sql = "SELECT P.ISTIHDAMSEKLI, SUM(P.GUNSAYISI) FROM HRCSEYYARGOREVPLANLANAN P INNER JOIN BHMKURUMSALYAPI KY ON P.HARCAMABIRIMI = KY.KOD LEFT OUTER JOIN HRCSEYYARGOREVREVIZEFORM AS F ON F.YIL = P.YIL AND F.BELGENO=P.BELGENO AND F.BELGETUR=" + (int)ENUMRevizeFormBelgeTur.DetayRevize;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("P", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", "ASAMA", "=", 3));
            if (saymanlik > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KY", "SAYMANLIKKOD", "=", saymanlik));
            if (!string.IsNullOrWhiteSpace(birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KY", "BIRIMKOD", "=", birim));
            if (!string.IsNullOrWhiteSpace(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("KY", "KURKOD", "=", kurKod.Replace(",", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", "(P.BELGENO=0 OR (P.GUNSAYISI<0 AND F.DURUM<=5) OR (P.GUNSAYISI>0 AND F.DURUM=5))");
            sql += " group by P.ISTIHDAMSEKLI order by P.ISTIHDAMSEKLI";
            return veriErisim.DataReaderDondur(sql);
        }

        public void SurecKullanilanBul(int yil, int saymanlik, string birim, string kurKod, int istihdamSekli, int islemKod, int surecYil, string surecNo, out int kullanilan, out double tutar)
        {
            kullanilan = 0;
            tutar = 0;
            string sql = "SELECT SUM(B.CARPAN), SUM(B.CARPAN * B.SABITTUTAR) FROM PSLCESITLIODEMEBORDRO AS B INNER JOIN HRCSUREC AS S ON B.YIL = S.YIL AND B.SURECNO = S.SURECNO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("B", "SURECYIL", "=", yil));

            if (saymanlik > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "MUHASEBEKOD", "=", saymanlik));

            if (!string.IsNullOrWhiteSpace(birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "BIRIMKOD", "=", birim));

            if (istihdamSekli > 0)
            {
                if (istihdamSekli == 4)
                    sql = VeriErisim.KosulEkle(sql, "AND", "B.ODEMETUR IN (4,5)");
                else 
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "ODEMETUR", "=", istihdamSekli));
            }

            if (islemKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "ISLEMKOD", "=", islemKod));

            if (!string.IsNullOrWhiteSpace(surecNo))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "YIL", "=", surecYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "SURECNO", "=", surecNo));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "SURECDURUM", "<", 9));

            IDataReader dr = this.veriErisim.DataReaderDondur(sql);
            if (dr != null)
            {
                if (dr.Read())
                {
                    kullanilan = (int)VeriErisim.DegerAl(dr, 0, (double)0);
                    tutar = VeriErisim.DegerAl(dr, 1, (double)0);
                }
                dr.Close();
            }
        }

        public static string IlTavanBul(int yil, string senaryoKod, BKD.ENUMTavanTur tavanTur, bool ilkTeklifTavan, string ilKodlar)
        {
            string sql = "SELECT O.KOD1, SUM(O.YIL0), SUM(O.YIL1), SUM(O.YIL2), SUM(O.EK0), SUM(O.EK1), SUM(O.EK2) FROM BHMTAVANGENEL AS O ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "ASAMA", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "TAVANTUR", "=", (int)tavanTur));
            if (ilkTeklifTavan)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "KOD2", "=", "000000"));
            if (!string.IsNullOrWhiteSpace(ilKodlar))
                sql = VeriErisim.KosulEkle(sql, "AND", "O.KOD1" + " IN (" + ilKodlar + ")");
            sql += " GROUP BY O.KOD1 ORDER BY 1";
            return sql;
        }

        public static string BirimTavanBul(int yil, string senaryoKod, BKD.ENUMTavanTur tavanTur, int ilKod, string birimKod, string kurKod)
        {
            string sql = "SELECT O.KOD1, O.KOD2, O.KOD3, O.YIL0, O.YIL1 FROM BHMTAVANGENEL AS O";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("O", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "ASAMA", "=", senaryoKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "TAVANTUR", "=", (int)tavanTur));

            if (ilKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "KOD1", "LIKE", ilKod.ToString("00") + "%"));

            if (!string.IsNullOrWhiteSpace(birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "KOD2", "=", birimKod));

            if (!string.IsNullOrWhiteSpace(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("O", "KOD3", "=", kurKod.Replace(".", "")));

            sql += " ORDER BY 1,2,3";
            return sql;
        }

        public static string BirimPlanlananBul(int yil, int asama, int istihdamSekli, bool sadeceBaslangic, TNSCollection birimKodlar, int belgeNo, bool belgeHaric, bool onaylilar)
        {
            string alanlar = "SG.HARCAMABIRIMI";
            string sql = "SELECT " + alanlar + ", SUM(SG.GUNSAYISI) FROM HRCSEYYARGOREVPLANLANAN AS SG LEFT OUTER JOIN HRCSEYYARGOREVREVIZEFORM AS F ON F.YIL = SG.YIL AND F.BELGENO = SG.BELGENO AND F.BELGETUR=" + (int)ENUMRevizeFormBelgeTur.DetayRevize;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("SG", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "ASAMA", "=", asama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "ISTIHDAMSEKLI", "=", istihdamSekli));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "GUNSAYISI", "<>", 0));

            bool onayli = false;
            if (belgeNo > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "BELGENO", belgeHaric ? "<" : "=", belgeNo));
                onayli = belgeHaric;
            }
            else if (sadeceBaslangic)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "BELGENO", "=", 0));
            else
            {
                if (!belgeHaric)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "BELGENO", ">", 0));
                onayli = true;
            }

            if (onayli)
            {
                if (onaylilar)
                    sql = VeriErisim.KosulEkle(sql, "AND", "(SG.BELGENO=0 OR F.DURUM=5)");
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", "(SG.BELGENO=0 OR (SG.GUNSAYISI<0 AND F.DURUM<=5) OR (SG.GUNSAYISI>0 AND F.DURUM=5))");
            }
            sql += TABRaporlar.BirimListesiKYKosulOl("AND", "SG", "HARCAMABIRIMI", birimKodlar);

            sql += " GROUP BY " + alanlar;
            sql += " ORDER BY " + alanlar;
            return sql;
        }

        public static string IlPlanlananBul(int yil, int asama, int istihdamSekli, string ilKodlar, int haricBelgeNo, bool sonDurum)
        {
            string alanAd1 = Arac.KYYapiIlKosul();// "CASE WHEN O.SAYMANLIKKOD < 1000 THEN '06' ELSE RIGHT('0' + CONVERT(VARCHAR, O.SAYMANLIKKOD/1000),2) END";

            string sql = "SELECT " + alanAd1 + ", SUM(SG.GUNSAYISI) FROM HRCSEYYARGOREVPLANLANAN AS SG INNER JOIN BHMKURUMSALYAPI AS O ON SG.HARCAMABIRIMI = O.KOD LEFT OUTER JOIN HRCSEYYARGOREVREVIZEFORM AS F ON F.YIL = SG.YIL AND F.BELGENO=SG.BELGENO AND F.BELGETUR=" + (int)ENUMRevizeFormBelgeTur.DetayRevize;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("SG", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "ASAMA", "=", asama));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "ISTIHDAMSEKLI", "=", istihdamSekli));
            if (haricBelgeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("SG", "BELGENO", "<>", haricBelgeNo));

            if (!string.IsNullOrWhiteSpace(ilKodlar))
                sql = VeriErisim.KosulEkle(sql, "AND", alanAd1 + " IN (" + ilKodlar + ")");
            if (sonDurum)
                sql = VeriErisim.KosulEkle(sql, "AND", "(SG.BELGENO = 0 OR (SG.GUNSAYISI > 0 AND F.DURUM <= 5) OR (SG.GUNSAYISI < 0 AND F.DURUM = 5))");
            else
                sql = VeriErisim.KosulEkle(sql, "AND", "SG.BELGENO = 0");
            sql += " GROUP BY " + alanAd1 + " ORDER BY 1";
            return sql;
        }

        public static string SurecKullanilanBulSql(int yil, int istihdamSekli, int islemKod, int surecYil, string surecNo, TNSCollection birimKodlar)
        {
            string sql = "SELECT O.KOD, SUM(B.CARPAN), SUM(B.CARPAN * B.SABITTUTAR) " +
                "FROM PSLCESITLIODEMEBORDRO AS B INNER JOIN HRCSUREC AS S ON B.YIL = S.YIL AND B.SURECNO = S.SURECNO " +
                "INNER JOIN BHMKURUMSALYAPI O ON O.SAYMANLIKKOD = S.MUHASEBEKOD AND O.BIRIMKOD = S.BIRIMKOD ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("B", "SURECYIL", "=", yil));

            if (istihdamSekli > 0)
            {
                if (istihdamSekli == 4 || istihdamSekli == 5)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "ODEMETUR", ">=", 4));
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "ODEMETUR", "=", istihdamSekli));
            }

            if (islemKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "ISLEMKOD", "=", islemKod));

            if (!string.IsNullOrWhiteSpace(surecNo))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "YIL", "=", surecYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "SURECNO", "=", surecNo));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("S", "SURECDURUM", "<", 9));

            sql += TABRaporlar.BirimListesiKYKosulOl("AND", "O", "KOD", birimKodlar);

            sql += " GROUP BY O.KOD,  O.BIRIMKOD, O.KURKOD ORDER BY 1, 2, 3";
            return sql;
        }

        public static string IlListeleSql(string ilKodlar)
        {
            string kod = Arac.KYYapiIlKosul(false);
            string sql = $"SELECT {kod}, AD FROM BHMKURUMSALYAPI WHERE LEN(KOD)=3 AND (KOD LIKE '2%' OR KOD LIKE '9%')";
            if (!string.IsNullOrWhiteSpace(ilKodlar))
                sql = VeriErisim.KosulEkle(sql, "AND", kod + " IN (" + ilKodlar + ")");
            sql += " ORDER BY 1";

            return sql;
        }

        public Sonuc OncekiAsamadanAl(int kaynakYil, int kaynakAsama, int hedefYil, int hedefAsama)
        {
            string sql = $"INSERT INTO {TABLOADPLANLANAN} (YIL, ASAMA, HARCAMABIRIMI, ISTIHDAMSEKLI, GUNSAYISI, BELGENO)"
                + $" SELECT {hedefYil} as YIL, {hedefAsama} AS ASAMA, HARCAMABIRIMI, ISTIHDAMSEKLI, GUNSAYISI, BELGENO"
                + $" from {TABLOADPLANLANAN} WHERE YIL={kaynakYil} AND ASAMA={kaynakAsama} AND BELGENO=0";

            return KomutCalistir(sql, 0);
        }

        public int FormSayiBul(SeyyarGorevRevize form, TNSCollection birimKodlar)
        {
            string sql = "SELECT COUNT(*) FROM HRCSEYYARGOREVREVIZEFORM P";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("P", "YIL", "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", "DURUM", "=", (int)form.durum));
            if (birimKodlar != null && birimKodlar.Count > 0)
            {
                string birimKosul = TABRaporlar.BirimListesiKYKosulOl("AND", "HRCSEYYARGOREVPLANLANAN", "HARCAMABIRIMI", birimKodlar);
                sql = VeriErisim.KosulEkle(sql, "AND", $"1 IN (SELECT 1 FROM HRCSEYYARGOREVPLANLANAN WHERE HRCSEYYARGOREVPLANLANAN.YIL=P.YIL AND HRCSEYYARGOREVPLANLANAN.BELGENO=P.BELGENO {birimKosul})");
            }
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
