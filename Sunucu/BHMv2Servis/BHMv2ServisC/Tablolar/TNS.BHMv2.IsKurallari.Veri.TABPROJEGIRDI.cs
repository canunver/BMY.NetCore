using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeGirdi : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeGirdi(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEGIRDI";
        public static string[] ALANLAR = {
                "KOD",
                "PROJEKOD",
                "TUR",
                "GIRDI",
                "BIRIM",
                "MIKTAR",
                "BIRIMMALIYET",
                "TOPLAMMALIYET",
        };

        public enum ENUMAlanlar
        {
            KOD = 0,
            PROJEKOD = 1,
            TUR = 2,
            GIRDI = 3,
            BIRIM = 4,
            MIKTAR = 5,
            BIRIMMALIYET = 6,
            TOPLAMMALIYET = 7,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeGirdi form)
        {
            string sql = "";

            if (form.kod == "")
            {
                form.kod = Guid.NewGuid().ToString();
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

                sql += VeriErisim.DegerYap(form.kod) + ", ";
                sql += VeriErisim.DegerYap(form.projeKod) + ", ";
                sql += VeriErisim.DegerYap((int)form.tur) + ", ";
                sql += VeriErisim.DegerYap(form.girdi) + ", ";
                sql += VeriErisim.DegerYap(form.birim) + ", ";
                sql += VeriErisim.DegerYap(form.miktar) + ", ";
                sql += VeriErisim.DegerYap(form.birimMaliyet) + ", ";
                sql += VeriErisim.DegerYap(form.toplamMaliyet) + ") ";

            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], (int)form.tur) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], form.birim) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], form.miktar) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMMALIYET], form.birimMaliyet) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAMMALIYET], form.toplamMaliyet);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeGirdi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (form.tur != ENUMProjeGirdiTur.TANIMSIZ)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", (int)form.tur));

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeGirdi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 0);
        }

    }
}
