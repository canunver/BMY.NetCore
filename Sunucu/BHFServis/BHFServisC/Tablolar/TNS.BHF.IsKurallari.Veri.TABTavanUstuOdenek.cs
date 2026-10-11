using System;
using TNS.Veri;
using System.Data;
using TNS;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Birimlerin tavan üstü ödenek ihtiyaçlarının açıklamalarının tutulduğu BHFTAVANUSTUACIKLAMA ile bu açıklama için girilen ekonomik kod bazında ödeneklerin tutulduğu BHFTAVANUSTUODENEK veri tabanı tabloları ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABTavanUstuOdenek : TABOrtak
    {
        /// <summary>
        /// Birimin girdiği tavan üstü ödenek ihtiyaçlarını ekonomik kod bazında tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOAD = "BHFTAVANUSTUODENEK";

        /// <summary>
        /// Birimin girdiği tavan üstü ödenek ihtiyaçlarını ekonomik kod bazında tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURUM alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURUM = 1,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURKOD = 2,

            /// <summary>
            /// EKOKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            EKOKOD = 3,

            /// <summary>
            /// ODENEK alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ODENEK = 4,
        }

        /// <summary>
        /// Birimin girdiği tavan üstü ödenek ihtiyaçlarını ekonomik kod bazında tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURUM", "KURKOD", "EKOKOD", "ODENEK" };

        /// <summary>
        /// Birimin girdiği tavan üstü ödenek ihtiyaçlarının açıklamalarını tutan tablonun adını tutan sabit değişken.
        /// </summary>
        public static string TABLOADACIKLAMA = "BHFTAVANUSTUACIKLAMA";

        /// <summary>
        /// Birimin girdiği tavan üstü ödenek ihtiyaçlarının açıklamalarını tutan tablonun alanlarının sırasını tutan enumeration.
        /// </summary>
        public enum ENUMAlanlarAciklama : int
        {
            /// <summary>
            /// YIL alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// ACIKLAMA alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            ACIKLAMA = 2,

        }

        /// <summary>
        /// Birimin girdiği tavan üstü ödenek ihtiyaçlarının açıklamalarını tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        public static string[] ALANLARACIKLAMA = { "YIL", "KURKOD", "ACIKLAMA" };

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABTavanUstuOdenek(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki tavan üstü ödenek ihtiyaçlarının ödeneklerini silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Tavan üstü ödenek ihtiyaçlarının ödenekleri silinecek yıl.</param>
        /// <param name="kurum">Tavan üstü ödenek ihtiyaçlarının ödenekleri silinecek kurum.</param>
        /// <param name="birim">Tavan üstü ödenek ihtiyaçlarının ödenekleri silinecek birim.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc OdenekSil(int yil, string kurum, string birim)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurum.Replace(".", "")));
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", birim.Replace(".", "")));
            sql = "DELETE FROM " + TABLOAD + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Verilen birimin, ilgili yıldaki tavan üstü ödenek ihtiyaçlarının açıklamalarını silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Tavan üstü ödenek ihtiyaçlarının açıklamaları silinecek yıl.</param>
        /// <param name="birim">Tavan üstü ödenek ihtiyaçlarının açıklamaları silinecek birim.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc AciklamaSil(int yil, string birim)
        {
            string kosul, sql;
            Sonuc sonuc;
            kosul = VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil);
            kosul = VeriErisim.KosulEkle(kosul, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", birim.Replace(".", "")));
            sql = "DELETE FROM " + TABLOADACIKLAMA + " WHERE " + kosul;
            sonuc = KomutCalistir(sql, 0);
            return sonuc;
        }

        /// <summary>
        /// Verilen birimin, ekonomik kod bazında tavan üstü ödenek ihtiyacını ekonomik kod bazında ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Tavan üstü ödenek ihtiyacı eklenecek yıl.</param>
        /// <param name="kurum">Tavan üstü ödenek ihtiyacı eklenecek kurum.</param>
        /// <param name="birim">Tavan üstü ödenek ihtiyacı eklenecek birim.</param>
        /// <param name="ekokod">Eklenecek tavan üstü ödeneğin ekonomik kodu.</param>
        /// <param name="odenek">Eklenecek tavan üstü ödeneğin miktarı.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc OdenekEkle(int yil, string kurum, string birim, string ekokod, double odenek)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurum.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(birim.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(ekokod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(odenek) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Verilen birimin, ekonomik kod bazında tavan üstü ödenek ihtiyacının açıklamasını ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Tavan üstü ödenek ihtiyacı açıklaması eklenecek yıl.</param>
        /// <param name="birim">Tavan üstü ödenek ihtiyacı açıklaması eklenecek birim.</param>
        /// <param name="aciklama">Tavan üstü ödenek ihtiyacının açıklama metni.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc AciklamaEkle(int yil, string birim, string aciklama)
        {
            string sql;
            sql = "INSERT INTO " + TABLOADACIKLAMA + "(" + VeriErisim.TumAlanlar("", ALANLARACIKLAMA) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(birim.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(aciklama) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Bir birime ait tavan üstü ödenek ihtiyaçlarının açıklamalarını ve ekonomik kod bazında miktarlarını döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// Açıklama ve ödenek tabloları birleştirilir. Açıklama bilgisi her ekonomik kod için tekrarlanmış olacaktır.
        /// </summary>
        /// <param name="yil">Tavan üstü ödenek ihtiyacı listelenecek yıl.</param>
        /// <param name="kurum">Tavan üstü ödenek ihtiyacı listelenecek kurum.</param>
        /// <param name="birim">Tavan üstü ödenek ihtiyacı listelenecek birim.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(int yil, string kurum, string birim)
        {
            string sql;

            sql = "SELECT  A.YIL,A.KURUM,A.KURKOD,A.EKOKOD,B.ACIKLAMA,A.ODENEK FROM  BHFTAVANUSTUODENEK A,  BHFTAVANUSTUACIKLAMA B ";
            sql = VeriErisim.KosulEkle(sql, " WHERE A.KURKOD = B.KURKOD AND A.YIL = B.YIL AND ", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurum.Replace(".", "")));
            if (birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("A", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", birim.Replace(".", "") + "%"));
            sql = sql + " ORDER BY 1,2,3";
            return veriErisim.DataReaderDondur(sql);
        }
    }
}
