using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABKadroKontenjanDetay : TABOrtak
    {
        public TABKadroKontenjanDetay(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYKADROKONTENJANDETAY";

        public enum ENUMAlanlar : int
        {
            KKOD = 0,
            UNVANKOD = 1,
            KADRODERECE = 2,
            KONTENJAN = 3
        }

        public static string[] ALANLAR = { "KKOD", "UNVANKOD", "KADRODERECE", "KONTENJAN" };

        public Sonuc Kaydet(KadroKontenjanDetay kkDetay)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kkDetay.kkod) + ",";   
            sql += VeriErisim.DegerYap(kkDetay.unvanKod) + ",";
            sql += VeriErisim.DegerYap(kkDetay.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(kkDetay.kontenjan) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(KadroKontenjanDetay kkDetay)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KKOD], "=", kkDetay.kkod));
       
            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(KadroKontenjanDetay kkDetay)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", U.AD as unvanAd FROM " + TABLOAD;
            sql += " left outer join IKYUNVAN U on U.KOD = " + ALANLAR[(int)ENUMAlanlar.UNVANKOD];

            string kosul = string.Empty;
            
            if (kkDetay.kkod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KKOD], "=", kkDetay.kkod));
            if (kkDetay.unvanKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVANKOD], "=", kkDetay.unvanKod));
            if (kkDetay.kadroDerece > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KADRODERECE], "=", kkDetay.kadroDerece));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;
            sql += " ORDER BY case when U.SIRANO IS NULL then 999999999 when U.SIRANO=0 then 999999998 else U.SIRANO end, U.AD";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UnvanVeKadroDereceBazliKontenjanBul(KadroKontenjanDetay kkDetay)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KADRODERECE];
            sql += ", U.AD";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KONTENJAN] + ")";         
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN IKYUNVAN U ON U.KOD = " + ALANLAR[(int)ENUMAlanlar.UNVANKOD];

            string kosul = string.Empty;
            if (kkDetay.unvanKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVANKOD], "=", kkDetay.unvanKod));
            if (kkDetay.kadroDerece > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KADRODERECE], "=", kkDetay.kadroDerece));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KADRODERECE];
            sql += ", U.AD";
            sql += " ORDER BY case when U.SIRANO IS NULL then 999999999 when U.SIRANO=0 then 999999998 else U.SIRANO end, U.AD";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}