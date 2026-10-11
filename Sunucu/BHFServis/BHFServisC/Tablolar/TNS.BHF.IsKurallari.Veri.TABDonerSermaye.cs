using System;
using TNS.Veri;
using System.Data;
using TNS;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Döner sermaye ve fon kimlik bilgilerini tutan BHFDONERSERMAYE, gelir, giderlerinin tutulduğu BHFDONERSERMAYEGELIRGIDER ve döner sermaye gelir gider kodlarının tutulduğu BHFDONERSERMAYESOZLUK veri tabanı tabloları ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABDonerSermaye : TABOrtak
    {
        /// <summary>
        /// Döner sermaye ve fon kimlik bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFDONERSERMAYE";

        /// <summary>
        /// Döner sermaye ve fon kimlik bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
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
            /// DSNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            DSNO = 2,

            /// <summary>
            /// DSAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            DSAD = 3,
        }

        /// <summary>
        /// Döner sermaye ve fon kimlik bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURKOD", "DSNO", "DSAD" };

        /// <summary>
        /// Döner sermaye/fon gelir, giderlerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADGG = "BHFDONERSERMAYEGELIRGIDER";

        /// <summary>
        /// Döner sermaye/fon gelir, giderlerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarGG : int
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
            /// DSNO alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            DSNO = 2,

            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KOD = 3,

            /// <summary>
            /// DSYIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            DSYIL = 4,

            /// <summary>
            /// HARCAMAMIODENEKMI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HARCAMAMIODENEKMI = 5,

            /// <summary>
            /// MIKTAR alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            MIKTAR = 6,
        }

        /// <summary>
        /// Döner sermaye/fon gelir, giderlerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLARGG = { "YIL", "KURKOD", "DSNO", "KOD", "DSYIL", "HARCAMAMIODENEKMI", "MIKTAR" };

        /// <summary>
        /// Döner sermaye gelir gider kodlarını tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADSOZLUK = "BHFDONERSERMAYESOZLUK";

        /// <summary>
        /// Döner sermaye gelir gider kodlarını tutan tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlarSOZLUK : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
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
        /// Döner sermaye gelir gider kodlarını tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLARSOZLUK = { "KOD", "AD", "VERIGIRILSIN" };

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABDonerSermaye(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen döner sermaye numarasına ait gelir/gider ve tanım bilgilerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Gelir/gider ve kimlik bilgileri silinecek döner sermayenin yılı.</param>
        /// <param name="kurKod">Gelir/gider ve kimlik bilgileri silinecek döner sermayenin sahibi birim.</param>
        /// <param name="donerSermayeNo">Gelir/gider ve kimlik bilgileri silinecek döner sermayenin numarası.</param>
        /// <returns>Silme sql komutları çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurKod, int donerSermayeNo)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DSNO], "=", donerSermayeNo));
            sql = "DELETE FROM " + TABLOADGG + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
                sonuc = KomutCalistir(sql, 0);
            }
            return sonuc;
        }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen döner sermaye numarasına ait tanım bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Kimlik bilgileri eklenecek döner sermayenin yılı.</param>
        /// <param name="kurKod">Kimlik bilgileri eklenecek döner sermayenin sahibi birim.</param>
        /// <param name="donerSermayeNo">Kimlik bilgileri eklenecek döner sermayenin numarası.</param>
        /// <param name="donerSermayeAd">Kimlik bilgileri eklenecek döner sermayenin adı.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc TanimEkle(int yil, string kurKod, int donerSermayeNo, string donerSermayeAd)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurKod) + ", ";
            sql = sql + VeriErisim.DegerYap(donerSermayeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(donerSermayeAd) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen döner sermaye numarasına, bilgileri verilen gelir/gider bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Kimlik bilgileri eklenecek döner sermayenin yılı.</param>
        /// <param name="kurKod">Kimlik bilgileri eklenecek döner sermayenin sahibi birim.</param>
        /// <param name="donerSermayeNo">Kimlik bilgileri eklenecek döner sermayenin numarası.</param>
        /// <param name="kod">Kaydedilecek miktarın gelir/gider kodu.</param>
        /// <param name="dsYil">Çok yıllı ödenek kaydı yapıldığından, yapılan ödeneğin hangi yıla ait olduğu. </param>
        /// <param name="odenekHarcama">Bu alan 1 ise eklenecek değer ödenek, 2 ise gerçekleşmedir.</param>
        /// <param name="deger">Eklenecek ödenek yada gerçekleşme miktarı.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc GelirGiderEkle(int yil, string kurKod, int donerSermayeNo, string kod, int dsYil, int odenekHarcama, double deger)
        {
            string sql;
            sql = "INSERT INTO " + TABLOADGG + "(" + VeriErisim.TumAlanlar("", ALANLARGG) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurKod) + ", ";
            sql = sql + VeriErisim.DegerYap(donerSermayeNo) + ", ";
            sql = sql + VeriErisim.DegerYap(kod) + ", ";
            sql = sql + VeriErisim.DegerYap(dsYil) + ", ";
            sql = sql + VeriErisim.DegerYap(odenekHarcama) + ", ";
            sql = sql + VeriErisim.DegerYap(deger) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Anahtar bilgileri verilen döner sermayenin adını bulan sql oluşturulur, çalıştırılır ve bulunan ad döndürülür.
        /// </summary>
        /// <param name="yil">Adı döndürülecek döner sermayenin yılı.</param>
        /// <param name="kurKod">Adı döndürülecek döner sermayenin sahibi birim.</param>
        /// <param name="donerSermayeNo">Adı döndürülecek döner sermayenin numarası.</param>
        /// <returns>Numarası verilen döner semayenin adı döndürülür.</returns>
        public string DonerSermayeAd(int yil, string kurKod, int donerSermayeNo)
        {
            string sql;
            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DSAD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DSNO], "=", donerSermayeNo));
            return veriErisim.TekKolonSQLStr(sql);
        }

        /// <summary>
        /// Döner sermaye gelir gider kodlarının bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader KodlariAl()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + " FROM " + TABLOADSOZLUK + " ORDER BY 1 ";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Döner sermaye anahtar bilgileri ile gelir/gider kodu verilen satırına ait, tüm yıllara ait ödenek ve gerçekleşme bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Ödenek ve gerçekleşme bilgileri döndürülecek döner sermayenin yılı.</param>
        /// <param name="kurKod">Ödenek ve gerçekleşme bilgileri döndürülecek döner sermayenin sahibi birim.</param>
        /// <param name="donerSermayeNo">Ödenek ve gerçekleşme bilgileri döndürülecek döner sermayenin numarası.</param>
        /// <param name="kod">Ödenek ve gerçekleşme bilgileri döndürülecek döner sermayenin gelir/gider kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader YillariAl(int yil, string kurKod, int donerSermayeNo, string kod)
        {
            string sql;
            sql = "SELECT " + ALANLARGG[(int)ENUMAlanlarGG.DSYIL] + ", " + ALANLARGG[(int)ENUMAlanlarGG.HARCAMAMIODENEKMI] + ", SUM(" + ALANLARGG[(int)ENUMAlanlarGG.MIKTAR] + ") FROM " + TABLOADGG;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGG[(int)ENUMAlanlarGG.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGG[(int)ENUMAlanlarGG.KURKOD], "=", kurKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGG[(int)ENUMAlanlarGG.DSNO], "=", donerSermayeNo));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARGG[(int)ENUMAlanlarGG.KOD], " LIKE ", kod + "%"));
            sql = sql + " GROUP BY " + ALANLARGG[(int)ENUMAlanlarGG.DSYIL] + ", " + ALANLARGG[(int)ENUMAlanlarGG.HARCAMAMIODENEKMI];
            sql = sql + " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Verilen döner sermaye sözlük bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="dss">Bilgileri eklenecek döner sermaye sözlük bilgilerini tutan nesne.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc KodKaydet(DonerSermayeSatir dss)
        {
            string sql = "INSERT INTO " + TABLOADSOZLUK + "(" + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + ") VALUES (";
            sql += VeriErisim.DegerYap(dss.kod) + ", ";
            sql += VeriErisim.DegerYap(dss.aciklama) + ", ";
            sql += VeriErisim.DegerYap(Convert.ToInt32(dss.degerGirilsinMi)) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Veri tabanındaki tüm sözlük bilgilerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <returns>Silme sql komutları çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc KodSil()
        {
            string sql = "DELETE FROM " + TABLOADSOZLUK;
            return KomutCalistir(sql, 0);
        }
    }
}

/*

        public IDataReader DonerSermayeNo(int yil, string kurum)
        {
            string sql;
            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.DSNO] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurum));
            return veriErisim.DataReaderDondur(sql);
        }



*/