using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTasinmazTalep : TABOrtak
    {
        public TABTasinmazTalep(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "GELTASINMAZTALEP";

        public enum ENUMAlanlar
        {
            TASINMAZYIL = 0,
            TASINMAZKOD = 1,
            FIRMAKOD = 2,
            SIRANO = 3,
            YAZINO = 4,
            YAZITARIH = 5,
            ACIKLAMA = 6,
            SONUC = 7,
            SONISLEMYAPAN = 8
        }

        public string[] ALANLAR = { "TASINMAZYIL", "TASINMAZKOD", "FIRMAKOD", "SIRANO", "YAZINO", "YAZITARIH", "ACIKLAMA", "SONUC", "SONISLEMYAPAN" };

        public IDataReader Listele(TasinmazTalep tasinmazTalep)
        {
            string sql = "";
            sql = " SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += "," + " GELFIRMABILGISI.AD FIRMAD ";
            sql += " ," + TABTasinmazTanim.TABLOAD + "." + TABTasinmazTanim.ALANLAR[(int)TABTasinmazTanim.ENUMAlanlar.AD];
            sql += " FROM " + TABLOAD;
            sql += " LEFT JOIN " + TABTasinmazTanim.TABLOAD + " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TASINMAZKOD] + "= " + TABTasinmazTanim.TABLOAD + "." + TABTasinmazTanim.ALANLAR[(int)TABTasinmazTanim.ENUMAlanlar.TASINMAZKOD];
            sql += " LEFT JOIN GELFIRMABILGISI ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKOD] + " = GELFIRMABILGISI.KOD ";
            sql += " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(tasinmazTalep.tasinmazKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", tasinmazTalep.tasinmazKod));
            if (tasinmazTalep.tasinmazYil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", tasinmazTalep.tasinmazYil));
            if (tasinmazTalep.firmaKodu > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", tasinmazTalep.firmaKodu));
            if (tasinmazTalep.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", tasinmazTalep.siraNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(TasinmazTalep bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Kaydet(TasinmazTalep bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.tasinmazYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmazKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.firmaKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaziNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.yaziTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.sonucKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.sonIslemYapanKodu) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public int TalepTasinmazSiraNoAl(TasinmazTalep bilgi)
        {
            string sql = "";
            sql = "SELECT      ISNULL(MAX(" + ENUMAlanlar.SIRANO + "),0)+1 AS MAXSIRANO ";
            sql += " FROM  " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public bool KayitVarmi(TasinmazTalep bilgi)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }

        public Sonuc Guncelle(TasinmazTalep bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAZINO], bilgi.yaziNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YAZITARIH], bilgi.yaziTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONUC], bilgi.sonucKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SONISLEMYAPAN], bilgi.sonIslemYapanKodu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKOD], "=", bilgi.tasinmazKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYIL], "=", bilgi.tasinmazYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKOD], "=", bilgi.firmaKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.siraNo));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }
    }
}
