using System;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Amortisman bilgilerini tutan TASAMORTISMAN veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>

    public class TABAmortisman : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAmortisman(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASAMORTISMAN";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            YIL = 1,
            DONEM = 2,
            PRSICILNO = 3,
            AMORTISMANTUR = 4,
            DEGERARTISKODU = 5,
            AMORTISMANMIKTAR = 6,
            MUHASEBEKOD = 7,
            HARCAMABIRIMKOD = 8,
            AMBARKOD = 9,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "YIL", "DONEM", "PRSICILNO", "AMORTISMANTUR", "DEGERARTISKODU", "AMORTISMANMIKTAR", "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD" };

        /// <summary>
        /// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Amortisman Başlama Yılını döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortisman başlama yılını tutan Int türünde nesne döndürür</returns>
        public int AmortismanBaslamaYilDondur(AmortismanKriter ak)
        {
            AmortismanKriter akIc = new AmortismanKriter();
            akIc.muhasebeKod = ak.muhasebeKod;
            akIc.harcamaKod = ak.harcamaKod;
            akIc.ambarKod = ak.ambarKod;

            string sql = "SELECT " + VeriErisim.IslecBul("MIN") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.YIL] + ")";
            sql += " FROM " + TABLOAD;
            sql = KosulEkle(akIc, sql, TABLOAD, "WHERE", false);

            return veriErisim.TekKolonSQLInt(sql);
        }

        ///// <summary>
        ///// Amortisman bilgilerini veri tabanına kaydeden
        ///// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="amortisman">Amortisman bilgilerini tutan, Amortisman türünde nesne</param>
        ///// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        //public Sonuc RakamKaydet(int yil, int prSicilNo, ENUMAmortismanTur amortismanTur, decimal amortismanMiktar)
        //{
        //    string sql = "UPDATE " + TABLOAD + " SET " + ALANLAR[(int)ENUMAlanlar.AMORTISMANMIKTAR] + " = " + VeriErisim.DegerYap(amortismanMiktar);

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMORTISMANTUR], "=", (int)amortismanTur));

        //    return KomutCalistir(sql, 1);
        //}

        /// <summary>
        /// Amortisman bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="amortisman">Amortisman bilgilerini tutan, Amortisman türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc RakamSil(int yil, int donem, int prSicilNo, ENUMAmortismanTur amortismanTur, string degerArtisKodu)
        {
            string sql = "DELETE FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", prSicilNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", donem));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMORTISMANTUR], "=", (int)amortismanTur));

            if (!string.IsNullOrEmpty(degerArtisKodu))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DEGERARTISKODU], "=", degerArtisKodu));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Amortisman bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="amortisman">Amortisman bilgilerini tutan, Amortisman türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(string kod, int yil, int donem, int prSicilNo, int amortismanTur, string degerArtisKod, decimal amortismanMiktar, string saymanlikKod, string birimKod, string ambarKod)
        {
            if (kod == "")
                kod = Guid.NewGuid().ToString("N");

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(donem) + ",";
            sql += VeriErisim.DegerYap(prSicilNo) + ",";
            sql += VeriErisim.DegerYap(amortismanTur) + ",";
            sql += VeriErisim.DegerYap(degerArtisKod) + ",";
            sql += VeriErisim.DegerYap(amortismanMiktar) + ",";
            sql += VeriErisim.DegerYap(saymanlikKod) + ",";
            sql += VeriErisim.DegerYap(birimKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(ambarKod) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman Kriter bilgilerini kullanarak
        /// ilgili Amortisman bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(AmortismanKriter ak)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ak.yil));
            if (ak.donem > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DONEM], "=", ak.donem));

            sql = KosulEkle(ak, sql, TABLOAD, "AND", false);

            return KomutCalistir(sql, 0);
        }

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Amortisman Cari Miktarını değerleme kriterine göre döndüren,
        ///// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="degerleme">Değerleme yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        ///// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortisman Cari miktarlarını tutan IDataReader türünde nesne döndürür</returns>
        //public IDataReader SicilAmortismanCariMiktarListele(AmortismanKriter ak, bool degerleme)
        //{
        //    string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];
        //    sql += ", " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMORTISMANMIKTAR];
        //    sql += " FROM " + TABLOAD;

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ak.yil));

        //    if (degerleme)
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMORTISMANTUR], "=", (int)ENUMAmortismanTur.DEGERLEME));
        //    else
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMORTISMANTUR], "<>", (int)ENUMAmortismanTur.DEGERLEME));

        //    sql = KosulEkle(ak, sql, TABLOAD, "AND", false);

        //    sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];

        //    return veriErisim.DataReaderDondur(sql);
        //}

        /// <summary>
        /// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Amortisman Sayıları ve Miktarlarını değerleme ve cari dahil kriterine göre döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <param name="degerleme">Değerleme yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        /// <param name="cariDahil">Carinin dahil edilip edilmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortisman Sayı ve miktarlarını tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader SicilAmortismanSayiVeMiktarListele(AmortismanKriter ak, bool degerleme, bool cariDahil)
        {
            string sql = "SELECT " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];
            sql += ", " + VeriErisim.IslecBul("COUNT") + "(*)";
            sql += ", " + VeriErisim.IslecBul("SUM") + "(" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.AMORTISMANMIKTAR] + ")";
            sql += " FROM " + TABLOAD;

            if (cariDahil)
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<=", ak.yil));
            else
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "<=", ak.yil - 1));

            if (degerleme)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMORTISMANTUR], "=", (int)ENUMAmortismanTur.DEGERLEME));
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AMORTISMANTUR], "<>", (int)ENUMAmortismanTur.DEGERLEME));

            sql = KosulEkle(ak, sql, TABLOAD, "AND", true);

            sql += " GROUP BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];
            sql += " ORDER BY " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];

            return veriErisim.DataReaderDondur(sql);
        }

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Amortisman Yapılacaklara ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        ///// </summary>
        ///// <param name="tabloRef">Tablo referansını tutan, String türünde nesne</param>
        ///// <param name="ak">Amortisman kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="count">Sayma işleminin yapılıp yapılmayacağını tutan, Bool türünde nesne</param>
        ///// <param name="bitenlerDahil">Amortismanı bitenlerin dahil edilip edilmeyeceğini tutan, Bool türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        //private string AmortismanYapilacaklarSQL(string tabloRef, AmortismanKriter ak, bool count, bool bitenlerDahil)
        //{
        //    string sql = "SELECT ";
        //    if (count)
        //        sql += VeriErisim.IslecBul("COUNT") + "(*)";
        //    else
        //        sql += "*";
        //    sql += " FROM TASSICILNOHAREKET " + tabloRef;

        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(tabloRef, "YIL", "<=", ak.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(tabloRef, "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));
        //    sql = VeriErisim.KosulEkle(sql, "AND (", VeriErisim.KosulOl(tabloRef, "ISLEMTURUKOD", "<=", (int)ENUMIslemTipi.ACILIS));
        //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(tabloRef, "ISLEMTURUKOD", "=", (int)ENUMIslemTipi.DEVIRCIKIS));
        //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(tabloRef, "ISLEMTURUKOD", "=", (int)ENUMIslemTipi.ZFVERME));
        //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(tabloRef, "ISLEMTURUKOD", "=", (int)ENUMIslemTipi.DTLVERME));
        //    sql = VeriErisim.KosulEkle(sql, "OR", VeriErisim.KosulOl(tabloRef, "ISLEMTURUKOD", "=", (int)ENUMIslemTipi.DAGITIMIADECIKIS));
        //    sql += ")";

        //    sql += " AND " + tabloRef + ".SIRANO = " + SicilMaksSiraNoIcSQLOlustur(ak, tabloRef);

        //    if (!bitenlerDahil)
        //    {
        //        string birikmisAmortismanSQL = SicilBirikmisAmortismanIcSQLOlustur(ak, tabloRef);
        //        string birikmisDegerlemeSQL = TABAmortismanDegerleme.SicilBirikmisDegerlemeIcSQLOlustur(ak, tabloRef);

        //        sql += " AND ((" + birikmisAmortismanSQL + " IS NULL)";
        //        sql += " OR (" + tabloRef + ".FIYAT > " + birikmisAmortismanSQL + ")";
        //        sql += " OR (" + tabloRef + ".FIYAT + " + birikmisDegerlemeSQL + " > " + birikmisAmortismanSQL + ")";
        //        sql += " OR (((" + tabloRef + ".FIYAT = " + birikmisAmortismanSQL;
        //        sql += " AND " + birikmisDegerlemeSQL + " IS NULL)";
        //        sql += " OR (" + tabloRef + ".FIYAT + " + birikmisDegerlemeSQL + " = " + birikmisAmortismanSQL + "))";
        //        sql += " AND " + SicilAmortismanMaksYilIcSQLOlustur(ak, tabloRef) + " >= " + ak.yil;
        //        sql += " AND " + SicilAmortismanMinYilIcSQLOlustur(ak, tabloRef) + " <= " + ak.yil + "))";
        //    }

        //    if (ak.edinmeIlkYili > 0 || ak.edinmeSonYili > 0)
        //        sql += " AND " + SicilYilIcSQLOlustur(ak, tabloRef) + " <= " + ak.yil;

        //    sql = KosulEkle(ak, sql, tabloRef, "AND", true);

        //    return sql;
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Amortisman Yapılacak Sayıları döndüren fonksiyon. Sql cümlesi Amortisman Yapılacaklar SQL rutini kullanılarak oluşturulur.
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda Amortisman yapılacaklarını sayısını tutan Int türünde nesne döndürür</returns>
        //public int AmortismanYapilacakSayiDondur(AmortismanKriter ak)
        //{
        //    string tabloRef = "A";
        //    string sql = AmortismanYapilacaklarSQL(tabloRef, ak, true, false);
        //    return veriErisim.TekKolonSQLInt(sql);
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Amortisman Yapılacakları döndüren fonksiyon. Sql cümlesi Amortisman Yapılacaklar SQL rutini kullanılarak oluşturulur.
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="bitenlerDahil">Amortismanı bitenlerin dahil edilip edilmeyeceği bilgisini tutan, Bool türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, Amortisman Yapılacaklar bilgisini tutan IDataReader türünde nesne döndürür</returns>
        //public IDataReader AmortismanYapilacaklar(AmortismanKriter ak, bool bitenlerDahil)
        //{
        //    string tabloRef = "A";
        //    string sql = AmortismanYapilacaklarSQL(tabloRef, ak, false, bitenlerDahil);
        //    sql += " ORDER BY " + tabloRef + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO];
        //    return veriErisim.DataReaderDondur(sql);
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Sicil yılına ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        //private string SicilYilIcSQLOlustur(AmortismanKriter ak, string tablo)
        //{
        //    string sql = "(SELECT TASSICILNOHAREKET.YIL";
        //    sql += " FROM TASSICILNOHAREKET";
        //    if (ak.edinmeIlkYili > 0 || ak.edinmeSonYili > 0)
        //        sql += ", TASISLEMDETAY";
        //    sql += " WHERE TASSICILNOHAREKET.PRSICILNO = " + tablo + ".PRSICILNO";
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNOHAREKET", "SIRANO", "=", 1));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNOHAREKET", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));

        //    if (ak.edinmeIlkYili > 0 || ak.edinmeSonYili > 0)
        //    {
        //        sql += " AND TASSICILNOHAREKET.YIL = TASISLEMDETAY.YIL";
        //        sql += " AND TASSICILNOHAREKET.MUHASEBEKOD = TASISLEMDETAY.MUHASEBEKOD";
        //        sql += " AND TASSICILNOHAREKET.HARCAMABIRIMKOD = TASISLEMDETAY.HARCAMABIRIMKOD";
        //        sql += " AND TASSICILNOHAREKET.FISNO = TASISLEMDETAY.FISNO";
        //        sql += " AND TASSICILNOHAREKET.FISSIRANO = TASISLEMDETAY.SIRANO";
        //        if (ak.edinmeIlkYili > 0)
        //            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMDETAY", "EALIMTARIHI", ">=", new TNSDateTime("01.01." + ak.edinmeIlkYili.ToString())));
        //        if (ak.edinmeSonYili > 0)
        //            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASISLEMDETAY", "EALIMTARIHI", "<=", new TNSDateTime("31.12." + ak.edinmeSonYili.ToString())));
        //    }
        //    return sql + ")";
        //}

        /// <summary>
        /// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Sicil devrine ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        private string SicilDevirIcSQLOlustur(AmortismanKriter ak, string tablo)
        {
            string sql = "(SELECT 1";
            sql += " FROM TASSICILNOHAREKET B";
            sql += " WHERE B.PRSICILNO = " + tablo + ".PRSICILNO";

            if (!string.IsNullOrEmpty(ak.muhasebeKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "MUHASEBEKOD", "=", ak.muhasebeKod));

            if (!string.IsNullOrEmpty(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));

            if (!string.IsNullOrEmpty(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "AMBARKOD", "=", ak.ambarKod));

            if (!string.IsNullOrEmpty(ak.gorSicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("B", "SICILNO", "=", ak.gorSicilNo));

            string icSQL = "SELECT " + VeriErisim.IslecBul("MAX") + "(SIRANO)";
            icSQL += " FROM TASSICILNOHAREKET";
            icSQL += " WHERE TASSICILNOHAREKET.PRSICILNO = " + "B.PRSICILNO";
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("TASSICILNOHAREKET", "YIL", "<=", ak.yil));
            icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("TASSICILNOHAREKET", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));

            sql += " AND B.SIRANO = (" + icSQL + ")";

            return sql + ")";
        }

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Sicil maksimum sıra numarasına ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        //private string SicilMaksSiraNoIcSQLOlustur(AmortismanKriter ak, string tablo)
        //{
        //    string sql = "(SELECT " + VeriErisim.IslecBul("MAX") + "(SIRANO)";
        //    sql += " FROM TASSICILNOHAREKET";
        //    sql += " WHERE TASSICILNOHAREKET.PRSICILNO = " + tablo + ".PRSICILNO";
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNOHAREKET", "YIL", "<=", ak.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TASSICILNOHAREKET", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));
        //    return sql + ")";
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Sicil Birikmiş Amortismana ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        //private string SicilBirikmisAmortismanIcSQLOlustur(AmortismanKriter ak, string tablo)
        //{
        //    string sql = "(SELECT " + VeriErisim.IslecBul("SUM") + "(" + ALANLAR[(int)ENUMAlanlar.AMORTISMANMIKTAR] + ")";
        //    sql += " FROM " + TABLOAD;
        //    sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " = " + tablo + ".PRSICILNO";
        //    return sql + ")";
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Sicil Amortisman Maksimum yılına ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        //private string SicilAmortismanMaksYilIcSQLOlustur(AmortismanKriter ak, string tablo)
        //{
        //    string sql = "(SELECT " + VeriErisim.IslecBul("MAX") + "(" + ALANLAR[(int)ENUMAlanlar.YIL] + ")";
        //    sql += " FROM " + TABLOAD;
        //    sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " = " + tablo + ".PRSICILNO";
        //    return sql + ")";
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Sicil Amortisman Minimum yılına ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        //private string SicilAmortismanMinYilIcSQLOlustur(AmortismanKriter ak, string tablo)
        //{
        //    string sql = "(SELECT " + VeriErisim.IslecBul("MIN") + "(" + ALANLAR[(int)ENUMAlanlar.YIL] + ")";
        //    sql += " FROM " + TABLOAD;
        //    sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " = " + tablo + ".PRSICILNO";
        //    return sql + ")";
        //}

        /// <summary>
        /// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun Amortisman Yapılan malzemelerin sayılarını döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <returns>Select komutunun çalışması sonucunda, Amortisman yapılan Taşınır malzemelerinin sayısını tutan IDataReader türünde nesne döndürür</returns>
        public int AmortismanYapilmisSayiDondur(AmortismanKriter ak)
        {
            string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(" + VeriErisim.IslecBul("DISTINCT") + " " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + ")";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", ak.yil));

            sql = KosulEkle(ak, sql, TABLOAD, "AND", false);

            return veriErisim.TekKolonSQLInt(sql);
        }

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Sicil Amortismanı biten Taşınır malzemelerine ilişkin SQL sorgu cümlesini oluşturan fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <param name="sadeceYildaBitenler">Belirlenen yılda bitenlere göre olup olmadığını tutan, Bool türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, oluşturulan SQL cümlesini içeren String türünde nesne döndürür</returns>
        //private string AmortismanBitenlerSQL(AmortismanKriter ak, bool sadeceYildaBitenler)
        //{
        //    string tablo = "TASSICILNOHAREKET";
        //    string sql = "SELECT * FROM " + tablo;
        //    sql += " WHERE (SELECT " + VeriErisim.IslecBul("MAX") + "(" + TABLOAD + ".YIL)";
        //    sql += " FROM " + TABLOAD + " WHERE " + TABLOAD + ".PRSICILNO = " + tablo + ".PRSICILNO)";

        //    if (sadeceYildaBitenler)
        //        sql += " = " + ak.yil;
        //    else
        //    {
        //        sql += " >= " + ak.yil;

        //        sql += " AND (SELECT " + VeriErisim.IslecBul("MIN") + "(" + TABLOAD + ".YIL)";
        //        sql += " FROM " + TABLOAD + " WHERE " + TABLOAD + ".PRSICILNO = " + tablo + ".PRSICILNO) <= " + ak.yil;
        //    }

        //    sql = KosulEkle(ak, sql, tablo, "AND", true);

        //    return sql;
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kıstas olarak kullanarak,
        ///// kıstasa uygun Amortismanı biten malzemelerin sayılarını döndüren,
        ///// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Amortismanı biten malzeme Sayısını tutan Int türünde nesne döndürür</returns>
        //public int AmortismanBitenSayiDondur(AmortismanKriter ak)
        //{
        //    string tablo = "TASSICILNOHAREKET";
        //    string sql = "SELECT " + VeriErisim.IslecBul("COUNT") + "(*)";
        //    sql += " FROM " + tablo;
        //    sql += " WHERE (SELECT " + VeriErisim.IslecBul("MAX") + "(" + TABLOAD + ".YIL)";
        //    sql += " FROM " + TABLOAD + " WHERE " + TABLOAD + ".PRSICILNO = " + tablo + ".PRSICILNO) >= " + ak.yil;
        //    sql += " AND (SELECT " + VeriErisim.IslecBul("MIN") + "(" + TABLOAD + ".YIL)";
        //    sql += " FROM " + TABLOAD + " WHERE " + TABLOAD + ".PRSICILNO = " + tablo + ".PRSICILNO) <= " + ak.yil;

        //    sql = KosulEkle(ak, sql, tablo, "AND", true);

        //    return veriErisim.TekKolonSQLInt(sql);
        //}

        ///// <summary>
        ///// Parametre olarak verilen Amortisman kriter bilgilerini kullanarak,
        ///// Amortisman Yapılacak Sayı Döndür ve Amortisman Yapılmış Sayı Döndür rutinlerini çalıştırarak,
        ///// Taşınır malzemelerinin hepsinin amortismanının yapılıp yapılmadığı bilgisini döndüren fonksiyon
        ///// </summary>
        ///// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        ///// <returns>Fonksiyonun çalışması sonucunda, Amortismanın hepsinin yapılıp yapılmadığı bilgisini tutan, yapılmış ise true, yapılmamışsa false döndüren, Bool türünde nesne döndürür</returns>
        //public bool AmortismanYapilmisMiHepsi(AmortismanKriter ak)
        //{
        //    return AmortismanYapilacakSayiDondur(ak) == AmortismanYapilmisSayiDondur(ak) ? true : false;
        //}

        /// <summary>
        /// Parametre olarak verilen Amortisman kriter bilgilerini kullanarak,
        /// Amortisman yapılıp yapılmadığı bilgisini döndüren fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <returns>Fonksiyonun çalışması sonucunda, Amortismanın yapılıp yapılmadığı bilgisini tutan, yapılmış ise true, yapılmamışsa false döndüren, Bool türünde nesne döndürür</returns>
        public bool AmortismanYapilmisMi(AmortismanKriter ak)
        {
            return AmortismanYapilmisSayiDondur(ak) > 0 ? true : false;
        }

        /// <summary>
        /// Parametre olarak verilen Amortisman kriter bilgileri, daha önce yazılmış sql cümlesi, tablo adı,
        /// bağlaç ve devir ekleme bilgilerini kullanarak daha önce hazırlanan sql cümlesine koşul ekleyen fonksiyon
        /// </summary>
        /// <param name="ak">Amortisman Kriter bilgilerini tutan, AmortismanKriter türünde nesne</param>
        /// <param name="sql">Daha önce hazırlanmış Sql cümleciği, String türünde nesne</param>
        /// <param name="tablo">Tablo adını tutan, String türünde nesne</param>
        /// <param name="baglac">Bağlaç ifadesini tutan, String türünde nesne</param>
        /// <param name="devirEkle">Devir eklenip eklenmeyeceğini tutan, Bool türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, koşul eklenmiş sql cümlesini tutan String türünde nesne döndürür</returns>
        public string KosulEkle(AmortismanKriter ak, string sql, string tablo, string baglac, bool devirEkle)
        {
            string kosul = string.Empty;

            if (devirEkle)
                kosul = VeriErisim.KosulEkle(kosul, "AND", "EXISTS " + SicilDevirIcSQLOlustur(ak, tablo));
            else
            {
                if (!string.IsNullOrEmpty(ak.muhasebeKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", ak.muhasebeKod));

                if (!string.IsNullOrEmpty(ak.harcamaKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", ak.harcamaKod.Replace(".", "")));

                if (!string.IsNullOrEmpty(ak.ambarKod))
                    kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(tablo, ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", ak.ambarKod));
            }

            if (ak.prSicilNolar.Count > 0)
            {
                kosul += " AND (";
                for (int i = 0; i < ak.prSicilNolar.Count; i++)
                {
                    if (i == 0)
                        kosul += VeriErisim.KosulOl(tablo, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", (int)ak.prSicilNolar[i]);
                    else
                        kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl(tablo, ALANLAR[(int)ENUMAlanlar.PRSICILNO], "=", (int)ak.prSicilNolar[i]));
                }
                kosul += ")";
            }
            else if (!string.IsNullOrEmpty(ak.belgeNo))
            {
                string icSQL = "(SELECT B.PRSICILNO FROM TASSICILNOHAREKET B";
                //icSQL += " WHERE " + tablo + "." + ALANLAR[(int)ENUMAlanlar.YIL] + " = B.YIL";
                //icSQL += " AND " + tablo + "." + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + " = B.MUHASEBEKOD";
                //icSQL += " AND " + tablo + "." + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + " = B.HARCAMABIRIMKOD";
                icSQL = VeriErisim.KosulEkle(icSQL, "WHERE", VeriErisim.KosulOl("B", "YIL", "=", ak.belgeYil));
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "MUHASEBEKOD", "=", ak.muhasebeKod));
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "FISNO", "=", ak.belgeNo));
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "ISLEMTIPIKOD", "<", (int)ENUMZimmetIslemTipi.ZFVERME));
                icSQL = VeriErisim.KosulEkle(icSQL, "AND", VeriErisim.KosulOl("B", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL)) + ")";

                kosul = VeriErisim.KosulEkle(kosul, "AND", tablo + "." + ALANLAR[(int)ENUMAlanlar.PRSICILNO] + " IN " + icSQL);
            }

            if (!string.IsNullOrEmpty(kosul))
                sql = VeriErisim.KosulEkle(sql, baglac, kosul);

            return sql;
        }

        public string SicilNoHareketGirisCikisTarihBulSql(int yil, string saymanlikKod, string birimKod, string ambarKod, string hesapKod, bool giris)
        {
            string sql = "SELECT A.PRSICILNO, A.FISTARIH, A.SIRANO FROM TASSICILNOHAREKET A";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "YIL", "<=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));
            if (!string.IsNullOrWhiteSpace(hesapKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HESAPPLANKOD", "LIKE", hesapKod.Replace(".", "") + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", saymanlikKod));
            if (!string.IsNullOrWhiteSpace(birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", birimKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "AMBARKOD", "=", ambarKod));

            sql = VeriErisim.KosulEkle(sql, "AND", "A.SIRANO = " + SIRANOSQL("A", giris, true, yil, true));
            sql += " ORDER BY 1";

            return sql;
        }

        public string SicilNoHareketGirisBulSql(int yil, string saymanlikKod, string birimKod, string ambarKod, string hesapKod, string DisSqlAlan)
        {
            string alanlar = "A.PRSICILNO, A.YIL, A.SICILNO, A.HESAPPLANKOD, A.KDVLIBIRIMFIYAT + A.DEGERARTISI, A.FISTARIH, A.ISLEMTURUKOD, HP.ACIKLAMA, HP.AMORTIYIL, A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.AMBARKOD, DA.KOD AS DEGERARTISKOD";
            if (!string.IsNullOrWhiteSpace(DisSqlAlan))
            {
                alanlar = "A.PRSICILNO";
            }
            string sql = "SELECT " + alanlar + " FROM TASSICILNOHAREKET A INNER JOIN TASHESAPPLAN HP ON A.HESAPPLANKOD = HP.KOD";

            if (string.IsNullOrWhiteSpace(DisSqlAlan))
                sql += " LEFT OUTER JOIN TASSICILNODEGERARTIS DA ON DA.PRSICILNO = A.PRSICILNO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "YIL", "<=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));
            if (!string.IsNullOrWhiteSpace(hesapKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HESAPPLANKOD", "LIKE", hesapKod.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", BENDEKISICILBUL("A", yil, saymanlikKod, birimKod, ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", "A.SIRANO = " + SIRANOSQL("A", true, true, yil, false));
            if (!string.IsNullOrWhiteSpace(DisSqlAlan))
                sql = VeriErisim.KosulEkle(sql, "AND", "A.PRSICILNO = " + DisSqlAlan);
            else
                sql += " ORDER BY 1";

            return sql;
        }

        private string BENDEKISICILBUL(string tabloAd, int yil, string saymanlikKod, string birimKod, string ambarKod)
        {
            string sql = "SELECT 1 FROM TASSICILNOHAREKET B";
            sql += " WHERE B.PRSICILNO = " + tabloAd + ".PRSICILNO";
            sql += " AND B.MUHASEBEKOD = " + VeriErisim.DegerYap(saymanlikKod);
            if (!string.IsNullOrWhiteSpace(birimKod))
                sql += " AND B.HARCAMABIRIMKOD = " + VeriErisim.DegerYap(birimKod.Replace(".", ""));

            if (ambarKod.StartsWith("@"))
            {
                ambarKod = ambarKod.Replace("@", "");
                string[] aranan = ambarKod.Split(',');
                string ekKosul = "";
                for (int i = 0; i < aranan.Length; i++)
                {
                    if (aranan[i] == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("B", "AMBARKOD", "LIKE", aranan[i]));
                }
                if (ekKosul != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
            }
            else if (!string.IsNullOrWhiteSpace(ambarKod))
                sql += " AND B.AMBARKOD = " + VeriErisim.DegerYap(ambarKod);

            sql += " AND B.DURUM <> 2";
            sql += " AND B.YIL <= " + VeriErisim.DegerYap(yil);
            //sql += " AND 1 NOT IN  (SELECT 1 FROM TASSICILNOHAREKET C WHERE C.PRSICILNO = B.PRSICILNO AND C.YIL<=" + VeriErisim.DegerYap(yil) + " AND C.ISLEMTURUKOD>=51 AND  C.ISLEMTURUKOD<=59)";
            sql += " AND " + tabloAd + ".SIRANO = B.SIRANO";

            return "1 IN (" + sql + ")";
        }

        private string SIRANOSQL(string tabloAd, bool? giris, bool maks, int yil, bool bendekiler)
        {
            string maksMin;
            if (maks)
                maksMin = "MAX";
            else
                maksMin = "MIN";
            string sql = "SELECT " + maksMin + "(B.SIRANO) FROM TASSICILNOHAREKET B";
            sql += " WHERE B.PRSICILNO = " + tabloAd + ".PRSICILNO";
            if (bendekiler)
            {
                sql += " AND B.MUHASEBEKOD = " + tabloAd + ".MUHASEBEKOD";
                sql += " AND B.HARCAMABIRIMKOD = " + tabloAd + ".HARCAMABIRIMKOD";
                sql += " AND B.AMBARKOD = " + tabloAd + ".AMBARKOD";
            }
            sql += " AND B.YIL <= " + VeriErisim.DegerYap(yil);
            sql += " AND B.DURUM <> 2";
            if (giris != null)
                sql += " AND B.ISLEMTURUKOD IN " + GirisCikisKodlar(giris.Value, bendekiler);
            return "(" + sql + ")";
        }

        public string SicilNoAmortismanBulSql(int yil, string saymanlikKod, string birimKod, string ambarKod, string hesapKod, bool uretimRaporu)
        {
            string sql = "SELECT ONCAMOR.PRSICILNO, SUM(ONCAMOR.AMORTISMANMIKTAR) FROM TASAMORTISMAN ONCAMOR";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("ONCAMOR", "YIL", "<", yil));

            string icSql;
            if (uretimRaporu)
                icSql = SicilNoHareketGirisBulSql(yil, saymanlikKod, birimKod, ambarKod, hesapKod, "ONCAMOR.PRSICILNO");
            else
                icSql = SicilNoAmortismanBilgiBulSql(yil, saymanlikKod, birimKod, ambarKod, hesapKod, "ONCAMOR.PRSICILNO");
            sql = VeriErisim.KosulEkle(sql, "AND", "ONCAMOR.PRSICILNO IN (" + icSql + ")");

            sql += " GROUP BY ONCAMOR.PRSICILNO";
            sql += " ORDER BY 1";

            return sql;
        }

        public string MIFBilgiBulSql(int yil, string saymanlikKod, string birimKod, string kumulatif)
        {
            string alan = "'2570'+SUBSTRING(A.HESAPPLANKOD,3, 5)";
            string alanAd = "HP.ACIKLAMA";
            //, 
            //+ ", A.KDVLIBIRIMFIYAT + A.DEGERARTISI"
            string sql1 = "SELECT A.HARCAMABIRIMKOD HB, " + alan + " HK, " + alanAd + " HA, SUM(T.AMORTISMANMIKTAR) AMORTISMANMIKTAR FROM TASSICILNOHAREKET A INNER JOIN TASAMORTISMAN T ON A.PRSICILNO=T.PRSICILNO LEFT OUTER JOIN ButceIslemleri.dbo.MUHHESAPPLANI HP ON HP.YIL=" + VeriErisim.DegerYap(yil) + " AND HP.HESAPKOD=" + alan;
            sql1 = VeriErisim.KosulEkle(sql1, "WHERE", "SIRANO = (SELECT max(SIRANO) FROM TASSICILNOHAREKET SHI WHERE SHI.PRSICILNO=A.PRSICILNO AND SHI.DURUM<>2 AND SHI.ISLEMTIPIKOD<1000 AND YIL <=" + VeriErisim.DegerYap(yil) + ")");
            sql1 = VeriErisim.KosulEkle(sql1, "AND", "ISLEMTURUKOD<50");
            sql1 = VeriErisim.KosulEkle(sql1, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", saymanlikKod));
            sql1 = VeriErisim.KosulEkle(sql1, "AND", VeriErisim.KosulOl("T", "YIL", (kumulatif == "1" ? "<=" : "="), yil));
            if (kumulatif != "1")
            {
                sql1 = VeriErisim.KosulEkle(sql1, "AND", VeriErisim.KosulOl("T", "DONEM", ">", 0));
            }
            if (!string.IsNullOrWhiteSpace(birimKod))
            {
                sql1 = VeriErisim.KosulEkle(sql1, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", birimKod.Replace(".", "")));
            }
            sql1 += " GROUP BY A.HARCAMABIRIMKOD, " + alan + ", " + alanAd;

            string sql2 = "SELECT A.HARCAMABIRIMKOD HB, " + alan + " HK, " + alanAd + " HA, SUM(A.KDVLIBIRIMFIYAT + A.DEGERARTISI) DEGER FROM TASSICILNOHAREKET A LEFT OUTER JOIN ButceIslemleri.dbo.MUHHESAPPLANI HP ON HP.YIL=" + VeriErisim.DegerYap(yil) + " AND HP.HESAPKOD=" + alan;
            sql2 = VeriErisim.KosulEkle(sql2, "WHERE", "SIRANO = (SELECT max(SIRANO) FROM TASSICILNOHAREKET SHI WHERE SHI.PRSICILNO=A.PRSICILNO AND SHI.DURUM<>2 AND SHI.ISLEMTIPIKOD<1000 AND YIL <=" + VeriErisim.DegerYap(yil) + ")");
            sql2 = VeriErisim.KosulEkle(sql2, "AND", "ISLEMTURUKOD<50");
            sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", saymanlikKod));
            if (!string.IsNullOrWhiteSpace(birimKod))
            {
                sql2 = VeriErisim.KosulEkle(sql2, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", birimKod.Replace(".", "")));
            }
            sql2 += " GROUP BY A.HARCAMABIRIMKOD, " + alan + ", " + alanAd;

            string sql = "SELECT case when A.HB is null then B.HB else A.HB end, case when A.HK is null then B.HK else A.HK end, case when A.HA is null then B.HA else A.HA end, A.AMORTISMANMIKTAR, B.DEGER FROM (" + sql1 + ") A FULL OUTER JOIN (" + sql2 + ") B ON A.HB= B.HB AND B.HK=A.HK";
            sql += " ORDER BY 1, 2";

            return sql;
        }

        private string GirisCikisKodlar(bool giris, bool bendekiler)
        {
            string retVal;
            if (giris)
            {
                retVal = (int)ENUMIslemTipi.SATINALMAGIRIS + "," + (int)ENUMIslemTipi.BAGISGIRIS + "," + (int)ENUMIslemTipi.SAYIMFAZLASIGIRIS + "," + (int)ENUMIslemTipi.IADEGIRIS + "," + (int)ENUMIslemTipi.URETILENGIRIS + "," + (int)ENUMIslemTipi.DEVIRGIRISKURUM + "," + (int)ENUMIslemTipi.DAGITIMIADEGIRIS + "," + (int)ENUMIslemTipi.ACILIS + "," + (int)ENUMIslemTipi.DEVIRGIRIS;
                if (bendekiler)
                    retVal += "";
            }
            else
            {
                retVal = (int)ENUMIslemTipi.TUKETIMCIKIS + "," + (int)ENUMIslemTipi.BAGISCIKIS + "," + (int)ENUMIslemTipi.SATISCIKIS + "," + (int)ENUMIslemTipi.KULLANILMAZCIKIS + "," + (int)ENUMIslemTipi.HURDACIKIS + "," + (int)ENUMIslemTipi.DEVIRCIKISKURUM + "," + (int)ENUMIslemTipi.SAYIMNOKSANICIKIS + "," + (int)ENUMIslemTipi.YILDEVIRCIKIS + "," + (int)ENUMIslemTipi.DAGITIMIADECIKIS + "," + (int)ENUMIslemTipi.DEVIRCIKIS;
                if (bendekiler)
                    retVal += "";
            }
            return "(" + retVal + ")";
        }

        public int MinYilBul()
        {
            string sql = "SELECT MIN(A.FISTARIH) FROM TASSICILNOHAREKET A ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));
            sql = VeriErisim.KosulEkle(sql, "AND", "A.ISLEMTURUKOD IN " + GirisCikisKodlar(true, true));

            TNSDateTime dt = this.veriErisim.TekKolonSQLDate(sql);
            if (!dt.isNull) return dt.Yil;
            else return System.DateTime.Now.Year;
        }

        public string SicilNoAmortismanBilgiBulSql(int yil, string saymanlikKod, string birimKod, string ambarKod, string hesapPlanKod, string DisSqlAlan)
        {
            string alanlar = "A.PRSICILNO, H.YIL, H.SICILNO, H.HESAPPLANKOD, H.KDVLIBIRIMFIYAT, H.ISLEMTARIH, H.ISLEMTURUKOD, HP.ACIKLAMA, HP.AMORTIYIL, A.MUHASEBEKOD,  A.HARCAMABIRIMKOD, A.AMBARKOD,  A.AMORTISMANMIKTAR";
            if (!string.IsNullOrWhiteSpace(DisSqlAlan))
            {
                alanlar = "A.PRSICILNO";
            }
            string sql = "SELECT " + alanlar + " FROM TASAMORTISMAN AS A INNER JOIN ";
            sql += "TASSICILNOHAREKET AS H ON A.PRSICILNO = H.PRSICILNO AND H.SIRANO = ";
            sql += "(SELECT MAX(SIRANO) FROM TASSICILNOHAREKET AS IC WHERE (PRSICILNO = H.PRSICILNO) AND (DURUM <> 2) AND YIL<=" + yil.ToString() + ") ";
            sql += "INNER JOIN TASHESAPPLAN HP ON H.HESAPPLANKOD=HP.KOD ";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "YIL", "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", saymanlikKod));

            if (!string.IsNullOrWhiteSpace(birimKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", birimKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "AMBARKOD", "=", ambarKod));
            if (!string.IsNullOrWhiteSpace(hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", hesapPlanKod.Replace(".", "") + "%"));
            if (!string.IsNullOrWhiteSpace(DisSqlAlan))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "A.PRSICILNO =" + DisSqlAlan);
            }

            return sql;
        }

        public IDataReader SicilNoHareketGirisBul(AmortismanKriter ak)
        {
            string alanlar = "A.PRSICILNO, A.YIL, A.SICILNO, A.HESAPPLANKOD, A.KDVLIBIRIMFIYAT, A.FISTARIH, A.ISLEMTIPIKOD, A.ISLEMTURUKOD, HP.ACIKLAMA, HP.AMORTIYIL, HP.KISTAMORTISMAN, A.MUHASEBEKOD, A.HARCAMABIRIMKOD, A.AMBARKOD, TASMUHASEBE.AD, TASHARCAMABIRIM.AD ";

            alanlar += ", (SELECT IC.PRSICILNO,IC.SIRANO,IC.YIL,IC.MUHASEBEKOD,IC.HARCAMABIRIMKOD,IC.AMBARKOD,IC.HESAPPLANKOD,IC.SICILNO,IC.SICILSIRANO,IC.FISSIRANO,IC.FISTARIH,IC.ISLEMTIPIKOD,IC.ISLEMTURUKOD,IC.ISLEMTARIH,IC.ISLEMYAPAN,IC.DURUM,IC.NEREYEGITTI,IC.KIMEGITTI,IC.KDVLIBIRIMFIYAT,IC.FISNO" +
                " FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO=A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) "; //ORDER BY IC.ISLEMTARIH
            alanlar += ", (SELECT IC.KOD, IC.MUHASEBEKOD, IC.HARCAMABIRIMKOD, IC.BELGENO, IC.BELGETARIHI, IC.PRSICILNO, IC.GORSICILNO, IC.TUTAR, IC.GEREKCE, IC.DURUM, IC.TUR FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) "; //ORDER BY IC.BELGETARIHI 
            alanlar += ", (SELECT IC.KOD, IC.YIL, IC.DONEM, IC.PRSICILNO, IC.AMORTISMANTUR, IC.DEGERARTISKODU, IC.AMORTISMANMIKTAR, IC.MUHASEBEKOD, IC.HARCAMABIRIMKOD, IC.AMBARKOD FROM TASAMORTISMAN IC WHERE IC.PRSICILNO = A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) ";
            alanlar += ", (SELECT IC.PRSICILNO, IC.BISNO, IC.ESKIBISNO1, IC.DISSICILNO, IC.AMORTISMANYIL, IC.AMORTISMANBITTI, IC.BULUNDUGUYER ";
            if (TNS.TMM.Arac.MerkezBankasiKullaniyor())
            {
                alanlar += ", TASODA.AD, TASODA.MALIYETMERKEZI ";
                alanlar += " FROM TASSICILNOOZELLIK IC LEFT OUTER JOIN TASODA ON IIF(ISNUMERIC(IC.BULUNDUGUYER) = 1, IC.BULUNDUGUYER, '') = TASODA.KOD WHERE IC.PRSICILNO=A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) "; //ORDER BY IC.ISLEMTARIH
            }
            else
            {
                alanlar += ", '' AS AD";
                alanlar += " FROM TASSICILNOOZELLIK IC WHERE IC.PRSICILNO=A.PRSICILNO FOR XML RAW, ELEMENTS xsinil, TYPE) "; //ORDER BY IC.ISLEMTARIH
            }

            string sql = "SELECT " + alanlar + " FROM TASSICILNOHAREKET A INNER JOIN TASHESAPPLAN HP ON A.HESAPPLANKOD = HP.KOD";
            sql += " INNER JOIN TASMUHASEBE ON A.MUHASEBEKOD = TASMUHASEBE.KOD";
            sql += " INNER JOIN TASHARCAMABIRIM ON A.MUHASEBEKOD = TASHARCAMABIRIM.MUHASEBEKOD AND A.HARCAMABIRIMKOD = TASHARCAMABIRIM.KOD";

            //(1)Cumhurbaşkanlığında kitaplar çok fazla olduğu için kilitlnme oluyor. Bu nedenle konuldu
            //ama bu sefer raporlarda amortisman hesaplanamyan malzemeler gözükmedi. Hesap kodu kriteri konuldu Melih Hüseyin 26.01.2023
            //sql += " INNER JOIN TASSICILNOOZELLIK OZ ON OZ.PRSICILNO = A.PRSICILNO";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "YIL", "<=", ak.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "DURUM", "<>", (int)ENUMSicilDurum.IPTAL));

            //(2)Cumhurbaşkanlığında kitaplar çok fazla olduğu için kilitlnme oluyor. Bu nedenle konuldu
            //ama bu sefer raporlarda amortisman hesaplanamyan malzemeler gözükmedi.Hesap kodu kriteri konuldu Melih Hüseyin 26.01.2023
            //sql = VeriErisim.KosulEkle(sql, "AND", "OZ.AMORTISMANYIL IS NOT NULL AND OZ.AMORTISMANYIL>0");

            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
            {
                if (ak.hesapPlanKod.StartsWith("@"))
                {
                    ak.hesapPlanKod = ak.hesapPlanKod.Replace("@", "");
                    string[] aranan = ak.hesapPlanKod.Split(' ');
                    string ekKosul = "";
                    for (int i = 0; i < aranan.Length; i++)
                    {
                        if (aranan[i] == "") continue;
                        ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("A", "HESAPPLANKOD", "LIKE", aranan[i].Replace(".", "") + "%"));
                    }
                    if (ekKosul != "")
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            }

            if (TNS.TMM.Arac.CumhurbaskanligiKullaniyor())
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HESAPPLANKOD", "NOT LIKE", "2550702%"));

            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "PRSICILNO", "=", ak.prSicilNo));

            if (ak.prSicilNolar != null && ak.prSicilNolar.Count > 0)
            {
                string ekKosul = "";
                foreach (string prs in ak.prSicilNolar)
                {
                    if (prs == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("A", "PRSICILNO", "=", prs));
                }
                if (ekKosul != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
            }

            sql = VeriErisim.KosulEkle(sql, "AND", BENDEKISICILBUL("A", ak.yil, ak.muhasebeKod, ak.harcamaKod, ak.ambarKod));
            sql = VeriErisim.KosulEkle(sql, "AND", "A.SIRANO = " + SIRANOSQL("A", true, true, ak.yil, false));


            //TEST PRSICILNO
            string testPrSicilNo = OrtakFonksiyonlar.WebConfigOku("TasinirMalTestPrSicilNo", "");
            if (!string.IsNullOrWhiteSpace(testPrSicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", "A.PRSICILNO IN (" + testPrSicilNo + ")");


            if (Arac.MerkezBankasiKullaniyor())
                sql = sql.Replace("TASODA", "YERLESKEODA");

            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }


        public IDataReader HesapPlaninaGoreDemirbasSayilariveAmortismanlari(AmortismanKriter ak)
        {
            string alanlar = "SUBSTRING(HP.KOD, 0, 4) AS HESAPKOD, HP2.ACIKLAMA";
            //alanlar += "SUM(H.KDVLIBIRIMFIYAT + ISNULL(DA.TUTAR, 0)) AS SONBEDEL, SUM(A.AMORTISMANMIKTAR + ISNULL(AA.AMORTISMANMIKTAR, 0)) AS AMORTISMANMIKTAR ";
            //alanlar += ", COUNT(*) AS ADET , SUM(H.KDVLIBIRIMFIYAT) AS ILKBEDEL " +
            //   ", SUM(ISNULL(DA.TUTAR,0)) AS BEDELDUZELTME " +
            //   ", SUM(A.AMORTISMANMIKTAR) AS AMORTISMANMIKTAR " +
            //   ", SUM(ISNULL(AA.AMORTISMANMIKTAR,0)) AS DEGERARTISAMORTISMAN ";

            alanlar += ", (H.KDVLIBIRIMFIYAT) AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5) AS BEDELDUZELTME ";
            //alanlar += ", (A.AMORTISMANMIKTAR) AS AMORTISMANMIKTAR ";
            //alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE (IC.PRSICILNO = A.PRSICILNO AND IC.AMORTISMANTUR <> 1)) AS DEGERARTISAMORTISMAN ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 1[A])) AS AMORTISMANMIKTAR ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 1[A])) AS AMORTISMANDUZELTME ";

            string sql = "SELECT " + alanlar + " FROM TASDEMIRBASVIEW AS H ";
            //sql += "LEFT OUTER JOIN TASAMORTISMAN AS A ON H.PRSICILNO = A.PRSICILNO ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD = HP.KOD ";
            sql += "INNER JOIN TASHESAPPLAN AS HP2 ON SUBSTRING(HP.KOD, 0, 4) = HP2.KOD ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", "ISNULL(A.AMORTISMANTUR, 0) <=" + 1);

            if (ak.sadeceCariYil)
            {
                string amortismanKosul = "";
                amortismanKosul = VeriErisim.KosulEkle(amortismanKosul, "AND", VeriErisim.KosulOl("IC", "YIL", "=", ak.yil));
                amortismanKosul = VeriErisim.KosulEkle(amortismanKosul, "AND", VeriErisim.KosulOl("IC", "DONEM", "=", ak.donem));
                sql = sql.Replace("[A]", " " + amortismanKosul);
            }
            else if (ak.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", "<=", ak.yil));

            if (!ak.sadeceCariYil)
                sql = sql.Replace("[A]", "");

            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO =" + ak.prSicilNo);
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SigortaListesi(AmortismanKriter ak)
        {
            string alanlar = "";

            alanlar = "OZ.BULUNDUGUYER, ODA.AD";
            alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5) AS BEDELDUZELTME ";

            string sql = "SELECT " + alanlar + " FROM TASDEMIRBASVIEW AS H ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
            //sql += "INNER JOIN TASODA AS ODA ON ODA.KOD = H.NEREYEGITTI ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));

            if (ak.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", "<=", ak.yil));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));


            if (ak.raporTur == (int)ENUMMBRaporTur.OTOVETABLOLARHARICSIGORTALISTESI)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", "H.HESAPPLANKOD NOT LIKE '075%'"); //075 - ARAÇLAR
                sql = VeriErisim.KosulEkle(sql, "AND", "H.HESAPPLANKOD NOT LIKE '088%'"); //0880106 - SANAT ESERLERİ
            }
            else if (ak.raporTur == (int)ENUMMBRaporTur.GENELSIGORTALISTESI)
            {

                if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                {
                    string baglac = "OR";
                    string like = ak.hesapPlanIslemDurum == 1 ? "LIKE" : "NOT LIKE";
                    if (like.IndexOf("NOT") > -1) baglac = "AND";

                    if (ak.hesapPlanKod.IndexOf("-") > -1)
                    {
                        string[] aranan = ak.hesapPlanKod.Split('-');
                        string ekKosul = "";
                        for (int i = 0; i < aranan.Length; i++)
                        {
                            if (aranan[i] == "") continue;
                            ekKosul = VeriErisim.KosulEkle(ekKosul, baglac, VeriErisim.KosulOl("H", "HESAPPLANKOD", like, aranan[i].Replace(".", "") + "%"));
                        }
                        if (ekKosul != "")
                            sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                    }
                    else
                        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", like, ak.hesapPlanKod.Replace(".", "") + "%"));
                }

            }

            //TEST PRSICILNO
            string testPrSicilNo = OrtakFonksiyonlar.WebConfigOku("TasinirMalTestPrSicilNo", "");
            if (!string.IsNullOrWhiteSpace(testPrSicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO IN (" + testPrSicilNo + ")");

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader MalzemeAmortismanRaporu(AmortismanKriter ak)
        {
            string alanlar = "";
            string sql = "";

            alanlar = "OZ.PRSICILNO, H.SICILNO, OZ.AMORTISMANYIL ";
            alanlar += ", H1.FISTARIH AS ALIMTARIHI "; //(SELECT FISTARIH FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO=H.PRSICILNO AND IC.SIRANO = (SELECT MIN(SIRANO) FROM TASSICILNOHAREKET IC2 WHERE IC2.PRSICILNO = IC.PRSICILNO AND IC2.ISLEMTURUKOD < 50 AND IC2.DURUM=1) ) AS ALIMTARIHI";
            alanlar += ", OZ.DISSICILNO, OZ.DISSICILNO2, OZ.BISNO, OZ.ESKIBISNO1, ";
            alanlar += "HP.KOD, HP.ACIKLAMA, MARKA.AD AS MARKAAD, MODEL.AD AS MODELAD, OZ.BULUNDUGUYER, ODA.AD, H.HARCAMABIRIMKOD, AMBAR.AD ";

            alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR <> 1) AS BEDELDUZELTME ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR = 1) AS ENFLASYONDUZELTME ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 1)) AS AMORTISMANMIKTAR ";
            //alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 1)) AS AMORTISMANDUZELTME ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 3)) AS AMORTISMANBEDEL ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2)) AS AMORTISMANEDF ";

            alanlar += ", OZ.PLAKA, OZ.ADI, OZ.YERIKONUSU, OZ.SASENO, OZ.EKNO, OZ.GIAI";
            alanlar += ", TIP.KOD, TIP.TUR, TIP.AD";
            //alanlar += ", TIF.FATURANO, TIF.FATURATARIH";

            if (ak.raporTur == (int)ENUMMBRaporTur.DEMIRBASDEFTERI)
            {
                alanlar += ", TASISLEMFORM.FATURANO, TASISLEMFORM.FATURATARIH, TASISLEMFORM.NEREDENGELDI";
            }

            sql = "SELECT " + alanlar + " FROM TASDEMIRBASVIEW AS H ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
            sql += "LEFT OUTER JOIN TASAMBAR AS AMBAR ON AMBAR.MUHASEBEKOD = H.MUHASEBEKOD AND AMBAR.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND AMBAR.KOD = H.AMBARKOD ";
            //sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = H.NEREYEGITTI ";
            sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";
            sql += "LEFT OUTER JOIN TASMARKA AS MARKA ON MARKA.KOD = OZ.MARKAKOD ";
            sql += "LEFT OUTER JOIN TASMODEL AS MODEL ON MARKA.KOD = MODEL.MARKAKOD AND MODEL.KOD = OZ.MODELKOD ";
            //sql += "INNER JOIN TASISLEMFORM TIF ON H.YIL = TIF.YIL AND H.MUHASEBEKOD = TIF.MUHASEBEKOD AND H.HARCAMABIRIMKOD = TIF.HARCAMABIRIMKOD AND H.FISNO = TIF.FISNO ";
            sql += "INNER JOIN TASSICILNOHAREKET H1 ON H1.PRSICILNO = H.PRSICILNO AND H1.SIRANO = 1 ";
            sql += "INNER JOIN TASISLEMTIP AS TIP ON TIP.KOD = H.ISLEMTIPIKOD ";


            if (ak.altOdalar && OrtakFonksiyonlar.ConvertToInt(ak.nereyeGitti, 0) > 0)
            {
                sql += " INNER JOIN dbo.TASODAALT(" + OrtakFonksiyonlar.ConvertToInt(ak.nereyeGitti, 0) + ") TASODAALT ON  CAST(TASODAALT.KOD as varchar(100)) = OZ.BULUNDUGUYER ";
            }

            if (ak.raporTur == (int)ENUMMBRaporTur.DEMIRBASDEFTERI)
            {
                sql += @"LEFT OUTER JOIN TASISLEMDETAY ON TASISLEMDETAY.YIL = H1.YIL AND TASISLEMDETAY.MUHASEBEKOD = H1.MUHASEBEKOD AND TASISLEMDETAY.HARCAMABIRIMKOD = H1.HARCAMABIRIMKOD AND TASISLEMDETAY.FISNO = H1.FISNO
                        AND TASISLEMDETAY.SIRANO = H1.FISSIRANO AND TASISLEMDETAY.HESAPPLANKOD = H1.HESAPPLANKOD AND H1.ISLEMTIPIKOD = 1
                        LEFT OUTER JOIN TASISLEMFORM ON TASISLEMFORM.YIL = TASISLEMDETAY.YIL AND TASISLEMFORM.MUHASEBEKOD = TASISLEMDETAY.MUHASEBEKOD AND TASISLEMFORM.HARCAMABIRIMKOD = TASISLEMDETAY.HARCAMABIRIMKOD AND TASISLEMFORM.FISNO = TASISLEMDETAY.FISNO
                        ";
            }


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));

            if (ak.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", "<=", ak.yil));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
            {
                if (ak.ambarKod.Contains(";"))
                {
                    string ekKosul = "";
                    string[] kodlar = ak.ambarKod.Split(';');
                    foreach (var kod in kodlar)
                    {
                        if (!string.IsNullOrWhiteSpace(kod))
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("H", "AMBARKOD", "=", kod));
                    }
                    if (!string.IsNullOrWhiteSpace(ekKosul))
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            }
            if (!string.IsNullOrWhiteSpace(ak.nereyeGitti) && !ak.altOdalar)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("OZ", "BULUNDUGUYER", "=", ak.nereyeGitti));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO =" + ak.prSicilNo);
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));
            //TEST PRSICILNO
            string testPrSicilNo = OrtakFonksiyonlar.WebConfigOku("TasinirMalTestPrSicilNo", "");
            if (!string.IsNullOrWhiteSpace(testPrSicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO IN (" + testPrSicilNo + ")");

            sql += " ORDER BY ";
            if (ak.raporTur == (int)ENUMMBRaporTur.BIRIMMALZEMELISTESIAMOTRISMANLI)
                sql += "OZ.AMORTISMANYIL, ALIMTARIHI, H.SICILNO, HP.ACIKLAMA";
            else if (ak.raporTur == (int)ENUMMBRaporTur.BIRIMMALZEMELISTESI)
                sql += "H.HESAPPLANKOD, ALIMTARIHI, H.SICILNO, HP.ACIKLAMA";
            else if (ak.raporTur == (int)ENUMMBRaporTur.BIRMALZEMETURUNEGORE)
                sql += "H.HESAPPLANKOD, ALIMTARIHI, H.SICILNO, ODA.AD";
            else if (ak.raporTur == (int)ENUMMBRaporTur.DEMIRBASDEFTERI)
                sql += "H.SICILNO, ALIMTARIHI";
            else if (ak.raporTur == (int)ENUMMBRaporTur.MUHASEBEBAKIYEKARSILASTIRMA)
                sql += "H.SICILNO, ALIMTARIHI";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YilSonuAmortismanRaporu(AmortismanKriter ak)
        {
            string alanlar = "";
            string sql = "";

            alanlar = "OZ.PRSICILNO, H.SICILNO, H.DISSICILNO, OZ.AMORTISMANYIL ";
            alanlar += ",(SELECT FISTARIH FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO=H.PRSICILNO AND IC.SIRANO = 1 ) AS ALIMTARIHI";
            alanlar += ", OZ.DISSICILNO, OZ.DISSICILNO2, OZ.BISNO, OZ.ESKIBISNO1, ";
            alanlar += "HP.KOD, HP.ACIKLAMA, MARKA.AD AS MARKAAD, MODEL.AD AS MODELAD, OZ.BULUNDUGUYER, ODA.AD ";

            alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            //alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5) AS BEDELDUZELTME ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR <> 1) AS BEDELDUZELTME ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR = 1) AS ENFLASYONDUZELTME ";
            //alanlar += ", A.AMORTISMANMIKTAR AS AMORTISMANMIKTAR ";
            //alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE (IC.PRSICILNO = A.PRSICILNO AND IC.AMORTISMANTUR <> 1)) AS DEGERARTISAMORTISMAN ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 1)) AS AMORTISMANMIKTAR ";
            //alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 1[A])) AS AMORTISMANDUZELTME ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 3)) AS AMORTISMANBEDEL ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2)) AS AMORTISMANEDF ";

            alanlar += ", OZ.PLAKA, OZ.ADI,OZ.YERIKONUSU,OZ.SASENO,OZ.EKNO";

            sql = "SELECT " + alanlar + " FROM TASDEMIRBASVIEW AS H ";
            //sql += "INNER JOIN TASAMORTISMAN AS A ON H.PRSICILNO = A.PRSICILNO ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
            //sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = H.NEREYEGITTI ";
            sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";
            sql += "LEFT OUTER JOIN TASMARKA AS MARKA ON MARKA.KOD = OZ.MARKAKOD ";
            sql += "LEFT OUTER JOIN TASMODEL AS MODEL ON MARKA.KOD = MODEL.MARKAKOD AND MODEL.KOD = OZ.MODELKOD ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", "ISNULL(A.AMORTISMANTUR, 0) <=" + 1);
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", "<=", ak.yil));

            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));

            if (ak.yil > 0 && ak.donem > 0)
            {
                string amortismanKosul = "";
                amortismanKosul = VeriErisim.KosulEkle(amortismanKosul, "AND", VeriErisim.KosulOl("IC", "YIL", "<=", ak.yil));
                amortismanKosul = VeriErisim.KosulEkle(amortismanKosul, "AND", VeriErisim.KosulOl("IC", "DONEM", "<=", ak.donem));
                sql = sql.Replace("[A]", " AND " + amortismanKosul);
            }
            else
                sql = sql.Replace("[A]", "");

            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO =" + ak.prSicilNo);
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));

            //TEST PRSICILNO
            string testPrSicilNo = OrtakFonksiyonlar.WebConfigOku("TasinirMalTestPrSicilNo", "");
            if (!string.IsNullOrWhiteSpace(testPrSicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO IN (" + testPrSicilNo + ")");

            sql += " ORDER BY HP.ACIKLAMA, ODA.AD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader IhracListesi(AmortismanKriter ak)
        {
            if (ak.raporTurEkBilgi.StartsWith("2")      //İhraç edilecek
                || ak.raporTurEkBilgi.StartsWith("4"))   //İhraç edilecek (geçici)
            {
                return IhracEdilecekler(ak, ak.raporTurEkBilgi);
            }
            else if (ak.raporTurEkBilgi.StartsWith("3"))  //İhraç edilmiş (geçici)
            {
                return IhracEdilecekler(ak, ak.raporTurEkBilgi);
            }
            else
            {
                string alanlar = "";
                string sql = "";

                alanlar = "OZ.PRSICILNO, H.SICILNO, OZ.AMORTISMANYIL ";
                alanlar += ",(SELECT FISTARIH FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO=H.PRSICILNO AND IC.SIRANO = 1 ) AS ALIMTARIHI";
                alanlar += ", OZ.DISSICILNO, OZ.DISSICILNO2, OZ.BISNO, OZ.ESKIBISNO1, ";
                alanlar += "HP.KOD, HP.ACIKLAMA, MARKA.AD AS MARKAAD, MODEL.AD AS MODELAD, OZ.BULUNDUGUYER, ODA.AD ";

                alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
                alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR <> 1) AS BEDELDUZELTME ";
                alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR = 1) AS ENFLASYONDUZELTME ";
                alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 1)) AS AMORTISMANMIKTAR ";
                //alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 1)) AS AMORTISMANDUZELTME ";
                alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 3)) AS AMORTISMANBEDEL ";
                alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2)) AS AMORTISMANEDF ";

                alanlar += ", OZ.PLAKA, OZ.ADI,OZ.YERIKONUSU,OZ.SASENO,OZ.EKNO, H2.HARCAMABIRIMKOD + '-' + H2.FISNO, H2.FISTARIH ";


                sql = "SELECT " + alanlar + " FROM TASSICILNOHAREKET AS H ";
                sql += "INNER JOIN TASSICILNOHAREKET AS H2 ON H.PRSICILNO = H2.PRSICILNO AND H2.ISLEMTURUKOD IN ({ISLEMTURUKOD}) ";
                sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
                sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
                sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";
                sql += "LEFT OUTER JOIN TASMARKA AS MARKA ON MARKA.KOD = OZ.MARKAKOD ";
                sql += "LEFT OUTER JOIN TASMODEL AS MODEL ON MARKA.KOD = MODEL.MARKAKOD AND MODEL.KOD = OZ.MODELKOD ";

                sql = VeriErisim.KosulEkle(sql, "WHERE", "H.SIRANO=" + 1);
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "MUHASEBEKOD", "=", ak.muhasebeKod));


                if (ak.raporTurEkBilgi.Contains("satislariDahilEt"))
                    sql = sql.Replace("{ISLEMTURUKOD}", "56,53"); //İhraç ve Satış

                else
                    sql = sql.Replace("{ISLEMTURUKOD}", "56");  //İhraç

                if (ak.yil > 0)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "YIL", "=", ak.yil));
                if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
                if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "AMBARKOD", "=", ak.ambarKod));
                if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
                if (!string.IsNullOrEmpty(ak.belgeNo))
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "FISNO", "=", ak.belgeNo));
                if (!ak.baslamaTarihi.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "FISTARIH", ">=", ak.baslamaTarihi));
                if (!ak.bitisTarihi.isNull)
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "FISTARIH", "<=", ak.bitisTarihi));

                //TEST PRSICILNO
                string testPrSicilNo = OrtakFonksiyonlar.WebConfigOku("TasinirMalTestPrSicilNo", "");
                if (!string.IsNullOrWhiteSpace(testPrSicilNo))
                    sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO IN (" + testPrSicilNo + ")");

                sql += " ORDER BY ";
                if (ak.raporTur == (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI)
                    sql += "OZ.AMORTISMANYIL, ALIMTARIHI, H.SICILNO";
                else if (ak.raporTur == (int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU)
                    sql += "HP.ACIKLAMA";


                return veriErisim.DataReaderDondur(sql);
            }

        }

        private IDataReader IhracEdilecekler(AmortismanKriter ak, string raporTurEkBilgi)
        {
            string alanlar = "";
            string sql = "";

            alanlar = "OZ.PRSICILNO, H.SICILNO, OZ.AMORTISMANYIL ";
            alanlar += ",(SELECT FISTARIH FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.SIRANO = 1 ) AS ALIMTARIHI ";
            alanlar += ", OZ.DISSICILNO, OZ.DISSICILNO2, OZ.BISNO, OZ.ESKIBISNO1, HP.KOD, HP.ACIKLAMA, MARKA.AD AS MARKAAD, MODEL.AD AS MODELAD, OZ.BULUNDUGUYER, ODA.AD ";

            alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR <> 1) AS BEDELDUZELTME ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR = 1) AS ENFLASYONDUZELTME ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 1)) AS AMORTISMANMIKTAR ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 3)) AS AMORTISMANBEDEL ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2)) AS AMORTISMANEDF ";

            alanlar += ", OZ.PLAKA, OZ.ADI,OZ.YERIKONUSU,OZ.SASENO,OZ.EKNO, H.HARCAMABIRIMKOD + '-' + F.FISNO, F.FISTARIH ";

            sql = "SELECT " + alanlar + " FROM TASISLEMFORM AS F ";

            //sql += "INNER JOIN TASSICILNOHAREKET H ON H.YIL = F.YIL AND H.MUHASEBEKOD = F.MUHASEBEKOD AND H.HARCAMABIRIMKOD = F.HARCAMABIRIMKOD AND H.AMBARKOD = F.AMBARKOD ";
            //sql += "AND H.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC2 WHERE IC2.PRSICILNO = H.PRSICILNO AND IC2.ISLEMTURUKOD < 50 AND IC2.DURUM = 1) ";

            sql += "INNER JOIN TASISLEMDETAY D ON D.YIL = F.YIL AND D.MUHASEBEKOD = F.MUHASEBEKOD AND D.HARCAMABIRIMKOD = F.HARCAMABIRIMKOD AND D.FISNO = F.FISNO ";
            //sql += "AND D.SICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASSICILNOHAREKET H ON D.SICILNO = H.PRSICILNO ";

            sql += "INNER JOIN TASISLEMTIP AS TIP ON F.ISLEMTIPIKOD = TIP.KOD ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD = HP.KOD ";
            //sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = H.NEREYEGITTI ";
            sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";
            sql += "LEFT OUTER JOIN TASMARKA AS MARKA ON MARKA.KOD = OZ.MARKAKOD ";
            sql += "LEFT OUTER JOIN TASMODEL AS MODEL ON MARKA.KOD = MODEL.MARKAKOD AND MODEL.KOD = OZ.MODELKOD ";


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("F", "YIL", "=", ak.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "MUHASEBEKOD", "=", ak.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TIP", "TUR", "=", (int)ENUMIslemTipi.DEVIRCIKISKURUM)); //İhraç işlemi için

            if (raporTurEkBilgi.StartsWith("3")) //İhraç edilmiş (geçici)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "DURUM", "=", 5)); //Onaylı ihraç (geçici)
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "DURUM", "<", 5)); //Onaysız ve iptal edilmemiş


            if (raporTurEkBilgi.StartsWith("4") || raporTurEkBilgi.StartsWith("3"))//İhraç edilecek (geçici)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TIP", "KOD", "=", 24)); //İhraç (Geçici)
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("TIP", "KOD", "=", 23)); //İhraç

            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "AMBARKOD", "=", ak.ambarKod));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (!string.IsNullOrEmpty(ak.belgeNo))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "FISNO", "=", ak.belgeNo));
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("F", "FISTARIH", "<=", ak.bitisTarihi));

            sql += "AND H.SIRANO = (SELECT MAX(IC2.SIRANO) FROM TASSICILNOHAREKET IC2 WHERE IC2.PRSICILNO = H.PRSICILNO AND IC2.ISLEMTURUKOD < 50 AND IC2.DURUM = 1) ";

            if (raporTurEkBilgi.StartsWith("3B"))
                sql += " AND ISNULL((SELECT COUNT(*) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.ISLEMTIPIKOD = 23 AND IC.DURUM = 1),0) < 1 ";

            sql += " ORDER BY ";
            if (ak.raporTur == (int)ENUMMBRaporTur.IHRACLISTESIAMORTISMANLI)
                sql += "OZ.AMORTISMANYIL, ALIMTARIHI, H.SICILNO";
            else if (ak.raporTur == (int)ENUMMBRaporTur.IHRACLISTESIMALZEMETURU)
                sql += "HP.ACIKLAMA";


            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YilBazindaAmortismanRaporu(AmortismanKriter ak)
        {
            string sql = "SELECT H.YIL,H.PRSICILNO, H.KDVLIBIRIMFIYAT + H.DEGERARTISI AS BEDEL ";
            sql += ", A.AMORTISMANMIKTAR, A.YIL, A.DONEM, OZ.AMORTISMANYIL ";
            sql += "FROM TASAMORTISMAN A ";
            sql += "INNER JOIN TASSICILNOHAREKET H ON H.PRSICILNO = A.PRSICILNO and H.SIRANO = 1 ";
            sql += "INNER JOIN TASSICILNOHAREKET H2 ON H2.PRSICILNO = A.PRSICILNO ";
            sql += "AND H2.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = A.PRSICILNO AND (IC.ISLEMTURUKOD < 50 OR IC.ISLEMTURUKOD = 90  OR IC.ISLEMTURUKOD = 51) AND DURUM=1) ";
            sql += "INNER JOIN TASSICILNOOZELLIK OZ ON OZ.PRSICILNO = A.PRSICILNO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", ak.muhasebeKod));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "AMBARKOD", "=", ak.ambarKod));
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));

            sql += " ORDER BY ";


            if (ak.raporTur == (int)ENUMMBRaporTur.ORANBAZINDATAMAMIAMORTIEDILMIS || ak.raporTur == (int)ENUMMBRaporTur.YILBAZINDATAMAMIAMORTIEDILMISLER)
                sql += "OZ.AMORTISMANYIL, H.PRSICILNO";
            else
                sql += "H.YIL, H.PRSICILNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader YerleskeYerineGoreToplamAmortismanRaporu(AmortismanKriter ak)
        {
            string alanlar = "";

            alanlar = "OZ.BULUNDUGUYER, ODA.AD";
            alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5) AS BEDELDUZELTME ";
            //alanlar += ", A.AMORTISMANMIKTAR AS AMORTISMANMIKTAR ";
            //alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE (IC.PRSICILNO = A.PRSICILNO AND IC.AMORTISMANTUR <> 1)) AS DEGERARTISAMORTISMAN ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 1)) AS AMORTISMANMIKTAR ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 1)) AS AMORTISMANDUZELTME ";

            string sql = "SELECT " + alanlar + " FROM TASDEMIRBASVIEW AS H ";
            //sql += "LEFT OUTER JOIN TASAMORTISMAN AS A ON H.PRSICILNO = A.PRSICILNO ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
            //sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.NEREYEGITTI ";
            sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));
            //sql = VeriErisim.KosulEkle(sql, "AND", "ISNULL(A.AMORTISMANTUR, 0) <=" + 1);

            if (ak.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", "<=", ak.yil));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            if (!string.IsNullOrWhiteSpace(ak.nereyeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("OZ", "BULUNDUGUYER", "=", ak.nereyeGitti));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));

            //TEST PRSICILNO
            string testPrSicilNo = OrtakFonksiyonlar.WebConfigOku("TasinirMalTestPrSicilNo", "");
            if (!string.IsNullOrWhiteSpace(testPrSicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO IN (" + testPrSicilNo + ")");

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader SosyalTesislereAyrilanCariAmortismanRaporu(AmortismanKriter ak)
        {
            string sql = "SELECT ODA.AD AS YERLESKEAD, OZ.BULUNDUGUYER AS YERLESKEKOD, SUM(A.AMORTISMANMIKTAR) AS CARIAMORTISMAN ";
            sql += "FROM TASAMORTISMAN A ";
            sql += "INNER JOIN TASSICILNOHAREKET H ON H.PRSICILNO = A.PRSICILNO and H.SIRANO = 1 ";
            sql += "INNER JOIN TASSICILNOHAREKET H2 ON H2.PRSICILNO = A.PRSICILNO ";
            sql += "AND H2.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = A.PRSICILNO AND(IC.ISLEMTURUKOD < 50 OR IC.ISLEMTURUKOD = 90 OR IC.ISLEMTURUKOD = 51)) ";
            sql += "INNER JOIN TASSICILNOOZELLIK OZ ON OZ.PRSICILNO = A.PRSICILNO ";
            //sql += "INNER JOIN TASODA AS ODA ON ODA.KOD = H2.NEREYEGITTI ";
            sql += "INNER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "YIL", "=", ak.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", ak.muhasebeKod));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "AMBARKOD", "=", ak.ambarKod));
            if (ak.donem > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "DONEM", "=", ak.donem));
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));

            if (ak.nereyeGitti.IndexOf(",") > -1)
            {
                string[] aranan = ak.nereyeGitti.Split(',');
                string ekKosul = "";
                for (int i = 0; i < aranan.Length; i++)
                {
                    if (aranan[i] == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("ODA", "KOD", "LIKE", aranan[i]));
                }
                if (ekKosul != "")
                    sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ODA", "KOD", "LIKE", ak.nereyeGitti));


            sql += " GROUP BY ODA.AD, OZ.BULUNDUGUYER ";
            sql += " ORDER BY ODA.AD";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader GenelAmortismanRaporu(AmortismanKriter ak)
        {
            string sql = "SELECT H.YIL, H.PRSICILNO, H.KDVLIBIRIMFIYAT AS ILKBEDEL, ";
            sql += "(SELECT SUM(TUTAR) FROM TASSICILNODEGERARTIS IC WHERE IC.PRSICILNO = A.PRSICILNO AND IC.DURUM=5) AS DEGERARTISI, ";
            sql += "A.AMORTISMANMIKTAR, A.AMORTISMANTUR, A.YIL, A.DONEM, OZ.AMORTISMANYIL FROM ";

            sql += "TASSICILNOHAREKET H2 ";
            sql += "INNER JOIN TASSICILNOOZELLIK OZ ON H2.PRSICILNO = OZ.PRSICILNO ";
            sql += "INNER JOIN TASSICILNOHAREKET H ON H.PRSICILNO = OZ.PRSICILNO and H.SIRANO = 1 ";
            //sql += "INNER JOIN TASSICILNOHAREKET H2 ON H2.PRSICILNO = OZ.PRSICILNO AND H2.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = OZ.PRSICILNO AND(IC.ISLEMTURUKOD < 50 OR IC.ISLEMTURUKOD = 90) AND DURUM = 1) ";
            sql += "INNER JOIN  TASAMORTISMAN A ON OZ.PRSICILNO = A.PRSICILNO ";

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H2", "MUHASEBEKOD", "=", ak.muhasebeKod));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "AMBARKOD", "=", ak.ambarKod));
            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H2", "PRSICILNO", "=", ak.prSicilNo));

            sql += " AND (H2.ISLEMTURUKOD < 50 OR H2.ISLEMTURUKOD = 90 OR H.ISLEMTURUKOD = 51) AND H2.DURUM = 1 AND H2.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = OZ.PRSICILNO AND(H2.ISLEMTURUKOD < 50 OR H2.ISLEMTURUKOD = 90) AND DURUM = 1) ";

            sql += " ORDER BY ";
            sql += "H.YIL, H.PRSICILNO, A.AMORTISMANTUR, A.YIL";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BirimlereGoreDemirbasAmortisman(AmortismanKriter ak)
        {
            string sql = @"
                        SELECT A.AD, SUM(A.AMORTISMAN) AMORTISMAN, SUM(A.KDVLIBIRIMFIYAT) KDVLIBIRIMFIYAT, SUM(A.DEGERARTISI) DEGERARTISI
                        FROM
                            (SELECT B.KOD,B.AD,H.PRSICILNO, SUM(A.AMORTISMANMIKTAR) AMORTISMAN, H.KDVLIBIRIMFIYAT, H.DEGERARTISI
                            FROM TASSICILNOHAREKET H
                            INNER JOIN TASHARCAMABIRIM B ON B.MUHASEBEKOD = H.MUHASEBEKOD AND B.KOD = H.HARCAMABIRIMKOD
                            LEFT OUTER JOIN TASAMORTISMAN A ON H.PRSICILNO = A.PRSICILNO

                            WHERE {KRITER}

                            GROUP BY B.KOD, B.AD, H.PRSICILNO, H.KDVLIBIRIMFIYAT, H.DEGERARTISI
                            ) A
                        GROUP BY A.KOD, A.AD
                        ORDER BY A.KOD
                            ";


            string kriter = "";

            kriter = VeriErisim.KosulEkle(kriter, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));

            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
            {
                string[] aranan = ak.ambarKod.Split(';');
                string ekKosul = "";
                for (int i = 0; i < aranan.Length; i++)
                {
                    if (aranan[i] == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("H", "AMBARKOD", "=", aranan[i]));
                }
                if (ekKosul != "")
                    kriter = VeriErisim.KosulEkle(kriter, "AND", "(" + ekKosul + ")");

                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            }
            if (ak.prSicilNo > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("H", "PRSICILNO", "=", ak.prSicilNo));

            kriter += " AND H.SIRANO = (SELECT MAX(IC2.SIRANO) FROM TASSICILNOHAREKET IC2 WHERE IC2.PRSICILNO = H.PRSICILNO) ";
            kriter += " AND (H.ISLEMTURUKOD < 50 OR H.ISLEMTURUKOD = 90 OR H.ISLEMTURUKOD = 51) AND H.DURUM = 1 ";
            //51: Merkez bankası  devir çıkış yapmış ancak giriş yapmamış kayıtlar için


            sql = sql.Replace("{KRITER}", kriter);

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader BirimlereGoreGayriMenkulAmortisman(AmortismanKriter ak)
        {
            string sql = @"
                        SELECT A.AD, SUM(A.AMORTISMAN) AMORTISMAN, SUM(A.KDVLIBIRIMFIYAT) KDVLIBIRIMFIYAT, SUM(A.DEGERARTISI) DEGERARTISI
                        FROM
                             (SELECT B.KOD,B.AD,H.PRSICILNO, SUM(A.AMORTISMANMIKTAR) AMORTISMAN, H.KDVLIBIRIMFIYAT, H.DEGERARTISI
                            FROM TASSICILNOHAREKET H
                            INNER JOIN TASHARCAMABIRIM B ON B.MUHASEBEKOD = H.MUHASEBEKOD AND B.KOD = H.HARCAMABIRIMKOD
                            LEFT OUTER JOIN TASAMORTISMAN A ON H.PRSICILNO = A.PRSICILNO

                            WHERE {KRITER}

                            GROUP BY B.KOD, B.AD, H.PRSICILNO, H.KDVLIBIRIMFIYAT, H.DEGERARTISI
                            ) A
                        GROUP BY A.KOD, A.AD
                        ORDER BY A.KOD
                            ";


            string kriter = "";

            kriter = VeriErisim.KosulEkle(kriter, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));

            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
            {
                string[] aranan = ak.ambarKod.Split(';');
                string ekKosul = "";
                for (int i = 0; i < aranan.Length; i++)
                {
                    if (aranan[i] == "") continue;
                    ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("H", "AMBARKOD", "=", aranan[i]));
                }
                if (ekKosul != "")
                    kriter = VeriErisim.KosulEkle(kriter, "AND", "(" + ekKosul + ")");

                //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            }
            if (ak.prSicilNo > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("H", "PRSICILNO", "=", ak.prSicilNo));

            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod));


            kriter += " AND H.SIRANO = (SELECT MAX(IC2.SIRANO) FROM TASSICILNOHAREKET IC2 WHERE IC2.PRSICILNO = H.PRSICILNO) ";
            kriter += " AND (H.ISLEMTURUKOD < 50 OR H.ISLEMTURUKOD = 90 OR H.ISLEMTURUKOD = 51) AND H.DURUM = 1 ";
            //51: Merkez bankası  devir çıkış yapmış ancak giriş yapmamış kayıtlar için


            sql = sql.Replace("{KRITER}", kriter);

            return veriErisim.DataReaderDondur(sql);
        }

        //public IDataReader AmortismanTamamlanmisMi(AmortismanKriter int[] prSilcilNoListe)
        //{
        //    string sql = "SELECT ODA.AD AS YERLESKEAD, OZ.BULUNDUGUYER AS YERLESKEKOD, SUM(A.AMORTISMANMIKTAR) AS CARIAMORTISMAN ";
        //    sql += "FROM TASAMORTISMAN A ";
        //    sql += "INNER JOIN TASSICILNOHAREKET H ON H.PRSICILNO = A.PRSICILNO and H.SIRANO = 1 ";
        //    sql += "INNER JOIN TASSICILNOHAREKET H2 ON H2.PRSICILNO = A.PRSICILNO ";
        //    sql += "AND H2.SIRANO = (SELECT MAX(SIRANO) FROM TASSICILNOHAREKET IC WHERE IC.PRSICILNO = A.PRSICILNO AND(IC.ISLEMTURUKOD < 50 OR IC.ISLEMTURUKOD = 90 OR IC.ISLEMTURUKOD = 51)) ";
        //    sql += "INNER JOIN TASSICILNOOZELLIK OZ ON OZ.PRSICILNO = A.PRSICILNO ";
        //    //sql += "INNER JOIN TASODA AS ODA ON ODA.KOD = H2.NEREYEGITTI ";
        //    sql += "INNER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";


        //    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", "YIL", "=", ak.yil));
        //    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "MUHASEBEKOD", "=", ak.muhasebeKod));
        //    if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
        //    if (!string.IsNullOrWhiteSpace(ak.ambarKod))
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "AMBARKOD", "=", ak.ambarKod));
        //    if (ak.donem > 0)
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", "DONEM", "=", ak.donem));
        //    if (!ak.baslamaTarihi.isNull)
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
        //    if (!ak.bitisTarihi.isNull)
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));

        //    if (ak.nereyeGitti.IndexOf(",") > -1)
        //    {
        //        string[] aranan = ak.nereyeGitti.Split(',');
        //        string ekKosul = "";
        //        for (int i = 0; i < aranan.Length; i++)
        //        {
        //            if (aranan[i] == "") continue;
        //            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("ODA", "KOD", "LIKE", aranan[i]));
        //        }
        //        if (ekKosul != "")
        //            sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
        //    }
        //    else
        //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("ODA", "KOD", "LIKE", ak.nereyeGitti));


        //    sql += " GROUP BY ODA.AD, OZ.BULUNDUGUYER ";
        //    sql += " ORDER BY ODA.AD";

        //    return veriErisim.DataReaderDondur(sql);
        //}


        public IDataReader EnflasyonDuzeltmesiAmortismanRaporu(AmortismanKriter ak)
        {
            string alanlar = "";
            string sql = "";

            alanlar = "OZ.PRSICILNO, H.SICILNO, OZ.AMORTISMANYIL ";
            alanlar += ", H1.FISTARIH AS ALIMTARIHI ";
            alanlar += ", OZ.DISSICILNO, OZ.DISSICILNO2, OZ.BISNO, OZ.ESKIBISNO1, ";
            alanlar += "HP.KOD, HP.ACIKLAMA, MARKA.AD AS MARKAAD, MODEL.AD AS MODELAD, OZ.BULUNDUGUYER, ODA.AD, H.HARCAMABIRIMKOD, H.HARCAMABIRIMAD, AMBAR.AD ";

            alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR <> 1) AS BEDELDUZELTME ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR = 1)  AS ENFLASYONDUZELTME ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 2))   AS AMORTISMANMIKTAR ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2))    AS AMORTISMANENFLASYONDUZELTME ";

            alanlar += ", OZ.PLAKA, OZ.ADI, OZ.YERIKONUSU, OZ.SASENO, OZ.EKNO, OZ.GIAI";
            alanlar += ", TIP.KOD, TIP.TUR, TIP.AD, AMBAR.KOD, H.KDVORAN";


            sql = "SELECT " + alanlar + " FROM TASDEMIRBASVIEW AS H ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
            sql += "LEFT OUTER JOIN TASAMBAR AS AMBAR ON AMBAR.MUHASEBEKOD = H.MUHASEBEKOD AND AMBAR.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND AMBAR.KOD = H.AMBARKOD ";
            //sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = H.NEREYEGITTI ";
            sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";
            sql += "LEFT OUTER JOIN TASMARKA AS MARKA ON MARKA.KOD = OZ.MARKAKOD ";
            sql += "LEFT OUTER JOIN TASMODEL AS MODEL ON MARKA.KOD = MODEL.MARKAKOD AND MODEL.KOD = OZ.MODELKOD ";
            //sql += "INNER JOIN TASISLEMFORM TIF ON H.YIL = TIF.YIL AND H.MUHASEBEKOD = TIF.MUHASEBEKOD AND H.HARCAMABIRIMKOD = TIF.HARCAMABIRIMKOD AND H.FISNO = TIF.FISNO ";
            sql += "INNER JOIN TASSICILNOHAREKET H1 ON H1.PRSICILNO = H.PRSICILNO AND H1.SIRANO = 1 ";
            sql += "INNER JOIN TASISLEMTIP AS TIP ON TIP.KOD = H.ISLEMTIPIKOD ";


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));

            // ************** Enflasyon artışı yapılacakların koşulları BAŞLA

            //52000 ve üstü fiyatı olanlar
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "KDVLIBIRIMFIYAT", ">=", 52000));

            //Tarihi eserler hariç
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "NOT LIKE", "25506%"));

            //Bazı hesap kodları 2023 yılında geçerli önceki yıllarda gelmesin
            sql += " AND (";
            sql += " (H1.FISTARIH <= {ts '2022-12-31 00:00:00.000'} ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550401%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550501%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550502%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550503%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550504%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551001%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551002%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551003%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551101%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551102%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551103%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551201%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551202%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2559903%')";
            sql += " OR ";
            sql += " (H1.FISTARIH >= {ts '2023-01-01 00:00:00.000'} AND H.HESAPPLANKOD LIKE '25%')";
            sql += " )";


            //5 yıl öncesine kadar = 2023-2019 yılları arasında 253 ve 255 için
            string icKosul1 = "";
            icKosul1 = VeriErisim.KosulEkle(icKosul1, "AND", VeriErisim.KosulOl("H1", "FISTARIH", ">=", new TNSDateTime("01.01.2019")));
            icKosul1 = VeriErisim.KosulEkle(icKosul1, "AND", VeriErisim.KosulOl("H1", "FISTARIH", "<=", new TNSDateTime("31.12.2023")));

            string icKosul11 = "";
            icKosul11 = VeriErisim.KosulEkle(icKosul11, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", "255%"));
            icKosul11 = VeriErisim.KosulEkle(icKosul11, "OR", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", "253%"));
            icKosul11 = "(" + icKosul11 + ")";
            icKosul1 = "(" + icKosul1 + " AND " + icKosul11 + ")";

            //10 yıl öncesine kadar = 2023-2014 yılları arasında 254 (araç) için
            string icKosul2 = "";
            icKosul2 = VeriErisim.KosulEkle(icKosul2, "AND", VeriErisim.KosulOl("H1", "FISTARIH", ">=", new TNSDateTime("01.01.2014")));
            icKosul2 = VeriErisim.KosulEkle(icKosul2, "AND", VeriErisim.KosulOl("H1", "FISTARIH", "<=", new TNSDateTime("31.12.2023")));
            icKosul2 = VeriErisim.KosulEkle(icKosul2, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", "254%"));
            icKosul2 = "(" + icKosul2 + ")";

            string icKosul = "(" + icKosul1 + " OR " + icKosul2 + ")";
            sql = VeriErisim.KosulEkle(sql, "AND", icKosul);
            // ************** Enflasyon artışı yapılacakların koşulları BİTİŞ



            if (ak.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", "<=", ak.yil));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
            {
                if (ak.ambarKod.Contains(";"))
                {
                    string ekKosul = "";
                    string[] kodlar = ak.ambarKod.Split(';');
                    foreach (var kod in kodlar)
                    {
                        if (!string.IsNullOrWhiteSpace(kod))
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("H", "AMBARKOD", "=", kod));
                    }
                    if (!string.IsNullOrWhiteSpace(ekKosul))
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            }
            if (!string.IsNullOrWhiteSpace(ak.nereyeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("OZ", "BULUNDUGUYER", "=", ak.nereyeGitti));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO =" + ak.prSicilNo);
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));


            sql += " ORDER BY ";
            if (ak.raporTur == (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI)
                sql += "ALIMTARIHI, H.HESAPPLANKOD, H.SICILNO";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EnflasyonDuzeltmesiAmortismanRaporu2025(AmortismanKriter ak, bool raporMu)
        {
            string alanlar = "";
            string sql = "";

            alanlar = "OZ.PRSICILNO, H.SICILNO, OZ.AMORTISMANYIL ";
            alanlar += ", H1.FISTARIH AS ALIMTARIHI ";
            alanlar += ", OZ.DISSICILNO, OZ.DISSICILNO2, OZ.BISNO, OZ.ESKIBISNO1, ";
            alanlar += "HP.KOD, HP.ACIKLAMA, MARKA.AD AS MARKAAD, MODEL.AD AS MODELAD, OZ.BULUNDUGUYER, ODA.AD, H.HARCAMABIRIMKOD, H.HARCAMABIRIMAD, AMBAR.AD ";

            alanlar += ", H1.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR <> 1) AS BEDELDUZELTME ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR = 1)  AS ENFLASYONDUZELTME ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 2))   AS AMORTISMANMIKTAR ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2))    AS AMORTISMANENFLASYONDUZELTME ";

            alanlar += ", EDF.TUTAR AS ENFLASYONDUZELTMECARI ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2) AND IC.DEGERARTISKODU = EDF.KOD) AS AMORTISMANENFLASYONDUZELTMECARI ";
            alanlar += ", EDF.KATSAYI";

            alanlar += ", OZ.PLAKA, OZ.ADI, OZ.YERIKONUSU, OZ.SASENO, OZ.EKNO, OZ.GIAI";
            alanlar += ", TIP.KOD, TIP.TUR, TIP.AD, AMBAR.KOD, H.KDVORAN";

            sql = "SELECT " + alanlar + " FROM TASDEMIRBASVIEW AS H ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
            sql += "LEFT OUTER JOIN TASAMBAR AS AMBAR ON AMBAR.MUHASEBEKOD = H.MUHASEBEKOD AND AMBAR.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND AMBAR.KOD = H.AMBARKOD ";
            //sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = H.NEREYEGITTI ";
            sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";
            sql += "LEFT OUTER JOIN TASMARKA AS MARKA ON MARKA.KOD = OZ.MARKAKOD ";
            sql += "LEFT OUTER JOIN TASMODEL AS MODEL ON MARKA.KOD = MODEL.MARKAKOD AND MODEL.KOD = OZ.MODELKOD ";
            //sql += "INNER JOIN TASISLEMFORM TIF ON H.YIL = TIF.YIL AND H.MUHASEBEKOD = TIF.MUHASEBEKOD AND H.HARCAMABIRIMKOD = TIF.HARCAMABIRIMKOD AND H.FISNO = TIF.FISNO ";
            sql += "INNER JOIN TASSICILNOHAREKET H1 ON H1.PRSICILNO = H.PRSICILNO AND H1.SIRANO = 1 ";
            sql += "INNER JOIN TASISLEMTIP AS TIP ON TIP.KOD = H.ISLEMTIPIKOD ";
            sql += $"LEFT OUTER JOIN TASSICILNODEGERARTIS AS EDF ON EDF.PRSICILNO = H.PRSICILNO AND EDF.MUHASEBEKOD = H.MUHASEBEKOD AND EDF.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND EDF.AMBARKOD = H.AMBARKOD AND EDF.DURUM = 5 AND EDF.TUR = 1 AND YEAR(EDF.BELGETARIHI) = {ak.yil} AND MONTH(EDF.BELGETARIHI) = 12 ";


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));

            // ************** Enflasyon artışı yapılacakların koşulları BAŞLA

            //52000 ve üstü fiyatı olanlar
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "KDVLIBIRIMFIYAT", ">=", 52000));

            //Tarihi eserler hariç
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "NOT LIKE", "25506%"));

            ////Jet Özel Uçaklar hariç
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "NOT LIKE", "25403%"));

            //Amortismanı bitenler hariç
            sql += $" AND YEAR(H1.FISTARIH) + OZ.AMORTISMANYIL > {ak.yil} ";

            //Bazı hesap kodları 2024 yılında geçerli önceki yıllarda gelmesin
            sql += " AND (";
            sql += " (H1.FISTARIH <= {ts '" + (ak.yil - 1) + "-12-31 00:00:00.000'} ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550401%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550501%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550502%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550503%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550504%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551001%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551002%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551003%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551101%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551102%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551103%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551201%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2551202%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2559903%')";
            sql += " OR ";
            sql += " (H1.FISTARIH >= {ts '" + (ak.yil) + "-01-01 00:00:00.000'} AND H.HESAPPLANKOD LIKE '25%')";
            sql += " )";

            //İstisnai olarak amortisman süresi 4 yıl olan 255.01.01 ; 255.01.02 ; 255.01.03; 255.01.04; 255.01.05 hesap kodları için sicil numarasındaki yıl bilgisi 2020 ve öncesi olanlar için
            sql += " AND (";
            sql += " (H1.FISTARIH <= {ts '" + (ak.yil - 4) + "-12-31 00:00:00.000'} ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550101%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550102%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550103%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550104%' ";
            sql += " AND H.HESAPPLANKOD NOT LIKE '2550105%')";
            sql += " OR ";
            sql += " (H1.FISTARIH >= {ts '" + (ak.yil - 3) + "-01-01 00:00:00.000'} AND H.HESAPPLANKOD LIKE '25%')";
            sql += " )";


            //5 yıl öncesine kadar = 2024-2020 yılları arasında 253 ve 255 için
            string icKosul1 = "";
            icKosul1 = VeriErisim.KosulEkle(icKosul1, "AND", VeriErisim.KosulOl("H1", "FISTARIH", ">=", new TNSDateTime("01.01." + (ak.yil - 4))));
            icKosul1 = VeriErisim.KosulEkle(icKosul1, "AND", VeriErisim.KosulOl("H1", "FISTARIH", "<=", new TNSDateTime("31.12." + (ak.yil))));

            string icKosul11 = "";
            icKosul11 = VeriErisim.KosulEkle(icKosul11, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", "255%"));
            icKosul11 = VeriErisim.KosulEkle(icKosul11, "OR", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", "253%"));
            icKosul11 = "(" + icKosul11 + ")";
            icKosul1 = "(" + icKosul1 + " AND " + icKosul11 + ")";

            //10 yıl öncesine kadar = 2024-2015 yılları arasında 254 (araç) için
            string icKosul2 = "";
            icKosul2 = VeriErisim.KosulEkle(icKosul2, "AND", VeriErisim.KosulOl("H1", "FISTARIH", ">=", new TNSDateTime("01.01." + (ak.yil - 9))));
            icKosul2 = VeriErisim.KosulEkle(icKosul2, "AND", VeriErisim.KosulOl("H1", "FISTARIH", "<=", new TNSDateTime("31.12." + (ak.yil))));
            icKosul2 = VeriErisim.KosulEkle(icKosul2, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", "254%"));
            icKosul2 = "(" + icKosul2 + ")";


            string icKosul = "(" + icKosul1 + " OR " + icKosul2 + ")";
            sql = VeriErisim.KosulEkle(sql, "AND", icKosul);
            // ************** Enflasyon artışı yapılacakların koşulları BİTİŞ

            if (ak.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "YIL", "<=", ak.yil));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
            {
                if (ak.ambarKod.Contains(";"))
                {
                    string ekKosul = "";
                    string[] kodlar = ak.ambarKod.Split(';');
                    foreach (var kod in kodlar)
                    {
                        if (!string.IsNullOrWhiteSpace(kod))
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("H", "AMBARKOD", "=", kod));
                    }
                    if (!string.IsNullOrWhiteSpace(ekKosul))
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            }
            if (!string.IsNullOrWhiteSpace(ak.nereyeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("OZ", "BULUNDUGUYER", "=", ak.nereyeGitti));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO =" + ak.prSicilNo);
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "FISTARIH", "<=", ak.bitisTarihi));


            ////TEST
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", "2530204130103%"));


            sql += " ORDER BY ";
            if (ak.raporTur == (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI)
            {
                if (raporMu)
                    sql += "H.HESAPPLANKOD, ALIMTARIHI, H.SICILNO";
                else
                    sql += "ALIMTARIHI, H.HESAPPLANKOD, H.SICILNO";
            }
            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader EnflasyonDuzeltmesiAmortismanRaporuMB(AmortismanKriter ak)
        {
            int yilDonemKontrol = ak.yil * 100 + (ak.donem / 100);

            string alanlar = "";
            string sql = "";

            alanlar = "OZ.PRSICILNO, H.SICILNO, OZ.AMORTISMANYIL ";
            alanlar += ", H1.FISTARIH AS ALIMTARIHI ";
            alanlar += ", OZ.DISSICILNO, OZ.DISSICILNO2, OZ.BISNO, OZ.ESKIBISNO1, ";
            alanlar += "HP.KOD, HP.ACIKLAMA, MARKA.AD AS MARKAAD, MODEL.AD AS MODELAD, OZ.BULUNDUGUYER, ODA.AD, H.HARCAMABIRIMKOD, HB.AD, AMBAR.AD ";

            alanlar += ", EDF.KATSAYI";
            alanlar += ", H.KDVLIBIRIMFIYAT AS ILKBEDEL ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR <> 1 AND BELGETARIHI <= " + VeriErisim.DegerYap(ak.bitisTarihi) + ") AS BEDELDUZELTME ";
            alanlar += ", EDF.TUTAR AS ENFLASYONDUZELTMECARI ";
            alanlar += ", (SELECT SUM(IC.TUTAR) FROM TASSICILNODEGERARTIS AS IC WHERE IC.PRSICILNO = H.PRSICILNO AND IC.DURUM = 5 AND IC.TUR = 1 AND BELGETARIHI <= " + VeriErisim.DegerYap(ak.bitisTarihi) + ")  AS ENFLASYONDUZELTMETOPLAM ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR <> 2 AND (IC.YIL*100 + IC.DONEM) <= (" + yilDonemKontrol + ")) )  AS AMORTISMANMIKTAR ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2) AND IC.DEGERARTISKODU = EDF.KOD) AS AMORTISMANENFLASYONDUZELTMECARI ";
            alanlar += ", (SELECT SUM(IC.AMORTISMANMIKTAR) FROM TASAMORTISMAN AS IC WHERE(IC.PRSICILNO = H.PRSICILNO AND IC.AMORTISMANTUR = 2 AND (IC.YIL*100 + IC.DONEM) <= (" + yilDonemKontrol + ")) )   AS AMORTISMANENFLASYONDUZELTMETOPLAM ";

            alanlar += ", OZ.PLAKA, OZ.ADI, OZ.YERIKONUSU, OZ.SASENO, OZ.EKNO, OZ.GIAI";
            alanlar += ", TIP.KOD, TIP.TUR, TIP.AD, AMBAR.KOD, H.KDVORAN";


            sql = "SELECT " + alanlar + " FROM TASSICILNOHAREKET AS H ";
            sql += "INNER JOIN TASSICILNOOZELLIK AS OZ ON OZ.PRSICILNO = H.PRSICILNO ";
            sql += "INNER JOIN TASHESAPPLAN AS HP ON H.HESAPPLANKOD=HP.KOD ";
            sql += "LEFT OUTER JOIN TASAMBAR AS AMBAR ON AMBAR.MUHASEBEKOD = H.MUHASEBEKOD AND AMBAR.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND AMBAR.KOD = H.AMBARKOD ";
            //sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = H.NEREYEGITTI ";
            sql += "LEFT OUTER JOIN TASODA AS ODA ON ODA.KOD = OZ.BULUNDUGUYER ";
            sql += "LEFT OUTER JOIN TASMARKA AS MARKA ON MARKA.KOD = OZ.MARKAKOD ";
            sql += "LEFT OUTER JOIN TASMODEL AS MODEL ON MARKA.KOD = MODEL.MARKAKOD AND MODEL.KOD = OZ.MODELKOD ";
            //sql += "INNER JOIN TASISLEMFORM TIF ON H.YIL = TIF.YIL AND H.MUHASEBEKOD = TIF.MUHASEBEKOD AND H.HARCAMABIRIMKOD = TIF.HARCAMABIRIMKOD AND H.FISNO = TIF.FISNO ";
            sql += "INNER JOIN TASSICILNOHAREKET H1 ON H1.PRSICILNO = H.PRSICILNO AND H1.SIRANO = 1 ";
            sql += "INNER JOIN TASISLEMTIP AS TIP ON TIP.KOD = H.ISLEMTIPIKOD ";
            sql += "INNER JOIN TASHARCAMABIRIM AS HB ON HB.KOD = H.HARCAMABIRIMKOD ";
            sql += "LEFT OUTER JOIN TASSICILNODEGERARTIS AS EDF ON EDF.PRSICILNO = H.PRSICILNO AND EDF.MUHASEBEKOD = H.MUHASEBEKOD AND EDF.HARCAMABIRIMKOD = H.HARCAMABIRIMKOD AND EDF.AMBARKOD = H.AMBARKOD " +
                "AND EDF.DURUM = 5 AND EDF.TUR = 1 AND YEAR(EDF.BELGETARIHI) = " + ak.yil + " AND MONTH(EDF.BELGETARIHI) = " + ak.donem / 100;


            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("H", "MUHASEBEKOD", "=", ak.muhasebeKod));

            // ************** Enflasyon artışı yapılacakların koşulları BAŞLA


            sql += "AND H.SIRANO = (SELECT MAX(IC2.SIRANO) FROM TASSICILNOHAREKET IC2 WHERE IC2.PRSICILNO = H.PRSICILNO AND IC2.DURUM = 1) ";
            sql += "AND H.ISLEMTURUKOD NOT IN(53,56) ";

            // ************** Enflasyon artışı yapılacakların koşulları BİTİŞ


            //Test
            //sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "PRSICILNO", "=", 116380));


            if (ak.yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H1", "YIL", "<=", ak.yil));
            if (!string.IsNullOrWhiteSpace(ak.harcamaKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HARCAMABIRIMKOD", "=", ak.harcamaKod.Replace(".", "")));
            if (!string.IsNullOrWhiteSpace(ak.ambarKod))
            {
                if (ak.ambarKod.Contains(";"))
                {
                    string ekKosul = "";
                    string[] kodlar = ak.ambarKod.Split(';');
                    foreach (var kod in kodlar)
                    {
                        if (!string.IsNullOrWhiteSpace(kod))
                            ekKosul = VeriErisim.KosulEkle(ekKosul, "OR", VeriErisim.KosulOl("H", "AMBARKOD", "=", kod));
                    }
                    if (!string.IsNullOrWhiteSpace(ekKosul))
                        sql = VeriErisim.KosulEkle(sql, "AND", "(" + ekKosul + ")");
                }
                else
                    sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "AMBARKOD", "=", ak.ambarKod));
            }
            if (!string.IsNullOrWhiteSpace(ak.nereyeGitti))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("OZ", "BULUNDUGUYER", "=", ak.nereyeGitti));
            if (!string.IsNullOrWhiteSpace(ak.hesapPlanKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H", "HESAPPLANKOD", "LIKE", ak.hesapPlanKod.Replace(".", "") + "%"));
            if (ak.prSicilNo > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO =" + ak.prSicilNo);
            if (!ak.baslamaTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H1", "FISTARIH", ">=", ak.baslamaTarihi));
            if (!ak.bitisTarihi.isNull)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("H1", "FISTARIH", "<=", ak.bitisTarihi));

            //TEST PRSICILNO
            string testPrSicilNo = OrtakFonksiyonlar.WebConfigOku("TasinirMalTestPrSicilNo", "");
            if (!string.IsNullOrWhiteSpace(testPrSicilNo))
                sql = VeriErisim.KosulEkle(sql, "AND", "H.PRSICILNO IN (" + testPrSicilNo + ")");


            sql += " ORDER BY ";
            if (ak.raporTur == (int)ENUMMBRaporTur.ENFLASYONDUZELTMESI)
                sql += "ALIMTARIHI, H.HESAPPLANKOD, H.SICILNO";

            return veriErisim.DataReaderDondur(sql);
        }

    }
}