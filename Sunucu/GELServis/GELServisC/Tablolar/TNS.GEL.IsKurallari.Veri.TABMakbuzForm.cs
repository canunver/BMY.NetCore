using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;


namespace TNS.GEL.IsKurallari.Veri
{
    public class TABMakbuzForm : TABOrtak
    {
        public TABMakbuzForm(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADMAKBUZFORMBILGI = "GELMAKBUZFORMBILGI";
        public enum ENUMAlanlarMakbuzFormBilgi
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,
            TASINMAZKODU=5,
            TAKSITNO = 6,
            PRMAKBUZNO=7,
            MAKBUZNO = 8,
            MAKBUZTARIHI = 9,
            KDVTEVKIFAT = 10,
            GELIRVERGISITEVKIFAT = 11,
            ACIKLAMA = 12
        }
        public static string[] ALANLARMAKBUZFORMBILGI = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU","TASINMAZKODU","TAKSITNO","PRMAKBUZNO", "MAKBUZNO", "MAKBUZTARIHI", "KDVTEVKIFAT", "GELIRVERGISITEVKIFAT", "ACIKLAMA" };


        public static string TABLOADMAKBUZFORMFDETAY = "GELMAKBUZFORMDETAY";
        public enum ENUMAlanlarMakbuzFormDetay
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,
            TASINMAZKODU = 5,
            TAKSITNO = 6,
            SIRANO = 7,
            RPMAKBUZNO = 8,
            MIKTAR = 9,
            KDV = 10,
            TUTAR = 11
        }
        public static string[] ALANLARMAKBUZFORMDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU", "TASINMAZKODU", "TAKSITNO", "SIRANO", "RPMAKBUZNO", "MIKTAR", "KDV", "TUTAR" };


        public Sonuc BilgiKaydet(Makbuz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADMAKBUZFORMBILGI + " (" + VeriErisim.TumAlanlar("", ALANLARMAKBUZFORMBILGI) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.taksitno) + ",";
            sql += VeriErisim.DegerYap(bilgi.prmakbuzno) + ",";
            sql += VeriErisim.DegerYap(bilgi.makbuzno) + ",";
            sql += VeriErisim.DegerYap(bilgi.makbuztarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.kdvTevkifati) + ",";
            sql += VeriErisim.DegerYap(bilgi.gelirtevkifati) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc BilgiSil(Makbuz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADMAKBUZFORMBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader BilgiListele(Makbuz bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMAKBUZFORMBILGI) + " FROM " + TABLOADMAKBUZFORMBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        public bool BilgiKaydiVarmi(Makbuz bilgi)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOADMAKBUZFORMBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }

        public Sonuc BilgiGuncelle(Makbuz bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADMAKBUZFORMBILGI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.PRMAKBUZNO], bilgi.prmakbuzno) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.MAKBUZNO], bilgi.makbuzno) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.MAKBUZTARIHI], bilgi.makbuztarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.KDVTEVKIFAT], bilgi.kdvTevkifati) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.GELIRVERGISITEVKIFAT], bilgi.gelirtevkifati) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.ACIKLAMA], bilgi.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMBILGI[(int)ENUMAlanlarMakbuzFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc DetayKaydet(MakbuzDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADMAKBUZFORMFDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARMAKBUZFORMDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecno) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazSistemKodu) + ","; 
            sql += VeriErisim.DegerYap(bilgi.taksitno) + ",";
            sql += VeriErisim.DegerYap(bilgi.sirano) + ",";
            sql += VeriErisim.DegerYap(bilgi.prmakbuzno) + ",";
            sql += VeriErisim.DegerYap(bilgi.miktar) + ",";
            sql += VeriErisim.DegerYap(bilgi.kdv) + ",";
            sql += VeriErisim.DegerYap(bilgi.tutar) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc DetaySil(int yil, int muhasebe, string birim, string surecno, int firmakodu,  string tasinmazkodu,int taksitno, int sirano)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADMAKBUZFORMFDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.SURECNO], "=", surecno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.FIRMAKODU], "=", firmakodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.TAKSITNO], "=", taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.TASINMAZKODU], "=", tasinmazkodu));
            if (sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.SIRANO], "=", sirano));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader DetayListele(int yil, int muhasebe, string birim, string surecno, int firmakodu,  string tasinmazkodu, int sirano, int taksitno)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMAKBUZFORMDETAY) + " FROM " + TABLOADMAKBUZFORMFDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.SURECNO], "=", surecno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.FIRMAKODU], "=", firmakodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.TASINMAZKODU], "=", tasinmazkodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.TAKSITNO], "=", taksitno));
            if (sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMAKBUZFORMDETAY[(int)ENUMAlanlarMakbuzFormDetay.SIRANO], "=", sirano));

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
