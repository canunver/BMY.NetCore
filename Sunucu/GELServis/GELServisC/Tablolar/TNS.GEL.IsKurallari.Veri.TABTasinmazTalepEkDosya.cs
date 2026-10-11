using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTasinmazTalepEkDosya : TABOrtak
    {
        public TABTasinmazTalepEkDosya(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "GELTASINMAZTALEPDOSYA";

        public enum ENUMAlanlar
        {
            TASINMAZYIL = 0,
            TASINMAZKOD = 1,
            FIRMAKOD = 2,
            TALEPSIRANO = 3,
            EKNO = 4,
            ACIKLAMA = 5,
            DOSYAYOLU = 6,

        }

        public string[] ALANLAR = {"TASINMAZYIL", "TASINMAZKOD", "FIRMAKOD", "TALEPSIRANO", "EKNO", "ACIKLAMA", "DOSYAYOLU" };

        public IDataReader Listele(TasinmazTalepEki bilgi)
        {
            string sql = "";
            sql = " SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(bilgi.tasinmazKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "LIKE", bilgi.tasinmazKod + "%"));
            if (bilgi.firmaKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            if (bilgi.firmaKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            if (bilgi.talepSiraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPSIRANO], "=", bilgi.talepSiraNo));
            if (bilgi.ekNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKNO], "=", bilgi.ekNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(TasinmazTalepEki bilgi, bool ekNoKontrol = true)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPSIRANO], "=", bilgi.talepSiraNo));
            if (ekNoKontrol)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKNO], "=", bilgi.ekNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Kaydet(TasinmazTalepEki bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.tasinmazYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmazKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.firmaKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.talepSiraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.ekNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.dosyaYolu) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int TalepTasinmazEkiSiraNoAl(TasinmazTalepEki bilgi)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.EKNO + "),0)+1 AS MAXEKNO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPSIRANO], "=", bilgi.talepSiraNo));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public string TalepEkiDosyaYoluGetir(TasinmazTalepEki bilgi, bool ekNoKontrol = true)
        {
            string sql = "";
            sql = " SELECT " + ENUMAlanlar.DOSYAYOLU;
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TALEPSIRANO], "=", bilgi.talepSiraNo));
            if (ekNoKontrol)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKNO], "=", bilgi.ekNo));
            return veriErisim.TekKolonSQLStr(sql);
        }


    }
}
