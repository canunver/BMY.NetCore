using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    public class TABKEPGonderi : TABOrtak
    {
        public TABKEPGonderi(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRKEPGONDERI";
        public enum ENUMAlanlar { KOD = 0, EVRAKBIRIMKODU = 1, EVREVRAKNO = 2, KIMDEN = 3, KIME = 4, ILGIKISI = 5, KONU = 6, TARIH = 7, ISLEMYAPAN = 8, DURUM = 9, MIME = 10, SMIME = 11 }
        public static string[] ALANLAR = { "KOD", "EVRAKBIRIMKODU", "EVREVRAKNO", "KIMDEN", "KIME", "ILGIKISI", "KONU", "TARIH", "ISLEMYAPAN", "DURUM", "MIME", "SMIME" };

        public Sonuc Kaydet(KEPGonderi form)
        {
            Sonuc sonuc = this.Guncelle(form);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
                sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.kimden) + ",";
                sql += VeriErisim.DegerYap(form.kime) + ",";
                sql += VeriErisim.DegerYap(form.ilgiKisi) + ",";
                sql += VeriErisim.DegerYap(form.konu) + ",";
                sql += VeriErisim.DegerYap(form.tarih) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.durum) + ",";
                sql += VeriErisim.DegerYap(form.mime) + ",";
                sql += VeriErisim.DegerYap(form.smime) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(KEPGonderi form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], form.evrakBirimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], form.evrEvrakNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIME], form.kime) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KIMDEN], form.kimden) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ILGIKISI], form.ilgiKisi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KONU], form.konu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH], form.tarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIME], form.mime);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleSMIME(KEPGonderi form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], 1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SMIME], form.smime);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncelleDurum(KEPGonderi form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], form.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Oku(KEPGonderi gonder)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (gonder.evrakBirimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", gonder.evrakBirimKod));

            if (gonder.kimden != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KIMDEN], "=", gonder.kimden));

            if (gonder.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gonder.kod));

            if (gonder.evrEvrakNo != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", gonder.evrEvrakNo));

            if (gonder.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", gonder.durum));

            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
