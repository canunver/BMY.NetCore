using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABPlaka : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABFatura"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABPlaka(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MUHPLAKA";
        public static string[] ALANLAR = { "KOD", "PLAKA1", "PLAKA2", "PLAKA3", "ACIKLAMA" };
        public enum ENUMAlanlar
        {
            KOD = 0,
            PLAKA1 = 1,
            PLAKA2 = 2,
            PLAKA3 = 3,
            ACIKLAMA = 4,
        }

        public static string TABLOADSAHIP = "MUHPLAKASAHIP";
        public static string[] ALANLARSAHIP = { "KOD", "PLAKAKOD", "VERGINO", "AD", "ADRES" };
        public enum ENUMAlanlarSahip
        {
            KOD = 0,
            PLAKAKOD = 1,
            VERGINO = 2,
            AD = 3,
            ADRES = 4,
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Plaka bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.plaka1) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.plaka2) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.plaka3) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Guncelles the specified bilgi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Guncelle(Plaka bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PLAKA1], bilgi.plaka1) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PLAKA2], bilgi.plaka2) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PLAKA3], bilgi.plaka3) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(Plaka bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADSAHIP, ALANLARSAHIP);
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN ";
            sql += " MUHPLAKASAHIP ON MUHPLAKA.KOD = MUHPLAKASAHIP.PLAKAKOD ";

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(bilgi.plaka1))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PLAKA1], "=", bilgi.plaka1));

            if (!string.IsNullOrEmpty(bilgi.plaka2))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PLAKA2], "=", bilgi.plaka2));

            if (!string.IsNullOrEmpty(bilgi.plaka3))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PLAKA3], "=", bilgi.plaka3));

            if (!string.IsNullOrEmpty(bilgi.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADSAHIP, ALANLARSAHIP[(int)ENUMAlanlarSahip.VERGINO], "=", bilgi.aciklama));

            if (!string.IsNullOrEmpty(kosul))
            {
                sql += " WHERE " + kosul;
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.PLAKA1] + "," + ALANLAR[(int)ENUMAlanlar.PLAKA2] + "," + ALANLAR[(int)ENUMAlanlar.PLAKA3];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sils the specified bilgi.
        /// </summary>
        /// <param name="kod">The kod.</param>
        /// <returns></returns>
        public Sonuc Sil(int kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Faturas the detay kaydet.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc SahipKaydet(PlakaSahip bilgi)
        {
            string sql = "INSERT INTO " + TABLOADSAHIP + "(" + VeriErisim.TumAlanlar("", ALANLARSAHIP) + ") VALUES(";

            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.plakaKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.vergiNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.ad) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.adres) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SahipGuncelle(PlakaSahip bilgi)
        {
            string sql = "UPDATE " + TABLOADSAHIP + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSAHIP[(int)ENUMAlanlarSahip.VERGINO], bilgi.vergiNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSAHIP[(int)ENUMAlanlarSahip.AD], bilgi.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSAHIP[(int)ENUMAlanlarSahip.ADRES], bilgi.adres);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSAHIP[(int)ENUMAlanlarSahip.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Faturas the detay sil.
        /// </summary>
        /// <param name="plakaKod">The fatura kod.</param>
        /// <returns></returns>
        public Sonuc SahipSil(int plakaKod)
        {
            string sql = "DELETE FROM " + TABLOADSAHIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADSAHIP, ALANLARSAHIP[(int)ENUMAlanlarSahip.PLAKAKOD], "=", plakaKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Ens the buyuk kod.
        /// </summary>
        /// <returns></returns>
        public int EnBuyukKod()
        {
            string sql = "";
            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")";
            sql += " FROM " + TABLOAD;

            int deger = veriErisim.TekKolonSQLInt(sql);

            return deger + 1;
        }

        /// <summary>
        /// Ens the buyuk detay kod.
        /// </summary>
        /// <returns></returns>
        public int EnBuyukSahipKod()
        {
            string sql = "";
            sql = "SELECT MAX(" + ALANLARSAHIP[(int)ENUMAlanlarSahip.KOD] + ")";
            sql += " FROM " + TABLOADSAHIP;

            int deger = veriErisim.TekKolonSQLInt(sql);
            return deger + 1;
        }
    }
}
