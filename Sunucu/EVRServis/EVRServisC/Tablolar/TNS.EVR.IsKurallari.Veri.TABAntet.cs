using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Antet bilgilerini içeren EVRANTET veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABAntet : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABAntet sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABAntet(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Antet bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRANTET";

        /// <summary>
        /// Antet bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        enum ENUMAlanlar
        {
            EVRAKBIRIMKODU = 0,
            ANTET = 1,
        }

        /// <summary>
        /// Antet bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "EVRAKBIRIMKODU", "ANTET" };

        /// <summary>
        /// Birim bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOADEVRBIRIM = "EVRBIRIM";

        /// <summary>
        /// Birim bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlarEvrBirimKodu
        {
            KOD = 0,
            AD = 1,
        }

        /// <summary>
        /// Birim bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAREVRBIRIM = { "KOD", "AD" };

        /// <summary>
        /// Parametre olarak verilen Antet nesne koleksiyonuna göre; antet bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Antet nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Antet bilgi in liste.objeler)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.evrakBirimKodu) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.antet) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ANTET], bilgi.antet);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", bilgi.evrakBirimKodu);

                    sonuc = KomutCalistir(sql, 1);
                }

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Antet nesne koleksiyonuna göre; antet bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Antet nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Antet bilgi in liste.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", bilgi.evrakBirimKodu);

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen Antet nesnesine göre; antet bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// <param name="bilgi">Antet bilgilerini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(Antet bilgi)
        {
            string sql = "";
            string kriter = "";

            sql = "SELECT " + ALANLAREVRBIRIM[(int)ENUMAlanlarEvrBirimKodu.KOD] + "," + ALANLAREVRBIRIM[(int)ENUMAlanlarEvrBirimKodu.AD] + "," + ALANLAR[(int)ENUMAlanlar.ANTET] + " FROM " + TABLOAD;
            sql += " RIGHT OUTER JOIN ";
            sql += TABLOADEVRBIRIM + " ON " + ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU] + " = " + ALANLAREVRBIRIM[(int)ENUMAlanlarEvrBirimKodu.KOD];
            if (bilgi.evrakBirimKodu != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOADEVRBIRIM, ALANLAREVRBIRIM[(int)ENUMAlanlarEvrBirimKodu.KOD], "=", bilgi.evrakBirimKodu));

            if (kriter != "")
                sql += " WHERE " + kriter;
            sql += " ORDER BY 1";

            //sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            //if (bilgi.kod != "")
            //    kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));
            //if (kriter != "")
            //    sql += " WHERE " + kriter;
            //sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);

        }
    }
}
