using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABMuhammenBedel : TABOrtak
    {
        public TABMuhammenBedel(VeriErisim veriErisim) : base(veriErisim) { }
       public static string TABLOAD = "GELMUHAMMENBEDEL";
        public enum ENUMAlanlar
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            TASINMAZKODU = 4,
            ISLEMKODU = 5,
            ISLEMADI = 6,
            KIRALAMASURESI = 7,
            KIRALAMATIPI = 8,
            BIRIMFIYAT = 9,
            TUTAR = 10,
            ACIKLAMA = 11

        }

        public string[] ALANLAR = { "YIL",
                            "MUHASEBEKOD",
                            "BIRIMKOD",
                            "SURECNO",
                            "TASINMAZKODU",
                            "ISLEMKODU",
                            "ISLEMADI",
                            "KIRALAMASURESI",
                            "KIRALAMATIPI",
                            "BIRIMFIYAT",
                            "TUTAR",
                            "ACIKLAMA"
                                  };

        public IDataReader Listele(MuhammenBedel bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(MuhammenBedel bilgi,MuhammenBedelSatir bilgisatiri )
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.tasinmazKodu) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.islem.kod) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.islem.ad) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.kiralamaSuresi) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.kiralamaTipi) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.birimFiyat) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.tutar) + ",";
            sql += VeriErisim.DegerYap(bilgisatiri.aciklama) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(MuhammenBedel bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKODU], "=", bilgi.tasinmazKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }


        
    }
}
