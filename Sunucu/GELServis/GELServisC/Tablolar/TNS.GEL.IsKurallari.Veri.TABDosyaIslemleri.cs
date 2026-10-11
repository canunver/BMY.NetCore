using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABDosyaIslem:TABOrtak
    {
        public TABDosyaIslem(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "DOSYALAR";
        public enum ENUMAlanlar
        {
            SIRANO = 0,
            YIL=1,
            ANAHTAR = 2,
            DOSYAYOLU = 3,
            DOSYAADI = 4,
            TUR = 5,
            ACIKLAMA = 6

        }
        public static string[] ALANLAR = { "SIRANO","YIL", "ANAHTAR", "DOSYAYOLU", "DOSYAADI", "TUR", "ACIKLAMA" };


        public Sonuc Kaydet(Dosya bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.sirano) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.anahtar) + ",";
            sql += VeriErisim.DegerYap(bilgi.dosyayolu) + ",";
            sql += VeriErisim.DegerYap(bilgi.dosyaadi) + ",";
            sql += VeriErisim.DegerYap(bilgi.tur) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }


        public Sonuc Sil(Dosya bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.sirano));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }


        public IDataReader Listele(Dosya bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.sirano>0)
               sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.sirano));
            return veriErisim.DataReaderDondur(sql);
        }

        public int SiraNoAl()
        {
            string sql = "";
            sql = "SELECT ISNULL(MAX("+ ENUMAlanlar.SIRANO +"),0)+1 ";
            sql += " FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
