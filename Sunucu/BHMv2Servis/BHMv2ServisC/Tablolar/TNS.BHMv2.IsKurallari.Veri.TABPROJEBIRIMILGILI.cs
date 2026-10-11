using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    /*
    public class TABProjeBirimIlgili : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeBirimIlgili(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEBIRIMILGILI";
        public static string[] ALANLAR = {
                "PROJEKOD",
                "BIRIMKOD",
                "MUHASEBEKOD",
                "KURKOD",
                "AD",
        };

        public enum ENUMAlanlar
        {
            PROJEKOD = 0,
            BIRIMKOD = 1,
            MUHASEBEKOD = 2,
            KURKOD = 3,
            AD = 4,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeBirimIlgili form)
        {
            Sonuc sonuc = new Sonuc();

            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql += VeriErisim.DegerYap(form.projeKod) + ", ";
            sql += VeriErisim.DegerYap(form.birimKod) + ", ";
            sql += VeriErisim.DegerYap(form.muhasebeKod) + ", ";
            sql += VeriErisim.DegerYap(form.kurKod) + ", ";
            sql += VeriErisim.DegerYap(form.ad) + ") ";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", form.kurKod));

                sonuc = KomutCalistir(sql, 1);

            }
            return sonuc;
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeBirimIlgili form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.birimKod) || form.muhasebeKod > 0 || !string.IsNullOrWhiteSpace(form.kurKod))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", form.kurKod));
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeBirimIlgili form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.birimKod) || form.muhasebeKod > 0 || !string.IsNullOrWhiteSpace(form.kurKod))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", form.birimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", form.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", form.kurKod));
            }

            return KomutCalistir(sql, 0);
        }

    }
*/
}
