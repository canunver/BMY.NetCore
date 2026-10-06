using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS;
using TNS.Veri;

namespace TNS.KYM.IsKurallari.Veri
{
    public class TABRapor : TABOrtak
    {
        public TABRapor(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "KULLANICIRAPORABONE";

        public enum ENUMAlanlar : int
        {
            TCKIMLIKNO = 0,
            AD = 1,
            SOYAD = 2,
            UNVAN = 3,
            EMAIL = 4,
            RAPORGRUPNO = 5,
            RAPORNO = 6,
            RAPORDONEM = 7,
            RAPORDONEMICI = 8,
            RAPORDIL = 9,
        }

        public static string[] ALANLAR = { "TCKIMLIKNO", "AD", "SOYAD", "UNVAN", "EMAIL", "RAPORGRUPNO", "RAPORNO", "RAPORDONEM", "RAPORDONEMICI", "RAPORDIL" };

        public IDataReader Listele()
        {
            DateTime date = DateTime.Now;
            int hafta = (int)date.DayOfWeek;
            int ayGun = date.Day;
            if (hafta == 0) hafta = 7;

            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RAPORDONEM], "=", "1"));
            sql += " OR (" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RAPORDONEM], "=", "2") + " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RAPORDONEMICI], "=", hafta) + ")";
            sql += " OR (" + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RAPORDONEM], "=", "3") + " AND " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.RAPORDONEMICI], "=", ayGun) + ")";

            sql += "ORDER BY 6,7,10";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}