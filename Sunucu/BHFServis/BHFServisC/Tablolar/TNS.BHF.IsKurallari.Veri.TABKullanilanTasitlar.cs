using System;
using TNS;
using TNS.Veri;
using TNS.BHF.IsKurallari;
using System.Data;
using System.Collections;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Taşıt genel bilgilerini (Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetler) tutan BHFHIZMETALIMITASITLARI veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// Not: Tablonun ve sınıfın adı, eskiden sadece hizmet alımı ile edinilecek taşıtları tuttuğu için HizmetAlimiTasitlari'dir. Fakat sonradan bu tablo araçlarla ilgili diğer bilgileri de tutar hale gelmiştir.
    /// </summary>
    public class TABKullanilanTasitlar : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABKullanilanTasitlar(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Taşıt genel bilgilerini tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFKULLANILANTASITLAR";

        /// <summary>
        /// Taşıt genel bilgilerini tutan tablonun alanlarının sırasını tutan enumeration.
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
            /// KURUMSAL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            MERKEZTASRA = 2,

            /// <summary>
            /// KOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            TKOD = 3,

            /// <summary>
            /// KAYNAK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KAYNAK = 4,

            /// <summary>
            /// DONERSERMAYE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YAS = 5,

            /// <summary>
            /// FON alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ADET = 6
        }

        /// <summary>
        /// Taşıt genel bilgilerini tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURUMSAL", "MERKEZTASRA", "TKOD", "KAYNAK", "YAS", "ADET" };


        /// <summary>
        /// Mevcut taşıtlar, tasfiye edilecek taşıtlar, hizmet alımı ile kullanılacak taşıt sayısı ve birim maliyetlerini döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Taşıt genel bilgilerinin listeleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin listeleneceği birimin kurumsal kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(int yil, string kurumsalKod, int merkezTasra)
        {
            string sql;
            sql = "select A.TKOD, S.ACIKLAMA, S.DIFERANSIYEL, A.KAYNAK, A.YAS, SUM(A.ADET)";
            sql = sql + " FROM " + TABLOAD + " A ";
            sql = sql + " INNER JOIN BHFTASITLARSOZLUK S ON S.YIL=A.YIL AND S.KOD=A.TKOD  " ;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "LIKE", kurumsalKod.Replace(".", "") + "%"));
            if (merkezTasra < 3)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.MERKEZTASRA], "=", merkezTasra));
            sql = sql + " group by A.TKOD, S.ACIKLAMA, S.DIFERANSIYEL, A.KAYNAK, A.YAS";
            sql = sql + " order by 1, 4, 5";

            return veriErisim.DataReaderDondur(sql);
        }

        /// <summary>
        /// Taşıt genel  bilgilerini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Taşıt genel bilgilerinin ekleneceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin ekleneceği birimin kurumsal kodu.</param>
        /// <param name="tasitGenelBilgi">Taşıt genel bilgilerini tutan nesne.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string kurumsalKod, int merkezTasra, TasitGenelBilgi tasitGenelBilgi)
        {
            string sql = "insert into " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (" +
               VeriErisim.DegerYap(yil) + ", " +
               VeriErisim.DegerYap(kurumsalKod.Replace(".", "")) + ", " +
               VeriErisim.DegerYap(merkezTasra) + ", " +
               VeriErisim.DegerYap(tasitGenelBilgi.tasitSozluk.siraNo) + ", " +
               VeriErisim.DegerYap(tasitGenelBilgi.kaynakKod) + ", " +
               VeriErisim.DegerYap(tasitGenelBilgi.yasKod) + ", " +
               VeriErisim.DegerYap(tasitGenelBilgi.adet) + ")";

            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir birime ait taşıt genel bilgilerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Taşıt genel bilgilerinin silineceği yıl bilgisi.</param>
        /// <param name="kurumsalKod">Taşıt genel bilgilerinin silineceği birimin kurumsal kodu.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurumsalKod, int merkezTasra)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUMSAL], "=", kurumsalKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.MERKEZTASRA], "=", merkezTasra));

            return KomutCalistir(sql, 0);
        }
    }
}
