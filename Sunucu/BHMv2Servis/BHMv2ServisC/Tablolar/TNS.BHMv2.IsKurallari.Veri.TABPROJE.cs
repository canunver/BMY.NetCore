using System;
using System.Data;
using TNS.Veri;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// 
    /// </summary>
    public class TABProje : TABOrtak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TABProje"/> class.
        /// </summary>
        /// <param name="veriErisim">The veri erisim.</param>
        public TABProje(VeriErisim veriErisim) : base(veriErisim) { }

        public static string TABLOAD = "BHMPROJE";
        public static string[] ALANLAR = {
                "KOD",
                "SORUMLUBIRIMKOD",
                "SORUMLUMUHASEBEKOD",
                "SORUMLUKURKOD",
                "AD",
                "SEKTORKOD",
                "BASLAMAYIL",
                "BITISYIL",
                "DURUM",
                "YATPROJEKOD",
                "YATALTPROJEKOD",
                "YATPROJEYIL",
                "SPHEDEFKOD"
        };

        public enum ENUMAlanlar
        {
            KOD = 0,
            SORUMLUBIRIMKOD = 1,
            SORUMLUMUHASEBEKOD = 2,
            SORUMLUKURKOD = 3,
            AD = 4,
            SEKTORKOD = 5,
            BASLAMAYIL = 6,
            BITISYIL = 7,
            DURUM = 8,
            YATPROJEKOD = 9,
            YATALTPROJEKOD = 10,
            YATPROJEYIL = 11,
            SPHEDEFKOD = 12
        }

        /// <summary>
        /// Kaydets the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <returns></returns>
        public Sonuc Kaydet(ProjeTeklif form)
        {
            string sql;

            if (form.kod == "")
            {
                form.kod = Guid.NewGuid().ToString();
                sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";

                sql += VeriErisim.DegerYap(form.kod) + ", ";
                sql += VeriErisim.DegerYap(form.sorumluBirim.birimKod) + ", ";
                sql += VeriErisim.DegerYap(form.sorumluBirim.muhasebeKod) + ", ";
                sql += VeriErisim.DegerYap(form.sorumluBirim.kurKod.Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(form.ad) + ", ";
                sql += VeriErisim.DegerYap(form.sektorKod.Replace(".", "")) + ", ";
                sql += VeriErisim.DegerYap(form.baslamaYil) + ", ";
                sql += VeriErisim.DegerYap(form.bitisYil);
                sql += ", " + VeriErisim.DegerYap((int)form.durum);
                sql += ", " + VeriErisim.DegerYap(form.yatProjeKod);
                sql += ", " + VeriErisim.DegerYap(form.yatAltProjeKod);
                sql += ", " + VeriErisim.DegerYap(form.yatProjeYil);
                sql += ", " + VeriErisim.DegerYap(form.spHedefKod);
                sql += ") ";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";

                if (form.sorumluBirim.birimKod != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD], form.sorumluBirim.birimKod) + ",";
                if (form.sorumluBirim.muhasebeKod != 0)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD], form.sorumluBirim.muhasebeKod) + ",";
                if (form.sorumluBirim.kurKod != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SORUMLUKURKOD], form.sorumluBirim.kurKod.Replace(".", "")) + ",";

                if (form.ad != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";

                if (form.sektorKod != "")
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SEKTORKOD], form.sektorKod.Replace(".", "")) + ",";

                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], form.baslamaYil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISYIL], form.bitisYil) + ",";
                if (form.durum != ENUMProjeDurum.TANIMSIZ)
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)form.durum) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SPHEDEFKOD], form.spHedefKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YATPROJEKOD], form.yatProjeKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YATPROJEYIL], form.yatProjeYil) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YATALTPROJEKOD], form.yatAltProjeKod);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Listeles the specified bilgi.
        /// </summary>
        /// <param name="form">The bilgi.</param>
        /// <param name="kullaniciBirimler">The bilgi.</param>
        /// <returns>Verilen form ve yetkili birimler kullanılarak elde edilen sql'in çalıştırılmasının sonucu olan dara reader döndürülür.</returns>
        public IDataReader Listele(ProjeTeklif form, string kullaniciBirimler)
        {
            string kosul = "";
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ", MUH.MUHASEBEAD, B.BIRIMAD, S.SEKTORAD, IL.ILADI, P.DPTPROJENO, P.AD, AP.AD FROM " + TABLOAD;

            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBEBIRIM AS B ON B.MUHASEBEKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD];
            sql += " AND B.BIRIMKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD];
            sql += " AND B.KURKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SORUMLUKURKOD];

            sql += " LEFT OUTER JOIN ButceIslemleri.dbo.MUHMUHASEBE AS MUH ON MUH.MUHASEBEKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD];

            sql += " LEFT OUTER JOIN ButceHazirlik.dbo.ABSSEKTOR AS S ON S.SEKTORKOD = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.SEKTORKOD];
            sql += " AND S.YIL = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.BASLAMAYIL];

            sql += " LEFT OUTER JOIN Yatirim.dbo.YATIL IL ON IL.ILKODU = (BHMPROJE.SORUMLUMUHASEBEKOD/1000) ";

            sql += " LEFT OUTER JOIN Yatirim.dbo.YATPROJE AS P ON P.YIL = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YATPROJEYIL];
            sql += " AND P.SENARYOKOD = '0600'";
            sql += " AND P.PROJENO = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YATPROJEKOD];

            sql += " LEFT OUTER JOIN Yatirim.dbo.YATALTPROJE AS AP ON AP.YIL = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YATPROJEYIL];
            sql += " AND AP.SENARYOKOD = '0600'";
            sql += " AND AP.ALTPROJENO = " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YATALTPROJEKOD];

            if (!string.IsNullOrWhiteSpace(form.kod))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", form.kod));
            else if (form.sadeceYATProje)
            {
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YATPROJEKOD], ">", (int)0));
            }
            else
            {
                string birimSql = "";
                string[] kulBirimDizi = kullaniciBirimler.Split(';');
                foreach (string birim in kulBirimDizi)
                {
                    if (string.IsNullOrWhiteSpace(birim)) continue;
                    string[] birimParcaDizi = birim.Split('-');
                    int sayKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(birimParcaDizi, 0, ""), 0);
                    int birKod = OrtakFonksiyonlar.ConvertToInt(OrtakFonksiyonlar.DiziElemanAl(birimParcaDizi, 1, ""), 0);
                    string kurKod = OrtakFonksiyonlar.DiziElemanAl(birimParcaDizi, 2, "");

                    string birimIcSql = "";
                    if (sayKod > 0)
                        birimIcSql = VeriErisim.KosulEkle(birimIcSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD], "=", sayKod));

                    if (birKod > 0)
                        birimIcSql = VeriErisim.KosulEkle(birimIcSql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD], "=", birKod.ToString("000")));
                    if (birimIcSql != "")
                        birimSql = VeriErisim.KosulEkle(birimSql, "OR", "(" + birimIcSql + ")");
                }

                if (birimSql != "")
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + birimSql + ")");

                if (!string.IsNullOrWhiteSpace(form.genelArama))
                {
                    string icKosul = VeriErisim.KosulEkle("", "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + form.genelArama + "%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", "IL.ILADI", "LIKE", "%" + form.genelArama + "%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", "P.AD", "LIKE", "%" + form.genelArama + "%"));
                    icKosul = VeriErisim.KosulEkle(icKosul, "OR", VeriErisim.KosulOl("", "AP.AD", "LIKE", "%" + form.genelArama + "%"));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", "(" + icKosul + ")");
                }

                if (form.yilAra > 0)
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BASLAMAYIL], "<=", form.yilAra));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.BITISYIL], ">=", form.yilAra));
                }

                if (!string.IsNullOrWhiteSpace(form.sektorKod))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SEKTORKOD], "=", form.sektorKod.Replace(".", "")));
                }

                if (!string.IsNullOrWhiteSpace(form.sorumluBirim.deger))
                {
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUMUHASEBEKOD], "=", form.sorumluBirim.muhasebeKod));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUBIRIMKOD], "=", form.sorumluBirim.birimKod));
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SORUMLUKURKOD], "=", form.sorumluBirim.kurKod));
                }
            }
            if (kosul != "")
                sql = VeriErisim.KosulEkle(sql, "WHERE", kosul);

            return veriErisim.DataReaderDondur(sql);
        }

        public int YatProjeSayiBul(ProjeTeklif form)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YATPROJEKOD], "=", form.yatProjeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YATALTPROJEKOD], "=", form.yatAltProjeKod));
            return veriErisim.TekKolonSQLInt(sql);
        }

        public Sonuc Sil(string kod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));

            return KomutCalistir(sql, 1);
        }

        public bool KayitVarMi(string projeKod)
        {
            string sql = "SELECT COUNT(*) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", projeKod));
            return veriErisim.TekKolonSQLInt(sql) == 1;
        }
    }
}
