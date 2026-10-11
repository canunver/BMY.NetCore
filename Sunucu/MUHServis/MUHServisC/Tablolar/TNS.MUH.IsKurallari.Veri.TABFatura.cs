using System;
using System.Data;
using TNS.Veri;

namespace TNS.MUH.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABFatura : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABFatura"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABFatura(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "MUHFATURA";
        public static string[] ALANLAR = { "KOD", "MUSTERIKOD", "FATURANO", "FATURATARIHI", "IRSALIYENO", "IRSALIYETARIHI", "ACIKLAMA", "TOPLAM", "KDVTOPLAM", "GENELTOPLAM", "ODEMETARIHI", "ODENENTOPLAM", "ODENENTUTAR", "KESINTITUTAR", "ODEMEPARABIRIMI", "FATURADILI", "FATURAPARABIRIMI", "DURUM", "MUSTERIADI", "MUHASEBEKOD", "BIRIMKOD", "TAHAKKUKBELGENO", "ODEMEBELGENO", "MUHBELGEACIKLAMA", "TAHAKKUKHESAPKOD", "ODEMEHESAPKOD", "ISLEMYAPAN", "ISLEMTARIHI", "MUSTERIADIEK", "MUDURLUKKOD", "TEVKIFATORAN", "TEVKIFATTUTAR" };
        public enum ENUMAlanlar
        {
            KOD = 0,
            MUSTERIKOD = 1,
            FATURANO = 2,
            FATURATARIHI = 3,
            IRSALIYENO = 4,
            IRSALIYETARIHI = 5,
            ACIKLAMA = 6,
            TOPLAM = 7,
            KDVTOPLAM = 8,
            GENELTOPLAM = 9,
            ODEMETARIHI = 10,
            ODENENTOPLAM = 11,
            ODENENTUTAR = 12,
            KESINTITUTAR = 13,
            ODEMEPARABIRIMI = 14,
            FATURADILI = 15,
            FATURAPARABIRIMI = 16,
            DURUM = 17,
            MUSTERIADI = 18,
            MUHASEBEKOD = 19,
            BIRIMKOD = 20,
            TAHAKKUKBELGENO = 21,
            ODEMEBELGENO = 22,
            MUHBELGEACIKLAMA = 23,
            TAHAKKUKHESAPKOD = 24,
            ODEMEHESAPKOD = 25,
            ISLEMYAPAN = 26,
            ISLEMTARIHI = 27,
            MUSTERIADIEK = 28,
            MUDURLUKKOD = 29,
            TEVKIFATORAN = 30,
            TEVKIFATTUTAR = 31
        }

        public static string TABLOADDETAY = "MUHFATURADETAY";
        public static string[] ALANLARDETAY = { "KOD", "FATURAKOD", "ACIKLAMA", "MIKTAR", "KDVORAN", "BIRIMFIYAT", "KDVTUTAR", "TUTAR", "HESAPKOD" };
        public enum ENUMAlanlarDetay
        {
            KOD = 0,
            FATURAKOD = 1,
            ACIKLAMA = 2,
            MIKTAR = 3,
            KDVORAN = 4,
            BIRIMFIYAT = 5,
            KDVTUTAR = 6,
            TUTAR = 7,
            HESAPKOD = 8,
        }

        public static string TABLOADACIKLAMA = "MUHFATURAACIKLAMA";
        public static string[] ALANLARACIKLAMA = { "KOD", "TUR", "ACIKLAMA" };
        public enum ENUMAlanlarAciklama
        {
            KOD = 0,
            TUR = 1,
            ACIKLAMA = 2
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(Fatura bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.musteriKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.faturaNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.faturaTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.irsaliyeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.irsaliyeTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ",";
            sql = sql + VeriErisim.DegerYap(bilgi.toplam) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kdvToplam) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.genelToplam) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.odemeTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.odenenToplam) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.odenenTutar) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kesintiTutar) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.odemeParaBirimi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.faturaDili) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.faturaParaBirimi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.durum) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.musteriAdi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.muhasebeKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.birimKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tahakkukBelgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.odemeBelgeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.muhBelgeAciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tahakkukHesapKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.odemeHesapKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemYapan) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.islemTarihi) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.musteriAdiEk) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.mudurlukKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tevkifatOran) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tevkifatTutar) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Guncelles the specified bilgi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Guncelle(Fatura bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], bilgi.muhasebeKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], bilgi.birimKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUDURLUKKOD], bilgi.mudurlukKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUSTERIKOD], bilgi.musteriKod) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURANO], bilgi.faturaNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURATARIHI], bilgi.faturaTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IRSALIYENO], bilgi.irsaliyeNo) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IRSALIYETARIHI], bilgi.irsaliyeTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], bilgi.aciklama) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TOPLAM], bilgi.toplam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KDVTOPLAM], bilgi.kdvToplam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.GENELTOPLAM], bilgi.genelToplam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEVKIFATORAN], bilgi.tevkifatOran) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TEVKIFATTUTAR], bilgi.tevkifatTutar) + ", ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUSTERIADIEK], bilgi.musteriAdiEk) + ", ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum) + ", ";

            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], bilgi.islemYapan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], bilgi.islemTarihi) + ", ";

            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAHAKKUKHESAPKOD], bilgi.tahakkukHesapKod) + ", ";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEHESAPKOD], bilgi.odemeHesapKod) + ", ";

            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMETARIHI], bilgi.odemeTarihi) + ", ";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENENTUTAR], bilgi.odenenTutar) + ", ";
            //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEPARABIRIMI], bilgi.odemeParaBirimi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURADILI], bilgi.faturaDili) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FATURAPARABIRIMI], bilgi.faturaParaBirimi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUSTERIADI], bilgi.musteriAdi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHBELGEACIKLAMA], bilgi.muhBelgeAciklama);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public Sonuc DurumGuncelle(Fatura bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], bilgi.islemYapan) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], bilgi.islemTarihi);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(Fatura bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", MUHFATURADETAY.ACIKLAMA, MUHFATURADETAY.MIKTAR, MUHFATURADETAY.KDVORAN, MUHFATURADETAY.BIRIMFIYAT, MUHFATURADETAY.KDVTUTAR, MUHFATURADETAY.TUTAR, MUHFATURADETAY.HESAPKOD";
            sql += " FROM " + TABLOAD;
            sql += " LEFT OUTER JOIN ";
            sql += " MUHFATURADETAY ON MUHFATURA.KOD = MUHFATURADETAY.FATURAKOD ";

            if (bilgi.soruguKod != null)
            {
                for (int i = 0; i < bilgi.soruguKod.Length; i++)
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.soruguKod[i]));
                kosul = "(" + kosul + ")";
            }
            else if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.muhasebeKod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", bilgi.muhasebeKod));

            if (bilgi.birimKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", bilgi.birimKod));

            if (bilgi.mudurlukKod != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUDURLUKKOD], "=", bilgi.mudurlukKod));

            if (!string.IsNullOrEmpty(bilgi.musteriKod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUSTERIKOD], "=", bilgi.musteriKod));

            if (!string.IsNullOrEmpty(bilgi.faturaNo))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURANO], "=", bilgi.faturaNo));

            if (bilgi.genelToplam > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GENELTOPLAM], "=", bilgi.genelToplam));

            if (!bilgi.faturaTarihi.isNull)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURATARIHI], "=", bilgi.faturaTarihi));
            else
            {
                if (!bilgi.sorguBasTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURATARIHI], ">=", bilgi.sorguBasTarih));
                if (!bilgi.sorguBitTarih.isNull)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.FATURATARIHI], "<=", bilgi.sorguBitTarih));
            }

            if (bilgi.islemYapan != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], "LIKE", bilgi.islemYapan + "%"));


            if (bilgi.kod == 0)
            {
                if (bilgi.durum == (int)ENUMFaturaDurum.BUTUNFATURALAR)
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "<", (int)ENUMFaturaDurum.IPTAL));
                else
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DURUM], "=", bilgi.durum));
            }

            if (!string.IsNullOrEmpty(kosul))
            {
                sql += " WHERE " + kosul;
            }
            sql += "ORDER BY MUHFATURA.FATURANO";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Sils the specified bilgi.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        //public Sonuc Sil(Fatura bilgi)
        //{
        //    string kosul = "";
        //    string sql = "DELETE FROM " + TABLOAD;
        //    if (bilgi.kod > 0)
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

        //    if (!string.IsNullOrEmpty(kosul))
        //    {
        //        sql += " WHERE " + kosul;
        //    }
        //    return KomutCalistir(sql, 1);
        //}

        public Sonuc OdemeGuncelle(Fatura bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENENTOPLAM], bilgi.odenenToplam) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODENENTUTAR], bilgi.odenenTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KESINTITUTAR], bilgi.kesintiTutar) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMETARIHI], bilgi.odemeTarihi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEPARABIRIMI], bilgi.odemeParaBirimi) + ", ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], bilgi.durum);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Faturas the detay kaydet.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns></returns>
        public Sonuc FaturaDetayKaydet(FaturaDetay bilgi)
        {
            string sql = "INSERT INTO " + TABLOADDETAY + "(" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES(";

            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.faturaKod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.miktar) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kdvOran) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.birimFiyat) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.kdvTutar) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tutar) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.hesapKod) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Faturas the detay sil.
        /// </summary>
        /// <param name="faturaKod">The fatura kod.</param>
        /// <returns></returns>
        public Sonuc FaturaDetaySil(int faturaKod)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOADDETAY, ALANLARDETAY[(int)ENUMAlanlarDetay.FATURAKOD], "=", faturaKod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc FaturaAciklamaKaydet(FaturaAciklama bilgi)
        {
            string sql = "INSERT INTO " + TABLOADACIKLAMA + "(" + VeriErisim.TumAlanlar("", ALANLARACIKLAMA) + ") VALUES(";
            sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.tur) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi.aciklama) + ") ";

            return KomutCalistir(sql, 1);
        }

        public Sonuc FaturaAciklamaGuncelle(FaturaAciklama bilgi)
        {
            string sql = "UPDATE " + TABLOADACIKLAMA + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.ACIKLAMA], bilgi.aciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public IDataReader FaturaAciklamaListele(FaturaAciklama bilgi)
        {
            string sql = "";
            string kosul = "";
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADACIKLAMA, ALANLARACIKLAMA);
            sql += " FROM " + TABLOADACIKLAMA;

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD], "=", bilgi.kod));

            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.TUR], "=", bilgi.tur));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY ACIKLAMA";

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc FaturaAciklamaSil(FaturaAciklama bilgi)
        {
            string sql = "DELETE FROM " + TABLOADACIKLAMA;
            string kosul = "";

            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD], "=", bilgi.kod));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            return KomutCalistir(sql, 1);
        }

        public Sonuc MIFBelgeNoGuncelle(Fatura bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            if (bilgi.tahakkukBelgeNo != "")
            {
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAHAKKUKHESAPKOD], bilgi.tahakkukHesapKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAHAKKUKBELGENO], bilgi.tahakkukBelgeNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMFaturaDurum.TAHAKKUK);// + ",";
                //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], bilgi.islemYapan) + ", ";
                //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], bilgi.islemTarihi);
            }
            if (bilgi.odemeBelgeNo != "")
            {
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEHESAPKOD], bilgi.odemeHesapKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMEBELGENO], bilgi.odemeBelgeNo) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ODEMETARIHI], bilgi.odemeTarihi) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)ENUMFaturaDurum.ODENDI);// + ", ";
                //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMYAPAN], bilgi.islemYapan) + ", ";
                //sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ISLEMTARIHI], bilgi.islemTarihi);
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        public int EnBuyukAciklamaKod()
        {
            string sql = "";
            sql = "SELECT MAX(" + ALANLARACIKLAMA[(int)ENUMAlanlarAciklama.KOD] + ")";
            sql += " FROM " + TABLOADACIKLAMA;

            int deger = veriErisim.TekKolonSQLInt(sql);
            return deger + 1;
        }

        public Sonuc GuncelleMuhasebeAciklama(Fatura bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.MUHBELGEACIKLAMA], bilgi.muhBelgeAciklama);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }
    }
}
