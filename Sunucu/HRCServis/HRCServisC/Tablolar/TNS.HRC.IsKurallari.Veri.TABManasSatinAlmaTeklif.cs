
using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABManasSatinAlmaTeklif : TABOrtak
    {
        public TABManasSatinAlmaTeklif(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MANASSATINALMATEKLIF";
        public enum ENUMAlanlar
        {
            SATINALMAKOD = 0,
            REFID = 1,
            URUNREFID = 2,
            FIRMAREFID = 3,
            TUTAR = 4,
            TUR = 5,
        }
        public static string[] ALANLAR = { "SATINALMAKOD", "REFID", "URUNREFID", "FIRMAREFID", "TUTAR", "TUR" };

        public static string TABLOADFORM = "MANASSATINALMATEKLIFFORM";
        public enum ENUMAlanlarForm
        {
            SATINALMAKOD = 0,
            TUR = 1,
            TARIH = 2,
            YONTEM = 3,
            TUTAR = 4
        }
        public static string[] ALANLARFORM = { "SATINALMAKOD", "TUR", "TARIH", "YONTEM", "TUTAR" };

        public Sonuc Kaydet(ManasSatinAlmaTeklif form)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.satinAlmaKod) + ",";
            sql += VeriErisim.DegerYap(form.refId) + ",";
            sql += VeriErisim.DegerYap(form.urunRefId) + ",";
            sql += VeriErisim.DegerYap(form.firmaRefId) + ",";
            sql += VeriErisim.DegerYap(form.tutar) + ",";
            sql += VeriErisim.DegerYap(form.tur) + ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetForm(ManasSatinAlmaTeklifForm form)
        {
            string sql = "INSERT INTO " + TABLOADFORM + " (" + VeriErisim.TumAlanlar("", ALANLARFORM) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.satinAlmaKod) + ",";
            sql += VeriErisim.DegerYap(form.tur) + ",";
            sql += VeriErisim.DegerYap(form.tarih) + ",";
            sql += VeriErisim.DegerYap(form.yontem) + ",";
            sql += VeriErisim.DegerYap(form.tutar) + ")";
            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(ManasSatinAlmaTeklif form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", MANASSATINALMAFIRMA.UNVAN, MANASSATINALMAURUNLER.HESAPPLANKOD, MANASSATINALMAURUNLER.HESAPPLANAD";
            sql += " FROM " + TABLOAD;
            sql += " INNER JOIN MANASSATINALMAFIRMA ON MANASSATINALMATEKLIF.SATINALMAKOD = MANASSATINALMAFIRMA.SATINALMAKOD AND MANASSATINALMATEKLIF.FIRMAREFID = MANASSATINALMAFIRMA.REFID ";
            sql += " INNER JOIN MANASSATINALMAURUNLER ON MANASSATINALMATEKLIF.SATINALMAKOD = MANASSATINALMAURUNLER.SATINALMAKOD AND MANASSATINALMATEKLIF.URUNREFID = MANASSATINALMAURUNLER.TALEPREFID";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", form.satinAlmaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.TUR], "=", form.tur));

            sql += " ORDER BY MANASSATINALMAURUNLER.SIRANO, MANASSATINALMAFIRMA.SIRANO";
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleForm(ManasSatinAlmaTeklifForm form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADFORM, ALANLARFORM);
            sql += " FROM " + TABLOADFORM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADFORM, ALANLARFORM[(int)ENUMAlanlarForm.SATINALMAKOD], "=", form.satinAlmaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADFORM, ALANLARFORM[(int)ENUMAlanlarForm.TUR], "=", form.tur));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(int satinAlmaKod, int tur)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SATINALMAKOD], "=", satinAlmaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", tur));

            return KomutCalistir(sql, 0);
        }
        public Sonuc SilForm(int satinAlmaKod, int tur)
        {
            string sql = "DELETE FROM " + TABLOADFORM;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.SATINALMAKOD], "=", satinAlmaKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARFORM[(int)ENUMAlanlarForm.TUR], "=", tur));

            return KomutCalistir(sql, 0);
        }
    }
}
