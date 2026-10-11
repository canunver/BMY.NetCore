using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTasinmazBolgeIl : TABOrtak
    {
        public TABTasinmazBolgeIl(VeriErisim veriErisim) : base(veriErisim) { }
        protected static string TABLOAD = "GELTASINMAZBOLGEIL";
        protected static string[] ALANLAR = { "BOLGEKOD", "ILKOD", "ILACIKLAMA" };
        protected enum ENUMAlanlar
        {
            BOLGEKOD = 0,
            ILKOD = 1,
            ILACIKLAMA = 2

        }

        public Sonuc Kaydet(Bolge bolge, BolgeIl il)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bolge.kod) + ",";
            sql += VeriErisim.DegerYap(il.kod) + ",";
            sql += VeriErisim.DegerYap(il.ad) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(int bolgeKod, string ilKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BOLGEKOD], "=", bolgeKod));
            if (!string.IsNullOrEmpty(ilKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ILKOD], "=", ilKod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }



        public System.Data.IDataReader Listele(int kod)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BOLGEKOD], "=", kod));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ILACIKLAMA] + " ASC ";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
