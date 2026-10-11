using System.Data;
using TNS.Veri;
using System;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABUretimRecetesi : TABOrtak
    {
        public TABUretimRecetesi(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "TASURETIMRECETESI";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ANAURUNHESAPKOD = 1,
            HESAPPLANKOD = 2,
            MIKTAR = 3,

        }
        static string[] ALANLAR = { "KOD", "ANAURUNHESAPKOD", "HESAPPLANKOD", "MIKTAR" };

        public Sonuc Kaydet(UretimRecetesi tk)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tk.kod) + ",";
            sql += VeriErisim.DegerYap(tk.anaUrun.hesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tk.altUrun.hesapKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(tk.miktar) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(string anaUrunKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAURUNHESAPKOD], "=", anaUrunKod.Replace(".", "")));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(UretimRecetesi tk)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", TASHESAPPLAN.ACIKLAMA, TASHESAPPLAN_1.ACIKLAMA";
            sql += " FROM TASURETIMRECETESI ";
            sql += " INNER JOIN TASHESAPPLAN ON TASURETIMRECETESI.ANAURUNHESAPKOD = TASHESAPPLAN.KOD";
            sql += " INNER JOIN TASHESAPPLAN AS TASHESAPPLAN_1 ON TASURETIMRECETESI.HESAPPLANKOD = TASHESAPPLAN_1.KOD";

            string kosul = string.Empty;

            if (tk.anaUrun.hesapKod.Replace(".", "") != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAURUNHESAPKOD], "=", tk.anaUrun.hesapKod.Replace(".", "")));

            if (tk.altUrun.hesapKod.Replace(".", "") != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "=", tk.altUrun.hesapKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY TASHESAPPLAN.ACIKLAMA";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}