using System;
using System.Data;
using TNS.Veri;

namespace TNS.HRC.IsKurallari.Veri
{
    /// <summary>
    /// Ölçü birim bilgisini tutan HRCOLCUBIRIM veri tabanı tablosu ile ilgili, 
    /// listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABOlcuBirim : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABOlcuBirim(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Ölçü birim bilgisi tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "HRCOLCUBIRIM";

        /// <summary>
        /// Ölçü birim bilgisi tablo alanlarının sırasını tutan enumeration
        /// </summary>
        enum ENUMAlanlar
        {
            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// AD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AD = 1,
        }

        /// <summary>
        /// Ölçü birim tablosu alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD" };

        /// <summary>
        /// Ölçü birim bilgisini veri tabanından çeken sql cümleciğini verilen kıstasa göre
        /// üretip, çalıştırıp, datareader döndüren yordam.
        /// </summary>
        /// <returns>
        /// Ölçü birim bilgisini bulmak için oluşturulan select komutunun çalıştırılması sonucu elde edilen
        /// DataReader nesnesini döndürür
        /// </returns>
        public IDataReader Listele()
        {
            string sql = "";


            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;


            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        public string OlcuBirimAdBul(int birimKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.AD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", birimKod));
            return this.veriErisim.TekKolonSQLStr(sql);
        }
    }
}