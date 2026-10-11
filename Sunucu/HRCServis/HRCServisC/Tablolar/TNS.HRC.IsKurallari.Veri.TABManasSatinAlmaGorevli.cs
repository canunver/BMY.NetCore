
using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABManasSatinAlmaGorevli : TABOrtak
    {
        public TABManasSatinAlmaGorevli(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MANASSATINALMAGOREVLI";
        public enum ENUMAlanlar { SATINALMAKOD = 0, SIRANO = 1, YETKILI = 2, UNVAN = 3 }
        public static string[] ALANLAR = { "SATINALMAKOD", "SIRANO", "YETKILI", "UNVAN", };

        public Sonuc Kaydet(ManasSatinAlmaGorevli form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.satinAlmaKod) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.yetkili) + ",";
            sql += VeriErisim.DegerYap(form.unvan) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ManasSatinAlmaGorevli form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", form.satinAlmaKod));
            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int satinAlmaKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", satinAlmaKod));

            return KomutCalistir(sql, 0);
        }
    }
}
