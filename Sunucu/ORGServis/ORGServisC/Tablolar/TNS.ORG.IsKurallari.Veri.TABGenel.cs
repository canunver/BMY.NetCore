using System;
using System.Collections.Generic;
using System.Text;
using TNS.Veri;

namespace TNS.ORG.IsKurallari.Veri
{
    /// <summary>
    /// İlgili veri tabanı tablosu ile ilgili olarak listeleme, kaydetme ve silme işlemini yapan sınıf
    /// </summary>
    public class TABGenel : TABOrtak
    {
        /// <summary>
        /// Sınıfın oluşturucu(Constructer) fonksiyonu
        /// </summary>
        /// <param name="veriErisim">Veri Erişim tipinde sınıf</param>
        public TABGenel(VeriErisim veriErisim) : base(veriErisim) { }

        /// <summary>
        /// Verilen parametrelerle tabloda kayıtın olup olmadığını döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim tipinde sınıf</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="whereStrAlan">Koşul alanı string array tipinde sınıf</param>
        /// <param name="whereStrDeger">Değer alanı string array tipinde sınıf</param>
        /// <param name="whereIntAlan">Koşul alanı int array tipinde sınıf</param>
        /// <param name="whereIntDeger">Değer alanı int array tipinde sınıf</param>
        /// <returns>Bool tipinde sınıf döndürür</returns>
        public static bool KayitVarmi(VeriErisim ve, string tablo, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            return TNS.Veri.Sayac.KayitVarmi(ve, tablo, whereStrAlan, whereStrDeger, whereIntAlan, whereIntDeger);
        }

        /// <summary>
        /// Verilen parametrelerle tabloda belirtilen alanın sayaç değerini döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim tipinde sınıf</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="alan">Alan adı</param>
        /// <returns>Int tipinde sınıf döndürür</returns>
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan)
        {
            return SayacDegerAl(ve, tablo, alan, new string[0], new string[0], new string[0], new int[0]);
        }

        /// <summary>
        /// Verilen parametrelerle tabloda belirtilen alanın sayaç değerini döndüren fonksiyon
        /// </summary>
        /// <param name="ve">Veri Erişim tipinde sınıf</param>
        /// <param name="tablo">Tablo adı</param>
        /// <param name="alan">Alan adı</param>
        /// <param name="whereStrAlan">Koşul alanı string array tipinde sınıf</param>
        /// <param name="whereStrDeger">Değer alanı string array tipinde sınıf</param>
        /// <param name="whereIntAlan">Koşul alanı int array tipinde sınıf</param>
        /// <param name="whereIntDeger">Değer alanı int array tipinde sınıf</param>
        /// <returns>Int tipinde sınıf döndürür</returns>
        public static int SayacDegerAl(VeriErisim ve, string tablo, string alan, string[] whereStrAlan, string[] whereStrDeger, string[] whereIntAlan, int[] whereIntDeger)
        {
            return TNS.Veri.Sayac.SayacDegerAl(ve, tablo, alan, whereStrAlan, whereStrDeger, whereIntAlan, whereIntDeger);
        }
    }
}
