using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimHedef : TABOrtak
    {
        public TABBirimHedef(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMHEDEF";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            HEDEFKOD = 2,
            SORUMLUBIRIMKOD = 3,
            ACIKLAMA = 4
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "HEDEFKOD", "SORUMLUBIRIMKOD", "ACIKLAMA" };

        public Sonuc Kaydet(BirimHedef d)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(d.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(d.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(d.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(d.sorumluBirimKod) + ",";
            sql += VeriErisim.DegerYap(d.aciklama) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Guncelle(BirimHedef d)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD], d.sorumluBirimKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], d.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", d.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", d.hedefKod.Replace(".", "")));

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(BirimHedef d)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", d.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", d.hedefKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimHedef d, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (d.donem != null && d.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donem.donemNo);
            if (!sorgu && d.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", d.orgBirimKod));
            if (!string.IsNullOrEmpty(d.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", d.hedefKod.Replace(".", "")));
            if (d.sorumluBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD], "=", d.sorumluBirimKod));
            if (!string.IsNullOrEmpty(d.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + d.aciklama + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], d.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}