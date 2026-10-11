using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// İhtiyaç Fazlası Taşınır Malzeme Form üst bilgileri tutan TASACIKPAZARFORM veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABAcikPazarForm : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABAcikPazarForm(VeriErisim veriErisim)
            : base(veriErisim)
        {
        }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASACIKPAZARFORM";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// Harcama Birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 1
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD" };

        /// <summary>
        /// İhtiyaç Fazlası Taşınır Malzeme Form üst bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="apf">İhtiyaç Fazlası Taşınır Malzeme Form üst bilgilerini tutan, AcikPazarForm türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(AcikPazarForm apf)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(apf.muhasebeKod) + ",";
            sql += VeriErisim.DegerYap(apf.harcamaKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Paremetre olarak verilen İhtiyaç Fazlası Taşınır Malzeme Form üst bilgilerini kullanarak 
        /// İlgili İhtiyaç Fazlası Taşınır Malzeme Form bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="apf">İhtiyaç Fazlası Taşınır Malzeme From üst bilgilerini tutan, AcikPazarForm türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(AcikPazarForm apf)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", apf.muhasebeKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", apf.harcamaKod.Replace(".", "")));
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen İhtiyaç Fazlası Taşınır Malzeme Form bilgilerini kullanarak,
        /// İhtiyaç Fazlası Taşınır Malzeme Form bilgilerinin kayıtlı olup olmadığını,
        /// KayıtVarMı rutinini çalıştırarak bulan ve sonucunu döndüren fonksiyon.
        /// </summary>
        /// <param name="apf">İhtiyaç Fazlası Taşınır Malzeme Form bilgilerini tutan, AcikPazarForm türünde nesne</param>
        /// <returns>Fonksiyonun çalıştırılması sonucunda, kayıt var ise true yoksa false değerini veren Bool türünde nesne döndürür</returns>
        public bool AcikPazarFormVarMi(AcikPazarForm apf)
        {
            string[] strAlan ={ ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] };
            string[] strDeger ={ apf.muhasebeKod, apf.harcamaKod.Replace(".", "") };

            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, new string[0], new int[0]);
        }
    }
}
