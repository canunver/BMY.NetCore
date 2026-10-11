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
    public class TABPaydasAnaliz : TABOrtak
    {
        public TABPaydasAnaliz(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "STRPAYDASANALIZ";

        public enum ENUMAlanlar : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            ANALIZYONTEMI = 2,
        }

        public static string[] ALANLAR = { "DONEMNO", "ORGBIRIMKOD", "ANALIZYONTEMI" };

        public static string TABLOADDETAY = "STRPAYDASANALIZDETAY";

        public enum ENUMAlanlarDetay : int
        {
            DONEMNO = 0,
            ORGBIRIMKOD = 1,
            SIRANO = 2,
            SINIF = 3,
            AD = 4,
            ACIKLAMA = 5,
            KATEGORI = 6,
            ONCELIK = 7,
            ETKI = 8,
            STRATEJI = 9,
        }

        public static string[] ALANLARDETAY = { "DONEMNO", "ORGBIRIMKOD", "SIRANO", "SINIF", "AD", "ACIKLAMA", "KATEGORI", "ONCELIK", "ETKI", "STRATEJI" };

        public Sonuc KaydetDetay(int donemNo, int orgBirimKod, PaydasAnalizDetay[] liste)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            foreach (PaydasAnalizDetay detay in liste)
            {
                string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(donemNo) + ",";
                sql += VeriErisim.DegerYap(orgBirimKod) + ",";
                sql += VeriErisim.DegerYap(detay.siraNo) + ",";
                sql += VeriErisim.DegerYap(detay.sinif) + ",";
                sql += VeriErisim.DegerYap(detay.ad) + ",";
                sql += VeriErisim.DegerYap(detay.aciklama) + ",";
                sql += VeriErisim.DegerYap(detay.kategoriKod) + ",";
                sql += VeriErisim.DegerYap(detay.oncelikKod) + ",";
                sql += VeriErisim.DegerYap(detay.etkiKod) + ",";
                sql += VeriErisim.DegerYap(detay.stratejiKod) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        public Sonuc SilDetay(PaydasAnalizDetay detay)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", detay.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", detay.orgBirimKod));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilDetay(int donemNo, int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleDetay(PaydasAnalizDetay detay, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;

            string kosul = string.Empty;
            if (detay.donem != null && detay.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.DONEMNO], "=", detay.donem.donemNo));
            if (!sorgu && detay.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], "=", detay.orgBirimKod));
            if (detay.sinif > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.SINIF], "=", detay.sinif));
            if (!string.IsNullOrEmpty(detay.ad))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.AD], "LIKE", "%" + detay.ad + "%"));
            if (!string.IsNullOrEmpty(detay.aciklama))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ACIKLAMA], "LIKE", "%" + detay.aciklama + "%"));
            if (!string.IsNullOrEmpty(detay.kategoriKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KATEGORI], "=", detay.kategoriKod));
            if (detay.oncelikKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ONCELIK], "=", detay.oncelikKod));
            if (detay.etkiKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ETKI], "=", detay.etkiKod));
            if (detay.stratejiKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.STRATEJI], "=", detay.stratejiKod));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.ORGBIRIMKOD], detay.orgBirimKod));
            
            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Kaydet(PaydasAnaliz paydasAnaliz)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(paydasAnaliz.donem.donemNo) + ",";
            sql += VeriErisim.DegerYap(paydasAnaliz.orgBirimKod) + ",";
            sql += VeriErisim.DegerYap(paydasAnaliz.analizYontemi) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public Sonuc Sil(PaydasAnaliz paydasAnaliz)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", paydasAnaliz.donem.donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", paydasAnaliz.orgBirimKod));
            return KomutCalistir(sql, 1);
        }

        public Sonuc Sil(int donemNo, int orgBirimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", donemNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", orgBirimKod));
            return KomutCalistir(sql, 0);
        }

        public IDataReader Listele(PaydasAnaliz paydasAnaliz, bool sorgu)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (paydasAnaliz.donem != null && paydasAnaliz.donem.donemNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEMNO], "=", paydasAnaliz.donem.donemNo));
            if (!sorgu && paydasAnaliz.orgBirimKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], "=", paydasAnaliz.orgBirimKod));
            if (!string.IsNullOrEmpty(paydasAnaliz.analizYontemi))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANALIZYONTEMI], "LIKE", "%" + paydasAnaliz.analizYontemi + "%"));

            if (sorgu)
                kosul = VeriErisim.KosulEkle(kosul, "AND", TABOrgBirim.BagliBirimKosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ORGBIRIMKOD], paydasAnaliz.orgBirimKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return veriErisim.DataReaderDondur(sql);
        }
    }
}