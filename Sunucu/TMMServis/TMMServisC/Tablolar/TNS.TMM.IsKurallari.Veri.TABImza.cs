using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Taşınır belgelerini imzalayacak olan kişilerin imza bilgilerinin tutulduğu TASIMZA veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABImza : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABImza(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASIMZA";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Muhasebe kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 0,
            /// <summary>
            /// Harcama birim kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            HARCAMABIRIMKOD = 1,
            /// <summary>
            /// Ambar kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AMBARKOD = 2,
            /// <summary>
            /// İmza yeri alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            IMZAYER = 3,
            /// <summary>
            /// Ad alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 4,
            /// <summary>
            /// Ünvan alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            UNVAN = 5,
            /// <summary>
            /// Görev Ünvanı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            GOREVUNVAN = 6,
            /// <summary>
            /// Tarih alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TARIH = 7,
            /// <summary>
            /// TcKimlikNo alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            TCKIMLIKNO = 8,
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR = { "MUHASEBEKOD", "HARCAMABIRIMKOD", "AMBARKOD", "IMZAYER", "AD", "UNVAN", "GOREVUNVAN", "TARIH", "TCKIMLIKNO" };

        /// <summary>
        /// İmza bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="imza">İmza bilgilerini tutan, İmzaBilgisi türünde nesne</param>
        /// <returns>Ekleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc ImzaKaydet(ImzaBilgisi imza)
        {
            Sonuc sonuc = new Sonuc(false, "", "");

            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(imza.muhasebe.Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.birim.Replace(".", "").Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.ambar.Trim()) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.imzaYer) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.adSoyad) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.unvan) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.gorevUnvan) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.tarih) + ", ";
            sql = sql + VeriErisim.DegerYap(imza.tcKimlikNo) + ") ";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Muhasebe, Birim ve Ambar bilgisini kullanarak
        /// ilgili İmza bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="birim">Birim bilgisini tutan, String türünde nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan, String türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc ImzaSil(string muhasebe, string birim, string ambar)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", birim.Replace(".", "").Trim()) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", ambar.Trim());

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Muhasebe, Birim, Ambar ve İmza Yeri bilgisini kullanarak
        /// ilgili İmza bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="birim">Birim bilgisini tutan, String türünde nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan, String türünde nesne</param>
        /// <param name="imzaYer">İmza yeri bilgisini tutan, String türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc ImzaSil(string muhasebe, string birim, string ambar, int imzaYer)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD + " WHERE ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", birim.Replace(".", "").Trim()) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", ambar.Trim()) + " AND ";
            sql += VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IMZAYER], "=", imzaYer);

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen Muhasebe, Birim, Ambar ve İmza Yeri bilgilerini kıstas olarak kullanarak,
        /// kıstasa uygun İmza bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="muhasebe">Muhasebe bilgisini tutan, String türünde nesne</param>
        /// <param name="birim">Birim bilgisini tutan, String türünde nesne</param>
        /// <param name="ambar">Ambar bilgisini tutan, String türünde nesne</param>
        /// <param name="imzaYer">İmza yeri bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun İmza bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader ImzaListele(string muhasebe, string birim, string ambar, int imzaYer)
        {
            string sql = string.Empty;
            string kosul = string.Empty;

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (!string.IsNullOrEmpty(muhasebe))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD], "=", muhasebe));

            if (!string.IsNullOrEmpty(birim))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD], "=", birim.Replace(".", "").Trim()));

            if (!string.IsNullOrEmpty(ambar))
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AMBARKOD], "=", ambar.Trim()));

            if (imzaYer > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.IMZAYER], "=", imzaYer));

            if (!string.IsNullOrEmpty(kosul))
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.MUHASEBEKOD] + "," + ALANLAR[(int)ENUMAlanlar.HARCAMABIRIMKOD] + "," + ALANLAR[(int)ENUMAlanlar.AMBARKOD] + "," + ALANLAR[(int)ENUMAlanlar.IMZAYER];

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
