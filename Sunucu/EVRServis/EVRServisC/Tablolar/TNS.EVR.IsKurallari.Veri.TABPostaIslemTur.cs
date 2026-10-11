using System;
using System.Data;
using TNS.Veri;

namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Posta işlem tür bilgilerini içeren EVRPOSTAISLEMTUR veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABPostaIslemTur : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABPostaIslemTur sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABPostaIslemTur(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Posta işlem tür bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRPOSTAISLEMTUR";

        /// <summary>
        /// Posta işlem tür bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar
        {
            KOD = 0,
            AD = 1,
        }

        /// <summary>
        /// Posta işlem tür bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD" };

        /// <summary>
        /// Parametre olarak verilen tanimlama nesne koleksiyonuna göre; posta işlem tür bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Tanimlama bilgi in liste.objeler)
            {
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql = sql + VeriErisim.DegerYap(bilgi.kod) + ", ";
                sql = sql + VeriErisim.DegerYap(bilgi.ad) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                {
                    veriErisim.hata = "";

                    sql = "UPDATE " + TABLOAD + " SET ";
                    sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bilgi.ad);
                    sql += " WHERE " + VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

                    sonuc = KomutCalistir(sql, 1);
                }

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen tanimlama nesne koleksiyonuna göre; posta işlem tür bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="liste">Tanimlama nesne koleksiyonu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(ObjectArray liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            foreach (Tanimlama bilgi in liste.objeler)
            {
                string sql = "";

                sql = "DELETE FROM " + TABLOAD + " WHERE ";
                sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod);

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }

            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen tanimlama nesnesine göre; posta işlem tür bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="bilgi">Tanımla bilgisi içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(Tanimlama bilgi)
        {
            string sql = "";
            string kriter = "";

            sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR) + " FROM " + TABLOAD;
            if (bilgi.kod > 0)
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], "=", bilgi.kod));

            if (bilgi.ad != "")
                kriter = VeriErisim.KosulEkle(kriter, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + bilgi.ad + "%"));

            if (kriter != "")
                sql += " WHERE " + kriter;

            sql += " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
