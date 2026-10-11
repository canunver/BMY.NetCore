using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    public class TABTORBATERTIPBIRIMODENEK : TABOrtak
    {
        public TABTORBATERTIPBIRIMODENEK(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOADTORBATERTIP = "BHMTORBATERTIPLER";
        public enum ENUMAlanlarTorbaTertip
        {
            YIL = 0,
            KURKOD = 1,
            FONKOD = 2,
            FINKOD = 3,
            EKOKOD = 4,
            PEBKOD = 5,
            MIKTAR = 5
        }
        public static string[] ALANLARTORBATERTIP = { "YIL",   "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "MIKTAR" };

        public static string TABLOADTORBABIRIM = "BHMTORBABIRIMODENEK";
        public enum ENUMAlanlarTorbaBirim
        {
            YIL = 0,
            MUHASEBE = 1,
            BIRIM = 2,
            KURKOD = 3,
            MIKTAR = 4
        }
        public static string[] ALANLARTORBABIRIM = { "YIL",  "MUHASEBE", "BIRIM", "KURKOD", "MIKTAR" };

        public Sonuc TertipKaydet(TorbaTertipBirimOdenek ttbo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            foreach (TorbaTertip item in ttbo.torbatertipler)
            {

                if (!sonuc.islemSonuc) break;
                string sql = "INSERT INTO " + TABLOADTORBATERTIP + " (" + VeriErisim.TumAlanlar("", ALANLARTORBATERTIP) + ") VALUES (";
                sql += VeriErisim.DegerYap(ttbo.yil) + ",";
                
                sql += VeriErisim.DegerYap(item.kodlar[0].Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(item.kodlar[1].Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(item.kodlar[2].Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(item.kodlar[3].Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(item.kodlar[4].Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(item.miktar) + ")";
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public Sonuc BirimKaydet(TorbaTertipBirimOdenek ttbo)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            foreach (MuhasebeBirim item in ttbo.birimler)
            {
                if (!sonuc.islemSonuc) break;
                string sql = "INSERT INTO " + TABLOADTORBABIRIM + " (" + VeriErisim.TumAlanlar("", ALANLARTORBABIRIM) + ") VALUES (";
                sql += VeriErisim.DegerYap(ttbo.yil) + ",";
               
                sql += VeriErisim.DegerYap(item.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(item.birimKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(item.kurkod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(item.miktar) + ")";
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }


        public IDataReader TertipListele(int yil,  string[] kodlar)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADTORBATERTIP, ALANLARTORBATERTIP) + " , T.AD FROM " + TABLOADTORBATERTIP;
            sql += " INNER JOIN BHMTERTIPLISTESIGIDER T ";
            sql += " ON " + TABLOADTORBATERTIP + ".YIL=T.YIL ";
            sql += "        AND " + TABLOADTORBATERTIP + ".KURKOD=T.KURKOD ";
            sql += "        AND " + TABLOADTORBATERTIP + ".FONKOD=T.FONKOD ";
            sql += "        AND " + TABLOADTORBATERTIP + ".FINKOD=T.FINKOD ";
            sql += "        AND " + TABLOADTORBATERTIP + ".EKOKOD = T.EKOKOD  ";
            sql += "        AND " + TABLOADTORBATERTIP + ".PEBKOD=T.PEBKOD ";
            VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADTORBATERTIP, ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.YIL], "=", yil));
            if (kodlar != null && kodlar.Length > 0)
            {
                if (kodlar[0] != null)
                    VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBATERTIP, ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.KURKOD], "=", kodlar[0]));
                if (kodlar[1] != null)
                    VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBATERTIP, ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.FONKOD], "=", kodlar[1]));
                if (kodlar[2] != null)
                    VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBATERTIP, ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.FINKOD], "=", kodlar[2]));
                if (kodlar[3] != null)
                    VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBATERTIP, ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.EKOKOD], "=", kodlar[3]));
                if (kodlar[4] != null)
                    VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBATERTIP, ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.PEBKOD], "=", kodlar[4]));
            }
            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader BirimListele(int yil,   int muhasebe, string birim)
        {
            birim = birim.Replace(".", "");
            string sql = "";

            sql = " SELECT  TBO.YIL,TBO.MUHASEBE,MM.MUHASEBEAD  ";
            sql += " ,TBO.BIRIM,MMB.BIRIMAD,TBO.KURKOD ,TBO.MIKTAR ";
            sql += "  FROM " + TABLOADTORBABIRIM + " TBO";
            sql += "    INNER JOIN ButceIslemleri.dbo.MUHMUHASEBE MM ON TBO.MUHASEBE =MM.MUHASEBEKOD ";
            sql += "    INNER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM MMB ON TBO.MUHASEBE=MMB.MUHASEBEKOD  ";
            sql += "    AND TBO.BIRIM=MMB.BIRIMKOD AND TBO.KURKOD=MMB.KURKOD ";

            VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("TBO", ALANLARTORBABIRIM[(int)ENUMAlanlarTorbaBirim.YIL], "=", yil));
            if (muhasebe > 0)
                VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TBO", ALANLARTORBABIRIM[(int)ENUMAlanlarTorbaBirim.MUHASEBE], "=", muhasebe));
            if (!string.IsNullOrEmpty(birim))
                VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TBO", ALANLARTORBABIRIM[(int)ENUMAlanlarTorbaBirim.BIRIM], "=", birim));
            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc TertipSil(int yil,   string tertipkod)
        {
            tertipkod = tertipkod.Replace(".", "");
            string sql = "";
            sql = "DELETE FROM " + TABLOADTORBATERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.YIL], "=", yil));
            if (!string.IsNullOrEmpty(tertipkod))
                VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBATERTIP, ALANLARTORBATERTIP[(int)ENUMAlanlarTorbaTertip.KURKOD], "=", tertipkod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        public Sonuc BirimSil(int yil, int muhasebe, string birim)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOADTORBABIRIM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARTORBABIRIM[(int)ENUMAlanlarTorbaBirim.YIL], "=", yil));
            if (muhasebe > 0)
                VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBABIRIM, ALANLARTORBABIRIM[(int)ENUMAlanlarTorbaBirim.MUHASEBE], "=", muhasebe));
            if (!string.IsNullOrEmpty(birim))
                VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADTORBABIRIM, ALANLARTORBABIRIM[(int)ENUMAlanlarTorbaBirim.BIRIM], "=", birim.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

    }
}
