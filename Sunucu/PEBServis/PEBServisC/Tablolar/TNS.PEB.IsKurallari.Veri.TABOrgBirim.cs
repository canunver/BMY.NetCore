using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABOrgBirim : TABOrtak
    {
        public TABOrgBirim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "ORGBIRIM";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            ADI = 1,
            KISAADI = 2,
            ANABIRIMKOD = 3,
            KOPRUKOD = 4,
            TURKOD = 5
        }

        public static string[] ALANLAR = { "KOD", "ADI", "KISAADI", "ANABIRIMKOD", "KOPRUKOD", "TURKOD" };

        public static string BagliBirimKosulOl(string tablo, string alan, int orgBirimKod)
        {
            string icSql = "SELECT B." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " FROM " + TABLOAD + " B";
            icSql = VeriErisim.KosulEkle(icSql, "WHERE", VeriErisim.KosulOl("B", ALANLAR[(int)ENUMAlanlar.KOD], "=", orgBirimKod));

            string sql = "SELECT A." + ALANLAR[(int)ENUMAlanlar.KOD] + " FROM " + TABLOAD + " A";
            sql = VeriErisim.KosulEkle(sql, "WHERE", "A." + ALANLAR[(int)ENUMAlanlar.KOPRUKOD] + " LIKE (" + icSql + ") " + VeriErisim.IslecBul("+") + " '%'");

            return tablo + "." + alan + " IN (" + sql + ")";
        }
    }
}