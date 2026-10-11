using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Performans hedeflerin stratejik hedeflerle bağını kuran ağırlık
    /// bilgilerini tutan PEBPERHEDEFAGIRLIK veri tabanı tablosu
    /// ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABPerHedefAgirlik : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABPerHedefAgirlik(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBPERHEDEFAGIRLIK";

        /// <summary>
        /// Tablo alanlarının sırasını tutan enumeration
        /// </summary>
        public enum ENUMAlanlar : int
        {
            /// <summary>
            /// YIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            YIL = 0,

            /// <summary>
            /// KURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KURKOD = 1,

            /// <summary>
            /// PERHEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PERHEDEFKOD = 2,

            /// <summary>
            /// DONEMBYIL alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            DONEMBYIL = 3,

            /// <summary>
            /// STRHEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            STRHEDEFKOD = 4,

            /// <summary>
            /// AGIRLIK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AGIRLIK = 5
        }

        /// <summary>
        /// Performans hedefin ağırlık bilgilerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "KURKOD", "PERHEDEFKOD", "DONEMBYIL", "STRHEDEFKOD", "AGIRLIK" };

        /// <summary>
        /// Performans hedefin ağırlık bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="hAgirlik">Performans hedefin ağırlık bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(PerformansHedefAgirlik hAgirlik)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(hAgirlik.yil) + ",";
            sql += VeriErisim.DegerYap(hAgirlik.kurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(hAgirlik.perHedefKod) + ",";
            sql += VeriErisim.DegerYap(hAgirlik.strHedef.donemBYil) + ",";
            sql += VeriErisim.DegerYap(hAgirlik.strHedef.strHedefKod) + ",";
            sql += VeriErisim.DegerYap(hAgirlik.agirlik) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen performans hedef ağırlık nesnesinin yılını,
        /// birim kodunu ve performans hedef programatik kodunu kullanarak,
        /// ilgili performans hedefin ağırlık bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="hAgirlik">Performans hedefin ağırlık bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(PerformansHedefAgirlik hAgirlik)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.YIL], "=", hAgirlik.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.KURKOD], "=", hAgirlik.kurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl("", ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", hAgirlik.perHedefKod));
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen yılı, birim kodunu ve performans hedef programatik kodunu
        /// kıstas olarak kullanarak, select sql cümlesini oluşturup, çalıştıran
        /// ve performans hedefe ait ağırlıkları DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="kurKod">Birim kodu</param>
        /// <param name="perHedefKod">Performans hedefin programatik kodu</param>
        /// <returns>Verilen kriterlere uygun olan performans hedef ağırlıklarını tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(int yil, string kurKod, int perHedefKod)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;

            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            if (!string.IsNullOrEmpty(kurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.KURKOD], "=", kurKod.Replace(".", "")));
            if (perHedefKod > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", perHedefKod));

            return veriErisim.DataReaderDondur(sql);
        }
    }
}