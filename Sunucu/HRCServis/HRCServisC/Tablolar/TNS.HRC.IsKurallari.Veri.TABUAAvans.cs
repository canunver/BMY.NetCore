using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.HRC.IsKurallari.Veri
{
    public class TABUAAvans : TABOrtak
    {
        public TABUAAvans(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "HRCUAAVANS";

        public static string[] ALANLAR = { "SURECYIL", "SURECNO", "SAYI", "GEREKCE", "MIKTAR", "BANKA", "IBANNO", "TCKIMLIKNO", "KOORDINATOR", "ONAYLAYAN", "BIRIMKOD", "SAYMANLIKKOD", "OLURTARIHI", "KOORDINATORUNVAN", "ONAYLAYANUNVAN", "ODEMEEMRINO", "DURUM" };

        public static string TABLOADKAPATMA = "HRCUAAVANSKAPATMA";

        public static string[] ALANLARKAPATMA = { "SURECYIL", "SURECNO", "ODEMEEMRINO", "KAPATMAODEMEEMRINO" };

        public static string TABLOADKAPATMATERTIP = "HRCUAAVANSTERTIP";

        public static string[] ALANLARKAPATMATERTIP = { "SURECYIL", "SURECNO", "TERTIP", "MIKTAR" };

        public enum ENUMKapatmaAlanlarTertip
        {
            SURECYIL = 0,
            SURECNO = 1,
            TERTIP = 2,
            MIKTAR = 3,

        }

        public enum ENUMKapatmaAlanlar
        {
            SURECYIL = 0,
            SURECNO = 1,
            ODEMEEMRINO = 2,
            KAPATMAODEMEEMRINO = 3,
        }

        public Sonuc KapatmaTertipEkle(UAAvansKapatma form, UAAvansKapatmaTertip bilgi)
        {
            string sql = "INSERT INTO " + TABLOADKAPATMATERTIP + "(" + VeriErisim.TumAlanlar("", ALANLARKAPATMATERTIP) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.surecYil) + "," + VeriErisim.DegerYap(form.surecNo) + "," + VeriErisim.DegerYap(bilgi.tertip) + "," + VeriErisim.DegerYap(bilgi.miktar);
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc KapatmaTertipSil(UAAvansKapatma bilgi)
        {
            string sql = "DELETE FROM " + TABLOADKAPATMATERTIP;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", bilgi.surecNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KapatmaEkle(UAAvansKapatma bilgi)
        {
            string sql = "INSERT INTO " + TABLOADKAPATMA + "(" + VeriErisim.TumAlanlar("", ALANLARKAPATMA) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.surecYil) + "," + VeriErisim.DegerYap(bilgi.surecNo) + "," + VeriErisim.DegerYap(bilgi.odemeEmriNo) + "," + VeriErisim.DegerYap(bilgi.kapatmaOdemeEmriNo);
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader KapatmaOku(UAAvansKapatma bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKAPATMA) + " FROM " + TABLOADKAPATMA;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", bilgi.surecNo));

            return this.veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KapatmaTertipOku(UAAvansKapatma bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKAPATMATERTIP) + " FROM " + TABLOADKAPATMATERTIP;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", bilgi.surecNo));

            return this.veriErisim.DataReaderDondur(sql);
        }

        public enum ENUMAlanlar
        {
            SURECYIL = 0,
            SURECNO = 1,
            SAYI = 2,
            GEREKCE = 3,
            MIKTAR = 4,
            BANKA = 5,
            IBANNO = 6,
            TCKIMLIKNO = 7,
            KOORDINATOR = 8,
            ONAYLAYAN = 9,
            BIRIMKOD = 10,
            SAYMANLIKKOD = 11,
            OLURTARIHI = 12,
            KOORDINATORUNVAN = 13,
            ONAYLAYANUNVAN = 14,
            ODEMEEMRINO = 15,
            DURUM = 16,
        }

        public Sonuc Ekle(UAAvansTalep bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.surecYil) + "," + VeriErisim.DegerYap(bilgi.surecNo) + "," + VeriErisim.DegerYap(bilgi.sayi) + "," + VeriErisim.DegerYap(bilgi.gerekce) + "," + VeriErisim.DegerYap(bilgi.miktar);
            sql += "," + VeriErisim.DegerYap(bilgi.banka) + "," + VeriErisim.DegerYap(bilgi.ibanNo) + "," + VeriErisim.DegerYap(bilgi.tcKimlikNo);
            sql += "," + VeriErisim.DegerYap(bilgi.koordinator) + "," + VeriErisim.DegerYap(bilgi.onaylayan) + "," + VeriErisim.DegerYap(bilgi.birimKod);
            sql += "," + VeriErisim.DegerYap(bilgi.saymanlikKod) + "," + VeriErisim.DegerYap(bilgi.olurTarihi) + "," + VeriErisim.DegerYap(bilgi.koordinatorUnvan);
            sql += "," + VeriErisim.DegerYap(bilgi.onaylayanUnvan) + "," + VeriErisim.DegerYap(bilgi.odemeEmriNo) + "," + VeriErisim.DegerYap(bilgi.durum);
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(UAAvansTalep bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BANKA], bilgi.banka) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], bilgi.birimKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GEREKCE], bilgi.gerekce) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IBANNO], bilgi.ibanNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOORDINATOR], bilgi.koordinator) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MIKTAR], bilgi.miktar) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAYAN], bilgi.onaylayan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYI], bilgi.sayi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SAYMANLIKKOD], bilgi.saymanlikKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TCKIMLIKNO], bilgi.tcKimlikNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.OLURTARIHI], bilgi.olurTarihi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOORDINATORUNVAN], bilgi.koordinatorUnvan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAYANUNVAN], bilgi.onaylayanUnvan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], bilgi.odemeEmriNo) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", bilgi.surecNo));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Oku(UAAvansTalep bilgi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", bilgi.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", bilgi.surecNo));

            return this.veriErisim.DataReaderDondur(sql);
        }

        public Sonuc OEIliskiKur(UAAvansTalep okunan)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEEMRINO], okunan.odemeEmriNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", okunan.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", okunan.surecNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KapatmaOEIliskiKur(UAAvansKapatma okunan)
        {
            string sql = "UPDATE " + TABLOADKAPATMA + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARKAPATMA[(int)ENUMKapatmaAlanlar.ODEMEEMRINO], okunan.odemeEmriNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", okunan.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", okunan.surecNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc KapatmaKalanOEIliskiKur(UAAvansKapatma okunan)
        {
            string sql = "UPDATE " + TABLOADKAPATMA + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARKAPATMA[(int)ENUMKapatmaAlanlar.KAPATMAODEMEEMRINO], okunan.kapatmaOdemeEmriNo);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "SURECYIL", "=", okunan.surecYil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "SURECNO", "=", okunan.surecNo));

            return KomutCalistir(sql, 1);
        }
    }
}
