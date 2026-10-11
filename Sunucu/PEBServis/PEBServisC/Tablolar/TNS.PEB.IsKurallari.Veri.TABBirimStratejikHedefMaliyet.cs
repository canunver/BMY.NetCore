using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimStratejikHedefMaliyet : TABOrtak
    {
        public TABBirimStratejikHedefMaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMHEDEFMALIYET";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            HEDEFKOD = 2,
            YIL = 3,
            MALIYET = 4
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "HEDEFKOD", "YIL", "MALIYET" };

        public Sonuc Kaydet(BirimStratejikHedefMaliyet sb)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sb.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(sb.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(sb.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sb.yil) + ",";
            sql += VeriErisim.DegerYap(sb.maliyet) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimStratejikHedefMaliyet sb)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", sb.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", sb.orgBirimKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimStratejikHedefMaliyet sb, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (sb.donem != null && sb.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", sb.donem.donemNo);
            if (!sorgu && sb.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", sb.orgBirimKod));
            if (!string.IsNullOrEmpty(sb.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "LIKE", sb.hedefKod.Replace(".", "") + "%"));
            if (sb.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", sb.yil));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], sb.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.HEDEFKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}