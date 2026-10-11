using System.Data;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABDonem : TABOrtak
    {
        public TABDonem(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRDONEM";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            BASLAMAYIL = 1,
            BITISYIL = 2,
            PASIF = 3,
            PASIFACIKLAMA = 4,
            VERSIYON = 5,
            DONEMAD = 6
        }

        public static string[] ALANLAR = { "DONEMNO", "BASLAMAYIL", "BITISYIL", "PASIF", "PASIFACIKLAMA", "VERSIYON", "DONEMAD" };

        public Sonuc Kaydet(Donem d)
        {
            d.donemNo = DonemNoAl();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(d.donemNo) + ",";
            sql += VeriErisim.DegerYap(d.baslamaYili) + ",";
            sql += VeriErisim.DegerYap(d.bitisYili) + ",";
            sql += VeriErisim.DegerYap(d.pasif ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(d.pasifAciklama) + ",";
            sql += VeriErisim.DegerYap(d.versiyon) + ",";
            sql += VeriErisim.DegerYap(d.donemAd) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = d.donemNo.ToString();
            return sonuc;
        }

        public Sonuc Guncelle(Donem d)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], d.baslamaYili) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISYIL], d.bitisYili) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PASIF], d.pasif ? 1 : 0) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PASIFACIKLAMA], d.pasifAciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DONEMAD], d.donemAd);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donemNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = d.donemNo.ToString();
            return sonuc;
        }

        public Sonuc Sil(Donem d)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donemNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(Donem d)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (d.donemNo > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", d.donemNo);
            if (d.baslamaYili > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], "=", d.baslamaYili));
            if (d.bitisYili > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BITISYIL], "=", d.bitisYili));
            if (d.donemNo == -1)
            {
                if (d.pasif)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PASIF], "=", 1));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PASIF], "=", 0));
            }
            if (!string.IsNullOrEmpty(d.pasifAciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PASIFACIKLAMA], "LIKE", "%" + d.pasifAciklama + "%"));
            if (d.versiyon > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.VERSIYON], "=", d.versiyon));
            if (!string.IsNullOrEmpty(d.donemAd))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMAD], "LIKE", "%" + d.donemAd + "%"));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.DONEMNO];

            return veriErisim.DataReaderDondur(sql);
        }

        public bool DonemVarMi(int donemNo)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMNO] };
            int[] intDeger = { donemNo };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        public bool DonemVarMi(Donem d, bool pasif)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.DONEMNO], "<>", d.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND ((", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], "<=", d.baslamaYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BITISYIL], ">=", d.baslamaYili));
            sql = VeriErisim.KosulEkle(sql, ") OR (", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], "<=", d.bitisYili));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.BITISYIL], ">=", d.bitisYili));
            sql = VeriErisim.KosulEkle(sql, ")) AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.PASIF], "=", pasif ? 1 : 0));

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }

        private int DonemNoAl()
        {
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], new string[0], new string[0], new string[0], new int[0]);
        }

        public int VersiyonNoAl(int baslamaYili, int bitisYili)
        {
            string sql = "SELECT MAX(VERSIYON) FROM " + TABLOAD;

            string kosul = string.Empty;
            if (baslamaYili > 0)
                kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], "=", baslamaYili);
            if (bitisYili > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BITISYIL], "=", bitisYili));

            return veriErisim.TekKolonSQLInt(sql + " WHERE " + kosul) + 1;
        }

        public int AktifDonemBul(int yil)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DONEMNO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], "<=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BITISYIL], ">=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PASIF], "=", 0));

            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}