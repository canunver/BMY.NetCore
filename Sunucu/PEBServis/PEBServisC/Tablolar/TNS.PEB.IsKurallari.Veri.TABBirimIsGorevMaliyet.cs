using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimIsGorevMaliyet : TABOrtak
    {
        public TABBirimIsGorevMaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMISGOREVMALIYET";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            ISGOREVKOD = 2,
            YIL = 3,
            MALIYET = 4,
            FAALIYETKOD = 5
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "ISGOREVKOD", "YIL", "MALIYET", "FAALIYETKOD" };

        public Sonuc Kaydet(BirimIsGorevMaliyet bigm)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bigm.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bigm.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bigm.isGorevKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bigm.yil) + ",";
            sql += VeriErisim.DegerYap(bigm.maliyet) + ",";
            sql += VeriErisim.DegerYap(bigm.faaliyetKod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimIsGorevMaliyet bigm)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bigm.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bigm.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bigm.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", bigm.faaliyetKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimIsGorevMaliyet bigm, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bigm.donem != null && bigm.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bigm.donem.donemNo);
            if (!sorgu && bigm.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bigm.orgBirimKod));
            if (!string.IsNullOrEmpty(bigm.isGorevKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISGOREVKOD], "LIKE", bigm.isGorevKod.Replace(".", "") + "%"));
            if (bigm.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bigm.yil));
            if (!string.IsNullOrEmpty(bigm.faaliyetKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "LIKE", bigm.faaliyetKod.Replace(".", "") + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bigm.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ISGOREVKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}