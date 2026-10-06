using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;
using TNS.ORG;
using TNS.ORG.IsKurallari.Veri;

namespace TNS.ORG.IsKurallari.Veri
{
    public class TABORGHarcamaKod : TABOrtak
    {
        public TABORGHarcamaKod(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "ORGHARCAMAKOD";

        public enum ENUMAlanlar : int
        {
            ORGBIRIMKOD = 0,
            KOD = 1,
            MUHASEBEBIRIM = 2,
            KURKOD = 3,
            FONKOD = 4
        }

        static string[] ALANLAR = { "ORGBIRIMKOD", "KOD", "MUHASEBEBIRIM", "KURKOD", "FONKOD" };

        public IDataReader Listele(int orgBirimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(int orgBirimKod, OrgHarcamaKod ohk)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(ohk.kod) + ",";
            sql += VeriErisim.DegerYap(ohk.muhasebeBirimKod) + ",";
            sql += VeriErisim.DegerYap(ohk.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(ohk.fonKod.Replace(".", "")) + ")";
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