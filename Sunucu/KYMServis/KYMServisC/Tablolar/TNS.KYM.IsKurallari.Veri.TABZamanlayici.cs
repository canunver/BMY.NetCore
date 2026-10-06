using System.Data;
using TNS.Veri;

namespace TNS.KYM.IsKurallari.Veri
{
    public class TABZamanlayici : TABOrtak
    {
        public TABZamanlayici(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "KYMZAMANLAYICI";
        public static string[] ALANLAR = { "KOD", "TUR", "ISLEMADI", "ZAMAN" };
        public enum ENUMAlanlar
        {
            KOD = 0,
            TUR = 1,
            ISLEMADI = 2,
            ZAMAN = 3
        }

        public Sonuc Kaydet(ZamanlayiciTip bilgi)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tur) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.zaman) + ") ";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ZamanlayiciTip bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT TOP 1 " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD + " WITH(XLOCK,ROWLOCK) ";

            if (!string.IsNullOrEmpty(bilgi.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.islemAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMADI], "=", bilgi.islemAdi));

            if (!string.IsNullOrEmpty(bilgi.tur))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ZAMAN] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
