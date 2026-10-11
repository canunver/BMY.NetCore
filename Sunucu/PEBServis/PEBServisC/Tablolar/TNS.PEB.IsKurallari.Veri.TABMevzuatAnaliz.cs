using System.Data;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABMevzuatAnaliz : TABOrtak
    {
        public TABMevzuatAnaliz(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRMEVZUATANALIZ";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            SIRANO = 2,
            ACIKLAMA = 3
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "SIRANO", "ACIKLAMA" };

        public Sonuc Kaydet(MevzuatAnaliz mevzuat)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(mevzuat.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(mevzuat.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(mevzuat.siraNo) + ",";
            sql += VeriErisim.DegerYap(mevzuat.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(MevzuatAnaliz mevzuat)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", mevzuat.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", mevzuat.orgBirimKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(MevzuatAnaliz mevzuat, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (mevzuat.donem != null && mevzuat.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", mevzuat.donem.donemNo));
            if (!sorgu && mevzuat.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", mevzuat.orgBirimKod));
            if (!string.IsNullOrEmpty(mevzuat.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + mevzuat.aciklama + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], mevzuat.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}