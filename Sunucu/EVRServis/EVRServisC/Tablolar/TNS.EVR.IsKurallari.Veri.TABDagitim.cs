using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Dağıtım bilgilerini içeren EVRDAGITIM ve EVRDAGITIMDETAY veri tabanı tabloları ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABDagitim : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABDagitim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABDagitim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Dağıtım bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOAD = "EVRDAGITIM";

        /// <summary>
        /// Dağıtım bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            KOD = 0,
            EVRAKBIRIMKODU = 1,
            AD = 2,
        }

        /// <summary>
        /// Dağıtım bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "EVRAKBIRIMKODU", "AD" };

        /// <summary>
        /// Dağıtım detay bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        static string TABLOADALANLAR = "EVRDAGITIMDETAY";

        /// <summary>
        /// Dağıtım detay bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarDagitimAlanlar
        {
            DAGITIMKODU = 0,
            SIRANO = 1,
            YER = 2,
            EVRAKBIRIMKODU = 3,
            ARZRICA = 4,
        }

        /// <summary>
        /// Dağıtım detay bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLARDAGITIMALANLAR = { "DAGITIMKODU", "SIRANO", "YER", "EVRAKBIRIMKODU", "ARZRICA", "KURUMBILGI" };

        /// <summary>
        /// Parametre olarak verilen dagitim nesnesine göre; dağıtım bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgi">Dağıtım bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(Dagitim form)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            form.kod = this.KodAl();
            if (form.kod > 0)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(form.kod) + ",";
                sql += VeriErisim.DegerYap(form.evrakBrimKodu) + ",";
                sql += VeriErisim.DegerYap(form.ad) + ")";

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }
        public Sonuc Guncelle(Dagitim form)
        {
            veriErisim.hata = "";
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], form.kod) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], form.evrakBrimKodu) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], form.ad);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (form.kod)));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", (form.evrakBrimKodu)));

            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// Parametre olarak verilen dagitimAlan nesne koleksiyonuna göre; dağıtım alan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgiler">DagitimAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetAlanlar(ArrayList bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (DagitimAlan bilgi in bilgiler)
            {
                sql = "INSERT INTO " + TABLOADALANLAR + " (" + VeriErisim.TumAlanlar("", ALANLARDAGITIMALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.dagitimKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.yer) + ",";
                sql += VeriErisim.DegerYap(bilgi.evrakBrimKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.arzRica) + ",";
                sql += VeriErisim.DegerYap(bilgi.kurumBilgi) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen evrak birim kodu ve dağıtım kod listesine göre; dağıtım bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Dağıtım kod listesini içeren nesne</param>
        /// <param name="evrakBrimKodu">Evrak birim kodu bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(string kodList, string evrakBrimKodu)
        {
            string sql = "";
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (!string.IsNullOrEmpty(kodList))
                kriter = " (" + TABLOAD + "." + ALANLAR[(int)ENUMAlanlar.KOD] + " IN (" + kodList + ")) ";

            if (!string.IsNullOrEmpty(evrakBrimKodu))
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", evrakBrimKodu));
            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen dağıtım kod listesine göre; dağıtım alan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Dağıtım kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader ListeleAlanlar(string kodList)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADALANLAR, ALANLARDAGITIMALANLAR) + " FROM " + TABLOADALANLAR;
            if (kodList == "")
                kodList = "0";
            sql += " WHERE (" + TABLOADALANLAR + "." + ALANLARDAGITIMALANLAR[(int)ENUMAlanlarDagitimAlanlar.DAGITIMKODU] + " IN (" + kodList + ")) ";
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen dağıtım kod bilgisine göre; dağıtım alan bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="dagitimKodu">Dağıtım kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SilAlanlar(int dagitimKodu)
        {
            string sql = "DELETE FROM " + TABLOADALANLAR;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDAGITIMALANLAR[(int)ENUMAlanlarDagitimAlanlar.DAGITIMKODU], "=", dagitimKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen dağıtım kod bilgisine göre; dağıtım bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="dagitimKodu">Dağıtım kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(int dagitimKodu)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            sonuc = SilAlanlar(dagitimKodu);
            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", dagitimKodu));
                return KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Veritabanına yeni dağıtım bilgisi kaydetmek için, sql cümlesi hazırlayıp çalıştırarak yeni bir dağıtım kodu oluşturan yordam.
        /// </summary>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        private int KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ")  FROM " + TABLOAD;
            return veriErisim.TekKolonSQLInt(sql) + 1;
        }
    }
}
