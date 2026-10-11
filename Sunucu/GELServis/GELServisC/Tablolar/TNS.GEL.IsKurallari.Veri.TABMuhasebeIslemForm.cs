using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABMuhasebeIslemForm : TABOrtak
    {
        public TABMuhasebeIslemForm(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADMUHASEBEISLEMFORMBILGI = "GELMUHASEBEISLEMFORMBILGI";
        public enum ENUMAlanlarMuhasebeIslemFormBilgi
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,
            TASINMAZKODU = 5,
            TAKSITNO = 6,
            PRMAKBUZNO = 7,
            PRODEMEEMRINO = 8,
            ODEMEEMRINO = 9,
            ODEMEEMRITARIHI = 10,
            ODEMEEMRIYILI=11,
            ACIKLAMA=12
        }
        static string[] ALANLARMUHASEBEISLEMFORMBILGI = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU", "TASINMAZKODU", "TAKSITNO", "PRMAKBUZNO", "PRODEMEEMRINO", "ODEMEEMRINO", "ODEMEEMRITARIHI", "ODEMEEMRIYILI", "ACIKLAMA" };


        public static string TABLOADMUHASEBEISLEMFORMDETAY = "GELMUHASEBEISLEMFORMDETAY";
        public enum ENUMAlanlarMuhasebeIslemFormDetay
        {
            YIL = 0,
            MUHASEBEKOD = 1,
            BIRIMKOD = 2,
            SURECNO = 3,
            FIRMAKODU = 4,
            TASINMAZKODU = 5,
            TAKSITNO = 6,
            SIRANO=7,
            HESAPKOD=8,
            YANSITMAHESAPKOD=9,
            TERTIP=10,
            BORC=11,
            ALACAK=12
        }
        static string[] ALANLARMUHASEBEISLEMFORMDETAY = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "FIRMAKODU", "TASINMAZKODU", "TAKSITNO", "SIRANO", "HESAPKOD", "YANSITMAHESAPKOD", "TERTIP", "BORC", "ALACAK"  };

        public Sonuc BilgiKaydet(MuhasebeIslemForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADMUHASEBEISLEMFORMBILGI + " (" + VeriErisim.TumAlanlar("", ALANLARMUHASEBEISLEMFORMBILGI) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.taksitno) + ",";
            sql += VeriErisim.DegerYap(bilgi.prmakbuzno) + ",";
            sql += VeriErisim.DegerYap(bilgi.prOdemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriTarih) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeEmriYil) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ")" ;
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc BilgiSil(MuhasebeIslemForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADMUHASEBEISLEMFORMBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader BilgiListele(MuhasebeIslemForm bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMUHASEBEISLEMFORMBILGI) + " FROM " + TABLOADMUHASEBEISLEMFORMBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            return veriErisim.DataReaderDondur(sql);
        }

        public bool BilgiKaydiVarmi(MuhasebeIslemForm bilgi)
        {
            bool kayitvar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOADMUHASEBEISLEMFORMBILGI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            kayitvar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitvar;
        }


        public Sonuc BilgiGuncelle(MuhasebeIslemForm bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOADMUHASEBEISLEMFORMBILGI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.PRMAKBUZNO], bilgi.prmakbuzno) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.PRODEMEEMRINO], bilgi.prOdemeEmriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.ODEMEEMRINO], bilgi.odemeEmriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.ODEMEEMRITARIHI], bilgi.odemeEmriTarih)+",";
            sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.ODEMEEMRIYILI], bilgi.odemeEmriYil) +",";
            sql += VeriErisim.AtamaOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.ACIKLAMA], bilgi.aciklama) ;
            
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.FIRMAKODU], "=", bilgi.firma.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TAKSITNO], "=", bilgi.taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMBILGI[(int)ENUMAlanlarMuhasebeIslemFormBilgi.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }



        public Sonuc DetayKaydet(MuhasebeIslemFormDetay bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOADMUHASEBEISLEMFORMDETAY+ " (" + VeriErisim.TumAlanlar("", ALANLARMUHASEBEISLEMFORMDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.taksitno) + ",";
            sql += VeriErisim.DegerYap(bilgi.sirano) + ",";
            sql += VeriErisim.DegerYap(bilgi.hesapPlanKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.yansitma) + ",";
            sql += VeriErisim.DegerYap(bilgi.tertip) + ",";
            sql += VeriErisim.DegerYap(bilgi.borc) + ",";
            sql += VeriErisim.DegerYap(bilgi.alacak) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }


        public Sonuc DetaySil(int yil, int muhasebe, string birim, string surecno, int firmakodu, string tasinmazkodu, int taksitno, int sirano)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOADMUHASEBEISLEMFORMDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.SURECNO], "=", surecno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.FIRMAKODU], "=", firmakodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.TAKSITNO], "=", taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.TASINMAZKODU], "=", tasinmazkodu));
            if (sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.SIRANO], "=", sirano));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader DetayListele(int yil, int muhasebe, string birim, string surecno, int firmakodu,  string tasinmazkodu, int sirano, int taksitno)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARMUHASEBEISLEMFORMDETAY) + " FROM " + TABLOADMUHASEBEISLEMFORMDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.SURECNO], "=", surecno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.FIRMAKODU], "=", firmakodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.TAKSITNO], "=", taksitno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.TASINMAZKODU], "=", tasinmazkodu));
            if (sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARMUHASEBEISLEMFORMDETAY[(int)ENUMAlanlarMuhasebeIslemFormDetay.SIRANO], "=", sirano));

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
