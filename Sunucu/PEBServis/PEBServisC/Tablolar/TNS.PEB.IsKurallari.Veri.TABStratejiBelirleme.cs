using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABStratejiBelirleme : TABOrtak
    {
        public TABStratejiBelirleme(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRSTRATEJIBELIRLEME";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            HEDEFKOD = 2,
            SIRANO = 3,
            STRATEJIAD = 4,
            STRATEJIACIKLAMA = 5
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "HEDEFKOD", "SIRANO", "STRATEJIAD", "STRATEJIACIKLAMA" };

        public Sonuc Kaydet(StratejiBelirleme sb)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sb.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(sb.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(sb.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(sb.siraNo) + ",";
            sql += VeriErisim.DegerYap(sb.stratejiAd) + ",";
            sql += VeriErisim.DegerYap(sb.stratejiAciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(StratejiBelirleme sb, string amacKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", sb.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", sb.orgBirimKod));

            if (!string.IsNullOrEmpty(amacKod))
            {
                string icSQL = "SELECT 1 FROM " + TABBirimAmacHedef.TABLOAD;
                icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DONEMNO] + "=" + TABBirimAmacHedef.TABLOAD + "." + TABBirimAmacHedef.ALANLAR[(int)TABBirimAmacHedef.ENUMAlanlar.DONEMNO];
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD] + "=" + TABBirimAmacHedef.TABLOAD + "." + TABBirimAmacHedef.ALANLAR[(int)TABBirimAmacHedef.ENUMAlanlar.ORGBIRIMKOD];
                icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HEDEFKOD] + "=" + TABBirimAmacHedef.TABLOAD + "." + TABBirimAmacHedef.ALANLAR[(int)TABBirimAmacHedef.ENUMAlanlar.AMACHEDEFKOD];
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl(TABBirimAmacHedef.TABLOAD, TABBirimAmacHedef.ALANLAR[(int)TABBirimAmacHedef.ENUMAlanlar.TEMAAMACKOD], "=", amacKod.Replace(".", "")));

                sql = VeriErisim.KosulEkle(sql, "AND", "EXISTS (" + icSQL + ")");
            }

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(StratejiBelirleme sb, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (sb.donem != null && sb.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", sb.donem.donemNo);
            if (!sorgu && sb.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", sb.orgBirimKod));
            if (!string.IsNullOrEmpty(sb.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "LIKE", sb.hedefKod.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(sb.stratejiAd))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.STRATEJIAD], "LIKE", "%" + sb.stratejiAd + "%"));
            if (!string.IsNullOrEmpty(sb.stratejiAciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.STRATEJIACIKLAMA], "LIKE", "%" + sb.stratejiAciklama + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], sb.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}