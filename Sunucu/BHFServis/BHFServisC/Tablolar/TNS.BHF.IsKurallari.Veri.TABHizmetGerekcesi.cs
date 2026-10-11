using System;
using TNS;
using TNS.Veri;
using TNS.BHF.IsKurallari;
using System.Data;
using System.Collections;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Hizmet gerekçesini parçalı olarak tutan BHFHIZMETGEREKCESI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABHizmetGerekcesi : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABHizmetGerekcesi(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Hizmet gerekçesini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFHIZMETGEREKCESI";

        /// <summary>
        /// Hizmet gerekçesini tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURUMSAL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURUMSAL = 1,

            /// <summary>
            /// PARCA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            PARCA = 2,

            /// <summary>
            /// BILGI alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            BILGI = 3,

            /// <summary>
            /// FAALIYETKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            FAALIYETKOD = 4,
        }

        /// <summary>
        /// Hizmet gerekçesini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURUMSAL", "PARCA", "BILGI", "FAALIYETKOD" };

        /// <summary>
        /// Hizmet gerekçesi parçalarını döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Hizmet gerekçesinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet gerekçesinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader HizmetGerekcesiListele(int yil, string kurumsalKod, int faaliyetKod)
        {
            string sql = "";
            sql = "select " + ALANLAR[(int)ENUMAlanlar.PARCA] + ", " + ALANLAR[(int)ENUMAlanlar.BILGI];
            sql = sql + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "=", kurumsalKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", faaliyetKod));
            sql = sql + " order by 1";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Hizmet gerekçesinin bir parçasını ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Hizmet gerekçesinin yıl bilgisi.</param>
        /// <param name="kurumsalKod">Hizmet gerekçesinin kurumsal kodu.</param>
        /// <param name="parcaNo">Hizmet gerekçesi parçasının sıra numarası.</param>
        /// <param name="bilgi">Hizmet gerekçesi parçasının metni.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetGerekcesiKaydet(int yil, string kurumsalKod, int faaliyetKod, int parcaNo, string bilgi)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurumsalKod) + ", ";
            sql = sql + VeriErisim.DegerYap(parcaNo) + ", ";
            sql = sql + VeriErisim.DegerYap(bilgi) + ", ";
            sql = sql + VeriErisim.DegerYap(faaliyetKod) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Hizmet gerekçesi parçalarını silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek hizmet gerekçesinin yıl bilgisi.</param>
        /// <param name="kurumsalKod">Silinecek hizmet gerekçesinin kurumsal kodu.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetGerekcesiSil(int yil, string kurumsalKod, int faaliyetKod)
        {
            string sql = "";

            sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "=", kurumsalKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.FAALIYETKOD], "=", faaliyetKod));

            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hizmet gerekçelerini bir önceki yıldan alıp, bu yıla kopyalayan sql komutunu oluşturup çalıştıran ve sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Birime ait hizmet gerekçesinin kopyalanacağı hedef yıl bilgisi.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesinin kopyalanacağı birimin kurumsal kodu.</param>
        /// <returns>İşlem sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc HizmetGerekcesiKopyalaOncYil(int yil, string kurumsalKod)
        {
            string sql = "INSERT INTO " + TABLOAD + " (";
            sql += ALANLAR[(int)ENUMAlanlar.YIL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.KURUMSAL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PARCA] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.BILGI] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FAALIYETKOD] + ") SELECT ";
            sql += ALANLAR[(int)ENUMAlanlar.YIL] + "+1, ";
            sql += ALANLAR[(int)ENUMAlanlar.KURUMSAL] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.PARCA] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.BILGI] + ", ";
            sql += ALANLAR[(int)ENUMAlanlar.FAALIYETKOD] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil - 1));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "LIKE", kurumsalKod + "%"));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Hizmet gerekçesi girmiş olan birim sayısını bulup döndüren sql komutunu hazırlayıp, çalıştırıp sonucunu döndüren yordam.
        /// </summary>
        /// <param name="yil">Gerekçe sayısı bulunacak yıl.</param>
        /// <param name="kurumsalKod">Birime ait hizmet gerekçesi sayısının bulunacağı birimin kurumsal kodu.</param>
        /// <returns>Hizmet gerekçesi sayısı döndürülür.</returns>
        public int HizmetGerekcesiSayisiBul(int yil, string kurumsalKod)
        {
            string sql = "select COUNT(DISTINCT " + ALANLAR[(int)ENUMAlanlar.KURUMSAL] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "LIKE", kurumsalKod + "%"));

            return veriErisim.TekKolonSQLInt(sql);
        }
    }
}
