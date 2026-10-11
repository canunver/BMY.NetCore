using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Havale kural bilgilerini içeren EVRHAVALEKURAL, EVRHAVALEKURALGB ve EVRHAVALEKURALDETAY veri tabanı tabloları ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABHavaleKural : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABHavaleKural sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABHavaleKural(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Havale kural bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOAD = "EVRHAVALEKURAL";

        /// <summary>
        /// Havale kural bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar { KOD = 0, KULLANICIKODU = 1, EVRAKBIRIMKODU = 2, AD = 3, BASLANGICTARIH = 4, BITISTARIH = 5 }

        /// <summary>
        /// Havale kural bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "KULLANICIKODU", "EVRAKBIRIMKODU", "AD", "BASLANGICTARIH", "BITISTARIH" };

        /// <summary>
        /// Havale kural gereği-bilgi bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOADGB = "EVRHAVALEKURALGB";

        /// <summary>
        /// Havale kural gereği-bilgi bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarGB { HAVALEKURALKODU = 0, KOD = 1, DEGER = 2, GEREGIBILGI = 3 }

        /// <summary>
        /// Havale kural gereği-bilgi bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARGB = { "HAVALEKURALKODU", "KOD", "DEGER", "GEREGIBILGI" };

        /// <summary>
        /// Havale kural detay bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOADDETAY = "EVRHAVALEKURALDETAY";

        /// <summary>
        /// Havale kural detay bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarDetay { HAVALEKURALKODU = 0, KOD = 1, ALAN = 2, ISLEC = 3, DEGER = 4 }

        /// <summary>
        /// Havale kural detay bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARDETAY = { "HAVALEKURALKODU", "KOD", "ALAN", "ISLEC", "DEGER" };

        /// <summary>
        /// Sql cümlesi içeren nesne
        /// </summary>
        private string sql = "";

        /// <summary>
        /// Parametre olarak verilen havale kural bilgisine göre; havale kural bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="form">Havale kural bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(HavaleKural form) 
        {
            if (form.kod == 0)
            {
                form.kod = KodAl();

                sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.kullanici) + ",";
                sql += VeriErisim.DegerYap(form.evrakBirimKodu) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ",";
                sql += VeriErisim.DegerYap(form.baslangicTarih) + ",";
                sql += VeriErisim.DegerYap(form.bitisTarih) + ")";
            }
            else
            {
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], form.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], form.kullanici) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], form.evrakBirimKodu) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BASLANGICTARIH], form.baslangicTarih) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.BITISTARIH], form.bitisTarih);

                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (form.kod)));
            }

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen havaleKuralGB nesne koleksiyonuna göre; havale kural gereği-bilgi bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgiler">HavaleKuralGB nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetGB(ArrayList bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            foreach (HavaleKuralGB gb in bilgiler)
            {
                sql = "INSERT INTO " + TABLOADGB + " (" + VeriErisim.TumAlanlar("", ALANLARGB) + ") VALUES (";
                sql += VeriErisim.DegerYap(gb.havaleKuralKodu) + ",";
                sql += VeriErisim.DegerYap(gb.kod) + ",";
                sql += VeriErisim.DegerYap(gb.deger) + ",";
                sql += VeriErisim.DegerYap(gb.geregiBilgi) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc) break;
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen havaleKuralDetay nesne koleksiyonuna göre; havale kural detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgiler">HavaleKuralDetay nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetDetay(ArrayList bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            foreach (HavaleKuralDetay detay in bilgiler)
            {
                sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(detay.havaleKuralKodu) + ",";
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
        /// Paramete olarak verilen bilgilere göre; havale kural bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
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
            sql = "SELECT ";
            sql += VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + ",";
            sql += VeriErisim.TumAlanlar(TABLOADGB, ALANLARGB) + ",";
            sql += VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM ";
            sql += TABLOAD + "," + TABLOADGB + "," + TABLOADDETAY;

            sql += " WHERE " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADGB + "." + ALANLARDETAY[(int)ENUMAlanlarGB.HAVALEKURALKODU];
            sql += " AND " + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + "=" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.HAVALEKURALKODU];

            if (!string.IsNullOrEmpty(kodList))
                kriter = " (" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " IN (" + kodList + ")) ";
            if (!string.IsNullOrEmpty(tcKimlikNo))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", tcKimlikNo));
            if (!string.IsNullOrEmpty(evrakBirimKodu))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", evrakBirimKodu));
            if (kriter != "")
                sql += " AND " + kriter;

            sql += " ORDER BY 1,8,12";
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen havale kural kod bilgisine göre; havale kural bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="havaleKuralKodu">Havale kural kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(int havaleKuralKodu)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            sonuc = SilGB(havaleKuralKodu);
            if (sonuc.islemSonuc)
            {
                sonuc = SilDetay(havaleKuralKodu);
                if (sonuc.islemSonuc)
                {
                    sql = "DELETE FROM " + TABLOAD;
                    sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", havaleKuralKodu));
                    return KomutCalistir(sql, 1);
                }
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen havale kural kod bilgisine göre; havale kural gereği-bilgi bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="havaleKuralKodu">Havale kural kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SilGB(int havaleKuralKodu)
        {
            sql = "DELETE FROM " + TABLOADGB;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARGB[(int)ENUMAlanlarGB.HAVALEKURALKODU], "=", havaleKuralKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen havale kural kod bilgisine göre; havale kural detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="havaleKuralKodu">Havale kural kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SilDetay(int havaleKuralKodu)
        {
            sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.HAVALEKURALKODU], "=", havaleKuralKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Veritabanına yeni havale kural bilgisi kaydetmek için, sql cümlesi hazırlayıp çalıştırarak yeni bir havale kural kodu oluşturan yordam.
        /// </summary>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        private int KodAl()
        {
            sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}
