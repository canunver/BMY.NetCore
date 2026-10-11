using System;
using System.Data;
using TNS.Veri;

namespace TNS.STR.IsKurallari.Veri
{
    /// <summary>
    /// Stratejik plan modülü veri tabanı erişim sınıflarının ortak kullandığı yordamları tutan sınıf
    /// </summary>
    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu (constructor) fonksiyonu, veri erişiminde kullanacağı nesneyi parametre olarak alır.
        /// </summary>
        /// <param name="veriErisim">Veri tabanı erişiminde kullanılacak nesne</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Parametre olarak gönderilen tabloda verilen kriterlere uygun olan kayıt var mı kontrolü yapan yordam
        /// </summary>
        /// <param name="ve">Veri tabanı erişiminde kullanılacak nesne</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="whereStrAlan">String tipinde veri tutan alan adları</param>
        /// <param name="whereStrDeger">String tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <param name="whereIntAlan">Sayı tipinde veri tutan alan adları</param>
        /// <param name="whereIntDeger">Sayı tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <returns>Kayıt varsa true, yoksa false döndürülür.</returns>
        public static bool KayitVarmi(VeriErisim ve, string tablo, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            return TNS.Veri.Sayac.KayitVarmi(ve, tablo, whereStrAlan, whereStrDeger, whereIntAlan, whereIntDeger);
        }

        /// <summary>
        /// Verilen tablonun anahtar alanı için sayaç değeri döndüren yordam
        /// Ek parametrelerde verilen kriterler bazında tablonun anahtar alanındaki maksimum değeri bulup 1 ekler.
        /// </summary>
        /// <param name="ve">Veri tabanı erişiminde kullanılacak nesne</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="alan">Alan adı</param>
        /// <param name="whereStrAlan">String tipinde veri tutan alan adları</param>
        /// <param name="whereStrDeger">String tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <param name="whereIntAlan">Sayı tipinde veri tutan alan adları</param>
        /// <param name="whereIntDeger">Sayı tipinde veri tutan alanlara eşitlenecek değerler</param>
        /// <returns>Üretilen yeni sayaç değeri döndürülür.</returns>
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            return TNS.Veri.Sayac.SayacDegerAl(ve, tablo, alan, whereStrAlan, whereStrDeger, whereIntAlan, whereIntDeger);
        }

        //public static int SayacDegerAl(VeriErisim ve, string tablo, string alan)
        //{
        //    return SayacDegerAl(ve, tablo, alan, new string[0], new string[0], new string[0], new int[0]);
        //}
    }
}
