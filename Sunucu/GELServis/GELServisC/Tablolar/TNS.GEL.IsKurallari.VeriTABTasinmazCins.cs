using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTasinmazCins : TABOrtak
    {
        public TABTasinmazCins(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "GELTASINMAZCINS";
        public static string[] ALANLAR = { "KOD", "AD" };
        public  enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
        }


        public Sonuc Kaydet(TasinmazCins bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod.Replace(".","")) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public IDataReader Listele(TasinmazCins bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(bilgi.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", bilgi.kod.Replace(".","")+"%"));
            if (!string.IsNullOrEmpty(bilgi.ad))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", bilgi.ad + "%"));
            return veriErisim.DataReaderDondur(sql);
        }


        public bool KayitVarmi(string kod)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod.Replace(".","")));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }

        public Sonuc Sil(string kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod.Replace(".","")));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Guncelle(TasinmazCins cins)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE  " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD],cins.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", cins.kod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
    }
}
