using System.Data;
using TNS.Veri;

namespace TNS.ATM.IsKurallari.Veri
{
    /// <summary>
    /// Faiz Tür bilgilerini tutan ATMFAIZTUR veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan nesne
    /// </summary>
    public class TABATMFaizTur : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABATMFaizTur(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "ATMFAIZTUR";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Yıl alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            YIL = 0,
            /// <summary>
            /// Kod alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 1,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 2,
            /// <summary>
            /// Oran alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            ORAN = 3
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "YIL", "KOD", "AD", "ORAN" };

        /// <summary>
        /// FaizTur yapısında tutulan bilgileri veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="faizTur">Bilgileri tutan Faiztur türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(FaizTur faizTur)
        {
            string[] kodYilStr = { ALANLAR[(int)ENUMAlanlar.YIL] };
            int[] kodYilInt = { faizTur.yil };
            int kod = TNS.Veri.Sayac.SayacDegerAl(veriErisim, TABLOAD, ALANLAR[(int)ENUMAlanlar.KOD], new string[0], new string[0], kodYilStr, kodYilInt);

            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(faizTur.yil) + ",";
            sql += VeriErisim.DegerYap(kod) + ",";
            sql += VeriErisim.DegerYap(faizTur.ad) + ",";
            sql += VeriErisim.DegerYap(faizTur.oran) + ")";

            Sonuc sonuc = KomutCalistir(sql, 1);
            sonuc.anahtar = kod.ToString();

            return sonuc;
        }


        /// <summary>
        /// FaizTur yapısında tutulan bilgileri veri tabanında güncelleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="faizTur">Bilgilerin tutulduğu FaizTur türünde nesne</param>
        /// <returns>Güncelle komutunun çalıştırılması sonucunda Sonuc türünde nesne döndürür</returns>
        public Sonuc Guncelle(FaizTur faizTur)
        {
            string sql = "UPDATE " + TABLOAD + " SET ";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], faizTur.ad) + ",";
            sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.ORAN], faizTur.oran);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", faizTur.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", faizTur.kod));

            return KomutCalistir(sql, 1);
        }


        /// <summary>
        /// Yıl ve Ad anahtar alanlarını kullanarak veri tabanından kayıtları listeleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="ad">Ad bilgisini tutan String türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda IDataReader yapısında nesne döndürür</returns>
        public IDataReader Listele(int yil, string ad)
        {
            string sql = "";
            if (yil > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));

            if (!string.IsNullOrEmpty(ad))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], "LIKE", "%" + ad + "%"));

            sql = VeriErisim.KosulEkle("SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD, "WHERE", sql);

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.YIL] + " DESC, "+ALANLAR[(int)ENUMAlanlar.AD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Yıl ve Kod anahatar alanlarını kullanarak TNS.Veri.Sayac.KayitVarmi fonksiyonunun çalıştırarak
        /// sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="yil">Yıl bilgisini tutan Int türünde nesne</param>
        /// <param name="kod">Kod bilgisini tutan Int türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda Bool türünde nesne döndürür</returns>
        public bool FaizTurVarMi(int yil, int kod)
        {
            string[] intAlan ={ ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.KOD] };
            int[] intDeger ={ yil, kod };
            return TNS.Veri.Sayac.KayitVarmi(veriErisim, TABLOAD, new string[0], new string[0], intAlan, intDeger);
        }
    }
}
