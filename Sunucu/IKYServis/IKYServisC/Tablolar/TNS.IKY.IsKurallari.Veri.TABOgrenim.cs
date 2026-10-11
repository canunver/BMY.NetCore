using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABPersonelOgrenim : TABOrtak
    {
        public TABPersonelOgrenim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELOGRENIM";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            OGRENIMKOD = 2,
            UNIVERSITEKOD = 3,
            FAKULTEKOD = 4,
            BOLUMKOD = 5,
            OKULADI = 6,
            BELGETUR = 7,
            BELGETARIHI = 8,
            BELGENO = 9,
            BASLANGICTARIHI = 10,
            DERECE = 11,
            GECERLI = 12,
            OGRENIMYERI = 13,
            DENKLIK = 14,
            DENKLIKTARIHI = 15,
            DENKOKUL = 16,
            DENKBOLUM = 17,
            HAZIRLIK = 18,
            KURUMAIBRAZTARIHI = 19,
            MEMURIYETEGIRISOKULU = 20,
            OGRENIMSURESI = 21
        }

        static string[] ALANLAR = { "KOD", "PKOD", "OGRENIMKOD", "UNIVERSITEKOD", "FAKULTEKOD", "BOLUMKOD", "OKULADI", "BELGETUR", "BELGETARIHI", "BELGENO", "BASLANGICTARIHI", "DERECE", "GECERLI", "OGRENIMYERI", "DENLIK", "DENKLIKTARIHI", "DENKOKUL", "DENKBOLUM", "HAZIRLIK", "KURUMAIBRAZTARIHI", "MEMURIYETEGIRISOKULU", "OGRENIMSURESI" };

        public static string ListeleUstTablo(string ustTabloAlan)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", IKYSOZLUK.KAYITKOD, IKYSOZLUK.HITAPID HITAPID1, SOZLUK2.HITAPID HITAPID2, IKYSOZLUK.KAYITAD ";
            sql += ", (case when SonOkul.KOD = IKYPERSONELOGRENIM.KOD then 1 else 0 end) gecerli";
            sql += " FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN " + TABSozluk.TABLOAD + " ON " + ALANLAR[(int)ENUMAlanlar.OGRENIMKOD] + "=" + TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KOD];
            sql += " LEFT OUTER JOIN  IKYSOZLUK AS SOZLUK2 ON OGRENIMYERI = SOZLUK2.KOD";
            sql += " left outer join (select KOD from IKYPERSONELOGRENIM oIc where BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) FROM IKYPERSONELOGRENIM oIcIc WHERE oIcIc.PKOD = oIc.PKOD)) as SonOkul on SonOkul.KOD = IKYPERSONELOGRENIM.KOD";
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PKOD] + " = " + ustTabloAlan;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI] + " DESC";
            return sql;
        }

        public IDataReader Listele(PersonelOgrenim p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", IKYSOZLUK.KAYITKOD, IKYSOZLUK.HITAPID, SOZLUK2.HITAPID, IKYSOZLUK.KAYITAD ";
            sql += @", (case when SonOkul.KOD = IKYPERSONELOGRENIM.KOD then 1 else 0 end) gecerli";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN " + TABSozluk.TABLOAD + " ON " + ALANLAR[(int)ENUMAlanlar.OGRENIMKOD] + "=" + TABSozluk.TABLOAD + "." + TABSozluk.ALANLAR[(int)TABSozluk.ENUMAlanlar.KOD];
            sql += " LEFT OUTER JOIN  IKYSOZLUK AS SOZLUK2 ON OGRENIMYERI = SOZLUK2.KOD";
            sql += @" left outer join (select KOD from IKYPERSONELOGRENIM oIc where BASLANGICTARIHI = (SELECT MAX(BASLANGICTARIHI) 
                                                                    FROM IKYPERSONELOGRENIM oIcIc WHERE oIcIc.PKOD = oIc.PKOD)) 
                     as SonOkul on SonOkul.KOD = IKYPERSONELOGRENIM.KOD";

            if (p.personelKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(PersonelOgrenim p)
        {
            string sql;
            if (p.kod == 0)
            {
                p.kod = TABGenel.SayacDegerAlInt(veriErisim, TABLOAD);
                if (!string.IsNullOrWhiteSpace(veriErisim.hata)) return new Sonuc(veriErisim.hata);
            }
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.personelKod) + ",";
            if (p.ogrenimKod > 0)
                sql += VeriErisim.DegerYap(p.ogrenimKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.universiteKod > 0)
                sql += VeriErisim.DegerYap(p.universiteKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.fakulteKod > 0)
                sql += VeriErisim.DegerYap(p.fakulteKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            if (p.bolumKod > 0)
                sql += VeriErisim.DegerYap(p.bolumKod) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.okulAdi) + ",";
            if (p.belgeTur > 0)
                sql += VeriErisim.DegerYap(p.belgeTur) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.belgeTarihi) + ",";
            sql += VeriErisim.DegerYap(p.belgeNo) + ",";
            sql += VeriErisim.DegerYap(p.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(p.derece) + ",";
            sql += VeriErisim.DegerYap(p.gecerli) + ",";
            if (p.ogrenimYeri > 0)
                sql += VeriErisim.DegerYap(p.ogrenimYeri) + ",";
            else
                sql += VeriErisim.DegerYap((string)null) + ",";
            sql += VeriErisim.DegerYap(p.denklik) + ",";
            sql += VeriErisim.DegerYap(p.denklikTarihi) + ",";
            sql += VeriErisim.DegerYap(p.denkOkul) + ",";
            sql += VeriErisim.DegerYap(p.denkBolum) + ",";
            sql += VeriErisim.DegerYap(p.hazirlikVarMi) + ",";
            sql += VeriErisim.DegerYap(p.kurumaIbrazTarihi) + ",";
            sql += VeriErisim.DegerYap(p.memuriyeteBasladigiOkul) + ",";
            sql += VeriErisim.DegerYap(p.ogrenimSuresi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(PersonelOgrenim p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            if (p.ogrenimKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMKOD], p.ogrenimKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMKOD], (string)null) + ",";

            if (p.universiteKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNIVERSITEKOD], p.universiteKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UNIVERSITEKOD], (string)null) + ",";

            if (p.fakulteKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKULTEKOD], p.fakulteKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FAKULTEKOD], (string)null) + ",";
            if (p.bolumKod > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOLUMKOD], p.bolumKod) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BOLUMKOD], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OKULADI], p.okulAdi) + ",";
            if (p.belgeTur > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], p.belgeTur) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], p.belgeTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], p.belgeNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIHI], p.baslangicTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DERECE], p.derece) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GECERLI], p.gecerli) + ",";
            if (p.ogrenimYeri > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMYERI], p.ogrenimYeri) + ",";
            else
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMYERI], (string)null) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DENKLIK], p.denklik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DENKLIKTARIHI], p.denklikTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DENKOKUL], p.denkOkul) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DENKBOLUM], p.denkBolum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HAZIRLIK], p.hazirlikVarMi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KURUMAIBRAZTARIHI], p.kurumaIbrazTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MEMURIYETEGIRISOKULU], p.memuriyeteBasladigiOkul) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OGRENIMSURESI], p.ogrenimSuresi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(PersonelOgrenim p)
        {
            string sql;

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.personelKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc Sil(int pKod)
        {
            string sql;

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", pKod));

            return KomutCalistir(sql, 0);
        }

        //public IDataReader OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        //{
        //    string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
        //    sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
        //    sql += " FROM " + TABLOAD + ", " + TABPersonel.TABLOAD;
        //    //sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABPersonel.TABLOAD + "." + TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.OGRENIMKODU];

        //    //if (muhasebeKod > 0)
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.MUHASEBEKODU], "=", muhasebeKod));

        //    //if (!string.IsNullOrEmpty(birimKod))
        //    //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.BIRIMKODU], "=", birimKod));

        //    sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
        //    sql += " ORDER BY " + VeriErisim.IslecBul("COUNT") + "(*) DESC";

        //    return veriErisim.DataReaderDondur(sql);
        //}
    }
}
