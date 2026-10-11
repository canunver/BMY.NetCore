using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABHitapx : TABOrtak
    {
        public TABHitapx(VeriErisim veriErisim) : base(veriErisim) { }
        public static string TABLOAD = "IKYHITAP";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            TIPKAYITKOD = 2,
            TIP = 3,
            HITAPKAYITNO = 4,
            ISLEMTURU = 5

        }
        public static string[] ALANLAR = { "KOD", "PKOD", "TIPKAYITKOD", "TIP", "HITAPKAYITNO", "ISLEMTURU" };

        public Sonuc Kaydet(IKYHitap hitap)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hitap.kod) + ",";
            sql += VeriErisim.DegerYap(hitap.personelKod) + ",";
            sql += VeriErisim.DegerYap(hitap.tipKayitKod) + ",";
            sql += VeriErisim.DegerYap(hitap.tip) + ",";
            sql += VeriErisim.DegerYap(hitap.hitapKayitNo) + ",";
            sql += VeriErisim.DegerYap(hitap.islemTuru) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(IKYHitap hitap)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIPKAYITKOD], hitap.tipKayitKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], hitap.tip) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HITAPKAYITNO], hitap.hitapKayitNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTURU], hitap.islemTuru);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hitap.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", hitap.personelKod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Sorgula(IKYHitap hitap)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (hitap.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hitap.kod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

    }
}
