using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimGenelHavuz : TABOrtak
    {
        public TABBirimGenelHavuz(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMGENELHAVUZ";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            GENELHAVUZKOD = 2
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "GENELHAVUZKOD" };

        public Sonuc Kaydet(Birim_SpGenelHavuz birimGenelHavuz)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(birimGenelHavuz.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(birimGenelHavuz.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(birimGenelHavuz.spGenelHavuz.kod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Birim_SpGenelHavuz birimGenelHavuz)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", birimGenelHavuz.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", birimGenelHavuz.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GENELHAVUZKOD], "LIKE", birimGenelHavuz.spGenelHavuz.kod.Substring(0, 2).Replace(".", "") + "%"));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(Birim_SpGenelHavuz birimGenelHavuz, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (birimGenelHavuz.donem != null && birimGenelHavuz.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", birimGenelHavuz.donem.donemNo);
            if (!sorgu && birimGenelHavuz.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", birimGenelHavuz.orgBirimKod));
            if (birimGenelHavuz.spGenelHavuz != null && !string.IsNullOrEmpty(birimGenelHavuz.spGenelHavuz.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GENELHAVUZKOD], "LIKE", birimGenelHavuz.spGenelHavuz.kod.Replace(".", "") + "%"));

            string bagliSQL = BagliSQLOl(birimGenelHavuz);
            if (!string.IsNullOrEmpty(bagliSQL))
                kosul = VeriErisim.KosulEkle(kosul, "AND", bagliSQL);

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], birimGenelHavuz.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DONEMNO];

            return veriErisim.DataReaderDondur(sql);
        }

        private string BagliSQLOl(Birim_SpGenelHavuz birimGenelHavuz)
        {
            if (birimGenelHavuz.spGenelHavuz.bagListe == null || birimGenelHavuz.spGenelHavuz.bagListe.Length <= 0)
                return string.Empty;

            if (!((int)(EnumSPGenelHavuzIliski)System.Enum.Parse(typeof(EnumSPGenelHavuzIliski), System.Enum.GetName(typeof(EnumSPGenelHavuzTur), OrtakFonksiyonlar.ConvertToInt(birimGenelHavuz.spGenelHavuz.kod.Substring(0, 2), 0))) > 0
                && birimGenelHavuz.donem != null && birimGenelHavuz.donem.donemNo > 0 && birimGenelHavuz.orgBirimKod > 0))
                return string.Empty;

            string icSQL = "SELECT 1 FROM " + TABSpGenelHavuz.TABLOADBAG + " A, " + TABLOAD + " B";
            icSQL += " WHERE A." + TABSpGenelHavuz.ALANLAR[(int)TABSpGenelHavuz.ENUMAlanlar.KOD] + " = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.GENELHAVUZKOD];
            icSQL += " AND B." + ALANLAR[(int)ENUMAlanlar.GENELHAVUZKOD] + " = A." + TABSpGenelHavuz.ALANLARBAG[(int)TABSpGenelHavuz.ENUMAlanlarBag.BAGLIKOD];
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", birimGenelHavuz.donem.donemNo));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", birimGenelHavuz.orgBirimKod));

            string ekKosul = string.Empty;
            foreach (SpGenelHavuzBag bag in birimGenelHavuz.spGenelHavuz.bagListe)
            {
                if (!string.IsNullOrEmpty(ekKosul))
                    ekKosul += " OR ";
                ekKosul += "A." + TABSpGenelHavuz.ALANLARBAG[(int)TABSpGenelHavuz.ENUMAlanlarBag.BAGLIKOD] + " = " + bag.bagliKod.kod;
            }
            if (!string.IsNullOrEmpty(ekKosul))
                icSQL += " AND (" + ekKosul + ")";

            return "EXISTS (" + icSQL + ")";
        }

        public bool KayitVarMi(string kod, int yil)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + " (*) FROM " + TABLOAD;

            string kosul = string.Empty;
            if (yil > 0)
            {
                string icSQL = "SELECT DONEMNO FROM STRDONEM WHERE ";
                icSQL += VeriErisim.KosulOl(TABDonem.TABLOAD, "BASLAMAYIL", "<=", yil);
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABDonem.TABLOAD, "BITISYIL", ">=", yil));
                kosul = VeriErisim.KosulEkle(kosul, "AND", " DONEMNO = (" + icSQL + ")");
            }

            if (!string.IsNullOrEmpty(kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GENELHAVUZKOD], "=", kod.Replace(".", "")));
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }
    }
}