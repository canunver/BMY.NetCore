using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABParca : TABOrtak
    {
        public TABParca(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "GELPARCA";
        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            SAYFA = 2,
            YUKSEKLIK = 3,
            GENISLIK = 4

        }

        public static string[] ALANLAR = { "KOD", "AD", "SAYFA", "YUKSEKLIK", "GENISLIK" };


        public Sonuc Kaydet(Parca bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.sayfa) + ",";
            sql += VeriErisim.DegerYap(bilgi.yukseklik) + ",";
            sql += VeriErisim.DegerYap(bilgi.genislik) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

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


        public IDataReader Listele(Parca bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (bilgi.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.ad != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + bilgi.ad + "%"));
            if (bilgi.sayfa != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYFA], " LIKE ", "%" + bilgi.sayfa + "%"));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];
            return veriErisim.DataReaderDondur(sql);

        }


        public Sonuc Guncelle(Parca bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYFA], bilgi.sayfa) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YUKSEKLIK], bilgi.yukseklik) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GENISLIK], bilgi.genislik);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public bool KayitVarmi(int kod)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }

        public int SiraNoAl()
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.KOD + "),0)+1 AS MAXEKNO ";
            sql += " FROM  " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
