using System.Data;
using TNS.Veri;

namespace TNS.TRM.IsKurallari.Veri
{

    public class TABButce : TABOrtak
    {
        public TABButce(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "TRMBUTCE";
        public enum ENUMAlanlar : int
        {
            ETAPNO = 0,
            ILKOD = 1,
            TIP = 2,
            YATIRIMTUR = 3,
            BUTCE = 4,
            KATSAYI = 5,
            TALEPEDILEN = 6,
        }
        public static string[] ALANLAR = { "ETAPNO", "ILKOD", "TIP", "YATIRIMTUR", "BUTCE", "KATSAYI", "TALEPEDILEN" };

        public Sonuc Kaydet(Butce form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.etapNo) + ",";
            sql += VeriErisim.DegerYap(form.ilKod) + ",";
            sql += VeriErisim.DegerYap(form.tip) + ",";
            sql += VeriErisim.DegerYap(form.yatirimOncelikTur) + ",";
            sql += VeriErisim.DegerYap(form.butce) + ",";
            sql += VeriErisim.DegerYap(form.katsayi) + ",";
            sql += VeriErisim.DegerYap(form.talepEdilen) + ")";
            return KomutCalistir(sql, 1);
        }
        public IDataReader Listele(Butce form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + ",ORGCOGRAFYA.AD FROM " + TABLOAD + ",ORGCOGRAFYA";
            kosul = "TRMBUTCE.ILKOD = ORGCOGRAFYA.KOD";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YATIRIMTUR], "=", form.yatirimOncelikTur));


            if (form.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));
            if (form.ilKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod));

            if (form.ilTur > 0)
            {
                if (form.ilTur == 1 || form.ilTur == 3 || form.ilTur == 4 || form.ilTur == 5)
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "=", form.ilTur.ToString()));
                else if (form.ilTur == 2)
                {
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "1"));
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "2"));
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "3"));
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "4"));
                    kosul = VeriErisim.KosulEkle(kosul, " AND", VeriErisim.KosulOl("ORGCOGRAFYA", "GAP", "<>", "5"));
                }
            }

            if (form.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", form.tip));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY ORGCOGRAFYA.AD";
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Sil(Butce form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YATIRIMTUR], "=", form.yatirimOncelikTur));
            return KomutCalistir(sql, 0);
        }
        public double ButceBul(Butce form)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.BUTCE] + ") FROM " + TABLOAD;

            string kosul = string.Empty;
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YATIRIMTUR], "=", form.yatirimOncelikTur));

            if (form.etapNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ETAPNO], "=", form.etapNo));
            if (!string.IsNullOrEmpty(form.ilKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.ILKOD], "=", form.ilKod.Replace(".", "")));
            if (form.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.TIP], "=", form.tip));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLDbl(sql);
        }
    }
}