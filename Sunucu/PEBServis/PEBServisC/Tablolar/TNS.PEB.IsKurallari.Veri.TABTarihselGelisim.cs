using System.Data;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABTarihselGelisim : TABOrtak
    {
        public TABTarihselGelisim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRTARIHSELGELISIM";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            SIRANO = 2,
            ACIKLAMA = 3
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "SIRANO", "ACIKLAMA" };

        public Sonuc Kaydet(TarihselGelisim tg)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tg.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(tg.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(tg.siraNo) + ",";
            sql += VeriErisim.DegerYap(tg.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(TarihselGelisim tg)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", tg.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", tg.orgBirimKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(TarihselGelisim tg, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (tg.donem != null && tg.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", tg.donem.donemNo));
            if (!sorgu && tg.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", tg.orgBirimKod));
            if (!string.IsNullOrEmpty(tg.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + tg.aciklama + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], tg.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}