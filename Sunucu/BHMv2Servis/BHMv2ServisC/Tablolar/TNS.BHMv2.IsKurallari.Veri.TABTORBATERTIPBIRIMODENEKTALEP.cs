using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    public class TABTORBATERTIPBIRIMODENEKTALEP : TABOrtak
    {
        public TABTORBATERTIPBIRIMODENEKTALEP(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMTORBABIRIMODENEKTALEP";
        public enum ENUMAlanlar
        {
            YIL = 0,
            KURKOD = 1,
            MUHASEBE = 2,
            BIRIM = 3,
            SIRANO = 4,
            ISAD = 5,
            TALEPGEREKCE = 6,
            MIKTAR = 7,
            DURUM = 8,
            REDGEREKCE = 9

        }
        public static string[] ALANLAR = { "YIL", "KURKOD", "MUHASEBE", "BIRIM", "SIRANO", "ISAD", "TALEPGEREKCE", "MIKTAR", "DURUM", "REDGEREKCE" };

        public Sonuc Kaydet(BirimTorbaOdenekTalep talep)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            int sirano = 1;
            foreach (BirimTorbaOdenekTalepDetay item in talep.detay)
            {
                if (!sonuc.islemSonuc) break;
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(talep.yil) + ",";
                sql += VeriErisim.DegerYap(talep.birim.kurkod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(talep.birim.muhasebeKod) + ",";
                sql += VeriErisim.DegerYap(talep.birim.birimKod.Replace(".", "")) + ",";
                sql += VeriErisim.DegerYap(sirano) + ",";
                sql += VeriErisim.DegerYap(item.isadi) + ",";
                sql += VeriErisim.DegerYap(item.talepgerekce) + ",";
                sql += VeriErisim.DegerYap(item.miktar) + ",";
                sql += VeriErisim.DegerYap(item.durum) + ",";
                sql += VeriErisim.DegerYap(item.redgerekce) + ")";
                sonuc = KomutCalistir(sql, 1);
                sirano++;
            }
            return sonuc;
        }

        public IDataReader Listele(int yil, string kurkod, int muhasebekod, string birimkod, int sirano)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurkod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", muhasebekod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birimkod.Replace(".", "")));
            if (sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", sirano));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int yil, string kurkod, int muhasebekod, string birimkod, int sirano)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurkod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", muhasebekod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIM], "=", birimkod.Replace(".", "")));
            if (sirano > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", sirano));
            return KomutCalistir(sql, 0);
        }

        public Sonuc DurumGuncelle(BirimTorbaOdenekTalep talep)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";
            foreach (BirimTorbaOdenekTalepDetay item in talep.detay)
            {
                if (!sonuc.islemSonuc) break;
                sql = " UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], item.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.REDGEREKCE], item.redgerekce);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", talep.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", talep.birim.kurkod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", talep.birim.muhasebeKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], "=", talep.birim.birimKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", item.sirano));
                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

    }
}
