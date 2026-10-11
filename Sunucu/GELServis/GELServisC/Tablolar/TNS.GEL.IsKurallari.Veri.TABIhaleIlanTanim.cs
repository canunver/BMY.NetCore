using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABGELIhaleIlanTanim : TABOrtak
    {
        public TABGELIhaleIlanTanim(VeriErisim veriErisim) : base(veriErisim) { }
        static string TABLOAD = "GELIHALEILANTANIM";
        enum ENUMAlanlar
        {
            /// <summary>
            /// IHALEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALETURKOD = 0,
            /// <summary>
            /// SIRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 1,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 2,
            /// <summary>
            /// YAKLASIKMALIYETMALHIZMET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETMALHIZMET = 3,
            /// <summary>
            /// YAKLASIKMALIYETMALHIZMETUST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETMALHIZMETUST = 4,
            /// <summary>
            /// YAKLASIKMALIYETYAPIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETYAPIM = 5,
            /// <summary>
            /// YAKLASIKMALIYETYAPIMUST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETYAPIMUST = 6,
            /// <summary>
            /// ZAMAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZAMAN = 7,
            /// <summary>
            /// ZAMANBASLAMATUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZAMANBASLAMATUR = 8,
        }
        static string[] ALANLAR = { "IHALETURKOD", "KOD", "ACIKLAMA", "YAKLASIKMALIYETMALHIZMET", "YAKLASIKMALIYETMALHIZMETUST", "YAKLASIKMALIYETYAPIM", "YAKLASIKMALIYETYAPIMUST", "ZAMAN", "ZAMANBASLAMATUR" };

        public IDataReader Listele(int ihaleKod, int sira)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (ihaleKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", ihaleKod));

                if (sira > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", sira));
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.ihaleKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleMaddeSira) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetMalHizmet) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetMalHizmetUst) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetYapim) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetYapimUst) + ",";
            sql += VeriErisim.DegerYap(bilgi.zaman) + ",";
            sql += VeriErisim.DegerYap(bilgi.zamanBaslamaTur) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.ihaleMaddeSira));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int KayitVarmi(IhaleIlanTanim bilgi)
        {

            string sql = "";
            sql = "SELECT count(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.ihaleMaddeSira));
            return veriErisim.TekKolonSQLInt(sql);

        }

        public Sonuc Guncelle(IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], bilgi.ihaleKod) +",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMET], bilgi.yaklasikMaliyetMalHizmet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMETUST], bilgi.yaklasikMaliyetMalHizmetUst) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIM], bilgi.yaklasikMaliyetYapim) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIMUST], bilgi.yaklasikMaliyetYapimUst) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMAN], bilgi.zaman) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMANBASLAMATUR], bilgi.zamanBaslamaTur) ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.ihaleMaddeSira));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public int KayitSiraNoAl(IhaleIlanTanim bilgi)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.KOD + "),0)+1 AS MAXSIRANO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALETURKOD], "=", bilgi.ihaleKod));
            return veriErisim.TekKolonSQLInt(sql);
        }
    }

    public class TABHRCIhaleIlanTanim : TABOrtak
    {
        public TABHRCIhaleIlanTanim(VeriErisim veriErisim) : base(veriErisim) { }
        static string TABLOAD = "HRCIHALEILANTANIM";
        enum ENUMAlanlar
        {
            /// <summary>
            /// IHALEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            IHALEKOD = 0,
            /// <summary>
            /// SIRA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            SIRA = 1,
            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 2,
            /// <summary>
            /// YAKLASIKMALIYETMALHIZMET alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETMALHIZMET = 3,
            /// <summary>
            /// YAKLASIKMALIYETMALHIZMETUST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETMALHIZMETUST = 4,
            /// <summary>
            /// YAKLASIKMALIYETYAPIM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETYAPIM = 5,
            /// <summary>
            /// YAKLASIKMALIYETYAPIMUST alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YAKLASIKMALIYETYAPIMUST = 6,
            /// <summary>
            /// ZAMAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZAMAN = 7,
            /// <summary>
            /// ZAMANBASLAMATUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ZAMANBASLAMATUR = 8,
        }
        static string[] ALANLAR = { "IHALEKOD", "SIRA", "ACIKLAMA", "YAKLASIKMALIYETMALHIZMET", "YAKLASIKMALIYETMALHIZMETUST", "YAKLASIKMALIYETYAPIM", "YAKLASIKMALIYETYAPIMUST", "ZAMAN", "ZAMANBASLAMATUR" };

        public IDataReader Listele(int ihaleKod, int sira)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (ihaleKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", ihaleKod));

                if (sira > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", sira));
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRA] + " ASC ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.ihaleKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ihaleMaddeSira) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetMalHizmet) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetMalHizmetUst) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetYapim) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaklasikMaliyetYapimUst) + ",";
            sql += VeriErisim.DegerYap(bilgi.zaman) + ",";
            sql += VeriErisim.DegerYap(bilgi.zamanBaslamaTur) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", bilgi.ihaleMaddeSira));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int KayitVarmi(IhaleIlanTanim bilgi)
        {

            string sql = "";
            sql = "SELECT count(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", bilgi.ihaleMaddeSira));
            return veriErisim.TekKolonSQLInt(sql);

        }

        public Sonuc Guncelle(IhaleIlanTanim bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], bilgi.ihaleKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMET], bilgi.yaklasikMaliyetMalHizmet) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETMALHIZMETUST], bilgi.yaklasikMaliyetMalHizmetUst) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIM], bilgi.yaklasikMaliyetYapim) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAKLASIKMALIYETYAPIMUST], bilgi.yaklasikMaliyetYapimUst) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMAN], bilgi.zaman) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ZAMANBASLAMATUR], bilgi.zamanBaslamaTur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", bilgi.ihaleMaddeSira));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public int KayitSiraNoAl(IhaleIlanTanim bilgi)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.SIRA + "),0)+1 AS MAXSIRANO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IHALEKOD], "=", bilgi.ihaleKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRA], "=", bilgi.ihaleMaddeSira));
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
