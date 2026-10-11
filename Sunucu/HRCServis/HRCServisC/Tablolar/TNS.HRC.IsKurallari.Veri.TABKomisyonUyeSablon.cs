using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABKomisyonUyeSablon : TABOrtak
    {
        public TABKomisyonUyeSablon(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        public static string TABLOAD = "HRCKOMISYONUYESABLON";
        public enum ENUMAlanlar : int
        {
            KOD = 0,
            SAHIBI = 1,
            ADI = 2,
            DURUM = 3,
        }
        public static string[] ALANLAR = { "KOD", "SAHIBI", "ADI", "DURUM" };

        public static string TABLOADDETAY = "HRCKOMISYONUYESABLONDETAY";
        public enum ENUMAlanlarDetay : int
        {
            KOD = 0,
            KOMISYONUYESABLONKOD = 1,
            SIRANO = 2,
            KOMISYONGOREV = 3,
            AD = 4,
            GOREV = 5,
            UNVAN = 6,
            SICILNO = 7,
            TELEFON = 8,
            CEPTELEFON = 9
        }
        public static string[] ALANLARDETAY = { "KOD", "KOMISYONUYESABLONKOD", "SIRANO", "KOMISYONGOREV", "AD", "GOREV", "UNVAN", "SICILNO", "TELEFON", "CEPTELEFON" };

        public Sonuc Kaydet(KomisyonUyeSablon gt)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(gt.kod) + ",";
            sql += VeriErisim.DegerYap(gt.sahibi) + ",";
            sql += VeriErisim.DegerYap(gt.adi) + ",";
            sql += VeriErisim.DegerYap(gt.durum) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(KomisyonUyeSablon gt)
        {
            string sql = "UPDATE  " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADI], gt.adi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], gt.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gt.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KaydetDetay(KomisyonUyeSablonDetay gt)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(gt.kod) + ",";
            sql += VeriErisim.DegerYap(gt.sablonKod) + ",";
            sql += VeriErisim.DegerYap(gt.siraNo) + ",";
            sql += VeriErisim.DegerYap(gt.komisyonGorev) + ",";
            sql += VeriErisim.DegerYap(gt.adi) + ",";
            sql += VeriErisim.DegerYap(gt.gorev) + ",";
            sql += VeriErisim.DegerYap(gt.unvan) + ",";
            sql += VeriErisim.DegerYap(gt.sicilNo) + ",";
            sql += VeriErisim.DegerYap(gt.telefon) + ",";
            sql += VeriErisim.DegerYap(gt.cepTelefon) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(KomisyonUyeSablon gt)
        {
            string sql = " SELECT " + VeriErisim.TumAlanlar("", ALANLAR);
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SAHIBI], "=", gt.sahibi));

            if (gt.kod != string.Empty)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", gt.kod));

            sql += " ORDER BY ADI";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader ListeleDetay(string sablonKod)
        {
            string sql = " SELECT " + VeriErisim.TumAlanlar("", ALANLARDETAY);
            sql += " FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.KOMISYONUYESABLONKOD], "=", sablonKod));
            sql += " ORDER BY SIRANO";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(KomisyonUyeSablon gt)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", gt.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc SilDetay(KomisyonUyeSablon gt)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.KOMISYONUYESABLONKOD], "=", gt.kod));

            return KomutCalistir(sql, 0);
        }
    }
}
