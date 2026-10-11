using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABEvrakSonraHavaleEt : TABOrtak
    {
        public TABEvrakSonraHavaleEt(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVREVRAKSONRAHAVALEET";
        public enum ENUMAlanlar : int { PREVRAKNO, SIRANO, EVRAKBIRIMKOD, TCKIMLIKNO, HAVALENOT, GONDERILDI }
        public static string[] ALANLAR = { "PREVRAKNO", "SIRANO", "EVRAKBIRIMKOD", "TCKIMLIKNO", "HAVALENOT", "GONDERILDI" };

        public Sonuc Kaydet(EvrakSonraHavaleEt form)
        {
            int siraNo = form.siraNo == 0 ? MaksSiraNo(form.prEvrakNo) : form.siraNo;
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.prEvrakNo) + ",";
            sql += VeriErisim.DegerYap(siraNo) + ",";
            sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(form.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(form.havaleNot) + ",";
            sql += VeriErisim.DegerYap(form.gonderildi) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(EvrakSonraHavaleEt form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", form.prEvrakNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GONDERILDI], "=", form.gonderildi));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc GonderildiOlarakIsaretle(string prEvrakNo)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GONDERILDI], 1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));

            return KomutCalistir(sql, 0);
        }

        public int MaksSiraNo(string prEvrakNo)
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.SIRANO] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PREVRAKNO], "=", prEvrakNo));

            return this.veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}
