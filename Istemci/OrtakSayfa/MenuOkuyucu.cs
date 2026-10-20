using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TNS;
using TNS.KYM;

namespace Istemci.OrtakSayfa
{
    /// <summary>Menüde bir satır: bağlantı ("a"), bölüm başlığı ("b") ya da ayırıcı çizgi ("c").</summary>
    public class MenuSatir
    {
        public string t { get; set; }
        /// <summary>Bağlantının menu.xml'deki ID'si; sık kullanılanlar bu ID ile saklanır (eski MenuSikKullanilan.aspx gibi).</summary>
        public string id { get; set; }
        public string ad { get; set; }
        public string url { get; set; }
        /// <summary>Bağlantı yeni pencerede açılır (menu.xml: AltPencere).</summary>
        public bool yeni { get; set; }
        /// <summary>Core'a taşınma durumu (menu.xml: Durum): Yok / Yeni / Eski. Menüde renkle gösterilir.</summary>
        public string durum { get; set; }
    }

    /// <summary>Sol menüdeki başlık (menu.xml: Tip="UstMenu") ve altındaki seçenekler.</summary>
    public class MenuBaslik
    {
        public string id { get; set; }
        public string ad { get; set; }
        /// <summary>wwwroot/img/menu altındaki ikon dosyası (menu.xml: Ikon). Boşsa adın iki harfi gösterilir.</summary>
        public string ikon { get; set; }
        /// <summary>Doluysa başlık panel açmaz, doğrudan bu adrese gider.</summary>
        public string url { get; set; }
        public List<List<MenuSatir>> kolonlar { get; set; } = new List<List<MenuSatir>>();
        /// <summary>
        /// Seçenekleri ayrı sayfada gösterilen menünün (SayfadaGoster) bağlantıları. Menüde çizilmez; yalnızca açık sayfanın
        /// adını bulmak ve sık kullanılanlar için kullanılır.
        /// </summary>
        public List<MenuSatir> ekSatirlar { get; set; }
    }

    /// <summary>Menü grubu (menu.xml: Tip="Grup"); sol menünün başındaki seçiciden değiştirilir.</summary>
    public class MenuGrup
    {
        public string id { get; set; }
        public string ad { get; set; }
        /// <summary>wwwroot/img/menu altındaki ikon dosyası (menu.xml: Ikon). Boşsa adın iki harfi gösterilir.</summary>
        public string ikon { get; set; }
        public List<MenuBaslik> menuler { get; set; } = new List<MenuBaslik>();
    }

    /// <summary>
    /// menu.xml dosyasını okuyup kullanıcının görebileceği seçeneklere indirger (eski GenelSayfa.XMLMenuHazirla).
    /// Seçenek adları isteğin diline göre kaynak dosyalarından gelir (Metin.Ortak).
    /// Kurallar eskisiyle aynıdır:
    ///   - Gizli niteliği dolu olan düğüm ve altındakiler alınmaz.
    ///   - Modul niteliği doluysa, sayılan modüllerden en az biri açık olmalıdır (sistem değişkeni /MODUL/x = 1 ya da 2).
    ///   - Yetki niteliği doluysa, kullanıcının tiplerinden biri listede olmalı ya da "YT|damga|değer" koşulu sağlanmalıdır.
    ///   - Kullanici niteliği doluysa (virgülle ayrılmış kullanıcı kodları), kullanıcının kodu listede olmalıdır (Yetki ile birlikte aranır).
    ///   - İçinde görünür seçenek kalmayan grup, başlık ve bölüm gösterilmez.
    /// Dosya okunduktan sonra, kullanıcı "stratek" ise yalnızca ona açık teknik seçenekler koddan eklenir (StratekMenu).
    /// </summary>
    public class MenuOkuyucu
    {
        private readonly string[] kullaniciYetki;
        private readonly HashTableArrayList kullaniciDamga;
        private readonly string kullaniciKodu;
        private readonly Kullanici kullanan;
        private readonly Dictionary<string, bool> modulDurum = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        public MenuOkuyucu(Kullanici kullanan)
        {
            this.kullanan = kullanan;
            kullaniciDamga = kullanan.yetkiTipiDegerleri;
            kullaniciKodu = kullanan.kullaniciKodu ?? "";
            if (kullanan.kullaniciTipi != null && kullanan.kullaniciTipi.Length > 0)
            {
                kullaniciYetki = new string[kullanan.kullaniciTipi.Length];
                for (int i = 0; i < kullanan.kullaniciTipi.Length; i++)
                    kullaniciYetki[i] = ((int)kullanan.kullaniciTipi[i]).ToString();
            }
            else
                kullaniciYetki = new string[0];
        }

        /// <summary>Menü dosyasının yolu. appSettings: MenuXML (göreli ise uygulama klasörüne göre; varsayılan Menu\menu.xml).</summary>
        public static string DosyaYolu(string uygulamaKlasoru)
        {
            string yol = Ayar.Oku("MenuXML", Path.Combine("Menu", "menu.xml"));
            return Path.IsPathRooted(yol) ? yol : Path.Combine(uygulamaKlasoru, yol);
        }

        public List<MenuGrup> Oku(string dosyaYolu)
        {
            List<MenuGrup> gruplar = new List<MenuGrup>();
            if (kullaniciYetki.Length == 0) return gruplar;

            XmlDocument belge = new XmlDocument();
            belge.Load(dosyaYolu);
            StratekMenu.Ekle(belge, kullanan);
            XmlNode kok = belge.DocumentElement;
            if (kok == null) return gruplar;

            foreach (XmlNode dugum in kok.ChildNodes)
            {
                if (dugum.NodeType != XmlNodeType.Element || !Gorunur(dugum)) continue;
                if (Ozellik(dugum, "Tip") != "Grup") continue;

                MenuGrup grup = new MenuGrup();
                grup.id = Ozellik(dugum, "ID");
                grup.ad = Ad(dugum);
                grup.ikon = Ikon(dugum);

                foreach (XmlNode alt in dugum.ChildNodes)
                {
                    if (alt.NodeType != XmlNodeType.Element || !Gorunur(alt)) continue;
                    MenuBaslik baslik = BaslikOku(alt);
                    if (baslik != null) grup.menuler.Add(baslik);
                }

                if (grup.menuler.Count > 0) gruplar.Add(grup);
            }
            return gruplar;
        }

        /// <summary>
        /// Kullanıcı, menüde verilen adresle (URL niteliği; büyük-küçük harf ayrımı yok) tanımlı bir seçeneği görebiliyor mu?
        /// Eski GenelIslemler.SayfayaGirisYapabilirMi'nin karşılığı: aynı sayfanın adresteki parametreye göre farklı
        /// kullanıcı tiplerine açıldığı durumlarda (ör. GenelKodTanimla.aspx?Tur=21) uçlar bununla denetler.
        /// Adres menüde hiç yoksa false döner.
        /// </summary>
        public bool SayfaYetkili(string dosyaYolu, string url)
        {
            if (kullaniciYetki.Length == 0) return false;
            XmlDocument belge = new XmlDocument();
            belge.Load(dosyaYolu);
            StratekMenu.Ekle(belge, kullanan);
            if (belge.DocumentElement == null) return false;
            return SayfaAra(belge.DocumentElement, url);
        }

        private bool SayfaAra(XmlNode ust, string url)
        {
            foreach (XmlNode dugum in ust.ChildNodes)
            {
                // Görünmeyen düğümün altındakiler de görünmez.
                if (dugum.NodeType != XmlNodeType.Element || !Gorunur(dugum)) continue;
                if (string.Equals(Ozellik(dugum, "URL"), url, StringComparison.OrdinalIgnoreCase)) return true;
                if (SayfaAra(dugum, url)) return true;
            }
            return false;
        }

        private MenuBaslik BaslikOku(XmlNode dugum)
        {
            MenuBaslik baslik = new MenuBaslik();
            baslik.id = Ozellik(dugum, "ID");
            baslik.ad = Ad(dugum);
            baslik.ikon = Ikon(dugum);

            // SayfadaGoster: seçenekleri panelde değil ayrı bir sayfada gösterilen (çok kalabalık) menü.
            string sayfadaGoster = Ozellik(dugum, "SayfadaGoster");
            if (sayfadaGoster != "")
            {
                baslik.url = "../OrtakSayfa/SayfaMenu?menuID=" + (sayfadaGoster.StartsWith("Menu", StringComparison.Ordinal) ? sayfadaGoster : baslik.id);
                List<MenuSatir> sayfadakiler = new List<MenuSatir>();
                SatirlariOku(dugum, sayfadakiler);
                baslik.ekSatirlar = sayfadakiler.FindAll(s => s.t == "a");
                return baslik;
            }

            bool altDugumVar = false;
            List<MenuSatir> kolonsuz = new List<MenuSatir>();
            foreach (XmlNode alt in dugum.ChildNodes)
            {
                if (alt.NodeType != XmlNodeType.Element) continue;
                altDugumVar = true;
                if (!Gorunur(alt)) continue;

                if (Ozellik(alt, "Tip") == "Kolon")
                {
                    List<MenuSatir> kolon = new List<MenuSatir>();
                    SatirlariOku(alt, kolon);
                    Toparla(kolon);
                    if (kolon.Count == 0) continue;
                    // Text verilmiş kolonun adı, kolonun başında bölüm başlığı olarak yazılır.
                    string kolonAd = Ad(alt);
                    if (kolonAd != "" && kolon[0].t != "b") kolon.Insert(0, new MenuSatir { t = "b", ad = kolonAd });
                    baslik.kolonlar.Add(kolon);
                }
                else
                    SatirOku(alt, kolonsuz);
            }
            Toparla(kolonsuz);
            if (kolonsuz.Count > 0) baslik.kolonlar.Add(kolonsuz);

            if (baslik.kolonlar.Count > 0) return baslik;

            // Alt seçeneği hiç tanımlanmamış başlık, adresi varsa doğrudan bağlantıdır.
            string url = Ozellik(dugum, "URL");
            if (!altDugumVar && url != "")
            {
                baslik.url = url;
                return baslik;
            }
            return null;
        }

        private void SatirlariOku(XmlNode ust, List<MenuSatir> satirlar)
        {
            foreach (XmlNode alt in ust.ChildNodes)
            {
                if (alt.NodeType != XmlNodeType.Element || !Gorunur(alt)) continue;
                SatirOku(alt, satirlar);
            }
        }

        private void SatirOku(XmlNode dugum, List<MenuSatir> satirlar)
        {
            string tip = Ozellik(dugum, "Tip");
            string ad = Ad(dugum);
            string url = Ozellik(dugum, "URL");

            if (tip == "AltBolum" || tip == "Kolon" || AltElemanVar(dugum))
            {
                // Bölüm: başlığı ve altındaki seçenekler. Altında görünür seçenek yoksa başlık da yazılmaz.
                List<MenuSatir> icerik = new List<MenuSatir>();
                SatirlariOku(dugum, icerik);
                Toparla(icerik);
                if (icerik.Count == 0) return;
                if (ad != "") satirlar.Add(new MenuSatir { t = "b", ad = ad });
                satirlar.AddRange(icerik);
                return;
            }

            if (url != "")
                satirlar.Add(new MenuSatir { t = "a", id = Ozellik(dugum, "ID"), ad = ad != "" ? ad : url, url = url, yeni = Ozellik(dugum, "AltPencere") != "", durum = Durum(dugum) });
            else if (ad == "")
                satirlar.Add(new MenuSatir { t = "c" });
        }

        /// <summary>Baştaki, sondaki ve art arda gelen ayırıcıları; ardından seçenek gelmeyen bölüm başlıklarını atar.</summary>
        private static void Toparla(List<MenuSatir> satirlar)
        {
            YapilanlarOne(satirlar, s => s.t == "a", s => s.durum == "Yok");
            for (int i = satirlar.Count - 1; i >= 0; i--)
            {
                bool sonda = i == satirlar.Count - 1;
                if (satirlar[i].t == "c" && (i == 0 || sonda || satirlar[i - 1].t != "a"))
                    satirlar.RemoveAt(i);
                else if (satirlar[i].t == "b" && (sonda || satirlar[i + 1].t == "b"))
                    satirlar.RemoveAt(i);
            }
        }

        /// <summary>
        /// Art arda gelen bağlantılar içinde Core'da yapılmış olanlar (Durum Yeni/Eski) yapılmamışların (Yok) önüne alınır;
        /// her grubun kendi sırası korunur. Bölüm başlığı ve ayırıcılar yerinde kalır. Bkz. SayfaMenuUclari.
        /// </summary>
        public static void YapilanlarOne<T>(List<T> satirlar, Func<T, bool> baglanti, Func<T, bool> yapilmadi)
        {
            int i = 0;
            while (i < satirlar.Count)
            {
                if (!baglanti(satirlar[i])) { i++; continue; }
                int bas = i;
                while (i < satirlar.Count && baglanti(satirlar[i])) i++;
                List<T> parca = satirlar.GetRange(bas, i - bas);
                List<T> sirali = new List<T>(parca.Count);
                foreach (T s in parca) if (!yapilmadi(s)) sirali.Add(s);
                foreach (T s in parca) if (yapilmadi(s)) sirali.Add(s);
                for (int k = 0; k < sirali.Count; k++) satirlar[bas + k] = sirali[k];
            }
        }

        private static bool AltElemanVar(XmlNode dugum)
        {
            foreach (XmlNode alt in dugum.ChildNodes)
                if (alt.NodeType == XmlNodeType.Element) return true;
            return false;
        }

        /// <summary>Düğüm bu kullanıcıya görünür mü (Gizli / Modul / Kullanici / Yetki); üst düğümlere bakılmaz. Bkz. SayfaMenuUclari.</summary>
        public bool DugumGorunur(XmlNode dugum)
        {
            return Gorunur(dugum);
        }

        private bool Gorunur(XmlNode dugum)
        {
            if (Ozellik(dugum, "Gizli") != "") return false;
            if (!ModulAcik(Ozellik(dugum, "Modul"))) return false;
            if (!KullaniciVar(Ozellik(dugum, "Kullanici"))) return false;
            return YetkiVar(Ozellik(dugum, "Yetki"));
        }

        /// <summary>Kullanici niteliği boşsa herkes; doluysa yalnızca listedeki kullanıcı kodları.</summary>
        private bool KullaniciVar(string kullanicilar)
        {
            if (string.IsNullOrEmpty(kullanicilar)) return true;
            foreach (string parca in kullanicilar.Split(','))
                if (parca.Trim() != "" && parca.Trim() == kullaniciKodu) return true;
            return false;
        }

        /// <summary>
        /// Menü seçeneğinin adı: önce isteğin dilindeki kaynak metni (anahtar = ID), yoksa menu.xml'deki Text.
        /// </summary>
        private static string Ad(XmlNode dugum)
        {
            return Metin.Ortak(Ozellik(dugum, "ID"), Ozellik(dugum, "Text"));
        }

        /// <summary>Durum niteliği (Yok / Yeni / Eski); başka değer ya da boşsa null.</summary>
        public static string Durum(XmlNode dugum)
        {
            string d = Ozellik(dugum, "Durum");
            foreach (string s in new[] { "Yok", "Yeni", "Eski" })
                if (string.Equals(d, s, StringComparison.OrdinalIgnoreCase)) return s;
            return null;
        }

        /// <summary>Ikon niteliği; yalnızca dosya adı kabul edilir (klasör içeremez).</summary>
        private static string Ikon(XmlNode dugum)
        {
            string ikon = Ozellik(dugum, "Ikon");
            return ikon != "" && ikon == Path.GetFileName(ikon) ? ikon : null;
        }

        private static string Ozellik(XmlNode dugum, string ad)
        {
            XmlAttribute o = dugum.Attributes != null ? dugum.Attributes[ad] : null;
            return o != null && o.Value != null ? o.Value.Trim() : "";
        }

        /// <summary>Eski GenelIslemlerIstemci.MenuGorunsunmu.</summary>
        private bool ModulAcik(string modulStr)
        {
            if (string.IsNullOrEmpty(modulStr)) return true;
            foreach (string parca in modulStr.Split(','))
            {
                string modul = parca.Trim();
                if (modul == "") continue;

                bool acik;
                if (!modulDurum.TryGetValue(modul, out acik))
                {
                    string durum = Ayar.Degisken(0, "/MODUL/" + modul).Trim();
                    acik = durum == "1" || durum == "2";
                    modulDurum[modul] = acik;
                }
                if (acik) return true;
            }
            return false;
        }

        /// <summary>Eski GenelIslemlerIstemci.MenuyeYetkiliMi.</summary>
        public bool YetkiVar(string menuYetki)
        {
            if (string.IsNullOrEmpty(menuYetki)) return true;

            foreach (string parca in menuYetki.Split(','))
            {
                string y = parca.Trim();
                if (Array.IndexOf(kullaniciYetki, y) >= 0) return true;

                if (y.StartsWith("YT", StringComparison.Ordinal))
                {
                    string[] damgaBilgi = y.Split('|');
                    if (damgaBilgi.Length < 3) return false;
                    if (kullaniciDamga == null) continue;

                    string deger = kullaniciDamga[damgaBilgi[1]];
                    if (string.IsNullOrEmpty(deger)) continue;
                    foreach (string d in deger.Split(';'))
                        if (d.Trim() == damgaBilgi[2]) return true;
                }
            }
            return false;
        }
    }
}
