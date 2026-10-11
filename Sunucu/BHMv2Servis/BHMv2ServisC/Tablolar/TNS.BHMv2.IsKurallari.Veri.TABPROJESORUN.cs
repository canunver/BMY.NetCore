using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeSorun : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeSorun(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJESORUN";
        public static string[] ALANLAR = {
                "KOD",
                "PROJEKOD",
                "TARIH",
                "SORUN",
                "COZUMONERISI",
        };

        public enum ENUMAlanlar
        {
            KOD = 0,
            PROJEKOD = 1,
            TARIH = 2,
            SORUN = 3,
            COZUMONERISI = 4,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeSorun form)
        {
            string sql = "";

            if (form.kod == "")
            {
                form.kod = Guid.NewGuid().ToString();
                form.tarih = new TNSDateTime(DateTime.Now);
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

                sql += VeriErisim.DegerYap(form.kod) + ", ";
                sql += VeriErisim.DegerYap(form.projeKod) + ", ";
                sql += VeriErisim.DegerYap(form.tarih) + ", ";
                sql += VeriErisim.DegerYap(form.sorun) + ", ";
                sql += VeriErisim.DegerYap(form.cozumOnerisi) + ") ";

            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUN], form.sorun) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.COZUMONERISI], form.cozumOnerisi);

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
        public IDataReader Listele(ProjeSorun form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeSorun form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 0);
        }

    }
}
