using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABKadroKontenjanDetay : TABOrtak
    {
        public TABKadroKontenjanDetay(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "PERKADROKONTENJANDETAY";

        public enum ENUMAlanlar : int
        {
            ORGBIRIMKOD = 0,
            KURKOD = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            SIRANO = 4,
            UNVANKOD = 5,
            KADRODERECE = 6,
            KONTENJAN = 7
        }

        public static string[] ALANLAR = { "ORGBIRIMKOD", "KURKOD", "MUHASEBEKOD", "BIRIMKOD", "SIRANO",
                                           "UNVANKOD", "KADRODERECE", "KONTENJAN" };

        public Sonuc Kaydet(KadroKontenjanDetay kkDetay)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kkDetay.orgBirimKod) + ",";
            if (kkDetay.mb != null)
            {
                sql += VeriErisim.DegerYap(kkDetay.mb.kurum.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(kkDetay.mb.muhasebe) + ",";
                sql += VeriErisim.DegerYap(kkDetay.mb.birimKod) + ",";
            }
            else
            {
                sql += VeriErisim.DegerYap(string.Empty) + ",";
                sql += VeriErisim.DegerYap(0) + ",";
                sql += VeriErisim.DegerYap(string.Empty) + ",";
            }
            sql += VeriErisim.DegerYap(kkDetay.siraNo) + ",";
            sql += VeriErisim.DegerYap(kkDetay.unvanKod) + ",";
            sql += VeriErisim.DegerYap(kkDetay.kadroDerece) + ",";
            sql += VeriErisim.DegerYap(kkDetay.kontenjan) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(KadroKontenjanDetay kkDetay)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", kkDetay.orgBirimKod));
            if (kkDetay.mb != null)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kkDetay.mb.kurum.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kkDetay.mb.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kkDetay.mb.birimKod));
            }
            else
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", string.Empty));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", 0));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", string.Empty));
            }
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", kkDetay.siraNo));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(KadroKontenjanDetay kkDetay)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (kkDetay.orgBirimKod > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", kkDetay.orgBirimKod);
            if (kkDetay.mb != null)
            {
                if (!string.IsNullOrEmpty(kkDetay.mb.kurum))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kkDetay.mb.kurum.Replace(".", "")));
                if (kkDetay.mb.muhasebe > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kkDetay.mb.muhasebe));
                if (!string.IsNullOrEmpty(kkDetay.mb.birimKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kkDetay.mb.birimKod));
            }
            if (kkDetay.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", kkDetay.siraNo));
            if (kkDetay.unvanKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVANKOD], "=", kkDetay.unvanKod));
            if (kkDetay.kadroDerece > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KADRODERECE], "=", kkDetay.kadroDerece));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UnvanVeKadroDereceBazliKontenjanBul(KadroKontenjanDetay kkDetay)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KADRODERECE];
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KONTENJAN] + ")";
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (kkDetay.orgBirimKod > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", kkDetay.orgBirimKod);
            if (kkDetay.mb != null)
            {
                if (!string.IsNullOrEmpty(kkDetay.mb.kurum))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kkDetay.mb.kurum.Replace(".", "")));
                if (kkDetay.mb.muhasebe > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kkDetay.mb.muhasebe));
                if (!string.IsNullOrEmpty(kkDetay.mb.birimKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", kkDetay.mb.birimKod));
            }
            if (kkDetay.unvanKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.UNVANKOD], "=", kkDetay.unvanKod));
            if (kkDetay.kadroDerece > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KADRODERECE], "=", kkDetay.kadroDerece));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UNVANKOD];
            sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KADRODERECE];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}