using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABHibeMakineYonetim : TABOrtak
    {
        public TABHibeMakineYonetim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMHIBEMAKINAYONETIM";
        public static string TABLOADGIRIS = "TRMHIBEMAKINAYONETIMGIRIS";

        public enum ENUMAlanlar : int
        {
            ETAPNO = 0,
            GERCEK = 1,
            TUZEL = 2,
            TARIH1 = 3,
            TARIH2 = 4,
            AKTIF = 5,
            GIRIS = 6,
            TEBLIGNO = 7,
            ONLINEKAPATMATARIHI = 8
        }
        public enum ENUMAlanlarGiris : int
        {
            ETAPNO = 0,
            ILKOD = 1,
            TARIH = 2
        }

        public static string[] ALANLAR = { "ETAPNO", "GERCEK", "TUZEL", "TARIH1", "TARIH2", "AKTIF", "GIRIS", "TEBLIGNO", "ONLINEKAPATMATARIHI" };
        public static string[] ALANLARGIRIS = { "ETAPNO", "ILKOD", "TARIH" };

        public Sonuc Kaydet(Yonetim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (form.aktif > 0)
            {
                sonuc = this.HepsiniPasifYap();
                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            sonuc = this.Guncelle(form);
            if (!sonuc.islemSonuc)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.etapNo) + ",";
                sql += VeriErisim.DegerYap(form.gercek) + ",";
                sql += VeriErisim.DegerYap(form.tuzel) + ",";
                sql += VeriErisim.DegerYap(form.tarih1) + ",";
                sql += VeriErisim.DegerYap(form.tarih2) + ",";
                sql += VeriErisim.DegerYap(form.aktif) + ",";
                sql += VeriErisim.DegerYap(form.giris) + ",";
                sql += VeriErisim.DegerYap(form.tebligNo) + ",";
                sql += VeriErisim.DegerYap(form.onlineGirisiKapatmaTarihi) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(Yonetim form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GERCEK], form.gercek) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUZEL], form.tuzel) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], form.tarih1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], form.tarih2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], form.aktif) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GIRIS], form.giris) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEBLIGNO], form.tebligNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONLINEKAPATMATARIHI], form.onlineGirisiKapatmaTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(YonetimGiris form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "UPDATE " + TABLOADGIRIS + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.TARIH], form.tarih);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ETAPNO], "=", form.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ILKOD], "=", form.ilKod));

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "INSERT INTO " + TABLOADGIRIS + " (" + VeriErisim.TumAlanlar("", ALANLARGIRIS) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.etapNo) + ",";
                sql += VeriErisim.DegerYap(form.ilKod) + ",";
                sql += VeriErisim.DegerYap(form.tarih) + ")";

                sonuc = KomutCalistir(sql, 1);
            }


            return sonuc;
        }

        public IDataReader Listele(Yonetim form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (form.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));
            if (form.aktif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AKTIF], "=", form.aktif));

            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " ORDER BY 1 DESC";

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader Listele(YonetimGiris form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARGIRIS) + " FROM " + TABLOADGIRIS;
            if (form.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGIRIS, ALANLARGIRIS[(int)ENUMAlanlarGiris.ETAPNO], "=", form.etapNo));
            if (!string.IsNullOrEmpty(form.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGIRIS, ALANLARGIRIS[(int)ENUMAlanlarGiris.ILKOD], "=", form.ilKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(Yonetim form)
        {
            Sonuc sonuc = this.Sil(new YonetimGiris() { etapNo = form.etapNo });
            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Sil(YonetimGiris form)
        {
            string sql = "DELETE FROM " + TABLOADGIRIS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ETAPNO], "=", form.etapNo));
            if (!string.IsNullOrEmpty(form.ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGIRIS[(int)ENUMAlanlarGiris.ILKOD], "=", form.ilKod));

            return KomutCalistir(sql, 0);
        }

        private Sonuc HepsiniPasifYap()
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], 0);
            return KomutCalistir(sql, 0);
        }
        private int EtapNoAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.ETAPNO] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public Sonuc AnketSorulariniAktar(string onEkTur, string eskiEtap, string yeniEtap, int eklenecekSatir, TNSDateTime baslamaTarih, TNSDateTime bitisTarih)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";

            sql = "DELETE ";
            sql += " FROM ANKANKETBILGI WHERE (ANAHTAR LIKE '" + onEkTur + "_" + yeniEtap + "%')";
            sonuc = KomutCalistir(sql, 0);
            sql = "DELETE ";
            sql += " FROM ANKANKETSORULAR WHERE (ANAHTAR LIKE '" + onEkTur + "_" + yeniEtap + "%')";
            sonuc = KomutCalistir(sql, 0);
            sql = "DELETE ";
            sql += " FROM ANKCEVAPSECENEKLER WHERE (PRSIRANO IN (SELECT PRSIRANO FROM ANKANKETSORULAR WHERE (ANAHTAR LIKE '" + onEkTur + "_" + yeniEtap + "%')))";
            sonuc = KomutCalistir(sql, 0);
            sql = "DELETE ";
            sql += " FROM ANKSORU WHERE (ANAHTAR1 LIKE '" + onEkTur + "_" + yeniEtap + "%')";
            sonuc = KomutCalistir(sql, 0);


            sql = "INSERT INTO ANKANKETBILGI  (MODUL, ANAHTAR, ANKETAD, BASLATARIH, BITISTARIH, TEKGIRISLIK)";
            sql += " SELECT MODUL, REPLACE(ANAHTAR,'_" + eskiEtap + "_','_" + yeniEtap + "_'), ANKETAD, " + VeriErisim.DegerYap(baslamaTarih) + ", " + VeriErisim.DegerYap(bitisTarih) + ", TEKGIRISLIK";
            sql += " FROM ANKANKETBILGI WHERE (ANAHTAR LIKE '" + onEkTur + "_" + eskiEtap + "%')";
            sonuc = KomutCalistir(sql, 0);

            sql = "INSERT INTO ANKANKETSORULAR (MODUL, ANAHTAR, PRSIRANO, SORUSIRA, SORUSEVIYE)";
            sql += " SELECT MODUL, REPLACE(ANAHTAR,'_" + eskiEtap + "_','_" + yeniEtap + "_'), PRSIRANO+" + eklenecekSatir + ", SORUSIRA, SORUSEVIYE";
            sql += " FROM ANKANKETSORULAR WHERE (ANAHTAR LIKE '" + onEkTur + "_" + eskiEtap + "%')";
            sonuc = KomutCalistir(sql, 0);

            sql = "INSERT INTO ANKCEVAPSECENEKLER (PRSIRANO, SECENEKSIRANO, SECENEK, METIN, PUAN)";
            sql += " SELECT PRSIRANO+" + eklenecekSatir + ", SECENEKSIRANO, SECENEK, METIN, PUAN";
            sql += " FROM ANKCEVAPSECENEKLER WHERE (PRSIRANO IN (SELECT PRSIRANO FROM ANKANKETSORULAR WHERE (ANAHTAR LIKE '" + onEkTur + "_" + eskiEtap + "%')))";
            sonuc = KomutCalistir(sql, 0);

            sql = "INSERT INTO ANKSORU (PRSIRANO, METIN, SECENEKTUR, ANAHTAR1, ANAHTAR2, CEVAPSECENEK, DURUM)";
            sql += " SELECT PRSIRANO+" + eklenecekSatir + ", METIN, SECENEKTUR, REPLACE(ANAHTAR1,'_" + eskiEtap + "_','_" + yeniEtap + "_'), ANAHTAR2, CEVAPSECENEK, DURUM";
            sql += " FROM ANKSORU WHERE (ANAHTAR1 LIKE '" + onEkTur + "_" + eskiEtap + "%')";
            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        public int AnketSoruMaxPrSiraNo()
        {
            string sql = "SELECT MAX(PRSIRANO) FROM ANKSORU";
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}