using System;
using System.Data;
using TNS;
using TNS.Veri;
using TNS.EVR;
using System.Collections;
namespace TNS.EVR.IsKurallari.Veri
{
    /// <summary>
    /// Şablon dağıtım bilgilerini içeren EVRSABLONDAGITIM veri tabanı tablosu ile ilgili, kaydetme, silme ve listeleme yordamlarını tutan sınıf.    
    /// </summary>
    public class TABSablonDagitim : TABOrtak
    {
        /// <summary>        
        /// Parametre olarak verilen veriErisim nesnesine göre, TABSablonDagitim sınıfının yeni bir örneğini oluşturur.
        /// </summary>
        /// <param name="veriErisim">Veri erişim bilgisini içeren nesne</param>
        public TABSablonDagitim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Şablon dağıtım bilgilerini içeren veri tabanı tablosunun adını tutan değişken.
        /// </summary>
        public static string TABLOAD = "EVRSABLONDAGITIM";

        /// <summary>
        /// Şablon dağıtım bilgilerini içeren veri tabanı tablosunun alanlarının sırasını tutan numaralandırma.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            SABLONKOD = 0, SIRANO = 1, DAGITIMKOD = 2, DAGITIMAD = 3, GEREGIBILGI = 4
        }

        /// <summary>
        /// Şablon dağıtım bilgilerini içeren veri tabanı tablosunun alanlarının isimlerini tutan dizi.
        /// </summary>
        public static string[] ALANLAR = { "SABLONKOD", "SIRANO", "DAGITIMKOD", "DAGITIMAD", "GEREGIBILGI" };

        /// <summary>
        /// Parametre olarak verilen şablon kodu ve sablonDagitim nesne koleksiyonu bilgisine göre; şablon dağıtım bilgisini, sql cümlesi hazırlayıp çalıştırarak veritabanına kaydeden yordam.
        /// </summary>
        /// <param name="sablonKod">Şablon kod bilgisini içeren nesne</param>
        /// <param name="liste">SablonDagitim nesne koleksiyonunu içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Kaydet(int sablonKod, ArrayList liste)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            int siraNo = 0;
            foreach (SablonDagitim form in liste)
            {
                siraNo++;
                string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
                sql += VeriErisim.DegerYap(sablonKod) + ",";
                sql += VeriErisim.DegerYap(siraNo) + ",";
                sql += VeriErisim.DegerYap(form.dagitimKod) + ",";
                sql += VeriErisim.DegerYap(form.dagitimAd) + ",";
                sql += VeriErisim.DegerYap(form.geregiBilgi) + ")";

                sonuc = KomutCalistir(sql, 1);

                if (!sonuc.islemSonuc)
                    return sonuc;
            }
            return sonuc;
        }

        /// <summary>
        /// Paramete olarak verilen şablon koduna göre; şablon dağıtım bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından okuyan yordam.
        /// </summary>
        /// <param name="sablonKod">Şablon kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(int sablonKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.GEREGIBILGI] + "," + ALANLAR[(int)ENUMAlanlar.DAGITIMKOD] + "," + ALANLAR[(int)ENUMAlanlar.DAGITIMAD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABLONKOD], "=", sablonKod));
            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.SIRANO];
            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Parametre olarak verilen şablon kod bilgisine göre; şablon dağıtım bilgilerini, sql cümlesi hazırlayıp çalıştırarak veritabanından silen yordam.
        /// </summary>
        /// <param name="sablonKod">Şablon kod bilgisini içeren nesne</param>
        /// <returns>İşlem sonucu Sonuc nesnesi olarak döndürülür.</returns>
        public Sonuc Sil(int sablonKod)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";
            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.SABLONKOD], "=", sablonKod));

            sonuc = KomutCalistir(sql, 0);

            return sonuc;
        }
    }
}
