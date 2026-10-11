using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABHavaleFavori : TABOrtak
    {
        public TABHavaleFavori(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRHAVALELISTE";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ISLEMYAPAN = 1,
            SIRANO = 2,
            HAVALEBIRIMKOD = 3,
            HAVALEPERMERNIS = 4,
            HAVALEPERAD = 5,
            HAVALEPERSOYAD = 6
        }
        public static string[] ALANLAR = { "KOD", "ISLEMYAPAN", "SIRANO", "HAVALEBIRIMKOD", "HAVALEPERMERNIS", "HAVALEPERAD", "HAVALEPERSOYAD" };

        public Sonuc Kaydet(HavaleFavori form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.siraNo) + ",";
                sql += VeriErisim.DegerYap(form.havaleBirimKod) + ",";
                sql += VeriErisim.DegerYap(form.havalePersonelMernis) + ",";
                sql += VeriErisim.DegerYap(form.havalePersonelAd) + ",";
                sql += VeriErisim.DegerYap(form.havalePersonelSoyad) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(HavaleFavori form)
        {
            string sql = "";
            if (form.kod > 0)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], form.siraNo);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            }

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(HavaleFavori form)
        {
            string sql = "SELECT TOP(20)" + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));

            sql += "ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO] + " DESC, " + ALANLAR[(int)ENUMAlanlar.KOD] + " DESC";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string[] kodList)
        {
            string sql = "DELETE FROM " + TABLOAD;
            string kosul = "";
            foreach (var kod in kodList)
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            sql += " WHERE " + kosul;

            return KomutCalistir(sql, 0);
        }

        public Sonuc KayitVarsaSil(string islemYapan, string havaleBirimKod, string havalePersonelMernis)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", islemYapan));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAVALEBIRIMKOD], "=", havaleBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HAVALEPERMERNIS], "=", havalePersonelMernis));

            return KomutCalistir(sql, 0);
        }

        public int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}
