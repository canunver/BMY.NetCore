using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABArsiv : TABOrtak
    {
        public TABArsiv(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRARSIV";
        public enum ENUMAlanlar : int { KOD, ANAKOD, SIRA, AD, TURKOD, KODYOL, ISLEMYAPAN, EVRAKBIRIMKOD }
        public static string[] ALANLAR = { "KOD", "ANAKOD", "SIRA", "AD", "TURKOD", "KODYOL", "ISLEMYAPAN", "EVRAKBIRIMKOD" };

        public static string TABLOADYETKI = "EVRARSIVYETKI";
        public enum ENUMAlanlarYetki : int { ARSIVKOD, YETKI, YETKITIP, ACIKLAMA }
        public static string[] ALANLARYETKI = { "ARSIVKOD", "YETKI", "YETKITIP", "ACIKLAMA" };

        public static string TABLOADMETA = "EVRARSIVMETA";
        public enum ENUMAlanlarMeta : int { ARSIVKOD, METAKOD }
        public static string[] ALANLARMETA = { "ARSIVKOD", "METAKOD" };

        public IDataReader Listele(Arsiv arsiv)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            if (arsiv.yetki != null && arsiv.yetki.Length > 0)
            {
                sql = sql.Replace("SELECT", "SELECT DISTINCT");
                sql += " LEFT OUTER JOIN EVRARSIVYETKI ON EVRARSIV.KOD = EVRARSIVYETKI.ARSIVKOD ";

                int index = 0;
                foreach (ArsivYetki y in arsiv.yetki)
                {
                    if (index == 0) kosul += " ( ";

                    index++;
                    if (y.yetkiTip == (int)ENUMArsivYetkiTip.BIRIM)
                        kosul += " (EVRARSIVYETKI.YETKITIP = " + y.yetkiTip + ") AND (EVRARSIVYETKI.YETKI LIKE '" + y.yetkiDeger + "%' ) ";
                    else
                        kosul += " (EVRARSIVYETKI.YETKITIP = " + y.yetkiTip + ") AND (EVRARSIVYETKI.YETKI = " + VeriErisim.DegerYap(y.yetkiDeger) + " ) ";

                    if (index < arsiv.yetki.Length)
                        kosul += " OR ";
                }

                if (arsiv.turKod == (int)ENUMEvrakArsivTur.KISISEL)
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", arsiv.islemYapan));

                kosul += " ) ";
            }
            else
                if (arsiv.turKod == (int)ENUMEvrakArsivTur.KISISEL)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", arsiv.islemYapan));

            if (arsiv.evrakBirimKod != "" && arsiv.turKod != (int)ENUMEvrakArsivTur.KISISEL && string.IsNullOrEmpty(arsiv.kodYol))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", arsiv.evrakBirimKod));

            if (arsiv.anaKod != 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAKOD], "=", arsiv.anaKod));
            if (arsiv.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", arsiv.kod));
            if (arsiv.turKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TURKOD], "=", arsiv.turKod));

            if (!string.IsNullOrEmpty(arsiv.kodYol))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODYOL], "LIKE", arsiv.kodYol));

            if (kosul == "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", ""));


            sql += " WHERE " + kosul;
            sql += " ORDER BY 2,3,4";
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(Arsiv form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.anaKod) + ",";
                sql += VeriErisim.DegerYap(form.sira) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap(form.turKod) + ",";
                form.kodYol += form.kod + "-";
                sql += VeriErisim.DegerYap(form.kodYol) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.evrakBirimKod) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(Arsiv form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Sil(Arsiv form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleYetki(int arsivKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARYETKI) + " FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYetki.ARSIVKOD], "=", arsivKod));

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(ArsivYetki form)
        {
            string sql = "INSERT INTO " + TABLOADYETKI + " (" + VeriErisim.TumAlanlar("", ALANLARYETKI) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.arsivKod) + ",";
            sql += VeriErisim.DegerYap(form.yetkiDeger) + ",";
            sql += VeriErisim.DegerYap(form.yetkiTip) + ",";
            sql += VeriErisim.DegerYap(form.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }
        public Sonuc SilYetki(int arsivKod)
        {
            string sql = "DELETE FROM " + TABLOADYETKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARYETKI[(int)ENUMAlanlarYetki.ARSIVKOD], "=", arsivKod));
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleMeta(int arsivKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABMeta.TABLOAD, TABMeta.ALANLAR) + ",EVRMETAGRUP.ADI" + " FROM " + TABLOADMETA;
            sql += " INNER JOIN " + TABMeta.TABLOAD + " ON EVRARSIVMETA.METAKOD = EVRMETA.KOD LEFT OUTER JOIN EVRMETAGRUP ON EVRMETA.GRUPKOD = EVRMETAGRUP.KOD";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADMETA, ALANLARMETA[(int)ENUMAlanlarMeta.ARSIVKOD], "=", arsivKod));
            sql += " ORDER BY EVRMETAGRUP.ADI, EVRMETA.AD";
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(ArsivMeta form)
        {
            string sql = "INSERT INTO " + TABLOADMETA + " (" + VeriErisim.TumAlanlar("", ALANLARMETA) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.arsivKod) + ",";
            sql += VeriErisim.DegerYap(form.meta.kod) + ")";

            return KomutCalistir(sql, 1);
        }
        public Sonuc SilMeta(int arsivKod)
        {
            string sql = "DELETE FROM " + TABLOADMETA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMETA[(int)ENUMAlanlarMeta.ARSIVKOD], "=", arsivKod));
            return KomutCalistir(sql, 0);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }


        public int ArsivDosyasinaKayitliMetaVarMi(int arsivKod, int metaKod)
        {
            string sql = "SELECT COUNT(*) FROM EVREVRAKARSIVMETA INNER JOIN EVREVRAKARSIV ON EVREVRAKARSIVMETA.PREVRAKNO = EVREVRAKARSIV.PREVRAKNO ";
            sql += "WHERE (EVREVRAKARSIVMETA.METAKOD = " + VeriErisim.DegerYap(metaKod) + ") AND (EVREVRAKARSIV.ARSIVKOD = " + VeriErisim.DegerYap(arsivKod) + ")";
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
