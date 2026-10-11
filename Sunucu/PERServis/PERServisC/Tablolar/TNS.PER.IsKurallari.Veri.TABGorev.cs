using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABGorev : TABOrtak
    {
        public TABGorev(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PERGOREV";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            AMAC = 3,
            TUR = 4,
            DURUM = 5,
            ISLEMKOD = 6,
            EVREVRAKNO = 7,
            PREVRAKNO = 8,
            ORGKOD = 9,
            KURUM = 10,
            VASITA = 11,
            AVANS = 12,
            YIL = 13,
            ALAN1 = 14,
            IMZA1 = 15,
            IMZA2 = 16,
            IMZA3 = 17,
        }
        static string[] ALANLAR = { "KOD", "MUHASEBEKOD", "BIRIMKOD", "AMAC", "TUR", "DURUM", "ISLEMKOD", "EVREVRAKNO", "PREVRAKNO", "ORGKOD", "KURUM", "VASITA", "AVANS", "YIL", "ALAN1", "IMZA1", "IMZA2", "IMZA3" };

        public static string TABLOADMUHASEBEVIEW = "HRCMUHASEBEVIEW";
        public enum ENUMAlanlarMUHASEBEVIEW
        {
            MUHASEBEKOD = 0,
            MUHASEBEAD = 1,
        }
        public static string[] ALANLARMUHASEBEVIEW = { "MUHASEBEKOD", "MUHASEBEAD" };


        public IDataReader Sorgula(Gorev p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.gorevliPersoneller != null && p.gorevliPersoneller.ObjeSayisi > 0)
            {
                sql += " LEFT OUTER JOIN " +
                TABGorevPersonel.TABLOAD + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " +
                TABGorevPersonel.TABLOAD + "." + TABGorevPersonel.ALANLAR[(int)TABGorevPersonel.ENUMAlanlar.GOREVYIL]
                + " AND " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " +
                TABGorevPersonel.TABLOAD + "." + TABGorevPersonel.ALANLAR[(int)TABGorevPersonel.ENUMAlanlar.GOREVKOD];
            }

            if (p.gorevYerleri != null && p.gorevYerleri.ObjeSayisi > 0)
            {
                sql += " LEFT OUTER JOIN " +
                TABGorevYerTarih.TABLOAD + " ON " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = " +
                TABGorevYerTarih.TABLOAD + "." + TABGorevYerTarih.ALANLAR[(int)TABGorevYerTarih.ENUMAlanlar.GOREVYIL]
                + " AND " +
                TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " +
                TABGorevYerTarih.TABLOAD + "." + TABGorevYerTarih.ALANLAR[(int)TABGorevYerTarih.ENUMAlanlar.GOREVKOD];
            }


            if (p.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", p.yil));

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", p.muhasebeKod));

            if (!string.IsNullOrEmpty(p.birimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", p.birimKod));

            if (!string.IsNullOrEmpty(p.kurum))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUM], "=", p.kurum));

            if (p.orgKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", p.orgKod));

            if (p.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", p.tur));

            if (p.islemKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", p.islemKod));

            if (p.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", p.durum));

            if (!string.IsNullOrEmpty(p.vasita))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VASITA], "=", p.vasita));

            if (!string.IsNullOrEmpty(p.avans))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AVANS], "=", p.avans));

            if (p.gorevliPersoneller != null && p.gorevliPersoneller.ObjeSayisi > 0)
            {
                GorevPersonel personel = (GorevPersonel)p.gorevliPersoneller.objeler[0];
                if (!string.IsNullOrEmpty(personel.tckimlikno))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABGorevPersonel.TABLOAD, TABGorevPersonel.ALANLAR[(int)TABGorevPersonel.ENUMAlanlar.TCKIMLIKNO], "=", personel.tckimlikno));

                if (!string.IsNullOrEmpty(personel.ad))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABGorevPersonel.TABLOAD, TABGorevPersonel.ALANLAR[(int)TABGorevPersonel.ENUMAlanlar.AD], "=", personel.ad));

                if (!string.IsNullOrEmpty(personel.soyad))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABGorevPersonel.TABLOAD, TABGorevPersonel.ALANLAR[(int)TABGorevPersonel.ENUMAlanlar.SOYAD], "=", personel.soyad));
            }

            if (p.gorevYerleri != null && p.gorevYerleri.ObjeSayisi > 0)
            {
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(Gorev p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", p.yil));

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", p.muhasebeKod));

            if (!string.IsNullOrEmpty(p.birimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", p.birimKod));

            if (!string.IsNullOrEmpty(p.kurum))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURUM], "=", p.kurum));

            if (!string.IsNullOrEmpty(p.prEvrakNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", p.prEvrakNo));

            if (!string.IsNullOrEmpty(p.evrEvrakNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", p.evrEvrakNo));

            if (p.orgKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", p.orgKod));

            if (p.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", p.tur));

            if (p.islemKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMKOD], "=", p.islemKod));

            if (p.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", p.durum));

            if (!string.IsNullOrEmpty(p.vasita))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.VASITA], "=", p.vasita));

            if (!string.IsNullOrEmpty(p.avans))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AVANS], "=", p.avans));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Gorev p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMAC], p.amac) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], p.tur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMKOD], p.islemKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], p.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], p.evrEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], p.prEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VASITA], p.vasita) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AVANS], p.avans);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", p.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(p.kod) + ",";
                sql += VeriErisim.DegerYap(p.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(p.birimKod) + ",";
                sql += VeriErisim.DegerYap(p.amac) + ",";
                sql += VeriErisim.DegerYap(p.tur) + ",";
                sql += VeriErisim.DegerYap(p.durum) + ",";
                sql += VeriErisim.DegerYap(p.islemKod) + ",";
                sql += VeriErisim.DegerYap(p.evrEvrakNo) + ",";
                sql += VeriErisim.DegerYap(p.prEvrakNo) + ",";
                sql += VeriErisim.DegerYap(p.orgKod) + ",";
                sql += VeriErisim.DegerYap(p.kurum.Replace(".","")) + ",";
                sql += VeriErisim.DegerYap(p.vasita) + ",";
                sql += VeriErisim.DegerYap(p.avans) + ",";
                sql += VeriErisim.DegerYap(p.yil) + ",";
                sql += VeriErisim.DegerYap(p.alan1) + ",";
                sql += VeriErisim.DegerYap(p.imza1) + ",";
                sql += VeriErisim.DegerYap(p.imza2) + ",";
                sql += VeriErisim.DegerYap(p.imza3) + ")";

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc DurumDegistir(int yil, int kod, int durum)
        {
            string sql = string.Empty;
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetEvrakBilgi(int yil, int kod, string evrEvrakNo, string prEvrakNo)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], evrEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], prEvrakNo);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(Gorev p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", p.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            if (p.muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", p.muhasebeKod));
            if (!string.IsNullOrEmpty(p.birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", p.birimKod));
            if (p.orgKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGKOD], "=", p.orgKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetAlanBilgi(int yil, int kod, string alan1, string imza1, string imza2, string imza3)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALAN1], alan1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZA1], imza1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZA2], imza2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IMZA3], imza3);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
    }
}
