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
    public class TABGzft : TABOrtak
    {
        public TABGzft(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRGZFT";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            ANALIZYONTEMI = 2
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "ANALIZYONTEMI" };

        public static string TABLOADDETAY = "STRGZFTDETAY";

        public enum ENUMAlanlarDetay : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            TUR = 2,
            SIRANO = 3,
            ACIKLAMA = 4,
            ONCELIK = 5,
            SORUNALANKOD = 6
        }

        public static string[] ALANLARDETAY = { "DONEMNO", "ORGBIRIMKOD", "TUR", "SIRANO", "ACIKLAMA", "ONCELIK", "SORUNALANKOD" };

        public Sonuc KaydetDetay(int donemNo, int orgBirimKod, GzftDetay gd)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(donemNo) + ",";
            sql += VeriErisim.DegerYap(orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(gd.tur) + ",";
            sql += VeriErisim.DegerYap(gd.siraNo) + ",";
            sql += VeriErisim.DegerYap(gd.aciklama) + ",";
            sql += VeriErisim.DegerYap(gd.oncelikKod) + ",";
            sql += VeriErisim.DegerYap(gd.sorunAlanKod.Replace(".", "")) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDetay(int donemNo, int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleDetay(GzftDetay detay, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;

            string kosul = string.Empty;
            if (detay.donem != null && detay.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", detay.donem.donemNo));
            if (!sorgu && detay.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", detay.orgBirimKod));
            if (detay.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TUR], "=", detay.tur));
            if (!string.IsNullOrEmpty(detay.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ACIKLAMA], "LIKE", "%" + detay.aciklama + "%"));
            if (detay.oncelikKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ONCELIK], "=", detay.oncelikKod));
            if (!string.IsNullOrEmpty(detay.sorunAlanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SORUNALANKOD], "=", detay.sorunAlanKod.Replace(".", "")));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], detay.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Gzft gzft)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(gzft.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(gzft.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(gzft.analizYontemi) + ")";

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

        public IDataReader Listele(Gzft gzft, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (gzft.donem != null && gzft.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", gzft.donem.donemNo));
            if (!sorgu && gzft.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", gzft.orgBirimKod));
            if (!string.IsNullOrEmpty(gzft.analizYontemi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANALIZYONTEMI], "LIKE", "%" + gzft.analizYontemi + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], gzft.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}