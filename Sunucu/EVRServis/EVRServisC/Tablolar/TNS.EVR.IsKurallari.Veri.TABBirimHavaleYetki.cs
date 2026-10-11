using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    public class TABBirimHavaleYetki : TABOrtak
    {
        public TABBirimHavaleYetki(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRBIRIMHAVALEYETKI";
        public enum ENUMAlanlar { EVRAKBIRIMKOD = 0, HAVALEBIRIMKOD = 1, SIRANO = 2 }
        public static string[] ALANLAR = { "EVRAKBIRIMKOD", "HAVALEBIRIMKOD", "SIRANO", };

        public Sonuc Kaydet(BirimHavaleYetki form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(form.havaleBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(BirimHavaleYetki form)
        {
            string sql = "";

            if (form.sorguIslem == 1)
            {
                sql = "SELECT Y.EVRAKBIRIMKOD, Y.HAVALEBIRIMKOD, 0, E.AD, E.IDAREKIMLIKKODU FROM EVRBIRIMHAVALEYETKI AS Y ";
                sql += "INNER JOIN EVRBIRIM E ON E.KOD = Y.HAVALEBIRIMKOD ";

                //sql += "WHERE Y.EVRAKBIRIMKOD = N'R.30.0.06.15' AND E.KOD<> N'R.30.0.06.15' ";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("Y", "EVRAKBIRIMKOD", "=", form.evrakBirimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("E", "KOD", "<>", form.evrakBirimKod));

                sql += "UNION ";

                sql += "SELECT " + VeriErisim.DegerYap(form.evrakBirimKod) + ", KOD, 0, AD, IDAREKIMLIKKODU FROM EVRBIRIM ";
                //sql += "WHERE KOD LIKE N'R.30.0.06.15.__' ";
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "LIKE", form.evrakBirimKod + ".__"));

                sql += "ORDER BY 2";
            }
            else
            {
                sql = "SELECT EVRBIRIMHAVALEYETKI.EVRAKBIRIMKOD, EVRBIRIMHAVALEYETKI.HAVALEBIRIMKOD, EVRBIRIMHAVALEYETKI.SIRANO, ";
                sql += "EVRBIRIM.AD, EVRBIRIM.IDAREKIMLIKKODU ";
                sql += "FROM EVRBIRIMHAVALEYETKI ";
                sql += "INNER JOIN EVRBIRIM ON EVRBIRIMHAVALEYETKI.HAVALEBIRIMKOD = EVRBIRIM.KOD ";

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "LIKE", form.evrakBirimKod));
                if (form.siraNo > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

                sql += " ORDER BY EVRBIRIMHAVALEYETKI.SIRANO";
            }


            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string evrakBirimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", evrakBirimKod));

            return KomutCalistir(sql, 0);
        }
    }
}
