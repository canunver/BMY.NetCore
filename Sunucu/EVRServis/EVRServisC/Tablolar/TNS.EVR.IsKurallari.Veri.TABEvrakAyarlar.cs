using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    public class TABEvrakAyarlar : TABOrtak
    {
        public TABEvrakAyarlar(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKAYARLAR";
        public enum ENUMAlanlar { ANAHTARSAHIBI = 0, ANAHTAR = 1, DEGER = 2, DEGER2 = 3, }
        public static string[] ALANLAR = { "ANAHTARSAHIBI", "ANAHTAR", "DEGER", "DEGER2" };

        public Sonuc Kaydet(EvrakAyarlar form)
        {
            Sonuc sonuc = this.Guncelle(form);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.anahtarSahibi) + ",";
                sql += VeriErisim.DegerYap(form.anahtar) + ",";
                sql += VeriErisim.DegerYap(form.deger) + ",";
                sql += VeriErisim.DegerYap(form.deger2) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(EvrakAyarlar form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGER], form.deger) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DEGER2], form.deger2);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAHTARSAHIBI], "=", (form.anahtarSahibi)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAHTAR], "=", (form.anahtar)));

            return KomutCalistir(sql, 1);
        }
        public IDataReader Oku(string anahtarSahibi, int anahtar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAHTARSAHIBI], "=", anahtarSahibi));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAHTAR], "=", anahtar));
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader Listele(EvrakAyarlar[] kriter)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            foreach (EvrakAyarlar form in kriter)
            {
                if (form == null)
                    continue;

                if (kosul != "")
                    kosul += " OR ";

                kosul += "(";
                kosul = VeriErisim.KosulEkle(kosul, "", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAHTARSAHIBI], "=", form.anahtarSahibi));
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAHTAR], "=", form.anahtar));
                kosul += ")";
            }

            if (kosul == "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ANAHTARSAHIBI], "=", "*")); //kriter boş gelirse kayıt gelmesin

            sql += " WHERE " + kosul;
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.ANAHTAR] + " ASC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
