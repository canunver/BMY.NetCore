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
    public class TABPest : TABOrtak
    {
        public TABPest(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRPEST";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            ANALIZYONTEMI = 2,
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "ANALIZYONTEMI" };

        public static string TABLOADDETAY = "STRPESTDETAY";

        public enum ENUMAlanlarDetay : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            KONUNO = 2,
            SIRANO = 3,
            TEMELEGILIMLER = 4,
            ACIKLAMA = 5,
            ILISKI = 6,
        }

        public static string[] ALANLARDETAY = { "DONEMNO", "ORGBIRIMKOD", "KONUNO", "SIRANO", "TEMELEGILIMLER", "ACIKLAMA", "ILISKI" };

        public Sonuc KaydetDetay(int donemNo, int orgBirimKod, PestDetay pd)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(donemNo) + ",";
            sql += VeriErisim.DegerYap(orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(pd.konuNo) + ",";
            sql += VeriErisim.DegerYap(pd.siraNo) + ",";
            sql += VeriErisim.DegerYap(pd.temelEgilimler) + ",";
            sql += VeriErisim.DegerYap(pd.aciklama) + ",";
            sql += VeriErisim.DegerYap(pd.iliski) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDetay(int donemNo, int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleDetay(PestDetay detay, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;

            string kosul = string.Empty;
            if (detay.donem != null && detay.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", detay.donem.donemNo));
            if (!sorgu && detay.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", detay.orgBirimKod));
            if (detay.konuNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KONUNO], "=", detay.konuNo));
            if (!string.IsNullOrEmpty(detay.temelEgilimler))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.TEMELEGILIMLER], "LIKE", "%" + detay.temelEgilimler + "%"));
            if (!string.IsNullOrEmpty(detay.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ACIKLAMA], "LIKE", "%" + detay.aciklama + "%"));
            if (!string.IsNullOrEmpty(detay.iliski))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ILISKI], "LIKE", "%" + detay.iliski + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], detay.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(Pest pest)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(pest.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(pest.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(pest.analizYontemi) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int donemNo, int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(Pest pest, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (pest.donem != null && pest.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", pest.donem.donemNo));
            if (!sorgu && pest.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", pest.orgBirimKod));
            if (!string.IsNullOrEmpty(pest.analizYontemi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANALIZYONTEMI], "LIKE", "%" + pest.analizYontemi + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], pest.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}