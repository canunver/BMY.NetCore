using System;
using System.Data;
using TNS;
using TNS.Veri;

namespace TNS.Veri
{
    public class Sayac
    {
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan)
        {
            return SayacDegerAl(ve, tablo, alan, new string[0], new string[0], new string[0], new int[0]);
        }

        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            string sql = string.Empty;
            string kosul = string.Empty;

            sql = "SELECT MAX(CAST(" + alan + " AS INT)) FROM " + tablo;

            if (whereStrAlan != null)
            {
                for (int i = 0; i < whereStrAlan.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereStrAlan[i], "=", whereStrDeger[i]));
            }

            if (whereIntAlan != null)
            {
                for (int i = 0; i < whereIntAlan.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereIntAlan[i], "=", whereIntDeger[i]));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return ve.TekKolonSQLInt(sql) + 1;
        }

        public static double KayitSayiVer(VeriErisim ve, string tablo, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            string sql = string.Empty;
            string kosul = string.Empty;

            sql = "SELECT COUNT(*) FROM " + tablo;

            if (whereStrAlan != null)
            {
                for (int i = 0; i < whereStrAlan.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereStrAlan[i], "=", whereStrDeger[i]));
            }

            if (whereIntAlan != null)
            {
                for (int i = 0; i < whereIntAlan.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereIntAlan[i], "=", whereIntDeger[i]));
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return ve.TekKolonSQLDbl(sql);
        }

        public static bool KayitVarmi(VeriErisim ve, string tablo, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            return KayitSayiVer(ve, tablo, whereStrAlan, whereStrDeger, whereIntAlan, whereIntDeger) != 0;
        }
    }
}
