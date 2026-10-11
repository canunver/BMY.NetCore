using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABBelgeler : TABOrtak
    {
        public TABBelgeler(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYPERSONELBELGELER";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            PKOD = 1,
            TIPKAYITKOD = 2,
            TIP = 3,
            BELGEADI = 4,
            BELGE = 5
        }
        static string[] ALANLAR = { "KOD", "PKOD", "TIPKAYITKOD", "TIP", "BELGEADI", "BELGE" };

        public IDataReader ListeleBelgeler(PersonelBelgeler p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.pKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.pKod));

            if (p.tipKayitKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIPKAYITKOD], "=", p.tipKayitKod));

            if (p.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", p.tip));

            if (!string.IsNullOrEmpty(p.belgeAdi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BELGEADI], "=", p.belgeAdi));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KaydetBelgeler(PersonelBelgeler p)
        {
            string sql = "";
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.pKod) + ",";
            sql += VeriErisim.DegerYap(p.tipKayitKod) + ",";
            sql += VeriErisim.DegerYap(p.tip) + ",";
            sql += VeriErisim.DegerYap(p.belgeAdi) + ",";
            string imgStr = "0x" + System.BitConverter.ToString(p.belge).Replace("-", string.Empty);
            sql += imgStr + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilBelgeler(PersonelBelgeler p)
        {
            string sql = "", kosul = "";

            sql = "DELETE FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            else
            {
                if (p.tipKayitKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIPKAYITKOD], "=", p.tipKayitKod));

                if (p.pKod > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PKOD], "=", p.pKod));

                if (p.tip > 0)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TIP], "=", p.tip));
            }

            if (kosul != "")
                sql += " WHERE " + kosul;

            if (p.kod > 0)
                return KomutCalistir(sql, 1);
            else
                return KomutCalistir(sql, 0);
        }
    }
}
