using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{

    public class TABRTFRapor : TABOrtak
    {
        public TABRTFRapor(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADSABLON = "STRRTFRAPORSABLON";
        public static string TABLOADMETIN = "STRRTFRAPORMETIN";

        public enum ENUMAlanlarSABLON : int
        {
            SABLONNO = 0,
            KOD = 1,
            SIRA = 2,
            METIN = 3,
            ICERLEK = 4,
            MERKEZTASRA = 5,
            TUR = 6,
            VARSAYILANMETIN = 7
        }

        public enum ENUMAlanlarMETIN : int
        {
            BIRIMKOD = 0,
            DONEMNO = 1,
            YIL = 2,
            SABLONNO = 3,
            KOD = 4,
            METIN = 5
        }

        /*        
        create table STRRTFRAPORSABLON
            (
            SABLONNO     int,
            KOD          int,
            SIRA         int,
            METIN        varchar(255),
            ICERLEK      int,
            MERKEZTASRA  int,
            TUR          varchar(50),
            VARSAYILANMETIN varchar(5000),
            primary key (SABLONNO, KOD)
            )
            create table STRRTFRAPORMETIN
            (
            BIRIMKOD     int,
            DONEMNO      int,
            YIL          int,
            SABLONNO     int,
            KOD          int,
            METIN        varchar(8000) not null,
            primary key (BIRIMKOD, DONEMNO, YIL, SABLONNO, KOD)
            )

         * */

        public static string[] ALANLARSABLON = { "SABLONNO",
            "KOD",
            "SIRA",
            "METIN",
            "ICERLEK",
            "MERKEZTASRA",
            "TUR", 
            "VARSAYILANMETIN" };

        public static string[] ALANLARMETIN = { "BIRIMKOD", "DONEMNO", "YIL", "SABLONNO", "KOD", "METIN" };

        public Sonuc Kaydet(int sablonNo, RTFOutline rtfOutline)
        {
            string sql = "INSERT INTO " + TABLOADSABLON + " (" + VeriErisim.TumAlanlar("", ALANLARSABLON) + ") VALUES (";
            sql += VeriErisim.DegerYap(sablonNo) + ",";
            sql += VeriErisim.DegerYap(rtfOutline.kod) + ",";
            sql += VeriErisim.DegerYap(rtfOutline.sira) + ",";
            sql += VeriErisim.DegerYap(rtfOutline.metin) + ",";
            sql += VeriErisim.DegerYap(rtfOutline.icerlek) + ",";
            sql += VeriErisim.DegerYap(3) + ",";
            sql += VeriErisim.DegerYap("") + ",";
            sql += VeriErisim.DegerYap((string)null) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(int sablonNo, RTFOutline rtfOutline)
        {
            string sql = "UPDATE " + TABLOADSABLON + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.SIRA], rtfOutline.sira) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.METIN], rtfOutline.metin) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.ICERLEK], rtfOutline.icerlek);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.SABLONNO], "=", sablonNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.KOD], "=", rtfOutline.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(int sablonNo, int merkezTasra)
        {
            string sql = "SELECT " + ALANLARSABLON[(int)ENUMAlanlarSABLON.KOD] + ", " + ALANLARSABLON[(int)ENUMAlanlarSABLON.SIRA] + ", " + ALANLARSABLON[(int)ENUMAlanlarSABLON.METIN] + ", " + ALANLARSABLON[(int)ENUMAlanlarSABLON.ICERLEK] + " FROM " + TABLOADSABLON;

            sql += " WHERE " + VeriErisim.KosulOl(TABLOADSABLON, ALANLARSABLON[(int)ENUMAlanlarSABLON.SABLONNO], "=", sablonNo);
            if (merkezTasra > 0)
            {
                sql += " AND (" + VeriErisim.KosulOl(TABLOADSABLON, ALANLARSABLON[(int)ENUMAlanlarSABLON.MERKEZTASRA], "=", merkezTasra);
                sql += " OR " + VeriErisim.KosulOl(TABLOADSABLON, ALANLARSABLON[(int)ENUMAlanlarSABLON.MERKEZTASRA], "=", 3) + ") ";
            }
            sql += " ORDER BY " + ALANLARSABLON[(int)ENUMAlanlarSABLON.SIRA];

            return veriErisim.DataReaderDondur(sql);
        }

        public int MaksNoBul(int sablonNo)
        {
            string sql = "SELECT MAX(" + ALANLARSABLON[(int)ENUMAlanlarSABLON.KOD] + ") FROM " + TABLOADSABLON;

            sql += " WHERE " + VeriErisim.KosulOl(TABLOADSABLON, ALANLARSABLON[(int)ENUMAlanlarSABLON.SABLONNO], "=", sablonNo);

            return (int)veriErisim.TekKolonSQLDbl(sql);
        }

        public Sonuc TurDegistir(int sablonNo, RTFOutline rtfOutline)
        {
            string sql = "UPDATE " + TABLOADSABLON + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.TUR], rtfOutline.tur) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.VARSAYILANMETIN], "") + ", ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.MERKEZTASRA], rtfOutline.merkezTasra);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.SABLONNO], "=", sablonNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSABLON.KOD], "=", rtfOutline.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader TurOku(int sablonNo, int kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADSABLON, ALANLARSABLON) + " FROM " + TABLOADSABLON;

            sql += " WHERE " + VeriErisim.KosulOl(TABLOADSABLON, ALANLARSABLON[(int)ENUMAlanlarSABLON.SABLONNO], "=", sablonNo);
            sql += " AND " + VeriErisim.KosulOl(TABLOADSABLON, ALANLARSABLON[(int)ENUMAlanlarSABLON.KOD], "=", kod);

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader RaporDetayOku(int sablonNo, int kod, int donemNo, int birimKod, int yil)
        {
            string sql = "SELECT " + ALANLARMETIN[(int)ENUMAlanlarMETIN.METIN] + " FROM " + TABLOADMETIN;

            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.BIRIMKOD], "=", birimKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.DONEMNO], "=", donemNo);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.SABLONNO], "=", sablonNo);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.KOD], "=", kod);

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc RaporDetayEkle(int sablonNo, int kod, int donemNo, int birimKod, int yil)
        {
            string sql = "INSERT INTO " + TABLOADMETIN + " (" + VeriErisim.TumAlanlar("", ALANLARMETIN) + ") VALUES (";
            sql += VeriErisim.DegerYap(birimKod) + ",";
            sql += VeriErisim.DegerYap(donemNo) + ",";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(sablonNo) + ",";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap("") + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc RaporDetayGuncelle(int sablonNo, int kod, int donemNo, int birimKod, int yil)
        {
            string sql = "UPDATE " + TABLOADMETIN + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.METIN], "");
            sql += " WHERE " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.BIRIMKOD], "=", birimKod);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.DONEMNO], "=", donemNo);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.YIL], "=", yil);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.SABLONNO], "=", sablonNo);
            sql += " AND " + VeriErisim.KosulOl("", ALANLARMETIN[(int)ENUMAlanlarMETIN.KOD], "=", kod);

            return KomutCalistir(sql, 1);
        }
    }
}