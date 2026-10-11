using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    public class TABBirimDagitimYetki : TABOrtak
    {
        public TABBirimDagitimYetki(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRBIRIMDAGITIMYETKI";
        public enum ENUMAlanlar { EVRAKBIRIMKOD = 0, DAGITIMBIRIMKOD = 1, SIRANO = 2 }
        public static string[] ALANLAR = { "EVRAKBIRIMKOD", "DAGITIMBIRIMKOD", "SIRANO", };

        public Sonuc Kaydet(BirimDagitimYetki form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(form.dagitimBirimi.kod) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(BirimDagitimYetki form)
        {
            string sql = "SELECT EVRBIRIMDAGITIMYETKI.EVRAKBIRIMKOD, EVRBIRIMDAGITIMYETKI.DAGITIMBIRIMKOD, EVRBIRIMDAGITIMYETKI.SIRANO, " +
             "EVRBIRIM.AD, EVRBIRIM.IDAREKIMLIKKODU " +
             "FROM EVRBIRIMDAGITIMYETKI " +
             "INNER JOIN EVRBIRIM ON EVRBIRIMDAGITIMYETKI.DAGITIMBIRIMKOD = EVRBIRIM.KOD ";
            //string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", form.evrakBirimKod));
            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            sql += " ORDER BY EVRBIRIMDAGITIMYETKI.SIRANO";

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
