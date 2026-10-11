using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABForm : TABOrtak
    {
        public TABForm(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRFORM";
        public enum ENUMAlanlar : int { SAYFAKOD, SAYFAAD, DURUM, SABLONKOD, SURECNO, EVRAKBIRIMKODU, GIDENEVRAKOLARAK }
        public static string[] ALANLAR = { "SAYFAKOD", "SAYFAAD", "DURUM", "SABLONKOD", "SURECNO", "EVRAKBIRIMKODU", "GIDENEVRAKOLARAK" };

        public static string TABLOADKONTROL = "EVRFORMKONTROL";
        public enum ENUMAlanlarKontrol : int { KONTROLKOD, SAYFAKOD, KONTROLTUR }
        public static string[] ALANLARKONTROL = { "KONTROLKOD", "SAYFAKOD", "KONTROLTUR" };

        public static string TABLOADOZELLIK = "EVRFORMOZELLIK";
        public enum ENUMAlanlarOzellik : int { KONTROLKOD, OZELLIKTUR, SAYFAKOD, DEGER }
        public static string[] ALANLAROZELLIK = { "KONTROLKOD", "OZELLIKTUR", "SAYFAKOD", "DEGER" };

        public Sonuc KaydetForm(Form form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.sayfaKod) + ",";
            sql += VeriErisim.DegerYap(form.sayfaAd) + ",";
            sql += VeriErisim.DegerYap(form.durum) + ",";
            sql += VeriErisim.DegerYap(form.sablonKod) + ",";
            sql += VeriErisim.DegerYap(form.surecNo) + ",";
            sql += VeriErisim.DegerYap(form.evrBirimKod) + ",";
            sql += VeriErisim.DegerYap(form.gidenEvrakOlarakKayitEdilebilir) + ")";

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
        public Sonuc KaydetKontrol(FormKontrol form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOADKONTROL + " (" + VeriErisim.TumAlanlar("", ALANLARKONTROL) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kontrolKod) + ",";
            sql += VeriErisim.DegerYap(form.sayfaKod) + ",";
            sql += VeriErisim.DegerYap(form.kontrolTur) + ")";

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }
        public Sonuc KaydetOzellik(FormOzellik form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "INSERT INTO " + TABLOADOZELLIK + " (" + VeriErisim.TumAlanlar("", ALANLAROZELLIK) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.kontrolKod) + ",";
            sql += VeriErisim.DegerYap(form.ozellikTur) + ",";
            sql += VeriErisim.DegerYap(form.sayfaKod) + ",";
            sql += VeriErisim.DegerYap(form.deger) + ")";

            sonuc = KomutCalistir(sql, 1);
            return sonuc;
        }

        public IDataReader ListeleForm(Form form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            string kosul = "";

            if (form.sayfaKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYFAKOD], "=", form.sayfaKod));
            if (form.evrBirimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", form.evrBirimKod));
            if (form.durum > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DURUM], "=", form.durum));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleKontrol(FormKontrol form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKONTROL) + " FROM " + TABLOADKONTROL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONTROL[(int)ENUMAlanlarKontrol.SAYFAKOD], "=", form.sayfaKod));

            if (!string.IsNullOrEmpty(form.kontrolKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKONTROL[(int)ENUMAlanlarKontrol.KONTROLKOD], "=", form.kontrolKod));

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleOzellik(FormOzellik form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAROZELLIK) + " FROM " + TABLOADOZELLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlarOzellik.KONTROLKOD], "=", form.kontrolKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilForm(string sayfaKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SAYFAKOD], "=", sayfaKod));
            return KomutCalistir(sql, 0);
        }
        public Sonuc SilKontrol(string sayfaKod, string kontrolKod)
        {
            string sql = "DELETE FROM " + TABLOADKONTROL;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKONTROL[(int)ENUMAlanlarKontrol.SAYFAKOD], "=", sayfaKod));
            if (kontrolKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKONTROL[(int)ENUMAlanlarKontrol.KONTROLKOD], "=", kontrolKod));
            return KomutCalistir(sql, 0);
        }
        public Sonuc SilOzellik(string sayfaKod, string kontrolKod)
        {
            string sql = "DELETE FROM " + TABLOADOZELLIK;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlarOzellik.SAYFAKOD], "=", sayfaKod));
            if (kontrolKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAROZELLIK[(int)ENUMAlanlarOzellik.KONTROLKOD], "=", kontrolKod));
            return KomutCalistir(sql, 0);
        }
    }
}

