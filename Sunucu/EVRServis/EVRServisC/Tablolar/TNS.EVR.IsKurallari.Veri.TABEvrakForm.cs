using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakForm : TABOrtak
    {
        public TABEvrakForm(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKFORM";
        public enum ENUMAlanlar : int { EVREVRAKNO, ALANADI, DEGER }
        public static string[] ALANLAR = { "EVREVRAKNO", "ALANADI", "DEGER" };

        public Sonuc Kaydet(EvrakForm form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.alanAdi) + ",";
            sql += VeriErisim.DegerYap(form.deger) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakForm form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));
            if (form.alanAdi != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALANADI], "=", form.alanAdi));
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string evrEvrakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", evrEvrakNo));
            return KomutCalistir(sql, 0);
        }

    }
}

