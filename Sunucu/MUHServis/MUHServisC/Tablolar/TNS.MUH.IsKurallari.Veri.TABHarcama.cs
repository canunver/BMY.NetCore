using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.ComponentModel;
using System.Data;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABHarcama : TABOrtak
    {
        public TABHarcama(VeriErisim veriErisim)
            : base(veriErisim)
        {

        }
        public static string TABLOAD = "ISLHARCAMAAYLIK";
        public string[] ALANLAR = { 
                                "YIL",
                                "MUHASEBEKOD",
                                "BIRIMKOD",
                                "KURKOD",
                                "FONKOD",
                                "FINKOD",
                                "EKOKOD",
                                "[1]","[2]","[3]","[4]","[5]","[6]","[7]","[8]","[9]","[10]","[11]","[12]"
                                 };
        public enum ENUMAlanlar : int
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            KURKOD = 3,
            FONKOD = 4,
            FINKOD = 5,
            EKOKOD = 6,
            [Description("[1]")]
            OCAK = 7,
            [Description("[2]")]
            SUBAT = 8,
            [Description("[3]")]
            MART = 9,
            [Description("[4]")]
            NISAN = 10,
            [Description("[5]")]
            MAYIS = 11,
            [Description("[6]")]
            HAZIRAN = 12,
            [Description("[7]")]
            TEMMUZ = 13,
            [Description("[8]")]
            AGUSTOS = 14,
            [Description("[9]")]
            EYLUL = 15,
            [Description("[10]")]
            EKIM = 16,
            [Description("[11]")]
            KASIM = 17,
            [Description("[12]")]
            ARALIK = 18
        }

        public IDataReader Listele(ISLHarcama bilgi)
        {
            string birimkod = bilgi.birimKod.Replace(".", "");
            string kur = bilgi.kurKod.Replace(".", "");
            string fon = bilgi.fonKod.Replace(".", "");
            string fin = bilgi.finKod.Replace(".", "");
            string eko = bilgi.ekoKod.Replace(".", "");
            string sql = "";
            sql = " SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebeKod>0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));
            if(!string.IsNullOrEmpty(birimkod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimkod));
            if (!string.IsNullOrEmpty(kur))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kur + "%"));
            if (!string.IsNullOrEmpty(fon))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", fon));
            if (!string.IsNullOrEmpty(fin))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", fin));
            if (!string.IsNullOrEmpty(eko.Trim()))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", eko));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
