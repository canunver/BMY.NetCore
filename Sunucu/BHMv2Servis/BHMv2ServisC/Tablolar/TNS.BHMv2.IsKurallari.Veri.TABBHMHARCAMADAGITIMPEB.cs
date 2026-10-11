using System;
using System.Text;
using TNS.Veri;
using System.Data;

namespace TNS.BHMv2.IsKurallari.Veri
{
    /// <summary>
    /// Harcama rakamlarının alt faaliyet bazında tutulduğu BHMODENEKMUHASEBATPEB veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABBHMHARCAMADAGITIMPEB : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABBHMHARCAMADAGITIMPEB(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Alt faaliyet bazında harcamaların tutulduğu tablonun adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "BHMODENEKMUHASEBATPEB";

        /// <summary>
        /// Alt faaliyet bazında harcamaların tutulduğu tablonun alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// AY alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            AY = 1,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// FONKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FONKOD = 3,

            /// <summary>
            /// FINKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            FINKOD = 4,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            EKOKOD = 5,

            /// <summary>
            /// PEBKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            PEBKOD = 6,

            /// <summary>
            /// MUHASEBEKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            MUHASEBEKOD = 7,

            /// <summary>
            /// BIRIMKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            BIRIMKOD = 8,

            /// <summary>
            /// ODENEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi)
            /// </summary>
            ODENEK = 9 
        }

        /// <summary>
        /// Alt faaliyet bazında harcamaların tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public string[] ALANLAR = { "YIL", "AY", "KURKOD", "FONKOD", "FINKOD", "EKOKOD", "PEBKOD", "MUHASEBEKOD", "BIRIMKOD", "ODENEK" };

        /// <summary>
        /// Daha önce sisteme kaydedilmiş alt faaliyet yüzdeleri ile, Harcama tablosunu birleştirip, rakamlarını çarparak
        /// alt faaliyet bazında elde ettiği harcama rakamlarını insert into sql komutu ile
        /// veri tabanına kaydeden yordam.
        /// </summary>
        /// <param name="yil">Alt faaliyetlere dağıtılacak harcama rakamlarının yıl parametresi</param>
        /// <param name="ay">Alt faaliyetlere dağıtılacak harcama rakamlarının ay parametresi</param>
        /// <returns>Dağıtım ve ekleme işlemi yapan sql komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Kaydet(int yil, int ay)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") ( ";
            sql += "SELECT h.YIL, h.AY, h.KURKOD, h.FONKOD, h.FINKOD, h.EKOKOD, y.PEBKOD, h.MUHASEBEKOD, h.BIRIMKOD, h.ODENEK * y.ORAN AS HESAPLANAN ";
            sql += "FROM  BHMODENEKMUHASEBAT as h INNER JOIN BHMODENEKMUHASEBATYUZDE as y ON ";
            sql += "h.AY = y.AY AND h.YIL = y.YIL AND h.KURKOD = y.KURKOD AND h.FONKOD = y.FONKOD AND h.FINKOD = y.FINKOD AND h.EKOKOD = y.EKOKOD ";
            sql += "WHERE (h.YIL=" + yil + " AND h.AY=" + ay + ") )";
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Alt faaliyet bazında harcama rakamlarının eklenmesinden önce, eski veriler silinmelidir.
        /// Silme komutunu hazırlayıp çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek alt faaliyetlere dağıtılmış harcama rakamlarının yıl parametresi</param>
        /// <param name="ay">Silinecek alt faaliyetlere dağıtılacak harcama rakamlarının ay parametresi</param>
        /// <returns>Silme işlemi yapan sql komutunun çalıştırılmasının sonucunu, Sonuc yapısında döndürür</returns>
        public Sonuc Sil(int yil, int ay)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.AY], "=", ay));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Verilen yıl ve aydaki harcama rakamlarının tertipleri alan ve bu tertiplerden yüzde dağıtım
        /// tablosunda yer almayanları bulan sql'i üretip, çalıştırıp, DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Yüzde parametresi girilmemiş tertiplerin listeleneceği yıl bilgisi</param>
        /// <param name="ay">Yüzde parametresi girilmemiş tertiplerin listeleneceği ay bilgisi</param>
        /// <returns>Yüzde parametresi girilmemiş tertipleri bulan sql'i çalıştırılmasının sonucu elde edilen DataReader döndürülür.</returns>
        public IDataReader ListeleYuzdesiOlmayanlar(int yil, int ay)
        {
            string sql = string.Empty;

            sql = "SELECT DISTINCT KURKOD, FONKOD, FINKOD, EKOKOD ";
            sql += "FROM BHMODENEKMUHASEBAT AS h ";
            sql += "WHERE (h.YIL=" + yil + " AND h.AY=" + ay + ") ";
            sql += "AND (NOT EXISTS ";
            sql += "(SELECT YIL, AY, KURKOD, FONKOD, FINKOD, EKOKOD, PEBKOD, ORAN ";
            sql += "FROM BHMODENEKMUHASEBATYUZDE AS y ";
            sql += "WHERE (YIL = h.YIL) AND (AY = h.AY) AND (KURKOD = h.KURKOD) AND (FONKOD = h.FONKOD) AND (FINKOD = h.FINKOD) AND (EKOKOD = h.EKOKOD))) ";
            sql += "GROUP BY KURKOD, FONKOD, FINKOD, EKOKOD ";
            sql += "ORDER BY KURKOD, FONKOD, FINKOD, EKOKOD ";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
