using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.HRC;
namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// HRCSEYYARGOREVREVIZE veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABHRCSEYYARGOREVREVIZEFORM : TABOrtak
    {
        /// <summary>
        /// TABHRCSEYYARGOREVREVIZE sınıf için oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABHRCSEYYARGOREVREVIZEFORM(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken. Tablonun oluşturulma sql'i
        /// 
        /// CREATE TABLE HRCSEYYARGOREVREVIZEFORM
        ///     (YIL INT
        ///     , BELGETUR INT
        ///     , BELGENO INT
        ///     , ACIKLAMA VARCHAR(2000)
        ///     , TARIH DATETIME
        ///     , ISLEMTUR INT
        ///     , DURUM INT
        ///     PRIMARY KEY (YIL, BELGETUR, BELGENO)
        ///     )
        ///     
        /// ALTER TABLE HRCSEYYARGOREVREVIZEFORM
        /// ADD ONAYASUNANAD VARCHAR(250),
        /// ONAYASUNANUNVAN VARCHAR(250),
        /// ONAYLAYANAD VARCHAR(250),
        /// ONAYLAYANUNVAN VARCHAR(250)
        /// </summary>
        public static string TABLOAD = "HRCSEYYARGOREVREVIZEFORM";
        //public static string TABLOADDETAYTAVAN = "BHMTAVANGENEL";
        public static string TABLOADDETAYSEYYARGOREV = "HRCSEYYARGOREVPLANLANAN";
        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// BELGETUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGETUR = 1,

            /// <summary>
            /// BELGENO alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            BELGENO = 2,

            /// <summary>
            /// ACIKLAMA alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ACIKLAMA = 3,

            /// <summary>
            /// TARIH alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 4,

            /// <summary>
            /// ISLEMTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTUR = 5,

            /// <summary>
            /// ISLEMTUR alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DURUM = 6,

            ONAYASUNANAD = 7,
            ONAYASUNANUNVAN = 8,
            ONAYLAYANAD = 9,
            ONAYLAYANUNVAN = 10,
            TALEPDURUMx = 11,
            KAYDEDEN = 12,
            ONAYLAYAN = 13,
            ICMALNO = 14,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = {
            "YIL"
            , "BELGETUR"
            , "BELGENO"
            , "ACIKLAMA"
            , "TARIH"
            , "ISLEMTUR"
            , "DURUM"

            , "ONAYASUNANAD"
            , "ONAYASUNANUNVAN"
            , "ONAYLAYANAD"
            , "ONAYLAYANUNVAN"
            , "TALEPDURUM"
            , "KAYDEDEN"
            , "ONAYLAYAN"
            , "ICMALNO"
        };

        /// <summary>
        /// Form yapısında duran anahtar alanları kullanarak, ilgili kaydı veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Anahtar değişkenleri tutan, SeyyarGorevRevize yapısında nesne</param>
        /// <returns>Sil komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Sil(SeyyarGorevRevize form)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", form.belgeNo));
            return KomutCalistir(sql, 0);
        }

        public Sonuc SilDetaySeyyarGorev(SeyyarGorevRevize form)
        {
            string sql = "DELETE FROM " + TABLOADDETAYSEYYARGOREV;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "YIL", "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", "BELGENO", "=", form.belgeNo));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Form yapısında duran alanları veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="form">Eklenecek kayıt bilgisini tutan, SeyyarGorevRevize yapısında nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucu Sonuc yapısında döndürür</returns>
        public Sonuc Ekle(SeyyarGorevRevize form, string kullanan)
        {
            string sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap((int)form.belgeTur);
            sql += "," + VeriErisim.DegerYap(form.belgeNo);
            sql += "," + VeriErisim.DegerYap(form.aciklama);
            sql += "," + VeriErisim.DegerYap(form.tarih);
            sql += "," + VeriErisim.DegerYap((int)form.altIslemTur);
            sql += "," + VeriErisim.DegerYap((int)form.durum);
            sql += "," + VeriErisim.DegerYap(form.onayaSunanAdSoyad);
            sql += "," + VeriErisim.DegerYap(form.onayaSunanUnvan);
            sql += "," + VeriErisim.DegerYap(form.onaylayanAdSoyad);
            sql += "," + VeriErisim.DegerYap(form.onaylayanUnvan);
            sql += "," + VeriErisim.DegerYap(0);
            sql += "," + VeriErisim.DegerYap(kullanan);
            sql += "," + VeriErisim.DegerYap("");
            sql += "," + VeriErisim.DegerYap(form.icmalNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        public Sonuc BelgeOnayla(SeyyarGorevRevize form, string kullanici, int icmalNo)
        {
            string sql = "UPDATE " + TABLOAD;
            sql += " SET " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DURUM], (int)form.durum);
            if (form.durum == ENUMRevizeFormDurum.KarsilananTalep)
                sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ICMALNO], icmalNo);
            sql += ", " + VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ONAYLAYAN], kullanici);

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", form.belgeNo));

            return KomutCalistir(sql, 1);
        }

        public Sonuc EkleDetaySeyyarGorev(SeyyarGorevRevize form, string harcamaBirimi, int istihdamSekli, int gunSayisi)
        {
            string sql = "INSERT INTO " + TABLOADDETAYSEYYARGOREV + "(YIL,HARCAMABIRIMI,ASAMA,ISTIHDAMSEKLI,GUNSAYISI,BELGENO) VALUES";
            sql += "(" + VeriErisim.DegerYap(form.yil);
            sql += "," + VeriErisim.DegerYap(harcamaBirimi.Replace(".", ""));
            sql += "," + VeriErisim.DegerYap(3);
            sql += "," + VeriErisim.DegerYap(istihdamSekli);
            sql += "," + VeriErisim.DegerYap(gunSayisi);
            sql += "," + VeriErisim.DegerYap(form.belgeNo);
            sql += ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Form yapısında duran alanları kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren fonksiyon
        /// </summary>
        /// <param name="form">Select edilecek kayıtlara ait kıstas bilgisini tutan, SeyyarGorevRevize yapısında nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucu elde edilen DataReader nesnesini döndürür</returns>
        public IDataReader Oku(SeyyarGorevRevize form)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGENO], "=", form.belgeNo));
            return veriErisim.DataReaderDondur(sql);
        }

        public int YeniBelgeNoBul(SeyyarGorevRevize form)
        {
            string sql = "SELECT MAX(BELGENO) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            double maks = veriErisim.TekKolonSQLDbl(sql);
            maks++;
            return Convert.ToInt32(maks);
        }

        public int YeniIcmalNoBul(SeyyarGorevRevize form)
        {
            string sql = "SELECT MAX(ICMALNO) FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BELGETUR], "=", (int)form.belgeTur));
            double maks = veriErisim.TekKolonSQLDbl(sql);
            maks++;
            return Convert.ToInt32(maks);
        }

        public IDataReader FormBirimleriListele(int yil, int belgeNo)
        {
            return veriErisim.DataReaderDondur(string.Format("SELECT DISTINCT BHMKURUMSALYAPI.KOD FROM HRCSEYYARGOREVPLANLANAN INNER JOIN BHMKURUMSALYAPI ON HRCSEYYARGOREVPLANLANAN.HARCAMABIRIMI = BHMKURUMSALYAPI.KOD WHERE (HRCSEYYARGOREVPLANLANAN.YIL = {0}) AND (HRCSEYYARGOREVPLANLANAN.BELGENO = {1})", yil, belgeNo));
        }

        public IDataReader FormIlleriListele(int yil, string belgeNo, TNS.BKD.ENUMTavanTur tavanTur)
        {
            return veriErisim.DataReaderDondur(string.Format("SELECT DISTINCT KOD1 FROM BHMTAVANGENEL WHERE (YIL = {0}) AND (ASAMA = {1}) AND (TAVANTUR={2}) AND (KOD2 = {3})", yil, VeriErisim.DegerYap("0300"), (int)tavanTur, VeriErisim.DegerYap(belgeNo)));
        }

        static string FormKosulOl(SeyyarGorevRevize form)
        {
            string sql = VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.YIL], "=", form.yil);

            if (form.belgeNoBas > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.BELGENO], ">=", form.belgeNoBas));

            if (form.belgeNoBit > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.BELGENO], "<=", form.belgeNoBit));

            if (form.icmalNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.ICMALNO], "<=", form.icmalNo));

            if (form.belgeNoBas <= 0 || form.belgeNoBas != form.belgeNoBit)
            {
                if (!TNSDateTime.IsNull(form.tarihBas))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.TARIH], ">=", form.tarihBas));

                if (!TNSDateTime.IsNull(form.tarihBit))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.TARIH], "<=", form.tarihBit));

                if (!string.IsNullOrWhiteSpace(form.aciklama))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.ACIKLAMA], "LIKE", "%" + form.aciklama + "%"));

                if (form.durum != ENUMRevizeFormDurum.Bos)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.DURUM], "=", (int)form.durum));
                else
                {
                    sql = VeriErisim.KosulEkle(sql, "AND", "A.DURUM IN (1, 4, 5, 91)");
                }

                if (form.altIslemTur != ENUMSeyyarGorevRevizeAltIslemTur.Bos)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.ISLEMTUR], "=", (int)form.altIslemTur));

                if (form.birimler != null && form.birimler.Count > 0)
                {
                    string icSql = "SELECT 1 FROM " + TABLOADDETAYSEYYARGOREV + " B ";
                    icSql = VeriErisim.KosulEkle(icSql, "WHERE", "B.YIL = A.YIL");
                    icSql = VeriErisim.KosulEkle(icSql, "AND", "B.BELGENO = A.BELGENO");
                    icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("B", "ASAMA", "=", 3));
                    icSql = VeriErisim.KosulEkle(icSql, "AND", BirimKosulOl(form.birimler));
                    sql = VeriErisim.KosulEkle(sql, "AND", "(1 IN (" + icSql + "))");
                }
                else if (!string.IsNullOrWhiteSpace(form.plakaKod))
                {
                    string icSql;

                    icSql = "SELECT 1 FROM " + TABLOADDETAYSEYYARGOREV + " B";
                    icSql = VeriErisim.KosulEkle(icSql, "WHERE", "B.YIL = A.YIL");
                    icSql = VeriErisim.KosulEkle(icSql, "AND", "B.BELGENO = A.BELGENO");
                    icSql = VeriErisim.KosulEkle(icSql, "AND", VeriErisim.KosulOl("B", "ASAMA", "=", 3));

                    string ilKosulSql;
                    if (form.plakaKod == "99")
                    {
                        ilKosulSql = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("B", "HARCAMABIRIMI", "LIKE", "900%"));
                    }
                    else
                    {
                        ilKosulSql = VeriErisim.KosulEkle("", "", VeriErisim.KosulOl("B", "HARCAMABIRIMI", "LIKE", "_" + form.plakaKod + "%"));
                        if (form.plakaKod == "06")
                        {
                            ilKosulSql = VeriErisim.KosulEkle(ilKosulSql, "OR", VeriErisim.KosulOl("B", "HARCAMABIRIMI", "LIKE", "100%"));
                            ilKosulSql = VeriErisim.KosulEkle(ilKosulSql, "OR", VeriErisim.KosulOl("B", "HARCAMABIRIMI", "LIKE", "300%"));
                        }
                    }
                    ilKosulSql = "(" + ilKosulSql + ")";

                    icSql = VeriErisim.KosulEkle(icSql, "AND", ilKosulSql);
                    sql = VeriErisim.KosulEkle(sql, "AND", "(1 IN (" + icSql + "))");
                }
            }
            return sql;
        }

        public IDataReader Listele(SeyyarGorevRevize form)
        {
            string sql = $"SELECT {VeriErisim.TumAlanlar("", ALANLAR)}, {EklenenKosulOl("A", ENUMPersonelIstihdamSekli.MEMUR, true)}, {EklenenKosulOl("A", ENUMPersonelIstihdamSekli.ISCI, true)}, {EklenenKosulOl("A", ENUMPersonelIstihdamSekli.SOZLESMELI, true)} FROM {TABLOAD} A WHERE {FormKosulOl(form)}";
            return veriErisim.DataReaderDondur(sql);
        }

        private string EklenenKosulOl(string anaTabloKod, ENUMPersonelIstihdamSekli ishSekli, bool sifirdanBuyuk)
        {
            return $"(SELECT SUM(GUNSAYISI) FROM HRCSEYYARGOREVPLANLANAN IC WHERE IC.YIL={anaTabloKod}.YIL AND IC.BELGENO={anaTabloKod}.BELGENO AND IC.ISTIHDAMSEKLI={(int)ishSekli} AND GUNSAYISI {(sifirdanBuyuk ? ">" : "<")} 0)";
        }

        public static string ListeleDetaySql(SeyyarGorevRevize form, int istSekli)
        {
            string sql = "SELECT P.HARCAMABIRIMI, SUM(P.GUNSAYISI) FROM HRCSEYYARGOREVPLANLANAN P LEFT OUTER JOIN HRCSEYYARGOREVREVIZEFORM A ON " + VeriErisim.Join("A", "P", new string[] { "YIL", "BELGENO" }) + " AND A.BELGETUR=" + VeriErisim.DegerYap((int)ENUMRevizeFormBelgeTur.DetayRevize) + " WHERE " + FormKosulOl(form);
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", "ISTIHDAMSEKLI", "=", istSekli));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", "ASAMA", "=", 3));
            sql += " GROUP BY P.HARCAMABIRIMI ORDER BY 1";
            return sql;
        }

        private static string BirimKosulOl(TNSCollection birimler)
        {
            string donDeger = "";
            foreach (string item in birimler)
            {
                string icSql = VeriErisim.KosulOl("B", "HARCAMABIRIMI", "=", item);
                donDeger = VeriErisim.KosulEkle(donDeger, "OR", icSql);
            }
            return "(" + donDeger + ")";
        }

        public void SeyyarKalanBul(BKD.TavanGenel tg, SeyyarGorevRevize form, out double kalanMemur, out double kalanIsci, out double kalanSozlesmeli)
        {
            string sql = "SELECT P.ISTIHDAMSEKLI, SUM(P.GUNSAYISI) FROM (select * from HRCSEYYARGOREVPLANLANAN where YIL = " + VeriErisim.DegerYap(form.yil) + " AND ASAMA = 3) P LEFT OUTER JOIN (SELECT * FROM HRCSEYYARGOREVREVIZEFORM where YIL = " + VeriErisim.DegerYap(form.yil) + " AND BELGETUR = 2) F";
            sql = VeriErisim.KosulEkle(sql, "ON", VeriErisim.Join("P", "F", new string[] { "YIL", "BELGENO" }));
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("P", "HARCAMABIRIMI", "=", tg.kod1.Replace(".", "")));
            if (form.belgeNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("P", "BELGENO", "<>", form.belgeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", "(P.BELGENO=0 OR (P.GUNSAYISI<0 AND F.DURUM<=5) OR (P.GUNSAYISI>0 AND F.DURUM=5)) group by P.ISTIHDAMSEKLI");

            IDataReader dr = this.veriErisim.DataReaderDondur(sql);
            kalanMemur = 0;
            kalanIsci = 0;
            kalanSozlesmeli = 0;
            if (dr != null)
            {
                while (dr.Read())
                {
                    int ist = VeriErisim.DegerAl(dr, 0, 0);
                    double kalan = VeriErisim.DegerAl(dr, 1, 0.0);
                    if (ist == 4)
                        kalanIsci = kalan;
                    else if (ist == 2)
                        kalanSozlesmeli = kalan;
                    else
                        kalanMemur = kalan;
                }
                dr.Close();
            }
            return;
        }
    }
}
