using System;
using System.Data;
using TNS.BKD;
using TNS.Veri;
using TNS.HRC;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABUAIhtiyacPlani : TABOrtak
    {
        public TABUAIhtiyacPlani(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "HRCUAIHTIYACPLANI";

        enum ENUMAlanlar
        {
            KOD = 0,
            ACIKLAMA = 1,
            DURUM = 2
        }

        static string[] ALANLAR = { "KOD", "ACIKLAMA", "DURUM" };

        public Sonuc HesapKodKaydet(ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = string.Empty;
            foreach (IhtiyacPlaniSatir hs in hesapPlani.objeler)
            {
                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(hs.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.aciklama) + ", ";
                sql = sql + VeriErisim.DegerYap(hs.durum) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], hs.aciklama) + ",";
                    sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], hs.durum);
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hs.kod));

                    sonuc = KomutCalistir(sql, 1);
                }

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }
        public Sonuc HesapKodKaydet(IhtiyacPlaniSatir hs)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = string.Empty;
            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(hs.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(hs.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(hs.durum) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";

                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], hs.aciklama) + ",";
                sql += VeriErisim.AtamaOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], hs.durum);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", hs.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public Sonuc HesapKodSil(ObjectArray hesapPlani)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (IhtiyacPlaniSatir hs in hesapPlani.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hs.kod);

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            return sonuc;
        }
        public Sonuc HesapKodSil(IhtiyacPlaniSatir hs)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", hs.kod);

            sonuc = KomutCalistir(sql, 1);

            return sonuc;
        }

        public IDataReader HesapKodListele(IhtiyacPlaniSatir hesKod)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (!string.IsNullOrWhiteSpace(hesKod.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "LIKE", "%" + hesKod.kod + "%"));

            if (!string.IsNullOrEmpty(hesKod.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + hesKod.aciklama + "%"));

            if (hesKod.durum > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", hesKod.durum));

            if (hesKod.kodAciklama != "")
            {
                string ekKosul = string.Empty;
                string[] aranan = hesKod.kodAciklama.Split(' ');

                foreach (string ara in aranan)
                {
                    if (string.IsNullOrEmpty(ara.Trim()))
                        continue;

                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], " LIKE ", ara.Replace(".", "").Trim() + "%"));
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ACIKLAMA], " LIKE ", "%" + ara + "%"));

                    if (!string.IsNullOrWhiteSpace(ekKosul))
                        kosul += "(" + ekKosul + ")";
                }
            }

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];
            IDataReader r = veriErisim.DataReaderDondur(sql);
            return r;
        }


    }
}
