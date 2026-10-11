using System;
using System.Data;
using TNS.PER;
using TNS.PER.IsKurallari.Veri;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABGenel : TABOrtak
    {
        public TABGenel(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }

        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan, string kosul)
        {
            string sql = "";

            sql = "SELECT MAX(" + alan + ") FROM " + tablo;
            if (kosul != "")
                sql += " WHERE " + kosul;
            double deger = ve.TekKolonSQLDbl(sql);
            return OrtakFonksiyonlar.ConvertToInt((deger + 1).ToString(), 0);
        }
    }
}
