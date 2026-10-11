using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABOgrenim : TABOrtak
    {
        public TABOgrenim(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "PEROGRENIM";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            SIRANO = 2,
            TIP = 3
        }

        public static string[] ALANLAR = { "KOD", "AD", "SIRANO", "TIP"};

        public IDataReader Listele(Ogrenim p)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + p.ad + "%"));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", p.siraNo));

            if (p.tip > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TIP], "=", p.tip));


            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.TIP] + "," + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Ogrenim p)
        {
            string sql = "";
            p.kod = TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "");

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.tip) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(Ogrenim p)
        {
            string sql = "";

            sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], p.siraNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], p.tip);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(Ogrenim p)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader OgrenimBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
            sql += " FROM " + TABLOAD + ", " + TABPersonel.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABPersonel.TABLOAD + "." + TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.OGRENIMKODU];

            if (muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.MUHASEBEKODU], "=", muhasebeKod));

            if (!string.IsNullOrEmpty(birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.BIRIMKODU], "=", birimKod));

            sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " ORDER BY " + VeriErisim.IslecBul("COUNT") + "(*) DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
