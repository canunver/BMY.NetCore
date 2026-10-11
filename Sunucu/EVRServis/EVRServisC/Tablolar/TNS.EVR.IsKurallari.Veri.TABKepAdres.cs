using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABKepAdres : TABOrtak
    {
        public TABKepAdres(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRKEPADRES";

        public enum ENUMAlanlar : int
        {
            KOD = 0, EVRAKBIRIMKODU = 1, ADRES = 2, TIP = 3, KULLANIMSAYISI = 4, KALICI = 5, ISLEMTARIHI = 6, ISLEMYAPAN = 7, ISLEMYAPANAD = 8, IPTAL = 9
        }

        public static string[] ALANLAR = { "KOD", "EVRAKBIRIMKODU", "ADRES", "TIP", "KULLANIMSAYISI", "KALICI", "ISLEMTARIHI", "ISLEMYAPAN", "ISLEMYAPANAD", "IPTAL" };

        public Sonuc Kaydet(KepAdres form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
                sql += VeriErisim.DegerYap(form.adres) + ",";
                sql += VeriErisim.DegerYap((int)form.tip) + ",";
                sql += VeriErisim.DegerYap(form.kullanimSayisi) + ",";
                sql += VeriErisim.DegerYap(form.kalici) + ",";
                sql += VeriErisim.DegerYap(form.islemTarihi) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
                sql += VeriErisim.DegerYap(form.iptal) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        public Sonuc Guncelle(KepAdres form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";

            if (form.tip != ENUMKepAdresTip.TANIMSIZ)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TIP], (int)form.tip) + ",";
            if (form.adres != "")
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ADRES], form.adres) + ",";
            if (form.kullanimSayisi > 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANIMSAYISI], form.kullanimSayisi) + ",";
            if (form.kalici != 0)
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANIMSAYISI], form.kalici > 0 ? form.kalici : 0) + ",";

            sql = sql.TrimEnd(',');

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(KepAdres form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IPTAL], "=", form.iptal));

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                string ekKosul = "";

                if (form.evrakBirimKod != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", form.evrakBirimKod));
                if (form.adres != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ADRES], "LIKE", "%" + form.adres.Trim() + "%"));
                if (form.kullanimSayisi > 0)
                {
                    string ekKosul2 = VeriErisim.KosulEkle("", "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANIMSAYISI], ">", form.kullanimSayisi));
                    ekKosul2 = VeriErisim.KosulEkle(ekKosul2, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KALICI], "=", 1));
                    ekKosul += " AND (" + ekKosul2 + ")";
                }
                if (form.islemYapan != "")
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));

                if (ekKosul != "")
                    sql += " AND " + ekKosul;
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KULLANIMSAYISI] + " DESC";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(KepAdres form)
        {
            //string sql = "DELETE FROM " + TABLOAD;
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IPTAL], form.iptal);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

    }
}
