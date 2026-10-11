using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    public class TABSayimAmbarForm : TABOrtak
    {
        public TABSayimAmbarForm(VeriErisim veriErisim) : base(veriErisim) { }

        static string TABLOAD = "TASSAYIMAMBARFORM";
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

        static string TABLOADAMBAR = "TASSAYIMAMBAR";
        static string[] ALANLARAMBAR = { "KOD", "SAYIMKOD", "AMBARKOD", "AMBARAD", "ODAKOD", "ODAAD", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI" };
        public enum ENUMAlanlarAmbar
        {
            KOD = 0,
            SAYIMKOD = 1,
            AMBARKOD = 2,
            AMBARAD = 3,
            ODAKOD = 4,
            ODAAD = 5,
            ISLEMYAPANAD = 6,
            ISLEMYAPANKOD = 7,
            ISLEMTARIHI = 8,
        }

        static string TABLOADAMBARDETAY = "TASSAYIMAMBARDETAY";
        static string[] ALANLARAMBARDETAY = { "KOD", "SAYIMKOD", "AMBARKOD", "PRSICILNO", "SICILNO", "MALZEMEAD", "OKUNDU", "FAZLA", "FAZLAAMBARKOD", "FAZLAAMBARAD", "ISLEMYAPANAD", "ISLEMYAPANKOD", "ISLEMTARIHI", "SAYIMDETAYKOD" };
        public enum ENUMAlanlarAmbarDetay
        {
            KOD = 0,
            SAYIMKOD = 1,
            AMBARKOD = 2,
            PRSICILNO = 3,
            SICILNO = 4,
            MALZEMEAD = 5,
            OKUNDU = 6,
            FAZLA = 7,
            FAZLAAMBARKOD = 8,
            FAZLAAMBARAD = 9,
            ISLEMYAPANAD = 10,
            ISLEMYAPANKOD = 11,
            ISLEMTARIHI = 12,
            SAYIMDETAYKOD = 13,
        }

        public Sonuc SayimBaslat(SayimAmbarForm syk)
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

        public Sonuc SayimSonlandir(SayimAmbarForm syk)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], syk.bitisTarih) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], syk.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], syk.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], syk.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", syk.kod));
            return KomutCalistir(sql, 1);
        }

        public IDataReader SayimListele(SayimAmbarForm syk)
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

        public Sonuc AmbarKaydet(Sayimambar sk)
        {
            string sql = "INSERT INTO " + TABLOADAMBAR + "(" + VeriErisim.TumAlanlar("", ALANLARAMBAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(sk.kod) + ", ";
            sql += VeriErisim.DegerYap(sk.sayimKod) + ", ";
            sql += VeriErisim.DegerYap(sk.ambarKod) + ", ";
            sql += VeriErisim.DegerYap(sk.ambarAd) + ", ";
            sql += VeriErisim.DegerYap(sk.odaKod) + ", ";
            sql += VeriErisim.DegerYap(sk.odaAd) + ", ";
            sql += VeriErisim.DegerYap(sk.islemYapanAd) + ", ";
            sql += VeriErisim.DegerYap(sk.islemYapanKod) + ", ";
            sql += VeriErisim.DegerYap(sk.islemTarihi) + ") ";
            return KomutCalistir(sql, 1);
        }

        public IDataReader AmbarListele(Sayimambar sk)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARAMBAR);
            sql += ",(SELECT COUNT(*) FROM " + TABLOADAMBARDETAY + " WHERE KD.SAYIMKOD=SAYIMKOD AND KD.AMBARKOD=AMBARKOD AND FAZLA=0 )";
            sql += ",(SELECT SUM(OKUNDU) FROM " + TABLOADAMBARDETAY + " WHERE KD.SAYIMKOD=SAYIMKOD AND KD.AMBARKOD=AMBARKOD AND FAZLA=0 )";
            sql += ",(SELECT SUM(FAZLA) FROM " + TABLOADAMBARDETAY + " WHERE KD.SAYIMKOD=SAYIMKOD AND KD.AMBARKOD=AMBARKOD  )";
            sql += " FROM " + TABLOADAMBAR + " KD ";

            if (!string.IsNullOrWhiteSpace(sk.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBAR[(int)ENUMAlanlarAmbar.KOD], "=", sk.kod));
            if (!string.IsNullOrWhiteSpace(sk.sayimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBAR[(int)ENUMAlanlarAmbar.SAYIMKOD], "=", sk.sayimKod));
            if (!string.IsNullOrWhiteSpace(sk.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBAR[(int)ENUMAlanlarAmbar.AMBARKOD], "=", sk.ambarKod));
            if (!string.IsNullOrWhiteSpace(sk.islemYapanKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBAR[(int)ENUMAlanlarAmbar.ISLEMYAPANKOD], "=", sk.islemYapanKod));

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARAMBAR[(int)ENUMAlanlarAmbar.AMBARAD] + "";
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc AmbarDetayKaydet(SayimAmbarDetay skd)
        {
            string sql = "INSERT INTO " + TABLOADAMBARDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARAMBARDETAY) + ") VALUES (";
            sql += VeriErisim.DegerYap(skd.kod) + ", ";
            sql += VeriErisim.DegerYap(skd.sayimKod) + ", ";
            sql += VeriErisim.DegerYap(skd.ambarKod) + ", ";
            sql += VeriErisim.DegerYap(skd.prSicilNo) + ", ";
            sql += VeriErisim.DegerYap(skd.sicilNo) + ", ";
            sql += VeriErisim.DegerYap(skd.malzemeAd) + ", ";
            sql += VeriErisim.DegerYap(skd.okundu) + ", ";
            sql += VeriErisim.DegerYap(skd.fazla) + ", ";
            sql += VeriErisim.DegerYap(skd.fazlaAmbarKod) + ", ";
            sql += VeriErisim.DegerYap(skd.fazlaAmbarAd) + ", ";
            sql += VeriErisim.DegerYap(skd.islemYapanAd) + ", ";
            sql += VeriErisim.DegerYap(skd.islemYapanKod) + ", ";
            sql += VeriErisim.DegerYap(skd.islemTarihi) + ", ";
            sql += VeriErisim.DegerYap(skd.sayimDetayKod) + ") ";
            return KomutCalistir(sql, 1);
        }

        public Sonuc AmbarDetayOkundu(SayimAmbarDetay skd)
        {
            string sql = "UPDATE " + TABLOADAMBARDETAY + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.OKUNDU], skd.okundu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANAD], skd.islemYapanAd) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPANKOD], skd.islemYapanKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], skd.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SAYIMKOD], "=", skd.sayimKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.PRSICILNO], "=", skd.prSicilNo));

            if (skd.ambarKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.AMBARKOD], "=", skd.ambarKod));
            if (skd.sayimDetayKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SAYIMDETAYKOD], "=", skd.sayimDetayKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Ambars the detay listele.
        /// </summary>
        /// <param name="sk">The sk.</param>
        /// <returns></returns>
        public IDataReader AmbarDetayListele(SayimAmbarDetay sk)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADAMBARDETAY, ALANLARAMBARDETAY) + ",TASSICILNORFID.RFIDNO FROM " + TABLOADAMBARDETAY;
            sql += " LEFT OUTER JOIN TASSICILNORFID ON " + TABLOADAMBARDETAY + ".PRSICILNO = TASSICILNORFID.PRSICILNO";

            if (!string.IsNullOrWhiteSpace(sk.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.KOD], "=", sk.kod));
            if (!string.IsNullOrWhiteSpace(sk.sayimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SAYIMKOD], "=", sk.sayimKod));
            if (!string.IsNullOrWhiteSpace(sk.sayimDetayKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SAYIMDETAYKOD], "=", sk.sayimDetayKod));
            if (!string.IsNullOrWhiteSpace(sk.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.AMBARKOD], "=", sk.ambarKod));
            if (sk.prSicilNo > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADAMBARDETAY, ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.PRSICILNO], "=", sk.prSicilNo));
            if (!string.IsNullOrWhiteSpace(sk.sicilNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SICILNO], "=", sk.sicilNo));

            if (!string.IsNullOrWhiteSpace(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.OKUNDU] + "," + ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SICILNO];
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc FazlaSil(SayimAmbarDetay skd)
        {
            string sql = "DELETE FROM " + TABLOADAMBARDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADAMBARDETAY, ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.KOD], "=", skd.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader Rapor(Sayimambar sayim)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADAMBAR, ALANLARAMBAR) + ", TASSAYIMAMBARDETAY.SICILNO, TASSAYIMAMBARDETAY.MALZEMEAD, TASSAYIMAMBARDETAY.OKUNDU, TASSAYIMAMBARDETAY.FAZLA, TASSAYIMAMBARDETAY.FAZLAAMBARKOD, TASSAYIMAMBARDETAY.FAZLAAMBARAD, TASAMBAR.AD, TASSAYIMAMBARFORM.BASLAMATARIH, TASSAYIMAMBARFORM.BITISTARIH, TASSAYIMAMBARFORM.ADI ";
            sql += "FROM " + TABLOADAMBAR;
            sql += " INNER JOIN TASSAYIMAMBARFORM ON TASSAYIMAMBAR.SAYIMKOD=TASSAYIMAMBARFORM.KOD ";
            sql += " INNER JOIN TASSAYIMAMBARDETAY ON TASSAYIMAMBAR.SAYIMKOD=TASSAYIMAMBARDETAY.SAYIMKOD AND TASSAYIMAMBAR.AMBARKOD = TASSAYIMAMBARDETAY.AMBARKOD ";
            sql += " INNER JOIN TASAMBAR ON TASSAYIMAMBAR.AMBARKOD=TASAMBAR.KOD ";

            if (!string.IsNullOrEmpty(sayim.sayimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADAMBAR, ALANLARAMBAR[(int)ENUMAlanlarAmbar.SAYIMKOD], "=", sayim.sayimKod));

            if (!string.IsNullOrEmpty(sayim.harcamabirimKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBAR", "HARCAMABIRIMKOD", "=", sayim.harcamabirimKod));

            if (!string.IsNullOrEmpty(sayim.ambarKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("TASAMBAR", "KOD", "=", sayim.ambarKod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.OKUNDU] + " DESC," + ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.FAZLA] + "," + ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.AMBARKOD] + "," + ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.FAZLAAMBARKOD] + "," + ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SICILNO];
            return veriErisim.DataReaderDondur(sql);
        }


        public Sonuc SayimAmbarDetaySil(string sayimKod)
        {
            string sql = "DELETE FROM " + TABLOADAMBARDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADAMBARDETAY, ALANLARAMBARDETAY[(int)ENUMAlanlarAmbarDetay.SAYIMKOD], "=", sayimKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SayimAmbarSil(string sayimKod)
        {
            string sql = "DELETE FROM " + TABLOADAMBAR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADAMBAR, ALANLARAMBAR[(int)ENUMAlanlarAmbar.SAYIMKOD], "=", sayimKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc SayimAmbarFormSil(string sayimKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", sayimKod));

            return KomutCalistir(sql, 1);
        }
    }
}
