using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Durum bilgilerini tutan ATMDURUM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMDurum : TABOrtak
    {

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMDurum(VeriErisim veriErisim): base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMDURUM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "KOD", "AD" };

        
        /// <summary>
        /// Hepsimi anahtar alanını kullanarak veri tabanında kayıtları listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="hepsiMi">Hepsimi bilgisini tutan Bool türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(bool hepsiMi)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!hepsiMi)
            {
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], ">=", (int)ENUMDurum.OdemeYapildi));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "<=", (int)ENUMDurum.Af));
            }

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }


        /// <summary>
        /// Kod anahtar alanını kullanarak TNS.Veri.Sayac.KayitVarmi fonksiyonunun çalıştırarak
        /// sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="kod">Kod bilgisini tutan Int türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda Bool türünde nesne döndürür</returns>
        public bool DurumVarMi(int kod)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.KOD] };
            int[] intDeger ={ kod };
            return TNS.Veri.Sayac.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }
    }
}
