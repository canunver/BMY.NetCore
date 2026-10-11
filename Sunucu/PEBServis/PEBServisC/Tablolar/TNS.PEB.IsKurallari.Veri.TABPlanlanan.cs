using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik amaç bilgilerini tutan STRGENELHAVUZ veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABPlanlanan : TABOrtak
    {
        public TABPlanlanan(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRPLANLANAN";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD" };

        public static string TABLOADDETAY = "STRPLANLANANDETAY";

        public enum ENUMAlanlarDetay : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            SIRANO = 2,
            CALISMAADIMI = 3,
            BASLANGICTARIH = 4,
            BITISTARIH = 5,
            GERBASLANGICTARIH = 6,
            GERBITISTARIH = 7
        }

        public static string[] ALANLARDETAY = { "DONEMNO", "ORGBIRIMKOD", "SIRANO", "CALISMAADIMI", "BASLANGICTARIH", "BITISTARIH", "GERBASLANGICTARIH", "GERBITISTARIH" };

        public Sonuc KaydetDetay(int donemNo, int orgBirimKod, PlanlananDetay gd)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(donemNo) + ",";
            sql += VeriErisim.DegerYap(orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(gd.siraNo) + ",";
            sql += VeriErisim.DegerYap(gd.calismaAdimi) + ",";
            sql += VeriErisim.DegerYap(gd.baslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(gd.bitisTarihi) + ",";
            sql += VeriErisim.DegerYap(gd.gerBaslangicTarihi) + ",";
            sql += VeriErisim.DegerYap(gd.gerBitisTarihi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDetay(int donemNo, int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleDetay(PlanlananDetay planlananDetay, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;

            string kosul = string.Empty;
            if (planlananDetay.donem != null && planlananDetay.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", planlananDetay.donem.donemNo));
            if (!sorgu && planlananDetay.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", planlananDetay.orgBirimKod));
            if (!string.IsNullOrEmpty(planlananDetay.calismaAdimi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.CALISMAADIMI], "LIKE", "%" + planlananDetay.calismaAdimi + "%"));
            if (planlananDetay.baslangicTarihi != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BASLANGICTARIH], ">=", planlananDetay.baslangicTarihi));
            if (planlananDetay.bitisTarihi != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.BITISTARIH], "<=", planlananDetay.bitisTarihi));
            if (planlananDetay.gerBaslangicTarihi != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.GERBASLANGICTARIH], ">=", planlananDetay.gerBaslangicTarihi));
            if (planlananDetay.gerBitisTarihi != null)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.GERBITISTARIH], "<=", planlananDetay.gerBitisTarihi));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], planlananDetay.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Planlanan planlanan)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(planlanan.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(planlanan.orgBirimKod) + ",";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(int donemNo, int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(Planlanan planlanan)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (planlanan.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", planlanan.donem.donemNo));
            if (planlanan.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", planlanan.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}