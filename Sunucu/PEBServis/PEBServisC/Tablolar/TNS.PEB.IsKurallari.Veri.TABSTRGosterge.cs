using System.Data;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABSTRGosterge : TABOrtak
    {
        public TABSTRGosterge(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRGOSTERGE";
        public static string TABLOADGERCEKLESME = "STRGOSTERGEGERCEKLESME";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            AD = 1,
            BIRIM = 2,
            BASARIMTURU = 3,
            GOSTERGETURU = 4,
            HESAPLAMAYONTEMI = 5,
            VERIKAYNAGI = 6,
            VERIKAYNAGISISTEMI = 7,
            EKPARAM = 8,
            VERITOPLAMAPERIYODU = 9,
            RAPORLAMAPERIYODU = 10,
            ACIKLAMA = 11,
            DURUM = 12,
            BASLANGICTARIHI = 13,
            BITISTARIHI = 14
        }

        public enum ENUMAlanlarGERCEKLESME : int
        {
            BIRIMKOD = 0,
            GOSTERGEKOD = 1,
            YIL = 2,
            AY = 3,
            DEGER = 4
        }

        public static string[] ALANLAR = { "KOD", "AD", "BIRIM", "BASARIMTURU", "GOSTERGETURU", "HESAPLAMAYONTEMI", "VERIKAYNAGI",
                                           "VERIKAYNAGISISTEMI", "EKPARAM", "VERITOPLAMAPERIYODU", "RAPORLAMAPERIYODU", "ACIKLAMA",
                                           "DURUM", "BASLANGICTARIHI", "BITISTARIHI" };

        public static string[] ALANLARGERCEKLESME = { "BIRIMKOD", "GOSTERGEKOD", "YIL", "AY", "DEGER" };

        public static string TABLOADBAG = "STRGOSTERGEBAG";

        public enum ENUMAlanlarBag : int
        {
            GOSTERGEKOD = 0,
            BAGLIKOD = 1
        }

        public static string[] ALANLARBAG = { "GOSTERGEKOD", "BAGLIKOD" };

        /*
         * 
         create table STRGOSTERGEGERCEKLESME (
            BIRIMKOD int, GOSTERGEKOD int,  YIL int, AY int, DEGER money,
            primary key (BIRIMKOD, GOSTERGEKOD, YIL, AY)
         )
         * 
         * 
         * */
        public Sonuc Kaydet(STRGosterge gosterge)
        {
            gosterge.kod = GostergeKodAl();

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(gosterge.kod) + ",";
            sql += VeriErisim.DegerYap(gosterge.ad) + ",";
            sql += VeriErisim.DegerYap(gosterge.birim) + ",";
            sql += VeriErisim.DegerYap(gosterge.basarimTuru) + ",";
            sql += VeriErisim.DegerYap(gosterge.gostergeTuru) + ",";
            sql += VeriErisim.DegerYap(gosterge.hesaplamaYontemi) + ",";
            sql += VeriErisim.DegerYap(gosterge.veriKaynagi) + ",";
            sql += VeriErisim.DegerYap(gosterge.veriKaynagiSistemi) + ",";
            sql += VeriErisim.DegerYap(gosterge.ekParam) + ",";
            sql += VeriErisim.DegerYap(gosterge.veriToplamaPeriyodu) + ",";
            sql += VeriErisim.DegerYap(gosterge.raporlamaPeriyodu) + ",";
            sql += VeriErisim.DegerYap(gosterge.aciklama) + ",";
            sql += VeriErisim.DegerYap(gosterge.durum) + ",";
            sql += VeriErisim.DegerYap(gosterge.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(gosterge.bitisTarihi) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = gosterge.kod.ToString();
            return sonuc;
        }

        public Sonuc Guncelle(STRGosterge gosterge)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], gosterge.ad) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIM], gosterge.birim) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASARIMTURU], gosterge.basarimTuru) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GOSTERGETURU], gosterge.gostergeTuru) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPLAMAYONTEMI], gosterge.hesaplamaYontemi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERIKAYNAGI], gosterge.veriKaynagi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERIKAYNAGISISTEMI], gosterge.veriKaynagiSistemi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EKPARAM], gosterge.ekParam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VERITOPLAMAPERIYODU], gosterge.veriToplamaPeriyodu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.RAPORLAMAPERIYODU], gosterge.raporlamaPeriyodu) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], gosterge.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], gosterge.durum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIHI], gosterge.bitisTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gosterge.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(STRGosterge gosterge, params string[] bagliKodlar)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string baglac = "WHERE";
            if (gosterge.kod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, baglac, VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", gosterge.kod));
                baglac = "AND";
            }

            if (gosterge.gostergeTuru > 0)
            {
                sql = VeriErisim.KosulEkle(sql, baglac, VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOSTERGETURU], "=", gosterge.gostergeTuru));
                baglac = "AND";
            }

            if (gosterge.durum != (int)EnumSPGenelHavuzDurum.Hepsi)
            {
                sql = VeriErisim.KosulEkle(sql, baglac, VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", gosterge.durum));
                baglac = "AND";
            }

            if (bagliKodlar != null && bagliKodlar.Length > 0)
            {
                string bagliKodKosul = string.Empty;
                foreach (string bagliKod in bagliKodlar)
                {
                    if (!string.IsNullOrEmpty(bagliKodKosul))
                        bagliKodKosul += ", ";
                    if (!string.IsNullOrEmpty(bagliKod))
                        bagliKodKosul += VeriErisim.DegerYap(bagliKod.Replace(".", ""));
                }

                if (!string.IsNullOrEmpty(bagliKodKosul))
                {
                    sql = VeriErisim.KosulEkle(sql, baglac, "EXISTS (SELECT 1 FROM " + TABLOADBAG);
                    sql = VeriErisim.KosulEkle(sql, "WHERE", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " = " + TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.GOSTERGEKOD]);
                    sql = VeriErisim.KosulEkle(sql, "AND", TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.BAGLIKOD] + " IN (" + bagliKodKosul + "))");
                }
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(STRGosterge gosterge)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gosterge.kod));

            return KomutCalistir(sql, 1);
        }

        private int GostergeKodAl()
        {
            return TABGenel.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD]);
        }

        public Sonuc KaydetBag(STRGostergeBag gostergeBag)
        {
            string sql = "INSERT INTO " + TABLOADBAG + " (" + VeriErisim.TumAlanlar("", ALANLARBAG) + ") VALUES (";
            sql += VeriErisim.DegerYap(gostergeBag.gostergeKod) + ",";
            sql += VeriErisim.DegerYap(gostergeBag.bagliKod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleBag(int gostergeKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARBAG) + " FROM " + TABLOADBAG;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBAG[(int)ENUMAlanlarBag.GOSTERGEKOD], "=", gostergeKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilBag(int gostergeKod)
        {
            string sql = "DELETE FROM " + TABLOADBAG;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARBAG[(int)ENUMAlanlarBag.GOSTERGEKOD], "=", gostergeKod));

            return KomutCalistir(sql, 0);
        }

        public bool AyniAdVarmi(int kod, string ad)
        {
            if (string.IsNullOrEmpty(ad))
                return false;

            string sql = "SELECT COUNT(*) FROM " + TABLOAD;

            string kosul = string.Empty;
            if (kod > 0)
                kosul = VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.KOD], "<>", kod);
            if (!string.IsNullOrEmpty(ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(string.Empty, ALANLAR[(int)ENUMAlanlar.AD], "=", ad));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }

        public Sonuc GerceklesmeSil(int orgBirimKod, int gostergeKod, int yil, int ay)
        {
            string sql = "DELETE FROM " + TABLOADGERCEKLESME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.BIRIMKOD], "=", orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.YIL], "=", yil));
            if (gostergeKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.GOSTERGEKOD], "=", gostergeKod));
            if (ay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.AY], "=", ay));

            return KomutCalistir(sql, 0);
        }

        public Sonuc GerceklesmeKaydet(int orgBirimKod, int gostergeKod, int yil, int ay, decimal deger)
        {
            string sql = "INSERT INTO " + TABLOADGERCEKLESME + " (" + VeriErisim.TumAlanlar("", ALANLARGERCEKLESME) + ") VALUES (";
            sql += VeriErisim.DegerYap(orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(gostergeKod) + ",";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(ay) + ",";
            sql += VeriErisim.DegerYap(deger) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader GerceklesmeListele(int orgBirimKod, int yil, int ay, params int[] gostergeKodlar)
        {
            string sql = "SELECT * FROM " + TABLOADGERCEKLESME;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.BIRIMKOD], "=", orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.YIL], "=", yil));
            if (ay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.AY], "=", ay));

            if (gostergeKodlar != null)
            {
                string gostergeKodStr = "";
                for (int i = 0; i < gostergeKodlar.Length; i++)
                {
                    if (!string.IsNullOrEmpty(gostergeKodStr)) gostergeKodStr += ",";
                    gostergeKodStr += gostergeKodlar[i].ToString();
                }
                if (!string.IsNullOrEmpty(gostergeKodStr))
                    sql = sql + " AND " + ALANLARGERCEKLESME[(int)ENUMAlanlarGERCEKLESME.GOSTERGEKOD] + " IN " + "(" + gostergeKodStr + ")";
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public bool BagVarMi(int gostergeKod, params string[] bagliKod)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*) FROM " + TABLOADBAG;

            string kosul = string.Empty;
            if (gostergeKod > 0)
                kosul = VeriErisim.KosulOl(TABLOADBAG, ALANLARBAG[(int)ENUMAlanlarBag.GOSTERGEKOD], "=", gostergeKod);

            if (bagliKod != null && bagliKod.Length > 0)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOADBAG + "." + ALANLARBAG[(int)ENUMAlanlarBag.BAGLIKOD] + " IN (");
                foreach (string bk in bagliKod)
                    kosul += " " + VeriErisim.DegerYap(bk.Replace(".", "")) + ",";
                kosul = kosul.Substring(0, kosul.Length - 1) + ")";
            }

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }
    }
}