using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Evrak haberci bilgilerini içeren EVRHABERCI ve EVRHABERCIDETAY veri tabanı tabloları ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABHaberci : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABHaberci sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABHaberci(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Haberci bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOAD = "EVRHABERCI";

        /// <summary>
        /// Haberci bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar { KOD = 0, KULLANICI = 1, EVRAKBIRIMKODU = 2, AD = 3, BASLANGICTARIH = 4, BITISTARIH = 5 }

        /// <summary>
        /// Haberci bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "KULLANICI", "EVRAKBIRIMKODU", "AD", "BASLANGICTARIH", "BITISTARIH" };

        /// <summary>
        /// Haberci detay bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOADDETAY = "EVRHABERCIDETAY";

        /// <summary>
        /// Haberci detay bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarDetay { HABERCIKODU = 0, KOD = 1, ALAN = 2, ISLEC = 3, DEGER = 4 }

        /// <summary>
        /// Haberci detay bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARDETAY = { "HABERCIKODU", "KOD", "ALAN", "ISLEC", "DEGER" };

        /// <summary>
        /// Sql cümlesi içeren nesne
        /// </summary>
        private string sql = "";

        /// <summary>
        /// Parametre olarak verilen haberci nesnesine göre; haberci bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Haberci bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(Haberci form) 
        {
            if (form.kod == 0)
            {
                form.kod = KodAl();

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.kullanici) + ",";
                sql += VeriErisim.DegerYap(form.evrakBrimKodu) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap(form.baslangicTarih) + ",";
                sql += VeriErisim.DegerYap(form.bitisTarih) + ")";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], form.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICI], form.kullanici) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], form.evrakBrimKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], form.baslangicTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], form.bitisTarih);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (form.kod)));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen haberciDetay nesne koleksiyonuna göre; haberci detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgiler">HaberciDetay nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetDetay(ArrayList bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            foreach (HaberciDetay detay in bilgiler)
            {
                sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(detay.haberciKodu) + ",";
                sql += VeriErisim.DegerYap(detay.kod) + ",";
                sql += VeriErisim.DegerYap(detay.alan) + ",";
                sql += VeriErisim.DegerYap(detay.islec) + ",";
                sql += VeriErisim.DegerYap(detay.deger) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) break;
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen bilgilere göre; haberci bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Haberci kod listesini içeren nesne</param>
        /// <param name="tcKimlikNo">TcKimlikNo bilgisini içeren nesne</param>
        /// <param name="evrakBirimKodu">The evrak birim kodu.</param>
        /// <returns>
        /// İşlem sonucu DataReader olarak döndürülür.
        /// </returns>
        public IDataReader Listele(string kodList, string tcKimlikNo, string evrakBirimKodu)
        {
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOAD + "," + TABLOADDETAY;
            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HABERCIKODU];
            if (!string.IsNullOrEmpty(kodList))
                kriter = " (" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " IN (" + kodList + ")) ";
            if (!string.IsNullOrEmpty(tcKimlikNo))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICI], "=", tcKimlikNo));
            if (!string.IsNullOrEmpty(evrakBirimKodu))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", evrakBirimKodu));
            if (kriter != "")
                sql += " AND " + kriter;

            sql += " ORDER BY 1";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen haberci kod listesine göre; haberci detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Haberci kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader ListeleDetay(string kodList)
        {
            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;
            if (kodList == "") { kodList = "0"; }
            sql += " WHERE (" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HABERCIKODU] + " IN (" + kodList + ")) ";
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen haberci kod bilgisine göre; haberci bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="haberciKodu">Haberci kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(int haberciKodu)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            sonuc = SilDetay(haberciKodu);
            if (sonuc.islemSonuc)
            {
                sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", haberciKodu));
                return KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen haberci kod bilgisine göre; haberci detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="haberciKodu">Haberci kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SilDetay(int haberciKodu)
        {
            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HABERCIKODU], "=", haberciKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Veritabanına yeni haberci bilgisi kaydetmek için, sql cümlesi hazırlayıp çalıştırarak yeni bir haberci kodu oluşturan yordam.
        /// </summary>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        private int KodAl()
        {
            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}
