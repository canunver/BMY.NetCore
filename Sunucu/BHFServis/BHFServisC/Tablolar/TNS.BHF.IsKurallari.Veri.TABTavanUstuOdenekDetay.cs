using System;
using TNS.Veri;
using System.Data;
using TNS;

namespace TNS.BHF.IsKurallari.Veri
{
    /// <summary>
    /// Tavan üstüne sığmayan ödeneklerin 03.2 ve 03.5 ekonomik kodlu olanlarının detaylarını tutan BHFTAVANUSTUODENEKDETAY veri tabanı tablosu ile ilgili, listeleme ve yazma işlemleri tutan sınıf.
    /// </summary>
    public class TABTavanUstuOdenekDetay : TABOrtak
    {
        /// <summary>
        /// Tavan üstüne sığmayan ödenekleri detay bazda tutan tablonun adını tutan sabit değişken.
        /// </summary>
        static string TABLOAD = "BHFTAVANUSTUODENEKDETAY";

        /// <summary>
        /// Tavan üstüne sığmayan ödenekleri detay bazda tutan tablonun alanlarının sırasını tutan enumeration.
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

            /// <summary>
            /// GEREKCE alanının alanlar dizisindeki ve sonuç kümesindeki sıra numarası (indisi).
            /// </summary>
            GEREKCE = 5,
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödenekleri detay bazda tutan tablonun alanlarının isimlerini tutan sabit dizi.
        /// </summary>
        static string[] ALANLAR = { "YIL", "KURUM", "KURKOD", "EKOKOD", "ODENEK", "GEREKCE" };

        /// <summary>
        /// Sınıfın oluşturucu (constructer) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne.</param>
        public TABTavanUstuOdenekDetay(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tavan üstüne sığmayan ödeneklerin verilen birim için detaylarını silen sql cümlelerini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Silinecek Tavan üstüne sığmayan ödeneklerin yıl bilgisi.</param>
        /// <param name="kurum">Tavan üstüne sığmayan ödeneklerin kurum kodu.</param>
        /// <param name="birim">Tavan üstüne sığmayan ödeneklerin kurumsal kodu.</param>
        /// <returns>Silme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string kurum, string birim)
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
        /// Tavan üstüne sığmayan ödenek bilgisini ekleyen sql cümlesini hazırlayıp, çalıştıran; sonucunu Sonuc yapısında döndüren yordam.
        /// </summary>
        /// <param name="yil">Hizmet gerekçesinin yıl bilgisi.</param>
        /// <param name="kurum">Tavan üstüne sığmayan ödeneğin kurum kodu.</param>
        /// <param name="birim">Tavan üstüne sığmayan ödeneğin kurumsal kodu.</param>
        /// <param name="ekokod">Tavan üstüne sığmayan ödeneğin ekonomik kodu.</param>
        /// <param name="odenek">Tavan üstüne sığmayan ödenek miktarı.</param>
        /// <param name="gerekce">Tavan üstüne sığmayan ödeneğin gerekçe metni.</param>
        /// <returns>Ekleme sql komutu çalıştırılır, işlemin sonucu Sonuc yapısında döndürülür.</returns>
        public Sonuc Ekle(int yil, string kurum, string birim, string ekokod, double odenek, string gerekce)
        {
            string sql;
            sql = "INSERT INTO " + TABLOAD + "(" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql = sql + VeriErisim.DegerYap(yil) + ", ";
            sql = sql + VeriErisim.DegerYap(kurum.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(birim.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(ekokod.Replace(".", "")) + ", ";
            sql = sql + VeriErisim.DegerYap(odenek) + ", ";
            sql = sql + VeriErisim.DegerYap(gerekce) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Tavan üstüne sığmayan ödenekleri döndüren sql cümlesini oluşturup, çalıştıran ve sonucunu DataReader olarak döndüren yordam.
        /// </summary>
        /// <param name="yil">Listelenecek tavan üstüne sığmayan ödeneklerin yıl bilgisi.</param>
        /// <param name="kurum">Listelenecek tavan üstüne sığmayan ödeneklerin kurum kodu.</param>
        /// <param name="birim">Listelenecek tavan üstüne sığmayan ödeneklerin kurumsal kodu.</param>
        /// <returns>Oluşturulan seçme sql komutunun çalıştırılmasının sonucu DataReader olarak döndürülür.</returns>
        public IDataReader Listele(int yil, string kurum, string birim)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.EKOKOD] + ", SUM(" + ALANLAR[(int)ENUMAlanlar.ODENEK] + ") FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurum.Replace(".", "")));
            if (birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", birim.Replace(".", "") + "%"));
            sql = sql + " GROUP BY " + ALANLAR[(int)ENUMAlanlar.EKOKOD];
            sql = sql + " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }

        public IDataReader GerekceListele(int yil, string kurum, string birim, string ekoKod)
        {
            string sql = "SELECT " + ALANLAR[(int)ENUMAlanlar.KURKOD] + ", " + ALANLAR[(int)ENUMAlanlar.GEREKCE] + " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURUM], "=", kurum.Replace(".", "")));
            if (birim != "")
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "LIKE", birim.Replace(".", "") + "%"));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.EKOKOD], "LIKE", ekoKod.Replace(".", "") + "%"));
            sql = sql + " ORDER BY 1";

            return veriErisim.DataReaderDondur(sql);
        }
    }
}
