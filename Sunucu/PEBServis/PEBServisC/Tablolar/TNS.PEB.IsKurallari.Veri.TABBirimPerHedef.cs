using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimPerHedef : TABOrtak
    {
        public TABBirimPerHedef(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMPERHEDEF";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            HEDEFKOD = 2,
            YIL = 3,
            PERHEDEF = 4
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "HEDEFKOD", "YIL", "PERHEDEF" };

        public Sonuc Kaydet(BirimPerHedef bph)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bph.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bph.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bph.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bph.yil) + ",";
            sql += VeriErisim.DegerYap(bph.perHedef) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimPerHedef bph)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bph.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bph.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", bph.hedefKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bph.yil));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(BirimPerHedef bph, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bph.donem != null && bph.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bph.donem.donemNo);
            if (!sorgu && bph.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bph.orgBirimKod));
            if (!string.IsNullOrEmpty(bph.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", bph.hedefKod.Replace(".", "")));
            if (bph.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bph.yil));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bph.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}