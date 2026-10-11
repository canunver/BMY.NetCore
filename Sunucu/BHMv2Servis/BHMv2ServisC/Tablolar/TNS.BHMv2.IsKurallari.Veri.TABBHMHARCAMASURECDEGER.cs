using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    public class TABBHMHARCAMASURECDEGER : TABOrtak
    {
        public TABBHMHARCAMASURECDEGER(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        public static string TABLOAD = "BHMODENEKSUREC";

        public enum ENUMAlanlar : int
        {
            YIL = 0,
            ISLEMYIL = 1,
            ISLEMAY = 2,
            SURECYIL = 3,
            SURECNO = 4,
            KURKOD = 5,
            FONKOD = 6,
            FINKOD = 7,
            EKOKOD = 8,
            PEBKOD = 9,
            MUHASEBEKOD = 10,
            BIRIMKOD = 11,
            ONAY = 12,
            ALIM = 13,
            FATURA = 14,
            TESLIM = 15,
            ODEME = 16,
        }

        static string[] ALANLAR = {
            "YIL",
            "ISLEMYIL",
            "ISLEMAY",
            "SURECYIL",
            "SURECNO",
            "KURKOD",
            "FONKOD",
            "FINKOD",
            "EKOKOD",
            "PEBKOD",
            "MUHASEBEKOD",
            "BIRIMKOD",
            "ONAY",
            "ALIM",
            "FATURA",
            "TESLIM",
            "ODEME"
        };

        public Sonuc KaydetSurecOnay(ObjectArray liste)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            foreach (TNS.HRC.BHMHarcama h in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAY], h.toplamTutar);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYIL], "=", h.islemYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMAY], "=", h.islemAy));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", h.surecYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", h.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", h.kur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", h.fon));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", h.fin));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", h.eko));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", h.peb));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", h.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", h.birim));

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(h.yil) + ",";
                    sql += VeriErisim.DegerYap(h.islemYil) + ",";
                    sql += VeriErisim.DegerYap(h.islemAy) + ",";
                    sql += VeriErisim.DegerYap(h.surecYil) + ",";
                    sql += VeriErisim.DegerYap(h.surecNo) + ",";
                    sql += VeriErisim.DegerYap(h.kur) + ",";
                    sql += VeriErisim.DegerYap(h.fon) + ",";
                    sql += VeriErisim.DegerYap(h.fin) + ",";
                    sql += VeriErisim.DegerYap(h.eko) + ",";
                    sql += VeriErisim.DegerYap(h.peb) + ",";
                    sql += VeriErisim.DegerYap(h.muhasebe) + ",";
                    sql += VeriErisim.DegerYap(h.birim) + ",";
                    sql += VeriErisim.DegerYap(h.toplamTutar) + ","; //onay
                    sql += VeriErisim.DegerYap(0) + ","; //alım
                    sql += VeriErisim.DegerYap(0) + ","; //fatura
                    sql += VeriErisim.DegerYap(0) + ","; //teslim
                    sql += VeriErisim.DegerYap(0) + ")"; //odeme
                    sonuc = KomutCalistir(sql, 1);
                }
            }

            return sonuc;
        }
        public Sonuc KaydetSurecAlim(ObjectArray liste)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            foreach (TNS.HRC.BHMHarcama h in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ALIM], h.toplamTutar);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYIL], "=", h.islemYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMAY], "=", h.islemAy));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", h.surecYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", h.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", h.kur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", h.fon));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", h.fin));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", h.eko));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", h.peb));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", h.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", h.birim));

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(h.yil) + ",";
                    sql += VeriErisim.DegerYap(h.islemYil) + ",";
                    sql += VeriErisim.DegerYap(h.islemAy) + ",";
                    sql += VeriErisim.DegerYap(h.surecYil) + ",";
                    sql += VeriErisim.DegerYap(h.surecNo) + ",";
                    sql += VeriErisim.DegerYap(h.kur) + ",";
                    sql += VeriErisim.DegerYap(h.fon) + ",";
                    sql += VeriErisim.DegerYap(h.fin) + ",";
                    sql += VeriErisim.DegerYap(h.eko) + ",";
                    sql += VeriErisim.DegerYap(h.peb) + ",";
                    sql += VeriErisim.DegerYap(h.muhasebe) + ",";
                    sql += VeriErisim.DegerYap(h.birim) + ",";
                    sql += VeriErisim.DegerYap(0) + ","; //onay
                    sql += VeriErisim.DegerYap(h.toplamTutar) + ","; //alım
                    sql += VeriErisim.DegerYap(0) + ","; //fatura
                    sql += VeriErisim.DegerYap(0) + ","; //teslim
                    sql += VeriErisim.DegerYap(0) + ")"; //odeme
                    sonuc = KomutCalistir(sql, 1);
                }
            }

            return sonuc;
        }
        public Sonuc KaydetSurecFatura(ObjectArray liste)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            foreach (TNS.HRC.BHMHarcama h in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURA], h.toplamTutar);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYIL], "=", h.islemYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMAY], "=", h.islemAy));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", h.surecYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", h.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", h.kur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", h.fon));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", h.fin));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", h.eko));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", h.peb));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", h.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", h.birim));

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(h.yil) + ",";
                    sql += VeriErisim.DegerYap(h.islemYil) + ",";
                    sql += VeriErisim.DegerYap(h.islemAy) + ",";
                    sql += VeriErisim.DegerYap(h.surecYil) + ",";
                    sql += VeriErisim.DegerYap(h.surecNo) + ",";
                    sql += VeriErisim.DegerYap(h.kur) + ",";
                    sql += VeriErisim.DegerYap(h.fon) + ",";
                    sql += VeriErisim.DegerYap(h.fin) + ",";
                    sql += VeriErisim.DegerYap(h.eko) + ",";
                    sql += VeriErisim.DegerYap(h.peb) + ",";
                    sql += VeriErisim.DegerYap(h.muhasebe) + ",";
                    sql += VeriErisim.DegerYap(h.birim) + ",";
                    sql += VeriErisim.DegerYap(0) + ","; //onay
                    sql += VeriErisim.DegerYap(0) + ","; //alım
                    sql += VeriErisim.DegerYap(h.toplamTutar) + ","; //fatura
                    sql += VeriErisim.DegerYap(0) + ","; //teslim
                    sql += VeriErisim.DegerYap(0) + ")"; //odeme
                    sonuc = KomutCalistir(sql, 1);
                }
            }

            return sonuc;
        }
        public Sonuc KaydetSurecTeslim(ObjectArray liste)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            foreach (TNS.HRC.BHMHarcama h in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TESLIM], h.toplamTutar);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYIL], "=", h.islemYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMAY], "=", h.islemAy));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", h.surecYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", h.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", h.kur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", h.fon));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", h.fin));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", h.eko));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", h.peb));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", h.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", h.birim));

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(h.yil) + ",";
                    sql += VeriErisim.DegerYap(h.islemYil) + ",";
                    sql += VeriErisim.DegerYap(h.islemAy) + ",";
                    sql += VeriErisim.DegerYap(h.surecYil) + ",";
                    sql += VeriErisim.DegerYap(h.surecNo) + ",";
                    sql += VeriErisim.DegerYap(h.kur) + ",";
                    sql += VeriErisim.DegerYap(h.fon) + ",";
                    sql += VeriErisim.DegerYap(h.fin) + ",";
                    sql += VeriErisim.DegerYap(h.eko) + ",";
                    sql += VeriErisim.DegerYap(h.peb) + ",";
                    sql += VeriErisim.DegerYap(h.muhasebe) + ",";
                    sql += VeriErisim.DegerYap(h.birim) + ",";
                    sql += VeriErisim.DegerYap(0) + ","; //onay
                    sql += VeriErisim.DegerYap(0) + ","; //alım
                    sql += VeriErisim.DegerYap(0) + ","; //fatura
                    sql += VeriErisim.DegerYap(h.toplamTutar) + ","; //teslim
                    sql += VeriErisim.DegerYap(0) + ")"; //odeme
                    sonuc = KomutCalistir(sql, 1);
                }
            }

            return sonuc;
        }
        public Sonuc KaydetSurecOdeme(ObjectArray liste)
        {
            string sql = "";
            Sonuc sonuc = new Sonuc();
            foreach (TNS.HRC.BHMHarcama h in liste.objeler)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEME], h.toplamTutar);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", h.yil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYIL], "=", h.islemYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMAY], "=", h.islemAy));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", h.surecYil));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.SURECNO], "=", h.surecNo));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", h.kur));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", h.fon));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", h.fin));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", h.eko));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PEBKOD], "=", h.peb));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", h.muhasebe));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", h.birim));

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                    sql += VeriErisim.DegerYap(h.yil) + ",";
                    sql += VeriErisim.DegerYap(h.islemYil) + ",";
                    sql += VeriErisim.DegerYap(h.islemAy) + ",";
                    sql += VeriErisim.DegerYap(h.surecYil) + ",";
                    sql += VeriErisim.DegerYap(h.surecNo) + ",";
                    sql += VeriErisim.DegerYap(h.kur) + ",";
                    sql += VeriErisim.DegerYap(h.fon) + ",";
                    sql += VeriErisim.DegerYap(h.fin) + ",";
                    sql += VeriErisim.DegerYap(h.eko) + ",";
                    sql += VeriErisim.DegerYap(h.peb) + ",";
                    sql += VeriErisim.DegerYap(h.muhasebe) + ",";
                    sql += VeriErisim.DegerYap(h.birim) + ",";
                    sql += VeriErisim.DegerYap(0) + ","; //onay
                    sql += VeriErisim.DegerYap(0) + ","; //alım
                    sql += VeriErisim.DegerYap(0) + ","; //fatura
                    sql += VeriErisim.DegerYap(0) + ","; //teslim
                    sql += VeriErisim.DegerYap(h.toplamTutar) + ")"; //odeme
                    sonuc = KomutCalistir(sql, 1);
                }
            }

            return sonuc;
        }

        public Sonuc SilSurecBilgi(int yil, string kur, string fon, string fin, string eko)
        {
            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (!string.IsNullOrEmpty(kur))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kur));
            if (!string.IsNullOrEmpty(fon))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], "=", fon));
            if (!string.IsNullOrEmpty(fin))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FINKOD], "=", fin));
            if (!string.IsNullOrEmpty(eko))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "=", eko));
            return KomutCalistir(sql, 0);
        }


        public IDataReader ListeleSurecBilgi(TNS.HRC.BHMHarcamaSorgu kriter)
        {
            string sql = string.Empty;

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            string kosul = string.Empty;
            if (!string.IsNullOrEmpty(kriter.eko))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", kriter.eko.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kriter.fin))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FINKOD], "LIKE", kriter.fin.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kriter.fon))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FONKOD], "LIKE", kriter.fon.Replace(".", "") + "%"));
            if (kriter.islemAy > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMAY], "=", kriter.islemAy));
            if (kriter.islemYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYIL], "=", kriter.islemYil));
            if (!string.IsNullOrEmpty(kriter.kur))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kriter.kur.Replace(".", "") + "%"));
            if (kriter.muhasebe > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", kriter.muhasebe));
            if (!string.IsNullOrEmpty(kriter.peb))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PEBKOD], "LIKE", kriter.peb.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(kriter.surecNo ))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECNO], "=", kriter.surecNo));
            if (kriter.surecYil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SURECYIL], "=", kriter.surecYil));
            if (kriter.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", kriter.yil));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ", " +
                                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + ", " +
                                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BIRIMKOD] + ", " +
                                    TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SURECNO];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
