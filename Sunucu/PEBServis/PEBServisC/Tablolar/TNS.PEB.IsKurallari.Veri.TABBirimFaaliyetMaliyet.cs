using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimFaaliyetMaliyet : TABOrtak
    {
        public TABBirimFaaliyetMaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMFAALIYETMALIYET";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            FAALIYETKOD = 2,
            YIL = 3,
            EKOKOD = 4,
            MALIYETTUR = 5,
            MALIYET = 6
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "FAALIYETKOD", "YIL", "EKOKOD", "MALIYETTUR", "MALIYET" };

        public Sonuc Kaydet(BirimFaaliyetMaliyet bfm)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bfm.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(bfm.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(bfm.faaliyetKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bfm.yil) + ",";
            sql += VeriErisim.DegerYap(bfm.ekoKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(bfm.maliyetTur) + ",";
            sql += VeriErisim.DegerYap(bfm.maliyet) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimFaaliyetMaliyet bfm)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bfm.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bfm.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", bfm.faaliyetKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimFaaliyetMaliyet bfm, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (bfm.donem != null && bfm.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", bfm.donem.donemNo);
            if (!sorgu && bfm.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", bfm.orgBirimKod));
            if (!string.IsNullOrEmpty(bfm.faaliyetKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "LIKE", bfm.faaliyetKod.Replace(".", "") + "%"));
            if (bfm.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", bfm.yil));
            if (!string.IsNullOrEmpty(bfm.ekoKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", bfm.ekoKod.Replace(".", "") + "%"));
            if (bfm.maliyetTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MALIYETTUR], "=", bfm.maliyetTur));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], bfm.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.FAALIYETKOD];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}