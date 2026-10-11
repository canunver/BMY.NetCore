using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.BKD;

namespace TNS.BKD.IsKurallari.Veri
{
    /// <summary>
    /// Alt faaliyet bilgilerini tutan ABSPEBKOD veri tabanı tablosu, yatırım bağını tutan YATBUTCEKAYNAKPEBKOD  ve performans bağını tutan PEBFAALIYETPROJEPEBFON tabloları ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABPRGKODYAPI : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPRGKODYAPI(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PRGKODYAPI";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {

            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// PRGTUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KODTUR = 1,

            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 2,

            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 3,

            /// <summary>
            /// TUR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            TUR = 4,

            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 5,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = {
            "YIL"
            , "KODTUR"
            , "KOD"
            , "AD"
            , "TUR"
            , "FONKOD"
        };

        public static string TABLOADPEBILISKI = "PRGPEBILISKI";

        public enum ENUMAlanlarPEBIliski : int
        {
            YIL = 0,
            PEBKOD = 1,
            PRGKOD = 2,
            ALTPRGKOD = 3,
            FAALKOD = 4,
            ALTFAALKOD = 5,
            FONKOD = 6,
        }

        public static string[] ALANLARPEBILISKI = {
            "YIL"
            , "PEBKOD"
            , "PRGKOD"
            , "ALTPRGKOD"
            , "FAALKOD"
            , "ALTFAALKOD"
            , "FONKOD"
        };

        /// <summary>
        /// PRGKODYAPI yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Eklenecek kayıt bilgisini tutan, AltFaaliyet türünde nesne</param>
        /// <returns>Oluşturulan insert komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(PrgKodYapi bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(bilgi.yil);
            sql += "," + VeriErisim.DegerYap(bilgi.kodTur);
            sql += "," + VeriErisim.DegerYap(bilgi.kod);
            sql += "," + VeriErisim.DegerYap(bilgi.ad);
            sql += "," + VeriErisim.DegerYap(bilgi.tur);
            sql += "," + VeriErisim.DegerYap(bilgi.fonKod);
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc Guncelle(PrgKodYapi bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.FONKOD], bilgi.fonKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.TUR], bilgi.tur);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", bilgi.kodTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);

        }

        public Sonuc GuncelleYil(PrgKodYapi bilgi)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.YIL], bilgi.yil);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", bilgi.kodTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// PRGKODYAPI yapısında duran alanlar ile verilmiş ise kod uzunluğunu kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Select edilecek kayıtlara ait kıstas bilgilerini tutan, PRGKODYAPI türünde nesne</param>
        /// <returns>Verilen kriterlere göre oluşturulan select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Listele(PrgKodYapi bilgi)
        {
            string kosul = "";

            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;

            if (bilgi.yil > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            if (bilgi.kodTur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", bilgi.kodTur));
            if (bilgi.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            if (bilgi.tur > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.TUR], "=", bilgi.tur));
            if (bilgi.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + bilgi.ad + "%"));

            if (bilgi.sorguKriter != "")
            {
                bilgi.sorguKriter = bilgi.sorguKriter.Replace(".", "");
                string[] aranan = bilgi.sorguKriter.Split(' ', ';');

                foreach (string ara in aranan)
                {
                    if (string.IsNullOrEmpty(ara.Trim()))
                        continue;

                    kosul = VeriErisim.KosulEkle(kosul, "AND (", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + ara.Replace(".", "").Trim() + "%"));
                    kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", OrtakFonksiyonlar.ConvertToInt(ara, 0)));
                    kosul += ")";
                }
            }

            if (!string.IsNullOrWhiteSpace(bilgi.ilgiliBirimKod))
            {
                string icKosul = "(1 In (SELECT 1 FROM PRGPEBILISKI I  INNER JOIN  ButceHazirlikv2.dbo.BHMTERTIPLISTESIGIDER T ON I.PEBKOD = t.PEBKOD"
                                 + " AND I.FAALKOD = PRGKODYAPI.KOD"
                                + $" WHERE T.YIL = PRGKODYAPI.YIL AND T.KURKOD LIKE {VeriErisim.DegerYap(bilgi.ilgiliBirimKod.Replace(".", "") + "%")}))";

                kosul = VeriErisim.KosulEkle(kosul, "AND", icKosul);
            }

            if (kosul != "")
                sql += " WHERE " + kosul;
            sql += " ORDER BY 1, 2, 3 ";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// PRGKODYAPI yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bilgi">Anahtar değişkenleri tutan PRGKODYAPI türünde nesne</param>
        /// <returns>Oluşturulan delete komutunun çalıştırılması sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(PrgKodYapi bilgi)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KODTUR], "=", bilgi.kodTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            return KomutCalistir(sql, 0);
        }


        public Sonuc EklePEBIliski(PrgPebIliski bilgi)
        {
            string sql = "INSERT INTO " + TABLOADPEBILISKI + "(" + VeriErisim.TumAlanlar("", ALANLARPEBILISKI) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(bilgi.yil);
            sql += "," + VeriErisim.DegerYap(bilgi.pebKod.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(bilgi.prgKod);
            sql += "," + VeriErisim.DegerYap(bilgi.altPrgKod);
            sql += "," + VeriErisim.DegerYap(bilgi.faalKod);
            sql += "," + VeriErisim.DegerYap(bilgi.altFaalKod);
            sql += "," + VeriErisim.DegerYap(bilgi.fonKod.Replace(".", ""));
            sql += ")";

            return KomutCalistir(sql, 1);
        }

        public Sonuc GuncellePEBIliski(PrgPebIliski bilgi)
        {
            string sql = "UPDATE " + TABLOADPEBILISKI + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.PRGKOD], bilgi.prgKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.ALTPRGKOD], bilgi.altPrgKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.FAALKOD], bilgi.faalKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.ALTFAALKOD], bilgi.altFaalKod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.FONKOD], bilgi.fonKod.Replace(".", ""));

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));

            return KomutCalistir(sql, 1);
        }


        public IDataReader ListelePEBIliski(PrgPebIliski bilgi)
        {
            Tertip tUz = new Tertip(bilgi.yil, ENUMTabloKod.TABPEBOdenek);
            int pebKodUzunluk = tUz.Kirilimlar(4).Replace(".", "").Length;
            string sql = "select ABSPEBKOD.PEBKOD, ABSPEBKOD.PEBAD, ";
            sql += "PRGPEBILISKI.ALTFAALKOD, ALTFAAL.AD, PRGPEBILISKI.PRGKOD, PRG.AD, PRGPEBILISKI.ALTPRGKOD, ALTPRG.AD, PRGPEBILISKI.FAALKOD, FAAL.AD, PRGPEBILISKI.FONKOD, FAAL.FONKOD ";
            sql += "from ABSPEBKOD ";
            sql += "left outer join PRGPEBILISKI ON PRGPEBILISKI.YIL = ABSPEBKOD.YIL AND PRGPEBILISKI.PEBKOD = ABSPEBKOD.PEBKOD ";
            sql += "LEFT OUTER JOIN PRGKODYAPI AS ALTFAAL ON ALTFAAL.YIL = PRGPEBILISKI.YIL AND ALTFAAL.KOD = PRGPEBILISKI.ALTFAALKOD AND ALTFAAL.KODTUR = 4 ";
            sql += "LEFT OUTER JOIN PRGKODYAPI AS PRG ON PRG.YIL = PRGPEBILISKI.YIL AND PRG.KOD = PRGPEBILISKI.PRGKOD AND PRG.KODTUR = 1 ";
            sql += "LEFT OUTER JOIN PRGKODYAPI AS ALTPRG ON ALTPRG.YIL = PRGPEBILISKI.YIL AND ALTPRG.KOD = PRGPEBILISKI.ALTPRGKOD AND ALTPRG.KODTUR = 2 ";
            sql += "LEFT OUTER JOIN PRGKODYAPI AS FAAL ON FAAL.YIL = PRGPEBILISKI.YIL AND FAAL.KOD = PRGPEBILISKI.FAALKOD AND FAAL.KODTUR = 3 ";
            sql += $"where ABSPEBKOD.YIL = {bilgi.yil} AND LEN(ABSPEBKOD.PEBKOD)= {pebKodUzunluk} ";

            if (bilgi.pebKod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ABSPEBKOD", "PEBKOD", "LIKE", bilgi.pebKod.Replace(".", "") + "%"));
            if (bilgi.prgKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPEBILISKI, ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.PRGKOD], "=", bilgi.prgKod));
            if (bilgi.altPrgKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPEBILISKI, ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.ALTPRGKOD], "=", bilgi.altPrgKod));
            if (bilgi.faalKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPEBILISKI, ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.FAALKOD], "=", bilgi.faalKod));
            if (bilgi.altFaalKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOADPEBILISKI, ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.ALTFAALKOD], "=", bilgi.altFaalKod));
            if (!string.IsNullOrWhiteSpace(bilgi.araMetin))
            {
                string ilkKelime = bilgi.araMetin.Split(' ')[0];
                int ilkDeger = OrtakFonksiyonlar.ConvertToInt(ilkKelime, 0);


                string k1 = VeriErisim.KosulOl("ABSPEBKOD", "PEBAD", "LIKE", "%" + bilgi.araMetin + "%");
                string k2 = VeriErisim.KosulOl("ABSPEBKOD", "PEBKOD", "LIKE", ilkKelime.Replace(".", "") + "%");
                string k3 = VeriErisim.KosulOl("ALTFAAL", "AD", "LIKE", "%" + bilgi.araMetin + "%");

                string k4 = VeriErisim.KosulOl("PRGPEBILISKI", "FONKOD", "LIKE", ilkKelime.Replace(".", "") + "%");

                sql = VeriErisim.KosulEkle(sql, "AND", "(" + k1 + " OR " + k2 + " OR " + k3 + " OR " + k4);

                if (ilkDeger > 0)
                {
                    k1 = VeriErisim.KosulOl("ALTFAAL", "KOD", "=", ilkDeger);
                    k2 = VeriErisim.KosulOl("FAAL", "KOD", "=", ilkDeger);
                    k3 = VeriErisim.KosulOl("PRG", "KOD", "=", ilkDeger);
                    k4 = VeriErisim.KosulOl("ALTPRG", "KOD", "=", ilkDeger);
                    sql = VeriErisim.KosulEkle(sql, "OR", k1 + " OR " + k2 + " OR " + k3 + " OR " + k4);
                }

                sql += ")";
            }

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc KopyalaIliski(int yil)
        {
            string sql = $"INSERT INTO PRGPEBILISKI SELECT {VeriErisim.DegerYap(yil)} YIL, PEBKOD, PRGKOD, ALTPRGKOD, FAALKOD, ALTFAALKOD, FONKOD from PRGPEBILISKI DIS where YIL={VeriErisim.DegerYap(yil - 1)} and 1 NOT IN (SELECT 1 FROM PRGPEBILISKI IC WHERE IC.YIL=DIS.YIL+1 AND IC.PEBKOD=DIS.PEBKOD)";
            return KomutCalistir(sql, 0);
        }

        public Sonuc KopyalaPEB(int yil)
        {
            string sql = $"INSERT INTO ABSPEBKOD(YIL, PEBKOD, PEBAD) SELECT {VeriErisim.DegerYap(yil)} YIL, PEBKOD, PEBAD from ABSPEBKOD DIS where YIL={VeriErisim.DegerYap(yil - 1)} and 1 NOT IN (SELECT 1 FROM ABSPEBKOD IC WHERE IC.YIL=DIS.YIL+1 AND IC.PEBKOD=DIS.PEBKOD)";
            return KomutCalistir(sql, 0);
        }

        public Sonuc KopyalaProg(int yil)
        {
            string sql = $"INSERT INTO PRGKODYAPI SELECT {VeriErisim.DegerYap(yil)} YIL, KODTUR, KOD, AD, TUR, FONKOD from PRGKODYAPI DIS where YIL={VeriErisim.DegerYap(yil - 1)} and 1 NOT IN (SELECT 1 FROM PRGKODYAPI IC WHERE IC.YIL=DIS.YIL+1 AND IC.KODTUR=DIS.KODTUR AND IC.KOD=DIS.KOD)";
            return KomutCalistir(sql, 0);
        }

        public IDataReader ListeleIliskisiOlmayanAltFaal(PrgPebIliski bilgi)
        {
            string sql = "select ALTFAAL.KOD, ALTFAAL.AD ";

            sql += "FROM PRGKODYAPI AS ALTFAAL ";
            sql += $"where ALTFAAL.YIL = {bilgi.yil} AND ALTFAAL.KODTUR = 4  ";
            sql += $"AND 1 NOT IN (SELECT 1 FROM PRGPEBILISKI where PRGPEBILISKI.YIL=ALTFAAL.YIL AND PRGPEBILISKI.ALTFAALKOD=ALTFAAL.KOD) ";

            if (bilgi.altFaalKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ALTFAAL", "KOD", "=", bilgi.altFaalKod));
            if (string.IsNullOrWhiteSpace(bilgi.araMetin))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ALTFAAL", "AD", "LIKE", "%" + bilgi.araMetin + "%"));

            return veriErisim.DataReaderDondur(sql);
        }

        public Sonuc SilPEBIliski(PrgPebIliski bilgi)
        {
            string sql = "DELETE FROM " + TABLOADPEBILISKI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.YIL], "=", bilgi.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARPEBILISKI[(int)ENUMAlanlarPEBIliski.PEBKOD], "=", bilgi.pebKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }
    }
}