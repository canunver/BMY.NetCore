using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
  public  class TABTasinmazEkDosya : TABOrtak
    {
        public TABTasinmazEkDosya(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "GELTASINMAZDOSYA";
        public enum ENUMAlanlar
        {
            TASINMAZYIL = 0,
            TASINMAZKOD = 1,
            EKSIRANO = 2,
            ACIKLAMA = 3,
            DOSYAYOLU = 4
        }

        public static string[] ALANLAR =  {
                                            "TASINMAZYIL",
                                            "TASINMAZKOD",
                                            "EKSIRANO",
                                            "ACIKLAMA",
                                            "DOSYAYOLU"
                                          };


        public Sonuc Kaydet(TasinmazEkDosya dosya)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(dosya.tasinmazYil) + ",";
            sql += VeriErisim.DegerYap(dosya.tasinmazKod) + ",";
            sql += VeriErisim.DegerYap(dosya.siraNo) + ",";
            sql += VeriErisim.DegerYap(dosya.aciklama) + ",";
            sql += VeriErisim.DegerYap(dosya.dosyaYolu) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(TasinmazEkDosya dosya)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", dosya.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", dosya.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKSIRANO], "=", dosya.siraNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader Listele(TasinmazEkDosya dosya)
        {
            string sql = "";
            sql = " SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(dosya.tasinmazKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", dosya.tasinmazKod));
            if (dosya.tasinmazYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", dosya.tasinmazYil));
            if (dosya.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKSIRANO], "=", dosya.siraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public int  SiraNoAl(TasinmazEkDosya dosya)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.EKSIRANO + "),0)+1 AS MAXEKNO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", dosya.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", dosya.tasinmazYil));
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
