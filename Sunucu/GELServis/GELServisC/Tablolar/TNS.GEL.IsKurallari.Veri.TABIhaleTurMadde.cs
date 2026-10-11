using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABGELIhaleTurMadde : TABOrtak
    {
        public TABGELIhaleTurMadde(VeriErisim veriErisim) : base(veriErisim) { }
        static string TABLOAD = "GELIHALETURMADDE";
        enum ENUMAlanlar
        {
            KOD = 0,
            IHALETURKOD = 1,
            ADI = 2,
            ACIKLAMA = 3,
            ILANZORUNLUMU = 4,
        }
        static string[] ALANLAR = { "KOD", "IHALETURKOD", "ADI", "ACIKLAMA", "ILANZORUNLUMU" };

        public IDataReader Listele(int ihaleKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (ihaleKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", ihaleKod));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.adi) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilanZorunlu) + ")";
            sonuc=KomutCalistir(sql,1);
            return sonuc;
        }

        public Sonuc Sil(IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int KayitSiraNoAl(IhaleTurMadde bilgi)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.KOD + "),0)+1 AS MAXSIRANO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public int KayitVarmi(IhaleTurMadde bilgi)
        {

            string sql = "";
            sql = "SELECT count(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            return veriErisim.TekKolonSQLInt(sql);

        }

        public Sonuc Guncelle(IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], bilgi.ihaleKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILANZORUNLUMU], bilgi.ilanZorunlu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }
    }

    public class TABHRCIhaleTurMadde : TABOrtak
    {
        public TABHRCIhaleTurMadde(VeriErisim veriErisim) : base(veriErisim) { }
        static string TABLOAD = "HRCIHALETURMADDE";
        enum ENUMAlanlar
        {
            KOD = 0,
            IHALEKOD = 1,
            ADI = 2,
            ACIKLAMA = 3,
            ILANZORUNLUMU = 4,
        }
        static string[] ALANLAR = { "KOD", "IHALEKOD", "ADI", "ACIKLAMA", "ILANZORUNLUMU" };

        public IDataReader Listele(int ihaleKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (ihaleKod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", ihaleKod));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.adi) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.ilanZorunlu) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int KayitSiraNoAl(IhaleTurMadde bilgi)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.KOD + "),0)+1 AS MAXSIRANO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public int KayitVarmi(IhaleTurMadde bilgi)
        {

            string sql = "";
            sql = "SELECT count(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            return veriErisim.TekKolonSQLInt(sql);

        }

        public Sonuc Guncelle(IhaleTurMadde bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], bilgi.ihaleKod) +",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], bilgi.adi)+",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILANZORUNLUMU], bilgi.ilanZorunlu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }
    }
}
