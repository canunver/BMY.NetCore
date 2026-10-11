using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakMeta : TABOrtak
    {
        public TABEvrakMeta(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKMETA";
        public enum ENUMAlanlar : int { EVREVRAKNO, METAKOD, METADEGER, METADEGER2 }
        public static string[] ALANLAR = { "EVREVRAKNO", "METAKOD", "METADEGER", "METADEGER2" };

        public IDataReader Listele(EvrakMeta form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", form.evrEvrakNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(EvrakMeta form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.evrEvrakNo) + ",";
            sql += VeriErisim.DegerYap(form.meta.kod) + ",";
            sql += VeriErisim.DegerYap(form.metaDeger) + ",";
            sql += VeriErisim.DegerYap(form.metaDeger2) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilMeta(string evrEvrakNo)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVREVRAKNO], "=", evrEvrakNo));
            return KomutCalistir(sql, 0);
        }

    }
}
