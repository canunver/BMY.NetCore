using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABGorevlendirmeEkip : TABOrtak
    {
        public TABGorevlendirmeEkip(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRGOREVLENDIRMEEKIP";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            KOD = 2,
            AD = 3
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "KOD", "AD" };

        public Sonuc Kaydet(GorevlendirmeEkip ekip)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ekip.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(ekip.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(ekip.kod) + ",";
            sql += VeriErisim.DegerYap(ekip.ad) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(GorevlendirmeEkip ekip)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", ekip.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", ekip.orgBirimKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(GorevlendirmeEkip ekip, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (ekip.donem != null && ekip.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", ekip.donem.donemNo);
            if (!sorgu && ekip.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", ekip.orgBirimKod));
            if (ekip.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", ekip.kod));
            if (!string.IsNullOrEmpty(ekip.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + ekip.ad + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], ekip.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}