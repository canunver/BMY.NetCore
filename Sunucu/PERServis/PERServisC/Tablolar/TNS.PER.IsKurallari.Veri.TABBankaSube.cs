using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABBankaSube : TABOrtak
    {
        public TABBankaSube(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PERBANKASUBE";
        enum ENUMAlanlar : int
        {
            KOD = 0,
            BANKAKODU = 1,
            AD = 2,
        }
        static string[] ALANLAR ={ "KOD", "BANKAKODU", "AD" };

        static string TABLOADBANKA = "PERBANKA";
        enum ENUMAlanlarBanka : int
        {
            KOD = 0,
            AD = 1,
        }
        static string[] ALANLARBANKA ={ "KOD", "AD" };

        public IDataReader Listele(BankaSube p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADBANKA, ALANLARBANKA) + " FROM " + TABLOAD + "," + TABLOADBANKA;

            kosul = TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BANKAKODU] + "=" + TABLOADBANKA + "." + ALANLAR[(int)ENUMAlanlarBanka.KOD];

            if (p.bankaKodu !="")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BANKAKODU], "=", p.bankaKodu));

            if (p.kod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + p.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY 2,3,4";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(BankaSube p)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.bankaKodu) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BANKAKODU], "=", p.bankaKodu));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc Sil(BankaSube p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BANKAKODU], "=", p.bankaKodu));

            return KomutCalistir(sql, 0);
        }
    }
}
