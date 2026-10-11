using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimHedefFaaliyet : TABOrtak
    {
        public TABBirimHedefFaaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMHEDEFFAALIYET";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            HEDEFKOD = 2,
            FAALIYETKOD = 3
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "HEDEFKOD", "FAALIYETKOD" };

        public Sonuc Kaydet(BirimHedefFaaliyet bhf)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bhf.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bhf.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bhf.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bhf.faaliyetKod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimHedefFaaliyet bhf)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bhf.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bhf.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", bhf.hedefKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimHedefFaaliyet bhf, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bhf.donem != null && bhf.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bhf.donem.donemNo);
            if (!sorgu && bhf.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bhf.orgBirimKod));
            if (!string.IsNullOrEmpty(bhf.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", bhf.hedefKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(bhf.faaliyetKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", bhf.faaliyetKod.Replace(".", "")));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bhf.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc BirimFaaliyetAktar(int donemNo, int orgBirimKod)
        {
            string sql = "INSERT INTO " + TABBirimGenelHavuz.TABLOAD + " (" + VeriErisim.TumAlanlar("", TABBirimGenelHavuz.ALANLAR) + ") ";
            sql += "SELECT DISTINCT " + ALANLAR[(int)ENUMAlanlar.DONEMNO];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD];
            sql += ", " + ALANLAR[(int)ENUMAlanlar.FAALIYETKOD];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));

            return KomutCalistir(sql, 0);
        }
    }
}