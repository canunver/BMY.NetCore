using System.Data;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    public class TABKurumsalAnaliz : TABOrtak
    {
        public TABKurumsalAnaliz(VeriErisim veriErisim) : base(veriErisim) { }

        #region KurumsalAnalizBolum

        public static string TABLOAD = "STRKURUMSALANALIZ";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            TUR = 2,
            SIRANO = 3,
            METIN = 4
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "TUR", "SIRANO", "METIN" };

        public Sonuc Kaydet(KurumsalAnaliz ka)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ka.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(ka.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(ka.tur) + ",";
            sql += VeriErisim.DegerYap(ka.siraNo) + ",";
            sql += VeriErisim.DegerYap(ka.metin) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(KurumsalAnaliz ka)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", ka.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", ka.orgBirimKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(KurumsalAnaliz ka, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (ka.donem != null && ka.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", ka.donem.donemNo);
            if (!sorgu && ka.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", ka.orgBirimKod));
            if (ka.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", ka.tur));
            if (!string.IsNullOrEmpty(ka.metin))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.METIN], "LIKE", "%" + ka.metin + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], ka.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        #endregion

        #region SablonBolum

        public static string TABLOADSABLON = "STRKURUMSALANALIZSABLON";

        public enum ENUMAlanlarSablon : int
        {
            DONEMNO = 0,
            KOD = 1,
            AD = 2
        }

        public static string[] ALANLARSABLON = { "DONEMNO", "KOD", "AD" };

        public Sonuc KaydetSablon(KurumsalAnalizSablon sablon)
        {
            sablon.kod = SablonSiraNoAl(sablon);

            string sql = "INSERT INTO " + TABLOADSABLON + " (" + VeriErisim.TumAlanlar("", ALANLARSABLON) + ") VALUES (";
            sql += VeriErisim.DegerYap(sablon.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(sablon.kod) + ",";
            sql += VeriErisim.DegerYap(sablon.ad) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = sablon.kod.ToString();
            return sonuc;
        }

        public Sonuc GuncelleSablon(KurumsalAnalizSablon sablon)
        {
            string sql = "UPDATE " + TABLOADSABLON + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.AD], sablon.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.DONEMNO], "=", sablon.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.KOD], "=", sablon.kod));

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = sablon.kod.ToString();
            return sonuc;
        }

        private int SablonSiraNoAl(KurumsalAnalizSablon sablon)
        {
            string[] intAlan = { ALANLARSABLON[(int)ENUMAlanlarSablon.DONEMNO] };
            int[] intDeger = { sablon.donem.donemNo };
            return TABGenel.SayacDegerAl(veriErisim, TABLOADSABLON, ALANLARSABLON[(int)ENUMAlanlarSablon.KOD], new string[0], new string[0], intAlan, intDeger);
        }

        public Sonuc SilSablon(KurumsalAnalizSablon sablon)
        {
            string sql = "DELETE FROM " + TABLOADSABLON;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.DONEMNO], "=", sablon.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.KOD], "=", sablon.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader ListeleSablon(KurumsalAnalizSablon sablon)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSABLON) + " FROM " + TABLOADSABLON;

            string kosul = string.Empty;
            if (sablon.donem != null && sablon.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.DONEMNO], "=", sablon.donem.donemNo);
            if (sablon.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSABLON[(int)ENUMAlanlarSablon.KOD], "=", sablon.kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        #endregion

        #region SablonDetayBolum

        public static string TABLOADSABLONDETAY = "STRKURUMSALANALIZSABLONDETAY";

        public enum ENUMAlanlarSablonDetay : int
        {
            DONEMNO = 0,
            KOD = 1,
            TUR = 2,
            SIRANO = 3,
            METIN = 4
        }

        public static string[] ALANLARSABLONDETAY = { "DONEMNO", "KOD", "TUR", "SIRANO", "METIN" };

        public Sonuc KaydetSablonDetay(KurumsalAnaliz ka)
        {
            string sql = "INSERT INTO " + TABLOADSABLONDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARSABLONDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(ka.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(ka.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(ka.tur) + ",";
            sql += VeriErisim.DegerYap(ka.siraNo) + ",";
            sql += VeriErisim.DegerYap(ka.metin) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilSablonDetay(KurumsalAnaliz ka)
        {
            string sql = "DELETE FROM " + TABLOADSABLONDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARSABLONDETAY[(int)ENUMAlanlarSablonDetay.DONEMNO], "=", ka.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARSABLONDETAY[(int)ENUMAlanlarSablonDetay.KOD], "=", ka.orgBirimKod));

            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleSablonDetay(KurumsalAnaliz ka)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSABLONDETAY) + " FROM " + TABLOADSABLONDETAY;

            string kosul = string.Empty;
            if (ka.donem != null && ka.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSABLONDETAY[(int)ENUMAlanlarSablonDetay.DONEMNO], "=", ka.donem.donemNo));
            if (ka.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARSABLONDETAY[(int)ENUMAlanlarSablonDetay.KOD], "=", ka.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        #endregion
    }
}