using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimFaaliyetAgirlik : TABOrtak
    {
        public TABBirimFaaliyetAgirlik(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMFAALIYETAGIRLIK";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            FAALIYETKOD = 2,
            HEDEFKOD = 3,
            AGIRLIK = 4
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "FAALIYETKOD", "HEDEFKOD", "AGIRLIK" };

        public Sonuc Kaydet(BirimFaaliyetAgirlik bfa)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bfa.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bfa.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bfa.faaliyetKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bfa.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bfa.agirlik) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimFaaliyetAgirlik bfa)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bfa.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bfa.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", bfa.faaliyetKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimFaaliyetAgirlik bfa, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bfa.donem != null && bfa.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bfa.donem.donemNo);
            if (!sorgu && bfa.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bfa.orgBirimKod));
            if (!string.IsNullOrEmpty(bfa.faaliyetKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", bfa.faaliyetKod.Replace(".", "")));
            if (!string.IsNullOrEmpty(bfa.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", bfa.hedefKod.Replace(".", "")));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bfa.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}