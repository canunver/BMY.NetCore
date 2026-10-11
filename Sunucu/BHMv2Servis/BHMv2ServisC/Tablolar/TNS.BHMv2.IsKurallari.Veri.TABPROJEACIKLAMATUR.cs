using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeAciklamaTur : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProjeAciklamaTur"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeAciklamaTur(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEACIKLAMATUR";
        public static string[] ALANLAR = {
                "KOD",
                "AD",
                "TUR",
                "SIRANO",
        };

        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            TUR = 2,
            SIRANO = 3,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeAciklamaTur form)
        {
            string sql = "";

            if (form.kod == "")
            {
                form.kod = Guid.NewGuid().ToString();
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ", ";
                sql += VeriErisim.DegerYap(form.ad) + ", ";
                sql += VeriErisim.DegerYap((int)form.tur) + ", ";
                sql += VeriErisim.DegerYap(form.siraNo) + ") ";

            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], (int)form.tur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], form.siraNo);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeAciklamaTur form)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrWhiteSpace(form.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }


    }
}
