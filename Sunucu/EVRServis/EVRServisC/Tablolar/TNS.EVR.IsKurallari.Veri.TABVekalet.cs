using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABVekalet : TABOrtak
    {
        public TABVekalet(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRVEKALET";

        public enum ENUMAlanlar : int
        {
            KOD = 0,
            KISI1 = 1,
            KISI2 = 2,
            TARIH1 = 3,
            TARIH2 = 4,
            ISLEMYAPAN = 5,
            ISLEMTARIH = 6,
            KISI1AD = 7,
            KISI2AD = 8,
            ISLEMYAPANAD = 9,
            AKTIF = 10,
            BIRIM1 = 11,
        }

        public static string[] ALANLAR = { "KOD", "KISI1", "KISI2", "TARIH1", "TARIH2", "ISLEMYAPAN", "ISLEMTARIH", "KISI1AD", "KISI2AD", "ISLEMYAPANAD", "AKTIF", "BIRIM1" };

        public Sonuc Kaydet(Vekalet form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.kisi1) + ",";
                sql += VeriErisim.DegerYap(form.kisi2) + ",";
                sql += VeriErisim.DegerYap(form.tarih1) + ",";
                sql += VeriErisim.DegerYap(form.tarih2) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.islemTarih) + ",";
                sql += VeriErisim.DegerYap(form.kisi1Ad) + ",";
                sql += VeriErisim.DegerYap(form.kisi2Ad) + ",";
                sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
                sql += VeriErisim.DegerYap(form.aktif) + ",";
                sql += VeriErisim.DegerYap(form.birim1) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(Vekalet form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI1], form.kisi1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI2], form.kisi2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], form.tarih1) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], form.tarih2) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], form.islemYapan) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIH], form.islemTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI1AD], form.kisi1Ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KISI2AD], form.kisi2Ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], form.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], form.aktif) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIM1], form.birim1);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }
        public IDataReader Listele(Vekalet form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], "=", 1));

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else if (form.kod == -100) // Kullanıcın aktif vekaletlerini listele
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI2], "=", form.kisi2));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], "<=", form.tarih1));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], ">=", form.tarih2));
            }
            else if (form.kod == -200) // Vekalet Veren Kullanıcının geçerli tarihlerde önceden aktif vekaleti var mı?
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.kisi1));

                if (form.birim1 != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIM1], "=", form.birim1));

                string ekKosul = "";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], "<=", form.tarih1));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], ">=", form.tarih1));
                sql += " AND ( (" + ekKosul + ") ";

                ekKosul = "";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], "<=", form.tarih2));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], ">=", form.tarih2));
                sql += " OR (" + ekKosul + ")";

                ekKosul = "";
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], ">=", form.tarih1));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], "<=", form.tarih2));
                sql += " OR (" + ekKosul + ") )";
            }
            else if (form.kod == -300) // Vekalet verenin aktif vekaletlerini listele
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.kisi1));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], "<=", form.tarih1));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], ">=", form.tarih2));
            }
            else
            {
                if (!string.IsNullOrEmpty(form.islemYapan))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));
                else
                {
                    if (!string.IsNullOrEmpty(form.kisi1))
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI1], "=", form.kisi1));
                    if (!string.IsNullOrEmpty(form.kisi2))
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KISI2], "=", form.kisi2));
                    if (!form.tarih1.isNull)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH1], "=", form.tarih1));
                    if (!form.tarih2.isNull)
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TARIH2], "=", form.tarih2));
                }
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(Vekalet form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AKTIF], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            //string sql = "DELETE FROM " + TABLOAD;
            //sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            return KomutCalistir(sql, 1);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}
