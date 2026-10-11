using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimHizmetPaydasIliski : TABOrtak
    {
        public TABBirimHizmetPaydasIliski(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMHIZMETPAYDASILISKI";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            PAYDASSIRANO = 2,
            PAYDASSINIF = 3,
            HIZMETKOD = 4,
            YARARLANMA = 5,
            ACIKLAMA = 6
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "PAYDASSIRANO", "PAYDASSINIF", "HIZMETKOD", "YARARLANMA", "ACIKLAMA" };

        public Sonuc Kaydet(Birim_SpGenelHavuz_Paydas_Iliski bhpi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bhpi.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bhpi.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bhpi.paydas.siraNo) + ",";
            sql += VeriErisim.DegerYap(bhpi.paydas.sinif) + ",";
            sql += VeriErisim.DegerYap(bhpi.spGenelHavuz.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bhpi.yararlanma) + ",";
            sql += VeriErisim.DegerYap(bhpi.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Birim_SpGenelHavuz_Paydas_Iliski bhpi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bhpi.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bhpi.orgBirimKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(Birim_SpGenelHavuz_Paydas_Iliski bhpi, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bhpi.donem != null && bhpi.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bhpi.donem.donemNo);
            if (bhpi.orgBirimKod > 0 && (!sorgu || bhpi.paydas != null))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bhpi.orgBirimKod));
            if (bhpi.paydas != null)
            {
                if (bhpi.paydas.siraNo > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PAYDASSIRANO], "=", bhpi.paydas.siraNo));
                if (bhpi.paydas.sinif > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PAYDASSINIF], "=", bhpi.paydas.sinif));
            }
            if (bhpi.spGenelHavuz != null && !string.IsNullOrEmpty(bhpi.spGenelHavuz.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HIZMETKOD], "LIKE", bhpi.spGenelHavuz.kod + "%"));
            if (bhpi.yararlanma > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YARARLANMA], "=", bhpi.yararlanma));
            if (!string.IsNullOrEmpty(bhpi.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + bhpi.aciklama + "%"));

            if (sorgu && bhpi.paydas == null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bhpi.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc PaydasiOlmayanlariSil(int donemNo, int orgBirimKod)
        {
            string icSQL = "SELECT 1 FROM " + TABPaydasAnaliz.TABLOADDETAY;
            icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DONEMNO] + " = " + TABPaydasAnaliz.TABLOADDETAY + "." + TABPaydasAnaliz.ALANLARDETAY[(int)TABPaydasAnaliz.ENUMAlanlarDetay.DONEMNO];
            icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD] + " = " + TABPaydasAnaliz.TABLOADDETAY + "." + TABPaydasAnaliz.ALANLARDETAY[(int)TABPaydasAnaliz.ENUMAlanlarDetay.ORGBIRIMKOD];
            icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PAYDASSIRANO] + " = " + TABPaydasAnaliz.TABLOADDETAY + "." + TABPaydasAnaliz.ALANLARDETAY[(int)TABPaydasAnaliz.ENUMAlanlarDetay.SIRANO];
            icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PAYDASSINIF] + " = " + TABPaydasAnaliz.TABLOADDETAY + "." + TABPaydasAnaliz.ALANLARDETAY[(int)TABPaydasAnaliz.ENUMAlanlarDetay.SINIF];

            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", "NOT EXISTS (" + icSQL + ")");

            return KomutCalistir(sql, 0);
        }
    }
}