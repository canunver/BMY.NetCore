using System;
using TNS.Veri;
using System.Data;
using TNS;
using TNS.BKD;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Hizmet maliyetinin tesbiti için girilen değerlerin tutulduğu BHFHIZMETMALIYETTESPIT ile girilecek değerlerin sözlük bilgilerinin tutulduğu BHFHIZMETMALIYETTESPITSOZLUK veri tabanı tabloları ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABHizmetMaliyet : TABOrtak
    {
        /// <summary>
        /// Hizmet maliyetinin tesbiti için girilen değerleri tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFHIZMETMALIYETTESPIT";

        /// <summary>
        /// Hizmet maliyetinin tesbiti için girilen değerleri tutan tablonun alanlarının sırasını tutan enumeration.
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
            /// GRUPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GRUPKOD = 2,

            /// <summary>
            /// GERCEKLESME alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GERCEKLESME = 3,

            /// <summary>
            /// HAZIRAN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            HAZIRAN = 4,

            /// <summary>
            /// TAHMIN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TAHMIN = 5,

            /// <summary>
            /// TAHMIN1 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TAHMIN1 = 6,

            /// <summary>
            /// TAHMIN2 alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TAHMIN2 = 7,

            /// <summary>
            /// DIGERBILGILER alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            DIGERBILGILER = 8,
        }

        /// <summary>
        /// Hizmet maliyetinin tesbiti için girilen değerleri tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURKOD", "GRUPKOD", "GERCEKLESME", "HAZIRAN", "TAHMIN", "TAHMIN1", "TAHMIN2", "DIGERBILGILER" };

        /// <summary>
        /// Hizmet maliyetinin tesbiti için girilecek bilgilerin sözlük bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOADSOZLUK = "BHFHIZMETMALIYETTESPITSOZLUK";

        /// <summary>
        /// Hizmet maliyetinin tesbiti için girilecek bilgilerin sözlük bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarSOZLUK : int
        {
            /// <summary>
            /// GRUPKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GRUPKOD = 0,

            /// <summary>
            /// GRUPAD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GRUPAD = 1,

            /// <summary>
            /// VERIGIRILSIN alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            VERIGIRILSIN = 2,
        }

        /// <summary>
        /// Hizmet maliyetinin tesbiti için girilecek bilgilerin sözlük bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLARSOZLUK = { "GRUPKOD", "GRUPAD", "VERIGIRILSIN" };

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABHizmetMaliyet(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki hizmet maliyetinin tesbiti için girilen değerlerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Hizmet maliyetinin tesbiti için girilen değerleri silinecek döner sermayenin yılı.</param>
        /// <param name="kurumsalKod">Hizmet maliyetinin tesbiti için girilen değerleri silinecek döner sermayenin sahibi birim.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurumsalKod)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurumsalKod));
            sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki verilen hizmet maliyeti tesbiti bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Eklenecek hizmet maliyeti tesbiti bilgisinin yılı.</param>
        /// <param name="kurumsalKod">Eklenecek hizmet maliyeti tesbiti bilgisinin birimi.</param>
        /// <param name="grupKod">Eklenecek hizmet maliyeti tesbiti bilgisinin sözlük kodu.</param>
        /// <param name="hmts">Eklenecek hizmet maliyeti tesbiti bilgisi.</param>
        /// <param name="digerBilgiler">Hizmet bilgisi 6 ile başlıyorsa, sözlükten değil serbest giriliyordur. Bu durumda girilen satırın kullanıcı tarafından girilen açıklaması.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Ekle(int yil, string kurumsalKod, string grupKod, HizmetMaliyetiTesbitiSatir hmts, string digerBilgiler)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(grupKod) + ", ";
            sql = sql + VeriErisim.DegerYap(hmts.gercek) + ", ";
            sql = sql + VeriErisim.DegerYap(hmts.haziran) + ", ";
            sql = sql + VeriErisim.DegerYap(hmts.tahmin) + ", ";
            sql = sql + VeriErisim.DegerYap(hmts.tahmin1) + ", ";
            sql = sql + VeriErisim.DegerYap(hmts.tahmin2) + ", ";
            sql = sql + VeriErisim.DegerYap(digerBilgiler) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hizmet maliyeti sözlük kodu ve anahtar bilgileri verilen satırın, maliyet bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Listelenecek hizmet maliyeti tesbiti bilgisinin yılı.</param>
        /// <param name="kurumsalKod">Listelenecek hizmet maliyeti tesbiti bilgisinin birimi.</param>
        /// <param name="grupKod">Listelenecek hizmet maliyeti tesbiti bilgisinin sözlük kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader DegeriAl(int yil, string kurumsalKod, string grupKod)
        {
            string sql;
            sql = "SELECT " + "SUM(" + ALANLAR[(int)ENUMAlanlar.GERCEKLESME] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.HAZIRAN] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.TAHMIN] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.TAHMIN1] + "), " + "SUM(" + ALANLAR[(int)ENUMAlanlar.TAHMIN2] + ") ";
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (kurumsalKod.Length == 2)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], ">", kurumsalKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "<", kurumsalKod + BKD.Arac.BagimsizKurumBaslangic()));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurumsalKod + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], " LIKE ", grupKod + "%"));
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hizmet maliyeti sözlüğünde son satıra kullanıcı serbest metin ve değer girmektedir. Sözlüğün son kodu ve anahtar bilgileri verilen satırların, maliyet bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Listelenecek hizmet maliyeti tesbiti bilgisinin yılı.</param>
        /// <param name="kurumsalKod">Listelenecek hizmet maliyeti tesbiti bilgisinin birimi.</param>
        /// <param name="sozlukSonKod">Serbest giriş yapılan ve listelenecek hizmet maliyeti tesbiti bilgisinin sözlük kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader DegerleriAlDiger(int yil, string kurumsalKod, string sozlukSonKod)
        {
            string sql;
            sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + ALANLAR[(int)ENUMAlanlar.GRUPKOD] + ", " + ALANLAR[(int)ENUMAlanlar.GERCEKLESME] + ", " + ALANLAR[(int)ENUMAlanlar.HAZIRAN] + ", " + ALANLAR[(int)ENUMAlanlar.TAHMIN] + ", " + ALANLAR[(int)ENUMAlanlar.TAHMIN1] + ", " + ALANLAR[(int)ENUMAlanlar.TAHMIN2] + ", " + ALANLAR[(int)ENUMAlanlar.DIGERBILGILER];
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (kurumsalKod.Length == 2)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], ">", kurumsalKod));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "<", kurumsalKod + BKD.Arac.BagimsizKurumBaslangic()));
            }
            else
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", kurumsalKod + "%"));

            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.GRUPKOD], " LIKE ", sozlukSonKod + "%"));
            sql += "ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hizmet maliyeti tesbiti sözlük bilgilerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader KodlariAl()
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + " FROM " + TABLOADSOZLUK + " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hizmet maliyeti tesbiti sözlük bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="hmts">Eklenecek Hizmet maliyeti tesbiti sözlük bilgisini tutan nesne.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc KodKaydet(HizmetMaliyetiTesbitiSatir hmts)
        {
            string sql = "INSERT INTO " + TABLOADSOZLUK + "(" + VeriErisim.TumAlanlar("", ALANLARSOZLUK) + ") VALUES (";
            sql += VeriErisim.DegerYap(hmts.kod) + ", ";
            sql += VeriErisim.DegerYap(hmts.aciklama) + ", ";
            sql += VeriErisim.DegerYap(Convert.ToInt32(hmts.degerGirilsinMi)) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hizmet maliyeti tesbiti sözlük bilgilerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc KodSil()
        {
            string sql = "DELETE FROM " + TABLOADSOZLUK;
            return KomutCalistir(sql, 0);
        }
    }
}
