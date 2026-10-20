using System;
using System.IO;
using System.Text;

namespace Istemci.OrtakSayfa
{
    /// <summary>
    /// Tarayıcıda saklanan menünün geçerli olup olmadığını anlamak için kısa bir sürüm değeri. Menüyü değiştiren iki şeyden üretilir:
    /// menu.xml'in değişme zamanı ve modül durumları (sistem değişkeni /MODUL/x, KYM/Moduller ekranı).
    /// Her sayfa bu değeri &lt;meta name="menu-surum"&gt; olarak basar (_Layout); menü verisi de aynı değeri taşır (MenuUclari).
    /// İkisi farklıysa menu.js menüyü sunucudan yeniden okur. Böylece modül açılıp kapanınca ya da menu.xml değişince
    /// bütün kullanıcıların menüsü bir sonraki sayfada kendiliğinden yenilenir.
    /// </summary>
    public static class MenuSurum
    {
        public static string Deger(string uygulamaKlasoru)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string dosya = MenuOkuyucu.DosyaYolu(uygulamaKlasoru);
                sb.Append(File.Exists(dosya) ? File.GetLastWriteTimeUtc(dosya).Ticks : 0);
                // Değişken değerleri önbellekten gelir (Ayar.Degisken → DEG.Arac.DegiskenDegerBulCache); yazınca önbellek temizlenir.
                foreach (string modul in TNS.KYM.Arac.moduller)
                    sb.Append('|').Append(Ayar.Degisken(0, "/MODUL/" + modul).Trim());
                return Ozet(sb.ToString());
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>FNV-1a 32 bit özet; yalnızca değişikliği anlamak için (güvenlik amaçlı değil).</summary>
        private static string Ozet(string metin)
        {
            uint h = 2166136261;
            foreach (char c in metin)
            {
                h ^= c;
                h *= 16777619;
            }
            return h.ToString("x8");
        }
    }
}
