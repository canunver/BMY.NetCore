using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABTopluIslem : TABOrtak
    {
        public TABTopluIslem(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "TASTOPLUISLEM";
        public enum ENUMAlanlar
        {
            ID = 0,
            GRUPKOD = 1,
            SICILNO = 2,
            KAYNAKMUH = 3,
            KAYNAKHRC = 4,
            KAYNAKAMB = 5,
            KAYNAKODA = 6,
            KAYNAKTC = 7,
            HEDEFMUH = 8,
            HEDEFHRC = 9,
            HEDEFAMB = 10,
            HEDEFODA = 11,
            HEDEFTC = 12,
            ZIMMETDUS = 13,
            ZIMMETVER = 14,
            TIFDEVIRVER = 15,
            TIFDEVIRAL = 16,
        }
        static string[] ALANLAR = { "ID", "GRUPKOD", "SICILNO", "KAYNAKMUH", "KAYNAKHRC", "KAYNAKAMB", "KAYNAKODA", "KAYNAKTC", "HEDEFMUH", "HEDEFHRC", "HEDEFAMB", "HEDEFODA", "HEDEFTC", "ZIMMETDUS", "ZIMMETVER", "TIFDEVIRVER", "TIFDEVIRAL" };

        static string TABLOADFORM = "TASTOPLUISLEMFORM";
        public enum ENUMAlanlarForm
        {
            GRUPKOD = 0,
            ACIKLAMA = 1,
        }
        static string[] ALANLARFORM = { "GRUPKOD", "ACIKLAMA" };

        public Sonuc Kaydet(TopluIslem t)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (GRUPKOD,SICILNO,KAYNAKMUH,KAYNAKHRC,KAYNAKAMB,KAYNAKODA,KAYNAKTC,HEDEFMUH,HEDEFHRC,HEDEFAMB,HEDEFODA,HEDEFTC) VALUES (";
            sql += VeriErisim.DegerYap(t.grupKod) + ",";
            sql += VeriErisim.DegerYap(t.sicilNo) + ",";
            sql += VeriErisim.DegerYap(t.kaynakMuh) + ",";
            sql += VeriErisim.DegerYap(t.kaynakHrc) + ",";
            sql += VeriErisim.DegerYap(t.kaynakAmb) + ",";
            sql += VeriErisim.DegerYap(t.kaynakOda) + ",";
            sql += VeriErisim.DegerYap(t.kaynakTC) + ",";
            sql += VeriErisim.DegerYap(t.hedefMuh) + ",";
            sql += VeriErisim.DegerYap(t.hedefHrc) + ",";
            sql += VeriErisim.DegerYap(t.hedefAmb) + ",";
            sql += VeriErisim.DegerYap(t.hedefOda) + ",";
            sql += VeriErisim.DegerYap(t.hedefTC) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetForm(TopluIslemForm t)
        {
            string sql = "";

            sql = "INSERT INTO " + TABLOADFORM + " (" + VeriErisim.TumAlanlar("", ALANLARFORM) + ") VALUES (";
            sql += VeriErisim.DegerYap(t.grupKod) + ",";
            sql += VeriErisim.DegerYap(t.aciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(TopluIslem t)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";

            if (t.zimmetDusBelgeNo != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZIMMETDUS], t.zimmetDusBelgeNo);
            if (t.zimmetVerBelgeNo != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZIMMETVER], t.zimmetVerBelgeNo);
            if (t.devirAlBelgeNo != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIFDEVIRAL], t.devirAlBelgeNo);
            if (t.devirVerBelgeNo != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIFDEVIRVER], t.devirVerBelgeNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ID], "=", t.id));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleForm(TopluIslemForm t)
        {
            string sql = "";

            sql = "UPDATE " + TABLOADFORM + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARFORM[(int)ENUMAlanlarForm.ACIKLAMA], t.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.GRUPKOD], "=", t.grupKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int grupKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", grupKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SilForm(int grupKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOADFORM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.GRUPKOD], "=", grupKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(TopluIslem t)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (t.id > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ID], "=", t.id));

            if (t.grupKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], "=", t.grupKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KAYNAKMUH] + "," + ALANLAR[(int)ENUMAlanlar.KAYNAKHRC] + "," + ALANLAR[(int)ENUMAlanlar.KAYNAKAMB] + "," + ALANLAR[(int)ENUMAlanlar.KAYNAKTC] + "," + ALANLAR[(int)ENUMAlanlar.KAYNAKODA];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleForm(TopluIslemForm t)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARFORM) + " FROM " + TABLOADFORM;

            if (t.grupKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.GRUPKOD], "=", t.grupKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARFORM[(int)ENUMAlanlarForm.ACIKLAMA];

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader DetaydanGrupKodlariniListele()
        {
            string sql = "SELECT GRUPKOD FROM " + TABLOAD;
            sql += " GROUP BY GRUPKOD";

            return veriErisim.DataReaderDondur(sql);
        }

        public int YeniGrupKoduVer()
        {
            string sql = "";

            sql = "SELECT MAX(GRUPKOD) FROM " + TABLOADFORM;

            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}