using System;
using System.Data;
using TNS.PER.IsKurallari;
using TNS.Veri;

namespace TNS.PER.IsKurallari.Veri
{
    public class TABUnvan : TABOrtak
    {
        public TABUnvan(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "PERUNVAN";

        enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            ADKISA = 2,
            SIRANO = 3,
            YONETICI = 4
        }
        static string[] ALANLAR = { "KOD", "AD", "ADKISA", "SIRANO", "YONETICI" };

        public IDataReader Listele(Unvan p)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + p.ad + "%"));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", p.siraNo));

            if (p.yonetici > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YONETICI], "=", p.yonetici));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Unvan p)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.adKisa) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.yonetici) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADKISA], p.adKisa) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], p.siraNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YONETICI], p.yonetici);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        public Sonuc Sil(Unvan p)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader UnvanBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
            sql += " FROM " + TABLOAD + ", " + TABPersonel.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABPersonel.TABLOAD + "." + TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.UNVANKODU];

            if (muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.MUHASEBEKODU], "=", muhasebeKod));

            if (!string.IsNullOrEmpty(birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.BIRIMKODU], "=", birimKod));

            sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " ORDER BY " + VeriErisim.IslecBul("COUNT") + "(*) DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UnvanVeKadroDereceBazliPersonelSayilariListele(int muhasebeKod, string birimKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + TABPersonel.TABLOAD + "." + TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.KADRODERECE];
            sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
            sql += " FROM " + TABLOAD + ", " + TABPersonel.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABPersonel.TABLOAD + "." + TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.UNVANKODU];

            if (muhasebeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.MUHASEBEKODU], "=", muhasebeKod));

            if (!string.IsNullOrEmpty(birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonel.TABLOAD, TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.BIRIMKODU], "=", birimKod));

            sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + TABPersonel.TABLOAD + "." + TABPersonel.ALANLAR[(int)TABPersonel.ENUMAlanlar.KADRODERECE];
            sql += " ORDER BY " + VeriErisim.IslecBul("COUNT") + "(*) DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
