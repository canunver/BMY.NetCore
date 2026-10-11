using System.Resources;

namespace Istemci
{
    /// <summary>
    /// Çok dilli metinler. Metinler Kaynak klasöründeki .resx dosyalarından, isteğin diline göre okunur;
    /// seçili dilde karşılığı olmayan metin için Türkçesi (dil eki olmayan dosya) kullanılır.
    ///   OrtakClassRes : eski OrtakClass kaynakları (menü adları dahil; anahtar = menu.xml'deki ID)
    ///   Menu          : yeni arayüzün kendi metinleri
    ///   Duyuru        : eski Duyuru kaynağı ve duyuru sayfalarının yeni metinleri
    ///   EkliDosya     : ortak ekli dosyalar bileşeni
    ///   Kullanici     : eski Kullanici kaynağı (kullanıcı arama / giriş sayfaları) ve yeni metinler
    ///   Muhasebe      : muhasebe tanım sayfaları (eski ButceMuhasebe kaynağından gerekenler ve yeni metinler)
    /// </summary>
    public static class Metin
    {
        private static readonly ResourceManager ortak = new ResourceManager("Istemci.Kaynak.OrtakClassRes", typeof(Metin).Assembly);
        private static readonly ResourceManager menu = new ResourceManager("Istemci.Kaynak.Menu", typeof(Metin).Assembly);
        private static readonly ResourceManager duyuru = new ResourceManager("Istemci.Kaynak.Duyuru", typeof(Metin).Assembly);
        private static readonly ResourceManager ekliDosya = new ResourceManager("Istemci.Kaynak.EkliDosya", typeof(Metin).Assembly);
        private static readonly ResourceManager kullanici = new ResourceManager("Istemci.Kaynak.Kullanici", typeof(Metin).Assembly);
        private static readonly ResourceManager muhasebe = new ResourceManager("Istemci.Kaynak.Muhasebe", typeof(Metin).Assembly);

        private static string Oku(ResourceManager kaynak, string anahtar, string varsayilan)
        {
            if (string.IsNullOrEmpty(anahtar)) return varsayilan;
            try
            {
                string metin = kaynak.GetString(anahtar);
                return string.IsNullOrEmpty(metin) ? varsayilan : metin;
            }
            catch (MissingManifestResourceException)
            {
                return varsayilan;
            }
        }

        /// <summary>Eski OrtakClassRes kaynağından metin; yoksa varsayılan.</summary>
        public static string Ortak(string anahtar, string varsayilan = null)
        {
            return Oku(ortak, anahtar, varsayilan);
        }

        /// <summary>Menu kaynağından metin; yoksa anahtarın kendisi.</summary>
        public static string Menu(string anahtar)
        {
            return Oku(menu, anahtar, anahtar);
        }

        /// <summary>Duyuru kaynağından metin; yoksa anahtarın kendisi.</summary>
        public static string Duyuru(string anahtar)
        {
            return Oku(duyuru, anahtar, anahtar);
        }

        /// <summary>Kullanici kaynağından metin; yoksa anahtarın kendisi.</summary>
        public static string Kullanici(string anahtar)
        {
            return Oku(kullanici, anahtar, anahtar);
        }

        /// <summary>Muhasebe kaynağından metin; yoksa anahtarın kendisi.</summary>
        public static string Muhasebe(string anahtar)
        {
            return Oku(muhasebe, anahtar, anahtar);
        }

        /// <summary>EkliDosya kaynağından metin; yoksa anahtarın kendisi.</summary>
        public static string EkliDosya(string anahtar)
        {
            return Oku(ekliDosya, anahtar, anahtar);
        }
    }
}
