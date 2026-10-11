using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimAmacHedef : TABOrtak
    {
        public TABBirimAmacHedef(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMAMACHEDEF";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            AMACHEDEFKOD = 2,
            TEMAAMACKOD = 3,
            ACIKLAMA = 4,
            SIRA = 5
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "AMACHEDEFKOD", "TEMAAMACKOD", "ACIKLAMA", "SIRA" };

        public Sonuc Kaydet(Birim_AmacHedef bah, bool amac)
        {
            bah.amacHedefKod = KodUret(bah, amac);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bah.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bah.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bah.amacHedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bah.temaAmacKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bah.aciklama) + ",";
            sql += VeriErisim.DegerYap(bah.sira) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = bah.amacHedefKod;
            return sonuc;
        }

        public Sonuc Guncelle(Birim_AmacHedef bah)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEMAAMACKOD], bah.temaAmacKod.Replace(".", "")) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bah.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRA], bah.sira);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bah.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bah.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD], "=", bah.amacHedefKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Birim_AmacHedef bah)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bah.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bah.orgBirimKod));
            if (!string.IsNullOrEmpty(bah.amacHedefKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD], "=", bah.amacHedefKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(bah.temaAmacKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEMAAMACKOD], "=", bah.temaAmacKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(Birim_AmacHedef bah, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bah.donem != null && bah.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bah.donem.donemNo);
            if (!sorgu && bah.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bah.orgBirimKod));
            if (!string.IsNullOrEmpty(bah.amacHedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD], "LIKE", bah.amacHedefKod.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(bah.temaAmacKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TEMAAMACKOD], "=", bah.temaAmacKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(bah.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + bah.aciklama + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bah.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SIRA];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ACIKLAMA];

            return veriErisim.DataReaderDondur(sql);
        }

        private string KodUret(Birim_AmacHedef bah, bool amac)
        {
            EnumSPGenelHavuzTur tur = amac ? EnumSPGenelHavuzTur.Amac : EnumSPGenelHavuzTur.Hedef;

            string sql = "SELECT " + VeriErisim.IslecBul("MAX") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bah.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bah.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD], "LIKE", ((int)tur).ToString().PadLeft(2, '0') + "%"));

            string sayacStr = veriErisim.TekKolonSQLStr(sql);
            int sayac = 0;
            if (!string.IsNullOrEmpty(sayacStr))
                sayac = OrtakFonksiyonlar.ConvertToInt(sayacStr.Substring(2), 0);

            return ((int)tur).ToString().PadLeft(2, '0') + (sayac + 1).ToString().PadLeft(6, '0');
        }

        public bool KayitVarMi(int donemNo, int orgBirimKod, bool amac)
        {
            EnumSPGenelHavuzTur tur = amac ? EnumSPGenelHavuzTur.Amac : EnumSPGenelHavuzTur.Hedef;

            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD], "LIKE", ((int)tur).ToString().PadLeft(2, '0') + "%"));

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }

        public bool SiraNoKullanilmisMi(Birim_AmacHedef bah, bool amac)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bah.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bah.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD], "LIKE", ((int)(amac ? EnumSPGenelHavuzTur.Amac : EnumSPGenelHavuzTur.Hedef)).ToString().PadLeft(2, '0') + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMACHEDEFKOD], "<>", bah.amacHedefKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRA], "=", bah.sira));

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }
    }
}