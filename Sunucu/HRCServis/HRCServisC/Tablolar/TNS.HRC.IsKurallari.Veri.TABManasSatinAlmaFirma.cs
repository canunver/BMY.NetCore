
using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABManasSatinAlmaFirma : TABOrtak
    {
        public TABManasSatinAlmaFirma(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MANASSATINALMAFIRMA";
        public enum ENUMAlanlar { SATINALMAKOD = 0, REFID = 1, TUR = 2, SIRANO = 3, VERGINO = 4, UNVAN = 5 }
        public static string[] ALANLAR = { "SATINALMAKOD", "REFID", "TUR", "SIRANO", "VERGINO", "UNVAN" };

        public Sonuc Kaydet(ManasSatinAlmaFirma form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.satinAlmaKod) + ",";
            sql += VeriErisim.DegerYap(form.refId) + ",";
            sql += VeriErisim.DegerYap(form.tur) + ",";
            sql += VeriErisim.DegerYap(form.siraNo) + ",";
            sql += VeriErisim.DegerYap(form.vergiNo) + ",";
            sql += VeriErisim.DegerYap(form.unvan) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ManasSatinAlmaFirma form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", form.satinAlmaKod));
            if (form.siraNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", form.siraNo));
            if (form.tur > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", form.tur));
            if (form.refId != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.REFID], "=", form.refId));

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SATINALMAKOD] + "," + ALANLAR[(int)ENUMAlanlar.SIRANO];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int satinAlmaKod, int tur)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", satinAlmaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));

            return KomutCalistir(sql, 0);
        }
    }
}
