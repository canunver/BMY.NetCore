using System;
using System.Data;
using TNS;
using TNS.YAT;
using TNS.YAT.IsKurallari.Veri;
using TNS.Veri;
using TNS.KYM;

namespace TNS.YAT.IsKurallari.Veri
{
    /// <summary>
    /// Yatırım modülü veri tabanı erişim sınıflarının ortak kullandığı yordamları tutan sınıf
    /// </summary>
    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen tablo alan adı ile değerleri tek tek "LIKE" ile birleştirip
        /// oluşan cümlecikleri de "OR" ile bağlayan ve döndüren yordam
        /// </summary>
        /// <param name="degerler">Tablo alan adına eşlenecek değerler</param>
        /// <param name="tAlanAd">Tablo alan adı</param>
        /// <returns>"OR" ile bağlanmış sql cümlesi koşul parçası döndürülür.</returns>
        public static string DegerleriOrla(string[] degerler, string tAlanAd, int kod)
        {
            if (kod > 0) return "";
            if (degerler == null || degerler.Length == 0) return "";
            string donusDeger = "";
            bool orlandiMi = false;
            foreach (string s in degerler)
            {
                if (s != "")
                {
                    if (donusDeger != "")
                    {
                        orlandiMi = true;
                        donusDeger += " OR ";
                    }

                    donusDeger += tAlanAd + " LIKE '" + s.Replace(".", "") + "%'";
                }
            }

            if (orlandiMi)
                donusDeger = "(" + donusDeger + ")";

            return donusDeger;
        }

        /// <summary>
        /// Verilen tablonun anahtar alanı için sayaç değeri döndüren yordam
        /// Tablonun anahtar alanındaki maksimum değeri bulup 1 ekler.
        /// </summary>
        /// <param name="ve">Veri tabanı erişiminde kullanılacak nesne</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="alan">Alan adı</param>
        /// <returns>Üretilen yeni sayaç değeri döndürülür.</returns>
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan)
        {
            return SayacDegerAl(ve, tablo, alan, new string[0], new string[0], new string[0], new int[0]);
        }

        /// <summary>
        /// Verilen tablonun anahtar alanı için sayaç değeri döndüren yordam
        /// Ek parametrelerde verilen kriterler bazında tablonun anahtar alanındaki maksimum değeri bulup 1 ekler.
        /// </summary>
        /// <param name="ve">Veri tabanı erişiminde kullanılacak nesne</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="alan">Alan adı</param>
        /// <param name="whereStrAlan">String tipinde veri tutan alan adları</param>
        /// <param name="whereStrDeger">String tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <param name="whereIntAlan">Sayı tipinde veri tutan alan adları</param>
        /// <param name="whereIntDeger">Sayı tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <returns>Üretilen yeni sayaç değeri döndürülür.</returns>
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT MAX(CAST(" + alan + " AS INT)) FROM " + tablo;

            for (int i = 0; i < whereStrAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereStrAlan[i], "=", whereStrDeger[i].Replace(".", "")));

            for (int i = 0; i < whereIntAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereIntAlan[i], "=", whereIntDeger[i]));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return OrtakFonksiyonlar.ConvertToInt(ve.TekKolonSQLDbl(sql).ToString(), 0) + 1;
        }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen kriterlere uygun olan kayıt var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="ve">Veri tabanı erişiminde kullanılacak nesne</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="whereStrAlan">String tipinde veri tutan alan adları</param>
        /// <param name="whereStrDeger">String tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <param name="whereIntAlan">Sayı tipinde veri tutan alan adları</param>
        /// <param name="whereIntDeger">Sayı tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public static bool KayitVarmi(VeriErisim ve, string tablo, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT COUNT(*) FROM " + tablo;
            for (int i = 0; i < whereStrAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereStrAlan[i], "=", whereStrDeger[i].Replace(".", "")));

            for (int i = 0; i < whereIntAlan.Length; i++)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", whereIntAlan[i], "=", whereIntDeger[i]));

            if (kosul != "")
                sql += " WHERE " + kosul;

            return ve.TekKolonSQLDbl(sql) != 0;
        }

        public IDataReader TanimSozlukListele(int sozlukTur, int yil, string senaryoKod)
        {
            //string sql = "SELECT " + AlanAd(sozlukTur) + ", AD FROM " + TabloAd(sozlukTur) + " order by 1";

            string sql = "SELECT SOZ.{0}, SOZ.AD, ALTTABLO.SAY ";
            sql += " FROM {1} AS SOZ LEFT OUTER JOIN";
            sql += " (SELECT {2}, COUNT(*) AS SAY FROM {3}";
            sql += " GROUP BY {2}) AS ALTTABLO ON SOZ.{0} = ALTTABLO.{2} ";
            if (yil > 0 && !string.IsNullOrWhiteSpace(senaryoKod))
            {
                sql += string.Format("WHERE 1 IN ( SELECT 1 FROM {0} ICALTTABLO WHERE ICALTTABLO.{1}=SOZ.{2} AND ICALTTABLO.YIL={3} AND ICALTTABLO.SENARYOKOD={4})", TabloAdDetay(sozlukTur), AlanDetayAd(sozlukTur), AlanAd(sozlukTur), VeriErisim.DegerYap(yil), VeriErisim.DegerYap(senaryoKod.Replace(".", "")));
            }
            sql += " ORDER BY 1";
            sql = String.Format(sql, AlanAd(sozlukTur), TabloAd(sozlukTur), AlanDetayAd(sozlukTur), TabloAdDetay(sozlukTur));

            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader TanimSozlukDetaySayisListele(int sozlukTur)
        //{
        //    string alanAd = AlanDetayAd(sozlukTur);
        //    string sql = "SELECT " + alanAd + ", COUNT(*) FROM " + TabloAdDetay(sozlukTur) + " group by " + alanAd + " order by 1";
        //    return veriErisim.DataReaderDondur(sql);
        //}

        public int SozlukSayacBul(int sozlukTur)
        {
            string sql = "SELECT MAX(" + AlanAd(sozlukTur) + ") FROM " + TabloAd(sozlukTur);
            double sonuc = veriErisim.TekKolonSQLDbl(sql);
            return Convert.ToInt32(sonuc) + 1;
        }

        public Sonuc SozlukEkle(int sozlukTur, Sozluk sozluk)
        {
            string sql = "INSERT INTO " + TabloAd(sozlukTur) + " VALUES(" + VeriErisim.DegerYap(sozluk.kod) + ", " + VeriErisim.DegerYap(sozluk.ad) + ")";
            return KomutCalistir(sql, 1);
        }

        private string TabloAdDetay(int sozlukTur)
        {
            switch (sozlukTur)
            {
                case 1: return "YATPROJE";
                case 2: return "YATALTPROJE";
                default: return "YATBUTCEKAYNAK";
            }
        }

        private string AlanDetayAd(int sozlukTur)
        {
            switch (sozlukTur)
            {
                case 1: return "PROJENO";
                case 2: return "ALTPROJENO";
                default: return "KAYNAKNO";
            }
        }

        private string TabloAd(int sozlukTur)
        {
            switch (sozlukTur)
            {
                case 1: return "YATSOZPROJE";
                case 2: return "YATSOZALTPROJE";
                default: return "YATSOZKAYNAK";
            }
        }

        private string AlanAd(int sozlukTur)
        {
            switch (sozlukTur)
            {
                case 1: return "PROJENO";
                case 2: return "ALTPROJENO";
                default: return "KAYNAKNO";
            }
        }

        /// <summary>
        /// CREATE TABLE YATDEFLATOR (YIL INT, SEKTORKOD VARCHAR(20), DEFLATOR DECIMAL(18,10), PRIMARY KEY (YIL, SEKTORKOD))
        /// </summary>
        /// <param name="butceYil"></param>
        /// <param name="sektorKod"></param>
        /// <returns></returns>

        public decimal DeflatorBul(int butceYil, string senaryoKod, string sektorKod)
        {
            string sql = "SELECT DEFLATOR FROM YATDEFLATOR WHERE YIL=" + VeriErisim.DegerYap(butceYil) + " AND SENARYOKOD = " + VeriErisim.DegerYap(senaryoKod.Replace(".", "")) + " AND SEKTORKOD=" + VeriErisim.DegerYap(sektorKod);

            return this.veriErisim.TekKolonSQLDecimal(sql);
        }
    }
}