using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakBilgiIliski : TABOrtak
    {
        public TABEvrakBilgiIliski(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKBILGIILISKI";
        public enum ENUMAlanlar : int { PREVRAKNO, PREVRAKNOILISKI, EVRAKNOILISKI, ONAY }
        public static string[] ALANLAR = { "PREVRAKNO", "PREVRAKNOILISKI", "EVRAKNOILISKI", "ONAY" };

        public IDataReader Listele(EvrakBilgiIliski form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("EVREVRAKBILGIILISKI", ALANLAR) + " , EVREVRAKBILGI.KULLANICIID, EVREVRAKBILGI.ACIKLAMA";
            sql += " FROM EVREVRAKBILGIILISKI LEFT OUTER JOIN EVREVRAKBILGI ON EVREVRAKBILGIILISKI.PREVRAKNOILISKI = EVREVRAKBILGI.PREVRAKNO ";

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKBILGIILISKI", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.evrakBilgi.prEvrakNo));

            if (!string.IsNullOrEmpty(form.prEvrakNoIliski))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("EVREVRAKBILGIILISKI", ALANLAR[(int)ENUMAlanlar.PREVRAKNOILISKI], "=", form.prEvrakNoIliski));

            sql += " WHERE " + kosul;
            sql += " ORDER BY 2";
            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(EvrakBilgiIliski form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrakBilgi.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.prEvrakNoIliski) + ",";
            sql += VeriErisim.DegerYap(form.evrakNoIliski) + ",";
            sql += VeriErisim.DegerYap(form.onay) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
                sonuc = this.Guncelle(form);
            return sonuc;
        }
        public Sonuc Guncelle(EvrakBilgiIliski form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAY], form.onay);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.evrakBilgi.prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNOILISKI], "=", form.prEvrakNoIliski));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Sil(EvrakBilgiIliski form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.evrakBilgi.prEvrakNo));
            if (!string.IsNullOrEmpty(form.prEvrakNoIliski))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNOILISKI], "=", form.prEvrakNoIliski));

            return KomutCalistir(sql, 0);
        }

    }
}
