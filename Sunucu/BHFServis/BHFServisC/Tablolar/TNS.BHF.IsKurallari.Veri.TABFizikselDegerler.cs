using System;
using TNS.Veri;
using System.Data;
using TNS;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Fiziksel değerler bilgilerini tutan BHFFIZIKSELDEGERLER ile fiziksel değerler sözlük bilgilerini tutan BHFFIZIKSELDEGERLERSOZLUK veri tabanı tabloları ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABFizikselDegerler : TABOrtak
    {
        /// <summary>
        /// Fiziksel değer bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFFIZIKSELDEGERLER";

        /// <summary>
        /// Fiziksel değer bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// DEGERKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            DEGERKOD = 2,

            /// <summary>
            /// MIKTARYILI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            MIKTARYILI = 3,

            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            MIKTAR = 4,
        }

        /// <summary>
        /// Fiziksel değer bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURKOD", "DEGERKOD", "MIKTARYILI", "MIKTAR" };

        /// <summary>
        /// Fiziksel değer sözlük bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADSOZLUK = "BHFFIZIKSELDEGERLERSOZLUK";

        /// <summary>
        /// Fiziksel değer sözlük bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarSOZLUK : int
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KOD = 0,
            
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            AD = 1,

            /// <summary>
            /// VERIGIRILSIN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            VERIGIRILSIN = 2,
        }

        /// <summary>
        /// Fiziksel değer sözlük bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLARSOZLUK = { "DEGERKOD", "DEGERAD", "VERIGIRILSIN" };

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABFizikselDegerler(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki fiziksel değerlerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Fiziksel değerleri silinecek döner sermayenin yılı.</param>
        /// <param name="kurKod">Fiziksel değerleri silinecek silinecek döner sermayenin sahibi birim.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurKod)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod.Replace(".", "")));
            sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen fiziksel değer bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Eklenecek fiziksel değerin yılı.</param>
        /// <param name="kurKod">Eklenecek fiziksel değerin sahibi birim.</param>
        /// <param name="degerKod">Eklenecek fiziksel değerin sözlük kodu.</param>
        /// <param name="miktarYili">Eklenecek fiziksel değerin ait olduğu yıl.</param>
        /// <param name="miktar">Fiziksel değerin yılı.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Ekle(int yil, string kurKod, string degerKod, int miktarYili, int miktar)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurKod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(degerKod) + ", ";
            sql = sql + VeriErisim.DegerYap(miktarYili) + ", ";
            sql = sql + VeriErisim.DegerYap(miktar) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Fiziksel değer sözlük bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader KodlariAl()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + " FROM " + TABLOADSOZLUK + " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Fiziksel değer sözlük bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="fizikselDeger">Eklenecek fiziksel değer sözlük bilgisini tutan nesne.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc KodKaydet(FizikselDeger fizikselDeger)
        {
            string sql = "INSERT INTO " + TABLOADSOZLUK + "(" + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + ") VALUES (";
            sql += VeriErisim.DegerYap(fizikselDeger.kod) + ", ";
            sql += VeriErisim.DegerYap(fizikselDeger.aciklama) + ", ";
            sql += VeriErisim.DegerYap(Convert.ToInt32(fizikselDeger.degerGirilsinMi)) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Fiziksel değer sözlük bilgilerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc KodSil()
        {
            string sql = "DELETE FROM " + TABLOADSOZLUK;
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Fiziksel değer kodu ve anahtar bilgileri verilen, tüm yıllara ait fiziksel değer bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Bilgileri döndürülecek fiziksel değerin yılı.</param>
        /// <param name="kurKod">Bilgileri döndürülecek fiziksel değerin sahibi birim.</param>
        /// <param name="kod">Bilgileri döndürülecek fiziksel değerin kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader YillariAl(int yil, string kurKod, string kod)
        {
            string sql;

            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.MIKTARYILI] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.MIKTAR] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], " LIKE ", kurKod.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DEGERKOD], " LIKE ", kod + "%"));
            sql = sql + " GROUP BY " + ALANLAR[(int)ENUMAlanlar.MIKTARYILI];
            sql = sql + " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}

/*
    //public class TABFizikselDegerlerTasra : TABOrtak
    //{
    //    public enum ENUMAlanlar : int
    //    {
    //        YIL = 0,
    //        MUHASEBEKOD = 1,
    //        BIRIMKOD = 2,
    //        KURKOD = 3,
    //        DEGERKOD = 4,
    //        MIKTARYILI = 5,
    //        MIKTAR = 6,
    //    }
    //    static string TABLOAD = "BHFFIZIKSELDEGERLERTASRA";
    //    static string[] ALANLAR = { "YIL", "MUHASEBEKOD", "BIRIMKOD", "KURKOD", "DEGERKOD", "MIKTARYILI", "MIKTAR" };
    //    static int[] ALANTIP = { (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.NUMERIC, (int)ENUMAlanTip.NUMERIC };

    //    public enum ENUMAlanlarSOZLUK : int
    //    {
    //        KOD = 0,
    //        AD = 1,
    //        VERIGIRILSIN = 2,
    //    }
    //    static string TABLOADSOZLUK = "BHFFIZIKSELDEGERLERSOZLUK";
    //    static string[] ALANLARSOZLUK = { "DEGERKOD", "DEGERAD", "VERIGIRILSIN" };
    //    static int[] ALANTIPSOZLUK = { (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.STRING, (int)ENUMAlanTip.NUMERIC };


    //    public TABFizikselDegerlerTasra(VeriErisim veriErisim) : base(veriErisim) { }

    //    public Sonuc Sil(int yil, int muhasebeKod, string birimKod, string kurumBirim)
    //    {
    //        string kosul, sql;
    //        Sonuc sonuc;
    //        kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
    //        kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurumBirim.Replace(".", "")));
    //        sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
    //        sonuc = KomutCalistir(sql, 0);
    //        return sonuc;
    //    }

    //    public Sonuc Ekle(int yil, int muhasebeKod, string birimKod, string kurumBirim, string degerKod, int miktarYili, int miktar)
    //    {
    //        string sql;
    //        sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
    //        sql = sql + VeriErisim.DegerYap(yil) + ", ";
    //        sql = sql + VeriErisim.DegerYap(muhasebeKod) + ", ";
    //        sql = sql + VeriErisim.DegerYap(birimKod) + ", ";
    //        sql = sql + VeriErisim.DegerYap(kurumBirim.Replace(".", "")) + ", ";
    //        sql = sql + VeriErisim.DegerYap(degerKod) + ", ";
    //        sql = sql + VeriErisim.DegerYap(miktarYili) + ", ";
    //        sql = sql + VeriErisim.DegerYap(miktar) + ")";
    //        return KomutCalistir(sql, 1);
    //    }

    //    public IDataReader KodlariAl()
    //    {
    //        string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + " FROM " + TABLOADSOZLUK + " ORDER BY 1";
    //        return veriErisim.DataReaderDondur(sql);
    //    }

    //    public Sonuc KodKaydet(FizikselDeger fd)
    //    {
    //        string sql = "INSERT INTO " + TABLOADSOZLUK + "(" + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + ") VALUES (";
    //        sql += VeriErisim.DegerYap(fd.kod) + ", ";
    //        sql += VeriErisim.DegerYap(fd.aciklama) + ", ";
    //        sql += VeriErisim.DegerYap(Convert.ToInt32(fd.degerGirilsinMi)) + ")";

    //        return KomutCalistir(sql, 1);
    //    }

    //    public Sonuc KodSil()
    //    {
    //        string sql = "DELETE FROM " + TABLOADSOZLUK;
    //        return KomutCalistir(sql, 0);
    //    }

    //    public IDataReader YillariAl(int yil, int muhasebeKod, string birimKod, string kurumBirim, string kod)
    //    {
    //        string sql;

    //        sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.MIKTARYILI] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.MIKTAR] + ") FROM " + TABLOAD;
    //        sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
    //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebeKod));
    //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.BIRIMKOD], "=", birimKod));
    //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], " LIKE ", kurumBirim.Replace(".", "") + "%"));
    //        sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DEGERKOD], " LIKE ", kod + "%"));
    //        sql = sql + " GROUP BY " + ALANLAR[(int)ENUMAlanlar.MIKTARYILI];
    //        sql = sql + " ORDER BY 1";
    //        return veriErisim.DataReaderDondur(sql);
    //    }
    //}
*/