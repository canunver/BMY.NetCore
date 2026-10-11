using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Amortisman değerleme bilgilerini tutan TASAMORTISMANDEGERLEME veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAmortismanDegerleme : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAmortismanDegerleme(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASAMORTISMANDEGERLEME";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Programatik Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            PRSICILNO = 1,
            /// <summary>
            /// Değerleme miktarı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERLEMEMIKTAR = 2,
            /// <summary>
            /// Değerleme oranı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            DEGERLEMEORAN = 3,
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 4,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 5,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 6,
            /// <summary>
            /// Sicil no alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            SICILNO = 7,
            /// <summary>
            /// Hesap plan kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HESAPPLANKOD = 8,
            /// <summary>
            /// İşlem tarihi alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMTARIH = 9,
            /// <summary>
            /// İşlem yapan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ISLEMYAPAN = 10
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "PRSICILNO", "DEGERLEMEMIKTAR", "DEGERLEMEORAN", "MUHASEBEKOD",
                                   "HARCAMABIRIMKOD", "AMBARKOD", "SICILNO", "HESAPPLANKOD", "ISLEMTARIH", "ISLEMYAPAN" };

        /// <summary>
        /// Amortisman Değerleme bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ad">Amortisman Değerleme bilgilerini tutan, AmortismanDegerleme türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(AmortismanDegerleme ad)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(ad.yil) + ",";
            sql += VeriErisim.DegerYap(ad.prSicilNo) + ",";
            sql += VeriErisim.DegerYap(ad.degerlemeMiktar) + ",";
            sql += VeriErisim.DegerYap(ad.degerlemeOran) + ",";
            sql += VeriErisim.DegerYap(ad.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(ad.harcamaKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(ad.ambarKod) + ",";
            sql += VeriErisim.DegerYap(ad.gorSicilNo) + ",";
            sql += VeriErisim.DegerYap(ad.hesapPlanKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(ad.islemTarih) + ",";
            sql += VeriErisim.DegerYap(ad.islemYapan) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman Kriter bilgilerini kullanarak
        /// ilgili Amortisman Değerleme bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(AmortismanKriter ak)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ak.yil));

            sql = new TABAmortisman(veriErisim).KosulEkle(ak, sql, TABLOAD, "AND", false);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman Kriter bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Amortisman Değerleme Miktar bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <param name="yilIci">Değerleme miktarlarının yil değerine eşit olup olmayacağını tutan, Bool türünde nesne </param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortisman Değerleme miktar bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SicilDegerlemeMiktarListele(AmortismanKriter ak, bool yilIci)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.DEGERLEMEMIKTAR] + ")";
            sql += " FROM " + TABLOAD;

            if (yilIci)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ak.yil));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<", ak.yil));

            sql = new TABAmortisman(veriErisim).KosulEkle(ak, sql, TABLOAD, "AND", true);

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman Kriter bilgilerini kıstas olarak kullanarak,
        /// Yıl'a göre o yıl içerisinde, amortisman değerlemesi yapılıp yapılmadığı bilgisini dönen,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, yıla göre değerleme yapılmışsa true yapılmamışsa false değerini tutan, Bool türünde nesne döndürür</returns>
        public bool DegerlemeYapilmisMi(AmortismanKriter ak)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*)";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ak.yil));

            sql = new TABAmortisman(veriErisim).KosulEkle(ak, sql, TABLOAD, "AND", false);

            return veriErisim.TekKolonSQLInt(sql) > 0 ? true : false;
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman Kriter bilgilerini kullanarak,
        /// Birikmiş Amortisman değerlerini gösterilmesini sağlayan sql cümlesini oluşturan fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgisini tutan, AmortismanKriter türünde nesne</param>
        /// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda üretilen sql cümlesini tutan, String türünde nesne döndürür</returns>
        public static string SicilBirikmisDegerlemeIcSQLOlustur(AmortismanKriter ak, string tablo)
        {
            string sql = "(SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.DEGERLEMEMIKTAR] + ")";
            sql += " FROM " + TABLOAD;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " = " + tablo + ".PRSICILNO";
            return sql + ")";
        }

        //private string KosulEkle(AmortismanKriter ak, string sql, string baglac)
        //{
        //    string kosul = string.Empty;

        //    if (!string.IsNullOrEmpty(ak.muhasebeKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", ak.muhasebeKod));

        //    if (!string.IsNullOrEmpty(ak.harcamaKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", ak.harcamaKod.Replace(".", "")));

        //    if (!string.IsNullOrEmpty(ak.ambarKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", ak.ambarKod));

        //    if (!string.IsNullOrEmpty(ak.gorSicilNo))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SICILNO], "=", ak.gorSicilNo));

        //    if (!string.IsNullOrEmpty(ak.hesapPlanKod))
        //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.HESAPPLANKOD], "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));

        //    if (ak.prSicilNolar.Count > 0)
        //    {
        //        kosul += " AND (";
        //        for (int i = 0; i < ak.prSicilNolar.Count; i++)
        //        {
        //            if (i == 0)
        //                kosul += VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", (int)ak.prSicilNolar[i]);
        //            else
        //                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", (int)ak.prSicilNolar[i]));
        //        }
        //        kosul += ")";
        //    }
        //    else if (!string.IsNullOrEmpty(ak.belgeNo))
        //    {
        //        string icSQL = "(SELECT B.PRSICILNO FROM TASSICILNOHAREKET B";
        //        //icSQL += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = B.YIL";
        //        //icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = B.MUHASEBEKOD";
        //        //icSQL += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + " = B.HARCAMABIRIMKOD";
        //        icSQL = VeriErisim.KosulEkle(icSQL, "WHERE", VeriErisim.KosulOl("B", "YIL", "=", ak.belgeYil));
        //        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "MUHASEBEKOD", "=", ak.muhasebeKod));
        //        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
        //        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "FISNO", "=", ak.belgeNo));
        //        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "ISLEMTIPIKOD", "<", (int)ENUMZimmetIslemTipi.ZFVERME));
        //        icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL)) + ")";

        //        kosul = VeriErisim.KosulEkle(kosul, "AND", TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " IN " + icSQL);
        //    }

        //    if (!string.IsNullOrEmpty(kosul))
        //        sql = VeriErisim.KosulEkle(sql, baglac, kosul);

        //    return sql;
        //}
    }
}