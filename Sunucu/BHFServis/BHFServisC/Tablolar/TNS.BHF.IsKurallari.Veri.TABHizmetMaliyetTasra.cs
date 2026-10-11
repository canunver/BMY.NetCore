using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.BKD;

namespace TNS.BHF.IsKurallari.Veri
{
    public class TABHizmetMaliyetTasra : TABOrtak
    {
        public enum ENUMAlanlar : int
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            KURKOD = 3,
            GRUPKOD = 4,
            GERCEKLESME = 5,
            HAZIRAN = 6,
            TAHMIN = 7,
            TAHMIN1 = 8,
            TAHMIN2 = 9,
            DIGERBILGILER = 10,
        }

        static string TABLOAD = "BHFHIZMETMALIYETTESPITTASRA";
        static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "GRUPKOD", "GERCEKLESME", "HAZIRAN", "TAHMIN", "TAHMIN1", "TAHMIN2", "DIGERBILGILER" };

        public TABHizmetMaliyetTasra(VeriErisim veriErisim) : base(veriErisim) { }

        public Sonuc Sil(int yil, int muhasebeKod, string birimKod, string kurumsalKod)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurumsalKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        public Sonuc Ekle(int yil, int muhasebeKod, string birimKod, string kurumsalKod, string grupKod, double gerceklesme, double haziran, double tahmin, double tahmin1, double tahmin2, string digerBilgiler)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(birimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(grupKod) + ", ";
            sql = sql + VeriErisim.DegerYap(gerceklesme) + ", ";
            sql = sql + VeriErisim.DegerYap(haziran) + ", ";
            sql = sql + VeriErisim.DegerYap(tahmin) + ", ";
            sql = sql + VeriErisim.DegerYap(tahmin1) + ", ";
            sql = sql + VeriErisim.DegerYap(tahmin2) + ", ";
            sql = sql + VeriErisim.DegerYap(digerBilgiler) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader DegerleriAl(int yil, int muhasebeKod, string birimKod, string kurumsalKod, string grupKod)
        {
            string sql;
            sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.GERCEKLESME] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.HAZIRAN] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.TAHMIN] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.TAHMIN1] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.TAHMIN2] + ") ";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (muhasebeKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }

            if (kurumsalKod.Length == 2)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], ">=", kurumsalKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "<", kurumsalKod + BKD.Arac.BagimsizKurumBaslangic()));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurumsalKod + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], " LIKE ", grupKod + "%"));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DegerleriAlDiger(int yil, int muhasebeKod, string birimKod, string kurumsalKod, string grupKod)
        {
            string sql;
            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + ALANLAR[(int)ENUMAlanlar.GRUPKOD] + ", " + ALANLAR[(int)ENUMAlanlar.GERCEKLESME] + ", " + ALANLAR[(int)ENUMAlanlar.HAZIRAN] + ", " + ALANLAR[(int)ENUMAlanlar.TAHMIN] + ", " + ALANLAR[(int)ENUMAlanlar.TAHMIN1] + ", " + ALANLAR[(int)ENUMAlanlar.TAHMIN2] + ", " + ALANLAR[(int)ENUMAlanlar.DIGERBILGILER];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (muhasebeKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
            }

            if (kurumsalKod.Length == 2)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], ">", kurumsalKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "<", kurumsalKod + BKD.Arac.BagimsizKurumBaslangic()));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurumsalKod + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], " LIKE ", grupKod + "%"));
            sql += "ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
