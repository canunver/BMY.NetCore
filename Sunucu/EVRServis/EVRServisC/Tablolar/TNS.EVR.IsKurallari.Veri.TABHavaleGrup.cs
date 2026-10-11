using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{

    public class TABHavaleGrup : TABOrtak
    {
        public TABHavaleGrup(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "EVRHAVALEGRUP";
        public static string TABLOADDETAY = "EVRHAVALEGRUPDETAY";
        public static string TABLOADPAYLAS = "EVRHAVALEGRUPPAYLAS";

        public enum ENUMAlanlar : int
        {
            KOD = 0, AD = 1, ISLEMYAPAN = 2, ISLEMYAPANAD = 3, EVRAKBIRIMKOD = 4, PAYLASIMDURUMU = 5, VARSAYILAN = 6
        }
        public enum ENUMAlanlarDetay : int
        {
            HAVALEGRUPKOD = 0, SIRANO = 1, TCKIMLIKNO = 2, AD = 3, SOYAD = 4, EVRAKBIRIMKOD = 5, EVRAKBIRIMAD = 6, SAYAC = 7
        }
        public enum ENUMAlanlarPaylas : int
        {
            HAVALEGRUPKOD = 0, TCKIMLIKNO = 1, AD = 2, SOYAD = 3, EVRAKBIRIMKOD = 4, EVRAKBIRIMAD = 5
        }

        public static string[] ALANLAR = { "KOD", "AD", "ISLEMYAPAN", "ISLEMYAPANAD", "EVRAKBIRIMKOD", "PAYLASIMDURUMU", "VARSAYILAN" };
        public static string[] ALANLARDETAY = { "HAVALEGRUPKOD", "SIRANO", "TCKIMLIKNO", "AD", "SOYAD", "EVRAKBIRIMKOD", "EVRAKBIRIMAD", "SAYAC" };
        public static string[] ALANLARPAYLAS = { "HAVALEGRUPKOD", "TCKIMLIKNO", "AD", "SOYAD", "EVRAKBIRIMKOD", "EVRAKBIRIMAD" };

        public Sonuc Kaydet(HavaleGrup form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap(form.islemYapan) + ",";
                sql += VeriErisim.DegerYap(form.islemYapanAd) + ",";
                sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
                sql += VeriErisim.DegerYap(form.paylasimDurumu) + ",";
                sql += VeriErisim.DegerYap(form.varsayilan) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(HavaleGrup form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc Kaydet(HavaleGrupDetay detay)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(detay.havaleGrupKod) + ",";
            sql += VeriErisim.DegerYap(detay.siraNo) + ",";
            sql += VeriErisim.DegerYap(detay.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(detay.ad) + ",";
            sql += VeriErisim.DegerYap(detay.soyad) + ",";
            sql += VeriErisim.DegerYap(detay.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(detay.evrakBirimAd) + ",";
            sql += VeriErisim.DegerYap(detay.sayac) + ")";

            return KomutCalistir(sql, 1);
        }
        public Sonuc Kaydet(HavaleGrupPaylas form)
        {
            string sql = "INSERT INTO " + TABLOADPAYLAS + " (" + VeriErisim.TumAlanlar("", ALANLARPAYLAS) + ") VALUES (";
            sql += VeriErisim.DegerYap(form.havaleGrupKod) + ",";
            sql += VeriErisim.DegerYap(form.tcKimlikNo) + ",";
            sql += VeriErisim.DegerYap(form.ad) + ",";
            sql += VeriErisim.DegerYap(form.soyad) + ",";
            sql += VeriErisim.DegerYap(form.evrakBirimKod) + ",";
            sql += VeriErisim.DegerYap(form.evrakBirimAd) + ")";

            return KomutCalistir(sql, 1);
        }

        public IDataReader Listele(HavaleGrup form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            string ekKosul = "";

            if (form.kod > 0)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {

                if (form.paylasimDurumu > 0)
                {
                    sql = sql.Replace("SELECT", "SELECT DISTINCT");
                    sql = sql.Replace(" FROM ", " ,EVRHAVALEGRUPPAYLAS.TCKIMLIKNO FROM ");
                    sql += " LEFT OUTER JOIN EVRHAVALEGRUPPAYLAS ON EVRHAVALEGRUP.KOD = EVRHAVALEGRUPPAYLAS.HAVALEGRUPKOD ";

                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", form.evrakBirimKod));
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PAYLASIMDURUMU], "=", form.paylasimDurumu));

                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", form.evrakBirimKod));
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "AND", VeriErisim.KosulOl(TABLOADPAYLAS, ALANLARPAYLAS[(int)ENUMAlanlarPaylas.TCKIMLIKNO], "=", form.islemYapan));
                }

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", form.evrakBirimKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));


                if (ekKosul != "")
                    sql += " OR " + ekKosul;

            }

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListeleDetay(int havaleGrupKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARDETAY) + " FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HAVALEGRUPKOD], "=", havaleGrupKod));
            sql += " ORDER BY " + ALANLARDETAY[(int)ENUMAlanlarDetay.SIRANO];

            return veriErisim.DataReaderDondur(sql);
        }
        public IDataReader ListelePaylas(int havaleGrupKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARPAYLAS) + " FROM " + TABLOADPAYLAS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPAYLAS[(int)ENUMAlanlarPaylas.HAVALEGRUPKOD], "=", havaleGrupKod));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(HavaleGrup form)
        {
            Sonuc sonuc = this.SilDetay(form.kod);
            if (sonuc.islemSonuc)
                sonuc = this.SilPaylas(form.kod);

            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc SilDetay(int havaleGrupKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HAVALEGRUPKOD], "=", havaleGrupKod));

            return KomutCalistir(sql, 0);
        }
        public Sonuc SilPaylas(int havaleGrupKod)
        {
            string sql = "DELETE FROM " + TABLOADPAYLAS;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPAYLAS[(int)ENUMAlanlarPaylas.HAVALEGRUPKOD], "=", havaleGrupKod));

            return KomutCalistir(sql, 0);
        }

        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }

        public Sonuc SayacGuncelle(HavaleGrupDetay detay)
        {
            string sql = "UPDATE " + TABLOADDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.SAYAC], detay.sayac);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HAVALEGRUPKOD], "=", detay.havaleGrupKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.TCKIMLIKNO], "=", detay.tcKimlikNo));
            if (string.IsNullOrWhiteSpace(detay.tcKimlikNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.EVRAKBIRIMKOD], "=", detay.evrakBirimKod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc VarsayilanYap(HavaleGrup form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.VARSAYILAN], 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKOD], "=", form.evrakBirimKod));

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

        public Sonuc PaylasimDurumuBelirle(HavaleGrup form)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.PAYLASIMDURUMU], form.paylasimDurumu);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "=", form.islemYapan));

            return KomutCalistir(sql, 1);
        }
    }
}
