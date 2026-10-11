using System.Data;
using TNS.Veri;
using static TNS.TMM.IsKurallari.Veri.TABSayimAmbarForm;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABSayimKisiForm : TABOrtak
    {
        public TABSayimKisiForm(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "TASSAYIMKISIFORM";
        static string[] ALANLAR = { "KOD", "MUHASEBE", "HARCAMABIRIMI", "BASLAMATARIH", "BITISTARIH", "ADI", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI" };
        public enum ENUMAlanlar
        {
            KOD = 0,
            MUHASEBE = 1,
            HARCAMABIRIMI = 2,
            BASLAMATARIH = 3,
            BITISTARIH = 4,
            ADI = 5,
            ISLEMYAPANAD = 6,
            ISLEMYAPANKOD = 7,
            ISLEMTARIHI = 8,
        }

        static string TABLOADKISI = "TASSAYIMKISI";
        static string[] ALANLARKISI = { "KOD", "SAYIMKOD", "KISIKOD", "KISIAD", "ODAKOD", "ODAAD", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI" };
        public enum ENUMAlanlarKisi
        {
            KOD = 0,
            SAYIMKOD = 1,
            KISIKOD = 2,
            KISIAD = 3,
            ODAKOD = 4,
            ODAAD = 5,
            ISLEMYAPANAD = 6,
            ISLEMYAPANKOD = 7,
            ISLEMTARIHI = 8,
        }

        static string TABLOADKISIDETAY = "TASSAYIMKISIDETAY";
        static string[] ALANLARKISIDETAY = { "KOD", "SAYIMKOD", "KISIKOD", "PRSICILNO", "SICILNO", "MALZEMEAD", "OKUNDU", "FAZLA", "FAZLAKISIKOD", "FAZLAKISIAD", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI", "SAYIMDETAYKOD" };
        public enum ENUMAlanlarKisiDetay
        {
            KOD = 0,
            SAYIMKOD = 1,
            KISIKOD = 2,
            PRSICILNO = 3,
            SICILNO = 4,
            MALZEMEAD = 5,
            OKUNDU = 6,
            FAZLA = 7,
            FAZLAKISIKOD = 8,
            FAZLAKISIAD = 9,
            ISLEMYAPANAD = 10,
            ISLEMYAPANKOD = 11,
            ISLEMTARIHI = 12,
            SAYIMDETAYKOD = 13,
        }

        static string TABLOADVIEW = "TASAMBARZIMMETVIEW";
        static string[] ALANLARVIEW = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "KIMEGITTI", "KISIAD" };
        public enum ENUMAlanlarView
        {
            MUHASEBEKOD = 0,
            HARCAMABIRIMKOD = 1,
            KIMEGITTI = 2,
            KISIAD = 3
        }

        public Sonuc SayimBaslat(SayimKisiForm syk)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(syk.kod) + ", ";
            sql += VeriErisim.DegerYap(syk.muhasebe) + ", ";
            sql += VeriErisim.DegerYap(syk.harcamaBirimi) + ", ";
            sql += VeriErisim.DegerYap(syk.baslamaTarih) + ", ";
            sql += VeriErisim.DegerYap(syk.bitisTarih) + ", ";
            sql += VeriErisim.DegerYap(syk.adi) + ", ";
            sql += VeriErisim.DegerYap(syk.islemYapanAd) + ", ";
            sql += VeriErisim.DegerYap(syk.islemYapanKod) + ", ";
            sql += VeriErisim.DegerYap(syk.islemTarihi) + ") ";
            return KomutCalistir(sql, 1);
        }

        public Sonuc SayimSonlandir(SayimKisiForm syk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], syk.bitisTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], syk.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], syk.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], syk.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", syk.kod));
            return KomutCalistir(sql, 1);
        }

        public IDataReader SayimListele(SayimKisiForm syk)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrWhiteSpace(syk.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", syk.kod));
            if (syk.muhasebe != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBE], "=", syk.muhasebe));
            if (syk.harcamaBirimi != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMI], "=", syk.harcamaBirimi.Replace(".", "")));

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.BITISTARIH] + "";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KisiKaydet(Sayimkisi sk)
        {
            string sql = "INSERT INTO " + TABLOADKISI + "(" + VeriErisim.TumAlanlar("", ALANLARKISI) + ") VALUES (";
            sql += VeriErisim.DegerYap(sk.kod) + ", ";
            sql += VeriErisim.DegerYap(sk.sayimKod) + ", ";
            sql += VeriErisim.DegerYap(sk.kisiKod) + ", ";
            sql += VeriErisim.DegerYap(sk.kisiAd) + ", ";
            sql += VeriErisim.DegerYap(sk.odaKod) + ", ";
            sql += VeriErisim.DegerYap(sk.odaAd) + ", ";
            sql += VeriErisim.DegerYap(sk.islemYapanAd) + ", ";
            sql += VeriErisim.DegerYap(sk.islemYapanKod) + ", ";
            sql += VeriErisim.DegerYap(sk.islemTarihi) + ") ";
            return KomutCalistir(sql, 1);
        }

        public IDataReader KisiListele(Sayimkisi sk)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARKISI);
            sql += ",(SELECT COUNT(*) FROM " + TABLOADKISIDETAY + " WHERE KD.SAYIMKOD=SAYIMKOD AND (KD.KISIKOD=KISIKOD OR KD.ODAKOD=KISIKOD) AND FAZLA=0 )";
            sql += ",(SELECT SUM(OKUNDU) FROM " + TABLOADKISIDETAY + " WHERE KD.SAYIMKOD=SAYIMKOD AND (KD.KISIKOD=KISIKOD  OR KD.ODAKOD=KISIKOD) AND FAZLA=0 )";
            sql += ",(SELECT SUM(FAZLA) FROM " + TABLOADKISIDETAY + " WHERE KD.SAYIMKOD=SAYIMKOD AND (KD.KISIKOD=KISIKOD  OR KD.ODAKOD=KISIKOD) )";
            sql += " FROM " + TABLOADKISI + " KD ";

            if (!string.IsNullOrWhiteSpace(sk.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISI[(int)ENUMAlanlarKisi.KOD], "=", sk.kod));
            if (!string.IsNullOrWhiteSpace(sk.sayimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISI[(int)ENUMAlanlarKisi.SAYIMKOD], "=", sk.sayimKod));
            if (!string.IsNullOrWhiteSpace(sk.kisiKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISI[(int)ENUMAlanlarKisi.KISIKOD], "=", sk.kisiKod));
            if (!string.IsNullOrWhiteSpace(sk.odaKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISI[(int)ENUMAlanlarKisi.ODAKOD], "=", sk.odaKod));
            if (!string.IsNullOrWhiteSpace(sk.islemYapanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISI[(int)ENUMAlanlarKisi.ISLEMYAPANKOD], "=", sk.islemYapanKod));


            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARKISI[(int)ENUMAlanlarKisi.KISIAD] + "";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KisiDetayKaydet(SayimKisiDetay skd)
        {
            string sql = "INSERT INTO " + TABLOADKISIDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARKISIDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(skd.kod) + ", ";
            sql += VeriErisim.DegerYap(skd.sayimKod) + ", ";
            sql += VeriErisim.DegerYap(skd.kisiKod) + ", ";
            sql += VeriErisim.DegerYap(skd.prSicilNo) + ", ";
            sql += VeriErisim.DegerYap(skd.sicilNo) + ", ";
            sql += VeriErisim.DegerYap(skd.malzemeAd) + ", ";
            sql += VeriErisim.DegerYap(skd.okundu) + ", ";
            sql += VeriErisim.DegerYap(skd.fazla) + ", ";
            sql += VeriErisim.DegerYap(skd.fazlaKisiKod) + ", ";
            sql += VeriErisim.DegerYap(skd.fazlaKisiAd) + ", ";
            sql += VeriErisim.DegerYap(skd.islemYapanAd) + ", ";
            sql += VeriErisim.DegerYap(skd.islemYapanKod) + ", ";
            sql += VeriErisim.DegerYap(skd.islemTarihi) + ", ";
            sql += VeriErisim.DegerYap(skd.sayimDetayKod) + ") ";
            return KomutCalistir(sql, 1);
        }

        public Sonuc KisiDetayOkundu(SayimKisiDetay skd)
        {
            string sql = "UPDATE " + TABLOADKISIDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.OKUNDU], skd.okundu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], skd.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], skd.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], skd.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SAYIMKOD], "=", skd.sayimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.PRSICILNO], "=", skd.prSicilNo));

            if (skd.kisiKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.KISIKOD], "=", skd.kisiKod));
            if (skd.sayimDetayKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SAYIMDETAYKOD], "=", skd.sayimDetayKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Kisis the detay listele.
        /// </summary>
        /// <param name="sk">The sk.</param>
        /// <returns></returns>
        public IDataReader KisiDetayListele(SayimKisiDetay sk)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKISIDETAY, ALANLARKISIDETAY) + ",TASSICILNORFID.RFIDNO FROM " + TABLOADKISIDETAY;
            sql += " LEFT OUTER JOIN TASSICILNORFID ON " + TABLOADKISIDETAY + ".PRSICILNO = TASSICILNORFID.PRSICILNO";

            if (!string.IsNullOrWhiteSpace(sk.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.KOD], "=", sk.kod));
            if (!string.IsNullOrWhiteSpace(sk.sayimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SAYIMKOD], "=", sk.sayimKod));
            if (!string.IsNullOrWhiteSpace(sk.sayimDetayKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SAYIMDETAYKOD], "=", sk.sayimDetayKod));
            if (!string.IsNullOrWhiteSpace(sk.kisiKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.KISIKOD], "=", sk.kisiKod));
            if (sk.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKISIDETAY, ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.PRSICILNO], "=", sk.prSicilNo));
            if (!string.IsNullOrWhiteSpace(sk.sicilNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SICILNO], "=", sk.sicilNo));

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.OKUNDU] + "," + ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SICILNO];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc FazlaSil(SayimKisiDetay skd)
        {
            string sql = "DELETE FROM " + TABLOADKISIDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKISIDETAY, ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.KOD], "=", skd.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Rapor(Sayimkisi sk)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADKISI, ALANLARKISI) + ",TASSAYIMKISIDETAY.SICILNO, TASSAYIMKISIDETAY.MALZEMEAD, TASSAYIMKISIDETAY.OKUNDU, TASSAYIMKISIDETAY.FAZLA, TASSAYIMKISIDETAY.FAZLAKISIKOD, TASSAYIMKISIDETAY.FAZLAKISIAD, TASSAYIMKISIFORM.BASLAMATARIH, TASSAYIMKISIFORM.BITISTARIH, TASSAYIMKISIFORM.ADI ";
            sql += " FROM " + TABLOADKISI;
            sql += " INNER JOIN TASSAYIMKISIFORM ON TASSAYIMKISI.SAYIMKOD=TASSAYIMKISIFORM.KOD ";
            sql += " INNER JOIN TASSAYIMKISIDETAY ON (TASSAYIMKISI.KISIKOD=TASSAYIMKISIDETAY.KISIKOD OR TASSAYIMKISI.ODAKOD=TASSAYIMKISIDETAY.KISIKOD) ";
            sql += " AND TASSAYIMKISI.SAYIMKOD = TASSAYIMKISIDETAY.SAYIMKOD";
            sql += " AND TASSAYIMKISI.KOD = TASSAYIMKISIDETAY.SAYIMDETAYKOD ";

            if (!string.IsNullOrEmpty(sk.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKISI, ALANLARKISI[(int)ENUMAlanlarKisi.KOD], "=", sk.kod));

            if (!string.IsNullOrEmpty(sk.sayimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKISI, ALANLARKISI[(int)ENUMAlanlarKisi.SAYIMKOD], "=", sk.sayimKod));

            if (!string.IsNullOrEmpty(sk.kisiKod))
            {
                string ekKosul = VeriErisim.KosulEkle("", "OR", VeriErisim.KosulOl(TABLOADKISI, ALANLARKISI[(int)ENUMAlanlarKisi.KISIKOD], "=", sk.kisiKod));
                ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl(TABLOADKISI, ALANLARKISI[(int)ENUMAlanlarKisi.ODAKOD], "=", sk.kisiKod));
                kosul += " AND (" + ekKosul + ") ";
            }

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.OKUNDU] + " DESC," + ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.FAZLA] + "," + ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.KISIKOD] + "," + ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.FAZLAKISIKOD] + "," + ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SICILNO];
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader KisiRapor(Sayimkisi sk)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT MUHASEBEKOD, HARCAMABIRIMKOD, KIMEGITTI, KISIAD ";
            sql += " FROM TASAMBARZIMMETVIEW ";
            sql += " WHERE KIMEGITTI NOT IN(";
            sql += " SELECT TASSAYIMKISI.KISIKOD FROM " + TABLOADKISI;
            sql += " INNER JOIN TASSAYIMKISIFORM ON TASSAYIMKISI.SAYIMKOD = TASSAYIMKISIFORM.KOD ";
            sql += " WHERE TASSAYIMKISIFORM.MUHASEBE=TASAMBARZIMMETVIEW.MUHASEBEKOD AND ";
            sql += " TASSAYIMKISIFORM.HARCAMABIRIMI=TASAMBARZIMMETVIEW.HARCAMABIRIMKOD AND ";

            if (!string.IsNullOrEmpty(sk.sayimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADKISI, ALANLARKISI[(int)ENUMAlanlarKisi.SAYIMKOD], "=", sk.sayimKod));
            sql += kosul + ")";
            kosul = "";

            if (!string.IsNullOrEmpty(sk.muhasebeKod))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "MUHASEBEKOD", "=", sk.muhasebeKod));
                sql += " AND " + kosul;
                kosul = "";
            }

            if (!string.IsNullOrEmpty(sk.harcamabirimKod))
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBARZIMMETVIEW", "HARCAMABIRIMKOD", "=", sk.harcamabirimKod));
                sql += " AND " + kosul;
            }

            sql += " GROUP BY MUHASEBEKOD, HARCAMABIRIMKOD, KIMEGITTI, KISIAD ";
            sql += " ORDER BY KISIAD ";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SayimKisiDetaySil(string sayimKod)
        {
            string sql = "DELETE FROM " + TABLOADKISIDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKISIDETAY, ALANLARKISIDETAY[(int)ENUMAlanlarKisiDetay.SAYIMKOD], "=", sayimKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SayimKisiSil(string sayimKod)
        {
            string sql = "DELETE FROM " + TABLOADKISI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADKISI, ALANLARKISI[(int)ENUMAlanlarKisi.SAYIMKOD], "=", sayimKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SayimKisiFormSil(string sayimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", sayimKod));

            return KomutCalistir(sql, 1);
        }

    }
}
