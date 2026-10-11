using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimHedefKoordineBirim : TABOrtak
    {
        public TABBirimHedefKoordineBirim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMHEDEFKOORDINEBIRIM";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            HEDEFKOD = 2,
            BIRIMKOD = 3,
            ACIKLAMA = 4,
            SIRANO = 5
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "HEDEFKOD", "BIRIMKOD", "ACIKLAMA", "SIRANO" };

        public Sonuc Kaydet(BirimHedefKoordineBirim d)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(d.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(d.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(d.hedefKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(d.birimKod) + ",";
            sql += VeriErisim.DegerYap(d.aciklama) + ",";
            sql += VeriErisim.DegerYap(d.siraNo) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(BirimHedefKoordineBirim d)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", d.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", d.hedefKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimHedefKoordineBirim d, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (d.donem != null && d.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donem.donemNo);
            if (!sorgu && d.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", d.orgBirimKod));
            if (!string.IsNullOrEmpty(d.hedefKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HEDEFKOD], "=", d.hedefKod.Replace(".", "")));
            if (d.birimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", d.birimKod));
            if (!string.IsNullOrEmpty(d.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + d.aciklama + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], d.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}