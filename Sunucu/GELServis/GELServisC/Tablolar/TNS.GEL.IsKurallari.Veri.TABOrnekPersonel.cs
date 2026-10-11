using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABOrnekPersonel : TABOrtak
    {
        public TABOrnekPersonel(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "AGELORNEKPERSONEL";
        public enum ENUMAlanlar
        {

            ID = 0,
            AD = 1,
            SOYAD = 2,
            CINSIYET = 3
        }
        public static string[] ALANLAR = { "ID", "AD", "SOYAD", "CINSIYET" };

        public Sonuc Kaydet(OrnekPersonel bilgi)
        {
            Sonuc sonuc = new Sonuc();
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.id) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.soyad) + ",";
            sql += VeriErisim.DegerYap(bilgi.cinsiyet) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int MaxIdGetir()
        {
            int maxid = -1;
            string sql = " SELECT MAX(" + ENUMAlanlar.ID + ") +1 ";
            sql += " FROM " + TABLOAD;
            maxid = veriErisim.TekKolonSQLInt(sql);
            return maxid;
        }

    }
}
