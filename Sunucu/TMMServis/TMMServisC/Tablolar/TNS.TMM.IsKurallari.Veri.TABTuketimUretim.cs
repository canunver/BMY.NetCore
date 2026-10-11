using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABTuketimUretim : TABOrtak
    {
        public TABTuketimUretim(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        static string TABLOAD = "TASTUKETIMURETIM";

        public enum ENUMAlanlar : int
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            HARCAMABIRIMKOD = 2,
            BELGENO = 3,
            TUR = 4,
            AMBARKOD = 5,
            BELGETARIHI = 6
        }

        static string[] ALANLAR ={ "YIL", "MUHASEBEKOD", "HARCAMABIRIMKOD",
                                   "BELGENO", "TUR", "AMBARKOD", "BELGETARIHI" };

        public Sonuc Kaydet(TuketimUretim tu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { tu.yil };
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] };
            string[] strDeger = { tu.muhasebe.kod, tu.harcamaBirimi.kod.Replace(".", "") };
            string belgeNo = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], strAlan, strDeger, intAlan, intDeger).ToString().PadLeft(6, '0');

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(tu.yil) + ",";
            sql += VeriErisim.DegerYap(tu.muhasebe.kod) + ",";
            sql += VeriErisim.DegerYap(tu.harcamaBirimi.kod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(belgeNo) + ",";
            sql += VeriErisim.DegerYap(tu.tur) + ",";
            sql += VeriErisim.DegerYap(tu.ambar.kod) + ",";
            sql += VeriErisim.DegerYap(tu.belgeTarihi) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = belgeNo;
            return sonuc;
        }

        public Sonuc Guncelle(TuketimUretim tu)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], tu.tur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], tu.ambar.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BELGETARIHI], tu.belgeTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", tu.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tu.muhasebe.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", tu.harcamaBirimi.kod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tu.belgeNo));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = tu.belgeNo;
            return sonuc;
        }

        public IDataReader Listele(TuketimUretim tu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (tu.yil > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", tu.yil);

            if (!string.IsNullOrEmpty(tu.muhasebe.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", tu.muhasebe.kod));

            if (!string.IsNullOrEmpty(tu.harcamaBirimi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "LIKE", tu.harcamaBirimi.kod.Replace(".", "")));

            if (!string.IsNullOrEmpty(tu.belgeNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGENO], "=", tu.belgeNo));

            if (tu.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", tu.tur));

            if (tu.ambar != null && !string.IsNullOrEmpty(tu.ambar.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", tu.ambar.kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public bool TuketimUretimVarMi(TuketimUretim tu)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] intDeger = { tu.yil };
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], ALANLAR[(int)ENUMAlanlar.BELGENO] };
            string[] strDeger = { tu.muhasebe.kod, tu.harcamaBirimi.kod.Replace(".", ""), tu.belgeNo };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }
    }
}