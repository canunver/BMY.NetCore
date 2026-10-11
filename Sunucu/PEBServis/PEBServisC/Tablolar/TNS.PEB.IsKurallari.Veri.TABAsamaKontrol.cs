using System.Data;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik amaç bilgilerini tutan STRGENELHAVUZ veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABAsamaKontrol : TABOrtak
    {
        public TABAsamaKontrol(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRASAMAKONTROL";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            KONUNO = 2,
            STATU = 3,
            BASLAMATARIHI = 4,
            TAMAMLANMATARIHI = 5,
            ISLEMYAPANBIRIM = 6,
            ISLEMTARIHI = 7,
            ISLEMYAPAN = 8
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "KONUNO", "STATU", "BASLAMATARIHI", "TAMAMLANMATARIHI",
                                             "ISLEMYAPANBIRIM", "ISLEMTARIHI", "ISLEMYAPAN" };

        public static string TABLOADDETAY = "STRASAMAKONTROLDETAY";

        public enum ENUMAlanlarDetay : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            KONUNO = 2,
            SIRANO = 3,
            KONTROLLISTESI = 4,
            BIRIMONAYI = 5,
            BIRIMACIKLAMA = 6,
            USTBIRIMONAYI = 7,
            USTBIRIMACIKLAMA = 8
        }

        public static string[] ALANLARDETAY = { "DONEMNO", "ORGBIRIMKOD", "KONUNO", "SIRANO", "KONTROLLISTESI",
                                                  "BIRIMONAYI", "BIRIMACIKLAMA", "USTBIRIMONAYI", "USTBIRIMACIKLAMA" };

        public static string TABLOADGECMIS = "STRASAMAKONTROLGECMIS";

        public enum ENUMAlanlarGecmis : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            KONUNO = 2,
            SIRANO = 3,
            ISLEM = 4,
            ISLEMIYAPAN = 5,
            TARIH = 6
        }

        public static string[] ALANLARGECMIS = { "DONEMNO", "ORGBIRIMKOD", "KONUNO", "SIRANO", "ISLEM", "ISLEMIYAPAN", "TARIH" };

        public Sonuc Kaydet(AsamaKontrol askon)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(askon.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(askon.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(askon.konuNo) + ",";
            sql += VeriErisim.DegerYap((int)askon.statu) + ",";
            sql += VeriErisim.DegerYap(askon.baslamaTarihi) + ",";
            sql += VeriErisim.DegerYap(askon.tamamlanmaTarihi) + ",";
            sql += VeriErisim.DegerYap(askon.islemYapanBirim) + ",";
            sql += VeriErisim.DegerYap(askon.islemTarihi) + ",";
            sql += VeriErisim.DegerYap(askon.islemYapan) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(AsamaKontrol askon)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.STATU], (int)askon.statu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAMAMLANMATARIHI], askon.tamamlanmaTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANBIRIM], askon.islemYapanBirim) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], askon.islemTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], askon.islemYapan);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", askon.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", askon.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KONUNO], "=", askon.konuNo));

            return KomutCalistir(sql, 1);
        }

        public bool AsamaKontrolKayitVarmi(AsamaKontrol askon)
        {
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.DONEMNO], ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], ALANLAR[(int)ENUMAlanlar.KONUNO] };
            int[] intDeger = { askon.donem.donemNo, askon.orgBirimKod, askon.konuNo };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }

        public Sonuc KaydetDetay(AsamaKontrolDetay askonDetay)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(askonDetay.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(askonDetay.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(askonDetay.konuNo) + ",";
            sql += VeriErisim.DegerYap(askonDetay.siraNo) + ",";
            sql += VeriErisim.DegerYap(askonDetay.kontrolListesi) + ",";
            sql += VeriErisim.DegerYap(askonDetay.birimOnayi ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(askonDetay.birimAciklama) + ",";
            sql += VeriErisim.DegerYap(askonDetay.ustBirimOnayi ? 1 : 0) + ",";
            sql += VeriErisim.DegerYap(askonDetay.ustBirimAciklama) + ")";

            return KomutCalistir(sql, 1);
        }

        public bool DetayOnayliMi(AsamaKontrolDetay askonDetay)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMONAYI];
            sql += " + " + ALANLARDETAY[(int)ENUMAlanlarDetay.USTBIRIMONAYI] + ") FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", askonDetay.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", askonDetay.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KONUNO], "=", askonDetay.konuNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KONTROLLISTESI], "=", askonDetay.kontrolListesi));

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }

        public Sonuc SilDetay(AsamaKontrol askon)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", askon.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", askon.orgBirimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KONUNO], "=", askon.konuNo));

            return KomutCalistir(sql, 0);
        }

        public Sonuc KaydetGecmis(AsamaKontrolGecmis askongecmis)
        {
            string sql = "INSERT INTO " + TABLOADGECMIS + " (" + VeriErisim.TumAlanlar("", ALANLARGECMIS) + ") VALUES (";
            sql += VeriErisim.DegerYap(askongecmis.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(askongecmis.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(askongecmis.konuNo) + ",";
            sql += VeriErisim.DegerYap(GecmisSiraNoAl(askongecmis)) + ",";
            sql += VeriErisim.DegerYap(askongecmis.islem) + ",";
            sql += VeriErisim.DegerYap(askongecmis.islemiYapan) + ",";
            sql += VeriErisim.DegerYap(askongecmis.tarih) + ")";

            return KomutCalistir(sql, 1);
        }

        private int GecmisSiraNoAl(AsamaKontrolGecmis askongecmis)
        {
            string[] intAlan = { ALANLARGECMIS[(int)ENUMAlanlarGecmis.DONEMNO], ALANLARGECMIS[(int)ENUMAlanlarGecmis.ORGBIRIMKOD], ALANLARGECMIS[(int)ENUMAlanlarGecmis.KONUNO] };
            int[] intDeger = { askongecmis.donem.donemNo, askongecmis.orgBirimKod, askongecmis.konuNo };
            return TABGenel.SayacDegerAl(veriErisim, TABLOADGECMIS, ALANLARGECMIS[(int)ENUMAlanlarGecmis.SIRANO], new string[0], new string[0], intAlan, intDeger);
        }

        public IDataReader Listele(AsamaKontrol askon, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (askon.donem != null && askon.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", askon.donem.donemNo));
            if (!sorgu && askon.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", askon.orgBirimKod));
            if (askon.konuNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KONUNO], "=", askon.konuNo));
            if ((int)askon.statu > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.STATU], "=", (int)askon.statu));
            if (askon.baslamaTarihi != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLAMATARIHI], ">=", askon.baslamaTarihi));
            if (askon.tamamlanmaTarihi != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TAMAMLANMATARIHI], "<=", askon.tamamlanmaTarihi));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], askon.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDetay(AsamaKontrolDetay askonDetay, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;

            string kosul = string.Empty;
            if (askonDetay.donem != null && askonDetay.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", askonDetay.donem.donemNo);
            if (!sorgu && askonDetay.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", askonDetay.orgBirimKod));
            if (askonDetay.konuNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KONUNO], "=", askonDetay.konuNo));
            if (!string.IsNullOrEmpty(askonDetay.kontrolListesi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KONTROLLISTESI], "=", askonDetay.kontrolListesi));
            if (askonDetay.birimOnayiSorguKriter > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMONAYI], "=", askonDetay.birimOnayiSorguKriter));
            if (!string.IsNullOrEmpty(askonDetay.birimAciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BIRIMACIKLAMA], "=", askonDetay.birimAciklama));
            if (askonDetay.ustBirimOnayiSorguKriter > -1)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.USTBIRIMONAYI], "=", askonDetay.ustBirimOnayiSorguKriter));
            if (!string.IsNullOrEmpty(askonDetay.ustBirimAciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.USTBIRIMACIKLAMA], "=", askonDetay.ustBirimAciklama));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], askonDetay.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleGecmis(AsamaKontrolGecmis askongecmis)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADGECMIS, ALANLARGECMIS);
            sql += " FROM " + TABLOADGECMIS;

            string kosul = string.Empty;
            if (askongecmis.donem.donemNo > 0)
                kosul = VeriErisim.KosulOl(TABLOADGECMIS, ALANLARGECMIS[(int)ENUMAlanlarGecmis.DONEMNO], "=", askongecmis.donem.donemNo);
            if (askongecmis.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGECMIS, ALANLARGECMIS[(int)ENUMAlanlarGecmis.ORGBIRIMKOD], "=", askongecmis.orgBirimKod));
            if (askongecmis.konuNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGECMIS, ALANLARGECMIS[(int)ENUMAlanlarGecmis.KONUNO], "=", askongecmis.konuNo));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}