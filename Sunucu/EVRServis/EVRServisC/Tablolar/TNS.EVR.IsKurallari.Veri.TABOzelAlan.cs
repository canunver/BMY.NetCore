using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Özel alan bilgilerini içeren EVROZELALAN veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABOzelAlan : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABOzelAlan sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABOzelAlan(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Özel alan bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVROZELALAN";

        /// <summary>
        /// Özel alan bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar
        {
            ALANKODU = 0,
            EVRAKBIRIMKODU = 1,
            DOSYAKODU = 2,
            SIRANO = 3,
            AD = 4,
        }

        /// <summary>
        /// Özel alan bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "ALANKODU", "EVRAKBIRIMKODU", "DOSYAKODU", "SIRANO", "AD" };

        /// <summary>
        /// Parametre olarak verilen özel alan bilgisine göre; özel alan bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="bilgi">Özel alan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(OzelAlan bilgi)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bilgi.alanKod) + ",";
            sql += VeriErisim.DegerYap(bilgi.evrakBirim.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.dosya.kod) + ",";
            sql += VeriErisim.DegerYap(bilgi.sira) + ",";
            sql += VeriErisim.DegerYap(bilgi.ad) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.DOSYAKODU], bilgi.dosya.kod) + ",";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.SIRANO], bilgi.sira);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALANKODU], "=", bilgi.alanKod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen özel alan bilgisine göre; özel alan bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="bilgi">The bilgi.</param>
        /// <returns>
        /// İşlem sonucu Sonuc nesnesi olarak döndürülür.
        /// </returns>
        public Sonuc Sil(OzelAlan bilgi)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.ALANKODU], "=", bilgi.alanKod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paramete olarak verilen özel alan bilgisine göre; özel alan bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Özel alan bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(OzelAlan bilgi)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + "," + TABDosyaPlani.TABLOAD + ".AD FROM " + TABLOAD;
            sql += "  LEFT OUTER  JOIN " + TABDosyaPlani.TABLOAD + " ON " + TABLOAD + ".DOSYAKODU = " + TABDosyaPlani.TABLOAD + ".KOD ";

            if (bilgi.evrakBirim.kod.Trim() != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.EVRAKBIRIMKODU], "=", bilgi.evrakBirim.kod));
            if (bilgi.dosya.kod.Trim() != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.DOSYAKODU], "=", bilgi.dosya.kod));
            if (bilgi.ad.Trim() != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], "=", bilgi.ad));
            if (bilgi.sira > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SIRANO], "=", bilgi.sira));
            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO] + "," + ALANLAR[(int)ENUMAlanlar.ALANKODU];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}