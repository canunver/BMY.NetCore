using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTasinmazBolge : TABOrtak
    {
        public TABTasinmazBolge(VeriErisim veriErisim) : base(veriErisim) { }
        public static string TABLOAD = "GELTASINMAZBOLGE";
        public static string[] ALANLAR = { "KOD", "AD", "AD2" };
        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            AD2 = 2

        }

        public bool BolgeKayitVarmi(Bolge bilgi)
        {
            string sql = "";
            sql = "SELECT count(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            return veriErisim.TekKolonSQLInt(sql) >= 1;

        }

        public int BolgeSiraNoAl()
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.KOD + "),0)+1 AS MAXSIRANO ";
            sql += " FROM  " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc Kaydet(Bolge bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad2) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc  Guncelle(Bolge bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD2], bilgi.ad2);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader Listele(Bolge bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (bilgi.kod>0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (!string.IsNullOrEmpty(bilgi.ad))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", bilgi.ad + "%"));

            if (!string.IsNullOrEmpty(bilgi.ad2))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", bilgi.ad2 + "%"));
            return veriErisim.DataReaderDondur(sql);          
        }

        public Sonuc Sil(int kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
    }
}
