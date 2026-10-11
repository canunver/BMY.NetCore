using System.Data;
using TNS.Veri;

namespace TNS.TMM.IsKurallari.Veri
{
    /// <summary>
    /// Bölge bilgilerini tutan TASBOLGE veri tabanı tablosu ile ilgili  silme, listeleme ve yazma işlemleri tutan sınıf
    /// </summary>
    public class TABBolge : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBolge(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        static string TABLOAD = "TASBOLGE";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar
        {
            /// <summary>
            /// Bölge kodu alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            KOD = 0,
            /// <summary>
            /// Bölge adı alanının select cümleceği sonrasında, DataReaderdaki sıra numarası (indisi)
            /// </summary>
            AD = 1
        }

        /// <summary>
        /// Tablo alanlarının isimlerini tutan sabit dizi
        /// </summary>
        static string[] ALANLAR ={ "KOD", "AD" };

        /// <summary>
        /// Bölge bilgilerini veri tabanına kaydeden
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bolge">Bölge bilgilerini tutan, Bölge türünde nesne</param>
        /// <returns>Ekleme,Ekleme yapılamazsa Güncelleme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Kaydet(Bolge bolge)
        {
            Sonuc sonuc = new Sonuc(false, "", "");
            string sql = "";

            sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(bolge.kod) + ",";
            sql += VeriErisim.DegerYap(bolge.ad) + ")";

            sonuc = KomutCalistir(sql, 1);

            if (!sonuc.islemSonuc)
            {
                veriErisim.hata = "";
                sql = "UPDATE " + TABLOAD + " SET ";
                sql += VeriErisim.AtamaOl("", ALANLAR[(int)ENUMAlanlar.AD], bolge.ad);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bolge.kod));

                sonuc = KomutCalistir(sql, 1);
            }

            return sonuc;
        }

        /// <summary>
        /// Parametre olarak verilen Bölge bilgisini kullanarak
        /// ilgili Bölge bilgilerini silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bolge">Bölge bilgilerini tutan, Bölge türünde nesne</param>
        /// <returns>Silme komutunun çalıştırılması sonucunda, işlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebini tutan Sonuc türünde nesne döndürür</returns>
        public Sonuc Sil(Bolge bolge)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bolge.kod));

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Bölge bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Bölge bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bolge">Bölge bilgilerini tutan, Bölge türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Bölge bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader Listele(Bolge bolge)
        {
            string sql = "";
            string kosul = "";

            sql = "SELECT " + VeriErisim.TumAlanlar("", ALANLAR) + " FROM " + TABLOAD;

            if (bolge.kod > 0)
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KOD], "=", bolge.kod));

            if (bolge.ad != "")
                kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AD], " LIKE ", "%" + bolge.ad + "%"));

            if (kosul != "")
                sql += " WHERE " + kosul;

            sql += " ORDER BY " + ALANLAR[(int)ENUMAlanlar.KOD];

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Taşınır Muhasebe tablosunda, verilen bölge kodu bilgisini güncelleyen,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bolgeKod">Bölge kodu bilgisini tutan, Int türünde nesne</param>
        /// <param name="muhasebe">Taşınır Muhasebe bilgilerini tutan, TNSCollection türünde nesne</param>
        public void BolgeMuhasebeEslestir(int bolgeKod, TNSCollection muhasebe)
        {
            string sql = "";

            foreach (Muhasebe m in muhasebe)
            {
                sql = "UPDATE TASMUHASEBE SET ";
                sql += VeriErisim.AtamaOl("", "BOLGEKOD", bolgeKod);
                sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "KOD", "=", m.kod));

                KomutCalistir(sql, 1);
            }
        }

        /// <summary>
        /// Taşınır Muhasebe tablosunda, verilen bölge kodu bilgisine 0 atayan,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bolgeKod">Bölge kodu bilgisini tutan, Int türünde nesne</param>
        public void BolgeyeAitMuhasebeleriSifirla(int bolgeKod)
        {
            string sql = "";

            sql = "UPDATE TASMUHASEBE SET ";
            sql += VeriErisim.AtamaOl("", "BOLGEKOD", 0);
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "BOLGEKOD", "=", bolgeKod));

            KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen Bölge kodu bilgisini kıstas olarak kullanarak,
        /// kıstasa uygun Taşınır Muhasebe bilgilerini döndüren,
        /// sql cümlesini oluşturup,çalıştıran ve sonucunu döndüren fonksiyon
        /// </summary>
        /// <param name="bolgeKod">Bölge kodu bilgisini tutan, Int türünde nesne</param>
        /// <returns>Select komutunun çalıştırılması sonucunda, verilen kriterlere uygun Taşınır Muhasebe bilgilerini tutan IDataReader türünde nesne döndürür</returns>
        public IDataReader BolgeyeAitMuhasebeler(int bolgeKod)
        {
            string sql = "";

            sql = "SELECT * FROM TASMUHASEBE";
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", "BOLGEKOD", "=", bolgeKod));

            sql += " ORDER BY KOD";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}