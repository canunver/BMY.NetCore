using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProjeFaaliyet : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProjeFaaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJEFAALIYET";
        public static string[] ALANLAR = {
                "KOD",
                "PROJEKOD",
                "AD",
                "YIL",
                "BASTARIH",
                "BITTARIH",
                "ANAFAALIYET",
                "SORUMLUBIRIMKOD",
                "SORUMLUMUHASEBEKOD",
                "SORUMLUKURKOD",
                "IZLEYENKOD",
                "IZLEYENAD",
                "TAHMINIMALIYET"
        };

        public enum ENUMAlanlar
        {
            KOD = 0,
            PROJEKOD = 1,
            AD = 2,
            YIL = 3,
            BASTARIH = 4,
            BITTARIH = 5,
            ANAFAALIYET = 6,
            SORUMLUBIRIMKOD = 7,
            SORUMLUMUHASEBEKOD = 8,
            SORUMLUKURKOD = 9,
            IZLEYENKOD = 10,
            IZLEYENAD = 11,
            TAHMINIMALIYET = 12,
        }

        public static string TABLOADGERC = "BHMPROJEFAALIYETGERC";
        public enum ENUMAlanlarGerc
        {
            FAALKOD = 0,
            YIL = 1,
            AY = 2,
            NAKDIGERCEKLESME = 3,
            ACIKLAMA = 4,
            KAYDEDENKULKOD = 5,
            KAYITTARIHI = 6,
        }

        public static string[] ALANLARGERC = {
            "FAALKOD",
            "YIL",
            "AY",
            "NAKDIGERCEKLESME",
            "ACIKLAMA",
            "KAYDEDENKULKOD",
            "KAYITTARIHI",
        };

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeFaaliyet form)
        {
            string sql;

            if (form.kod == "")
            {
                form.kod = Guid.NewGuid().ToString();
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

                sql += VeriErisim.DegerYap(form.kod) + ", ";
                sql += VeriErisim.DegerYap(form.projeKod) + ", ";
                sql += VeriErisim.DegerYap(form.ad) + ", ";
                sql += VeriErisim.DegerYap(form.yil) + ", ";
                sql += VeriErisim.DegerYap(form.basTarih) + ", ";
                sql += VeriErisim.DegerYap(form.bitTarih) + ", ";
                sql += VeriErisim.DegerYap(form.anaFaaliyet) + ", ";
                sql += VeriErisim.DegerYap(form.sorumluBirim.birimKod) + ", ";
                sql += VeriErisim.DegerYap(form.sorumluBirim.muhasebeKod) + ", ";
                sql += VeriErisim.DegerYap(form.sorumluBirim.kurKod) + ", ";
                sql += VeriErisim.DegerYap(form.izleyenKod) + ", ";
                sql += VeriErisim.DegerYap(form.izleyenAd) + ", ";
                sql += VeriErisim.DegerYap(form.tahminiMaliyet) + ") ";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], form.yil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASTARIH], form.basTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITTARIH], form.bitTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANAFAALIYET], form.anaFaaliyet) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TAHMINIMALIYET], form.tahminiMaliyet) + ",";

                if (form.sorumluBirim.birimKod != "")
                {
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD], form.sorumluBirim.birimKod) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD], form.sorumluBirim.muhasebeKod) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUKURKOD], form.sorumluBirim.kurKod) + ",";
                }

                if (form.izleyenKod != "")
                {
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZLEYENKOD], form.izleyenKod) + ",";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.IZLEYENAD], form.izleyenAd) + ",";
                }

                sql = sql.TrimEnd(',');

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public IDataReader Listele(ProjeFaaliyet form, string kullaniciKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", F.AD, B.BIRIMAD, P.AD FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN " + TABProje.TABLOAD + " P ON P.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PROJEKOD];
            sql += " LEFT OUTER JOIN " + TABProjeAnaFaaliyet.TABLOAD + " F ON F.KOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.ANAFAALIYET];

            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM AS B ON B.MUHASEBEKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD];
            sql += " AND B.BIRIMKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD];
            sql += " AND B.KURKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SORUMLUKURKOD];

            string kosul = "";

            if (!string.IsNullOrWhiteSpace(form.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else
            {
                if (!string.IsNullOrWhiteSpace(form.projeKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

                if (!string.IsNullOrWhiteSpace(form.anaFaaliyet))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.ANAFAALIYET], "=", form.anaFaaliyet));

                if (!string.IsNullOrWhiteSpace(form.aramaAlani))
                {
                    string icKosul = "";
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "" + form.aramaAlani + "%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("P", "AD", "LIKE", "" + form.aramaAlani + "%"));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
                }

                if (!string.IsNullOrWhiteSpace(form.sorumluBirim.deger))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD], "=", form.sorumluBirim.muhasebeKod));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD], "=", form.sorumluBirim.birimKod));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUKURKOD], "=", form.sorumluBirim.kurKod));
                }

                if (form.kullaniciFaaliyetleri)
                {
                    //kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.IZLEYENKOD], "=", kullaniciKod));
                }
            }

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader GerceklesmeListele(int yil, int ay, string faaliyetId)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar(TABLOADGERC, ALANLARGERC)}, {TABLOAD}.IZLEYENKOD, {TABLOAD}.SORUMLUBIRIMKOD, {TABLOAD}.SORUMLUMUHASEBEKOD, {TABLOAD}.SORUMLUKURKOD, {TABLOAD}.TAHMINIMALIYET FROM {TABLOADGERC} RIGHT OUTER JOIN {TABLOAD} ON {TABLOAD}.KOD = {TABLOADGERC}.FAALKOD";

            string kosul = "";
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGERC, ALANLARGERC[(int)ENUMAlanlarGerc.FAALKOD], "=", faaliyetId));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGERC, ALANLARGERC[(int)ENUMAlanlarGerc.YIL], "=", yil));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOADGERC, ALANLARGERC[(int)ENUMAlanlarGerc.AY], "=", ay));

            sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);
            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc Sil(ProjeFaaliyet form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PROJEKOD], "=", form.projeKod));

            if (!string.IsNullOrWhiteSpace(form.kod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));

            return KomutCalistir(sql, 0);
        }

        public Sonuc GerceklesmeEkle(ProjeFaaliyetGerceklesme gerc, string kullaniciKod)
        {
            string sql = "INSERT INTO " + TABLOADGERC + "(" + VeriErisim.TumAlanlar("", ALANLARGERC) + ") VALUES (";

            sql += VeriErisim.DegerYap(gerc.kod) + ", ";
            sql += VeriErisim.DegerYap(gerc.yil) + ", ";
            sql += VeriErisim.DegerYap(gerc.ay) + ", ";
            sql += VeriErisim.DegerYap(gerc.gerceklesenDeger) + ", ";
            sql += VeriErisim.DegerYap(gerc.aciklama) + ", ";
            sql += VeriErisim.DegerYap(kullaniciKod) + ", ";
            sql += VeriErisim.DegerYap(new TNSDateTime(DateTime.Now)) + ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GerceklesmeGuncelle(ProjeFaaliyetGerceklesme gerc, string kullaniciKod)
        {
            string sql;
            sql = "UPDATE " + TABLOADGERC + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARGERC[(int)ENUMAlanlarGerc.NAKDIGERCEKLESME], gerc.gerceklesenDeger) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARGERC[(int)ENUMAlanlarGerc.ACIKLAMA], gerc.aciklama) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARGERC[(int)ENUMAlanlarGerc.KAYITTARIHI], new TNSDateTime(DateTime.Now)) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARGERC[(int)ENUMAlanlarGerc.KAYDEDENKULKOD], kullaniciKod);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGERC[(int)ENUMAlanlarGerc.FAALKOD], "=", gerc.kod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERC[(int)ENUMAlanlarGerc.YIL], "=", gerc.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGERC[(int)ENUMAlanlarGerc.AY], "=", gerc.ay));
            return KomutCalistir(sql, 1);
        }
    }
}
