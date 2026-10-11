using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Birim dosya planı bilgilerini içeren EVRDOSYAPLANIBIRIM ve EVRDOSYAPLANIFAVORI veri tabanı tabloları ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABBirimDosyaPlani : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABBirimDosyaPlani sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABBirimDosyaPlani(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Birim dosya planı bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRDOSYAPLANIBIRIM";

        /// <summary>
        /// Birim dosya planı bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            EVRAKBIRIMKODU = 0,
            DOSYAPLANKODU = 1,
        }

        /// <summary>
        /// Birim dosya planı bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "EVRAKBIRIMKODU", "DOSYAPLANKODU" };

        /// <summary>
        /// Dosya planı favori bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADFAVORI = "EVRDOSYAPLANIFAVORI";

        /// <summary>
        /// Dosya planı favori bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlarFAVORI
        {
            KULLANICIKODU = 0,
            DOSYAPLANKODU = 1,
            DOSYAPLANADI = 2
        }

        /// <summary>
        /// Dosya planı favori bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARFAVORI = { "KULLANICIKODU", "DOSYAPLANKODU", "DOSYAPLANADI" };

        /// <summary>
        /// Parametre olarak verilen DosyaPlaniBirim nesnesine göre; birim dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Birim dosya planı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            foreach (DosyaPlaniBirim bilgi in liste.objeler)
            {
                if (string.IsNullOrEmpty(bilgi.dosyaPlani.kod.Trim()))
                    continue;

                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.evrakBirimKodu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.dosyaPlani.kod) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen evrak birim kodu ve dosya plan kodu bilgilerine göre; birim dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="birimKod">Evrak birim kodu bilgisini içeren nesne</param>
        /// <param name="kod">Dosya plan kodu bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(string birimKod, string kod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (birimKod == "" && kod == "")//İki parametrede boş olamaz
                return sonuc;

            string sql = "DELETE FROM " + TABLOAD + " WHERE ";

            if (birimKod != "")
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", birimKod);
            if (kod != "")
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], "=", kod);

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen DosyaPlaniBirim nesnesine göre; birim dosya planı bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Birim dosya planı bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(DosyaPlaniBirim bilgi)
        {
            //string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",AD,SEVIYE,DETAY,EVRDOSYAPLANIBOLUMTANIM.BOLUMADI FROM " + TABLOAD + ",EVRDOSYAPLANI";
            //sql += " LEFT JOIN EVRDOSYAPLANIBOLUMTANIM ON EVRDOSYAPLANIBOLUMTANIM.HESAPKODU=EVRDOSYAPLANI.KOD";


            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",AD,SEVIYE,DETAY FROM " + TABLOAD + ",EVRDOSYAPLANI";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", bilgi.evrakBirimKodu));
            sql += " AND EVRDOSYAPLANIBIRIM.DOSYAPLANKODU=EVRDOSYAPLANI.KOD";

            if (bilgi.dosyaPlani.kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYAPLANKODU], " LIKE ", bilgi.dosyaPlani.kod + "%"));
            if (bilgi.dosyaPlani.seviye > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRDOSYAPLANI", "SEVIYE", "=", bilgi.dosyaPlani.seviye));
            if (bilgi.dosyaPlani.kullanilmiyor > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("EVRDOSYAPLANI", "KULLANILMIYOR", "=", bilgi.dosyaPlani.kullanilmiyor));

            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen TckimlikNo ve DosyaPlani nesne koleksiyonuna göre; favori dosya planı bililerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="tcKimlikNo">TckimlikNo bilgisini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetFavori(string tcKimlikNo, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            foreach (DosyaPlani bilgi in liste.objeler)
            {
                string sql = "INSERT INTO " + TABLOADFAVORI + " (" + VeriErisim.TumAlanlar("", ALANLARFAVORI) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(tcKimlikNo) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.ad) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen bilgilere göre; favori dosya planı bililerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="tcKimlikNo">TckimlikNo bilgisini içeren nesne</param>
        /// <param name="ad">Dosya planı ad bilgisi içeren nesne</param>
        /// <param name="kod">Dosya planı kod bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader ListeleFavori(string tcKimlikNo, string ad, string kod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADFAVORI, ALANLARFAVORI) + " FROM " + TABLOADFAVORI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFAVORI[(int)ENUMAlanlarFAVORI.KULLANICIKODU], "=", tcKimlikNo));

            if (kod != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARFAVORI[(int)ENUMAlanlarFAVORI.DOSYAPLANKODU], "LIKE", kod + "%"));
            if (ad != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLARFAVORI[(int)ENUMAlanlarFAVORI.DOSYAPLANADI], "LIKE", ad + "%"));

            sql += " ORDER BY 1,2";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen bilgilere göre; favori dosya planı bililerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="tcKimlikNo">TckimlikNo bilgisini içeren nesne</param>
        /// <param name="liste">DosyaPlani nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SilFavori(string tcKimlikNo, ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            if (liste.objeler.Count == 0)
                return sonuc;

            string sql = "DELETE FROM " + TABLOADFAVORI;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARFAVORI[(int)ENUMAlanlarFAVORI.KULLANICIKODU], "=", tcKimlikNo));

            string kosul = "";
            foreach (DosyaPlani bilgi in liste.objeler)
                kosul = VeriErisim.KosulEkle(kosul, "OR", VeriErisim.KosulOl("", ALANLARFAVORI[(int)ENUMAlanlarFAVORI.DOSYAPLANKODU], "=", bilgi.kod));

            sql = VeriErisim.KosulEkle(sql, "AND (", kosul) + ")";

            return KomutCalistir(sql, 0);
        }

    }
}
