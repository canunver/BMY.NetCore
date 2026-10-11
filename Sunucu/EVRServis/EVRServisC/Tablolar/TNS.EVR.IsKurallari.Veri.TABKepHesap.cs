using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABKepHesap : TABOrtak
    {
        public TABKepHesap(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRKEPHESAP";

        public enum ENUMAlanlar : int
        {
            KOD = 0, HESAPTUR = 1, HESAP = 2, HESAPSAHIBI = 3, VARSAYILAN = 4, ISLEMYAPAN = 5, ISLEMYAPANAD = 6, ANAHTAR = 7
        }

        public static string[] ALANLAR = { "KOD", "HESAPTUR", "HESAP", "HESAPSAHIBI", "VARSAYILAN", "ISLEMYAPAN", "ISLEMYAPANAD", "ANAHTAR" };

        public Sonuc Kaydet(KepHesap form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.hesapTur) + ",";
                sql += VeriErisim.DegerYap(form.hesap) + ",";
                sql += VeriErisim.DegerYap(form.hesapSahibi) + ",";
                sql += VeriErisim.DegerYap(form.varsayilan) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
                sql += VeriErisim.DegerYap(form.anahtar) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public Sonuc Guncelle(KepHesap form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPTUR], form.hesapTur) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAP], form.hesap) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.HESAPSAHIBI], form.hesapSahibi) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAHTAR], form.anahtar);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(KepHesap form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                string ekKosul = "";

                if (form.hesapSahibi != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPSAHIBI], "=", form.hesapSahibi));
                if (form.hesap != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAP], "=", form.hesap));
                if (form.islemYapan != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));

                if (ekKosul != "")
                    sql += " WHERE " + ekKosul;
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(KepHesap form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public Sonuc VarsayilanYap(KepHesap form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VARSAYILAN], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));

            Sonuc sonuc = KomutCalistir(sql, 0);
            if (sonuc.islemSonuc)
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VARSAYILAN], 1);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

    }
}
