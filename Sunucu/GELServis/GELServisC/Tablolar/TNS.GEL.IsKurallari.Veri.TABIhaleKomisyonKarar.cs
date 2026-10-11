using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;


namespace TNS.GEL.IsKurallari.Veri
{
    public class TABIhaleKomisyonKarar : TABOrtak
    {
        public TABIhaleKomisyonKarar(VeriErisim veriErisim) : base(veriErisim) { }
        public static string TABLOAD = "GELIHLKOMISYONKARAR";
        public enum ENUMAlanlar
        {
            KARARNO = 0,
            YIL = 1,
            MUHASEBEKOD = 2,
            BIRIMKOD = 3,
            SURECNO = 4,
            TASINMAZKODU = 5,
            TEKLIFTIPI = 6,
            UYGUNFIRMA1 = 7,
            TUTARFIRMA1 = 8,
            UYGUNFIRMA2 = 9,
            TUTARFIRMA2 = 10,
            ACIKLAMA = 11,
            KARARTARIHI = 12
        }
        public static string[] ALANLAR = { "KARARNO", "YIL", "MUHASEBEKOD", "BIRIMKOD", "SURECNO", "TASINMAZKODU", "TEKLIFTIPI", "UYGUNFIRMA1", "TUTARFIRMA1", "UYGUNFIRMA2", "TUTARFIRMA2", "ACIKLAMA", "KARARTARIHI" };


        public Sonuc Kaydet(KomisyonKarar bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kararno) + ",";
            sql += VeriErisim.DegerYap(bilgi.yil) + ",";
            sql += VeriErisim.DegerYap(bilgi.muhasebe) + ",";
            sql += VeriErisim.DegerYap(bilgi.birim) + ",";
            sql += VeriErisim.DegerYap(bilgi.surecNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.tasinmaz.tasinmazSistemKodu) + ",";
            sql += VeriErisim.DegerYap(bilgi.tekliftipi) + ",";
            sql += VeriErisim.DegerYap(bilgi.uygunfirma1.firmaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.uygunfirma1.tekliftutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.uygunfirma2.firmaNo) + ",";
            sql += VeriErisim.DegerYap(bilgi.uygunfirma2.tekliftutari) + ",";
            sql += VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql += VeriErisim.DegerYap(bilgi.karartarihi) + ")";
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Guncelle(KomisyonKarar bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], bilgi.yil) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], bilgi.muhasebe) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], bilgi.birim) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], bilgi.surecNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKODU], bilgi.tasinmaz.tasinmazSistemKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFTIPI], bilgi.tekliftipi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UYGUNFIRMA1], bilgi.uygunfirma1.firmaNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTARFIRMA1], bilgi.uygunfirma1.tekliftutari) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.UYGUNFIRMA2], bilgi.uygunfirma2.firmaNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUTARFIRMA2], bilgi.uygunfirma2.tekliftutari) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KARARTARIHI], bilgi.karartarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KARARNO], "=", bilgi.kararno));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;

        }

        public Sonuc Sil(KomisyonKarar bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";
            sql = "DELETE  FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KARARNO], "=", bilgi.kararno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKODU], "=", bilgi.tasinmaz.tasinmazSistemKodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFTIPI], "=", bilgi.tekliftipi));
            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
 
        public IDataReader Listele(KomisyonKarar bilgi)
        {
            string sql = "";
            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql += " WHERE 1=1 ";
            if (bilgi.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.muhasebe > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebe));
            if (!string.IsNullOrEmpty(bilgi.birim))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birim));
            if (!string.IsNullOrEmpty(bilgi.surecNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", bilgi.surecNo));
            if (bilgi.tekliftipi > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.TEKLIFTIPI], "=", bilgi.tekliftipi));
            if(bilgi.uygunfirma1.firmaNo>0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.UYGUNFIRMA1], "=", bilgi.uygunfirma1.firmaNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public bool KayitVarmi(int yil, int muhasebe, string birim, string surecno, string tasinmazkodu, int tekliftipi)
        {
            bool kayitVar = false;
            string sql = "";
            sql = "SELECT COUNT(*) FROM " + TABLOAD;
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KARARNO], "=", bilgi.kararno));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birim));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", surecno));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TASINMAZKODU], "=", tasinmazkodu));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TEKLIFTIPI], "=", tekliftipi));
            kayitVar = veriErisim.TekKolonSQLInt(sql) > 0;
            return kayitVar;
        
        }

    }
}
