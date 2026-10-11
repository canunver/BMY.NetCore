using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABKomisyonUye : TABOrtak
    {
        public TABKomisyonUye(VeriErisim veriErisim) : base(veriErisim) { }
        static string TABLOAD = "GELKOMISYONUYE";
        enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            TIP = 4,
            SIRANO = 5,
            TCKIMLIKNO = 6,
            AD = 7,
            GOREV = 8,
            UNVAN = 9,
            KOMISYONGOREV = 10,
            GOREVLENDIRMETARIH =11,
            GOREVLENDIRMENO = 12

        }
        static string[] ALANLAR = { "YIL",
                                    "MUHASEBEKOD",
                                    "BIRIMKOD",
                                    "SURECNO",
                                    "TIP",
                                    "SIRANO",
                                    "TCKIMLIKNO",
                                    "AD",
                                    "GOREV",
                                    "UNVAN",
                                    "KOMISYONGOREV",
                                    "GOREVLENDIRMETARIH",
                                    "GOREVLENDIRMENO"
                                  };
        public Sonuc Kaydet(KomisyonUye bilgi, Uye uye)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tip) + ",";

            sql += VeriErisim.DegerYap(uye.siraNo) + ",";
            sql += VeriErisim.DegerYap(uye.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(uye.ad) + ",";
            sql += VeriErisim.DegerYap(uye.gorev) + ",";
            sql += VeriErisim.DegerYap(uye.unvan) + ",";
            sql += VeriErisim.DegerYap(uye.komisyonGorev) + ",";

            sql += VeriErisim.DegerYap(bilgi.gorevlendirmeTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.gorevlendirmeNo) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc Sil(KomisyonUye bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public IDataReader Listele(KomisyonUye bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (bilgi.birim != null && bilgi.birim.Trim() != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (bilgi.surecNo!= null && bilgi.surecNo.Trim() != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tip > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", bilgi.tip));
            return veriErisim.DataReaderDondur(sql);

        }
    }
}
