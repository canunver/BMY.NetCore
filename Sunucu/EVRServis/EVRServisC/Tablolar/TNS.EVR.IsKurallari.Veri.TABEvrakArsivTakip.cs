using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{


    public class TABEvrakArsivTakip : TABOrtak
    {
        public TABEvrakArsivTakip(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKARSIVTAKIP";
        public enum ENUMAlanlar : int
        {
            KLASORAD = 0,
            ISLEMYAPAN = 1,
            KOPYALATARIH1 = 2,
            KOPYALATARIH2 = 3,
            KAYITTARIH1 = 4,
            KAYITTARIH2 = 5,
        }
        public static string[] ALANLAR = {  "KLASORAD"
                                        ,"ISLEMYAPAN"
                                        ,"KOPYALATARIH1"
                                        ,"KOPYALATARIH2"
                                        ,"KAYITTARIH1"
                                        ,"KAYITTARIH2"
                                        };

        public Sonuc Kaydet(EvrakArsivTakip form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.klasorad) + ",";
            sql += VeriErisim.DegerYap(form.islemyapan) + ",";
            sql += VeriErisim.DegerYap(form.kopyalatarih1) + ",";
            sql += VeriErisim.DegerYap(form.kopyalatarih2) + ",";
            sql += VeriErisim.DegerYap(form.kayittarih1) + ",";
            sql += VeriErisim.DegerYap(form.kayittarih2) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakArsivTakip form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            if (!string.IsNullOrEmpty(form.klasorad))
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KLASORAD], "=", form.klasorad));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemyapan));
            sql += " ORDER BY KOPYALATARIH1 DESC";
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
