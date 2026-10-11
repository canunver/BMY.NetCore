using System;
using System.Data;
using TNS.IKY.IsKurallari;
using TNS.Veri;

namespace TNS.IKY.IsKurallari.Veri
{
    public class TABUnvan : TABOrtak
    {
        public TABUnvan(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "IKYUNVAN";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            SIRANO = 2,
            ADKISA = 3,
            YONETICI = 4,
            SINIFKOD = 5,
            AKTIF = 6,
            HITAPKOD = 7
        }

        public static string[] ALANLAR = { "KOD", "AD", "SIRANO", "ADKISA", "YONETICI", "SINIFKOD", "AKTIF", "HITAPKOD" };

        public IDataReader Listele(Unvan p)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("U", ALANLAR) + ", S.AD FROM " + TABLOAD + " U LEFT OUTER JOIN " + TABSinif.TABLOAD + " S ON U.SINIFKOD= S.KOD";

            string kosul = string.Empty;
            if (p.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("U", ALANLAR[(int)ENUMAlanlar.KOD], "=", p.kod));

            if (p.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("U", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + p.ad + "%"));

            if (p.siraNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("U", ALANLAR[(int)ENUMAlanlar.SIRANO], "=", p.siraNo));

            if (p.yonetici > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("U", ALANLAR[(int)ENUMAlanlar.YONETICI], "=", p.yonetici));

            if (p.sinifKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("U", ALANLAR[(int)ENUMAlanlar.SINIFKOD], "=", p.sinifKod));

            if (p.aktif)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("U", ALANLAR[(int)ENUMAlanlar.AKTIF], "=", 1));

            if (p.hitapKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("U", ALANLAR[(int)ENUMAlanlar.HITAPKOD], "=", p.hitapKod));

            if (kosul != "")
                sql += " WHERE " + kosul;

            string siralama;
            if (p.listeSiralama == ENUMPersonelSira.Unvan)
            {
                siralama = "case when U.SIRANO IS NULL then 999999999 when U.SIRANO=0 then 999999998 else U.SIRANO end";
            }
            else
                siralama = "U." + ALANLAR[(int)ENUMAlanlar.AD];
            sql += " ORDER BY " + siralama;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Unvan p)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(p.kod) + ",";
            sql += VeriErisim.DegerYap(p.ad) + ",";
            sql += VeriErisim.DegerYap(p.siraNo) + ",";
            sql += VeriErisim.DegerYap(p.sinifAdKisa) + ",";
            sql += VeriErisim.DegerYap(p.yonetici) + ",";

            if (p.sinifKod > 0)
                sql += VeriErisim.DegerYap(p.sinifKod);
            else
                sql += VeriErisim.DegerYap((string)null);
            sql += "," + VeriErisim.DegerYap(p.aktif ? 1 : 0);
            sql += "," + VeriErisim.DegerYap(p.hitapKod) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], p.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], p.siraNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADKISA], p.sinifAdKisa) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YONETICI], p.yonetici) + ",";
                if (p.sinifKod > 0)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFKOD], p.sinifKod);
                else
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SINIFKOD], (string)null);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], p.aktif ? 1 : 0);
                sql += "," + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HITAPKOD], p.hitapKod);
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

        public IDataReader UnvanBazliPersonelSayilariListele(int organizasyonKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
            sql += " FROM " + TABLOAD + ", " + TABPersonelKadro.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABPersonelKadro.TABLOAD + "." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.UNVANKOD];

            if (organizasyonKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.ORGANIZASYONKOD], "=", organizasyonKod));

            sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " ORDER BY " + VeriErisim.IslecBul("COUNT") + "(*) DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader UnvanVeKadroDereceBazliPersonelSayilariListele(int organizasyonKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + TABPersonelKadro.TABLOAD + "." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.KADRODERECE];
            sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
            sql += " FROM " + TABLOAD + ", " + TABPersonelKadro.TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABPersonelKadro.TABLOAD + "." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.UNVANKOD];

            if (organizasyonKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABPersonelKadro.TABLOAD, TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.ORGANIZASYONKOD], "=", organizasyonKod));

            sql += " GROUP BY " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += ", " + TABPersonelKadro.TABLOAD + "." + TABPersonelKadro.ALANLAR[(int)TABPersonelKadro.ENUMALANLAR.KADRODERECE];
            sql += " ORDER BY " + VeriErisim.IslecBul("COUNT") + "(*) DESC";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
