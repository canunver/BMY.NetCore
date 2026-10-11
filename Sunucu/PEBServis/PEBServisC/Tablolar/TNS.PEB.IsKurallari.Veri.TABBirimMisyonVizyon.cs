using System.Data;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABBirimMisyonVizyon : TABOrtak
    {
        public TABBirimMisyonVizyon(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRBIRIMMISYONVIZYON";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            TUR = 2,
            SIRANO = 3,
            AD = 4,
            ACIKLAMA = 5
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "TUR", "SIRANO", "AD", "ACIKLAMA" };

        public Sonuc Kaydet(BirimMisyonVizyon MisyonVizyon)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(MisyonVizyon.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(MisyonVizyon.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(MisyonVizyon.tur) + ",";
            sql += VeriErisim.DegerYap(MisyonVizyon.siraNo) + ",";
            sql += VeriErisim.DegerYap(MisyonVizyon.ad) + ",";
            sql += VeriErisim.DegerYap(MisyonVizyon.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(BirimMisyonVizyon misyonVizyon)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", misyonVizyon.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", misyonVizyon.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", misyonVizyon.tur));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(BirimMisyonVizyon misyonVizyon, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (misyonVizyon.donem != null && misyonVizyon.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", misyonVizyon.donem.donemNo));
            if (!sorgu && misyonVizyon.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", misyonVizyon.orgBirimKod));
            if (misyonVizyon.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", misyonVizyon.tur));
            if (!string.IsNullOrEmpty(misyonVizyon.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + misyonVizyon.ad + "%"));
            if (!string.IsNullOrEmpty(misyonVizyon.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + misyonVizyon.aciklama + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], misyonVizyon.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}