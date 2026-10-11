using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABVeritabani : TABOrtak
    {
        public TABVeritabani(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "GUNCELLEMEBILGI";

        public enum ENUMAlanlar : int
        {
            KOD = 0, ISLEMTARIH = 1
        }

        public static string[] ALANLAR = { "KOD", "ISLEMTARIH" };

        public Sonuc Kaydet(string kod, TNSDateTime islemTarihi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(islemTarihi) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(string kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(string kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            if (kod != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 0);
        }

    }
}
