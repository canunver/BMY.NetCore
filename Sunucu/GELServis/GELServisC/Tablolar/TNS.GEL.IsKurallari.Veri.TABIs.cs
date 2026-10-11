using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABIs : TABOrtak
    {
        public TABIs(VeriErisim veriErisim) : base(veriErisim) { }

        protected static string TABLOAD = "GELIS";
        protected enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
            EKOKOD = 2,
            HESAPKOD = 3,
            TUR = 4
        }

        protected static string[] ALANLAR = { "KOD", "AD", "EKOKOD", "HESAPKOD", "TUR" };


        public Sonuc Kaydet(Is bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ",";
            sql += VeriErisim.DegerYap(bilgi.ekoKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeHesapKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc Sil(int kod, int tur)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }


        public IDataReader Listele(int kod, int tur)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));
            if (kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            return veriErisim.DataReaderDondur(sql);

        }


        public Sonuc Guncelle(Is bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], bilgi.ekoKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPKOD], bilgi.odemeHesapKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], bilgi.tur);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public bool KayitVarmi(int kod, int tur)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));
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
