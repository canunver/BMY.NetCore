using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Performans göstergesinin gerçekleşen değerlerini tutan PEBGOSTERGEGERCEK veri tabanı
    /// tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABGostergeGercek : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGostergeGercek(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBGOSTERGEGERCEK";

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
            /// PHKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PHKURKOD = 1,

            /// <summary>
            /// PERHEDEFKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            PERHEDEFKOD = 2,

            /// <summary>
            /// AY alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            AY = 3,

            /// <summary>
            /// GOSTERGEKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GOSTERGEKOD = 4,

            /// <summary>
            /// MIKTAR alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            MIKTAR = 5,

            /// <summary>
            /// KAYNAK alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            KAYNAK = 6,

            /// <summary>
            /// SONUC alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SONUC = 7,

            /// <summary>
            /// SAPMA alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            SAPMA = 8,

            /// <summary>
            /// ONLEM alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            ONLEM = 9,

            /// <summary>
            /// GGKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GGKURKOD = 10
        }

        /// <summary>
        /// Performans göstergesinin gerçekleşen değerlerinin tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "PHKURKOD", "PERHEDEFKOD", "AY", "GOSTERGEKOD", "MIKTAR",
                                   "KAYNAK", "SONUC", "SAPMA", "ONLEM", "GGKURKOD" };

        /// <summary>
        /// Performans göstergesinin gerçekleşen değerlerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="gGercek">Performans göstergesi için girilen gerçekleşen değerlerin üst bilgilerini tutan nesne</param>
        /// <param name="gGercekDeger">Performans göstergesi için girilen gerçekleşen değerleri tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(PerformansGostergeGerceklesen gGercek, PerformansGostergeGerceklesenDeger gGercekDeger)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(gGercek.yil) + ",";
            sql += VeriErisim.DegerYap(gGercek.phKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(gGercek.perHedefKod) + ",";
            sql += VeriErisim.DegerYap(gGercek.ay) + ",";
            sql += VeriErisim.DegerYap(gGercekDeger.gosterge.gostergeKod) + ",";
            sql += (gGercekDeger.veriYok ? "null" : VeriErisim.DegerYap(gGercekDeger.miktar)) + ",";
            sql += VeriErisim.DegerYap(gGercekDeger.kaynak) + ",";
            sql += VeriErisim.DegerYap(gGercekDeger.sonuc) + ",";
            sql += VeriErisim.DegerYap(gGercekDeger.sapma) + ",";
            sql += VeriErisim.DegerYap(gGercekDeger.onlem) + ",";
            sql += VeriErisim.DegerYap(gGercek.ggKurKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kullanarak, ilgili
        /// performans göstergesinin gerçekleşen değerlerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="gGercek">Performans göstergesi için girilen gerçekleşen değerlerin üst bilgilerini tutan nesne</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(PerformansGostergeGerceklesen gGercek, PerformansGostergeGerceklesenDeger gGercekDeger = null)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", gGercek.yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PHKURKOD], "=", gGercek.phKurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", gGercek.perHedefKod));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", gGercek.ay));
            //Performans Hedef silinince buraya geliyor, bütün perHedef silinmeli birim bazında değil
            if (!string.IsNullOrEmpty(gGercek.ggKurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GGKURKOD], "=", gGercek.ggKurKod.Replace(".", "")));
            if (gGercekDeger != null && gGercekDeger.gosterge.gostergeKod > 0)
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GOSTERGEKOD], "=", gGercekDeger.gosterge.gostergeKod));
            }
            return KomutCalistir(sql, 0);
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kıstas olarak kullanarak,
        /// select sql cümlesini oluşturup, çalıştıran ve performans göstergesinin
        /// gerçekleşen değerlerini DataReader yapısında döndüren yordam
        /// </summary>
        /// <param name="gGercek">Performans göstergesi için girilen gerçekleşen değerlerin üst bilgilerini tutan nesne</param>
        /// <returns>Verilen kriterlere uygun olan performans göstergesinin gerçekleşen değerlerini tutan DataReader nesnesi döndürülür.</returns>
        public IDataReader Listele(PerformansGostergeGerceklesen gGercek)
        {
            string sql = "SELECT " + VeriErisim.TumAlanlar(TABLOAD, ALANLAR);
            sql += " FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", gGercek.yil));
            if (!string.IsNullOrEmpty(gGercek.phKurKod))
            {
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PHKURKOD], "=", gGercek.phKurKod.Replace(".", "")));
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", gGercek.perHedefKod));
            }
            if (gGercek.ay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", gGercek.ay));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GGKURKOD], "LIKE", gGercek.ggKurKod.Replace(".", "") + "%"));
            return veriErisim.DataReaderDondur(sql);
        }
    }
}