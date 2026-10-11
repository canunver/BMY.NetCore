using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeFaaliyetGosterge : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeFaaliyetGosterge(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEFAALIYETGOSTERGE";
        public static string[] ALANLAR = {
                "KOD",
                "FAALIYETKOD",
                "YIL",
                "GOSTERGE",
                "BIRIM",
                "PLANLANANDEGER",
        };

        public enum ENUMAlanlar
        {
            KOD = 0,
            FAALIYETKOD = 1,
            YIL = 2,
            GOSTERGE = 3,
            BIRIM = 4,
            PLANLANANDEGER = 5,
        }

        public static string TABLOADGERC = "BHMPROJEFAALIYETGOSTERGEGERC";
        public enum ENUMAlanlarGerc
        {
            GOSKOD = 0,
            YIL = 1,
            AY = 2,
            GERCEKLESME = 3,
        }

        public static string[] ALANLARGERC = {
            "GOSKOD",
            "YIL",
            "AY",
            "GERCEKLESME",
        };

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeFaaliyetGosterge form)
        {
            string sql;

            if (form.kod == "")
            {
                form.kod = Guid.NewGuid().ToString();
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

                sql += VeriErisim.DegerYap(form.kod) + ", ";
                sql += VeriErisim.DegerYap(form.faaliyetKod) + ", ";
                sql += VeriErisim.DegerYap(form.yil) + ", ";
                sql += VeriErisim.DegerYap(form.gosterge) + ", ";
                sql += VeriErisim.DegerYap(form.birim) + ", ";
                sql += VeriErisim.DegerYap(form.planlananDeger) + ") ";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], form.yil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GOSTERGE], form.gosterge) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], form.birim) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PLANLANANDEGER], form.planlananDeger);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeFaaliyetGosterge form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", F.AD FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN " + TABProjeFaaliyet.TABLOAD + " F ON F.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FAALIYETKOD];

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod));

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeFaaliyetGosterge form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", form.faaliyetKod));

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader GerceklesmeListele(int yil, int ay, string faaliyetKod)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("GOS", ALANLAR)}, {VeriErisim.TumAlanlar("GERC", ALANLARGERC)} FROM {TABLOAD} GOS ";
            sql += $" LEFT OUTER JOIN {TABLOADGERC} GERC ON GERC.GOSKOD = GOS.KOD AND GERC.YIL = GOS.YIL AND GERC.AY={ay}";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("GOS", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", faaliyetKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("GOS", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GerceklesmeEkle(ProjeFaaliyetGosterge gost)
        {
            string sql = "INSERT INTO " + TABLOADGERC + "(" + VeriErisim.TumAlanlar("", ALANLARGERC) + ") VALUES (";

            sql += VeriErisim.DegerYap(gost.kod) + ", ";
            sql += VeriErisim.DegerYap(gost.yil) + ", ";
            sql += VeriErisim.DegerYap(gost.ay) + ", ";
            sql += VeriErisim.DegerYap(gost.gerceklesenDeger) + ") ";

            return KomutCalistir(sql, 0);
        }

        public Sonuc GerceklesmeGuncelle(ProjeFaaliyetGosterge gost)
        {
            string sql = "UPDATE " + TABLOADGERC + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARGERC[(int)ENUMAlanlarGerc.GERCEKLESME], gost.gerceklesenDeger);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERC[(int)ENUMAlanlarGerc.GOSKOD], "=", gost.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERC[(int)ENUMAlanlarGerc.YIL], "=", gost.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERC[(int)ENUMAlanlarGerc.AY], "=", gost.ay));
            return KomutCalistir(sql, 0);
        }
    }
}
