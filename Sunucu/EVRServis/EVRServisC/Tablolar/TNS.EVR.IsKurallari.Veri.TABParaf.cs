using System;
using System.Data;
using TNS.Veri;
using System.Collections;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Paraf bilgilerini içeren EVRPARAF ve EVRPARAFDETAY veri tabanı tabloları ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABParaf : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABParaf sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABParaf(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Paraf bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRPARAF";

        /// <summary>
        /// Paraf bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            KOD = 0,
            KULLANICIKODU = 1,
            AD = 2,
        }

        /// <summary>
        /// Paraf bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "KULLANICIKODU", "AD" };

        /// <summary>
        /// Paraf detay bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADDETAY = "EVRPARAFDETAY";

        /// <summary>
        /// Paraf detay bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarDetay
        {
            PARAFKODU = 0,
            SIRANO = 1,
            AD = 2,
            SOYAD = 3,
            UNVAN = 4,
        }

        /// <summary>
        /// Paraf detay bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLARDETAY = { "PARAFKODU", "SIRANO", "AD", "SOYAD", "UNVAN" };

        /// <summary>
        /// Parametre olarak verilen paraf nesnesine göre; paraf bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgi">Paraf bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(Paraf bilgi)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.kullaniciKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ")";

            sonuc = KomutCalistir(sql, 1);
            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KOD], bilgi.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], bilgi.kullaniciKod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", (bilgi.kod)));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", (bilgi.kullaniciKod)));

                sonuc = KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen parafDetay nesne koleksiyonuna göre; paraf detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgiler">DagitimAlan nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc KaydetDetay(ArrayList bilgiler)
        {
            Sonuc sonuc = new Sonuc(true, "", "");
            string sql = "";

            foreach (ParafDetay bilgi in bilgiler)
            {
                sql = "INSERT INTO " + TABLOADDETAY + " (" + VeriErisim.TumAlanlar("", ALANLARDETAY) + ") VALUES (";
                sql += VeriErisim.DegerYap(bilgi.parafKodu) + ",";
                sql += VeriErisim.DegerYap(bilgi.siraNo) + ",";
                sql += VeriErisim.DegerYap(bilgi.ad) + ",";
                sql += VeriErisim.DegerYap(bilgi.soyad) + ",";
                sql += VeriErisim.DegerYap(bilgi.unvan) + ")";

                sonuc = KomutCalistir(sql, 1);
                if (!sonuc.islemSonuc)
                    break;
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen kullanıcı kodu ve paraf kod bilgisine göre; paraf bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kod">Paraf kod bilgisini içeren nesne</param>
        /// <param name="kullaniciKod">Kullanıcı kodu bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(int kod, string kullaniciKod)
        {
            string sql = "";
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (kod > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", kod));
            if (kullaniciKod != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KULLANICIKODU], "=", kullaniciKod));
            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen paraf kod listesine göre; paraf detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="kodList">Paraf kod listesini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader ListeleDetay(string kodList)
        {
            string sql = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOADDETAY, ALANLARDETAY) + " FROM " + TABLOADDETAY;
            if (kodList == "")
                kodList = "0";
            sql += " WHERE (" + TABLOADDETAY + "." + ALANLARDETAY[(int)ENUMAlanlarDetay.PARAFKODU] + " IN (" + kodList + ")) ";
            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen paraf kod bilgisine göre; paraf detay bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="parafKodu">Paraf kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc SilDetay(int parafKodu)
        {
            string sql = "DELETE FROM " + TABLOADDETAY;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLARDETAY[(int)ENUMAlanlarDetay.PARAFKODU], "=", parafKodu));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen paraf kod bilgisine göre; paraf bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="parafKodu">Paraf kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(int parafKodu)
        {
            Sonuc sonuc = new Sonuc(true, "", "");

            sonuc = SilDetay(parafKodu);
            if (sonuc.islemSonuc)
            {
                string sql = "DELETE FROM " + TABLOAD;
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", parafKodu));
                return KomutCalistir(sql, 1);
            }
            return sonuc;
        }

        /// <summary>
        /// Veritabanına yeni paraf bilgisi kaydetmek için, sql cümlesi hazırlayıp çalıştırarak yeni bir paraf kodu oluşturan yordam.
        /// </summary>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader KodAl()
        {
            string sql = "SELECT MAX(" + ALANLAR[(int)ENUMAlanlar.KOD] + ") + 1 FROM " + TABLOAD;
            return veriErisim.DataReaderDondur(sql);
        }

    }
}
