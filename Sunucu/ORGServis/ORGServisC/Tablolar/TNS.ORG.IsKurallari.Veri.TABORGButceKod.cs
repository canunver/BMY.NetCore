using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.ORG;
using TNS.ORG.IsKurallari.Veri;

namespace TNS.ORG.IsKurallari.Veri
{
    public class TABORGButceKod : TABOrtak
    {
        public TABORGButceKod(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ORGBUTCEKOD";

        public enum ENUMAlanlar : int
        {
            ORGBIRIMKOD = 0,
            TUR = 1,
            KURKOD = 2
        }

        static string[] ALANLAR = { "ORGBIRIMKOD", "TUR", "KURKOD" };

        public IDataReader Listele(int orgBirimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TUR];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(int orgBirimKod, OrgButceKod obk)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(obk.tur) + ",";
            sql += VeriErisim.DegerYap(obk.kurKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 0);
        }
    }
}