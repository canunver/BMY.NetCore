using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeAciklama : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeAciklama(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEACIKLAMA";
        public static string[] ALANLAR = {
                "PROJEKOD",
                "TUR",
                "ACIKLAMA",
        };

        public enum ENUMAlanlar
        {
            PROJEKOD = 0,
            TUR = 1,
            ACIKLAMA = 2,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeAciklama form)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql += VeriErisim.DegerYap(form.projeKod) + ", ";
            sql += VeriErisim.DegerYap(form.tur.kod) + ", ";
            sql += VeriErisim.DegerYap(form.aciklama) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeAciklama form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", T.AD FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN " + TABProjeAciklamaTur.TABLOAD + " T ON T.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TUR];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.tur.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", form.tur.kod));

            sql += " ORDER BY T.SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeAciklama form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.tur.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", form.tur.kod));

            return KomutCalistir(sql, 0);
        }

    }
}
