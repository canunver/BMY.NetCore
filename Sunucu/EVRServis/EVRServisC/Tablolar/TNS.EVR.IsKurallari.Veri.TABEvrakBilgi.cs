using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakBilgi : TABOrtak
    {
        public TABEvrakBilgi(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKBILGI";
        public enum ENUMAlanlar : int { PREVRAKNO, KULLANICIID, ACIKLAMA }
        public static string[] ALANLAR = { "PREVRAKNO", "KULLANICIID", "ACIKLAMA" };

        public IDataReader Listele(EvrakBilgi form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            sql += " WHERE " + kosul;
            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(string[] prEvrakNoListe)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql += " WHERE (PREVRAKNO IN (";
            foreach (string prEvrakNo in prEvrakNoListe)
                sql += VeriErisim.DegerYap(prEvrakNo) + ",";
            sql = sql.TrimEnd(',');
            sql += "))";
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }
        public Sonuc Kaydet(EvrakBilgi form)
        {
            Sonuc sonuc = this.Guncelle(form);
            if (!sonuc.islemSonuc)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
                sql += VeriErisim.DegerYap(form.kullaniciId) + ",";
                sql += VeriErisim.DegerYap(form.aciklama) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(EvrakBilgi form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIID], form.kullaniciId) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], form.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));

            return KomutCalistir(sql, 1);
        }
        public Sonuc Sil(EvrakBilgi form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            return KomutCalistir(sql, 1);
        }

    }
}
