using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.GEL.IsKurallari.Veri
{
    public class TABTasinmazOdemePlani : TABOrtak
    {
        public TABTasinmazOdemePlani(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "GELTASINMAZODEMEPLANI";
        public enum ENUMAlanlar
        {
            TASINMAZYILI=0,
            TASINMAZKODU = 1,
            TAKSITNO = 2,
            ODEMETARIHI = 3,
            FIRMAKISI = 4,
            ODEMETUTARI = 5,
            MAKBUZNO = 6
        }
        static string[] ALANLAR = { 
                                "TASINMAZYILI",
                                "TASINMAZKODU"
                                ,"TAKSITNO"
                                ,"ODEMETARIHI"
                                ,"FIRMAKISI"
                                ,"ODEMETUTARI"
                                ,"MAKBUZNO"
                                };

        public Sonuc Kaydet(TasinmazOdemePlan bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazBilgisi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeTarihi) + ",";
            sql += VeriErisim.DegerYap(bilgi.firma.firmaBilgisi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.odemeTutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.makbuzno) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(TasinmazOdemePlan bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYILI], "=", bilgi.tasinmaz.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAKSITNO], "=", bilgi.siraNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKISI], "=", bilgi.firma.firmaBilgisi.kod));
            if (bilgi.odemeTarihi != null)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ODEMETARIHI], "=", bilgi.odemeTarihi));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader Listele(TasinmazOdemePlan bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " , TT." + TABTasinmazTanim.ALANLAR[(int)TABTasinmazTanim.ENUMAlanlar.AD];
            sql += " FROM " + TABLOAD;
            sql += " LEFT JOIN " + TABTasinmazTanim.TABLOAD + " AS TT ";
            sql += " ON " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TASINMAZKODU] + "= TT." + TABTasinmazTanim.ALANLAR[(int)TABTasinmazTanim.ENUMAlanlar.TASINMAZKOD];
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazBilgisi.tasinmazSistemKodu.Trim()));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYILI], "=", bilgi.tasinmaz.tasinmazBilgisi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FIRMAKISI], "=", bilgi.firma.firmaBilgisi.kod));
            if (bilgi.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TAKSITNO], "=", bilgi.siraNo));
            if (bilgi.odemeTarihi != null && bilgi.odemeTarihi.Oku().ToString() != "01.01.0001 00:00:00")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ODEMETARIHI], "=", bilgi.odemeTarihi));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader FirmaKisiListele(string tasinmazSistemKod,int tasinmazYili)
        {
            string sql = "";
            sql += " SELECT ";
            sql += ALANLAR[(int)ENUMAlanlar.TASINMAZKODU] + ",";
            sql += ALANLAR[(int)ENUMAlanlar.FIRMAKISI] + ",";
            sql += " FB.AD  ";
            sql += " FROM  " + TABLOAD;
            sql += " LEFT JOIN GELFIRMABILGISI AS FB on " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.FIRMAKISI] + " = FB.KOD  ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKODU], "=", tasinmazSistemKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZYILI], "=", tasinmazYili));
            sql += " GROUP BY ";
            sql += ALANLAR[(int)ENUMAlanlar.TASINMAZKODU] + ",";
            sql += ALANLAR[(int)ENUMAlanlar.FIRMAKISI] + ",";
            sql += " FB.AD";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
