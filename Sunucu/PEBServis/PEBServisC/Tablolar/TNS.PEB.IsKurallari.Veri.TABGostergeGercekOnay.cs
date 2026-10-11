using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.PEB.IsKurallari.Veri
{
    /// <summary>
    /// Performans göstergesinin gerçekleşen değerlerinin onaylı olup olmadığını tutan PEBGOSTERGEGERCEKONAY
    /// veri tabanı tablosu ile ilgili listeleme, yazma ve diğer sql işlemlerini tutan sınıf
    /// </summary>
    public class TABGostergeGercekOnay : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGostergeGercekOnay(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Tablo adını tutan sabit değişken
        /// </summary>
        public static string TABLOAD = "PEBGOSTERGEGERCEKONAY";

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
            /// GGKURKOD alanının ALANLAR dizisindeki sıra numarası (indisi)
            /// </summary>
            GGKURKOD = 4
        }

        /// <summary>
        /// Performans göstergesinin gerçekleşen değerlerinin onaylı olup olmadığının tutulduğu tablonun alanlarının isimlerini tutan sabit dizi
        /// </summary>
        public static string[] ALANLAR = { "YIL", "PHKURKOD", "PERHEDEFKOD", "AY", "GGKURKOD" };

        /// <summary>
        /// Performans göstergesinin gerçekleşen değerlerinin onay bilgilerini veri tabanına ekleyen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="phKurKod">Performans hedef kurumsal kodu</param>
        /// <param name="phKod">Performans hedef kodu</param>
        /// <param name="ay">Ay kriteri</param>
        /// <param name="ggKurKod">Performans gösterge gerçekleşen kurumsal kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Kaydet(int yil, string phKurKod, int phKod, int ay, string ggKurKod)
        {
            string sql = "INSERT INTO " + TABLOAD + " (" + VeriErisim.TumAlanlar("", ALANLAR) + ") VALUES (";
            sql += VeriErisim.DegerYap(yil) + ",";
            sql += VeriErisim.DegerYap(phKurKod.Replace(".", "")) + ",";
            sql += VeriErisim.DegerYap(phKod) + ",";
            sql += VeriErisim.DegerYap(ay) + ",";
            sql += VeriErisim.DegerYap(ggKurKod.Replace(".", "")) + ")";
            return KomutCalistir(sql, 1);
        }

        /// <summary>
        /// Parametre olarak verilen kriterleri kullanarak, ilgili
        /// performans göstergesinin gerçekleşen değerlerinin onay bilgilerini veri tabanından silen
        /// sql cümlesini oluşturup, çalıştıran ve sonucunu döndüren yordam
        /// </summary>
        /// <param name="yil">Yıl kriteri</param>
        /// <param name="phKurKod">Performans hedef kurumsal kodu</param>
        /// <param name="phKod">Performans hedef kodu</param>
        /// <param name="ay">Ay kriteri</param>
        /// <param name="ggKurKod">Performans gösterge gerçekleşen kurumsal kodu</param>
        /// <returns>İşlemin sonucunun başarılı olup olmadığı ve başarılı değilse sebebi Sonuc yapısında döndürülür.</returns>
        public Sonuc Sil(int yil, string phKurKod, int phKod, int ay, string ggKurKod)
        {
            string sql = "DELETE FROM " + TABLOAD;
            sql = VeriErisim.KosulEkle(sql, "WHERE", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.YIL], "=", yil));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PHKURKOD], "=", phKurKod.Replace(".", "")));
            sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], "=", phKod));
            if (ay > 0)
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.AY], "=", ay));
            if (!string.IsNullOrEmpty(ggKurKod))
                sql = VeriErisim.KosulEkle(sql, "AND", VeriErisim.KosulOl(TABLOAD, ALANLAR[(int)ENUMAlanlar.GGKURKOD], "=", ggKurKod.Replace(".", "")));
            return KomutCalistir(sql, 0);
        }

        public bool OnayliMi(int yil, string phKurKod, int phKod, int ay, string ggKurKod)
        {
            string[] strAlan = { ALANLAR[(int)ENUMAlanlar.PHKURKOD], ALANLAR[(int)ENUMAlanlar.GGKURKOD] };
            string[] strDeger = { phKurKod.Replace(".", ""), ggKurKod.Replace(".", "") };
            string[] intAlan = { ALANLAR[(int)ENUMAlanlar.YIL], ALANLAR[(int)ENUMAlanlar.PERHEDEFKOD], ALANLAR[(int)ENUMAlanlar.AY] };
            int[] intDeger = { yil, phKod, ay };
            return TABGenel.KayitVarmi(veriErisim, TABLOAD, strAlan, strDeger, intAlan, intDeger);
        }
    }
}