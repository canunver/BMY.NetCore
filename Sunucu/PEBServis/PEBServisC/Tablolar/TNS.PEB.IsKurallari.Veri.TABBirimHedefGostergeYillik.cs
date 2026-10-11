using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimHedefGostergeYillik : TABOrtak
    {
        public TABBirimHedefGostergeYillik(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMHEDEFGOSTERGEYILLIK";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            HEDEFKOD = 2,
            GOSTERGEKOD = 3,
            YIL = 4,
            HEDEFLENENDEGER = 5,
            TAHMINDEGER = 6
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "HEDEFKOD", "GOSTERGEKOD", "YIL", "HEDEFLENENDEGER", "TAHMINDEGER" };

        public Sonuc Kaydet(BirimHedefGostergeYillik bhgy)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bhgy.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bhgy.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bhgy.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bhgy.gostergeKod) + ",";
            sql += VeriErisim.DegerYap(bhgy.yil) + ",";
            sql += VeriErisim.DegerYap(bhgy.hedeflenenDeger) + ",";
            sql += VeriErisim.DegerYap(bhgy.tahminDeger) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimHedefGostergeYillik bhgy)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bhgy.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bhgy.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", bhgy.hedefKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bhgy.yil));
            if (bhgy.gostergeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", bhgy.gostergeKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimHedefGostergeYillik bhgy, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bhgy.donem != null && bhgy.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bhgy.donem.donemNo);
            if (!sorgu && bhgy.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bhgy.orgBirimKod));
            if (!string.IsNullOrEmpty(bhgy.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", bhgy.hedefKod.Replace(".", "")));
            if (bhgy.gostergeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", bhgy.gostergeKod));
            if (bhgy.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bhgy.yil));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bhgy.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}