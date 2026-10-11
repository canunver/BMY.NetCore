using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeUrun : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeUrun(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEURUN";
        public static string[] ALANLAR = {
                "PROJEKOD",
                "URUNKOD",
        };

        public enum ENUMAlanlar
        {
            PROJEKOD = 0,
            URUNKOD = 1,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeUrun form)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.projeKod) + ", ";
            sql += VeriErisim.DegerYap(form.urunKod) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeUrun form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", U.AD, G.AD FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN " + TABUrun.TABLOAD + " U ON U.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.URUNKOD];
            sql += " LEFT OUTER JOIN BHMURUNGRUP G ON G.KOD = U." + TABUrun.ALANLAR[(int)TABUrun.ENUMAlanlar.GRUPKOD];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.urunKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.URUNKOD], "=", form.urunKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeUrun form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.urunKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.URUNKOD], "=", form.urunKod));

            return KomutCalistir(sql, 0);
        }

    }
}
